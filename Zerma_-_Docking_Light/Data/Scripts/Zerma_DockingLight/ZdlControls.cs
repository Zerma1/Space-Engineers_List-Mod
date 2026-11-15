using System;
using System.Collections.Generic;
using System.Text;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Interfaces.Terminal;
using VRage.ModAPI;
using VRage.Utils;

namespace Zerma.DockingLight
{
    // Contrôles terminal et actions de barre d'outils ajoutés aux projecteurs (visibles uniquement sur nos blocs (préfixe Zerma_DockingLight_)).
    // IMPORTANT : CreateControls() ne doit être appelé qu'une fois qu'un projecteur existe (depuis ZdlLogic), car le jeu
    // saute la création de TOUS ses contrôles natifs (On/Off, couleur, nom, « afficher dans le terminal », actions…)
    // si la liste de contrôles du type existe déjà (MyTerminalControlFactory.AreControlsCreated).
    public static class ZdlControls
    {
        public const string SubtypePrefix = "Zerma_DockingLight_";
        private const string IconIncrease = @"Textures\GUI\Icons\Actions\Increase.dds";
        private const string IconDecrease = @"Textures\GUI\Icons\Actions\Decrease.dds";
        private const string IconCycle = @"Textures\GUI\Icons\Actions\Toggle.dds";

        private static bool _created;
        private static bool _gettersHooked;

        // Identifiants des contrôles de clignotement natifs de MyLightingBlock (lus dans Sandbox.Game.dll) ;
        // leurs actions natives ont des Id qui contiennent « Blink ». Rien d'autre n'est masqué.
        private static readonly HashSet<string> NativeBlinkIds = new HashSet<string> { "Blink Interval", "Blink Lenght", "Blink Offset" };

        public static bool IsDockingLight(IMyTerminalBlock block)
        {
            return block != null && block.BlockDefinition.SubtypeId != null
                && block.BlockDefinition.SubtypeId.StartsWith(SubtypePrefix);
        }

        private static ZdlLogic Logic(IMyTerminalBlock block)
        {
            if (block == null || block.GameLogic == null) return null;
            ZdlLogic logic = block.GameLogic.GetAs<ZdlLogic>();
            return logic != null && logic.IsReady ? logic : null;
        }

        private static ZdlLimits LimitsOf(IMyTerminalBlock block)
        {
            ZdlLogic logic = Logic(block);
            if (logic != null) return logic.Limits;
            return block != null && block.CubeGrid != null && block.CubeGrid.GridSizeEnum == VRage.Game.MyCubeSize.Small ? ZdlLimits.Small : ZdlLimits.Large;
        }

        // Toute modification (curseur, combobox, propriété PB, action) passe par ici : application locale + synchro réseau.
        private static void Change(IMyTerminalBlock block, Action<ZdlSettings> edit)
        {
            ZdlLogic logic = Logic(block);
            if (logic == null) return;
            ZdlSettings s = logic.Settings.Clone();
            edit(s);
            logic.ChangeSettings(s);
        }

        // Appelé par la session (BeforeStart) : ne crée aucun contrôle, n'ajoute que les filtres d'affichage.
        public static void HookGetters()
        {
            if (_gettersHooked) return;
            MyAPIGateway.TerminalControls.CustomControlGetter += HideNativeBlinkControls;
            MyAPIGateway.TerminalControls.CustomActionGetter += HideNativeBlinkActions;
            _gettersHooked = true;
        }

        public static void Unload()
        {
            if (!_gettersHooked) return;
            MyAPIGateway.TerminalControls.CustomControlGetter -= HideNativeBlinkControls;
            MyAPIGateway.TerminalControls.CustomActionGetter -= HideNativeBlinkActions;
            _gettersHooked = false;
        }

        // Appelé par le premier ZdlLogic initialisé : les contrôles natifs du projecteur existent déjà à ce moment.
        public static void CreateControls()
        {
            if (_created) return;
            _created = true;

            var tc = MyAPIGateway.TerminalControls;

            var sep = tc.CreateControl<IMyTerminalControlSeparator, IMyReflectorLight>("ZermaDL_Separator");
            sep.Visible = IsDockingLight;
            tc.AddControl<IMyReflectorLight>(sep);

            var label = tc.CreateControl<IMyTerminalControlLabel, IMyReflectorLight>("ZermaDL_Label");
            label.Label = MyStringId.GetOrCompute("Ligne d'approche");
            label.Visible = IsDockingLight;
            tc.AddControl<IMyReflectorLight>(label);

            AddSlider("ZermaDL_Distance", "Distance", "Distance du dernier globe, mesurée depuis la vitre du projecteur.",
                l => l.DistanceMin, l => l.DistanceMax,
                s => s.Distance, (s, v) => s.Distance = v, FormatMeters1);
            AddSlider("ZermaDL_Spacing", "Espacement", "Distance entre deux globes.",
                l => l.SpacingMin, l => l.SpacingMax,
                s => s.Spacing, (s, v) => s.Spacing = v, FormatMeters2);
            AddSlider("ZermaDL_GlobeSize", "Taille des globes", "Diamètre des globes.",
                l => l.SizeMin, l => l.SizeMax,
                s => s.GlobeSize, (s, v) => s.GlobeSize = v, FormatMeters2);

            var mode = tc.CreateControl<IMyTerminalControlCombobox, IMyReflectorLight>("ZermaDL_BlinkMode");
            mode.Title = MyStringId.GetOrCompute("Clignotement");
            mode.Tooltip = MyStringId.GetOrCompute("Aucun, général, vague sortante (part de la lampe) ou vague entrante (arrive vers la lampe).");
            mode.Visible = IsDockingLight;
            mode.Enabled = IsDockingLight;
            mode.ComboBoxContent = list =>
            {
                list.Add(new MyTerminalControlComboBoxItem { Key = 0, Value = MyStringId.GetOrCompute("Aucun") });
                list.Add(new MyTerminalControlComboBoxItem { Key = 1, Value = MyStringId.GetOrCompute("Général") });
                list.Add(new MyTerminalControlComboBoxItem { Key = 2, Value = MyStringId.GetOrCompute("Vague sortante") });
                list.Add(new MyTerminalControlComboBoxItem { Key = 3, Value = MyStringId.GetOrCompute("Vague entrante") });
            };
            mode.Getter = b => { ZdlLogic l = Logic(b); return l != null ? l.Settings.BlinkMode : 0L; };
            mode.Setter = (b, v) => Change(b, s => s.BlinkMode = v);
            tc.AddControl<IMyReflectorLight>(mode);

            AddSlider("ZermaDL_BlinkPeriod", "Période", "Période du clignotement, ou durée de parcours d'une vague.",
                l => ZdlLimits.PeriodMin, l => ZdlLimits.PeriodMax,
                s => s.BlinkPeriod, (s, v) => s.BlinkPeriod = v, v => v.ToString("0.0") + " s");
            AddSlider("ZermaDL_BlinkDuty", "Durée allumée / largeur de vague", "Général : part du temps allumée. Vague : largeur de la vague.",
                l => ZdlLimits.DutyMin, l => ZdlLimits.DutyMax,
                s => s.BlinkDuty, (s, v) => s.BlinkDuty = v, v => v.ToString("0") + " %");
            AddSlider("ZermaDL_MinLevel", "Niveau minimum", "0 % = globes cachés entre deux impulsions.",
                l => 0f, l => ZdlLimits.MinLevelMax,
                s => s.MinLevel, (s, v) => s.MinLevel = v, v => v.ToString("0") + " %");

            AddSlider("ZermaDL_OffsetV", "Décalage vertical", "+ = vers le HAUT du bloc (flèche Up en mode construction).",
                l => -l.OffsetMax, l => l.OffsetMax,
                s => s.OffsetV, (s, v) => s.OffsetV = v, FormatSigned);
            AddSlider("ZermaDL_OffsetH", "Décalage horizontal", "+ = vers la DROITE en regardant dans le sens du faisceau.",
                l => -l.OffsetMax, l => l.OffsetMax,
                s => s.OffsetH, (s, v) => s.OffsetH = v, FormatSigned);

            // Choix du pas utilisé par les actions +/- : une combobox par réglage (propriétés PB ZermaDL_*Step, type long = index)
            // + une action « Pas … » qui fait défiler les pas.
            AddStepSelector("Distance", "Pas distance", ZdlLimits.DistanceSteps,
                s => s.DistanceStepIndex, (s, v) => s.DistanceStepIndex = v);
            AddStepSelector("Spacing", "Pas espacement", ZdlLimits.SpacingSteps,
                s => s.SpacingStepIndex, (s, v) => s.SpacingStepIndex = v);
            AddStepSelector("OffsetV", "Pas décalage vertical", ZdlLimits.OffsetSteps,
                s => s.OffsetVStepIndex, (s, v) => s.OffsetVStepIndex = v);
            AddStepSelector("OffsetH", "Pas décalage horizontal", ZdlLimits.OffsetSteps,
                s => s.OffsetHStepIndex, (s, v) => s.OffsetHStepIndex = v);

            // Actions de barre d'outils / cockpit (On/Off, Bascule, intensité, rayon… restent les actions natives du projecteur).
            AddStepActions("Distance", "la distance", (l, s) => l.Step(ZdlLimits.DistanceSteps, s.DistanceStepIndex),
                s => s.Distance, (s, v) => s.Distance = v, FormatMeters1);
            AddStepActions("Spacing", "l'espacement", (l, s) => l.Step(ZdlLimits.SpacingSteps, s.SpacingStepIndex),
                s => s.Spacing, (s, v) => s.Spacing = v, FormatMeters2);
            AddStepActions("OffsetV", "le décalage vertical", (l, s) => l.Step(ZdlLimits.OffsetSteps, s.OffsetVStepIndex),
                s => s.OffsetV, (s, v) => s.OffsetV = v, FormatSigned);
            AddStepActions("OffsetH", "le décalage horizontal", (l, s) => l.Step(ZdlLimits.OffsetSteps, s.OffsetHStepIndex),
                s => s.OffsetH, (s, v) => s.OffsetH = v, FormatSigned);
        }

        private static string FormatStep(float v)
        {
            return v.ToString("0.###") + " m";
        }

        private static void AddStepSelector(string key, string title, float[] table,
            Func<ZdlSettings, long> getIndex, Action<ZdlSettings, long> setIndex)
        {
            var combo = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlCombobox, IMyReflectorLight>("ZermaDL_" + key + "Step");
            combo.Title = MyStringId.GetOrCompute(title);
            combo.Tooltip = MyStringId.GetOrCompute("Pas utilisé par les actions Augmenter / Diminuer (petite grille : moitié).");
            combo.Visible = IsDockingLight;
            combo.Enabled = IsDockingLight;
            // Le contenu n'est pas propre à un bloc : chaque entrée affiche la valeur grande grille et petite grille.
            combo.ComboBoxContent = list =>
            {
                for (int i = 0; i < table.Length; i++)
                    list.Add(new MyTerminalControlComboBoxItem
                    {
                        Key = i,
                        Value = MyStringId.GetOrCompute(FormatStep(table[i] * ZdlLimits.Large.StepScale) + "  (petite grille : " + FormatStep(table[i] * ZdlLimits.Small.StepScale) + ")"),
                    });
            };
            combo.Getter = b => { ZdlLogic l = Logic(b); return l != null ? getIndex(l.Settings) : 0L; };
            combo.Setter = (b, v) => Change(b, s => setIndex(s, v));
            MyAPIGateway.TerminalControls.AddControl<IMyReflectorLight>(combo);

            var cycle = MyAPIGateway.TerminalControls.CreateAction<IMyReflectorLight>("ZermaDL_" + key + "Step_Cycle");
            cycle.Name = new StringBuilder(title);
            cycle.Icon = IconCycle;
            cycle.ValidForGroups = true;
            cycle.Enabled = IsDockingLight;
            cycle.Action = b => Change(b, s => setIndex(s, (getIndex(s) + 1) % table.Length));
            cycle.Writer = (b, sb) =>
            {
                ZdlLogic l = Logic(b);
                if (l != null) sb.Append(FormatStep(l.Limits.Step(table, getIndex(l.Settings))));
            };
            MyAPIGateway.TerminalControls.AddAction<IMyReflectorLight>(cycle);
        }

        private static string FormatMeters1(float v) { return v.ToString("0.0") + " m"; }
        private static string FormatMeters2(float v) { return v.ToString("0.00") + " m"; }
        private static string FormatSigned(float v) { return v.ToString("+0.00#;-0.00#;0.00") + " m"; }

        private static void AddSlider(string id, string title, string tooltip,
            Func<ZdlLimits, float> min, Func<ZdlLimits, float> max,
            Func<ZdlSettings, float> get, Action<ZdlSettings, float> set, Func<float, string> format)
        {
            var c = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlSlider, IMyReflectorLight>(id);
            c.Title = MyStringId.GetOrCompute(title);
            c.Tooltip = MyStringId.GetOrCompute(tooltip);
            c.Visible = IsDockingLight;
            c.Enabled = IsDockingLight;
            c.SetLimits(b => min(LimitsOf(b)), b => max(LimitsOf(b)));
            c.Getter = b => { ZdlLogic l = Logic(b); return l != null ? get(l.Settings) : 0f; };
            c.Setter = (b, v) => Change(b, s => set(s, v));
            c.Writer = (b, sb) => { ZdlLogic l = Logic(b); if (l != null) sb.Append(format(get(l.Settings))); };
            MyAPIGateway.TerminalControls.AddControl<IMyReflectorLight>(c);
        }

        // Deux actions « Augmenter / Diminuer » avec le pas choisi pour ce bloc ; la valeur est bornée par
        // ZdlSettings.Clamp dans ChangeSettings.
        private static void AddStepActions(string key, string label, Func<ZdlLimits, ZdlSettings, float> step,
            Func<ZdlSettings, float> get, Action<ZdlSettings, float> set, Func<float, string> format)
        {
            AddStepAction("ZermaDL_" + key + "_Inc", "Augmenter " + label, IconIncrease, 1f, step, get, set, format);
            AddStepAction("ZermaDL_" + key + "_Dec", "Diminuer " + label, IconDecrease, -1f, step, get, set, format);
        }

        private static void AddStepAction(string id, string name, string icon, float sign, Func<ZdlLimits, ZdlSettings, float> step,
            Func<ZdlSettings, float> get, Action<ZdlSettings, float> set, Func<float, string> format)
        {
            var a = MyAPIGateway.TerminalControls.CreateAction<IMyReflectorLight>(id);
            a.Name = new StringBuilder(name);
            a.Icon = icon;
            a.ValidForGroups = true;
            a.Enabled = IsDockingLight;
            a.Action = b =>
            {
                ZdlLimits limits = LimitsOf(b);
                Change(b, s => set(s, get(s) + sign * step(limits, s)));
            };
            a.Writer = (b, sb) => { ZdlLogic l = Logic(b); if (l != null) sb.Append(format(get(l.Settings))); };
            MyAPIGateway.TerminalControls.AddAction<IMyReflectorLight>(a);
        }

        private static void HideNativeBlinkControls(IMyTerminalBlock block, List<IMyTerminalControl> controls)
        {
            if (!IsDockingLight(block)) return;
            controls.RemoveAll(c => NativeBlinkIds.Contains(c.Id));
        }

        private static void HideNativeBlinkActions(IMyTerminalBlock block, List<IMyTerminalAction> actions)
        {
            if (IsDockingLight(block))
                actions.RemoveAll(a => a.Id != null && a.Id.Contains("Blink"));
            else
                actions.RemoveAll(a => a.Id != null && a.Id.StartsWith("ZermaDL_")); // nos actions n'apparaissent pas sur les autres projecteurs
        }
    }
}
