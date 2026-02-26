using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020000FD RID: 253
public class Compartment : MonoBehaviour
{
	// Token: 0x060006E9 RID: 1769 RVA: 0x0003A220 File Offset: 0x00038420
	private void Start()
	{
		if (UIFunctions.globaluifunctions.museumObject.activeSelf)
		{
			return;
		}
		this.lowestYValue = 1f;
		this.canflood = false;
		if (this.compartmentType == 3)
		{
			this.compartmentArmor = this.activeVessel.bouyancyCompartments[4].compartmentArmor;
		}
	}

	// Token: 0x060006EA RID: 1770 RVA: 0x0003A278 File Offset: 0x00038478
	private void FixedUpdate()
	{
		if (this.isSleeping)
		{
			return;
		}
		if (this.compartmentType != 0)
		{
			return;
		}
		if (this.activeVessel.submerged)
		{
			return;
		}
		if (this.compartmentType != 0)
		{
			return;
		}
		if (this.activeVessel.isSinking && base.transform.position.y < 999.94f)
		{
			this.isSinking = true;
		}
		if (this.isSinking)
		{
			this.floodingAmount += this.sinkRate * Time.deltaTime;
			if (this.floodingAmount > 1.2f)
			{
				base.enabled = false;
			}
		}
		if (this.floodingAmount > this.compartmentMaximumDamage && !this.activeVessel.isSinking)
		{
			this.floodingAmount = this.compartmentMaximumDamage;
		}
		else if (this.floodingAmount < this.compartmentCurrentDamage)
		{
			this.floodingAmount += this.sinkRate * Time.deltaTime;
		}
	}

	// Token: 0x060006EB RID: 1771 RVA: 0x0003A388 File Offset: 0x00038588
	private void OnTriggerStay(Collider otherObject)
	{
		if (this.activeVessel.databaseshipdata.shipType == "BIOLOGIC")
		{
			return;
		}
		if ((otherObject.name == "Terrain Chunk" || otherObject.name.Contains("Ice")) && this.activeVessel.vesselmovement.shipSpeed.z > 0f)
		{
			VesselMovement vesselmovement = this.activeVessel.vesselmovement;
			vesselmovement.shipSpeed.z = vesselmovement.shipSpeed.z - 0.5f * Time.deltaTime;
		}
	}

	// Token: 0x060006EC RID: 1772 RVA: 0x0003A428 File Offset: 0x00038628
	private void OnTriggerEnter(Collider otherObject)
	{
		if (otherObject.name == "Terrain Chunk" || otherObject.name.Contains("Ice"))
		{
			if (this.activeVessel.databaseshipdata.shipType == "BIOLOGIC")
			{
				return;
			}
			if (this.activeVessel.vesselmovement.shipSpeed.z > 0.5f)
			{
				this.createdDust = false;
				if (this.activeVessel.playercontrolled)
				{
					float num = this.activeVessel.vesselmovement.shipSpeed.z * UnityEngine.Random.Range(5f, 15f);
					this.activeVessel.damagesystem.shipCurrentDamagePoints += num;
					float num2 = this.activeVessel.damagesystem.shipCurrentDamagePoints / this.activeVessel.damagesystem.shipTotalDamagePoints;
					if (num2 >= 1f)
					{
						this.SinkShip(this.activeVessel, true);
						UIFunctions.globaluifunctions.playerfunctions.playerSunkBy = "COLLISION";
					}
					int num3 = Mathf.RoundToInt((1f - num2) * 100f);
					UIFunctions.globaluifunctions.playerfunctions.damagecontrol.hullStatusReadout.text = LanguageManager.interfaceDictionary["HullStatus"] + " " + num3.ToString() + LanguageManager.interfaceDictionary["Percentage"];
					float num4 = num / (this.activeVessel.damagesystem.shipTotalDamagePoints / 5f);
					this.CheckSubsystemsDamage(num4);
					if (num4 < 1f)
					{
						this.ApplyDamageDecal(false, -1);
					}
					else
					{
						this.ApplyDamageDecal(true, -1);
					}
					this.ApplyFlooding((int)UnityEngine.Random.Range(0f, this.activeVessel.vesselmovement.shipSpeed.z - 0.5f) * 2);
				}
			}
			if (otherObject.name == "Terrain Chunk" && !this.createdDust)
			{
				this.createdDust = true;
				LayerMask mask = 1073741824;
				RaycastHit raycastHit;
				if (Physics.Raycast(base.transform.position, -Vector3.up, out raycastHit, 120f, mask))
				{
					UIFunctions.globaluifunctions.database.CreateFloorDust(raycastHit.point);
				}
			}
			return;
		}
		else
		{
			float num5 = 0f;
			Torpedo component = otherObject.GetComponent<Torpedo>();
			Projectile_DepthCharge projectile_DepthCharge = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (component != null)
			{
				if (component.damageDealt != 0)
				{
					return;
				}
				component.damageDealt++;
				num5 = UIFunctions.globaluifunctions.database.databaseweapondata[component.databaseweapondata.weaponID].warhead;
				if (component.databaseweapondata.weaponType == "MISSILE")
				{
					flag2 = true;
					component.missileHit = true;
					num5 /= 3f;
				}
				else
				{
					flag = true;
					num5 /= 2f;
					if (this.activeVessel.databaseshipdata.shipType == "SUBMARINE")
					{
						num5 /= 2f;
					}
				}
				if (component.whichNavy == 0)
				{
					flag3 = true;
				}
				component.destroyMe = true;
				if (ManualCameraZoom.target != component.transform)
				{
					bool surface = false;
					if (this.activeVessel.transform.position.y > 999.9f)
					{
						surface = true;
					}
					UIFunctions.globaluifunctions.playerfunctions.eventcamera.CheckForEventCamera(this.activeVessel.transform, null, 10f, surface, false, false, this.activeVessel.databaseshipdata.minCameraDistance + 4f, this.activeVessel.databaseshipdata.minCameraDistance, -1f, true);
				}
			}
			else
			{
				projectile_DepthCharge = otherObject.GetComponent<Projectile_DepthCharge>();
				if (projectile_DepthCharge == null)
				{
					projectile_DepthCharge = otherObject.GetComponentInParent<Projectile_DepthCharge>();
				}
				if (projectile_DepthCharge != null)
				{
					if (projectile_DepthCharge.alreadyHit != 0)
					{
						return;
					}
					projectile_DepthCharge.alreadyHit++;
					num5 = projectile_DepthCharge.blastzone.warhead;
					if (num5 > 0f)
					{
						if (!projectile_DepthCharge.blastzone.enabled)
						{
							if (projectile_DepthCharge.isContactExploded)
							{
								num5 /= 2f;
							}
							else
							{
								if (!projectile_DepthCharge.isProximityMine)
								{
									return;
								}
								num5 /= 4f;
							}
						}
						else
						{
							num5 /= 4f;
						}
					}
					if (projectile_DepthCharge.isProximityMine)
					{
						ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.underwaterLargeExplosions[UnityEngine.Random.Range(0, UIFunctions.globaluifunctions.database.underwaterLargeExplosions.Length)], base.transform.position, Quaternion.identity);
						ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.surfacePlumes[0], new Vector3(base.transform.position.x, 1000f, base.transform.position.z), Quaternion.identity);
						UnityEngine.Object.Destroy(projectile_DepthCharge.gameObject);
					}
				}
			}
			if (num5 <= 0f)
			{
				return;
			}
			if (this.activeVessel.databaseshipdata.shipType == "BIOLOGIC")
			{
				this.activeVessel.gameObject.SetActive(false);
				this.activeVessel.transform.Translate(Vector3.up * -500f);
				return;
			}
			float num6 = num5;
			if (flag3)
			{
				this.activeVessel.damagesystem.vesselDamagedByPlayer = true;
			}
			this.activeVessel.damagesystem.shipCurrentDamagePoints += num6;
			float num7 = this.activeVessel.damagesystem.shipCurrentDamagePoints / this.activeVessel.damagesystem.shipTotalDamagePoints;
			int numberOfHits = this.GetNumberOfHits(num7);
			int num8 = 1;
			int num9 = 2;
			int num10 = 3;
			int[] array = new int[3];
			if (this.activeVessel.databaseshipdata.shipType == "SUBMARINE")
			{
				num8 = 4;
				num9 = 2;
				num10 = 0;
			}
			else if (flag)
			{
				num8 = 4;
				num9 = 4;
				num10 = 1;
				array[0] = 1;
			}
			else if (this.activeVessel.databaseshipdata.shipType == "MERCHANT")
			{
				num8 = 2;
				num9 = 3;
				num10 = 3;
			}
			if (flag2 && UnityEngine.Random.value < 0.5f)
			{
				array[1]++;
			}
			if (this.activeVessel.isSinking && array[0] == 0)
			{
				array[0]++;
			}
			for (int i = 0; i < numberOfHits; i++)
			{
				int num11 = UnityEngine.Random.Range(1, 11);
				if (num11 <= num8)
				{
					array[0]++;
				}
				else if (num11 <= num8 + num9)
				{
					array[1]++;
				}
				else if (num11 <= num8 + num9 + num10)
				{
					array[2]++;
				}
			}
			if (array[0] > 0)
			{
				this.ApplyFlooding(array[0]);
			}
			if (array[1] > 0)
			{
				this.ApplyFire(array[1]);
			}
			if (array[2] > 0)
			{
				this.ApplyExplosion(array[2]);
			}
			bool flag4 = false;
			bool destroyed = false;
			float num12 = num6 / (this.activeVessel.damagesystem.shipTotalDamagePoints / 10f);
			if (flag || flag2)
			{
				destroyed = true;
				flag4 = true;
			}
			else if (num12 > 1f)
			{
				destroyed = true;
				flag4 = true;
			}
			else if (UnityEngine.Random.value < 0.5f)
			{
				flag4 = true;
			}
			num7 = this.activeVessel.damagesystem.shipCurrentDamagePoints / this.activeVessel.damagesystem.shipTotalDamagePoints;
			if (num7 >= 1f)
			{
				this.SinkShip(this.activeVessel, true);
				num7 = 1f;
				destroyed = true;
				flag4 = true;
				if (this.activeVessel.playercontrolled && UIFunctions.globaluifunctions.playerfunctions.playerSunkBy == string.Empty)
				{
					UIFunctions.globaluifunctions.playerfunctions.playerSunkBy = "WEAPON|";
					if (component != null)
					{
						PlayerFunctions playerfunctions = UIFunctions.globaluifunctions.playerfunctions;
						playerfunctions.playerSunkBy += component.databaseweapondata.weaponDescriptiveName.ToUpper();
					}
					if (projectile_DepthCharge != null)
					{
						PlayerFunctions playerfunctions2 = UIFunctions.globaluifunctions.playerfunctions;
						playerfunctions2.playerSunkBy += UIFunctions.globaluifunctions.database.databasedepthchargedata[projectile_DepthCharge.depthChargeID].depthchargeName.ToUpper();
					}
				}
			}
			if (flag4)
			{
				this.ApplyDamageDecal(destroyed, -1);
			}
			if (!this.activeVessel.isSubmarine)
			{
				this.activeVessel.WakeVessel(45f);
			}
			else
			{
				this.activeVessel.vesselmovement.currentCrushDepth = 1000f - (1000f - this.activeVessel.vesselmovement.crushDepth) * (1f - num7 * 0.5f);
			}
			if (!this.activeVessel.playercontrolled)
			{
				if (!UIFunctions.globaluifunctions.combatai.warnedFleet)
				{
					UIFunctions.globaluifunctions.combatai.warnedFleet = true;
					UIFunctions.globaluifunctions.combatai.warnTimer = UnityEngine.Random.Range(10f, 20f);
					UIFunctions.globaluifunctions.combatai.positionWarnedFrom = base.transform.position;
					UIFunctions.globaluifunctions.combatai.WarnFleet();
				}
				if (this.activeVessel.databaseshipdata.shipType == "SUBMARINE")
				{
				}
				if (this.compartmentCurrentDamage >= this.compartmentMaximumDamage)
				{
					float num13 = 0f;
					if (this.activeVessel.databaseshipdata.shipType != "SUBMARINE")
					{
						string text = this.compartmentPosition;
						switch (text)
						{
						case "FORE":
							this.activeVessel.uifunctions.playerfunctions.damagecontrol.compartmentTotalFlooding[1] = this.compartmentCurrentDamage / this.compartmentMaximumDamage;
							num13 = 0.2f;
							break;
						case "MID":
							this.activeVessel.uifunctions.playerfunctions.damagecontrol.compartmentTotalFlooding[2] = this.compartmentCurrentDamage / this.compartmentMaximumDamage;
							num13 = 0.5f;
							break;
						case "AFT":
							this.activeVessel.uifunctions.playerfunctions.damagecontrol.compartmentTotalFlooding[3] = this.compartmentCurrentDamage / this.compartmentMaximumDamage;
							num13 = 0.2f;
							break;
						}
						if (UnityEngine.Random.value < num13)
						{
							this.SinkShip(this.activeVessel, false);
							this.activeVessel.isCapsizing = true;
						}
					}
				}
			}
			else
			{
				int num15 = Mathf.RoundToInt((1f - num7) * 100f);
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.hullStatusReadout.text = LanguageManager.interfaceDictionary["HullStatus"] + " " + num15.ToString() + LanguageManager.interfaceDictionary["Percentage"];
				this.CheckSubsystemsDamage(num6 / (this.activeVessel.damagesystem.shipTotalDamagePoints / 5f));
			}
			if (!this.activeVessel.playercontrolled && this.activeVessel.databaseshipdata.shipType != "SUBMARINE")
			{
				if (this.activeVessel.vesselai.attackRole == "KILLER")
				{
					UIFunctions.globaluifunctions.combatai.AssignNewHunterKillers(this.activeVessel, true);
				}
				else if (this.activeVessel.vesselai.attackRole == "HUNTER")
				{
					UIFunctions.globaluifunctions.combatai.AssignNewHunterKillers(this.activeVessel, false);
				}
				if (this.compartmentPosition == "AFT" || this.compartmentPosition == "REAR" || this.compartmentPosition == "MID")
				{
					if (UnityEngine.Random.value < this.compartmentCurrentDamage / this.compartmentMaximumDamage)
					{
						this.activeVessel.damagesystem.maxTelegraph--;
					}
					if (this.compartmentPosition == "AFT" && UnityEngine.Random.value < this.compartmentCurrentDamage / this.compartmentMaximumDamage)
					{
						this.activeVessel.damagesystem.maxTelegraph--;
					}
					if (this.activeVessel.damagesystem.maxTelegraph < 1)
					{
						this.activeVessel.damagesystem.maxTelegraph = 1;
					}
				}
				this.activeVessel.damagesystem.CheckMaxTelegraphValue();
			}
			return;
		}
	}

	// Token: 0x060006ED RID: 1773 RVA: 0x0003B190 File Offset: 0x00039390
	public void ApplyFlooding(int number)
	{
		float num = 0f;
		for (int i = 0; i < number; i++)
		{
			int num2 = UnityEngine.Random.Range(1, 11);
			if (num2 < 6)
			{
				num += UnityEngine.Random.Range(0.1f, 0.2f);
			}
			else if (num2 < 9)
			{
				num += UnityEngine.Random.Range(0.2f, 0.4f);
			}
			else
			{
				num += UnityEngine.Random.Range(0.4f, 0.6f);
			}
		}
		this.activeVessel.damagesystem.shipCurrentDamagePoints += this.activeVessel.damagesystem.shipTotalDamagePoints * (num / 10f);
		this.activeVessel.damagesystem.currentFlooding += num / this.compartmentMaximumDamage;
		this.compartmentCurrentDamage += num * this.compartmentMaximumDamage;
		if (this.compartmentCurrentDamage > this.compartmentMaximumDamage)
		{
			this.compartmentCurrentDamage = this.compartmentMaximumDamage;
		}
		if (this.activeVessel.playercontrolled)
		{
			this.CalculateSubmarineFlooding(true);
			string text = LanguageManager.messageLogDictionary["FloodingInCompartment"];
			text = text.Replace("<COMPARTMENT>", UIFunctions.globaluifunctions.database.GetCompartment(this.compartmentPosition));
			UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentFloodedCompartment = this.compartmentPosition;
			UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["XO"], "FloodingInCompartment", false);
		}
		else if (this.activeVessel.databaseshipdata.shipType == "SUBMARINE")
		{
			this.CalculateSubmarineFlooding(false);
			this.activeVessel.vesselmovement.ballastAngle.x = this.activeVessel.damagesystem.currentFlooding * -4f;
		}
	}

	// Token: 0x060006EE RID: 1774 RVA: 0x0003B374 File Offset: 0x00039574
	private void CalculateSubmarineFlooding(bool player = false)
	{
		float num = 0f;
		int num2 = 0;
		for (int i = 0; i < 10; i++)
		{
			float num3 = this.activeVessel.bouyancyCompartments[i].compartmentCurrentDamage / this.activeVessel.bouyancyCompartments[i].compartmentMaximumDamage;
			i++;
			num3 += this.activeVessel.bouyancyCompartments[i].compartmentCurrentDamage / this.activeVessel.bouyancyCompartments[i].compartmentMaximumDamage;
			num3 = Mathf.Clamp01(num3);
			num += num3;
			if (player)
			{
				this.activeVessel.uifunctions.playerfunctions.damagecontrol.compartmentTotalFlooding[num2] = num3;
				num2++;
			}
		}
		this.activeVessel.damagesystem.currentFlooding = num;
	}

	// Token: 0x060006EF RID: 1775 RVA: 0x0003B438 File Offset: 0x00039638
	private void ApplyFire(int number)
	{
		for (int i = 0; i < number; i++)
		{
			int num = UnityEngine.Random.Range(1, 11);
			if (num < 6)
			{
				this.activeVessel.damagesystem.shipCurrentDamagePoints += this.activeVessel.damagesystem.shipTotalDamagePoints * 0.02f;
			}
			else if (num < 9)
			{
				this.activeVessel.damagesystem.shipCurrentDamagePoints += this.activeVessel.damagesystem.shipTotalDamagePoints * 0.04f;
			}
			else
			{
				this.activeVessel.damagesystem.shipCurrentDamagePoints += this.activeVessel.damagesystem.shipTotalDamagePoints * 0.06f;
			}
		}
		int fire = 1;
		string text = this.compartmentPosition;
		switch (text)
		{
		case "FRONT":
			fire = 0;
			break;
		case "FORE":
			fire = 0;
			break;
		case "AFT":
			fire = 2;
			break;
		case "REAR":
			fire = 2;
			break;
		}
		this.activeVessel.damagesystem.SetFire(fire);
		this.activeVessel.damagesystem.isBurning = true;
	}

	// Token: 0x060006F0 RID: 1776 RVA: 0x0003B5C8 File Offset: 0x000397C8
	public void ApplyExplosion(int number)
	{
		int num = 0;
		for (int i = 0; i < number; i++)
		{
			int num2 = UnityEngine.Random.Range(1, 11);
			if (num2 < 6)
			{
				this.activeVessel.damagesystem.shipCurrentDamagePoints += this.activeVessel.damagesystem.shipTotalDamagePoints * 0.02f;
			}
			else if (num2 < 9)
			{
				this.activeVessel.damagesystem.shipCurrentDamagePoints += this.activeVessel.damagesystem.shipTotalDamagePoints * 0.04f;
				num++;
			}
			else
			{
				this.activeVessel.damagesystem.shipCurrentDamagePoints += this.activeVessel.damagesystem.shipTotalDamagePoints * 0.06f;
				num++;
			}
		}
		string text = this.compartmentPosition;
		switch (text)
		{
		}
		UIFunctions.globaluifunctions.database.CreateExplosion(this.activeVessel, num, base.transform.position);
	}

	// Token: 0x060006F1 RID: 1777 RVA: 0x0003B764 File Offset: 0x00039964
	private int GetNumberOfHits(float damageRatio)
	{
		int num = (int)(damageRatio * 10f);
		num = Mathf.FloorToInt((float)num);
		num = num + UnityEngine.Random.Range(1, 7) - 5;
		return Mathf.Clamp(num, 0, 10);
	}

	// Token: 0x060006F2 RID: 1778 RVA: 0x0003B79C File Offset: 0x0003999C
	private void CheckSubsystemsDamage(float percentageCompartment)
	{
		percentageCompartment = Mathf.Clamp01(percentageCompartment);
		for (int i = 0; i < this.activeVessel.databaseshipdata.subsystemPrimaryPositions.Length; i++)
		{
			bool flag = false;
			if (this.activeVessel.databaseshipdata.subsystemPrimaryPositions[i] == this.compartmentPosition)
			{
				flag = true;
			}
			else if (this.activeVessel.databaseshipdata.subsystemSecondaryPositions[i] == this.compartmentPosition)
			{
				flag = true;
			}
			if (flag && UnityEngine.Random.value < percentageCompartment / 2f)
			{
				bool destroy = false;
				if (UnityEngine.Random.value < percentageCompartment / 2f)
				{
					destroy = true;
				}
				this.activeVessel.uifunctions.playerfunctions.damagecontrol.KnockoutSubsystem(UIFunctions.globaluifunctions.database.databasesubsystemsdata[i].subsystem, destroy);
			}
		}
	}

	// Token: 0x060006F3 RID: 1779 RVA: 0x0003B880 File Offset: 0x00039A80
	public void ApplyDamageDecal(bool destroyed, int placedDecalID)
	{
		if (this.destroyedDecalPlaced)
		{
			return;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate((GameObject)Resources.Load("template_objects/meshTemplate"), base.transform.position, Quaternion.identity) as GameObject;
		gameObject.transform.SetParent(this.activeVessel.meshHolder, false);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		int num;
		Material material;
		if (destroyed)
		{
			this.destroyedDecalPlaced = true;
			num = UnityEngine.Random.Range(0, UIFunctions.globaluifunctions.database.destroyedDecals.Length);
			if (placedDecalID != -1)
			{
				num = placedDecalID;
			}
			material = UIFunctions.globaluifunctions.database.destroyedDecals[num];
			num += 100;
		}
		else
		{
			num = UnityEngine.Random.Range(0, UIFunctions.globaluifunctions.database.damageDecals.Length);
			if (placedDecalID != -1)
			{
				num = placedDecalID;
			}
			material = UIFunctions.globaluifunctions.database.damageDecals[num];
			num += 10;
		}
		MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
		component.material = material;
		component.shadowCastingMode = ShadowCastingMode.Off;
		int num2 = 0;
		string name = base.name;
		switch (name)
		{
		case "11":
			num2 = 0;
			break;
		case "12":
			num2 = 1;
			break;
		case "21":
			num2 = 2;
			break;
		case "22":
			num2 = 3;
			break;
		case "31":
			num2 = 4;
			break;
		case "32":
			num2 = 5;
			break;
		case "41":
			num2 = 6;
			break;
		case "42":
			num2 = 7;
			break;
		case "51":
			num2 = 8;
			break;
		case "52":
			num2 = 9;
			break;
		}
		if (placedDecalID == -1 && this.activeVessel.playercontrolled && !GameDataManager.trainingMode && !GameDataManager.missionMode)
		{
			if (UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.decalNames == null)
			{
				UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.decalNames = new int[10];
			}
			else if (UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.decalNames.Length != 10)
			{
				UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.decalNames = new int[10];
			}
			UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.decalNames[num2] = num;
		}
		if (num2 < this.activeVessel.damagesystem.hullDamageMeshes.Length)
		{
			Mesh mesh = this.activeVessel.damagesystem.hullDamageMeshes[num2];
			gameObject.GetComponent<MeshFilter>().mesh = mesh;
			gameObject.name = "decal";
		}
	}

	// Token: 0x060006F4 RID: 1780 RVA: 0x0003BBDC File Offset: 0x00039DDC
	public void SinkShip(Vessel vesselToSink, bool explosion = false)
	{
		if (vesselToSink.isSinking)
		{
			return;
		}
		if (vesselToSink.databaseshipdata.shipType == "OILRIG" || vesselToSink.vesselmovement.atAnchor)
		{
			vesselToSink.vesselmovement.engineSound.Stop();
			if (vesselToSink.damagesystem.funnelSmoke != null)
			{
				vesselToSink.damagesystem.funnelSmoke.gameObject.SetActive(false);
			}
		}
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.SetShipDisplayed(vesselToSink.vesselListIndex);
		if (UIFunctions.globaluifunctions.playerfunctions.currentTargetIndex == this.activeVessel.vesselListIndex)
		{
			UIFunctions.globaluifunctions.playerfunctions.currentTargetIndex = -1;
			UIFunctions.globaluifunctions.playerfunctions.SetContactToNone();
		}
		this.activeVessel.vesselmovement.isCruising = false;
		this.activeVessel.vesselmovement.rudderAngle = Vector2.zero;
		this.activeVessel.vesselmovement.diveAngle = Vector2.zero;
		if (this.activeVessel.vesselai != null)
		{
			this.activeVessel.vesselai.enabled = false;
			if (this.activeVessel.vesselai.enemymissile != null)
			{
				this.activeVessel.vesselai.enemymissile.enabled = false;
			}
			if (this.activeVessel.vesselai.enemytorpedo != null)
			{
				this.activeVessel.vesselai.enemytorpedo.enabled = false;
			}
			if (this.activeVessel.vesselai.enemymissiledefense != null)
			{
				this.activeVessel.vesselai.enemymissiledefense.StopAllFiring();
				this.activeVessel.vesselai.enemymissiledefense.enabled = false;
			}
			if (this.activeVessel.vesselai.enemymissiledefense != null)
			{
				this.activeVessel.vesselai.enemymissiledefense.StopAllFiring();
				this.activeVessel.vesselai.enemymissiledefense.enabled = false;
			}
			if (this.activeVessel.vesselai.enemynavalguns != null)
			{
				this.activeVessel.vesselai.enemynavalguns.enabled = false;
			}
			if (this.activeVessel.vesselai.enemyrbu != null)
			{
				this.activeVessel.vesselai.enemyrbu.enabled = false;
			}
			UIFunctions.globaluifunctions.playerfunctions.sensormanager.ClearAllSensorData(vesselToSink.vesselListIndex);
		}
		if (this.activeVessel.damagesystem.sinkingBubbles == null)
		{
			UIFunctions.globaluifunctions.database.PlaceSinkingBubbles(this.activeVessel);
		}
		if (!this.activeVessel.playercontrolled)
		{
			UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("range gating", false);
			this.activeVessel.uifunctions.playerfunctions.sensormanager.tacticalmap.mapContact[this.activeVessel.vesselListIndex].shipDisplayIcon.sprite = this.activeVessel.uifunctions.playerfunctions.sensormanager.sonarPaintImages[4];
			if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.initialDetectedByPlayer[this.activeVessel.vesselListIndex])
			{
				UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentVesselIndex = this.activeVessel.vesselListIndex;
				string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "ContactSinking");
				text = text.Replace("<BRG>", string.Format("{0:0}", UIFunctions.globaluifunctions.playerfunctions.sensormanager.bearingToContacts[this.activeVessel.vesselListIndex]));
				text = text.Replace("<CONTACT>", UIFunctions.globaluifunctions.playerfunctions.GetFullContactName(UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.mapContact[this.activeVessel.vesselListIndex].contactText.text, this.activeVessel.vesselListIndex));
				UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["Sonar"], "ContactSinking", false);
			}
		}
		else
		{
			UIFunctions.globaluifunctions.playerfunctions.esmGameObject.SetActive(false);
			UIFunctions.globaluifunctions.playerfunctions.statusIconsParent.SetActive(false);
			this.activeVessel.uifunctions.playerfunctions.sensormanager.tacticalmap.playerMapContact.shipDisplayIcon.sprite = this.activeVessel.uifunctions.playerfunctions.sensormanager.sonarPaintImages[4];
			UIFunctions.globaluifunctions.HUDholder.SetActive(false);
			if (ManualCameraZoom.binoculars)
			{
				GameDataManager.playervesselsonlevel[0].submarineFunctions.LeavePeriscopeView();
			}
			UIFunctions.globaluifunctions.periscopeMatMask.SetActive(false);
			if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.background.activeSelf)
			{
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.SetTacticalMap();
			}
			UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.tacMapCamera.gameObject.SetActive(false);
			UIFunctions.globaluifunctions.keybindManager.gameObject.SetActive(false);
			if (UIFunctions.globaluifunctions.playerfunctions.timeCompressionToggle == 1)
			{
				UIFunctions.globaluifunctions.playerfunctions.SetTimeCompression();
			}
			UIFunctions.globaluifunctions.combatai.DisengageAllShips();
			if (GameDataManager.trainingMode)
			{
				UIFunctions.globaluifunctions.HUDholder.SetActive(true);
				UIFunctions.globaluifunctions.playerfunctions.menuPanel.SetActive(false);
				UIFunctions.globaluifunctions.playerfunctions.otherPanel.SetActive(false);
				UIFunctions.globaluifunctions.bearingMarker.gameObject.SetActive(false);
				UIFunctions.globaluifunctions.helpmanager.trainingIndex = 1;
				UIFunctions.globaluifunctions.helpmanager.GetTrainingContent(UIFunctions.globaluifunctions.helpmanager.trainingIndex, "training_accident", "TRAINING", false);
			}
		}
		if (this.activeVessel.vesselmovement.isSubmarine)
		{
			this.activeVessel.vesselmovement.ballastAngle = Vector2.zero;
			if (vesselToSink.vesselmovement.activeSubWake != null)
			{
				vesselToSink.vesselmovement.activeSubWake.emissionRate = 0f;
			}
			this.activeVessel.damagesystem.sinkingBubbles.gameObject.transform.position = base.transform.position;
			float num = 0f;
			if (this.activeVessel.playercontrolled)
			{
				num += UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentTotalFlooding[0] + UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentTotalFlooding[1];
				num -= UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentTotalFlooding[3] + UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentTotalFlooding[4];
				if (num > 0f)
				{
					num = (float)UnityEngine.Random.Range(0, 60);
				}
				else if (num < 0f)
				{
					num = (float)UnityEngine.Random.Range(-60, 0);
				}
				else
				{
					num = (float)UnityEngine.Random.Range(-60, 60);
				}
			}
			else if (this.compartmentPosition == "FRONT" || this.compartmentPosition == "FORE")
			{
				num = (float)UnityEngine.Random.Range(0, 60);
			}
			else if (this.compartmentPosition == "AFT" || this.compartmentPosition == "REAR")
			{
				num = (float)UnityEngine.Random.Range(-60, 0);
			}
			else
			{
				num = (float)UnityEngine.Random.Range(-60, 60);
			}
			this.activeVessel.vesselmovement.subSinkingAngles = new Vector2(num, (float)UnityEngine.Random.Range(-180, 180));
			this.activeVessel.vesselmovement.subSinkingRates = new Vector2(UnityEngine.Random.Range(0.1f, 1f), UnityEngine.Random.Range(0.1f, 1f));
		}
		vesselToSink.isSinking = true;
		this.activeVessel.vesselmovement.bowwave.Stop();
		vesselToSink.damagesystem.sinkingBubbles.Play();
		if (UnityEngine.Random.value < 0.3f)
		{
			this.isCapsizing = true;
		}
		if (!this.activeVessel.playercontrolled)
		{
			UIFunctions.globaluifunctions.combatai.AIAdjustTelegraph(this.activeVessel, 1);
		}
		else
		{
			vesselToSink.vesselmovement.telegraphValue = 1;
			vesselToSink.vesselmovement.engineSpeed.x = (float)(-1 + vesselToSink.vesselmovement.telegraphValue) / 5f * vesselToSink.vesselmovement.shipSpeed.y;
		}
		vesselToSink.vesselmovement.rudderAngle.x = 0f;
		if (vesselToSink.damagesystem.funnelSmoke != null)
		{
			vesselToSink.damagesystem.funnelSmoke.Stop();
		}
		for (int i = 0; i < vesselToSink.damagesystem.radars.Length; i++)
		{
			vesselToSink.damagesystem.radars[i].isDestroyed = true;
		}
		float num2 = 0f;
		Compartment compartment = vesselToSink.bouyancyCompartments[0];
		foreach (Compartment compartment2 in vesselToSink.bouyancyCompartments)
		{
			float num3 = compartment2.compartmentCurrentDamage;
			if (num3 > num2)
			{
				num2 = num3;
				compartment = compartment2;
			}
		}
		if (!this.activeVessel.isSubmarine && (explosion || !vesselToSink.damagesystem.destroyedMesh))
		{
			vesselToSink.uifunctions.database.CreateMagazineExplosion(vesselToSink.transform.position);
			if (!vesselToSink.damagesystem.destroyedMesh)
			{
				vesselToSink.damagesystem.SetMeshes();
			}
		}
		compartment.isSinking = true;
		compartment.canflood = true;
		if (vesselToSink.submerged)
		{
			vesselToSink.submerged = false;
		}
		if (!vesselToSink.vesselmovement.isSubmarine)
		{
			vesselToSink.WakeVessel(20f);
		}
		UIFunctions.globaluifunctions.database.PlaceScreechAudioSource(this.activeVessel);
		GameObject prefab = vesselToSink.uifunctions.database.oilSlicks[2];
		if (vesselToSink.isSubmarine)
		{
			prefab = vesselToSink.uifunctions.database.oilSlicks[0];
		}
		GameObject gameObject = ObjectPoolManager.CreatePooled(prefab, new Vector3(vesselToSink.transform.position.x, 1000f, vesselToSink.transform.position.z), Quaternion.Euler(0f, GameDataManager.smokeAngle + 180f, 0f));
		gameObject.transform.parent = vesselToSink.transform;
	}

	// Token: 0x04000837 RID: 2103
	public Vessel activeVessel;

	// Token: 0x04000838 RID: 2104
	public int compartmentType;

	// Token: 0x04000839 RID: 2105
	public float compartmentHeight;

	// Token: 0x0400083A RID: 2106
	public float floodingAmount;

	// Token: 0x0400083B RID: 2107
	public float sinkRate;

	// Token: 0x0400083C RID: 2108
	public float lowestYValue;

	// Token: 0x0400083D RID: 2109
	public bool isSinking;

	// Token: 0x0400083E RID: 2110
	public bool isCapsizing;

	// Token: 0x0400083F RID: 2111
	public bool isBurning;

	// Token: 0x04000840 RID: 2112
	public int whichNavy;

	// Token: 0x04000841 RID: 2113
	public float compartmentArmor;

	// Token: 0x04000842 RID: 2114
	public float compartmentTorpDefense;

	// Token: 0x04000843 RID: 2115
	public float compartmentMaximumDamage;

	// Token: 0x04000844 RID: 2116
	public float compartmentCurrentDamage;

	// Token: 0x04000845 RID: 2117
	public bool isSleeping;

	// Token: 0x04000846 RID: 2118
	public Compartment linkedCompartment1;

	// Token: 0x04000847 RID: 2119
	public Compartment linkedCompartment2;

	// Token: 0x04000848 RID: 2120
	public bool canflood;

	// Token: 0x04000849 RID: 2121
	public string compartmentPosition;

	// Token: 0x0400084A RID: 2122
	public bool destroyedDecalPlaced;

	// Token: 0x0400084B RID: 2123
	public bool createdDust;
}
