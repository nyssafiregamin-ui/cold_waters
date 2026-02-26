using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000E1 RID: 225
public class tfQuad
{
	// Token: 0x060005FC RID: 1532 RVA: 0x00029464 File Offset: 0x00027664
	public tfQuad(float x, float y, float z)
	{
		this.pos = new Vector3(x, y, z);
		tfQuad.quads.Add(this);
	}

	// Token: 0x0400062C RID: 1580
	public static List<tfQuad> quads = new List<tfQuad>();

	// Token: 0x0400062D RID: 1581
	public Vector3 pos;

	// Token: 0x0400062E RID: 1582
	public float scale = 1f;
}
