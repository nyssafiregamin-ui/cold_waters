using System;
using UnityEngine;

// Token: 0x020000F6 RID: 246
public class CampaignRegionWaypoint : ScriptableObject
{
	// Token: 0x040007B0 RID: 1968
	public int waypointID;

	// Token: 0x040007B1 RID: 1969
	public string waypointName;

	// Token: 0x040007B2 RID: 1970
	public string waypointDescriptiveName;

	// Token: 0x040007B3 RID: 1971
	public string country;

	// Token: 0x040007B4 RID: 1972
	public string faction;

	// Token: 0x040007B5 RID: 1973
	public string currentFaction;

	// Token: 0x040007B6 RID: 1974
	public Vector3 waypointPosition;

	// Token: 0x040007B7 RID: 1975
	public string[] northWaypointsNames;

	// Token: 0x040007B8 RID: 1976
	public string[] southWaypointsNames;

	// Token: 0x040007B9 RID: 1977
	public string[] eastWaypointsNames;

	// Token: 0x040007BA RID: 1978
	public string[] westWaypointsNames;

	// Token: 0x040007BB RID: 1979
	public int[] connectedWaypoints = new int[0];

	// Token: 0x040007BC RID: 1980
	public int[] northWaypoints;

	// Token: 0x040007BD RID: 1981
	public int[] southWaypoints;

	// Token: 0x040007BE RID: 1982
	public int[] eastWaypoints;

	// Token: 0x040007BF RID: 1983
	public int[] westWaypoints;

	// Token: 0x040007C0 RID: 1984
	public string invadedByRoute;
}
