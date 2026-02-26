using System;
using UnityEngine;

// Token: 0x02000174 RID: 372
public class Whale_AI : MonoBehaviour
{
	// Token: 0x06000B4D RID: 2893 RVA: 0x000A7834 File Offset: 0x000A5A34
	private void Start()
	{
		if (UIFunctions.globaluifunctions.museumObject.activeSelf)
		{
			base.enabled = false;
			return;
		}
		this.parentVessel.vesselmovement.wakeObject.SetActive(false);
		this.GetWhaleWaypoint();
		this.parentVessel.isSubmarine = true;
		this.parentVessel.vesselmovement.isCruising = false;
		base.transform.Translate(-Vector3.up * UnityEngine.Random.Range(0f, 0.5f));
		VesselMovement vesselmovement = this.parentVessel.vesselmovement;
		vesselmovement.shipSpeed.x = vesselmovement.shipSpeed.x / UnityEngine.Random.Range(10f, 20f);
		base.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
		base.transform.Translate(Vector3.right * (float)UnityEngine.Random.Range(20, 50));
		base.transform.Translate(Vector3.forward * (float)UnityEngine.Random.Range(20, 50));
	}

	// Token: 0x06000B4E RID: 2894 RVA: 0x000A7954 File Offset: 0x000A5B54
	private void GetWhaleWaypoint()
	{
		this.parentVessel.vesselai.avoidingTerrain = false;
		this.parentVessel.vesselai.avoidingIceberg = false;
		this.avoiding = false;
		if (UnityEngine.Random.value < 0.6f)
		{
			this.parentVessel.acoustics.sensorNavigator.transform.localPosition = Vector3.zero;
			this.parentVessel.acoustics.sensorNavigator.transform.localRotation = Quaternion.identity;
			this.parentVessel.acoustics.sensorNavigator.transform.Translate(Vector3.forward * UnityEngine.Random.Range(15f, 30f));
			this.waypoint = this.parentVessel.acoustics.sensorNavigator.transform.position;
			this.parentVessel.acoustics.sensorNavigator.transform.localPosition = Vector3.zero;
		}
		else
		{
			this.waypoint.x = this.waypoint.x + UnityEngine.Random.Range(-30f, 30f);
			this.waypoint.z = this.waypoint.z + UnityEngine.Random.Range(-30f, 30f);
		}
		this.waypoint.x = this.waypoint.x + UnityEngine.Random.Range(-5f, 5f);
		this.waypoint.z = this.waypoint.z + UnityEngine.Random.Range(-5f, 5f);
		if (UnityEngine.Random.value < 0.3f & this.waypoint.y != 999.99f)
		{
			this.waypoint.y = 999.99f;
		}
		else
		{
			this.waypoint.y = UnityEngine.Random.Range(1000f - this.parentVessel.databaseshipdata.testDepth * GameDataManager.feetToUnits, 999.99f);
		}
		if (this.waypoint.y < base.transform.position.y - this.parentVessel.vesselai.depthUnderkeel - 0.5f)
		{
			this.waypoint.y = base.transform.position.y - this.parentVessel.vesselai.depthUnderkeel - 0.5f;
		}
	}

	// Token: 0x06000B4F RID: 2895 RVA: 0x000A7BB0 File Offset: 0x000A5DB0
	private void CheckWaypointDistance()
	{
		float num = Vector3.Distance(base.transform.position, this.waypoint);
		if (num < 6f)
		{
			this.GetWhaleWaypoint();
		}
	}

	// Token: 0x06000B50 RID: 2896 RVA: 0x000A7BE8 File Offset: 0x000A5DE8
	private void FixedUpdate()
	{
		this.whaleSongTimer -= Time.deltaTime;
		if (this.whaleSongTimer < 0f && !this.parentVessel.vesselmovement.pingSound.isPlaying)
		{
			this.parentVessel.vesselmovement.pingSound.clip = UIFunctions.globaluifunctions.database.whaleSongs[UnityEngine.Random.Range(0, UIFunctions.globaluifunctions.database.whaleSongs.Length)];
			this.parentVessel.vesselmovement.pingSound.Play();
			this.whaleSongTimer = UnityEngine.Random.Range(30f, 180f);
		}
		if (this.parentVessel.vesselmovement.shipSpeed.z < 0.4f)
		{
			VesselMovement vesselmovement = this.parentVessel.vesselmovement;
			vesselmovement.shipSpeed.x = vesselmovement.shipSpeed.x / UnityEngine.Random.Range(10f, 20f);
		}
		if (this.parentVessel.transform.eulerAngles.x > 5f && this.parentVessel.transform.eulerAngles.x < 180f)
		{
			this.waypoint.y = this.waypoint.y + this.parentVessel.transform.eulerAngles.x * Time.deltaTime * 0.05f;
			if (this.waypoint.y > 999.99f)
			{
				this.waypoint.y = 999.99f;
			}
		}
		this.parentVessel.acoustics.sensorNavigator.transform.LookAt(this.waypoint);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.parentVessel.acoustics.sensorNavigator.transform.rotation, this.parentVessel.databaseshipdata.turnrate * Time.deltaTime);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(base.transform.eulerAngles.x, base.transform.eulerAngles.y, 0f), 1f);
		this.actionTimer += Time.deltaTime;
		if (this.actionTimer > 10f)
		{
			this.CheckWaypointDistance();
			this.depthunderkeel = UIFunctions.globaluifunctions.playerfunctions.sensormanager.GetUnitsDepthUnderTransform(this.parentVessel.transform);
			this.actionTimer -= 10f;
		}
		if (!this.avoiding)
		{
			if (this.parentVessel.vesselai.avoidingIceberg)
			{
				this.parentVessel.acoustics.sensorNavigator.transform.localPosition = Vector3.zero;
				this.parentVessel.acoustics.sensorNavigator.transform.localRotation = Quaternion.identity;
				float d = 1f;
				if (UnityEngine.Random.value < 0.5f)
				{
					d = -1f;
				}
				this.parentVessel.acoustics.sensorNavigator.transform.Translate(Vector3.right * UnityEngine.Random.Range(15f, 30f) * d);
				this.waypoint.x = this.waypoint.x + UnityEngine.Random.Range(-5f, 5f);
				this.waypoint.z = this.waypoint.z + UnityEngine.Random.Range(-5f, 5f);
				this.avoiding = true;
			}
			if (this.depthunderkeel < 0.2f)
			{
				if (base.transform.position.y < 999.9f)
				{
					this.waypoint.y = 999.99f;
					return;
				}
				float num = 0f;
				float[] array = new float[2];
				LayerMask mask = 1073741824;
				float num2 = 20f;
				for (int i = -1; i < 2; i++)
				{
					for (int j = -1; j < 2; j++)
					{
						Vector3 origin = new Vector3(this.parentVessel.transform.position.x + num2 * (float)i, 1000f, this.parentVessel.transform.position.z + num2 * (float)j);
						RaycastHit raycastHit;
						if (Physics.Raycast(origin, -Vector3.up, out raycastHit, 2.66f, mask) && raycastHit.distance > num)
						{
							num = raycastHit.distance;
							array[0] = origin.x;
							array[1] = origin.z;
						}
					}
				}
				this.waypoint = new Vector3(array[0], base.transform.position.y, array[1]);
				this.avoiding = true;
			}
		}
	}

	// Token: 0x0400125F RID: 4703
	public Vessel parentVessel;

	// Token: 0x04001260 RID: 4704
	public Vector3 waypoint;

	// Token: 0x04001261 RID: 4705
	public float actionTimer;

	// Token: 0x04001262 RID: 4706
	public bool avoiding;

	// Token: 0x04001263 RID: 4707
	public float depthunderkeel;

	// Token: 0x04001264 RID: 4708
	public float whaleSongTimer;
}
