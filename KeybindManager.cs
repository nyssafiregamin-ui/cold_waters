using System;
using UnityEngine;

// Token: 0x0200012C RID: 300
public class KeybindManager : MonoBehaviour
{
	// Token: 0x06000821 RID: 2081 RVA: 0x00050EB8 File Offset: 0x0004F0B8
	private void FixedUpdate()
	{
		if (this.rudderTimer > 0f)
		{
			this.rudderTimer -= Time.deltaTime / Time.timeScale;
		}
		if (this.planesTimer > 0f)
		{
			this.planesTimer -= Time.deltaTime / Time.timeScale;
		}
		if (this.telegraphTimer > 0f)
		{
			this.telegraphTimer -= Time.deltaTime / Time.timeScale;
		}
		if (this.ballastTimer > 0f)
		{
			this.ballastTimer -= Time.deltaTime / Time.timeScale;
		}
	}

	// Token: 0x06000822 RID: 2082 RVA: 0x00050F68 File Offset: 0x0004F168
	private void Update()
	{
		if (UIFunctions.globaluifunctions.helpmanager.gameObject.activeSelf)
		{
			return;
		}
		if (UIFunctions.globaluifunctions.menuSystemParent.activeSelf)
		{
			return;
		}
		if (this.uifunctions.playerfunctions.currentActiveTorpedo != null)
		{
			if (this.uifunctions.playerfunctions.currentActiveTorpedo.playerControlling)
			{
				this.uifunctions.playerfunctions.currentActiveTorpedo.playerTurnInput = 0f;
				this.uifunctions.playerfunctions.currentActiveTorpedo.playerDepthInput = 0f;
				this.uifunctions.playerfunctions.currentActiveTorpedo.playerControlling = false;
			}
			if (this.torpedoButtonSteer != "NONE")
			{
				if (this.torpedoButtonSteer == "UP")
				{
					this.playerfunctions.TorpedoDepth(1f);
				}
				else if (this.torpedoButtonSteer == "DOWN")
				{
					this.playerfunctions.TorpedoDepth(-1f);
				}
				else if (this.torpedoButtonSteer == "LEFT")
				{
					this.playerfunctions.SteerTorpedo(-1f);
				}
				else if (this.torpedoButtonSteer == "RIGHT")
				{
					this.playerfunctions.SteerTorpedo(1f);
				}
			}
		}
		if (Input.GetAxis("Mouse ScrollWheel") != 0f)
		{
			if (ManualCameraZoom.binoculars)
			{
				float axis = Input.GetAxis("Mouse ScrollWheel");
				if (axis != 0f && !UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.IsMouseOverTacMiniMap())
				{
					if (axis > 0f)
					{
						if (this.playerfunctions.binocularZoomLevel < 2)
						{
							this.playerfunctions.binocularZoomLevel++;
							this.uifunctions.SetFOV();
						}
					}
					else if (this.playerfunctions.binocularZoomLevel > 0)
					{
						this.playerfunctions.binocularZoomLevel--;
						this.uifunctions.SetFOV();
					}
					return;
				}
			}
			if (TacticalMap.tacMapEnabled)
			{
				float axis2 = Input.GetAxis("Mouse ScrollWheel");
				if (axis2 < 0f)
				{
					this.uifunctions.levelloadmanager.tacticalmap.ZoomOut();
					return;
				}
				if (axis2 > 0f)
				{
					this.uifunctions.levelloadmanager.tacticalmap.ZoomIn();
					return;
				}
			}
		}
		if (Input.anyKey)
		{
			float num = 0f;
			if (InputManager.globalInputManager.GetButtonDown("Rudder Left", true))
			{
				num = -1f;
			}
			if (InputManager.globalInputManager.GetButtonDown("Rudder Right", true))
			{
				num = 1f;
			}
			if (num != 0f && this.rudderTimer <= 0f)
			{
				if (num > 0f)
				{
					this.playerfunctions.PlayerRudderRight();
				}
				else
				{
					this.playerfunctions.PlayerRudderLeft();
				}
				this.rudderTimer = this.inputTime;
			}
			float num2 = 0f;
			if (InputManager.globalInputManager.GetButtonDown("Planes Up", true))
			{
				num2 = 1f;
			}
			if (InputManager.globalInputManager.GetButtonDown("Planes Down", true))
			{
				num2 = -1f;
			}
			if (num2 != 0f && this.planesTimer <= 0f)
			{
				if (num2 > 0f)
				{
					this.playerfunctions.PlayerSurfaceSubmarine();
				}
				else
				{
					this.playerfunctions.PlayerDiveSubmarine();
				}
				this.planesTimer = this.inputTime;
			}
			float num3 = 0f;
			if (InputManager.globalInputManager.GetButtonDown("Ballast Up", true))
			{
				num3 = -1f;
			}
			if (InputManager.globalInputManager.GetButtonDown("Ballast Down", true))
			{
				num3 = 1f;
			}
			if (num3 != 0f && this.ballastTimer <= 0f)
			{
				if (num3 > 0f)
				{
					this.playerfunctions.PlayerBallastUp();
				}
				else
				{
					this.playerfunctions.PlayerBallastDown();
				}
				this.ballastTimer = this.inputTime;
			}
			float num4 = 0f;
			if (InputManager.globalInputManager.GetButtonDown("Increase Power", true))
			{
				num4 = 1f;
			}
			if (InputManager.globalInputManager.GetButtonDown("Decrease Power", true))
			{
				num4 = -1f;
			}
			if (num4 != 0f && this.telegraphTimer <= 0f)
			{
				if (num4 > 0f)
				{
					this.playerfunctions.IncreaseTelegraph();
				}
				else
				{
					this.playerfunctions.DecreaseTelegraph();
				}
				this.telegraphTimer = this.inputTime;
			}
			if (this.playerfunctions.currentActiveTorpedo != null)
			{
				if (InputManager.globalInputManager.GetButtonDown("Steer Torpedo Left", true))
				{
					this.playerfunctions.SteerTorpedo(-1f);
				}
				else if (InputManager.globalInputManager.GetButtonDown("Steer Torpedo Right", true))
				{
					this.playerfunctions.SteerTorpedo(1f);
				}
				if (InputManager.globalInputManager.GetButtonDown("Steer Torpedo Shallow", true))
				{
					this.playerfunctions.TorpedoDepth(1f);
				}
				else if (InputManager.globalInputManager.GetButtonDown("Steer Torpedo Deep", true))
				{
					this.playerfunctions.TorpedoDepth(-1f);
				}
			}
		}
		if (Input.anyKeyDown)
		{
			if (InputManager.globalInputManager.GetButtonDown("Next Tube", false))
			{
				this.playerfunctions.GetNextTube();
			}
			if (TacticalMap.tacMapEnabled || UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.IsMouseOverTacMiniMap())
			{
				if (InputManager.globalInputManager.GetButtonDown("Zoom In", false))
				{
					this.uifunctions.levelloadmanager.tacticalmap.ZoomIn();
				}
				else if (InputManager.globalInputManager.GetButtonDown("Zoom Out", false))
				{
					this.uifunctions.levelloadmanager.tacticalmap.ZoomOut();
				}
			}
			if (ManualCameraZoom.binoculars && !UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.IsMouseOverTacMiniMap())
			{
				float num5 = 0f;
				if (InputManager.globalInputManager.GetButtonDown("Zoom In", false))
				{
					num5 = 1f;
				}
				if (InputManager.globalInputManager.GetButtonDown("Zoom Out", false))
				{
					num5 = -1f;
				}
				if (num5 > 0f)
				{
					if (this.playerfunctions.binocularZoomLevel < 2)
					{
						this.playerfunctions.binocularZoomLevel++;
						this.uifunctions.SetFOV();
					}
					return;
				}
				if (num5 < 0f)
				{
					if (this.playerfunctions.binocularZoomLevel > 0)
					{
						this.playerfunctions.binocularZoomLevel--;
						this.uifunctions.SetFOV();
					}
					return;
				}
			}
			if (InputManager.globalInputManager.GetButtonDown("Level Submarine", false))
			{
				this.playerfunctions.LevelSubmarine();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Periscope Depth", false))
			{
				this.playerfunctions.helmmanager.SetPeriscopeDepth();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Emergency Deep", false))
			{
				this.playerfunctions.EmergencyDeep();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Blow Ballast", false))
			{
				this.playerfunctions.BlowBallast();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Sonar Mode", false))
			{
				this.playerfunctions.SetPlayerSonarMode();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Noisemaker", false))
			{
				this.playerfunctions.DropNoisemaker();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Time Compression", false))
			{
				this.playerfunctions.SetTimeCompression();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Hide HUD", false))
			{
				this.playerfunctions.HideHUD();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Tactical Map", false))
			{
				this.uifunctions.levelloadmanager.tacticalmap.SetTacticalMap();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Conditions", false))
			{
				this.playerfunctions.OpenContextualPanel(0);
			}
			else if (InputManager.globalInputManager.GetButtonDown("Signature", false))
			{
				this.playerfunctions.OpenContextualPanel(1);
			}
			else if (InputManager.globalInputManager.GetButtonDown("Damage Report", false))
			{
				this.playerfunctions.OpenContextualPanel(2);
			}
			else if (InputManager.globalInputManager.GetButtonDown("Stores", false))
			{
				this.playerfunctions.OpenContextualPanel(3);
			}
			else if (InputManager.globalInputManager.GetButtonDown("Centre Map", false))
			{
				this.uifunctions.levelloadmanager.tacticalmap.CentreMap();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Auto Centre Map", false))
			{
				UIFunctions.globaluifunctions.playerfunctions.AutoCentreMap();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Stores", false))
			{
				this.playerfunctions.OpenContextualPanel(3);
			}
			else if (InputManager.globalInputManager.GetButtonDown("Set Course", false))
			{
				this.playerfunctions.helmmanager.SetNavWaypoint();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Recognition Manual", false))
			{
				UIFunctions.globaluifunctions.ingamereference.SetInGameReferencePanel();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Helm Controls", false))
			{
				this.playerfunctions.helmmanager.SetHelmPanel(0);
			}
			else if (InputManager.globalInputManager.GetButtonDown("Dive Controls", false))
			{
				this.playerfunctions.helmmanager.SetHelmPanel(1);
			}
			else if (InputManager.globalInputManager.GetButtonDown("Sensor Controls", false))
			{
				this.playerfunctions.helmmanager.SetHelmPanel(2);
			}
			else if (InputManager.globalInputManager.GetButtonDown("Expand Log", false))
			{
				this.playerfunctions.ToggleFullLog();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Periscope", false))
			{
				this.playerfunctions.ScopeOne();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Mark Target", false))
			{
				this.playerfunctions.MarkTarget();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Night Vision", false))
			{
				this.playerfunctions.NightVision();
			}
			else if (InputManager.globalInputManager.GetButtonDown("ESM Mast", false))
			{
				this.playerfunctions.ScopeTwo();
			}
			else if (InputManager.globalInputManager.GetButtonDown("RADAR Mast", false))
			{
				this.playerfunctions.ScopeThree();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Periscope View", false))
			{
				this.SetPeriscopeView();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Load Tube", false))
			{
				this.playerfunctions.ReloadTube();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Fire Tube", false))
			{
				if (UIFunctions.globaluifunctions.HUDholder.activeSelf)
				{
					this.playerfunctions.LaunchWeapon();
				}
			}
			else if (InputManager.globalInputManager.GetButtonDown("Silent Running", false))
			{
				this.SetRunSilent();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Weapon Homing", false))
			{
				this.playerfunctions.SetWeaponHoming();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Weapon Attack", false))
			{
				this.playerfunctions.SetWeaponAttack();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Weapon Depth", false))
			{
				this.playerfunctions.SetWeaponDepth();
			}
			else if (InputManager.globalInputManager.GetButtonDown("Context Menu Next", false) || InputManager.globalInputManager.GetButtonDown("Context Menu Previous", false))
			{
				if (this.playerfunctions.currentOpenPanel == 1)
				{
					if (InputManager.globalInputManager.GetButtonDown("Context Menu Next", false))
					{
						this.playerfunctions.currentSignatureIndex++;
					}
					else
					{
						this.playerfunctions.currentSignatureIndex--;
					}
					if (this.playerfunctions.currentSignatureIndex == UIFunctions.globaluifunctions.playerfunctions.otherVesselList.Length)
					{
						this.playerfunctions.currentSignatureIndex = 0;
					}
					else if (this.playerfunctions.currentSignatureIndex < 0)
					{
						this.playerfunctions.currentSignatureIndex = UIFunctions.globaluifunctions.playerfunctions.otherVesselList.Length - 1;
					}
					this.playerfunctions.SetProfileGraphic();
				}
				else if (this.playerfunctions.currentOpenPanel == 2)
				{
					int num6 = this.playerfunctions.damagecontrol.currentSubsystem;
					if (InputManager.globalInputManager.GetButtonDown("Context Menu Next", false))
					{
						do
						{
							num6++;
							if (num6 == this.playerfunctions.damagecontrol.damageControlLabels.Length)
							{
								num6 = 0;
							}
						}
						while (!this.playerfunctions.damagecontrol.damageControlLabels[num6].gameObject.activeSelf);
					}
					else
					{
						do
						{
							num6--;
							if (num6 < 0)
							{
								num6 = this.playerfunctions.damagecontrol.damageControlLabels.Length - 1;
							}
						}
						while (!this.playerfunctions.damagecontrol.damageControlLabels[num6].gameObject.activeSelf);
					}
					UIFunctions.globaluifunctions.playerfunctions.damagecontrol.BoldText(num6);
					this.playerfunctions.damagecontrol.currentCompartment = this.playerfunctions.damagecontrol.GetSubsystemCompartment();
					this.playerfunctions.helmmanager.damageControlDelayTimer = 1f;
					if (PlayerFunctions.runningSilent)
					{
						UIFunctions.globaluifunctions.playerfunctions.LeaveRunningSilent();
					}
				}
				else if (this.playerfunctions.currentOpenPanel == 0)
				{
					if (InputManager.globalInputManager.GetButtonDown("Context Menu Next", false))
					{
						UIFunctions.globaluifunctions.playerfunctions.sensormanager.conditionsdisplay.SetDepthMultiplier(1);
					}
					else if (InputManager.globalInputManager.GetButtonDown("Context Menu Previous", false))
					{
						UIFunctions.globaluifunctions.playerfunctions.sensormanager.conditionsdisplay.SetDepthMultiplier(-1);
					}
				}
			}
			else if (InputManager.globalInputManager.GetButtonDown("Context Menu Set", false))
			{
				if (this.playerfunctions.currentOpenPanel == 1)
				{
					this.playerfunctions.sensormanager.ContactClassifiedManuallyByPlayer();
				}
				else if (this.playerfunctions.currentOpenPanel == 2)
				{
				}
			}
			else if (this.playerfunctions.currentActiveTorpedo != null)
			{
				if (InputManager.globalInputManager.GetButtonDown("Activate Torpedo", false))
				{
					this.playerfunctions.EnableTorpedo();
				}
				else if (InputManager.globalInputManager.GetButtonDown("Cut Wire", false))
				{
					this.playerfunctions.CutWire(this.playerfunctions.currentActiveTorpedo.tubefiredFrom);
				}
			}
		}
	}

	// Token: 0x06000823 RID: 2083 RVA: 0x00051EEC File Offset: 0x000500EC
	public void SetPeriscopeView()
	{
		if (!LevelLoadManager.inMuseum)
		{
			if (!ManualCameraZoom.binoculars)
			{
				if (GameDataManager.playervesselsonlevel[0].submarineFunctions.scopeStatus[0] == 1 && !GameDataManager.playervesselsonlevel[0].submarineFunctions.upScope[0] && !GameDataManager.playervesselsonlevel[0].submarineFunctions.downScope[0])
				{
					GameDataManager.playervesselsonlevel[0].submarineFunctions.EnterPeriscopeView();
				}
			}
			else
			{
				GameDataManager.playervesselsonlevel[0].submarineFunctions.LeavePeriscopeView();
			}
		}
	}

	// Token: 0x06000824 RID: 2084 RVA: 0x00051F80 File Offset: 0x00050180
	public void SigAnalysisChange(int change)
	{
		this.playerfunctions.currentSignatureIndex += change;
		if (this.playerfunctions.currentSignatureIndex == UIFunctions.globaluifunctions.playerfunctions.otherVesselList.Length)
		{
			this.playerfunctions.currentSignatureIndex = 0;
		}
		else if (this.playerfunctions.currentSignatureIndex < 0)
		{
			this.playerfunctions.currentSignatureIndex = UIFunctions.globaluifunctions.playerfunctions.otherVesselList.Length - 1;
		}
		this.playerfunctions.SetProfileGraphic();
	}

	// Token: 0x06000825 RID: 2085 RVA: 0x0005200C File Offset: 0x0005020C
	public void SigAnalysisSet()
	{
		this.playerfunctions.sensormanager.ContactClassifiedManuallyByPlayer();
	}

	// Token: 0x06000826 RID: 2086 RVA: 0x00052020 File Offset: 0x00050220
	public void SetRunSilent()
	{
		if (PlayerFunctions.runningSilent)
		{
			this.playerfunctions.LeaveRunningSilent();
		}
		else
		{
			this.playerfunctions.EnterRunningSilent();
		}
	}

	// Token: 0x04000BD2 RID: 3026
	public UIFunctions uifunctions;

	// Token: 0x04000BD3 RID: 3027
	public PlayerFunctions playerfunctions;

	// Token: 0x04000BD4 RID: 3028
	public float rudderTimer;

	// Token: 0x04000BD5 RID: 3029
	public float planesTimer;

	// Token: 0x04000BD6 RID: 3030
	public float telegraphTimer;

	// Token: 0x04000BD7 RID: 3031
	public float ballastTimer;

	// Token: 0x04000BD8 RID: 3032
	public float inputTime;

	// Token: 0x04000BD9 RID: 3033
	public bool rudderReady;

	// Token: 0x04000BDA RID: 3034
	public bool planesReady;

	// Token: 0x04000BDB RID: 3035
	public bool telegraphReady;

	// Token: 0x04000BDC RID: 3036
	public string torpedoButtonSteer;
}
