﻿using System;
using System.Collections.Generic;
using System.Linq;
using Object = System.Object;

namespace SyncingParametersSystem {
	
	public class EventHandler<T> : Exception where T : EventBase {
		private List<Listener<T>> listeners = new List<Listener<T>>();
		
		public int SubscribeEvent(Action<T> method, EventPriority priority = EventPriority.Normal) {
			Listener<T> listener = new Listener<T>(method, priority);
			listeners.Add(listener);
			listeners = listeners.OrderBy(x => x.GetPriority()).ToList();
			return listener.GetId();
		}
		
		public bool UnSubscribeEvent(int ID) {
			return listeners.RemoveAll(x => ID == x.GetId()) >= 1;
		}
		
		public void UnSubscribeAll() {
			listeners.Clear();
		}
		
		public T CallListeners(T e) {
			List<Listener<T>> toUnsubscribe = new List<Listener<T>>();
			foreach (Listener<T> listener in listeners) {
				listener.CallMethod(e);

				if (e.IsUnsubscribe) {
					e.IsUnsubscribe = false;
					toUnsubscribe.Add(listener);
				}
			}

			listeners.RemoveAll(l => toUnsubscribe.Contains(l));
			return e;
		}
		
		public void Reset() {
			listeners.Clear();
		}

		public int ListenersCount() {
			return listeners.Count;
		}

		public class Listener<T> where T: EventBase {
			private readonly int id;
			private Action<T> method;
			private EventPriority priority;
			
			public Listener(Action<T> method, EventPriority priority) {
				id = Utils.rnd.Next();
				this.method = method;
				this.priority = priority;
			}

			public int GetId() {
				return id;
			}

			public EventPriority GetPriority() {
				return priority;
			}

			public void CallMethod(T args) {
				method.Invoke(args);
			}

			public override bool Equals(Object obj) {
				if (!(obj is Listener<T>))
					return false;

				int id = ((EventHandler<T>.Listener<T>) obj).GetId();
				return id == this.id;
			}

			public override int GetHashCode() {
				return id;
			}
		}
	}
}