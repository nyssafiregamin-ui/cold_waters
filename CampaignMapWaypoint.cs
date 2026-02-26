using System;
using UnityEngine;

// Token: 0x020000F5 RID: 245
public class CampaignMapWaypoint : ScriptableObject
{
	// Token: 0x040007A1 RID: 1953
	public int waypointID;

	// Token: 0x040007A2 RID: 1954
	public string waypointName;

	// Token: 0x040007A3 RID: 1955
	public string waypointDescriptiveName;

	// Token: 0x040007A4 RID: 1956
	public Vector3 waypointPosition;

	// Token: 0x040007A5 RID: 1957
	public float waypointRadius;

	// Token: 0x040007A6 RID: 1958
	public string[] northWaypointsNames;

	// Token: 0x040007A7 RID: 1959
	public string[] southWaypointsNames;

	// Token: 0x040007A8 RID: 1960
	public string[] eastWaypointsNames;

	// Token: 0x040007A9 RID: 1961
	public string[] westWaypointsNames;

	// Token: 0x040007AA RID: 1962
	public int[] northWaypoints;

	// Token: 0x040007AB RID: 1963
	public int[] southWaypoints;

	// Token: 0x040007AC RID: 1964
	public int[] eastWaypoints;

	// Token: 0x040007AD RID: 1965
	public int[] westWaypoints;

	// Token: 0x040007AE RID: 1966
	public bool submarineOnly;

	// Token: 0x040007AF RID: 1967
	public int[] linksToLocation;
}
