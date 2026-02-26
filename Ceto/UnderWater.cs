using System;
using Ceto.Common.Unity.Utility;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ceto
{
	// Token: 0x020000BF RID: 191
	[RequireComponent(typeof(Ocean))]
	[DisallowMultipleComponent]
	[AddComponentMenu("Ceto/Components/UnderWater")]
	public class UnderWater : UnderWaterBase
	{
		// Token: 0x1700012F RID: 303
		// (get) Token: 0x06000582 RID: 1410 RVA: 0x00023EB0 File Offset: 0x000220B0
		public override UNDERWATER_MODE Mode
		{
			get
			{
				return this.underwaterMode;
			}
		}

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x06000583 RID: 1411 RVA: 0x00023EB8 File Offset: 0x000220B8
		public override DEPTH_MODE DepthMode
		{
			get
			{
				return this.depthMode;
			}
		}

		// Token: 0x06000584 RID: 1412 RVA: 0x00023EC0 File Offset: 0x000220C0
		private void Start()
		{
			try
			{
				this.m_refractionCommand = new RefractionCommand(this.copyDepthSdr);
				Mesh mesh = this.CreateBottomMesh(32, 512);
				this.m_bottomMask = new GameObject("Ceto Bottom Mask Gameobject");
				MeshFilter meshFilter = this.m_bottomMask.AddComponent<MeshFilter>();
				MeshRenderer meshRenderer = this.m_bottomMask.AddComponent<MeshRenderer>();
				NotifyOnWillRender notifyOnWillRender = this.m_bottomMask.AddComponent<NotifyOnWillRender>();
				meshFilter.sharedMesh = mesh;
				meshRenderer.receiveShadows = false;
				meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
				meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
				meshRenderer.material = new Material(this.oceanBottomSdr);
				notifyOnWillRender.AddAction(new Action<GameObject>(this.m_ocean.RenderWaveOverlays));
				notifyOnWillRender.AddAction(new Action<GameObject>(this.m_ocean.RenderOceanMask));
				notifyOnWillRender.AddAction(new Action<GameObject>(this.m_ocean.RenderOceanDepth));
				this.m_bottomMask.layer = LayerMask.NameToLayer(Ocean.OCEAN_LAYER);
				this.m_bottomMask.hideFlags = HideFlags.HideAndDontSave;
				this.UpdateBottomBounds();
				UnityEngine.Object.Destroy(mesh);
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		// Token: 0x06000585 RID: 1413 RVA: 0x00024004 File Offset: 0x00022204
		protected override void OnEnable()
		{
			base.OnEnable();
			try
			{
				Shader.EnableKeyword("CETO_UNDERWATER_ON");
				this.SetBottomActive(this.m_bottomMask, true);
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		// Token: 0x06000586 RID: 1414 RVA: 0x00024070 File Offset: 0x00022270
		protected override void OnDisable()
		{
			base.OnDisable();
			try
			{
				Shader.DisableKeyword("CETO_UNDERWATER_ON");
				this.SetBottomActive(this.m_bottomMask, false);
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		// Token: 0x06000587 RID: 1415 RVA: 0x000240DC File Offset: 0x000222DC
		protected override void OnDestroy()
		{
			base.OnDestroy();
			try
			{
				if (this.m_bottomMask != null)
				{
					Mesh mesh = this.m_bottomMask.GetComponent<MeshFilter>().mesh;
					UnityEngine.Object.Destroy(this.m_bottomMask);
					UnityEngine.Object.Destroy(mesh);
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		// Token: 0x06000588 RID: 1416 RVA: 0x00024164 File Offset: 0x00022364
		private void Update()
		{
			try
			{
				Vector4 vector = new Vector4(this.absorptionR, this.absorptionG, this.absorptionB, 1f);
				Vector4 vec = vector;
				Vector4 vec2 = vector;
				vector.w = Mathf.Max(0f, this.aboveAbsorptionModifier.scale);
				vec.w = Mathf.Max(0f, this.subSurfaceScatterModifier.scale);
				vec2.w = Mathf.Max(0f, this.belowAbsorptionModifier.scale);
				Color c = this.aboveAbsorptionModifier.tint * Mathf.Max(0f, this.aboveAbsorptionModifier.intensity);
				Color c2 = this.subSurfaceScatterModifier.tint * Mathf.Max(0f, this.subSurfaceScatterModifier.intensity);
				Color c3 = this.belowAbsorptionModifier.tint * Mathf.Max(0f, this.belowAbsorptionModifier.intensity);
				Shader.SetGlobalVector("Ceto_AbsCof", vector);
				Shader.SetGlobalVector("Ceto_AbsTint", c);
				Shader.SetGlobalVector("Ceto_SSSCof", vec);
				Shader.SetGlobalVector("Ceto_SSSTint", c2);
				Shader.SetGlobalVector("Ceto_BelowCof", vec2);
				Shader.SetGlobalVector("Ceto_BelowTint", c3);
				Color color = this.aboveInscatterModifier.color;
				color.a = Mathf.Clamp01(this.aboveInscatterModifier.intensity);
				Shader.SetGlobalFloat("Ceto_AboveInscatterScale", Mathf.Max(0.1f, this.aboveInscatterModifier.scale));
				Shader.SetGlobalVector("Ceto_AboveInscatterMode", this.InscatterModeToMask(this.aboveInscatterModifier.mode));
				Shader.SetGlobalVector("Ceto_AboveInscatterColor", color);
				Color color2 = this.belowInscatterModifier.color;
				color2.a = Mathf.Clamp01(this.belowInscatterModifier.intensity);
				Shader.SetGlobalFloat("Ceto_BelowInscatterScale", Mathf.Max(0.1f, this.belowInscatterModifier.scale));
				Shader.SetGlobalVector("Ceto_BelowInscatterMode", this.InscatterModeToMask(this.belowInscatterModifier.mode));
				Shader.SetGlobalVector("Ceto_BelowInscatterColor", color2);
				Shader.SetGlobalFloat("Ceto_RefractionIntensity", Mathf.Max(0f, this.refractionIntensity));
				Shader.SetGlobalFloat("Ceto_RefractionDistortion", this.refractionDistortion * 0.05f);
				Shader.SetGlobalFloat("Ceto_MaxDepthDist", Mathf.Max(0f, this.MAX_DEPTH_DIST));
				Shader.SetGlobalFloat("Ceto_DepthBlend", Mathf.Clamp01(this.depthBlend));
				Shader.SetGlobalFloat("Ceto_EdgeFade", Mathf.Lerp(20f, 2f, Mathf.Clamp01(this.edgeFade)));
				if (this.depthMode == DEPTH_MODE.USE_OCEAN_DEPTH_PASS)
				{
					Shader.EnableKeyword("CETO_USE_OCEAN_DEPTHS_BUFFER");
					if (this.underwaterMode == UNDERWATER_MODE.ABOVE_ONLY)
					{
						this.SetBottomActive(this.m_bottomMask, false);
					}
					else
					{
						this.SetBottomActive(this.m_bottomMask, true);
						this.UpdateBottomBounds();
					}
				}
				else
				{
					Shader.DisableKeyword("CETO_USE_OCEAN_DEPTHS_BUFFER");
					if (this.underwaterMode == UNDERWATER_MODE.ABOVE_ONLY)
					{
						this.SetBottomActive(this.m_bottomMask, false);
					}
					else
					{
						this.SetBottomActive(this.m_bottomMask, true);
						this.UpdateBottomBounds();
					}
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		// Token: 0x06000589 RID: 1417 RVA: 0x000244E0 File Offset: 0x000226E0
		private void SetBottomActive(GameObject bottom, bool active)
		{
			if (bottom != null)
			{
				bottom.SetActive(active);
			}
		}

		// Token: 0x0600058A RID: 1418 RVA: 0x000244F8 File Offset: 0x000226F8
		private Vector3 InscatterModeToMask(UnderWater.INSCATTER_MODE mode)
		{
			switch (mode)
			{
			case UnderWater.INSCATTER_MODE.LINEAR:
				return new Vector3(1f, 0f, 0f);
			case UnderWater.INSCATTER_MODE.EXP:
				return new Vector3(0f, 1f, 0f);
			case UnderWater.INSCATTER_MODE.EXP2:
				return new Vector3(0f, 0f, 1f);
			default:
				return new Vector3(0f, 0f, 1f);
			}
		}

		// Token: 0x0600058B RID: 1419 RVA: 0x00024574 File Offset: 0x00022774
		private void FitBottomToCamera()
		{
			if (!base.enabled || this.m_bottomMask == null)
			{
				return;
			}
			Camera current = Camera.current;
			Vector3 position = current.transform.position;
			float num = current.farClipPlane * 0.85f;
			this.m_bottomMask.transform.localScale = new Vector3(num, this.OCEAN_BOTTOM_DEPTH, num);
			float num2 = 0f;
			this.m_bottomMask.transform.localPosition = new Vector3(position.x, -this.OCEAN_BOTTOM_DEPTH + this.m_ocean.level - num2, position.z);
		}

		// Token: 0x0600058C RID: 1420 RVA: 0x00024618 File Offset: 0x00022818
		private void UpdateBottomBounds()
		{
			float num = 100000000f;
			float level = this.m_ocean.level;
			if (this.m_bottomMask != null && this.m_bottomMask.activeSelf)
			{
				Bounds bounds = new Bounds(new Vector3(0f, level, 0f), new Vector3(num, this.OCEAN_BOTTOM_DEPTH, num));
				this.m_bottomMask.GetComponent<MeshFilter>().mesh.bounds = bounds;
			}
		}

		// Token: 0x0600058D RID: 1421 RVA: 0x00024694 File Offset: 0x00022894
		private LayerMask GetOceanDepthsLayermask(OceanCameraSettings settings)
		{
			return (!(settings != null)) ? this.oceanDepthsMask : settings.oceanDepthsMask;
		}

		// Token: 0x0600058E RID: 1422 RVA: 0x000246B4 File Offset: 0x000228B4
		private bool GetDisableUnderwater(OceanCameraSettings settings)
		{
			return settings != null && settings.disableUnderwater;
		}

		// Token: 0x0600058F RID: 1423 RVA: 0x000246D0 File Offset: 0x000228D0
		public override void RenderOceanMask(GameObject go)
		{
			try
			{
				if (base.enabled)
				{
					if (!(this.oceanMaskSdr == null))
					{
						if (this.underwaterMode != UNDERWATER_MODE.ABOVE_ONLY)
						{
							Camera current = Camera.current;
							if (!(current == null))
							{
								CameraData cameraData = this.m_ocean.FindCameraData(current);
								if (cameraData.mask == null)
								{
									cameraData.mask = new MaskData();
								}
								if (!cameraData.mask.updated)
								{
									if (current.name == "SceneCamera" || current.GetComponent<UnderWaterPostEffect>() == null || SystemInfo.graphicsShaderLevel < 30 || this.GetDisableUnderwater(cameraData.settings))
									{
										Shader.SetGlobalTexture("Ceto_OceanMask", Texture2D.blackTexture);
										cameraData.mask.updated = true;
									}
									else
									{
										this.CreateMaskCameraFor(current, cameraData.mask);
										this.FitBottomToCamera();
										NotifyOnEvent.Disable = true;
										cameraData.mask.cam.RenderWithShader(this.oceanMaskSdr, "OceanMask");
										NotifyOnEvent.Disable = false;
										Shader.SetGlobalTexture("Ceto_OceanMask", cameraData.mask.cam.targetTexture);
										cameraData.mask.updated = true;
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		// Token: 0x06000590 RID: 1424 RVA: 0x00024868 File Offset: 0x00022A68
		public override void RenderOceanDepth(GameObject go)
		{
			try
			{
				if (base.enabled)
				{
					Camera current = Camera.current;
					if (!(current == null))
					{
						CameraData cameraData = this.m_ocean.FindCameraData(current);
						if (cameraData.depth == null)
						{
							cameraData.depth = new DepthData();
						}
						if (!cameraData.depth.updated)
						{
							Shader.SetGlobalTexture(Ocean.REFRACTION_GRAB_TEXTURE_NAME, Texture2D.blackTexture);
							Shader.SetGlobalTexture(Ocean.DEPTH_GRAB_TEXTURE_NAME, Texture2D.whiteTexture);
							Shader.SetGlobalTexture("Ceto_OceanDepth", Texture2D.whiteTexture);
							if (this.GetDisableUnderwater(cameraData.settings))
							{
								Shader.DisableKeyword("CETO_UNDERWATER_ON");
								cameraData.depth.updated = true;
							}
							else
							{
								Shader.EnableKeyword("CETO_UNDERWATER_ON");
								if (current.name == "SceneCamera" || SystemInfo.graphicsShaderLevel < 30)
								{
									Shader.SetGlobalMatrix("Ceto_Camera_IVP", (current.projectionMatrix * current.worldToCameraMatrix).inverse);
									cameraData.depth.updated = true;
								}
								else if (this.depthMode == DEPTH_MODE.USE_DEPTH_BUFFER)
								{
									current.depthTextureMode |= DepthTextureMode.Depth;
									Shader.SetGlobalMatrix("Ceto_Camera_IVP", (current.projectionMatrix * current.worldToCameraMatrix).inverse);
									this.CreateRefractionGrab(current, cameraData.depth);
									cameraData.depth.updated = true;
								}
								else if (this.depthMode == DEPTH_MODE.USE_OCEAN_DEPTH_PASS)
								{
									this.CreateDepthCameraFor(current, cameraData.depth);
									this.CreateRefractionGrab(current, cameraData.depth);
									cameraData.depth.cam.cullingMask = this.GetOceanDepthsLayermask(cameraData.settings);
									cameraData.depth.cam.cullingMask = OceanUtility.HideLayer(cameraData.depth.cam.cullingMask, Ocean.OCEAN_LAYER);
									NotifyOnEvent.Disable = true;
									cameraData.depth.cam.RenderWithShader(this.oceanDepthSdr, "RenderType");
									NotifyOnEvent.Disable = false;
									Shader.SetGlobalTexture("Ceto_OceanDepth", cameraData.depth.cam.targetTexture);
									cameraData.depth.updated = true;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		// Token: 0x06000591 RID: 1425 RVA: 0x00024AE8 File Offset: 0x00022CE8
		private void CreateMaskCameraFor(Camera cam, MaskData data)
		{
			if (data.cam == null)
			{
				GameObject gameObject = new GameObject("Ceto Mask Camera: " + cam.name);
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				gameObject.AddComponent<IgnoreOceanEvents>();
				gameObject.AddComponent<DisableFog>();
				gameObject.AddComponent<DisableShadows>();
				data.cam = gameObject.AddComponent<Camera>();
				data.cam.clearFlags = CameraClearFlags.Color;
				data.cam.backgroundColor = Color.black;
				data.cam.cullingMask = 1 << LayerMask.NameToLayer(Ocean.OCEAN_LAYER);
				data.cam.enabled = false;
				data.cam.renderingPath = RenderingPath.Forward;
				data.cam.targetTexture = null;
				data.cam.useOcclusionCulling = false;
				data.cam.RemoveAllCommandBuffers();
				data.cam.targetTexture = null;
			}
			data.cam.fieldOfView = cam.fieldOfView;
			data.cam.nearClipPlane = cam.nearClipPlane;
			data.cam.farClipPlane = cam.farClipPlane;
			data.cam.transform.position = cam.transform.position;
			data.cam.transform.rotation = cam.transform.rotation;
			data.cam.worldToCameraMatrix = cam.worldToCameraMatrix;
			data.cam.projectionMatrix = cam.projectionMatrix;
			data.cam.orthographic = cam.orthographic;
			data.cam.aspect = cam.aspect;
			data.cam.orthographicSize = cam.orthographicSize;
			data.cam.rect = new Rect(0f, 0f, 1f, 1f);
			if (data.cam.farClipPlane < this.OCEAN_BOTTOM_DEPTH * 2f)
			{
				data.cam.farClipPlane = this.OCEAN_BOTTOM_DEPTH * 2f;
				data.cam.ResetProjectionMatrix();
			}
			RenderTexture targetTexture = data.cam.targetTexture;
			if (targetTexture == null || targetTexture.width != cam.pixelWidth || targetTexture.height != cam.pixelHeight)
			{
				if (targetTexture != null)
				{
					RTUtility.ReleaseAndDestroy(targetTexture);
				}
				int pixelWidth = cam.pixelWidth;
				int pixelHeight = cam.pixelHeight;
				int depth = 32;
				data.cam.targetTexture = new RenderTexture(pixelWidth, pixelHeight, depth, RenderTextureFormat.RGHalf, RenderTextureReadWrite.Linear);
				data.cam.targetTexture.filterMode = FilterMode.Point;
				data.cam.targetTexture.hideFlags = HideFlags.DontSave;
				data.cam.targetTexture.name = "Ceto Mask Render Target: " + cam.name;
			}
		}

		// Token: 0x06000592 RID: 1426 RVA: 0x00024D9C File Offset: 0x00022F9C
		private void CreateDepthCameraFor(Camera cam, DepthData data)
		{
			if (data.cam == null)
			{
				GameObject gameObject = new GameObject("Ceto Depth Camera: " + cam.name);
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				gameObject.AddComponent<IgnoreOceanEvents>();
				gameObject.AddComponent<DisableFog>();
				gameObject.AddComponent<DisableShadows>();
				data.cam = gameObject.AddComponent<Camera>();
				data.cam.clearFlags = CameraClearFlags.Color;
				data.cam.backgroundColor = Color.white;
				data.cam.enabled = false;
				data.cam.renderingPath = RenderingPath.Forward;
				data.cam.targetTexture = null;
				data.cam.useOcclusionCulling = false;
				data.cam.RemoveAllCommandBuffers();
				data.cam.targetTexture = null;
			}
			data.cam.fieldOfView = cam.fieldOfView;
			data.cam.nearClipPlane = cam.nearClipPlane;
			data.cam.farClipPlane = cam.farClipPlane;
			data.cam.transform.position = cam.transform.position;
			data.cam.transform.rotation = cam.transform.rotation;
			data.cam.worldToCameraMatrix = cam.worldToCameraMatrix;
			data.cam.projectionMatrix = cam.projectionMatrix;
			data.cam.orthographic = cam.orthographic;
			data.cam.aspect = cam.aspect;
			data.cam.orthographicSize = cam.orthographicSize;
			data.cam.rect = new Rect(0f, 0f, 1f, 1f);
			data.cam.layerCullDistances = cam.layerCullDistances;
			data.cam.layerCullSpherical = cam.layerCullSpherical;
			RenderTexture targetTexture = data.cam.targetTexture;
			if (targetTexture == null || targetTexture.width != cam.pixelWidth || targetTexture.height != cam.pixelHeight)
			{
				if (targetTexture != null)
				{
					RTUtility.ReleaseAndDestroy(targetTexture);
				}
				int pixelWidth = cam.pixelWidth;
				int pixelHeight = cam.pixelHeight;
				int depth = 24;
				RenderTextureFormat format;
				if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGFloat))
				{
					format = RenderTextureFormat.RGFloat;
				}
				else
				{
					format = RenderTextureFormat.RGHalf;
				}
				data.cam.targetTexture = new RenderTexture(pixelWidth, pixelHeight, depth, format, RenderTextureReadWrite.Linear);
				data.cam.targetTexture.filterMode = FilterMode.Bilinear;
				data.cam.targetTexture.hideFlags = HideFlags.DontSave;
				data.cam.targetTexture.name = "Ceto Ocean Depths Render Target: " + cam.name;
			}
		}

		// Token: 0x06000593 RID: 1427 RVA: 0x00025034 File Offset: 0x00023234
		private void CreateRefractionGrab(Camera cam, DepthData data)
		{
			IRefractionCommand refractionCommand = this.GetRefractionCommand();
			CameraEvent commandEvent = this.GetCommandEvent(cam);
			if (data.grabCmd != null)
			{
				if (this.depthMode == DEPTH_MODE.USE_DEPTH_BUFFER && this.refractionResolution == refractionCommand.Resolution && commandEvent == data.cmdEvent && refractionCommand.Matches(cam))
				{
					return;
				}
				refractionCommand.Remove(cam);
				data.grabCmd = null;
			}
			if (this.depthMode == DEPTH_MODE.USE_DEPTH_BUFFER)
			{
				refractionCommand.Resolution = this.refractionResolution;
				refractionCommand.Event = commandEvent;
				data.grabCmd = refractionCommand.Create(cam);
				data.cmdEvent = commandEvent;
			}
		}

		// Token: 0x06000594 RID: 1428 RVA: 0x000250D8 File Offset: 0x000232D8
		private CameraEvent GetCommandEvent(Camera cam)
		{
			RenderingPath actualRenderingPath = cam.actualRenderingPath;
			if (actualRenderingPath == RenderingPath.DeferredShading)
			{
				return CameraEvent.AfterLighting;
			}
			if (actualRenderingPath == RenderingPath.DeferredLighting)
			{
				return CameraEvent.AfterLighting;
			}
			return CameraEvent.AfterDepthTexture;
		}

		// Token: 0x06000595 RID: 1429 RVA: 0x00025100 File Offset: 0x00023300
		private IRefractionCommand GetRefractionCommand()
		{
			IRefractionCommand result;
			if (base.CustomRefractionCommand != null)
			{
				result = base.CustomRefractionCommand;
				this.m_refractionCommand.RemoveAll();
			}
			else
			{
				result = this.m_refractionCommand;
			}
			return result;
		}

		// Token: 0x06000596 RID: 1430 RVA: 0x0002513C File Offset: 0x0002333C
		private Mesh CreateBottomMesh(int segementsX, int segementsY)
		{
			Vector3[] array = new Vector3[segementsX * segementsY];
			Vector2[] uv = new Vector2[segementsX * segementsY];
			float num = 6.2831855f;
			for (int i = 0; i < segementsX; i++)
			{
				for (int j = 0; j < segementsY; j++)
				{
					float num2 = (float)i / (float)(segementsX - 1);
					array[i + j * segementsX].x = num2 * Mathf.Cos(num * (float)j / (float)(segementsY - 1));
					array[i + j * segementsX].y = 0f;
					array[i + j * segementsX].z = num2 * Mathf.Sin(num * (float)j / (float)(segementsY - 1));
					if (i == segementsX - 1)
					{
						array[i + j * segementsX].y = 1f;
					}
				}
			}
			int[] array2 = new int[segementsX * segementsY * 6];
			int num3 = 0;
			for (int k = 0; k < segementsX - 1; k++)
			{
				for (int l = 0; l < segementsY - 1; l++)
				{
					array2[num3++] = k + l * segementsX;
					array2[num3++] = k + (l + 1) * segementsX;
					array2[num3++] = k + 1 + l * segementsX;
					array2[num3++] = k + (l + 1) * segementsX;
					array2[num3++] = k + 1 + (l + 1) * segementsX;
					array2[num3++] = k + 1 + l * segementsX;
				}
			}
			Mesh mesh = new Mesh();
			mesh.vertices = array;
			mesh.uv = uv;
			mesh.triangles = array2;
			mesh.name = "Ceto Bottom Mesh";
			mesh.hideFlags = HideFlags.HideAndDontSave;
			mesh.Optimize();
			return mesh;
		}

		// Token: 0x0400054D RID: 1357
		public const float MAX_REFRACTION_INTENSITY = 2f;

		// Token: 0x0400054E RID: 1358
		public const float MAX_REFRACTION_DISORTION = 4f;

		// Token: 0x0400054F RID: 1359
		public const float MAX_ABSORPTION_INTENSITY = 10f;

		// Token: 0x04000550 RID: 1360
		public const float MAX_INSCATTER_INTENSITY = 2f;

		// Token: 0x04000551 RID: 1361
		public const CameraEvent FORWARD_EVENT = CameraEvent.AfterDepthTexture;

		// Token: 0x04000552 RID: 1362
		public const CameraEvent DEFERRED_EVENT = CameraEvent.AfterLighting;

		// Token: 0x04000553 RID: 1363
		public const CameraEvent LEGACY_DEFERRED_EVENT = CameraEvent.AfterLighting;

		// Token: 0x04000554 RID: 1364
		private readonly float OCEAN_BOTTOM_DEPTH = 1000f;

		// Token: 0x04000555 RID: 1365
		private readonly float MAX_DEPTH_DIST = 500f;

		// Token: 0x04000556 RID: 1366
		public UNDERWATER_MODE underwaterMode;

		// Token: 0x04000557 RID: 1367
		public DEPTH_MODE depthMode;

		// Token: 0x04000558 RID: 1368
		public LayerMask oceanDepthsMask = 1;

		// Token: 0x04000559 RID: 1369
		private REFRACTION_RESOLUTION refractionResolution;

		// Token: 0x0400055A RID: 1370
		[Range(0f, 1f)]
		public float depthBlend = 0.2f;

		// Token: 0x0400055B RID: 1371
		[Range(0f, 1f)]
		public float edgeFade = 0.2f;

		// Token: 0x0400055C RID: 1372
		[Range(0f, 1f)]
		public float absorptionR = 0.45f;

		// Token: 0x0400055D RID: 1373
		[Range(0f, 1f)]
		public float absorptionG = 0.029f;

		// Token: 0x0400055E RID: 1374
		[Range(0f, 1f)]
		public float absorptionB = 0.018f;

		// Token: 0x0400055F RID: 1375
		public UnderWater.AbsorptionModifier aboveAbsorptionModifier = new UnderWater.AbsorptionModifier(2f, 1f, Color.white);

		// Token: 0x04000560 RID: 1376
		public UnderWater.AbsorptionModifier belowAbsorptionModifier = new UnderWater.AbsorptionModifier(0.1f, 1f, Color.white);

		// Token: 0x04000561 RID: 1377
		public UnderWater.AbsorptionModifier subSurfaceScatterModifier = new UnderWater.AbsorptionModifier(10f, 1.5f, new Color32(220, 250, 180, byte.MaxValue));

		// Token: 0x04000562 RID: 1378
		public UnderWater.InscatterModifier aboveInscatterModifier = new UnderWater.InscatterModifier(300f, 1f, new Color32(0, 19, 30, byte.MaxValue), UnderWater.INSCATTER_MODE.EXP2);

		// Token: 0x04000563 RID: 1379
		public UnderWater.InscatterModifier belowInscatterModifier = new UnderWater.InscatterModifier(60f, 1f, new Color32(1, 37, 58, byte.MaxValue), UnderWater.INSCATTER_MODE.EXP);

		// Token: 0x04000564 RID: 1380
		[Range(0f, 2f)]
		public float refractionIntensity = 0.5f;

		// Token: 0x04000565 RID: 1381
		[Range(0f, 4f)]
		public float refractionDistortion = 0.5f;

		// Token: 0x04000566 RID: 1382
		private IRefractionCommand m_refractionCommand;

		// Token: 0x04000567 RID: 1383
		private GameObject m_bottomMask;

		// Token: 0x04000568 RID: 1384
		[HideInInspector]
		public Shader oceanBottomSdr;

		// Token: 0x04000569 RID: 1385
		[HideInInspector]
		public Shader oceanDepthSdr;

		// Token: 0x0400056A RID: 1386
		[HideInInspector]
		public Shader copyDepthSdr;

		// Token: 0x0400056B RID: 1387
		[HideInInspector]
		public Shader oceanMaskSdr;

		// Token: 0x020000C0 RID: 192
		[Serializable]
		public struct AbsorptionModifier
		{
			// Token: 0x06000597 RID: 1431 RVA: 0x00025304 File Offset: 0x00023504
			public AbsorptionModifier(float scale, float intensity, Color tint)
			{
				this.scale = scale;
				this.intensity = intensity;
				this.tint = tint;
			}

			// Token: 0x0400056C RID: 1388
			[Range(0f, 50f)]
			public float scale;

			// Token: 0x0400056D RID: 1389
			[Range(0f, 10f)]
			public float intensity;

			// Token: 0x0400056E RID: 1390
			public Color tint;
		}

		// Token: 0x020000C1 RID: 193
		public enum INSCATTER_MODE
		{
			// Token: 0x04000570 RID: 1392
			LINEAR,
			// Token: 0x04000571 RID: 1393
			EXP,
			// Token: 0x04000572 RID: 1394
			EXP2
		}

		// Token: 0x020000C2 RID: 194
		[Serializable]
		public struct InscatterModifier
		{
			// Token: 0x06000598 RID: 1432 RVA: 0x0002531C File Offset: 0x0002351C
			public InscatterModifier(float scale, float intensity, Color color, UnderWater.INSCATTER_MODE mode)
			{
				this.scale = scale;
				this.intensity = intensity;
				this.color = color;
				this.mode = mode;
			}

			// Token: 0x04000573 RID: 1395
			[Range(0f, 5000f)]
			public float scale;

			// Token: 0x04000574 RID: 1396
			[Range(0f, 2f)]
			public float intensity;

			// Token: 0x04000575 RID: 1397
			public UnderWater.INSCATTER_MODE mode;

			// Token: 0x04000576 RID: 1398
			public Color color;
		}
	}
}
