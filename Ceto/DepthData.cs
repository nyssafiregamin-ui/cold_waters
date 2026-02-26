using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ceto
{
	// Token: 0x0200007A RID: 122
	public class DepthData
	{
		// Token: 0x0400034C RID: 844
		public bool updated;

		// Token: 0x0400034D RID: 845
		public Camera cam;

		// Token: 0x0400034E RID: 846
		public CommandBuffer grabCmd;

		// Token: 0x0400034F RID: 847
		public CameraEvent cmdEvent;
	}
}
