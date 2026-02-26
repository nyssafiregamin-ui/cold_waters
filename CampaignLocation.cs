using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000F4 RID: 244
public class CampaignLocation : ScriptableObject
{
	// Token: 0x0400078A RID: 1930
	public int locationID;

	// Token: 0x0400078B RID: 1931
	public string locationName;

	// Token: 0x0400078C RID: 1932
	public string country;

	// Token: 0x0400078D RID: 1933
	public string faction;

	// Token: 0x0400078E RID: 1934
	public string originalFaction;

	// Token: 0x0400078F RID: 1935
	public List<string> function;

	// Token: 0x04000790 RID: 1936
	public Vector2 baseLocation;

	// Token: 0x04000791 RID: 1937
	public int aircraftType;

	// Token: 0x04000792 RID: 1938
	public int aircraftTypeInvaded;

	// Token: 0x04000793 RID: 1939
	public float aircraftFrequency;

	// Token: 0x04000794 RID: 1940
	public Vector2 aircraftHeadings;

	// Token: 0x04000795 RID: 1941
	public float airSearchRange;

	// Token: 0x04000796 RID: 1942
	public List<string> missionTypes;

	// Token: 0x04000797 RID: 1943
	public bool hasAirbase;

	// Token: 0x04000798 RID: 1944
	public float aircraftTimer;

	// Token: 0x04000799 RID: 1945
	public Vector3[] aircraftWaypoints;

	// Token: 0x0400079A RID: 1946
	public int currentWaypoint;

	// Token: 0x0400079B RID: 1947
	public bool onPatrol;

	// Token: 0x0400079C RID: 1948
	public float strategicPointValue;

	// Token: 0x0400079D RID: 1949
	public int[] linksToWaypoint;

	// Token: 0x0400079E RID: 1950
	public int linksToRegionWaypoint;

	// Token: 0x0400079F RID: 1951
	public string[] linksToSosus;

	// Token: 0x040007A0 RID: 1952
	public Vector2 combatCoords;
}
