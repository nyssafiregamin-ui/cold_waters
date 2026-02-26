using System;
using UnityEngine;

// Token: 0x0200011A RID: 282
public class EventCamera : MonoBehaviour
{
	// Token: 0x06000795 RID: 1941 RVA: 0x00046B10 File Offset: 0x00044D10
	private void FixedUpdate()
	{
		this.eventCameraTimer -= Time.deltaTime;
		if (this.eventCameraTimer <= 0f)
		{
			base.enabled = false;
		}
	}

	// Token: 0x06000796 RID: 1942 RVA: 0x00046B3C File Offset: 0x00044D3C
	public void CheckForEventCamera(Transform newCameraTarget, Transform lookAtTransform = null, float eventTime = 10f, bool surface = true, bool fixedPosition = false, bool CameraReturns = true, float distance = -1f, float minDistance = -1f, float sideangle = -1f, bool checkDistance = true)
	{
		if (!GameDataManager.optionsBoolSettings[18] || !this.eventCameraOn)
		{
			return;
		}
		if (this.eventCameraTimer > 0f)
		{
			return;
		}
		if (ManualCameraZoom.binoculars && !this.eventIfInBinoculars)
		{
			return;
		}
		if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.background.activeSelf && !this.eventIfInTacticalMap)
		{
			return;
		}
		if (PlayerFunctions.draggingWaypoint && !this.eventIfDraggingWaypoint)
		{
			return;
		}
		for (int i = 0; i < UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects.Length; i++)
		{
			if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[i].targetTransform == GameDataManager.playervesselsonlevel[0].transform && !UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[i].guidanceActive)
			{
				return;
			}
		}
		if (checkDistance && Vector3.Distance(UIFunctions.globaluifunctions.MainCamera.transform.position, newCameraTarget.position) < this.minDistanceToMove)
		{
			return;
		}
		if (UIFunctions.globaluifunctions.playerfunctions.timeCompressionToggle == 1)
		{
			UIFunctions.globaluifunctions.playerfunctions.SetTimeCompression();
		}
		this.cameraReturned = false;
		base.enabled = true;
		this.eventCameraTimer = eventTime;
		ManualCameraZoom.target = newCameraTarget;
		ManualCameraZoom.distance = 8f;
		if (lookAtTransform != null)
		{
			UIFunctions.globaluifunctions.directionFinder.transform.position = newCameraTarget.transform.position;
			UIFunctions.globaluifunctions.directionFinder.transform.LookAt(lookAtTransform.position);
			ManualCameraZoom.x = UIFunctions.globaluifunctions.directionFinder.transform.eulerAngles.y;
			if (sideangle != -1f)
			{
				ManualCameraZoom.x += sideangle;
				if (UnityEngine.Random.value < 0.5f)
				{
					ManualCameraZoom.x -= sideangle * 2f;
				}
			}
		}
		if (distance != -1f)
		{
			ManualCameraZoom.distance = distance;
			ManualCameraZoom.minDistance = minDistance;
		}
		this.CheckSeaLevelSwap(surface);
		if (fixedPosition)
		{
			UIFunctions.globaluifunctions.playerfunctions.FreezeCamera();
		}
		UIFunctions.globaluifunctions.skyobjectcenterer.ForceSkyboxPosition(newCameraTarget);
	}

	// Token: 0x06000797 RID: 1943 RVA: 0x00046DAC File Offset: 0x00044FAC
	public void CheckSeaLevelSwap(bool surface)
	{
		if (surface && ManualCameraZoom.underwater)
		{
			UIFunctions.globaluifunctions.levelloadmanager.EnvironmentSwitch(false);
			this.manualcamerazoom.transform.position = new Vector3(this.manualcamerazoom.transform.position.x, 1000.1f, this.manualcamerazoom.transform.position.z);
			ManualCameraZoom.y = 5f;
		}
		else if (!surface && !ManualCameraZoom.underwater)
		{
			UIFunctions.globaluifunctions.levelloadmanager.EnvironmentSwitch(true);
			this.manualcamerazoom.transform.position = new Vector3(this.manualcamerazoom.transform.position.x, 999.9f, this.manualcamerazoom.transform.position.z);
			ManualCameraZoom.y = -5f;
		}
	}

	// Token: 0x06000798 RID: 1944 RVA: 0x00046EA8 File Offset: 0x000450A8
	private void SetCameraReturnParameters()
	{
		this.lastCameraTransform = ManualCameraZoom.target;
		this.lastCameraXY = new Vector2(ManualCameraZoom.x, ManualCameraZoom.y);
		this.lastCameraDistance = ManualCameraZoom.distance;
		this.lastCameraMinDistance = ManualCameraZoom.minDistance;
	}

	// Token: 0x06000799 RID: 1945 RVA: 0x00046EEC File Offset: 0x000450EC
	private void ApplyCameraReturnParameters()
	{
	}

	// Token: 0x04000A8C RID: 2700
	public ManualCameraZoom manualcamerazoom;

	// Token: 0x04000A8D RID: 2701
	private Transform lastCameraTransform;

	// Token: 0x04000A8E RID: 2702
	private Vector2 lastCameraXY;

	// Token: 0x04000A8F RID: 2703
	private float lastCameraDistance;

	// Token: 0x04000A90 RID: 2704
	private float lastCameraMinDistance;

	// Token: 0x04000A91 RID: 2705
	private float minDistanceToMove = 10f;

	// Token: 0x04000A92 RID: 2706
	public bool eventCameraOn = true;

	// Token: 0x04000A93 RID: 2707
	private bool returnCamera;

	// Token: 0x04000A94 RID: 2708
	private bool cameraReturned;

	// Token: 0x04000A95 RID: 2709
	private float minTimeBetweenEvents = 5f;

	// Token: 0x04000A96 RID: 2710
	public float eventCameraTimer;

	// Token: 0x04000A97 RID: 2711
	private bool eventIfInBinoculars;

	// Token: 0x04000A98 RID: 2712
	private bool eventIfInTacticalMap = true;

	// Token: 0x04000A99 RID: 2713
	private bool eventIfDraggingWaypoint;
}
