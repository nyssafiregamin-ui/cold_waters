using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x020000B3 RID: 179
	public interface ICustomWaveSpectrum
	{
		// Token: 0x17000107 RID: 263
		// (get) Token: 0x060004FB RID: 1275
		bool MultiThreadTask { get; }

		// Token: 0x060004FC RID: 1276
		WaveSpectrumConditionKey CreateKey(int size, float windDir, SPECTRUM_TYPE spectrumType, int numGrids);

		// Token: 0x060004FD RID: 1277
		ISpectrum CreateSpectrum(WaveSpectrumConditionKey key);

		// Token: 0x060004FE RID: 1278
		Vector4 GetGridSizes(int numGrids);

		// Token: 0x060004FF RID: 1279
		Vector4 GetChoppyness(int numGrids);

		// Token: 0x06000500 RID: 1280
		Vector4 GetWaveAmps(int numGrids);
	}
}
