using System;
using UnityEngine;

namespace AmplifyBloom
{
	// Token: 0x02000010 RID: 16
	[Serializable]
	public class AmplifyStarlineCache
	{
		// Token: 0x06000098 RID: 152 RVA: 0x00004F40 File Offset: 0x00003140
		public AmplifyStarlineCache()
		{
			this.Passes = new AmplifyPassCache[4];
			for (int i = 0; i < 4; i++)
			{
				this.Passes[i] = new AmplifyPassCache();
			}
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00004F80 File Offset: 0x00003180
		public void Destroy()
		{
			for (int i = 0; i < 4; i++)
			{
				this.Passes[i].Destroy();
			}
			this.Passes = null;
		}

		// Token: 0x040000A9 RID: 169
		[SerializeField]
		internal AmplifyPassCache[] Passes;
	}
}
