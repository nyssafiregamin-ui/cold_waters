using System;
using System.Collections.Generic;
using AmplifyColor;
using UnityEngine;

// Token: 0x02000022 RID: 34
[AddComponentMenu("")]
public class AmplifyColorBase : MonoBehaviour
{
	// Token: 0x17000064 RID: 100
	// (get) Token: 0x0600011D RID: 285 RVA: 0x00006FC8 File Offset: 0x000051C8
	public Texture2D DefaultLut
	{
		get
		{
			return (!(this.defaultLut == null)) ? this.defaultLut : this.CreateDefaultLut();
		}
	}

	// Token: 0x17000065 RID: 101
	// (get) Token: 0x0600011E RID: 286 RVA: 0x00006FF8 File Offset: 0x000051F8
	public bool IsBlending
	{
		get
		{
			return this.blending;
		}
	}

	// Token: 0x17000066 RID: 102
	// (get) Token: 0x0600011F RID: 287 RVA: 0x00007000 File Offset: 0x00005200
	private float effectVolumesBlendAdjusted
	{
		get
		{
			return Mathf.Clamp01((this.effectVolumesBlendAdjust >= 0.99f) ? 1f : ((this.volumesBlendAmount - this.effectVolumesBlendAdjust) / (1f - this.effectVolumesBlendAdjust)));
		}
	}

	// Token: 0x17000067 RID: 103
	// (get) Token: 0x06000120 RID: 288 RVA: 0x0000703C File Offset: 0x0000523C
	public string SharedInstanceID
	{
		get
		{
			return this.sharedInstanceID;
		}
	}

	// Token: 0x17000068 RID: 104
	// (get) Token: 0x06000121 RID: 289 RVA: 0x00007044 File Offset: 0x00005244
	public bool WillItBlend
	{
		get
		{
			return this.LutTexture != null && this.LutBlendTexture != null && !this.blending;
		}
	}

	// Token: 0x06000122 RID: 290 RVA: 0x00007080 File Offset: 0x00005280
	public void NewSharedInstanceID()
	{
		this.sharedInstanceID = Guid.NewGuid().ToString();
	}

	// Token: 0x06000123 RID: 291 RVA: 0x000070A0 File Offset: 0x000052A0
	private void ReportMissingShaders()
	{
		Debug.LogError("[AmplifyColor] Failed to initialize shaders. Please attempt to re-enable the Amplify Color Effect component. If that fails, please reinstall Amplify Color.");
		base.enabled = false;
	}

	// Token: 0x06000124 RID: 292 RVA: 0x000070B4 File Offset: 0x000052B4
	private void ReportNotSupported()
	{
		Debug.LogWarning("[AmplifyColor] This image effect is not supported on this platform.");
		base.enabled = false;
	}

	// Token: 0x06000125 RID: 293 RVA: 0x000070C8 File Offset: 0x000052C8
	private bool CheckShader(Shader s)
	{
		if (s == null)
		{
			this.ReportMissingShaders();
			return false;
		}
		if (!s.isSupported)
		{
			this.ReportNotSupported();
			return false;
		}
		return true;
	}

	// Token: 0x06000126 RID: 294 RVA: 0x00007100 File Offset: 0x00005300
	private bool CheckShaders()
	{
		return this.CheckShader(this.shaderBase) && this.CheckShader(this.shaderBlend) && this.CheckShader(this.shaderBlendCache) && this.CheckShader(this.shaderMask) && this.CheckShader(this.shaderMaskBlend) && this.CheckShader(this.shaderProcessOnly);
	}

	// Token: 0x06000127 RID: 295 RVA: 0x00007174 File Offset: 0x00005374
	private bool CheckSupport()
	{
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
		{
			this.ReportNotSupported();
			return false;
		}
		return true;
	}

	// Token: 0x06000128 RID: 296 RVA: 0x00007194 File Offset: 0x00005394
	private void OnEnable()
	{
		if (!this.CheckSupport())
		{
			return;
		}
		if (!this.CreateMaterials())
		{
			return;
		}
		Texture2D texture2D = this.LutTexture as Texture2D;
		Texture2D texture2D2 = this.LutBlendTexture as Texture2D;
		if ((texture2D != null && texture2D.mipmapCount > 1) || (texture2D2 != null && texture2D2.mipmapCount > 1))
		{
			Debug.LogError("[AmplifyColor] Please disable \"Generate Mip Maps\" import settings on all LUT textures to avoid visual glitches. Change Texture Type to \"Advanced\" to access Mip settings.");
		}
	}

	// Token: 0x06000129 RID: 297 RVA: 0x0000720C File Offset: 0x0000540C
	private void OnDisable()
	{
		if (this.actualTriggerProxy != null)
		{
			UnityEngine.Object.DestroyImmediate(this.actualTriggerProxy.gameObject);
			this.actualTriggerProxy = null;
		}
		this.ReleaseMaterials();
		this.ReleaseTextures();
	}

	// Token: 0x0600012A RID: 298 RVA: 0x00007250 File Offset: 0x00005450
	private void VolumesBlendTo(Texture blendTargetLUT, float blendTimeInSec)
	{
		this.volumesLutBlendTexture = blendTargetLUT;
		this.volumesBlendAmount = 0f;
		this.volumesBlendingTime = blendTimeInSec;
		this.volumesBlendingTimeCountdown = blendTimeInSec;
		this.volumesBlending = true;
	}

	// Token: 0x0600012B RID: 299 RVA: 0x0000727C File Offset: 0x0000547C
	public void BlendTo(Texture blendTargetLUT, float blendTimeInSec, Action onFinishBlend)
	{
		this.LutBlendTexture = blendTargetLUT;
		this.BlendAmount = 0f;
		this.onFinishBlend = onFinishBlend;
		this.blendingTime = blendTimeInSec;
		this.blendingTimeCountdown = blendTimeInSec;
		this.blending = true;
	}

	// Token: 0x0600012C RID: 300 RVA: 0x000072B8 File Offset: 0x000054B8
	private void CheckCamera()
	{
		if (this.ownerCamera == null)
		{
			this.ownerCamera = base.GetComponent<Camera>();
		}
		if (this.UseDepthMask && (this.ownerCamera.depthTextureMode & DepthTextureMode.Depth) == DepthTextureMode.None)
		{
			this.ownerCamera.depthTextureMode |= DepthTextureMode.Depth;
		}
	}

	// Token: 0x0600012D RID: 301 RVA: 0x00007314 File Offset: 0x00005514
	private void Start()
	{
		this.CheckCamera();
		this.worldLUT = this.LutTexture;
		this.worldVolumeEffects = this.EffectFlags.GenerateEffectData(this);
		this.blendVolumeEffects = (this.currentVolumeEffects = this.worldVolumeEffects);
		this.worldExposure = this.Exposure;
		this.blendExposure = (this.currentExposure = this.worldExposure);
	}

	// Token: 0x0600012E RID: 302 RVA: 0x0000737C File Offset: 0x0000557C
	private void Update()
	{
		this.CheckCamera();
		bool flag = false;
		if (this.volumesBlending)
		{
			this.volumesBlendAmount = (this.volumesBlendingTime - this.volumesBlendingTimeCountdown) / this.volumesBlendingTime;
			this.volumesBlendingTimeCountdown -= Time.smoothDeltaTime;
			if (this.volumesBlendAmount >= 1f)
			{
				this.volumesBlendAmount = 1f;
				flag = true;
			}
		}
		else
		{
			this.volumesBlendAmount = Mathf.Clamp01(this.volumesBlendAmount);
		}
		if (this.blending)
		{
			this.BlendAmount = (this.blendingTime - this.blendingTimeCountdown) / this.blendingTime;
			this.blendingTimeCountdown -= Time.smoothDeltaTime;
			if (this.BlendAmount >= 1f)
			{
				this.LutTexture = this.LutBlendTexture;
				this.BlendAmount = 0f;
				this.blending = false;
				this.LutBlendTexture = null;
				if (this.onFinishBlend != null)
				{
					this.onFinishBlend();
				}
			}
		}
		else
		{
			this.BlendAmount = Mathf.Clamp01(this.BlendAmount);
		}
		if (this.UseVolumes)
		{
			if (this.actualTriggerProxy == null)
			{
				GameObject gameObject = new GameObject(base.name + "+ACVolumeProxy")
				{
					hideFlags = HideFlags.HideAndDontSave
				};
				if (this.TriggerVolumeProxy != null && this.TriggerVolumeProxy.GetComponent<Collider2D>() != null)
				{
					this.actualTriggerProxy = gameObject.AddComponent<AmplifyColorTriggerProxy2D>();
				}
				else
				{
					this.actualTriggerProxy = gameObject.AddComponent<AmplifyColorTriggerProxy>();
				}
				this.actualTriggerProxy.OwnerEffect = this;
			}
			this.UpdateVolumes();
		}
		else if (this.actualTriggerProxy != null)
		{
			UnityEngine.Object.DestroyImmediate(this.actualTriggerProxy.gameObject);
			this.actualTriggerProxy = null;
		}
		if (flag)
		{
			this.LutTexture = this.volumesLutBlendTexture;
			this.volumesBlendAmount = 0f;
			this.volumesBlending = false;
			this.volumesLutBlendTexture = null;
			this.effectVolumesBlendAdjust = 0f;
			this.currentVolumeEffects = this.blendVolumeEffects;
			this.currentVolumeEffects.SetValues(this);
			this.currentExposure = this.blendExposure;
			if (this.blendingFromMidBlend && this.midBlendLUT != null)
			{
				this.midBlendLUT.DiscardContents();
			}
			this.blendingFromMidBlend = false;
		}
	}

	// Token: 0x0600012F RID: 303 RVA: 0x000075DC File Offset: 0x000057DC
	public void EnterVolume(AmplifyColorVolumeBase volume)
	{
		if (!this.enteredVolumes.Contains(volume))
		{
			this.enteredVolumes.Insert(0, volume);
		}
	}

	// Token: 0x06000130 RID: 304 RVA: 0x000075FC File Offset: 0x000057FC
	public void ExitVolume(AmplifyColorVolumeBase volume)
	{
		if (this.enteredVolumes.Contains(volume))
		{
			this.enteredVolumes.Remove(volume);
		}
	}

	// Token: 0x06000131 RID: 305 RVA: 0x0000761C File Offset: 0x0000581C
	private void UpdateVolumes()
	{
		if (this.volumesBlending)
		{
			this.currentVolumeEffects.BlendValues(this, this.blendVolumeEffects, this.effectVolumesBlendAdjusted);
		}
		if (this.volumesBlending)
		{
			this.Exposure = Mathf.Lerp(this.currentExposure, this.blendExposure, this.effectVolumesBlendAdjusted);
		}
		Transform transform = (!(this.TriggerVolumeProxy == null)) ? this.TriggerVolumeProxy : base.transform;
		if (this.actualTriggerProxy.transform.parent != transform)
		{
			this.actualTriggerProxy.Reference = transform;
			this.actualTriggerProxy.gameObject.layer = transform.gameObject.layer;
		}
		AmplifyColorVolumeBase amplifyColorVolumeBase = null;
		int num = int.MinValue;
		for (int i = 0; i < this.enteredVolumes.Count; i++)
		{
			AmplifyColorVolumeBase amplifyColorVolumeBase2 = this.enteredVolumes[i];
			if (amplifyColorVolumeBase2.Priority > num)
			{
				amplifyColorVolumeBase = amplifyColorVolumeBase2;
				num = amplifyColorVolumeBase2.Priority;
			}
		}
		if (amplifyColorVolumeBase != this.currentVolumeLut)
		{
			this.currentVolumeLut = amplifyColorVolumeBase;
			Texture texture = (!(amplifyColorVolumeBase == null)) ? amplifyColorVolumeBase.LutTexture : this.worldLUT;
			float num2 = (!(amplifyColorVolumeBase == null)) ? amplifyColorVolumeBase.EnterBlendTime : this.ExitVolumeBlendTime;
			if (this.volumesBlending && !this.blendingFromMidBlend && texture == this.LutTexture)
			{
				this.LutTexture = this.volumesLutBlendTexture;
				this.volumesLutBlendTexture = texture;
				this.volumesBlendingTimeCountdown = num2 * ((this.volumesBlendingTime - this.volumesBlendingTimeCountdown) / this.volumesBlendingTime);
				this.volumesBlendingTime = num2;
				this.currentVolumeEffects = VolumeEffect.BlendValuesToVolumeEffect(this.EffectFlags, this.currentVolumeEffects, this.blendVolumeEffects, this.effectVolumesBlendAdjusted);
				this.currentExposure = Mathf.Lerp(this.currentExposure, this.blendExposure, this.effectVolumesBlendAdjusted);
				this.effectVolumesBlendAdjust = 1f - this.volumesBlendAmount;
				this.volumesBlendAmount = 1f - this.volumesBlendAmount;
			}
			else
			{
				if (this.volumesBlending)
				{
					this.materialBlendCache.SetFloat("_LerpAmount", this.volumesBlendAmount);
					if (this.blendingFromMidBlend)
					{
						Graphics.Blit(this.midBlendLUT, this.blendCacheLut);
						this.materialBlendCache.SetTexture("_RgbTex", this.blendCacheLut);
					}
					else
					{
						this.materialBlendCache.SetTexture("_RgbTex", this.LutTexture);
					}
					this.materialBlendCache.SetTexture("_LerpRgbTex", (!(this.volumesLutBlendTexture != null)) ? this.defaultLut : this.volumesLutBlendTexture);
					Graphics.Blit(this.midBlendLUT, this.midBlendLUT, this.materialBlendCache);
					this.blendCacheLut.DiscardContents();
					this.currentVolumeEffects = VolumeEffect.BlendValuesToVolumeEffect(this.EffectFlags, this.currentVolumeEffects, this.blendVolumeEffects, this.effectVolumesBlendAdjusted);
					this.currentExposure = Mathf.Lerp(this.currentExposure, this.blendExposure, this.effectVolumesBlendAdjusted);
					this.effectVolumesBlendAdjust = 0f;
					this.blendingFromMidBlend = true;
				}
				this.VolumesBlendTo(texture, num2);
			}
			this.blendVolumeEffects = ((!(amplifyColorVolumeBase == null)) ? amplifyColorVolumeBase.EffectContainer.FindVolumeEffect(this) : this.worldVolumeEffects);
			this.blendExposure = ((!(amplifyColorVolumeBase == null)) ? amplifyColorVolumeBase.Exposure : this.worldExposure);
			if (this.blendVolumeEffects == null)
			{
				this.blendVolumeEffects = this.worldVolumeEffects;
			}
		}
	}

	// Token: 0x06000132 RID: 306 RVA: 0x000079C4 File Offset: 0x00005BC4
	private void SetupShader()
	{
		this.colorSpace = QualitySettings.activeColorSpace;
		this.qualityLevel = this.QualityLevel;
		this.shaderBase = Shader.Find("Hidden/Amplify Color/Base");
		this.shaderBlend = Shader.Find("Hidden/Amplify Color/Blend");
		this.shaderBlendCache = Shader.Find("Hidden/Amplify Color/BlendCache");
		this.shaderMask = Shader.Find("Hidden/Amplify Color/Mask");
		this.shaderMaskBlend = Shader.Find("Hidden/Amplify Color/MaskBlend");
		this.shaderDepthMask = Shader.Find("Hidden/Amplify Color/DepthMask");
		this.shaderDepthMaskBlend = Shader.Find("Hidden/Amplify Color/DepthMaskBlend");
		this.shaderProcessOnly = Shader.Find("Hidden/Amplify Color/ProcessOnly");
	}

	// Token: 0x06000133 RID: 307 RVA: 0x00007A68 File Offset: 0x00005C68
	private void ReleaseMaterials()
	{
		this.SafeRelease<Material>(ref this.materialBase);
		this.SafeRelease<Material>(ref this.materialBlend);
		this.SafeRelease<Material>(ref this.materialBlendCache);
		this.SafeRelease<Material>(ref this.materialMask);
		this.SafeRelease<Material>(ref this.materialMaskBlend);
		this.SafeRelease<Material>(ref this.materialDepthMask);
		this.SafeRelease<Material>(ref this.materialDepthMaskBlend);
		this.SafeRelease<Material>(ref this.materialProcessOnly);
	}

	// Token: 0x06000134 RID: 308 RVA: 0x00007AD8 File Offset: 0x00005CD8
	private Texture2D CreateDefaultLut()
	{
		this.defaultLut = new Texture2D(1024, 32, TextureFormat.RGB24, false, true)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.defaultLut.name = "DefaultLut";
		this.defaultLut.hideFlags = HideFlags.DontSave;
		this.defaultLut.anisoLevel = 1;
		this.defaultLut.filterMode = FilterMode.Bilinear;
		Color32[] array = new Color32[32768];
		for (int i = 0; i < 32; i++)
		{
			int num = i * 32;
			for (int j = 0; j < 32; j++)
			{
				int num2 = num + j * 1024;
				for (int k = 0; k < 32; k++)
				{
					float num3 = (float)k / 31f;
					float num4 = (float)j / 31f;
					float num5 = (float)i / 31f;
					byte r = (byte)(num3 * 255f);
					byte g = (byte)(num4 * 255f);
					byte b = (byte)(num5 * 255f);
					array[num2 + k] = new Color32(r, g, b, byte.MaxValue);
				}
			}
		}
		this.defaultLut.SetPixels32(array);
		this.defaultLut.Apply();
		return this.defaultLut;
	}

	// Token: 0x06000135 RID: 309 RVA: 0x00007C18 File Offset: 0x00005E18
	private Texture2D CreateDepthCurveLut()
	{
		this.SafeRelease<Texture2D>(ref this.depthCurveLut);
		this.depthCurveLut = new Texture2D(1024, 1, TextureFormat.Alpha8, false, true)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.depthCurveLut.name = "DepthCurveLut";
		this.depthCurveLut.hideFlags = HideFlags.DontSave;
		this.depthCurveLut.anisoLevel = 1;
		this.depthCurveLut.wrapMode = TextureWrapMode.Clamp;
		this.depthCurveLut.filterMode = FilterMode.Bilinear;
		this.depthCurveColors = new Color32[1024];
		return this.depthCurveLut;
	}

	// Token: 0x06000136 RID: 310 RVA: 0x00007CA8 File Offset: 0x00005EA8
	private void UpdateDepthCurveLut()
	{
		if (this.depthCurveLut == null)
		{
			this.CreateDepthCurveLut();
		}
		float num = 0f;
		int i = 0;
		while (i < 1024)
		{
			this.depthCurveColors[i].a = (byte)Mathf.FloorToInt(Mathf.Clamp01(this.DepthMaskCurve.Evaluate(num)) * 255f);
			i++;
			num += 0.0009775171f;
		}
		this.depthCurveLut.SetPixels32(this.depthCurveColors);
		this.depthCurveLut.Apply();
	}

	// Token: 0x06000137 RID: 311 RVA: 0x00007D3C File Offset: 0x00005F3C
	private void CheckUpdateDepthCurveLut()
	{
		bool flag = false;
		if (this.DepthMaskCurve.length != this.prevDepthMaskCurve.length)
		{
			flag = true;
		}
		else
		{
			float num = 0f;
			int i = 0;
			while (i < this.DepthMaskCurve.length)
			{
				if (Mathf.Abs(this.DepthMaskCurve.Evaluate(num) - this.prevDepthMaskCurve.Evaluate(num)) > 1E-45f)
				{
					flag = true;
					break;
				}
				i++;
				num += 0.0009775171f;
			}
		}
		if (this.depthCurveLut == null || flag)
		{
			this.UpdateDepthCurveLut();
			this.prevDepthMaskCurve = new AnimationCurve(this.DepthMaskCurve.keys);
		}
	}

	// Token: 0x06000138 RID: 312 RVA: 0x00007DFC File Offset: 0x00005FFC
	private void CreateHelperTextures()
	{
		this.ReleaseTextures();
		this.blendCacheLut = new RenderTexture(1024, 32, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.blendCacheLut.name = "BlendCacheLut";
		this.blendCacheLut.wrapMode = TextureWrapMode.Clamp;
		this.blendCacheLut.useMipMap = false;
		this.blendCacheLut.anisoLevel = 0;
		this.blendCacheLut.Create();
		this.midBlendLUT = new RenderTexture(1024, 32, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.midBlendLUT.name = "MidBlendLut";
		this.midBlendLUT.wrapMode = TextureWrapMode.Clamp;
		this.midBlendLUT.useMipMap = false;
		this.midBlendLUT.anisoLevel = 0;
		this.midBlendLUT.Create();
		this.CreateDefaultLut();
		if (this.UseDepthMask)
		{
			this.CreateDepthCurveLut();
		}
	}

	// Token: 0x06000139 RID: 313 RVA: 0x00007EE8 File Offset: 0x000060E8
	private bool CheckMaterialAndShader(Material material, string name)
	{
		if (material == null || material.shader == null)
		{
			Debug.LogWarning("[AmplifyColor] Error creating " + name + " material. Effect disabled.");
			base.enabled = false;
		}
		else if (!material.shader.isSupported)
		{
			Debug.LogWarning("[AmplifyColor] " + name + " shader not supported on this platform. Effect disabled.");
			base.enabled = false;
		}
		else
		{
			material.hideFlags = HideFlags.HideAndDontSave;
		}
		return base.enabled;
	}

	// Token: 0x0600013A RID: 314 RVA: 0x00007F74 File Offset: 0x00006174
	private bool CreateMaterials()
	{
		this.SetupShader();
		if (!this.CheckShaders())
		{
			return false;
		}
		this.ReleaseMaterials();
		this.materialBase = new Material(this.shaderBase);
		this.materialBlend = new Material(this.shaderBlend);
		this.materialBlendCache = new Material(this.shaderBlendCache);
		this.materialMask = new Material(this.shaderMask);
		this.materialMaskBlend = new Material(this.shaderMaskBlend);
		this.materialDepthMask = new Material(this.shaderDepthMask);
		this.materialDepthMaskBlend = new Material(this.shaderDepthMaskBlend);
		this.materialProcessOnly = new Material(this.shaderProcessOnly);
		bool flag = true;
		flag = (flag && this.CheckMaterialAndShader(this.materialBase, "BaseMaterial"));
		flag = (flag && this.CheckMaterialAndShader(this.materialBlend, "BlendMaterial"));
		flag = (flag && this.CheckMaterialAndShader(this.materialBlendCache, "BlendCacheMaterial"));
		flag = (flag && this.CheckMaterialAndShader(this.materialMask, "MaskMaterial"));
		flag = (flag && this.CheckMaterialAndShader(this.materialMaskBlend, "MaskBlendMaterial"));
		flag = (flag && this.CheckMaterialAndShader(this.materialDepthMask, "DepthMaskMaterial"));
		flag = (flag && this.CheckMaterialAndShader(this.materialDepthMaskBlend, "DepthMaskBlendMaterial"));
		if (!flag || !this.CheckMaterialAndShader(this.materialProcessOnly, "ProcessOnlyMaterial"))
		{
			return false;
		}
		this.CreateHelperTextures();
		return true;
	}

	// Token: 0x0600013B RID: 315 RVA: 0x0000810C File Offset: 0x0000630C
	private void SetMaterialKeyword(string keyword, bool state)
	{
		bool flag = this.materialBase.IsKeywordEnabled(keyword);
		if (state && !flag)
		{
			this.materialBase.EnableKeyword(keyword);
			this.materialBlend.EnableKeyword(keyword);
			this.materialBlendCache.EnableKeyword(keyword);
			this.materialMask.EnableKeyword(keyword);
			this.materialMaskBlend.EnableKeyword(keyword);
			this.materialDepthMask.EnableKeyword(keyword);
			this.materialDepthMaskBlend.EnableKeyword(keyword);
			this.materialProcessOnly.EnableKeyword(keyword);
		}
		else if (!state && this.materialBase.IsKeywordEnabled(keyword))
		{
			this.materialBase.DisableKeyword(keyword);
			this.materialBlend.DisableKeyword(keyword);
			this.materialBlendCache.DisableKeyword(keyword);
			this.materialMask.DisableKeyword(keyword);
			this.materialMaskBlend.DisableKeyword(keyword);
			this.materialDepthMask.DisableKeyword(keyword);
			this.materialDepthMaskBlend.DisableKeyword(keyword);
			this.materialProcessOnly.DisableKeyword(keyword);
		}
	}

	// Token: 0x0600013C RID: 316 RVA: 0x00008210 File Offset: 0x00006410
	private void UpdateShaderKeywords()
	{
		this.SetMaterialKeyword("AC_QUALITY_MOBILE", this.QualityLevel == Quality.Mobile);
		this.SetMaterialKeyword("AC_DITHERING", this.UseDithering);
		this.SetMaterialKeyword("AC_TONEMAPPING", this.UseToneMapping);
	}

	// Token: 0x0600013D RID: 317 RVA: 0x00008254 File Offset: 0x00006454
	private void SafeRelease<T>(ref T obj) where T : UnityEngine.Object
	{
		if (obj != null)
		{
			UnityEngine.Object.DestroyImmediate(obj);
			obj = (T)((object)null);
		}
	}

	// Token: 0x0600013E RID: 318 RVA: 0x00008294 File Offset: 0x00006494
	private void ReleaseTextures()
	{
		this.SafeRelease<RenderTexture>(ref this.blendCacheLut);
		this.SafeRelease<RenderTexture>(ref this.midBlendLUT);
		this.SafeRelease<Texture2D>(ref this.defaultLut);
		this.SafeRelease<Texture2D>(ref this.depthCurveLut);
	}

	// Token: 0x0600013F RID: 319 RVA: 0x000082D4 File Offset: 0x000064D4
	public static bool ValidateLutDimensions(Texture lut)
	{
		bool result = true;
		if (lut != null)
		{
			if (lut.width / lut.height != lut.height)
			{
				Debug.LogWarning("[AmplifyColor] Lut " + lut.name + " has invalid dimensions.");
				result = false;
			}
			else if (lut.anisoLevel != 0)
			{
				lut.anisoLevel = 0;
			}
		}
		return result;
	}

	// Token: 0x06000140 RID: 320 RVA: 0x0000833C File Offset: 0x0000653C
	private void UpdatePostEffectParams()
	{
		if (this.UseDepthMask)
		{
			this.CheckUpdateDepthCurveLut();
		}
		this.Exposure = Mathf.Max(this.Exposure, 0f);
	}

	// Token: 0x06000141 RID: 321 RVA: 0x00008368 File Offset: 0x00006568
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.BlendAmount = Mathf.Clamp01(this.BlendAmount);
		if (this.colorSpace != QualitySettings.activeColorSpace || this.qualityLevel != this.QualityLevel)
		{
			this.CreateMaterials();
		}
		this.UpdatePostEffectParams();
		this.UpdateShaderKeywords();
		bool flag = AmplifyColorBase.ValidateLutDimensions(this.LutTexture);
		bool flag2 = AmplifyColorBase.ValidateLutDimensions(this.LutBlendTexture);
		bool flag3 = this.LutTexture == null && this.LutBlendTexture == null && this.volumesLutBlendTexture == null;
		Texture texture = (!(this.LutTexture == null)) ? this.LutTexture : this.defaultLut;
		Texture lutBlendTexture = this.LutBlendTexture;
		int pass = ((this.colorSpace != ColorSpace.Linear) ? 0 : 2) + ((!this.ownerCamera.hdr) ? 0 : 1);
		bool flag4 = this.BlendAmount != 0f || this.blending;
		bool flag5 = flag4 || (flag4 && lutBlendTexture != null);
		bool flag6 = flag5;
		bool flag7 = !flag || !flag2 || flag3;
		Material material;
		if (flag7)
		{
			material = this.materialProcessOnly;
		}
		else if (flag5 || this.volumesBlending)
		{
			if (this.UseDepthMask)
			{
				material = this.materialDepthMaskBlend;
			}
			else
			{
				material = ((!(this.MaskTexture != null)) ? this.materialBlend : this.materialMaskBlend);
			}
		}
		else if (this.UseDepthMask)
		{
			material = this.materialDepthMask;
		}
		else
		{
			material = ((!(this.MaskTexture != null)) ? this.materialBase : this.materialMask);
		}
		material.SetFloat("_Exposure", this.Exposure);
		material.SetFloat("_LerpAmount", this.BlendAmount);
		if (this.MaskTexture != null)
		{
			material.SetTexture("_MaskTex", this.MaskTexture);
		}
		if (this.UseDepthMask)
		{
			material.SetTexture("_DepthCurveLut", this.depthCurveLut);
		}
		if (!flag7)
		{
			if (this.volumesBlending)
			{
				this.volumesBlendAmount = Mathf.Clamp01(this.volumesBlendAmount);
				this.materialBlendCache.SetFloat("_LerpAmount", this.volumesBlendAmount);
				if (this.blendingFromMidBlend)
				{
					this.materialBlendCache.SetTexture("_RgbTex", this.midBlendLUT);
				}
				else
				{
					this.materialBlendCache.SetTexture("_RgbTex", texture);
				}
				this.materialBlendCache.SetTexture("_LerpRgbTex", (!(this.volumesLutBlendTexture != null)) ? this.defaultLut : this.volumesLutBlendTexture);
				Graphics.Blit(texture, this.blendCacheLut, this.materialBlendCache);
			}
			if (flag6)
			{
				this.materialBlendCache.SetFloat("_LerpAmount", this.BlendAmount);
				RenderTexture renderTexture = null;
				if (this.volumesBlending)
				{
					renderTexture = RenderTexture.GetTemporary(this.blendCacheLut.width, this.blendCacheLut.height, this.blendCacheLut.depth, this.blendCacheLut.format, RenderTextureReadWrite.Linear);
					Graphics.Blit(this.blendCacheLut, renderTexture);
					this.materialBlendCache.SetTexture("_RgbTex", renderTexture);
				}
				else
				{
					this.materialBlendCache.SetTexture("_RgbTex", texture);
				}
				this.materialBlendCache.SetTexture("_LerpRgbTex", (!(lutBlendTexture != null)) ? this.defaultLut : lutBlendTexture);
				Graphics.Blit(texture, this.blendCacheLut, this.materialBlendCache);
				if (renderTexture != null)
				{
					RenderTexture.ReleaseTemporary(renderTexture);
				}
				material.SetTexture("_RgbBlendCacheTex", this.blendCacheLut);
			}
			else if (this.volumesBlending)
			{
				material.SetTexture("_RgbBlendCacheTex", this.blendCacheLut);
			}
			else
			{
				if (texture != null)
				{
					material.SetTexture("_RgbTex", texture);
				}
				if (lutBlendTexture != null)
				{
					material.SetTexture("_LerpRgbTex", lutBlendTexture);
				}
			}
		}
		Graphics.Blit(source, destination, material, pass);
		if (flag6 || this.volumesBlending)
		{
			this.blendCacheLut.DiscardContents();
		}
	}

	// Token: 0x0400013C RID: 316
	public const int LutSize = 32;

	// Token: 0x0400013D RID: 317
	public const int LutWidth = 1024;

	// Token: 0x0400013E RID: 318
	public const int LutHeight = 32;

	// Token: 0x0400013F RID: 319
	private const int DepthCurveLutRange = 1024;

	// Token: 0x04000140 RID: 320
	public float Exposure = 1f;

	// Token: 0x04000141 RID: 321
	public bool UseToneMapping;

	// Token: 0x04000142 RID: 322
	public bool UseDithering;

	// Token: 0x04000143 RID: 323
	public Quality QualityLevel = Quality.Standard;

	// Token: 0x04000144 RID: 324
	public float BlendAmount;

	// Token: 0x04000145 RID: 325
	public Texture LutTexture;

	// Token: 0x04000146 RID: 326
	public Texture LutBlendTexture;

	// Token: 0x04000147 RID: 327
	public Texture MaskTexture;

	// Token: 0x04000148 RID: 328
	public bool UseDepthMask;

	// Token: 0x04000149 RID: 329
	public AnimationCurve DepthMaskCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	// Token: 0x0400014A RID: 330
	public bool UseVolumes;

	// Token: 0x0400014B RID: 331
	public float ExitVolumeBlendTime = 1f;

	// Token: 0x0400014C RID: 332
	public Transform TriggerVolumeProxy;

	// Token: 0x0400014D RID: 333
	public LayerMask VolumeCollisionMask = -1;

	// Token: 0x0400014E RID: 334
	private Camera ownerCamera;

	// Token: 0x0400014F RID: 335
	private Shader shaderBase;

	// Token: 0x04000150 RID: 336
	private Shader shaderBlend;

	// Token: 0x04000151 RID: 337
	private Shader shaderBlendCache;

	// Token: 0x04000152 RID: 338
	private Shader shaderMask;

	// Token: 0x04000153 RID: 339
	private Shader shaderMaskBlend;

	// Token: 0x04000154 RID: 340
	private Shader shaderDepthMask;

	// Token: 0x04000155 RID: 341
	private Shader shaderDepthMaskBlend;

	// Token: 0x04000156 RID: 342
	private Shader shaderProcessOnly;

	// Token: 0x04000157 RID: 343
	private RenderTexture blendCacheLut;

	// Token: 0x04000158 RID: 344
	private Texture2D defaultLut;

	// Token: 0x04000159 RID: 345
	private Texture2D depthCurveLut;

	// Token: 0x0400015A RID: 346
	private Color32[] depthCurveColors;

	// Token: 0x0400015B RID: 347
	private ColorSpace colorSpace = ColorSpace.Uninitialized;

	// Token: 0x0400015C RID: 348
	private Quality qualityLevel = Quality.Standard;

	// Token: 0x0400015D RID: 349
	private Material materialBase;

	// Token: 0x0400015E RID: 350
	private Material materialBlend;

	// Token: 0x0400015F RID: 351
	private Material materialBlendCache;

	// Token: 0x04000160 RID: 352
	private Material materialMask;

	// Token: 0x04000161 RID: 353
	private Material materialMaskBlend;

	// Token: 0x04000162 RID: 354
	private Material materialDepthMask;

	// Token: 0x04000163 RID: 355
	private Material materialDepthMaskBlend;

	// Token: 0x04000164 RID: 356
	private Material materialProcessOnly;

	// Token: 0x04000165 RID: 357
	private bool blending;

	// Token: 0x04000166 RID: 358
	private float blendingTime;

	// Token: 0x04000167 RID: 359
	private float blendingTimeCountdown;

	// Token: 0x04000168 RID: 360
	private Action onFinishBlend;

	// Token: 0x04000169 RID: 361
	private AnimationCurve prevDepthMaskCurve = new AnimationCurve();

	// Token: 0x0400016A RID: 362
	private bool volumesBlending;

	// Token: 0x0400016B RID: 363
	private float volumesBlendingTime;

	// Token: 0x0400016C RID: 364
	private float volumesBlendingTimeCountdown;

	// Token: 0x0400016D RID: 365
	private Texture volumesLutBlendTexture;

	// Token: 0x0400016E RID: 366
	private float volumesBlendAmount;

	// Token: 0x0400016F RID: 367
	private Texture worldLUT;

	// Token: 0x04000170 RID: 368
	private AmplifyColorVolumeBase currentVolumeLut;

	// Token: 0x04000171 RID: 369
	private RenderTexture midBlendLUT;

	// Token: 0x04000172 RID: 370
	private bool blendingFromMidBlend;

	// Token: 0x04000173 RID: 371
	private VolumeEffect worldVolumeEffects;

	// Token: 0x04000174 RID: 372
	private VolumeEffect currentVolumeEffects;

	// Token: 0x04000175 RID: 373
	private VolumeEffect blendVolumeEffects;

	// Token: 0x04000176 RID: 374
	private float worldExposure = 1f;

	// Token: 0x04000177 RID: 375
	private float currentExposure = 1f;

	// Token: 0x04000178 RID: 376
	private float blendExposure = 1f;

	// Token: 0x04000179 RID: 377
	private float effectVolumesBlendAdjust;

	// Token: 0x0400017A RID: 378
	private List<AmplifyColorVolumeBase> enteredVolumes = new List<AmplifyColorVolumeBase>();

	// Token: 0x0400017B RID: 379
	private AmplifyColorTriggerProxyBase actualTriggerProxy;

	// Token: 0x0400017C RID: 380
	[HideInInspector]
	public VolumeEffectFlags EffectFlags = new VolumeEffectFlags();

	// Token: 0x0400017D RID: 381
	[SerializeField]
	[HideInInspector]
	private string sharedInstanceID = string.Empty;
}
