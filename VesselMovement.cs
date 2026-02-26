using System;
using Ceto;
using UnityEngine;

// Token: 0x0200016E RID: 366
public class VesselMovement : MonoBehaviour
{
	// Token: 0x06000B27 RID: 2855 RVA: 0x000A1D00 File Offset: 0x0009FF00
	private void Start()
	{
		if (this.parentVessel.databaseshipdata.shipType != "SUBMARINE")
		{
			this.hullFoam.gameObject.SetActive(true);
			this.sternFoam.gameObject.SetActive(true);
			this.hullFoam.Play();
			this.sternFoam.Play();
			this.percentageSurfaced = 1f;
		}
		else
		{
			this.percentageSurfaced = 0f;
			this.cavBubbles.Stop();
		}
		this.enginepitchdifference = this.enginePitchRange.y - this.enginePitchRange.x;
		this.proppitchdifference = this.propPitchRange.y - this.propPitchRange.x;
		this.engineSound.Play();
		this.propSound.Play();
		this.rudderSpeedModifier = 1f;
		this.cavTimer = UnityEngine.Random.Range(5f, 9f);
		if (this.parentVessel.databaseshipdata.shipType == "OILRIG")
		{
			this.atAnchor = true;
			this.parentVessel.vesselai.isNeutral = true;
			if (this.propSound != null)
			{
				this.propSound.Stop();
			}
		}
	}

	// Token: 0x06000B28 RID: 2856 RVA: 0x000A1E50 File Offset: 0x000A0050
	public float GetActualVesselSpeed()
	{
		if (this.parentVessel.playercontrolled && UIFunctions.globaluifunctions.playerfunctions.usingKnots)
		{
			return UIFunctions.globaluifunctions.playerfunctions.currentKnots * this.rudderSpeedModifier;
		}
		return this.parentVessel.databaseshipdata.telegraphSpeeds[this.telegraphValue];
	}

	// Token: 0x06000B29 RID: 2857 RVA: 0x000A1EB0 File Offset: 0x000A00B0
	private void FixedUpdate()
	{
		this.engineSoundsTimer += Time.deltaTime;
		if (this.engineSoundsTimer > 0.2f)
		{
			this.RecalculateEngineSounds();
			this.engineSoundsTimer -= 0.2f;
		}
		if (this.parentVessel.databaseshipdata.shipType == "SUBMARINE" && this.cavTimer > 0f)
		{
			this.cavTimer -= Time.deltaTime;
			if (this.cavTimer <= 0f)
			{
				this.CheckIfCavitating();
			}
		}
	}

	// Token: 0x06000B2A RID: 2858 RVA: 0x000A1F50 File Offset: 0x000A0150
	private void RecalculateEngineSounds()
	{
		if (this.shipSpeed.x == 0f)
		{
			return;
		}
		float num = this.engineSpeed.y / (this.shipSpeed.x / 10f);
		if (this.engineSpeed.y < 0f)
		{
			num = -this.engineSpeed.y / (this.shipSpeed.x / 10f);
		}
		float pitch = num * this.enginepitchdifference + this.enginePitchRange.x;
		this.engineSound.pitch = pitch;
		pitch = num * this.proppitchdifference + this.propPitchRange.x;
		this.propSound.pitch = pitch;
		if (this.bowwave.isPlaying)
		{
			this.bowwaveSound.volume = this.percentageSpeed;
		}
		else
		{
			this.bowwaveSound.volume = 0f;
		}
	}

	// Token: 0x06000B2B RID: 2859 RVA: 0x000A2040 File Offset: 0x000A0240
	public void InitialiseVesselMovement()
	{
		this.accelerationRate = this.parentVessel.databaseshipdata.accelerationrate;
		this.decelerationRate = this.parentVessel.databaseshipdata.decellerationrate;
		this.turnSpeed = this.parentVessel.databaseshipdata.turnrate;
		this.diveRate = this.parentVessel.databaseshipdata.diverate;
		this.surfaceRate = this.parentVessel.databaseshipdata.surfacerate;
		this.ballastRate = this.parentVessel.databaseshipdata.ballastrate;
		this.shipSpeed.x = this.parentVessel.databaseshipdata.surfacespeed;
		if (this.parentVessel.isSubmarine)
		{
			this.shipSpeed.x = this.parentVessel.databaseshipdata.submergedspeed;
			this.surfaceSpeedPenalty = 1f - this.parentVessel.databaseshipdata.surfacespeed / this.parentVessel.databaseshipdata.submergedspeed;
		}
		this.shipSpeed.y = this.shipSpeed.x / 10f;
		this.meshHolderInitialY = this.parentVessel.meshHolder.transform.localPosition.y;
		this.telegraphValue = 3;
		if (this.parentVessel.isSubmarine)
		{
			this.telegraphValue = 2;
		}
		if (!this.parentVessel.playercontrolled)
		{
			this.isCruising = true;
			this.cruiseSpeed = UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.formationCruiseSpeed;
			this.parentVessel.vesselmovement.shipSpeed.z = this.cruiseSpeed;
			if (this.parentVessel.vesselmovement.atAnchor)
			{
				this.cruiseSpeed = 0f;
				this.isCruising = false;
				this.parentVessel.vesselmovement.shipSpeed.z = 0f;
				this.parentVessel.vesselmovement.telegraphValue = 1;
				this.engineSound.Stop();
			}
			else if (this.parentVessel.vesselai.isNeutral)
			{
				this.cruiseSpeed = this.parentVessel.databaseshipdata.telegraphSpeeds[5];
				this.isCruising = false;
				this.parentVessel.vesselmovement.telegraphValue = 5;
			}
			if (this.parentVessel.databaseshipdata.shipType == "OILRIG")
			{
				this.parentVessel.vesselmovement.telegraphValue = 1;
				this.parentVessel.vesselmovement.shipSpeed.z = 0f;
				this.parentVessel.vesselmovement.cruiseSpeed = 0f;
				this.parentVessel.vesselmovement.atAnchor = true;
			}
		}
		else
		{
			if (!GameDataManager.trainingMode && !GameDataManager.missionMode)
			{
				if (UIFunctions.globaluifunctions.campaignmanager.playerCurrentSpeed > UIFunctions.globaluifunctions.campaignmanager.playerMapSpeeds[1])
				{
					this.telegraphValue = UIFunctions.globaluifunctions.campaignmanager.playerStartTelegraphs[2];
				}
				else if (UIFunctions.globaluifunctions.campaignmanager.playerCurrentSpeed > UIFunctions.globaluifunctions.campaignmanager.playerMapSpeeds[0])
				{
					this.telegraphValue = UIFunctions.globaluifunctions.campaignmanager.playerStartTelegraphs[1];
				}
				else
				{
					this.telegraphValue = UIFunctions.globaluifunctions.campaignmanager.playerStartTelegraphs[0];
				}
				if (this.telegraphValue > 5)
				{
					this.telegraphValue = 5;
				}
				if (UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.destroyedSubsystems != null && UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.destroyedSubsystems[DamageControl.GetSubsystemIndex("PROPULSION")])
				{
					this.telegraphValue = 3;
					UIFunctions.globaluifunctions.playerfunctions.damagecontrol.SetPlayerMaxTelegraph(3);
				}
			}
			this.shipSpeed.z = this.GetActualVesselSpeed();
		}
		this.engineSpeed.x = this.shipSpeed.z;
		this.engineSpeed.y = this.engineSpeed.x;
		this.bowwaveHeight = -0.025f;
		if (this.bowwaveHeight > 0f)
		{
			this.bowwaveHeight = 0f;
		}
		this.bowwave.transform.localPosition = new Vector3(0f, this.bowwaveHeight, this.bowwave.transform.localPosition.z);
		if (!this.isSubmarine)
		{
			this.hullFoam.startColor = global::Environment.whiteLevel;
			this.sternFoam.startColor = global::Environment.whiteLevel;
			for (int i = 0; i < this.wakeRenderer.Length; i++)
			{
				this.wakeRenderer[i].material.SetFloat("_ScrollX", 0.015f * (1f / this.wakeRenderer[i].gameObject.transform.parent.transform.lossyScale.x));
				this.wakeRenderer[i].material.SetFloat("_MMultiplier", (0.1f + this.bowwaveHeight) * 10f);
			}
			if (this.cavBubbles != null)
			{
				this.cavBubbles.Play();
				this.cavBubbles.subEmitters.enabled = true;
			}
			if (this.bowwave != null)
			{
				this.bowwave.Play();
			}
		}
		if (this.bowwave != null)
		{
			this.bowwave.subEmitters.enabled = true;
		}
		if (this.propwash != null)
		{
			this.propwash.subEmitters.enabled = true;
		}
		for (int j = 0; j < this.wakeRenderer.Length; j++)
		{
			this.wakeRenderer[j].material.SetColor("_TintColor", global::Environment.whiteLevel);
		}
		if (!this.isSubmarine)
		{
			base.transform.position = new Vector3(base.transform.position.x, 1000f, base.transform.position.z);
		}
		else
		{
			float num = 1000f - this.parentVessel.databaseshipdata.actualTestDepth;
			this.crushDepth = this.parentVessel.databaseshipdata.actualTestDepth - num * UnityEngine.Random.Range(0.5f, 0.75f);
			this.currentCrushDepth = this.crushDepth;
			this.bowwave.Stop();
			this.bowwave.Clear();
			this.foamTrails[0].foamTexture = null;
			this.foamTrails[1].foamTexture = null;
			if (this.propwash != null)
			{
				this.propwash.Stop();
				this.propwash.Clear();
			}
			this.penaltyRange = this.parentVessel.databaseshipdata.waterline - this.parentVessel.databaseshipdata.submergedat;
			if (this.penaltyRange <= 0f)
			{
				this.penaltyRange = 0.001f;
			}
			this.wakeObject.SetActive(true);
		}
	}

	// Token: 0x06000B2C RID: 2860 RVA: 0x000A278C File Offset: 0x000A098C
	public void MoveVessel()
	{
		if (!this.atAnchor)
		{
			if (base.transform.eulerAngles.z != this.vesselUprightRotation && this.parentVessel.damagesystem.currentFlooding == 0f)
			{
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y, this.vesselUprightRotation), 0.8f * this.timeInterval);
			}
			else if (this.parentVessel.databaseshipdata.shipType == "SUBMARINE")
			{
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y, this.vesselUprightRotation), 0.8f * this.timeInterval);
			}
			float num = Vector3.Distance(base.transform.position, UIFunctions.globaluifunctions.MainCamera.transform.position);
			if (num < 20f)
			{
			}
			float y = base.transform.position.y;
			if (this.rudderAngle.y != this.rudderAngle.x)
			{
				if (Mathf.Abs(this.rudderAngle.y - this.rudderAngle.x) < 0.05f)
				{
					this.rudderAngle.y = this.rudderAngle.x;
				}
				else if (this.rudderAngle.y < this.rudderAngle.x)
				{
					this.rudderAngle.y = this.rudderAngle.y + this.parentVessel.databaseshipdata.rudderturnrate * this.timeInterval;
				}
				else if (this.rudderAngle.y > this.rudderAngle.x)
				{
					this.rudderAngle.y = this.rudderAngle.y - this.parentVessel.databaseshipdata.rudderturnrate * this.timeInterval;
				}
				for (int i = 0; i < this.rudder.Length; i++)
				{
					this.rudder[i].transform.localRotation = Quaternion.Slerp(this.rudder[i].rotation, Quaternion.Euler(0f, -this.rudderAngle.y * 10f, 0f), 1f);
				}
			}
			if (this.shipSpeed.y > 0f)
			{
				float num2 = this.rudderAngle.y * 11f;
				if (num2 < 0f)
				{
					num2 *= -1f;
				}
				if (num2 > 2f)
				{
					this.rudderSpeedModifier = 1f - num2 / 300f;
				}
				else
				{
					this.rudderSpeedModifier = 1f;
				}
			}
			if (this.parentVessel.playercontrolled)
			{
				float currentFlooding = this.parentVessel.damagesystem.currentFlooding;
				if (currentFlooding > 0f)
				{
					this.rudderSpeedModifier -= currentFlooding * 2f;
					float num3 = base.transform.eulerAngles.x;
					if (num3 > 180f)
					{
						num3 -= 360f;
					}
					if (num3 < 0f)
					{
						this.rudderSpeedModifier += num3 / 90f;
					}
					if (this.rudderSpeedModifier < 0f)
					{
						this.rudderSpeedModifier = 0f;
					}
				}
			}
			if (this.isSubmarine)
			{
				if (this.parentVessel.isSinking)
				{
					base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.Euler(this.subSinkingAngles.x, base.transform.eulerAngles.y, base.transform.eulerAngles.z), this.subSinkingRates.x * Time.deltaTime);
					base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y, this.subSinkingAngles.y), this.subSinkingRates.y * Time.deltaTime);
				}
				else if (y < this.currentCrushDepth)
				{
					UIFunctions.globaluifunctions.playerfunctions.eventcamera.CheckForEventCamera(this.parentVessel.transform, null, 10f, false, true, false, -1f, -1f, -1f, true);
					this.CrushSubmarine();
				}
				this.currentSurfaceSpeedPenalty = 1f;
				this.percentageSurfaced = 0f;
				if (y > this.parentVessel.databaseshipdata.submergedat)
				{
					this.percentageSurfaced = (y - this.parentVessel.databaseshipdata.submergedat) / this.penaltyRange;
					this.currentSurfaceSpeedPenalty = 1f - this.percentageSurfaced * this.surfaceSpeedPenalty;
				}
				else if (this.bowwave.isPlaying)
				{
					this.bowwave.Stop();
				}
				if (!this.parentVessel.isSinking)
				{
					this.SubDivePlanes(y);
				}
			}
			float num4 = this.GetActualVesselSpeed();
			if (this.isCruising)
			{
				if (this.parentVessel.damagesystem.maxTelegraph == 6)
				{
					num4 = this.cruiseSpeed;
				}
				else
				{
					num4 = (float)(-1 + this.parentVessel.damagesystem.maxTelegraph) / 5f * this.shipSpeed.y;
					if (num4 > this.cruiseSpeed)
					{
						num4 = this.cruiseSpeed;
					}
				}
			}
			if (this.isSubmarine)
			{
				num4 *= this.currentSurfaceSpeedPenalty;
			}
			this.percentageSpeed = this.shipSpeed.z / (this.shipSpeed.x / 10f);
			if (this.shipSpeed.z != num4)
			{
				if (Mathf.Abs(this.shipSpeed.z - num4) < 0.035f)
				{
					if (num4 != 0f)
					{
						this.shipSpeed.z = num4;
					}
					if (this.parentVessel.isSinking)
					{
						this.shipSpeed.z = 0f;
						if (this.parentVessel.databaseshipdata.shipType != "SUBMARINE")
						{
							this.hullFoam.emissionRate = 0f;
							this.sternFoam.emissionRate = 0f;
						}
						if (this.engineSound != null)
						{
							this.engineSound.Stop();
						}
						if (this.propSound != null)
						{
							this.propSound.Stop();
						}
					}
				}
				else if (this.shipSpeed.z < num4)
				{
					this.shipSpeed.z = this.shipSpeed.z + this.accelerationRate * this.timeInterval * UIFunctions.globaluifunctions.gamedatamanager.accelerationCurve.Evaluate(this.percentageSpeed) * 0.3f;
				}
				else if (this.shipSpeed.z > num4)
				{
					if (this.telegraphValue != 0)
					{
						this.shipSpeed.z = this.shipSpeed.z - this.decelerationRate * this.timeInterval * UIFunctions.globaluifunctions.gamedatamanager.decelerationCurve.Evaluate(this.percentageSpeed);
					}
					else
					{
						this.shipSpeed.z = this.shipSpeed.z - this.accelerationRate * this.timeInterval;
					}
					if (this.parentVessel.isSinking)
					{
						this.shipSpeed.z = this.shipSpeed.z - this.decelerationRate * this.timeInterval;
						if (this.parentVessel.playercontrolled)
						{
							this.shipSpeed.z = this.shipSpeed.z - this.decelerationRate * this.timeInterval * 2f;
						}
					}
				}
			}
			if (this.shipSpeed.z > this.parentVessel.databaseshipdata.telegraphSpeeds[6])
			{
				this.shipSpeed.z = this.parentVessel.databaseshipdata.telegraphSpeeds[6];
			}
			if (this.shipSpeed.z < 0.5f && !this.isCavitating)
			{
				if (this.bowwave != null)
				{
					this.bowwave.Stop();
				}
				if (this.propwash != null)
				{
					this.propwash.Stop();
				}
				if (this.cavBubbles != null)
				{
					this.cavBubbles.Stop();
				}
				if (this.parentVessel.isSinking)
				{
					this.parentVessel.vesselmovement.enabled = false;
				}
			}
			if (!this.isSubmarine)
			{
				float num5 = 1f - (1000f - base.transform.position.y) / 0.2f;
				if (num5 > 0.02f)
				{
					float num6 = 200f / this.parentVessel.compartmentvolume * 1f + 0.4f;
					float xclock = GameDataManager.xclock;
					float num7 = num6 * GameDataManager.yclock / 6f;
					this.meshHolder.transform.localRotation = Quaternion.Slerp(this.meshHolder.transform.localRotation, Quaternion.Euler(num7, 0f, num6 * xclock), 1f);
					num7 /= 100f;
					this.meshHolder.transform.localPosition = new Vector3(this.meshHolder.transform.localPosition.x, this.meshHolderInitialY + num7, this.meshHolder.transform.localPosition.z);
				}
			}
			else if (!this.parentVessel.isSinking)
			{
				float num8 = 0f;
				if (this.percentageSurfaced > 0f)
				{
					float num9 = (200f / this.parentVessel.compartmentvolume * 1f + 0.4f) * this.percentageSurfaced;
					float xclock2 = GameDataManager.xclock;
					float num10 = num9 * GameDataManager.yclock / 600f;
					num8 = num9 * xclock2;
					this.meshHolder.transform.localPosition = new Vector3(this.meshHolder.transform.localPosition.x, this.meshHolderInitialY + num10, this.meshHolder.transform.localPosition.z);
				}
				float num11 = this.rudderAngle.y * -5f;
				float num12 = 1f - this.percentageSurfaced;
				num11 *= num12 * this.percentageSpeed;
				this.meshHolder.transform.localRotation = Quaternion.Slerp(this.meshHolder.transform.localRotation, Quaternion.Euler(0f, 0f, num11 + num8), 1f);
			}
			if (this.props.Length > 0)
			{
				this.AdjustProps(this.rudderAngle.y * 3f * this.percentageSpeed, this.movementDistance);
			}
			return;
		}
		if (this.parentVessel.databaseshipdata.shipType == "OILRIG")
		{
			return;
		}
		float num13 = 1f - (1000f - base.transform.position.y) / 0.2f;
		if (num13 > 0.02f)
		{
			float num14 = 200f / this.parentVessel.compartmentvolume * 1f + 0.4f;
			float xclock3 = GameDataManager.xclock;
			float num15 = num14 * GameDataManager.yclock / 6f;
			this.meshHolder.transform.localRotation = Quaternion.Slerp(this.meshHolder.transform.localRotation, Quaternion.Euler(num15, 0f, num14 * xclock3), 1f);
			num15 /= 100f;
			this.meshHolder.transform.localPosition = new Vector3(this.meshHolder.transform.localPosition.x, this.meshHolderInitialY + num15, this.meshHolder.transform.localPosition.z);
		}
		if (!this.parentVessel.isSinking && base.transform.position.y != 1000f)
		{
			base.transform.position = new Vector3(base.transform.position.x, 1000f, base.transform.position.z);
		}
		if (this.shipSpeed.z != 0f)
		{
			this.shipSpeed.z = 0f;
		}
	}

	// Token: 0x06000B2D RID: 2861 RVA: 0x000A34EC File Offset: 0x000A16EC
	private void CrushSubmarine()
	{
		this.parentVessel.bouyancyCompartments[0].SinkShip(this.parentVessel, false);
		this.parentVessel.gameObject.SetActive(false);
		this.parentVessel.vesselmovement.shipSpeed = Vector3.zero;
		ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.underwaterImplosions[UnityEngine.Random.Range(0, UIFunctions.globaluifunctions.database.underwaterImplosions.Length)], this.parentVessel.transform.position, Quaternion.identity);
		if (this.parentVessel.playercontrolled && UIFunctions.globaluifunctions.playerfunctions.playerSunkBy == string.Empty)
		{
			UIFunctions.globaluifunctions.playerfunctions.playerSunkBy = "IMPLOSION";
		}
	}

	// Token: 0x06000B2E RID: 2862 RVA: 0x000A35BC File Offset: 0x000A17BC
	public void TranslateShipForward()
	{
		if (this.atAnchor && !this.parentVessel.isSinking)
		{
			return;
		}
		this.movementDistance = this.shipSpeed.z * Time.deltaTime * GameDataManager.globalTranslationSpeed;
		if (this.parentVessel.databaseshipdata.shipType == "SUBMARINE")
		{
			float y = this.SubBallast(base.transform.position.y, this.timeInterval);
			base.transform.Translate(0f, y, 0f, Space.World);
		}
		base.transform.Translate(0f, 0f, this.movementDistance, Space.Self);
		if (!this.parentVessel.isSubmarine)
		{
			if (!this.parentVessel.isSinking && base.transform.position.y != 1000f)
			{
				base.transform.position = new Vector3(base.transform.position.x, 1000f, base.transform.position.z);
			}
		}
		else if (this.parentVessel.databaseshipdata.shipType == "BIOLOGIC" && base.transform.position.y > 999.99f)
		{
			base.transform.position = new Vector3(base.transform.position.x, 999.99f, base.transform.position.z);
		}
		if (this.shipSpeed.z < 0f)
		{
			base.transform.Rotate(new Vector3(0f, this.rudderAngle.y, 0f) * Time.deltaTime * this.turnSpeed * this.percentageSpeed, Space.World);
		}
		else
		{
			float num = this.shipSpeed.z / 3f;
			if (num > 1f)
			{
				num = 1f;
			}
			num *= UIFunctions.globaluifunctions.gamedatamanager.globalSpeedModifier;
			base.transform.RotateAround(base.transform.TransformPoint(new Vector3(0f, 0f, this.parentVessel.databaseshipdata.pivotpointturning)), Vector3.up, this.rudderAngle.y * num * Time.deltaTime * this.turnSpeed);
			if (!this.isSubmarine && this.rudderAngle.y != 0f && this.parentVessel.damagesystem.shipCurrentDamagePoints == 0f)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y, this.rudderAngle.y * 3f * this.percentageSpeed), 1f);
			}
		}
	}

	// Token: 0x06000B2F RID: 2863 RVA: 0x000A38FC File Offset: 0x000A1AFC
	private void SubDivePlanes(float yvalue)
	{
		if (this.parentVessel.playercontrolled && ManualCameraZoom.binoculars && this.parentVessel.uifunctions.playerfunctions.playerDepthInFeet > UIFunctions.globaluifunctions.playerfunctions.mastThresholdDepth && UIFunctions.globaluifunctions.playerfunctions.damagecontrol.CheckSubsystem("PERISCOPE", false))
		{
			this.parentVessel.uifunctions.playerfunctions.MoveScope(0);
		}
		if (this.diveAngle.y != this.diveAngle.x)
		{
			if (Mathf.Abs(this.diveAngle.y - this.diveAngle.x) < 0.05f)
			{
				this.diveAngle.y = this.diveAngle.x;
			}
			else if (this.diveAngle.y < this.diveAngle.x)
			{
				this.diveAngle.y = this.diveAngle.y + this.diveRate * this.timeInterval;
			}
			else if (this.diveAngle.y > this.diveAngle.x)
			{
				this.diveAngle.y = this.diveAngle.y - this.diveRate * this.timeInterval;
			}
		}
		float num = this.parentVessel.transform.eulerAngles.x;
		float num2 = -0.2f;
		if (num > 180f)
		{
			num -= 360f;
			num2 = 0.2f;
		}
		float num3 = this.diveAngle.y * 10f - num;
		this.planes[0].transform.localRotation = Quaternion.RotateTowards(this.planes[0].transform.localRotation, Quaternion.Euler(num3, 0f, 0f), 20f * this.timeInterval);
		this.planes[1].transform.localRotation = Quaternion.RotateTowards(this.planes[1].transform.localRotation, Quaternion.Euler(-num3, 0f, 0f), 20f * this.timeInterval);
		if (this.shipSpeed.z < 0f)
		{
			num3 = 0f;
		}
		float num4 = this.percentageSpeed * 2f;
		if (num4 > 1f)
		{
			num4 = 1f;
		}
		base.transform.Rotate(Vector3.right * this.timeInterval * num3 * 0.1f * num4);
		if (this.levelOutSubmarine)
		{
			float num5 = Mathf.Abs(num);
			if (num5 < 0.01f)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(0f, base.transform.eulerAngles.y, base.transform.eulerAngles.z), 1f);
				this.levelOutSubmarine = false;
			}
			if (num5 < 10f)
			{
				base.transform.Rotate(num2 * this.timeInterval, 0f, 0f);
			}
		}
	}

	// Token: 0x06000B30 RID: 2864 RVA: 0x000A3C3C File Offset: 0x000A1E3C
	private float SubBallast(float yvalue, float timeAmount)
	{
		if (this.ballastAngle.y != this.ballastAngle.x)
		{
			if (Mathf.Abs(this.ballastAngle.y - this.ballastAngle.x) < 0.05f)
			{
				this.ballastAngle.y = this.ballastAngle.x;
			}
			else if (this.ballastAngle.y < this.ballastAngle.x)
			{
				this.ballastAngle.y = this.ballastAngle.y + this.ballastRate * this.timeInterval;
			}
			else if (this.ballastAngle.y > this.ballastAngle.x)
			{
				this.ballastAngle.y = this.ballastAngle.y - this.ballastRate * this.timeInterval;
			}
			if (this.parentVessel.isSinking && this.ballastAngle.y > -3f)
			{
				this.ballastAngle.y = this.ballastAngle.y - this.ballastRate * this.timeInterval * 10f;
			}
		}
		float num = 0.005f;
		bool flag = false;
		float num2 = this.parentVessel.bouyancyCompartments[0].transform.position.y - this.parentVessel.databaseshipdata.waterline;
		if (num2 > 0f)
		{
			flag = true;
		}
		else
		{
			num2 = this.parentVessel.bouyancyCompartments[9].transform.position.y - this.parentVessel.databaseshipdata.waterline;
			if (num2 > 0f)
			{
				flag = true;
			}
		}
		if (flag)
		{
			float num3 = base.transform.eulerAngles.x;
			if (num3 > 180f)
			{
				num3 -= 360f;
			}
			if (num3 < 0f)
			{
				float xAngle = num2 * timeAmount * 75f;
				base.transform.Rotate(xAngle, 0f, 0f);
			}
			else
			{
				float xAngle2 = num2 * timeAmount * -75f;
				base.transform.Rotate(xAngle2, 0f, 0f);
			}
		}
		if (this.parentVessel.isSinking)
		{
			this.ballastAngle.x = -1f;
			this.ballastAngle.y = -1f;
		}
		else if (this.blowBallast)
		{
			this.ballastAngle.x = 6f;
		}
		float num4 = this.ballastAngle.y;
		if (this.parentVessel.playercontrolled)
		{
			num4 -= UIFunctions.globaluifunctions.playerfunctions.damagecontrol.GetPlayerCurrentFlooding() * 4f;
		}
		else
		{
			num4 -= this.parentVessel.damagesystem.enemyCurrentFlooding * 10f;
		}
		if (yvalue > this.parentVessel.databaseshipdata.submergedat && this.ballastAngle.y > 0f)
		{
			float num5 = 1f - (yvalue - this.parentVessel.databaseshipdata.submergedat) / (this.parentVessel.databaseshipdata.waterline - this.parentVessel.databaseshipdata.submergedat);
			num *= num5;
		}
		num2 = this.parentVessel.transform.position.y - this.parentVessel.databaseshipdata.waterline;
		float result;
		if (num2 > 0f)
		{
			base.transform.Translate(0f, -num2 * this.timeInterval * 0.5f, 0f, Space.Self);
			result = -3f * this.timeInterval * num;
		}
		else
		{
			result = num4 * this.timeInterval * num;
		}
		if (this.parentVessel.isSinking)
		{
			this.ballastRate = -3f * this.timeInterval * num;
		}
		return result;
	}

	// Token: 0x06000B31 RID: 2865 RVA: 0x000A4038 File Offset: 0x000A2238
	private bool CheckIfCavitating()
	{
		this.cavTimer += UnityEngine.Random.Range(2f, 4f);
		if (this.parentVessel.playercontrolled && UIFunctions.globaluifunctions.playerfunctions.telegraphDelayTimer > 0f)
		{
			return false;
		}
		float num = this.GetActualVesselSpeed() * 10f;
		float num2 = this.parentVessel.databaseshipdata.cavitationparameters.x * num + this.parentVessel.databaseshipdata.cavitationparameters.y;
		if (base.transform.position.y > 1000f - num2 * GameDataManager.feetToUnits)
		{
			this.isCavitating = true;
			if (!this.cavBubbles.isPlaying)
			{
				this.cavBubbles.Play();
			}
			if (!this.wasCavitating)
			{
				this.wasCavitating = true;
				if (this.parentVessel.playercontrolled)
				{
					UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "PlayerCavitating"), UIFunctions.globaluifunctions.playerfunctions.messageLogColors["Sonar"], "PlayerCavitating", false);
					UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("cavitating", true);
				}
				else if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.detectedByPlayer[this.parentVessel.vesselListIndex])
				{
					string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "EnemyCavitating");
					text = text.Replace("<CONTACT>", UIFunctions.globaluifunctions.playerfunctions.GetFullContactName(UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.mapContact[this.parentVessel.vesselListIndex].contactText.text, this.parentVessel.vesselListIndex));
					UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["Sonar"], "EnemyCavitating", false);
				}
			}
		}
		else
		{
			if (this.cavBubbles.isPlaying)
			{
				this.cavBubbles.Stop();
			}
			this.isCavitating = false;
			if (this.parentVessel.playercontrolled && this.wasCavitating)
			{
				this.wasCavitating = false;
				UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("cavitating", false);
				UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "PlayerStopCavitating"), UIFunctions.globaluifunctions.playerfunctions.messageLogColors["Sonar"], "PlayerStopCavitating", false);
			}
		}
		return false;
	}

	// Token: 0x06000B32 RID: 2866 RVA: 0x000A42DC File Offset: 0x000A24DC
	public void SetVesselUprightAfterCollision()
	{
	}

	// Token: 0x06000B33 RID: 2867 RVA: 0x000A42E0 File Offset: 0x000A24E0
	public void AdjustProps(float rudderMagnitude, float movementDistance)
	{
		float num = this.shipSpeed.z * 10f;
		float num2 = Mathf.Abs(this.engineSpeed.x - this.engineSpeed.y);
		if (num2 < 0.05f)
		{
			num2 = 0f;
			this.engineSpeed.y = this.engineSpeed.x;
		}
		if (num2 != 0f)
		{
			if (this.engineSpeed.y < this.engineSpeed.x)
			{
				this.engineSpeed.y = this.engineSpeed.y + this.timeInterval * 0.75f;
			}
			else
			{
				float num3 = 0.75f;
				if (this.telegraphValue == 0)
				{
					num3 = 1.5f;
				}
				this.engineSpeed.y = this.engineSpeed.y - this.timeInterval * num3;
			}
		}
		for (int i = 0; i < this.props.Length; i++)
		{
			this.props[i].Rotate(Vector3.forward * this.timeInterval * this.engineSpeed.y * this.parentVessel.databaseshipdata.proprotationspeed[i]);
		}
		if (!this.isSubmarine)
		{
			this.SetPropWash();
		}
		else if (this.props[0].position.y > 999.9f)
		{
			this.SetPropWash();
		}
		else if (this.propwash != null && this.propwash.isPlaying)
		{
			this.propwash.Stop();
		}
		if (this.bowwave.isPlaying && this.shipSpeed.z <= 0f)
		{
			this.bowwave.Stop();
			if (this.cavBubbles != null && !this.isCavitating)
			{
				this.cavBubbles.Stop();
			}
		}
		else if (!this.bowwave.isPlaying && this.shipSpeed.z > 0f)
		{
			if (!this.isSubmarine)
			{
				this.bowwave.Play();
				if (this.cavBubbles != null)
				{
					this.cavBubbles.Play();
				}
			}
			else if (this.percentageSurfaced > 0.5f)
			{
				this.wakeObject.transform.localPosition = Vector3.zero;
				this.bowwave.Play();
			}
		}
		this.bowwaveHeight = (-1f + this.percentageSpeed) * 0.1f;
		float num4 = 1f;
		if (this.bowwaveHeight > 0f)
		{
			this.bowwaveHeight = 0f;
		}
		if (this.bowwave.transform.localPosition.y > this.bowwaveHeight)
		{
			num4 = -1f;
		}
		this.bowwave.transform.localPosition = new Vector3(0f, this.bowwave.transform.localPosition.y + this.timeInterval * 0.02f * num4, 0f);
		float num5 = this.shipSpeed.z / 1.5f;
		num5 = Mathf.Clamp01(num5);
		if (this.foamTrails != null && this.foamTrails[0].foamTexture != null)
		{
			this.foamTrails[0].alpha = num5 * 2f;
			this.foamTrails[1].alpha = num5 * 2f;
		}
		if (this.parentVessel.isSubmarine)
		{
			if (this.percentageSurfaced < 0.05f)
			{
				if (this.foamTrails[0].foamTexture != null)
				{
					this.foamTrails[0].foamTexture = null;
					this.foamTrails[1].foamTexture = null;
				}
			}
			else if (this.foamTrails[0].foamTexture == null)
			{
				this.foamTrails[0].foamTexture = UIFunctions.globaluifunctions.database.wakeTextures[0];
				this.foamTrails[1].foamTexture = UIFunctions.globaluifunctions.database.wakeTextures[1];
			}
		}
		if (this.kelvinWaveOverlay != null)
		{
			num5 *= this.percentageSurfaced;
			if (this.parentVessel.isSubmarine)
			{
				if (this.percentageSurfaced < 0.05f)
				{
					if (this.kelvinWaveOverlay.gameObject.activeSelf)
					{
						this.kelvinWaveOverlay.gameObject.SetActive(false);
					}
					return;
				}
				if (!this.kelvinWaveOverlay.gameObject.activeSelf)
				{
					this.kelvinWaveOverlay.gameObject.SetActive(true);
				}
			}
			this.kelvinWaveOverlay.heightTexture.alpha = num5 / 20f;
			this.kelvinWaveOverlay.normalTexture.alpha = num5 * 10f;
			this.kelvinWaveOverlay.foamTexture.alpha = num5;
			this.kelvinWaveOverlay.foamTexture.maskAlpha = num5;
		}
	}

	// Token: 0x06000B34 RID: 2868 RVA: 0x000A4810 File Offset: 0x000A2A10
	private void SetPropWash()
	{
		if (this.propwash != null)
		{
			if (this.engineSpeed.y >= 1.2f)
			{
				if (!this.propwash.isPlaying)
				{
					this.propwash.Play();
				}
			}
			else if (this.propwash.isPlaying && this.engineSpeed.y < 1.2f)
			{
				this.propwash.Stop();
			}
		}
	}

	// Token: 0x06000B35 RID: 2869 RVA: 0x000A4894 File Offset: 0x000A2A94
	private void OnCollisionStay()
	{
		if (this.parentVessel.databaseshipdata.shipType == "BIOLOGIC")
		{
			this.parentVessel.GetComponent<Whale_AI>().waypoint.y = 999.99f;
			return;
		}
		if (this.parentVessel.damagesystem.screechSound == null)
		{
			UIFunctions.globaluifunctions.database.PlaceScreechAudioSource(this.parentVessel);
		}
		if (!this.parentVessel.damagesystem.screechSound.isPlaying)
		{
			this.parentVessel.damagesystem.screechSound.clip = UIFunctions.globaluifunctions.database.screeches[0];
			this.parentVessel.damagesystem.screechSound.Play();
		}
		if (!this.parentVessel.isSinking)
		{
			if (this.parentVessel.databaseshipdata.shipType != "SUBMARINE")
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f), 1f);
			}
			if (this.shipSpeed.z > 0.5f)
			{
				this.shipSpeed.z = this.shipSpeed.z - Time.deltaTime * 0.1f;
			}
		}
		if (this.parentVessel.playercontrolled && Time.timeScale > 1f)
		{
			UIFunctions.globaluifunctions.playerfunctions.SetTimeCompression();
		}
	}

	// Token: 0x040011D0 RID: 4560
	public Vessel parentVessel;

	// Token: 0x040011D1 RID: 4561
	public bool hasWeaponSource;

	// Token: 0x040011D2 RID: 4562
	public WeaponSource weaponSource;

	// Token: 0x040011D3 RID: 4563
	public Vector2 rudderAngle;

	// Token: 0x040011D4 RID: 4564
	public Vector2 diveAngle;

	// Token: 0x040011D5 RID: 4565
	public Vector2 ballastAngle;

	// Token: 0x040011D6 RID: 4566
	public MeshRenderer hullNumberRenderer;

	// Token: 0x040011D7 RID: 4567
	public MeshRenderer flagRenderer;

	// Token: 0x040011D8 RID: 4568
	public int telegraphValue;

	// Token: 0x040011D9 RID: 4569
	public Vector3 shipSpeed;

	// Token: 0x040011DA RID: 4570
	public float percentageSpeed;

	// Token: 0x040011DB RID: 4571
	public float accelerationRate;

	// Token: 0x040011DC RID: 4572
	public float decelerationRate;

	// Token: 0x040011DD RID: 4573
	public float movementDistance;

	// Token: 0x040011DE RID: 4574
	public bool isCruising;

	// Token: 0x040011DF RID: 4575
	public float cruiseSpeed;

	// Token: 0x040011E0 RID: 4576
	public Vector2 engineSpeed;

	// Token: 0x040011E1 RID: 4577
	public float engineSoundsTimer;

	// Token: 0x040011E2 RID: 4578
	public float turnSpeed;

	// Token: 0x040011E3 RID: 4579
	public float rudderSpeedModifier;

	// Token: 0x040011E4 RID: 4580
	public float cameraDistance;

	// Token: 0x040011E5 RID: 4581
	public bool isSubmarine;

	// Token: 0x040011E6 RID: 4582
	public float diveRate;

	// Token: 0x040011E7 RID: 4583
	public float surfaceRate;

	// Token: 0x040011E8 RID: 4584
	public float ballastRate;

	// Token: 0x040011E9 RID: 4585
	public float crushDepth;

	// Token: 0x040011EA RID: 4586
	public float currentCrushDepth;

	// Token: 0x040011EB RID: 4587
	public Vector2 subSinkingAngles;

	// Token: 0x040011EC RID: 4588
	public Vector2 subSinkingRates;

	// Token: 0x040011ED RID: 4589
	public bool blowBallast;

	// Token: 0x040011EE RID: 4590
	public float percentageSurfaced;

	// Token: 0x040011EF RID: 4591
	public float surfaceSpeedPenalty;

	// Token: 0x040011F0 RID: 4592
	public float currentSurfaceSpeedPenalty;

	// Token: 0x040011F1 RID: 4593
	public float penaltyRange;

	// Token: 0x040011F2 RID: 4594
	public Transform[] planes;

	// Token: 0x040011F3 RID: 4595
	public bool isCavitating;

	// Token: 0x040011F4 RID: 4596
	public bool wasCavitating;

	// Token: 0x040011F5 RID: 4597
	public float buoyancyValue;

	// Token: 0x040011F6 RID: 4598
	public float wakePosition;

	// Token: 0x040011F7 RID: 4599
	public ParticleSystem activeSubWake;

	// Token: 0x040011F8 RID: 4600
	public GameObject subWake;

	// Token: 0x040011F9 RID: 4601
	public Transform spawnedWakePosition;

	// Token: 0x040011FA RID: 4602
	public bool levelOutSubmarine;

	// Token: 0x040011FB RID: 4603
	public TrailRenderer playerTowedArray;

	// Token: 0x040011FC RID: 4604
	public GameObject wakeObject;

	// Token: 0x040011FD RID: 4605
	public Renderer[] wakeRenderer;

	// Token: 0x040011FE RID: 4606
	public AddWaveOverlay kelvinWaveOverlay;

	// Token: 0x040011FF RID: 4607
	public AddFoamTrail[] foamTrails;

	// Token: 0x04001200 RID: 4608
	public float[] submarineFoamDurations;

	// Token: 0x04001201 RID: 4609
	public ParticleSystem hullFoam;

	// Token: 0x04001202 RID: 4610
	public ParticleSystem sternFoam;

	// Token: 0x04001203 RID: 4611
	public ParticleSystem bowwave;

	// Token: 0x04001204 RID: 4612
	public ParticleSystem propwash;

	// Token: 0x04001205 RID: 4613
	public bool wakeMoving;

	// Token: 0x04001206 RID: 4614
	public int wakedirection;

	// Token: 0x04001207 RID: 4615
	public float wakeSpeed;

	// Token: 0x04001208 RID: 4616
	public float wakeuvOffset;

	// Token: 0x04001209 RID: 4617
	public Color wakeColor;

	// Token: 0x0400120A RID: 4618
	public GameObject propsCav;

	// Token: 0x0400120B RID: 4619
	public ParticleSystem cavBubbles;

	// Token: 0x0400120C RID: 4620
	public Transform[] props;

	// Token: 0x0400120D RID: 4621
	public Transform[] rudder;

	// Token: 0x0400120E RID: 4622
	public AudioSource engineSound;

	// Token: 0x0400120F RID: 4623
	public AudioSource propSound;

	// Token: 0x04001210 RID: 4624
	public AudioSource pingSound;

	// Token: 0x04001211 RID: 4625
	public AudioSource bowwaveSound;

	// Token: 0x04001212 RID: 4626
	public Vector2 enginePitchRange;

	// Token: 0x04001213 RID: 4627
	public Vector2 propPitchRange;

	// Token: 0x04001214 RID: 4628
	private float enginepitchdifference;

	// Token: 0x04001215 RID: 4629
	private float proppitchdifference;

	// Token: 0x04001216 RID: 4630
	public float bowwaveHeight;

	// Token: 0x04001217 RID: 4631
	public float wakeMultiplier;

	// Token: 0x04001218 RID: 4632
	public float propTimer;

	// Token: 0x04001219 RID: 4633
	public float meshHolderInitialY;

	// Token: 0x0400121A RID: 4634
	public float timeInterval;

	// Token: 0x0400121B RID: 4635
	public float vesselUprightRotation;

	// Token: 0x0400121C RID: 4636
	public float cavTimer;

	// Token: 0x0400121D RID: 4637
	public bool atAnchor;

	// Token: 0x0400121E RID: 4638
	public bool notMissileTarget;

	// Token: 0x0400121F RID: 4639
	public Transform meshHolder;

	// Token: 0x04001220 RID: 4640
	public Transform bowwaveHolder;
}
