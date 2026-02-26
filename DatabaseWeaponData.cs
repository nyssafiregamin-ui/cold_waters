using System;
using UnityEngine;

// Token: 0x02000104 RID: 260
public class DatabaseWeaponData : ScriptableObject
{
	// Token: 0x040008D5 RID: 2261
	public int weaponID;

	// Token: 0x040008D6 RID: 2262
	public GameObject weaponObject;

	// Token: 0x040008D7 RID: 2263
	public string weaponPrefabName;

	// Token: 0x040008D8 RID: 2264
	public Sprite weaponImage;

	// Token: 0x040008D9 RID: 2265
	public string weaponName;

	// Token: 0x040008DA RID: 2266
	public string weaponDescriptiveName;

	// Token: 0x040008DB RID: 2267
	public string weaponType;

	// Token: 0x040008DC RID: 2268
	public bool hasPayload;

	// Token: 0x040008DD RID: 2269
	public int missilePayload;

	// Token: 0x040008DE RID: 2270
	public bool isMissile;

	// Token: 0x040008DF RID: 2271
	public bool isDecoy;

	// Token: 0x040008E0 RID: 2272
	public bool isSonobuoy;

	// Token: 0x040008E1 RID: 2273
	public bool surfaceLaunched;

	// Token: 0x040008E2 RID: 2274
	public float maxLaunchDepth = 1000f;

	// Token: 0x040008E3 RID: 2275
	public bool wireGuided;

	// Token: 0x040008E4 RID: 2276
	public float wireBreakOnLaunchProbability = 0.1f;

	// Token: 0x040008E5 RID: 2277
	public float wireBreakSpeedThreshold = 1.5f;

	// Token: 0x040008E6 RID: 2278
	public float wireBreakThreshold = 30f;

	// Token: 0x040008E7 RID: 2279
	public bool landAttack;

	// Token: 0x040008E8 RID: 2280
	public float warhead;

	// Token: 0x040008E9 RID: 2281
	public bool swimOut;

	// Token: 0x040008EA RID: 2282
	public Vector2 noiseValues;

	// Token: 0x040008EB RID: 2283
	public bool isDumbfire;

	// Token: 0x040008EC RID: 2284
	public float rangeInYards;

	// Token: 0x040008ED RID: 2285
	public float runTime;

	// Token: 0x040008EE RID: 2286
	public float runSpeed;

	// Token: 0x040008EF RID: 2287
	public float cruiseAltitude;

	// Token: 0x040008F0 RID: 2288
	public float activeRunSpeed;

	// Token: 0x040008F1 RID: 2289
	public float turnRate;

	// Token: 0x040008F2 RID: 2290
	public Vector2 sensorAngles;

	// Token: 0x040008F3 RID: 2291
	public float sensorRange;

	// Token: 0x040008F4 RID: 2292
	public Vector2 missileFiringRange;

	// Token: 0x040008F5 RID: 2293
	public float maxPitchAngle = 7f;

	// Token: 0x040008F6 RID: 2294
	public float boosterReleasedAfterSeconds;

	// Token: 0x040008F7 RID: 2295
	public string[] searchSettings;

	// Token: 0x040008F8 RID: 2296
	public string[] heightSettings;

	// Token: 0x040008F9 RID: 2297
	public string[] homeSettings;

	// Token: 0x040008FA RID: 2298
	public float fixedRunDepth = -1f;

	// Token: 0x040008FB RID: 2299
	public float actualRunSpeed;

	// Token: 0x040008FC RID: 2300
	public float actualActiveRunSpeed;

	// Token: 0x040008FD RID: 2301
	public float actualSensorRange;

	// Token: 0x040008FE RID: 2302
	public string[] weaponDescription;

	// Token: 0x040008FF RID: 2303
	public float minCameraDistance;

	// Token: 0x04000900 RID: 2304
	public float replenishTime;

	// Token: 0x04000901 RID: 2305
	public GameObject cavitationParticle;

	// Token: 0x04000902 RID: 2306
	public GameObject missileTrail;

	// Token: 0x04000903 RID: 2307
	public GameObject boosterParticle;

	// Token: 0x04000904 RID: 2308
	public GameObject parachute;
}
