using System;
using UnityEngine;

// Token: 0x02000163 RID: 355
public class SubmarineMarker : MonoBehaviour
{
	// Token: 0x06000A42 RID: 2626 RVA: 0x0007D8F0 File Offset: 0x0007BAF0
	public void LateUpdate()
	{
		base.transform.position = new Vector3(this.playerTransform.position.x, this.waterLevel, this.playerTransform.position.z);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(0f, this.playerTransform.eulerAngles.y + 90f, 0f), 1f);
	}

	// Token: 0x04000FE0 RID: 4064
	public Transform playerTransform;

	// Token: 0x04000FE1 RID: 4065
	public float waterLevel = 1000f;
}
