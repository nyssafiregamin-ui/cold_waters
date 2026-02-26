using System;
using Ceto.Common.Containers.Interpolation;
using UnityEngine;

namespace Ceto
{
	// Token: 0x02000080 RID: 128
	[DisallowMultipleComponent]
	[AddComponentMenu("Ceto/Overlays/AddAutoShoreMask")]
	public class AddAutoShoreMask : AddWaveOverlayBase
	{
		// Token: 0x0600035F RID: 863 RVA: 0x000133A8 File Offset: 0x000115A8
		protected override void Start()
		{
			this.m_overlays.Add(new WaveOverlay());
			this.UpdateOverlay();
		}

		// Token: 0x06000360 RID: 864 RVA: 0x000133C0 File Offset: 0x000115C0
		protected override void Update()
		{
			if (this.m_overlays == null || this.m_overlays.Count != 1)
			{
				return;
			}
			this.UpdateOverlay();
		}

		// Token: 0x06000361 RID: 865 RVA: 0x000133E8 File Offset: 0x000115E8
		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.Release();
		}

		// Token: 0x06000362 RID: 866 RVA: 0x000133F8 File Offset: 0x000115F8
		private void UpdateOverlay()
		{
			if (Ocean.Instance != null && (!this.m_registered || this.SettingsChanged()))
			{
				this.CreateShoreMasks();
			}
			Vector2 halfSize = new Vector2(this.m_width * 0.5f, this.m_height * 0.5f);
			Vector3 position = base.transform.position;
			position.x += halfSize.x;
			position.z += halfSize.y;
			this.m_overlays[0].Position = position;
			this.m_overlays[0].HalfSize = halfSize;
			this.m_overlays[0].HeightTex.maskAlpha = ((!this.useHeightMask) ? 0f : this.heightAlpha);
			this.m_overlays[0].HeightTex.ignoreQuerys = this.ignoreQuerys;
			this.m_overlays[0].NormalTex.maskAlpha = ((!this.useNormalMask) ? 0f : this.normalAlpha);
			this.m_overlays[0].NormalTex.maskMode = OVERLAY_MASK_MODE.WAVES_AND_OVERLAY_BLEND;
			this.m_overlays[0].FoamTex.alpha = ((!this.useEdgeFoam) ? 0f : this.edgeFoamAlpha);
			this.m_overlays[0].FoamTex.textureFoam = this.textureFoam;
			this.m_overlays[0].FoamTex.maskAlpha = ((!this.useFoamMask) ? 0f : this.foamMaskAlpha);
			this.m_overlays[0].ClipTex.alpha = ((!this.useClipMask) ? 0f : 1f);
			this.m_overlays[0].ClipTex.ignoreQuerys = this.ignoreQuerys;
			this.m_overlays[0].UpdateOverlay();
		}

		// Token: 0x06000363 RID: 867 RVA: 0x0001361C File Offset: 0x0001181C
		private void CreateShoreMasks()
		{
			this.Release();
			Terrain component = base.GetComponent<Terrain>();
			if (component == null)
			{
				Ocean.LogWarning("The AddAutoShoreMask script must be attached to a component with a Terrain. The shore mask will not be created.");
				base.enabled = false;
				return;
			}
			if (component.terrainData == null)
			{
				Ocean.LogWarning("The terrain data is null. The shore mask will not be created.");
				base.enabled = false;
				return;
			}
			Vector3 size = component.terrainData.size;
			this.resolution = Mathf.Clamp(this.resolution, 32, 4096);
			this.m_width = size.x;
			this.m_height = size.z;
			float level = Ocean.Instance.level;
			float[] data = ShoreMaskGenerator.CreateHeightMap(component);
			int heightmapResolution = component.terrainData.heightmapResolution;
			InterpolatedArray2f heightMap = new InterpolatedArray2f(data, heightmapResolution, heightmapResolution, 1, false);
			if (this.useHeightMask || this.useNormalMask || this.useFoamMask)
			{
				this.m_heightMask = ShoreMaskGenerator.CreateMask(heightMap, this.resolution, this.resolution, level, this.heightSpread, TextureFormat.ARGB32);
			}
			if (this.useEdgeFoam)
			{
				this.m_edgeFoam = ShoreMaskGenerator.CreateMask(heightMap, this.resolution, this.resolution, level, this.foamSpread, TextureFormat.ARGB32);
			}
			if (this.useClipMask)
			{
				this.m_clipMask = ShoreMaskGenerator.CreateClipMask(heightMap, this.resolution, this.resolution, level + this.clipOffset, TextureFormat.ARGB32);
			}
			if (this.useHeightMask)
			{
				this.m_overlays[0].HeightTex.mask = this.m_heightMask;
			}
			if (this.useNormalMask)
			{
				this.m_overlays[0].NormalTex.mask = this.m_heightMask;
			}
			if (this.useFoamMask)
			{
				this.m_overlays[0].FoamTex.mask = this.m_heightMask;
			}
			if (this.useEdgeFoam)
			{
				this.m_overlays[0].FoamTex.tex = this.m_edgeFoam;
			}
			if (this.useClipMask)
			{
				this.m_overlays[0].ClipTex.tex = this.m_clipMask;
			}
			if (!this.m_registered)
			{
				Ocean.Instance.OverlayManager.Add(this.m_overlays[0]);
				this.m_registered = true;
			}
			this.m_heightSpread = this.heightSpread;
			this.m_foamSpread = this.foamSpread;
			this.m_clipOffset = this.clipOffset;
			this.m_resolution = (float)this.resolution;
		}

		// Token: 0x06000364 RID: 868 RVA: 0x000138A0 File Offset: 0x00011AA0
		private bool SettingsChanged()
		{
			return this.m_heightSpread != this.heightSpread || this.m_foamSpread != this.foamSpread || this.m_clipOffset != this.clipOffset || this.m_resolution != (float)this.resolution;
		}

		// Token: 0x06000365 RID: 869 RVA: 0x000138FC File Offset: 0x00011AFC
		private void Release()
		{
			if (this.m_heightMask != null)
			{
				UnityEngine.Object.Destroy(this.m_heightMask);
				this.m_heightMask = null;
			}
			if (this.m_edgeFoam != null)
			{
				UnityEngine.Object.Destroy(this.m_edgeFoam);
				this.m_edgeFoam = null;
			}
			if (this.m_clipMask != null)
			{
				UnityEngine.Object.Destroy(this.m_clipMask);
				this.m_clipMask = null;
			}
			if (this.m_overlays != null && this.m_overlays.Count == 1)
			{
				this.m_overlays[0].HeightTex.mask = null;
				this.m_overlays[0].NormalTex.mask = null;
				this.m_overlays[0].FoamTex.mask = null;
				this.m_overlays[0].FoamTex.tex = null;
				this.m_overlays[0].ClipTex.tex = null;
			}
		}

		// Token: 0x0400035E RID: 862
		public bool ignoreQuerys;

		// Token: 0x0400035F RID: 863
		public bool textureFoam = true;

		// Token: 0x04000360 RID: 864
		[Range(0.1f, 100f)]
		public float heightSpread = 10f;

		// Token: 0x04000361 RID: 865
		[Range(0.1f, 10f)]
		public float foamSpread = 2f;

		// Token: 0x04000362 RID: 866
		[Range(0.1f, 10f)]
		public float clipOffset = 4f;

		// Token: 0x04000363 RID: 867
		public int resolution = 1024;

		// Token: 0x04000364 RID: 868
		public bool useHeightMask = true;

		// Token: 0x04000365 RID: 869
		[Range(0f, 1f)]
		public float heightAlpha = 0.9f;

		// Token: 0x04000366 RID: 870
		public bool useNormalMask = true;

		// Token: 0x04000367 RID: 871
		[Range(0f, 1f)]
		public float normalAlpha = 0.8f;

		// Token: 0x04000368 RID: 872
		public bool useEdgeFoam = true;

		// Token: 0x04000369 RID: 873
		[Range(0f, 10f)]
		public float edgeFoamAlpha = 1f;

		// Token: 0x0400036A RID: 874
		public bool useFoamMask = true;

		// Token: 0x0400036B RID: 875
		[Range(0f, 1f)]
		public float foamMaskAlpha = 1f;

		// Token: 0x0400036C RID: 876
		public bool useClipMask = true;

		// Token: 0x0400036D RID: 877
		private bool m_registered;

		// Token: 0x0400036E RID: 878
		private float m_width;

		// Token: 0x0400036F RID: 879
		private float m_height;

		// Token: 0x04000370 RID: 880
		private Texture2D m_heightMask;

		// Token: 0x04000371 RID: 881
		private Texture2D m_edgeFoam;

		// Token: 0x04000372 RID: 882
		private Texture2D m_clipMask;

		// Token: 0x04000373 RID: 883
		private float m_heightSpread;

		// Token: 0x04000374 RID: 884
		private float m_foamSpread;

		// Token: 0x04000375 RID: 885
		private float m_clipOffset;

		// Token: 0x04000376 RID: 886
		private float m_resolution;
	}
}
