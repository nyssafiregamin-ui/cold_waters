using System;

namespace Ceto
{
	// Token: 0x020000AA RID: 170
	public class UnifiedSpectrumConditionKey : WaveSpectrumConditionKey
	{
		// Token: 0x0600049E RID: 1182 RVA: 0x0001D5A4 File Offset: 0x0001B7A4
		public UnifiedSpectrumConditionKey(float windSpeed, float waveAge, int size, float windDir, SPECTRUM_TYPE spectrumType, int numGrids) : base(size, windDir, spectrumType, numGrids)
		{
			this.WindSpeed = windSpeed;
			this.WaveAge = waveAge;
		}

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x0600049F RID: 1183 RVA: 0x0001D5C4 File Offset: 0x0001B7C4
		// (set) Token: 0x060004A0 RID: 1184 RVA: 0x0001D5CC File Offset: 0x0001B7CC
		public float WindSpeed { get; private set; }

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x060004A1 RID: 1185 RVA: 0x0001D5D8 File Offset: 0x0001B7D8
		// (set) Token: 0x060004A2 RID: 1186 RVA: 0x0001D5E0 File Offset: 0x0001B7E0
		public float WaveAge { get; private set; }

		// Token: 0x060004A3 RID: 1187 RVA: 0x0001D5EC File Offset: 0x0001B7EC
		protected override bool Matches(WaveSpectrumConditionKey k)
		{
			UnifiedSpectrumConditionKey unifiedSpectrumConditionKey = k as UnifiedSpectrumConditionKey;
			return !(unifiedSpectrumConditionKey == null) && this.WindSpeed == unifiedSpectrumConditionKey.WindSpeed && this.WaveAge == unifiedSpectrumConditionKey.WaveAge;
		}

		// Token: 0x060004A4 RID: 1188 RVA: 0x0001D638 File Offset: 0x0001B838
		protected override int AddToHashCode(int hashcode)
		{
			hashcode = hashcode * 37 + this.WindSpeed.GetHashCode();
			hashcode = hashcode * 37 + this.WaveAge.GetHashCode();
			return hashcode;
		}
	}
}
