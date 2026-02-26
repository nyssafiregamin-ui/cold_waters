using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x020000C8 RID: 200
	public class FreeCam : MonoBehaviour
	{
		// Token: 0x060005AF RID: 1455 RVA: 0x00025E88 File Offset: 0x00024088
		private void Start()
		{
			base.transform.localEulerAngles = new Vector3(-this.rotationY, base.transform.localEulerAngles.y, 0f);
		}

		// Token: 0x060005B0 RID: 1456 RVA: 0x00025EC4 File Offset: 0x000240C4
		private void OnGUI()
		{
			if (Event.current == null)
			{
				return;
			}
			if (Event.current.isMouse)
			{
				this.m_takeMouseInput = true;
			}
			else
			{
				this.m_takeMouseInput = false;
			}
		}

		// Token: 0x060005B1 RID: 1457 RVA: 0x00025EF4 File Offset: 0x000240F4
		private void Update()
		{
			float num = this.m_speed;
			if (Input.GetKey(KeyCode.Space))
			{
				num *= 10f;
			}
			Vector3 translation = new Vector3(0f, 0f, 0f);
			if (Input.GetKey(KeyCode.A))
			{
				translation = new Vector3(-1f, 0f, 0f) * Time.deltaTime * num;
			}
			if (Input.GetKey(KeyCode.D))
			{
				translation = new Vector3(1f, 0f, 0f) * Time.deltaTime * num;
			}
			if (Input.GetKey(KeyCode.W))
			{
				translation = new Vector3(0f, 0f, 1f) * Time.deltaTime * num;
			}
			if (Input.GetKey(KeyCode.S))
			{
				translation = new Vector3(0f, 0f, -1f) * Time.deltaTime * num;
			}
			if (Input.GetKey(KeyCode.E))
			{
				translation = new Vector3(0f, -1f, 0f) * Time.deltaTime * num;
			}
			if (Input.GetKey(KeyCode.Q))
			{
				translation = new Vector3(0f, 1f, 0f) * Time.deltaTime * num;
			}
			base.transform.Translate(translation);
			if (this.m_takeMouseInput)
			{
				if (this.axes == FreeCam.RotationAxes.MouseXAndY)
				{
					float y = base.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * this.sensitivityX;
					this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
					this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY);
					base.transform.localEulerAngles = new Vector3(-this.rotationY, y, 0f);
				}
				else if (this.axes == FreeCam.RotationAxes.MouseX)
				{
					base.transform.Rotate(0f, Input.GetAxis("Mouse X") * this.sensitivityX, 0f);
				}
				else
				{
					this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
					this.rotationY = Mathf.Clamp(this.rotationY, this.minimumY, this.maximumY);
					base.transform.localEulerAngles = new Vector3(-this.rotationY, base.transform.localEulerAngles.y, 0f);
				}
			}
		}

		// Token: 0x04000598 RID: 1432
		public float m_speed = 50f;

		// Token: 0x04000599 RID: 1433
		public FreeCam.RotationAxes axes;

		// Token: 0x0400059A RID: 1434
		public float sensitivityX = 15f;

		// Token: 0x0400059B RID: 1435
		public float sensitivityY = 15f;

		// Token: 0x0400059C RID: 1436
		public float minimumX = -360f;

		// Token: 0x0400059D RID: 1437
		public float maximumX = 360f;

		// Token: 0x0400059E RID: 1438
		public float minimumY = -60f;

		// Token: 0x0400059F RID: 1439
		public float maximumY = 60f;

		// Token: 0x040005A0 RID: 1440
		public float rotationY;

		// Token: 0x040005A1 RID: 1441
		private bool m_takeMouseInput = true;

		// Token: 0x020000C9 RID: 201
		public enum RotationAxes
		{
			// Token: 0x040005A3 RID: 1443
			MouseXAndY,
			// Token: 0x040005A4 RID: 1444
			MouseX,
			// Token: 0x040005A5 RID: 1445
			MouseY
		}
	}
}
