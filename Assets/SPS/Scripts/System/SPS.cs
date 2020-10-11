using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;


namespace SyncingParametersSystem {
    
    public static class SPS {
        
        public static readonly EventHandler<ConnectionEvent> playerRequestPlayersEvent = new EventHandler<ConnectionEvent>();
        public static readonly EventHandler<PlayerEvent> playerReceivedPlayersEvent = new EventHandler<PlayerEvent>();
        public static readonly EventHandler<PlayerEvent> playerAddedEvent = new EventHandler<PlayerEvent>();
        public static readonly EventHandler<PlayerEvent> playerRemovedEvent = new EventHandler<PlayerEvent>();
        
        private static List<Type> loadedStates;
        private static readonly List<Player> players = new List<Player>();
        private static readonly Dictionary<int, Player> playerFromId = new Dictionary<int, Player>();
        private static readonly Dictionary<NetworkConnection, Player> playerFromConn = new Dictionary<NetworkConnection, Player>();
        private static readonly Dictionary<NetworkIdentity, Player> playerFromIdentity = new Dictionary<NetworkIdentity, Player>();
        
        public static List<Player> All => players;
        public static int ClientId { set; get; }
        public static int HostId { set; get; }

        public static NetworkConnection HostConn { set; get; }
        public static IEnumerable<int> Ids => playerFromId.Keys;
        public static IEnumerable<NetworkConnection> Conns => playerFromConn.Keys;
        public static Dictionary<NetworkIdentity, Player> PlayerFromIdentity => playerFromIdentity;

        public static void Initialize() {
            if (loadedStates != null)
                return;

            loadedStates = Utils.FindChildesOfType(typeof(PlayerState)).ToList();
        }

        public static void ResetPlayers() {
            players.Clear();
            playerFromId.Clear();
            playerFromConn.Clear();
            playerFromIdentity.Clear();
            HostId = 0;
            ClientId = 0;
        }

        public static Player AddPlayer(NetworkConnection conn) {
            if (playerFromConn.ContainsKey(conn))
                return playerFromConn[conn];

            Player player = new Player(conn, Utils.rnd.Next());
            if (players.Count == 0) {
                HostId = player.Id;
                HostConn = conn;
                ClientId = HostId;
            }

            players.Add(player);
            playerFromId.Add(player.Id, player);
            playerFromConn.Add(conn, player);

            new AddPlayerClientMessage(player.Id).SendToAllClients(conn);
            playerAddedEvent.CallListeners(new PlayerEvent(player));
            return player;
        }

        public static Player AddPlayer(int id) {
            if (playerFromId.ContainsKey(id))
                return playerFromId[id];

            Player player = new Player(null, id);
            players.Add(player);
            playerFromId.Add(id, player);
            playerAddedEvent.CallListeners(new PlayerEvent(player));
            return player;
        }

        public static void RemovePlayer(NetworkConnection conn) {
            Player toRemove = GetPlayer(conn);
            players.Remove(toRemove);
            playerFromId.Remove(toRemove.Id);
            playerFromConn.Remove(conn);

            playerRemovedEvent.CallListeners(new PlayerEvent(toRemove));

            foreach (GeneralStateValue stateValue in toRemove.allValues.Values)
                stateValue.OnRemoveState();

            new RemovePlayerClientMessage(toRemove.Id).SendToAllClients(conn);
        }

        public static void RemovePlayer(int id) {
            Player toRemove = GetPlayer(id);
            players.Remove(toRemove);
            playerFromId.Remove(id);

            playerRemovedEvent.CallListeners(new PlayerEvent(toRemove));
        }

        public static Player GetPlayer(int id) {
            return playerFromId.Get(id);
        }

        public static Player GetPlayer(NetworkConnection conn) {
            return playerFromConn.Get(conn);
        }
        
        public static Player GetPlayer(NetworkIdentity identity) {
            return playerFromIdentity.Get(identity);
        }

        public static List<Type> GetLoadedStates() {
            return loadedStates;
        }
        
        public static Player GetClient() {
            return playerFromId[ClientId];
        }

        public static Player GetHost() {
            return playerFromId[HostId];
        }

        public static bool IsPlayerExists(int id) {
            return playerFromId.ContainsKey(id);
        }
        
        public static GlobalState GetGlobal() {
            return playerFromId[HostId].GetState<GlobalState>();
        }
        
        public static IEnumerable<T> GetStates<T>() where T : PlayerState {
            return All.Select(s => s.GetState<T>());
        }
        
        public static void RemoveStates(Type stateType, bool sync = true) {
            foreach (Player player in All)
                player.RemoveState(stateType, false);

            if (!sync)
                return;

            RemoveStatesMessage message = new RemoveStatesMessage(stateType.ToString());
            if (SPSManager.IsServer)
                message.SendToAllClients();
            else
                message.SendToServer();
        }
        
        public static void BindIdentityToPlayer(Player player, NetworkIdentity identity, bool sync = true) {
            try {
                NetworkIdentity toRemove = playerFromIdentity.First(pair => pair.Value.Id == player.Id).Key;
                playerFromIdentity.Remove(toRemove);
            }
            catch (InvalidOperationException e) {
            }

            playerFromIdentity[identity] = player;

            if (!sync)
                return;
            
            if (SPSManager.IsServer)
                new BindIdentityToPlayerMessage(player.Id, identity).SendToAllClientsExceptHost();
            else
                new BindIdentityToPlayerMessage(player.Id, identity).SendToServer();
        }

        public class ConnectionEvent : EventBase {

            public NetworkConnection Conn;

            public ConnectionEvent(NetworkConnection conn) : base(null, false) {
                Conn = conn;
            }
        }

        public class PlayerEvent : EventBase {

            public Player Player;

            public PlayerEvent(Player player) : base(null, false) {
                Player = player;
            }
        }
    }
}