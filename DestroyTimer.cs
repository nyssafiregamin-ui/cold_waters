using System;
using UnityEngine;

// Token: 0x0200010C RID: 268
public class DestroyTimer : MonoBehaviour
{
	// Token: 0x06000737 RID: 1847 RVA: 0x000401A4 File Offset: 0x0003E3A4
	private void Start()
	{
		this.countdown = this.timer;
	}

	// Token: 0x06000738 RID: 1848 RVA: 0x000401B4 File Offset: 0x0003E3B4
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
	}

	// Token: 0x040009C7 RID: 2503
	public float timer;

	// Token: 0x040009C8 RID: 2504
	private float countdown;
}
