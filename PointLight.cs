using System;
using UnityEngine;

// Token: 0x02000144 RID: 324
public class PointLight : MonoBehaviour
{
	// Token: 0x0600096B RID: 2411 RVA: 0x0006D3C4 File Offset: 0x0006B5C4
	private void OnEnable()
	{
		this.pointLight.enabled = true;
		this.pointLight.intensity = 0f;
	}

	// Token: 0x0600096C RID: 2412 RVA: 0x0006D3E4 File Offset: 0x0006B5E4
	private void FixedUpdate()
	{
		this.timer += Time.deltaTime;
		this.pointLight.intensity = this.curve.Evaluate(this.timer);
		if (this.timer > 3.5f)
		{
			base.enabled = false;
			this.pointLight.intensity = 0f;
		}
	}

	// Token: 0x04000E54 RID: 3668
	public Light pointLight;

	// Token: 0x04000E55 RID: 3669
	public AnimationCurve curve;

	// Token: 0x04000E56 RID: 3670
	public Color lightColor;

	// Token: 0x04000E57 RID: 3671
	public float timer;
}
