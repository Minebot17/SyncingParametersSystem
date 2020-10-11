using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace SyncingParametersSystem {
    
    public class SPSManager : MonoBehaviour {
        public static SPSManager Instance;
        public static bool IsServer;

        [Header("Maximum time in seconds to wait for synchronization confirmation")]
        [SerializeField] private float confirmationTime;
        private bool Init;
        
        public float ConfirmationTime => confirmationTime;

        private void Awake() {
            if (Instance != null)
                Destroy(Instance.gameObject);
            
            DontDestroyOnLoad(gameObject);
            Instance = this;
            Init = true;
            GameMessage.RegisteredOnServer = false;
            GameMessage.RegisteredOnClient = false;
            GameMessage.Initialize();
            SPS.Initialize();
        }

        private void Start() {
            if (!Init) 
                return;
            
            Init = false;
            if (!IsServer)
                new SyncPlayersMessage().SendToServer();
        }
    }
}