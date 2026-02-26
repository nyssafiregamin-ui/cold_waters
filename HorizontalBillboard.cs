using System;
using UnityEngine;

// Token: 0x02000127 RID: 295
public class HorizontalBillboard : MonoBehaviour
{
	// Token: 0x06000803 RID: 2051 RVA: 0x0004EBAC File Offset: 0x0004CDAC
	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f);
		Vector3 position = base.transform.position;
		if (this.billboardheight > 0f)
		{
			base.transform.position = new Vector3(position.x, this.billboardheight, position.z);
		}
	}

	// Token: 0x04000BBA RID: 3002
	public float billboardheight;
}
