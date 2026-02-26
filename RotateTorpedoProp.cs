using System;
using UnityEngine;

// Token: 0x02000151 RID: 337
public class RotateTorpedoProp : MonoBehaviour
{
	// Token: 0x060009BB RID: 2491 RVA: 0x00070314 File Offset: 0x0006E514
	private void Update()
	{
		base.transform.Rotate(Vector3.up * Time.deltaTime * this.speed);
	}

	// Token: 0x04000E95 RID: 3733
	public float speed;
}
