﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using Object = System.Object;

namespace SyncingParametersSystem {
	
	/// <summary>
	/// Система эвентов для какого-либо компонента, что бы перехватывать какие-либо действия и изменять их. Что бы это реализовать, компонент должен иметь реализацию интерфейса IEventProvider
	/// </summary>
	/// <typeparam name="T">Тип эвента для системы</typeparam>
	public class EventHandler<T> : Exception where T : EventBase {
		private List<Listner<T>> listners = new List<Listner<T>>();

		/// <summary>
		/// Добавить промежуточный метод. Привязать метод к эвенту
		/// </summary>
		public int SubcribeEvent(Action<T> method, EventPriority priority = EventPriority.Normal) {
			Listner<T> listner = new Listner<T>(method, priority);
			listners.Add(listner);
			listners = listners.OrderBy(x => x.GetPriority()).ToList();
			return listner.GetId();
		}

		/// <summary>
		/// Отписывает метод с каким-либо ID от эвента
		/// </summary>
		public bool UnSubcribeEvent(int ID) {
			return listners.RemoveAll(x => ID == x.GetId()) >= 1;
		}
		
		public void UnSubcribeAll() {
			listners.Clear();
		}

		/// <summary>
		/// Вызвать эвент у всех слушателей
		/// </summary>
		/// <param name="e">Эвент</param>
		/// <returns>Эвент, прогнаный через слушателей</returns>
		public T CallListners(T e) {
			List<Listner<T>> toUnsubscribe = new List<Listner<T>>();
			foreach (Listner<T> listener in listners) {
				listener.CallMethod(e);

				if (e.IsUnsubscribe) {
					e.IsUnsubscribe = false;
					toUnsubscribe.Add(listener);
				}
			}

			listners.RemoveAll(l => toUnsubscribe.Contains(l));
			return e;
		}

		/// <summary>
		/// Убирает из эвента все подписанные методы
		/// </summary>
		public void Reset() {
			listners.Clear();
		}

		public class Listner<T> where T: EventBase {
			private readonly int id;
			private Action<T> method;
			private EventPriority priority;
			
			public Listner(Action<T> method, EventPriority priority) {
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
				if (!(obj is Listner<T>))
					return false;

				int id = ((EventHandler<T>.Listner<T>) obj).GetId();
				return id == this.id;
			}

			public override int GetHashCode() {
				return id;
			}
		}
	}
}