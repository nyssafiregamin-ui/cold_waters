using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000101 RID: 257
public class DamageControl : MonoBehaviour
{
	// Token: 0x06000704 RID: 1796 RVA: 0x0003D108 File Offset: 0x0003B308
	public void InitialisePlayerDamageControl()
	{
		this.playervessel = GameDataManager.playervesselsonlevel[0];
		this.damageControlCurrentTimers = new float[UIFunctions.globaluifunctions.database.databasesubsystemsdata.Length];
		this.subsystemOffline = new bool[UIFunctions.globaluifunctions.database.databasesubsystemsdata.Length];
		for (int i = 0; i < this.damageControlCurrentTimers.Length; i++)
		{
			this.statusscreens.timers[i].text = string.Empty;
		}
		base.enabled = true;
		this.compartmentCurrentFlooding = new float[5];
		this.compartmentTotalFlooding = new float[5];
		this.BuildDamageControlPanel();
		this.ResetFloodingIndicators();
		this.currentSubsystem = DamageControl.GetSubsystemIndex("PUMPS");
		this.BoldText(this.currentSubsystem);
		this.GetSubsystemCompartment();
		this.playerMaxTelegraph = 6;
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x0003D1E0 File Offset: 0x0003B3E0
	public void ResetFloodingIndicators()
	{
		this.waterRanges = new float[this.compartmentWaterLevels.Length];
		for (int i = 0; i < this.compartmentWaterLevels.Length; i++)
		{
			this.compartmentWaterLevels[i].transform.localPosition = new Vector3(this.playervessel.databaseshipdata.compartmentPositionsAndWidth[i].x - 258.5f, this.playervessel.databaseshipdata.compartmentFloodingRanges[i].x + 137.5f, this.compartmentWaterLevels[i].transform.localPosition.z);
			this.compartmentWaterLevels[i].sizeDelta = new Vector2(this.playervessel.databaseshipdata.compartmentPositionsAndWidth[i].y, this.compartmentWaterLevels[i].sizeDelta.y);
			this.waterRanges[i] = this.playervessel.databaseshipdata.compartmentFloodingRanges[i].y - this.playervessel.databaseshipdata.compartmentFloodingRanges[i].x;
		}
		this.pumpCompartment.transform.localPosition = new Vector3(this.pumpCompartment.transform.localPosition.x, this.playervessel.databaseshipdata.damageControlPartyY + 137.5f, this.pumpCompartment.transform.localPosition.z);
	}

	// Token: 0x06000706 RID: 1798 RVA: 0x0003D36C File Offset: 0x0003B56C
	public bool IsTooFlooded()
	{
		float num = 0f;
		for (int i = 0; i < UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentCurrentFlooding.Length; i++)
		{
			if (UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentCurrentFlooding[i] > 0.1f)
			{
				return true;
			}
			num += UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentCurrentFlooding[i];
		}
		return num > 0.2f;
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x0003D3F0 File Offset: 0x0003B5F0
	private void BuildDamageControlPanel()
	{
		for (int i = 0; i < UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlLabels.Length; i++)
		{
			UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlLabels[i].transform.localPosition = Vector3.zero;
			UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlLabels[i].gameObject.SetActive(false);
		}
		for (int j = 0; j < GameDataManager.playervesselsonlevel[0].databaseshipdata.subsystemLabelPositions.Length; j++)
		{
			UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlLabels[j].transform.localPosition = GameDataManager.playervesselsonlevel[0].databaseshipdata.subsystemLabelPositions[j];
			if (UIFunctions.globaluifunctions.playerfunctions.playerVessel.databaseshipdata.subsystemPrimaryPositions[j] != "FALSE")
			{
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlLabels[j].text = UIFunctions.globaluifunctions.database.databasesubsystemsdata[j].subsystemName;
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlLabels[j].gameObject.SetActive(true);
			}
			else
			{
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlLabels[j].text = string.Empty;
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlLabels[j].gameObject.SetActive(false);
			}
		}
		for (int k = 0; k < this.damageControlSliders.Length; k++)
		{
			this.damageControlSliders[k].value = 0f;
		}
	}

	// Token: 0x06000708 RID: 1800 RVA: 0x0003D5BC File Offset: 0x0003B7BC
	public void BoldText(int index)
	{
		string text = this.damageControlLabels[this.currentSubsystem].text;
		text = text.Replace("<b>", string.Empty);
		text = text.Replace("</b>", string.Empty);
		this.damageControlLabels[this.currentSubsystem].text = text;
		this.damageControlLabels[index].text = "<b>" + this.damageControlLabels[index].text + "</b>";
		this.currentSubsystem = index;
	}

	// Token: 0x06000709 RID: 1801 RVA: 0x0003D644 File Offset: 0x0003B844
	public void SelectDamageControlSubsystem(string subsystem)
	{
		if (PlayerFunctions.runningSilent)
		{
			UIFunctions.globaluifunctions.playerfunctions.LeaveRunningSilent();
		}
		int subsystemIndex = DamageControl.GetSubsystemIndex(subsystem);
		bool flag = false;
		if (subsystemIndex == this.currentSubsystem)
		{
			flag = true;
		}
		this.BoldText(subsystemIndex);
		if (UIFunctions.globaluifunctions.campaignmanager.playerInPort)
		{
			this.GetTimeToRepair();
			if (flag)
			{
				this.RepairSubsystem();
				UIFunctions.globaluifunctions.campaignmanager.PortTopMenu();
			}
		}
		else
		{
			UIFunctions.globaluifunctions.playerfunctions.helmmanager.damageControlDelayTimer = 1f;
		}
		this.currentCompartment = this.GetSubsystemCompartment();
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x0003D6E8 File Offset: 0x0003B8E8
	public void CheckDamageControlPartyMessage()
	{
		int subsystemCompartment = this.GetSubsystemCompartment();
		if (subsystemCompartment.ToString() != UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentDCCompartment)
		{
			string text = LanguageManager.messageLogDictionary["DCPartyToCompartment"];
			string[] array = new string[]
			{
				"TorpedoRoom",
				"ControlRoom",
				"ReactorSpace",
				"EngineRoom",
				"MachinerySpace"
			};
			text = text.Replace("<COMPARTMENT>", LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, array[subsystemCompartment]));
			UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentDCCompartment = subsystemCompartment.ToString();
			UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["XO"], "DCPartyToCompartment", true);
		}
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x0003D7C4 File Offset: 0x0003B9C4
	public int GetSubsystemCompartment()
	{
		string text = this.playervessel.databaseshipdata.subsystemPrimaryPositions[this.currentSubsystem];
		int num = 2;
		string text2 = text;
		switch (text2)
		{
		case "FRONT":
			num = 0;
			break;
		case "FORE":
			num = 1;
			break;
		case "AFT":
			num = 3;
			break;
		case "REAR":
			num = 4;
			break;
		case "SAIL":
			num = -1;
			break;
		}
		if (num != -1)
		{
			this.pumpCompartment.transform.localPosition = new Vector3(this.playervessel.databaseshipdata.compartmentPositionsAndWidth[num].x - 258.5f, this.pumpCompartment.transform.localPosition.y, this.pumpCompartment.transform.localPosition.z);
			this.currentCompartment = num;
			return num;
		}
		return 0;
	}

	// Token: 0x0600070C RID: 1804 RVA: 0x0003D914 File Offset: 0x0003BB14
	public bool CheckSubsystem(string subsystem, bool messagePlayer)
	{
		bool result = true;
		int subsystemIndex = DamageControl.GetSubsystemIndex(subsystem);
		if (this.subsystemOffline[subsystemIndex])
		{
			result = false;
			if (messagePlayer)
			{
				if (this.damageControlCurrentTimers[subsystemIndex] != 10000f)
				{
					UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentSubsystem = UIFunctions.globaluifunctions.database.databasesubsystemsdata[subsystemIndex].subsystem;
					string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "SubsystemDamaged");
					text = text.Replace("<SUBSYSTEM>", UIFunctions.globaluifunctions.database.databasesubsystemsdata[subsystemIndex].subsystemName);
					text = text.Replace("<TIMETOREPAIR>", string.Format("{0:0}", this.damageControlCurrentTimers[subsystemIndex] / 60f));
					UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["XO"], "SubsystemDamaged", true);
					if (UIFunctions.globaluifunctions.playerfunctions.currentOpenPanel == 2)
					{
						UIFunctions.globaluifunctions.playerfunctions.SetDamageControlLabels();
					}
				}
				else
				{
					UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentSubsystem = UIFunctions.globaluifunctions.database.databasesubsystemsdata[subsystemIndex].subsystem;
					string text2 = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "SubsystemDestroyed");
					text2 = text2.Replace("<SUBSYSTEM>", UIFunctions.globaluifunctions.database.databasesubsystemsdata[subsystemIndex].subsystemName);
					UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text2, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["XO"], "SubsystemDestroyed", true);
					if (UIFunctions.globaluifunctions.playerfunctions.currentOpenPanel == 2)
					{
						UIFunctions.globaluifunctions.playerfunctions.SetDamageControlLabels();
					}
				}
			}
		}
		return result;
	}

	// Token: 0x0600070D RID: 1805 RVA: 0x0003DAE0 File Offset: 0x0003BCE0
	public void KnockoutSubsystem(string subsystem, bool destroy)
	{
		int subsystemIndex = DamageControl.GetSubsystemIndex(subsystem);
		if (this.damageControlCurrentTimers[subsystemIndex] == 10000f && subsystem != "TUBES")
		{
			return;
		}
		switch (subsystem)
		{
		case "PERISCOPE":
			if (ManualCameraZoom.binoculars)
			{
				this.uifunctions.CancelZoom();
			}
			UIFunctions.globaluifunctions.playerfunctions.playerVessel.submarineFunctions.ClearDetectionTypes(1);
			UIFunctions.globaluifunctions.playerfunctions.helmmanager.periscopePanel.SetActive(false);
			break;
		case "ESM_MAST":
			UIFunctions.globaluifunctions.playerfunctions.DisableESMMeter();
			UIFunctions.globaluifunctions.playerfunctions.playerVessel.submarineFunctions.ClearDetectionTypes(3);
			break;
		case "RADAR_MAST":
			UIFunctions.globaluifunctions.playerfunctions.playerVessel.submarineFunctions.ClearDetectionTypes(2);
			break;
		case "TUBES":
		{
			int num2 = UnityEngine.Random.Range(0, this.playervessel.vesselmovement.weaponSource.torpedoTubes.Length);
			if (this.playervessel.vesselmovement.weaponSource.tubeStatus[num2] != -200)
			{
				if (this.playervessel.vesselmovement.weaponSource.weaponInTube[num2] > -1 || this.playervessel.vesselmovement.weaponSource.tubeReloadingTimer[num2] > 0.1f)
				{
					this.playervessel.vesselmovement.weaponSource.currentTorpsOnBoard[this.playervessel.vesselmovement.weaponSource.tubeStatus[num2]]--;
					if (UIFunctions.globaluifunctions.playerfunctions.storesPanel.activeSelf)
					{
						UIFunctions.globaluifunctions.portRearm.SetLoadoutStats();
					}
				}
				this.uifunctions.playerfunctions.CutWire(num2);
				this.playervessel.vesselmovement.weaponSource.tubeStatus[num2] = -200;
				this.playervessel.vesselmovement.weaponSource.weaponInTube[num2] = -200;
				this.damageControlCurrentTimers[subsystemIndex] = 10000f;
				this.damageControlSliders[subsystemIndex].value = 0f;
				UIFunctions.globaluifunctions.playerfunctions.ClearTubeSettingButtons(num2);
				this.playervessel.vesselmovement.weaponSource.tubeReloadingDirection[num2] = 0f;
				UIFunctions.globaluifunctions.playerfunctions.torpedoTubeImages[num2].transform.localPosition = new Vector3(0f, -0.5f, 0f);
				UIFunctions.globaluifunctions.playerfunctions.torpedoTubeImages[num2].gameObject.SetActive(true);
				UIFunctions.globaluifunctions.playerfunctions.torpedoTubeImages[num2].sprite = UIFunctions.globaluifunctions.playerfunctions.tubeDestroyedSprite;
				string text = LanguageManager.messageLogDictionary["TubeDestroyed"];
				text = text.Replace("<TUBE>", (num2 + 1).ToString());
				UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["XO"], "TubeDestroyed", false);
				if (UIFunctions.globaluifunctions.playerfunctions.timeCompressionToggle == 1)
				{
					UIFunctions.globaluifunctions.playerfunctions.SetTimeCompression();
				}
			}
			return;
		}
		case "PROPULSION":
			if (this.CheckSubsystem("REACTOR", false))
			{
				this.SetPlayerMaxTelegraph(3);
			}
			else
			{
				this.SetPlayerMaxTelegraph(1);
			}
			break;
		case "RUDDER":
			this.playervessel.vesselmovement.turnSpeed /= 2f;
			break;
		case "PLANES":
			this.playervessel.vesselmovement.diveRate /= 2f;
			this.playervessel.vesselmovement.surfaceRate /= 2f;
			break;
		case "BALLAST":
			this.playervessel.vesselmovement.ballastRate /= 2f;
			if (this.playervessel.vesselmovement.ballastAngle.x > 3f)
			{
				this.playervessel.vesselmovement.ballastAngle.x = 3f;
			}
			break;
		case "REACTOR":
			this.SetPlayerMaxTelegraph(1);
			UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("reactor", true);
			break;
		}
		if (this.damageControlCurrentTimers[subsystemIndex] > 0f && subsystem == "PUMPS" && subsystem == "REACTOR" && subsystem == "BALLAST")
		{
			return;
		}
		this.subsystemOffline[subsystemIndex] = true;
		if (destroy && subsystem != "PUMPS" && subsystem != "REACTOR" && subsystem != "BALLAST")
		{
			this.damageControlCurrentTimers[subsystemIndex] = 10000f;
			this.damageControlSliders[subsystemIndex].value = 0f;
		}
		else
		{
			this.damageControlCurrentTimers[subsystemIndex] += UnityEngine.Random.Range(60f, 300f) * OptionsManager.difficultySettings["CombatRepairTimeModifier"];
			this.damageControlSliders[subsystemIndex].value = this.damageControlCurrentTimers[subsystemIndex];
		}
		this.CheckSubsystem(subsystem, true);
		if (UIFunctions.globaluifunctions.playerfunctions.timeCompressionToggle == 1)
		{
			UIFunctions.globaluifunctions.playerfunctions.SetTimeCompression();
		}
		if (this.uifunctions.playerfunctions.statusscreens.statusPages[1].activeSelf)
		{
			this.UpdateDamageTimers();
		}
	}

	// Token: 0x0600070E RID: 1806 RVA: 0x0003E184 File Offset: 0x0003C384
	public void SetPlayerMaxTelegraph(int max)
	{
		this.playerMaxTelegraph = max;
		if (this.playervessel.vesselmovement.telegraphValue > this.playerMaxTelegraph)
		{
			this.playervessel.vesselmovement.telegraphValue = this.playerMaxTelegraph;
			UIFunctions.globaluifunctions.playerfunctions.helmmanager.wantedSpeed = (float)Mathf.RoundToInt(GameDataManager.playervesselsonlevel[0].databaseshipdata.telegraphSpeeds[this.playervessel.vesselmovement.telegraphValue] * 10f);
			UIFunctions.globaluifunctions.playerfunctions.helmmanager.speedDisplayText.text = UIFunctions.globaluifunctions.playerfunctions.helmmanager.wantedSpeed.ToString();
			UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentSpeed = UIFunctions.globaluifunctions.playerfunctions.helmmanager.wantedSpeed.ToString();
		}
		if (this.playerMaxTelegraph == 1)
		{
			this.playervessel.vesselmovement.telegraphValue = this.playerMaxTelegraph;
			UIFunctions.globaluifunctions.playerfunctions.helmmanager.wantedSpeed = 0f;
			UIFunctions.globaluifunctions.playerfunctions.helmmanager.speedDisplayText.text = UIFunctions.globaluifunctions.playerfunctions.helmmanager.wantedSpeed.ToString();
			UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentSpeed = UIFunctions.globaluifunctions.playerfunctions.helmmanager.wantedSpeed.ToString();
		}
		UIFunctions.globaluifunctions.playerfunctions.helmmanager.DisplayCurrentTelegraph();
		GameDataManager.playervesselsonlevel[0].vesselmovement.engineSpeed.x = (float)(-1 + GameDataManager.playervesselsonlevel[0].vesselmovement.telegraphValue) / 5f * GameDataManager.playervesselsonlevel[0].vesselmovement.shipSpeed.y;
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x0003E360 File Offset: 0x0003C560
	private void ReEnableSubsystem(string subsystem)
	{
		int subsystemIndex = DamageControl.GetSubsystemIndex(subsystem);
		switch (subsystem)
		{
		case "PERISCOPE":
			UIFunctions.globaluifunctions.playerfunctions.helmmanager.periscopePanel.SetActive(UIFunctions.globaluifunctions.playerfunctions.statusIcons[12].gameObject.activeSelf);
			break;
		case "PROPULSION":
			if (this.CheckSubsystem("REACTOR", false))
			{
				this.SetPlayerMaxTelegraph(6);
			}
			else
			{
				this.SetPlayerMaxTelegraph(1);
			}
			break;
		case "RUDDER":
			this.playervessel.vesselmovement.turnSpeed = this.playervessel.databaseshipdata.turnrate;
			break;
		case "PLANES":
			this.playervessel.vesselmovement.diveRate = this.playervessel.databaseshipdata.diverate;
			this.playervessel.vesselmovement.surfaceRate = this.playervessel.databaseshipdata.surfacerate;
			break;
		case "BALLAST":
			this.playervessel.vesselmovement.ballastRate = this.playervessel.databaseshipdata.ballastrate;
			break;
		case "REACTOR":
			if (this.CheckSubsystem("PROPULSION", false))
			{
				this.SetPlayerMaxTelegraph(6);
			}
			else
			{
				this.SetPlayerMaxTelegraph(3);
			}
			UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("reactor", false);
			break;
		}
		UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentSubsystem = subsystem;
		string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "SubsystemRepaired");
		text = text.Replace("<SUBSYSTEM>", UIFunctions.globaluifunctions.database.databasesubsystemsdata[subsystemIndex].subsystemName);
		UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["XO"], "SubsystemRepaired", false);
		this.subsystemOffline[subsystemIndex] = false;
	}

	// Token: 0x06000710 RID: 1808 RVA: 0x0003E644 File Offset: 0x0003C844
	private void FixedUpdate()
	{
		if (this.kotubes)
		{
			this.kotubes = false;
			this.KnockoutSubsystem("TUBES", true);
		}
		if (this.koSomething)
		{
			this.koSomething = false;
			this.KnockoutSubsystem("REACTOR", true);
		}
		this.damageControlTimer += Time.deltaTime;
		if (this.damageControlTimer > 1f)
		{
			if (!PlayerFunctions.runningSilent)
			{
				this.UpdateDamageTimers();
			}
			this.UpdateFloodingStatus();
			this.damageControlTimer -= 1f;
		}
	}

	// Token: 0x06000711 RID: 1809 RVA: 0x0003E6D8 File Offset: 0x0003C8D8
	private void UpdateFloodingStatus()
	{
		if (GameDataManager.playervesselsonlevel[0] == null)
		{
			return;
		}
		if (GameDataManager.playervesselsonlevel[0].damagesystem.shipCurrentDamagePoints == 0f)
		{
			return;
		}
		float num = 0f;
		this.floodRateModifier = (1f + (1000f - GameDataManager.playervesselsonlevel[0].transform.position.y)) / 5f;
		bool flag = this.CheckSubsystem("PUMPS", false);
		for (int i = 0; i < this.compartmentCurrentFlooding.Length; i++)
		{
			if (this.compartmentCurrentFlooding[i] < this.compartmentTotalFlooding[i] && this.compartmentCurrentFlooding[i] < 1f)
			{
				this.compartmentCurrentFlooding[i] += this.compartmentTotalFlooding[i] * this.floodRateModifier * Time.deltaTime * this.floodSpeedFactor;
				if (this.compartmentCurrentFlooding[i] > 1f)
				{
					this.compartmentCurrentFlooding[i] = 1f;
				}
			}
			if (this.compartmentCurrentFlooding[i] > 0f)
			{
				if (!PlayerFunctions.runningSilent)
				{
					if (flag)
					{
						this.compartmentCurrentFlooding[i] -= this.playerPumpRate * Time.deltaTime * this.floodSpeedFactor;
					}
					if (this.currentCompartment == i)
					{
						this.compartmentCurrentFlooding[i] -= this.playerPumpRate * Time.deltaTime * this.floodSpeedFactor;
					}
				}
				if (this.compartmentCurrentFlooding[i] < 0f)
				{
					this.compartmentCurrentFlooding[i] = 0f;
					this.compartmentTotalFlooding[i] = 0f;
				}
				num += this.compartmentCurrentFlooding[i];
				float num2 = this.compartmentCurrentFlooding[i] * this.waterRanges[i] + GameDataManager.playervesselsonlevel[0].databaseshipdata.compartmentFloodingRanges[i].x;
				this.compartmentWaterLevels[i].transform.localPosition = new Vector3(this.compartmentWaterLevels[i].transform.localPosition.x, num2 + 137.5f, this.compartmentWaterLevels[i].transform.localPosition.z);
			}
		}
		bool flag2 = this.IsTooFlooded();
		if (UIFunctions.globaluifunctions.playerfunctions.statusIcons[8].gameObject.activeSelf != flag2)
		{
			UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("flooding", this.IsTooFlooded());
		}
	}

	// Token: 0x06000712 RID: 1810 RVA: 0x0003E960 File Offset: 0x0003CB60
	public float GetPlayerCurrentFlooding()
	{
		float num = 0f;
		for (int i = 0; i < this.compartmentCurrentFlooding.Length; i++)
		{
			num += this.compartmentCurrentFlooding[i];
		}
		return num;
	}

	// Token: 0x06000713 RID: 1811 RVA: 0x0003E998 File Offset: 0x0003CB98
	public void UpdateDamageTimers()
	{
		bool flag = false;
		for (int i = 0; i < this.damageControlCurrentTimers.Length; i++)
		{
			if (this.damageControlCurrentTimers[i] > 0f && this.damageControlCurrentTimers[i] < 10000f)
			{
				this.damageControlCurrentTimers[i] -= 1f;
				if (this.currentSubsystem == i)
				{
					this.damageControlCurrentTimers[i] -= 1f;
				}
				if (this.damageControlCurrentTimers[i] <= 0f)
				{
					this.ReEnableSubsystem(UIFunctions.globaluifunctions.database.databasesubsystemsdata[i].subsystem);
					this.damageControlCurrentTimers[i] = 0f;
				}
				else
				{
					flag = true;
				}
				if (UIFunctions.globaluifunctions.playerfunctions.currentOpenPanel == 2)
				{
					this.DisplayDamageLabelColors(i);
				}
				this.damageControlSliders[i].value = this.damageControlCurrentTimers[i];
			}
		}
		if (UIFunctions.globaluifunctions.playerfunctions.statusIcons[7].gameObject.activeSelf != flag)
		{
			UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("damage", flag);
		}
	}

	// Token: 0x06000714 RID: 1812 RVA: 0x0003EAC4 File Offset: 0x0003CCC4
	public void DisplayDamageLabelColors(int i)
	{
		if (this.damageControlCurrentTimers[i] >= 10000f)
		{
			this.damageControlLabels[i].color = this.damageLabelColors[4];
		}
		else if (this.damageControlCurrentTimers[i] > 0f)
		{
			this.damageControlLabels[i].color = this.damageLabelColors[3];
		}
		else
		{
			this.damageControlLabels[i].color = this.damageLabelColors[0];
		}
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x0003EB68 File Offset: 0x0003CD68
	public int GetTimeToRepair()
	{
		if (!UIFunctions.globaluifunctions.campaignmanager.playerInPort)
		{
			this.timeToRepairReadout.text = string.Empty;
			return 0;
		}
		int num = 0;
		if (this.damageControlCurrentTimers[this.currentSubsystem] == 10000f)
		{
			num = (int)(UIFunctions.globaluifunctions.database.databasesubsystemsdata[this.currentSubsystem].repairTime * OptionsManager.difficultySettings["RepairTimeModifier"]);
		}
		if (num > 0)
		{
			this.timeToRepairReadout.gameObject.SetActive(true);
			this.timeToRepairReadout.text = string.Concat(new string[]
			{
				"<b>",
				LanguageManager.interfaceDictionary["RepairTime"],
				" ",
				num.ToString(),
				" ",
				LanguageManager.interfaceDictionary["Hours"],
				"</b>"
			});
		}
		else
		{
			this.timeToRepairReadout.text = string.Empty;
		}
		return num;
	}

	// Token: 0x06000716 RID: 1814 RVA: 0x0003EC74 File Offset: 0x0003CE74
	public void RepairSubsystem()
	{
		if (UIFunctions.globaluifunctions.backgroundImagesOnly.activeSelf && this.timeToRepairReadout.gameObject.activeSelf && this.timeToRepairReadout.text != string.Empty)
		{
			UIFunctions.globaluifunctions.campaignmanager.timeInPort += (float)this.GetTimeToRepair();
			UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlCurrentTimers[this.currentSubsystem] = 0f;
			this.DisplayDamageLabelColors(this.currentSubsystem);
			this.timeToRepairReadout.gameObject.SetActive(false);
			UIFunctions.globaluifunctions.campaignmanager.gameObject.SetActive(true);
			UIFunctions.globaluifunctions.campaignmanager.enabled = true;
			if (DamageControl.GetSubsystemIndex("TUBES") == this.currentSubsystem)
			{
				for (int i = 0; i < GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.tubeStatus.Length; i++)
				{
					if (GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.tubeStatus[i] == -200)
					{
						GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.tubeStatus[i] = -10;
						GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.weaponInTube[i] = -10;
						UIFunctions.globaluifunctions.playerfunctions.torpedoTubeImages[i].gameObject.SetActive(false);
						UIFunctions.globaluifunctions.playerfunctions.ClearTubeSettingButtons(i);
					}
				}
			}
		}
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x0003EE04 File Offset: 0x0003D004
	public static int GetSubsystemIndex(string subsystem)
	{
		int num = -1;
		for (int i = 0; i < UIFunctions.globaluifunctions.database.databasesubsystemsdata.Length; i++)
		{
			if (subsystem == UIFunctions.globaluifunctions.database.databasesubsystemsdata[i].subsystem)
			{
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			Debug.LogError("Failed to find SUBSYSTEM: " + subsystem);
		}
		return num;
	}

	// Token: 0x04000863 RID: 2147
	public StatusScreens statusscreens;

	// Token: 0x04000864 RID: 2148
	public UIFunctions uifunctions;

	// Token: 0x04000865 RID: 2149
	private Vessel playervessel;

	// Token: 0x04000866 RID: 2150
	public bool[] subsystemOffline;

	// Token: 0x04000867 RID: 2151
	public float[] damageControlCurrentTimers;

	// Token: 0x04000868 RID: 2152
	private float damageControlTimer;

	// Token: 0x04000869 RID: 2153
	public float floodRateModifier;

	// Token: 0x0400086A RID: 2154
	public float playerPumpRate;

	// Token: 0x0400086B RID: 2155
	public float[] compartmentCurrentFlooding;

	// Token: 0x0400086C RID: 2156
	public float[] compartmentTotalFlooding;

	// Token: 0x0400086D RID: 2157
	public GameObject damageControlPanel;

	// Token: 0x0400086E RID: 2158
	public Text[] damageControlLabels;

	// Token: 0x0400086F RID: 2159
	public Slider[] damageControlSliders;

	// Token: 0x04000870 RID: 2160
	public Color32[] damageLabelColors;

	// Token: 0x04000871 RID: 2161
	public int currentSubsystem;

	// Token: 0x04000872 RID: 2162
	public int currentCompartment;

	// Token: 0x04000873 RID: 2163
	public RectTransform[] compartmentWaterLevels;

	// Token: 0x04000874 RID: 2164
	public float[] waterRanges;

	// Token: 0x04000875 RID: 2165
	public Text timeToRepairReadout;

	// Token: 0x04000876 RID: 2166
	public Text hullStatusReadout;

	// Token: 0x04000877 RID: 2167
	public float floodSpeedFactor = 1f;

	// Token: 0x04000878 RID: 2168
	public Transform pumpCompartment;

	// Token: 0x04000879 RID: 2169
	public int playerMaxTelegraph;

	// Token: 0x0400087A RID: 2170
	public bool kotubes;

	// Token: 0x0400087B RID: 2171
	public bool koSomething;
}
