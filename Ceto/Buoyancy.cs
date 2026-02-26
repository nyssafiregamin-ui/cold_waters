using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x0200005E RID: 94
	[AddComponentMenu("Ceto/Buoyancy/Buoyancy")]
	public class Buoyancy : MonoBehaviour
	{
		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060002E0 RID: 736 RVA: 0x00011074 File Offset: 0x0000F274
		// (set) Token: 0x060002E1 RID: 737 RVA: 0x0001107C File Offset: 0x0000F27C
		public bool PartOfStructure { get; set; }

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060002E2 RID: 738 RVA: 0x00011088 File Offset: 0x0000F288
		// (set) Token: 0x060002E3 RID: 739 RVA: 0x00011090 File Offset: 0x0000F290
		public float Volume { get; private set; }

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060002E4 RID: 740 RVA: 0x0001109C File Offset: 0x0000F29C
		// (set) Token: 0x060002E5 RID: 741 RVA: 0x000110A4 File Offset: 0x0000F2A4
		public float SubmergedVolume { get; private set; }

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060002E6 RID: 742 RVA: 0x000110B0 File Offset: 0x0000F2B0
		public float PercentageSubmerged
		{
			get
			{
				return this.SubmergedVolume / this.Volume;
			}
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060002E7 RID: 743 RVA: 0x000110C0 File Offset: 0x0000F2C0
		// (set) Token: 0x060002E8 RID: 744 RVA: 0x000110C8 File Offset: 0x0000F2C8
		public float SurfaceArea { get; private set; }

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x060002E9 RID: 745 RVA: 0x000110D4 File Offset: 0x0000F2D4
		// (set) Token: 0x060002EA RID: 746 RVA: 0x000110DC File Offset: 0x0000F2DC
		public float Mass { get; private set; }

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x060002EB RID: 747 RVA: 0x000110E8 File Offset: 0x0000F2E8
		// (set) Token: 0x060002EC RID: 748 RVA: 0x000110F0 File Offset: 0x0000F2F0
		public float WaterHeight { get; private set; }

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x060002ED RID: 749 RVA: 0x000110FC File Offset: 0x0000F2FC
		// (set) Token: 0x060002EE RID: 750 RVA: 0x00011104 File Offset: 0x0000F304
		public Vector3 BuoyantForce { get; private set; }

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x060002EF RID: 751 RVA: 0x00011110 File Offset: 0x0000F310
		// (set) Token: 0x060002F0 RID: 752 RVA: 0x00011118 File Offset: 0x0000F318
		public Vector3 DragForce { get; private set; }

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x060002F1 RID: 753 RVA: 0x00011124 File Offset: 0x0000F324
		// (set) Token: 0x060002F2 RID: 754 RVA: 0x0001112C File Offset: 0x0000F32C
		public Vector3 Stickyness { get; private set; }

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x060002F3 RID: 755 RVA: 0x00011138 File Offset: 0x0000F338
		public Vector3 TotalForces
		{
			get
			{
				return this.BuoyantForce + this.DragForce + this.Stickyness;
			}
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x00011164 File Offset: 0x0000F364
		private void Start()
		{
			this.UpdateProperties();
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x0001116C File Offset: 0x0000F36C
		private void FixedUpdate()
		{
			if (this.PartOfStructure)
			{
				return;
			}
			Rigidbody rigidbody = base.GetComponent<Rigidbody>();
			if (rigidbody == null)
			{
				rigidbody = base.gameObject.AddComponent<Rigidbody>();
			}
			rigidbody.mass = this.Mass;
			this.UpdateProperties();
			this.UpdateForces(rigidbody);
			rigidbody.AddForce(this.TotalForces);
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x000111CC File Offset: 0x0000F3CC
		public void UpdateProperties()
		{
			this.Volume = 4.1887903f * Mathf.Pow(this.radius, 3f);
			this.Mass = this.Volume * this.density * this.GetUnitScale();
			this.SurfaceArea = 12.566371f * Mathf.Pow(this.radius, 2f);
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x0001122C File Offset: 0x0000F42C
		public void UpdateForces(Rigidbody body)
		{
			if (Ocean.Instance == null)
			{
				this.BuoyantForce = Vector3.zero;
				this.DragForce = Vector3.zero;
				this.Stickyness = Vector3.zero;
				return;
			}
			Vector3 position = base.transform.position;
			this.WaterHeight = Ocean.Instance.QueryWaves(position.x, position.z);
			this.CalculateSubmersion(this.radius, position.y);
			float unitScale = this.GetUnitScale();
			float num = this.DENSITY_WATER * unitScale * this.SubmergedVolume;
			this.BuoyantForce = Physics.gravity * -num;
			Vector3 a = body.velocity;
			float magnitude = a.magnitude;
			a = a.normalized * magnitude * magnitude * -1f;
			this.DragForce = 0.5f * this.dragCoefficient * this.DENSITY_WATER * unitScale * this.SubmergedVolume * a;
			this.Stickyness = Vector3.up * (this.WaterHeight - position.y) * this.Mass * this.stickyness;
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x00011360 File Offset: 0x0000F560
		private void CalculateSubmersion(float r, float y)
		{
			float num = this.WaterHeight - (y - this.radius);
			float num2 = 2f * r - num;
			if (num2 <= 0f)
			{
				this.SubmergedVolume = this.Volume;
				return;
			}
			if (num2 > 2f * r)
			{
				this.SubmergedVolume = 0f;
				return;
			}
			float num3 = Mathf.Sqrt(num * num2);
			this.SubmergedVolume = 0.5235988f * num * (3f * num3 * num3 + num * num);
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x000113DC File Offset: 0x0000F5DC
		private float GetUnitScale()
		{
			switch (this.unit)
			{
			case Buoyancy.MASS_UNIT.KILOGRAMS:
				return 1f;
			case Buoyancy.MASS_UNIT.TENS_OF_KILOGRAMS:
				return 0.1f;
			case Buoyancy.MASS_UNIT.TONNES:
				return 0.001f;
			case Buoyancy.MASS_UNIT.TENS_OF_TONNES:
				return 0.0001f;
			default:
				return 1f;
			}
		}

		// Token: 0x060002FA RID: 762 RVA: 0x00011428 File Offset: 0x0000F628
		private void OnDrawGizmos()
		{
			if (!base.enabled)
			{
				return;
			}
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(base.transform.position, this.radius);
		}

		// Token: 0x040002AC RID: 684
		private float DENSITY_WATER = 999.97f;

		// Token: 0x040002AD RID: 685
		public float radius = 0.5f;

		// Token: 0x040002AE RID: 686
		[Range(100f, 10000f)]
		public float density = 400f;

		// Token: 0x040002AF RID: 687
		[Range(0f, 100f)]
		public float stickyness = 0.1f;

		// Token: 0x040002B0 RID: 688
		public Buoyancy.MASS_UNIT unit = Buoyancy.MASS_UNIT.TENS_OF_TONNES;

		// Token: 0x040002B1 RID: 689
		public float dragCoefficient = 0.3f;

		// Token: 0x0200005F RID: 95
		public enum MASS_UNIT
		{
			// Token: 0x040002BC RID: 700
			KILOGRAMS,
			// Token: 0x040002BD RID: 701
			TENS_OF_KILOGRAMS,
			// Token: 0x040002BE RID: 702
			TONNES,
			// Token: 0x040002BF RID: 703
			TENS_OF_TONNES
		}
	}
}
