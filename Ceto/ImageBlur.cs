using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x020000CB RID: 203
	public class ImageBlur
	{
		// Token: 0x060005B5 RID: 1461 RVA: 0x00026A38 File Offset: 0x00024C38
		public ImageBlur(Shader blurShader)
		{
			this.BlurIterations = 1;
			this.BlurSpread = 0.6f;
			this.BlurMode = ImageBlur.BLUR_MODE.DOWNSAMPLE_2;
			if (blurShader != null)
			{
				this.m_blurMaterial = new Material(blurShader);
			}
		}

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x060005B6 RID: 1462 RVA: 0x00026A7C File Offset: 0x00024C7C
		// (set) Token: 0x060005B7 RID: 1463 RVA: 0x00026A84 File Offset: 0x00024C84
		public ImageBlur.BLUR_MODE BlurMode { get; set; }

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x060005B8 RID: 1464 RVA: 0x00026A90 File Offset: 0x00024C90
		// (set) Token: 0x060005B9 RID: 1465 RVA: 0x00026A98 File Offset: 0x00024C98
		public int BlurIterations { get; set; }

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x060005BA RID: 1466 RVA: 0x00026AA4 File Offset: 0x00024CA4
		// (set) Token: 0x060005BB RID: 1467 RVA: 0x00026AAC File Offset: 0x00024CAC
		public float BlurSpread { get; set; }

		// Token: 0x060005BC RID: 1468 RVA: 0x00026AB8 File Offset: 0x00024CB8
		public void Blur(RenderTexture source)
		{
			int blurMode = (int)this.BlurMode;
			if (this.BlurIterations > 0 && this.m_blurMaterial != null && blurMode > 0)
			{
				int width = source.width / blurMode;
				int height = source.height / blurMode;
				RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, source.format, RenderTextureReadWrite.Default);
				this.DownSample4x(source, renderTexture);
				for (int i = 0; i < this.BlurIterations; i++)
				{
					RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, source.format, RenderTextureReadWrite.Default);
					this.FourTapCone(renderTexture, temporary, i);
					RenderTexture.ReleaseTemporary(renderTexture);
					renderTexture = temporary;
				}
				Graphics.Blit(renderTexture, source);
				RenderTexture.ReleaseTemporary(renderTexture);
			}
		}

		// Token: 0x060005BD RID: 1469 RVA: 0x00026B68 File Offset: 0x00024D68
		private void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
		{
			float num = 0.5f + (float)iteration * this.BlurSpread;
			Graphics.BlitMultiTap(source, dest, this.m_blurMaterial, new Vector2[]
			{
				new Vector2(-num, -num),
				new Vector2(-num, num),
				new Vector2(num, num),
				new Vector2(num, -num)
			});
		}

		// Token: 0x060005BE RID: 1470 RVA: 0x00026BE8 File Offset: 0x00024DE8
		private void DownSample4x(RenderTexture source, RenderTexture dest)
		{
			float num = 1f;
			Graphics.BlitMultiTap(source, dest, this.m_blurMaterial, new Vector2[]
			{
				new Vector2(-num, -num),
				new Vector2(-num, num),
				new Vector2(num, num),
				new Vector2(num, -num)
			});
		}

		// Token: 0x040005B0 RID: 1456
		public Material m_blurMaterial;

		// Token: 0x020000CC RID: 204
		public enum BLUR_MODE
		{
			// Token: 0x040005B5 RID: 1461
			OFF,
			// Token: 0x040005B6 RID: 1462
			NO_DOWNSAMPLE,
			// Token: 0x040005B7 RID: 1463
			DOWNSAMPLE_2,
			// Token: 0x040005B8 RID: 1464
			DOWNSAMPLE_4 = 4
		}
	}
}
