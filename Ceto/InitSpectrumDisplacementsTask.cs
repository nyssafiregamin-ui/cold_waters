using System;
using System.Collections;
using Ceto.Common.Threading.Tasks;
using UnityEngine;

namespace Ceto
{
	// Token: 0x020000B6 RID: 182
	public class InitSpectrumDisplacementsTask : ThreadedTask
	{
		// Token: 0x0600050A RID: 1290 RVA: 0x0001F820 File Offset: 0x0001DA20
		public InitSpectrumDisplacementsTask(DisplacementBufferCPU buffer, WaveSpectrumCondition condition, float time) : base(true)
		{
			this.Buffer = buffer;
			this.NumGrids = condition.Key.NumGrids;
			this.Size = condition.Key.Size;
			this.SpectrumType = condition.Key.SpectrumType;
			this.TimeValue = time;
			this.Reset(condition, time);
			this.CreateKTables(condition.InverseGridSizes());
		}

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x0600050B RID: 1291 RVA: 0x0001F88C File Offset: 0x0001DA8C
		// (set) Token: 0x0600050C RID: 1292 RVA: 0x0001F894 File Offset: 0x0001DA94
		public int NumGrids { get; private set; }

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x0600050D RID: 1293 RVA: 0x0001F8A0 File Offset: 0x0001DAA0
		// (set) Token: 0x0600050E RID: 1294 RVA: 0x0001F8A8 File Offset: 0x0001DAA8
		public int Size { get; private set; }

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x0600050F RID: 1295 RVA: 0x0001F8B4 File Offset: 0x0001DAB4
		// (set) Token: 0x06000510 RID: 1296 RVA: 0x0001F8BC File Offset: 0x0001DABC
		public int LastUpdated { get; protected set; }

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000511 RID: 1297 RVA: 0x0001F8C8 File Offset: 0x0001DAC8
		// (set) Token: 0x06000512 RID: 1298 RVA: 0x0001F8D0 File Offset: 0x0001DAD0
		public float TimeValue { get; protected set; }

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06000513 RID: 1299 RVA: 0x0001F8DC File Offset: 0x0001DADC
		// (set) Token: 0x06000514 RID: 1300 RVA: 0x0001F8E4 File Offset: 0x0001DAE4
		public SPECTRUM_TYPE SpectrumType { get; private set; }

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06000515 RID: 1301 RVA: 0x0001F8F0 File Offset: 0x0001DAF0
		// (set) Token: 0x06000516 RID: 1302 RVA: 0x0001F8F8 File Offset: 0x0001DAF8
		private protected DisplacementBufferCPU Buffer { protected get; private set; }

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x06000517 RID: 1303 RVA: 0x0001F904 File Offset: 0x0001DB04
		// (set) Token: 0x06000518 RID: 1304 RVA: 0x0001F90C File Offset: 0x0001DB0C
		protected Vector4[] Data0 { get; set; }

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000519 RID: 1305 RVA: 0x0001F918 File Offset: 0x0001DB18
		// (set) Token: 0x0600051A RID: 1306 RVA: 0x0001F920 File Offset: 0x0001DB20
		protected Vector4[] Data1 { get; set; }

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x0600051B RID: 1307 RVA: 0x0001F92C File Offset: 0x0001DB2C
		// (set) Token: 0x0600051C RID: 1308 RVA: 0x0001F934 File Offset: 0x0001DB34
		protected Vector4[] Data2 { get; set; }

		// Token: 0x0600051D RID: 1309 RVA: 0x0001F940 File Offset: 0x0001DB40
		public void Reset(WaveSpectrumCondition condition, float time)
		{
			if (condition.Key.SpectrumType != this.SpectrumType)
			{
				throw new InvalidOperationException("Trying to reset a Unified InitSpectrum task with wrong condition type = " + condition.Key.SpectrumType);
			}
			if (condition.Key.Size != this.Size)
			{
				throw new InvalidOperationException("Trying to reset a Unified InitSpectrum task with wrong condition size = " + condition.Key.Size);
			}
			base.Reset();
			int num = this.Size * this.Size;
			if (this.m_spectrum01 == null)
			{
				this.m_spectrum01 = new Color[num];
			}
			if (this.m_spectrum23 == null && this.NumGrids > 2)
			{
				this.m_spectrum23 = new Color[num];
			}
			if (this.m_wtable == null)
			{
				this.m_wtable = new Color[num];
			}
			this.TimeValue = time;
			this.Data0 = this.Buffer.GetReadBuffer(0);
			this.Data1 = this.Buffer.GetReadBuffer(1);
			this.Data2 = this.Buffer.GetReadBuffer(2);
			WaveSpectrumBufferCPU.Buffer buffer = this.Buffer.GetBuffer(0);
			WaveSpectrumBufferCPU.Buffer buffer2 = this.Buffer.GetBuffer(1);
			WaveSpectrumBufferCPU.Buffer buffer3 = this.Buffer.GetBuffer(2);
			if (buffer != null)
			{
				if (this.NumGrids > 2)
				{
					buffer.doublePacked = true;
				}
				else
				{
					buffer.doublePacked = false;
				}
			}
			if (buffer2 != null)
			{
				if (this.NumGrids > 1)
				{
					buffer2.doublePacked = true;
				}
				else
				{
					buffer2.doublePacked = false;
				}
			}
			if (buffer3 != null)
			{
				if (this.NumGrids > 3)
				{
					buffer3.doublePacked = true;
				}
				else
				{
					buffer3.doublePacked = false;
				}
			}
			if (this.LastUpdated != condition.LastUpdated)
			{
				this.LastUpdated = condition.LastUpdated;
				if (this.m_spectrum01 != null && condition.SpectrumData01 != null)
				{
					Array.Copy(condition.SpectrumData01, this.m_spectrum01, num);
				}
				if (this.m_spectrum23 != null && condition.SpectrumData23 != null)
				{
					Array.Copy(condition.SpectrumData23, this.m_spectrum23, num);
				}
				if (this.m_wtable != null && condition.WTableData != null)
				{
					Array.Copy(condition.WTableData, this.m_wtable, num);
				}
			}
		}

		// Token: 0x0600051E RID: 1310 RVA: 0x0001FB88 File Offset: 0x0001DD88
		public override IEnumerator Run()
		{
			if (this.NumGrids == 1)
			{
				this.InitilizeGrids1();
			}
			else if (this.NumGrids == 2)
			{
				this.InitilizeGrids2();
			}
			else if (this.NumGrids == 3)
			{
				this.InitilizeGrids3();
			}
			else if (this.NumGrids == 4)
			{
				this.InitilizeGrids4();
			}
			this.FinishedRunning();
			return null;
		}

		// Token: 0x0600051F RID: 1311 RVA: 0x0001FBF4 File Offset: 0x0001DDF4
		private void InitilizeGrids1()
		{
			int size = this.Size;
			float num = 1f / (float)size;
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					if (this.Cancelled)
					{
						return;
					}
					Vector2 vector;
					vector.x = (float)j * num;
					vector.y = (float)i * num;
					Vector2 vector2;
					vector2.x = ((vector.x <= 0.5f) ? vector.x : (vector.x - 1f));
					vector2.y = ((vector.y <= 0.5f) ? vector.y : (vector.y - 1f));
					int num2 = j + i * size;
					int num3 = (size - j) % size + (size - i) % size * size;
					Color color = this.m_spectrum01[num2];
					Color color2 = this.m_spectrum01[num3];
					float f = this.m_wtable[num2].r * this.TimeValue;
					float num4 = Mathf.Cos(f);
					float num5 = Mathf.Sin(f);
					Vector2 vector3;
					vector3.x = (color.r + color2.r) * num4 - (color.g + color2.g) * num5;
					vector3.y = (color.r - color2.r) * num5 + (color.g - color2.g) * num4;
					if (this.Data0 != null)
					{
						this.Data0[num2].x = vector3.x;
						this.Data0[num2].y = vector3.y;
						this.Data0[num2].z = 0f;
						this.Data0[num2].w = 0f;
					}
					if (this.Data1 != null)
					{
						Vector3 vector4 = this.m_ktable1[num2];
						Vector2 vector5;
						vector5.x = -(vector4.x * vector3.y) - vector4.y * vector3.x;
						vector5.y = vector4.x * vector3.x - vector4.y * vector3.y;
						this.Data1[num2].x = vector5.x * vector4.z;
						this.Data1[num2].y = vector5.y * vector4.z;
						this.Data1[num2].z = 0f;
						this.Data1[num2].w = 0f;
					}
				}
			}
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x0001FEE0 File Offset: 0x0001E0E0
		private void InitilizeGrids2()
		{
			int size = this.Size;
			float num = 1f / (float)size;
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					if (this.Cancelled)
					{
						return;
					}
					Vector2 vector;
					vector.x = (float)j * num;
					vector.y = (float)i * num;
					Vector2 vector2;
					vector2.x = ((vector.x <= 0.5f) ? vector.x : (vector.x - 1f));
					vector2.y = ((vector.y <= 0.5f) ? vector.y : (vector.y - 1f));
					int num2 = j + i * size;
					int num3 = (size - j) % size + (size - i) % size * size;
					Color color = this.m_spectrum01[num2];
					Color color2 = this.m_spectrum01[num3];
					Color color3 = this.m_wtable[num2];
					color3.r *= this.TimeValue;
					color3.g *= this.TimeValue;
					float num4 = Mathf.Cos(color3.r);
					float num5 = Mathf.Sin(color3.r);
					Vector2 vector3;
					vector3.x = (color.r + color2.r) * num4 - (color.g + color2.g) * num5;
					vector3.y = (color.r - color2.r) * num5 + (color.g - color2.g) * num4;
					num4 = Mathf.Cos(color3.g);
					num5 = Mathf.Sin(color3.g);
					Vector2 vector4;
					vector4.x = (color.b + color2.b) * num4 - (color.a + color2.a) * num5;
					vector4.y = (color.b - color2.b) * num5 + (color.a - color2.a) * num4;
					if (this.Data0 != null)
					{
						this.Data0[num2].x = vector3.x + -vector4.y;
						this.Data0[num2].y = vector3.y + vector4.x;
						this.Data0[num2].z = 0f;
						this.Data0[num2].w = 0f;
					}
					if (this.Data1 != null)
					{
						Vector3 vector5 = this.m_ktable1[num2];
						Vector3 vector6 = this.m_ktable2[num2];
						Vector2 vector7;
						vector7.x = -(vector5.x * vector3.y) - vector5.y * vector3.x;
						vector7.y = vector5.x * vector3.x - vector5.y * vector3.y;
						Vector2 vector8;
						vector8.x = -(vector6.x * vector4.y) - vector6.y * vector4.x;
						vector8.y = vector6.x * vector4.x - vector6.y * vector4.y;
						this.Data1[num2].x = vector7.x * vector5.z;
						this.Data1[num2].y = vector7.y * vector5.z;
						this.Data1[num2].z = vector8.x * vector6.z;
						this.Data1[num2].w = vector8.y * vector6.z;
					}
				}
			}
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x000202F0 File Offset: 0x0001E4F0
		private void InitilizeGrids3()
		{
			int size = this.Size;
			float num = 1f / (float)size;
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					if (this.Cancelled)
					{
						return;
					}
					Vector2 vector;
					vector.x = (float)j * num;
					vector.y = (float)i * num;
					Vector2 vector2;
					vector2.x = ((vector.x <= 0.5f) ? vector.x : (vector.x - 1f));
					vector2.y = ((vector.y <= 0.5f) ? vector.y : (vector.y - 1f));
					int num2 = j + i * size;
					int num3 = (size - j) % size + (size - i) % size * size;
					Color color = this.m_spectrum01[num2];
					Color color2 = this.m_spectrum23[num2];
					Color color3 = this.m_spectrum01[num3];
					Color color4 = this.m_spectrum23[num3];
					Color color5 = this.m_wtable[num2];
					color5.r *= this.TimeValue;
					color5.g *= this.TimeValue;
					color5.b *= this.TimeValue;
					color5.a *= this.TimeValue;
					float num4 = Mathf.Cos(color5.r);
					float num5 = Mathf.Sin(color5.r);
					Vector2 vector3;
					vector3.x = (color.r + color3.r) * num4 - (color.g + color3.g) * num5;
					vector3.y = (color.r - color3.r) * num5 + (color.g - color3.g) * num4;
					num4 = Mathf.Cos(color5.g);
					num5 = Mathf.Sin(color5.g);
					Vector2 vector4;
					vector4.x = (color.b + color3.b) * num4 - (color.a + color3.a) * num5;
					vector4.y = (color.b - color3.b) * num5 + (color.a - color3.a) * num4;
					num4 = Mathf.Cos(color5.b);
					num5 = Mathf.Sin(color5.b);
					Vector2 vector5;
					vector5.x = (color2.r + color4.r) * num4 - (color2.g + color4.g) * num5;
					vector5.y = (color2.r - color4.r) * num5 + (color2.g - color4.g) * num4;
					if (this.Data0 != null)
					{
						this.Data0[num2].x = vector3.x + -vector4.y;
						this.Data0[num2].y = vector3.y + vector4.x;
						this.Data0[num2].z = vector5.x;
						this.Data0[num2].w = vector5.y;
					}
					if (this.Data1 != null)
					{
						Vector3 vector6 = this.m_ktable1[num2];
						Vector3 vector7 = this.m_ktable2[num2];
						Vector2 vector8;
						vector8.x = -(vector6.x * vector3.y) - vector6.y * vector3.x;
						vector8.y = vector6.x * vector3.x - vector6.y * vector3.y;
						Vector2 vector9;
						vector9.x = -(vector7.x * vector4.y) - vector7.y * vector4.x;
						vector9.y = vector7.x * vector4.x - vector7.y * vector4.y;
						this.Data1[num2].x = vector8.x * vector6.z;
						this.Data1[num2].y = vector8.y * vector6.z;
						this.Data1[num2].z = vector9.x * vector7.z;
						this.Data1[num2].w = vector9.y * vector7.z;
					}
					if (this.Data2 != null)
					{
						Vector3 vector10 = this.m_ktable3[num2];
						Vector2 vector11;
						vector11.x = -(vector10.x * vector5.y) - vector10.y * vector5.x;
						vector11.y = vector10.x * vector5.x - vector10.y * vector5.y;
						this.Data2[num2].x = vector11.x * vector10.z;
						this.Data2[num2].y = vector11.y * vector10.z;
						this.Data2[num2].z = 0f;
						this.Data2[num2].w = 0f;
					}
				}
			}
		}

		// Token: 0x06000522 RID: 1314 RVA: 0x000208A4 File Offset: 0x0001EAA4
		private void InitilizeGrids4()
		{
			int size = this.Size;
			float num = 1f / (float)size;
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					if (this.Cancelled)
					{
						return;
					}
					Vector2 vector;
					vector.x = (float)j * num;
					vector.y = (float)i * num;
					Vector2 vector2;
					vector2.x = ((vector.x <= 0.5f) ? vector.x : (vector.x - 1f));
					vector2.y = ((vector.y <= 0.5f) ? vector.y : (vector.y - 1f));
					int num2 = j + i * size;
					int num3 = (size - j) % size + (size - i) % size * size;
					Color color = this.m_spectrum01[num2];
					Color color2 = this.m_spectrum23[num2];
					Color color3 = this.m_spectrum01[num3];
					Color color4 = this.m_spectrum23[num3];
					Color color5 = this.m_wtable[num2];
					color5.r *= this.TimeValue;
					color5.g *= this.TimeValue;
					color5.b *= this.TimeValue;
					color5.a *= this.TimeValue;
					float num4 = Mathf.Cos(color5.r);
					float num5 = Mathf.Sin(color5.r);
					Vector2 vector3;
					vector3.x = (color.r + color3.r) * num4 - (color.g + color3.g) * num5;
					vector3.y = (color.r - color3.r) * num5 + (color.g - color3.g) * num4;
					num4 = Mathf.Cos(color5.g);
					num5 = Mathf.Sin(color5.g);
					Vector2 vector4;
					vector4.x = (color.b + color3.b) * num4 - (color.a + color3.a) * num5;
					vector4.y = (color.b - color3.b) * num5 + (color.a - color3.a) * num4;
					num4 = Mathf.Cos(color5.b);
					num5 = Mathf.Sin(color5.b);
					Vector2 vector5;
					vector5.x = (color2.r + color4.r) * num4 - (color2.g + color4.g) * num5;
					vector5.y = (color2.r - color4.r) * num5 + (color2.g - color4.g) * num4;
					num4 = Mathf.Cos(color5.a);
					num5 = Mathf.Sin(color5.a);
					Vector2 vector6;
					vector6.x = (color2.b + color4.b) * num4 - (color2.a + color4.a) * num5;
					vector6.y = (color2.b - color4.b) * num5 + (color2.a - color4.a) * num4;
					if (this.Data0 != null)
					{
						this.Data0[num2].x = vector3.x + -vector4.y;
						this.Data0[num2].y = vector3.y + vector4.x;
						this.Data0[num2].z = vector5.x + -vector6.y;
						this.Data0[num2].w = vector5.y + vector6.x;
					}
					if (this.Data1 != null)
					{
						Vector3 vector7 = this.m_ktable1[num2];
						Vector3 vector8 = this.m_ktable2[num2];
						Vector2 vector9;
						vector9.x = -(vector7.x * vector3.y) - vector7.y * vector3.x;
						vector9.y = vector7.x * vector3.x - vector7.y * vector3.y;
						Vector2 vector10;
						vector10.x = -(vector8.x * vector4.y) - vector8.y * vector4.x;
						vector10.y = vector8.x * vector4.x - vector8.y * vector4.y;
						this.Data1[num2].x = vector9.x * vector7.z;
						this.Data1[num2].y = vector9.y * vector7.z;
						this.Data1[num2].z = vector10.x * vector8.z;
						this.Data1[num2].w = vector10.y * vector8.z;
					}
					if (this.Data2 != null)
					{
						Vector3 vector11 = this.m_ktable3[num2];
						Vector3 vector12 = this.m_ktable4[num2];
						Vector2 vector13;
						vector13.x = -(vector11.x * vector5.y) - vector11.y * vector5.x;
						vector13.y = vector11.x * vector5.x - vector11.y * vector5.y;
						Vector2 vector14;
						vector14.x = -(vector12.x * vector6.y) - vector12.y * vector6.x;
						vector14.y = vector12.x * vector6.x - vector12.y * vector6.y;
						this.Data2[num2].x = vector13.x * vector11.z;
						this.Data2[num2].y = vector13.y * vector11.z;
						this.Data2[num2].z = vector14.x * vector12.z;
						this.Data2[num2].w = vector14.y * vector12.z;
					}
				}
			}
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x00020F50 File Offset: 0x0001F150
		private void CreateKTables(Vector4 inverseGridSizes)
		{
			int size = this.Size;
			float num = 1f / (float)size;
			int numGrids = this.NumGrids;
			if (numGrids > 0)
			{
				this.m_ktable1 = new Vector3[size * size];
			}
			if (numGrids > 1)
			{
				this.m_ktable2 = new Vector3[size * size];
			}
			if (numGrids > 2)
			{
				this.m_ktable3 = new Vector3[size * size];
			}
			if (numGrids > 3)
			{
				this.m_ktable4 = new Vector3[size * size];
			}
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					Vector2 vector;
					vector.x = (float)j * num;
					vector.y = (float)i * num;
					Vector2 vector2;
					vector2.x = ((vector.x <= 0.5f) ? vector.x : (vector.x - 1f));
					vector2.y = ((vector.y <= 0.5f) ? vector.y : (vector.y - 1f));
					int num2 = j + i * size;
					if (numGrids > 0)
					{
						Vector2 vector3;
						vector3.x = vector2.x * inverseGridSizes.x;
						vector3.y = vector2.y * inverseGridSizes.x;
						float num3 = Mathf.Sqrt(vector3.x * vector3.x + vector3.y * vector3.y);
						float z = (num3 != 0f) ? (1f / num3) : 0f;
						this.m_ktable1[num2].x = vector3.x;
						this.m_ktable1[num2].y = vector3.y;
						this.m_ktable1[num2].z = z;
					}
					if (numGrids > 1)
					{
						Vector2 vector4;
						vector4.x = vector2.x * inverseGridSizes.y;
						vector4.y = vector2.y * inverseGridSizes.y;
						float num4 = Mathf.Sqrt(vector4.x * vector4.x + vector4.y * vector4.y);
						float z2 = (num4 != 0f) ? (1f / num4) : 0f;
						this.m_ktable2[num2].x = vector4.x;
						this.m_ktable2[num2].y = vector4.y;
						this.m_ktable2[num2].z = z2;
					}
					if (numGrids > 2)
					{
						Vector2 vector5;
						vector5.x = vector2.x * inverseGridSizes.z;
						vector5.y = vector2.y * inverseGridSizes.z;
						float num5 = Mathf.Sqrt(vector5.x * vector5.x + vector5.y * vector5.y);
						float z3 = (num5 != 0f) ? (1f / num5) : 0f;
						this.m_ktable3[num2].x = vector5.x;
						this.m_ktable3[num2].y = vector5.y;
						this.m_ktable3[num2].z = z3;
					}
					if (numGrids > 3)
					{
						Vector2 vector6;
						vector6.x = vector2.x * inverseGridSizes.w;
						vector6.y = vector2.y * inverseGridSizes.w;
						float num6 = Mathf.Sqrt(vector6.x * vector6.x + vector6.y * vector6.y);
						float z4 = (num6 != 0f) ? (1f / num6) : 0f;
						this.m_ktable4[num2].x = vector6.x;
						this.m_ktable4[num2].y = vector6.y;
						this.m_ktable4[num2].z = z4;
					}
				}
			}
		}

		// Token: 0x040004E1 RID: 1249
		private Color[] m_spectrum01;

		// Token: 0x040004E2 RID: 1250
		private Color[] m_spectrum23;

		// Token: 0x040004E3 RID: 1251
		private Color[] m_wtable;

		// Token: 0x040004E4 RID: 1252
		private Vector3[] m_ktable1;

		// Token: 0x040004E5 RID: 1253
		private Vector3[] m_ktable2;

		// Token: 0x040004E6 RID: 1254
		private Vector3[] m_ktable3;

		// Token: 0x040004E7 RID: 1255
		private Vector3[] m_ktable4;
	}
}
