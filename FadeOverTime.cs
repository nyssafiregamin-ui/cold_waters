using System;
using UnityEngine;

// Token: 0x0200011F RID: 287
public class FadeOverTime : MonoBehaviour
{
	// Token: 0x060007B4 RID: 1972 RVA: 0x00048E78 File Offset: 0x00047078
	private void FixedUpdate()
	{
		this.currentColor.a = this.currentColor.a - Time.deltaTime * this.fadeRate;
		if (this.currentColor.a <= 0f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			this.linerenderer.SetColors(this.currentColor, this.lineColorEnd);
		}
	}

	// Token: 0x04000ADF RID: 2783
	public LineRenderer linerenderer;

	// Token: 0x04000AE0 RID: 2784
	public Color lineColor;

	// Token: 0x04000AE1 RID: 2785
	public Color lineColorEnd;

	// Token: 0x04000AE2 RID: 2786
	public Color currentColor;

	// Token: 0x04000AE3 RID: 2787
	public float fadeRate;
}
