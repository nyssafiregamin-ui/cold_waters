using System;
using UnityEngine;

// Token: 0x02000113 RID: 275
public class Enemy_AntiMissileGuns : MonoBehaviour
{
	// Token: 0x06000766 RID: 1894 RVA: 0x000429A8 File Offset: 0x00040BA8
	private void Start()
	{
		this.leadFactor = 4f;
		base.enabled = false;
		this.timer = 0f;
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x000429C8 File Offset: 0x00040BC8
	public void InitialiseEnemyMissileDefense()
	{
		this.gunRangeCollider.radius = this.parentVessel.databaseshipdata.gunRange * GameDataManager.inverseYardsScale;
		this.tracers = new ParticleSystem[this.turrets.Length];
		this.tracerPointLights = new PointLight[this.turrets.Length];
		this.tracerAudios = new AudioSource[this.turrets.Length];
		string ciwsParticle = this.parentVessel.databaseshipdata.ciwsParticle;
		for (int i = 0; i < this.barrels.Length; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate((GameObject)Resources.Load(ciwsParticle), this.barrels[i].position, this.barrels[i].rotation) as GameObject;
			gameObject.transform.SetParent(this.barrels[i]);
			gameObject.transform.localPosition = new Vector3(0f, 0.0046f, 0.0234f);
			gameObject.transform.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(90f, 0f, 0f), 1f);
			this.tracers[i] = gameObject.GetComponent<ParticleSystem>();
			this.tracerPointLights[i] = gameObject.GetComponentInChildren<PointLight>();
			this.tracerAudios[i] = gameObject.GetComponent<AudioSource>();
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(UIFunctions.globaluifunctions.database.blankTransform, base.transform.position, Quaternion.identity) as GameObject;
		gameObject2.transform.SetParent(this.parentVessel.transform, true);
		gameObject2.transform.localRotation = Quaternion.identity;
		gameObject2.transform.localPosition = Vector3.zero;
		this.directionFinder = gameObject2.transform;
		gameObject2.name = "Anti-Missile Direction Finder";
	}

	// Token: 0x06000768 RID: 1896 RVA: 0x00042B8C File Offset: 0x00040D8C
	private void OnTriggerEnter(Collider otherObject)
	{
		if (this.parentVessel.isSinking)
		{
			this.StopAllFiring();
			return;
		}
		Torpedo component = otherObject.GetComponent<Torpedo>();
		if (component != null)
		{
			if (component.databaseweapondata.landAttack)
			{
				return;
			}
			if (!component.databaseweapondata.isMissile)
			{
				return;
			}
			if (component.whichNavy == 1)
			{
				return;
			}
			Torpedo[] array = new Torpedo[this.targets.Length + 1];
			for (int i = 0; i < this.targets.Length; i++)
			{
				array[i] = this.targets[i];
			}
			array[array.Length - 1] = component;
			this.targets = array;
			base.enabled = true;
			if (this.parentVessel.databaseshipdata.numberChafflaunched > 0)
			{
				for (int j = 0; j < this.parentVessel.databaseshipdata.numberChafflaunched; j++)
				{
					this.DeployChaff(component);
				}
			}
			UIFunctions.globaluifunctions.playerfunctions.eventcamera.CheckForEventCamera(component.transform, base.transform, 10f, true, false, false, 5f, -1f, -1f, true);
		}
	}

	// Token: 0x06000769 RID: 1897 RVA: 0x00042CB4 File Offset: 0x00040EB4
	private void DeployChaff(Torpedo incomingMissile)
	{
		GameObject gameObject = ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.databasecountermeasuredata[this.parentVessel.databaseshipdata.chaffID].countermeasureObject, base.transform.position, base.transform.rotation);
		gameObject.SetActive(true);
		Noisemaker component = gameObject.GetComponent<Noisemaker>();
		component.databasecountermeasuredata = UIFunctions.globaluifunctions.database.databasecountermeasuredata[this.parentVessel.databaseshipdata.chaffID];
		gameObject.transform.LookAt(incomingMissile.transform.position);
		gameObject.transform.Rotate(Vector3.up * UnityEngine.Random.Range(-10f, 10f));
		gameObject.transform.Translate(Vector3.forward * UnityEngine.Random.Range(2f, 4f));
		gameObject.transform.Translate(Vector3.up * UnityEngine.Random.Range(0.25f, 0.75f));
		float num = this.parentVessel.databaseshipdata.chaffProbability;
		Vector3 from = incomingMissile.transform.position - base.transform.position;
		float num2 = Vector3.Angle(from, base.transform.forward);
		if (num2 < 20f || num2 > 160f)
		{
			num *= 2f;
		}
		else if (num2 < 45f || num2 > 135f)
		{
			num *= 1.5f;
		}
		if (UnityEngine.Random.value < num && !incomingMissile.databaseweapondata.landAttack)
		{
			incomingMissile.chaffed = true;
			incomingMissile.targetTransform = gameObject.transform;
		}
	}

	// Token: 0x0600076A RID: 1898 RVA: 0x00042E6C File Offset: 0x0004106C
	public float GetBearingToTarget(Vector3 targetPosition)
	{
		this.directionFinder.LookAt(targetPosition);
		float num = this.directionFinder.localEulerAngles.y;
		if (num > 180f)
		{
			num -= 360f;
		}
		return num;
	}

	// Token: 0x0600076B RID: 1899 RVA: 0x00042EB0 File Offset: 0x000410B0
	private void FixedUpdate()
	{
		for (int i = 0; i < this.targets.Length; i++)
		{
			if (this.targets[i] == null)
			{
				if (this.targets.Length == 1)
				{
					this.targets = new Torpedo[0];
					this.StopAllFiring();
				}
				else
				{
					Torpedo[] array = new Torpedo[this.targets.Length - 1];
					int num = 0;
					for (int j = 0; j < this.targets.Length; j++)
					{
						if (j != i)
						{
							array[num] = this.targets[j];
							num++;
						}
					}
					this.targets = array;
				}
				return;
			}
			float num2 = Vector3.Distance(base.transform.position, this.targets[i].transform.position);
			float d = num2 / this.leadFactor;
			Vector3 targetPosition = this.targets[i].transform.position + this.targets[i].transform.forward * this.targets[i].actualCurrentSpeed * GameDataManager.globalTranslationSpeed * d;
			this.timer += Time.deltaTime;
			float bearingToTarget = this.GetBearingToTarget(targetPosition);
			for (int k = 0; k < this.turrets.Length; k++)
			{
				if (this.parentVessel.databaseshipdata.gunRestAngle[k] == 180f)
				{
					if (bearingToTarget < this.parentVessel.databaseshipdata.gunFiringArcFinish[k] || bearingToTarget > this.parentVessel.databaseshipdata.gunFiringArcStart[k])
					{
						this.FireGun(k, targetPosition);
						if (this.timer > 0.2f && UnityEngine.Random.value < this.parentVessel.databaseshipdata.gunProbability)
						{
							this.targets[i].ShootMissileDown(true);
						}
					}
					else if (this.tracers[k].isPlaying)
					{
						this.StopGun(k);
					}
				}
				else if (bearingToTarget < this.parentVessel.databaseshipdata.gunFiringArcFinish[k] && bearingToTarget > this.parentVessel.databaseshipdata.gunFiringArcStart[k])
				{
					this.FireGun(k, targetPosition);
					if (this.timer > 0.2f && UnityEngine.Random.value < this.parentVessel.databaseshipdata.gunProbability)
					{
						this.targets[i].ShootMissileDown(true);
					}
				}
				else if (this.tracers[k].isPlaying)
				{
					this.StopGun(k);
				}
			}
			if (num2 > this.gunRangeCollider.radius * 1.5f)
			{
				this.targets[i] = null;
			}
		}
		if (this.timer > 0.2f)
		{
			this.timer = 0f;
		}
	}

	// Token: 0x0600076C RID: 1900 RVA: 0x00043198 File Offset: 0x00041398
	private void FireGun(int i, Vector3 targetPosition)
	{
		this.directionFinders[i].transform.LookAt(targetPosition);
		this.turrets[i].transform.rotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(0f, this.directionFinders[i].eulerAngles.y, 0f), 1f);
		if (this.trackingRadars.Length != 0)
		{
			this.trackingRadars[this.parentVessel.databaseshipdata.gunUsesRadar[i]].transform.rotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(0f, this.directionFinders[i].eulerAngles.y, 0f), 1f);
		}
		this.barrels[i].transform.rotation = Quaternion.Slerp(Quaternion.identity, this.directionFinders[i].rotation, 1f);
		if (!this.tracers[i].isPlaying)
		{
			this.tracers[i].Play();
			this.tracerAudios[i].Play();
		}
	}

	// Token: 0x0600076D RID: 1901 RVA: 0x000432BC File Offset: 0x000414BC
	private void StopGun(int i)
	{
		if (this.tracers[i].isPlaying)
		{
			this.tracers[i].Stop();
			this.tracerAudios[i].Stop();
			this.turrets[i].transform.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(0f, this.parentVessel.databaseshipdata.gunRestAngle[i], 0f), 1f);
			if (this.trackingRadars.Length != 0)
			{
				this.trackingRadars[this.parentVessel.databaseshipdata.gunUsesRadar[i]].transform.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(0f, this.parentVessel.databaseshipdata.gunRadarRestAngles[this.parentVessel.databaseshipdata.gunUsesRadar[i]], 0f), 1f);
			}
		}
	}

	// Token: 0x0600076E RID: 1902 RVA: 0x000433A8 File Offset: 0x000415A8
	public void StopAllFiring()
	{
		for (int i = 0; i < this.turrets.Length; i++)
		{
			this.StopGun(i);
		}
		base.enabled = false;
	}

	// Token: 0x04000A20 RID: 2592
	public Vessel parentVessel;

	// Token: 0x04000A21 RID: 2593
	public GameObject[] turrets;

	// Token: 0x04000A22 RID: 2594
	public GameObject[] trackingRadars;

	// Token: 0x04000A23 RID: 2595
	public Transform[] barrels;

	// Token: 0x04000A24 RID: 2596
	public ParticleSystem[] tracers;

	// Token: 0x04000A25 RID: 2597
	public PointLight[] tracerPointLights;

	// Token: 0x04000A26 RID: 2598
	public AudioSource[] tracerAudios;

	// Token: 0x04000A27 RID: 2599
	public Transform directionFinder;

	// Token: 0x04000A28 RID: 2600
	public Transform[] directionFinders;

	// Token: 0x04000A29 RID: 2601
	public bool firing;

	// Token: 0x04000A2A RID: 2602
	public Torpedo[] targets;

	// Token: 0x04000A2B RID: 2603
	public float timer;

	// Token: 0x04000A2C RID: 2604
	public SphereCollider gunRangeCollider;

	// Token: 0x04000A2D RID: 2605
	public float leadFactor;
}
