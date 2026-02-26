using System;

// Token: 0x02000153 RID: 339
[Serializable]
internal class SaveDataCampaign
{
	// Token: 0x04000E9D RID: 3741
	public string saveFileName;

	// Token: 0x04000E9E RID: 3742
	public string gameVersion;

	// Token: 0x04000E9F RID: 3743
	public string campaignReferenceName;

	// Token: 0x04000EA0 RID: 3744
	public float julianStartDate;

	// Token: 0x04000EA1 RID: 3745
	public float hoursDurationOfCampaign;

	// Token: 0x04000EA2 RID: 3746
	public float hoursSinceLastGeneralEvent;

	// Token: 0x04000EA3 RID: 3747
	public float generalEventModifier;

	// Token: 0x04000EA4 RID: 3748
	public int lastMissionCompleteStatus;

	// Token: 0x04000EA5 RID: 3749
	public float campaignPoints;

	// Token: 0x04000EA6 RID: 3750
	public string playerCommanderName;

	// Token: 0x04000EA7 RID: 3751
	public bool playerHasMission;

	// Token: 0x04000EA8 RID: 3752
	public string playerMissionType;

	// Token: 0x04000EA9 RID: 3753
	public int currentMissionTaskForceID;

	// Token: 0x04000EAA RID: 3754
	public string missionGivenOnDate;

	// Token: 0x04000EAB RID: 3755
	public float totalTonnage;

	// Token: 0x04000EAC RID: 3756
	public float patrolTonnage;

	// Token: 0x04000EAD RID: 3757
	public float patrolPoints;

	// Token: 0x04000EAE RID: 3758
	public float[] campaignStats;

	// Token: 0x04000EAF RID: 3759
	public int missionsPassed;

	// Token: 0x04000EB0 RID: 3760
	public int[] patrolMedals;

	// Token: 0x04000EB1 RID: 3761
	public int[] cumulativeMedals;

	// Token: 0x04000EB2 RID: 3762
	public int[] woundedMedals;

	// Token: 0x04000EB3 RID: 3763
	public int[] playerVesselsLost;

	// Token: 0x04000EB4 RID: 3764
	public int[] playerInstancesLost;

	// Token: 0x04000EB5 RID: 3765
	public float playerPostionOnMapX;

	// Token: 0x04000EB6 RID: 3766
	public float playerPostionOnMapY;

	// Token: 0x04000EB7 RID: 3767
	public string playerVesselClass;

	// Token: 0x04000EB8 RID: 3768
	public int playerVesselInstance;

	// Token: 0x04000EB9 RID: 3769
	public float secondsCampaignPlayed;

	// Token: 0x04000EBA RID: 3770
	public float playerRevealed;

	// Token: 0x04000EBB RID: 3771
	public bool playerInPort;

	// Token: 0x04000EBC RID: 3772
	public string[] groundWarZoneCurrentFactions;

	// Token: 0x04000EBD RID: 3773
	public bool armisticeEventOccurred;

	// Token: 0x04000EBE RID: 3774
	public bool thresholdMet;

	// Token: 0x04000EBF RID: 3775
	public int[] playerTorpeodesOnBoard;

	// Token: 0x04000EC0 RID: 3776
	public int[] playerVLSTorpeodesOnBoard;

	// Token: 0x04000EC1 RID: 3777
	public int[] playerTubeStatus;

	// Token: 0x04000EC2 RID: 3778
	public int[] playerWeaponInTube;

	// Token: 0x04000EC3 RID: 3779
	public int[] playerSettingsOne;

	// Token: 0x04000EC4 RID: 3780
	public int[] playerSettingsTwo;

	// Token: 0x04000EC5 RID: 3781
	public int[] playerSettingsThree;

	// Token: 0x04000EC6 RID: 3782
	public int playerNoisemakersOnBoard;

	// Token: 0x04000EC7 RID: 3783
	public bool playerSealTeamOnBoard;

	// Token: 0x04000EC8 RID: 3784
	public float vesselTotalDamage;

	// Token: 0x04000EC9 RID: 3785
	public float[] compartmentCurrentFlooding;

	// Token: 0x04000ECA RID: 3786
	public float[] compartmentTotalFlooding;

	// Token: 0x04000ECB RID: 3787
	public int[] decalNames;

	// Token: 0x04000ECC RID: 3788
	public float[] damageControlCurrentTimers;

	// Token: 0x04000ECD RID: 3789
	public string[] factions;

	// Token: 0x04000ECE RID: 3790
	public float[] aircratTimers;

	// Token: 0x04000ECF RID: 3791
	public float[,] aircraftWaypoints0;

	// Token: 0x04000ED0 RID: 3792
	public float[,] aircraftWaypoints1;

	// Token: 0x04000ED1 RID: 3793
	public int[] aircraftCurrentWaypoint;

	// Token: 0x04000ED2 RID: 3794
	public float[,] aircraftCurrentPositions;

	// Token: 0x04000ED3 RID: 3795
	public bool[] aircraftActive;

	// Token: 0x04000ED4 RID: 3796
	public float[] aircraftPrepTimes;

	// Token: 0x04000ED5 RID: 3797
	public bool[] onPatrol;

	// Token: 0x04000ED6 RID: 3798
	public bool[] sosusStatus;

	// Token: 0x04000ED7 RID: 3799
	public float[] satelliteTimers;

	// Token: 0x04000ED8 RID: 3800
	public float[] satelliteCurrentAngles;

	// Token: 0x04000ED9 RID: 3801
	public float[,] satelliteCurrentPositions;

	// Token: 0x04000EDA RID: 3802
	public float[,] satelliteWaypoints0;

	// Token: 0x04000EDB RID: 3803
	public float[,] satelliteWaypoints1;

	// Token: 0x04000EDC RID: 3804
	public bool[] satellitesReversed;

	// Token: 0x04000EDD RID: 3805
	public bool[] satellitesMoving;

	// Token: 0x04000EDE RID: 3806
	public bool[] satellitesActive;

	// Token: 0x04000EDF RID: 3807
	public bool[] activeTaskForces;

	// Token: 0x04000EE0 RID: 3808
	public float[,] taskForceCurrentPositions;

	// Token: 0x04000EE1 RID: 3809
	public int[] missionType;

	// Token: 0x04000EE2 RID: 3810
	public int[] actualMission;

	// Token: 0x04000EE3 RID: 3811
	public int[] missionID;

	// Token: 0x04000EE4 RID: 3812
	public bool[] isCurrentlyInUse;

	// Token: 0x04000EE5 RID: 3813
	public float[] speed;

	// Token: 0x04000EE6 RID: 3814
	public int[] prevWaypoint;

	// Token: 0x04000EE7 RID: 3815
	public int[] currentWaypoint;

	// Token: 0x04000EE8 RID: 3816
	public int[] legOfRoute;

	// Token: 0x04000EE9 RID: 3817
	public float[] missionStartTimers;

	// Token: 0x04000EEA RID: 3818
	public int[] mustUseWaypointTaskForce;

	// Token: 0x04000EEB RID: 3819
	public int[] allMustUseWaypoints;

	// Token: 0x04000EEC RID: 3820
	public int[] prohibitedWaypointTaskForce;

	// Token: 0x04000EED RID: 3821
	public int[] allProhibitedWaypoints;

	// Token: 0x04000EEE RID: 3822
	public float[,] waypointPosition;

	// Token: 0x04000EEF RID: 3823
	public float[,] finalLocationPosition;

	// Token: 0x04000EF0 RID: 3824
	public string[] currTile;

	// Token: 0x04000EF1 RID: 3825
	public string[] destTile;

	// Token: 0x04000EF2 RID: 3826
	public string[] nextTile;

	// Token: 0x04000EF3 RID: 3827
	public bool[] takeDirectRoute;

	// Token: 0x04000EF4 RID: 3828
	public bool[] startIsLocation;

	// Token: 0x04000EF5 RID: 3829
	public bool[] endIsLocation;

	// Token: 0x04000EF6 RID: 3830
	public bool[] finalLegDone;

	// Token: 0x04000EF7 RID: 3831
	public int[] startPosID;

	// Token: 0x04000EF8 RID: 3832
	public int[] endLocationID;

	// Token: 0x04000EF9 RID: 3833
	public int[] allShipNumbers;

	// Token: 0x04000EFA RID: 3834
	public int[] shipNumberTaskForces;

	// Token: 0x04000EFB RID: 3835
	public string[] allShipClasses;

	// Token: 0x04000EFC RID: 3836
	public bool[] allMissionCritVessels;

	// Token: 0x04000EFD RID: 3837
	public bool[] allMissionDefensiveVessels;

	// Token: 0x04000EFE RID: 3838
	public int[] allShipsTaskForces;

	// Token: 0x04000EFF RID: 3839
	public int[] currentAction;

	// Token: 0x04000F00 RID: 3840
	public string[] behaviourType;

	// Token: 0x04000F01 RID: 3841
	public int[] taskForceType;

	// Token: 0x04000F02 RID: 3842
	public float[] patrolForHours;

	// Token: 0x04000F03 RID: 3843
	public float[] revealContactTimers;

	// Token: 0x04000F04 RID: 3844
	public float[] lastRevealContactPositionx;

	// Token: 0x04000F05 RID: 3845
	public float[] lastRevealContactPositiony;
}
