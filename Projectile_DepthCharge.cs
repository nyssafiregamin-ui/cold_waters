using System;
using UnityEngine;

// Token: 0x0200014E RID: 334
public class Projectile_DepthCharge : MonoBehaviour
{
	// Token: 0x060009AF RID: 2479 RVA: 0x0006FB90 File Offset: 0x0006DD90
	public void Start()
	{
		if (this.isProximityMine)
		{
			base.enabled = false;
			this.blastzone.enabled = false;
			base.gameObject.SetActive(true);
			return;
		}
		if (!this.particlesInitialised && !this.isNavalShell)
		{
			this.InitialiseParticles();
		}
		this.blastzone.InitialiseBlastZone();
		this.hasSplashed = false;
		this.dcrigidbody.velocity = Vector3.zero;
		this.dcrigidbody.angularVelocity = Vector3.zero;
		if (!this.isNavalShell)
		{
			this.dcBubbles.Clear();
			this.dcBubbles.Play();
			this.dcmesh.SetActive(true);
			if (this.isDepthExploded)
			{
				this.dcExplodeDepth = GameDataManager.playervesselsonlevel[0].transform.position.y;
				this.dcExplodeDepth += UnityEngine.Random.Range(-0.2f, 0.2f);
				if (this.dcExplodeDepth > 1000f)
				{
					this.dcExplodeDepth = UnityEngine.Random.Range(999.9f, 1000f);
				}
			}
			if (this.velocity > 0f)
			{
				this.dcrigidbody.velocity = base.transform.forward * this.velocity;
			}
		}
		else
		{
			this.dcmesh.SetActive(false);
			this.dcExplodeDepth = 1000f;
			this.isContactExploded = true;
			if (this.velocity > 0f)
			{
				this.yDown = -0.1427f;
				this.dcrigidbody.velocity = base.transform.forward * this.velocity;
			}
		}
		this.alreadyHit = 0;
		if (UIFunctions.globaluifunctions.playerfunctions.timeCompressionToggle == 1)
		{
			UIFunctions.globaluifunctions.playerfunctions.SetTimeCompression();
		}
	}

	// Token: 0x060009B0 RID: 2480 RVA: 0x0006FD6C File Offset: 0x0006DF6C
	private void OnTriggerEnter(Collider otherObject)
	{
		if (otherObject.name == "Terrain Chunk")
		{
			if (this.isProximityMine)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else
			{
				this.ExplodeDepthCharge();
			}
		}
	}

	// Token: 0x060009B1 RID: 2481 RVA: 0x0006FDB0 File Offset: 0x0006DFB0
	public void InitialiseParticles()
	{
		if (base.name.Contains("proximity"))
		{
			return;
		}
		this.particlesInitialised = true;
		DatabaseDepthChargeData databaseDepthChargeData = UIFunctions.globaluifunctions.database.databasedepthchargedata[this.depthChargeID];
		GameObject gameObject = UnityEngine.Object.Instantiate(databaseDepthChargeData.bubbles, this.bubblesTransform.position, this.bubblesTransform.transform.rotation) as GameObject;
		ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
		this.dcBubbles = component;
		gameObject.transform.SetParent(this.bubblesTransform, true);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		if (databaseDepthChargeData.launchFlare != null)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(databaseDepthChargeData.launchFlare, this.bubblesTransform.position, this.bubblesTransform.transform.rotation) as GameObject;
			ParticleSystem component2 = gameObject.GetComponent<ParticleSystem>();
			gameObject2.transform.SetParent(this.bubblesTransform, true);
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localRotation = Quaternion.identity;
		}
	}

	// Token: 0x060009B2 RID: 2482 RVA: 0x0006FED4 File Offset: 0x0006E0D4
	public void FixedUpdate()
	{
		if (GameDataManager.playervesselsonlevel[0] == null)
		{
			this.DestroyMe();
		}
		float y = base.transform.position.y;
		if (!this.hasSplashed)
		{
			Quaternion rotation = base.transform.rotation;
			if (y > 1000f && this.dcrigidbody.velocity != Vector3.zero)
			{
				rotation.SetLookRotation(this.dcrigidbody.velocity);
				base.transform.rotation = rotation;
			}
			this.dcrigidbody.AddForce(0f, this.yDown, 0f);
			if (y < 1000f)
			{
				this.hasSplashed = true;
				this.dcrigidbody.velocity = Vector3.zero;
				this.dcrigidbody.angularVelocity = Vector3.zero;
				int num = 0;
				this.blastzone.collisionvolume.enabled = true;
				if (this.isNavalShell)
				{
					num = 1;
					this.blastzone.enabled = true;
					this.dcrigidbody.velocity = Vector3.zero;
					this.dcrigidbody.angularVelocity = Vector3.zero;
					base.transform.position = new Vector3(base.transform.position.x, 1000f, base.transform.position.z);
				}
				ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.splashes[num], new Vector3(base.transform.position.x, 1000f, base.transform.position.z), Quaternion.identity);
			}
		}
		else
		{
			if (base.transform.rotation.x < 90f)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(90f, 0f, 0f), 5f * Time.deltaTime);
			}
			else
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(0f, 0f, 0f), 1f);
			}
			base.transform.Translate(Vector3.forward * this.sinkSpeed * Time.deltaTime);
		}
		if (this.isContactExploded && y < GameDataManager.playervesselsonlevel[0].transform.position.y - 1f)
		{
			base.enabled = true;
			this.DestroyMe();
		}
		if (this.isDepthExploded && y <= this.dcExplodeDepth)
		{
			this.ExplodeDepthCharge();
		}
	}

	// Token: 0x060009B3 RID: 2483 RVA: 0x000701A4 File Offset: 0x0006E3A4
	public void ExplodeDepthCharge()
	{
		this.blastzone.enabled = true;
		this.dcmesh.SetActive(false);
		if (this.dcBubbles != null)
		{
			this.dcBubbles.Stop();
		}
		ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.underwaterExplosions[UnityEngine.Random.Range(0, UIFunctions.globaluifunctions.database.underwaterExplosions.Length)], base.transform.position, Quaternion.identity);
		base.enabled = false;
	}

	// Token: 0x060009B4 RID: 2484 RVA: 0x0007022C File Offset: 0x0006E42C
	private void DestroyMe()
	{
		if (this.amPooled)
		{
			ObjectPoolManager.DestroyPooled(base.gameObject);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x04000E80 RID: 3712
	public int depthChargeID;

	// Token: 0x04000E81 RID: 3713
	public Blastzone blastzone;

	// Token: 0x04000E82 RID: 3714
	public Rigidbody dcrigidbody;

	// Token: 0x04000E83 RID: 3715
	public GameObject dcmesh;

	// Token: 0x04000E84 RID: 3716
	public ParticleSystem dcBubbles;

	// Token: 0x04000E85 RID: 3717
	public int alreadyHit;

	// Token: 0x04000E86 RID: 3718
	public float sinkSpeed;

	// Token: 0x04000E87 RID: 3719
	public float velocity;

	// Token: 0x04000E88 RID: 3720
	public bool isDepthExploded;

	// Token: 0x04000E89 RID: 3721
	public bool isContactExploded;

	// Token: 0x04000E8A RID: 3722
	public bool particlesInitialised;

	// Token: 0x04000E8B RID: 3723
	public float dcExplodeDepth;

	// Token: 0x04000E8C RID: 3724
	public bool hasSplashed;

	// Token: 0x04000E8D RID: 3725
	public float yDown;

	// Token: 0x04000E8E RID: 3726
	public Transform bubblesTransform;

	// Token: 0x04000E8F RID: 3727
	public bool isNavalShell;

	// Token: 0x04000E90 RID: 3728
	public bool isProximityMine;

	// Token: 0x04000E91 RID: 3729
	public bool amPooled = true;
}
