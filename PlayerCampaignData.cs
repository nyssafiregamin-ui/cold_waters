using System;
using UnityEngine;

// Token: 0x020000F8 RID: 248
public class PlayerCampaignData : ScriptableObject
{
	// Token: 0x040007E0 RID: 2016
	public bool playerHasMission;

	// Token: 0x040007E1 RID: 2017
	public string playerMissionType;

	// Token: 0x040007E2 RID: 2018
	public int currentMissionTaskForceID;

	// Token: 0x040007E3 RID: 2019
	public float totalTonnage;

	// Token: 0x040007E4 RID: 2020
	public float patrolTonnage;

	// Token: 0x040007E5 RID: 2021
	public float patrolPoints;

	// Token: 0x040007E6 RID: 2022
	public int missionsPassed;

	// Token: 0x040007E7 RID: 2023
	public float[] campaignStats = new float[12];

	// Token: 0x040007E8 RID: 2024
	public int[] patrolMedals;

	// Token: 0x040007E9 RID: 2025
	public int[] cumulativeMedals;

	// Token: 0x040007EA RID: 2026
	public int[] woundedMedals;

	// Token: 0x040007EB RID: 2027
	public int[] playerVesselsLost;

	// Token: 0x040007EC RID: 2028
	public int[] playerInstancesLost;

	// Token: 0x040007ED RID: 2029
	public Vector2 playerPostionOnMap;

	// Token: 0x040007EE RID: 2030
	public float secondsCampaignPlayed;

	// Token: 0x040007EF RID: 2031
	public float playerRevealed;

	// Token: 0x040007F0 RID: 2032
	public int[] playerTorpeodesOnBoard;

	// Token: 0x040007F1 RID: 2033
	public int[] playerVLSTorpeodesOnBoard;

	// Token: 0x040007F2 RID: 2034
	public int[] playerTubeStatus;

	// Token: 0x040007F3 RID: 2035
	public int[] playerWeaponInTube;

	// Token: 0x040007F4 RID: 2036
	public int[] playerSettingsOne;

	// Token: 0x040007F5 RID: 2037
	public int[] playerSettingsTwo;

	// Token: 0x040007F6 RID: 2038
	public int[] playerSettingsThree;

	// Token: 0x040007F7 RID: 2039
	public int playerNoisemakersOnBoard;

	// Token: 0x040007F8 RID: 2040
	public bool playerSealTeamOnBoard;

	// Token: 0x040007F9 RID: 2041
	public float vesselTotalDamage;

	// Token: 0x040007FA RID: 2042
	public float[] compartmentCurrentFlooding = new float[5];

	// Token: 0x040007FB RID: 2043
	public float[] compartmentTotalFlooding = new float[5];

	// Token: 0x040007FC RID: 2044
	public int[] decalNames = new int[10];

	// Token: 0x040007FD RID: 2045
	public bool[] destroyedSubsystems;

	// Token: 0x040007FE RID: 2046
	public float[] neutralCasualties = new float[3];
}
