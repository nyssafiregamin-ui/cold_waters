using System;
using UnityEngine;

// Token: 0x02000100 RID: 256
public class CreditScroll : MonoBehaviour
{
	// Token: 0x06000701 RID: 1793 RVA: 0x0003CF90 File Offset: 0x0003B190
	private void Start()
	{
		UIFunctions.globaluifunctions.mainTitle.text = string.Empty;
		UIFunctions.globaluifunctions.secondaryTitle.text = string.Empty;
		UIFunctions.globaluifunctions.mainColumn.text = string.Empty;
		UIFunctions.globaluifunctions.secondColumm.text = string.Empty;
		this.creditObject.transform.localPosition = new Vector3(this.creditObject.transform.localPosition.x, 0f, this.creditObject.transform.localPosition.z);
		Time.timeScale = 1f;
	}

	// Token: 0x06000702 RID: 1794 RVA: 0x0003D044 File Offset: 0x0003B244
	private void Update()
	{
		this.creditObject.transform.Translate(Vector3.up * this.scrollSpeed * Time.deltaTime);
		this.scrolltimer += Time.deltaTime;
		if (this.scrolltimer > this.resetTime)
		{
			this.scrolltimer = 0f;
			this.creditObject.transform.localPosition = new Vector3(this.creditObject.transform.localPosition.x, 0f, this.creditObject.transform.localPosition.z);
		}
	}

	// Token: 0x0400085F RID: 2143
	public Transform creditObject;

	// Token: 0x04000860 RID: 2144
	public float scrollSpeed;

	// Token: 0x04000861 RID: 2145
	public float scrolltimer;

	// Token: 0x04000862 RID: 2146
	public float resetTime;
}
