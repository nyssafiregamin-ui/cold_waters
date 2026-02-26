using System;
using UnityEngine;

namespace AmplifyOcclusion
{
	// Token: 0x0200003A RID: 58
	[Serializable]
	public class VersionInfo
	{
		// Token: 0x060001B6 RID: 438 RVA: 0x0000BFA0 File Offset: 0x0000A1A0
		private VersionInfo()
		{
			this.m_major = 1;
			this.m_minor = 1;
			this.m_release = 0;
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x0000BFC0 File Offset: 0x0000A1C0
		private VersionInfo(byte major, byte minor, byte release)
		{
			this.m_major = (int)major;
			this.m_minor = (int)minor;
			this.m_release = (int)release;
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000BFEC File Offset: 0x0000A1EC
		public static string StaticToString()
		{
			return string.Format("{0}.{1}.{2}", 1, 1, 0) + VersionInfo.StageSuffix;
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000C020 File Offset: 0x0000A220
		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}", this.m_major, this.m_minor, this.m_release) + VersionInfo.StageSuffix;
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060001BB RID: 443 RVA: 0x0000C058 File Offset: 0x0000A258
		public int Number
		{
			get
			{
				return this.m_major * 100 + this.m_minor * 10 + this.m_release;
			}
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000C074 File Offset: 0x0000A274
		public static VersionInfo Current()
		{
			return new VersionInfo(1, 1, 0);
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000C080 File Offset: 0x0000A280
		public static bool Matches(VersionInfo version)
		{
			return version.m_major == 1 && version.m_minor == 1 && 0 == version.m_release;
		}

		// Token: 0x04000233 RID: 563
		public const byte Major = 1;

		// Token: 0x04000234 RID: 564
		public const byte Minor = 1;

		// Token: 0x04000235 RID: 565
		public const byte Release = 0;

		// Token: 0x04000236 RID: 566
		private static string StageSuffix = "_dev001";

		// Token: 0x04000237 RID: 567
		[SerializeField]
		private int m_major;

		// Token: 0x04000238 RID: 568
		[SerializeField]
		private int m_minor;

		// Token: 0x04000239 RID: 569
		[SerializeField]
		private int m_release;
	}
}
