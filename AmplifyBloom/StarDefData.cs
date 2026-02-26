using System;
using UnityEngine;

namespace AmplifyBloom
{
	// Token: 0x02000016 RID: 22
	[Serializable]
	public class StarDefData
	{
		// Token: 0x060000C9 RID: 201 RVA: 0x0000561C File Offset: 0x0000381C
		public StarDefData()
		{
		}

		// Token: 0x060000CA RID: 202 RVA: 0x0000566C File Offset: 0x0000386C
		public StarDefData(StarLibType starType, string starName, int starLinesCount, int passCount, float sampleLength, float attenuation, float inclination, float rotation, float longAttenuation = 0f, float customIncrement = -1f)
		{
			this.m_starType = starType;
			this.m_starName = starName;
			this.m_passCount = passCount;
			this.m_sampleLength = sampleLength;
			this.m_attenuation = attenuation;
			this.m_starlinesCount = starLinesCount;
			this.m_inclination = inclination;
			this.m_rotation = rotation;
			this.m_customIncrement = customIncrement;
			this.m_longAttenuation = longAttenuation;
			this.CalculateStarData();
		}

		// Token: 0x060000CB RID: 203 RVA: 0x0000570C File Offset: 0x0000390C
		public void Destroy()
		{
			this.m_starLinesArr = null;
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00005718 File Offset: 0x00003918
		public void CalculateStarData()
		{
			if (this.m_starlinesCount == 0)
			{
				return;
			}
			this.m_starLinesArr = new StarLineData[this.m_starlinesCount];
			float num = (this.m_customIncrement <= 0f) ? (180f / (float)this.m_starlinesCount) : this.m_customIncrement;
			num *= 0.017453292f;
			for (int i = 0; i < this.m_starlinesCount; i++)
			{
				this.m_starLinesArr[i] = new StarLineData();
				this.m_starLinesArr[i].PassCount = this.m_passCount;
				this.m_starLinesArr[i].SampleLength = this.m_sampleLength;
				if (this.m_longAttenuation > 0f)
				{
					this.m_starLinesArr[i].Attenuation = ((i % 2 != 0) ? this.m_attenuation : this.m_longAttenuation);
				}
				else
				{
					this.m_starLinesArr[i].Attenuation = this.m_attenuation;
				}
				this.m_starLinesArr[i].Inclination = num * (float)i;
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060000CD RID: 205 RVA: 0x0000581C File Offset: 0x00003A1C
		// (set) Token: 0x060000CE RID: 206 RVA: 0x00005824 File Offset: 0x00003A24
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

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060000CF RID: 207 RVA: 0x00005830 File Offset: 0x00003A30
		// (set) Token: 0x060000D0 RID: 208 RVA: 0x00005838 File Offset: 0x00003A38
		public string StarName
		{
			get
			{
				return this.m_starName;
			}
			set
			{
				this.m_starName = value;
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060000D1 RID: 209 RVA: 0x00005844 File Offset: 0x00003A44
		// (set) Token: 0x060000D2 RID: 210 RVA: 0x0000584C File Offset: 0x00003A4C
		public int StarlinesCount
		{
			get
			{
				return this.m_starlinesCount;
			}
			set
			{
				this.m_starlinesCount = value;
				this.CalculateStarData();
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060000D3 RID: 211 RVA: 0x0000585C File Offset: 0x00003A5C
		// (set) Token: 0x060000D4 RID: 212 RVA: 0x00005864 File Offset: 0x00003A64
		public int PassCount
		{
			get
			{
				return this.m_passCount;
			}
			set
			{
				this.m_passCount = value;
				this.CalculateStarData();
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x00005874 File Offset: 0x00003A74
		// (set) Token: 0x060000D6 RID: 214 RVA: 0x0000587C File Offset: 0x00003A7C
		public float SampleLength
		{
			get
			{
				return this.m_sampleLength;
			}
			set
			{
				this.m_sampleLength = value;
				this.CalculateStarData();
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060000D7 RID: 215 RVA: 0x0000588C File Offset: 0x00003A8C
		// (set) Token: 0x060000D8 RID: 216 RVA: 0x00005894 File Offset: 0x00003A94
		public float Attenuation
		{
			get
			{
				return this.m_attenuation;
			}
			set
			{
				this.m_attenuation = value;
				this.CalculateStarData();
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060000D9 RID: 217 RVA: 0x000058A4 File Offset: 0x00003AA4
		// (set) Token: 0x060000DA RID: 218 RVA: 0x000058AC File Offset: 0x00003AAC
		public float Inclination
		{
			get
			{
				return this.m_inclination;
			}
			set
			{
				this.m_inclination = value;
				this.CalculateStarData();
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060000DB RID: 219 RVA: 0x000058BC File Offset: 0x00003ABC
		// (set) Token: 0x060000DC RID: 220 RVA: 0x000058C4 File Offset: 0x00003AC4
		public float CameraRotInfluence
		{
			get
			{
				return this.m_rotation;
			}
			set
			{
				this.m_rotation = value;
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060000DD RID: 221 RVA: 0x000058D0 File Offset: 0x00003AD0
		public StarLineData[] StarLinesArr
		{
			get
			{
				return this.m_starLinesArr;
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060000DE RID: 222 RVA: 0x000058D8 File Offset: 0x00003AD8
		// (set) Token: 0x060000DF RID: 223 RVA: 0x000058E0 File Offset: 0x00003AE0
		public float CustomIncrement
		{
			get
			{
				return this.m_customIncrement;
			}
			set
			{
				this.m_customIncrement = value;
				this.CalculateStarData();
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x000058F0 File Offset: 0x00003AF0
		// (set) Token: 0x060000E1 RID: 225 RVA: 0x000058F8 File Offset: 0x00003AF8
		public float LongAttenuation
		{
			get
			{
				return this.m_longAttenuation;
			}
			set
			{
				this.m_longAttenuation = value;
				this.CalculateStarData();
			}
		}

		// Token: 0x040000C9 RID: 201
		[SerializeField]
		private StarLibType m_starType;

		// Token: 0x040000CA RID: 202
		[SerializeField]
		private string m_starName = string.Empty;

		// Token: 0x040000CB RID: 203
		[SerializeField]
		private int m_starlinesCount = 2;

		// Token: 0x040000CC RID: 204
		[SerializeField]
		private int m_passCount = 4;

		// Token: 0x040000CD RID: 205
		[SerializeField]
		private float m_sampleLength = 1f;

		// Token: 0x040000CE RID: 206
		[SerializeField]
		private float m_attenuation = 0.85f;

		// Token: 0x040000CF RID: 207
		[SerializeField]
		private float m_inclination;

		// Token: 0x040000D0 RID: 208
		[SerializeField]
		private float m_rotation;

		// Token: 0x040000D1 RID: 209
		[SerializeField]
		private StarLineData[] m_starLinesArr;

		// Token: 0x040000D2 RID: 210
		[SerializeField]
		private float m_customIncrement = 90f;

		// Token: 0x040000D3 RID: 211
		[SerializeField]
		private float m_longAttenuation;
	}
}
