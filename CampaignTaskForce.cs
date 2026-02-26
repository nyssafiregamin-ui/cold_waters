using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000F2 RID: 242
public class CampaignTaskForce : ScriptableObject
{
	// Token: 0x04000764 RID: 1892
	public int taskForceID;

	// Token: 0x04000765 RID: 1893
	public int missionType;

	// Token: 0x04000766 RID: 1894
	public int actualMission;

	// Token: 0x04000767 RID: 1895
	public int missionID;

	// Token: 0x04000768 RID: 1896
	public float speed;

	// Token: 0x04000769 RID: 1897
	public int previousWaypoint;

	// Token: 0x0400076A RID: 1898
	public int currentWaypoint;

	// Token: 0x0400076B RID: 1899
	public int legOfRoute;

	// Token: 0x0400076C RID: 1900
	public int[] mustUseWaypoints;

	// Token: 0x0400076D RID: 1901
	public int[] prohibitedWaypoints;

	// Token: 0x0400076E RID: 1902
	public List<int> possibleWaypoints;

	// Token: 0x0400076F RID: 1903
	public Vector3 waypointPosition;

	// Token: 0x04000770 RID: 1904
	public Vector2 finalLocationPosition;

	// Token: 0x04000771 RID: 1905
	public bool takeDirectRoute;

	// Token: 0x04000772 RID: 1906
	public bool startIsLocation;

	// Token: 0x04000773 RID: 1907
	public bool endIsLocation;

	// Token: 0x04000774 RID: 1908
	public bool finalLegDone;

	// Token: 0x04000775 RID: 1909
	public int startPositionID;

	// Token: 0x04000776 RID: 1910
	public int endLocationID;

	// Token: 0x04000777 RID: 1911
	public Path_AStar pathAStar;

	// Token: 0x04000778 RID: 1912
	public string currTile;

	// Token: 0x04000779 RID: 1913
	public string destTile;

	// Token: 0x0400077A RID: 1914
	public string nextTile;

	// Token: 0x0400077B RID: 1915
	public int[] shipNumbers;

	// Token: 0x0400077C RID: 1916
	public string[] shipClasses;

	// Token: 0x0400077D RID: 1917
	public int currentAction;

	// Token: 0x0400077E RID: 1918
	public string behaviourType;

	// Token: 0x0400077F RID: 1919
	public int taskForceType;

	// Token: 0x04000780 RID: 1920
	public float patrolForHours;

	// Token: 0x04000781 RID: 1921
	public bool[] missionCriticalVessels;

	// Token: 0x04000782 RID: 1922
	public bool[] defensiveVessels;
}
