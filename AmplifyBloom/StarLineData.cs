using System;
using UnityEngine;

namespace AmplifyBloom
{
	// Token: 0x02000014 RID: 20
	[Serializable]
	public class StarLineData
	{
		// Token: 0x040000BF RID: 191
		[SerializeField]
		internal int PassCount;

		// Token: 0x040000C0 RID: 192
		[SerializeField]
		internal float SampleLength;

		// Token: 0x040000C1 RID: 193
		[SerializeField]
		internal float Attenuation;

		// Token: 0x040000C2 RID: 194
		[SerializeField]
		internal float Inclination;
	}
}
