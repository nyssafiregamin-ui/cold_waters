using System;
using UnityEngine;

namespace mset
{
	// Token: 0x020000E6 RID: 230
	[Serializable]
	public class SHEncoding
	{
		// Token: 0x0600060C RID: 1548 RVA: 0x0002A0C8 File Offset: 0x000282C8
		public SHEncoding()
		{
			this.clearToBlack();
		}

		// Token: 0x0600060E RID: 1550 RVA: 0x0002A118 File Offset: 0x00028318
		public void clearToBlack()
		{
			for (int i = 0; i < 27; i++)
			{
				this.c[i] = 0f;
			}
			for (int j = 0; j < 9; j++)
			{
				this.cBuffer[j] = Vector4.zero;
			}
		}

		// Token: 0x0600060F RID: 1551 RVA: 0x0002A170 File Offset: 0x00028370
		public void copyFrom(SHEncoding src)
		{
			for (int i = 0; i < 27; i++)
			{
				this.c[i] = src.c[i];
			}
			this.copyToBuffer();
		}

		// Token: 0x06000610 RID: 1552 RVA: 0x0002A1A8 File Offset: 0x000283A8
		public void copyToBuffer()
		{
			for (int i = 0; i < 9; i++)
			{
				float num = SHEncoding.sEquationConstants[i];
				this.cBuffer[i].x = this.c[i * 3] * num;
				this.cBuffer[i].y = this.c[i * 3 + 1] * num;
				this.cBuffer[i].z = this.c[i * 3 + 2] * num;
			}
		}

		// Token: 0x0400064D RID: 1613
		public float[] c = new float[27];

		// Token: 0x0400064E RID: 1614
		public Vector4[] cBuffer = new Vector4[9];

		// Token: 0x0400064F RID: 1615
		public static float[] sEquationConstants = new float[]
		{
			0.28209478f,
			0.4886025f,
			0.4886025f,
			0.4886025f,
			1.0925485f,
			1.0925485f,
			0.31539157f,
			1.0925485f,
			0.54627424f
		};
	}
}
