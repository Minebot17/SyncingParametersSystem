using UnityEngine.Networking;

namespace SyncingParametersSystem {
    public class BindIdentityToPlayerMessage : GameMessage {
        
        public BindIdentityToPlayerMessage() {
        }
        
        public BindIdentityToPlayerMessage(int playerId, NetworkIdentity identity) {
            Writer.Write(playerId);
            Writer.Write(identity);
        }

        public override void OnServer(NetworkReader reader, NetworkConnection conn) {
            int playerId = reader.ReadInt32();
            NetworkIdentity identity = reader.ReadNetworkIdentity();
            SPS.BindIdentityToPlayer(SPS.GetPlayer(playerId), identity, false);
            new BindIdentityToPlayerMessage(playerId, identity).SendToAllClientsExceptHost();
        }

        public override void OnClient(NetworkReader reader) {
            SPS.BindIdentityToPlayer(SPS.GetPlayer(reader.ReadInt32()), reader.ReadNetworkIdentity(), false);
        }
    }
}