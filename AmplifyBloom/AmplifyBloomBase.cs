using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace AmplifyBloom
{
	// Token: 0x02000007 RID: 7
	[AddComponentMenu("")]
	[Serializable]
	public class AmplifyBloomBase : MonoBehaviour
	{
		// Token: 0x06000002 RID: 2 RVA: 0x0000225C File Offset: 0x0000045C
		private void Awake()
		{
			bool flag = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
			if (flag)
			{
				AmplifyUtils.DebugLog("Null graphics device detected. Skipping effect silently.", LogType.Error);
				this.silentError = true;
				return;
			}
			if (!AmplifyUtils.IsInitialized)
			{
				AmplifyUtils.InitializeIds();
			}
			for (int i = 0; i < 6; i++)
			{
				this.m_tempDownsamplesSizes[i] = new Vector2(0f, 0f);
			}
			this.m_cameraTransform = base.transform;
			this.m_tempFilterBuffer = null;
			this.m_starburstMat = Matrix4x4.identity;
			if (this.m_temporalFilteringCurve == null)
			{
				this.m_temporalFilteringCurve = new AnimationCurve(new Keyframe[]
				{
					new Keyframe(0f, 0f),
					new Keyframe(1f, 0.999f)
				});
			}
			this.m_bloomShader = Shader.Find("Hidden/AmplifyBloom");
			if (this.m_bloomShader != null)
			{
				this.m_bloomMaterial = new Material(this.m_bloomShader);
				this.m_bloomMaterial.hideFlags = HideFlags.DontSave;
			}
			else
			{
				AmplifyUtils.DebugLog("Main Bloom shader not found", LogType.Error);
				base.gameObject.SetActive(false);
			}
			this.m_finalCompositionShader = Shader.Find("Hidden/BloomFinal");
			if (this.m_finalCompositionShader != null)
			{
				this.m_finalCompositionMaterial = new Material(this.m_finalCompositionShader);
				if (!this.m_finalCompositionMaterial.GetTag(AmplifyUtils.ShaderModeTag, false).Equals(AmplifyUtils.ShaderModeValue))
				{
					if (this.m_showDebugMessages)
					{
						AmplifyUtils.DebugLog("Amplify Bloom is running on a limited hardware and may lead to a decrease on its visual quality.", LogType.Warning);
					}
				}
				else
				{
					this.m_softMaxdownscales = 6;
				}
				this.m_finalCompositionMaterial.hideFlags = HideFlags.DontSave;
				if (this.m_lensDirtTexture == null)
				{
					this.m_lensDirtTexture = this.m_finalCompositionMaterial.GetTexture(AmplifyUtils.LensDirtRTId);
				}
				if (this.m_lensStardurstTex == null)
				{
					this.m_lensStardurstTex = this.m_finalCompositionMaterial.GetTexture(AmplifyUtils.LensStarburstRTId);
				}
			}
			else
			{
				AmplifyUtils.DebugLog("Bloom Composition shader not found", LogType.Error);
				base.gameObject.SetActive(false);
			}
			this.m_camera = base.GetComponent<Camera>();
			this.m_camera.depthTextureMode |= DepthTextureMode.Depth;
			this.m_lensFlare.CreateLUTexture();
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000024AC File Offset: 0x000006AC
		private void OnDestroy()
		{
			if (this.m_bokehFilter != null)
			{
				this.m_bokehFilter.Destroy();
				this.m_bokehFilter = null;
			}
			if (this.m_anamorphicGlare != null)
			{
				this.m_anamorphicGlare.Destroy();
				this.m_anamorphicGlare = null;
			}
			if (this.m_lensFlare != null)
			{
				this.m_lensFlare.Destroy();
				this.m_lensFlare = null;
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002510 File Offset: 0x00000710
		private void ApplyGaussianBlur(RenderTexture renderTexture, int amount, float radius = 1f, bool applyTemporal = false)
		{
			if (amount == 0)
			{
				return;
			}
			this.m_bloomMaterial.SetFloat(AmplifyUtils.BlurRadiusId, radius);
			RenderTexture tempRenderTarget = AmplifyUtils.GetTempRenderTarget(renderTexture.width, renderTexture.height);
			for (int i = 0; i < amount; i++)
			{
				tempRenderTarget.DiscardContents();
				Graphics.Blit(renderTexture, tempRenderTarget, this.m_bloomMaterial, 14);
				if (this.m_temporalFilteringActive && applyTemporal && i == amount - 1)
				{
					if (this.m_tempFilterBuffer != null && this.m_temporalFilteringActive)
					{
						float value = this.m_temporalFilteringCurve.Evaluate(this.m_temporalFilteringValue);
						this.m_bloomMaterial.SetFloat(AmplifyUtils.TempFilterValueId, value);
						this.m_bloomMaterial.SetTexture(AmplifyUtils.AnamorphicRTS[0], this.m_tempFilterBuffer);
						renderTexture.DiscardContents();
						Graphics.Blit(tempRenderTarget, renderTexture, this.m_bloomMaterial, 16);
					}
					else
					{
						renderTexture.DiscardContents();
						Graphics.Blit(tempRenderTarget, renderTexture, this.m_bloomMaterial, 15);
					}
					bool flag = false;
					if (this.m_tempFilterBuffer != null)
					{
						if (this.m_tempFilterBuffer.format != renderTexture.format || this.m_tempFilterBuffer.width != renderTexture.width || this.m_tempFilterBuffer.height != renderTexture.height)
						{
							this.CleanTempFilterRT();
							flag = true;
						}
					}
					else
					{
						flag = true;
					}
					if (flag)
					{
						this.CreateTempFilterRT(renderTexture);
					}
					this.m_tempFilterBuffer.DiscardContents();
					Graphics.Blit(renderTexture, this.m_tempFilterBuffer);
				}
				else
				{
					renderTexture.DiscardContents();
					Graphics.Blit(tempRenderTarget, renderTexture, this.m_bloomMaterial, 15);
				}
			}
			AmplifyUtils.ReleaseTempRenderTarget(tempRenderTarget);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000026B8 File Offset: 0x000008B8
		private void CreateTempFilterRT(RenderTexture source)
		{
			if (this.m_tempFilterBuffer != null)
			{
				this.CleanTempFilterRT();
			}
			this.m_tempFilterBuffer = new RenderTexture(source.width, source.height, 0, source.format, AmplifyUtils.CurrentReadWriteMode);
			this.m_tempFilterBuffer.filterMode = AmplifyUtils.CurrentFilterMode;
			this.m_tempFilterBuffer.wrapMode = AmplifyUtils.CurrentWrapMode;
			this.m_tempFilterBuffer.Create();
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000272C File Offset: 0x0000092C
		private void CleanTempFilterRT()
		{
			if (this.m_tempFilterBuffer != null)
			{
				RenderTexture.active = null;
				this.m_tempFilterBuffer.Release();
				UnityEngine.Object.DestroyImmediate(this.m_tempFilterBuffer);
				this.m_tempFilterBuffer = null;
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002770 File Offset: 0x00000970
		private void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			if (this.silentError)
			{
				return;
			}
			if (!AmplifyUtils.IsInitialized)
			{
				AmplifyUtils.InitializeIds();
			}
			if (this.m_highPrecision)
			{
				AmplifyUtils.EnsureKeywordEnabled(this.m_bloomMaterial, AmplifyUtils.HighPrecisionKeyword, true);
				AmplifyUtils.EnsureKeywordEnabled(this.m_finalCompositionMaterial, AmplifyUtils.HighPrecisionKeyword, true);
				AmplifyUtils.CurrentRTFormat = RenderTextureFormat.DefaultHDR;
			}
			else
			{
				AmplifyUtils.EnsureKeywordEnabled(this.m_bloomMaterial, AmplifyUtils.HighPrecisionKeyword, false);
				AmplifyUtils.EnsureKeywordEnabled(this.m_finalCompositionMaterial, AmplifyUtils.HighPrecisionKeyword, false);
				AmplifyUtils.CurrentRTFormat = RenderTextureFormat.Default;
			}
			float num = Mathf.Acos(Vector3.Dot(this.m_cameraTransform.right, Vector3.right));
			if (Vector3.Cross(this.m_cameraTransform.right, Vector3.right).y > 0f)
			{
				num = -num;
			}
			RenderTexture renderTexture = null;
			RenderTexture renderTexture2 = null;
			if (!this.m_highPrecision)
			{
				this.m_bloomRange.y = 1f / this.m_bloomRange.x;
				this.m_bloomMaterial.SetVector(AmplifyUtils.BloomRangeId, this.m_bloomRange);
				this.m_finalCompositionMaterial.SetVector(AmplifyUtils.BloomRangeId, this.m_bloomRange);
			}
			this.m_bloomParams.y = this.m_overallThreshold;
			this.m_bloomMaterial.SetVector(AmplifyUtils.BloomParamsId, this.m_bloomParams);
			this.m_finalCompositionMaterial.SetVector(AmplifyUtils.BloomParamsId, this.m_bloomParams);
			int num2 = 1;
			MainThresholdSizeEnum mainThresholdSize = this.m_mainThresholdSize;
			if (mainThresholdSize != MainThresholdSizeEnum.Half)
			{
				if (mainThresholdSize == MainThresholdSizeEnum.Quarter)
				{
					num2 = 4;
				}
			}
			else
			{
				num2 = 2;
			}
			RenderTexture tempRenderTarget = AmplifyUtils.GetTempRenderTarget(src.width / num2, src.height / num2);
			if (this.m_maskTexture != null)
			{
				this.m_bloomMaterial.SetTexture(AmplifyUtils.MaskTextureId, this.m_maskTexture);
				Graphics.Blit(src, tempRenderTarget, this.m_bloomMaterial, 1);
			}
			else
			{
				Graphics.Blit(src, tempRenderTarget, this.m_bloomMaterial, 0);
			}
			if (this.m_debugToScreen == DebugToScreenEnum.MainThreshold)
			{
				Graphics.Blit(tempRenderTarget, dest, this.m_bloomMaterial, 33);
				AmplifyUtils.ReleaseAllRT();
				return;
			}
			bool flag = true;
			RenderTexture renderTexture3 = tempRenderTarget;
			if (this.m_bloomDownsampleCount > 0)
			{
				flag = false;
				int num3 = tempRenderTarget.width;
				int num4 = tempRenderTarget.height;
				for (int i = 0; i < this.m_bloomDownsampleCount; i++)
				{
					this.m_tempDownsamplesSizes[i].x = (float)num3;
					this.m_tempDownsamplesSizes[i].y = (float)num4;
					num3 = num3 + 1 >> 1;
					num4 = num4 + 1 >> 1;
					this.m_tempAuxDownsampleRTs[i] = AmplifyUtils.GetTempRenderTarget(num3, num4);
					if (i == 0)
					{
						if (!this.m_temporalFilteringActive || this.m_gaussianSteps[i] != 0)
						{
							if (this.m_upscaleQuality == UpscaleQualityEnum.Realistic)
							{
								Graphics.Blit(renderTexture3, this.m_tempAuxDownsampleRTs[i], this.m_bloomMaterial, 10);
							}
							else
							{
								Graphics.Blit(renderTexture3, this.m_tempAuxDownsampleRTs[i], this.m_bloomMaterial, 11);
							}
						}
						else
						{
							if (this.m_tempFilterBuffer != null && this.m_temporalFilteringActive)
							{
								float value = this.m_temporalFilteringCurve.Evaluate(this.m_temporalFilteringValue);
								this.m_bloomMaterial.SetFloat(AmplifyUtils.TempFilterValueId, value);
								this.m_bloomMaterial.SetTexture(AmplifyUtils.AnamorphicRTS[0], this.m_tempFilterBuffer);
								if (this.m_upscaleQuality == UpscaleQualityEnum.Realistic)
								{
									Graphics.Blit(renderTexture3, this.m_tempAuxDownsampleRTs[i], this.m_bloomMaterial, 12);
								}
								else
								{
									Graphics.Blit(renderTexture3, this.m_tempAuxDownsampleRTs[i], this.m_bloomMaterial, 13);
								}
							}
							else if (this.m_upscaleQuality == UpscaleQualityEnum.Realistic)
							{
								Graphics.Blit(renderTexture3, this.m_tempAuxDownsampleRTs[i], this.m_bloomMaterial, 10);
							}
							else
							{
								Graphics.Blit(renderTexture3, this.m_tempAuxDownsampleRTs[i], this.m_bloomMaterial, 11);
							}
							bool flag2 = false;
							if (this.m_tempFilterBuffer != null)
							{
								if (this.m_tempFilterBuffer.format != this.m_tempAuxDownsampleRTs[i].format || this.m_tempFilterBuffer.width != this.m_tempAuxDownsampleRTs[i].width || this.m_tempFilterBuffer.height != this.m_tempAuxDownsampleRTs[i].height)
								{
									this.CleanTempFilterRT();
									flag2 = true;
								}
							}
							else
							{
								flag2 = true;
							}
							if (flag2)
							{
								this.CreateTempFilterRT(this.m_tempAuxDownsampleRTs[i]);
							}
							this.m_tempFilterBuffer.DiscardContents();
							Graphics.Blit(this.m_tempAuxDownsampleRTs[i], this.m_tempFilterBuffer);
							if (this.m_debugToScreen == DebugToScreenEnum.TemporalFilter)
							{
								Graphics.Blit(this.m_tempAuxDownsampleRTs[i], dest);
								AmplifyUtils.ReleaseAllRT();
								return;
							}
						}
					}
					else
					{
						Graphics.Blit(this.m_tempAuxDownsampleRTs[i - 1], this.m_tempAuxDownsampleRTs[i], this.m_bloomMaterial, 9);
					}
					if (this.m_gaussianSteps[i] > 0)
					{
						this.ApplyGaussianBlur(this.m_tempAuxDownsampleRTs[i], this.m_gaussianSteps[i], this.m_gaussianRadius[i], i == 0);
						if (this.m_temporalFilteringActive && this.m_debugToScreen == DebugToScreenEnum.TemporalFilter)
						{
							Graphics.Blit(this.m_tempAuxDownsampleRTs[i], dest);
							AmplifyUtils.ReleaseAllRT();
							return;
						}
					}
				}
				renderTexture3 = this.m_tempAuxDownsampleRTs[this.m_featuresSourceId];
				AmplifyUtils.ReleaseTempRenderTarget(tempRenderTarget);
			}
			if (this.m_bokehFilter.ApplyBokeh && this.m_bokehFilter.ApplyOnBloomSource)
			{
				this.m_bokehFilter.ApplyBokehFilter(renderTexture3, this.m_bloomMaterial);
				if (this.m_debugToScreen == DebugToScreenEnum.BokehFilter)
				{
					Graphics.Blit(renderTexture3, dest);
					AmplifyUtils.ReleaseAllRT();
					return;
				}
			}
			bool flag3 = false;
			RenderTexture renderTexture4;
			if (this.m_separateFeaturesThreshold)
			{
				this.m_bloomParams.y = this.m_featuresThreshold;
				this.m_bloomMaterial.SetVector(AmplifyUtils.BloomParamsId, this.m_bloomParams);
				this.m_finalCompositionMaterial.SetVector(AmplifyUtils.BloomParamsId, this.m_bloomParams);
				renderTexture4 = AmplifyUtils.GetTempRenderTarget(renderTexture3.width, renderTexture3.height);
				flag3 = true;
				Graphics.Blit(renderTexture3, renderTexture4, this.m_bloomMaterial, 0);
				if (this.m_debugToScreen == DebugToScreenEnum.FeaturesThreshold)
				{
					Graphics.Blit(renderTexture4, dest);
					AmplifyUtils.ReleaseAllRT();
					return;
				}
			}
			else
			{
				renderTexture4 = renderTexture3;
			}
			if (this.m_bokehFilter.ApplyBokeh && !this.m_bokehFilter.ApplyOnBloomSource)
			{
				if (!flag3)
				{
					flag3 = true;
					renderTexture4 = AmplifyUtils.GetTempRenderTarget(renderTexture3.width, renderTexture3.height);
					Graphics.Blit(renderTexture3, renderTexture4);
				}
				this.m_bokehFilter.ApplyBokehFilter(renderTexture4, this.m_bloomMaterial);
				if (this.m_debugToScreen == DebugToScreenEnum.BokehFilter)
				{
					Graphics.Blit(renderTexture4, dest);
					AmplifyUtils.ReleaseAllRT();
					return;
				}
			}
			if (this.m_lensFlare.ApplyLensFlare && this.m_debugToScreen != DebugToScreenEnum.Bloom)
			{
				renderTexture = this.m_lensFlare.ApplyFlare(this.m_bloomMaterial, renderTexture4);
				this.ApplyGaussianBlur(renderTexture, this.m_lensFlare.LensFlareGaussianBlurAmount, 1f, false);
				if (this.m_debugToScreen == DebugToScreenEnum.LensFlare)
				{
					Graphics.Blit(renderTexture, dest);
					AmplifyUtils.ReleaseAllRT();
					return;
				}
			}
			if (this.m_anamorphicGlare.ApplyLensGlare && this.m_debugToScreen != DebugToScreenEnum.Bloom)
			{
				renderTexture2 = AmplifyUtils.GetTempRenderTarget(renderTexture3.width, renderTexture3.height);
				this.m_anamorphicGlare.OnRenderImage(this.m_bloomMaterial, renderTexture4, renderTexture2, num);
				if (this.m_debugToScreen == DebugToScreenEnum.LensGlare)
				{
					Graphics.Blit(renderTexture2, dest);
					AmplifyUtils.ReleaseAllRT();
					return;
				}
			}
			if (flag3)
			{
				AmplifyUtils.ReleaseTempRenderTarget(renderTexture4);
			}
			if (flag)
			{
				this.ApplyGaussianBlur(renderTexture3, this.m_gaussianSteps[0], this.m_gaussianRadius[0], false);
			}
			if (this.m_bloomDownsampleCount > 0)
			{
				if (this.m_bloomDownsampleCount == 1)
				{
					if (this.m_upscaleQuality == UpscaleQualityEnum.Realistic)
					{
						this.ApplyUpscale();
						this.m_finalCompositionMaterial.SetTexture(AmplifyUtils.MipResultsRTS[0], this.m_tempUpscaleRTs[0]);
					}
					else
					{
						this.m_finalCompositionMaterial.SetTexture(AmplifyUtils.MipResultsRTS[0], this.m_tempAuxDownsampleRTs[0]);
					}
					this.m_finalCompositionMaterial.SetFloat(AmplifyUtils.UpscaleWeightsStr[0], this.m_upscaleWeights[0]);
				}
				else if (this.m_upscaleQuality == UpscaleQualityEnum.Realistic)
				{
					this.ApplyUpscale();
					for (int j = 0; j < this.m_bloomDownsampleCount; j++)
					{
						int num5 = this.m_bloomDownsampleCount - j - 1;
						this.m_finalCompositionMaterial.SetTexture(AmplifyUtils.MipResultsRTS[num5], this.m_tempUpscaleRTs[j]);
						this.m_finalCompositionMaterial.SetFloat(AmplifyUtils.UpscaleWeightsStr[num5], this.m_upscaleWeights[j]);
					}
				}
				else
				{
					for (int k = 0; k < this.m_bloomDownsampleCount; k++)
					{
						int num6 = this.m_bloomDownsampleCount - 1 - k;
						this.m_finalCompositionMaterial.SetTexture(AmplifyUtils.MipResultsRTS[num6], this.m_tempAuxDownsampleRTs[num6]);
						this.m_finalCompositionMaterial.SetFloat(AmplifyUtils.UpscaleWeightsStr[num6], this.m_upscaleWeights[k]);
					}
				}
			}
			else
			{
				this.m_finalCompositionMaterial.SetTexture(AmplifyUtils.MipResultsRTS[0], renderTexture3);
				this.m_finalCompositionMaterial.SetFloat(AmplifyUtils.UpscaleWeightsStr[0], 1f);
			}
			if (this.m_debugToScreen == DebugToScreenEnum.Bloom)
			{
				this.m_finalCompositionMaterial.SetFloat(AmplifyUtils.SourceContributionId, 0f);
				this.FinalComposition(0f, 1f, src, dest, 0);
				return;
			}
			if (this.m_bloomDownsampleCount > 1)
			{
				for (int l = 0; l < this.m_bloomDownsampleCount; l++)
				{
					this.m_finalCompositionMaterial.SetFloat(AmplifyUtils.LensDirtWeightsStr[this.m_bloomDownsampleCount - l - 1], this.m_lensDirtWeights[l]);
					this.m_finalCompositionMaterial.SetFloat(AmplifyUtils.LensStarburstWeightsStr[this.m_bloomDownsampleCount - l - 1], this.m_lensStarburstWeights[l]);
				}
			}
			else
			{
				this.m_finalCompositionMaterial.SetFloat(AmplifyUtils.LensDirtWeightsStr[0], this.m_lensDirtWeights[0]);
				this.m_finalCompositionMaterial.SetFloat(AmplifyUtils.LensStarburstWeightsStr[0], this.m_lensStarburstWeights[0]);
			}
			if (this.m_lensFlare.ApplyLensFlare)
			{
				this.m_finalCompositionMaterial.SetTexture(AmplifyUtils.LensFlareRTId, renderTexture);
			}
			if (this.m_anamorphicGlare.ApplyLensGlare)
			{
				this.m_finalCompositionMaterial.SetTexture(AmplifyUtils.LensGlareRTId, renderTexture2);
			}
			if (this.m_applyLensDirt)
			{
				this.m_finalCompositionMaterial.SetTexture(AmplifyUtils.LensDirtRTId, this.m_lensDirtTexture);
				this.m_finalCompositionMaterial.SetFloat(AmplifyUtils.LensDirtStrengthId, this.m_lensDirtStrength * 1f);
				if (this.m_debugToScreen == DebugToScreenEnum.LensDirt)
				{
					this.FinalComposition(0f, 0f, src, dest, 2);
					return;
				}
			}
			if (this.m_applyLensStardurst)
			{
				this.m_starburstMat[0, 0] = Mathf.Cos(num);
				this.m_starburstMat[0, 1] = -Mathf.Sin(num);
				this.m_starburstMat[1, 0] = Mathf.Sin(num);
				this.m_starburstMat[1, 1] = Mathf.Cos(num);
				this.m_finalCompositionMaterial.SetMatrix(AmplifyUtils.LensFlareStarMatrixId, this.m_starburstMat);
				this.m_finalCompositionMaterial.SetFloat(AmplifyUtils.LensFlareStarburstStrengthId, this.m_lensStarburstStrength * 1f);
				this.m_finalCompositionMaterial.SetTexture(AmplifyUtils.LensStarburstRTId, this.m_lensStardurstTex);
				if (this.m_debugToScreen == DebugToScreenEnum.LensStarburst)
				{
					this.FinalComposition(0f, 0f, src, dest, 1);
					return;
				}
			}
			if (this.m_targetTexture != null)
			{
				this.m_targetTexture.DiscardContents();
				this.FinalComposition(0f, 1f, src, this.m_targetTexture, -1);
				Graphics.Blit(src, dest);
			}
			else
			{
				this.FinalComposition(1f, 1f, src, dest, -1);
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00003328 File Offset: 0x00001528
		private void FinalComposition(float srcContribution, float upscaleContribution, RenderTexture src, RenderTexture dest, int forcePassId)
		{
			this.m_finalCompositionMaterial.SetFloat(AmplifyUtils.SourceContributionId, srcContribution);
			this.m_finalCompositionMaterial.SetFloat(AmplifyUtils.UpscaleContributionId, upscaleContribution);
			int num = 0;
			if (forcePassId > -1)
			{
				num = forcePassId;
			}
			else
			{
				if (this.LensFlareInstance.ApplyLensFlare)
				{
					num |= 8;
				}
				if (this.LensGlareInstance.ApplyLensGlare)
				{
					num |= 4;
				}
				if (this.m_applyLensDirt)
				{
					num |= 2;
				}
				if (this.m_applyLensStardurst)
				{
					num |= 1;
				}
			}
			num += (this.m_bloomDownsampleCount - 1) * 16;
			Graphics.Blit(src, dest, this.m_finalCompositionMaterial, num);
			AmplifyUtils.ReleaseAllRT();
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000033D4 File Offset: 0x000015D4
		private void ApplyUpscale()
		{
			int num = this.m_bloomDownsampleCount - 1;
			int num2 = 0;
			for (int i = num; i > -1; i--)
			{
				this.m_tempUpscaleRTs[num2] = AmplifyUtils.GetTempRenderTarget((int)this.m_tempDownsamplesSizes[i].x, (int)this.m_tempDownsamplesSizes[i].y);
				if (i == num)
				{
					Graphics.Blit(this.m_tempAuxDownsampleRTs[num], this.m_tempUpscaleRTs[num2], this.m_bloomMaterial, 17);
				}
				else
				{
					this.m_bloomMaterial.SetTexture(AmplifyUtils.AnamorphicRTS[0], this.m_tempUpscaleRTs[num2 - 1]);
					Graphics.Blit(this.m_tempAuxDownsampleRTs[i], this.m_tempUpscaleRTs[num2], this.m_bloomMaterial, 18);
				}
				num2++;
			}
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000A RID: 10 RVA: 0x00003498 File Offset: 0x00001698
		public AmplifyGlare LensGlareInstance
		{
			get
			{
				return this.m_anamorphicGlare;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000B RID: 11 RVA: 0x000034A0 File Offset: 0x000016A0
		public AmplifyBokeh BokehFilterInstance
		{
			get
			{
				return this.m_bokehFilter;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000C RID: 12 RVA: 0x000034A8 File Offset: 0x000016A8
		public AmplifyLensFlare LensFlareInstance
		{
			get
			{
				return this.m_lensFlare;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600000D RID: 13 RVA: 0x000034B0 File Offset: 0x000016B0
		// (set) Token: 0x0600000E RID: 14 RVA: 0x000034B8 File Offset: 0x000016B8
		public bool ApplyLensDirt
		{
			get
			{
				return this.m_applyLensDirt;
			}
			set
			{
				this.m_applyLensDirt = value;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000034C4 File Offset: 0x000016C4
		// (set) Token: 0x06000010 RID: 16 RVA: 0x000034CC File Offset: 0x000016CC
		public float LensDirtStrength
		{
			get
			{
				return this.m_lensDirtStrength;
			}
			set
			{
				this.m_lensDirtStrength = ((value >= 0f) ? value : 0f);
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000011 RID: 17 RVA: 0x000034EC File Offset: 0x000016EC
		// (set) Token: 0x06000012 RID: 18 RVA: 0x000034F4 File Offset: 0x000016F4
		public Texture LensDirtTexture
		{
			get
			{
				return this.m_lensDirtTexture;
			}
			set
			{
				this.m_lensDirtTexture = value;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000013 RID: 19 RVA: 0x00003500 File Offset: 0x00001700
		// (set) Token: 0x06000014 RID: 20 RVA: 0x00003508 File Offset: 0x00001708
		public bool ApplyLensStardurst
		{
			get
			{
				return this.m_applyLensStardurst;
			}
			set
			{
				this.m_applyLensStardurst = value;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000015 RID: 21 RVA: 0x00003514 File Offset: 0x00001714
		// (set) Token: 0x06000016 RID: 22 RVA: 0x0000351C File Offset: 0x0000171C
		public Texture LensStardurstTex
		{
			get
			{
				return this.m_lensStardurstTex;
			}
			set
			{
				this.m_lensStardurstTex = value;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00003528 File Offset: 0x00001728
		// (set) Token: 0x06000018 RID: 24 RVA: 0x00003530 File Offset: 0x00001730
		public float LensStarburstStrength
		{
			get
			{
				return this.m_lensStarburstStrength;
			}
			set
			{
				this.m_lensStarburstStrength = ((value >= 0f) ? value : 0f);
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000019 RID: 25 RVA: 0x00003550 File Offset: 0x00001750
		// (set) Token: 0x0600001A RID: 26 RVA: 0x00003560 File Offset: 0x00001760
		public PrecisionModes CurrentPrecisionMode
		{
			get
			{
				if (this.m_highPrecision)
				{
					return PrecisionModes.High;
				}
				return PrecisionModes.Low;
			}
			set
			{
				this.HighPrecision = (value == PrecisionModes.High);
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600001B RID: 27 RVA: 0x0000356C File Offset: 0x0000176C
		// (set) Token: 0x0600001C RID: 28 RVA: 0x00003574 File Offset: 0x00001774
		public bool HighPrecision
		{
			get
			{
				return this.m_highPrecision;
			}
			set
			{
				if (this.m_highPrecision != value)
				{
					this.m_highPrecision = value;
					this.CleanTempFilterRT();
				}
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00003590 File Offset: 0x00001790
		// (set) Token: 0x0600001E RID: 30 RVA: 0x000035A0 File Offset: 0x000017A0
		public float BloomRange
		{
			get
			{
				return this.m_bloomRange.x;
			}
			set
			{
				this.m_bloomRange.x = ((value >= 0f) ? value : 0f);
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600001F RID: 31 RVA: 0x000035C4 File Offset: 0x000017C4
		// (set) Token: 0x06000020 RID: 32 RVA: 0x000035CC File Offset: 0x000017CC
		public float OverallThreshold
		{
			get
			{
				return this.m_overallThreshold;
			}
			set
			{
				this.m_overallThreshold = ((value >= 0f) ? value : 0f);
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000021 RID: 33 RVA: 0x000035EC File Offset: 0x000017EC
		// (set) Token: 0x06000022 RID: 34 RVA: 0x000035F4 File Offset: 0x000017F4
		public Vector4 BloomParams
		{
			get
			{
				return this.m_bloomParams;
			}
			set
			{
				this.m_bloomParams = value;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000023 RID: 35 RVA: 0x00003600 File Offset: 0x00001800
		// (set) Token: 0x06000024 RID: 36 RVA: 0x00003610 File Offset: 0x00001810
		public float OverallIntensity
		{
			get
			{
				return this.m_bloomParams.x;
			}
			set
			{
				this.m_bloomParams.x = ((value >= 0f) ? value : 0f);
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000025 RID: 37 RVA: 0x00003634 File Offset: 0x00001834
		// (set) Token: 0x06000026 RID: 38 RVA: 0x00003644 File Offset: 0x00001844
		public float BloomScale
		{
			get
			{
				return this.m_bloomParams.w;
			}
			set
			{
				this.m_bloomParams.w = ((value >= 0f) ? value : 0f);
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000027 RID: 39 RVA: 0x00003668 File Offset: 0x00001868
		// (set) Token: 0x06000028 RID: 40 RVA: 0x00003678 File Offset: 0x00001878
		public float UpscaleBlurRadius
		{
			get
			{
				return this.m_bloomParams.z;
			}
			set
			{
				this.m_bloomParams.z = value;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000029 RID: 41 RVA: 0x00003688 File Offset: 0x00001888
		// (set) Token: 0x0600002A RID: 42 RVA: 0x00003690 File Offset: 0x00001890
		public bool TemporalFilteringActive
		{
			get
			{
				return this.m_temporalFilteringActive;
			}
			set
			{
				if (this.m_temporalFilteringActive != value)
				{
					this.CleanTempFilterRT();
				}
				this.m_temporalFilteringActive = value;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600002B RID: 43 RVA: 0x000036AC File Offset: 0x000018AC
		// (set) Token: 0x0600002C RID: 44 RVA: 0x000036B4 File Offset: 0x000018B4
		public float TemporalFilteringValue
		{
			get
			{
				return this.m_temporalFilteringValue;
			}
			set
			{
				this.m_temporalFilteringValue = value;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600002D RID: 45 RVA: 0x000036C0 File Offset: 0x000018C0
		public int SoftMaxdownscales
		{
			get
			{
				return this.m_softMaxdownscales;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600002E RID: 46 RVA: 0x000036C8 File Offset: 0x000018C8
		// (set) Token: 0x0600002F RID: 47 RVA: 0x000036D0 File Offset: 0x000018D0
		public int BloomDownsampleCount
		{
			get
			{
				return this.m_bloomDownsampleCount;
			}
			set
			{
				this.m_bloomDownsampleCount = Mathf.Clamp(value, 1, this.m_softMaxdownscales);
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000030 RID: 48 RVA: 0x000036E8 File Offset: 0x000018E8
		// (set) Token: 0x06000031 RID: 49 RVA: 0x000036F0 File Offset: 0x000018F0
		public int FeaturesSourceId
		{
			get
			{
				return this.m_featuresSourceId;
			}
			set
			{
				this.m_featuresSourceId = Mathf.Clamp(value, 0, this.m_bloomDownsampleCount - 1);
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000032 RID: 50 RVA: 0x00003708 File Offset: 0x00001908
		public bool[] DownscaleSettingsFoldout
		{
			get
			{
				return this.m_downscaleSettingsFoldout;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00003710 File Offset: 0x00001910
		public float[] UpscaleWeights
		{
			get
			{
				return this.m_upscaleWeights;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000034 RID: 52 RVA: 0x00003718 File Offset: 0x00001918
		public float[] LensDirtWeights
		{
			get
			{
				return this.m_lensDirtWeights;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00003720 File Offset: 0x00001920
		public float[] LensStarburstWeights
		{
			get
			{
				return this.m_lensStarburstWeights;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00003728 File Offset: 0x00001928
		public float[] GaussianRadius
		{
			get
			{
				return this.m_gaussianRadius;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00003730 File Offset: 0x00001930
		public int[] GaussianSteps
		{
			get
			{
				return this.m_gaussianSteps;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00003738 File Offset: 0x00001938
		// (set) Token: 0x06000039 RID: 57 RVA: 0x00003740 File Offset: 0x00001940
		public AnimationCurve TemporalFilteringCurve
		{
			get
			{
				return this.m_temporalFilteringCurve;
			}
			set
			{
				this.m_temporalFilteringCurve = value;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600003A RID: 58 RVA: 0x0000374C File Offset: 0x0000194C
		// (set) Token: 0x0600003B RID: 59 RVA: 0x00003754 File Offset: 0x00001954
		public bool SeparateFeaturesThreshold
		{
			get
			{
				return this.m_separateFeaturesThreshold;
			}
			set
			{
				this.m_separateFeaturesThreshold = value;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00003760 File Offset: 0x00001960
		// (set) Token: 0x0600003D RID: 61 RVA: 0x00003768 File Offset: 0x00001968
		public float FeaturesThreshold
		{
			get
			{
				return this.m_featuresThreshold;
			}
			set
			{
				this.m_featuresThreshold = ((value >= 0f) ? value : 0f);
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x0600003E RID: 62 RVA: 0x00003788 File Offset: 0x00001988
		// (set) Token: 0x0600003F RID: 63 RVA: 0x00003790 File Offset: 0x00001990
		public DebugToScreenEnum DebugToScreen
		{
			get
			{
				return this.m_debugToScreen;
			}
			set
			{
				this.m_debugToScreen = value;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000040 RID: 64 RVA: 0x0000379C File Offset: 0x0000199C
		// (set) Token: 0x06000041 RID: 65 RVA: 0x000037A4 File Offset: 0x000019A4
		public UpscaleQualityEnum UpscaleQuality
		{
			get
			{
				return this.m_upscaleQuality;
			}
			set
			{
				this.m_upscaleQuality = value;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000042 RID: 66 RVA: 0x000037B0 File Offset: 0x000019B0
		// (set) Token: 0x06000043 RID: 67 RVA: 0x000037B8 File Offset: 0x000019B8
		public bool ShowDebugMessages
		{
			get
			{
				return this.m_showDebugMessages;
			}
			set
			{
				this.m_showDebugMessages = value;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000044 RID: 68 RVA: 0x000037C4 File Offset: 0x000019C4
		// (set) Token: 0x06000045 RID: 69 RVA: 0x000037CC File Offset: 0x000019CC
		public MainThresholdSizeEnum MainThresholdSize
		{
			get
			{
				return this.m_mainThresholdSize;
			}
			set
			{
				this.m_mainThresholdSize = value;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000046 RID: 70 RVA: 0x000037D8 File Offset: 0x000019D8
		// (set) Token: 0x06000047 RID: 71 RVA: 0x000037E0 File Offset: 0x000019E0
		public RenderTexture TargetTexture
		{
			get
			{
				return this.m_targetTexture;
			}
			set
			{
				this.m_targetTexture = value;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000048 RID: 72 RVA: 0x000037EC File Offset: 0x000019EC
		// (set) Token: 0x06000049 RID: 73 RVA: 0x000037F4 File Offset: 0x000019F4
		public Texture MaskTexture
		{
			get
			{
				return this.m_maskTexture;
			}
			set
			{
				this.m_maskTexture = value;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600004A RID: 74 RVA: 0x00003800 File Offset: 0x00001A00
		// (set) Token: 0x0600004B RID: 75 RVA: 0x00003810 File Offset: 0x00001A10
		public bool ApplyBokehFilter
		{
			get
			{
				return this.m_bokehFilter.ApplyBokeh;
			}
			set
			{
				this.m_bokehFilter.ApplyBokeh = value;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600004C RID: 76 RVA: 0x00003820 File Offset: 0x00001A20
		// (set) Token: 0x0600004D RID: 77 RVA: 0x00003830 File Offset: 0x00001A30
		public bool ApplyLensFlare
		{
			get
			{
				return this.m_lensFlare.ApplyLensFlare;
			}
			set
			{
				this.m_lensFlare.ApplyLensFlare = value;
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600004E RID: 78 RVA: 0x00003840 File Offset: 0x00001A40
		// (set) Token: 0x0600004F RID: 79 RVA: 0x00003850 File Offset: 0x00001A50
		public bool ApplyLensGlare
		{
			get
			{
				return this.m_anamorphicGlare.ApplyLensGlare;
			}
			set
			{
				this.m_anamorphicGlare.ApplyLensGlare = value;
			}
		}

		// Token: 0x0400003A RID: 58
		public const int MaxGhosts = 5;

		// Token: 0x0400003B RID: 59
		public const int MinDownscales = 1;

		// Token: 0x0400003C RID: 60
		public const int MaxDownscales = 6;

		// Token: 0x0400003D RID: 61
		public const int MaxGaussian = 8;

		// Token: 0x0400003E RID: 62
		private const float MaxDirtIntensity = 1f;

		// Token: 0x0400003F RID: 63
		private const float MaxStarburstIntensity = 1f;

		// Token: 0x04000040 RID: 64
		[SerializeField]
		private Texture m_maskTexture;

		// Token: 0x04000041 RID: 65
		[SerializeField]
		private RenderTexture m_targetTexture;

		// Token: 0x04000042 RID: 66
		[SerializeField]
		private bool m_showDebugMessages = true;

		// Token: 0x04000043 RID: 67
		[SerializeField]
		private int m_softMaxdownscales = 6;

		// Token: 0x04000044 RID: 68
		[SerializeField]
		private DebugToScreenEnum m_debugToScreen;

		// Token: 0x04000045 RID: 69
		[SerializeField]
		private bool m_highPrecision;

		// Token: 0x04000046 RID: 70
		[SerializeField]
		private Vector4 m_bloomRange = new Vector4(500f, 1f, 0f, 0f);

		// Token: 0x04000047 RID: 71
		[SerializeField]
		private float m_overallThreshold = 0.53f;

		// Token: 0x04000048 RID: 72
		[SerializeField]
		private Vector4 m_bloomParams = new Vector4(0.8f, 1f, 1f, 1f);

		// Token: 0x04000049 RID: 73
		[SerializeField]
		private bool m_temporalFilteringActive;

		// Token: 0x0400004A RID: 74
		[SerializeField]
		private float m_temporalFilteringValue = 0.05f;

		// Token: 0x0400004B RID: 75
		[SerializeField]
		private int m_bloomDownsampleCount = 6;

		// Token: 0x0400004C RID: 76
		[SerializeField]
		private AnimationCurve m_temporalFilteringCurve;

		// Token: 0x0400004D RID: 77
		[SerializeField]
		private bool m_separateFeaturesThreshold;

		// Token: 0x0400004E RID: 78
		[SerializeField]
		private float m_featuresThreshold = 0.05f;

		// Token: 0x0400004F RID: 79
		[SerializeField]
		private AmplifyLensFlare m_lensFlare = new AmplifyLensFlare();

		// Token: 0x04000050 RID: 80
		[SerializeField]
		private bool m_applyLensDirt = true;

		// Token: 0x04000051 RID: 81
		[SerializeField]
		private float m_lensDirtStrength = 2f;

		// Token: 0x04000052 RID: 82
		[SerializeField]
		private Texture m_lensDirtTexture;

		// Token: 0x04000053 RID: 83
		[SerializeField]
		private bool m_applyLensStardurst = true;

		// Token: 0x04000054 RID: 84
		[SerializeField]
		private Texture m_lensStardurstTex;

		// Token: 0x04000055 RID: 85
		[SerializeField]
		private float m_lensStarburstStrength = 2f;

		// Token: 0x04000056 RID: 86
		[SerializeField]
		private AmplifyGlare m_anamorphicGlare = new AmplifyGlare();

		// Token: 0x04000057 RID: 87
		[SerializeField]
		private AmplifyBokeh m_bokehFilter = new AmplifyBokeh();

		// Token: 0x04000058 RID: 88
		[SerializeField]
		private float[] m_upscaleWeights = new float[]
		{
			0.0842f,
			0.1282f,
			0.1648f,
			0.2197f,
			0.2197f,
			0.1831f
		};

		// Token: 0x04000059 RID: 89
		[SerializeField]
		private float[] m_gaussianRadius = new float[]
		{
			1f,
			1f,
			1f,
			1f,
			1f,
			1f
		};

		// Token: 0x0400005A RID: 90
		[SerializeField]
		private int[] m_gaussianSteps = new int[]
		{
			1,
			1,
			1,
			1,
			1,
			1
		};

		// Token: 0x0400005B RID: 91
		[SerializeField]
		private float[] m_lensDirtWeights = new float[]
		{
			0.067f,
			0.102f,
			0.1311f,
			0.1749f,
			0.2332f,
			0.3f
		};

		// Token: 0x0400005C RID: 92
		[SerializeField]
		private float[] m_lensStarburstWeights = new float[]
		{
			0.067f,
			0.102f,
			0.1311f,
			0.1749f,
			0.2332f,
			0.3f
		};

		// Token: 0x0400005D RID: 93
		[SerializeField]
		private bool[] m_downscaleSettingsFoldout = new bool[6];

		// Token: 0x0400005E RID: 94
		[SerializeField]
		private int m_featuresSourceId;

		// Token: 0x0400005F RID: 95
		[SerializeField]
		private UpscaleQualityEnum m_upscaleQuality;

		// Token: 0x04000060 RID: 96
		[SerializeField]
		private MainThresholdSizeEnum m_mainThresholdSize;

		// Token: 0x04000061 RID: 97
		private Transform m_cameraTransform;

		// Token: 0x04000062 RID: 98
		private Matrix4x4 m_starburstMat;

		// Token: 0x04000063 RID: 99
		private Shader m_bloomShader;

		// Token: 0x04000064 RID: 100
		private Material m_bloomMaterial;

		// Token: 0x04000065 RID: 101
		private Shader m_finalCompositionShader;

		// Token: 0x04000066 RID: 102
		private Material m_finalCompositionMaterial;

		// Token: 0x04000067 RID: 103
		private RenderTexture m_tempFilterBuffer;

		// Token: 0x04000068 RID: 104
		private Camera m_camera;

		// Token: 0x04000069 RID: 105
		private RenderTexture[] m_tempUpscaleRTs = new RenderTexture[6];

		// Token: 0x0400006A RID: 106
		private RenderTexture[] m_tempAuxDownsampleRTs = new RenderTexture[6];

		// Token: 0x0400006B RID: 107
		private Vector2[] m_tempDownsamplesSizes = new Vector2[6];

		// Token: 0x0400006C RID: 108
		private bool silentError;
	}
}
