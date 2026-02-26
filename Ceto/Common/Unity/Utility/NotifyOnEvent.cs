using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto.Common.Unity.Utility
{
	// Token: 0x02000050 RID: 80
	public abstract class NotifyOnEvent : MonoBehaviour
	{
		// Token: 0x060002A2 RID: 674 RVA: 0x0000F05C File Offset: 0x0000D25C
		protected void OnEvent()
		{
			if (NotifyOnEvent.Disable)
			{
				return;
			}
			int count = this.m_actions.Count;
			for (int i = 0; i < count; i++)
			{
				NotifyOnEvent.INotify notify = this.m_actions[i];
				if (notify is NotifyOnEvent.Notify)
				{
					NotifyOnEvent.Notify notify2 = notify as NotifyOnEvent.Notify;
					notify2.action(base.gameObject);
				}
				else if (notify is NotifyOnEvent.NotifyWithArg)
				{
					NotifyOnEvent.NotifyWithArg notifyWithArg = notify as NotifyOnEvent.NotifyWithArg;
					notifyWithArg.action(base.gameObject, notifyWithArg.arg);
				}
			}
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x0000F0F4 File Offset: 0x0000D2F4
		public void AddAction(Action<GameObject, object> action, object arg)
		{
			NotifyOnEvent.NotifyWithArg notifyWithArg = new NotifyOnEvent.NotifyWithArg();
			notifyWithArg.action = action;
			notifyWithArg.arg = arg;
			this.m_actions.Add(notifyWithArg);
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x0000F124 File Offset: 0x0000D324
		public void AddAction(Action<GameObject> action)
		{
			NotifyOnEvent.Notify notify = new NotifyOnEvent.Notify();
			notify.action = action;
			this.m_actions.Add(notify);
		}

		// Token: 0x0400027F RID: 639
		public static bool Disable;

		// Token: 0x04000280 RID: 640
		private IList<NotifyOnEvent.INotify> m_actions = new List<NotifyOnEvent.INotify>();

		// Token: 0x02000051 RID: 81
		private interface INotify
		{
		}

		// Token: 0x02000052 RID: 82
		private class Notify : NotifyOnEvent.INotify
		{
			// Token: 0x04000281 RID: 641
			public Action<GameObject> action;
		}

		// Token: 0x02000053 RID: 83
		private class NotifyWithArg : NotifyOnEvent.INotify
		{
			// Token: 0x04000282 RID: 642
			public Action<GameObject, object> action;

			// Token: 0x04000283 RID: 643
			public object arg;
		}
	}
}
