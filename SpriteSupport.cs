using System;
using UnityEngine;

// Token: 0x0200015F RID: 351
public class SpriteSupport : MonoBehaviour
{
	// Token: 0x06000A22 RID: 2594 RVA: 0x0007B724 File Offset: 0x00079924
	private void Start()
	{
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		this.countdown = this.timer;
		this.spriterenderer.GetComponent<Renderer>().enabled = true;
		if (GameDataManager.isNight && this.nightfade)
		{
			this.sprite.SetColor(new Color(0.5f, 0.5f, 0.5f, 1f));
		}
		else if (this.nightfade)
		{
			if (GameDataManager.weathertype < 4)
			{
				this.sprite.SetColor(new Color(1f, 1f, 1f, 1f));
			}
			else
			{
				this.sprite.SetColor(new Color(0.75f, 0.75f, 0.75f, 1f));
			}
		}
		this.sprite.DoAnim(this.animationname);
		if (this.cameraface)
		{
			base.transform.LookAt(base.transform.position + Camera.main.transform.rotation * Vector3.back, Camera.main.transform.rotation * Vector3.up);
		}
	}

	// Token: 0x06000A23 RID: 2595 RVA: 0x0007B878 File Offset: 0x00079A78
	private void FixedUpdate()
	{
		if (!GameDataManager.HUDActive)
		{
			this.countdown = -1f;
		}
		if (this.countdown > 0f)
		{
			this.countdown -= Time.deltaTime;
		}
		else
		{
			ObjectPoolManager.DestroyPooled(base.gameObject);
		}
		if (this.camerafollow && GameDataManager.HUDActive)
		{
			base.transform.LookAt(base.transform.position + Camera.main.transform.rotation * Vector3.back, Camera.main.transform.rotation * Vector3.up);
		}
	}

	// Token: 0x04000FA8 RID: 4008
	public float timer;

	// Token: 0x04000FA9 RID: 4009
	public MeshRenderer spriterenderer;

	// Token: 0x04000FAA RID: 4010
	public PackedSprite sprite;

	// Token: 0x04000FAB RID: 4011
	private float countdown;

	// Token: 0x04000FAC RID: 4012
	public string animationname;

	// Token: 0x04000FAD RID: 4013
	public bool cameraface;

	// Token: 0x04000FAE RID: 4014
	public bool camerafollow;

	// Token: 0x04000FAF RID: 4015
	public bool nightfade;
}
