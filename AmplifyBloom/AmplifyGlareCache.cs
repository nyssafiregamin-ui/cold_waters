using System;
using UnityEngine;

namespace AmplifyBloom
{
	// Token: 0x02000011 RID: 17
	[Serializable]
	public class AmplifyGlareCache
	{
		// Token: 0x0600009A RID: 154 RVA: 0x00004FB4 File Offset: 0x000031B4
		public AmplifyGlareCache()
		{
			this.Starlines = new AmplifyStarlineCache[4];
			this.CromaticAberrationMat = new Vector4[4, 8];
			for (int i = 0; i < 4; i++)
			{
				this.Starlines[i] = new AmplifyStarlineCache();
			}
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00005000 File Offset: 0x00003200
		public void Destroy()
		{
			for (int i = 0; i < 4; i++)
			{
				this.Starlines[i].Destroy();
			}
			this.Starlines = null;
			this.CromaticAberrationMat = null;
		}

		// Token: 0x040000AA RID: 170
		[SerializeField]
		internal AmplifyStarlineCache[] Starlines;

		// Token: 0x040000AB RID: 171
		[SerializeField]
		internal Vector4 AverageWeight;

		// Token: 0x040000AC RID: 172
		[SerializeField]
		internal Vector4[,] CromaticAberrationMat;

		// Token: 0x040000AD RID: 173
		[SerializeField]
		internal int TotalRT;

		// Token: 0x040000AE RID: 174
		[SerializeField]
		internal GlareDefData GlareDef;

		// Token: 0x040000AF RID: 175
		[SerializeField]
		internal StarDefData StarDef;

		// Token: 0x040000B0 RID: 176
		[SerializeField]
		internal int CurrentPassCount;
	}
}
