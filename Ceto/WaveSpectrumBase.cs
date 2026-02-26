using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x020000BA RID: 186
	[DisallowMultipleComponent]
	public abstract class WaveSpectrumBase : OceanComponent
	{
		// Token: 0x17000122 RID: 290
		// (get) Token: 0x0600055E RID: 1374
		public abstract Vector4 GridSizes { get; }

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x0600055F RID: 1375
		public abstract Vector4 Choppyness { get; }

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x06000560 RID: 1376
		public abstract float GridScale { get; }

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x06000561 RID: 1377
		// (set) Token: 0x06000562 RID: 1378
		public abstract Vector2 MaxDisplacement { get; set; }

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x06000563 RID: 1379
		public abstract bool DisableReadBack { get; }

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x06000564 RID: 1380
		public abstract IDisplacementBuffer DisplacementBuffer { get; }

		// Token: 0x06000565 RID: 1381
		public abstract void QueryWaves(WaveQuery query);

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x06000566 RID: 1382 RVA: 0x00023A78 File Offset: 0x00021C78
		// (set) Token: 0x06000567 RID: 1383 RVA: 0x00023A80 File Offset: 0x00021C80
		public ICustomWaveSpectrum CustomWaveSpectrum { get; set; }
	}
}
