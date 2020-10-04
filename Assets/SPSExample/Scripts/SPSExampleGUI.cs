using System;
using System.Linq;
using UnityEngine;
using SyncingParametersSystem;

namespace SPSExample {
    public class SPSExampleGUI : MonoBehaviour {
        
        private GUIStyle normalStyle = new GUIStyle();
        private GUIStyle redStyle;
        private Player client1;
        private Player client2;
        private bool inited;

        private void Start() {
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
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            {
                GUILayout.Label("Client 1", !SPSManager.IsServer && SPS.GetClient() == client1 ? redStyle : normalStyle);

                if (client1 != null) {
                    GUILayout.Label("Player connected");
                    GUILayout.Label("Player ID: " + client1.Id);
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
                }
                else
                    GUILayout.Label("Player not connected");
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            
        }
    }
}
