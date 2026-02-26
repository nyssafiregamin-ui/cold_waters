using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000161 RID: 353
public class StatusScreens : MonoBehaviour
{
	// Token: 0x06000A29 RID: 2601 RVA: 0x0007BBB4 File Offset: 0x00079DB4
	public void InitialiseStatusScreens()
	{
		this.playerSubInitialised = true;
		this.playerVessel = GameDataManager.playervesselsonlevel[0];
		this.currentScreen = -1;
		this.currentWeapon = 0;
		this.currentSubsystem = 0;
		this.repairIcon.transform.localPosition = new Vector3(0f, -500f, 0f);
		for (int i = 0; i < this.torpedoTubes.Length; i++)
		{
			this.torpedoTubes[i].SetActive(false);
		}
		for (int j = 0; j < this.reloadNames.Length; j++)
		{
			this.reloadNames[j].gameObject.SetActive(false);
		}
		for (int k = 0; k < this.playerVessel.databaseshipdata.torpedotubes; k++)
		{
			this.tubeLabels[k].color = Color.white;
			this.tubeLabels[k].text = (k + 1).ToString();
			this.torpedoTubes[k].SetActive(true);
		}
		for (int l = 0; l < this.playerVessel.vesselmovement.weaponSource.torpedoTubes.Length; l++)
		{
			if (this.playerVessel.vesselmovement.weaponSource.weaponInTube[l] >= 0)
			{
				this.playerVessel.vesselmovement.weaponSource.torpedoSearchPattern[l] = this.playerfunctions.GetSettingIndex(UIFunctions.globaluifunctions.database.databaseweapondata[this.playerVessel.vesselmovement.weaponSource.weaponInTube[l]].searchSettings[0], this.playerfunctions.attackSettingDefinitions);
				this.playerVessel.vesselmovement.weaponSource.torpedoDepthPattern[l] = this.playerfunctions.GetSettingIndex(UIFunctions.globaluifunctions.database.databaseweapondata[this.playerVessel.vesselmovement.weaponSource.weaponInTube[l]].heightSettings[0], this.playerfunctions.depthSettingDefinitions);
			}
		}
		for (int m = 0; m < this.playerVessel.vesselmovement.weaponSource.torpedoNames.Length; m++)
		{
			this.reloadNames[m].gameObject.SetActive(true);
			this.reloadNames[m].text = this.playerVessel.vesselmovement.weaponSource.torpedoNames[m];
		}
		this.waterRanges = new float[this.compartmentWaterLevels.Length];
		for (int n = 0; n < this.compartmentWaterLevels.Length; n++)
		{
			this.compartmentWaterLevels[n].transform.localPosition = new Vector3(this.compartmentWaterLevels[n].transform.localPosition.x, 0f, this.compartmentWaterLevels[n].transform.localPosition.z);
			this.waterRanges[n] = this.playerVessel.databaseshipdata.compartmentFloodingRanges[n].y - this.playerVessel.databaseshipdata.compartmentFloodingRanges[n].x;
		}
		for (int num = 0; num < 3; num++)
		{
			string text = string.Empty;
			if (num == 0)
			{
				text = UIFunctions.globaluifunctions.database.databasesonardata[this.playerVessel.databaseshipdata.passiveSonarID].sonarModel;
				text += " Passive Sonar";
			}
			else if (num == 1)
			{
				text = UIFunctions.globaluifunctions.database.databasesonardata[this.playerVessel.databaseshipdata.activeSonarID].sonarModel;
				text += " Active Sonar";
			}
			if (num == 2)
			{
				text = UIFunctions.globaluifunctions.database.databasesonardata[this.playerVessel.databaseshipdata.towedSonarID].sonarModel;
				text += " Towed Array";
			}
			this.subsystemTitles[num].text = text;
		}
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x0007BFCC File Offset: 0x0007A1CC
	public void SetLoadoutStats()
	{
	}

	// Token: 0x06000A2B RID: 2603 RVA: 0x0007BFD0 File Offset: 0x0007A1D0
	public void ManualSetWeaponDescription(int current)
	{
		this.currentWeapon = current;
		this.SetWeaponStats();
	}

	// Token: 0x06000A2C RID: 2604 RVA: 0x0007BFE0 File Offset: 0x0007A1E0
	public void SetWeaponStats()
	{
		if (this.currentWeapon != 6)
		{
			UIFunctions.globaluifunctions.mainColumn.text = this.reloadNames[this.currentWeapon].text + "\n\n";
			Text mainColumn = UIFunctions.globaluifunctions.mainColumn;
			mainColumn.text += UIFunctions.globaluifunctions.database.databaseweapondata[this.playerVessel.vesselmovement.weaponSource.torpedoTypes[this.currentWeapon]].weaponDescription[0];
		}
		else
		{
			UIFunctions.globaluifunctions.mainColumn.text = "Noisemaker\n\n";
			Text mainColumn2 = UIFunctions.globaluifunctions.mainColumn;
			mainColumn2.text += UIFunctions.globaluifunctions.database.databasecountermeasuredata[this.playerVessel.databaseshipdata.noiseMakerID].countermeasureDescription[0];
		}
		UIFunctions.globaluifunctions.secondColumm.text = string.Empty;
	}

	// Token: 0x06000A2D RID: 2605 RVA: 0x0007C0E0 File Offset: 0x0007A2E0
	public void BackToBriefing()
	{
		if (UIFunctions.globaluifunctions.campaignmanager.eventManager.missionOverBriefingDisplayed)
		{
			UIFunctions.globaluifunctions.campaignmanager.EndCampaignCombat();
			return;
		}
		UIFunctions.globaluifunctions.playerfunctions.currentOpenPanel = 0;
		UIFunctions.globaluifunctions.scrollbarDefault.value = 1f;
		UIFunctions.globaluifunctions.missionmanager.BringInBriefing(false);
		if (!GameDataManager.missionMode && !GameDataManager.trainingMode)
		{
			UIFunctions.globaluifunctions.campaignmanager.SetCampaignBriefingButtons();
			UIFunctions.globaluifunctions.HUDCameraObject.enabled = true;
			if (UIFunctions.globaluifunctions.campaignmanager.playerInPort)
			{
				UIFunctions.globaluifunctions.backgroundImagesOnly.SetActive(true);
				if (UIFunctions.globaluifunctions.campaignmanager.timeInPortTimer < UIFunctions.globaluifunctions.campaignmanager.timeInPort)
				{
					UIFunctions.globaluifunctions.campaignmanager.enabled = true;
				}
				else
				{
					UIFunctions.globaluifunctions.campaignmanager.enabled = false;
				}
			}
		}
		UIFunctions.globaluifunctions.campaignmanager.gameObject.SetActive(true);
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x0007C204 File Offset: 0x0007A404
	public void SetRepair()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		this.repairIcon.transform.position = this.timers[this.currentSubsystem].transform.position;
		this.repairIcon.transform.Translate(Vector3.right * 10f);
		this.currentRepair = this.currentSubsystem;
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x0007C274 File Offset: 0x0007A474
	public void SetSubsystemsStats()
	{
		UIFunctions.globaluifunctions.mainColumn.text = UIFunctions.globaluifunctions.database.databasesubsystemsdata[this.currentSubsystem].subsystemName + "\n\n";
		if (this.currentSubsystem <= 2)
		{
			int num = 0;
			if (this.currentSubsystem == 0)
			{
				num = this.playerVessel.databaseshipdata.passiveSonarID;
			}
			else if (this.currentSubsystem == 1)
			{
				num = this.playerVessel.databaseshipdata.activeSonarID;
			}
			else if (this.currentSubsystem == 2)
			{
				num = this.playerVessel.databaseshipdata.towedSonarID;
			}
			for (int i = 0; i < UIFunctions.globaluifunctions.database.databasesonardata[num].sonarDescription.Length; i++)
			{
				Text mainColumn = UIFunctions.globaluifunctions.mainColumn;
				mainColumn.text = mainColumn.text + UIFunctions.globaluifunctions.database.databasesonardata[num].sonarDescription[i] + "\n";
			}
		}
		UIFunctions.globaluifunctions.secondColumm.text = string.Empty;
	}

	// Token: 0x06000A30 RID: 2608 RVA: 0x0007C3A8 File Offset: 0x0007A5A8
	public void OpenStatusScreens(int currentscreen)
	{
		this.currentScreen = currentscreen;
		StatusScreens.statusPagesEnabled = true;
		UIFunctions.globaluifunctions.SetMenuSystem("STATUSSCREENS");
		if (!UIFunctions.globaluifunctions.GUICamera.activeSelf)
		{
			UIFunctions.globaluifunctions.GUICamera.SetActive(true);
			AudioListener component = UIFunctions.globaluifunctions.GUICamera.GetComponent<AudioListener>();
			if (component.enabled)
			{
				component.enabled = false;
			}
		}
		UIFunctions.globaluifunctions.scrollbarDefault.value = 1f;
		switch (currentscreen)
		{
		case 0:
			UIFunctions.globaluifunctions.campaignmanager.strategicMapToolbar.SetActive(false);
			UIFunctions.globaluifunctions.BuildBriefingHeader();
			UIFunctions.globaluifunctions.mainColumn.text = "THIS IS A TEST";
			UIFunctions.globaluifunctions.secondColumm.text = "SECOND TEST";
			UIFunctions.globaluifunctions.SetMainColumnHeight(false);
			UIFunctions.globaluifunctions.HUDCameraObject.enabled = false;
			if (!UIFunctions.globaluifunctions.campaignmanager.playerInPort)
			{
				UIFunctions.globaluifunctions.backgroundImagesOnly.gameObject.SetActive(false);
				UIFunctions.globaluifunctions.campaignmanager.gameObject.SetActive(true);
			}
			else
			{
				UIFunctions.globaluifunctions.backgroundImagesOnly.gameObject.SetActive(true);
				UIFunctions.globaluifunctions.campaignmanager.gameObject.SetActive(false);
			}
			this.playerfunctions.menuPanel.SetActive(false);
			break;
		case 2:
			UIFunctions.globaluifunctions.campaignmanager.strategicMapToolbar.SetActive(false);
			UIFunctions.globaluifunctions.BuildBriefingHeader();
			UIFunctions.globaluifunctions.mainColumn.text = this.GetMissionOrdersText();
			UIFunctions.globaluifunctions.secondColumm.text = UIFunctions.globaluifunctions.campaignmanager.missionGivenOnDate;
			UIFunctions.globaluifunctions.SetMainColumnHeight(false);
			UIFunctions.globaluifunctions.HUDCameraObject.enabled = false;
			this.playerfunctions.menuPanel.SetActive(false);
			break;
		case 3:
			UIFunctions.globaluifunctions.playerfunctions.menuPanel.SetActive(false);
			UIFunctions.globaluifunctions.campaignmanager.strategicMapToolbar.SetActive(false);
			UIFunctions.globaluifunctions.campaignmanager.GetNearestLocationToPlayer();
			UIFunctions.globaluifunctions.BuildBriefingHeader();
			UIFunctions.globaluifunctions.mainColumn.text = this.GetAfterMissionMessageText();
			UIFunctions.globaluifunctions.secondColumm.text = string.Empty;
			UIFunctions.globaluifunctions.SetMainColumnHeight(false);
			UIFunctions.globaluifunctions.HUDCameraObject.enabled = false;
			UIFunctions.globaluifunctions.campaignmanager.eventManager.missionOverBriefingDisplayed = true;
			if (!UIFunctions.globaluifunctions.campaignmanager.playerInPort)
			{
				UIFunctions.globaluifunctions.textparser.SetImageSprite(UIFunctions.globaluifunctions.campaignmanager.transitImage, UIFunctions.globaluifunctions.backgroundImage);
				UIFunctions.globaluifunctions.backgroundImagesOnly.SetActive(true);
			}
			else
			{
				UIFunctions.globaluifunctions.backgroundImagesOnly.gameObject.SetActive(true);
				UIFunctions.globaluifunctions.campaignmanager.gameObject.SetActive(false);
			}
			this.statusBackButton.SetActive(true);
			break;
		}
	}

	// Token: 0x06000A31 RID: 2609 RVA: 0x0007C6DC File Offset: 0x0007A8DC
	public void CloseStatusScreens()
	{
		UIFunctions.globaluifunctions.backgroundImage.gameObject.SetActive(false);
		this.currentScreen = -1;
		StatusScreens.statusPagesEnabled = false;
		UIFunctions.globaluifunctions.menuSystemParent.SetActive(false);
		AudioListener component = UIFunctions.globaluifunctions.GUICamera.GetComponent<AudioListener>();
		if (!component.enabled)
		{
			component.enabled = true;
		}
		UIFunctions.globaluifunctions.HUDCameraObject.enabled = true;
	}

	// Token: 0x06000A32 RID: 2610 RVA: 0x0007C750 File Offset: 0x0007A950
	private string GetMissionOrdersText()
	{
		bool flag = false;
		string text = UIFunctions.globaluifunctions.textparser.ReadDirectTextFromFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("language/message/orders_header" + this.playerfunctions.GetPlayerNavySuffix()));
		int num = 0;
		string[] array;
		if (GameDataManager.trainingMode || GameDataManager.missionMode)
		{
			string filePathFromString = UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/missions/ss_specops_" + num.ToString());
			array = UIFunctions.globaluifunctions.textparser.OpenTextDataFile(filePathFromString);
		}
		else
		{
			CampaignTaskForce campaignTaskForce = UIFunctions.globaluifunctions.campaignmanager.campaignTaskForces[UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.currentMissionTaskForceID];
			string filePathFromString = UIFunctions.globaluifunctions.textparser.GetFilePathFromString(string.Concat(new string[]
			{
				"campaign/",
				CampaignManager.campaignReferenceName,
				"/language/missions/",
				UIFunctions.globaluifunctions.campaignmanager.campaignMissionTypes[campaignTaskForce.missionType],
				"_",
				campaignTaskForce.actualMission.ToString()
			}));
			array = UIFunctions.globaluifunctions.textparser.OpenTextDataFile(filePathFromString);
			UIFunctions.globaluifunctions.campaignmanager.BringInDisabledCampaignManager();
		}
		for (int i = 0; i < array.Length; i++)
		{
			string text2 = array[i].Trim();
			if (flag)
			{
				if (text2.Contains("[MISSION WIN]"))
				{
					break;
				}
				text = text + "\n" + text2;
				text = UIFunctions.globaluifunctions.textparser.PopulateDynamicTags(text, true);
			}
			else
			{
				string[] array2 = text2.Split(new char[]
				{
					'='
				});
				if (array2[0] == "[ORDERS]")
				{
					flag = true;
				}
			}
		}
		return text;
	}

	// Token: 0x06000A33 RID: 2611 RVA: 0x0007C924 File Offset: 0x0007AB24
	private string GetNonPlayerMissionMessageText()
	{
		string text = UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/aftermission_");
		string str = "win";
		if (!UIFunctions.globaluifunctions.campaignmanager.eventManager.missionPassed)
		{
			str = "fail";
		}
		text += str;
		string text2 = UIFunctions.globaluifunctions.textparser.ReadDirectTextFromFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("language/message/orders_outcome" + this.playerfunctions.GetPlayerNavySuffix()));
		text2 = text2 + "\n" + UIFunctions.globaluifunctions.textparser.GetRandomTextLineFromFile(text);
		return UIFunctions.globaluifunctions.textparser.PopulateDynamicTags(text2, true);
	}

	// Token: 0x06000A34 RID: 2612 RVA: 0x0007C9E8 File Offset: 0x0007ABE8
	private string GetAfterMissionMessageText()
	{
		if (UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.currentMissionTaskForceID != UIFunctions.globaluifunctions.campaignmanager.currentTaskForceEngagedWith)
		{
			return this.GetNonPlayerMissionMessageText();
		}
		bool flag = false;
		CampaignTaskForce campaignTaskForce = UIFunctions.globaluifunctions.campaignmanager.campaignTaskForces[UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.currentMissionTaskForceID];
		string filePathFromString = UIFunctions.globaluifunctions.textparser.GetFilePathFromString(string.Concat(new string[]
		{
			"campaign/",
			CampaignManager.campaignReferenceName,
			"/language/missions/",
			UIFunctions.globaluifunctions.campaignmanager.campaignMissionTypes[campaignTaskForce.missionType],
			"_",
			campaignTaskForce.actualMission.ToString()
		}));
		string[] array = UIFunctions.globaluifunctions.textparser.OpenTextDataFile(filePathFromString);
		UIFunctions.globaluifunctions.campaignmanager.BringInDisabledCampaignManager();
		string b = "[MISSION FAIL]";
		if (UIFunctions.globaluifunctions.campaignmanager.eventManager.missionMissed)
		{
			b = "[MISSION MISSED]";
			UIFunctions.globaluifunctions.campaignmanager.eventManager.missionMissed = false;
		}
		else if (UIFunctions.globaluifunctions.campaignmanager.eventManager.missionPassed)
		{
			b = "[MISSION WIN]";
		}
		string text = UIFunctions.globaluifunctions.textparser.ReadDirectTextFromFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("language/message/orders_outcome" + this.playerfunctions.GetPlayerNavySuffix()));
		for (int i = 0; i < array.Length; i++)
		{
			string text2 = array[i].Trim();
			if (flag)
			{
				text += text2;
				text = text + " " + UIFunctions.globaluifunctions.textparser.GetRandomTextLineFromFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("language/message/orders_await")).ToUpper();
				text = UIFunctions.globaluifunctions.textparser.PopulateDynamicTags(text, true);
				return "\n" + text;
			}
			string[] array2 = text2.Split(new char[]
			{
				'='
			});
			if (array2[0] == b)
			{
				flag = true;
			}
		}
		return null;
	}

	// Token: 0x06000A35 RID: 2613 RVA: 0x0007CC18 File Offset: 0x0007AE18
	public void PortLoadSealTeam()
	{
		this.AddSealTeamOnBoard(GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.sealsOnBoard);
	}

	// Token: 0x06000A36 RID: 2614 RVA: 0x0007CC38 File Offset: 0x0007AE38
	private void AddSealTeamOnBoard(bool addSeals)
	{
		if (!addSeals)
		{
			int num = 0;
			for (int i = 0; i < this.playerVessel.databaseshipdata.torpedoNumbers.Length; i++)
			{
				num += this.playerVessel.databaseshipdata.torpedoNumbers[i];
			}
			int num2 = 0;
			for (int j = 0; j < this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard.Length; j++)
			{
				num2 += this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[j];
			}
			int num3 = 0;
			for (int k = 0; k < this.playerVessel.vesselmovement.weaponSource.tubeStatus.Length; k++)
			{
				if (this.playerVessel.vesselmovement.weaponSource.tubeStatus[k] >= 0)
				{
					num3++;
				}
			}
			int num4 = num2 - num3;
			if (num4 > 0)
			{
				for (int l = 0; l < this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard.Length; l++)
				{
					int num5 = 0;
					for (int m = 0; m < this.playerVessel.vesselmovement.weaponSource.tubeStatus.Length; m++)
					{
						if (this.playerVessel.vesselmovement.weaponSource.tubeStatus[m] == l)
						{
							num5++;
						}
					}
					this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[l] = num5;
				}
			}
			this.sealTeamImage.SetActive(true);
			this.sealTeamText.text = this.sealTeamLabels[1];
			this.playerVessel.vesselmovement.weaponSource.sealsOnBoard = true;
			this.SetLoadoutStats();
		}
		else
		{
			this.sealTeamImage.SetActive(false);
			this.sealTeamText.text = this.sealTeamLabels[0];
			this.playerVessel.vesselmovement.weaponSource.sealsOnBoard = false;
			this.SetLoadoutStats();
		}
	}

	// Token: 0x06000A37 RID: 2615 RVA: 0x0007CE40 File Offset: 0x0007B040
	public void PortLoadWeapon(bool isAdded)
	{
		if (!isAdded)
		{
			int num = 0;
			for (int i = 0; i < this.playerVessel.vesselmovement.weaponSource.tubeStatus.Length; i++)
			{
				if (this.playerVessel.vesselmovement.weaponSource.tubeStatus[i] == this.currentWeapon)
				{
					num++;
				}
			}
			if (this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[this.currentWeapon] > num)
			{
				this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[this.currentWeapon]--;
			}
			else
			{
				for (int j = 0; j < this.playerVessel.vesselmovement.weaponSource.tubeStatus.Length; j++)
				{
					if (this.playerVessel.vesselmovement.weaponSource.tubeStatus[j] == this.currentWeapon)
					{
						this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[this.currentWeapon]--;
						this.playerVessel.vesselmovement.weaponSource.tubeStatus[j] = -1;
						break;
					}
				}
			}
		}
		else
		{
			int num2 = 0;
			for (int k = 0; k < this.playerVessel.databaseshipdata.torpedoNumbers.Length; k++)
			{
				num2 += this.playerVessel.databaseshipdata.torpedoNumbers[k];
			}
			int num3 = 0;
			for (int l = 0; l < this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard.Length; l++)
			{
				num3 += this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[l];
			}
			int num4 = 0;
			for (int m = 0; m < this.playerVessel.vesselmovement.weaponSource.tubeStatus.Length; m++)
			{
				if (this.playerVessel.vesselmovement.weaponSource.tubeStatus[m] >= 0)
				{
					num4++;
				}
			}
			int num5 = num3 - num4;
			if (num5 < num2 - this.playerVessel.vesselmovement.weaponSource.tubeStatus.Length && !this.playerVessel.vesselmovement.weaponSource.sealsOnBoard)
			{
				this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[this.currentWeapon]++;
			}
			else if (num4 < this.playerVessel.vesselmovement.weaponSource.tubeStatus.Length)
			{
				for (int n = 0; n < this.playerVessel.vesselmovement.weaponSource.tubeStatus.Length; n++)
				{
					if (this.playerVessel.vesselmovement.weaponSource.tubeStatus[n] < 0)
					{
						this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[this.currentWeapon]++;
						this.playerVessel.vesselmovement.weaponSource.tubeStatus[n] = this.currentWeapon;
						break;
					}
				}
			}
		}
		this.SetLoadoutStats();
	}

	// Token: 0x04000FB6 RID: 4022
	public PlayerFunctions playerfunctions;

	// Token: 0x04000FB7 RID: 4023
	public Vessel playerVessel;

	// Token: 0x04000FB8 RID: 4024
	public bool playerSubInitialised;

	// Token: 0x04000FB9 RID: 4025
	public int currentScreen;

	// Token: 0x04000FBA RID: 4026
	public int currentSubsystem;

	// Token: 0x04000FBB RID: 4027
	public int currentWeapon;

	// Token: 0x04000FBC RID: 4028
	public int currentRepair;

	// Token: 0x04000FBD RID: 4029
	public int currentPump;

	// Token: 0x04000FBE RID: 4030
	public GameObject[] statusPages;

	// Token: 0x04000FBF RID: 4031
	public GameObject statusBackButton;

	// Token: 0x04000FC0 RID: 4032
	public static bool conditionsEnabled;

	// Token: 0x04000FC1 RID: 4033
	public static bool statusPagesEnabled;

	// Token: 0x04000FC2 RID: 4034
	public GameObject conditionsObject;

	// Token: 0x04000FC3 RID: 4035
	public GameObject[] submarineGraphics;

	// Token: 0x04000FC4 RID: 4036
	public Text numberOfNoiseMakers;

	// Token: 0x04000FC5 RID: 4037
	public GameObject[] torpedoTubes;

	// Token: 0x04000FC6 RID: 4038
	public Text[] reloadNames;

	// Token: 0x04000FC7 RID: 4039
	public Text[] reloadNumbers;

	// Token: 0x04000FC8 RID: 4040
	public Text[] tubeLabels;

	// Token: 0x04000FC9 RID: 4041
	public Image[] weaponImages;

	// Token: 0x04000FCA RID: 4042
	public Image[] torpedoTubeImages;

	// Token: 0x04000FCB RID: 4043
	public string[] sealTeamLabels;

	// Token: 0x04000FCC RID: 4044
	public Text sealTeamText;

	// Token: 0x04000FCD RID: 4045
	public GameObject sealTeamImage;

	// Token: 0x04000FCE RID: 4046
	public GameObject portReloadGUI;

	// Token: 0x04000FCF RID: 4047
	public Text[] portReloadingTexts;

	// Token: 0x04000FD0 RID: 4048
	public Text[] subsystemTitles;

	// Token: 0x04000FD1 RID: 4049
	public Text[] timers;

	// Token: 0x04000FD2 RID: 4050
	public GameObject repairIcon;

	// Token: 0x04000FD3 RID: 4051
	public float catchTime = 0.25f;

	// Token: 0x04000FD4 RID: 4052
	public GameObject[] compartmentWaterLevels;

	// Token: 0x04000FD5 RID: 4053
	public float[] waterRanges;
}
