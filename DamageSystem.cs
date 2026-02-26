using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000102 RID: 258
public class DamageSystem : MonoBehaviour
{
	// Token: 0x06000719 RID: 1817 RVA: 0x0003EE80 File Offset: 0x0003D080
	public void DamageInit()
	{
		this.maxTelegraph = 6;
		this.burningTimer = UnityEngine.Random.Range(120f, 300f);
		this.shipTotalDamagePoints = this.parentVessel.databaseshipdata.displacement;
		if (this.shipTotalDamagePoints <= 500f)
		{
			this.shipTotalDamagePoints /= 20f;
		}
		else if (this.shipTotalDamagePoints <= 5000f)
		{
			this.shipTotalDamagePoints /= 30f;
			this.shipTotalDamagePoints += 9f;
		}
		else if (this.shipTotalDamagePoints <= 12000f)
		{
			this.shipTotalDamagePoints /= 50f;
			this.shipTotalDamagePoints += 76f;
		}
		else
		{
			this.shipTotalDamagePoints /= 60f;
			this.shipTotalDamagePoints += 116f;
		}
		if (this.parentVessel.databaseshipdata.shipType == "SUBMARINE")
		{
			this.shipTotalDamagePoints *= 0.5f;
		}
		if (this.parentVessel.databaseshipdata.shipType == "MERCHANT")
		{
			this.shipTotalDamagePoints *= 0.5f;
		}
		if (!this.parentVessel.playercontrolled)
		{
			this.shipTotalDamagePoints *= 0.9f;
		}
		if (this.parentVessel.playercontrolled)
		{
			this.shipTotalDamagePoints *= OptionsManager.difficultySettings["PlayerHullPoints"];
		}
		else
		{
			this.shipTotalDamagePoints *= OptionsManager.difficultySettings["EnemyHullPoints"];
		}
		this.fires = new ParticleSystem[3];
	}

	// Token: 0x0600071A RID: 1818 RVA: 0x0003F05C File Offset: 0x0003D25C
	private void FixedUpdate()
	{
		if (this.isBurning && !this.parentVessel.isSinking)
		{
			this.burningTimer -= Time.deltaTime;
			if (this.burningTimer < 0f)
			{
				this.CheckBurning();
				this.burningTimer = UnityEngine.Random.Range(120f, 300f);
			}
		}
		if (this.parentVessel.databaseshipdata.shipType == "SUBMARINE" && this.enemyCurrentFlooding < this.currentFlooding * 2f)
		{
			this.enemyCurrentFlooding += 0.05f * Time.deltaTime;
		}
	}

	// Token: 0x0600071B RID: 1819 RVA: 0x0003F110 File Offset: 0x0003D310
	private void KillShipByDOT()
	{
		bool explosion = false;
		if (UnityEngine.Random.value < 0.5f)
		{
			explosion = true;
		}
		this.parentVessel.bouyancyCompartments[0].SinkShip(this.parentVessel, explosion);
		if (this.parentVessel.playercontrolled && UIFunctions.globaluifunctions.playerfunctions.playerSunkBy == string.Empty)
		{
			UIFunctions.globaluifunctions.playerfunctions.playerSunkBy = "DAMAGEOVERTIME";
		}
	}

	// Token: 0x0600071C RID: 1820 RVA: 0x0003F18C File Offset: 0x0003D38C
	public void CheckMaxTelegraphValue()
	{
		float num = this.shipCurrentDamagePoints / this.shipTotalDamagePoints;
		int num2 = 6;
		if (num >= 0.9f)
		{
			num2 = 1;
		}
		else if (num >= 0.75f)
		{
			num2 = 3;
		}
		else if (num >= 0.5f)
		{
			num2 = 4;
		}
		else if (num >= 0.25f)
		{
			num2 = 5;
		}
		if (this.maxTelegraph > num2)
		{
			this.maxTelegraph = num2;
		}
		if (this.parentVessel.vesselmovement.telegraphValue > num2)
		{
			UIFunctions.globaluifunctions.combatai.AIAdjustTelegraph(this.parentVessel, num2);
		}
		if (this.parentVessel.damagesystem.maxTelegraph == 1 && this.parentVessel.databaseshipdata.shipType != "SUBMARINE" && UnityEngine.Random.value < 0.5f)
		{
			this.parentVessel.vesselai.enabled = false;
		}
	}

	// Token: 0x0600071D RID: 1821 RVA: 0x0003F280 File Offset: 0x0003D480
	private void CheckBurning()
	{
		int num = this.VesselNumberBurning();
		if (num == 0)
		{
			this.isBurning = false;
			return;
		}
		if (num == this.fires.Length && UnityEngine.Random.value <= 0.1f)
		{
			this.KillShipByDOT();
			return;
		}
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = num;
		List<int> list = new List<int>();
		for (int i = 0; i < this.fires.Length; i++)
		{
			if (this.fires[i] != null && this.fires[i].isPlaying)
			{
				list.Add(i);
			}
		}
		for (int j = 0; j < num; j++)
		{
			int num6 = UnityEngine.Random.Range(1, 11) + num5;
			if (num6 < 5)
			{
				num2++;
			}
			else if (num6 >= 9)
			{
				num3++;
				if (UnityEngine.Random.value < 0.25f)
				{
					num4++;
				}
			}
		}
		if (num2 > 0)
		{
			num3 = 0;
			num4 = 0;
			if (num == 1)
			{
				for (int k = 0; k < this.fires.Length; k++)
				{
					if (this.fires[k] != null && this.fires[k].isPlaying)
					{
						this.fires[k].Stop();
						this.fireSound.Stop();
					}
				}
			}
			else
			{
				int num7 = list[UnityEngine.Random.Range(0, list.Count)];
				if (this.fires[num7].isPlaying)
				{
					this.fires[num7].Stop();
				}
			}
		}
		int num8 = 1;
		if (num3 > 0)
		{
			if (num == 3)
			{
				num4++;
			}
			else
			{
				List<int> list2 = new List<int>();
				for (int l = 0; l < this.fires.Length; l++)
				{
					bool flag = false;
					bool flag2 = false;
					if (this.fires[l] == null)
					{
						flag2 = true;
					}
					else if (!this.fires[l].isPlaying)
					{
						flag2 = true;
					}
					if (l == 0 || l == 2)
					{
						if (this.fires[1] != null && this.fires[1].isPlaying)
						{
							flag = true;
						}
					}
					else if (this.fires[0] != null)
					{
						if (this.fires[0].isPlaying)
						{
							flag = true;
						}
					}
					else if (this.fires[2] != null && this.fires[2].isPlaying)
					{
						flag = true;
					}
					if (flag2 && flag && !list2.Contains(l))
					{
						list2.Add(l);
					}
				}
				if (list2.Count < 3)
				{
					num8 = list2[UnityEngine.Random.Range(0, list2.Count)];
					if (this.fires[num8] == null)
					{
						this.SetFire(num8);
					}
					else
					{
						this.fires[num8].Play();
					}
				}
			}
		}
		if (num4 > 0)
		{
			int num6 = UnityEngine.Random.Range(1, 11);
			if (num6 < 6)
			{
				this.shipCurrentDamagePoints += this.shipTotalDamagePoints * 0.02f;
			}
			else if (num6 < 9)
			{
				this.shipCurrentDamagePoints += this.shipTotalDamagePoints * 0.04f;
				num4++;
			}
			else
			{
				this.shipCurrentDamagePoints += this.shipTotalDamagePoints * 0.06f;
				num4++;
			}
			if (this.parentVessel.damagesystem.fires[num8] != null)
			{
				UIFunctions.globaluifunctions.database.CreateExplosion(this.parentVessel, num4, this.parentVessel.damagesystem.fires[num8].transform.position);
			}
		}
		if (num - num2 > 0)
		{
			for (int m = 0; m < num - num2; m++)
			{
				int num6 = UnityEngine.Random.Range(1, 11);
				if (num6 < 6)
				{
					this.shipCurrentDamagePoints += this.shipTotalDamagePoints * 0.01f;
				}
				else if (num6 < 9)
				{
					this.shipCurrentDamagePoints += this.shipTotalDamagePoints * 0.02f;
					num4++;
				}
				else
				{
					this.shipCurrentDamagePoints += this.shipTotalDamagePoints * 0.03f;
					num4++;
				}
			}
		}
		if (this.shipCurrentDamagePoints > this.shipTotalDamagePoints)
		{
			this.KillShipByDOT();
		}
		this.CheckMaxTelegraphValue();
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x0003F734 File Offset: 0x0003D934
	public void SetMeshes()
	{
		for (int i = 0; i < this.objectMeshesToHide.Length; i++)
		{
			this.objectMeshesToHide[i].SetActive(false);
		}
		for (int j = 0; j < this.damageMeshFilters.Length; j++)
		{
			this.damageMeshFilters[j].mesh = this.damageMeshes[j];
		}
		this.destroyedMesh = true;
	}

	// Token: 0x0600071F RID: 1823 RVA: 0x0003F7A0 File Offset: 0x0003D9A0
	public void SetFire(int fireNumber)
	{
		if (!this.parentVessel.isSubmarine)
		{
			if (this.fires[fireNumber] == null)
			{
				ParticleSystem particleSystem = UnityEngine.Object.Instantiate(this.parentVessel.uifunctions.database.shipFires[0], base.transform.position, Quaternion.identity) as ParticleSystem;
				particleSystem.transform.parent = this.parentVessel.damagesystem.transform;
				float z = 0f;
				if (fireNumber == 0)
				{
					z = this.parentVessel.bouyancyCompartments[2].transform.localPosition.z;
				}
				else if (fireNumber == 2)
				{
					z = this.parentVessel.bouyancyCompartments[6].transform.localPosition.z;
				}
				particleSystem.transform.localPosition = new Vector3(0f, this.parentVessel.databaseshipdata.hullHeight / 2f + 0.2f, z);
				this.parentVessel.damagesystem.fires[fireNumber] = particleSystem;
			}
			if (!this.fireSound.isPlaying)
			{
				this.fireSound.Play();
			}
			if (!this.fires[fireNumber].isPlaying)
			{
				this.fires[fireNumber].Play();
			}
		}
	}

	// Token: 0x06000720 RID: 1824 RVA: 0x0003F8F4 File Offset: 0x0003DAF4
	public void CheckFireHeight()
	{
		for (int i = 0; i < this.fires.Length; i++)
		{
			if (this.fires[i] != null && this.fires[i].isPlaying)
			{
				this.fires[i].transform.rotation = Quaternion.identity;
				if (this.fires[i].transform.position.y < 999.9f)
				{
					this.fires[i].Stop();
					if (this.VesselNumberBurning() == 0)
					{
						this.fireSound.Stop();
					}
				}
			}
		}
	}

	// Token: 0x06000721 RID: 1825 RVA: 0x0003F9A0 File Offset: 0x0003DBA0
	private int VesselNumberBurning()
	{
		int num = 0;
		for (int i = 0; i < this.fires.Length; i++)
		{
			if (this.fires[i] != null && this.fires[i].isPlaying)
			{
				num++;
			}
		}
		if (num > 0)
		{
			this.isBurning = true;
		}
		return num;
	}

	// Token: 0x0400087F RID: 2175
	public Vessel parentVessel;

	// Token: 0x04000880 RID: 2176
	public global::LOD lodsystem;

	// Token: 0x04000881 RID: 2177
	public ParticleSystem funnelSmoke;

	// Token: 0x04000882 RID: 2178
	public ParticleSystem emergencyBlow;

	// Token: 0x04000883 RID: 2179
	public Radar[] radars;

	// Token: 0x04000884 RID: 2180
	public ParticleSystem[] fires;

	// Token: 0x04000885 RID: 2181
	public bool noFireOut;

	// Token: 0x04000886 RID: 2182
	public AudioSource fireSound;

	// Token: 0x04000887 RID: 2183
	public AudioSource screechSound;

	// Token: 0x04000888 RID: 2184
	public bool usesDamageMeshes;

	// Token: 0x04000889 RID: 2185
	public bool destroyedMesh;

	// Token: 0x0400088A RID: 2186
	public Mesh[] damageMeshes;

	// Token: 0x0400088B RID: 2187
	public MeshFilter[] damageMeshFilters;

	// Token: 0x0400088C RID: 2188
	public GameObject[] objectMeshesToHide;

	// Token: 0x0400088D RID: 2189
	public Mesh[] hullDamageMeshes;

	// Token: 0x0400088E RID: 2190
	public ParticleSystem sinkingBubbles;

	// Token: 0x0400088F RID: 2191
	public MeshRenderer[] torpedoHits;

	// Token: 0x04000890 RID: 2192
	public float shipTotalDamagePoints;

	// Token: 0x04000891 RID: 2193
	public float shipCurrentDamagePoints;

	// Token: 0x04000892 RID: 2194
	public string cargoType;

	// Token: 0x04000893 RID: 2195
	public float currentFlooding;

	// Token: 0x04000894 RID: 2196
	public float enemyCurrentFlooding;

	// Token: 0x04000895 RID: 2197
	public float currentBurning;

	// Token: 0x04000896 RID: 2198
	public float burningTimer;

	// Token: 0x04000897 RID: 2199
	public bool isBurning;

	// Token: 0x04000898 RID: 2200
	public int maxTelegraph;

	// Token: 0x04000899 RID: 2201
	public bool vesselDamagedByPlayer;
}
