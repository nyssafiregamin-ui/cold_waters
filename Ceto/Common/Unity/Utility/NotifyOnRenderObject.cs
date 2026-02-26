using System;

namespace Ceto.Common.Unity.Utility
{
	// Token: 0x02000054 RID: 84
	public class NotifyOnRenderObject : NotifyOnEvent
	{
		// Token: 0x060002A8 RID: 680 RVA: 0x0000F164 File Offset: 0x0000D364
		private void OnRenderObject()
		{
			base.OnEvent();
		}
	}
}
