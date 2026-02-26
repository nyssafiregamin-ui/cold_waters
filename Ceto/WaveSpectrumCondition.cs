using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x020000AB RID: 171
	public abstract class WaveSpectrumCondition
	{
		// Token: 0x060004A5 RID: 1189 RVA: 0x0001D670 File Offset: 0x0001B870
		public WaveSpectrumCondition(int size, int numGrids)
		{
			this.GridSizes = Vector4.one;
			this.Choppyness = Vector4.one;
			this.WaveAmps = Vector4.one;
			this.SpectrumData01 = new Color[size * size];
			if (numGrids > 2)
			{
				this.SpectrumData23 = new Color[size * size];
			}
			this.WTableData = new Color[size * size];
			this.LastUpdated = -1;
			this.SupportsJacobians = true;
		}

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x060004A6 RID: 1190 RVA: 0x0001D6E4 File Offset: 0x0001B8E4
		// (set) Token: 0x060004A7 RID: 1191 RVA: 0x0001D6EC File Offset: 0x0001B8EC
		public bool Done { get; set; }

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x060004A8 RID: 1192 RVA: 0x0001D6F8 File Offset: 0x0001B8F8
		// (set) Token: 0x060004A9 RID: 1193 RVA: 0x0001D700 File Offset: 0x0001B900
		public Vector4 GridSizes { get; protected set; }

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x060004AA RID: 1194 RVA: 0x0001D70C File Offset: 0x0001B90C
		// (set) Token: 0x060004AB RID: 1195 RVA: 0x0001D714 File Offset: 0x0001B914
		public Vector4 Choppyness { get; protected set; }

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x060004AC RID: 1196 RVA: 0x0001D720 File Offset: 0x0001B920
		// (set) Token: 0x060004AD RID: 1197 RVA: 0x0001D728 File Offset: 0x0001B928
		public Vector4 WaveAmps { get; protected set; }

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x060004AE RID: 1198 RVA: 0x0001D734 File Offset: 0x0001B934
		// (set) Token: 0x060004AF RID: 1199 RVA: 0x0001D73C File Offset: 0x0001B93C
		public WaveSpectrumConditionKey Key { get; protected set; }

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x060004B0 RID: 1200 RVA: 0x0001D748 File Offset: 0x0001B948
		// (set) Token: 0x060004B1 RID: 1201 RVA: 0x0001D750 File Offset: 0x0001B950
		public Texture2D Spectrum01 { get; private set; }

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x060004B2 RID: 1202 RVA: 0x0001D75C File Offset: 0x0001B95C
		// (set) Token: 0x060004B3 RID: 1203 RVA: 0x0001D764 File Offset: 0x0001B964
		public Color[] SpectrumData01 { get; private set; }

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x060004B4 RID: 1204 RVA: 0x0001D770 File Offset: 0x0001B970
		// (set) Token: 0x060004B5 RID: 1205 RVA: 0x0001D778 File Offset: 0x0001B978
		public Texture2D Spectrum23 { get; private set; }

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x060004B6 RID: 1206 RVA: 0x0001D784 File Offset: 0x0001B984
		// (set) Token: 0x060004B7 RID: 1207 RVA: 0x0001D78C File Offset: 0x0001B98C
		public Color[] SpectrumData23 { get; private set; }

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x060004B8 RID: 1208 RVA: 0x0001D798 File Offset: 0x0001B998
		// (set) Token: 0x060004B9 RID: 1209 RVA: 0x0001D7A0 File Offset: 0x0001B9A0
		public Texture2D WTable { get; private set; }

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x060004BA RID: 1210 RVA: 0x0001D7AC File Offset: 0x0001B9AC
		// (set) Token: 0x060004BB RID: 1211 RVA: 0x0001D7B4 File Offset: 0x0001B9B4
		public Color[] WTableData { get; private set; }

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x060004BC RID: 1212 RVA: 0x0001D7C0 File Offset: 0x0001B9C0
		// (set) Token: 0x060004BD RID: 1213 RVA: 0x0001D7C8 File Offset: 0x0001B9C8
		public int LastUpdated { get; set; }

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x060004BE RID: 1214 RVA: 0x0001D7D4 File Offset: 0x0001B9D4
		// (set) Token: 0x060004BF RID: 1215 RVA: 0x0001D7DC File Offset: 0x0001B9DC
		public bool SupportsJacobians { get; protected set; }

		// Token: 0x060004C0 RID: 1216
		public abstract SpectrumTask GetCreateSpectrumConditionTask();

		// Token: 0x060004C1 RID: 1217 RVA: 0x0001D7E8 File Offset: 0x0001B9E8
		public InitSpectrumDisplacementsTask GetInitSpectrumDisplacementsTask(DisplacementBufferCPU buffer, float time)
		{
			return new InitSpectrumDisplacementsTask(buffer, this, time);
		}

		// Token: 0x060004C2 RID: 1218 RVA: 0x0001D7F4 File Offset: 0x0001B9F4
		public Vector4 InverseGridSizes()
		{
			float num = 6.2831855f * (float)this.Key.Size;
			return new Vector4(num / this.GridSizes.x, num / this.GridSizes.y, num / this.GridSizes.z, num / this.GridSizes.w);
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x0001D85C File Offset: 0x0001BA5C
		public void Release()
		{
			if (this.Spectrum01 != null)
			{
				UnityEngine.Object.Destroy(this.Spectrum01);
				this.Spectrum01 = null;
			}
			if (this.Spectrum23 != null)
			{
				UnityEngine.Object.Destroy(this.Spectrum23);
				this.Spectrum23 = null;
			}
			if (this.WTable != null)
			{
				UnityEngine.Object.Destroy(this.WTable);
				this.WTable = null;
			}
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x0001D8D4 File Offset: 0x0001BAD4
		public void CreateTextures()
		{
			if (this.Spectrum01 == null)
			{
				this.Spectrum01 = new Texture2D(this.Key.Size, this.Key.Size, TextureFormat.RGBAFloat, false, true);
				this.Spectrum01.filterMode = FilterMode.Point;
				this.Spectrum01.wrapMode = TextureWrapMode.Repeat;
				this.Spectrum01.hideFlags = HideFlags.HideAndDontSave;
				this.Spectrum01.name = "Ceto Spectrum01 Condition";
			}
			if (this.Spectrum23 == null && this.Key.NumGrids > 2)
			{
				this.Spectrum23 = new Texture2D(this.Key.Size, this.Key.Size, TextureFormat.RGBAFloat, false, true);
				this.Spectrum23.filterMode = FilterMode.Point;
				this.Spectrum23.wrapMode = TextureWrapMode.Repeat;
				this.Spectrum23.hideFlags = HideFlags.HideAndDontSave;
				this.Spectrum23.name = "Ceto Spectrum23 Condition";
			}
			if (this.WTable == null)
			{
				this.WTable = new Texture2D(this.Key.Size, this.Key.Size, TextureFormat.RGBAFloat, false, true);
				this.WTable.filterMode = FilterMode.Point;
				this.WTable.wrapMode = TextureWrapMode.Clamp;
				this.WTable.hideFlags = HideFlags.HideAndDontSave;
				this.WTable.name = "Ceto Wave Spectrum GPU WTable";
			}
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x0001DA34 File Offset: 0x0001BC34
		public void Apply(Color[] spectrum01, Color[] spectrum23, Color[] wtable)
		{
			if (this.Spectrum01 != null && spectrum01 != null)
			{
				this.Spectrum01.SetPixels(spectrum01);
				this.Spectrum01.Apply();
				Array.Copy(spectrum01, this.SpectrumData01, spectrum01.Length);
			}
			if (this.Spectrum23 != null && spectrum23 != null)
			{
				this.Spectrum23.SetPixels(spectrum23);
				this.Spectrum23.Apply();
				Array.Copy(spectrum23, this.SpectrumData23, spectrum23.Length);
			}
			if (this.WTable != null && wtable != null)
			{
				this.WTable.SetPixels(wtable);
				this.WTable.Apply();
				Array.Copy(wtable, this.WTableData, wtable.Length);
			}
		}
	}
}
