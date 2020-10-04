using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace SyncingParametersSystem {
    public class SyncPlayersMessage : GameMessage {

        public SyncPlayersMessage() {
        }

        public SyncPlayersMessage(List<int> players, int clientId, int hostId) {
            Writer.Write(new IntegerListMessage(players));
            Writer.Write(clientId);
            Writer.Write(hostId);
        }

        public override void OnServer(NetworkReader reader, NetworkConnection conn) {
            SPS.playerRequestPlayersEvent.CallListners(new SPS.ConnectionEvent(conn));
            new SyncPlayersMessage(
            SPS.All.Select(s => s.Id).ToList(),
            SPS.GetPlayer(conn).Id,
            SPS.HostId
            ).SendToClient(conn);
        }

        public override void OnClient(NetworkReader reader) {
            List<int> ids = reader.ReadMessage<IntegerListMessage>().Value;
            int clientId = reader.ReadInt32();
            SPS.HostId = reader.ReadInt32();
            SPS.ClientId = clientId;
            ids.ForEach(id => SPS.AddPlayer(id));
        }
    }
}