using UnityEngine.Networking;

namespace SyncingParametersSystem {
    public class SPSNetworkManager : NetworkManager {
        
        public override void OnStartHost() {
            SPSManager.IsServer = true;
        }
        
        public override void OnStopHost() {
            SPSManager.IsServer = false;
            SPS.ResetPlayers();
        }
        
        public override void OnServerConnect(NetworkConnection conn) {
            SPS.AddPlayer(conn);
        }
	
        public override void OnServerDisconnect(NetworkConnection conn) {
            SPS.RemovePlayer(conn);
        }
    }
}