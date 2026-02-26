using System;
using UnityEngine;

// Token: 0x020000F0 RID: 240
public class CameraFacingBillboard : MonoBehaviour
{
	// Token: 0x0600065B RID: 1627 RVA: 0x0002D89C File Offset: 0x0002BA9C
	private void Update()
	{
		if (!GameDataManager.HUDActive)
		{
			return;
		}
		base.transform.LookAt(base.transform.position + UIFunctions.globaluifunctions.MainCamera.transform.rotation * Vector3.back, UIFunctions.globaluifunctions.MainCamera.transform.rotation * Vector3.up);
		if (this.limit > 0f && base.transform.rotation.eulerAngles.x < 360f - this.limit)
		{
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(360f - this.limit, base.transform.eulerAngles.y, base.transform.eulerAngles.z), 1f);
		}
	}

	// Token: 0x040006C2 RID: 1730
	public float limit;
}
