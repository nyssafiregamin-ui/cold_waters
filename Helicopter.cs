using System;
using UnityEngine;

// Token: 0x02000124 RID: 292
public class Helicopter : MonoBehaviour
{
	// Token: 0x060007BF RID: 1983 RVA: 0x000490FC File Offset: 0x000472FC
	private void Start()
	{
		this.relativeFactors = new Vector2(this.relativefactor, this.relativefactor / 3f);
		this.upFactors = new Vector2(this.upfactor, this.upfactor / 3f);
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
		this.torpedoesOnBoard = this.databaseaircraftdata.torpedoNumbers[0];
		this.depthBombsOnBoard = this.databaseaircraftdata.depthBombs[0];
		this.destroyed = false;
		this.throttleon = false;
		this.landing = false;
		this.target = Quaternion.identity;
		this.maxVelocity = Mathf.Sqrt(this.sqrMaxVelocity);
		this.audiosource.Play();
		if (!UIFunctions.globaluifunctions.combatai.usingFormationAI)
		{
			this.GetNextPointInSearchArea();
		}
		this.currentAttack = "UNDETERMINED";
		this.doNotDip = false;
	}

	// Token: 0x060007C0 RID: 1984 RVA: 0x00049274 File Offset: 0x00047474
	public void PlaceInitialWaypoint()
	{
		this.doNotDip = false;
		if (this.sensordata.decibelsLastDetected > 25f || this.sensordata.decibelsTotalDetected > 150f)
		{
			this.attackPlayer = true;
			this.transformToAttack = UIFunctions.globaluifunctions.combatai.GetActualPositionToAttack(base.transform);
			if (this.transformToAttack == null)
			{
				this.attackPlayer = false;
			}
			return;
		}
		if (this.sensordata.decibelsLastDetected > 15f)
		{
			this.SetAttackWaypoint();
			return;
		}
		this.waypointTransform.SetParent(UIFunctions.globaluifunctions.combatai.levelloadmanager.levelloaddata.formationGrid.transform, false);
		this.waypointTransform.transform.localPosition = Vector3.zero;
		if (this.pathFinding.Contains("FRONT"))
		{
			this.waypointTransform.transform.localPosition = new Vector3(UnityEngine.Random.Range(UIFunctions.globaluifunctions.combatai.formationBounds.x, UIFunctions.globaluifunctions.combatai.formationBounds.y), 0f, UIFunctions.globaluifunctions.combatai.formationBounds.z + UnityEngine.Random.Range(30f, 50f));
			if (this.pathFinding.Contains("LEFT"))
			{
				this.waypointTransform.transform.Translate(Vector3.right * (UIFunctions.globaluifunctions.combatai.formationBounds.x + UnityEngine.Random.Range(30f, 50f)));
			}
			else if (this.pathFinding.Contains("RIGHT"))
			{
				this.waypointTransform.transform.Translate(Vector3.right * (UIFunctions.globaluifunctions.combatai.formationBounds.y + UnityEngine.Random.Range(30f, 50f)));
			}
		}
		else if (this.pathFinding.Contains("LEFT"))
		{
			this.waypointTransform.transform.localPosition = new Vector3(UIFunctions.globaluifunctions.combatai.formationBounds.x - UnityEngine.Random.Range(30f, 50f), 0f, UnityEngine.Random.Range(-30f, 30f));
		}
		else if (this.pathFinding.Contains("RIGHT"))
		{
			this.waypointTransform.transform.localPosition = new Vector3(UIFunctions.globaluifunctions.combatai.formationBounds.y + UnityEngine.Random.Range(30f, 50f), 0f, UnityEngine.Random.Range(-30f, 30f));
		}
		if (this.isTerrainPresent)
		{
			this.doNotDip = !UIFunctions.globaluifunctions.combatai.CheckWaterLocationIsValid(this.waypointTransform.position);
		}
		this.waypoint = this.waypointTransform;
		this.waypointTransform.parent = null;
	}

	// Token: 0x060007C1 RID: 1985 RVA: 0x00049588 File Offset: 0x00047788
	private void GetNextWaypoint()
	{
		this.doNotDip = false;
		if (this.searchingTempArea)
		{
			this.GetNextPointInTempSearchArea();
			return;
		}
		if (!UIFunctions.globaluifunctions.combatai.usingFormationAI)
		{
			this.GetNextPointInSearchArea();
		}
		else
		{
			this.PlaceInitialWaypoint();
		}
	}

	// Token: 0x060007C2 RID: 1986 RVA: 0x000495D4 File Offset: 0x000477D4
	private void GetNearbyWaypoint()
	{
		float num = UnityEngine.Random.Range(10f, 15f);
		float num2 = UnityEngine.Random.Range(10f, 15f);
		if (UnityEngine.Random.value < 0.5f)
		{
			num *= -1f;
		}
		if (UnityEngine.Random.value < 0.5f)
		{
			num2 *= -1f;
		}
		this.waypoint.transform.Translate(new Vector3(num, 0f, num2));
	}

	// Token: 0x060007C3 RID: 1987 RVA: 0x0004964C File Offset: 0x0004784C
	private void GetNextPointInSearchArea()
	{
		int num = 0;
		do
		{
			Vector3 vector = UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.aircraftSearchAreas[this.helicopterIndex];
			Vector2 vector2 = UnityEngine.Random.insideUnitCircle * vector.z;
			this.waypoint.transform.position = new Vector3(vector.x + vector2.x, 1000f, vector.y + vector2.y);
			num++;
		}
		while (UIFunctions.globaluifunctions.GetXZDistance(base.transform.position, this.waypoint.position) < 10f && num < 10);
	}

	// Token: 0x060007C4 RID: 1988 RVA: 0x00049700 File Offset: 0x00047900
	private void GetNextPointInTempSearchArea()
	{
		int num = 0;
		do
		{
			Vector3 vector = this.searchingTempAreaPosition;
			Vector2 vector2 = UnityEngine.Random.insideUnitCircle * 30f;
			this.waypoint.transform.position = new Vector3(vector.x + vector2.x, 1000f, vector.z + vector2.y);
			num++;
		}
		while (UIFunctions.globaluifunctions.GetXZDistance(base.transform.position, this.waypoint.position) < 10f && num < 10);
	}

	// Token: 0x060007C5 RID: 1989 RVA: 0x00049794 File Offset: 0x00047994
	private void Navigation()
	{
		float num = 0f;
		float num2 = 0f;
		if (this.currentAction == 0)
		{
			if (this.waypoint != null)
			{
				Vector3 normalized = (base.transform.position - this.waypoint.position).normalized;
				float xzdistance = UIFunctions.globaluifunctions.GetXZDistance(base.transform.position, this.waypoint.position);
				if (xzdistance > 3f)
				{
					num = normalized.x * this.clampmax;
					num2 = normalized.z * this.clampmax;
				}
				else
				{
					if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.GetDistanceToNearestEnemyVessel(base.transform, true, true) < 5000f)
					{
						this.searchingTempArea = true;
						this.numberOfTempSearches = UnityEngine.Random.Range(1, 5);
						this.searchingTempAreaPosition = new Vector3(base.transform.position.x + (float)UnityEngine.Random.Range(-5, 5), 1000f, base.transform.position.z + (float)UnityEngine.Random.Range(-5, 5));
						this.GetNextWaypoint();
						return;
					}
					if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.GetDistanceToNearestEnemyHelicopter(base.transform, true) < 2000f)
					{
						this.searchingTempArea = true;
						this.numberOfTempSearches = UnityEngine.Random.Range(1, 5);
						this.searchingTempAreaPosition = new Vector3(base.transform.position.x + (float)UnityEngine.Random.Range(-5, 5), 1000f, base.transform.position.z + (float)UnityEngine.Random.Range(-5, 5));
						this.GetNextWaypoint();
						return;
					}
					if (this.doNotDip)
					{
						this.GetNextWaypoint();
						return;
					}
					this.lerpTime = 3f;
					this.currentLerpTime = 0f;
					this.lerpXValue = this.rotationX;
					this.lerpZValue = this.rotationZ;
					this.currentAction = 1;
					return;
				}
			}
			if (this.rotationX < num)
			{
				this.rotationX += Time.deltaTime * 1f;
			}
			else if (this.rotationX > num)
			{
				this.rotationX -= Time.deltaTime * 1f;
			}
			if (this.rotationZ < num2)
			{
				this.rotationZ += Time.deltaTime;
			}
			else if (this.rotationZ > num2)
			{
				this.rotationZ -= Time.deltaTime;
			}
		}
		else if (this.currentAction == 1)
		{
			this.currentLerpTime += Time.deltaTime;
			if (this.currentLerpTime > this.lerpTime)
			{
				this.currentLerpTime = this.lerpTime;
			}
			float num3 = this.currentLerpTime / this.lerpTime;
			num3 = num3 * num3 * num3 * (num3 * (6f * num3 - 15f) + 10f);
			this.rotationX = Mathf.Lerp(this.lerpXValue, 0f, num3);
			this.rotationZ = Mathf.Lerp(this.lerpZValue, 0f, num3);
			if (this.currentLerpTime == this.lerpTime)
			{
				if (!UIFunctions.globaluifunctions.combatai.CheckWaterLocationIsValid(this.waypoint.position))
				{
					this.currentAction = 0;
					this.GetNextWaypoint();
				}
				else
				{
					this.currentAction = 2;
					this.currentaltitude = this.dipaltitude;
				}
			}
		}
		else if (this.currentAction == 2 && base.transform.position.y < 1000.26f)
		{
			this.currentAction = 3;
			this.movingSonar = -1;
		}
		this.rotationX = Mathf.Clamp(this.rotationX, -this.clampmax, this.clampmax);
		this.rotationZ = Mathf.Clamp(this.rotationZ, -this.clampmax, this.clampmax);
	}

	// Token: 0x060007C6 RID: 1990 RVA: 0x00049B9C File Offset: 0x00047D9C
	private void CheckDippingSonar()
	{
		int num = -1;
		if (this.databaseaircraftdata.passiveSonarID != -1 && this.databaseaircraftdata.activeSonarID != -1)
		{
			num = 2;
		}
		else if (this.databaseaircraftdata.activeSonarID != -1)
		{
			num = 1;
		}
		else if (this.databaseaircraftdata.passiveSonarID != -1)
		{
			num = 0;
		}
		if (num != -1)
		{
			this.sensordata.lastDipDetected = UIFunctions.globaluifunctions.playerfunctions.sensormanager.CheckSingleSonar(base.gameObject, GameDataManager.playervesselsonlevel[0].gameObject, this.databaseaircraftdata.passiveSonarID, this.databaseaircraftdata.activeSonarID, num, this.sensordata, false, null);
			for (int i = 0; i < UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects.Length; i++)
			{
				if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[i].databaseweapondata.weaponType == "DECOY" && UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[i].whichNavy == 0)
				{
					this.sensordata.lastDipDetected = UIFunctions.globaluifunctions.playerfunctions.sensormanager.CheckSingleSonar(base.gameObject, UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[i].gameObject, this.databaseaircraftdata.passiveSonarID, this.databaseaircraftdata.activeSonarID, num, this.sensordata, false, null);
				}
			}
			if (this.sensordata.decibelsLastDetected > UnityEngine.Random.Range(20f, 30f) && this.torpedoesOnBoard > 0 && this.currentAttackTimer <= 0f && Vector3.Distance(base.transform.position, GameDataManager.playervesselsonlevel[0].transform.position) < 2000f * GameDataManager.inverseYardsScale)
			{
				this.DropTorpedo();
				this.currentAttackTimer = UnityEngine.Random.Range(5f, 20f);
			}
			if (this.sensordata.decibelsLastDetected > 0f)
			{
				this.searchingTempArea = true;
				this.numberOfTempSearches = UnityEngine.Random.Range(4, 9);
				this.searchingTempAreaPosition = new Vector3(base.transform.position.x + (float)UnityEngine.Random.Range(-5, 5), 1000f, base.transform.position.z + (float)UnityEngine.Random.Range(-5, 5));
			}
		}
	}

	// Token: 0x060007C7 RID: 1991 RVA: 0x00049E28 File Offset: 0x00048028
	private void SetAttackWaypoint()
	{
		if (this.transformToAttack == null)
		{
			this.attackPlayer = false;
			this.OnAttackRun = false;
			this.GetNextWaypoint();
			return;
		}
		this.waypoint.parent = null;
		this.waypoint.transform.position = this.transformToAttack.position;
		float num = 0f;
		if (this.helicopterRole == "HUNTER")
		{
			num = 16f;
		}
		float num2 = this.sensordata.decibelsTotalDetected;
		if (num2 > 100f)
		{
			num2 = 100f;
		}
		num2 = 100f - num2;
		num2 /= 15f;
		num2 += 1f;
		this.waypoint.transform.Translate(Vector3.forward * (UnityEngine.Random.Range(-num2, num2) + num));
		this.waypoint.transform.Translate(Vector3.right * (UnityEngine.Random.Range(-num2, num2) + num));
		float d = 1000f - this.waypoint.transform.position.y;
		this.waypoint.transform.Translate(Vector3.up * d);
	}

	// Token: 0x060007C8 RID: 1992 RVA: 0x00049F5C File Offset: 0x0004815C
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

	// Token: 0x060007C9 RID: 1993 RVA: 0x0004A028 File Offset: 0x00048228
	private void SetPlayerPositionAsWaypoint()
	{
		float num = 1000f - GameDataManager.playervesselsonlevel[0].transform.position.y;
		if (num < 0.3f)
		{
			float d = num * 5f + 5f;
			Transform transform = this.transformToAttack;
			if (this.transformToAttack == null)
			{
				this.attackPlayer = false;
				this.OnAttackRun = false;
				this.GetNextWaypoint();
				return;
			}
			Vector3 position = transform.position + transform.forward * GameDataManager.playervesselsonlevel[0].vesselmovement.shipSpeed.z * GameDataManager.globalTranslationSpeed * d;
			position.y = 1000f;
			this.waypoint.transform.position = position;
		}
	}

	// Token: 0x060007CA RID: 1994 RVA: 0x0004A0F8 File Offset: 0x000482F8
	private void ScanForTerrain()
	{
		if (this.currentAction == 0)
		{
			int layerMask = 1073741824;
			this.raycastPosition.transform.localRotation = Quaternion.Euler(-8.1f, 0f, 0f);
			RaycastHit raycastHit;
			if (Physics.Raycast(this.raycastPosition.position, this.raycastPosition.forward, out raycastHit, 30f, layerMask))
			{
				this.currentaltitude += 0.2f;
				if (raycastHit.distance < 15f)
				{
					Vector3 position = base.transform.position + base.transform.forward * -10f;
					position.y = 1000f;
					this.waypoint.position = position;
					this.doNotDip = true;
				}
				this.throttleon = true;
				this.throttleTimer = 0f;
			}
			else if (Physics.Raycast(this.raycastPosition.position, -Vector3.up, out raycastHit, 1f, layerMask))
			{
				this.currentaltitude += 0.2f;
				this.throttleon = true;
				this.throttleTimer = 0f;
			}
			else if (this.currentaltitude > this.cruisealtitude + 0.1f)
			{
				this.currentaltitude -= 0.005f;
			}
		}
		this.terrainScanTimer = 0f;
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x0004A268 File Offset: 0x00048468
	private void FixedUpdate()
	{
		if (Time.timeScale == 1f)
		{
			this.relativefactor = this.relativeFactors[0];
			this.upfactor = this.upFactors[0];
		}
		else
		{
			this.relativefactor = this.relativeFactors[1];
			this.upfactor = this.upFactors[1];
		}
		if (!this.doOnce)
		{
			this.doOnce = true;
			float num = UIFunctions.globaluifunctions.levelloadmanager.GetTerrainHeightAtPosition(base.transform.position.x, base.transform.position.z);
			if (num < 0f)
			{
				num -= 3f;
				base.transform.Translate(Vector3.up * -num);
			}
		}
		if (this.throttleon)
		{
			this.throttleTimer += Time.deltaTime;
			if (this.throttleTimer > 3f)
			{
				this.throttleon = false;
				this.throttleTimer = 0f;
			}
			this.helicopterrigidbody.AddRelativeForce(base.transform.up * 3000f * Time.deltaTime);
		}
		if (this.currentAction != 0)
		{
			this.avoidShipsTimer += Time.deltaTime;
			if (this.avoidShipsTimer > 4f)
			{
				this.avoidShipsTimer = 0f;
				float distanceToNearestEnemyVessel = UIFunctions.globaluifunctions.playerfunctions.sensormanager.GetDistanceToNearestEnemyVessel(base.transform, true, true);
				if (distanceToNearestEnemyVessel < 4000f)
				{
					this.FinishDipping();
				}
			}
		}
		else if (this.isTerrainPresent)
		{
			this.terrainScanTimer += Time.deltaTime;
			if (this.terrainScanTimer > 1f)
			{
				this.ScanForTerrain();
			}
		}
		if (this.dropSonobuoy)
		{
			this.DropSonobuoy();
			this.dropSonobuoy = false;
		}
		if (this.dropTorpedo)
		{
			this.DropTorpedo();
			this.dropTorpedo = false;
		}
		this.helibody.transform.localPosition = new Vector3(0f, GameDataManager.xclock / this.yClockFactor, 0f);
		if (this.currentAttackTimer > 0f)
		{
			this.currentAttackTimer -= Time.deltaTime;
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
				this.depthBombsDropped = 0;
				this.depthBombTimer = 0f;
			}
		}
		if (this.helicopterRole == "HUNTER" && this.attackPlayer)
		{
			this.attackPlayer = false;
		}
		if (this.attackPlayer)
		{
			this.attackPlayer = false;
			this.transformToAttack = UIFunctions.globaluifunctions.combatai.GetActualPositionToAttack(base.transform);
			this.SetAttackWaypoint();
			this.OnAttackRun = true;
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
				if (!this.OnAttackRun)
				{
					this.attackPlayer = true;
				}
			}
		}
		if (this.OnAttackRun)
		{
			if (this.attackPlayerActualTransform)
			{
				this.SetPlayerPositionAsWaypoint();
			}
			Vector3 position = base.transform.position;
			position.y = 1000f;
			float xzdistance = UIFunctions.globaluifunctions.GetXZDistance(position, this.waypoint.transform.position);
			if (this.currentAttack == "UNDETERMINED" && this.currentAttackTimer <= 0f)
			{
				if (xzdistance < 10f && (UIFunctions.globaluifunctions.playerfunctions.sensormanager.HaveVisualOnPlayerSub(base.transform.position) || this.attackPlayerActualTransform))
				{
					this.attackPlayerActualTransform = true;
					this.GetAttackType();
				}
				else if (xzdistance < 3f)
				{
					this.GetAttackType();
				}
			}
			if (xzdistance < this.attackDist)
			{
				float distanceToNearestEnemyVessel2 = UIFunctions.globaluifunctions.playerfunctions.sensormanager.GetDistanceToNearestEnemyVessel(base.transform, true, true);
				if (distanceToNearestEnemyVessel2 < 2500f)
				{
					float num2 = UnityEngine.Random.Range(10f, 15f);
					float num3 = UnityEngine.Random.Range(10f, 15f);
					if (UnityEngine.Random.value < 0.5f)
					{
						num2 *= -1f;
					}
					if (UnityEngine.Random.value < 0.5f)
					{
						num3 *= -1f;
					}
					Vector3 position2 = base.transform.position + base.transform.forward * num3;
					position2 = base.transform.position + base.transform.right * num2;
					position2.y = 1000f;
					this.waypoint.position = position2;
				}
				else
				{
					string text = this.currentAttack;
					switch (text)
					{
					case "TORPEDO":
						this.DropTorpedo();
						this.currentAttackTimer = UnityEngine.Random.Range(5f, 20f);
						break;
					case "DEPTHBOMB":
						this.depthBombsDropped = 1;
						this.currentAttackTimer = UnityEngine.Random.Range(5f, 20f);
						break;
					case "SONOBUOY":
						this.DropSonobuoy();
						break;
					case "DIP":
						this.lerpTime = 3f;
						this.currentLerpTime = 0f;
						this.lerpXValue = this.rotationX;
						this.lerpZValue = this.rotationZ;
						this.currentAction = 1;
						break;
					}
					this.sensordata.decibelsLastDetected = 0f;
					this.sensordata.decibelsTotalDetected = 0f;
					this.sensordata.radarTotalDetected = 0f;
				}
				this.OnAttackRun = false;
				this.currentAttack = "UNDETERMINED";
				this.attackPlayerActualTransform = false;
			}
		}
		if (this.movingSonar != 0)
		{
			this.sonarheight += 0.02f * Time.deltaTime * (float)this.movingSonar;
			this.sonarLine.SetPosition(1, new Vector3(0f, this.sonarheight, 0f));
			if (this.sonarheight < -0.35f && this.movingSonar == -1)
			{
				this.movingSonar = 0;
				this.listeningTimer = UnityEngine.Random.Range(30f, 45f);
				this.CheckDippingSonar();
			}
			else if (this.sonarheight > 0f && this.movingSonar == 1)
			{
				this.movingSonar = 0;
				this.sonarheight = 0f;
				this.currentAction = 0;
				this.currentLerpTime = 0f;
				this.lerpTime = 1f;
				this.currentaltitude = this.cruisealtitude;
				this.GetNextWaypoint();
			}
		}
		if (base.transform.position.y < 1000.5f)
		{
			if (!this.hoverparticle.isPlaying)
			{
				this.hoverparticle.Play();
			}
			this.hoverparticle.transform.localPosition = new Vector3(0f, this.hoverparticle.transform.localPosition.y, 0f);
		}
		else if (this.hoverparticle.isPlaying)
		{
			this.hoverparticle.Stop();
		}
		if (this.listeningTimer > 0f)
		{
			this.listeningTimer -= Time.deltaTime;
			if (this.listeningTimer < 0f)
			{
				this.FinishDipping();
			}
		}
		this.Navigation();
		this.MoveHelicopter();
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x0004AAFC File Offset: 0x00048CFC
	private void FinishDipping()
	{
		this.OnAttackRun = false;
		this.CheckDippingSonar();
		this.movingSonar = 1;
		if (this.searchingTempArea)
		{
			this.numberOfTempSearches--;
			if (this.numberOfTempSearches == 0)
			{
				this.searchingTempArea = false;
			}
		}
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x0004AB48 File Offset: 0x00048D48
	private bool CanUseDepthBombs()
	{
		float num = 1000f - this.waypoint.transform.position.y;
		return num < 0.6f && this.depthBombsOnBoard > 0 && GameDataManager.playervesselsonlevel[0].vesselmovement.shipSpeed.z < 15f && UIFunctions.globaluifunctions.GetXZDistance(base.transform.position, GameDataManager.playervesselsonlevel[0].transform.position) > 6f;
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x0004ABE0 File Offset: 0x00048DE0
	private string GetAttackType()
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (this.CanUseDepthBombs())
		{
			flag = true;
		}
		if (this.torpedoesOnBoard > 0)
		{
			flag2 = true;
		}
		this.sonobuoyTypeToDrop = this.GetSonobuoyTypeToDrop();
		if (this.sonobuoyTypeToDrop != -1)
		{
			flag3 = true;
		}
		string result = "UNDETERMINED";
		if (this.attackPlayerActualTransform)
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
			else
			{
				this.currentAttack = "DIP";
			}
		}
		else if (this.sensordata.decibelsTotalDetected > 150f && this.sensordata.decibelsLastDetected > 5f)
		{
			if (flag2)
			{
				this.currentAttack = "TORPEDO";
			}
			else if (flag3)
			{
				this.currentAttack = "SONOBUOY";
			}
			else
			{
				this.currentAttack = "DIP";
			}
		}
		else
		{
			this.currentAttack = "DIP";
		}
		string text = this.currentAttack;
		switch (text)
		{
		case "TORPEDO":
			this.attackDist = UnityEngine.Random.Range(3f, 8f);
			break;
		case "DEPTHBOMB":
			this.attackDist = 2.925f + UnityEngine.Random.Range(-0.3f, 0.3f);
			break;
		case "SONOBUOY":
			this.attackDist = UnityEngine.Random.Range(0f, 4f);
			break;
		case "DIP":
			this.attackDist = UnityEngine.Random.Range(0f, 5f);
			break;
		}
		return result;
	}

	// Token: 0x060007CF RID: 1999 RVA: 0x0004AE24 File Offset: 0x00049024
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

	// Token: 0x060007D0 RID: 2000 RVA: 0x0004AEE8 File Offset: 0x000490E8
	public void MoveHelicopter()
	{
		float x = Mathf.Clamp(base.transform.rotation.x, this.minimumX, this.maximumX);
		float z = Mathf.Clamp(base.transform.rotation.z, this.minimumZ, this.maximumZ);
		base.transform.rotation = Quaternion.Euler(x, base.transform.eulerAngles.y, z);
		this.target = Quaternion.Euler(-this.rotationZ, 0f, this.rotationX);
		base.transform.rotation = this.target;
		Vector3 vector = new Vector3(-this.rotationX, 0f, -this.rotationZ);
		this.xzvector = vector.normalized;
		if (this.xzvector != Vector3.zero)
		{
			Quaternion to = Quaternion.LookRotation(this.xzvector);
			float maxDegreesDelta = this.turnspeed * Time.deltaTime;
			this.helibody.transform.rotation = Quaternion.RotateTowards(this.helibody.transform.rotation, to, maxDegreesDelta);
			this.helibody.transform.localEulerAngles = new Vector3(0f, this.helibody.transform.localEulerAngles.y, 0f);
		}
		Vector3 vector2 = this.helicopterrigidbody.velocity;
		float sqrMagnitude = vector2.sqrMagnitude;
		float num = 1f - (base.transform.position.y - this.currentaltitude) / this.levelheight;
		Vector3 a = -Physics.gravity * (num - this.helicopterrigidbody.velocity.y * this.dampening);
		this.helicopterrigidbody.AddRelativeForce(a * this.relativefactor * Time.deltaTime);
		this.helicopterrigidbody.AddForce(a * this.upfactor * Time.deltaTime);
		vector2 = new Vector3(vector2.x, 0f, vector2.z);
		this.helicopterrigidbody.AddRelativeForce(Vector3.up * sqrMagnitude * 25f * Time.deltaTime);
		if (sqrMagnitude > this.maxVelocity * this.maxVelocity)
		{
			vector2 = vector2.normalized * this.maxVelocity;
			this.helicopterrigidbody.velocity = new Vector3(vector2.x, this.helicopterrigidbody.velocity.y, vector2.z);
		}
		if (this.helicopterrigidbody.velocity.y < -this.maxDownVelocity)
		{
			this.helicopterrigidbody.velocity = new Vector3(this.helicopterrigidbody.velocity.x, -this.maxDownVelocity, this.helicopterrigidbody.velocity.z);
		}
		else if (this.helicopterrigidbody.velocity.y > this.maxDownVelocity)
		{
			this.helicopterrigidbody.velocity = new Vector3(this.helicopterrigidbody.velocity.x, this.maxDownVelocity, this.helicopterrigidbody.velocity.z);
		}
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x0004B25C File Offset: 0x0004945C
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

	// Token: 0x060007D2 RID: 2002 RVA: 0x0004B2D4 File Offset: 0x000494D4
	private void DropSonobuoy()
	{
		this.sonobuoysOnBoard[this.sonobuoyTypeToDrop]--;
		this.dropSonobuoy = false;
		if (this.GetDistanceToNearestSonobuoy(true) < 2000f)
		{
			return;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.databaseweapondata[UIFunctions.globaluifunctions.database.aerialSonobuoyID].weaponObject, this.helibody.position, this.helibody.rotation) as GameObject;
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
		component.actualCurrentSpeed = this.helibody.InverseTransformDirection(this.helicopterrigidbody.velocity).z;
		component.sensorData = this.sensordata;
		if (this.sonobuoyTypeToDrop == 1)
		{
			component.isActiveSonobuoy = true;
		}
	}

	// Token: 0x060007D3 RID: 2003 RVA: 0x0004B464 File Offset: 0x00049664
	private void DropDepthBomb()
	{
		UIFunctions.globaluifunctions.playerfunctions.eventcamera.CheckForEventCamera(base.transform, GameDataManager.playervesselsonlevel[0].transform, 10f, true, false, false, 2f, 2f, -1f, true);
		this.depthBombsDropped++;
		this.ClearSensorData();
		this.dropPayload = false;
		this.depthBombsOnBoard--;
		GameObject gameObject = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.databasedepthchargedata[this.databaseaircraftdata.depthBombIDs[0]].depthChargeObject, this.helibody.position, this.helibody.rotation) as GameObject;
		gameObject.SetActive(true);
		Projectile_DepthCharge component = gameObject.GetComponent<Projectile_DepthCharge>();
		component.depthChargeID = this.databaseaircraftdata.depthBombIDs[0];
		component.sinkSpeed = UIFunctions.globaluifunctions.database.databasedepthchargedata[component.depthChargeID].sinkRate;
		component.isDepthExploded = UIFunctions.globaluifunctions.database.databasedepthchargedata[component.depthChargeID].depthExploded;
		component.isContactExploded = UIFunctions.globaluifunctions.database.databasedepthchargedata[component.depthChargeID].contactExploded;
		component.yDown = -0.4f;
		component.dcrigidbody.velocity = Vector3.zero;
		component.velocity = this.helibody.InverseTransformDirection(this.helicopterrigidbody.velocity).z;
		component.amPooled = false;
		if (ManualCameraZoom.target == base.transform)
		{
			ManualCameraZoom.target = gameObject.transform;
		}
	}

	// Token: 0x060007D4 RID: 2004 RVA: 0x0004B608 File Offset: 0x00049808
	private void DropTorpedo()
	{
		this.dropPayload = false;
		this.torpedoesOnBoard--;
		if (this.torpedoesOnBoard < 0)
		{
			return;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.databaseweapondata[this.databaseaircraftdata.torpedoIDs[0]].weaponObject, this.helibody.position, this.helibody.rotation) as GameObject;
		this.HideWeaponHardpoint(gameObject);
		gameObject.SetActive(true);
		Torpedo component = gameObject.GetComponent<Torpedo>();
		component.databaseweapondata = UIFunctions.globaluifunctions.database.databaseweapondata[this.databaseaircraftdata.torpedoIDs[0]];
		component.noSurfaceTargets = UIFunctions.globaluifunctions.combatai.AreHostileShipsInArea();
		component.InitialiseTorpedo();
		UIFunctions.globaluifunctions.playerfunctions.eventcamera.CheckForEventCamera(gameObject.transform, GameDataManager.playervesselsonlevel[0].transform, 10f, true, false, false, component.databaseweapondata.minCameraDistance + 1f, component.databaseweapondata.minCameraDistance, -1f, true);
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.AddTorpedoToArray(component);
		if (ManualCameraZoom.target == base.transform)
		{
			ManualCameraZoom.target = gameObject.transform;
		}
		component.isAirborne = true;
		component.actualCurrentSpeed = this.helibody.InverseTransformDirection(this.helicopterrigidbody.velocity).z;
		this.GetNextWaypoint();
		component.cruiseYValue = GameDataManager.playervesselsonlevel[0].transform.position.y;
		component.searchYValue = GameDataManager.playervesselsonlevel[0].transform.position.y;
		this.ClearSensorData();
	}

	// Token: 0x060007D5 RID: 2005 RVA: 0x0004B7C8 File Offset: 0x000499C8
	private void HideWeaponHardpoint(GameObject weaponFired)
	{
		int num = this.databaseaircraftdata.torpedoNumbers[0] - this.torpedoesOnBoard - 1;
		if (num <= this.torpedoHardpoints.Length - 1)
		{
			this.torpedoHardpoints[num].SetActive(false);
			weaponFired.transform.position = this.torpedoHardpoints[num].transform.position;
			weaponFired.transform.rotation = this.torpedoHardpoints[num].transform.rotation;
		}
	}

	// Token: 0x060007D6 RID: 2006 RVA: 0x0004B844 File Offset: 0x00049A44
	private void ClearSensorData()
	{
		this.sensordata.decibelsTotalDetected = 0f;
		this.sensordata.decibelsLastDetected = 0f;
		this.sensordata.radarTotalDetected = 0f;
		this.sensordata.timeTrackingPlayer = 0f;
	}

	// Token: 0x04000B1A RID: 2842
	public DatabaseAircraftData databaseaircraftdata;

	// Token: 0x04000B1B RID: 2843
	public int aircraftID;

	// Token: 0x04000B1C RID: 2844
	public SensorData sensordata;

	// Token: 0x04000B1D RID: 2845
	public float rotationX;

	// Token: 0x04000B1E RID: 2846
	public float rotationZ;

	// Token: 0x04000B1F RID: 2847
	public float sqrMaxVelocity;

	// Token: 0x04000B20 RID: 2848
	public float sqrMaxVelocityThrottle;

	// Token: 0x04000B21 RID: 2849
	public float maxDownVelocity;

	// Token: 0x04000B22 RID: 2850
	private float maxVelocity;

	// Token: 0x04000B23 RID: 2851
	public float turnspeed;

	// Token: 0x04000B24 RID: 2852
	public Transform helibody;

	// Token: 0x04000B25 RID: 2853
	private Quaternion target;

	// Token: 0x04000B26 RID: 2854
	public float minimumX;

	// Token: 0x04000B27 RID: 2855
	public float maximumX;

	// Token: 0x04000B28 RID: 2856
	public float minimumZ;

	// Token: 0x04000B29 RID: 2857
	public float maximumZ;

	// Token: 0x04000B2A RID: 2858
	public float levelheight;

	// Token: 0x04000B2B RID: 2859
	public float dampening;

	// Token: 0x04000B2C RID: 2860
	private Vector3 throttle;

	// Token: 0x04000B2D RID: 2861
	private Vector3 horizontalvelocity;

	// Token: 0x04000B2E RID: 2862
	private Vector3 xzvector;

	// Token: 0x04000B2F RID: 2863
	public bool throttleon;

	// Token: 0x04000B30 RID: 2864
	public float throttleTimer;

	// Token: 0x04000B31 RID: 2865
	public bool landing;

	// Token: 0x04000B32 RID: 2866
	public float currentaltitude;

	// Token: 0x04000B33 RID: 2867
	public float cruisealtitude;

	// Token: 0x04000B34 RID: 2868
	public float dipaltitude;

	// Token: 0x04000B35 RID: 2869
	public Rigidbody helicopterrigidbody;

	// Token: 0x04000B36 RID: 2870
	private Vector3 horizontalspeed;

	// Token: 0x04000B37 RID: 2871
	public float relativefactor;

	// Token: 0x04000B38 RID: 2872
	public float upfactor;

	// Token: 0x04000B39 RID: 2873
	public bool destroyed;

	// Token: 0x04000B3A RID: 2874
	public Transform waypoint;

	// Token: 0x04000B3B RID: 2875
	public float clampmax;

	// Token: 0x04000B3C RID: 2876
	public int movingSonar;

	// Token: 0x04000B3D RID: 2877
	public LineRenderer sonarLine;

	// Token: 0x04000B3E RID: 2878
	public float sonarheight;

	// Token: 0x04000B3F RID: 2879
	public float listeningTimer;

	// Token: 0x04000B40 RID: 2880
	public Transform waypointTransform;

	// Token: 0x04000B41 RID: 2881
	public int helicopterIndex;

	// Token: 0x04000B42 RID: 2882
	public string pathFinding;

	// Token: 0x04000B43 RID: 2883
	public bool isTerrainPresent;

	// Token: 0x04000B44 RID: 2884
	public float terrainScanTimer;

	// Token: 0x04000B45 RID: 2885
	public Transform raycastPosition;

	// Token: 0x04000B46 RID: 2886
	public float defaultAltitude;

	// Token: 0x04000B47 RID: 2887
	public float avoidShipsTimer;

	// Token: 0x04000B48 RID: 2888
	public float lerpTime;

	// Token: 0x04000B49 RID: 2889
	public float currentLerpTime;

	// Token: 0x04000B4A RID: 2890
	public float lerpXValue;

	// Token: 0x04000B4B RID: 2891
	public float lerpZValue;

	// Token: 0x04000B4C RID: 2892
	public bool attackPlayer;

	// Token: 0x04000B4D RID: 2893
	public bool OnAttackRun;

	// Token: 0x04000B4E RID: 2894
	public float attackDist;

	// Token: 0x04000B4F RID: 2895
	public bool dropPayload;

	// Token: 0x04000B50 RID: 2896
	public bool dropSonobuoy;

	// Token: 0x04000B51 RID: 2897
	public bool dropTorpedo;

	// Token: 0x04000B52 RID: 2898
	public float depthBombTimer;

	// Token: 0x04000B53 RID: 2899
	public int depthBombsDropped;

	// Token: 0x04000B54 RID: 2900
	public int[] sonobuoysOnBoard;

	// Token: 0x04000B55 RID: 2901
	public int sonobuoyTypeToDrop;

	// Token: 0x04000B56 RID: 2902
	public int torpedoesOnBoard;

	// Token: 0x04000B57 RID: 2903
	public float torpedoSpeed;

	// Token: 0x04000B58 RID: 2904
	public GameObject[] torpedoHardpoints;

	// Token: 0x04000B59 RID: 2905
	public int depthBombsOnBoard;

	// Token: 0x04000B5A RID: 2906
	public int currentAction;

	// Token: 0x04000B5B RID: 2907
	public string helicopterRole;

	// Token: 0x04000B5C RID: 2908
	public string currentAttack;

	// Token: 0x04000B5D RID: 2909
	public float currentAttackTimer;

	// Token: 0x04000B5E RID: 2910
	public bool doNotDip;

	// Token: 0x04000B5F RID: 2911
	public AudioSource audiosource;

	// Token: 0x04000B60 RID: 2912
	public ParticleSystem hoverparticle;

	// Token: 0x04000B61 RID: 2913
	public Transform transformToAttack;

	// Token: 0x04000B62 RID: 2914
	public float yClockFactor = 300f;

	// Token: 0x04000B63 RID: 2915
	public bool doOnce;

	// Token: 0x04000B64 RID: 2916
	public bool searchingTempArea;

	// Token: 0x04000B65 RID: 2917
	public Vector3 searchingTempAreaPosition;

	// Token: 0x04000B66 RID: 2918
	public int numberOfTempSearches;

	// Token: 0x04000B67 RID: 2919
	public bool attackPlayerActualTransform;

	// Token: 0x04000B68 RID: 2920
	public Vector2 relativeFactors;

	// Token: 0x04000B69 RID: 2921
	public Vector2 upFactors;
}
