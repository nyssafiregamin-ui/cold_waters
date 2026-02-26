using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x020000CD RID: 205
	public class ShipCamera : MonoBehaviour
	{
		// Token: 0x17000137 RID: 311
		// (get) Token: 0x060005C0 RID: 1472 RVA: 0x00026CDC File Offset: 0x00024EDC
		private GameObject Ship
		{
			get
			{
				if (this.m_ship == null)
				{
					this.m_dummy = new GameObject();
					this.m_ship = this.m_dummy;
				}
				return this.m_ship;
			}
		}

		// Token: 0x060005C1 RID: 1473 RVA: 0x00026D18 File Offset: 0x00024F18
		private void Start()
		{
			this.m_position.camRotation.x = this.m_camStartRotationX;
			this.m_position.camRotation.y = this.m_camStartRotationY;
			this.m_position.camDistance = this.m_camStartDistance;
			this.m_position.forwardAmount = Vector3.zero;
			this.m_target = this.m_position;
			this.m_previousPos = this.Ship.transform.position;
		}

		// Token: 0x060005C2 RID: 1474 RVA: 0x00026D94 File Offset: 0x00024F94
		private void LateUpdate()
		{
			this.ProcessInput();
			this.InterpolateToTarget();
			this.MoveShip();
		}

		// Token: 0x060005C3 RID: 1475 RVA: 0x00026DA8 File Offset: 0x00024FA8
		private void OnDestroy()
		{
			if (this.m_dummy != null)
			{
				UnityEngine.Object.DestroyImmediate(this.m_dummy);
			}
		}

		// Token: 0x060005C4 RID: 1476 RVA: 0x00026DC8 File Offset: 0x00024FC8
		private void MoveShip()
		{
			this.Ship.transform.position += this.m_position.forwardAmount;
			Vector3 eulerAngles = this.Ship.transform.eulerAngles;
			eulerAngles.y += this.m_position.turnAmount;
			this.Ship.transform.eulerAngles = eulerAngles;
			float y = Mathf.Cos(this.m_position.camRotation.y * 0.017453292f);
			float num = Mathf.Sin(this.m_position.camRotation.y * 0.017453292f);
			float num2 = Mathf.Cos(this.m_position.camRotation.x * 0.017453292f);
			float num3 = Mathf.Sin(this.m_position.camRotation.x * 0.017453292f);
			Vector3 position = this.Ship.transform.position;
			Vector3 position2 = position + new Vector3(num3 * num, y, num2 * num) * this.m_position.camDistance;
			base.transform.position = position2;
			base.transform.LookAt(position);
			this.m_velocity = this.Ship.transform.position - this.m_previousPos;
			this.m_previousPos = this.Ship.transform.position;
		}

		// Token: 0x060005C5 RID: 1477 RVA: 0x00026F30 File Offset: 0x00025130
		private void InterpolateToTarget()
		{
			if (this.disableInterpolation || Time.timeScale == 0f)
			{
				this.m_position = this.m_target;
				return;
			}
			float num = 1f / Mathf.Clamp(this.camSmoothness, 0.01f, 1f);
			float t = Mathf.Clamp01(Time.deltaTime * num);
			num = 1f / Mathf.Clamp(this.shipSmoothness, 0.01f, 1f);
			float t2 = Mathf.Clamp01(Time.deltaTime * num);
			this.m_position.camDistance = Mathf.Lerp(this.m_position.camDistance, this.m_target.camDistance, t);
			this.m_position.camRotation = Vector2.Lerp(this.m_position.camRotation, this.m_target.camRotation, t);
			this.m_position.forwardAmount = Vector3.Lerp(this.m_position.forwardAmount, this.m_target.forwardAmount, t2);
			this.m_position.turnAmount = Mathf.Lerp(this.m_position.turnAmount, this.m_target.turnAmount, t2);
		}

		// Token: 0x060005C6 RID: 1478 RVA: 0x00027054 File Offset: 0x00025254
		private void ProcessInput()
		{
			float shipMoveSpeed = this.m_shipMoveSpeed;
			float magnitude = this.m_velocity.magnitude;
			this.m_target.forwardAmount = Vector3.zero;
			this.m_target.turnAmount = 0f;
			if (Input.GetKey(KeyCode.A))
			{
				float num = shipMoveSpeed * magnitude;
				this.m_target.turnAmount = this.m_target.turnAmount - num * Time.deltaTime;
				this.m_target.camRotation.x = this.m_target.camRotation.x - num * Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.D))
			{
				float num2 = shipMoveSpeed * magnitude;
				this.m_target.turnAmount = this.m_target.turnAmount + num2 * Time.deltaTime;
				this.m_target.camRotation.x = this.m_target.camRotation.x + num2 * Time.deltaTime;
			}
			Vector3 a = this.Ship.transform.localToWorldMatrix * this.m_forward;
			a.Normalize();
			if (Input.GetKey(KeyCode.W))
			{
				this.m_acceleration += Time.deltaTime * 1f;
			}
			else
			{
				this.m_acceleration -= Time.deltaTime * 0.25f;
			}
			this.m_acceleration = Mathf.Clamp(this.m_acceleration, 0f, 1f);
			this.m_target.forwardAmount = this.m_target.forwardAmount + a * shipMoveSpeed * this.m_acceleration * Time.deltaTime;
			float a2 = Time.deltaTime * 1000f;
			float num3 = Mathf.Pow(1.02f, Mathf.Min(a2, 1f));
			if (Input.GetAxis("Mouse ScrollWheel") < 0f)
			{
				this.m_target.camDistance = this.m_target.camDistance * num3;
			}
			else if (Input.GetAxis("Mouse ScrollWheel") > 0f)
			{
				this.m_target.camDistance = this.m_target.camDistance / num3;
			}
			this.m_target.camDistance = Mathf.Max(1f, this.m_target.camDistance);
			this.m_target.camRotation.y = Mathf.Clamp(this.m_target.camRotation.y, 20f, 160f);
			if (Input.GetMouseButton(0))
			{
				this.m_target.camRotation.y = this.m_target.camRotation.y + Input.GetAxis("Mouse Y") * this.m_camRotationSpeed;
				this.m_target.camRotation.x = this.m_target.camRotation.x + Input.GetAxis("Mouse X") * this.m_camRotationSpeed;
			}
		}

		// Token: 0x040005B9 RID: 1465
		private const float MAX_ACCELERATION = 1f;

		// Token: 0x040005BA RID: 1466
		private const float ACCELERATION_RATE = 1f;

		// Token: 0x040005BB RID: 1467
		private const float DECELERATION_RATE = 0.25f;

		// Token: 0x040005BC RID: 1468
		public bool disableInterpolation;

		// Token: 0x040005BD RID: 1469
		public GameObject m_ship;

		// Token: 0x040005BE RID: 1470
		public Vector3 m_forward = new Vector3(0f, 0f, 1f);

		// Token: 0x040005BF RID: 1471
		public float m_shipMoveSpeed = 20f;

		// Token: 0x040005C0 RID: 1472
		[Range(0.01f, 1f)]
		public float shipSmoothness = 0.5f;

		// Token: 0x040005C1 RID: 1473
		public float m_camRotationSpeed = 10f;

		// Token: 0x040005C2 RID: 1474
		public float m_camStartRotationX = 180f;

		// Token: 0x040005C3 RID: 1475
		public float m_camStartRotationY = 60f;

		// Token: 0x040005C4 RID: 1476
		public float m_camStartDistance = 100f;

		// Token: 0x040005C5 RID: 1477
		[Range(0.01f, 1f)]
		public float camSmoothness = 0.5f;

		// Token: 0x040005C6 RID: 1478
		private ShipCamera.Position m_position;

		// Token: 0x040005C7 RID: 1479
		private ShipCamera.Position m_target;

		// Token: 0x040005C8 RID: 1480
		private float m_acceleration;

		// Token: 0x040005C9 RID: 1481
		private Vector3 m_previousPos;

		// Token: 0x040005CA RID: 1482
		private Vector3 m_velocity;

		// Token: 0x040005CB RID: 1483
		private GameObject m_dummy;

		// Token: 0x020000CE RID: 206
		private struct Position
		{
			// Token: 0x040005CC RID: 1484
			public Vector3 forwardAmount;

			// Token: 0x040005CD RID: 1485
			public float turnAmount;

			// Token: 0x040005CE RID: 1486
			public Vector2 camRotation;

			// Token: 0x040005CF RID: 1487
			public float camDistance;
		}
	}
}
