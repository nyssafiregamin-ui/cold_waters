using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000155 RID: 341
public class SensorManager : MonoBehaviour
{
	// Token: 0x060009D0 RID: 2512 RVA: 0x000734FC File Offset: 0x000716FC
	public void InitialiseSensorManager()
	{
		this.addTMA = false;
		this.noiseThresholds = new float[]
		{
			120f,
			130f,
			140f,
			150f,
			160f,
			170f
		};
		this.strongestRadarSignal = 0f;
		this.actualHighFrequencyNavSonarRange = this.highFrequencyNavSonarRange * GameDataManager.inverseYardsScale;
		this.visualRange = 30000f;
		if (LevelLoadManager.isRaining || LevelLoadManager.isSnowing || GameDataManager.isNight)
		{
			this.visualRange /= 2f;
		}
		if (LevelLoadManager.isStorm)
		{
			this.visualRange /= 2f;
		}
		if (GameDataManager.playerCommanderName == "TheTruthIsOutThere")
		{
			this.truthIsOutThere = true;
		}
		this.targetNoisePerKnot = 1f;
	}

	// Token: 0x060009D1 RID: 2513 RVA: 0x000735C4 File Offset: 0x000717C4
	public void AddTorpedoToArray(Torpedo objectToAdd)
	{
		int num = this.torpedoObjects.Length;
		int num2 = num + 1;
		Torpedo[] array = new Torpedo[num2];
		array[0] = objectToAdd;
		array[0].torpedoID = 0;
		for (int i = 1; i < num2; i++)
		{
			array[i] = this.torpedoObjects[i - 1];
			array[i].torpedoID = i;
		}
		this.torpedoObjects = array;
		if (objectToAdd.databaseweapondata.weaponType == "MISSILE")
		{
			return;
		}
		if (objectToAdd.tacMapTorpedoIcon != null)
		{
			objectToAdd.tacMapTorpedoIcon.name = num.ToString();
			objectToAdd.tacMapTorpedoIcon.transform.SetParent(this.tacticalmap.torpedoLayer.transform, false);
			objectToAdd.tacMapTorpedoIcon.transform.localPosition = new Vector3(objectToAdd.transform.position.x * this.tacticalmap.zoomFactor, objectToAdd.transform.position.z * this.tacticalmap.zoomFactor, -5f);
			objectToAdd.tacMapTorpedoIcon.shipRectTransform.transform.rotation = Quaternion.Euler(0f, 180f, objectToAdd.transform.eulerAngles.y);
			objectToAdd.tacMapTorpedoIcon.contactButton = objectToAdd.tacMapTorpedoIcon.GetComponentInChildren<Button>();
			objectToAdd.tacMapTorpedoIcon.contactButton.onClick.RemoveAllListeners();
			objectToAdd.tacMapTorpedoIcon.contactButton.onClick.AddListener(delegate()
			{
				UIFunctions.globaluifunctions.playerfunctions.MapTorpedoButton(objectToAdd.transform, objectToAdd.whichNavy);
			});
			float num3 = this.tacticalmap.iconSize * this.tacticalmap.orthFactor;
			objectToAdd.tacMapTorpedoIcon.shipRectTransform.sizeDelta = new Vector2(num3, num3);
		}
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x000737FC File Offset: 0x000719FC
	public void RemoveTorpedoFromArray(int x)
	{
		if (x < this.torpedoObjects.Length && this.torpedoObjects[x].databaseweapondata.weaponType == "TORPEDO")
		{
			this.torpedoObjects[x].tacMapTorpedoIcon.contactButton.onClick.RemoveListener(delegate()
			{
				UIFunctions.globaluifunctions.playerfunctions.MapTorpedoButton(this.torpedoObjects[x].transform, 0);
			});
		}
		int num = this.torpedoObjects.Length;
		if (num - 1 > 0)
		{
			Torpedo[] array = new Torpedo[num - 1];
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (i != x)
				{
					array[num2] = this.torpedoObjects[i];
					array[num2].torpedoID = num2;
					num2++;
				}
			}
			this.torpedoObjects = array;
		}
		else
		{
			this.torpedoObjects = new Torpedo[0];
		}
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x000738F8 File Offset: 0x00071AF8
	public void AddSonobuoyToArray(Noisemaker objectToAdd, SensorData sensorData)
	{
		int num = sensorData.sonobuoys.Length;
		int num2 = num + 1;
		Noisemaker[] array = new Noisemaker[num2];
		array[0] = objectToAdd;
		array[0].countermeasureArrayID = 0;
		for (int i = 1; i < num2; i++)
		{
			array[i] = sensorData.sonobuoys[i - 1];
			array[i].countermeasureArrayID = i;
		}
		sensorData.sonobuoys = array;
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x00073958 File Offset: 0x00071B58
	public void AddNoiseMakerToArray(Noisemaker objectToAdd)
	{
		int num = this.noisemakerObjects.Length;
		int num2 = num + 1;
		Noisemaker[] array = new Noisemaker[num2];
		array[0] = objectToAdd;
		array[0].countermeasureArrayID = 0;
		for (int i = 1; i < num2; i++)
		{
			array[i] = this.noisemakerObjects[i - 1];
			array[i].countermeasureArrayID = i;
		}
		this.noisemakerObjects = array;
		objectToAdd.tacMapNoisemakerIcon.name = num.ToString();
		objectToAdd.tacMapNoisemakerIcon.transform.SetParent(this.tacticalmap.noisemakerLayer.transform, false);
		objectToAdd.tacMapNoisemakerIcon.transform.localPosition = new Vector3(objectToAdd.transform.position.x * this.tacticalmap.zoomFactor, objectToAdd.transform.position.z * this.tacticalmap.zoomFactor, -5f);
		objectToAdd.tacMapNoisemakerIcon.shipRectTransform.transform.rotation = Quaternion.identity;
		float num3 = this.tacticalmap.iconSize * this.tacticalmap.orthFactor;
		objectToAdd.tacMapNoisemakerIcon.shipRectTransform.sizeDelta = new Vector2(num3, num3);
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x00073A90 File Offset: 0x00071C90
	public void RemoveNoiseMakerFromArray(int x)
	{
		int num = this.noisemakerObjects.Length;
		if (num == 0)
		{
			return;
		}
		Noisemaker[] array = new Noisemaker[num - 1];
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			if (i != x)
			{
				array[num2] = this.noisemakerObjects[i];
				array[num2].countermeasureArrayID = num2;
				num2++;
			}
		}
		this.noisemakerObjects = array;
	}

	// Token: 0x060009D6 RID: 2518 RVA: 0x00073AF0 File Offset: 0x00071CF0
	private void ZeroOcean()
	{
		this.layerStrength = 0f;
		this.layerDepthInFeet = 0f;
		this.surfaceDuctStrength = 0f;
		this.convergenceZoneStrength = 0f;
	}

	// Token: 0x060009D7 RID: 2519 RVA: 0x00073B2C File Offset: 0x00071D2C
	public float GetlayerDepthInFeet()
	{
		float num = UnityEngine.Random.Range(80f, 250f);
		if (this.oceanFloorDepthInFeet - num > 1000f && UnityEngine.Random.value < 0.2f)
		{
			num *= 1.5f;
		}
		return num;
	}

	// Token: 0x060009D8 RID: 2520 RVA: 0x00073B74 File Offset: 0x00071D74
	public void InitialiseOceanCharacteristics()
	{
		this.ZeroOcean();
		this.oceanFloorDepthInFeet = UnityEngine.Random.Range(4000f, 9000f);
		if (UIFunctions.globaluifunctions.levelloadmanager.currentMapGeneratorInstance != null)
		{
			this.oceanFloorDepthInFeet = (1000f - UIFunctions.globaluifunctions.levelloadmanager.GetTerrainHeightAtPositionFromHeightMap(0f, 0f)) * GameDataManager.unitsToFeet;
			if (this.oceanFloorDepthInFeet < 0f)
			{
				this.oceanFloorDepthInFeet = 200f;
			}
		}
		this.oceanFloorDepth = 1000f - this.oceanFloorDepthInFeet * GameDataManager.feetToUnits;
		if (this.oceanFloorDepthInFeet >= UnityEngine.Random.Range(600f, 800f))
		{
			if (UnityEngine.Random.value > 0.2f)
			{
				if (this.oceanFloorDepthInFeet - this.layerDepthInFeet < UnityEngine.Random.Range(100f, 300f))
				{
					this.layerStrength = 0f;
					this.surfaceDuctStrength = 0f;
				}
				else
				{
					this.layerDepthInFeet = this.GetlayerDepthInFeet();
					this.surfaceDuctStrength -= this.seaState * 5f;
					this.layerStrength = UnityEngine.Random.value;
					this.layerStrength -= this.seaState * 0.05f;
					this.surfaceDuctStrength = this.layerStrength * UnityEngine.Random.Range(0.8f, 1.2f);
					this.surfaceDuctStrength += this.layerDepthInFeet / 1000f;
					this.surfaceDuctStrength -= this.seaState * 0.05f;
				}
			}
			this.usingBottomBounce = false;
			if (UnityEngine.Random.value > 0.3f)
			{
				this.convergenceZoneStrength = 0f;
				this.convergenceZoneDistanceMetres = UnityEngine.Random.Range(this.convergenceRange.x, this.convergenceRange.y);
				float num = this.oceanFloorDepthInFeet;
				if (0.616f * this.convergenceZoneDistanceMetres - 19600f < num)
				{
					this.convergenceZoneStrength = UnityEngine.Random.value;
				}
			}
			if (this.convergenceZoneStrength == 0f)
			{
				this.bottomBounceCurrentDepth = UnityEngine.Random.Range(this.bottomBounceDepthRange.x, this.bottomBounceDepthRange.y);
				if (this.oceanFloorDepthInFeet < this.bottomBounceCurrentDepth)
				{
					this.convergenceZoneDistanceMetres = UnityEngine.Random.Range(this.bottomBounceRange.x, this.bottomBounceRange.y);
					this.convergenceZoneStrength = UnityEngine.Random.value;
					this.usingBottomBounce = true;
				}
			}
		}
		if ((GameDataManager.trainingMode || GameDataManager.missionMode) && UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.usePresetEnvironment)
		{
			if (UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.layerStrength != -1f)
			{
				this.layerStrength = UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.layerStrength;
				this.layerDepthInFeet = this.GetlayerDepthInFeet();
				if (GameDataManager.trainingMode)
				{
					this.layerDepthInFeet = 150f;
				}
			}
			if (UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.ductStrength != -1f)
			{
				this.surfaceDuctStrength = UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.ductStrength;
			}
		}
		this.layerStrength = Mathf.Clamp01(this.layerStrength);
		this.surfaceDuctStrength = Mathf.Clamp01(this.surfaceDuctStrength);
		this.convergenceZoneStrength = Mathf.Clamp01(this.convergenceZoneStrength);
		this.currentOceanAmbientNoise = this.oceanAmbientBaseNoise + this.noisePerSeaState * this.seaState / (this.passiveCompressionFactor / 2f);
		if (UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.usePresetPositions && UIFunctions.globaluifunctions.gamedatamanager.worldYardsScale == 1f)
		{
			this.currentOceanAmbientNoise += 10f;
		}
		if (LevelLoadManager.isRaining)
		{
			this.currentOceanAmbientNoise += this.maxNoiseFromRain * UnityEngine.Random.Range(0.5f, 1f);
		}
		else if (LevelLoadManager.isSnowing)
		{
			this.currentOceanAmbientNoise += this.maxNoiseFromRain * UnityEngine.Random.Range(0.3f, 0.8f);
		}
		if (this.layerStrength == 0f)
		{
			this.playerfunctions.contactDepth.gameObject.SetActive(false);
		}
		this.layerDepth = 1000f - this.layerDepthInFeet * GameDataManager.feetToUnits;
		this.layerAngle = (1000f - this.layerDepth) * GameDataManager.unitsToFeet;
		this.layerAngle = 0.0073f * this.layerAngle + 0.75f;
		this.torpedoObjects = new Torpedo[0];
		this.noisemakerObjects = new Noisemaker[0];
	}

	// Token: 0x060009D9 RID: 2521 RVA: 0x00074038 File Offset: 0x00072238
	public void InitialiseSonarDisplay()
	{
		UIFunctions.globaluifunctions.playerfunctions.SetContactToNone();
		int num = GameDataManager.enemyvesselsonlevel.Length;
		this.initialRun = true;
		this.timer = 0f;
		this.bearingToContacts = new float[num];
		this.signalFromContacts = new float[num];
		this.sensorOfContacts = new float[num];
		this.signalLastPassiveReading = new float[num];
		this.sensorLastPassiveReading = new float[num];
		this.signalFromContactsType = new string[num];
		this.sensorOfContactsType = new string[num];
		this.headingOfContacts = new float[num];
		this.speedOfContacts = new float[num];
		this.rangeToContacts = new float[num];
		this.solutionQualityOfContacts = new float[num];
		this.timeTrackingContact = new float[num];
		this.enemyPositions = new Vector3[num];
		this.solutionRangeErrors = new float[num];
		this.shipTypes = new int[10];
		this.initialDetectedByPlayer = new bool[GameDataManager.enemyvesselsonlevel.Length];
		this.initialDetectedByPlayerName = new string[GameDataManager.enemyvesselsonlevel.Length];
		this.detectedByPlayer = new bool[GameDataManager.enemyvesselsonlevel.Length];
		this.identifiedByPlayer = new bool[GameDataManager.enemyvesselsonlevel.Length];
		this.classifiedByPlayer = new bool[GameDataManager.enemyvesselsonlevel.Length];
		this.classifiedByPlayerAsClass = new int[GameDataManager.enemyvesselsonlevel.Length];
		this.shipTypes = new int[GameDataManager.enemyvesselsonlevel.Length];
		this.timeTrackingContact = new float[GameDataManager.enemyvesselsonlevel.Length];
		this.numberOfSonarContacts = 0;
		this.numberOfRADARContacts = 0;
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			if (UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.kmShipClasses[i]].shipType == "SUBMARINE")
			{
				this.shipTypes[i] = 2;
			}
			else if (UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.kmShipClasses[i]].shipType == "MERCHANT")
			{
				this.shipTypes[i] = 3;
			}
			else if (UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.kmShipClasses[i]].shipType == "BIOLOGIC")
			{
				this.shipTypes[i] = 0;
			}
			else
			{
				this.shipTypes[i] = 1;
			}
		}
		this.conditionsdisplay.InitialiseConditions();
		this.tacticalmap.playerMapContact.contactText.text = GameDataManager.playervesselsonlevel[0].databaseshipdata.shipclass;
		this.torpedoDetectionRange = 10000f;
		if (this.seaState > 0f)
		{
			this.torpedoDetectionRange -= this.seaState * 1000f;
		}
	}

	// Token: 0x060009DA RID: 2522 RVA: 0x00074320 File Offset: 0x00072520
	private void BackToPlayerShip()
	{
		this.ReturnToPlayerSub();
	}

	// Token: 0x060009DB RID: 2523 RVA: 0x00074328 File Offset: 0x00072528
	private void ReturnToPlayerSub()
	{
		if (!ManualCameraZoom.binoculars)
		{
			this.cameraCurrentTargetIndex = -1;
			ManualCameraZoom.target = GameDataManager.playervesselsonlevel[0].gameObject.transform;
			ManualCameraZoom.minDistance = GameDataManager.playervesselsonlevel[0].databaseshipdata.minCameraDistance;
		}
	}

	// Token: 0x060009DC RID: 2524 RVA: 0x00074368 File Offset: 0x00072568
	public void SelectTarget()
	{
		int num = this.playerfunctions.currentTargetIndex;
		num++;
		if (num == -1)
		{
			num = 0;
		}
		for (int i = num; i < this.detectedByPlayer.Length; i++)
		{
			if (this.detectedByPlayer[i] && !GameDataManager.enemyvesselsonlevel[i].isSinking)
			{
				this.playerfunctions.currentTargetIndex = i;
				UIFunctions.globaluifunctions.playerfunctions.SetContactProfileSprite(GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipPrefabName + "_sig");
				this.playerfunctions.SetTargetData();
				this.SetSignatureData();
				UIFunctions.globaluifunctions.playerfunctions.SetProfileToClassifiedContact();
				return;
			}
		}
		for (int j = 0; j < this.detectedByPlayer.Length; j++)
		{
			if (this.detectedByPlayer[j] && !GameDataManager.enemyvesselsonlevel[j].isSinking)
			{
				this.playerfunctions.currentTargetIndex = j;
				UIFunctions.globaluifunctions.playerfunctions.SetContactProfileSprite(GameDataManager.enemyvesselsonlevel[j].databaseshipdata.shipPrefabName + "_sig");
				this.playerfunctions.SetTargetData();
				this.SetSignatureData();
				UIFunctions.globaluifunctions.playerfunctions.SetProfileToClassifiedContact();
				return;
			}
		}
		UIFunctions.globaluifunctions.playerfunctions.SetContactToNone();
		this.playerfunctions.currentTargetIndex = -1;
		UIFunctions.globaluifunctions.playerfunctions.SetContactProfileSprite("blank_sig");
		this.playerfunctions.ClearAllDataTexts();
		this.playerfunctions.targetShipData[0].gameObject.SetActive(true);
		if (this.playerfunctions.contextualPanels[1].activeSelf)
		{
			this.SetSignatureData();
		}
	}

	// Token: 0x060009DD RID: 2525 RVA: 0x0007451C File Offset: 0x0007271C
	private void SelectShipButton()
	{
		if (this.playerfunctions.currentTargetIndex > -1 && !ManualCameraZoom.binoculars)
		{
			this.FocusCameraOnVessel(this.playerfunctions.currentTargetIndex);
		}
	}

	// Token: 0x060009DE RID: 2526 RVA: 0x00074558 File Offset: 0x00072758
	private void FocusCameraOnVessel(int shipIndex)
	{
		if (!ManualCameraZoom.binoculars)
		{
			this.cameraCurrentTargetIndex = shipIndex;
			ManualCameraZoom.target = GameDataManager.enemyvesselsonlevel[shipIndex].gameObject.transform;
			ManualCameraZoom.minDistance = GameDataManager.enemyvesselsonlevel[shipIndex].vesselmovement.cameraDistance;
			if (ManualCameraZoom.distance < GameDataManager.enemyvesselsonlevel[shipIndex].vesselmovement.cameraDistance)
			{
				ManualCameraZoom.distance = GameDataManager.enemyvesselsonlevel[shipIndex].vesselmovement.cameraDistance;
			}
		}
	}

	// Token: 0x060009DF RID: 2527 RVA: 0x000745D4 File Offset: 0x000727D4
	private int GetNumberOfPlayerDetectionTypes(int index)
	{
		int num = 0;
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel[index].acoustics.playerHasDetectedWith.Length; i++)
		{
			if (GameDataManager.enemyvesselsonlevel[index].acoustics.playerHasDetectedWith[i])
			{
				if (i == 1 && UIFunctions.globaluifunctions.playerfunctions.playerVessel.submarineFunctions.mastHeads[0].transform.position.y < 1000f)
				{
					GameDataManager.enemyvesselsonlevel[index].acoustics.playerHasDetectedWith[i] = false;
				}
				else if (i == 2 && UIFunctions.globaluifunctions.playerfunctions.playerVessel.submarineFunctions.mastHeads[2].transform.position.y < 1000f)
				{
					GameDataManager.enemyvesselsonlevel[index].acoustics.playerHasDetectedWith[i] = false;
				}
				else if (i == 3 && UIFunctions.globaluifunctions.playerfunctions.playerVessel.submarineFunctions.mastHeads[1].transform.position.y < 1000f)
				{
					GameDataManager.enemyvesselsonlevel[index].acoustics.playerHasDetectedWith[i] = false;
				}
				if (GameDataManager.enemyvesselsonlevel[index].acoustics.playerHasDetectedWith[i])
				{
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x060009E0 RID: 2528 RVA: 0x00074740 File Offset: 0x00072940
	public void CheckIfUpgradeContactToMaster(int index, bool forceIt = false)
	{
		if (GameDataManager.enemyvesselsonlevel[index].acoustics.isMaster || GameDataManager.enemyvesselsonlevel[index].isSinking)
		{
			return;
		}
		if (this.GetNumberOfPlayerDetectionTypes(index) > 1 || forceIt)
		{
			GameDataManager.enemyvesselsonlevel[index].acoustics.isMaster = true;
			string basicContactName = this.playerfunctions.GetBasicContactName(this.initialDetectedByPlayerName[index]);
			this.initialDetectedByPlayerName[index] = this.initialDetectedByPlayerName[index].Replace("S", "M");
			this.initialDetectedByPlayerName[index] = this.initialDetectedByPlayerName[index].Replace("R", "M");
			this.initialDetectedByPlayerName[index] = this.initialDetectedByPlayerName[index].Replace("E", "M");
			this.initialDetectedByPlayerName[index] = this.initialDetectedByPlayerName[index].Replace("V", "M");
			this.tacticalmap.mapContact[index].contactText.text = this.initialDetectedByPlayerName[index];
			string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "MasterContact");
			text = text.Replace("<ORIGINAL>", basicContactName);
			text = text.Replace("<CONTACT>", this.playerfunctions.GetFullContactName(this.tacticalmap.mapContact[index].contactText.text, index));
			this.playerfunctions.PlayerMessage(text, this.playerfunctions.messageLogColors["XO"], "MasterContact", false);
			this.SetContactName(index);
		}
	}

	// Token: 0x060009E1 RID: 2529 RVA: 0x000748C4 File Offset: 0x00072AC4
	private int GetNumberOfCurrentContacts()
	{
		int num = 0;
		for (int i = 0; i < this.detectedByPlayer.Length; i++)
		{
			if (this.detectedByPlayer[i])
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060009E2 RID: 2530 RVA: 0x00074900 File Offset: 0x00072B00
	public void ContactDetected(int i, string detectionType)
	{
		this.tacticalmap.mapContact[i].shipDisplayIcon.sprite = this.sonarPaintImages[0];
		this.tacticalmap.mapContact[i].gameObject.SetActive(true);
		if (this.initialDetectedByPlayerName[i] == null)
		{
			this.initialDetectedByPlayer[i] = true;
			this.numberOfSonarContacts++;
			if (detectionType == "SONAR")
			{
				this.tacticalmap.mapContact[i].contactText.text = "S" + this.numberOfSonarContacts.ToString();
				UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentVesselIndex = i;
				string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "NewSonarContact");
				text = text.Replace("<BRG>", string.Format("{0:0}", this.bearingToContacts[i]));
				text = text.Replace("<CONTACT>", this.playerfunctions.GetFullContactName(this.tacticalmap.mapContact[i].contactText.text, i));
				this.initialDetectedByPlayerName[i] = this.tacticalmap.mapContact[i].contactText.text;
				this.playerfunctions.PlayerMessage(text, this.playerfunctions.messageLogColors["Sonar"], "NewSonarContact", false);
			}
			else if (detectionType == "RADAR")
			{
				this.tacticalmap.mapContact[i].contactText.text = "R" + this.numberOfSonarContacts.ToString();
				UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentVesselIndex = i;
				string text2 = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "NewRADARContact");
				text2 = text2.Replace("<BRG>", string.Format("{0:0}", this.bearingToContacts[i]));
				text2 = text2.Replace("<CONTACT>", this.playerfunctions.GetFullContactName(this.tacticalmap.mapContact[i].contactText.text, i));
				this.initialDetectedByPlayerName[i] = this.tacticalmap.mapContact[i].contactText.text;
				this.playerfunctions.PlayerMessage(text2, this.playerfunctions.messageLogColors["XO"], "NewRADARContact", false);
			}
			else if (detectionType == "VISUAL")
			{
				this.tacticalmap.mapContact[i].contactText.text = "V" + this.numberOfSonarContacts.ToString();
				UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentVesselIndex = i;
				string text3 = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "NewVisualContact");
				text3 = text3.Replace("<BRG>", string.Format("{0:0}", this.bearingToContacts[i]));
				text3 = text3.Replace("<CONTACT>", this.playerfunctions.GetFullContactName(this.tacticalmap.mapContact[i].contactText.text, i));
				this.initialDetectedByPlayerName[i] = this.tacticalmap.mapContact[i].contactText.text;
				this.playerfunctions.PlayerMessage(text3, this.playerfunctions.messageLogColors["XO"], "NewVisualContact", false);
			}
			else if (detectionType == "ESM")
			{
				this.tacticalmap.mapContact[i].contactText.text = "E" + this.numberOfSonarContacts.ToString();
				UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentVesselIndex = i;
				string text4 = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "NewESMContact");
				text4 = text4.Replace("<BRG>", string.Format("{0:0}", this.bearingToContacts[i]));
				text4 = text4.Replace("<CONTACT>", this.playerfunctions.GetFullContactName(this.tacticalmap.mapContact[i].contactText.text, i));
				this.initialDetectedByPlayerName[i] = this.tacticalmap.mapContact[i].contactText.text;
				this.playerfunctions.PlayerMessage(text4, this.playerfunctions.messageLogColors["XO"], "NewESMContact", false);
			}
		}
		else
		{
			if (detectionType == "SONAR")
			{
				UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentVesselIndex = i;
				string text5 = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "ReEstablishSonarContact");
				text5 = text5.Replace("<BRG>", string.Format("{0:0}", this.bearingToContacts[i]));
				text5 = text5.Replace("<CONTACT>", this.playerfunctions.GetFullContactName(this.tacticalmap.mapContact[i].contactText.text, i));
				this.playerfunctions.PlayerMessage(text5, this.playerfunctions.messageLogColors["Sonar"], "ReEstablishSonarContact", false);
				if (!this.initialDetectedByPlayerName[i].Contains("S"))
				{
					this.CheckIfUpgradeContactToMaster(i, true);
				}
			}
			else if (detectionType == "RADAR")
			{
				UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentVesselIndex = i;
				string text6 = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "ReEstablishRADARContact");
				text6 = text6.Replace("<BRG>", string.Format("{0:0}", this.bearingToContacts[i]));
				text6 = text6.Replace("<CONTACT>", this.playerfunctions.GetFullContactName(this.tacticalmap.mapContact[i].contactText.text, i));
				this.playerfunctions.PlayerMessage(text6, this.playerfunctions.messageLogColors["XO"], "ReEstablishRADARContact", false);
				if (!this.initialDetectedByPlayerName[i].Contains("R"))
				{
					this.CheckIfUpgradeContactToMaster(i, true);
				}
			}
			else if (detectionType == "VISUAL")
			{
				UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentVesselIndex = i;
				string text7 = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "ReEstablishVisualContact");
				text7 = text7.Replace("<BRG>", string.Format("{0:0}", this.bearingToContacts[i]));
				text7 = text7.Replace("<CONTACT>", this.playerfunctions.GetFullContactName(this.tacticalmap.mapContact[i].contactText.text, i));
				this.playerfunctions.PlayerMessage(text7, this.playerfunctions.messageLogColors["XO"], "ReEstablishVisualContact", false);
				if (!this.initialDetectedByPlayerName[i].Contains("V"))
				{
					this.CheckIfUpgradeContactToMaster(i, true);
				}
			}
			else if (detectionType == "ESM")
			{
				UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentVesselIndex = i;
				string text8 = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "ReEstablishESMContact");
				text8 = text8.Replace("<BRG>", string.Format("{0:0}", this.bearingToContacts[i]));
				text8 = text8.Replace("<CONTACT>", this.playerfunctions.GetFullContactName(this.tacticalmap.mapContact[i].contactText.text, i));
				this.playerfunctions.PlayerMessage(text8, this.playerfunctions.messageLogColors["XO"], "ReEstablishESMContact", false);
				if (!this.initialDetectedByPlayerName[i].Contains("E"))
				{
					this.CheckIfUpgradeContactToMaster(i, true);
				}
			}
			this.tacticalmap.mapContact[i].shipDisplayIcon.color = new Color(this.tacticalmap.mapContact[i].shipDisplayIcon.color.r, this.tacticalmap.mapContact[i].shipDisplayIcon.color.g, this.tacticalmap.mapContact[i].shipDisplayIcon.color.b, 1f);
			this.tacticalmap.mapContact[i].contactText.color = new Color(this.tacticalmap.mapContact[i].shipDisplayIcon.color.r, this.tacticalmap.mapContact[i].shipDisplayIcon.color.g, this.tacticalmap.mapContact[i].shipDisplayIcon.color.b, 1f);
		}
		this.detectedByPlayer[i] = true;
		if (this.playerfunctions.currentTargetIndex == -1)
		{
			this.playerfunctions.currentTargetIndex = i;
			UIFunctions.globaluifunctions.playerfunctions.SetContactProfileSprite(GameDataManager.enemyvesselsonlevel[this.playerfunctions.currentTargetIndex].databaseshipdata.shipPrefabName + "_sig");
		}
		if (GameDataManager.enemyvesselsonlevel[i].vesselmovement.isCavitating)
		{
			string text9 = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "EnemyCavitating");
			text9 = text9.Replace("<CONTACT>", this.playerfunctions.GetFullContactName(this.tacticalmap.mapContact[i].contactText.text, i));
			UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text9, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["Sonar"], "EnemyCavitating", false);
		}
		this.CalculateRangeError(i);
	}

	// Token: 0x060009E3 RID: 2531 RVA: 0x000752A8 File Offset: 0x000734A8
	public void ContactIdentified(int i)
	{
		this.tacticalmap.mapContact[i].shipDisplayIcon.sprite = this.sonarPaintImages[this.shipTypes[i]];
		this.tacticalmap.mapContact[i].shipDisplayIcon.transform.rotation = Quaternion.identity;
		string text = "Submarine";
		if (this.shipTypes[i] == 1)
		{
			text = "Warship";
		}
		else if (this.shipTypes[i] == 3)
		{
			text = "Merchant";
		}
		string text2 = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "ContactIdentified");
		text2 = text2.Replace("<CONTACT>", this.playerfunctions.GetFullContactName(this.tacticalmap.mapContact[i].contactText.text, i));
		text2 = text2.Replace("<CONTACTTYPE>", GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipType.ToLower());
		this.playerfunctions.PlayerMessage(text2, this.playerfunctions.messageLogColors["Sonar"], "ContactIdentified", false);
		this.tacticalmap.mapContact[i].contactText.text = text;
		this.identifiedByPlayer[i] = true;
	}

	// Token: 0x060009E4 RID: 2532 RVA: 0x000753D8 File Offset: 0x000735D8
	public void ContactClassifiedManuallyByPlayer()
	{
		if (this.playerfunctions.currentTargetIndex == -1)
		{
			return;
		}
		this.classifiedByPlayerAsClass[this.playerfunctions.currentTargetIndex] = this.playerfunctions.otherVesselList[this.playerfunctions.currentSignatureIndex];
		this.classifiedByPlayer[this.playerfunctions.currentTargetIndex] = true;
		this.ClassifyTargetIconSetup(this.playerfunctions.currentTargetIndex);
		this.SetSignatureData();
		UIFunctions.globaluifunctions.playerfunctions.SetProfileToClassifiedContact();
	}

	// Token: 0x060009E5 RID: 2533 RVA: 0x0007545C File Offset: 0x0007365C
	public void ContactClassified(int i)
	{
		if (this.classifiedByPlayerAsClass[i] == GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipID)
		{
			return;
		}
		if (GameDataManager.optionsFloatSettings[3] == 3f)
		{
			return;
		}
		this.classifiedByPlayerAsClass[i] = GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipID;
		this.ClassifyTargetIconSetup(i);
		if (UIFunctions.globaluifunctions.playerfunctions.currentTargetIndex == i)
		{
			this.SetSignatureData();
			UIFunctions.globaluifunctions.playerfunctions.SetProfileToClassifiedContact();
		}
	}

	// Token: 0x060009E6 RID: 2534 RVA: 0x000754E4 File Offset: 0x000736E4
	private void ClassifyTargetIconSetup(int i)
	{
		this.tacticalmap.mapContact[i].shipDisplayIcon.sprite = this.sonarPaintImages[this.shipTypes[i]];
		this.tacticalmap.mapContact[i].shipDisplayIcon.transform.rotation = Quaternion.identity;
		this.classifiedByPlayer[i] = true;
		this.SetContactName(i);
		UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentVesselIndex = i;
		string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "ContactClassified");
		text = text.Replace("<CONTACT>", this.playerfunctions.GetBasicContactName(this.initialDetectedByPlayerName[i]));
		text = text.Replace("<CONTACTCLASSIFIED>", UIFunctions.globaluifunctions.database.databaseshipdata[this.classifiedByPlayerAsClass[i]].shipclass);
		this.playerfunctions.PlayerMessage(text, this.playerfunctions.messageLogColors["Sonar"], "ContactClassified", false);
		Color contactColor = this.GetContactColor(i);
		this.tacticalmap.mapContact[i].shipDisplayIcon.color = contactColor;
		this.tacticalmap.mapContact[i].contactText.color = contactColor;
	}

	// Token: 0x060009E7 RID: 2535 RVA: 0x00075614 File Offset: 0x00073814
	public void SetContactName(int i)
	{
		string text = this.initialDetectedByPlayerName[i];
		if (this.classifiedByPlayer[i])
		{
			text = text + " " + UIFunctions.globaluifunctions.database.databaseshipdata[this.classifiedByPlayerAsClass[i]].shipclass;
		}
		this.tacticalmap.mapContact[i].contactText.text = text;
		if (this.playerfunctions.currentTargetIndex == i)
		{
			this.playerfunctions.contactDataName.text = text.ToUpper();
		}
	}

	// Token: 0x060009E8 RID: 2536 RVA: 0x000756A0 File Offset: 0x000738A0
	public Color GetContactColor(int i)
	{
		int num = 1;
		if (UIFunctions.globaluifunctions.database.databaseshipdata[this.classifiedByPlayerAsClass[i]].shipType == "BIOLOGIC" || Database.GetIsCivilian(UIFunctions.globaluifunctions.database.databaseshipdata[this.classifiedByPlayerAsClass[i]].shipPrefabName))
		{
			num = 2;
		}
		return this.tacticalmap.navyColors[num];
	}

	// Token: 0x060009E9 RID: 2537 RVA: 0x0007571C File Offset: 0x0007391C
	public void FixedUpdate()
	{
		if (!GameDataManager.HUDActive)
		{
			return;
		}
		if (!GameDataManager.playervesselsonlevel[0].isSinking)
		{
			this.timer += Time.deltaTime;
			this.surfaceRadarCheckTimer += Time.deltaTime;
			this.airRadarCheckTimer += Time.deltaTime;
		}
		if (this.airRadarCheckTimer > 2f)
		{
			float num = 0f;
			for (int i = 0; i < this.torpedoObjects.Length; i++)
			{
				if (this.torpedoObjects[i].whichNavy == 0 && this.torpedoObjects[i].isAirborne && this.torpedoObjects[i].databaseweapondata.weaponType == "MISSILE")
				{
					num += 0.2f;
					if (this.torpedoObjects[i].sensorsActive)
					{
						num += 2f;
					}
				}
				if (num > 0f)
				{
					for (int j = 0; j < GameDataManager.enemyvesselsonlevel.Length; j++)
					{
						if (GameDataManager.enemyvesselsonlevel[j].databaseshipdata.radarID != -1 && !GameDataManager.enemyvesselsonlevel[j].isSinking && this.CheckIfSensorClearOfTerrain(GameDataManager.enemyvesselsonlevel[j].transform, this.torpedoObjects[i].transform))
						{
							float distanceInYards = Vector3.Distance(this.torpedoObjects[i].transform.position, GameDataManager.enemyvesselsonlevel[j].transform.position) * GameDataManager.yardsScale;
							if (GameDataManager.enemyvesselsonlevel[j].databaseshipdata.shipType != "SUBMARINE")
							{
								this.CheckEnemyUnitRADAR(j, GameDataManager.enemyvesselsonlevel[j].databaseshipdata.radarID, distanceInYards, num, GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata, "VESSEL", this.torpedoObjects[i].transform.position);
							}
							else if (GameDataManager.enemyvesselsonlevel[j].transform.position.y > 999.69f)
							{
								this.CheckEnemyUnitRADAR(j, GameDataManager.enemyvesselsonlevel[j].databaseshipdata.radarID, distanceInYards, num, GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata, "VESSEL", this.torpedoObjects[i].transform.position);
							}
						}
					}
					for (int k = 0; k < UIFunctions.globaluifunctions.combatai.enemyHelicopters.Length; k++)
					{
						float distanceInYards2 = Vector3.Distance(this.torpedoObjects[i].transform.position, UIFunctions.globaluifunctions.combatai.enemyHelicopters[k].transform.position) * GameDataManager.yardsScale;
						if (UIFunctions.globaluifunctions.combatai.enemyHelicopters[k].databaseaircraftdata.radarID != -1 && this.CheckIfSensorClearOfTerrain(UIFunctions.globaluifunctions.combatai.enemyHelicopters[k].transform, this.torpedoObjects[i].transform))
						{
							this.CheckEnemyUnitRADAR(k, UIFunctions.globaluifunctions.combatai.enemyHelicopters[k].databaseaircraftdata.radarID, distanceInYards2, num, UIFunctions.globaluifunctions.combatai.enemyHelicopters[k].sensordata, "HELICOPTER", this.torpedoObjects[i].transform.position);
						}
					}
					for (int l = 0; l < UIFunctions.globaluifunctions.combatai.enemyAircraft.Length; l++)
					{
						float distanceInYards3 = Vector3.Distance(this.torpedoObjects[i].transform.position, UIFunctions.globaluifunctions.combatai.enemyAircraft[l].transform.position) * GameDataManager.yardsScale;
						if (UIFunctions.globaluifunctions.combatai.enemyAircraft[l].databaseaircraftdata.radarID != -1 && this.CheckIfSensorClearOfTerrain(UIFunctions.globaluifunctions.combatai.enemyAircraft[l].transform, this.torpedoObjects[i].transform))
						{
							this.CheckEnemyUnitRADAR(l, UIFunctions.globaluifunctions.combatai.enemyAircraft[l].databaseaircraftdata.radarID, distanceInYards3, num, UIFunctions.globaluifunctions.combatai.enemyAircraft[l].sensordata, "AIRCRAFT", this.torpedoObjects[i].transform.position);
						}
					}
				}
			}
			this.airRadarCheckTimer -= UnityEngine.Random.Range(2f, 4f);
		}
		if (this.surfaceRadarCheckTimer > 10f)
		{
			float num2 = 0f;
			if (GameDataManager.playervesselsonlevel[0].transform.position.y > GameDataManager.playervesselsonlevel[0].databaseshipdata.submergedat)
			{
				num2 = 0.6f;
			}
			for (int m = 0; m < GameDataManager.playervesselsonlevel[0].submarineFunctions.scopeStatus.Length; m++)
			{
				if (GameDataManager.playervesselsonlevel[0].submarineFunctions.GetMastIsUp(m))
				{
					num2 += 0.04f;
					if (m == 2)
					{
						num2 += 1.5f;
						this.CheckPlayerRADAR();
					}
					else if (m < 2)
					{
						this.CycleThroughESM();
					}
				}
			}
			if (this.playerfunctions.esmGameObject.activeSelf && !GameDataManager.playervesselsonlevel[0].submarineFunctions.GetMastIsUp(0) && !GameDataManager.playervesselsonlevel[0].submarineFunctions.GetMastIsUp(1))
			{
				this.playerfunctions.DisableESMMeter();
			}
			if (num2 > 1.5f)
			{
				num2 = 1.5f;
			}
			if (num2 > 0f)
			{
				for (int n = 0; n < GameDataManager.enemyvesselsonlevel.Length; n++)
				{
					if (GameDataManager.enemyvesselsonlevel[n].databaseshipdata.radarID != -1 && !GameDataManager.enemyvesselsonlevel[n].isSinking && this.CheckIfSensorClearOfTerrain(GameDataManager.enemyvesselsonlevel[n].transform, GameDataManager.playervesselsonlevel[0].transform))
					{
						if (GameDataManager.enemyvesselsonlevel[n].databaseshipdata.shipType != "SUBMARINE")
						{
							float distanceInYards4 = Vector3.Distance(GameDataManager.playervesselsonlevel[0].transform.position, GameDataManager.enemyvesselsonlevel[n].transform.position) * GameDataManager.yardsScale;
							this.CheckEnemyUnitRADAR(n, GameDataManager.enemyvesselsonlevel[n].databaseshipdata.radarID, distanceInYards4, num2, GameDataManager.enemyvesselsonlevel[n].vesselai.sensordata, "VESSEL", GameDataManager.playervesselsonlevel[0].transform.position);
						}
						else if (GameDataManager.enemyvesselsonlevel[n].transform.position.y > 999.69f)
						{
							float distanceInYards5 = Vector3.Distance(GameDataManager.playervesselsonlevel[0].transform.position, GameDataManager.enemyvesselsonlevel[n].transform.position) * GameDataManager.yardsScale;
							this.CheckEnemyUnitRADAR(n, GameDataManager.enemyvesselsonlevel[n].databaseshipdata.radarID, distanceInYards5, num2, GameDataManager.enemyvesselsonlevel[n].vesselai.sensordata, "VESSEL", GameDataManager.playervesselsonlevel[0].transform.position);
						}
					}
				}
			}
			for (int num3 = 0; num3 < UIFunctions.globaluifunctions.combatai.enemyHelicopters.Length; num3++)
			{
				UIFunctions.globaluifunctions.combatai.enemyHelicopters[num3].sensordata.radarTotalDetected = 0f;
				if (UIFunctions.globaluifunctions.combatai.enemyHelicopters[num3].databaseaircraftdata.radarID != -1 && this.CheckIfSensorClearOfTerrain(UIFunctions.globaluifunctions.combatai.enemyHelicopters[num3].transform, GameDataManager.playervesselsonlevel[0].transform))
				{
					float distanceInYards6 = Vector3.Distance(GameDataManager.playervesselsonlevel[0].transform.position, UIFunctions.globaluifunctions.combatai.enemyHelicopters[num3].transform.position) * GameDataManager.yardsScale;
					if (num2 > 0f && this.CheckIfSensorClearOfTerrain(UIFunctions.globaluifunctions.combatai.enemyHelicopters[num3].transform, GameDataManager.playervesselsonlevel[0].transform))
					{
						this.CheckEnemyUnitRADAR(num3, UIFunctions.globaluifunctions.combatai.enemyHelicopters[num3].databaseaircraftdata.radarID, distanceInYards6, num2, UIFunctions.globaluifunctions.combatai.enemyHelicopters[num3].sensordata, "HELICOPTER", GameDataManager.playervesselsonlevel[0].transform.position);
					}
					this.CheckEnemyUnitMAD(num3, distanceInYards6, UIFunctions.globaluifunctions.combatai.enemyHelicopters[num3].sensordata, "HELICOPTER", GameDataManager.playervesselsonlevel[0].transform.position);
				}
			}
			for (int num4 = 0; num4 < UIFunctions.globaluifunctions.combatai.enemyAircraft.Length; num4++)
			{
				UIFunctions.globaluifunctions.combatai.enemyAircraft[num4].sensordata.radarTotalDetected = 0f;
				if (UIFunctions.globaluifunctions.combatai.enemyAircraft[num4].databaseaircraftdata.radarID != -1 && this.CheckIfSensorClearOfTerrain(UIFunctions.globaluifunctions.combatai.enemyAircraft[num4].transform, GameDataManager.playervesselsonlevel[0].transform))
				{
					float distanceInYards7 = Vector3.Distance(GameDataManager.playervesselsonlevel[0].transform.position, UIFunctions.globaluifunctions.combatai.enemyAircraft[num4].transform.position) * GameDataManager.yardsScale;
					if (num2 > 0f)
					{
						this.CheckEnemyUnitRADAR(num4, UIFunctions.globaluifunctions.combatai.enemyAircraft[num4].databaseaircraftdata.radarID, distanceInYards7, num2, UIFunctions.globaluifunctions.combatai.enemyAircraft[num4].sensordata, "AIRCRAFT", GameDataManager.playervesselsonlevel[0].transform.position);
					}
					this.CheckEnemyUnitMAD(num4, distanceInYards7, UIFunctions.globaluifunctions.combatai.enemyAircraft[num4].sensordata, "AIRCRAFT", GameDataManager.playervesselsonlevel[0].transform.position);
				}
			}
			this.surfaceRadarCheckTimer -= UnityEngine.Random.Range(8f, 12f);
			UIFunctions.globaluifunctions.playerfunctions.CheckMastExposure();
		}
		if (this.timer > 8f)
		{
			for (int num5 = 0; num5 < this.detectedByPlayer.Length; num5++)
			{
				if (this.detectedByPlayer[num5])
				{
					this.timeTrackingContact[num5] += this.timer;
				}
				else
				{
					if (this.timeTrackingContact[num5] > 0f)
					{
						this.timeTrackingContact[num5] -= this.timer * 0.1f;
					}
					if (this.solutionQualityOfContacts[num5] > 0f)
					{
						this.solutionQualityOfContacts[num5] -= UnityEngine.Random.Range(0f, 0.25f);
					}
				}
			}
			if (!UIFunctions.globaluifunctions.playerfunctions.playerVessel.isSinking)
			{
				this.SonarCheck();
			}
			this.CheckNavHazard();
			if (this.playerfunctions.currentTargetIndex >= 0)
			{
				this.playerfunctions.SetTargetData();
			}
			else
			{
				UIFunctions.globaluifunctions.playerfunctions.SetContactToNone();
			}
			this.timer -= 8f;
		}
		for (int num6 = 0; num6 < this.detectedByPlayer.Length; num6++)
		{
			if (this.detectedByPlayer[num6])
			{
				this.timeTrackingContact[num6] += Time.deltaTime;
			}
			else if (this.timeTrackingContact[num6] > 0f)
			{
				this.timeTrackingContact[num6] -= Time.deltaTime * 0.3f;
			}
		}
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x00076338 File Offset: 0x00074538
	private void CheckNavHazard()
	{
		if (UIFunctions.globaluifunctions.levelloadmanager.icePresent || UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.proximityMineLocations.Length > 0)
		{
			bool flag = false;
			bool flag2 = false;
			Vector3 hazardPosition = Vector3.zero;
			Vector3 hazardPosition2 = Vector3.zero;
			RaycastHit raycastHit;
			if (Physics.Raycast(GameDataManager.playervesselsonlevel[0].transform.position + GameDataManager.playervesselsonlevel[0].transform.right, GameDataManager.playervesselsonlevel[0].transform.forward, out raycastHit, this.actualHighFrequencyNavSonarRange, this.navSonarMask))
			{
				if (raycastHit.collider.gameObject.name.Contains("Ice"))
				{
					flag = true;
					hazardPosition = raycastHit.point;
				}
				else if (raycastHit.collider.gameObject.name.Contains("minefield"))
				{
					flag2 = true;
					hazardPosition2 = raycastHit.point;
				}
			}
			if (Physics.Raycast(GameDataManager.playervesselsonlevel[0].transform.position - GameDataManager.playervesselsonlevel[0].transform.right, GameDataManager.playervesselsonlevel[0].transform.forward, out raycastHit, this.actualHighFrequencyNavSonarRange, this.navSonarMask))
			{
				if (raycastHit.collider.gameObject.name.Contains("Ice"))
				{
					flag = true;
					hazardPosition = raycastHit.point;
				}
				else if (raycastHit.collider.gameObject.name.Contains("minefield"))
				{
					flag2 = true;
					hazardPosition2 = raycastHit.point;
				}
			}
			if (UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.proximityMineLocations.Length > 0)
			{
				for (int i = 0; i < UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.mineFields.Length; i++)
				{
					if (UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.mineFields[i] != null && UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.mineFields[i].bounds.Contains(GameDataManager.playervesselsonlevel[0].transform.position))
					{
						flag2 = true;
					}
				}
			}
			if (UIFunctions.globaluifunctions.playerfunctions.statusIcons[2].gameObject.activeSelf != flag)
			{
				UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("icewarning", flag);
			}
			if (UIFunctions.globaluifunctions.playerfunctions.statusIcons[3].gameObject.activeSelf != flag2)
			{
				UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("minefield", flag2);
			}
			if (flag)
			{
				this.tacticalmap.PlaceHazardIconOnMap(hazardPosition, 0);
				if (!UIFunctions.globaluifunctions.playerfunctions.iceWarned)
				{
					UIFunctions.globaluifunctions.playerfunctions.iceWarned = true;
					if (UIFunctions.globaluifunctions.playerfunctions.timeCompressionToggle == 1)
					{
						UIFunctions.globaluifunctions.playerfunctions.SetTimeCompression();
					}
					UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "IceHazard"), UIFunctions.globaluifunctions.playerfunctions.messageLogColors["Helm"], "IceHazard", false);
				}
			}
			else
			{
				UIFunctions.globaluifunctions.playerfunctions.iceWarned = false;
			}
			if (flag2)
			{
				this.tacticalmap.PlaceHazardIconOnMap(hazardPosition2, 1);
				if (!UIFunctions.globaluifunctions.playerfunctions.mineWarned)
				{
					UIFunctions.globaluifunctions.playerfunctions.mineWarned = true;
					if (UIFunctions.globaluifunctions.playerfunctions.timeCompressionToggle == 1)
					{
						UIFunctions.globaluifunctions.playerfunctions.SetTimeCompression();
					}
					UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "MineHazard"), UIFunctions.globaluifunctions.playerfunctions.messageLogColors["Helm"], "MineHazard", false);
				}
			}
			else
			{
				UIFunctions.globaluifunctions.playerfunctions.mineWarned = false;
			}
		}
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x00076754 File Offset: 0x00074954
	public bool CheckIfSensorClearOfTerrain(Transform listener, Transform contact)
	{
		return this.GetLandMaskingPenalty(listener, contact) == 1f;
	}

	// Token: 0x060009EC RID: 2540 RVA: 0x0007676C File Offset: 0x0007496C
	public void CheckPlayerRADAR()
	{
		if (!UIFunctions.globaluifunctions.playerfunctions.damagecontrol.CheckSubsystem("RADAR_MAST", false))
		{
			UIFunctions.globaluifunctions.playerfunctions.playerVessel.submarineFunctions.ClearDetectionTypes(2);
			return;
		}
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			if (this.CheckIfSensorClearOfTerrain(GameDataManager.enemyvesselsonlevel[i].transform, GameDataManager.playervesselsonlevel[0].transform))
			{
				float num = Vector3.Distance(GameDataManager.playervesselsonlevel[0].transform.position, GameDataManager.enemyvesselsonlevel[i].transform.position) * GameDataManager.yardsScale;
				float num2 = 1f;
				string radarSignature = GameDataManager.enemyvesselsonlevel[i].databaseshipdata.radarSignature;
				switch (radarSignature)
				{
				case "MEDIUM":
					num2 = 0.6f;
					break;
				case "SMALL":
					num2 = 0.4f;
					break;
				case "VERY SMALL":
					num2 = 0.2f;
					break;
				}
				bool flag = false;
				if (GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipType == "SUBMARINE")
				{
					if (GameDataManager.enemyvesselsonlevel[i].transform.position.y < 999.69f)
					{
						flag = true;
					}
				}
				else if (GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipType == "BIOLOGIC")
				{
					flag = true;
				}
				else if (GameDataManager.enemyvesselsonlevel[i].transform.position.y < 999.85f)
				{
					flag = true;
				}
				if (!flag)
				{
					float num4 = UIFunctions.globaluifunctions.database.databaseradardata[GameDataManager.playervesselsonlevel[0].databaseshipdata.radarID].radarRange;
					num4 *= num2;
					if (LevelLoadManager.isRaining)
					{
						num4 /= UnityEngine.Random.Range(1.5f, 2.5f);
					}
					if (num < num4)
					{
						GameDataManager.enemyvesselsonlevel[i].acoustics.playerHasDetectedWith[2] = true;
						this.solutionQualityOfContacts[i] = UIFunctions.globaluifunctions.playerfunctions.maximumPlayerTMA;
						if (!this.detectedByPlayer[i])
						{
							this.ContactDetected(i, "RADAR");
						}
						else
						{
							this.CheckIfUpgradeContactToMaster(i, false);
						}
					}
				}
			}
		}
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x00076A10 File Offset: 0x00074C10
	public void CycleThroughESM()
	{
		if (!UIFunctions.globaluifunctions.playerfunctions.damagecontrol.CheckSubsystem("ESM_MAST", false))
		{
			UIFunctions.globaluifunctions.playerfunctions.playerVessel.submarineFunctions.ClearDetectionTypes(3);
			this.playerfunctions.SetESMMeter(0f);
			return;
		}
		this.strongestRadarSignal = 0f;
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			if (GameDataManager.enemyvesselsonlevel[i].databaseshipdata.radarID != -1 && !GameDataManager.enemyvesselsonlevel[i].isSinking && !GameDataManager.enemyvesselsonlevel[i].vesselmovement.atAnchor && this.CheckIfSensorClearOfTerrain(GameDataManager.enemyvesselsonlevel[i].transform, GameDataManager.playervesselsonlevel[0].transform))
			{
				if (GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipType != "SUBMARINE")
				{
					this.CheckESMAtPosition(i, GameDataManager.enemyvesselsonlevel[i].databaseshipdata.radarID, GameDataManager.playervesselsonlevel[0].transform.position, GameDataManager.enemyvesselsonlevel[i].transform, "VESSEL");
				}
				else if (GameDataManager.enemyvesselsonlevel[i].transform.position.y > 999.69f)
				{
					this.CheckESMAtPosition(i, GameDataManager.enemyvesselsonlevel[i].databaseshipdata.radarID, GameDataManager.playervesselsonlevel[0].transform.position, GameDataManager.enemyvesselsonlevel[i].transform, "VESSEL");
				}
			}
		}
		for (int j = 0; j < UIFunctions.globaluifunctions.combatai.enemyHelicopters.Length; j++)
		{
			if (UIFunctions.globaluifunctions.combatai.enemyHelicopters[j].databaseaircraftdata.radarID != -1 && this.CheckIfSensorClearOfTerrain(UIFunctions.globaluifunctions.combatai.enemyHelicopters[j].transform, GameDataManager.playervesselsonlevel[0].transform))
			{
				this.CheckESMAtPosition(j, UIFunctions.globaluifunctions.combatai.enemyHelicopters[j].databaseaircraftdata.radarID, GameDataManager.playervesselsonlevel[0].transform.position, UIFunctions.globaluifunctions.combatai.enemyHelicopters[j].transform, "HELICOPTER");
			}
		}
		for (int k = 0; k < UIFunctions.globaluifunctions.combatai.enemyAircraft.Length; k++)
		{
			if (UIFunctions.globaluifunctions.combatai.enemyAircraft[k].databaseaircraftdata.radarID != -1 && this.CheckIfSensorClearOfTerrain(UIFunctions.globaluifunctions.combatai.enemyAircraft[k].transform, GameDataManager.playervesselsonlevel[0].transform))
			{
				this.CheckESMAtPosition(k, UIFunctions.globaluifunctions.combatai.enemyAircraft[k].databaseaircraftdata.radarID, GameDataManager.playervesselsonlevel[0].transform.position, UIFunctions.globaluifunctions.combatai.enemyAircraft[k].transform, "AIRCRAFT");
			}
		}
		this.playerfunctions.SetESMMeter((float)this.playerfunctions.esmcurrentIntensity);
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x00076D3C File Offset: 0x00074F3C
	public void CheckESMAtPosition(int i, int radarID, Vector3 listenPosition, Transform emitterPosition, string unitType)
	{
		float num = Vector3.Distance(listenPosition, emitterPosition.position) * GameDataManager.yardsScale;
		float num2 = UIFunctions.globaluifunctions.database.databaseradardata[radarID].radarRange;
		if (LevelLoadManager.isRaining)
		{
			num2 /= UnityEngine.Random.Range(1.5f, 2.5f);
		}
		float num3 = 1f - num / num2;
		if (num3 > this.strongestRadarSignal)
		{
			this.strongestRadarSignal = num3;
			GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.LookAt(emitterPosition.position);
			this.playerfunctions.esmcurrentBearing = GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.eulerAngles.y;
			this.playerfunctions.esmcurrentIntensity = (int)(this.strongestRadarSignal * 10f);
		}
		if (num < num2 && UnityEngine.Random.value < 0.2f)
		{
			UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.DrawPingLine(emitterPosition, UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.pingLines[2]);
			switch (unitType)
			{
			case "VESSEL":
				if (!this.detectedByPlayer[i])
				{
					this.ContactDetected(i, "ESM");
				}
				else
				{
					this.CheckIfUpgradeContactToMaster(i, false);
				}
				this.solutionQualityOfContacts[i] += UnityEngine.Random.Range(1f, 8f);
				GameDataManager.enemyvesselsonlevel[i].acoustics.playerHasDetectedWith[3] = true;
				break;
			}
		}
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x00076F48 File Offset: 0x00075148
	private void CheckEnemyUnitMAD(int i, float distanceInYards, SensorData sensordata, string unitType, Vector3 targetPosition)
	{
		if (this.playerfunctions.playerDepthInFeet < 500 && distanceInYards < this.madDetectionRange)
		{
			sensordata.playerDetected = true;
			sensordata.rangeYardsLastDetected = distanceInYards;
			Vector3 lastKnownTargetPosition = GameDataManager.playervesselsonlevel[0].transform.position;
			if (sensordata.timeTrackingPlayer < 60f)
			{
				lastKnownTargetPosition = targetPosition;
			}
			if (unitType == "HELICOPTER")
			{
				sensordata.lastKnownTargetPosition = lastKnownTargetPosition;
				sensordata.lastKnownTargetPosition.y = 1000f;
				UIFunctions.globaluifunctions.combatai.enemyHelicopters[i].attackPlayer = true;
			}
			else if (unitType == "AIRCRAFT" && Vector3.Distance(UIFunctions.globaluifunctions.combatai.enemyAircraft[i].transform.position, GameDataManager.playervesselsonlevel[0].transform.position) > 20f)
			{
				sensordata.lastKnownTargetPosition = lastKnownTargetPosition;
				sensordata.lastKnownTargetPosition.y = 1000f;
				UIFunctions.globaluifunctions.combatai.enemyAircraft[i].attackPlayer = true;
				if (!UIFunctions.globaluifunctions.combatai.enemyAircraft[i].attackPlayerActualTransform)
				{
					UIFunctions.globaluifunctions.combatai.enemyAircraft[i].waypoint.position = GameDataManager.playervesselsonlevel[0].transform.position;
				}
			}
		}
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x000770B0 File Offset: 0x000752B0
	private void CheckEnemyUnitRADAR(int i, int radarID, float distanceInYards, float sizeFactor, SensorData sensordata, string unitType, Vector3 targetPosition)
	{
		float num = UIFunctions.globaluifunctions.database.databaseradardata[radarID].radarRange;
		num *= sizeFactor;
		if (LevelLoadManager.isRaining)
		{
			num /= UnityEngine.Random.Range(1.5f, 2.5f);
		}
		float num2 = 0.5f;
		if (distanceInYards < num)
		{
			float num3 = 1f - distanceInYards / num;
			if (UnityEngine.Random.value < num2)
			{
				sensordata.playerDetected = true;
				sensordata.radarTotalDetected += 500f;
				if (num3 > 0.5f)
				{
					sensordata.radarTotalDetected += 500f;
				}
				sensordata.rangeYardsLastDetected = distanceInYards;
				if (sensordata.radarTotalDetected > UnityEngine.Random.Range(80f, 200f))
				{
					Vector3 vector = GameDataManager.playervesselsonlevel[0].transform.position;
					if (sensordata.timeTrackingPlayer < 60f)
					{
						vector = targetPosition;
					}
					if (unitType == "VESSEL")
					{
						UIFunctions.globaluifunctions.combatai.BeginAttackOnPosition(GameDataManager.enemyvesselsonlevel[i], vector);
					}
					else if (unitType == "HELICOPTER")
					{
						sensordata.lastKnownTargetPosition = vector;
						sensordata.lastKnownTargetPosition.y = 1000f;
						if (!UIFunctions.globaluifunctions.combatai.enemyHelicopters[i].OnAttackRun)
						{
							UIFunctions.globaluifunctions.combatai.enemyHelicopters[i].attackPlayer = true;
						}
					}
					else if (unitType == "AIRCRAFT" && Vector3.Distance(UIFunctions.globaluifunctions.combatai.enemyAircraft[i].transform.position, GameDataManager.playervesselsonlevel[0].transform.position) > 20f)
					{
						sensordata.lastKnownTargetPosition = vector;
						sensordata.lastKnownTargetPosition.y = 1000f;
						if (!UIFunctions.globaluifunctions.combatai.enemyAircraft[i].OnAttackRun)
						{
							UIFunctions.globaluifunctions.combatai.enemyAircraft[i].attackPlayer = true;
						}
					}
				}
			}
		}
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x000772C0 File Offset: 0x000754C0
	private float GetVesselNoiseLevel(Vessel activeVessel)
	{
		float num = activeVessel.databaseshipdata.selfnoise;
		num = num / this.passiveCompressionFactor + this.passiveBaselineFactor;
		num += Mathf.Abs(activeVessel.vesselmovement.shipSpeed.z) * 10f * this.targetNoisePerKnot;
		if (activeVessel.vesselmovement.isCavitating)
		{
			num += this.noiseFromCavitation;
		}
		if (activeVessel.playercontrolled)
		{
			if (PlayerFunctions.runningSilent)
			{
				num -= UnityEngine.Random.Range(this.runningSilentBonus.x, this.runningSilentBonus.y);
			}
			if (activeVessel.acoustics.usingActiveSonar)
			{
				num += UIFunctions.globaluifunctions.database.databasesonardata[activeVessel.databaseshipdata.activeSonarID].sonarOutput - num;
			}
		}
		return num + activeVessel.damagesystem.shipCurrentDamagePoints / activeVessel.damagesystem.shipTotalDamagePoints * 10f;
	}

	// Token: 0x060009F2 RID: 2546 RVA: 0x000773B0 File Offset: 0x000755B0
	private float GetVesselOwnNoiseLevel(Vessel activeVessel)
	{
		float num = this.currentOceanAmbientNoise;
		num += activeVessel.vesselmovement.shipSpeed.z * 10f;
		if (!activeVessel.playercontrolled)
		{
			activeVessel.vesselai.depthUnderkeel = UIFunctions.globaluifunctions.playerfunctions.sensormanager.GetUnitsDepthUnderTransform(activeVessel.transform) * GameDataManager.unitsToFeet;
			if (activeVessel.vesselai.depthUnderkeel < 115f && !activeVessel.vesselai.avoidingTerrain)
			{
				activeVessel.vesselai.AvoidTerrain();
			}
		}
		if (UIFunctions.globaluifunctions.levelloadmanager.currentMapGeneratorInstance != null)
		{
			LayerMask mask = 1073741824;
			Vector3 origin = new Vector3(activeVessel.transform.position.x, 1000f, activeVessel.transform.position.z);
			RaycastHit raycastHit;
			if (Physics.Raycast(origin, -Vector3.up, out raycastHit, 2.66f, mask))
			{
				num += this.maxShallowsAbsorption * (1f - raycastHit.distance / 2.66f);
			}
			if (!activeVessel.playercontrolled && !activeVessel.vesselai.avoidingTerrain && Physics.Raycast(origin, Vector3.forward, out raycastHit, 30f, mask) && !activeVessel.vesselai.avoidingTerrain)
			{
				activeVessel.vesselai.AvoidTerrain();
			}
		}
		return num;
	}

	// Token: 0x060009F3 RID: 2547 RVA: 0x00077530 File Offset: 0x00075730
	private Vector2 GetArrayBonus(Vessel activeVessel, Transform contactPosition)
	{
		if (activeVessel.playercontrolled)
		{
			if (!this.playerfunctions.damagecontrol.CheckSubsystem("TOWED", false))
			{
				return Vector2.zero;
			}
			if (this.playerfunctions.playerDepthUnderKeel < 200)
			{
				return Vector2.zero;
			}
		}
		float num = UIFunctions.globaluifunctions.GetBearingToTransform(activeVessel.transform, contactPosition);
		if (num < 0f)
		{
			num *= -1f;
		}
		if (num < 7.5f)
		{
			return Vector2.zero;
		}
		float num2 = activeVessel.vesselmovement.shipSpeed.z * 10f;
		float num3 = activeVessel.databaseshipdata.passiveArrayBonus;
		float num4 = activeVessel.databaseshipdata.activeArrayBonus;
		if (activeVessel.databaseshipdata.shipType == "SUBMARINE")
		{
			if (num2 > 0f && num2 <= 15f)
			{
				if (num2 <= 5f)
				{
					num3 *= num2 / 5f;
					num4 *= num2 / 5f;
				}
				else
				{
					num3 -= (5f - num2) * -0.1f * num3;
					num4 -= (5f - num2) * -0.1f * num4;
				}
			}
			else
			{
				num3 = 0f;
				num4 = 0f;
			}
		}
		else if (num2 > 0f && num2 <= 25f)
		{
			if (num2 <= 5f)
			{
				num3 *= num2 / 5f;
				num4 *= num2 / 5f;
			}
			else
			{
				num3 -= (5f - num2) * -0.05f * num3;
				num4 -= (5f - num2) * -0.05f * num4;
			}
		}
		else
		{
			num3 = 0f;
			num4 = 0f;
		}
		return new Vector3(num4, num3);
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x000776F4 File Offset: 0x000758F4
	private float GetConvergenceZoneBonus(float distance, GameObject listener, GameObject contact)
	{
		float num = 0f;
		float num2 = Mathf.Abs(this.convergenceZoneDistanceMetres - distance);
		float num3 = this.convergenceZoneDistanceMetres * 0.1f;
		if (num2 < num3 / 3f)
		{
			num = this.convergenceZoneStrength;
		}
		else if (num2 < num3 / 2f)
		{
			num = this.convergenceZoneStrength * 0.5f;
		}
		else if (num2 < num3)
		{
			num = this.convergenceZoneStrength * 0.25f;
		}
		if (this.usingBottomBounce)
		{
			Vector3 vector = (listener.transform.position + contact.transform.position) / 2f;
			float num4 = UIFunctions.globaluifunctions.levelloadmanager.GetTerrainHeightAtPosition(vector.x, vector.z) * GameDataManager.unitsToFeet;
			num4 = Mathf.Abs(num4 - this.bottomBounceCurrentDepth);
			num4 = Mathf.Clamp(num4, 0f, 200f);
			num *= this.maxBottomBounceBonus;
			num /= 1f + num4 / 20f;
		}
		else
		{
			num *= this.maxConvergenceBonus;
		}
		return num;
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x00077810 File Offset: 0x00075A10
	private float GetLayerBonus(Transform listenerTransform, bool listenerAboveLayer, Transform contactTransform, bool contactAboveLayer)
	{
		float result = 0f;
		if (listenerAboveLayer && contactAboveLayer)
		{
			result = this.surfaceDuctStrength * this.maxSurfaceDuctBonus;
		}
		else if (listenerAboveLayer || contactAboveLayer)
		{
			UIFunctions.globaluifunctions.directionFinder.position = listenerTransform.position;
			UIFunctions.globaluifunctions.directionFinder.LookAt(contactTransform.position);
			float num = UIFunctions.globaluifunctions.directionFinder.eulerAngles.x;
			if (num > 180f)
			{
				num = -num + 360f;
			}
			if (num < this.layerAngle)
			{
				result = -this.maxLayerBonus * this.layerStrength;
			}
		}
		return result;
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x000778CC File Offset: 0x00075ACC
	public float GetLandMaskingPenalty(Transform listenerTransform, Transform contactTransform)
	{
		if (UIFunctions.globaluifunctions.levelloadmanager.currentMapGeneratorInstance == null)
		{
			return 1f;
		}
		int layerMask = 1073741824;
		float result = 1f;
		UIFunctions.globaluifunctions.directionFinder.position = listenerTransform.position;
		UIFunctions.globaluifunctions.directionFinder.LookAt(contactTransform.position);
		RaycastHit raycastHit;
		if (UIFunctions.globaluifunctions.levelloadmanager.currentMapGeneratorInstance != null && Physics.Raycast(UIFunctions.globaluifunctions.directionFinder.position, UIFunctions.globaluifunctions.directionFinder.forward, out raycastHit, Vector3.Distance(listenerTransform.position, contactTransform.position), layerMask))
		{
			result = UnityEngine.Random.Range(this.landAbsorptionFold.x, this.landAbsorptionFold.y);
		}
		return result;
	}

	// Token: 0x060009F7 RID: 2551 RVA: 0x000779A4 File Offset: 0x00075BA4
	public float GetUnitsDepthUnderTransform(Transform vesselTransform)
	{
		LayerMask mask = 1073741824;
		RaycastHit raycastHit;
		if (Physics.Raycast(vesselTransform.position, -Vector3.up, out raycastHit, 120f, mask))
		{
			return raycastHit.distance;
		}
		return 20f;
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x000779F0 File Offset: 0x00075BF0
	public bool CheckSingleSonar(GameObject listeningObject, GameObject contactObject, int passiveSonarID, int activeSonarID, int mode, SensorData sensordata, bool isSonobuoy, GameObject sonobuoyObject = null)
	{
		Vessel component = listeningObject.GetComponent<Vessel>();
		string text = "VESSEL";
		if (component == null)
		{
			Helicopter component2 = listeningObject.GetComponent<Helicopter>();
			text = "HELICOPTER";
			if (component2 == null)
			{
				Aircraft component3 = listeningObject.GetComponent<Aircraft>();
				text = "AIRCRAFT";
				if (component3 == null)
				{
					return false;
				}
			}
		}
		Torpedo component4 = contactObject.GetComponent<Torpedo>();
		Vessel vessel = null;
		string text2 = "TORPEDO";
		if (component4 == null)
		{
			vessel = contactObject.GetComponent<Vessel>();
			text2 = "VESSEL";
			if (vessel == null)
			{
				return false;
			}
			if (vessel.playercontrolled && vessel.isSinking)
			{
				return false;
			}
		}
		else
		{
			if (component4.databaseweapondata.weaponType == "MISSILE" || component4.isAirborne)
			{
				return false;
			}
			if (Vector3.Distance(listeningObject.transform.position, contactObject.transform.position) < 2500f * GameDataManager.inverseYardsScale)
			{
				return true;
			}
		}
		if (text == "VESSEL" && text2 == "TORPEDO" && component.playercontrolled && component4.whichNavy == 0 && (component4.onWire || component4.databaseweapondata.searchSettings[0].Length == 0))
		{
			return true;
		}
		GameObject gameObject = listeningObject;
		if (isSonobuoy)
		{
			gameObject = sonobuoyObject;
		}
		bool listenerAboveLayer = true;
		bool flag = true;
		float currentOwnNoise = this.currentOceanAmbientNoise;
		float num = 0f;
		float num2 = 0f;
		string text3 = text;
		switch (text3)
		{
		case "VESSEL":
			listenerAboveLayer = component.acoustics.currentlyAboveLayer;
			currentOwnNoise = component.acoustics.currentOwnNoise;
			break;
		}
		text3 = text2;
		if (text3 != null)
		{
			if (SensorManager.<>f__switch$map22 == null)
			{
				SensorManager.<>f__switch$map22 = new Dictionary<string, int>(2)
				{
					{
						"VESSEL",
						0
					},
					{
						"TORPEDO",
						1
					}
				};
			}
			int num3;
			if (SensorManager.<>f__switch$map22.TryGetValue(text3, out num3))
			{
				if (num3 != 0)
				{
					if (num3 == 1)
					{
						num = component4.databaseweapondata.noiseValues.x;
						if (component4.sensorsActive && !component4.passiveHoming)
						{
							num = component4.databaseweapondata.noiseValues.y;
						}
						if (component4.databaseweapondata.weaponType == "DECOY")
						{
							num = component4.databaseweapondata.noiseValues.x;
						}
						num = num / this.passiveCompressionFactor + this.passiveBaselineFactor;
						if (component4.transform.position.y > this.layerDepth)
						{
							flag = true;
						}
					}
				}
				else
				{
					flag = vessel.acoustics.currentlyAboveLayer;
					num = vessel.acoustics.currentNoise;
				}
			}
		}
		if (text2 == "HELICOPTER")
		{
			listenerAboveLayer = flag;
		}
		if (isSonobuoy)
		{
			listenerAboveLayer = true;
		}
		if (mode == 1)
		{
			num = 0f;
		}
		float num4 = Vector3.Distance(gameObject.transform.position, contactObject.transform.position);
		float num5 = num4 * GameDataManager.yardsScale;
		float num6 = 0f;
		float num7 = 0f;
		float num8 = 0f;
		float num9 = 0f;
		if (num5 > 0f)
		{
			num5 *= 0.9144f;
			num6 = this.attenuationFactor * num5;
			num7 = this.spreadingFactor * Mathf.Log10(num5);
			if (this.convergenceZoneStrength > 0f)
			{
				num9 = this.GetConvergenceZoneBonus(num5, gameObject, contactObject);
			}
			num8 = this.GetLayerBonus(gameObject.transform, listenerAboveLayer, contactObject.transform, flag);
		}
		Vector2 vector = Vector2.zero;
		if (component != null)
		{
			vector = this.GetArrayBonus(component, contactObject.transform);
		}
		if ((mode == 0 || mode == 2) && passiveSonarID != -1)
		{
			num -= num7 + currentOwnNoise - num8 - num9 + num6 - UIFunctions.globaluifunctions.database.databasesonardata[passiveSonarID].sonarPassiveSensitivity;
			if (component != null && component.playercontrolled && !this.playerfunctions.damagecontrol.CheckSubsystem("BOWSONAR", false) && this.playerSonarDamagedPenalty > 0f)
			{
				num /= this.playerSonarDamagedPenalty;
			}
			num += vector.y;
		}
		if (mode > 0)
		{
			float angle = Vector3.Angle(UIFunctions.globaluifunctions.directionFinder.forward, contactObject.transform.forward);
			num2 = this.GetActiveSonarNoise(num5, num7, currentOwnNoise, num8, num9, angle, GameDataManager.playervesselsonlevel[0].databaseshipdata.activesonarreflection, UIFunctions.globaluifunctions.database.databasesonardata[activeSonarID].sonarOutput, num6);
			num2 += UIFunctions.globaluifunctions.database.databasesonardata[activeSonarID].sonarActiveSensitivity;
			if (component != null && component.playercontrolled && !this.playerfunctions.damagecontrol.CheckSubsystem("BOWSONAR", false) && this.playerSonarDamagedPenalty > 0f)
			{
				num /= this.playerSonarDamagedPenalty;
			}
			num2 += vector.x;
		}
		RaycastHit raycastHit;
		if (passiveSonarID != -1 && num > 0f && Physics.Raycast(gameObject.transform.position, UIFunctions.globaluifunctions.directionFinder.forward, out raycastHit, num4, this.maskingLayers))
		{
			if (raycastHit.collider.gameObject.layer == 14)
			{
				float num10 = UnityEngine.Random.Range(3f, 4f);
				num /= num10;
			}
			else
			{
				string name = raycastHit.collider.name;
				int num11 = int.Parse(name);
				float num12 = Vector3.Distance(GameDataManager.enemyvesselsonlevel[num11].transform.position, gameObject.transform.position) * GameDataManager.yardsScale;
				num12 *= 0.9144f;
				float num13 = this.spreadingFactor * Mathf.Log10(num12);
				float num14 = this.attenuationFactor * num12;
				float num15 = this.GetVesselNoiseLevel(GameDataManager.enemyvesselsonlevel[num11]);
				num15 -= num13 + currentOwnNoise + num14 - UIFunctions.globaluifunctions.database.databasesonardata[passiveSonarID].sonarPassiveSensitivity;
				if (num15 > num)
				{
					float num16 = UnityEngine.Random.Range(3f, 4f);
					num /= num16;
				}
			}
		}
		float landMaskingPenalty = this.GetLandMaskingPenalty(gameObject.transform, contactObject.transform);
		num /= landMaskingPenalty;
		num2 /= landMaskingPenalty;
		if (text == "VESSEL" && (this.CheckIfIsInBaffles(component, contactObject.transform) || this.CheckIfIsInVerticalBaffles(component, contactObject.transform)))
		{
			num -= 300f;
			num2 -= 300f;
		}
		if (text2 == "TORPEDO")
		{
			if (num < 0f)
			{
				return false;
			}
			if (!(component4.databaseweapondata.weaponType == "DECOY"))
			{
				return true;
			}
			if (component != null && component.playercontrolled)
			{
				return false;
			}
		}
		if (sensordata != null)
		{
			if (num2 > num)
			{
				num = num2;
			}
			if (num > this.detectionThresholds.x)
			{
				sensordata.playerDetected = true;
				num *= OptionsManager.difficultySettings["EnemyTMARate"];
				sensordata.decibelsTotalDetected += num;
				if (isSonobuoy)
				{
					if (sensordata.decibelsLastDetected < num)
					{
						sensordata.decibelsLastDetected = num;
					}
				}
				else
				{
					sensordata.decibelsLastDetected = num;
				}
				sensordata.rangeYardsLastDetected = num5;
			}
			else if (num >= this.detectionThresholds.y)
			{
				if (sensordata.playerDetected)
				{
					num *= OptionsManager.difficultySettings["EnemyTMARate"];
					sensordata.decibelsTotalDetected += num;
					if (isSonobuoy)
					{
						if (sensordata.decibelsLastDetected < num)
						{
							sensordata.decibelsLastDetected = num;
						}
					}
					else
					{
						sensordata.decibelsLastDetected = num;
					}
					sensordata.rangeYardsLastDetected = num5;
				}
			}
			else if (num < this.detectionThresholds.y && !isSonobuoy)
			{
				sensordata.playerDetected = false;
				sensordata.decibelsLastDetected = 0f;
			}
		}
		return false;
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x00078304 File Offset: 0x00076504
	private void SaveVesselAcoustics(Vessel activeVessel, float noise, float ownNoise, bool aboveLayer)
	{
		activeVessel.acoustics.currentNoise = noise;
		if (activeVessel.playercontrolled)
		{
			activeVessel.acoustics.currentNoise *= OptionsManager.difficultySettings["PlayerNoiseModifier"];
		}
		else
		{
			activeVessel.acoustics.currentNoise *= OptionsManager.difficultySettings["EnemyNoiseModifier"];
		}
		activeVessel.acoustics.currentOwnNoise = ownNoise;
		activeVessel.acoustics.currentlyAboveLayer = aboveLayer;
	}

	// Token: 0x060009FA RID: 2554 RVA: 0x00078388 File Offset: 0x00076588
	public void SonarCheck()
	{
		float vesselNoiseLevel = this.GetVesselNoiseLevel(GameDataManager.playervesselsonlevel[0]);
		float vesselOwnNoiseLevel = this.GetVesselOwnNoiseLevel(GameDataManager.playervesselsonlevel[0]);
		bool flag = false;
		float y = GameDataManager.playervesselsonlevel[0].transform.position.y;
		if (y > this.layerDepth)
		{
			flag = true;
		}
		this.SaveVesselAcoustics(GameDataManager.playervesselsonlevel[0], vesselNoiseLevel, vesselOwnNoiseLevel, flag);
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			GameDataManager.enemyvesselsonlevel[i].acoustics.masked = false;
		}
		int num4 = 0;
		for (int j = 0; j < GameDataManager.enemyvesselsonlevel.Length; j++)
		{
			float num5 = vesselNoiseLevel;
			float num6 = vesselOwnNoiseLevel;
			float y2 = GameDataManager.enemyvesselsonlevel[j].transform.position.y;
			float num7 = this.GetVesselNoiseLevel(GameDataManager.enemyvesselsonlevel[j]);
			float num8 = 0f;
			float vesselOwnNoiseLevel2 = this.GetVesselOwnNoiseLevel(GameDataManager.enemyvesselsonlevel[j]);
			bool flag2 = false;
			if (y2 > this.layerDepth)
			{
				flag2 = true;
			}
			this.SaveVesselAcoustics(GameDataManager.enemyvesselsonlevel[j], num7, vesselOwnNoiseLevel2, flag2);
			GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.localPosition = Vector3.zero;
			GameDataManager.enemyvesselsonlevel[j].acoustics.sensorNavigator.localPosition = Vector3.zero;
			GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.LookAt(GameDataManager.enemyvesselsonlevel[j].transform.position);
			GameDataManager.enemyvesselsonlevel[j].acoustics.sensorNavigator.transform.LookAt(GameDataManager.playervesselsonlevel[0].transform.position);
			float num9 = Vector3.Distance(GameDataManager.playervesselsonlevel[0].transform.position, GameDataManager.enemyvesselsonlevel[j].transform.position);
			float num10 = num9 * GameDataManager.yardsScale;
			float num11 = 0f;
			if (num10 > 0f)
			{
				num10 *= 0.9144f;
				num11 = this.attenuationFactor * num10;
				num = this.spreadingFactor * Mathf.Log10(num10);
				if (this.convergenceZoneStrength > 0f)
				{
					num2 = this.GetConvergenceZoneBonus(num10, GameDataManager.playervesselsonlevel[0].gameObject, GameDataManager.enemyvesselsonlevel[j].gameObject);
				}
				num3 = this.GetLayerBonus(GameDataManager.playervesselsonlevel[0].transform, flag, GameDataManager.enemyvesselsonlevel[j].transform, flag2);
			}
			float num12 = GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.localEulerAngles.y;
			float y3 = GameDataManager.enemyvesselsonlevel[j].acoustics.sensorNavigator.transform.localEulerAngles.y;
			Vector2 arrayBonus = this.GetArrayBonus(GameDataManager.playervesselsonlevel[0], GameDataManager.enemyvesselsonlevel[j].transform);
			float sonarPassiveSensitivity = UIFunctions.globaluifunctions.database.databasesonardata[GameDataManager.playervesselsonlevel[0].databaseshipdata.passiveSonarID].sonarPassiveSensitivity;
			float sonarActiveSensitivity = UIFunctions.globaluifunctions.database.databasesonardata[GameDataManager.playervesselsonlevel[0].databaseshipdata.activeSonarID].sonarActiveSensitivity;
			if (GameDataManager.playervesselsonlevel[0].databaseshipdata.passiveSonarID == -1)
			{
				num7 = -1000f;
			}
			else
			{
				num7 -= num + num6 - num3 - num2 + num11 - sonarPassiveSensitivity;
			}
			if (GameDataManager.playervesselsonlevel[0].databaseshipdata.activeSonarID == -1)
			{
				num8 -= 1000f;
			}
			else
			{
				float angle = Vector3.Angle(GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.forward, GameDataManager.enemyvesselsonlevel[j].transform.forward);
				num8 = this.GetActiveSonarNoise(num10, num, num6, num3, num2, angle, GameDataManager.enemyvesselsonlevel[j].databaseshipdata.activesonarreflection, UIFunctions.globaluifunctions.database.databasesonardata[GameDataManager.playervesselsonlevel[0].databaseshipdata.activeSonarID].sonarOutput, num11);
				num8 += sonarActiveSensitivity;
			}
			if (!this.playerfunctions.damagecontrol.CheckSubsystem("BOWSONAR", false) && this.playerSonarDamagedPenalty > 0f)
			{
				num7 /= this.playerSonarDamagedPenalty;
				num8 /= this.playerSonarDamagedPenalty;
			}
			Vector2 arrayBonus2 = this.GetArrayBonus(GameDataManager.enemyvesselsonlevel[j], GameDataManager.playervesselsonlevel[0].transform);
			if (GameDataManager.enemyvesselsonlevel[j].databaseshipdata.passiveSonarID != -1)
			{
				num5 -= num + vesselOwnNoiseLevel2 - num3 - num2 + num11 - UIFunctions.globaluifunctions.database.databasesonardata[GameDataManager.enemyvesselsonlevel[j].databaseshipdata.passiveSonarID].sonarPassiveSensitivity;
			}
			else
			{
				num5 = -1000f;
			}
			float num13;
			if (GameDataManager.enemyvesselsonlevel[j].databaseshipdata.activeSonarID != -1)
			{
				float angle2 = Vector3.Angle(GameDataManager.enemyvesselsonlevel[j].acoustics.sensorNavigator.forward, GameDataManager.playervesselsonlevel[0].transform.forward);
				num13 = this.GetActiveSonarNoise(num10, num, vesselOwnNoiseLevel2, num3, num2, angle2, GameDataManager.playervesselsonlevel[0].databaseshipdata.activesonarreflection, UIFunctions.globaluifunctions.database.databasesonardata[GameDataManager.enemyvesselsonlevel[j].databaseshipdata.activeSonarID].sonarOutput, num11);
				num13 += UIFunctions.globaluifunctions.database.databasesonardata[GameDataManager.enemyvesselsonlevel[j].databaseshipdata.activeSonarID].sonarActiveSensitivity;
			}
			else
			{
				num13 = -1000f;
			}
			if (this.CheckIfIsInBaffles(GameDataManager.playervesselsonlevel[0], GameDataManager.enemyvesselsonlevel[j].transform) || this.CheckIfIsInVerticalBaffles(GameDataManager.playervesselsonlevel[0], GameDataManager.enemyvesselsonlevel[j].transform))
			{
				num7 -= 300f;
				num8 -= 300f;
			}
			if (this.CheckIfIsInBaffles(GameDataManager.enemyvesselsonlevel[j], GameDataManager.playervesselsonlevel[0].transform) || this.CheckIfIsInVerticalBaffles(GameDataManager.enemyvesselsonlevel[j], GameDataManager.playervesselsonlevel[0].transform))
			{
				num5 -= 300f;
				num13 -= 300f;
			}
			float num14 = Vector3.Distance(GameDataManager.playervesselsonlevel[0].transform.position, GameDataManager.enemyvesselsonlevel[j].transform.position);
			if (this.initialRun)
			{
				num7 += arrayBonus.y;
				num5 += arrayBonus2.y;
				this.signalLastPassiveReading[j] = num7;
				if (num13 > num5)
				{
					num5 = num13;
				}
				this.sensorLastPassiveReading[j] = num5;
			}
			else
			{
				RaycastHit raycastHit;
				if (num7 + arrayBonus.y > 0f && Physics.Raycast(GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.position, GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.forward, out raycastHit, num9, this.maskingLayers))
				{
					if (raycastHit.collider.gameObject.layer == 14)
					{
						float num15 = UnityEngine.Random.Range(3f, 4f);
						num7 /= num15;
					}
					else
					{
						string name = raycastHit.collider.name;
						int num16 = int.Parse(name);
						if (this.signalLastPassiveReading[num16] > this.signalLastPassiveReading[j])
						{
							if (this.identifiedByPlayer[num16] || this.identifiedByPlayer[j])
							{
								float num17 = UnityEngine.Random.Range(2f, 3f);
								num7 /= num17;
							}
							else
							{
								float num18 = UnityEngine.Random.Range(3f, 4f);
								num7 /= num18;
							}
						}
					}
				}
				if (GameDataManager.enemyvesselsonlevel[j].databaseshipdata.passiveSonarID != -1 && num5 + arrayBonus2.y > 0f && Physics.Raycast(GameDataManager.enemyvesselsonlevel[j].acoustics.sensorNavigator.transform.position, GameDataManager.enemyvesselsonlevel[j].acoustics.sensorNavigator.transform.forward, out raycastHit, num9, this.maskingLayers))
				{
					if (raycastHit.collider.gameObject.layer == 14)
					{
						float num19 = UnityEngine.Random.Range(3f, 4f);
						num7 /= num19;
					}
					else
					{
						string name2 = raycastHit.collider.name;
						int num20 = int.Parse(name2);
						float num21 = Vector3.Distance(GameDataManager.enemyvesselsonlevel[num20].transform.position, GameDataManager.enemyvesselsonlevel[j].transform.position) * GameDataManager.yardsScale;
						num21 *= 0.9144f;
						float num22 = this.spreadingFactor * Mathf.Log10(num21);
						float num23 = this.attenuationFactor * num21;
						float num24 = this.GetVesselNoiseLevel(GameDataManager.enemyvesselsonlevel[num20]);
						num24 -= num22 + vesselOwnNoiseLevel2 + num23 - UIFunctions.globaluifunctions.database.databasesonardata[GameDataManager.enemyvesselsonlevel[j].databaseshipdata.passiveSonarID].sonarPassiveSensitivity;
						if (num24 > num5)
						{
							float num25 = UnityEngine.Random.Range(3f, 4f);
							num5 /= num25;
						}
					}
				}
				if (UIFunctions.globaluifunctions.levelloadmanager.currentMapGeneratorInstance != null)
				{
					LayerMask mask = 1073741824;
					if (Physics.Raycast(GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.position, GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.forward, out raycastHit, num9, mask))
					{
						float num26 = UnityEngine.Random.Range(this.landAbsorptionFold.x, this.landAbsorptionFold.y);
						num5 /= num26;
						num7 /= num26;
					}
				}
				GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.playerSignatureData[1] = num7;
				GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.contactSignatureData[1] = num5;
				GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.playerSignatureData[0] = num8;
				GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.contactSignatureData[0] = num13;
				if (UIFunctions.globaluifunctions.database.databaseshipdata[GameDataManager.playervesselsonlevel[0].databaseshipdata.shipID].passiveArrayBonus > 0f)
				{
					float num27 = arrayBonus.y / UIFunctions.globaluifunctions.database.databaseshipdata[GameDataManager.playervesselsonlevel[0].databaseshipdata.shipID].passiveArrayBonus;
					float num28 = (num7 + arrayBonus.y) * num27;
					GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.playerSignatureData[2] = num28;
					if (num28 > num7)
					{
						num7 = num28;
					}
				}
				else
				{
					GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.playerSignatureData[2] = 0f;
				}
				if (UIFunctions.globaluifunctions.database.databaseshipdata[GameDataManager.enemyvesselsonlevel[j].databaseshipdata.shipID].passiveArrayBonus > 0f)
				{
					float num29 = arrayBonus2.y / UIFunctions.globaluifunctions.database.databaseshipdata[GameDataManager.enemyvesselsonlevel[j].databaseshipdata.shipID].passiveArrayBonus;
					float num30 = (num5 + arrayBonus2.y) * num29;
					GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.contactSignatureData[2] = num30;
					if (num30 > num5)
					{
						num5 = num30;
					}
				}
				else
				{
					GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.contactSignatureData[2] = 0f;
				}
				bool playerDetectedWithActive = false;
				if (GameDataManager.playervesselsonlevel[0].acoustics.usingActiveSonar)
				{
					if (num8 > num7)
					{
						num7 = num8;
					}
					if (num8 > 0f)
					{
						playerDetectedWithActive = true;
					}
				}
				if (GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata != null && GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.usingActiveSonar && num13 > num5)
				{
					num5 = num13;
				}
				Vessel vessel = GameDataManager.enemyvesselsonlevel[j];
				num14 *= GameDataManager.yardsScale;
				this.signalFromContacts[j] = num7;
				this.sensorOfContacts[j] = num5;
				float y4;
				num12 = (y4 = GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.eulerAngles.y);
				this.bearingToContacts[j] = y4;
				if (this.bearingToContacts[j] > 359.5f)
				{
					this.bearingToContacts[j] = 0f;
				}
				this.headingOfContacts[j] = vessel.transform.eulerAngles.y;
				this.speedOfContacts[j] = vessel.vesselmovement.shipSpeed.z * 10f;
				this.rangeToContacts[j] = num14;
				this.signalLastPassiveReading[j] = num7;
				this.sensorLastPassiveReading[j] = num5;
				if (num7 > this.detectionThresholds.x)
				{
					if (num4 == 0)
					{
						if (!this.detectedByPlayer[j])
						{
							GameDataManager.enemyvesselsonlevel[j].acoustics.playerHasDetectedWith[0] = true;
							num4++;
							this.ContactDetected(j, "SONAR");
							if (this.solutionQualityOfContacts[j] <= 0f)
							{
								this.solutionQualityOfContacts[j] = num7 / 10f + UnityEngine.Random.Range(4f, 11f);
							}
							if (MissionManager.initialBearing == string.Empty)
							{
								MissionManager.initialBearing = string.Format("{0:0}", num12);
							}
						}
						else
						{
							this.CheckIfUpgradeContactToMaster(j, false);
						}
					}
				}
				else if (this.detectedByPlayer[j] && num7 < this.detectionThresholds.y && !GameDataManager.enemyvesselsonlevel[j].isSinking)
				{
					GameDataManager.enemyvesselsonlevel[j].acoustics.playerHasDetectedWith[0] = false;
					this.CheckIfContactLost(j, "SONAR", false);
				}
				this.SetShipDisplayed(j);
				if (!this.initialRun)
				{
					if (this.detectedByPlayer[j])
					{
						this.CalculatePlayerTMA(j, playerDetectedWithActive);
					}
					else
					{
						this.solutionQualityOfContacts[j] -= (float)UnityEngine.Random.Range(4, 10);
						if (this.solutionQualityOfContacts[j] < 0f)
						{
							this.solutionQualityOfContacts[j] = UnityEngine.Random.Range(1f, 10f);
						}
					}
				}
				if (GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata != null)
				{
					if (GameDataManager.enemyvesselsonlevel[j].isSinking || GameDataManager.enemyvesselsonlevel[j].isCapsizing)
					{
						this.ClearAllSensorData(j);
					}
					else if (num5 > this.detectionThresholds.x)
					{
						num5 *= OptionsManager.difficultySettings["EnemyTMARate"];
						if (!UIFunctions.globaluifunctions.combatai.playerWasDetected)
						{
							UIFunctions.globaluifunctions.combatai.playerWasDetected = true;
						}
						GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.playerDetected = true;
						GameDataManager.enemyvesselsonlevel[j].vesselai.subCanUseActiveSonar = true;
						GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.decibelsTotalDetected += num5;
						GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.decibelsLastDetected = num5;
						GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.rangeYardsLastDetected = num10;
					}
					else if (num5 >= this.detectionThresholds.y)
					{
						if (GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.playerDetected)
						{
							num5 *= OptionsManager.difficultySettings["EnemyTMARate"];
							GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.decibelsTotalDetected += num5;
							GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.decibelsLastDetected = num5;
							GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.rangeYardsLastDetected = num10;
						}
					}
					else if (num5 < this.detectionThresholds.y)
					{
						GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.playerDetected = false;
						GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.decibelsLastDetected = 0f;
					}
				}
			}
			if (this.truthIsOutThere)
			{
				if (!this.detectedByPlayer[j])
				{
					this.ContactDetected(j, "SONAR");
				}
				this.solutionQualityOfContacts[j] = 100f;
				string text = "\n\n\n\n" + this.initialDetectedByPlayerName[j] + " " + GameDataManager.enemyvesselsonlevel[j].databaseshipdata.shipclass;
				text = text + "\nR: " + string.Format("{0:0.0}", this.rangeToContacts[j]);
				text = text + "\nSIG: A: " + string.Format("{0:0.0}", GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.playerSignatureData[0]);
				text = text + " P: " + string.Format("{0:0.0}", GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.playerSignatureData[1]);
				text = text + " T: " + string.Format("{0:0.0}", GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.playerSignatureData[2]);
				text = text + "\nSENS: A: " + string.Format("{0:0.0}", GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.contactSignatureData[0]);
				text = text + " P: " + string.Format("{0:0.0}", GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.contactSignatureData[1]);
				text = text + " T: " + string.Format("{0:0.0}", GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.contactSignatureData[2]);
				text = text + "\nPlayerDetected: " + GameDataManager.enemyvesselsonlevel[j].vesselai.sensordata.playerDetected;
				if (GameDataManager.enemyvesselsonlevel[j].vesselai.attackRole != null)
				{
					text = text + "\nRole: " + GameDataManager.enemyvesselsonlevel[j].vesselai.attackRole;
				}
				text = text + "\nSpeed: " + string.Format("{0:0.0}", GameDataManager.enemyvesselsonlevel[j].vesselmovement.shipSpeed.z * 10f);
				text = text + "\nNoise: " + string.Format("{0:0.0}", GameDataManager.enemyvesselsonlevel[j].acoustics.currentNoise);
				this.tacticalmap.mapContact[j].contactText.text = text;
			}
		}
		if (this.playerfunctions.currentOpenPanel == 1)
		{
			this.SetSignatureData();
		}
		this.playerTransient = false;
	}

	// Token: 0x060009FB RID: 2555 RVA: 0x0007971C File Offset: 0x0007791C
	public void ClearAllSensorData(int i)
	{
		GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata.decibelsTotalDetected = 0f;
		GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata.decibelsLastDetected = 0f;
		GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata.radarTotalDetected = 0f;
		GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata.timeTrackingPlayer = 0f;
		GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata.playerDetected = false;
	}

	// Token: 0x060009FC RID: 2556 RVA: 0x000797AC File Offset: 0x000779AC
	public void CheckIfContactLost(int i, string sensorType, bool playerBaffled = false)
	{
		if (this.truthIsOutThere)
		{
			return;
		}
		if (this.GetNumberOfPlayerDetectionTypes(i) == 0)
		{
			this.tacticalmap.ClearMapContactTrail(this.tacticalmap.mapContact[i], true);
			this.tacticalmap.mapContact[i].gameObject.SetActive(true);
			this.tacticalmap.mapContact[i].shipDisplayIcon.color = new Color(this.tacticalmap.mapContact[i].shipDisplayIcon.color.r, this.tacticalmap.mapContact[i].shipDisplayIcon.color.g, this.tacticalmap.mapContact[i].shipDisplayIcon.color.b, 0.5f);
			this.tacticalmap.mapContact[i].contactText.color = new Color(this.tacticalmap.mapContact[i].shipDisplayIcon.color.r, this.tacticalmap.mapContact[i].shipDisplayIcon.color.g, this.tacticalmap.mapContact[i].shipDisplayIcon.color.b, 0.5f);
			this.detectedByPlayer[i] = false;
			this.identifiedByPlayer[i] = false;
			if (ManualCameraZoom.target == GameDataManager.enemyvesselsonlevel[i].transform)
			{
				this.ReturnToPlayerSub();
			}
			if (this.playerfunctions.currentTargetIndex == i)
			{
				this.playerfunctions.contactDataName.text = string.Empty;
				this.playerfunctions.contactData.text = string.Empty;
				this.playerfunctions.currentTargetIndex = -1;
			}
			this.enemyPositions[i] = Vector3.zero;
			if (this.playerfunctions.statusscreens.conditionsObject.activeSelf)
			{
				this.conditionsdisplay.UpdateConditionsDisplay(false);
			}
			this.solutionQualityOfContacts[i] -= (float)UnityEngine.Random.Range(10, 15);
			if (this.solutionQualityOfContacts[i] < 0f)
			{
				this.solutionQualityOfContacts[i] = 0f;
			}
			if (sensorType == "SONAR")
			{
				if (!this.CheckIfIsInBaffles(GameDataManager.playervesselsonlevel[0], GameDataManager.enemyvesselsonlevel[i].transform))
				{
					this.playerfunctions.voicemanager.currentVesselIndex = i;
					string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "LostSonarContact");
					text = text.Replace("<BRG>", string.Format("{0:0}", this.bearingToContacts[i]));
					text = text.Replace("<CONTACT>", this.playerfunctions.GetFullContactName(this.tacticalmap.mapContact[i].contactText.text, i));
					this.playerfunctions.PlayerMessage(text, this.playerfunctions.messageLogColors["Sonar"], "LostSonarContact", false);
				}
				else
				{
					this.playerfunctions.voicemanager.currentVesselIndex = i;
					string text2 = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "LostSonarContactBaffles");
					text2 = text2.Replace("<BRG>", string.Format("{0:0}", this.bearingToContacts[i]));
					text2 = text2.Replace("<CONTACT>", this.playerfunctions.GetFullContactName(this.tacticalmap.mapContact[i].contactText.text, i));
					this.playerfunctions.PlayerMessage(text2, this.playerfunctions.messageLogColors["Sonar"], "LostSonarContactBaffles", false);
				}
			}
			else if (sensorType == "RADAR")
			{
				string text3 = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "LostRADARContact");
				text3 = text3.Replace("<BRG>", string.Format("{0:0}", this.bearingToContacts[i]));
				text3 = text3.Replace("<CONTACT>", this.playerfunctions.GetFullContactName(this.tacticalmap.mapContact[i].contactText.text, i));
				this.playerfunctions.PlayerMessage(text3, this.playerfunctions.messageLogColors["XO"], "LostRADARContact", false);
			}
			else if (sensorType == "ESM")
			{
				string text4 = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "LostESMContact");
				text4 = text4.Replace("<BRG>", string.Format("{0:0}", this.bearingToContacts[i]));
				text4 = text4.Replace("<CONTACT>", this.playerfunctions.GetFullContactName(this.tacticalmap.mapContact[i].contactText.text, i));
				this.playerfunctions.PlayerMessage(text4, this.playerfunctions.messageLogColors["XO"], "LostESMContact", false);
			}
			else if (sensorType == "VISUAL")
			{
				string text5 = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "LostVisualContact");
				text5 = text5.Replace("<BRG>", string.Format("{0:0}", this.bearingToContacts[i]));
				text5 = text5.Replace("<CONTACT>", this.playerfunctions.GetFullContactName(this.tacticalmap.mapContact[i].contactText.text, i));
				this.playerfunctions.PlayerMessage(text5, this.playerfunctions.messageLogColors["XO"], "LostVisualContact", false);
			}
		}
	}

	// Token: 0x060009FD RID: 2557 RVA: 0x00079D3C File Offset: 0x00077F3C
	public bool HaveVisualOnPlayerSub(Vector3 viewerPosition)
	{
		float num = Vector3.Distance(GameDataManager.playervesselsonlevel[0].transform.position, viewerPosition);
		if (GameDataManager.playervesselsonlevel[0].vesselmovement.percentageSurfaced > 0.1f && num < 15f)
		{
			return true;
		}
		for (int i = 0; i < GameDataManager.playervesselsonlevel[0].submarineFunctions.mastHeads.Length; i++)
		{
			if (GameDataManager.playervesselsonlevel[0].submarineFunctions.GetMastIsUp(i) && num < 8f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060009FE RID: 2558 RVA: 0x00079DD4 File Offset: 0x00077FD4
	public float GetDistanceToNearestEnemyVessel(Transform t, bool surfaceOnly, bool convertToYards = true)
	{
		float num = 20000f;
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			float num2 = Vector3.Distance(t.position, GameDataManager.enemyvesselsonlevel[i].transform.position);
			if (num2 < num)
			{
				num = num2;
			}
		}
		if (convertToYards)
		{
			num *= GameDataManager.yardsScale;
		}
		return num;
	}

	// Token: 0x060009FF RID: 2559 RVA: 0x00079E34 File Offset: 0x00078034
	public float GetDistanceToNearestEnemyHelicopter(Transform t, bool convertToYards = true)
	{
		float num = 20000f;
		for (int i = 0; i < UIFunctions.globaluifunctions.combatai.enemyHelicopters.Length; i++)
		{
			if (t != UIFunctions.globaluifunctions.combatai.enemyHelicopters[i].transform)
			{
				float num2 = Vector3.Distance(t.position, UIFunctions.globaluifunctions.combatai.enemyHelicopters[i].transform.position);
				if (num2 < num && num != 0f)
				{
					num = num2;
				}
			}
		}
		if (convertToYards)
		{
			num *= GameDataManager.yardsScale;
		}
		return num;
	}

	// Token: 0x06000A00 RID: 2560 RVA: 0x00079ED4 File Offset: 0x000780D4
	public void SetSignatureData()
	{
		if (this.playerfunctions.currentTargetIndex < 0)
		{
			this.ClearSignatureData();
			this.playerfunctions.SetProfileGraphic();
			this.playerfunctions.SetContactGraphic();
			return;
		}
		this.playerfunctions.signatureData[4].text = string.Empty;
		this.playerfunctions.signatureData[5].text = string.Empty;
		if (this.classifiedByPlayer[this.playerfunctions.currentTargetIndex])
		{
			this.SetSonarSignatureLabelData(this.classifiedByPlayerAsClass[this.playerfunctions.currentTargetIndex], 4);
			this.playerfunctions.signatureData[5].text = this.GetSonarReadingValues(GameDataManager.enemyvesselsonlevel[this.playerfunctions.currentTargetIndex], GameDataManager.enemyvesselsonlevel[this.playerfunctions.currentTargetIndex].vesselai.sensordata.contactSignatureData, false);
		}
		this.playerfunctions.signatureData[1].text = this.tacticalmap.mapContact[this.playerfunctions.currentTargetIndex].contactText.text.ToUpper();
		this.playerfunctions.signatureData[3].text = this.GetSonarReadingValues(GameDataManager.enemyvesselsonlevel[this.playerfunctions.currentTargetIndex], GameDataManager.enemyvesselsonlevel[this.playerfunctions.currentTargetIndex].vesselai.sensordata.playerSignatureData, true);
		this.playerfunctions.SetContactGraphic();
	}

	// Token: 0x06000A01 RID: 2561 RVA: 0x0007A044 File Offset: 0x00078244
	private string GetSonarReadingValues(Vessel activeVessel, float[] sonarData, bool isPlayerData)
	{
		string text = string.Empty;
		if (isPlayerData)
		{
			if (this.playerfunctions.playerVessel.databaseshipdata.activeSonarID == -1)
			{
				text += "\n";
			}
			else
			{
				float num = sonarData[0];
				if (num < -50f)
				{
					text += "-50\n";
				}
				else
				{
					text = text + string.Format("{0:0}", num) + "\n";
				}
			}
			if (this.playerfunctions.playerVessel.databaseshipdata.passiveSonarID == -1)
			{
				text += "\n";
			}
			else
			{
				float num = sonarData[1];
				if (num < -50f)
				{
					text += "-50\n";
				}
				else
				{
					text = text + string.Format("{0:0}", num) + "\n";
				}
			}
			if (this.playerfunctions.playerVessel.databaseshipdata.towedSonarID == -1 || !this.playerfunctions.damagecontrol.CheckSubsystem("TOWED", false))
			{
				text += "\n";
			}
			else
			{
				float num = sonarData[2];
				if (num < -50f)
				{
					text += "-50\n";
				}
				else
				{
					text = text + string.Format("{0:0}", num) + "\n";
				}
			}
			return text;
		}
		if (UIFunctions.globaluifunctions.database.databaseshipdata[this.classifiedByPlayerAsClass[this.playerfunctions.currentTargetIndex]].activeSonarID == -1)
		{
			text += "\n";
		}
		else
		{
			float num = sonarData[0];
			if (num < -50f)
			{
				text += "-50\n";
			}
			else
			{
				text = text + string.Format("{0:0}", num) + "\n";
			}
		}
		if (UIFunctions.globaluifunctions.database.databaseshipdata[this.classifiedByPlayerAsClass[this.playerfunctions.currentTargetIndex]].passiveSonarID == -1)
		{
			text += "\n";
		}
		else
		{
			float num = sonarData[1];
			if (num < -50f)
			{
				text += "-50\n";
			}
			else
			{
				text = text + string.Format("{0:0}", num) + "\n";
			}
		}
		if (UIFunctions.globaluifunctions.database.databaseshipdata[this.classifiedByPlayerAsClass[this.playerfunctions.currentTargetIndex]].towedSonarID == -1)
		{
			text += "\n";
		}
		else
		{
			float num = sonarData[2];
			if (num < -50f)
			{
				text += "-50\n";
			}
			else
			{
				text = text + string.Format("{0:0}", num) + "\n";
			}
		}
		return text;
	}

	// Token: 0x06000A02 RID: 2562 RVA: 0x0007A328 File Offset: 0x00078528
	private void ClearSignatureData()
	{
		this.playerfunctions.signatureData[1].text = LanguageManager.interfaceDictionary["NoContact"];
		this.playerfunctions.signatureData[3].text = string.Empty;
		this.playerfunctions.signatureData[4].text = string.Empty;
		this.playerfunctions.signatureData[5].text = string.Empty;
		this.playerfunctions.signatureMaterials[1].SetTexture("_Signature", UIFunctions.globaluifunctions.textparser.GetTexture("vessels/signature/blank_sig.png"));
		GC.Collect();
	}

	// Token: 0x06000A03 RID: 2563 RVA: 0x0007A3CC File Offset: 0x000785CC
	public void SetSonarSignatureLabelData(int vesselIndex, int textIndex)
	{
		if (UIFunctions.globaluifunctions.database.databaseshipdata[vesselIndex].activeSonarID != -1)
		{
			for (int i = 0; i < UIFunctions.globaluifunctions.database.databasesonardata[UIFunctions.globaluifunctions.database.databaseshipdata[vesselIndex].activeSonarID].sonarFrequencies.Length; i++)
			{
				this.playerfunctions.signatureData[textIndex].text = UIFunctions.globaluifunctions.database.databasesonardata[UIFunctions.globaluifunctions.database.databaseshipdata[vesselIndex].activeSonarID].sonarFrequencies[i] + LanguageManager.interfaceDictionary["FrequencyAbbreviated"] + " ";
			}
			Text text = this.playerfunctions.signatureData[textIndex];
			text.text = text.text + LanguageManager.interfaceDictionary["ReferenceActive"].ToUpper() + "\n";
		}
		else
		{
			this.playerfunctions.signatureData[textIndex].text = "\n";
		}
		if (UIFunctions.globaluifunctions.database.databaseshipdata[vesselIndex].passiveSonarID != -1)
		{
			for (int j = 0; j < UIFunctions.globaluifunctions.database.databasesonardata[UIFunctions.globaluifunctions.database.databaseshipdata[vesselIndex].passiveSonarID].sonarFrequencies.Length; j++)
			{
				Text text2 = this.playerfunctions.signatureData[textIndex];
				text2.text = text2.text + UIFunctions.globaluifunctions.database.databasesonardata[UIFunctions.globaluifunctions.database.databaseshipdata[vesselIndex].passiveSonarID].sonarFrequencies[j] + LanguageManager.interfaceDictionary["FrequencyAbbreviated"] + " ";
			}
			Text text3 = this.playerfunctions.signatureData[textIndex];
			text3.text = text3.text + LanguageManager.interfaceDictionary["ReferencePassive"].ToUpper() + "\n";
		}
		else
		{
			Text text4 = this.playerfunctions.signatureData[textIndex];
			text4.text += "\n";
		}
		if (UIFunctions.globaluifunctions.database.databaseshipdata[vesselIndex].towedSonarID != -1)
		{
			for (int k = 0; k < UIFunctions.globaluifunctions.database.databasesonardata[UIFunctions.globaluifunctions.database.databaseshipdata[vesselIndex].towedSonarID].sonarFrequencies.Length; k++)
			{
				Text text5 = this.playerfunctions.signatureData[textIndex];
				text5.text = text5.text + UIFunctions.globaluifunctions.database.databasesonardata[UIFunctions.globaluifunctions.database.databaseshipdata[vesselIndex].towedSonarID].sonarFrequencies[k] + LanguageManager.interfaceDictionary["FrequencyAbbreviated"] + " ";
			}
			Text text6 = this.playerfunctions.signatureData[textIndex];
			text6.text = text6.text + LanguageManager.interfaceDictionary["ReferenceTowed"].ToUpper() + "\n";
		}
	}

	// Token: 0x06000A04 RID: 2564 RVA: 0x0007A6E0 File Offset: 0x000788E0
	private bool CompareVesselPrefabNames(string actualID, string classifiedID)
	{
		string[] array = actualID.Split(new char[]
		{
			'_'
		});
		string[] array2 = classifiedID.Split(new char[]
		{
			'_'
		});
		string text = string.Empty;
		string text2 = string.Empty;
		for (int i = 1; i < array.Length; i++)
		{
			text += array[i];
		}
		for (int j = 1; j < array2.Length; j++)
		{
			text2 += array2[j];
		}
		return text == text2;
	}

	// Token: 0x06000A05 RID: 2565 RVA: 0x0007A774 File Offset: 0x00078974
	private void CalculatePlayerTMA(int enemyIndex, bool playerDetectedWithActive)
	{
		if (!this.addTMA)
		{
			return;
		}
		bool flag = false;
		if (this.classifiedByPlayer[enemyIndex] && this.classifiedByPlayerAsClass[enemyIndex] != GameDataManager.enemyvesselsonlevel[enemyIndex].databaseshipdata.shipID && !this.CompareVesselPrefabNames(GameDataManager.enemyvesselsonlevel[enemyIndex].databaseshipdata.shipPrefabName, UIFunctions.globaluifunctions.database.databaseshipdata[this.classifiedByPlayerAsClass[enemyIndex]].shipPrefabName))
		{
			flag = true;
		}
		if (!this.identifiedByPlayer[enemyIndex] && this.signalFromContactsType[enemyIndex] == "P" && this.timeTrackingContact[enemyIndex] * this.signalFromContacts[enemyIndex] > UnityEngine.Random.Range(500f, 900f))
		{
			this.ContactIdentified(enemyIndex);
		}
		if ((!this.classifiedByPlayer[enemyIndex] || flag) && this.solutionQualityOfContacts[enemyIndex] >= this.tacticalmap.qualityToDrawTails)
		{
			this.ContactClassified(enemyIndex);
		}
		float num = this.solutionRangeErrors[enemyIndex];
		if (num < 1f)
		{
			num = -4.285f * num + 5.285f;
		}
		num = (num - 1f) * 33.334f;
		float num2 = this.solutionQualityOfContacts[enemyIndex];
		float num3 = 0f;
		if (num2 < 80f && num < 5f)
		{
			this.CalculateRangeError(enemyIndex);
			this.solutionQualityOfContacts[enemyIndex] -= UnityEngine.Random.Range(2f, 15f);
		}
		if (num2 > 85f && num > 0f)
		{
			this.solutionQualityOfContacts[enemyIndex] += UnityEngine.Random.Range(2f, 15f);
			this.solutionRangeErrors[enemyIndex] = 1f;
		}
		else if (num > num2 && UnityEngine.Random.value * 4f < num / 100f)
		{
			this.CalculateRangeError(enemyIndex);
			this.solutionQualityOfContacts[enemyIndex] -= UnityEngine.Random.Range(2f, 15f);
		}
		if (GameDataManager.enemyvesselsonlevel[enemyIndex].vesselai.vesselIsDecoyed)
		{
			this.solutionQualityOfContacts[enemyIndex] -= GameDataManager.enemyvesselsonlevel[enemyIndex].vesselai.decoyTimer / 2f;
		}
		if (this.solutionQualityOfContacts[enemyIndex] < num3)
		{
			this.solutionQualityOfContacts[enemyIndex] = UnityEngine.Random.Range(1f, 10f);
		}
		if (this.enemyPositions[enemyIndex] != Vector3.zero)
		{
			Vector3 from = GameDataManager.enemyvesselsonlevel[enemyIndex].transform.position - GameDataManager.playervesselsonlevel[0].transform.position;
			Vector3 to = this.enemyPositions[enemyIndex] - GameDataManager.playervesselsonlevel[0].transform.position;
			float num4 = Vector3.Angle(from, to);
			float num5 = GameDataManager.enemyvesselsonlevel[enemyIndex].vesselmovement.rudderAngle.y * GameDataManager.enemyvesselsonlevel[enemyIndex].vesselmovement.percentageSpeed;
			num5 += GameDataManager.playervesselsonlevel[0].vesselmovement.rudderAngle.y * GameDataManager.playervesselsonlevel[0].vesselmovement.percentageSpeed / 3f;
			if (GameDataManager.enemyvesselsonlevel[enemyIndex].acoustics.playerHasDetectedWith[1] || GameDataManager.enemyvesselsonlevel[enemyIndex].acoustics.playerHasDetectedWith[2])
			{
				num5 = 0f;
			}
			float num6 = 2f;
			float num7 = this.solutionQualityOfContacts[enemyIndex];
			if (this.classifiedByPlayer[enemyIndex])
			{
				num6 += 1f;
			}
			else if (this.identifiedByPlayer[enemyIndex])
			{
				num6 += 0.5f;
			}
			if (this.playerfunctions.currentTargetIndex == enemyIndex)
			{
				num6 += 1f;
			}
			if (flag)
			{
				num6 -= 1f;
			}
			else
			{
				num6 += 1f;
			}
			num7 += (num6 * num4 - num5 * 4f) * OptionsManager.difficultySettings["PlayerTMARate"];
			if (playerDetectedWithActive)
			{
				num7 += UIFunctions.globaluifunctions.playerfunctions.maximumPlayerTMA / 1.5f;
			}
			else if (num7 < 30f)
			{
				num7 += UnityEngine.Random.Range(5f, 15f);
			}
			if (num7 > UIFunctions.globaluifunctions.playerfunctions.maximumPlayerTMA)
			{
				num7 = UIFunctions.globaluifunctions.playerfunctions.maximumPlayerTMA;
			}
			else if (num7 < 0f)
			{
				num7 = 0f;
			}
			this.solutionQualityOfContacts[enemyIndex] = num7;
		}
		GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.LookAt(GameDataManager.enemyvesselsonlevel[enemyIndex].transform.position);
		GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.Translate(Vector3.forward * this.solutionRangeErrors[enemyIndex] * GameDataManager.inverseYardsScale * this.rangeToContacts[enemyIndex]);
		this.enemyPositions[enemyIndex] = GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.position;
		GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.localPosition = Vector3.zero;
	}

	// Token: 0x06000A06 RID: 2566 RVA: 0x0007ACDC File Offset: 0x00078EDC
	public bool IsVesselVisibleByPeriscope(int i)
	{
		if (UIFunctions.globaluifunctions.playerfunctions.playerVessel.submarineFunctions.GetMastIsUp(0) && UIFunctions.globaluifunctions.playerfunctions.damagecontrol.CheckSubsystem("PERISCOPE", false) && GameDataManager.enemyvesselsonlevel[i].transform.position.y > 999.9f && this.rangeToContacts[i] < this.visualRange)
		{
			GameDataManager.enemyvesselsonlevel[i].acoustics.playerHasDetectedWith[1] = true;
			return true;
		}
		return false;
	}

	// Token: 0x06000A07 RID: 2567 RVA: 0x0007AD74 File Offset: 0x00078F74
	public void SetShipDisplayed(int i)
	{
		if (!GameDataManager.optionsBoolSettings[15] || this.solutionQualityOfContacts[i] >= this.tacticalmap.qualityToDrawTails || GameDataManager.enemyvesselsonlevel[i].isSinking || this.IsVesselVisibleByPeriscope(i))
		{
			if (!GameDataManager.enemyvesselsonlevel[i].meshHolder.gameObject.activeSelf)
			{
				GameDataManager.enemyvesselsonlevel[i].meshHolder.gameObject.SetActive(true);
				if (GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipType != "BIOLOGIC")
				{
					GameDataManager.enemyvesselsonlevel[i].vesselmovement.wakeObject.SetActive(true);
				}
			}
		}
		else if (GameDataManager.enemyvesselsonlevel[i].meshHolder.gameObject.activeSelf)
		{
			GameDataManager.enemyvesselsonlevel[i].meshHolder.gameObject.SetActive(false);
			GameDataManager.enemyvesselsonlevel[i].vesselmovement.wakeObject.SetActive(false);
			if (ManualCameraZoom.target == GameDataManager.enemyvesselsonlevel[i].transform)
			{
				ManualCameraZoom.target = GameDataManager.playervesselsonlevel[0].gameObject.transform;
				ManualCameraZoom.minDistance = GameDataManager.playervesselsonlevel[0].vesselmovement.cameraDistance;
				if (ManualCameraZoom.distance < GameDataManager.playervesselsonlevel[0].vesselmovement.cameraDistance)
				{
					ManualCameraZoom.distance = GameDataManager.playervesselsonlevel[0].vesselmovement.cameraDistance;
				}
			}
		}
	}

	// Token: 0x06000A08 RID: 2568 RVA: 0x0007AEF4 File Offset: 0x000790F4
	public void CalculateRangeError(int i)
	{
		float num = UnityEngine.Random.Range(1f, 4f);
		num *= 1f - this.solutionQualityOfContacts[i] / 100f;
		if (UnityEngine.Random.value < 0.5f)
		{
			num = (num - 5.285f) / -4.285f;
		}
		this.solutionRangeErrors[i] = num;
	}

	// Token: 0x06000A09 RID: 2569 RVA: 0x0007AF50 File Offset: 0x00079150
	public bool CheckIfIsInVerticalBaffles(Vessel listeningVessel, Transform noiseLocation)
	{
		listeningVessel.acoustics.sensorNavigator.transform.LookAt(noiseLocation.position);
		float num = listeningVessel.acoustics.sensorNavigator.transform.localEulerAngles.x;
		if (num > 180f)
		{
			num = -360f + num;
		}
		num = Mathf.Abs(num);
		return num > 45f;
	}

	// Token: 0x06000A0A RID: 2570 RVA: 0x0007AFC0 File Offset: 0x000791C0
	public bool CheckIfIsInBaffles(Vessel listeningVessel, Transform noiseLocation)
	{
		if (listeningVessel.databaseshipdata.towedSonarID > -1 && listeningVessel.vesselmovement.shipSpeed.z > 0f && listeningVessel.vesselmovement.shipSpeed.z < 1f)
		{
			if (!listeningVessel.playercontrolled)
			{
				return false;
			}
			if (this.playerfunctions.damagecontrol.CheckSubsystem("TOWED", false))
			{
				return false;
			}
		}
		float num = UIFunctions.globaluifunctions.GetBearingToTransform(listeningVessel.transform, noiseLocation);
		if (num < 0f)
		{
			num *= -1f;
		}
		return num > 150f;
	}

	// Token: 0x06000A0B RID: 2571 RVA: 0x0007B074 File Offset: 0x00079274
	public float CheckCanHearTransient(Vessel listeningVessel, Vessel firingVessel)
	{
		if (listeningVessel.databaseshipdata.passiveSonarID == -1)
		{
			return 0f;
		}
		if (!listeningVessel.playercontrolled)
		{
			if (firingVessel.databaseshipdata.passiveSonarID != -1)
			{
				float num = this.sensorLastPassiveReading[listeningVessel.vesselListIndex] + this.noiseFromTransient;
				if (num > 10f)
				{
					if (this.CheckIfIsInBaffles(listeningVessel, firingVessel.transform) || this.CheckIfIsInVerticalBaffles(listeningVessel, firingVessel.transform))
					{
						return 0f;
					}
					return num;
				}
			}
			return 0f;
		}
		float result = this.signalLastPassiveReading[firingVessel.vesselListIndex] + this.noiseFromTransient;
		if (this.signalLastPassiveReading[firingVessel.vesselListIndex] + this.noiseFromTransient <= 10f)
		{
			return 0f;
		}
		if (this.CheckIfIsInBaffles(listeningVessel, firingVessel.transform) || this.CheckIfIsInVerticalBaffles(listeningVessel, firingVessel.transform))
		{
			return 0f;
		}
		return result;
	}

	// Token: 0x06000A0C RID: 2572 RVA: 0x0007B16C File Offset: 0x0007936C
	private float GetActiveSonarNoise(float d, float spreadingLoss, float vesselOwnNoise, float layerBonus, float convergenceBonus, float angle, float targetStrength, float sonarStrength, float attenuation)
	{
		if (angle > 90f)
		{
			angle = 90f - (angle - 90f);
		}
		float num = angle / 90f * 0.4f + 0.6f;
		targetStrength *= num;
		return sonarStrength - 2f * (spreadingLoss + attenuation) + targetStrength - vesselOwnNoise + layerBonus + convergenceBonus;
	}

	// Token: 0x04000F18 RID: 3864
	public global::Environment environment;

	// Token: 0x04000F19 RID: 3865
	public PlayerFunctions playerfunctions;

	// Token: 0x04000F1A RID: 3866
	public TacticalMap tacticalmap;

	// Token: 0x04000F1B RID: 3867
	public ConditionsDisplay conditionsdisplay;

	// Token: 0x04000F1C RID: 3868
	public bool Debugging;

	// Token: 0x04000F1D RID: 3869
	public float maxSurfaceDuctBonus;

	// Token: 0x04000F1E RID: 3870
	public float maxLayerBonus;

	// Token: 0x04000F1F RID: 3871
	public float maxConvergenceBonus;

	// Token: 0x04000F20 RID: 3872
	public float maxBottomBounceBonus;

	// Token: 0x04000F21 RID: 3873
	public float attenuationFactor;

	// Token: 0x04000F22 RID: 3874
	public float currentOceanAmbientNoise;

	// Token: 0x04000F23 RID: 3875
	public float oceanAmbientBaseNoise;

	// Token: 0x04000F24 RID: 3876
	public float noisePerSeaState;

	// Token: 0x04000F25 RID: 3877
	public float maxNoiseFromRain;

	// Token: 0x04000F26 RID: 3878
	public float noiseFromCavitation;

	// Token: 0x04000F27 RID: 3879
	public float noiseFromTransient;

	// Token: 0x04000F28 RID: 3880
	public float targetNoisePerKnot;

	// Token: 0x04000F29 RID: 3881
	public float spreadingFactor;

	// Token: 0x04000F2A RID: 3882
	public Vector2 convergenceRange;

	// Token: 0x04000F2B RID: 3883
	public Vector2 bottomBounceRange;

	// Token: 0x04000F2C RID: 3884
	public Vector2 bottomBounceDepthRange;

	// Token: 0x04000F2D RID: 3885
	public float bottomBounceCurrentDepth;

	// Token: 0x04000F2E RID: 3886
	public string[] strengthTypes;

	// Token: 0x04000F2F RID: 3887
	public string[] timesOfDay;

	// Token: 0x04000F30 RID: 3888
	public int currentWeather;

	// Token: 0x04000F31 RID: 3889
	public string[] weather;

	// Token: 0x04000F32 RID: 3890
	public float[] weatherProbabilities;

	// Token: 0x04000F33 RID: 3891
	public float weatherSeasonModifier;

	// Token: 0x04000F34 RID: 3892
	public string[] seaStates;

	// Token: 0x04000F35 RID: 3893
	public float[] seaStateProbabilities;

	// Token: 0x04000F36 RID: 3894
	public float seaStateSeasonModifier;

	// Token: 0x04000F37 RID: 3895
	public float passiveCompressionFactor;

	// Token: 0x04000F38 RID: 3896
	public float passiveBaselineFactor;

	// Token: 0x04000F39 RID: 3897
	public float playerSonarDamagedPenalty = 6f;

	// Token: 0x04000F3A RID: 3898
	public Vector2 landAbsorptionFold;

	// Token: 0x04000F3B RID: 3899
	public float maxShallowsAbsorption;

	// Token: 0x04000F3C RID: 3900
	public Vector2 detectionThresholds;

	// Token: 0x04000F3D RID: 3901
	public float convergenceZoneDistanceMetres;

	// Token: 0x04000F3E RID: 3902
	public bool usingBottomBounce;

	// Token: 0x04000F3F RID: 3903
	public float layerStrength;

	// Token: 0x04000F40 RID: 3904
	public float surfaceDuctStrength;

	// Token: 0x04000F41 RID: 3905
	public float convergenceZoneStrength;

	// Token: 0x04000F42 RID: 3906
	public float seaState;

	// Token: 0x04000F43 RID: 3907
	public float layerDepthInFeet;

	// Token: 0x04000F44 RID: 3908
	public float oceanFloorDepthInFeet;

	// Token: 0x04000F45 RID: 3909
	public float layerDepth;

	// Token: 0x04000F46 RID: 3910
	public float oceanFloorDepth;

	// Token: 0x04000F47 RID: 3911
	public float layerAngle;

	// Token: 0x04000F48 RID: 3912
	public Camera hudCamera;

	// Token: 0x04000F49 RID: 3913
	public GameObject targetmarker;

	// Token: 0x04000F4A RID: 3914
	public SonarContact[] sonarContacts;

	// Token: 0x04000F4B RID: 3915
	public SonarContact playerSonarContact;

	// Token: 0x04000F4C RID: 3916
	public int[] shipTypes;

	// Token: 0x04000F4D RID: 3917
	public bool[] initialDetectedByPlayer;

	// Token: 0x04000F4E RID: 3918
	public bool[] detectedByPlayer;

	// Token: 0x04000F4F RID: 3919
	public bool[] identifiedByPlayer;

	// Token: 0x04000F50 RID: 3920
	public bool[] classifiedByPlayer;

	// Token: 0x04000F51 RID: 3921
	public int[] classifiedByPlayerAsClass;

	// Token: 0x04000F52 RID: 3922
	public string[] initialDetectedByPlayerName;

	// Token: 0x04000F53 RID: 3923
	public float[] bearingToContacts;

	// Token: 0x04000F54 RID: 3924
	public float[] signalFromContacts;

	// Token: 0x04000F55 RID: 3925
	public float[] sensorOfContacts;

	// Token: 0x04000F56 RID: 3926
	public float[] signalLastPassiveReading;

	// Token: 0x04000F57 RID: 3927
	public float[] sensorLastPassiveReading;

	// Token: 0x04000F58 RID: 3928
	public string[] signalFromContactsType;

	// Token: 0x04000F59 RID: 3929
	public string[] sensorOfContactsType;

	// Token: 0x04000F5A RID: 3930
	public float[] headingOfContacts;

	// Token: 0x04000F5B RID: 3931
	public float[] speedOfContacts;

	// Token: 0x04000F5C RID: 3932
	public float[] rangeToContacts;

	// Token: 0x04000F5D RID: 3933
	public float[] solutionQualityOfContacts;

	// Token: 0x04000F5E RID: 3934
	public float[] timeTrackingContact;

	// Token: 0x04000F5F RID: 3935
	public float[] solutionRangeErrors;

	// Token: 0x04000F60 RID: 3936
	public bool playerTransient;

	// Token: 0x04000F61 RID: 3937
	public int numberOfSonarContacts;

	// Token: 0x04000F62 RID: 3938
	public int numberOfRADARContacts;

	// Token: 0x04000F63 RID: 3939
	public int cameraCurrentTargetIndex;

	// Token: 0x04000F64 RID: 3940
	public Vector3[] enemyPositions;

	// Token: 0x04000F65 RID: 3941
	public GameObject sonarPaintIconObject;

	// Token: 0x04000F66 RID: 3942
	public Sprite[] sonarPaintImages;

	// Token: 0x04000F67 RID: 3943
	public Torpedo[] torpedoObjects;

	// Token: 0x04000F68 RID: 3944
	public float torpedoDetectionRange;

	// Token: 0x04000F69 RID: 3945
	public float torpedoActiveDetectionRange;

	// Token: 0x04000F6A RID: 3946
	public Noisemaker[] noisemakerObjects;

	// Token: 0x04000F6B RID: 3947
	public int knuckleID;

	// Token: 0x04000F6C RID: 3948
	public static bool playerKnuckle;

	// Token: 0x04000F6D RID: 3949
	public float timer;

	// Token: 0x04000F6E RID: 3950
	public float surfaceRadarCheckTimer;

	// Token: 0x04000F6F RID: 3951
	public float airRadarCheckTimer;

	// Token: 0x04000F70 RID: 3952
	public bool initialRun;

	// Token: 0x04000F71 RID: 3953
	public float strongestRadarSignal;

	// Token: 0x04000F72 RID: 3954
	public List<Torpedo> sonosInFlight;

	// Token: 0x04000F73 RID: 3955
	public Vector2 runningSilentBonus;

	// Token: 0x04000F74 RID: 3956
	public LayerMask maskingLayers;

	// Token: 0x04000F75 RID: 3957
	public float highFrequencyNavSonarRange;

	// Token: 0x04000F76 RID: 3958
	public float actualHighFrequencyNavSonarRange;

	// Token: 0x04000F77 RID: 3959
	public LayerMask navSonarMask;

	// Token: 0x04000F78 RID: 3960
	public float visualRange;

	// Token: 0x04000F79 RID: 3961
	public float madDetectionRange;

	// Token: 0x04000F7A RID: 3962
	public bool truthIsOutThere;

	// Token: 0x04000F7B RID: 3963
	public float[] noiseThresholds;

	// Token: 0x04000F7C RID: 3964
	public bool addTMA;
}
