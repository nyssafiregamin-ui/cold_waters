using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000152 RID: 338
public class SaveLoadManager : MonoBehaviour
{
	// Token: 0x060009BD RID: 2493 RVA: 0x00070350 File Offset: 0x0006E550
	public void SaveCampaign(bool autosave)
	{
		string text = Path.Combine(Application.persistentDataPath, GameDataManager.saveGameFolder);
		text = Path.Combine(text, CampaignManager.campaignReferenceName);
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		if (!GameDataManager.optionsBoolSettings[12])
		{
			autosave = false;
		}
		if (!autosave)
		{
			text = Path.Combine(text, this.campaignmanager.campaignSaveFileName + GameDataManager.saveGameFileExtension);
		}
		else
		{
			text = Path.Combine(text, "AutoSave" + GameDataManager.saveGameFileExtension);
		}
		System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream fileStream = File.Create(text);
		SaveDataCampaign saveDataCampaign = new SaveDataCampaign();
		saveDataCampaign.saveFileName = this.campaignmanager.campaignSaveFileName;
		saveDataCampaign.gameVersion = GameDataManager.gameVersion;
		saveDataCampaign.campaignReferenceName = CampaignManager.campaignReferenceName;
		saveDataCampaign.hoursDurationOfCampaign = this.campaignmanager.hoursDurationOfCampaign;
		saveDataCampaign.julianStartDate = this.campaignmanager.julianStartDay;
		saveDataCampaign.hoursSinceLastGeneralEvent = this.campaignmanager.hoursToNextGeneralEvent;
		saveDataCampaign.generalEventModifier = this.campaignmanager.generalEventModifier;
		saveDataCampaign.lastMissionCompleteStatus = this.campaignmanager.lastMissionCompleteStatus;
		saveDataCampaign.campaignPoints = this.campaignmanager.campaignPoints;
		saveDataCampaign.playerCommanderName = GameDataManager.playerCommanderName;
		saveDataCampaign.playerHasMission = this.campaignmanager.playercampaigndata.playerHasMission;
		saveDataCampaign.playerMissionType = this.campaignmanager.playercampaigndata.playerMissionType;
		saveDataCampaign.currentMissionTaskForceID = this.campaignmanager.playercampaigndata.currentMissionTaskForceID;
		saveDataCampaign.missionGivenOnDate = this.campaignmanager.missionGivenOnDate;
		saveDataCampaign.playerPostionOnMapX = this.campaignmanager.playerGameObject.transform.localPosition.x;
		saveDataCampaign.playerPostionOnMapY = this.campaignmanager.playerGameObject.transform.localPosition.y;
		saveDataCampaign.playerVesselClass = UIFunctions.globaluifunctions.playerfunctions.playerVessel.databaseshipdata.shipPrefabName;
		saveDataCampaign.playerVesselInstance = UIFunctions.globaluifunctions.playerfunctions.playerVesselInstance;
		saveDataCampaign.secondsCampaignPlayed = this.campaignmanager.playercampaigndata.secondsCampaignPlayed;
		saveDataCampaign.playerRevealed = this.campaignmanager.playercampaigndata.playerRevealed;
		saveDataCampaign.playerInPort = this.campaignmanager.playerInPort;
		saveDataCampaign.totalTonnage = this.campaignmanager.playercampaigndata.totalTonnage;
		saveDataCampaign.patrolTonnage = this.campaignmanager.playercampaigndata.patrolTonnage;
		saveDataCampaign.patrolPoints = this.campaignmanager.playercampaigndata.patrolPoints;
		saveDataCampaign.campaignStats = this.campaignmanager.playercampaigndata.campaignStats;
		saveDataCampaign.missionsPassed = this.campaignmanager.playercampaigndata.missionsPassed;
		saveDataCampaign.patrolMedals = this.campaignmanager.playercampaigndata.patrolMedals;
		saveDataCampaign.cumulativeMedals = this.campaignmanager.playercampaigndata.cumulativeMedals;
		saveDataCampaign.woundedMedals = this.campaignmanager.playercampaigndata.woundedMedals;
		saveDataCampaign.playerVesselsLost = this.campaignmanager.playercampaigndata.playerVesselsLost;
		saveDataCampaign.playerInstancesLost = this.campaignmanager.playercampaigndata.playerInstancesLost;
		saveDataCampaign.groundWarZoneCurrentFactions = new string[this.campaignmanager.campaignregionwaypoints.Length];
		for (int i = 0; i < this.campaignmanager.campaignregionwaypoints.Length; i++)
		{
			saveDataCampaign.groundWarZoneCurrentFactions[i] = this.campaignmanager.campaignregionwaypoints[i].currentFaction;
		}
		saveDataCampaign.thresholdMet = this.campaignmanager.thresholdMet;
		saveDataCampaign.armisticeEventOccurred = this.campaignmanager.armisticeEventOccurred;
		saveDataCampaign.sosusStatus = new bool[this.campaignmanager.sosusBarriers.Length];
		for (int j = 0; j < saveDataCampaign.sosusStatus.Length; j++)
		{
			saveDataCampaign.sosusStatus[j] = this.campaignmanager.sosusBarriers[j].activeSelf;
		}
		saveDataCampaign.playerTorpeodesOnBoard = this.campaignmanager.playercampaigndata.playerTorpeodesOnBoard;
		saveDataCampaign.playerVLSTorpeodesOnBoard = this.campaignmanager.playercampaigndata.playerVLSTorpeodesOnBoard;
		saveDataCampaign.playerTubeStatus = this.campaignmanager.playercampaigndata.playerTubeStatus;
		saveDataCampaign.playerWeaponInTube = this.campaignmanager.playercampaigndata.playerWeaponInTube;
		saveDataCampaign.playerSettingsOne = this.campaignmanager.playercampaigndata.playerSettingsOne;
		saveDataCampaign.playerSettingsTwo = this.campaignmanager.playercampaigndata.playerSettingsTwo;
		saveDataCampaign.playerSettingsThree = this.campaignmanager.playercampaigndata.playerSettingsThree;
		saveDataCampaign.playerNoisemakersOnBoard = this.campaignmanager.playercampaigndata.playerNoisemakersOnBoard;
		saveDataCampaign.playerSealTeamOnBoard = this.campaignmanager.playercampaigndata.playerSealTeamOnBoard;
		saveDataCampaign.vesselTotalDamage = this.campaignmanager.playercampaigndata.vesselTotalDamage;
		saveDataCampaign.compartmentCurrentFlooding = new float[UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentCurrentFlooding.Length];
		saveDataCampaign.compartmentTotalFlooding = new float[UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentTotalFlooding.Length];
		for (int k = 0; k < UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentCurrentFlooding.Length; k++)
		{
			saveDataCampaign.compartmentCurrentFlooding[k] = UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentCurrentFlooding[k];
			saveDataCampaign.compartmentTotalFlooding[k] = UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentTotalFlooding[k];
		}
		saveDataCampaign.damageControlCurrentTimers = new float[UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlCurrentTimers.Length];
		for (int l = 0; l < UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlCurrentTimers.Length; l++)
		{
			saveDataCampaign.damageControlCurrentTimers[l] = UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlCurrentTimers[l];
		}
		saveDataCampaign.decalNames = this.campaignmanager.playercampaigndata.decalNames;
		saveDataCampaign.factions = new string[this.campaignmanager.campaignlocations.Length];
		saveDataCampaign.aircratTimers = new float[this.campaignmanager.campaignlocations.Length];
		saveDataCampaign.aircraftWaypoints0 = new float[this.campaignmanager.campaignlocations.Length, 3];
		saveDataCampaign.aircraftWaypoints1 = new float[this.campaignmanager.campaignlocations.Length, 3];
		saveDataCampaign.aircraftCurrentWaypoint = new int[this.campaignmanager.campaignlocations.Length];
		saveDataCampaign.aircraftPrepTimes = new float[this.campaignmanager.campaignlocations.Length];
		saveDataCampaign.aircraftCurrentPositions = new float[this.campaignmanager.campaignAircraftObjects.Length, 4];
		saveDataCampaign.aircraftActive = new bool[this.campaignmanager.campaignAircraftObjects.Length];
		saveDataCampaign.onPatrol = new bool[this.campaignmanager.campaignAircraftObjects.Length];
		for (int m = 0; m < this.campaignmanager.campaignAircraftObjects.Length; m++)
		{
			saveDataCampaign.factions[m] = this.campaignmanager.campaignlocations[m].faction;
			saveDataCampaign.aircratTimers[m] = this.campaignmanager.campaignlocations[m].aircraftTimer;
			saveDataCampaign.aircraftCurrentWaypoint[m] = this.campaignmanager.campaignlocations[m].currentWaypoint;
			saveDataCampaign.aircraftPrepTimes[m] = this.campaignmanager.aircraftPrepTime[m];
			saveDataCampaign.onPatrol[m] = this.campaignmanager.campaignlocations[m].onPatrol;
			if (this.campaignmanager.campaignAircraftObjects[m] != null)
			{
				saveDataCampaign.aircraftCurrentPositions[m, 0] = this.campaignmanager.campaignAircraftObjects[m].transform.localPosition.x;
				saveDataCampaign.aircraftCurrentPositions[m, 1] = this.campaignmanager.campaignAircraftObjects[m].transform.localPosition.y;
				saveDataCampaign.aircraftCurrentPositions[m, 2] = this.campaignmanager.campaignAircraftObjects[m].transform.localRotation.x;
				saveDataCampaign.aircraftCurrentPositions[m, 3] = this.campaignmanager.campaignAircraftObjects[m].transform.localRotation.y;
				saveDataCampaign.aircraftActive[m] = this.campaignmanager.campaignAircraftObjects[m].activeSelf;
				if (this.campaignmanager.campaignlocations[m].aircraftWaypoints != null)
				{
					saveDataCampaign.aircraftWaypoints0[m, 0] = this.campaignmanager.campaignlocations[m].aircraftWaypoints[0].x;
					saveDataCampaign.aircraftWaypoints0[m, 1] = this.campaignmanager.campaignlocations[m].aircraftWaypoints[0].y;
					saveDataCampaign.aircraftWaypoints0[m, 2] = this.campaignmanager.campaignlocations[m].aircraftWaypoints[0].z;
					saveDataCampaign.aircraftWaypoints1[m, 0] = this.campaignmanager.campaignlocations[m].aircraftWaypoints[1].x;
					saveDataCampaign.aircraftWaypoints1[m, 1] = this.campaignmanager.campaignlocations[m].aircraftWaypoints[1].y;
					saveDataCampaign.aircraftWaypoints1[m, 2] = this.campaignmanager.campaignlocations[m].aircraftWaypoints[1].z;
				}
			}
		}
		saveDataCampaign.satelliteTimers = new float[this.campaignmanager.campaignSatelliteObjects.Length];
		saveDataCampaign.satelliteCurrentAngles = new float[this.campaignmanager.campaignSatelliteObjects.Length];
		saveDataCampaign.satelliteCurrentPositions = new float[this.campaignmanager.campaignSatelliteObjects.Length, 4];
		saveDataCampaign.satelliteWaypoints0 = new float[this.campaignmanager.campaignSatelliteObjects.Length, 3];
		saveDataCampaign.satelliteWaypoints1 = new float[this.campaignmanager.campaignSatelliteObjects.Length, 3];
		saveDataCampaign.satellitesReversed = new bool[this.campaignmanager.campaignSatelliteObjects.Length];
		saveDataCampaign.satellitesMoving = new bool[this.campaignmanager.campaignSatelliteObjects.Length];
		saveDataCampaign.satellitesActive = new bool[this.campaignmanager.campaignSatelliteObjects.Length];
		for (int n = 0; n < this.campaignmanager.campaignSatelliteObjects.Length; n++)
		{
			saveDataCampaign.satelliteTimers[n] = this.campaignmanager.satelliteTimers[n];
			saveDataCampaign.satelliteCurrentAngles[n] = this.campaignmanager.currentSatelliteAngle[n];
			saveDataCampaign.satelliteCurrentPositions[n, 0] = this.campaignmanager.campaignSatelliteObjects[n].transform.localPosition.x;
			saveDataCampaign.satelliteCurrentPositions[n, 1] = this.campaignmanager.campaignSatelliteObjects[n].transform.localPosition.y;
			saveDataCampaign.satelliteCurrentPositions[n, 2] = this.campaignmanager.campaignSatelliteObjects[n].transform.localRotation.x;
			saveDataCampaign.satelliteCurrentPositions[n, 3] = this.campaignmanager.campaignSatelliteObjects[n].transform.localRotation.y;
			saveDataCampaign.satelliteWaypoints0[n, 0] = this.campaignmanager.satelliteWaypoints[0].x;
			saveDataCampaign.satelliteWaypoints0[n, 1] = this.campaignmanager.satelliteWaypoints[0].y;
			saveDataCampaign.satelliteWaypoints0[n, 2] = this.campaignmanager.satelliteWaypoints[0].z;
			saveDataCampaign.satelliteWaypoints1[n, 0] = this.campaignmanager.satelliteWaypoints[1].x;
			saveDataCampaign.satelliteWaypoints1[n, 1] = this.campaignmanager.satelliteWaypoints[1].y;
			saveDataCampaign.satelliteWaypoints1[n, 2] = this.campaignmanager.satelliteWaypoints[1].z;
			saveDataCampaign.satellitesReversed[n] = this.campaignmanager.satelliteReversed[n];
			saveDataCampaign.satellitesMoving[n] = this.campaignmanager.satelliteMoving[n];
			saveDataCampaign.satellitesActive[n] = this.campaignmanager.campaignSatelliteObjects[n].activeSelf;
		}
		saveDataCampaign.missionType = new int[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.actualMission = new int[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.missionID = new int[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.speed = new float[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.prevWaypoint = new int[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.currentWaypoint = new int[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.legOfRoute = new int[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.missionStartTimers = new float[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.currTile = new string[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.destTile = new string[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.nextTile = new string[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.takeDirectRoute = new bool[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.startIsLocation = new bool[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.endIsLocation = new bool[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.finalLegDone = new bool[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.startPosID = new int[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.endLocationID = new int[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.currentAction = new int[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.taskForceType = new int[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.behaviourType = new string[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.patrolForHours = new float[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.waypointPosition = new float[this.campaignmanager.campaignTaskForces.Length, 3];
		saveDataCampaign.finalLocationPosition = new float[this.campaignmanager.campaignTaskForces.Length, 2];
		saveDataCampaign.taskForceCurrentPositions = new float[this.campaignmanager.campaignTaskForces.Length, 2];
		saveDataCampaign.revealContactTimers = new float[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.lastRevealContactPositionx = new float[this.campaignmanager.campaignTaskForces.Length];
		saveDataCampaign.lastRevealContactPositiony = new float[this.campaignmanager.campaignTaskForces.Length];
		for (int num = 0; num < this.campaignmanager.campaignTaskForces.Length; num++)
		{
			saveDataCampaign.missionType[num] = this.campaignmanager.campaignTaskForces[num].missionType;
			saveDataCampaign.actualMission[num] = this.campaignmanager.campaignTaskForces[num].actualMission;
			saveDataCampaign.missionID[num] = this.campaignmanager.campaignTaskForces[num].missionID;
			saveDataCampaign.speed[num] = this.campaignmanager.campaignTaskForces[num].speed;
			saveDataCampaign.prevWaypoint[num] = this.campaignmanager.campaignTaskForces[num].previousWaypoint;
			saveDataCampaign.currentWaypoint[num] = this.campaignmanager.campaignTaskForces[num].currentWaypoint;
			saveDataCampaign.legOfRoute[num] = this.campaignmanager.campaignTaskForces[num].legOfRoute;
			saveDataCampaign.missionStartTimers[num] = this.campaignmanager.taskForceActivateTimers[num];
			saveDataCampaign.currTile[num] = this.campaignmanager.campaignTaskForces[num].currTile;
			saveDataCampaign.nextTile[num] = this.campaignmanager.campaignTaskForces[num].nextTile;
			saveDataCampaign.destTile[num] = this.campaignmanager.campaignTaskForces[num].destTile;
			saveDataCampaign.takeDirectRoute[num] = this.campaignmanager.campaignTaskForces[num].takeDirectRoute;
			saveDataCampaign.startIsLocation[num] = this.campaignmanager.campaignTaskForces[num].startIsLocation;
			saveDataCampaign.endIsLocation[num] = this.campaignmanager.campaignTaskForces[num].endIsLocation;
			saveDataCampaign.finalLegDone[num] = this.campaignmanager.campaignTaskForces[num].finalLegDone;
			saveDataCampaign.startPosID[num] = this.campaignmanager.campaignTaskForces[num].startPositionID;
			saveDataCampaign.endLocationID[num] = this.campaignmanager.campaignTaskForces[num].endLocationID;
			saveDataCampaign.currentAction[num] = this.campaignmanager.campaignTaskForces[num].currentAction;
			saveDataCampaign.taskForceType[num] = this.campaignmanager.campaignTaskForces[num].taskForceType;
			saveDataCampaign.behaviourType[num] = this.campaignmanager.campaignTaskForces[num].behaviourType;
			saveDataCampaign.patrolForHours[num] = this.campaignmanager.campaignTaskForces[num].patrolForHours;
			saveDataCampaign.waypointPosition[num, 0] = this.campaignmanager.campaignTaskForces[num].waypointPosition.x;
			saveDataCampaign.waypointPosition[num, 1] = this.campaignmanager.campaignTaskForces[num].waypointPosition.y;
			saveDataCampaign.waypointPosition[num, 2] = this.campaignmanager.campaignTaskForces[num].waypointPosition.z;
			saveDataCampaign.finalLocationPosition[num, 0] = this.campaignmanager.campaignTaskForces[num].finalLocationPosition.x;
			saveDataCampaign.finalLocationPosition[num, 1] = this.campaignmanager.campaignTaskForces[num].finalLocationPosition.y;
			saveDataCampaign.taskForceCurrentPositions[num, 0] = this.campaignmanager.campaignTaskForceObjects[num].transform.localPosition.x;
			saveDataCampaign.taskForceCurrentPositions[num, 1] = this.campaignmanager.campaignTaskForceObjects[num].transform.localPosition.y;
			saveDataCampaign.revealContactTimers[num] = this.campaignmanager.campaignTaskForceObjects[num].GetComponent<CampaignMapRevealContact>().timer;
			saveDataCampaign.lastRevealContactPositionx[num] = this.campaignmanager.campaignTaskForceObjects[num].GetComponent<CampaignMapRevealContact>().lastKnownPosition.x;
			saveDataCampaign.lastRevealContactPositiony[num] = this.campaignmanager.campaignTaskForceObjects[num].GetComponent<CampaignMapRevealContact>().lastKnownPosition.y;
		}
		saveDataCampaign.activeTaskForces = this.campaignmanager.activeTaskForces;
		int num2 = 0;
		for (int num3 = 0; num3 < this.campaignmanager.campaignTaskForces.Length; num3++)
		{
			if (this.campaignmanager.campaignTaskForces[num3].mustUseWaypoints != null)
			{
				num2 += this.campaignmanager.campaignTaskForces[num3].mustUseWaypoints.Length;
			}
		}
		saveDataCampaign.allMustUseWaypoints = new int[num2];
		saveDataCampaign.mustUseWaypointTaskForce = new int[num2];
		int num4 = 0;
		for (int num5 = 0; num5 < this.campaignmanager.campaignTaskForces.Length; num5++)
		{
			if (this.campaignmanager.campaignTaskForces[num5].mustUseWaypoints != null)
			{
				for (int num6 = 0; num6 < this.campaignmanager.campaignTaskForces[num5].mustUseWaypoints.Length; num6++)
				{
					saveDataCampaign.allMustUseWaypoints[num4] = this.campaignmanager.campaignTaskForces[num5].mustUseWaypoints[num6];
					saveDataCampaign.mustUseWaypointTaskForce[num4] = num5;
					num4++;
				}
			}
		}
		num2 = 0;
		for (int num7 = 0; num7 < this.campaignmanager.campaignTaskForces.Length; num7++)
		{
			if (this.campaignmanager.campaignTaskForces[num7].prohibitedWaypoints != null)
			{
				num2 += this.campaignmanager.campaignTaskForces[num7].prohibitedWaypoints.Length;
			}
		}
		saveDataCampaign.allProhibitedWaypoints = new int[num2];
		saveDataCampaign.prohibitedWaypointTaskForce = new int[num2];
		num4 = 0;
		for (int num8 = 0; num8 < this.campaignmanager.campaignTaskForces.Length; num8++)
		{
			if (this.campaignmanager.campaignTaskForces[num8].prohibitedWaypoints != null)
			{
				for (int num9 = 0; num9 < this.campaignmanager.campaignTaskForces[num8].prohibitedWaypoints.Length; num9++)
				{
					saveDataCampaign.allProhibitedWaypoints[num4] = this.campaignmanager.campaignTaskForces[num8].prohibitedWaypoints[num9];
					saveDataCampaign.prohibitedWaypointTaskForce[num4] = num8;
					num4++;
				}
			}
		}
		int num10 = 0;
		for (int num11 = 0; num11 < this.campaignmanager.campaignTaskForces.Length; num11++)
		{
			if (this.campaignmanager.campaignTaskForces[num11].shipClasses != null)
			{
				num10 += this.campaignmanager.campaignTaskForces[num11].shipClasses.Length;
			}
		}
		num4 = 0;
		saveDataCampaign.allShipClasses = new string[num10];
		saveDataCampaign.allMissionCritVessels = new bool[num10];
		saveDataCampaign.allMissionDefensiveVessels = new bool[num10];
		saveDataCampaign.allShipsTaskForces = new int[num10];
		for (int num12 = 0; num12 < this.campaignmanager.campaignTaskForces.Length; num12++)
		{
			if (this.campaignmanager.campaignTaskForces[num12].shipClasses != null)
			{
				for (int num13 = 0; num13 < this.campaignmanager.campaignTaskForces[num12].shipClasses.Length; num13++)
				{
					saveDataCampaign.allShipClasses[num4] = this.campaignmanager.campaignTaskForces[num12].shipClasses[num13];
					saveDataCampaign.allMissionCritVessels[num4] = this.campaignmanager.campaignTaskForces[num12].missionCriticalVessels[num13];
					saveDataCampaign.allMissionDefensiveVessels[num4] = this.campaignmanager.campaignTaskForces[num12].defensiveVessels[num13];
					saveDataCampaign.allShipsTaskForces[num4] = num12;
					num4++;
				}
			}
		}
		num10 = 0;
		for (int num14 = 0; num14 < this.campaignmanager.campaignTaskForces.Length; num14++)
		{
			if (this.campaignmanager.campaignTaskForces[num14].shipNumbers != null)
			{
				num10 += this.campaignmanager.campaignTaskForces[num14].shipNumbers.Length;
			}
		}
		saveDataCampaign.allShipNumbers = new int[num10];
		saveDataCampaign.shipNumberTaskForces = new int[num10];
		num4 = 0;
		for (int num15 = 0; num15 < this.campaignmanager.campaignTaskForces.Length; num15++)
		{
			if (this.campaignmanager.campaignTaskForces[num15].shipNumbers != null)
			{
				for (int num16 = 0; num16 < this.campaignmanager.campaignTaskForces[num15].shipNumbers.Length; num16++)
				{
					saveDataCampaign.allShipNumbers[num4] = this.campaignmanager.campaignTaskForces[num15].shipNumbers[num16];
					saveDataCampaign.shipNumberTaskForces[num4] = num15;
					num4++;
				}
			}
		}
		binaryFormatter.Serialize(fileStream, saveDataCampaign);
		fileStream.Close();
	}

	// Token: 0x060009BE RID: 2494 RVA: 0x00071AEC File Offset: 0x0006FCEC
	public static void AutoSaveCampaign()
	{
		if (!GameDataManager.optionsBoolSettings[11] && GameDataManager.optionsBoolSettings[12])
		{
			return;
		}
		UIFunctions.globaluifunctions.database.saveloadmanager.SaveCampaign(true);
	}

	// Token: 0x060009BF RID: 2495 RVA: 0x00071B2C File Offset: 0x0006FD2C
	public void LoadTheCampaign()
	{
		UIFunctions.globaluifunctions.selectionGroupPanel.SetActive(false);
		UIFunctions.globaluifunctions.ClearSelectionToggleBars();
		UIFunctions.globaluifunctions.backgroundImage.gameObject.SetActive(false);
		SaveDataCampaign saveDataCampaign = null;
		string text = Path.Combine(Application.persistentDataPath, GameDataManager.saveGameFolder);
		text = Path.Combine(text, CampaignManager.campaignReferenceName);
		string[] files = Directory.GetFiles(text);
		if (File.Exists(files[this.indexOfSaveFileToLoad]))
		{
			System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = File.Open(files[this.indexOfSaveFileToLoad], FileMode.Open);
			saveDataCampaign = (SaveDataCampaign)binaryFormatter.Deserialize(fileStream);
			fileStream.Close();
			this.campaignmanager.StartCampaign();
			CampaignManager.campaignReferenceName = saveDataCampaign.campaignReferenceName;
			this.campaignmanager.campaignSaveFileName = saveDataCampaign.saveFileName;
			this.campaignmanager.hoursDurationOfCampaign = saveDataCampaign.hoursDurationOfCampaign;
			this.campaignmanager.julianStartDay = saveDataCampaign.julianStartDate;
			this.campaignmanager.hoursToNextGeneralEvent = saveDataCampaign.hoursSinceLastGeneralEvent;
			this.campaignmanager.generalEventModifier = saveDataCampaign.generalEventModifier;
			this.campaignmanager.lastMissionCompleteStatus = saveDataCampaign.lastMissionCompleteStatus;
			this.campaignmanager.campaignPoints = saveDataCampaign.campaignPoints;
			GameDataManager.playerCommanderName = saveDataCampaign.playerCommanderName;
			this.campaignmanager.playercampaigndata.playerHasMission = saveDataCampaign.playerHasMission;
			this.campaignmanager.playercampaigndata.playerMissionType = saveDataCampaign.playerMissionType;
			this.campaignmanager.playercampaigndata.currentMissionTaskForceID = saveDataCampaign.currentMissionTaskForceID;
			this.campaignmanager.missionGivenOnDate = saveDataCampaign.missionGivenOnDate;
			this.campaignmanager.playercampaigndata.playerPostionOnMap.x = saveDataCampaign.playerPostionOnMapX;
			this.campaignmanager.playercampaigndata.playerPostionOnMap.y = saveDataCampaign.playerPostionOnMapY;
			for (int i = 0; i < UIFunctions.globaluifunctions.database.databaseshipdata.Length; i++)
			{
				if (UIFunctions.globaluifunctions.database.databaseshipdata[i].shipPrefabName == saveDataCampaign.playerVesselClass)
				{
					UIFunctions.globaluifunctions.playerfunctions.playerVesselClass = i;
					break;
				}
			}
			UIFunctions.globaluifunctions.playerfunctions.playerVesselInstance = saveDataCampaign.playerVesselInstance;
			this.campaignmanager.playercampaigndata.secondsCampaignPlayed = saveDataCampaign.secondsCampaignPlayed;
			this.campaignmanager.playercampaigndata.playerRevealed = saveDataCampaign.playerRevealed;
			this.campaignmanager.playercampaigndata.totalTonnage = saveDataCampaign.totalTonnage;
			this.campaignmanager.playercampaigndata.patrolTonnage = saveDataCampaign.patrolTonnage;
			this.campaignmanager.playercampaigndata.patrolPoints = saveDataCampaign.patrolPoints;
			this.campaignmanager.playercampaigndata.campaignStats = saveDataCampaign.campaignStats;
			this.campaignmanager.playercampaigndata.missionsPassed = saveDataCampaign.missionsPassed;
			this.campaignmanager.playercampaigndata.patrolMedals = saveDataCampaign.patrolMedals;
			this.campaignmanager.playercampaigndata.cumulativeMedals = saveDataCampaign.cumulativeMedals;
			this.campaignmanager.playercampaigndata.woundedMedals = saveDataCampaign.woundedMedals;
			this.campaignmanager.playercampaigndata.playerVesselsLost = saveDataCampaign.playerVesselsLost;
			this.campaignmanager.playercampaigndata.playerInstancesLost = saveDataCampaign.playerVesselsLost;
			for (int j = 0; j < this.campaignmanager.campaignregionwaypoints.Length; j++)
			{
				this.campaignmanager.campaignregionwaypoints[j].currentFaction = saveDataCampaign.groundWarZoneCurrentFactions[j];
			}
			this.campaignmanager.thresholdMet = saveDataCampaign.thresholdMet;
			this.campaignmanager.armisticeEventOccurred = saveDataCampaign.armisticeEventOccurred;
			for (int k = 0; k < this.campaignmanager.sosusBarriers.Length; k++)
			{
				this.campaignmanager.sosusBarriers[k].SetActive(saveDataCampaign.sosusStatus[k]);
			}
			this.campaignmanager.playercampaigndata.playerTorpeodesOnBoard = saveDataCampaign.playerTorpeodesOnBoard;
			this.campaignmanager.playercampaigndata.playerVLSTorpeodesOnBoard = saveDataCampaign.playerVLSTorpeodesOnBoard;
			this.campaignmanager.playercampaigndata.playerTubeStatus = saveDataCampaign.playerTubeStatus;
			this.campaignmanager.playercampaigndata.playerWeaponInTube = saveDataCampaign.playerWeaponInTube;
			this.campaignmanager.playercampaigndata.playerSettingsOne = saveDataCampaign.playerSettingsOne;
			this.campaignmanager.playercampaigndata.playerSettingsTwo = saveDataCampaign.playerSettingsTwo;
			this.campaignmanager.playercampaigndata.playerSettingsThree = saveDataCampaign.playerSettingsThree;
			this.campaignmanager.playercampaigndata.playerNoisemakersOnBoard = saveDataCampaign.playerNoisemakersOnBoard;
			this.campaignmanager.playercampaigndata.playerSealTeamOnBoard = saveDataCampaign.playerSealTeamOnBoard;
			this.campaignmanager.playercampaigndata.vesselTotalDamage = saveDataCampaign.vesselTotalDamage;
			for (int l = 0; l < saveDataCampaign.compartmentCurrentFlooding.Length; l++)
			{
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentCurrentFlooding[l] = saveDataCampaign.compartmentCurrentFlooding[l];
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentTotalFlooding[l] = saveDataCampaign.compartmentTotalFlooding[l];
			}
			this.campaignmanager.playercampaigndata.decalNames = saveDataCampaign.decalNames;
			for (int m = 0; m < saveDataCampaign.damageControlCurrentTimers.Length; m++)
			{
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlCurrentTimers[m] = saveDataCampaign.damageControlCurrentTimers[m];
			}
			UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.destroyedSubsystems = new bool[UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlCurrentTimers.Length];
			for (int n = 0; n < UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlCurrentTimers.Length; n++)
			{
				if (UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlCurrentTimers[n] == 10000f)
				{
					UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.destroyedSubsystems[n] = true;
				}
			}
			this.campaignmanager.playerGameObject.transform.localPosition = new Vector3(this.campaignmanager.playercampaigndata.playerPostionOnMap.x, this.campaignmanager.playercampaigndata.playerPostionOnMap.y, this.campaignmanager.playerGameObject.transform.localPosition.z);
			for (int num = 0; num < this.campaignmanager.campaignAircraftObjects.Length; num++)
			{
				if (this.campaignmanager.campaignAircraftObjects[num] != null)
				{
					this.campaignmanager.campaignAircraftObjects[num].SetActive(saveDataCampaign.aircraftActive[num]);
					this.campaignmanager.campaignAircraftObjects[num].transform.localPosition = new Vector3(saveDataCampaign.aircraftCurrentPositions[num, 0], saveDataCampaign.aircraftCurrentPositions[num, 1], 0f);
					this.campaignmanager.campaignAircraftObjects[num].transform.localRotation = Quaternion.Slerp(this.campaignmanager.campaignAircraftObjects[num].transform.localRotation, Quaternion.Euler(saveDataCampaign.aircraftCurrentPositions[num, 2], saveDataCampaign.aircraftCurrentPositions[num, 3], 0f), 1f);
				}
			}
			for (int num2 = 0; num2 < this.campaignmanager.campaignlocations.Length; num2++)
			{
				this.campaignmanager.campaignlocations[num2].faction = saveDataCampaign.factions[num2];
				this.campaignmanager.campaignlocations[num2].aircraftTimer = saveDataCampaign.aircratTimers[num2];
				this.campaignmanager.campaignlocations[num2].currentWaypoint = saveDataCampaign.aircraftCurrentWaypoint[num2];
				this.campaignmanager.aircraftPrepTime[num2] = saveDataCampaign.aircraftPrepTimes[num2];
				this.campaignmanager.campaignlocations[num2].onPatrol = saveDataCampaign.onPatrol[num2];
				if (this.campaignmanager.campaignAircraftObjects[num2] != null)
				{
					this.campaignmanager.campaignAircraftObjects[num2].SetActive(saveDataCampaign.aircraftActive[num2]);
					if (this.campaignmanager.campaignAircraftObjects[num2].activeSelf)
					{
						this.campaignmanager.campaignlocations[num2].aircraftWaypoints = new Vector3[2];
						this.campaignmanager.campaignlocations[num2].aircraftWaypoints[0].x = saveDataCampaign.aircraftWaypoints0[num2, 0];
						this.campaignmanager.campaignlocations[num2].aircraftWaypoints[0].y = saveDataCampaign.aircraftWaypoints0[num2, 1];
						this.campaignmanager.campaignlocations[num2].aircraftWaypoints[0].z = saveDataCampaign.aircraftWaypoints0[num2, 2];
						this.campaignmanager.campaignlocations[num2].aircraftWaypoints[1].x = saveDataCampaign.aircraftWaypoints1[num2, 0];
						this.campaignmanager.campaignlocations[num2].aircraftWaypoints[1].y = saveDataCampaign.aircraftWaypoints1[num2, 1];
						this.campaignmanager.campaignlocations[num2].aircraftWaypoints[1].z = saveDataCampaign.aircraftWaypoints1[num2, 2];
					}
				}
			}
			for (int num3 = 0; num3 < this.campaignmanager.campaignSatelliteObjects.Length; num3++)
			{
				this.campaignmanager.satelliteTimers[num3] = saveDataCampaign.satelliteTimers[num3];
				this.campaignmanager.currentSatelliteAngle[num3] = saveDataCampaign.satelliteCurrentAngles[num3];
				this.campaignmanager.campaignSatelliteObjects[num3].transform.localPosition = new Vector3(saveDataCampaign.satelliteCurrentPositions[num3, 0], saveDataCampaign.satelliteCurrentPositions[num3, 1], 0f);
				this.campaignmanager.campaignSatelliteObjects[num3].transform.localRotation = Quaternion.Slerp(this.campaignmanager.campaignSatelliteObjects[num3].transform.localRotation, Quaternion.Euler(saveDataCampaign.satelliteCurrentPositions[num3, 2], saveDataCampaign.satelliteCurrentPositions[num3, 3], 0f), 1f);
				this.campaignmanager.satelliteWaypoints[0] = new Vector3(saveDataCampaign.satelliteWaypoints0[num3, 0], saveDataCampaign.satelliteWaypoints0[num3, 1], saveDataCampaign.satelliteWaypoints0[num3, 2]);
				this.campaignmanager.satelliteWaypoints[1] = new Vector3(saveDataCampaign.satelliteWaypoints1[num3, 0], saveDataCampaign.satelliteWaypoints1[num3, 1], saveDataCampaign.satelliteWaypoints1[num3, 2]);
				this.campaignmanager.satelliteReversed[num3] = saveDataCampaign.satellitesReversed[num3];
				this.campaignmanager.satelliteMoving[num3] = saveDataCampaign.satellitesMoving[num3];
				this.campaignmanager.campaignSatelliteObjects[num3].SetActive(saveDataCampaign.satellitesActive[num3]);
			}
			this.campaignmanager.activeTaskForces = saveDataCampaign.activeTaskForces;
			for (int num4 = 0; num4 < this.campaignmanager.campaignTaskForces.Length; num4++)
			{
				this.campaignmanager.campaignTaskForces[num4].missionType = saveDataCampaign.missionType[num4];
				this.campaignmanager.campaignTaskForces[num4].actualMission = saveDataCampaign.actualMission[num4];
				this.campaignmanager.campaignTaskForces[num4].missionID = saveDataCampaign.missionID[num4];
				this.campaignmanager.campaignTaskForces[num4].speed = saveDataCampaign.speed[num4];
				this.campaignmanager.campaignTaskForces[num4].previousWaypoint = saveDataCampaign.prevWaypoint[num4];
				this.campaignmanager.campaignTaskForces[num4].currentWaypoint = saveDataCampaign.currentWaypoint[num4];
				this.campaignmanager.campaignTaskForces[num4].legOfRoute = saveDataCampaign.legOfRoute[num4];
				this.campaignmanager.taskForceActivateTimers[num4] = saveDataCampaign.missionStartTimers[num4];
				this.campaignmanager.campaignTaskForces[num4].currTile = saveDataCampaign.currTile[num4];
				this.campaignmanager.campaignTaskForces[num4].destTile = saveDataCampaign.destTile[num4];
				this.campaignmanager.campaignTaskForces[num4].nextTile = saveDataCampaign.nextTile[num4];
				this.campaignmanager.campaignTaskForces[num4].takeDirectRoute = saveDataCampaign.takeDirectRoute[num4];
				this.campaignmanager.campaignTaskForces[num4].startIsLocation = saveDataCampaign.startIsLocation[num4];
				this.campaignmanager.campaignTaskForces[num4].endIsLocation = saveDataCampaign.endIsLocation[num4];
				this.campaignmanager.campaignTaskForces[num4].finalLegDone = saveDataCampaign.finalLegDone[num4];
				this.campaignmanager.campaignTaskForces[num4].startPositionID = saveDataCampaign.startPosID[num4];
				this.campaignmanager.campaignTaskForces[num4].endLocationID = saveDataCampaign.endLocationID[num4];
				this.campaignmanager.campaignTaskForces[num4].currentAction = saveDataCampaign.currentAction[num4];
				this.campaignmanager.campaignTaskForces[num4].taskForceType = saveDataCampaign.taskForceType[num4];
				this.campaignmanager.campaignTaskForces[num4].behaviourType = saveDataCampaign.behaviourType[num4];
				this.campaignmanager.campaignTaskForces[num4].patrolForHours = saveDataCampaign.patrolForHours[num4];
				this.campaignmanager.campaignTaskForces[num4].waypointPosition = new Vector3(saveDataCampaign.waypointPosition[num4, 0], saveDataCampaign.waypointPosition[num4, 1], saveDataCampaign.waypointPosition[num4, 2]);
				this.campaignmanager.campaignTaskForces[num4].finalLocationPosition = new Vector2(saveDataCampaign.finalLocationPosition[num4, 0], saveDataCampaign.finalLocationPosition[num4, 1]);
				this.campaignmanager.campaignTaskForceObjects[num4].SetActive(this.campaignmanager.activeTaskForces[num4]);
				this.campaignmanager.campaignTaskForceObjects[num4].transform.localPosition = new Vector3(saveDataCampaign.taskForceCurrentPositions[num4, 0], saveDataCampaign.taskForceCurrentPositions[num4, 1], this.campaignmanager.campaignTaskForceObjects[num4].transform.localPosition.z);
				if (this.campaignmanager.campaignmissions[this.campaignmanager.campaignTaskForces[num4].actualMission].missionType == "RETURN_TO_BASE")
				{
					this.campaignmanager.campaignTaskForceIcons[num4].sprite = null;
				}
				else
				{
					this.campaignmanager.campaignTaskForceIcons[num4].sprite = this.campaignmanager.reconImages[this.campaignmanager.campaignTaskForces[num4].taskForceType];
				}
				CampaignMapRevealContact component = this.campaignmanager.campaignTaskForceObjects[num4].GetComponent<CampaignMapRevealContact>();
				component.timer = saveDataCampaign.revealContactTimers[num4];
				if (component.timer > 0f)
				{
					component.enabled = true;
					component.FixedUpdate();
					component.iconImage.transform.localRotation = Quaternion.identity;
					component.iconImage.gameObject.SetActive(true);
					if (component.timer >= 3f)
					{
						component.lastKnownPosition = new Vector2(saveDataCampaign.lastRevealContactPositionx[num4], saveDataCampaign.lastRevealContactPositiony[num4]);
						component.iconImage.gameObject.transform.position = new Vector3(component.lastKnownPosition.x, component.lastKnownPosition.y, component.transform.position.z);
						component.iconImage.gameObject.transform.SetParent(UIFunctions.globaluifunctions.campaignmanager.shipLayer, true);
					}
				}
				else
				{
					component.iconImage.gameObject.SetActive(false);
				}
				if (this.campaignmanager.campaignTaskForces[num4].currentAction == 5)
				{
					this.campaignmanager.campaignTaskForces[num4].pathAStar = new Path_AStar(this.campaignmanager.campaignTaskForces[num4].currTile, this.campaignmanager.campaignTaskForces[num4].destTile);
				}
				this.campaignmanager.campaignTaskForces[num4].mustUseWaypoints = this.GetIntArrayFromAllData(num4, saveDataCampaign.allMustUseWaypoints, saveDataCampaign.mustUseWaypointTaskForce);
				this.campaignmanager.campaignTaskForces[num4].prohibitedWaypoints = this.GetIntArrayFromAllData(num4, saveDataCampaign.allProhibitedWaypoints, saveDataCampaign.prohibitedWaypointTaskForce);
				this.campaignmanager.campaignTaskForces[num4].shipNumbers = this.GetIntArrayFromAllData(num4, saveDataCampaign.allShipNumbers, saveDataCampaign.shipNumberTaskForces);
				this.campaignmanager.campaignTaskForces[num4].shipClasses = this.GetStringArrayFromAllData(num4, saveDataCampaign.allShipClasses, saveDataCampaign.allShipsTaskForces);
				this.campaignmanager.campaignTaskForces[num4].missionCriticalVessels = this.GetBoolArrayFromAllData(num4, saveDataCampaign.allMissionCritVessels, saveDataCampaign.allShipsTaskForces);
				this.campaignmanager.campaignTaskForces[num4].defensiveVessels = this.GetBoolArrayFromAllData(num4, saveDataCampaign.allMissionDefensiveVessels, saveDataCampaign.allShipsTaskForces);
			}
		}
		if (this.campaignmanager.useLandWar)
		{
			for (int num5 = 0; num5 < this.campaignmanager.campaignregionwaypoints.Length; num5++)
			{
				this.campaignmanager.SetTerritoryState(num5, this.campaignmanager.campaignregionwaypoints[num5].currentFaction);
			}
		}
		this.campaignmanager.SetCalendarData();
		this.campaignmanager.playerInPort = false;
		this.campaignmanager.OpenMissionBriefing(0);
		this.campaignmanager.BringInDisabledCampaignManager();
		this.campaignmanager.playerInPort = saveDataCampaign.playerInPort;
		this.campaignmanager.timeInPort = 0f;
		this.campaignmanager.timeInPortTimer = 0f;
		UIFunctions.globaluifunctions.campaignmanager.strategicMapToolbar.SetActive(false);
		AudioManager.audiomanager.PlayMusicClip(1, string.Empty);
		Time.timeScale = 1f;
		this.campaignmanager.enabled = false;
		UIFunctions.globaluifunctions.missionmanager.ContinueCourse();
	}

	// Token: 0x060009C0 RID: 2496 RVA: 0x00072DB0 File Offset: 0x00070FB0
	public void LoadCampaign()
	{
		if (this.allPathsInDirectory.Length == 0)
		{
			return;
		}
		UIFunctions.globaluifunctions.loadingMask.gameObject.SetActive(true);
		UIFunctions.globaluifunctions.missionmanager.loadTheCampaign = true;
		Time.timeScale = 1f;
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x00072DF0 File Offset: 0x00070FF0
	public static string StripSpecialCharsFromString(string inputString)
	{
		inputString = inputString.Replace("/", string.Empty);
		inputString = inputString.Replace("\\", string.Empty);
		return inputString;
	}

	// Token: 0x060009C2 RID: 2498 RVA: 0x00072E18 File Offset: 0x00071018
	public void BringInCampaignFileNameEntry()
	{
		this.BuildCampaignSaveGamesMenu(true);
		this.fileMenuObjects[0].SetActive(false);
		this.fileMenuObjects[1].SetActive(true);
		this.fileMenuObjects[2].SetActive(false);
		this.fileMenuObjects[3].SetActive(false);
		this.fileMenuObjects[4].SetActive(false);
		string[] array = UIFunctions.globaluifunctions.textparser.OpenTextDataFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("language/campaign/campaigns"));
		string[] array2 = CampaignManager.campaignReferenceName.Split(new char[]
		{
			'n'
		});
		string str = array[int.Parse(array2[1]) - 1].Trim();
		this.saveFileNameInputField.text = str + "SAVE";
		this.commanderNameInputField.text = GameDataManager.playerCommanderName;
	}

	// Token: 0x060009C3 RID: 2499 RVA: 0x00072EE4 File Offset: 0x000710E4
	public void BackToNewCampaign()
	{
		this.fileMenuObjects[0].SetActive(true);
		this.fileMenuObjects[1].SetActive(false);
		this.fileMenuObjects[2].SetActive(false);
		this.fileMenuObjects[3].SetActive(false);
		this.fileMenuObjects[4].SetActive(false);
		UIFunctions.globaluifunctions.ClearSelectionToggleBars();
		UIFunctions.globaluifunctions.campaignmanager.BringInCampaignSelection();
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x00072F50 File Offset: 0x00071150
	public void BuildCampaignSaveGamesMenu(bool newCampaign)
	{
		UIFunctions.globaluifunctions.ClearSelectionToggleBars();
		UIFunctions.globaluifunctions.scrollbarDefault.gameObject.SetActive(false);
		UIFunctions.globaluifunctions.selectionGroupPanel.SetActive(true);
		if (newCampaign)
		{
			UIFunctions.globaluifunctions.mainTitle.text = LanguageManager.interfaceDictionary["NewCampaignHeader"];
		}
		else
		{
			UIFunctions.globaluifunctions.mainTitle.text = LanguageManager.interfaceDictionary["LoadCampaignHeader"];
		}
		this.fileMenuObjects[0].SetActive(false);
		this.fileMenuObjects[1].SetActive(false);
		this.fileMenuObjects[2].SetActive(false);
		this.fileMenuObjects[3].SetActive(true);
		this.fileMenuObjects[4].SetActive(false);
		string text = Path.Combine(Application.persistentDataPath, GameDataManager.saveGameFolder);
		text = Path.Combine(text, CampaignManager.campaignReferenceName);
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		this.allPathsInDirectory = Directory.GetFiles(text);
		UIFunctions.globaluifunctions.selectionBars = new GameObject[this.allPathsInDirectory.Length];
		for (int i = 0; i < this.allPathsInDirectory.Length; i++)
		{
			string text2 = Path.GetFileName(this.allPathsInDirectory[i]);
			if (text2.Contains(GameDataManager.saveGameFileExtension))
			{
				text2 = text2.Replace(GameDataManager.saveGameFileExtension, string.Empty);
				GameObject gameObject = UnityEngine.Object.Instantiate(this.campaignmanager.savegameBar, UIFunctions.globaluifunctions.selectionToggleGroup.transform.position, UIFunctions.globaluifunctions.selectionToggleGroup.transform.rotation) as GameObject;
				gameObject.transform.SetParent(UIFunctions.globaluifunctions.selectionToggleGroup.transform, false);
				gameObject.transform.localPosition = Vector2.zero;
				UIFunctions.globaluifunctions.selectionBars[i] = gameObject;
				Text componentInChildren = gameObject.GetComponentInChildren<Text>();
				componentInChildren.text = text2;
				Toggle toggle = gameObject.GetComponent<Toggle>();
				toggle.name = i.ToString();
				toggle.group = UIFunctions.globaluifunctions.selectionToggleGroup;
				toggle.onValueChanged.AddListener(delegate(bool value)
				{
					this.SetSaveFileIndexToLoad(int.Parse(toggle.name));
				});
				if (!newCampaign && i == 0)
				{
					toggle.isOn = true;
				}
			}
		}
		RectTransform component = UIFunctions.globaluifunctions.selectionToggleGroup.gameObject.GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(component.sizeDelta.x, (float)UIFunctions.globaluifunctions.selectionBars.Length * GameDataManager.menuScrollListSpacing + 10f);
	}

	// Token: 0x060009C5 RID: 2501 RVA: 0x00073204 File Offset: 0x00071404
	public void SetSaveFileIndexToLoad(int number)
	{
		this.indexOfSaveFileToLoad = number;
		if (this.fileMenuObjects[1].activeSelf)
		{
			this.saveFileNameInputField.text = UIFunctions.globaluifunctions.selectionBars[number].GetComponentInChildren<Text>().text;
		}
	}

	// Token: 0x060009C6 RID: 2502 RVA: 0x0007324C File Offset: 0x0007144C
	public void WarnFileAlreadyExists()
	{
		this.fileMenuObjects[2].SetActive(true);
	}

	// Token: 0x060009C7 RID: 2503 RVA: 0x0007325C File Offset: 0x0007145C
	public void WarnOfFileDeletion()
	{
		if (this.allPathsInDirectory.Length == 0)
		{
			return;
		}
		this.fileMenuObjects[4].SetActive(true);
		string text = Path.GetFileName(this.allPathsInDirectory[this.indexOfSaveFileToLoad]);
		text = text.Replace(GameDataManager.saveGameFileExtension, string.Empty);
		this.deleteFileWarning.text = LanguageManager.interfaceDictionary[this.deleteFileWarning.name];
		this.deleteFileWarning.text = this.deleteFileWarning.text.Replace("<FILENAME>", text);
	}

	// Token: 0x060009C8 RID: 2504 RVA: 0x000732EC File Offset: 0x000714EC
	public void DeleteSaveFile()
	{
		this.fileMenuObjects[4].SetActive(false);
		string text = Path.Combine(Application.persistentDataPath, GameDataManager.saveGameFolder);
		text = Path.Combine(text, CampaignManager.campaignReferenceName);
		string[] files = Directory.GetFiles(text);
		File.Delete(files[this.indexOfSaveFileToLoad]);
		for (int i = 0; i < UIFunctions.globaluifunctions.selectionBars.Length; i++)
		{
			if (UIFunctions.globaluifunctions.selectionBars[i] != null)
			{
				UnityEngine.Object.Destroy(UIFunctions.globaluifunctions.selectionBars[i]);
			}
		}
		this.BuildCampaignSaveGamesMenu(false);
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x00073384 File Offset: 0x00071584
	public void DismissMenuWarning(int i)
	{
		this.fileMenuObjects[i].SetActive(false);
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x00073394 File Offset: 0x00071594
	private int[] GetIntArrayFromAllData(int index, int[] allArray, int[] indexArray)
	{
		int num = 0;
		for (int i = 0; i < indexArray.Length; i++)
		{
			if (indexArray[i] == index)
			{
				num++;
			}
		}
		int[] array = new int[num];
		num = 0;
		for (int j = 0; j < allArray.Length; j++)
		{
			if (indexArray[j] == index)
			{
				array[num] = allArray[j];
				num++;
			}
		}
		return array;
	}

	// Token: 0x060009CB RID: 2507 RVA: 0x000733F8 File Offset: 0x000715F8
	private string[] GetStringArrayFromAllData(int index, string[] allArray, int[] indexArray)
	{
		int num = 0;
		for (int i = 0; i < indexArray.Length; i++)
		{
			if (indexArray[i] == index)
			{
				num++;
			}
		}
		string[] array = new string[num];
		num = 0;
		for (int j = 0; j < allArray.Length; j++)
		{
			if (indexArray[j] == index)
			{
				array[num] = allArray[j];
				num++;
			}
		}
		return array;
	}

	// Token: 0x060009CC RID: 2508 RVA: 0x0007345C File Offset: 0x0007165C
	private bool[] GetBoolArrayFromAllData(int index, bool[] allArray, int[] indexArray)
	{
		int num = 0;
		for (int i = 0; i < indexArray.Length; i++)
		{
			if (indexArray[i] == index)
			{
				num++;
			}
		}
		bool[] array = new bool[num];
		num = 0;
		for (int j = 0; j < allArray.Length; j++)
		{
			if (indexArray[j] == index)
			{
				array[num] = allArray[j];
				num++;
			}
		}
		return array;
	}

	// Token: 0x04000E96 RID: 3734
	public CampaignManager campaignmanager;

	// Token: 0x04000E97 RID: 3735
	public GameObject[] fileMenuObjects;

	// Token: 0x04000E98 RID: 3736
	public InputField saveFileNameInputField;

	// Token: 0x04000E99 RID: 3737
	public InputField commanderNameInputField;

	// Token: 0x04000E9A RID: 3738
	public Text deleteFileWarning;

	// Token: 0x04000E9B RID: 3739
	public int indexOfSaveFileToLoad;

	// Token: 0x04000E9C RID: 3740
	public string[] allPathsInDirectory;
}
