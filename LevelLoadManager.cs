using System;
using System.Collections.Generic;
using AmplifyBloom;
using Ceto;
using mset;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000132 RID: 306
public class LevelLoadManager : MonoBehaviour
{
	// Token: 0x0600083F RID: 2111 RVA: 0x00053F54 File Offset: 0x00052154
	private void SetupCombat()
	{
		UIFunctions.globaluifunctions.textparser.BuildHUD(UIFunctions.globaluifunctions.textparser.GetFilePathFromString(UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].playerHUD));
		UIFunctions.globaluifunctions.playerfunctions.signatureMaterials[1].SetTexture("_Signature", UIFunctions.globaluifunctions.textparser.GetTexture(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("vessels/signature/blank_sig.png")));
		for (int i = 0; i < UIFunctions.globaluifunctions.playerfunctions.messageLogAlphas.Length; i++)
		{
			UIFunctions.globaluifunctions.playerfunctions.messageLogAlphas[i] = 0.01f;
		}
		this.missionmanager.abandonedShip = false;
		this.missionmanager.playerSunk = false;
		this.playerfunctions.periscopeNightVisionLight.enabled = false;
		this.sensormanager.tacticalmap.waypointReadout.text = string.Empty;
		PlayerFunctions.runningSilent = false;
		this.playerfunctions.runSilentButton.interactable = PlayerFunctions.runningSilent;
		this.playerfunctions.currentAircraftIndex = -1;
		this.playerfunctions.currentWeaponIndex = -1;
		this.playerfunctions.binocularZoomText.gameObject.SetActive(false);
		SensorManager.playerKnuckle = false;
		GameDataManager.cameraTimeScale = 1f;
		this.playerfunctions.sensormanager.cameraCurrentTargetIndex = -1;
		this.playerfunctions.currentTargetIndex = -1;
		this.sensormanager.InitialiseSensorManager();
		UIFunctions.globaluifunctions.periscopeMatMask.SetActive(false);
		for (int j = 0; j < this.playerfunctions.messageLog.Length; j++)
		{
			this.playerfunctions.messageLog[j].text = string.Empty;
		}
		StatusScreens.conditionsEnabled = false;
		TacticalMap.tacMapEnabled = false;
		this.playerfunctions.sensormanager.tacticalmap.minimapIsOpen = true;
		this.playerfunctions.statusscreens.currentScreen = -1;
		this.playerfunctions.weaponRange = 5000f;
		this.uifunctions.lightningtimer = UnityEngine.Random.Range(0.5f, 120f);
		ManualCameraZoom.followingAircraft = false;
		ManualCameraZoom.sensitivity = GameDataManager.camerasensitivity * 0.9f + 0.1f;
		ManualCameraZoom.previousTarget = null;
		Resources.UnloadUnusedAssets();
		this.uifunctions.ResetMainCamera();
		this.MainCamera.GetComponent<Camera>().rect = new Rect(0f, 0f, 1f, 1f);
		this.MainCamera.GetComponent<Camera>().depth = 0f;
		GameDataManager.HUDActive = true;
		this.uifunctions.menuSystemParent.SetActive(false);
		if (!GameDataManager.optionsBoolSettings[6])
		{
			this.amplifycoloreffect.enabled = false;
		}
		else
		{
			this.amplifycoloreffect.enabled = true;
		}
		if (!GameDataManager.optionsBoolSettings[7])
		{
			this.amplifyocclusioneffect.enabled = false;
		}
		else
		{
			this.amplifyocclusioneffect.enabled = true;
		}
		if (!GameDataManager.optionsBoolSettings[8])
		{
			this.amplifybloom.enabled = false;
		}
		else
		{
			this.amplifybloom.enabled = true;
		}
		this.lowWind = false;
	}

	// Token: 0x06000840 RID: 2112 RVA: 0x0005428C File Offset: 0x0005248C
	public void LoadLevel()
	{
		UIFunctions.globaluifunctions.cameraMount.gameObject.SetActive(true);
		UIFunctions.globaluifunctions.museumObject.SetActive(false);
		UIFunctions.globaluifunctions.MainCamera.GetComponent<ManualCameraZoom>().enabled = true;
		for (int i = 0; i < GameDataManager.playervesselsonlevel.Length; i++)
		{
			if (GameDataManager.playervesselsonlevel[i] != null)
			{
				UnityEngine.Object.Destroy(GameDataManager.playervesselsonlevel[i].gameObject);
			}
		}
		for (int j = 0; j < GameDataManager.enemyvesselsonlevel.Length; j++)
		{
			if (GameDataManager.enemyvesselsonlevel[j] != null)
			{
				UnityEngine.Object.Destroy(GameDataManager.enemyvesselsonlevel[j].gameObject);
			}
		}
		Time.timeScale = 0f;
		this.SetupCombat();
		this.BuildWorldObjectsDictionary();
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlayObject.GetComponent<MeshRenderer>().enabled = false;
		if (!GameDataManager.missionMode && !GameDataManager.trainingMode && CampaignManager.isEncounter)
		{
			string elevationMapDataPath = this.uifunctions.campaignmanager.elevationMapDataPath;
			Vector2 vector = this.uifunctions.campaignmanager.GetCombatMapCoords();
			vector.x = Mathf.Floor(vector.x);
			vector.y = Mathf.Floor(vector.y);
			if (this.levelloaddata.usePresetPositions)
			{
				vector = this.levelloaddata.mapPosition;
			}
			this.levelloaddata.mapPosition = vector;
			this.SetupTerrain(vector, elevationMapDataPath, GameDataManager.optionsBoolSettings[4]);
		}
		else
		{
			string mapElevationData = this.levelloaddata.mapElevationData;
			Vector2 mapPosition = this.levelloaddata.mapPosition;
			bool terrainOn = true;
			if (!this.levelloaddata.useTerrain || !GameDataManager.optionsBoolSettings[4])
			{
				terrainOn = false;
			}
			this.SetupTerrain(mapPosition, mapElevationData, terrainOn);
		}
		if (GameDataManager.missionMode || GameDataManager.trainingMode)
		{
			this.spawnObjects[0].transform.localRotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
			this.spawnObjects[1].transform.localRotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 180f), 0f);
			this.spawnObjects[2].transform.localRotation = Quaternion.Euler(0f, UnityEngine.Random.Range(180f, 360f), 0f);
		}
		else
		{
			this.spawnObjects[0].transform.localRotation = Quaternion.Euler(0f, this.levelloaddata.globalSpawnRotation, 0f);
			this.spawnObjects[1].transform.rotation = Quaternion.Euler(0f, this.levelloaddata.localSpawnRotation.x, 0f);
			this.spawnObjects[2].transform.rotation = Quaternion.Euler(0f, this.levelloaddata.localSpawnRotation.y, 0f);
		}
		float y = UnityEngine.Random.Range(0f, 360f);
		this.spawnObjects[6].transform.localRotation = Quaternion.Euler(0f, y, 0f);
		y = UnityEngine.Random.Range(0f, 360f);
		this.spawnObjects[8].transform.localRotation = Quaternion.Euler(0f, y, 0f);
		GameDataManager.activePlayerSlot = 0;
		this.environmenttype = this.GetEnvironment();
		GameDataManager.weathertype = this.environmenttype;
		this.CreateEnvironment(this.environmenttype);
		if (global::Environment.whiteLevel.r > 0.3f)
		{
			Material material = Resources.Load<Material>("periscope/periscope_material");
			material.SetTexture("_MainTex", UIFunctions.globaluifunctions.textparser.GetTexture(UIFunctions.globaluifunctions.textparser.GetFilePathFromString(this.periscopeMasks[0])));
			this.periscopeMaskRenderer.material = material;
		}
		else
		{
			Material material2 = Resources.Load<Material>("periscope/periscope_material");
			material2.SetTexture("_MainTex", UIFunctions.globaluifunctions.textparser.GetTexture(UIFunctions.globaluifunctions.textparser.GetFilePathFromString(this.periscopeMasks[1])));
			this.periscopeMaskRenderer.material = material2;
		}
		this.sensormanager.InitialiseOceanCharacteristics();
		UIFunctions.globaluifunctions.particlefield.InitialiseUnderWaterParticles();
		UIFunctions.globaluifunctions.particlefield.gameObject.SetActive(GameDataManager.optionsBoolSettings[14]);
		GameDataManager.playerNumberofShips = 1;
		GameDataManager.enemyNumberofShips = this.levelloaddata.kmShipClasses.Length;
		if (GameDataManager.enemyNumberofShips > GameDataManager.maxShips)
		{
			GameDataManager.enemyNumberofShips = GameDataManager.maxShips;
		}
		GameDataManager.playervesselsonlevel = new Vessel[GameDataManager.playerNumberofShips];
		this.shipSlots = this.levelloaddata.rnSlots;
		GameObject gameObject = this.SpawnPlayerVessel(0, UIFunctions.globaluifunctions.playerfunctions.playerVesselClass);
		Vessel vessel = GameDataManager.playervesselsonlevel[0];
		vessel.vesselmovement.weaponSource.InitialiseWeaponSource();
		vessel.vesselmovement.InitialiseVesselMovement();
		GameDataManager.CurrentAction = 0;
		GameDataManager.enemysunk = 0;
		GameDataManager.playersunk = 0;
		if (GameDataManager.isNight)
		{
			this.hudmaterial.color = new Color(0.351f, 0.214f, 0.214f, 1f);
		}
		GameDataManager.CurrentAction = -1;
		this.shipSlots = this.GetShipSlotNumbers();
		this.levelloaddata.AssignNeutralVessels();
		this.BuildWorldVessels(this.levelloaddata.mapPosition);
		GameDataManager.enemyvesselsonlevel = new Vessel[GameDataManager.enemyNumberofShips];
		for (int k = 0; k < this.levelloaddata.kmShipClasses.Length; k++)
		{
			this.SpawnShip(k);
		}
		UIFunctions.globaluifunctions.missionmanager.playerWasBehind = false;
		if (!this.levelloaddata.usePresetPositions && CampaignManager.isEncounter)
		{
			float bearingToTransform = UIFunctions.globaluifunctions.GetBearingToTransform(GameDataManager.enemyvesselsonlevel[0].transform, GameDataManager.playervesselsonlevel[0].transform);
			if (bearingToTransform < -80f || bearingToTransform > 80f)
			{
				UIFunctions.globaluifunctions.missionmanager.ForceConvergingCourses();
			}
		}
		float y2 = this.windVane[0].transform.eulerAngles.y;
		this.oceanObjectInstance.transform.rotation = Quaternion.Euler(0f, y2 - 90f, 0f);
		GameDataManager.smokeAngle = y2 + 90f;
		this.waveclockx.xspeed = -25f - this.windstrength * 500f;
		this.waveclocky.yspeed = 35f + this.windstrength * 500f;
		this.waveclockx.xlength = 1f + this.windstrength * 100f;
		this.waveclocky.ylength = 1f + this.windstrength * 200f;
		this.uifunctions.skyobjectcenterer.ForceSkyboxPosition(gameObject.transform);
		ManualCameraZoom.y = -3f;
		ManualCameraZoom.distance = 2f;
		ManualCameraZoom.minDistance = this.uifunctions.database.databaseshipdata[GameDataManager.playervesselsonlevel[0].shipID].minCameraDistance;
		UIFunctions.globaluifunctions.MainCamera.transform.position = new Vector3(UIFunctions.globaluifunctions.MainCamera.transform.position.x, 999.5f, UIFunctions.globaluifunctions.MainCamera.transform.position.z);
		this.tacticalmap.TacticalMapInit();
		this.tapeColor = Color.black;
		if (GameDataManager.isNight)
		{
			this.tapeColor = Color.white;
		}
		this.uifunctions.tapeBearing.SetColor("_Color", this.tapeColor);
		this.uifunctions.bearingMarker.color = this.tapeColor;
		this.waveclockx.Start();
		this.waveclocky.Start();
		this.pv = GameDataManager.playervesselsonlevel;
		this.ev = GameDataManager.enemyvesselsonlevel;
		this.EnvironmentSwitch(true);
		this.EnvironmentSwitch(false);
		this.EnvironmentSwitch(true);
		foreach (Vessel vessel2 in GameDataManager.playervesselsonlevel)
		{
			vessel2.vesselPositionHistory = new Vector2[40];
			vessel2.vesselPositionHistory[0].x = vessel2.transform.position.x;
			vessel2.vesselPositionHistory[0].y = vessel2.transform.position.z;
		}
		foreach (Vessel vessel3 in GameDataManager.enemyvesselsonlevel)
		{
			vessel3.vesselPositionHistory = new Vector2[40];
			vessel3.vesselPositionHistory[0].x = vessel3.transform.position.x;
			vessel3.vesselPositionHistory[0].y = vessel3.transform.position.z;
		}
		this.playerfunctions.playerVessel = GameDataManager.playervesselsonlevel[0];
		this.playerfunctions.sensormanager.InitialiseSonarDisplay();
		this.playerfunctions.InitialiseWeapons();
		this.spawnObjects[2].transform.Translate(Vector3.forward * 40f);
		UIFunctions.globaluifunctions.combatai.formationHeading = this.spawnObjects[4].transform.eulerAngles.y;
		this.playerfunctions.damagecontrol.InitialisePlayerDamageControl();
		this.SetupFormationGrid();
		UIFunctions.globaluifunctions.combatai.CalculateFormationBounds();
		if (!this.packIcePresent)
		{
			this.SetupAircraft();
		}
		if (this.levelloaddata.usePresetPositions)
		{
			this.PresetLocations();
			this.sensormanager.initialRun = false;
		}
		for (int n = this.levelloaddata.wanderingNeutralsStartIndex; n < GameDataManager.enemyvesselsonlevel.Length - this.worldVessels.Count; n++)
		{
			if (GameDataManager.enemyvesselsonlevel[n].vesselmovement.flagRenderer != null)
			{
				GameDataManager.enemyvesselsonlevel[n].vesselmovement.flagRenderer.material = this.levelloaddata.neutralFlagsToSpawn[n - this.levelloaddata.wanderingNeutralsStartIndex];
				GameDataManager.enemyvesselsonlevel[n].vesselmovement.flagRenderer.material.color = global::Environment.whiteLevel;
			}
		}
		for (int num = 0; num < this.levelloaddata.wanderingNeutralsStartIndex; num++)
		{
			if (Database.GetIsCivilian(GameDataManager.enemyvesselsonlevel[num].databaseshipdata.shipPrefabName) && GameDataManager.enemyvesselsonlevel[num].vesselmovement.flagRenderer != null)
			{
				GameDataManager.enemyvesselsonlevel[num].vesselmovement.flagRenderer.material = (Resources.Load(this.levelloaddata.neutralMerchantFlags4[UnityEngine.Random.Range(0, this.levelloaddata.neutralMerchantFlags4.Count)]) as Material);
				GameDataManager.enemyvesselsonlevel[num].vesselmovement.flagRenderer.material.color = global::Environment.whiteLevel;
			}
		}
		this.PlaceWorldVessels(this.levelloaddata.mapPosition);
		this.missionmanager.BringInBriefing(true);
		this.submarineMarker.playerTransform = GameDataManager.playervesselsonlevel[0].transform;
		this.submarineMarker.LateUpdate();
		this.submarineMarker.gameObject.SetActive(GameDataManager.optionsBoolSettings[13]);
		this.playerfunctions.sensormanager.SelectTarget();
		this.playerfunctions.SetProfileGraphic();
		UIFunctions.globaluifunctions.combatai.InitialiseCombatAI();
		this.tacticalmap.SetTacticalMap();
		this.tacticalmap.SetTacticalMap();
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.tacMapCamera.gameObject.SetActive(false);
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.tacMapCamera.depth = 0f;
		PlayerFunctions.hudInMenu = true;
		UIFunctions.globaluifunctions.playerfunctions.otherPanel.SetActive(false);
		UIFunctions.globaluifunctions.playerfunctions.menuPanel.SetActive(false);
		UIFunctions.globaluifunctions.HUDholder.SetActive(true);
		UIFunctions.globaluifunctions.HUDCameraObject.enabled = true;
		this.uifunctions.bearingMarker.gameObject.SetActive(false);
		UIFunctions.globaluifunctions.cameraMount.gameObject.SetActive(false);
		AudioManager.audiomanager.SetCombatSounds(false);
		UIFunctions.globaluifunctions.playerfunctions.helmmanager.InitialiseHelmManager();
	}

	// Token: 0x06000841 RID: 2113 RVA: 0x00054F88 File Offset: 0x00053188
	public void BuildWorldObjectsDictionary()
	{
		this.sceneryDataList = new List<LevelLoadManager.SceneryData>();
		string[] array = UIFunctions.globaluifunctions.textparser.OpenTextDataFile(this.levelloaddata.worldObjectsData);
		LevelLoadManager.SceneryData sceneryData = null;
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			string text = array2[0];
			switch (text)
			{
			case "WorldObject":
				sceneryData = new LevelLoadManager.SceneryData();
				sceneryData.sceneryPath = array2[1].Trim();
				this.sceneryDataList.Add(sceneryData);
				sceneryData.childObjects = new List<string>();
				sceneryData.childObjectPositions = new List<Vector3>();
				sceneryData.childObjectRotations = new List<Vector3>();
				break;
			case "VesselObject":
				sceneryData = new LevelLoadManager.SceneryData();
				sceneryData.vesselPrefabName = array2[1].Trim();
				this.sceneryDataList.Add(sceneryData);
				break;
			case "WorldObjectCoords":
				sceneryData.sceneryCoords = UIFunctions.globaluifunctions.textparser.PopulateVector2(array2[1]);
				break;
			case "MeshPosition":
				sceneryData.sceneryMeshPosition = UIFunctions.globaluifunctions.textparser.PopulateVector3(array2[1]);
				break;
			case "MeshRotation":
				sceneryData.sceneryMeshRotation = UIFunctions.globaluifunctions.textparser.PopulateVector3(array2[1]);
				break;
			case "ChildObject":
				sceneryData.childObjects.Add(array2[1]);
				break;
			case "ChildMeshPosition":
				sceneryData.childObjectPositions.Add(UIFunctions.globaluifunctions.textparser.PopulateVector3(array2[1]));
				break;
			case "ChildMeshRotation":
				sceneryData.childObjectRotations.Add(UIFunctions.globaluifunctions.textparser.PopulateVector3(array2[1]));
				break;
			}
		}
	}

	// Token: 0x06000842 RID: 2114 RVA: 0x000551CC File Offset: 0x000533CC
	private void CheckForWorldObjects(Vector2 mapCoords)
	{
		List<Transform> list = new List<Transform>();
		this.levelloaddata.missileHitLocations = new Transform[10];
		this.levelloaddata.numberOfLandHits = 0;
		MapGenerator component = this.currentMapGeneratorInstance.gameObject.GetComponent<MapGenerator>();
		int[] array = new int[]
		{
			(int)mapCoords.x - component.sampleAreaSize / 2,
			(int)mapCoords.y - component.sampleAreaSize / 2
		};
		int[] array2 = new int[]
		{
			array[0] + component.sampleAreaSize,
			array[1] + component.sampleAreaSize
		};
		int num = 0;
		for (int i = 0; i < this.sceneryDataList.Count; i++)
		{
			if (this.sceneryDataList[i].vesselPrefabName == string.Empty && (int)this.sceneryDataList[i].sceneryCoords.x >= array[0] && (int)this.sceneryDataList[i].sceneryCoords.x <= array2[0] && (int)this.sceneryDataList[i].sceneryCoords.y >= array[1] && (int)this.sceneryDataList[i].sceneryCoords.y <= array2[1])
			{
				float num2 = 20f;
				float num3 = this.sceneryDataList[i].sceneryCoords.x - mapCoords.x;
				float num4 = this.sceneryDataList[i].sceneryCoords.y - mapCoords.y;
				num3 *= num2;
				num4 *= num2;
				string sceneryPath = this.sceneryDataList[i].sceneryPath;
				GameObject original = Resources.Load(UIFunctions.globaluifunctions.textparser.GetFilePathFromString(sceneryPath)) as GameObject;
				GameObject gameObject = UnityEngine.Object.Instantiate(original, new Vector3(num3 + this.sceneryDataList[i].sceneryMeshPosition.x, 1000f + this.sceneryDataList[i].sceneryMeshPosition.y, num4 + this.sceneryDataList[i].sceneryMeshPosition.z), Quaternion.Euler(this.sceneryDataList[i].sceneryMeshRotation.x, this.sceneryDataList[i].sceneryMeshRotation.y, this.sceneryDataList[i].sceneryMeshRotation.z)) as GameObject;
				gameObject.transform.SetParent(component.transform);
				gameObject.AddComponent<SimpleLOD>();
				this.lowWind = true;
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.PlaceHazardIconOnMap(gameObject.transform.position, 2);
				if (gameObject.transform.Find("trees") != null)
				{
					Material sharedMaterial = gameObject.transform.Find("trees").GetComponentInChildren<MeshRenderer>().sharedMaterial;
					if (sharedMaterial != null)
					{
						sharedMaterial.SetTexture("_MainTex", component.treeMaterial.GetTexture("_MainTex"));
					}
				}
				for (int j = 0; j < this.sceneryDataList[i].childObjects.Count; j++)
				{
					GameObject original2 = Resources.Load(UIFunctions.globaluifunctions.textparser.GetFilePathFromString(this.sceneryDataList[i].childObjects[j])) as GameObject;
					GameObject gameObject2 = UnityEngine.Object.Instantiate(original2, Vector3.zero, Quaternion.identity) as GameObject;
					gameObject2.transform.SetParent(gameObject.transform);
					gameObject2.transform.localPosition = this.sceneryDataList[i].childObjectPositions[j];
					gameObject2.transform.localRotation = Quaternion.Euler(this.sceneryDataList[i].childObjectRotations[j]);
					if (gameObject2.name == "landhit(Clone)")
					{
						gameObject2.name = "landhit";
					}
					else if (gameObject2.name == "SHIPDOCK(Clone)")
					{
						gameObject2.name = "SHIPDOCK";
					}
				}
				foreach (object obj in gameObject.transform)
				{
					Transform transform = (Transform)obj;
					if (transform.name.Contains("landhit"))
					{
						if (this.levelloaddata.numberOfLandHits < this.levelloaddata.missileHitLocations.Length)
						{
							this.levelloaddata.missileHitLocations[this.levelloaddata.numberOfLandHits] = transform;
							this.levelloaddata.numberOfLandHits++;
						}
					}
					else if (transform.name.Contains("SHIPDOCK"))
					{
						list.Add(transform);
						num++;
					}
				}
			}
		}
		this.levelloaddata.shipDockLocations = list.ToArray();
		for (int k = 0; k < this.levelloaddata.shipDockLocations.Length; k++)
		{
			Transform transform2 = this.levelloaddata.shipDockLocations[k];
			int num5 = UnityEngine.Random.Range(k, this.levelloaddata.shipDockLocations.Length);
			this.levelloaddata.shipDockLocations[k] = this.levelloaddata.shipDockLocations[num5];
			this.levelloaddata.shipDockLocations[num5] = transform2;
		}
		this.levelloaddata.numberOfLandHits = 0;
	}

	// Token: 0x06000843 RID: 2115 RVA: 0x000557A4 File Offset: 0x000539A4
	public void BuildWorldVessels(Vector2 mapCoords)
	{
		this.worldVessels = new List<LevelLoadManager.SceneryData>();
		if (this.levelloaddata.noOtherVessels)
		{
			return;
		}
		int[] array = new int[]
		{
			(int)mapCoords.x - 128,
			(int)mapCoords.y - 128
		};
		int[] array2 = new int[]
		{
			array[0] + 256,
			array[1] + 256
		};
		for (int i = 0; i < this.sceneryDataList.Count; i++)
		{
			if (this.sceneryDataList[i].vesselPrefabName != string.Empty && (int)this.sceneryDataList[i].sceneryCoords.x >= array[0] && (int)this.sceneryDataList[i].sceneryCoords.x <= array2[0] && (int)this.sceneryDataList[i].sceneryCoords.y >= array[1] && (int)this.sceneryDataList[i].sceneryCoords.y <= array2[1])
			{
				this.worldVessels.Add(this.sceneryDataList[i]);
			}
		}
		if (this.worldVessels.Count > 0)
		{
			int[] array3 = new int[this.levelloaddata.kmShipClasses.Length + this.worldVessels.Count];
			for (int j = 0; j < array3.Length; j++)
			{
				if (j < this.levelloaddata.kmShipClasses.Length)
				{
					array3[j] = this.levelloaddata.kmShipClasses[j];
				}
				else
				{
					array3[j] = UIFunctions.globaluifunctions.textparser.GetShipID(this.worldVessels[j - this.levelloaddata.kmShipClasses.Length].vesselPrefabName);
				}
			}
			this.levelloaddata.kmShipClasses = array3;
			GameDataManager.enemyNumberofShips = this.levelloaddata.kmShipClasses.Length;
		}
	}

	// Token: 0x06000844 RID: 2116 RVA: 0x000559A8 File Offset: 0x00053BA8
	private void PlaceWorldVessels(Vector2 mapCoords)
	{
		if (this.worldVessels.Count == 0)
		{
			return;
		}
		int num = this.levelloaddata.kmShipClasses.Length - this.worldVessels.Count;
		for (int i = 0; i < this.worldVessels.Count; i++)
		{
			float num2 = 20f;
			float num3 = this.worldVessels[i].sceneryCoords.x - mapCoords.x;
			float num4 = this.worldVessels[i].sceneryCoords.y - mapCoords.y;
			num3 *= num2;
			num4 *= num2;
			string sceneryPath = this.worldVessels[i].sceneryPath;
			GameDataManager.enemyvesselsonlevel[num].gameObject.transform.position = new Vector3(num3 + this.worldVessels[i].sceneryMeshPosition.x, 1000f + this.worldVessels[i].sceneryMeshPosition.y, num4 + this.worldVessels[i].sceneryMeshPosition.z);
			GameDataManager.enemyvesselsonlevel[num].gameObject.transform.rotation = Quaternion.Euler(this.worldVessels[i].sceneryMeshRotation.x, this.worldVessels[i].sceneryMeshRotation.y, this.worldVessels[i].sceneryMeshRotation.z);
			num++;
		}
	}

	// Token: 0x06000845 RID: 2117 RVA: 0x00055B28 File Offset: 0x00053D28
	private void SetupTerrain(Vector2 mapCoords, string elevationDataPath, bool terrainOn = true)
	{
		if (GameDataManager.trainingMode || GameDataManager.missionMode)
		{
			UIFunctions.globaluifunctions.campaignmanager.mapNavigation = UIFunctions.globaluifunctions.textparser.GetTexture(this.levelloaddata.mapNavigationData);
			this.levelloaddata.season = CalendarFunctions.GetSeason(this.levelloaddata.month, this.levelloaddata.hemisphere);
		}
		int[] array = new int[]
		{
			Mathf.FloorToInt(mapCoords.x / 16f),
			Mathf.FloorToInt(mapCoords.y / 16f)
		};
		float g = UIFunctions.globaluifunctions.campaignmanager.mapNavigation.GetPixel(array[0], array[1]).g;
		int num;
		if (g < 0.1f)
		{
			num = 0;
		}
		else if (g < 0.15f)
		{
			num = 1;
		}
		else if (g < 0.2f)
		{
			num = 2;
		}
		else if ((double)g < 0.28)
		{
			num = 3;
		}
		else if ((double)g < 0.35)
		{
			num = 4;
		}
		else if ((double)g < 0.42)
		{
			num = 5;
		}
		else if ((double)g < 0.5)
		{
			num = 7;
		}
		else
		{
			num = 7;
		}
		string biomename = this.GetBiomename(num);
		string text = this.levelloaddata.season;
		int num2 = 1;
		this.environmentTemperature = 1;
		if (text == "winter")
		{
			num2--;
			this.environmentTemperature = 0;
		}
		else if (text == "summer")
		{
			num2++;
			this.environmentTemperature = 2;
		}
		float num3 = UIFunctions.globaluifunctions.campaignmanager.mapNavigation.GetPixel(Mathf.FloorToInt((float)array[0]), Mathf.FloorToInt((float)array[1])).b;
		num3 = (1f - num3) * 8f;
		this.packIcePresent = false;
		this.icePresent = false;
		if (this.currentMapGeneratorInstance != null)
		{
			UnityEngine.Object.Destroy(this.currentMapGeneratorInstance);
		}
		if (terrainOn)
		{
			float a = UIFunctions.globaluifunctions.campaignmanager.mapNavigation.GetPixel(array[0], array[1]).a;
			float num4 = this.iceThresholds[num2];
			this.packIcePresent = false;
			if (a > num4)
			{
				this.icePresent = true;
				if (a > this.iceThresholds[3])
				{
					this.packIcePresent = true;
				}
				else if (text == "winter" && a > this.iceThresholds[2])
				{
					this.packIcePresent = true;
				}
				if (this.packIcePresent)
				{
					this.lowWind = true;
					for (int i = 0; i < this.levelloaddata.kmShipClasses.Length; i++)
					{
						if (UIFunctions.globaluifunctions.database.databaseshipdata[this.levelloaddata.kmShipClasses[i]].shipType != "SUBMARINE" && UIFunctions.globaluifunctions.database.databaseshipdata[this.levelloaddata.kmShipClasses[i]].shipType != "BIOLOGIC")
						{
							this.packIcePresent = false;
							break;
						}
					}
				}
			}
			this.currentMapGeneratorInstance = (UnityEngine.Object.Instantiate(this.mapGeneratorObject, new Vector3(72.5f, 1000f, 67.5f), Quaternion.identity) as GameObject);
			MapGenerator component = this.currentMapGeneratorInstance.gameObject.GetComponent<MapGenerator>();
			component.addTrees = GameDataManager.optionsBoolSettings[5];
			component.treeDensity = this.uifunctions.gamedatamanager.biomeTreeDensity[num];
			component.demMapCoods = mapCoords;
			component.InitialiseTerrain(elevationDataPath);
			EndlessTerrain component2 = this.currentMapGeneratorInstance.gameObject.GetComponent<EndlessTerrain>();
			component2.viewer = this.uifunctions.MainCamera.transform;
			component2.enabled = true;
			component2.InitialiseEndlessTerrain();
			this.levelloaddata.missileHitLocations = new Transform[0];
			if (component.gameObject.activeSelf && GameDataManager.optionsBoolSettings[16])
			{
				this.CheckForWorldObjects(mapCoords);
			}
			Material mapMaterial = component2.mapMaterial;
			Material treeMaterial = component.treeMaterial;
			treeMaterial.SetColor("_TintColor", global::Environment.treeColor);
			component.seaweedMaterial.SetColor("_TintColor", global::Environment.treeColor);
			string text2 = num.ToString() + "_" + biomename;
			if (num > 3)
			{
				text = "summer";
			}
			Debug.Log(string.Concat(new string[]
			{
				"terrain/",
				text2,
				"/detail_",
				biomename.ToLower(),
				"_",
				text
			}));
			mapMaterial.SetTexture("_Mask", UIFunctions.globaluifunctions.textparser.GetTexture(string.Concat(new string[]
			{
				"terrain/",
				text2,
				"/detail_",
				biomename.ToLower(),
				"_",
				text
			})));
			mapMaterial.SetTexture("_Grass", UIFunctions.globaluifunctions.textparser.GetTexture(string.Concat(new string[]
			{
				"terrain/",
				text2,
				"/grass_",
				biomename.ToLower(),
				"_",
				text
			})));
			mapMaterial.SetTexture("_Rock", UIFunctions.globaluifunctions.textparser.GetTexture(string.Concat(new string[]
			{
				"terrain/",
				text2,
				"/rock_",
				biomename.ToLower(),
				"_",
				text
			})));
			mapMaterial.SetTexture("_Rock_Detail", UIFunctions.globaluifunctions.textparser.GetTexture(string.Concat(new string[]
			{
				"terrain/",
				text2,
				"/rock_",
				biomename.ToLower(),
				"_",
				text,
				"_nm"
			})));
			mapMaterial.SetTexture("_Sand", UIFunctions.globaluifunctions.textparser.GetTexture(string.Concat(new string[]
			{
				"terrain/",
				text2,
				"/sand_",
				biomename.ToLower(),
				"_",
				text
			})));
			treeMaterial.SetTexture("_MainTex", UIFunctions.globaluifunctions.textparser.GetTexture(string.Concat(new string[]
			{
				"terrain/",
				text2,
				"/vegetation_",
				biomename.ToLower(),
				"_",
				text
			})));
			this.sceneryTreeMaterial.SetTexture("_MainTex", UIFunctions.globaluifunctions.textparser.GetTexture(string.Concat(new string[]
			{
				"terrain/",
				text2,
				"/vegetation_",
				biomename.ToLower(),
				"_",
				text
			})));
			UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlay = new Texture2D(component.scaledAreaSize, component.scaledAreaSize);
			for (int j = 0; j < component.scaledAreaSize; j++)
			{
				for (int k = 0; k < component.scaledAreaSize; k++)
				{
					Color pixel = DEMGenerator.combatZoneDEM.GetPixel(j, k);
					if (pixel.r < UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlayThresholds[0])
					{
						UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlay.SetPixel(j, k, Color.Lerp(pixel, UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlayWaterColor, UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlayLerps[2]));
					}
					else if (pixel.r < UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlayThresholds[1])
					{
						UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlay.SetPixel(j, k, Color.Lerp(pixel, UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlayCoastColor, UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlayLerps[1]));
					}
					else
					{
						UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlay.SetPixel(j, k, Color.Lerp(pixel, UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlayLandColor, UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlayLerps[0]));
					}
				}
			}
			UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlay.Apply();
			if (!UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlayFilter)
			{
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlay.filterMode = FilterMode.Point;
			}
			UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlayMaterial.SetTexture("_MainTex", UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlay);
			UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlayObject.GetComponent<MeshRenderer>().enabled = true;
		}
	}

	// Token: 0x06000846 RID: 2118 RVA: 0x000564E0 File Offset: 0x000546E0
	private string GetBiomename(int biomeNumber)
	{
		string result = string.Empty;
		switch (biomeNumber)
		{
		case 0:
			result = "Polar";
			break;
		case 1:
			result = "Tundra";
			break;
		case 2:
			result = "Taiga";
			break;
		case 3:
			result = "Grasslands";
			break;
		case 4:
			result = "Temperate";
			break;
		case 5:
			result = "Tropical";
			break;
		case 6:
			result = "Savanna";
			break;
		case 7:
			result = "Desert";
			break;
		}
		return result;
	}

	// Token: 0x06000847 RID: 2119 RVA: 0x0005657C File Offset: 0x0005477C
	public void SetupIcebergChunk(Transform terrainChunk)
	{
		int num = UnityEngine.Random.Range(0, 3);
		float num2 = 15f;
		int num3 = 0;
		int num4 = 6;
		float max = 1.1f;
		if (this.packIcePresent)
		{
			num *= 4;
			num3++;
			num4++;
			max = 1.3f;
		}
		for (int i = 0; i < num; i++)
		{
			string inputString = "terrain/Ice/iceberg_" + UnityEngine.Random.Range(num3, num4);
			if (this.packIcePresent && UnityEngine.Random.value < 0.5f)
			{
				inputString = "terrain/Ice/iceberg_" + (num4 - 1).ToString();
			}
			Vector3 localPosition = new Vector3(UnityEngine.Random.Range(-num2, num2), 0f, UnityEngine.Random.Range(-num2, num2));
			if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.GetUnitsDepthUnderTransform(base.transform) >= 2.5f)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load(UIFunctions.globaluifunctions.textparser.GetFilePathFromString(inputString)) as GameObject, Vector3.zero, Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f)) as GameObject;
				gameObject.name = "Ice";
				gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x * UnityEngine.Random.Range(0.8f, 1.2f), gameObject.transform.localScale.y * UnityEngine.Random.Range(0.9f, max), gameObject.transform.localScale.z * UnityEngine.Random.Range(0.8f, 1.2f));
				gameObject.transform.SetParent(terrainChunk);
				gameObject.transform.localPosition = localPosition;
				gameObject.transform.SetParent(this.currentMapGeneratorInstance.transform);
			}
		}
	}

	// Token: 0x06000848 RID: 2120 RVA: 0x0005676C File Offset: 0x0005496C
	public void CheckIcebergLandHits()
	{
	}

	// Token: 0x06000849 RID: 2121 RVA: 0x00056770 File Offset: 0x00054970
	public float GetTerrainHeightAtPositionFromHeightMap(float x, float z)
	{
		x += 2560f;
		z += 2560f;
		x /= 5f;
		z /= 5f;
		x = (float)Mathf.FloorToInt(x);
		z = (float)Mathf.FloorToInt(z);
		MapGenerator component = this.currentMapGeneratorInstance.gameObject.GetComponent<MapGenerator>();
		float num = DEMGenerator.combatZoneDEM.GetPixel((int)x, (int)z).r;
		num = component.meshHeightCurve.Evaluate(num);
		return num * component.transform.lossyScale.y;
	}

	// Token: 0x0600084A RID: 2122 RVA: 0x00056800 File Offset: 0x00054A00
	private void SetupAircraft()
	{
		this.uifunctions.combatai.enemyHelicopters = new Helicopter[0];
		this.uifunctions.combatai.enemyAircraft = new Aircraft[0];
		Vector3 position = this.spawnObjects[2].transform.position;
		position.y = 1000f;
		if (this.levelloaddata.numberOfHelicopters > 0)
		{
			this.uifunctions.combatai.enemyHelicopters = new Helicopter[this.levelloaddata.numberOfHelicopters];
			for (int i = 0; i < this.uifunctions.combatai.enemyHelicopters.Length; i++)
			{
				int aircraftID = this.uifunctions.textparser.GetAircraftID(this.levelloaddata.helicopterType);
				GameObject gameObject = this.uifunctions.vesselbuilder.CreateAircraft(aircraftID, position, GameDataManager.enemyvesselsonlevel[0].transform.rotation, true);
				Helicopter component = gameObject.GetComponent<Helicopter>();
				component.databaseaircraftdata = this.uifunctions.database.databaseaircraftdata[aircraftID];
				this.uifunctions.combatai.enemyHelicopters[i] = component;
				gameObject.transform.Translate(Vector3.up);
				component.cruisealtitude = 1002.4f + (float)i / 10f;
				component.defaultAltitude = component.cruisealtitude;
				component.currentaltitude = component.cruisealtitude;
				component.helicopterIndex = i;
				if (this.currentMapGeneratorInstance != null)
				{
					component.isTerrainPresent = true;
				}
			}
		}
		if (this.levelloaddata.numberOfHelicopters == 1)
		{
			this.uifunctions.combatai.enemyHelicopters[0].pathFinding = "FRONT";
		}
		else if (this.levelloaddata.numberOfHelicopters == 2)
		{
			this.uifunctions.combatai.enemyHelicopters[0].pathFinding = "FRONTLEFT";
			this.uifunctions.combatai.enemyHelicopters[1].pathFinding = "FRONTRIGHT";
		}
		else if (this.levelloaddata.numberOfHelicopters == 3)
		{
			this.uifunctions.combatai.enemyHelicopters[0].pathFinding = "FRONT";
			this.uifunctions.combatai.enemyHelicopters[1].pathFinding = "LEFT";
			this.uifunctions.combatai.enemyHelicopters[2].pathFinding = "RIGHT";
		}
		else if (this.levelloaddata.numberOfHelicopters == 4)
		{
			this.uifunctions.combatai.enemyHelicopters[0].pathFinding = "FRONTLEFT";
			this.uifunctions.combatai.enemyHelicopters[1].pathFinding = "FRONTRIGHT";
			this.uifunctions.combatai.enemyHelicopters[2].pathFinding = "LEFT";
			this.uifunctions.combatai.enemyHelicopters[3].pathFinding = "RIGHT";
		}
		for (int j = 0; j < this.uifunctions.combatai.enemyHelicopters.Length; j++)
		{
			this.uifunctions.combatai.enemyHelicopters[j].PlaceInitialWaypoint();
			this.uifunctions.combatai.enemyHelicopters[j].transform.position = new Vector3(this.uifunctions.combatai.enemyHelicopters[j].waypoint.position.x, this.uifunctions.combatai.enemyHelicopters[j].transform.position.y, this.uifunctions.combatai.enemyHelicopters[j].waypoint.position.z);
		}
		if (this.levelloaddata.numberOfAircraft > 0)
		{
			this.uifunctions.combatai.enemyAircraft = new Aircraft[this.levelloaddata.numberOfAircraft];
			for (int k = 0; k < this.uifunctions.combatai.enemyAircraft.Length; k++)
			{
				int aircraftID2 = this.uifunctions.textparser.GetAircraftID(this.levelloaddata.aircraftType);
				GameObject gameObject2 = this.uifunctions.vesselbuilder.CreateAircraft(aircraftID2, position, GameDataManager.enemyvesselsonlevel[0].transform.rotation, false);
				Aircraft component2 = gameObject2.GetComponent<Aircraft>();
				component2.databaseaircraftdata = this.uifunctions.database.databaseaircraftdata[aircraftID2];
				this.uifunctions.combatai.enemyAircraft[k] = component2;
				component2.aircraftInstanceId = k;
				component2.currentWaypoint = UnityEngine.Random.Range(0, 2);
				component2.waypoint.position = component2.GetNewFormationWaypoint();
				component2.transform.position = new Vector3(component2.waypoint.position.x, 1001f + 0.3f * (float)k, component2.waypoint.position.z);
				if (this.currentMapGeneratorInstance != null)
				{
					component2.isTerrainPresent = true;
				}
			}
		}
	}

	// Token: 0x0600084B RID: 2123 RVA: 0x00056D24 File Offset: 0x00054F24
	private void SetupFormationGrid()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(this.levelloaddata.formationPrefab, this.spawnObjects[2].transform.position, GameDataManager.enemyvesselsonlevel[0].transform.rotation) as GameObject;
		this.levelloaddata.formationGrid = gameObject;
		this.levelloaddata.formationPositions = new GameObject[GameDataManager.enemyvesselsonlevel.Length];
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(this.levelloaddata.formationPrefab, this.spawnObjects[4].transform.position, this.spawnObjects[4].transform.rotation) as GameObject;
			gameObject2.transform.SetParent(gameObject.transform, true);
			gameObject2.transform.position = GameDataManager.enemyvesselsonlevel[i].transform.position;
			this.levelloaddata.formationPositions[i] = gameObject2;
		}
		gameObject.transform.Translate(Vector3.forward * 0.5f);
		this.uifunctions.combatai.formationCruiseSpeeds = new float[3];
		this.uifunctions.combatai.formationCruiseSpeeds[0] = this.levelloaddata.formationCruiseSpeed * 0.75f;
		this.uifunctions.combatai.formationCruiseSpeeds[1] = this.levelloaddata.formationCruiseSpeed;
		this.uifunctions.combatai.formationCruiseSpeeds[2] = this.levelloaddata.formationCruiseSpeed * 1.25f;
		if (this.levelloaddata.usePresetPositions || GameDataManager.trainingMode)
		{
			UIFunctions.globaluifunctions.combatai.usingFormationAI = false;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
		}
		else
		{
			UIFunctions.globaluifunctions.combatai.usingFormationAI = true;
		}
	}

	// Token: 0x0600084C RID: 2124 RVA: 0x00056F10 File Offset: 0x00055110
	public bool GetIsSubmarinesOnly()
	{
		for (int i = 0; i < this.levelloaddata.kmShipClasses.Length; i++)
		{
			if (this.uifunctions.database.databaseshipdata[this.levelloaddata.kmShipClasses[i]].shipType != "SUBMARINE")
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600084D RID: 2125 RVA: 0x00056F70 File Offset: 0x00055170
	private int[] GetShipSlotNumbers()
	{
		this.shipSlots = new int[this.levelloaddata.kmShipClasses.Length];
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < this.shipSlots.Length; i++)
		{
			if (this.uifunctions.database.databaseshipdata[this.levelloaddata.kmShipClasses[i]].shipType == "ESCORT")
			{
				num2++;
			}
			else if (this.uifunctions.database.databaseshipdata[this.levelloaddata.kmShipClasses[i]].shipType == "SUBMARINE")
			{
				if (this.uifunctions.database.databaseshipdata[this.levelloaddata.kmShipClasses[i]].shipDesignation != "SSBN" && this.uifunctions.database.databaseshipdata[this.levelloaddata.kmShipClasses[i]].shipDesignation != "SSGN")
				{
					num3++;
				}
				else
				{
					num4++;
				}
			}
			else
			{
				num++;
			}
		}
		this.levelloaddata.submarinesOnly = this.GetIsSubmarinesOnly();
		int[] escortSlots = this.GetEscortSlots(num2);
		int[] coreSlots = this.GetCoreSlots(num, true);
		int[] escortSlots2 = this.GetEscortSlots(num3);
		int[] coreSlots2 = this.GetCoreSlots(num4, false);
		for (int j = 0; j < coreSlots.Length; j++)
		{
			int num5 = coreSlots[j];
			int num6 = UnityEngine.Random.Range(j, coreSlots.Length);
			coreSlots[j] = coreSlots[num6];
			coreSlots[num6] = num5;
		}
		for (int k = 0; k < escortSlots.Length; k++)
		{
			int num7 = escortSlots[k];
			int num8 = UnityEngine.Random.Range(k, escortSlots.Length);
			escortSlots[k] = escortSlots[num8];
			escortSlots[num8] = num7;
		}
		for (int l = 0; l < escortSlots2.Length; l++)
		{
			int num9 = escortSlots2[l];
			int num10 = UnityEngine.Random.Range(l, escortSlots2.Length);
			escortSlots2[l] = escortSlots2[num10];
			escortSlots2[num10] = num9;
		}
		for (int m = 0; m < coreSlots2.Length; m++)
		{
			int num11 = coreSlots2[m];
			int num12 = UnityEngine.Random.Range(m, coreSlots2.Length);
			coreSlots2[m] = coreSlots2[num12];
			coreSlots2[num12] = num11;
		}
		if (this.numberCoreColumns < 3)
		{
			for (int n = 0; n < escortSlots.Length; n++)
			{
				if (escortSlots[n] == 11)
				{
					escortSlots[n] = 7;
				}
				else if (escortSlots[n] == 13)
				{
					escortSlots[n] = 8;
				}
				if (this.numberCoreColumns == 1)
				{
					if (escortSlots[n] == 12)
					{
						escortSlots[n] = 5;
					}
					else if (escortSlots[n] == 14)
					{
						escortSlots[n] = 4;
					}
				}
			}
		}
		int num13 = this.numberSubCoreColumns;
		if (this.numberSubCoreColumns < this.numberCoreColumns)
		{
			num13 = this.numberCoreColumns;
		}
		if (num13 < 3)
		{
			for (int num14 = 0; num14 < escortSlots2.Length; num14++)
			{
				if (escortSlots2[num14] == 11)
				{
					escortSlots2[num14] = 7;
				}
				else if (escortSlots2[num14] == 13)
				{
					escortSlots2[num14] = 8;
				}
				if (num13 == 1)
				{
					if (escortSlots2[num14] == 12)
					{
						escortSlots2[num14] = 5;
					}
					else if (escortSlots2[num14] == 14)
					{
						escortSlots2[num14] = 4;
					}
				}
			}
		}
		int num15 = 0;
		int num16 = 0;
		int num17 = 0;
		int num18 = 0;
		for (int num19 = 0; num19 < this.shipSlots.Length; num19++)
		{
			if (this.uifunctions.database.databaseshipdata[this.levelloaddata.kmShipClasses[num19]].shipType == "ESCORT")
			{
				this.shipSlots[num19] = escortSlots[num15];
				num15++;
			}
			else if (this.uifunctions.database.databaseshipdata[this.levelloaddata.kmShipClasses[num19]].shipType == "SUBMARINE")
			{
				if (this.uifunctions.database.databaseshipdata[this.levelloaddata.kmShipClasses[num19]].shipDesignation != "SSBN" && this.uifunctions.database.databaseshipdata[this.levelloaddata.kmShipClasses[num19]].shipDesignation != "SSGN")
				{
					this.shipSlots[num19] = escortSlots2[num17];
					num17++;
				}
				else
				{
					this.shipSlots[num19] = coreSlots2[num18];
					num18++;
				}
			}
			else
			{
				this.shipSlots[num19] = coreSlots[num16];
				num16++;
			}
		}
		return this.shipSlots;
	}

	// Token: 0x0600084E RID: 2126 RVA: 0x00057450 File Offset: 0x00055650
	private int[] GetEscortSlots(int escorts)
	{
		int[] array = new int[0];
		switch (escorts)
		{
		case 1:
			array = new int[]
			{
				15
			};
			break;
		case 2:
			array = new int[]
			{
				13,
				14
			};
			break;
		case 3:
			array = new int[]
			{
				11,
				12,
				15
			};
			break;
		case 4:
			array = new int[]
			{
				11,
				12,
				13,
				14
			};
			break;
		case 5:
			array = new int[]
			{
				11,
				12,
				13,
				14,
				15
			};
			break;
		case 6:
			array = new int[]
			{
				10,
				11,
				12,
				13,
				14,
				15
			};
			break;
		default:
		{
			array = new int[escorts];
			int num = 15;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = num;
				num--;
			}
			break;
		}
		}
		return array;
	}

	// Token: 0x0600084F RID: 2127 RVA: 0x00057540 File Offset: 0x00055740
	private int[] GetCoreSlots(int coreShips, bool ships)
	{
		int[] array = new int[0];
		int num;
		switch (coreShips)
		{
		case 1:
			array = new int[]
			{
				1
			};
			if (this.levelloaddata.submarinesOnly)
			{
				array[0] = 0;
			}
			num = 1;
			break;
		case 2:
			array = new int[]
			{
				0,
				1
			};
			if (UnityEngine.Random.value < 0.5f)
			{
				array = new int[]
				{
					0,
					2
				};
			}
			num = 1;
			break;
		case 3:
			array = new int[]
			{
				0,
				1,
				2
			};
			num = 1;
			if (UnityEngine.Random.value < 0.33f)
			{
				array = new int[]
				{
					6,
					0,
					3
				};
				num = 3;
			}
			else if (UnityEngine.Random.value < 0.33f)
			{
				array = new int[]
				{
					8,
					1,
					4
				};
				num = 3;
			}
			break;
		case 4:
			array = new int[]
			{
				1,
				4,
				0,
				3
			};
			num = 2;
			break;
		case 5:
			array = new int[]
			{
				6,
				0,
				3,
				2,
				1
			};
			if (UnityEngine.Random.value < 0.33f)
			{
				array = new int[]
				{
					8,
					1,
					4,
					2,
					0
				};
			}
			else if (UnityEngine.Random.value < 0.33f)
			{
				array = new int[]
				{
					7,
					2,
					5,
					0,
					1
				};
			}
			num = 3;
			break;
		case 6:
			array = new int[]
			{
				8,
				1,
				4,
				6,
				0,
				3
			};
			num = 3;
			if (UnityEngine.Random.value < 0.33f)
			{
				array = new int[]
				{
					6,
					0,
					3,
					7,
					2,
					5
				};
				num = 3;
			}
			else if (UnityEngine.Random.value < 0.33f)
			{
				array = new int[]
				{
					1,
					4,
					0,
					3,
					2,
					5
				};
				num = 2;
			}
			break;
		case 7:
			array = new int[]
			{
				6,
				0,
				3,
				7,
				5,
				8,
				4
			};
			if (UnityEngine.Random.value < 0.33f)
			{
				array = new int[]
				{
					8,
					1,
					4,
					7,
					6,
					5,
					3
				};
			}
			else if (UnityEngine.Random.value < 0.33f)
			{
				array = new int[]
				{
					7,
					2,
					5,
					6,
					8,
					3,
					4
				};
			}
			num = 3;
			break;
		case 8:
			array = new int[]
			{
				1,
				4,
				0,
				3,
				2,
				5,
				9,
				10
			};
			num = 2;
			break;
		case 9:
			array = new int[]
			{
				8,
				1,
				4,
				6,
				0,
				3,
				7,
				2,
				5
			};
			num = 3;
			break;
		case 10:
			array = new int[]
			{
				8,
				1,
				4,
				6,
				0,
				3,
				7,
				2,
				5,
				10
			};
			num = 3;
			break;
		default:
			array = new int[coreShips];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = i;
			}
			num = 3;
			break;
		}
		if (ships)
		{
			this.numberCoreColumns = num;
		}
		else
		{
			this.numberSubCoreColumns = num;
		}
		return array;
	}

	// Token: 0x06000850 RID: 2128 RVA: 0x0005780C File Offset: 0x00055A0C
	private void SpawnShip(int shipNumber)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (this.uifunctions.database.databaseshipdata[this.levelloaddata.kmShipClasses[shipNumber]].shipType == "ESCORT")
		{
			flag = true;
		}
		else if (this.uifunctions.database.databaseshipdata[this.levelloaddata.kmShipClasses[shipNumber]].shipType == "SUBMARINE")
		{
			if (this.uifunctions.database.databaseshipdata[this.levelloaddata.kmShipClasses[shipNumber]].shipDesignation != "SSBN" && this.uifunctions.database.databaseshipdata[this.levelloaddata.kmShipClasses[shipNumber]].shipDesignation != "SSGN")
			{
				flag2 = true;
			}
			else
			{
				flag3 = true;
			}
		}
		float num = 0f;
		float num2 = 0f;
		if (shipNumber < this.shipSlots.Length)
		{
			switch (this.shipSlots[shipNumber])
			{
			case 1:
				num2 = this.spacing * 1f;
				break;
			case 2:
				num2 = this.spacing * -1f;
				break;
			case 3:
				num = this.spacing * 1f;
				break;
			case 4:
				num2 = this.spacing * 1f;
				num = this.spacing * 1f;
				break;
			case 5:
				num2 = this.spacing * -1f;
				num = this.spacing * 1f;
				break;
			case 6:
				num = this.spacing * -1f;
				break;
			case 7:
				num2 = this.spacing * -1f;
				num = this.spacing * -1f;
				break;
			case 8:
				num2 = this.spacing * 1f;
				num = this.spacing * -1f;
				break;
			case 9:
				num2 = this.spacing * -2f;
				num = this.spacing * 1f;
				break;
			case 10:
				num2 = this.spacing * -2f;
				break;
			case 11:
				num2 = this.spacing * -1f;
				num = this.spacing * -2f;
				break;
			case 12:
				num2 = this.spacing * -1f;
				num = this.spacing * 2f;
				break;
			case 13:
				num2 = this.spacing * 1f;
				num = this.spacing * -2f;
				break;
			case 14:
				num2 = this.spacing * 1f;
				num = this.spacing * 2f;
				break;
			case 15:
				num2 = this.spacing * 2f;
				break;
			}
			if (flag || flag2)
			{
				float num3 = 0.5f;
				float num4 = 1f;
				if (flag2)
				{
					if (!this.levelloaddata.submarinesOnly)
					{
						num3 = 2f;
						num4 = 3f;
					}
					else
					{
						num3 = 0.05f;
						num4 = 0.05f;
					}
				}
				if (num < 0f)
				{
					num -= this.spacing * num3;
				}
				else if (num > 0f)
				{
					num += this.spacing * num3;
				}
				if (num2 < 0f)
				{
					num2 -= this.spacing * num4;
				}
				else if (num2 > 0f)
				{
					num2 += this.spacing * num4;
				}
				if ((this.shipSlots[shipNumber] == 15 || this.shipSlots[shipNumber] == 10) && this.numberCoreColumns == 2)
				{
					num = this.spacing * 0.5f;
				}
			}
			if (flag3)
			{
				if (this.numberSubCoreColumns == 1 && this.numberCoreColumns == 2)
				{
					num = this.spacing * 0.5f;
				}
				if (this.numberSubCoreColumns == 2 && this.numberCoreColumns == 1)
				{
					num = this.spacing * -0.5f;
				}
			}
		}
		Database database = this.uifunctions.database;
		Transform transform = this.spawnObjects[4].transform;
		int num5 = this.levelloaddata.kmShipClasses[shipNumber];
		transform.localPosition = new Vector3(num, 0f, num2);
		GameObject gameObject = this.uifunctions.vesselbuilder.CreateVessel(database.databaseshipdata[num5].shipID, false, transform.position, transform.rotation);
		if (this.uifunctions.database.databaseshipdata[this.levelloaddata.kmShipClasses[shipNumber]].shipType == "SUBMARINE")
		{
			gameObject.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
			gameObject.transform.position = new Vector3(gameObject.transform.position.x, 1000f - UnityEngine.Random.Range(70f, 450f) * GameDataManager.feetToUnits, gameObject.transform.position.z);
		}
		Vessel component = gameObject.GetComponent<Vessel>();
		component.whichNavy = 1;
		if (Database.GetIsCivilian(component.databaseshipdata.shipPrefabName))
		{
			component.whichNavy = 4;
			if (shipNumber > GameDataManager.enemyNumberofShips - this.levelloaddata.neutralIDsToSpawn.Count - this.worldVessels.Count - 1)
			{
				component.vesselai.isNeutral = true;
				if (component.databaseshipdata.shipType != "OILRIG")
				{
					gameObject.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
					gameObject.transform.position = new Vector3(GameDataManager.enemyvesselsonlevel[0].transform.position.x, 1000f, GameDataManager.enemyvesselsonlevel[0].transform.position.z);
					gameObject.transform.Translate(Vector3.forward * (float)UnityEngine.Random.Range(40, 200));
					gameObject.transform.Translate(Vector3.right * (float)UnityEngine.Random.Range(-200, 200));
					for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
					{
						if (GameDataManager.enemyvesselsonlevel[i] != null && Vector3.Distance(gameObject.transform.position, GameDataManager.enemyvesselsonlevel[i].transform.position) < 5f)
						{
							gameObject.transform.Translate(Vector3.forward * 10f);
						}
					}
					gameObject.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
					component.vesselai.GetNextWaypoint();
				}
			}
		}
		if (component.databaseshipdata.shipType == "BIOLOGIC")
		{
			component.whichNavy = 3;
		}
		component.vesselmovement.InitialiseVesselMovement();
		component.databaseshipdata = this.uifunctions.database.databaseshipdata[num5];
		component.vesselai.formationWaypointModifier.x = num;
		component.vesselai.formationWaypointModifier.z = num2;
		component.vesselListIndex = shipNumber;
		GameObject gameObject2 = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.noiseRadius, component.transform.position, component.transform.rotation) as GameObject;
		gameObject2.transform.SetParent(component.transform, false);
		gameObject2.transform.localPosition = Vector3.zero;
		gameObject2.transform.localRotation = Quaternion.identity;
		component.acoustics.noiseRadius = gameObject2.GetComponent<SphereCollider>();
		component.acoustics.noiseRadius.name = shipNumber.ToString();
		component.playercontrolled = false;
		GameDataManager.enemyvesselsonlevel[shipNumber] = component;
		gameObject.tag = base.tag;
		this.SetupVesselPositionHistory(component);
		component.damagesystem.DamageInit();
	}

	// Token: 0x06000851 RID: 2129 RVA: 0x0005808C File Offset: 0x0005628C
	private void SetupVesselPositionHistory(Vessel spawnedvessel)
	{
		spawnedvessel.vesselPositionHistory = new Vector2[20];
		spawnedvessel.vesselPositionHistory[0].x = spawnedvessel.transform.position.x;
		spawnedvessel.vesselPositionHistory[0].y = spawnedvessel.transform.position.z;
	}

	// Token: 0x06000852 RID: 2130 RVA: 0x000580F0 File Offset: 0x000562F0
	public void EnterUnitReference()
	{
		UIFunctions.globaluifunctions.helpmanager.inUnitReference = true;
		this.museumFilters.SetActive(true);
		this.currentNationFilter = 0;
		this.currentUnitFilter = "all";
		this.SetFilteredLists();
		this.BackgroundMuseumRender(true);
	}

	// Token: 0x06000853 RID: 2131 RVA: 0x00058130 File Offset: 0x00056330
	public void SetFilteredLists()
	{
		this.currentFilteredVessels = new List<int>();
		this.currentFilteredAircraft = new List<int>();
		this.currentFilteredWeapons = new List<int>();
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool active = false;
		bool active2 = false;
		bool flag4 = false;
		for (int i = 0; i < UIFunctions.globaluifunctions.database.databaseshipdata.Length; i++)
		{
			string[] array = UIFunctions.globaluifunctions.database.databaseshipdata[i].shipPrefabName.Split(new char[]
			{
				'_'
			});
			if (array[0] == this.nations[this.currentNationFilter] || this.currentNationFilter == 0)
			{
				this.currentFilteredVessels.Add(i);
				if (!flag && UIFunctions.globaluifunctions.database.databaseshipdata[i].shipType == "SUBMARINE")
				{
					flag = true;
				}
				if (!flag2 && (UIFunctions.globaluifunctions.database.databaseshipdata[i].shipType == "ESCORT" || UIFunctions.globaluifunctions.database.databaseshipdata[i].shipType == "CAPITAL"))
				{
					flag2 = true;
				}
				if (!flag3 && UIFunctions.globaluifunctions.database.databaseshipdata[i].shipType == "MERCHANT")
				{
					flag3 = true;
				}
				if (!flag4 && UIFunctions.globaluifunctions.database.databaseshipdata[i].shipType == "BIOLOGIC")
				{
					flag4 = true;
				}
			}
		}
		for (int j = 0; j < UIFunctions.globaluifunctions.database.databaseaircraftdata.Length; j++)
		{
			string[] array2 = UIFunctions.globaluifunctions.database.databaseaircraftdata[j].aircraftPrefabName.Split(new char[]
			{
				'_'
			});
			if (array2[0] == this.nations[this.currentNationFilter] || this.currentNationFilter == 0)
			{
				this.currentFilteredAircraft.Add(j);
				active = true;
			}
		}
		for (int k = 0; k < UIFunctions.globaluifunctions.database.databaseweapondata.Length; k++)
		{
			string[] array3 = UIFunctions.globaluifunctions.database.databaseweapondata[k].weaponPrefabName.Split(new char[]
			{
				'_'
			});
			if (array3[0] == this.nations[this.currentNationFilter] || this.currentNationFilter == 0)
			{
				this.currentFilteredWeapons.Add(k);
				active2 = true;
			}
		}
		this.filterButtons[0].gameObject.SetActive(flag);
		this.filterButtons[1].gameObject.SetActive(flag2);
		this.filterButtons[2].gameObject.SetActive(flag3);
		this.filterButtons[3].gameObject.SetActive(active);
		this.filterButtons[4].gameObject.SetActive(active2);
		this.filterButtons[5].gameObject.SetActive(flag4);
	}

	// Token: 0x06000854 RID: 2132 RVA: 0x0005844C File Offset: 0x0005664C
	public void SetNation()
	{
		this.currentNationFilter++;
		if (this.currentNationFilter == this.nations.Length)
		{
			this.currentNationFilter = 0;
		}
		this.nationImage.sprite = this.nationSprites[this.currentNationFilter];
		this.SetFilteredLists();
		this.currentMuseumItem = 0;
		this.BackgroundMuseumRender(true);
	}

	// Token: 0x06000855 RID: 2133 RVA: 0x000584B0 File Offset: 0x000566B0
	public void GetFilteredUnit(string type)
	{
		if (type == "AIRCRAFT")
		{
			this.currentMuseumItem = this.currentFilteredVessels.Count;
		}
		else if (type == "WEAPON")
		{
			this.currentMuseumItem = this.currentFilteredVessels.Count + this.currentFilteredAircraft.Count;
		}
		else if (type != "WARSHIP")
		{
			for (int i = 0; i < this.currentFilteredVessels.Count; i++)
			{
				if (UIFunctions.globaluifunctions.database.databaseshipdata[this.currentFilteredVessels[i]].shipType == type)
				{
					this.currentMuseumItem = i;
					break;
				}
			}
		}
		else
		{
			for (int j = 0; j < UIFunctions.globaluifunctions.database.databaseshipdata.Length; j++)
			{
				if (UIFunctions.globaluifunctions.database.databaseshipdata[this.currentFilteredVessels[j]].shipType == "ESCORT" || UIFunctions.globaluifunctions.database.databaseshipdata[this.currentFilteredVessels[j]].shipType == "CAPITAL")
				{
					this.currentMuseumItem = j;
					break;
				}
			}
		}
		UIFunctions.globaluifunctions.levelloadmanager.BackgroundMuseumRender(false);
	}

	// Token: 0x06000856 RID: 2134 RVA: 0x0005861C File Offset: 0x0005681C
	public void BackgroundMuseumRender(bool resetCamera)
	{
		UIFunctions.globaluifunctions.cameraMount.gameObject.SetActive(true);
		UIFunctions.globaluifunctions.MainCamera.GetComponent<ManualCameraZoom>().enabled = true;
		UIFunctions.globaluifunctions.missionmanager.assignedShip = false;
		Rect rect = new Rect(0f, 0f, 1f, 1f);
		UIFunctions.globaluifunctions.GUICameraObject.rect = rect;
		if (!KeybindManagerMuseum.selectionScreen)
		{
			UIFunctions.globaluifunctions.keybindManagerMuseum.selectShipButton.gameObject.SetActive(false);
		}
		LevelLoadManager.inMuseum = true;
		ManualCameraZoom.museumThreshold = UIFunctions.globaluifunctions.GUICameraObject.WorldToScreenPoint(this.museumThreshold.position);
		UIFunctions.globaluifunctions.keybindManagerMenu.enabled = false;
		this.amplifycoloreffect.enabled = false;
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			UnityEngine.Object.Destroy(GameDataManager.enemyvesselsonlevel[i].gameObject);
		}
		if (this.currentMuseumInstantiatedObject != null)
		{
			UnityEngine.Object.Destroy(this.currentMuseumInstantiatedObject);
		}
		Resources.UnloadUnusedAssets();
		Time.timeScale = 1f;
		this.CreateEnvironment(0);
		this.skyboxobject.SetActive(false);
		this.environment.directionalLights[0].transform.localRotation = Quaternion.Euler(UIFunctions.globaluifunctions.levelloadmanager.museumLightingAngle);
		float y = 0f;
		ManualCameraZoom.sensitivity = GameDataManager.camerasensitivity;
		if (ManualCameraZoom.sensitivity <= 0f)
		{
			ManualCameraZoom.sensitivity = 0.05f;
		}
		GameDataManager.cameraTimeScale = 1f;
		int num = this.currentMuseumItem;
		string a = "vessel";
		if (!KeybindManagerMuseum.selectionScreen)
		{
			if (num < this.currentFilteredVessels.Count)
			{
				this.museumObjectNumber = this.currentFilteredVessels[num];
			}
			else
			{
				this.museumObjectNumber = 0;
				num -= this.currentFilteredVessels.Count;
				if (num < this.currentFilteredAircraft.Count)
				{
					a = "aircraft";
					num = this.currentFilteredAircraft[num];
				}
				else
				{
					num -= this.currentFilteredAircraft.Count;
					a = "torpedo";
					num = this.currentFilteredWeapons[num];
				}
			}
		}
		else
		{
			this.museumObjectNumber = this.currentMuseumItem;
		}
		GameObject gameObject = this.uifunctions.vesselbuilder.CreateVessel(this.uifunctions.database.databaseshipdata[this.museumObjectNumber].shipID, false, new Vector3(1f, 1100f, 2f), Quaternion.identity);
		Vessel component = gameObject.GetComponent<Vessel>();
		component.databaseshipdata = this.uifunctions.database.databaseshipdata[this.museumObjectNumber];
		gameObject.transform.localRotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(0f, y, 0f), 1f);
		GameDataManager.enemyvesselsonlevel = new Vessel[1];
		GameDataManager.enemyvesselsonlevel[0] = gameObject.GetComponent<Vessel>();
		if (GameDataManager.enemyvesselsonlevel[0].vesselai != null)
		{
			GameDataManager.enemyvesselsonlevel[0].vesselai.enabled = false;
		}
		if (GameDataManager.enemyvesselsonlevel[0].vesselmovement.weaponSource != null)
		{
			GameDataManager.enemyvesselsonlevel[0].vesselmovement.weaponSource.enabled = false;
		}
		if (a == "vessel")
		{
			this.PopulateMuseumData("vessel", this.museumObjectNumber);
			ManualCameraZoom.minDistance = component.databaseshipdata.minCameraDistance;
			ManualCameraZoom.distance = ManualCameraZoom.minDistance * 2f;
			if (component.databaseshipdata.shipType == "OILRIG")
			{
				ManualCameraZoom.distance *= 1.2f;
			}
			if (component.databaseshipdata.shipType == "BIOLOGIC")
			{
				ManualCameraZoom.distance = component.databaseshipdata.minCameraDistance;
			}
			this.currentMuseumInstantiatedObject = gameObject;
		}
		else if (a == "aircraft")
		{
			this.PopulateMuseumData("aircraft", num);
			GameObject gameObject2 = this.uifunctions.vesselbuilder.CreateAircraft(num, new Vector3(1f, 1100f, 2f), Quaternion.identity, true);
			Helicopter component2 = gameObject2.GetComponent<Helicopter>();
			if (component2 != null)
			{
				component2.enabled = false;
				component2.helicopterrigidbody.useGravity = false;
				gameObject2.transform.Translate(Vector3.up * 0.025f);
			}
			else
			{
				Aircraft component3 = gameObject2.GetComponent<Aircraft>();
				component3.enabled = false;
			}
			this.currentMuseumInstantiatedObject = gameObject2;
			GameDataManager.enemyvesselsonlevel[0].gameObject.SetActive(false);
			ManualCameraZoom.minDistance = this.uifunctions.database.databaseaircraftdata[num].minCameraDistance;
			ManualCameraZoom.distance = ManualCameraZoom.minDistance;
		}
		else if (a == "torpedo")
		{
			this.PopulateMuseumData("torpedo", num);
			GameObject gameObject3 = UnityEngine.Object.Instantiate(this.uifunctions.database.databaseweapondata[num].weaponObject, new Vector3(1f, 1100f, 2f), Quaternion.identity) as GameObject;
			gameObject3.SetActive(true);
			Torpedo component4 = gameObject3.GetComponent<Torpedo>();
			component4.enabled = false;
			this.currentMuseumInstantiatedObject = gameObject3;
			GameDataManager.enemyvesselsonlevel[0].gameObject.SetActive(false);
			ManualCameraZoom.minDistance = this.uifunctions.database.databaseweapondata[num].minCameraDistance;
			ManualCameraZoom.distance = ManualCameraZoom.minDistance;
		}
		this.cetoOcean.gameObject.SetActive(false);
		Camera component5 = this.MainCamera.GetComponent<Camera>();
		component5.depth = 2f;
		component5.clearFlags = CameraClearFlags.Nothing;
		component5.rect = new Rect(0.3865f, 0.157f, 1f, 0.701f);
		this.MainCamera.SetActive(true);
		ManualCameraZoom.oceanShadowPlane.SetActive(false);
		this.uifunctions.SetMenuSystem("MUSEUM");
		GameDataManager.enemyNumberofShips = 1;
		GameDataManager.playerNumberofShips = 0;
		this.uifunctions.skyobjectcenterer.ForceSkyboxPosition(GameDataManager.enemyvesselsonlevel[0].transform);
		ManualCameraZoom.yMinLimit = -89;
		this.uifunctions.backgroundImage.gameObject.SetActive(false);
		GC.Collect();
		this.skyboxobject.SetActive(false);
		if (resetCamera)
		{
			ManualCameraZoom.x = 135f;
			ManualCameraZoom.y = 22.5f;
		}
	}

	// Token: 0x06000857 RID: 2135 RVA: 0x00058CC8 File Offset: 0x00056EC8
	public void PopulateMuseumData(string museumObjectType, int index)
	{
		this.uifunctions.scrollbarDefault.value = 1f;
		this.uifunctions.mainTitle.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "UnitReferenceHeader");
		if (KeybindManagerMuseum.selectionScreen)
		{
			this.uifunctions.mainTitle.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "SelectVesselHeader");
		}
		this.uifunctions.mainColumn.text = string.Empty;
		this.uifunctions.secondColumm.text = string.Empty;
		if (museumObjectType == "vessel")
		{
			this.currentShipDebugIndex = 0;
			DatabaseShipData databaseShipData = UIFunctions.globaluifunctions.database.databaseshipdata[index];
			this.uifunctions.mainColumn.text = "\n<size=22><b>" + databaseShipData.description + "</b></size>\n\n";
			Text secondColumm = this.uifunctions.secondColumm;
			secondColumm.text = secondColumm.text + "\n<size=22> </size>\n\n<b>" + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceDimensions") + ":</b>\n";
			Text secondColumm2 = this.uifunctions.secondColumm;
			string text = secondColumm2.text;
			secondColumm2.text = string.Concat(new string[]
			{
				text,
				string.Format("{0:#,0}", databaseShipData.displacement),
				" ",
				LanguageManager.interfaceDictionary["ReferenceTons"],
				"\n"
			});
			if (databaseShipData.displayLength == 0f)
			{
				Text secondColumm3 = this.uifunctions.secondColumm;
				secondColumm3.text = secondColumm3.text + databaseShipData.length.ToString() + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceMetre") + " x ";
			}
			else
			{
				Text secondColumm4 = this.uifunctions.secondColumm;
				secondColumm4.text = secondColumm4.text + databaseShipData.displayLength.ToString() + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceMetre") + " x ";
			}
			if (databaseShipData.displayBeam == 0f)
			{
				Text secondColumm5 = this.uifunctions.secondColumm;
				secondColumm5.text = secondColumm5.text + databaseShipData.beam.ToString() + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceMetre") + "\n";
			}
			else
			{
				Text secondColumm6 = this.uifunctions.secondColumm;
				secondColumm6.text = secondColumm6.text + databaseShipData.displayBeam.ToString() + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceMetre") + "\n";
			}
			float num = databaseShipData.surfacespeed;
			if (databaseShipData.shipType == "SUBMARINE")
			{
				num = databaseShipData.submergedspeed;
			}
			if (!(databaseShipData.shipType != "BIOLOGIC"))
			{
				Text secondColumm7 = this.uifunctions.secondColumm;
				secondColumm7.text = secondColumm7.text + num.ToString() + " " + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceKnot");
				Text mainColumn = this.uifunctions.mainColumn;
				mainColumn.text = mainColumn.text + "\n\n\n\n<b>" + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceNotes") + ":</b>\n";
				for (int i = 0; i < databaseShipData.history.Length; i++)
				{
					Text mainColumn2 = this.uifunctions.mainColumn;
					mainColumn2.text = mainColumn2.text + databaseShipData.history[i] + "\n";
				}
				return;
			}
			Text secondColumm8 = this.uifunctions.secondColumm;
			text = secondColumm8.text;
			secondColumm8.text = string.Concat(new string[]
			{
				text,
				num.ToString(),
				" ",
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceKnot"),
				", ",
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceCrew"),
				" ",
				databaseShipData.crew.ToString()
			});
			if (databaseShipData.shipType == "SUBMARINE")
			{
				Text secondColumm9 = this.uifunctions.secondColumm;
				text = secondColumm9.text;
				secondColumm9.text = string.Concat(new string[]
				{
					text,
					"\n",
					LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceTestDepth"),
					" ",
					databaseShipData.testDepth.ToString(),
					" ",
					LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceFeet")
				});
				if (databaseShipData.numberOfWires > 0)
				{
					Text secondColumm10 = this.uifunctions.secondColumm;
					text = secondColumm10.text;
					secondColumm10.text = string.Concat(new string[]
					{
						text,
						"\n",
						LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceWires"),
						" ",
						databaseShipData.numberOfWires.ToString()
					});
				}
			}
			int num2 = 0;
			Text mainColumn3 = this.uifunctions.mainColumn;
			mainColumn3.text = mainColumn3.text + "<b>" + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceDefensive") + ":</b>\n";
			if (databaseShipData.defensiveWeapons != null)
			{
				for (int j = 0; j < databaseShipData.defensiveWeapons.Length; j++)
				{
					Text mainColumn4 = this.uifunctions.mainColumn;
					mainColumn4.text = mainColumn4.text + databaseShipData.defensiveWeapons[j] + "\n";
					num2++;
				}
			}
			if (databaseShipData.gunProbability > 0f)
			{
				Text mainColumn5 = this.uifunctions.mainColumn;
				mainColumn5.text = mainColumn5.text + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Reference30mmCIWS") + "\n";
			}
			if (num2 == 0)
			{
				Text mainColumn6 = this.uifunctions.mainColumn;
				mainColumn6.text = mainColumn6.text + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceNone") + "\n";
			}
			Text mainColumn7 = this.uifunctions.mainColumn;
			mainColumn7.text = mainColumn7.text + "\n<b>" + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceOffensive") + ":</b>\n";
			num2 = 0;
			if (databaseShipData.missileGameObject != null && databaseShipData.missilesPerLauncher.Length > 0)
			{
				Text mainColumn8 = this.uifunctions.mainColumn;
				text = mainColumn8.text;
				mainColumn8.text = string.Concat(new string[]
				{
					text,
					this.uifunctions.database.databaseweapondata[databaseShipData.missileType].weaponName,
					" ",
					LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceMissiles"),
					"\n"
				});
				num2++;
			}
			if (databaseShipData.torpedotubes > 0)
			{
				List<int> list = new List<int>();
				for (int k = 0; k < databaseShipData.torpedotypes.Length; k++)
				{
					list.Add(databaseShipData.torpedoIDs[k]);
				}
				if (databaseShipData.vlsTorpedoIDs != null && databaseShipData.vlsTorpedoIDs.Length > 0)
				{
					for (int l = 0; l < databaseShipData.vlsTorpedotypes.Length; l++)
					{
						if (!list.Contains(databaseShipData.vlsTorpedoIDs[l]))
						{
							list.Add(databaseShipData.vlsTorpedoIDs[l]);
						}
					}
				}
				for (int m = 0; m < list.Count; m++)
				{
					Text mainColumn9 = this.uifunctions.mainColumn;
					mainColumn9.text += this.uifunctions.database.databaseweapondata[list[m]].weaponName;
					if (this.uifunctions.database.databaseweapondata[list[m]].isMissile)
					{
						Text mainColumn10 = this.uifunctions.mainColumn;
						mainColumn10.text = mainColumn10.text + " " + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceMissiles") + "\n";
						num2++;
					}
					else if (this.uifunctions.database.databaseweapondata[list[m]].isDecoy)
					{
						Text mainColumn11 = this.uifunctions.mainColumn;
						mainColumn11.text = mainColumn11.text + " " + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceDecoys") + "\n";
						num2++;
					}
					else
					{
						Text mainColumn12 = this.uifunctions.mainColumn;
						mainColumn12.text = mainColumn12.text + " " + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceTorpedoes") + "\n";
						num2++;
					}
				}
			}
			if (databaseShipData.rbuLauncherTypes != null)
			{
				string text2 = string.Empty;
				for (int n = 0; n < databaseShipData.rbuLauncherTypes.Length; n++)
				{
					string depthchargeName = this.uifunctions.database.databasedepthchargedata[databaseShipData.rbuLauncherTypes[n]].depthchargeName;
					if (!text2.Contains(depthchargeName))
					{
						text2 = text2 + depthchargeName + "\n";
						num2++;
					}
				}
				Text mainColumn13 = this.uifunctions.mainColumn;
				mainColumn13.text += text2;
			}
			if (databaseShipData.navalGunTypes != null)
			{
				string text3 = string.Empty;
				for (int num3 = 0; num3 < databaseShipData.navalGunTypes.Length; num3++)
				{
					string depthchargeName2 = this.uifunctions.database.databasedepthchargedata[databaseShipData.navalGunTypes[num3]].depthchargeName;
					if (!text3.Contains(depthchargeName2))
					{
						text3 = text3 + depthchargeName2 + "\n";
						num2++;
					}
				}
				Text mainColumn14 = this.uifunctions.mainColumn;
				mainColumn14.text += text3;
			}
			if (num2 == 0)
			{
				Text mainColumn15 = this.uifunctions.mainColumn;
				mainColumn15.text = mainColumn15.text + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceNone") + "\n";
			}
			Text mainColumn16 = this.uifunctions.mainColumn;
			mainColumn16.text = mainColumn16.text + "\n<b>" + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceSensors") + ":</b>\n";
			if (databaseShipData.activeSonarID == -1 && databaseShipData.passiveSonarID == -1 && databaseShipData.towedSonarID == -1 && databaseShipData.radarID == -1)
			{
				Text mainColumn17 = this.uifunctions.mainColumn;
				mainColumn17.text = mainColumn17.text + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceNavRADAR") + "\n";
			}
			else
			{
				if (databaseShipData.radarID > -1)
				{
					Text mainColumn18 = this.uifunctions.mainColumn;
					mainColumn18.text = mainColumn18.text + this.uifunctions.database.databaseradardata[databaseShipData.radarID].radarDisplayName + " " + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceSearchRADAR");
					if (databaseShipData.shipType == "SUBMARINE")
					{
						Text mainColumn19 = this.uifunctions.mainColumn;
						mainColumn19.text = mainColumn19.text + ", " + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceMastMounted") + "\n";
					}
					else
					{
						Text mainColumn20 = this.uifunctions.mainColumn;
						mainColumn20.text += "\n";
					}
				}
				bool flag = false;
				if (databaseShipData.activeSonarID > -1 && databaseShipData.passiveSonarID > -1)
				{
					if (this.uifunctions.database.databasesonardata[databaseShipData.activeSonarID].sonarActiveSensitivity > 0f && this.uifunctions.database.databasesonardata[databaseShipData.activeSonarID].sonarPassiveSensitivity > 0f)
					{
						Text mainColumn21 = this.uifunctions.mainColumn;
						text = mainColumn21.text;
						mainColumn21.text = string.Concat(new string[]
						{
							text,
							this.uifunctions.database.databasesonardata[databaseShipData.activeSonarID].sonarDisplayName,
							" ",
							LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceSonarBoth"),
							", "
						});
						Text mainColumn22 = this.uifunctions.mainColumn;
						text = mainColumn22.text;
						mainColumn22.text = string.Concat(new string[]
						{
							text,
							this.GetFrequencies(this.uifunctions.database.databasesonardata[databaseShipData.activeSonarID].sonarFrequencies),
							", ",
							LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceHullMounted"),
							"\n"
						});
					}
					else
					{
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					if (databaseShipData.activeSonarID > -1)
					{
						Text mainColumn23 = this.uifunctions.mainColumn;
						text = mainColumn23.text;
						mainColumn23.text = string.Concat(new string[]
						{
							text,
							this.uifunctions.database.databasesonardata[databaseShipData.activeSonarID].sonarDisplayName,
							" ",
							LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceSonarActive"),
							", "
						});
						Text mainColumn24 = this.uifunctions.mainColumn;
						text = mainColumn24.text;
						mainColumn24.text = string.Concat(new string[]
						{
							text,
							this.GetFrequencies(this.uifunctions.database.databasesonardata[databaseShipData.activeSonarID].sonarFrequencies),
							", ",
							LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceHullMounted"),
							"\n"
						});
					}
					if (databaseShipData.passiveSonarID > -1)
					{
						Text mainColumn25 = this.uifunctions.mainColumn;
						text = mainColumn25.text;
						mainColumn25.text = string.Concat(new string[]
						{
							text,
							this.uifunctions.database.databasesonardata[databaseShipData.passiveSonarID].sonarDisplayName,
							" ",
							LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceSonarPassive"),
							", "
						});
						Text mainColumn26 = this.uifunctions.mainColumn;
						text = mainColumn26.text;
						mainColumn26.text = string.Concat(new string[]
						{
							text,
							this.GetFrequencies(this.uifunctions.database.databasesonardata[databaseShipData.passiveSonarID].sonarFrequencies),
							", ",
							LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceHullMounted"),
							"\n"
						});
					}
				}
				if (databaseShipData.towedSonarID > -1)
				{
					Text mainColumn27 = this.uifunctions.mainColumn;
					mainColumn27.text = mainColumn27.text + this.uifunctions.database.databasesonardata[databaseShipData.towedSonarID].sonarDisplayName + " ";
					if (this.uifunctions.database.databasesonardata[databaseShipData.towedSonarID].sonarActiveSensitivity > 0f && this.uifunctions.database.databasesonardata[databaseShipData.towedSonarID].sonarPassiveSensitivity > 0f)
					{
						Text mainColumn28 = this.uifunctions.mainColumn;
						mainColumn28.text = mainColumn28.text + " " + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceSonarBoth") + ", ";
					}
					else if (this.uifunctions.database.databasesonardata[databaseShipData.towedSonarID].sonarActiveSensitivity > 0f)
					{
						Text mainColumn29 = this.uifunctions.mainColumn;
						mainColumn29.text = mainColumn29.text + " " + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceSoanrActive") + ", ";
					}
					else
					{
						Text mainColumn30 = this.uifunctions.mainColumn;
						mainColumn30.text = mainColumn30.text + " " + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceSonarPassive") + ", ";
					}
					Text mainColumn31 = this.uifunctions.mainColumn;
					text = mainColumn31.text;
					mainColumn31.text = string.Concat(new string[]
					{
						text,
						this.GetFrequencies(this.uifunctions.database.databasesonardata[databaseShipData.passiveSonarID].sonarFrequencies),
						", ",
						LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceTowedArray"),
						"\n"
					});
				}
			}
			if (databaseShipData.aircraftOnBoard != null)
			{
				Text mainColumn32 = this.uifunctions.mainColumn;
				mainColumn32.text = mainColumn32.text + "\n<b>" + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceAircraft") + ":</b>\n";
				for (int num4 = 0; num4 < databaseShipData.aircraftOnBoard.Length; num4++)
				{
					Text mainColumn33 = this.uifunctions.mainColumn;
					mainColumn33.text = mainColumn33.text + databaseShipData.aircraftOnBoard[num4] + "\n";
				}
			}
			Text mainColumn34 = this.uifunctions.mainColumn;
			mainColumn34.text = mainColumn34.text + "\n<b>" + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceNotes") + ":</b>\n";
			for (int num5 = 0; num5 < databaseShipData.history.Length; num5++)
			{
				Text mainColumn35 = this.uifunctions.mainColumn;
				mainColumn35.text = mainColumn35.text + databaseShipData.history[num5] + "\n";
			}
		}
		if (museumObjectType == "aircraft")
		{
			DatabaseAircraftData databaseAircraftData = this.uifunctions.database.databaseaircraftdata[index];
			this.uifunctions.mainColumn.text = "\n<size=22><b>" + databaseAircraftData.aircraftDescriptiveName + "</b></size>\n\n";
			Text secondColumm11 = this.uifunctions.secondColumm;
			secondColumm11.text = secondColumm11.text + "\n<size=22> </size>\n\n<b>" + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceDimensions") + ":</b>\n";
			Text secondColumm12 = this.uifunctions.secondColumm;
			string text = secondColumm12.text;
			secondColumm12.text = string.Concat(new string[]
			{
				text,
				string.Format("{0:#,0}", databaseAircraftData.weight),
				" ",
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceKilogram"),
				"\n"
			});
			Text secondColumm13 = this.uifunctions.secondColumm;
			text = secondColumm13.text;
			secondColumm13.text = string.Concat(new string[]
			{
				text,
				databaseAircraftData.length.ToString(),
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceMetre"),
				" x ",
				databaseAircraftData.height.ToString(),
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceMetre"),
				"\n"
			});
			Text secondColumm14 = this.uifunctions.secondColumm;
			text = secondColumm14.text;
			secondColumm14.text = string.Concat(new string[]
			{
				text,
				databaseAircraftData.cruiseSpeed.ToString(),
				" ",
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceKnot"),
				" , ",
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceCrew"),
				": ",
				databaseAircraftData.crew.ToString()
			});
			int num6 = 0;
			Text mainColumn36 = this.uifunctions.mainColumn;
			mainColumn36.text = mainColumn36.text + "<b>" + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceOffensive") + ":</b>\n";
			if (databaseAircraftData.torpedotypes.Length > 0)
			{
				for (int num7 = 0; num7 < databaseAircraftData.torpedotypes.Length; num7++)
				{
					Text mainColumn37 = this.uifunctions.mainColumn;
					mainColumn37.text += this.uifunctions.database.databaseweapondata[databaseAircraftData.torpedoIDs[num7]].weaponName;
					if (this.uifunctions.database.databaseweapondata[databaseAircraftData.torpedoIDs[num7]].isMissile)
					{
						Text mainColumn38 = this.uifunctions.mainColumn;
						mainColumn38.text = mainColumn38.text + " " + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceMissiles") + "\n";
						num6++;
					}
					else if (this.uifunctions.database.databaseweapondata[databaseAircraftData.torpedoIDs[num7]].isDecoy)
					{
						Text mainColumn39 = this.uifunctions.mainColumn;
						mainColumn39.text = mainColumn39.text + " " + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceDecoys") + "\n";
						num6++;
					}
					else
					{
						Text mainColumn40 = this.uifunctions.mainColumn;
						mainColumn40.text = mainColumn40.text + " " + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceTorpedoes") + "\n";
						num6++;
					}
				}
			}
			if (num6 == 0)
			{
				Text mainColumn41 = this.uifunctions.mainColumn;
				mainColumn41.text = mainColumn41.text + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceNone") + "\n";
			}
			Text mainColumn42 = this.uifunctions.mainColumn;
			mainColumn42.text = mainColumn42.text + "\n<b>" + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceSensors") + ":</b>\n";
			if (databaseAircraftData.radarID > -1)
			{
				Text mainColumn43 = this.uifunctions.mainColumn;
				text = mainColumn43.text;
				mainColumn43.text = string.Concat(new string[]
				{
					text,
					this.uifunctions.database.databaseradardata[databaseAircraftData.radarID].radarDisplayName,
					" ",
					LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceSearchRADAR"),
					"\n"
				});
			}
			bool flag2 = false;
			if (databaseAircraftData.activeSonarID > -1 && databaseAircraftData.passiveSonarID > -1)
			{
				if (this.uifunctions.database.databasesonardata[databaseAircraftData.activeSonarID].sonarActiveSensitivity > 0f && this.uifunctions.database.databasesonardata[databaseAircraftData.activeSonarID].sonarPassiveSensitivity > 0f)
				{
					Text mainColumn44 = this.uifunctions.mainColumn;
					text = mainColumn44.text;
					mainColumn44.text = string.Concat(new string[]
					{
						text,
						this.uifunctions.database.databasesonardata[databaseAircraftData.activeSonarID].sonarDisplayName,
						" ",
						LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceSonarBoth"),
						", "
					});
					Text mainColumn45 = this.uifunctions.mainColumn;
					text = mainColumn45.text;
					mainColumn45.text = string.Concat(new string[]
					{
						text,
						this.GetFrequencies(this.uifunctions.database.databasesonardata[databaseAircraftData.activeSonarID].sonarFrequencies),
						", ",
						LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceDipping"),
						"\n"
					});
				}
				else
				{
					flag2 = true;
				}
			}
			else
			{
				flag2 = true;
			}
			if (flag2)
			{
				if (databaseAircraftData.activeSonarID > -1)
				{
					Text mainColumn46 = this.uifunctions.mainColumn;
					text = mainColumn46.text;
					mainColumn46.text = string.Concat(new string[]
					{
						text,
						this.uifunctions.database.databasesonardata[databaseAircraftData.activeSonarID].sonarDisplayName,
						" ",
						LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceSonarActive"),
						" , "
					});
					Text mainColumn47 = this.uifunctions.mainColumn;
					text = mainColumn47.text;
					mainColumn47.text = string.Concat(new string[]
					{
						text,
						this.GetFrequencies(this.uifunctions.database.databasesonardata[databaseAircraftData.activeSonarID].sonarFrequencies),
						", ",
						LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceDipping"),
						"\n"
					});
				}
				if (databaseAircraftData.passiveSonarID > -1)
				{
					Text mainColumn48 = this.uifunctions.mainColumn;
					text = mainColumn48.text;
					mainColumn48.text = string.Concat(new string[]
					{
						text,
						this.uifunctions.database.databasesonardata[databaseAircraftData.passiveSonarID].sonarDisplayName,
						" ",
						LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceSonarPassive"),
						", "
					});
					Text mainColumn49 = this.uifunctions.mainColumn;
					text = mainColumn49.text;
					mainColumn49.text = string.Concat(new string[]
					{
						text,
						this.GetFrequencies(this.uifunctions.database.databasesonardata[databaseAircraftData.passiveSonarID].sonarFrequencies),
						", ",
						LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceDipping"),
						"\n"
					});
				}
			}
			if (databaseAircraftData.sonobuoytypes[0] != "FALSE" && databaseAircraftData.sonobuoytypes.Length > 0)
			{
				for (int num8 = 0; num8 < databaseAircraftData.sonobuoyIDs.Length; num8++)
				{
					Text mainColumn50 = this.uifunctions.mainColumn;
					mainColumn50.text = mainColumn50.text + databaseAircraftData.sonobuoyNumbers[num8].ToString() + " x ";
					if (this.uifunctions.database.databasesonardata[databaseAircraftData.sonobuoyIDs[num8]].sonarActiveSensitivity > 0f && this.uifunctions.database.databasesonardata[databaseAircraftData.sonobuoyIDs[num8]].sonarPassiveSensitivity > 0f)
					{
						Text mainColumn51 = this.uifunctions.mainColumn;
						text = mainColumn51.text;
						mainColumn51.text = string.Concat(new string[]
						{
							text,
							this.uifunctions.database.databasesonardata[databaseAircraftData.sonobuoyIDs[num8]].sonarDisplayName,
							" ",
							LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceSonarBoth"),
							", "
						});
					}
					else if (this.uifunctions.database.databasesonardata[databaseAircraftData.sonobuoyIDs[num8]].sonarActiveSensitivity > 0f)
					{
						Text mainColumn52 = this.uifunctions.mainColumn;
						text = mainColumn52.text;
						mainColumn52.text = string.Concat(new string[]
						{
							text,
							this.uifunctions.database.databasesonardata[databaseAircraftData.sonobuoyIDs[num8]].sonarDisplayName,
							" ",
							LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceSonarActive"),
							", "
						});
					}
					else
					{
						Text mainColumn53 = this.uifunctions.mainColumn;
						text = mainColumn53.text;
						mainColumn53.text = string.Concat(new string[]
						{
							text,
							this.uifunctions.database.databasesonardata[databaseAircraftData.sonobuoyIDs[num8]].sonarDisplayName,
							" ",
							LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceSonarPassive"),
							", "
						});
					}
					Text mainColumn54 = this.uifunctions.mainColumn;
					mainColumn54.text = mainColumn54.text + this.GetFrequencies(this.uifunctions.database.databasesonardata[databaseAircraftData.sonobuoyIDs[num8]].sonarFrequencies) + ", " + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceSonobuoys");
					Text mainColumn55 = this.uifunctions.mainColumn;
					mainColumn55.text += "\n";
				}
			}
			Text mainColumn56 = this.uifunctions.mainColumn;
			mainColumn56.text = mainColumn56.text + "\n<b>" + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceNotes") + ":</b>\n";
			for (int num9 = 0; num9 < databaseAircraftData.aircraftDescription.Length; num9++)
			{
				Text mainColumn57 = this.uifunctions.mainColumn;
				mainColumn57.text = mainColumn57.text + databaseAircraftData.aircraftDescription[num9] + "\n";
			}
		}
		else if (museumObjectType == "torpedo")
		{
			DatabaseWeaponData databaseWeaponData = this.uifunctions.database.databaseweapondata[index];
			this.uifunctions.mainColumn.text = "\n<size=22><b>" + databaseWeaponData.weaponDescriptiveName + "</b></size>\n\n";
			if (!databaseWeaponData.isSonobuoy)
			{
				Text mainColumn58 = this.uifunctions.mainColumn;
				string text = mainColumn58.text;
				mainColumn58.text = string.Concat(new string[]
				{
					text,
					LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceRange"),
					": ",
					string.Format("{0:#,0}", databaseWeaponData.rangeInYards),
					" ",
					LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceYard")
				});
				if (databaseWeaponData.weaponType == "MISSILE")
				{
					Text mainColumn59 = this.uifunctions.mainColumn;
					text = mainColumn59.text;
					mainColumn59.text = string.Concat(new string[]
					{
						text,
						" ",
						LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceAt"),
						" ",
						databaseWeaponData.activeRunSpeed.ToString(),
						" ",
						LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceKnot"),
						"\n"
					});
				}
				else
				{
					Text mainColumn60 = this.uifunctions.mainColumn;
					text = mainColumn60.text;
					mainColumn60.text = string.Concat(new string[]
					{
						text,
						" ",
						LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceAt"),
						" ",
						databaseWeaponData.runSpeed.ToString(),
						" ",
						LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceKnot"),
						"\n"
					});
				}
				Text mainColumn61 = this.uifunctions.mainColumn;
				text = mainColumn61.text;
				mainColumn61.text = string.Concat(new string[]
				{
					text,
					LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceMaxSpeed"),
					": ",
					databaseWeaponData.activeRunSpeed.ToString(),
					" ",
					LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceKnot"),
					"\n"
				});
				if (databaseWeaponData.sensorRange > 0f)
				{
					Text mainColumn62 = this.uifunctions.mainColumn;
					text = mainColumn62.text;
					mainColumn62.text = string.Concat(new string[]
					{
						text,
						LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceSeekerRange"),
						": ",
						string.Format("{0:#,0}", databaseWeaponData.sensorRange),
						" ",
						LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceYard"),
						"\n"
					});
				}
				Text mainColumn63 = this.uifunctions.mainColumn;
				mainColumn63.text += "\n";
			}
			Text mainColumn64 = this.uifunctions.mainColumn;
			mainColumn64.text = mainColumn64.text + "<b>" + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceNotes") + ":</b>\n";
			for (int num10 = 0; num10 < databaseWeaponData.weaponDescription.Length; num10++)
			{
				Text mainColumn65 = this.uifunctions.mainColumn;
				mainColumn65.text = mainColumn65.text + databaseWeaponData.weaponDescription[num10] + "\n";
			}
		}
		UIFunctions.globaluifunctions.SetMainColumnHeight(true);
	}

	// Token: 0x06000858 RID: 2136 RVA: 0x0005AC3C File Offset: 0x00058E3C
	private string GetFrequencies(string[] array)
	{
		string text = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			string text2 = array[i];
			switch (text2)
			{
			case "VL":
				text += LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "FrequencyVeryLow");
				break;
			case "L":
				text += LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "FrequencyLow");
				break;
			case "M":
				text += LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "FrequencyMedium");
				break;
			case "H":
				text += LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "FrequencyHigh");
				break;
			}
			if (i < array.Length - 1)
			{
				text += ", ";
			}
		}
		if (array.Length > 1)
		{
			text = text + " " + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Frequencies");
		}
		else
		{
			text = text + " " + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Frequency");
		}
		return text;
	}

	// Token: 0x06000859 RID: 2137 RVA: 0x0005ADAC File Offset: 0x00058FAC
	public void CloseMuseum()
	{
		AudioManager.audiomanager.musicSources[0].Stop();
		AudioManager.audiomanager.musicSources[1].Stop();
		this.missionmanager.ReloadMainMenu();
	}

	// Token: 0x0600085A RID: 2138 RVA: 0x0005ADDC File Offset: 0x00058FDC
	public GameObject SpawnPlayerVessel(int vesselNumber, int shipClass)
	{
		Transform transform = this.spawnObjects[1].transform;
		GameObject gameObject = this.uifunctions.vesselbuilder.CreateVessel(shipClass, true, transform.position, transform.rotation);
		Vessel component = gameObject.GetComponent<Vessel>();
		component.databaseshipdata = this.uifunctions.database.databaseshipdata[shipClass];
		float num = UnityEngine.Random.Range(70f, 450f);
		if (!GameDataManager.missionMode && !GameDataManager.trainingMode)
		{
			if (UIFunctions.globaluifunctions.campaignmanager.playerCurrentSpeed > UIFunctions.globaluifunctions.campaignmanager.playerMapSpeeds[1])
			{
				num = UIFunctions.globaluifunctions.campaignmanager.playerStartDepths[2];
			}
			else if (UIFunctions.globaluifunctions.campaignmanager.playerCurrentSpeed > UIFunctions.globaluifunctions.campaignmanager.playerMapSpeeds[0])
			{
				num = UIFunctions.globaluifunctions.campaignmanager.playerStartDepths[1];
			}
			else
			{
				num = UIFunctions.globaluifunctions.campaignmanager.playerStartDepths[0];
			}
		}
		gameObject.transform.position = new Vector3(gameObject.transform.position.x, 1000f - num * GameDataManager.feetToUnits, gameObject.transform.position.z);
		component.vesselListIndex = vesselNumber;
		GameDataManager.playervesselsonlevel[vesselNumber] = component;
		component.playercontrolled = true;
		component.damagesystem.DamageInit();
		return gameObject;
	}

	// Token: 0x0600085B RID: 2139 RVA: 0x0005AF54 File Offset: 0x00059154
	private int DistanceModifier(int shipID)
	{
		int num = 1;
		switch (shipID)
		{
		case 0:
			num = 3;
			break;
		case 1:
			num = 3;
			break;
		case 2:
			num = 3;
			break;
		case 3:
			num = 0;
			break;
		case 4:
			num = 2;
			break;
		case 5:
			num = 1;
			break;
		case 6:
			num = 0;
			break;
		case 7:
			num = 3;
			break;
		case 8:
			num = 3;
			break;
		case 9:
			num = 0;
			break;
		case 10:
			num = 2;
			break;
		case 11:
			num = 1;
			break;
		case 12:
			num = 1;
			break;
		case 13:
			num = 0;
			break;
		}
		if (GameDataManager.isNight)
		{
			num /= 2;
		}
		return num;
	}

	// Token: 0x0600085C RID: 2140 RVA: 0x0005B01C File Offset: 0x0005921C
	public void EnvironmentSwitch(bool godeep)
	{
		if (godeep)
		{
			AudioManager.audiomanager.GoDeepAudio(0.3f);
			ManualCameraZoom.yMinLimit = -89;
			if (this.oceanblending != null)
			{
				this.oceanblending.oceanrenderer.transform.localRotation = Quaternion.Euler(0f, 180f, 180f);
			}
			ManualCameraZoom.underwater = true;
			ManualCameraZoom.y = -1f;
			ManualCameraZoom.relativeLocation = ManualCameraZoom.target.position;
			if (ManualCameraZoom.target.position.y >= 999.92f)
			{
				ManualCameraZoom.relativeLocation = new Vector3(ManualCameraZoom.target.position.x, 999.92f, ManualCameraZoom.target.position.z);
			}
			this.lightRays.gameObject.SetActive(true);
			this.skyboxobject.SetActive(false);
			RenderSettings.fog = true;
			RenderSettings.fogDensity = this.environment.actualSubmergedFogDensity;
			Color32 c = this.environment.actualSubmergedFogColor;
			RenderSettings.fogColor = c;
			Camera.main.backgroundColor = c;
			RenderSettings.ambientLight = this.environment.actualAmbientColor;
			this.depthMask.SetActive(true);
			this.MainCamera.GetComponent<Camera>().cullingMask = this.underwaterLayerMask;
			this.uifunctions.tapeBearing.SetColor("_Color", Color.white);
			this.uifunctions.bearingMarker.color = Color.white;
			this.environment.directionalLights[0].gameObject.SetActive(false);
			this.environment.directionalLights[1].gameObject.SetActive(true);
			if (GameDataManager.optionsBoolSettings[6])
			{
				this.amplifycoloreffect.LutTexture = this.amplifyColorTextures[1];
			}
		}
		else
		{
			AudioManager.audiomanager.GoSurfaceAudio(0.3f);
			this.oceanblending.oceanrenderer.transform.localRotation = Quaternion.identity;
			ManualCameraZoom.underwater = false;
			ManualCameraZoom.y = 1f;
			ManualCameraZoom.relativeLocation = ManualCameraZoom.target.position;
			if (ManualCameraZoom.target.position.y <= 1000.08f)
			{
				ManualCameraZoom.relativeLocation = new Vector3(ManualCameraZoom.target.position.x, 1000.08f, ManualCameraZoom.target.position.z);
			}
			this.lightRays.gameObject.SetActive(false);
			this.skyboxobject.SetActive(true);
			RenderSettings.fog = true;
			RenderSettings.fogColor = this.environment.actualSurfaceFogColor;
			RenderSettings.fogDensity = this.environment.actualSurfaceFogDensity;
			Camera.main.backgroundColor = this.environment.actualSubmergedFogColor;
			this.depthMask.SetActive(false);
			this.MainCamera.GetComponent<Camera>().cullingMask = this.surfaceLayerMask;
			this.uifunctions.tapeBearing.SetColor("_Color", this.tapeColor);
			this.uifunctions.bearingMarker.color = this.tapeColor;
			this.environment.directionalLights[0].gameObject.SetActive(true);
			this.environment.directionalLights[1].gameObject.SetActive(false);
			if (GameDataManager.optionsBoolSettings[6])
			{
				this.amplifycoloreffect.LutTexture = this.amplifyColorTextures[0];
			}
			else
			{
				this.amplifycoloreffect.enabled = false;
			}
		}
	}

	// Token: 0x0600085D RID: 2141 RVA: 0x0005B3B0 File Offset: 0x000595B0
	private void CreateEnvironment(int environmenttype)
	{
		this.rain.SetActive(false);
		this.snow.SetActive(false);
		if (this.packIcePresent)
		{
			this.sensormanager.seaState = 0f;
		}
		else if (this.lowWind)
		{
			this.sensormanager.seaState = (float)UnityEngine.Random.Range(0, 3);
		}
		if (this.environmentObject != null)
		{
			UnityEngine.Object.Destroy(this.environmentObject.gameObject);
		}
		if (this.oceanObjectInstance != null)
		{
			UnityEngine.Object.Destroy(this.oceanObjectInstance.gameObject);
		}
		GameObject gameObject = UnityEngine.Object.Instantiate((GameObject)Resources.Load("template_objects/environment_template"), new Vector3(0f, 1000f, 0f), Quaternion.Euler(0f, 0f, 0f)) as GameObject;
		this.environment = gameObject.GetComponent<global::Environment>();
		this.environmentSky = gameObject.GetComponentInChildren<mset.Sky>();
		string text = this.environmentNamesAndTags[environmenttype];
		string[] array = text.Split(new char[]
		{
			','
		});
		UIFunctions.globaluifunctions.textparser.ReadEnvironmentData(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("environment/" + array[0]));
		this.environment.directionalLights[0].gameObject.SetActive(true);
		this.environment.directionalLights[1].gameObject.SetActive(false);
		GameObject gameObject2 = this.SetOcean(LevelLoadManager.lowOcean);
		this.uifunctions.database.bowwaveMaterial.SetColor("_TintColor", global::Environment.whiteLevel);
		this.uifunctions.database.fireSmokeMaterial.SetColor("_TintColor", global::Environment.whiteLevel);
		this.uifunctions.database.funnelSmokeMaterial.SetColor("_TintColor", new Color(global::Environment.whiteLevel.r, global::Environment.whiteLevel.g, global::Environment.whiteLevel.b, 0.19f));
		this.uifunctions.database.whiteSmokeMaterial.SetColor("_TintColor", global::Environment.whiteLevel);
		this.environmentObject = gameObject;
		float num = this.windVane[0].transform.eulerAngles.y + 90f;
		if (num > 360f)
		{
			num -= 360f;
		}
		this.cetoOcean.windDir = num;
		this.windstrength = this.sensormanager.seaState * (0.04f / ((float)this.sensormanager.seaStates.Length - 1f));
		this.wavespectrum.windSpeed = this.windstrength / 0.04f * 4.3f + 2.3f;
		this.windVane[1].transform.localPosition = new Vector3(0f, 0f, this.windstrength);
		GameDataManager.windX = this.windVane[1].transform.position.x;
		GameDataManager.windY = this.windVane[1].transform.position.z;
		GameDataManager.windZ = 0f;
		float num2 = 50f;
		this.spawnObjects[1].transform.localPosition = new Vector3(-num2, 0f, 0f);
		this.spawnObjects[2].transform.localPosition = new Vector3(num2, 0f, 0f);
		this.spawnObjects[5].transform.localPosition = new Vector3(0f, 0f, 0f);
		this.spawnObjects[7].transform.localPosition = new Vector3(0f, 0f, 0f);
		this.spawnObjects[6].transform.localPosition = new Vector3(0f, 0f, 0f);
		this.spawnObjects[8].transform.localPosition = new Vector3(0f, 0f, 0f);
	}

	// Token: 0x0600085E RID: 2142 RVA: 0x0005B7CC File Offset: 0x000599CC
	public GameObject SetOcean(bool lowOcean)
	{
		GameObject gameObject;
		if (lowOcean)
		{
			this.cetoOcean.gameObject.SetActive(false);
			gameObject = (UnityEngine.Object.Instantiate((GameObject)Resources.Load("environment/ocean"), new Vector3(0f, 1000f, 0f), Quaternion.Euler(0f, 0f, 0f)) as GameObject);
			gameObject.SetActive(true);
		}
		else
		{
			this.cetoOcean.gameObject.SetActive(true);
			gameObject = (UnityEngine.Object.Instantiate((GameObject)Resources.Load("environment/ocean"), new Vector3(0f, 1000f, 0f), Quaternion.Euler(0f, 0f, 0f)) as GameObject);
			this.SetCetoOceanDetail((int)GameDataManager.optionsFloatSettings[5]);
		}
		this.oceanObjectInstance = gameObject;
		this.oceanblending = gameObject.GetComponent<ColorBlend>();
		if (lowOcean)
		{
			this.oceanblending.oceanrenderer.enabled = true;
		}
		else
		{
			this.oceanblending.oceanrenderer.enabled = false;
		}
		this.oceanplane = gameObject.transform;
		this.oceanTilingScript.oceanposition = this.oceanblending.oceanrenderer.transform;
		this.oceanTilingScript.enabled = true;
		global::Environment.whiteLevel = this.environment.actualLightLevel;
		return gameObject;
	}

	// Token: 0x0600085F RID: 2143 RVA: 0x0005B924 File Offset: 0x00059B24
	public void SetCetoOceanDetail(int detailLevel)
	{
		if (detailLevel == 0)
		{
			this.wavespectrum.fourierSize = FOURIER_SIZE.LOW_32_CPU;
			this.projectedgrid.resolution = MESH_RESOLUTION.LOW;
		}
		if (detailLevel == 1)
		{
			this.wavespectrum.fourierSize = FOURIER_SIZE.MEDIUM_64_CPU;
			this.projectedgrid.resolution = MESH_RESOLUTION.MEDIUM;
		}
		if (detailLevel == 2)
		{
			this.wavespectrum.fourierSize = FOURIER_SIZE.HIGH_128_CPU;
			this.projectedgrid.resolution = MESH_RESOLUTION.HIGH;
		}
	}

	// Token: 0x06000860 RID: 2144 RVA: 0x0005B990 File Offset: 0x00059B90
	private int GetEnvironment()
	{
		int weatherType = this.levelloaddata.environment;
		int num = this.levelloaddata.timeofday;
		int seaState = this.levelloaddata.seaState;
		if (!this.levelloaddata.usePresetEnvironment)
		{
			weatherType = this.GetWeatherType();
			seaState = this.GetSeaState(weatherType);
			if (GameDataManager.trainingMode || GameDataManager.missionMode)
			{
				num = this.GetTimeOfDay();
			}
		}
		if (weatherType == 4 || weatherType == 5)
		{
		}
		LevelLoadManager.isRaining = false;
		this.rain.SetActive(false);
		LevelLoadManager.isSnowing = false;
		this.snow.SetActive(false);
		LevelLoadManager.isStorm = false;
		string str = "Day";
		string str2 = "Clear";
		switch (weatherType)
		{
		case 0:
			str2 = "Clear";
			break;
		case 1:
			str2 = "Scattered";
			break;
		case 2:
			str2 = "Broken";
			break;
		case 3:
			str2 = "Overcast";
			break;
		case 4:
			str2 = "Overcast";
			LevelLoadManager.isRaining = true;
			this.rain.SetActive(true);
			break;
		case 5:
			str2 = "Overcast";
			LevelLoadManager.isRaining = true;
			this.rain.SetActive(true);
			LevelLoadManager.isStorm = true;
			break;
		case 6:
			str2 = "Overcast";
			LevelLoadManager.isSnowing = true;
			this.snow.SetActive(true);
			break;
		case 7:
			str2 = "Overcast";
			LevelLoadManager.isSnowing = true;
			this.snow.SetActive(true);
			LevelLoadManager.isStorm = true;
			break;
		}
		GameDataManager.isNight = false;
		if (num == 0)
		{
			str = "Dawn";
		}
		else if (num == 4)
		{
			str = "Dusk";
		}
		else if (num == 5)
		{
			str = "Night";
			GameDataManager.isNight = true;
		}
		this.sensormanager.seaState = (float)seaState;
		this.sensormanager.currentWeather = weatherType;
		this.cetoOcean.windDir = UnityEngine.Random.Range(0f, 360f);
		CalendarFunctions.GetMoonPhase();
		string a = str + "_" + str2;
		for (int i = 0; i < this.environmentNamesAndTags.Length; i++)
		{
			if (a == this.environmentNamesAndTags[i])
			{
				return i;
			}
		}
		return 0;
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x0005BBE0 File Offset: 0x00059DE0
	private int GetTimeOfDay()
	{
		return UnityEngine.Random.Range(0, this.sensormanager.timesOfDay.Length);
	}

	// Token: 0x06000862 RID: 2146 RVA: 0x0005BC04 File Offset: 0x00059E04
	private int GetWeatherType()
	{
		float value = UnityEngine.Random.value;
		float num = 0f;
		for (int i = 0; i < this.sensormanager.weatherProbabilities.Length; i++)
		{
			num += this.sensormanager.weatherProbabilities[i];
			if (value < num)
			{
				return i;
			}
		}
		return this.sensormanager.weatherProbabilities.Length - 1;
	}

	// Token: 0x06000863 RID: 2147 RVA: 0x0005BC64 File Offset: 0x00059E64
	private int GetSeaState(int weatherType)
	{
		float num = UnityEngine.Random.value;
		float num2 = 0f;
		if (weatherType == 5 || weatherType == 7)
		{
			num = (float)UnityEngine.Random.Range(this.sensormanager.seaStateProbabilities.Length - 2, this.sensormanager.seaStateProbabilities.Length);
		}
		for (int i = 0; i < this.sensormanager.seaStateProbabilities.Length; i++)
		{
			num2 += this.sensormanager.seaStateProbabilities[i];
			if (num < num2)
			{
				return i;
			}
		}
		return this.sensormanager.seaStateProbabilities.Length - 1;
	}

	// Token: 0x06000864 RID: 2148 RVA: 0x0005BCF4 File Offset: 0x00059EF4
	public void CheckSpawnDepth(Vessel activeVessel)
	{
		float num = 1000f + this.GetTerrainHeightAtPositionFromHeightMap(activeVessel.transform.position.x, activeVessel.transform.position.z);
		if (num < 999.5f)
		{
			if (activeVessel.transform.position.y < num)
			{
				float num2 = num + 100f * GameDataManager.feetToUnits;
				if (num2 > 1000f)
				{
					num2 = 1000f;
				}
				activeVessel.transform.position = new Vector3(activeVessel.transform.position.x, num2, activeVessel.transform.position.z);
			}
		}
		else
		{
			Vector2[] array = new Vector2[]
			{
				new Vector2(0f, 20f),
				new Vector2(0f, -20f),
				new Vector2(20f, 0f),
				new Vector2(-20f, 0f),
				new Vector2(0f, 40f),
				new Vector2(0f, -40f),
				new Vector2(40f, 0f),
				new Vector2(-40f, 0f),
				new Vector2(0f, 60f),
				new Vector2(0f, -60f),
				new Vector2(60f, 0f),
				new Vector2(-60f, 0f),
				new Vector2(0f, 80f),
				new Vector2(0f, -80f),
				new Vector2(80f, 0f),
				new Vector2(-80f, 0f),
				new Vector2(0f, 100f),
				new Vector2(0f, -100f),
				new Vector2(100f, 0f),
				new Vector2(-100f, 0f),
				new Vector2(0f, 200f),
				new Vector2(0f, -200f),
				new Vector2(200f, 0f),
				new Vector2(-200f, 0f)
			};
			bool flag = false;
			for (int i = 0; i < array.Length; i++)
			{
				num = 1000f + this.GetTerrainHeightAtPositionFromHeightMap(activeVessel.transform.position.x + array[i].x, activeVessel.transform.position.z + array[i].y);
				if (num < 999.5f)
				{
					float y = 1000f;
					if (activeVessel.databaseshipdata.shipType == "SUBMARINE" || activeVessel.databaseshipdata.shipType == "BIOLOGIC")
					{
						y = 999.8f;
					}
					activeVessel.transform.Translate(Vector3.forward * array[i].y, Space.World);
					activeVessel.transform.Translate(Vector3.right * array[i].x, Space.World);
					activeVessel.transform.position = new Vector3(activeVessel.transform.position.x, y, activeVessel.transform.position.z);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (activeVessel.databaseshipdata.shipType == "BIOLOGIC")
				{
					activeVessel.vesselmovement.shipSpeed = Vector3.zero;
					activeVessel.gameObject.SetActive(false);
				}
				else if (!activeVessel.playercontrolled)
				{
					Debug.LogError("CheckSpawnDepth could not find a valid position for " + activeVessel.name);
					activeVessel.vesselRemovedFromCombat = true;
					activeVessel.isSinking = true;
					activeVessel.gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x06000865 RID: 2149 RVA: 0x0005C214 File Offset: 0x0005A414
	public float GetTerrainHeightAtPosition(float x, float z)
	{
		LayerMask mask = 1073741824;
		Vector3 origin = new Vector3(x, 1030f, z);
		float result = 5000f;
		RaycastHit raycastHit;
		if (Physics.Raycast(origin, -Vector3.up, out raycastHit, 130f, mask))
		{
			result = -30f + raycastHit.distance;
		}
		return result;
	}

	// Token: 0x06000866 RID: 2150 RVA: 0x0005C274 File Offset: 0x0005A474
	private void PresetLocations()
	{
		float y = GameDataManager.playervesselsonlevel[0].gameObject.transform.position.y;
		if (this.levelloaddata.rnShipDepth != -1f)
		{
			y = 1000f - this.levelloaddata.rnShipDepth * GameDataManager.feetToUnits;
		}
		GameDataManager.playervesselsonlevel[0].gameObject.transform.position = new Vector3(this.levelloaddata.rnShipPosition.x, y, this.levelloaddata.rnShipPosition.y);
		GameDataManager.playervesselsonlevel[0].gameObject.transform.rotation = Quaternion.Slerp(GameDataManager.playervesselsonlevel[0].gameObject.transform.rotation, Quaternion.Euler(0f, this.levelloaddata.rnShipHeading, 0f), 1f);
		int num = 0;
		for (int i = 0; i < this.levelloaddata.kmShipClasses.Length - this.worldVessels.Count; i++)
		{
			if (this.levelloaddata.enemyWaypoints[i].Contains("ANCHORED"))
			{
				string[] array = this.levelloaddata.enemyWaypoints[i].Split(new char[]
				{
					','
				});
				GameDataManager.enemyvesselsonlevel[i].vesselai.hasMissile = false;
				GameDataManager.enemyvesselsonlevel[i].vesselai.hasMissileDefense = false;
				GameDataManager.enemyvesselsonlevel[i].vesselai.hasNoiseMaker = false;
				GameDataManager.enemyvesselsonlevel[i].vesselai.hasRBU = false;
				GameDataManager.enemyvesselsonlevel[i].vesselai.hasTorpedo = false;
				GameDataManager.enemyvesselsonlevel[i].vesselmovement.atAnchor = true;
				GameDataManager.enemyvesselsonlevel[i].vesselmovement.cavBubbles.gameObject.SetActive(false);
				GameDataManager.enemyvesselsonlevel[i].vesselmovement.bowwave.gameObject.SetActive(false);
				GameDataManager.enemyvesselsonlevel[i].damagesystem.funnelSmoke.gameObject.SetActive(false);
				GameDataManager.enemyvesselsonlevel[i].vesselmovement.telegraphValue = 0;
				if (this.levelloaddata.enemyWaypoints[i].Contains("NOMISSILE"))
				{
					GameDataManager.enemyvesselsonlevel[i].vesselmovement.notMissileTarget = true;
				}
				bool flag = false;
				if (this.levelloaddata.enemyWaypoints[i].Contains("DOCKED") && num < this.levelloaddata.shipDockLocations.Length)
				{
					float num2 = 1f;
					if (this.levelloaddata.enemyWaypoints[i].Contains("DOCKED50"))
					{
						num2 = 0.5f;
					}
					else if (this.levelloaddata.enemyWaypoints[i].Contains("DOCKED25"))
					{
						num2 = 0.25f;
					}
					else if (this.levelloaddata.enemyWaypoints[i].Contains("DOCKED75"))
					{
						num2 = 0.75f;
					}
					if (UnityEngine.Random.value < num2)
					{
						flag = true;
					}
				}
				if (flag)
				{
					GameDataManager.enemyvesselsonlevel[i].gameObject.transform.position = this.levelloaddata.shipDockLocations[num].position;
					GameDataManager.enemyvesselsonlevel[i].gameObject.transform.rotation = this.levelloaddata.shipDockLocations[num].rotation;
					if (this.levelloaddata.shipDockLocations[num].name.Contains("PORT"))
					{
						GameDataManager.enemyvesselsonlevel[i].gameObject.transform.Translate(Vector3.right * GameDataManager.enemyvesselsonlevel[i].bouyancyCompartments[0].transform.localPosition.x * -2.2f);
					}
					else
					{
						GameDataManager.enemyvesselsonlevel[i].gameObject.transform.Translate(Vector3.right * GameDataManager.enemyvesselsonlevel[i].bouyancyCompartments[0].transform.localPosition.x * 2.2f);
					}
					float d = 1f;
					if (UnityEngine.Random.value < 0.5f)
					{
						d = -1f;
						GameDataManager.enemyvesselsonlevel[i].gameObject.transform.rotation = Quaternion.Euler(0f, this.levelloaddata.shipDockLocations[num].eulerAngles.y + 180f, 0f);
					}
					GameDataManager.enemyvesselsonlevel[i].gameObject.transform.Translate(Vector3.forward * GameDataManager.enemyvesselsonlevel[i].bouyancyCompartments[2].transform.localPosition.z * -0.5f * d);
					GameDataManager.enemyvesselsonlevel[i].vesselai.isDocked = true;
					num++;
				}
				else if (this.levelloaddata.enemyWaypoints[i].Contains("EXACT"))
				{
					GameDataManager.enemyvesselsonlevel[i].gameObject.transform.position = new Vector3(float.Parse(array[0]), 1000f, float.Parse(array[1]));
					GameDataManager.enemyvesselsonlevel[i].gameObject.transform.rotation = Quaternion.Euler(0f, float.Parse(array[2]), 0f);
				}
				else
				{
					Vector2 vector = UnityEngine.Random.insideUnitCircle * float.Parse(array[2]);
					GameDataManager.enemyvesselsonlevel[i].gameObject.transform.position = new Vector3(float.Parse(array[0]) + vector.x, 1000f, float.Parse(array[1]) + vector.y);
					GameDataManager.enemyvesselsonlevel[i].gameObject.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
				}
			}
			else
			{
				string[] array = this.levelloaddata.enemyWaypoints[i].Split(new char[]
				{
					'|'
				});
				GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata.unitWaypoints = new Vector2[array.Length];
				for (int j = 0; j < array.Length; j++)
				{
					GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata.unitWaypoints[j] = UIFunctions.globaluifunctions.textparser.PopulateVector2(array[j]);
				}
				if (UnityEngine.Random.value < 0.5f)
				{
					GameDataManager.enemyvesselsonlevel[i].vesselai.reverseWaypoints = true;
				}
				else
				{
					GameDataManager.enemyvesselsonlevel[i].vesselai.reverseWaypoints = false;
				}
				int num3 = UnityEngine.Random.Range(0, GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata.unitWaypoints.Length);
				GameDataManager.enemyvesselsonlevel[i].vesselai.waypointNumber = num3;
				GameDataManager.enemyvesselsonlevel[i].gameObject.transform.position = new Vector3(GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata.unitWaypoints[num3].x, GameDataManager.enemyvesselsonlevel[i].gameObject.transform.position.y, GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata.unitWaypoints[num3].y);
				GameDataManager.enemyvesselsonlevel[i].vesselai.GetNextWaypoint();
				Vector3 vector2 = new Vector3(GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata.unitWaypoints[GameDataManager.enemyvesselsonlevel[i].vesselai.waypointNumber].x, GameDataManager.enemyvesselsonlevel[i].gameObject.transform.position.y, GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata.unitWaypoints[GameDataManager.enemyvesselsonlevel[i].vesselai.waypointNumber].y);
				GameDataManager.enemyvesselsonlevel[i].gameObject.transform.LookAt(vector2);
				float num4 = Vector3.Distance(GameDataManager.enemyvesselsonlevel[i].gameObject.transform.position, vector2);
				GameDataManager.enemyvesselsonlevel[i].gameObject.transform.Translate(Vector3.forward * UnityEngine.Random.Range(0f, num4 * 0.75f), Space.Self);
			}
		}
		for (int k = 0; k < this.levelloaddata.numberOfHelicopters; k++)
		{
			UIFunctions.globaluifunctions.combatai.enemyHelicopters[k].transform.position = new Vector3(this.levelloaddata.aircraftSearchAreas[k].x, UIFunctions.globaluifunctions.combatai.enemyHelicopters[k].transform.position.y, this.levelloaddata.aircraftSearchAreas[k].y);
		}
		for (int l = 0; l < this.levelloaddata.numberOfAircraft; l++)
		{
			UIFunctions.globaluifunctions.combatai.enemyAircraft[l].transform.position = new Vector3(this.levelloaddata.aircraftSearchAreas[l + this.levelloaddata.numberOfHelicopters].x, UIFunctions.globaluifunctions.combatai.enemyAircraft[l].transform.position.y, this.levelloaddata.aircraftSearchAreas[l + this.levelloaddata.numberOfHelicopters].y);
		}
		if (this.levelloaddata.proximityMineLocations.Length > 0)
		{
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.mineFields = new BoxCollider[this.levelloaddata.proximityMineLocations.Length];
		}
		for (int m = 0; m < this.levelloaddata.proximityMineLocations.Length; m++)
		{
			Vector3 position = new Vector3(this.levelloaddata.proximityMineLocations[m].x, 1000f, this.levelloaddata.proximityMineLocations[m].y);
			GameObject gameObject = UnityEngine.Object.Instantiate(this.levelloaddata.formationPrefab, position, Quaternion.identity) as GameObject;
			gameObject.name = "minefield";
			gameObject.layer = 27;
			BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
			boxCollider.size = new Vector3(this.levelloaddata.proximityMineParameters[m].z, 10f, this.levelloaddata.proximityMineParameters[m].w);
			boxCollider.center = new Vector3(0f, -4.5f, 0f);
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.mineFields[m] = boxCollider;
			float num5 = this.levelloaddata.proximityMineParameters[m].z / this.levelloaddata.proximityMineParameters[m].x;
			float num6 = this.levelloaddata.proximityMineParameters[m].w / this.levelloaddata.proximityMineParameters[m].y;
			float num7 = num5 * this.levelloaddata.proximityMineScatter[m];
			float num8 = num6 * this.levelloaddata.proximityMineScatter[m];
			float num9 = this.GetTerrainHeightAtPositionFromHeightMap(position.x, position.z) + 0.4f;
			Vector3 vector3 = new Vector3((this.levelloaddata.proximityMineParameters[m].z - 1f) / -2f, 0f, 0f);
			bool flag2 = false;
			int num10 = 0;
			while ((float)num10 < this.levelloaddata.proximityMineParameters[m].x)
			{
				vector3.z = (this.levelloaddata.proximityMineParameters[m].w + 1f) / -2f;
				int num11 = 0;
				while ((float)num11 < this.levelloaddata.proximityMineParameters[m].y)
				{
					vector3.z += num6;
					Vector3 instancePostition = vector3;
					instancePostition.y = -0.18f + num9 * UnityEngine.Random.value;
					if (this.levelloaddata.proximityMineScatter[m] > 0f)
					{
						instancePostition.x += UnityEngine.Random.Range(-num7, num7);
						instancePostition.z += UnityEngine.Random.Range(-num8, num8);
					}
					if (flag2)
					{
						instancePostition.x += num5 / 2f;
						flag2 = false;
					}
					else
					{
						flag2 = true;
					}
					this.SpawnProximityMine(gameObject.transform, instancePostition);
					num11++;
				}
				vector3.x += num5;
				num10++;
			}
			gameObject.transform.rotation = Quaternion.Euler(0f, this.levelloaddata.proximityMineAngles[m], 0f);
			gameObject.transform.SetParent(UIFunctions.globaluifunctions.levelloadmanager.currentMapGeneratorInstance.transform);
		}
		if (GameDataManager.trainingMode && this.levelloaddata.usePresetPositions)
		{
			if (this.levelloaddata.rnShipTelegraph != -10)
			{
				GameDataManager.playervesselsonlevel[0].vesselmovement.telegraphValue = this.levelloaddata.rnShipTelegraph;
				GameDataManager.playervesselsonlevel[0].vesselmovement.shipSpeed.z = 0f;
				GameDataManager.playervesselsonlevel[0].vesselmovement.engineSpeed = Vector2.zero;
			}
			for (int n = 0; n < GameDataManager.enemyvesselsonlevel.Length; n++)
			{
				GameDataManager.enemyvesselsonlevel[n].transform.position = new Vector3(this.levelloaddata.enemyShipPositionsX[n], GameDataManager.enemyvesselsonlevel[n].transform.position.y, this.levelloaddata.enemyShipPositionsZ[n]);
				GameDataManager.enemyvesselsonlevel[n].transform.rotation = Quaternion.Euler(0f, this.levelloaddata.enemyshipHeadings[n], 0f);
			}
		}
	}

	// Token: 0x06000867 RID: 2151 RVA: 0x0005D118 File Offset: 0x0005B318
	private void SpawnProximityMine(Transform master, Vector3 instancePostition)
	{
		Projectile_DepthCharge component = ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.databasedepthchargedata[this.proximityMineIndex].depthChargeObject, Vector3.zero, Quaternion.Euler(0f, (float)UnityEngine.Random.Range(0, 360), 0f)).GetComponent<Projectile_DepthCharge>();
		component.transform.SetParent(master);
		component.transform.localPosition = instancePostition;
		component.isProximityMine = true;
		component.gameObject.SetActive(true);
		component.blastzone.collisionvolume.radius = UIFunctions.globaluifunctions.database.databasedepthchargedata[this.proximityMineIndex].killRadius * GameDataManager.inverseYardsScale;
		component.blastzone.warhead = UIFunctions.globaluifunctions.database.databasedepthchargedata[this.proximityMineIndex].warhead;
	}

	// Token: 0x04000C6B RID: 3179
	public LevelLoadData levelloaddata;

	// Token: 0x04000C6C RID: 3180
	public PlayerFunctions playerfunctions;

	// Token: 0x04000C6D RID: 3181
	public GameDataManager gamedatamanager;

	// Token: 0x04000C6E RID: 3182
	public SensorManager sensormanager;

	// Token: 0x04000C6F RID: 3183
	public Clock waveclockx;

	// Token: 0x04000C70 RID: 3184
	public Clock waveclocky;

	// Token: 0x04000C71 RID: 3185
	public UIFunctions uifunctions;

	// Token: 0x04000C72 RID: 3186
	public MissionManager missionmanager;

	// Token: 0x04000C73 RID: 3187
	public TacticalMap tacticalmap;

	// Token: 0x04000C74 RID: 3188
	public GameObject GUICamera;

	// Token: 0x04000C75 RID: 3189
	public GameObject HUDCamera;

	// Token: 0x04000C76 RID: 3190
	public GameObject MainCamera;

	// Token: 0x04000C77 RID: 3191
	public GameObject oceanObjectInstance;

	// Token: 0x04000C78 RID: 3192
	public OceanTiling oceanTilingScript;

	// Token: 0x04000C79 RID: 3193
	public GameObject environmentObject;

	// Token: 0x04000C7A RID: 3194
	public GameObject primaryLight;

	// Token: 0x04000C7B RID: 3195
	public Quaternion primaryRotation;

	// Token: 0x04000C7C RID: 3196
	public ColorBlend oceanblending;

	// Token: 0x04000C7D RID: 3197
	public GameObject skyboxobject;

	// Token: 0x04000C7E RID: 3198
	public mset.Sky environmentSky;

	// Token: 0x04000C7F RID: 3199
	public GameObject oceanhigh;

	// Token: 0x04000C80 RID: 3200
	public OceanTiling oceantiles;

	// Token: 0x04000C81 RID: 3201
	public Transform oceanplane;

	// Token: 0x04000C82 RID: 3202
	public int museumObjectNumber;

	// Token: 0x04000C83 RID: 3203
	public Material guimaterial;

	// Token: 0x04000C84 RID: 3204
	public Material hudmaterial;

	// Token: 0x04000C85 RID: 3205
	public Material skyboxmaterial;

	// Token: 0x04000C86 RID: 3206
	public GameObject starshell;

	// Token: 0x04000C87 RID: 3207
	public Transform panCam;

	// Token: 0x04000C88 RID: 3208
	public Vector3 panVelocity;

	// Token: 0x04000C89 RID: 3209
	public float panSpeed;

	// Token: 0x04000C8A RID: 3210
	public GameObject rain;

	// Token: 0x04000C8B RID: 3211
	public GameObject snow;

	// Token: 0x04000C8C RID: 3212
	public static bool isRaining;

	// Token: 0x04000C8D RID: 3213
	public static bool isSnowing;

	// Token: 0x04000C8E RID: 3214
	public static bool isStorm;

	// Token: 0x04000C8F RID: 3215
	public GameObject underwaterFX;

	// Token: 0x04000C90 RID: 3216
	public GameObject depthMask;

	// Token: 0x04000C91 RID: 3217
	public ColorBlend colorblend;

	// Token: 0x04000C92 RID: 3218
	public global::Environment environment;

	// Token: 0x04000C93 RID: 3219
	public int environmenttype;

	// Token: 0x04000C94 RID: 3220
	public Color tapeColor;

	// Token: 0x04000C95 RID: 3221
	public Transform[] spawnObjects;

	// Token: 0x04000C96 RID: 3222
	public float spacing;

	// Token: 0x04000C97 RID: 3223
	public int[] shipSlots;

	// Token: 0x04000C98 RID: 3224
	public int[] coreslots;

	// Token: 0x04000C99 RID: 3225
	public int[] edgeslots;

	// Token: 0x04000C9A RID: 3226
	public int[] isincore;

	// Token: 0x04000C9B RID: 3227
	public int numberCoreColumns;

	// Token: 0x04000C9C RID: 3228
	public int numberSubCoreColumns;

	// Token: 0x04000C9D RID: 3229
	public static float windY;

	// Token: 0x04000C9E RID: 3230
	public int currentMission;

	// Token: 0x04000C9F RID: 3231
	public bool missionCombat;

	// Token: 0x04000CA0 RID: 3232
	public Vessel[] pv;

	// Token: 0x04000CA1 RID: 3233
	public Vessel[] ev;

	// Token: 0x04000CA2 RID: 3234
	public Transform[] windVane;

	// Token: 0x04000CA3 RID: 3235
	public float windstrength;

	// Token: 0x04000CA4 RID: 3236
	public GameObject moonObject;

	// Token: 0x04000CA5 RID: 3237
	public Transform directorTapePosition;

	// Token: 0x04000CA6 RID: 3238
	public Transform scopeTransformPosition;

	// Token: 0x04000CA7 RID: 3239
	public GameObject lightRays;

	// Token: 0x04000CA8 RID: 3240
	public static bool lowOcean;

	// Token: 0x04000CA9 RID: 3241
	public GameObject detailedOceanObject;

	// Token: 0x04000CAA RID: 3242
	public Ocean cetoOcean;

	// Token: 0x04000CAB RID: 3243
	public WaveSpectrum wavespectrum;

	// Token: 0x04000CAC RID: 3244
	public PlanarReflection planarreflection;

	// Token: 0x04000CAD RID: 3245
	public ProjectedGrid projectedgrid;

	// Token: 0x04000CAE RID: 3246
	public UnderWater cetoUnderwater;

	// Token: 0x04000CAF RID: 3247
	public int currentMuseumItem;

	// Token: 0x04000CB0 RID: 3248
	public GameObject currentMuseumInstantiatedObject;

	// Token: 0x04000CB1 RID: 3249
	public GameObject mapGeneratorObject;

	// Token: 0x04000CB2 RID: 3250
	public GameObject currentMapGeneratorInstance;

	// Token: 0x04000CB3 RID: 3251
	public float minimumIcebergDepth;

	// Token: 0x04000CB4 RID: 3252
	public LayerMask surfaceLayerMask;

	// Token: 0x04000CB5 RID: 3253
	public LayerMask underwaterLayerMask;

	// Token: 0x04000CB6 RID: 3254
	public SubmarineMarker submarineMarker;

	// Token: 0x04000CB7 RID: 3255
	public static bool inMuseum;

	// Token: 0x04000CB8 RID: 3256
	public Transform museumThreshold;

	// Token: 0x04000CB9 RID: 3257
	public GameObject playerTowedArray;

	// Token: 0x04000CBA RID: 3258
	public Transform playerTowedArrayEnd;

	// Token: 0x04000CBB RID: 3259
	public AmplifyColorEffect amplifycoloreffect;

	// Token: 0x04000CBC RID: 3260
	public AmplifyOcclusionEffect amplifyocclusioneffect;

	// Token: 0x04000CBD RID: 3261
	public AmplifyBloomEffect amplifybloom;

	// Token: 0x04000CBE RID: 3262
	public Texture[] amplifyColorTextures;

	// Token: 0x04000CBF RID: 3263
	public MeshRenderer periscopeMaskRenderer;

	// Token: 0x04000CC0 RID: 3264
	public float[] iceThresholds;

	// Token: 0x04000CC1 RID: 3265
	public bool icePresent;

	// Token: 0x04000CC2 RID: 3266
	public bool packIcePresent;

	// Token: 0x04000CC3 RID: 3267
	public string[] environmentNamesAndTags;

	// Token: 0x04000CC4 RID: 3268
	public int environmentTemperature;

	// Token: 0x04000CC5 RID: 3269
	public Vector3 museumLightingAngle;

	// Token: 0x04000CC6 RID: 3270
	public string[] periscopeMasks;

	// Token: 0x04000CC7 RID: 3271
	public string lastHUDBuilt;

	// Token: 0x04000CC8 RID: 3272
	public List<int[]> worldObjectsDictionary;

	// Token: 0x04000CC9 RID: 3273
	public List<LevelLoadManager.SceneryData> sceneryDataList;

	// Token: 0x04000CCA RID: 3274
	public bool lowWind;

	// Token: 0x04000CCB RID: 3275
	public bool shipDebugModeOn;

	// Token: 0x04000CCC RID: 3276
	public int currentShipDebugIndex;

	// Token: 0x04000CCD RID: 3277
	public int proximityMineIndex;

	// Token: 0x04000CCE RID: 3278
	public Material sceneryTreeMaterial;

	// Token: 0x04000CCF RID: 3279
	public GameObject museumFilters;

	// Token: 0x04000CD0 RID: 3280
	public Image nationImage;

	// Token: 0x04000CD1 RID: 3281
	public Sprite[] nationSprites;

	// Token: 0x04000CD2 RID: 3282
	public int currentNationFilter;

	// Token: 0x04000CD3 RID: 3283
	private string[] nations = new string[]
	{
		"all",
		"usn",
		"wp",
		"plan"
	};

	// Token: 0x04000CD4 RID: 3284
	public Button[] filterButtons;

	// Token: 0x04000CD5 RID: 3285
	private string currentUnitFilter;

	// Token: 0x04000CD6 RID: 3286
	public List<int> currentFilteredVessels;

	// Token: 0x04000CD7 RID: 3287
	public List<int> currentFilteredAircraft;

	// Token: 0x04000CD8 RID: 3288
	public List<int> currentFilteredWeapons;

	// Token: 0x04000CD9 RID: 3289
	public List<LevelLoadManager.SceneryData> worldVessels;

	// Token: 0x02000133 RID: 307
	public class SceneryData
	{
		// Token: 0x04000CDC RID: 3292
		public Vector2 sceneryCoords;

		// Token: 0x04000CDD RID: 3293
		public Vector3 sceneryMeshPosition;

		// Token: 0x04000CDE RID: 3294
		public Vector3 sceneryMeshRotation;

		// Token: 0x04000CDF RID: 3295
		public string sceneryPath;

		// Token: 0x04000CE0 RID: 3296
		public string vesselPrefabName = string.Empty;

		// Token: 0x04000CE1 RID: 3297
		public List<string> childObjects;

		// Token: 0x04000CE2 RID: 3298
		public List<Vector3> childObjectPositions;

		// Token: 0x04000CE3 RID: 3299
		public List<Vector3> childObjectRotations;
	}
}
