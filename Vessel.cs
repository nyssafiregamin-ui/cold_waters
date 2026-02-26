using System;
using UnityEngine;

// Token: 0x0200016A RID: 362
public class Vessel : MonoBehaviour
{
	// Token: 0x06000AFE RID: 2814 RVA: 0x00098D0C File Offset: 0x00096F0C
	public void Start()
	{
		this.uifunctions = UIFunctions.globaluifunctions;
		if (this.uifunctions.museumObject.activeSelf)
		{
			this.vesselmovement.wakeObject.SetActive(false);
			base.enabled = false;
			this.vesselmovement.enabled = false;
			if (this.vesselmovement.parentVessel.damagesystem.funnelSmoke != null)
			{
				this.vesselmovement.parentVessel.damagesystem.funnelSmoke.Stop();
			}
			return;
		}
		this.waterLevel = 1000f;
		this.centerOfMassOffset = new Vector3(0f, -0.01f, 0f);
		if (this.playercontrolled)
		{
			this.uifunctions.portRearm.InitialisePortRearmRepair();
			if (this.vesselmovement.weaponSource.noisemakersOnBoard == 0)
			{
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[2].color = UIFunctions.globaluifunctions.playerfunctions.helmmanager.buttonColors[0];
			}
		}
		else
		{
			if (this.vesselai.hasNoiseMaker)
			{
				this.vesselai.enemynoisemaker.InitialiseEnemyNoisemaker();
			}
			if (this.vesselai.hasTorpedo)
			{
				this.vesselai.enemytorpedo.InitialiseEnemyTorpedo();
			}
			if (this.vesselai.hasMissile)
			{
				this.vesselai.enemymissile.InitialiseEnemyMissile();
			}
			if (this.vesselai.hasMissileDefense)
			{
				this.vesselai.enemymissiledefense.InitialiseEnemyMissileDefense();
			}
			if (GameDataManager.trainingMode)
			{
				this.vesselai.hasMissile = false;
				this.vesselai.hasTorpedo = false;
				this.vesselai.hasRBU = false;
				this.vesselai.hasMissileDefense = false;
				this.vesselai.hasNavalGuns = false;
				this.vesselai.hasNoiseMaker = false;
			}
		}
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x00098F04 File Offset: 0x00097104
	private void ApplyVesselMovement()
	{
		this.vesselmovement.timeInterval = Time.deltaTime;
		this.vesselmovement.MoveVessel();
	}

	// Token: 0x06000B00 RID: 2816 RVA: 0x00098F24 File Offset: 0x00097124
	private void FixedUpdate()
	{
		if (this.submarineFunctions != null)
		{
			bool flag = false;
			float num = -10f;
			int num2 = 0;
			for (int i = 0; i < this.submarineFunctions.mastTransforms.Length; i++)
			{
				if (this.submarineFunctions.scopeStatus[i] == 1 && this.submarineFunctions.parentVesselMovement.shipSpeed.z > 0.5f && this.submarineFunctions.mastHeads[i].position.y > 1000f && this.submarineFunctions.mastTransforms[i].position.y < 1000f)
				{
					flag = true;
					if (this.submarineFunctions.mastHeads[i].localPosition.z > num)
					{
						num = this.submarineFunctions.mastHeads[i].localPosition.z;
						num2 = i;
					}
				}
			}
			if (flag)
			{
				if (!this.submarineFunctions.periwaveParticle.isPlaying)
				{
					this.submarineFunctions.periwaveParticle.Play();
				}
			}
			else if (this.submarineFunctions.periwaveParticle.isPlaying)
			{
				this.submarineFunctions.periwaveParticle.Stop();
			}
			if (this.submarineFunctions.periwaveParticle.isPlaying)
			{
				this.submarineFunctions.periwaveParticle.transform.position = new Vector3(this.submarineFunctions.mastHeads[num2].position.x, 1000f, this.submarineFunctions.mastHeads[num2].position.z);
			}
		}
		if (this.vesselmovement != null)
		{
			this.ApplyVesselMovement();
		}
		if (this.sleepTimer > 0f)
		{
			this.sleepTimer -= Time.deltaTime;
		}
		else if (!this.isSinking && !this.isCapsizing)
		{
			this.SleepVessel();
		}
		if (this.databaseshipdata.shipType != "SUBMARINE" && this.isSleeping && this.damagesystem.shipCurrentDamagePoints > 0f && !this.submerged && !this.doonce)
		{
			this.WakeVessel(20f);
			this.doonce = true;
		}
		this.sleepTimer += 1f;
		if (this.damagesystem.isBurning)
		{
			this.damagesystem.CheckFireHeight();
		}
		Vector3 position = base.transform.position;
		if (this.isSinking)
		{
			if (position.y < 990f)
			{
				if (this.damagesystem.screechSound != null)
				{
					this.damagesystem.screechSound.Stop();
				}
				base.gameObject.SetActive(false);
				base.transform.Translate(Vector3.up * -500f);
				return;
			}
			if (!this.vesselmovement.isSubmarine)
			{
				float @float = this.vesselmovement.wakeRenderer[0].material.GetFloat("_MMultiplier");
				if (@float > 0f)
				{
					for (int j = 0; j < this.vesselmovement.wakeRenderer.Length; j++)
					{
						this.vesselmovement.wakeRenderer[j].material.SetFloat("_MMultiplier", this.vesselmovement.wakeRenderer[j].material.GetFloat("_MMultiplier") - Time.deltaTime * 0.1f);
					}
				}
			}
		}
		if (!this.isSleeping)
		{
			this.vesselTotalFlooding = 0f;
			float num3 = 0f;
			for (int k = 0; k < this.bouyancyCompartments.Length; k++)
			{
				Compartment compartment = this.bouyancyCompartments[k];
				float floodingAmount = compartment.floodingAmount;
				num3 += floodingAmount;
				this.vesselTotalFlooding += compartment.compartmentCurrentDamage;
				if (this.isCapsizing)
				{
					compartment.isCapsizing = true;
				}
			}
			if (num3 < 1f)
			{
				num3 *= num3;
			}
			float num4 = num3 * this.damageBouyancy;
			if (num3 > 0f && this.waterLevel < this.waterLevel + num4)
			{
				num4 += this.waterLevel;
			}
			else
			{
				num4 = this.waterLevel;
			}
			for (int l = 0; l < this.bouyancyCompartments.Length; l++)
			{
				Vector3 position2 = this.bouyancyCompartments[l].transform.position;
				float num5 = (1f - (position2.y - num4) / this.floatHeight) / (float)this.bouyancyCompartments.Length;
				if (num5 > 0f)
				{
					Vector3 force = -Physics.gravity * (num5 - base.GetComponent<Rigidbody>().velocity.y * (this.bounceDamp / (float)this.bouyancyCompartments.Length * Time.deltaTime));
					Compartment compartment2 = this.bouyancyCompartments[l];
					float floodingAmount2 = compartment2.floodingAmount;
					force.y *= 1f - floodingAmount2;
					base.GetComponent<Rigidbody>().AddForceAtPosition(force, position2);
				}
			}
			if (this.isCapsizing)
			{
				if (this.capsizevalue < 0.2f)
				{
					this.capsizevalue += 0.0002f * Time.deltaTime;
					base.GetComponent<Rigidbody>().centerOfMass = new Vector3(0f, this.centerOfMassOffset.y + this.capsizevalue, 0f);
				}
			}
			else
			{
				base.GetComponent<Rigidbody>().centerOfMass = this.centerOfMassOffset;
			}
		}
		if (this.isCapsizing || this.isSinking)
		{
			this.vesselmovement.shipSpeed.y = 0f;
			if (this.damagesystem.screechSound == null)
			{
				UIFunctions.globaluifunctions.database.PlaceScreechAudioSource(this.vesselmovement.parentVessel);
			}
			if (!this.damagesystem.screechSound.isPlaying && position.y < 1001f)
			{
				int num6 = UnityEngine.Random.Range(0, 9);
				this.damagesystem.screechSound.clip = UIFunctions.globaluifunctions.database.screeches[num6];
				this.damagesystem.screechSound.Play();
			}
		}
	}

	// Token: 0x06000B01 RID: 2817 RVA: 0x000995D4 File Offset: 0x000977D4
	public void WakeVessel(float time)
	{
		if (this.isSleeping)
		{
			foreach (Compartment compartment in this.bouyancyCompartments)
			{
				compartment.isSleeping = false;
			}
			this.isSleeping = false;
			this.sleepTimer = time;
			this.vesselRigidbody.useGravity = true;
		}
		else
		{
			this.sleepTimer = time;
		}
	}

	// Token: 0x06000B02 RID: 2818 RVA: 0x00099638 File Offset: 0x00097838
	public void SleepVessel()
	{
		foreach (Compartment compartment in this.bouyancyCompartments)
		{
			compartment.isSleeping = true;
		}
		this.isSleeping = true;
		this.vesselRigidbody.useGravity = false;
	}

	// Token: 0x0400113A RID: 4410
	public UIFunctions uifunctions;

	// Token: 0x0400113B RID: 4411
	public DatabaseShipData databaseshipdata;

	// Token: 0x0400113C RID: 4412
	public VesselMovement vesselmovement;

	// Token: 0x0400113D RID: 4413
	public DamageSystem damagesystem;

	// Token: 0x0400113E RID: 4414
	public Acoustics acoustics;

	// Token: 0x0400113F RID: 4415
	public int vesselListIndex;

	// Token: 0x04001140 RID: 4416
	public bool isSubmarine;

	// Token: 0x04001141 RID: 4417
	public SubmarineFunctions submarineFunctions;

	// Token: 0x04001142 RID: 4418
	public VesselAI vesselai;

	// Token: 0x04001143 RID: 4419
	public int shipID;

	// Token: 0x04001144 RID: 4420
	public float movementTimer;

	// Token: 0x04001145 RID: 4421
	public bool playercontrolled;

	// Token: 0x04001146 RID: 4422
	public float waterLevel;

	// Token: 0x04001147 RID: 4423
	private float floatHeight = 1f;

	// Token: 0x04001148 RID: 4424
	private float bounceDamp = 0.05f;

	// Token: 0x04001149 RID: 4425
	public float damageBouyancy;

	// Token: 0x0400114A RID: 4426
	public float compartmentvolume;

	// Token: 0x0400114B RID: 4427
	public Vector3 centerOfMassOffset;

	// Token: 0x0400114C RID: 4428
	public Compartment[] bouyancyCompartments;

	// Token: 0x0400114D RID: 4429
	public MeshCollider hullCollider;

	// Token: 0x0400114E RID: 4430
	public MeshCollider[] superstructureColliders;

	// Token: 0x0400114F RID: 4431
	public bool isCapsizing;

	// Token: 0x04001150 RID: 4432
	public bool isSinking;

	// Token: 0x04001151 RID: 4433
	public float sinkModifier;

	// Token: 0x04001152 RID: 4434
	public bool submerged;

	// Token: 0x04001153 RID: 4435
	public float lookDistance;

	// Token: 0x04001154 RID: 4436
	public float vesselTotalFlooding;

	// Token: 0x04001155 RID: 4437
	public int turnNumber;

	// Token: 0x04001156 RID: 4438
	public int whichNavy;

	// Token: 0x04001157 RID: 4439
	public bool isSleeping;

	// Token: 0x04001158 RID: 4440
	public float sleepTimer;

	// Token: 0x04001159 RID: 4441
	public bool doonce;

	// Token: 0x0400115A RID: 4442
	public Rigidbody vesselRigidbody;

	// Token: 0x0400115B RID: 4443
	public float capsizevalue;

	// Token: 0x0400115C RID: 4444
	public float bouyancymodifier;

	// Token: 0x0400115D RID: 4445
	public int takenaction;

	// Token: 0x0400115E RID: 4446
	public int torpsfired;

	// Token: 0x0400115F RID: 4447
	public Transform meshHolder;

	// Token: 0x04001160 RID: 4448
	public Transform weaponsHolder;

	// Token: 0x04001161 RID: 4449
	public bool wakeMoving;

	// Token: 0x04001162 RID: 4450
	public int wakedirection;

	// Token: 0x04001163 RID: 4451
	public float wakeSpeed;

	// Token: 0x04001164 RID: 4452
	public float wakeuvOffset;

	// Token: 0x04001165 RID: 4453
	public Vector4 lastSliderValues;

	// Token: 0x04001166 RID: 4454
	public float CurrentElevation;

	// Token: 0x04001167 RID: 4455
	public float CurrentHeading;

	// Token: 0x04001168 RID: 4456
	public float CurrentAircraftElevation;

	// Token: 0x04001169 RID: 4457
	public float CurrentAircraftHeading;

	// Token: 0x0400116A RID: 4458
	public int lastPlayerSlotAttacked;

	// Token: 0x0400116B RID: 4459
	public bool forcedmove;

	// Token: 0x0400116C RID: 4460
	public int vesselNumberofMoves;

	// Token: 0x0400116D RID: 4461
	public Vector2[] vesselPositionHistory;

	// Token: 0x0400116E RID: 4462
	public bool inConvoy;

	// Token: 0x0400116F RID: 4463
	public Vessel[] convoy;

	// Token: 0x04001170 RID: 4464
	public float wakeRotationMod;

	// Token: 0x04001171 RID: 4465
	public Transform[] bMarkers;

	// Token: 0x04001172 RID: 4466
	public Vessel lastTarget;

	// Token: 0x04001173 RID: 4467
	public int lastTargetVesselIndex;

	// Token: 0x04001174 RID: 4468
	public int turnsOnTarget;

	// Token: 0x04001175 RID: 4469
	public string lastTargetName;

	// Token: 0x04001176 RID: 4470
	public int speedDirection;

	// Token: 0x04001177 RID: 4471
	public float bowwaveHeight;

	// Token: 0x04001178 RID: 4472
	public float wakeMultiplier;

	// Token: 0x04001179 RID: 4473
	public int bowwavedirection;

	// Token: 0x0400117A RID: 4474
	public bool vesselRemovedFromCombat;
}
