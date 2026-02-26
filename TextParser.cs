using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000165 RID: 357
public class TextParser : MonoBehaviour
{
	// Token: 0x06000A63 RID: 2659 RVA: 0x00080068 File Offset: 0x0007E268
	private void Start()
	{
		this.ReadSensorData();
		this.ReadWeaponData();
		this.ReadAircraftData();
		this.ReadSubsystemData();
		this.ReadBriefingData();
		this.ReadShipData();
	}

	// Token: 0x06000A64 RID: 2660 RVA: 0x0008009C File Offset: 0x0007E29C
	public string[] OpenTextDataFile(string filename)
	{
		string[] array = new string[0];
		string text = string.Concat(new string[]
		{
			"file://",
			Application.streamingAssetsPath,
			"/override/",
			filename,
			".txt"
		});
		WWW www = new WWW(text);
		while (!www.isDone)
		{
		}
		if (www.error != null)
		{
			text = string.Concat(new string[]
			{
				"file://",
				Application.streamingAssetsPath,
				"/default/",
				filename,
				".txt"
			});
			www = new WWW(text);
			while (!www.isDone)
			{
			}
			if (www.error != null)
			{
				UIFunctions.globaluifunctions.SetPlayerErrorMessage("ERROR:  \"" + text + "\"  not found");
				return null;
			}
		}
		return www.text.Split(new char[]
		{
			'\n'
		});
	}

	// Token: 0x06000A65 RID: 2661 RVA: 0x00080188 File Offset: 0x0007E388
	public void BuildHUD(string hudFile)
	{
		string shipPrefabName = UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].shipPrefabName;
		this.hudSprites[2].sprite = this.GetSprite(this.GetFilePathFromString("hud/damage_control/" + shipPrefabName + ".png"));
		this.hudSprites[3].sprite = this.GetSprite(this.GetFilePathFromString("hud/damage_control/" + shipPrefabName + "_mask.png"));
		if (UIFunctions.globaluifunctions.levelloadmanager.lastHUDBuilt == hudFile)
		{
			GC.Collect();
			return;
		}
		UIFunctions.globaluifunctions.levelloadmanager.lastHUDBuilt = hudFile;
		UIFunctions.languagemanager.BuildCombatHUD();
		string[] array = this.OpenTextDataFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString(hudFile));
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.navyColors = new Color[4];
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.pingLines = new Color[3];
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.torpedoColors = new Color[4];
		UIFunctions.globaluifunctions.playerfunctions.messageLogColors = new Dictionary<string, Color>();
		UIFunctions.globaluifunctions.playerfunctions.helmmanager.buttonColors = new Color32[2];
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			string text = array2[0];
			switch (text)
			{
			case "PlayerDataColor1":
			{
				Color32 color = this.GetColor32(array2[1].Trim());
				UIFunctions.languagemanager.titlesPlayer.color = color;
				UIFunctions.languagemanager.unitsPlayer.color = color;
				UIFunctions.languagemanager.titlesContact.color = color;
				UIFunctions.languagemanager.unitsContact.color = color;
				UIFunctions.languagemanager.titlesWeapon.color = color;
				UIFunctions.languagemanager.unitsWeapon.color = color;
				UIFunctions.globaluifunctions.playerfunctions.contactDepth.color = color;
				break;
			}
			case "PlayerDataColor2":
			{
				Color32 color2 = this.GetColor32(array2[1].Trim());
				UIFunctions.globaluifunctions.playerfunctions.ownShipData.color = color2;
				UIFunctions.globaluifunctions.playerfunctions.contactData.color = color2;
				UIFunctions.globaluifunctions.playerfunctions.contactDataName.color = color2;
				UIFunctions.globaluifunctions.playerfunctions.wireData[0].color = color2;
				UIFunctions.globaluifunctions.playerfunctions.wireData[1].color = color2;
				break;
			}
			case "ContactAboveLayer":
				UIFunctions.globaluifunctions.playerfunctions.contactDepthSprites[0] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ContactBelowLayer":
				UIFunctions.globaluifunctions.playerfunctions.contactDepthSprites[1] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "PlayerMarkerColor":
				UIFunctions.globaluifunctions.levelloadmanager.submarineMarker.GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", this.GetColor32(array2[1].Trim()));
				break;
			case "BackgroundBlockColor":
				this.hudSprites[1].color = this.GetColor32(array2[1].Trim());
				break;
			case "TabPanels":
				this.hudSprites[0].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "MessageLogBackground":
				foreach (Image image in UIFunctions.globaluifunctions.playerfunctions.messageLogBackgrounds)
				{
					image.sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				}
				break;
			case "DamageControlPanel":
				UIFunctions.globaluifunctions.playerfunctions.contextualPanels[2].GetComponent<Image>().sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "DamageControlColor1":
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageLabelColors[0] = this.GetColor32(array2[1].Trim());
				break;
			case "DamageControlColor2":
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageLabelColors[1] = this.GetColor32(array2[1].Trim());
				break;
			case "DamageControlColor3":
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageLabelColors[2] = this.GetColor32(array2[1].Trim());
				break;
			case "DamageControlColor4":
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageLabelColors[3] = this.GetColor32(array2[1].Trim());
				break;
			case "DamageControlColor5":
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageLabelColors[4] = this.GetColor32(array2[1].Trim());
				break;
			case "DamageControlColor6":
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.timeToRepairReadout.color = this.GetColor32(array2[1].Trim());
				break;
			case "DamageControlIcon":
				this.hudSprites[4].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "SignaturePanel":
				UIFunctions.globaluifunctions.playerfunctions.contextualPanels[1].GetComponent<Image>().sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "SignatureTextColor1":
			{
				Color32 color3 = this.GetColor32(array2[1].Trim());
				UIFunctions.languagemanager.signatureLabels.color = color3;
				UIFunctions.languagemanager.profile.color = color3;
				foreach (Text text2 in UIFunctions.languagemanager.signatureUnits)
				{
					text2.color = color3;
				}
				foreach (Image image2 in UIFunctions.globaluifunctions.playerfunctions.signatureIcons)
				{
					image2.color = color3;
				}
				UIFunctions.globaluifunctions.playerfunctions.signatureData[2].color = color3;
				UIFunctions.globaluifunctions.playerfunctions.signatureData[4].color = color3;
				break;
			}
			case "SignatureTextColor2":
			{
				Color32 color4 = this.GetColor32(array2[1].Trim());
				UIFunctions.globaluifunctions.playerfunctions.signatureData[0].color = color4;
				UIFunctions.globaluifunctions.playerfunctions.signatureData[1].color = color4;
				UIFunctions.globaluifunctions.playerfunctions.signatureData[3].color = color4;
				UIFunctions.globaluifunctions.playerfunctions.signatureData[5].color = color4;
				break;
			}
			case "SignatureIconColor":
			{
				Color32 color5 = this.GetColor32(array2[1].Trim());
				this.hudSprites[8].color = color5;
				this.hudSprites[9].color = color5;
				this.hudSprites[10].color = color5;
				break;
			}
			case "SignatureIconNext":
				this.hudSprites[8].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "SignatureIconPrev":
				this.hudSprites[9].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "SignatureIconSelect":
				this.hudSprites[10].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "StoresPanel":
				UIFunctions.globaluifunctions.playerfunctions.contextualPanels[3].GetComponent<Image>().sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "StoresPanelCombat":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.storesBackgrounds[0].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "StoresPanelPort":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.storesBackgrounds[1].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "CommandoIcon":
				this.hudSprites[5].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "NoisemakerIcon":
				this.hudSprites[7].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "StoresTextColor1":
				UIFunctions.globaluifunctions.portRearm.textColors[0] = this.GetColor32(array2[1].Trim());
				break;
			case "StoresTextColor2":
				UIFunctions.globaluifunctions.portRearm.textColors[1] = this.GetColor32(array2[1].Trim());
				break;
			case "StoresTextColor3":
			{
				Color32 color6 = this.GetColor32(array2[1].Trim());
				UIFunctions.globaluifunctions.portRearm.sealTeamTimeText.color = this.GetColor32(array2[1].Trim());
				foreach (Text text3 in UIFunctions.globaluifunctions.portRearm.vlsNumber)
				{
					text3.color = color6;
				}
				break;
			}
			case "ConditionsPanel":
				UIFunctions.globaluifunctions.playerfunctions.contextualPanels[0].GetComponent<Image>().sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ConditionsTextColor":
			{
				Color32 color7 = this.GetColor32(array2[1].Trim());
				foreach (Text text4 in UIFunctions.globaluifunctions.playerfunctions.sensormanager.conditionsdisplay.conditionsLabels)
				{
					text4.color = color7;
				}
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.conditionsdisplay.depthGauge.color = color7;
				break;
			}
			case "ConditionsPlayer":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.conditionsdisplay.contactImage[0] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.conditionsdisplay.playerIcon.GetComponent<Image>().sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ConditionsTorpedo":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.conditionsdisplay.torpedoIcon.GetComponent<Image>().sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ConditionsWarship":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.conditionsdisplay.contactImage[1] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ConditionsSubmarine":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.conditionsdisplay.contactImage[2] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ConditionsMerchant":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.conditionsdisplay.contactImage[3] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ConditionsBiologic":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.conditionsdisplay.contactImage[4] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ConditionsUnknown":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.conditionsdisplay.contactImage[5] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ConditionsSeaLevel":
				this.hudSprites[6].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ConditionsFloor":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.conditionsdisplay.floorLine.gameObject.GetComponent<Image>().sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ConditionsLayer":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.conditionsdisplay.layerLine.gameObject.GetComponent<Image>().sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "PeriscopeESMMeter":
				UIFunctions.globaluifunctions.playerfunctions.esmGameObject.GetComponent<Image>().sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "PeriscopeMaskDay":
				UIFunctions.globaluifunctions.levelloadmanager.periscopeMasks = new string[2];
				UIFunctions.globaluifunctions.levelloadmanager.periscopeMasks[0] = array2[1].Trim();
				break;
			case "PeriscopeMaskNight":
				UIFunctions.globaluifunctions.levelloadmanager.periscopeMasks[1] = array2[1].Trim();
				break;
			case "MiniMapOff":
				UIFunctions.globaluifunctions.playerfunctions.tacMapMinimisedGraphic.sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "MiniMapOn":
				UIFunctions.globaluifunctions.playerfunctions.tacMapMaximisedGraphic.sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "StatusLayout":
			{
				bool flag = true;
				if (array2[1].Trim() != "VERTICAL")
				{
					flag = false;
				}
				Vector2 zero = Vector2.zero;
				zero.x -= 40f;
				for (int num2 = 0; num2 < UIFunctions.globaluifunctions.playerfunctions.statusIcons.Length; num2++)
				{
					UIFunctions.globaluifunctions.playerfunctions.statusIcons[num2].transform.localPosition += new Vector3(zero.x, zero.y, 0f);
					if (flag)
					{
						zero.y -= 40f;
					}
					else
					{
						zero.x -= 40f;
					}
				}
				break;
			}
			case "StatusTimeCompression":
				UIFunctions.globaluifunctions.playerfunctions.statusIcons[0].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "StatusShallow":
				UIFunctions.globaluifunctions.playerfunctions.statusIcons[1].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "StatusIce":
				UIFunctions.globaluifunctions.playerfunctions.statusIcons[2].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "StatusMine":
				UIFunctions.globaluifunctions.playerfunctions.statusIcons[3].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "StatusRunningSilent":
				UIFunctions.globaluifunctions.playerfunctions.statusIcons[4].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "StatusCavitating":
				UIFunctions.globaluifunctions.playerfunctions.statusIcons[5].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "StatusReactor":
				UIFunctions.globaluifunctions.playerfunctions.statusIcons[6].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "StatusDamage":
				UIFunctions.globaluifunctions.playerfunctions.statusIcons[7].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "StatusFlooding":
				UIFunctions.globaluifunctions.playerfunctions.statusIcons[8].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "StatusTorpedo":
				UIFunctions.globaluifunctions.playerfunctions.statusIcons[9].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "StatusRangeGating":
				UIFunctions.globaluifunctions.playerfunctions.statusIcons[10].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "StatusActiveSonar":
				UIFunctions.globaluifunctions.playerfunctions.statusIcons[11].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "StatusPeriscope":
				UIFunctions.globaluifunctions.playerfunctions.statusIcons[12].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "StatusESMMast":
				UIFunctions.globaluifunctions.playerfunctions.statusIcons[13].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "StatusRADARMast":
				UIFunctions.globaluifunctions.playerfunctions.statusIcons[14].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "StatusEventCamera":
				UIFunctions.globaluifunctions.playerfunctions.statusIcons[15].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "StatusCourse":
				UIFunctions.globaluifunctions.playerfunctions.statusIcons[16].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "StatusDepth":
				UIFunctions.globaluifunctions.playerfunctions.statusIcons[17].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "CameraBearingMarker":
				UIFunctions.globaluifunctions.bearingMarker.sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "CameraBearingTape":
				this.tapeGuage.SetTexture("_MainTex", UIFunctions.globaluifunctions.textparser.GetTexture(array2[1].Trim()));
				break;
			case "TorpedoTubeSelected":
				UIFunctions.globaluifunctions.playerfunctions.torpedoTubeBackgrounds[1] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "TorpedoTubeUnselected":
				UIFunctions.globaluifunctions.playerfunctions.torpedoTubeBackgrounds[0] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "TorpedoTubeDestroyed":
				UIFunctions.globaluifunctions.playerfunctions.tubeDestroyedSprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "TorpedoTubeWire":
				UIFunctions.globaluifunctions.playerfunctions.wireSprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "VerticalLaunchSystemTextColor":
				UIFunctions.globaluifunctions.playerfunctions.vlsLabel.color = this.GetColor32(array2[1].Trim());
				break;
			case "WeaponHighlight":
				for (int num3 = 0; num3 < UIFunctions.globaluifunctions.portRearm.highlights.Length; num3++)
				{
					UIFunctions.globaluifunctions.portRearm.highlights[num3].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				}
				break;
			case "ButtonOffColor":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.buttonColors[0] = this.GetColor32(array2[1].Trim());
				break;
			case "ButtonOnColor":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.buttonColors[1] = this.GetColor32(array2[1].Trim());
				break;
			case "TorpedoSettingStraightSearch":
				UIFunctions.globaluifunctions.playerfunctions.attackSettingSprites[0] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "TorpedoSettingSnakeSearch":
				UIFunctions.globaluifunctions.playerfunctions.attackSettingSprites[1] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "TorpedoSettingLeftSearch":
				UIFunctions.globaluifunctions.playerfunctions.attackSettingSprites[2] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "TorpedoSettingRightSearch":
				UIFunctions.globaluifunctions.playerfunctions.attackSettingSprites[3] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "MissileSettingWideCone":
				UIFunctions.globaluifunctions.playerfunctions.attackSettingSprites[4] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "MissileSettingNarrowCone":
				UIFunctions.globaluifunctions.playerfunctions.attackSettingSprites[5] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "TorpedoSettingShallow":
				UIFunctions.globaluifunctions.playerfunctions.depthSettingSprites[0] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "TorpedoSettingLevel":
				UIFunctions.globaluifunctions.playerfunctions.depthSettingSprites[1] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "TorpedoSettingDeep":
				UIFunctions.globaluifunctions.playerfunctions.depthSettingSprites[2] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "MissileSettingSkim":
				UIFunctions.globaluifunctions.playerfunctions.depthSettingSprites[3] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "MissileSettingPopUp":
				UIFunctions.globaluifunctions.playerfunctions.depthSettingSprites[4] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "UnitReferenceMain":
				UIFunctions.globaluifunctions.ingamereference.ingameReferencePanels[0].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "UnitReferenceSecondary":
				UIFunctions.globaluifunctions.ingamereference.ingameReferencePanels[1].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "UnitReferenceTab":
				UIFunctions.globaluifunctions.ingamereference.ingameReferencePanels[2].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				UIFunctions.globaluifunctions.ingamereference.ingameReferencePanels[3].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "UnitReferenceIconColor":
			{
				Color32 color8 = this.GetColor32(array2[1].Trim());
				this.hudSprites[11].color = color8;
				this.hudSprites[12].color = color8;
				this.hudSprites[13].color = color8;
				this.hudSprites[14].color = color8;
				break;
			}
			case "UnitReferenceTextHighlightColor":
				UIFunctions.globaluifunctions.ingamereference.highlightColorWord = array2[1].Trim();
				break;
			case "UnitReferenceIconNext":
				this.hudSprites[11].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "UnitReferenceIconPrev":
				this.hudSprites[12].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "UnitReferenceIconSelect":
				this.hudSprites[13].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "UnitReferenceIconHighlightMarker":
				this.hudSprites[14].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "TorpedoSettingHomeActive":
				UIFunctions.globaluifunctions.playerfunctions.homeSettingSprites[0] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "TorpedoSettingHomePassive":
				UIFunctions.globaluifunctions.playerfunctions.homeSettingSprites[1] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "TacticalMapBackground":
				this.tacMapBackground.SetTexture("_MainTex", UIFunctions.globaluifunctions.textparser.GetTexture(array2[1].Trim()));
				break;
			case "TacticalMapOverlay":
				UIFunctions.globaluifunctions.tacmapCameraOverlay.texture = UIFunctions.globaluifunctions.textparser.GetTexture(array2[1].Trim());
				break;
			case "TacticalMapContactIcon":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.sonarPaintImages[0] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.sonarPaintImages[1] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.sonarPaintImages[2] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.sonarPaintImages[3] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "TacticalMapPlottedIcon":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.sonarPaintImages[5] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "TacticalMapSunkIcon":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.sonarPaintImages[4] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "TacticalMapWaypointIcon":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.waypointMarker.gameObject.GetComponent<Image>().sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "TacticalMapInterceptPointIcon":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.dumbfireMarker.gameObject.GetComponent<Image>().sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "TacticalMapNoisemakerIcon":
				for (int num4 = 0; num4 < UIFunctions.globaluifunctions.database.databasecountermeasuredata.Length; num4++)
				{
					if (UIFunctions.globaluifunctions.database.databasecountermeasuredata[num4].isNoisemaker)
					{
						UIFunctions.globaluifunctions.database.databasecountermeasuredata[num4].countermeasureObject.GetComponent<Noisemaker>().tacMapNoisemakerIcon.shipDisplayIcon.sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
					}
				}
				break;
			case "TacticalMapKnuckleIcon":
				for (int num5 = 0; num5 < UIFunctions.globaluifunctions.database.databasecountermeasuredata.Length; num5++)
				{
					if (UIFunctions.globaluifunctions.database.databasecountermeasuredata[num5].isKnuckle)
					{
						UIFunctions.globaluifunctions.database.databasecountermeasuredata[num5].countermeasureObject.GetComponent<Noisemaker>().tacMapNoisemakerIcon.shipDisplayIcon.sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
					}
				}
				break;
			case "TacticalMapSonobuoyIcon":
				UIFunctions.globaluifunctions.database.sonobuoyInWaterObject.GetComponent<Noisemaker>().tacMapNoisemakerIcon.shipDisplayIcon.sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "TacticalMapIceHazard":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.hazardSprites[0] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "TacticalMapMineHazard":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.hazardSprites[1] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "TacticalMapPort":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.hazardSprites[2] = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "IceHazardColor":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.hazardColors[0] = this.GetColor32(array2[1].Trim());
				break;
			case "MineHazardColor":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.hazardColors[1] = this.GetColor32(array2[1].Trim());
				break;
			case "PortColor":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.hazardColors[2] = this.GetColor32(array2[1].Trim());
				break;
			case "Thresholds":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlayThresholds = this.PopulateFloatArray(array2[1]);
				break;
			case "ColorBlend":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlayLerps = this.PopulateFloatArray(array2[1]);
				break;
			case "LandColor":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlayLandColor = this.GetColor32(array2[1].Trim());
				break;
			case "CoastColor":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlayCoastColor = this.GetColor32(array2[1].Trim());
				break;
			case "WaterColor":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlayWaterColor = this.GetColor32(array2[1].Trim());
				break;
			case "FilterOverlay":
				if (array2[1].Trim() == "TRUE")
				{
					UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlayFilter = true;
				}
				else
				{
					UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.landOverlayFilter = false;
				}
				if (this.hudBuiltOnce)
				{
					i = array.Length - 1;
				}
				break;
			case "PlayerColor":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.navyColors[0] = this.GetColor32(array2[1].Trim());
				break;
			case "EnemyColor":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.navyColors[1] = this.GetColor32(array2[1].Trim());
				break;
			case "NeutralColor":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.navyColors[2] = this.GetColor32(array2[1].Trim());
				break;
			case "VesselTrails":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.navyColors[3] = this.GetColor32(array2[1].Trim());
				break;
			case "SensorCone":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.sensorCone = this.GetColor32(array2[1].Trim());
				break;
			case "SensorConeActive":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.sensorConeTracking = this.GetColor32(array2[1].Trim());
				break;
			case "SonarPingLine":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.pingLines[0] = this.GetColor32(array2[1].Trim());
				break;
			case "TorpedoPingLine":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.pingLines[1] = this.GetColor32(array2[1].Trim());
				break;
			case "ESMPingLine":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.pingLines[2] = this.GetColor32(array2[1].Trim());
				break;
			case "TorpedoOnWire":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.torpedoColors[0] = this.GetColor32(array2[1].Trim());
				break;
			case "TorpedoOnWireActive":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.torpedoColors[1] = this.GetColor32(array2[1].Trim());
				break;
			case "Torpedo":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.torpedoColors[2] = this.GetColor32(array2[1].Trim());
				break;
			case "TorpedoActive":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.torpedoColors[3] = this.GetColor32(array2[1].Trim());
				break;
			case "VesselSunk":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.sunkColor = this.GetColor32(array2[1].Trim());
				break;
			case "WaypointColor":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.waypointMapColor = this.GetColor32(array2[1].Trim());
				break;
			case "WaypointReadOutColor":
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.waypointReadoutColor = this.GetColor32(array2[1].Trim());
				break;
			case "MissionMarkerColorInsertion":
				UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.missionMarkerColors[0] = this.GetColor32(array2[1].Trim());
				break;
			case "MissionMarkerColorLandStrike":
				UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.missionMarkerColors[1] = this.GetColor32(array2[1].Trim());
				break;
			case "LogDefault":
				UIFunctions.globaluifunctions.playerfunctions.messageLogColors.Add("Default", this.GetColor32(array2[1].Trim()));
				break;
			case "LogHelm":
				UIFunctions.globaluifunctions.playerfunctions.messageLogColors.Add("Helm", this.GetColor32(array2[1].Trim()));
				break;
			case "LogManeuver":
				UIFunctions.globaluifunctions.playerfunctions.messageLogColors.Add("Maneuver", this.GetColor32(array2[1].Trim()));
				break;
			case "LogSonar":
				UIFunctions.globaluifunctions.playerfunctions.messageLogColors.Add("Sonar", this.GetColor32(array2[1].Trim()));
				break;
			case "LogTorpedoRoom":
				UIFunctions.globaluifunctions.playerfunctions.messageLogColors.Add("TorpedoRoom", this.GetColor32(array2[1].Trim()));
				break;
			case "LogFireControl":
				UIFunctions.globaluifunctions.playerfunctions.messageLogColors.Add("FireControl", this.GetColor32(array2[1].Trim()));
				break;
			case "LogXO":
				UIFunctions.globaluifunctions.playerfunctions.messageLogColors.Add("XO", this.GetColor32(array2[1].Trim()));
				break;
			case "LogWarning":
				UIFunctions.globaluifunctions.playerfunctions.messageLogColors.Add("Warning", this.GetColor32(array2[1].Trim()));
				break;
			case "BottomLeftPanelPos":
			{
				Vector2 miniMapOffset = this.PopulateVector2(array2[1].Trim());
				UIFunctions.globaluifunctions.playerfunctions.otherPanel.transform.localPosition += new Vector3(miniMapOffset.x, miniMapOffset.y, 0f);
				UIFunctions.globaluifunctions.optionsmanager.miniMapOffset = miniMapOffset;
				break;
			}
			case "ToolbarsPos":
			{
				Vector3 vector = this.PopulateVector3(array2[1].Trim());
				if (vector.z == 0f)
				{
					UIFunctions.globaluifunctions.playerfunctions.helmmanager.helmToolbars.SetParent(UIFunctions.globaluifunctions.playerfunctions.helmmanager.helmmanagerAnchor);
				}
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.helmToolbars.localPosition += new Vector3(vector.x, vector.y, 0f);
				break;
			}
			case "ForceAllToolbarsOn":
				if (array2[1].Trim() == "TRUE")
				{
					UIFunctions.globaluifunctions.playerfunctions.helmmanager.forceAllToolbarsOn = true;
				}
				else
				{
					UIFunctions.globaluifunctions.playerfunctions.helmmanager.forceAllToolbarsOn = false;
				}
				break;
			case "HelmToolbarOffset":
			{
				Vector2 vector2 = this.PopulateVector2(array2[1].Trim());
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.helmPanel.transform.localPosition += new Vector3(vector2.x, vector2.y, 0f);
				break;
			}
			case "DiveToolbarOffset":
			{
				Vector2 vector3 = this.PopulateVector2(array2[1].Trim());
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.divePanel.transform.localPosition += new Vector3(vector3.x, vector3.y, 0f);
				break;
			}
			case "SensorToolbarOffset":
			{
				Vector2 vector4 = this.PopulateVector2(array2[1].Trim());
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.MastPanel.transform.localPosition += new Vector3(vector4.x, vector4.y, 0f);
				break;
			}
			case "BottomRightPanelPos":
				UIFunctions.globaluifunctions.playerfunctions.menuPanelOffset = this.PopulateVector2(array2[1].Trim());
				break;
			case "UpperRightStatusIcons":
			{
				Vector2 vector5 = this.PopulateVector2(array2[1].Trim());
				UIFunctions.globaluifunctions.playerfunctions.statusIconsParent.transform.localPosition += new Vector3(vector5.x, vector5.y, 0f);
				break;
			}
			case "UpperCentreBearingTapePos":
			{
				Vector2 vector6 = this.PopulateVector2(array2[1].Trim());
				UIFunctions.globaluifunctions.bearingMarker.gameObject.transform.localPosition += new Vector3(vector6.x, vector6.y, 0f);
				break;
			}
			case "BottomCentreWaypointInfoPos":
			{
				Vector2 vector7 = this.PopulateVector2(array2[1].Trim());
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.waypointReadout.transform.localPosition += new Vector3(vector7.x, vector7.y, 0f);
				break;
			}
			case "UpperRightTacMapZoomReadoutPos":
			{
				Vector2 vector8 = this.PopulateVector2(array2[1].Trim());
				UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.zoomText.transform.localPosition += new Vector3(vector8.x, vector8.y, 0f);
				break;
			}
			case "UpperRightPeriscopeZoomReadoutPos":
			{
				Vector2 vector9 = this.PopulateVector2(array2[1].Trim());
				UIFunctions.globaluifunctions.playerfunctions.binocularZoomText.transform.localPosition += new Vector3(vector9.x, vector9.y, 0f);
				break;
			}
			case "UpperRightRecognitionManual":
			{
				Vector2 vector10 = this.PopulateVector2(array2[1].Trim());
				UIFunctions.globaluifunctions.ingamereference.mainPanel.transform.parent.transform.localPosition += new Vector3(vector10.x, vector10.y, 0f);
				break;
			}
			case "UpperLeftPeriscopeESMPos":
			{
				Vector2 vector11 = this.PopulateVector2(array2[1].Trim());
				UIFunctions.globaluifunctions.playerfunctions.esmGameObject.transform.localPosition += new Vector3(vector11.x, vector11.y, 0f);
				break;
			}
			case "MainCameraFieldOfView":
				GameDataManager.mainCameraFOV = float.Parse(array2[1].Trim());
				UIFunctions.globaluifunctions.MainCamera.GetComponent<Camera>().fieldOfView = GameDataManager.mainCameraFOV;
				break;
			case "HelmMaximized":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.toolbarImages[1].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "HelmMinimized":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.toolbarImages[0].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "DiveMaximized":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.toolbarImages[3].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "DiveMinimized":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.toolbarImages[2].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "SensorsMaximized":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.toolbarImages[5].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "SensorsMinimized":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.toolbarImages[4].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "GenericArrowUp":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[0].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[4].sprite = UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[0].sprite;
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[19].sprite = UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[0].sprite;
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[26].sprite = UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[0].sprite;
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[30].sprite = UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[0].sprite;
				break;
			case "GenericArrowDown":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[1].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[5].sprite = UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[1].sprite;
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[20].sprite = UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[1].sprite;
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[27].sprite = UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[1].sprite;
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[31].sprite = UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[1].sprite;
				break;
			case "GenericArrowLeft":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[21].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "GenericArrowRight":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[22].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[28].sprite = UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[22].sprite;
				break;
			case "ButtonSilentRunning":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[2].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ButtonPlotCourse":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[3].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "TelegraphDisplayValues":
			{
				string[] array3 = this.PopulateStringArray(array2[1].Trim());
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.telegraphTexts[0].text = array3[0];
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.telegraphTexts[1].text = array3[1];
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.telegraphTexts[2].text = array3[2];
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.telegraphTexts[3].text = array3[3];
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.telegraphTexts[4].text = array3[4];
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.telegraphTexts[5].text = array3[5];
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.telegraphTexts[6].text = array3[6];
				break;
			}
			case "TelegraphDisplayMarker":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.toolbarImages[8].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ButtonPeriscopeDepth":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[6].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ButtonEmergencyDeep":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[8].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ButtonEmergencyBlow":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[7].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ButtonPeriscope":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[9].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ButtonESM":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[10].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ButtonRadar":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[11].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ButtonSonarMode":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[12].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ButtonNoisemaker":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[13].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ButtonNightVision":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[14].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ButtonMarkTarget":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[15].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ButtonCutWire":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[16].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ButtonEnableTorpedo":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[17].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ButtonEditTorpedoWaypoint":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[18].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ButtonNextTube":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[24].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ButtonLoadTube":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[25].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ButtonFireTube":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[29].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "ButtonAutoCenterMap":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[23].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "PeriscopeToolbar":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.toolbarImages[6].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "PeriscopeESMMeterFill":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.toolbarImages[7].sprite = this.GetSprite(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "PeriscopeESMMeterColor":
				UIFunctions.globaluifunctions.playerfunctions.helmmanager.toolbarImages[7].color = this.GetColor32(array2[1].Trim());
				break;
			}
		}
		this.hudBuiltOnce = true;
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.mapPositionMarkers[0].SetColor("_Color", UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.navyColors[3]);
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.mapPositionMarkers[1].SetColor("_Color", UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.navyColors[0]);
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.mapPositionMarkers[2].SetColor("_Color", UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.torpedoColors[2]);
		UIFunctions.globaluifunctions.optionsmanager.CalculateAndSetMiniMapDimensions();
		GC.Collect();
	}

	// Token: 0x06000A66 RID: 2662 RVA: 0x000842C4 File Offset: 0x000824C4
	public void ReadConfigData()
	{
		string[] array = this.OpenTextDataFile("config");
		List<string> list = new List<string>();
		AudioManager.audiomanager.musicTrackPaths = new string[9];
		this.campaignmanager.campaignDebugForcedMission = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			string text = array2[0];
			switch (text)
			{
			case "MenuCameraRect":
			{
				string[] array3 = array2[1].Trim().Split(new char[]
				{
					','
				});
				UIFunctions.globaluifunctions.GUICameraObject.rect = new Rect(float.Parse(array3[0]), float.Parse(array3[1]), float.Parse(array3[2]), float.Parse(array3[3]));
				break;
			}
			case "MenuCameraOrthographicSize":
				if (array2[1].Trim() == "DEFAULT")
				{
					UIFunctions.globaluifunctions.optionsmanager.defaultMenuCameraOrth = true;
				}
				else
				{
					UIFunctions.globaluifunctions.GUICameraObject.orthographicSize = float.Parse(array2[1].Trim());
				}
				break;
			case "MenuPixelDensity":
				UIFunctions.globaluifunctions.menuSystemParent.GetComponent<CanvasScaler>().dynamicPixelsPerUnit = float.Parse(array2[1].Trim());
				break;
			case "LoadVideos":
				if (array2[1].Trim() != "TRUE")
				{
					GameDataManager.loadVideo = false;
				}
				else
				{
					GameDataManager.loadVideo = true;
				}
				break;
			case "GenerateFullLog":
				if (array2[1].Trim() != "TRUE")
				{
					UIFunctions.globaluifunctions.playerfunctions.generateFullLog = false;
				}
				else
				{
					UIFunctions.globaluifunctions.playerfunctions.generateFullLog = true;
				}
				break;
			case "MusicMainMenu":
				AudioManager.audiomanager.musicTrackPaths[0] = array2[1].Trim();
				break;
			case "MusicStrategicMap":
				AudioManager.audiomanager.musicTrackPaths[1] = array2[1].Trim();
				break;
			case "MusicCombatAmbient1":
				AudioManager.audiomanager.musicTrackPaths[2] = array2[1].Trim();
				break;
			case "MusicCombatAmbient2":
				AudioManager.audiomanager.musicTrackPaths[3] = array2[1].Trim();
				break;
			case "MusicCombatAmbientNight":
				AudioManager.audiomanager.musicTrackPaths[4] = array2[1].Trim();
				break;
			case "MusicCombatAmbientIce":
				AudioManager.audiomanager.musicTrackPaths[5] = array2[1].Trim();
				break;
			case "MusicCombatAction":
				AudioManager.audiomanager.musicTrackPaths[6] = array2[1].Trim();
				break;
			case "MusicActionReportVictory":
				AudioManager.audiomanager.musicTrackPaths[7] = array2[1].Trim();
				break;
			case "MusicActionReportLoss":
				AudioManager.audiomanager.musicTrackPaths[8] = array2[1].Trim();
				break;
			case "MaxSurfaceDuctBonus":
				this.playerfunctions.sensormanager.maxSurfaceDuctBonus = float.Parse(array2[1]);
				break;
			case "MaxLayerBonus":
				this.playerfunctions.sensormanager.maxLayerBonus = float.Parse(array2[1]);
				break;
			case "MaxConvergenceBonus":
				this.playerfunctions.sensormanager.maxConvergenceBonus = float.Parse(array2[1]);
				break;
			case "ConvergenceRange":
				this.playerfunctions.sensormanager.convergenceRange = this.PopulateVector2(array2[1].Trim());
				break;
			case "MaxBottomBounceBonus":
				this.playerfunctions.sensormanager.maxBottomBounceBonus = float.Parse(array2[1]);
				break;
			case "BottomBounceRange":
				this.playerfunctions.sensormanager.bottomBounceRange = this.PopulateVector2(array2[1].Trim());
				break;
			case "BottomBounceDepthRange":
				this.playerfunctions.sensormanager.bottomBounceDepthRange = this.PopulateVector2(array2[1].Trim());
				break;
			case "AttenuationFactor":
				this.playerfunctions.sensormanager.attenuationFactor = float.Parse(array2[1]);
				break;
			case "SpreadingFactor":
				this.playerfunctions.sensormanager.spreadingFactor = float.Parse(array2[1]);
				break;
			case "SeaStates":
				this.playerfunctions.sensormanager.seaStates = this.PopulateStringArray(array2[1]);
				break;
			case "SeaStateProbability":
				this.playerfunctions.sensormanager.seaStateProbabilities = this.PopulateFloatArray(array2[1]);
				break;
			case "SeaStateSeasonModifier":
				this.playerfunctions.sensormanager.seaStateSeasonModifier = float.Parse(array2[1]);
				break;
			case "UnderwaterParticles":
				if (UIFunctions.globaluifunctions.particlefield == null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>(array2[1].Trim()));
					gameObject.transform.SetParent(UIFunctions.globaluifunctions.levelloadmanager.depthMask.transform);
					gameObject.transform.localRotation = Quaternion.identity;
					gameObject.transform.localPosition = new Vector3(0f, 0f, 7f);
					UIFunctions.globaluifunctions.particlefield = gameObject.GetComponent<ParticleField>();
					gameObject.SetActive(false);
				}
				break;
			case "RunningSilentBonus":
				this.playerfunctions.sensormanager.runningSilentBonus = this.PopulateVector2(array2[1].Trim());
				break;
			case "DetectionThreshold":
				this.playerfunctions.sensormanager.detectionThresholds.x = float.Parse(array2[1]);
				break;
			case "MaintainContactThreshold":
				this.playerfunctions.sensormanager.detectionThresholds.y = float.Parse(array2[1]);
				break;
			case "ContactProfileGraphicFactor":
				this.playerfunctions.contactProfileMultiplier = float.Parse(array2[1]);
				if (this.playerfunctions.contactProfileMultiplier <= 1f)
				{
					this.playerfunctions.contactProfileMultiplier = 1f;
				}
				break;
			case "Environment":
				list.Add(array2[1].Trim());
				break;
			case "UnitReferenceLightingAngle":
				UIFunctions.globaluifunctions.levelloadmanager.museumLightingAngle = this.PopulateVector3(array2[1].Trim());
				break;
			case "StrengthTypes":
				this.playerfunctions.sensormanager.strengthTypes = this.PopulateStringArray(array2[1]);
				break;
			case "TimesOfDay":
				this.playerfunctions.sensormanager.timesOfDay = this.PopulateStringArray(array2[1]);
				break;
			case "Weather":
				this.playerfunctions.sensormanager.weather = this.PopulateStringArray(array2[1]);
				break;
			case "WeatherProbability":
				this.playerfunctions.sensormanager.weatherProbabilities = this.PopulateFloatArray(array2[1]);
				break;
			case "WeatherSeasonModifier":
				this.playerfunctions.sensormanager.weatherSeasonModifier = float.Parse(array2[1]);
				break;
			case "OceanBaseAmbientNoise":
				this.playerfunctions.sensormanager.oceanAmbientBaseNoise = float.Parse(array2[1]);
				break;
			case "NoisePerSeaState":
				this.playerfunctions.sensormanager.noisePerSeaState = float.Parse(array2[1]);
				break;
			case "MaxNoiseFromRain":
				this.playerfunctions.sensormanager.maxNoiseFromRain = float.Parse(array2[1]);
				break;
			case "NoiseFromCavitation":
				this.playerfunctions.sensormanager.noiseFromCavitation = float.Parse(array2[1]);
				break;
			case "NoiseFromTransient":
				this.playerfunctions.sensormanager.noiseFromTransient = float.Parse(array2[1]);
				break;
			case "TargetNoisePerKnot":
				this.playerfunctions.sensormanager.targetNoisePerKnot = float.Parse(array2[1]);
				break;
			case "PassiveCompressionFactor":
				this.playerfunctions.sensormanager.passiveCompressionFactor = float.Parse(array2[1]);
				if (this.playerfunctions.sensormanager.passiveCompressionFactor != 0f)
				{
					this.playerfunctions.sensormanager.passiveBaselineFactor = 135f - 135f / this.playerfunctions.sensormanager.passiveCompressionFactor;
				}
				else
				{
					this.playerfunctions.sensormanager.passiveCompressionFactor = 1f;
					this.playerfunctions.sensormanager.passiveBaselineFactor = 0f;
				}
				break;
			case "LandAbsorptionFold":
				this.playerfunctions.sensormanager.landAbsorptionFold = this.PopulateVector2(array2[1].Trim());
				break;
			case "MaxShallowsAbsorption":
				this.playerfunctions.sensormanager.maxShallowsAbsorption = float.Parse(array2[1]);
				break;
			case "NavigationSonarRange":
				this.playerfunctions.sensormanager.highFrequencyNavSonarRange = float.Parse(array2[1]);
				break;
			case "MADDetectionRangeInYards":
				this.playerfunctions.sensormanager.madDetectionRange = float.Parse(array2[1]);
				break;
			case "NearbyVesselMinDistance":
				UIFunctions.globaluifunctions.missionmanager.contactLeaveDist = float.Parse(array2[1]);
				break;
			case "NearbyAircraftMinDistance":
				UIFunctions.globaluifunctions.missionmanager.aircraftLeaveDist = float.Parse(array2[1]);
				break;
			case "NearbyWeaponMinDistance":
				UIFunctions.globaluifunctions.missionmanager.weaponLeaveDist = float.Parse(array2[1]);
				break;
			case "WhaleProbability":
				UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.whaleProbability = float.Parse(array2[1]);
				break;
			case "NeutralVesselProbability1":
				UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.neutralProb1 = float.Parse(array2[1]);
				if (UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.neutralProb1 == 0f)
				{
					UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.neutralProb1 = 500f;
				}
				else
				{
					UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.neutralProb1 = 1f / UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.neutralProb1;
				}
				break;
			case "NeutralVesselProbability2":
				UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.neutralProb2 = float.Parse(array2[1]);
				if (UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.neutralProb2 == 0f)
				{
					UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.neutralProb2 = 500f;
				}
				else
				{
					UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.neutralProb2 = 1f / UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.neutralProb2;
				}
				break;
			case "CampaignDebugOnLoad":
				if (array2[1].Trim() == "TRUE")
				{
					this.campaignmanager.campaignDebugMode = true;
				}
				break;
			case "EventsDebugOnContinue":
				if (array2[1].Trim() == "TRUE")
				{
					this.campaignmanager.eventManager.eventDebugMode = true;
				}
				break;
			case "UnitReferenceDebugOnSpace":
				if (array2[1].Trim() == "TRUE")
				{
					UIFunctions.globaluifunctions.levelloadmanager.shipDebugModeOn = true;
				}
				break;
			case "AutoCameraPanAndZoom":
				if (array2[1].Trim() == "TRUE")
				{
					UIFunctions.globaluifunctions.optionsmanager.cameraAutoPan.SetActive(true);
				}
				break;
			case "ForceCampaignMissionType":
				if (array2[1].Trim() != "FALSE")
				{
					this.campaignmanager.campaignDebugForcedMission = array2[1].Trim();
				}
				break;
			case "ForceCampaignMissionNumber":
				if (array2[1].Trim() != "FALSE")
				{
					this.campaignmanager.campaignDebugForcedMissionNumber = int.Parse(array2[1]);
				}
				break;
			case "TruthIsOutThere":
				if (array2[1].Trim() == "TRUE")
				{
					UIFunctions.globaluifunctions.playerfunctions.sensormanager.truthIsOutThere = true;
				}
				break;
			}
		}
		UIFunctions.globaluifunctions.levelloadmanager.environmentNamesAndTags = list.ToArray();
	}

	// Token: 0x06000A67 RID: 2663 RVA: 0x00085234 File Offset: 0x00083434
	public void ReadDifficultyData()
	{
		string[] array = this.OpenTextDataFile("config");
		int num = (int)GameDataManager.optionsFloatSettings[3];
		OptionsManager.difficultySettings = new Dictionary<string, float>();
		bool flag = false;
		for (int i = 0; i < array.Length; i++)
		{
			if (flag)
			{
				if (array[i].Contains("="))
				{
					string[] array2 = array[i].Split(new char[]
					{
						'='
					});
					string[] array3 = array2[1].Split(new char[]
					{
						','
					});
					OptionsManager.difficultySettings.Add(array2[0], float.Parse(array3[num]));
				}
			}
			else if (array[i].Contains("[Difficulty Settings]"))
			{
				flag = true;
			}
		}
	}

	// Token: 0x06000A68 RID: 2664 RVA: 0x000852EC File Offset: 0x000834EC
	public void ReadSensorData()
	{
		DatabaseSonarData databaseSonarData = null;
		DatabaseRADARData databaseRADARData = null;
		string[] array = this.OpenTextDataFile("sensors");
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			string text = array2[0];
			if (text != null)
			{
				if (TextParser.<>f__switch$map25 == null)
				{
					TextParser.<>f__switch$map25 = new Dictionary<string, int>(2)
					{
						{
							"SonarModel",
							0
						},
						{
							"RADARModel",
							1
						}
					};
				}
				int num3;
				if (TextParser.<>f__switch$map25.TryGetValue(text, out num3))
				{
					if (num3 != 0)
					{
						if (num3 == 1)
						{
							num2++;
						}
					}
					else
					{
						num++;
					}
				}
			}
		}
		int num4 = -1;
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j].Trim() == "[Sonar]")
			{
				this.database.databasesonardata = new DatabaseSonarData[num];
				num4 = -1;
			}
			else if (array[j].Trim() == "[RADAR]")
			{
				this.database.databaseradardata = new DatabaseRADARData[num2];
				num4 = -1;
			}
			else
			{
				string[] array3 = array[j].Split(new char[]
				{
					'='
				});
				string text = array3[0];
				switch (text)
				{
				case "SonarModel":
					num4++;
					databaseSonarData = ScriptableObject.CreateInstance<DatabaseSonarData>();
					this.database.databasesonardata[num4] = databaseSonarData;
					databaseSonarData.sonarID = num4;
					databaseSonarData.sonarModel = array3[1].Trim();
					break;
				case "SonarType":
					if (array3[1].Trim() == "TOWED")
					{
						databaseSonarData.isTowed = true;
					}
					databaseSonarData.SonarType = array3[1].Trim();
					break;
				case "SonarFrequencies":
					databaseSonarData.sonarFrequencies = this.PopulateStringArray(array3[1]);
					break;
				case "SonarActiveSensitivity":
					databaseSonarData.sonarActiveSensitivity = float.Parse(array3[1]);
					break;
				case "SonarPassiveSensitivity":
					databaseSonarData.sonarPassiveSensitivity = float.Parse(array3[1]);
					break;
				case "SonarBaffle":
					if (array3[1].Trim() != "FALSE")
					{
						databaseSonarData.hasBaffle = true;
						databaseSonarData.sonarBaffle = float.Parse(array3[1]);
					}
					break;
				case "SonarNoisePerKnot":
					databaseSonarData.sonarNoisePerKnot = float.Parse(array3[1]);
					break;
				case "SonarOutput":
					databaseSonarData.sonarOutput = float.Parse(array3[1]);
					break;
				case "RADARModel":
					num4++;
					databaseRADARData = ScriptableObject.CreateInstance<DatabaseRADARData>();
					this.database.databaseradardata[num4] = databaseRADARData;
					databaseRADARData.radarname = array3[1].Trim();
					break;
				case "RADARRange":
					databaseRADARData.radarRange = float.Parse(array3[1]);
					break;
				}
			}
		}
		this.ReadSensorDescriptionData();
	}

	// Token: 0x06000A69 RID: 2665 RVA: 0x000856A0 File Offset: 0x000838A0
	private Mesh GetMesh(string meshName)
	{
		for (int i = 0; i < this.allMeshes.Length; i++)
		{
			if (this.allMeshes[i].name == meshName)
			{
				return this.allMeshes[i];
			}
		}
		Debug.Log("MESH not found " + meshName);
		return null;
	}

	// Token: 0x06000A6A RID: 2666 RVA: 0x000856F8 File Offset: 0x000838F8
	public void ReadWeaponData()
	{
		DatabaseWeaponData databaseWeaponData = null;
		DatabaseDepthChargeData databaseDepthChargeData = null;
		DatabaseCountermeasureData databaseCountermeasureData = null;
		string[] array = this.OpenTextDataFile("weapons");
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		Projectile_DepthCharge projectile_DepthCharge = null;
		Noisemaker noisemaker = null;
		Texture texture = null;
		Vector3 localPosition = Vector3.zero;
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			string text = array2[0];
			switch (text)
			{
			case "WeaponObjectReference":
				num++;
				break;
			case "DepthWeaponObjectReference":
				num2++;
				break;
			case "CountermeasureName":
				num3++;
				break;
			}
		}
		int num5 = -1;
		bool flag = false;
		bool flag2 = false;
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j].Trim() == "[Torpedoes and Missiles]")
			{
				this.database.databaseweapondata = new DatabaseWeaponData[num];
				num5 = -1;
			}
			else if (array[j].Trim() == "[Depth Charges, Mortars and Shells]")
			{
				this.database.databasedepthchargedata = new DatabaseDepthChargeData[num2];
				num5 = -1;
				flag = true;
			}
			else if (array[j].Trim() == "[Countermeasures]")
			{
				this.database.databasecountermeasuredata = new DatabaseCountermeasureData[num3];
				num5 = -1;
				flag2 = true;
			}
			else
			{
				string[] array3 = array[j].Split(new char[]
				{
					'='
				});
				string text = array3[0];
				switch (text)
				{
				case "WeaponObjectReference":
					num5++;
					databaseWeaponData = ScriptableObject.CreateInstance<DatabaseWeaponData>();
					this.database.databaseweapondata[num5] = databaseWeaponData;
					databaseWeaponData.weaponID = num5;
					databaseWeaponData.weaponName = array3[1].Trim();
					this.database.databaseweapondata[databaseWeaponData.weaponID].weaponPrefabName = array3[1].Trim();
					this.ReadWeaponDescriptionData(databaseWeaponData);
					databaseWeaponData.homeSettings = new string[]
					{
						"FALSE"
					};
					databaseWeaponData.searchSettings = new string[]
					{
						"FALSE"
					};
					databaseWeaponData.heightSettings = new string[]
					{
						"FALSE"
					};
					databaseWeaponData.maxLaunchDepth = 5000f;
					this.database.databaseweapondata[databaseWeaponData.weaponID].weaponObject = Resources.Load<GameObject>("weapons/" + array3[1].Trim());
					break;
				case "WeaponSprite":
					this.database.databaseweapondata[databaseWeaponData.weaponID].weaponImage = this.GetSprite(this.GetFilePathFromString(array3[1].Trim()));
					break;
				case "WeaponType":
					if (array3[1].Trim() == "MISSILE")
					{
						databaseWeaponData.isMissile = true;
					}
					else if (array3[1].Trim() == "DECOY")
					{
						databaseWeaponData.isDecoy = true;
					}
					else if (array3[1].Trim() == "SONOBUOY")
					{
						databaseWeaponData.isSonobuoy = true;
						this.database.aerialSonobuoyID = databaseWeaponData.weaponID;
					}
					databaseWeaponData.weaponType = array3[1].Trim();
					break;
				case "MissilePayload":
					databaseWeaponData.missilePayload = this.GetWeaponID(array3[1].Trim());
					databaseWeaponData.hasPayload = true;
					break;
				case "Warhead":
					databaseWeaponData.warhead = float.Parse(array3[1]);
					break;
				case "SurfaceLaunched":
					databaseWeaponData.surfaceLaunched = this.SetBoolean(array3[1].Trim());
					break;
				case "SwimOut":
					databaseWeaponData.swimOut = this.SetBoolean(array3[1].Trim());
					break;
				case "MaxLaunchDepth":
					databaseWeaponData.maxLaunchDepth = float.Parse(array3[1]);
					break;
				case "WireGuided":
					databaseWeaponData.wireGuided = this.SetBoolean(array3[1].Trim());
					break;
				case "WireBreakOnLaunchProbability":
					databaseWeaponData.wireBreakOnLaunchProbability = float.Parse(array3[1]);
					break;
				case "WireBreakSpeedThreshold":
					databaseWeaponData.wireBreakSpeedThreshold = float.Parse(array3[1]) / 10f;
					break;
				case "WireBreakThreshold":
					databaseWeaponData.wireBreakThreshold = float.Parse(array3[1]);
					break;
				case "LandAttack":
					databaseWeaponData.landAttack = this.SetBoolean(array3[1].Trim());
					break;
				case "RangeInYards":
					databaseWeaponData.rangeInYards = float.Parse(array3[1]);
					break;
				case "RunSpeed":
					databaseWeaponData.runSpeed = float.Parse(array3[1]);
					databaseWeaponData.actualRunSpeed = databaseWeaponData.runSpeed / 10f * GameDataManager.globalTranslationSpeed;
					databaseWeaponData.activeRunSpeed = float.Parse(array3[1]);
					databaseWeaponData.actualActiveRunSpeed = databaseWeaponData.activeRunSpeed / 10f * GameDataManager.globalTranslationSpeed;
					break;
				case "ActiveRunSpeed":
					databaseWeaponData.activeRunSpeed = float.Parse(array3[1]);
					databaseWeaponData.actualActiveRunSpeed = databaseWeaponData.activeRunSpeed / 10f * GameDataManager.globalTranslationSpeed;
					break;
				case "CruiseAltitude":
					databaseWeaponData.cruiseAltitude = float.Parse(array3[1]) / 75.13f;
					break;
				case "TurnRate":
					databaseWeaponData.turnRate = float.Parse(array3[1]);
					break;
				case "SensorAngles":
					databaseWeaponData.sensorAngles = this.PopulateVector2(array3[1]);
					break;
				case "SensorRange":
					databaseWeaponData.sensorRange = float.Parse(array3[1]);
					databaseWeaponData.actualSensorRange = databaseWeaponData.sensorRange * GameDataManager.inverseYardsScale;
					break;
				case "WeaponNoiseValues":
					databaseWeaponData.noiseValues = this.PopulateVector2(array3[1]);
					break;
				case "MissileFiringRange":
					databaseWeaponData.missileFiringRange = this.PopulateVector2(array3[1]);
					break;
				case "MaxPitchAngle":
					databaseWeaponData.maxPitchAngle = float.Parse(array3[1]);
					break;
				case "BoosterReleasedAfterSeconds":
					databaseWeaponData.boosterReleasedAfterSeconds = float.Parse(array3[1]);
					break;
				case "HomeSettings":
					databaseWeaponData.homeSettings = this.PopulateStringArray(array3[1]);
					break;
				case "AttackSettings":
					databaseWeaponData.searchSettings = this.PopulateStringArray(array3[1]);
					break;
				case "DepthSettings":
					databaseWeaponData.heightSettings = this.PopulateStringArray(array3[1]);
					break;
				case "FixedRunDepth":
					databaseWeaponData.fixedRunDepth = float.Parse(array3[1]);
					break;
				case "ResupplyTime":
					databaseWeaponData.replenishTime = float.Parse(array3[1]);
					break;
				case "MinCameraDistance":
					databaseWeaponData.minCameraDistance = float.Parse(array3[1]);
					break;
				case "CavitationParticle":
					databaseWeaponData.cavitationParticle = (Resources.Load(array3[1].Trim()) as GameObject);
					break;
				case "MissileTrailParticle":
					databaseWeaponData.missileTrail = (Resources.Load(array3[1].Trim()) as GameObject);
					break;
				case "BoosterParticle":
					databaseWeaponData.boosterParticle = (Resources.Load(array3[1].Trim()) as GameObject);
					break;
				case "ParachuteParticle":
					databaseWeaponData.parachute = (Resources.Load(array3[1].Trim()) as GameObject);
					break;
				case "ModelFile":
					this.allMeshes = Resources.LoadAll<Mesh>(array3[1].Trim());
					break;
				case "MaterialTexture":
					if (flag)
					{
						texture = (Resources.Load(array3[1].Trim()) as Texture);
					}
					break;
				case "Material":
					if (flag)
					{
						Material material = Resources.Load(array3[1].Trim()) as Material;
						material.SetTexture("_MainTex", texture);
						if (flag2)
						{
							noisemaker.cmMesh.GetComponent<MeshRenderer>().sharedMaterial = material;
						}
						else
						{
							projectile_DepthCharge.dcmesh.GetComponent<MeshRenderer>().sharedMaterial = material;
						}
					}
					break;
				case "MeshPosition":
					if (flag)
					{
						localPosition = this.PopulateVector3(array3[1].Trim());
					}
					break;
				case "DepthWeaponObjectReference":
				{
					num5++;
					databaseDepthChargeData = ScriptableObject.CreateInstance<DatabaseDepthChargeData>();
					this.database.databasedepthchargedata[num5] = databaseDepthChargeData;
					databaseDepthChargeData.depthChargeID = num5;
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load("template_objects/depthchargeTemplate"));
					gameObject.name = array3[1].Trim();
					UIFunctions.globaluifunctions.database.databasedepthchargedata[databaseDepthChargeData.depthChargeID].depthChargeObject = gameObject;
					projectile_DepthCharge = UIFunctions.globaluifunctions.database.databasedepthchargedata[databaseDepthChargeData.depthChargeID].depthChargeObject.GetComponent<Projectile_DepthCharge>();
					projectile_DepthCharge.depthChargeID = num5;
					gameObject.transform.SetParent(UIFunctions.globaluifunctions.prePlacedObjects);
					break;
				}
				case "IsProximityMine":
					databaseDepthChargeData.contactExploded = this.SetBoolean(array3[1].Trim());
					UIFunctions.globaluifunctions.levelloadmanager.proximityMineIndex = num5;
					break;
				case "DepthWeaponWarhead":
					databaseDepthChargeData.warhead = float.Parse(array3[1]);
					break;
				case "DepthWeaponRange":
					databaseDepthChargeData.weaponRange = this.PopulateVector2(array3[1]);
					break;
				case "DepthWeaponContactExploded":
					databaseDepthChargeData.contactExploded = this.SetBoolean(array3[1].Trim());
					break;
				case "DepthWeaponDepthExploded":
					databaseDepthChargeData.depthExploded = this.SetBoolean(array3[1].Trim());
					break;
				case "DepthWeaponKillRadius":
					databaseDepthChargeData.killRadius = float.Parse(array3[1]);
					break;
				case "DepthWeaponNumberFired":
					databaseDepthChargeData.numberOfMortars = int.Parse(array3[1]);
					break;
				case "DepthWeaponRateOfFire":
					databaseDepthChargeData.rateOfFire = float.Parse(array3[1]);
					break;
				case "DepthWeaponReloadTime":
					databaseDepthChargeData.reloadTime = float.Parse(array3[1]);
					break;
				case "DepthWeaponMortarPositions":
					databaseDepthChargeData.mortarPositions = this.PopulateFloatArray(array3[1]);
					break;
				case "DepthWeaponFiringPosition":
					databaseDepthChargeData.firingPositions = this.PopulateVector2(array3[1]);
					break;
				case "DepthWeaponSpreadRadius":
					databaseDepthChargeData.spreadRadius = float.Parse(array3[1]) / 150f * 2f;
					break;
				case "DepthWeaponVelocity":
					databaseDepthChargeData.velocity = float.Parse(array3[1]);
					break;
				case "DepthWeaponSinkRate":
					databaseDepthChargeData.sinkRate = float.Parse(array3[1]);
					break;
				case "DepthWeaponBubblesParticle":
					databaseDepthChargeData.bubbles = (Resources.Load(array3[1].Trim()) as GameObject);
					projectile_DepthCharge.bubblesTransform.localPosition = localPosition;
					break;
				case "DepthWeaponLaunchParticle":
					databaseDepthChargeData.launchFlare = (Resources.Load(array3[1].Trim()) as GameObject);
					break;
				case "MeshDepthWeapon":
					projectile_DepthCharge.dcmesh.GetComponent<MeshFilter>().mesh = this.GetMesh(array3[1].Trim());
					break;
				case "CountermeasureName":
					num5++;
					databaseCountermeasureData = ScriptableObject.CreateInstance<DatabaseCountermeasureData>();
					this.database.databasecountermeasuredata[num5] = databaseCountermeasureData;
					databaseCountermeasureData.countermeasureID = num5;
					databaseCountermeasureData.countermeasureName = array3[1].Trim();
					break;
				case "CountermeasureObjectReference":
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load("template_objects/countermeasureTemplate"));
					gameObject2.name = array3[1].Trim();
					UIFunctions.globaluifunctions.database.databasecountermeasuredata[databaseCountermeasureData.countermeasureID].countermeasureObject = gameObject2;
					noisemaker = UIFunctions.globaluifunctions.database.databasecountermeasuredata[databaseCountermeasureData.countermeasureID].countermeasureObject.GetComponent<Noisemaker>();
					gameObject2.transform.SetParent(UIFunctions.globaluifunctions.prePlacedObjects);
					break;
				}
				case "CountermeasureType":
				{
					string a = array3[1].Trim();
					if (a == "NOISEMAKER")
					{
						databaseCountermeasureData.isNoisemaker = true;
						databaseCountermeasureData.isChaff = false;
						databaseCountermeasureData.isKnuckle = false;
					}
					if (a == "CHAFF")
					{
						databaseCountermeasureData.isNoisemaker = false;
						databaseCountermeasureData.isChaff = true;
						databaseCountermeasureData.isKnuckle = false;
					}
					if (a == "KNUCKLE")
					{
						databaseCountermeasureData.isNoisemaker = false;
						databaseCountermeasureData.isChaff = false;
						databaseCountermeasureData.isKnuckle = true;
						this.playerfunctions.sensormanager.knuckleID = databaseCountermeasureData.countermeasureID;
					}
					break;
				}
				case "Lifetime":
					databaseCountermeasureData.lifetime = float.Parse(array3[1]);
					break;
				case "SinkRate":
					databaseCountermeasureData.fallSpeed = float.Parse(array3[1]);
					break;
				case "NoiseStrength":
					databaseCountermeasureData.noiseStrength = float.Parse(array3[1]);
					break;
				case "CountermeasureDescription":
					databaseCountermeasureData.countermeasureDescription = this.PopulateMultiLineTextArray(array3[1].Trim());
					break;
				case "CountermeasureParticle":
					databaseCountermeasureData.countermeasureParticle = (Resources.Load(array3[1].Trim()) as GameObject);
					noisemaker.cmParticleTransform.localPosition = localPosition;
					break;
				case "MeshCountermeasure":
					noisemaker.cmMesh.GetComponent<MeshFilter>().mesh = this.GetMesh(array3[1].Trim());
					break;
				}
			}
		}
		this.ReadDepthWeaponDescriptionData();
		for (int k = 0; k < this.database.databaseweapondata.Length; k++)
		{
			UIFunctions.globaluifunctions.vesselbuilder.CreateWeaponPrefabObject(k);
			bool flag3 = false;
			if (this.database.databaseweapondata[k].homeSettings[0] != "FALSE")
			{
				flag3 = true;
			}
			if (this.database.databaseweapondata[k].heightSettings[0] != "FALSE")
			{
				flag3 = true;
			}
			if (this.database.databaseweapondata[k].searchSettings[0] != "FALSE")
			{
				flag3 = true;
			}
			if (!flag3)
			{
				this.database.databaseweapondata[k].isDumbfire = true;
			}
			float num6 = 2025.37f;
			this.database.databaseweapondata[k].runTime = this.database.databaseweapondata[k].rangeInYards / UIFunctions.globaluifunctions.gamedatamanager.worldYardsScale / (this.database.databaseweapondata[k].runSpeed * UIFunctions.globaluifunctions.gamedatamanager.globalSpeedModifier * num6 / 3600f);
		}
	}

	// Token: 0x06000A6B RID: 2667 RVA: 0x000868AC File Offset: 0x00084AAC
	public void ReadWeaponDescriptionData(DatabaseWeaponData currentweapon)
	{
		string[] array = this.OpenTextDataFile(this.GetFilePathFromString("language/weapon/" + currentweapon.weaponPrefabName + "_description"));
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			string text = array2[0];
			switch (text)
			{
			case "WeaponName":
				currentweapon.weaponName = array2[1].Trim();
				break;
			case "WeaponDescriptiveName":
				currentweapon.weaponDescriptiveName = array2[1].Trim();
				break;
			case "WeaponDescription":
				currentweapon.weaponDescription = this.PopulateMultiLineTextArray(array2[1].Trim());
				break;
			}
		}
	}

	// Token: 0x06000A6C RID: 2668 RVA: 0x000869B8 File Offset: 0x00084BB8
	public void ReadSensorDescriptionData()
	{
		string[] array = this.OpenTextDataFile(this.GetFilePathFromString("language/sensor/sensor_display_names"));
		bool flag = false;
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			if (array2.Length == 3)
			{
				if (!flag)
				{
					int num = this.GetSonarID(array2[0].Trim());
					if (num != -1)
					{
						UIFunctions.globaluifunctions.database.databasesonardata[num].sonarDescription = array2[2].Trim();
						UIFunctions.globaluifunctions.database.databasesonardata[num].sonarDisplayName = array2[1].Trim();
					}
				}
				else
				{
					int num = this.GetRADARID(array2[0].Trim());
					if (num != -1)
					{
						UIFunctions.globaluifunctions.database.databaseradardata[num].radarDescription = array2[2].Trim();
						UIFunctions.globaluifunctions.database.databaseradardata[num].radarDisplayName = array2[1].Trim();
					}
				}
			}
			if (array2[0].Contains("[RADAR]"))
			{
				flag = true;
			}
		}
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x00086AD8 File Offset: 0x00084CD8
	public void ReadDepthWeaponDescriptionData()
	{
		string[] array = this.OpenTextDataFile(this.GetFilePathFromString("language/weapon/depth_weapon_display_names"));
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			if (array2.Length == 3)
			{
				int depthWeaponID = this.GetDepthWeaponID(array2[0].Trim());
				if (depthWeaponID != -1)
				{
					UIFunctions.globaluifunctions.database.databasedepthchargedata[depthWeaponID].depthchargeName = array2[1].Trim();
					UIFunctions.globaluifunctions.database.databasedepthchargedata[depthWeaponID].depthchargeDescriptiveName = array2[2].Trim();
				}
			}
		}
	}

	// Token: 0x06000A6E RID: 2670 RVA: 0x00086B7C File Offset: 0x00084D7C
	public void ReadAircraftData()
	{
		DatabaseAircraftData databaseAircraftData = null;
		string[] array = this.OpenTextDataFile("aircraft");
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			string text = array2[0];
			if (text != null)
			{
				if (TextParser.<>f__switch$map2A == null)
				{
					TextParser.<>f__switch$map2A = new Dictionary<string, int>(1)
					{
						{
							"AircraftObjectReference",
							0
						}
					};
				}
				int num2;
				if (TextParser.<>f__switch$map2A.TryGetValue(text, out num2))
				{
					if (num2 == 0)
					{
						num++;
					}
				}
			}
		}
		int num3 = -1;
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j].Trim() == "[Aircraft]")
			{
				this.database.databaseaircraftdata = new DatabaseAircraftData[num];
				num3 = -1;
			}
			else
			{
				string[] array3 = array[j].Split(new char[]
				{
					'='
				});
				string text = array3[0];
				switch (text)
				{
				case "AircraftObjectReference":
					num3++;
					databaseAircraftData = ScriptableObject.CreateInstance<DatabaseAircraftData>();
					this.database.databaseaircraftdata[num3] = databaseAircraftData;
					databaseAircraftData.aircraftID = num3;
					this.database.databaseaircraftdata[databaseAircraftData.aircraftID].aircraftPrefabName = array3[1].Trim();
					this.ReadAircraftDescriptionData(databaseAircraftData);
					break;
				case "AircraftType":
					databaseAircraftData.aircraftType = array3[1].Trim();
					break;
				case "CruiseSpeed":
					databaseAircraftData.cruiseSpeed = float.Parse(array3[1]);
					break;
				case "Length":
					databaseAircraftData.length = float.Parse(array3[1]);
					break;
				case "Height":
					databaseAircraftData.height = float.Parse(array3[1]);
					break;
				case "Weight":
					databaseAircraftData.weight = int.Parse(array3[1]);
					break;
				case "Crew":
					databaseAircraftData.crew = int.Parse(array3[1]);
					break;
				case "RADAR":
					databaseAircraftData.radarID = this.GetRADARID(array3[1].Trim());
					break;
				case "RADARSignature":
					databaseAircraftData.radarSignature = array3[1].Trim();
					break;
				case "ActiveSonarModel":
					databaseAircraftData.activeSonarID = this.GetSonarID(array3[1].Trim());
					break;
				case "PassiveSonarModel":
					databaseAircraftData.passiveSonarID = this.GetSonarID(array3[1].Trim());
					break;
				case "SonobuoyTypes":
					if (array3[1] != "FALSE")
					{
						databaseAircraftData.sonobuoytypes = this.PopulateStringArray(array3[1]);
						databaseAircraftData.sonobuoyIDs = new int[databaseAircraftData.sonobuoytypes.Length];
						for (int k = 0; k < databaseAircraftData.sonobuoytypes.Length; k++)
						{
							databaseAircraftData.sonobuoyIDs[k] = this.GetSonarID(databaseAircraftData.sonobuoytypes[k]);
						}
					}
					break;
				case "SonobuoyNumbers":
					databaseAircraftData.sonobuoyNumbers = this.PopulateIntArray(array3[1]);
					break;
				case "TorpedoTypes":
					databaseAircraftData.torpedotypes = this.PopulateStringArray(array3[1]);
					databaseAircraftData.torpedoIDs = new int[databaseAircraftData.torpedotypes.Length];
					for (int l = 0; l < databaseAircraftData.torpedotypes.Length; l++)
					{
						databaseAircraftData.torpedoIDs[l] = this.GetWeaponID(databaseAircraftData.torpedotypes[l]);
					}
					break;
				case "TorpedoNumbers":
					databaseAircraftData.torpedoNumbers = this.PopulateIntArray(array3[1]);
					break;
				case "DepthBomb":
					if (array3[1] != "FALSE")
					{
						string[] array4 = this.PopulateStringArray(array3[1]);
						databaseAircraftData.depthBombIDs = new int[databaseAircraftData.torpedotypes.Length];
						for (int m = 0; m < array4.Length; m++)
						{
							databaseAircraftData.depthBombIDs[m] = this.GetDepthWeaponID(array4[m]);
						}
					}
					break;
				case "DepthBombNumbers":
					databaseAircraftData.depthBombs = this.PopulateIntArray(array3[1]);
					break;
				case "MinCameraDistance":
					databaseAircraftData.minCameraDistance = float.Parse(array3[1]);
					break;
				}
			}
		}
	}

	// Token: 0x06000A6F RID: 2671 RVA: 0x000870B0 File Offset: 0x000852B0
	public void ReadAircraftDescriptionData(DatabaseAircraftData currentaircraft)
	{
		string[] array = this.OpenTextDataFile(this.GetFilePathFromString("language/aircraft/" + currentaircraft.aircraftPrefabName + "_description"));
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			string text = array2[0];
			switch (text)
			{
			case "AircraftName":
				currentaircraft.aircraftName = array2[1].Trim();
				break;
			case "AircraftDescriptiveName":
				currentaircraft.aircraftDescriptiveName = array2[1].Trim();
				break;
			case "History":
				currentaircraft.aircraftDescription = this.PopulateMultiLineTextArray(array2[1].Trim());
				break;
			}
		}
	}

	// Token: 0x06000A70 RID: 2672 RVA: 0x000871BC File Offset: 0x000853BC
	public void ReadSubsystemData()
	{
		string[] array = this.OpenTextDataFile("subsystems");
		DatabaseSubsystemsData databaseSubsystemsData = null;
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			string text = array2[0];
			if (text != null)
			{
				if (TextParser.<>f__switch$map2D == null)
				{
					TextParser.<>f__switch$map2D = new Dictionary<string, int>(1)
					{
						{
							"Subsystem",
							0
						}
					};
				}
				int num2;
				if (TextParser.<>f__switch$map2D.TryGetValue(text, out num2))
				{
					if (num2 == 0)
					{
						num++;
					}
				}
			}
		}
		int num3 = -1;
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j].Trim() == "[Subsystems]")
			{
				this.database.databasesubsystemsdata = new DatabaseSubsystemsData[num];
				num3 = -1;
			}
			else
			{
				string[] array3 = array[j].Split(new char[]
				{
					'='
				});
				string text = array3[0];
				if (text != null)
				{
					if (TextParser.<>f__switch$map2E == null)
					{
						TextParser.<>f__switch$map2E = new Dictionary<string, int>(2)
						{
							{
								"Subsystem",
								0
							},
							{
								"RepairTime",
								1
							}
						};
					}
					int num2;
					if (TextParser.<>f__switch$map2E.TryGetValue(text, out num2))
					{
						if (num2 != 0)
						{
							if (num2 == 1)
							{
								databaseSubsystemsData.repairTime = float.Parse(array3[1]);
							}
						}
						else
						{
							num3++;
							databaseSubsystemsData = ScriptableObject.CreateInstance<DatabaseSubsystemsData>();
							this.database.databasesubsystemsdata[num3] = databaseSubsystemsData;
							databaseSubsystemsData.subsystem = array3[1].Trim();
						}
					}
				}
			}
		}
		array = this.OpenTextDataFile(this.GetFilePathFromString("language/subsystem/subsystem_display_names"));
		for (int k = 1; k < array.Length; k++)
		{
			this.database.databasesubsystemsdata[k - 1].subsystemName = array[k].Trim();
		}
	}

	// Token: 0x06000A71 RID: 2673 RVA: 0x000873B4 File Offset: 0x000855B4
	public void ReadAwardData()
	{
		string filePathFromString = this.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/awards");
		string[] array = this.OpenTextDataFile(filePathFromString);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			string text = array2[0];
			switch (text)
			{
			case "PatrolAwards":
				this.campaignmanager.eventManager.patrolAwards = this.PopulateStringArray(array2[1]);
				break;
			case "PatrolTonnage":
				this.campaignmanager.eventManager.patrolTonnage = this.PopulateIntArray(array2[1]);
				break;
			case "CumulativeAwards":
				this.campaignmanager.eventManager.cumulativeAwards = this.PopulateStringArray(array2[1]);
				break;
			case "CumulativeTonnage":
				this.campaignmanager.eventManager.cumulativeTonnage = this.PopulateIntArray(array2[1]);
				break;
			case "MissionsPassed":
				this.campaignmanager.eventManager.missionsPassed = this.PopulateIntArray(array2[1]);
				break;
			case "WoundedAwards":
				this.campaignmanager.eventManager.woundedAwards = this.PopulateStringArray(array2[1]);
				break;
			case "ProbabilityWounded":
				this.campaignmanager.eventManager.probabilityWounded = this.PopulateFloatArray(array2[1]);
				break;
			}
		}
	}

	// Token: 0x06000A72 RID: 2674 RVA: 0x0008759C File Offset: 0x0008579C
	public void ReadBriefingData()
	{
		string[] array = this.OpenTextDataFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("language/message/briefing"));
		string text = string.Empty;
		string text2 = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			string text3 = array2[0];
			switch (text3)
			{
			case "Text":
				text = text + array2[1].TrimEnd(new char[0]) + "#";
				break;
			case "Text2":
				text2 = text2 + array2[1].TrimEnd(new char[0]) + "#";
				break;
			case "TextNoContact":
				UIFunctions.globaluifunctions.missionmanager.briefingNoContact = array2[1].Trim();
				break;
			}
		}
		string[] array3 = text.Split(new char[]
		{
			'#'
		});
		UIFunctions.globaluifunctions.missionmanager.briefingPrimaryText = new string[array3.Length];
		for (int j = 0; j < array3.Length; j++)
		{
			UIFunctions.globaluifunctions.missionmanager.briefingPrimaryText[j] = array3[j];
		}
		array3 = text2.Split(new char[]
		{
			'#'
		});
		UIFunctions.globaluifunctions.missionmanager.briefingSecondaryText = new string[array3.Length];
		for (int k = 0; k < array3.Length; k++)
		{
			UIFunctions.globaluifunctions.missionmanager.briefingSecondaryText[k] = array3[k];
		}
	}

	// Token: 0x06000A73 RID: 2675 RVA: 0x00087784 File Offset: 0x00085984
	public void ReadShipData()
	{
		string filePathFromString = this.GetFilePathFromString("vessels/_vessel_list");
		string[] array = this.OpenTextDataFile(filePathFromString);
		this.database.databaseshipdata = new DatabaseShipData[array.Length];
		Vector2 vector = Vector2.zero;
		string[] array2 = new string[0];
		int num = -1;
		for (int i = 0; i < array.Length; i++)
		{
			filePathFromString = this.GetFilePathFromString("vessels/" + array[i].Trim());
			string[] array3 = this.OpenTextDataFile(filePathFromString);
			bool flag = false;
			for (int j = 0; j < array3.Length; j++)
			{
				string[] array4 = array3[j].Split(new char[]
				{
					'='
				});
				string text = array4[0];
				switch (text)
				{
				case "Designation":
					num++;
					this.database.databaseshipdata[i] = ScriptableObject.CreateInstance<DatabaseShipData>();
					this.database.databaseshipdata[i].shipID = num;
					this.database.databaseshipdata[i].shipPrefabName = array[i].Trim();
					this.ReadVesselDescriptionData(this.database.databaseshipdata[i]);
					this.database.databaseshipdata[i].proprotationspeed = new float[0];
					this.database.databaseshipdata[i].subsystemPrimaryPositions = new string[this.database.databasesubsystemsdata.Length];
					this.database.databaseshipdata[i].subsystemSecondaryPositions = new string[this.database.databasesubsystemsdata.Length];
					this.database.databaseshipdata[i].subsystemTertiaryPositions = new string[this.database.databasesubsystemsdata.Length];
					this.database.databaseshipdata[i].subsystemLabelPositions = new Vector2[this.database.databasesubsystemsdata.Length];
					this.database.databaseshipdata[i].telegraphSpeeds = new float[7];
					this.database.databaseshipdata[i].shipDesignation = array4[1].Trim();
					this.database.databaseshipdata[i].aircraftNumbers = new int[0];
					this.database.databaseshipdata[i].aircraftIDs = new int[0];
					break;
				case "ShipType":
					this.database.databaseshipdata[i].shipType = array4[1].Trim();
					break;
				case "PlayerHUD":
					this.database.databaseshipdata[i].playerHUD = array4[1].Trim();
					break;
				case "Length":
					if (array4[1].Contains('|'))
					{
						string[] array5 = array4[1].Trim().Split(new char[]
						{
							'|'
						});
						this.database.databaseshipdata[i].length = float.Parse(array5[0]);
						this.database.databaseshipdata[i].displayLength = float.Parse(array5[1]);
					}
					else
					{
						this.database.databaseshipdata[i].length = float.Parse(array4[1]);
					}
					this.database.databaseshipdata[i].minCameraDistance = this.database.databaseshipdata[i].length / 84f;
					if (this.database.databaseshipdata[i].shipType == "OILRIG")
					{
						this.database.databaseshipdata[i].minCameraDistance = 0.77380955f;
						this.database.databaseshipdata[i].minCameraDistance *= 2f;
					}
					if (this.database.databaseshipdata[i].minCameraDistance < 0.7f)
					{
						this.database.databaseshipdata[i].minCameraDistance = 0.7f;
					}
					break;
				case "Beam":
					if (array4[1].Contains('|'))
					{
						string[] array6 = array4[1].Trim().Split(new char[]
						{
							'|'
						});
						this.database.databaseshipdata[i].beam = float.Parse(array6[0]);
						this.database.databaseshipdata[i].displayBeam = float.Parse(array6[1]);
					}
					else
					{
						this.database.databaseshipdata[i].beam = float.Parse(array4[1]);
					}
					break;
				case "HullHeight":
					this.database.databaseshipdata[i].hullHeight = float.Parse(array4[1]);
					break;
				case "Displacement":
					this.database.databaseshipdata[i].displacement = float.Parse(array4[1]);
					break;
				case "Crew":
					this.database.databaseshipdata[i].crew = float.Parse(array4[1]);
					break;
				case "AircraftNumbers":
					this.database.databaseshipdata[i].aircraftNumbers = this.PopulateIntArray(array4[1]);
					break;
				case "AircraftTypes":
				{
					string[] array7 = this.PopulateStringArray(array4[1]);
					this.database.databaseshipdata[i].aircraftIDs = new int[array7.Length];
					for (int k = 0; k < array7.Length; k++)
					{
						this.database.databaseshipdata[i].aircraftIDs[k] = this.GetAircraftID(array7[k]);
					}
					break;
				}
				case "Waterline":
					this.database.databaseshipdata[i].waterline = 1000f - float.Parse(array4[1]);
					break;
				case "PeriscopeDepthInFeet":
					this.database.databaseshipdata[i].periscopeDepthInFeet = int.Parse(array4[1]);
					break;
				case "HullNumbers":
					if (array4[1].Trim() == "FALSE")
					{
						this.database.databaseshipdata[i].hullnumbers = new string[0];
					}
					else
					{
						this.database.databaseshipdata[i].hullnumbers = this.PopulateStringArray(array4[1]);
					}
					break;
				case "AccelerationRate":
					this.database.databaseshipdata[i].accelerationrate = float.Parse(array4[1]);
					break;
				case "DecelerationRate":
					this.database.databaseshipdata[i].decellerationrate = float.Parse(array4[1]);
					break;
				case "RudderTurnRate":
					this.database.databaseshipdata[i].rudderturnrate = float.Parse(array4[1]);
					break;
				case "TurnRate":
					this.database.databaseshipdata[i].turnrate = float.Parse(array4[1]);
					break;
				case "PivotPointTurning":
					this.database.databaseshipdata[i].pivotpointturning = float.Parse(array4[1]);
					break;
				case "DiveRate":
					this.database.databaseshipdata[i].diverate = float.Parse(array4[1]);
					break;
				case "SurfaceRate":
					this.database.databaseshipdata[i].surfacerate = float.Parse(array4[1]);
					break;
				case "BallastRate":
					this.database.databaseshipdata[i].ballastrate = float.Parse(array4[1]);
					break;
				case "SubmergedAt":
					this.database.databaseshipdata[i].submergedat = float.Parse(array4[1]);
					this.database.databaseshipdata[i].submergedat = 1000f - this.database.databaseshipdata[i].submergedat;
					break;
				case "SurfaceSpeed":
					this.database.databaseshipdata[i].surfacespeed = float.Parse(array4[1]);
					break;
				case "SubmergedSpeed":
					this.database.databaseshipdata[i].submergedspeed = float.Parse(array4[1]);
					break;
				case "TelegraphSpeeds":
					this.database.databaseshipdata[i].telegraphSpeeds = this.PopulateFloatArray(array4[1]);
					for (int l = 0; l < this.database.databaseshipdata[i].telegraphSpeeds.Length; l++)
					{
						this.database.databaseshipdata[i].telegraphSpeeds[l] *= 0.1f;
					}
					flag = true;
					break;
				case "CavitationParameters":
					this.database.databaseshipdata[i].cavitationparameters = this.PopulateVector2(array4[1]);
					break;
				case "PropRotationSpeed":
					this.database.databaseshipdata[i].proprotationspeed = this.PopulateFloatArray(array4[1]);
					break;
				case "TestDepth":
					this.database.databaseshipdata[i].testDepth = float.Parse(array4[1]);
					this.database.databaseshipdata[i].actualTestDepth = 1000f - this.database.databaseshipdata[i].testDepth / GameDataManager.unitsToFeet;
					break;
				case "EscapeDepth":
					this.database.databaseshipdata[i].escapeDepth = float.Parse(array4[1]);
					break;
				case "SelfNoise":
					this.database.databaseshipdata[i].selfnoise = float.Parse(array4[1]);
					break;
				case "ActiveSonarReflection":
					this.database.databaseshipdata[i].activesonarreflection = float.Parse(array4[1]);
					break;
				case "ActiveSonarModel":
					this.database.databaseshipdata[i].activeSonarID = this.GetSonarID(array4[1].Trim());
					break;
				case "PassiveSonarModel":
					this.database.databaseshipdata[i].passiveSonarID = this.GetSonarID(array4[1].Trim());
					break;
				case "TowedArrayModel":
					this.database.databaseshipdata[i].towedSonarID = this.GetSonarID(array4[1].Trim());
					if (this.database.databaseshipdata[i].towedSonarID != -1)
					{
						this.database.databaseshipdata[i].passiveArrayBonus = this.database.databasesonardata[this.database.databaseshipdata[i].towedSonarID].sonarPassiveSensitivity - this.database.databasesonardata[this.database.databaseshipdata[i].passiveSonarID].sonarPassiveSensitivity;
						this.database.databaseshipdata[i].activeArrayBonus = this.database.databasesonardata[this.database.databaseshipdata[i].towedSonarID].sonarActiveSensitivity - this.database.databasesonardata[this.database.databaseshipdata[i].activeSonarID].sonarActiveSensitivity;
						if (this.database.databaseshipdata[i].passiveArrayBonus < 0f)
						{
							this.database.databaseshipdata[i].passiveArrayBonus = 0f;
						}
						if (this.database.databaseshipdata[i].activeArrayBonus < 0f)
						{
							this.database.databaseshipdata[i].activeArrayBonus = 0f;
						}
					}
					break;
				case "AnechoicCoating":
					this.database.databaseshipdata[i].anechoicCoating = this.SetBoolean(array4[1].Trim());
					break;
				case "RADAR":
					this.database.databaseshipdata[i].radarID = this.GetRADARID(array4[1].Trim());
					break;
				case "RADARSignature":
					this.database.databaseshipdata[i].radarSignature = array4[1].Trim();
					break;
				case "MissileTargetPoint":
					if (array4[1].Trim() == "FALSE")
					{
						this.database.databaseshipdata[i].targetPoint.x = 0f;
					}
					else
					{
						this.database.databaseshipdata[i].targetPoint.x = float.Parse(array4[1]);
					}
					break;
				case "TorpedoTargetPoint":
					this.database.databaseshipdata[i].targetPoint.y = float.Parse(array4[1]);
					break;
				case "TowedArrayPosition":
					this.database.databaseshipdata[i].towedArrayPosition = this.PopulateVector3(array4[1].Trim());
					break;
				case "TorpedoTypes":
					this.database.databaseshipdata[i].torpedotypes = this.PopulateStringArray(array4[1]);
					this.database.databaseshipdata[i].torpedoIDs = new int[this.database.databaseshipdata[i].torpedotypes.Length];
					for (int m = 0; m < this.database.databaseshipdata[i].torpedotypes.Length; m++)
					{
						this.database.databaseshipdata[i].torpedoIDs[m] = this.GetWeaponID(this.database.databaseshipdata[i].torpedotypes[m]);
					}
					this.database.databaseshipdata[i].torpedoGameObjects = new GameObject[this.database.databaseshipdata[i].torpedoIDs.Length];
					for (int n = 0; n < this.database.databaseshipdata[i].torpedoIDs.Length; n++)
					{
						this.database.databaseshipdata[i].torpedoGameObjects[n] = this.database.databaseweapondata[this.database.databaseshipdata[i].torpedoIDs[n]].weaponObject;
					}
					for (int num3 = 0; num3 < this.database.databaseshipdata[i].torpedotypes.Length; num3++)
					{
						this.database.databaseshipdata[i].torpedotypes[num3] = this.database.databaseweapondata[this.database.databaseshipdata[i].torpedoIDs[num3]].weaponName;
					}
					break;
				case "TorpedoNumbers":
					this.database.databaseshipdata[i].torpedoNumbers = this.PopulateIntArray(array4[1]);
					break;
				case "TorpedoTubes":
					this.database.databaseshipdata[i].torpedotubes = int.Parse(array4[1]);
					break;
				case "NumberOfWires":
					this.database.databaseshipdata[i].numberOfWires = int.Parse(array4[1]);
					break;
				case "TubeConfig":
					this.database.databaseshipdata[i].torpedoConfig = this.PopulateIntArray(array4[1]);
					break;
				case "TorpedoTubeSize":
					this.database.databaseshipdata[i].torpedotubeSize = float.Parse(array4[1]);
					break;
				case "TubeReloadTime":
					this.database.databaseshipdata[i].tubereloadtime = float.Parse(array4[1]);
					break;
				case "VLSTorpedoTypes":
					this.database.databaseshipdata[i].vlsTorpedotypes = this.PopulateStringArray(array4[1]);
					this.database.databaseshipdata[i].vlsTorpedoIDs = new int[this.database.databaseshipdata[i].vlsTorpedotypes.Length];
					for (int num4 = 0; num4 < this.database.databaseshipdata[i].vlsTorpedotypes.Length; num4++)
					{
						this.database.databaseshipdata[i].vlsTorpedoIDs[num4] = this.GetWeaponID(this.database.databaseshipdata[i].vlsTorpedotypes[num4]);
					}
					this.database.databaseshipdata[i].vlsTorpedoGameObjects = new GameObject[this.database.databaseshipdata[i].vlsTorpedoIDs.Length];
					for (int num5 = 0; num5 < this.database.databaseshipdata[i].vlsTorpedoIDs.Length; num5++)
					{
						this.database.databaseshipdata[i].vlsTorpedoGameObjects[num5] = this.database.databaseweapondata[this.database.databaseshipdata[i].vlsTorpedoIDs[num5]].weaponObject;
					}
					for (int num6 = 0; num6 < this.database.databaseshipdata[i].vlsTorpedotypes.Length; num6++)
					{
						this.database.databaseshipdata[i].vlsTorpedotypes[num6] = this.database.databaseweapondata[this.database.databaseshipdata[i].vlsTorpedoIDs[num6]].weaponName;
					}
					break;
				case "VLSTorpedoNumbers":
					this.database.databaseshipdata[i].vlsTorpedoNumbers = this.PopulateIntArray(array4[1]);
					break;
				case "VLSMaxDepthToFire":
					this.database.databaseshipdata[i].vlsMaxDepthToFire = float.Parse(array4[1]);
					break;
				case "VLSMaxSpeedToFire":
					this.database.databaseshipdata[i].vlsMaxSpeedToFire = float.Parse(array4[1]) / 10f;
					break;
				case "MissileType":
					this.database.databaseshipdata[i].missileType = this.GetWeaponID(array4[1].Trim());
					this.database.databaseshipdata[i].missileGameObject = this.database.databaseweapondata[this.database.databaseshipdata[i].missileType].weaponObject;
					break;
				case "MissilesPerLauncher":
					this.database.databaseshipdata[i].missilesPerLauncher = this.PopulateIntArray(array4[1]);
					break;
				case "MissileLauncherElevates":
					this.database.databaseshipdata[i].missileLauncherElevates = this.PopulateBoolArray(array4[1]);
					break;
				case "MissileLauncherElevationMin":
					this.database.databaseshipdata[i].missileLauncherElevationMin = this.PopulateFloatArray(array4[1]);
					break;
				case "MissileLauncherElevationMax":
					this.database.databaseshipdata[i].missileLauncherElevationMax = this.PopulateFloatArray(array4[1]);
					break;
				case "NavalGuns":
				{
					string[] array8 = this.PopulateStringArray(array4[1]);
					this.database.databaseshipdata[i].navalGunTypes = new int[array8.Length];
					for (int num7 = 0; num7 < this.database.databaseshipdata[i].navalGunTypes.Length; num7++)
					{
						this.database.databaseshipdata[i].navalGunTypes[num7] = this.GetDepthWeaponID(array8[num7]);
					}
					break;
				}
				case "NavalGunFiringArcBearingMin":
					this.database.databaseshipdata[i].navalGunFiringArcMin = this.PopulateFloatArray(array4[1]);
					break;
				case "NavalGunFiringArcBearingMax":
					this.database.databaseshipdata[i].navalGunFiringArcMax = this.PopulateFloatArray(array4[1]);
					break;
				case "NavalGunRestAngle":
					this.database.databaseshipdata[i].navalGunRestAngle = this.PopulateFloatArray(array4[1]);
					this.database.databaseshipdata[i].rearArcFiring = new bool[this.database.databaseshipdata[i].navalGunRestAngle.Length];
					for (int num8 = 0; num8 < this.database.databaseshipdata[i].navalGunRestAngle.Length; num8++)
					{
						if (this.database.databaseshipdata[i].navalGunRestAngle[num8] == 180f && this.database.databaseshipdata[i].navalGunFiringArcMin[num8] > 0f && this.database.databaseshipdata[i].navalGunFiringArcMax[num8] < 0f)
						{
							this.database.databaseshipdata[i].rearArcFiring[num8] = true;
						}
					}
					break;
				case "NavalGunParticle":
					this.database.databaseshipdata[i].navalGunParticleEffect = Resources.Load<GameObject>(array4[1].Trim());
					break;
				case "NavalGunSmokeParticle":
					this.database.databaseshipdata[i].navalGunSmokeEffect = Resources.Load<GameObject>(array4[1].Trim());
					break;
				case "RBULaunchers":
				{
					string[] array9 = this.PopulateStringArray(array4[1]);
					this.database.databaseshipdata[i].rbuLauncherTypes = new int[array9.Length];
					for (int num9 = 0; num9 < this.database.databaseshipdata[i].rbuLauncherTypes.Length; num9++)
					{
						this.database.databaseshipdata[i].rbuLauncherTypes[num9] = this.GetDepthWeaponID(array9[num9]);
					}
					break;
				}
				case "RBUSalvos":
					this.database.databaseshipdata[i].rbuSalvos = this.PopulateIntArray(array4[1]);
					break;
				case "RBUFiringArcBearingMin":
					this.database.databaseshipdata[i].rbuFiringArcMin = this.PopulateFloatArray(array4[1]);
					break;
				case "RBUFiringArcBearingMax":
					this.database.databaseshipdata[i].rbuFiringArcMax = this.PopulateFloatArray(array4[1]);
					break;
				case "Anti-MissileGunHitProbability":
					this.database.databaseshipdata[i].gunProbability = float.Parse(array4[1]);
					break;
				case "Anti-MissileGunRange":
					this.database.databaseshipdata[i].gunRange = float.Parse(array4[1]);
					break;
				case "Anti-MissileGunFiringArcStart":
					this.database.databaseshipdata[i].gunFiringArcStart = this.PopulateFloatArray(array4[1]);
					break;
				case "Anti-MissileGunFiringArcFinish":
					this.database.databaseshipdata[i].gunFiringArcFinish = this.PopulateFloatArray(array4[1]);
					break;
				case "Anti-MissileGunRestAngle":
					this.database.databaseshipdata[i].gunRestAngle = this.PopulateFloatArray(array4[1]);
					break;
				case "Anti-MissileRADARRestAngle":
					this.database.databaseshipdata[i].gunRadarRestAngles = this.PopulateFloatArray(array4[1]);
					break;
				case "Anti-MissileGunUsesRADAR":
					this.database.databaseshipdata[i].gunUsesRadar = this.PopulateIntArray(array4[1]);
					break;
				case "Anti-MissileGunParticle":
					this.database.databaseshipdata[i].ciwsParticle = array4[1].Trim();
					break;
				case "ChaffType":
					this.database.databaseshipdata[i].chaffID = this.GetCountermeasureID(array4[1].Trim());
					break;
				case "ChaffProbability":
					this.database.databaseshipdata[i].chaffProbability = float.Parse(array4[1]);
					break;
				case "NumberChaffLaunched":
					this.database.databaseshipdata[i].numberChafflaunched = int.Parse(array4[1]);
					break;
				case "NoisemakerName":
					this.database.databaseshipdata[i].noiseMakerID = this.GetCountermeasureID(array4[1].Trim());
					break;
				case "NumberOfNoisemakers":
					this.database.databaseshipdata[i].numberofnoisemakers = int.Parse(array4[1]);
					if (this.database.databaseshipdata[i].numberofnoisemakers > 0)
					{
						this.database.databaseshipdata[i].hasnoisemaker = true;
					}
					break;
				case "NoisemakerReloadTime":
					this.database.databaseshipdata[i].noisemakerreloadtime = float.Parse(array4[1]);
					break;
				case "LabelPosition":
					vector = this.PopulateVector2(array4[1].Trim());
					vector.x -= 517f;
					break;
				case "BOWSONAR":
					this.PopulateSubsystemArray(i, array4[0], array4[1].Trim());
					this.database.databaseshipdata[i].subsystemLabelPositions[DamageControl.GetSubsystemIndex(array4[0].Trim())] = vector;
					break;
				case "TOWED":
					this.PopulateSubsystemArray(i, array4[0], array4[1].Trim());
					this.database.databaseshipdata[i].subsystemLabelPositions[DamageControl.GetSubsystemIndex(array4[0].Trim())] = vector;
					break;
				case "PERISCOPE":
					this.PopulateSubsystemArray(i, array4[0], array4[1].Trim());
					this.database.databaseshipdata[i].subsystemLabelPositions[DamageControl.GetSubsystemIndex(array4[0].Trim())] = vector;
					break;
				case "ESM_MAST":
					this.PopulateSubsystemArray(i, array4[0], array4[1].Trim());
					this.database.databaseshipdata[i].subsystemLabelPositions[DamageControl.GetSubsystemIndex(array4[0].Trim())] = vector;
					break;
				case "RADAR_MAST":
					this.PopulateSubsystemArray(i, array4[0], array4[1].Trim());
					this.database.databaseshipdata[i].subsystemLabelPositions[DamageControl.GetSubsystemIndex(array4[0].Trim())] = vector;
					break;
				case "TUBES":
					this.PopulateSubsystemArray(i, array4[0], array4[1].Trim());
					this.database.databaseshipdata[i].subsystemLabelPositions[DamageControl.GetSubsystemIndex(array4[0].Trim())] = vector;
					break;
				case "FIRECONTROL":
					this.PopulateSubsystemArray(i, array4[0], array4[1].Trim());
					this.database.databaseshipdata[i].subsystemLabelPositions[DamageControl.GetSubsystemIndex(array4[0].Trim())] = vector;
					break;
				case "PUMPS":
					this.PopulateSubsystemArray(i, array4[0], array4[1].Trim());
					this.database.databaseshipdata[i].subsystemLabelPositions[DamageControl.GetSubsystemIndex(array4[0].Trim())] = vector;
					break;
				case "PROPULSION":
					this.PopulateSubsystemArray(i, array4[0], array4[1].Trim());
					this.database.databaseshipdata[i].subsystemLabelPositions[DamageControl.GetSubsystemIndex(array4[0].Trim())] = vector;
					break;
				case "RUDDER":
					this.PopulateSubsystemArray(i, array4[0], array4[1].Trim());
					this.database.databaseshipdata[i].subsystemLabelPositions[DamageControl.GetSubsystemIndex(array4[0].Trim())] = vector;
					break;
				case "PLANES":
					this.PopulateSubsystemArray(i, array4[0], array4[1].Trim());
					this.database.databaseshipdata[i].subsystemLabelPositions[DamageControl.GetSubsystemIndex(array4[0].Trim())] = vector;
					break;
				case "BALLAST":
					this.PopulateSubsystemArray(i, array4[0], array4[1].Trim());
					this.database.databaseshipdata[i].subsystemLabelPositions[DamageControl.GetSubsystemIndex(array4[0].Trim())] = vector;
					break;
				case "REACTOR":
					this.PopulateSubsystemArray(i, array4[0], array4[1].Trim());
					this.database.databaseshipdata[i].subsystemLabelPositions[DamageControl.GetSubsystemIndex(array4[0].Trim())] = vector;
					break;
				case "FLOODING1":
					this.database.databaseshipdata[i].compartmentFloodingRanges = new Vector2[5];
					this.database.databaseshipdata[i].compartmentPositionsAndWidth = new Vector2[5];
					array2 = array4[1].Trim().Split(new char[]
					{
						','
					});
					this.database.databaseshipdata[i].compartmentFloodingRanges[0].x = float.Parse(array2[2]);
					this.database.databaseshipdata[i].compartmentFloodingRanges[0].y = float.Parse(array2[3]);
					this.database.databaseshipdata[i].compartmentPositionsAndWidth[0].x = float.Parse(array2[0]);
					this.database.databaseshipdata[i].compartmentPositionsAndWidth[0].y = float.Parse(array2[1]);
					break;
				case "FLOODING2":
					array2 = array4[1].Trim().Split(new char[]
					{
						','
					});
					this.database.databaseshipdata[i].compartmentFloodingRanges[1].x = float.Parse(array2[2]);
					this.database.databaseshipdata[i].compartmentFloodingRanges[1].y = float.Parse(array2[3]);
					this.database.databaseshipdata[i].compartmentPositionsAndWidth[1].x = float.Parse(array2[0]);
					this.database.databaseshipdata[i].compartmentPositionsAndWidth[1].y = float.Parse(array2[1]);
					break;
				case "FLOODING3":
					array2 = array4[1].Trim().Split(new char[]
					{
						','
					});
					this.database.databaseshipdata[i].compartmentFloodingRanges[2].x = float.Parse(array2[2]);
					this.database.databaseshipdata[i].compartmentFloodingRanges[2].y = float.Parse(array2[3]);
					this.database.databaseshipdata[i].compartmentPositionsAndWidth[2].x = float.Parse(array2[0]);
					this.database.databaseshipdata[i].compartmentPositionsAndWidth[2].y = float.Parse(array2[1]);
					break;
				case "FLOODING4":
					array2 = array4[1].Trim().Split(new char[]
					{
						','
					});
					this.database.databaseshipdata[i].compartmentFloodingRanges[3].x = float.Parse(array2[2]);
					this.database.databaseshipdata[i].compartmentFloodingRanges[3].y = float.Parse(array2[3]);
					this.database.databaseshipdata[i].compartmentPositionsAndWidth[3].x = float.Parse(array2[0]);
					this.database.databaseshipdata[i].compartmentPositionsAndWidth[3].y = float.Parse(array2[1]);
					break;
				case "FLOODING5":
				{
					string[] array10 = array4[1].Trim().Split(new char[]
					{
						','
					});
					this.database.databaseshipdata[i].compartmentFloodingRanges[4].x = float.Parse(array10[2]);
					this.database.databaseshipdata[i].compartmentFloodingRanges[4].y = float.Parse(array10[3]);
					this.database.databaseshipdata[i].compartmentPositionsAndWidth[4].x = float.Parse(array10[0]);
					this.database.databaseshipdata[i].compartmentPositionsAndWidth[4].y = float.Parse(array10[1]);
					break;
				}
				case "DamageControlPartyY":
					this.database.databaseshipdata[i].damageControlPartyY = float.Parse(array4[1]);
					break;
				}
				if (!flag)
				{
					float num10;
					if (this.database.databaseshipdata[i].submergedspeed > this.database.databaseshipdata[i].surfacespeed)
					{
						num10 = this.database.databaseshipdata[i].submergedspeed;
					}
					else
					{
						num10 = this.database.databaseshipdata[i].surfacespeed;
					}
					float num11 = -0.2f;
					for (int num12 = 0; num12 < 7; num12++)
					{
						this.database.databaseshipdata[i].telegraphSpeeds[num12] = num11 * num10 * 0.1f;
						num11 += 0.2f;
					}
					if (this.database.databaseshipdata[i].shipType == "SUBMARINE")
					{
						this.database.databaseshipdata[i].telegraphSpeeds[2] = 0.5f;
						this.database.databaseshipdata[i].telegraphSpeeds[3] = 1f;
					}
				}
			}
		}
	}

	// Token: 0x06000A74 RID: 2676 RVA: 0x00089D60 File Offset: 0x00087F60
	public void ReadVesselDescriptionData(DatabaseShipData currentDatabaseShipData)
	{
		string[] array = this.OpenTextDataFile(this.GetFilePathFromString("language/vessel/" + currentDatabaseShipData.shipPrefabName + "_description"));
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			string text = array2[0];
			switch (text)
			{
			case "ShipClass":
				currentDatabaseShipData.shipclass = array2[1].Trim();
				break;
			case "Description":
				currentDatabaseShipData.description = array2[1].Trim();
				break;
			case "DefensiveWeapons":
				currentDatabaseShipData.defensiveWeapons = this.PopulateMultiLineTextArray(array2[1].Trim());
				break;
			case "OffensiveWeapons":
				currentDatabaseShipData.offensiveWeapons = this.PopulateMultiLineTextArray(array2[1].Trim());
				break;
			case "Sensors":
				currentDatabaseShipData.sensors = this.PopulateMultiLineTextArray(array2[1].Trim());
				break;
			case "History":
				currentDatabaseShipData.history = this.PopulateMultiLineTextArray(array2[1].Trim());
				break;
			case "Aircraft":
				currentDatabaseShipData.aircraftOnBoard = this.PopulateMultiLineTextArray(array2[1].Trim());
				break;
			case "PlayerClassNames":
				currentDatabaseShipData.playerClassNames = this.PopulateStringArray(array2[1].Trim());
				break;
			case "PlayerClassHullNumbers":
				currentDatabaseShipData.playerClassHullNumbers = this.PopulateStringArray(array2[1].Trim());
				break;
			}
		}
	}

	// Token: 0x06000A75 RID: 2677 RVA: 0x00089F68 File Offset: 0x00088168
	public void ReadCampaignData()
	{
		CampaignLocation campaignLocation = null;
		CampaignRecon campaignRecon = null;
		List<CampaignMission> list = new List<CampaignMission>();
		List<CampaignEvent> list2 = new List<CampaignEvent>();
		int num = 0;
		int num2 = 0;
		int labelFont = 0;
		Color32 labelColor = Color.white;
		bool labelOutline = true;
		int labelSize = 15;
		Vector2 labelPosition = Vector2.zero;
		string filePathFromString = this.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/campaign_data");
		string[] array = this.OpenTextDataFile(filePathFromString);
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		this.campaignmanager.eventManager.specialEventIDs = new int[25];
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			string text = array2[0];
			switch (text)
			{
			case "AircraftName":
				num4++;
				break;
			case "SatelliteName":
				num5++;
				break;
			case "Alignment":
				num3++;
				break;
			case "SOSUSName":
				num6++;
				break;
			}
		}
		int num8 = -1;
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j].Trim() == "[Aircraft]")
			{
				this.campaignmanager.campaignaircraft = new CampaignRecon[num4];
				num8 = -1;
			}
			else if (array[j].Trim() == "[Satellites]")
			{
				this.campaignmanager.campaignsatellites = new CampaignRecon[num5];
				num8 = -1;
			}
			else if (array[j].Trim() == "[Locations]")
			{
				this.campaignmanager.campaignlocations = new CampaignLocation[num3];
				num8 = -1;
			}
			else if (array[j].Trim() == "[SOSUS]")
			{
				this.campaignmanager.sosusBarriers = new GameObject[num6];
				this.campaignmanager.sosusNames = new string[num6];
				this.campaignmanager.sosusAlignments = new string[num6];
				this.campaignmanager.sosusStarts = new Vector2[num6];
				this.campaignmanager.sosusEnds = new Vector2[num6];
				this.campaignmanager.sosusDetectionRange = new float[num6];
				this.campaignmanager.sosusAngles = new float[num6];
				num8 = -1;
			}
			else
			{
				string[] array3 = array[j].Split(new char[]
				{
					'='
				});
				string text = array3[0].Trim();
				switch (text)
				{
				case "MapImage":
					this.campaignmanager.mapSprite.sprite = this.GetSprite(this.GetFilePathFromString(array3[1].Trim()));
					break;
				case "MapNavigationData":
					this.campaignmanager.mapNavigation = this.GetTexture(this.GetFilePathFromString(array3[1].Trim()));
					break;
				case "MapElevationData":
					this.campaignmanager.elevationMapDataPath = array3[1].Trim();
					break;
				case "WorldObjectsData":
					UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.worldObjectsData = array3[1].Trim();
					break;
				case "VesselsAndTraffic":
					this.ReadCivilianTrafficData(this.GetFilePathFromString(array3[1].Trim()));
					break;
				case "MapHeightInNM":
					this.campaignmanager.mapHeightInNM = float.Parse(array3[1]);
					break;
				case "MapTimeCompression":
					this.campaignmanager.mapTimeCompression = float.Parse(array3[1]);
					if (this.campaignmanager.mapTimeCompression <= 1f)
					{
						this.campaignmanager.mapTimeCompression = 1f;
					}
					this.campaignmanager.mapTimeCompression = 3600f / this.campaignmanager.mapTimeCompression;
					break;
				case "Hemisphere":
					this.campaignmanager.hemisphere = array3[1].Trim();
					break;
				case "EquatorYValue":
					this.campaignmanager.equatorYValue = float.Parse(array3[1]);
					break;
				case "JulianStartDate":
					this.campaignmanager.julianStartDay = float.Parse(array3[1]);
					break;
				case "StartDateRange":
					this.campaignmanager.dayRange = float.Parse(array3[1]);
					break;
				case "HoursToNextGeneralEvent":
					this.campaignmanager.hoursPerGeneralEvent = float.Parse(array3[1]);
					break;
				case "HoursToNextGeneralEventRange":
					this.campaignmanager.hoursPerGeneralEventRange = this.PopulateVector2(array3[1]);
					break;
				case "CampaignPoints":
					this.campaignmanager.totalCampaignPoints = float.Parse(array3[1]);
					break;
				case "CampaignStartPoints":
					this.campaignmanager.campaignPoints = float.Parse(array3[1]);
					this.campaignmanager.campaignPoints *= OptionsManager.difficultySettings["StartPointsModifier"];
					break;
				case "PlayerPositionOnLeavePort":
					this.campaignmanager.playerPositionOnLeavePort = this.PopulateVector2(array3[1]);
					break;
				case "SpritePlayer":
					this.campaignmanager.reconImages[1] = this.GetSprite(this.GetFilePathFromString(array3[1].Trim()));
					break;
				case "SpriteEnemySurfaceForce":
					this.campaignmanager.reconImages[2] = this.GetSprite(this.GetFilePathFromString(array3[1].Trim()));
					break;
				case "SpriteEnemySubmarineForce":
					this.campaignmanager.reconImages[3] = this.GetSprite(this.GetFilePathFromString(array3[1].Trim()));
					break;
				case "SpriteSatellite":
					this.campaignmanager.reconImages[0] = this.GetSprite(this.GetFilePathFromString(array3[1].Trim()));
					break;
				case "SpritePlayerOccupiedZone":
					this.campaignmanager.occupiedZoneSprite[0] = this.GetSprite(this.GetFilePathFromString(array3[1].Trim()));
					break;
				case "SpriteEnemyOccupiedZone":
					this.campaignmanager.occupiedZoneSprite[1] = this.GetSprite(this.GetFilePathFromString(array3[1].Trim()));
					break;
				case "ToolbarTextColor":
					this.campaignmanager.dateDisplay.color = this.GetColor32(array3[1].Trim());
					this.campaignmanager.timeDisplay.color = this.GetColor32(array3[1].Trim());
					UIFunctions.languagemanager.campaignTextFields[0].color = this.GetColor32(array3[1].Trim());
					UIFunctions.languagemanager.campaignTextFields[1].color = this.GetColor32(array3[1].Trim());
					break;
				case "ToolbarBackground":
					this.campaignmanager.strategicMapToolbar.GetComponent<Image>().sprite = this.GetSprite(this.GetFilePathFromString(array3[1].Trim()));
					break;
				case "PortIcon":
					this.campaignmanager.straticMapEnterPortButton.GetComponent<Image>().sprite = this.GetSprite(this.GetFilePathFromString(array3[1].Trim()));
					break;
				case "SosusBarrier":
					this.campaignmanager.sosusBarrierGameObject.GetComponent<Image>().sprite = this.GetSprite(this.GetFilePathFromString(array3[1].Trim()));
					break;
				case "PlayerIconColor":
					this.campaignmanager.playerIconColor = this.GetColor32(array3[1].Trim());
					break;
				case "EnemyIconColor":
					this.campaignmanager.enemyIconColor = this.GetColor32(array3[1].Trim());
					break;
				case "ContactOverTimeColor1":
					this.campaignmanager.contactColorsOverTime[0] = this.GetColor32(array3[1].Trim());
					break;
				case "ContactOverTimeColor2":
					this.campaignmanager.contactColorsOverTime[1] = this.GetColor32(array3[1].Trim());
					break;
				case "ContactOverTimeColor3":
					this.campaignmanager.contactColorsOverTime[2] = this.GetColor32(array3[1].Trim());
					break;
				case "ContactOverTimeColor4":
					this.campaignmanager.contactColorsOverTime[3] = this.GetColor32(array3[1].Trim());
					break;
				case "UseLandWar":
					if (array3[1].Trim() == "TRUE")
					{
						this.campaignmanager.useLandWar = true;
					}
					else
					{
						this.campaignmanager.useLandWar = false;
					}
					break;
				case "IconsInOccupiedZonesOnly":
					if (array3[1].Trim() == "TRUE")
					{
						this.campaignmanager.iconsInOccupiedOnly = true;
					}
					else
					{
						this.campaignmanager.iconsInOccupiedOnly = false;
					}
					break;
				case "FirstOccupiedTerritory":
				{
					string[] array4 = this.PopulateStringArray(array3[1].Trim());
					this.campaignmanager.firstOccupiedTerritory = new int[array4.Length];
					for (int k = 0; k < this.campaignmanager.firstOccupiedTerritory.Length; k++)
					{
						this.campaignmanager.firstOccupiedTerritory[k] = this.GetSingleCampaignRegionWaypointID(array4[k]);
					}
					break;
				}
				case "TerritoryTakebackThreshold":
					this.campaignmanager.territoryTakebackThreshold = int.Parse(array3[1]);
					break;
				case "NewFrontProbability":
					this.campaignmanager.newFrontChance = float.Parse(array3[1]);
					this.campaignmanager.newFrontChance = Mathf.Clamp01(this.campaignmanager.newFrontChance);
					break;
				case "PlayerStartTelegraphs":
					this.campaignmanager.playerStartTelegraphs = this.PopulateIntArray(array3[1].Trim());
					break;
				case "PlayerStartDepths":
					this.campaignmanager.playerStartDepths = this.PopulateFloatArray(array3[1].Trim());
					break;
				case "MapSpeedModifier":
					this.campaignmanager.mapSpeedModifier = float.Parse(array3[1]);
					break;
				case "DisruptTime":
				{
					float[] array5 = this.PopulateFloatArray(array3[1].Trim());
					this.campaignmanager.disruptTimes.x = array5[0];
					this.campaignmanager.disruptTimes.y = array5[1];
					break;
				}
				case "TimePenaltyOnPortEnter":
					this.campaignmanager.timePenaltyOnPortEnter = float.Parse(array3[1]);
					break;
				case "PortImage":
					this.campaignmanager.portImage = array3[1].Trim();
					break;
				case "TransitImage":
					this.campaignmanager.transitImage = array3[1].Trim();
					break;
				case "TimePenaltyOnSunk":
					this.campaignmanager.timePenaltyOnSunk = float.Parse(array3[1]);
					break;
				case "Alignment":
					num8++;
					campaignLocation = ScriptableObject.CreateInstance<CampaignLocation>();
					this.campaignmanager.campaignlocations[num8] = campaignLocation;
					campaignLocation.locationID = num8;
					campaignLocation.function = new List<string>();
					campaignLocation.missionTypes = new List<string>();
					campaignLocation.faction = array3[1].Trim();
					campaignLocation.originalFaction = array3[1].Trim();
					campaignLocation.linksToRegionWaypoint = -1;
					break;
				case "Function":
					campaignLocation.function = this.PopulateStringList(array3[1].Trim());
					break;
				case "BaseMapPosition":
					campaignLocation.baseLocation = this.PopulateVector2(array3[1]);
					break;
				case "AircraftType":
					campaignLocation.aircraftType = this.GetCampaignAircraftID(array3[1].Trim());
					break;
				case "AircraftTypeInvaded":
					campaignLocation.aircraftTypeInvaded = this.GetCampaignAircraftID(array3[1].Trim());
					break;
				case "AircraftPrepTime":
					campaignLocation.aircraftFrequency = float.Parse(array3[1]);
					break;
				case "AircraftHeadings":
					if (array3[1].Contains("SOUTH"))
					{
						string[] array6 = array3[1].Split(new char[]
						{
							','
						});
						campaignLocation.aircraftHeadings.x = float.Parse(array6[0]);
						campaignLocation.aircraftHeadings.y = float.Parse(array6[1]);
						campaignLocation.aircraftHeadings.x = 360f - campaignLocation.aircraftHeadings.x;
						campaignLocation.aircraftHeadings.y = 360f - campaignLocation.aircraftHeadings.y;
						Vector2 aircraftHeadings = campaignLocation.aircraftHeadings;
						campaignLocation.aircraftHeadings.x = aircraftHeadings.y;
						campaignLocation.aircraftHeadings.y = aircraftHeadings.x;
					}
					else
					{
						campaignLocation.aircraftHeadings = this.PopulateVector2(array3[1]);
						if (campaignLocation.aircraftHeadings.x <= 180f)
						{
							CampaignLocation campaignLocation2 = campaignLocation;
							campaignLocation2.aircraftHeadings.x = campaignLocation2.aircraftHeadings.x * -1f;
						}
						if (campaignLocation.aircraftHeadings.y <= 180f)
						{
							CampaignLocation campaignLocation3 = campaignLocation;
							campaignLocation3.aircraftHeadings.y = campaignLocation3.aircraftHeadings.y * -1f;
						}
						if (campaignLocation.aircraftHeadings.x > 180f)
						{
							campaignLocation.aircraftHeadings.x = 360f - campaignLocation.aircraftHeadings.x;
						}
						if (campaignLocation.aircraftHeadings.y > 180f)
						{
							campaignLocation.aircraftHeadings.y = 360f - campaignLocation.aircraftHeadings.y;
						}
						Vector2 aircraftHeadings2 = campaignLocation.aircraftHeadings;
						campaignLocation.aircraftHeadings.x = aircraftHeadings2.y;
						campaignLocation.aircraftHeadings.y = aircraftHeadings2.x;
					}
					break;
				case "AircraftSearchRange":
					campaignLocation.airSearchRange = float.Parse(array3[1]);
					campaignLocation.airSearchRange *= 780f / this.campaignmanager.mapHeightInNM * 0.2567f;
					break;
				case "MissionTypes":
					campaignLocation.missionTypes = this.PopulateStringList(array3[1].Trim());
					break;
				case "LinksToWaypoint":
					campaignLocation.linksToWaypoint = this.GetCampaignMapWaypointIDs(this.PopulateStringArray(array3[1].Trim()));
					break;
				case "LinksToRegionWaypoint":
					campaignLocation.linksToRegionWaypoint = this.GetSingleCampaignRegionWaypointID(array3[1].Trim());
					break;
				case "RelatedSOSUS":
					campaignLocation.linksToSosus = this.PopulateStringArray(array3[1].Trim());
					break;
				case "CombatCoords":
					campaignLocation.combatCoords = this.PopulateVector2(array3[1]);
					break;
				case "AircraftName":
					num8++;
					campaignRecon = ScriptableObject.CreateInstance<CampaignRecon>();
					this.campaignmanager.campaignaircraft[num8] = campaignRecon;
					campaignRecon.reconName = array3[1].Trim();
					break;
				case "AircraftFaction":
					campaignRecon.faction = array3[1].Trim();
					break;
				case "AircraftPatrolSpeed":
					campaignRecon.speed = float.Parse(array3[1]);
					campaignRecon.speed /= 10f;
					break;
				case "AircraftPatrolRange":
					campaignRecon.patrolRange = float.Parse(array3[1]);
					campaignRecon.patrolRange *= 780f / this.campaignmanager.mapHeightInNM * 0.2567f;
					break;
				case "AircraftDetectionRange":
					campaignRecon.detectionRange = float.Parse(array3[1]);
					break;
				case "AircraftSprite":
					campaignRecon.reconSprite = this.GetSprite(this.GetFilePathFromString(array3[1].Trim()));
					break;
				case "SatelliteName":
					num8++;
					campaignRecon = ScriptableObject.CreateInstance<CampaignRecon>();
					this.campaignmanager.campaignsatellites[num8] = campaignRecon;
					campaignRecon.reconName = array3[1].Trim();
					break;
				case "SatelliteFaction":
					campaignRecon.faction = array3[1].Trim();
					break;
				case "SatelliteSpeed":
					campaignRecon.speed = float.Parse(array3[1]);
					campaignRecon.speed /= 140f;
					campaignRecon.speed /= 2f;
					break;
				case "DetectionRange":
					campaignRecon.detectionRange = float.Parse(array3[1]);
					break;
				case "SOSUSName":
					num8++;
					this.campaignmanager.sosusNames[num8] = array3[1].Trim();
					break;
				case "SOSUSDetectionRange":
					this.campaignmanager.sosusDetectionRange[num8] = float.Parse(array3[1].Trim());
					break;
				case "SOSUSAngle":
					this.campaignmanager.sosusAngles[num8] = float.Parse(array3[1].Trim());
					break;
				case "SOSUSStartPosition":
					this.campaignmanager.sosusStarts[num8] = this.PopulateVector2(array3[1].Trim());
					break;
				case "SOSUSEndPosition":
					this.campaignmanager.sosusEnds[num8] = this.PopulateVector2(array3[1].Trim());
					break;
				case "SOSUSAlignment":
					this.campaignmanager.sosusAlignments[num8] = array3[1].Trim();
					break;
				case "PlayerMissionTypes":
					this.campaignmanager.campaignMissionTypes = this.PopulateStringArray(array3[1].Trim());
					this.campaignmanager.numberofPlayerMissionTypes = this.campaignmanager.campaignMissionTypes.Length;
					break;
				case "PlayerMissionFrequency":
					this.campaignmanager.playerMissionFreqs = this.PopulateFloatArray(array3[1].Trim());
					this.campaignmanager.playerMissionThresholds = new float[this.campaignmanager.playerMissionFreqs.Length];
					break;
				case "PlayerMissionThreshold":
					this.campaignmanager.playerMissionThresholds = this.PopulateFloatArray(array3[1].Trim());
					break;
				case "CommandoLoadTime":
					this.campaignmanager.commandoLoadTime = float.Parse(array3[1].Trim());
					break;
				case "DefaultMissionTypes":
					this.campaignmanager.defaultMissionTypes = this.PopulateStringArray(array3[1].Trim());
					break;
				case "[PLAYER MISSIONS]":
					num8 = 0;
					break;
				case "Mission":
				{
					CampaignMission campaignMission = ScriptableObject.CreateInstance<CampaignMission>();
					campaignMission = this.GetMissionData(campaignMission, array3[1].Trim());
					campaignMission.missionID = num8;
					list.Add(campaignMission);
					num8++;
					break;
				}
				case "Non-PlayerMissionTypes":
				{
					string[] array7 = this.PopulateStringArray(array3[1].Trim());
					this.campaignmanager.numberofNonPlayerMissionTypes = array7.Length;
					string[] array8 = new string[this.campaignmanager.campaignMissionTypes.Length + array7.Length];
					for (int l = 0; l < this.campaignmanager.campaignMissionTypes.Length; l++)
					{
						array8[l] = this.campaignmanager.campaignMissionTypes[l];
					}
					for (int m = 0; m < array7.Length; m++)
					{
						array8[m + this.campaignmanager.campaignMissionTypes.Length] = array7[m];
					}
					this.campaignmanager.campaignMissionTypes = array8;
					break;
				}
				case "Non-PlayerMissionFrequency":
					this.campaignmanager.nonPlayerMissionFreqs = this.PopulateFloatArray(array3[1].Trim());
					break;
				case "Non-PlayerMission":
				{
					CampaignMission campaignMission = ScriptableObject.CreateInstance<CampaignMission>();
					campaignMission = this.GetMissionData(campaignMission, array3[1].Trim());
					campaignMission.missionID = num8;
					list.Add(campaignMission);
					num++;
					num8++;
					break;
				}
				case "[EVENTS]":
					num8 = 0;
					break;
				case "Event":
				{
					CampaignEvent campaignEvent = ScriptableObject.CreateInstance<CampaignEvent>();
					campaignEvent = this.GetEventData(campaignEvent, array3[1].Trim());
					campaignEvent.eventID = num8;
					list2.Add(campaignEvent);
					if (array3.Length == 3)
					{
						array3[2] = array3[2].Trim();
						if (array3[2] == "START")
						{
							this.campaignmanager.eventManager.specialEventIDs[0] = num8;
						}
						else if (array3[2] == "WIN")
						{
							this.campaignmanager.eventManager.specialEventIDs[1] = num8;
						}
						else if (array3[2] == "DRAW")
						{
							this.campaignmanager.eventManager.specialEventIDs[2] = num8;
						}
						else if (array3[2] == "FAIL")
						{
							this.campaignmanager.eventManager.specialEventIDs[3] = num8;
						}
						else if (array3[2] == "CRITICAL_FAIL")
						{
							this.campaignmanager.eventManager.specialEventIDs[4] = num8;
						}
						else if (array3[2] == "RESCUE")
						{
							this.campaignmanager.eventManager.specialEventIDs[5] = num8;
						}
						else if (array3[2] == "CAPTURE")
						{
							this.campaignmanager.eventManager.specialEventIDs[6] = num8;
						}
						else if (array3[2] == "LOST_AT_SEA")
						{
							this.campaignmanager.eventManager.specialEventIDs[7] = num8;
						}
						else if (array3[2] == "ARMISTICE_ADVANTAGE")
						{
							this.campaignmanager.eventManager.specialEventIDs[8] = num8;
						}
						else if (array3[2] == "ARMISTICE_DISADVANTAGE")
						{
							this.campaignmanager.eventManager.specialEventIDs[9] = num8;
						}
						else if (array3[2] == "IMPASSE")
						{
							this.campaignmanager.eventManager.specialEventIDs[10] = num8;
						}
						else if (array3[2] == "PLAYER_INVASION_SEA")
						{
							this.campaignmanager.eventManager.specialEventIDs[11] = num8;
						}
						else if (array3[2] == "PLAYER_INVASION_LAND")
						{
							this.campaignmanager.eventManager.specialEventIDs[12] = num8;
						}
						else if (array3[2] == "PLAYER_INVASION_AIR")
						{
							this.campaignmanager.eventManager.specialEventIDs[13] = num8;
						}
						else if (array3[2] == "ENEMY_INVASION_SEA")
						{
							this.campaignmanager.eventManager.specialEventIDs[14] = num8;
						}
						else if (array3[2] == "ENEMY_INVASION_LAND")
						{
							this.campaignmanager.eventManager.specialEventIDs[15] = num8;
						}
						else if (array3[2] == "ENEMY_INVASION_AIR")
						{
							this.campaignmanager.eventManager.specialEventIDs[16] = num8;
						}
						else if (array3[2] == "PLAYER_LIBERATION_SEA")
						{
							this.campaignmanager.eventManager.specialEventIDs[17] = num8;
						}
						else if (array3[2] == "PLAYER_LIBERATION_LAND")
						{
							this.campaignmanager.eventManager.specialEventIDs[18] = num8;
						}
						else if (array3[2] == "PLAYER_LIBERATION_AIR")
						{
							this.campaignmanager.eventManager.specialEventIDs[19] = num8;
						}
						else if (array3[2] == "ENEMY_LIBERATION_SEA")
						{
							this.campaignmanager.eventManager.specialEventIDs[20] = num8;
						}
						else if (array3[2] == "ENEMY_LIBERATION_LAND")
						{
							this.campaignmanager.eventManager.specialEventIDs[21] = num8;
						}
						else if (array3[2] == "ENEMY_LIBERATION_AIR")
						{
							this.campaignmanager.eventManager.specialEventIDs[22] = num8;
						}
						else if (array3[2] == "STATISTICS")
						{
							this.campaignmanager.eventManager.specialEventIDs[23] = num8;
						}
						else if (array3[2] == "COURT_MARTIAL")
						{
							this.campaignmanager.eventManager.specialEventIDs[24] = num8;
						}
					}
					num8++;
					break;
				}
				}
			}
		}
		filePathFromString = this.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/campaign_map_data");
		array = this.OpenTextDataFile(filePathFromString);
		num8 = 0;
		for (int n = 0; n < array.Length; n++)
		{
			string[] array9 = array[n].Split(new char[]
			{
				'='
			});
			string text = array9[0].Trim();
			switch (text)
			{
			case "LocationName":
				this.campaignmanager.campaignlocations[num8].locationName = array9[1].Trim();
				this.campaignmanager.campaignlocations[num8].country = array9[2].Trim();
				num8++;
				break;
			case "Outline":
				labelOutline = (array9[1].Trim() == "TRUE");
				break;
			case "Size":
				labelSize = int.Parse(array9[1].Trim());
				break;
			case "Font":
				labelFont = int.Parse(array9[1].Trim());
				break;
			case "Color":
				labelColor = this.GetColor32(array9[1].Trim());
				break;
			case "LabelPosition":
				labelPosition = this.PopulateVector2(array9[1].Trim());
				break;
			case "LabelName":
				this.BuildMapLabel(array9[1].Trim(), labelColor, labelSize, labelFont, labelPosition, labelOutline);
				break;
			case "LocationMarkerPosition":
				this.BuildMapMarker(labelColor, labelSize, this.PopulateVector2(array9[1].Trim()), labelOutline);
				num2++;
				break;
			}
		}
		this.campaignmanager.campaignmissions = list.ToArray();
		this.campaignmanager.numberOfPlayerMissions = this.campaignmanager.campaignmissions.Length - num;
		this.campaignmanager.numberOfNonPlayerMissions = num;
		this.campaignmanager.eventManager.campaignevents = list2.ToArray();
	}

	// Token: 0x06000A76 RID: 2678 RVA: 0x0008BEA4 File Offset: 0x0008A0A4
	private void BuildMapLabel(string labelname, Color32 labelColor, int labelSize, int labelFont, Vector2 labelPosition, bool labelOutline)
	{
		Text text = UnityEngine.Object.Instantiate<Text>(this.campaignmanager.campaignMapLabel);
		labelname = labelname.Replace("\\n", "\n");
		text.text = labelname;
		text.fontSize = labelSize;
		text.font = this.fonts[labelFont];
		text.color = labelColor;
		text.transform.SetParent(this.campaignmanager.labelsLayer.transform, false);
		text.transform.localPosition = new Vector3(labelPosition.x, labelPosition.y, 0f);
		if (!labelOutline)
		{
			Outline component = text.GetComponent<Outline>();
			component.enabled = false;
		}
	}

	// Token: 0x06000A77 RID: 2679 RVA: 0x0008BF54 File Offset: 0x0008A154
	private void BuildMapMarker(Color32 labelColor, int labelSize, Vector2 labelPosition, bool labelOutline)
	{
		Image image = UnityEngine.Object.Instantiate<Image>(this.campaignmanager.campaignMapLocation);
		image.color = labelColor;
		image.transform.SetParent(this.campaignmanager.labelsLayer.transform, false);
		image.transform.localPosition = new Vector3(labelPosition.x, labelPosition.y, 0f);
		if (!labelOutline)
		{
			Outline component = image.GetComponent<Outline>();
			component.enabled = false;
		}
		image.GetComponent<RectTransform>().sizeDelta = new Vector2((float)labelSize, (float)labelSize);
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x0008BFE8 File Offset: 0x0008A1E8
	private CampaignMission GetMissionData(CampaignMission campaignMission, string filename)
	{
		string filePathFromString = this.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/missions/" + filename);
		string[] array = this.OpenTextDataFile(filePathFromString);
		campaignMission.requiresWeaponID = -1;
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			string text = array2[0];
			switch (text)
			{
			case "MissionType":
				campaignMission.missionType = array2[1].Trim();
				if (campaignMission.missionType == this.campaignmanager.campaignMissionTypes[this.campaignmanager.numberofPlayerMissionTypes - 1])
				{
					campaignMission.finalMission = true;
				}
				campaignMission.missionFileName = filename;
				break;
			case "HoursToStart":
				campaignMission.hoursToStart = this.PopulateVector2(array2[1].Trim());
				break;
			case "StartLocation":
				campaignMission.startLocation = this.PopulateStringArray(array2[1].Trim());
				break;
			case "StartAlignment":
				campaignMission.startAlignment = array2[1].Trim();
				break;
			case "EndLocation":
				campaignMission.endLocation = this.PopulateStringArray(array2[1].Trim());
				break;
			case "EndAlignment":
				campaignMission.endAlignment = array2[1].Trim();
				break;
			case "Speed":
				campaignMission.speed = float.Parse(array2[1].Trim());
				break;
			case "MustUseWaypoints":
				campaignMission.mustUseWaypoints = this.GetCampaignMapWaypointIDs(this.PopulateStringArray(array2[1].Trim()));
				if (campaignMission.mustUseWaypoints.Length == 1 && campaignMission.mustUseWaypoints[0] == -1)
				{
					campaignMission.mustUseWaypoints = new int[0];
				}
				break;
			case "UseAtLeastOneWaypointOf":
				campaignMission.useAtLeastOneWaypointOf = this.GetCampaignMapWaypointIDs(this.PopulateStringArray(array2[1].Trim()));
				if (campaignMission.useAtLeastOneWaypointOf.Length == 1 && campaignMission.useAtLeastOneWaypointOf[0] == -1)
				{
					campaignMission.useAtLeastOneWaypointOf = new int[0];
				}
				break;
			case "ProhibitedWaypoints":
				campaignMission.prohibitedWaypoints = this.GetCampaignMapWaypointIDs(this.PopulateStringArray(array2[1].Trim()));
				if (campaignMission.prohibitedWaypoints.Length == 1 && campaignMission.prohibitedWaypoints[0] == -1)
				{
					campaignMission.prohibitedWaypoints = new int[0];
				}
				break;
			case "PatrolForHours":
				if (array2[1].Contains(','))
				{
					campaignMission.patrolForHours = this.PopulateVector2(array2[1].Trim());
				}
				break;
			case "StrategicValue":
				campaignMission.strategicValue = float.Parse(array2[1].Trim());
				break;
			case "MissionEndsWhen":
				campaignMission.missionEndsWhen = array2[1].Trim();
				break;
			case "DisruptOnFail":
				campaignMission.disruptOnFail = this.SetBoolean(array2[1].Trim());
				break;
			case "DisruptOnPass":
				campaignMission.disruptOnPass = this.SetBoolean(array2[1].Trim());
				break;
			case "InvadeOnFail":
				campaignMission.invadeOnFail = this.SetBoolean(array2[1].Trim());
				break;
			case "RequiresStealth":
				campaignMission.requiresStealth = this.SetBoolean(array2[1].Trim());
				break;
			case "RequiresWeapon":
			{
				string[] array3 = array2[1].Split(new char[]
				{
					','
				});
				if (array3.Length == 3)
				{
					campaignMission.requiresWeaponID = this.GetWeaponID(array3[0].Trim());
					campaignMission.numberOfRequiredWeapon = int.Parse(array3[1].Trim());
					campaignMission.numberOfRequiredWeaponWord = array3[2].Trim();
				}
				break;
			}
			case "NumberOfEnemyUnits":
				if (array2[1].Trim() != "NONE")
				{
					string[] array4 = this.PopulateStringArray(array2[1].Trim());
					campaignMission.numberOfEnemyUnits = new Vector2[array4.Length];
					for (int j = 0; j < array4.Length; j++)
					{
						string[] array5 = array4[j].Split(new char[]
						{
							'-'
						});
						campaignMission.numberOfEnemyUnits[j].x = float.Parse(array5[0]);
						campaignMission.numberOfEnemyUnits[j].y = float.Parse(array5[1]);
					}
				}
				break;
			case "EnemyUnitMissionCritical":
				if (array2[1].Trim() != "NONE")
				{
					campaignMission.enemyUnitMissionCritical = this.PopulateBoolArray(array2[1].Trim());
				}
				break;
			case "EnemyShipClasses":
				if (array2[1].Trim() != "NONE")
				{
					campaignMission.enemyShipClasses = this.PopulateStringArray(array2[1].Trim());
				}
				break;
			case "MapBehaviour":
				campaignMission.taskForceBehaviour = array2[1].Trim();
				break;
			case "CombatBehaviour":
				campaignMission.enemyShipBehaviour = this.PopulateStringArray(array2[1].Trim());
				break;
			case "EventWin":
				campaignMission.eventWin = array2[1].Trim();
				break;
			case "EventFail":
				campaignMission.eventFail = array2[1].Trim();
				break;
			}
		}
		return campaignMission;
	}

	// Token: 0x06000A79 RID: 2681 RVA: 0x0008C658 File Offset: 0x0008A858
	public CampaignEvent GetEventData(CampaignEvent campaignEvent, string filename)
	{
		campaignEvent.eventFilename = filename;
		string filePathFromString = this.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/" + filename);
		string[] array = this.OpenTextDataFile(filePathFromString);
		bool flag = false;
		string str = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Trim() == "[EVENT TEXT]")
			{
				flag = true;
			}
			else if (flag)
			{
				str = str + array[i] + "#";
			}
			else
			{
				string[] array2 = array[i].Split(new char[]
				{
					'='
				});
				string text = array2[0];
				if (text != null)
				{
					if (TextParser.<>f__switch$map37 == null)
					{
						TextParser.<>f__switch$map37 = new Dictionary<string, int>(1)
						{
							{
								"NextAction",
								0
							}
						};
					}
					int num;
					if (TextParser.<>f__switch$map37.TryGetValue(text, out num))
					{
						if (num == 0)
						{
							campaignEvent.nextAction = array2[1].Trim();
						}
					}
				}
			}
		}
		return campaignEvent;
	}

	// Token: 0x06000A7A RID: 2682 RVA: 0x0008C768 File Offset: 0x0008A968
	public void ReadCampaignWaypointData()
	{
		string filePathFromString = this.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/waypoints_sea");
		string[] array = this.OpenTextDataFile(filePathFromString);
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			string text = array2[0];
			if (text != null)
			{
				if (TextParser.<>f__switch$map38 == null)
				{
					TextParser.<>f__switch$map38 = new Dictionary<string, int>(1)
					{
						{
							"WaypointName",
							0
						}
					};
				}
				int num2;
				if (TextParser.<>f__switch$map38.TryGetValue(text, out num2))
				{
					if (num2 == 0)
					{
						num++;
					}
				}
			}
		}
		this.campaignmanager.campaignmapwaypoints = new CampaignMapWaypoint[num];
		CampaignMapWaypoint campaignMapWaypoint = null;
		int num3 = -1;
		for (int j = 0; j < array.Length; j++)
		{
			string[] array3 = array[j].Split(new char[]
			{
				'='
			});
			string text = array3[0];
			switch (text)
			{
			case "WaypointName":
				num3++;
				campaignMapWaypoint = ScriptableObject.CreateInstance<CampaignMapWaypoint>();
				this.campaignmanager.campaignmapwaypoints[num3] = campaignMapWaypoint;
				campaignMapWaypoint.waypointName = array3[1].Trim();
				campaignMapWaypoint.waypointID = num3;
				campaignMapWaypoint.northWaypoints = new int[0];
				campaignMapWaypoint.southWaypoints = new int[0];
				campaignMapWaypoint.eastWaypoints = new int[0];
				campaignMapWaypoint.westWaypoints = new int[0];
				break;
			case "WaypointPosition":
				campaignMapWaypoint.waypointPosition = this.PopulateVector3(array3[1].Trim());
				campaignMapWaypoint.waypointRadius = campaignMapWaypoint.waypointPosition.z / 2f;
				campaignMapWaypoint.waypointPosition.z = -5f;
				break;
			case "SubmarineOnly":
				campaignMapWaypoint.submarineOnly = this.SetBoolean(array3[1].Trim());
				break;
			}
		}
		num3 = -1;
		for (int k = 0; k < array.Length; k++)
		{
			string[] array4 = array[k].Split(new char[]
			{
				'='
			});
			string text = array4[0];
			switch (text)
			{
			case "WaypointName":
				num3++;
				campaignMapWaypoint = this.campaignmanager.campaignmapwaypoints[num3];
				break;
			case "NorthWaypoints":
				if (array4.Length > 0)
				{
					campaignMapWaypoint.northWaypointsNames = this.PopulateStringArray(array4[1].Trim());
					campaignMapWaypoint.northWaypoints = this.GetCampaignMapWaypointIDs(campaignMapWaypoint.northWaypointsNames);
				}
				break;
			case "SouthWaypoints":
				if (array4.Length > 0)
				{
					campaignMapWaypoint.southWaypointsNames = this.PopulateStringArray(array4[1].Trim());
					campaignMapWaypoint.southWaypoints = this.GetCampaignMapWaypointIDs(campaignMapWaypoint.southWaypointsNames);
				}
				break;
			case "EastWaypoints":
				if (array4.Length > 0)
				{
					campaignMapWaypoint.eastWaypointsNames = this.PopulateStringArray(array4[1].Trim());
					campaignMapWaypoint.eastWaypoints = this.GetCampaignMapWaypointIDs(campaignMapWaypoint.eastWaypointsNames);
				}
				break;
			case "WestWaypoints":
				if (array4.Length > 0)
				{
					campaignMapWaypoint.westWaypointsNames = this.PopulateStringArray(array4[1].Trim());
					campaignMapWaypoint.westWaypoints = this.GetCampaignMapWaypointIDs(campaignMapWaypoint.westWaypointsNames);
				}
				break;
			}
		}
		filePathFromString = this.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/waypoints_sea_names");
		array = this.OpenTextDataFile(filePathFromString);
		for (int l = 0; l < array.Length; l++)
		{
			this.campaignmanager.campaignmapwaypoints[l].waypointDescriptiveName = array[l].Trim();
		}
		if (this.campaignmanager.campaignDebugMode)
		{
			string text2 = "Error: " + filePathFromString + "\n";
			bool flag = false;
			for (int m = 0; m < this.campaignmanager.campaignmapwaypoints.Length; m++)
			{
				for (int n = 0; n < this.campaignmanager.campaignmapwaypoints[m].northWaypoints.Length; n++)
				{
					if (!this.isNumberInArray(this.campaignmanager.campaignmapwaypoints[m].waypointID, this.campaignmanager.campaignmapwaypoints[this.campaignmanager.campaignmapwaypoints[m].northWaypoints[n]].southWaypoints))
					{
						string text = text2;
						text2 = string.Concat(new string[]
						{
							text,
							this.campaignmanager.campaignmapwaypoints[m].waypointName,
							" not set as South of  ",
							this.campaignmanager.campaignmapwaypoints[this.campaignmanager.campaignmapwaypoints[m].northWaypoints[n]].waypointName,
							"\n"
						});
						flag = true;
					}
				}
				for (int num4 = 0; num4 < this.campaignmanager.campaignmapwaypoints[m].southWaypoints.Length; num4++)
				{
					if (!this.isNumberInArray(this.campaignmanager.campaignmapwaypoints[m].waypointID, this.campaignmanager.campaignmapwaypoints[this.campaignmanager.campaignmapwaypoints[m].southWaypoints[num4]].northWaypoints))
					{
						string text = text2;
						text2 = string.Concat(new string[]
						{
							text,
							this.campaignmanager.campaignmapwaypoints[m].waypointName,
							" not set as North of  ",
							this.campaignmanager.campaignmapwaypoints[this.campaignmanager.campaignmapwaypoints[m].southWaypoints[num4]].waypointName,
							"\n"
						});
						flag = true;
					}
				}
				for (int num5 = 0; num5 < this.campaignmanager.campaignmapwaypoints[m].eastWaypoints.Length; num5++)
				{
					if (!this.isNumberInArray(this.campaignmanager.campaignmapwaypoints[m].waypointID, this.campaignmanager.campaignmapwaypoints[this.campaignmanager.campaignmapwaypoints[m].eastWaypoints[num5]].westWaypoints))
					{
						string text = text2;
						text2 = string.Concat(new string[]
						{
							text,
							this.campaignmanager.campaignmapwaypoints[m].waypointName,
							" not set as WEST of  ",
							this.campaignmanager.campaignmapwaypoints[this.campaignmanager.campaignmapwaypoints[m].eastWaypoints[num5]].waypointName,
							"\n"
						});
						flag = true;
					}
				}
				for (int num6 = 0; num6 < this.campaignmanager.campaignmapwaypoints[m].westWaypoints.Length; num6++)
				{
					if (!this.isNumberInArray(this.campaignmanager.campaignmapwaypoints[m].waypointID, this.campaignmanager.campaignmapwaypoints[this.campaignmanager.campaignmapwaypoints[m].westWaypoints[num6]].eastWaypoints))
					{
						string text = text2;
						text2 = string.Concat(new string[]
						{
							text,
							this.campaignmanager.campaignmapwaypoints[m].waypointName,
							" not set as EAST of  ",
							this.campaignmanager.campaignmapwaypoints[this.campaignmanager.campaignmapwaypoints[m].westWaypoints[num6]].waypointName,
							"\n"
						});
						flag = true;
					}
				}
			}
			if (flag)
			{
				UIFunctions.globaluifunctions.SetPlayerErrorMessage(text2);
			}
		}
	}

	// Token: 0x06000A7B RID: 2683 RVA: 0x0008CF88 File Offset: 0x0008B188
	private bool isNumberInArray(int number, int[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == number)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000A7C RID: 2684 RVA: 0x0008CFB8 File Offset: 0x0008B1B8
	public void ReadCampaignRegionWaypointData()
	{
		string filePathFromString = this.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/waypoints_region");
		string[] array = this.OpenTextDataFile(filePathFromString);
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			string text = array2[0];
			if (text != null)
			{
				if (TextParser.<>f__switch$map3B == null)
				{
					TextParser.<>f__switch$map3B = new Dictionary<string, int>(1)
					{
						{
							"WaypointName",
							0
						}
					};
				}
				int num2;
				if (TextParser.<>f__switch$map3B.TryGetValue(text, out num2))
				{
					if (num2 == 0)
					{
						num++;
					}
				}
			}
		}
		this.campaignmanager.campaignregionwaypoints = new CampaignRegionWaypoint[num];
		CampaignRegionWaypoint campaignRegionWaypoint = null;
		int num3 = -1;
		for (int j = 0; j < array.Length; j++)
		{
			string[] array3 = array[j].Split(new char[]
			{
				'='
			});
			string text = array3[0];
			switch (text)
			{
			case "WaypointName":
				num3++;
				campaignRegionWaypoint = ScriptableObject.CreateInstance<CampaignRegionWaypoint>();
				this.campaignmanager.campaignregionwaypoints[num3] = campaignRegionWaypoint;
				campaignRegionWaypoint.waypointName = array3[1].Trim();
				campaignRegionWaypoint.waypointID = num3;
				campaignRegionWaypoint.northWaypoints = new int[0];
				campaignRegionWaypoint.southWaypoints = new int[0];
				campaignRegionWaypoint.eastWaypoints = new int[0];
				campaignRegionWaypoint.westWaypoints = new int[0];
				campaignRegionWaypoint.invadedByRoute = string.Empty;
				break;
			case "WaypointPosition":
				campaignRegionWaypoint.waypointPosition = this.PopulateVector3(array3[1].Trim() + ",0");
				campaignRegionWaypoint.waypointPosition.z = -5f;
				break;
			case "Alignment":
				campaignRegionWaypoint.faction = array3[1].Trim();
				break;
			case "InvadedBy":
				campaignRegionWaypoint.invadedByRoute = array3[1].Trim();
				break;
			}
		}
		num3 = -1;
		for (int k = 0; k < array.Length; k++)
		{
			string[] array4 = array[k].Split(new char[]
			{
				'='
			});
			string text = array4[0];
			if (text != null)
			{
				if (TextParser.<>f__switch$map3D == null)
				{
					TextParser.<>f__switch$map3D = new Dictionary<string, int>(2)
					{
						{
							"WaypointName",
							0
						},
						{
							"ConnectedZones",
							1
						}
					};
				}
				int num2;
				if (TextParser.<>f__switch$map3D.TryGetValue(text, out num2))
				{
					if (num2 != 0)
					{
						if (num2 == 1)
						{
							if (array4.Length > 0)
							{
								string[] arraydata = this.PopulateStringArray(array4[1].Trim());
								campaignRegionWaypoint.connectedWaypoints = this.GetCampaignRegionWaypointIDs(arraydata);
							}
						}
					}
					else
					{
						num3++;
						campaignRegionWaypoint = this.campaignmanager.campaignregionwaypoints[num3];
					}
				}
			}
		}
		filePathFromString = this.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/waypoints_region_names");
		array = this.OpenTextDataFile(filePathFromString);
		num3 = 0;
		for (int l = 0; l < array.Length; l++)
		{
			string[] array5 = array[l].Split(new char[]
			{
				'='
			});
			if (array5.Length == 2)
			{
				array5[0] = array5[0].Trim();
				array5[1] = array5[1].Trim();
				if (array5[0] == "Regions")
				{
					this.campaignmanager.regionNames = this.PopulateStringArray(array5[1]);
				}
				else
				{
					this.campaignmanager.campaignregionwaypoints[num3].waypointDescriptiveName = array5[0];
					this.campaignmanager.campaignregionwaypoints[num3].country = array5[1];
					num3++;
				}
			}
		}
	}

	// Token: 0x06000A7D RID: 2685 RVA: 0x0008D3DC File Offset: 0x0008B5DC
	public void ReadCombatantsData()
	{
		string filePathFromString = this.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/combatants");
		string[] array = this.OpenTextDataFile(filePathFromString);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			string text = array2[0];
			switch (text)
			{
			case "FRIENDLY":
				this.campaignmanager.eventManager.friendly = this.PopulateStringArray(array2[1].Trim());
				break;
			case "ENEMY":
				this.campaignmanager.eventManager.enemy = this.PopulateStringArray(array2[1].Trim());
				break;
			case "FRIENDLY_SINGULAR":
				this.campaignmanager.eventManager.friendlySingular = this.PopulateStringArray(array2[1].Trim());
				break;
			case "ENEMY_SINGULAR":
				this.campaignmanager.eventManager.enemySingular = this.PopulateStringArray(array2[1].Trim());
				break;
			case "FRIENDLY_PREFIX":
				this.campaignmanager.eventManager.friendlyPrefix = this.PopulateStringArray(array2[1].Trim());
				break;
			case "ENEMY_PREFIX":
				this.campaignmanager.eventManager.enemyPrefix = this.PopulateStringArray(array2[1].Trim());
				break;
			case "FRIENDLY_SUFFIX":
				this.campaignmanager.eventManager.friendlySuffix = this.PopulateStringArray(array2[1].Trim());
				break;
			case "ENEMY_SUFFIX":
				this.campaignmanager.eventManager.enemySuffix = this.PopulateStringArray(array2[1].Trim());
				break;
			}
		}
	}

	// Token: 0x06000A7E RID: 2686 RVA: 0x0008D618 File Offset: 0x0008B818
	public void PopulateLevelLoadData(string filename, string[] customLinesInFile = null)
	{
		int num = 0;
		int num2 = 0;
		this.levelloaddata.usePresetPositions = false;
		this.levelloaddata.usePresetEnvironment = false;
		List<string> list = new List<string>();
		this.levelloaddata.rnShipTelegraph = -10;
		this.levelloaddata.rnShipDepth = -1f;
		this.levelloaddata.missionPosition.x = -10000f;
		this.levelloaddata.proximityMineLocations = new Vector2[0];
		this.levelloaddata.mineFields = new BoxCollider[0];
		this.levelloaddata.sonobuoyLocations = new Vector3[0];
		this.levelloaddata.numberOfAircraft = 0;
		this.levelloaddata.numberOfHelicopters = 0;
		if (GameDataManager.missionMode || GameDataManager.trainingMode)
		{
			this.levelloaddata.neutralMerchantIDs1 = new List<int>();
			this.levelloaddata.neutralMerchantIDs2 = new List<int>();
			this.levelloaddata.neutralMerchantIDs3 = new List<int>();
			this.levelloaddata.neutralMerchantIDs4 = new List<int>();
			this.levelloaddata.neutralMerchantFlags1 = new List<string>();
			this.levelloaddata.neutralMerchantFlags2 = new List<string>();
			this.levelloaddata.neutralMerchantFlags3 = new List<string>();
			this.levelloaddata.neutralMerchantFlags4 = new List<string>();
			this.levelloaddata.neutralFishingIDs1 = new List<int>();
			this.levelloaddata.neutralFishingIDs2 = new List<int>();
			this.levelloaddata.neutralFishingIDs3 = new List<int>();
			this.levelloaddata.neutralFishingIDs4 = new List<int>();
			this.levelloaddata.neutralFishingFlags1 = new List<string>();
			this.levelloaddata.neutralFishingFlags2 = new List<string>();
			this.levelloaddata.neutralFishingFlags3 = new List<string>();
			this.levelloaddata.neutralFishingFlags4 = new List<string>();
		}
		string[] array;
		if (customLinesInFile != null)
		{
			array = customLinesInFile;
		}
		else
		{
			array = this.OpenTextDataFile(filename);
		}
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			string text = array2[0];
			switch (text)
			{
			case "UseTerrain":
				if (array2[1].Trim() == "TRUE")
				{
					this.levelloaddata.useTerrain = true;
				}
				else
				{
					this.levelloaddata.useTerrain = false;
				}
				break;
			case "MapCoordinates":
				this.levelloaddata.mapPosition = this.PopulateVector2(array2[1].Trim());
				break;
			case "MapElevationData":
				this.levelloaddata.mapElevationData = array2[1].Trim();
				break;
			case "MapNavigationData":
				this.levelloaddata.mapNavigationData = array2[1].Trim();
				break;
			case "WorldObjectsData":
				this.levelloaddata.worldObjectsData = this.GetFilePathFromString(array2[1].Trim());
				break;
			case "VesselsAndTraffic":
				this.ReadCivilianTrafficData(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "PlayerVessels":
			{
				string[] array3 = this.PopulateStringArray(array2[1].Trim());
				this.playerfunctions.playerVesselList = new int[array3.Length];
				for (int j = 0; j < array3.Length; j++)
				{
					this.playerfunctions.playerVesselList[j] = this.GetShipID(array3[j]);
				}
				break;
			}
			case "NumberOfEnemyUnits":
				if (array2[1].Trim() != "NONE")
				{
					string[] array4 = this.PopulateStringArray(array2[1].Trim());
					this.levelloaddata.numberOfEnemyUnits = new Vector2[array4.Length];
					for (int k = 0; k < array4.Length; k++)
					{
						string[] array5 = array4[k].Split(new char[]
						{
							'-'
						});
						this.levelloaddata.numberOfEnemyUnits[k].x = float.Parse(array5[0]);
						this.levelloaddata.numberOfEnemyUnits[k].y = float.Parse(array5[1]);
						num += (int)this.levelloaddata.numberOfEnemyUnits[k].y;
					}
				}
				this.levelloaddata.enemyWaypoints = new string[num];
				break;
			case "EnemyUnitMissionCritical":
				if (array2[1].Trim() != "NONE")
				{
					this.levelloaddata.enemyUnitMissionCritical = this.PopulateBoolArray(array2[1].Trim());
				}
				break;
			case "CombatBehaviour":
				this.levelloaddata.enemyShipBehaviour = this.PopulateStringArray(array2[1].Trim());
				break;
			case "EnemyShipClasses":
				if (array2[1].Trim() != "NONE")
				{
					this.levelloaddata.enemyShipClasses = this.PopulateStringArray(array2[1].Trim());
				}
				break;
			case "EnemyPositionsX":
				if (array2[1].Trim() != "NONE")
				{
					this.levelloaddata.enemyShipPositionsX = this.PopulateFloatArray(array2[1].Trim());
				}
				break;
			case "EnemyPositionsZ":
				if (array2[1].Trim() != "NONE")
				{
					this.levelloaddata.enemyShipPositionsZ = this.PopulateFloatArray(array2[1].Trim());
				}
				break;
			case "EnemyHeadings":
				if (array2[1].Trim() != "NONE")
				{
					this.levelloaddata.enemyshipHeadings = this.PopulateFloatArray(array2[1].Trim());
				}
				break;
			case "EnemyWaypoints":
				if (this.levelloaddata.usePresetPositions)
				{
					list.Add(array2[1].Trim());
				}
				break;
			case "MissionPosition":
				this.levelloaddata.missionPosition = this.PopulateVector2(array2[1]);
				break;
			case "MissionPositionColor":
				if (array2[1].Trim() == "LAND_STRIKE")
				{
					this.levelloaddata.missionMarkerCurrentColor = 1;
				}
				else
				{
					this.levelloaddata.missionMarkerCurrentColor = 0;
				}
				break;
			case "FormationCruiseSpeed":
				this.levelloaddata.formationCruiseSpeed = float.Parse(array2[1]) / 10f;
				break;
			case "NumberOfHelicopters":
				this.levelloaddata.numberOfHelicopters = int.Parse(array2[1]);
				this.levelloaddata.aircraftSearchAreas = new Vector3[this.levelloaddata.numberOfHelicopters];
				break;
			case "HelicopterType":
				this.levelloaddata.helicopterType = array2[1].Trim();
				break;
			case "NumberOfAircraft":
				this.levelloaddata.numberOfAircraft = int.Parse(array2[1]);
				this.levelloaddata.aircraftSearchAreas = new Vector3[this.levelloaddata.numberOfAircraft + this.levelloaddata.numberOfHelicopters];
				break;
			case "AircraftType":
				this.levelloaddata.aircraftType = array2[1].Trim();
				break;
			case "AircraftSearchArea":
				if (num2 < this.levelloaddata.aircraftSearchAreas.Length)
				{
					this.levelloaddata.aircraftSearchAreas[num2] = this.PopulateVector3(array2[1].Trim());
				}
				num2++;
				break;
			case "SonoBarrierLocations":
			{
				string[] array6 = array2[1].Trim().Split(new char[]
				{
					'|'
				});
				this.levelloaddata.sonobuoyLocations = new Vector3[array6.Length];
				this.levelloaddata.sonobuoyRanges = new float[array6.Length];
				for (int l = 0; l < this.levelloaddata.sonobuoyLocations.Length; l++)
				{
					this.levelloaddata.sonobuoyLocations[l] = this.PopulateVector3(array6[l]);
					this.levelloaddata.sonobuoyRanges[l] = this.levelloaddata.sonobuoyLocations[l].z * UnityEngine.Random.Range(0.9f, 1f);
					this.levelloaddata.sonobuoyLocations[l].z = this.levelloaddata.sonobuoyLocations[l].y;
					this.levelloaddata.sonobuoyLocations[l].y = 1000f;
				}
				break;
			}
			case "ProximityMineLocations":
			{
				string[] array7 = array2[1].Trim().Split(new char[]
				{
					'|'
				});
				this.levelloaddata.proximityMineLocations = new Vector2[array7.Length];
				for (int m = 0; m < this.levelloaddata.proximityMineLocations.Length; m++)
				{
					this.levelloaddata.proximityMineLocations[m] = this.PopulateVector2(array7[m]);
				}
				break;
			}
			case "ProximityMineField":
			{
				string[] array8 = array2[1].Trim().Split(new char[]
				{
					'|'
				});
				this.levelloaddata.proximityMineParameters = new Vector4[array8.Length];
				for (int n = 0; n < this.levelloaddata.proximityMineParameters.Length; n++)
				{
					this.levelloaddata.proximityMineParameters[n] = this.PopulateVector4(array8[n]);
				}
				break;
			}
			case "ProximityMineFieldAngles":
				this.levelloaddata.proximityMineAngles = this.PopulateFloatArray(array2[1]);
				break;
			case "ProximityMineScatter":
				this.levelloaddata.proximityMineScatter = this.PopulateFloatArray(array2[1]);
				for (int num4 = 0; num4 < this.levelloaddata.proximityMineScatter.Length; num4++)
				{
					this.levelloaddata.proximityMineScatter[num4] = Mathf.Clamp01(this.levelloaddata.proximityMineScatter[num4]);
				}
				break;
			case "UsePresetEnvironment":
				this.levelloaddata.usePresetEnvironment = this.SetBoolean(array2[1].Trim());
				break;
			case "Date":
			{
				this.levelloaddata.date = array2[1].Trim();
				string[] array9 = this.levelloaddata.date.Split(new char[]
				{
					' '
				});
				this.levelloaddata.month = array9[1];
				break;
			}
			case "Hemisphere":
				this.levelloaddata.hemisphere = array2[1].Trim();
				this.levelloaddata.season = CalendarFunctions.GetSeason(this.levelloaddata.month, this.levelloaddata.hemisphere);
				break;
			case "Time":
				this.levelloaddata.timeOfDayString = array2[1].Trim();
				if (this.levelloaddata.timeOfDayString == "RANDOM")
				{
					this.levelloaddata.timeOfDayString = (UnityEngine.Random.Range(0, 25) * 100).ToString("0000");
					this.levelloaddata.timeofday = this.campaignmanager.GetCampaignTimeOfDay(Mathf.FloorToInt((float)(int.Parse(this.levelloaddata.timeOfDayString) / 100)));
				}
				else
				{
					int hour = Mathf.FloorToInt((float)(int.Parse(this.levelloaddata.timeOfDayString) / 100));
					this.levelloaddata.timeofday = this.campaignmanager.GetCampaignTimeOfDay(hour);
				}
				break;
			case "Weather":
			{
				string a = array2[1].Trim();
				if (a == "RANDOM")
				{
					a = this.playerfunctions.sensormanager.weather[UnityEngine.Random.Range(0, this.playerfunctions.sensormanager.weather.Length)];
				}
				for (int num5 = 0; num5 < this.playerfunctions.sensormanager.weather.Length; num5++)
				{
					if (a == this.playerfunctions.sensormanager.weather[num5])
					{
						this.levelloaddata.environment = num5;
					}
				}
				break;
			}
			case "SeaState":
			{
				string a2 = array2[1].Trim();
				if (a2 == "RANDOM")
				{
					a2 = this.playerfunctions.sensormanager.seaStates[UnityEngine.Random.Range(0, this.playerfunctions.sensormanager.seaStates.Length)];
				}
				for (int num6 = 0; num6 < this.playerfunctions.sensormanager.seaStates.Length; num6++)
				{
					if (a2 == this.playerfunctions.sensormanager.seaStates[num6])
					{
						this.levelloaddata.seaState = num6;
					}
				}
				break;
			}
			case "DuctStrength":
			{
				string a3 = array2[1].Trim();
				if (a3 == "RANDOM")
				{
					this.levelloaddata.ductStrength = -1f;
				}
				else
				{
					for (int num7 = 0; num7 < this.playerfunctions.sensormanager.strengthTypes.Length; num7++)
					{
						if (a3 == this.playerfunctions.sensormanager.strengthTypes[num7])
						{
							this.levelloaddata.ductStrength = (float)num7 / 5f;
						}
					}
				}
				break;
			}
			case "LayerStrength":
			{
				string a4 = array2[1].Trim();
				if (a4 == "RANDOM")
				{
					this.levelloaddata.layerStrength = -1f;
				}
				else
				{
					for (int num8 = 0; num8 < this.playerfunctions.sensormanager.strengthTypes.Length; num8++)
					{
						if (a4 == this.playerfunctions.sensormanager.strengthTypes[num8])
						{
							this.levelloaddata.layerStrength = (float)num8 / 5f;
						}
					}
				}
				break;
			}
			case "UsePresetPositions":
				this.levelloaddata.usePresetPositions = this.SetBoolean(array2[1].Trim());
				break;
			case "PlayerPosition":
				this.levelloaddata.rnShipPosition = this.PopulateVector2(array2[1]);
				break;
			case "PlayerHeading":
				this.levelloaddata.rnShipHeading = float.Parse(array2[1]);
				break;
			case "PlayerDepthInFeet":
				if (array2[1].Trim() != "FALSE")
				{
					this.levelloaddata.rnShipDepth = float.Parse(array2[1]);
				}
				break;
			case "PlayerTelegraph":
				if (array2[1].Trim() != "FALSE")
				{
					this.levelloaddata.rnShipTelegraph = int.Parse(array2[1]);
				}
				break;
			case "CommanderName":
				UIFunctions.globaluifunctions.campaignmanager.commanderName = array2[1].Trim();
				break;
			case "CommanderFleetName":
				UIFunctions.globaluifunctions.campaignmanager.commanderFleetName = array2[1].Trim();
				break;
			}
		}
		if (this.levelloaddata.usePresetPositions)
		{
			int num9 = this.levelloaddata.numberOfAircraft + this.levelloaddata.numberOfHelicopters;
			if (num9 != this.levelloaddata.aircraftSearchAreas.Length)
			{
				this.levelloaddata.numberOfAircraft = 0;
				this.levelloaddata.numberOfHelicopters = 0;
			}
		}
		this.levelloaddata.enemyWaypoints = list.ToArray();
		int[] array10 = new int[this.levelloaddata.numberOfEnemyUnits.Length];
		int num10 = 0;
		for (int num11 = 0; num11 < array10.Length; num11++)
		{
			array10[num11] = (int)UnityEngine.Random.Range(this.levelloaddata.numberOfEnemyUnits[num11].x, this.levelloaddata.numberOfEnemyUnits[num11].y);
			num10 += array10[num11];
		}
		if (num10 > GameDataManager.maxShips)
		{
			num10 = GameDataManager.maxShips;
		}
		string[] array11 = new string[num10];
		bool[] array12 = new bool[num10];
		num10 = 0;
		for (int num12 = 0; num12 < array10.Length; num12++)
		{
			for (int num13 = 0; num13 < array10[num12]; num13++)
			{
				if (num10 < GameDataManager.maxShips)
				{
					if (this.levelloaddata.enemyShipBehaviour[num12] == "DEFENSIVE")
					{
						array12[num10] = true;
					}
					array11[num10] = UIFunctions.globaluifunctions.campaignmanager.GetTaskForceShip(this.levelloaddata.enemyShipClasses[num12]);
					num10++;
				}
			}
		}
		this.levelloaddata.kmShipClasses = new int[array11.Length];
		this.levelloaddata.kmShipInstances = new int[array11.Length];
		this.levelloaddata.kmSlots = new int[this.levelloaddata.kmShipClasses.Length];
		for (int num14 = 0; num14 < array11.Length; num14++)
		{
			this.levelloaddata.kmShipClasses[num14] = this.GetShipID(array11[num14]);
		}
		if (GameDataManager.missionMode)
		{
			if (customLinesInFile == null)
			{
				this.GetSingleMissionBriefingData(this.GetFilePathFromString("language/mission/" + filename));
			}
			else
			{
				this.GetSingleMissionBriefingData(this.GetFilePathFromString("language/mission/single001"));
			}
		}
		else if (GameDataManager.trainingMode)
		{
			this.GetSingleMissionBriefingData(this.GetFilePathFromString("language/training/" + filename));
		}
		if (!UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.usePresetPositions)
		{
			if (!GameDataManager.missionMode)
			{
				UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.GetHelicopterNumbers();
			}
			UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.AssignWhales();
		}
	}

	// Token: 0x06000A7F RID: 2687 RVA: 0x0008E9F8 File Offset: 0x0008CBF8
	private void GetSingleMissionBriefingData(string filename)
	{
		string[] array = this.OpenTextDataFile(filename);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			string text = array2[0];
			switch (text)
			{
			case "MissionLocation":
				this.levelloaddata.combatLocation = array2[1].Trim();
				break;
			case "Briefing":
			{
				string text2 = array2[1];
				text2 = UIFunctions.globaluifunctions.helpmanager.PopulateHelpTags(text2);
				text2 = text2.Replace("maroon", "lime");
				Text selectionGroupText = UIFunctions.globaluifunctions.selectionGroupText;
				selectionGroupText.text = selectionGroupText.text + text2 + "\n";
				break;
			}
			case "Image":
				this.SetImageSprite(this.GetFilePathFromString(array2[1].Trim()), UIFunctions.globaluifunctions.backgroundImage);
				break;
			}
		}
	}

	// Token: 0x06000A80 RID: 2688 RVA: 0x0008EB34 File Offset: 0x0008CD34
	public void ReadCivilianTrafficData(string filename)
	{
		string[] array = this.OpenTextDataFile(filename);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			string text = array2[0];
			switch (text)
			{
			case "MapNeutralMerchantData":
				this.campaignmanager.mapMerchantTraffic = this.GetTexture(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "MapNeutralFishingData":
				this.campaignmanager.mapFishingTraffic = this.GetTexture(this.GetFilePathFromString(array2[1].Trim()));
				break;
			case "NeutralMerchant1":
			{
				string[] array3 = this.PopulateStringArray(array2[1].Trim());
				this.levelloaddata.neutralMerchantIDs1 = new List<int>();
				for (int j = 0; j < array3.Length; j++)
				{
					this.levelloaddata.neutralMerchantIDs1.Add(this.GetShipID(array3[j]));
				}
				break;
			}
			case "NeutralMerchant2":
			{
				string[] array4 = this.PopulateStringArray(array2[1].Trim());
				this.levelloaddata.neutralMerchantIDs2 = new List<int>();
				for (int k = 0; k < array4.Length; k++)
				{
					this.levelloaddata.neutralMerchantIDs2.Add(this.GetShipID(array4[k]));
				}
				break;
			}
			case "NeutralMerchant3":
			{
				string[] array5 = this.PopulateStringArray(array2[1].Trim());
				this.levelloaddata.neutralMerchantIDs3 = new List<int>();
				for (int l = 0; l < array5.Length; l++)
				{
					this.levelloaddata.neutralMerchantIDs3.Add(this.GetShipID(array5[l]));
				}
				break;
			}
			case "NeutralMerchant4":
			{
				string[] array6 = this.PopulateStringArray(array2[1].Trim());
				this.levelloaddata.neutralMerchantIDs4 = new List<int>();
				for (int m = 0; m < array6.Length; m++)
				{
					this.levelloaddata.neutralMerchantIDs4.Add(this.GetShipID(array6[m]));
				}
				break;
			}
			case "NeutralFishing1":
			{
				string[] array7 = this.PopulateStringArray(array2[1].Trim());
				this.levelloaddata.neutralFishingIDs1 = new List<int>();
				for (int n = 0; n < array7.Length; n++)
				{
					this.levelloaddata.neutralFishingIDs1.Add(this.GetShipID(array7[n]));
				}
				break;
			}
			case "NeutralFishing2":
			{
				string[] array8 = this.PopulateStringArray(array2[1].Trim());
				this.levelloaddata.neutralFishingIDs2 = new List<int>();
				for (int num2 = 0; num2 < array8.Length; num2++)
				{
					this.levelloaddata.neutralFishingIDs2.Add(this.GetShipID(array8[num2]));
				}
				break;
			}
			case "NeutralFishing3":
			{
				string[] array9 = this.PopulateStringArray(array2[1].Trim());
				this.levelloaddata.neutralFishingIDs3 = new List<int>();
				for (int num3 = 0; num3 < array9.Length; num3++)
				{
					this.levelloaddata.neutralFishingIDs3.Add(this.GetShipID(array9[num3]));
				}
				break;
			}
			case "NeutralFishing4":
			{
				string[] array10 = this.PopulateStringArray(array2[1].Trim());
				this.levelloaddata.neutralFishingIDs4 = new List<int>();
				for (int num4 = 0; num4 < array10.Length; num4++)
				{
					this.levelloaddata.neutralFishingIDs4.Add(this.GetShipID(array10[num4]));
				}
				break;
			}
			case "NeutralMerchantFlags1":
				this.levelloaddata.neutralMerchantFlags1 = this.PopulateStringList(array2[1].Trim());
				break;
			case "NeutralMerchantFlags2":
				this.levelloaddata.neutralMerchantFlags2 = this.PopulateStringList(array2[1].Trim());
				break;
			case "NeutralMerchantFlags3":
				this.levelloaddata.neutralMerchantFlags3 = this.PopulateStringList(array2[1].Trim());
				break;
			case "NeutralMerchantFlags4":
				this.levelloaddata.neutralMerchantFlags4 = this.PopulateStringList(array2[1].Trim());
				break;
			case "NeutralFishingFlags1":
				this.levelloaddata.neutralFishingFlags1 = this.PopulateStringList(array2[1].Trim());
				break;
			case "NeutralFishingFlags2":
				this.levelloaddata.neutralFishingFlags2 = this.PopulateStringList(array2[1].Trim());
				break;
			case "NeutralFishingFlags3":
				this.levelloaddata.neutralFishingFlags3 = this.PopulateStringList(array2[1].Trim());
				break;
			case "NeutralFishingFlags4":
				this.levelloaddata.neutralFishingFlags4 = this.PopulateStringList(array2[1].Trim());
				break;
			case "OtherVessels":
			{
				string[] array11 = this.PopulateStringArray(array2[1].Trim());
				this.playerfunctions.otherVesselList = new int[array11.Length];
				for (int num5 = 0; num5 < array11.Length; num5++)
				{
					this.playerfunctions.otherVesselList[num5] = this.GetShipID(array11[num5]);
				}
				break;
			}
			}
		}
	}

	// Token: 0x06000A81 RID: 2689 RVA: 0x0008F15C File Offset: 0x0008D35C
	public void ReadEnvironmentData(string filepath)
	{
		global::Environment environment = UIFunctions.globaluifunctions.levelloadmanager.environment;
		string[] array = this.OpenTextDataFile(filepath);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Trim().Split(new char[]
			{
				'='
			});
			string text = array2[0];
			switch (text)
			{
			case "SkyboxTexture":
				UIFunctions.globaluifunctions.levelloadmanager.skyboxmaterial.mainTexture = Resources.Load<Texture>(this.GetFilePathFromString(array2[1]));
				UIFunctions.globaluifunctions.levelloadmanager.skyboxmaterial.SetTexture("_StarTex", Resources.Load<Texture>(this.GetFilePathFromString("environment/sky/black")));
				UIFunctions.globaluifunctions.levelloadmanager.skyboxmaterial.SetTexture("_MaskTex", Resources.Load<Texture>(this.GetFilePathFromString("environment/sky/black")));
				break;
			case "StarsTexture":
				UIFunctions.globaluifunctions.levelloadmanager.skyboxmaterial.SetTexture("_StarTex", Resources.Load<Texture>(this.GetFilePathFromString(array2[1])));
				break;
			case "NightMask":
				UIFunctions.globaluifunctions.levelloadmanager.skyboxmaterial.SetTexture("_MaskTex", Resources.Load<Texture>(this.GetFilePathFromString(array2[1])));
				break;
			case "MoonRotation":
			{
				UIFunctions.globaluifunctions.levelloadmanager.moonObject.transform.localRotation = Quaternion.Euler(this.PopulateVector3(array2[1]));
				int moonPhase = CalendarFunctions.GetMoonPhase();
				Vector2 zero = Vector2.zero;
				switch (moonPhase)
				{
				case 0:
					zero.x = 0f;
					zero.y = 0.5f;
					break;
				case 1:
					zero.x = 0.25f;
					zero.y = 0.5f;
					break;
				case 2:
					zero.x = 0.5f;
					zero.y = 0.5f;
					break;
				case 3:
					zero.x = 0.75f;
					zero.y = 0.5f;
					break;
				case 4:
					zero.x = 0f;
					zero.y = 0f;
					break;
				case 5:
					zero.x = 0.25f;
					zero.y = 0f;
					break;
				case 6:
					zero.x = 0.5f;
					zero.y = 0f;
					break;
				case 7:
					zero.x = 0.75f;
					zero.y = 0f;
					break;
				}
				UIFunctions.globaluifunctions.levelloadmanager.moonObject.GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex", this.GetTexture(this.GetFilePathFromString("environment/sky/moon")));
				UIFunctions.globaluifunctions.levelloadmanager.moonObject.GetComponentInChildren<MeshRenderer>().material.SetTextureOffset("_MainTex", zero);
				UIFunctions.globaluifunctions.levelloadmanager.moonObject.SetActive(true);
				break;
			}
			case "SkyboxDiffuse":
				UIFunctions.globaluifunctions.levelloadmanager.environmentSky.diffuseCube = Resources.Load<Cubemap>(this.GetFilePathFromString(array2[1]));
				break;
			case "SkyboxSpecular":
				UIFunctions.globaluifunctions.levelloadmanager.environmentSky.specularCube = Resources.Load<Cubemap>(this.GetFilePathFromString(array2[1]));
				break;
			case "AmbientColor":
				environment.actualAmbientColor = this.GetColor32(array2[1]);
				break;
			case "LightLevel":
				environment.actualLightLevel = this.GetColor32(array2[1]);
				global::Environment.treeColor = environment.actualLightLevel;
				break;
			case "FoamColor":
				UIFunctions.globaluifunctions.levelloadmanager.cetoOcean.foamTint = this.GetColor32(array2[1]);
				break;
			case "SurfaceFogColor":
				environment.actualSurfaceFogColor = this.GetColor32(array2[1]);
				break;
			case "SubmergedFogColor":
				environment.actualSubmergedFogColor = this.GetColor32(array2[1]);
				break;
			case "SkyColor":
				UIFunctions.globaluifunctions.levelloadmanager.cetoOcean.defaultSkyColor = this.GetColor32(array2[1]);
				break;
			case "OceanColor":
				UIFunctions.globaluifunctions.levelloadmanager.cetoOcean.defaultOceanColor = this.GetColor32(array2[1]);
				break;
			case "ReflectionTint":
				UIFunctions.globaluifunctions.levelloadmanager.planarreflection.reflectionTint = this.GetColor32(array2[1]);
				break;
			case "ReflectionIntensity":
				UIFunctions.globaluifunctions.levelloadmanager.planarreflection.reflectionIntensity = float.Parse(array2[1]);
				break;
			case "AboveInscatter":
				UIFunctions.globaluifunctions.levelloadmanager.cetoUnderwater.aboveInscatterModifier.color = this.GetColor32(array2[1]);
				break;
			case "SurfaceFogDensity":
				environment.actualSurfaceFogDensity = float.Parse(array2[1]);
				break;
			case "SubmergedFogDensity":
				environment.actualSubmergedFogDensity = float.Parse(array2[1]);
				break;
			case "PrimaryLightRotation":
				environment.directionalLights[0].transform.localRotation = Quaternion.Euler(this.PopulateVector3(array2[1]));
				break;
			case "PrimaryLightIntensity":
				environment.directionalLights[0].intensity = float.Parse(array2[1]);
				break;
			case "PrimaryLightColor":
				environment.directionalLights[0].color = this.GetColor32(array2[1]);
				break;
			case "PrimaryLensFlareBrightness":
				environment.primaryLensFlare.brightness = float.Parse(array2[1]);
				environment.primaryLensFlare.enabled = true;
				break;
			case "PrimaryLensFlareFadeSpeed":
				environment.primaryLensFlare.fadeSpeed = float.Parse(array2[1]);
				break;
			case "ShadowIntensity":
				environment.directionalLights[0].shadowStrength = float.Parse(array2[1]);
				break;
			case "OceanShadows":
				if (array2[1].Trim() == "TRUE")
				{
					ManualCameraZoom.oceanShadowPlane.SetActive(true);
				}
				else
				{
					ManualCameraZoom.oceanShadowPlane.SetActive(false);
				}
				break;
			case "PostProcessingSurface":
			{
				string[] array3 = array2[1].Split(new char[]
				{
					','
				});
				UIFunctions.globaluifunctions.levelloadmanager.amplifyColorTextures[0] = this.GetTexture(this.GetFilePathFromString("environment/lut/" + array3[UIFunctions.globaluifunctions.levelloadmanager.environmentTemperature].Trim()));
				break;
			}
			case "PostProcessingUnderwater":
				UIFunctions.globaluifunctions.levelloadmanager.amplifyColorTextures[1] = this.GetTexture(this.GetFilePathFromString("environment/lut/" + array2[1].Trim()));
				break;
			case "SecondaryLightRotation":
				environment.directionalLights[1].transform.localRotation = Quaternion.Euler(this.PopulateVector3(array2[1]));
				break;
			case "SecondaryLightIntensity":
				environment.directionalLights[1].intensity = float.Parse(array2[1]);
				break;
			case "SecondaryLightColor":
				environment.directionalLights[1].color = this.GetColor32(array2[1]);
				break;
			}
		}
		UIFunctions.globaluifunctions.levelloadmanager.environment = environment;
	}

	// Token: 0x06000A82 RID: 2690 RVA: 0x0008FA8C File Offset: 0x0008DC8C
	public void BuildInterfaceDictionary(string filepath)
	{
		LanguageManager.interfaceDictionary = new Dictionary<string, string>();
		string[] array = this.OpenTextDataFile(filepath);
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = array[i].Trim();
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			if (array2.Length == 2)
			{
				if (!LanguageManager.interfaceDictionary.ContainsKey(array2[0]))
				{
					LanguageManager.interfaceDictionary.Add(array2[0], array2[1]);
				}
			}
		}
	}

	// Token: 0x06000A83 RID: 2691 RVA: 0x0008FB10 File Offset: 0x0008DD10
	public void BuildMessageLogDictionary(string filepath)
	{
		LanguageManager.messageLogDictionary = new Dictionary<string, string>();
		LanguageManager.messageLogAudioClipDictionary = new Dictionary<string, string>();
		LanguageManager.messageLogVoiceDictionary = new Dictionary<string, string>();
		string[] array = this.OpenTextDataFile(filepath);
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = array[i].Trim();
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			if (array2[0] == "[AUDIO]")
			{
				flag = true;
			}
			if (array2[0] == "[VOICES]")
			{
				flag = false;
				flag2 = true;
			}
			if (flag)
			{
				if (array2.Length == 2)
				{
					if (!LanguageManager.messageLogAudioClipDictionary.ContainsKey(array2[0]))
					{
						LanguageManager.messageLogAudioClipDictionary.Add(array2[0], array2[1]);
					}
				}
			}
			else if (flag2)
			{
				if (array2.Length == 2)
				{
					if (!LanguageManager.messageLogVoiceDictionary.ContainsKey(array2[0]))
					{
						LanguageManager.messageLogVoiceDictionary.Add(array2[0], array2[1]);
					}
				}
			}
			else if (array2.Length == 2)
			{
				if (!LanguageManager.messageLogDictionary.ContainsKey(array2[0]))
				{
					LanguageManager.messageLogDictionary.Add(array2[0], array2[1]);
				}
			}
		}
	}

	// Token: 0x06000A84 RID: 2692 RVA: 0x0008FC5C File Offset: 0x0008DE5C
	public void BuildEditorDictionary(string filepath)
	{
		LanguageManager.editorDictionary = new Dictionary<string, string>();
		EditorMission.instance.mapNames = new List<string>();
		string[] array = this.OpenTextDataFile(filepath);
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = array[i].Trim();
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			if (array2.Length == 2)
			{
				if (array2[0] == "MapName")
				{
					EditorMission.instance.mapNames.Add(array2[1]);
				}
				else if (!LanguageManager.editorDictionary.ContainsKey(array2[0]))
				{
					LanguageManager.editorDictionary.Add(array2[0], array2[1]);
				}
			}
		}
	}

	// Token: 0x06000A85 RID: 2693 RVA: 0x0008FD18 File Offset: 0x0008DF18
	private bool SetBoolean(string value)
	{
		return value == "TRUE";
	}

	// Token: 0x06000A86 RID: 2694 RVA: 0x0008FD30 File Offset: 0x0008DF30
	private void PopulateSubsystemArray(int s, string subsystem, string linedata)
	{
		int subsystemIndex = DamageControl.GetSubsystemIndex(subsystem);
		string[] array = linedata.Split(new char[]
		{
			','
		});
		string[] array2 = new string[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = array[i].Trim();
		}
		if (array2.Length == 1)
		{
			this.database.databaseshipdata[s].subsystemPrimaryPositions[subsystemIndex] = array2[0];
		}
		else if (array2.Length == 2)
		{
			this.database.databaseshipdata[s].subsystemPrimaryPositions[subsystemIndex] = array2[0];
			this.database.databaseshipdata[s].subsystemSecondaryPositions[subsystemIndex] = array2[1];
		}
		else
		{
			this.database.databaseshipdata[s].subsystemPrimaryPositions[subsystemIndex] = array2[0];
			this.database.databaseshipdata[s].subsystemSecondaryPositions[subsystemIndex] = array2[1];
			this.database.databaseshipdata[s].subsystemTertiaryPositions[subsystemIndex] = array2[1];
		}
	}

	// Token: 0x06000A87 RID: 2695 RVA: 0x0008FE24 File Offset: 0x0008E024
	private float[] PopulateFloatArray(string linedata)
	{
		string[] array = linedata.Split(new char[]
		{
			','
		});
		float[] array2 = new float[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = float.Parse(array[i]);
		}
		return array2;
	}

	// Token: 0x06000A88 RID: 2696 RVA: 0x0008FE6C File Offset: 0x0008E06C
	public string[] PopulateStringArray(string linedata)
	{
		string[] array = linedata.Split(new char[]
		{
			','
		});
		string[] array2 = new string[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = array[i].Trim();
		}
		return array2;
	}

	// Token: 0x06000A89 RID: 2697 RVA: 0x0008FEB4 File Offset: 0x0008E0B4
	private string[] PopulateMultiLineTextArray(string linedata)
	{
		linedata = linedata.Replace("\\n", "_");
		string[] array = linedata.Split(new char[]
		{
			'_'
		});
		string[] array2 = new string[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = array[i].Trim();
		}
		return array2;
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x0008FF10 File Offset: 0x0008E110
	private int[] PopulateIntArray(string linedata)
	{
		string[] array = linedata.Split(new char[]
		{
			','
		});
		int[] array2 = new int[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = int.Parse(array[i]);
		}
		return array2;
	}

	// Token: 0x06000A8B RID: 2699 RVA: 0x0008FF58 File Offset: 0x0008E158
	private int[] PopulateBlankIntArray(string linedata)
	{
		string[] array = linedata.Split(new char[]
		{
			','
		});
		int[] array2 = new int[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = 0;
		}
		return array2;
	}

	// Token: 0x06000A8C RID: 2700 RVA: 0x0008FF9C File Offset: 0x0008E19C
	private bool[] PopulateBoolArray(string linedata)
	{
		string[] array = linedata.Split(new char[]
		{
			','
		});
		bool[] array2 = new bool[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Trim() == "TRUE")
			{
				array2[i] = true;
			}
			else
			{
				array2[i] = false;
			}
		}
		return array2;
	}

	// Token: 0x06000A8D RID: 2701 RVA: 0x00090000 File Offset: 0x0008E200
	private List<string> PopulateStringList(string linedata)
	{
		string[] array = linedata.Split(new char[]
		{
			','
		});
		List<string> list = new List<string>();
		for (int i = 0; i < array.Length; i++)
		{
			list.Add(array[i].Trim());
		}
		return list;
	}

	// Token: 0x06000A8E RID: 2702 RVA: 0x00090048 File Offset: 0x0008E248
	public Vector2 PopulateVector2(string linedata)
	{
		string[] array = linedata.Split(new char[]
		{
			','
		});
		return new Vector3(float.Parse(array[0]), float.Parse(array[1]));
	}

	// Token: 0x06000A8F RID: 2703 RVA: 0x00090084 File Offset: 0x0008E284
	public Vector3 PopulateVector3(string linedata)
	{
		string[] array = linedata.Split(new char[]
		{
			','
		});
		Vector3 result = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
		return result;
	}

	// Token: 0x06000A90 RID: 2704 RVA: 0x000900C4 File Offset: 0x0008E2C4
	public Vector4 PopulateVector4(string linedata)
	{
		string[] array = linedata.Split(new char[]
		{
			','
		});
		Vector4 result = new Vector4(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), float.Parse(array[3]));
		return result;
	}

	// Token: 0x06000A91 RID: 2705 RVA: 0x0009010C File Offset: 0x0008E30C
	private int GetWeaponID(string weaponName)
	{
		for (int i = 0; i < this.database.databaseweapondata.Length; i++)
		{
			if (this.database.databaseweapondata[i] != null && weaponName == this.database.databaseweapondata[i].weaponPrefabName)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000A92 RID: 2706 RVA: 0x00090170 File Offset: 0x0008E370
	public Color32 GetColor32(string linedata)
	{
		string[] array = linedata.Split(new char[]
		{
			','
		});
		if (array.Length != 4)
		{
			return Color.magenta;
		}
		return new Color32(byte.Parse(array[0]), byte.Parse(array[1]), byte.Parse(array[2]), byte.Parse(array[3]));
	}

	// Token: 0x06000A93 RID: 2707 RVA: 0x000901C8 File Offset: 0x0008E3C8
	public Color GetColor(string linedata)
	{
		string[] array = linedata.Split(new char[]
		{
			','
		});
		if (array.Length != 4)
		{
			return Color.magenta;
		}
		return new Color((float)byte.Parse(array[0]), (float)(byte.Parse(array[1]) / byte.MaxValue), (float)(byte.Parse(array[2]) / byte.MaxValue), (float)(byte.Parse(array[3]) / byte.MaxValue));
	}

	// Token: 0x06000A94 RID: 2708 RVA: 0x00090234 File Offset: 0x0008E434
	private int GetSonarID(string sonarName)
	{
		if (sonarName == "FALSE")
		{
			return -1;
		}
		for (int i = 0; i < this.database.databasesonardata.Length; i++)
		{
			if (sonarName == this.database.databasesonardata[i].sonarModel)
			{
				return i;
			}
		}
		Debug.LogError("GetSonarID not found: " + sonarName);
		return -1;
	}

	// Token: 0x06000A95 RID: 2709 RVA: 0x000902A4 File Offset: 0x0008E4A4
	private int GetRADARID(string radarName)
	{
		if (radarName == "FALSE")
		{
			return -1;
		}
		for (int i = 0; i < this.database.databaseradardata.Length; i++)
		{
			if (radarName == this.database.databaseradardata[i].radarname)
			{
				return i;
			}
		}
		Debug.LogError("GetRADARID not found: " + radarName);
		return -1;
	}

	// Token: 0x06000A96 RID: 2710 RVA: 0x00090314 File Offset: 0x0008E514
	private int GetCountermeasureID(string countermeasureName)
	{
		if (countermeasureName == "FALSE")
		{
			return -1;
		}
		for (int i = 0; i < this.database.databasecountermeasuredata.Length; i++)
		{
			if (countermeasureName == this.database.databasecountermeasuredata[i].countermeasureObject.name)
			{
				return i;
			}
		}
		Debug.LogError("GetCountermeasureID not found: " + countermeasureName);
		return -1;
	}

	// Token: 0x06000A97 RID: 2711 RVA: 0x00090388 File Offset: 0x0008E588
	public int GetShipID(string shipName)
	{
		for (int i = 0; i < this.database.databaseshipdata.Length; i++)
		{
			if (shipName == this.database.databaseshipdata[i].shipPrefabName)
			{
				return i;
			}
		}
		Debug.LogError("GetShipID not found: " + shipName);
		return -1;
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x000903E4 File Offset: 0x0008E5E4
	public int GetAircraftID(string aircraftName)
	{
		for (int i = 0; i < this.database.databaseaircraftdata.Length; i++)
		{
			if (aircraftName == this.database.databaseaircraftdata[i].aircraftPrefabName)
			{
				return i;
			}
		}
		Debug.LogError("GetAircraftID not found: " + aircraftName);
		return -1;
	}

	// Token: 0x06000A99 RID: 2713 RVA: 0x00090440 File Offset: 0x0008E640
	public int GetCampaignAircraftID(string aircraftName)
	{
		for (int i = 0; i < this.campaignmanager.campaignaircraft.Length; i++)
		{
			if (aircraftName == this.campaignmanager.campaignaircraft[i].reconName)
			{
				return i;
			}
		}
		Debug.LogError("GetCampaignAircraftID not found: " + aircraftName);
		return -1;
	}

	// Token: 0x06000A9A RID: 2714 RVA: 0x0009049C File Offset: 0x0008E69C
	public int GetDepthWeaponID(string rbuName)
	{
		if (rbuName == "FALSE")
		{
			return -1;
		}
		for (int i = 0; i < this.database.databasedepthchargedata.Length; i++)
		{
			if (rbuName == this.database.databasedepthchargedata[i].depthChargeObject.name)
			{
				return i;
			}
		}
		Debug.LogError("GetDepthWeaponID not found: " + rbuName);
		return -1;
	}

	// Token: 0x06000A9B RID: 2715 RVA: 0x00090510 File Offset: 0x0008E710
	public int[] GetCampaignMapWaypointIDs(string[] arraydata)
	{
		int[] array = new int[arraydata.Length];
		for (int i = 0; i < arraydata.Length; i++)
		{
			array[i] = this.GetSingleCampaignMapWaypointID(arraydata[i]);
		}
		return array;
	}

	// Token: 0x06000A9C RID: 2716 RVA: 0x00090548 File Offset: 0x0008E748
	public int GetSingleCampaignMapWaypointID(string currentWaypoint)
	{
		for (int i = 0; i < this.campaignmanager.campaignmapwaypoints.Length; i++)
		{
			if (currentWaypoint == this.campaignmanager.campaignmapwaypoints[i].waypointName)
			{
				return this.campaignmanager.campaignmapwaypoints[i].waypointID;
			}
		}
		return -1;
	}

	// Token: 0x06000A9D RID: 2717 RVA: 0x000905A4 File Offset: 0x0008E7A4
	public int[] GetCampaignRegionWaypointIDs(string[] arraydata)
	{
		int[] array = new int[arraydata.Length];
		for (int i = 0; i < arraydata.Length; i++)
		{
			array[i] = this.GetSingleCampaignRegionWaypointID(arraydata[i]);
		}
		return array;
	}

	// Token: 0x06000A9E RID: 2718 RVA: 0x000905DC File Offset: 0x0008E7DC
	public int GetSingleCampaignRegionWaypointID(string currentWaypoint)
	{
		for (int i = 0; i < this.campaignmanager.campaignregionwaypoints.Length; i++)
		{
			if (currentWaypoint == this.campaignmanager.campaignregionwaypoints[i].waypointName)
			{
				return this.campaignmanager.campaignregionwaypoints[i].waypointID;
			}
		}
		return -1;
	}

	// Token: 0x06000A9F RID: 2719 RVA: 0x00090638 File Offset: 0x0008E838
	public int[] AddIntToArray(int[] originalArray, int addMe)
	{
		List<int> list = new List<int>();
		if (originalArray != null)
		{
			list = originalArray.ToList<int>();
		}
		list.Add(addMe);
		return list.ToArray<int>();
	}

	// Token: 0x06000AA0 RID: 2720 RVA: 0x00090668 File Offset: 0x0008E868
	public string PopulateDynamicTags(string s, bool uppercase)
	{
		s = s.Replace("<n>", "\n");
		if (s.Contains("<PLAYERVESSELNAVY>"))
		{
			s = s.Replace("<PLAYERVESSELNAVY>", this.GetHullPrefix(UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].shipPrefabName));
		}
		if (s.Contains("<PLAYERVESSELNAME>"))
		{
			s = s.Replace("<PLAYERVESSELNAME>", this.database.databaseshipdata[this.playerfunctions.playerVesselClass].playerClassNames[this.playerfunctions.playerVesselInstance]);
		}
		if (s.Contains("<PLAYERVESSELHULL>"))
		{
			s = s.Replace("<PLAYERVESSELHULL>", this.database.databaseshipdata[this.playerfunctions.playerVesselClass].playerClassHullNumbers[this.playerfunctions.playerVesselInstance]);
		}
		if (s.Contains("<PLAYERVESSELCLASS>"))
		{
			s = s.Replace("<PLAYERVESSELCLASS>", UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].shipclass);
		}
		if (s.Contains("<PLAYERVESSELTYPE>"))
		{
			s = s.Replace("<PLAYERVESSELTYPE>", LanguageManager.interfaceDictionary[this.database.databaseshipdata[this.playerfunctions.playerVesselClass].shipType]);
		}
		if (s.Contains("<PLAYERNAME>"))
		{
			s = s.Replace("<PLAYERNAME>", GameDataManager.playerCommanderName);
		}
		if (s.Contains("<DATE>"))
		{
			s = s.Replace("<DATE>", UIFunctions.globaluifunctions.campaignmanager.eventManager.currentFullDate);
		}
		if (s.Contains("<PLAYERMAPREGION>"))
		{
			if (UIFunctions.globaluifunctions.campaignmanager.eventManager.playerMapRegionIsLocation)
			{
				s = s.Replace("<PLAYERMAPREGION>", UIFunctions.globaluifunctions.campaignmanager.eventManager.playerMapRegion);
			}
			else
			{
				s = s.Replace("<PLAYERMAPREGION>", UIFunctions.globaluifunctions.campaignmanager.eventManager.playerMapRegion);
			}
		}
		if (s.Contains("<POSTHUMOUSLY>"))
		{
			if (UIFunctions.globaluifunctions.missionmanager.playerSunk && !UIFunctions.globaluifunctions.missionmanager.abandonedShip)
			{
				s = s.Replace("<POSTHUMOUSLY>", LanguageManager.interfaceDictionary["Posthumously"]);
			}
			else
			{
				s = s.Replace("<POSTHUMOUSLY>", string.Empty);
			}
		}
		if (s.Contains("<PLAYERVESSELSTATUS>"))
		{
			if (UIFunctions.globaluifunctions.missionmanager.abandonedShip)
			{
				s = s.Replace("<PLAYERVESSELSTATUS>", LanguageManager.interfaceDictionary["ActionReportLost"]);
			}
			else
			{
				s = s.Replace("<PLAYERVESSELSTATUS>", LanguageManager.interfaceDictionary["ActionReportLostAllHands"]);
			}
			s = s + "\n\t\t" + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ActionReportHowSunk") + " ";
			if (UIFunctions.globaluifunctions.playerfunctions.playerSunkBy.Contains("WEAPON|"))
			{
				string[] array = UIFunctions.globaluifunctions.playerfunctions.playerSunkBy.Split(new char[]
				{
					'|'
				});
				s += array[1];
			}
			else if (UIFunctions.globaluifunctions.playerfunctions.playerSunkBy == "IMPLOSION")
			{
				s += LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ActionReportImplosion");
			}
			else if (UIFunctions.globaluifunctions.playerfunctions.playerSunkBy == "COLLISION")
			{
				s += LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ActionReportCollision");
			}
			else
			{
				s += LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ActionReportScuttled");
			}
		}
		if (s.Contains("<COMMANDER>"))
		{
			s = s.Replace("<COMMANDER>", this.campaignmanager.commanderName);
		}
		if (s.Contains("<COMMANDERFLEET>"))
		{
			s = s.Replace("<COMMANDERFLEET>", this.campaignmanager.commanderFleetName);
		}
		if (s.Contains("<IMPASSE>"))
		{
			s = s.Replace("<IMPASSE>", UIFunctions.globaluifunctions.campaignmanager.eventManager.GetDynamicWarContent("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/impasse"));
		}
		else if (s.Contains("<ENEMY_TITLE_INVASION>") || s.Contains("<ENEMY_INVASION_LAND>") || s.Contains("<ENEMY_INVASION_SEA>") || s.Contains("<ENEMY_INVASION_AIR>"))
		{
			s = s.Replace("<ENEMY_TITLE_INVASION>", UIFunctions.globaluifunctions.campaignmanager.eventManager.GetDynamicWarTakeOverContent("<ENEMY_TITLE_INVASION>"));
			s = s.Replace("<ENEMY_INVASION_LAND>", UIFunctions.globaluifunctions.campaignmanager.eventManager.GetDynamicWarTakeOverContent("<ENEMY_INVASION_LAND>"));
			s = s.Replace("<ENEMY_INVASION_SEA>", UIFunctions.globaluifunctions.campaignmanager.eventManager.GetDynamicWarTakeOverContent("<ENEMY_INVASION_SEA>"));
			s = s.Replace("<ENEMY_INVASION_AIR>", UIFunctions.globaluifunctions.campaignmanager.eventManager.GetDynamicWarTakeOverContent("<ENEMY_INVASION_AIR>"));
		}
		else if (s.Contains("<FRIENDLY_TITLE_LIBERATION>") || s.Contains("<FRIENDLY_LIBERATION_LAND>") || s.Contains("<FRIENDLY_LIBERATION_SEA>") || s.Contains("<FRIENDLY_LIBERATION_AIR>"))
		{
			s = s.Replace("<FRIENDLY_TITLE_LIBERATION>", UIFunctions.globaluifunctions.campaignmanager.eventManager.GetDynamicWarTakeOverContent("<FRIENDLY_TITLE_LIBERATION>"));
			s = s.Replace("<FRIENDLY_LIBERATION_LAND>", UIFunctions.globaluifunctions.campaignmanager.eventManager.GetDynamicWarTakeOverContent("<FRIENDLY_LIBERATION_LAND>"));
			s = s.Replace("<FRIENDLY_LIBERATION_SEA>", UIFunctions.globaluifunctions.campaignmanager.eventManager.GetDynamicWarTakeOverContent("<FRIENDLY_LIBERATION_SEA>"));
			s = s.Replace("<FRIENDLY_LIBERATION_AIR>", UIFunctions.globaluifunctions.campaignmanager.eventManager.GetDynamicWarTakeOverContent("<FRIENDLY_LIBERATION_AIR>"));
		}
		else if (s.Contains("<FRIENDLY_TITLE_INVASION>") || s.Contains("<FRIENDLY_INVASION_LAND>") || s.Contains("<FRIENDLY_INVASION_SEA>") || s.Contains("<FRIENDLY_INVASION_AIR>"))
		{
			s = s.Replace("<FRIENDLY_TITLE_INVASION>", UIFunctions.globaluifunctions.campaignmanager.eventManager.GetDynamicWarTakeOverContent("<FRIENDLY_TITLE_INVASION>"));
			s = s.Replace("<FRIENDLY_INVASION_LAND>", UIFunctions.globaluifunctions.campaignmanager.eventManager.GetDynamicWarTakeOverContent("<FRIENDLY_INVASION_LAND>"));
			s = s.Replace("<FRIENDLY_INVASION_SEA>", UIFunctions.globaluifunctions.campaignmanager.eventManager.GetDynamicWarTakeOverContent("<FRIENDLY_INVASION_SEA>"));
			s = s.Replace("<FRIENDLY_INVASION_AIR>", UIFunctions.globaluifunctions.campaignmanager.eventManager.GetDynamicWarTakeOverContent("<FRIENDLY_INVASION_AIR>"));
		}
		else if (s.Contains("<ENEMY_TITLE_LIBERATION>") || s.Contains("<ENEMY_LIBERATION_LAND>") || s.Contains("<ENEMY_LIBERATION_SEA>") || s.Contains("<ENEMY_LIBERATION_AIR>"))
		{
			s = s.Replace("<ENEMY_TITLE_LIBERATION>", UIFunctions.globaluifunctions.campaignmanager.eventManager.GetDynamicWarTakeOverContent("<ENEMY_TITLE_LIBERATION>"));
			s = s.Replace("<ENEMY_LIBERATION_LAND>", UIFunctions.globaluifunctions.campaignmanager.eventManager.GetDynamicWarTakeOverContent("<ENEMY_LIBERATION_LAND>"));
			s = s.Replace("<ENEMY_LIBERATION_SEA>", UIFunctions.globaluifunctions.campaignmanager.eventManager.GetDynamicWarTakeOverContent("<ENEMY_LIBERATION_SEA>"));
			s = s.Replace("<ENEMY_LIBERATION_AIR>", UIFunctions.globaluifunctions.campaignmanager.eventManager.GetDynamicWarTakeOverContent("<ENEMY_LIBERATION_AIR>"));
		}
		if (s.Contains("<INVASION_SECOND>"))
		{
			s = s.Replace("<INVASION_SECOND>", this.GetRandomTextLineFromFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/invasion_second")));
		}
		if (s.Contains("<LIBERATION_SECOND>"))
		{
			s = s.Replace("<LIBERATION_SECOND>", this.GetRandomTextLineFromFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/liberation_second")));
		}
		if (s.Contains("<LANDCOUNTRY>"))
		{
			s = s.Replace("<LANDCOUNTRY>", UIFunctions.globaluifunctions.campaignmanager.campaignregionwaypoints[this.campaignmanager.eventManager.lastZoneModified].country);
		}
		if (s.Contains("<LANDLOCAL>"))
		{
			s = s.Replace("<LANDLOCAL>", UIFunctions.globaluifunctions.campaignmanager.campaignregionwaypoints[this.campaignmanager.eventManager.lastZoneModified].waypointDescriptiveName);
		}
		if (s.Contains("<WARSTATUS1>"))
		{
			s = s.Replace("<WARSTATUS1>", UIFunctions.globaluifunctions.campaignmanager.eventManager.GetDynamicWarContent("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/warstatus1"));
		}
		if (s.Contains("<WARSTATUS2>"))
		{
			s = s.Replace("<WARSTATUS2>", UIFunctions.globaluifunctions.campaignmanager.eventManager.GetDynamicWarContent("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/warstatus2"));
		}
		if (s.Contains("<FRIENDLY>"))
		{
			s = s.Replace("<FRIENDLY>", this.GetRandomTextBasedOnTag("FRIENDLY", this.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/combatants")));
		}
		if (s.Contains("<ENEMY>"))
		{
			s = s.Replace("<ENEMY>", this.GetRandomTextBasedOnTag("ENEMY", this.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/combatants")));
		}
		if (s.Contains("<FRIENDLY_PREFIX>"))
		{
			s = s.Replace("<FRIENDLY_PREFIX>", this.GetRandomTextBasedOnTag("FRIENDLY_PREFIX", this.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/combatants")));
		}
		if (s.Contains("<ENEMY_PREFIX>"))
		{
			s = s.Replace("<ENEMY_PREFIX>", this.GetRandomTextBasedOnTag("ENEMY_PREFIX", this.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/combatants")));
		}
		if (s.Contains("<FRIENDLY_SUFFIX>"))
		{
			s = s.Replace("<FRIENDLY_SUFFIX>", this.GetRandomTextBasedOnTag("FRIENDLY_SUFFIX", this.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/combatants")));
		}
		if (s.Contains("<ENEMY_SUFFIX>"))
		{
			s = s.Replace("<ENEMY_SUFFIX>", this.GetRandomTextBasedOnTag("ENEMY_SUFFIX", this.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/combatants")));
		}
		if (s.Contains("<FRIENDLY_SINGULAR>"))
		{
			s = s.Replace("<FRIENDLY_SINGULAR>", this.GetRandomTextBasedOnTag("FRIENDLY_SINGULAR", this.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/combatants")));
		}
		if (s.Contains("<ENEMY_SINGULAR>"))
		{
			s = s.Replace("<ENEMY_SINGULAR>", this.GetRandomTextBasedOnTag("ENEMY_SINGULAR", this.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/combatants")));
		}
		if (s.Contains("<STARTLOCATION>"))
		{
			s = s.Replace("<STARTLOCATION>", this.GetOrdersField("<STARTLOCATION>"));
		}
		if (s.Contains("<ENDLOCATION>"))
		{
			s = s.Replace("<ENDLOCATION>", this.GetOrdersField("<ENDLOCATION>"));
		}
		if (s.Contains("<MUSTUSELOCATION0>"))
		{
			s = s.Replace("<MUSTUSELOCATION0>", this.GetOrdersField("<MUSTUSELOCATION0>"));
		}
		if (s.Contains("<COUNTRY>"))
		{
			s = s.Replace("<COUNTRY>", this.GetOrdersField("<COUNTRY>"));
		}
		if (s.Contains("<WEAPONNUMBER>"))
		{
			s = s.Replace("<WEAPONNUMBER>", UIFunctions.globaluifunctions.campaignmanager.campaignmissions[UIFunctions.globaluifunctions.campaignmanager.campaignTaskForces[UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.currentMissionTaskForceID].missionID].numberOfRequiredWeaponWord);
		}
		if (s.Contains("<WEAPONNUMBERDIGITS>"))
		{
			s = s.Replace("<WEAPONNUMBERDIGITS>", UIFunctions.globaluifunctions.campaignmanager.campaignmissions[UIFunctions.globaluifunctions.campaignmanager.campaignTaskForces[UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.currentMissionTaskForceID].missionID].numberOfRequiredWeapon.ToString());
		}
		if (s.Contains("<WEAPONREQUIRED>"))
		{
			s = s.Replace("<WEAPONREQUIRED>", UIFunctions.globaluifunctions.database.databaseweapondata[UIFunctions.globaluifunctions.campaignmanager.campaignmissions[UIFunctions.globaluifunctions.campaignmanager.campaignTaskForces[UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.currentMissionTaskForceID].missionID].requiresWeaponID].weaponName);
		}
		if (s.Contains("<SEALSONBOARD>"))
		{
			string newValue = "lime";
			if (!GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.sealsOnBoard)
			{
				newValue = "red";
			}
			s = s.Replace("<SEALSONBOARD>", newValue);
		}
		if (s.Contains("<REQUIREDWEAPONSONBOARD>"))
		{
			string newValue2 = "lime";
			int num = 0;
			for (int i = 0; i < GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.torpedoNames.Length; i++)
			{
				if (GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.torpedoNames[i] == UIFunctions.globaluifunctions.database.databaseweapondata[UIFunctions.globaluifunctions.campaignmanager.campaignmissions[UIFunctions.globaluifunctions.campaignmanager.campaignTaskForces[UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.currentMissionTaskForceID].missionID].requiresWeaponID].weaponName)
				{
					num += GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.currentTorpsOnBoard[i];
				}
			}
			if (GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.hasVLS)
			{
				for (int j = 0; j < GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.vlsTorpedoNames.Length; j++)
				{
					if (GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.vlsTorpedoNames[j] == UIFunctions.globaluifunctions.database.databaseweapondata[UIFunctions.globaluifunctions.campaignmanager.campaignmissions[UIFunctions.globaluifunctions.campaignmanager.campaignTaskForces[UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.currentMissionTaskForceID].missionID].requiresWeaponID].weaponName)
					{
						num += GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.vlsCurrentTorpsOnBoard[j];
					}
				}
			}
			if (num < UIFunctions.globaluifunctions.campaignmanager.campaignmissions[UIFunctions.globaluifunctions.campaignmanager.campaignTaskForces[UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.currentMissionTaskForceID].missionID].numberOfRequiredWeapon)
			{
				newValue2 = "red";
			}
			s = s.Replace("<REQUIREDWEAPONSONBOARD>", newValue2);
		}
		if (!uppercase)
		{
			return s;
		}
		return s.ToUpper();
	}

	// Token: 0x06000AA1 RID: 2721 RVA: 0x00091648 File Offset: 0x0008F848
	private string GetHullPrefix(string prefabName)
	{
		string[] array = prefabName.Split(new char[]
		{
			'_'
		});
		string result = string.Empty;
		if (array[0] == "usn")
		{
			result = LanguageManager.interfaceDictionary["NavyPrefixUSS"];
		}
		else
		{
			result = string.Empty;
		}
		return result;
	}

	// Token: 0x06000AA2 RID: 2722 RVA: 0x0009169C File Offset: 0x0008F89C
	public string GetPlayerVesselName()
	{
		string str = string.Empty;
		str = str + this.GetHullPrefix(this.database.databaseshipdata[this.playerfunctions.playerVesselClass].shipPrefabName) + " ";
		str = str + this.database.databaseshipdata[this.playerfunctions.playerVesselClass].playerClassNames[this.playerfunctions.playerVesselInstance].ToUpper() + " ";
		return str + this.database.databaseshipdata[this.playerfunctions.playerVesselClass].playerClassHullNumbers[this.playerfunctions.playerVesselInstance];
	}

	// Token: 0x06000AA3 RID: 2723 RVA: 0x00091748 File Offset: 0x0008F948
	private string GetOrdersField(string field)
	{
		if (this.campaignmanager.eventManager.eventDebugMode)
		{
			return string.Empty;
		}
		string result = string.Empty;
		CampaignTaskForce campaignTaskForce = UIFunctions.globaluifunctions.campaignmanager.campaignTaskForces[UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.currentMissionTaskForceID];
		switch (field)
		{
		case "<STARTLOCATION>":
			if (campaignTaskForce.startIsLocation)
			{
				result = UIFunctions.globaluifunctions.campaignmanager.campaignlocations[campaignTaskForce.startPositionID].locationName;
			}
			else
			{
				result = UIFunctions.globaluifunctions.campaignmanager.campaignmapwaypoints[campaignTaskForce.startPositionID].waypointDescriptiveName;
			}
			break;
		case "<ENDLOCATION>":
			if (campaignTaskForce.endIsLocation)
			{
				result = UIFunctions.globaluifunctions.campaignmanager.campaignlocations[campaignTaskForce.endLocationID].locationName;
			}
			else
			{
				result = UIFunctions.globaluifunctions.campaignmanager.campaignmapwaypoints[campaignTaskForce.mustUseWaypoints[campaignTaskForce.mustUseWaypoints.Length - 1]].waypointDescriptiveName;
			}
			break;
		case "<MUSTUSELOCATION0>":
			result = UIFunctions.globaluifunctions.campaignmanager.campaignmapwaypoints[campaignTaskForce.mustUseWaypoints[0]].waypointDescriptiveName;
			break;
		case "<COUNTRY>":
			if (!campaignTaskForce.endIsLocation)
			{
				result = UIFunctions.globaluifunctions.campaignmanager.campaignmapwaypoints[campaignTaskForce.mustUseWaypoints[0]].waypointDescriptiveName;
			}
			else
			{
				result = UIFunctions.globaluifunctions.campaignmanager.campaignlocations[campaignTaskForce.endLocationID].country;
			}
			break;
		}
		return result;
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x00091930 File Offset: 0x0008FB30
	public string ReadDirectTextFromFile(string filePath)
	{
		string text = string.Empty;
		string[] array = UIFunctions.globaluifunctions.textparser.OpenTextDataFile(filePath);
		for (int i = 0; i < array.Length; i++)
		{
			string text2 = array[i].Trim();
			text2 = this.PopulateDynamicTags(text2, true);
			text = text + text2 + "\n";
		}
		return text;
	}

	// Token: 0x06000AA5 RID: 2725 RVA: 0x00091988 File Offset: 0x0008FB88
	private string GetRandomTextBasedOnTag(string tag, string filePath)
	{
		string empty = string.Empty;
		string[] array = UIFunctions.globaluifunctions.textparser.OpenTextDataFile(filePath);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			if (array2[0].Trim() == tag)
			{
				string[] array3 = array2[1].Split(new char[]
				{
					'|'
				});
				int num = UnityEngine.Random.Range(0, array3.Length);
				return array3[num];
			}
		}
		return null;
	}

	// Token: 0x06000AA6 RID: 2726 RVA: 0x00091A10 File Offset: 0x0008FC10
	public string GetRandomTextLineFromFile(string filePath)
	{
		string[] array = UIFunctions.globaluifunctions.textparser.OpenTextDataFile(filePath);
		List<string> list = new List<string>();
		for (int i = 0; i < array.Length; i++)
		{
			array[0].Trim();
			if (array[i].Length > 0)
			{
				list.Add(array[i]);
			}
		}
		if (list.Count<string>() > 0)
		{
			return list[UnityEngine.Random.Range(0, list.Count)];
		}
		return null;
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x00091A88 File Offset: 0x0008FC88
	public string GetFilePathFromString(string inputString)
	{
		inputString = inputString.Trim();
		if (inputString.Contains("language"))
		{
			inputString = inputString.Replace("language", "language" + GameDataManager.language);
		}
		string[] array = inputString.Split(new char[]
		{
			'/'
		});
		string text = array[0];
		for (int i = 0; i < array.Length - 1; i++)
		{
			text = Path.Combine(text, array[i + 1]);
		}
		return text;
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x00091B04 File Offset: 0x0008FD04
	public void SetImageSprite(string imageName, Image image)
	{
		if (imageName == "NONE")
		{
			image.gameObject.SetActive(false);
			return;
		}
		string text = "file://" + Application.streamingAssetsPath + "/override/" + imageName;
		bool flag = false;
		WWW www = new WWW(text);
		while (!www.isDone)
		{
		}
		if (www.error == null)
		{
			Texture texture = www.texture;
			Sprite sprite = Sprite.Create(texture as Texture2D, new Rect(0f, 0f, (float)texture.width, (float)texture.height), Vector2.zero);
			image.sprite = sprite;
			flag = true;
		}
		if (!flag)
		{
			text = "file://" + Application.streamingAssetsPath + "/default/" + imageName;
			www = new WWW(text);
			while (!www.isDone)
			{
			}
			if (www.error == null)
			{
				Texture texture2 = www.texture;
				Sprite sprite2 = Sprite.Create(texture2 as Texture2D, new Rect(0f, 0f, (float)texture2.width, (float)texture2.height), Vector2.zero);
				image.sprite = sprite2;
				flag = true;
			}
		}
		if (!flag)
		{
			text = UIFunctions.globaluifunctions.textparser.GetFilePathFromString(imageName);
			image.sprite = Resources.Load<Sprite>(text);
		}
		if (!image.gameObject.activeSelf)
		{
			image.gameObject.SetActive(true);
		}
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x00091C6C File Offset: 0x0008FE6C
	public MovieTexture GetMovieTexture(string moviePath)
	{
		if (moviePath == "FALSE")
		{
			return null;
		}
		string url = "file://" + Application.streamingAssetsPath + "/override/" + moviePath;
		WWW www = new WWW(url);
		while (!www.isDone)
		{
		}
		if (www.error == null)
		{
			return www.movie;
		}
		url = "file://" + Application.streamingAssetsPath + "/default/" + moviePath;
		www = new WWW(url);
		while (!www.isDone)
		{
		}
		if (www.error == null)
		{
			return www.movie;
		}
		url = UIFunctions.globaluifunctions.textparser.GetFilePathFromString(moviePath);
		return Resources.Load<MovieTexture>(moviePath);
	}

	// Token: 0x06000AAA RID: 2730 RVA: 0x00091D20 File Offset: 0x0008FF20
	public AudioClip GetAudioClip(string clipPath)
	{
		if (clipPath == "FALSE")
		{
			return null;
		}
		string url = "file://" + Application.streamingAssetsPath + "/override/" + clipPath;
		WWW www = new WWW(url);
		while (!www.isDone)
		{
		}
		if (www.error == null)
		{
			return www.audioClip;
		}
		url = "file://" + Application.streamingAssetsPath + "/default/" + clipPath.Trim();
		www = new WWW(url);
		while (!www.isDone)
		{
		}
		if (www.error == null)
		{
			return www.audioClip;
		}
		url = UIFunctions.globaluifunctions.textparser.GetFilePathFromString(clipPath);
		return Resources.Load<AudioClip>(clipPath);
	}

	// Token: 0x06000AAB RID: 2731 RVA: 0x00091DDC File Offset: 0x0008FFDC
	public Texture2D GetTexture(string texturePath)
	{
		if (texturePath == "FALSE")
		{
			return null;
		}
		string text = "file://" + Application.streamingAssetsPath + "/override/" + texturePath;
		WWW www = new WWW(text);
		while (!www.isDone)
		{
		}
		if (www.error == null)
		{
			return www.texture;
		}
		text = "file://" + Application.streamingAssetsPath + "/default/" + texturePath;
		www = new WWW(text);
		while (!www.isDone)
		{
		}
		if (www.error == null)
		{
			return www.texture;
		}
		text = UIFunctions.globaluifunctions.textparser.GetFilePathFromString(texturePath);
		return Resources.Load<Texture2D>(text);
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x00091E90 File Offset: 0x00090090
	public Sprite GetSprite(string spritePath)
	{
		if (spritePath == "FALSE")
		{
			return null;
		}
		string text = "file://" + Application.streamingAssetsPath + "/override/" + spritePath;
		WWW www = new WWW(text);
		while (!www.isDone)
		{
		}
		if (www.error == null)
		{
			Texture texture = www.texture;
			return Sprite.Create(texture as Texture2D, new Rect(0f, 0f, (float)texture.width, (float)texture.height), Vector2.zero);
		}
		text = "file://" + Application.streamingAssetsPath + "/default/" + spritePath;
		www = new WWW(text);
		while (!www.isDone)
		{
		}
		if (www.error == null)
		{
			Texture texture2 = www.texture;
			return Sprite.Create(texture2 as Texture2D, new Rect(0f, 0f, (float)texture2.width, (float)texture2.height), Vector2.zero);
		}
		text = UIFunctions.globaluifunctions.textparser.GetFilePathFromString(spritePath);
		return Resources.Load<Sprite>(text);
	}

	// Token: 0x0400103D RID: 4157
	public LevelLoadData levelloaddata;

	// Token: 0x0400103E RID: 4158
	public PlayerFunctions playerfunctions;

	// Token: 0x0400103F RID: 4159
	public Database database;

	// Token: 0x04001040 RID: 4160
	public CampaignManager campaignmanager;

	// Token: 0x04001041 RID: 4161
	public Font[] fonts;

	// Token: 0x04001042 RID: 4162
	private string[] positiondata;

	// Token: 0x04001043 RID: 4163
	public Mesh[] allMeshes;

	// Token: 0x04001044 RID: 4164
	public Image[] hudSprites;

	// Token: 0x04001045 RID: 4165
	public Material tapeGuage;

	// Token: 0x04001046 RID: 4166
	public Material tacMapBackground;

	// Token: 0x04001047 RID: 4167
	public bool hudBuiltOnce;
}
