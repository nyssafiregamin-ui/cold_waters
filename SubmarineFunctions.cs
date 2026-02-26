using System;
using UnityEngine;

// Token: 0x02000162 RID: 354
public class SubmarineFunctions : MonoBehaviour
{
	// Token: 0x06000A39 RID: 2617 RVA: 0x0007D194 File Offset: 0x0007B394
	private void FixedUpdate()
	{
		bool flag = false;
		for (int i = 0; i < this.mastTransforms.Length; i++)
		{
			if (this.upScope[i])
			{
				if (this.mastTransforms[i].localPosition.y < this.peiscopeStops[i].y)
				{
					this.mastTransforms[i].Translate(Vector3.up * 0.02f * Time.deltaTime, Space.Self);
				}
				else if (this.mastTransforms[i].localPosition.y >= this.peiscopeStops[i].y)
				{
					this.upScope[i] = false;
					this.scopeStatus[i] = 1;
					flag = true;
					if (i == 2)
					{
						UIFunctions.globaluifunctions.playerfunctions.sensormanager.CheckPlayerRADAR();
						UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("radar", true);
					}
					else if (i == 0)
					{
						UIFunctions.globaluifunctions.playerfunctions.EnableESMMeter();
						UIFunctions.globaluifunctions.playerfunctions.sensormanager.CycleThroughESM();
						UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("periscope", true);
						for (int j = 0; j < GameDataManager.enemyvesselsonlevel.Length; j++)
						{
							UIFunctions.globaluifunctions.playerfunctions.sensormanager.SetShipDisplayed(j);
						}
					}
					else
					{
						UIFunctions.globaluifunctions.playerfunctions.EnableESMMeter();
						UIFunctions.globaluifunctions.playerfunctions.sensormanager.CycleThroughESM();
						UIFunctions.globaluifunctions.playerfunctions.SetStatusIcon("esm", true);
					}
				}
			}
			else if (this.downScope[i])
			{
				if (this.mastTransforms[i].localPosition.y > this.peiscopeStops[i].x)
				{
					this.mastTransforms[i].Translate(Vector3.up * -0.02f * Time.deltaTime, Space.Self);
				}
				else if (this.mastTransforms[i].localPosition.y <= this.peiscopeStops[i].x)
				{
					this.downScope[i] = false;
					this.scopeStatus[i] = -1;
					flag = true;
					UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "StowMast" + i.ToString()), UIFunctions.globaluifunctions.playerfunctions.messageLogColors["XO"], "StowMast", false);
				}
			}
		}
		if (flag)
		{
			for (int k = 0; k < this.upScope.Length; k++)
			{
				if (this.upScope[k])
				{
					return;
				}
			}
			for (int l = 0; l < this.downScope.Length; l++)
			{
				if (this.downScope[l])
				{
					return;
				}
			}
			base.enabled = false;
		}
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x0007D494 File Offset: 0x0007B694
	public void EnterPeriscopeView()
	{
		if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.background.activeSelf)
		{
			this.parentVesselMovement.parentVessel.uifunctions.playerfunctions.sensormanager.tacticalmap.SetTacticalMap();
		}
		if (UIFunctions.globaluifunctions.levelloadmanager.amplifyocclusioneffect.enabled)
		{
			UIFunctions.globaluifunctions.levelloadmanager.amplifyocclusioneffect.enabled = false;
		}
		ManualCameraZoom.binoculars = true;
		UIFunctions.globaluifunctions.levelloadmanager.submarineMarker.gameObject.SetActive(false);
		UIFunctions.globaluifunctions.playerfunctions.periscopeNightVisionLight.enabled = false;
		UIFunctions.globaluifunctions.playerfunctions.binocularZoomText.gameObject.SetActive(true);
		UIFunctions.globaluifunctions.periscopeMatMask.SetActive(true);
		UIFunctions.globaluifunctions.ZoomView();
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x0007D580 File Offset: 0x0007B780
	public void LeavePeriscopeView()
	{
		if (ManualCameraZoom.binoculars)
		{
			UIFunctions.globaluifunctions.levelloadmanager.amplifyocclusioneffect.enabled = GameDataManager.optionsBoolSettings[7];
			UIFunctions.globaluifunctions.levelloadmanager.submarineMarker.gameObject.SetActive(GameDataManager.optionsBoolSettings[13]);
			UIFunctions.globaluifunctions.playerfunctions.periscopeNightVisionLight.enabled = false;
			this.parentVesselMovement.parentVessel.uifunctions.CancelZoom();
			UIFunctions.globaluifunctions.levelloadmanager.EnvironmentSwitch(false);
			ManualCameraZoom.distance = ManualCameraZoom.minDistance + 0.5f;
		}
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			UIFunctions.globaluifunctions.playerfunctions.sensormanager.SetShipDisplayed(i);
		}
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x0007D64C File Offset: 0x0007B84C
	public void UpScopeFunction(int scopeNumber)
	{
		this.downScope[scopeNumber] = false;
		this.upScope[scopeNumber] = true;
		base.enabled = true;
		UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "RaiseMast" + scopeNumber.ToString()), UIFunctions.globaluifunctions.playerfunctions.messageLogColors["XO"], "RaiseMast" + scopeNumber.ToString(), false);
	}

	// Token: 0x06000A3D RID: 2621 RVA: 0x0007D6C8 File Offset: 0x0007B8C8
	public void DownScopeFunction(int scopeNumber)
	{
		this.upScope[scopeNumber] = false;
		this.downScope[scopeNumber] = true;
		base.enabled = true;
		if (scopeNumber == 0)
		{
			this.LeavePeriscopeView();
			this.ClearDetectionTypes(1);
		}
		else if (scopeNumber == 1)
		{
			this.ClearDetectionTypes(3);
		}
		else if (scopeNumber == 2)
		{
			this.ClearDetectionTypes(2);
		}
		if (!this.GetMastIsUp(0) && !this.GetMastIsUp(1))
		{
			UIFunctions.globaluifunctions.playerfunctions.DisableESMMeter();
		}
		UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "LowerMast" + scopeNumber.ToString()), UIFunctions.globaluifunctions.playerfunctions.messageLogColors["XO"], "LowerMast" + scopeNumber.ToString(), false);
	}

	// Token: 0x06000A3E RID: 2622 RVA: 0x0007D7A4 File Offset: 0x0007B9A4
	public bool GetMastIsExposed(int scopeNumber)
	{
		return GameDataManager.playervesselsonlevel[0].submarineFunctions.scopeStatus[scopeNumber] == 1;
	}

	// Token: 0x06000A3F RID: 2623 RVA: 0x0007D7C4 File Offset: 0x0007B9C4
	public bool GetMastIsUp(int scopeNumber)
	{
		return GameDataManager.playervesselsonlevel[0].submarineFunctions.scopeStatus[scopeNumber] == 1 && !this.downScope[scopeNumber] && GameDataManager.playervesselsonlevel[0].submarineFunctions.mastHeads[scopeNumber].transform.transform.position.y > 1000f;
	}

	// Token: 0x06000A40 RID: 2624 RVA: 0x0007D830 File Offset: 0x0007BA30
	public void ClearDetectionTypes(int detectionType)
	{
		string sensorType = "SONAR";
		switch (detectionType)
		{
		case 1:
			sensorType = "VISUAL";
			break;
		case 2:
			sensorType = "RADAR";
			break;
		case 3:
			sensorType = "ESM";
			break;
		}
		for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
		{
			if (GameDataManager.enemyvesselsonlevel[i].acoustics.playerHasDetectedWith[detectionType])
			{
				GameDataManager.enemyvesselsonlevel[i].acoustics.playerHasDetectedWith[detectionType] = false;
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.CheckIfContactLost(i, sensorType, false);
			}
		}
	}

	// Token: 0x04000FD6 RID: 4054
	public VesselMovement parentVesselMovement;

	// Token: 0x04000FD7 RID: 4055
	public Transform[] mastTransforms;

	// Token: 0x04000FD8 RID: 4056
	public Transform[] mastHeads;

	// Token: 0x04000FD9 RID: 4057
	public bool[] upScope;

	// Token: 0x04000FDA RID: 4058
	public bool[] downScope;

	// Token: 0x04000FDB RID: 4059
	public int[] scopeStatus;

	// Token: 0x04000FDC RID: 4060
	public Vector2[] peiscopeStops;

	// Token: 0x04000FDD RID: 4061
	public Transform periscopeCameraMount;

	// Token: 0x04000FDE RID: 4062
	private string periwaveParticlePath = "ships/particles/periwave";

	// Token: 0x04000FDF RID: 4063
	public ParticleSystem periwaveParticle;
}
