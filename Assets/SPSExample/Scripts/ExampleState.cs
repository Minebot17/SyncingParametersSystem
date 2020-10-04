using System.Collections.Generic;
using SyncingParametersSystem;
using UnityEngine;

namespace SPSExample {
    public class ExampleState : PlayerState {
        
        public readonly BoolStateValue BoolValue;
        public readonly IntStateValue IntValue;
        public readonly FloatStateValue FloatValue;
        public readonly StringStateValue StringValue;
        public readonly Vector2StateValue Vector2Value;
        public readonly Vector3StateValue Vector3Value;
        public readonly NetworkIdentityStateValue IdentityValue;

        public ExampleState(Player parent, bool isTest) : base(parent, isTest) {
            BoolValue = new BoolStateValue(this, "BoolValue", false, SyncType.ALL_SYNC);
            IntValue = new IntStateValue(this, "IntValue", 0);
            FloatValue = new FloatStateValue(this, "FloatValue", 5, SyncType.OWNER_SYNC);
            StringValue = new StringStateValue(this, "StringValue", "example", SyncType.ALL_SYNC, true);
            Vector2Value = new Vector2StateValue(this, "Vector2Value", Vector2.zero, SyncType.OWNER_SYNC, true);
            Vector3Value = new Vector3StateValue(this, "Vector3State", Vector3.zero, SyncType.NOT_SYNC, true);
            IdentityValue = new NetworkIdentityStateValue(this, "IdentityValue", null, SyncType.ALL_SYNC);
        }
    }
}