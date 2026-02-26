using System;

namespace Ceto.Common.Containers.Interpolation
{
	// Token: 0x0200003F RID: 63
	public abstract class InterpolatedArray
	{
		// Token: 0x060001D5 RID: 469 RVA: 0x0000C8F8 File Offset: 0x0000AAF8
		public InterpolatedArray(bool wrap)
		{
			this.m_wrap = wrap;
			this.HalfPixelOffset = true;
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060001D6 RID: 470 RVA: 0x0000C910 File Offset: 0x0000AB10
		// (set) Token: 0x060001D7 RID: 471 RVA: 0x0000C918 File Offset: 0x0000AB18
		public bool Wrap
		{
			get
			{
				return this.m_wrap;
			}
			set
			{
				this.m_wrap = value;
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060001D8 RID: 472 RVA: 0x0000C924 File Offset: 0x0000AB24
		// (set) Token: 0x060001D9 RID: 473 RVA: 0x0000C92C File Offset: 0x0000AB2C
		public bool HalfPixelOffset { get; set; }

		// Token: 0x060001DA RID: 474 RVA: 0x0000C938 File Offset: 0x0000AB38
		public void Index(ref int x, int sx)
		{
			if (this.m_wrap)
			{
				if (x >= sx || x <= -sx)
				{
					x %= sx;
				}
				if (x < 0)
				{
					x = sx - -x;
				}
			}
			else if (x < 0)
			{
				x = 0;
			}
			else if (x >= sx)
			{
				x = sx - 1;
			}
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000C998 File Offset: 0x0000AB98
		public void Index(double x, int sx, out int ix0, out int ix1)
		{
			ix0 = (int)x;
			ix1 = (int)x + Math.Sign(x);
			if (this.m_wrap)
			{
				if (ix0 >= sx || ix0 <= -sx)
				{
					ix0 %= sx;
				}
				if (ix0 < 0)
				{
					ix0 = sx - -ix0;
				}
				if (ix1 >= sx || ix1 <= -sx)
				{
					ix1 %= sx;
				}
				if (ix1 < 0)
				{
					ix1 = sx - -ix1;
				}
			}
			else
			{
				if (ix0 < 0)
				{
					ix0 = 0;
				}
				else if (ix0 >= sx)
				{
					ix0 = sx - 1;
				}
				if (ix1 < 0)
				{
					ix1 = 0;
				}
				else if (ix1 >= sx)
				{
					ix1 = sx - 1;
				}
			}
		}

		// Token: 0x0400024F RID: 591
		private bool m_wrap;
	}
}
