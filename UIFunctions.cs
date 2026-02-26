using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

// Token: 0x02000169 RID: 361
public class UIFunctions : MonoBehaviour
{
	// Token: 0x06000AD4 RID: 2772 RVA: 0x0009638C File Offset: 0x0009458C
	private void OnApplicationQuit()
	{
		this.skyboxmaterial.SetTexture("_MainTex", null);
		this.hudmaterial.color = Color.white;
	}

	// Token: 0x06000AD5 RID: 2773 RVA: 0x000963B0 File Offset: 0x000945B0
	private void Start()
	{
		AudioManager.audiomanager.SetEffectsVolume(GameDataManager.currentvolume);
		AudioManager.audiomanager.SetMusicVolume(GameDataManager.currentmusicvolume);
		UIFunctions.globaluifunctions.playerfunctions.voicemanager.voiceSource.volume = GameDataManager.optionsFloatSettings[9];
		AudioManager.audiomanager.SetCombatSounds(false);
		AudioManager.audiomanager.PlayMusicClip(0, string.Empty);
		this.combatai.enabled = false;
		UIFunctions.globaluifunctions.HUDholder.SetActive(false);
		this.helpmanager.InitialiseHelpDictionaries();
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x00096440 File Offset: 0x00094640
	private void Awake()
	{
		this.versionText.text = "© 2017        version " + GameDataManager.gameVersion;
		Time.timeScale = 1f;
		UIFunctions.globaluifunctions = this;
		UIFunctions.languagemanager = UnityEngine.Object.FindObjectOfType<LanguageManager>();
		UIFunctions.languagemanager.InitialiseLanguageSetUp();
		InputManager.globalInputManager = UnityEngine.Object.FindObjectOfType<InputManager>();
		InputManager.globalInputManager.InitialiseInputManager();
		AudioManager.audiomanager = UnityEngine.Object.FindObjectOfType<AudioManager>();
		ManualCameraZoom.oceanShadowPlane = GameObject.Find("ocean_shadows");
		ManualCameraZoom.oceanShadowPlane.SetActive(false);
		GameDataManager.missionMode = false;
		GameDataManager.trainingMode = false;
		GameDataManager.campaignMode = false;
		LevelLoadManager.inMuseum = false;
		KeybindManagerMuseum.selectionScreen = false;
		GameDataManager.playervesselsonlevel = new Vessel[0];
		GameDataManager.enemyvesselsonlevel = new Vessel[0];
		this.levelloadmanager.submarineMarker.gameObject.SetActive(false);
		GameDataManager.HUDActive = false;
		GameDataManager.screenResolution = new int[2];
		this.optionsmanager.OptionsStart();
		this.SetMenuSystem("MAINMENU");
		this.scrollbarDefault.gameObject.SetActive(false);
		this.levelLoadMessage.SetActive(false);
		Screen.orientation = ScreenOrientation.AutoRotation;
		this.hudmaterial.color = Color.white;
		this.bringinHUD = false;
		this.damageControlIcons = new GameObject[2];
		UIFunctions.globaluifunctions.cameraMount.gameObject.SetActive(false);
		if (GameDataManager.loadVideo)
		{
			this.backgroundImagesOnly.SetActive(false);
			this.movietexture = this.textparser.GetMovieTexture("menu_1080");
			this.movietexture.loop = true;
			this.movieMaterial.SetTexture("_MainTex", this.movietexture);
			this.movietexture.Stop();
			this.movietexture.Play();
		}
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x000965F4 File Offset: 0x000947F4
	public void SetMenuSystem(string menuPage)
	{
		int menuSystemPageIndex = this.database.GetMenuSystemPageIndex(menuPage);
		this.backgroundTemplate.SetActive(true);
		this.backgroundImagesOnly.SetActive(true);
		if (!this.menuSystemParent.activeSelf)
		{
			this.menuSystemParent.SetActive(true);
		}
		for (int i = 0; i < this.menuSystem.Length; i++)
		{
			if (i == menuSystemPageIndex)
			{
				this.menuSystem[i].SetActive(true);
			}
			else
			{
				this.menuSystem[i].SetActive(false);
			}
		}
		UIFunctions.globaluifunctions.scrollbarDefault.gameObject.SetActive(true);
		if (menuPage != "OPTIONS" && menuPage != "MAINMENU")
		{
			if (this.movietexture != null)
			{
				this.movietexture = null;
				this.movieMaterial.SetTexture("_MainTex", null);
				GC.Collect();
			}
			this.movieObject.SetActive(false);
		}
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x000966F4 File Offset: 0x000948F4
	public void OpenCredits()
	{
		this.SetMenuSystem("CREDITS");
		this.mainTitle.text = string.Empty;
		this.secondaryTitle.text = string.Empty;
		this.mainColumn.text = string.Empty;
		this.secondaryTitle.text = string.Empty;
		UIFunctions.globaluifunctions.scrollbarDefault.gameObject.SetActive(false);
		AudioManager.audiomanager.PlayMusicClip(6, string.Empty);
		if (GameDataManager.loadVideo)
		{
			this.backgroundImagesOnly.SetActive(false);
			this.creditMovietexture = this.textparser.GetMovieTexture("credits_1080");
			this.creditMovietexture.loop = true;
			this.movieMaterial.SetTexture("_MainTex", this.creditMovietexture);
			this.creditMovietexture.Stop();
			this.creditMovietexture.Play();
		}
	}

	// Token: 0x06000AD9 RID: 2777 RVA: 0x000967D8 File Offset: 0x000949D8
	public void CloseActionReport()
	{
		if (UIFunctions.globaluifunctions.playerfunctions.currentFullLogParentObject != null)
		{
			UnityEngine.Object.Destroy(UIFunctions.globaluifunctions.playerfunctions.currentFullLogParentObject);
		}
		this.afterActionReport.SetActive(false);
		UIFunctions.globaluifunctions.playerfunctions.menuPanel.transform.localPosition -= new Vector3(UIFunctions.globaluifunctions.playerfunctions.menuPanelOffset.x, UIFunctions.globaluifunctions.playerfunctions.menuPanelOffset.y, 0f);
		this.CleanupLevel();
		if (!GameDataManager.trainingMode && !GameDataManager.missionMode)
		{
			this.campaignmanager.EndCampaignCombat();
			if (this.campaignmanager.playercampaigndata.currentMissionTaskForceID != this.campaignmanager.currentTaskForceEngagedWith && !UIFunctions.globaluifunctions.campaignmanager.eventManager.gameObject.activeSelf)
			{
				this.campaignmanager.enabled = true;
				AudioManager.audiomanager.PlayMusicClip(1, string.Empty);
			}
		}
		Time.timeScale = 1f;
	}

	// Token: 0x06000ADA RID: 2778 RVA: 0x00096900 File Offset: 0x00094B00
	private void Update()
	{
		if (InputManager.globalInputManager.GetButtonDown("Help", false))
		{
			UIFunctions.globaluifunctions.helpmanager.OpenHelp();
		}
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x00096934 File Offset: 0x00094B34
	private void FixedUpdate()
	{
		if (!GameDataManager.HUDActive)
		{
			return;
		}
		float num = this.MainCamera.transform.eulerAngles.y;
		if (num > 180f)
		{
			num = -360f + num;
		}
		else if (num < -180f)
		{
			num = 360f + num;
		}
		this.tapeBearing.SetTextureOffset("_MainTex", new Vector2(0f, -num * 0.0013f));
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x000969B4 File Offset: 0x00094BB4
	public float GetBearingToTransform(Transform activeTransform, Transform targetTransform)
	{
		this.directionFinder.transform.parent = activeTransform;
		this.directionFinder.localPosition = Vector3.zero;
		this.directionFinder.LookAt(targetTransform);
		float num = this.directionFinder.localEulerAngles.y;
		if (num > 180f)
		{
			num -= 360f;
		}
		this.directionFinder.transform.parent = null;
		return num;
	}

	// Token: 0x06000ADD RID: 2781 RVA: 0x00096A28 File Offset: 0x00094C28
	public float GetXZDistance(Vector3 position1, Vector3 position2)
	{
		return Vector2.Distance(new Vector2(position1.x, position1.z), new Vector2(position2.x, position2.z));
	}

	// Token: 0x06000ADE RID: 2782 RVA: 0x00096A58 File Offset: 0x00094C58
	private float ConvertAngleToBearing(float degreestotarget, float currentheading)
	{
		degreestotarget -= currentheading;
		if (degreestotarget < -180f)
		{
			degreestotarget = 360f + degreestotarget;
		}
		else if (degreestotarget > 360f)
		{
			degreestotarget -= 360f;
		}
		if (degreestotarget > 180f)
		{
			degreestotarget = -1f * (degreestotarget - 180f - 180f);
		}
		return degreestotarget;
	}

	// Token: 0x06000ADF RID: 2783 RVA: 0x00096AB8 File Offset: 0x00094CB8
	private string GetShipStatus(Vessel currentVessel, int enemyIndex, bool playingCampaign)
	{
		string result = string.Empty;
		float shipCurrentDamagePoints = currentVessel.damagesystem.shipCurrentDamagePoints;
		bool isCivilian = Database.GetIsCivilian(currentVessel.databaseshipdata.shipPrefabName);
		if (currentVessel.isSinking || currentVessel.isCapsizing)
		{
			result = LanguageManager.interfaceDictionary["ActionReportSunk"];
			if (playingCampaign)
			{
				this.totalMissionPointsEarned += 1f;
				if (enemyIndex < this.levelloadmanager.levelloaddata.enemyUnitMissionCritical.Length && this.levelloadmanager.levelloaddata.enemyUnitMissionCritical[enemyIndex])
				{
					this.totalCriticalMissionPointsEarned += 1f;
				}
				this.campaignmanager.playercampaigndata.patrolTonnage += (float)((int)currentVessel.databaseshipdata.displacement);
				this.campaignmanager.playercampaigndata.totalTonnage += (float)((int)currentVessel.databaseshipdata.displacement);
				int[] array = new int[]
				{
					4,
					8
				};
				if (currentVessel.databaseshipdata.shipType == "MERCHANT")
				{
					array = new int[]
					{
						6,
						10
					};
				}
				else if (currentVessel.databaseshipdata.shipType == "SUBMARINE")
				{
					array = new int[]
					{
						5,
						9
					};
				}
				else if (currentVessel.databaseshipdata.shipType == "CAPITAL")
				{
					array = new int[]
					{
						3,
						7
					};
				}
				this.campaignmanager.playercampaigndata.campaignStats[array[0]] += 1f;
				this.campaignmanager.playercampaigndata.campaignStats[array[1]] += currentVessel.databaseshipdata.displacement;
			}
		}
		else if (shipCurrentDamagePoints == 0f)
		{
			result = LanguageManager.interfaceDictionary["ActionReportEscaped"];
		}
		else
		{
			result = LanguageManager.interfaceDictionary["ActionReportDamaged"];
			if (playingCampaign)
			{
				this.totalMissionPointsEarned += 0.5f;
				if (this.levelloadmanager.levelloaddata.enemyUnitMissionCritical[enemyIndex])
				{
					this.totalCriticalMissionPointsEarned += 0.5f;
				}
			}
		}
		if (!this.playerfunctions.sensormanager.initialDetectedByPlayer[enemyIndex])
		{
			result = LanguageManager.interfaceDictionary["ActionReportUndetected"];
		}
		return result;
	}

	// Token: 0x06000AE0 RID: 2784 RVA: 0x00096D38 File Offset: 0x00094F38
	private void CalculateNeutralCasualties()
	{
		if (this.campaignmanager.playercampaigndata.neutralCasualties == null)
		{
			this.campaignmanager.playercampaigndata.neutralCasualties = new float[3];
		}
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			if (Database.GetIsCivilian(GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipPrefabName))
			{
				if (GameDataManager.enemyvesselsonlevel[i].isSinking && GameDataManager.enemyvesselsonlevel[i].damagesystem.vesselDamagedByPlayer)
				{
					this.campaignmanager.playercampaigndata.neutralCasualties[1] += 1f;
					this.campaignmanager.playercampaigndata.neutralCasualties[2] += GameDataManager.enemyvesselsonlevel[i].databaseshipdata.displacement;
				}
				else if (GameDataManager.enemyvesselsonlevel[i].damagesystem.vesselDamagedByPlayer)
				{
					this.campaignmanager.playercampaigndata.neutralCasualties[0] += 1f;
				}
			}
		}
	}

	// Token: 0x06000AE1 RID: 2785 RVA: 0x00096E50 File Offset: 0x00095050
	public void ClearSelectionToggleBars()
	{
		if (this.selectionBars != null)
		{
			for (int i = 0; i < this.selectionBars.Length; i++)
			{
				UnityEngine.Object.Destroy(this.selectionBars[i]);
			}
		}
		this.selectionBars = null;
		this.selectionGroupText.text = string.Empty;
	}

	// Token: 0x06000AE2 RID: 2786 RVA: 0x00096EA8 File Offset: 0x000950A8
	public void DestroyAllAircraft()
	{
		for (int i = 0; i < this.combatai.enemyAircraft.Length; i++)
		{
			if (this.combatai.enemyAircraft[i] != null)
			{
				this.combatai.enemyAircraft[i].waypoint.SetParent(this.combatai.enemyAircraft[i].transform, false);
				for (int j = 0; j < this.combatai.enemyAircraft[i].sensordata.sonobuoys.Length; j++)
				{
					UnityEngine.Object.Destroy(this.combatai.enemyAircraft[i].sensordata.sonobuoys[j].gameObject);
				}
				UnityEngine.Object.Destroy(this.combatai.enemyAircraft[i].gameObject);
			}
		}
		for (int k = 0; k < this.combatai.enemyHelicopters.Length; k++)
		{
			if (this.combatai.enemyHelicopters[k] != null)
			{
				this.combatai.enemyHelicopters[k].waypointTransform.SetParent(this.combatai.enemyHelicopters[k].transform, false);
				for (int l = 0; l < this.combatai.enemyHelicopters[k].sensordata.sonobuoys.Length; l++)
				{
					UnityEngine.Object.Destroy(this.combatai.enemyHelicopters[k].sensordata.sonobuoys[l].gameObject);
				}
				UnityEngine.Object.Destroy(this.combatai.enemyHelicopters[k].gameObject);
			}
		}
		this.combatai.enemyAircraft = new Aircraft[0];
		this.combatai.enemyHelicopters = new Helicopter[0];
		this.levelloadmanager.levelloaddata.numberOfHelicopters = 0;
		this.levelloadmanager.levelloaddata.numberOfAircraft = 0;
	}

	// Token: 0x06000AE3 RID: 2787 RVA: 0x00097084 File Offset: 0x00095284
	public void CleanupLevel()
	{
		AudioManager.audiomanager.StopCombatMusic();
		ManualCameraZoom.oceanShadowPlane.SetActive(false);
		this.playerfunctions.esmGameObject.SetActive(false);
		this.playerfunctions.statusIconsParent.SetActive(false);
		this.combatai.enabled = false;
		if (GameDataManager.trainingMode || GameDataManager.missionMode)
		{
			AudioManager.audiomanager.PlayMusicClip(0, string.Empty);
			if (UIFunctions.globaluifunctions.helpmanager.tutorialObject.activeSelf)
			{
				UIFunctions.globaluifunctions.helpmanager.tutorialObject.SetActive(false);
			}
		}
		else
		{
			this.playerfunctions.damagecontrol.ResetFloodingIndicators();
		}
		if (this.levelloadmanager.currentMapGeneratorInstance != null)
		{
			UnityEngine.Object.Destroy(this.levelloadmanager.currentMapGeneratorInstance);
		}
		this.levelloadmanager.periscopeMaskRenderer.material = null;
		this.levelloadmanager.submarineMarker.gameObject.SetActive(false);
		this.particlefield.gameObject.SetActive(false);
		this.playerfunctions.binocularZoomText.gameObject.SetActive(false);
		this.playerfunctions.sensormanager.tacticalmap.zoomText.text = string.Empty;
		this.levelloadmanager.oceanTilingScript.enabled = false;
		this.ClearDefaultTextBox();
		this.keybindManager.gameObject.SetActive(false);
		this.MainCamera.transform.position = new Vector3(0f, 2000f, 0f);
		if (this.levelloadmanager.oceanblending != null)
		{
			this.levelloadmanager.oceanblending.enabled = false;
		}
		this.GUICamera.active = true;
		this.HUDCamera.active = false;
		AudioListener component = UIFunctions.globaluifunctions.GUICamera.GetComponent<AudioListener>();
		if (!component.enabled)
		{
			component.enabled = true;
		}
		component = this.MainCamera.GetComponent<AudioListener>();
		component.enabled = false;
		this.HUDholder.SetActive(false);
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.tacMapCamera.gameObject.SetActive(false);
		this.playerfunctions.bearingLine.transform.parent = null;
		this.skyboxmaterial.SetTexture("_MainTex", null);
		this.levelloadmanager.skyboxobject.SetActive(false);
		this.levelloadmanager.moonObject.SetActive(false);
		UnityEngine.Object.Destroy(this.levelloadmanager.environmentObject.gameObject);
		UnityEngine.Object.Destroy(this.levelloadmanager.oceanObjectInstance.gameObject);
		UnityEngine.Object.Destroy(this.levelloadmanager.levelloaddata.formationGrid.gameObject);
		for (int i = 0; i < this.levelloadmanager.tacticalmap.allVesselsList.Length; i++)
		{
			UnityEngine.Object.Destroy(this.levelloadmanager.tacticalmap.allVesselsList[i].gameObject);
		}
		GameDataManager.enemyvesselsonlevel = new Vessel[0];
		this.levelloadmanager.tacticalmap.TacticalMapCleanup();
		GameDataManager.HUDActive = false;
		if (GameDataManager.trainingMode)
		{
			this.missionmanager.InitialiseMissionList("TRAINING");
			this.backgroundImagesOnly.SetActive(true);
		}
		else if (GameDataManager.missionMode)
		{
			this.missionmanager.InitialiseMissionList("SINGLE");
			this.backgroundImagesOnly.SetActive(true);
		}
		this.oceansound.Stop();
		this.underwatersound.Stop();
		if (GameDataManager.currentmusicvolume > 0f && !this.music.isPlaying)
		{
			this.music.Play();
		}
		ObjectPoolManager objectPoolManager = UnityEngine.Object.FindObjectOfType(typeof(ObjectPoolManager)) as ObjectPoolManager;
		if (objectPoolManager != null)
		{
			UnityEngine.Object.Destroy(objectPoolManager.gameObject);
		}
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	// Token: 0x06000AE4 RID: 2788 RVA: 0x00097474 File Offset: 0x00095674
	public void ClearDefaultTextBox()
	{
		this.mainTitle.text = string.Empty;
		this.secondaryTitle.text = string.Empty;
		this.dateTitle.text = string.Empty;
		this.mainColumn.text = string.Empty;
		this.secondColumm.text = string.Empty;
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x000974D4 File Offset: 0x000956D4
	public void SetMainColumnHeight(bool scrollbarAlwaysOn)
	{
		float preferredHeight = this.mainColumn.preferredHeight;
		this.mainColumn.rectTransform.sizeDelta = new Vector2(this.mainColumn.rectTransform.sizeDelta.x, preferredHeight);
		if (preferredHeight > 545f)
		{
			this.scrollbarDefault.gameObject.SetActive(true);
			this.scrollbarDefault.value = 1f;
		}
		else if (!scrollbarAlwaysOn)
		{
			this.scrollbarDefault.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x00097564 File Offset: 0x00095764
	public void CloseCombatScreens()
	{
		if (!this.bearingMarker.gameObject.activeSelf)
		{
			this.levelloadmanager.tacticalmap.SetTacticalMap();
		}
	}

	// Token: 0x06000AE7 RID: 2791 RVA: 0x0009758C File Offset: 0x0009578C
	public void BuildBriefingHeader()
	{
		this.mainTitle.text = string.Empty;
		string playerVesselName = this.textparser.GetPlayerVesselName();
		this.secondaryTitle.text = playerVesselName + "\n";
		this.SetSecondaryTitles();
	}

	// Token: 0x06000AE8 RID: 2792 RVA: 0x000975D4 File Offset: 0x000957D4
	public void SinkAllAndEndLevel()
	{
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			if (GameDataManager.enemyvesselsonlevel[i].whichNavy == 1)
			{
				GameDataManager.enemyvesselsonlevel[i].isSinking = true;
			}
		}
		this.EndLevel();
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x00097620 File Offset: 0x00095820
	public void EndLevel()
	{
		base.StopAllCoroutines();
		UIFunctions.globaluifunctions.playerfunctions.voicemanager.StopAudioArray();
		UIFunctions.globaluifunctions.playerfunctions.voicemanager.voiceSource.enabled = false;
		this.MainCamera.GetComponent<ManualCameraZoom>().enabled = false;
		for (int i = 0; i < 3; i++)
		{
			UIFunctions.globaluifunctions.ingamereference.ingameReferencePanels[i].gameObject.SetActive(false);
		}
		if (GameDataManager.playervesselsonlevel[0].isSinking)
		{
			UIFunctions.globaluifunctions.missionmanager.playerSunk = true;
		}
		UIFunctions.globaluifunctions.totalCriticalMissionPointsEarned = 0f;
		UIFunctions.globaluifunctions.totalMissionPointsEarned = 0f;
		bool flag = false;
		if (!GameDataManager.missionMode && !GameDataManager.trainingMode)
		{
			flag = true;
		}
		this.keybindManager.gameObject.SetActive(false);
		this.combatai.enabled = false;
		PlayerFunctions.draggingWaypoint = false;
		AudioManager.audiomanager.SetCombatSounds(false);
		this.playerfunctions.sensormanager.tacticalmap.missionMarker.gameObject.SetActive(false);
		UIFunctions.globaluifunctions.bearingMarker.gameObject.SetActive(false);
		if (ManualCameraZoom.binoculars)
		{
			this.MainCamera.GetComponent<Camera>().fieldOfView = GameDataManager.mainCameraFOV;
			this.playerfunctions.helmmanager.SetPeriscopeMask(false);
		}
		this.playerfunctions.sensormanager.tacticalmap.tacMapCamera.gameObject.SetActive(false);
		this.CloseCombatScreens();
		this.combatai.enabled = false;
		this.playerfunctions.damagecontrol.enabled = false;
		this.HUDholder.SetActive(false);
		this.SetMenuSystem("ACTIONREPORT");
		UIFunctions.globaluifunctions.dateTitle.text = this.missionmanager.exitMissionDate.text;
		UIFunctions.globaluifunctions.secondaryTitle.text = this.missionmanager.exitMissionTitle.text;
		UIFunctions.globaluifunctions.secondaryTitle.gameObject.SetActive(true);
		this.backgroundImagesOnly.SetActive(false);
		AudioListener component = this.MainCamera.GetComponent<AudioListener>();
		component.enabled = false;
		component = this.GUICameraObject.GetComponent<AudioListener>();
		component.enabled = true;
		this.GUICamera.active = true;
		this.playerfunctions.sensormanager.tacticalmap.tacMapCamera.gameObject.SetActive(false);
		this.scrollbarDefault.value = 1f;
		this.backgroundImage.gameObject.SetActive(true);
		this.BuildBriefingHeader();
		string str = string.Empty;
		if (this.missionmanager.playerSunk || this.missionmanager.abandonedShip)
		{
			str = "_sunk";
		}
		if (flag)
		{
			this.mainColumn.text = UIFunctions.globaluifunctions.textparser.ReadDirectTextFromFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("language/message/orders_action_report" + str + this.playerfunctions.GetPlayerNavySuffix()));
		}
		else
		{
			this.mainColumn.text = UIFunctions.globaluifunctions.textparser.ReadDirectTextFromFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("language/message/orders_action_report" + str + this.playerfunctions.GetPlayerNavySuffix()));
		}
		string[] engagedContactsList = this.GetEngagedContactsList(flag);
		this.mainColumn.text = this.mainColumn.text.Replace("<ENGAGEDCONTACTSLIST>", engagedContactsList[0]);
		this.secondColumm.text = engagedContactsList[1];
		if (!this.missionmanager.playerSunk && !this.missionmanager.abandonedShip)
		{
			this.mainColumn.text = this.mainColumn.text.Replace("<WEAPONSONBOARD>", this.GetPlayerWeapons());
			this.mainColumn.text = this.mainColumn.text.Replace("<SYSTEMSDAMAGE>", this.GetPlayerSystems());
		}
		else
		{
			this.mainColumn.text = UIFunctions.globaluifunctions.textparser.PopulateDynamicTags(this.mainColumn.text, true);
			if (flag)
			{
				this.campaignmanager.playercampaigndata.playerVesselsLost = this.textparser.AddIntToArray(this.campaignmanager.playercampaigndata.playerVesselsLost, this.playerfunctions.playerVesselClass);
				this.campaignmanager.playercampaigndata.playerInstancesLost = this.textparser.AddIntToArray(this.campaignmanager.playercampaigndata.playerInstancesLost, this.playerfunctions.playerVesselInstance);
			}
		}
		if (flag)
		{
			this.SetCampaignMissionOutcome();
		}
		Text text = this.mainColumn;
		text.text += "\n";
		this.mainColumn.gameObject.SetActive(true);
		this.secondColumm.gameObject.SetActive(true);
		this.SetMainColumnHeight(false);
		this.DestroyAllAircraft();
	}

	// Token: 0x06000AEA RID: 2794 RVA: 0x00097B14 File Offset: 0x00095D14
	private void SetCampaignMissionOutcome()
	{
		float numberOfMissionCriticalShips = this.GetNumberOfMissionCriticalShips();
		float strategicValue = this.campaignmanager.campaignmissions[this.campaignmanager.campaignTaskForces[this.campaignmanager.currentTaskForceEngagedWith].missionID].strategicValue;
		this.campaignmanager.eventManager.missionPassed = false;
		this.campaignmanager.eventManager.medalAwarded = false;
		if (!this.campaignmanager.campaignmissions[this.campaignmanager.campaignTaskForces[this.campaignmanager.currentTaskForceEngagedWith].missionID].requiresStealth)
		{
			if (this.campaignmanager.campaignmissions[this.campaignmanager.campaignTaskForces[this.campaignmanager.currentTaskForceEngagedWith].missionID].requiresWeaponID == -1)
			{
				if (numberOfMissionCriticalShips > 0f)
				{
					if (UIFunctions.globaluifunctions.totalCriticalMissionPointsEarned / numberOfMissionCriticalShips >= 0.5f)
					{
						this.campaignmanager.campaignPoints -= strategicValue * OptionsManager.difficultySettings["WinMissionModifier"];
						this.campaignmanager.playercampaigndata.patrolPoints -= strategicValue * OptionsManager.difficultySettings["WinMissionModifier"];
						this.campaignmanager.eventManager.missionPassed = true;
					}
					else
					{
						this.campaignmanager.campaignPoints += strategicValue * OptionsManager.difficultySettings["FailMissionModifier"];
						this.campaignmanager.playercampaigndata.patrolPoints += strategicValue * OptionsManager.difficultySettings["FailMissionModifier"];
					}
				}
				else if (UIFunctions.globaluifunctions.totalMissionPointsEarned / (float)GameDataManager.enemyvesselsonlevel.Length >= 0.5f)
				{
					this.campaignmanager.campaignPoints -= strategicValue * OptionsManager.difficultySettings["WinMissionModifier"];
					this.campaignmanager.playercampaigndata.patrolPoints -= strategicValue * OptionsManager.difficultySettings["WinMissionModifier"];
					this.campaignmanager.eventManager.missionPassed = true;
				}
			}
			else if (UIFunctions.globaluifunctions.playerfunctions.landAttackNumber >= this.campaignmanager.campaignmissions[this.campaignmanager.campaignTaskForces[this.campaignmanager.currentTaskForceEngagedWith].missionID].numberOfRequiredWeapon)
			{
				this.campaignmanager.campaignPoints -= strategicValue * OptionsManager.difficultySettings["WinMissionModifier"];
				this.campaignmanager.playercampaigndata.patrolPoints -= strategicValue * OptionsManager.difficultySettings["WinMissionModifier"];
				this.campaignmanager.eventManager.missionPassed = true;
			}
			else
			{
				this.campaignmanager.campaignPoints += strategicValue * OptionsManager.difficultySettings["FailMissionModifier"];
				this.campaignmanager.playercampaigndata.patrolPoints += strategicValue * OptionsManager.difficultySettings["FailMissionModifier"];
			}
		}
		else if (this.campaignmanager.eventManager.sealsReleased)
		{
			this.campaignmanager.campaignPoints -= strategicValue * OptionsManager.difficultySettings["WinMissionModifier"];
			this.campaignmanager.playercampaigndata.patrolPoints -= OptionsManager.difficultySettings["WinMissionModifier"];
			this.campaignmanager.eventManager.missionPassed = true;
		}
		else
		{
			this.campaignmanager.campaignPoints += strategicValue * OptionsManager.difficultySettings["FailMissionModifier"];
			this.campaignmanager.playercampaigndata.patrolPoints += strategicValue * OptionsManager.difficultySettings["FailMissionModifier"];
			this.campaignmanager.eventManager.missionPassed = false;
		}
		float num = 0f;
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			if (GameDataManager.enemyvesselsonlevel[i].isSinking || GameDataManager.enemyvesselsonlevel[i].isCapsizing)
			{
				num += GameDataManager.enemyvesselsonlevel[i].databaseshipdata.displacement / 5000f;
			}
			else if (GameDataManager.enemyvesselsonlevel[i].damagesystem.shipCurrentDamagePoints > 0f)
			{
				num += GameDataManager.enemyvesselsonlevel[i].databaseshipdata.displacement / 10000f;
			}
		}
		this.campaignmanager.campaignPoints -= num * OptionsManager.difficultySettings["TonnageSunkModifier"];
		this.campaignmanager.playercampaigndata.patrolPoints -= num * OptionsManager.difficultySettings["TonnageSunkModifier"];
		if (this.campaignmanager.currentTaskForceEngagedWith == this.campaignmanager.playercampaigndata.currentMissionTaskForceID)
		{
			if (this.campaignmanager.eventManager.missionPassed)
			{
				this.campaignmanager.playercampaigndata.campaignStats[0] += 1f;
			}
			if (this.campaignmanager.playercampaigndata.playerMissionType == "INSERTION")
			{
				this.campaignmanager.playercampaigndata.campaignStats[2] += 1f;
			}
			if (this.campaignmanager.playercampaigndata.playerMissionType == "LAND_STRIKE")
			{
				this.campaignmanager.playercampaigndata.campaignStats[1] += 1f;
			}
		}
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x000980A0 File Offset: 0x000962A0
	private void SetSecondaryTitles()
	{
		if (!GameDataManager.trainingMode && !GameDataManager.missionMode)
		{
			if (!this.campaignmanager.playerInPort)
			{
				Text text = this.secondaryTitle;
				text.text += LanguageManager.interfaceDictionary["InTheVicinityOf"];
				Text text2 = this.secondaryTitle;
				text2.text = text2.text + " " + this.campaignmanager.GetNearestLocationToPlayer();
			}
			else
			{
				Text text3 = this.secondaryTitle;
				text3.text += LanguageManager.interfaceDictionary["DockedAtPort"];
				string text4 = string.Empty;
				for (int i = 0; i < this.campaignmanager.campaignlocations.Length; i++)
				{
					for (int j = 0; j < this.campaignmanager.campaignlocations[i].function.Count; j++)
					{
						if (this.campaignmanager.campaignlocations[i].function[j] == "PLAYER_BASE")
						{
							text4 = this.campaignmanager.campaignlocations[i].locationName;
						}
					}
					if (text4 != string.Empty)
					{
						break;
					}
				}
				this.secondaryTitle.text = this.secondaryTitle.text.Replace("<PLAYERPORT>", text4);
			}
			this.dateTitle.text = this.campaignmanager.dateDisplay.text;
			Text text5 = this.dateTitle;
			string text6 = text5.text;
			text5.text = string.Concat(new string[]
			{
				text6,
				"    ",
				this.campaignmanager.timeDisplay.text,
				" ",
				LanguageManager.interfaceDictionary["Hours"]
			});
		}
		else
		{
			Text text7 = this.secondaryTitle;
			text7.text += this.levelloadmanager.levelloaddata.combatLocation;
			this.dateTitle.text = this.levelloadmanager.levelloaddata.date;
			Text text8 = this.dateTitle;
			string text6 = text8.text;
			text8.text = string.Concat(new string[]
			{
				text6,
				"    ",
				this.levelloadmanager.levelloaddata.timeOfDayString,
				" ",
				LanguageManager.interfaceDictionary["Hours"]
			});
		}
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x00098310 File Offset: 0x00096510
	private string[] GetEngagedContactsList(bool playingCampaign)
	{
		string[] array = new string[]
		{
			null,
			"\n\n\n\n\n\n\n"
		};
		bool flag = false;
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			if (GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipType != "BIOLOGIC" && GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipType != "OILRIG" && !GameDataManager.enemyvesselsonlevel[i].vesselRemovedFromCombat && !Database.GetIsCivilian(GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipPrefabName))
			{
				string[] array2 = array;
				int num = 0;
				array2[num] = array2[num] + "\t\t" + GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipclass.ToUpper() + "\n";
				string[] array3 = array;
				int num2 = 1;
				array3[num2] = array3[num2] + this.GetShipStatus(GameDataManager.enemyvesselsonlevel[i], i, playingCampaign) + "\n";
				flag = true;
			}
		}
		if (!flag)
		{
			string[] array4 = array;
			int num3 = 0;
			array4[num3] = array4[num3] + "\t" + LanguageManager.interfaceDictionary["ActionReportNone"] + "\n";
		}
		if (playingCampaign)
		{
			this.CalculateNeutralCasualties();
		}
		return array;
	}

	// Token: 0x06000AED RID: 2797 RVA: 0x00098444 File Offset: 0x00096644
	private string GetPlayerWeapons()
	{
		string text = "\t";
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		for (int i = 0; i < GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.currentTorpsOnBoard.Length; i++)
		{
			dictionary.Add(GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.torpedoTypes[i], GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.currentTorpsOnBoard[i]);
		}
		if (GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.hasVLS)
		{
			for (int j = 0; j < GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.vlsCurrentTorpsOnBoard.Length; j++)
			{
				if (dictionary.ContainsKey(GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.vlsTorpedoTypes[j]))
				{
					Dictionary<int, int> dictionary3;
					Dictionary<int, int> dictionary2 = dictionary3 = dictionary;
					int num;
					int key = num = GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.vlsTorpedoTypes[j];
					num = dictionary3[num];
					dictionary2[key] = num + GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.vlsCurrentTorpsOnBoard[j];
				}
				else
				{
					dictionary.Add(GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.vlsTorpedoTypes[j], GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.vlsCurrentTorpsOnBoard[j]);
				}
			}
		}
		foreach (KeyValuePair<int, int> keyValuePair in dictionary)
		{
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"     ",
				keyValuePair.Value,
				" ",
				this.database.databaseweapondata[keyValuePair.Key].weaponName
			});
		}
		text += "\n";
		return text;
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x00098654 File Offset: 0x00096854
	private string GetPlayerSystems()
	{
		string text = string.Empty;
		int num = 0;
		for (int i = 0; i < this.playerfunctions.damagecontrol.damageControlCurrentTimers.Length; i++)
		{
			if (this.playerfunctions.damagecontrol.damageControlCurrentTimers[i] == 10000f)
			{
				text = text + "\t" + this.database.databasesubsystemsdata[i].subsystemName.ToUpper() + "\n";
				num++;
			}
		}
		if (num == 0)
		{
			text = "\t" + LanguageManager.interfaceDictionary["ActionReportNone"] + "\n";
		}
		return text;
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x000986FC File Offset: 0x000968FC
	private float GetNumberOfMissionCriticalShips()
	{
		int num = 0;
		for (int i = 0; i < this.levelloadmanager.levelloaddata.enemyUnitMissionCritical.Length; i++)
		{
			if (this.levelloadmanager.levelloaddata.enemyUnitMissionCritical[i])
			{
				num++;
			}
		}
		return (float)num;
	}

	// Token: 0x06000AF0 RID: 2800 RVA: 0x0009874C File Offset: 0x0009694C
	public void ResetMainCamera()
	{
		this.MainCamera.GetComponent<Camera>().depth = 0f;
		this.MainCamera.GetComponent<Camera>().rect = new Rect(0f, 0f, 1f, 1f);
		this.MainCamera.GetComponent<Camera>().fieldOfView = GameDataManager.mainCameraFOV;
	}

	// Token: 0x06000AF1 RID: 2801 RVA: 0x000987AC File Offset: 0x000969AC
	public void BackToMain()
	{
		this.ClearDefaultTextBox();
		this.SetMenuSystem("MAINMENU");
		this.scrollbarDefault.gameObject.SetActive(false);
		Time.timeScale = 1f;
	}

	// Token: 0x06000AF2 RID: 2802 RVA: 0x000987E8 File Offset: 0x000969E8
	public void WaveSoundOff()
	{
		this.oceansound.Stop();
		this.underwatersound.Stop();
		this.combatmusic.Stop();
	}

	// Token: 0x06000AF3 RID: 2803 RVA: 0x0009880C File Offset: 0x00096A0C
	public void ZoomViewButton()
	{
		this.ZoomView();
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x00098814 File Offset: 0x00096A14
	public void ZoomView()
	{
		if (ManualCameraZoom.underwater)
		{
			this.levelloadmanager.EnvironmentSwitch(false);
		}
		if (ManualCameraZoom.binoculars && !GameDataManager.playervesselsonlevel[GameDataManager.activePlayerSlot].isSinking)
		{
			this.SetFOV();
			Transform transform = GameDataManager.playervesselsonlevel[GameDataManager.activePlayerSlot].transform;
			this.playerfunctions.helmmanager.SetPeriscopeMask(true);
			this.MainCamera.GetComponent<Camera>().nearClipPlane = 0.2f;
			this.MainCamera.GetComponent<Camera>().farClipPlane = 50000f;
			this.cameraMount.transform.parent = GameDataManager.playervesselsonlevel[GameDataManager.activePlayerSlot].submarineFunctions.periscopeCameraMount;
			this.cameraMount.transform.localPosition = Vector3.zero;
			this.MainCamera.transform.localPosition = Vector3.zero;
			this.MainCamera.transform.localRotation = Quaternion.identity;
			ManualCameraZoom.cameraMount = this.cameraMount;
			ManualCameraZoom.binoculars = true;
			this.weatherholder.transform.localPosition = new Vector3(0f, 0f, 2.5f);
		}
		else
		{
			ManualCameraZoom.target = GameDataManager.playervesselsonlevel[GameDataManager.activePlayerSlot].transform;
			this.cameraMount.transform.parent = null;
			this.MainCamera.GetComponent<Camera>().fieldOfView = GameDataManager.mainCameraFOV;
			this.MainCamera.GetComponent<Camera>().farClipPlane = 500f;
			this.playerfunctions.helmmanager.SetPeriscopeMask(false);
			if (ManualCameraZoom.underwater)
			{
				this.tapeBearing.SetColor("_Color", Color.white);
				this.bearingMarker.color = Color.white;
			}
			else if (!GameDataManager.isNight)
			{
				this.tapeBearing.SetColor("_Color", Color.black);
				this.bearingMarker.color = Color.black;
			}
			else
			{
				this.tapeBearing.SetColor("_Color", Color.white);
				this.bearingMarker.color = Color.white;
			}
			this.MainCamera.GetComponent<Camera>().nearClipPlane = 0.2f;
			ManualCameraZoom.cameraMount = null;
			ManualCameraZoom.binoculars = false;
			this.weatherholder.transform.localPosition = new Vector3(0f, 0f, 0.5f);
		}
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x00098A7C File Offset: 0x00096C7C
	public void CancelZoom()
	{
		ManualCameraZoom.binoculars = false;
		this.playerfunctions.binocularZoomText.gameObject.SetActive(false);
		this.periscopeMatMask.SetActive(false);
		this.ZoomView();
	}

	// Token: 0x06000AF6 RID: 2806 RVA: 0x00098AB8 File Offset: 0x00096CB8
	public void SetFOV()
	{
		switch (this.playerfunctions.binocularZoomLevel)
		{
		case 0:
			this.MainCamera.GetComponent<Camera>().fieldOfView = GameDataManager.mainCameraFOV / 2f;
			this.playerfunctions.binocularZoomText.text = "2x";
			break;
		case 1:
			this.MainCamera.GetComponent<Camera>().fieldOfView = GameDataManager.mainCameraFOV / 4f;
			this.playerfunctions.binocularZoomText.text = "4x";
			break;
		case 2:
			this.MainCamera.GetComponent<Camera>().fieldOfView = GameDataManager.mainCameraFOV / 8f;
			this.playerfunctions.binocularZoomText.text = "8x";
			break;
		case 3:
			this.MainCamera.GetComponent<Camera>().fieldOfView = GameDataManager.mainCameraFOV / 16f;
			this.playerfunctions.binocularZoomText.text = "16x";
			break;
		}
	}

	// Token: 0x06000AF7 RID: 2807 RVA: 0x00098BC0 File Offset: 0x00096DC0
	public void SetBearingToView()
	{
		float num = this.MainCamera.transform.eulerAngles.y - GameDataManager.playervesselsonlevel[GameDataManager.activePlayerSlot].transform.eulerAngles.y;
		if (num > 360f)
		{
			num -= 360f;
		}
		else if (num < 0f)
		{
			num += 360f;
		}
		if (num > 180f)
		{
			num -= 360f;
		}
		num /= 767.26f;
	}

	// Token: 0x06000AF8 RID: 2808 RVA: 0x00098C4C File Offset: 0x00096E4C
	public void SetPlayerErrorMessage(string message)
	{
		this.playerErrorMessage.text = message;
		this.playerErrorMessage.transform.parent.gameObject.SetActive(true);
		this.SetMenuSystem("MAINMENU");
	}

	// Token: 0x06000AF9 RID: 2809 RVA: 0x00098C8C File Offset: 0x00096E8C
	public void SetBackgroundImageSprite(string imageName)
	{
		this.backgroundImage.sprite = Resources.Load<Sprite>("background/" + imageName);
		this.backgroundImage.gameObject.SetActive(true);
	}

	// Token: 0x06000AFA RID: 2810 RVA: 0x00098CC8 File Offset: 0x00096EC8
	public void OpenKillerfish()
	{
		Application.OpenURL("http://killerfishgames.com/");
	}

	// Token: 0x06000AFB RID: 2811 RVA: 0x00098CD4 File Offset: 0x00096ED4
	private void OpenKillerfishFBPage()
	{
		Application.OpenURL("https://www.facebook.com/pages/Killerfish-Games/297301143813883/");
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x00098CE0 File Offset: 0x00096EE0
	private void OpenKillerfishYouTube()
	{
		Application.OpenURL("https://www.youtube.com/channel/UCv9mYr1v_58ndYO4xA9vBQw/");
	}

	// Token: 0x040010DD RID: 4317
	public static UIFunctions globaluifunctions;

	// Token: 0x040010DE RID: 4318
	public static LanguageManager languagemanager;

	// Token: 0x040010DF RID: 4319
	public PlayerFunctions playerfunctions;

	// Token: 0x040010E0 RID: 4320
	public CampaignManager campaignmanager;

	// Token: 0x040010E1 RID: 4321
	public InGameReference ingamereference;

	// Token: 0x040010E2 RID: 4322
	public GameDataManager gamedatamanager;

	// Token: 0x040010E3 RID: 4323
	public LevelLoadManager levelloadmanager;

	// Token: 0x040010E4 RID: 4324
	public VesselBuilder vesselbuilder;

	// Token: 0x040010E5 RID: 4325
	public OptionsManager optionsmanager;

	// Token: 0x040010E6 RID: 4326
	public MissionManager missionmanager;

	// Token: 0x040010E7 RID: 4327
	public HelpManager helpmanager;

	// Token: 0x040010E8 RID: 4328
	public Database database;

	// Token: 0x040010E9 RID: 4329
	public CombatAI combatai;

	// Token: 0x040010EA RID: 4330
	public SkyObjectCenterer skyobjectcenterer;

	// Token: 0x040010EB RID: 4331
	public TextParser textparser;

	// Token: 0x040010EC RID: 4332
	public int levelID;

	// Token: 0x040010ED RID: 4333
	public PortRearm portRearm;

	// Token: 0x040010EE RID: 4334
	public PortRepair portRepair;

	// Token: 0x040010EF RID: 4335
	public ParticleField particlefield;

	// Token: 0x040010F0 RID: 4336
	public GameObject menuSystemParent;

	// Token: 0x040010F1 RID: 4337
	public GameObject[] menuSystem;

	// Token: 0x040010F2 RID: 4338
	public GameObject levelLoadMessage;

	// Token: 0x040010F3 RID: 4339
	public int torpedoType;

	// Token: 0x040010F4 RID: 4340
	public int fireTorpedoes;

	// Token: 0x040010F5 RID: 4341
	public GameObject torpedoLine;

	// Token: 0x040010F6 RID: 4342
	public static string USNactionText;

	// Token: 0x040010F7 RID: 4343
	private Vector3 moveVec;

	// Token: 0x040010F8 RID: 4344
	private Vector3 currentPosition;

	// Token: 0x040010F9 RID: 4345
	public GameObject GUICamera;

	// Token: 0x040010FA RID: 4346
	public GameObject HUDCamera;

	// Token: 0x040010FB RID: 4347
	public GameObject MainCamera;

	// Token: 0x040010FC RID: 4348
	public bool gameloaded;

	// Token: 0x040010FD RID: 4349
	public bool dismisscompletebutton;

	// Token: 0x040010FE RID: 4350
	public bool bringinHUD;

	// Token: 0x040010FF RID: 4351
	public GameObject HUDholder;

	// Token: 0x04001100 RID: 4352
	public Material guimaterial;

	// Token: 0x04001101 RID: 4353
	public Material hudmaterial;

	// Token: 0x04001102 RID: 4354
	public Material skyboxmaterial;

	// Token: 0x04001103 RID: 4355
	public Camera GUICameraObject;

	// Token: 0x04001104 RID: 4356
	public Camera HUDCameraObject;

	// Token: 0x04001105 RID: 4357
	public AudioSource music;

	// Token: 0x04001106 RID: 4358
	public AudioSource oceansound;

	// Token: 0x04001107 RID: 4359
	public AudioSource underwatersound;

	// Token: 0x04001108 RID: 4360
	public AudioSource combatmusic;

	// Token: 0x04001109 RID: 4361
	public AudioClip[] musicScores;

	// Token: 0x0400110A RID: 4362
	public Transform directionFinder;

	// Token: 0x0400110B RID: 4363
	public GameObject periscopeMask;

	// Token: 0x0400110C RID: 4364
	public Transform cameraMount;

	// Token: 0x0400110D RID: 4365
	public GameObject[] damageControlIcons;

	// Token: 0x0400110E RID: 4366
	public Transform[] damageControlPosition;

	// Token: 0x0400110F RID: 4367
	public Image bearingMarker;

	// Token: 0x04001110 RID: 4368
	public GameObject tapeBearingGameObject;

	// Token: 0x04001111 RID: 4369
	public Material tapeBearing;

	// Token: 0x04001112 RID: 4370
	public Transform weatherholder;

	// Token: 0x04001113 RID: 4371
	public Transform prePlacedObjects;

	// Token: 0x04001114 RID: 4372
	public GameObject[] lightningObjects;

	// Token: 0x04001115 RID: 4373
	public float lightningtimer;

	// Token: 0x04001116 RID: 4374
	public float lightningcount;

	// Token: 0x04001117 RID: 4375
	public GameObject damageReportIcon;

	// Token: 0x04001118 RID: 4376
	public GameObject disengageButton;

	// Token: 0x04001119 RID: 4377
	public GameObject campaignSelectScreen;

	// Token: 0x0400111A RID: 4378
	public KeybindManager keybindManager;

	// Token: 0x0400111B RID: 4379
	public KeybindManagerMuseum keybindManagerMuseum;

	// Token: 0x0400111C RID: 4380
	public KeybindManagerMenu keybindManagerMenu;

	// Token: 0x0400111D RID: 4381
	public Text mainTitle;

	// Token: 0x0400111E RID: 4382
	public Text secondaryTitle;

	// Token: 0x0400111F RID: 4383
	public Text dateTitle;

	// Token: 0x04001120 RID: 4384
	public Text mainColumn;

	// Token: 0x04001121 RID: 4385
	public Text secondColumm;

	// Token: 0x04001122 RID: 4386
	public Scrollbar scrollbarDefault;

	// Token: 0x04001123 RID: 4387
	public GameObject backgroundTemplate;

	// Token: 0x04001124 RID: 4388
	public GameObject backgroundImagesOnly;

	// Token: 0x04001125 RID: 4389
	public Image backgroundImage;

	// Token: 0x04001126 RID: 4390
	public GameObject afterActionReport;

	// Token: 0x04001127 RID: 4391
	public GameObject museumObject;

	// Token: 0x04001128 RID: 4392
	public GameObject selectionGroupPanel;

	// Token: 0x04001129 RID: 4393
	public GameObject[] selectionBars;

	// Token: 0x0400112A RID: 4394
	public ToggleGroup selectionToggleGroup;

	// Token: 0x0400112B RID: 4395
	public Text selectionGroupText;

	// Token: 0x0400112C RID: 4396
	public float totalMissionPointsEarned;

	// Token: 0x0400112D RID: 4397
	public float totalCriticalMissionPointsEarned;

	// Token: 0x0400112E RID: 4398
	public Text playerErrorMessage;

	// Token: 0x0400112F RID: 4399
	public ScreenOverlay guiCameraOverlay;

	// Token: 0x04001130 RID: 4400
	public ScreenOverlay tacmapCameraOverlay;

	// Token: 0x04001131 RID: 4401
	public Bloom guiCameraBloom;

	// Token: 0x04001132 RID: 4402
	public Bloom tacmapCameraBloom;

	// Token: 0x04001133 RID: 4403
	public Material movieMaterial;

	// Token: 0x04001134 RID: 4404
	public MovieTexture movietexture;

	// Token: 0x04001135 RID: 4405
	public MovieTexture creditMovietexture;

	// Token: 0x04001136 RID: 4406
	public GameObject movieObject;

	// Token: 0x04001137 RID: 4407
	public GameObject periscopeMatMask;

	// Token: 0x04001138 RID: 4408
	public Text versionText;

	// Token: 0x04001139 RID: 4409
	public GameObject loadingMask;
}
