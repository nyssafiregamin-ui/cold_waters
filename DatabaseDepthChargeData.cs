using System;
using UnityEngine;

// Token: 0x02000105 RID: 261
public class DatabaseDepthChargeData : ScriptableObject
{
	// Token: 0x04000905 RID: 2309
	public int depthChargeID;

	// Token: 0x04000906 RID: 2310
	public string depthChargePrefabReference;

	// Token: 0x04000907 RID: 2311
	public bool isProximityMine;

	// Token: 0x04000908 RID: 2312
	public GameObject depthChargeObject;

	// Token: 0x04000909 RID: 2313
	public string depthchargeName;

	// Token: 0x0400090A RID: 2314
	public string depthchargeDescriptiveName;

	// Token: 0x0400090B RID: 2315
	public float warhead;

	// Token: 0x0400090C RID: 2316
	public Vector2 weaponRange;

	// Token: 0x0400090D RID: 2317
	public bool contactExploded;

	// Token: 0x0400090E RID: 2318
	public bool depthExploded;

	// Token: 0x0400090F RID: 2319
	public float killRadius;

	// Token: 0x04000910 RID: 2320
	public int numberOfMortars;

	// Token: 0x04000911 RID: 2321
	public float rateOfFire;

	// Token: 0x04000912 RID: 2322
	public float reloadTime;

	// Token: 0x04000913 RID: 2323
	public float spreadRadius;

	// Token: 0x04000914 RID: 2324
	public float velocity;

	// Token: 0x04000915 RID: 2325
	public float sinkRate;

	// Token: 0x04000916 RID: 2326
	public float[] mortarPositions;

	// Token: 0x04000917 RID: 2327
	public Vector2 firingPositions;

	// Token: 0x04000918 RID: 2328
	public GameObject bubbles;

	// Token: 0x04000919 RID: 2329
	public GameObject launchFlare;
}
