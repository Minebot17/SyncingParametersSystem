using System;
using UnityEngine;
using UnityEngine.Networking;

namespace SyncingParametersSystem {
    public class ModificationStateValueServerMessage : GameMessage {

        public ModificationStateValueServerMessage() {
        }

        public ModificationStateValueServerMessage(int id, string name) {
            Writer.Write(id);
            Writer.Write(name);
        }

        public override void OnServer(NetworkReader reader, NetworkConnection conn) {
            Player player = SPS.GetPlayer(conn);
            int senderId = player.Id;
            int id = reader.ReadInt32();
            string name = reader.ReadString();
            GeneralStateValue stateValue = player.GetStateValue(name);

            if (stateValue == null) {
                player.CreateStateWithValue(name);
                stateValue = player.GetStateValue(name);

                if (stateValue == null) {
                    Debug.LogError(name + " do not exist in any player state");
                    Debug.LogError(Environment.StackTrace);
                    return;
                }
            }
            
            if (senderId != id && !(stateValue.GetParent() is GlobalState)) {
                Debug.LogError("Try modification not own state value");
                Debug.LogError(Environment.StackTrace);
                return;
            }

            stateValue.Read(reader, conn);
        }

        public override void OnClient(NetworkReader reader) {

        }
    }
}