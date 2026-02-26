using System;
using System.Collections.Generic;
using Ceto.Common.Unity.Utility;
using UnityEngine;

namespace Ceto
{
	// Token: 0x020000A0 RID: 160
	public class WaveSpectrumBufferGPU : WaveSpectrumBuffer
	{
		// Token: 0x06000479 RID: 1145 RVA: 0x0001BE48 File Offset: 0x0001A048
		public WaveSpectrumBufferGPU(int size, Shader fourierSdr, int numBuffers)
		{
			if (numBuffers < 1 || numBuffers > 4)
			{
				throw new InvalidOperationException("Number of buffers is " + numBuffers + " but must be between (inclusive) 1 and 4");
			}
			this.m_buffers = new WaveSpectrumBufferGPU.Buffer[numBuffers];
			this.m_fourier = new FourierGPU(size, fourierSdr);
			this.m_tmpList = new List<RenderTexture>();
			this.m_offset = new Vector4(1f + 0.5f / (float)this.Size, 1f + 0.5f / (float)this.Size, 0f, 0f);
			for (int i = 0; i < numBuffers; i++)
			{
				this.m_buffers[i] = this.CreateBuffer(size);
			}
			this.m_enabledData = new List<RenderTexture[]>();
			this.UpdateEnabledData();
			this.m_bufferName = "Ceto Wave Spectrum GPU Buffer";
		}

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x0600047A RID: 1146 RVA: 0x0001BF30 File Offset: 0x0001A130
		public override bool Done
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x0600047B RID: 1147 RVA: 0x0001BF34 File Offset: 0x0001A134
		public override int Size
		{
			get
			{
				return this.m_fourier.size;
			}
		}

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x0600047C RID: 1148 RVA: 0x0001BF44 File Offset: 0x0001A144
		public override bool IsGPU
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600047D RID: 1149 RVA: 0x0001BF48 File Offset: 0x0001A148
		private WaveSpectrumBufferGPU.Buffer CreateBuffer(int size)
		{
			return new WaveSpectrumBufferGPU.Buffer
			{
				data = new RenderTexture[2]
			};
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x0001BF6C File Offset: 0x0001A16C
		public override Texture GetTexture(int idx)
		{
			if (this.m_index == -1)
			{
				return Texture2D.blackTexture;
			}
			if (idx < 0 || idx >= this.m_buffers.Length)
			{
				return Texture2D.blackTexture;
			}
			if (this.m_buffers[idx].disabled)
			{
				return Texture2D.blackTexture;
			}
			return this.m_buffers[idx].data[this.m_index];
		}

		// Token: 0x0600047F RID: 1151 RVA: 0x0001BFDC File Offset: 0x0001A1DC
		public override void Release()
		{
			this.m_tmpList.Clear();
			this.m_fourier.Release();
			int num = this.m_buffers.Length;
			for (int i = 0; i < num; i++)
			{
				RTUtility.ReleaseAndDestroy(this.m_buffers[i].data);
				this.m_buffers[i].data[0] = null;
				this.m_buffers[i].data[1] = null;
			}
		}

		// Token: 0x06000480 RID: 1152 RVA: 0x0001C058 File Offset: 0x0001A258
		protected override void Initilize(WaveSpectrumCondition condition, float time)
		{
			if (base.InitMaterial == null)
			{
				throw new InvalidOperationException("GPU buffer has not had its Init material set");
			}
			if (base.InitPass == -1)
			{
				throw new InvalidOperationException("GPU buffer has not had its Init material pass set");
			}
			base.InitMaterial.SetTexture("Ceto_Spectrum01", (!(condition.Spectrum01 != null)) ? Texture2D.blackTexture : condition.Spectrum01);
			base.InitMaterial.SetTexture("Ceto_Spectrum23", (!(condition.Spectrum23 != null)) ? Texture2D.blackTexture : condition.Spectrum23);
			base.InitMaterial.SetTexture("Ceto_WTable", condition.WTable);
			base.InitMaterial.SetVector("Ceto_InverseGridSizes", condition.InverseGridSizes());
			base.InitMaterial.SetVector("Ceto_GridSizes", condition.GridSizes);
			base.InitMaterial.SetVector("Ceto_Offset", this.m_offset);
			base.InitMaterial.SetFloat("Ceto_Time", time);
			this.m_tmpList.Clear();
			int num = this.m_buffers.Length;
			for (int i = 0; i < num; i++)
			{
				if (!this.m_buffers[i].disabled)
				{
					this.m_tmpList.Add(this.m_buffers[i].data[1]);
				}
			}
			num = this.m_tmpList.Count;
			if (num == 0)
			{
				return;
			}
			if (num == 1)
			{
				Graphics.Blit(null, this.m_tmpList[0], base.InitMaterial, base.InitPass);
			}
			else
			{
				RTUtility.MultiTargetBlit(this.m_tmpList, base.InitMaterial, base.InitPass);
			}
		}

		// Token: 0x06000481 RID: 1153 RVA: 0x0001C214 File Offset: 0x0001A414
		public void UpdateEnabledData()
		{
			this.m_enabledData.Clear();
			int num = this.m_buffers.Length;
			for (int i = 0; i < num; i++)
			{
				if (!this.m_buffers[i].disabled)
				{
					this.m_enabledData.Add(this.m_buffers[i].data);
				}
			}
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x0001C27C File Offset: 0x0001A47C
		public override void EnableBuffer(int idx)
		{
			int num = this.m_buffers.Length;
			if (idx < -1 || idx >= num)
			{
				return;
			}
			if (idx == -1)
			{
				for (int i = 0; i < num; i++)
				{
					this.m_buffers[i].disabled = false;
				}
			}
			else
			{
				this.m_buffers[idx].disabled = false;
			}
			this.UpdateEnabledData();
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x0001C2EC File Offset: 0x0001A4EC
		public override void DisableBuffer(int idx)
		{
			int num = this.m_buffers.Length;
			if (idx < -1 || idx >= num)
			{
				return;
			}
			if (idx == -1)
			{
				for (int i = 0; i < num; i++)
				{
					this.m_buffers[i].disabled = true;
				}
			}
			else
			{
				this.m_buffers[idx].disabled = true;
			}
			this.UpdateEnabledData();
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x0001C35C File Offset: 0x0001A55C
		public override int EnabledBuffers()
		{
			return this.m_enabledData.Count;
		}

		// Token: 0x06000485 RID: 1157 RVA: 0x0001C36C File Offset: 0x0001A56C
		public override bool IsEnabledBuffer(int idx)
		{
			return idx >= 0 && idx < this.m_buffers.Length && !this.m_buffers[idx].disabled;
		}

		// Token: 0x06000486 RID: 1158 RVA: 0x0001C39C File Offset: 0x0001A59C
		private void CreateTextures()
		{
			int count = this.m_enabledData.Count;
			for (int i = 0; i < count; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					if (this.m_enabledData[i][j] != null)
					{
						RenderTexture.ReleaseTemporary(this.m_enabledData[i][j]);
					}
					RenderTexture temporary = RenderTexture.GetTemporary(this.Size, this.Size, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
					temporary.filterMode = FilterMode.Point;
					temporary.wrapMode = TextureWrapMode.Clamp;
					temporary.name = this.m_bufferName;
					temporary.anisoLevel = 0;
					temporary.Create();
					this.m_enabledData[i][j] = temporary;
				}
			}
		}

		// Token: 0x06000487 RID: 1159 RVA: 0x0001C454 File Offset: 0x0001A654
		public override void Run(WaveSpectrumCondition condition, float time)
		{
			base.TimeValue = time;
			base.HasRun = true;
			base.BeenSampled = false;
			if (this.m_samplingEnabled)
			{
				throw new InvalidOperationException("Can not run if sampling enabled");
			}
			this.UpdateEnabledData();
			this.CreateTextures();
			int count = this.m_enabledData.Count;
			if (count == 0)
			{
				return;
			}
			this.Initilize(condition, time);
			if (count == 1)
			{
				this.m_index = this.m_fourier.PeformFFT(this.m_enabledData[0]);
			}
			else if (count == 2)
			{
				this.m_index = this.m_fourier.PeformFFT(this.m_enabledData[0], this.m_enabledData[1]);
			}
			else if (count == 3)
			{
				this.m_index = this.m_fourier.PeformFFT(this.m_enabledData[0], this.m_enabledData[1], this.m_enabledData[2]);
			}
			else if (count == 4)
			{
				this.m_index = this.m_fourier.PeformFFT(this.m_enabledData[0], this.m_enabledData[1], this.m_enabledData[2], this.m_enabledData[3]);
			}
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x0001C59C File Offset: 0x0001A79C
		public override void EnableSampling()
		{
			if (this.m_index == -1)
			{
				return;
			}
			this.m_samplingEnabled = true;
			int num = this.m_buffers.Length;
			for (int i = 0; i < num; i++)
			{
				if (!(this.m_buffers[i].data[this.m_index] == null))
				{
					this.m_buffers[i].data[this.m_index].filterMode = FilterMode.Bilinear;
					this.m_buffers[i].data[this.m_index].wrapMode = TextureWrapMode.Repeat;
				}
			}
		}

		// Token: 0x06000489 RID: 1161 RVA: 0x0001C63C File Offset: 0x0001A83C
		public override void DisableSampling()
		{
			if (this.m_index == -1)
			{
				return;
			}
			this.m_samplingEnabled = false;
			int num = this.m_buffers.Length;
			for (int i = 0; i < num; i++)
			{
				if (!(this.m_buffers[i].data[this.m_index] == null))
				{
					this.m_buffers[i].data[this.m_index].filterMode = FilterMode.Point;
					this.m_buffers[i].data[this.m_index].wrapMode = TextureWrapMode.Clamp;
				}
			}
		}

		// Token: 0x04000474 RID: 1140
		private WaveSpectrumBufferGPU.Buffer[] m_buffers;

		// Token: 0x04000475 RID: 1141
		private bool m_samplingEnabled;

		// Token: 0x04000476 RID: 1142
		private int m_index = -1;

		// Token: 0x04000477 RID: 1143
		private FourierGPU m_fourier;

		// Token: 0x04000478 RID: 1144
		private IList<RenderTexture[]> m_enabledData;

		// Token: 0x04000479 RID: 1145
		private string m_bufferName;

		// Token: 0x0400047A RID: 1146
		private IList<RenderTexture> m_tmpList;

		// Token: 0x0400047B RID: 1147
		private Vector4 m_offset;

		// Token: 0x020000A1 RID: 161
		private struct Buffer
		{
			// Token: 0x0400047C RID: 1148
			public RenderTexture[] data;

			// Token: 0x0400047D RID: 1149
			public bool disabled;
		}
	}
}
