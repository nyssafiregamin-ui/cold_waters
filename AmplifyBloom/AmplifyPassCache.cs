using System;
using UnityEngine;

namespace AmplifyBloom
{
	// Token: 0x0200000F RID: 15
	[Serializable]
	public class AmplifyPassCache
	{
		// Token: 0x06000096 RID: 150 RVA: 0x00004F00 File Offset: 0x00003100
		public AmplifyPassCache()
		{
			this.Offsets = new Vector4[16];
			this.Weights = new Vector4[16];
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00004F30 File Offset: 0x00003130
		public void Destroy()
		{
			this.Offsets = null;
			this.Weights = null;
		}

		// Token: 0x040000A7 RID: 167
		[SerializeField]
		internal Vector4[] Offsets;

		// Token: 0x040000A8 RID: 168
		[SerializeField]
		internal Vector4[] Weights;
	}
}
