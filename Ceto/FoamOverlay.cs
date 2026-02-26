using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x0200008A RID: 138
	public class FoamOverlay : WaveOverlay
	{
		// Token: 0x0600038E RID: 910 RVA: 0x00014DFC File Offset: 0x00012FFC
		public FoamOverlay(Vector3 pos, float rotation, float size, float duration, Texture texture) : base(pos, rotation, new Vector2(size * 0.5f, size * 0.5f), duration)
		{
			this.Size = size;
			base.FoamTex.tex = texture;
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x0600038F RID: 911 RVA: 0x00014E30 File Offset: 0x00013030
		// (set) Token: 0x06000390 RID: 912 RVA: 0x00014E38 File Offset: 0x00013038
		public Vector3 Momentum { get; set; }

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000391 RID: 913 RVA: 0x00014E44 File Offset: 0x00013044
		// (set) Token: 0x06000392 RID: 914 RVA: 0x00014E4C File Offset: 0x0001304C
		public float Spin { get; set; }

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000393 RID: 915 RVA: 0x00014E58 File Offset: 0x00013058
		// (set) Token: 0x06000394 RID: 916 RVA: 0x00014E60 File Offset: 0x00013060
		public float Expansion { get; set; }

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000395 RID: 917 RVA: 0x00014E6C File Offset: 0x0001306C
		// (set) Token: 0x06000396 RID: 918 RVA: 0x00014E74 File Offset: 0x00013074
		public float Size { get; set; }

		// Token: 0x06000397 RID: 919 RVA: 0x00014E80 File Offset: 0x00013080
		public override void UpdateOverlay()
		{
			base.Position += this.Momentum * Time.deltaTime;
			this.Size += this.Expansion * Time.deltaTime;
			base.Rotation += this.Spin * Time.deltaTime;
			base.HalfSize = new Vector2(this.Size * 0.5f, this.Size * 0.5f);
			base.UpdateOverlay();
		}
	}
}
