using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x02000060 RID: 96
	[AddComponentMenu("Ceto/Buoyancy/BuoyantStructure")]
	public class BuoyantStructure : MonoBehaviour
	{
		// Token: 0x060002FC RID: 764 RVA: 0x00011478 File Offset: 0x0000F678
		private void Start()
		{
			this.m_buoyancy = base.GetComponentsInChildren<Buoyancy>();
			int num = this.m_buoyancy.Length;
			for (int i = 0; i < num; i++)
			{
				this.m_buoyancy[i].PartOfStructure = true;
			}
		}

		// Token: 0x060002FD RID: 765 RVA: 0x000114BC File Offset: 0x0000F6BC
		private void FixedUpdate()
		{
			Rigidbody rigidbody = base.GetComponent<Rigidbody>();
			if (rigidbody == null)
			{
				rigidbody = base.gameObject.AddComponent<Rigidbody>();
			}
			float num = 0f;
			int num2 = this.m_buoyancy.Length;
			for (int i = 0; i < num2; i++)
			{
				if (this.m_buoyancy[i].enabled)
				{
					this.m_buoyancy[i].UpdateProperties();
					num += this.m_buoyancy[i].Mass;
				}
			}
			rigidbody.mass = num;
			Vector3 position = base.transform.position;
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			for (int j = 0; j < num2; j++)
			{
				if (this.m_buoyancy[j].enabled)
				{
					this.m_buoyancy[j].UpdateForces(rigidbody);
					Vector3 position2 = this.m_buoyancy[j].transform.position;
					Vector3 totalForces = this.m_buoyancy[j].TotalForces;
					Vector3 lhs = position2 - position;
					vector += totalForces;
					vector2 += Vector3.Cross(lhs, totalForces);
				}
			}
			rigidbody.maxAngularVelocity = this.maxAngularVelocity;
			rigidbody.AddForce(vector);
			rigidbody.AddTorque(vector2);
		}

		// Token: 0x040002C0 RID: 704
		public float maxAngularVelocity = 0.05f;

		// Token: 0x040002C1 RID: 705
		private Buoyancy[] m_buoyancy;
	}
}
