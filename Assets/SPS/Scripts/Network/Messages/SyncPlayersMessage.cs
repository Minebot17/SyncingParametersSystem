using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace SyncingParametersSystem {
    public class SyncPlayersMessage : GameMessage {

        public SyncPlayersMessage() {
        }

        public SyncPlayersMessage(List<int> players, int clientId, int hostId, Dictionary<NetworkIdentity, Player> playerFromIdentity) {
            Writer.Write(new IntegerListMessage(players));
            Writer.Write(clientId);
            Writer.Write(hostId);
            
            Writer.Write(playerFromIdentity.Count);
            foreach (KeyValuePair<NetworkIdentity, Player> pair in playerFromIdentity) {
                Writer.Write(pair.Value);
                Writer.Write(pair.Key);
            }
        }

        public override void OnServer(NetworkReader reader, NetworkConnection conn) {
            SPS.playerRequestPlayersEvent.CallListeners(new SPS.ConnectionEvent(conn));
            new SyncPlayersMessage(
                SPS.All.Select(s => s.Id).ToList(),
                SPS.GetPlayer(conn).Id,
                SPS.HostId,
                SPS.PlayerFromIdentity.Where(pair => pair.Key && SPS.IsPlayerExists(pair.Value.Id)).ToDictionary(pair => pair.Key, pair => pair.Value)
            ).SendToClient(conn);
        }

        public override void OnClient(NetworkReader reader) {
            List<int> ids = reader.ReadMessage<IntegerListMessage>().Value;
            int clientId = reader.ReadInt32();
            SPS.HostId = reader.ReadInt32();
            SPS.ClientId = clientId;
            List<Player> toRemove = new List<Player>(SPS.All);
            toRemove.RemoveAll(p => ids.Contains(p.Id));
            toRemove.ForEach(p => SPS.RemovePlayer(p.Id));
            ids.ForEach(id => SPS.AddPlayer(id));

            int identityCount = reader.ReadInt32();
            for (int i = 0; i < identityCount; i++)
                SPS.BindIdentityToPlayer(reader.ReadPlayer(), reader.ReadNetworkIdentity(), false);

            SPS.playerReceivedPlayersEvent.CallListeners(new SPS.PlayerEvent(SPS.GetClient()));
        }
    }
}