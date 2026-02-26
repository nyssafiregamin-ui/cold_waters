using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x0200008D RID: 141
	[Serializable]
	public class OverlayNormalTexture
	{
		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x060003BA RID: 954 RVA: 0x000162EC File Offset: 0x000144EC
		public bool IsDrawable
		{
			get
			{
				return (this.alpha != 0f && this.tex != null) || (this.maskAlpha != 0f && this.mask != null);
			}
		}

		// Token: 0x040003E1 RID: 993
		public Texture tex;

		// Token: 0x040003E2 RID: 994
		public Vector2 scaleUV = Vector2.one;

		// Token: 0x040003E3 RID: 995
		public Vector2 offsetUV;

		// Token: 0x040003E4 RID: 996
		[Range(0f, 4f)]
		public float alpha = 1f;

		// Token: 0x040003E5 RID: 997
		public Texture mask;

		// Token: 0x040003E6 RID: 998
		public OVERLAY_MASK_MODE maskMode = OVERLAY_MASK_MODE.WAVES_AND_OVERLAY_BLEND;

		// Token: 0x040003E7 RID: 999
		[Range(0f, 1f)]
		public float maskAlpha = 1f;
	}
}
