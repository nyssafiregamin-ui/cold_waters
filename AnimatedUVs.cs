using System;
using UnityEngine;

// Token: 0x020000EC RID: 236
public class AnimatedUVs : MonoBehaviour
{
	// Token: 0x0600063F RID: 1599 RVA: 0x0002CCF8 File Offset: 0x0002AEF8
	private void LateUpdate()
	{
		this.uv1Offset += this.uv1AnimationRate * Time.deltaTime / 2f;
		this.uv2Offset += this.uv2AnimationRate * Time.deltaTime / 2f;
		if (base.GetComponent<Renderer>().enabled)
		{
			base.GetComponent<Renderer>().materials[this.materialIndex].SetTextureOffset(this.texture1Name, this.uv1Offset);
			base.GetComponent<Renderer>().materials[this.materialIndex].SetTextureOffset(this.texture2Name, this.uv2Offset);
		}
	}

	// Token: 0x06000640 RID: 1600 RVA: 0x0002CDB4 File Offset: 0x0002AFB4
	private void SetupWind()
	{
	}

	// Token: 0x040006A0 RID: 1696
	public int materialIndex;

	// Token: 0x040006A1 RID: 1697
	public Vector2 uv1AnimationRate;

	// Token: 0x040006A2 RID: 1698
	public Vector2 uv2AnimationRate;

	// Token: 0x040006A3 RID: 1699
	public string texture1Name = "_MainTex";

	// Token: 0x040006A4 RID: 1700
	public string texture2Name = "_Texture2";

	// Token: 0x040006A5 RID: 1701
	private Vector2 uv1Offset = Vector2.zero;

	// Token: 0x040006A6 RID: 1702
	private Vector2 uv2Offset = Vector2.zero;
}
