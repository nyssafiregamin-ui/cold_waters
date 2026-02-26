using System;
using UnityEngine;

// Token: 0x0200015D RID: 349
public class SpriteComplex : MonoBehaviour
{
	// Token: 0x06000A1E RID: 2590 RVA: 0x0007B4D0 File Offset: 0x000796D0
	private void Start()
	{
		base.transform.localScale = new Vector3(1f, 1f, 1f);
		this.modifier = UnityEngine.Random.value * 0.5f;
		this.sprite.SetSize(this.startwidth, this.startheight);
		this.a = 0f;
		this.b = 0f;
		this.timer = 0f;
	}

	// Token: 0x06000A1F RID: 2591 RVA: 0x0007B548 File Offset: 0x00079748
	private void FixedUpdate()
	{
		this.timer += Time.deltaTime;
		if (this.timer > this.fadeafter)
		{
			this.a += this.faderate * Time.deltaTime * 50f;
		}
		if (this.timer > this.expandafter)
		{
			this.b += this.growthrate * Time.deltaTime * 50f * (1f + this.modifier);
		}
		Color color = new Color(this.basecolor.r, this.basecolor.g, this.basecolor.b, this.basecolor.a - this.a);
		if (GameDataManager.isNight && this.nightfade)
		{
			color = new Color(this.basecolor.r * 0.5f, this.basecolor.g * 0.5f, this.basecolor.b * 0.5f, this.basecolor.a - this.a);
		}
		else if (this.nightfade && GameDataManager.weathertype > 3 && GameDataManager.weathertype < 8)
		{
			color = new Color(this.basecolor.r * 0.75f, this.basecolor.g * 0.75f, this.basecolor.b * 0.75f, this.basecolor.a - this.a);
		}
		this.sprite.SetColor(color);
		this.sprite.SetSize(this.startwidth + this.b, this.startheight + this.b);
	}

	// Token: 0x04000F8F RID: 3983
	public PackedSprite sprite;

	// Token: 0x04000F90 RID: 3984
	public Color basecolor;

	// Token: 0x04000F91 RID: 3985
	public float startwidth;

	// Token: 0x04000F92 RID: 3986
	public float startheight;

	// Token: 0x04000F93 RID: 3987
	public float growthrate;

	// Token: 0x04000F94 RID: 3988
	public float faderate;

	// Token: 0x04000F95 RID: 3989
	public float a;

	// Token: 0x04000F96 RID: 3990
	public float b;

	// Token: 0x04000F97 RID: 3991
	public float fadeafter;

	// Token: 0x04000F98 RID: 3992
	public float expandafter;

	// Token: 0x04000F99 RID: 3993
	public float timer;

	// Token: 0x04000F9A RID: 3994
	public bool nightfade;

	// Token: 0x04000F9B RID: 3995
	private float modifier;
}
