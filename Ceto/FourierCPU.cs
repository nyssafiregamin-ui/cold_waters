using System;
using System.Collections.Generic;
using Ceto.Common.Threading.Tasks;
using UnityEngine;

namespace Ceto
{
	// Token: 0x020000B0 RID: 176
	public class FourierCPU
	{
		// Token: 0x060004E7 RID: 1255 RVA: 0x0001DFD4 File Offset: 0x0001C1D4
		public FourierCPU(int size)
		{
			if (!Mathf.IsPowerOfTwo(size))
			{
				throw new ArgumentException("Fourier grid size must be pow2 number");
			}
			this.m_size = size;
			this.m_fsize = (float)this.m_size;
			this.m_passes = (int)(Mathf.Log(this.m_fsize) / Mathf.Log(2f));
			this.ComputeButterflyLookupTable();
		}

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x060004E8 RID: 1256 RVA: 0x0001E034 File Offset: 0x0001C234
		public int size
		{
			get
			{
				return this.m_size;
			}
		}

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x060004E9 RID: 1257 RVA: 0x0001E03C File Offset: 0x0001C23C
		public int passes
		{
			get
			{
				return this.m_passes;
			}
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x0001E044 File Offset: 0x0001C244
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

		// Token: 0x060004EB RID: 1259 RVA: 0x0001E090 File Offset: 0x0001C290
		private void ComputeButterflyLookupTable()
		{
			this.m_butterflyLookupTable = new FourierCPU.LookUp[this.m_size * this.m_passes];
			for (int i = 0; i < this.m_passes; i++)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.m_passes - 1 - i));
				int num2 = (int)Mathf.Pow(2f, (float)i);
				for (int j = 0; j < num; j++)
				{
					for (int k = 0; k < num2; k++)
					{
						int num3;
						int num4;
						int j2;
						int j3;
						if (i == 0)
						{
							num3 = j * num2 * 2 + k;
							num4 = j * num2 * 2 + num2 + k;
							j2 = this.BitReverse(num3);
							j3 = this.BitReverse(num4);
						}
						else
						{
							num3 = j * num2 * 2 + k;
							num4 = j * num2 * 2 + num2 + k;
							j2 = num3;
							j3 = num4;
						}
						float num5 = Mathf.Cos(6.2831855f * (float)(k * num) / this.m_fsize);
						float num6 = Mathf.Sin(6.2831855f * (float)(k * num) / this.m_fsize);
						int num7 = num3 + i * this.m_size;
						this.m_butterflyLookupTable[num7].j1 = j2;
						this.m_butterflyLookupTable[num7].j2 = j3;
						this.m_butterflyLookupTable[num7].wr = num5;
						this.m_butterflyLookupTable[num7].wi = num6;
						int num8 = num4 + i * this.m_size;
						this.m_butterflyLookupTable[num8].j1 = j2;
						this.m_butterflyLookupTable[num8].j2 = j3;
						this.m_butterflyLookupTable[num8].wr = -num5;
						this.m_butterflyLookupTable[num8].wi = -num6;
					}
				}
			}
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x0001E25C File Offset: 0x0001C45C
		private Vector4 FFT(Vector2 w, Vector4 input1, Vector4 input2)
		{
			input1.x += w.x * input2.x - w.y * input2.y;
			input1.y += w.y * input2.x + w.x * input2.y;
			input1.z += w.x * input2.z - w.y * input2.w;
			input1.w += w.y * input2.z + w.x * input2.w;
			return input1;
		}

		// Token: 0x060004ED RID: 1261 RVA: 0x0001E320 File Offset: 0x0001C520
		private Vector2 FFT(Vector2 w, Vector2 input1, Vector2 input2)
		{
			input1.x += w.x * input2.x - w.y * input2.y;
			input1.y += w.y * input2.x + w.x * input2.y;
			return input1;
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x0001E388 File Offset: 0x0001C588
		public int PeformFFT_SinglePacked(int startIdx, IList<Vector4[]> data0, ICancelToken token)
		{
			int num = 0;
			int num2 = startIdx;
			int i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int index = (num2 + 1) % 2;
				Vector4[] array = data0[num];
				Vector4[] array2 = data0[index];
				int num3 = i * this.m_size;
				for (int j = 0; j < this.m_size; j++)
				{
					int num4 = j + num3;
					int num5 = this.m_butterflyLookupTable[num4].j1;
					int num6 = this.m_butterflyLookupTable[num4].j2;
					float wr = this.m_butterflyLookupTable[num4].wr;
					float wi = this.m_butterflyLookupTable[num4].wi;
					for (int k = 0; k < this.m_size; k++)
					{
						if (token.Cancelled)
						{
							return -1;
						}
						int num7 = k * this.m_size;
						int num8 = j + num7;
						int num9 = num5 + num7;
						int num10 = num6 + num7;
						array[num8].x = array2[num9].x + wr * array2[num10].x - wi * array2[num10].y;
						array[num8].y = array2[num9].y + wi * array2[num10].x + wr * array2[num10].y;
					}
				}
				i++;
				num2++;
			}
			i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int index = (num2 + 1) % 2;
				Vector4[] array3 = data0[num];
				Vector4[] array4 = data0[index];
				int num3 = i * this.m_size;
				for (int k = 0; k < this.m_size; k++)
				{
					int num4 = k + num3;
					int num5 = this.m_butterflyLookupTable[num4].j1 * this.m_size;
					int num6 = this.m_butterflyLookupTable[num4].j2 * this.m_size;
					float wr = this.m_butterflyLookupTable[num4].wr;
					float wi = this.m_butterflyLookupTable[num4].wi;
					for (int j = 0; j < this.m_size; j++)
					{
						if (token.Cancelled)
						{
							return -1;
						}
						int num8 = j + k * this.m_size;
						int num9 = j + num5;
						int num10 = j + num6;
						array3[num8].x = array4[num9].x + wr * array4[num10].x - wi * array4[num10].y;
						array3[num8].y = array4[num9].y + wi * array4[num10].x + wr * array4[num10].y;
					}
				}
				i++;
				num2++;
			}
			return num;
		}

		// Token: 0x060004EF RID: 1263 RVA: 0x0001E690 File Offset: 0x0001C890
		public int PeformFFT_DoublePacked(int startIdx, IList<Vector4[]> data0, ICancelToken token)
		{
			int num = 0;
			int num2 = startIdx;
			int i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int index = (num2 + 1) % 2;
				Vector4[] array = data0[num];
				Vector4[] array2 = data0[index];
				int num3 = i * this.m_size;
				for (int j = 0; j < this.m_size; j++)
				{
					int num4 = j + num3;
					int num5 = this.m_butterflyLookupTable[num4].j1;
					int num6 = this.m_butterflyLookupTable[num4].j2;
					float wr = this.m_butterflyLookupTable[num4].wr;
					float wi = this.m_butterflyLookupTable[num4].wi;
					for (int k = 0; k < this.m_size; k++)
					{
						if (token.Cancelled)
						{
							return -1;
						}
						int num7 = k * this.m_size;
						int num8 = j + num7;
						int num9 = num5 + num7;
						int num10 = num6 + num7;
						array[num8].x = array2[num9].x + wr * array2[num10].x - wi * array2[num10].y;
						array[num8].y = array2[num9].y + wi * array2[num10].x + wr * array2[num10].y;
						array[num8].z = array2[num9].z + wr * array2[num10].z - wi * array2[num10].w;
						array[num8].w = array2[num9].w + wi * array2[num10].z + wr * array2[num10].w;
					}
				}
				i++;
				num2++;
			}
			i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int index = (num2 + 1) % 2;
				Vector4[] array3 = data0[num];
				Vector4[] array4 = data0[index];
				int num3 = i * this.m_size;
				for (int k = 0; k < this.m_size; k++)
				{
					int num4 = k + num3;
					int num5 = this.m_butterflyLookupTable[num4].j1 * this.m_size;
					int num6 = this.m_butterflyLookupTable[num4].j2 * this.m_size;
					float wr = this.m_butterflyLookupTable[num4].wr;
					float wi = this.m_butterflyLookupTable[num4].wi;
					for (int j = 0; j < this.m_size; j++)
					{
						if (token.Cancelled)
						{
							return -1;
						}
						int num8 = j + k * this.m_size;
						int num9 = j + num5;
						int num10 = j + num6;
						array3[num8].x = array4[num9].x + wr * array4[num10].x - wi * array4[num10].y;
						array3[num8].y = array4[num9].y + wi * array4[num10].x + wr * array4[num10].y;
						array3[num8].z = array4[num9].z + wr * array4[num10].z - wi * array4[num10].w;
						array3[num8].w = array4[num9].w + wi * array4[num10].z + wr * array4[num10].w;
					}
				}
				i++;
				num2++;
			}
			return num;
		}

		// Token: 0x040004BA RID: 1210
		private int m_size;

		// Token: 0x040004BB RID: 1211
		private float m_fsize;

		// Token: 0x040004BC RID: 1212
		private int m_passes;

		// Token: 0x040004BD RID: 1213
		private FourierCPU.LookUp[] m_butterflyLookupTable;

		// Token: 0x020000B1 RID: 177
		private struct LookUp
		{
			// Token: 0x040004BE RID: 1214
			public int j1;

			// Token: 0x040004BF RID: 1215
			public int j2;

			// Token: 0x040004C0 RID: 1216
			public float wr;

			// Token: 0x040004C1 RID: 1217
			public float wi;
		}
	}
}
