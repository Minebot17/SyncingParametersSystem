using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace SyncingParametersSystem {
    public abstract class StateValue<T> : GeneralStateValue {
        
        private PlayerState parent;
        private string name;
        private T defaultValue;
        private T value;
        private SyncType sync;
        private bool modification;
        private int playerRequestPlayersEventId;
        private NetworkConnection modificationBuffer;
        private int countToConfirm = -1;
        private Coroutine confirmationTimer;

        /// <summary>
        /// Эвент, срабатывающий при изменении значения параметра
        /// </summary>
        public readonly EventHandler<OnChangeValueEvent> onChangeValueEvent = new EventHandler<OnChangeValueEvent>();

        /// <summary>
        /// Срабатывает на сервере когда все получатели синхронизировали значение. После срабатывания очищается в null
        /// </summary>
        public Action onAllSynced;

        public T Value {
            get => value;
            set {
                OnChangeValueEvent result = onChangeValueEvent.CallListners(new OnChangeValueEvent(this.value, value));
                if (result.IsCancel)
                    return;

                this.value = result.NewValue;

                if (sync != SyncType.NOT_SYNC && SPSManager.Instance.IsServer) {
                    bool toConfirm = onAllSynced != null;
                    SyncStateValueMessage message = new SyncStateValueMessage(GetOwnerId(), GetName(), toConfirm);
                    Write(message.Writer);
                    if (modificationBuffer != null) {
                        if (sync == SyncType.ALL_SYNC)
                            message.SendToAllClient(modificationBuffer, SPS.HostConn);
                        else
                            message.SendToClient(parent.GetParent().Conn);

                        if (toConfirm)
                            StartConfirmation(sync == SyncType.ALL_SYNC
                                              ? (parent.GetParent() == SPS.GetHost()
                                                 ? SPS.All.Count - 1
                                                 : SPS.All.Count - 2)
                                              : (SPS.GetHost() == parent.GetParent() ? 0 : 1)
                            );

                        modificationBuffer = null;
                    }
                    else {
                        if (sync == SyncType.ALL_SYNC)
                            message.SendToAllClientExceptHost();
                        else
                            message.SendToClient(parent.GetParent().Conn);

                        if (toConfirm)
                            StartConfirmation(sync == SyncType.ALL_SYNC
                                              ? SPS.All.Count - 1
                                              : (SPS.GetHost() == parent.GetParent() ? 0 : 1)
                            );
                    }
                }

                if (modification
                    && !SPSManager.Instance.IsServer
                    && SPS.ClientId != 0
                    && SPS.GetClient().Equals(parent.GetParent())) {
                    ModificationStateValueServerMessage message =
                    new ModificationStateValueServerMessage(GetOwnerId(), GetName());
                    Write(message.Writer);
                    message.SendToServer();
                }
            }
        }

        protected StateValue(PlayerState parent, string name, T defaultValue, bool isTest, SyncType sync,
        bool modification) {
            this.parent = parent;
            this.name = name;
            this.defaultValue = defaultValue;
            this.sync = sync;
            this.modification = modification;
            value = defaultValue;

            if (isTest)
                parent.GetParent().testValues.Add(name, this);
            else {
                if (parent.GetParent().allValues.ContainsKey(name)) {
                    Debug.LogError("StateValue with name: \"" + name + "\" already created");
                    Debug.LogError(Environment.StackTrace);
                }

                parent.GetParent().allValues.Add(name, this);
                parent.AddValue(this);
                playerRequestPlayersEventId = SPS.playerRequestPlayersEvent.SubcribeEvent(e => {
                    if (this.defaultValue == null ? value == null : this.defaultValue.Equals(value))
                        return;

                    SyncStateValueMessage message = new SyncStateValueMessage(GetOwnerId(), GetName(), false);
                    Write(message.Writer);
                    message.SendToClient(e.Conn);
                }
                );
            }
        }

        protected int GetOwnerId() {
            return parent.GetParent().Id;
        }

        public override string GetName() {
            return name;
        }

        public override void Reset() {
            Value = defaultValue;
        }

        public override void Read(NetworkReader reader, NetworkConnection modificationBuffer) {
            this.modificationBuffer = modificationBuffer;
            Value = ReadValue(reader);
        }

        public override void OnRemoveState() {
            SPS.playerRequestPlayersEvent.UnSubcribeEvent(playerRequestPlayersEventId);
        }

        /// <summary>
        /// То же самое, что и Value = value, но не синхронизирует если значение в итоге не поменялось
        /// </summary>
        public void SetWithCheckEquals(T newValue) {
            if (!newValue.Equals(value))
                Value = newValue;
        }

        /// <summary>
        /// Вызывается от клиента для подтверждения прихода значений, если того требовал сервер
        /// </summary>
        public override void Confirm() {
            if (countToConfirm == -1)
                return;

            countToConfirm--;
            if (countToConfirm != 0)
                return;

            onAllSynced.Invoke();
            onAllSynced = null;
            countToConfirm = -1;
            SPSManager.Instance.StopCoroutine(confirmationTimer);
        }

        private void StartConfirmation(int countToConfirm) {
            if (countToConfirm == 0) {
                onAllSynced.Invoke();
                return;
            }

            confirmationTimer = SPSManager.Instance.StartCoroutine(ConfirmationTimer());
            this.countToConfirm = countToConfirm;
        }

        private IEnumerator ConfirmationTimer() {
            yield return new WaitForSeconds(SPSManager.Instance.ConfirmationTime);
            if (countToConfirm == -1)
                yield break;

            Debug.LogError("State confirmation time out!");
            onAllSynced = null;
            countToConfirm = -1;
        }

        protected abstract T ReadValue(NetworkReader reader);

        public class OnChangeValueEvent : EventBase {

            public T OldValue;
            public T NewValue;

            public OnChangeValueEvent(T oldValue, T newValue) : base(null, true) {
                OldValue = oldValue;
                NewValue = newValue;
            }
        }
    }
}