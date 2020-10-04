namespace SyncingParametersSystem {
	public class EventBase {

		public object Sender;
		public bool IsCancable;
		private bool isUnsubscribe;

		public EventBase(object sender, bool isCancable) {
			Sender = sender;
			IsCancable = isCancable;
		}

		public bool IsCancel {
			get { return IsUnsubscribe; }
			set { IsUnsubscribe = IsCancable && value; }
		}
		
		public bool IsUnsubscribe { get; set; }
	}
}


