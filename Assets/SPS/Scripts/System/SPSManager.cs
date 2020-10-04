using UnityEngine;
using UnityEngine.Networking;

namespace SyncingParametersSystem {
    
    [RequireComponent(typeof(NetworkIdentity))]
    public class SPSManager : MonoBehaviour {
        public static SPSManager Instance;

        [SerializeField] private float confirmationTime;
        private NetworkIdentity identity;
        private bool Init;

        public bool IsServer => identity.isServer;
        public float ConfirmationTime => confirmationTime;

        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }

            identity = GetComponent<NetworkIdentity>();
            DontDestroyOnLoad(gameObject);
            Instance = this;
            Init = true;
            GameMessage.Initialize();
            SPS.Initialize();
        }

        private void Start() {
            if (!Init) 
                return;
            
            Init = false;
            new SyncPlayersMessage().SendToServer();
        }
    }
}