using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x02000139 RID: 313
public class MissionManager : MonoBehaviour
{
	// Token: 0x06000878 RID: 2168 RVA: 0x0005E6A0 File Offset: 0x0005C8A0
	private void Start()
	{
		this.selectionGUI.SetActive(false);
		this.closureParams = new Vector3(10000f, 25000f, 5000f);
	}

	// Token: 0x06000879 RID: 2169 RVA: 0x0005E6D4 File Offset: 0x0005C8D4
	public void SetClosureRange()
	{
		this.closureValue -= this.closureParams.z;
		if (this.closureValue < this.closureParams.x)
		{
			this.closureValue = this.closureParams.y;
		}
		this.closureRangeDisplay.text = (this.closureValue / 1000f).ToString() + " " + LanguageManager.interfaceDictionary["KiloYard"];
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x0005E758 File Offset: 0x0005C958
	public void SetMissionNumber(int number)
	{
		this.currentMission = number;
		this.currentMissionID = number;
		UIFunctions.globaluifunctions.selectionGroupText.text = null;
		if (GameDataManager.trainingMode)
		{
			this.uifunctions.textparser.PopulateLevelLoadData("training" + (number + 1).ToString("000"), null);
		}
		else if (GameDataManager.missionMode)
		{
			this.uifunctions.textparser.PopulateLevelLoadData("single" + (number + 1).ToString("000"), null);
		}
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x0005E7F4 File Offset: 0x0005C9F4
	private bool HaveTorpedoContact()
	{
		for (int i = 0; i < UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects.Length; i++)
		{
			if (Vector3.Distance(UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[i].transform.position, GameDataManager.playervesselsonlevel[0].transform.position) * GameDataManager.yardsScale < this.weaponLeaveDist && UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[i].databaseweapondata.weaponType != "DECOY")
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600087C RID: 2172 RVA: 0x0005E8A4 File Offset: 0x0005CAA4
	private bool HaveAircraftContact()
	{
		for (int i = 0; i < UIFunctions.globaluifunctions.combatai.enemyHelicopters.Length; i++)
		{
			if (Vector3.Distance(UIFunctions.globaluifunctions.combatai.enemyHelicopters[i].transform.position, GameDataManager.playervesselsonlevel[0].transform.position) * GameDataManager.yardsScale < this.aircraftLeaveDist)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600087D RID: 2173 RVA: 0x0005E918 File Offset: 0x0005CB18
	private bool HaveContact()
	{
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			if (Vector3.Distance(GameDataManager.enemyvesselsonlevel[i].transform.position, GameDataManager.playervesselsonlevel[0].transform.position) * GameDataManager.yardsScale < this.contactLeaveDist && !GameDataManager.enemyvesselsonlevel[i].isSinking && GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipType != "BIOLOGIC" && GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipType != "MERCHANT" && GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipType != "OILRIG")
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600087E RID: 2174 RVA: 0x0005E9EC File Offset: 0x0005CBEC
	public void QuitCombat()
	{
		UIFunctions.globaluifunctions.EndLevel();
		UIFunctions.globaluifunctions.CloseActionReport();
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x0005EA04 File Offset: 0x0005CC04
	public void BringInExitMenu(bool outofBounds = false)
	{
		this.sinkAllButton.SetActive(false);
		if (GameDataManager.playerCommanderName == "TheTruthIsOutThere")
		{
			this.sinkAllButton.SetActive(true);
		}
		if (!UIFunctions.globaluifunctions.playerfunctions.tacMapMaximisedGraphic.enabled)
		{
			UIFunctions.globaluifunctions.playerfunctions.HUDTacMap();
		}
		if (UIFunctions.globaluifunctions.playerfunctions.hudHidden)
		{
			UIFunctions.globaluifunctions.playerfunctions.HideHUD();
		}
		if (UIFunctions.globaluifunctions.playerfunctions.timeCompressionToggle == 1)
		{
			UIFunctions.globaluifunctions.playerfunctions.SetTimeCompression();
		}
		bool[] array = new bool[4];
		string text = "None";
		if (this.HaveContact())
		{
			text = "Contact";
			array[0] = true;
		}
		if (this.HaveTorpedoContact())
		{
			text = "Contact";
			array[1] = true;
		}
		if (this.HaveAircraftContact())
		{
			text = "Contact";
			array[2] = true;
		}
		if (text == "None")
		{
			text = "NoContact";
		}
		if (UIFunctions.globaluifunctions.playerfunctions.statusIcons[8].gameObject.activeSelf)
		{
			text = "Flooding";
			array[3] = true;
		}
		if (GameDataManager.playervesselsonlevel[0].isSinking)
		{
			text = "Sinking";
			if ((float)UIFunctions.globaluifunctions.playerfunctions.playerDepthInFeet > GameDataManager.playervesselsonlevel[0].databaseshipdata.escapeDepth && GameDataManager.playervesselsonlevel[0].gameObject.activeSelf)
			{
				text = "Doomed";
			}
			array[3] = true;
		}
		if (outofBounds)
		{
			text = "OutOfBounds";
		}
		if (!GameDataManager.playervesselsonlevel[0].gameObject.activeSelf)
		{
			text = "None";
			this.exitMissionMain.text = string.Empty;
		}
		if (text != "None")
		{
			string[] array2 = UIFunctions.globaluifunctions.textparser.OpenTextDataFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("language/message/mission_exit"));
			for (int i = 0; i < array2.Length; i++)
			{
				string[] array3 = array2[i].Split(new char[]
				{
					'='
				});
				if (array3[0].Trim() == text)
				{
					this.exitMissionMain.text = array3[1].Trim();
					this.exitMissionMain.text = this.exitMissionMain.text.Replace("\\n", "\n");
					break;
				}
			}
			string text2 = string.Empty;
			string[] array4 = new string[]
			{
				"lime",
				"red"
			};
			if (array[0])
			{
				string text3 = text2;
				text2 = string.Concat(new string[]
				{
					text3,
					"          <color=",
					array4[1],
					"><b>",
					LanguageManager.interfaceDictionary["LeaveCombatVessels"],
					"</b></color>\n"
				});
			}
			else
			{
				string text3 = text2;
				text2 = string.Concat(new string[]
				{
					text3,
					"          <color=",
					array4[0],
					"><b>",
					LanguageManager.interfaceDictionary["LeaveCombatNeg"],
					" ",
					LanguageManager.interfaceDictionary["LeaveCombatVessels"],
					"</b></color>\n"
				});
			}
			if (array[1])
			{
				string text3 = text2;
				text2 = string.Concat(new string[]
				{
					text3,
					"          <color=",
					array4[1],
					"><b>",
					LanguageManager.interfaceDictionary["LeaveCombatWeapons"],
					"</b></color>\n"
				});
			}
			else
			{
				string text3 = text2;
				text2 = string.Concat(new string[]
				{
					text3,
					"          <color=",
					array4[0],
					"><b>",
					LanguageManager.interfaceDictionary["LeaveCombatNeg"],
					" ",
					LanguageManager.interfaceDictionary["LeaveCombatWeapons"],
					"</b></color>\n"
				});
			}
			if (array[2])
			{
				string text3 = text2;
				text2 = string.Concat(new string[]
				{
					text3,
					"          <color=",
					array4[1],
					"><b>",
					LanguageManager.interfaceDictionary["LeaveCombatAircraft"],
					"</b></color>\n"
				});
			}
			else
			{
				string text3 = text2;
				text2 = string.Concat(new string[]
				{
					text3,
					"          <color=",
					array4[0],
					"><b>",
					LanguageManager.interfaceDictionary["LeaveCombatNeg"],
					" ",
					LanguageManager.interfaceDictionary["LeaveCombatAircraft"],
					"</b></color>\n"
				});
			}
			if (array[3])
			{
				string text3 = text2;
				text2 = string.Concat(new string[]
				{
					text3,
					"          <color=",
					array4[1],
					"><b>",
					LanguageManager.interfaceDictionary["LeaveCombatFlooding"],
					"</b></color>\n"
				});
			}
			else
			{
				string text3 = text2;
				text2 = string.Concat(new string[]
				{
					text3,
					"          <color=",
					array4[0],
					"><b>",
					LanguageManager.interfaceDictionary["LeaveCombatNeg"],
					" ",
					LanguageManager.interfaceDictionary["LeaveCombatFlooding"],
					"</b></color>\n"
				});
			}
			text2 += this.AppendSpecialMissionStatus();
			this.exitMissionMain.text = this.exitMissionMain.text.Replace("<LEAVESTATUS>", text2);
		}
		this.abandonShipButton.SetActive(true);
		if (text == "Contact" || text == "Flooding")
		{
			this.leaveCombatButton.SetActive(false);
		}
		else
		{
			this.leaveCombatButton.SetActive(true);
		}
		this.quitCombatButton.SetActive(false);
		if (GameDataManager.trainingMode || GameDataManager.missionMode)
		{
			this.leaveCombatButton.SetActive(true);
			this.quitCombatButton.SetActive(false);
		}
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.tacMapCamera.depth = -1f;
		Time.timeScale = 0f;
		UIFunctions.globaluifunctions.SetMenuSystem("COMBATEXIT");
		UIFunctions.globaluifunctions.backgroundTemplate.SetActive(false);
		UIFunctions.globaluifunctions.HUDholder.SetActive(false);
		if (!UIFunctions.globaluifunctions.GUICamera.activeSelf)
		{
			UIFunctions.globaluifunctions.GUICamera.SetActive(true);
			AudioListener component = UIFunctions.globaluifunctions.GUICamera.GetComponent<AudioListener>();
			if (component.enabled)
			{
				component.enabled = false;
			}
		}
	}

	// Token: 0x06000880 RID: 2176 RVA: 0x0005F0C8 File Offset: 0x0005D2C8
	private string AppendSpecialMissionStatus()
	{
		if (GameDataManager.missionMode || GameDataManager.trainingMode)
		{
			return "\n";
		}
		string text = string.Empty;
		string[] array = new string[]
		{
			"lime",
			"red"
		};
		if (UIFunctions.globaluifunctions.campaignmanager.currentTaskForceEngagedWith == UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.currentMissionTaskForceID)
		{
			if (UIFunctions.globaluifunctions.campaignmanager.campaignmissions[UIFunctions.globaluifunctions.campaignmanager.campaignTaskForces[UIFunctions.globaluifunctions.campaignmanager.currentTaskForceEngagedWith].missionID].missionType == "INSERTION")
			{
				if (UIFunctions.globaluifunctions.campaignmanager.eventManager.sealsReleased)
				{
					text = string.Concat(new string[]
					{
						"          <color=",
						array[0],
						"><b>",
						LanguageManager.interfaceDictionary["LeaveCombatInsertion"],
						"</b></color>\n"
					});
					text = text.Replace("<MISSIONSTATUS>", string.Empty);
				}
				else
				{
					text = string.Concat(new string[]
					{
						"          <color=",
						array[1],
						"><b>",
						LanguageManager.interfaceDictionary["LeaveCombatInsertion"],
						"</b></color>\n"
					});
					text = text.Replace("<MISSIONSTATUS>", LanguageManager.interfaceDictionary["LeaveCombatMissionNeg"]);
				}
			}
			else if (UIFunctions.globaluifunctions.campaignmanager.campaignmissions[UIFunctions.globaluifunctions.campaignmanager.campaignTaskForces[UIFunctions.globaluifunctions.campaignmanager.currentTaskForceEngagedWith].missionID].missionType == "LAND_STRIKE")
			{
				if (UIFunctions.globaluifunctions.playerfunctions.landAttackNumber >= UIFunctions.globaluifunctions.campaignmanager.campaignmissions[UIFunctions.globaluifunctions.campaignmanager.campaignTaskForces[UIFunctions.globaluifunctions.campaignmanager.currentTaskForceEngagedWith].missionID].numberOfRequiredWeapon)
				{
					text = string.Concat(new string[]
					{
						"          <color=",
						array[0],
						"><b>",
						LanguageManager.interfaceDictionary["LeaveCombatLandStrike"],
						"</b></color>\n"
					});
					text = text.Replace("<MISSIONSTATUS>", string.Empty);
				}
				else
				{
					text = string.Concat(new string[]
					{
						"          <color=",
						array[1],
						"><b>",
						LanguageManager.interfaceDictionary["LeaveCombatLandStrike"],
						"</b></color>\n"
					});
					text = text.Replace("<MISSIONSTATUS>", LanguageManager.interfaceDictionary["LeaveCombatMissionNeg"]);
				}
				text = text.Replace("<LANDSTRIKESTATUS>", UIFunctions.globaluifunctions.playerfunctions.landAttackNumber.ToString() + "/" + UIFunctions.globaluifunctions.campaignmanager.campaignmissions[UIFunctions.globaluifunctions.campaignmanager.campaignTaskForces[UIFunctions.globaluifunctions.campaignmanager.currentTaskForceEngagedWith].missionID].numberOfRequiredWeapon.ToString());
			}
		}
		return text;
	}

	// Token: 0x06000881 RID: 2177 RVA: 0x0005F3EC File Offset: 0x0005D5EC
	public void DismissExitMenu()
	{
		UIFunctions.globaluifunctions.playerfunctions.timeCompressionToggle = 0;
		UIFunctions.globaluifunctions.GUICamera.SetActive(false);
		Time.timeScale = 1f;
		if (!GameDataManager.playervesselsonlevel[0].isSinking)
		{
			UIFunctions.globaluifunctions.HUDholder.SetActive(true);
			if (UIFunctions.globaluifunctions.playerfunctions.tacMapMaximisedGraphic.enabled)
			{
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.tacMapCamera.depth = 2f;
			}
		}
		this.missionExitMenu.gameObject.SetActive(false);
		UIFunctions.globaluifunctions.menuSystemParent.SetActive(false);
		if (GameDataManager.trainingMode)
		{
			UIFunctions.globaluifunctions.HUDholder.SetActive(true);
		}
	}

	// Token: 0x06000882 RID: 2178 RVA: 0x0005F4BC File Offset: 0x0005D6BC
	public void BringInBriefing(bool setButtons)
	{
		this.uifunctions.SetMenuSystem("BRIEFING");
		if (UIFunctions.globaluifunctions.loadingMask.activeSelf)
		{
			UIFunctions.globaluifunctions.loadingMask.SetActive(false);
		}
		if (GameDataManager.trainingMode || GameDataManager.missionMode)
		{
			this.uifunctions.backgroundImagesOnly.SetActive(true);
		}
		else
		{
			this.uifunctions.backgroundImagesOnly.SetActive(false);
		}
		UIFunctions.globaluifunctions.backgroundImage.gameObject.SetActive(true);
		UIFunctions.globaluifunctions.HUDCamera.SetActive(true);
		UIFunctions.globaluifunctions.HUDCameraObject.enabled = true;
		UIFunctions.globaluifunctions.campaignmanager.SetPortControls(true);
		UIFunctions.globaluifunctions.playerfunctions.damagecontrol.timeToRepairReadout.gameObject.SetActive(false);
		this.PopulateBriefingText();
		UIFunctions.globaluifunctions.playerfunctions.statusscreens.currentScreen = -1;
		UIFunctions.globaluifunctions.playerfunctions.statusscreens.statusBackButton.SetActive(true);
		if (setButtons)
		{
			if (GameDataManager.missionMode || GameDataManager.trainingMode)
			{
				this.backButton.SetActive(true);
				this.battleStationsButton.SetActive(true);
				this.closureButton.SetActive(true);
				this.continueCourseButton.SetActive(false);
				this.ordersButton.SetActive(false);
				this.statusButton.SetActive(true);
				this.rearmButton.SetActive(false);
			}
			else
			{
				this.backButton.SetActive(false);
				this.battleStationsButton.SetActive(false);
				this.closureButton.SetActive(false);
				this.continueCourseButton.SetActive(true);
				this.ordersButton.SetActive(true);
				this.statusButton.SetActive(true);
				this.rearmButton.SetActive(false);
			}
		}
	}

	// Token: 0x06000883 RID: 2179 RVA: 0x0005F698 File Offset: 0x0005D898
	public void PopulateBriefingText()
	{
		UIFunctions.globaluifunctions.BuildBriefingHeader();
		string text = string.Empty;
		string text2 = string.Empty;
		bool flag = true;
		MissionManager.initialBearing = string.Empty;
		if (GameDataManager.missionMode || GameDataManager.trainingMode || CampaignManager.isEncounter)
		{
			this.MovePlayerToStartPosition();
		}
		else
		{
			flag = false;
		}
		for (int i = 0; i < this.briefingPrimaryText.Length; i++)
		{
			string text3 = this.briefingPrimaryText[i];
			if (!flag && text3.Contains("<BEARING>"))
			{
				text3 = this.briefingNoContact;
			}
			if (text3.Contains("<BEARING>"))
			{
				if (MissionManager.initialBearing == string.Empty)
				{
					float num = -500f;
					for (int j = 0; j < UIFunctions.globaluifunctions.playerfunctions.sensormanager.sensorLastPassiveReading.Length; j++)
					{
						if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.sensorLastPassiveReading[j] > num)
						{
							num = UIFunctions.globaluifunctions.playerfunctions.sensormanager.sensorLastPassiveReading[j];
							MissionManager.initialBearing = (MissionManager.initialBearing = string.Format("{0:0}", UIFunctions.globaluifunctions.playerfunctions.sensormanager.bearingToContacts[j]));
							this.initialIndex = j;
						}
					}
				}
				text3 = text3.Replace("<BEARING>", string.Format("{0:0}", MissionManager.initialBearing));
			}
			if (text3.Contains("<DEPTH>"))
			{
				text3 = text3.Replace("<DEPTH>", ((int)Mathf.Round((GameDataManager.playervesselsonlevel[0].transform.position.y - 1000f) * -GameDataManager.unitsToFeet)).ToString() + " " + LanguageManager.interfaceDictionary["Feet"].ToLower());
			}
			if (text3.Contains("<HEADING>"))
			{
				text3 = text3.Replace("<HEADING>", string.Format("{0:0}", GameDataManager.playervesselsonlevel[0].transform.eulerAngles.y));
			}
			if (text3.Contains("<SPEED>"))
			{
				text3 = text3.Replace("<SPEED>", string.Format("{0:0}", GameDataManager.playervesselsonlevel[0].vesselmovement.shipSpeed.z * 10f) + " kt");
			}
			if (text3.Contains("<DEPLOYMENTZONE>"))
			{
				text3 = text3.Replace("<DEPLOYMENTZONE>", this.GetDeploymentZoneDisplay());
			}
			text = text + text3 + "\n";
		}
		for (int k = 0; k < this.briefingSecondaryText.Length; k++)
		{
			string text4 = this.briefingSecondaryText[k];
			if (text4.Contains("<CONDITIONS>"))
			{
				string text5 = LanguageManager.interfaceDictionary[UIFunctions.globaluifunctions.playerfunctions.sensormanager.weather[UIFunctions.globaluifunctions.playerfunctions.sensormanager.currentWeather]];
				text5 = text5 + ", " + LanguageManager.interfaceDictionary[UIFunctions.globaluifunctions.playerfunctions.sensormanager.seaStates[(int)UIFunctions.globaluifunctions.playerfunctions.sensormanager.seaState]].ToLower();
				text4 = text4.Replace("<CONDITIONS>", text5);
			}
			if (text4.Contains("<SURFACEDUCT>"))
			{
				text4 = text4.Replace("<SURFACEDUCT>", this.GetStrengthDisplay(UIFunctions.globaluifunctions.playerfunctions.sensormanager.surfaceDuctStrength));
			}
			if (text4.Contains("<STRENGTH>"))
			{
				text4 = text4.Replace("<STRENGTH>", this.GetStrengthDisplay(UIFunctions.globaluifunctions.playerfunctions.sensormanager.layerStrength));
			}
			if (text4.Contains("<LAYERDEPTH>"))
			{
				if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.layerStrength > 0f)
				{
					text4 = text4.Replace("<LAYERDEPTH>", string.Format("{0:0}", UIFunctions.globaluifunctions.playerfunctions.sensormanager.layerDepthInFeet) + " " + LanguageManager.interfaceDictionary["Feet"].ToLower());
				}
				else
				{
					text4 = text4.Replace("<LAYERDEPTH>", string.Empty);
				}
			}
			if (text4.Contains("<WATERDEPTH>"))
			{
				text4 = text4.Replace("<WATERDEPTH>", this.getWaterDepth());
			}
			text2 = text2 + text4 + "\n";
		}
		UIFunctions.globaluifunctions.mainColumn.text = text;
		UIFunctions.globaluifunctions.secondColumm.text = text2;
		UIFunctions.globaluifunctions.SetMainColumnHeight(false);
		this.closureValue = this.closureParams.x;
		this.SetClosureRange();
	}

	// Token: 0x06000884 RID: 2180 RVA: 0x0005FBA0 File Offset: 0x0005DDA0
	private string getWaterDepth()
	{
		float waterDepth = this.uifunctions.levelloadmanager.levelloaddata.waterDepth;
		if (waterDepth < 300f)
		{
			return "very shallow";
		}
		if (waterDepth < 500f)
		{
			return "shallow";
		}
		if (waterDepth < 750f)
		{
			return "moderate";
		}
		if (waterDepth < 1000f)
		{
			return "deep";
		}
		return "very deep";
	}

	// Token: 0x06000885 RID: 2181 RVA: 0x0005FC0C File Offset: 0x0005DE0C
	private int SimplifiedMovePlayerToStartPosition()
	{
		if (!GameDataManager.missionMode && !GameDataManager.trainingMode)
		{
			UIFunctions.globaluifunctions.campaignmanager.GetPlayerCampaignData();
		}
		if (UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.usePresetPositions)
		{
			this.uifunctions.playerfunctions.sensormanager.SonarCheck();
			return 0;
		}
		Vector3 position = new Vector3(UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.formationGrid.transform.position.x, GameDataManager.playervesselsonlevel[0].transform.position.y, UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.formationGrid.transform.position.z);
		GameDataManager.playervesselsonlevel[0].transform.position = position;
		GameDataManager.playervesselsonlevel[0].transform.rotation = UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.formationGrid.transform.rotation;
		Vector2 vector = new Vector2(20f, 35f);
		if (GameDataManager.playervesselsonlevel[0].vesselmovement.shipSpeed.z > 12f)
		{
			vector = new Vector2(10f, 20f);
		}
		else if (GameDataManager.playervesselsonlevel[0].vesselmovement.shipSpeed.z > 20f)
		{
			vector = new Vector2(3f, 10f);
		}
		if (UIFunctions.globaluifunctions.levelloadmanager.GetIsSubmarinesOnly())
		{
			vector.x -= 3f;
			vector.y -= 3f;
		}
		float num = UnityEngine.Random.Range(vector.x, vector.y) * 1000f;
		num *= GameDataManager.inverseYardsScale;
		GameDataManager.playervesselsonlevel[0].transform.Translate(Vector3.forward * num);
		GameDataManager.playervesselsonlevel[0].transform.Translate(Vector3.right * UnityEngine.Random.Range(num * -0.3f, num * 0.3f));
		num = (float)(180 + UnityEngine.Random.Range(-45, 45));
		GameDataManager.playervesselsonlevel[0].transform.rotation = Quaternion.Euler(0f, GameDataManager.playervesselsonlevel[0].transform.eulerAngles.y + num, 0f);
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.initialRun = false;
		this.uifunctions.playerfunctions.sensormanager.SonarCheck();
		return 0;
	}

	// Token: 0x06000886 RID: 2182 RVA: 0x0005FEB4 File Offset: 0x0005E0B4
	public void ForceConvergingCourses()
	{
		GameDataManager.playervesselsonlevel[0].transform.position = new Vector3(GameDataManager.enemyvesselsonlevel[0].transform.position.x, GameDataManager.playervesselsonlevel[0].transform.position.y, GameDataManager.enemyvesselsonlevel[0].transform.position.z);
		GameDataManager.playervesselsonlevel[0].transform.rotation = Quaternion.Slerp(GameDataManager.playervesselsonlevel[0].transform.rotation, Quaternion.Euler(0f, GameDataManager.enemyvesselsonlevel[0].transform.eulerAngles.y + 180f, 0f), 1f);
		GameDataManager.playervesselsonlevel[0].transform.Translate(Vector3.forward * -50f);
		UIFunctions.globaluifunctions.missionmanager.playerWasBehind = true;
	}

	// Token: 0x06000887 RID: 2183 RVA: 0x0005FFAC File Offset: 0x0005E1AC
	private void MoveAddedShips(float amount)
	{
		int num = 0;
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			if (GameDataManager.enemyvesselsonlevel[i].whichNavy == 1)
			{
				num++;
			}
			else
			{
				GameDataManager.enemyvesselsonlevel[i].transform.Translate(Vector3.up * amount);
			}
		}
	}

	// Token: 0x06000888 RID: 2184 RVA: 0x0006000C File Offset: 0x0005E20C
	private int MovePlayerToStartPosition()
	{
		if (!GameDataManager.missionMode && !GameDataManager.trainingMode)
		{
			UIFunctions.globaluifunctions.campaignmanager.GetPlayerCampaignData();
		}
		if (UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.usePresetPositions)
		{
			this.uifunctions.playerfunctions.sensormanager.SonarCheck();
			return 0;
		}
		if (!this.playerWasBehind)
		{
			Vector3 vector = new Vector3(UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.formationGrid.transform.position.x, GameDataManager.playervesselsonlevel[0].transform.position.y, UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.formationGrid.transform.position.z);
			GameDataManager.playervesselsonlevel[0].transform.LookAt(vector);
			GameDataManager.playervesselsonlevel[0].transform.position = vector;
			GameDataManager.playervesselsonlevel[0].transform.Translate(Vector3.forward * -100f);
		}
		this.uifunctions.playerfunctions.sensormanager.SonarCheck();
		int loudestVesselIndex = this.GetLoudestVesselIndex();
		float num = UIFunctions.globaluifunctions.playerfunctions.sensormanager.signalLastPassiveReading[loudestVesselIndex];
		int highestSensorIndex = this.GetHighestSensorIndex();
		float num2 = UIFunctions.globaluifunctions.playerfunctions.sensormanager.sensorLastPassiveReading[highestSensorIndex];
		int num3 = 0;
		float num4 = 10f;
		float y = GameDataManager.playervesselsonlevel[0].transform.eulerAngles.y;
		if (num < 10f)
		{
			GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.LookAt(GameDataManager.enemyvesselsonlevel[loudestVesselIndex].transform.position);
			GameDataManager.playervesselsonlevel[0].transform.rotation = Quaternion.Slerp(GameDataManager.playervesselsonlevel[0].transform.rotation, Quaternion.Euler(0f, GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.eulerAngles.y, 0f), 1f);
			do
			{
				GameDataManager.playervesselsonlevel[0].transform.Translate(Vector3.forward * num4, Space.Self);
				this.uifunctions.playerfunctions.sensormanager.SonarCheck();
				num3++;
			}
			while (UIFunctions.globaluifunctions.playerfunctions.sensormanager.signalLastPassiveReading[loudestVesselIndex] < 10f && UIFunctions.globaluifunctions.playerfunctions.sensormanager.sensorLastPassiveReading[highestSensorIndex] < 10f && num3 < 25);
			GameDataManager.playervesselsonlevel[0].transform.rotation = Quaternion.Slerp(GameDataManager.playervesselsonlevel[0].transform.rotation, Quaternion.Euler(0f, y, 0f), 1f);
		}
		else if (num > 20f)
		{
			GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.LookAt(GameDataManager.enemyvesselsonlevel[loudestVesselIndex].transform.position);
			GameDataManager.playervesselsonlevel[0].transform.rotation = Quaternion.Slerp(GameDataManager.playervesselsonlevel[0].transform.rotation, Quaternion.Euler(0f, GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.eulerAngles.y, 0f), 1f);
			do
			{
				GameDataManager.playervesselsonlevel[0].transform.Translate(Vector3.forward * -num4, Space.Self);
				this.uifunctions.playerfunctions.sensormanager.SonarCheck();
				num3++;
			}
			while (UIFunctions.globaluifunctions.playerfunctions.sensormanager.signalLastPassiveReading[loudestVesselIndex] > 13f && num3 < 25);
			GameDataManager.playervesselsonlevel[0].transform.rotation = Quaternion.Slerp(GameDataManager.playervesselsonlevel[0].transform.rotation, Quaternion.Euler(0f, y, 0f), 1f);
		}
		if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.sensorLastPassiveReading[highestSensorIndex] > 20f && GameDataManager.enemyvesselsonlevel[highestSensorIndex].vesselai.sensordata != null)
		{
			GameDataManager.enemyvesselsonlevel[highestSensorIndex].vesselai.sensordata.playerDetected = true;
			GameDataManager.enemyvesselsonlevel[highestSensorIndex].vesselai.sensordata.decibelsTotalDetected = UnityEngine.Random.Range(20f, 100f);
		}
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.initialRun = false;
		this.uifunctions.playerfunctions.sensormanager.SonarCheck();
		GameDataManager.playervesselsonlevel[0].transform.rotation = Quaternion.Euler(0f, GameDataManager.playervesselsonlevel[0].transform.eulerAngles.y + UnityEngine.Random.Range(-90f, 90f), 0f);
		this.CheckPlayerTooClose();
		return loudestVesselIndex;
	}

	// Token: 0x06000889 RID: 2185 RVA: 0x00060528 File Offset: 0x0005E728
	private int GetHighestSensorIndex()
	{
		float num = -500f;
		int result = 0;
		for (int i = 0; i < UIFunctions.globaluifunctions.playerfunctions.sensormanager.sensorLastPassiveReading.Length; i++)
		{
			if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.sensorLastPassiveReading[i] > num)
			{
				num = UIFunctions.globaluifunctions.playerfunctions.sensormanager.sensorLastPassiveReading[i];
				result = i;
			}
		}
		return result;
	}

	// Token: 0x0600088A RID: 2186 RVA: 0x0006059C File Offset: 0x0005E79C
	private int GetLoudestVesselIndex()
	{
		float num = float.PositiveInfinity;
		int result = 0;
		for (int i = 0; i < UIFunctions.globaluifunctions.playerfunctions.sensormanager.sensorLastPassiveReading.Length; i++)
		{
			if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.sensorLastPassiveReading[i] > num && GameDataManager.enemyvesselsonlevel[i].whichNavy == 1)
			{
				num = UIFunctions.globaluifunctions.playerfunctions.sensormanager.sensorLastPassiveReading[i];
				result = i;
			}
		}
		return result;
	}

	// Token: 0x0600088B RID: 2187 RVA: 0x00060620 File Offset: 0x0005E820
	private int GetClosestEngagementVesselIndex()
	{
		float num = float.PositiveInfinity;
		int result = 0;
		for (int i = 0; i < UIFunctions.globaluifunctions.playerfunctions.sensormanager.rangeToContacts.Length; i++)
		{
			if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.rangeToContacts[i] < num && GameDataManager.enemyvesselsonlevel[i].whichNavy == 1)
			{
				num = UIFunctions.globaluifunctions.playerfunctions.sensormanager.rangeToContacts[i];
				result = i;
			}
		}
		return result;
	}

	// Token: 0x0600088C RID: 2188 RVA: 0x000606A4 File Offset: 0x0005E8A4
	private int GetClosestVesselIndex()
	{
		float num = float.PositiveInfinity;
		int result = 0;
		for (int i = 0; i < UIFunctions.globaluifunctions.playerfunctions.sensormanager.rangeToContacts.Length; i++)
		{
			if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.rangeToContacts[i] < num && GameDataManager.enemyvesselsonlevel[i].whichNavy == 1)
			{
				num = UIFunctions.globaluifunctions.playerfunctions.sensormanager.rangeToContacts[i];
				result = i;
			}
		}
		return result;
	}

	// Token: 0x0600088D RID: 2189 RVA: 0x00060728 File Offset: 0x0005E928
	private void CheckPlayerTooClose()
	{
		if (UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.usePresetPositions)
		{
			return;
		}
		float y = GameDataManager.playervesselsonlevel[0].transform.eulerAngles.y;
		int closestVesselIndex = this.GetClosestVesselIndex();
		float num = UIFunctions.globaluifunctions.playerfunctions.sensormanager.rangeToContacts[closestVesselIndex];
		GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.LookAt(GameDataManager.enemyvesselsonlevel[closestVesselIndex].transform.position);
		int num2 = 0;
		float num3 = 750f * GameDataManager.inverseYardsScale;
		float num4 = 7000f;
		if (GameDataManager.enemyvesselsonlevel[closestVesselIndex].databaseshipdata.shipType != "SUBMARINE")
		{
			num4 = 10000f;
		}
		if (num < num4)
		{
			GameDataManager.playervesselsonlevel[0].transform.rotation = Quaternion.Slerp(GameDataManager.playervesselsonlevel[0].transform.rotation, Quaternion.Euler(0f, GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.eulerAngles.y, 0f), 1f);
			do
			{
				if (GameDataManager.enemyvesselsonlevel[closestVesselIndex].databaseshipdata.shipType == "SUBMARINE")
				{
					num4 = 7000f;
				}
				else
				{
					num4 = 10000f;
				}
				GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.LookAt(GameDataManager.enemyvesselsonlevel[closestVesselIndex].transform.position);
				GameDataManager.playervesselsonlevel[0].transform.Translate(Vector3.forward * -num3, Space.Self);
				this.uifunctions.playerfunctions.sensormanager.SonarCheck();
				closestVesselIndex = this.GetClosestVesselIndex();
				num2++;
			}
			while (UIFunctions.globaluifunctions.playerfunctions.sensormanager.rangeToContacts[closestVesselIndex] < num4 || (this.IsBehindPLayer(GameDataManager.enemyvesselsonlevel[closestVesselIndex].transform) && num2 < 25));
			GameDataManager.playervesselsonlevel[0].transform.rotation = Quaternion.Slerp(GameDataManager.playervesselsonlevel[0].transform.rotation, Quaternion.Euler(0f, y, 0f), 1f);
		}
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.initialRun = false;
		this.uifunctions.playerfunctions.sensormanager.SonarCheck();
	}

	// Token: 0x0600088E RID: 2190 RVA: 0x0006099C File Offset: 0x0005EB9C
	private bool IsBehindPLayer(Transform vesselTransform)
	{
		return GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.localEulerAngles.y > 90f && GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.localEulerAngles.y < 270f;
	}

	// Token: 0x0600088F RID: 2191 RVA: 0x00060A08 File Offset: 0x0005EC08
	public void MovePlayerToStartDistance()
	{
		if (UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.usePresetPositions)
		{
			return;
		}
		float y = GameDataManager.playervesselsonlevel[0].transform.eulerAngles.y;
		int closestEngagementVesselIndex = this.GetClosestEngagementVesselIndex();
		float num = UIFunctions.globaluifunctions.playerfunctions.sensormanager.rangeToContacts[closestEngagementVesselIndex];
		GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.LookAt(GameDataManager.enemyvesselsonlevel[closestEngagementVesselIndex].transform.position);
		int num2 = 0;
		float d = 2000f * GameDataManager.inverseYardsScale;
		bool flag = true;
		if (num > this.closureValue * 1.5f)
		{
			GameDataManager.playervesselsonlevel[0].transform.rotation = Quaternion.Slerp(GameDataManager.playervesselsonlevel[0].transform.rotation, Quaternion.Euler(0f, GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.eulerAngles.y, 0f), 1f);
			do
			{
				GameDataManager.playervesselsonlevel[0].transform.Translate(Vector3.forward * d, Space.Self);
				this.uifunctions.playerfunctions.sensormanager.SonarCheck();
				for (int i = 0; i < UIFunctions.globaluifunctions.playerfunctions.sensormanager.sensorOfContacts.Length; i++)
				{
					if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.sensorOfContacts[i] > 0f)
					{
						flag = false;
					}
				}
				num2++;
			}
			while (UIFunctions.globaluifunctions.playerfunctions.sensormanager.rangeToContacts[closestEngagementVesselIndex] > this.closureValue * UnityEngine.Random.Range(1.1f, 2f) && num2 < 25 && flag);
			GameDataManager.playervesselsonlevel[0].transform.rotation = Quaternion.Slerp(GameDataManager.playervesselsonlevel[0].transform.rotation, Quaternion.Euler(0f, y, 0f), 1f);
		}
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.initialRun = false;
		this.uifunctions.playerfunctions.sensormanager.SonarCheck();
		this.CheckPlayerTooClose();
		this.Battlestations();
	}

	// Token: 0x06000890 RID: 2192 RVA: 0x00060C50 File Offset: 0x0005EE50
	public string GetStrengthDisplay(float value)
	{
		if (value <= 0f)
		{
			return LanguageManager.interfaceDictionary[UIFunctions.globaluifunctions.playerfunctions.sensormanager.strengthTypes[0]];
		}
		if (value <= 0.2f)
		{
			return LanguageManager.interfaceDictionary[UIFunctions.globaluifunctions.playerfunctions.sensormanager.strengthTypes[1]];
		}
		if (value <= 0.4f)
		{
			return LanguageManager.interfaceDictionary[UIFunctions.globaluifunctions.playerfunctions.sensormanager.strengthTypes[2]];
		}
		if (value <= 0.6f)
		{
			return LanguageManager.interfaceDictionary[UIFunctions.globaluifunctions.playerfunctions.sensormanager.strengthTypes[3]];
		}
		if (value <= 0.8f)
		{
			return LanguageManager.interfaceDictionary[UIFunctions.globaluifunctions.playerfunctions.sensormanager.strengthTypes[4]];
		}
		return LanguageManager.interfaceDictionary[UIFunctions.globaluifunctions.playerfunctions.sensormanager.strengthTypes[5]];
	}

	// Token: 0x06000891 RID: 2193 RVA: 0x00060D5C File Offset: 0x0005EF5C
	private string GetDeploymentZoneDisplay()
	{
		if (CampaignManager.isEncounter && UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.missionMarker.gameObject.activeSelf)
		{
			return "<color=lime><b>" + LanguageManager.interfaceDictionary["NearDeploymentZone"] + "</b></color>  ";
		}
		if (UIFunctions.globaluifunctions.campaignmanager.isThisALandStrike)
		{
			return "<color=lime><b>" + LanguageManager.interfaceDictionary["NearDeploymentZone"] + "</b></color>  ";
		}
		return string.Empty;
	}

	// Token: 0x06000892 RID: 2194 RVA: 0x00060DF4 File Offset: 0x0005EFF4
	public void Battlestations()
	{
		LevelLoadManager.inMuseum = false;
		UIFunctions.globaluifunctions.campaignmanager.SetPortControls(false);
		UIFunctions.globaluifunctions.playerfunctions.voicemanager.StopAudioArray();
		UIFunctions.globaluifunctions.playerfunctions.voicemanager.voiceSource.enabled = true;
		UIFunctions.globaluifunctions.HUDholder.SetActive(true);
		UIFunctions.globaluifunctions.playerfunctions.menuPanel.SetActive(true);
		UIFunctions.globaluifunctions.playerfunctions.otherPanel.SetActive(true);
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.tacMapCamera.gameObject.SetActive(true);
		if (GameDataManager.trainingMode)
		{
			UIFunctions.globaluifunctions.selectionGroupText.text = null;
			if (GameDataManager.trainingMode)
			{
				UIFunctions.globaluifunctions.helpmanager.trainingIndex = 1;
				UIFunctions.globaluifunctions.helpmanager.GetTrainingContent(UIFunctions.globaluifunctions.helpmanager.trainingIndex, "training_combat_" + (this.currentMission + 1).ToString(), "TRAINING", true);
			}
			UIFunctions.globaluifunctions.helpmanager.tutorialEventDone = new bool[10];
		}
		if (UIFunctions.globaluifunctions.levelloadmanager.currentMapGeneratorInstance != null)
		{
			UIFunctions.globaluifunctions.levelloadmanager.CheckSpawnDepth(GameDataManager.playervesselsonlevel[0]);
			for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
			{
				if (!GameDataManager.enemyvesselsonlevel[i].vesselai.isDocked)
				{
					UIFunctions.globaluifunctions.levelloadmanager.CheckSpawnDepth(GameDataManager.enemyvesselsonlevel[i]);
				}
			}
		}
		UIFunctions.globaluifunctions.playerfunctions.statusscreens.statusBackButton.SetActive(false);
		UIFunctions.globaluifunctions.playerfunctions.statusscreens.currentScreen = -1;
		UIFunctions.globaluifunctions.levelloadmanager.MainCamera.active = true;
		AudioListener component = UIFunctions.globaluifunctions.levelloadmanager.MainCamera.GetComponent<AudioListener>();
		component.enabled = true;
		UIFunctions.globaluifunctions.levelloadmanager.GUICamera.active = false;
		UIFunctions.globaluifunctions.levelloadmanager.HUDCamera.active = true;
		UIFunctions.globaluifunctions.HUDCameraObject.enabled = true;
		UIFunctions.globaluifunctions.playerfunctions.damagecontrol.enabled = true;
		UIFunctions.globaluifunctions.menuSystemParent.SetActive(false);
		UIFunctions.globaluifunctions.tapeBearingGameObject.SetActive(true);
		Time.timeScale = 1f;
		this.uifunctions.keybindManager.gameObject.SetActive(true);
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.tacMapCamera.enabled = true;
		UIFunctions.globaluifunctions.playerfunctions.HUDTacMap();
		if (!UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.minimapIsOpen)
		{
			UIFunctions.globaluifunctions.playerfunctions.HUDTacMap();
		}
		UIFunctions.globaluifunctions.bearingMarker.gameObject.SetActive(true);
		if (!UIFunctions.globaluifunctions.cameraMount.gameObject.activeSelf)
		{
			UIFunctions.globaluifunctions.cameraMount.gameObject.SetActive(true);
		}
		AudioManager.audiomanager.SetCombatSounds(true);
		if (!this.uifunctions.oceansound.isPlaying)
		{
			this.uifunctions.oceansound.Play();
			this.uifunctions.underwatersound.Play();
		}
		if (GameDataManager.optionsBoolSettings[0])
		{
			AudioManager.audiomanager.InitialiseCombatMusic();
			AudioManager.audiomanager.StopMusic();
		}
		this.closureButton.SetActive(false);
		UIFunctions.globaluifunctions.playerfunctions.menuPanel.transform.localPosition += new Vector3(UIFunctions.globaluifunctions.playerfunctions.menuPanelOffset.x, UIFunctions.globaluifunctions.playerfunctions.menuPanelOffset.y, 0f);
		UIFunctions.globaluifunctions.playerfunctions.currentOpenPanel = 0;
		UIFunctions.globaluifunctions.playerfunctions.OpenContextualPanel(0);
		UIFunctions.globaluifunctions.playerfunctions.messageLogPanel.SetActive(true);
		UIFunctions.globaluifunctions.combatai.enabled = true;
		UIFunctions.globaluifunctions.playerfunctions.SetPlayerData();
		float num = (float)((int)(UIFunctions.globaluifunctions.playerfunctions.sensormanager.GetUnitsDepthUnderTransform(GameDataManager.playervesselsonlevel[0].transform) * GameDataManager.unitsToFeet));
		if (num < 50f)
		{
			GameDataManager.playervesselsonlevel[0].transform.Translate(Vector3.up);
			if (GameDataManager.playervesselsonlevel[0].transform.position.y > GameDataManager.playervesselsonlevel[0].databaseshipdata.submergedat)
			{
				GameDataManager.playervesselsonlevel[0].transform.position = new Vector3(GameDataManager.playervesselsonlevel[0].transform.position.x, GameDataManager.playervesselsonlevel[0].databaseshipdata.submergedat, GameDataManager.playervesselsonlevel[0].transform.position.z);
			}
		}
		for (int j = 0; j < GameDataManager.enemyvesselsonlevel.Length; j++)
		{
			num = UIFunctions.globaluifunctions.playerfunctions.sensormanager.GetUnitsDepthUnderTransform(GameDataManager.enemyvesselsonlevel[j].transform) * GameDataManager.unitsToFeet;
			if (num < 50f)
			{
				GameDataManager.enemyvesselsonlevel[j].transform.Translate(Vector3.up);
				if (GameDataManager.enemyvesselsonlevel[j].transform.position.y > GameDataManager.enemyvesselsonlevel[j].databaseshipdata.submergedat)
				{
					GameDataManager.enemyvesselsonlevel[j].transform.position = new Vector3(GameDataManager.enemyvesselsonlevel[j].transform.position.x, GameDataManager.enemyvesselsonlevel[j].databaseshipdata.submergedat, GameDataManager.enemyvesselsonlevel[j].transform.position.z);
				}
			}
		}
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.CentreMap();
		this.exitMissionDate.text = UIFunctions.globaluifunctions.dateTitle.text;
		this.exitMissionTitle.text = UIFunctions.globaluifunctions.secondaryTitle.text;
		this.exitMissionMain.text = string.Empty;
		UIFunctions.globaluifunctions.dateTitle.text = string.Empty;
		UIFunctions.globaluifunctions.secondaryTitle.text = string.Empty;
		for (int k = 0; k < GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.originalLoadedWeaponInTube.Length; k++)
		{
			GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.originalLoadedWeaponInTube[k] = -1;
		}
		UIFunctions.globaluifunctions.ingamereference.SetInGameUnitReference(true);
		float num2 = (float)((int)Mathf.Round((GameDataManager.playervesselsonlevel[0].transform.position.y - 1000f) * -GameDataManager.unitsToFeet));
		UIFunctions.globaluifunctions.playerfunctions.helmmanager.wantedDepth = (float)(Mathf.RoundToInt(num2 / 50f) * 50);
		UIFunctions.globaluifunctions.playerfunctions.helmmanager.depthDisplayText.text = UIFunctions.globaluifunctions.playerfunctions.helmmanager.wantedDepth.ToString();
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.initialDetectedByPlayer = new bool[GameDataManager.enemyvesselsonlevel.Length];
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.initialDetectedByPlayerName = new string[GameDataManager.enemyvesselsonlevel.Length];
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.detectedByPlayer = new bool[GameDataManager.enemyvesselsonlevel.Length];
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.identifiedByPlayer = new bool[GameDataManager.enemyvesselsonlevel.Length];
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.classifiedByPlayer = new bool[GameDataManager.enemyvesselsonlevel.Length];
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.classifiedByPlayerAsClass = new int[GameDataManager.enemyvesselsonlevel.Length];
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.numberOfSonarContacts = 0;
		for (int l = 0; l < UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.mapContact.Length; l++)
		{
			UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.mapContact[l].gameObject.SetActive(false);
		}
		this.uifunctions.playerfunctions.SetContactToNone();
		this.uifunctions.playerfunctions.currentTargetIndex = -1;
		this.uifunctions.playerfunctions.sensormanager.addTMA = true;
		this.uifunctions.playerfunctions.sensormanager.SonarCheck();
		int num3 = 0;
		for (int m = 0; m < UIFunctions.globaluifunctions.playerfunctions.sensormanager.detectedByPlayer.Length; m++)
		{
			if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.detectedByPlayer[m])
			{
				num3++;
				this.uifunctions.playerfunctions.currentTargetIndex = m;
				this.uifunctions.playerfunctions.sensormanager.SelectTarget();
				break;
			}
		}
		if (num3 == 0)
		{
			string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "LostSonarContact");
			text = text.Replace("<BRG>", MissionManager.initialBearing);
			text = text.Replace("<CONTACT>", LanguageManager.interfaceDictionary["SonarContact"] + " 1");
			UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["Sonar"], "LostSonarContact", true);
		}
	}

	// Token: 0x06000893 RID: 2195 RVA: 0x000617DC File Offset: 0x0005F9DC
	public void LeavePort()
	{
		if (UIFunctions.globaluifunctions.campaignmanager.timeInPortTimer < UIFunctions.globaluifunctions.campaignmanager.timeInPort)
		{
			return;
		}
		UIFunctions.globaluifunctions.campaignmanager.timeInPortTimer = 0f;
		UIFunctions.globaluifunctions.campaignmanager.timeInPort = 0f;
		UIFunctions.globaluifunctions.campaignmanager.accumulatedTimeInPort = 0f;
		UIFunctions.globaluifunctions.campaignmanager.playerGameObject.transform.localPosition = new Vector3(UIFunctions.globaluifunctions.campaignmanager.playerPositionOnLeavePort.x, UIFunctions.globaluifunctions.campaignmanager.playerPositionOnLeavePort.y, UIFunctions.globaluifunctions.campaignmanager.playerGameObject.transform.localPosition.z);
		UIFunctions.globaluifunctions.campaignmanager.playerInPort = false;
		UIFunctions.globaluifunctions.campaignmanager.eventManager.playerWounded = false;
		UIFunctions.globaluifunctions.playerfunctions.damagecontrol.timeToRepairReadout.gameObject.SetActive(false);
		UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.patrolTonnage = 0f;
		UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.neutralCasualties = new float[3];
		UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.patrolPoints = 0f;
		GameDataManager.playervesselsonlevel[0].damagesystem.shipCurrentDamagePoints = 0f;
		UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.vesselTotalDamage = 0f;
		UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.decalNames = null;
		foreach (object obj in GameDataManager.playervesselsonlevel[0].meshHolder.transform)
		{
			Transform transform = (Transform)obj;
			if (transform.gameObject.name == "decal")
			{
				UnityEngine.Object.Destroy(transform.gameObject);
			}
		}
		GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.noisemakersOnBoard = GameDataManager.playervesselsonlevel[0].databaseshipdata.numberofnoisemakers;
		UIFunctions.globaluifunctions.campaignmanager.SetPlayerCampaignData();
		SaveLoadManager.AutoSaveCampaign();
		this.ContinueCourse();
	}

	// Token: 0x06000894 RID: 2196 RVA: 0x00061A50 File Offset: 0x0005FC50
	public void ContinueCourse()
	{
		UIFunctions.globaluifunctions.campaignmanager.SetPlayerCampaignData();
		UIFunctions.globaluifunctions.HUDCameraObject.enabled = false;
		UIFunctions.globaluifunctions.SetMenuSystem("CAMPAIGN");
		UIFunctions.globaluifunctions.backgroundTemplate.SetActive(false);
		UIFunctions.globaluifunctions.campaignmanager.enabled = true;
		Time.timeScale = UIFunctions.globaluifunctions.campaignmanager.mapTimeCompression;
		UIFunctions.globaluifunctions.campaignmanager.strategicMapToolbar.SetActive(true);
		UIFunctions.globaluifunctions.playerfunctions.menuPanel.SetActive(false);
		UIFunctions.globaluifunctions.playerfunctions.currentOpenPanel = 0;
	}

	// Token: 0x06000895 RID: 2197 RVA: 0x00061AFC File Offset: 0x0005FCFC
	public void SelectShip()
	{
		UIFunctions.globaluifunctions.keybindManagerMuseum.unitsToDisplay = new int[0];
		UIFunctions.globaluifunctions.keybindManagerMuseum.unitsToDisplay = UIFunctions.globaluifunctions.playerfunctions.playerVesselList;
		KeybindManagerMuseum.selectionScreen = true;
		UIFunctions.globaluifunctions.levelloadmanager.currentMuseumItem = UIFunctions.globaluifunctions.keybindManagerMuseum.unitsToDisplay[0];
		if (!GameDataManager.trainingMode && !GameDataManager.missionMode)
		{
			UIFunctions.globaluifunctions.keybindManagerMuseum.unitsToDisplay = UIFunctions.globaluifunctions.campaignmanager.GetClassesAvailable();
			if (UIFunctions.globaluifunctions.keybindManagerMuseum.unitsToDisplay.Length == 0)
			{
				UIFunctions.globaluifunctions.campaignmanager.eventManager.campaignOver = true;
				UIFunctions.globaluifunctions.campaignmanager.enabled = false;
				UIFunctions.globaluifunctions.campaignmanager.eventManager.continueButton.SetActive(false);
				UIFunctions.globaluifunctions.SetMenuSystem("EVENT");
				UIFunctions.globaluifunctions.scrollbarDefault.value = 1f;
				UIFunctions.globaluifunctions.campaignmanager.eventManager.BringInCampaignSummary();
				EventTemplate component = UIFunctions.globaluifunctions.campaignmanager.eventManager.currentEventTemplate.GetComponent<EventTemplate>();
				component.templateTexts[2].text = UIFunctions.globaluifunctions.textparser.GetRandomTextLineFromFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/ending_no_vessels"));
				return;
			}
			UIFunctions.globaluifunctions.levelloadmanager.currentMuseumItem = UIFunctions.globaluifunctions.keybindManagerMuseum.unitsToDisplay[0];
		}
		if (GameDataManager.trainingMode)
		{
			UIFunctions.globaluifunctions.helpmanager.trainingIndex++;
			UIFunctions.globaluifunctions.helpmanager.GetTrainingContent(UIFunctions.globaluifunctions.helpmanager.trainingIndex, "training_menu_" + (this.currentMissionID + 1).ToString(), "TRAINING_MENU", false);
			if (!UIFunctions.globaluifunctions.helpmanager.tutorialObject.activeSelf)
			{
				UIFunctions.globaluifunctions.helpmanager.tutorialObject.SetActive(true);
			}
		}
		UIFunctions.globaluifunctions.levelloadmanager.BackgroundMuseumRender(true);
		UIFunctions.globaluifunctions.keybindManagerMuseum.selectShipButton.gameObject.SetActive(true);
		UIFunctions.globaluifunctions.keybindManagerMuseum.randomShipButton.gameObject.SetActive(true);
		UIFunctions.globaluifunctions.keybindManagerMuseum.nextPrevButton.gameObject.SetActive(true);
		UIFunctions.globaluifunctions.ClearSelectionToggleBars();
		UIFunctions.globaluifunctions.selectionGroupPanel.SetActive(false);
		if (UIFunctions.globaluifunctions.keybindManagerMuseum.unitsToDisplay.Length == 1)
		{
			this.SelectShipAccept();
		}
	}

	// Token: 0x06000896 RID: 2198 RVA: 0x00061DBC File Offset: 0x0005FFBC
	public void RandomShipAccept()
	{
		UIFunctions.globaluifunctions.levelloadmanager.currentMuseumItem = UIFunctions.globaluifunctions.playerfunctions.playerVesselList[UnityEngine.Random.Range(0, UIFunctions.globaluifunctions.playerfunctions.playerVesselList.Length)];
		UIFunctions.globaluifunctions.levelloadmanager.BackgroundMuseumRender(false);
		this.SelectShipAccept();
	}

	// Token: 0x06000897 RID: 2199 RVA: 0x00061E18 File Offset: 0x00060018
	private void FixedUpdate()
	{
		if (this.loadTheLevel)
		{
			this.loadTheLevel = false;
			this.LoadTheLevelAndBriefing();
		}
		else if (this.loadTheCampaign)
		{
			this.loadTheCampaign = false;
			UIFunctions.globaluifunctions.database.saveloadmanager.LoadTheCampaign();
		}
	}

	// Token: 0x06000898 RID: 2200 RVA: 0x00061E68 File Offset: 0x00060068
	private void LoadTheLevelAndBriefing()
	{
		Camera component = UIFunctions.globaluifunctions.MainCamera.GetComponent<Camera>();
		component.depth = 0f;
		component.clearFlags = CameraClearFlags.Color;
		component.rect = new Rect(0f, 0f, 1f, 1f);
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.rnShipClasses = new int[1];
		if (GameDataManager.trainingMode || GameDataManager.missionMode)
		{
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.rnShipClasses[0] = UIFunctions.globaluifunctions.playerfunctions.playerVesselClass;
			if (GameDataManager.trainingMode)
			{
				UIFunctions.globaluifunctions.helpmanager.trainingIndex++;
				UIFunctions.globaluifunctions.helpmanager.GetTrainingContent(UIFunctions.globaluifunctions.helpmanager.trainingIndex, "training_menu_" + (this.currentMissionID + 1).ToString(), "TRAINING_MENU", false);
			}
			this.LoadTraining();
		}
		else if (UIFunctions.globaluifunctions.missionmanager.playerSunk || UIFunctions.globaluifunctions.missionmanager.abandonedShip)
		{
			UIFunctions.globaluifunctions.missionmanager.playerSunk = false;
			UIFunctions.globaluifunctions.missionmanager.abandonedShip = false;
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.rnShipClasses[0] = UIFunctions.globaluifunctions.playerfunctions.playerVesselClass;
			UIFunctions.globaluifunctions.campaignmanager.playerInPort = true;
			UIFunctions.globaluifunctions.campaignmanager.timeInPort = UIFunctions.globaluifunctions.campaignmanager.timePenaltyOnSunk * OptionsManager.difficultySettings["RescueTimeModifier"];
			UIFunctions.globaluifunctions.campaignmanager.SetupPlayerSubmarine();
			UIFunctions.globaluifunctions.campaignmanager.SetSubmarineClassDefaultData();
			CampaignManager.isEncounter = false;
			UIFunctions.globaluifunctions.campaignmanager.OpenMissionBriefing(0);
			UIFunctions.globaluifunctions.campaignmanager.EndCampaignCombat();
		}
		else
		{
			UIFunctions.globaluifunctions.campaignmanager.StartNewCampaign(true);
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.rnShipClasses[0] = UIFunctions.globaluifunctions.playerfunctions.playerVesselClass;
		}
		UIFunctions.globaluifunctions.keybindManagerMenu.enabled = true;
	}

	// Token: 0x06000899 RID: 2201 RVA: 0x000620AC File Offset: 0x000602AC
	public void SelectShipAccept()
	{
		UIFunctions.globaluifunctions.keybindManagerMuseum.randomShipButton.gameObject.SetActive(false);
		UIFunctions.globaluifunctions.keybindManagerMuseum.nextPrevButton.gameObject.SetActive(false);
		if (!this.assignedShip)
		{
			this.assignedShip = true;
			UIFunctions.globaluifunctions.playerfunctions.playerVesselClass = UIFunctions.globaluifunctions.levelloadmanager.currentMuseumItem;
			if (!GameDataManager.trainingMode && !GameDataManager.missionMode)
			{
				UIFunctions.globaluifunctions.playerfunctions.playerVesselInstance = UIFunctions.globaluifunctions.campaignmanager.GetValidVesselInstance();
			}
			else
			{
				UIFunctions.globaluifunctions.playerfunctions.playerVesselInstance = UnityEngine.Random.Range(0, UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].playerClassNames.Length);
				if (GameDataManager.trainingMode)
				{
					UIFunctions.globaluifunctions.helpmanager.trainingIndex++;
					UIFunctions.globaluifunctions.helpmanager.GetTrainingContent(UIFunctions.globaluifunctions.helpmanager.trainingIndex, "training_menu_" + (this.currentMissionID + 1).ToString(), "TRAINING_MENU", false);
				}
			}
			this.SetAssignedShipText();
			return;
		}
		UIFunctions.globaluifunctions.loadingMask.gameObject.SetActive(true);
		this.loadTheLevel = true;
		Time.timeScale = 1f;
	}

	// Token: 0x0600089A RID: 2202 RVA: 0x0006221C File Offset: 0x0006041C
	private void SetAssignedShipText()
	{
		UIFunctions.globaluifunctions.mainColumn.text = string.Empty;
		UIFunctions.globaluifunctions.secondColumm.text = string.Empty;
		string text = UIFunctions.globaluifunctions.textparser.ReadDirectTextFromFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("language/message/assignment"));
		text = UIFunctions.globaluifunctions.textparser.PopulateDynamicTags(text, true);
		UIFunctions.globaluifunctions.mainColumn.text = text;
		UIFunctions.globaluifunctions.SetMainColumnHeight(false);
	}

	// Token: 0x0600089B RID: 2203 RVA: 0x000622A4 File Offset: 0x000604A4
	public void LoadTraining()
	{
		this.uifunctions.levelloadmanager.missionCombat = true;
		this.selectionGUI.SetActive(false);
		this.uifunctions.levelloadmanager.LoadLevel();
	}

	// Token: 0x0600089C RID: 2204 RVA: 0x000622D4 File Offset: 0x000604D4
	public void BackToMain()
	{
		UIFunctions.globaluifunctions.loadingMask.SetActive(true);
		AudioManager.audiomanager.musicSources[0].gameObject.SetActive(false);
		AudioManager.audiomanager.musicSources[1].gameObject.SetActive(false);
		this.ReloadMainMenu();
	}

	// Token: 0x0600089D RID: 2205 RVA: 0x00062328 File Offset: 0x00060528
	public void ReloadMainMenu()
	{
		UIFunctions.globaluifunctions.loadingMask.SetActive(true);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	// Token: 0x0600089E RID: 2206 RVA: 0x00062358 File Offset: 0x00060558
	private void ActivateTrainingHud()
	{
		UIFunctions.globaluifunctions.HUDholder.SetActive(true);
		UIFunctions.globaluifunctions.levelloadmanager.HUDCamera.active = true;
		UIFunctions.globaluifunctions.HUDCameraObject.enabled = true;
		UIFunctions.globaluifunctions.playerfunctions.menuPanel.SetActive(false);
		UIFunctions.globaluifunctions.playerfunctions.otherPanel.SetActive(false);
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.zoomText.text = string.Empty;
	}

	// Token: 0x0600089F RID: 2207 RVA: 0x000623E8 File Offset: 0x000605E8
	public void InitialiseMissionList(string MissionType)
	{
		UIFunctions.globaluifunctions.selectionGroupText.text = null;
		string filename = string.Empty;
		if (MissionType == "TRAINING")
		{
			GameDataManager.trainingMode = true;
			GameDataManager.missionMode = false;
			GameDataManager.campaignMode = false;
			filename = UIFunctions.globaluifunctions.textparser.GetFilePathFromString("language/training/missions_training");
			this.uifunctions.mainTitle.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "TrainingMissionHeader");
			this.ActivateTrainingHud();
			UIFunctions.globaluifunctions.helpmanager.trainingIndex = 1;
			UIFunctions.globaluifunctions.helpmanager.GetTrainingContent(UIFunctions.globaluifunctions.helpmanager.trainingIndex, "training_menu_" + (this.currentMissionID + 1).ToString(), "TRAINING_MENU", false);
		}
		else if (MissionType == "SINGLE")
		{
			GameDataManager.trainingMode = false;
			GameDataManager.missionMode = true;
			GameDataManager.campaignMode = false;
			filename = UIFunctions.globaluifunctions.textparser.GetFilePathFromString("language/mission/missions_single");
			this.uifunctions.mainTitle.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "SingleMissionHeader");
			EditorMission.instance.quickMissionButton.SetActive(true);
		}
		else if (MissionType == "CAMPAIGN")
		{
			GameDataManager.trainingMode = false;
			GameDataManager.missionMode = false;
			filename = UIFunctions.globaluifunctions.textparser.GetFilePathFromString("language/campaign/campaigns");
		}
		this.uifunctions.SetMenuSystem("SELECTION");
		this.uifunctions.scrollbarDefault.gameObject.SetActive(false);
		this.BuildMissionMenu(filename);
		this.currentMission = 0;
	}

	// Token: 0x060008A0 RID: 2208 RVA: 0x0006258C File Offset: 0x0006078C
	public void AbandonShip()
	{
		this.abandonedShip = true;
		if ((float)UIFunctions.globaluifunctions.playerfunctions.playerDepthInFeet > GameDataManager.playervesselsonlevel[0].databaseshipdata.escapeDepth)
		{
			this.abandonedShip = false;
			GameDataManager.playervesselsonlevel[0].isSinking = true;
			this.playerSunk = true;
		}
		this.uifunctions.EndLevel();
	}

	// Token: 0x060008A1 RID: 2209 RVA: 0x000625EC File Offset: 0x000607EC
	private void BuildMissionMenu(string filename)
	{
		UIFunctions.globaluifunctions.ClearSelectionToggleBars();
		UIFunctions.globaluifunctions.scrollbarDefault.gameObject.SetActive(false);
		UIFunctions.globaluifunctions.selectionGroupPanel.SetActive(true);
		string[] array = UIFunctions.globaluifunctions.textparser.OpenTextDataFile(filename);
		UIFunctions.globaluifunctions.selectionBars = new GameObject[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.campaignmanager.savegameBar, this.missionListToggles.transform.position, Quaternion.identity) as GameObject;
			gameObject.transform.SetParent(UIFunctions.globaluifunctions.selectionToggleGroup.transform, false);
			gameObject.transform.localPosition = Vector2.zero;
			UIFunctions.globaluifunctions.selectionBars[i] = gameObject;
			Text componentInChildren = gameObject.GetComponentInChildren<Text>();
			componentInChildren.text = (i + 1).ToString() + ". " + array[i];
			Toggle toggle = gameObject.GetComponent<Toggle>();
			toggle.name = i.ToString();
			toggle.group = this.missionListToggles;
			toggle.onValueChanged.AddListener(delegate(bool value)
			{
				this.SetMissionNumber(int.Parse(toggle.name));
			});
			if (i == 0)
			{
				toggle.isOn = true;
			}
		}
		RectTransform component = UIFunctions.globaluifunctions.selectionToggleGroup.gameObject.GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(component.sizeDelta.x, (float)UIFunctions.globaluifunctions.selectionBars.Length * GameDataManager.menuScrollListSpacing + 10f);
	}

	// Token: 0x04000D16 RID: 3350
	public UIFunctions uifunctions;

	// Token: 0x04000D17 RID: 3351
	public UIScrollList missionRoster;

	// Token: 0x04000D18 RID: 3352
	public UIScrollList rnRoster;

	// Token: 0x04000D19 RID: 3353
	public UIScrollList kmRoster;

	// Token: 0x04000D1A RID: 3354
	public Transform map;

	// Token: 0x04000D1B RID: 3355
	public GameObject selectionGUI;

	// Token: 0x04000D1C RID: 3356
	public GameObject toolbar;

	// Token: 0x04000D1D RID: 3357
	public SpriteText missionTitle;

	// Token: 0x04000D1E RID: 3358
	public SpriteText missionSubHeader;

	// Token: 0x04000D1F RID: 3359
	public SpriteText missionDate;

	// Token: 0x04000D20 RID: 3360
	public SpriteText missionTime;

	// Token: 0x04000D21 RID: 3361
	public PackedSprite weatherDisplay;

	// Token: 0x04000D22 RID: 3362
	public Color32 colorGray;

	// Token: 0x04000D23 RID: 3363
	public Color32 colorBlue;

	// Token: 0x04000D24 RID: 3364
	public Color32 colorYellow;

	// Token: 0x04000D25 RID: 3365
	public Color32 colorRed;

	// Token: 0x04000D26 RID: 3366
	public Color32 colorGreen;

	// Token: 0x04000D27 RID: 3367
	public Color32 colorOrange;

	// Token: 0x04000D28 RID: 3368
	public int currentMission;

	// Token: 0x04000D29 RID: 3369
	public int currentMissionID;

	// Token: 0x04000D2A RID: 3370
	public int lastRNShip;

	// Token: 0x04000D2B RID: 3371
	public int lastKMShip;

	// Token: 0x04000D2C RID: 3372
	public GameObject attackbutton;

	// Token: 0x04000D2D RID: 3373
	public GameObject customiseToolbar;

	// Token: 0x04000D2E RID: 3374
	public bool missionport;

	// Token: 0x04000D2F RID: 3375
	public int[] kmships;

	// Token: 0x04000D30 RID: 3376
	public int[] rnships;

	// Token: 0x04000D31 RID: 3377
	public int[] kminstances;

	// Token: 0x04000D32 RID: 3378
	public int[] rninstances;

	// Token: 0x04000D33 RID: 3379
	public UIStateToggleBtn rnPlanes;

	// Token: 0x04000D34 RID: 3380
	public UIStateToggleBtn kmPlanes;

	// Token: 0x04000D35 RID: 3381
	public UIStateToggleBtn rnPlaneChoice;

	// Token: 0x04000D36 RID: 3382
	public UIStateToggleBtn kmPlaneChoice;

	// Token: 0x04000D37 RID: 3383
	public GameObject briefBackground;

	// Token: 0x04000D38 RID: 3384
	public SpriteText briefText;

	// Token: 0x04000D39 RID: 3385
	public GameObject tutorialPages;

	// Token: 0x04000D3A RID: 3386
	public GameObject battleStationsButton;

	// Token: 0x04000D3B RID: 3387
	public GameObject continueCourseButton;

	// Token: 0x04000D3C RID: 3388
	public GameObject leavePortButton;

	// Token: 0x04000D3D RID: 3389
	public GameObject ordersButton;

	// Token: 0x04000D3E RID: 3390
	public GameObject statusButton;

	// Token: 0x04000D3F RID: 3391
	public GameObject rearmButton;

	// Token: 0x04000D40 RID: 3392
	public GameObject closureButton;

	// Token: 0x04000D41 RID: 3393
	public Text closureRangeDisplay;

	// Token: 0x04000D42 RID: 3394
	public float closureValue;

	// Token: 0x04000D43 RID: 3395
	public Vector3 closureParams;

	// Token: 0x04000D44 RID: 3396
	public GameObject backButton;

	// Token: 0x04000D45 RID: 3397
	public ToggleGroup missionListToggles;

	// Token: 0x04000D46 RID: 3398
	public GameObject missionSelectBar;

	// Token: 0x04000D47 RID: 3399
	public string briefingTitle;

	// Token: 0x04000D48 RID: 3400
	public string briefingNoContact;

	// Token: 0x04000D49 RID: 3401
	public string[] briefingPrimaryText;

	// Token: 0x04000D4A RID: 3402
	public string[] briefingSecondaryText;

	// Token: 0x04000D4B RID: 3403
	public GameObject missionExitMenu;

	// Token: 0x04000D4C RID: 3404
	public GameObject sinkAllButton;

	// Token: 0x04000D4D RID: 3405
	public bool abandonedShip;

	// Token: 0x04000D4E RID: 3406
	public bool playerSunk;

	// Token: 0x04000D4F RID: 3407
	public bool assignedShip;

	// Token: 0x04000D50 RID: 3408
	public static string initialBearing;

	// Token: 0x04000D51 RID: 3409
	public int initialIndex;

	// Token: 0x04000D52 RID: 3410
	public Text exitMissionTitle;

	// Token: 0x04000D53 RID: 3411
	public Text exitMissionDate;

	// Token: 0x04000D54 RID: 3412
	public Text exitMissionMain;

	// Token: 0x04000D55 RID: 3413
	public GameObject abandonShipButton;

	// Token: 0x04000D56 RID: 3414
	public GameObject leaveCombatButton;

	// Token: 0x04000D57 RID: 3415
	public GameObject quitCombatButton;

	// Token: 0x04000D58 RID: 3416
	public bool playerNeedsANewCampaignSub;

	// Token: 0x04000D59 RID: 3417
	public bool loadTheLevel;

	// Token: 0x04000D5A RID: 3418
	public bool loadTheCampaign;

	// Token: 0x04000D5B RID: 3419
	public float contactLeaveDist;

	// Token: 0x04000D5C RID: 3420
	public float aircraftLeaveDist;

	// Token: 0x04000D5D RID: 3421
	public float weaponLeaveDist;

	// Token: 0x04000D5E RID: 3422
	public bool playerWasBehind;
}
