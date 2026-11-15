using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Sandbox.ModAPI;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.Entities;
using Sandbox.Game.EntityComponents;
using VRage.Game;
using VRage.Game.ModAPI;
using VRage.ObjectBuilders;
using VRage.Game.Components;
using VRageRender;

using System;
using VRage.ModAPI;
using VRageMath;
using VRage.Game.Entity;
using Sandbox.Definitions;
using Sandbox.ModAPI.Interfaces.Terminal;
using VRage.Utils;
using DoorStatus = Sandbox.ModAPI.Ingame.DoorStatus;

namespace Hannobal.ForceField
{
    // How the force field works (repository rework, 2026-10-28):
    //
    //                       | air                         | players / objects / ships
    //   active              | blocked                     | free (collision disabled for now)
    //   inactive            | free                        | free
    //
    // "active" = switched on, functional, and the grid can supply the power it needs.
    //
    // The game decides airtightness from the hangar door state only (MyGridGasSystem.IsAirtightBlock:
    // a closed door is airtight, an open one is not). So the script uses the real door state for
    // the AIR only: the door is kept closed while the field is active and opened when it is not.
    // The "solid/passable" state is stored per block in the Custom Data ("ForceField: solid|passable")
    // because Custom Data is saved with the world and synced in multiplayer. The vanilla "Open"
    // terminal switch, its toolbar actions, the use key and programmable blocks switch it. For now it
    // only selects the color (closed/open color): the collision subpart is kept disabled, see
    // UpdateCollision.

    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_AirtightHangarDoor),true,
        "ForceFieldDoor0", "ForceFieldDoor1", "ForceFieldDoor2",
        "ForceFieldDoor3", "ForceFieldDoor4", "ForceFieldDoor5",
        "ForceFieldDoor6", "ForceFieldDoor7", "ForceFieldDoor8",
        "ForceFieldDoor9", "ForceFieldDoor10",
        "SmallForceFieldDoor0", "SmallForceFieldDoor1", "SmallForceFieldDoor2",
        "SmallForceFieldDoor3", "SmallForceFieldDoor4", "SmallForceFieldDoor5",
        "SmallForceFieldDoor6", "SmallForceFieldDoor7", "SmallForceFieldDoor8",
        "SmallForceFieldDoor9", "SmallForceFieldDoor10")]
    public class ForceField : MyGameLogicComponent
    {
        private const string StateKey = "ForceField:";
        private const string DebugTag = "[ffdebug]";
        // Checks for returning grid power every POWER_RETRY_PERIOD * 10 frames (~1.7 s).
        private const int POWER_RETRY_PERIOD = 10;

        private MyObjectBuilder_EntityBase m_objectBuilder = null;
        private IMyAirtightHangarDoor block;
        private MySoundPair idleSound;
        private MySoundPair openSound;
        private MyEntity3DSoundEmitter idleSoundEmitter;
        private MyEntity3DSoundEmitter openSoundEmitter;
        private string colorOpen, colorClosed;
        private readonly string[] colorNames = {
            "red","orange","yellow","lime","green","spring","cyan","sky","blue","purple","magenta","pink"
        };

        private bool passable;
        private bool lastActive;
        // Server only: true while the grid supplies the field. When false the block asks for 0 W,
        // which the game counts as "powered", so the script can still open the door.
        private bool fieldPowered = true;
        private int powerRetryCountdown;
        private bool changingDoor;
        // Last open state requested by the script. DoorStateChanged is raised both when the state
        // changes and when the animation ends; only a change to something else comes from outside.
        private bool expectedOpen;
        private int debugCountdown;
        // True once the game has been asked to rebuild the door subparts (see UpdateCollision).
        private bool rebuildRequested;
        // Number of UpdateAfterSimulation10 calls that still have to refresh the visuals.
        private int visualRefreshesPending = 2;
        private MyResourceSinkComponentBase sink;
        private Func<float> originalPowerFunc;

        public bool Passable { get { return passable; } }

        public static ForceField Get(IMyTerminalBlock terminalBlock)
        {
            if (terminalBlock == null || terminalBlock.GameLogic == null) return null;
            return terminalBlock.GameLogic.GetAs<ForceField>();
        }

        private static bool IsServer
        {
            get { return MyAPIGateway.Multiplayer == null || MyAPIGateway.Multiplayer.IsServer; }
        }

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            m_objectBuilder = objectBuilder;
            Entity.NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME | MyEntityUpdateEnum.EACH_10TH_FRAME;
        }

        public override void UpdateOnceBeforeFrame()
        {
            if (block != null)
            {
                // Called again after UpdateCollision asked the game to rebuild the door subparts
                // (the game's own UpdateOnceBeforeFrame runs in the same pass): only refresh.
                rebuildRequested = false;
                Log("subparts rebuilt by the game: " + DescribeState());
                UpdateFFVisibility();
                visualRefreshesPending = 2;
                return;
            }
            // actual initialization
            block = Entity as IMyAirtightHangarDoor;
            if (block == null) return;
            block.IsWorkingChanged += HandleIsWorkingChanged;
            block.DoorStateChanged += HandleDoorStateChanged;
            block.CustomNameChanged += HandleNameChanged;
            block.CustomDataChanged += HandleCustomDataChanged;
            idleSound = new MySoundPair("BlockProjectorOff");
            idleSoundEmitter = new MyEntity3DSoundEmitter((MyEntity)Entity);
            idleSoundEmitter.CanPlayLoopSounds = true;
            openSound = new MySoundPair("BlockProjectorOn");
            openSoundEmitter = new MyEntity3DSoundEmitter((MyEntity)Entity);
            openSoundEmitter.CanPlayLoopSounds = false;
            if (block.BlockDefinition.SubtypeId.Contains("Small"))
            {
                idleSoundEmitter.VolumeMultiplier = 0.1f;
                openSoundEmitter.VolumeMultiplier = 0.2f;
            }
            else
            {
                idleSoundEmitter.VolumeMultiplier = 0.65f;
                openSoundEmitter.VolumeMultiplier = 1f;
            }
            colorOpen = colorNames[4];
            colorClosed = colorNames[7];

            // Worlds saved before the rework have no state in the Custom Data: an open door meant
            // "passable" in the original mod, keep that meaning.
            expectedOpen = block.Status == DoorStatus.Open || block.Status == DoorStatus.Opening;
            bool found;
            passable = ReadPassable(out found);
            if (!found)
            {
                passable = expectedOpen;
                if (IsServer) WritePassable(passable);
            }

            if (IsServer)
            {
                // Replace the door's power requirement so the script can drop it to 0 W when the
                // grid cannot supply the field (see UpdatePowerState).
                sink = block.ResourceSink;
                if (sink != null)
                    originalPowerFunc = sink.SetRequiredInputFuncByType(MyResourceDistributorComponent.ElectricityId, ComputeRequiredPower);
            }

            if (ForceFieldSession.Instance != null) ForceFieldSession.Instance.HookTerminal();

            HandleNameChanged(block as IMyTerminalBlock);
            lastActive = !IsFieldActive();
            RefreshState();
            Log("init " + DescribeState());
        }

        public override void UpdateAfterSimulation10()
        {
            base.UpdateAfterSimulation10();
            if (block == null) return;
            // The original mod forced the world matrix of every subpart here every 10 frames. The game
            // already keeps them up to date, and for the collision subparts it teleported their physics
            // body out of the doorway while the game's constraint pulled it back: the field jittered and
            // violently pushed players away (logs 2026-10-28 12:55 and 13:05). Removed.
            // But right after a world load the visuals do not follow a moving grid (tests 7 and 8:
            // base and field panel left behind in space until the first state change, although the
            // subpart entities do follow the block). Refreshing the render objects
            // (UpdateFFVisibility) fixes it, but only once the game has finished setting up the door
            // subparts: it is repeated on the first updates after a load or a rebuild.
            if (visualRefreshesPending > 0)
            {
                visualRefreshesPending--;
                UpdateFFVisibility();
                Log("visuals refreshed after load/rebuild");
            }
            if (HasTag(DebugTag.Trim('[', ']')) && debugCountdown == 5)
                Log("positions (block frame, metres): " + DescribePositions());
            if (IsServer)
            {
                UpdatePowerState();
                UpdateDoor();
            }
            RefreshState();
            ShowDebug();
        }

        // Positions of the collision subparts, their physics bodies and the visible field, expressed
        // in the block's own frame, to see which of them actually sits in the doorway.
        string DescribePositions()
        {
            var door = Entity as MyEntity;
            if (door == null || door.Subparts == null) return "none";
            MatrixD toBlock = MatrixD.Invert(Entity.WorldMatrix);
            string text = "";
            foreach (KeyValuePair<string, MyEntitySubpart> sp in door.Subparts)
            {
                if (sp.Key == null || sp.Value == null) continue;
                bool collision = sp.Key.Contains("_door");
                bool shownField = sp.Value.Render != null && sp.Value.Render.Visible && !sp.Key.Contains("Base");
                if (!collision && !shownField) continue;
                text += " " + sp.Key + ": local=" + FormatVector(sp.Value.PositionComp.LocalMatrixRef.Translation)
                    + " world->block=" + FormatVector(Vector3D.Transform(sp.Value.WorldMatrix.Translation, toBlock));
                if (sp.Value.Physics != null)
                    text += " body->block=" + FormatVector(Vector3D.Transform(sp.Value.Physics.GetWorldMatrix().Translation, toBlock));
                text += ";";
            }
            return text;
        }

        static string FormatVector(Vector3D v)
        {
            return "(" + v.X.ToString("0.00") + " " + v.Y.ToString("0.00") + " " + v.Z.ToString("0.00") + ")";
        }

        public override void Close()
        {
            if (idleSoundEmitter != null) idleSoundEmitter.StopSound(true, true);
            if (openSoundEmitter != null) openSoundEmitter.StopSound(true, true);
            openSoundEmitter = null;
            idleSoundEmitter = null;
            openSound = null;
            idleSound = null;
            if (sink != null && originalPowerFunc != null)
                sink.SetRequiredInputFuncByType(MyResourceDistributorComponent.ElectricityId, originalPowerFunc);
            block = Entity as IMyAirtightHangarDoor;
            if (block == null) return;
            block.IsWorkingChanged -= HandleIsWorkingChanged;
            block.DoorStateChanged -= HandleDoorStateChanged;
            block.CustomNameChanged -= HandleNameChanged;
            block.CustomDataChanged -= HandleCustomDataChanged;
        }

        // --- State ------------------------------------------------------------------------

        // Same answer on every machine: the server keeps the door closed only while the field is
        // active, and the door state is synced by the game.
        bool IsFieldActive()
        {
            if (block == null || !block.IsWorking) return false;
            return block.Status == DoorStatus.Closed || block.Status == DoorStatus.Closing;
        }

        // Server only: what the field should be, from which the door state is derived.
        bool ShouldBeActive()
        {
            return block.Enabled && block.IsFunctional && fieldPowered;
        }

        public void SetPassable(bool value)
        {
            if (block == null || value == passable) return;
            // Applied locally at once; the Custom Data change is synced to the other machines.
            passable = value;
            WritePassable(value);
            PlayOpenSound();
            UpdateFFVisibility();
            Log("passable set: " + DescribeState());
        }

        bool ReadPassable(out bool found)
        {
            found = false;
            if (block == null || block.CustomData == null) return false;
            foreach (string line in block.CustomData.Split('\n'))
            {
                string trimmed = line.Trim();
                if (!trimmed.StartsWith(StateKey, StringComparison.OrdinalIgnoreCase)) continue;
                found = true;
                return trimmed.Substring(StateKey.Length).Trim().Equals("passable", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        void WritePassable(bool value)
        {
            string state = StateKey + " " + (value ? "passable" : "solid");
            var lines = (block.CustomData ?? "").Split('\n').ToList();
            int index = lines.FindIndex(l => l.Trim().StartsWith(StateKey, StringComparison.OrdinalIgnoreCase));
            if (index >= 0) lines[index] = state;
            else if (lines.Count == 1 && lines[0].Trim().Length == 0) lines[0] = state;
            else lines.Add(state);
            string text = string.Join("\n", lines);
            if (text != block.CustomData) block.CustomData = text;
        }

        // --- Power (server) ---------------------------------------------------------------

        float ComputeRequiredPower()
        {
            if (!fieldPowered || originalPowerFunc == null) return 0f;
            return originalPowerFunc();
        }

        void UpdatePowerState()
        {
            if (sink == null || originalPowerFunc == null) return;
            if (!block.Enabled || !block.IsFunctional) return;
            var electricity = MyResourceDistributorComponent.ElectricityId;
            if (fieldPowered)
            {
                if (sink.IsPoweredByType(electricity)) return;
                // The grid no longer supplies the field: ask for nothing so the game still lets
                // the script open the door (it refuses on unpowered blocks).
                fieldPowered = false;
                powerRetryCountdown = POWER_RETRY_PERIOD;
                sink.SetRequiredInputByType(electricity, ComputeRequiredPower());
                return;
            }
            if (--powerRetryCountdown > 0) return;
            powerRetryCountdown = POWER_RETRY_PERIOD;
            var distributor = block.CubeGrid.ResourceDistributor;
            if (distributor == null) return;
            float headroom = distributor.MaxAvailableResourceByType(electricity, block.CubeGrid)
                - distributor.TotalRequiredInputByType(electricity, block.CubeGrid);
            if (headroom < originalPowerFunc()) return;
            fieldPowered = true;
            sink.SetRequiredInputByType(electricity, ComputeRequiredPower());
        }

        // --- Door = airtightness (server) -------------------------------------------------

        void UpdateDoor()
        {
            bool wantClosed = ShouldBeActive();
            bool closed = block.Status == DoorStatus.Closed || block.Status == DoorStatus.Closing;
            if (wantClosed == closed) return;
            expectedOpen = !wantClosed;
            changingDoor = true;
            bool switchedOn = false;
            try
            {
                if (block.IsWorking)
                {
                    if (wantClosed) block.CloseDoor();
                    else block.OpenDoor();
                }
                else if (!wantClosed && !block.Enabled && block.IsFunctional)
                {
                    // The game ignores OpenDoor() on a switched off block: switch it on for the call.
                    // Done here rather than in IsWorkingChanged to avoid re-entering that event.
                    switchedOn = true;
                    block.Enabled = true;
                    if (block.IsWorking) block.OpenDoor();
                }
                    // A damaged (non-functional) block cannot move its door: it stays as it is.
            }
            finally
            {
                if (switchedOn) block.Enabled = false;
                changingDoor = false;
            }
        }

        // --- Events -----------------------------------------------------------------------

        void HandleNameChanged(IMyTerminalBlock obj)
        {
            var parts = Regex.Matches(obj.CustomName, @"[\""].+?[\""]|[^ ]+")
                .Cast<Match>()
                .Select(m => m.Value)
                .ToList();
            bool changed = false;
            for (int i=0;i<parts.Count;i++)
            {
                if(parts[i].Equals("co"))
                {
                    i++;
                    colorOpen = (i< parts.Count && CheckColorName(parts[i])) ? parts[i] : colorNames[4];
                    changed = true;
                }
                else if (parts[i].Equals("cc"))
                {
                    i++;
                    colorClosed = (i < parts.Count && CheckColorName(parts[i])) ? parts[i] : colorNames[7];
                    changed = true;
                }
            }
            if (changed) UpdateFFVisibility();
            //MyAPIGateway.Utilities.ShowNotification(colorOpen + " " + colorClosed, 10000, MyFontEnum.White);
        }

        bool CheckColorName(string color)
        {
            if (color == null) return false;
            foreach(string str in colorNames)
            {
                if (str.Equals(color)) return true;
            }
            return false;
        }

        void HandleCustomDataChanged(IMyTerminalBlock obj)
        {
            bool found;
            bool value = ReadPassable(out found);
            if (!found || value == passable) return;
            passable = value;
            PlayOpenSound();
            UpdateFFVisibility();
            Log("passable synced: " + DescribeState());
        }

        void HandleIsWorkingChanged(IMyCubeBlock obj)
        {
            if (block == null || changingDoor) return;
            RefreshState();
        }

        void HandleDoorStateChanged(bool open)
        {
            if (block == null || changingDoor) return;
            // The use key and programmable blocks open the door directly. While the field is
            // active that would let the air out: treat it as a solid/passable toggle and close
            // the door again in the same frame.
            if (open && !expectedOpen && IsServer && ShouldBeActive())
            {
                SetPassable(!passable);
                changingDoor = true;
                try
                {
                    block.CloseDoor();
                }
                finally
                {
                    changingDoor = false;
                }
            }
            RefreshState();
        }

        // --- Visuals, sound and collision -------------------------------------------------

        void RefreshState()
        {
            bool active = IsFieldActive();
            if (active != lastActive)
            {
                lastActive = active;
                if (idleSoundEmitter != null)
                {
                    if (active) idleSoundEmitter.PlaySound(idleSound, true, true, false, false, false);
                    else idleSoundEmitter.StopSound(false);
                }
                PlayOpenSound();
                UpdateFFVisibility();
                Log("active changed: " + DescribeState());
                return;
            }
            // Re-applied every 10 frames: the game recreates the subpart physics on some grid
            // changes, and the collision must follow the field state.
            UpdateCollision();
        }

        void PlayOpenSound()
        {
            if (openSoundEmitter == null) return;
            // 1. stopPrevious is standard
            // 2. skipIntro set true to avoid weird and loud bug with the loop
            // 3. force2d set false
            // 4. alwaysHearOnRealistic
            // 5. skipToEnd set to false to play the loop
            openSoundEmitter.PlaySound(openSound, true, true, false, false, true);
        }

        void UpdateFFVisibility()
        {
            if (MyAPIGateway.Session == null) return;
            var door = Entity as MyEntity;
            if (door == null) return;
            if (block == null) return;
            if (door.Subparts == null) return;
            string color = passable ? colorOpen : colorClosed;
            foreach (KeyValuePair<string, MyEntitySubpart> sp in door.Subparts)
            {
                if (sp.Value == null) continue;
                if (sp.Value.Render == null) continue;
                sp.Value.Render.UpdateRenderObject(true);
                sp.Value.NeedsUpdate = MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
                sp.Value.NeedsWorldMatrix = true;
                if (sp.Key == null) continue;
                if (sp.Key.Contains("Base")) continue;
                // The "HangarDoor_door<N>" subpart only carries the collision; it is never shown.
                sp.Value.Render.Visible = lastActive && !sp.Key.Contains("_door") && sp.Key.Contains(color);
            }
            UpdateCollision();
        }

        // Collision is enabled only while the field is active, solid and fully closed.
        // Switching it off: the collision bodies are simply disabled.
        // Switching it on: a disabled body cannot just be enabled again — the game then places it on
        // its subpart entity, which sits at the block origin, out of the doorway, and its constraint
        // no longer brings it back (logs 2026-10-28 13:05). Instead the game is asked to rebuild the
        // door subparts (its UpdateOnceBeforeFrame -> InitSubparts recreates physics and constraints),
        // the same path as when the block is placed, which puts the collision in the doorway.
        void UpdateCollision()
        {
            var door = Entity as MyEntity;
            if (door == null || door.Subparts == null || block == null) return;
            // Only solid while the door is fully closed: the collision subpart moves with the door.
            bool solid = lastActive && !passable && block.IsFullyClosed;
            foreach (KeyValuePair<string, MyEntitySubpart> sp in door.Subparts)
            {
                if (sp.Value == null || sp.Key == null || sp.Value.Physics == null) continue;
                if (sp.Key.Contains("Base")) continue;
                var physics = sp.Value.Physics;
                bool wanted = solid && sp.Key.Contains("_door");
                if (physics.Enabled == wanted) continue;
                if (!wanted)
                {
                    physics.Enabled = false;
                    Log("collision " + sp.Key + " -> OFF | " + DescribeBody(sp.Value));
                }
                else if (!rebuildRequested)
                {
                    rebuildRequested = true;
                    Entity.NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
                    Log("collision -> ON requested: asking the game to rebuild the door subparts");
                }
            }
        }

        static string DescribeBody(MyEntitySubpart subpart)
        {
            var physics = subpart.Physics;
            if (physics == null) return "no physics";
            double offset = (physics.GetWorldMatrix().Translation - subpart.WorldMatrix.Translation).Length();
            return "enabled=" + physics.Enabled + " inWorld=" + physics.IsInWorld + " static=" + physics.IsStatic
                + " offset=" + offset.ToString("0.00") + "m";
        }

        // --- Diagnostics (in-game tests) ----------------------------------------------------

        bool HasTag(string tag)
        {
            return block != null && block.CustomName != null
                && block.CustomName.IndexOf(tag, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        // Only for blocks with "ffdebug" in their name, to keep players' logs clean.
        void Log(string message)
        {
            if (!HasTag(DebugTag.Trim('[', ']'))) return;
            MyLog.Default.WriteLineAndConsole("ForceField [" + Entity.EntityId + " '" + block.CustomName + "']: " + message);
        }

        string DescribeState()
        {
            string text = "active=" + lastActive + " passable=" + passable
                + " | working=" + block.IsWorking + " enabled=" + block.Enabled + " functional=" + block.IsFunctional
                + " | door=" + block.Status + " ratio=" + block.OpenRatio.ToString("0.00")
                + " | fieldPowered=" + (IsServer ? fieldPowered.ToString() : "client")
                + " | subparts:";
            var door = Entity as MyEntity;
            if (door == null || door.Subparts == null) return text + " none";
            foreach (KeyValuePair<string, MyEntitySubpart> sp in door.Subparts)
            {
                if (sp.Value == null) continue;
                text += " " + sp.Key + "[visible=" + (sp.Value.Render != null && sp.Value.Render.Visible);
                var physics = sp.Value.Physics;
                if (physics == null)
                {
                    text += " no physics]";
                    continue;
                }
                // Distance between the physics body and the subpart: should be ~0.
                double offset = (physics.GetWorldMatrix().Translation - sp.Value.WorldMatrix.Translation).Length();
                text += " physics enabled=" + physics.Enabled + " inWorld=" + physics.IsInWorld
                    + " static=" + physics.IsStatic + " offset=" + offset.ToString("0.00") + "m]";
            }
            return text;
        }

        // "ffdebug" in the block name (any case, brackets optional): state written to the game log
        // about once per second, and shown on screen.
        void ShowDebug()
        {
            if (!HasTag(DebugTag.Trim('[', ']'))) return;
            if (++debugCountdown < 6) return;
            debugCountdown = 0;
            Log(DescribeState());
            if (MyAPIGateway.Session == null || MyAPIGateway.Session.Player == null) return;
            // Short on-screen summary (the full state is in SpaceEngineers.log).
            MyAPIGateway.Utilities.ShowNotification("FF active=" + lastActive + " passable=" + passable
                + " door=" + block.Status + " power=" + (IsServer ? fieldPowered.ToString() : "?"), 1000, MyFontEnum.White);
            var door = Entity as MyEntity;
            if (door == null || door.Subparts == null) return;
            foreach (KeyValuePair<string, MyEntitySubpart> sp in door.Subparts)
            {
                if (sp.Key == null || !sp.Key.Contains("_door") || sp.Value == null) continue;
                var physics = sp.Value.Physics;
                string state = physics == null ? "no physics"
                    : "on=" + physics.Enabled + " inWorld=" + physics.IsInWorld
                      + " offset=" + (physics.GetWorldMatrix().Translation - sp.Value.WorldMatrix.Translation).Length().ToString("0.00");
                MyAPIGateway.Utilities.ShowNotification(sp.Key.Replace("HangarDoor_", "") + ": " + state, 1000, MyFontEnum.White);
            }
        }

        public override MyObjectBuilder_EntityBase GetObjectBuilder(bool copy = false)
        {
            return m_objectBuilder;
        }
    }

    // Redirects the vanilla door "Open" switch and its toolbar actions to solid/passable for force
    // field blocks. Other doors keep their original behaviour; originals are restored on unload.
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class ForceFieldSession : MySessionComponentBase
    {
        public static ForceFieldSession Instance;
        private bool hooked;
        private readonly List<Action> restore = new List<Action>();

        public override void LoadData()
        {
            Instance = this;
        }

        protected override void UnloadData()
        {
            foreach (Action undo in restore) undo();
            restore.Clear();
            Instance = null;
        }

        // Called by the first force field block: the door terminal controls exist by then.
        public void HookTerminal()
        {
            if (hooked) return;
            hooked = true;

            List<IMyTerminalControl> controls;
            MyAPIGateway.TerminalControls.GetControls<IMyAirtightHangarDoor>(out controls);
            foreach (IMyTerminalControl control in controls)
            {
                if (control.Id != "Open") continue;
                var openSwitch = control as IMyTerminalValueControl<bool>;
                if (openSwitch == null) continue;
                var getter = openSwitch.Getter;
                var setter = openSwitch.Setter;
                openSwitch.Getter = b =>
                {
                    var ff = ForceField.Get(b);
                    return ff != null ? ff.Passable : getter(b);
                };
                openSwitch.Setter = (b, value) =>
                {
                    var ff = ForceField.Get(b);
                    if (ff != null) ff.SetPassable(value);
                    else setter(b, value);
                };
                restore.Add(() => { openSwitch.Getter = getter; openSwitch.Setter = setter; });
            }

            List<IMyTerminalAction> actions;
            MyAPIGateway.TerminalControls.GetActions<IMyAirtightHangarDoor>(out actions);
            foreach (IMyTerminalAction terminalAction in actions)
            {
                string id = terminalAction.Id;
                if (id != "Open" && id != "Open_On" && id != "Open_Off") continue;
                var action = terminalAction.Action;
                var writer = terminalAction.Writer;
                terminalAction.Action = b =>
                {
                    var ff = ForceField.Get(b);
                    if (ff == null) { if (action != null) action(b); return; }
                    if (id == "Open") ff.SetPassable(!ff.Passable);
                    else ff.SetPassable(id == "Open_On");
                };
                terminalAction.Writer = (b, text) =>
                {
                    var ff = ForceField.Get(b);
                    if (ff == null) { if (writer != null) writer(b, text); return; }
                    text.Append(ff.Passable ? "Open" : "Closed");
                };
                var current = terminalAction;
                restore.Add(() => { current.Action = action; current.Writer = writer; });
            }
        }
    }
}
