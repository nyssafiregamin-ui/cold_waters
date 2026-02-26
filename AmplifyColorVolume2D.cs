using System;
using UnityEngine;

// Token: 0x02000028 RID: 40
[RequireComponent(typeof(BoxCollider2D))]
[AddComponentMenu("Image Effects/Amplify Color Volume 2D")]
public class AmplifyColorVolume2D : AmplifyColorVolumeBase
{
	// Token: 0x0600014E RID: 334 RVA: 0x00008A0C File Offset: 0x00006C0C
	private void OnTriggerEnter2D(Collider2D other)
	{
		AmplifyColorTriggerProxy2D component = other.GetComponent<AmplifyColorTriggerProxy2D>();
		if (component != null && component.OwnerEffect.UseVolumes && (component.OwnerEffect.VolumeCollisionMask & 1 << base.gameObject.layer) != 0)
		{
			component.OwnerEffect.EnterVolume(this);
		}
	}

	// Token: 0x0600014F RID: 335 RVA: 0x00008A70 File Offset: 0x00006C70
	private void OnTriggerExit2D(Collider2D other)
	{
		AmplifyColorTriggerProxy2D component = other.GetComponent<AmplifyColorTriggerProxy2D>();
		if (component != null && component.OwnerEffect.UseVolumes && (component.OwnerEffect.VolumeCollisionMask & 1 << base.gameObject.layer) != 0)
		{
			component.OwnerEffect.ExitVolume(this);
		}
	}
}
