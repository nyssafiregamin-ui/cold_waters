using System;
using System.Collections.Generic;
using Ceto.Common.Threading.Scheduling;
using Ceto.Common.Unity.Utility;
using UnityEngine;

namespace Ceto
{
	// Token: 0x02000065 RID: 101
	[AddComponentMenu("Ceto/Components/Ocean")]
	[DisallowMultipleComponent]
	public class Ocean : MonoBehaviour
	{
		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000307 RID: 775 RVA: 0x00011788 File Offset: 0x0000F988
		// (set) Token: 0x06000308 RID: 776 RVA: 0x00011790 File Offset: 0x0000F990
		public static Ocean Instance { get; private set; }

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000309 RID: 777 RVA: 0x00011798 File Offset: 0x0000F998
		public static float MAX_WAVE_HEIGHT
		{
			get
			{
				return 60f;
			}
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x0600030A RID: 778 RVA: 0x000117A0 File Offset: 0x0000F9A0
		public bool ProjectSceneView
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x0600030B RID: 779 RVA: 0x000117A4 File Offset: 0x0000F9A4
		// (set) Token: 0x0600030C RID: 780 RVA: 0x000117AC File Offset: 0x0000F9AC
		public Vector3 WindDirVector { get; private set; }

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x0600030D RID: 781 RVA: 0x000117B8 File Offset: 0x0000F9B8
		// (set) Token: 0x0600030E RID: 782 RVA: 0x000117C0 File Offset: 0x0000F9C0
		public OceanGridBase Grid { get; set; }

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x0600030F RID: 783 RVA: 0x000117CC File Offset: 0x0000F9CC
		// (set) Token: 0x06000310 RID: 784 RVA: 0x000117D4 File Offset: 0x0000F9D4
		public ReflectionBase Reflection { get; set; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000311 RID: 785 RVA: 0x000117E0 File Offset: 0x0000F9E0
		// (set) Token: 0x06000312 RID: 786 RVA: 0x000117E8 File Offset: 0x0000F9E8
		public WaveSpectrumBase Spectrum { get; set; }

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000313 RID: 787 RVA: 0x000117F4 File Offset: 0x0000F9F4
		// (set) Token: 0x06000314 RID: 788 RVA: 0x000117FC File Offset: 0x0000F9FC
		public UnderWaterBase UnderWater { get; set; }

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000315 RID: 789 RVA: 0x00011808 File Offset: 0x0000FA08
		// (set) Token: 0x06000316 RID: 790 RVA: 0x00011810 File Offset: 0x0000FA10
		public IOceanTime OceanTime { get; set; }

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000317 RID: 791 RVA: 0x0001181C File Offset: 0x0000FA1C
		// (set) Token: 0x06000318 RID: 792 RVA: 0x00011824 File Offset: 0x0000FA24
		public IProjection Projection { get; private set; }

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000319 RID: 793 RVA: 0x00011830 File Offset: 0x0000FA30
		// (set) Token: 0x0600031A RID: 794 RVA: 0x00011838 File Offset: 0x0000FA38
		public Vector3 PositionOffset
		{
			get
			{
				return this.m_positionOffset;
			}
			set
			{
				this.m_positionOffset = value;
				Shader.SetGlobalVector("Ceto_PosOffset", this.m_positionOffset);
			}
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x0600031B RID: 795 RVA: 0x00011858 File Offset: 0x0000FA58
		public int CameraCount
		{
			get
			{
				return this.m_cameraData.Count;
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x0600031C RID: 796 RVA: 0x00011868 File Offset: 0x0000FA68
		// (set) Token: 0x0600031D RID: 797 RVA: 0x00011870 File Offset: 0x0000FA70
		public OverlayManager OverlayManager { get; private set; }

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x0600031E RID: 798 RVA: 0x0001187C File Offset: 0x0000FA7C
		// (set) Token: 0x0600031F RID: 799 RVA: 0x00011884 File Offset: 0x0000FA84
		public bool WasError { get; private set; }

		// Token: 0x06000320 RID: 800 RVA: 0x00011890 File Offset: 0x0000FA90
		private void Awake()
		{
			try
			{
				if (Ocean.Instance != null)
				{
					throw new InvalidOperationException("There can only be one ocean instance.");
				}
				Ocean.Instance = this;
				this.WindDirVector = this.CalculateWindDirVector();
				if (this.doublePrecisionProjection)
				{
					this.Projection = new Projection3d(this);
				}
				else
				{
					this.Projection = new Projection3f(this);
				}
				this.OceanTime = new OceanTime();
				this.m_waveOverlayMat = new Material(this.waveOverlaySdr);
				this.OverlayManager = new OverlayManager(this.m_waveOverlayMat);
				this.m_scheduler = new Scheduler();
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				this.DisableOcean();
			}
		}

		// Token: 0x06000321 RID: 801 RVA: 0x00011964 File Offset: 0x0000FB64
		private void Start()
		{
			try
			{
				Matrix4x4 identity = Matrix4x4.identity;
				identity.m00 = 2f;
				identity.m03 = -1f;
				identity.m11 = 2f;
				identity.m13 = -1f;
				for (int i = 0; i < 4; i++)
				{
					identity[1, i] = -identity[1, i];
				}
				Shader.SetGlobalMatrix("Ceto_T2S", identity);
				Shader.SetGlobalTexture("Ceto_Overlay_NormalMap", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_Overlay_HeightMap", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_Overlay_FoamMap", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_Overlay_ClipMap", Texture2D.blackTexture);
				Shader.SetGlobalTexture(Ocean.REFRACTION_GRAB_TEXTURE_NAME, Texture2D.blackTexture);
				Shader.SetGlobalTexture(Ocean.DEPTH_GRAB_TEXTURE_NAME, Texture2D.whiteTexture);
				Shader.SetGlobalTexture("Ceto_OceanMask", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_OceanDepth", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_SlopeMap0", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_SlopeMap1", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_DisplacementMap0", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_DisplacementMap1", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_DisplacementMap2", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_DisplacementMap3", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_FoamMap0", Texture2D.blackTexture);
				Shader.SetGlobalTexture("Ceto_FoamMap1", Texture2D.blackTexture);
				Shader.SetGlobalVector("Ceto_GridSizes", Vector4.one);
				Shader.SetGlobalVector("Ceto_GridScale", Vector4.one);
				Shader.SetGlobalVector("Ceto_Choppyness", Vector4.one);
				Shader.SetGlobalFloat("Ceto_MapSize", 1f);
				Shader.SetGlobalColor("Ceto_FoamTint", Color.white);
				Shader.SetGlobalTexture("Ceto_FoamTexture0", Texture2D.whiteTexture);
				Shader.SetGlobalTexture("Ceto_FoamTexture2", Texture2D.whiteTexture);
				Shader.SetGlobalVector("Ceto_FoamTextureScale0", Vector4.one);
				Shader.SetGlobalVector("Ceto_FoamTextureScale1", Vector4.one);
				Shader.SetGlobalFloat("Ceto_MaxWaveHeight", 40f);
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				this.DisableOcean();
			}
		}

		// Token: 0x06000322 RID: 802 RVA: 0x00011B98 File Offset: 0x0000FD98
		private void OnEnable()
		{
			if (this.WasError)
			{
				this.DisableOcean();
			}
			Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(this.OceanOnPostRender));
		}

		// Token: 0x06000323 RID: 803 RVA: 0x00011BCC File Offset: 0x0000FDCC
		private void OnDisable()
		{
			if (!this.WasError)
			{
				base.enabled = true;
			}
			Camera.onPostRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPostRender, new Camera.CameraCallback(this.OceanOnPostRender));
		}

		// Token: 0x06000324 RID: 804 RVA: 0x00011C0C File Offset: 0x0000FE0C
		private void DisableOcean()
		{
			this.WasError = true;
			base.enabled = false;
			base.gameObject.AddComponent<DisableGameObject>();
		}

		// Token: 0x06000325 RID: 805 RVA: 0x00011C34 File Offset: 0x0000FE34
		private void Update()
		{
			try
			{
				this.WindDirVector = this.CalculateWindDirVector();
				this.UpdateOceanScheduler();
				this.OverlayManager.Update();
				this.specularRoughness = Mathf.Clamp01(this.specularRoughness);
				this.specularIntensity = Mathf.Max(0f, this.specularIntensity);
				this.minFresnel = Mathf.Clamp01(this.minFresnel);
				this.fresnelPower = Mathf.Max(0f, this.fresnelPower);
				float value = Mathf.Lerp(2E-05f, 0.02f, this.specularRoughness);
				Shader.SetGlobalColor("Ceto_DefaultSkyColor", this.defaultSkyColor);
				Shader.SetGlobalColor("Ceto_DefaultOceanColor", this.defaultOceanColor);
				Shader.SetGlobalFloat("Ceto_SpecularRoughness", value);
				Shader.SetGlobalFloat("Ceto_FresnelPower", this.fresnelPower);
				Shader.SetGlobalFloat("Ceto_SpecularIntensity", this.specularIntensity);
				Shader.SetGlobalFloat("Ceto_MinFresnel", this.minFresnel);
				Shader.SetGlobalFloat("Ceto_OceanLevel", this.level);
				Shader.SetGlobalFloat("Ceto_MaxWaveHeight", 40f);
				Shader.SetGlobalColor("Ceto_FoamTint", this.foamTint * this.foamIntensity);
				Shader.SetGlobalVector("Ceto_SunDir", this.SunDir());
				Shader.SetGlobalVector("Ceto_SunColor", this.SunColor());
				Vector4 vector = default(Vector4);
				vector.x = this.foamTexture0.scale.x;
				vector.y = this.foamTexture0.scale.y;
				vector.z = this.foamTexture0.scrollSpeed * this.OceanTime.Now;
				vector.w = 0f;
				Vector4 vector2 = default(Vector4);
				vector2.x = this.foamTexture1.scale.x;
				vector2.y = this.foamTexture1.scale.y;
				vector2.z = this.foamTexture1.scrollSpeed * this.OceanTime.Now;
				vector2.w = 0f;
				Shader.SetGlobalTexture("Ceto_FoamTexture0", (!(this.foamTexture0.tex != null)) ? Texture2D.whiteTexture : this.foamTexture0.tex);
				Shader.SetGlobalVector("Ceto_FoamTextureScale0", new Vector4(1f / vector.x, 1f / vector.y, vector.z, vector.w));
				Shader.SetGlobalTexture("Ceto_FoamTexture1", (!(this.foamTexture1.tex != null)) ? Texture2D.whiteTexture : this.foamTexture1.tex);
				Shader.SetGlobalVector("Ceto_FoamTextureScale1", new Vector4(1f / vector2.x, 1f / vector2.y, vector2.z, vector2.w));
				foreach (KeyValuePair<Camera, CameraData> keyValuePair in this.m_cameraData)
				{
					CameraData value2 = keyValuePair.Value;
					if (value2.mask != null)
					{
						value2.mask.updated = false;
					}
					if (value2.depth != null)
					{
						value2.depth.updated = false;
					}
					if (value2.overlay != null)
					{
						value2.overlay.updated = false;
					}
					if (value2.reflection != null)
					{
						value2.reflection.updated = false;
					}
					if (value2.projection != null)
					{
						value2.projection.updated = false;
					}
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				this.DisableOcean();
			}
		}

		// Token: 0x06000326 RID: 806 RVA: 0x00012000 File Offset: 0x00010200
		private void OnDestroy()
		{
			try
			{
				Ocean.Instance = null;
				if (this.OverlayManager != null)
				{
					this.OverlayManager.Release();
				}
				if (this.m_scheduler != null)
				{
					this.m_scheduler.ShutingDown = true;
					this.m_scheduler.CancelAllTasks();
				}
				List<Camera> list = new List<Camera>(this.m_cameraData.Keys);
				foreach (Camera cam in list)
				{
					this.RemoveCameraData(cam);
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				this.DisableOcean();
			}
		}

		// Token: 0x06000327 RID: 807 RVA: 0x000120E4 File Offset: 0x000102E4
		public void Register(OceanComponent component)
		{
			if (component is OceanGridBase)
			{
				this.Grid = (component as OceanGridBase);
			}
			else if (component is WaveSpectrumBase)
			{
				this.Spectrum = (component as WaveSpectrumBase);
			}
			else if (component is ReflectionBase)
			{
				this.Reflection = (component as ReflectionBase);
			}
			else
			{
				if (!(component is UnderWaterBase))
				{
					throw new InvalidCastException("Could not cast ocean component " + component.GetType());
				}
				this.UnderWater = (component as UnderWaterBase);
			}
		}

		// Token: 0x06000328 RID: 808 RVA: 0x00012178 File Offset: 0x00010378
		public void Deregister(OceanComponent component)
		{
			if (component is OceanGridBase)
			{
				this.Grid = null;
			}
			else if (component is WaveSpectrumBase)
			{
				this.Spectrum = null;
			}
			else if (component is ReflectionBase)
			{
				this.Reflection = null;
			}
			else
			{
				if (!(component is UnderWaterBase))
				{
					throw new InvalidCastException("Could not cast ocean component " + component.GetType());
				}
				this.UnderWater = null;
			}
		}

		// Token: 0x06000329 RID: 809 RVA: 0x000121F8 File Offset: 0x000103F8
		public static void LogError(string msg)
		{
			Debug.Log("<color=red>Ceto (" + Ocean.VERSION + ") Error:</color> " + msg);
		}

		// Token: 0x0600032A RID: 810 RVA: 0x00012214 File Offset: 0x00010414
		public static void LogWarning(string msg)
		{
			if (Ocean.Instance != null && Ocean.Instance.disableWarnings)
			{
				return;
			}
			Debug.Log("<color=yellow>Ceto (" + Ocean.VERSION + ") Warning:</color> " + msg);
		}

		// Token: 0x0600032B RID: 811 RVA: 0x0001225C File Offset: 0x0001045C
		public static void LogInfo(string msg)
		{
			Debug.Log("<color=cyan>Ceto (" + Ocean.VERSION + ") Info:</color> " + msg);
		}

		// Token: 0x0600032C RID: 812 RVA: 0x00012278 File Offset: 0x00010478
		private void UpdateOceanScheduler()
		{
			try
			{
				if (this.m_scheduler != null)
				{
					this.m_scheduler.DisableMultithreading = Ocean.DISABLE_ALL_MULTITHREADING;
					this.m_scheduler.CheckForException();
					this.m_scheduler.Update();
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				this.DisableOcean();
			}
		}

		// Token: 0x0600032D RID: 813 RVA: 0x000122F4 File Offset: 0x000104F4
		public Vector3 CalculateWindDirVector()
		{
			float f = this.windDir * 3.1415927f / 180f;
			float x = -Mathf.Cos(f);
			float z = Mathf.Sin(f);
			Vector3 vector = new Vector3(x, 0f, z);
			return vector.normalized;
		}

		// Token: 0x0600032E RID: 814 RVA: 0x00012338 File Offset: 0x00010538
		public Vector3 SunDir()
		{
			if (this.m_sun == null || this.m_sun.GetComponent<Light>() == null)
			{
				return Vector3.up;
			}
			return this.m_sun.transform.forward * -1f;
		}

		// Token: 0x0600032F RID: 815 RVA: 0x0001238C File Offset: 0x0001058C
		public Color SunColor()
		{
			if (this.m_sun == null || this.m_sun.GetComponent<Light>() == null)
			{
				return Color.white;
			}
			return this.m_sun.GetComponent<Light>().color;
		}

		// Token: 0x06000330 RID: 816 RVA: 0x000123D8 File Offset: 0x000105D8
		public CameraData FindCameraData(Camera cam)
		{
			if (cam == null)
			{
				throw new InvalidOperationException("Can not find camera data for null camera");
			}
			if (!this.m_cameraData.ContainsKey(cam))
			{
				this.m_cameraData.Add(cam, new CameraData());
			}
			CameraData cameraData = this.m_cameraData[cam];
			cameraData.settings = cam.GetComponent<OceanCameraSettings>();
			return cameraData;
		}

		// Token: 0x06000331 RID: 817 RVA: 0x00012438 File Offset: 0x00010638
		public void RemoveCameraData(Camera cam)
		{
			if (!this.m_cameraData.ContainsKey(cam))
			{
				return;
			}
			CameraData cameraData = this.m_cameraData[cam];
			if (cameraData.overlay != null)
			{
				this.OverlayManager.DestroyBuffers(cameraData.overlay);
			}
			if (cameraData.reflection != null)
			{
				RTUtility.ReleaseAndDestroy(cameraData.reflection.tex);
				cameraData.reflection.tex = null;
				if (cameraData.reflection.cam != null)
				{
					RTUtility.ReleaseAndDestroy(cameraData.reflection.cam.targetTexture);
					cameraData.reflection.cam.targetTexture = null;
					UnityEngine.Object.Destroy(cameraData.reflection.cam.gameObject);
					UnityEngine.Object.Destroy(cameraData.reflection.cam);
					cameraData.reflection.cam = null;
				}
			}
			if (cameraData.depth != null && cameraData.depth.cam != null)
			{
				RTUtility.ReleaseAndDestroy(cameraData.depth.cam.targetTexture);
				cameraData.depth.cam.targetTexture = null;
				UnityEngine.Object.Destroy(cameraData.depth.cam.gameObject);
				UnityEngine.Object.Destroy(cameraData.depth.cam);
				cameraData.depth.cam = null;
			}
			if (cameraData.mask != null && cameraData.mask.cam != null)
			{
				RTUtility.ReleaseAndDestroy(cameraData.mask.cam.targetTexture);
				cameraData.mask.cam.targetTexture = null;
				UnityEngine.Object.Destroy(cameraData.mask.cam.gameObject);
				UnityEngine.Object.Destroy(cameraData.mask.cam);
				cameraData.mask.cam = null;
			}
			this.m_cameraData.Remove(cam);
		}

		// Token: 0x06000332 RID: 818 RVA: 0x00012614 File Offset: 0x00010814
		public float FindMaxDisplacement(bool inculdeOverlays)
		{
			float num = 0f;
			if (this.Spectrum != null)
			{
				num += this.Spectrum.MaxDisplacement.y;
			}
			if (inculdeOverlays && this.OverlayManager != null)
			{
				num += this.OverlayManager.MaxDisplacement;
			}
			return Mathf.Max(0f, num);
		}

		// Token: 0x06000333 RID: 819 RVA: 0x0001267C File Offset: 0x0001087C
		public void SetQueryWavesSampling(bool overlays, bool grid0, bool grid1, bool grid2, bool grid3)
		{
			this.m_query.sampleOverlay = overlays;
			this.m_query.sampleSpectrum[0] = grid0;
			this.m_query.sampleSpectrum[1] = grid1;
			this.m_query.sampleSpectrum[2] = grid2;
			this.m_query.sampleSpectrum[3] = grid3;
		}

		// Token: 0x06000334 RID: 820 RVA: 0x000126D0 File Offset: 0x000108D0
		public Vector3 QueryNormal(float x, float z)
		{
			float num = 0.5f;
			float num2 = this.QueryWaves(x - num, z);
			float num3 = this.QueryWaves(x, z - num);
			float num4 = this.QueryWaves(x + num, z);
			float num5 = this.QueryWaves(x, z + num);
			float num6 = num2 - num4;
			float num7 = num3 - num5;
			Vector3 result = new Vector3(num6, Mathf.Sqrt(1f - num6 * num6 - num7 * num7), num7);
			return result;
		}

		// Token: 0x06000335 RID: 821 RVA: 0x00012740 File Offset: 0x00010940
		public float QueryWaves(float x, float z)
		{
			this.m_query.result.Clear();
			this.m_query.posX = x;
			this.m_query.posZ = z;
			if (base.enabled)
			{
				if (this.Spectrum != null)
				{
					this.Spectrum.QueryWaves(this.m_query);
				}
				if (this.OverlayManager != null)
				{
					this.OverlayManager.QueryWaves(this.m_query);
				}
			}
			return this.m_query.result.height + this.level;
		}

		// Token: 0x06000336 RID: 822 RVA: 0x000127D8 File Offset: 0x000109D8
		public float QueryWaves(float x, float z, out bool isClipped)
		{
			this.m_query.result.Clear();
			this.m_query.posX = x;
			this.m_query.posZ = z;
			if (base.enabled)
			{
				if (this.Spectrum != null)
				{
					this.Spectrum.QueryWaves(this.m_query);
				}
				if (this.OverlayManager != null)
				{
					this.OverlayManager.QueryWaves(this.m_query);
				}
			}
			isClipped = this.m_query.result.isClipped;
			return this.m_query.result.height + this.level;
		}

		// Token: 0x06000337 RID: 823 RVA: 0x00012880 File Offset: 0x00010A80
		public void QueryWaves(WaveQuery query)
		{
			query.result.Clear();
			if (base.enabled)
			{
				if (this.Spectrum != null)
				{
					this.Spectrum.QueryWaves(query);
				}
				if (this.OverlayManager != null)
				{
					this.OverlayManager.QueryWaves(query);
				}
			}
			query.result.height = query.result.height + this.level;
		}

		// Token: 0x06000338 RID: 824 RVA: 0x000128F0 File Offset: 0x00010AF0
		public void QueryWaves(IEnumerable<WaveQuery> querys)
		{
			foreach (WaveQuery waveQuery in querys)
			{
				waveQuery.result.Clear();
				if (base.enabled)
				{
					if (this.Spectrum != null)
					{
						this.Spectrum.QueryWaves(waveQuery);
					}
					if (this.OverlayManager != null)
					{
						this.OverlayManager.QueryWaves(waveQuery);
					}
				}
				WaveQuery waveQuery2 = waveQuery;
				waveQuery2.result.height = waveQuery2.result.height + this.level;
			}
		}

		// Token: 0x06000339 RID: 825 RVA: 0x000129AC File Offset: 0x00010BAC
		public void QueryWavesAsync(IEnumerable<WaveQuery> querys, Action<IEnumerable<WaveQuery>> callBack)
		{
			IDisplacementBuffer displacementBuffer = this.Spectrum.DisplacementBuffer;
			if (base.enabled && this.Spectrum != null && displacementBuffer != null && (!displacementBuffer.IsGPU || !this.Spectrum.DisableReadBack))
			{
				WaveQueryTask task = new WaveQueryTask(this.Spectrum, this.level, this.PositionOffset, querys, callBack);
				this.m_scheduler.Run(task);
			}
			else
			{
				foreach (WaveQuery waveQuery in querys)
				{
					waveQuery.result.Clear();
					waveQuery.result.height = this.level;
				}
				callBack(querys);
			}
		}

		// Token: 0x0600033A RID: 826 RVA: 0x00012A9C File Offset: 0x00010C9C
		private void OceanOnPreRender(Camera cam)
		{
			if (cam.GetComponent<IgnoreOceanEvents>() != null)
			{
				return;
			}
			CameraData data = this.FindCameraData(cam);
			if (this.Grid != null)
			{
				this.Grid.OceanOnPreRender(cam, data);
			}
			if (this.Spectrum != null)
			{
				this.Spectrum.OceanOnPreRender(cam, data);
			}
			if (this.Reflection != null)
			{
				this.Reflection.OceanOnPreRender(cam, data);
			}
			if (this.UnderWater != null)
			{
				this.UnderWater.OceanOnPreRender(cam, data);
			}
		}

		// Token: 0x0600033B RID: 827 RVA: 0x00012B3C File Offset: 0x00010D3C
		private void OceanOnPreCull(Camera cam)
		{
			if (cam.GetComponent<IgnoreOceanEvents>() != null)
			{
				return;
			}
			CameraData data = this.FindCameraData(cam);
			if (this.Grid != null)
			{
				this.Grid.OceanOnPreCull(cam, data);
			}
			if (this.Spectrum != null)
			{
				this.Spectrum.OceanOnPreCull(cam, data);
			}
			if (this.Reflection != null)
			{
				this.Reflection.OceanOnPreCull(cam, data);
			}
			if (this.UnderWater != null)
			{
				this.UnderWater.OceanOnPreCull(cam, data);
			}
		}

		// Token: 0x0600033C RID: 828 RVA: 0x00012BDC File Offset: 0x00010DDC
		private void OceanOnPostRender(Camera cam)
		{
			if (cam.GetComponent<IgnoreOceanEvents>() != null)
			{
				return;
			}
			CameraData data = this.FindCameraData(cam);
			if (this.Grid != null)
			{
				this.Grid.OceanOnPostRender(cam, data);
			}
			if (this.Spectrum != null)
			{
				this.Spectrum.OceanOnPostRender(cam, data);
			}
			if (this.Reflection != null)
			{
				this.Reflection.OceanOnPostRender(cam, data);
			}
			if (this.UnderWater != null)
			{
				this.UnderWater.OceanOnPostRender(cam, data);
			}
		}

		// Token: 0x0600033D RID: 829 RVA: 0x00012C7C File Offset: 0x00010E7C
		private bool GetDisableAllOverlays(OceanCameraSettings settings)
		{
			return settings != null && settings.disableAllOverlays;
		}

		// Token: 0x0600033E RID: 830 RVA: 0x00012C98 File Offset: 0x00010E98
		public void RenderWaveOverlays(GameObject go)
		{
			try
			{
				if (base.enabled)
				{
					Camera current = Camera.current;
					if (!this.m_cameraData.ContainsKey(current))
					{
						this.m_cameraData.Add(current, new CameraData());
					}
					CameraData cameraData = this.m_cameraData[current];
					if (cameraData.overlay == null)
					{
						cameraData.overlay = new WaveOverlayData();
					}
					if (cameraData.projection == null)
					{
						cameraData.projection = new ProjectionData();
					}
					if (!cameraData.overlay.updated)
					{
						if (!cameraData.projection.updated)
						{
							this.Projection.UpdateProjection(current, cameraData, this.ProjectSceneView);
							Shader.SetGlobalMatrix("Ceto_Interpolation", cameraData.projection.interpolation);
							Shader.SetGlobalMatrix("Ceto_ProjectorVP", cameraData.projection.projectorVP);
						}
						if (this.GetDisableAllOverlays(cameraData.settings))
						{
							this.OverlayManager.DestroyBuffers(cameraData.overlay);
							Shader.SetGlobalTexture("Ceto_Overlay_NormalMap", Texture2D.blackTexture);
							Shader.SetGlobalTexture("Ceto_Overlay_HeightMap", Texture2D.blackTexture);
							Shader.SetGlobalTexture("Ceto_Overlay_FoamMap", Texture2D.blackTexture);
							Shader.SetGlobalTexture("Ceto_Overlay_ClipMap", Texture2D.blackTexture);
						}
						else
						{
							OVERLAY_MAP_SIZE overlay_MAP_SIZE = (!(cameraData.settings != null)) ? this.normalOverlaySize : cameraData.settings.normalOverlaySize;
							OVERLAY_MAP_SIZE overlay_MAP_SIZE2 = (!(cameraData.settings != null)) ? this.heightOverlaySize : cameraData.settings.heightOverlaySize;
							OVERLAY_MAP_SIZE overlay_MAP_SIZE3 = (!(cameraData.settings != null)) ? this.foamOverlaySize : cameraData.settings.foamOverlaySize;
							OVERLAY_MAP_SIZE overlay_MAP_SIZE4 = (!(cameraData.settings != null)) ? this.clipOverlaySize : cameraData.settings.clipOverlaySize;
							this.OverlayManager.CreateOverlays(current, cameraData.overlay, overlay_MAP_SIZE, overlay_MAP_SIZE2, overlay_MAP_SIZE3, overlay_MAP_SIZE4);
							this.OverlayManager.RenderWaveOverlays(current, cameraData.overlay);
						}
						cameraData.overlay.updated = true;
					}
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				this.DisableOcean();
			}
		}

		// Token: 0x0600033F RID: 831 RVA: 0x00012EF0 File Offset: 0x000110F0
		public void RenderReflection(GameObject go)
		{
			if (!base.enabled || this.Reflection == null)
			{
				return;
			}
			this.Reflection.RenderReflection(go);
		}

		// Token: 0x06000340 RID: 832 RVA: 0x00012F28 File Offset: 0x00011128
		public void RenderOceanMask(GameObject go)
		{
			if (!base.enabled || this.UnderWater == null)
			{
				return;
			}
			this.UnderWater.RenderOceanMask(go);
		}

		// Token: 0x06000341 RID: 833 RVA: 0x00012F60 File Offset: 0x00011160
		public void RenderOceanDepth(GameObject go)
		{
			if (!base.enabled || this.UnderWater == null)
			{
				return;
			}
			this.UnderWater.RenderOceanDepth(go);
		}

		// Token: 0x040002C2 RID: 706
		public const float MAX_SPECTRUM_WAVE_HEIGHT = 40f;

		// Token: 0x040002C3 RID: 707
		public const float MAX_OVERLAY_WAVE_HEIGHT = 20f;

		// Token: 0x040002C4 RID: 708
		public static readonly bool DISABLE_FOURIER_MULTITHREADING;

		// Token: 0x040002C5 RID: 709
		public static readonly bool DISABLE_PROCESS_DATA_MULTITHREADING;

		// Token: 0x040002C6 RID: 710
		public static readonly bool DISABLE_PROJECTED_GRID_BORDER;

		// Token: 0x040002C7 RID: 711
		public static readonly bool DISABLE_ALL_MULTITHREADING;

		// Token: 0x040002C8 RID: 712
		public static readonly string VERSION = "1.1.0";

		// Token: 0x040002C9 RID: 713
		public static string OCEAN_LAYER = "Water";

		// Token: 0x040002CA RID: 714
		public static readonly string REFLECTION_TEXTURE_NAME = "Ceto_Reflections";

		// Token: 0x040002CB RID: 715
		public static readonly string REFRACTION_GRAB_TEXTURE_NAME = "Ceto_RefractionGrab";

		// Token: 0x040002CC RID: 716
		public static readonly string DEPTH_GRAB_TEXTURE_NAME = "Ceto_DepthBuffer";

		// Token: 0x040002CD RID: 717
		public bool disableWarnings;

		// Token: 0x040002CE RID: 718
		public bool doublePrecisionProjection = true;

		// Token: 0x040002CF RID: 719
		public GameObject m_sun;

		// Token: 0x040002D0 RID: 720
		public float level;

		// Token: 0x040002D1 RID: 721
		public OVERLAY_MAP_SIZE heightOverlaySize = OVERLAY_MAP_SIZE.HALF;

		// Token: 0x040002D2 RID: 722
		public OVERLAY_MAP_SIZE normalOverlaySize = OVERLAY_MAP_SIZE.FULL;

		// Token: 0x040002D3 RID: 723
		public OVERLAY_MAP_SIZE foamOverlaySize = OVERLAY_MAP_SIZE.FULL;

		// Token: 0x040002D4 RID: 724
		public OVERLAY_MAP_SIZE clipOverlaySize = OVERLAY_MAP_SIZE.HALF;

		// Token: 0x040002D5 RID: 725
		public Color defaultSkyColor = new Color32(96, 147, 210, byte.MaxValue);

		// Token: 0x040002D6 RID: 726
		public Color defaultOceanColor = new Color32(0, 19, 30, byte.MaxValue);

		// Token: 0x040002D7 RID: 727
		[Range(0f, 360f)]
		public float windDir;

		// Token: 0x040002D8 RID: 728
		[Range(0f, 1f)]
		public float specularRoughness = 0.2f;

		// Token: 0x040002D9 RID: 729
		[Range(0f, 1f)]
		public float specularIntensity = 0.2f;

		// Token: 0x040002DA RID: 730
		[Range(0f, 10f)]
		public float fresnelPower = 5f;

		// Token: 0x040002DB RID: 731
		[Range(0f, 1f)]
		public float minFresnel = 0.02f;

		// Token: 0x040002DC RID: 732
		public Color foamTint = Color.white;

		// Token: 0x040002DD RID: 733
		[Range(0f, 3f)]
		public float foamIntensity = 1f;

		// Token: 0x040002DE RID: 734
		public FoamTexture foamTexture0;

		// Token: 0x040002DF RID: 735
		public FoamTexture foamTexture1;

		// Token: 0x040002E0 RID: 736
		[HideInInspector]
		public Shader waveOverlaySdr;

		// Token: 0x040002E1 RID: 737
		private Vector3 m_positionOffset;

		// Token: 0x040002E2 RID: 738
		private Dictionary<Camera, CameraData> m_cameraData = new Dictionary<Camera, CameraData>();

		// Token: 0x040002E3 RID: 739
		private Material m_waveOverlayMat;

		// Token: 0x040002E4 RID: 740
		private Scheduler m_scheduler;

		// Token: 0x040002E5 RID: 741
		private WaveQuery m_query = new WaveQuery(0f, 0f);
	}
}
