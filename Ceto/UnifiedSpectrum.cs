using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x020000A8 RID: 168
	public class UnifiedSpectrum : ISpectrum
	{
		// Token: 0x06000498 RID: 1176 RVA: 0x0001CEB4 File Offset: 0x0001B0B4
		public UnifiedSpectrum(float windSpeed, float windDir, float waveAge)
		{
			this.WindSpeed = windSpeed;
			this.WaveAge = waveAge;
			float f = windDir * 3.1415927f / 180f;
			this.WindDir = new Vector2(Mathf.Cos(f), Mathf.Sin(f));
			this.U10 = this.WindSpeed;
			this.PI_2 = 6.2831855f;
			this.SQRT_10 = Mathf.Sqrt(10f);
			this.G_SQ_OMEGA_U10 = this.GRAVITY * this.sqr(this.WaveAge / this.U10);
			this.Z_SQ_U10_G = 3.7E-05f * this.sqr(this.U10) / 9.81f;
			this.LOG_OMEGA_6 = Mathf.Log(this.WaveAge) * 6f;
			this.SIGMA = 0.08f * (1f + 4f / Mathf.Pow(this.WaveAge, 3f));
			this.SQ_SIGMA_2 = this.sqr(this.SIGMA) * 2f;
			this.ALPHA_P = 0.006f * Mathf.Sqrt(this.WaveAge);
			this.LOG_2_4 = Mathf.Log(2f) / 4f;
			this.kp = this.G_SQ_OMEGA_U10;
			this.cp = this.omega(this.kp) / this.kp;
			this.z0 = this.Z_SQ_U10_G * Mathf.Pow(this.U10 / this.cp, 0.9f);
			this.u_star = 0.41f * this.U10 / Mathf.Log(10f / this.z0);
			this.gamma = ((this.WaveAge >= 1f) ? (1.7f + this.LOG_OMEGA_6) : 1.7f);
			this.HALF_ALPHA_P_CP = 0.5f * this.ALPHA_P * this.cp;
			this.alpham = 0.01f * ((this.u_star >= this.WAVE_CM) ? (1f + 3f * Mathf.Log(this.u_star / this.WAVE_CM)) : (1f + Mathf.Log(this.u_star / this.WAVE_CM)));
			this.HALF_ALPHAM_WAVE_CM = 0.5f * this.alpham * this.WAVE_CM;
			this.am = 0.13f * this.u_star / this.WAVE_CM;
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x0001D140 File Offset: 0x0001B340
		private float sqr(float x)
		{
			return x * x;
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x0001D148 File Offset: 0x0001B348
		private float omega(float k)
		{
			return Mathf.Sqrt(this.GRAVITY * k * (1f + this.sqr(k / this.WAVE_KM)));
		}

		// Token: 0x0600049B RID: 1179 RVA: 0x0001D178 File Offset: 0x0001B378
		public float Spectrum(float kx, float ky)
		{
			float num = kx * this.WindDir.x - ky * this.WindDir.y;
			float num2 = kx * this.WindDir.y + ky * this.WindDir.x;
			kx = num;
			ky = num2;
			float num3 = Mathf.Sqrt(kx * kx + ky * ky);
			float num4 = this.omega(num3) / num3;
			float num5 = Mathf.Exp(-1.25f * this.sqr(this.kp / num3));
			float p = Mathf.Exp(-1f / this.SQ_SIGMA_2 * this.sqr(Mathf.Sqrt(num3 / this.kp) - 1f));
			float num6 = Mathf.Pow(this.gamma, p);
			float num7 = num5 * num6 * Mathf.Exp(-this.WaveAge / this.SQRT_10 * (Mathf.Sqrt(num3 / this.kp) - 1f));
			float num8 = this.HALF_ALPHA_P_CP / num4 * num7;
			float num9 = Mathf.Exp(-0.25f * this.sqr(num3 / this.WAVE_KM - 1f));
			float num10 = this.HALF_ALPHAM_WAVE_CM / num4 * num9 * num5;
			float num11 = (float)Math.Tanh((double)(this.LOG_2_4 + 4f * Mathf.Pow(num4 / this.cp, 2.5f) + this.am * Mathf.Pow(this.WAVE_CM / num4, 2.5f)));
			float num12 = Mathf.Atan2(ky, kx);
			if (kx < 0f)
			{
				return 0f;
			}
			num8 *= 2f;
			num10 *= 2f;
			float num13 = Mathf.Sqrt(Mathf.Max(kx / num3, 0f));
			return (num8 + num10) * (1f + num11 * Mathf.Cos(2f * num12)) / (this.PI_2 * num3 * num3 * num3 * num3) * num13;
		}

		// Token: 0x04000486 RID: 1158
		private readonly float GRAVITY = 9.818286f;

		// Token: 0x04000487 RID: 1159
		private readonly float WAVE_CM = 0.23f;

		// Token: 0x04000488 RID: 1160
		private readonly float WAVE_KM = 370f;

		// Token: 0x04000489 RID: 1161
		private readonly float U10;

		// Token: 0x0400048A RID: 1162
		private readonly float PI_2;

		// Token: 0x0400048B RID: 1163
		private readonly float SQRT_10;

		// Token: 0x0400048C RID: 1164
		private readonly float G_SQ_OMEGA_U10;

		// Token: 0x0400048D RID: 1165
		private readonly float Z_SQ_U10_G;

		// Token: 0x0400048E RID: 1166
		private readonly float LOG_OMEGA_6;

		// Token: 0x0400048F RID: 1167
		private readonly float SIGMA;

		// Token: 0x04000490 RID: 1168
		private readonly float SQ_SIGMA_2;

		// Token: 0x04000491 RID: 1169
		private readonly float ALPHA_P;

		// Token: 0x04000492 RID: 1170
		private readonly float LOG_2_4;

		// Token: 0x04000493 RID: 1171
		private readonly float kp;

		// Token: 0x04000494 RID: 1172
		private readonly float cp;

		// Token: 0x04000495 RID: 1173
		private readonly float z0;

		// Token: 0x04000496 RID: 1174
		private readonly float u_star;

		// Token: 0x04000497 RID: 1175
		private readonly float gamma;

		// Token: 0x04000498 RID: 1176
		private readonly float HALF_ALPHA_P_CP;

		// Token: 0x04000499 RID: 1177
		private readonly float alpham;

		// Token: 0x0400049A RID: 1178
		private readonly float HALF_ALPHAM_WAVE_CM;

		// Token: 0x0400049B RID: 1179
		private readonly float am;

		// Token: 0x0400049C RID: 1180
		private readonly float WindSpeed;

		// Token: 0x0400049D RID: 1181
		private readonly float WaveAge;

		// Token: 0x0400049E RID: 1182
		private readonly Vector2 WindDir;
	}
}
