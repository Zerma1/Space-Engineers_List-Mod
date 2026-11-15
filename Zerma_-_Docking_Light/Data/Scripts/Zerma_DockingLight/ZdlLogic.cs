using System;
using System.Collections.Generic;
using System.Text;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;
using VRageMath;

namespace Zerma.DockingLight
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_ReflectorLight), false, "Zerma_DockingLight_Large", "Zerma_DockingLight_Small")]
    public class ZdlLogic : MyGameLogicComponent
    {
        private const string EmissiveMaterial = "Emissive";
        private const string LightDummy = "light001";
        private static readonly MyStringId HaloMaterial = MyStringId.GetOrCompute("WhiteDot");

        public IMyReflectorLight Block;
        public ZdlSettings Settings;
        public ZdlLimits Limits;
        public bool IsReady;

        private IMyCubeGrid _grid;
        private MyResourceSinkComponent _sink;
        private Func<float> _powerFunc;

        private readonly List<MyEntity> _globes = new List<MyEntity>();
        private int[] _levels = new int[0];          // dernier niveau envoyé au rendu, quantifié sur 16 (-1 = à renvoyer)
        private bool _layoutDirty = true;
        private bool _active;
        private bool _networkDirty;                  // réglage modifié localement, à envoyer (client → serveur ou serveur → clients)
        private Color _lastColor;
        private float _lastIntensity;
        private int _frame;
        private bool _capped;
        private readonly Dictionary<string, IMyModelDummy> _dummies = new Dictionary<string, IMyModelDummy>();

        public int GlobeCount { get { return _globes.Count; } }

        private static bool IsClient { get { return ZdlSession.Instance != null && !ZdlSession.Instance.IsDedicated; } }

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME | MyEntityUpdateEnum.EACH_100TH_FRAME;
        }

        public override void UpdateOnceBeforeFrame()
        {
            // Ici le projecteur existe : ses contrôles natifs sont déjà créés, on peut ajouter les nôtres sans les bloquer.
            if (Entity is IMyReflectorLight) ZdlControls.CreateControls();
            TryInitialize();
        }

        // Une grille sans physique (projection, ou grille collée dont la physique arrive plus tard) : on réessaie toutes les 100 frames.
        private void TryInitialize()
        {
            if (IsReady) return;
            Block = Entity as IMyReflectorLight;
            if (Block == null || Block.CubeGrid == null || Block.CubeGrid.Physics == null || ZdlSession.Instance == null)
                return;

            Limits = Block.CubeGrid.GridSizeEnum == MyCubeSize.Large ? ZdlLimits.Large : ZdlLimits.Small;
            Settings = LoadSettings();
            _grid = Block.CubeGrid;
            _sink = Entity.Components.Get<MyResourceSinkComponent>();
            _powerFunc = ComputeRequiredPower;
            IsReady = true;

            ApplyPower();
            Block.EnabledChanged += OnEnabledChanged;
            Block.IsWorkingChanged += OnWorkingChanged;
            Block.AppendingCustomInfo += OnAppendingCustomInfo;
            ZdlSession.Instance.Register(this);

            NeedsUpdate |= MyEntityUpdateEnum.EACH_10TH_FRAME;
            UpdateFrameFlag();
            Refresh();
        }

        // ---------- Réglages ----------

        private ZdlSettings LoadSettings()
        {
            string text;
            if (Entity.Storage != null && Entity.Storage.TryGetValue(ZdlSession.StorageGuid, out text))
                return ZdlSettings.Deserialize(text, Limits);
            return ZdlSettings.CreateDefault(Limits);
        }

        private void SaveSettings()
        {
            if (Entity.Storage == null) Entity.Storage = new MyModStorageComponent();
            Entity.Storage.SetValue(ZdlSession.StorageGuid, Settings.Serialize());
        }

        // Appelé par un contrôle terminal ou une propriété PB sur la machine où le réglage est modifié.
        // L'envoi réseau est regroupé dans UpdateBeforeSimulation10 (dernière valeur gagnante) pour ne pas
        // émettre un message à chaque cran d'un curseur.
        public void ChangeSettings(ZdlSettings settings)
        {
            settings.Clamp(Limits);
            ApplySettings(settings);
            _networkDirty = true;
        }

        public void ApplyFromNetwork(ZdlSettings settings, bool rebroadcast)
        {
            settings.Clamp(Limits);
            ApplySettings(settings);
            if (rebroadcast) _networkDirty = true;
        }

        private void ApplySettings(ZdlSettings settings)
        {
            Settings = settings;
            SaveSettings();
            _layoutDirty = true;
            ApplyPower();
            UpdateFrameFlag();
            Refresh();
            Block.RefreshCustomInfo();
        }

        private void FlushNetwork()
        {
            if (!_networkDirty) return;
            _networkDirty = false;
            if (ZdlSession.Instance.IsServer) ZdlSession.Instance.Broadcast(this);
            else ZdlSession.Instance.SendToServer(this);
        }

        // ---------- Mises à jour ----------

        public override void UpdateBeforeSimulation10()
        {
            if (!IsReady) return;
            FlushNetwork();
            if (Block.CubeGrid != _grid)
            {
                // Fusion / scission de grille : on repart d'un pool vide sur la nouvelle grille.
                DestroyGlobes();
                _grid = Block.CubeGrid;
                _layoutDirty = true;
                ApplyPower();
            }
            if (IsClient && _active && !_layoutDirty)
            {
                // Une lampe bridée par le budget global récupère ses globes quand de la place se libère.
                float firstOffset;
                int wanted = TheoreticalCount(out firstOffset);
                if (_globes.Count < wanted && ZdlSession.Instance.AllowedGlobes(this, wanted) > _globes.Count)
                    _layoutDirty = true;
            }
            Refresh();
        }

        public override void UpdateBeforeSimulation100()
        {
            if (!IsReady)
            {
                TryInitialize();
                return;
            }
            ApplyPower(); // filet de sécurité : la fonction de consommation du mod doit rester active
            Block.RefreshCustomInfo();
        }

        public override void UpdateAfterSimulation()
        {
            if (!IsReady || !_active || Settings.BlinkMode == (long)ZdlBlinkMode.None) return;
            if ((++_frame & 1) != 0) return; // 30 Hz
            Animate();
        }

        private void UpdateFrameFlag()
        {
            if (IsClient && Settings != null && Settings.BlinkMode != (long)ZdlBlinkMode.None)
                NeedsUpdate |= MyEntityUpdateEnum.EACH_FRAME;
            else
                NeedsUpdate &= ~MyEntityUpdateEnum.EACH_FRAME;
        }

        private void OnEnabledChanged(IMyTerminalBlock block)
        {
            ApplyPower();
            Refresh();
        }

        private void OnWorkingChanged(IMyCubeBlock block)
        {
            ApplyPower();
            Refresh();
        }

        // Vérifie l'état (allumé, caméra, couleur) et recrée/masque la ligne si besoin. Client uniquement pour le rendu.
        private void Refresh()
        {
            if (!IsReady || !IsClient) return;

            bool shouldShow = Block.IsWorking && Block.Enabled && CameraInRange();
            if (!shouldShow)
            {
                if (_globes.Count > 0) DestroyGlobes();
                _active = false;
                return;
            }

            bool wasActive = _active;
            _active = true;
            if (_layoutDirty || !wasActive) RebuildLayout();

            Color color = Block.Color;
            float intensity = Block.Intensity;
            if (color != _lastColor || Math.Abs(intensity - _lastIntensity) > 0.001f)
            {
                _lastColor = color;
                _lastIntensity = intensity;
                for (int i = 0; i < _levels.Length; i++) _levels[i] = -1;
            }

            if (Settings.BlinkMode == (long)ZdlBlinkMode.None)
                for (int i = 0; i < _globes.Count; i++) ApplyLevel(i, 1f);
            else
                Animate();
        }

        private bool CameraInRange()
        {
            var camera = MyAPIGateway.Session.Camera;
            if (camera == null) return true;
            return Vector3D.DistanceSquared(camera.Position, Block.GetPosition()) <= Limits.CameraMaxDistance * Limits.CameraMaxDistance;
        }

        // ---------- Géométrie de la ligne ----------

        // Le premier globe n'est jamais posé au-delà de la distance réglée.
        private int TheoreticalCount(out float firstOffset)
        {
            firstOffset = Math.Min(Math.Max(Settings.Spacing, 0.5f * Settings.GlobeSize + 0.15f), Settings.Distance);
            int n = (int)Math.Floor((Settings.Distance - firstOffset) / Settings.Spacing) + 1;
            _capped = n > Limits.MaxGlobes;
            return Math.Max(1, Math.Min(n, Limits.MaxGlobes));
        }

        private Vector3 LightPosition()
        {
            _dummies.Clear();
            if (Block.Model != null)
            {
                Block.Model.GetDummies(_dummies);
                IMyModelDummy dummy;
                if (_dummies.TryGetValue(LightDummy, out dummy)) return dummy.Matrix.Translation;
            }
            return new Vector3(0f, 0f, Limits.LightZFallback);
        }

        private void RebuildLayout()
        {
            _layoutDirty = false;
            float firstOffset;
            int wanted = TheoreticalCount(out firstOffset);
            int allowed = ZdlSession.Instance.AllowedGlobes(this, wanted);

            while (_globes.Count > allowed)
            {
                int last = _globes.Count - 1;
                CloseGlobe(_globes[last]);
                _globes.RemoveAt(last);
            }
            while (_globes.Count < allowed)
            {
                MyEntity globe = CreateGlobe();
                if (globe == null) break;
                _globes.Add(globe);
            }

            Vector3 origin = LightPosition();
            float size = Settings.GlobeSize;
            for (int i = 0; i < _globes.Count; i++)
            {
                // Repère local du bloc : +X = Right, +Y = Up, -Z = Forward (sens du faisceau).
                var position = new Vector3(origin.X + Settings.OffsetH, origin.Y + Settings.OffsetV, origin.Z - (firstOffset + i * Settings.Spacing));
                Matrix matrix = Matrix.CreateScale(size) * Matrix.CreateTranslation(position);
                _globes[i].PositionComp.SetLocalMatrix(ref matrix);
            }

            _levels = new int[_globes.Count];
            for (int i = 0; i < _levels.Length; i++) _levels[i] = -1;
        }

        private MyEntity CreateGlobe()
        {
            try
            {
                var globe = new MyEntity();
                globe.Init(null, ZdlSession.Instance.GlobeModelPath, (MyEntity)Entity, null, null);
                globe.Save = false;
                globe.Flags = EntityFlags.Visible | EntityFlags.NeedsDraw | EntityFlags.NeedsDrawFromParent | EntityFlags.InvalidateOnMove;
                globe.Render.CastShadows = false;
                globe.Render.EnableColorMaskHsv = false;
                globe.OnAddedToScene(Entity);
                globe.Render.Visible = false;
                return globe;
            }
            catch (Exception e)
            {
                ZdlSession.Log("Creation d'un globe impossible : " + e);
                return null;
            }
        }

        private static void CloseGlobe(MyEntity globe)
        {
            if (globe != null && !globe.Closed) globe.Close();
        }

        public void DestroyGlobes()
        {
            for (int i = 0; i < _globes.Count; i++) CloseGlobe(_globes[i]);
            _globes.Clear();
            _levels = new int[0];
            _layoutDirty = true;
        }

        // ---------- Luminosité ----------

        private void Animate()
        {
            int n = _globes.Count;
            if (n == 0) return;
            double t = MyAPIGateway.Session.ElapsedPlayTime.TotalSeconds;
            for (int i = 0; i < n; i++) ApplyLevel(i, Level(i, n, t));
        }

        private float Level(int index, int count, double time)
        {
            float duty = Settings.BlinkDuty / 100f;
            float min = Settings.MinLevel / 100f;
            double phase = time / Settings.BlinkPeriod;
            switch ((ZdlBlinkMode)Settings.BlinkMode)
            {
                case ZdlBlinkMode.Global:
                    return Frac(phase) < duty ? 1f : min;
                case ZdlBlinkMode.WaveOut:
                case ZdlBlinkMode.WaveIn:
                    // x dans [0 ; 1[ : le premier et le dernier globe n'ont jamais la même phase.
                    float x = (float)index / count;
                    if (Settings.BlinkMode == (long)ZdlBlinkMode.WaveIn) x = (float)(count - 1 - index) / count;
                    float u = (float)Frac(phase - x);
                    return u < duty ? Math.Max(min, 1f - u / duty) : min;
                default:
                    return 1f;
            }
        }

        private static double Frac(double value)
        {
            return value - Math.Floor(value);
        }

        private void ApplyLevel(int index, float level)
        {
            int quantized = (int)Math.Round(MathHelper.Clamp(level, 0f, 1f) * 16f);
            if (_levels[index] == quantized) return;
            _levels[index] = quantized;

            MyEntity globe = _globes[index];
            if (quantized == 0)
            {
                globe.Render.Visible = false;
                return;
            }
            globe.Render.Visible = true;
            float k = MathHelper.Lerp(0.5f, 1f, MathHelper.Clamp((Block.Intensity - Limits.IntensityMin) / (Limits.IntensityMax - Limits.IntensityMin), 0f, 1f));
            globe.SetEmissiveParts(EmissiveMaterial, _lastColor, quantized / 16f * k);
        }

        public void DrawHalo()
        {
            if (!IsReady || !_active) return; // halo toujours actif
            Vector4 baseColor = _lastColor.ToVector4();
            for (int i = 0; i < _globes.Count; i++)
            {
                if (_levels[i] <= 0) continue;
                float level = _levels[i] / 16f;
                MyTransparentGeometry.AddPointBillboard(HaloMaterial, baseColor * level * 0.35f,
                    _globes[i].PositionComp.GetPosition(), Settings.GlobeSize * 0.9f, 0f);
            }
        }

        // ---------- Énergie ----------

        private void ApplyPower()
        {
            if (_sink == null || Limits == null) return;
            float max = Limits.PowerBaseMW + Limits.MaxGlobes * Limits.PowerPerGlobeMW * 2f;
            _sink.SetMaxRequiredInputByType(MyResourceDistributorComponent.ElectricityId, max);
            _sink.SetRequiredInputFuncByType(MyResourceDistributorComponent.ElectricityId, _powerFunc);
            _sink.Update();
        }

        private float ComputeRequiredPower()
        {
            if (!IsReady || !Block.Enabled || !Block.IsFunctional) return 0f;
            float firstOffset;
            int n = TheoreticalCount(out firstOffset);
            float kIntensity = MathHelper.Clamp(Block.Intensity / Limits.IntensityDefault, 0.5f, 2f);
            return Limits.PowerBaseMW + n * Limits.PowerPerGlobeMW * kIntensity;
        }

        private void OnAppendingCustomInfo(IMyTerminalBlock block, StringBuilder sb)
        {
            if (!IsReady) return;
            float firstOffset;
            int n = TheoreticalCount(out firstOffset);
            sb.Append("Globes : ").Append(n);
            if (_capped) sb.Append(" (limité à ").Append(Limits.MaxGlobes).Append(" globes)");
            sb.Append('\n');
            if (IsClient && _active && _globes.Count < n)
                sb.Append("Affichés : ").Append(_globes.Count).Append(" (limité par le budget global de ").Append(ZdlSession.GlobalGlobeBudget).Append(" globes)\n");
            sb.Append("Consommation : ").Append((ComputeRequiredPower() * 1000f).ToString("0.0")).Append(" kW\n");
            sb.Append("Pas des actions : distance ").Append(Limits.Step(ZdlLimits.DistanceSteps, Settings.DistanceStepIndex).ToString("0.###"))
              .Append(" m, espacement ").Append(Limits.Step(ZdlLimits.SpacingSteps, Settings.SpacingStepIndex).ToString("0.###"))
              .Append(" m, décalage V ").Append(Limits.Step(ZdlLimits.OffsetSteps, Settings.OffsetVStepIndex).ToString("0.###"))
              .Append(" m, décalage H ").Append(Limits.Step(ZdlLimits.OffsetSteps, Settings.OffsetHStepIndex).ToString("0.###")).Append(" m\n");
        }

        // ---------- Nettoyage ----------

        public override void OnRemovedFromScene()
        {
            DestroyGlobes();
            _active = false;
        }

        public override void OnAddedToScene()
        {
            _layoutDirty = true;
            if (IsReady) Refresh();
        }

        public override void MarkForClose()
        {
            DestroyGlobes();
        }

        public override void Close()
        {
            DestroyGlobes();
            if (Block != null && IsReady)
            {
                Block.EnabledChanged -= OnEnabledChanged;
                Block.IsWorkingChanged -= OnWorkingChanged;
                Block.AppendingCustomInfo -= OnAppendingCustomInfo;
            }
            if (ZdlSession.Instance != null) ZdlSession.Instance.Unregister(this);
            IsReady = false;
        }
    }
}
