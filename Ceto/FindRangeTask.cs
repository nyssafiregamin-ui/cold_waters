using System;
using System.Collections;
using System.Collections.Generic;
using Ceto.Common.Containers.Interpolation;
using Ceto.Common.Threading.Tasks;
using UnityEngine;

namespace Ceto
{
	// Token: 0x020000B4 RID: 180
	public class FindRangeTask : ThreadedTask
	{
		// Token: 0x06000501 RID: 1281 RVA: 0x0001F5A0 File Offset: 0x0001D7A0
		public FindRangeTask(WaveSpectrumBase spectrum) : base(true)
		{
			this.m_spectrum = spectrum;
			this.m_choppyness = spectrum.Choppyness;
			this.m_gridScale = new Vector2(spectrum.GridScale, spectrum.GridScale);
			IDisplacementBuffer displacementBuffer = spectrum.DisplacementBuffer;
			displacementBuffer.CopyAndCreateDisplacements(out this.m_displacements);
		}

		// Token: 0x06000502 RID: 1282 RVA: 0x0001F5F4 File Offset: 0x0001D7F4
		public override void Reset()
		{
			base.Reset();
			this.m_choppyness = this.m_spectrum.Choppyness;
			this.m_gridScale = new Vector2(this.m_spectrum.GridScale, this.m_spectrum.GridScale);
			IDisplacementBuffer displacementBuffer = this.m_spectrum.DisplacementBuffer;
			displacementBuffer.CopyDisplacements(this.m_displacements);
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x0001F654 File Offset: 0x0001D854
		public override IEnumerator Run()
		{
			this.m_max = QueryDisplacements.MaxRange(this.m_displacements, this.m_choppyness, this.m_gridScale, this);
			this.FinishedRunning();
			return null;
		}

		// Token: 0x06000504 RID: 1284 RVA: 0x0001F67C File Offset: 0x0001D87C
		public override void End()
		{
			this.m_spectrum.MaxDisplacement = new Vector2(Mathf.Max(this.m_max.x, this.m_max.z), this.m_max.y);
			base.End();
		}

		// Token: 0x040004D5 RID: 1237
		private IList<InterpolatedArray2f> m_displacements;

		// Token: 0x040004D6 RID: 1238
		private WaveSpectrumBase m_spectrum;

		// Token: 0x040004D7 RID: 1239
		private Vector4 m_max;

		// Token: 0x040004D8 RID: 1240
		private Vector4 m_choppyness;

		// Token: 0x040004D9 RID: 1241
		private Vector2 m_gridScale;
	}
}
