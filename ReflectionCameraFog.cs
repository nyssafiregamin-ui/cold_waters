using System;
using UnityEngine;

// Token: 0x02000150 RID: 336
public class ReflectionCameraFog : MonoBehaviour
{
	// Token: 0x060009B8 RID: 2488 RVA: 0x000702DC File Offset: 0x0006E4DC
	private void OnPreRender()
	{
		this.previousFogDensity = RenderSettings.fogDensity;
		RenderSettings.fogDensity *= 25f;
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x000702FC File Offset: 0x0006E4FC
	private void OnPostRender()
	{
		RenderSettings.fogDensity = this.previousFogDensity;
	}

	// Token: 0x04000E94 RID: 3732
	private float previousFogDensity;
}
