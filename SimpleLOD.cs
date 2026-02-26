using System;
using UnityEngine;

// Token: 0x02000157 RID: 343
public class SimpleLOD : MonoBehaviour
{
	// Token: 0x06000A10 RID: 2576 RVA: 0x0007B200 File Offset: 0x00079400
	private void LateUpdate()
	{
		if (Vector3.Distance(base.transform.position, UIFunctions.globaluifunctions.MainCamera.transform.position) < this.dist)
		{
			if (!this.isOn)
			{
				this.SetChilderen(true);
			}
		}
		else if (this.isOn)
		{
			this.SetChilderen(false);
		}
	}

	// Token: 0x06000A11 RID: 2577 RVA: 0x0007B268 File Offset: 0x00079468
	private void SetChilderen(bool on)
	{
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			transform.gameObject.SetActive(on);
		}
		this.isOn = on;
	}

	// Token: 0x04000F82 RID: 3970
	public float dist = 350f;

	// Token: 0x04000F83 RID: 3971
	public bool isOn = true;
}
