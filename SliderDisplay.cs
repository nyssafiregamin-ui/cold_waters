using System;
using UnityEngine;

// Token: 0x02000159 RID: 345
public class SliderDisplay : MonoBehaviour
{
	// Token: 0x06000A16 RID: 2582 RVA: 0x0007B454 File Offset: 0x00079654
	private void LateUpdate()
	{
		float x = this.length * (this.slider.Value - 0.5f) * 2f;
		base.transform.localPosition = new Vector3(x, 0f, 0f);
	}

	// Token: 0x04000F88 RID: 3976
	public UISlider slider;

	// Token: 0x04000F89 RID: 3977
	public float length;
}
