using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x020000C3 RID: 195
	[DisallowMultipleComponent]
	public abstract class UnderWaterBase : OceanComponent
	{
		// Token: 0x17000131 RID: 305
		// (get) Token: 0x0600059A RID: 1434
		public abstract UNDERWATER_MODE Mode { get; }

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x0600059B RID: 1435
		public abstract DEPTH_MODE DepthMode { get; }

		// Token: 0x0600059C RID: 1436
		public abstract void RenderOceanMask(GameObject go);

		// Token: 0x0600059D RID: 1437
		public abstract void RenderOceanDepth(GameObject go);

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x0600059E RID: 1438 RVA: 0x00025344 File Offset: 0x00023544
		// (set) Token: 0x0600059F RID: 1439 RVA: 0x0002534C File Offset: 0x0002354C
		public IRefractionCommand CustomRefractionCommand { get; set; }
	}
}
