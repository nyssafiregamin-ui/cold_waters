using System;
using UnityEngine;

// Token: 0x0200013A RID: 314
public class Noisemaker : MonoBehaviour
{
	// Token: 0x060008A3 RID: 2211 RVA: 0x000627B8 File Offset: 0x000609B8
	private void Start()
	{
		if (this.databasecountermeasuredata.isNoisemaker || this.databasecountermeasuredata.isKnuckle)
		{
			this.spherecollider.enabled = true;
			this.spherecollider.radius = this.databasecountermeasuredata.noiseStrength / 150f;
		}
		else
		{
			this.spherecollider.enabled = false;
		}
		this.timer = 0f;
		if (this.databasecountermeasuredata.countermeasureParticle != null && !this.particlesInitialised)
		{
			this.particlesInitialised = true;
			GameObject gameObject = UnityEngine.Object.Instantiate(this.databasecountermeasuredata.countermeasureParticle, this.cmParticleTransform.position, this.cmParticleTransform.transform.rotation) as GameObject;
			ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
			gameObject.transform.SetParent(this.cmParticleTransform, true);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
		}
	}

	// Token: 0x060008A4 RID: 2212 RVA: 0x000628BC File Offset: 0x00060ABC
	private void FixedUpdate()
	{
		this.timer += Time.deltaTime;
		if (this.timer > this.databasecountermeasuredata.lifetime)
		{
			this.DestroyCountermeasure();
		}
		if (this.databasecountermeasuredata.isNoisemaker)
		{
			if (this.currentSpeed < this.databasecountermeasuredata.fallSpeed)
			{
				this.currentSpeed += Time.deltaTime * 0.2f;
			}
			base.transform.Translate(Vector3.up * Time.deltaTime * -this.currentSpeed);
		}
	}

	// Token: 0x060008A5 RID: 2213 RVA: 0x0006295C File Offset: 0x00060B5C
	public void DestroyCountermeasure()
	{
		if (this.playerGenerated)
		{
			SensorManager.playerKnuckle = false;
		}
		if (this.databasecountermeasuredata != null && !this.databasecountermeasuredata.isChaff)
		{
			if (this.tacMapNoisemakerIcon != null)
			{
				this.tacMapNoisemakerIcon.transform.SetParent(base.transform, false);
			}
			if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.noisemakerObjects.Length > 0)
			{
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.RemoveNoiseMakerFromArray(this.countermeasureArrayID);
			}
		}
		if (this.tacMapNoisemakerIcon != null)
		{
			this.tacMapNoisemakerIcon.transform.SetParent(base.transform, false);
		}
		ObjectPoolManager.DestroyPooled(base.gameObject);
	}

	// Token: 0x04000D5F RID: 3423
	public DatabaseCountermeasureData databasecountermeasuredata;

	// Token: 0x04000D60 RID: 3424
	public int countermeasureArrayID;

	// Token: 0x04000D61 RID: 3425
	public bool playerGenerated;

	// Token: 0x04000D62 RID: 3426
	public float currentSpeed;

	// Token: 0x04000D63 RID: 3427
	public float timer;

	// Token: 0x04000D64 RID: 3428
	public SphereCollider spherecollider;

	// Token: 0x04000D65 RID: 3429
	public MapContact tacMapNoisemakerIcon;

	// Token: 0x04000D66 RID: 3430
	public GameObject cmMesh;

	// Token: 0x04000D67 RID: 3431
	public Transform cmParticleTransform;

	// Token: 0x04000D68 RID: 3432
	public bool particlesInitialised;

	// Token: 0x04000D69 RID: 3433
	public int sonarIDofSonobuoy;
}
