using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000032 RID: 50
[AddComponentMenu("")]
public class AmplifyOcclusionBase : MonoBehaviour
{
	// Token: 0x06000191 RID: 401 RVA: 0x0000A5B8 File Offset: 0x000087B8
	private bool CheckParamsChanged()
	{
		return this.prevScreenWidth != this.m_camera.pixelWidth || this.prevScreenHeight != this.m_camera.pixelHeight || this.prevHDR != this.m_camera.hdr || this.prevApplyMethod != this.ApplyMethod || this.prevSampleCount != this.SampleCount || this.prevPerPixelNormals != this.PerPixelNormals || this.prevCacheAware != this.CacheAware || this.prevDownscale != this.Downsample || this.prevBlurEnabled != this.BlurEnabled || this.prevBlurRadius != this.BlurRadius || this.prevBlurPasses != this.BlurPasses;
	}

	// Token: 0x06000192 RID: 402 RVA: 0x0000A694 File Offset: 0x00008894
	private void UpdateParams()
	{
		this.prevScreenWidth = this.m_camera.pixelWidth;
		this.prevScreenHeight = this.m_camera.pixelHeight;
		this.prevHDR = this.m_camera.hdr;
		this.prevApplyMethod = this.ApplyMethod;
		this.prevSampleCount = this.SampleCount;
		this.prevPerPixelNormals = this.PerPixelNormals;
		this.prevCacheAware = this.CacheAware;
		this.prevDownscale = this.Downsample;
		this.prevBlurEnabled = this.BlurEnabled;
		this.prevBlurRadius = this.BlurRadius;
		this.prevBlurPasses = this.BlurPasses;
	}

	// Token: 0x06000193 RID: 403 RVA: 0x0000A734 File Offset: 0x00008934
	private void Warmup()
	{
		this.CheckMaterial();
		this.CheckRandomData();
		this.m_depthLayerRT = new int[16];
		this.m_normalLayerRT = new int[16];
		this.m_occlusionLayerRT = new int[16];
		this.m_mrtCount = Mathf.Min(SystemInfo.supportedRenderTargetCount, 4);
		this.m_layerOffsetNames = new string[this.m_mrtCount];
		this.m_layerRandomNames = new string[this.m_mrtCount];
		for (int i = 0; i < this.m_mrtCount; i++)
		{
			this.m_layerOffsetNames[i] = "_AO_LayerOffset" + i;
			this.m_layerRandomNames[i] = "_AO_LayerRandom" + i;
		}
		this.m_layerDepthNames = new string[16];
		this.m_layerNormalNames = new string[16];
		this.m_layerOcclusionNames = new string[16];
		for (int j = 0; j < 16; j++)
		{
			this.m_layerDepthNames[j] = "_AO_DepthLayer" + j;
			this.m_layerNormalNames[j] = "_AO_NormalLayer" + j;
			this.m_layerOcclusionNames[j] = "_AO_OcclusionLayer" + j;
		}
		this.m_depthTargets = new RenderTargetIdentifier[this.m_mrtCount];
		this.m_normalTargets = new RenderTargetIdentifier[this.m_mrtCount];
		int mrtCount = this.m_mrtCount;
		if (mrtCount != 4)
		{
			this.m_deinterleaveDepthPass = 5;
			this.m_deinterleaveNormalPass = 6;
		}
		else
		{
			this.m_deinterleaveDepthPass = 10;
			this.m_deinterleaveNormalPass = 11;
		}
		this.m_applyDeferredTargets = new RenderTargetIdentifier[2];
		if (this.m_blitMesh != null)
		{
			UnityEngine.Object.DestroyImmediate(this.m_blitMesh);
		}
		this.m_blitMesh = new Mesh();
		this.m_blitMesh.vertices = new Vector3[]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(0f, 1f, 0f),
			new Vector3(1f, 1f, 0f),
			new Vector3(1f, 0f, 0f)
		};
		this.m_blitMesh.uv = new Vector2[]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f)
		};
		this.m_blitMesh.triangles = new int[]
		{
			0,
			1,
			2,
			0,
			2,
			3
		};
	}

	// Token: 0x06000194 RID: 404 RVA: 0x0000AA2C File Offset: 0x00008C2C
	private void Shutdown()
	{
		this.CommandBuffer_UnregisterAll();
		this.SafeReleaseRT(ref this.m_occlusionRT);
		if (this.m_occlusionMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.m_occlusionMat);
		}
		if (this.m_blurMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.m_blurMat);
		}
		if (this.m_copyMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.m_copyMat);
		}
		if (this.m_randomTex != null)
		{
			UnityEngine.Object.DestroyImmediate(this.m_randomTex);
		}
		if (this.m_blitMesh != null)
		{
			UnityEngine.Object.DestroyImmediate(this.m_blitMesh);
		}
	}

	// Token: 0x06000195 RID: 405 RVA: 0x0000AAD8 File Offset: 0x00008CD8
	private void OnEnable()
	{
		this.m_camera = base.GetComponent<Camera>();
		this.Warmup();
		this.CommandBuffer_UnregisterAll();
	}

	// Token: 0x06000196 RID: 406 RVA: 0x0000AAF4 File Offset: 0x00008CF4
	private void OnDisable()
	{
		this.Shutdown();
	}

	// Token: 0x06000197 RID: 407 RVA: 0x0000AAFC File Offset: 0x00008CFC
	private void OnDestroy()
	{
		this.Shutdown();
	}

	// Token: 0x06000198 RID: 408 RVA: 0x0000AB04 File Offset: 0x00008D04
	private void Update()
	{
		if (this.m_camera.actualRenderingPath != RenderingPath.DeferredShading)
		{
			if (this.PerPixelNormals != AmplifyOcclusionBase.PerPixelNormalSource.None && this.PerPixelNormals != AmplifyOcclusionBase.PerPixelNormalSource.Camera)
			{
				this.PerPixelNormals = AmplifyOcclusionBase.PerPixelNormalSource.Camera;
				Debug.LogWarning("[AmplifyOcclusion] GBuffer Normals only available in Camera Deferred Shading mode. Switched to Camera source.");
			}
			if (this.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.Deferred)
			{
				this.ApplyMethod = AmplifyOcclusionBase.ApplicationMethod.PostEffect;
				Debug.LogWarning("[AmplifyOcclusion] Deferred Method requires a Deferred Shading path. Switching to Post Effect Method.");
			}
		}
		if (this.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.Deferred && this.PerPixelNormals == AmplifyOcclusionBase.PerPixelNormalSource.Camera)
		{
			this.PerPixelNormals = AmplifyOcclusionBase.PerPixelNormalSource.GBuffer;
			Debug.LogWarning("[AmplifyOcclusion] Camera Normals not supported for Deferred Method. Switching to GBuffer Normals.");
		}
		if ((this.m_camera.depthTextureMode & DepthTextureMode.Depth) == DepthTextureMode.None)
		{
			this.m_camera.depthTextureMode |= DepthTextureMode.Depth;
		}
		if (this.PerPixelNormals == AmplifyOcclusionBase.PerPixelNormalSource.Camera && (this.m_camera.depthTextureMode & DepthTextureMode.DepthNormals) == DepthTextureMode.None)
		{
			this.m_camera.depthTextureMode |= DepthTextureMode.DepthNormals;
		}
		this.CheckMaterial();
		this.CheckRandomData();
	}

	// Token: 0x06000199 RID: 409 RVA: 0x0000ABF4 File Offset: 0x00008DF4
	private void CheckMaterial()
	{
		if (this.m_occlusionMat == null)
		{
			this.m_occlusionMat = new Material(Shader.Find("Hidden/Amplify Occlusion/Occlusion"))
			{
				hideFlags = HideFlags.DontSave
			};
		}
		if (this.m_blurMat == null)
		{
			this.m_blurMat = new Material(Shader.Find("Hidden/Amplify Occlusion/Blur"))
			{
				hideFlags = HideFlags.DontSave
			};
		}
		if (this.m_copyMat == null)
		{
			this.m_copyMat = new Material(Shader.Find("Hidden/Amplify Occlusion/Copy"))
			{
				hideFlags = HideFlags.DontSave
			};
		}
	}

	// Token: 0x0600019A RID: 410 RVA: 0x0000AC94 File Offset: 0x00008E94
	private void CheckRandomData()
	{
		if (this.m_randomData == null)
		{
			this.m_randomData = AmplifyOcclusionBase.GenerateRandomizationData();
		}
		if (this.m_randomTex == null)
		{
			this.m_randomTex = AmplifyOcclusionBase.GenerateRandomizationTexture(this.m_randomData);
		}
	}

	// Token: 0x0600019B RID: 411 RVA: 0x0000ACDC File Offset: 0x00008EDC
	public static Color[] GenerateRandomizationData()
	{
		Color[] array = new Color[16];
		int i = 0;
		int num = 0;
		while (i < 16)
		{
			float num2 = RandomTable.Values[num++];
			float b = RandomTable.Values[num++];
			float f = 6.2831855f * num2 / 8f;
			array[i].r = Mathf.Cos(f);
			array[i].g = Mathf.Sin(f);
			array[i].b = b;
			array[i].a = 0f;
			i++;
		}
		return array;
	}

	// Token: 0x0600019C RID: 412 RVA: 0x0000AD74 File Offset: 0x00008F74
	public static Texture2D GenerateRandomizationTexture(Color[] randomPixels)
	{
		Texture2D texture2D = new Texture2D(4, 4, TextureFormat.ARGB32, false, true)
		{
			hideFlags = HideFlags.DontSave
		};
		texture2D.name = "RandomTexture";
		texture2D.filterMode = FilterMode.Point;
		texture2D.wrapMode = TextureWrapMode.Repeat;
		texture2D.SetPixels(randomPixels);
		texture2D.Apply();
		return texture2D;
	}

	// Token: 0x0600019D RID: 413 RVA: 0x0000ADC0 File Offset: 0x00008FC0
	private RenderTexture SafeAllocateRT(string name, int width, int height, RenderTextureFormat format, RenderTextureReadWrite readWrite)
	{
		width = Mathf.Max(width, 1);
		height = Mathf.Max(height, 1);
		RenderTexture renderTexture = new RenderTexture(width, height, 0, format, readWrite)
		{
			hideFlags = HideFlags.DontSave
		};
		renderTexture.name = name;
		renderTexture.filterMode = FilterMode.Point;
		renderTexture.wrapMode = TextureWrapMode.Clamp;
		renderTexture.Create();
		return renderTexture;
	}

	// Token: 0x0600019E RID: 414 RVA: 0x0000AE14 File Offset: 0x00009014
	private void SafeReleaseRT(ref RenderTexture rt)
	{
		if (rt != null)
		{
			RenderTexture.active = null;
			rt.Release();
			UnityEngine.Object.DestroyImmediate(rt);
			rt = null;
		}
	}

	// Token: 0x0600019F RID: 415 RVA: 0x0000AE48 File Offset: 0x00009048
	private int SafeAllocateTemporaryRT(CommandBuffer cb, string propertyName, int width, int height, RenderTextureFormat format = RenderTextureFormat.Default, RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default, FilterMode filterMode = FilterMode.Point)
	{
		int num = Shader.PropertyToID(propertyName);
		cb.GetTemporaryRT(num, width, height, 0, filterMode, format, readWrite);
		return num;
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x0000AE70 File Offset: 0x00009070
	private void SafeReleaseTemporaryRT(CommandBuffer cb, int id)
	{
		cb.ReleaseTemporaryRT(id);
	}

	// Token: 0x060001A1 RID: 417 RVA: 0x0000AE7C File Offset: 0x0000907C
	private void SetBlitTarget(CommandBuffer cb, RenderTargetIdentifier[] targets, int targetWidth, int targetHeight)
	{
		cb.SetGlobalVector("_AO_Target_TexelSize", new Vector4(1f / (float)targetWidth, 1f / (float)targetHeight, (float)targetWidth, (float)targetHeight));
		cb.SetGlobalVector("_AO_Target_Position", Vector2.zero);
		cb.SetRenderTarget(targets, targets[0]);
	}

	// Token: 0x060001A2 RID: 418 RVA: 0x0000AED8 File Offset: 0x000090D8
	private void SetBlitTarget(CommandBuffer cb, RenderTargetIdentifier target, int targetWidth, int targetHeight)
	{
		cb.SetGlobalVector("_AO_Target_TexelSize", new Vector4(1f / (float)targetWidth, 1f / (float)targetHeight, (float)targetWidth, (float)targetHeight));
		cb.SetRenderTarget(target);
	}

	// Token: 0x060001A3 RID: 419 RVA: 0x0000AF08 File Offset: 0x00009108
	private void PerformBlit(CommandBuffer cb, Material mat, int pass)
	{
		cb.DrawMesh(this.m_blitMesh, Matrix4x4.identity, mat, 0, pass);
	}

	// Token: 0x060001A4 RID: 420 RVA: 0x0000AF20 File Offset: 0x00009120
	private void PerformBlit(CommandBuffer cb, Material mat, int pass, int x, int y)
	{
		cb.SetGlobalVector("_AO_Target_Position", new Vector2((float)x, (float)y));
		this.PerformBlit(cb, mat, pass);
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x0000AF54 File Offset: 0x00009154
	private void PerformBlit(CommandBuffer cb, RenderTargetIdentifier source, int sourceWidth, int sourceHeight, Material mat, int pass)
	{
		cb.SetGlobalTexture("_AO_Source", source);
		cb.SetGlobalVector("_AO_Source_TexelSize", new Vector4(1f / (float)sourceWidth, 1f / (float)sourceHeight, (float)sourceWidth, (float)sourceHeight));
		this.PerformBlit(cb, mat, pass);
	}

	// Token: 0x060001A6 RID: 422 RVA: 0x0000AFA0 File Offset: 0x000091A0
	private void PerformBlit(CommandBuffer cb, RenderTargetIdentifier source, int sourceWidth, int sourceHeight, Material mat, int pass, int x, int y)
	{
		cb.SetGlobalVector("_AO_Target_Position", new Vector2((float)x, (float)y));
		this.PerformBlit(cb, source, sourceWidth, sourceHeight, mat, pass);
	}

	// Token: 0x060001A7 RID: 423 RVA: 0x0000AFD8 File Offset: 0x000091D8
	private CommandBuffer CommandBuffer_Allocate(string name)
	{
		return new CommandBuffer
		{
			name = name
		};
	}

	// Token: 0x060001A8 RID: 424 RVA: 0x0000AFF4 File Offset: 0x000091F4
	private void CommandBuffer_Register(CameraEvent cameraEvent, CommandBuffer commandBuffer)
	{
		this.m_camera.AddCommandBuffer(cameraEvent, commandBuffer);
		this.m_registeredCommandBuffers.Add(cameraEvent, commandBuffer);
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x0000B010 File Offset: 0x00009210
	private void CommandBuffer_Unregister(CameraEvent cameraEvent, CommandBuffer commandBuffer)
	{
		if (this.m_camera != null)
		{
			CommandBuffer[] commandBuffers = this.m_camera.GetCommandBuffers(cameraEvent);
			foreach (CommandBuffer commandBuffer2 in commandBuffers)
			{
				if (commandBuffer2.name == commandBuffer.name)
				{
					this.m_camera.RemoveCommandBuffer(cameraEvent, commandBuffer2);
				}
			}
		}
	}

	// Token: 0x060001AA RID: 426 RVA: 0x0000B078 File Offset: 0x00009278
	private CommandBuffer CommandBuffer_AllocateRegister(CameraEvent cameraEvent)
	{
		string name = string.Empty;
		if (cameraEvent == CameraEvent.BeforeReflections)
		{
			name = "AO-BeforeRefl";
		}
		else if (cameraEvent == CameraEvent.AfterLighting)
		{
			name = "AO-AfterLighting";
		}
		else if (cameraEvent == CameraEvent.BeforeImageEffectsOpaque)
		{
			name = "AO-BeforePostOpaque";
		}
		else
		{
			Debug.LogError("[AmplifyOcclusion] Unsupported CameraEvent. Please contact support.");
		}
		CommandBuffer commandBuffer = this.CommandBuffer_Allocate(name);
		this.CommandBuffer_Register(cameraEvent, commandBuffer);
		return commandBuffer;
	}

	// Token: 0x060001AB RID: 427 RVA: 0x0000B0E0 File Offset: 0x000092E0
	private void CommandBuffer_UnregisterAll()
	{
		foreach (KeyValuePair<CameraEvent, CommandBuffer> keyValuePair in this.m_registeredCommandBuffers)
		{
			this.CommandBuffer_Unregister(keyValuePair.Key, keyValuePair.Value);
		}
		this.m_registeredCommandBuffers.Clear();
	}

	// Token: 0x060001AC RID: 428 RVA: 0x0000B160 File Offset: 0x00009360
	private void UpdateGlobalShaderConstants(AmplifyOcclusionBase.TargetDesc target)
	{
		float num = this.m_camera.fieldOfView * 0.017453292f;
		Vector2 vector = new Vector2(1f / Mathf.Tan(num * 0.5f) * ((float)target.height / (float)target.width), 1f / Mathf.Tan(num * 0.5f));
		Vector2 vector2 = new Vector2(1f / vector.x, 1f / vector.y);
		float num2;
		if (this.m_camera.orthographic)
		{
			num2 = (float)target.height / this.m_camera.orthographicSize;
		}
		else
		{
			num2 = (float)target.height / (Mathf.Tan(num * 0.5f) * 2f);
		}
		float num3 = Mathf.Clamp(this.Bias, 0f, 1f);
		Shader.SetGlobalMatrix("_AO_CameraProj", GL.GetGPUProjectionMatrix(Matrix4x4.Ortho(0f, 1f, 0f, 1f, -1f, 100f), false));
		Shader.SetGlobalMatrix("_AO_CameraView", this.m_camera.worldToCameraMatrix);
		Shader.SetGlobalVector("_AO_UVToView", new Vector4(2f * vector2.x, -2f * vector2.y, -1f * vector2.x, 1f * vector2.y));
		Shader.SetGlobalFloat("_AO_NegRcpR2", -1f / (this.Radius * this.Radius));
		Shader.SetGlobalFloat("_AO_RadiusToScreen", this.Radius * 0.5f * num2);
		Shader.SetGlobalFloat("_AO_PowExponent", this.PowerExponent);
		Shader.SetGlobalFloat("_AO_Bias", num3);
		Shader.SetGlobalFloat("_AO_Multiplier", 1f / (1f - num3));
		Shader.SetGlobalFloat("_AO_BlurSharpness", this.BlurSharpness);
		Shader.SetGlobalColor("_AO_Levels", new Color(this.Tint.r, this.Tint.g, this.Tint.b, this.Intensity));
	}

	// Token: 0x060001AD RID: 429 RVA: 0x0000B374 File Offset: 0x00009574
	private void CommandBuffer_FillComputeOcclusion(CommandBuffer cb, AmplifyOcclusionBase.TargetDesc target)
	{
		this.CheckMaterial();
		this.CheckRandomData();
		cb.SetGlobalVector("_AO_Buffer_PadScale", new Vector4(target.padRatioWidth, target.padRatioHeight, 1f / target.padRatioWidth, 1f / target.padRatioHeight));
		cb.SetGlobalVector("_AO_Buffer_TexelSize", new Vector4(1f / (float)target.width, 1f / (float)target.height, (float)target.width, (float)target.height));
		cb.SetGlobalVector("_AO_QuarterBuffer_TexelSize", new Vector4(1f / (float)target.quarterWidth, 1f / (float)target.quarterHeight, (float)target.quarterWidth, (float)target.quarterHeight));
		cb.SetGlobalFloat("_AO_MaxRadiusPixels", (float)Mathf.Min(target.width, target.height));
		if (this.m_occlusionRT == null || this.m_occlusionRT.width != target.width || this.m_occlusionRT.height != target.height || !this.m_occlusionRT.IsCreated())
		{
			this.SafeReleaseRT(ref this.m_occlusionRT);
			this.m_occlusionRT = this.SafeAllocateRT("_AO_OcclusionTexture", target.width, target.height, RenderTextureFormat.RGHalf, RenderTextureReadWrite.Linear);
		}
		int num = -1;
		if (this.Downsample)
		{
			num = this.SafeAllocateTemporaryRT(cb, "_AO_SmallOcclusionTexture", target.width / 2, target.height / 2, RenderTextureFormat.RGHalf, RenderTextureReadWrite.Linear, FilterMode.Bilinear);
		}
		if (this.CacheAware && !this.Downsample)
		{
			int num2 = this.SafeAllocateTemporaryRT(cb, "_AO_OcclusionAtlas", target.width, target.height, RenderTextureFormat.RGHalf, RenderTextureReadWrite.Linear, FilterMode.Point);
			for (int i = 0; i < 16; i++)
			{
				this.m_depthLayerRT[i] = this.SafeAllocateTemporaryRT(cb, this.m_layerDepthNames[i], target.quarterWidth, target.quarterHeight, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear, FilterMode.Point);
				this.m_normalLayerRT[i] = this.SafeAllocateTemporaryRT(cb, this.m_layerNormalNames[i], target.quarterWidth, target.quarterHeight, RenderTextureFormat.ARGB2101010, RenderTextureReadWrite.Linear, FilterMode.Point);
				this.m_occlusionLayerRT[i] = this.SafeAllocateTemporaryRT(cb, this.m_layerOcclusionNames[i], target.quarterWidth, target.quarterHeight, RenderTextureFormat.RGHalf, RenderTextureReadWrite.Linear, FilterMode.Point);
			}
			for (int j = 0; j < 16; j += this.m_mrtCount)
			{
				for (int k = 0; k < this.m_mrtCount; k++)
				{
					int num3 = k + j;
					int num4 = num3 & 3;
					int num5 = num3 >> 2;
					cb.SetGlobalVector(this.m_layerOffsetNames[k], new Vector2((float)num4 + 0.5f, (float)num5 + 0.5f));
					this.m_depthTargets[k] = this.m_depthLayerRT[num3];
					this.m_normalTargets[k] = this.m_normalLayerRT[num3];
				}
				this.SetBlitTarget(cb, this.m_depthTargets, target.quarterWidth, target.quarterHeight);
				this.PerformBlit(cb, this.m_occlusionMat, this.m_deinterleaveDepthPass);
				this.SetBlitTarget(cb, this.m_normalTargets, target.quarterWidth, target.quarterHeight);
				this.PerformBlit(cb, this.m_occlusionMat, (int)(this.m_deinterleaveNormalPass + this.PerPixelNormals));
			}
			for (int l = 0; l < 16; l++)
			{
				cb.SetGlobalVector("_AO_LayerOffset", new Vector2((float)(l & 3) + 0.5f, (float)(l >> 2) + 0.5f));
				cb.SetGlobalVector("_AO_LayerRandom", this.m_randomData[l]);
				cb.SetGlobalTexture("_AO_NormalTexture", this.m_normalLayerRT[l]);
				cb.SetGlobalTexture("_AO_DepthTexture", this.m_depthLayerRT[l]);
				this.SetBlitTarget(cb, this.m_occlusionLayerRT[l], target.quarterWidth, target.quarterHeight);
				this.PerformBlit(cb, this.m_occlusionMat, (int)(15 + this.SampleCount));
			}
			this.SetBlitTarget(cb, num2, target.width, target.height);
			for (int m = 0; m < 16; m++)
			{
				int x = (m & 3) * target.quarterWidth;
				int y = (m >> 2) * target.quarterHeight;
				this.PerformBlit(cb, this.m_occlusionLayerRT[m], target.quarterWidth, target.quarterHeight, this.m_copyMat, 0, x, y);
			}
			cb.SetGlobalTexture("_AO_OcclusionAtlas", num2);
			this.SetBlitTarget(cb, this.m_occlusionRT, target.width, target.height);
			this.PerformBlit(cb, this.m_occlusionMat, 19);
			for (int n = 0; n < 16; n++)
			{
				this.SafeReleaseTemporaryRT(cb, this.m_occlusionLayerRT[n]);
				this.SafeReleaseTemporaryRT(cb, this.m_normalLayerRT[n]);
				this.SafeReleaseTemporaryRT(cb, this.m_depthLayerRT[n]);
			}
			this.SafeReleaseTemporaryRT(cb, num2);
		}
		else
		{
			this.m_occlusionMat.SetTexture("_AO_RandomTexture", this.m_randomTex);
			int pass = (int)(20 + this.SampleCount * (AmplifyOcclusionBase.SampleCountLevel)4 + (int)this.PerPixelNormals);
			if (this.Downsample)
			{
				cb.Blit(null, new RenderTargetIdentifier(num), this.m_occlusionMat, pass);
				this.SetBlitTarget(cb, this.m_occlusionRT, target.width, target.height);
				this.PerformBlit(cb, num, target.width / 2, target.height / 2, this.m_occlusionMat, 41);
			}
			else
			{
				cb.Blit(null, this.m_occlusionRT, this.m_occlusionMat, pass);
			}
		}
		if (this.BlurEnabled)
		{
			int num6 = this.SafeAllocateTemporaryRT(cb, "_AO_TEMP", target.width, target.height, RenderTextureFormat.RGHalf, RenderTextureReadWrite.Linear, FilterMode.Point);
			for (int num7 = 0; num7 < this.BlurPasses; num7++)
			{
				this.SetBlitTarget(cb, num6, target.width, target.height);
				this.PerformBlit(cb, this.m_occlusionRT, target.width, target.height, this.m_blurMat, 0 + (this.BlurRadius - 1) * 2);
				this.SetBlitTarget(cb, this.m_occlusionRT, target.width, target.height);
				this.PerformBlit(cb, num6, target.width, target.height, this.m_blurMat, 1 + (this.BlurRadius - 1) * 2);
			}
			this.SafeReleaseTemporaryRT(cb, num6);
		}
		if (this.Downsample && num >= 0)
		{
			this.SafeReleaseTemporaryRT(cb, num);
		}
		cb.SetRenderTarget(null);
	}

	// Token: 0x060001AE RID: 430 RVA: 0x0000BA80 File Offset: 0x00009C80
	private void CommandBuffer_FillApplyDeferred(CommandBuffer cb, AmplifyOcclusionBase.TargetDesc target, bool logTarget)
	{
		cb.SetGlobalTexture("_AO_OcclusionTexture", this.m_occlusionRT);
		this.m_applyDeferredTargets[0] = BuiltinRenderTextureType.GBuffer0;
		this.m_applyDeferredTargets[1] = ((!logTarget) ? BuiltinRenderTextureType.CameraTarget : BuiltinRenderTextureType.GBuffer3);
		if (!logTarget)
		{
			this.SetBlitTarget(cb, this.m_applyDeferredTargets, target.fullWidth, target.fullHeight);
			this.PerformBlit(cb, this.m_occlusionMat, 37);
		}
		else
		{
			int num = this.SafeAllocateTemporaryRT(cb, "_AO_GBufferAlbedo", target.fullWidth, target.fullHeight, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, FilterMode.Point);
			int num2 = this.SafeAllocateTemporaryRT(cb, "_AO_GBufferEmission", target.fullWidth, target.fullHeight, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, FilterMode.Point);
			cb.Blit(this.m_applyDeferredTargets[0], num);
			cb.Blit(this.m_applyDeferredTargets[1], num2);
			cb.SetGlobalTexture("_AO_GBufferAlbedo", num);
			cb.SetGlobalTexture("_AO_GBufferEmission", num2);
			this.SetBlitTarget(cb, this.m_applyDeferredTargets, target.fullWidth, target.fullHeight);
			this.PerformBlit(cb, this.m_occlusionMat, 38);
			this.SafeReleaseTemporaryRT(cb, num);
			this.SafeReleaseTemporaryRT(cb, num2);
		}
		cb.SetRenderTarget(null);
	}

	// Token: 0x060001AF RID: 431 RVA: 0x0000BBF4 File Offset: 0x00009DF4
	private void CommandBuffer_FillApplyPostEffect(CommandBuffer cb, AmplifyOcclusionBase.TargetDesc target, bool logTarget)
	{
		cb.SetGlobalTexture("_AO_OcclusionTexture", this.m_occlusionRT);
		if (!logTarget)
		{
			this.SetBlitTarget(cb, BuiltinRenderTextureType.CameraTarget, target.fullWidth, target.fullHeight);
			this.PerformBlit(cb, this.m_occlusionMat, 39);
		}
		else
		{
			int num = this.SafeAllocateTemporaryRT(cb, "_AO_GBufferEmission", target.fullWidth, target.fullHeight, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, FilterMode.Point);
			cb.Blit(BuiltinRenderTextureType.GBuffer3, num);
			cb.SetGlobalTexture("_AO_GBufferEmission", num);
			this.SetBlitTarget(cb, BuiltinRenderTextureType.GBuffer3, target.fullWidth, target.fullHeight);
			this.PerformBlit(cb, this.m_occlusionMat, 40);
			this.SafeReleaseTemporaryRT(cb, num);
		}
		cb.SetRenderTarget(null);
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x0000BCD0 File Offset: 0x00009ED0
	private void CommandBuffer_FillApplyDebug(CommandBuffer cb, AmplifyOcclusionBase.TargetDesc target)
	{
		cb.SetGlobalTexture("_AO_OcclusionTexture", this.m_occlusionRT);
		this.SetBlitTarget(cb, BuiltinRenderTextureType.CameraTarget, target.fullWidth, target.fullHeight);
		this.PerformBlit(cb, this.m_occlusionMat, 36);
		cb.SetRenderTarget(null);
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x0000BD2C File Offset: 0x00009F2C
	private void CommandBuffer_Rebuild(AmplifyOcclusionBase.TargetDesc target)
	{
		bool flag = this.PerPixelNormals == AmplifyOcclusionBase.PerPixelNormalSource.GBuffer || this.PerPixelNormals == AmplifyOcclusionBase.PerPixelNormalSource.GBufferOctaEncoded;
		CameraEvent cameraEvent = (!flag) ? CameraEvent.BeforeImageEffectsOpaque : CameraEvent.AfterLighting;
		if (this.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.Debug)
		{
			CommandBuffer cb = this.CommandBuffer_AllocateRegister(cameraEvent);
			this.CommandBuffer_FillComputeOcclusion(cb, target);
			this.CommandBuffer_FillApplyDebug(cb, target);
		}
		else
		{
			bool logTarget = !this.m_camera.hdr && flag;
			cameraEvent = ((this.ApplyMethod != AmplifyOcclusionBase.ApplicationMethod.Deferred) ? cameraEvent : CameraEvent.BeforeReflections);
			CommandBuffer cb = this.CommandBuffer_AllocateRegister(cameraEvent);
			this.CommandBuffer_FillComputeOcclusion(cb, target);
			if (this.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.PostEffect)
			{
				this.CommandBuffer_FillApplyPostEffect(cb, target, logTarget);
			}
			else if (this.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.Deferred)
			{
				this.CommandBuffer_FillApplyDeferred(cb, target, logTarget);
			}
		}
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x0000BDF4 File Offset: 0x00009FF4
	private void OnPreRender()
	{
		this.m_target.fullWidth = this.m_camera.pixelWidth;
		this.m_target.fullHeight = this.m_camera.pixelHeight;
		this.m_target.format = ((!this.m_camera.hdr) ? RenderTextureFormat.ARGB32 : RenderTextureFormat.ARGBHalf);
		this.m_target.width = ((!this.CacheAware) ? this.m_target.fullWidth : (this.m_target.fullWidth + 3 & -4));
		this.m_target.height = ((!this.CacheAware) ? this.m_target.fullHeight : (this.m_target.fullHeight + 3 & -4));
		this.m_target.quarterWidth = this.m_target.width / 4;
		this.m_target.quarterHeight = this.m_target.height / 4;
		this.m_target.padRatioWidth = (float)this.m_target.width / (float)this.m_target.fullWidth;
		this.m_target.padRatioHeight = (float)this.m_target.height / (float)this.m_target.fullHeight;
		this.UpdateGlobalShaderConstants(this.m_target);
		if (this.CheckParamsChanged() || this.m_registeredCommandBuffers.Count == 0)
		{
			this.CommandBuffer_UnregisterAll();
			this.CommandBuffer_Rebuild(this.m_target);
			this.UpdateParams();
		}
	}

	// Token: 0x040001B4 RID: 436
	private const int PerPixelNormalSourceCount = 4;

	// Token: 0x040001B5 RID: 437
	private const int RandomSize = 4;

	// Token: 0x040001B6 RID: 438
	private const int DirectionCount = 8;

	// Token: 0x040001B7 RID: 439
	[Header("Ambient Occlusion")]
	public AmplifyOcclusionBase.ApplicationMethod ApplyMethod;

	// Token: 0x040001B8 RID: 440
	public AmplifyOcclusionBase.SampleCountLevel SampleCount = AmplifyOcclusionBase.SampleCountLevel.Medium;

	// Token: 0x040001B9 RID: 441
	public AmplifyOcclusionBase.PerPixelNormalSource PerPixelNormals = AmplifyOcclusionBase.PerPixelNormalSource.Camera;

	// Token: 0x040001BA RID: 442
	[Range(0f, 1f)]
	public float Intensity = 1f;

	// Token: 0x040001BB RID: 443
	public Color Tint = Color.black;

	// Token: 0x040001BC RID: 444
	[Range(0f, 16f)]
	public float Radius = 1f;

	// Token: 0x040001BD RID: 445
	[Range(0f, 16f)]
	public float PowerExponent = 1.8f;

	// Token: 0x040001BE RID: 446
	[Range(0f, 0.99f)]
	public float Bias = 0.05f;

	// Token: 0x040001BF RID: 447
	public bool CacheAware;

	// Token: 0x040001C0 RID: 448
	public bool Downsample;

	// Token: 0x040001C1 RID: 449
	[Header("Bilateral Blur")]
	public bool BlurEnabled = true;

	// Token: 0x040001C2 RID: 450
	[Range(1f, 4f)]
	public int BlurRadius = 2;

	// Token: 0x040001C3 RID: 451
	[Range(1f, 4f)]
	public int BlurPasses = 1;

	// Token: 0x040001C4 RID: 452
	[Range(0f, 20f)]
	public float BlurSharpness = 10f;

	// Token: 0x040001C5 RID: 453
	private int prevScreenWidth;

	// Token: 0x040001C6 RID: 454
	private int prevScreenHeight;

	// Token: 0x040001C7 RID: 455
	private bool prevHDR;

	// Token: 0x040001C8 RID: 456
	private AmplifyOcclusionBase.ApplicationMethod prevApplyMethod;

	// Token: 0x040001C9 RID: 457
	private AmplifyOcclusionBase.SampleCountLevel prevSampleCount;

	// Token: 0x040001CA RID: 458
	private AmplifyOcclusionBase.PerPixelNormalSource prevPerPixelNormals;

	// Token: 0x040001CB RID: 459
	private bool prevCacheAware;

	// Token: 0x040001CC RID: 460
	private bool prevDownscale;

	// Token: 0x040001CD RID: 461
	private bool prevBlurEnabled;

	// Token: 0x040001CE RID: 462
	private int prevBlurRadius;

	// Token: 0x040001CF RID: 463
	private int prevBlurPasses;

	// Token: 0x040001D0 RID: 464
	private Camera m_camera;

	// Token: 0x040001D1 RID: 465
	private Material m_occlusionMat;

	// Token: 0x040001D2 RID: 466
	private Material m_blurMat;

	// Token: 0x040001D3 RID: 467
	private Material m_copyMat;

	// Token: 0x040001D4 RID: 468
	private Texture2D m_randomTex;

	// Token: 0x040001D5 RID: 469
	private Color[] m_randomData;

	// Token: 0x040001D6 RID: 470
	private string[] m_layerOffsetNames;

	// Token: 0x040001D7 RID: 471
	private string[] m_layerRandomNames;

	// Token: 0x040001D8 RID: 472
	private string[] m_layerDepthNames;

	// Token: 0x040001D9 RID: 473
	private string[] m_layerNormalNames;

	// Token: 0x040001DA RID: 474
	private string[] m_layerOcclusionNames;

	// Token: 0x040001DB RID: 475
	private RenderTexture m_occlusionRT;

	// Token: 0x040001DC RID: 476
	private int[] m_depthLayerRT;

	// Token: 0x040001DD RID: 477
	private int[] m_normalLayerRT;

	// Token: 0x040001DE RID: 478
	private int[] m_occlusionLayerRT;

	// Token: 0x040001DF RID: 479
	private int m_mrtCount;

	// Token: 0x040001E0 RID: 480
	private RenderTargetIdentifier[] m_depthTargets;

	// Token: 0x040001E1 RID: 481
	private RenderTargetIdentifier[] m_normalTargets;

	// Token: 0x040001E2 RID: 482
	private int m_deinterleaveDepthPass;

	// Token: 0x040001E3 RID: 483
	private int m_deinterleaveNormalPass;

	// Token: 0x040001E4 RID: 484
	private RenderTargetIdentifier[] m_applyDeferredTargets;

	// Token: 0x040001E5 RID: 485
	private Mesh m_blitMesh;

	// Token: 0x040001E6 RID: 486
	private AmplifyOcclusionBase.TargetDesc m_target = default(AmplifyOcclusionBase.TargetDesc);

	// Token: 0x040001E7 RID: 487
	private Dictionary<CameraEvent, CommandBuffer> m_registeredCommandBuffers = new Dictionary<CameraEvent, CommandBuffer>();

	// Token: 0x02000033 RID: 51
	public enum ApplicationMethod
	{
		// Token: 0x040001E9 RID: 489
		PostEffect,
		// Token: 0x040001EA RID: 490
		Deferred,
		// Token: 0x040001EB RID: 491
		Debug
	}

	// Token: 0x02000034 RID: 52
	public enum PerPixelNormalSource
	{
		// Token: 0x040001ED RID: 493
		None,
		// Token: 0x040001EE RID: 494
		Camera,
		// Token: 0x040001EF RID: 495
		GBuffer,
		// Token: 0x040001F0 RID: 496
		GBufferOctaEncoded
	}

	// Token: 0x02000035 RID: 53
	public enum SampleCountLevel
	{
		// Token: 0x040001F2 RID: 498
		Low,
		// Token: 0x040001F3 RID: 499
		Medium,
		// Token: 0x040001F4 RID: 500
		High,
		// Token: 0x040001F5 RID: 501
		VeryHigh
	}

	// Token: 0x02000036 RID: 54
	private static class ShaderPass
	{
		// Token: 0x040001F6 RID: 502
		public const int FullDepth = 0;

		// Token: 0x040001F7 RID: 503
		public const int FullNormal_None = 1;

		// Token: 0x040001F8 RID: 504
		public const int FullNormal_Camera = 2;

		// Token: 0x040001F9 RID: 505
		public const int FullNormal_GBuffer = 3;

		// Token: 0x040001FA RID: 506
		public const int FullNormal_GBufferOctaEncoded = 4;

		// Token: 0x040001FB RID: 507
		public const int DeinterleaveDepth1 = 5;

		// Token: 0x040001FC RID: 508
		public const int DeinterleaveNormal1_None = 6;

		// Token: 0x040001FD RID: 509
		public const int DeinterleaveNormal1_Camera = 7;

		// Token: 0x040001FE RID: 510
		public const int DeinterleaveNormal1_GBuffer = 8;

		// Token: 0x040001FF RID: 511
		public const int DeinterleaveNormal1_GBufferOctaEncoded = 9;

		// Token: 0x04000200 RID: 512
		public const int DeinterleaveDepth4 = 10;

		// Token: 0x04000201 RID: 513
		public const int DeinterleaveNormal4_None = 11;

		// Token: 0x04000202 RID: 514
		public const int DeinterleaveNormal4_Camera = 12;

		// Token: 0x04000203 RID: 515
		public const int DeinterleaveNormal4_GBuffer = 13;

		// Token: 0x04000204 RID: 516
		public const int DeinterleaveNormal4_GBufferOctaEncoded = 14;

		// Token: 0x04000205 RID: 517
		public const int OcclusionCache_Low = 15;

		// Token: 0x04000206 RID: 518
		public const int OcclusionCache_Medium = 16;

		// Token: 0x04000207 RID: 519
		public const int OcclusionCache_High = 17;

		// Token: 0x04000208 RID: 520
		public const int OcclusionCache_VeryHigh = 18;

		// Token: 0x04000209 RID: 521
		public const int Reinterleave = 19;

		// Token: 0x0400020A RID: 522
		public const int OcclusionLow_None = 20;

		// Token: 0x0400020B RID: 523
		public const int OcclusionLow_Camera = 21;

		// Token: 0x0400020C RID: 524
		public const int OcclusionLow_GBuffer = 22;

		// Token: 0x0400020D RID: 525
		public const int OcclusionLow_GBufferOctaEncoded = 23;

		// Token: 0x0400020E RID: 526
		public const int OcclusionMedium_None = 24;

		// Token: 0x0400020F RID: 527
		public const int OcclusionMedium_Camera = 25;

		// Token: 0x04000210 RID: 528
		public const int OcclusionMedium_GBuffer = 26;

		// Token: 0x04000211 RID: 529
		public const int OcclusionMedium_GBufferOctaEncoded = 27;

		// Token: 0x04000212 RID: 530
		public const int OcclusionHigh_None = 28;

		// Token: 0x04000213 RID: 531
		public const int OcclusionHigh_Camera = 29;

		// Token: 0x04000214 RID: 532
		public const int OcclusionHigh_GBuffer = 30;

		// Token: 0x04000215 RID: 533
		public const int OcclusionHigh_GBufferOctaEncoded = 31;

		// Token: 0x04000216 RID: 534
		public const int OcclusionVeryHigh_None = 32;

		// Token: 0x04000217 RID: 535
		public const int OcclusionVeryHigh_Camera = 33;

		// Token: 0x04000218 RID: 536
		public const int OcclusionVeryHigh_GBuffer = 34;

		// Token: 0x04000219 RID: 537
		public const int OcclusionVeryHigh_GBufferNormalEncoded = 35;

		// Token: 0x0400021A RID: 538
		public const int ApplyDebug = 36;

		// Token: 0x0400021B RID: 539
		public const int ApplyDeferred = 37;

		// Token: 0x0400021C RID: 540
		public const int ApplyDeferredLog = 38;

		// Token: 0x0400021D RID: 541
		public const int ApplyPostEffect = 39;

		// Token: 0x0400021E RID: 542
		public const int ApplyPostEffectLog = 40;

		// Token: 0x0400021F RID: 543
		public const int CombineDownsampledOcclusionDepth = 41;

		// Token: 0x04000220 RID: 544
		public const int BlurHorizontal1 = 0;

		// Token: 0x04000221 RID: 545
		public const int BlurVertical1 = 1;

		// Token: 0x04000222 RID: 546
		public const int BlurHorizontal2 = 2;

		// Token: 0x04000223 RID: 547
		public const int BlurVertical2 = 3;

		// Token: 0x04000224 RID: 548
		public const int BlurHorizontal3 = 4;

		// Token: 0x04000225 RID: 549
		public const int BlurVertical3 = 5;

		// Token: 0x04000226 RID: 550
		public const int BlurHorizontal4 = 6;

		// Token: 0x04000227 RID: 551
		public const int BlurVertical4 = 7;

		// Token: 0x04000228 RID: 552
		public const int Copy = 0;
	}

	// Token: 0x02000037 RID: 55
	private struct TargetDesc
	{
		// Token: 0x04000229 RID: 553
		public int fullWidth;

		// Token: 0x0400022A RID: 554
		public int fullHeight;

		// Token: 0x0400022B RID: 555
		public RenderTextureFormat format;

		// Token: 0x0400022C RID: 556
		public int width;

		// Token: 0x0400022D RID: 557
		public int height;

		// Token: 0x0400022E RID: 558
		public int quarterWidth;

		// Token: 0x0400022F RID: 559
		public int quarterHeight;

		// Token: 0x04000230 RID: 560
		public float padRatioWidth;

		// Token: 0x04000231 RID: 561
		public float padRatioHeight;
	}
}
