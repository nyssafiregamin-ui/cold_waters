using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AmplifyBloom
{
	// Token: 0x0200001A RID: 26
	public class DemoCameraMovement : MonoBehaviour
	{
		// Token: 0x060000F3 RID: 243 RVA: 0x000060FC File Offset: 0x000042FC
		private void Start()
		{
			this._transform = base.transform;
			this._pitch = this._transform.localEulerAngles.x;
			this._yaw = this._transform.localEulerAngles.y;
			if (Input.GetJoystickNames().Length > 0)
			{
				this.m_gamePadMode = true;
			}
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x0000615C File Offset: 0x0000435C
		private void Update()
		{
			if (this.m_gamePadMode)
			{
				this.ChangeYaw(Input.GetAxisRaw("Horizontal") * this.yawSpeed);
				this.ChangePitch(-Input.GetAxisRaw("Vertical") * this.pitchSpeed);
			}
			else if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
			{
				this.ChangeYaw(Input.GetAxisRaw("Mouse X") * this.yawSpeed);
				this.ChangePitch(-Input.GetAxisRaw("Mouse Y") * this.pitchSpeed);
			}
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x000061F4 File Offset: 0x000043F4
		private void MoveForwards(float delta)
		{
			this._transform.position += delta * this._transform.forward;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00006228 File Offset: 0x00004428
		private void Strafe(float delta)
		{
			this._transform.position += delta * this._transform.right;
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x0000625C File Offset: 0x0000445C
		private void ChangeYaw(float delta)
		{
			this._yaw += delta;
			this.WrapAngle(ref this._yaw);
			this._transform.localEulerAngles = new Vector3(this._pitch, this._yaw, 0f);
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x0000629C File Offset: 0x0000449C
		private void ChangePitch(float delta)
		{
			this._pitch += delta;
			this.WrapAngle(ref this._pitch);
			this._transform.localEulerAngles = new Vector3(this._pitch, this._yaw, 0f);
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x000062DC File Offset: 0x000044DC
		public void WrapAngle(ref float angle)
		{
			if (angle < 0f)
			{
				angle = 360f + angle;
			}
			if (angle > 360f)
			{
				angle -= 360f;
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060000FA RID: 250 RVA: 0x00006318 File Offset: 0x00004518
		public bool GamePadMode
		{
			get
			{
				return this.m_gamePadMode;
			}
		}

		// Token: 0x04000105 RID: 261
		private const string X_AXIS_KEYBOARD = "Mouse X";

		// Token: 0x04000106 RID: 262
		private const string Y_AXIS_KEYBOARD = "Mouse Y";

		// Token: 0x04000107 RID: 263
		private const string X_AXIS_GAMEPAD = "Horizontal";

		// Token: 0x04000108 RID: 264
		private const string Y_AXIS_GAMEPAD = "Vertical";

		// Token: 0x04000109 RID: 265
		private bool m_gamePadMode;

		// Token: 0x0400010A RID: 266
		public float moveSpeed = 1f;

		// Token: 0x0400010B RID: 267
		public float yawSpeed = 3f;

		// Token: 0x0400010C RID: 268
		public float pitchSpeed = 3f;

		// Token: 0x0400010D RID: 269
		private float _yaw;

		// Token: 0x0400010E RID: 270
		private float _pitch;

		// Token: 0x0400010F RID: 271
		private Transform _transform;
	}
}
