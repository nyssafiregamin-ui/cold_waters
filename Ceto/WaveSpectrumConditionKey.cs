using System;

namespace Ceto
{
	// Token: 0x020000AC RID: 172
	public abstract class WaveSpectrumConditionKey : IEquatable<WaveSpectrumConditionKey>
	{
		// Token: 0x060004C6 RID: 1222 RVA: 0x0001DAF8 File Offset: 0x0001BCF8
		public WaveSpectrumConditionKey(int size, float windDir, SPECTRUM_TYPE spectrumType, int numGrids)
		{
			this.Size = size;
			this.NumGrids = numGrids;
			this.WindDir = windDir;
			this.SpectrumType = spectrumType;
		}

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x060004C7 RID: 1223 RVA: 0x0001DB28 File Offset: 0x0001BD28
		// (set) Token: 0x060004C8 RID: 1224 RVA: 0x0001DB30 File Offset: 0x0001BD30
		public int Size { get; private set; }

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x060004C9 RID: 1225 RVA: 0x0001DB3C File Offset: 0x0001BD3C
		// (set) Token: 0x060004CA RID: 1226 RVA: 0x0001DB44 File Offset: 0x0001BD44
		public int NumGrids { get; private set; }

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x060004CB RID: 1227 RVA: 0x0001DB50 File Offset: 0x0001BD50
		// (set) Token: 0x060004CC RID: 1228 RVA: 0x0001DB58 File Offset: 0x0001BD58
		public float WindDir { get; private set; }

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x060004CD RID: 1229 RVA: 0x0001DB64 File Offset: 0x0001BD64
		// (set) Token: 0x060004CE RID: 1230 RVA: 0x0001DB6C File Offset: 0x0001BD6C
		public SPECTRUM_TYPE SpectrumType { get; private set; }

		// Token: 0x060004CF RID: 1231
		protected abstract bool Matches(WaveSpectrumConditionKey k);

		// Token: 0x060004D0 RID: 1232
		protected abstract int AddToHashCode(int hashcode);

		// Token: 0x060004D1 RID: 1233 RVA: 0x0001DB78 File Offset: 0x0001BD78
		public override bool Equals(object o)
		{
			WaveSpectrumConditionKey k = o as WaveSpectrumConditionKey;
			return !(k == null) && k == this;
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x0001DBA4 File Offset: 0x0001BDA4
		public bool Equals(WaveSpectrumConditionKey k)
		{
			return k == this;
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x0001DBB0 File Offset: 0x0001BDB0
		public override int GetHashCode()
		{
			int num = 23;
			num = num * 37 + this.Size.GetHashCode();
			num = num * 37 + this.NumGrids.GetHashCode();
			num = num * 37 + this.WindDir.GetHashCode();
			num = num * 37 + this.SpectrumType.GetHashCode();
			return this.AddToHashCode(num);
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x0001DC1C File Offset: 0x0001BE1C
		public static bool operator ==(WaveSpectrumConditionKey k1, WaveSpectrumConditionKey k2)
		{
			return object.ReferenceEquals(k1, k2) || (k1 != null && k2 != null && k1.Size == k2.Size && k1.NumGrids == k2.NumGrids && k1.WindDir == k2.WindDir && k1.SpectrumType == k2.SpectrumType && k1.Matches(k2));
		}

		// Token: 0x060004D5 RID: 1237 RVA: 0x0001DC98 File Offset: 0x0001BE98
		public static bool operator !=(WaveSpectrumConditionKey k1, WaveSpectrumConditionKey k2)
		{
			return !(k1 == k2);
		}
	}
}
