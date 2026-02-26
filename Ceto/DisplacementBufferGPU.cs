using System;
using System.Collections.Generic;
using Ceto.Common.Containers.Interpolation;
using UnityEngine;

namespace Ceto
{
	// Token: 0x0200009A RID: 154
	public class DisplacementBufferGPU : WaveSpectrumBufferGPU, IDisplacementBuffer
	{
		// Token: 0x06000434 RID: 1076 RVA: 0x0001A7D4 File Offset: 0x000189D4
		public DisplacementBufferGPU(int size, Shader fourierSdr) : base(size, fourierSdr, DisplacementBufferGPU.NUM_BUFFERS)
		{
			int grids = QueryDisplacements.GRIDS;
			int channels = QueryDisplacements.CHANNELS;
			this.m_displacements = new InterpolatedArray2f[grids];
			for (int i = 0; i < grids; i++)
			{
				this.m_displacements[i] = new InterpolatedArray2f(size, size, channels, true);
			}
		}

		// Token: 0x06000436 RID: 1078 RVA: 0x0001A834 File Offset: 0x00018A34
		public InterpolatedArray2f[] GetReadDisplacements()
		{
			return this.m_displacements;
		}

		// Token: 0x06000437 RID: 1079 RVA: 0x0001A83C File Offset: 0x00018A3C
		public void CopyAndCreateDisplacements(out IList<InterpolatedArray2f> displacements)
		{
			QueryDisplacements.CopyAndCreateDisplacements(this.m_displacements, out displacements);
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x0001A84C File Offset: 0x00018A4C
		public void CopyDisplacements(IList<InterpolatedArray2f> displacements)
		{
			QueryDisplacements.CopyDisplacements(this.m_displacements, displacements);
		}

		// Token: 0x06000439 RID: 1081 RVA: 0x0001A85C File Offset: 0x00018A5C
		public Vector4 MaxRange(Vector4 choppyness, Vector2 gridScale)
		{
			return QueryDisplacements.MaxRange(this.m_displacements, choppyness, gridScale, null);
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x0001A86C File Offset: 0x00018A6C
		public void QueryWaves(WaveQuery query, QueryGridScaling scaling)
		{
			int num = this.EnabledBuffers();
			if (num == 0)
			{
				return;
			}
			QueryDisplacements.QueryWaves(query, num, this.m_displacements, scaling);
		}

		// Token: 0x0400045E RID: 1118
		private static readonly int NUM_BUFFERS = 3;

		// Token: 0x0400045F RID: 1119
		private InterpolatedArray2f[] m_displacements;
	}
}
