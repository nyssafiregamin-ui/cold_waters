using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000135 RID: 309
public class MapContact : MonoBehaviour
{
	// Token: 0x06000870 RID: 2160 RVA: 0x0005E544 File Offset: 0x0005C744
	private void Start()
	{
		if (this.flashIconTime == 0f)
		{
			base.enabled = false;
		}
		this.contactNumberOfMoves = 0;
	}

	// Token: 0x06000871 RID: 2161 RVA: 0x0005E564 File Offset: 0x0005C764
	private void FixedUpdate()
	{
		this.flashIconTimer += Time.deltaTime;
		if (this.flashIconTimer > this.flashIconTime)
		{
			if (this.shipDisplayIcon.gameObject.activeSelf)
			{
				this.shipDisplayIcon.gameObject.SetActive(false);
			}
			else
			{
				this.shipDisplayIcon.gameObject.SetActive(true);
			}
			this.flashIconTimer -= this.flashIconTime;
		}
	}

	// Token: 0x04000D0B RID: 3339
	public Text contactText;

	// Token: 0x04000D0C RID: 3340
	public Image shipDisplayIcon;

	// Token: 0x04000D0D RID: 3341
	public RectTransform shipRectTransform;

	// Token: 0x04000D0E RID: 3342
	public Queue<Transform> positionMarkerQueue;

	// Token: 0x04000D0F RID: 3343
	public int contactNumberOfMoves;

	// Token: 0x04000D10 RID: 3344
	public Button contactButton;

	// Token: 0x04000D11 RID: 3345
	public LineRenderer[] sensorConeLines;

	// Token: 0x04000D12 RID: 3346
	public float flashIconTimer;

	// Token: 0x04000D13 RID: 3347
	public float flashIconTime;
}
