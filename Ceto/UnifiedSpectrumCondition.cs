using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x020000A9 RID: 169
	public class UnifiedSpectrumCondition : WaveSpectrumCondition
	{
		// Token: 0x0600049C RID: 1180 RVA: 0x0001D360 File Offset: 0x0001B560
		public UnifiedSpectrumCondition(int size, float windSpeed, float windDir, float waveAge, int numGrids) : base(size, numGrids)
		{
			if (numGrids < 1 || numGrids > 4)
			{
				throw new ArgumentException("UnifiedSpectrumCondition must have 1 to 4 grids not " + numGrids);
			}
			base.Key = new UnifiedSpectrumConditionKey(windSpeed, waveAge, size, windDir, SPECTRUM_TYPE.UNIFIED, numGrids);
			if (numGrids == 1)
			{
				base.GridSizes = new Vector4(772f, 1f, 1f, 1f);
				base.Choppyness = new Vector4(2.3f, 1f, 1f, 1f);
				base.WaveAmps = new Vector4(1f, 1f, 1f, 1f);
			}
			else if (numGrids == 2)
			{
				base.GridSizes = new Vector4(772f, 57f, 1f, 1f);
				base.Choppyness = new Vector4(2.3f, 2.1f, 1f, 1f);
				base.WaveAmps = new Vector4(1f, 1f, 1f, 1f);
			}
			else if (numGrids == 3)
			{
				base.GridSizes = new Vector4(1372f, 392f, 28f, 1f);
				base.Choppyness = new Vector4(2.3f, 2.1f, 1.6f, 1f);
				base.WaveAmps = new Vector4(1f, 1f, 1f, 1f);
			}
			else if (numGrids == 4)
			{
				base.GridSizes = new Vector4(1372f, 392f, 28f, 4f);
				base.Choppyness = new Vector4(2.3f, 2.1f, 1.6f, 0.9f);
				base.WaveAmps = new Vector4(1f, 1f, 1f, 1f);
			}
		}

		// Token: 0x0600049D RID: 1181 RVA: 0x0001D554 File Offset: 0x0001B754
		public override SpectrumTask GetCreateSpectrumConditionTask()
		{
			UnifiedSpectrumConditionKey unifiedSpectrumConditionKey = base.Key as UnifiedSpectrumConditionKey;
			UnifiedSpectrum unifiedSpectrum = new UnifiedSpectrum(unifiedSpectrumConditionKey.WindSpeed, unifiedSpectrumConditionKey.WindDir, unifiedSpectrumConditionKey.WaveAge);
			return new SpectrumTask(this, true, new ISpectrum[]
			{
				unifiedSpectrum,
				unifiedSpectrum,
				unifiedSpectrum,
				unifiedSpectrum
			});
		}
	}
}
