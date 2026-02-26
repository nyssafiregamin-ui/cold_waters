using System;
using UnityEngine;

namespace AmplifyBloom
{
	// Token: 0x0200000A RID: 10
	[Serializable]
	public class AmplifyBokehData
	{
		// Token: 0x06000051 RID: 81 RVA: 0x00003868 File Offset: 0x00001A68
		public AmplifyBokehData(Vector4[] offsets)
		{
			this.Offsets = offsets;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003878 File Offset: 0x00001A78
		public void Destroy()
		{
			if (this.BokehRenderTexture != null)
			{
				AmplifyUtils.ReleaseTempRenderTarget(this.BokehRenderTexture);
				this.BokehRenderTexture = null;
			}
			this.Offsets = null;
		}

		// Token: 0x04000071 RID: 113
		internal RenderTexture BokehRenderTexture;

		// Token: 0x04000072 RID: 114
		internal Vector4[] Offsets;
	}
}
