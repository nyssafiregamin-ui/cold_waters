using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x02000087 RID: 135
	[AddComponentMenu("Ceto/Overlays/AddWaveOverlay")]
	public class AddWaveOverlay : AddWaveOverlayBase
	{
		// Token: 0x06000381 RID: 897 RVA: 0x00014840 File Offset: 0x00012A40
		protected override void Start()
		{
			if (this.checkTextures)
			{
				if (!this.heightTexture.ignoreQuerys)
				{
					base.CheckCanSampleTex(this.heightTexture.tex, "height texture");
				}
				if (!this.heightTexture.ignoreQuerys)
				{
					base.CheckCanSampleTex(this.heightTexture.mask, "height mask");
				}
				if (!this.clipTexture.ignoreQuerys)
				{
					base.CheckCanSampleTex(this.clipTexture.tex, "clip texture");
				}
			}
			Vector2 halfSize = new Vector2(this.width * 0.5f, this.height * 0.5f);
			this.m_overlays.Add(new WaveOverlay(base.transform.position, this.Rotation(), halfSize, 0f));
			this.m_overlays[0].HeightTex = this.heightTexture;
			this.m_overlays[0].NormalTex = this.normalTexture;
			this.m_overlays[0].FoamTex = this.foamTexture;
			this.m_overlays[0].ClipTex = this.clipTexture;
			if (!this.m_registered && Ocean.Instance != null)
			{
				Ocean.Instance.OverlayManager.Add(this.m_overlays[0]);
				this.m_registered = true;
			}
		}

		// Token: 0x06000382 RID: 898 RVA: 0x000149AC File Offset: 0x00012BAC
		protected override void Update()
		{
			if (this.m_overlays == null || this.m_overlays.Count != 1)
			{
				return;
			}
			if (!this.m_registered && Ocean.Instance != null)
			{
				Ocean.Instance.OverlayManager.Add(this.m_overlays[0]);
				this.m_registered = true;
			}
			this.m_overlays[0].Position = base.transform.position;
			this.m_overlays[0].HalfSize = new Vector2(this.width * 0.5f, this.height * 0.5f);
			this.m_overlays[0].Rotation = this.Rotation();
			this.m_overlays[0].UpdateOverlay();
		}

		// Token: 0x06000383 RID: 899 RVA: 0x00014A88 File Offset: 0x00012C88
		private float Rotation()
		{
			AddWaveOverlay.ROTATION rotationMode = this.m_rotationMode;
			if (rotationMode == AddWaveOverlay.ROTATION.RELATIVE_TO_PARENT)
			{
				return base.transform.eulerAngles.y + this.rotation;
			}
			if (rotationMode != AddWaveOverlay.ROTATION.INDEPENDANT_TO_PARENT)
			{
				return this.rotation;
			}
			return this.rotation;
		}

		// Token: 0x06000384 RID: 900 RVA: 0x00014AD8 File Offset: 0x00012CD8
		private void OnDrawGizmos()
		{
			if (!base.enabled)
			{
				return;
			}
			Vector3 s = new Vector3(this.width * 0.5f, 1f, this.height * 0.5f);
			Vector3 position = base.transform.position;
			Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(position.x, 0f, position.z), Quaternion.Euler(0f, this.Rotation(), 0f), s);
			Gizmos.color = Color.yellow;
			Gizmos.matrix = matrix;
			Gizmos.DrawWireCube(Vector3.zero, new Vector3(2f, 10f, 2f));
		}

		// Token: 0x040003B4 RID: 948
		public bool checkTextures = true;

		// Token: 0x040003B5 RID: 949
		public OverlayHeightTexture heightTexture;

		// Token: 0x040003B6 RID: 950
		public OverlayNormalTexture normalTexture;

		// Token: 0x040003B7 RID: 951
		public OverlayFoamTexture foamTexture;

		// Token: 0x040003B8 RID: 952
		public OverlayClipTexture clipTexture;

		// Token: 0x040003B9 RID: 953
		public float width = 10f;

		// Token: 0x040003BA RID: 954
		public float height = 10f;

		// Token: 0x040003BB RID: 955
		public AddWaveOverlay.ROTATION m_rotationMode;

		// Token: 0x040003BC RID: 956
		[Range(0f, 360f)]
		public float rotation;

		// Token: 0x040003BD RID: 957
		private bool m_registered;

		// Token: 0x02000088 RID: 136
		public enum ROTATION
		{
			// Token: 0x040003BF RID: 959
			RELATIVE_TO_PARENT,
			// Token: 0x040003C0 RID: 960
			INDEPENDANT_TO_PARENT
		}
	}
}
