using System;
using UnityEngine;

// Token: 0x02000120 RID: 288
public class FollowCameraTargetTransform : MonoBehaviour
{
	// Token: 0x060007B6 RID: 1974 RVA: 0x00048EE8 File Offset: 0x000470E8
	private void Update()
	{
		if (ManualCameraZoom.target != null)
		{
			base.transform.position = new Vector3(ManualCameraZoom.target.position.x, 1000f, ManualCameraZoom.target.position.z);
		}
	}
}
