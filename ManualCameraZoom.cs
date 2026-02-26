using System;
using UnityEngine;

// Token: 0x02000134 RID: 308
[AddComponentMenu("Camera-Control/Keyboard Orbit")]
public class ManualCameraZoom : MonoBehaviour
{
	// Token: 0x0600086B RID: 2155 RVA: 0x0005D230 File Offset: 0x0005B430
	public void Start()
	{
		ManualCameraZoom.cameraDummyTransform = this.dummyTransform;
		Vector3 eulerAngles = base.transform.eulerAngles;
	}

	// Token: 0x0600086C RID: 2156 RVA: 0x0005D254 File Offset: 0x0005B454
	private void Update()
	{
		if (Input.anyKeyDown)
		{
			if (InputManager.globalInputManager.GetButtonDown("Cancel or Quit", false))
			{
				if (UIFunctions.globaluifunctions.helpmanager.gameObject.activeSelf)
				{
					UIFunctions.globaluifunctions.helpmanager.gameObject.SetActive(false);
					return;
				}
				if (LevelLoadManager.inMuseum)
				{
					return;
				}
				if (PlayerFunctions.draggingWaypoint)
				{
					UIFunctions.globaluifunctions.playerfunctions.DisableWaypointDragging();
					return;
				}
				if (UIFunctions.globaluifunctions.playerfunctions.helmmanager.draggingNavWaypoint)
				{
					UIFunctions.globaluifunctions.playerfunctions.helmmanager.DisableWaypointDragging();
					return;
				}
				if (Time.timeScale == 0f)
				{
					return;
				}
				if (ManualCameraZoom.binoculars)
				{
					GameDataManager.playervesselsonlevel[0].submarineFunctions.LeavePeriscopeView();
				}
				else if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.background.activeSelf)
				{
					UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.SetTacticalMap();
				}
				else
				{
					UIFunctions.globaluifunctions.missionmanager.BringInExitMenu(false);
				}
				return;
			}
			else if (InputManager.globalInputManager.GetButtonDown("Next Contact", false))
			{
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.SelectTarget();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Camera To Player", false))
			{
				if (GameDataManager.playervesselsonlevel != null && GameDataManager.playervesselsonlevel.Length != 0 && GameDataManager.playervesselsonlevel[0] != null)
				{
					UIFunctions.globaluifunctions.playerfunctions.eventcamera.enabled = false;
					UIFunctions.globaluifunctions.CloseCombatScreens();
					UIFunctions.globaluifunctions.skyobjectcenterer.ForceSkyboxPosition(GameDataManager.playervesselsonlevel[0].transform);
					ManualCameraZoom.minDistance = GameDataManager.playervesselsonlevel[0].databaseshipdata.minCameraDistance;
					UIFunctions.globaluifunctions.playerfunctions.sensormanager.FixedUpdate();
				}
			}
			else if (InputManager.globalInputManager.GetButtonDown("Camera To Selected Contact", false))
			{
				UIFunctions.globaluifunctions.playerfunctions.eventcamera.enabled = false;
				int currentTargetIndex = UIFunctions.globaluifunctions.playerfunctions.currentTargetIndex;
				if (currentTargetIndex != -1)
				{
					UIFunctions.globaluifunctions.CloseCombatScreens();
					UIFunctions.globaluifunctions.skyobjectcenterer.ForceSkyboxPosition(GameDataManager.enemyvesselsonlevel[currentTargetIndex].transform);
					ManualCameraZoom.minDistance = GameDataManager.enemyvesselsonlevel[currentTargetIndex].databaseshipdata.minCameraDistance;
					UIFunctions.globaluifunctions.playerfunctions.sensormanager.FixedUpdate();
				}
			}
			else if (InputManager.globalInputManager.GetButtonDown("Still Camera", false))
			{
				UIFunctions.globaluifunctions.playerfunctions.eventcamera.enabled = false;
				UIFunctions.globaluifunctions.playerfunctions.FreezeCamera();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Event Camera", false))
			{
				UIFunctions.globaluifunctions.playerfunctions.SetEventCameraMode();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Camera To Weapon", false))
			{
				UIFunctions.globaluifunctions.playerfunctions.eventcamera.enabled = false;
				if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects.Length == 0)
				{
					return;
				}
				UIFunctions.globaluifunctions.playerfunctions.currentWeaponIndex++;
				UIFunctions.globaluifunctions.CloseCombatScreens();
				if (UIFunctions.globaluifunctions.playerfunctions.currentWeaponIndex < UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects.Length)
				{
					UIFunctions.globaluifunctions.skyobjectcenterer.ForceSkyboxPosition(UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[UIFunctions.globaluifunctions.playerfunctions.currentWeaponIndex].transform);
					ManualCameraZoom.minDistance = UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[UIFunctions.globaluifunctions.playerfunctions.currentWeaponIndex].databaseweapondata.minCameraDistance;
				}
				else
				{
					UIFunctions.globaluifunctions.skyobjectcenterer.ForceSkyboxPosition(UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[0].transform);
					UIFunctions.globaluifunctions.playerfunctions.currentWeaponIndex = 0;
					ManualCameraZoom.minDistance = UIFunctions.globaluifunctions.playerfunctions.sensormanager.torpedoObjects[UIFunctions.globaluifunctions.playerfunctions.currentWeaponIndex].databaseweapondata.minCameraDistance;
				}
			}
			else if (InputManager.globalInputManager.GetButtonDown("Camera To Aircraft", false))
			{
				UIFunctions.globaluifunctions.playerfunctions.eventcamera.enabled = false;
				if (UIFunctions.globaluifunctions.combatai.enemyHelicopters.Length + UIFunctions.globaluifunctions.combatai.enemyAircraft.Length == 0)
				{
					return;
				}
				int num = UIFunctions.globaluifunctions.playerfunctions.currentAircraftIndex + 1;
				UIFunctions.globaluifunctions.playerfunctions.currentAircraftIndex = num;
				UIFunctions.globaluifunctions.CloseCombatScreens();
				if (num < UIFunctions.globaluifunctions.combatai.enemyHelicopters.Length && UIFunctions.globaluifunctions.combatai.enemyHelicopters.Length > 0)
				{
					UIFunctions.globaluifunctions.skyobjectcenterer.ForceSkyboxPosition(UIFunctions.globaluifunctions.combatai.enemyHelicopters[num].transform);
					ManualCameraZoom.minDistance = UIFunctions.globaluifunctions.combatai.enemyHelicopters[UIFunctions.globaluifunctions.playerfunctions.currentAircraftIndex].databaseaircraftdata.minCameraDistance;
				}
				else if (num - UIFunctions.globaluifunctions.combatai.enemyHelicopters.Length < UIFunctions.globaluifunctions.combatai.enemyAircraft.Length && UIFunctions.globaluifunctions.combatai.enemyAircraft.Length > 0)
				{
					UIFunctions.globaluifunctions.skyobjectcenterer.ForceSkyboxPosition(UIFunctions.globaluifunctions.combatai.enemyAircraft[num - UIFunctions.globaluifunctions.combatai.enemyHelicopters.Length].transform);
					ManualCameraZoom.minDistance = UIFunctions.globaluifunctions.combatai.enemyAircraft[num - UIFunctions.globaluifunctions.combatai.enemyHelicopters.Length].databaseaircraftdata.minCameraDistance;
				}
				else
				{
					if (UIFunctions.globaluifunctions.combatai.enemyHelicopters.Length > 0)
					{
						UIFunctions.globaluifunctions.skyobjectcenterer.ForceSkyboxPosition(UIFunctions.globaluifunctions.combatai.enemyHelicopters[0].transform);
						ManualCameraZoom.minDistance = UIFunctions.globaluifunctions.combatai.enemyHelicopters[0].databaseaircraftdata.minCameraDistance;
					}
					else
					{
						UIFunctions.globaluifunctions.skyobjectcenterer.ForceSkyboxPosition(UIFunctions.globaluifunctions.combatai.enemyAircraft[0].transform);
						ManualCameraZoom.minDistance = UIFunctions.globaluifunctions.combatai.enemyAircraft[0].databaseaircraftdata.minCameraDistance;
					}
					UIFunctions.globaluifunctions.playerfunctions.currentAircraftIndex = 0;
				}
			}
			else if (InputManager.globalInputManager.GetButtonDown("Camera To Scenery", false))
			{
				UIFunctions.globaluifunctions.playerfunctions.eventcamera.enabled = false;
				UIFunctions.globaluifunctions.playerfunctions.currentSceneryIndex++;
				UIFunctions.globaluifunctions.CloseCombatScreens();
				if (UIFunctions.globaluifunctions.playerfunctions.currentSceneryIndex < UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.missileHitLocations.Length)
				{
					UIFunctions.globaluifunctions.skyobjectcenterer.ForceSkyboxPosition(UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.missileHitLocations[UIFunctions.globaluifunctions.playerfunctions.currentSceneryIndex]);
					ManualCameraZoom.minDistance = 3f;
				}
				else
				{
					UIFunctions.globaluifunctions.skyobjectcenterer.ForceSkyboxPosition(UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.missileHitLocations[0].transform);
					UIFunctions.globaluifunctions.playerfunctions.currentSceneryIndex = 0;
					ManualCameraZoom.minDistance = 3f;
				}
			}
		}
	}

	// Token: 0x0600086D RID: 2157 RVA: 0x0005DA34 File Offset: 0x0005BC34
	public void LateUpdate()
	{
		if (!LevelLoadManager.inMuseum && UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.background.activeSelf)
		{
			return;
		}
		this.originalY = ManualCameraZoom.y;
		this.originalDistance = ManualCameraZoom.distance;
		bool flag = true;
		if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.background.activeSelf)
		{
			flag = false;
		}
		float num = 0f;
		if (!LevelLoadManager.inMuseum)
		{
			num = Input.GetAxis("Mouse ScrollWheel");
			if (ManualCameraZoom.binoculars)
			{
				if (InputManager.globalInputManager.GetButtonDown("Zoom In", false))
				{
					num = 1f;
				}
				if (InputManager.globalInputManager.GetButtonDown("Zoom Out", false))
				{
					num = -1f;
				}
			}
			if (num != 0f && UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.IsMouseOverTacMiniMap())
			{
				flag = false;
			}
		}
		else if (Input.mousePosition.x > ManualCameraZoom.museumThreshold.x)
		{
			num = Input.GetAxis("Mouse ScrollWheel");
		}
		Vector2 zero = Vector2.zero;
		float num2 = Time.deltaTime * GameDataManager.cameraTimeScale;
		if (Input.anyKey || Input.anyKeyDown || num != 0f || this.currentDampeningValues != Vector2.zero)
		{
			if (InputManager.globalInputManager.GetButtonDown("Camera Left", true))
			{
				zero.x = -1f;
			}
			else if (InputManager.globalInputManager.GetButtonDown("Camera Right", true))
			{
				zero.x = 1f;
			}
			if (InputManager.globalInputManager.GetButtonDown("Camera Up", true))
			{
				zero.y = 1f;
			}
			else if (InputManager.globalInputManager.GetButtonDown("Camera Down", true))
			{
				zero.y = -1f;
			}
			this.currentDampeningValues.x = this.currentDampeningValues.x + zero.x * this.dampeningRate * Time.deltaTime;
			this.currentDampeningValues.y = this.currentDampeningValues.y + zero.y * this.dampeningRate * Time.deltaTime;
			this.currentDampeningValues.x = Mathf.Clamp(this.currentDampeningValues.x, -1f, 1f);
			this.currentDampeningValues.y = Mathf.Clamp(this.currentDampeningValues.y, -1f, 1f);
			if (ManualCameraZoom.binoculars)
			{
				ManualCameraZoom.x += this.currentDampeningValues.x * this.xSpeed * (Camera.main.fieldOfView / 30f) * num2;
			}
			else
			{
				ManualCameraZoom.x -= this.currentDampeningValues.x * this.xSpeed * num2;
				ManualCameraZoom.y += this.currentDampeningValues.y * this.ySpeed * num2;
				if (LevelLoadManager.inMuseum)
				{
					flag = true;
				}
				if (flag)
				{
					if (InputManager.globalInputManager.GetButtonDown("Zoom In", true))
					{
						if (!UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.IsMouseOverTacMiniMap())
						{
							ManualCameraZoom.distance -= 4f * num2;
						}
					}
					else if (InputManager.globalInputManager.GetButtonDown("Zoom Out", true) && !UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.IsMouseOverTacMiniMap())
					{
						ManualCameraZoom.distance += 4f * num2;
					}
					if (num > 0f)
					{
						ManualCameraZoom.distance -= 80f * num2 * ManualCameraZoom.sensitivity;
					}
					else if (num < 0f)
					{
						ManualCameraZoom.distance += 80f * num2 * ManualCameraZoom.sensitivity;
					}
				}
			}
			if (InputManager.globalInputManager.GetButtonDown("Drag Camera", true))
			{
				this.currentPosition = Input.mousePosition;
				this.deltaPosition = this.currentPosition - this.lastPosition;
				this.lastPosition = this.currentPosition;
				if (ManualCameraZoom.binoculars)
				{
					ManualCameraZoom.x -= Input.GetAxis("Mouse X") * (4f * (ManualCameraZoom.sensitivity * ManualCameraZoom.sensitivity * 0.7f + 0.3f)) * (Camera.main.fieldOfView / 30f);
				}
				else
				{
					ManualCameraZoom.x += Input.GetAxis("Mouse X") * (4f * (ManualCameraZoom.sensitivity * ManualCameraZoom.sensitivity * 2.8f + 0.3f));
					ManualCameraZoom.y -= Input.GetAxis("Mouse Y") * (4f * (ManualCameraZoom.sensitivity * ManualCameraZoom.sensitivity * 2.8f + 0.3f)) * ManualCameraZoom.invertMouse;
				}
			}
		}
		if (Time.timeScale != 1f)
		{
			this.currentDampeningValues = Vector2.zero;
		}
		if (ManualCameraZoom.cameraMount)
		{
			Quaternion rotation = Quaternion.Euler(0f, ManualCameraZoom.x, 0f);
			ManualCameraZoom.cameraMount.transform.rotation = rotation;
			if (zero.x == 0f && this.currentDampeningValues.x != 0f && this.currentDampeningValues.x != 0f)
			{
				if (this.currentDampeningValues.x > 0f)
				{
					this.currentDampeningValues.x = this.currentDampeningValues.x - this.dampeningRate * num2;
				}
				else
				{
					this.currentDampeningValues.x = this.currentDampeningValues.x + this.dampeningRate * num2;
				}
				if (Mathf.Abs(this.currentDampeningValues.x) < 0.05f)
				{
					this.currentDampeningValues.x = 0f;
				}
			}
			return;
		}
		if (zero.x == 0f && this.currentDampeningValues.x != 0f && this.currentDampeningValues.x != 0f)
		{
			if (this.currentDampeningValues.x > 0f)
			{
				this.currentDampeningValues.x = this.currentDampeningValues.x - this.dampeningRate * num2;
			}
			else
			{
				this.currentDampeningValues.x = this.currentDampeningValues.x + this.dampeningRate * num2;
			}
			if (Mathf.Abs(this.currentDampeningValues.x) < 0.05f)
			{
				this.currentDampeningValues.x = 0f;
			}
		}
		if (zero.y == 0f && this.currentDampeningValues.y != 0f && this.currentDampeningValues.y != 0f)
		{
			if (this.currentDampeningValues.y > 0f)
			{
				this.currentDampeningValues.y = this.currentDampeningValues.y - this.dampeningRate * num2;
			}
			else
			{
				this.currentDampeningValues.y = this.currentDampeningValues.y + this.dampeningRate * num2;
			}
			if (Mathf.Abs(this.currentDampeningValues.y) < 0.05f)
			{
				this.currentDampeningValues.y = 0f;
			}
		}
		if (ManualCameraZoom.target)
		{
			ManualCameraZoom.relativeLocation = ManualCameraZoom.target.position;
			if (!ManualCameraZoom.underwater && ManualCameraZoom.target.position.y < this.surfaceLevel)
			{
				ManualCameraZoom.relativeLocation = new Vector3(ManualCameraZoom.target.position.x, this.surfaceLevel, ManualCameraZoom.target.position.z);
			}
			else if (ManualCameraZoom.underwater && ManualCameraZoom.target.position.y > this.underwaterLevel)
			{
				ManualCameraZoom.relativeLocation = new Vector3(ManualCameraZoom.target.position.x, this.underwaterLevel, ManualCameraZoom.target.position.z);
			}
			ManualCameraZoom.y = ManualCameraZoom.ClampAngle(ManualCameraZoom.y, (float)ManualCameraZoom.yMinLimit, (float)this.yMaxLimit);
			if (base.transform.position.y <= this.surfaceLevel && !ManualCameraZoom.underwater)
			{
				this.levelloadmanager.EnvironmentSwitch(true);
			}
			else if (base.transform.position.y >= this.underwaterLevel && ManualCameraZoom.underwater)
			{
				this.levelloadmanager.EnvironmentSwitch(false);
			}
			if (ManualCameraZoom.distance < ManualCameraZoom.minDistance)
			{
				ManualCameraZoom.distance = ManualCameraZoom.minDistance;
			}
			else if (ManualCameraZoom.distance > 15f)
			{
				ManualCameraZoom.distance = 15f;
			}
			Quaternion rotation2 = Quaternion.Euler(ManualCameraZoom.y, ManualCameraZoom.x, 0f);
			Vector3 position = rotation2 * new Vector3(0f, 0f, -ManualCameraZoom.distance) + ManualCameraZoom.relativeLocation;
			base.transform.rotation = rotation2;
			base.transform.position = position;
			if (UIFunctions.globaluifunctions.optionsmanager.cameraParams != Vector3.zero)
			{
				ManualCameraZoom.x += UIFunctions.globaluifunctions.optionsmanager.cameraParams.x * Time.deltaTime;
				ManualCameraZoom.y += UIFunctions.globaluifunctions.optionsmanager.cameraParams.y * Time.deltaTime;
				ManualCameraZoom.distance += UIFunctions.globaluifunctions.optionsmanager.cameraParams.z * Time.deltaTime;
			}
			if (ManualCameraZoom.target == ManualCameraZoom.cameraDummyTransform && UIFunctions.globaluifunctions.optionsmanager.cameraMoveParams != Vector3.zero)
			{
				ManualCameraZoom.cameraDummyTransform.transform.Translate(Vector3.right * UIFunctions.globaluifunctions.optionsmanager.cameraMoveParams.x * Time.deltaTime);
				ManualCameraZoom.cameraDummyTransform.transform.Translate(Vector3.up * UIFunctions.globaluifunctions.optionsmanager.cameraMoveParams.y * Time.deltaTime);
				ManualCameraZoom.cameraDummyTransform.transform.Translate(Vector3.forward * UIFunctions.globaluifunctions.optionsmanager.cameraMoveParams.z * Time.deltaTime);
			}
		}
	}

	// Token: 0x0600086E RID: 2158 RVA: 0x0005E4FC File Offset: 0x0005C6FC
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

	// Token: 0x04000CE4 RID: 3300
	public LevelLoadManager levelloadmanager;

	// Token: 0x04000CE5 RID: 3301
	public static Transform target;

	// Token: 0x04000CE6 RID: 3302
	public static Transform cameraMount;

	// Token: 0x04000CE7 RID: 3303
	public static Transform previousTarget;

	// Token: 0x04000CE8 RID: 3304
	public static float distance;

	// Token: 0x04000CE9 RID: 3305
	public static float sensitivity;

	// Token: 0x04000CEA RID: 3306
	public static float minDistance;

	// Token: 0x04000CEB RID: 3307
	public float xSpeed;

	// Token: 0x04000CEC RID: 3308
	public float ySpeed;

	// Token: 0x04000CED RID: 3309
	public static int yMinLimit;

	// Token: 0x04000CEE RID: 3310
	public int yMaxLimit;

	// Token: 0x04000CEF RID: 3311
	public static float x;

	// Token: 0x04000CF0 RID: 3312
	public static float y;

	// Token: 0x04000CF1 RID: 3313
	private float lastf0f1Dist;

	// Token: 0x04000CF2 RID: 3314
	private float zoomSpeed;

	// Token: 0x04000CF3 RID: 3315
	public static bool followingAircraft;

	// Token: 0x04000CF4 RID: 3316
	public static float minAircraftDistance;

	// Token: 0x04000CF5 RID: 3317
	public static bool underwater;

	// Token: 0x04000CF6 RID: 3318
	public static bool binoculars;

	// Token: 0x04000CF7 RID: 3319
	public static Vector3 relativeLocation;

	// Token: 0x04000CF8 RID: 3320
	public float surfaceLevel = 1000.08f;

	// Token: 0x04000CF9 RID: 3321
	public float underwaterLevel = 999.92f;

	// Token: 0x04000CFA RID: 3322
	public float oceanLODHeight;

	// Token: 0x04000CFB RID: 3323
	public GameObject oceanSmall;

	// Token: 0x04000CFC RID: 3324
	public Transform rays;

	// Token: 0x04000CFD RID: 3325
	public static Transform cameraDummyTransform;

	// Token: 0x04000CFE RID: 3326
	public Transform dummyTransform;

	// Token: 0x04000CFF RID: 3327
	public Vector3 currentPosition;

	// Token: 0x04000D00 RID: 3328
	public Vector3 deltaPosition;

	// Token: 0x04000D01 RID: 3329
	public Vector3 lastPosition;

	// Token: 0x04000D02 RID: 3330
	public float distanceOffset;

	// Token: 0x04000D03 RID: 3331
	public float originalY;

	// Token: 0x04000D04 RID: 3332
	public float originalDistance;

	// Token: 0x04000D05 RID: 3333
	public Vector2 currentDampeningValues;

	// Token: 0x04000D06 RID: 3334
	public float dampeningRate;

	// Token: 0x04000D07 RID: 3335
	public static float invertMouse = 1f;

	// Token: 0x04000D08 RID: 3336
	public static Vector3 museumThreshold;

	// Token: 0x04000D09 RID: 3337
	public static bool oceanShadowsOn;

	// Token: 0x04000D0A RID: 3338
	public static GameObject oceanShadowPlane;
}
