using System.Collections.Generic;
using Sandbox.Game;
using Sandbox.Game.Components;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using VRage;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;

namespace Zerma.FullFirstSpawn
{
    // The first time each identity spawns in the world, fill its health, food, suit energy,
    // oxygen and hydrogen, and add a starter kit to its inventory (on top of the default tools).
    // Runs on the server only; the game syncs the values to the client.
    // Identities already served are stored in the world (sandbox variable), so a respawn after
    // death or a server restart does not refill them again.
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public class FullFirstSpawnSession : MySessionComponentBase
    {
        const string StorageKey = "Zerma_FullFirstSpawn_Identities";
        // spawn code sets its own values first: wait ~2 s before overwriting them
        const int DelayTicks = 120;

        // starter kit: S-10 pistol + 2 magazines, flare gun + 5 flares
        static readonly KitItem[] StarterKit =
        {
            new KitItem(typeof(MyObjectBuilder_PhysicalGunObject), "SemiAutoPistolItem", 1),
            new KitItem(typeof(MyObjectBuilder_AmmoMagazine), "SemiAutoPistolMagazine", 2),
            new KitItem(typeof(MyObjectBuilder_PhysicalGunObject), "FlareGunItem", 1),
            new KitItem(typeof(MyObjectBuilder_AmmoMagazine), "FlareClip", 5),
        };

        struct KitItem
        {
            public readonly MyDefinitionId Id;
            public readonly int Amount;

            public KitItem(MyObjectBuilderType type, string subtype, int amount)
            {
                Id = new MyDefinitionId(type, subtype);
                Amount = amount;
            }
        }

        readonly HashSet<long> served = new HashSet<long>();
        readonly Dictionary<long, int> pending = new Dictionary<long, int>();
        readonly List<long> ready = new List<long>();
        bool isServer;

        public override void BeforeStart()
        {
            isServer = MyAPIGateway.Session.IsServer;
            if (!isServer)
                return;

            string stored;
            if (MyAPIGateway.Utilities.GetVariable(StorageKey, out stored) && !string.IsNullOrEmpty(stored))
            {
                foreach (var part in stored.Split(','))
                {
                    long id;
                    if (long.TryParse(part, out id))
                        served.Add(id);
                }
            }

            MyVisualScriptLogicProvider.PlayerSpawned += OnPlayerSpawned;
        }

        protected override void UnloadData()
        {
            if (isServer)
                MyVisualScriptLogicProvider.PlayerSpawned -= OnPlayerSpawned;
        }

        void OnPlayerSpawned(long identityId)
        {
            if (!served.Contains(identityId))
                pending[identityId] = DelayTicks;
        }

        public override void UpdateAfterSimulation()
        {
            if (!isServer || pending.Count == 0)
                return;

            ready.Clear();
            foreach (var id in new List<long>(pending.Keys))
            {
                if (--pending[id] <= 0)
                    ready.Add(id);
            }

            foreach (var id in ready)
            {
                pending.Remove(id);
                if (Fill(id))
                {
                    served.Add(id);
                    MyAPIGateway.Utilities.SetVariable(StorageKey, string.Join(",", served));
                }
            }
        }

        static bool Fill(long identityId)
        {
            var character = GetCharacter(identityId);
            if (character == null || character.IsDead)
                return false;

            // suit energy, oxygen and hydrogen take a 0..1 ratio
            MyVisualScriptLogicProvider.SetPlayersEnergyLevel(identityId, 1f);
            MyVisualScriptLogicProvider.SetPlayersOxygenLevel(identityId, 1f);
            MyVisualScriptLogicProvider.SetPlayersHydrogenLevel(identityId, 1f);

            var stats = character.Components.Get<MyEntityStatComponent>();
            if (stats != null)
            {
                FillStat(stats, "Health");
                FillStat(stats, "Food");
            }

            GiveStarterKit(character);

            MyLog.Default.WriteLineAndConsole("Full First Spawn: stats filled and starter kit given for identity " + identityId);
            return true;
        }

        static void GiveStarterKit(IMyCharacter character)
        {
            var inventory = character.GetInventory();
            if (inventory == null)
                return;

            foreach (var item in StarterKit)
            {
                var content = (MyObjectBuilder_PhysicalObject)MyObjectBuilderSerializer.CreateNewObject(item.Id);
                var amount = (MyFixedPoint)item.Amount;
                if (inventory.CanItemsBeAdded(amount, item.Id))
                    inventory.AddItems(amount, content);
                else
                    MyLog.Default.WriteLineAndConsole("Full First Spawn: inventory full, " + item.Id + " not given");
            }
        }

        static void FillStat(MyEntityStatComponent stats, string name)
        {
            MyEntityStat stat;
            if (stats.TryGetStat(MyStringHash.GetOrCompute(name), out stat) && stat != null)
                stat.Value = stat.MaxValue;
        }

        static IMyCharacter GetCharacter(long identityId)
        {
            var players = new List<IMyPlayer>();
            MyAPIGateway.Players.GetPlayers(players, p => p.IdentityId == identityId);
            return players.Count > 0 ? players[0].Character : null;
        }
    }
}
