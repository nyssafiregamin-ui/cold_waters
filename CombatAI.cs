using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000FC RID: 252
public class CombatAI : MonoBehaviour
{
	// Token: 0x060006C9 RID: 1737 RVA: 0x0003809C File Offset: 0x0003629C
	public void InitialiseCombatAI()
	{
		this.warnedFleet = false;
		this.enemyAircraftRolesAssigned = false;
		this.formationLastZigZag = 1f;
		if (UnityEngine.Random.value < 0.5f)
		{
			this.formationLastZigZag = -1f;
		}
		this.formationZigZagAngle = UnityEngine.Random.Range(15f, 30f);
		this.formationLegLenth = UnityEngine.Random.Range(300f, 600f);
		this.formationLegTimer = UnityEngine.Random.Range(10f, this.formationLegLenth);
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			if (GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipType == "BIOLOGIC")
			{
				GameDataManager.enemyvesselsonlevel[i].enabled = false;
				GameDataManager.enemyvesselsonlevel[i].vesselmovement.enabled = false;
				GameDataManager.enemyvesselsonlevel[i].vesselai.enabled = false;
			}
			else
			{
				this.totalEnemyUnits++;
			}
		}
		this.combatCheckTimer = 1f;
		this.playerWasDetected = false;
		this.playerWasFiredUpon = false;
		switch ((int)GameDataManager.optionsFloatSettings[3])
		{
		case 0:
			this.torpedoSalvoSize = 2f;
			this.missileSalvoSize = 1f;
			this.maxWeaponsInPlay = 3;
			break;
		case 1:
			this.torpedoSalvoSize = 3f;
			this.missileSalvoSize = 2f;
			this.maxWeaponsInPlay = 3;
			break;
		case 2:
			this.torpedoSalvoSize = 3f;
			this.missileSalvoSize = 2f;
			this.maxWeaponsInPlay = 4;
			break;
		case 3:
			this.torpedoSalvoSize = 4f;
			this.missileSalvoSize = 3f;
			this.maxWeaponsInPlay = 5;
			break;
		}
		int num = 0;
		for (int j = 0; j < GameDataManager.enemyvesselsonlevel.Length; j++)
		{
			if (GameDataManager.enemyvesselsonlevel[j].whichNavy == 1)
			{
				num++;
			}
		}
		if (this.maxWeaponsInPlay > num * 3)
		{
			this.maxWeaponsInPlay = num * 3;
		}
		this.maxWeaponsInPlay += UnityEngine.Random.Range(0, 3);
	}

	// Token: 0x060006CA RID: 1738 RVA: 0x000382B8 File Offset: 0x000364B8
	public bool AreHostileShipsInArea()
	{
		return true;
	}

	// Token: 0x060006CB RID: 1739 RVA: 0x000382C8 File Offset: 0x000364C8
	public void DisengageAllShips()
	{
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			if (GameDataManager.enemyvesselsonlevel[i].vesselai.takingAction == 1)
			{
				GameDataManager.enemyvesselsonlevel[i].vesselai.takingAction = 0;
			}
			GameDataManager.enemyvesselsonlevel[i].vesselai.attackRole = "DEFENDER";
			GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata.playerDetected = false;
			GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata.timeTrackingPlayer = 0f;
			GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata.decibelsLastDetected = 0f;
			GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata.decibelsTotalDetected = 0f;
			GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata.radarTotalDetected = 0f;
		}
	}

	// Token: 0x060006CC RID: 1740 RVA: 0x000383B0 File Offset: 0x000365B0
	public void CalculateFormationBounds()
	{
		this.formationBounds = new Vector4(-30f, 30f, 40f, -20f);
		for (int i = 0; i < UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.formationPositions.Length; i++)
		{
			if (UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.formationPositions[i].transform.localPosition.x < this.formationBounds.x)
			{
				this.formationBounds.x = UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.formationPositions[i].transform.localPosition.x;
			}
			else if (UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.formationPositions[i].transform.localPosition.x > this.formationBounds.y)
			{
				this.formationBounds.y = UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.formationPositions[i].transform.localPosition.x;
			}
			if (UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.formationPositions[i].transform.localPosition.z > this.formationBounds.z)
			{
				this.formationBounds.z = UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.formationPositions[i].transform.localPosition.z;
			}
			else if (UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.formationPositions[i].transform.localPosition.z < this.formationBounds.w)
			{
				this.formationBounds.w = UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.formationPositions[i].transform.localPosition.z;
			}
		}
	}

	// Token: 0x060006CD RID: 1741 RVA: 0x000385C0 File Offset: 0x000367C0
	public int GetnumberOfTorpedoesFiredByVessel(Vessel activeVessel)
	{
		int num = 0;
		for (int i = 0; i < UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects.Length; i++)
		{
			if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[i].whichNavy == 1)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060006CE RID: 1742 RVA: 0x0003861C File Offset: 0x0003681C
	public bool CheckWaterLocationIsValid(Vector3 checkPosition)
	{
		bool result = true;
		float terrainHeightAtPosition = UIFunctions.globaluifunctions.levelloadmanager.GetTerrainHeightAtPosition(checkPosition.x, checkPosition.z + 3f);
		if (terrainHeightAtPosition < 0.1f)
		{
			result = false;
		}
		terrainHeightAtPosition = UIFunctions.globaluifunctions.levelloadmanager.GetTerrainHeightAtPosition(checkPosition.x - 3f, checkPosition.z - 3f);
		if (terrainHeightAtPosition < 0.1f)
		{
			result = false;
		}
		terrainHeightAtPosition = UIFunctions.globaluifunctions.levelloadmanager.GetTerrainHeightAtPosition(checkPosition.x + 3f, checkPosition.z - 3f);
		if (terrainHeightAtPosition < 0.1f)
		{
			result = false;
		}
		return result;
	}

	// Token: 0x060006CF RID: 1743 RVA: 0x000386CC File Offset: 0x000368CC
	public Transform GetActualPositionToAttack(Transform attackingTransform)
	{
		List<Torpedo> list = new List<Torpedo>();
		for (int i = 0; i < UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects.Length; i++)
		{
			if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[i].databaseweapondata.isDecoy && UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[i].whichNavy == 0 && UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[i] != null)
			{
				list.Add(UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[i]);
			}
		}
		if (list.Count == 0)
		{
			return GameDataManager.playervesselsonlevel[0].transform;
		}
		float num = Vector3.Distance(attackingTransform.position, GameDataManager.playervesselsonlevel[0].transform.position);
		float num2 = num;
		int num3 = -1;
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j] != null)
			{
				num = Vector3.Distance(attackingTransform.position, list[j].transform.position);
				if (num < num2)
				{
					num2 = num;
					num3 = j;
				}
			}
		}
		if (num3 == -1)
		{
			return GameDataManager.playervesselsonlevel[0].transform;
		}
		if (UnityEngine.Random.value < 0.1f)
		{
			list[num3].decoyIdentified = true;
		}
		return list[num3].transform;
	}

	// Token: 0x060006D0 RID: 1744 RVA: 0x0003885C File Offset: 0x00036A5C
	public void PlaySonarPing(Vessel activeVessel)
	{
		activeVessel.vesselmovement.pingSound.Play();
	}

	// Token: 0x060006D1 RID: 1745 RVA: 0x00038870 File Offset: 0x00036A70
	public void WarnFleet()
	{
		float num = 5000f * GameDataManager.inverseYardsScale;
		this.positionWarnedFrom.x = this.positionWarnedFrom.x + UnityEngine.Random.Range(-num, num);
		this.positionWarnedFrom.y = this.positionWarnedFrom.y + UnityEngine.Random.Range(-num, num);
		float timeToRun = UnityEngine.Random.Range(120f, 300f);
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			if ((GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipType == "MERCHANT" || GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipType == "CAPITAL") && !GameDataManager.enemyvesselsonlevel[i].vesselai.isNeutral)
			{
				this.RetreatFromPosition(GameDataManager.enemyvesselsonlevel[i], this.positionWarnedFrom, timeToRun);
			}
		}
		this.InitiateAttackAssigningNewKiller();
		this.SendAircraftToSuspectedLocation();
		this.SendHeloToSuspectedLocation(false);
	}

	// Token: 0x060006D2 RID: 1746 RVA: 0x00038964 File Offset: 0x00036B64
	private void InitiateAttackAssigningNewKiller()
	{
		bool flag = false;
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			if (GameDataManager.enemyvesselsonlevel[i].vesselai.attackRole == string.Empty)
			{
				float num = float.PositiveInfinity;
				int num2 = -1;
				for (int j = 0; j < UIFunctions.globaluifunctions.playerfunctions.sensormanager.rangeToContacts.Length; j++)
				{
					if (GameDataManager.enemyvesselsonlevel[j].vesselai.attackRole == "KILLER")
					{
						flag = true;
						break;
					}
					if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.rangeToContacts[i] < num && !GameDataManager.enemyvesselsonlevel[j].vesselai.actDefensively && GameDataManager.enemyvesselsonlevel[j].databaseshipdata.passiveSonarID != -1 && (GameDataManager.enemyvesselsonlevel[j].databaseshipdata.shipType == "SUBMARINE" || GameDataManager.enemyvesselsonlevel[j].databaseshipdata.shipType == "ESCORT"))
					{
						num = UIFunctions.globaluifunctions.playerfunctions.sensormanager.rangeToContacts[i];
						num2 = i;
					}
				}
				if (num2 != -1 && !flag)
				{
					this.BeginAttackOnPosition(GameDataManager.enemyvesselsonlevel[num2], this.positionWarnedFrom);
				}
			}
		}
	}

	// Token: 0x060006D3 RID: 1747 RVA: 0x00038AD0 File Offset: 0x00036CD0
	private void SendAircraftToSuspectedLocation()
	{
		if (this.enemyAircraft.Length > 0)
		{
			float num = 10000f * GameDataManager.inverseYardsScale;
			if (!this.enemyAircraft[0].sensordata.playerDetected)
			{
				this.enemyAircraft[0].waypoint.transform.position = new Vector3(this.positionWarnedFrom.x + UnityEngine.Random.Range(-num, num), 1000f, this.positionWarnedFrom.z + UnityEngine.Random.Range(-num, num));
			}
		}
	}

	// Token: 0x060006D4 RID: 1748 RVA: 0x00038B58 File Offset: 0x00036D58
	private void SendHeloToSuspectedLocation(bool narrowSearch = false)
	{
		if (this.enemyHelicopters.Length > 0)
		{
			int num = 0;
			if (this.enemyHelicopters.Length > 1)
			{
				if (this.enemyHelicopters[0].torpedoesOnBoard == 0)
				{
					num = 1;
				}
				else if (this.enemyHelicopters.Length > 2)
				{
					if (this.enemyHelicopters[1].torpedoesOnBoard == 0)
					{
						num = 2;
					}
					else if (this.enemyHelicopters.Length > 3 && this.enemyHelicopters[2].torpedoesOnBoard == 0)
					{
						num = 3;
					}
				}
			}
			if (!this.enemyHelicopters[num].OnAttackRun)
			{
				float num2 = 5000f * GameDataManager.inverseYardsScale;
				if (narrowSearch)
				{
					num2 /= 2f;
				}
				this.enemyHelicopters[num].searchingTempArea = true;
				this.enemyHelicopters[num].numberOfTempSearches = UnityEngine.Random.Range(4, 9);
				this.enemyHelicopters[num].searchingTempAreaPosition = new Vector3(this.positionWarnedFrom.x + UnityEngine.Random.Range(-num2, num2), 1000f, this.positionWarnedFrom.z + UnityEngine.Random.Range(-num2, num2));
			}
		}
		if (this.enemyAircraft.Length > 0 && !this.enemyAircraft[0].OnAttackRun)
		{
			float num3 = 5000f * GameDataManager.inverseYardsScale;
			this.enemyAircraft[0].OnAttackRun = true;
			this.enemyAircraft[0].waypoint.position = new Vector3(this.positionWarnedFrom.x + UnityEngine.Random.Range(-num3, num3), 1000f, this.positionWarnedFrom.z + UnityEngine.Random.Range(-num3, num3));
		}
	}

	// Token: 0x060006D5 RID: 1749 RVA: 0x00038CF4 File Offset: 0x00036EF4
	public bool CheckIfKnuckle(Vessel activeVessel)
	{
		float num = activeVessel.vesselmovement.shipSpeed.z * 10f;
		if (num < 24f)
		{
			return false;
		}
		if (activeVessel.vesselmovement.rudderAngle.x == 3f || activeVessel.vesselmovement.rudderAngle.x == -3f)
		{
			Noisemaker component = ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.databasecountermeasuredata[UIFunctions.globaluifunctions.playerfunctions.sensormanager.knuckleID].countermeasureObject, activeVessel.vesselmovement.rudder[0].position, Quaternion.identity).GetComponent<Noisemaker>();
			component.gameObject.SetActive(true);
			component.tacMapNoisemakerIcon.shipDisplayIcon.color = UIFunctions.globaluifunctions.levelloadmanager.tacticalmap.navyColors[1];
			component.databasecountermeasuredata = UIFunctions.globaluifunctions.database.databasecountermeasuredata[UIFunctions.globaluifunctions.playerfunctions.sensormanager.knuckleID];
			activeVessel.uifunctions.playerfunctions.sensormanager.AddNoiseMakerToArray(component);
			return true;
		}
		return false;
	}

	// Token: 0x060006D6 RID: 1750 RVA: 0x00038E24 File Offset: 0x00037024
	public void GetSonobuoyData(SensorData aircraftSensorData, GameObject listeningAircraft)
	{
		if (aircraftSensorData.sonobuoys.Length == 0)
		{
			return;
		}
		for (int i = 0; i < aircraftSensorData.sonobuoys.Length; i++)
		{
			bool flag = false;
			if (aircraftSensorData.sonobuoys[i].name == "ACTIVE")
			{
				flag = true;
			}
			int num = 0;
			if (flag)
			{
				num = 1;
			}
			int num2 = aircraftSensorData.sonobuoySonarIDs[num];
			UIFunctions.globaluifunctions.playerfunctions.sensormanager.CheckSingleSonar(listeningAircraft, GameDataManager.playervesselsonlevel[0].gameObject, num2, num2, num, aircraftSensorData, true, aircraftSensorData.sonobuoys[i].gameObject);
			for (int j = 0; j < UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects.Length; j++)
			{
				if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[j].databaseweapondata.weaponType == "DECOY" && UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[j].whichNavy == 0)
				{
					UIFunctions.globaluifunctions.playerfunctions.sensormanager.CheckSingleSonar(listeningAircraft, UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[j].gameObject, num2, num2, num, aircraftSensorData, true, aircraftSensorData.sonobuoys[i].gameObject);
				}
			}
		}
	}

	// Token: 0x060006D7 RID: 1751 RVA: 0x00038F80 File Offset: 0x00037180
	public bool CheckAircraftToAttackPlayer(Aircraft aircraft, Helicopter enemyAircraft, SensorData sensordata)
	{
		if (!this.PlayerIsCombatWorthy())
		{
			return false;
		}
		float num = 200f;
		if (sensordata.usingActiveSonar && sensordata.decibelsLastDetected > 10f)
		{
			num /= 2f;
		}
		else if (sensordata.usingActiveSonar && sensordata.decibelsLastDetected > 0f)
		{
			num /= 1.5f;
		}
		if (sensordata.decibelsTotalDetected + sensordata.radarTotalDetected > UnityEngine.Random.Range(num - 50f, num + 50f))
		{
			if (!this.enemyAircraftRolesAssigned)
			{
				this.AssignEnemyAircraftRoles(aircraft, enemyAircraft);
			}
			return true;
		}
		return false;
	}

	// Token: 0x060006D8 RID: 1752 RVA: 0x00039028 File Offset: 0x00037228
	private void AssignEnemyAircraftRoles(Aircraft planes, Helicopter helicopters)
	{
		int num = this.enemyHelicopters.Length + this.enemyAircraft.Length;
		if (num == 1)
		{
			if (planes != null)
			{
				planes.aircraftRole = "KILLER";
				return;
			}
			if (helicopters != null)
			{
				helicopters.helicopterRole = "KILLER";
				return;
			}
		}
		for (int i = 0; i < this.enemyAircraft.Length; i++)
		{
			this.enemyAircraft[i].aircraftRole = "HUNTER";
		}
		for (int j = 0; j < this.enemyHelicopters.Length; j++)
		{
			this.enemyHelicopters[j].helicopterRole = "HUNTER";
		}
		if (planes != null)
		{
			planes.aircraftRole = "KILLER";
			return;
		}
		if (helicopters != null)
		{
			helicopters.helicopterRole = "KILLER";
			return;
		}
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x00039104 File Offset: 0x00037304
	public bool PlayerIsCombatWorthy()
	{
		return !GameDataManager.playervesselsonlevel[0].isSinking;
	}

	// Token: 0x060006DA RID: 1754 RVA: 0x0003911C File Offset: 0x0003731C
	public void AIAdjustTelegraph(Vessel activeVessel, int telegraphValue)
	{
		if (activeVessel.vesselmovement.atAnchor)
		{
			return;
		}
		activeVessel.vesselmovement.telegraphValue = telegraphValue;
		if (activeVessel.vesselmovement.telegraphValue > activeVessel.damagesystem.maxTelegraph)
		{
			activeVessel.vesselmovement.telegraphValue = activeVessel.damagesystem.maxTelegraph;
		}
		activeVessel.vesselmovement.engineSpeed.x = (float)(-1 + activeVessel.vesselmovement.telegraphValue) / 5f * activeVessel.vesselmovement.shipSpeed.y;
	}

	// Token: 0x060006DB RID: 1755 RVA: 0x000391AC File Offset: 0x000373AC
	public void CheckToAttackPlayer(Vessel activeVessel)
	{
		if (activeVessel.vesselai.takingAction != 1)
		{
			if (activeVessel.vesselai.sensordata.decibelsLastDetected > UnityEngine.Random.Range(10f, 20f))
			{
				activeVessel.vesselai.takingAction = 1;
				this.BeginAttackOnPosition(activeVessel, this.GetActualPositionToAttack(activeVessel.transform).position);
			}
			else if (activeVessel.vesselai.sensordata.decibelsLastDetected > UnityEngine.Random.Range(0f, 20f))
			{
				activeVessel.vesselai.takingAction = 4;
				activeVessel.vesselmovement.isCruising = false;
				this.AIAdjustTelegraph(activeVessel, 2);
				activeVessel.vesselai.actionTimeToFinish = UnityEngine.Random.Range(8f, 30f);
			}
			else if (UnityEngine.Random.Range(0, 100) < 15)
			{
				if (UnityEngine.Random.value < 0.5f || activeVessel.isSubmarine)
				{
					activeVessel.vesselai.takingAction = 4;
					activeVessel.vesselmovement.isCruising = false;
					this.AIAdjustTelegraph(activeVessel, 2);
					activeVessel.vesselai.actionTimeToFinish = UnityEngine.Random.Range(8f, 30f);
				}
				else
				{
					activeVessel.vesselai.takingAction = 5;
					activeVessel.vesselmovement.isCruising = false;
					this.AIAdjustTelegraph(activeVessel, 6);
					activeVessel.vesselai.actionTimeToFinish = UnityEngine.Random.Range(8f, 30f);
				}
			}
		}
		bool flag = false;
		float num = 200f;
		if (activeVessel.acoustics.usingActiveSonar && activeVessel.vesselai.sensordata.decibelsLastDetected > 10f)
		{
			num /= 2f;
		}
		else if (activeVessel.acoustics.usingActiveSonar && activeVessel.vesselai.sensordata.decibelsLastDetected > 0f)
		{
			num /= 1.5f;
		}
		if (activeVessel.vesselai.sensordata.decibelsTotalDetected + activeVessel.vesselai.sensordata.radarTotalDetected > UnityEngine.Random.Range(num - 50f, num + 50f))
		{
			flag = this.BeginAttackOnPosition(activeVessel, this.GetActualPositionToAttack(activeVessel.transform).position);
		}
		if (flag)
		{
			activeVessel.vesselai.AttackPosition();
			if (activeVessel.vesselai.sensordata.decibelsTotalDetected > 1000f)
			{
				if (UnityEngine.Random.value < 0.15f)
				{
					if (this.enemyHelicopters.Length > 0 && !this.enemyHelicopters[0].OnAttackRun)
					{
						this.enemyHelicopters[0].transformToAttack = activeVessel.vesselai.transformToAttack;
						this.enemyHelicopters[0].attackPlayer = true;
					}
				}
				else if (UnityEngine.Random.value < 0.15f && this.enemyAircraft.Length > 0 && !this.enemyAircraft[0].OnAttackRun)
				{
					this.enemyAircraft[0].transformToAttack = activeVessel.vesselai.transformToAttack;
					this.enemyAircraft[0].attackPlayer = true;
				}
			}
		}
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x000394BC File Offset: 0x000376BC
	public void CheckPlayerTransient()
	{
		int num = 0;
		float num2 = 0f;
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			float num3 = this.uifunctions.playerfunctions.sensormanager.CheckCanHearTransient(GameDataManager.enemyvesselsonlevel[i], GameDataManager.playervesselsonlevel[0]);
			if (num3 > num2 && !GameDataManager.enemyvesselsonlevel[i].isSubmarine)
			{
				num2 = num3;
				num = i;
			}
			if (GameDataManager.enemyvesselsonlevel[i].isSubmarine && num3 > 10f)
			{
				this.AttackPosition(i);
			}
		}
		if (num2 > 10f && !this.warnedFleet)
		{
			this.warnedFleet = true;
			this.positionWarnedFrom = this.uifunctions.playerfunctions.playerVessel.transform.position;
			this.warnTimer = UnityEngine.Random.Range(10f, 20f);
			this.BeginAttackOnPosition(GameDataManager.enemyvesselsonlevel[num], this.uifunctions.playerfunctions.playerVessel.transform.position);
		}
	}

	// Token: 0x060006DD RID: 1757 RVA: 0x000395C8 File Offset: 0x000377C8
	private void AttackPosition(int i)
	{
		GameDataManager.enemyvesselsonlevel[i].vesselai.AttackPosition();
	}

	// Token: 0x060006DE RID: 1758 RVA: 0x000395DC File Offset: 0x000377DC
	private void RetreatFromPosition(Vessel retreatingVessel, Vector3 retreatfrom, float timeToRun)
	{
		retreatingVessel.vesselai.takingAction = 2;
		retreatingVessel.vesselai.actionPosition = this.uifunctions.playerfunctions.playerVessel.transform.position + retreatfrom;
		retreatingVessel.vesselmovement.isCruising = false;
		this.AIAdjustTelegraph(retreatingVessel, 6);
		retreatingVessel.vesselai.actionTimer = timeToRun;
	}

	// Token: 0x060006DF RID: 1759 RVA: 0x00039640 File Offset: 0x00037840
	public bool BeginAttackOnPosition(Vessel attackingVessel, Vector3 positionToAttack)
	{
		if (attackingVessel.vesselai.actDefensively && Vector3.Distance(GameDataManager.playervesselsonlevel[0].transform.position, attackingVessel.transform.position) > 5000f * GameDataManager.inverseYardsScale)
		{
			attackingVessel.vesselai.actionPosition = positionToAttack;
			attackingVessel.vesselai.takingAction = 2;
			return false;
		}
		if (!(attackingVessel.databaseshipdata.shipType == "SUBMARINE"))
		{
			for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
			{
				if (!GameDataManager.enemyvesselsonlevel[i].vesselai.isNeutral && GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipType != "SUBMARINE")
				{
					GameDataManager.enemyvesselsonlevel[i].vesselai.attackRole = "DEFENDER";
					GameDataManager.enemyvesselsonlevel[i].vesselai.takingAction = 2;
					GameDataManager.enemyvesselsonlevel[i].vesselai.actionPosition = positionToAttack;
				}
			}
		}
		attackingVessel.vesselai.attackRole = "KILLER";
		this.killerVessel = attackingVessel;
		this.killerVessel.vesselai.sensordata.lastKnownTargetPosition = positionToAttack;
		this.killerVessel.vesselai.takingAction = 1;
		this.killerVessel.vesselai.SwitchToActiveSonar();
		if (this.killerVessel.vesselai.sensordata.rangeYardsLastDetected > 15000f)
		{
			if (this.enemyHelicopters.Length > 0)
			{
				this.enemyHelicopters[0].sensordata.decibelsLastDetected = 100f;
			}
			else if (this.enemyAircraft.Length > 0)
			{
				this.enemyAircraft[0].sensordata.decibelsLastDetected = 100f;
			}
			float num = 0.2f;
			if (this.killerVessel.vesselai.hasMissile && UnityEngine.Random.value < num)
			{
				this.killerVessel.vesselai.enemymissile.fireAtTarget = true;
			}
		}
		if (attackingVessel.databaseshipdata.shipType != "SUBMARINE")
		{
			this.AssignNewHunter(attackingVessel, positionToAttack);
		}
		return true;
	}

	// Token: 0x060006E0 RID: 1760 RVA: 0x0003986C File Offset: 0x00037A6C
	public void SetTelegraphToEngage(Vessel activeVessel)
	{
		activeVessel.vesselmovement.isCruising = false;
		if (activeVessel.vesselai.sensordata.rangeYardsLastDetected > 10000f)
		{
			this.AIAdjustTelegraph(activeVessel, 6);
		}
		else if (activeVessel.vesselai.sensordata.rangeYardsLastDetected < 6000f)
		{
			this.AIAdjustTelegraph(activeVessel, 2);
		}
		else
		{
			this.AIAdjustTelegraph(activeVessel, UnityEngine.Random.Range(3, 5));
		}
		if (activeVessel.vesselai.attackRole == "HUNTER")
		{
			this.AIAdjustTelegraph(activeVessel, 2);
		}
	}

	// Token: 0x060006E1 RID: 1761 RVA: 0x00039904 File Offset: 0x00037B04
	public void AssignNewHunter(Vessel attackingVessel, Vector3 positionToAttack)
	{
		int num = -1;
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			float num2 = 1000f;
			if (GameDataManager.enemyvesselsonlevel[i].databaseshipdata.activeSonarID != -1 && GameDataManager.enemyvesselsonlevel[i].vesselai.attackRole == "DEFENDER" && GameDataManager.enemyvesselsonlevel[i].damagesystem.shipCurrentDamagePoints == 0f && !GameDataManager.enemyvesselsonlevel[i].vesselai.actDefensively && GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipType != "SUBMARINE")
			{
				float num3 = Vector3.Distance(attackingVessel.transform.position, GameDataManager.enemyvesselsonlevel[i].transform.position);
				if (num3 < num2)
				{
					num = i;
				}
			}
		}
		if (num != -1)
		{
			this.hunterVessel = GameDataManager.enemyvesselsonlevel[num];
			this.hunterVessel.vesselai.attackRole = "HUNTER";
			this.hunterVessel.vesselai.SwitchToActiveSonar();
			this.hunterVessel.vesselai.sensordata.lastKnownTargetPosition = positionToAttack;
			this.hunterVessel.vesselai.takingAction = 1;
		}
		else
		{
			this.hunterVessel = null;
		}
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x00039A54 File Offset: 0x00037C54
	public void AssignNewHunterKillers(Vessel initiatingVessel, bool wasKiller)
	{
		if (wasKiller)
		{
			for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
			{
				if (GameDataManager.enemyvesselsonlevel[i].vesselai.attackRole == "HUNTER")
				{
					GameDataManager.enemyvesselsonlevel[i].vesselai.attackRole = "KILLER";
					this.killerVessel = GameDataManager.enemyvesselsonlevel[i];
					this.killerVessel.vesselai.sensordata.lastKnownTargetPosition = initiatingVessel.vesselai.sensordata.lastKnownTargetPosition;
					this.killerVessel.vesselai.takingAction = 1;
					this.killerVessel.vesselai.SwitchToActiveSonar();
					this.hunterVessel = null;
					this.AssignNewHunter(this.killerVessel, this.killerVessel.vesselai.sensordata.lastKnownTargetPosition);
				}
			}
		}
		else
		{
			this.AssignNewHunter(this.killerVessel, this.killerVessel.vesselai.sensordata.lastKnownTargetPosition);
		}
		initiatingVessel.vesselai.attackRole = "DEFENDER";
		if (!initiatingVessel.isSinking)
		{
			initiatingVessel.vesselai.takingAction = 0;
			this.AIAdjustTelegraph(initiatingVessel, UnityEngine.Random.Range(3, 7));
			initiatingVessel.vesselmovement.isCruising = true;
		}
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x00039B98 File Offset: 0x00037D98
	public bool CheckClearLOSToTarget(Vessel firingVessel, Vector3 firingPosition, Vector3 targetPosition)
	{
		LayerMask mask = 1048576;
		firingPosition.y = 1000f;
		targetPosition.y = 1000f;
		float maxDistance = Vector3.Distance(firingPosition, targetPosition);
		firingVessel.acoustics.sensorNavigator.transform.LookAt(targetPosition);
		RaycastHit raycastHit;
		return !Physics.Raycast(firingVessel.acoustics.sensorNavigator.position, firingVessel.acoustics.sensorNavigator.transform.forward, out raycastHit, maxDistance, mask) && UIFunctions.globaluifunctions.playerfunctions.sensormanager.GetLandMaskingPenalty(base.transform, GameDataManager.playervesselsonlevel[0].transform) == 1f;
	}

	// Token: 0x060006E4 RID: 1764 RVA: 0x00039C54 File Offset: 0x00037E54
	private void MoveStuff()
	{
		if (GameDataManager.playervesselsonlevel[0].gameObject.activeSelf)
		{
			GameDataManager.playervesselsonlevel[0].vesselmovement.TranslateShipForward();
		}
		foreach (Vessel vessel in GameDataManager.enemyvesselsonlevel)
		{
			if (vessel.gameObject.activeSelf)
			{
				vessel.vesselmovement.TranslateShipForward();
			}
		}
		foreach (Torpedo torpedo in UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects)
		{
			torpedo.TranslateTorpedoForward();
		}
		foreach (Aircraft aircraft in this.enemyAircraft)
		{
			aircraft.TranslateAircraftForward();
		}
		foreach (Torpedo torpedo2 in UIFunctions.globaluifunctions.playerfunctions.sensormanager.sonosInFlight)
		{
			if (torpedo2 != null)
			{
				torpedo2.TranslateTorpedoForward();
			}
		}
	}

	// Token: 0x060006E5 RID: 1765 RVA: 0x00039DA4 File Offset: 0x00037FA4
	private void CheckOverallCombat()
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			if (GameDataManager.enemyvesselsonlevel[i].databaseshipdata.shipType != "BIOLOGIC")
			{
				if (GameDataManager.enemyvesselsonlevel[i].isSinking)
				{
					num++;
				}
				else if (GameDataManager.enemyvesselsonlevel[i].vesselai.sensordata.decibelsLastDetected > 5f)
				{
					num2++;
				}
			}
		}
		if (num2 == 0)
		{
			for (int j = 0; j < this.enemyHelicopters.Length; j++)
			{
				if (this.enemyHelicopters[j].sensordata.decibelsLastDetected > 5f)
				{
					num2++;
				}
			}
			for (int k = 0; k < this.enemyAircraft.Length; k++)
			{
				if (this.enemyAircraft[k].sensordata.decibelsLastDetected > 5f)
				{
					num2++;
				}
			}
		}
		if (num2 == 0)
		{
			AudioManager.audiomanager.PlayCombatAmbientMusic();
			this.playerCanDisengage = true;
		}
	}

	// Token: 0x060006E6 RID: 1766 RVA: 0x00039EC0 File Offset: 0x000380C0
	private void FixedUpdate()
	{
		this.MoveStuff();
		if (this.combatCheckTimer > 0f)
		{
			this.combatCheckTimer -= Time.deltaTime;
			if (this.combatCheckTimer <= 0f)
			{
				this.CheckOverallCombat();
				this.combatCheckTimer = 30f;
			}
		}
		if (this.warnTimer > 0f)
		{
			this.warnTimer -= Time.deltaTime;
			if (this.warnTimer <= 0f)
			{
				this.WarnFleet();
			}
		}
		this.sonobuoyTimer += Time.deltaTime;
		if (this.sonobuoyTimer > 10f)
		{
			this.sonobuoyTimer -= UnityEngine.Random.Range(9f, 11f);
			for (int i = 0; i < this.enemyAircraft.Length; i++)
			{
				this.enemyAircraft[i].sensordata.decibelsLastDetected = 0f;
				if (this.enemyAircraft[i].sensordata.decibelsTotalDetected > 0f)
				{
					this.enemyAircraft[i].sensordata.decibelsTotalDetected -= 2f;
				}
				this.GetSonobuoyData(this.enemyAircraft[i].sensordata, this.enemyAircraft[i].gameObject);
			}
			for (int j = 0; j < this.enemyHelicopters.Length; j++)
			{
				if (this.enemyHelicopters[j].sensordata.decibelsTotalDetected > 0f)
				{
					this.enemyHelicopters[j].sensordata.decibelsTotalDetected -= 2f;
				}
				this.GetSonobuoyData(this.enemyHelicopters[j].sensordata, this.enemyHelicopters[j].gameObject);
			}
		}
		if (this.usingFormationAI)
		{
			float d = this.levelloadmanager.levelloaddata.formationCruiseSpeed * Time.deltaTime * GameDataManager.globalTranslationSpeed;
			this.levelloadmanager.levelloaddata.formationGrid.transform.Translate(Vector3.forward * d);
			if (this.formationHeading != this.levelloadmanager.levelloaddata.formationGrid.transform.eulerAngles.y)
			{
				this.formationTurning = true;
				this.levelloadmanager.levelloaddata.formationGrid.transform.rotation = Quaternion.RotateTowards(this.levelloadmanager.levelloaddata.formationGrid.transform.rotation, Quaternion.Euler(0f, this.formationHeading, 0f), Time.deltaTime * 0.4f);
				this.levelloadmanager.levelloaddata.formationGrid.transform.Translate(Vector3.forward * d * 0.1f);
			}
			else
			{
				this.formationTurning = false;
			}
			this.formationLegTimer -= Time.deltaTime;
			if (this.formationLegTimer < --0f)
			{
				this.formationLegTimer += this.formationLegLenth;
				this.RecalculateFormationHeading();
			}
		}
	}

	// Token: 0x060006E7 RID: 1767 RVA: 0x0003A1DC File Offset: 0x000383DC
	private void RecalculateFormationHeading()
	{
		this.formationHeading += this.formationZigZagAngle * this.formationLastZigZag;
		this.formationLastZigZag *= -1f;
	}

	// Token: 0x04000815 RID: 2069
	public UIFunctions uifunctions;

	// Token: 0x04000816 RID: 2070
	public LevelLoadManager levelloadmanager;

	// Token: 0x04000817 RID: 2071
	public bool warnedFleet;

	// Token: 0x04000818 RID: 2072
	public Vessel killerVessel;

	// Token: 0x04000819 RID: 2073
	public Vessel hunterVessel;

	// Token: 0x0400081A RID: 2074
	public bool usingFormationAI;

	// Token: 0x0400081B RID: 2075
	public bool formationTurning;

	// Token: 0x0400081C RID: 2076
	public float formationHeading;

	// Token: 0x0400081D RID: 2077
	public float formationTurningDirection;

	// Token: 0x0400081E RID: 2078
	public float[] formationCruiseSpeeds;

	// Token: 0x0400081F RID: 2079
	public float formationLegLenth;

	// Token: 0x04000820 RID: 2080
	public float formationZigZagAngle;

	// Token: 0x04000821 RID: 2081
	public float formationLegTimer;

	// Token: 0x04000822 RID: 2082
	public float formationLastZigZag;

	// Token: 0x04000823 RID: 2083
	public Vector4 formationBounds;

	// Token: 0x04000824 RID: 2084
	public Helicopter[] enemyHelicopters;

	// Token: 0x04000825 RID: 2085
	public Aircraft[] enemyAircraft;

	// Token: 0x04000826 RID: 2086
	public bool enemyAircraftRolesAssigned;

	// Token: 0x04000827 RID: 2087
	public int totalEnemyUnits;

	// Token: 0x04000828 RID: 2088
	public bool playerCanDisengage;

	// Token: 0x04000829 RID: 2089
	public bool playerWasDetected;

	// Token: 0x0400082A RID: 2090
	public bool playerWasFiredUpon;

	// Token: 0x0400082B RID: 2091
	public float sonobuoyTimer;

	// Token: 0x0400082C RID: 2092
	public float warnTimer;

	// Token: 0x0400082D RID: 2093
	public float attackPlayerTimer;

	// Token: 0x0400082E RID: 2094
	public float combatCheckTimer;

	// Token: 0x0400082F RID: 2095
	public Vector3 positionWarnedFrom;

	// Token: 0x04000830 RID: 2096
	public float cameraDirectionY;

	// Token: 0x04000831 RID: 2097
	public float cameraDirectionX;

	// Token: 0x04000832 RID: 2098
	public bool playerIsAmbushed;

	// Token: 0x04000833 RID: 2099
	public LayerMask terrainMinesMask;

	// Token: 0x04000834 RID: 2100
	public float torpedoSalvoSize = 3f;

	// Token: 0x04000835 RID: 2101
	public float missileSalvoSize = 2f;

	// Token: 0x04000836 RID: 2102
	public int maxWeaponsInPlay = 6;
}
