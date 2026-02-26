using System;
using UnityEngine;

// Token: 0x0200014F RID: 335
public class Radar : MonoBehaviour
{
	// Token: 0x060009B6 RID: 2486 RVA: 0x00070268 File Offset: 0x0006E468
	private void Update()
	{
		base.transform.Rotate(Vector3.up * Time.deltaTime * this.speed);
		if (this.isDestroyed)
		{
			this.speed -= Time.deltaTime * 20f;
			if (this.speed <= 0f)
			{
				base.enabled = false;
			}
		}
	}

	// Token: 0x04000E92 RID: 3730
	public float speed;

	// Token: 0x04000E93 RID: 3731
	public bool isDestroyed;
}
