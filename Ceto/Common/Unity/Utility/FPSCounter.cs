using System;
using UnityEngine;

namespace Ceto.Common.Unity.Utility
{
	// Token: 0x0200004F RID: 79
	public class FPSCounter : MonoBehaviour
	{
		// Token: 0x1700009F RID: 159
		// (get) Token: 0x0600029D RID: 669 RVA: 0x0000EF94 File Offset: 0x0000D194
		// (set) Token: 0x0600029E RID: 670 RVA: 0x0000EF9C File Offset: 0x0000D19C
		public float FrameRate { get; set; }

		// Token: 0x0600029F RID: 671 RVA: 0x0000EFA8 File Offset: 0x0000D1A8
		private void Start()
		{
			this.timeleft = this.updateInterval;
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x0000EFB8 File Offset: 0x0000D1B8
		private void Update()
		{
			this.timeleft -= Time.deltaTime;
			this.accum += Time.timeScale / Time.deltaTime;
			this.frames += 1f;
			if (this.timeleft <= 0f)
			{
				this.FrameRate = this.accum / this.frames;
				this.timeleft = this.updateInterval;
				this.accum = 0f;
				this.frames = 0f;
			}
		}

		// Token: 0x0400027A RID: 634
		private float updateInterval = 0.5f;

		// Token: 0x0400027B RID: 635
		private float accum;

		// Token: 0x0400027C RID: 636
		private float frames;

		// Token: 0x0400027D RID: 637
		private float timeleft;
	}
}
