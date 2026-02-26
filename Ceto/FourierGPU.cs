using System;
using Ceto.Common.Unity.Utility;
using UnityEngine;

namespace Ceto
{
	// Token: 0x020000B2 RID: 178
	public class FourierGPU
	{
		// Token: 0x060004F0 RID: 1264 RVA: 0x0001EA98 File Offset: 0x0001CC98
		public FourierGPU(int size, Shader sdr)
		{
			if (!Mathf.IsPowerOfTwo(size))
			{
				throw new ArgumentException("Fourier grid size must be pow2 number");
			}
			this.m_fourier = new Material(sdr);
			this.m_size = size;
			this.m_fsize = (float)this.m_size;
			this.m_passes = (int)(Mathf.Log(this.m_fsize) / Mathf.Log(2f));
			this.m_butterflyLookupTable = new Texture2D[this.m_passes];
			this.ComputeButterflyLookupTable();
			this.m_fourier.SetFloat("Ceto_FourierSize", this.m_fsize);
			this.m_pass0RT2 = new RenderBuffer[2];
			this.m_pass1RT2 = new RenderBuffer[2];
			this.m_pass0RT3 = new RenderBuffer[3];
			this.m_pass1RT3 = new RenderBuffer[3];
			this.m_pass0RT4 = new RenderBuffer[4];
			this.m_pass1RT4 = new RenderBuffer[4];
		}

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x060004F1 RID: 1265 RVA: 0x0001EB74 File Offset: 0x0001CD74
		public int size
		{
			get
			{
				return this.m_size;
			}
		}

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x060004F2 RID: 1266 RVA: 0x0001EB7C File Offset: 0x0001CD7C
		public int passes
		{
			get
			{
				return this.m_passes;
			}
		}

		// Token: 0x060004F3 RID: 1267 RVA: 0x0001EB84 File Offset: 0x0001CD84
		public void Release()
		{
			int num = this.m_butterflyLookupTable.Length;
			for (int i = 0; i < num; i++)
			{
				UnityEngine.Object.Destroy(this.m_butterflyLookupTable[i]);
			}
			this.m_butterflyLookupTable = null;
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x0001EBC0 File Offset: 0x0001CDC0
		private int BitReverse(int i)
		{
			int num = 0;
			int num2 = 1;
			for (int num3 = this.m_size / 2; num3 != 0; num3 /= 2)
			{
				int num4 = ((i & num3) <= num3 - 1) ? 0 : 1;
				num += num4 * num2;
				num2 *= 2;
			}
			return num;
		}

		// Token: 0x060004F5 RID: 1269 RVA: 0x0001EC0C File Offset: 0x0001CE0C
		private Texture2D Make1DTex(int i)
		{
			return new Texture2D(this.m_size, 1, TextureFormat.RGBAFloat, false, true)
			{
				filterMode = FilterMode.Point,
				wrapMode = TextureWrapMode.Clamp,
				hideFlags = HideFlags.HideAndDontSave,
				name = "Ceto Fouier GPU Butterfly Lookup"
			};
		}

		// Token: 0x060004F6 RID: 1270 RVA: 0x0001EC4C File Offset: 0x0001CE4C
		private void ComputeButterflyLookupTable()
		{
			float num = (float)this.m_size;
			float num2 = (float)(this.m_size - 1);
			for (int i = 0; i < this.m_passes; i++)
			{
				int num3 = (int)Mathf.Pow(2f, (float)(this.m_passes - 1 - i));
				int num4 = (int)Mathf.Pow(2f, (float)i);
				this.m_butterflyLookupTable[i] = this.Make1DTex(i);
				for (int j = 0; j < num3; j++)
				{
					for (int k = 0; k < num4; k++)
					{
						int num5;
						int num6;
						int num7;
						int num8;
						if (i == 0)
						{
							num5 = j * num4 * 2 + k;
							num6 = j * num4 * 2 + num4 + k;
							num7 = this.BitReverse(num5);
							num8 = this.BitReverse(num6);
						}
						else
						{
							num5 = j * num4 * 2 + k;
							num6 = j * num4 * 2 + num4 + k;
							num7 = num5;
							num8 = num6;
						}
						float num9 = Mathf.Cos(6.2831855f * (float)(k * num3) / num);
						float num10 = Mathf.Sin(6.2831855f * (float)(k * num3) / num);
						this.m_butterflyLookupTable[i].SetPixel(num5, 0, new Color((float)num7 / num2, (float)num8 / num2, num9, num10));
						this.m_butterflyLookupTable[i].SetPixel(num6, 0, new Color((float)num7 / num2, (float)num8 / num2, -num9, -num10));
					}
				}
				this.m_butterflyLookupTable[i].Apply();
			}
		}

		// Token: 0x060004F7 RID: 1271 RVA: 0x0001EDC0 File Offset: 0x0001CFC0
		public int PeformFFT(RenderTexture[] data0)
		{
			if (this.m_butterflyLookupTable == null)
			{
				return -1;
			}
			RenderTexture dest = data0[0];
			RenderTexture dest2 = data0[1];
			int num = 0;
			int num2 = 0;
			int i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int num3 = (num2 + 1) % 2;
				this.m_fourier.SetTexture("Ceto_ButterFlyLookUp", this.m_butterflyLookupTable[i]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer0", data0[num3]);
				if (num == 0)
				{
					Graphics.Blit(null, dest, this.m_fourier, 0);
				}
				else
				{
					Graphics.Blit(null, dest2, this.m_fourier, 0);
				}
				i++;
				num2++;
			}
			i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int num3 = (num2 + 1) % 2;
				this.m_fourier.SetTexture("Ceto_ButterFlyLookUp", this.m_butterflyLookupTable[i]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer0", data0[num3]);
				if (num == 0)
				{
					Graphics.Blit(null, dest, this.m_fourier, 1);
				}
				else
				{
					Graphics.Blit(null, dest2, this.m_fourier, 1);
				}
				i++;
				num2++;
			}
			return num;
		}

		// Token: 0x060004F8 RID: 1272 RVA: 0x0001EEE4 File Offset: 0x0001D0E4
		public int PeformFFT(RenderTexture[] data0, RenderTexture[] data1)
		{
			if (this.m_butterflyLookupTable == null)
			{
				return -1;
			}
			if (SystemInfo.supportedRenderTargetCount < 2)
			{
				throw new InvalidOperationException("System does not support at least 2 render targets");
			}
			this.m_pass0RT2[0] = data0[0].colorBuffer;
			this.m_pass0RT2[1] = data1[0].colorBuffer;
			this.m_pass1RT2[0] = data0[1].colorBuffer;
			this.m_pass1RT2[1] = data1[1].colorBuffer;
			RenderBuffer depthBuffer = data0[0].depthBuffer;
			RenderBuffer depthBuffer2 = data0[1].depthBuffer;
			int num = 0;
			int num2 = 0;
			int i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int num3 = (num2 + 1) % 2;
				this.m_fourier.SetTexture("Ceto_ButterFlyLookUp", this.m_butterflyLookupTable[i]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer0", data0[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer1", data1[num3]);
				if (num == 0)
				{
					RTUtility.MultiTargetBlit(this.m_pass0RT2, depthBuffer, this.m_fourier, 2);
				}
				else
				{
					RTUtility.MultiTargetBlit(this.m_pass1RT2, depthBuffer2, this.m_fourier, 2);
				}
				i++;
				num2++;
			}
			i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int num3 = (num2 + 1) % 2;
				this.m_fourier.SetTexture("Ceto_ButterFlyLookUp", this.m_butterflyLookupTable[i]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer0", data0[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer1", data1[num3]);
				if (num == 0)
				{
					RTUtility.MultiTargetBlit(this.m_pass0RT2, depthBuffer, this.m_fourier, 3);
				}
				else
				{
					RTUtility.MultiTargetBlit(this.m_pass1RT2, depthBuffer2, this.m_fourier, 3);
				}
				i++;
				num2++;
			}
			return num;
		}

		// Token: 0x060004F9 RID: 1273 RVA: 0x0001F0C8 File Offset: 0x0001D2C8
		public int PeformFFT(RenderTexture[] data0, RenderTexture[] data1, RenderTexture[] data2)
		{
			if (this.m_butterflyLookupTable == null)
			{
				return -1;
			}
			if (SystemInfo.supportedRenderTargetCount < 3)
			{
				throw new InvalidOperationException("System does not support at least 3 render targets");
			}
			this.m_pass0RT3[0] = data0[0].colorBuffer;
			this.m_pass0RT3[1] = data1[0].colorBuffer;
			this.m_pass0RT3[2] = data2[0].colorBuffer;
			this.m_pass1RT3[0] = data0[1].colorBuffer;
			this.m_pass1RT3[1] = data1[1].colorBuffer;
			this.m_pass1RT3[2] = data2[1].colorBuffer;
			RenderBuffer depthBuffer = data0[0].depthBuffer;
			RenderBuffer depthBuffer2 = data0[1].depthBuffer;
			int num = 0;
			int num2 = 0;
			int i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int num3 = (num2 + 1) % 2;
				this.m_fourier.SetTexture("Ceto_ButterFlyLookUp", this.m_butterflyLookupTable[i]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer0", data0[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer1", data1[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer2", data2[num3]);
				if (num == 0)
				{
					RTUtility.MultiTargetBlit(this.m_pass0RT3, depthBuffer, this.m_fourier, 4);
				}
				else
				{
					RTUtility.MultiTargetBlit(this.m_pass1RT3, depthBuffer2, this.m_fourier, 4);
				}
				i++;
				num2++;
			}
			i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int num3 = (num2 + 1) % 2;
				this.m_fourier.SetTexture("Ceto_ButterFlyLookUp", this.m_butterflyLookupTable[i]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer0", data0[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer1", data1[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer2", data2[num3]);
				if (num == 0)
				{
					RTUtility.MultiTargetBlit(this.m_pass0RT3, depthBuffer, this.m_fourier, 5);
				}
				else
				{
					RTUtility.MultiTargetBlit(this.m_pass1RT3, depthBuffer2, this.m_fourier, 5);
				}
				i++;
				num2++;
			}
			return num;
		}

		// Token: 0x060004FA RID: 1274 RVA: 0x0001F304 File Offset: 0x0001D504
		public int PeformFFT(RenderTexture[] data0, RenderTexture[] data1, RenderTexture[] data2, RenderTexture[] data3)
		{
			if (this.m_butterflyLookupTable == null)
			{
				return -1;
			}
			if (SystemInfo.supportedRenderTargetCount < 4)
			{
				throw new InvalidOperationException("System does not support at least 4 render targets");
			}
			this.m_pass0RT4[0] = data0[0].colorBuffer;
			this.m_pass0RT4[1] = data1[0].colorBuffer;
			this.m_pass0RT4[2] = data2[0].colorBuffer;
			this.m_pass0RT4[3] = data3[0].colorBuffer;
			this.m_pass1RT4[0] = data0[1].colorBuffer;
			this.m_pass1RT4[1] = data1[1].colorBuffer;
			this.m_pass1RT4[2] = data2[1].colorBuffer;
			this.m_pass1RT4[3] = data3[1].colorBuffer;
			RenderBuffer depthBuffer = data0[0].depthBuffer;
			RenderBuffer depthBuffer2 = data0[1].depthBuffer;
			int num = 0;
			int num2 = 0;
			int i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int num3 = (num2 + 1) % 2;
				this.m_fourier.SetTexture("Ceto_ButterFlyLookUp", this.m_butterflyLookupTable[i]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer0", data0[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer1", data1[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer2", data2[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer3", data3[num3]);
				if (num == 0)
				{
					RTUtility.MultiTargetBlit(this.m_pass0RT4, depthBuffer, this.m_fourier, 6);
				}
				else
				{
					RTUtility.MultiTargetBlit(this.m_pass1RT4, depthBuffer2, this.m_fourier, 6);
				}
				i++;
				num2++;
			}
			i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int num3 = (num2 + 1) % 2;
				this.m_fourier.SetTexture("Ceto_ButterFlyLookUp", this.m_butterflyLookupTable[i]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer0", data0[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer1", data1[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer2", data2[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer3", data3[num3]);
				if (num == 0)
				{
					RTUtility.MultiTargetBlit(this.m_pass0RT4, depthBuffer, this.m_fourier, 7);
				}
				else
				{
					RTUtility.MultiTargetBlit(this.m_pass1RT4, depthBuffer2, this.m_fourier, 7);
				}
				i++;
				num2++;
			}
			return num;
		}

		// Token: 0x040004C2 RID: 1218
		private const int PASS_X_1 = 0;

		// Token: 0x040004C3 RID: 1219
		private const int PASS_Y_1 = 1;

		// Token: 0x040004C4 RID: 1220
		private const int PASS_X_2 = 2;

		// Token: 0x040004C5 RID: 1221
		private const int PASS_Y_2 = 3;

		// Token: 0x040004C6 RID: 1222
		private const int PASS_X_3 = 4;

		// Token: 0x040004C7 RID: 1223
		private const int PASS_Y_3 = 5;

		// Token: 0x040004C8 RID: 1224
		private const int PASS_X_4 = 6;

		// Token: 0x040004C9 RID: 1225
		private const int PASS_Y_4 = 7;

		// Token: 0x040004CA RID: 1226
		private int m_size;

		// Token: 0x040004CB RID: 1227
		private float m_fsize;

		// Token: 0x040004CC RID: 1228
		private int m_passes;

		// Token: 0x040004CD RID: 1229
		private Texture2D[] m_butterflyLookupTable;

		// Token: 0x040004CE RID: 1230
		private Material m_fourier;

		// Token: 0x040004CF RID: 1231
		private RenderBuffer[] m_pass0RT2;

		// Token: 0x040004D0 RID: 1232
		private RenderBuffer[] m_pass1RT2;

		// Token: 0x040004D1 RID: 1233
		private RenderBuffer[] m_pass0RT3;

		// Token: 0x040004D2 RID: 1234
		private RenderBuffer[] m_pass1RT3;

		// Token: 0x040004D3 RID: 1235
		private RenderBuffer[] m_pass0RT4;

		// Token: 0x040004D4 RID: 1236
		private RenderBuffer[] m_pass1RT4;
	}
}
