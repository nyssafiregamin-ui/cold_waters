using System;
using UnityEngine;

// Token: 0x02000027 RID: 39
[RequireComponent(typeof(BoxCollider))]
[AddComponentMenu("Image Effects/Amplify Color Volume")]
public class AmplifyColorVolume : AmplifyColorVolumeBase
{
	// Token: 0x0600014B RID: 331 RVA: 0x0000893C File Offset: 0x00006B3C
	private void OnTriggerEnter(Collider other)
	{
		AmplifyColorTriggerProxy component = other.GetComponent<AmplifyColorTriggerProxy>();
		if (component != null && component.OwnerEffect.UseVolumes && (component.OwnerEffect.VolumeCollisionMask & 1 << base.gameObject.layer) != 0)
		{
			component.OwnerEffect.EnterVolume(this);
		}
	}

	// Token: 0x0600014C RID: 332 RVA: 0x000089A0 File Offset: 0x00006BA0
	private void OnTriggerExit(Collider other)
	{
		AmplifyColorTriggerProxy component = other.GetComponent<AmplifyColorTriggerProxy>();
		if (component != null && component.OwnerEffect.UseVolumes && (component.OwnerEffect.VolumeCollisionMask & 1 << base.gameObject.layer) != 0)
		{
			component.OwnerEffect.ExitVolume(this);
		}
	}
}
