using System;
using UnityEngine;

// Token: 0x0200003C RID: 60
public class MouseLook : MonoBehaviour
{
	// Token: 0x060001C1 RID: 449 RVA: 0x0000C5A0 File Offset: 0x0000A7A0
	private void Update()
	{
		if (GUIUtility.hotControl != 0)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			this.look = true;
		}
		if (Input.GetMouseButtonUp(0))
		{
			this.look = false;
		}
		if (this.look)
		{
			if (this.axes == global::MouseLook.RotationAxes.MouseXAndY)
			{
				this.rotationX += Input.GetAxis("Mouse X") * this.sensitivityX;
				this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
				this.rotationX = global::MouseLook.ClampAngle(this.rotationX, this.minimumX, this.maximumX);
				this.rotationY = global::MouseLook.ClampAngle(this.rotationY, this.minimumY, this.maximumY);
				Quaternion rhs = Quaternion.AngleAxis(this.rotationX, Vector3.up);
				Quaternion rhs2 = Quaternion.AngleAxis(this.rotationY, Vector3.left);
				base.transform.localRotation = this.originalRotation * rhs * rhs2;
			}
			else if (this.axes == global::MouseLook.RotationAxes.MouseX)
			{
				this.rotationX += Input.GetAxis("Mouse X") * this.sensitivityX;
				this.rotationX = global::MouseLook.ClampAngle(this.rotationX, this.minimumX, this.maximumX);
				Quaternion rhs3 = Quaternion.AngleAxis(this.rotationX, Vector3.up);
				base.transform.localRotation = this.originalRotation * rhs3;
			}
			else
			{
				this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
				this.rotationY = global::MouseLook.ClampAngle(this.rotationY, this.minimumY, this.maximumY);
				Quaternion rhs4 = Quaternion.AngleAxis(this.rotationY, Vector3.left);
				base.transform.localRotation = this.originalRotation * rhs4;
			}
		}
		Vector3 vector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
		vector = base.transform.TransformDirection(vector);
		vector *= 10f;
		float num = (!Input.GetKey(KeyCode.LeftShift)) ? 50f : 150f;
		float num2 = Input.GetAxis("Vertical") * this.forwardSpeedScale * num;
		float num3 = Input.GetAxis("Horizontal") * this.strafeSpeedScale * num;
		if (num2 != 0f)
		{
			base.transform.position += base.transform.forward * num2;
		}
		if (num3 != 0f)
		{
			base.transform.position += base.transform.right * num3;
		}
	}

	// Token: 0x060001C2 RID: 450 RVA: 0x0000C874 File Offset: 0x0000AA74
	private void Start()
	{
		if (base.GetComponent<Rigidbody>())
		{
			base.GetComponent<Rigidbody>().freezeRotation = true;
		}
		this.originalRotation = base.transform.localRotation;
		this.look = false;
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x0000C8B8 File Offset: 0x0000AAB8
	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	// Token: 0x0400023E RID: 574
	public global::MouseLook.RotationAxes axes;

	// Token: 0x0400023F RID: 575
	public float sensitivityX = 3f;

	// Token: 0x04000240 RID: 576
	public float sensitivityY = 3f;

	// Token: 0x04000241 RID: 577
	public float minimumX = -360f;

	// Token: 0x04000242 RID: 578
	public float maximumX = 360f;

	// Token: 0x04000243 RID: 579
	public float minimumY = -80f;

	// Token: 0x04000244 RID: 580
	public float maximumY = 80f;

	// Token: 0x04000245 RID: 581
	public float forwardSpeedScale = 0.03f;

	// Token: 0x04000246 RID: 582
	public float strafeSpeedScale = 0.03f;

	// Token: 0x04000247 RID: 583
	private float rotationX;

	// Token: 0x04000248 RID: 584
	private float rotationY;

	// Token: 0x04000249 RID: 585
	private bool look;

	// Token: 0x0400024A RID: 586
	private Quaternion originalRotation;

	// Token: 0x0200003D RID: 61
	public enum RotationAxes
	{
		// Token: 0x0400024C RID: 588
		MouseXAndY,
		// Token: 0x0400024D RID: 589
		MouseX,
		// Token: 0x0400024E RID: 590
		MouseY
	}
}
