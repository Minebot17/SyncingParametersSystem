using System.Collections.Generic;

namespace SyncingParametersSystem {
    public class GlobalState : PlayerState {

        public readonly List<IntStateValue> IntStateList; // TODO remove on export

        public GlobalState(Player parent, bool isTest) : base(parent, isTest) {
            IntStateList = new List<IntStateValue>();
            for (int i = 0; i < 3; i++)
                IntStateList.Add(new IntStateValue(this, "IntStateList_" + i, i, SyncType.ALL_SYNC, true));
        }
    }
}