using UnityEngine;

namespace SyncingParametersSystem {
    
    public class SPSManager : MonoBehaviour {
        public static SPSManager Instance;
        public static bool IsServer;

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
        }

        private void Start() {
            if (!Init) 
                return;
            
            Init = false;
            new SyncPlayersMessage().SendToServer();
        }
    }
}