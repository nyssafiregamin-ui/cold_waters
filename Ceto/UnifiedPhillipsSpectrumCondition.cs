using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x020000A7 RID: 167
	public class UnifiedPhillipsSpectrumCondition : WaveSpectrumCondition
	{
		// Token: 0x06000496 RID: 1174 RVA: 0x0001CBE8 File Offset: 0x0001ADE8
		public UnifiedPhillipsSpectrumCondition(int size, float windSpeed, float windDir, float waveAge, int numGrids) : base(size, numGrids)
		{
			if (numGrids < 1 || numGrids > 4)
			{
				throw new ArgumentException("UnifiedPhillipsSpectrumCondition must have 1 to 4 grids not " + numGrids);
			}
			base.Key = new UnifiedSpectrumConditionKey(windSpeed, waveAge, size, windDir, SPECTRUM_TYPE.UNIFIED_PHILLIPS, numGrids);
			if (numGrids == 1)
			{
				base.GridSizes = new Vector4(772f, 1f, 1f, 1f);
				base.Choppyness = new Vector4(2.3f, 1f, 1f, 1f);
				base.WaveAmps = new Vector4(1f, 1f, 1f, 1f);
			}
			else if (numGrids == 2)
			{
				base.GridSizes = new Vector4(772f, 97f, 1f, 1f);
				base.Choppyness = new Vector4(2.3f, 1.2f, 1f, 1f);
				base.WaveAmps = new Vector4(1f, 1f, 1f, 1f);
			}
			else if (numGrids == 3)
			{
				base.GridSizes = new Vector4(1372f, 392f, 31f, 1f);
				base.Choppyness = new Vector4(2.3f, 2.1f, 1f, 1f);
				base.WaveAmps = new Vector4(1f, 1f, 1f, 1f);
			}
			else if (numGrids == 4)
			{
				base.GridSizes = new Vector4(1372f, 392f, 31f, 4f);
				base.Choppyness = new Vector4(2.3f, 2.1f, 1f, 0.9f);
				base.WaveAmps = new Vector4(1f, 1f, 1f, 1f);
			}
		}

		// Token: 0x06000497 RID: 1175 RVA: 0x0001CDDC File Offset: 0x0001AFDC
		public override SpectrumTask GetCreateSpectrumConditionTask()
		{
			UnifiedSpectrumConditionKey unifiedSpectrumConditionKey = base.Key as UnifiedSpectrumConditionKey;
			UnifiedSpectrum unifiedSpectrum = new UnifiedSpectrum(unifiedSpectrumConditionKey.WindSpeed, unifiedSpectrumConditionKey.WindDir, unifiedSpectrumConditionKey.WaveAge);
			PhillipsSpectrum phillipsSpectrum = new PhillipsSpectrum(unifiedSpectrumConditionKey.WindSpeed, unifiedSpectrumConditionKey.WindDir);
			if (base.Key.NumGrids == 1)
			{
				bool multiThreadTask = true;
				ISpectrum[] array = new ISpectrum[4];
				array[0] = unifiedSpectrum;
				return new SpectrumTask(this, multiThreadTask, array);
			}
			if (base.Key.NumGrids == 2)
			{
				bool multiThreadTask2 = true;
				ISpectrum[] array2 = new ISpectrum[4];
				array2[0] = unifiedSpectrum;
				array2[1] = phillipsSpectrum;
				return new SpectrumTask(this, multiThreadTask2, array2);
			}
			if (base.Key.NumGrids == 3)
			{
				bool multiThreadTask3 = true;
				ISpectrum[] array3 = new ISpectrum[4];
				array3[0] = unifiedSpectrum;
				array3[1] = unifiedSpectrum;
				array3[2] = phillipsSpectrum;
				return new SpectrumTask(this, multiThreadTask3, array3);
			}
			return new SpectrumTask(this, true, new ISpectrum[]
			{
				unifiedSpectrum,
				unifiedSpectrum,
				phillipsSpectrum,
				unifiedSpectrum
			});
		}
	}
}
