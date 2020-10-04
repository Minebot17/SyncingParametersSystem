using System.Collections.Generic;
using SyncingParametersSystem;
using UnityEngine;

namespace SPSExample {
    public class ExampleState : PlayerState {

        public IntStateValue SlotNumber;
        public BoolStateValue BoolState;
        public IntStateValue IntState;
        public FloatStateValue FloatState;
        public StringStateValue StringState;
        public Vector2StateValue Vector2State;
        public Vector3StateValue Vector3State;
        public NetworkIdentityStateValue IdentityState;
        public List<IntStateValue> IntStateList;
        
        public ExampleState(Player parent, bool isTest) : base(parent, isTest) {
            SlotNumber = new IntStateValue(this, "SlotNumber", 0, SyncType.ALL_SYNC, true);
            BoolState = new BoolStateValue(this, "BoolState", false, SyncType.ALL_SYNC);
            IntState = new IntStateValue(this, "IntState", 0);
            FloatState = new FloatStateValue(this, "FloatState", 5, SyncType.OWNER_SYNC);
            StringState = new StringStateValue(this, "StringState", "example", SyncType.ALL_SYNC, true);
            Vector2State = new Vector2StateValue(this, "Vector2State", Vector2.zero, SyncType.OWNER_SYNC, true);
            Vector3State = new Vector3StateValue(this, "Vector3State", Vector3.zero, SyncType.ALL_SYNC);
            IdentityState = new NetworkIdentityStateValue(this, "IdentityState", null, SyncType.ALL_SYNC);
            IntStateList = new List<IntStateValue>();
            for (int i = 0; i < 3; i++)
                IntStateList.Add(new IntStateValue(this, "IntStateList_0", i, SyncType.ALL_SYNC));
        }
    }
}