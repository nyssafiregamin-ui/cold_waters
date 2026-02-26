using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x0200008E RID: 142
	[Serializable]
	public class OverlayHeightTexture
	{
		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x060003BC RID: 956 RVA: 0x00016378 File Offset: 0x00014578
		public bool IsDrawable
		{
			get
			{
				return (this.alpha != 0f && this.tex != null) || (this.maskAlpha != 0f && this.mask != null);
			}
		}

		// Token: 0x040003E8 RID: 1000
		public Texture tex;

		// Token: 0x040003E9 RID: 1001
		public Vector2 scaleUV = Vector2.one;

		// Token: 0x040003EA RID: 1002
		public Vector2 offsetUV;

		// Token: 0x040003EB RID: 1003
		[Range(-20f, 20f)]
		public float alpha = 1f;

		// Token: 0x040003EC RID: 1004
		public Texture mask;

		// Token: 0x040003ED RID: 1005
		public OVERLAY_MASK_MODE maskMode = OVERLAY_MASK_MODE.WAVES_AND_OVERLAY_BLEND;

		// Token: 0x040003EE RID: 1006
		[Range(0f, 1f)]
		public float maskAlpha = 1f;

		// Token: 0x040003EF RID: 1007
		public bool ignoreQuerys;
	}
}
