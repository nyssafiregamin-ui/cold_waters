using System;
using UnityEngine;

// Token: 0x02000172 RID: 370
public class WaveMotion : MonoBehaviour
{
	// Token: 0x06000B43 RID: 2883 RVA: 0x000A5CF4 File Offset: 0x000A3EF4
	private void FixedUpdate()
	{
		float num = 200f / this.compartmentvolume * 1f + 0.4f;
		float xclock = GameDataManager.xclock;
		float num2 = num * GameDataManager.yclock / 6f;
		base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, Quaternion.Euler(num2, 0f, num * xclock), 1f);
		num2 /= 100f;
	}

	// Token: 0x04001238 RID: 4664
	public float compartmentvolume;
}
