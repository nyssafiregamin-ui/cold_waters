using System;

namespace Ceto.Common.Containers.Interpolation
{
	// Token: 0x02000040 RID: 64
	public class InterpolatedArray2f : InterpolatedArray, IInterpolatedArray2
	{
		// Token: 0x060001DC RID: 476 RVA: 0x0000CA58 File Offset: 0x0000AC58
		public InterpolatedArray2f(int sx, int sy, int c, bool wrap) : base(wrap)
		{
			this.m_sx = sx;
			this.m_sy = sy;
			this.m_c = c;
			this.m_data = new float[this.m_sx * this.m_sy * this.m_c];
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000CAA4 File Offset: 0x0000ACA4
		public InterpolatedArray2f(float[] data, int sx, int sy, int c, bool wrap) : base(wrap)
		{
			this.m_sx = sx;
			this.m_sy = sy;
			this.m_c = c;
			this.m_data = new float[this.m_sx * this.m_sy * this.m_c];
			this.Copy(data);
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000CAF8 File Offset: 0x0000ACF8
		public InterpolatedArray2f(float[,,] data, bool wrap) : base(wrap)
		{
			this.m_sx = data.GetLength(0);
			this.m_sy = data.GetLength(1);
			this.m_c = data.GetLength(2);
			this.m_data = new float[this.m_sx * this.m_sy * this.m_c];
			this.Copy(data);
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x060001DF RID: 479 RVA: 0x0000CB5C File Offset: 0x0000AD5C
		public float[] Data
		{
			get
			{
				return this.m_data;
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x060001E0 RID: 480 RVA: 0x0000CB64 File Offset: 0x0000AD64
		public int SX
		{
			get
			{
				return this.m_sx;
			}
		}

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x060001E1 RID: 481 RVA: 0x0000CB6C File Offset: 0x0000AD6C
		public int SY
		{
			get
			{
				return this.m_sy;
			}
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x060001E2 RID: 482 RVA: 0x0000CB74 File Offset: 0x0000AD74
		public int Channels
		{
			get
			{
				return this.m_c;
			}
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000CB7C File Offset: 0x0000AD7C
		public void Clear()
		{
			Array.Clear(this.m_data, 0, this.m_data.Length);
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000CB94 File Offset: 0x0000AD94
		public void Copy(Array data)
		{
			Array.Copy(data, this.m_data, this.m_data.Length);
		}

		// Token: 0x17000077 RID: 119
		public float this[int x, int y, int c]
		{
			get
			{
				return this.m_data[(x + y * this.m_sx) * this.m_c + c];
			}
			set
			{
				this.m_data[(x + y * this.m_sx) * this.m_c + c] = value;
			}
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0000CBE8 File Offset: 0x0000ADE8
		public float Get(int x, int y, int c)
		{
			return this.m_data[(x + y * this.m_sx) * this.m_c + c];
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x0000CC04 File Offset: 0x0000AE04
		public void Set(int x, int y, int c, float v)
		{
			this.m_data[(x + y * this.m_sx) * this.m_c + c] = v;
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000CC24 File Offset: 0x0000AE24
		public void Set(int x, int y, float[] v)
		{
			for (int i = 0; i < this.m_c; i++)
			{
				this.m_data[(x + y * this.m_sx) * this.m_c + i] = v[i];
			}
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0000CC68 File Offset: 0x0000AE68
		public void Get(int x, int y, float[] v)
		{
			for (int i = 0; i < this.m_c; i++)
			{
				v[i] = this.m_data[(x + y * this.m_sx) * this.m_c + i];
			}
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000CCAC File Offset: 0x0000AEAC
		public void Get(float x, float y, float[] v)
		{
			if (base.HalfPixelOffset)
			{
				x *= (float)this.m_sx;
				y *= (float)this.m_sy;
				x -= 0.5f;
				y -= 0.5f;
			}
			else
			{
				x *= (float)(this.m_sx - 1);
				y *= (float)(this.m_sy - 1);
			}
			float num = Math.Abs(x - (float)((int)x));
			int num2;
			int num3;
			base.Index((double)x, this.m_sx, out num2, out num3);
			float num4 = Math.Abs(y - (float)((int)y));
			int num5;
			int num6;
			base.Index((double)y, this.m_sy, out num5, out num6);
			for (int i = 0; i < this.m_c; i++)
			{
				float num7 = this.m_data[(num2 + num5 * this.m_sx) * this.m_c + i] * (1f - num) + this.m_data[(num3 + num5 * this.m_sx) * this.m_c + i] * num;
				float num8 = this.m_data[(num2 + num6 * this.m_sx) * this.m_c + i] * (1f - num) + this.m_data[(num3 + num6 * this.m_sx) * this.m_c + i] * num;
				v[i] = num7 * (1f - num4) + num8 * num4;
			}
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000CE00 File Offset: 0x0000B000
		public float Get(float x, float y, int c)
		{
			if (base.HalfPixelOffset)
			{
				x *= (float)this.m_sx;
				y *= (float)this.m_sy;
				x -= 0.5f;
				y -= 0.5f;
			}
			else
			{
				x *= (float)(this.m_sx - 1);
				y *= (float)(this.m_sy - 1);
			}
			float num = Math.Abs(x - (float)((int)x));
			int num2;
			int num3;
			base.Index((double)x, this.m_sx, out num2, out num3);
			float num4 = Math.Abs(y - (float)((int)y));
			int num5;
			int num6;
			base.Index((double)y, this.m_sy, out num5, out num6);
			float num7 = this.m_data[(num2 + num5 * this.m_sx) * this.m_c + c] * (1f - num) + this.m_data[(num3 + num5 * this.m_sx) * this.m_c + c] * num;
			float num8 = this.m_data[(num2 + num6 * this.m_sx) * this.m_c + c] * (1f - num) + this.m_data[(num3 + num6 * this.m_sx) * this.m_c + c] * num;
			return num7 * (1f - num4) + num8 * num4;
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000CF30 File Offset: 0x0000B130
		virtual bool get_HalfPixelOffset()
		{
			return base.HalfPixelOffset;
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000CF38 File Offset: 0x0000B138
		virtual void set_HalfPixelOffset(bool value)
		{
			base.HalfPixelOffset = value;
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000CF44 File Offset: 0x0000B144
		virtual bool get_Wrap()
		{
			return base.Wrap;
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000CF4C File Offset: 0x0000B14C
		virtual void set_Wrap(bool value)
		{
			base.Wrap = value;
		}

		// Token: 0x04000251 RID: 593
		private float[] m_data;

		// Token: 0x04000252 RID: 594
		private int m_sx;

		// Token: 0x04000253 RID: 595
		private int m_sy;

		// Token: 0x04000254 RID: 596
		private int m_c;
	}
}
