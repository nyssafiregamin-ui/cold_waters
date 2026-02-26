using System;
using UnityEngine;

// Token: 0x02000103 RID: 259
public class Database : MonoBehaviour
{
	// Token: 0x06000723 RID: 1827 RVA: 0x0003FA08 File Offset: 0x0003DC08
	public int GetMenuSystemPageIndex(string index)
	{
		int result = 0;
		switch (index)
		{
		case "MAINMENU":
			result = 0;
			break;
		case "SELECTION":
			result = 1;
			break;
		case "ACTIONREPORT":
			result = 2;
			break;
		case "STATUSSCREENS":
			result = 3;
			break;
		case "MUSEUM":
			result = 4;
			break;
		case "BRIEFING":
			result = 5;
			break;
		case "CAMPAIGN":
			result = 6;
			break;
		case "CAMPAIGNSELECTION":
			result = 7;
			break;
		case "EVENT":
			result = 8;
			break;
		case "COMBATEXIT":
			result = 9;
			break;
		case "CAMPAIGNEXIT":
			result = 10;
			break;
		case "OPTIONS":
			result = 11;
			break;
		case "CREDITS":
			result = 12;
			break;
		}
		return result;
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x0003FB88 File Offset: 0x0003DD88
	public static bool GetIsCivilian(string prefabName)
	{
		string[] array = prefabName.Split(new char[]
		{
			'_'
		});
		return array[0] == "civ" || prefabName.Contains("trawler");
	}

	// Token: 0x06000725 RID: 1829 RVA: 0x0003FBCC File Offset: 0x0003DDCC
	public string GetCompartment(string compartmentPosition)
	{
		string result = string.Empty;
		switch (compartmentPosition)
		{
		case "FRONT":
			result = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "TorpedoRoom");
			break;
		case "FORE":
			result = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ControlRoom");
			break;
		case "MID":
			result = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReactorSpace");
			break;
		case "AFT":
			result = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "EngineRoom");
			break;
		case "REAR":
			result = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "MachinerySpace");
			break;
		}
		return result;
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x0003FCD8 File Offset: 0x0003DED8
	public void PlaceSinkingBubbles(Vessel activeVessel)
	{
		if (activeVessel.damagesystem.sinkingBubbles == null)
		{
			ParticleSystem particleSystem = this.sinkingbubbles[0];
			if (activeVessel.databaseshipdata.shipType != "SUBMARINE")
			{
				particleSystem = this.sinkingbubbles[1];
			}
			particleSystem = (UnityEngine.Object.Instantiate(particleSystem, activeVessel.transform.position, Quaternion.identity) as ParticleSystem);
			particleSystem.transform.SetParent(activeVessel.transform, true);
			particleSystem.transform.localPosition = new Vector3(0f, activeVessel.databaseshipdata.hullHeight / 2f, 0f);
			particleSystem.transform.localRotation = Quaternion.identity;
			particleSystem.Play();
			activeVessel.damagesystem.sinkingBubbles = particleSystem;
			if (activeVessel.damagesystem.screechSound == null)
			{
				this.PlaceScreechAudioSource(activeVessel);
			}
			particleSystem.transform.localScale = new Vector3(activeVessel.bouyancyCompartments[0].transform.lossyScale.x, 0f, activeVessel.bouyancyCompartments[0].transform.lossyScale.z * 4f);
		}
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x0003FE10 File Offset: 0x0003E010
	public void PlaceScreechAudioSource(Vessel activeVessel)
	{
		if (activeVessel.damagesystem.screechSound == null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.screechSound, activeVessel.transform.position, Quaternion.identity) as GameObject;
			gameObject.transform.SetParent(activeVessel.transform, true);
			AudioSource component = gameObject.GetComponent<AudioSource>();
			activeVessel.damagesystem.screechSound = component;
		}
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x0003FE84 File Offset: 0x0003E084
	public void CreateImpactEffect(float calibre, Vector3 impactPosition)
	{
		if (calibre > 18f)
		{
			ObjectPoolManager.CreatePooled(this.magazineExplosions[1], new Vector3(impactPosition.x, 1000f, impactPosition.z), Quaternion.Euler(0f, GameDataManager.smokeAngle, 0f));
		}
		else if (calibre > 12f)
		{
			ObjectPoolManager.CreatePooled(this.impactEffects[3], impactPosition, Quaternion.identity);
		}
		else if (calibre > 7f)
		{
			ObjectPoolManager.CreatePooled(this.impactEffects[2], impactPosition, Quaternion.identity);
		}
		else if (calibre > 5f)
		{
			ObjectPoolManager.CreatePooled(this.impactEffects[1], impactPosition, Quaternion.identity);
		}
		else
		{
			ObjectPoolManager.CreatePooled(this.impactEffects[0], impactPosition, Quaternion.identity);
		}
	}

	// Token: 0x06000729 RID: 1833 RVA: 0x0003FF5C File Offset: 0x0003E15C
	public void CreateSecondaryEffect(Vector3 explosionPosition, int explosionIndex)
	{
		ObjectPoolManager.CreatePooled(this.secondaryExplosions[explosionIndex], new Vector3(explosionPosition.x, 1000f, explosionPosition.z), Quaternion.Euler(0f, GameDataManager.smokeAngle, 0f));
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x0003FFA4 File Offset: 0x0003E1A4
	public void CreateMagazineExplosion(Vector3 explosionPosition)
	{
		ObjectPoolManager.CreatePooled(this.magazineExplosions[UnityEngine.Random.Range(0, 3)], new Vector3(explosionPosition.x, 1000f, explosionPosition.z), Quaternion.Euler(0f, GameDataManager.smokeAngle, 0f));
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x0003FFF4 File Offset: 0x0003E1F4
	public void CreateExplosion(Vessel activeVessel, int magnitude, Vector3 explosionPosition)
	{
		int max = UIFunctions.globaluifunctions.database.secondaryExplosions.Length - 1;
		if (activeVessel.damagesystem.shipTotalDamagePoints < 100f)
		{
			max = 2;
		}
		magnitude = Mathf.Clamp(magnitude, 0, max);
		UIFunctions.globaluifunctions.database.CreateSecondaryEffect(explosionPosition, magnitude);
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x00040048 File Offset: 0x0003E248
	public void CreateFloorDust(Vector3 dustPosition)
	{
		GameObject gameObject = ObjectPoolManager.CreatePooled(this.floorDust, dustPosition, Quaternion.identity);
		AudioSource component = gameObject.GetComponent<AudioSource>();
		component.clip = this.screeches[UnityEngine.Random.Range(0, this.screeches.Length)];
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x0004008C File Offset: 0x0003E28C
	public Texture GetWakeOverlayTexture()
	{
		return this.wakeTextures[0];
	}

	// Token: 0x0400089A RID: 2202
	public SaveLoadManager saveloadmanager;

	// Token: 0x0400089B RID: 2203
	public SortArray sortarray;

	// Token: 0x0400089C RID: 2204
	public DatabaseShipData[] databaseshipdata;

	// Token: 0x0400089D RID: 2205
	public DatabaseWeaponData[] databaseweapondata;

	// Token: 0x0400089E RID: 2206
	public DatabaseDepthChargeData[] databasedepthchargedata;

	// Token: 0x0400089F RID: 2207
	public DatabaseSonarData[] databasesonardata;

	// Token: 0x040008A0 RID: 2208
	public DatabaseAircraftData[] databaseaircraftdata;

	// Token: 0x040008A1 RID: 2209
	public DatabaseCountermeasureData[] databasecountermeasuredata;

	// Token: 0x040008A2 RID: 2210
	public DatabaseSubsystemsData[] databasesubsystemsdata;

	// Token: 0x040008A3 RID: 2211
	public DatabaseRADARData[] databaseradardata;

	// Token: 0x040008A4 RID: 2212
	public int aerialSonobuoyID;

	// Token: 0x040008A5 RID: 2213
	public GameObject sonobuoyInWaterObject;

	// Token: 0x040008A6 RID: 2214
	public DatabaseShipData[] kmDatabaseShipData;

	// Token: 0x040008A7 RID: 2215
	public DatabaseShipData[] rnDatabaseShipData;

	// Token: 0x040008A8 RID: 2216
	public int[] kmRenderOrder;

	// Token: 0x040008A9 RID: 2217
	public int[] rnRenderOrder;

	// Token: 0x040008AA RID: 2218
	public GameObject engineSound;

	// Token: 0x040008AB RID: 2219
	public GameObject propSound;

	// Token: 0x040008AC RID: 2220
	public GameObject screechSound;

	// Token: 0x040008AD RID: 2221
	public AudioClip[] engineClips;

	// Token: 0x040008AE RID: 2222
	public GameObject navRadius;

	// Token: 0x040008AF RID: 2223
	public GameObject noiseRadius;

	// Token: 0x040008B0 RID: 2224
	public GameObject blankTransform;

	// Token: 0x040008B1 RID: 2225
	public GameObject submarineFoam;

	// Token: 0x040008B2 RID: 2226
	public Material[] genericShipMaterials;

	// Token: 0x040008B3 RID: 2227
	public Material[] damageDecals;

	// Token: 0x040008B4 RID: 2228
	public Material[] destroyedDecals;

	// Token: 0x040008B5 RID: 2229
	public GameObject[] impactEffects;

	// Token: 0x040008B6 RID: 2230
	public GameObject[] secondaryExplosions;

	// Token: 0x040008B7 RID: 2231
	public GameObject[] magazineExplosions;

	// Token: 0x040008B8 RID: 2232
	public GameObject[] magazineExplosionsLand;

	// Token: 0x040008B9 RID: 2233
	public GameObject[] oilSlicks;

	// Token: 0x040008BA RID: 2234
	public GameObject[] muzzleFlashes;

	// Token: 0x040008BB RID: 2235
	public GameObject[] muzzleSmokes;

	// Token: 0x040008BC RID: 2236
	public GameObject[] underwaterTorpedoLaunch;

	// Token: 0x040008BD RID: 2237
	public GameObject[] torpedoLaunch;

	// Token: 0x040008BE RID: 2238
	public GameObject[] missileLaunch;

	// Token: 0x040008BF RID: 2239
	public GameObject[] underwaterExplosions;

	// Token: 0x040008C0 RID: 2240
	public GameObject[] underwaterLargeExplosions;

	// Token: 0x040008C1 RID: 2241
	public GameObject[] underwaterImplosions;

	// Token: 0x040008C2 RID: 2242
	public GameObject[] surfacePlumes;

	// Token: 0x040008C3 RID: 2243
	public GameObject rbuLaunchFlare;

	// Token: 0x040008C4 RID: 2244
	public ParticleSystem[] shipFires;

	// Token: 0x040008C5 RID: 2245
	public ParticleSystem[] sinkingbubbles;

	// Token: 0x040008C6 RID: 2246
	public GameObject[] missileShotDown;

	// Token: 0x040008C7 RID: 2247
	public GameObject[] missileImpacts;

	// Token: 0x040008C8 RID: 2248
	public GameObject floorDust;

	// Token: 0x040008C9 RID: 2249
	public string[] playerNonWeaponSunkTypes;

	// Token: 0x040008CA RID: 2250
	public AudioClip[] whaleSongs;

	// Token: 0x040008CB RID: 2251
	public GameObject shootDownEffect;

	// Token: 0x040008CC RID: 2252
	public Material bowwaveMaterial;

	// Token: 0x040008CD RID: 2253
	public Material fireSmokeMaterial;

	// Token: 0x040008CE RID: 2254
	public Material funnelSmokeMaterial;

	// Token: 0x040008CF RID: 2255
	public Material whiteSmokeMaterial;

	// Token: 0x040008D0 RID: 2256
	public Texture[] wakeTextures;

	// Token: 0x040008D1 RID: 2257
	public GameObject[] splashes;

	// Token: 0x040008D2 RID: 2258
	public AudioClip[] screeches;
}
