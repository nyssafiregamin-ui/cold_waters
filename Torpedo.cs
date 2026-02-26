using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000166 RID: 358
public class Torpedo : MonoBehaviour
{
	// Token: 0x06000AAE RID: 2734 RVA: 0x00091FA8 File Offset: 0x000901A8
	private void OnTriggerEnter(Collider otherObject)
	{
		if (otherObject.name == "Terrain Chunk" || otherObject.name == "Ice")
		{
			this.destroyMe = true;
		}
	}

	// Token: 0x06000AAF RID: 2735 RVA: 0x00091FDC File Offset: 0x000901DC
	public void ShootMissileDown(bool explodeOnDown)
	{
		if (!this.shotDown)
		{
			if (explodeOnDown)
			{
				GameObject gameObject = ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.shootDownEffect, base.transform.position, base.transform.rotation);
				ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.missileShotDown[UnityEngine.Random.Range(0, UIFunctions.globaluifunctions.database.missileShotDown.Length)], base.transform.position, base.transform.rotation);
				gameObject.transform.SetParent(base.transform);
				this.shotDownParticleEffect = gameObject;
				this.shotDownParticleEffect.GetComponent<DestroyTimer>().enabled = false;
			}
			this.shotDownAngles.x = UnityEngine.Random.Range(0.05f, 0.3f);
			this.shotDownAngles.y = UnityEngine.Random.Range(-10f, 10f);
			this.shotDown = true;
			return;
		}
	}

	// Token: 0x06000AB0 RID: 2736 RVA: 0x000920D0 File Offset: 0x000902D0
	private void ScanForTerrain()
	{
		this.terrainScanTimer = 0f;
		float num = 25f;
		if (this.targetTransform != null)
		{
			num = Vector3.Distance(base.transform.position, this.targetTransform.position);
			if (num < 30f)
			{
				return;
			}
			if (num > 25f)
			{
				num = 25f;
			}
		}
		int layerMask = 1073741824;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position - Vector3.up, base.transform.forward, out raycastHit, num, layerMask) && this.cruiseYValue < 1007f)
		{
			this.cruiseYValue += 0.5f;
		}
	}

	// Token: 0x06000AB1 RID: 2737 RVA: 0x00092190 File Offset: 0x00090390
	public void InitialiseTorpedo()
	{
		this.snakeMode = 1;
		if (UnityEngine.Random.value < 0.5f)
		{
			this.snakeMode *= -1;
		}
		this.snakeTime = 9f;
		this.InitialiseParticles();
		this.passiveRange /= UIFunctions.globaluifunctions.playerfunctions.sensormanager.seaState / 4f;
		this.damageDealt = 0;
		this.playerMessageStatus = 0;
		if (this.databaseweapondata.weaponType == "TORPEDO" || this.databaseweapondata.weaponType == "DECOY")
		{
			this.tacMapTorpedoIcon = ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.torpedoMapContactObject, Vector3.zero, Quaternion.identity).GetComponent<MapContact>();
			this.tacMapTorpedoIcon.contactText.text = string.Empty;
			this.tacMapTorpedoIcon.sensorConeLines[0].gameObject.SetActive(false);
			this.tacMapTorpedoIcon.positionMarkerQueue = new Queue<Transform>();
			if (this.whichNavy == 0)
			{
				this.tacMapTorpedoIcon.shipDisplayIcon.color = UIFunctions.globaluifunctions.levelloadmanager.tacticalmap.torpedoColors[0];
			}
			else
			{
				this.tacMapTorpedoIcon.shipDisplayIcon.color = UIFunctions.globaluifunctions.levelloadmanager.tacticalmap.torpedoColors[2];
			}
			if (this.cavitationAudioSource != null && !this.isAirborne)
			{
				this.cavitationAudioSource.enabled = true;
				this.cavitationAudioSource.Play();
			}
			if (this.databaseweapondata.weaponType == "DECOY")
			{
				this.missileSnapShotAt = true;
				this.torpedoSnapShotAt = true;
				this.cruiseYValue = base.transform.position.y;
				this.searchYValue = this.cruiseYValue;
			}
			if ((float)UnityEngine.Random.Range(1, 11) < 4f + GameDataManager.optionsFloatSettings[3] * 3f)
			{
				this.missileSnapShotAt = false;
				this.torpedoSnapShotAt = false;
			}
			else
			{
				this.missileSnapShotAt = true;
				this.torpedoSnapShotAt = true;
			}
		}
		if (this.databaseweapondata.isMissile || !this.isAirborne)
		{
			this.actualCurrentSpeed = this.databaseweapondata.actualRunSpeed;
		}
		this.guidanceActive = true;
		this.sensorsActive = false;
		this.searching = false;
		this.destroyMe = false;
		this.SetTorpedoTargetsList();
		if (this.databaseweapondata.landAttack)
		{
			this.targetsList.Clear();
		}
		this.actionTimer = 7f;
		this.sortTargetsTimer = 10f;
		this.playerControlling = false;
		if (this.databaseweapondata.isMissile)
		{
			if (this.torpedoTrail != null)
			{
				this.torpedoTrail.gameObject.SetActive(true);
				this.torpedoTrail.Play();
			}
			this.missileTrail.gameObject.SetActive(false);
			this.missileTrail.Stop();
			this.boosterRelease.Stop();
			this.boosterReleased = false;
			this.boosterTransform.gameObject.SetActive(true);
			this.poppedUp = false;
			this.poppingUp = false;
			this.cruiseYValue = 1000f + this.databaseweapondata.cruiseAltitude;
		}
		else
		{
			this.snakeTimer = this.snakeTime / 2f;
		}
		this.playerTimeToWaypoint = -100f;
		if (this.whichNavy == 1)
		{
			if (UIFunctions.globaluifunctions.playerfunctions.timeCompressionToggle == 1)
			{
				UIFunctions.globaluifunctions.playerfunctions.SetTimeCompression();
			}
			if (UnityEngine.Random.value < 0.2f)
			{
				foreach (string a in this.databaseweapondata.homeSettings)
				{
					if (a == "PASSIVE")
					{
						this.passiveHoming = true;
					}
				}
			}
			if (UnityEngine.Random.value > 0.5f)
			{
				this.willDriveThroughOnJam = true;
			}
		}
	}

	// Token: 0x06000AB2 RID: 2738 RVA: 0x000925BC File Offset: 0x000907BC
	private void SetTorpedoTargetsList()
	{
		this.targetsList.Clear();
		this.targetsList.Add(GameDataManager.playervesselsonlevel[0].transform);
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			if (!GameDataManager.enemyvesselsonlevel[i].isSinking && (!GameDataManager.enemyvesselsonlevel[i].vesselmovement.notMissileTarget || this.databaseweapondata.weaponType != "MISSILE"))
			{
				if (!this.noSurfaceTargets)
				{
					this.targetsList.Add(GameDataManager.enemyvesselsonlevel[i].transform);
				}
				else if (GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipType == "SUBMARINE" || GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipType == "BIOLOGIC" || GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipType == "OILRIG")
				{
					this.targetsList.Add(GameDataManager.enemyvesselsonlevel[i].transform);
				}
			}
			if (this.databaseweapondata.isMissile && GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipType == "BIOLOGIC")
			{
				this.targetsList.Remove(GameDataManager.enemyvesselsonlevel[i].transform);
			}
		}
	}

	// Token: 0x06000AB3 RID: 2739 RVA: 0x0009272C File Offset: 0x0009092C
	private void Start()
	{
		this.actualSensorAngles = this.databaseweapondata.sensorAngles;
		if (this.narrowCone)
		{
			this.actualSensorAngles.x = this.actualSensorAngles.x / 2f;
		}
		if (this.databaseweapondata.fixedRunDepth != -1f)
		{
			this.cruiseYValue = 1000f - this.databaseweapondata.fixedRunDepth * GameDataManager.feetToUnits;
			this.searchYValue = this.cruiseYValue;
		}
		if (this.databaseweapondata.isDumbfire)
		{
			this.runStraight = true;
		}
	}

	// Token: 0x06000AB4 RID: 2740 RVA: 0x000927C4 File Offset: 0x000909C4
	public void InitialiseParticles()
	{
		if (this.databaseweapondata.cavitationParticle != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(this.databaseweapondata.cavitationParticle, this.cavitationTransform.position, this.cavitationTransform.transform.rotation) as GameObject;
			ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
			this.torpedoTrail = component;
			gameObject.transform.SetParent(this.cavitationTransform, true);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			if (base.transform.position.y > 1000f)
			{
				AudioSource component2 = component.GetComponent<AudioSource>();
				if (component2 != null)
				{
					component2.enabled = false;
				}
			}
			if (this.databaseweapondata.swimOut)
			{
				gameObject.GetComponent<AudioSource>().enabled = false;
			}
		}
		if (this.databaseweapondata.missileTrail != null)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(this.databaseweapondata.missileTrail, this.missileTrailTransform.position, this.missileTrailTransform.transform.rotation) as GameObject;
			ParticleSystem component3 = gameObject2.GetComponent<ParticleSystem>();
			this.missileTrail = component3;
			gameObject2.transform.SetParent(this.missileTrailTransform, true);
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localRotation = Quaternion.identity;
		}
		if (this.databaseweapondata.parachute != null)
		{
			GameObject gameObject3 = UnityEngine.Object.Instantiate(this.databaseweapondata.parachute, this.parachuteTransform.position, this.parachuteTransform.transform.rotation) as GameObject;
			ParticleSystem component4 = gameObject3.GetComponent<ParticleSystem>();
			this.parachute = component4;
			gameObject3.transform.SetParent(this.parachuteTransform, true);
			gameObject3.transform.localPosition = Vector3.zero;
			gameObject3.transform.localRotation = Quaternion.identity;
		}
		if (this.databaseweapondata.boosterParticle != null)
		{
			GameObject gameObject4 = UnityEngine.Object.Instantiate(this.databaseweapondata.boosterParticle, this.boosterParticleTransform.position, this.boosterParticleTransform.transform.rotation) as GameObject;
			ParticleSystem component5 = gameObject4.GetComponent<ParticleSystem>();
			this.boosterRelease = component5;
			gameObject4.transform.SetParent(this.boosterParticleTransform, true);
			gameObject4.transform.localPosition = Vector3.zero;
			gameObject4.transform.localRotation = Quaternion.identity;
		}
		if (this.databaseweapondata.weaponType == "DECOY")
		{
			this.decoyAcoustics.SetActive(true);
		}
	}

	// Token: 0x06000AB5 RID: 2741 RVA: 0x00092A78 File Offset: 0x00090C78
	public void CalculateSensorCone()
	{
		float num = this.databaseweapondata.sensorAngles.x / 2f;
		if (num > 30f)
		{
			num = 30f;
		}
		float num2 = this.databaseweapondata.actualSensorRange * 5f;
		num2 *= Mathf.Cos(0.017453292f * num);
		float num3 = num2 * Mathf.Tan(0.017453292f * num);
		this.tacMapTorpedoIcon.sensorConeLines[0].SetPosition(1, new Vector3(-num3, num2, -1f));
		this.tacMapTorpedoIcon.sensorConeLines[1].SetPosition(1, new Vector3(num3, num2, -1f));
		this.tacMapTorpedoIcon.sensorConeLines[2].SetPosition(0, new Vector3(-num3, num2, -1f));
		this.tacMapTorpedoIcon.sensorConeLines[2].SetPosition(1, new Vector3(num3, num2, -1f));
		this.tacMapTorpedoIcon.sensorConeLines[3].SetPosition(0, new Vector3(-num3, num2, -1f));
		this.tacMapTorpedoIcon.sensorConeLines[3].SetPosition(1, new Vector3(-num3, 10000f, -1f));
		this.tacMapTorpedoIcon.sensorConeLines[4].SetPosition(0, new Vector3(num3, num2, -1f));
		this.tacMapTorpedoIcon.sensorConeLines[4].SetPosition(1, new Vector3(num3, 10000f, -1f));
		this.tacMapTorpedoIcon.sensorConeLines[0].gameObject.SetActive(false);
		this.coneInitialized = true;
	}

	// Token: 0x06000AB6 RID: 2742 RVA: 0x00092C00 File Offset: 0x00090E00
	private void DisplaySensorCone(bool target)
	{
		if (!this.coneInitialized)
		{
			return;
		}
		this.tacMapTorpedoIcon.sensorConeLines[0].gameObject.SetActive(true);
		for (int i = 0; i < this.tacMapTorpedoIcon.sensorConeLines.Length; i++)
		{
			this.tacMapTorpedoIcon.sensorConeLines[i].SetWidth(UIFunctions.globaluifunctions.levelloadmanager.tacticalmap.waypointLineSize * UIFunctions.globaluifunctions.levelloadmanager.tacticalmap.orthFactor, UIFunctions.globaluifunctions.levelloadmanager.tacticalmap.waypointLineSize * UIFunctions.globaluifunctions.levelloadmanager.tacticalmap.orthFactor);
			this.DisplaySensorConeColor(target);
		}
	}

	// Token: 0x06000AB7 RID: 2743 RVA: 0x00092CBC File Offset: 0x00090EBC
	private void DisplaySensorConeColor(bool target)
	{
		for (int i = 0; i < this.tacMapTorpedoIcon.sensorConeLines.Length; i++)
		{
			if (target)
			{
				this.tacMapTorpedoIcon.sensorConeLines[i].SetColors(UIFunctions.globaluifunctions.levelloadmanager.tacticalmap.sensorConeTracking, UIFunctions.globaluifunctions.levelloadmanager.tacticalmap.sensorConeTracking);
				if (this.onWire && this.playerMessageStatus != 1)
				{
					this.playerMessageStatus = 1;
					string dictionaryString = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "WeaponAcquire");
					UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(dictionaryString, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["FireControl"], "WeaponAcquire", false);
				}
			}
			else
			{
				this.tacMapTorpedoIcon.sensorConeLines[i].SetColors(UIFunctions.globaluifunctions.levelloadmanager.tacticalmap.sensorCone, UIFunctions.globaluifunctions.levelloadmanager.tacticalmap.sensorCone);
			}
		}
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x00092DC4 File Offset: 0x00090FC4
	private void SortTargetsByDistance()
	{
		this.targetsList.Sort((Transform t1, Transform t2) => Vector3.Distance(t1.position, base.transform.position).CompareTo(Vector3.Distance(t2.position, base.transform.position)));
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x00092DE0 File Offset: 0x00090FE0
	public void CheckLandStrikeNumber()
	{
		if (!GameDataManager.missionMode && !GameDataManager.trainingMode && !UIFunctions.globaluifunctions.campaignmanager.isThisALandStrike)
		{
			return;
		}
		UIFunctions.globaluifunctions.playerfunctions.landAttackNumber++;
		string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "LandStrikeHoming");
		text = text.Replace("<LANDTARGETSTATUS>", UIFunctions.globaluifunctions.playerfunctions.landAttackNumber.ToString());
		UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["XO"], "LandStrikeHoming", true);
		this.landAttackTerminal = true;
		if (!GameDataManager.trainingMode && !GameDataManager.missionMode)
		{
			if (UIFunctions.globaluifunctions.playerfunctions.landAttackNumber >= UIFunctions.globaluifunctions.campaignmanager.campaignmissions[UIFunctions.globaluifunctions.campaignmanager.campaignTaskForces[UIFunctions.globaluifunctions.campaignmanager.currentTaskForceEngagedWith].missionID].numberOfRequiredWeapon)
			{
				text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "LandStrikeComplete");
				UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["XO"], "LandStrikeComplete", true);
			}
		}
		else if (UIFunctions.globaluifunctions.playerfunctions.landAttackNumber > 8)
		{
			text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "LandStrikeComplete");
			UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["XO"], "LandStrikeComplete", true);
		}
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x00092F8C File Offset: 0x0009118C
	public void ActivateTorpedo()
	{
		if (this.databaseweapondata.landAttack)
		{
			this.guidanceActive = false;
			this.sensorsActive = true;
			if (UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.missileHitLocations.Length > 0)
			{
				this.targetTransform = UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.missileHitLocations[UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.numberOfLandHits];
				UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.numberOfLandHits++;
				if (UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.numberOfLandHits == UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.missileHitLocations.Length)
				{
					UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.numberOfLandHits = 0;
				}
			}
			else
			{
				this.ShootMissileDown(false);
			}
			return;
		}
		this.guidanceActive = false;
		this.sensorsActive = true;
		this.playerControlling = false;
		this.cruiseYValue = this.searchYValue;
		this.playerTimeToWaypoint = 10000f;
		if (!this.databaseweapondata.isDumbfire && this.databaseweapondata.weaponType == "TORPEDO")
		{
			if (this.whichNavy == 0)
			{
				this.tacMapTorpedoIcon.shipDisplayIcon.color = UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.torpedoColors[1];
			}
			else
			{
				this.tacMapTorpedoIcon.shipDisplayIcon.color = UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.torpedoColors[3];
			}
			if (this.whichNavy == 0 && this.onWire)
			{
				this.DisplaySensorCone(false);
			}
			this.TorpedoActivePing();
		}
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x00093160 File Offset: 0x00091360
	private void TorpedoActivePing()
	{
		if (this.passiveHoming || this.databaseweapondata.isDumbfire)
		{
			return;
		}
		this.activePingAudioSource.enabled = true;
		this.activePingAudioSource.Play();
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.DrawPingLine(base.transform, UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.pingLines[1]);
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x000931E4 File Offset: 0x000913E4
	public void DeactivateTorpedo()
	{
		this.guidanceActive = false;
		this.sensorsActive = false;
		this.searching = false;
		Graphic shipDisplayIcon = this.tacMapTorpedoIcon.shipDisplayIcon;
		Color color = UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.torpedoColors[2];
		this.tacMapTorpedoIcon.shipDisplayIcon.color = color;
		shipDisplayIcon.color = color;
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x00093250 File Offset: 0x00091450
	private void CheckWire()
	{
		if (this.whichNavy == 1 && this.guidanceActive && !this.vesselFiredFrom.isSinking && this.vesselFiredFrom.vesselai.transformToAttack != null && this.vesselFiredFrom.vesselai.sensordata.decibelsLastDetected > 0f)
		{
			float num = Vector3.Distance(this.vesselFiredFrom.vesselai.transformToAttack.position, base.transform.position);
			if (num < this.databaseweapondata.actualSensorRange * 0.85f)
			{
				this.ActivateTorpedo();
				return;
			}
			float d = num / (this.databaseweapondata.runSpeed / 10f * GameDataManager.globalTranslationSpeed);
			Vector3 b = this.vesselFiredFrom.vesselai.transformToAttack.position + this.vesselFiredFrom.vesselai.transformToAttack.forward * GameDataManager.playervesselsonlevel[0].vesselmovement.shipSpeed.z * GameDataManager.globalTranslationSpeed * d;
			b = Vector3.Lerp(base.transform.position, b, 0.8f);
			this.initialWaypointPosition = b;
		}
		float num2 = this.vesselFiredFrom.vesselmovement.shipSpeed.z * 20f;
		num2 += Mathf.Abs(this.vesselFiredFrom.vesselmovement.diveAngle.y) * 10f;
		num2 -= 30f;
		if (num2 > 0f && (float)UnityEngine.Random.Range(0, 50) < num2)
		{
			this.BreakWire();
			return;
		}
		if (this.vesselFiredFrom.isSinking)
		{
			this.BreakWire();
		}
	}

	// Token: 0x06000ABE RID: 2750 RVA: 0x0009341C File Offset: 0x0009161C
	private void BreakWire()
	{
		if (this.whichNavy == 0)
		{
			if (GameDataManager.trainingMode)
			{
				return;
			}
			GameDataManager.playervesselsonlevel[0].uifunctions.playerfunctions.CutWire(this.tubefiredFrom);
			string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "WireBreak");
			text = text.Replace("<WEAPON>", this.databaseweapondata.weaponName);
			UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["FireControl"], "WireBreak", false);
		}
		else if (UnityEngine.Random.value < 0.4f)
		{
			this.onWire = false;
		}
	}

	// Token: 0x06000ABF RID: 2751 RVA: 0x000934CC File Offset: 0x000916CC
	public void TranslateTorpedoForward()
	{
		base.transform.Translate(Vector3.forward * Time.deltaTime * this.actualCurrentSpeed);
	}

	// Token: 0x06000AC0 RID: 2752 RVA: 0x00093500 File Offset: 0x00091700
	private void FixedUpdate()
	{
		if (this.destroyTimer > 0f)
		{
			this.destroyTimer -= Time.deltaTime;
			if (this.destroyTimer <= 0f)
			{
				this.destroyMe = true;
			}
		}
		if (this.databaseweapondata.landAttack)
		{
			this.terrainScanTimer += Time.deltaTime;
			if (this.terrainScanTimer > 0.5f)
			{
				this.ScanForTerrain();
			}
		}
		if (this.destroyMe)
		{
			this.DestroyTorpedo(true);
			return;
		}
		this.timer += Time.deltaTime;
		if (this.timer > this.databaseweapondata.runTime)
		{
			this.destroyMe = true;
		}
		if (!this.boxcollider.enabled)
		{
			if (this.timer > 5f)
			{
				this.boxcollider.enabled = true;
			}
			if (UIFunctions.globaluifunctions.levelloadmanager.currentMapGeneratorInstance != null)
			{
				LayerMask mask = 1073741824;
				RaycastHit raycastHit;
				if (Physics.Raycast(base.transform.position, -Vector3.up, out raycastHit, 0.05f, mask))
				{
					this.destroyMe = true;
				}
			}
		}
		if (this.onWire)
		{
			float num = 5f;
			this.wireTimer += Time.deltaTime;
			if (this.wireTimer > num)
			{
				this.CheckWire();
				this.wireTimer -= num;
			}
		}
		if (this.sensorsActive)
		{
			this.sortTargetsTimer += Time.deltaTime;
			if (this.sortTargetsTimer > 6f)
			{
				this.SortTargetsByDistance();
				this.sortTargetsTimer = 0f;
			}
		}
		if (this.databaseweapondata.isMissile)
		{
			if (this.shotDown)
			{
				this.torpedoGuidance.transform.localRotation = Quaternion.Slerp(this.torpedoGuidance.transform.localRotation, Quaternion.Euler(this.shotDownAngles.x, this.shotDownAngles.y, 0f), 1f);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.torpedoGuidance.transform.rotation, this.databaseweapondata.turnRate * Time.deltaTime);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y, 0f), 1f);
				if (base.transform.position.y < 1000f)
				{
					ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.missileShotDown[UnityEngine.Random.Range(0, UIFunctions.globaluifunctions.database.missileShotDown.Length)], base.transform.position, base.transform.rotation);
					ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.surfacePlumes[UnityEngine.Random.Range(0, UIFunctions.globaluifunctions.database.surfacePlumes.Length)], base.transform.position, base.transform.rotation);
					if (this.shotDownParticleEffect != null)
					{
						this.shotDownParticleEffect.transform.parent = null;
						this.shotDownParticleEffect.GetComponent<DestroyTimer>().timer = 5f;
						this.shotDownParticleEffect.GetComponent<DestroyTimer>().enabled = true;
						this.shotDownParticleEffect.GetComponent<ParticleSystem>().Stop();
					}
					this.destroyMe = true;
				}
				return;
			}
			if (!this.isAirborne)
			{
				this.torpedoGuidance.transform.localRotation = Quaternion.Slerp(this.torpedoGuidance.transform.localRotation, Quaternion.Euler(-5f, 0f, 0f), 1f);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.torpedoGuidance.transform.rotation, this.databaseweapondata.turnRate * Time.deltaTime);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y, 0f), 1f);
				if (base.transform.position.y > 999.99f)
				{
					this.isAirborne = true;
					if (this.missileTrail != null)
					{
						this.missileTrail.gameObject.SetActive(true);
						this.missileTrail.Play();
					}
					if (this.launchAudioSource != null)
					{
						this.launchAudioSource.enabled = true;
						this.launchAudioSource.Play();
					}
					if (!this.databaseweapondata.surfaceLaunched)
					{
						ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.splashes[0], new Vector3(base.transform.position.x, 1000f, base.transform.position.z), Quaternion.Euler(0f, 0f, 0f));
					}
					this.timer = 0f;
					if (ManualCameraZoom.target == base.transform)
					{
						UIFunctions.globaluifunctions.playerfunctions.eventcamera.CheckSeaLevelSwap(true);
					}
				}
				if (base.transform.eulerAngles.x > 350f)
				{
					base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(350f, base.transform.eulerAngles.y, 0f), 1f);
				}
				return;
			}
			if (this.actualCurrentSpeed < this.databaseweapondata.actualActiveRunSpeed)
			{
				this.actualCurrentSpeed += 0.5f * Time.deltaTime;
			}
			this.actionTimer += Time.deltaTime;
			if (!this.boosterReleased && this.databaseweapondata.boosterReleasedAfterSeconds > 0f && this.timer > this.databaseweapondata.boosterReleasedAfterSeconds)
			{
				this.ReleaseBooster();
			}
			if (this.guidanceActive)
			{
				float num2 = base.transform.position.y - this.cruiseYValue;
				if (num2 > -0.01f && num2 < 0.01f)
				{
					num2 = 0f;
				}
				else
				{
					num2 *= 20f;
					if (num2 > 7f)
					{
						num2 = 7f;
					}
					else if (num2 < -7f)
					{
						num2 = -7f;
					}
				}
				this.torpedoGuidance.transform.LookAt(this.initialWaypointPosition);
				float y = this.torpedoGuidance.transform.eulerAngles.y;
				this.torpedoGuidance.transform.rotation = Quaternion.Slerp(this.torpedoGuidance.transform.rotation, Quaternion.Euler(num2, y, 0f), 1f);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.torpedoGuidance.transform.rotation, this.databaseweapondata.turnRate * Time.deltaTime);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y, 0f), 1f);
				if (Vector3.Distance(this.launchPosition, base.transform.position) > this.distanceToWaypoint)
				{
					if (!this.databaseweapondata.hasPayload)
					{
						this.ActivateTorpedo();
					}
					else if (!this.payloadDropped)
					{
						if (this.databaseweapondata.boosterReleasedAfterSeconds < 0f)
						{
							this.ReleaseBooster();
						}
						GameObject gameObject = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.databaseweapondata[this.databaseweapondata.missilePayload].weaponObject, this.payloadPosition.position, base.transform.rotation) as GameObject;
						if (ManualCameraZoom.target == base.transform)
						{
							ManualCameraZoom.target = gameObject.transform;
						}
						gameObject.SetActive(true);
						Torpedo component = gameObject.GetComponent<Torpedo>();
						component.databaseweapondata = UIFunctions.globaluifunctions.database.databaseweapondata[this.databaseweapondata.missilePayload];
						component.guidanceActive = false;
						component.sensorsActive = true;
						component.searching = true;
						component.vesselFiredFrom = this.vesselFiredFrom;
						component.cruiseYValue = GameDataManager.playervesselsonlevel[0].transform.position.y;
						component.searchYValue = GameDataManager.playervesselsonlevel[0].transform.position.y;
						component.noSurfaceTargets = UIFunctions.globaluifunctions.combatai.AreHostileShipsInArea();
						component.InitialiseTorpedo();
						UIFunctions.globaluifunctions.playerfunctions.sensormanager.AddTorpedoToArray(component);
						component.isAirborne = true;
						component.actualCurrentSpeed = this.actualCurrentSpeed;
						this.payloadDropped = true;
						this.onWire = false;
						this.destroyTimer = UnityEngine.Random.Range(4f, 9f);
						this.guidanceActive = false;
					}
				}
			}
			else if (this.sensorsActive)
			{
				if (this.landAttackTerminal)
				{
					float num3 = Vector3.Distance(base.transform.position, this.targetTransform.position);
					if (num3 < 1f)
					{
						ParticleSystem particleSystem = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.shipFires[0], this.targetTransform.position + Vector3.up * 0.15f, Quaternion.identity) as ParticleSystem;
						particleSystem.transform.SetParent(UIFunctions.globaluifunctions.levelloadmanager.currentMapGeneratorInstance.transform);
						ParticleSystem particleSystem2 = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.magazineExplosionsLand[UnityEngine.Random.Range(0, 2)], this.targetTransform.position, Quaternion.identity) as ParticleSystem;
						this.DestroyTorpedo(false);
						return;
					}
					if (num3 < 5f && !this.eventCameraSet)
					{
						this.eventCameraSet = true;
						UIFunctions.globaluifunctions.playerfunctions.eventcamera.CheckForEventCamera(base.transform, this.targetTransform, 10f, true, false, false, -1f, -1f, -1f, false);
					}
				}
				bool flag;
				if (this.chaffed)
				{
					this.torpedoGuidance.transform.LookAt(this.targetTransform.position);
					flag = true;
					this.driveAroundTimer += Time.deltaTime;
					if (this.driveAroundTimer > 5f)
					{
						this.driveAroundTimer = 0f;
						this.chaffed = false;
					}
				}
				else if (!this.databaseweapondata.landAttack || this.targetTransform == null)
				{
					if (this.poppingUp && !this.poppedUp)
					{
						flag = true;
						this.torpedoGuidance.transform.localRotation = Quaternion.Euler(10f, 0f, 0f);
					}
					else
					{
						flag = this.CheckTargetInSensorCone();
					}
				}
				else
				{
					flag = true;
					this.torpedoGuidance.transform.LookAt(this.targetTransform.position + Vector3.up * this.cruiseAltitudeBonus);
				}
				if (this.poppingUp && !this.chaffed)
				{
					this.torpedoGuidance.transform.rotation = Quaternion.Euler(-10f, 0f, 0f);
					if (base.transform.position.y > this.cruiseYValue)
					{
						this.poppingUp = false;
						this.poppedUp = true;
						this.cruiseYValue = 1000.1f;
						this.targetTransform = this.popUpTransform;
					}
				}
				else if (flag && !this.poppedUp && !this.runLow && Vector3.Distance(base.transform.position, this.targetTransform.position) < 31f)
				{
					this.poppingUp = true;
					this.popUpTransform = this.targetTransform;
					this.cruiseYValue = 1002f;
				}
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.torpedoGuidance.transform.rotation, this.databaseweapondata.turnRate * Time.deltaTime);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y, 0f), 1f);
			}
			if (base.transform.position.y < 1000f && this.timer > 2f)
			{
				ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.underwaterLargeExplosions[UnityEngine.Random.Range(0, UIFunctions.globaluifunctions.database.underwaterLargeExplosions.Length)], new Vector3(base.transform.position.x, 1000f, base.transform.position.z), Quaternion.identity);
				this.destroyMe = true;
			}
			if (base.transform.position.y < this.cruiseYValue)
			{
				float value = base.transform.position.y - this.cruiseYValue;
				value = Mathf.Clamp01(value);
			}
			else if (base.transform.position.y < this.cruiseYValue + 0.1f && base.transform.eulerAngles.x < 180f)
			{
				float num4 = base.transform.position.y - this.cruiseYValue;
				base.transform.Rotate(Vector3.right * -num4 * 2f);
			}
			return;
		}
		else
		{
			if (this.isAirborne)
			{
				if (this.actualCurrentSpeed > this.databaseweapondata.actualActiveRunSpeed && this.actualCurrentSpeed > 0.5f)
				{
					this.actualCurrentSpeed -= 0.5f * Time.deltaTime;
				}
				if (this.actualCurrentSpeed < 0.5f)
				{
					base.transform.Translate(Vector3.up * Time.deltaTime * -0.2f, Space.World);
				}
				base.transform.Translate(Vector3.up * Time.deltaTime * -0.1f, Space.World);
				float x = base.transform.eulerAngles.x;
				if (x < 88f || x > 180f)
				{
					base.transform.Rotate(Vector3.right * Time.deltaTime * 10f);
				}
				if (base.transform.position.y < 1000f)
				{
					if (!this.databaseweapondata.isSonobuoy)
					{
						this.isAirborne = false;
						this.tacMapTorpedoIcon.gameObject.SetActive(true);
						ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.splashes[0], new Vector3(base.transform.position.x, 1000f, base.transform.position.z), Quaternion.Euler(0f, 0f, 0f));
						this.actualCurrentSpeed = this.databaseweapondata.actualRunSpeed;
						if (this.cavitationAudioSource != null)
						{
							this.cavitationAudioSource.Play();
						}
						if (this.parachute != null)
						{
							this.parachute.Stop();
							this.parachute.gameObject.SetActive(false);
							if (this.whichNavy == 1)
							{
								this.cruiseYValue = GameDataManager.playervesselsonlevel[0].transform.position.y;
								float num5 = Vector3.Distance(base.transform.position, GameDataManager.playervesselsonlevel[0].transform.position) * GameDataManager.inverseYardsScale;
								if (num5 < 1500f)
								{
									if (UnityEngine.Random.value < 0.5f)
									{
										this.searchLeft = true;
									}
								}
								else
								{
									float value2 = UnityEngine.Random.value;
									if (value2 < 0.4f)
									{
										this.searchLeft = true;
									}
									else if (value2 >= 0.4f)
									{
										this.snakeSearch = true;
									}
									this.torpedoGuidance.LookAt(GameDataManager.playervesselsonlevel[0].transform.position);
									if (this.torpedoGuidance.localEulerAngles.y > 270f && this.torpedoGuidance.localEulerAngles.y < 90f && value2 < 0.6f)
									{
										this.searchLeft = false;
										this.snakeSearch = true;
									}
								}
							}
							this.ActivateTorpedo();
							if (ManualCameraZoom.target == base.transform)
							{
								UIFunctions.globaluifunctions.playerfunctions.eventcamera.CheckSeaLevelSwap(false);
							}
						}
					}
					else
					{
						ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.splashes[0], new Vector3(base.transform.position.x, 1000f, base.transform.position.z), Quaternion.Euler(0f, 0f, 0f));
						Noisemaker component2 = ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.sonobuoyInWaterObject, base.transform.position, Quaternion.identity).GetComponent<Noisemaker>();
						UIFunctions.globaluifunctions.playerfunctions.sensormanager.AddSonobuoyToArray(component2, this.sensorData);
						if (this.isActiveSonobuoy)
						{
							UIFunctions.globaluifunctions.playerfunctions.sensormanager.AddNoiseMakerToArray(component2);
							component2.tacMapNoisemakerIcon.shipDisplayIcon.color = UIFunctions.globaluifunctions.levelloadmanager.tacticalmap.navyColors[1];
							component2.name = "ACTIVE";
						}
						if (ManualCameraZoom.target == base.transform)
						{
							ManualCameraZoom.target = component2.gameObject.transform;
						}
						UIFunctions.globaluifunctions.playerfunctions.sensormanager.sonosInFlight.Remove(this);
						UnityEngine.Object.Destroy(base.gameObject);
					}
				}
				return;
			}
			if (this.sensorsActive)
			{
				this.RaycastSensor();
				if (this.actualCurrentSpeed < this.databaseweapondata.actualActiveRunSpeed)
				{
					this.actualCurrentSpeed += 0.2f * Time.deltaTime;
				}
				this.actionTimer += Time.deltaTime;
			}
			if (base.transform.position.y > 999.9f && !this.isAirborne)
			{
				float d = base.transform.position.y - 999.9f;
				base.transform.Rotate(Vector3.right * d * 6f);
			}
			float num6 = base.transform.position.y - this.cruiseYValue;
			if (num6 > -0.01f && num6 < 0.01f)
			{
				num6 = 0f;
			}
			else
			{
				num6 *= 20f;
				if (num6 > this.databaseweapondata.maxPitchAngle)
				{
					num6 = this.databaseweapondata.maxPitchAngle;
				}
				else if (num6 < -this.databaseweapondata.maxPitchAngle)
				{
					num6 = -this.databaseweapondata.maxPitchAngle;
				}
			}
			if (this.playerControlling)
			{
				this.playerDistToWaypoint = Vector2.Distance(new Vector2(base.transform.position.x, base.transform.position.z), new Vector2(this.initialWaypointPosition.x, this.initialWaypointPosition.z));
				if (this.playerTimeToWaypoint == -100f)
				{
					this.playerTimeToWaypoint = this.playerDistToWaypoint / this.actualCurrentSpeed;
				}
				this.playerTimeToWaypoint -= Time.deltaTime;
				this.cruiseYValue += this.playerDepthInput * Time.deltaTime * 0.5f;
				if (this.cruiseYValue > 999.98f)
				{
					this.cruiseYValue = 999.98f;
				}
				this.torpedoGuidance.transform.localRotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(0f, 10f * this.playerTurnInput, 0f), 1f);
				this.torpedoGuidance.transform.rotation = Quaternion.Slerp(this.torpedoGuidance.transform.rotation, Quaternion.Euler(num6, this.torpedoGuidance.transform.eulerAngles.y, 0f), 1f);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.torpedoGuidance.transform.rotation, this.databaseweapondata.turnRate * Time.deltaTime);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y, 0f), 1f);
				if (this.playerTimeToWaypoint < 0f)
				{
					this.ActivateTorpedo();
				}
				this.runStraight = true;
				if (this.playerDepthInput != 0f)
				{
					this.wasPlayerDepthControlled = true;
				}
				return;
			}
			if (this.wasPlayerDepthControlled)
			{
				this.wasPlayerDepthControlled = false;
				this.cruiseYValue = base.transform.position.y;
			}
			if (this.guidanceActive)
			{
				this.torpedoGuidance.transform.LookAt(this.initialWaypointPosition);
				if (this.lockGuidance)
				{
					this.torpedoGuidance.transform.rotation = Quaternion.Slerp(this.torpedoGuidance.transform.rotation, Quaternion.Euler(num6, base.transform.eulerAngles.y, 0f), 1f);
				}
				else
				{
					this.torpedoGuidance.transform.rotation = Quaternion.Slerp(this.torpedoGuidance.transform.rotation, Quaternion.Euler(num6, this.torpedoGuidance.transform.eulerAngles.y, 0f), 1f);
				}
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.torpedoGuidance.transform.rotation, this.databaseweapondata.turnRate * Time.deltaTime);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y, 0f), 1f);
				if (Vector2.Distance(new Vector2(base.transform.position.x, base.transform.position.z), new Vector2(this.initialWaypointPosition.x, this.initialWaypointPosition.z)) < 1f)
				{
					this.ActivateTorpedo();
				}
				return;
			}
			bool flag2 = this.CheckTargetInSensorCone();
			if (this.databaseweapondata.weaponType == "TORPEDO")
			{
				this.pingTimer += Time.deltaTime;
				float num7 = 10f;
				if (this.targetTransform != null)
				{
					num7 = Vector3.Distance(base.transform.position, this.targetTransform.position) / 2f;
					if (num7 < 1f)
					{
						num7 = 1f;
					}
				}
				if (this.pingTimer > num7 && !this.jammed)
				{
					this.TorpedoActivePing();
					this.pingTimer = 0f;
					if (flag2 && this.whichNavy == 0 && this.onWire && !this.passiveHoming)
					{
						for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
						{
							if (this.targetTransform == GameDataManager.enemyvesselsonlevel[i].transform)
							{
								UIFunctions.globaluifunctions.playerfunctions.sensormanager.solutionQualityOfContacts[i] = UIFunctions.globaluifunctions.playerfunctions.maximumPlayerTMA;
							}
						}
					}
				}
			}
			if (this.tacMapTorpedoIcon.sensorConeLines[0].gameObject.activeSelf)
			{
				if (this.jammed || this.driveThrough || this.drivingAround)
				{
					this.DisplaySensorConeColor(false);
				}
				else
				{
					this.DisplaySensorConeColor(flag2);
				}
			}
			if (flag2)
			{
				if (!this.jammed)
				{
					base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.Euler(num6, this.torpedoGuidance.transform.eulerAngles.y, 0f), this.databaseweapondata.turnRate * Time.deltaTime);
					if (!this.onTarget)
					{
						this.onTarget = true;
					}
				}
				else if (this.driveThrough)
				{
					this.torpedoGuidance.transform.localRotation = Quaternion.identity;
					base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.Euler(num6, this.torpedoGuidance.transform.eulerAngles.y, 0f), this.databaseweapondata.turnRate * Time.deltaTime);
					if (this.onTarget)
					{
						this.onTarget = false;
					}
				}
				else
				{
					this.drivingAround = true;
					this.driveAroundTimer = 0f;
					if (this.onTarget)
					{
						this.onTarget = false;
					}
				}
			}
			else
			{
				if (this.drivingAround)
				{
					this.driveAroundTimer += Time.deltaTime;
					if (this.driveAroundTimer > 8f)
					{
						this.driveAroundTimer = 0f;
						this.drivingAround = false;
						this.targetTransform = null;
						this.searching = true;
					}
					if (this.searchLeft)
					{
						base.transform.Rotate(Vector3.up * 10f * Time.deltaTime);
					}
					else
					{
						base.transform.Rotate(Vector3.up * -10f * Time.deltaTime);
					}
				}
				else if (!this.runStraight)
				{
					if (this.snakeSearch)
					{
						this.snakeTimer += Time.deltaTime;
						if (this.snakeTimer > this.snakeTime)
						{
							this.snakeMode *= -1;
							this.snakeTimer = 0f;
						}
						base.transform.Rotate(Vector3.up * 10f * (float)this.snakeMode * Time.deltaTime);
					}
					else if (this.searchLeft)
					{
						base.transform.Rotate(Vector3.up * -10f * Time.deltaTime);
					}
					else
					{
						base.transform.Rotate(Vector3.up * 10f * Time.deltaTime);
					}
				}
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.Euler(num6, base.transform.eulerAngles.y, 0f), this.databaseweapondata.turnRate * Time.deltaTime);
			}
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y, 0f), 1f);
			return;
		}
	}

	// Token: 0x06000AC1 RID: 2753 RVA: 0x0009529C File Offset: 0x0009349C
	private void ReleaseBooster()
	{
		this.boosterTransform.gameObject.SetActive(false);
		this.missileTrail.emissionRate = 0f;
		this.missileTrail.Stop();
		this.boosterRelease.Play();
		this.boosterReleased = true;
		if (this.databaseweapondata.boosterReleasedAfterSeconds > 0f && this.engineAudioSource != null)
		{
			this.engineAudioSource.enabled = true;
			this.engineAudioSource.Play();
		}
	}

	// Token: 0x06000AC2 RID: 2754 RVA: 0x00095324 File Offset: 0x00093524
	private bool CheckTargetInSensorCone()
	{
		if (!this.jammed)
		{
			if (!this.databaseweapondata.isMissile)
			{
				for (int i = 0; i < UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects.Length; i++)
				{
					if (UIFunctions.globaluifunctions.database.databaseweapondata[UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[i].databaseweapondata.weaponID].isDecoy && this.CheckWithinSensorAngles(UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[i].gameObject.transform))
					{
						this.targetTransform = UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[i].transform;
						this.DisableSnake();
						return true;
					}
				}
			}
			foreach (Transform transform in this.targetsList)
			{
				if (this.CheckWithinSensorAngles(transform))
				{
					this.targetTransform = transform;
					this.DisableSnake();
					return true;
				}
			}
			if (this.targetTransform != null)
			{
				this.targetTransform = null;
				this.targetVessel = null;
			}
			this.searching = true;
			if (this.databaseweapondata.isMissile && !this.poppedUp && !this.poppingUp)
			{
				this.ShootMissileDown(false);
			}
			return false;
		}
		if (this.drivingAround)
		{
			return false;
		}
		if (this.CheckWithinSensorAngles(this.targetTransform))
		{
			return true;
		}
		this.jammed = false;
		return false;
	}

	// Token: 0x06000AC3 RID: 2755 RVA: 0x000954F4 File Offset: 0x000936F4
	private void DisableSnake()
	{
		this.snakeSearch = false;
		this.runStraight = false;
		if (UnityEngine.Random.value < 0.5f)
		{
			this.searchLeft = true;
		}
		else
		{
			this.searchLeft = false;
		}
	}

	// Token: 0x06000AC4 RID: 2756 RVA: 0x00095534 File Offset: 0x00093734
	private void RaycastSensor()
	{
		this.sensorTimer += Time.deltaTime;
		if (this.sensorTimer < 1f)
		{
			return;
		}
		this.sensorTimer = 0f;
		int layerMask = 16384;
		this.driveThrough = false;
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, base.transform.forward, out raycastHit, this.databaseweapondata.actualSensorRange, layerMask))
		{
			string name = raycastHit.collider.name;
			if (!(name == "decoyAcoustics"))
			{
				this.jammed = true;
				if (this.willDriveThroughOnJam)
				{
					this.driveThrough = true;
				}
				this.runStraight = false;
				if (this.snakeSearch)
				{
					this.DisableSnake();
				}
				this.targetTransform = raycastHit.transform;
				if ((this.onWire || this.whichNavy == 0) && this.playerMessageStatus != 2)
				{
					this.playerMessageStatus = 2;
					string dictionaryString = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "WeaponDecoy");
					UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(dictionaryString, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["FireControl"], "WeaponDecoy", false);
				}
			}
		}
	}

	// Token: 0x06000AC5 RID: 2757 RVA: 0x00095674 File Offset: 0x00093874
	private bool CheckWithinSensorAngles(Transform targetTransform)
	{
		if (targetTransform == null)
		{
			return false;
		}
		if (this.poppingUp || this.poppedUp)
		{
			this.torpedoGuidance.transform.LookAt(targetTransform.position + Vector3.up * 0.05f);
			return true;
		}
		if (!this.databaseweapondata.isMissile)
		{
			float num = Vector3.Distance(targetTransform.position, base.transform.position);
			if (this.targetVessel != null && this.targetVessel.databaseshipdata.anechoicCoating && !this.passiveHoming)
			{
				num *= 2f;
			}
			float actualSensorRange = this.databaseweapondata.actualSensorRange;
			if (num > actualSensorRange)
			{
				return false;
			}
			if (this.whichNavy == 1 && targetTransform.position.y > 999.8f && targetTransform != GameDataManager.playervesselsonlevel[0].transform)
			{
				return false;
			}
		}
		if (this.databaseweapondata.isMissile)
		{
			if (targetTransform.position.y < 999.9f)
			{
				return false;
			}
			this.torpedoGuidance.transform.LookAt(targetTransform.position + Vector3.up * 0.05f);
		}
		else
		{
			this.torpedoGuidance.transform.LookAt(targetTransform.position + Vector3.up * -0.04f);
		}
		float num2 = this.torpedoGuidance.transform.localEulerAngles.y;
		float num3 = this.torpedoGuidance.transform.localEulerAngles.x;
		if (num2 > 180f)
		{
			num2 = 360f - num2;
		}
		if (num3 > 180f)
		{
			num3 = 360f - num3;
		}
		if (num2 < this.actualSensorAngles.x && num3 < this.actualSensorAngles.y)
		{
			if (this.onWire)
			{
				if (this.whichNavy == 0)
				{
					if (targetTransform == GameDataManager.playervesselsonlevel[0].transform)
					{
						return false;
					}
				}
				else if (targetTransform != GameDataManager.playervesselsonlevel[0].transform)
				{
					return false;
				}
			}
			if (!this.databaseweapondata.isMissile && !this.jammed)
			{
				this.cruiseYValue = targetTransform.position.y;
			}
			if (!this.databaseweapondata.isMissile && this.targetVessel == null)
			{
				this.targetVessel = targetTransform.GetComponent<Vessel>();
				if (this.targetVessel != null && this.targetVessel.whichNavy == 1 && this.targetVessel.vesselai.takingAction < 6)
				{
					this.targetVessel.vesselai.AvoidHazard(base.transform);
				}
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000AC6 RID: 2758 RVA: 0x00095980 File Offset: 0x00093B80
	private void DestroyTorpedo(bool effects = true)
	{
		if (effects)
		{
			if (!this.databaseweapondata.isMissile)
			{
				if (this.databaseweapondata.weaponType != "DECOY")
				{
					ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.underwaterLargeExplosions[UnityEngine.Random.Range(0, UIFunctions.globaluifunctions.database.underwaterLargeExplosions.Length)], base.transform.position, Quaternion.identity);
					ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.surfacePlumes[0], new Vector3(base.transform.position.x, 1000f, base.transform.position.z), Quaternion.identity);
				}
			}
			else if (!this.missileHit)
			{
				ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.surfacePlumes[UnityEngine.Random.Range(0, UIFunctions.globaluifunctions.database.surfacePlumes.Length)], new Vector3(base.transform.position.x, 1000f, base.transform.position.z), base.transform.rotation);
			}
			else
			{
				Vector3 eulerAngles = base.transform.eulerAngles;
				eulerAngles.x = 0f;
				eulerAngles.z = 0f;
				ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.missileImpacts[UnityEngine.Random.Range(0, UIFunctions.globaluifunctions.database.missileImpacts.Length)], new Vector3(base.transform.position.x, 1000f, base.transform.position.z), Quaternion.Euler(eulerAngles));
			}
		}
		if (ManualCameraZoom.target == base.transform)
		{
			ManualCameraZoom.cameraDummyTransform.position = base.transform.position;
			ManualCameraZoom.target = ManualCameraZoom.cameraDummyTransform;
			ManualCameraZoom.minDistance = 0.5f;
		}
		if (!this.isAirborne && this.tacMapTorpedoIcon != null)
		{
			UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.ClearMapContactTrail(this.tacMapTorpedoIcon, false);
		}
		if (this.tacMapTorpedoIcon != null)
		{
			ObjectPoolManager.DestroyPooled(this.tacMapTorpedoIcon.gameObject);
		}
		if (this.whichNavy == 0)
		{
			if (this.onWire)
			{
				this.TorpedoCutWire();
				if (UIFunctions.globaluifunctions.playerfunctions.currentActiveTorpedo != null && UIFunctions.globaluifunctions.playerfunctions.currentActiveTorpedo.tubefiredFrom == this.tubefiredFrom)
				{
					UIFunctions.globaluifunctions.playerfunctions.ClearCurrentActiveTorpedo();
				}
			}
			if (this.databaseweapondata.weaponType == "DECOY")
			{
				for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
				{
					float num = Vector3.Distance(base.transform.position, GameDataManager.enemyvesselsonlevel[i].transform.position);
					if (Vector3.Distance(GameDataManager.playervesselsonlevel[0].transform.position, GameDataManager.enemyvesselsonlevel[i].transform.position) > num)
					{
						GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata.playerDetected = false;
						GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata.timeTrackingPlayer = 0f;
						GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata.decibelsTotalDetected = 0f;
						GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata.bearingLastDetected = 0f;
					}
				}
				for (int j = 0; j < UIFunctions.globaluifunctions.combatai.enemyHelicopters.Length; j++)
				{
					float num2 = Vector3.Distance(base.transform.position, UIFunctions.globaluifunctions.combatai.enemyHelicopters[j].transform.position);
					if (Vector3.Distance(GameDataManager.playervesselsonlevel[0].transform.position, UIFunctions.globaluifunctions.combatai.enemyHelicopters[j].transform.position) > num2)
					{
						UIFunctions.globaluifunctions.combatai.enemyHelicopters[j].sensordata.playerDetected = false;
						UIFunctions.globaluifunctions.combatai.enemyHelicopters[j].sensordata.timeTrackingPlayer = 0f;
						UIFunctions.globaluifunctions.combatai.enemyHelicopters[j].sensordata.decibelsTotalDetected = 0f;
						UIFunctions.globaluifunctions.combatai.enemyHelicopters[j].sensordata.bearingLastDetected = 0f;
					}
				}
				for (int k = 0; k < UIFunctions.globaluifunctions.combatai.enemyAircraft.Length; k++)
				{
					float num3 = Vector3.Distance(base.transform.position, UIFunctions.globaluifunctions.combatai.enemyAircraft[k].transform.position);
					if (Vector3.Distance(GameDataManager.playervesselsonlevel[0].transform.position, UIFunctions.globaluifunctions.combatai.enemyAircraft[k].transform.position) > num3)
					{
						UIFunctions.globaluifunctions.combatai.enemyAircraft[k].sensordata.playerDetected = false;
						UIFunctions.globaluifunctions.combatai.enemyAircraft[k].sensordata.timeTrackingPlayer = 0f;
						UIFunctions.globaluifunctions.combatai.enemyAircraft[k].sensordata.decibelsTotalDetected = 0f;
						UIFunctions.globaluifunctions.combatai.enemyAircraft[k].sensordata.bearingLastDetected = 0f;
					}
				}
			}
		}
		if ((this.databaseweapondata.weaponType == "TORPEDO" || this.databaseweapondata.weaponType == "MISSILE" || this.databaseweapondata.weaponType == "DECOY") && UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects.Length > 0)
		{
			UIFunctions.globaluifunctions.playerfunctions.sensormanager.RemoveTorpedoFromArray(this.torpedoID);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x06000AC7 RID: 2759 RVA: 0x00095FD8 File Offset: 0x000941D8
	public void TorpedoCutWire()
	{
		this.onWire = false;
		if (this.whichNavy == 0)
		{
			if (UIFunctions.globaluifunctions.playerfunctions.numberOfWiresUsed > 0)
			{
				UIFunctions.globaluifunctions.playerfunctions.numberOfWiresUsed--;
				UIFunctions.globaluifunctions.portRearm.SetPlayerNumberOfWires();
			}
			UIFunctions.globaluifunctions.playerfunctions.torpedoTubeImages[this.tubefiredFrom].gameObject.SetActive(false);
			UIFunctions.globaluifunctions.playerfunctions.ClearTubeSettingButtons(this.tubefiredFrom);
			UIFunctions.globaluifunctions.playerfunctions.wireData[0].text = string.Empty;
			UIFunctions.globaluifunctions.playerfunctions.wireData[1].text = string.Empty;
		}
		this.tacMapTorpedoIcon.contactText.gameObject.SetActive(false);
		if (this.whichNavy == 0)
		{
			if (!this.guidanceActive)
			{
				this.tacMapTorpedoIcon.shipDisplayIcon.color = UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.torpedoColors[3];
			}
			else
			{
				Graphic shipDisplayIcon = this.tacMapTorpedoIcon.shipDisplayIcon;
				Color color = UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.torpedoColors[2];
				this.tacMapTorpedoIcon.shipDisplayIcon.color = color;
				shipDisplayIcon.color = color;
			}
			this.tacMapTorpedoIcon.sensorConeLines[0].gameObject.SetActive(false);
		}
		GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.torpedoesOnWire[this.tubefiredFrom] = null;
		GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.weaponInTube[this.tubefiredFrom] = -10;
	}

	// Token: 0x04001069 RID: 4201
	public DatabaseWeaponData databaseweapondata;

	// Token: 0x0400106A RID: 4202
	public int torpedoID;

	// Token: 0x0400106B RID: 4203
	public int whichNavy;

	// Token: 0x0400106C RID: 4204
	public int tubefiredFrom;

	// Token: 0x0400106D RID: 4205
	public bool guidanceActive;

	// Token: 0x0400106E RID: 4206
	public bool sensorsActive;

	// Token: 0x0400106F RID: 4207
	public bool searching;

	// Token: 0x04001070 RID: 4208
	public Transform torpedoGuidance;

	// Token: 0x04001071 RID: 4209
	public Vector3 launchPosition;

	// Token: 0x04001072 RID: 4210
	public float distanceToWaypoint;

	// Token: 0x04001073 RID: 4211
	public float cruiseYValue;

	// Token: 0x04001074 RID: 4212
	public float searchYValue;

	// Token: 0x04001075 RID: 4213
	public bool isInPlayerBaffles;

	// Token: 0x04001076 RID: 4214
	public bool lockGuidance;

	// Token: 0x04001077 RID: 4215
	public bool isActiveSonobuoy;

	// Token: 0x04001078 RID: 4216
	public SensorData sensorData;

	// Token: 0x04001079 RID: 4217
	public bool onWire;

	// Token: 0x0400107A RID: 4218
	public float wireLaunchAngle;

	// Token: 0x0400107B RID: 4219
	public bool playerControlling;

	// Token: 0x0400107C RID: 4220
	public float playerTurnInput;

	// Token: 0x0400107D RID: 4221
	public float playerDepthInput;

	// Token: 0x0400107E RID: 4222
	public float playerControlTimer;

	// Token: 0x0400107F RID: 4223
	public float wireTimer;

	// Token: 0x04001080 RID: 4224
	public bool driveThrough;

	// Token: 0x04001081 RID: 4225
	public bool driveAround;

	// Token: 0x04001082 RID: 4226
	public bool drivingAround;

	// Token: 0x04001083 RID: 4227
	public float playerTimeToWaypoint;

	// Token: 0x04001084 RID: 4228
	public float playerDistToWaypoint;

	// Token: 0x04001085 RID: 4229
	public AudioSource cavitationAudioSource;

	// Token: 0x04001086 RID: 4230
	public bool passiveHoming;

	// Token: 0x04001087 RID: 4231
	public float passiveRange;

	// Token: 0x04001088 RID: 4232
	public float pingTimer;

	// Token: 0x04001089 RID: 4233
	public AudioSource activePingAudioSource;

	// Token: 0x0400108A RID: 4234
	public bool decoyIdentified;

	// Token: 0x0400108B RID: 4235
	public bool isAirborne;

	// Token: 0x0400108C RID: 4236
	public bool runLow;

	// Token: 0x0400108D RID: 4237
	public bool poppingUp;

	// Token: 0x0400108E RID: 4238
	public bool poppedUp;

	// Token: 0x0400108F RID: 4239
	public Transform torpedoGuidanceElevation;

	// Token: 0x04001090 RID: 4240
	public AudioSource launchAudioSource;

	// Token: 0x04001091 RID: 4241
	public AudioSource engineAudioSource;

	// Token: 0x04001092 RID: 4242
	public bool boosterReleased;

	// Token: 0x04001093 RID: 4243
	public Vector3 initialWaypointPosition;

	// Token: 0x04001094 RID: 4244
	public int gunsOnMissile;

	// Token: 0x04001095 RID: 4245
	public bool chaffed;

	// Token: 0x04001096 RID: 4246
	public bool narrowCone;

	// Token: 0x04001097 RID: 4247
	public Vector2 actualSensorAngles;

	// Token: 0x04001098 RID: 4248
	public bool missileHit;

	// Token: 0x04001099 RID: 4249
	public bool payloadDropped;

	// Token: 0x0400109A RID: 4250
	public int runDepth;

	// Token: 0x0400109B RID: 4251
	public bool searchLeft;

	// Token: 0x0400109C RID: 4252
	public bool runAround;

	// Token: 0x0400109D RID: 4253
	public bool runAroundLeft;

	// Token: 0x0400109E RID: 4254
	public bool snakeSearch;

	// Token: 0x0400109F RID: 4255
	public bool runStraight;

	// Token: 0x040010A0 RID: 4256
	public int snakeMode;

	// Token: 0x040010A1 RID: 4257
	public float snakeTime;

	// Token: 0x040010A2 RID: 4258
	public float snakeTimer;

	// Token: 0x040010A3 RID: 4259
	public float driveAroundTimer;

	// Token: 0x040010A4 RID: 4260
	public Transform targetTransform;

	// Token: 0x040010A5 RID: 4261
	public Vessel targetVessel;

	// Token: 0x040010A6 RID: 4262
	public List<Transform> targetsList;

	// Token: 0x040010A7 RID: 4263
	public float actualCurrentSpeed;

	// Token: 0x040010A8 RID: 4264
	public float timer;

	// Token: 0x040010A9 RID: 4265
	public float actionTimer;

	// Token: 0x040010AA RID: 4266
	public float sensorTimer;

	// Token: 0x040010AB RID: 4267
	public float sortTargetsTimer;

	// Token: 0x040010AC RID: 4268
	public bool destroyMe;

	// Token: 0x040010AD RID: 4269
	public bool shotDown;

	// Token: 0x040010AE RID: 4270
	public bool jammed;

	// Token: 0x040010AF RID: 4271
	public bool onTarget;

	// Token: 0x040010B0 RID: 4272
	public bool noSurfaceTargets;

	// Token: 0x040010B1 RID: 4273
	public MapContact tacMapTorpedoIcon;

	// Token: 0x040010B2 RID: 4274
	public BoxCollider boxcollider;

	// Token: 0x040010B3 RID: 4275
	public bool coneInitialized;

	// Token: 0x040010B4 RID: 4276
	public GameObject[] torpedoMeshes;

	// Token: 0x040010B5 RID: 4277
	public GameObject[] torpedoPropMeshes;

	// Token: 0x040010B6 RID: 4278
	public RotateTorpedoProp[] propRotations;

	// Token: 0x040010B7 RID: 4279
	public GameObject boosterMesh;

	// Token: 0x040010B8 RID: 4280
	public Transform boosterTransform;

	// Token: 0x040010B9 RID: 4281
	public Transform boosterParticleTransform;

	// Token: 0x040010BA RID: 4282
	public Transform parachuteTransform;

	// Token: 0x040010BB RID: 4283
	public Transform missileTrailTransform;

	// Token: 0x040010BC RID: 4284
	public Transform cavitationTransform;

	// Token: 0x040010BD RID: 4285
	public Transform payloadPosition;

	// Token: 0x040010BE RID: 4286
	public ParticleSystem parachute;

	// Token: 0x040010BF RID: 4287
	public ParticleSystem torpedoTrail;

	// Token: 0x040010C0 RID: 4288
	public ParticleSystem missileTrail;

	// Token: 0x040010C1 RID: 4289
	public ParticleSystem boosterRelease;

	// Token: 0x040010C2 RID: 4290
	public GameObject blastzoneObject;

	// Token: 0x040010C3 RID: 4291
	public GameObject decoyAcoustics;

	// Token: 0x040010C4 RID: 4292
	public Vessel vesselFiredFrom;

	// Token: 0x040010C5 RID: 4293
	public int damageDealt;

	// Token: 0x040010C6 RID: 4294
	public float destroyTimer;

	// Token: 0x040010C7 RID: 4295
	public GameObject shotDownParticleEffect;

	// Token: 0x040010C8 RID: 4296
	public Vector2 shotDownAngles;

	// Token: 0x040010C9 RID: 4297
	private bool wasPlayerDepthControlled;

	// Token: 0x040010CA RID: 4298
	public bool willDriveThroughOnJam;

	// Token: 0x040010CB RID: 4299
	public bool landAttackTerminal;

	// Token: 0x040010CC RID: 4300
	public float terrainScanTimer;

	// Token: 0x040010CD RID: 4301
	public float cruiseAltitudeBonus;

	// Token: 0x040010CE RID: 4302
	public bool eventCameraSet;

	// Token: 0x040010CF RID: 4303
	public Transform popUpTransform;

	// Token: 0x040010D0 RID: 4304
	public bool calledOutToPlayer;

	// Token: 0x040010D1 RID: 4305
	public int playerMessageStatus;

	// Token: 0x040010D2 RID: 4306
	public bool missileSnapShotAt;

	// Token: 0x040010D3 RID: 4307
	public bool torpedoSnapShotAt;
}
