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
            for (int i = 0; i < gState.IntStateList.Count; i++)
                gState.IntStateList[i].SetWithCheckEquals(EditorGUILayout.IntField("Element " + i, gState.IntStateList[i].Value));
        }

        private void DrawParameters(ExampleState state) {
            state.BoolValue.SetWithCheckEquals(GUILayout.Toggle(state.BoolValue.Value, "BoolValue"));
            state.IntValue.SetWithCheckEquals(EditorGUILayout.IntField("IntValue", state.IntValue.Value));
            state.FloatValue.SetWithCheckEquals(EditorGUILayout.FloatField("FloatValue", state.FloatValue.Value));
            state.StringValue.SetWithCheckEquals(EditorGUILayout.TextField("StringValue", state.StringValue.Value));
            state.Vector2Value.SetWithCheckEquals(EditorGUILayout.Vector2Field("Vector2Value", state.Vector2Value.Value));
            state.Vector3Value.SetWithCheckEquals(EditorGUILayout.Vector3Field("Vector3Value", state.Vector3Value.Value));
            EditorGUILayout.ObjectField("NetworkIdentityState", state.IdentityValue.Value, typeof(NetworkIdentity), true);
        }
    }
}
