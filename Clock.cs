using System;
using UnityEngine;

// Token: 0x020000FA RID: 250
public class Clock : MonoBehaviour
{
	// Token: 0x060006C4 RID: 1732 RVA: 0x00037E2C File Offset: 0x0003602C
	public void Start()
	{
		base.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
		if (this.x)
		{
			this.handx.transform.localPosition = new Vector3(0f, this.xlength, 0f);
		}
		else
		{
			this.handx.transform.localPosition = new Vector3(0f, this.ylength, 0f);
		}
		GameDataManager.xclock = 0f;
		GameDataManager.yclock = 0f;
	}

	// Token: 0x060006C5 RID: 1733 RVA: 0x00037EC8 File Offset: 0x000360C8
	private void FixedUpdate()
	{
		if (this.x)
		{
			base.transform.Rotate(Vector3.forward * Time.deltaTime * this.xspeed);
			GameDataManager.xclock = this.handx.transform.position.y;
		}
		else
		{
			base.transform.Rotate(Vector3.forward * Time.deltaTime * this.yspeed);
			GameDataManager.yclock = this.handx.transform.position.y;
		}
	}

	// Token: 0x04000802 RID: 2050
	public GameObject handx;

	// Token: 0x04000803 RID: 2051
	public float xspeed;

	// Token: 0x04000804 RID: 2052
	public float yspeed;

	// Token: 0x04000805 RID: 2053
	public float xlength;

	// Token: 0x04000806 RID: 2054
	public float ylength;

	// Token: 0x04000807 RID: 2055
	public bool x;
}
