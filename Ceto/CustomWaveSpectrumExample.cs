using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x020000AD RID: 173
	[AddComponentMenu("Ceto/Components/CustomWaveSpectrumExample")]
	[RequireComponent(typeof(Ocean))]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(WaveSpectrum))]
	public class CustomWaveSpectrumExample : MonoBehaviour, ICustomWaveSpectrum
	{
		// Token: 0x17000101 RID: 257
		// (get) Token: 0x060004D7 RID: 1239 RVA: 0x0001DCB8 File Offset: 0x0001BEB8
		public bool MultiThreadTask
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060004D8 RID: 1240 RVA: 0x0001DCBC File Offset: 0x0001BEBC
		private void Awake()
		{
			WaveSpectrum component = base.GetComponent<WaveSpectrum>();
			component.CustomWaveSpectrum = this;
		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x0001DCD8 File Offset: 0x0001BED8
		private void Start()
		{
		}

		// Token: 0x060004DA RID: 1242 RVA: 0x0001DCDC File Offset: 0x0001BEDC
		private void Update()
		{
		}

		// Token: 0x060004DB RID: 1243 RVA: 0x0001DCE0 File Offset: 0x0001BEE0
		public WaveSpectrumConditionKey CreateKey(int size, float windDir, SPECTRUM_TYPE spectrumType, int numGrids)
		{
			return new CustomWaveSpectrumExample.CustomSpectrumConditionKey(this.windSpeed, size, windDir, spectrumType, numGrids);
		}

		// Token: 0x060004DC RID: 1244 RVA: 0x0001DCF4 File Offset: 0x0001BEF4
		public ISpectrum CreateSpectrum(WaveSpectrumConditionKey key)
		{
			CustomWaveSpectrumExample.CustomSpectrumConditionKey customSpectrumConditionKey = key as CustomWaveSpectrumExample.CustomSpectrumConditionKey;
			if (customSpectrumConditionKey == null)
			{
				throw new InvalidCastException("Spectrum condition key is null or not the correct type");
			}
			float num = customSpectrumConditionKey.WindSpeed;
			float windDir = customSpectrumConditionKey.WindDir;
			return new CustomWaveSpectrumExample.CustomSpectrum(num, windDir);
		}

		// Token: 0x060004DD RID: 1245 RVA: 0x0001DD34 File Offset: 0x0001BF34
		public Vector4 GetGridSizes(int numGrids)
		{
			if (numGrids == 4)
			{
				return new Vector4(1372f, 217f, 97f, 31f);
			}
			return new Vector4(217f, 97f, 31f, 1f);
		}

		// Token: 0x060004DE RID: 1246 RVA: 0x0001DD7C File Offset: 0x0001BF7C
		public Vector4 GetChoppyness(int numGrids)
		{
			return new Vector4(1.5f, 1.2f, 1f, 1f);
		}

		// Token: 0x060004DF RID: 1247 RVA: 0x0001DD98 File Offset: 0x0001BF98
		public Vector4 GetWaveAmps(int numGrids)
		{
			if (numGrids == 4)
			{
				return new Vector4(0.5f, 1f, 1f, 1f);
			}
			return new Vector4(0.25f, 0.5f, 1f, 1f);
		}

		// Token: 0x040004B2 RID: 1202
		[Range(0f, 30f)]
		public float windSpeed = 10f;

		// Token: 0x020000AE RID: 174
		public class CustomSpectrumConditionKey : WaveSpectrumConditionKey
		{
			// Token: 0x060004E0 RID: 1248 RVA: 0x0001DDE0 File Offset: 0x0001BFE0
			public CustomSpectrumConditionKey(float windSpeed, int size, float windDir, SPECTRUM_TYPE spectrumType, int numGrids) : base(size, windDir, spectrumType, numGrids)
			{
				this.WindSpeed = windSpeed;
			}

			// Token: 0x17000102 RID: 258
			// (get) Token: 0x060004E1 RID: 1249 RVA: 0x0001DDF8 File Offset: 0x0001BFF8
			// (set) Token: 0x060004E2 RID: 1250 RVA: 0x0001DE00 File Offset: 0x0001C000
			public float WindSpeed { get; private set; }

			// Token: 0x060004E3 RID: 1251 RVA: 0x0001DE0C File Offset: 0x0001C00C
			protected override bool Matches(WaveSpectrumConditionKey k)
			{
				CustomWaveSpectrumExample.CustomSpectrumConditionKey customSpectrumConditionKey = k as CustomWaveSpectrumExample.CustomSpectrumConditionKey;
				return !(customSpectrumConditionKey == null) && this.WindSpeed == customSpectrumConditionKey.WindSpeed;
			}

			// Token: 0x060004E4 RID: 1252 RVA: 0x0001DE44 File Offset: 0x0001C044
			protected override int AddToHashCode(int hashcode)
			{
				hashcode = hashcode * 37 + this.WindSpeed.GetHashCode();
				return hashcode;
			}
		}

		// Token: 0x020000AF RID: 175
		public class CustomSpectrum : ISpectrum
		{
			// Token: 0x060004E5 RID: 1253 RVA: 0x0001DE68 File Offset: 0x0001C068
			public CustomSpectrum(float windSpeed, float windDir)
			{
				this.WindSpeed = windSpeed;
				float f = windDir * 3.1415927f / 180f;
				this.WindDir = new Vector2(Mathf.Cos(f), Mathf.Sin(f));
				float num = this.WindSpeed * this.WindSpeed / this.GRAVITY;
				this.length2 = num * num;
				float num2 = 0.001f;
				this.dampedLength2 = this.length2 * num2 * num2;
			}

			// Token: 0x060004E6 RID: 1254 RVA: 0x0001DEF4 File Offset: 0x0001C0F4
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

			// Token: 0x040004B4 RID: 1204
			private readonly float GRAVITY = 9.818286f;

			// Token: 0x040004B5 RID: 1205
			private readonly float AMP = 0.02f;

			// Token: 0x040004B6 RID: 1206
			private readonly float WindSpeed;

			// Token: 0x040004B7 RID: 1207
			private readonly Vector2 WindDir;

			// Token: 0x040004B8 RID: 1208
			private readonly float length2;

			// Token: 0x040004B9 RID: 1209
			private readonly float dampedLength2;
		}
	}
}
