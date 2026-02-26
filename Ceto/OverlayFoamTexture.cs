using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x0200008C RID: 140
	[Serializable]
	public class OverlayFoamTexture
	{
		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x060003B8 RID: 952 RVA: 0x00016260 File Offset: 0x00014460
		public bool IsDrawable
		{
			get
			{
				return (this.alpha != 0f && this.tex != null) || (this.maskAlpha != 0f && this.mask != null);
			}
		}

		// Token: 0x040003D9 RID: 985
		public Texture tex;

		// Token: 0x040003DA RID: 986
		public Vector2 scaleUV = Vector2.one;

		// Token: 0x040003DB RID: 987
		public Vector2 offsetUV;

		// Token: 0x040003DC RID: 988
		public bool textureFoam = true;

		// Token: 0x040003DD RID: 989
		[Range(0f, 4f)]
		public float alpha = 1f;

		// Token: 0x040003DE RID: 990
		public Texture mask;

		// Token: 0x040003DF RID: 991
		public OVERLAY_MASK_MODE maskMode;

		// Token: 0x040003E0 RID: 992
		[Range(0f, 1f)]
		public float maskAlpha = 1f;
	}
}
