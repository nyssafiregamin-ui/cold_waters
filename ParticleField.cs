using System;
using UnityEngine;

// Token: 0x0200013F RID: 319
public class ParticleField : MonoBehaviour
{
	// Token: 0x060008D7 RID: 2263 RVA: 0x000645B0 File Offset: 0x000627B0
	public void InitialiseUnderWaterParticles()
	{
		this.thisTransform = base.transform;
		this.particleDistanceSqr = this.particleDistance * this.particleDistance;
		if (this.points != null)
		{
			this.particlesystem.Clear();
		}
		this.CreateParticles();
		base.gameObject.SetActive(true);
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x00064604 File Offset: 0x00062804
	private void CreateParticles()
	{
		this.points = new ParticleSystem.Particle[this.particlesMax];
		for (int i = 0; i < this.particlesMax; i++)
		{
			this.points[i].position = this.GetNewParticlePosition();
			this.points[i].startColor = this.particleColor;
			this.points[i].startSize = this.particleSize;
		}
	}

	// Token: 0x060008D9 RID: 2265 RVA: 0x00064684 File Offset: 0x00062884
	private Vector3 GetNewParticlePosition()
	{
		Vector3 result = UnityEngine.Random.insideUnitSphere * this.particleDistance + this.thisTransform.position;
		if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.layerStrength > 0f && UnityEngine.Random.value < UIFunctions.globaluifunctions.playerfunctions.sensormanager.layerStrength)
		{
			result.y = UIFunctions.globaluifunctions.playerfunctions.sensormanager.layerDepth + UnityEngine.Random.Range(-0.1f, 0.1f);
		}
		return result;
	}

	// Token: 0x060008DA RID: 2266 RVA: 0x0006471C File Offset: 0x0006291C
	private void Update()
	{
		for (int i = 0; i < this.particlesMax; i++)
		{
			float sqrMagnitude = (this.points[i].position - this.thisTransform.position).sqrMagnitude;
			if (sqrMagnitude > this.particleDistanceSqr)
			{
				this.points[i].position = this.GetNewParticlePosition();
			}
		}
		this.particlesystem.SetParticles(this.points, this.points.Length);
	}

	// Token: 0x04000DA0 RID: 3488
	public ParticleSystem particlesystem;

	// Token: 0x04000DA1 RID: 3489
	private Transform thisTransform;

	// Token: 0x04000DA2 RID: 3490
	private ParticleSystem.Particle[] points;

	// Token: 0x04000DA3 RID: 3491
	private float particleDistanceSqr;

	// Token: 0x04000DA4 RID: 3492
	private float particleClipDistanceSqr;

	// Token: 0x04000DA5 RID: 3493
	public Color particleColor;

	// Token: 0x04000DA6 RID: 3494
	public int particlesMax;

	// Token: 0x04000DA7 RID: 3495
	public float particleSize;

	// Token: 0x04000DA8 RID: 3496
	public float particleDistance;
}
