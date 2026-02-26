using System;
using UnityEngine;

namespace Ceto.Common.Unity.Utility
{
	// Token: 0x02000059 RID: 89
	public class RotateLight : MonoBehaviour
	{
		// Token: 0x060002BE RID: 702 RVA: 0x0000F9BC File Offset: 0x0000DBBC
		private void Start()
		{
		}

		// Token: 0x060002BF RID: 703 RVA: 0x0000F9C0 File Offset: 0x0000DBC0
		private void Update()
		{
			float num = Time.deltaTime * this.speed;
			Vector3 eulerAngles = new Vector3(num, num, num);
			if (Input.GetKey(this.decrementKey))
			{
				eulerAngles.x *= -this.axis.x;
				eulerAngles.y *= -this.axis.y;
				eulerAngles.z *= -this.axis.z;
				base.transform.Rotate(eulerAngles);
			}
			if (Input.GetKey(this.incrementKey))
			{
				eulerAngles.x *= this.axis.x;
				eulerAngles.y *= this.axis.y;
				eulerAngles.z *= this.axis.z;
				base.transform.Rotate(eulerAngles);
			}
		}

		// Token: 0x04000291 RID: 657
		public float speed = 50f;

		// Token: 0x04000292 RID: 658
		public Vector3 axis = new Vector3(1f, 0f, 0f);

		// Token: 0x04000293 RID: 659
		public KeyCode decrementKey = KeyCode.KeypadMinus;

		// Token: 0x04000294 RID: 660
		public KeyCode incrementKey = KeyCode.KeypadPlus;
	}
}
