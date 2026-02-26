using System;
using Ceto.Common.Unity.Utility;
using UnityEngine;

namespace Ceto
{
	// Token: 0x02000096 RID: 150
	[AddComponentMenu("Ceto/Components/PlanarReflection")]
	[RequireComponent(typeof(Ocean))]
	[DisallowMultipleComponent]
	public class PlanarReflection : ReflectionBase
	{
		// Token: 0x06000412 RID: 1042 RVA: 0x00019078 File Offset: 0x00017278
		private void Start()
		{
			try
			{
				this.m_imageBlur = new ImageBlur(this.blurShader);
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x000190D8 File Offset: 0x000172D8
		protected override void OnEnable()
		{
			base.OnEnable();
			Shader.EnableKeyword("CETO_REFLECTION_ON");
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x000190EC File Offset: 0x000172EC
		protected override void OnDisable()
		{
			base.OnDisable();
			Shader.DisableKeyword("CETO_REFLECTION_ON");
		}

		// Token: 0x06000415 RID: 1045 RVA: 0x00019100 File Offset: 0x00017300
		protected override void OnDestroy()
		{
			base.OnDestroy();
			try
			{
				if (this.m_dummy != null)
				{
					UnityEngine.Object.DestroyImmediate(this.m_dummy);
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x00019170 File Offset: 0x00017370
		private void Update()
		{
			this.reflectionIntensity = Mathf.Max(0f, this.reflectionIntensity);
			Shader.SetGlobalVector("Ceto_ReflectionTint", this.reflectionTint * this.reflectionIntensity);
			Shader.SetGlobalFloat("Ceto_ReflectionDistortion", this.reflectionDistortion * 0.05f);
		}

		// Token: 0x06000417 RID: 1047 RVA: 0x000191CC File Offset: 0x000173CC
		private LayerMask GetReflectionLayermask(OceanCameraSettings settings)
		{
			return (!(settings != null)) ? this.reflectionMask : settings.reflectionMask;
		}

		// Token: 0x06000418 RID: 1048 RVA: 0x000191EC File Offset: 0x000173EC
		private bool GetDisableReflections(OceanCameraSettings settings)
		{
			return settings != null && settings.disableReflections;
		}

		// Token: 0x06000419 RID: 1049 RVA: 0x00019208 File Offset: 0x00017408
		private REFLECTION_RESOLUTION GetReflectionResolution(OceanCameraSettings settings)
		{
			return (!(settings != null)) ? this.reflectionResolution : settings.reflectionResolution;
		}

		// Token: 0x0600041A RID: 1050 RVA: 0x00019228 File Offset: 0x00017428
		public override void RenderReflection(GameObject go)
		{
			try
			{
				if (base.enabled)
				{
					Camera current = Camera.current;
					if (!(current == null))
					{
						CameraData cameraData = this.m_ocean.FindCameraData(current);
						if (cameraData.reflection == null)
						{
							cameraData.reflection = new ReflectionData();
						}
						if (!cameraData.reflection.updated)
						{
							if (this.GetDisableReflections(cameraData.settings))
							{
								Shader.DisableKeyword("CETO_REFLECTION_ON");
								cameraData.reflection.updated = true;
							}
							else
							{
								Shader.EnableKeyword("CETO_REFLECTION_ON");
								if (cameraData.reflection.cam != null)
								{
									DisableFog component = cameraData.reflection.cam.GetComponent<DisableFog>();
									if (component != null)
									{
										component.enabled = !this.fogInReflection;
									}
								}
								RenderTexture renderTexture;
								if (this.RenderReflectionCustom != null)
								{
									if (cameraData.reflection.cam != null)
									{
										RTUtility.ReleaseAndDestroy(cameraData.reflection.cam.targetTexture);
										cameraData.reflection.cam.targetTexture = null;
										UnityEngine.Object.Destroy(cameraData.reflection.cam.gameObject);
										UnityEngine.Object.Destroy(cameraData.reflection.cam);
										cameraData.reflection.cam = null;
									}
									this.CreateRenderTarget(cameraData.reflection, current.name, current.pixelWidth, current.pixelHeight, current.hdr, cameraData.settings);
									if (this.m_dummy == null)
									{
										this.m_dummy = new GameObject("Ceto Reflection Dummy Gameobject");
										this.m_dummy.hideFlags = HideFlags.HideAndDontSave;
									}
									this.m_dummy.transform.position = new Vector3(0f, this.m_ocean.level, 0f);
									renderTexture = this.RenderReflectionCustom(this.m_dummy);
								}
								else
								{
									this.CreateReflectionCameraFor(current, cameraData.reflection);
									this.CreateRenderTarget(cameraData.reflection, current.name, current.pixelWidth, current.pixelHeight, current.hdr, cameraData.settings);
									NotifyOnEvent.Disable = true;
									this.RenderReflectionFor(current, cameraData.reflection.cam, cameraData.settings);
									NotifyOnEvent.Disable = false;
									renderTexture = cameraData.reflection.cam.targetTexture;
								}
								if (renderTexture != null)
								{
									Graphics.Blit(renderTexture, cameraData.reflection.tex);
									this.m_imageBlur.BlurIterations = this.blurIterations;
									this.m_imageBlur.BlurMode = this.blurMode;
									this.m_imageBlur.BlurSpread = this.blurSpread;
									this.m_imageBlur.Blur(cameraData.reflection.tex);
									Shader.SetGlobalTexture(Ocean.REFLECTION_TEXTURE_NAME, cameraData.reflection.tex);
								}
								cameraData.reflection.updated = true;
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

		// Token: 0x0600041B RID: 1051 RVA: 0x00019554 File Offset: 0x00017754
		private void CreateReflectionCameraFor(Camera cam, ReflectionData data)
		{
			if (data.cam == null)
			{
				GameObject gameObject = new GameObject("Ceto Reflection Camera: " + cam.name);
				gameObject.AddComponent<IgnoreOceanEvents>();
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				DisableFog disableFog = gameObject.AddComponent<DisableFog>();
				disableFog.enabled = !this.fogInReflection;
				data.cam = gameObject.AddComponent<Camera>();
				gameObject.AddComponent<ReflectionCameraFog>();
				data.cam.depthTextureMode = DepthTextureMode.None;
				data.cam.renderingPath = RenderingPath.Forward;
				data.cam.enabled = false;
				data.cam.hdr = cam.hdr;
				data.cam.targetTexture = null;
			}
			data.cam.fieldOfView = cam.fieldOfView;
			data.cam.nearClipPlane = cam.nearClipPlane;
			data.cam.farClipPlane = cam.farClipPlane;
			data.cam.orthographic = cam.orthographic;
			data.cam.aspect = cam.aspect;
			data.cam.orthographicSize = cam.orthographicSize;
			data.cam.rect = new Rect(0f, 0f, 1f, 1f);
			data.cam.backgroundColor = this.m_ocean.defaultSkyColor;
			data.cam.clearFlags = ((!this.skyboxInReflection) ? CameraClearFlags.Color : CameraClearFlags.Skybox);
			if (this.copyCullDistances)
			{
				data.cam.layerCullDistances = cam.layerCullDistances;
				data.cam.layerCullSpherical = true;
			}
		}

		// Token: 0x0600041C RID: 1052 RVA: 0x000196E8 File Offset: 0x000178E8
		private void CreateRenderTarget(ReflectionData data, string camName, int width, int height, bool isHdr, OceanCameraSettings settings)
		{
			int num = this.ResolutionToNumber(this.GetReflectionResolution(settings));
			width /= num;
			height /= num;
			if (data.tex != null && data.tex.width == width && data.tex.height == height)
			{
				data.tex.anisoLevel = this.ansio;
				return;
			}
			if (data.tex != null)
			{
				RTUtility.ReleaseAndDestroy(data.tex);
			}
			RenderTextureFormat format;
			if (isHdr || QualitySettings.activeColorSpace == ColorSpace.Linear)
			{
				format = RenderTextureFormat.ARGBHalf;
			}
			else
			{
				format = RenderTextureFormat.ARGB32;
			}
			data.tex = new RenderTexture(width, height, 0, format, RenderTextureReadWrite.Default);
			data.tex.filterMode = FilterMode.Bilinear;
			data.tex.wrapMode = TextureWrapMode.Clamp;
			data.tex.useMipMap = false;
			data.tex.anisoLevel = this.ansio;
			data.tex.hideFlags = HideFlags.HideAndDontSave;
			data.tex.name = "Ceto Reflection Texture: " + camName;
			if (data.cam != null)
			{
				if (data.cam.targetTexture != null)
				{
					RTUtility.ReleaseAndDestroy(data.cam.targetTexture);
				}
				data.cam.targetTexture = new RenderTexture(width, height, 16, format, RenderTextureReadWrite.Default);
				data.cam.targetTexture.filterMode = FilterMode.Bilinear;
				data.cam.targetTexture.wrapMode = TextureWrapMode.Clamp;
				data.cam.targetTexture.useMipMap = false;
				data.cam.targetTexture.anisoLevel = 0;
				data.cam.targetTexture.hideFlags = HideFlags.HideAndDontSave;
				data.cam.targetTexture.name = "Ceto Reflection Render Target: " + camName;
			}
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x000198B4 File Offset: 0x00017AB4
		private int ResolutionToNumber(REFLECTION_RESOLUTION resolution)
		{
			switch (resolution)
			{
			case REFLECTION_RESOLUTION.FULL:
				return 1;
			case REFLECTION_RESOLUTION.HALF:
				return 2;
			case REFLECTION_RESOLUTION.QUARTER:
				return 4;
			default:
				return 2;
			}
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x000198E4 File Offset: 0x00017AE4
		private void UpdateSkyBox(Camera cam, Camera reflectCamera)
		{
			reflectCamera.backgroundColor = this.m_ocean.defaultSkyColor;
			reflectCamera.clearFlags = CameraClearFlags.Color;
			if (this.skyboxInReflection && cam.clearFlags == CameraClearFlags.Skybox)
			{
				reflectCamera.clearFlags = CameraClearFlags.Skybox;
				Skybox component = cam.gameObject.GetComponent<Skybox>();
				if (component)
				{
					Skybox skybox = reflectCamera.gameObject.GetComponent<Skybox>();
					if (!skybox)
					{
						skybox = reflectCamera.gameObject.AddComponent<Skybox>();
					}
					skybox.material = component.material;
				}
			}
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x00019970 File Offset: 0x00017B70
		private void RenderReflectionFor(Camera cam, Camera reflectCamera, OceanCameraSettings settings)
		{
			Vector3 eulerAngles = cam.transform.eulerAngles;
			reflectCamera.transform.eulerAngles = new Vector3(-eulerAngles.x, eulerAngles.y, eulerAngles.z);
			reflectCamera.transform.position = cam.transform.position;
			float level = this.m_ocean.level;
			Vector3 vector = new Vector3(0f, level, 0f);
			Vector3 up = Vector3.up;
			float w = -Vector3.Dot(up, vector) - this.clipPlaneOffset;
			Vector4 plane = new Vector4(up.x, up.y, up.z, w);
			Matrix4x4 matrix4x = Matrix4x4.zero;
			matrix4x = this.CalculateReflectionMatrix(matrix4x, plane);
			Vector3 position = cam.transform.position;
			Vector3 position2 = matrix4x.MultiplyPoint(position);
			reflectCamera.worldToCameraMatrix = cam.worldToCameraMatrix * matrix4x;
			Vector4 clipPlane = this.CameraSpacePlane(reflectCamera, vector, up, 1f);
			Matrix4x4 matrix4x2 = cam.projectionMatrix;
			matrix4x2 = this.CalculateObliqueMatrix(matrix4x2, clipPlane);
			reflectCamera.projectionMatrix = matrix4x2;
			reflectCamera.transform.position = position2;
			Vector3 eulerAngles2 = cam.transform.eulerAngles;
			reflectCamera.transform.eulerAngles = new Vector3(-eulerAngles2.x, eulerAngles2.y, eulerAngles2.z);
			reflectCamera.cullingMask = this.GetReflectionLayermask(settings);
			reflectCamera.cullingMask = OceanUtility.HideLayer(reflectCamera.cullingMask, Ocean.OCEAN_LAYER);
			int pixelLightCount = QualitySettings.pixelLightCount;
			if (!this.pixelLightsInReflection)
			{
				QualitySettings.pixelLightCount = 0;
			}
			bool invertCulling = GL.invertCulling;
			GL.invertCulling = !invertCulling;
			reflectCamera.Render();
			QualitySettings.pixelLightCount = pixelLightCount;
			GL.invertCulling = invertCulling;
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x00019B28 File Offset: 0x00017D28
		private Matrix4x4 CalculateObliqueMatrix(Matrix4x4 projection, Vector4 clipPlane)
		{
			Vector4 b = projection.inverse * new Vector4(Mathf.Sign(clipPlane.x), Mathf.Sign(clipPlane.y), 1f, 1f);
			Vector4 vector = clipPlane * (2f / Vector4.Dot(clipPlane, b));
			projection[2] = vector.x - projection[3];
			projection[6] = vector.y - projection[7];
			projection[10] = vector.z - projection[11];
			projection[14] = vector.w - projection[15];
			return projection;
		}

		// Token: 0x06000421 RID: 1057 RVA: 0x00019BE4 File Offset: 0x00017DE4
		private Matrix4x4 CalculateReflectionMatrix(Matrix4x4 reflectionMat, Vector4 plane)
		{
			reflectionMat.m00 = 1f - 2f * plane[0] * plane[0];
			reflectionMat.m01 = -2f * plane[0] * plane[1];
			reflectionMat.m02 = -2f * plane[0] * plane[2];
			reflectionMat.m03 = -2f * plane[3] * plane[0];
			reflectionMat.m10 = -2f * plane[1] * plane[0];
			reflectionMat.m11 = 1f - 2f * plane[1] * plane[1];
			reflectionMat.m12 = -2f * plane[1] * plane[2];
			reflectionMat.m13 = -2f * plane[3] * plane[1];
			reflectionMat.m20 = -2f * plane[2] * plane[0];
			reflectionMat.m21 = -2f * plane[2] * plane[1];
			reflectionMat.m22 = 1f - 2f * plane[2] * plane[2];
			reflectionMat.m23 = -2f * plane[3] * plane[2];
			reflectionMat.m30 = 0f;
			reflectionMat.m31 = 0f;
			reflectionMat.m32 = 0f;
			reflectionMat.m33 = 1f;
			return reflectionMat;
		}

		// Token: 0x06000422 RID: 1058 RVA: 0x00019D9C File Offset: 0x00017F9C
		private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
		{
			Vector3 v = pos + normal * this.clipPlaneOffset;
			Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
			Vector3 lhs = worldToCameraMatrix.MultiplyPoint(v);
			Vector3 rhs = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
			return new Vector4(rhs.x, rhs.y, rhs.z, -Vector3.Dot(lhs, rhs));
		}

		// Token: 0x04000445 RID: 1093
		public const float MAX_REFLECTION_INTENSITY = 2f;

		// Token: 0x04000446 RID: 1094
		public const float MAX_REFLECTION_DISORTION = 4f;

		// Token: 0x04000447 RID: 1095
		public LayerMask reflectionMask = 1;

		// Token: 0x04000448 RID: 1096
		public REFLECTION_RESOLUTION reflectionResolution = REFLECTION_RESOLUTION.HALF;

		// Token: 0x04000449 RID: 1097
		public float clipPlaneOffset = 0.07f;

		// Token: 0x0400044A RID: 1098
		public bool pixelLightsInReflection;

		// Token: 0x0400044B RID: 1099
		public bool fogInReflection;

		// Token: 0x0400044C RID: 1100
		public bool skyboxInReflection = true;

		// Token: 0x0400044D RID: 1101
		public bool copyCullDistances;

		// Token: 0x0400044E RID: 1102
		public ImageBlur.BLUR_MODE blurMode;

		// Token: 0x0400044F RID: 1103
		[Range(0f, 4f)]
		public int blurIterations = 1;

		// Token: 0x04000450 RID: 1104
		[Range(0.5f, 1f)]
		private float blurSpread = 0.6f;

		// Token: 0x04000451 RID: 1105
		public Color reflectionTint = Color.white;

		// Token: 0x04000452 RID: 1106
		[Range(0f, 2f)]
		public float reflectionIntensity = 0.6f;

		// Token: 0x04000453 RID: 1107
		[Range(0f, 4f)]
		public float reflectionDistortion = 0.5f;

		// Token: 0x04000454 RID: 1108
		public int ansio = 2;

		// Token: 0x04000455 RID: 1109
		private GameObject m_dummy;

		// Token: 0x04000456 RID: 1110
		private ImageBlur m_imageBlur;

		// Token: 0x04000457 RID: 1111
		[HideInInspector]
		public Shader blurShader;
	}
}
