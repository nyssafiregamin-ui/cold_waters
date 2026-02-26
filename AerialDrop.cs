using System;
using UnityEngine;

// Token: 0x020000E9 RID: 233
public class AerialDrop : MonoBehaviour
{
	// Token: 0x0600062A RID: 1578 RVA: 0x0002AE54 File Offset: 0x00029054
	private void FixedUpdate()
	{
		if (base.transform.position.y > 1000f)
		{
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y - Time.deltaTime * this.fallrate, base.transform.position.z);
			base.transform.Rotate(Vector3.right * Time.deltaTime * 8f);
			if (this.hasParachute && base.transform.position.y < 1000.4f)
			{
				this.parachute.Play();
				this.hasParachute = false;
			}
		}
		else
		{
			base.enabled = false;
		}
	}

	// Token: 0x04000672 RID: 1650
	public float fallrate;

	// Token: 0x04000673 RID: 1651
	public bool hasParachute;

	// Token: 0x04000674 RID: 1652
	public ParticleSystem parachute;
}
