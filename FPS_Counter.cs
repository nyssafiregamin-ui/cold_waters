using System;
using UnityEngine;

// Token: 0x0200011E RID: 286
public class FPS_Counter : MonoBehaviour
{
	// Token: 0x060007B1 RID: 1969 RVA: 0x00048D70 File Offset: 0x00046F70
	private void Update()
	{
		this.frameCount++;
		this.dt += Time.deltaTime;
		if (this.dt > 1f / this.updateRate)
		{
			this.fps = (float)this.frameCount / this.dt;
			this.frameCount = 0;
			this.dt -= 1f / this.updateRate;
			this.fpscounterdisplay.Text = string.Format("{0:0.000}", this.fps);
		}
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x00048E08 File Offset: 0x00047008
	private void SwapOcean()
	{
		int stateNum = this.oceanToggle.StateNum;
		if (stateNum != 0)
		{
			if (stateNum == 1)
			{
				this.oceanLarge.SetActive(false);
				this.oceanSmall.SetActive(true);
			}
		}
		else
		{
			this.oceanLarge.SetActive(true);
			this.oceanSmall.SetActive(false);
		}
	}

	// Token: 0x04000AD7 RID: 2775
	public SpriteText fpscounterdisplay;

	// Token: 0x04000AD8 RID: 2776
	private int frameCount;

	// Token: 0x04000AD9 RID: 2777
	private float dt;

	// Token: 0x04000ADA RID: 2778
	private float fps;

	// Token: 0x04000ADB RID: 2779
	private float updateRate = 4f;

	// Token: 0x04000ADC RID: 2780
	public GameObject oceanLarge;

	// Token: 0x04000ADD RID: 2781
	public GameObject oceanSmall;

	// Token: 0x04000ADE RID: 2782
	public UIStateToggleBtn oceanToggle;
}
