using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto
{
	// Token: 0x02000093 RID: 147
	public class WaveQuery
	{
		// Token: 0x06000408 RID: 1032 RVA: 0x00018BB8 File Offset: 0x00016DB8
		public WaveQuery(Vector3 worldPos)
		{
			this.posX = worldPos.x;
			this.posZ = worldPos.z;
			this.minError = 0.1f;
			bool[] array = new bool[4];
			array[0] = true;
			array[1] = true;
			this.sampleSpectrum = array;
			this.sampleOverlay = true;
			this.mode = QUERY_MODE.POSITION;
			this.result.displacement = new Vector3[4];
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x00018C24 File Offset: 0x00016E24
		public WaveQuery(float x, float z)
		{
			this.posX = x;
			this.posZ = z;
			this.minError = 0.1f;
			bool[] array = new bool[4];
			array[0] = true;
			array[1] = true;
			this.sampleSpectrum = array;
			this.sampleOverlay = true;
			this.mode = QUERY_MODE.POSITION;
			this.result.displacement = new Vector3[4];
		}

		// Token: 0x0400042B RID: 1067
		public static readonly float MIN_ERROR = 0.01f;

		// Token: 0x0400042C RID: 1068
		public static readonly int MAX_ITERATIONS = 20;

		// Token: 0x0400042D RID: 1069
		public float minError;

		// Token: 0x0400042E RID: 1070
		public float posX;

		// Token: 0x0400042F RID: 1071
		public float posZ;

		// Token: 0x04000430 RID: 1072
		public readonly bool[] sampleSpectrum;

		// Token: 0x04000431 RID: 1073
		public bool sampleOverlay;

		// Token: 0x04000432 RID: 1074
		public bool overrideIgnoreQuerys;

		// Token: 0x04000433 RID: 1075
		public QUERY_MODE mode;

		// Token: 0x04000434 RID: 1076
		public int tag;

		// Token: 0x04000435 RID: 1077
		public WaveQuery.WaveQueryResult result;

		// Token: 0x02000094 RID: 148
		public struct WaveQueryResult
		{
			// Token: 0x0600040B RID: 1035 RVA: 0x00018C98 File Offset: 0x00016E98
			public void Clear()
			{
				this.height = 0f;
				this.overlayHeight = 0f;
				this.displacementX = 0f;
				this.displacementZ = 0f;
				this.iterations = 0;
				this.error = 0f;
				this.isClipped = false;
				this.overlays = null;
				this.displacement[0].x = 0f;
				this.displacement[0].y = 0f;
				this.displacement[0].z = 0f;
				this.displacement[1].x = 0f;
				this.displacement[1].y = 0f;
				this.displacement[1].z = 0f;
				this.displacement[2].x = 0f;
				this.displacement[2].y = 0f;
				this.displacement[2].z = 0f;
				this.displacement[3].x = 0f;
				this.displacement[3].y = 0f;
				this.displacement[3].z = 0f;
			}

			// Token: 0x04000436 RID: 1078
			public float height;

			// Token: 0x04000437 RID: 1079
			public float displacementX;

			// Token: 0x04000438 RID: 1080
			public float displacementZ;

			// Token: 0x04000439 RID: 1081
			public float overlayHeight;

			// Token: 0x0400043A RID: 1082
			public Vector3[] displacement;

			// Token: 0x0400043B RID: 1083
			public int iterations;

			// Token: 0x0400043C RID: 1084
			public float error;

			// Token: 0x0400043D RID: 1085
			public bool isClipped;

			// Token: 0x0400043E RID: 1086
			public IEnumerable<QueryableOverlayResult> overlays;
		}
	}
}
