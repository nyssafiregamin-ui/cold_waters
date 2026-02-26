using System;

namespace Ceto
{
	// Token: 0x020000A2 RID: 162
	public class CustomWaveSpectrumCondition : WaveSpectrumCondition
	{
		// Token: 0x0600048A RID: 1162 RVA: 0x0001C6DC File Offset: 0x0001A8DC
		public CustomWaveSpectrumCondition(ICustomWaveSpectrum custom, int size, float windDir, int numGrids) : base(size, numGrids)
		{
			if (numGrids < 1 || numGrids > 4)
			{
				throw new ArgumentException("UCustomSpectrumCondition must have 1 to 4 grids not " + numGrids);
			}
			this.m_custom = custom;
			base.Key = this.m_custom.CreateKey(size, windDir, SPECTRUM_TYPE.CUSTOM, numGrids);
			base.GridSizes = this.m_custom.GetGridSizes(numGrids);
			base.Choppyness = this.m_custom.GetChoppyness(numGrids);
			base.WaveAmps = this.m_custom.GetWaveAmps(numGrids);
		}

		// Token: 0x0600048B RID: 1163 RVA: 0x0001C770 File Offset: 0x0001A970
		public override SpectrumTask GetCreateSpectrumConditionTask()
		{
			ISpectrum spectrum = this.m_custom.CreateSpectrum(base.Key);
			bool multiThreadTask = this.m_custom.MultiThreadTask;
			return new SpectrumTask(this, multiThreadTask, new ISpectrum[]
			{
				spectrum,
				spectrum,
				spectrum,
				spectrum
			});
		}

		// Token: 0x0400047E RID: 1150
		private ICustomWaveSpectrum m_custom;
	}
}
