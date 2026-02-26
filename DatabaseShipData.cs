using System;
using UnityEngine;

// Token: 0x0200010B RID: 267
public class DatabaseShipData : ScriptableObject
{
	// Token: 0x04000954 RID: 2388
	public int shipID;

	// Token: 0x04000955 RID: 2389
	public string shipPrefabName;

	// Token: 0x04000956 RID: 2390
	public string shipclass;

	// Token: 0x04000957 RID: 2391
	public string shipDesignation;

	// Token: 0x04000958 RID: 2392
	public string shipType;

	// Token: 0x04000959 RID: 2393
	public string playerHUD = "hud/default";

	// Token: 0x0400095A RID: 2394
	public float displacement;

	// Token: 0x0400095B RID: 2395
	public float length;

	// Token: 0x0400095C RID: 2396
	public float beam;

	// Token: 0x0400095D RID: 2397
	public float displayLength;

	// Token: 0x0400095E RID: 2398
	public float displayBeam;

	// Token: 0x0400095F RID: 2399
	public float hullHeight;

	// Token: 0x04000960 RID: 2400
	public float range;

	// Token: 0x04000961 RID: 2401
	public float crew;

	// Token: 0x04000962 RID: 2402
	public int[] aircraftNumbers;

	// Token: 0x04000963 RID: 2403
	public int[] aircraftIDs;

	// Token: 0x04000964 RID: 2404
	public string[] torpedotypes;

	// Token: 0x04000965 RID: 2405
	public int[] torpedoIDs;

	// Token: 0x04000966 RID: 2406
	public GameObject[] torpedoGameObjects;

	// Token: 0x04000967 RID: 2407
	public int[] torpedoNumbers;

	// Token: 0x04000968 RID: 2408
	public int torpedotubes;

	// Token: 0x04000969 RID: 2409
	public int numberOfWires;

	// Token: 0x0400096A RID: 2410
	public int[] torpedoConfig;

	// Token: 0x0400096B RID: 2411
	public float torpedotubeSize;

	// Token: 0x0400096C RID: 2412
	public float tubereloadtime;

	// Token: 0x0400096D RID: 2413
	public string[] vlsTorpedotypes;

	// Token: 0x0400096E RID: 2414
	public int[] vlsTorpedoIDs;

	// Token: 0x0400096F RID: 2415
	public GameObject[] vlsTorpedoGameObjects;

	// Token: 0x04000970 RID: 2416
	public int[] vlsTorpedoNumbers;

	// Token: 0x04000971 RID: 2417
	public float vlsMaxDepthToFire;

	// Token: 0x04000972 RID: 2418
	public float vlsMaxSpeedToFire;

	// Token: 0x04000973 RID: 2419
	public int missileType;

	// Token: 0x04000974 RID: 2420
	public GameObject missileGameObject;

	// Token: 0x04000975 RID: 2421
	public int[] missilesPerLauncher;

	// Token: 0x04000976 RID: 2422
	public bool[] missileLauncherElevates;

	// Token: 0x04000977 RID: 2423
	public float[] missileLauncherElevationMin;

	// Token: 0x04000978 RID: 2424
	public float[] missileLauncherElevationMax;

	// Token: 0x04000979 RID: 2425
	public bool[] missileLauncherPivots;

	// Token: 0x0400097A RID: 2426
	public float[] missileLauncherFiringArcBearingMin;

	// Token: 0x0400097B RID: 2427
	public float[] missileLauncherFiringArcBearingMax;

	// Token: 0x0400097C RID: 2428
	public float[] missileLauncherRestAngle;

	// Token: 0x0400097D RID: 2429
	public int[] navalGunTypes;

	// Token: 0x0400097E RID: 2430
	public float[] navalGunFiringArcMin;

	// Token: 0x0400097F RID: 2431
	public float[] navalGunFiringArcMax;

	// Token: 0x04000980 RID: 2432
	public float[] navalGunRestAngle;

	// Token: 0x04000981 RID: 2433
	public GameObject navalGunParticleEffect;

	// Token: 0x04000982 RID: 2434
	public GameObject navalGunSmokeEffect;

	// Token: 0x04000983 RID: 2435
	public bool[] rearArcFiring;

	// Token: 0x04000984 RID: 2436
	public int[] rbuLauncherTypes;

	// Token: 0x04000985 RID: 2437
	public int[] rbuSalvos;

	// Token: 0x04000986 RID: 2438
	public float[] rbuFiringArcMin;

	// Token: 0x04000987 RID: 2439
	public float[] rbuFiringArcMax;

	// Token: 0x04000988 RID: 2440
	public bool hasnoisemaker;

	// Token: 0x04000989 RID: 2441
	public int noiseMakerID;

	// Token: 0x0400098A RID: 2442
	public int numberofnoisemakers;

	// Token: 0x0400098B RID: 2443
	public float noisemakerreloadtime;

	// Token: 0x0400098C RID: 2444
	public float gunProbability;

	// Token: 0x0400098D RID: 2445
	public float gunRange;

	// Token: 0x0400098E RID: 2446
	public float[] gunFiringArcStart;

	// Token: 0x0400098F RID: 2447
	public float[] gunFiringArcFinish;

	// Token: 0x04000990 RID: 2448
	public float[] gunRestAngle;

	// Token: 0x04000991 RID: 2449
	public float[] gunRadarRestAngles;

	// Token: 0x04000992 RID: 2450
	public int[] gunUsesRadar;

	// Token: 0x04000993 RID: 2451
	public float chaffProbability;

	// Token: 0x04000994 RID: 2452
	public int chaffID;

	// Token: 0x04000995 RID: 2453
	public int numberChafflaunched;

	// Token: 0x04000996 RID: 2454
	public string ciwsParticle = "ships/particles/tracers_burst";

	// Token: 0x04000997 RID: 2455
	public float waterline;

	// Token: 0x04000998 RID: 2456
	public int periscopeDepthInFeet = 45;

	// Token: 0x04000999 RID: 2457
	public float accelerationrate;

	// Token: 0x0400099A RID: 2458
	public float decellerationrate;

	// Token: 0x0400099B RID: 2459
	public float rudderturnrate;

	// Token: 0x0400099C RID: 2460
	public float turnrate;

	// Token: 0x0400099D RID: 2461
	public float pivotpointturning;

	// Token: 0x0400099E RID: 2462
	public float diverate;

	// Token: 0x0400099F RID: 2463
	public float surfacerate;

	// Token: 0x040009A0 RID: 2464
	public float ballastrate;

	// Token: 0x040009A1 RID: 2465
	public float submergedat;

	// Token: 0x040009A2 RID: 2466
	public float surfacespeed;

	// Token: 0x040009A3 RID: 2467
	public float submergedspeed;

	// Token: 0x040009A4 RID: 2468
	public float[] telegraphSpeeds;

	// Token: 0x040009A5 RID: 2469
	public Vector2 cavitationparameters;

	// Token: 0x040009A6 RID: 2470
	public float[] proprotationspeed;

	// Token: 0x040009A7 RID: 2471
	public float testDepth;

	// Token: 0x040009A8 RID: 2472
	public float actualTestDepth;

	// Token: 0x040009A9 RID: 2473
	public float escapeDepth;

	// Token: 0x040009AA RID: 2474
	public float selfnoise;

	// Token: 0x040009AB RID: 2475
	public float activesonarreflection;

	// Token: 0x040009AC RID: 2476
	public int activeSonarID;

	// Token: 0x040009AD RID: 2477
	public int passiveSonarID;

	// Token: 0x040009AE RID: 2478
	public int towedSonarID;

	// Token: 0x040009AF RID: 2479
	public float activeArrayBonus;

	// Token: 0x040009B0 RID: 2480
	public float passiveArrayBonus;

	// Token: 0x040009B1 RID: 2481
	public bool anechoicCoating;

	// Token: 0x040009B2 RID: 2482
	public int radarID;

	// Token: 0x040009B3 RID: 2483
	public string radarSignature;

	// Token: 0x040009B4 RID: 2484
	public Vector2 targetPoint;

	// Token: 0x040009B5 RID: 2485
	public Vector3 towedArrayPosition;

	// Token: 0x040009B6 RID: 2486
	public string[] subsystemPrimaryPositions;

	// Token: 0x040009B7 RID: 2487
	public string[] subsystemSecondaryPositions;

	// Token: 0x040009B8 RID: 2488
	public string[] subsystemTertiaryPositions;

	// Token: 0x040009B9 RID: 2489
	public Vector2[] compartmentPositionsAndWidth;

	// Token: 0x040009BA RID: 2490
	public Vector2[] compartmentFloodingRanges;

	// Token: 0x040009BB RID: 2491
	public float damageControlPartyY;

	// Token: 0x040009BC RID: 2492
	public Vector2[] subsystemLabelPositions;

	// Token: 0x040009BD RID: 2493
	public string[] hullnumbers;

	// Token: 0x040009BE RID: 2494
	public float minCameraDistance;

	// Token: 0x040009BF RID: 2495
	public string description;

	// Token: 0x040009C0 RID: 2496
	public string[] defensiveWeapons;

	// Token: 0x040009C1 RID: 2497
	public string[] offensiveWeapons;

	// Token: 0x040009C2 RID: 2498
	public string[] sensors;

	// Token: 0x040009C3 RID: 2499
	public string[] history;

	// Token: 0x040009C4 RID: 2500
	public string[] aircraftOnBoard;

	// Token: 0x040009C5 RID: 2501
	public string[] playerClassNames;

	// Token: 0x040009C6 RID: 2502
	public string[] playerClassHullNumbers;
}
