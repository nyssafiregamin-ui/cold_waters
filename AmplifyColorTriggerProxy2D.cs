using System;
using UnityEngine;

// Token: 0x02000025 RID: 37
[AddComponentMenu("")]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class AmplifyColorTriggerProxy2D : AmplifyColorTriggerProxyBase
{
	// Token: 0x06000147 RID: 327 RVA: 0x00008890 File Offset: 0x00006A90
	private void Start()
	{
		this.circleCollider = base.GetComponent<CircleCollider2D>();
		this.circleCollider.radius = 0.01f;
		this.circleCollider.isTrigger = true;
		this.rigidBody = base.GetComponent<Rigidbody2D>();
		this.rigidBody.gravityScale = 0f;
		this.rigidBody.isKinematic = true;
	}

	// Token: 0x06000148 RID: 328 RVA: 0x000088F0 File Offset: 0x00006AF0
	private void LateUpdate()
	{
		base.transform.position = this.Reference.position;
		base.transform.rotation = this.Reference.rotation;
	}

	// Token: 0x04000180 RID: 384
	private CircleCollider2D circleCollider;

	// Token: 0x04000181 RID: 385
	private Rigidbody2D rigidBody;
}
