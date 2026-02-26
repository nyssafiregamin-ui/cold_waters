using System;
using UnityEngine;

// Token: 0x0200010D RID: 269
public class DestroyonHUDoff : MonoBehaviour
{
	// Token: 0x0600073A RID: 1850 RVA: 0x00040210 File Offset: 0x0003E410
	private void FixedUpdate()
	{
		if (!GameDataManager.HUDActive)
		{
			ObjectPoolManager.DestroyPooled(base.gameObject);
		}
	}
}
