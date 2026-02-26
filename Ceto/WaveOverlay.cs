using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x02000090 RID: 144
	public class WaveOverlay
	{
		// Token: 0x060003BF RID: 959 RVA: 0x0001640C File Offset: 0x0001460C
		public WaveOverlay(Vector3 pos, float rotation, Vector2 halfSize, float duration)
		{
			this.Position = pos;
			this.HalfSize = halfSize;
			this.Rotation = rotation;
			this.Creation = this.OceanTime();
			this.Duration = Mathf.Max(duration, 0.001f);
			this.Corners = new Vector4[4];
			this.HeightTex = new OverlayHeightTexture();
			this.NormalTex = new OverlayNormalTexture();
			this.FoamTex = new OverlayFoamTexture();
			this.ClipTex = new OverlayClipTexture();
			this.CalculateLocalToWorld();
			this.CalculateBounds();
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x00016498 File Offset: 0x00014698
		public WaveOverlay()
		{
			this.HalfSize = Vector2.one;
			this.Rotation = 0f;
			this.Creation = this.OceanTime();
			this.Duration = 0.001f;
			this.Corners = new Vector4[4];
			this.HeightTex = new OverlayHeightTexture();
			this.NormalTex = new OverlayNormalTexture();
			this.FoamTex = new OverlayFoamTexture();
			this.ClipTex = new OverlayClipTexture();
			this.CalculateLocalToWorld();
			this.CalculateBounds();
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x060003C2 RID: 962 RVA: 0x000165C8 File Offset: 0x000147C8
		// (set) Token: 0x060003C3 RID: 963 RVA: 0x000165D0 File Offset: 0x000147D0
		public bool Kill { get; set; }

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x060003C4 RID: 964 RVA: 0x000165DC File Offset: 0x000147DC
		// (set) Token: 0x060003C5 RID: 965 RVA: 0x000165E4 File Offset: 0x000147E4
		public bool Hide { get; set; }

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x060003C6 RID: 966 RVA: 0x000165F0 File Offset: 0x000147F0
		// (set) Token: 0x060003C7 RID: 967 RVA: 0x000165F8 File Offset: 0x000147F8
		public Vector3 Position { get; set; }

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x060003C8 RID: 968 RVA: 0x00016604 File Offset: 0x00014804
		// (set) Token: 0x060003C9 RID: 969 RVA: 0x0001660C File Offset: 0x0001480C
		public Vector2 HalfSize { get; set; }

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x060003CA RID: 970 RVA: 0x00016618 File Offset: 0x00014818
		// (set) Token: 0x060003CB RID: 971 RVA: 0x00016620 File Offset: 0x00014820
		public float Rotation { get; set; }

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x060003CC RID: 972 RVA: 0x0001662C File Offset: 0x0001482C
		// (set) Token: 0x060003CD RID: 973 RVA: 0x00016634 File Offset: 0x00014834
		public float Creation { get; protected set; }

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x060003CE RID: 974 RVA: 0x00016640 File Offset: 0x00014840
		public float Age
		{
			get
			{
				return this.OceanTime() - this.Creation;
			}
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x060003CF RID: 975 RVA: 0x00016650 File Offset: 0x00014850
		// (set) Token: 0x060003D0 RID: 976 RVA: 0x00016658 File Offset: 0x00014858
		public float Duration { get; protected set; }

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x060003D1 RID: 977 RVA: 0x00016664 File Offset: 0x00014864
		public float NormalizedAge
		{
			get
			{
				return Mathf.Clamp01(this.Age / this.Duration);
			}
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x060003D2 RID: 978 RVA: 0x00016678 File Offset: 0x00014878
		// (set) Token: 0x060003D3 RID: 979 RVA: 0x00016680 File Offset: 0x00014880
		public Vector4[] Corners { get; private set; }

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x060003D4 RID: 980 RVA: 0x0001668C File Offset: 0x0001488C
		// (set) Token: 0x060003D5 RID: 981 RVA: 0x00016694 File Offset: 0x00014894
		public OverlayHeightTexture HeightTex { get; set; }

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x060003D6 RID: 982 RVA: 0x000166A0 File Offset: 0x000148A0
		// (set) Token: 0x060003D7 RID: 983 RVA: 0x000166A8 File Offset: 0x000148A8
		public OverlayNormalTexture NormalTex { get; set; }

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x060003D8 RID: 984 RVA: 0x000166B4 File Offset: 0x000148B4
		// (set) Token: 0x060003D9 RID: 985 RVA: 0x000166BC File Offset: 0x000148BC
		public OverlayFoamTexture FoamTex { get; set; }

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x060003DA RID: 986 RVA: 0x000166C8 File Offset: 0x000148C8
		// (set) Token: 0x060003DB RID: 987 RVA: 0x000166D0 File Offset: 0x000148D0
		public OverlayClipTexture ClipTex { get; set; }

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x060003DC RID: 988 RVA: 0x000166DC File Offset: 0x000148DC
		// (set) Token: 0x060003DD RID: 989 RVA: 0x000166E4 File Offset: 0x000148E4
		public Matrix4x4 LocalToWorld { get; protected set; }

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x060003DE RID: 990 RVA: 0x000166F0 File Offset: 0x000148F0
		// (set) Token: 0x060003DF RID: 991 RVA: 0x000166F8 File Offset: 0x000148F8
		public Bounds BoundingBox { get; protected set; }

		// Token: 0x060003E0 RID: 992 RVA: 0x00016704 File Offset: 0x00014904
		private float OceanTime()
		{
			if (Ocean.Instance == null)
			{
				return 0f;
			}
			return Ocean.Instance.OceanTime.Now;
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x00016738 File Offset: 0x00014938
		public virtual void UpdateOverlay()
		{
			this.CalculateLocalToWorld();
			this.CalculateBounds();
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x00016748 File Offset: 0x00014948
		public virtual void CalculateLocalToWorld()
		{
			Vector3 s = new Vector3(this.HalfSize.x, 1f, this.HalfSize.y);
			this.LocalToWorld = Matrix4x4.TRS(new Vector3(this.Position.x, 0f, this.Position.z), Quaternion.Euler(0f, this.Rotation, 0f), s);
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x000167C8 File Offset: 0x000149C8
		public virtual void CalculateBounds()
		{
			Vector3 vector = new Vector3(this.HalfSize.x, 1f, this.HalfSize.y);
			float y = 0f;
			if (Ocean.Instance != null)
			{
				y = Ocean.Instance.level;
				vector.y = Ocean.MAX_WAVE_HEIGHT;
			}
			float num = float.PositiveInfinity;
			float num2 = float.PositiveInfinity;
			float num3 = float.NegativeInfinity;
			float num4 = float.NegativeInfinity;
			for (int i = 0; i < 4; i++)
			{
				this.Corners[i] = this.LocalToWorld * WaveOverlay.CORNERS[i];
				this.Corners[i].y = y;
				if (this.Corners[i].x < num)
				{
					num = this.Corners[i].x;
				}
				if (this.Corners[i].z < num2)
				{
					num2 = this.Corners[i].z;
				}
				if (this.Corners[i].x > num3)
				{
					num3 = this.Corners[i].x;
				}
				if (this.Corners[i].z > num4)
				{
					num4 = this.Corners[i].z;
				}
			}
			Vector3 center = new Vector3(this.Position.x, y, this.Position.z);
			Vector3 size = new Vector3(num3 - num, vector.y, num4 - num2);
			this.BoundingBox = new Bounds(center, size);
		}

		// Token: 0x060003E4 RID: 996 RVA: 0x00016998 File Offset: 0x00014B98
		public bool Contains(float x, float z)
		{
			float num;
			float num2;
			return this.Contains(x, z, out num, out num2);
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x000169B4 File Offset: 0x00014BB4
		public bool Contains(float x, float z, out float u, out float v)
		{
			u = 0f;
			v = 0f;
			float num = x - this.Position.x;
			float num2 = z - this.Position.z;
			float rotation = this.Rotation;
			float num3 = Mathf.Cos(rotation * 3.1415927f / 180f);
			float num4 = Mathf.Sin(rotation * 3.1415927f / 180f);
			u = num * num3 - num2 * num4;
			v = num * num4 + num2 * num3;
			if (u > -this.HalfSize.x && u < this.HalfSize.x && v > -this.HalfSize.y && v < this.HalfSize.y)
			{
				u /= this.HalfSize.x;
				v /= this.HalfSize.y;
				u = u * 0.5f + 0.5f;
				v = v * 0.5f + 0.5f;
				return true;
			}
			return false;
		}

		// Token: 0x040003F5 RID: 1013
		private static readonly Vector4[] CORNERS = new Vector4[]
		{
			new Vector4(-1f, 0f, -1f, 1f),
			new Vector4(1f, 0f, -1f, 1f),
			new Vector4(1f, 0f, 1f, 1f),
			new Vector4(-1f, 0f, 1f, 1f)
		};
	}
}
