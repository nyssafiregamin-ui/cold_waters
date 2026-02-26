using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x020000A4 RID: 164
	public class PhillipsSpectrum : ISpectrum
	{
		// Token: 0x0600048D RID: 1165 RVA: 0x0001C7B8 File Offset: 0x0001A9B8
		public PhillipsSpectrum(float windSpeed, float windDir)
		{
			this.WindSpeed = windSpeed;
			float f = windDir * 3.1415927f / 180f;
			this.WindDir = new Vector2(Mathf.Cos(f), Mathf.Sin(f));
			float num = this.WindSpeed * this.WindSpeed / this.GRAVITY;
			this.length2 = num * num;
			float num2 = 0.001f;
			this.dampedLength2 = this.length2 * num2 * num2;
		}

		// Token: 0x0600048E RID: 1166 RVA: 0x0001C844 File Offset: 0x0001AA44
		public float Spectrum(float kx, float kz)
		{
			float num = kx * this.WindDir.x - kz * this.WindDir.y;
			float num2 = kx * this.WindDir.y + kz * this.WindDir.x;
			kx = num;
			kz = num2;
			float num3 = Mathf.Sqrt(kx * kx + kz * kz);
			if (num3 < 1E-06f)
			{
				return 0f;
			}
			float num4 = num3 * num3;
			float num5 = num4 * num4;
			kx /= num3;
			kz /= num3;
			float num6 = kx * 1f + kz * 0f;
			float num7 = num6 * num6 * num6 * num6 * num6 * num6;
			return this.AMP * Mathf.Exp(-1f / (num4 * this.length2)) / num5 * num7 * Mathf.Exp(-num4 * this.dampedLength2);
		}

		// Token: 0x0400047F RID: 1151
		private readonly float GRAVITY = 9.818286f;

		// Token: 0x04000480 RID: 1152
		private readonly float AMP = 0.02f;

		// Token: 0x04000481 RID: 1153
		private readonly float WindSpeed;

		// Token: 0x04000482 RID: 1154
		private readonly Vector2 WindDir;

		// Token: 0x04000483 RID: 1155
		private readonly float length2;

		// Token: 0x04000484 RID: 1156
		private readonly float dampedLength2;
	}
}
