using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000131 RID: 305
public class LevelLoadData : MonoBehaviour
{
	// Token: 0x06000839 RID: 2105 RVA: 0x000534C4 File Offset: 0x000516C4
	public void GetHelicopterNumbers()
	{
		if (UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.usePresetPositions)
		{
			return;
		}
		int[] array = new int[UIFunctions.globaluifunctions.database.databaseaircraftdata.Length];
		for (int i = 0; i < UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.kmShipClasses.Length; i++)
		{
			for (int j = 0; j < UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.kmShipClasses[i]].aircraftIDs.Length; j++)
			{
				array[UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.kmShipClasses[i]].aircraftIDs[j]] = UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.kmShipClasses[i]].aircraftNumbers[j];
			}
		}
		int num = 0;
		int num2 = 0;
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.numberOfHelicopters = 0;
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.helicopterType = string.Empty;
		for (int k = 0; k < array.Length; k++)
		{
			if (array[k] > num)
			{
				num = array[k];
				num2 = k;
			}
		}
		if (num > 0)
		{
			if (num > 4)
			{
				num = 4;
			}
			num = UnityEngine.Random.Range(1, num + 1);
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.numberOfHelicopters = num;
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.helicopterType = UIFunctions.globaluifunctions.database.databaseaircraftdata[num2].aircraftPrefabName;
		}
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.aircraftType = string.Empty;
		List<string> list = new List<string>();
		for (int l = 0; l < UIFunctions.globaluifunctions.campaignmanager.campaignaircraft.Length; l++)
		{
			if (UIFunctions.globaluifunctions.campaignmanager.campaignaircraft[l].faction == "ENEMY")
			{
				list.Add(UIFunctions.globaluifunctions.campaignmanager.campaignaircraft[l].reconName);
			}
		}
		if (list.Count > 0)
		{
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.aircraftType = list[UnityEngine.Random.Range(0, list.Count)];
		}
		else
		{
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.numberOfAircraft = 0;
		}
	}

	// Token: 0x0600083A RID: 2106 RVA: 0x00053754 File Offset: 0x00051954
	public void AssignNeutralVessels()
	{
		this.wanderingNeutralsStartIndex = this.kmShipClasses.Length;
		this.neutralFlagsToSpawn = new List<Material>();
		this.neutralIDsToSpawn = this.GetWanderingNeutralsNumber();
		if (this.neutralIDsToSpawn.Count > 0)
		{
			int[] array = new int[this.kmShipClasses.Length + this.neutralIDsToSpawn.Count];
			for (int i = 0; i < array.Length; i++)
			{
				if (i < this.kmShipClasses.Length)
				{
					array[i] = this.kmShipClasses[i];
				}
				else
				{
					array[i] = this.neutralIDsToSpawn[i - this.kmShipClasses.Length];
				}
			}
			this.kmShipClasses = array;
			GameDataManager.enemyNumberofShips = this.kmShipClasses.Length;
		}
	}

	// Token: 0x0600083B RID: 2107 RVA: 0x00053810 File Offset: 0x00051A10
	private List<int> GetWanderingNeutralsNumber()
	{
		List<int> list = new List<int>();
		List<Material> list2 = new List<Material>();
		if (this.usePresetPositions || this.noOtherVessels)
		{
			return list;
		}
		if (UIFunctions.globaluifunctions.levelloadmanager.icePresent || UIFunctions.globaluifunctions.levelloadmanager.packIcePresent)
		{
			return list;
		}
		if (UIFunctions.globaluifunctions.campaignmanager.mapMerchantTraffic == null && UIFunctions.globaluifunctions.campaignmanager.mapFishingTraffic == null)
		{
			return list;
		}
		int[] array = new int[]
		{
			Mathf.FloorToInt(this.mapPosition.x / 16f),
			Mathf.FloorToInt(this.mapPosition.y / 16f)
		};
		if (UnityEngine.Random.value < 0.5f)
		{
			list = this.AppendNeutralsList(list, UIFunctions.globaluifunctions.campaignmanager.mapFishingTraffic.GetPixel(Mathf.FloorToInt((float)array[0]), Mathf.FloorToInt((float)array[1])).r, this.neutralFishingIDs1, this.neutralFishingFlags1);
			list = this.AppendNeutralsList(list, UIFunctions.globaluifunctions.campaignmanager.mapFishingTraffic.GetPixel(Mathf.FloorToInt((float)array[0]), Mathf.FloorToInt((float)array[1])).g, this.neutralFishingIDs2, this.neutralFishingFlags2);
			list = this.AppendNeutralsList(list, UIFunctions.globaluifunctions.campaignmanager.mapFishingTraffic.GetPixel(Mathf.FloorToInt((float)array[0]), Mathf.FloorToInt((float)array[1])).b, this.neutralFishingIDs3, this.neutralFishingFlags3);
			list = this.AppendNeutralsList(list, UIFunctions.globaluifunctions.campaignmanager.mapFishingTraffic.GetPixel(Mathf.FloorToInt((float)array[0]), Mathf.FloorToInt((float)array[1])).a, this.neutralFishingIDs4, this.neutralFishingFlags4);
			list = this.AppendNeutralsList(list, UIFunctions.globaluifunctions.campaignmanager.mapMerchantTraffic.GetPixel(Mathf.FloorToInt((float)array[0]), Mathf.FloorToInt((float)array[1])).r, this.neutralMerchantIDs1, this.neutralMerchantFlags1);
			list = this.AppendNeutralsList(list, UIFunctions.globaluifunctions.campaignmanager.mapMerchantTraffic.GetPixel(Mathf.FloorToInt((float)array[0]), Mathf.FloorToInt((float)array[1])).g, this.neutralMerchantIDs2, this.neutralMerchantFlags2);
			list = this.AppendNeutralsList(list, UIFunctions.globaluifunctions.campaignmanager.mapMerchantTraffic.GetPixel(Mathf.FloorToInt((float)array[0]), Mathf.FloorToInt((float)array[1])).b, this.neutralMerchantIDs3, this.neutralMerchantFlags3);
			list = this.AppendNeutralsList(list, UIFunctions.globaluifunctions.campaignmanager.mapMerchantTraffic.GetPixel(Mathf.FloorToInt((float)array[0]), Mathf.FloorToInt((float)array[1])).a, this.neutralMerchantIDs4, this.neutralMerchantFlags4);
		}
		else
		{
			list = this.AppendNeutralsList(list, UIFunctions.globaluifunctions.campaignmanager.mapMerchantTraffic.GetPixel(Mathf.FloorToInt((float)array[0]), Mathf.FloorToInt((float)array[1])).r, this.neutralMerchantIDs1, this.neutralMerchantFlags1);
			list = this.AppendNeutralsList(list, UIFunctions.globaluifunctions.campaignmanager.mapMerchantTraffic.GetPixel(Mathf.FloorToInt((float)array[0]), Mathf.FloorToInt((float)array[1])).g, this.neutralMerchantIDs2, this.neutralMerchantFlags2);
			list = this.AppendNeutralsList(list, UIFunctions.globaluifunctions.campaignmanager.mapMerchantTraffic.GetPixel(Mathf.FloorToInt((float)array[0]), Mathf.FloorToInt((float)array[1])).b, this.neutralMerchantIDs3, this.neutralMerchantFlags3);
			list = this.AppendNeutralsList(list, UIFunctions.globaluifunctions.campaignmanager.mapMerchantTraffic.GetPixel(Mathf.FloorToInt((float)array[0]), Mathf.FloorToInt((float)array[1])).a, this.neutralMerchantIDs4, this.neutralMerchantFlags4);
			list = this.AppendNeutralsList(list, UIFunctions.globaluifunctions.campaignmanager.mapFishingTraffic.GetPixel(Mathf.FloorToInt((float)array[0]), Mathf.FloorToInt((float)array[1])).r, this.neutralFishingIDs1, this.neutralFishingFlags1);
			list = this.AppendNeutralsList(list, UIFunctions.globaluifunctions.campaignmanager.mapFishingTraffic.GetPixel(Mathf.FloorToInt((float)array[0]), Mathf.FloorToInt((float)array[1])).g, this.neutralFishingIDs2, this.neutralFishingFlags2);
			list = this.AppendNeutralsList(list, UIFunctions.globaluifunctions.campaignmanager.mapFishingTraffic.GetPixel(Mathf.FloorToInt((float)array[0]), Mathf.FloorToInt((float)array[1])).b, this.neutralFishingIDs3, this.neutralFishingFlags3);
			list = this.AppendNeutralsList(list, UIFunctions.globaluifunctions.campaignmanager.mapFishingTraffic.GetPixel(Mathf.FloorToInt((float)array[0]), Mathf.FloorToInt((float)array[1])).a, this.neutralFishingIDs4, this.neutralFishingFlags4);
		}
		return list;
	}

	// Token: 0x0600083C RID: 2108 RVA: 0x00053D20 File Offset: 0x00051F20
	private List<int> AppendNeutralsList(List<int> currentNeutralsList, float threshold, List<int> vesselIDs, List<string> flagMaterialPaths)
	{
		int num = 4;
		if (threshold == 0f)
		{
			return currentNeutralsList;
		}
		if (currentNeutralsList.Count > 5)
		{
			return currentNeutralsList;
		}
		if (this.kmShipClasses.Length + currentNeutralsList.Count > 14)
		{
			return currentNeutralsList;
		}
		if (UnityEngine.Random.value * this.neutralProb1 < threshold)
		{
			currentNeutralsList.Add(vesselIDs[UnityEngine.Random.Range(0, vesselIDs.Count)]);
			this.neutralFlagsToSpawn.Add(Resources.Load(flagMaterialPaths[UnityEngine.Random.Range(0, flagMaterialPaths.Count)]) as Material);
			if (currentNeutralsList.Count > num)
			{
				return currentNeutralsList;
			}
		}
		if (UnityEngine.Random.value * this.neutralProb2 < threshold)
		{
			currentNeutralsList.Add(vesselIDs[UnityEngine.Random.Range(0, vesselIDs.Count)]);
			this.neutralFlagsToSpawn.Add(Resources.Load(flagMaterialPaths[UnityEngine.Random.Range(0, flagMaterialPaths.Count)]) as Material);
		}
		return currentNeutralsList;
	}

	// Token: 0x0600083D RID: 2109 RVA: 0x00053E18 File Offset: 0x00052018
	public void AssignWhales()
	{
		if (this.noOtherVessels)
		{
			return;
		}
		if (UnityEngine.Random.value < this.whaleProbability)
		{
			int num = UnityEngine.Random.Range(1, 3);
			if (this.kmShipClasses.Length + num < 16)
			{
				List<int> list = new List<int>();
				for (int i = 0; i < UIFunctions.globaluifunctions.database.databaseshipdata.Length; i++)
				{
					if (UIFunctions.globaluifunctions.database.databaseshipdata[i].shipType == "BIOLOGIC")
					{
						list.Add(i);
					}
				}
				int[] array = new int[this.kmShipClasses.Length + num];
				for (int j = 0; j < array.Length; j++)
				{
					if (j < this.kmShipClasses.Length)
					{
						array[j] = this.kmShipClasses[j];
					}
					else
					{
						array[j] = list[UnityEngine.Random.Range(0, list.Count)];
					}
				}
				this.kmShipClasses = array;
			}
		}
	}

	// Token: 0x04000C0D RID: 3085
	public string missionName;

	// Token: 0x04000C0E RID: 3086
	public string missionType;

	// Token: 0x04000C0F RID: 3087
	public int missionNumber;

	// Token: 0x04000C10 RID: 3088
	public int missionID;

	// Token: 0x04000C11 RID: 3089
	public Vector2[] numberOfEnemyUnits;

	// Token: 0x04000C12 RID: 3090
	public bool[] enemyUnitMissionCritical;

	// Token: 0x04000C13 RID: 3091
	public string[] enemyShipClasses;

	// Token: 0x04000C14 RID: 3092
	public string[] enemyShipBehaviour;

	// Token: 0x04000C15 RID: 3093
	public int[] rnShipClasses;

	// Token: 0x04000C16 RID: 3094
	public int[] rnShipInstances;

	// Token: 0x04000C17 RID: 3095
	public int[] kmShipClasses;

	// Token: 0x04000C18 RID: 3096
	public int[] kmShipInstances;

	// Token: 0x04000C19 RID: 3097
	public int[] rnSlots;

	// Token: 0x04000C1A RID: 3098
	public int[] kmSlots;

	// Token: 0x04000C1B RID: 3099
	public float distance;

	// Token: 0x04000C1C RID: 3100
	public float globalSpawnRotation;

	// Token: 0x04000C1D RID: 3101
	public Vector2 localSpawnRotation;

	// Token: 0x04000C1E RID: 3102
	public bool submarinesOnly;

	// Token: 0x04000C1F RID: 3103
	public bool useTerrain;

	// Token: 0x04000C20 RID: 3104
	public Vector2 mapPosition;

	// Token: 0x04000C21 RID: 3105
	public string mapFromCampaign;

	// Token: 0x04000C22 RID: 3106
	public string mapElevationData;

	// Token: 0x04000C23 RID: 3107
	public string mapNavigationData;

	// Token: 0x04000C24 RID: 3108
	public string worldObjectsData;

	// Token: 0x04000C25 RID: 3109
	public bool noOtherVessels;

	// Token: 0x04000C26 RID: 3110
	public string date;

	// Token: 0x04000C27 RID: 3111
	public string month;

	// Token: 0x04000C28 RID: 3112
	public string season;

	// Token: 0x04000C29 RID: 3113
	public string hemisphere;

	// Token: 0x04000C2A RID: 3114
	public int numberOfHelicopters;

	// Token: 0x04000C2B RID: 3115
	public string helicopterType;

	// Token: 0x04000C2C RID: 3116
	public Vector2 helicopterOffsets;

	// Token: 0x04000C2D RID: 3117
	public Vector3[] aircraftSearchAreas;

	// Token: 0x04000C2E RID: 3118
	public Vector3[] sonobuoyLocations;

	// Token: 0x04000C2F RID: 3119
	public float[] sonobuoyRanges;

	// Token: 0x04000C30 RID: 3120
	public Vector2[] proximityMineLocations;

	// Token: 0x04000C31 RID: 3121
	public Vector4[] proximityMineParameters;

	// Token: 0x04000C32 RID: 3122
	public float[] proximityMineAngles;

	// Token: 0x04000C33 RID: 3123
	public float[] proximityMineScatter;

	// Token: 0x04000C34 RID: 3124
	public int numberOfAircraft;

	// Token: 0x04000C35 RID: 3125
	public string aircraftType;

	// Token: 0x04000C36 RID: 3126
	public bool usePresetEnvironment;

	// Token: 0x04000C37 RID: 3127
	public int timeofday;

	// Token: 0x04000C38 RID: 3128
	public string timeOfDayString;

	// Token: 0x04000C39 RID: 3129
	public int environment;

	// Token: 0x04000C3A RID: 3130
	public int seaState;

	// Token: 0x04000C3B RID: 3131
	public float ductStrength;

	// Token: 0x04000C3C RID: 3132
	public float layerStrength;

	// Token: 0x04000C3D RID: 3133
	public bool usePresetPositions;

	// Token: 0x04000C3E RID: 3134
	public Vector2 rnShipPosition;

	// Token: 0x04000C3F RID: 3135
	public float rnShipHeading;

	// Token: 0x04000C40 RID: 3136
	public float rnShipDepth;

	// Token: 0x04000C41 RID: 3137
	public int rnShipTelegraph;

	// Token: 0x04000C42 RID: 3138
	public float[] enemyShipPositionsX;

	// Token: 0x04000C43 RID: 3139
	public float[] enemyShipPositionsZ;

	// Token: 0x04000C44 RID: 3140
	public float[] enemyshipHeadings;

	// Token: 0x04000C45 RID: 3141
	public Vector3[] finalEnemyPositionsAndHeadings;

	// Token: 0x04000C46 RID: 3142
	public string[] enemyWaypoints;

	// Token: 0x04000C47 RID: 3143
	public string briefing;

	// Token: 0x04000C48 RID: 3144
	public float formationCruiseSpeed;

	// Token: 0x04000C49 RID: 3145
	public GameObject formationPrefab;

	// Token: 0x04000C4A RID: 3146
	public GameObject formationGrid;

	// Token: 0x04000C4B RID: 3147
	public GameObject[] formationPositions;

	// Token: 0x04000C4C RID: 3148
	public float waterDepth;

	// Token: 0x04000C4D RID: 3149
	public string combatLocation;

	// Token: 0x04000C4E RID: 3150
	public Vector2 missionPosition;

	// Token: 0x04000C4F RID: 3151
	public BoxCollider[] mineFields;

	// Token: 0x04000C50 RID: 3152
	public int numberOfLandHits;

	// Token: 0x04000C51 RID: 3153
	public Transform[] missileHitLocations;

	// Token: 0x04000C52 RID: 3154
	public Transform[] shipDockLocations;

	// Token: 0x04000C53 RID: 3155
	public Color[] missionMarkerColors;

	// Token: 0x04000C54 RID: 3156
	public int missionMarkerCurrentColor;

	// Token: 0x04000C55 RID: 3157
	public int wanderingNeutralsStartIndex;

	// Token: 0x04000C56 RID: 3158
	public List<int> neutralMerchantIDs1;

	// Token: 0x04000C57 RID: 3159
	public List<int> neutralMerchantIDs2;

	// Token: 0x04000C58 RID: 3160
	public List<int> neutralMerchantIDs3;

	// Token: 0x04000C59 RID: 3161
	public List<int> neutralMerchantIDs4;

	// Token: 0x04000C5A RID: 3162
	public List<string> neutralMerchantFlags1;

	// Token: 0x04000C5B RID: 3163
	public List<string> neutralMerchantFlags2;

	// Token: 0x04000C5C RID: 3164
	public List<string> neutralMerchantFlags3;

	// Token: 0x04000C5D RID: 3165
	public List<string> neutralMerchantFlags4;

	// Token: 0x04000C5E RID: 3166
	public List<int> neutralFishingIDs1;

	// Token: 0x04000C5F RID: 3167
	public List<int> neutralFishingIDs2;

	// Token: 0x04000C60 RID: 3168
	public List<int> neutralFishingIDs3;

	// Token: 0x04000C61 RID: 3169
	public List<int> neutralFishingIDs4;

	// Token: 0x04000C62 RID: 3170
	public List<string> neutralFishingFlags1;

	// Token: 0x04000C63 RID: 3171
	public List<string> neutralFishingFlags2;

	// Token: 0x04000C64 RID: 3172
	public List<string> neutralFishingFlags3;

	// Token: 0x04000C65 RID: 3173
	public List<string> neutralFishingFlags4;

	// Token: 0x04000C66 RID: 3174
	public List<int> neutralIDsToSpawn;

	// Token: 0x04000C67 RID: 3175
	public List<Material> neutralFlagsToSpawn;

	// Token: 0x04000C68 RID: 3176
	public float whaleProbability;

	// Token: 0x04000C69 RID: 3177
	public float neutralProb1;

	// Token: 0x04000C6A RID: 3178
	public float neutralProb2;
}
