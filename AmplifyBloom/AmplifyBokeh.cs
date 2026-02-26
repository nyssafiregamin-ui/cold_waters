using System;
using System.Collections.Generic;
using UnityEngine;

namespace AmplifyBloom
{
	// Token: 0x0200000B RID: 11
	[Serializable]
	public sealed class AmplifyBokeh : IAmplifyItem, ISerializationCallbackReceiver
	{
		// Token: 0x06000053 RID: 83 RVA: 0x000038B0 File Offset: 0x00001AB0
		public AmplifyBokeh()
		{
			this.m_bokehOffsets = new List<AmplifyBokehData>();
			this.CreateBokehOffsets(ApertureShape.Hexagon);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003908 File Offset: 0x00001B08
		public void Destroy()
		{
			for (int i = 0; i < this.m_bokehOffsets.Count; i++)
			{
				this.m_bokehOffsets[i].Destroy();
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003944 File Offset: 0x00001B44
		private void CreateBokehOffsets(ApertureShape shape)
		{
			this.m_bokehOffsets.Clear();
			switch (shape)
			{
			case ApertureShape.Square:
				this.m_bokehOffsets.Add(new AmplifyBokehData(this.CalculateBokehSamples(8, this.m_offsetRotation)));
				this.m_bokehOffsets.Add(new AmplifyBokehData(this.CalculateBokehSamples(8, this.m_offsetRotation + 90f)));
				break;
			case ApertureShape.Hexagon:
				this.m_bokehOffsets.Add(new AmplifyBokehData(this.CalculateBokehSamples(8, this.m_offsetRotation)));
				this.m_bokehOffsets.Add(new AmplifyBokehData(this.CalculateBokehSamples(8, this.m_offsetRotation - 75f)));
				this.m_bokehOffsets.Add(new AmplifyBokehData(this.CalculateBokehSamples(8, this.m_offsetRotation + 75f)));
				break;
			case ApertureShape.Octagon:
				this.m_bokehOffsets.Add(new AmplifyBokehData(this.CalculateBokehSamples(8, this.m_offsetRotation)));
				this.m_bokehOffsets.Add(new AmplifyBokehData(this.CalculateBokehSamples(8, this.m_offsetRotation + 65f)));
				this.m_bokehOffsets.Add(new AmplifyBokehData(this.CalculateBokehSamples(8, this.m_offsetRotation + 90f)));
				this.m_bokehOffsets.Add(new AmplifyBokehData(this.CalculateBokehSamples(8, this.m_offsetRotation + 115f)));
				break;
			}
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00003AB0 File Offset: 0x00001CB0
		private Vector4[] CalculateBokehSamples(int sampleCount, float angle)
		{
			Vector4[] array = new Vector4[sampleCount];
			float f = 0.017453292f * angle;
			float num = (float)Screen.width / (float)Screen.height;
			Vector4 vector = new Vector4(this.m_bokehSampleRadius * Mathf.Cos(f), this.m_bokehSampleRadius * Mathf.Sin(f));
			vector.x /= num;
			for (int i = 0; i < sampleCount; i++)
			{
				float t = (float)i / ((float)sampleCount - 1f);
				array[i] = Vector4.Lerp(-vector, vector, t);
			}
			return array;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003B4C File Offset: 0x00001D4C
		public void ApplyBokehFilter(RenderTexture source, Material material)
		{
			for (int i = 0; i < this.m_bokehOffsets.Count; i++)
			{
				this.m_bokehOffsets[i].BokehRenderTexture = AmplifyUtils.GetTempRenderTarget(source.width, source.height);
			}
			material.SetVector(AmplifyUtils.BokehParamsId, this.m_bokehCameraProperties);
			for (int j = 0; j < this.m_bokehOffsets.Count; j++)
			{
				for (int k = 0; k < 8; k++)
				{
					material.SetVector(AmplifyUtils.AnamorphicGlareWeightsStr[k], this.m_bokehOffsets[j].Offsets[k]);
				}
				Graphics.Blit(source, this.m_bokehOffsets[j].BokehRenderTexture, material, 27);
			}
			for (int l = 0; l < this.m_bokehOffsets.Count - 1; l++)
			{
				material.SetTexture(AmplifyUtils.AnamorphicRTS[l], this.m_bokehOffsets[l].BokehRenderTexture);
			}
			source.DiscardContents();
			Graphics.Blit(this.m_bokehOffsets[this.m_bokehOffsets.Count - 1].BokehRenderTexture, source, material, 28 + (this.m_bokehOffsets.Count - 2));
			for (int m = 0; m < this.m_bokehOffsets.Count; m++)
			{
				AmplifyUtils.ReleaseTempRenderTarget(this.m_bokehOffsets[m].BokehRenderTexture);
				this.m_bokehOffsets[m].BokehRenderTexture = null;
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00003CDC File Offset: 0x00001EDC
		public void OnAfterDeserialize()
		{
			this.CreateBokehOffsets(this.m_apertureShape);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003CEC File Offset: 0x00001EEC
		public void OnBeforeSerialize()
		{
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00003CF0 File Offset: 0x00001EF0
		// (set) Token: 0x0600005B RID: 91 RVA: 0x00003CF8 File Offset: 0x00001EF8
		public ApertureShape ApertureShape
		{
			get
			{
				return this.m_apertureShape;
			}
			set
			{
				if (this.m_apertureShape != value)
				{
					this.m_apertureShape = value;
					this.CreateBokehOffsets(value);
				}
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600005C RID: 92 RVA: 0x00003D14 File Offset: 0x00001F14
		// (set) Token: 0x0600005D RID: 93 RVA: 0x00003D1C File Offset: 0x00001F1C
		public bool ApplyBokeh
		{
			get
			{
				return this.m_isActive;
			}
			set
			{
				this.m_isActive = value;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00003D28 File Offset: 0x00001F28
		// (set) Token: 0x0600005F RID: 95 RVA: 0x00003D30 File Offset: 0x00001F30
		public bool ApplyOnBloomSource
		{
			get
			{
				return this.m_applyOnBloomSource;
			}
			set
			{
				this.m_applyOnBloomSource = value;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00003D3C File Offset: 0x00001F3C
		// (set) Token: 0x06000061 RID: 97 RVA: 0x00003D44 File Offset: 0x00001F44
		public float BokehSampleRadius
		{
			get
			{
				return this.m_bokehSampleRadius;
			}
			set
			{
				this.m_bokehSampleRadius = value;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000062 RID: 98 RVA: 0x00003D50 File Offset: 0x00001F50
		// (set) Token: 0x06000063 RID: 99 RVA: 0x00003D58 File Offset: 0x00001F58
		public float OffsetRotation
		{
			get
			{
				return this.m_offsetRotation;
			}
			set
			{
				this.m_offsetRotation = value;
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000064 RID: 100 RVA: 0x00003D64 File Offset: 0x00001F64
		// (set) Token: 0x06000065 RID: 101 RVA: 0x00003D6C File Offset: 0x00001F6C
		public Vector4 BokehCameraProperties
		{
			get
			{
				return this.m_bokehCameraProperties;
			}
			set
			{
				this.m_bokehCameraProperties = value;
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00003D78 File Offset: 0x00001F78
		// (set) Token: 0x06000067 RID: 103 RVA: 0x00003D88 File Offset: 0x00001F88
		public float Aperture
		{
			get
			{
				return this.m_bokehCameraProperties.x;
			}
			set
			{
				this.m_bokehCameraProperties.x = value;
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000068 RID: 104 RVA: 0x00003D98 File Offset: 0x00001F98
		// (set) Token: 0x06000069 RID: 105 RVA: 0x00003DA8 File Offset: 0x00001FA8
		public float FocalLength
		{
			get
			{
				return this.m_bokehCameraProperties.y;
			}
			set
			{
				this.m_bokehCameraProperties.y = value;
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x0600006A RID: 106 RVA: 0x00003DB8 File Offset: 0x00001FB8
		// (set) Token: 0x0600006B RID: 107 RVA: 0x00003DC8 File Offset: 0x00001FC8
		public float FocalDistance
		{
			get
			{
				return this.m_bokehCameraProperties.z;
			}
			set
			{
				this.m_bokehCameraProperties.z = value;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600006C RID: 108 RVA: 0x00003DD8 File Offset: 0x00001FD8
		// (set) Token: 0x0600006D RID: 109 RVA: 0x00003DE8 File Offset: 0x00001FE8
		public float MaxCoCDiameter
		{
			get
			{
				return this.m_bokehCameraProperties.w;
			}
			set
			{
				this.m_bokehCameraProperties.w = value;
			}
		}

		// Token: 0x04000073 RID: 115
		private const int PerPassSampleCount = 8;

		// Token: 0x04000074 RID: 116
		[SerializeField]
		private bool m_isActive;

		// Token: 0x04000075 RID: 117
		[SerializeField]
		private bool m_applyOnBloomSource;

		// Token: 0x04000076 RID: 118
		[SerializeField]
		private float m_bokehSampleRadius = 0.5f;

		// Token: 0x04000077 RID: 119
		[SerializeField]
		private Vector4 m_bokehCameraProperties = new Vector4(0.05f, 0.018f, 1.34f, 0.18f);

		// Token: 0x04000078 RID: 120
		[SerializeField]
		private float m_offsetRotation;

		// Token: 0x04000079 RID: 121
		[SerializeField]
		private ApertureShape m_apertureShape = ApertureShape.Hexagon;

		// Token: 0x0400007A RID: 122
		private List<AmplifyBokehData> m_bokehOffsets;
	}
}
