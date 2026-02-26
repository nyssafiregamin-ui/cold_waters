using System;
using UnityEngine;

// Token: 0x02000121 RID: 289
public class FollowObject : MonoBehaviour
{
	// Token: 0x060007B8 RID: 1976 RVA: 0x00048F48 File Offset: 0x00047148
	private void FixedUpdate()
	{
		base.transform.position = new Vector3(this.objectToFollow.position.x, this.height, this.objectToFollow.position.z);
	}

	// Token: 0x04000AE4 RID: 2788
	public Transform objectToFollow;

	// Token: 0x04000AE5 RID: 2789
	public float height;
}
