using System;
using UnityEngine;

// Token: 0x0200013D RID: 317
public class OceanTiling : MonoBehaviour
{
	// Token: 0x060008BB RID: 2235 RVA: 0x00062EFC File Offset: 0x000610FC
	private void FixedUpdate()
	{
		if (this.oceanposition.gameObject == null)
		{
			return;
		}
		Vector3 a = base.transform.position + base.transform.forward * 6f;
		a = new Vector3(a.x, 1000f, a.z);
		this.distance = Vector3.Distance(a, new Vector3(this.oceanposition.position.x, 1000f, this.oceanposition.position.z));
		if (this.distance > 5f)
		{
			this.oceanposition.position = new Vector3(a.x, this.oceanposition.position.y, a.z);
			this.x = Mathf.Round(this.oceanposition.localPosition.x / this.gridsize) * this.gridsize;
			this.z = Mathf.Round(this.oceanposition.localPosition.z / this.gridsize) * this.gridsize;
			this.oceanposition.localPosition = new Vector3(this.x, this.oceanposition.localPosition.y, this.z);
			this.moved = true;
		}
	}

	// Token: 0x04000D71 RID: 3441
	public float gridsize;

	// Token: 0x04000D72 RID: 3442
	public float x;

	// Token: 0x04000D73 RID: 3443
	public float z;

	// Token: 0x04000D74 RID: 3444
	public float distance;

	// Token: 0x04000D75 RID: 3445
	public bool moved;

	// Token: 0x04000D76 RID: 3446
	public Transform oceanposition;
}
