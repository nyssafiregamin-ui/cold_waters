using System;
using UnityEngine;

namespace AmplifyBloom
{
	// Token: 0x02000013 RID: 19
	[Serializable]
	public class AmplifyLensFlare : IAmplifyItem
	{
		// Token: 0x0600009D RID: 157 RVA: 0x0000503C File Offset: 0x0000323C
		public AmplifyLensFlare()
		{
			this.m_lensGradient = new Gradient();
			GradientColorKey[] colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(Color.white, 0f),
				new GradientColorKey(Color.blue, 0.25f),
				new GradientColorKey(Color.green, 0.5f),
				new GradientColorKey(Color.yellow, 0.75f),
				new GradientColorKey(Color.red, 1f)
			};
			GradientAlphaKey[] alphaKeys = new GradientAlphaKey[]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 0.25f),
				new GradientAlphaKey(1f, 0.5f),
				new GradientAlphaKey(1f, 0.75f),
				new GradientAlphaKey(1f, 1f)
			};
			this.m_lensGradient.SetKeys(colorKeys, alphaKeys);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00005220 File Offset: 0x00003420
		public void Destroy()
		{
			if (this.m_lensFlareGradTexture != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_lensFlareGradTexture);
				this.m_lensFlareGradTexture = null;
			}
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00005248 File Offset: 0x00003448
		public void CreateLUTexture()
		{
			this.m_lensFlareGradTexture = new Texture2D(256, 1, TextureFormat.ARGB32, false);
			this.m_lensFlareGradTexture.filterMode = FilterMode.Bilinear;
			this.TextureFromGradient();
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00005270 File Offset: 0x00003470
		public RenderTexture ApplyFlare(Material material, RenderTexture source)
		{
			RenderTexture tempRenderTarget = AmplifyUtils.GetTempRenderTarget(source.width, source.height);
			material.SetVector(AmplifyUtils.LensFlareGhostsParamsId, this.m_lensFlareGhostsParams);
			material.SetTexture(AmplifyUtils.LensFlareLUTId, this.m_lensFlareGradTexture);
			material.SetVector(AmplifyUtils.LensFlareHaloParamsId, this.m_lensFlareHaloParams);
			material.SetFloat(AmplifyUtils.LensFlareGhostChrDistortionId, this.m_lensFlareGhostChrDistortion);
			material.SetFloat(AmplifyUtils.LensFlareHaloChrDistortionId, this.m_lensFlareHaloChrDistortion);
			Graphics.Blit(source, tempRenderTarget, material, 3 + this.m_lensFlareGhostAmount);
			return tempRenderTarget;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000052F8 File Offset: 0x000034F8
		public void TextureFromGradient()
		{
			for (int i = 0; i < 256; i++)
			{
				this.m_lensFlareGradColor[i] = this.m_lensGradient.Evaluate((float)i / 255f);
			}
			this.m_lensFlareGradTexture.SetPixels(this.m_lensFlareGradColor);
			this.m_lensFlareGradTexture.Apply();
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x0000535C File Offset: 0x0000355C
		// (set) Token: 0x060000A3 RID: 163 RVA: 0x00005364 File Offset: 0x00003564
		public bool ApplyLensFlare
		{
			get
			{
				return this.m_applyLensFlare;
			}
			set
			{
				this.m_applyLensFlare = value;
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x00005370 File Offset: 0x00003570
		// (set) Token: 0x060000A5 RID: 165 RVA: 0x00005378 File Offset: 0x00003578
		public float OverallIntensity
		{
			get
			{
				return this.m_overallIntensity;
			}
			set
			{
				this.m_overallIntensity = ((value >= 0f) ? value : 0f);
				this.m_lensFlareGhostsParams.x = value * this.m_normalizedGhostIntensity;
				this.m_lensFlareHaloParams.x = value * this.m_normalizedHaloIntensity;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x000053C8 File Offset: 0x000035C8
		// (set) Token: 0x060000A7 RID: 167 RVA: 0x000053D0 File Offset: 0x000035D0
		public int LensFlareGhostAmount
		{
			get
			{
				return this.m_lensFlareGhostAmount;
			}
			set
			{
				this.m_lensFlareGhostAmount = value;
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x000053DC File Offset: 0x000035DC
		// (set) Token: 0x060000A9 RID: 169 RVA: 0x000053E4 File Offset: 0x000035E4
		public Vector4 LensFlareGhostsParams
		{
			get
			{
				return this.m_lensFlareGhostsParams;
			}
			set
			{
				this.m_lensFlareGhostsParams = value;
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000AA RID: 170 RVA: 0x000053F0 File Offset: 0x000035F0
		// (set) Token: 0x060000AB RID: 171 RVA: 0x000053F8 File Offset: 0x000035F8
		public float LensFlareNormalizedGhostsIntensity
		{
			get
			{
				return this.m_normalizedGhostIntensity;
			}
			set
			{
				this.m_normalizedGhostIntensity = ((value >= 0f) ? value : 0f);
				this.m_lensFlareGhostsParams.x = this.m_overallIntensity * this.m_normalizedGhostIntensity;
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000AC RID: 172 RVA: 0x0000543C File Offset: 0x0000363C
		// (set) Token: 0x060000AD RID: 173 RVA: 0x0000544C File Offset: 0x0000364C
		public float LensFlareGhostsIntensity
		{
			get
			{
				return this.m_lensFlareGhostsParams.x;
			}
			set
			{
				this.m_lensFlareGhostsParams.x = ((value >= 0f) ? value : 0f);
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000AE RID: 174 RVA: 0x00005470 File Offset: 0x00003670
		// (set) Token: 0x060000AF RID: 175 RVA: 0x00005480 File Offset: 0x00003680
		public float LensFlareGhostsDispersal
		{
			get
			{
				return this.m_lensFlareGhostsParams.y;
			}
			set
			{
				this.m_lensFlareGhostsParams.y = value;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x060000B0 RID: 176 RVA: 0x00005490 File Offset: 0x00003690
		// (set) Token: 0x060000B1 RID: 177 RVA: 0x000054A0 File Offset: 0x000036A0
		public float LensFlareGhostsPowerFactor
		{
			get
			{
				return this.m_lensFlareGhostsParams.z;
			}
			set
			{
				this.m_lensFlareGhostsParams.z = value;
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x000054B0 File Offset: 0x000036B0
		// (set) Token: 0x060000B3 RID: 179 RVA: 0x000054C0 File Offset: 0x000036C0
		public float LensFlareGhostsPowerFalloff
		{
			get
			{
				return this.m_lensFlareGhostsParams.w;
			}
			set
			{
				this.m_lensFlareGhostsParams.w = value;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x000054D0 File Offset: 0x000036D0
		// (set) Token: 0x060000B5 RID: 181 RVA: 0x000054D8 File Offset: 0x000036D8
		public Gradient LensFlareGradient
		{
			get
			{
				return this.m_lensGradient;
			}
			set
			{
				this.m_lensGradient = value;
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x000054E4 File Offset: 0x000036E4
		// (set) Token: 0x060000B7 RID: 183 RVA: 0x000054EC File Offset: 0x000036EC
		public Vector4 LensFlareHaloParams
		{
			get
			{
				return this.m_lensFlareHaloParams;
			}
			set
			{
				this.m_lensFlareHaloParams = value;
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x000054F8 File Offset: 0x000036F8
		// (set) Token: 0x060000B9 RID: 185 RVA: 0x00005500 File Offset: 0x00003700
		public float LensFlareNormalizedHaloIntensity
		{
			get
			{
				return this.m_normalizedHaloIntensity;
			}
			set
			{
				this.m_normalizedHaloIntensity = ((value >= 0f) ? value : 0f);
				this.m_lensFlareHaloParams.x = this.m_overallIntensity * this.m_normalizedHaloIntensity;
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x060000BA RID: 186 RVA: 0x00005544 File Offset: 0x00003744
		// (set) Token: 0x060000BB RID: 187 RVA: 0x00005554 File Offset: 0x00003754
		public float LensFlareHaloIntensity
		{
			get
			{
				return this.m_lensFlareHaloParams.x;
			}
			set
			{
				this.m_lensFlareHaloParams.x = ((value >= 0f) ? value : 0f);
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00005578 File Offset: 0x00003778
		// (set) Token: 0x060000BD RID: 189 RVA: 0x00005588 File Offset: 0x00003788
		public float LensFlareHaloWidth
		{
			get
			{
				return this.m_lensFlareHaloParams.y;
			}
			set
			{
				this.m_lensFlareHaloParams.y = value;
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x060000BE RID: 190 RVA: 0x00005598 File Offset: 0x00003798
		// (set) Token: 0x060000BF RID: 191 RVA: 0x000055A8 File Offset: 0x000037A8
		public float LensFlareHaloPowerFactor
		{
			get
			{
				return this.m_lensFlareHaloParams.z;
			}
			set
			{
				this.m_lensFlareHaloParams.z = value;
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x000055B8 File Offset: 0x000037B8
		// (set) Token: 0x060000C1 RID: 193 RVA: 0x000055C8 File Offset: 0x000037C8
		public float LensFlareHaloPowerFalloff
		{
			get
			{
				return this.m_lensFlareHaloParams.w;
			}
			set
			{
				this.m_lensFlareHaloParams.w = value;
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x000055D8 File Offset: 0x000037D8
		// (set) Token: 0x060000C3 RID: 195 RVA: 0x000055E0 File Offset: 0x000037E0
		public float LensFlareGhostChrDistortion
		{
			get
			{
				return this.m_lensFlareGhostChrDistortion;
			}
			set
			{
				this.m_lensFlareGhostChrDistortion = value;
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x000055EC File Offset: 0x000037EC
		// (set) Token: 0x060000C5 RID: 197 RVA: 0x000055F4 File Offset: 0x000037F4
		public float LensFlareHaloChrDistortion
		{
			get
			{
				return this.m_lensFlareHaloChrDistortion;
			}
			set
			{
				this.m_lensFlareHaloChrDistortion = value;
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060000C6 RID: 198 RVA: 0x00005600 File Offset: 0x00003800
		// (set) Token: 0x060000C7 RID: 199 RVA: 0x00005608 File Offset: 0x00003808
		public int LensFlareGaussianBlurAmount
		{
			get
			{
				return this.m_lensFlareGaussianBlurAmount;
			}
			set
			{
				this.m_lensFlareGaussianBlurAmount = value;
			}
		}

		// Token: 0x040000B1 RID: 177
		private const int LUTTextureWidth = 256;

		// Token: 0x040000B2 RID: 178
		[SerializeField]
		private float m_overallIntensity = 1f;

		// Token: 0x040000B3 RID: 179
		[SerializeField]
		private float m_normalizedGhostIntensity = 0.8f;

		// Token: 0x040000B4 RID: 180
		[SerializeField]
		private float m_normalizedHaloIntensity = 0.1f;

		// Token: 0x040000B5 RID: 181
		[SerializeField]
		private bool m_applyLensFlare = true;

		// Token: 0x040000B6 RID: 182
		[SerializeField]
		private int m_lensFlareGhostAmount = 3;

		// Token: 0x040000B7 RID: 183
		[SerializeField]
		private Vector4 m_lensFlareGhostsParams = new Vector4(0.8f, 0.228f, 1f, 4f);

		// Token: 0x040000B8 RID: 184
		[SerializeField]
		private float m_lensFlareGhostChrDistortion = 2f;

		// Token: 0x040000B9 RID: 185
		[SerializeField]
		private Gradient m_lensGradient;

		// Token: 0x040000BA RID: 186
		[SerializeField]
		private Texture2D m_lensFlareGradTexture;

		// Token: 0x040000BB RID: 187
		private Color[] m_lensFlareGradColor = new Color[256];

		// Token: 0x040000BC RID: 188
		[SerializeField]
		private Vector4 m_lensFlareHaloParams = new Vector4(0.1f, 0.573f, 1f, 128f);

		// Token: 0x040000BD RID: 189
		[SerializeField]
		private float m_lensFlareHaloChrDistortion = 1.51f;

		// Token: 0x040000BE RID: 190
		[SerializeField]
		private int m_lensFlareGaussianBlurAmount = 1;
	}
}
