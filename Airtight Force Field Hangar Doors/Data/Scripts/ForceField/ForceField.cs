using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Sandbox.ModAPI;
using Sandbox.Game.Entities;
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

namespace Hannobal.ForceField
{

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

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            m_objectBuilder = objectBuilder;
            Entity.NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME | MyEntityUpdateEnum.EACH_10TH_FRAME;
        }

        public override void UpdateOnceBeforeFrame()
        {
            // actual initialization
            block = Entity as IMyAirtightHangarDoor;
            block.IsWorkingChanged += HandleIsWorkingChanged;
            block.DoorStateChanged += HandleDoorStateChanged;
            block.CustomNameChanged += HandleNameChanged;
            idleSound = new MySoundPair("BlockProjectorOff");
            idleSoundEmitter = new MyEntity3DSoundEmitter(MyEntities.GetEntityById(block.EntityId));
            idleSoundEmitter.CanPlayLoopSounds = true;
            openSound = new MySoundPair("BlockProjectorOn");
            openSoundEmitter = new MyEntity3DSoundEmitter(MyEntities.GetEntityById(block.EntityId));
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
            HandleNameChanged(block as IMyTerminalBlock);
            HandleIsWorkingChanged(block as IMyCubeBlock);
            Entity.NeedsUpdate |= MyEntityUpdateEnum.NONE;
            var door = Entity as MyAirtightDoorGeneric;
        }

        public override void UpdateAfterSimulation10()
        {
            base.UpdateAfterSimulation10();
            MatrixD mat = Entity.WorldMatrix;
            var door = Entity as MyAirtightDoorGeneric;
            foreach (KeyValuePair<string, MyEntitySubpart> sp in door.Subparts)
            {
                sp.Value.PositionComp.UpdateWorldMatrix(ref mat, null, true, true);
            }
        }

        public override void Close()
        {
            if (idleSoundEmitter != null) idleSoundEmitter.StopSound(true, true);
            if (openSoundEmitter != null) openSoundEmitter.StopSound(true, true);
            openSoundEmitter = null;
            idleSoundEmitter = null;
            openSound = null;
            idleSound = null;
            block = Entity as IMyAirtightHangarDoor;
            if (block == null) return;
            block.IsWorkingChanged -= HandleIsWorkingChanged;
            block.DoorStateChanged -= HandleDoorStateChanged;
            block.CustomNameChanged -= HandleNameChanged;
        }

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

        void HandleIsWorkingChanged(IMyCubeBlock obj)
        {
            if (block == null) return;
            if (block.IsWorking)
            {
                idleSoundEmitter.PlaySound(idleSound, true, true, false, false, false);
            }
            else
            {
                idleSoundEmitter.StopSound(false);
            }
            UpdateFFVisibility();
        }

        void HandleDoorStateChanged(bool b)
        {
            // 1. stopPrevious is standard
            // 2. skipIntro set true to avoid weird and loud bug with the loop
            // 3. force2d set false
            // 4. alwaysHearOnRealistic
            // 5. skipToEnd set to false to play the loop
            openSoundEmitter.PlaySound(openSound, true, true, false, false, true);
            UpdateFFVisibility();
        }

        void UpdateFFVisibility()
        {
            if (MyAPIGateway.Session == null) return;
            var door = Entity as MyAirtightDoorGeneric;
            if (door == null) return;
            if (block == null) return;
            if (door.Subparts == null) return;
            foreach (KeyValuePair<string, MyEntitySubpart> sp in door.Subparts)
            {
                if (sp.Value == null) continue;
                if (sp.Value.Render == null) continue;
                sp.Value.Render.UpdateRenderObject(true);
                sp.Value.NeedsUpdate = MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
                sp.Value.NeedsWorldMatrix = true;
                if (sp.Key == null) continue;
                if (sp.Key.Contains("Base")) continue;
                if (block.IsWorking)
                {
                    if ((door.OpenRatio==0f && (sp.Key.Contains(colorClosed)))
                        || (door.OpenRatio!=0f && sp.Key.Contains(colorOpen)))
                    {
                        sp.Value.Render.Visible = true;
                    }
                    else if (door.OpenRatio==0f && sp.Key.Contains("_door"))
                    {
                        sp.Value.Render.Visible = false;
                        if (sp.Value.Physics != null) sp.Value.Physics.Enabled = true;
                    }
                    else
                    {
                        sp.Value.Render.Visible = false;
                        if (sp.Value.Physics != null) sp.Value.Physics.Enabled = false;
                    }
                }
                else
                {
                    sp.Value.Render.Visible = false;
                    if (sp.Value.Physics != null) sp.Value.Physics.Enabled = false;
                }
            }
        }

        public override MyObjectBuilder_EntityBase GetObjectBuilder(bool copy = false)
        {
            return m_objectBuilder;
        }
    }
}