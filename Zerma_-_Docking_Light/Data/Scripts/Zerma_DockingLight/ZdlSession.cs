using System;
using System.Collections.Generic;
using System.Text;
using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.ModAPI;

namespace Zerma.DockingLight
{
    // Composant de session : contrôles terminal (une fois), réseau, chemin du modèle, budget global, halo, nettoyage.
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class ZdlSession : MySessionComponentBase
    {
        public static ZdlSession Instance;

        public const ushort NetworkChannel = 47231;
        public const int GlobalGlobeBudget = 1500;
        public static readonly Guid StorageGuid = new Guid("1ff9969e-e6f3-430e-a54a-9b17306728c3");

        public string GlobeModelPath;
        public bool IsDedicated;
        public bool IsServer;

        private readonly List<ZdlLogic> _logics = new List<ZdlLogic>();
        private bool _handlerRegistered;

        public override void LoadData()
        {
            Instance = this;
            GlobeModelPath = ModContext.ModPath + "\\Models\\Zerma_DockingLight_Globe.mwm";
            IsDedicated = MyAPIGateway.Utilities.IsDedicated;
            IsServer = MyAPIGateway.Multiplayer.IsServer;
            MyAPIGateway.Multiplayer.RegisterSecureMessageHandler(NetworkChannel, OnMessage);
            _handlerRegistered = true;
        }

        // Les contrôles eux-mêmes sont créés par le premier ZdlLogic (voir ZdlControls.CreateControls).
        public override void BeforeStart()
        {
            ZdlControls.HookGetters();
        }

        protected override void UnloadData()
        {
            if (_handlerRegistered)
                MyAPIGateway.Multiplayer.UnregisterSecureMessageHandler(NetworkChannel, OnMessage);
            ZdlControls.Unload();
            foreach (ZdlLogic logic in _logics.ToArray()) logic.DestroyGlobes();
            _logics.Clear();
            Instance = null;
        }

        public override void Draw()
        {
            if (IsDedicated) return;
            for (int i = 0; i < _logics.Count; i++) _logics[i].DrawHalo();
        }

        public void Register(ZdlLogic logic)
        {
            if (!_logics.Contains(logic)) _logics.Add(logic);
        }

        public void Unregister(ZdlLogic logic)
        {
            _logics.Remove(logic);
        }

        // Nombre de globes que cette lampe peut afficher sans dépasser le budget client global.
        public int AllowedGlobes(ZdlLogic requester, int wanted)
        {
            int used = 0;
            for (int i = 0; i < _logics.Count; i++)
                if (_logics[i] != requester) used += _logics[i].GlobeCount;
            return Math.Max(0, Math.Min(wanted, GlobalGlobeBudget - used));
        }

        // --- Réseau : message = "entityId|réglages sérialisés" en UTF-8 ---

        public void SendToServer(ZdlLogic logic)
        {
            MyAPIGateway.Multiplayer.SendMessageToServer(NetworkChannel, Encode(logic));
        }

        public void Broadcast(ZdlLogic logic)
        {
            if (MyAPIGateway.Multiplayer.MultiplayerActive)
                MyAPIGateway.Multiplayer.SendMessageToOthers(NetworkChannel, Encode(logic));
        }

        private static byte[] Encode(ZdlLogic logic)
        {
            return Encoding.UTF8.GetBytes(logic.Entity.EntityId.ToString() + "|" + logic.Settings.Serialize());
        }

        private void OnMessage(ushort channel, byte[] data, ulong senderSteamId, bool fromServer)
        {
            try
            {
                string text = Encoding.UTF8.GetString(data);
                int sep = text.IndexOf('|');
                if (sep <= 0) return;
                long entityId;
                if (!long.TryParse(text.Substring(0, sep), out entityId)) return;

                IMyEntity entity = MyAPIGateway.Entities.GetEntityById(entityId);
                IMyTerminalBlock block = entity as IMyTerminalBlock;
                if (block == null || block.GameLogic == null) return;
                ZdlLogic logic = block.GameLogic.GetAs<ZdlLogic>();
                if (logic == null || !logic.IsReady) return;

                ZdlSettings settings = ZdlSettings.Deserialize(text.Substring(sep + 1), logic.Limits);

                if (IsServer && !fromServer)
                {
                    long identityId = MyAPIGateway.Players.TryGetIdentityId(senderSteamId);
                    if (!block.HasPlayerAccess(identityId))
                    {
                        // Refus : on renvoie l'état faisant foi à l'expéditeur pour annuler son changement local.
                        MyAPIGateway.Multiplayer.SendMessageTo(NetworkChannel, Encode(logic), senderSteamId);
                        return;
                    }
                    logic.ApplyFromNetwork(settings, true);
                }
                else if (fromServer)
                {
                    logic.ApplyFromNetwork(settings, false);
                }
            }
            catch (Exception e)
            {
                Log("Erreur reseau : " + e);
            }
        }

        public static void Log(string message)
        {
            VRage.Utils.MyLog.Default.WriteLineAndConsole("[Zerma Docking Light] " + message);
        }
    }
}
