using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200011D RID: 285
public class EventTemplate : MonoBehaviour
{
	// Token: 0x060007AD RID: 1965 RVA: 0x00048B9C File Offset: 0x00046D9C
	private void Start()
	{
		this.alpha = 0f;
		this.timer = 0f;
		this.fading = false;
		this.SetAlphas();
	}

	// Token: 0x060007AE RID: 1966 RVA: 0x00048BC4 File Offset: 0x00046DC4
	private void FixedUpdate()
	{
		if (this.fading)
		{
			this.alpha += Time.deltaTime;
			if (this.alpha >= 1f)
			{
				this.alpha = 1f;
				base.enabled = false;
				UIFunctions.globaluifunctions.campaignmanager.eventManager.continueButton.SetActive(true);
			}
			this.SetAlphas();
			return;
		}
		this.timer += Time.deltaTime;
		if (this.timer > this.timeToFadeIn)
		{
			this.timer = 0f;
			this.fading = true;
		}
	}

	// Token: 0x060007AF RID: 1967 RVA: 0x00048C68 File Offset: 0x00046E68
	private void SetAlphas()
	{
		for (int i = 0; i < this.templateImages.Length; i++)
		{
			this.templateImages[i].color = new Color(this.templateImages[i].color.r, this.templateImages[i].color.g, this.templateImages[i].color.b, this.alpha);
		}
		for (int j = 0; j < this.templateTexts.Length; j++)
		{
			this.templateTexts[j].color = new Color(this.templateTexts[j].color.r, this.templateTexts[j].color.g, this.templateTexts[j].color.b, this.alpha);
		}
	}

	// Token: 0x04000AD0 RID: 2768
	public Image[] templateImages;

	// Token: 0x04000AD1 RID: 2769
	public Text[] templateTexts;

	// Token: 0x04000AD2 RID: 2770
	public float timeToFadeIn;

	// Token: 0x04000AD3 RID: 2771
	public float fadeInTime;

	// Token: 0x04000AD4 RID: 2772
	public float alpha;

	// Token: 0x04000AD5 RID: 2773
	public bool fading;

	// Token: 0x04000AD6 RID: 2774
	public float timer;
}
