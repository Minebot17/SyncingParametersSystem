using System.Collections.Generic;

namespace SyncingParametersSystem {
    public class PlayerState {
        private List<GeneralStateValue> valuesList = new List<GeneralStateValue>();
        private Player parent;
        private bool isTest;

        // Такой конструктор обязателен у детей!
        public PlayerState(Player parent, bool isTest) {
            this.parent = parent;
            this.isTest = isTest;
        }

        public Player GetParent() {
            return parent;
        }

        public bool IsTest() {
            return isTest;
        }

        public void AddValue(GeneralStateValue value) {
            valuesList.Add(value);
        }

        public void ResetValues() {
            foreach (GeneralStateValue value in valuesList)
                value.Reset();
        }

        public void OnRemoveState() {
            Dictionary<string, GeneralStateValue> fromRemove = GetParent().allValues;

            foreach (GeneralStateValue value in valuesList) {
                fromRemove.Remove(value.GetName());
            }
        }
    }
}