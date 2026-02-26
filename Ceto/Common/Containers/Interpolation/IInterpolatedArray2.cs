using System;

namespace Ceto.Common.Containers.Interpolation
{
	// Token: 0x0200003E RID: 62
	public interface IInterpolatedArray2
	{
		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060001C4 RID: 452
		int SX { get; }

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060001C5 RID: 453
		int SY { get; }

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060001C6 RID: 454
		int Channels { get; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060001C7 RID: 455
		// (set) Token: 0x060001C8 RID: 456
		bool HalfPixelOffset { get; set; }

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x060001C9 RID: 457
		// (set) Token: 0x060001CA RID: 458
		bool Wrap { get; set; }

		// Token: 0x17000070 RID: 112
		float this[int x, int y, int c]
		{
			get;
			set;
		}

		// Token: 0x060001CD RID: 461
		void Clear();

		// Token: 0x060001CE RID: 462
		void Copy(Array data);

		// Token: 0x060001CF RID: 463
		void Set(int x, int y, int c, float v);

		// Token: 0x060001D0 RID: 464
		void Set(int x, int y, float[] v);

		// Token: 0x060001D1 RID: 465
		float Get(int x, int y, int c);

		// Token: 0x060001D2 RID: 466
		void Get(int x, int y, float[] v);

		// Token: 0x060001D3 RID: 467
		void Get(float x, float y, float[] v);

		// Token: 0x060001D4 RID: 468
		float Get(float x, float y, int c);
	}
}
