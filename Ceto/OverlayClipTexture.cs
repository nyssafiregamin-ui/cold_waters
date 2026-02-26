using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x0200008F RID: 143
	[Serializable]
	public class OverlayClipTexture
	{
		// Token: 0x170000CA RID: 202
		// (get) Token: 0x060003BE RID: 958 RVA: 0x000163E8 File Offset: 0x000145E8
		public bool IsDrawable
		{
			get
			{
				return this.alpha != 0f && this.tex != null;
			}
		}

		// Token: 0x040003F0 RID: 1008
		public Texture tex;

		// Token: 0x040003F1 RID: 1009
		public Vector2 scaleUV = Vector2.one;

		// Token: 0x040003F2 RID: 1010
		public Vector2 offsetUV;

		// Token: 0x040003F3 RID: 1011
		[Range(0f, 4f)]
		public float alpha = 1f;

		// Token: 0x040003F4 RID: 1012
		public bool ignoreQuerys;
	}
}
