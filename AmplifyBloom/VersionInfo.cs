using System;
using UnityEngine;

namespace AmplifyBloom
{
	// Token: 0x02000019 RID: 25
	[Serializable]
	public class VersionInfo
	{
		// Token: 0x060000EA RID: 234 RVA: 0x00005FC4 File Offset: 0x000041C4
		private VersionInfo()
		{
			this.m_major = 1;
			this.m_minor = 0;
			this.m_release = 9;
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00005FE4 File Offset: 0x000041E4
		private VersionInfo(byte major, byte minor, byte release)
		{
			this.m_major = (int)major;
			this.m_minor = (int)minor;
			this.m_release = (int)release;
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00006010 File Offset: 0x00004210
		public static string StaticToString()
		{
			return string.Format("{0}.{1}.{2}", 1, 0, 9) + VersionInfo.StageSuffix;
		}

		// Token: 0x060000EE RID: 238 RVA: 0x0000603C File Offset: 0x0000423C
		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}", this.m_major, this.m_minor, this.m_release) + VersionInfo.StageSuffix;
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060000EF RID: 239 RVA: 0x00006074 File Offset: 0x00004274
		public int Number
		{
			get
			{
				return this.m_major * 100 + this.m_minor * 10 + this.m_release;
			}
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00006090 File Offset: 0x00004290
		public static VersionInfo Current()
		{
			return new VersionInfo(1, 0, 9);
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x0000609C File Offset: 0x0000429C
		public static bool Matches(VersionInfo version)
		{
			return version.m_major == 1 && version.m_minor == 0 && 9 == version.m_release;
		}

		// Token: 0x040000FE RID: 254
		public const byte Major = 1;

		// Token: 0x040000FF RID: 255
		public const byte Minor = 0;

		// Token: 0x04000100 RID: 256
		public const byte Release = 9;

		// Token: 0x04000101 RID: 257
		private static string StageSuffix = "_dev001";

		// Token: 0x04000102 RID: 258
		[SerializeField]
		private int m_major;

		// Token: 0x04000103 RID: 259
		[SerializeField]
		private int m_minor;

		// Token: 0x04000104 RID: 260
		[SerializeField]
		private int m_release;
	}
}
