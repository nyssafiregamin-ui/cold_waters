using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000173 RID: 371
public class WeaponSource : MonoBehaviour
{
	// Token: 0x06000B45 RID: 2885 RVA: 0x000A5D6C File Offset: 0x000A3F6C
	public void InitialiseWeaponSource()
	{
		this.torpedoNames = this.parentVesselMovement.parentVessel.databaseshipdata.torpedotypes;
		this.torpedoTypes = this.parentVesselMovement.parentVessel.databaseshipdata.torpedoIDs;
		this.currentTorpsOnBoard = new int[this.parentVesselMovement.parentVessel.databaseshipdata.torpedoNumbers.Length];
		for (int i = 0; i < this.currentTorpsOnBoard.Length; i++)
		{
			this.currentTorpsOnBoard[i] = this.parentVesselMovement.parentVessel.databaseshipdata.torpedoNumbers[i];
		}
		if (this.parentVesselMovement.parentVessel.databaseshipdata.vlsTorpedotypes != null)
		{
			this.vlsTorpedoNames = this.parentVesselMovement.parentVessel.databaseshipdata.vlsTorpedotypes;
			this.vlsTorpedoTypes = this.parentVesselMovement.parentVessel.databaseshipdata.vlsTorpedoIDs;
			this.vlsCurrentTorpsOnBoard = new int[this.parentVesselMovement.parentVessel.databaseshipdata.vlsTorpedoNumbers.Length];
			for (int j = 0; j < this.vlsCurrentTorpsOnBoard.Length; j++)
			{
				this.vlsCurrentTorpsOnBoard[j] = this.parentVesselMovement.parentVessel.databaseshipdata.vlsTorpedoNumbers[j];
			}
			this.hasVLS = true;
		}
		this.tubeParticleSystems = new ParticleSystem[this.tubeParticleEffects.Length];
		for (int k = 0; k < this.torpedoTubes.Length; k++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.torpedoLaunch[0], this.torpedoTubes[k].position, this.torpedoTubes[k].rotation) as GameObject;
			gameObject.transform.SetParent(this.parentVesselMovement.parentVessel.meshHolder);
			gameObject.transform.localPosition = this.tubeParticleEffects[k];
			gameObject.transform.localRotation = Quaternion.identity;
			this.tubeParticleSystems[k] = gameObject.GetComponent<ParticleSystem>();
		}
		this.tubeStatus = new int[this.parentVesselMovement.parentVessel.databaseshipdata.torpedotubes];
		this.weaponInTube = new int[this.parentVesselMovement.parentVessel.databaseshipdata.torpedotubes];
		this.wantedWeaponInTube = new int[this.parentVesselMovement.parentVessel.databaseshipdata.torpedotubes];
		this.originalLoadedWeaponInTube = new int[this.parentVesselMovement.parentVessel.databaseshipdata.torpedotubes];
		for (int l = 0; l < this.originalLoadedWeaponInTube.Length; l++)
		{
			this.originalLoadedWeaponInTube[l] = -1;
		}
		this.originalLoadedWeaponInTubeSet = new bool[this.parentVesselMovement.parentVessel.databaseshipdata.torpedotubes];
		this.originalLoadedWeaponTimer = new float[this.parentVesselMovement.parentVessel.databaseshipdata.torpedotubes];
		this.lastTubeLoading = -1;
		this.torpedoSearchPattern = new int[this.parentVesselMovement.parentVessel.databaseshipdata.torpedotubes];
		this.torpedoDepthPattern = new int[this.parentVesselMovement.parentVessel.databaseshipdata.torpedotubes];
		this.torpedoHomingPattern = new int[this.parentVesselMovement.parentVessel.databaseshipdata.torpedotubes];
		this.tubeReloadingTimer = new float[this.parentVesselMovement.parentVessel.databaseshipdata.torpedotubes];
		this.tubeReloadingDirection = new float[this.parentVesselMovement.parentVessel.databaseshipdata.torpedotubes];
		this.torpedoesOnWire = new Torpedo[this.parentVesselMovement.parentVessel.databaseshipdata.torpedotubes];
		this.noisemakersOnBoard = this.parentVesselMovement.parentVessel.databaseshipdata.numberofnoisemakers;
		this.noisemakerReloadTime = this.parentVesselMovement.parentVessel.databaseshipdata.noisemakerreloadtime * OptionsManager.difficultySettings["PlayerWeaponReloadTime"];
		this.tubeReloadTime = this.parentVesselMovement.parentVessel.databaseshipdata.tubereloadtime * OptionsManager.difficultySettings["PlayerWeaponReloadTime"];
		UIFunctions.globaluifunctions.playerfunctions.numberOfWires = this.parentVesselMovement.parentVessel.databaseshipdata.numberOfWires;
		UIFunctions.globaluifunctions.playerfunctions.numberOfWiresUsed = 0;
		UIFunctions.globaluifunctions.portRearm.SetPlayerNumberOfWires();
		UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentCurrentFlooding = new float[5];
		UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentTotalFlooding = new float[5];
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x000A6208 File Offset: 0x000A4408
	public void DropNoisemaker()
	{
		if (this.noisemakersOnBoard > 0 && !this.noisemakerReloading)
		{
			this.noisemakersOnBoard--;
			this.noisemakerReloading = true;
			UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[13].color = UIFunctions.globaluifunctions.playerfunctions.helmmanager.buttonColors[0];
			Noisemaker component = ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.databasecountermeasuredata[this.parentVesselMovement.parentVessel.databaseshipdata.noiseMakerID].countermeasureObject, this.noisemakerTubes.position, this.noisemakerTubes.rotation).GetComponent<Noisemaker>();
			component.gameObject.SetActive(true);
			component.databasecountermeasuredata = UIFunctions.globaluifunctions.database.databasecountermeasuredata[this.parentVesselMovement.parentVessel.databaseshipdata.noiseMakerID];
			component.tacMapNoisemakerIcon.shipDisplayIcon.color = UIFunctions.globaluifunctions.levelloadmanager.tacticalmap.navyColors[0];
			UIFunctions.globaluifunctions.playerfunctions.sensormanager.AddNoiseMakerToArray(component);
			if (this.parentVesselMovement.parentVessel.uifunctions.playerfunctions.storesPanel.activeSelf)
			{
				UIFunctions.globaluifunctions.portRearm.SetLoadoutStats();
			}
			UIFunctions.globaluifunctions.portRearm.noisemakerHelmPanelText.text = this.parentVesselMovement.weaponSource.noisemakersOnBoard.ToString();
		}
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x000A63A4 File Offset: 0x000A45A4
	public void SetWeaponWaypointData(Torpedo torpedo)
	{
		if (!UIFunctions.globaluifunctions.playerfunctions.wireData[0].gameObject.activeSelf)
		{
			UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.waypointMarker.gameObject.SetActive(false);
		}
		torpedo.launchPosition = base.transform.position;
		torpedo.distanceToWaypoint = Vector3.Distance(torpedo.transform.position, UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.waypointWorldMarker.transform.position);
		torpedo.initialWaypointPosition = UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.waypointWorldMarker.transform.position;
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x000A6468 File Offset: 0x000A4668
	public void FireTube()
	{
		Vessel vessel = GameDataManager.playervesselsonlevel[0];
		if (this.tubeStatus[this.activeTube] >= 0)
		{
			if (!this.hasVLS || this.activeTube != this.torpedoTubes.Length - 1)
			{
				if ((this.torpedoTubes[this.activeTube].localEulerAngles.y > 5f || this.torpedoTubes[this.activeTube].localEulerAngles.y < 355f) && this.parentVesselMovement.shipSpeed.z > 2f && UnityEngine.Random.Range(1f, 8f) < this.parentVesselMovement.shipSpeed.z)
				{
					this.currentTorpsOnBoard[this.tubeStatus[this.activeTube]]--;
					this.tubeStatus[this.activeTube] = -200;
					this.weaponInTube[this.activeTube] = -200;
					UIFunctions.globaluifunctions.playerfunctions.ClearTubeSettingButtons(this.activeTube);
					this.tubeReloadingDirection[this.activeTube] = 0f;
					UIFunctions.globaluifunctions.playerfunctions.torpedoTubeImages[this.activeTube].transform.localPosition = new Vector3(0f, -0.5f, 0f);
					UIFunctions.globaluifunctions.playerfunctions.torpedoTubeImages[this.activeTube].gameObject.SetActive(true);
					UIFunctions.globaluifunctions.playerfunctions.torpedoTubeImages[this.activeTube].sprite = UIFunctions.globaluifunctions.playerfunctions.tubeDestroyedSprite;
					string text = LanguageManager.messageLogDictionary["TubeJamDestroyed"];
					text = text.Replace("<TUBE>", (this.activeTube + 1).ToString());
					UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["TorpedoRoom"], "TubeJamDestroyed", false);
					if (UIFunctions.globaluifunctions.playerfunctions.storesPanel.activeSelf)
					{
						UIFunctions.globaluifunctions.portRearm.SetLoadoutStats();
					}
					UIFunctions.globaluifunctions.playerfunctions.DisableWaypointDragging();
					UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlCurrentTimers[DamageControl.GetSubsystemIndex("TUBES")] = 10000f;
					return;
				}
			}
			GameObject original;
			if (this.hasVLS && this.activeTube == this.torpedoTubes.Length - 1)
			{
				original = this.parentVesselMovement.parentVessel.databaseshipdata.vlsTorpedoGameObjects[this.tubeStatus[this.activeTube]];
				this.vlsCurrentTorpsOnBoard[this.tubeStatus[this.activeTube]]--;
				if (this.vlsCurrentTorpsOnBoard[this.tubeStatus[this.activeTube]] < 0)
				{
					this.vlsCurrentTorpsOnBoard[this.tubeStatus[this.activeTube]] = 0;
				}
			}
			else
			{
				original = this.parentVesselMovement.parentVessel.databaseshipdata.torpedoGameObjects[this.tubeStatus[this.activeTube]];
				this.currentTorpsOnBoard[this.tubeStatus[this.activeTube]]--;
				if (this.currentTorpsOnBoard[this.tubeStatus[this.activeTube]] < 0)
				{
					this.currentTorpsOnBoard[this.tubeStatus[this.activeTube]] = 0;
				}
			}
			if (UIFunctions.globaluifunctions.playerfunctions.storesPanel.activeSelf)
			{
				UIFunctions.globaluifunctions.portRearm.SetLoadoutStats();
			}
			this.originalLoadedWeaponInTube[this.activeTube] = -1;
			GameObject gameObject = UnityEngine.Object.Instantiate(original, this.torpedoTubes[this.activeTube].position, this.torpedoTubes[this.activeTube].rotation) as GameObject;
			gameObject.SetActive(true);
			Torpedo component = gameObject.GetComponent<Torpedo>();
			component.databaseweapondata = UIFunctions.globaluifunctions.database.databaseweapondata[UIFunctions.globaluifunctions.playerfunctions.GetPlayerTorpedoIDInTube(this.activeTube)];
			this.torpedoesOnWire[this.activeTube] = component;
			component.tubefiredFrom = this.activeTube;
			component.vesselFiredFrom = vessel;
			component.whichNavy = 0;
			if (!component.databaseweapondata.swimOut)
			{
				this.parentVesselMovement.parentVessel.uifunctions.playerfunctions.sensormanager.playerTransient = true;
				UIFunctions.globaluifunctions.combatai.CheckPlayerTransient();
			}
			UIFunctions.globaluifunctions.playerfunctions.eventcamera.CheckForEventCamera(gameObject.transform, GameDataManager.playervesselsonlevel[0].transform, 6f, false, false, false, component.databaseweapondata.minCameraDistance + 1f, component.databaseweapondata.minCameraDistance, UnityEngine.Random.Range(25f, 60f), false);
			string text2 = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "FireTube");
			text2 = text2.Replace("<WEAPON>", component.databaseweapondata.weaponName);
			text2 = text2.Replace("<TUBE>", (this.activeTube + 1).ToString());
			UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentTubeNumber = (this.activeTube + 1).ToString();
			UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text2, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["FireControl"], "FireTube", false);
			this.tubeParticleSystems[this.activeTube].Play();
			this.tubeParticleSystems[this.activeTube].GetComponent<AudioSource>().Play();
			component.InitialiseTorpedo();
			this.parentVesselMovement.parentVessel.uifunctions.playerfunctions.sensormanager.AddTorpedoToArray(component);
			this.SetWeaponWaypointData(component);
			if (component.databaseweapondata.isMissile)
			{
				if (component.databaseweapondata.landAttack)
				{
					component.CheckLandStrikeNumber();
				}
				component.onWire = false;
				UIFunctions.globaluifunctions.playerfunctions.ClearTubeSettingButtons(this.activeTube);
				UIFunctions.globaluifunctions.playerfunctions.torpedoTubeImages[this.activeTube].gameObject.SetActive(false);
				vessel.vesselmovement.weaponSource.weaponInTube[this.activeTube] = -10;
				this.tubeStatus[this.activeTube] = -1;
				if (component.distanceToWaypoint < 33f)
				{
					component.distanceToWaypoint = 33f;
				}
				if (this.torpedoDepthPattern[this.activeTube] >= 0)
				{
					string text3 = UIFunctions.globaluifunctions.playerfunctions.depthSettingDefinitions[this.torpedoDepthPattern[this.activeTube]];
					string text4 = text3;
					if (text4 != null)
					{
						if (WeaponSource.<>f__switch$map4C == null)
						{
							WeaponSource.<>f__switch$map4C = new Dictionary<string, int>(2)
							{
								{
									"POP-UP",
									0
								},
								{
									"SKIM",
									1
								}
							};
						}
						int num;
						if (WeaponSource.<>f__switch$map4C.TryGetValue(text4, out num))
						{
							if (num != 0)
							{
								if (num == 1)
								{
									component.runLow = true;
								}
							}
							else
							{
								component.runLow = false;
							}
						}
					}
				}
				if (this.torpedoSearchPattern[this.activeTube] >= 0)
				{
					string text5 = UIFunctions.globaluifunctions.playerfunctions.attackSettingDefinitions[this.torpedoSearchPattern[this.activeTube]];
					string text4 = text5;
					if (text4 != null)
					{
						if (WeaponSource.<>f__switch$map4D == null)
						{
							WeaponSource.<>f__switch$map4D = new Dictionary<string, int>(2)
							{
								{
									"NARROW",
									0
								},
								{
									"WIDE",
									1
								}
							};
						}
						int num;
						if (WeaponSource.<>f__switch$map4D.TryGetValue(text4, out num))
						{
							if (num != 0)
							{
								if (num == 1)
								{
									component.narrowCone = false;
								}
							}
							else
							{
								component.narrowCone = true;
							}
						}
					}
				}
				return;
			}
			if (component.databaseweapondata.sensorRange > 0f)
			{
				component.CalculateSensorCone();
			}
			if (component.databaseweapondata.wireGuided)
			{
				component.onWire = true;
				vessel.vesselmovement.weaponSource.weaponInTube[vessel.vesselmovement.weaponSource.activeTube] = -100;
				component.wireLaunchAngle = GameDataManager.playervesselsonlevel[0].transform.eulerAngles.y;
				UIFunctions.globaluifunctions.playerfunctions.numberOfWiresUsed++;
				UIFunctions.globaluifunctions.portRearm.SetPlayerNumberOfWires();
				vessel.uifunctions.playerfunctions.currentActiveTorpedo = component;
			}
			else
			{
				component.onWire = false;
				vessel.vesselmovement.weaponSource.weaponInTube[vessel.vesselmovement.weaponSource.activeTube] = -10;
			}
			if (component.databaseweapondata.isDumbfire)
			{
				component.tacMapTorpedoIcon.shipDisplayIcon.color = UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.torpedoColors[3];
			}
			float num2 = component.databaseweapondata.wireBreakOnLaunchProbability;
			if (GameDataManager.trainingMode)
			{
				num2 = -2f;
			}
			if ((UnityEngine.Random.value < num2 || !component.databaseweapondata.wireGuided || UIFunctions.globaluifunctions.playerfunctions.numberOfWiresUsed > UIFunctions.globaluifunctions.playerfunctions.numberOfWires) && !GameDataManager.trainingMode)
			{
				vessel.uifunctions.playerfunctions.CutWire(vessel.vesselmovement.weaponSource.activeTube);
				if (component.databaseweapondata.wireGuided && UIFunctions.globaluifunctions.playerfunctions.numberOfWiresUsed <= UIFunctions.globaluifunctions.playerfunctions.numberOfWires)
				{
					text2 = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "WireBreak");
					text2 = text2.Replace("<WEAPON>", component.databaseweapondata.weaponName);
					UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text2, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["FireControl"], "WireBreak", false);
				}
				vessel.vesselmovement.weaponSource.weaponInTube[vessel.vesselmovement.weaponSource.activeTube] = -10;
				component.onWire = false;
				component.tacMapTorpedoIcon.sensorConeLines[0].gameObject.SetActive(false);
				this.torpedoesOnWire[this.activeTube] = null;
				component.tacMapTorpedoIcon.contactText.gameObject.SetActive(false);
			}
			else
			{
				component.tacMapTorpedoIcon.contactText.text = (component.tubefiredFrom + 1).ToString();
				component.tacMapTorpedoIcon.contactText.transform.localScale = new Vector3(UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.textMinMaxSize, UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.textMinMaxSize, UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.textMinMaxSize);
				component.tacMapTorpedoIcon.contactText.transform.localPosition = new Vector3(0f, UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.textYOffset * UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.orthFactor, 0f);
				component.tacMapTorpedoIcon.contactText.gameObject.SetActive(true);
			}
			if (component.onWire)
			{
				UIFunctions.globaluifunctions.playerfunctions.torpedoTubeImages[this.activeTube].sprite = UIFunctions.globaluifunctions.playerfunctions.wireSprite;
			}
			else
			{
				UIFunctions.globaluifunctions.playerfunctions.torpedoTubeImages[this.activeTube].gameObject.SetActive(false);
				UIFunctions.globaluifunctions.playerfunctions.ClearTubeSettingButtons(this.activeTube);
			}
			string text6 = string.Empty;
			if (this.torpedoSearchPattern[this.activeTube] >= 0)
			{
				text6 = UIFunctions.globaluifunctions.playerfunctions.attackSettingDefinitions[this.torpedoSearchPattern[this.activeTube]];
				string text4 = text6;
				switch (text4)
				{
				case "STRAIGHT":
					component.searchLeft = false;
					component.runStraight = true;
					break;
				case "SNAKE":
					component.searchLeft = false;
					component.snakeSearch = true;
					break;
				case "LEFT":
					component.searchLeft = true;
					component.snakeSearch = false;
					break;
				case "RIGHT":
					component.searchLeft = false;
					component.snakeSearch = false;
					break;
				}
			}
			component.cruiseYValue = vessel.transform.position.y;
			if (this.torpedoDepthPattern[this.activeTube] >= 0)
			{
				text6 = UIFunctions.globaluifunctions.playerfunctions.depthSettingDefinitions[this.torpedoDepthPattern[this.activeTube]];
				string text4 = text6;
				switch (text4)
				{
				case "SHALLOW":
					component.searchYValue = this.GetShallowRunDepth();
					break;
				case "LEVEL":
					component.searchYValue = component.transform.position.y;
					break;
				case "DEEP":
					component.searchYValue = this.GetDeepRunDepth();
					break;
				}
			}
			if (this.torpedoHomingPattern[this.activeTube] >= 0)
			{
				text6 = UIFunctions.globaluifunctions.playerfunctions.homeSettingDefinitions[this.torpedoHomingPattern[this.activeTube]];
				string text4 = text6;
				if (text4 != null)
				{
					if (WeaponSource.<>f__switch$map4B == null)
					{
						WeaponSource.<>f__switch$map4B = new Dictionary<string, int>(2)
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
					int num;
					if (WeaponSource.<>f__switch$map4B.TryGetValue(text4, out num))
					{
						if (num != 0)
						{
							if (num == 1)
							{
								component.passiveHoming = true;
							}
						}
					}
				}
			}
		}
		this.tubeStatus[this.activeTube] = -1;
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x000A7344 File Offset: 0x000A5544
	public float GetDeepRunDepth()
	{
		if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.layerDepth > 0f)
		{
			return UIFunctions.globaluifunctions.playerfunctions.sensormanager.layerDepth - 0.132f;
		}
		return 998.446f;
	}

	// Token: 0x06000B4A RID: 2890 RVA: 0x000A7390 File Offset: 0x000A5590
	public float GetShallowRunDepth()
	{
		return 999.868f;
	}

	// Token: 0x06000B4B RID: 2891 RVA: 0x000A7398 File Offset: 0x000A5598
	public void FixedUpdate()
	{
		if (PlayerFunctions.runningSilent)
		{
			return;
		}
		if (this.noisemakerReloading)
		{
			this.noisemakerReloadingTimer += Time.deltaTime;
			if (this.noisemakerReloadingTimer > this.noisemakerReloadTime)
			{
				this.noisemakerReloadingTimer = 0f;
				this.noisemakerReloading = false;
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[13].color = UIFunctions.globaluifunctions.playerfunctions.helmmanager.buttonColors[1];
			}
		}
		if (this.weaponDelayTimer > 0f)
		{
			this.weaponDelayTimer -= Time.deltaTime;
			return;
		}
		float num = 0f;
		int num2 = -1;
		for (int i = 0; i < this.torpedoTubes.Length; i++)
		{
			if (this.tubeReloadingDirection[i] > 0f && this.tubeReloadingTimer[i] > num)
			{
				num = this.tubeReloadingTimer[i];
				num2 = i;
			}
		}
		if (num2 != -1)
		{
			this.tubeReloadingTimer[num2] += Time.deltaTime;
			this.parentVesselMovement.parentVessel.uifunctions.playerfunctions.torpedoTubeImages[num2].color = Color.white;
			if (this.originalLoadedWeaponInTubeSet[num2])
			{
				if (this.originalLoadedWeaponInTube[num2] == this.tubeStatus[num2])
				{
					if (this.originalLoadedWeaponTimer[num2] == 0f)
					{
						this.tubeReloadingTimer[num2] = this.tubeReloadTime + 1f;
					}
					else
					{
						this.tubeReloadingTimer[num2] = this.originalLoadedWeaponTimer[num2];
					}
				}
				this.originalLoadedWeaponInTubeSet[num2] = false;
			}
			if (this.tubeReloadingTimer[num2] > this.tubeReloadTime)
			{
				this.tubeReloadingTimer[num2] = 0f;
				this.tubeReloadingDirection[num2] = 0f;
				this.weaponInTube[num2] = this.tubeStatus[num2];
				int playerTorpedoIDInTube = UIFunctions.globaluifunctions.playerfunctions.GetPlayerTorpedoIDInTube(num2);
				this.torpedoSearchPattern[num2] = this.parentVesselMovement.parentVessel.uifunctions.playerfunctions.GetSettingIndex(UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTube].searchSettings[0], this.parentVesselMovement.parentVessel.uifunctions.playerfunctions.attackSettingDefinitions);
				this.torpedoDepthPattern[num2] = this.parentVesselMovement.parentVessel.uifunctions.playerfunctions.GetSettingIndex(UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTube].heightSettings[0], this.parentVesselMovement.parentVessel.uifunctions.playerfunctions.depthSettingDefinitions);
				this.torpedoHomingPattern[num2] = this.parentVesselMovement.parentVessel.uifunctions.playerfunctions.GetSettingIndex(UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTube].homeSettings[0], this.parentVesselMovement.parentVessel.uifunctions.playerfunctions.homeSettingDefinitions);
				this.parentVesselMovement.parentVessel.uifunctions.playerfunctions.SetTubeSettingButtons(num2);
				if (GameDataManager.playervesselsonlevel[0].uifunctions.playerfunctions.tubeData[0].gameObject.activeSelf)
				{
					this.parentVesselMovement.parentVessel.uifunctions.playerfunctions.SetTubesData();
				}
				string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "WeaponLoaded");
				text = text.Replace("<WEAPON>", this.torpedoNames[this.weaponInTube[num2]]);
				text = text.Replace("<TUBE>", (num2 + 1).ToString());
				UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentTubeNumber = (num2 + 1).ToString();
				UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["TorpedoRoom"], "WeaponLoaded", false);
				return;
			}
			if (this.tubeReloadTime > 0f)
			{
				float x = (1f - this.tubeReloadingTimer[num2] / this.tubeReloadTime) * -152f;
				this.parentVesselMovement.parentVessel.uifunctions.playerfunctions.torpedoTubeImages[num2].transform.localPosition = new Vector3(x, -0.5f, 0f);
			}
			else
			{
				this.parentVesselMovement.parentVessel.uifunctions.playerfunctions.torpedoTubeImages[num2].transform.localPosition = new Vector3(0f, -0.5f, 0f);
			}
		}
	}

	// Token: 0x04001239 RID: 4665
	public VesselMovement parentVesselMovement;

	// Token: 0x0400123A RID: 4666
	public int activeTube;

	// Token: 0x0400123B RID: 4667
	public int[] torpedoTypes;

	// Token: 0x0400123C RID: 4668
	public string[] torpedoNames;

	// Token: 0x0400123D RID: 4669
	public int[] currentTorpsOnBoard;

	// Token: 0x0400123E RID: 4670
	public bool hasVLS;

	// Token: 0x0400123F RID: 4671
	public int[] vlsTorpedoTypes;

	// Token: 0x04001240 RID: 4672
	public string[] vlsTorpedoNames;

	// Token: 0x04001241 RID: 4673
	public int[] vlsCurrentTorpsOnBoard;

	// Token: 0x04001242 RID: 4674
	public Transform[] torpedoTubes;

	// Token: 0x04001243 RID: 4675
	public int[] tubeStatus;

	// Token: 0x04001244 RID: 4676
	public int[] weaponInTube;

	// Token: 0x04001245 RID: 4677
	public int[] wantedWeaponInTube;

	// Token: 0x04001246 RID: 4678
	public bool[] originalLoadedWeaponInTubeSet;

	// Token: 0x04001247 RID: 4679
	public int[] originalLoadedWeaponInTube;

	// Token: 0x04001248 RID: 4680
	public float[] originalLoadedWeaponTimer;

	// Token: 0x04001249 RID: 4681
	public int[] torpedoSearchPattern;

	// Token: 0x0400124A RID: 4682
	public int[] torpedoDepthPattern;

	// Token: 0x0400124B RID: 4683
	public int[] torpedoHomingPattern;

	// Token: 0x0400124C RID: 4684
	public float[] tubeReloadingTimer;

	// Token: 0x0400124D RID: 4685
	public float[] tubeReloadingDirection;

	// Token: 0x0400124E RID: 4686
	public Torpedo[] torpedoesOnWire;

	// Token: 0x0400124F RID: 4687
	public Vector3[] tubeParticleEffects;

	// Token: 0x04001250 RID: 4688
	public ParticleSystem[] tubeParticleSystems;

	// Token: 0x04001251 RID: 4689
	public bool sealsOnBoard;

	// Token: 0x04001252 RID: 4690
	public int noisemakersOnBoard;

	// Token: 0x04001253 RID: 4691
	public bool noisemakerReloading;

	// Token: 0x04001254 RID: 4692
	public Transform noisemakerTubes;

	// Token: 0x04001255 RID: 4693
	public float noisemakerReloadTime;

	// Token: 0x04001256 RID: 4694
	public float noisemakerReloadingTimer;

	// Token: 0x04001257 RID: 4695
	public float weaponDelayTimer;

	// Token: 0x04001258 RID: 4696
	public float tubeReloadTime;

	// Token: 0x04001259 RID: 4697
	public int lastTubeLoading;
}
