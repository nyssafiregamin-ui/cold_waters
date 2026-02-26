using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000F1 RID: 241
public class CampaignManager : MonoBehaviour
{
	// Token: 0x0600065D RID: 1629 RVA: 0x0002DA04 File Offset: 0x0002BC04
	public void BringInCampaignSelection()
	{
		UIFunctions.globaluifunctions.SetMenuSystem("CAMPAIGNSELECTION");
		UIFunctions.globaluifunctions.mainTitle.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "NewCampaignHeader");
		this.BuildCampaignMenu(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("language/campaign/campaigns"));
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x0002DA58 File Offset: 0x0002BC58
	public void BringInExitMenu()
	{
		Time.timeScale = 0f;
		UIFunctions.globaluifunctions.campaignmanager.strategicMapToolbar.SetActive(false);
		UIFunctions.globaluifunctions.SetMenuSystem("CAMPAIGNEXIT");
		UIFunctions.globaluifunctions.campaignmanager.BringInDisabledCampaignManager();
		UIFunctions.globaluifunctions.backgroundTemplate.SetActive(false);
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x0002DAB4 File Offset: 0x0002BCB4
	public void ToolbarLabels()
	{
		this.labelsLayer.SetActive(!this.labelsLayer.activeSelf);
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x0002DAD0 File Offset: 0x0002BCD0
	public void DismissExitMenu()
	{
		Time.timeScale = this.mapTimeCompression;
		base.enabled = true;
		this.campaignExitMenu.gameObject.SetActive(false);
		UIFunctions.globaluifunctions.campaignmanager.strategicMapToolbar.SetActive(true);
	}

	// Token: 0x06000661 RID: 1633 RVA: 0x0002DB18 File Offset: 0x0002BD18
	public void ExitCampaign()
	{
		if (GameDataManager.optionsBoolSettings[11] || !GameDataManager.optionsBoolSettings[12])
		{
			SaveLoadManager.AutoSaveCampaign();
		}
		UIFunctions.globaluifunctions.missionmanager.BackToMain();
	}

	// Token: 0x06000662 RID: 1634 RVA: 0x0002DB54 File Offset: 0x0002BD54
	public void CheckSaveAlreadyExists(bool overwrite)
	{
		this.campaignSaveFileName = UIFunctions.globaluifunctions.database.saveloadmanager.saveFileNameInputField.text;
		GameDataManager.playerCommanderName = UIFunctions.globaluifunctions.database.saveloadmanager.commanderNameInputField.text;
		if (this.campaignSaveFileName.Length < 1)
		{
			return;
		}
		string text = Path.Combine(Application.persistentDataPath, UIFunctions.globaluifunctions.textparser.GetFilePathFromString(GameDataManager.saveGameFolder + "/" + CampaignManager.campaignReferenceName));
		text = Path.Combine(text, this.campaignSaveFileName + GameDataManager.saveGameFileExtension);
		if (!overwrite && File.Exists(text))
		{
			UIFunctions.globaluifunctions.database.saveloadmanager.WarnFileAlreadyExists();
			return;
		}
		UIFunctions.globaluifunctions.missionmanager.SelectShip();
	}

	// Token: 0x06000663 RID: 1635 RVA: 0x0002DC2C File Offset: 0x0002BE2C
	public void StartNewCampaign(bool overwrite)
	{
		this.campaignSaveFileName = UIFunctions.globaluifunctions.database.saveloadmanager.saveFileNameInputField.text;
		if (this.campaignSaveFileName.Length < 1)
		{
			return;
		}
		string text = Path.Combine(Application.persistentDataPath, GameDataManager.saveGameFolder);
		text = Path.Combine(text, this.campaignSaveFileName + GameDataManager.saveGameFileExtension);
		if (!overwrite && File.Exists(text))
		{
			UIFunctions.globaluifunctions.database.saveloadmanager.WarnFileAlreadyExists();
			return;
		}
		UIFunctions.globaluifunctions.backgroundImage.gameObject.SetActive(false);
		UIFunctions.globaluifunctions.selectionGroupPanel.SetActive(false);
		this.StartCampaign();
		this.eventManager.BringInEvent(this.eventManager.specialEventIDs[0], false, false);
		int num = (int)OptionsManager.difficultySettings["NumberOfASW"];
		for (int i = 0; i < num; i++)
		{
			this.GenerateCampaignMission(false);
		}
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x0002DD28 File Offset: 0x0002BF28
	public void StartCampaign()
	{
		UIFunctions.globaluifunctions.textparser.ReadAwardData();
		GameDataManager.missionMode = false;
		GameDataManager.trainingMode = false;
		this.eventManager.nextEvent = -1;
		this.eventManager.currentEvent = -1;
		this.playercampaigndata = ScriptableObject.CreateInstance<PlayerCampaignData>();
		this.playercampaigndata.patrolMedals = new int[this.eventManager.patrolAwards.Length];
		this.playercampaigndata.cumulativeMedals = new int[this.eventManager.cumulativeAwards.Length];
		this.playercampaigndata.woundedMedals = new int[this.eventManager.woundedAwards.Length];
		this.playercampaigndata.playerHasMission = false;
		UIFunctions.globaluifunctions.SetMenuSystem("CAMPAIGN");
		UIFunctions.globaluifunctions.backgroundTemplate.SetActive(false);
		this.InitialiseCampaign();
		this.GenerateCampaignDataFiles();
		string text = string.Concat(new string[]
		{
			"<b>  ",
			Mathf.RoundToInt(this.commandoLoadTime * OptionsManager.difficultySettings["RestockTimeModifier"]).ToString(),
			" ",
			LanguageManager.interfaceDictionary["Minutes"],
			"</b>"
		});
		UIFunctions.globaluifunctions.portRearm.sealTeamTimeText.text = text;
		this.tileGraph = new Path_TileGraph();
		for (int i = 0; i < this.campaignlocations.Length; i++)
		{
			Vector2 mapCoords = this.GetMapCoords(this.campaignlocations[i].baseLocation.x * 0.2567394f, this.campaignlocations[i].baseLocation.y * 0.2567394f, 256, 128);
			string key = Mathf.FloorToInt(mapCoords.x) + "," + Mathf.FloorToInt(mapCoords.y);
			if (!this.tileGraph.nodes.ContainsKey(key))
			{
				Debug.Log(this.campaignlocations[i].locationName + " is not in the NAV MAP as walkable");
			}
		}
		for (int j = 0; j < this.campaignmapwaypoints.Length; j++)
		{
			Vector2 mapCoords2 = this.GetMapCoords(this.campaignmapwaypoints[j].waypointPosition.x * 0.2567394f, this.campaignmapwaypoints[j].waypointPosition.y * 0.2567394f, 256, 128);
			string key2 = Mathf.FloorToInt(mapCoords2.x) + "," + Mathf.FloorToInt(mapCoords2.y);
			if (!this.tileGraph.nodes.ContainsKey(key2))
			{
				Debug.Log(this.campaignmapwaypoints[j].waypointName + " is not in the NAV MAP as walkable");
			}
		}
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x0002DFFC File Offset: 0x0002C1FC
	private void GenerateCampaignDataFiles()
	{
		string[] array = new string[100];
		int num = 0;
		for (int i = 0; i < this.ports.Length; i++)
		{
			array[num] = string.Concat(new string[]
			{
				this.ports[i].name,
				"=",
				this.ports[i].localPosition.x.ToString("0.00"),
				",",
				this.ports[i].localPosition.y.ToString("0.00")
			});
			num++;
		}
		for (int j = 0; j < this.sosus.Length; j++)
		{
			array[num] = string.Concat(new string[]
			{
				this.sosus[j].name,
				"=",
				this.sosus[j].localPosition.x.ToString("0.00"),
				",",
				this.sosus[j].localPosition.y.ToString("0.00")
			});
			num++;
		}
		for (int k = 0; k < this.waypoints.Length; k++)
		{
			array[num] = string.Concat(new object[]
			{
				this.waypoints[k].name,
				"=",
				this.waypoints[k].localPosition.x.ToString("0.00"),
				",",
				this.waypoints[k].localPosition.y.ToString("0.00"),
				",",
				this.waypoints[k].localScale.x
			});
			num++;
		}
		File.WriteAllLines(Application.persistentDataPath + "/testdata.dat", array);
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x0002E20C File Offset: 0x0002C40C
	private void BuildCampaignMenu(string filename)
	{
		string[] array = UIFunctions.globaluifunctions.textparser.OpenTextDataFile(filename);
		UIFunctions.globaluifunctions.scrollbarDefault.gameObject.SetActive(false);
		UIFunctions.globaluifunctions.selectionGroupPanel.SetActive(true);
		UIFunctions.globaluifunctions.selectionBars = new GameObject[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(this.savegameBar, this.missionListToggles.transform.position, Quaternion.identity) as GameObject;
			UIFunctions.globaluifunctions.selectionBars[i] = gameObject;
			gameObject.transform.SetParent(UIFunctions.globaluifunctions.selectionToggleGroup.transform, false);
			gameObject.transform.localPosition = Vector2.zero;
			Text componentInChildren = gameObject.GetComponentInChildren<Text>();
			componentInChildren.text = (i + 1).ToString() + ". " + array[i];
			Toggle toggle = gameObject.GetComponent<Toggle>();
			toggle.name = i.ToString();
			toggle.group = this.missionListToggles;
			toggle.onValueChanged.AddListener(delegate(bool value)
			{
				this.SetCampaignPrefab(int.Parse(toggle.name));
			});
			if (i == 0)
			{
				toggle.isOn = true;
				this.SetCampaignPrefab(i);
			}
		}
		UIFunctions.globaluifunctions.database.saveloadmanager.fileMenuObjects[0].SetActive(true);
		UIFunctions.globaluifunctions.database.saveloadmanager.fileMenuObjects[1].SetActive(false);
		UIFunctions.globaluifunctions.database.saveloadmanager.fileMenuObjects[2].SetActive(false);
		UIFunctions.globaluifunctions.database.saveloadmanager.fileMenuObjects[3].SetActive(false);
		RectTransform component = UIFunctions.globaluifunctions.selectionToggleGroup.gameObject.GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(component.sizeDelta.x, (float)UIFunctions.globaluifunctions.selectionBars.Length * GameDataManager.menuScrollListSpacing + 10f);
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x0002E434 File Offset: 0x0002C634
	public void SetCampaignPrefab(int number)
	{
		CampaignManager.campaignReferenceName = "campaign" + (number + 1).ToString("000");
		string filePathFromString = UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/summary");
		string[] array = UIFunctions.globaluifunctions.textparser.OpenTextDataFile(filePathFromString);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Trim().Split(new char[]
			{
				'='
			});
			string text = array2[0];
			switch (text)
			{
			case "Image":
				UIFunctions.globaluifunctions.textparser.SetImageSprite(UIFunctions.globaluifunctions.textparser.GetFilePathFromString(array2[1].Trim()), UIFunctions.globaluifunctions.backgroundImage);
				UIFunctions.globaluifunctions.backgroundImage.gameObject.SetActive(true);
				break;
			case "PlayerVessels":
			{
				string[] array3 = UIFunctions.globaluifunctions.textparser.PopulateStringArray(array2[1].Trim());
				UIFunctions.globaluifunctions.playerfunctions.playerVesselList = new int[array3.Length];
				for (int j = 0; j < array3.Length; j++)
				{
					UIFunctions.globaluifunctions.playerfunctions.playerVesselList[j] = UIFunctions.globaluifunctions.textparser.GetShipID(array3[j]);
				}
				break;
			}
			case "CommanderName":
				this.commanderName = array2[1].Trim();
				break;
			case "CommanderFleetName":
				this.commanderFleetName = array2[1].Trim();
				break;
			}
		}
		UIFunctions.globaluifunctions.selectionGroupText.text = UIFunctions.globaluifunctions.textparser.OpenTextDataFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/description"))[0];
		UIFunctions.globaluifunctions.selectionGroupText.text = UIFunctions.globaluifunctions.selectionGroupText.text.Replace("\\n", "\n");
	}

	// Token: 0x06000668 RID: 1640 RVA: 0x0002E6A0 File Offset: 0x0002C8A0
	public void CheckMissionEnded(int currentTaskforce, string condition)
	{
		if (condition == "STATIONARY")
		{
			this.CampaignEngagement(currentTaskforce);
			return;
		}
		bool flag = false;
		if (this.campaignmissions[this.campaignTaskForces[currentTaskforce].missionID].missionEndsWhen == condition)
		{
			this.DeactivateTaskforce(currentTaskforce);
			if (currentTaskforce == this.playercampaigndata.currentMissionTaskForceID)
			{
				flag = true;
			}
			else
			{
				this.GenerateCampaignMission(false);
			}
		}
		if (flag && this.campaignmissions[this.campaignTaskForces[currentTaskforce].missionID].missionEndsWhen == condition)
		{
			this.campaignPoints += this.campaignmissions[this.campaignTaskForces[currentTaskforce].missionID].strategicValue * OptionsManager.difficultySettings["FailMissionModifier"];
			this.playercampaigndata.patrolPoints += this.campaignmissions[this.campaignTaskForces[currentTaskforce].missionID].strategicValue * OptionsManager.difficultySettings["FailMissionModifier"];
			this.eventManager.missionMissed = true;
			if (this.campaignmissions[this.campaignTaskForces[currentTaskforce].missionID].disruptOnFail)
			{
				this.DisruptLocation(this.campaignTaskForces[currentTaskforce].endLocationID);
			}
			if (this.campaignmissions[this.campaignTaskForces[currentTaskforce].missionID].invadeOnFail)
			{
				this.InvadeLocation(this.campaignTaskForces[currentTaskforce].endLocationID);
			}
			this.currentTaskForceEngagedWith = currentTaskforce;
			UIFunctions.globaluifunctions.playerfunctions.statusscreens.OpenStatusScreens(3);
		}
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x0002E838 File Offset: 0x0002CA38
	public bool CheckStoresForMissionWeapons(int weaponRequired, int numberRequired, bool subtractStores)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		bool flag = false;
		if (GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.hasVLS)
		{
			for (int i = 0; i < GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.vlsTorpedoTypes.Length; i++)
			{
				if (GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.vlsTorpedoTypes[i] == weaponRequired)
				{
					num2 = GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.vlsCurrentTorpsOnBoard[i];
					num3 = i;
					if (num2 >= numberRequired)
					{
						if (subtractStores)
						{
							GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.vlsCurrentTorpsOnBoard[i] -= numberRequired;
						}
						flag = true;
					}
				}
			}
		}
		if (!flag)
		{
			for (int j = 0; j < GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.torpedoTypes.Length; j++)
			{
				if (GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.torpedoTypes[j] == weaponRequired)
				{
					num = GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.currentTorpsOnBoard[j];
					num4 = j;
					if (num >= numberRequired)
					{
						if (subtractStores)
						{
							GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.currentTorpsOnBoard[j] -= numberRequired;
						}
						flag = true;
					}
				}
			}
		}
		if (!flag && num2 + num >= numberRequired)
		{
			if (subtractStores)
			{
				GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.currentTorpsOnBoard[num4] = 0;
				GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.vlsCurrentTorpsOnBoard[num3] -= numberRequired - num;
			}
			flag = true;
		}
		return flag;
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x0002EA00 File Offset: 0x0002CC00
	private void DisruptLocation(int locationIndex)
	{
		if (this.campaignlocations[locationIndex].hasAirbase)
		{
			this.campaignlocations[locationIndex].aircraftTimer = -UnityEngine.Random.Range(this.disruptTimes.x, this.disruptTimes.y);
			this.campaignlocations[locationIndex].currentWaypoint = 0;
			this.campaignlocations[locationIndex].onPatrol = false;
			this.campaignAircraftObjects[locationIndex].SetActive(false);
		}
		if (this.GetHasLocationFunction(locationIndex, "SOSUS_NODE"))
		{
			this.DisruptSosus(locationIndex);
		}
	}

	// Token: 0x0600066B RID: 1643 RVA: 0x0002EA8C File Offset: 0x0002CC8C
	private void InvadeLocation(int locationIndex)
	{
		this.DisruptLocation(locationIndex);
		if (this.campaignlocations[locationIndex].originalFaction == this.campaignlocations[locationIndex].faction)
		{
			if (this.campaignlocations[locationIndex].originalFaction == "FRIENDLY")
			{
				this.campaignlocations[locationIndex].faction = "ENEMY_OCCUPIED";
				this.SetTerritoryState(this.campaignlocations[locationIndex].linksToRegionWaypoint, "ENEMY");
			}
			else
			{
				this.campaignlocations[locationIndex].faction = "FRIENDLY_OCCUPIED";
				this.SetTerritoryState(this.campaignlocations[locationIndex].linksToRegionWaypoint, "FRIENDLY");
			}
		}
		else
		{
			this.campaignlocations[locationIndex].faction = this.campaignlocations[locationIndex].originalFaction;
		}
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x0002EB58 File Offset: 0x0002CD58
	private void DisruptSosus(int locationIndex)
	{
		if (this.campaignlocations[locationIndex].linksToSosus != null)
		{
			foreach (string b in this.campaignlocations[locationIndex].linksToSosus)
			{
				for (int j = 0; j < this.sosusNames.Length; j++)
				{
					if (this.sosusNames[j] == b)
					{
						this.sosusBarriers[j].SetActive(false);
					}
				}
			}
		}
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x0002EBD8 File Offset: 0x0002CDD8
	public void RevealPlayerOnMap()
	{
		this.playercampaigndata.playerRevealed = 6f;
		for (int i = 0; i < this.activeTaskForces.Length; i++)
		{
			if (this.activeTaskForces[i])
			{
				float num = Vector3.Distance(this.campaignTaskForceObjects[i].transform.position, this.playerGameObject.transform.position);
				if (num < 50f && this.campaignTaskForces[i].behaviourType == "OFFENSIVE")
				{
					Vector2 mapCoords = this.GetMapCoords(this.campaignTaskForceObjects[i].transform.position.x, this.campaignTaskForceObjects[i].transform.position.y, 256, 128);
					string key = Mathf.FloorToInt(mapCoords.x).ToString() + "," + Mathf.FloorToInt(mapCoords.y).ToString();
					if (!this.tileGraph.nodes.ContainsKey(key))
					{
						this.tileGraph.GetWalkableNeighbours(Mathf.FloorToInt(mapCoords.x), Mathf.FloorToInt(mapCoords.y));
					}
					this.SetAStarPath(i, new Vector2(this.playerGameObject.transform.position.x, this.playerGameObject.transform.position.y));
					this.GetNextTilePosition(i);
					this.campaignTaskForces[i].currentAction = 5;
				}
			}
		}
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x0002ED74 File Offset: 0x0002CF74
	private void SetAStarPath(int i, Vector2 target)
	{
		this.campaignTaskForces[i].pathAStar = null;
		Vector2 mapCoords = this.GetMapCoords(this.campaignTaskForceObjects[i].transform.position.x, this.campaignTaskForceObjects[i].transform.position.y, 256, 128);
		this.campaignTaskForces[i].currTile = Mathf.FloorToInt(mapCoords.x).ToString() + "," + Mathf.FloorToInt(mapCoords.y).ToString();
		mapCoords = this.GetMapCoords(target.x, target.y, 256, 128);
		this.campaignTaskForces[i].destTile = Mathf.FloorToInt(mapCoords.x).ToString() + "," + Mathf.FloorToInt(mapCoords.y).ToString();
		this.campaignTaskForces[i].pathAStar = new Path_AStar(this.campaignTaskForces[i].currTile, this.campaignTaskForces[i].destTile);
		this.campaignTaskForces[i].pathAStar.timer = 2f;
		this.campaignTaskForces[i].currentAction = 5;
		this.campaignTaskForces[i].previousWaypoint = -1;
		if (this.campaignTaskForces[i].finalLegDone)
		{
			this.campaignTaskForces[i].finalLegDone = false;
			this.campaignTaskForces[i].legOfRoute--;
			if (this.campaignTaskForces[i].legOfRoute < 0)
			{
				this.campaignTaskForces[i].legOfRoute = 0;
			}
		}
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x0002EF2C File Offset: 0x0002D12C
	private void CheckMapRanges()
	{
		float[] array = new float[this.activeTaskForces.Length];
		bool flag = false;
		for (int i = 0; i < this.activeTaskForces.Length; i++)
		{
			if (this.activeTaskForces[i])
			{
				array[i] = Vector3.Distance(this.campaignTaskForceObjects[i].transform.position, this.playerGameObject.transform.position);
				if (array[i] < this.currentEngageDistance)
				{
					flag = true;
				}
				else if (array[i] < this.currentDetectionDistance)
				{
					CampaignMapRevealContact component = this.campaignTaskForceObjects[i].GetComponent<CampaignMapRevealContact>();
					component.RevealContact();
				}
			}
		}
		if (flag)
		{
			float num = 5000f;
			int taskforceID = 0;
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j] < num && this.activeTaskForces[j])
				{
					num = array[j];
					taskforceID = j;
				}
			}
			UIFunctions.globaluifunctions.campaignmanager.CampaignEngagement(taskforceID);
		}
		else if (this.campaignmissions[this.campaignTaskForces[this.playercampaigndata.currentMissionTaskForceID].missionID].taskForceBehaviour == "STATIONARY")
		{
			Vector3 vector = Vector3.zero;
			if (this.campaignTaskForces[this.playercampaigndata.currentMissionTaskForceID].endIsLocation)
			{
				vector = this.campaignlocations[this.campaignTaskForces[this.playercampaigndata.currentMissionTaskForceID].endLocationID].baseLocation;
			}
			else
			{
				vector = this.campaignmapwaypoints[this.campaignTaskForces[this.playercampaigndata.currentMissionTaskForceID].endLocationID].waypointPosition;
			}
			if (Vector3.Distance(this.playerGameObject.transform.localPosition, new Vector3(vector.x, vector.y, this.playerGameObject.transform.localPosition.z)) < 10f)
			{
				this.CheckMissionEnded(this.playercampaigndata.currentMissionTaskForceID, "STATIONARY");
			}
		}
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x0002F13C File Offset: 0x0002D33C
	private void FixedUpdate()
	{
		this.hoursDurationOfCampaign += Time.deltaTime;
		if (!this.eventManager.gameObject.activeSelf)
		{
			this.hoursToNextGeneralEvent += Time.deltaTime;
		}
		this.clockTimer += Time.deltaTime;
		if (this.clockTimer >= 1f)
		{
			this.SetCalendarData();
			this.clockTimer = 0f;
		}
		for (int i = 0; i < this.taskForceActivateTimers.Length; i++)
		{
			if (this.taskForceActivateTimers[i] > 0f)
			{
				this.taskForceActivateTimers[i] -= Time.deltaTime;
				if (this.taskForceActivateTimers[i] <= 0f)
				{
					this.activeTaskForces[i] = true;
					this.campaignTaskForceObjects[i].SetActive(true);
				}
			}
		}
		this.strategicRangesTimer += Time.deltaTime;
		if (this.strategicRangesTimer > this.strategicRangeCheck)
		{
			this.CheckMapRanges();
			this.strategicRangesTimer -= this.strategicRangeCheck;
		}
		if (this.playercampaigndata.playerRevealed > 0f)
		{
			this.playercampaigndata.playerRevealed -= Time.deltaTime;
		}
		if (this.campaignInitialised)
		{
			for (int j = 0; j < this.campaignlocations.Length; j++)
			{
				if (this.campaignlocations[j].hasAirbase)
				{
					this.campaignlocations[j].aircraftTimer += Time.deltaTime;
					if (!this.campaignlocations[j].onPatrol && this.campaignlocations[j].aircraftTimer > this.aircraftPrepTime[j])
					{
						this.SetAircraftWaypoints(j);
					}
					else if (this.campaignlocations[j].onPatrol)
					{
						this.campaignAircraftObjects[j].transform.LookAt(this.campaignlocations[j].aircraftWaypoints[this.campaignlocations[j].currentWaypoint]);
						this.campaignAircraftObjects[j].transform.Translate(Vector3.forward * this.campaignaircraft[this.campaignlocations[j].aircraftType].speed * Time.deltaTime);
						if (Vector3.Distance(this.campaignAircraftObjects[j].transform.position, this.campaignlocations[j].aircraftWaypoints[this.campaignlocations[j].currentWaypoint]) < 2f)
						{
							this.campaignlocations[j].currentWaypoint++;
							if (this.campaignlocations[j].currentWaypoint >= this.campaignlocations[j].aircraftWaypoints.Length)
							{
								this.campaignlocations[j].currentWaypoint = 0;
								this.campaignlocations[j].onPatrol = false;
								this.campaignAircraftObjects[j].SetActive(false);
								this.campaignlocations[j].aircraftTimer = 0f;
								this.aircraftPrepTime[j] = 3f + UnityEngine.Random.Range(0f, this.campaignlocations[j].aircraftFrequency);
							}
						}
					}
				}
			}
			for (int k = 0; k < this.campaignSatelliteObjects.Length; k++)
			{
				if (!this.satelliteMoving[k])
				{
					this.satelliteTimers[k] += Time.deltaTime;
					if (this.satelliteTimers[k] > this.satelliteReturnTime)
					{
						this.satelliteMoving[k] = true;
						this.SetSatelliteWaypoint(k);
					}
				}
				else
				{
					this.campaignSatelliteObjects[k].transform.LookAt(this.satelliteWaypoints[k]);
					this.campaignSatelliteObjects[k].transform.Translate(Vector3.forward * this.campaignsatellites[k].speed * Time.deltaTime);
					if (Vector3.Distance(this.campaignSatelliteObjects[k].transform.position, this.satelliteWaypoints[k]) < 2f)
					{
						this.satelliteTimers[k] = 0f;
						this.satelliteMoving[k] = false;
						this.campaignSatelliteObjects[k].SetActive(false);
					}
				}
			}
			for (int l = 0; l < this.campaignTaskForces.Length; l++)
			{
				if (this.activeTaskForces[l])
				{
					this.ExecuteTaskForceAI(l);
				}
			}
		}
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x0002F5AC File Offset: 0x0002D7AC
	private void SetGeneralEvent()
	{
		this.hoursToNextGeneralEvent = 0f;
		this.generalEventModifier = UnityEngine.Random.Range(this.hoursPerGeneralEventRange.x, this.hoursPerGeneralEventRange.y);
		this.eventManager.lastZoneModified = -1;
		if (!this.useLandWar)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < this.campaignregionwaypoints.Length; i++)
		{
			if (this.campaignregionwaypoints[i].currentFaction == "FRIENDLY")
			{
				num2++;
			}
			else
			{
				num++;
			}
		}
		bool flag = true;
		float num3 = this.campaignPoints / this.totalCampaignPoints;
		if (!this.armisticeEventOccurred && this.thresholdMet)
		{
			if (num3 >= 0.5f)
			{
				flag = false;
			}
			if (num3 <= 0.1f || num3 >= 0.9f)
			{
				if (flag)
				{
					this.eventManager.BringInEvent(this.eventManager.specialEventIDs[8], false, false);
				}
				else
				{
					this.eventManager.BringInEvent(this.eventManager.specialEventIDs[9], false, false);
				}
				this.armisticeEventOccurred = true;
				this.lastMissionCompleteStatus = 0;
				return;
			}
		}
		bool flag2 = false;
		int[] array = new int[3];
		array[0] = -1;
		array[1] = -1;
		int[] array2 = array;
		bool flag3 = false;
		int[] array3 = new int[3];
		array3[0] = -1;
		array3[1] = -1;
		int[] array4 = array3;
		if (!this.thresholdMet)
		{
			if (this.campaignregionwaypoints[this.firstOccupiedTerritory[0]].faction == "FRIENDLY")
			{
				if (num < this.territoryTakebackThreshold)
				{
					array4 = this.GetInvadedZone("ENEMY");
					if (array4 == null)
					{
						return;
					}
					flag3 = true;
					num++;
					if (num >= this.territoryTakebackThreshold)
					{
						this.thresholdMet = true;
					}
				}
				else
				{
					array2 = this.GetInvadedZone("FRIENDLY");
					if (array2 == null)
					{
						return;
					}
					flag2 = true;
					num2++;
					if (num2 >= this.territoryTakebackThreshold)
					{
						this.thresholdMet = true;
					}
				}
			}
		}
		else
		{
			float value = UnityEngine.Random.value;
			if (value > num3)
			{
				array2 = this.GetInvadedZone("FRIENDLY");
				if (array2 == null)
				{
					return;
				}
				if (array2[1] != -1)
				{
					flag2 = true;
				}
			}
			value = UnityEngine.Random.value;
			if (value < num3)
			{
				array4 = this.GetInvadedZone("ENEMY");
				if (array4 == null)
				{
					return;
				}
				if (array4[1] != -1)
				{
					flag3 = true;
				}
			}
		}
		if (flag2)
		{
			if (array2[0] != -1)
			{
				this.InvadeLocation(array2[0]);
			}
			this.SetEventInvasionStatus(array2[1], "FRIENDLY");
			this.SetTerritoryState(array2[1], "FRIENDLY");
			int num4 = 12;
			if (array2[2] == 1)
			{
				if (this.campaignregionwaypoints[array2[1]].invadedByRoute.Contains("AIR"))
				{
					num4 = 13;
				}
				else if (this.campaignregionwaypoints[array2[1]].invadedByRoute.Contains("SEA"))
				{
					num4 = 11;
				}
			}
			if (this.campaignregionwaypoints[array2[1]].faction == "FRIENDLY")
			{
				num4 += 6;
			}
			this.eventManager.BringInEvent(this.eventManager.specialEventIDs[num4], false, false);
		}
		else if (flag3)
		{
			if (array4[0] != -1)
			{
				this.InvadeLocation(array4[0]);
			}
			this.SetEventInvasionStatus(array4[1], "ENEMY");
			this.SetTerritoryState(array4[1], "ENEMY");
			int num5 = 15;
			if (array4[2] == 1)
			{
				if (this.campaignregionwaypoints[array4[1]].invadedByRoute.Contains("AIR"))
				{
					num5 = 16;
				}
				else if (this.campaignregionwaypoints[array4[1]].invadedByRoute.Contains("SEA"))
				{
					num5 = 14;
				}
			}
			if (this.campaignregionwaypoints[array4[1]].faction == "ENEMY")
			{
				num5 += 6;
			}
			this.eventManager.BringInEvent(this.eventManager.specialEventIDs[num5], false, false);
		}
		this.lastMissionCompleteStatus = 0;
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x0002F9C0 File Offset: 0x0002DBC0
	private void SetEventInvasionStatus(int zoneIndex, string invadingFaction)
	{
		this.eventManager.currentTakeOverStatus = "_PARTIAL";
		string country = this.campaignregionwaypoints[zoneIndex].country;
		int num = 0;
		for (int i = 0; i < this.regionNames.Length; i++)
		{
			if (this.regionNames[i] == country)
			{
				num = this.regionNumberOfZones[i];
				break;
			}
		}
		int num2 = 0;
		for (int j = 0; j < this.campaignregionwaypoints.Length; j++)
		{
			if (this.campaignregionwaypoints[j].country == country && this.campaignregionwaypoints[j].faction != this.campaignregionwaypoints[j].currentFaction)
			{
				num2++;
			}
		}
		if (invadingFaction != this.campaignregionwaypoints[zoneIndex].faction)
		{
			if (num2 == num - 1)
			{
				this.eventManager.currentTakeOverStatus = "_TOTAL";
			}
			else if (num2 == 0)
			{
				this.eventManager.currentTakeOverStatus = "_INITIAL";
			}
		}
		else if (num2 == num)
		{
			this.eventManager.currentTakeOverStatus = "_TOTAL";
		}
		else if (num2 == 1)
		{
			this.eventManager.currentTakeOverStatus = "_INITIAL";
		}
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x0002FB10 File Offset: 0x0002DD10
	public void SetTerritoryState(int territoryIndex, string newFaction)
	{
		this.eventManager.lastZoneModified = territoryIndex;
		this.campaignregionwaypoints[territoryIndex].currentFaction = newFaction;
		if (this.iconsInOccupiedOnly)
		{
			if (this.campaignregionwaypoints[territoryIndex].currentFaction != this.campaignregionwaypoints[territoryIndex].faction)
			{
				if (!this.regionIcons[territoryIndex].gameObject.activeSelf)
				{
					this.regionIcons[territoryIndex].gameObject.SetActive(true);
				}
			}
			else
			{
				this.regionIcons[territoryIndex].gameObject.SetActive(false);
			}
		}
		else if (!this.regionIcons[territoryIndex].gameObject.activeSelf)
		{
			this.regionIcons[territoryIndex].gameObject.SetActive(true);
		}
		this.SetRegionIcon(territoryIndex);
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x0002FBE0 File Offset: 0x0002DDE0
	private void SetRegionIcon(int territoryIndex)
	{
		if (this.regionIcons[territoryIndex].gameObject.activeSelf)
		{
			if (this.campaignregionwaypoints[territoryIndex].currentFaction == "FRIENDLY")
			{
				this.regionIcons[territoryIndex].color = this.playerIconColor;
				this.regionIcons[territoryIndex].sprite = this.occupiedZoneSprite[0];
			}
			else
			{
				this.regionIcons[territoryIndex].color = this.enemyIconColor;
				this.regionIcons[territoryIndex].sprite = this.occupiedZoneSprite[1];
			}
		}
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x0002FC78 File Offset: 0x0002DE78
	private int[] GetInvadedZone(string invadingFaction)
	{
		List<int> list = new List<int>();
		string b = "FRIENDLY";
		if (invadingFaction == "FRIENDLY")
		{
			b = "ENEMY";
		}
		for (int i = 0; i < this.campaignregionwaypoints.Length; i++)
		{
			if (this.campaignregionwaypoints[i].currentFaction == invadingFaction)
			{
				for (int j = 0; j < this.campaignregionwaypoints[i].connectedWaypoints.Length; j++)
				{
					if (this.campaignregionwaypoints[this.campaignregionwaypoints[i].connectedWaypoints[j]].currentFaction == b)
					{
						list.Add(this.campaignregionwaypoints[i].connectedWaypoints[j]);
					}
				}
			}
		}
		int num = 0;
		if (list.Count<int>() == 0)
		{
			num = 1;
		}
		else if (UnityEngine.Random.value < this.newFrontChance)
		{
			num = 1;
		}
		if (num == 1)
		{
			for (int k = 0; k < this.campaignregionwaypoints.Length; k++)
			{
				if ((this.campaignregionwaypoints[k].invadedByRoute.Contains("SEA") || this.campaignregionwaypoints[k].invadedByRoute.Contains("LAND") || this.campaignregionwaypoints[k].invadedByRoute.Contains("AIR")) && this.campaignregionwaypoints[k].currentFaction != invadingFaction && this.campaignregionwaypoints[k].invadedByRoute.Contains(invadingFaction) && this.campaignregionwaypoints[k].invadedByRoute.Contains(invadingFaction))
				{
					list.Add(this.campaignregionwaypoints[k].waypointID);
				}
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		int num2 = list[UnityEngine.Random.Range(0, list.Count)];
		int num3 = -1;
		for (int l = 0; l < this.campaignlocations.Length; l++)
		{
			if (this.campaignlocations[l].linksToRegionWaypoint == num2)
			{
				num3 = l;
				break;
			}
		}
		if (num == 1 && !this.campaignregionwaypoints[num2].invadedByRoute.Contains(invadingFaction))
		{
			num = 0;
		}
		if (this.campaignTaskForces[this.playercampaigndata.currentMissionTaskForceID].endIsLocation && num3 == this.campaignTaskForces[this.playercampaigndata.currentMissionTaskForceID].endLocationID)
		{
			return null;
		}
		return new int[]
		{
			num3,
			num2,
			num
		};
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x0002FF0C File Offset: 0x0002E10C
	public void PlayerEnterPort()
	{
		UIFunctions.globaluifunctions.GUICameraObject.depth = 1f;
		UIFunctions.globaluifunctions.campaignmanager.OpenPassiveBriefingMenu();
		this.eventManager.BringInCampaignSummary();
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x0002FF48 File Offset: 0x0002E148
	public void SetCalendarData()
	{
		int num = Mathf.FloorToInt(this.hoursDurationOfCampaign / 24f);
		string text = CalendarFunctions.FromJulian((long)(this.julianStartDay + (float)num), "dd/MM/yy");
		string[] array = text.Split(new char[]
		{
			'/'
		});
		int num2 = int.Parse(array[1]);
		this.dateDisplay.text = string.Concat(new string[]
		{
			array[0],
			" ",
			CalendarFunctions.GetFullMonth(num2, true, true),
			" ",
			array[2]
		});
		int num3 = (int)this.hoursDurationOfCampaign - num * 24;
		this.currentHourMonth[0] = num3;
		this.currentHourMonth[1] = num2;
		if (num3 < 10)
		{
			this.timeDisplay.text = "0" + num3.ToString() + "00";
		}
		else
		{
			this.timeDisplay.text = num3.ToString() + "00";
		}
		if (this.hoursToNextGeneralEvent > this.hoursPerGeneralEvent + this.generalEventModifier && !this.eventManager.gameObject.activeSelf)
		{
			this.SetGeneralEvent();
		}
		this.straticMapEnterPortButton.SetActive(this.playerInPort);
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x00030084 File Offset: 0x0002E284
	private void Update()
	{
		if (this.timeInPortTimer < this.timeInPort && UIFunctions.globaluifunctions.menuSystem[5].activeSelf)
		{
			Time.timeScale = 10f * this.mapTimeCompression;
			this.timeInPortTimer += Time.deltaTime;
			this.accumulatedTimeInPort += Time.deltaTime;
			if (this.timeInPortTimer >= this.timeInPort)
			{
				this.timeInPort = 0f;
				this.timeInPortTimer = this.timeInPort;
				Time.timeScale = this.mapTimeCompression;
				base.enabled = false;
			}
			this.timeInPortDisplay.text = LanguageManager.interfaceDictionary[this.timeInPortDisplay.name] + "  " + string.Format("{0:0}", this.accumulatedTimeInPort);
			UIFunctions.globaluifunctions.dateTitle.text = this.dateDisplay.text;
			Text dateTitle = UIFunctions.globaluifunctions.dateTitle;
			string text = dateTitle.text;
			dateTitle.text = string.Concat(new string[]
			{
				text,
				"    ",
				this.timeDisplay.text,
				" ",
				LanguageManager.interfaceDictionary["Hours"]
			});
		}
		else
		{
			this.UpdatePlayerInput();
		}
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x000301E0 File Offset: 0x0002E3E0
	public void PauseStratMap()
	{
		if (Time.timeScale == 0f)
		{
			Time.timeScale = this.mapTimeCompression;
		}
		else
		{
			Time.timeScale = 0f;
		}
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x0003020C File Offset: 0x0002E40C
	public void SlowStratMap()
	{
		Time.timeScale = this.mapTimeCompression / 5f;
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x00030220 File Offset: 0x0002E420
	public void PlayStratMap()
	{
		Time.timeScale = this.mapTimeCompression;
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x00030230 File Offset: 0x0002E430
	private void SetPlayerToolbarStats(int value)
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		Vector3 zero = Vector3.zero;
		zero.x = this.GetStrategicMapHeading(this.playerGameObject.transform.localEulerAngles.x, this.playerGameObject.transform.localEulerAngles.y);
		zero.y = this.playerMapSpeeds[value] / this.mapSpeedModifier * 10f;
		zero.z = this.playerStartDepths[value];
		if (value == 0)
		{
			this.toolbarPlayerStats.text = "STATIONARY";
		}
		else if (value == 1)
		{
			this.toolbarPlayerStats.text = "PATROL";
		}
		else
		{
			this.toolbarPlayerStats.text = "TRANSIT";
		}
		Text text = this.toolbarPlayerStats;
		text.text = text.text + "\n" + string.Format("{0:0}", zero.x);
		Text text2 = this.toolbarPlayerStats;
		text2.text = text2.text + "\n" + string.Format("{0:0}", zero.y);
		Text text3 = this.toolbarPlayerStats;
		text3.text = text3.text + "\n" + string.Format("{0:0}", zero.z);
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x00030398 File Offset: 0x0002E598
	public void UpdatePlayerInput()
	{
		bool flag = false;
		this.currentDetectionDistance = this.detectionDistances.x;
		if (InputManager.globalInputManager.GetButtonDown("Patrol Speed", true))
		{
			this.playerCurrentSpeed = this.playerMapSpeeds[1];
			flag = true;
			this.currentDetectionDistance = this.detectionDistances.y;
			this.SetPlayerToolbarStats(1);
		}
		else if (InputManager.globalInputManager.GetButtonDown("Flank Speed", true))
		{
			this.playerCurrentSpeed = this.playerMapSpeeds[2];
			flag = true;
			this.currentDetectionDistance = this.detectionDistances.z;
			this.SetPlayerToolbarStats(2);
		}
		else if (InputManager.globalInputManager.GetButtonDown("Continue", false))
		{
			this.PauseStratMap();
		}
		if (flag)
		{
			Vector3 position = this.playerGameObjectIcon.transform.position;
			this.lastClickedPosition = UIFunctions.globaluifunctions.GUICameraObject.ScreenToWorldPoint(Input.mousePosition);
			this.lastClickedPosition.z = 123.634f;
			this.playerGameObject.transform.LookAt(this.lastClickedPosition);
			this.playerGameObject.transform.Translate(Vector3.forward * this.playerCurrentSpeed * Time.deltaTime);
			this.playerGameObjectIcon.transform.rotation = Quaternion.identity;
			Vector2 mapCoords = this.GetMapCoords(this.playerGameObject.transform.position.x, this.playerGameObject.transform.position.y, this.mapNavigation.width, this.mapNavigation.height);
			float r = this.mapNavigation.GetPixel(Mathf.FloorToInt(mapCoords.x), Mathf.FloorToInt(mapCoords.y)).r;
			if (r > 0.3f)
			{
				this.playerGameObject.transform.position = position;
				float num = this.lastClickedPosition.x - this.playerGameObject.transform.position.x;
				float num2 = this.lastClickedPosition.y - this.playerGameObject.transform.position.y;
				Vector3 position2 = this.playerGameObject.transform.position;
				r = this.mapNavigation.GetPixel(Mathf.FloorToInt(mapCoords.x), Mathf.FloorToInt(mapCoords.y + 1f)).r;
				if (r < 0.3f && num2 < 0f)
				{
					position2.y += 10f;
				}
				r = this.mapNavigation.GetPixel(Mathf.FloorToInt(mapCoords.x), Mathf.FloorToInt(mapCoords.y - 1f)).r;
				if (r < 0.3f && num2 > 0f)
				{
					position2.y -= 10f;
				}
				r = this.mapNavigation.GetPixel(Mathf.FloorToInt(mapCoords.x + 1f), Mathf.FloorToInt(mapCoords.y)).r;
				if (r < 0.3f && num < 0f)
				{
					position2.x += 10f;
				}
				r = this.mapNavigation.GetPixel(Mathf.FloorToInt(mapCoords.x - 1f), Mathf.FloorToInt(mapCoords.y)).r;
				if (r < 0.3f && num > 0f)
				{
					position2.x -= 10f;
				}
				this.playerGameObject.transform.LookAt(position2);
				this.playerGameObject.transform.Translate(Vector3.forward * this.playerCurrentSpeed * Time.deltaTime);
				this.playerGameObjectIcon.transform.rotation = Quaternion.identity;
				mapCoords = this.GetMapCoords(this.playerGameObject.transform.position.x, this.playerGameObject.transform.position.y, this.mapNavigation.width, this.mapNavigation.height);
				r = this.mapNavigation.GetPixel(Mathf.FloorToInt(mapCoords.x), Mathf.FloorToInt(mapCoords.y)).r;
				if (r > 0.3f)
				{
					this.playerGameObject.transform.position = position;
				}
			}
			this.playerGameObject.transform.localPosition = new Vector3(this.playerGameObject.transform.localPosition.x, this.playerGameObject.transform.localPosition.y, 0f);
		}
		else
		{
			this.playerCurrentSpeed = this.playerMapSpeeds[0];
			this.SetPlayerToolbarStats(0);
		}
		if (InputManager.globalInputManager.GetButtonDown("Briefing", false))
		{
			UIFunctions.globaluifunctions.campaignmanager.strategicMapToolbar.SetActive(false);
			UIFunctions.globaluifunctions.HUDCameraObject.enabled = true;
			this.OpenStrategicMapBriefing();
		}
		if (InputManager.globalInputManager.GetButtonDown("Cancel or Quit", false))
		{
			this.BringInExitMenu();
		}
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x00030904 File Offset: 0x0002EB04
	public void OpenStrategicMapBriefing()
	{
		UIFunctions.globaluifunctions.campaignmanager.strategicMapToolbar.SetActive(false);
		UIFunctions.globaluifunctions.HUDCameraObject.enabled = true;
		this.OpenPassiveBriefingMenu();
	}

	// Token: 0x0600067F RID: 1663 RVA: 0x00030934 File Offset: 0x0002EB34
	public Vector2 GetMapCoords(float posX, float posY, int imageWidth, int imageHeight)
	{
		float x = (posX + 200.2567f) / 400.5135f * (float)imageWidth;
		float y = (posY + 100.1284f) / 200.2567f * (float)imageHeight;
		return new Vector2(x, y);
	}

	// Token: 0x06000680 RID: 1664 RVA: 0x0003096C File Offset: 0x0002EB6C
	public Vector2 GetWorldCoords(int mapX, int mapY, float worldWidth, float worldHeight, int imageWidth, int imageHeight)
	{
		float x = (float)mapX / (float)imageWidth * worldWidth - worldWidth / 2f;
		float y = (float)mapY / (float)imageHeight * worldHeight - worldHeight / 2f;
		return new Vector2(x, y);
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x000309A4 File Offset: 0x0002EBA4
	public Vector2 GetCombatMapCoords()
	{
		Vector2 vector = this.GetMapCoords(this.playerGameObject.transform.position.x, this.playerGameObject.transform.position.y, 256, 128);
		if (this.CheckIfNeighbouringTilesAreWater(vector))
		{
			return this.GetMapCoords(this.playerGameObject.transform.position.x, this.playerGameObject.transform.position.y, 4096, 2048);
		}
		int num = 0;
		num += UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.seaState / 2;
		if (UIFunctions.globaluifunctions.campaignmanager.playerCurrentSpeed > UIFunctions.globaluifunctions.campaignmanager.playerMapSpeeds[0])
		{
			num++;
		}
		if (UIFunctions.globaluifunctions.campaignmanager.playerCurrentSpeed > UIFunctions.globaluifunctions.campaignmanager.playerMapSpeeds[1])
		{
			num++;
		}
		Vector2 zero = Vector2.zero;
		if (num > 2)
		{
			bool[] array = new bool[8];
			int num2 = 0;
			int num3 = 0;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					float r = this.mapNavigation.GetPixel(Mathf.FloorToInt(vector.x + (float)i), Mathf.FloorToInt(vector.y + (float)j)).r;
					if (r <= 0.2f && i != 0 && j != 0)
					{
						array[num2] = true;
						num3++;
					}
				}
			}
			float num4 = 0f;
			for (int k = 0; k < array.Length; k++)
			{
				if (array[k])
				{
					switch (k)
					{
					case 0:
						zero = new Vector2(-8f, -8f);
						num4 = 225f;
						break;
					case 1:
						zero = new Vector2(0f, -8f);
						num4 = 180f;
						break;
					case 2:
						zero = new Vector2(8f, -8f);
						num4 = 135f;
						break;
					case 3:
						zero = new Vector2(-8f, 0f);
						num4 = 270f;
						break;
					case 4:
						zero = new Vector2(8f, 0f);
						num4 = 90f;
						break;
					case 5:
						zero = new Vector2(-8f, 8f);
						num4 = 315f;
						break;
					case 6:
						zero = new Vector2(0f, 8f);
						num4 = 0f;
						break;
					case 7:
						zero = new Vector2(8f, 8f);
						num4 = 45f;
						break;
					}
				}
			}
			num4 += UnityEngine.Random.Range(-15f, 15f);
			vector = new Vector3(vector.x * 16f + zero.x, vector.y * 16f + zero.y);
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.globalSpawnRotation = num4;
			return vector;
		}
		int num5 = 3;
		for (int l = 0; l < 10; l++)
		{
			for (int m = (num5 - 1) / -2; m < (num5 - 1) / 2 + 1; m++)
			{
				for (int n = (num5 - 1) / -2; n < (num5 - 1) / 2 + 1; n++)
				{
					bool flag = false;
					if (n == (num5 - 1) / -2 || n == (num5 - 1) / 2)
					{
						flag = true;
					}
					else if (m == (num5 - 1) / -2 || m == (num5 - 1) / 2)
					{
						flag = true;
					}
					if (flag)
					{
						zero = new Vector2(vector.x + (float)n, vector.y + (float)m);
						if (this.CheckIfNeighbouringTilesAreWater(zero))
						{
							float num6 = vector.x - zero.x;
							float num7 = vector.y - zero.y;
							Vector2 vector2 = new Vector2(8f, 8f);
							if (num6 < 0f)
							{
								vector2.x = -8f;
							}
							if (num7 < 0f)
							{
								vector2.y = -8f;
							}
							return new Vector2(zero.x * 16f + vector2.x, zero.y * 16f + vector2.y);
						}
					}
				}
			}
			num5 += 2;
		}
		return vector;
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x00030E64 File Offset: 0x0002F064
	private bool CheckIf2x2AreWater(Vector2 combatPosition)
	{
		bool result = true;
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				float r = this.mapNavigation.GetPixel(Mathf.FloorToInt(combatPosition.x + (float)i), Mathf.FloorToInt(combatPosition.y + (float)j)).r;
				if (r > 0.2f)
				{
					result = false;
				}
			}
		}
		return result;
	}

	// Token: 0x06000683 RID: 1667 RVA: 0x00030ED8 File Offset: 0x0002F0D8
	private bool CheckIfNeighbouringTilesAreWater(Vector2 combatPosition)
	{
		bool result = true;
		for (int i = -1; i < 2; i++)
		{
			for (int j = -1; j < 2; j++)
			{
				float r = this.mapNavigation.GetPixel(Mathf.FloorToInt(combatPosition.x + (float)i), Mathf.FloorToInt(combatPosition.y + (float)j)).r;
				if (r > 0.2f)
				{
					result = false;
				}
			}
		}
		return result;
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x00030F4C File Offset: 0x0002F14C
	public void BringInDisabledCampaignManager()
	{
		UIFunctions.globaluifunctions.campaignmanager.gameObject.SetActive(true);
		UIFunctions.globaluifunctions.campaignmanager.enabled = false;
	}

	// Token: 0x06000685 RID: 1669 RVA: 0x00030F74 File Offset: 0x0002F174
	private Vector3 GetNextWaypointPosition(int currentTaskforce, int nextWaypoint)
	{
		this.campaignTaskForces[currentTaskforce].previousWaypoint = this.campaignTaskForces[currentTaskforce].currentWaypoint;
		Vector3 b = UnityEngine.Random.insideUnitCircle * this.campaignmapwaypoints[nextWaypoint].waypointRadius;
		Vector3 localPosition = this.campaignmapwaypoints[nextWaypoint].waypointPosition + b;
		this.navHelper.transform.localPosition = localPosition;
		this.campaignTaskForces[currentTaskforce].currentWaypoint = nextWaypoint;
		Vector3 position = this.navHelper.transform.position;
		position.z = 125.1605f;
		return position;
	}

	// Token: 0x06000686 RID: 1670 RVA: 0x0003100C File Offset: 0x0002F20C
	private void ExecuteTaskForceAI(int currentTaskforce)
	{
		switch (this.campaignTaskForces[currentTaskforce].currentAction)
		{
		case 0:
			return;
		case 1:
			this.MoveStraightToWaypoint(currentTaskforce);
			if (Vector3.Distance(this.campaignTaskForceObjects[currentTaskforce].transform.position, this.campaignTaskForces[currentTaskforce].waypointPosition) < 2f)
			{
				this.campaignTaskForces[currentTaskforce].currentAction = 2;
			}
			break;
		case 2:
		{
			int taskForceNextWaypoint = this.GetTaskForceNextWaypoint(currentTaskforce);
			if (taskForceNextWaypoint != -1 && taskForceNextWaypoint != -3)
			{
				if (taskForceNextWaypoint != -2)
				{
					this.campaignTaskForces[currentTaskforce].waypointPosition = this.GetNextWaypointPosition(currentTaskforce, taskForceNextWaypoint);
					this.campaignTaskForces[currentTaskforce].currentAction = 1;
				}
			}
			else if (taskForceNextWaypoint != -3)
			{
				this.campaignTaskForces[currentTaskforce].currentAction = 0;
			}
			break;
		}
		case 3:
			this.campaignTaskForces[currentTaskforce].patrolForHours -= Time.deltaTime;
			if (this.campaignTaskForces[currentTaskforce].patrolForHours < 0f)
			{
				this.CheckMissionEnded(currentTaskforce, "PATROL_END");
				if (this.campaignTaskForces[currentTaskforce].endIsLocation)
				{
					this.CheckMissionEnded(currentTaskforce, "RETURN_HOME");
					return;
				}
				if (!this.campaignTaskForces[currentTaskforce].startIsLocation)
				{
					string[] locationTypes = new string[]
					{
						"NAVAL_BASE"
					};
					int[] arrayOfLocations = this.GetArrayOfLocations("ENEMY", locationTypes, "ANY", false);
					int num = UnityEngine.Random.Range(0, arrayOfLocations.Length);
					this.campaignTaskForces[currentTaskforce].startPositionID = arrayOfLocations[num];
				}
				if (this.campaignmissions[this.campaignTaskForces[currentTaskforce].missionID].missionEndsWhen == "RETURN_HOME")
				{
					this.campaignTaskForces[currentTaskforce].mustUseWaypoints = this.campaignlocations[this.campaignTaskForces[currentTaskforce].startPositionID].linksToWaypoint;
					this.campaignTaskForces[currentTaskforce].finalLocationPosition = this.campaignlocations[this.campaignTaskForces[currentTaskforce].startPositionID].baseLocation;
					this.campaignTaskForces[currentTaskforce].legOfRoute = 0;
					this.campaignTaskForces[currentTaskforce].finalLegDone = false;
					this.campaignTaskForces[currentTaskforce].endIsLocation = true;
					this.campaignTaskForces[currentTaskforce].currentAction = 1;
					this.campaignTaskForces[currentTaskforce].previousWaypoint = -1;
				}
			}
			if (this.campaignTaskForces[currentTaskforce].behaviourType == "STATIONARY")
			{
				return;
			}
			if (this.campaignMissionTypes[this.campaignTaskForces[currentTaskforce].missionType] == "INSERTION" || this.campaignMissionTypes[this.campaignTaskForces[currentTaskforce].missionType] == "LANDING_FORCE" || this.campaignMissionTypes[this.campaignTaskForces[currentTaskforce].missionType] == "RESUPPLY_CONVOY")
			{
				return;
			}
			this.MoveStraightToWaypoint(currentTaskforce);
			if (this.campaignmapwaypoints[this.campaignTaskForces[currentTaskforce].currentWaypoint].waypointRadius > 30f && Vector3.Distance(this.campaignTaskForceObjects[currentTaskforce].transform.position, this.campaignTaskForces[currentTaskforce].waypointPosition) < 2f)
			{
				this.campaignTaskForces[currentTaskforce].waypointPosition = this.GetNextWaypointPosition(currentTaskforce, this.campaignTaskForces[currentTaskforce].currentWaypoint);
			}
			break;
		case 5:
		{
			float num2 = Vector3.Distance(this.playerGameObject.transform.position, this.campaignTaskForceObjects[currentTaskforce].transform.position);
			if (this.playercampaigndata.playerRevealed > 0f)
			{
				if (this.playerInPort)
				{
					if (num2 < 35f)
					{
						this.playercampaigndata.playerRevealed = 0f;
						this.campaignTaskForces[currentTaskforce].pathAStar = null;
						Vector2 nearestMapPositionToReturnTo = this.GetNearestMapPositionToReturnTo(currentTaskforce);
						this.SetAStarPath(currentTaskforce, new Vector2(nearestMapPositionToReturnTo.x, nearestMapPositionToReturnTo.y));
						if (this.campaignTaskForces[currentTaskforce].pathAStar.Length() < 3)
						{
							this.campaignTaskForces[currentTaskforce].currentAction = 2;
						}
						return;
					}
				}
				else if (num2 < 100f)
				{
					this.campaignTaskForces[currentTaskforce].pathAStar.timer -= Time.deltaTime;
					if (this.campaignTaskForces[currentTaskforce].pathAStar.timer <= 0f)
					{
						this.SetAStarPath(currentTaskforce, new Vector2(this.playerGameObject.transform.position.x, this.playerGameObject.transform.position.y));
					}
				}
			}
			if (this.campaignTaskForces[currentTaskforce].currTile == this.campaignTaskForces[currentTaskforce].destTile)
			{
				this.campaignTaskForces[currentTaskforce].pathAStar = null;
				Vector2 nearestMapPositionToReturnTo2 = this.GetNearestMapPositionToReturnTo(currentTaskforce);
				this.SetAStarPath(currentTaskforce, new Vector2(nearestMapPositionToReturnTo2.x, nearestMapPositionToReturnTo2.y));
				if (this.campaignTaskForces[currentTaskforce].pathAStar.Length() < 3)
				{
					this.campaignTaskForces[currentTaskforce].currentAction = 2;
				}
				return;
			}
			if (this.campaignTaskForces[currentTaskforce].nextTile == null || this.campaignTaskForces[currentTaskforce].nextTile == this.campaignTaskForces[currentTaskforce].currTile)
			{
				if (this.campaignTaskForces[currentTaskforce].pathAStar == null || this.campaignTaskForces[currentTaskforce].pathAStar.Length() == 0)
				{
					this.campaignTaskForces[currentTaskforce].pathAStar = new Path_AStar(this.campaignTaskForces[currentTaskforce].currTile, this.campaignTaskForces[currentTaskforce].destTile);
					if (this.campaignTaskForces[currentTaskforce].pathAStar.Length() == 0)
					{
						this.campaignTaskForces[currentTaskforce].pathAStar = null;
						this.campaignTaskForces[currentTaskforce].currentAction = 0;
						return;
					}
					if (this.campaignTaskForces[currentTaskforce].pathAStar.Length() > 1)
					{
						this.campaignTaskForces[currentTaskforce].nextTile = this.campaignTaskForces[currentTaskforce].pathAStar.Dequeue();
					}
				}
				this.GetNextTilePosition(currentTaskforce);
			}
			this.MoveTowardsNextAStarTile(currentTaskforce, 0.5f);
			break;
		}
		}
	}

	// Token: 0x06000687 RID: 1671 RVA: 0x00031630 File Offset: 0x0002F830
	private void MoveStraightToWaypoint(int currentTaskforce)
	{
		this.campaignTaskForceObjects[currentTaskforce].transform.LookAt(this.campaignTaskForces[currentTaskforce].waypointPosition);
		this.campaignTaskForceObjects[currentTaskforce].transform.Translate(Vector3.forward * this.campaignTaskForces[currentTaskforce].speed * Time.deltaTime * this.mapSpeedModifier);
		this.campaignTaskForceObjects[currentTaskforce].transform.localPosition = new Vector3(this.campaignTaskForceObjects[currentTaskforce].transform.localPosition.x, this.campaignTaskForceObjects[currentTaskforce].transform.localPosition.y, 0f);
		this.campaignTaskForceIcons[currentTaskforce].transform.rotation = Quaternion.identity;
	}

	// Token: 0x06000688 RID: 1672 RVA: 0x00031704 File Offset: 0x0002F904
	private void GetNextTilePosition(int currentTaskforce)
	{
		this.campaignTaskForces[currentTaskforce].nextTile = this.campaignTaskForces[currentTaskforce].pathAStar.Dequeue();
		string[] array = this.campaignTaskForces[currentTaskforce].nextTile.Split(new char[]
		{
			','
		});
		this.campaignTaskForces[currentTaskforce].waypointPosition = this.GetWorldCoords(int.Parse(array[0]), int.Parse(array[1]), 400.5135f, 200.2567f, 256, 128);
		this.campaignTaskForces[currentTaskforce].waypointPosition.z = 125.1605f;
	}

	// Token: 0x06000689 RID: 1673 RVA: 0x000317A4 File Offset: 0x0002F9A4
	private void MoveTowardsNextAStarTile(int currentTaskforce, float speedBonus)
	{
		this.campaignTaskForceObjects[currentTaskforce].transform.LookAt(this.campaignTaskForces[currentTaskforce].waypointPosition);
		this.campaignTaskForceObjects[currentTaskforce].transform.Translate(Vector3.forward * (this.campaignTaskForces[currentTaskforce].speed + speedBonus) * Time.deltaTime * this.mapSpeedModifier);
		this.campaignTaskForceObjects[currentTaskforce].transform.localPosition = new Vector3(this.campaignTaskForceObjects[currentTaskforce].transform.localPosition.x, this.campaignTaskForceObjects[currentTaskforce].transform.localPosition.y, 0f);
		this.campaignTaskForceIcons[currentTaskforce].transform.rotation = Quaternion.identity;
		if (Vector3.Distance(this.campaignTaskForceObjects[currentTaskforce].transform.position, this.campaignTaskForces[currentTaskforce].waypointPosition) < 2f)
		{
			this.campaignTaskForces[currentTaskforce].currTile = this.campaignTaskForces[currentTaskforce].nextTile;
		}
	}

	// Token: 0x0600068A RID: 1674 RVA: 0x000318C0 File Offset: 0x0002FAC0
	private Vector2 GetNearestMapPositionToReturnTo(int currentTaskForce)
	{
		Dictionary<int, float> dictionary = new Dictionary<int, float>();
		for (int i = 0; i < this.campaignmapwaypoints.Length; i++)
		{
			if (!this.campaignTaskForces[currentTaskForce].prohibitedWaypoints.Contains(i))
			{
				dictionary[i] = Vector2.Distance(this.campaignTaskForceObjects[currentTaskForce].transform.position, new Vector3(this.campaignmapwaypoints[i].waypointPosition.x * 0.25674f, this.campaignmapwaypoints[i].waypointPosition.y * 0.25674f, this.campaignmapwaypoints[i].waypointPosition.z));
			}
		}
		foreach (KeyValuePair<int, float> keyValuePair in from key in dictionary
		orderby key.Value
		select key)
		{
			Vector2 vector = this.GetNextWaypointPosition(currentTaskForce, keyValuePair.Key);
			vector = this.GetMapCoords(vector.x, vector.y, 256, 128);
			string key2 = Mathf.FloorToInt(vector.x) + "," + Mathf.FloorToInt(vector.y);
			if (this.tileGraph.nodes.ContainsKey(key2))
			{
				if (this.campaignTaskForces[currentTaskForce].legOfRoute >= this.campaignTaskForces[currentTaskForce].mustUseWaypoints.Length)
				{
					this.campaignTaskForces[currentTaskForce].legOfRoute = this.campaignTaskForces[currentTaskForce].mustUseWaypoints.Length - 1;
				}
				return new Vector3(this.campaignmapwaypoints[keyValuePair.Key].waypointPosition.x * 0.25674f, this.campaignmapwaypoints[keyValuePair.Key].waypointPosition.y * 0.25674f, this.campaignmapwaypoints[keyValuePair.Key].waypointPosition.z);
			}
		}
		return Vector2.zero;
	}

	// Token: 0x0600068B RID: 1675 RVA: 0x00031B08 File Offset: 0x0002FD08
	public void DeactivateTaskforce(int currentTaskforce)
	{
		if (currentTaskforce == -1)
		{
			return;
		}
		this.campaignTaskForceObjects[currentTaskforce].GetComponent<CampaignMapRevealContact>().timer = 0f;
		this.campaignTaskForceObjects[currentTaskforce].SetActive(false);
		this.activeTaskForces[currentTaskforce] = false;
	}

	// Token: 0x0600068C RID: 1676 RVA: 0x00031B4C File Offset: 0x0002FD4C
	private int GetTaskForceNextWaypoint(int currentTaskforce)
	{
		if (this.campaignTaskForces[currentTaskforce].currentWaypoint == this.campaignTaskForces[currentTaskforce].mustUseWaypoints[this.campaignTaskForces[currentTaskforce].legOfRoute])
		{
			this.campaignTaskForces[currentTaskforce].legOfRoute++;
			if (this.campaignTaskForces[currentTaskforce].legOfRoute >= this.campaignTaskForces[currentTaskforce].mustUseWaypoints.Length)
			{
				if (!this.campaignTaskForces[currentTaskforce].endIsLocation || this.campaignTaskForces[currentTaskforce].finalLegDone)
				{
					this.campaignTaskForces[currentTaskforce].currentAction = 3;
					this.CheckMissionEnded(currentTaskforce, "PATROL_START");
					return -3;
				}
				this.campaignTaskForces[currentTaskforce].finalLegDone = true;
				Vector3 zero = Vector3.zero;
				zero.x = this.campaignTaskForces[currentTaskforce].finalLocationPosition.x;
				zero.y = this.campaignTaskForces[currentTaskforce].finalLocationPosition.y;
				this.navHelper.transform.localPosition = zero;
				this.campaignTaskForces[currentTaskforce].waypointPosition = this.navHelper.transform.position;
				this.campaignTaskForces[currentTaskforce].waypointPosition.z = 125.1605f;
				this.campaignTaskForces[currentTaskforce].legOfRoute--;
				this.campaignTaskForces[currentTaskforce].currentAction = 1;
				return -2;
			}
		}
		float num = this.campaignmapwaypoints[this.campaignTaskForces[currentTaskforce].mustUseWaypoints[this.campaignTaskForces[currentTaskforce].legOfRoute]].waypointPosition.x - this.campaignTaskForceObjects[currentTaskforce].transform.localPosition.x;
		float num2 = this.campaignmapwaypoints[this.campaignTaskForces[currentTaskforce].mustUseWaypoints[this.campaignTaskForces[currentTaskforce].legOfRoute]].waypointPosition.y - this.campaignTaskForceObjects[currentTaskforce].transform.localPosition.y;
		int currentWaypoint = this.campaignTaskForces[currentTaskforce].currentWaypoint;
		this.campaignTaskForces[currentTaskforce].possibleWaypoints = new List<int>();
		bool[] array = new bool[4];
		if (num >= 0f)
		{
			this.GetAdditionalPossibleWaypoints(this.campaignTaskForces[currentTaskforce], this.campaignmapwaypoints[currentWaypoint].eastWaypoints);
			array[1] = true;
		}
		else
		{
			this.GetAdditionalPossibleWaypoints(this.campaignTaskForces[currentTaskforce], this.campaignmapwaypoints[currentWaypoint].westWaypoints);
			array[3] = true;
		}
		if (num2 >= 0f)
		{
			this.GetAdditionalPossibleWaypoints(this.campaignTaskForces[currentTaskforce], this.campaignmapwaypoints[currentWaypoint].northWaypoints);
			array[0] = true;
		}
		else
		{
			this.GetAdditionalPossibleWaypoints(this.campaignTaskForces[currentTaskforce], this.campaignmapwaypoints[currentWaypoint].southWaypoints);
			array[2] = true;
		}
		if (this.campaignTaskForces[currentTaskforce].possibleWaypoints.Count == 0)
		{
			if (!array[0])
			{
				this.GetAdditionalPossibleWaypoints(this.campaignTaskForces[currentTaskforce], this.campaignmapwaypoints[currentWaypoint].northWaypoints);
			}
			if (!array[1])
			{
				this.GetAdditionalPossibleWaypoints(this.campaignTaskForces[currentTaskforce], this.campaignmapwaypoints[currentWaypoint].eastWaypoints);
			}
			if (!array[2])
			{
				this.GetAdditionalPossibleWaypoints(this.campaignTaskForces[currentTaskforce], this.campaignmapwaypoints[currentWaypoint].southWaypoints);
			}
			if (!array[3])
			{
				this.GetAdditionalPossibleWaypoints(this.campaignTaskForces[currentTaskforce], this.campaignmapwaypoints[currentWaypoint].westWaypoints);
			}
		}
		if (this.campaignTaskForces[currentTaskforce].possibleWaypoints.Count == 0)
		{
			return this.campaignTaskForces[currentTaskforce].previousWaypoint;
		}
		if (this.campaignTaskForces[currentTaskforce].possibleWaypoints.Contains(this.campaignTaskForces[currentTaskforce].mustUseWaypoints[this.campaignTaskForces[currentTaskforce].legOfRoute]))
		{
			return this.campaignTaskForces[currentTaskforce].mustUseWaypoints[this.campaignTaskForces[currentTaskforce].legOfRoute];
		}
		if (this.campaignTaskForces[currentTaskforce].takeDirectRoute && this.campaignTaskForces[currentTaskforce].possibleWaypoints.Count > 1)
		{
			float num3 = 5000f;
			int index = 0;
			for (int i = 0; i < this.campaignTaskForces[currentTaskforce].possibleWaypoints.Count; i++)
			{
				float num4 = Vector3.Distance(this.campaignmapwaypoints[this.campaignTaskForces[currentTaskforce].possibleWaypoints[i]].waypointPosition, this.campaignmapwaypoints[this.campaignTaskForces[currentTaskforce].mustUseWaypoints[this.campaignTaskForces[currentTaskforce].legOfRoute]].waypointPosition);
				if (num4 < num3)
				{
					num3 = num4;
					index = i;
				}
			}
			return this.campaignTaskForces[currentTaskforce].possibleWaypoints[index];
		}
		return this.campaignTaskForces[currentTaskforce].possibleWaypoints[UnityEngine.Random.Range(0, this.campaignTaskForces[currentTaskforce].possibleWaypoints.Count)];
	}

	// Token: 0x0600068D RID: 1677 RVA: 0x00032020 File Offset: 0x00030220
	private void GetAdditionalPossibleWaypoints(CampaignTaskForce currentTaskForce, int[] waypointsToCheck)
	{
		for (int i = 0; i < waypointsToCheck.Length; i++)
		{
			if (waypointsToCheck[i] != currentTaskForce.previousWaypoint && !this.CheckProhibitedWaypoint(currentTaskForce, waypointsToCheck[i]))
			{
				bool flag = false;
				if (currentTaskForce.possibleWaypoints.Contains(waypointsToCheck[i]))
				{
					flag = true;
				}
				if (!flag)
				{
					currentTaskForce.possibleWaypoints.Add(waypointsToCheck[i]);
				}
			}
		}
	}

	// Token: 0x0600068E RID: 1678 RVA: 0x0003208C File Offset: 0x0003028C
	private bool CheckProhibitedWaypoint(CampaignTaskForce currentTaskForce, int waypointNumber)
	{
		for (int i = 0; i < currentTaskForce.prohibitedWaypoints.Length; i++)
		{
			if (currentTaskForce.prohibitedWaypoints[i] == waypointNumber)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600068F RID: 1679 RVA: 0x000320C4 File Offset: 0x000302C4
	private void SetAircraftWaypoints(int i)
	{
		this.campaignlocations[i].aircraftWaypoints = new Vector3[2];
		this.campaignAircraftObjects[i].transform.localPosition = new Vector3(this.campaignlocations[i].baseLocation.x, this.campaignlocations[i].baseLocation.y, 0f);
		this.campaignAircraftObjects[i].transform.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(this.campaignlocations[i].aircraftHeadings.x, this.campaignlocations[i].aircraftHeadings.y)), 1f);
		float num = this.campaignlocations[i].airSearchRange;
		float patrolRange = this.campaignaircraft[this.campaignlocations[i].aircraftType].patrolRange;
		if (this.campaignlocations[i].faction.Contains("OCCUPIED"))
		{
			patrolRange = this.campaignaircraft[this.campaignlocations[i].aircraftTypeInvaded].patrolRange;
		}
		if (num > patrolRange)
		{
			num = patrolRange;
		}
		this.campaignAircraftObjects[i].transform.Translate(Vector3.up * num);
		this.campaignlocations[i].aircraftWaypoints[0] = this.campaignAircraftObjects[i].transform.position;
		this.campaignAircraftObjects[i].transform.Translate(Vector3.up * -num);
		this.campaignlocations[i].aircraftWaypoints[1] = this.campaignAircraftObjects[i].transform.position;
		this.campaignlocations[i].onPatrol = true;
		this.campaignlocations[i].currentWaypoint = 0;
		this.SetAircraftFaction(i);
		this.campaignAircraftObjects[i].SetActive(true);
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x000322A4 File Offset: 0x000304A4
	private void SetSatelliteWaypoint(int i)
	{
		this.campaignSatelliteObjects[i].transform.position = this.northPole.transform.position;
		this.currentSatelliteAngle[i] += 33f;
		this.campaignSatelliteObjects[i].transform.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(0f, 0f, -this.currentSatelliteAngle[i]), 1f);
		this.campaignSatelliteObjects[i].transform.Translate(Vector3.up * 300f);
		if (this.satelliteReversed[i])
		{
			this.satelliteWaypoints[i] = this.northPole.transform.position;
		}
		else
		{
			this.satelliteWaypoints[i] = this.campaignSatelliteObjects[i].transform.position;
			this.campaignSatelliteObjects[i].transform.position = this.northPole.transform.position;
		}
		this.campaignSatelliteObjects[i].SetActive(true);
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x000323C8 File Offset: 0x000305C8
	private void RunDebugChecker()
	{
		for (int i = 0; i < this.campaignmissions.Length; i++)
		{
			if (this.campaignmissions[i].numberOfEnemyUnits != null)
			{
				int num = this.campaignmissions[i].numberOfEnemyUnits.Length;
				if (this.campaignmissions[i].enemyUnitMissionCritical.Length != num || this.campaignmissions[i].enemyShipClasses.Length != num || this.campaignmissions[i].enemyShipBehaviour.Length != num)
				{
					UIFunctions.globaluifunctions.SetPlayerErrorMessage(string.Concat(new object[]
					{
						"ERROR:  ",
						this.campaignmissions[i].missionFileName,
						" Mission #",
						this.campaignmissions[i].missionID,
						"\nNumberOfEnemyUnits/CombatBehaviour/EnemyUnitMissionCritical/EnemyShipClasses\nNot all the same number specified"
					}));
					break;
				}
				for (int j = 0; j < num; j++)
				{
					if (false || this.campaignmissions[i].enemyShipClasses[j] == string.Empty || this.campaignmissions[i].enemyShipBehaviour[j] == string.Empty)
					{
						UIFunctions.globaluifunctions.SetPlayerErrorMessage(string.Concat(new object[]
						{
							"ERROR:  ",
							this.campaignmissions[i].missionFileName,
							" Mission #",
							this.campaignmissions[i].missionID,
							"\nNumberOfEnemyUnits/CombatBehaviour/EnemyUnitMissionCritical/EnemyShipClasses\nMissing a reference"
						}));
						break;
					}
				}
			}
			if (i < this.numberOfPlayerMissions && this.campaignmissions[i].missionType != "RETURN_TO_BASE" && !this.campaignmissions[i].finalMission)
			{
				if (!this.GetEventExists(this.campaignmissions[i].eventWin))
				{
					UIFunctions.globaluifunctions.SetPlayerErrorMessage(string.Concat(new object[]
					{
						"ERROR:  ",
						this.campaignmissions[i].missionFileName,
						" Mission #",
						this.campaignmissions[i].missionID,
						"\nEventWin no file found: ",
						this.campaignmissions[i].eventWin
					}));
					break;
				}
				if (!this.GetEventExists(this.campaignmissions[i].eventFail))
				{
					UIFunctions.globaluifunctions.SetPlayerErrorMessage(string.Concat(new object[]
					{
						"ERROR:  ",
						this.campaignmissions[i].missionFileName,
						" Mission #",
						this.campaignmissions[i].missionID,
						"\nEventFail no file found: ",
						this.campaignmissions[i].eventFail
					}));
					break;
				}
			}
		}
		for (int k = 0; k < this.eventManager.campaignevents.Length; k++)
		{
			string nextAction = this.eventManager.campaignevents[k].nextAction;
			if (!(nextAction == "MAP") && !(nextAction == "ASSIGN_NEW") && !(nextAction == "CHECK_MISSION") && !(nextAction == "END"))
			{
				if (!this.GetEventExists(this.eventManager.campaignevents[k].nextAction))
				{
					UIFunctions.globaluifunctions.SetPlayerErrorMessage("ERROR:  " + this.eventManager.campaignevents[k].eventFilename + "\nNextAction no file found: " + this.eventManager.campaignevents[k].nextAction);
					break;
				}
			}
		}
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x00032768 File Offset: 0x00030968
	private bool GetEventExists(string eventName)
	{
		for (int i = 0; i < this.eventManager.campaignevents.Length; i++)
		{
			if (this.eventManager.campaignevents[i].eventFilename == eventName)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x000327B4 File Offset: 0x000309B4
	private void InitialiseCampaign()
	{
		this.campaignInitialised = true;
		CampaignManager.isEncounter = false;
		this.playerInPort = true;
		UIFunctions.globaluifunctions.textparser.ReadCampaignWaypointData();
		UIFunctions.globaluifunctions.textparser.ReadCampaignRegionWaypointData();
		UIFunctions.globaluifunctions.textparser.ReadCombatantsData();
		UIFunctions.globaluifunctions.textparser.ReadCampaignData();
		if (this.campaignDebugMode)
		{
			this.RunDebugChecker();
		}
		if (this.campaignPoints > this.totalCampaignPoints)
		{
			this.campaignPoints = this.totalCampaignPoints;
		}
		this.campaignPoints = this.totalCampaignPoints - this.campaignPoints;
		this.regionNumberOfZones = new int[this.regionNames.Length];
		for (int i = 0; i < this.regionNames.Length; i++)
		{
			for (int j = 0; j < this.campaignregionwaypoints.Length; j++)
			{
				if (this.campaignregionwaypoints[j].country == this.regionNames[i])
				{
					this.regionNumberOfZones[i]++;
				}
			}
		}
		for (int k = 0; k < this.campaignlocations.Length; k++)
		{
			for (int l = 0; l < this.campaignlocations[k].function.Count; l++)
			{
				if (this.campaignlocations[k].function[l] == "PLAYER_BASE")
				{
					this.playerBasePosition = new Vector2(this.campaignlocations[k].baseLocation.x, this.campaignlocations[k].baseLocation.y);
				}
			}
		}
		this.campaignSatelliteObjects = new GameObject[this.campaignsatellites.Length];
		this.satelliteTimers = new float[this.campaignsatellites.Length];
		this.satelliteWaypoints = new Vector3[this.campaignsatellites.Length];
		this.satelliteReversed = new bool[this.campaignsatellites.Length];
		this.satelliteMoving = new bool[this.campaignsatellites.Length];
		this.currentSatelliteAngle = new float[this.campaignsatellites.Length];
		for (int m = 0; m < this.campaignsatellites.Length; m++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(this.mapReconObject, this.reconLayer.position, Quaternion.identity) as GameObject;
			this.campaignSatelliteObjects[m] = gameObject;
			gameObject.GetComponent<SphereCollider>().radius = this.campaignsatellites[m].detectionRange / 10.25f;
			Image componentInChildren = gameObject.GetComponentInChildren<Image>();
			if (this.campaignsatellites[m].faction == "FRIENDLY")
			{
				componentInChildren.color = this.playerIconColor;
				gameObject.name = "FRIENDLY";
			}
			else
			{
				componentInChildren.color = this.enemyIconColor;
				gameObject.name = "ENEMY";
			}
			componentInChildren.sprite = this.reconImages[0];
			gameObject.transform.SetParent(this.reconLayer, false);
			gameObject.transform.position = this.northPole.position;
			this.satelliteReversed[m] = false;
			if (UnityEngine.Random.value < 0.5f)
			{
				this.satelliteReversed[m] = true;
			}
			this.currentSatelliteAngle[m] = UnityEngine.Random.Range(0f, 360f);
			this.satelliteTimers[m] = 0f;
			if (UnityEngine.Random.value < 0.5f)
			{
				this.satelliteMoving[m] = true;
				this.SetSatelliteWaypoint(m);
				this.campaignSatelliteObjects[m].transform.Translate(Vector3.up * UnityEngine.Random.Range(0f, 300f));
			}
			else
			{
				this.satelliteTimers[m] = UnityEngine.Random.Range(0f, this.satelliteReturnTime);
			}
		}
		this.InitialiseAircraft();
		this.sosusBarriers = new GameObject[this.sosusNames.Length];
		for (int n = 0; n < this.sosusBarriers.Length; n++)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(this.sosusBarrierGameObject, this.reconLayer.position, Quaternion.identity) as GameObject;
			this.sosusBarriers[n] = gameObject2;
			BoxCollider component = gameObject2.GetComponent<BoxCollider>();
			gameObject2.name = this.sosusAlignments[n];
			float x = (this.sosusStarts[n].x + this.sosusEnds[n].x) / 2f;
			float y = (this.sosusStarts[n].y + this.sosusEnds[n].y) / 2f;
			gameObject2.transform.SetParent(this.sosusLayer, false);
			gameObject2.transform.localPosition = new Vector3(x, y, 0f);
			component.size = new Vector3(this.sosusDetectionRange[n] / 10.25f, Mathf.Abs(this.sosusStarts[n].y - this.sosusEnds[n].y), 5f);
			gameObject2.transform.rotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(0f, 0f, -this.sosusAngles[n]), 1f);
			RectTransform component2 = gameObject2.GetComponent<RectTransform>();
			component2.sizeDelta = new Vector2(9f, component.size.y);
		}
		this.InitialiseGroundWar();
		this.SetupPlayerSubmarine();
		this.CreatePlayer();
		CampaignManager.isEncounter = false;
		this.OpenMissionBriefing(0);
		UIFunctions.globaluifunctions.portRearm.InitialisePortRearmRepair();
		this.playerPortDetector = (UnityEngine.Object.Instantiate(this.mapTaskForceObject, this.reconLayer.position, Quaternion.identity) as GameObject);
		this.playerPortDetector.transform.SetParent(this.shipLayer, false);
		this.playerPortDetector.transform.position = this.reconLayer.transform.position;
		this.playerPortDetector.transform.localPosition = new Vector3(this.playerBasePosition.x, this.playerBasePosition.y, this.playerPortDetector.transform.localPosition.z);
		this.playerPortDetector.name = "PLAYER_PORT";
		this.playerPortDetector.GetComponent<SphereCollider>().radius = 25f;
		this.campaignTaskForces = new CampaignTaskForce[10];
		this.campaignTaskForceObjects = new GameObject[10];
		this.activeTaskForces = new bool[10];
		this.taskForceActivateTimers = new float[10];
		this.campaignTaskForceIcons = new Image[10];
		for (int num = 0; num < this.campaignTaskForces.Length; num++)
		{
			CampaignTaskForce campaignTaskForce = ScriptableObject.CreateInstance<CampaignTaskForce>();
			this.campaignTaskForces[num] = campaignTaskForce;
			this.campaignTaskForces[num].taskForceID = num;
			GameObject gameObject3 = UnityEngine.Object.Instantiate(this.mapTaskForceObject, this.reconLayer.position, Quaternion.identity) as GameObject;
			gameObject3.name = num.ToString();
			this.campaignTaskForceObjects[num] = gameObject3;
			gameObject3.GetComponent<SphereCollider>().radius = 12f;
			gameObject3.transform.SetParent(this.shipLayer, false);
			this.campaignTaskForceIcons[num] = gameObject3.GetComponentInChildren<Image>();
			this.campaignTaskForceObjects[num].SetActive(false);
			gameObject3.transform.localPosition = Vector3.zero;
		}
		this.julianStartDay += UnityEngine.Random.Range(-this.dayRange, this.dayRange);
		this.hoursDurationOfCampaign = UnityEngine.Random.Range(0f, 24f);
		this.hoursToNextGeneralEvent = 0f;
		this.lastMissionCompleteStatus = 0;
		this.SetCalendarData();
	}

	// Token: 0x06000694 RID: 1684 RVA: 0x00032F78 File Offset: 0x00031178
	public void InitialiseGroundWar()
	{
		this.regionIcons = new Image[this.campaignregionwaypoints.Length];
		for (int i = 0; i < this.campaignregionwaypoints.Length; i++)
		{
			this.campaignregionwaypoints[i].currentFaction = this.campaignregionwaypoints[i].faction;
			GameObject gameObject = UnityEngine.Object.Instantiate(this.mapOccupiedZone, this.reconLayer.position, Quaternion.identity) as GameObject;
			gameObject.transform.SetParent(this.sosusLayer, false);
			gameObject.transform.localPosition = new Vector3(this.campaignregionwaypoints[i].waypointPosition.x, this.campaignregionwaypoints[i].waypointPosition.y, 0f);
			Image component = gameObject.GetComponent<Image>();
			this.regionIcons[i] = component;
			this.SetTerritoryState(i, this.campaignregionwaypoints[i].currentFaction);
		}
		for (int j = 0; j < this.firstOccupiedTerritory.Length; j++)
		{
			string newFaction = "FRIENDLY";
			if (this.campaignregionwaypoints[j].faction == "FRIENDLY")
			{
				newFaction = "ENEMY";
			}
			this.SetTerritoryState(this.firstOccupiedTerritory[j], newFaction);
			for (int k = 0; k < this.campaignlocations.Length; k++)
			{
				if (this.campaignlocations[k].linksToRegionWaypoint == this.firstOccupiedTerritory[j])
				{
					this.InvadeLocation(k);
					break;
				}
			}
		}
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x000330F4 File Offset: 0x000312F4
	public void SetupPlayerSubmarine()
	{
		this.playerMapSpeeds = new float[3];
		this.playerMapSpeeds[0] = UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].telegraphSpeeds[this.playerStartTelegraphs[0]] * this.mapSpeedModifier;
		this.playerMapSpeeds[1] = UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].telegraphSpeeds[this.playerStartTelegraphs[1]] * this.mapSpeedModifier;
		this.playerMapSpeeds[2] = UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].telegraphSpeeds[this.playerStartTelegraphs[2]] * this.mapSpeedModifier;
		this.playercampaigndata.playerNoisemakersOnBoard = UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].numberofnoisemakers;
		this.playercampaigndata.playerTorpeodesOnBoard = new int[UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].torpedoNumbers.Length];
		for (int i = 0; i < UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].torpedoNumbers.Length; i++)
		{
			this.playercampaigndata.playerTorpeodesOnBoard[i] = UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].torpedoNumbers[i];
		}
		if (UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].vlsTorpedoGameObjects != null)
		{
			this.playercampaigndata.playerVLSTorpeodesOnBoard = new int[UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].vlsTorpedoNumbers.Length];
			for (int j = 0; j < UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].vlsTorpedoNumbers.Length; j++)
			{
				this.playercampaigndata.playerVLSTorpeodesOnBoard[j] = UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].vlsTorpedoNumbers[j];
			}
		}
		this.playercampaigndata.playerTubeStatus = new int[UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].torpedoConfig.Length];
		this.playercampaigndata.playerWeaponInTube = new int[UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].torpedoConfig.Length];
		this.playercampaigndata.playerSettingsOne = new int[UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].torpedoConfig.Length];
		this.playercampaigndata.playerSettingsTwo = new int[UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].torpedoConfig.Length];
		this.playercampaigndata.playerSettingsThree = new int[UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].torpedoConfig.Length];
	}

	// Token: 0x06000696 RID: 1686 RVA: 0x00033468 File Offset: 0x00031668
	public void InitialiseAircraft()
	{
		this.campaignAircraftObjects = new GameObject[this.campaignlocations.Length];
		this.aircraftPrepTime = new float[this.campaignlocations.Length];
		for (int i = 0; i < this.campaignlocations.Length; i++)
		{
			if (this.GetHasLocationFunction(i, "AIRBASE"))
			{
				this.campaignlocations[i].hasAirbase = true;
			}
		}
		for (int j = 0; j < this.campaignlocations.Length; j++)
		{
			if (this.campaignlocations[j].hasAirbase)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(this.mapReconObject, this.reconLayer.position, Quaternion.identity) as GameObject;
				this.campaignAircraftObjects[j] = gameObject;
				gameObject.GetComponent<SphereCollider>().radius = this.campaignaircraft[this.campaignlocations[j].aircraftType].detectionRange / 10.25f;
				this.SetAircraftFaction(j);
				gameObject.transform.SetParent(this.reconLayer, false);
				this.aircraftPrepTime[j] = 3f + UnityEngine.Random.Range(0f, this.campaignlocations[j].aircraftFrequency);
				this.campaignlocations[j].aircraftTimer = UnityEngine.Random.Range(0f, this.aircraftPrepTime[j]);
				if (UnityEngine.Random.value < 0.5f)
				{
					this.SetAircraftWaypoints(j);
					this.campaignAircraftObjects[j].transform.Translate(Vector3.up * UnityEngine.Random.Range(0f, 55f));
					this.campaignlocations[j].currentWaypoint = 0;
					if (UnityEngine.Random.value < 0.5f)
					{
						this.campaignlocations[j].currentWaypoint = 1;
					}
				}
			}
		}
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x0003361C File Offset: 0x0003181C
	private void SetAircraftFaction(int i)
	{
		Image componentInChildren = this.campaignAircraftObjects[i].GetComponentInChildren<Image>();
		if (this.campaignlocations[i].faction == "FRIENDLY" || this.campaignlocations[i].faction == "ENEMY")
		{
			componentInChildren.sprite = this.campaignaircraft[this.campaignlocations[i].aircraftType].reconSprite;
		}
		else
		{
			componentInChildren.sprite = this.campaignaircraft[this.campaignlocations[i].aircraftTypeInvaded].reconSprite;
		}
		if (this.campaignlocations[i].faction.Contains("FRIENDLY"))
		{
			componentInChildren.color = this.playerIconColor;
			this.campaignAircraftObjects[i].name = "FRIENDLY";
		}
		else
		{
			componentInChildren.color = this.enemyIconColor;
			this.campaignAircraftObjects[i].name = "ENEMY";
		}
	}

	// Token: 0x06000698 RID: 1688 RVA: 0x00033710 File Offset: 0x00031910
	public void CreatePlayer()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(this.mapTaskForceObject, this.reconLayer.position, Quaternion.identity) as GameObject;
		this.playerGameObject = gameObject;
		this.playerGameObject.name = "PLAYER_SUBMARINE";
		this.playerGameObject.GetComponent<SphereCollider>().radius = 12f;
		this.playerGameObjectIcon = this.playerGameObject.GetComponentInChildren<Image>();
		this.playerGameObjectIcon.sprite = this.reconImages[1];
		this.playerGameObjectIcon.transform.localRotation = Quaternion.identity;
		this.playerGameObject.transform.localPosition = new Vector3(0f, 0f, -5f);
		this.playerGameObjectIcon.color = this.playerIconColor;
		this.playerGameObject.transform.SetParent(this.shipLayer, false);
		gameObject.transform.localPosition = new Vector3(this.playerBasePosition.x, this.playerBasePosition.y, 0f);
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x0003381C File Offset: 0x00031A1C
	public void SetCampaignBriefingButtons()
	{
		string text = "NONE";
		if (CampaignManager.isEncounter)
		{
			text = "ENCOUNTER";
		}
		else if (this.playerInPort)
		{
			text = "PORT";
		}
		UIFunctions.globaluifunctions.missionmanager.backButton.SetActive(false);
		base.gameObject.SetActive(false);
		UIFunctions.globaluifunctions.HUDCamera.SetActive(true);
		UIFunctions.globaluifunctions.HUDCameraObject.enabled = true;
		string text2 = text;
		switch (text2)
		{
		case "PORT":
			UIFunctions.globaluifunctions.missionmanager.battleStationsButton.SetActive(false);
			UIFunctions.globaluifunctions.missionmanager.closureButton.SetActive(false);
			UIFunctions.globaluifunctions.missionmanager.continueCourseButton.SetActive(false);
			UIFunctions.globaluifunctions.missionmanager.leavePortButton.SetActive(true);
			UIFunctions.globaluifunctions.missionmanager.rearmButton.SetActive(true);
			UIFunctions.globaluifunctions.missionmanager.statusButton.SetActive(false);
			UIFunctions.globaluifunctions.missionmanager.backButton.SetActive(false);
			UIFunctions.globaluifunctions.BuildBriefingHeader();
			this.PortTopMenu();
			Time.timeScale = 1f;
			base.gameObject.SetActive(true);
			base.enabled = true;
			UIFunctions.globaluifunctions.textparser.SetImageSprite(this.portImage, UIFunctions.globaluifunctions.backgroundImage);
			UIFunctions.globaluifunctions.backgroundImagesOnly.SetActive(true);
			UIFunctions.globaluifunctions.playerfunctions.damagecontrol.timeToRepairReadout.gameObject.SetActive(true);
			UIFunctions.globaluifunctions.playerfunctions.damagecontrol.GetTimeToRepair();
			break;
		case "ENCOUNTER":
			UIFunctions.globaluifunctions.missionmanager.battleStationsButton.SetActive(true);
			UIFunctions.globaluifunctions.missionmanager.closureButton.SetActive(true);
			UIFunctions.globaluifunctions.missionmanager.continueCourseButton.SetActive(false);
			UIFunctions.globaluifunctions.missionmanager.leavePortButton.SetActive(false);
			UIFunctions.globaluifunctions.missionmanager.rearmButton.SetActive(false);
			UIFunctions.globaluifunctions.missionmanager.statusButton.SetActive(true);
			UIFunctions.globaluifunctions.missionmanager.backButton.SetActive(false);
			this.BringInDisabledCampaignManager();
			this.strategicMapToolbar.SetActive(false);
			SaveLoadManager.AutoSaveCampaign();
			break;
		case "NONE":
			UIFunctions.globaluifunctions.missionmanager.battleStationsButton.SetActive(false);
			UIFunctions.globaluifunctions.missionmanager.closureButton.SetActive(false);
			UIFunctions.globaluifunctions.missionmanager.continueCourseButton.SetActive(true);
			UIFunctions.globaluifunctions.missionmanager.leavePortButton.SetActive(false);
			UIFunctions.globaluifunctions.missionmanager.rearmButton.SetActive(false);
			UIFunctions.globaluifunctions.missionmanager.statusButton.SetActive(true);
			UIFunctions.globaluifunctions.missionmanager.backButton.SetActive(false);
			break;
		}
		this.SetPortControls(this.playerInPort);
		if (!this.playerInPort)
		{
			UIFunctions.globaluifunctions.textparser.SetImageSprite(this.transitImage, UIFunctions.globaluifunctions.backgroundImage);
			UIFunctions.globaluifunctions.backgroundImagesOnly.SetActive(true);
		}
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x00033BB8 File Offset: 0x00031DB8
	public void OpenPassiveBriefingMenu()
	{
		Time.timeScale = 0f;
		UIFunctions.globaluifunctions.SetMenuSystem("BRIEFING");
		this.BringInDisabledCampaignManager();
		UIFunctions.globaluifunctions.playerfunctions.statusscreens.statusBackButton.SetActive(true);
		this.GetPlayerCampaignData();
		UIFunctions.globaluifunctions.HUDCamera.SetActive(true);
		UIFunctions.globaluifunctions.HUDCameraObject.enabled = true;
		if (!this.playerInPort)
		{
			UIFunctions.globaluifunctions.missionmanager.battleStationsButton.SetActive(false);
			UIFunctions.globaluifunctions.missionmanager.closureButton.SetActive(false);
			UIFunctions.globaluifunctions.missionmanager.continueCourseButton.SetActive(true);
			UIFunctions.globaluifunctions.missionmanager.rearmButton.SetActive(false);
			UIFunctions.globaluifunctions.missionmanager.statusButton.SetActive(true);
			UIFunctions.globaluifunctions.missionmanager.leavePortButton.SetActive(false);
			UIFunctions.globaluifunctions.playerfunctions.statusscreens.BackToBriefing();
		}
		else
		{
			UIFunctions.globaluifunctions.BuildBriefingHeader();
			this.PortTopMenu();
			UIFunctions.globaluifunctions.backgroundTemplate.SetActive(true);
			UIFunctions.globaluifunctions.missionmanager.battleStationsButton.SetActive(false);
			UIFunctions.globaluifunctions.missionmanager.closureButton.SetActive(false);
			UIFunctions.globaluifunctions.missionmanager.continueCourseButton.SetActive(false);
			UIFunctions.globaluifunctions.missionmanager.leavePortButton.SetActive(true);
			UIFunctions.globaluifunctions.missionmanager.rearmButton.SetActive(true);
			UIFunctions.globaluifunctions.missionmanager.statusButton.SetActive(false);
			UIFunctions.globaluifunctions.portRearm.portControls.SetActive(true);
			UIFunctions.globaluifunctions.portRearm.combatControls.SetActive(false);
			Time.timeScale = 1f;
			base.gameObject.SetActive(true);
			base.enabled = true;
			UIFunctions.globaluifunctions.textparser.SetImageSprite(this.portImage, UIFunctions.globaluifunctions.backgroundImage);
			UIFunctions.globaluifunctions.backgroundImagesOnly.SetActive(true);
			UIFunctions.globaluifunctions.playerfunctions.damagecontrol.timeToRepairReadout.gameObject.SetActive(true);
			UIFunctions.globaluifunctions.playerfunctions.damagecontrol.GetTimeToRepair();
			Time.timeScale = 1f;
		}
		this.SetPortControls(this.playerInPort);
	}

	// Token: 0x0600069B RID: 1691 RVA: 0x00033E24 File Offset: 0x00032024
	public void PortTopMenu()
	{
		if (GameDataManager.missionMode || GameDataManager.trainingMode)
		{
			return;
		}
		UIFunctions.globaluifunctions.mainColumn.text = string.Empty;
		UIFunctions.globaluifunctions.secondColumm.text = string.Empty;
		foreach (string str in UIFunctions.globaluifunctions.textparser.OpenTextDataFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("language/message/briefing_port")))
		{
			Text mainColumn = UIFunctions.globaluifunctions.mainColumn;
			mainColumn.text = mainColumn.text + str + "\n";
		}
		bool flag = false;
		int num = 0;
		if (UIFunctions.globaluifunctions.playerfunctions.playerVessel.vesselmovement.weaponSource.hasVLS)
		{
			num = 1;
			int num2 = 0;
			for (int j = 0; j < UIFunctions.globaluifunctions.playerfunctions.playerVessel.databaseshipdata.vlsTorpedoNumbers.Length; j++)
			{
				num2 += UIFunctions.globaluifunctions.playerfunctions.playerVessel.databaseshipdata.vlsTorpedoNumbers[j];
			}
			int num3 = 0;
			for (int k = 0; k < UIFunctions.globaluifunctions.playerfunctions.playerVessel.vesselmovement.weaponSource.vlsCurrentTorpsOnBoard.Length; k++)
			{
				num3 += UIFunctions.globaluifunctions.playerfunctions.playerVessel.vesselmovement.weaponSource.vlsCurrentTorpsOnBoard[k];
			}
			if (num3 != num2)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			int num4 = 0;
			if (UIFunctions.globaluifunctions.playerfunctions.playerVessel.vesselmovement.weaponSource.sealsOnBoard)
			{
				num4 = UIFunctions.globaluifunctions.playerfunctions.playerVessel.databaseshipdata.torpedoConfig.Length - num;
			}
			else
			{
				for (int l = 0; l < UIFunctions.globaluifunctions.playerfunctions.playerVessel.databaseshipdata.torpedoNumbers.Length; l++)
				{
					num4 += UIFunctions.globaluifunctions.playerfunctions.playerVessel.databaseshipdata.torpedoNumbers[l];
				}
			}
			int num5 = 0;
			for (int m = 0; m < UIFunctions.globaluifunctions.playerfunctions.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard.Length; m++)
			{
				num5 += UIFunctions.globaluifunctions.playerfunctions.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[m];
			}
			if (num4 != num5)
			{
				flag = true;
			}
		}
		if (flag)
		{
			UIFunctions.globaluifunctions.mainColumn.text = UIFunctions.globaluifunctions.mainColumn.text.Replace("<STORESSTATUS>", "   <color=red><b>" + LanguageManager.interfaceDictionary["StoresStatusNeg"] + "</b></color>");
		}
		else
		{
			UIFunctions.globaluifunctions.mainColumn.text = UIFunctions.globaluifunctions.mainColumn.text.Replace("<STORESSTATUS>", "   <color=lime><b>" + LanguageManager.interfaceDictionary["StoresStatus"] + "</b></color>");
		}
		flag = false;
		foreach (float num6 in UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlCurrentTimers)
		{
			if (num6 == 10000f)
			{
				flag = true;
			}
		}
		if (flag)
		{
			UIFunctions.globaluifunctions.mainColumn.text = UIFunctions.globaluifunctions.mainColumn.text.Replace("<SYSTEMSSTATUS>", "   <color=red><b>" + LanguageManager.interfaceDictionary["SystemsStatusNeg"] + "</b></color>");
		}
		else
		{
			UIFunctions.globaluifunctions.mainColumn.text = UIFunctions.globaluifunctions.mainColumn.text.Replace("<SYSTEMSSTATUS>", "   <color=lime><b>" + LanguageManager.interfaceDictionary["SystemsStatus"] + "</b></color>");
		}
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x00034234 File Offset: 0x00032434
	public void SetPortControls(bool on)
	{
		UIFunctions.globaluifunctions.portRearm.portControls.SetActive(on);
		UIFunctions.globaluifunctions.playerfunctions.damagecontrol.timeToRepairReadout.gameObject.SetActive(on);
		UIFunctions.globaluifunctions.portRearm.combatControls.SetActive(!on);
		UIFunctions.globaluifunctions.playerfunctions.CloseAllContextualPanels();
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x0003429C File Offset: 0x0003249C
	public void OpenMissionBriefing(int taskforceID)
	{
		UIFunctions.globaluifunctions.missionmanager.currentMission = 0;
		UIFunctions.globaluifunctions.missionmanager.currentMissionID = 0;
		if (!CampaignManager.isEncounter)
		{
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.useTerrain = false;
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.usePresetPositions = false;
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.noOtherVessels = true;
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.enemyShipBehaviour = new string[]
			{
				"DEFENSIVE"
			};
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.enemyUnitMissionCritical = new bool[1];
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.kmShipClasses = new int[]
			{
				UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].shipID
			};
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.kmShipInstances = new int[1];
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.kmSlots = new int[1];
			GameDataManager.enemyNumberofShips = 1;
			UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.TacticalMapCleanup();
		}
		else
		{
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.noOtherVessels = false;
			this.PopulateLevelLoadData(taskforceID);
		}
		UIFunctions.globaluifunctions.missionmanager.LoadTraining();
		this.GetPlayerCampaignData();
		GameDataManager.missionMode = false;
		GameDataManager.trainingMode = false;
		if (!CampaignManager.isEncounter)
		{
			GameDataManager.playervesselsonlevel[0].vesselmovement.enabled = false;
			GameDataManager.enemyvesselsonlevel[0].gameObject.SetActive(false);
			UIFunctions.globaluifunctions.levelloadmanager.cetoOcean.gameObject.SetActive(false);
		}
		this.SetCampaignBriefingButtons();
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x00034478 File Offset: 0x00032678
	public string GetNearestLocationToPlayer()
	{
		float num = 10000f;
		bool flag = true;
		int num2 = -1;
		string text = string.Empty;
		for (int i = 0; i < this.campaignlocations.Length; i++)
		{
			float num3 = Vector3.Distance(this.playerGameObject.transform.localPosition, new Vector3(this.campaignlocations[i].baseLocation.x, this.campaignlocations[i].baseLocation.y, this.playerGameObject.transform.localPosition.z));
			if (num3 < num)
			{
				num = num3;
				num2 = i;
			}
		}
		for (int j = 0; j < this.campaignmapwaypoints.Length; j++)
		{
			float num4 = Vector3.Distance(this.playerGameObject.transform.localPosition, new Vector3(this.campaignmapwaypoints[j].waypointPosition.x, this.campaignmapwaypoints[j].waypointPosition.y, this.playerGameObject.transform.localPosition.z));
			if (num4 < num)
			{
				num = num4;
				num2 = j;
				flag = false;
			}
		}
		if (flag)
		{
			text = this.campaignlocations[num2].locationName + ", " + this.campaignlocations[num2].country;
		}
		else
		{
			text = this.campaignmapwaypoints[num2].waypointDescriptiveName;
		}
		this.eventManager.playerMapRegion = text;
		this.eventManager.playerMapRegionIsLocation = flag;
		if (num2 != -1)
		{
			return text;
		}
		return null;
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x00034604 File Offset: 0x00032804
	private void PopulateLevelLoadData(int taskforceID)
	{
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.rnShipClasses = new int[1];
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.rnShipInstances = new int[1];
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.rnSlots = new int[1];
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.rnShipTelegraph = -10;
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.rnShipDepth = -1f;
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.missionPosition.x = -10000f;
		int num = 0;
		for (int i = 0; i < this.campaignTaskForces[taskforceID].shipNumbers.Length; i++)
		{
			num += this.campaignTaskForces[taskforceID].shipNumbers[i];
		}
		if (num > 10)
		{
			num = 10;
		}
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.kmShipClasses = new int[num];
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.enemyUnitMissionCritical = this.campaignTaskForces[taskforceID].missionCriticalVessels;
		for (int j = 0; j < UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.kmShipClasses.Length; j++)
		{
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.kmShipClasses[j] = UIFunctions.globaluifunctions.textparser.GetShipID(this.campaignTaskForces[taskforceID].shipClasses[j]);
		}
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.usePresetPositions = false;
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.formationCruiseSpeed = this.campaignTaskForces[taskforceID].speed / this.mapSpeedModifier;
		float x = this.campaignTaskForceObjects[taskforceID].transform.localPosition.x - this.playerGameObject.transform.localPosition.x;
		float z = this.campaignTaskForceObjects[taskforceID].transform.localPosition.y - this.playerGameObject.transform.localPosition.y;
		UIFunctions.globaluifunctions.directionFinder.transform.position = UIFunctions.globaluifunctions.levelloadmanager.spawnObjects[0].transform.position;
		UIFunctions.globaluifunctions.directionFinder.transform.LookAt(new Vector3(x, 1000f, z));
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.globalSpawnRotation = UIFunctions.globaluifunctions.directionFinder.transform.eulerAngles.y - 90f;
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.localSpawnRotation.x = this.GetStrategicMapHeading(this.playerGameObject.transform.localEulerAngles.x, this.playerGameObject.transform.localEulerAngles.y);
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.localSpawnRotation.y = this.GetStrategicMapHeading(this.campaignTaskForceObjects[taskforceID].transform.localEulerAngles.x, this.campaignTaskForceObjects[taskforceID].transform.localEulerAngles.y);
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.distance = 50f;
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.month = this.currentHourMonth[1].ToString();
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.season = CalendarFunctions.GetSeason(CalendarFunctions.GetFullMonth(this.currentHourMonth[1], true, true), this.GetCampaignHemisphere());
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.timeofday = this.GetCampaignTimeOfDay(this.currentHourMonth[0]);
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.GetHelicopterNumbers();
		this.GetFixedWingAircraftNumbers();
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.AssignWhales();
		if (this.playercampaigndata.currentMissionTaskForceID == taskforceID && this.campaignTaskForces[taskforceID].behaviourType == "STATIONARY" && this.campaignTaskForces[taskforceID].endIsLocation)
		{
			string filePathFromString = UIFunctions.globaluifunctions.textparser.GetFilePathFromString(string.Concat(new string[]
			{
				"campaign/",
				CampaignManager.campaignReferenceName,
				"/missions/",
				this.campaignmissions[this.campaignTaskForces[taskforceID].missionID].missionType,
				"_LOCATION_",
				this.campaignTaskForces[taskforceID].endLocationID.ToString()
			}));
			UIFunctions.globaluifunctions.textparser.PopulateLevelLoadData(UIFunctions.globaluifunctions.textparser.GetFilePathFromString(filePathFromString), null);
		}
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x00034AE4 File Offset: 0x00032CE4
	private void GetFixedWingAircraftNumbers()
	{
		if (UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.usePresetPositions)
		{
			return;
		}
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.numberOfAircraft = 0;
		if (UIFunctions.globaluifunctions.levelloadmanager.GetIsSubmarinesOnly())
		{
			return;
		}
		float num = this.mapHeightInNM / 780f;
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.numberOfAircraft = 0;
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.aircraftType = string.Empty;
		float num2 = 10000f;
		float num3 = 10000f;
		for (int i = 0; i < this.campaignlocations.Length; i++)
		{
			if ((this.campaignlocations[i].faction == "ENEMY" || this.campaignlocations[i].faction == "ENEMY_OCCUPIED") && this.campaignlocations[i].hasAirbase && this.campaignlocations[i].aircraftTimer >= 0f)
			{
				Vector3 vector = this.campaignlocations[i].baseLocation;
				float num4 = Vector3.Distance(this.playerGameObject.transform.localPosition, new Vector3(vector.x, vector.y, this.playerGameObject.transform.localPosition.z)) * num;
				if (num4 < num2)
				{
					num2 = num4;
				}
			}
		}
		for (int j = 0; j < this.campaignAircraftObjects.Length; j++)
		{
			if (this.campaignAircraftObjects[j] != null && this.campaignAircraftObjects[j].name == "ENEMY" && this.campaignAircraftObjects[j].activeSelf)
			{
				float num5 = Vector3.Distance(this.playerGameObject.transform.localPosition, this.campaignAircraftObjects[j].transform.localPosition) * num;
				if (num5 < num3)
				{
					num3 = num5;
				}
			}
		}
		int num6 = 0;
		if (this.playercampaigndata.playerRevealed > 0f)
		{
			num6 = 100;
		}
		if (num2 < (float)(100 + num6))
		{
			if (UnityEngine.Random.value < 0.8f)
			{
				UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.numberOfAircraft++;
			}
		}
		else if (num2 < (float)(200 + num6))
		{
			if (UnityEngine.Random.value < 0.4f)
			{
				UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.numberOfAircraft++;
			}
		}
		else if (num2 < (float)(400 + num6) && UnityEngine.Random.value < 0.2f)
		{
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.numberOfAircraft++;
		}
		if (num3 < (float)(100 + num6))
		{
			if (UnityEngine.Random.value < 0.8f)
			{
				UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.numberOfAircraft++;
			}
		}
		else if (num3 < (float)(200 + num6))
		{
			if (UnityEngine.Random.value < 0.4f)
			{
				UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.numberOfAircraft++;
			}
		}
		else if (num3 < (float)(400 + num6) && UnityEngine.Random.value < 0.2f)
		{
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.numberOfAircraft++;
		}
		if (UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.numberOfAircraft > 0)
		{
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.numberOfAircraft = 1;
			List<int> list = new List<int>();
			for (int k = 0; k < this.campaignaircraft.Length; k++)
			{
				if (this.campaignaircraft[k].faction == "ENEMY")
				{
					list.Add(k);
				}
			}
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.aircraftType = this.campaignaircraft[list[UnityEngine.Random.Range(0, list.Count)]].reconName;
		}
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x00034F28 File Offset: 0x00033128
	private string GetCampaignHemisphere()
	{
		if (this.hemisphere == "NORTH" || this.hemisphere == "SOUTH")
		{
			return this.hemisphere;
		}
		if (this.playerGameObject.transform.position.y > this.equatorYValue)
		{
			return "NORTH";
		}
		return "SOUTH";
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x00034F94 File Offset: 0x00033194
	public int GetCampaignTimeOfDay(int hour)
	{
		int result = 5;
		if (UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.season == "Spring" || UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.season == "Autumn")
		{
			if (hour > 19)
			{
				result = 5;
			}
			else if (hour > 16)
			{
				result = 4;
			}
			else if (hour > 13)
			{
				result = 3;
			}
			else if (hour > 11)
			{
				result = 2;
			}
			else if (hour > 8)
			{
				result = 1;
			}
			else if (hour > 6)
			{
				result = 0;
			}
		}
		else if (UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.season == "Summer")
		{
			if (hour > 21)
			{
				result = 5;
			}
			else if (hour > 18)
			{
				result = 4;
			}
			else if (hour > 14)
			{
				result = 3;
			}
			else if (hour > 10)
			{
				result = 2;
			}
			else if (hour > 7)
			{
				result = 1;
			}
			else if (hour > 5)
			{
				result = 0;
			}
		}
		else if (hour > 17)
		{
			result = 5;
		}
		else if (hour > 15)
		{
			result = 4;
		}
		else if (hour > 13)
		{
			result = 3;
		}
		else if (hour > 11)
		{
			result = 2;
		}
		else if (hour > 9)
		{
			result = 1;
		}
		else if (hour > 7)
		{
			result = 0;
		}
		return result;
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x00035114 File Offset: 0x00033314
	private float GetStrategicMapHeading(float xrot, float yrot)
	{
		xrot += 90f;
		if (yrot > 180f)
		{
			xrot = 360f - xrot;
			if (xrot < 360f)
			{
				xrot += 360f;
			}
		}
		if (xrot > 360f)
		{
			xrot -= 360f;
		}
		return xrot;
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x00035168 File Offset: 0x00033368
	public void CampaignEngagement(int taskforceID)
	{
		if (this.playerInPort)
		{
			return;
		}
		UIFunctions.globaluifunctions.CleanupLevel();
		CampaignManager.isEncounter = true;
		this.isThisALandStrike = false;
		if (taskforceID == this.playercampaigndata.currentMissionTaskForceID && this.playercampaigndata.playerMissionType == "LAND_STRIKE")
		{
			this.isThisALandStrike = true;
		}
		this.OpenMissionBriefing(taskforceID);
		this.currentTaskForceEngagedWith = taskforceID;
	}

	// Token: 0x060006A5 RID: 1701 RVA: 0x000351D8 File Offset: 0x000333D8
	private bool GetHasLocationFunction(int locationIndex, string locationFunction)
	{
		return this.campaignlocations[locationIndex].function.Contains(locationFunction);
	}

	// Token: 0x060006A6 RID: 1702 RVA: 0x000351F8 File Offset: 0x000333F8
	public void GenerateCampaignMission(bool isPlayerMission)
	{
		bool isFinalMission = false;
		if (isPlayerMission && (this.campaignPoints <= 0f || this.campaignPoints >= this.totalCampaignPoints))
		{
			isFinalMission = true;
		}
		int nextFreeTaskForce = this.GetNextFreeTaskForce();
		int num = 0;
		bool missionData;
		do
		{
			missionData = this.GetMissionData(nextFreeTaskForce, isPlayerMission, isFinalMission, string.Empty);
			num++;
		}
		while (!missionData && num < 10);
		if (num == 10 && isPlayerMission)
		{
			this.GetMissionData(nextFreeTaskForce, isPlayerMission, isFinalMission, this.defaultMissionTypes[UnityEngine.Random.Range(0, this.defaultMissionTypes.Length)]);
		}
		if (isPlayerMission)
		{
			this.missionGivenOnDate = this.GetMissionIDDateText();
			this.playercampaigndata.currentMissionTaskForceID = nextFreeTaskForce;
			this.playercampaigndata.playerHasMission = true;
			CampaignManager.isEncounter = false;
			this.OpenMissionBriefing(nextFreeTaskForce);
			UIFunctions.globaluifunctions.playerfunctions.statusscreens.OpenStatusScreens(2);
			AudioManager.audiomanager.PlayMusicClip(1, string.Empty);
		}
		if (isPlayerMission)
		{
			SaveLoadManager.AutoSaveCampaign();
		}
	}

	// Token: 0x060006A7 RID: 1703 RVA: 0x000352F0 File Offset: 0x000334F0
	private string GetMissionIDDateText()
	{
		int num = UnityEngine.Random.Range(100, 1000);
		int num2 = UnityEngine.Random.Range(100, 1000);
		int num3 = UnityEngine.Random.Range(100, 1000);
		string text = string.Concat(new string[]
		{
			"\n\n\t\t\t<b><color=red>TOP SECRET</color></b>\n\n\t\t\t#0-",
			num.ToString(),
			".",
			num2.ToString(),
			".",
			num3.ToString()
		});
		string text2 = text;
		return string.Concat(new string[]
		{
			text2,
			"\n\t\t\t",
			this.dateDisplay.text,
			"   ",
			this.timeDisplay.text
		});
	}

	// Token: 0x060006A8 RID: 1704 RVA: 0x000353A8 File Offset: 0x000335A8
	private int GetNextFreeTaskForce()
	{
		for (int i = 0; i < this.activeTaskForces.Length; i++)
		{
			if (!this.activeTaskForces[i] && this.playercampaigndata.currentMissionTaskForceID != i && this.taskForceActivateTimers[i] <= 0f)
			{
				return i;
			}
		}
		float num = 0f;
		int result = 0;
		for (int j = 0; j < this.campaignTaskForceObjects.Length; j++)
		{
			if (this.playercampaigndata.currentMissionTaskForceID != j)
			{
				float num2 = Vector3.Distance(this.campaignTaskForceObjects[j].transform.position, this.playerGameObject.transform.position);
				if (num2 > num)
				{
					num = num2;
					result = j;
				}
			}
		}
		return result;
	}

	// Token: 0x060006A9 RID: 1705 RVA: 0x0003546C File Offset: 0x0003366C
	private int GetMissionType(int missionForcedNumber)
	{
		float value = UnityEngine.Random.value;
		float num = 0f;
		for (int i = 0; i < this.playerMissionFreqs.Length; i++)
		{
			num += this.playerMissionFreqs[i];
			if (value <= num)
			{
				return i;
			}
		}
		return 0;
	}

	// Token: 0x060006AA RID: 1706 RVA: 0x000354B4 File Offset: 0x000336B4
	private bool GetMissionData(int taskforceID, bool isPlayerMission, bool isFinalMission, string defaultMissionType = "")
	{
		bool flag = false;
		if (UnityEngine.Random.value < 0.5f)
		{
		}
		int num = -1;
		if (isPlayerMission || flag)
		{
			int num2 = -1;
			string b = "RETURN_TO_BASE";
			for (int i = 0; i < this.campaignMissionTypes.Length; i++)
			{
				if (this.campaignMissionTypes[i] == b)
				{
					num2 = i;
					break;
				}
			}
			int num3 = 0;
			bool flag2;
			do
			{
				flag2 = true;
				num = this.GetMissionType(num2);
				float num4 = this.campaignPoints / this.totalCampaignPoints;
				if (this.playerMissionThresholds[num] > 1f - num4)
				{
					flag2 = false;
				}
				if (this.campaignMissionTypes[num] == this.playercampaigndata.playerMissionType)
				{
					flag2 = false;
				}
				num3++;
			}
			while (!flag2 && num3 < 10);
			if (num3 == 10)
			{
				num = 0;
				string a = this.defaultMissionTypes[UnityEngine.Random.Range(0, this.defaultMissionTypes.Length)];
				for (int j = 0; j < this.campaignMissionTypes.Length; j++)
				{
					if (a == this.campaignMissionTypes[j])
					{
						num = j;
					}
				}
			}
			if (isPlayerMission)
			{
				if (num2 != -1 && !this.playerInPort)
				{
					int num5 = 0;
					for (int k = 0; k < GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.currentTorpsOnBoard.Length; k++)
					{
						num5 += GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.currentTorpsOnBoard[k];
					}
					if (num5 < UnityEngine.Random.Range(8, 13))
					{
						num = num2;
					}
					if (GameDataManager.playervesselsonlevel[0].damagesystem.shipCurrentDamagePoints / GameDataManager.playervesselsonlevel[0].damagesystem.shipTotalDamagePoints > 0.5f)
					{
						num = num2;
					}
				}
				if (isFinalMission && num != num2)
				{
					num = this.numberofPlayerMissionTypes - 1;
				}
				if (defaultMissionType != string.Empty)
				{
					for (int l = 0; l < this.campaignMissionTypes.Length; l++)
					{
						if (this.campaignMissionTypes[l] == defaultMissionType)
						{
							num = l;
							break;
						}
					}
				}
				else if (this.campaignDebugForcedMission != string.Empty)
				{
					for (int m = 0; m < this.campaignMissionTypes.Length; m++)
					{
						if (this.campaignMissionTypes[m] == this.campaignDebugForcedMission)
						{
							num = m;
							break;
						}
					}
				}
			}
		}
		else
		{
			float value = UnityEngine.Random.value;
			float num6 = 0f;
			for (int n = 0; n < this.nonPlayerMissionFreqs.Length; n++)
			{
				num6 += this.nonPlayerMissionFreqs[n];
				if (value <= num6)
				{
					num = n + this.numberofPlayerMissionTypes;
					break;
				}
			}
		}
		int[] arrayOfMissions = this.GetArrayOfMissions(this.campaignMissionTypes[num]);
		int num7 = UnityEngine.Random.Range(0, arrayOfMissions.Length);
		if (this.campaignDebugForcedMission != string.Empty && this.campaignDebugForcedMissionNumber != -1 && this.campaignDebugForcedMissionNumber < arrayOfMissions.Length)
		{
			num7 = this.campaignDebugForcedMissionNumber;
		}
		bool flag3 = false;
		if (this.campaignmissions[arrayOfMissions[num7]].requiresWeaponID != -1)
		{
			for (int num8 = 0; num8 < GameDataManager.playervesselsonlevel[0].databaseshipdata.torpedoIDs.Length; num8++)
			{
				if (this.campaignmissions[arrayOfMissions[num7]].requiresWeaponID == GameDataManager.playervesselsonlevel[0].databaseshipdata.torpedoIDs[num8])
				{
					flag3 = true;
				}
			}
			if (GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.hasVLS)
			{
				for (int num9 = 0; num9 < GameDataManager.playervesselsonlevel[0].databaseshipdata.vlsTorpedoIDs.Length; num9++)
				{
					if (this.campaignmissions[arrayOfMissions[num7]].requiresWeaponID == GameDataManager.playervesselsonlevel[0].databaseshipdata.vlsTorpedoIDs[num9])
					{
						flag3 = true;
					}
				}
			}
		}
		else
		{
			flag3 = true;
		}
		if (!flag3)
		{
			return false;
		}
		if (this.campaignmissions[arrayOfMissions[num7]].startAlignment == "ENEMY_OCCUPIED" || this.campaignmissions[arrayOfMissions[num7]].endAlignment == "ENEMY_OCCUPIED")
		{
			int num10 = 0;
			for (int num11 = 0; num11 < this.campaignlocations.Length; num11++)
			{
				if (this.campaignlocations[num11].faction == "ENEMY" && this.campaignlocations[num11].originalFaction == "FRIENDLY")
				{
					num10++;
				}
			}
		}
		int[] array = new int[0];
		int[] array2 = new int[0];
		this.campaignTaskForces[taskforceID].startIsLocation = false;
		this.campaignTaskForces[taskforceID].endIsLocation = false;
		if (this.campaignmissions[arrayOfMissions[num7]].startLocation[0] != "WAYPOINT")
		{
			this.campaignTaskForces[taskforceID].startIsLocation = true;
			array = this.GetArrayOfLocations(this.campaignmissions[arrayOfMissions[num7]].startAlignment, this.campaignmissions[arrayOfMissions[num7]].startLocation, this.campaignMissionTypes[num], true);
		}
		else if (this.campaignmissions[arrayOfMissions[num7]].mustUseWaypoints.Length > 0)
		{
			array = new int[]
			{
				this.campaignmissions[arrayOfMissions[num7]].mustUseWaypoints[0]
			};
		}
		else
		{
			array = this.campaignmissions[arrayOfMissions[num7]].useAtLeastOneWaypointOf;
		}
		if (this.campaignmissions[arrayOfMissions[num7]].endLocation[0] != "WAYPOINT")
		{
			this.campaignTaskForces[taskforceID].endIsLocation = true;
			if (this.campaignmissions[arrayOfMissions[num7]].endAlignment != "ENEMY_OCCUPIED")
			{
				array2 = this.GetArrayOfLocations(this.campaignmissions[arrayOfMissions[num7]].endAlignment, this.campaignmissions[arrayOfMissions[num7]].endLocation, this.campaignMissionTypes[num], false);
			}
			else
			{
				array2 = this.GetArrayOfOccupiedLocations(this.campaignmissions[arrayOfMissions[num7]]);
			}
		}
		else if (this.campaignmissions[arrayOfMissions[num7]].mustUseWaypoints.Length > 0)
		{
			array2 = this.campaignmissions[arrayOfMissions[num7]].mustUseWaypoints;
		}
		else
		{
			array2 = this.campaignmissions[arrayOfMissions[num7]].useAtLeastOneWaypointOf;
		}
		if (array.Length > 0 && array2.Length > 0)
		{
			int num12 = UnityEngine.Random.Range(0, array.Length);
			int num13 = UnityEngine.Random.Range(0, array2.Length);
			CampaignTaskForce campaignTaskForce = this.campaignTaskForces[taskforceID];
			campaignTaskForce.missionType = num;
			campaignTaskForce.actualMission = num7;
			campaignTaskForce.missionID = this.campaignmissions[arrayOfMissions[num7]].missionID;
			campaignTaskForce.legOfRoute = 0;
			campaignTaskForce.mustUseWaypoints = this.campaignmissions[arrayOfMissions[num7]].mustUseWaypoints;
			int[] useAtLeastOneWaypointOf = this.campaignmissions[arrayOfMissions[num7]].useAtLeastOneWaypointOf;
			if (useAtLeastOneWaypointOf.Length > 0)
			{
				int num14 = UnityEngine.Random.Range(0, useAtLeastOneWaypointOf.Length);
				int num15 = useAtLeastOneWaypointOf[num14];
				if (this.campaignmissions[arrayOfMissions[num7]].mustUseWaypoints.Length == 0)
				{
					campaignTaskForce.mustUseWaypoints = new int[]
					{
						num15
					};
				}
				else
				{
					List<int> list = this.campaignmissions[arrayOfMissions[num7]].mustUseWaypoints.ToList<int>();
					if (!list.Contains(num15))
					{
						int[] array3 = new int[campaignTaskForce.mustUseWaypoints.Length + 1];
						array3[0] = num15;
						for (int num16 = 1; num16 < campaignTaskForce.mustUseWaypoints.Length + 1; num16++)
						{
							array3[num16] = campaignTaskForce.mustUseWaypoints[num16 - 1];
						}
						campaignTaskForce.mustUseWaypoints = array3;
					}
				}
			}
			campaignTaskForce.prohibitedWaypoints = this.campaignmissions[arrayOfMissions[num7]].prohibitedWaypoints;
			campaignTaskForce.previousWaypoint = -1;
			campaignTaskForce.currentAction = 1;
			if (this.campaignTaskForces[taskforceID].startIsLocation)
			{
				campaignTaskForce.currentWaypoint = this.campaignlocations[array[num12]].linksToWaypoint[this.campaignlocations[array[num12]].linksToWaypoint.Length - 1];
				campaignTaskForce.startPositionID = array[num12];
				this.campaignTaskForceObjects[taskforceID].transform.localPosition = new Vector3(this.campaignlocations[array[num12]].baseLocation.x, this.campaignlocations[array[num12]].baseLocation.y, 0f);
			}
			else
			{
				campaignTaskForce.currentWaypoint = array[num12];
				campaignTaskForce.startPositionID = array[num12];
				this.campaignTaskForceObjects[taskforceID].transform.localPosition = this.campaignmapwaypoints[campaignTaskForce.currentWaypoint].waypointPosition;
			}
			if (this.campaignTaskForces[taskforceID].endIsLocation)
			{
				campaignTaskForce.mustUseWaypoints = this.campaignlocations[array2[num13]].linksToWaypoint;
				this.campaignTaskForces[taskforceID].finalLocationPosition = this.campaignlocations[array2[num13]].baseLocation;
				this.campaignTaskForces[taskforceID].endLocationID = array2[num13];
			}
			if (this.campaignmissions[arrayOfMissions[num7]].taskForceBehaviour == "STATIONARY")
			{
				if (this.campaignTaskForces[taskforceID].endIsLocation)
				{
					this.campaignTaskForceObjects[taskforceID].transform.localPosition = new Vector3(this.campaignlocations[array2[num13]].baseLocation.x, this.campaignlocations[array2[num13]].baseLocation.y, 0f);
				}
				else
				{
					this.campaignTaskForceObjects[taskforceID].transform.localPosition = this.campaignmapwaypoints[campaignTaskForce.currentWaypoint].waypointPosition;
				}
				this.campaignTaskForces[taskforceID].currentAction = 3;
				float num17 = Vector3.Distance(this.campaignTaskForceObjects[taskforceID].transform.position, this.playerGameObject.transform.position);
				if (num17 < 10f)
				{
					Debug.LogError("TOO CLOSE TO PLAYER " + this.campaignmissions[arrayOfMissions[num7]].missionFileName);
					return false;
				}
				this.campaignTaskForceObjects[taskforceID].transform.Translate(Vector3.up * -500f);
			}
			this.navHelper.transform.localPosition = this.campaignmapwaypoints[campaignTaskForce.currentWaypoint].waypointPosition;
			campaignTaskForce.waypointPosition = this.navHelper.transform.position;
			campaignTaskForce.waypointPosition.z = 125.1605f;
			campaignTaskForce.finalLegDone = false;
			if (this.campaignmissions[arrayOfMissions[num7]].numberOfEnemyUnits != null)
			{
				campaignTaskForce.shipNumbers = new int[this.campaignmissions[arrayOfMissions[num7]].numberOfEnemyUnits.Length];
				int num18 = 0;
				for (int num19 = 0; num19 < campaignTaskForce.shipNumbers.Length; num19++)
				{
					campaignTaskForce.shipNumbers[num19] = (int)UnityEngine.Random.Range(this.campaignmissions[arrayOfMissions[num7]].numberOfEnemyUnits[num19].x, this.campaignmissions[arrayOfMissions[num7]].numberOfEnemyUnits[num19].y);
					num18 += campaignTaskForce.shipNumbers[num19];
				}
				if (num18 > GameDataManager.maxShips)
				{
					num18 = GameDataManager.maxShips;
				}
				campaignTaskForce.shipClasses = new string[num18];
				campaignTaskForce.missionCriticalVessels = new bool[num18];
				campaignTaskForce.defensiveVessels = new bool[num18];
				num18 = 0;
				for (int num20 = 0; num20 < campaignTaskForce.shipNumbers.Length; num20++)
				{
					for (int num21 = 0; num21 < campaignTaskForce.shipNumbers[num20]; num21++)
					{
						if (num18 < 10)
						{
							if (isPlayerMission && this.campaignmissions[arrayOfMissions[num7]].enemyUnitMissionCritical[num20])
							{
								campaignTaskForce.missionCriticalVessels[num18] = true;
							}
							if (this.campaignmissions[arrayOfMissions[num7]].enemyShipBehaviour[num20] == "DEFENSIVE")
							{
								campaignTaskForce.defensiveVessels[num18] = true;
							}
							campaignTaskForce.shipClasses[num18] = this.GetTaskForceShip(this.campaignmissions[arrayOfMissions[num7]].enemyShipClasses[num20]);
							num18++;
						}
					}
				}
				this.SetTaskForceIcon(campaignTaskForce);
				campaignTaskForce.behaviourType = this.campaignmissions[arrayOfMissions[num7]].taskForceBehaviour;
				this.activeTaskForces[taskforceID] = true;
				campaignTaskForce.takeDirectRoute = true;
				campaignTaskForce.patrolForHours = UnityEngine.Random.Range(this.campaignmissions[arrayOfMissions[num7]].patrolForHours.x, this.campaignmissions[arrayOfMissions[num7]].patrolForHours.y);
			}
			else
			{
				this.campaignTaskForceIcons[campaignTaskForce.taskForceID].sprite = null;
				this.campaignTaskForceIcons[campaignTaskForce.taskForceID].gameObject.SetActive(false);
			}
			this.campaignTaskForceObjects[taskforceID].SetActive(true);
			this.campaignTaskForces[taskforceID] = campaignTaskForce;
			this.campaignTaskForces[taskforceID].speed = this.campaignmissions[arrayOfMissions[num7]].speed / 10f * this.mapSpeedModifier;
			if (this.campaignTaskForces[taskforceID].speed == 0f)
			{
				float num22 = 50f;
				for (int num23 = 0; num23 < campaignTaskForce.shipClasses.Length; num23++)
				{
					int shipID = UIFunctions.globaluifunctions.textparser.GetShipID(campaignTaskForce.shipClasses[num23]);
					float num24 = UIFunctions.globaluifunctions.database.databaseshipdata[shipID].surfacespeed;
					if (UIFunctions.globaluifunctions.database.databaseshipdata[shipID].shipType == "SUBMARINE")
					{
						num24 = UIFunctions.globaluifunctions.database.databaseshipdata[shipID].submergedspeed;
					}
					if (num24 < num22)
					{
						num22 = num24;
					}
				}
				campaignTaskForce.speed = num22 / 1.5f;
				if (campaignTaskForce.speed > 20f)
				{
					campaignTaskForce.speed = 16f;
					if (UnityEngine.Random.value < 0.5f)
					{
						campaignTaskForce.speed = 14f;
					}
				}
				else if (campaignTaskForce.speed > 15f)
				{
					campaignTaskForce.speed = 14f;
					if (UnityEngine.Random.value < 0.5f)
					{
						campaignTaskForce.speed = 10f;
					}
				}
				else
				{
					campaignTaskForce.speed = 8f;
				}
				campaignTaskForce.speed /= 10f;
			}
			if (isPlayerMission)
			{
				this.playercampaigndata.playerMissionType = this.campaignmissions[arrayOfMissions[num7]].missionType;
			}
			this.taskForceActivateTimers[taskforceID] = UnityEngine.Random.Range(this.campaignmissions[arrayOfMissions[num7]].hoursToStart.x, this.campaignmissions[arrayOfMissions[num7]].hoursToStart.y);
			if (this.taskForceActivateTimers[taskforceID] > 0f)
			{
				this.activeTaskForces[taskforceID] = false;
				this.campaignTaskForceObjects[taskforceID].SetActive(false);
			}
			this.campaignTaskForceObjects[taskforceID].GetComponent<CampaignMapRevealContact>().timer = 0f;
			return true;
		}
		Debug.LogError("NO STARTS OR ENDS " + this.campaignmissions[arrayOfMissions[num7]].missionFileName);
		return false;
	}

	// Token: 0x060006AB RID: 1707 RVA: 0x00036430 File Offset: 0x00034630
	public void EndCampaignCombat()
	{
		this.DeactivateTaskforce(this.currentTaskForceEngagedWith);
		this.SetPlayerCampaignData();
		CampaignManager.isEncounter = false;
		if (!this.eventManager.missionOverBriefingDisplayed)
		{
			this.eventManager.missionOverBriefingDisplayed = true;
			if (!UIFunctions.globaluifunctions.missionmanager.playerSunk && !UIFunctions.globaluifunctions.missionmanager.abandonedShip)
			{
				UIFunctions.globaluifunctions.playerfunctions.statusscreens.OpenStatusScreens(3);
				return;
			}
		}
		if (!this.eventManager.medalAwarded && this.eventManager.missionPassed)
		{
			for (int i = this.eventManager.patrolAwards.Length - 1; i > -1; i--)
			{
				if (this.playercampaigndata.patrolTonnage >= (float)this.eventManager.patrolTonnage[i] && this.playercampaigndata.patrolMedals[i] != 1)
				{
					this.playercampaigndata.patrolMedals[i] = 1;
					this.eventManager.BringInEvent(this.eventManager.GetEventID(this.eventManager.patrolAwards[i]), true, false);
					this.eventManager.medalAwarded = true;
					this.playercampaigndata.patrolTonnage = 0f;
					return;
				}
			}
			for (int j = this.eventManager.cumulativeAwards.Length - 1; j > -1; j--)
			{
				if (this.playercampaigndata.totalTonnage >= (float)this.eventManager.cumulativeTonnage[j] && this.playercampaigndata.cumulativeMedals[j] != 1 && this.playercampaigndata.campaignStats[0] >= (float)this.eventManager.missionsPassed[j])
				{
					this.playercampaigndata.cumulativeMedals[j] = 1;
					this.eventManager.BringInEvent(this.eventManager.GetEventID(this.eventManager.cumulativeAwards[j]), true, false);
					this.eventManager.medalAwarded = true;
					return;
				}
			}
		}
		if (UIFunctions.globaluifunctions.missionmanager.playerSunk || UIFunctions.globaluifunctions.missionmanager.abandonedShip)
		{
			int eventID = this.eventManager.specialEventIDs[5];
			if (UnityEngine.Random.value < 0.3f)
			{
				eventID = this.eventManager.specialEventIDs[6];
				this.eventManager.endingType = 4;
			}
			if (!UIFunctions.globaluifunctions.missionmanager.abandonedShip)
			{
				eventID = this.eventManager.specialEventIDs[7];
				this.eventManager.playerDead = true;
				this.eventManager.endingType = 5;
			}
			this.eventManager.BringInEvent(eventID, false, false);
			return;
		}
		if (this.eventManager.playerDead)
		{
			this.playercampaigndata.woundedMedals[0]++;
			this.eventManager.BringInEvent(this.eventManager.GetEventID(this.eventManager.woundedAwards[0]), true, false);
			return;
		}
		if (this.playercampaigndata.currentMissionTaskForceID != this.currentTaskForceEngagedWith)
		{
			this.ResetNonPlayerMission();
			this.eventManager.missionOverBriefingDisplayed = false;
			SaveLoadManager.AutoSaveCampaign();
			if (!this.playerInPort)
			{
				this.OpenMissionBriefing(this.playercampaigndata.currentMissionTaskForceID);
				UIFunctions.globaluifunctions.missionmanager.ContinueCourse();
			}
			else
			{
				UIFunctions.globaluifunctions.campaignmanager.OpenPassiveBriefingMenu();
			}
			return;
		}
		if (this.currentTaskForceEngagedWith == this.playercampaigndata.currentMissionTaskForceID)
		{
			this.playercampaigndata.playerHasMission = false;
			if (this.campaignmissions[this.campaignTaskForces[this.currentTaskForceEngagedWith].missionID].finalMission)
			{
				bool flag = true;
				if (this.campaignPoints / this.totalCampaignPoints >= 0.5f)
				{
					flag = false;
				}
				if (flag && this.eventManager.missionPassed)
				{
					this.eventManager.BringInEvent(this.eventManager.specialEventIDs[1], false, false);
					this.eventManager.endingType = 0;
				}
				else if (flag)
				{
					this.eventManager.BringInEvent(this.eventManager.specialEventIDs[2], false, false);
					this.eventManager.endingType = 1;
				}
				else if (this.eventManager.missionPassed)
				{
					this.eventManager.BringInEvent(this.eventManager.specialEventIDs[3], false, false);
					this.eventManager.endingType = 2;
				}
				else
				{
					this.eventManager.BringInEvent(this.eventManager.specialEventIDs[4], false, false);
					this.eventManager.endingType = 3;
				}
			}
			else
			{
				if (this.eventManager.missionPassed)
				{
					this.lastMissionCompleteStatus = -1;
				}
				else
				{
					this.lastMissionCompleteStatus = 1;
				}
				string eventString = string.Empty;
				if (this.eventManager.missionPassed)
				{
					eventString = this.campaignmissions[UIFunctions.globaluifunctions.campaignmanager.campaignTaskForces[UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.currentMissionTaskForceID].missionID].eventWin;
				}
				else
				{
					eventString = this.campaignmissions[UIFunctions.globaluifunctions.campaignmanager.campaignTaskForces[UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.currentMissionTaskForceID].missionID].eventFail;
				}
				this.eventManager.BringInEvent(this.eventManager.GetEventID(eventString), false, false);
				this.eventManager.missionOverBriefingDisplayed = false;
			}
			this.DeactivateTaskforce(this.currentTaskForceEngagedWith);
			this.currentTaskForceEngagedWith = -1;
			this.eventManager.missionPassed = false;
			this.eventManager.medalAwarded = false;
			SaveLoadManager.AutoSaveCampaign();
			return;
		}
	}

	// Token: 0x060006AC RID: 1708 RVA: 0x000369C0 File Offset: 0x00034BC0
	private void ResetNonPlayerMission()
	{
		this.DeactivateTaskforce(this.currentTaskForceEngagedWith);
		this.currentTaskForceEngagedWith = -1;
		this.eventManager.missionPassed = false;
		this.eventManager.medalAwarded = false;
		this.GenerateCampaignMission(false);
		UIFunctions.globaluifunctions.SetMenuSystem("CAMPAIGN");
		UIFunctions.globaluifunctions.backgroundTemplate.SetActive(false);
		Time.timeScale = this.mapTimeCompression;
	}

	// Token: 0x060006AD RID: 1709 RVA: 0x00036A2C File Offset: 0x00034C2C
	public int[] GetClassesAvailable()
	{
		UIFunctions.globaluifunctions.keybindManagerMuseum.unitsToDisplay = UIFunctions.globaluifunctions.playerfunctions.playerVesselList;
		if (this.playercampaigndata == null || this.playercampaigndata.playerVesselsLost == null)
		{
			return UIFunctions.globaluifunctions.keybindManagerMuseum.unitsToDisplay;
		}
		int[] array = new int[UIFunctions.globaluifunctions.keybindManagerMuseum.unitsToDisplay.Length];
		List<int> list = new List<int>();
		for (int i = 0; i < UIFunctions.globaluifunctions.keybindManagerMuseum.unitsToDisplay.Length; i++)
		{
			for (int j = 0; j < this.playercampaigndata.playerVesselsLost.Length; j++)
			{
				if (this.playercampaigndata.playerVesselsLost[j] == UIFunctions.globaluifunctions.keybindManagerMuseum.unitsToDisplay[i])
				{
					array[i]++;
				}
			}
		}
		for (int k = 0; k < UIFunctions.globaluifunctions.keybindManagerMuseum.unitsToDisplay.Length; k++)
		{
			if (array[k] < UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.keybindManagerMuseum.unitsToDisplay[k]].playerClassNames.Length)
			{
				list.Add(UIFunctions.globaluifunctions.keybindManagerMuseum.unitsToDisplay[k]);
			}
		}
		return list.ToArray<int>();
	}

	// Token: 0x060006AE RID: 1710 RVA: 0x00036B8C File Offset: 0x00034D8C
	public int GetValidVesselInstance()
	{
		if (this.playercampaigndata == null || this.playercampaigndata.playerVesselsLost == null)
		{
			return UnityEngine.Random.Range(0, UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].playerClassNames.Length);
		}
		int playerVesselClass = UIFunctions.globaluifunctions.playerfunctions.playerVesselClass;
		List<int> list = new List<int>();
		for (int i = 0; i < this.playercampaigndata.playerVesselsLost.Length; i++)
		{
			if (this.playercampaigndata.playerVesselsLost[i] == playerVesselClass)
			{
				list.Add(this.playercampaigndata.playerInstancesLost[i]);
			}
		}
		List<int> list2 = new List<int>();
		for (int j = 0; j < UIFunctions.globaluifunctions.database.databaseshipdata[playerVesselClass].playerClassNames.Length; j++)
		{
			if (!list.Contains(j))
			{
				list2.Add(j);
			}
		}
		return list2[UnityEngine.Random.Range(0, list2.Count)];
	}

	// Token: 0x060006AF RID: 1711 RVA: 0x00036CA0 File Offset: 0x00034EA0
	public string GetTaskForceShip(string shipGroup)
	{
		string[] array = shipGroup.Split(new char[]
		{
			'|'
		});
		return array[UnityEngine.Random.Range(0, array.Length)];
	}

	// Token: 0x060006B0 RID: 1712 RVA: 0x00036CCC File Offset: 0x00034ECC
	private void SetTaskForceIcon(CampaignTaskForce currentTaskForce)
	{
		bool flag = false;
		for (int i = 0; i < currentTaskForce.shipClasses.Length; i++)
		{
			int shipID = UIFunctions.globaluifunctions.textparser.GetShipID(currentTaskForce.shipClasses[i]);
			if (UIFunctions.globaluifunctions.database.databaseshipdata[shipID].shipType != "SUBMARINE")
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			currentTaskForce.taskForceType = 2;
		}
		else
		{
			currentTaskForce.taskForceType = 3;
		}
		this.campaignTaskForceIcons[currentTaskForce.taskForceID].sprite = this.reconImages[currentTaskForce.taskForceType];
		this.campaignTaskForceIcons[currentTaskForce.taskForceID].gameObject.SetActive(false);
	}

	// Token: 0x060006B1 RID: 1713 RVA: 0x00036D88 File Offset: 0x00034F88
	private int[] GetArrayOfMissions(string missionType)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < this.campaignmissions.Length; i++)
		{
			if (this.campaignmissions[i].missionType == missionType)
			{
				list.Add(i);
			}
		}
		return list.ToArray();
	}

	// Token: 0x060006B2 RID: 1714 RVA: 0x00036DDC File Offset: 0x00034FDC
	private int[] GetArrayOfOccupiedLocations(CampaignMission thisMission)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < this.campaignlocations.Length; i++)
		{
			if (thisMission.missionType == "RESUPPLY_CONVOY")
			{
				if (this.campaignlocations[i].faction == "ENEMY_OCCUPIED")
				{
					list.Add(i);
				}
			}
			else if (thisMission.missionType == "INSERTION" && this.campaignlocations[i].faction == "ENEMY_OCCUPIED" && this.campaignlocations[i].originalFaction == "FRIENDLY" && this.campaignlocations[i].function.Contains("INSERTION_TARGET"))
			{
				list.Add(i);
			}
		}
		return list.ToArray();
	}

	// Token: 0x060006B3 RID: 1715 RVA: 0x00036EC0 File Offset: 0x000350C0
	private int[] GetArrayOfLocations(string faction, string[] locationTypes, string missionType, bool enforceType)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < this.campaignlocations.Length; i++)
		{
			if (this.campaignlocations[i].faction == faction || faction == null)
			{
				bool flag = false;
				for (int j = 0; j < locationTypes.Length; j++)
				{
					if (this.campaignlocations[i].function.Contains(locationTypes[j]))
					{
						if (!enforceType)
						{
							flag = true;
							break;
						}
						if (this.campaignlocations[i].missionTypes.Contains(missionType) || missionType == "ANY")
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					if (this.campaignlocations[i].function.Contains("PLAYER_BASE"))
					{
						if (missionType == "RETURN_TO_BASE" || missionType == "LAND_STRIKE" || missionType == "INSERTION")
						{
							list.Add(i);
						}
					}
					else
					{
						list.Add(i);
					}
				}
			}
		}
		return list.ToArray();
	}

	// Token: 0x060006B4 RID: 1716 RVA: 0x00036FEC File Offset: 0x000351EC
	public void SetSubmarineClassDefaultData()
	{
		this.playercampaigndata.playerNoisemakersOnBoard = UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].numberofnoisemakers;
		this.playercampaigndata.playerTorpeodesOnBoard = new int[UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].torpedoNumbers.Length];
		for (int i = 0; i < UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].torpedoNumbers.Length; i++)
		{
			this.playercampaigndata.playerTorpeodesOnBoard[i] = UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].torpedoNumbers[i];
		}
		if (UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].vlsTorpedoGameObjects != null)
		{
			this.playercampaigndata.playerVLSTorpeodesOnBoard = new int[UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].vlsTorpedoNumbers.Length];
			for (int j = 0; j < UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].vlsTorpedoNumbers.Length; j++)
			{
				this.playercampaigndata.playerVLSTorpeodesOnBoard[j] = UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].vlsTorpedoNumbers[j];
			}
		}
		this.playercampaigndata.playerTubeStatus = new int[UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].torpedoConfig.Length];
		this.playercampaigndata.playerWeaponInTube = new int[UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].torpedoConfig.Length];
		this.playercampaigndata.playerSettingsOne = new int[UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].torpedoConfig.Length];
		this.playercampaigndata.playerSettingsTwo = new int[UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].torpedoConfig.Length];
		this.playercampaigndata.playerSettingsThree = new int[UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].torpedoConfig.Length];
		this.playercampaigndata.playerSealTeamOnBoard = false;
		this.playercampaigndata.vesselTotalDamage = 0f;
		this.playercampaigndata.compartmentCurrentFlooding = new float[this.playercampaigndata.compartmentCurrentFlooding.Length];
		this.playercampaigndata.compartmentTotalFlooding = new float[this.playercampaigndata.compartmentTotalFlooding.Length];
		this.playercampaigndata.destroyedSubsystems = new bool[UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlCurrentTimers.Length];
		this.playercampaigndata.decalNames = new int[10];
	}

	// Token: 0x060006B5 RID: 1717 RVA: 0x00037330 File Offset: 0x00035530
	public void SetPlayerCampaignData()
	{
		this.playercampaigndata.playerNoisemakersOnBoard = GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.noisemakersOnBoard;
		this.playercampaigndata.playerTorpeodesOnBoard = GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.currentTorpsOnBoard;
		if (GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.hasVLS)
		{
			this.playercampaigndata.playerVLSTorpeodesOnBoard = GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.vlsCurrentTorpsOnBoard;
		}
		this.playercampaigndata.playerTubeStatus = GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.tubeStatus;
		this.playercampaigndata.playerWeaponInTube = GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.weaponInTube;
		this.playercampaigndata.playerSettingsOne = GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.torpedoSearchPattern;
		this.playercampaigndata.playerSettingsTwo = GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.torpedoDepthPattern;
		this.playercampaigndata.playerSettingsThree = GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.torpedoHomingPattern;
		this.playercampaigndata.playerSealTeamOnBoard = GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.sealsOnBoard;
		this.playercampaigndata.vesselTotalDamage = GameDataManager.playervesselsonlevel[0].damagesystem.shipCurrentDamagePoints;
		for (int i = 0; i < UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentCurrentFlooding.Length; i++)
		{
			this.playercampaigndata.compartmentCurrentFlooding[i] = UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentCurrentFlooding[i];
			this.playercampaigndata.compartmentTotalFlooding[i] = UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentTotalFlooding[i];
		}
		this.playercampaigndata.destroyedSubsystems = new bool[UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlCurrentTimers.Length];
		for (int j = 0; j < UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlCurrentTimers.Length; j++)
		{
			if (UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlCurrentTimers[j] == 10000f)
			{
				this.playercampaigndata.destroyedSubsystems[j] = true;
			}
		}
	}

	// Token: 0x060006B6 RID: 1718 RVA: 0x00037588 File Offset: 0x00035788
	public void GetPlayerCampaignData()
	{
		GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.noisemakersOnBoard = this.playercampaigndata.playerNoisemakersOnBoard;
		GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.currentTorpsOnBoard = this.playercampaigndata.playerTorpeodesOnBoard;
		GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.vlsCurrentTorpsOnBoard = this.playercampaigndata.playerVLSTorpeodesOnBoard;
		GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.tubeStatus = this.playercampaigndata.playerTubeStatus;
		GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.weaponInTube = this.playercampaigndata.playerWeaponInTube;
		GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.torpedoSearchPattern = this.playercampaigndata.playerSettingsOne;
		GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.torpedoDepthPattern = this.playercampaigndata.playerSettingsTwo;
		GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.torpedoHomingPattern = this.playercampaigndata.playerSettingsThree;
		GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.sealsOnBoard = this.playercampaigndata.playerSealTeamOnBoard;
		GameDataManager.playervesselsonlevel[0].damagesystem.shipCurrentDamagePoints = this.playercampaigndata.vesselTotalDamage;
		for (int i = 0; i < UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentCurrentFlooding.Length; i++)
		{
			UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentCurrentFlooding[i] = this.playercampaigndata.compartmentCurrentFlooding[i];
			UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentTotalFlooding[i] = this.playercampaigndata.compartmentTotalFlooding[i];
		}
		if (this.playercampaigndata.decalNames != null)
		{
			for (int j = 0; j < this.playercampaigndata.decalNames.Length; j++)
			{
				if (this.playercampaigndata.decalNames[j] > 99)
				{
					GameDataManager.playervesselsonlevel[0].bouyancyCompartments[j].ApplyDamageDecal(true, this.playercampaigndata.decalNames[j] - 100);
				}
				else if (this.playercampaigndata.decalNames[j] > 9)
				{
					GameDataManager.playervesselsonlevel[0].bouyancyCompartments[j].ApplyDamageDecal(false, this.playercampaigndata.decalNames[j] - 10);
				}
			}
		}
		if (this.playercampaigndata.destroyedSubsystems == null)
		{
			return;
		}
		for (int k = 0; k < this.playercampaigndata.destroyedSubsystems.Length; k++)
		{
			if (this.playercampaigndata.destroyedSubsystems[k])
			{
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.subsystemOffline[k] = true;
				if (UIFunctions.globaluifunctions.database.databasesubsystemsdata[k].subsystem != "TUBES")
				{
					UIFunctions.globaluifunctions.playerfunctions.damagecontrol.KnockoutSubsystem(UIFunctions.globaluifunctions.database.databasesubsystemsdata[k].subsystem, true);
				}
				else
				{
					foreach (int num in GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.tubeStatus)
					{
						if (num == -200)
						{
							int subsystemIndex = DamageControl.GetSubsystemIndex("TUBES");
							UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlCurrentTimers[subsystemIndex] = 10000f;
							break;
						}
					}
				}
			}
		}
	}

	// Token: 0x040006C3 RID: 1731
	public static string campaignReferenceName;

	// Token: 0x040006C4 RID: 1732
	public string campaignSaveFileName;

	// Token: 0x040006C5 RID: 1733
	public bool campaignDebugMode;

	// Token: 0x040006C6 RID: 1734
	public string campaignDebugForcedMission;

	// Token: 0x040006C7 RID: 1735
	public int campaignDebugForcedMissionNumber = -1;

	// Token: 0x040006C8 RID: 1736
	public string[] defaultMissionTypes;

	// Token: 0x040006C9 RID: 1737
	public string commanderName;

	// Token: 0x040006CA RID: 1738
	public string commanderFleetName;

	// Token: 0x040006CB RID: 1739
	public Image mapSprite;

	// Token: 0x040006CC RID: 1740
	public Texture2D mapNavigation;

	// Token: 0x040006CD RID: 1741
	public Texture2D mapMerchantTraffic;

	// Token: 0x040006CE RID: 1742
	public Texture2D mapFishingTraffic;

	// Token: 0x040006CF RID: 1743
	public string elevationMapDataPath;

	// Token: 0x040006D0 RID: 1744
	public EventManager eventManager;

	// Token: 0x040006D1 RID: 1745
	public string portImage;

	// Token: 0x040006D2 RID: 1746
	public string transitImage;

	// Token: 0x040006D3 RID: 1747
	public int[] playerStartTelegraphs;

	// Token: 0x040006D4 RID: 1748
	public float[] playerStartDepths;

	// Token: 0x040006D5 RID: 1749
	public float[] playerMapSpeeds;

	// Token: 0x040006D6 RID: 1750
	public float mapSpeedModifier;

	// Token: 0x040006D7 RID: 1751
	public float timePenaltyOnSunk;

	// Token: 0x040006D8 RID: 1752
	public Vector2 playerBasePosition;

	// Token: 0x040006D9 RID: 1753
	public CampaignLocation[] campaignlocations;

	// Token: 0x040006DA RID: 1754
	public CampaignRecon[] campaignaircraft;

	// Token: 0x040006DB RID: 1755
	public CampaignRecon[] campaignsatellites;

	// Token: 0x040006DC RID: 1756
	public Sprite[] reconImages;

	// Token: 0x040006DD RID: 1757
	public GameObject mapReconObject;

	// Token: 0x040006DE RID: 1758
	public GameObject mapTaskForceObject;

	// Token: 0x040006DF RID: 1759
	public GameObject mapOccupiedZone;

	// Token: 0x040006E0 RID: 1760
	public Sprite[] occupiedZoneSprite;

	// Token: 0x040006E1 RID: 1761
	public float mapHeightInNM;

	// Token: 0x040006E2 RID: 1762
	public float mapTimeCompression;

	// Token: 0x040006E3 RID: 1763
	public string hemisphere;

	// Token: 0x040006E4 RID: 1764
	public float equatorYValue;

	// Token: 0x040006E5 RID: 1765
	public Vector2 disruptTimes;

	// Token: 0x040006E6 RID: 1766
	public float strategicRangeCheck;

	// Token: 0x040006E7 RID: 1767
	public float strategicRangesTimer;

	// Token: 0x040006E8 RID: 1768
	public Text campaignMapLabel;

	// Token: 0x040006E9 RID: 1769
	public Image campaignMapLocation;

	// Token: 0x040006EA RID: 1770
	public Path_TileGraph tileGraph;

	// Token: 0x040006EB RID: 1771
	public GameObject labelsLayer;

	// Token: 0x040006EC RID: 1772
	public GameObject strategicMapToolbar;

	// Token: 0x040006ED RID: 1773
	public GameObject straticMapEnterPortButton;

	// Token: 0x040006EE RID: 1774
	public Text toolbarPlayerStats;

	// Token: 0x040006EF RID: 1775
	public int[] firstOccupiedTerritory;

	// Token: 0x040006F0 RID: 1776
	public float newFrontChance = 0.15f;

	// Token: 0x040006F1 RID: 1777
	public int territoryTakebackThreshold;

	// Token: 0x040006F2 RID: 1778
	public bool thresholdMet;

	// Token: 0x040006F3 RID: 1779
	public string[] regionNames;

	// Token: 0x040006F4 RID: 1780
	public int[] regionNumberOfZones;

	// Token: 0x040006F5 RID: 1781
	public Image[] regionIcons;

	// Token: 0x040006F6 RID: 1782
	public bool useLandWar;

	// Token: 0x040006F7 RID: 1783
	public bool iconsInOccupiedOnly;

	// Token: 0x040006F8 RID: 1784
	public bool armisticeEventOccurred;

	// Token: 0x040006F9 RID: 1785
	public float hoursDurationOfCampaign;

	// Token: 0x040006FA RID: 1786
	public float hoursToNextGeneralEvent;

	// Token: 0x040006FB RID: 1787
	public float generalEventModifier;

	// Token: 0x040006FC RID: 1788
	public float campaignPoints;

	// Token: 0x040006FD RID: 1789
	public float totalCampaignPoints;

	// Token: 0x040006FE RID: 1790
	public Vector2 playerPositionOnLeavePort;

	// Token: 0x040006FF RID: 1791
	public SpriteRenderer[] warbar;

	// Token: 0x04000700 RID: 1792
	public Color[] warbarColors;

	// Token: 0x04000701 RID: 1793
	public Text timeDisplay;

	// Token: 0x04000702 RID: 1794
	public Text dateDisplay;

	// Token: 0x04000703 RID: 1795
	public float clockTimer;

	// Token: 0x04000704 RID: 1796
	public float julianStartDay;

	// Token: 0x04000705 RID: 1797
	public float dayRange;

	// Token: 0x04000706 RID: 1798
	public float hoursPerGeneralEvent;

	// Token: 0x04000707 RID: 1799
	public Vector2 hoursPerGeneralEventRange;

	// Token: 0x04000708 RID: 1800
	public int lastMissionCompleteStatus;

	// Token: 0x04000709 RID: 1801
	public int[] currentHourMonth = new int[2];

	// Token: 0x0400070A RID: 1802
	public GameObject playerGameObject;

	// Token: 0x0400070B RID: 1803
	public Image playerGameObjectIcon;

	// Token: 0x0400070C RID: 1804
	public float playerCurrentSpeed;

	// Token: 0x0400070D RID: 1805
	public Vector3 lastClickedPosition;

	// Token: 0x0400070E RID: 1806
	public string briefingType;

	// Token: 0x0400070F RID: 1807
	public bool playerInPort;

	// Token: 0x04000710 RID: 1808
	public Vector3 detectionDistances = new Vector3(12f, 8f, 4f);

	// Token: 0x04000711 RID: 1809
	public float currentEngageDistance = 6f;

	// Token: 0x04000712 RID: 1810
	public float currentDetectionDistance;

	// Token: 0x04000713 RID: 1811
	public Text timeInPortDisplay;

	// Token: 0x04000714 RID: 1812
	public float timeInPort;

	// Token: 0x04000715 RID: 1813
	public float timeInPortTimer;

	// Token: 0x04000716 RID: 1814
	public float timePenaltyOnPortEnter;

	// Token: 0x04000717 RID: 1815
	public float timePenaltyHullDamageMultiplier = 24f;

	// Token: 0x04000718 RID: 1816
	public float accumulatedTimeInPort;

	// Token: 0x04000719 RID: 1817
	public GameObject sosusBarrierGameObject;

	// Token: 0x0400071A RID: 1818
	public string[] sosusNames;

	// Token: 0x0400071B RID: 1819
	public string[] sosusAlignments;

	// Token: 0x0400071C RID: 1820
	public Vector2[] sosusStarts;

	// Token: 0x0400071D RID: 1821
	public Vector2[] sosusEnds;

	// Token: 0x0400071E RID: 1822
	public float[] sosusAngles;

	// Token: 0x0400071F RID: 1823
	public float[] sosusDetectionRange;

	// Token: 0x04000720 RID: 1824
	public GameObject[] sosusBarriers;

	// Token: 0x04000721 RID: 1825
	public GameObject[] campaignAircraftObjects;

	// Token: 0x04000722 RID: 1826
	public float[] aircraftPrepTime;

	// Token: 0x04000723 RID: 1827
	public GameObject[] campaignSatelliteObjects;

	// Token: 0x04000724 RID: 1828
	public float[] satelliteTimers;

	// Token: 0x04000725 RID: 1829
	public float[] currentSatelliteAngle;

	// Token: 0x04000726 RID: 1830
	public Vector3[] satelliteWaypoints;

	// Token: 0x04000727 RID: 1831
	public bool[] satelliteReversed;

	// Token: 0x04000728 RID: 1832
	public bool[] satelliteMoving;

	// Token: 0x04000729 RID: 1833
	public float satelliteReturnTime;

	// Token: 0x0400072A RID: 1834
	public CampaignTaskForce[] campaignTaskForces;

	// Token: 0x0400072B RID: 1835
	public GameObject[] campaignTaskForceObjects;

	// Token: 0x0400072C RID: 1836
	public bool[] activeTaskForces;

	// Token: 0x0400072D RID: 1837
	public float[] taskForceActivateTimers;

	// Token: 0x0400072E RID: 1838
	public Image[] campaignTaskForceIcons;

	// Token: 0x0400072F RID: 1839
	public int currentTaskForceEngagedWith;

	// Token: 0x04000730 RID: 1840
	public bool isThisALandStrike;

	// Token: 0x04000731 RID: 1841
	public CampaignMapWaypoint[] campaignmapwaypoints;

	// Token: 0x04000732 RID: 1842
	public CampaignRegionWaypoint[] campaignregionwaypoints;

	// Token: 0x04000733 RID: 1843
	public Transform navHelper;

	// Token: 0x04000734 RID: 1844
	public Color playerIconColor;

	// Token: 0x04000735 RID: 1845
	public Color enemyIconColor;

	// Token: 0x04000736 RID: 1846
	public Color[] contactColorsOverTime;

	// Token: 0x04000737 RID: 1847
	public Transform reconLayer;

	// Token: 0x04000738 RID: 1848
	public Transform shipLayer;

	// Token: 0x04000739 RID: 1849
	public Transform sosusLayer;

	// Token: 0x0400073A RID: 1850
	public Transform northPole;

	// Token: 0x0400073B RID: 1851
	public bool initCampaign;

	// Token: 0x0400073C RID: 1852
	public bool campaignInitialised;

	// Token: 0x0400073D RID: 1853
	public Transform[] ports;

	// Token: 0x0400073E RID: 1854
	public Transform[] waypoints;

	// Token: 0x0400073F RID: 1855
	public Transform[] sosus;

	// Token: 0x04000740 RID: 1856
	public int numberofPlayerMissionTypes;

	// Token: 0x04000741 RID: 1857
	public int numberOfPlayerMissions;

	// Token: 0x04000742 RID: 1858
	public int numberofNonPlayerMissionTypes;

	// Token: 0x04000743 RID: 1859
	public int numberOfNonPlayerMissions;

	// Token: 0x04000744 RID: 1860
	public string[] campaignMissionTypes;

	// Token: 0x04000745 RID: 1861
	public CampaignMission[] campaignmissions;

	// Token: 0x04000746 RID: 1862
	public float[] playerMissionFreqs;

	// Token: 0x04000747 RID: 1863
	public float[] nonPlayerMissionFreqs;

	// Token: 0x04000748 RID: 1864
	public float[] playerMissionThresholds;

	// Token: 0x04000749 RID: 1865
	public float commandoLoadTime;

	// Token: 0x0400074A RID: 1866
	public Dictionary<string, int> locationIDs;

	// Token: 0x0400074B RID: 1867
	public int[] playerNavalBases;

	// Token: 0x0400074C RID: 1868
	public int[] playerAirBases;

	// Token: 0x0400074D RID: 1869
	public int[] playerStrategicNodes;

	// Token: 0x0400074E RID: 1870
	public int[] playerSosusNodes;

	// Token: 0x0400074F RID: 1871
	public int[] enemyNavalBases;

	// Token: 0x04000750 RID: 1872
	public int[] enemyAirBases;

	// Token: 0x04000751 RID: 1873
	public PlayerCampaignData playercampaigndata;

	// Token: 0x04000752 RID: 1874
	public float currentMissionStartHour;

	// Token: 0x04000753 RID: 1875
	public float currentMissionEndHour;

	// Token: 0x04000754 RID: 1876
	public string currentMissionStartLocation;

	// Token: 0x04000755 RID: 1877
	public string currentMissionEndLocation;

	// Token: 0x04000756 RID: 1878
	public string currentMissionCountry;

	// Token: 0x04000757 RID: 1879
	public string currentMissionTimeInDays;

	// Token: 0x04000758 RID: 1880
	public string currentMissionTimeInHours;

	// Token: 0x04000759 RID: 1881
	public string[] shipClasses;

	// Token: 0x0400075A RID: 1882
	public string missionGivenOnDate;

	// Token: 0x0400075B RID: 1883
	public ToggleGroup missionListToggles;

	// Token: 0x0400075C RID: 1884
	public GameObject missionSelectBar;

	// Token: 0x0400075D RID: 1885
	public GameObject savegameBar;

	// Token: 0x0400075E RID: 1886
	public GameObject campaignExitMenu;

	// Token: 0x0400075F RID: 1887
	public static bool isEncounter;

	// Token: 0x04000760 RID: 1888
	public GameObject playerPortDetector;
}
