using System;
using UnityEngine;

namespace AmplifyBloom
{
	// Token: 0x0200000D RID: 13
	[Serializable]
	public class GlareDefData
	{
		// Token: 0x0600006E RID: 110 RVA: 0x00003DF8 File Offset: 0x00001FF8
		public GlareDefData()
		{
			this.m_customStarData = new StarDefData();
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00003E14 File Offset: 0x00002014
		public GlareDefData(StarLibType starType, float starInclination, float chromaticAberration)
		{
			this.m_starType = starType;
			this.m_starInclination = starInclination;
			this.m_chromaticAberration = chromaticAberration;
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000070 RID: 112 RVA: 0x00003E44 File Offset: 0x00002044
		// (set) Token: 0x06000071 RID: 113 RVA: 0x00003E4C File Offset: 0x0000204C
		public StarLibType StarType
		{
			get
			{
				return this.m_starType;
			}
			set
			{
				this.m_starType = value;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000072 RID: 114 RVA: 0x00003E58 File Offset: 0x00002058
		// (set) Token: 0x06000073 RID: 115 RVA: 0x00003E60 File Offset: 0x00002060
		public float StarInclination
		{
			get
			{
				return this.m_starInclination;
			}
			set
			{
				this.m_starInclination = value;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000074 RID: 116 RVA: 0x00003E6C File Offset: 0x0000206C
		// (set) Token: 0x06000075 RID: 117 RVA: 0x00003E7C File Offset: 0x0000207C
		public float StarInclinationDeg
		{
			get
			{
				return this.m_starInclination * 57.29578f;
			}
			set
			{
				this.m_starInclination = value * 0.017453292f;
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000076 RID: 118 RVA: 0x00003E8C File Offset: 0x0000208C
		// (set) Token: 0x06000077 RID: 119 RVA: 0x00003E94 File Offset: 0x00002094
		public float ChromaticAberration
		{
			get
			{
				return this.m_chromaticAberration;
			}
			set
			{
				this.m_chromaticAberration = value;
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000078 RID: 120 RVA: 0x00003EA0 File Offset: 0x000020A0
		// (set) Token: 0x06000079 RID: 121 RVA: 0x00003EA8 File Offset: 0x000020A8
		public StarDefData CustomStarData
		{
			get
			{
				return this.m_customStarData;
			}
			set
			{
				this.m_customStarData = value;
			}
		}

		// Token: 0x04000086 RID: 134
		public bool FoldoutValue = true;

		// Token: 0x04000087 RID: 135
		[SerializeField]
		private StarLibType m_starType;

		// Token: 0x04000088 RID: 136
		[SerializeField]
		private float m_starInclination;

		// Token: 0x04000089 RID: 137
		[SerializeField]
		private float m_chromaticAberration;

		// Token: 0x0400008A RID: 138
		[SerializeField]
		private StarDefData m_customStarData;
	}
}
