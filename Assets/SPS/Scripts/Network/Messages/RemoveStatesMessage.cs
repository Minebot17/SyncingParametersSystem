using System;
using UnityEngine.Networking;

namespace SyncingParametersSystem {
    public class RemoveStatesMessage : GameMessage {

        public RemoveStatesMessage() {
        }

        public RemoveStatesMessage(string stateClass) {
            Writer.Write(stateClass);
        }

        public override void OnServer(NetworkReader reader, NetworkConnection conn) {
            string stateClass = reader.ReadString();

            Type type = Type.GetType(stateClass);
            SPS.RemoveStates(type, false);

            new RemoveStatesMessage(stateClass).SendToAllClient(conn);
        }

        public override void OnClient(NetworkReader reader) {
            string stateClass = reader.ReadString();

            Type type = Type.GetType(stateClass);
            SPS.RemoveStates(type, false);
        }
    }
}