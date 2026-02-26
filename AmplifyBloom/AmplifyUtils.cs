using System;
using System.Collections.Generic;
using UnityEngine;

namespace AmplifyBloom
{
	// Token: 0x02000018 RID: 24
	public class AmplifyUtils
	{
		// Token: 0x060000E4 RID: 228 RVA: 0x00005974 File Offset: 0x00003B74
		public static void InitializeIds()
		{
			AmplifyUtils.IsInitialized = true;
			AmplifyUtils.MaskTextureId = Shader.PropertyToID("_MaskTex");
			AmplifyUtils.MipResultsRTS = new int[]
			{
				Shader.PropertyToID("_MipResultsRTS0"),
				Shader.PropertyToID("_MipResultsRTS1"),
				Shader.PropertyToID("_MipResultsRTS2"),
				Shader.PropertyToID("_MipResultsRTS3"),
				Shader.PropertyToID("_MipResultsRTS4"),
				Shader.PropertyToID("_MipResultsRTS5")
			};
			AmplifyUtils.AnamorphicRTS = new int[]
			{
				Shader.PropertyToID("_AnamorphicRTS0"),
				Shader.PropertyToID("_AnamorphicRTS1"),
				Shader.PropertyToID("_AnamorphicRTS2"),
				Shader.PropertyToID("_AnamorphicRTS3"),
				Shader.PropertyToID("_AnamorphicRTS4"),
				Shader.PropertyToID("_AnamorphicRTS5"),
				Shader.PropertyToID("_AnamorphicRTS6"),
				Shader.PropertyToID("_AnamorphicRTS7")
			};
			AmplifyUtils.AnamorphicGlareWeightsMatStr = new int[]
			{
				Shader.PropertyToID("_AnamorphicGlareWeightsMat0"),
				Shader.PropertyToID("_AnamorphicGlareWeightsMat1"),
				Shader.PropertyToID("_AnamorphicGlareWeightsMat2"),
				Shader.PropertyToID("_AnamorphicGlareWeightsMat3")
			};
			AmplifyUtils.AnamorphicGlareOffsetsMatStr = new int[]
			{
				Shader.PropertyToID("_AnamorphicGlareOffsetsMat0"),
				Shader.PropertyToID("_AnamorphicGlareOffsetsMat1"),
				Shader.PropertyToID("_AnamorphicGlareOffsetsMat2"),
				Shader.PropertyToID("_AnamorphicGlareOffsetsMat3")
			};
			AmplifyUtils.AnamorphicGlareWeightsStr = new int[]
			{
				Shader.PropertyToID("_AnamorphicGlareWeights0"),
				Shader.PropertyToID("_AnamorphicGlareWeights1"),
				Shader.PropertyToID("_AnamorphicGlareWeights2"),
				Shader.PropertyToID("_AnamorphicGlareWeights3"),
				Shader.PropertyToID("_AnamorphicGlareWeights4"),
				Shader.PropertyToID("_AnamorphicGlareWeights5"),
				Shader.PropertyToID("_AnamorphicGlareWeights6"),
				Shader.PropertyToID("_AnamorphicGlareWeights7"),
				Shader.PropertyToID("_AnamorphicGlareWeights8"),
				Shader.PropertyToID("_AnamorphicGlareWeights9"),
				Shader.PropertyToID("_AnamorphicGlareWeights10"),
				Shader.PropertyToID("_AnamorphicGlareWeights11"),
				Shader.PropertyToID("_AnamorphicGlareWeights12"),
				Shader.PropertyToID("_AnamorphicGlareWeights13"),
				Shader.PropertyToID("_AnamorphicGlareWeights14"),
				Shader.PropertyToID("_AnamorphicGlareWeights15")
			};
			AmplifyUtils.UpscaleWeightsStr = new int[]
			{
				Shader.PropertyToID("_UpscaleWeights0"),
				Shader.PropertyToID("_UpscaleWeights1"),
				Shader.PropertyToID("_UpscaleWeights2"),
				Shader.PropertyToID("_UpscaleWeights3"),
				Shader.PropertyToID("_UpscaleWeights4"),
				Shader.PropertyToID("_UpscaleWeights5"),
				Shader.PropertyToID("_UpscaleWeights6"),
				Shader.PropertyToID("_UpscaleWeights7")
			};
			AmplifyUtils.LensDirtWeightsStr = new int[]
			{
				Shader.PropertyToID("_LensDirtWeights0"),
				Shader.PropertyToID("_LensDirtWeights1"),
				Shader.PropertyToID("_LensDirtWeights2"),
				Shader.PropertyToID("_LensDirtWeights3"),
				Shader.PropertyToID("_LensDirtWeights4"),
				Shader.PropertyToID("_LensDirtWeights5"),
				Shader.PropertyToID("_LensDirtWeights6"),
				Shader.PropertyToID("_LensDirtWeights7")
			};
			AmplifyUtils.LensStarburstWeightsStr = new int[]
			{
				Shader.PropertyToID("_LensStarburstWeights0"),
				Shader.PropertyToID("_LensStarburstWeights1"),
				Shader.PropertyToID("_LensStarburstWeights2"),
				Shader.PropertyToID("_LensStarburstWeights3"),
				Shader.PropertyToID("_LensStarburstWeights4"),
				Shader.PropertyToID("_LensStarburstWeights5"),
				Shader.PropertyToID("_LensStarburstWeights6"),
				Shader.PropertyToID("_LensStarburstWeights7")
			};
			AmplifyUtils.BloomRangeId = Shader.PropertyToID("_BloomRange");
			AmplifyUtils.LensDirtStrengthId = Shader.PropertyToID("_LensDirtStrength");
			AmplifyUtils.BloomParamsId = Shader.PropertyToID("_BloomParams");
			AmplifyUtils.TempFilterValueId = Shader.PropertyToID("_TempFilterValue");
			AmplifyUtils.LensFlareStarMatrixId = Shader.PropertyToID("_LensFlareStarMatrix");
			AmplifyUtils.LensFlareStarburstStrengthId = Shader.PropertyToID("_LensFlareStarburstStrength");
			AmplifyUtils.LensFlareGhostsParamsId = Shader.PropertyToID("_LensFlareGhostsParams");
			AmplifyUtils.LensFlareLUTId = Shader.PropertyToID("_LensFlareLUT");
			AmplifyUtils.LensFlareHaloParamsId = Shader.PropertyToID("_LensFlareHaloParams");
			AmplifyUtils.LensFlareGhostChrDistortionId = Shader.PropertyToID("_LensFlareGhostChrDistortion");
			AmplifyUtils.LensFlareHaloChrDistortionId = Shader.PropertyToID("_LensFlareHaloChrDistortion");
			AmplifyUtils.BokehParamsId = Shader.PropertyToID("_BokehParams");
			AmplifyUtils.BlurRadiusId = Shader.PropertyToID("_BlurRadius");
			AmplifyUtils.LensStarburstRTId = Shader.PropertyToID("_LensStarburst");
			AmplifyUtils.LensDirtRTId = Shader.PropertyToID("_LensDirt");
			AmplifyUtils.LensFlareRTId = Shader.PropertyToID("_LensFlare");
			AmplifyUtils.LensGlareRTId = Shader.PropertyToID("_LensGlare");
			AmplifyUtils.SourceContributionId = Shader.PropertyToID("_SourceContribution");
			AmplifyUtils.UpscaleContributionId = Shader.PropertyToID("_UpscaleContribution");
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00005E3C File Offset: 0x0000403C
		public static void DebugLog(string value, LogType type)
		{
			switch (type)
			{
			case LogType.Normal:
				Debug.Log(AmplifyUtils.DebugStr + value);
				break;
			case LogType.Warning:
				Debug.LogWarning(AmplifyUtils.DebugStr + value);
				break;
			case LogType.Error:
				Debug.LogError(AmplifyUtils.DebugStr + value);
				break;
			}
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00005EA4 File Offset: 0x000040A4
		public static RenderTexture GetTempRenderTarget(int width, int height)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, AmplifyUtils.CurrentRTFormat, AmplifyUtils.CurrentReadWriteMode);
			temporary.filterMode = AmplifyUtils.CurrentFilterMode;
			temporary.wrapMode = AmplifyUtils.CurrentWrapMode;
			AmplifyUtils._allocatedRT.Add(temporary);
			return temporary;
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00005EE8 File Offset: 0x000040E8
		public static void ReleaseTempRenderTarget(RenderTexture renderTarget)
		{
			if (renderTarget != null && AmplifyUtils._allocatedRT.Remove(renderTarget))
			{
				renderTarget.DiscardContents();
				RenderTexture.ReleaseTemporary(renderTarget);
			}
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00005F20 File Offset: 0x00004120
		public static void ReleaseAllRT()
		{
			for (int i = 0; i < AmplifyUtils._allocatedRT.Count; i++)
			{
				AmplifyUtils._allocatedRT[i].DiscardContents();
				RenderTexture.ReleaseTemporary(AmplifyUtils._allocatedRT[i]);
			}
			AmplifyUtils._allocatedRT.Clear();
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00005F74 File Offset: 0x00004174
		public static void EnsureKeywordEnabled(Material mat, string keyword, bool state)
		{
			if (mat != null)
			{
				if (state && !mat.IsKeywordEnabled(keyword))
				{
					mat.EnableKeyword(keyword);
				}
				else if (!state && mat.IsKeywordEnabled(keyword))
				{
					mat.DisableKeyword(keyword);
				}
			}
		}

		// Token: 0x040000D8 RID: 216
		public static int MaskTextureId;

		// Token: 0x040000D9 RID: 217
		public static int BlurRadiusId;

		// Token: 0x040000DA RID: 218
		public static string HighPrecisionKeyword = "AB_HIGH_PRECISION";

		// Token: 0x040000DB RID: 219
		public static string ShaderModeTag = "Mode";

		// Token: 0x040000DC RID: 220
		public static string ShaderModeValue = "Full";

		// Token: 0x040000DD RID: 221
		public static string DebugStr = "[ Amplify Bloom ] ";

		// Token: 0x040000DE RID: 222
		public static int UpscaleContributionId;

		// Token: 0x040000DF RID: 223
		public static int SourceContributionId;

		// Token: 0x040000E0 RID: 224
		public static int LensStarburstRTId;

		// Token: 0x040000E1 RID: 225
		public static int LensDirtRTId;

		// Token: 0x040000E2 RID: 226
		public static int LensFlareRTId;

		// Token: 0x040000E3 RID: 227
		public static int LensGlareRTId;

		// Token: 0x040000E4 RID: 228
		public static int[] MipResultsRTS;

		// Token: 0x040000E5 RID: 229
		public static int[] AnamorphicRTS;

		// Token: 0x040000E6 RID: 230
		public static int[] AnamorphicGlareWeightsMatStr;

		// Token: 0x040000E7 RID: 231
		public static int[] AnamorphicGlareOffsetsMatStr;

		// Token: 0x040000E8 RID: 232
		public static int[] AnamorphicGlareWeightsStr;

		// Token: 0x040000E9 RID: 233
		public static int[] UpscaleWeightsStr;

		// Token: 0x040000EA RID: 234
		public static int[] LensDirtWeightsStr;

		// Token: 0x040000EB RID: 235
		public static int[] LensStarburstWeightsStr;

		// Token: 0x040000EC RID: 236
		public static int BloomRangeId;

		// Token: 0x040000ED RID: 237
		public static int LensDirtStrengthId;

		// Token: 0x040000EE RID: 238
		public static int BloomParamsId;

		// Token: 0x040000EF RID: 239
		public static int TempFilterValueId;

		// Token: 0x040000F0 RID: 240
		public static int LensFlareStarMatrixId;

		// Token: 0x040000F1 RID: 241
		public static int LensFlareStarburstStrengthId;

		// Token: 0x040000F2 RID: 242
		public static int LensFlareGhostsParamsId;

		// Token: 0x040000F3 RID: 243
		public static int LensFlareLUTId;

		// Token: 0x040000F4 RID: 244
		public static int LensFlareHaloParamsId;

		// Token: 0x040000F5 RID: 245
		public static int LensFlareGhostChrDistortionId;

		// Token: 0x040000F6 RID: 246
		public static int LensFlareHaloChrDistortionId;

		// Token: 0x040000F7 RID: 247
		public static int BokehParamsId = -1;

		// Token: 0x040000F8 RID: 248
		public static RenderTextureFormat CurrentRTFormat = RenderTextureFormat.DefaultHDR;

		// Token: 0x040000F9 RID: 249
		public static FilterMode CurrentFilterMode = FilterMode.Bilinear;

		// Token: 0x040000FA RID: 250
		public static TextureWrapMode CurrentWrapMode = TextureWrapMode.Clamp;

		// Token: 0x040000FB RID: 251
		public static RenderTextureReadWrite CurrentReadWriteMode = RenderTextureReadWrite.sRGB;

		// Token: 0x040000FC RID: 252
		public static bool IsInitialized = false;

		// Token: 0x040000FD RID: 253
		private static List<RenderTexture> _allocatedRT = new List<RenderTexture>();
	}
}
