using System;
using System.Collections.Generic;
using Ceto.Common.Unity.Utility;
using UnityEngine;

namespace Ceto
{
	// Token: 0x0200008B RID: 139
	public class OverlayManager
	{
		// Token: 0x06000398 RID: 920 RVA: 0x00014F0C File Offset: 0x0001310C
		public OverlayManager(Material mat)
		{
			this.MaxDisplacement = 1f;
			this.m_overlayMat = mat;
			this.m_waveOverlays = new LinkedList<WaveOverlay>();
			this.m_queryableOverlays = new List<WaveOverlay>(32);
			this.m_removeOverlays = new List<WaveOverlay>(32);
			this.m_containingOverlays = new List<QueryableOverlayResult>(32);
			this.m_heightOverlays = new List<WaveOverlay>(32);
			this.m_normalOverlays = new List<WaveOverlay>(32);
			this.m_foamOverlays = new List<WaveOverlay>(32);
			this.m_clipOverlays = new List<WaveOverlay>(32);
			this.m_blankNormal = new Texture2D(1, 1, TextureFormat.ARGB32, false, true);
			this.m_blankNormal.SetPixel(0, 0, new Color(0.5f, 0.5f, 1f, 0.5f));
			this.m_blankNormal.hideFlags = HideFlags.HideAndDontSave;
			this.m_blankNormal.name = "Ceto Blank Normal Texture";
			this.m_blankNormal.Apply();
			this.m_clearColor = new Color(0f, 0f, 0f, 0f);
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x0600039A RID: 922 RVA: 0x0001504C File Offset: 0x0001324C
		// (set) Token: 0x0600039B RID: 923 RVA: 0x00015054 File Offset: 0x00013254
		public bool HasHeightOverlay { get; private set; }

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x0600039C RID: 924 RVA: 0x00015060 File Offset: 0x00013260
		// (set) Token: 0x0600039D RID: 925 RVA: 0x00015068 File Offset: 0x00013268
		public bool HasNormalOverlay { get; private set; }

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x0600039E RID: 926 RVA: 0x00015074 File Offset: 0x00013274
		// (set) Token: 0x0600039F RID: 927 RVA: 0x0001507C File Offset: 0x0001327C
		public bool HasFoamOverlay { get; private set; }

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x060003A0 RID: 928 RVA: 0x00015088 File Offset: 0x00013288
		// (set) Token: 0x060003A1 RID: 929 RVA: 0x00015090 File Offset: 0x00013290
		public bool HasClipOverlay { get; private set; }

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x060003A2 RID: 930 RVA: 0x0001509C File Offset: 0x0001329C
		// (set) Token: 0x060003A3 RID: 931 RVA: 0x000150A4 File Offset: 0x000132A4
		public float MaxDisplacement { get; private set; }

		// Token: 0x060003A4 RID: 932 RVA: 0x000150B0 File Offset: 0x000132B0
		public void Release()
		{
			UnityEngine.Object.DestroyImmediate(this.m_blankNormal);
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x000150C0 File Offset: 0x000132C0
		public void Update()
		{
			this.HasHeightOverlay = false;
			this.HasNormalOverlay = false;
			this.HasFoamOverlay = false;
			this.HasClipOverlay = false;
			this.MaxDisplacement = 1f;
			this.m_queryableOverlays.Clear();
			this.m_removeOverlays.Clear();
			foreach (WaveOverlay waveOverlay in this.m_waveOverlays)
			{
				if (waveOverlay.Kill)
				{
					this.m_removeOverlays.Add(waveOverlay);
				}
				else if (!waveOverlay.Hide)
				{
					bool flag = false;
					if (waveOverlay.HeightTex.IsDrawable)
					{
						this.HasHeightOverlay = true;
						flag = true;
						if (waveOverlay.HeightTex.alpha > this.MaxDisplacement)
						{
							this.MaxDisplacement = waveOverlay.HeightTex.alpha;
						}
					}
					if (waveOverlay.NormalTex.IsDrawable)
					{
						this.HasNormalOverlay = true;
					}
					if (waveOverlay.FoamTex.IsDrawable)
					{
						this.HasFoamOverlay = true;
					}
					if (waveOverlay.ClipTex.IsDrawable)
					{
						this.HasClipOverlay = true;
						flag = true;
					}
					if (flag)
					{
						this.m_queryableOverlays.Add(waveOverlay);
					}
				}
			}
			this.MaxDisplacement = Mathf.Min(this.MaxDisplacement, 20f);
			foreach (WaveOverlay value in this.m_removeOverlays)
			{
				this.m_waveOverlays.Remove(value);
			}
		}

		// Token: 0x060003A6 RID: 934 RVA: 0x00015238 File Offset: 0x00013438
		public void Add(WaveOverlay overlay)
		{
			if (overlay.Kill)
			{
				return;
			}
			this.m_waveOverlays.AddLast(overlay);
		}

		// Token: 0x060003A7 RID: 935 RVA: 0x00015254 File Offset: 0x00013454
		public void Remove(WaveOverlay overlay)
		{
			this.m_waveOverlays.Remove(overlay);
		}

		// Token: 0x060003A8 RID: 936 RVA: 0x00015264 File Offset: 0x00013464
		public void Clear()
		{
			this.m_waveOverlays.Clear();
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x00015274 File Offset: 0x00013474
		public void ClearBuffers(WaveOverlayData data)
		{
			RTUtility.ClearColor(data.height, this.m_clearColor);
			RTUtility.ClearColor(data.normal, this.m_clearColor);
			RTUtility.ClearColor(data.foam, this.m_clearColor);
			RTUtility.ClearColor(data.clip, this.m_clearColor);
		}

		// Token: 0x060003AA RID: 938 RVA: 0x000152C8 File Offset: 0x000134C8
		public void DestroyBuffers(WaveOverlayData data)
		{
			RTUtility.ReleaseAndDestroy(data.normal);
			RTUtility.ReleaseAndDestroy(data.height);
			RTUtility.ReleaseAndDestroy(data.foam);
			RTUtility.ReleaseAndDestroy(data.clip);
			data.normal = null;
			data.height = null;
			data.foam = null;
			data.clip = null;
		}

		// Token: 0x060003AB RID: 939 RVA: 0x00015320 File Offset: 0x00013520
		public bool QueryableContains(float x, float z, bool overrideIqnoreQuerys)
		{
			foreach (WaveOverlay waveOverlay in this.m_queryableOverlays)
			{
				if (!waveOverlay.Hide)
				{
					bool flag = (overrideIqnoreQuerys || !waveOverlay.HeightTex.ignoreQuerys) && waveOverlay.HeightTex.IsDrawable;
					bool flag2 = (overrideIqnoreQuerys || !waveOverlay.ClipTex.ignoreQuerys) && waveOverlay.ClipTex.IsDrawable;
					if (flag || flag2)
					{
						if (waveOverlay.Contains(x, z))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x060003AC RID: 940 RVA: 0x000153D0 File Offset: 0x000135D0
		public void GetQueryableContaining(float x, float z, bool overrideIqnoreQuerys, bool clipOnly)
		{
			this.m_containingOverlays.Clear();
			foreach (WaveOverlay waveOverlay in this.m_queryableOverlays)
			{
				if (!waveOverlay.Hide)
				{
					bool flag = !clipOnly && (overrideIqnoreQuerys || !waveOverlay.HeightTex.ignoreQuerys) && waveOverlay.HeightTex.IsDrawable;
					bool flag2 = (overrideIqnoreQuerys || !waveOverlay.ClipTex.ignoreQuerys) && waveOverlay.ClipTex.IsDrawable;
					if (flag || flag2)
					{
						float u;
						float v;
						if (waveOverlay.Contains(x, z, out u, out v))
						{
							QueryableOverlayResult item;
							item.overlay = waveOverlay;
							item.u = u;
							item.v = v;
							this.m_containingOverlays.Add(item);
						}
					}
				}
			}
		}

		// Token: 0x060003AD RID: 941 RVA: 0x000154B8 File Offset: 0x000136B8
		public void QueryWaves(WaveQuery query)
		{
			if (this.m_queryableOverlays.Count == 0)
			{
				return;
			}
			if (!query.sampleOverlay)
			{
				return;
			}
			bool flag = query.mode == QUERY_MODE.CLIP_TEST;
			float posX = query.posX;
			float posZ = query.posZ;
			this.GetQueryableContaining(posX, posZ, query.overrideIgnoreQuerys, flag);
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			foreach (QueryableOverlayResult queryableOverlayResult in this.m_containingOverlays)
			{
				try
				{
					OverlayClipTexture clipTex = queryableOverlayResult.overlay.ClipTex;
					OverlayHeightTexture heightTex = queryableOverlayResult.overlay.HeightTex;
					if (clipTex.IsDrawable && clipTex.tex is Texture2D)
					{
						float a = (clipTex.tex as Texture2D).GetPixelBilinear(queryableOverlayResult.u, queryableOverlayResult.v).a;
						num += a * Mathf.Max(0f, clipTex.alpha);
					}
					if (!flag && heightTex.IsDrawable)
					{
						float alpha = heightTex.alpha;
						float num4 = Mathf.Max(0f, heightTex.maskAlpha);
						float num5 = 0f;
						float num6 = 0f;
						if (heightTex.tex != null && heightTex.tex is Texture2D)
						{
							num5 = (heightTex.tex as Texture2D).GetPixelBilinear(queryableOverlayResult.u, queryableOverlayResult.v).a;
						}
						if (heightTex.mask != null && heightTex.mask is Texture2D)
						{
							num6 = (heightTex.mask as Texture2D).GetPixelBilinear(queryableOverlayResult.u, queryableOverlayResult.v).a;
							num6 = Mathf.Clamp01(num6 * num4);
						}
						if (heightTex.maskMode == OVERLAY_MASK_MODE.WAVES)
						{
							num5 *= alpha;
						}
						else if (heightTex.maskMode == OVERLAY_MASK_MODE.OVERLAY)
						{
							num5 *= alpha * num6;
							num6 = 0f;
						}
						else if (heightTex.maskMode == OVERLAY_MASK_MODE.WAVES_AND_OVERLAY)
						{
							num5 *= alpha * (1f - num6);
						}
						else if (heightTex.maskMode == OVERLAY_MASK_MODE.WAVES_AND_OVERLAY_BLEND)
						{
							num5 *= alpha * num6;
						}
						num2 += num5;
						num3 += num6;
					}
				}
				catch
				{
				}
			}
			num = Mathf.Clamp01(num);
			if (0.5f - num < 0f)
			{
				query.result.isClipped = true;
			}
			num3 = 1f - Mathf.Clamp01(num3);
			query.result.height = query.result.height * num3;
			query.result.displacementX = query.result.displacementX * num3;
			query.result.displacementZ = query.result.displacementZ * num3;
			query.result.height = query.result.height + num2;
			query.result.overlayHeight = num2;
		}

		// Token: 0x060003AE RID: 942 RVA: 0x000157E0 File Offset: 0x000139E0
		public void RenderWaveOverlays(Camera cam, WaveOverlayData data)
		{
			if (!this.m_beenCleared)
			{
				this.ClearBuffers(data);
				this.m_beenCleared = true;
			}
			if (this.m_waveOverlays.Count == 0)
			{
				return;
			}
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.current);
			this.m_heightOverlays.Clear();
			this.m_normalOverlays.Clear();
			this.m_foamOverlays.Clear();
			this.m_clipOverlays.Clear();
			foreach (WaveOverlay waveOverlay in this.m_waveOverlays)
			{
				if (!waveOverlay.Hide && GeometryUtility.TestPlanesAABB(planes, waveOverlay.BoundingBox))
				{
					if (waveOverlay.HeightTex.IsDrawable)
					{
						this.m_heightOverlays.Add(waveOverlay);
					}
					if (waveOverlay.NormalTex.IsDrawable)
					{
						this.m_normalOverlays.Add(waveOverlay);
					}
					if (waveOverlay.FoamTex.IsDrawable)
					{
						this.m_foamOverlays.Add(waveOverlay);
					}
					if (waveOverlay.ClipTex.IsDrawable)
					{
						this.m_clipOverlays.Add(waveOverlay);
					}
				}
			}
			this.RenderHeightOverlays(this.m_heightOverlays, data.height);
			this.RenderNormalOverlays(this.m_normalOverlays, data.normal);
			this.RenderFoamOverlays(this.m_foamOverlays, data.foam);
			this.RenderClipOverlays(this.m_clipOverlays, data.clip);
			if (data.normal != null)
			{
				Shader.SetGlobalTexture("Ceto_Overlay_NormalMap", data.normal);
			}
			else
			{
				Shader.SetGlobalTexture("Ceto_Overlay_NormalMap", Texture2D.blackTexture);
			}
			if (data.height != null)
			{
				Shader.SetGlobalTexture("Ceto_Overlay_HeightMap", data.height);
			}
			else
			{
				Shader.SetGlobalTexture("Ceto_Overlay_HeightMap", Texture2D.blackTexture);
			}
			if (data.foam != null)
			{
				Shader.SetGlobalTexture("Ceto_Overlay_FoamMap", data.foam);
			}
			else
			{
				Shader.SetGlobalTexture("Ceto_Overlay_FoamMap", Texture2D.blackTexture);
			}
			if (data.clip != null)
			{
				Shader.SetGlobalTexture("Ceto_Overlay_ClipMap", data.clip);
			}
			else
			{
				Shader.SetGlobalTexture("Ceto_Overlay_ClipMap", Texture2D.blackTexture);
			}
			this.m_beenCleared = false;
		}

		// Token: 0x060003AF RID: 943 RVA: 0x00015A24 File Offset: 0x00013C24
		private void RenderHeightOverlays(IEnumerable<WaveOverlay> overlays, RenderTexture target)
		{
			if (target == null)
			{
				return;
			}
			foreach (WaveOverlay waveOverlay in overlays)
			{
				this.m_overlayMat.SetFloat("Ceto_Overlay_Alpha", waveOverlay.HeightTex.alpha);
				this.m_overlayMat.SetFloat("Ceto_Overlay_MaskAlpha", Mathf.Max(0f, waveOverlay.HeightTex.maskAlpha));
				this.m_overlayMat.SetTexture("Ceto_Overlay_Height", (!(waveOverlay.HeightTex.tex != null)) ? Texture2D.blackTexture : waveOverlay.HeightTex.tex);
				this.m_overlayMat.SetTexture("Ceto_Overlay_HeightMask", (!(waveOverlay.HeightTex.mask != null)) ? Texture2D.blackTexture : waveOverlay.HeightTex.mask);
				this.m_overlayMat.SetFloat("Ceto_Overlay_MaskMode", (float)waveOverlay.HeightTex.maskMode);
				this.Blit(waveOverlay.Corners, waveOverlay.HeightTex.scaleUV, waveOverlay.HeightTex.offsetUV, target, this.m_overlayMat, 0);
			}
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x00015B58 File Offset: 0x00013D58
		private void RenderNormalOverlays(IEnumerable<WaveOverlay> overlays, RenderTexture target)
		{
			if (target == null)
			{
				return;
			}
			foreach (WaveOverlay waveOverlay in overlays)
			{
				this.m_overlayMat.SetFloat("Ceto_Overlay_Alpha", Mathf.Max(0f, waveOverlay.NormalTex.alpha));
				this.m_overlayMat.SetFloat("Ceto_Overlay_MaskAlpha", Mathf.Max(0f, waveOverlay.NormalTex.maskAlpha));
				this.m_overlayMat.SetTexture("Ceto_Overlay_Normal", (!(waveOverlay.NormalTex.tex != null)) ? this.m_blankNormal : waveOverlay.NormalTex.tex);
				this.m_overlayMat.SetTexture("Ceto_Overlay_NormalMask", (!(waveOverlay.NormalTex.mask != null)) ? Texture2D.blackTexture : waveOverlay.NormalTex.mask);
				this.m_overlayMat.SetFloat("Ceto_Overlay_MaskMode", (float)waveOverlay.NormalTex.maskMode);
				this.Blit(waveOverlay.Corners, waveOverlay.NormalTex.scaleUV, waveOverlay.NormalTex.offsetUV, target, this.m_overlayMat, 1);
			}
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x00015C98 File Offset: 0x00013E98
		private void RenderFoamOverlays(IEnumerable<WaveOverlay> overlays, RenderTexture target)
		{
			if (target == null)
			{
				return;
			}
			foreach (WaveOverlay waveOverlay in overlays)
			{
				this.m_overlayMat.SetFloat("Ceto_Overlay_Alpha", Mathf.Max(0f, waveOverlay.FoamTex.alpha));
				this.m_overlayMat.SetFloat("Ceto_Overlay_MaskAlpha", Mathf.Max(0f, waveOverlay.FoamTex.maskAlpha));
				this.m_overlayMat.SetTexture("Ceto_Overlay_Foam", (!(waveOverlay.FoamTex.tex != null)) ? Texture2D.blackTexture : waveOverlay.FoamTex.tex);
				this.m_overlayMat.SetTexture("Ceto_Overlay_FoamMask", (!(waveOverlay.FoamTex.mask != null)) ? Texture2D.blackTexture : waveOverlay.FoamTex.mask);
				this.m_overlayMat.SetFloat("Ceto_Overlay_MaskMode", (float)waveOverlay.FoamTex.maskMode);
				this.m_overlayMat.SetVector("Ceto_TextureFoam", (!waveOverlay.FoamTex.textureFoam) ? OverlayManager.DONT_TEXTURE_FOAM : OverlayManager.TEXTURE_FOAM);
				this.Blit(waveOverlay.Corners, waveOverlay.FoamTex.scaleUV, waveOverlay.FoamTex.offsetUV, target, this.m_overlayMat, 2);
			}
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x00015E0C File Offset: 0x0001400C
		private void RenderClipOverlays(IEnumerable<WaveOverlay> overlays, RenderTexture target)
		{
			if (target == null)
			{
				return;
			}
			foreach (WaveOverlay waveOverlay in overlays)
			{
				this.m_overlayMat.SetFloat("Ceto_Overlay_Alpha", Mathf.Max(0f, waveOverlay.ClipTex.alpha));
				this.m_overlayMat.SetTexture("Ceto_Overlay_Clip", (!(waveOverlay.ClipTex.tex != null)) ? Texture2D.blackTexture : waveOverlay.ClipTex.tex);
				this.Blit(waveOverlay.Corners, waveOverlay.ClipTex.scaleUV, waveOverlay.ClipTex.offsetUV, target, this.m_overlayMat, 3);
			}
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x00015ED0 File Offset: 0x000140D0
		private void Blit(Vector4[] corners, Vector2 scale, Vector2 offset, RenderTexture des, Material mat, int pass)
		{
			Graphics.SetRenderTarget(des);
			GL.PushMatrix();
			GL.LoadOrtho();
			mat.SetPass(pass);
			GL.Begin(7);
			GL.MultiTexCoord2(0, offset.x, offset.y);
			GL.MultiTexCoord2(1, 0f, 0f);
			GL.Vertex(corners[0]);
			GL.MultiTexCoord2(0, offset.x + 1f * scale.x, offset.y);
			GL.MultiTexCoord2(1, 1f, 0f);
			GL.Vertex(corners[1]);
			GL.MultiTexCoord2(0, offset.x + 1f * scale.x, offset.y + 1f * scale.y);
			GL.MultiTexCoord2(1, 1f, 1f);
			GL.Vertex(corners[2]);
			GL.MultiTexCoord2(0, offset.x, offset.y + 1f * scale.y);
			GL.MultiTexCoord2(1, 0f, 1f);
			GL.Vertex(corners[3]);
			GL.End();
			GL.PopMatrix();
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x00016028 File Offset: 0x00014228
		public void CreateOverlays(Camera cam, WaveOverlayData overlay, OVERLAY_MAP_SIZE normalOverlaySize, OVERLAY_MAP_SIZE heightOverlaySize, OVERLAY_MAP_SIZE foamOverlaySize, OVERLAY_MAP_SIZE clipOverlaySize)
		{
			if (this.HasNormalOverlay)
			{
				RenderTextureFormat format = RenderTextureFormat.ARGBHalf;
				if (!SystemInfo.SupportsRenderTextureFormat(format))
				{
					format = RenderTextureFormat.ARGB32;
				}
				this.CreateBuffer("Normal", cam, normalOverlaySize, format, true, ref overlay.normal);
			}
			if (this.HasHeightOverlay)
			{
				RenderTextureFormat format2 = RenderTextureFormat.RGHalf;
				if (!SystemInfo.SupportsRenderTextureFormat(format2))
				{
					format2 = RenderTextureFormat.ARGB32;
				}
				this.CreateBuffer("Height", cam, heightOverlaySize, format2, true, ref overlay.height);
			}
			if (this.HasFoamOverlay)
			{
				RenderTextureFormat format3 = RenderTextureFormat.ARGB32;
				this.CreateBuffer("Foam", cam, foamOverlaySize, format3, false, ref overlay.foam);
			}
			if (this.HasClipOverlay)
			{
				RenderTextureFormat format4 = RenderTextureFormat.R8;
				if (!SystemInfo.SupportsRenderTextureFormat(format4))
				{
					format4 = RenderTextureFormat.RHalf;
				}
				if (!SystemInfo.SupportsRenderTextureFormat(format4))
				{
					format4 = RenderTextureFormat.ARGB32;
				}
				this.CreateBuffer("Clip", cam, clipOverlaySize, format4, true, ref overlay.clip);
			}
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x000160F8 File Offset: 0x000142F8
		public void CreateBuffer(string name, Camera cam, OVERLAY_MAP_SIZE size, RenderTextureFormat format, bool isLinear, ref RenderTexture map)
		{
			float num = this.SizeToValue(size);
			int num2 = Mathf.Min(4096, (int)((float)cam.pixelWidth * num));
			int num3 = Mathf.Min(4096, (int)((float)cam.pixelHeight * num));
			if (map == null || map.width != num2 || map.height != num3)
			{
				if (map != null)
				{
					RTUtility.ReleaseAndDestroy(map);
				}
				RenderTextureReadWrite readWrite = (!isLinear) ? RenderTextureReadWrite.Default : RenderTextureReadWrite.Linear;
				map = new RenderTexture(num2, num3, 0, format, readWrite);
				map.useMipMap = false;
				map.filterMode = FilterMode.Bilinear;
				map.hideFlags = HideFlags.DontSave;
				map.name = "Ceto Overlay " + name + " Buffer: " + cam.name;
			}
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x000161D0 File Offset: 0x000143D0
		private float SizeToValue(OVERLAY_MAP_SIZE size)
		{
			switch (size)
			{
			case OVERLAY_MAP_SIZE.DOUBLE:
				return 2f;
			case OVERLAY_MAP_SIZE.FULL_HALF:
				return 1.5f;
			case OVERLAY_MAP_SIZE.FULL:
				return 1f;
			case OVERLAY_MAP_SIZE.HALF:
				return 0.5f;
			case OVERLAY_MAP_SIZE.QUARTER:
				return 0.25f;
			default:
				return 1f;
			}
		}

		// Token: 0x040003C6 RID: 966
		private static readonly Vector2 TEXTURE_FOAM = new Vector2(1f, 0f);

		// Token: 0x040003C7 RID: 967
		private static readonly Vector2 DONT_TEXTURE_FOAM = new Vector2(0f, 1f);

		// Token: 0x040003C8 RID: 968
		private Material m_overlayMat;

		// Token: 0x040003C9 RID: 969
		private LinkedList<WaveOverlay> m_waveOverlays;

		// Token: 0x040003CA RID: 970
		private List<WaveOverlay> m_queryableOverlays;

		// Token: 0x040003CB RID: 971
		private List<WaveOverlay> m_removeOverlays;

		// Token: 0x040003CC RID: 972
		private List<QueryableOverlayResult> m_containingOverlays;

		// Token: 0x040003CD RID: 973
		private Texture2D m_blankNormal;

		// Token: 0x040003CE RID: 974
		private bool m_beenCleared;

		// Token: 0x040003CF RID: 975
		private List<WaveOverlay> m_heightOverlays;

		// Token: 0x040003D0 RID: 976
		private List<WaveOverlay> m_normalOverlays;

		// Token: 0x040003D1 RID: 977
		private List<WaveOverlay> m_foamOverlays;

		// Token: 0x040003D2 RID: 978
		private List<WaveOverlay> m_clipOverlays;

		// Token: 0x040003D3 RID: 979
		private Color m_clearColor;
	}
}
