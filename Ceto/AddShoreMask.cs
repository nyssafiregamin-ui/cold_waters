using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x02000085 RID: 133
	[AddComponentMenu("Ceto/Overlays/AddShoreMask")]
	public class AddShoreMask : AddWaveOverlayBase
	{
		// Token: 0x0600037B RID: 891 RVA: 0x0001445C File Offset: 0x0001265C
		protected override void Start()
		{
			if (this.checkTextures && !this.ignoreQuerys)
			{
				base.CheckCanSampleTex(this.heightMask, "height mask");
				base.CheckCanSampleTex(this.clipMask, "clip mask");
			}
			this.m_overlays.Add(new WaveOverlay());
			this.UpdateOverlay();
		}

		// Token: 0x0600037C RID: 892 RVA: 0x000144B8 File Offset: 0x000126B8
		protected override void Update()
		{
			if (this.m_overlays == null || this.m_overlays.Count != 1)
			{
				return;
			}
			this.UpdateOverlay();
		}

		// Token: 0x0600037D RID: 893 RVA: 0x000144E0 File Offset: 0x000126E0
		private void UpdateOverlay()
		{
			if (!this.m_registered && Ocean.Instance != null)
			{
				Ocean.Instance.OverlayManager.Add(this.m_overlays[0]);
				this.m_registered = true;
			}
			this.m_overlays[0].Position = base.transform.position + this.offset;
			this.m_overlays[0].HalfSize = new Vector2(this.width * 0.5f, this.height * 0.5f);
			this.m_overlays[0].Rotation = this.Rotation();
			this.m_overlays[0].HeightTex.mask = this.heightMask;
			this.m_overlays[0].HeightTex.maskAlpha = this.heightAlpha;
			this.m_overlays[0].HeightTex.ignoreQuerys = this.ignoreQuerys;
			this.m_overlays[0].NormalTex.mask = this.normalMask;
			this.m_overlays[0].NormalTex.maskAlpha = this.normalAlpha;
			this.m_overlays[0].NormalTex.maskMode = OVERLAY_MASK_MODE.WAVES_AND_OVERLAY_BLEND;
			this.m_overlays[0].FoamTex.tex = this.edgeFoam;
			this.m_overlays[0].FoamTex.alpha = this.edgeFoamAlpha;
			this.m_overlays[0].FoamTex.textureFoam = this.textureFoam;
			this.m_overlays[0].FoamTex.mask = this.foamMask;
			this.m_overlays[0].FoamTex.maskAlpha = this.foamMaskAlpha;
			this.m_overlays[0].ClipTex.tex = this.clipMask;
			this.m_overlays[0].ClipTex.ignoreQuerys = this.ignoreQuerys;
			this.m_overlays[0].UpdateOverlay();
		}

		// Token: 0x0600037E RID: 894 RVA: 0x00014710 File Offset: 0x00012910
		private float Rotation()
		{
			AddShoreMask.ROTATION rotationMode = this.m_rotationMode;
			if (rotationMode == AddShoreMask.ROTATION.RELATIVE_TO_PARENT)
			{
				return base.transform.eulerAngles.y + this.rotation;
			}
			if (rotationMode != AddShoreMask.ROTATION.INDEPENDANT_TO_PARENT)
			{
				return this.rotation;
			}
			return this.rotation;
		}

		// Token: 0x0600037F RID: 895 RVA: 0x00014760 File Offset: 0x00012960
		private void OnDrawGizmos()
		{
			if (!base.enabled)
			{
				return;
			}
			Vector3 s = new Vector3(this.width * 0.5f, 1f, this.height * 0.5f);
			Vector3 vector = base.transform.position + this.offset;
			Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(vector.x, 0f, vector.z), Quaternion.Euler(0f, this.Rotation(), 0f), s);
			Gizmos.color = Color.yellow;
			Gizmos.matrix = matrix;
			Gizmos.DrawWireCube(Vector3.zero, new Vector3(2f, 10f, 2f));
		}

		// Token: 0x0400039F RID: 927
		public bool checkTextures = true;

		// Token: 0x040003A0 RID: 928
		public bool ignoreQuerys;

		// Token: 0x040003A1 RID: 929
		public Texture heightMask;

		// Token: 0x040003A2 RID: 930
		[Range(0f, 1f)]
		public float heightAlpha = 1f;

		// Token: 0x040003A3 RID: 931
		public Texture normalMask;

		// Token: 0x040003A4 RID: 932
		[Range(0f, 1f)]
		public float normalAlpha = 1f;

		// Token: 0x040003A5 RID: 933
		public Texture edgeFoam;

		// Token: 0x040003A6 RID: 934
		[Range(0f, 10f)]
		public float edgeFoamAlpha = 1f;

		// Token: 0x040003A7 RID: 935
		public bool textureFoam = true;

		// Token: 0x040003A8 RID: 936
		public Texture foamMask;

		// Token: 0x040003A9 RID: 937
		[Range(0f, 1f)]
		public float foamMaskAlpha = 1f;

		// Token: 0x040003AA RID: 938
		public Texture clipMask;

		// Token: 0x040003AB RID: 939
		public float width = 10f;

		// Token: 0x040003AC RID: 940
		public float height = 10f;

		// Token: 0x040003AD RID: 941
		public Vector3 offset;

		// Token: 0x040003AE RID: 942
		public AddShoreMask.ROTATION m_rotationMode;

		// Token: 0x040003AF RID: 943
		[Range(0f, 360f)]
		public float rotation;

		// Token: 0x040003B0 RID: 944
		private bool m_registered;

		// Token: 0x02000086 RID: 134
		public enum ROTATION
		{
			// Token: 0x040003B2 RID: 946
			RELATIVE_TO_PARENT,
			// Token: 0x040003B3 RID: 947
			INDEPENDANT_TO_PARENT
		}
	}
}
