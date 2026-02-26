using System;
using UnityEngine;

// Token: 0x0200010A RID: 266
public class DatabaseAircraftData : ScriptableObject
{
	// Token: 0x04000939 RID: 2361
	public int aircraftID;

	// Token: 0x0400093A RID: 2362
	public string aircraftPrefabName;

	// Token: 0x0400093B RID: 2363
	public string aircraftName;

	// Token: 0x0400093C RID: 2364
	public string aircraftDescriptiveName;

	// Token: 0x0400093D RID: 2365
	public string aircraftType;

	// Token: 0x0400093E RID: 2366
	public float cruiseSpeed;

	// Token: 0x0400093F RID: 2367
	public float length;

	// Token: 0x04000940 RID: 2368
	public float height;

	// Token: 0x04000941 RID: 2369
	public int weight;

	// Token: 0x04000942 RID: 2370
	public int crew;

	// Token: 0x04000943 RID: 2371
	public string[] torpedotypes;

	// Token: 0x04000944 RID: 2372
	public int[] torpedoIDs;

	// Token: 0x04000945 RID: 2373
	public int[] torpedoNumbers;

	// Token: 0x04000946 RID: 2374
	public int torpedotubes;

	// Token: 0x04000947 RID: 2375
	public int[] depthBombIDs = new int[0];

	// Token: 0x04000948 RID: 2376
	public int[] depthBombs = new int[0];

	// Token: 0x04000949 RID: 2377
	public string[] sonobuoytypes = new string[0];

	// Token: 0x0400094A RID: 2378
	public int[] sonobuoyIDs = new int[0];

	// Token: 0x0400094B RID: 2379
	public int[] sonobuoyNumbers = new int[0];

	// Token: 0x0400094C RID: 2380
	public int sonobuoytubes;

	// Token: 0x0400094D RID: 2381
	public int activeSonarID;

	// Token: 0x0400094E RID: 2382
	public int passiveSonarID;

	// Token: 0x0400094F RID: 2383
	public int radarID;

	// Token: 0x04000950 RID: 2384
	public string radarSignature;

	// Token: 0x04000951 RID: 2385
	public string[] aircraftDescription;

	// Token: 0x04000952 RID: 2386
	public float minCameraDistance;

	// Token: 0x04000953 RID: 2387
	public string audioFile;
}
