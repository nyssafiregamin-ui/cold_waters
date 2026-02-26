using System;
using UnityEngine;

namespace AmplifyColor
{
	// Token: 0x0200002A RID: 42
	[Serializable]
	public class VersionInfo
	{
		// Token: 0x06000153 RID: 339 RVA: 0x00008C58 File Offset: 0x00006E58
		private VersionInfo()
		{
			this.m_major = 1;
			this.m_minor = 5;
			this.m_release = 1;
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00008C78 File Offset: 0x00006E78
		private VersionInfo(byte major, byte minor, byte release)
		{
			this.m_major = (int)major;
			this.m_minor = (int)minor;
			this.m_release = (int)release;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00008CB0 File Offset: 0x00006EB0
		public static string StaticToString()
		{
			return string.Format("{0}.{1}.{2}", 1, 5, 1) + VersionInfo.StageSuffix + VersionInfo.TrialSuffix;
		}

		// Token: 0x06000157 RID: 343 RVA: 0x00008CE0 File Offset: 0x00006EE0
		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}", this.m_major, this.m_minor, this.m_release) + VersionInfo.StageSuffix + VersionInfo.TrialSuffix;
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x06000158 RID: 344 RVA: 0x00008D28 File Offset: 0x00006F28
		public int Number
		{
			get
			{
				return this.m_major * 100 + this.m_minor * 10 + this.m_release;
			}
		}

		// Token: 0x06000159 RID: 345 RVA: 0x00008D44 File Offset: 0x00006F44
		public static VersionInfo Current()
		{
			return new VersionInfo(1, 5, 1);
		}

		// Token: 0x0600015A RID: 346 RVA: 0x00008D50 File Offset: 0x00006F50
		public static bool Matches(VersionInfo version)
		{
			return version.m_major == 1 && version.m_minor == 5 && 1 == version.m_release;
		}

		// Token: 0x0400018A RID: 394
		public const byte Major = 1;

		// Token: 0x0400018B RID: 395
		public const byte Minor = 5;

		// Token: 0x0400018C RID: 396
		public const byte Release = 1;

		// Token: 0x0400018D RID: 397
		private static string StageSuffix = "_dev007";

		// Token: 0x0400018E RID: 398
		private static string TrialSuffix = string.Empty;

		// Token: 0x0400018F RID: 399
		[SerializeField]
		private int m_major;

		// Token: 0x04000190 RID: 400
		[SerializeField]
		private int m_minor;

		// Token: 0x04000191 RID: 401
		[SerializeField]
		private int m_release;
	}
}
