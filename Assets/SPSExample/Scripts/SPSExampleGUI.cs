using System;
using System.Linq;
using UnityEngine;
using SyncingParametersSystem;
using UnityEditor;
using UnityEngine.Networking;

namespace SPSExample {
    public class SPSExampleGUI : MonoBehaviour {

        [SerializeField] private NetworkIdentity identityToState;
        private GUIStyle normalStyle = new GUIStyle();
        private GUIStyle redStyle;
        private Player client1;
        private Player client2;
        private bool inited;
        private GlobalState gState;

        private void Start() {
            if (SPSManager.IsServer) {
                SPS.GetHost().GetState<ExampleState>().IdentityValue.Value = identityToState;
                gState = SPS.GetGlobal();
            }

            inited = SPSManager.IsServer;
            redStyle = new GUIStyle {normal = {textColor = Color.red}};
            
            SPS.playerReceivedPlayersEvent.SubcribeEvent(ev => {
                int playersCount = SPS.All.Count;
                client2 = null;
                
                if (playersCount == 2)
                    client1 = ev.Player;
                else if (playersCount == 3) {
                    client1 = SPS.All.First(p => p != ev.Player && p != SPS.GetHost());
                    client2 = ev.Player;
                }

                gState = SPS.GetGlobal();
                inited = true;
            });
            
            SPS.playerAddedEvent.SubcribeEvent(ev => {
                if (SPS.ClientId == ev.Player.Id)
                    return;
                
                if (client1 == null)
                    client1 = ev.Player;
                else if (client2 == null)
                    client2 = ev.Player;

                if (SPSManager.IsServer)
                    ev.Player.GetState<ExampleState>().IdentityValue.SetNotSync(identityToState);
            });

            SPS.playerRemovedEvent.SubcribeEvent(ev => {
                if (client1 == ev.Player) {
                    client1 = client2;
                    client2 = null;
                }
                else if (client2 == ev.Player)
                    client2 = null;
            });
        }

        private void OnGUI() {
            if (!inited || !SPS.IsPlayerExists(SPS.ClientId))
                return;
            
            GUILayout.Label("Local player highlighted in red. Players count: " + SPS.All.Count);
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            {
                GUILayout.Label("Host", SPSManager.IsServer ? redStyle : normalStyle);
                GUILayout.Label("Player connected");
                GUILayout.Label("Player ID: " + SPS.HostId);
                GUILayout.Space(10);
                DrawParameters(SPS.GetHost().GetState<ExampleState>());
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            {
                GUILayout.Label("Client 1", !SPSManager.IsServer && SPS.GetClient() == client1 ? redStyle : normalStyle);

                if (client1 != null) {
                    GUILayout.Label("Player connected");
                    GUILayout.Label("Player ID: " + client1.Id);
                    GUILayout.Space(10);
                    DrawParameters(client1.GetState<ExampleState>());
                }
                else
                    GUILayout.Label("Player not connected");
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            {
                GUILayout.Label("Client 2", !SPSManager.IsServer && SPS.GetClient() == client2 ? redStyle : normalStyle);
                
                if (client2 != null) {
                    GUILayout.Label("Player connected");
                    GUILayout.Label("Player ID: " + client2.Id);
                    GUILayout.Space(10);
                    DrawParameters(client2.GetState<ExampleState>());
                }
                else
                    GUILayout.Label("Player not connected");
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.Label("Int list in GlobalState");
            for (int i = 0; i < gState.IntStateList.Count; i++) {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Element " + i);
                gState.IntStateList[i].SetWithCheckEquals(int.Parse(GUILayout.TextField(gState.IntStateList[i].Value+"")));
                GUILayout.EndHorizontal();
            }
        }

        private void DrawParameters(ExampleState state) {
            state.BoolValue.SetWithCheckEquals(GUILayout.Toggle(state.BoolValue.Value, "BoolValue"));
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("IntValue");
            state.IntValue.SetWithCheckEquals(int.Parse(GUILayout.TextField(state.IntValue.Value+"")));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("FloatValue");
            state.FloatValue.SetWithCheckEquals(float.Parse(GUILayout.TextField(state.FloatValue.Value+"")));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("StringValue");
            state.StringValue.SetWithCheckEquals(GUILayout.TextField(state.StringValue.Value));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Vector2Value");
            Vector2 vec2 = Vector2.zero;
            vec2.x = float.Parse(GUILayout.TextField(state.Vector2Value.Value.x + ""));
            vec2.y = float.Parse(GUILayout.TextField(state.Vector2Value.Value.y + ""));
            state.Vector2Value.SetWithCheckEquals(vec2);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Vector3Value");
            Vector3 vec3 = Vector3.zero;
            vec3.x = float.Parse(GUILayout.TextField(state.Vector3Value.Value.x + ""));
            vec3.y = float.Parse(GUILayout.TextField(state.Vector3Value.Value.y + ""));
            vec3.z = float.Parse(GUILayout.TextField(state.Vector3Value.Value.z + ""));
            state.Vector3Value.SetWithCheckEquals(vec3);
            GUILayout.EndHorizontal();

            string identityName = state.IdentityValue.Value == null ? "null" : state.IdentityValue.Value.name;
            GUILayout.Label("NetworkIdentityState: " + identityName);
        }
    }
}
