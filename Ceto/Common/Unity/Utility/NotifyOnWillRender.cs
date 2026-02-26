using System;

namespace Ceto.Common.Unity.Utility
{
	// Token: 0x02000055 RID: 85
	public class NotifyOnWillRender : NotifyOnEvent
	{
		// Token: 0x060002AA RID: 682 RVA: 0x0000F174 File Offset: 0x0000D374
		private void OnWillRenderObject()
		{
			base.OnEvent();
		}
	}
}
