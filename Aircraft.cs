using System;
using UnityEngine;

// Token: 0x020000EA RID: 234
public class Aircraft : MonoBehaviour
{
	// Token: 0x0600062C RID: 1580 RVA: 0x0002AF48 File Offset: 0x00029148
	private void Start()
	{
		this.turningParameters = Vector2.zero;
		this.airspeed = this.databaseaircraftdata.cruiseSpeed / 10f * GameDataManager.globalTranslationSpeed;
		this.airspeed /= UIFunctions.globaluifunctions.gamedatamanager.globalSpeedModifier;
		this.waypoint.parent = null;
		this.torpedoesOnBoard = this.databaseaircraftdata.torpedoNumbers[0];
		this.depthBombsOnBoard = this.databaseaircraftdata.depthBombs[0];
		this.sonobuoysOnBoard = new int[this.databaseaircraftdata.sonobuoyNumbers.Length];
		for (int i = 0; i < this.databaseaircraftdata.sonobuoyNumbers.Length; i++)
		{
			this.sonobuoysOnBoard[i] = this.databaseaircraftdata.sonobuoyNumbers[i];
		}
		this.sensordata.sonobuoySonarIDs = new int[this.databaseaircraftdata.sonobuoyIDs.Length];
		for (int j = 0; j < this.databaseaircraftdata.sonobuoyIDs.Length; j++)
		{
			this.sensordata.sonobuoySonarIDs[j] = this.databaseaircraftdata.sonobuoyIDs[j];
		}
		this.audiosource.Play();
		if (!UIFunctions.globaluifunctions.combatai.usingFormationAI)
		{
			this.GetNextPointInSearchArea();
		}
		base.transform.Translate(Vector3.up * (0.3f * (float)this.aircraftInstanceId));
		this.currentAttack = "UNDETERMINED";
		this.defaultCruiseAltitude = base.transform.position.y;
		this.cruiseAltitude = new Vector2(this.defaultCruiseAltitude, this.defaultCruiseAltitude);
	}

	// Token: 0x0600062D RID: 1581 RVA: 0x0002B0F0 File Offset: 0x000292F0
	private void ScanForTerrain()
	{
		int layerMask = 1073741824;
		this.raycastPosition.transform.rotation = Quaternion.Euler(1.8f, this.raycastPosition.transform.eulerAngles.y, 0f);
		RaycastHit raycastHit;
		if (Physics.Raycast(this.raycastPosition.position, this.raycastPosition.forward, out raycastHit, 30f, layerMask))
		{
			this.cruiseAltitude.y = this.cruiseAltitude.y + 0.2f;
			if (raycastHit.distance < 20f)
			{
				this.cruiseAltitude.y = this.cruiseAltitude.y + 0.2f;
			}
		}
		else if (this.cruiseAltitude.x > this.defaultCruiseAltitude)
		{
			this.cruiseAltitude.y = this.cruiseAltitude.y - 0.02f;
		}
		this.terrainScanTimer = 0f;
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x0002B1E0 File Offset: 0x000293E0
	private int GetSonobuoyTypeToDrop()
	{
		int num = 0;
		for (int i = 0; i < this.sonobuoysOnBoard.Length; i++)
		{
			if (this.sonobuoysOnBoard[i] != 0)
			{
				num++;
				break;
			}
		}
		if (num == 0)
		{
			return -1;
		}
		int num2 = UnityEngine.Random.Range(0, this.sonobuoysOnBoard.Length);
		if (this.sonobuoysOnBoard[num2] == 0)
		{
			num2 = UnityEngine.Random.Range(0, this.sonobuoysOnBoard.Length);
		}
		if (this.sonobuoysOnBoard[num2] == 0)
		{
			num2 = UnityEngine.Random.Range(0, this.sonobuoysOnBoard.Length);
		}
		if (this.sonobuoysOnBoard[num2] == 0)
		{
			num2 = 0;
			for (int j = 0; j < this.sonobuoysOnBoard.Length; j++)
			{
				if (this.sonobuoysOnBoard[j] != 0)
				{
					num2 = j;
					break;
				}
			}
		}
		return num2;
	}

	// Token: 0x0600062F RID: 1583 RVA: 0x0002B2AC File Offset: 0x000294AC
	private void GetNextPointInSearchArea()
	{
		int num = 0;
		do
		{
			Vector3 vector = UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.aircraftSearchAreas[UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.numberOfHelicopters];
			Vector2 vector2 = UnityEngine.Random.insideUnitCircle * vector.z;
			this.waypoint.transform.position = new Vector3(vector.x + vector2.x, 1000f, vector.y + vector2.y);
			num++;
		}
		while (UIFunctions.globaluifunctions.GetXZDistance(base.transform.position, this.waypoint.position) < 25f && num < 10);
	}

	// Token: 0x06000630 RID: 1584 RVA: 0x0002B370 File Offset: 0x00029570
	private void SetPlayerPositionAsWaypoint()
	{
		float num = 1000f - GameDataManager.playervesselsonlevel[0].transform.position.y;
		if (this.currentAttack != "DEPTHBOMB")
		{
			num = 0f;
		}
		float d = num * 2f + 2.5f;
		Transform transform = this.transformToAttack;
		if (transform == null)
		{
			this.attackPlayer = false;
			this.OnAttackRun = false;
			this.GetNextPointInSearchArea();
			return;
		}
		Vector3 position = transform.position + transform.forward * GameDataManager.playervesselsonlevel[0].vesselmovement.shipSpeed.z * GameDataManager.globalTranslationSpeed * d;
		position.y = 1000f;
		this.waypoint.transform.position = position;
	}

	// Token: 0x06000631 RID: 1585 RVA: 0x0002B44C File Offset: 0x0002964C
	public void TranslateAircraftForward()
	{
		base.transform.Translate(Vector3.forward * Time.deltaTime * this.airspeed);
	}

	// Token: 0x06000632 RID: 1586 RVA: 0x0002B480 File Offset: 0x00029680
	public Vector3 GetNewFormationWaypoint()
	{
		float num = (float)(this.aircraftInstanceId + 2);
		Vector3 vector = UIFunctions.globaluifunctions.combatai.levelloadmanager.levelloaddata.formationGrid.transform.position;
		vector += UIFunctions.globaluifunctions.combatai.levelloadmanager.levelloaddata.formationGrid.transform.forward * (UIFunctions.globaluifunctions.combatai.formationBounds.z + 20f * num);
		if (this.currentWaypoint == 0)
		{
			vector += UIFunctions.globaluifunctions.combatai.levelloadmanager.levelloaddata.formationGrid.transform.right * ((float)UnityEngine.Random.Range(20, 50) * num);
			vector += UIFunctions.globaluifunctions.combatai.levelloadmanager.levelloaddata.formationGrid.transform.forward * ((float)UnityEngine.Random.Range(0, 20) * num);
		}
		else
		{
			vector += UIFunctions.globaluifunctions.combatai.levelloadmanager.levelloaddata.formationGrid.transform.right * ((float)UnityEngine.Random.Range(-50, -20) * num);
			vector += UIFunctions.globaluifunctions.combatai.levelloadmanager.levelloaddata.formationGrid.transform.forward * ((float)UnityEngine.Random.Range(0, 20) * num);
		}
		if (UnityEngine.Random.value < 0.1f * (float)UIFunctions.globaluifunctions.combatai.enemyAircraft.Length)
		{
			vector = UIFunctions.globaluifunctions.combatai.levelloadmanager.levelloaddata.formationGrid.transform.position;
			if (this.currentWaypoint == 1)
			{
				vector += UIFunctions.globaluifunctions.combatai.levelloadmanager.levelloaddata.formationGrid.transform.right * ((float)UnityEngine.Random.Range(20, 50) * num);
			}
			else
			{
				vector += UIFunctions.globaluifunctions.combatai.levelloadmanager.levelloaddata.formationGrid.transform.right * ((float)UnityEngine.Random.Range(-50, -20) * num);
			}
			vector += UIFunctions.globaluifunctions.combatai.levelloadmanager.levelloaddata.formationGrid.transform.forward * ((float)UnityEngine.Random.Range(-40, 0) * num);
		}
		if (UIFunctions.globaluifunctions.GetXZDistance(this.waypoint.position, vector) < 15f)
		{
			vector += UIFunctions.globaluifunctions.combatai.levelloadmanager.levelloaddata.formationGrid.transform.forward * 25f;
		}
		return vector;
	}

	// Token: 0x06000633 RID: 1587 RVA: 0x0002B75C File Offset: 0x0002995C
	private void PassContactInfoToAllVessels()
	{
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			if (GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata != null && GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipType != "SUBMARINE")
			{
				SensorData sensorData = GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata;
				if (this.sensordata.decibelsLastDetected > sensorData.decibelsLastDetected)
				{
					sensorData.decibelsLastDetected = this.sensordata.decibelsLastDetected;
				}
				if (this.sensordata.decibelsTotalDetected > sensorData.decibelsTotalDetected)
				{
					sensorData.decibelsTotalDetected = this.sensordata.decibelsTotalDetected;
				}
			}
		}
	}

	// Token: 0x06000634 RID: 1588 RVA: 0x0002B820 File Offset: 0x00029A20
	private void FixedUpdate()
	{
		if (this.lastAttackRunTimer > 0f)
		{
			this.lastAttackRunTimer -= Time.deltaTime;
		}
		if (!this.doOnce)
		{
			this.doOnce = true;
			float num = UIFunctions.globaluifunctions.levelloadmanager.GetTerrainHeightAtPosition(base.transform.position.x, base.transform.position.z);
			if (num < 0f)
			{
				num -= 3f;
				base.transform.Translate(Vector3.up * -num);
				this.cruiseAltitude = new Vector2(base.transform.position.y, base.transform.position.y);
			}
		}
		if (this.isTerrainPresent)
		{
			this.terrainScanTimer += Time.deltaTime;
			if (this.terrainScanTimer > 1f)
			{
				this.ScanForTerrain();
			}
		}
		if (this.sonobuoyTimer > 0f)
		{
			this.sonobuoyTimer -= Time.deltaTime;
			if (this.sonobuoyTimer < 0f)
			{
				this.sonobuoyTypeToDrop = this.GetSonobuoyTypeToDrop();
				if (this.sonobuoyTypeToDrop != -1)
				{
					this.DropSonoBuoy();
				}
			}
		}
		if (this.depthBombsDropped != 0)
		{
			this.depthBombTimer += Time.deltaTime;
			if (this.depthBombTimer > 0f && this.depthBombsDropped == 1)
			{
				this.DropDepthBomb();
			}
			else if (this.depthBombTimer > 0.5f && this.depthBombsDropped == 2)
			{
				this.DropDepthBomb();
			}
			else if (this.depthBombTimer > 1f && this.depthBombsDropped == 3)
			{
				this.DropDepthBomb();
				this.depthBombsDropped = 0;
				this.depthBombTimer = 0f;
			}
		}
		base.transform.Rotate(Vector3.up, this.turningParameters.y * Time.deltaTime * 2f);
		if (this.turningParameters.y != 0f)
		{
			this.meshHolder.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, Quaternion.Euler(0f, 0f, -this.turningParameters.y * 2f), 1f);
		}
		this.cruiseAltitude.x = base.transform.position.y;
		if (this.cruiseAltitude.y != this.cruiseAltitude.x)
		{
			float num2 = 1f;
			float num3 = Mathf.Abs(this.cruiseAltitude.x - this.cruiseAltitude.y);
			if (this.cruiseAltitude.y > this.cruiseAltitude.x)
			{
				num2 = -1f;
			}
			base.transform.Rotate(num3 * num2 * Time.deltaTime, 0f, 0f);
			this.timePitching += Time.deltaTime;
			if (this.timePitching > 3f && (base.transform.localEulerAngles.x < 0.05f || base.transform.localEulerAngles.x > 359.95f))
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(0f, base.transform.localEulerAngles.y, 0f), 1f);
				this.cruiseAltitude.x = base.transform.position.y;
				this.cruiseAltitude.y = this.cruiseAltitude.x;
				this.timePitching = 0f;
			}
		}
		if (base.transform.eulerAngles.z != 0f)
		{
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y, 0f), 1f);
		}
		if (!this.attackPlayer && !this.OnAttackRun)
		{
			if (UIFunctions.globaluifunctions.GetXZDistance(base.transform.position, this.waypoint.position) < 12f)
			{
				if (!UIFunctions.globaluifunctions.combatai.usingFormationAI)
				{
					this.GetNextPointInSearchArea();
				}
				else
				{
					this.currentWaypoint++;
					if (this.currentWaypoint > 1)
					{
						this.currentWaypoint = 0;
					}
					this.waypoint.position = this.GetNewFormationWaypoint();
				}
			}
			if (this.sensordata.playerDetected)
			{
				if (this.sensordata.decibelsLastDetected < 0f)
				{
					this.sensordata.playerDetected = false;
				}
				else if (this.sensordata.decibelsLastDetected > 8f && this.sensordata.decibelsTotalDetected > 150f)
				{
					this.attackPlayerActualTransform = true;
					this.attackPlayer = true;
				}
			}
		}
		if (this.attackPlayer)
		{
			this.attackPlayer = false;
			float d = 1000f - this.waypoint.transform.position.y;
			this.transformToAttack = UIFunctions.globaluifunctions.combatai.GetActualPositionToAttack(base.transform);
			if (this.transformToAttack == null)
			{
				this.attackPlayer = false;
				this.OnAttackRun = false;
				return;
			}
			this.waypoint.transform.position = this.transformToAttack.position;
			this.waypoint.transform.Translate(Vector3.up * d);
			this.OnAttackRun = true;
		}
		else if (this.OnAttackRun && this.lastAttackRunTimer <= 0f)
		{
			if (this.sonobuoysDropped > 6 && this.sensordata.decibelsLastDetected <= 0f)
			{
				this.OnAttackRun = false;
				this.sonobuoysDropped = 0;
			}
			if (this.attackPlayerActualTransform)
			{
				this.SetPlayerPositionAsWaypoint();
			}
			Vector3 position = base.transform.position;
			position.y = 1000f;
			float xzdistance = UIFunctions.globaluifunctions.GetXZDistance(position, this.waypoint.transform.position);
			if (xzdistance < 8f && this.currentAttack == "UNDETERMINED")
			{
				this.GetAttackType();
			}
			if (xzdistance < this.attackDist)
			{
				float distanceToNearestEnemyVessel = UIFunctions.globaluifunctions.playerfunctions.sensormanager.GetDistanceToNearestEnemyVessel(base.transform, true, true);
				if (distanceToNearestEnemyVessel < 2500f)
				{
					float num4 = UnityEngine.Random.Range(15f, 25f);
					float num5 = UnityEngine.Random.Range(15f, 25f);
					if (UnityEngine.Random.value < 0.5f)
					{
						num4 *= -1f;
					}
					if (UnityEngine.Random.value < 0.5f)
					{
						num5 *= -1f;
					}
					Vector3 position2 = base.transform.position + base.transform.forward * num5;
					position2 = base.transform.position + base.transform.right * num4;
					position2.y = 1000f;
					this.waypoint.position = position2;
					this.sensordata.decibelsLastDetected = 0f;
					this.sensordata.decibelsTotalDetected = 0f;
					this.sensordata.radarTotalDetected = 0f;
				}
				else
				{
					string text = this.currentAttack;
					switch (text)
					{
					case "TORPEDO":
						this.DropTorpedo();
						break;
					case "DEPTHBOMB":
						this.depthBombsDropped = 1;
						break;
					case "SONOBUOY":
						this.DropSonoBuoy();
						break;
					}
				}
				this.OnAttackRun = false;
				this.currentAttack = "UNDETERMINED";
				this.attackPlayerActualTransform = false;
				this.lastAttackRunTimer += (float)UnityEngine.Random.Range(12, 16);
			}
		}
		if (this.straightRunTimer <= 0f)
		{
			this.navigator.transform.LookAt(this.waypoint);
		}
		else
		{
			this.navigator.transform.localRotation = Quaternion.identity;
			this.straightRunTimer -= Time.deltaTime;
		}
		float num7 = this.navigator.transform.localEulerAngles.y;
		if (num7 > 180f)
		{
			num7 -= 360f;
		}
		if (num7 < -90f)
		{
			num7 = -90f;
		}
		else if (num7 > 90f)
		{
			num7 = 90f;
		}
		num7 /= 15f;
		this.turningParameters.x = num7;
		if (this.straightRunTimer <= 0f && Mathf.Abs(num7) == 6f && UIFunctions.globaluifunctions.GetXZDistance(this.waypoint.position, this.navigator.transform.position) < 15f)
		{
			this.straightRunTimer = UnityEngine.Random.Range(20f, 40f);
			this.turningParameters.x = 0f;
		}
		if (this.turningParameters.y != this.turningParameters.x)
		{
			float num8 = Mathf.Abs(this.turningParameters.y - this.turningParameters.x);
			if (num8 < 0.05f)
			{
				this.turningParameters.y = this.turningParameters.x;
			}
			else
			{
				float num9 = 1f;
				if (num8 > 0.05f && num8 < 1f)
				{
					num9 = num8;
				}
				if (this.turningParameters.y < this.turningParameters.x)
				{
					this.turningParameters.y = this.turningParameters.y + 1f * num9 * Time.deltaTime;
				}
				else if (this.turningParameters.y > this.turningParameters.x)
				{
					this.turningParameters.y = this.turningParameters.y - 1f * num9 * Time.deltaTime;
				}
			}
		}
	}

	// Token: 0x06000635 RID: 1589 RVA: 0x0002C314 File Offset: 0x0002A514
	private string GetAttackType()
	{
		float num = 1000f - this.waypoint.transform.position.y;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (num < 0.6f && this.depthBombsOnBoard > 0 && GameDataManager.playervesselsonlevel[0].vesselmovement.shipSpeed.z < 15f && base.transform.position.y < 1002f)
		{
			flag = true;
		}
		if (this.torpedoesOnBoard > 0 && base.transform.position.y < 1002f)
		{
			flag2 = true;
		}
		this.sonobuoyTypeToDrop = this.GetSonobuoyTypeToDrop();
		if (this.sonobuoyTypeToDrop != -1 && base.transform.position.y < 1003f)
		{
			flag3 = true;
		}
		string result = "UNDETERMINED";
		if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.HaveVisualOnPlayerSub(base.transform.position))
		{
			this.sensordata.decibelsTotalDetected += 500f;
			this.sensordata.decibelsLastDetected = 50f;
			this.attackPlayerActualTransform = true;
			if (flag)
			{
				this.currentAttack = "DEPTHBOMB";
			}
			else if (flag2)
			{
				this.currentAttack = "TORPEDO";
			}
			else if (flag3)
			{
				this.currentAttack = "SONOBUOY";
			}
		}
		else if (this.sensordata.decibelsTotalDetected > 150f)
		{
			if (flag2)
			{
				this.currentAttack = "TORPEDO";
			}
			else if (flag3)
			{
				this.currentAttack = "SONOBUOY";
			}
			if (UnityEngine.Random.value < 0.5f && this.attackPlayerActualTransform && flag)
			{
				this.currentAttack = "DEPTHBOMB";
			}
		}
		this.PassContactInfoToAllVessels();
		if (this.currentAttack != "SONOBUOY" && flag3 && this.sonobuoyTimer <= 0f)
		{
			this.sonobuoyTimer = UnityEngine.Random.Range(6f, 12f);
		}
		string text = this.currentAttack;
		switch (text)
		{
		case "TORPEDO":
			this.attackDist = UnityEngine.Random.Range(2f, 8f);
			break;
		case "DEPTHBOMB":
		{
			float num3 = base.transform.position.y - 1000f;
			float num4 = num3 / 0.4f;
			this.attackDist = num4 * this.airspeed;
			break;
		}
		case "SONOBUOY":
			this.attackDist = UnityEngine.Random.Range(2f, 8f);
			break;
		}
		return result;
	}

	// Token: 0x06000636 RID: 1590 RVA: 0x0002C634 File Offset: 0x0002A834
	private float GetDistanceToNearestSonobuoy(bool convertToYards = true)
	{
		float num = 20000f;
		for (int i = 0; i < this.sensordata.sonobuoys.Length; i++)
		{
			float xzdistance = UIFunctions.globaluifunctions.GetXZDistance(base.transform.position, this.sensordata.sonobuoys[i].transform.position);
			if (xzdistance < num)
			{
				num = xzdistance;
			}
		}
		if (convertToYards)
		{
			num *= GameDataManager.yardsScale;
		}
		return num;
	}

	// Token: 0x06000637 RID: 1591 RVA: 0x0002C6AC File Offset: 0x0002A8AC
	private void DropSonoBuoy()
	{
		this.dropSonobuoy = false;
		if (this.GetDistanceToNearestSonobuoy(true) < 2000f)
		{
			return;
		}
		this.sonobuoysDropped++;
		this.sonobuoysOnBoard[this.sonobuoyTypeToDrop]--;
		GameObject gameObject = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.databaseweapondata[UIFunctions.globaluifunctions.database.aerialSonobuoyID].weaponObject, base.transform.position, base.transform.rotation) as GameObject;
		gameObject.SetActive(true);
		Torpedo component = gameObject.GetComponent<Torpedo>();
		component.databaseweapondata = UIFunctions.globaluifunctions.database.databaseweapondata[UIFunctions.globaluifunctions.database.aerialSonobuoyID];
		component.InitialiseTorpedo();
		UIFunctions.globaluifunctions.playerfunctions.eventcamera.CheckForEventCamera(gameObject.transform, GameDataManager.playervesselsonlevel[0].transform, 10f, true, false, false, component.databaseweapondata.minCameraDistance + 1f, component.databaseweapondata.minCameraDistance, -1f, true);
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.sonosInFlight.Add(component);
		if (ManualCameraZoom.target == base.transform)
		{
			ManualCameraZoom.target = gameObject.transform;
		}
		component.isAirborne = true;
		component.actualCurrentSpeed = this.airspeed;
		component.sensorData = this.sensordata;
		if (this.sonobuoyTypeToDrop == 1)
		{
			component.isActiveSonobuoy = true;
		}
	}

	// Token: 0x06000638 RID: 1592 RVA: 0x0002C830 File Offset: 0x0002AA30
	private void DropDepthBomb()
	{
		UIFunctions.globaluifunctions.playerfunctions.eventcamera.CheckForEventCamera(base.transform, GameDataManager.playervesselsonlevel[0].transform, 10f, true, false, false, 2f, 2f, -1f, true);
		this.depthBombsDropped++;
		this.ClearSensorData();
		this.dropPayload = false;
		this.depthBombsOnBoard--;
		GameObject gameObject = ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.databasedepthchargedata[this.databaseaircraftdata.depthBombIDs[0]].depthChargeObject, base.transform.position, base.transform.rotation);
		gameObject.SetActive(true);
		Projectile_DepthCharge component = gameObject.GetComponent<Projectile_DepthCharge>();
		component.depthChargeID = this.databaseaircraftdata.depthBombIDs[0];
		component.velocity = UIFunctions.globaluifunctions.database.databasedepthchargedata[component.depthChargeID].velocity;
		component.sinkSpeed = UIFunctions.globaluifunctions.database.databasedepthchargedata[component.depthChargeID].sinkRate;
		component.isDepthExploded = UIFunctions.globaluifunctions.database.databasedepthchargedata[component.depthChargeID].depthExploded;
		component.isContactExploded = UIFunctions.globaluifunctions.database.databasedepthchargedata[component.depthChargeID].contactExploded;
		component.yDown = -0.4f;
		component.velocity = this.airspeed;
		component.amPooled = false;
		if (ManualCameraZoom.target == base.transform)
		{
			ManualCameraZoom.target = gameObject.transform;
		}
		if (this.sonobuoyTimer <= 0f)
		{
			this.sonobuoyTimer = UnityEngine.Random.Range(8f, 16f);
		}
	}

	// Token: 0x06000639 RID: 1593 RVA: 0x0002C9EC File Offset: 0x0002ABEC
	private void DropTorpedo()
	{
		this.dropPayload = false;
		this.torpedoesOnBoard--;
		GameObject gameObject = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.databaseweapondata[this.databaseaircraftdata.torpedoIDs[0]].weaponObject, base.transform.position, base.transform.rotation) as GameObject;
		UIFunctions.globaluifunctions.playerfunctions.eventcamera.CheckForEventCamera(gameObject.transform, null, 10f, true, false, true, -1f, -1f, -1f, true);
		gameObject.SetActive(true);
		Torpedo component = gameObject.GetComponent<Torpedo>();
		component.databaseweapondata = UIFunctions.globaluifunctions.database.databaseweapondata[this.databaseaircraftdata.torpedoIDs[0]];
		component.noSurfaceTargets = UIFunctions.globaluifunctions.combatai.AreHostileShipsInArea();
		component.InitialiseTorpedo();
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.AddTorpedoToArray(component);
		if (ManualCameraZoom.target == base.transform)
		{
			ManualCameraZoom.target = gameObject.transform;
		}
		component.isAirborne = true;
		component.actualCurrentSpeed = this.airspeed;
		float num = 1000f - UIFunctions.globaluifunctions.levelloadmanager.GetTerrainHeightAtPosition(base.transform.position.x, base.transform.position.z);
		if (num < 998f)
		{
			num = 998f;
		}
		float num2 = UnityEngine.Random.Range(num, 999.5f);
		if (this.sensordata.radarTotalDetected > 0f)
		{
			component.searchYValue = 999.6f;
		}
		component.cruiseYValue = num2;
		component.searchYValue = num2;
		this.ClearSensorData();
		if (this.sonobuoyTimer <= 0f)
		{
			this.sonobuoyTimer = UnityEngine.Random.Range(8f, 16f);
		}
	}

	// Token: 0x0600063A RID: 1594 RVA: 0x0002CBD4 File Offset: 0x0002ADD4
	private void ClearSensorData()
	{
		this.sensordata.decibelsTotalDetected = 0f;
		this.sensordata.decibelsLastDetected = 0f;
		this.sensordata.radarTotalDetected = 0f;
		this.sensordata.timeTrackingPlayer = 0f;
	}

	// Token: 0x04000675 RID: 1653
	public DatabaseAircraftData databaseaircraftdata;

	// Token: 0x04000676 RID: 1654
	public SensorData sensordata;

	// Token: 0x04000677 RID: 1655
	public float airspeed;

	// Token: 0x04000678 RID: 1656
	public int aircraftInstanceId;

	// Token: 0x04000679 RID: 1657
	public Transform meshHolder;

	// Token: 0x0400067A RID: 1658
	public Transform waypoint;

	// Token: 0x0400067B RID: 1659
	public Transform navigator;

	// Token: 0x0400067C RID: 1660
	public float startcameradist;

	// Token: 0x0400067D RID: 1661
	public Vector2 turningParameters;

	// Token: 0x0400067E RID: 1662
	public float defaultCruiseAltitude;

	// Token: 0x0400067F RID: 1663
	public Vector2 cruiseAltitude;

	// Token: 0x04000680 RID: 1664
	public float timePitching;

	// Token: 0x04000681 RID: 1665
	public bool isTerrainPresent;

	// Token: 0x04000682 RID: 1666
	public float terrainScanTimer;

	// Token: 0x04000683 RID: 1667
	public Transform raycastPosition;

	// Token: 0x04000684 RID: 1668
	public int currentWaypoint;

	// Token: 0x04000685 RID: 1669
	public bool attackPlayer;

	// Token: 0x04000686 RID: 1670
	public bool OnAttackRun;

	// Token: 0x04000687 RID: 1671
	public string currentAttack;

	// Token: 0x04000688 RID: 1672
	public bool dropPayload;

	// Token: 0x04000689 RID: 1673
	public bool dropSonobuoy;

	// Token: 0x0400068A RID: 1674
	public float attackDist;

	// Token: 0x0400068B RID: 1675
	public float depthBombTimer;

	// Token: 0x0400068C RID: 1676
	public int depthBombsDropped;

	// Token: 0x0400068D RID: 1677
	public float sonobuoyTimer;

	// Token: 0x0400068E RID: 1678
	public int torpedoesOnBoard;

	// Token: 0x0400068F RID: 1679
	public int depthBombsOnBoard;

	// Token: 0x04000690 RID: 1680
	public string aircraftRole;

	// Token: 0x04000691 RID: 1681
	public int[] sonobuoysOnBoard;

	// Token: 0x04000692 RID: 1682
	public int sonobuoyTypeToDrop;

	// Token: 0x04000693 RID: 1683
	public AudioSource audiosource;

	// Token: 0x04000694 RID: 1684
	public Transform transformToAttack;

	// Token: 0x04000695 RID: 1685
	public bool doOnce;

	// Token: 0x04000696 RID: 1686
	public bool attackPlayerActualTransform;

	// Token: 0x04000697 RID: 1687
	public float straightRunTimer;

	// Token: 0x04000698 RID: 1688
	public int sonobuoysDropped;

	// Token: 0x04000699 RID: 1689
	public float lastAttackRunTimer;
}
