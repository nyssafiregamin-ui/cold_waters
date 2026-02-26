using System;
using System.Collections.Generic;
using Ceto.Common.Containers.Interpolation;
using Ceto.Common.Containers.Queues;
using Ceto.Common.Threading.Scheduling;
using Ceto.Common.Threading.Tasks;
using Ceto.Common.Unity.Utility;
using UnityEngine;

namespace Ceto
{
	// Token: 0x020000B8 RID: 184
	[DisallowMultipleComponent]
	[AddComponentMenu("Ceto/Components/WaveSpectrum")]
	[RequireComponent(typeof(Ocean))]
	public class WaveSpectrum : WaveSpectrumBase
	{
		// Token: 0x17000117 RID: 279
		// (get) Token: 0x0600053A RID: 1338 RVA: 0x00021E64 File Offset: 0x00020064
		public override bool DisableReadBack
		{
			get
			{
				return this.disableReadBack;
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x0600053B RID: 1339 RVA: 0x00021E6C File Offset: 0x0002006C
		public override float GridScale
		{
			get
			{
				return this.gridScale;
			}
		}

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x0600053C RID: 1340 RVA: 0x00021E74 File Offset: 0x00020074
		public IList<RenderTexture> DisplacementMaps
		{
			get
			{
				return this.m_displacementMaps;
			}
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x0600053D RID: 1341 RVA: 0x00021E7C File Offset: 0x0002007C
		public IList<RenderTexture> SlopeMaps
		{
			get
			{
				return this.m_slopeMaps;
			}
		}

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x0600053E RID: 1342 RVA: 0x00021E84 File Offset: 0x00020084
		public IList<RenderTexture> FoamMaps
		{
			get
			{
				return this.m_foamMaps;
			}
		}

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x0600053F RID: 1343 RVA: 0x00021E8C File Offset: 0x0002008C
		// (set) Token: 0x06000540 RID: 1344 RVA: 0x00021E94 File Offset: 0x00020094
		public override Vector2 MaxDisplacement { get; set; }

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x06000541 RID: 1345 RVA: 0x00021EA0 File Offset: 0x000200A0
		public bool IsCreatingNewCondition
		{
			get
			{
				return this.m_conditions[1] != null;
			}
		}

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06000542 RID: 1346 RVA: 0x00021EB0 File Offset: 0x000200B0
		public override Vector4 GridSizes
		{
			get
			{
				return this.m_gridSizes;
			}
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x06000543 RID: 1347 RVA: 0x00021EB8 File Offset: 0x000200B8
		public override Vector4 Choppyness
		{
			get
			{
				return this.m_choppyness;
			}
		}

		// Token: 0x17000120 RID: 288
		// (set) Token: 0x06000544 RID: 1348 RVA: 0x00021EC0 File Offset: 0x000200C0
		public int MaxConditionCacheSize
		{
			set
			{
				this.m_maxConditionCacheSize = value;
			}
		}

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x06000545 RID: 1349 RVA: 0x00021ECC File Offset: 0x000200CC
		public override IDisplacementBuffer DisplacementBuffer
		{
			get
			{
				if (this.m_displacementBuffer == null)
				{
					return null;
				}
				if (!(this.m_displacementBuffer is IDisplacementBuffer))
				{
					throw new InvalidCastException("Displacement buffer cast exception");
				}
				return this.m_displacementBuffer as IDisplacementBuffer;
			}
		}

		// Token: 0x06000546 RID: 1350 RVA: 0x00021F04 File Offset: 0x00020104
		private void Start()
		{
			try
			{
				Shader.SetGlobalTexture("Ceto_SlopeMap0", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_SlopeMap1", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_DisplacementMap0", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_DisplacementMap1", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_DisplacementMap2", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_DisplacementMap3", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_FoamMap0", Texture2D.blackTexture);
				this.m_slopeCopyMat = new Material(this.slopeCopySdr);
				this.m_displacementCopyMat = new Material(this.displacementCopySdr);
				this.m_foamCopyMat = new Material(this.foamCopySdr);
				this.m_slopeInitMat = new Material(this.initSlopeSdr);
				this.m_displacementInitMat = new Material(this.initDisplacementSdr);
				this.m_foamInitMat = new Material(this.initJacobianSdr);
				this.m_conditionCache = new DictionaryQueue<WaveSpectrumConditionKey, WaveSpectrumCondition>();
				this.m_scheduler = new Scheduler();
				this.CreateBuffers();
				this.CreateRenderTextures();
				this.CreateConditions();
				this.UpdateQueryScaling();
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x00022050 File Offset: 0x00020250
		protected override void OnDisable()
		{
			base.OnDisable();
			Shader.DisableKeyword("CETO_USE_4_SPECTRUM_GRIDS");
			Shader.SetGlobalTexture("Ceto_SlopeMap0", Texture2D.blackTexture);
			Shader.SetGlobalTexture("Ceto_SlopeMap1", Texture2D.blackTexture);
			Shader.SetGlobalTexture("Ceto_DisplacementMap0", Texture2D.blackTexture);
			Shader.SetGlobalTexture("Ceto_DisplacementMap1", Texture2D.blackTexture);
			Shader.SetGlobalTexture("Ceto_DisplacementMap2", Texture2D.blackTexture);
			Shader.SetGlobalTexture("Ceto_DisplacementMap3", Texture2D.blackTexture);
			Shader.SetGlobalTexture("Ceto_FoamMap0", Texture2D.blackTexture);
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x000220D8 File Offset: 0x000202D8
		protected override void OnDestroy()
		{
			base.OnDestroy();
			try
			{
				if (this.m_scheduler != null)
				{
					this.m_scheduler.ShutingDown = true;
					this.m_scheduler.CancelAllTasks();
				}
				if (this.m_conditionCache != null)
				{
					foreach (WaveSpectrumCondition waveSpectrumCondition in this.m_conditionCache)
					{
						waveSpectrumCondition.Release();
					}
					this.m_conditionCache.Clear();
					this.m_conditionCache = null;
				}
				this.Release();
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x000221C4 File Offset: 0x000203C4
		private void Release()
		{
			if (this.m_displacementBuffer != null)
			{
				this.m_displacementBuffer.Release();
				this.m_displacementBuffer = null;
			}
			if (this.m_slopeBuffer != null)
			{
				this.m_slopeBuffer.Release();
				this.m_slopeBuffer = null;
			}
			if (this.m_jacobianBuffer != null)
			{
				this.m_jacobianBuffer.Release();
				this.m_jacobianBuffer = null;
			}
			if (this.m_readBuffer != null)
			{
				this.m_readBuffer.Release();
				this.m_readBuffer = null;
			}
			if (this.m_conditions != null && this.m_conditions[0] != null && this.m_conditions[0].Done)
			{
				this.CacheCondition(this.m_conditions[0]);
				if (this.m_conditionCache == null || !this.m_conditionCache.ContainsKey(this.m_conditions[0].Key))
				{
					this.m_conditions[0].Release();
					this.m_conditions[0] = null;
				}
			}
			if (this.m_conditions != null && this.m_conditions[1] != null && this.m_conditions[1].Done && (this.m_conditionCache == null || !this.m_conditionCache.ContainsKey(this.m_conditions[1].Key)))
			{
				this.m_conditions[1].Release();
				this.m_conditions[1] = null;
			}
			this.m_conditions = null;
			this.m_findRangeTask = null;
			RTUtility.ReleaseAndDestroy(this.m_displacementMaps);
			this.m_displacementMaps = null;
			RTUtility.ReleaseAndDestroy(this.m_slopeMaps);
			this.m_slopeMaps = null;
			RTUtility.ReleaseAndDestroy(this.m_foamMaps);
			this.m_foamMaps = null;
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x00022368 File Offset: 0x00020568
		private void Update()
		{
			try
			{
				this.gridScale = Mathf.Clamp(this.gridScale, 0.01f, 1f);
				this.windSpeed = Mathf.Clamp(this.windSpeed, 0f, 30f);
				this.waveAge = Mathf.Clamp(this.waveAge, 0.5f, 1f);
				this.waveSpeed = Mathf.Clamp(this.waveSpeed, 0f, 10f);
				this.foamAmount = Mathf.Clamp(this.foamAmount, 0f, 6f);
				this.foamCoverage = Mathf.Clamp(this.foamCoverage, 0f, 0.5f);
				this.waveSmoothing = Mathf.Clamp(this.waveSmoothing, 1f, 6f);
				this.slopeSmoothing = Mathf.Clamp(this.slopeSmoothing, 1f, 6f);
				this.foamSmoothing = Mathf.Clamp(this.foamSmoothing, 1f, 6f);
				float time = this.m_ocean.OceanTime.Now * this.waveSpeed;
				this.CreateBuffers();
				this.CreateRenderTextures();
				this.CreateConditions();
				int numGrids = this.m_conditions[0].Key.NumGrids;
				if (numGrids > 2)
				{
					Shader.EnableKeyword("CETO_USE_4_SPECTRUM_GRIDS");
				}
				else
				{
					Shader.DisableKeyword("CETO_USE_4_SPECTRUM_GRIDS");
				}
				this.UpdateQueryScaling();
				Shader.SetGlobalVector("Ceto_GridSizes", this.GridSizes);
				Shader.SetGlobalVector("Ceto_GridScale", new Vector2(this.GridScale, this.GridScale));
				Shader.SetGlobalVector("Ceto_Choppyness", this.Choppyness);
				Shader.SetGlobalFloat("Ceto_MapSize", (float)this.m_bufferSettings.size);
				Shader.SetGlobalFloat("Ceto_WaveSmoothing", this.waveSmoothing);
				Shader.SetGlobalFloat("Ceto_SlopeSmoothing", this.slopeSmoothing);
				Shader.SetGlobalFloat("Ceto_FoamSmoothing", this.foamSmoothing);
				Shader.SetGlobalFloat("Ceto_TextureWaveFoam", (!this.textureFoam) ? 0f : 1f);
				this.UpdateSpectrumScheduler();
				this.GenerateDisplacement(time);
				this.GenerateSlopes(time);
				this.GenerateFoam(time);
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		// Token: 0x0600054B RID: 1355 RVA: 0x000225D0 File Offset: 0x000207D0
		private void UpdateSpectrumScheduler()
		{
			try
			{
				this.m_scheduler.DisableMultithreading = Ocean.DISABLE_ALL_MULTITHREADING;
				this.m_scheduler.CheckForException();
				this.m_scheduler.Update();
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		// Token: 0x0600054C RID: 1356 RVA: 0x00022644 File Offset: 0x00020844
		private void UpdateQueryScaling()
		{
			this.m_choppyness = this.m_conditions[0].Choppyness * Mathf.Clamp(this.choppyness, 0f, 1.2f);
			this.m_gridSizes = this.m_conditions[0].GridSizes;
			Vector4 invGridSizes = default(Vector4);
			invGridSizes.x = 1f / (this.GridSizes.x * this.GridScale);
			invGridSizes.y = 1f / (this.GridSizes.y * this.GridScale);
			invGridSizes.z = 1f / (this.GridSizes.z * this.GridScale);
			invGridSizes.w = 1f / (this.GridSizes.w * this.GridScale);
			this.m_queryScaling.invGridSizes = invGridSizes;
			this.m_queryScaling.scaleY = this.GridScale;
			this.m_queryScaling.choppyness = this.Choppyness * this.GridScale;
			this.m_queryScaling.offset = this.m_ocean.PositionOffset;
			this.m_queryScaling.numGrids = this.m_conditions[0].Key.NumGrids;
			if (this.m_queryScaling.tmp == null)
			{
				this.m_queryScaling.tmp = new float[QueryDisplacements.CHANNELS];
			}
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x000227B4 File Offset: 0x000209B4
		private void GenerateSlopes(float time)
		{
			if (!this.disableSlopes && SystemInfo.graphicsShaderLevel < 30)
			{
				Ocean.LogWarning("Spectrum slopes needs at least SM3 to run. Disabling slopes.");
				this.disableSlopes = true;
			}
			if (this.disableSlopes)
			{
				this.m_slopeBuffer.DisableBuffer(-1);
			}
			else
			{
				this.m_slopeBuffer.EnableBuffer(-1);
			}
			if (this.m_slopeBuffer.EnabledBuffers() == 0)
			{
				Shader.SetGlobalTexture("Ceto_SlopeMap0", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_SlopeMap1", Texture2D.blackTexture);
			}
			else
			{
				int numGrids = this.m_conditions[0].Key.NumGrids;
				if (numGrids <= 2)
				{
					this.m_slopeBuffer.DisableBuffer(1);
				}
				if (!this.m_slopeBuffer.HasRun || this.m_slopeBuffer.TimeValue != time)
				{
					this.m_slopeBuffer.InitMaterial = this.m_slopeInitMat;
					this.m_slopeBuffer.InitPass = numGrids - 1;
					this.m_slopeBuffer.Run(this.m_conditions[0], time);
				}
				if (!this.m_slopeBuffer.BeenSampled)
				{
					this.m_slopeBuffer.EnableSampling();
					if (numGrids > 0)
					{
						this.m_slopeCopyMat.SetTexture("Ceto_SlopeBuffer", this.m_slopeBuffer.GetTexture(0));
						Graphics.Blit(null, this.m_slopeMaps[0], this.m_slopeCopyMat, 0);
						Shader.SetGlobalTexture("Ceto_SlopeMap0", this.m_slopeMaps[0]);
					}
					else
					{
						Shader.SetGlobalTexture("Ceto_SlopeMap0", Texture2D.blackTexture);
					}
					if (numGrids > 2)
					{
						this.m_slopeCopyMat.SetTexture("Ceto_SlopeBuffer", this.m_slopeBuffer.GetTexture(1));
						Graphics.Blit(null, this.m_slopeMaps[1], this.m_slopeCopyMat, 0);
						Shader.SetGlobalTexture("Ceto_SlopeMap1", this.m_slopeMaps[1]);
					}
					else
					{
						Shader.SetGlobalTexture("Ceto_SlopeMap1", Texture2D.blackTexture);
					}
					this.m_slopeBuffer.DisableSampling();
					this.m_slopeBuffer.BeenSampled = true;
				}
			}
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x000229AC File Offset: 0x00020BAC
		private void GenerateDisplacement(float time)
		{
			if (!this.disableDisplacements && SystemInfo.graphicsShaderLevel < 30 && this.m_displacementBuffer.IsGPU)
			{
				Ocean.LogWarning("Spectrum displacements needs at least SM3 to run on GPU. Disabling displacement.");
				this.disableDisplacements = true;
			}
			this.m_displacementBuffer.EnableBuffer(-1);
			if (this.disableDisplacements)
			{
				this.m_displacementBuffer.DisableBuffer(-1);
			}
			if (!this.disableDisplacements && this.choppyness == 0f)
			{
				this.m_displacementBuffer.DisableBuffer(1);
				this.m_displacementBuffer.DisableBuffer(2);
			}
			if (!this.disableDisplacements && this.choppyness > 0f)
			{
				this.m_displacementBuffer.EnableBuffer(1);
				this.m_displacementBuffer.EnableBuffer(2);
			}
			if (this.m_displacementBuffer.EnabledBuffers() == 0)
			{
				Shader.SetGlobalTexture("Ceto_DisplacementMap0", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_DisplacementMap1", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_DisplacementMap2", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_DisplacementMap3", Texture2D.blackTexture);
				return;
			}
			if (this.m_displacementBuffer.Done)
			{
				int numGrids = this.m_conditions[0].Key.NumGrids;
				if (numGrids <= 2)
				{
					this.m_displacementBuffer.DisableBuffer(2);
				}
				if (!this.m_displacementBuffer.HasRun || this.m_displacementBuffer.TimeValue != time)
				{
					this.m_displacementBuffer.InitMaterial = this.m_displacementInitMat;
					this.m_displacementBuffer.InitPass = numGrids - 1;
					this.m_displacementBuffer.Run(this.m_conditions[0], time);
				}
				if (!this.m_displacementBuffer.BeenSampled)
				{
					this.m_displacementBuffer.EnableSampling();
					this.m_displacementCopyMat.SetTexture("Ceto_HeightBuffer", this.m_displacementBuffer.GetTexture(0));
					this.m_displacementCopyMat.SetTexture("Ceto_DisplacementBuffer", this.m_displacementBuffer.GetTexture(1));
					if (numGrids > 0)
					{
						Graphics.Blit(null, this.m_displacementMaps[0], this.m_displacementCopyMat, (numGrids != 1) ? 0 : 4);
						Shader.SetGlobalTexture("Ceto_DisplacementMap0", this.m_displacementMaps[0]);
					}
					else
					{
						Shader.SetGlobalTexture("Ceto_DisplacementMap0", Texture2D.blackTexture);
					}
					if (numGrids > 1)
					{
						Graphics.Blit(null, this.m_displacementMaps[1], this.m_displacementCopyMat, 1);
						Shader.SetGlobalTexture("Ceto_DisplacementMap1", this.m_displacementMaps[1]);
					}
					else
					{
						Shader.SetGlobalTexture("Ceto_DisplacementMap1", Texture2D.blackTexture);
					}
					this.m_displacementCopyMat.SetTexture("Ceto_DisplacementBuffer", this.m_displacementBuffer.GetTexture(2));
					if (numGrids > 2)
					{
						Graphics.Blit(null, this.m_displacementMaps[2], this.m_displacementCopyMat, 2);
						Shader.SetGlobalTexture("Ceto_DisplacementMap2", this.m_displacementMaps[2]);
					}
					else
					{
						Shader.SetGlobalTexture("Ceto_DisplacementMap2", Texture2D.blackTexture);
					}
					if (numGrids > 3)
					{
						Graphics.Blit(null, this.m_displacementMaps[3], this.m_displacementCopyMat, 3);
						Shader.SetGlobalTexture("Ceto_DisplacementMap3", this.m_displacementMaps[3]);
					}
					else
					{
						Shader.SetGlobalTexture("Ceto_DisplacementMap3", Texture2D.blackTexture);
					}
					this.m_displacementBuffer.DisableSampling();
					this.m_displacementBuffer.BeenSampled = true;
					if (this.m_displacementBuffer.IsGPU)
					{
						this.ReadFromGPU(numGrids);
					}
					this.FindRanges();
				}
			}
		}

		// Token: 0x0600054F RID: 1359 RVA: 0x00022D0C File Offset: 0x00020F0C
		private void ReadFromGPU(int numGrids)
		{
			if (!this.disableReadBack && this.readSdr == null)
			{
				Ocean.LogWarning("Trying to read GPU displacement data but the read shader is null");
			}
			bool flag = SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;
			if (!this.disableReadBack && this.readSdr != null && this.m_readBuffer != null && flag)
			{
				InterpolatedArray2f[] readDisplacements = this.DisplacementBuffer.GetReadDisplacements();
				if (numGrids > 0)
				{
					CBUtility.ReadFromRenderTexture(this.m_displacementMaps[0], 3, this.m_readBuffer, this.readSdr);
					this.m_readBuffer.GetData(readDisplacements[0].Data);
				}
				else
				{
					readDisplacements[0].Clear();
				}
				if (numGrids > 1)
				{
					CBUtility.ReadFromRenderTexture(this.m_displacementMaps[1], 3, this.m_readBuffer, this.readSdr);
					this.m_readBuffer.GetData(readDisplacements[1].Data);
				}
				else
				{
					readDisplacements[1].Clear();
				}
				if (numGrids > 2)
				{
					CBUtility.ReadFromRenderTexture(this.m_displacementMaps[2], 3, this.m_readBuffer, this.readSdr);
					this.m_readBuffer.GetData(readDisplacements[2].Data);
				}
				else
				{
					readDisplacements[2].Clear();
				}
				if (numGrids > 3)
				{
				}
			}
		}

		// Token: 0x06000550 RID: 1360 RVA: 0x00022E5C File Offset: 0x0002105C
		private void FindRanges()
		{
			if (this.disableReadBack && this.DisplacementBuffer.IsGPU)
			{
				this.MaxDisplacement = new Vector2(0f, 40f * this.gridScale);
			}
			else if (this.m_findRangeTask == null || this.m_findRangeTask.Done)
			{
				if (this.m_findRangeTask == null)
				{
					this.m_findRangeTask = new FindRangeTask(this);
				}
				this.m_findRangeTask.Reset();
				this.m_scheduler.Run(this.m_findRangeTask);
			}
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x00022EF4 File Offset: 0x000210F4
		private void GenerateFoam(float time)
		{
			Vector4 vector = this.Choppyness;
			if (!this.disableFoam && SystemInfo.graphicsShaderLevel < 30)
			{
				Ocean.LogWarning("Spectrum foam needs at least SM3 to run. Disabling foam.");
				this.disableFoam = true;
			}
			float sqrMagnitude = vector.sqrMagnitude;
			this.m_jacobianBuffer.EnableBuffer(-1);
			if (this.disableFoam || this.foamAmount == 0f || sqrMagnitude == 0f || !this.m_conditions[0].SupportsJacobians)
			{
				this.m_jacobianBuffer.DisableBuffer(-1);
			}
			if (this.m_jacobianBuffer.EnabledBuffers() == 0)
			{
				Shader.SetGlobalTexture("Ceto_FoamMap0", Texture2D.blackTexture);
			}
			else
			{
				int numGrids = this.m_conditions[0].Key.NumGrids;
				if (numGrids == 1)
				{
					this.m_jacobianBuffer.DisableBuffer(1);
					this.m_jacobianBuffer.DisableBuffer(2);
				}
				else if (numGrids == 2)
				{
					this.m_jacobianBuffer.DisableBuffer(2);
				}
				if (!this.m_jacobianBuffer.HasRun || this.m_jacobianBuffer.TimeValue != time)
				{
					this.m_foamInitMat.SetFloat("Ceto_FoamAmount", this.foamAmount);
					this.m_jacobianBuffer.InitMaterial = this.m_foamInitMat;
					this.m_jacobianBuffer.InitPass = numGrids - 1;
					this.m_jacobianBuffer.Run(this.m_conditions[0], time);
				}
				if (!this.m_jacobianBuffer.BeenSampled)
				{
					this.m_jacobianBuffer.EnableSampling();
					this.m_foamCopyMat.SetTexture("Ceto_JacobianBuffer0", this.m_jacobianBuffer.GetTexture(0));
					this.m_foamCopyMat.SetTexture("Ceto_JacobianBuffer1", this.m_jacobianBuffer.GetTexture(1));
					this.m_foamCopyMat.SetTexture("Ceto_JacobianBuffer2", this.m_jacobianBuffer.GetTexture(2));
					this.m_foamCopyMat.SetTexture("Ceto_HeightBuffer", this.m_displacementBuffer.GetTexture(0));
					this.m_foamCopyMat.SetVector("Ceto_FoamChoppyness", vector);
					this.m_foamCopyMat.SetFloat("Ceto_FoamCoverage", this.foamCoverage);
					Graphics.Blit(null, this.m_foamMaps[0], this.m_foamCopyMat, numGrids - 1);
					Shader.SetGlobalTexture("Ceto_FoamMap0", this.m_foamMaps[0]);
					this.m_jacobianBuffer.DisableSampling();
					this.m_jacobianBuffer.BeenSampled = true;
				}
			}
		}

		// Token: 0x06000552 RID: 1362 RVA: 0x00023154 File Offset: 0x00021354
		public override void QueryWaves(WaveQuery query)
		{
			if (!base.enabled)
			{
				return;
			}
			IDisplacementBuffer displacementBuffer = this.DisplacementBuffer;
			if (displacementBuffer == null)
			{
				return;
			}
			if (this.disableReadBack && displacementBuffer.IsGPU)
			{
				return;
			}
			if (query.mode != QUERY_MODE.DISPLACEMENT && query.mode != QUERY_MODE.POSITION)
			{
				return;
			}
			if (!query.sampleSpectrum[0] && !query.sampleSpectrum[1] && !query.sampleSpectrum[2] && !query.sampleSpectrum[3])
			{
				return;
			}
			displacementBuffer.QueryWaves(query, this.m_queryScaling);
		}

		// Token: 0x06000553 RID: 1363 RVA: 0x000231EC File Offset: 0x000213EC
		private void CreateBuffers()
		{
			int num;
			bool flag;
			this.GetFourierSize(out num, out flag);
			if (this.m_bufferSettings.beenCreated)
			{
				if (this.m_bufferSettings.size == num && this.m_bufferSettings.isCpu == flag)
				{
					return;
				}
				while (this.m_scheduler.HasTasks())
				{
					this.UpdateSpectrumScheduler();
				}
				this.Release();
				this.m_bufferSettings.beenCreated = false;
			}
			if (flag)
			{
				this.m_displacementBuffer = new DisplacementBufferCPU(num, this.m_scheduler);
			}
			else
			{
				this.m_displacementBuffer = new DisplacementBufferGPU(num, this.fourierSdr);
			}
			this.m_slopeBuffer = new WaveSpectrumBufferGPU(num, this.fourierSdr, 2);
			this.m_jacobianBuffer = new WaveSpectrumBufferGPU(num, this.fourierSdr, 3);
			this.m_readBuffer = new ComputeBuffer(num * num, 12);
			this.m_conditions = new WaveSpectrumCondition[2];
			this.m_displacementMaps = new RenderTexture[4];
			this.m_slopeMaps = new RenderTexture[2];
			this.m_foamMaps = new RenderTexture[1];
			this.m_bufferSettings.beenCreated = true;
			this.m_bufferSettings.size = num;
			this.m_bufferSettings.isCpu = flag;
		}

		// Token: 0x06000554 RID: 1364 RVA: 0x0002331C File Offset: 0x0002151C
		private void CreateRenderTextures()
		{
			int size = this.m_bufferSettings.size;
			int ansio = 9;
			RenderTextureFormat format = RenderTextureFormat.ARGBFloat;
			for (int i = 0; i < this.m_displacementMaps.Length; i++)
			{
				this.CreateMap(ref this.m_displacementMaps[i], "Displacement", format, size, ansio);
			}
			for (int j = 0; j < this.m_slopeMaps.Length; j++)
			{
				this.CreateMap(ref this.m_slopeMaps[j], "Slope", format, size, ansio);
			}
			for (int k = 0; k < this.m_foamMaps.Length; k++)
			{
				this.CreateMap(ref this.m_foamMaps[k], "Foam", format, size, ansio);
			}
		}

		// Token: 0x06000555 RID: 1365 RVA: 0x000233E0 File Offset: 0x000215E0
		private void CreateMap(ref RenderTexture map, string name, RenderTextureFormat format, int size, int ansio)
		{
			if (map != null)
			{
				if (!map.IsCreated())
				{
					map.Create();
				}
				return;
			}
			map = new RenderTexture(size, size, 0, format, RenderTextureReadWrite.Linear);
			map.filterMode = FilterMode.Trilinear;
			map.wrapMode = TextureWrapMode.Repeat;
			map.anisoLevel = ansio;
			map.useMipMap = true;
			map.hideFlags = HideFlags.HideAndDontSave;
			map.name = "Ceto Wave Spectrum " + name + " Texture";
			map.Create();
		}

		// Token: 0x06000556 RID: 1366 RVA: 0x00023468 File Offset: 0x00021668
		private void CreateConditions()
		{
			int size = this.m_bufferSettings.size;
			WaveSpectrumConditionKey waveSpectrumConditionKey = this.NewSpectrumConditionKey(size, this.windSpeed, this.m_ocean.windDir, this.waveAge);
			if (this.m_conditions[0] == null)
			{
				if (this.m_conditionCache.ContainsKey(waveSpectrumConditionKey))
				{
					this.m_conditions[0] = this.m_conditionCache[waveSpectrumConditionKey];
					this.m_conditionCache.Remove(waveSpectrumConditionKey);
				}
				else
				{
					this.m_conditions[0] = this.NewSpectrumCondition(size, this.windSpeed, this.m_ocean.windDir, this.waveAge);
					IThreadedTask createSpectrumConditionTask = this.m_conditions[0].GetCreateSpectrumConditionTask();
					createSpectrumConditionTask.Start();
					createSpectrumConditionTask.Run();
					createSpectrumConditionTask.End();
				}
			}
			else if (this.m_conditions[1] != null && this.m_conditions[1].Done)
			{
				this.CacheCondition(this.m_conditions[0]);
				this.m_conditions[0] = this.m_conditions[1];
				this.m_conditions[1] = null;
			}
			else if (this.m_conditions[1] == null && this.m_conditions[0].Done && waveSpectrumConditionKey != this.m_conditions[0].Key)
			{
				if (this.m_conditionCache.ContainsKey(waveSpectrumConditionKey))
				{
					this.m_conditions[0] = this.m_conditionCache[waveSpectrumConditionKey];
					this.m_conditionCache.Remove(waveSpectrumConditionKey);
				}
				else
				{
					this.m_conditions[1] = this.NewSpectrumCondition(size, this.windSpeed, this.m_ocean.windDir, this.waveAge);
					IThreadedTask createSpectrumConditionTask2 = this.m_conditions[1].GetCreateSpectrumConditionTask();
					this.m_scheduler.Add(createSpectrumConditionTask2);
				}
			}
		}

		// Token: 0x06000557 RID: 1367 RVA: 0x0002362C File Offset: 0x0002182C
		private void CacheCondition(WaveSpectrumCondition condition)
		{
			if (condition == null || this.m_conditionCache == null)
			{
				return;
			}
			if (this.m_conditionCache.ContainsKey(condition.Key))
			{
				return;
			}
			this.m_conditionCache.AddFirst(condition.Key, condition);
			while (this.m_conditionCache.Count != 0 && this.m_conditionCache.Count > this.m_maxConditionCacheSize)
			{
				WaveSpectrumCondition waveSpectrumCondition = this.m_conditionCache.RemoveLast();
				waveSpectrumCondition.Release();
			}
		}

		// Token: 0x06000558 RID: 1368 RVA: 0x000236B4 File Offset: 0x000218B4
		public void CreateAndCacheCondition(FOURIER_SIZE fourierSize)
		{
			int num;
			bool flag;
			this.GetFourierSize(out num, out flag);
			this.CreateAndCacheCondition(num, this.windSpeed, this.m_ocean.windDir, this.waveAge);
		}

		// Token: 0x06000559 RID: 1369 RVA: 0x000236EC File Offset: 0x000218EC
		public void CreateAndCacheCondition(int fourierSize, float windSpeed, float windDir, float waveAge)
		{
			if (this.m_conditionCache == null)
			{
				return;
			}
			if (this.m_conditionCache.Count >= this.m_maxConditionCacheSize)
			{
				Ocean.LogWarning("Condition cache full. Condition not cached.");
				return;
			}
			if (!Mathf.IsPowerOfTwo(fourierSize) || fourierSize < 32 || fourierSize > 512)
			{
				Ocean.LogWarning("Fourier size must be a pow2 number from 32 to 512. Condition not cached.");
				return;
			}
			WaveSpectrumCondition waveSpectrumCondition = this.NewSpectrumCondition(fourierSize, windSpeed, windDir, waveAge);
			if (this.m_conditionCache.ContainsKey(waveSpectrumCondition.Key))
			{
				return;
			}
			IThreadedTask createSpectrumConditionTask = waveSpectrumCondition.GetCreateSpectrumConditionTask();
			createSpectrumConditionTask.Start();
			createSpectrumConditionTask.Run();
			createSpectrumConditionTask.End();
			this.m_conditionCache.AddFirst(waveSpectrumCondition.Key, waveSpectrumCondition);
		}

		// Token: 0x0600055A RID: 1370 RVA: 0x000237A0 File Offset: 0x000219A0
		private WaveSpectrumCondition NewSpectrumCondition(int fourierSize, float windSpeed, float windDir, float waveAge)
		{
			WaveSpectrumCondition result;
			switch (this.spectrumType)
			{
			case SPECTRUM_TYPE.UNIFIED:
				result = new UnifiedSpectrumCondition(fourierSize, windSpeed, windDir, waveAge, this.numberOfGrids);
				break;
			case SPECTRUM_TYPE.PHILLIPS:
				result = new PhillipsSpectrumCondition(fourierSize, windSpeed, windDir, waveAge, this.numberOfGrids);
				break;
			case SPECTRUM_TYPE.UNIFIED_PHILLIPS:
				result = new UnifiedPhillipsSpectrumCondition(fourierSize, windSpeed, windDir, waveAge, this.numberOfGrids);
				break;
			case SPECTRUM_TYPE.CUSTOM:
				if (base.CustomWaveSpectrum == null)
				{
					Ocean.LogWarning("Custom spectrum type selected but no custom spectrum interface has been added to the wave spectrum. Defaulting to Unified Spectrum");
					this.spectrumType = SPECTRUM_TYPE.UNIFIED;
					result = new UnifiedSpectrumCondition(fourierSize, windSpeed, windDir, waveAge, this.numberOfGrids);
				}
				else
				{
					result = new CustomWaveSpectrumCondition(base.CustomWaveSpectrum, fourierSize, windDir, this.numberOfGrids);
				}
				break;
			default:
				throw new InvalidOperationException("Invalid spectrum type = " + this.spectrumType);
			}
			return result;
		}

		// Token: 0x0600055B RID: 1371 RVA: 0x0002387C File Offset: 0x00021A7C
		private WaveSpectrumConditionKey NewSpectrumConditionKey(int fourierSize, float windSpeed, float windDir, float waveAge)
		{
			WaveSpectrumConditionKey result;
			switch (this.spectrumType)
			{
			case SPECTRUM_TYPE.UNIFIED:
				result = new UnifiedSpectrumConditionKey(windSpeed, waveAge, fourierSize, windDir, this.spectrumType, this.numberOfGrids);
				break;
			case SPECTRUM_TYPE.PHILLIPS:
				result = new PhillipsSpectrumConditionKey(windSpeed, fourierSize, windDir, this.spectrumType, this.numberOfGrids);
				break;
			case SPECTRUM_TYPE.UNIFIED_PHILLIPS:
				result = new UnifiedSpectrumConditionKey(windSpeed, waveAge, fourierSize, windDir, this.spectrumType, this.numberOfGrids);
				break;
			case SPECTRUM_TYPE.CUSTOM:
				if (base.CustomWaveSpectrum == null)
				{
					Ocean.LogWarning("Custom spectrum type selected but no custom spectrum interface has been added to the wave spectrum. Defaulting to Unified Spectrum");
					this.spectrumType = SPECTRUM_TYPE.UNIFIED;
					result = new UnifiedSpectrumConditionKey(windSpeed, waveAge, fourierSize, windDir, this.spectrumType, this.numberOfGrids);
				}
				else
				{
					result = base.CustomWaveSpectrum.CreateKey(fourierSize, windDir, this.spectrumType, this.numberOfGrids);
				}
				break;
			default:
				throw new InvalidOperationException("Invalid spectrum type = " + this.spectrumType);
			}
			return result;
		}

		// Token: 0x0600055C RID: 1372 RVA: 0x00023974 File Offset: 0x00021B74
		private void GetFourierSize(out int size, out bool isCpu)
		{
			switch (this.fourierSize)
			{
			case FOURIER_SIZE.LOW_32_CPU:
				size = 32;
				isCpu = true;
				break;
			case FOURIER_SIZE.LOW_32_GPU:
				size = 32;
				isCpu = false;
				break;
			case FOURIER_SIZE.MEDIUM_64_CPU:
				size = 64;
				isCpu = true;
				break;
			case FOURIER_SIZE.MEDIUM_64_GPU:
				size = 64;
				isCpu = false;
				break;
			case FOURIER_SIZE.HIGH_128_CPU:
				size = 128;
				isCpu = true;
				break;
			case FOURIER_SIZE.HIGH_128_GPU:
				size = 128;
				isCpu = false;
				break;
			case FOURIER_SIZE.ULTRA_256_GPU:
				size = 256;
				isCpu = false;
				break;
			case FOURIER_SIZE.EXTREME_512_GPU:
				size = 512;
				isCpu = false;
				break;
			default:
				size = 64;
				isCpu = true;
				break;
			}
			bool flag = SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;
			if (!isCpu && !this.disableReadBack && !flag)
			{
				Ocean.LogWarning("You card does not support dx11. Fourier can not be GPU. Changing to CPU. Disable read backs to use GPU but with no height querys.");
				this.fourierSize = FOURIER_SIZE.MEDIUM_64_CPU;
				size = 64;
				isCpu = true;
			}
		}

		// Token: 0x04000500 RID: 1280
		public const float MAX_CHOPPYNESS = 1.2f;

		// Token: 0x04000501 RID: 1281
		public const float MAX_FOAM_AMOUNT = 6f;

		// Token: 0x04000502 RID: 1282
		public const float MAX_FOAM_COVERAGE = 0.5f;

		// Token: 0x04000503 RID: 1283
		public const float MAX_WIND_SPEED = 30f;

		// Token: 0x04000504 RID: 1284
		public const float MIN_WAVE_AGE = 0.5f;

		// Token: 0x04000505 RID: 1285
		public const float MAX_WAVE_AGE = 1f;

		// Token: 0x04000506 RID: 1286
		public const float MAX_WAVE_SPEED = 10f;

		// Token: 0x04000507 RID: 1287
		public const float MIN_GRID_SCALE = 0.01f;

		// Token: 0x04000508 RID: 1288
		public const float MAX_GRID_SCALE = 1f;

		// Token: 0x04000509 RID: 1289
		public const float MAX_WAVE_SMOOTHING = 6f;

		// Token: 0x0400050A RID: 1290
		public const float MIN_WAVE_SMOOTHING = 1f;

		// Token: 0x0400050B RID: 1291
		public const float MAX_SLOPE_SMOOTHING = 6f;

		// Token: 0x0400050C RID: 1292
		public const float MIN_SLOPE_SMOOTHING = 1f;

		// Token: 0x0400050D RID: 1293
		public const float MAX_FOAM_SMOOTHING = 6f;

		// Token: 0x0400050E RID: 1294
		public const float MIN_FOAM_SMOOTHING = 1f;

		// Token: 0x0400050F RID: 1295
		public FOURIER_SIZE fourierSize = FOURIER_SIZE.MEDIUM_64_CPU;

		// Token: 0x04000510 RID: 1296
		public SPECTRUM_TYPE spectrumType;

		// Token: 0x04000511 RID: 1297
		[Range(1f, 4f)]
		public int numberOfGrids = 4;

		// Token: 0x04000512 RID: 1298
		public bool disableReadBack = true;

		// Token: 0x04000513 RID: 1299
		public bool disableDisplacements;

		// Token: 0x04000514 RID: 1300
		public bool disableSlopes;

		// Token: 0x04000515 RID: 1301
		public bool disableFoam;

		// Token: 0x04000516 RID: 1302
		public bool textureFoam = true;

		// Token: 0x04000517 RID: 1303
		[Range(0f, 1.2f)]
		public float choppyness = 0.8f;

		// Token: 0x04000518 RID: 1304
		[Range(0f, 6f)]
		public float foamAmount = 1f;

		// Token: 0x04000519 RID: 1305
		[Range(0f, 0.5f)]
		public float foamCoverage = 0.1f;

		// Token: 0x0400051A RID: 1306
		[Range(0f, 30f)]
		public float windSpeed = 8f;

		// Token: 0x0400051B RID: 1307
		[Range(0.5f, 1f)]
		public float waveAge = 0.64f;

		// Token: 0x0400051C RID: 1308
		[Range(0f, 10f)]
		public float waveSpeed = 1f;

		// Token: 0x0400051D RID: 1309
		[Range(0.01f, 1f)]
		public float gridScale = 0.5f;

		// Token: 0x0400051E RID: 1310
		[Range(1f, 6f)]
		public float waveSmoothing = 2f;

		// Token: 0x0400051F RID: 1311
		[Range(1f, 6f)]
		private float slopeSmoothing = 2f;

		// Token: 0x04000520 RID: 1312
		[Range(1f, 6f)]
		private float foamSmoothing = 2f;

		// Token: 0x04000521 RID: 1313
		private RenderTexture[] m_displacementMaps;

		// Token: 0x04000522 RID: 1314
		private RenderTexture[] m_slopeMaps;

		// Token: 0x04000523 RID: 1315
		private RenderTexture[] m_foamMaps;

		// Token: 0x04000524 RID: 1316
		private Material m_slopeCopyMat;

		// Token: 0x04000525 RID: 1317
		private Material m_displacementCopyMat;

		// Token: 0x04000526 RID: 1318
		private Material m_foamCopyMat;

		// Token: 0x04000527 RID: 1319
		private Material m_slopeInitMat;

		// Token: 0x04000528 RID: 1320
		private Material m_displacementInitMat;

		// Token: 0x04000529 RID: 1321
		private Material m_foamInitMat;

		// Token: 0x0400052A RID: 1322
		private Vector4 m_gridSizes = Vector4.one;

		// Token: 0x0400052B RID: 1323
		private Vector4 m_choppyness = Vector4.one;

		// Token: 0x0400052C RID: 1324
		private Scheduler m_scheduler;

		// Token: 0x0400052D RID: 1325
		private WaveSpectrumCondition[] m_conditions;

		// Token: 0x0400052E RID: 1326
		private WaveSpectrumBuffer m_displacementBuffer;

		// Token: 0x0400052F RID: 1327
		private WaveSpectrumBuffer m_slopeBuffer;

		// Token: 0x04000530 RID: 1328
		private WaveSpectrumBuffer m_jacobianBuffer;

		// Token: 0x04000531 RID: 1329
		private FindRangeTask m_findRangeTask;

		// Token: 0x04000532 RID: 1330
		private ComputeBuffer m_readBuffer;

		// Token: 0x04000533 RID: 1331
		private QueryGridScaling m_queryScaling = new QueryGridScaling();

		// Token: 0x04000534 RID: 1332
		private WaveSpectrum.BufferSettings m_bufferSettings = default(WaveSpectrum.BufferSettings);

		// Token: 0x04000535 RID: 1333
		private DictionaryQueue<WaveSpectrumConditionKey, WaveSpectrumCondition> m_conditionCache;

		// Token: 0x04000536 RID: 1334
		private int m_maxConditionCacheSize = 10;

		// Token: 0x04000537 RID: 1335
		[HideInInspector]
		public Shader initSlopeSdr;

		// Token: 0x04000538 RID: 1336
		[HideInInspector]
		public Shader initDisplacementSdr;

		// Token: 0x04000539 RID: 1337
		[HideInInspector]
		public Shader initJacobianSdr;

		// Token: 0x0400053A RID: 1338
		[HideInInspector]
		public Shader fourierSdr;

		// Token: 0x0400053B RID: 1339
		[HideInInspector]
		public Shader slopeCopySdr;

		// Token: 0x0400053C RID: 1340
		[HideInInspector]
		public Shader displacementCopySdr;

		// Token: 0x0400053D RID: 1341
		[HideInInspector]
		public Shader foamCopySdr;

		// Token: 0x0400053E RID: 1342
		[HideInInspector]
		public ComputeShader readSdr;

		// Token: 0x020000B9 RID: 185
		private struct BufferSettings
		{
			// Token: 0x04000540 RID: 1344
			public bool beenCreated;

			// Token: 0x04000541 RID: 1345
			public bool isCpu;

			// Token: 0x04000542 RID: 1346
			public int size;
		}
	}
}
