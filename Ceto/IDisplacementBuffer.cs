using System;
using System.Collections.Generic;
using Ceto.Common.Containers.Interpolation;
using UnityEngine;

namespace Ceto
{
	// Token: 0x0200009B RID: 155
	public interface IDisplacementBuffer
	{
		// Token: 0x170000DD RID: 221
		// (get) Token: 0x0600043B RID: 1083
		bool IsGPU { get; }

		// Token: 0x0600043C RID: 1084
		InterpolatedArray2f[] GetReadDisplacements();

		// Token: 0x0600043D RID: 1085
		void CopyAndCreateDisplacements(out IList<InterpolatedArray2f> displacements);

		// Token: 0x0600043E RID: 1086
		void CopyDisplacements(IList<InterpolatedArray2f> des);

		// Token: 0x0600043F RID: 1087
		Vector4 MaxRange(Vector4 choppyness, Vector2 gridScale);

		// Token: 0x06000440 RID: 1088
		void QueryWaves(WaveQuery query, QueryGridScaling scaling);

		// Token: 0x06000441 RID: 1089
		int EnabledBuffers();
	}
}
