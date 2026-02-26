using System;
using UnityEngine;

// Token: 0x02000024 RID: 36
[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("")]
[RequireComponent(typeof(SphereCollider))]
public class AmplifyColorTriggerProxy : AmplifyColorTriggerProxyBase
{
	// Token: 0x06000144 RID: 324 RVA: 0x000087F0 File Offset: 0x000069F0
	private void Start()
	{
		this.sphereCollider = base.GetComponent<SphereCollider>();
		this.sphereCollider.radius = 0.01f;
		this.sphereCollider.isTrigger = true;
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.rigidBody.useGravity = false;
		this.rigidBody.isKinematic = true;
	}

	// Token: 0x06000145 RID: 325 RVA: 0x0000884C File Offset: 0x00006A4C
	private void LateUpdate()
	{
		base.transform.position = this.Reference.position;
		base.transform.rotation = this.Reference.rotation;
	}

	// Token: 0x0400017E RID: 382
	private SphereCollider sphereCollider;

	// Token: 0x0400017F RID: 383
	private Rigidbody rigidBody;
}
