using System;

namespace Ceto
{
	// Token: 0x020000A6 RID: 166
	public class PhillipsSpectrumConditionKey : WaveSpectrumConditionKey
	{
		// Token: 0x06000491 RID: 1169 RVA: 0x0001CB60 File Offset: 0x0001AD60
		public PhillipsSpectrumConditionKey(float windSpeed, int size, float windDir, SPECTRUM_TYPE spectrumType, int numGrids) : base(size, windDir, spectrumType, numGrids)
		{
			this.WindSpeed = windSpeed;
		}

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x06000492 RID: 1170 RVA: 0x0001CB78 File Offset: 0x0001AD78
		// (set) Token: 0x06000493 RID: 1171 RVA: 0x0001CB80 File Offset: 0x0001AD80
		public float WindSpeed { get; private set; }

		// Token: 0x06000494 RID: 1172 RVA: 0x0001CB8C File Offset: 0x0001AD8C
		protected override bool Matches(WaveSpectrumConditionKey k)
		{
			PhillipsSpectrumConditionKey phillipsSpectrumConditionKey = k as PhillipsSpectrumConditionKey;
			return !(phillipsSpectrumConditionKey == null) && this.WindSpeed == phillipsSpectrumConditionKey.WindSpeed;
		}

		// Token: 0x06000495 RID: 1173 RVA: 0x0001CBC4 File Offset: 0x0001ADC4
		protected override int AddToHashCode(int hashcode)
		{
			hashcode = hashcode * 37 + this.WindSpeed.GetHashCode();
			return hashcode;
		}
	}
}
