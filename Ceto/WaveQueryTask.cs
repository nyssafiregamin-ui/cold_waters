using System;
using System.Collections;
using System.Collections.Generic;
using Ceto.Common.Containers.Interpolation;
using Ceto.Common.Threading.Tasks;
using UnityEngine;

namespace Ceto
{
	// Token: 0x02000095 RID: 149
	public class WaveQueryTask : ThreadedTask
	{
		// Token: 0x0600040C RID: 1036 RVA: 0x00018DFC File Offset: 0x00016FFC
		public WaveQueryTask(WaveSpectrumBase spectrum, float level, Vector3 offset, IEnumerable<WaveQuery> querys, Action<IEnumerable<WaveQuery>> callBack) : base(true)
		{
			IDisplacementBuffer displacementBuffer = spectrum.DisplacementBuffer;
			displacementBuffer.CopyAndCreateDisplacements(out this.m_displacements);
			this.m_querys = querys;
			this.m_callBack = callBack;
			this.m_enabled = displacementBuffer.EnabledBuffers();
			this.m_level = level;
			Vector4 invGridSizes = default(Vector4);
			invGridSizes.x = 1f / (spectrum.GridSizes.x * spectrum.GridScale);
			invGridSizes.y = 1f / (spectrum.GridSizes.y * spectrum.GridScale);
			invGridSizes.z = 1f / (spectrum.GridSizes.z * spectrum.GridScale);
			invGridSizes.w = 1f / (spectrum.GridSizes.w * spectrum.GridScale);
			this.m_scaling = new QueryGridScaling();
			this.m_scaling.invGridSizes = invGridSizes;
			this.m_scaling.choppyness = spectrum.Choppyness * spectrum.GridScale;
			this.m_scaling.scaleY = spectrum.GridScale;
			this.m_scaling.offset = offset;
			this.m_scaling.tmp = new float[QueryDisplacements.CHANNELS];
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x00018F40 File Offset: 0x00017140
		public override void Start()
		{
			base.Start();
		}

		// Token: 0x0600040E RID: 1038 RVA: 0x00018F48 File Offset: 0x00017148
		public override IEnumerator Run()
		{
			this.RunQueries();
			this.FinishedRunning();
			return null;
		}

		// Token: 0x0600040F RID: 1039 RVA: 0x00018F58 File Offset: 0x00017158
		public override void End()
		{
			base.End();
			this.m_callBack(this.m_querys);
		}

		// Token: 0x06000410 RID: 1040 RVA: 0x00018F74 File Offset: 0x00017174
		private void RunQueries()
		{
			IEnumerator<WaveQuery> enumerator = this.m_querys.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (this.Cancelled)
				{
					return;
				}
				WaveQuery waveQuery = enumerator.Current;
				waveQuery.result.Clear();
				if (waveQuery.mode == QUERY_MODE.DISPLACEMENT || waveQuery.mode == QUERY_MODE.POSITION)
				{
					QueryDisplacements.QueryWaves(waveQuery, this.m_enabled, this.m_displacements, this.m_scaling);
				}
				WaveQuery waveQuery2 = waveQuery;
				waveQuery2.result.height = waveQuery2.result.height + this.m_level;
			}
		}

		// Token: 0x0400043F RID: 1087
		private IList<InterpolatedArray2f> m_displacements;

		// Token: 0x04000440 RID: 1088
		private IEnumerable<WaveQuery> m_querys;

		// Token: 0x04000441 RID: 1089
		private int m_enabled;

		// Token: 0x04000442 RID: 1090
		private Action<IEnumerable<WaveQuery>> m_callBack;

		// Token: 0x04000443 RID: 1091
		private float m_level;

		// Token: 0x04000444 RID: 1092
		private QueryGridScaling m_scaling;
	}
}
