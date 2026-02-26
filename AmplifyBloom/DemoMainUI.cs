using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AmplifyBloom
{
	// Token: 0x0200001C RID: 28
	public class DemoMainUI : MonoBehaviour
	{
		// Token: 0x060000FF RID: 255 RVA: 0x0000646C File Offset: 0x0000466C
		private void Awake()
		{
			this._camera = Camera.main;
			this._amplifyBloomEffect = this._camera.GetComponent<AmplifyBloomEffect>();
			this.BloomToggle.isOn = this._amplifyBloomEffect.enabled;
			this.HighPrecision.isOn = this._amplifyBloomEffect.HighPrecision;
			this.UpscaleType.isOn = (this._amplifyBloomEffect.UpscaleQuality == UpscaleQualityEnum.Realistic);
			this.TemporalFilter.isOn = this._amplifyBloomEffect.TemporalFilteringActive;
			this.BokehToggle.isOn = this._amplifyBloomEffect.BokehFilterInstance.ApplyBokeh;
			this.LensFlareToggle.isOn = this._amplifyBloomEffect.LensFlareInstance.ApplyLensFlare;
			this.LensGlareToggle.isOn = this._amplifyBloomEffect.LensGlareInstance.ApplyLensGlare;
			this.LensDirtToggle.isOn = this._amplifyBloomEffect.ApplyLensDirt;
			this.LensStarburstToggle.isOn = this._amplifyBloomEffect.ApplyLensStardurst;
			this.BloomToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnBloomToggle));
			this.HighPrecision.onValueChanged.AddListener(new UnityAction<bool>(this.OnHighPrecisionToggle));
			this.UpscaleType.onValueChanged.AddListener(new UnityAction<bool>(this.OnUpscaleTypeToogle));
			this.TemporalFilter.onValueChanged.AddListener(new UnityAction<bool>(this.OnTemporalFilterToggle));
			this.BokehToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnBokehFilterToggle));
			this.LensFlareToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnLensFlareToggle));
			this.LensGlareToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnLensGlareToggle));
			this.LensDirtToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnLensDirtToggle));
			this.LensStarburstToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnLensStarburstToggle));
			this.ThresholdSlider.value = this._amplifyBloomEffect.OverallThreshold;
			this.ThresholdSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnThresholdSlider));
			this.DownscaleAmountSlider.value = (float)this._amplifyBloomEffect.BloomDownsampleCount;
			this.DownscaleAmountSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnDownscaleAmount));
			this.IntensitySlider.value = this._amplifyBloomEffect.OverallIntensity;
			this.IntensitySlider.onValueChanged.AddListener(new UnityAction<float>(this.OnIntensitySlider));
			this.ThresholdSizeSlider.value = (float)this._amplifyBloomEffect.MainThresholdSize;
			this.ThresholdSizeSlider.onValueChanged.AddListener(new UnityAction<float>(this.OnThresholdSize));
			if (Input.GetJoystickNames().Length > 0)
			{
				this.m_gamePadMode = true;
				this.m_uiElements = new DemoUIElement[13];
				this.m_uiElements[0] = this.BloomToggle.GetComponent<DemoUIElement>();
				this.m_uiElements[1] = this.HighPrecision.GetComponent<DemoUIElement>();
				this.m_uiElements[2] = this.UpscaleType.GetComponent<DemoUIElement>();
				this.m_uiElements[3] = this.TemporalFilter.GetComponent<DemoUIElement>();
				this.m_uiElements[4] = this.BokehToggle.GetComponent<DemoUIElement>();
				this.m_uiElements[5] = this.LensFlareToggle.GetComponent<DemoUIElement>();
				this.m_uiElements[6] = this.LensGlareToggle.GetComponent<DemoUIElement>();
				this.m_uiElements[7] = this.LensDirtToggle.GetComponent<DemoUIElement>();
				this.m_uiElements[8] = this.LensStarburstToggle.GetComponent<DemoUIElement>();
				this.m_uiElements[9] = this.ThresholdSlider.GetComponent<DemoUIElement>();
				this.m_uiElements[10] = this.DownscaleAmountSlider.GetComponent<DemoUIElement>();
				this.m_uiElements[11] = this.IntensitySlider.GetComponent<DemoUIElement>();
				this.m_uiElements[12] = this.ThresholdSizeSlider.GetComponent<DemoUIElement>();
				for (int i = 0; i < this.m_uiElements.Length; i++)
				{
					this.m_uiElements[i].Init();
				}
				this.m_uiElements[this.m_currentOption].Select = true;
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00006888 File Offset: 0x00004A88
		public void OnThresholdSize(float selection)
		{
			this._amplifyBloomEffect.MainThresholdSize = (MainThresholdSizeEnum)selection;
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00006898 File Offset: 0x00004A98
		public void OnThresholdSlider(float value)
		{
			this._amplifyBloomEffect.OverallThreshold = value;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x000068A8 File Offset: 0x00004AA8
		public void OnDownscaleAmount(float value)
		{
			this._amplifyBloomEffect.BloomDownsampleCount = (int)value;
		}

		// Token: 0x06000103 RID: 259 RVA: 0x000068B8 File Offset: 0x00004AB8
		public void OnIntensitySlider(float value)
		{
			this._amplifyBloomEffect.OverallIntensity = value;
		}

		// Token: 0x06000104 RID: 260 RVA: 0x000068C8 File Offset: 0x00004AC8
		public void OnBloomToggle(bool value)
		{
			this._amplifyBloomEffect.enabled = this.BloomToggle.isOn;
		}

		// Token: 0x06000105 RID: 261 RVA: 0x000068E0 File Offset: 0x00004AE0
		public void OnHighPrecisionToggle(bool value)
		{
			this._amplifyBloomEffect.HighPrecision = value;
		}

		// Token: 0x06000106 RID: 262 RVA: 0x000068F0 File Offset: 0x00004AF0
		public void OnUpscaleTypeToogle(bool value)
		{
			this._amplifyBloomEffect.UpscaleQuality = ((!value) ? UpscaleQualityEnum.Natural : UpscaleQualityEnum.Realistic);
		}

		// Token: 0x06000107 RID: 263 RVA: 0x0000690C File Offset: 0x00004B0C
		public void OnTemporalFilterToggle(bool value)
		{
			this._amplifyBloomEffect.TemporalFilteringActive = value;
		}

		// Token: 0x06000108 RID: 264 RVA: 0x0000691C File Offset: 0x00004B1C
		public void OnBokehFilterToggle(bool value)
		{
			this._amplifyBloomEffect.BokehFilterInstance.ApplyBokeh = this.BokehToggle.isOn;
		}

		// Token: 0x06000109 RID: 265 RVA: 0x0000693C File Offset: 0x00004B3C
		public void OnLensFlareToggle(bool value)
		{
			this._amplifyBloomEffect.LensFlareInstance.ApplyLensFlare = this.LensFlareToggle.isOn;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x0000695C File Offset: 0x00004B5C
		public void OnLensGlareToggle(bool value)
		{
			this._amplifyBloomEffect.LensGlareInstance.ApplyLensGlare = this.LensGlareToggle.isOn;
		}

		// Token: 0x0600010B RID: 267 RVA: 0x0000697C File Offset: 0x00004B7C
		public void OnLensDirtToggle(bool value)
		{
			this._amplifyBloomEffect.ApplyLensDirt = this.LensDirtToggle.isOn;
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00006994 File Offset: 0x00004B94
		public void OnLensStarburstToggle(bool value)
		{
			this._amplifyBloomEffect.ApplyLensStardurst = this.LensStarburstToggle.isOn;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x000069AC File Offset: 0x00004BAC
		public void OnQuitButton()
		{
			Application.Quit();
		}

		// Token: 0x0600010E RID: 270 RVA: 0x000069B4 File Offset: 0x00004BB4
		private void Update()
		{
			if (this.m_gamePadMode)
			{
				int num = (int)Input.GetAxis("Vertical");
				if (num != this.m_lastAxisValue)
				{
					this.m_lastAxisValue = num;
					if (num == 1)
					{
						this.m_currentOption = (this.m_currentOption + 1) % this.m_uiElements.Length;
					}
					else if (num == -1)
					{
						this.m_currentOption = ((this.m_currentOption != 0) ? (this.m_currentOption - 1) : (this.m_uiElements.Length - 1));
					}
					this.m_uiElements[this.m_lastOption].Select = false;
					this.m_uiElements[this.m_currentOption].Select = true;
					this.m_lastOption = this.m_currentOption;
				}
				if (Input.GetButtonDown("Submit"))
				{
					this.m_uiElements[this.m_currentOption].DoAction(DemoUIElementAction.Press, new object[0]);
				}
				float axis = Input.GetAxis("Horizontal");
				if (Mathf.Abs(axis) > 0f)
				{
					this.m_uiElements[this.m_currentOption].DoAction(DemoUIElementAction.Slide, new object[]
					{
						axis * Time.deltaTime
					});
				}
				else
				{
					this.m_uiElements[this.m_currentOption].Idle();
				}
			}
			if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.Q))
			{
				this.OnQuitButton();
			}
			if (Input.GetKeyDown(KeyCode.Alpha0))
			{
				this._camera.orthographic = !this._camera.orthographic;
			}
			bool flag;
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				Behaviour amplifyBloomEffect = this._amplifyBloomEffect;
				flag = !this.BloomToggle.isOn;
				this.BloomToggle.isOn = flag;
				amplifyBloomEffect.enabled = flag;
			}
			Selectable bokehToggle = this.BokehToggle;
			flag = this.BloomToggle.isOn;
			this.IntensitySlider.interactable = flag;
			flag = flag;
			this.HighPrecision.interactable = flag;
			flag = flag;
			this.DownscaleAmountSlider.interactable = flag;
			flag = flag;
			this.ThresholdSlider.interactable = flag;
			flag = flag;
			this.LensStarburstToggle.interactable = flag;
			flag = flag;
			this.LensDirtToggle.interactable = flag;
			flag = flag;
			this.LensGlareToggle.interactable = flag;
			flag = flag;
			this.LensFlareToggle.interactable = flag;
			bokehToggle.interactable = flag;
			if (this.BloomToggle.isOn)
			{
				if (Input.GetKeyDown(KeyCode.Alpha2))
				{
					AmplifyBokeh bokehFilterInstance = this._amplifyBloomEffect.BokehFilterInstance;
					flag = !this.BokehToggle.isOn;
					this.BokehToggle.isOn = flag;
					bokehFilterInstance.ApplyBokeh = flag;
				}
				if (Input.GetKeyDown(KeyCode.Alpha3))
				{
					AmplifyLensFlare lensFlareInstance = this._amplifyBloomEffect.LensFlareInstance;
					flag = !this.LensFlareToggle.isOn;
					this.LensFlareToggle.isOn = flag;
					lensFlareInstance.ApplyLensFlare = flag;
				}
				if (Input.GetKeyDown(KeyCode.Alpha4))
				{
					AmplifyGlare lensGlareInstance = this._amplifyBloomEffect.LensGlareInstance;
					flag = !this.LensGlareToggle.isOn;
					this.LensGlareToggle.isOn = flag;
					lensGlareInstance.ApplyLensGlare = flag;
				}
				if (Input.GetKeyDown(KeyCode.Alpha5))
				{
					AmplifyBloomBase amplifyBloomEffect2 = this._amplifyBloomEffect;
					flag = !this.LensDirtToggle.isOn;
					this.LensDirtToggle.isOn = flag;
					amplifyBloomEffect2.ApplyLensDirt = flag;
				}
				if (Input.GetKeyDown(KeyCode.Alpha6))
				{
					AmplifyBloomBase amplifyBloomEffect3 = this._amplifyBloomEffect;
					flag = !this.LensStarburstToggle.isOn;
					this.LensStarburstToggle.isOn = flag;
					amplifyBloomEffect3.ApplyLensStardurst = flag;
				}
			}
		}

		// Token: 0x04000117 RID: 279
		private const string VERTICAL_GAMEPAD = "Vertical";

		// Token: 0x04000118 RID: 280
		private const string HORIZONTAL_GAMEPAD = "Horizontal";

		// Token: 0x04000119 RID: 281
		private const string SUBMIT_BUTTON = "Submit";

		// Token: 0x0400011A RID: 282
		public Toggle BloomToggle;

		// Token: 0x0400011B RID: 283
		public Toggle HighPrecision;

		// Token: 0x0400011C RID: 284
		public Toggle UpscaleType;

		// Token: 0x0400011D RID: 285
		public Toggle TemporalFilter;

		// Token: 0x0400011E RID: 286
		public Toggle BokehToggle;

		// Token: 0x0400011F RID: 287
		public Toggle LensFlareToggle;

		// Token: 0x04000120 RID: 288
		public Toggle LensGlareToggle;

		// Token: 0x04000121 RID: 289
		public Toggle LensDirtToggle;

		// Token: 0x04000122 RID: 290
		public Toggle LensStarburstToggle;

		// Token: 0x04000123 RID: 291
		public Slider ThresholdSlider;

		// Token: 0x04000124 RID: 292
		public Slider DownscaleAmountSlider;

		// Token: 0x04000125 RID: 293
		public Slider IntensitySlider;

		// Token: 0x04000126 RID: 294
		public Slider ThresholdSizeSlider;

		// Token: 0x04000127 RID: 295
		private AmplifyBloomEffect _amplifyBloomEffect;

		// Token: 0x04000128 RID: 296
		private Camera _camera;

		// Token: 0x04000129 RID: 297
		private DemoUIElement[] m_uiElements;

		// Token: 0x0400012A RID: 298
		private bool m_gamePadMode;

		// Token: 0x0400012B RID: 299
		private int m_currentOption;

		// Token: 0x0400012C RID: 300
		private int m_lastOption;

		// Token: 0x0400012D RID: 301
		private int m_lastAxisValue;
	}
}
