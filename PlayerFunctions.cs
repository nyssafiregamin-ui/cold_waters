using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000142 RID: 322
public class PlayerFunctions : MonoBehaviour
{
	// Token: 0x060008E6 RID: 2278 RVA: 0x00064FFC File Offset: 0x000631FC
	private void Start()
	{
		this.homeSettingDefinitions = new string[]
		{
			"ACTIVE",
			"PASSIVE"
		};
		this.attackSettingDefinitions = new string[]
		{
			"STRAIGHT",
			"SNAKE",
			"LEFT",
			"RIGHT",
			"WIDE",
			"NARROW"
		};
		this.depthSettingDefinitions = new string[]
		{
			"SHALLOW",
			"LEVEL",
			"DEEP",
			"SKIM",
			"POP-UP"
		};
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x00065098 File Offset: 0x00063298
	private void Update()
	{
		if (PlayerFunctions.draggingWaypoint)
		{
			this.DraggingWaypointUpdate();
			this.sensormanager.tacticalmap.waypointMarker.gameObject.transform.localPosition = new Vector3(this.sensormanager.tacticalmap.waypointMarker.gameObject.transform.localPosition.x, this.sensormanager.tacticalmap.waypointMarker.gameObject.transform.localPosition.y, 0f);
			this.sensormanager.tacticalmap.waypointMarker.gameObject.SetActive(true);
			this.sensormanager.tacticalmap.waypointWorldMarker.transform.position = new Vector3(this.sensormanager.tacticalmap.waypointMarker.transform.localPosition.x / this.sensormanager.tacticalmap.zoomFactor, 1000f, this.sensormanager.tacticalmap.waypointMarker.transform.localPosition.y / this.sensormanager.tacticalmap.zoomFactor);
			string[] waypointDetails = this.sensormanager.tacticalmap.GetWaypointDetails();
			string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "WaypointSet");
			text = text.Replace("<BRG>", waypointDetails[0]);
			text = text.Replace("<RANGE>", waypointDetails[1]);
			text = text.Replace("<KILOYARD>", LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "KiloYard"));
			this.sensormanager.tacticalmap.waypointReadout.text = text;
			int playerTorpedoIDInTube = this.GetPlayerTorpedoIDInTube(UIFunctions.globaluifunctions.playerfunctions.playerVessel.vesselmovement.weaponSource.activeTube);
			float num = float.Parse(waypointDetails[1]) * 1000f;
			if (num < UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTube].missileFiringRange.x || num > UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTube].rangeInYards)
			{
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.waypointMarker.gameObject.GetComponent<Image>().color = Color.red;
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.waypointMarker.gameObject.GetComponent<LineRenderer>().SetColors(Color.red, Color.red);
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.waypointReadout.color = Color.red;
			}
			else
			{
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.waypointMarker.gameObject.GetComponent<Image>().color = UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.waypointMapColor;
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.waypointMarker.gameObject.GetComponent<LineRenderer>().SetColors(UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.waypointMapColor, UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.waypointMapColor);
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.waypointReadout.color = UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.waypointReadoutColor;
			}
			if (InputManager.globalInputManager.GetButtonDown("Set Waypoint", false) && (TacticalMap.tacMapEnabled || UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.IsMouseOverTacMiniMap()))
			{
				if (UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTube].weaponType == "MISSILE")
				{
					float num2 = UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTube].missileFiringRange.x / 1000f;
					string[] waypointDetails2 = this.sensormanager.tacticalmap.GetWaypointDetails();
					if (float.Parse(waypointDetails2[1]) < num2)
					{
						string text2 = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "BelowMinRange");
						text2 = text2.Replace("<WEAPON>", UIFunctions.globaluifunctions.database.databaseweapondata[this.GetPlayerTorpedoIDInTube(this.playerVessel.vesselmovement.weaponSource.activeTube)].weaponName);
						text2 = text2.Replace("<RANGE>", num2.ToString());
						text2 = text2.Replace("<KILOYARD>", LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "KiloYard"));
						this.PlayerMessage(text2, this.messageLogColors["TorpedoRoom"], "BelowMinRange", true);
						return;
					}
				}
				this.sensormanager.tacticalmap.waypointReadout.text = string.Empty;
				this.sensormanager.tacticalmap.SetWaypointMarker();
				this.DisableWaypointDragging();
				if (this.currentActiveTorpedo != null)
				{
					this.playerVessel.vesselmovement.weaponSource.SetWeaponWaypointData(this.currentActiveTorpedo);
					this.currentActiveTorpedo.playerTimeToWaypoint = -100f;
					this.currentActiveTorpedo.playerControlling = false;
				}
				else
				{
					this.playerVessel.vesselmovement.weaponSource.FireTube();
				}
			}
		}
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x000655E8 File Offset: 0x000637E8
	public void ClearStatusIcons()
	{
		for (int i = 0; i < this.statusIcons.Length; i++)
		{
			this.statusIcons[i].gameObject.SetActive(false);
		}
		this.statusIconsParent.SetActive(true);
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x00065630 File Offset: 0x00063830
	public void SetStatusIcon(string statusName, bool statusValue)
	{
		for (int i = 0; i < this.statusIcons.Length; i++)
		{
			if (this.statusIcons[i].name == statusName)
			{
				this.statusIcons[i].gameObject.SetActive(statusValue);
				break;
			}
		}
		if (statusName == "periscope")
		{
			UIFunctions.globaluifunctions.playerfunctions.helmmanager.periscopePanel.SetActive(statusValue);
		}
		this.BuildStatusIconBar();
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x000656B8 File Offset: 0x000638B8
	private void BuildStatusIconBar()
	{
		int num = 0;
		for (int i = 0; i < UIFunctions.globaluifunctions.playerfunctions.statusIconOrder.Length; i++)
		{
			if (UIFunctions.globaluifunctions.playerfunctions.statusIcons[UIFunctions.globaluifunctions.playerfunctions.statusIconOrder[i]].gameObject.activeSelf)
			{
				UIFunctions.globaluifunctions.playerfunctions.statusIcons[UIFunctions.globaluifunctions.playerfunctions.statusIconOrder[i]].transform.localPosition = new Vector3(-40f + (float)num * -40f, 0f, 0f);
				num++;
			}
		}
	}

	// Token: 0x060008EB RID: 2283 RVA: 0x00065768 File Offset: 0x00063968
	private void FadeLog()
	{
		for (int i = 0; i < this.messageLogAlphas.Length; i++)
		{
			if (this.messageLogAlphas[i] > 0f)
			{
				this.messageLogAlphas[i] = this.messageLogAlphas[i] - Time.deltaTime / Time.timeScale * this.messageLogFadeRate;
				if (this.messageLogAlphas[i] < 0f)
				{
					this.messageLogAlphas[i] = 0f;
				}
				this.messageLog[i].color = new Color(this.messageLog[i].color.r, this.messageLog[i].color.g, this.messageLog[i].color.b, this.messageLogAlphas[i]);
				this.messageLogBackgrounds[i].color = new Color(this.messageLogBackgrounds[i].color.r, this.messageLogBackgrounds[i].color.g, this.messageLogBackgrounds[i].color.b, this.messageLogAlphas[i] / 2f);
			}
		}
	}

	// Token: 0x060008EC RID: 2284 RVA: 0x000658A0 File Offset: 0x00063AA0
	public void CheckMastExposure()
	{
		if (this.playerVessel.vesselmovement.shipSpeed.z > 1.2f && this.playerDepthInFeet > 40)
		{
			for (int i = 0; i < this.playerVessel.submarineFunctions.mastTransforms.Length; i++)
			{
				if (this.playerVessel.submarineFunctions.GetMastIsExposed(i))
				{
					string subsystem = string.Empty;
					switch (i)
					{
					case 0:
						subsystem = "PERISCOPE";
						break;
					case 1:
						subsystem = "ESM_MAST";
						break;
					case 2:
						subsystem = "RADAR_MAST";
						break;
					}
					int subsystemIndex = DamageControl.GetSubsystemIndex(subsystem);
					if (this.damagecontrol.damageControlCurrentTimers[subsystemIndex] != 10000f)
					{
						bool destroy = false;
						if (this.damagecontrol.damageControlCurrentTimers[subsystemIndex] > 500f)
						{
							destroy = true;
						}
						this.damagecontrol.KnockoutSubsystem(subsystem, destroy);
					}
				}
			}
		}
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x0006599C File Offset: 0x00063B9C
	private void FixedUpdate()
	{
		if (this.ballastRechargeTimer > 0f && this.playerVessel.vesselmovement.percentageSurfaced > 0.9f)
		{
			this.ballastRechargeTimer -= Time.deltaTime;
			if (this.ballastRechargeTimer <= 0f)
			{
				this.PlayerMessage(LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "BallastRecharged"), this.messageLogColors["Helm"], "BallastRecharged", false);
				this.helmmanager.guiButtonImages[7].color = this.helmmanager.buttonColors[0];
				if (this.playerVessel.vesselmovement.blowBallast)
				{
					this.playerVessel.vesselmovement.blowBallast = false;
					this.playerVessel.vesselmovement.ballastAngle = Vector2.zero;
				}
			}
		}
		this.FadeLog();
		if (this.playerVessel != null)
		{
			this.playerStatTimer += Time.deltaTime;
			if (this.playerStatTimer > 0.3f)
			{
				this.SetPlayerData();
				if (this.currentActiveTorpedo != null)
				{
					this.SetWireDataText();
				}
				this.playerStatTimer -= 0.2f;
				if (Mathf.Abs(this.playerVessel.transform.position.x) > 2303f || Mathf.Abs(this.playerVessel.transform.position.z) > 2303f)
				{
					UIFunctions.globaluifunctions.missionmanager.BringInExitMenu(true);
				}
			}
		}
		if (this.playerVessel != null && this.playerVessel.acoustics.usingActiveSonar && !this.playerVessel.isSinking)
		{
			this.activeSonarPingTimer += Time.deltaTime;
			if (this.activeSonarPingTimer > this.activeSonarPingRate)
			{
				if (Time.timeScale == 1f)
				{
					this.playerVessel.vesselmovement.pingSound.Play();
				}
				this.activeSonarPingTimer -= this.activeSonarPingRate;
			}
		}
		if (this.sensormanager.tacticalmap.missionMarker.gameObject.activeSelf && !this.playerVessel.isSinking && this.playerVessel.vesselmovement.weaponSource.sealsOnBoard && !UIFunctions.globaluifunctions.campaignmanager.eventManager.sealsReleased && Vector2.Distance(new Vector2(this.playerVessel.transform.position.x, this.playerVessel.transform.position.z), UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.missionPosition) < 20f)
		{
			this.inMissionZoneTimer += Time.deltaTime;
			if (Mathf.Abs(this.playerVessel.vesselmovement.shipSpeed.z) < 0.6f && this.playerDepthInFeet < 100)
			{
				UIFunctions.globaluifunctions.campaignmanager.eventManager.sealsReleased = true;
				GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.sealsOnBoard = false;
				string dictionaryString = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "CommandoDeployed");
				this.PlayerMessage(dictionaryString, this.messageLogColors["XO"], "CommandoDeployed", false);
			}
			else if (this.inMissionZoneTimer > 30f)
			{
				this.inMissionZoneTimer = 0f;
				string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "InDeploymentZone");
				text = text.Replace("<FEET>", LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Feet").ToLower());
				this.PlayerMessage(text, this.messageLogColors["XO"], "InDeploymentZone", false);
			}
		}
	}

	// Token: 0x060008EE RID: 2286 RVA: 0x00065DA4 File Offset: 0x00063FA4
	public void SetESMMeter(float intensity)
	{
		if (!this.playerVessel.submarineFunctions.GetMastIsUp(0) && !this.playerVessel.submarineFunctions.GetMastIsUp(1))
		{
			this.DisableESMMeter();
			return;
		}
		if (!this.esmGameObject.activeSelf)
		{
			this.EnableESMMeter();
		}
		this.esmIntensityDisplay.value = (float)(9 - (int)intensity);
	}

	// Token: 0x060008EF RID: 2287 RVA: 0x00065E0C File Offset: 0x0006400C
	public void EnableESMMeter()
	{
		if (this.damagecontrol.CheckSubsystem("ESM_MAST", false))
		{
			this.esmGameObject.SetActive(true);
			this.SetESMMeter(0f);
		}
	}

	// Token: 0x060008F0 RID: 2288 RVA: 0x00065E3C File Offset: 0x0006403C
	public void DisableESMMeter()
	{
		this.esmGameObject.SetActive(false);
	}

	// Token: 0x060008F1 RID: 2289 RVA: 0x00065E4C File Offset: 0x0006404C
	public void HUDTacMap()
	{
		if (this.tacMapMaximisedGraphic.enabled)
		{
			this.tacMapMaximisedGraphic.enabled = false;
			this.tacMapMaximisedGraphic.transform.localPosition = new Vector3(332f, -232.5f, 0f);
			this.sensormanager.tacticalmap.minimapIsOpen = false;
			this.sensormanager.tacticalmap.SetTacMiniMap(false);
			this.tacMapMiniMapButtons[0].interactable = true;
			this.tacMapMinimisedGraphic.enabled = true;
		}
		else
		{
			this.tacMapMaximisedGraphic.enabled = true;
			this.tacMapMaximisedGraphic.transform.localPosition = new Vector3(332f, 0f, 0f);
			this.sensormanager.tacticalmap.minimapIsOpen = true;
			this.sensormanager.tacticalmap.SetTacMiniMap(true);
			this.tacMapMiniMapButtons[0].interactable = false;
			this.tacMapMinimisedGraphic.enabled = false;
		}
	}

	// Token: 0x060008F2 RID: 2290 RVA: 0x00065F48 File Offset: 0x00064148
	private void SetMessageLogPosition(bool up)
	{
		if (!up)
		{
			this.messageLogPanel.transform.localPosition = new Vector3(0f, this.messageLogPositions.x, 0f);
		}
		else
		{
			this.messageLogPanel.transform.localPosition = new Vector3(0f, this.messageLogPositions.y, 0f);
		}
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x00065FB4 File Offset: 0x000641B4
	public void CloseAllContextualPanels()
	{
		for (int i = 0; i < this.contextualPanels.Length; i++)
		{
			this.contextualPanels[i].SetActive(false);
		}
	}

	// Token: 0x060008F4 RID: 2292 RVA: 0x00065FE8 File Offset: 0x000641E8
	public void OpenContextualPanel(int index)
	{
		if (UIFunctions.globaluifunctions.playerfunctions.helmmanager.currentTooltip == "Conditions" || UIFunctions.globaluifunctions.playerfunctions.helmmanager.currentTooltip == "Signature" || UIFunctions.globaluifunctions.playerfunctions.helmmanager.currentTooltip == "Stores" || UIFunctions.globaluifunctions.playerfunctions.helmmanager.currentTooltip == "Damage Report")
		{
			UIFunctions.globaluifunctions.playerfunctions.helmmanager.currentTooltip = string.Empty;
			UIFunctions.globaluifunctions.playerfunctions.helmmanager.toolTipText.text = string.Empty;
		}
		bool flag = false;
		if (UIFunctions.globaluifunctions.menuSystemParent.activeSelf)
		{
			flag = true;
			if (!this.menuPanel.activeSelf)
			{
				this.menuPanel.gameObject.SetActive(true);
				this.contextualPanels[0].SetActive(false);
				this.contextualPanels[1].SetActive(false);
				this.messageLogPanel.SetActive(false);
			}
		}
		if (index < 2 && flag)
		{
			return;
		}
		if (index != this.currentOpenPanel)
		{
			for (int i = 0; i < this.contextualPanels.Length; i++)
			{
				if (!flag)
				{
					this.contextualPanels[i].SetActive(true);
				}
				else if (i < 2)
				{
					this.contextualPanels[i].SetActive(false);
				}
				else
				{
					this.contextualPanels[i].SetActive(true);
				}
				this.contextualPanelLabels[i].color = new Color(1f, 1f, 1f, 0.5f);
				if (index == i)
				{
					this.contextualPanelLabels[i].color = Color.white;
					this.contextualPanels[i].transform.SetAsLastSibling();
					this.currentOpenPanel = i;
					switch (this.currentOpenPanel)
					{
					case 0:
						UIFunctions.globaluifunctions.playerfunctions.sensormanager.conditionsdisplay.UpdateConditionsDisplay(false);
						break;
					case 1:
						this.sensormanager.SetSignatureData();
						break;
					case 2:
						this.SetDamageControlLabels();
						break;
					case 3:
						UIFunctions.globaluifunctions.portRearm.SetLoadoutStats();
						break;
					}
				}
			}
			this.SetMessageLogPosition(true);
			return;
		}
		if (flag)
		{
			this.currentOpenPanel = 0;
			this.menuPanel.SetActive(false);
			return;
		}
		for (int j = 0; j < this.contextualPanels.Length; j++)
		{
			this.contextualPanels[j].SetActive(false);
		}
		this.currentOpenPanel = -1;
		this.SetMessageLogPosition(false);
	}

	// Token: 0x060008F5 RID: 2293 RVA: 0x000662AC File Offset: 0x000644AC
	public void SetDamageControlLabels()
	{
		for (int i = 0; i < this.damagecontrol.damageControlCurrentTimers.Length; i++)
		{
			this.damagecontrol.DisplayDamageLabelColors(i);
		}
		int num = Mathf.RoundToInt((1f - this.playerVessel.damagesystem.shipCurrentDamagePoints / this.playerVessel.damagesystem.shipTotalDamagePoints) * 100f);
		UIFunctions.globaluifunctions.playerfunctions.damagecontrol.hullStatusReadout.text = LanguageManager.interfaceDictionary["HullStatus"] + " " + num.ToString() + LanguageManager.interfaceDictionary["Percentage"];
	}

	// Token: 0x060008F6 RID: 2294 RVA: 0x00066360 File Offset: 0x00064560
	public void OpenMenuStores()
	{
		this.messageLogPanel.SetActive(false);
		if (this.menuPanel.activeSelf)
		{
			this.menuPanel.SetActive(false);
		}
		else
		{
			this.menuPanel.gameObject.SetActive(true);
			this.currentOpenPanel = 0;
			this.OpenContextualPanel(3);
		}
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x000663BC File Offset: 0x000645BC
	public void SetContactGraphic()
	{
		float num = this.playerVessel.vesselmovement.percentageSpeed;
		num = Mathf.Clamp01(Mathf.Abs(num));
		this.signatureMaterials[1].SetFloat("_AmbientNoise", num);
		if (this.currentTargetIndex > -1)
		{
			float num2 = GameDataManager.enemyvesselsonlevel[this.currentTargetIndex].vesselai.sensordata.playerSignatureData[2];
			if (GameDataManager.enemyvesselsonlevel[this.currentTargetIndex].vesselai.sensordata.playerSignatureData[1] > num2)
			{
				num2 = GameDataManager.enemyvesselsonlevel[this.currentTargetIndex].vesselai.sensordata.playerSignatureData[1];
			}
			this.signatureMaterials[1].SetFloat("_SignalStrength", num2 / this.contactProfileMultiplier + 10f * num);
		}
		else
		{
			this.signatureMaterials[1].SetFloat("_SignalStrength", 0f);
		}
	}

	// Token: 0x060008F8 RID: 2296 RVA: 0x000664A4 File Offset: 0x000646A4
	public void SetContactProfileSprite(string graphicName)
	{
		this.signatureMaterials[1].SetTexture("_Signature", UIFunctions.globaluifunctions.textparser.GetTexture(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("vessels/signature/" + graphicName + ".png")));
	}

	// Token: 0x060008F9 RID: 2297 RVA: 0x000664F4 File Offset: 0x000646F4
	public void SetProfileGraphic()
	{
		string str = UIFunctions.globaluifunctions.database.databaseshipdata[this.otherVesselList[this.currentSignatureIndex]].shipPrefabName + "_sig";
		this.signatureMaterials[0].SetTexture("__MainTex", UIFunctions.globaluifunctions.textparser.GetTexture(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("vessels/signature/" + str + ".png")));
		this.signatureData[0].text = UIFunctions.globaluifunctions.database.databaseshipdata[this.otherVesselList[this.currentSignatureIndex]].shipclass.ToUpper() + " " + UIFunctions.globaluifunctions.database.databaseshipdata[this.otherVesselList[this.currentSignatureIndex]].shipDesignation;
		if (UIFunctions.globaluifunctions.ingamereference.mainPanel.activeSelf)
		{
			UIFunctions.globaluifunctions.ingamereference.RefreshInGameReferenceData(this.otherVesselList[this.currentSignatureIndex]);
		}
		GC.Collect();
	}

	// Token: 0x060008FA RID: 2298 RVA: 0x00066608 File Offset: 0x00064808
	public void SetProfileToClassifiedContact()
	{
		string str = UIFunctions.globaluifunctions.database.databaseshipdata[this.otherVesselList[this.currentSignatureIndex]].shipPrefabName + "_sig";
		if (!UIFunctions.globaluifunctions.playerfunctions.sensormanager.classifiedByPlayer[this.currentTargetIndex])
		{
			this.signatureMaterials[0].SetTexture("__MainTex", UIFunctions.globaluifunctions.textparser.GetTexture(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("vessels/signature/" + str + ".png")));
			this.signatureData[0].text = UIFunctions.globaluifunctions.database.databaseshipdata[this.otherVesselList[this.currentSignatureIndex]].shipclass.ToUpper() + " " + UIFunctions.globaluifunctions.database.databaseshipdata[this.otherVesselList[this.currentSignatureIndex]].shipDesignation;
			UIFunctions.globaluifunctions.ingamereference.currentInGameReferenceIndex = this.otherVesselList[this.currentSignatureIndex];
			if (UIFunctions.globaluifunctions.ingamereference.mainPanel.activeSelf)
			{
				UIFunctions.globaluifunctions.ingamereference.RefreshInGameReferenceData(UIFunctions.globaluifunctions.ingamereference.currentInGameReferenceIndex);
			}
		}
		else
		{
			str = UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.sensormanager.classifiedByPlayerAsClass[this.currentTargetIndex]].shipPrefabName + "_sig";
			this.signatureMaterials[0].SetTexture("__MainTex", UIFunctions.globaluifunctions.textparser.GetTexture(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("vessels/signature/" + str + ".png")));
			this.signatureData[0].text = UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.sensormanager.classifiedByPlayerAsClass[this.currentTargetIndex]].shipclass.ToUpper() + " " + UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.sensormanager.classifiedByPlayerAsClass[this.currentTargetIndex]].shipDesignation;
			UIFunctions.globaluifunctions.ingamereference.currentInGameReferenceIndex = UIFunctions.globaluifunctions.playerfunctions.sensormanager.classifiedByPlayerAsClass[this.currentTargetIndex];
			if (UIFunctions.globaluifunctions.ingamereference.mainPanel.activeSelf)
			{
				UIFunctions.globaluifunctions.ingamereference.RefreshInGameReferenceData(UIFunctions.globaluifunctions.ingamereference.currentInGameReferenceIndex);
			}
		}
		GC.Collect();
	}

	// Token: 0x060008FB RID: 2299 RVA: 0x000668B8 File Offset: 0x00064AB8
	private void DraggingWaypointUpdate()
	{
		if (TacticalMap.tacMapEnabled)
		{
			this.sensormanager.tacticalmap.waypointMarker.gameObject.SetActive(true);
			Vector2 v;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.sensormanager.tacticalmap.tacmapCanvas.transform as RectTransform, Input.mousePosition, this.sensormanager.tacticalmap.tacmapCanvas.worldCamera, out v);
			this.sensormanager.tacticalmap.waypointMarker.transform.position = this.sensormanager.tacticalmap.tacmapCanvas.transform.TransformPoint(v);
			if (this.currentActiveTorpedo != null)
			{
				this.sensormanager.tacticalmap.waypointLine.SetPosition(1, this.currentActiveTorpedo.tacMapTorpedoIcon.transform.position - this.sensormanager.tacticalmap.waypointMarker.transform.position);
			}
			else
			{
				this.sensormanager.tacticalmap.waypointLine.SetPosition(1, this.sensormanager.tacticalmap.playerMapContact.transform.position - this.sensormanager.tacticalmap.waypointMarker.transform.position);
			}
		}
	}

	// Token: 0x060008FC RID: 2300 RVA: 0x00066A14 File Offset: 0x00064C14
	public void EnterRunningSilent()
	{
		PlayerFunctions.runningSilent = true;
		this.runSilentButton.interactable = PlayerFunctions.runningSilent;
		this.PlayerMessage(LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "RunSilentOn"), this.messageLogColors["XO"], "RunSilentOn", false);
		this.usingKnots = false;
		this.damagecontrol.SetPlayerMaxTelegraph(2);
		UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("running silent", PlayerFunctions.runningSilent);
		this.helmmanager.guiButtonImages[2].color = this.helmmanager.buttonColors[1];
	}

	// Token: 0x060008FD RID: 2301 RVA: 0x00066ABC File Offset: 0x00064CBC
	public void LeaveRunningSilent()
	{
		PlayerFunctions.runningSilent = false;
		this.runSilentButton.interactable = PlayerFunctions.runningSilent;
		this.PlayerMessage(LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "RunSilentOff"), this.messageLogColors["XO"], "RunSilentOff", false);
		int playerMaxTelegraph = 6;
		if (!this.damagecontrol.CheckSubsystem("PROPULSION", false))
		{
			playerMaxTelegraph = 3;
		}
		else if (!this.damagecontrol.CheckSubsystem("REACTOR", false))
		{
			playerMaxTelegraph = 0;
		}
		this.damagecontrol.SetPlayerMaxTelegraph(playerMaxTelegraph);
		UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("running silent", PlayerFunctions.runningSilent);
		this.helmmanager.guiButtonImages[2].color = this.helmmanager.buttonColors[0];
	}

	// Token: 0x060008FE RID: 2302 RVA: 0x00066B94 File Offset: 0x00064D94
	public void DisableWaypointDragging()
	{
		this.sensormanager.tacticalmap.waypointReadout.text = string.Empty;
		PlayerFunctions.draggingWaypoint = false;
		this.sensormanager.tacticalmap.waypointMarker.gameObject.SetActive(false);
	}

	// Token: 0x060008FF RID: 2303 RVA: 0x00066BD4 File Offset: 0x00064DD4
	private void DecreaseRange()
	{
		this.weaponRangeButtonTimers.x = this.weaponRangeButtonTimers.x + Time.deltaTime;
	}

	// Token: 0x06000900 RID: 2304 RVA: 0x00066BF0 File Offset: 0x00064DF0
	private void IncreaseRange()
	{
		this.weaponRangeButtonTimers.y = this.weaponRangeButtonTimers.y + Time.deltaTime;
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x00066C0C File Offset: 0x00064E0C
	private void TorpRight()
	{
		this.torpedoHeadingButtonTimers.x = this.torpedoHeadingButtonTimers.x + Time.deltaTime;
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x00066C28 File Offset: 0x00064E28
	private void TorpLeft()
	{
		this.torpedoHeadingButtonTimers.y = this.torpedoHeadingButtonTimers.y + Time.deltaTime;
	}

	// Token: 0x06000903 RID: 2307 RVA: 0x00066C44 File Offset: 0x00064E44
	private void ManualAdjustTorpedo()
	{
	}

	// Token: 0x06000904 RID: 2308 RVA: 0x00066C48 File Offset: 0x00064E48
	public void SetEventCameraMode()
	{
		if (GameDataManager.optionsBoolSettings[18])
		{
			this.eventcamera.eventCameraOn = !this.eventcamera.eventCameraOn;
			UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("event camera", this.eventcamera.eventCameraOn);
		}
		else if (this.statusIcons[15].gameObject.activeSelf)
		{
			UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("event camera", false);
		}
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x00066CCC File Offset: 0x00064ECC
	public void SetPlayerSonarMode()
	{
		if (this.playerVessel.acoustics.usingActiveSonar)
		{
			this.playerVessel.acoustics.usingActiveSonar = false;
			this.PlayerMessage(LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "SwitchToPassive"), this.messageLogColors["Sonar"], "SwitchToPassive", false);
			this.helmmanager.guiButtonImages[12].color = this.helmmanager.buttonColors[0];
		}
		else
		{
			this.playerVessel.acoustics.usingActiveSonar = true;
			this.PlayerMessage(LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "SwitchToActive"), this.messageLogColors["Sonar"], "SwitchToActive", false);
			this.activeSonarPingTimer = this.activeSonarPingRate;
			this.helmmanager.guiButtonImages[12].color = this.helmmanager.buttonColors[1];
		}
		UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("active sonar", this.playerVessel.acoustics.usingActiveSonar);
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x00066DF8 File Offset: 0x00064FF8
	public int GetSettingIndex(string input, string[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			if (input == array[i])
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x00066E2C File Offset: 0x0006502C
	public void SetTubeSettingButtons(int tubeID)
	{
		int playerTorpedoIDInTube = this.GetPlayerTorpedoIDInTube(tubeID);
		string input = string.Empty;
		if (playerTorpedoIDInTube > -1)
		{
			if (this.playerVessel.vesselmovement.weaponSource.torpedoHomingPattern[tubeID] != -1)
			{
				input = this.homeSettingDefinitions[this.playerVessel.vesselmovement.weaponSource.torpedoHomingPattern[tubeID]];
				int settingIndex = this.GetSettingIndex(input, UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTube].homeSettings);
				this.torpedoTubesGUIs[tubeID].homeSettingButton.image.sprite = this.homeSettingSprites[this.playerVessel.vesselmovement.weaponSource.torpedoHomingPattern[tubeID]];
				this.torpedoTubesGUIs[tubeID].homeSettingButton.gameObject.SetActive(true);
			}
			else
			{
				this.torpedoTubesGUIs[tubeID].homeSettingButton.gameObject.SetActive(false);
			}
			if (this.playerVessel.vesselmovement.weaponSource.torpedoSearchPattern[tubeID] != -1)
			{
				input = this.attackSettingDefinitions[this.playerVessel.vesselmovement.weaponSource.torpedoSearchPattern[tubeID]];
				int settingIndex = this.GetSettingIndex(input, UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTube].searchSettings);
				this.torpedoTubesGUIs[tubeID].attackSettingButton.image.sprite = this.attackSettingSprites[this.playerVessel.vesselmovement.weaponSource.torpedoSearchPattern[tubeID]];
				this.torpedoTubesGUIs[tubeID].attackSettingButton.gameObject.SetActive(true);
			}
			else
			{
				this.torpedoTubesGUIs[tubeID].attackSettingButton.gameObject.SetActive(false);
			}
			if (this.playerVessel.vesselmovement.weaponSource.torpedoDepthPattern[tubeID] != -1)
			{
				input = this.depthSettingDefinitions[this.playerVessel.vesselmovement.weaponSource.torpedoDepthPattern[tubeID]];
				int settingIndex = this.GetSettingIndex(input, UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTube].heightSettings);
				this.torpedoTubesGUIs[tubeID].depthSettingButton.image.sprite = this.depthSettingSprites[this.playerVessel.vesselmovement.weaponSource.torpedoDepthPattern[tubeID]];
				this.torpedoTubesGUIs[tubeID].depthSettingButton.gameObject.SetActive(true);
			}
			else
			{
				this.torpedoTubesGUIs[tubeID].depthSettingButton.gameObject.SetActive(false);
			}
		}
		else
		{
			this.ClearTubeSettingButtons(tubeID);
		}
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x000670A8 File Offset: 0x000652A8
	public void ClearTubeSettingButtons(int tubeID)
	{
		this.torpedoTubesGUIs[tubeID].homeSettingButton.gameObject.SetActive(false);
		this.torpedoTubesGUIs[tubeID].attackSettingButton.gameObject.SetActive(false);
		this.torpedoTubesGUIs[tubeID].depthSettingButton.gameObject.SetActive(false);
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x00067100 File Offset: 0x00065300
	public void SetWeaponAttack()
	{
		int num = UIFunctions.globaluifunctions.playerfunctions.GetPlayerTorpedoIDInTube(this.playerVessel.vesselmovement.weaponSource.activeTube);
		if (num < 0)
		{
			if (this.currentActiveTorpedo == null || !this.currentActiveTorpedo.onWire)
			{
				return;
			}
			num = this.currentActiveTorpedo.databaseweapondata.weaponID;
		}
		if (UIFunctions.globaluifunctions.database.databaseweapondata[num].searchSettings[0] == "FALSE")
		{
			return;
		}
		string text = this.attackSettingDefinitions[this.playerVessel.vesselmovement.weaponSource.torpedoSearchPattern[this.playerVessel.vesselmovement.weaponSource.activeTube]];
		int num2 = this.GetSettingIndex(text, UIFunctions.globaluifunctions.database.databaseweapondata[num].searchSettings);
		num2++;
		if (num2 >= UIFunctions.globaluifunctions.database.databaseweapondata[num].searchSettings.Length)
		{
			num2 = 0;
		}
		text = UIFunctions.globaluifunctions.database.databaseweapondata[num].searchSettings[num2];
		int settingIndex = this.GetSettingIndex(text, this.attackSettingDefinitions);
		this.playerVessel.vesselmovement.weaponSource.torpedoSearchPattern[this.playerVessel.vesselmovement.weaponSource.activeTube] = settingIndex;
		this.torpedoTubesGUIs[this.playerVessel.vesselmovement.weaponSource.activeTube].attackSettingButton.image.sprite = this.attackSettingSprites[settingIndex];
		if (this.playerVessel.vesselmovement.weaponSource.weaponInTube[this.playerVessel.vesselmovement.weaponSource.activeTube] == -100)
		{
			string text2 = text;
			switch (text2)
			{
			case "STRAIGHT":
				this.currentActiveTorpedo.searchLeft = false;
				this.currentActiveTorpedo.runStraight = true;
				this.currentActiveTorpedo.snakeSearch = false;
				break;
			case "SNAKE":
				this.currentActiveTorpedo.searchLeft = false;
				this.currentActiveTorpedo.runStraight = false;
				this.currentActiveTorpedo.snakeSearch = true;
				break;
			case "LEFT":
				this.currentActiveTorpedo.searchLeft = true;
				this.currentActiveTorpedo.runStraight = false;
				this.currentActiveTorpedo.snakeSearch = false;
				break;
			case "RIGHT":
				this.currentActiveTorpedo.searchLeft = false;
				this.currentActiveTorpedo.runStraight = false;
				this.currentActiveTorpedo.snakeSearch = false;
				break;
			}
			this.currentActiveTorpedo.playerControlling = false;
		}
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x000673F8 File Offset: 0x000655F8
	public void SetWeaponDepth()
	{
		int num = UIFunctions.globaluifunctions.playerfunctions.GetPlayerTorpedoIDInTube(this.playerVessel.vesselmovement.weaponSource.activeTube);
		if (num < 0)
		{
			if (this.currentActiveTorpedo == null || !this.currentActiveTorpedo.onWire)
			{
				return;
			}
			num = this.currentActiveTorpedo.databaseweapondata.weaponID;
		}
		if (UIFunctions.globaluifunctions.database.databaseweapondata[num].heightSettings[0] == "FALSE")
		{
			return;
		}
		string text = this.depthSettingDefinitions[this.playerVessel.vesselmovement.weaponSource.torpedoDepthPattern[this.playerVessel.vesselmovement.weaponSource.activeTube]];
		int num2 = this.GetSettingIndex(text, UIFunctions.globaluifunctions.database.databaseweapondata[num].heightSettings);
		num2++;
		if (num2 >= UIFunctions.globaluifunctions.database.databaseweapondata[num].heightSettings.Length)
		{
			num2 = 0;
		}
		text = UIFunctions.globaluifunctions.database.databaseweapondata[num].heightSettings[num2];
		int settingIndex = this.GetSettingIndex(text, this.depthSettingDefinitions);
		this.playerVessel.vesselmovement.weaponSource.torpedoDepthPattern[this.playerVessel.vesselmovement.weaponSource.activeTube] = settingIndex;
		this.torpedoTubesGUIs[this.playerVessel.vesselmovement.weaponSource.activeTube].depthSettingButton.image.sprite = this.depthSettingSprites[settingIndex];
		if (this.playerVessel.vesselmovement.weaponSource.weaponInTube[this.playerVessel.vesselmovement.weaponSource.activeTube] == -100)
		{
			string text2 = text;
			switch (text2)
			{
			case "SHALLOW":
				this.currentActiveTorpedo.searchYValue = this.playerVessel.vesselmovement.weaponSource.GetShallowRunDepth();
				break;
			case "LEVEL":
				this.currentActiveTorpedo.searchYValue = this.currentActiveTorpedo.transform.position.y;
				break;
			case "DEEP":
				this.currentActiveTorpedo.searchYValue = this.playerVessel.vesselmovement.weaponSource.GetDeepRunDepth();
				break;
			}
			if (this.currentActiveTorpedo.sensorsActive)
			{
				this.currentActiveTorpedo.cruiseYValue = this.currentActiveTorpedo.searchYValue;
			}
			this.currentActiveTorpedo.playerControlling = false;
		}
	}

	// Token: 0x0600090B RID: 2315 RVA: 0x000676D4 File Offset: 0x000658D4
	public void SetWeaponHoming()
	{
		int num = UIFunctions.globaluifunctions.playerfunctions.GetPlayerTorpedoIDInTube(this.playerVessel.vesselmovement.weaponSource.activeTube);
		if (num < 0)
		{
			if (this.currentActiveTorpedo == null || !this.currentActiveTorpedo.onWire)
			{
				return;
			}
			num = this.currentActiveTorpedo.databaseweapondata.weaponID;
		}
		if (UIFunctions.globaluifunctions.database.databaseweapondata[num].homeSettings[0] == "FALSE")
		{
			return;
		}
		string text = this.homeSettingDefinitions[this.playerVessel.vesselmovement.weaponSource.torpedoHomingPattern[this.playerVessel.vesselmovement.weaponSource.activeTube]];
		int num2 = this.GetSettingIndex(text, UIFunctions.globaluifunctions.database.databaseweapondata[num].homeSettings);
		num2++;
		if (num2 >= UIFunctions.globaluifunctions.database.databaseweapondata[num].homeSettings.Length)
		{
			num2 = 0;
		}
		text = UIFunctions.globaluifunctions.database.databaseweapondata[num].homeSettings[num2];
		int settingIndex = this.GetSettingIndex(text, this.homeSettingDefinitions);
		this.playerVessel.vesselmovement.weaponSource.torpedoHomingPattern[this.playerVessel.vesselmovement.weaponSource.activeTube] = settingIndex;
		this.torpedoTubesGUIs[this.playerVessel.vesselmovement.weaponSource.activeTube].homeSettingButton.image.sprite = this.homeSettingSprites[settingIndex];
		if (this.playerVessel.vesselmovement.weaponSource.weaponInTube[this.playerVessel.vesselmovement.weaponSource.activeTube] == -100)
		{
			string text2 = text;
			if (text2 != null)
			{
				if (PlayerFunctions.<>f__switch$map1E == null)
				{
					PlayerFunctions.<>f__switch$map1E = new Dictionary<string, int>(2)
					{
						{
							"ACTIVE",
							0
						},
						{
							"PASSIVE",
							1
						}
					};
				}
				int num3;
				if (PlayerFunctions.<>f__switch$map1E.TryGetValue(text2, out num3))
				{
					if (num3 != 0)
					{
						if (num3 == 1)
						{
							this.currentActiveTorpedo.passiveHoming = true;
						}
					}
					else
					{
						this.currentActiveTorpedo.passiveHoming = false;
					}
				}
			}
			this.currentActiveTorpedo.playerControlling = false;
		}
	}

	// Token: 0x0600090C RID: 2316 RVA: 0x00067928 File Offset: 0x00065B28
	public void ClickOnTube(int i)
	{
		if (PlayerFunctions.draggingWaypoint)
		{
			return;
		}
		if (i == this.playerVessel.vesselmovement.weaponSource.activeTube)
		{
			return;
		}
		this.ClearCurrentActiveTorpedo();
		this.playerVessel.vesselmovement.weaponSource.activeTube = i;
		this.HighlightActiveTube();
		if (this.playerVessel.vesselmovement.weaponSource.weaponInTube[this.playerVessel.vesselmovement.weaponSource.activeTube] == -100)
		{
			this.currentActiveTorpedo = this.playerVessel.vesselmovement.weaponSource.torpedoesOnWire[this.playerVessel.vesselmovement.weaponSource.activeTube];
		}
		else
		{
			this.currentActiveTorpedo = null;
			this.SetWireDataText();
		}
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x000679F4 File Offset: 0x00065BF4
	public void InitialiseWeapons()
	{
		this.mastThresholdDepth = this.playerVessel.databaseshipdata.periscopeDepthInFeet + 5;
		this.fullMessageLog = new List<string>();
		this.fullMessageLogColors = new List<Color32>();
		this.numberOfLogEntries = 0;
		if (this.currentFullLogParentObject != null)
		{
			UnityEngine.Object.Destroy(this.currentFullLogParentObject);
		}
		this.currentFullLogParentObject = UnityEngine.Object.Instantiate<GameObject>(this.fullLogParentObject);
		this.currentFullLogParentObject.transform.SetParent(this.fullLogParentObject.transform, false);
		this.currentFullLogParentObject.GetComponent<Image>().enabled = true;
		this.fullLogScrollRect.content = this.currentFullLogParentObject.GetComponent<RectTransform>();
		this.fullLogObject.SetActive(false);
		this.fullLogToggleButton.SetActive(this.generateFullLog);
		this.playerSunkBy = string.Empty;
		this.hudHidden = false;
		this.eventcamera.eventCameraOn = false;
		this.SetEventCameraMode();
		this.ballastRechargeTimer = 0f;
		this.ballastRechargeTime = 120f;
		this.landAttackNumber = 0;
		this.ClearStatusIcons();
		this.firstDepthCheckDone = false;
		PlayerFunctions.draggingWaypoint = false;
		for (int i = 0; i < this.torpedoTubesGUIs.Length; i++)
		{
			UnityEngine.Object.Destroy(this.torpedoTubesGUIs[i].gameObject);
		}
		this.weaponSprites = new Sprite[this.playerVessel.databaseshipdata.torpedoIDs.Length];
		for (int j = 0; j < this.playerVessel.databaseshipdata.torpedoIDs.Length; j++)
		{
			this.weaponSprites[j] = UIFunctions.globaluifunctions.database.databaseweapondata[this.playerVessel.databaseshipdata.torpedoIDs[j]].weaponImage;
		}
		Vector2 vector = new Vector2(-260f, 36f);
		int num = Mathf.FloorToInt((float)(this.playerVessel.databaseshipdata.torpedotubes / 2));
		int num2 = 1;
		float num3 = 0f;
		if (this.playerVessel.databaseshipdata.vlsTorpedoIDs != null)
		{
			num2 = 0;
			num3 = 36f;
		}
		float x = vector.x;
		float num4 = vector.y * ((float)num - (float)num2);
		this.torpedoTubesGUIs = new TorpedoTubeGUI[this.playerVessel.databaseshipdata.torpedotubes];
		this.torpedoTubeImages = new Image[this.playerVessel.databaseshipdata.torpedotubes];
		for (int k = 0; k < this.playerVessel.databaseshipdata.torpedotubes; k++)
		{
			GameObject torpTube = UnityEngine.Object.Instantiate(this.torpedoTubeGUIObject, this.hudTransfrom.position, Quaternion.identity) as GameObject;
			torpTube.SetActive(true);
			torpTube.transform.SetParent(this.menuPanel.transform, true);
			RectTransform component = torpTube.GetComponent<RectTransform>();
			component.localScale = Vector3.one;
			torpTube.transform.localPosition = new Vector2(x, num4);
			torpTube.name = k.ToString();
			num4 -= vector.y;
			if (k == num - 1)
			{
				x = 0f;
				num4 = vector.y * ((float)num - (float)num2);
			}
			torpTube.transform.SetParent(this.menuPanel.transform, true);
			this.torpedoTubesGUIs[k] = torpTube.GetComponent<TorpedoTubeGUI>();
			this.torpedoTubeImages[k] = this.torpedoTubesGUIs[k].weaponInTube;
			this.torpedoTubesGUIs[k].maskSprite.gameObject.GetComponent<Button>().onClick.AddListener(delegate()
			{
				this.ClickOnTube(int.Parse(torpTube.name));
			});
			ColorBlock colors = this.torpedoTubesGUIs[k].attackSettingButton.colors;
			colors.normalColor = this.helmmanager.buttonColors[1];
			colors.highlightedColor = this.helmmanager.buttonColors[1];
			colors.pressedColor = this.helmmanager.buttonColors[1];
			colors.disabledColor = this.helmmanager.buttonColors[0];
			this.torpedoTubesGUIs[k].attackSettingButton.colors = colors;
			this.torpedoTubesGUIs[k].homeSettingButton.colors = colors;
			this.torpedoTubesGUIs[k].depthSettingButton.colors = colors;
		}
		if (!GameDataManager.trainingMode && !GameDataManager.missionMode)
		{
			UIFunctions.globaluifunctions.campaignmanager.GetPlayerCampaignData();
		}
		for (int l = 0; l < this.playerVessel.databaseshipdata.torpedotubes; l++)
		{
			if (!GameDataManager.trainingMode && !GameDataManager.missionMode && UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.playerTubeStatus[l] == -200)
			{
				this.torpedoTubeImages[l].sprite = UIFunctions.globaluifunctions.playerfunctions.tubeDestroyedSprite;
				this.ClearTubeSettingButtons(l);
				this.playerVessel.vesselmovement.weaponSource.tubeStatus[l] = -200;
				this.playerVessel.vesselmovement.weaponSource.weaponInTube[l] = -200;
			}
			else
			{
				int playerTorpedoIDInTubeOnInit = this.GetPlayerTorpedoIDInTubeOnInit(l);
				bool flag = false;
				foreach (int num5 in this.playerVessel.databaseshipdata.torpedoIDs)
				{
					if (playerTorpedoIDInTubeOnInit == num5)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					this.playerVessel.vesselmovement.weaponSource.tubeStatus[l] = -10;
					this.playerVessel.vesselmovement.weaponSource.weaponInTube[l] = -10;
					this.torpedoTubeImages[l].gameObject.SetActive(false);
					this.ClearTubeSettingButtons(l);
				}
				else
				{
					this.playerVessel.vesselmovement.weaponSource.torpedoSearchPattern[l] = this.GetSettingIndex(UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTubeOnInit].searchSettings[0], this.attackSettingDefinitions);
					this.playerVessel.vesselmovement.weaponSource.torpedoDepthPattern[l] = this.GetSettingIndex(UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTubeOnInit].heightSettings[0], this.depthSettingDefinitions);
					this.playerVessel.vesselmovement.weaponSource.torpedoHomingPattern[l] = this.GetSettingIndex(UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTubeOnInit].homeSettings[0], this.homeSettingDefinitions);
					this.SetTubeSettingButtons(l);
				}
			}
		}
		this.HighlightActiveTube();
		Vector2 v = new Vector2(0f, vector.y * (float)num + vector.y - 36f + num3);
		this.signaturePanel.transform.localPosition = v;
		this.conditionsPanel.transform.localPosition = v;
		this.damagePanel.transform.localPosition = v;
		this.storesPanel.transform.localPosition = v;
		this.messageLogPanel.transform.localPosition = new Vector2(0f, 36f * (float)num + 28f + num3);
		this.messageLogPositions = new Vector2(36f * (float)num + 28f + num3, 36f * (float)num + 275f + num3);
		if (this.currentOpenPanel != -1)
		{
			this.OpenContextualPanel(this.currentOpenPanel);
		}
		this.currentSignatureIndex = 0;
		this.sensormanager.SetSonarSignatureLabelData(this.playerVessel.databaseshipdata.shipID, 2);
		this.DisableESMMeter();
		this.storesPanel.SetActive(false);
		this.wireData[0].text = string.Empty;
		this.wireData[1].text = string.Empty;
	}

	// Token: 0x0600090E RID: 2318 RVA: 0x00068244 File Offset: 0x00066444
	private int GetPlayerTorpedoIDInTubeOnInit(int tube)
	{
		if (tube == this.playerVessel.vesselmovement.weaponSource.torpedoTubes.Length - 1 && this.playerVessel.vesselmovement.weaponSource.hasVLS)
		{
			if (this.playerVessel.vesselmovement.weaponSource.weaponInTube[tube] > -1)
			{
				return this.playerVessel.vesselmovement.weaponSource.vlsTorpedoTypes[this.playerVessel.vesselmovement.weaponSource.tubeStatus[tube]];
			}
			return -1;
		}
		else
		{
			if (this.playerVessel.vesselmovement.weaponSource.weaponInTube[tube] > -1)
			{
				return this.playerVessel.vesselmovement.weaponSource.torpedoTypes[this.playerVessel.vesselmovement.weaponSource.tubeStatus[tube]];
			}
			return -1;
		}
	}

	// Token: 0x0600090F RID: 2319 RVA: 0x00068324 File Offset: 0x00066524
	public int GetPlayerTorpedoIDInTube(int tube)
	{
		if (!this.playerVessel.vesselmovement.weaponSource.hasVLS)
		{
			if (this.playerVessel.vesselmovement.weaponSource.weaponInTube[tube] != -100 && this.playerVessel.vesselmovement.weaponSource.tubeStatus[tube] > -1)
			{
				return this.playerVessel.vesselmovement.weaponSource.torpedoTypes[this.playerVessel.vesselmovement.weaponSource.tubeStatus[tube]];
			}
			if (this.currentActiveTorpedo != null)
			{
				return this.currentActiveTorpedo.databaseweapondata.weaponID;
			}
		}
		if (tube == this.playerVessel.vesselmovement.weaponSource.torpedoTubes.Length - 1 && this.playerVessel.vesselmovement.weaponSource.tubeStatus[tube] > -1)
		{
			return this.playerVessel.vesselmovement.weaponSource.vlsTorpedoTypes[this.playerVessel.vesselmovement.weaponSource.tubeStatus[tube]];
		}
		if (this.playerVessel.vesselmovement.weaponSource.weaponInTube[tube] != -100 && this.playerVessel.vesselmovement.weaponSource.tubeStatus[tube] > -1)
		{
			return this.playerVessel.vesselmovement.weaponSource.torpedoTypes[this.playerVessel.vesselmovement.weaponSource.tubeStatus[tube]];
		}
		if (this.currentActiveTorpedo != null)
		{
			return this.currentActiveTorpedo.databaseweapondata.weaponID;
		}
		return -1;
	}

	// Token: 0x06000910 RID: 2320 RVA: 0x000684CC File Offset: 0x000666CC
	private void HighlightActiveTube()
	{
		for (int i = 0; i < this.torpedoTubesGUIs.Length; i++)
		{
			int num;
			bool interactable;
			if (i == this.playerVessel.vesselmovement.weaponSource.activeTube)
			{
				num = 1;
				interactable = true;
			}
			else
			{
				num = 0;
				interactable = false;
			}
			this.torpedoTubesGUIs[i].tubeBackground.sprite = this.torpedoTubeBackgrounds[num];
			this.torpedoTubesGUIs[i].attackSettingButton.interactable = interactable;
			this.torpedoTubesGUIs[i].homeSettingButton.interactable = interactable;
			this.torpedoTubesGUIs[i].depthSettingButton.interactable = interactable;
		}
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x00068574 File Offset: 0x00066774
	public void PlayMessageLogAudio(string clipPath)
	{
		UIFunctions.globaluifunctions.playerfunctions.messageLogSounds.Stop();
		UIFunctions.globaluifunctions.playerfunctions.messageLogSounds.clip = UIFunctions.globaluifunctions.textparser.GetAudioClip(clipPath);
		UIFunctions.globaluifunctions.playerfunctions.messageLogSounds.Play();
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x000685D0 File Offset: 0x000667D0
	public void PlayerMessage(string message, Color lineColor, string lookupValue, bool checkDuplicate = false)
	{
		if (this.sensormanager.initialRun || !this.sensormanager.addTMA)
		{
			return;
		}
		if (checkDuplicate && this.messageLog[0].text == message)
		{
			return;
		}
		if (LanguageManager.messageLogAudioClipDictionary.ContainsKey(lookupValue))
		{
			this.PlayMessageLogAudio(LanguageManager.messageLogAudioClipDictionary[lookupValue]);
		}
		if (LanguageManager.messageLogVoiceDictionary.ContainsKey(lookupValue))
		{
			this.voicemanager.PlayMessageLogVoice(lookupValue);
		}
		if (message.Contains(LanguageManager.messageLogVoiceDictionary["SpeedPrefixDuplicateCheck"]) && this.messageLog[0].text.Contains(LanguageManager.messageLogVoiceDictionary["SpeedPrefixDuplicateCheck"]))
		{
			this.messageLog[0].text = message;
			this.messageLogAlphas[0] = 1f;
			if (this.currentLogLineObject != null)
			{
				this.currentLogLineObject.GetComponent<Text>().text = message;
			}
			return;
		}
		this.messageLog[4].text = this.messageLog[3].text;
		this.messageLog[3].text = this.messageLog[2].text;
		this.messageLog[2].text = this.messageLog[1].text;
		this.messageLog[1].text = this.messageLog[0].text;
		this.messageLog[0].text = message;
		this.messageLog[4].color = this.messageLog[3].color;
		this.messageLog[3].color = this.messageLog[2].color;
		this.messageLog[2].color = this.messageLog[1].color;
		this.messageLog[1].color = this.messageLog[0].color;
		this.messageLog[0].color = lineColor;
		this.messageLogAlphas[4] = this.messageLogAlphas[3];
		this.messageLogAlphas[3] = this.messageLogAlphas[2];
		this.messageLogAlphas[2] = this.messageLogAlphas[1];
		this.messageLogAlphas[1] = this.messageLogAlphas[0];
		this.messageLogAlphas[0] = 1f;
		this.CreateLogEntry(message, lineColor);
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x00068820 File Offset: 0x00066A20
	private void CreateLogEntry(string message, Color32 lineColor)
	{
		if (!this.generateFullLog)
		{
			return;
		}
		this.numberOfLogEntries++;
		this.currentFullLogParentObject.GetComponent<RectTransform>().sizeDelta = new Vector2(512f, (float)(this.numberOfLogEntries * 20));
		this.currentLogLineObject = (UnityEngine.Object.Instantiate(this.logTextPrefab, this.currentFullLogParentObject.transform.position, Quaternion.identity) as GameObject);
		this.currentLogLineObject.transform.SetParent(this.currentFullLogParentObject.transform, false);
		this.currentLogLineObject.transform.localPosition = new Vector3(0f, 40f, 0f);
		this.currentLogLineObject.transform.Translate(Vector3.up * -20f * this.hudCanvas.lossyScale.x, Space.Self);
		Text component = this.currentLogLineObject.GetComponent<Text>();
		component.color = lineColor;
		component.text = message;
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x00068930 File Offset: 0x00066B30
	public void ToggleFullLog()
	{
		this.fullLogObject.SetActive(!this.fullLogObject.activeSelf);
	}

	// Token: 0x06000915 RID: 2325 RVA: 0x0006894C File Offset: 0x00066B4C
	public string GetFullContactName(string contact, int vesselIndex)
	{
		string text = this.sensormanager.tacticalmap.mapContact[vesselIndex].contactText.text;
		if (this.sensormanager.classifiedByPlayer[vesselIndex])
		{
			return text;
		}
		return this.GetBasicContactName(text);
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x00068994 File Offset: 0x00066B94
	public string GetBasicContactName(string message)
	{
		message = message.Replace("S", LanguageManager.interfaceDictionary["SonarContact"] + " ");
		message = message.Replace("R", LanguageManager.interfaceDictionary["RadarContact"] + " ");
		message = message.Replace("V", LanguageManager.interfaceDictionary["VisualContact"] + " ");
		message = message.Replace("M", LanguageManager.interfaceDictionary["MultipleContact"] + " ");
		message = message.Replace("E", LanguageManager.interfaceDictionary["ESMContact"] + " ");
		return message;
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x00068A60 File Offset: 0x00066C60
	public void MapContactButton(int targetID, bool isPlayer)
	{
		if (Input.anyKeyDown)
		{
			return;
		}
		if (!UIFunctions.globaluifunctions.playerfunctions.sensormanager.detectedByPlayer[targetID])
		{
			return;
		}
		if (isPlayer)
		{
			UIFunctions.globaluifunctions.skyobjectcenterer.ForceSkyboxPosition(GameDataManager.playervesselsonlevel[0].transform);
			ManualCameraZoom.minDistance = GameDataManager.playervesselsonlevel[0].databaseshipdata.minCameraDistance;
			return;
		}
		if (targetID == this.currentTargetIndex)
		{
			UIFunctions.globaluifunctions.skyobjectcenterer.ForceSkyboxPosition(GameDataManager.enemyvesselsonlevel[targetID].transform);
			ManualCameraZoom.minDistance = GameDataManager.enemyvesselsonlevel[targetID].databaseshipdata.minCameraDistance;
		}
		if (GameDataManager.enemyvesselsonlevel[targetID].isSinking)
		{
			UIFunctions.globaluifunctions.skyobjectcenterer.ForceSkyboxPosition(GameDataManager.enemyvesselsonlevel[targetID].transform);
			ManualCameraZoom.minDistance = GameDataManager.enemyvesselsonlevel[targetID].databaseshipdata.minCameraDistance;
			return;
		}
		this.currentTargetIndex = targetID;
		this.SetTargetData();
		this.SetProfileToClassifiedContact();
		UIFunctions.globaluifunctions.playerfunctions.SetContactProfileSprite(GameDataManager.enemyvesselsonlevel[targetID].databaseshipdata.shipPrefabName + "_sig");
		if (this.contextualPanels[1].activeSelf)
		{
			this.sensormanager.SetSignatureData();
		}
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x00068BA8 File Offset: 0x00066DA8
	public void MapTorpedoButton(Transform torpedoPosition, int whichnavy)
	{
		if (whichnavy == 1)
		{
			UIFunctions.globaluifunctions.CloseCombatScreens();
			UIFunctions.globaluifunctions.skyobjectcenterer.ForceSkyboxPosition(torpedoPosition);
			return;
		}
		if (whichnavy == 0)
		{
			Torpedo component = torpedoPosition.gameObject.GetComponent<Torpedo>();
			if (this.currentActiveTorpedo != null && this.currentActiveTorpedo.transform == torpedoPosition)
			{
				UIFunctions.globaluifunctions.CloseCombatScreens();
				UIFunctions.globaluifunctions.skyobjectcenterer.ForceSkyboxPosition(torpedoPosition);
				return;
			}
			if (component.onWire)
			{
				this.currentActiveTorpedo = component;
				this.playerVessel.vesselmovement.weaponSource.activeTube = this.currentActiveTorpedo.tubefiredFrom;
				this.HighlightActiveTube();
				this.SetWireDataText();
			}
			else
			{
				UIFunctions.globaluifunctions.CloseCombatScreens();
				UIFunctions.globaluifunctions.skyobjectcenterer.ForceSkyboxPosition(torpedoPosition);
			}
		}
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x00068C88 File Offset: 0x00066E88
	private void FireTube()
	{
		bool flag = false;
		if (this.playerVessel.vesselmovement.weaponSource.hasVLS && this.playerVessel.vesselmovement.weaponSource.activeTube == this.playerVessel.vesselmovement.weaponSource.torpedoTubes.Length - 1)
		{
			flag = true;
		}
		float num = UIFunctions.globaluifunctions.database.databaseweapondata[this.GetPlayerTorpedoIDInTube(this.playerVessel.vesselmovement.weaponSource.activeTube)].maxLaunchDepth;
		if (flag)
		{
			num = this.playerVessel.databaseshipdata.vlsMaxDepthToFire;
		}
		if ((float)this.playerDepthInFeet > num)
		{
			string lookupValue = "TooDeep";
			string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "TooDeep");
			if (flag)
			{
				text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "VLSTooDeep");
				lookupValue = "VLSTooDeep";
			}
			text = text.Replace("<WEAPON>", UIFunctions.globaluifunctions.database.databaseweapondata[this.GetPlayerTorpedoIDInTube(this.playerVessel.vesselmovement.weaponSource.activeTube)].weaponName);
			text = text.Replace("<DEPTH>", num.ToString());
			text = text.Replace("<FEET>", LanguageManager.interfaceDictionary["ReferenceFeet"]);
			text = text.Replace("<VLS>", LanguageManager.interfaceDictionary["VerticalLaunchSystemAbb"]);
			this.PlayerMessage(text, this.messageLogColors["TorpedoRoom"], lookupValue, false);
			return;
		}
		if (flag && this.playerVessel.vesselmovement.shipSpeed.z > this.playerVessel.databaseshipdata.vlsMaxSpeedToFire)
		{
			string text2 = LanguageManager.messageLogDictionary["VLSTooFast"];
			text2 = text2.Replace("<SPEED>", (this.playerVessel.databaseshipdata.vlsMaxSpeedToFire * 10f).ToString());
			text2 = text2.Replace("<KNOT>", LanguageManager.interfaceDictionary["ReferenceKnot"]);
			text2 = text2.Replace("<VLS>", LanguageManager.interfaceDictionary["VerticalLaunchSystemAbb"]);
			UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text2, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["TorpedoRoom"], "VLSTooFast", false);
			return;
		}
		this.SetWeaponWaypoint();
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x00068EEC File Offset: 0x000670EC
	public void SetWeaponWaypoint()
	{
		if (this.helmmanager.draggingNavWaypoint || PlayerFunctions.draggingWaypoint)
		{
			this.sensormanager.tacticalmap.waypointReadout.text = string.Empty;
			this.helmmanager.DisableWaypointDragging();
			this.DisableWaypointDragging();
			return;
		}
		PlayerFunctions.draggingWaypoint = true;
		if (TacticalMap.tacMapEnabled)
		{
			if (this.wireData[0].gameObject.activeSelf && this.currentActiveTorpedo != null && this.currentActiveTorpedo.tubefiredFrom == this.playerVessel.vesselmovement.weaponSource.activeTube)
			{
				this.DisplayWireWaypoint();
			}
		}
		else
		{
			this.bearingLine.transform.position = this.playerVessel.transform.position;
			this.bearingLine.transform.rotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(0f, UIFunctions.globaluifunctions.MainCamera.transform.eulerAngles.y, 0f), 1f);
			if (this.currentTargetIndex < 0 || GameDataManager.enemyvesselsonlevel[this.currentTargetIndex] == null)
			{
				float num = 4000f;
				if (UIFunctions.globaluifunctions.database.databaseweapondata[this.playerVessel.vesselmovement.weaponSource.weaponInTube[this.playerVessel.vesselmovement.weaponSource.activeTube]].isMissile)
				{
					num = 6000f;
				}
				this.bearingLineRangeMarker.transform.localPosition = new Vector3(0f, 0f, num * GameDataManager.inverseYardsScale);
			}
			else
			{
				float num2 = this.sensormanager.rangeToContacts[this.currentTargetIndex] * this.sensormanager.solutionRangeErrors[this.currentTargetIndex];
				if (num2 < 1000f)
				{
					num2 = 100f;
				}
				num2 *= GameDataManager.inverseYardsScale;
				this.bearingLineRangeMarker.transform.localPosition = new Vector3(0f, 0f, num2);
			}
			this.sensormanager.tacticalmap.waypointWorldMarker.transform.SetParent(this.bearingLineRangeMarker.transform, false);
			this.sensormanager.tacticalmap.waypointWorldMarker.transform.localPosition = Vector3.zero;
			string[] waypointDetails = this.sensormanager.tacticalmap.GetWaypointDetails();
			string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "WaypointSet");
			text = text.Replace("<BRG>", waypointDetails[0]);
			text = text.Replace("<RANGE>", waypointDetails[1]);
			text = text.Replace("<KILOYARD>", LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "KiloYard"));
			this.PlayerMessage(text, this.messageLogColors["FireControl"], "WaypointSet", true);
			this.playerVessel.vesselmovement.weaponSource.FireTube();
			this.sensormanager.tacticalmap.waypointWorldMarker.transform.parent = null;
			if (this.wireData[0].gameObject.activeSelf && this.currentActiveTorpedo != null)
			{
				this.playerVessel.vesselmovement.weaponSource.SetWeaponWaypointData(this.currentActiveTorpedo);
				this.currentActiveTorpedo.playerTimeToWaypoint = -100f;
				this.currentActiveTorpedo.playerControlling = false;
			}
			this.DisableWaypointDragging();
			this.bearingLine.transform.SetParent(null);
		}
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x00069274 File Offset: 0x00067474
	private void ReloadVLSTube()
	{
		int activeTube = this.playerVessel.vesselmovement.weaponSource.activeTube;
		int num = 0;
		for (int i = 0; i < this.playerVessel.vesselmovement.weaponSource.vlsCurrentTorpsOnBoard.Length; i++)
		{
			num += this.playerVessel.vesselmovement.weaponSource.vlsCurrentTorpsOnBoard[i];
		}
		if (num <= 0)
		{
			this.PlayerMessage(LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "NoVLSReloadsAvailable"), this.messageLogColors["TorpedoRoom"], "NoVLSReloadsAvailable", false);
			return;
		}
		int num2 = this.playerVessel.vesselmovement.weaponSource.wantedWeaponInTube[activeTube];
		if (this.playerVessel.vesselmovement.weaponSource.tubeStatus[activeTube] >= 0)
		{
			num2++;
		}
		if (num2 >= this.playerVessel.vesselmovement.weaponSource.vlsTorpedoTypes.Length)
		{
			num2 = 0;
		}
		for (int j = 0; j < this.playerVessel.vesselmovement.weaponSource.vlsCurrentTorpsOnBoard.Length; j++)
		{
			if (this.playerVessel.vesselmovement.weaponSource.vlsCurrentTorpsOnBoard[num2] > 0)
			{
				break;
			}
			num2++;
			if (num2 >= this.playerVessel.vesselmovement.weaponSource.vlsTorpedoTypes.Length)
			{
				num2 = 0;
			}
		}
		this.playerVessel.vesselmovement.weaponSource.wantedWeaponInTube[activeTube] = num2;
		this.playerVessel.vesselmovement.weaponSource.weaponInTube[activeTube] = num2;
		this.playerVessel.vesselmovement.weaponSource.tubeStatus[activeTube] = num2;
		int playerTorpedoIDInTube = this.GetPlayerTorpedoIDInTube(activeTube);
		this.torpedoTubeImages[activeTube].transform.localPosition = new Vector3(0f, -0.5f, 0f);
		this.torpedoTubeImages[activeTube].sprite = UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTube].weaponImage;
		this.torpedoTubeImages[activeTube].gameObject.SetActive(true);
		this.playerVessel.vesselmovement.weaponSource.torpedoSearchPattern[activeTube] = this.GetSettingIndex(UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTube].searchSettings[0], this.attackSettingDefinitions);
		this.playerVessel.vesselmovement.weaponSource.torpedoDepthPattern[activeTube] = this.GetSettingIndex(UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTube].heightSettings[0], this.depthSettingDefinitions);
		this.playerVessel.vesselmovement.weaponSource.torpedoHomingPattern[activeTube] = this.GetSettingIndex(UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTube].homeSettings[0], this.homeSettingDefinitions);
		this.SetTubeSettingButtons(activeTube);
	}

	// Token: 0x0600091C RID: 2332 RVA: 0x00069548 File Offset: 0x00067748
	public void ReloadTube()
	{
		if (PlayerFunctions.draggingWaypoint)
		{
			return;
		}
		int activeTube = this.playerVessel.vesselmovement.weaponSource.activeTube;
		if (this.playerVessel.vesselmovement.weaponSource.weaponInTube[activeTube] == -100)
		{
			if (GameDataManager.trainingMode)
			{
				return;
			}
			this.CutWire(activeTube);
		}
		if (PlayerFunctions.runningSilent)
		{
			this.LeaveRunningSilent();
		}
		if (this.playerVessel.vesselmovement.weaponSource.hasVLS && this.playerVessel.vesselmovement.weaponSource.activeTube == this.playerVessel.vesselmovement.weaponSource.torpedoTubes.Length - 1)
		{
			this.ReloadVLSTube();
			return;
		}
		if (this.playerVessel.vesselmovement.weaponSource.weaponInTube[activeTube] == -200)
		{
			string text = LanguageManager.messageLogDictionary["TubeDestroyed"];
			text = text.Replace("<TUBE>", (activeTube + 1).ToString());
			UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["TorpedoRoom"], "TubeDestroyed", false);
			return;
		}
		int num = 0;
		if (this.playerVessel.vesselmovement.weaponSource.hasVLS)
		{
			num = 1;
		}
		int[] array = new int[this.playerVessel.vesselmovement.weaponSource.torpedoNames.Length];
		for (int i = 0; i < this.playerVessel.vesselmovement.weaponSource.tubeStatus.Length - num; i++)
		{
			if (this.playerVessel.vesselmovement.weaponSource.tubeStatus[i] >= 0)
			{
				array[this.playerVessel.vesselmovement.weaponSource.tubeStatus[i]]++;
			}
		}
		int num2 = 0;
		for (int j = 0; j < this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard.Length; j++)
		{
			num2 += this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[j] - array[j];
		}
		if (num2 <= 0)
		{
			this.PlayerMessage(LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "NoReloadsAvailable"), this.messageLogColors["TorpedoRoom"], "NoReloadsAvailable", true);
			return;
		}
		if (!this.playerVessel.vesselmovement.weaponSource.originalLoadedWeaponInTubeSet[activeTube])
		{
			this.playerVessel.vesselmovement.weaponSource.originalLoadedWeaponInTubeSet[activeTube] = true;
			if (this.playerVessel.vesselmovement.weaponSource.tubeStatus[activeTube] < 0)
			{
				this.playerVessel.vesselmovement.weaponSource.originalLoadedWeaponInTube[activeTube] = -2;
			}
			else
			{
				this.playerVessel.vesselmovement.weaponSource.originalLoadedWeaponInTube[activeTube] = this.playerVessel.vesselmovement.weaponSource.tubeStatus[activeTube];
				this.playerVessel.vesselmovement.weaponSource.originalLoadedWeaponTimer[activeTube] = this.playerVessel.vesselmovement.weaponSource.tubeReloadingTimer[activeTube];
			}
		}
		int num3 = this.playerVessel.vesselmovement.weaponSource.wantedWeaponInTube[activeTube];
		if (this.playerVessel.vesselmovement.weaponSource.tubeStatus[activeTube] >= 0)
		{
			num3++;
		}
		if (num3 >= this.playerVessel.vesselmovement.weaponSource.torpedoTypes.Length)
		{
			num3 = 0;
		}
		for (int k = 0; k < this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard.Length; k++)
		{
			if (this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[num3] - array[num3] > 0)
			{
				break;
			}
			num3++;
			if (num3 >= this.playerVessel.vesselmovement.weaponSource.torpedoTypes.Length)
			{
				num3 = 0;
			}
		}
		this.playerVessel.vesselmovement.weaponSource.wantedWeaponInTube[activeTube] = num3;
		this.playerVessel.vesselmovement.weaponSource.tubeStatus[activeTube] = num3;
		int playerTorpedoIDInTube = this.GetPlayerTorpedoIDInTube(activeTube);
		this.playerVessel.vesselmovement.weaponSource.weaponInTube[activeTube] = -1;
		this.playerVessel.vesselmovement.weaponSource.tubeReloadingTimer[activeTube] = 0.01f;
		this.playerVessel.vesselmovement.weaponSource.tubeReloadingDirection[activeTube] = 1f;
		if (Time.timeScale == 0f || UIFunctions.globaluifunctions.campaignmanager.playerInPort)
		{
			this.torpedoTubeImages[activeTube].transform.localPosition = new Vector3(0f, -0.5f, 0f);
			this.playerVessel.vesselmovement.weaponSource.weaponInTube[activeTube] = num3;
			this.torpedoTubeImages[activeTube].sprite = UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTube].weaponImage;
			this.playerVessel.vesselmovement.weaponSource.torpedoSearchPattern[activeTube] = this.GetSettingIndex(UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTube].searchSettings[0], this.attackSettingDefinitions);
			this.playerVessel.vesselmovement.weaponSource.torpedoDepthPattern[activeTube] = this.GetSettingIndex(UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTube].heightSettings[0], this.depthSettingDefinitions);
			this.playerVessel.vesselmovement.weaponSource.torpedoHomingPattern[activeTube] = this.GetSettingIndex(UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTube].homeSettings[0], this.homeSettingDefinitions);
			this.playerVessel.vesselmovement.weaponSource.tubeReloadingDirection[activeTube] = 0f;
			this.SetTubeSettingButtons(activeTube);
		}
		else
		{
			this.torpedoTubeImages[activeTube].gameObject.SetActive(true);
			this.torpedoTubeImages[activeTube].sprite = UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTube].weaponImage;
			this.torpedoTubeImages[activeTube].color = new Color(1f, 1f, 1f, 0.5f);
			this.torpedoTubeImages[activeTube].transform.localPosition = new Vector3(0f, -0.5f, 0f);
			this.ClearTubeSettingButtons(activeTube);
		}
		if (this.storesPanel.activeSelf)
		{
			UIFunctions.globaluifunctions.portRearm.SetLoadoutStats();
		}
		GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.weaponDelayTimer = 2f;
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x00069C0C File Offset: 0x00067E0C
	public void InitialiseHUD()
	{
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x00069C10 File Offset: 0x00067E10
	public void HideHUD()
	{
		if (!this.hudHidden)
		{
			this.hudHidden = true;
			UIFunctions.globaluifunctions.HUDholder.SetActive(false);
			if (!this.sensormanager.tacticalmap.background.activeSelf)
			{
				if (this.sensormanager.tacticalmap.minimapIsOpen)
				{
					UIFunctions.globaluifunctions.playerfunctions.HUDTacMap();
					this.hudHiddenWithMiniMapOpen = true;
				}
				else
				{
					this.hudHiddenWithMiniMapOpen = false;
				}
			}
		}
		else
		{
			this.hudHidden = false;
			UIFunctions.globaluifunctions.HUDholder.SetActive(true);
			if (!this.sensormanager.tacticalmap.background.activeSelf && this.hudHiddenWithMiniMapOpen)
			{
				UIFunctions.globaluifunctions.playerfunctions.HUDTacMap();
			}
		}
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x00069CE0 File Offset: 0x00067EE0
	public void SetTimeCompression()
	{
		float num = 1f;
		if (this.timeCompressionToggle == 0)
		{
			num = 10f;
			this.timeCompressionToggle = 1;
			Time.fixedDeltaTime = 0.06f;
			this.SetAllShipsAngularDrag(6.67f);
			this.SetStatusIcon("time compression", true);
		}
		else
		{
			this.timeCompressionToggle = 0;
			Time.fixedDeltaTime = 0.02f;
			this.SetAllShipsAngularDrag(20f);
			this.SetStatusIcon("time compression", false);
		}
		if (num != 0f)
		{
			GameDataManager.cameraTimeScale = 1f / num;
		}
		Time.timeScale = num;
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x00069D78 File Offset: 0x00067F78
	private void SetAllShipsAngularDrag(float amount)
	{
		this.playerVessel.vesselRigidbody.angularDrag = amount;
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			GameDataManager.enemyvesselsonlevel[i].vesselRigidbody.angularDrag = amount;
		}
	}

	// Token: 0x06000921 RID: 2337 RVA: 0x00069DC0 File Offset: 0x00067FC0
	private void CheckDepthMessage()
	{
		int num = this.playerDepthInFeet / 100;
		if (!this.firstDepthCheckDone)
		{
			this.playerDepthTier = num;
			this.firstDepthCheckDone = true;
			return;
		}
		int num2 = 0;
		if (num != this.playerDepthTier)
		{
			if (num < this.playerDepthTier)
			{
				num2 = 1;
			}
			this.playerDepthTier = num;
			if (!this.helmmanager.autoDiving)
			{
				string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "PassingDepth");
				text = text.Replace("<FEET>", LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Feet").ToLower());
				this.voicemanager.currentDepthTier = ((num + num2) * 100).ToString();
				text = text.Replace("<DEPTH>", this.voicemanager.currentDepthTier);
				Color lineColor = this.messageLogColors["Helm"];
				if (this.playerVessel.transform.position.y < this.playerVessel.databaseshipdata.actualTestDepth)
				{
					lineColor = this.messageLogColors["Warning"];
				}
				this.PlayerMessage(text, lineColor, "PassingDepth", false);
			}
		}
		if (this.playerDepthUnderKeel < 50)
		{
			if (!this.statusIcons[1].gameObject.activeSelf)
			{
				this.SetStatusIcon("under keel", true);
			}
			if (!this.depthUnderKeelWarned)
			{
				string text2 = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "TooShallowUnderKeel");
				text2 = text2.Replace("<FEET>", LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Feet").ToLower());
				text2 = text2.Replace("<DEPTH>", "50");
				this.PlayerMessage(text2, this.messageLogColors["Helm"], "TooShallowUnderKeel", false);
				if (this.timeCompressionToggle == 1)
				{
					UIFunctions.globaluifunctions.playerfunctions.SetTimeCompression();
				}
				this.depthUnderKeelWarned = true;
			}
		}
		else
		{
			if (this.statusIcons[1].gameObject.activeSelf)
			{
				this.SetStatusIcon("under keel", false);
			}
			this.depthUnderKeelWarned = false;
		}
	}

	// Token: 0x06000922 RID: 2338 RVA: 0x00069FD4 File Offset: 0x000681D4
	public void SetPlayerData()
	{
		this.ownShipData.text = string.Empty;
		string text = string.Format("{0:0}", this.playerVessel.transform.eulerAngles.y);
		if (text == "360")
		{
			text = "0";
		}
		Text text2 = this.ownShipData;
		text2.text = text2.text + text + "\n";
		Text text3 = this.ownShipData;
		text3.text = text3.text + string.Format("{0:0}", this.playerVessel.vesselmovement.shipSpeed.z * 10f) + "\n";
		this.playerDepthInFeet = (int)Mathf.Round((this.playerVessel.transform.position.y - 1000f) * -GameDataManager.unitsToFeet);
		this.playerDepthUnderKeel = (int)(UIFunctions.globaluifunctions.playerfunctions.sensormanager.GetUnitsDepthUnderTransform(this.playerVessel.transform) * GameDataManager.unitsToFeet);
		this.CheckDepthMessage();
		Text text4 = this.ownShipData;
		text4.text += this.playerDepthInFeet.ToString();
		Text text5 = this.ownShipData;
		text5.text = text5.text + "\n" + string.Format("{0:0}", this.playerVessel.vesselmovement.rudderAngle.x * 10f);
		Text text6 = this.ownShipData;
		text6.text = text6.text + "\n" + string.Format("{0:0}", this.playerVessel.vesselmovement.diveAngle.x * -10f);
		if (this.ballastRechargeTimer <= 0f)
		{
			Text text7 = this.ownShipData;
			text7.text = text7.text + "\n" + string.Format("{0:0}", this.playerVessel.vesselmovement.ballastAngle.x * 10f);
		}
		else
		{
			float num = this.ballastRechargeTimer / this.ballastRechargeTime;
			Text text8 = this.ownShipData;
			text8.text = text8.text + "\n" + string.Format("{0:0}", num * 60f);
		}
		if (this.currentOpenPanel == 1)
		{
			this.SetContactGraphic();
		}
	}

	// Token: 0x06000923 RID: 2339 RVA: 0x0006A24C File Offset: 0x0006844C
	public void DropNoisemaker()
	{
		this.playerVessel.vesselmovement.weaponSource.DropNoisemaker();
	}

	// Token: 0x06000924 RID: 2340 RVA: 0x0006A264 File Offset: 0x00068464
	public void ClearAllDataTexts()
	{
	}

	// Token: 0x06000925 RID: 2341 RVA: 0x0006A268 File Offset: 0x00068468
	public void SetContactToNone()
	{
		this.contactDataName.text = LanguageManager.interfaceDictionary["NoContact"];
		this.contactData.text = string.Empty;
		UIFunctions.globaluifunctions.playerfunctions.SetContactProfileSprite("blank_sig");
	}

	// Token: 0x06000926 RID: 2342 RVA: 0x0006A2B4 File Offset: 0x000684B4
	public void SetTargetData()
	{
		this.contactDataName.text = string.Empty;
		this.contactData.text = string.Empty;
		if (!this.damagecontrol.CheckSubsystem("FIRECONTROL", false))
		{
			this.contactDataName.text = LanguageManager.interfaceDictionary["Offline"];
			this.contactData.text = string.Empty;
			return;
		}
		if (this.sensormanager.classifiedByPlayer[this.currentTargetIndex])
		{
			this.contactDataName.text = this.sensormanager.tacticalmap.mapContact[this.currentTargetIndex].contactText.text.ToUpper();
		}
		else if (this.sensormanager.identifiedByPlayer[this.currentTargetIndex])
		{
			this.contactDataName.text = this.sensormanager.tacticalmap.mapContact[this.currentTargetIndex].contactText.text.ToUpper();
		}
		else
		{
			this.contactDataName.text = LanguageManager.interfaceDictionary["Contact"] + " " + this.sensormanager.tacticalmap.mapContact[this.currentTargetIndex].contactText.text.ToUpper();
		}
		if (this.sensormanager.truthIsOutThere)
		{
			this.contactDataName.text = this.sensormanager.initialDetectedByPlayerName[this.currentTargetIndex] + " " + GameDataManager.enemyvesselsonlevel[this.currentTargetIndex].databaseshipdata.shipclass.ToUpper();
		}
		this.contactData.text = "\n" + string.Format("{0:0}", this.sensormanager.bearingToContacts[this.currentTargetIndex]) + "\n";
		if (this.sensormanager.solutionQualityOfContacts[this.currentTargetIndex] > this.sensormanager.tacticalmap.qualityToCourse)
		{
			Text text = this.contactData;
			text.text = text.text + string.Format("{0:0}", this.sensormanager.headingOfContacts[this.currentTargetIndex]) + "\n";
		}
		else
		{
			Text text2 = this.contactData;
			text2.text += "---\n";
		}
		if (this.sensormanager.solutionQualityOfContacts[this.currentTargetIndex] > this.sensormanager.tacticalmap.qualityToSpeed)
		{
			Text text3 = this.contactData;
			text3.text = text3.text + string.Format("{0:0}", this.sensormanager.speedOfContacts[this.currentTargetIndex]) + "\n";
			if (this.sensormanager.layerStrength > 0f)
			{
				if (!this.contactDepth.gameObject.activeSelf)
				{
					this.contactDepth.gameObject.SetActive(true);
				}
				if (GameDataManager.enemyvesselsonlevel[this.currentTargetIndex].acoustics.currentlyAboveLayer)
				{
					this.contactDepth.sprite = this.contactDepthSprites[0];
				}
				else
				{
					this.contactDepth.sprite = this.contactDepthSprites[1];
				}
			}
		}
		else
		{
			Text text4 = this.contactData;
			text4.text += "---\n";
			this.contactDepth.gameObject.SetActive(false);
		}
		float num = this.sensormanager.rangeToContacts[this.currentTargetIndex] / 1000f;
		if (this.sensormanager.solutionQualityOfContacts[this.currentTargetIndex] > this.sensormanager.tacticalmap.qualityToRange)
		{
			Text text5 = this.contactData;
			text5.text = text5.text + string.Format("{0:0.0}", num * this.sensormanager.solutionRangeErrors[this.currentTargetIndex]) + "\n";
		}
		else
		{
			Text text6 = this.contactData;
			text6.text += "---\n";
		}
		Text text7 = this.contactData;
		text7.text += string.Format("{0:0}", Mathf.Round(this.sensormanager.solutionQualityOfContacts[this.currentTargetIndex]));
	}

	// Token: 0x06000927 RID: 2343 RVA: 0x0006A708 File Offset: 0x00068908
	public void ClearCurrentActiveTorpedo()
	{
		this.currentActiveTorpedo = null;
		PlayerFunctions.draggingWaypoint = false;
		this.sensormanager.tacticalmap.waypointMarker.gameObject.SetActive(false);
		this.wireData[0].text = string.Empty;
		this.wireData[1].text = string.Empty;
	}

	// Token: 0x06000928 RID: 2344 RVA: 0x0006A764 File Offset: 0x00068964
	public void GetNextTube()
	{
		this.ClearCurrentActiveTorpedo();
		if (PlayerFunctions.draggingWaypoint)
		{
			return;
		}
		this.playerVessel.vesselmovement.weaponSource.activeTube++;
		if (this.playerVessel.vesselmovement.weaponSource.activeTube == this.playerVessel.vesselmovement.weaponSource.torpedoTubes.Length)
		{
			this.playerVessel.vesselmovement.weaponSource.activeTube = 0;
		}
		this.HighlightActiveTube();
		if (this.playerVessel.vesselmovement.weaponSource.weaponInTube[this.playerVessel.vesselmovement.weaponSource.activeTube] == -100)
		{
			this.currentActiveTorpedo = this.playerVessel.vesselmovement.weaponSource.torpedoesOnWire[this.playerVessel.vesselmovement.weaponSource.activeTube];
		}
		else
		{
			this.currentActiveTorpedo = null;
			this.SetWireDataText();
		}
	}

	// Token: 0x06000929 RID: 2345 RVA: 0x0006A864 File Offset: 0x00068A64
	public void SetTubesData()
	{
		this.ClearAllDataTexts();
		this.tubeData[0].gameObject.SetActive(true);
		int num = this.playerVessel.vesselmovement.weaponSource.torpedoTubes.Length;
		for (int i = 0; i < num; i++)
		{
			this.tubeData[i].text = string.Empty;
			for (int j = 0; j < i; j++)
			{
				Text text = this.tubeData[i];
				text.text += "\n";
			}
			int num2 = this.playerVessel.vesselmovement.weaponSource.weaponInTube[i];
			if (num2 >= 0)
			{
				Text text2 = this.tubeData[i];
				text2.text = text2.text + (i + 1).ToString() + ": " + this.playerVessel.vesselmovement.weaponSource.torpedoNames[this.playerVessel.vesselmovement.weaponSource.tubeStatus[i]];
			}
			else if (num2 == -10)
			{
				Text text3 = this.tubeData[i];
				text3.text = text3.text + (i + 1).ToString() + ": EMPTY";
			}
			else if (num2 == -100)
			{
				Text text4 = this.tubeData[i];
				text4.text = text4.text + (i + 1).ToString() + ": WIRE";
			}
			else if (num2 == -1)
			{
				float num3 = this.playerVessel.databaseshipdata.tubereloadtime - this.playerVessel.vesselmovement.weaponSource.tubeReloadingTimer[i];
				int num4 = Mathf.FloorToInt(num3 / 60f);
				int num5 = Mathf.FloorToInt(num3 - (float)(num4 * 60));
				Text text5 = this.tubeData[i];
				text5.text = text5.text + (i + 1).ToString() + ": " + this.playerVessel.vesselmovement.weaponSource.torpedoNames[this.playerVessel.vesselmovement.weaponSource.tubeStatus[i]];
				this.depthSettings[i].text = string.Empty;
				this.homeSettings[i].text = string.Empty;
			}
		}
	}

	// Token: 0x0600092A RID: 2346 RVA: 0x0006AAA8 File Offset: 0x00068CA8
	public void DisplayWireWaypoint()
	{
		if (PlayerFunctions.draggingWaypoint)
		{
			return;
		}
		if (this.currentActiveTorpedo != null)
		{
			if (this.currentActiveTorpedo.onWire && this.currentActiveTorpedo.guidanceActive)
			{
				if (!this.sensormanager.tacticalmap.waypointMarker.gameObject.activeSelf)
				{
					this.sensormanager.tacticalmap.waypointMarker.gameObject.SetActive(true);
				}
				float zoomFactor = this.sensormanager.tacticalmap.zoomFactor;
				this.sensormanager.tacticalmap.waypointMarker.transform.localPosition = new Vector3(this.currentActiveTorpedo.initialWaypointPosition.x * zoomFactor, this.currentActiveTorpedo.initialWaypointPosition.z * zoomFactor, 0f);
				this.sensormanager.tacticalmap.waypointLine.SetPosition(1, this.currentActiveTorpedo.tacMapTorpedoIcon.transform.position - this.sensormanager.tacticalmap.waypointMarker.transform.position);
			}
			else if (this.sensormanager.tacticalmap.waypointMarker.gameObject.activeSelf)
			{
				this.sensormanager.tacticalmap.waypointMarker.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x0600092B RID: 2347 RVA: 0x0006AC10 File Offset: 0x00068E10
	public void AutoCentreMap()
	{
		UIFunctions.globaluifunctions.levelloadmanager.tacticalmap.autoCentreMap = !UIFunctions.globaluifunctions.levelloadmanager.tacticalmap.autoCentreMap;
		if (UIFunctions.globaluifunctions.levelloadmanager.tacticalmap.autoCentreMap)
		{
			UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[23].color = UIFunctions.globaluifunctions.playerfunctions.helmmanager.buttonColors[1];
		}
		else
		{
			UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[23].color = UIFunctions.globaluifunctions.playerfunctions.helmmanager.buttonColors[0];
		}
	}

	// Token: 0x0600092C RID: 2348 RVA: 0x0006ACE8 File Offset: 0x00068EE8
	private void SetWireDataText()
	{
		if (this.currentActiveTorpedo == null)
		{
			this.wireData[0].text = string.Empty;
			this.wireData[1].text = string.Empty;
			return;
		}
		int tubefiredFrom = this.currentActiveTorpedo.tubefiredFrom;
		if (TacticalMap.tacMapEnabled)
		{
			this.DisplayWireWaypoint();
		}
		this.wireData[0].text = (this.playerVessel.vesselmovement.weaponSource.torpedoesOnWire[tubefiredFrom].tubefiredFrom + 1).ToString();
		float y = this.playerVessel.vesselmovement.weaponSource.torpedoesOnWire[tubefiredFrom].transform.eulerAngles.y;
		this.wireData[1].text = "\n" + string.Format("{0:0}", y) + "\n";
		float num = Vector3.Distance(this.playerVessel.transform.position, this.playerVessel.vesselmovement.weaponSource.torpedoesOnWire[tubefiredFrom].transform.position) * GameDataManager.yardsScale / 1000f;
		Text text = this.wireData[1];
		text.text = text.text + string.Format("{0:0.0}", num) + "\n";
		this.playerVessel.acoustics.sensorNavigator.transform.LookAt(this.playerVessel.vesselmovement.weaponSource.torpedoesOnWire[tubefiredFrom].transform.position);
		string text2 = string.Format("{0:0}", this.playerVessel.acoustics.sensorNavigator.transform.eulerAngles.y);
		if (text2 == "360")
		{
			text2 = "0";
		}
		Text text3 = this.wireData[1];
		text3.text = text3.text + string.Format("{0:0}", text2) + "\n";
		if (this.playerVessel.vesselmovement.weaponSource.torpedoesOnWire[tubefiredFrom].guidanceActive)
		{
			num = Vector2.Distance(new Vector2(this.playerVessel.vesselmovement.weaponSource.torpedoesOnWire[tubefiredFrom].transform.position.x, this.playerVessel.vesselmovement.weaponSource.torpedoesOnWire[tubefiredFrom].transform.position.z), new Vector2(this.playerVessel.vesselmovement.weaponSource.torpedoesOnWire[tubefiredFrom].initialWaypointPosition.x, this.playerVessel.vesselmovement.weaponSource.torpedoesOnWire[tubefiredFrom].initialWaypointPosition.z)) * GameDataManager.yardsScale / 1000f;
			Text text4 = this.wireData[1];
			text4.text = text4.text + string.Format("{0:0.0}", num) + "\n";
		}
		else
		{
			Text text5 = this.wireData[1];
			text5.text += "---\n";
		}
		float num2 = this.playerVessel.vesselmovement.weaponSource.torpedoesOnWire[tubefiredFrom].databaseweapondata.runTime - this.playerVessel.vesselmovement.weaponSource.torpedoesOnWire[tubefiredFrom].timer;
		int num3 = Mathf.FloorToInt(num2 / 60f);
		int num4 = Mathf.FloorToInt(num2 - (float)(num3 * 60));
		Text text6 = this.wireData[1];
		text6.text += string.Format("{0:00}:{1:00}", num3, num4);
	}

	// Token: 0x0600092D RID: 2349 RVA: 0x0006B0A0 File Offset: 0x000692A0
	public void EnableTorpedo()
	{
		if (this.currentActiveTorpedo != null && !this.currentActiveTorpedo.sensorsActive)
		{
			this.currentActiveTorpedo.ActivateTorpedo();
		}
	}

	// Token: 0x0600092E RID: 2350 RVA: 0x0006B0DC File Offset: 0x000692DC
	public void DeactivateTorpedo()
	{
		if (this.currentActiveTorpedo != null)
		{
			this.currentActiveTorpedo.DeactivateTorpedo();
		}
	}

	// Token: 0x0600092F RID: 2351 RVA: 0x0006B0FC File Offset: 0x000692FC
	public void CutWire(int tubeNumber)
	{
		if (GameDataManager.trainingMode)
		{
			return;
		}
		if (this.playerVessel.vesselmovement.weaponSource.torpedoesOnWire[tubeNumber] != null)
		{
			if (this.playerVessel.vesselmovement.weaponSource.torpedoesOnWire[tubeNumber].onWire)
			{
				this.playerVessel.vesselmovement.weaponSource.torpedoesOnWire[tubeNumber].TorpedoCutWire();
			}
			if (this.currentActiveTorpedo != null && this.currentActiveTorpedo.tubefiredFrom == tubeNumber)
			{
				this.ClearCurrentActiveTorpedo();
			}
		}
	}

	// Token: 0x06000930 RID: 2352 RVA: 0x0006B19C File Offset: 0x0006939C
	public void ButtonTorpedoButtons(string action)
	{
		if (this.currentActiveTorpedo != null)
		{
			if (action == "ACTIVATE")
			{
				this.EnableTorpedo();
				return;
			}
			if (action == "CUT")
			{
				this.CutWire(this.currentActiveTorpedo.tubefiredFrom);
				return;
			}
			if (action == "WAYPOINT")
			{
				if (this.currentActiveTorpedo.guidanceActive)
				{
					this.SetWeaponWaypoint();
				}
				return;
			}
		}
		if (action == "FIRE" && UIFunctions.globaluifunctions.HUDholder.activeSelf)
		{
			this.LaunchWeapon();
		}
	}

	// Token: 0x06000931 RID: 2353 RVA: 0x0006B244 File Offset: 0x00069444
	public void ManualSteerTorpedo(int dir)
	{
		if (this.currentActiveTorpedo != null && dir > -1)
		{
			switch (dir)
			{
			case 0:
				this.TorpedoDepth(1f);
				break;
			case 1:
				this.TorpedoDepth(-1f);
				break;
			case 2:
				this.SteerTorpedo(-1f);
				break;
			case 3:
				this.SteerTorpedo(1f);
				break;
			}
		}
	}

	// Token: 0x06000932 RID: 2354 RVA: 0x0006B2C8 File Offset: 0x000694C8
	private int GetPlayerWeaponIndex(string weapontype)
	{
		for (int i = 0; i < this.playerVessel.vesselmovement.weaponSource.torpedoTypes.Length; i++)
		{
			if (UIFunctions.globaluifunctions.database.databaseweapondata[i].weaponType == weapontype)
			{
				return i;
			}
		}
		return 0;
	}

	// Token: 0x06000933 RID: 2355 RVA: 0x0006B324 File Offset: 0x00069524
	public void LaunchWeapon()
	{
		if (this.playerVessel.vesselmovement.weaponSource.weaponInTube[this.playerVessel.vesselmovement.weaponSource.activeTube] >= 0)
		{
			if (!this.tacMapMaximisedGraphic.enabled && !this.sensormanager.tacticalmap.background.activeSelf)
			{
				this.HUDTacMap();
			}
			this.FireTube();
			return;
		}
		string text = "NotReady";
		string text2 = LanguageManager.messageLogDictionary["NotReady"];
		if (this.playerVessel.vesselmovement.weaponSource.tubeStatus[this.playerVessel.vesselmovement.weaponSource.activeTube] == -200)
		{
			text2 = LanguageManager.messageLogDictionary["TubeDestroyed"];
			text = "TubeDestroyed";
		}
		text2 = LanguageManager.messageLogDictionary[text];
		text2 = text2.Replace("<TUBE>", (this.playerVessel.vesselmovement.weaponSource.activeTube + 1).ToString());
		this.PlayerMessage(text2, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["TorpedoRoom"], text, false);
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x0006B454 File Offset: 0x00069654
	public void LoadWeapon(string weapontype)
	{
		int playerWeaponIndex = this.GetPlayerWeaponIndex(weapontype);
		if (this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[playerWeaponIndex] == 0)
		{
			this.PlayerMessage(LanguageManager.messageLogDictionary["NoReloadsAvailable"], this.messageLogColors["TorpedoRoom"], "NoReloadsAvailable", true);
			return;
		}
		int num = this.playerVessel.vesselmovement.weaponSource.torpedoTubes.Length;
		for (int i = 0; i < num; i++)
		{
			if (this.playerVessel.vesselmovement.weaponSource.weaponInTube[i] == -10)
			{
				this.playerVessel.vesselmovement.weaponSource.activeTube = i;
				this.playerVessel.vesselmovement.weaponSource.weaponInTube[i] = -1;
				this.playerVessel.vesselmovement.weaponSource.tubeStatus[i] = playerWeaponIndex;
				string text = LanguageManager.messageLogDictionary["LoadWeapon"];
				text = text.Replace("<WEAPON>", this.playerVessel.vesselmovement.weaponSource.torpedoNames[playerWeaponIndex]);
				text = text.Replace("<TUBE>", (this.playerVessel.vesselmovement.weaponSource.activeTube + 1).ToString());
				this.PlayerMessage(text, this.messageLogColors["TorpedoRoom"], "LoadWeapon", false);
				if (this.tubeData[0].gameObject.activeSelf)
				{
					this.SetTubesData();
				}
				if (this.statusscreens.statusPages[0].activeSelf)
				{
					this.statusscreens.SetLoadoutStats();
					this.playerVessel.vesselmovement.weaponSource.FixedUpdate();
				}
				return;
			}
		}
	}

	// Token: 0x06000935 RID: 2357 RVA: 0x0006B610 File Offset: 0x00069810
	public void SteerTorpedo(float steer)
	{
		if (this.currentActiveTorpedo != null && this.currentActiveTorpedo.sensorsActive)
		{
			this.currentActiveTorpedo.playerControlling = true;
			this.currentActiveTorpedo.playerTurnInput = steer;
		}
	}

	// Token: 0x06000936 RID: 2358 RVA: 0x0006B64C File Offset: 0x0006984C
	public void TorpedoDepth(float depth)
	{
		if (this.currentActiveTorpedo != null && this.currentActiveTorpedo.sensorsActive)
		{
			this.currentActiveTorpedo.playerControlling = true;
			this.currentActiveTorpedo.playerDepthInput = depth;
		}
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x0006B688 File Offset: 0x00069888
	public void PlayerRudderLeft()
	{
		this.playerVessel.vesselmovement.rudderAngle.x = this.RoundToNearest(this.playerVessel.vesselmovement.rudderAngle.x, 0.5f);
		this.helmmanager.CancelAutoTurning();
		if (this.playerVessel.vesselmovement.rudderAngle.x > -3f)
		{
			VesselMovement vesselmovement = this.playerVessel.vesselmovement;
			vesselmovement.rudderAngle.x = vesselmovement.rudderAngle.x - 0.5f;
		}
		if (this.playerVessel.vesselmovement.rudderAngle.x == -3f)
		{
			this.CheckIfKnuckle();
		}
		this.SetRudderColor();
		this.SetPlayerData();
	}

	// Token: 0x06000938 RID: 2360 RVA: 0x0006B748 File Offset: 0x00069948
	public void PlayerRudderRight()
	{
		this.playerVessel.vesselmovement.rudderAngle.x = this.RoundToNearest(this.playerVessel.vesselmovement.rudderAngle.x, 0.5f);
		this.helmmanager.CancelAutoTurning();
		if (this.playerVessel.vesselmovement.rudderAngle.x < 3f)
		{
			VesselMovement vesselmovement = this.playerVessel.vesselmovement;
			vesselmovement.rudderAngle.x = vesselmovement.rudderAngle.x + 0.5f;
		}
		if (this.playerVessel.vesselmovement.rudderAngle.x == 3f)
		{
			this.CheckIfKnuckle();
		}
		this.SetRudderColor();
		this.SetPlayerData();
	}

	// Token: 0x06000939 RID: 2361 RVA: 0x0006B808 File Offset: 0x00069A08
	private void SetRudderColor()
	{
		if (this.playerVessel.vesselmovement.rudderAngle.x != 0f)
		{
			this.playerShipData[4].color = Color.green;
		}
		else
		{
			this.playerShipData[4].color = Color.white;
		}
	}

	// Token: 0x0600093A RID: 2362 RVA: 0x0006B860 File Offset: 0x00069A60
	private void SetPlanesColor()
	{
		if (this.playerVessel.vesselmovement.diveAngle.x != 0f)
		{
			this.playerShipData[3].color = Color.green;
		}
		else
		{
			this.playerShipData[3].color = Color.white;
		}
	}

	// Token: 0x0600093B RID: 2363 RVA: 0x0006B8B8 File Offset: 0x00069AB8
	private void SetBallastColor()
	{
		if (this.playerVessel.vesselmovement.ballastAngle.x != 0f)
		{
			this.playerShipData[5].color = Color.green;
		}
		else
		{
			this.playerShipData[5].color = Color.white;
		}
	}

	// Token: 0x0600093C RID: 2364 RVA: 0x0006B910 File Offset: 0x00069B10
	public bool CheckIfKnuckle()
	{
		if (Mathf.Abs(this.playerVessel.vesselmovement.rudderAngle.x) - Mathf.Abs(this.playerVessel.vesselmovement.rudderAngle.y) < 2f)
		{
			return false;
		}
		float num = this.playerVessel.vesselmovement.shipSpeed.z * 10f;
		if (num < 24f)
		{
			return false;
		}
		if (!SensorManager.playerKnuckle)
		{
			if (this.playerVessel.vesselmovement.rudderAngle.x == 3f || this.playerVessel.vesselmovement.rudderAngle.x == -3f)
			{
				SensorManager.playerKnuckle = true;
				Noisemaker component = ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.databasecountermeasuredata[this.sensormanager.knuckleID].countermeasureObject, this.playerVessel.vesselmovement.rudder[0].position, Quaternion.identity).GetComponent<Noisemaker>();
				component.gameObject.SetActive(true);
				component.tacMapNoisemakerIcon.shipDisplayIcon.color = this.sensormanager.tacticalmap.navyColors[0];
				component.databasecountermeasuredata = UIFunctions.globaluifunctions.database.databasecountermeasuredata[this.sensormanager.knuckleID];
				component.playerGenerated = true;
				this.playerVessel.uifunctions.playerfunctions.sensormanager.AddNoiseMakerToArray(component);
				this.PlayerMessage(LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "KnuckleFormed"), this.messageLogColors["Helm"], "KnuckleFormed", false);
			}
			return true;
		}
		return false;
	}

	// Token: 0x0600093D RID: 2365 RVA: 0x0006BAC4 File Offset: 0x00069CC4
	private float RoundToNearest(float value, float factor)
	{
		if (factor == 0f)
		{
			factor = 1f;
		}
		return Mathf.Round(value / factor) * factor;
	}

	// Token: 0x0600093E RID: 2366 RVA: 0x0006BAE4 File Offset: 0x00069CE4
	public void PlayerDiveSubmarine()
	{
		this.playerVessel.vesselmovement.diveAngle.x = this.RoundToNearest(this.playerVessel.vesselmovement.diveAngle.x, 0.5f);
		this.helmmanager.CancelAutoDiving();
		if (this.playerVessel.vesselmovement.diveAngle.x < 3f)
		{
			VesselMovement vesselmovement = this.playerVessel.vesselmovement;
			vesselmovement.diveAngle.x = vesselmovement.diveAngle.x + 0.5f;
			this.playerVessel.vesselmovement.levelOutSubmarine = this.IsPlayerLevellingOut();
		}
		this.SetPlanesColor();
		this.SetPlayerData();
	}

	// Token: 0x0600093F RID: 2367 RVA: 0x0006BB94 File Offset: 0x00069D94
	public void PlayerSurfaceSubmarine()
	{
		this.playerVessel.vesselmovement.diveAngle.x = this.RoundToNearest(this.playerVessel.vesselmovement.diveAngle.x, 0.5f);
		this.helmmanager.CancelAutoDiving();
		if (this.playerVessel.vesselmovement.diveAngle.x > -3f)
		{
			VesselMovement vesselmovement = this.playerVessel.vesselmovement;
			vesselmovement.diveAngle.x = vesselmovement.diveAngle.x - 0.5f;
			this.playerVessel.vesselmovement.levelOutSubmarine = this.IsPlayerLevellingOut();
		}
		this.SetPlanesColor();
		this.SetPlayerData();
	}

	// Token: 0x06000940 RID: 2368 RVA: 0x0006BC44 File Offset: 0x00069E44
	public void LevelSubmarine()
	{
		this.playerVessel.vesselmovement.rudderAngle.x = 0f;
		this.playerVessel.vesselmovement.diveAngle.x = 0f;
		this.playerVessel.vesselmovement.ballastAngle.x = 0f;
		this.playerVessel.vesselmovement.levelOutSubmarine = true;
		this.SetRudderColor();
		this.SetPlanesColor();
		this.SetBallastColor();
		this.SetPlayerData();
		this.helmmanager.CancelAutoTurning();
		this.helmmanager.CancelAutoDiving();
	}

	// Token: 0x06000941 RID: 2369 RVA: 0x0006BCE0 File Offset: 0x00069EE0
	public void EmergencyDeep()
	{
		this.helmmanager.autoDiving = false;
		if (this.playerVessel.vesselmovement.blowBallast)
		{
			this.PlayerMessage(LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "BallastRecharging"), this.messageLogColors["Helm"], "BallastRecharging", true);
			return;
		}
		for (int i = 0; i < this.playerVessel.submarineFunctions.mastTransforms.Length; i++)
		{
			if (this.playerVessel.submarineFunctions.upScope[i] || this.playerVessel.submarineFunctions.scopeStatus[i] == 1)
			{
				switch (i)
				{
				case 0:
					if (this.damagecontrol.CheckSubsystem("PERISCOPE", true))
					{
						this.MoveScope(0);
					}
					break;
				case 1:
					if (this.damagecontrol.CheckSubsystem("ESM_MAST", true))
					{
						this.MoveScope(1);
					}
					break;
				case 2:
					if (this.damagecontrol.CheckSubsystem("RADAR_MAST", true))
					{
						this.MoveScope(2);
					}
					break;
				}
			}
		}
		this.playerVessel.vesselmovement.diveAngle.x = 3f;
		this.playerVessel.vesselmovement.ballastAngle.x = -3f;
		this.helmmanager.SetDirectTelegraph(6);
	}

	// Token: 0x06000942 RID: 2370 RVA: 0x0006BE4C File Offset: 0x0006A04C
	public void BlowBallast()
	{
		if (this.damagecontrol.CheckSubsystem("BALLAST", true) && this.ballastRechargeTimer <= 0f)
		{
			this.ballastRechargeTimer = this.ballastRechargeTime;
			this.PlayerMessage(LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "BlowBallast"), this.messageLogColors["Helm"], "BlowBallast", false);
			this.playerVessel.vesselmovement.blowBallast = true;
			this.playerVessel.vesselmovement.ballastAngle = Vector3.zero;
			this.playerVessel.vesselmovement.diveAngle.x = -3f;
			this.helmmanager.wantedDepth = 0f;
			this.helmmanager.depthDisplayText.text = this.helmmanager.wantedDepth.ToString();
			UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentWantedDepth = this.helmmanager.depthDisplayText.text;
			if (this.playerVessel.damagesystem.emergencyBlow != null)
			{
				this.playerVessel.damagesystem.emergencyBlow.Play();
				this.playerVessel.damagesystem.emergencyBlow.GetComponent<AudioSource>().Play();
				this.helmmanager.guiButtonImages[7].color = this.helmmanager.buttonColors[1];
			}
		}
		else
		{
			this.PlayerMessage(LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "BallastRecharging"), this.messageLogColors["Helm"], "BallastRecharging", true);
		}
		this.helmmanager.CancelAutoDiving();
	}

	// Token: 0x06000943 RID: 2371 RVA: 0x0006C004 File Offset: 0x0006A204
	private bool IsPlayerLevellingOut()
	{
		return this.playerVessel.vesselmovement.diveAngle.x == 0f;
	}

	// Token: 0x06000944 RID: 2372 RVA: 0x0006C034 File Offset: 0x0006A234
	public void PlayerBallastDown()
	{
		if (this.playerVessel.vesselmovement.blowBallast)
		{
			this.PlayerMessage(LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "BallastRecharging"), this.messageLogColors["Helm"], "BallastRecharging", true);
			return;
		}
		this.playerVessel.vesselmovement.ballastAngle.x = this.RoundToNearest(this.playerVessel.vesselmovement.ballastAngle.x, 0.5f);
		this.helmmanager.CancelAutoDiving();
		if (this.playerVessel.vesselmovement.ballastAngle.x < 3f)
		{
			VesselMovement vesselmovement = this.playerVessel.vesselmovement;
			vesselmovement.ballastAngle.x = vesselmovement.ballastAngle.x + 0.5f;
		}
		this.SetBallastColor();
		this.SetPlayerData();
	}

	// Token: 0x06000945 RID: 2373 RVA: 0x0006C110 File Offset: 0x0006A310
	public void PlayerBallastUp()
	{
		if (this.playerVessel.vesselmovement.blowBallast)
		{
			this.PlayerMessage(LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "BallastRecharging"), this.messageLogColors["Helm"], "BallastRecharging", true);
			return;
		}
		this.playerVessel.vesselmovement.ballastAngle.x = this.RoundToNearest(this.playerVessel.vesselmovement.ballastAngle.x, 0.5f);
		this.helmmanager.CancelAutoDiving();
		if (this.playerVessel.vesselmovement.ballastAngle.x > -3f)
		{
			VesselMovement vesselmovement = this.playerVessel.vesselmovement;
			vesselmovement.ballastAngle.x = vesselmovement.ballastAngle.x - 0.5f;
		}
		this.SetBallastColor();
		this.SetPlayerData();
	}

	// Token: 0x06000946 RID: 2374 RVA: 0x0006C1EC File Offset: 0x0006A3EC
	public void MarkTarget()
	{
		if (ManualCameraZoom.binoculars)
		{
			int layerMask = this.markTargetLayerMask;
			Vector3 origin = new Vector3(this.playerVessel.transform.position.x, 1000f, this.playerVessel.transform.position.z);
			RaycastHit raycastHit;
			if (Physics.Raycast(origin, this.playerVessel.uifunctions.cameraMount.forward, out raycastHit, 500f, layerMask))
			{
				if (raycastHit.transform.gameObject.name.Contains("Ice"))
				{
					this.sensormanager.tacticalmap.PlaceHazardIconOnMap(raycastHit.point, 0);
					return;
				}
				Vessel component = raycastHit.transform.gameObject.GetComponent<Vessel>();
				if (component != null)
				{
					this.currentTargetIndex = component.vesselListIndex;
					this.sensormanager.ContactDetected(this.currentTargetIndex, "VISUAL");
					if (!this.sensormanager.classifiedByPlayer[this.currentTargetIndex])
					{
						this.sensormanager.ContactClassified(this.currentTargetIndex);
					}
					this.sensormanager.solutionQualityOfContacts[this.currentTargetIndex] = this.maximumPlayerTMA;
					this.sensormanager.CalculateRangeError(this.currentTargetIndex);
					UIFunctions.globaluifunctions.playerfunctions.SetContactProfileSprite(GameDataManager.enemyvesselsonlevel[this.currentTargetIndex].databaseshipdata.shipPrefabName + "_sig");
					this.SetTargetData();
					UIFunctions.globaluifunctions.playerfunctions.sensormanager.SetSignatureData();
				}
			}
		}
	}

	// Token: 0x06000947 RID: 2375 RVA: 0x0006C388 File Offset: 0x0006A588
	public void NightVision()
	{
		if (ManualCameraZoom.binoculars)
		{
			if (this.periscopeNightVisionLight.enabled)
			{
				this.periscopeNightVisionLight.enabled = false;
				this.helmmanager.guiButtonImages[14].color = this.helmmanager.buttonColors[0];
				if (GameDataManager.optionsBoolSettings[6])
				{
					if (ManualCameraZoom.underwater)
					{
						UIFunctions.globaluifunctions.levelloadmanager.amplifycoloreffect.LutTexture = UIFunctions.globaluifunctions.levelloadmanager.amplifyColorTextures[1];
					}
					else
					{
						UIFunctions.globaluifunctions.levelloadmanager.amplifycoloreffect.LutTexture = UIFunctions.globaluifunctions.levelloadmanager.amplifyColorTextures[0];
					}
				}
				else
				{
					UIFunctions.globaluifunctions.levelloadmanager.amplifycoloreffect.enabled = false;
				}
			}
			else
			{
				this.periscopeNightVisionLight.enabled = true;
				this.helmmanager.guiButtonImages[14].color = this.helmmanager.buttonColors[1];
				UIFunctions.globaluifunctions.levelloadmanager.amplifycoloreffect.LutTexture = this.nightVisionRamp;
			}
		}
	}

	// Token: 0x06000948 RID: 2376 RVA: 0x0006C4C4 File Offset: 0x0006A6C4
	public void IncreaseTelegraph()
	{
		if (!this.damagecontrol.CheckSubsystem("REACTOR", true))
		{
			this.playerVessel.vesselmovement.telegraphValue = 1;
			return;
		}
		this.usingKnots = false;
		this.playerVessel.vesselmovement.telegraphValue++;
		if (PlayerFunctions.runningSilent && this.playerVessel.vesselmovement.telegraphValue > this.damagecontrol.playerMaxTelegraph)
		{
			this.LeaveRunningSilent();
		}
		if (this.playerVessel.vesselmovement.telegraphValue > 6)
		{
			this.playerVessel.vesselmovement.telegraphValue = 6;
			return;
		}
		if (this.playerVessel.vesselmovement.telegraphValue > this.damagecontrol.playerMaxTelegraph)
		{
			this.playerVessel.vesselmovement.telegraphValue = this.damagecontrol.playerMaxTelegraph;
			return;
		}
		this.TelegraphMessage();
		this.telegraphDelayTimer = 1f;
		this.helmmanager.atSpeed = false;
	}

	// Token: 0x06000949 RID: 2377 RVA: 0x0006C5CC File Offset: 0x0006A7CC
	public void DecreaseTelegraph()
	{
		if (!this.damagecontrol.CheckSubsystem("REACTOR", true))
		{
			this.playerVessel.vesselmovement.telegraphValue = 1;
			return;
		}
		this.usingKnots = false;
		this.playerVessel.vesselmovement.telegraphValue--;
		if (this.playerVessel.vesselmovement.telegraphValue < 0)
		{
			this.playerVessel.vesselmovement.telegraphValue = 0;
			return;
		}
		this.TelegraphMessage();
		this.telegraphDelayTimer = 1f;
		this.helmmanager.atSpeed = false;
	}

	// Token: 0x0600094A RID: 2378 RVA: 0x0006C668 File Offset: 0x0006A868
	public void TelegraphMessage()
	{
		if (PlayerFunctions.runningSilent && this.playerVessel.vesselmovement.telegraphValue > 2)
		{
			this.LeaveRunningSilent();
		}
		float num = (float)Mathf.RoundToInt(this.playerVessel.vesselmovement.GetActualVesselSpeed() * 10f);
		UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentSpeed = num.ToString();
		string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "SetSpeed");
		if (num >= 0f)
		{
			text = text.Replace("<SPEED>", num.ToString());
			this.PlayerMessage(text, this.messageLogColors["Maneuver"], "SetSpeed", true);
		}
		else
		{
			text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "BackSpeed");
			this.PlayerMessage(text, this.messageLogColors["Maneuver"], "BackSpeed", true);
		}
		this.helmmanager.DisplayCurrentTelegraph();
		this.playerVessel.vesselmovement.engineSpeed.x = (float)(-1 + this.playerVessel.vesselmovement.telegraphValue) / 5f * this.playerVessel.vesselmovement.shipSpeed.y;
		if (!this.usingKnots)
		{
			this.helmmanager.wantedSpeed = (float)Mathf.RoundToInt(UIFunctions.globaluifunctions.playerfunctions.playerVessel.databaseshipdata.telegraphSpeeds[this.playerVessel.vesselmovement.telegraphValue] * 10f);
			this.helmmanager.speedDisplayText.text = this.helmmanager.wantedSpeed.ToString();
		}
	}

	// Token: 0x0600094B RID: 2379 RVA: 0x0006C810 File Offset: 0x0006AA10
	public void ScopeOne()
	{
		if (this.damagecontrol.CheckSubsystem("PERISCOPE", true))
		{
			this.MoveScope(0);
		}
	}

	// Token: 0x0600094C RID: 2380 RVA: 0x0006C830 File Offset: 0x0006AA30
	public void ScopeTwo()
	{
		if (this.damagecontrol.CheckSubsystem("ESM_MAST", true))
		{
			this.MoveScope(1);
		}
	}

	// Token: 0x0600094D RID: 2381 RVA: 0x0006C850 File Offset: 0x0006AA50
	public void ScopeThree()
	{
		if (this.damagecontrol.CheckSubsystem("RADAR_MAST", true))
		{
			this.MoveScope(2);
		}
	}

	// Token: 0x0600094E RID: 2382 RVA: 0x0006C870 File Offset: 0x0006AA70
	private bool CheckScopeDepth()
	{
		if (this.playerDepthInFeet > this.mastThresholdDepth)
		{
			string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "CannotUseMasts");
			text = text.Replace("<THRESHOLD>", this.mastThresholdDepth.ToString());
			text = text.Replace("<FEET>", LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Feet").ToLower());
			this.PlayerMessage(text, this.messageLogColors["XO"], "CannotUseMasts", true);
			return false;
		}
		return true;
	}

	// Token: 0x0600094F RID: 2383 RVA: 0x0006C8F8 File Offset: 0x0006AAF8
	public void MoveScope(int i)
	{
		bool flag = false;
		if (this.playerVessel.submarineFunctions.upScope[i])
		{
			this.playerVessel.submarineFunctions.upScope[i] = false;
			this.playerVessel.submarineFunctions.DownScopeFunction(i);
		}
		else if (this.playerVessel.submarineFunctions.downScope[i])
		{
			if (this.CheckScopeDepth())
			{
				this.playerVessel.submarineFunctions.downScope[i] = false;
				this.playerVessel.submarineFunctions.UpScopeFunction(i);
				flag = true;
			}
		}
		else if (this.playerVessel.submarineFunctions.scopeStatus[i] == -1)
		{
			if (this.CheckScopeDepth())
			{
				this.playerVessel.submarineFunctions.UpScopeFunction(i);
				flag = true;
			}
		}
		else if (this.playerVessel.submarineFunctions.scopeStatus[i] == 1)
		{
			this.playerVessel.submarineFunctions.DownScopeFunction(i);
		}
		if (!flag)
		{
			if (i == 0)
			{
				UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("periscope", flag);
				this.helmmanager.guiButtonImages[9].color = this.helmmanager.buttonColors[0];
			}
			else if (i == 1)
			{
				UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("esm", flag);
				this.helmmanager.guiButtonImages[10].color = this.helmmanager.buttonColors[0];
			}
			else
			{
				UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("radar", flag);
				this.helmmanager.guiButtonImages[11].color = this.helmmanager.buttonColors[0];
			}
		}
		else if (i == 0)
		{
			this.helmmanager.guiButtonImages[9].color = this.helmmanager.buttonColors[1];
		}
		else if (i == 1)
		{
			this.helmmanager.guiButtonImages[10].color = this.helmmanager.buttonColors[1];
		}
		else
		{
			this.helmmanager.guiButtonImages[11].color = this.helmmanager.buttonColors[1];
		}
	}

	// Token: 0x06000950 RID: 2384 RVA: 0x0006CB84 File Offset: 0x0006AD84
	public void BringInConditions()
	{
		if (TacticalMap.tacMapEnabled)
		{
			this.sensormanager.tacticalmap.SetTacticalMap();
		}
		else if (StatusScreens.statusPagesEnabled)
		{
			this.statusscreens.CloseStatusScreens();
		}
		StatusScreens.conditionsEnabled = true;
		this.statusscreens.conditionsObject.SetActive(true);
		this.sensormanager.conditionsdisplay.UpdateConditionsDisplay(false);
		UIFunctions.globaluifunctions.guiCameraOverlay.enabled = true;
		UIFunctions.globaluifunctions.guiCameraBloom.enabled = true;
		UIFunctions.globaluifunctions.SetMenuSystem("STATUSSCREENS");
		UIFunctions.globaluifunctions.backgroundTemplate.SetActive(false);
		for (int i = 0; i < this.statusscreens.statusPages.Length; i++)
		{
			this.statusscreens.statusPages[i].SetActive(false);
		}
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

	// Token: 0x06000951 RID: 2385 RVA: 0x0006CCA4 File Offset: 0x0006AEA4
	public void FreezeCamera()
	{
		if (ManualCameraZoom.target == ManualCameraZoom.cameraDummyTransform)
		{
			if (ManualCameraZoom.previousTarget != null)
			{
				ManualCameraZoom.target = ManualCameraZoom.previousTarget;
			}
		}
		else
		{
			ManualCameraZoom.previousTarget = ManualCameraZoom.target;
			ManualCameraZoom.cameraDummyTransform.position = ManualCameraZoom.target.transform.position;
			ManualCameraZoom.target = ManualCameraZoom.cameraDummyTransform;
		}
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x0006CD14 File Offset: 0x0006AF14
	public void ExitConditions()
	{
		StatusScreens.conditionsEnabled = false;
		this.statusscreens.conditionsObject.SetActive(false);
		UIFunctions.globaluifunctions.menuSystemParent.SetActive(false);
		AudioListener component = UIFunctions.globaluifunctions.GUICamera.GetComponent<AudioListener>();
		if (!component.enabled)
		{
			component.enabled = true;
		}
		UIFunctions.globaluifunctions.GUICamera.SetActive(false);
		UIFunctions.globaluifunctions.guiCameraOverlay.enabled = false;
		UIFunctions.globaluifunctions.guiCameraBloom.enabled = false;
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x0006CD9C File Offset: 0x0006AF9C
	public string GetPlayerNavySuffix()
	{
		return "_usn";
	}

	// Token: 0x04000DAC RID: 3500
	public SensorManager sensormanager;

	// Token: 0x04000DAD RID: 3501
	public StatusScreens statusscreens;

	// Token: 0x04000DAE RID: 3502
	public DamageControl damagecontrol;

	// Token: 0x04000DAF RID: 3503
	public EventCamera eventcamera;

	// Token: 0x04000DB0 RID: 3504
	public HelmManager helmmanager;

	// Token: 0x04000DB1 RID: 3505
	public VoiceManager voicemanager;

	// Token: 0x04000DB2 RID: 3506
	public Vessel playerVessel;

	// Token: 0x04000DB3 RID: 3507
	public int currentTargetIndex;

	// Token: 0x04000DB4 RID: 3508
	public int currentWeaponIndex;

	// Token: 0x04000DB5 RID: 3509
	public int currentAircraftIndex;

	// Token: 0x04000DB6 RID: 3510
	public int currentSignatureIndex;

	// Token: 0x04000DB7 RID: 3511
	public int currentSceneryIndex;

	// Token: 0x04000DB8 RID: 3512
	public Text[] messageLog;

	// Token: 0x04000DB9 RID: 3513
	public Image[] messageLogBackgrounds;

	// Token: 0x04000DBA RID: 3514
	public float[] messageLogAlphas;

	// Token: 0x04000DBB RID: 3515
	public float messageLogFadeRate;

	// Token: 0x04000DBC RID: 3516
	public Dictionary<string, Color> messageLogColors;

	// Token: 0x04000DBD RID: 3517
	public float DepthChange;

	// Token: 0x04000DBE RID: 3518
	public float maximumPlayerTMA = 95f;

	// Token: 0x04000DBF RID: 3519
	public Color textColorDefault;

	// Token: 0x04000DC0 RID: 3520
	public Color textColorRed;

	// Token: 0x04000DC1 RID: 3521
	public Color[] logColors;

	// Token: 0x04000DC2 RID: 3522
	public int originallyInTube;

	// Token: 0x04000DC3 RID: 3523
	public float originalTimer;

	// Token: 0x04000DC4 RID: 3524
	public float weaponRange;

	// Token: 0x04000DC5 RID: 3525
	public Torpedo currentActiveTorpedo;

	// Token: 0x04000DC6 RID: 3526
	public Vector2 weaponRangeButtonTimers;

	// Token: 0x04000DC7 RID: 3527
	public Vector2 torpedoHeadingButtonTimers;

	// Token: 0x04000DC8 RID: 3528
	public Text[] playerShipData;

	// Token: 0x04000DC9 RID: 3529
	public Text[] tubeData;

	// Token: 0x04000DCA RID: 3530
	public Text[] searchSettings;

	// Token: 0x04000DCB RID: 3531
	public Text[] homeSettings;

	// Token: 0x04000DCC RID: 3532
	public Text[] depthSettings;

	// Token: 0x04000DCD RID: 3533
	public Text[] targetShipData;

	// Token: 0x04000DCE RID: 3534
	public Color tubeTextColor;

	// Token: 0x04000DCF RID: 3535
	public Text ownShipData;

	// Token: 0x04000DD0 RID: 3536
	public Text contactData;

	// Token: 0x04000DD1 RID: 3537
	public Text contactDataName;

	// Token: 0x04000DD2 RID: 3538
	public Image contactDepth;

	// Token: 0x04000DD3 RID: 3539
	public Sprite[] contactDepthSprites;

	// Token: 0x04000DD4 RID: 3540
	public Text[] wireData;

	// Token: 0x04000DD5 RID: 3541
	public Text[] contextualPanelLabels;

	// Token: 0x04000DD6 RID: 3542
	public GameObject[] contextualPanels;

	// Token: 0x04000DD7 RID: 3543
	public Material[] signatureMaterials;

	// Token: 0x04000DD8 RID: 3544
	public float contactProfileMultiplier = 2f;

	// Token: 0x04000DD9 RID: 3545
	public Image[] torpedoTubeImages;

	// Token: 0x04000DDA RID: 3546
	public Sprite[] attackSettingSprites;

	// Token: 0x04000DDB RID: 3547
	public Sprite[] depthSettingSprites;

	// Token: 0x04000DDC RID: 3548
	public Sprite[] homeSettingSprites;

	// Token: 0x04000DDD RID: 3549
	public Sprite[] weaponSprites;

	// Token: 0x04000DDE RID: 3550
	public Sprite wireSprite;

	// Token: 0x04000DDF RID: 3551
	public Sprite tubeDestroyedSprite;

	// Token: 0x04000DE0 RID: 3552
	public Text[] storesColumns;

	// Token: 0x04000DE1 RID: 3553
	public Text[] storesLabels;

	// Token: 0x04000DE2 RID: 3554
	public GameObject esmGameObject;

	// Token: 0x04000DE3 RID: 3555
	public Transform hudTransfrom;

	// Token: 0x04000DE4 RID: 3556
	public GameObject torpedoTubeGUIObject;

	// Token: 0x04000DE5 RID: 3557
	public TorpedoTubeGUI[] torpedoTubesGUIs;

	// Token: 0x04000DE6 RID: 3558
	public Sprite[] torpedoTubeBackgrounds;

	// Token: 0x04000DE7 RID: 3559
	public GameObject messageLogPanel;

	// Token: 0x04000DE8 RID: 3560
	public GameObject storesPanel;

	// Token: 0x04000DE9 RID: 3561
	public GameObject conditionsPanel;

	// Token: 0x04000DEA RID: 3562
	public GameObject signaturePanel;

	// Token: 0x04000DEB RID: 3563
	public GameObject damagePanel;

	// Token: 0x04000DEC RID: 3564
	public int currentOpenPanel;

	// Token: 0x04000DED RID: 3565
	public Vector2 messageLogPositions;

	// Token: 0x04000DEE RID: 3566
	public Button[] tacMapMiniMapButtons;

	// Token: 0x04000DEF RID: 3567
	public Image tacMapMaximisedGraphic;

	// Token: 0x04000DF0 RID: 3568
	public Image tacMapMinimisedGraphic;

	// Token: 0x04000DF1 RID: 3569
	public CanvasScaler hudScaler;

	// Token: 0x04000DF2 RID: 3570
	public Text[] signatureData;

	// Token: 0x04000DF3 RID: 3571
	public Image[] signatureIcons;

	// Token: 0x04000DF4 RID: 3572
	public static bool hudInMenu;

	// Token: 0x04000DF5 RID: 3573
	public GameObject otherPanel;

	// Token: 0x04000DF6 RID: 3574
	public GameObject menuPanel;

	// Token: 0x04000DF7 RID: 3575
	public Vector2 menuPanelOffset;

	// Token: 0x04000DF8 RID: 3576
	public Slider esmIntensityDisplay;

	// Token: 0x04000DF9 RID: 3577
	public int timeCompressionToggle;

	// Token: 0x04000DFA RID: 3578
	public bool firstDepthCheckDone;

	// Token: 0x04000DFB RID: 3579
	public int playerDepthInFeet;

	// Token: 0x04000DFC RID: 3580
	public int playerDepthTier;

	// Token: 0x04000DFD RID: 3581
	public int playerDepthUnderKeel;

	// Token: 0x04000DFE RID: 3582
	public bool depthUnderKeelWarned;

	// Token: 0x04000DFF RID: 3583
	public bool iceWarned;

	// Token: 0x04000E00 RID: 3584
	public bool mineWarned;

	// Token: 0x04000E01 RID: 3585
	public int binocularZoomLevel;

	// Token: 0x04000E02 RID: 3586
	public Text binocularZoomText;

	// Token: 0x04000E03 RID: 3587
	public int missileAttackProfile;

	// Token: 0x04000E04 RID: 3588
	public int missileSeekerCone;

	// Token: 0x04000E05 RID: 3589
	public GameObject bearingLine;

	// Token: 0x04000E06 RID: 3590
	public GameObject bearingLineRangeMarker;

	// Token: 0x04000E07 RID: 3591
	public Text waypointDistanceText;

	// Token: 0x04000E08 RID: 3592
	public GameObject towedarraymanager;

	// Token: 0x04000E09 RID: 3593
	public bool towedArrayDeployed;

	// Token: 0x04000E0A RID: 3594
	public static bool draggingWaypoint;

	// Token: 0x04000E0B RID: 3595
	public float playerStatTimer;

	// Token: 0x04000E0C RID: 3596
	public float activeSonarPingRate = 10f;

	// Token: 0x04000E0D RID: 3597
	public float activeSonarPingTimer;

	// Token: 0x04000E0E RID: 3598
	public int[] otherVesselList;

	// Token: 0x04000E0F RID: 3599
	public int[] playerVesselList;

	// Token: 0x04000E10 RID: 3600
	public int playerVesselClass;

	// Token: 0x04000E11 RID: 3601
	public int playerVesselInstance;

	// Token: 0x04000E12 RID: 3602
	public static bool runningSilent;

	// Token: 0x04000E13 RID: 3603
	public Button runSilentButton;

	// Token: 0x04000E14 RID: 3604
	public LayerMask markTargetLayerMask;

	// Token: 0x04000E15 RID: 3605
	public Light periscopeNightVisionLight;

	// Token: 0x04000E16 RID: 3606
	public Texture nightVisionRamp;

	// Token: 0x04000E17 RID: 3607
	public int numberOfWires;

	// Token: 0x04000E18 RID: 3608
	public int numberOfWiresUsed;

	// Token: 0x04000E19 RID: 3609
	public string[] attackSettingDefinitions;

	// Token: 0x04000E1A RID: 3610
	public string[] depthSettingDefinitions;

	// Token: 0x04000E1B RID: 3611
	public string[] homeSettingDefinitions;

	// Token: 0x04000E1C RID: 3612
	public RectTransform[] meterRects;

	// Token: 0x04000E1D RID: 3613
	public Color[] meterColors;

	// Token: 0x04000E1E RID: 3614
	public Image meterUnitSprite;

	// Token: 0x04000E1F RID: 3615
	public Image[] noiseMeter;

	// Token: 0x04000E20 RID: 3616
	public Image[] wlr8Meter;

	// Token: 0x04000E21 RID: 3617
	public Image[] esmMeter;

	// Token: 0x04000E22 RID: 3618
	public Text wlr8BearingDisplay;

	// Token: 0x04000E23 RID: 3619
	public float esmcurrentBearing;

	// Token: 0x04000E24 RID: 3620
	public float esmTimer;

	// Token: 0x04000E25 RID: 3621
	public float wlr8currentBearing;

	// Token: 0x04000E26 RID: 3622
	public float wlr8Timer;

	// Token: 0x04000E27 RID: 3623
	public int esmcurrentIntensity;

	// Token: 0x04000E28 RID: 3624
	public int wlr8currentIntensity;

	// Token: 0x04000E29 RID: 3625
	public Image[] statusIcons;

	// Token: 0x04000E2A RID: 3626
	public GameObject statusIconsParent;

	// Token: 0x04000E2B RID: 3627
	public int[] statusIconOrder;

	// Token: 0x04000E2C RID: 3628
	public GameObject simpleConditions;

	// Token: 0x04000E2D RID: 3629
	public Text[] simpleConditionsText;

	// Token: 0x04000E2E RID: 3630
	public float inMissionZoneTimer;

	// Token: 0x04000E2F RID: 3631
	public AudioSource messageLogSounds;

	// Token: 0x04000E30 RID: 3632
	public bool hudHiddenWithMiniMapOpen;

	// Token: 0x04000E31 RID: 3633
	public int mastThresholdDepth = 50;

	// Token: 0x04000E32 RID: 3634
	public Text vlsLabel;

	// Token: 0x04000E33 RID: 3635
	public float ballastRechargeTime;

	// Token: 0x04000E34 RID: 3636
	public float ballastRechargeTimer;

	// Token: 0x04000E35 RID: 3637
	public int landAttackNumber;

	// Token: 0x04000E36 RID: 3638
	public bool hudHidden;

	// Token: 0x04000E37 RID: 3639
	public string contactSignatureName;

	// Token: 0x04000E38 RID: 3640
	public bool usingKnots;

	// Token: 0x04000E39 RID: 3641
	public float currentKnots;

	// Token: 0x04000E3A RID: 3642
	public int wantedTelegraph;

	// Token: 0x04000E3B RID: 3643
	public float telegraphDelayTimer;

	// Token: 0x04000E3C RID: 3644
	public bool generateFullLog;

	// Token: 0x04000E3D RID: 3645
	public List<string> fullMessageLog;

	// Token: 0x04000E3E RID: 3646
	public List<Color32> fullMessageLogColors;

	// Token: 0x04000E3F RID: 3647
	public Text fullMessageLogText;

	// Token: 0x04000E40 RID: 3648
	public GameObject logTextPrefab;

	// Token: 0x04000E41 RID: 3649
	public GameObject fullLogParentObject;

	// Token: 0x04000E42 RID: 3650
	public GameObject currentFullLogParentObject;

	// Token: 0x04000E43 RID: 3651
	public GameObject fullLogObject;

	// Token: 0x04000E44 RID: 3652
	public GameObject currentLogLineObject;

	// Token: 0x04000E45 RID: 3653
	public GameObject fullLogToggleButton;

	// Token: 0x04000E46 RID: 3654
	public int numberOfLogEntries;

	// Token: 0x04000E47 RID: 3655
	public ScrollRect fullLogScrollRect;

	// Token: 0x04000E48 RID: 3656
	public RectTransform hudCanvas;

	// Token: 0x04000E49 RID: 3657
	public string playerSunkBy;
}
