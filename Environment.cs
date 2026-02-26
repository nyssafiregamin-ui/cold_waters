using System;
using UnityEngine;

// Token: 0x02000119 RID: 281
public class Environment : MonoBehaviour
{
	// Token: 0x04000A80 RID: 2688
	public Color actualAmbientColor;

	// Token: 0x04000A81 RID: 2689
	public Color actualSurfaceFogColor;

	// Token: 0x04000A82 RID: 2690
	public Color actualLightLevel;

	// Token: 0x04000A83 RID: 2691
	public float actualSurfaceFogDensity;

	// Token: 0x04000A84 RID: 2692
	public Color actualSubmergedFogColor;

	// Token: 0x04000A85 RID: 2693
	public float actualSubmergedFogDensity;

	// Token: 0x04000A86 RID: 2694
	public static Color treeColor;

	// Token: 0x04000A87 RID: 2695
	public Light[] directionalLights;

	// Token: 0x04000A88 RID: 2696
	public LensFlare primaryLensFlare;

	// Token: 0x04000A89 RID: 2697
	public static Color whiteLevel;

	// Token: 0x04000A8A RID: 2698
	public string[] amplifyColorTexturesSurface;

	// Token: 0x04000A8B RID: 2699
	public string[] amplifyColorTexturesUnderwater;
}
