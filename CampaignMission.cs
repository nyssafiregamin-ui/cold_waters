using System;
using UnityEngine;

// Token: 0x020000F7 RID: 247
public class CampaignMission : ScriptableObject
{
	// Token: 0x040007C1 RID: 1985
	public int missionID;

	// Token: 0x040007C2 RID: 1986
	public string missionType;

	// Token: 0x040007C3 RID: 1987
	public string missionFileName;

	// Token: 0x040007C4 RID: 1988
	public string[] startLocation;

	// Token: 0x040007C5 RID: 1989
	public string[] endLocation;

	// Token: 0x040007C6 RID: 1990
	public string startAlignment;

	// Token: 0x040007C7 RID: 1991
	public string endAlignment;

	// Token: 0x040007C8 RID: 1992
	public Vector2 hoursToStart;

	// Token: 0x040007C9 RID: 1993
	public float speed;

	// Token: 0x040007CA RID: 1994
	public int[] mustUseWaypoints;

	// Token: 0x040007CB RID: 1995
	public int[] useAtLeastOneWaypointOf;

	// Token: 0x040007CC RID: 1996
	public int[] prohibitedWaypoints;

	// Token: 0x040007CD RID: 1997
	public Vector2 patrolForHours;

	// Token: 0x040007CE RID: 1998
	public float strategicValue;

	// Token: 0x040007CF RID: 1999
	public string missionEndsWhen;

	// Token: 0x040007D0 RID: 2000
	public bool disruptOnFail;

	// Token: 0x040007D1 RID: 2001
	public bool disruptOnPass;

	// Token: 0x040007D2 RID: 2002
	public bool invadeOnFail;

	// Token: 0x040007D3 RID: 2003
	public bool requiresStealth;

	// Token: 0x040007D4 RID: 2004
	public int requiresWeaponID;

	// Token: 0x040007D5 RID: 2005
	public int numberOfRequiredWeapon;

	// Token: 0x040007D6 RID: 2006
	public string numberOfRequiredWeaponWord;

	// Token: 0x040007D7 RID: 2007
	public Vector2[] numberOfEnemyUnits;

	// Token: 0x040007D8 RID: 2008
	public bool[] enemyUnitMissionCritical;

	// Token: 0x040007D9 RID: 2009
	public string[] enemyShipClasses;

	// Token: 0x040007DA RID: 2010
	public string[] enemyShipBehaviour;

	// Token: 0x040007DB RID: 2011
	public bool finalMission;

	// Token: 0x040007DC RID: 2012
	public bool playerMission;

	// Token: 0x040007DD RID: 2013
	public string taskForceBehaviour;

	// Token: 0x040007DE RID: 2014
	public string eventWin;

	// Token: 0x040007DF RID: 2015
	public string eventFail;
}
