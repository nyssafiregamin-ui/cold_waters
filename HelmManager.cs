using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000125 RID: 293
public class HelmManager : MonoBehaviour
{
	// Token: 0x060007D8 RID: 2008 RVA: 0x0004B8A8 File Offset: 0x00049AA8
	public void InitialiseHelmManager()
	{
		this.DisplayCurrentTelegraph();
		this.depthTimer = this.depthTimerInterval;
		this.depthDelayTimer = -1f;
		this.damageControlDelayTimer = -1f;
		this.autoDiving = false;
		this.autoTurning = false;
		this.atSpeed = true;
		this.draggingNavWaypoint = false;
		this.navWaypointMarker.gameObject.SetActive(false);
		this.ClearHelmPanels();
		this.periscopePanel.SetActive(false);
		this.depthInterval = 50f;
		this.telegraphLimits = new Vector3(UIFunctions.globaluifunctions.playerfunctions.playerVessel.databaseshipdata.telegraphSpeeds[0] * 10f, UIFunctions.globaluifunctions.playerfunctions.playerVessel.databaseshipdata.telegraphSpeeds[6] * 10f);
		for (int i = 0; i < this.guiButtonImages.Length; i++)
		{
			this.guiButtonImages[i].color = this.buttonColors[0];
		}
		this.guiButtonImages[13].color = this.buttonColors[1];
		this.guiButtonImages[29].color = Color.white;
		UIFunctions.globaluifunctions.keybindManager.torpedoButtonSteer = "NONE";
		this.wantedSpeed = (float)Mathf.RoundToInt(this.playerfunctions.playerVessel.databaseshipdata.telegraphSpeeds[this.playerfunctions.playerVessel.vesselmovement.telegraphValue] * 10f);
		this.speedDisplayText.text = this.wantedSpeed.ToString();
		if (this.forceAllToolbarsOn)
		{
			for (int j = 0; j < this.toolbarTabs.Length; j++)
			{
				this.toolbarTabs[j].SetActive(!this.forceAllToolbarsOn);
			}
			this.helmPanel.SetActive(true);
			this.divePanel.SetActive(true);
			this.MastPanel.SetActive(true);
		}
	}

	// Token: 0x060007D9 RID: 2009 RVA: 0x0004BAB4 File Offset: 0x00049CB4
	public void SetHelmPanel(int value)
	{
		if (this.forceAllToolbarsOn)
		{
			return;
		}
		switch (value)
		{
		case 0:
			this.divePanel.SetActive(false);
			this.MastPanel.SetActive(false);
			this.helmPanel.SetActive(!this.helmPanel.activeSelf);
			break;
		case 1:
			this.helmPanel.SetActive(false);
			this.MastPanel.SetActive(false);
			this.divePanel.SetActive(!this.divePanel.activeSelf);
			break;
		case 2:
			this.helmPanel.SetActive(false);
			this.divePanel.SetActive(false);
			this.MastPanel.SetActive(!this.MastPanel.activeSelf);
			break;
		}
	}

	// Token: 0x060007DA RID: 2010 RVA: 0x0004BB88 File Offset: 0x00049D88
	public void SetPeriscopeMask(bool on)
	{
		UIFunctions.globaluifunctions.periscopeMask.SetActive(on);
	}

	// Token: 0x060007DB RID: 2011 RVA: 0x0004BB9C File Offset: 0x00049D9C
	public void ClearHelmPanels()
	{
		if (this.forceAllToolbarsOn)
		{
			return;
		}
		this.helmPanel.SetActive(false);
		this.divePanel.SetActive(false);
		this.MastPanel.SetActive(false);
	}

	// Token: 0x060007DC RID: 2012 RVA: 0x0004BBDC File Offset: 0x00049DDC
	public void SetDirectTelegraph(int value)
	{
		if (value == this.playerfunctions.playerVessel.vesselmovement.telegraphValue)
		{
			return;
		}
		if (!this.playerfunctions.damagecontrol.CheckSubsystem("REACTOR", false))
		{
			return;
		}
		if (PlayerFunctions.runningSilent && value > this.playerfunctions.damagecontrol.playerMaxTelegraph)
		{
			this.playerfunctions.LeaveRunningSilent();
		}
		if (value > this.playerfunctions.damagecontrol.playerMaxTelegraph)
		{
			return;
		}
		UIFunctions.globaluifunctions.playerfunctions.usingKnots = false;
		this.playerfunctions.playerVessel.vesselmovement.telegraphValue = value;
		this.playerfunctions.TelegraphMessage();
		this.DisplayCurrentTelegraph();
		this.atSpeed = false;
	}

	// Token: 0x060007DD RID: 2013 RVA: 0x0004BCA4 File Offset: 0x00049EA4
	public void DisplayCurrentTelegraph()
	{
		this.telegraphIndicator.transform.position = this.telegraphTexts[this.playerfunctions.playerVessel.vesselmovement.telegraphValue].transform.position;
	}

	// Token: 0x060007DE RID: 2014 RVA: 0x0004BCE8 File Offset: 0x00049EE8
	public void SetDepthPointerDown(int value)
	{
		if (UIFunctions.globaluifunctions.playerfunctions.playerVessel.vesselmovement.blowBallast)
		{
			UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "BallastRecharging"), UIFunctions.globaluifunctions.playerfunctions.messageLogColors["Helm"], "BallastRecharging", true);
			return;
		}
		this.depthDirection = (float)value;
		this.autoDiving = false;
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x0004BD60 File Offset: 0x00049F60
	public void SetDepthPointerUp()
	{
		if (UIFunctions.globaluifunctions.playerfunctions.playerVessel.vesselmovement.blowBallast)
		{
			return;
		}
		this.depthDirection = 0f;
		this.depthTimer = this.depthTimerInterval;
		this.depthDelayTimer = 1f;
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x0004BDB0 File Offset: 0x00049FB0
	public void SetSpeedPointerDown(int value)
	{
		if (!this.playerfunctions.damagecontrol.CheckSubsystem("REACTOR", false))
		{
			return;
		}
		this.speedDirection = (float)value;
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x0004BDE4 File Offset: 0x00049FE4
	public void SetSpeedPointerUp()
	{
		this.speedDirection = 0f;
		this.speedTimer = this.speedTimerInterval;
		this.speedDelayTimer = 1f;
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x0004BE14 File Offset: 0x0004A014
	public void SetFixedDepth(float value)
	{
		this.wantedDepth = value;
		this.wantedDepth = Mathf.Clamp(this.wantedDepth, 0f, 3000f);
		this.depthDisplayText.text = this.wantedDepth.ToString();
		UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentWantedDepth = this.depthDisplayText.text;
		this.autoDiving = true;
		string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "NavigationSetDepth");
		text = text.Replace("<WANTEDDEPTH>", this.wantedDepth.ToString());
		text = text.Replace("<FEET>", LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Feet").ToLower());
		this.playerfunctions.PlayerMessage(text, this.playerfunctions.messageLogColors["Helm"], "NavigationSetDepth", true);
		UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("depth", true);
	}

	// Token: 0x060007E3 RID: 2019 RVA: 0x0004BF04 File Offset: 0x0004A104
	public void SetPeriscopeDepth()
	{
		this.wantedDepth = (float)this.playerfunctions.playerVessel.databaseshipdata.periscopeDepthInFeet;
		this.wantedDepth = Mathf.Clamp(this.wantedDepth, 0f, 3000f);
		this.depthDisplayText.text = this.wantedDepth.ToString();
		UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentWantedDepth = this.depthDisplayText.text;
		this.autoDiving = true;
		string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "NavigationSetDepth");
		text = text.Replace("<WANTEDDEPTH>", this.wantedDepth.ToString());
		text = text.Replace("<FEET>", LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Feet").ToLower());
		this.playerfunctions.PlayerMessage(text, this.playerfunctions.messageLogColors["Helm"], "NavigationSetDepth", true);
		UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("depth", true);
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x0004C008 File Offset: 0x0004A208
	private void Update()
	{
		if (this.currentTooltip != string.Empty && GameDataManager.optionsBoolSettings[19])
		{
			Vector3 mousePosition = Input.mousePosition;
			mousePosition.z = 2f;
			this.tooltipOffset = new Vector3(0f, -7f, 0f);
			if (mousePosition.x / (float)Screen.width > 0.5f)
			{
				this.toolTipText.alignment = TextAnchor.MiddleRight;
			}
			else
			{
				this.toolTipText.alignment = TextAnchor.MiddleLeft;
			}
			if (mousePosition.y / (float)Screen.height < 0.5f)
			{
				this.tooltipOffset.y = 4f;
			}
			this.toolTipText.transform.position = UIFunctions.globaluifunctions.HUDCameraObject.ScreenToWorldPoint(mousePosition) + this.tooltipOffset;
			this.tooltipTimer += Time.deltaTime;
			if (this.tooltipTimer > this.tooltipDelay)
			{
				this.DisplayToolTip();
			}
		}
		if (this.draggingNavWaypoint)
		{
			this.DraggingWaypointUpdate();
			this.playerfunctions.sensormanager.tacticalmap.waypointWorldMarker.transform.position = new Vector3(this.navWaypointMarker.transform.localPosition.x / this.playerfunctions.sensormanager.tacticalmap.zoomFactor, 1000f, this.navWaypointMarker.transform.localPosition.y / this.playerfunctions.sensormanager.tacticalmap.zoomFactor);
			string[] waypointDetails = this.playerfunctions.sensormanager.tacticalmap.GetWaypointDetails();
			string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "NavigationWaypointInfo");
			text = text.Replace("<BRG>", waypointDetails[0]);
			text = text.Replace("<RANGE>", waypointDetails[1]);
			text = text.Replace("<KILOYARD>", LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "KiloYard"));
			this.playerfunctions.sensormanager.tacticalmap.waypointReadout.text = text;
			if (InputManager.globalInputManager.GetButtonDown("Set Waypoint", false) && (TacticalMap.tacMapEnabled || UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.IsMouseOverTacMiniMap()))
			{
				this.playerfunctions.sensormanager.tacticalmap.waypointReadout.text = string.Empty;
				this.DisableWaypointDragging();
				this.wantedCourse = (float)Mathf.RoundToInt(float.Parse(waypointDetails[0]));
				this.autoTurning = true;
				string text2 = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "NavigationSetCourse");
				text2 = text2.Replace("<BRG>", waypointDetails[0]);
				float localDirection = this.GetLocalDirection();
				string dictionaryString = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Right");
				UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentLeftOrRight = "HELMRIGHT";
				if (localDirection < 0f)
				{
					dictionaryString = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Left");
					UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentLeftOrRight = "HELMLEFT";
				}
				text2 = text2.Replace("<DIRECTION>", dictionaryString);
				UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentCourse = waypointDetails[0];
				this.playerfunctions.PlayerMessage(text2, this.playerfunctions.messageLogColors["Helm"], "NavigationSetCourse", true);
				UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("course", true);
			}
		}
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x0004C38C File Offset: 0x0004A58C
	private void SetFinalSpeed()
	{
		UIFunctions.globaluifunctions.playerfunctions.usingKnots = true;
		UIFunctions.globaluifunctions.playerfunctions.currentKnots = this.wantedSpeed / 10f;
		int num;
		if (UIFunctions.globaluifunctions.playerfunctions.currentKnots < UIFunctions.globaluifunctions.playerfunctions.playerVessel.databaseshipdata.telegraphSpeeds[1])
		{
			num = 0;
		}
		else if (UIFunctions.globaluifunctions.playerfunctions.currentKnots == UIFunctions.globaluifunctions.playerfunctions.playerVessel.databaseshipdata.telegraphSpeeds[1])
		{
			num = 1;
		}
		else if (UIFunctions.globaluifunctions.playerfunctions.currentKnots <= UIFunctions.globaluifunctions.playerfunctions.playerVessel.databaseshipdata.telegraphSpeeds[2])
		{
			num = 2;
		}
		else if (UIFunctions.globaluifunctions.playerfunctions.currentKnots <= UIFunctions.globaluifunctions.playerfunctions.playerVessel.databaseshipdata.telegraphSpeeds[3])
		{
			num = 3;
		}
		else if (UIFunctions.globaluifunctions.playerfunctions.currentKnots <= UIFunctions.globaluifunctions.playerfunctions.playerVessel.databaseshipdata.telegraphSpeeds[4])
		{
			num = 4;
		}
		else if (UIFunctions.globaluifunctions.playerfunctions.currentKnots <= UIFunctions.globaluifunctions.playerfunctions.playerVessel.databaseshipdata.telegraphSpeeds[5])
		{
			num = 5;
		}
		else
		{
			num = 6;
		}
		if (PlayerFunctions.runningSilent && num > UIFunctions.globaluifunctions.playerfunctions.damagecontrol.playerMaxTelegraph)
		{
			UIFunctions.globaluifunctions.playerfunctions.LeaveRunningSilent();
		}
		if (num > UIFunctions.globaluifunctions.playerfunctions.damagecontrol.playerMaxTelegraph)
		{
			num = UIFunctions.globaluifunctions.playerfunctions.damagecontrol.playerMaxTelegraph;
		}
		UIFunctions.globaluifunctions.playerfunctions.playerVessel.vesselmovement.telegraphValue = num;
		UIFunctions.globaluifunctions.playerfunctions.TelegraphMessage();
		this.DisplayCurrentTelegraph();
		this.atSpeed = false;
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x0004C5A4 File Offset: 0x0004A7A4
	public void SetToolTip(string tooltipID)
	{
		this.currentTooltip = tooltipID;
		this.tooltipTimer = 0f;
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x0004C5B8 File Offset: 0x0004A7B8
	public void ClearToolTip()
	{
		this.currentTooltip = string.Empty;
		this.toolTipText.text = this.currentTooltip;
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x0004C5D8 File Offset: 0x0004A7D8
	private void DisplayToolTip()
	{
		string text = string.Empty;
		if (!this.currentTooltip.Contains("|"))
		{
			if (!LanguageManager.interfaceDictionary.ContainsKey(this.currentTooltip))
			{
				this.toolTipText.text = "NOT FOUND";
				return;
			}
			text = LanguageManager.interfaceDictionary[this.currentTooltip] + "  ( ";
			string currentLine = "<KEY:" + this.currentTooltip + ">";
			text = text + UIFunctions.globaluifunctions.helpmanager.PopulateHelpTags(currentLine) + " )";
			text = text.Replace("maroon", "lime");
		}
		else
		{
			string[] array = this.currentTooltip.Split(new char[]
			{
				'|'
			});
			bool flag = false;
			for (int i = 0; i < array.Length; i++)
			{
				if (LanguageManager.interfaceDictionary.ContainsKey(array[i]))
				{
					text = text + LanguageManager.interfaceDictionary[array[i]] + "  ";
					string currentLine2 = "<KEY:" + array[i] + ">";
					text = text + "( " + UIFunctions.globaluifunctions.helpmanager.PopulateHelpTags(currentLine2) + " )\n";
					text = text.Replace("maroon", "lime");
					flag = true;
				}
			}
			if (!flag)
			{
				this.toolTipText.text = "NOT FOUND";
				return;
			}
		}
		this.toolTipText.text = text;
	}

	// Token: 0x060007E9 RID: 2025 RVA: 0x0004C75C File Offset: 0x0004A95C
	private void FixedUpdate()
	{
		float num = Time.timeScale;
		if (num <= 0f)
		{
			num = 1f;
		}
		if (!this.atSpeed && Mathf.Abs(this.wantedSpeed - GameDataManager.playervesselsonlevel[0].vesselmovement.shipSpeed.z * 10f) < 0.25f)
		{
			string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "AtSpeed");
			text = text.Replace("<SPEED>", this.wantedSpeed.ToString());
			this.playerfunctions.PlayerMessage(text, this.playerfunctions.messageLogColors["Maneuver"], "AtSpeed", true);
			this.atSpeed = true;
		}
		if (this.playerfunctions.telegraphDelayTimer > 0f)
		{
			this.playerfunctions.telegraphDelayTimer -= Time.deltaTime / num;
		}
		if (this.depthDelayTimer > 0f)
		{
			this.depthDelayTimer -= Time.deltaTime / num;
			if (this.depthDelayTimer <= 0f)
			{
				this.depthDelayTimer = -1f;
				this.SetFixedDepth(this.wantedDepth);
			}
		}
		if (this.speedDelayTimer > 0f)
		{
			this.speedDelayTimer -= Time.deltaTime / num;
			if (this.speedDelayTimer <= 0f)
			{
				this.speedDelayTimer = -1f;
				this.SetFinalSpeed();
			}
		}
		if (this.telegraphDelayTimer > 0f)
		{
			this.telegraphDelayTimer -= Time.deltaTime / num;
			if (this.telegraphDelayTimer <= 0f)
			{
				this.telegraphDelayTimer = -1f;
				this.playerfunctions.TelegraphMessage();
			}
		}
		if (this.damageControlDelayTimer > 0f)
		{
			this.damageControlDelayTimer -= Time.deltaTime / num;
			if (this.damageControlDelayTimer <= 0f)
			{
				this.damageControlDelayTimer = -1f;
				this.playerfunctions.damagecontrol.CheckDamageControlPartyMessage();
			}
		}
		if (this.depthDirection != 0f)
		{
			this.depthTimer += Time.deltaTime / num;
			if (this.depthTimer > this.depthTimerInterval)
			{
				this.wantedDepth += this.depthInterval * this.depthDirection;
				this.wantedDepth /= this.depthInterval;
				this.wantedDepth = (float)Mathf.RoundToInt(this.wantedDepth) * this.depthInterval;
				this.depthTimer = 0f;
				this.wantedDepth = Mathf.Clamp(this.wantedDepth, 0f, 3000f);
				this.depthDisplayText.text = this.wantedDepth.ToString();
			}
		}
		if (this.speedDirection != 0f)
		{
			this.speedTimer += Time.deltaTime / num;
			if (this.speedTimer > this.speedTimerInterval)
			{
				this.wantedSpeed += 1f * this.speedDirection;
				this.wantedSpeed = (float)Mathf.RoundToInt(this.wantedSpeed);
				this.speedTimer = 0f;
				float max = UIFunctions.globaluifunctions.playerfunctions.playerVessel.databaseshipdata.telegraphSpeeds[UIFunctions.globaluifunctions.playerfunctions.damagecontrol.playerMaxTelegraph] * 10f;
				this.wantedSpeed = Mathf.Clamp(this.wantedSpeed, this.telegraphLimits.x, max);
				this.speedDisplayText.text = this.wantedSpeed.ToString();
			}
		}
		if (this.autoTurning)
		{
			float num2 = this.GetLocalDirection();
			num2 /= 3f;
			num2 = Mathf.Clamp(num2, -3f, 3f);
			this.playerfunctions.playerVessel.vesselmovement.rudderAngle.x = num2;
			if (Mathf.Abs(num2) < 0.05f)
			{
				string dictionaryString = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "NavigationAtCourse");
				this.playerfunctions.PlayerMessage(dictionaryString, this.playerfunctions.messageLogColors["Helm"], "NavigationAtCourse", true);
				this.CancelAutoTurning();
				this.playerfunctions.playerVessel.vesselmovement.rudderAngle.x = 0f;
			}
		}
		if (this.autoDiving)
		{
			if (this.playerfunctions.playerVessel.vesselmovement.blowBallast)
			{
				this.CancelAutoDiving();
				return;
			}
			float num3 = this.wantedDepth - (float)this.playerfunctions.playerDepthInFeet;
			float num4 = Mathf.Abs(num3);
			if (num4 < 0.5f && (this.playerfunctions.playerVessel.transform.eulerAngles.x < 0.5f || this.playerfunctions.playerVessel.transform.eulerAngles.x > 359.5f))
			{
				string text2 = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "NavigationAtDepth");
				text2 = text2.Replace("<WANTEDDEPTH>", this.wantedDepth.ToString());
				text2 = text2.Replace("<FEET>", LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Feet").ToLower());
				this.playerfunctions.PlayerMessage(text2, this.playerfunctions.messageLogColors["Helm"], "NavigationAtDepth", true);
				this.CancelAutoDiving();
				this.playerfunctions.playerVessel.vesselmovement.diveAngle.x = 0f;
				this.playerfunctions.playerVessel.vesselmovement.ballastAngle.x = 0f;
				return;
			}
			float num5 = 3f;
			if (num4 < 50f)
			{
				num5 = 1f;
				if (this.playerfunctions.playerVessel.vesselmovement.shipSpeed.z < 0.5f)
				{
					num5 = 2f;
				}
			}
			float num6 = num3 / -5f;
			num3 /= 300f;
			num3 = Mathf.Clamp(num3, -num5, num5);
			num6 = Mathf.Clamp(num6, -num5, num5);
			if (num6 < 0f && num6 > -0.5f)
			{
				num6 = -0.5f;
			}
			else if (num6 > 0f && num6 < 0.5f)
			{
				num6 = 0.5f;
			}
			this.playerfunctions.playerVessel.vesselmovement.diveAngle.x = num3;
			this.playerfunctions.playerVessel.vesselmovement.ballastAngle.x = num6;
		}
	}

	// Token: 0x060007EA RID: 2026 RVA: 0x0004CDF8 File Offset: 0x0004AFF8
	private float GetLocalDirection()
	{
		this.playerfunctions.playerVessel.acoustics.sensorNavigator.transform.rotation = Quaternion.Euler(0f, this.wantedCourse, 0f);
		float num = this.playerfunctions.playerVessel.acoustics.sensorNavigator.transform.localEulerAngles.y;
		if (num > 180f)
		{
			num -= 360f;
		}
		return num;
	}

	// Token: 0x060007EB RID: 2027 RVA: 0x0004CE78 File Offset: 0x0004B078
	public void SetNavWaypoint()
	{
		if (this.draggingNavWaypoint || PlayerFunctions.draggingWaypoint)
		{
			this.playerfunctions.sensormanager.tacticalmap.waypointReadout.text = string.Empty;
			this.DisableWaypointDragging();
			this.playerfunctions.DisableWaypointDragging();
			return;
		}
		if (!this.playerfunctions.tacMapMaximisedGraphic.enabled && !this.playerfunctions.sensormanager.tacticalmap.background.activeSelf)
		{
			this.playerfunctions.HUDTacMap();
		}
		this.draggingNavWaypoint = true;
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.waypointReadout.color = UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.waypointReadoutColor;
	}

	// Token: 0x060007EC RID: 2028 RVA: 0x0004CF48 File Offset: 0x0004B148
	private void DraggingWaypointUpdate()
	{
		if (TacticalMap.tacMapEnabled)
		{
			this.navWaypointMarker.gameObject.SetActive(true);
			Vector2 v;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.playerfunctions.sensormanager.tacticalmap.tacmapCanvas.transform as RectTransform, Input.mousePosition, this.playerfunctions.sensormanager.tacticalmap.tacmapCanvas.worldCamera, out v);
			this.navWaypointMarker.transform.position = this.playerfunctions.sensormanager.tacticalmap.tacmapCanvas.transform.TransformPoint(v);
			this.navWaypointLine.SetPosition(1, this.playerfunctions.sensormanager.tacticalmap.playerMapContact.transform.position - this.navWaypointMarker.transform.position);
		}
	}

	// Token: 0x060007ED RID: 2029 RVA: 0x0004D030 File Offset: 0x0004B230
	public void DisableWaypointDragging()
	{
		this.playerfunctions.sensormanager.tacticalmap.waypointReadout.text = string.Empty;
		PlayerFunctions.draggingWaypoint = false;
		this.navWaypointMarker.gameObject.SetActive(false);
		this.draggingNavWaypoint = false;
	}

	// Token: 0x060007EE RID: 2030 RVA: 0x0004D070 File Offset: 0x0004B270
	public void CancelAutoTurning()
	{
		this.autoTurning = false;
		UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("course", false);
	}

	// Token: 0x060007EF RID: 2031 RVA: 0x0004D090 File Offset: 0x0004B290
	public void CancelAutoDiving()
	{
		this.autoDiving = false;
		UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("depth", false);
	}

	// Token: 0x060007F0 RID: 2032 RVA: 0x0004D0B0 File Offset: 0x0004B2B0
	public void ButtonTorpedoSteer(string dir)
	{
		UIFunctions.globaluifunctions.keybindManager.torpedoButtonSteer = dir;
	}

	// Token: 0x060007F1 RID: 2033 RVA: 0x0004D0C4 File Offset: 0x0004B2C4
	public void ClearButtonTorpedoSteer()
	{
		UIFunctions.globaluifunctions.keybindManager.torpedoButtonSteer = "NONE";
	}

	// Token: 0x04000B6C RID: 2924
	public PlayerFunctions playerfunctions;

	// Token: 0x04000B6D RID: 2925
	public bool autoDiving;

	// Token: 0x04000B6E RID: 2926
	public bool autoTurning;

	// Token: 0x04000B6F RID: 2927
	public bool atSpeed;

	// Token: 0x04000B70 RID: 2928
	public float wantedDepth;

	// Token: 0x04000B71 RID: 2929
	public float depthDirection;

	// Token: 0x04000B72 RID: 2930
	public float wantedSpeed;

	// Token: 0x04000B73 RID: 2931
	public float speedDirection;

	// Token: 0x04000B74 RID: 2932
	public float wantedCourse;

	// Token: 0x04000B75 RID: 2933
	public bool forceAllToolbarsOn;

	// Token: 0x04000B76 RID: 2934
	public Text[] telegraphTexts;

	// Token: 0x04000B77 RID: 2935
	public GameObject telegraphIndicator;

	// Token: 0x04000B78 RID: 2936
	public Image[] toolbarImages;

	// Token: 0x04000B79 RID: 2937
	public Text depthDisplayText;

	// Token: 0x04000B7A RID: 2938
	public Text speedDisplayText;

	// Token: 0x04000B7B RID: 2939
	public float depthTimer;

	// Token: 0x04000B7C RID: 2940
	public float depthDelayTimer;

	// Token: 0x04000B7D RID: 2941
	public float telegraphDelayTimer;

	// Token: 0x04000B7E RID: 2942
	public float speedTimer;

	// Token: 0x04000B7F RID: 2943
	public float speedDelayTimer;

	// Token: 0x04000B80 RID: 2944
	public float damageControlDelayTimer;

	// Token: 0x04000B81 RID: 2945
	public float depthTimerInterval;

	// Token: 0x04000B82 RID: 2946
	public float speedTimerInterval;

	// Token: 0x04000B83 RID: 2947
	public Vector2 telegraphLimits;

	// Token: 0x04000B84 RID: 2948
	public bool draggingNavWaypoint;

	// Token: 0x04000B85 RID: 2949
	public RectTransform navWaypointMarker;

	// Token: 0x04000B86 RID: 2950
	public LineRenderer navWaypointLine;

	// Token: 0x04000B87 RID: 2951
	public float depthInterval = 50f;

	// Token: 0x04000B88 RID: 2952
	public GameObject helmPanel;

	// Token: 0x04000B89 RID: 2953
	public GameObject divePanel;

	// Token: 0x04000B8A RID: 2954
	public GameObject MastPanel;

	// Token: 0x04000B8B RID: 2955
	public GameObject periscopePanel;

	// Token: 0x04000B8C RID: 2956
	public Color32[] buttonColors;

	// Token: 0x04000B8D RID: 2957
	public Image[] guiButtonImages;

	// Token: 0x04000B8E RID: 2958
	public string currentTooltip;

	// Token: 0x04000B8F RID: 2959
	public Vector2 lastMousePosition;

	// Token: 0x04000B90 RID: 2960
	public float tooltipTimer;

	// Token: 0x04000B91 RID: 2961
	public float tooltipDelay;

	// Token: 0x04000B92 RID: 2962
	public Text toolTipText;

	// Token: 0x04000B93 RID: 2963
	public Vector3 tooltipOffset;

	// Token: 0x04000B94 RID: 2964
	public Image[] storesBackgrounds;

	// Token: 0x04000B95 RID: 2965
	public Transform helmmanagerAnchor;

	// Token: 0x04000B96 RID: 2966
	public Transform helmToolbars;

	// Token: 0x04000B97 RID: 2967
	public GameObject[] toolbarTabs;
}
