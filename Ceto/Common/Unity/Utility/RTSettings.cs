using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto.Common.Unity.Utility
{
	// Token: 0x02000057 RID: 87
	public class RTSettings
	{
		// Token: 0x04000285 RID: 645
		public string name = string.Empty;

		// Token: 0x04000286 RID: 646
		public int width = 1;

		// Token: 0x04000287 RID: 647
		public int height = 1;

		// Token: 0x04000288 RID: 648
		public int depth;

		// Token: 0x04000289 RID: 649
		public int ansioLevel = 1;

		// Token: 0x0400028A RID: 650
		public bool mipmaps;

		// Token: 0x0400028B RID: 651
		public bool randomWrite;

		// Token: 0x0400028C RID: 652
		public RenderTextureReadWrite readWrite;

		// Token: 0x0400028D RID: 653
		public TextureWrapMode wrap = TextureWrapMode.Clamp;

		// Token: 0x0400028E RID: 654
		public FilterMode filer = FilterMode.Bilinear;

		// Token: 0x0400028F RID: 655
		public RenderTextureFormat format;

		// Token: 0x04000290 RID: 656
		public List<RenderTextureFormat> fallbackFormats = new List<RenderTextureFormat>();
	}
}
