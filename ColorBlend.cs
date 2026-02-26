using System;
using UnityEngine;

// Token: 0x020000FB RID: 251
public class ColorBlend : MonoBehaviour
{
	// Token: 0x060006C7 RID: 1735 RVA: 0x00037F74 File Offset: 0x00036174
	private void Update()
	{
		if (!ManualCameraZoom.underwater)
		{
			this.rrange = this.oceanacute.r - this.oceanobtuse.r;
			this.grange = this.oceanacute.g - this.oceanobtuse.g;
			this.brange = this.oceanacute.b - this.oceanobtuse.b;
			float num = 1f - ManualCameraZoom.y / 90f;
			this.r = num * this.rrange + this.oceanobtuse.r;
			this.g = num * this.grange + this.oceanobtuse.g;
			this.b = num * this.brange + this.oceanobtuse.b;
			this.oceanrenderer.material.color = new Color(this.r, this.g, this.b, 1f);
		}
	}

	// Token: 0x04000808 RID: 2056
	public LevelLoadManager levelloadmanager;

	// Token: 0x04000809 RID: 2057
	public Color oceanacute;

	// Token: 0x0400080A RID: 2058
	public Color oceanobtuse;

	// Token: 0x0400080B RID: 2059
	public float r;

	// Token: 0x0400080C RID: 2060
	public float g;

	// Token: 0x0400080D RID: 2061
	public float b;

	// Token: 0x0400080E RID: 2062
	public float rrange;

	// Token: 0x0400080F RID: 2063
	public float grange;

	// Token: 0x04000810 RID: 2064
	public float brange;

	// Token: 0x04000811 RID: 2065
	public float percentage;

	// Token: 0x04000812 RID: 2066
	public Renderer oceanrenderer;

	// Token: 0x04000813 RID: 2067
	public Renderer oceansmallrenderer;

	// Token: 0x04000814 RID: 2068
	public GameObject lightRays;
}
