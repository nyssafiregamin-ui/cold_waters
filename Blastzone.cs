using System;
using UnityEngine;

// Token: 0x020000EE RID: 238
public class Blastzone : MonoBehaviour
{
	// Token: 0x06000650 RID: 1616 RVA: 0x0002D1CC File Offset: 0x0002B3CC
	public void InitialiseBlastZone()
	{
		this.collisionvolume.radius = 0.01f;
		this.superstructurehit = false;
		this.bouyancyhit = false;
		this.damagedealt = false;
		base.enabled = false;
		this.collisionvolume.enabled = false;
		this.warhead = UIFunctions.globaluifunctions.database.databasedepthchargedata[this.parentDepthCharge.depthChargeID].warhead;
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x0002D238 File Offset: 0x0002B438
	private void FixedUpdate()
	{
		this.collisionvolume.radius += 0.7f * Time.deltaTime;
		if (this.collisionvolume.radius > this.warhead / 150f)
		{
			this.collisionvolume.radius = 0.01f;
			this.parentDepthCharge.enabled = true;
			ObjectPoolManager.DestroyPooled(this.parentDepthCharge.gameObject);
		}
	}

	// Token: 0x040006BA RID: 1722
	public SphereCollider collisionvolume;

	// Token: 0x040006BB RID: 1723
	public bool superstructurehit;

	// Token: 0x040006BC RID: 1724
	public bool bouyancyhit;

	// Token: 0x040006BD RID: 1725
	public bool damagedealt;

	// Token: 0x040006BE RID: 1726
	public Projectile_DepthCharge parentDepthCharge;

	// Token: 0x040006BF RID: 1727
	public float warhead;
}
