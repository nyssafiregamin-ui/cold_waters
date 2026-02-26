using System;
using UnityEngine;

// Token: 0x02000160 RID: 352
public class SpriteVerticle : MonoBehaviour
{
	// Token: 0x06000A25 RID: 2597 RVA: 0x0007B938 File Offset: 0x00079B38
	private void Start()
	{
		this.a = 1f;
		this.doonce = false;
		this.randomx = 0f;
		if ((double)UnityEngine.Random.value < 0.5)
		{
			this.randomx *= -1f;
		}
		this.maxheight2 = this.maxheight;
		this.maxheight2 += UnityEngine.Random.value / 100f;
	}

	// Token: 0x06000A26 RID: 2598 RVA: 0x0007B9AC File Offset: 0x00079BAC
	private void FixedUpdate()
	{
		Vector3 position = base.transform.position;
		float num = 0.05f - this.a / 1000f;
		if (position.y < this.maxheight2 - 0.1f && !this.doonce)
		{
			base.transform.position = new Vector3(position.x, position.y + num, position.z);
		}
		else if (position.y < this.maxheight2 && this.a < 32f)
		{
			base.transform.position = new Vector3(position.x, position.y + num / (0.125f * this.a * this.a), position.z);
		}
		else
		{
			if (this.randomx < 0f)
			{
				this.randomx *= -1f;
			}
			base.transform.position = new Vector3(position.x, position.y - 0.0018f - this.randomx, position.z);
			if (!this.doonce)
			{
				this.doonce = true;
			}
		}
		this.a += 1f;
	}

	// Token: 0x06000A27 RID: 2599 RVA: 0x0007BB04 File Offset: 0x00079D04
	private void LateUpdate()
	{
		if (GameDataManager.HUDActive && this.camerafollow)
		{
			base.transform.LookAt(base.transform.position + Camera.main.transform.rotation * Vector3.back, Camera.main.transform.rotation * Vector3.up);
			base.transform.rotation = Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f);
		}
	}

	// Token: 0x04000FB0 RID: 4016
	public float maxheight;

	// Token: 0x04000FB1 RID: 4017
	public float maxheight2;

	// Token: 0x04000FB2 RID: 4018
	public bool doonce;

	// Token: 0x04000FB3 RID: 4019
	public float a;

	// Token: 0x04000FB4 RID: 4020
	public float randomx;

	// Token: 0x04000FB5 RID: 4021
	public bool camerafollow;
}
