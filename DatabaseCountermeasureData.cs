using System;
using UnityEngine;

// Token: 0x02000109 RID: 265
public class DatabaseCountermeasureData : ScriptableObject
{
	// Token: 0x0400092E RID: 2350
	public int countermeasureID;

	// Token: 0x0400092F RID: 2351
	public GameObject countermeasureObject;

	// Token: 0x04000930 RID: 2352
	public string countermeasureName;

	// Token: 0x04000931 RID: 2353
	public bool isNoisemaker;

	// Token: 0x04000932 RID: 2354
	public bool isKnuckle;

	// Token: 0x04000933 RID: 2355
	public bool isChaff;

	// Token: 0x04000934 RID: 2356
	public float lifetime;

	// Token: 0x04000935 RID: 2357
	public float fallSpeed;

	// Token: 0x04000936 RID: 2358
	public float noiseStrength;

	// Token: 0x04000937 RID: 2359
	public string[] countermeasureDescription;

	// Token: 0x04000938 RID: 2360
	public GameObject countermeasureParticle;
}
