using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000129 RID: 297
public class InGameReference : MonoBehaviour
{
	// Token: 0x06000808 RID: 2056 RVA: 0x0004ED0C File Offset: 0x0004CF0C
	public void SetInGameUnitReference(bool switchOn)
	{
		if (switchOn)
		{
			this.ingameReferencePanels[2].gameObject.SetActive(true);
			this.ingameReferencePanels[0].gameObject.SetActive(false);
		}
		else
		{
			this.ingameReferencePanels[2].gameObject.SetActive(false);
		}
	}

	// Token: 0x06000809 RID: 2057 RVA: 0x0004ED60 File Offset: 0x0004CF60
	public void GoToProfile(string type)
	{
		if (type == "OWN")
		{
			this.RefreshInGameReferenceData(UIFunctions.globaluifunctions.playerfunctions.playerVesselClass);
		}
		else if (type == "MERCHANT" || type == "SUBMARINE")
		{
			for (int i = 0; i < UIFunctions.globaluifunctions.playerfunctions.otherVesselList.Length; i++)
			{
				if (UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.otherVesselList[i]].shipType == type)
				{
					this.currentInGameReferenceIndex = i;
					UIFunctions.globaluifunctions.playerfunctions.currentSignatureIndex = i;
					this.RefreshInGameReferenceData(UIFunctions.globaluifunctions.playerfunctions.playerVesselClass);
					UIFunctions.globaluifunctions.playerfunctions.SetProfileGraphic();
					break;
				}
			}
		}
		else
		{
			for (int j = 0; j < UIFunctions.globaluifunctions.playerfunctions.otherVesselList.Length; j++)
			{
				if (UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.otherVesselList[j]].shipType == "ESCORT" || UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.otherVesselList[j]].shipType == "CAPITAL")
				{
					this.currentInGameReferenceIndex = j;
					UIFunctions.globaluifunctions.playerfunctions.currentSignatureIndex = j;
					this.RefreshInGameReferenceData(UIFunctions.globaluifunctions.playerfunctions.playerVesselClass);
					UIFunctions.globaluifunctions.playerfunctions.SetProfileGraphic();
					break;
				}
			}
		}
	}

	// Token: 0x0600080A RID: 2058 RVA: 0x0004EF1C File Offset: 0x0004D11C
	public void SetInGameReferencePanel()
	{
		if (this.mainPanel.activeSelf)
		{
			this.mainPanel.SetActive(false);
		}
		else
		{
			this.mainPanel.SetActive(true);
			this.RefreshInGameReferenceData(UIFunctions.globaluifunctions.playerfunctions.otherVesselList[UIFunctions.globaluifunctions.playerfunctions.currentSignatureIndex]);
		}
	}

	// Token: 0x0600080B RID: 2059 RVA: 0x0004EF7C File Offset: 0x0004D17C
	public void RefreshInGameReferenceData(int index)
	{
		this.currentInGameReferenceIndex = index;
		string str = UIFunctions.globaluifunctions.database.databaseshipdata[index].shipPrefabName + "_profile";
		this.ingameReferencePanels[4].sprite = UIFunctions.globaluifunctions.textparser.GetSprite(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("vessels/profile/" + str + ".png"));
		this.infoWindow.SetActive(false);
		this.BuildInGameReferenceText(index);
	}

	// Token: 0x0600080C RID: 2060 RVA: 0x0004F000 File Offset: 0x0004D200
	private void BuildInGameReferenceText(int index)
	{
		this.infoButtonLinks = new string[10];
		this.infoButtonIndexes = new int[10];
		this.infoButtonInUse = new bool[10];
		int index2 = 0;
		DatabaseShipData databaseShipData = UIFunctions.globaluifunctions.database.databaseshipdata[index];
		this.ingameReferenceTexts[0].text = string.Concat(new string[]
		{
			"<color=",
			this.highlightColorWord,
			">",
			databaseShipData.shipclass,
			" ",
			databaseShipData.shipDesignation,
			"</color>"
		});
		this.ingameReferenceTexts[1].text = string.Format("{0:#,0}", databaseShipData.displacement) + " " + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceTons") + "\n";
		if (databaseShipData.displayLength == 0f)
		{
			Text text = this.ingameReferenceTexts[1];
			text.text = text.text + databaseShipData.length.ToString() + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceMetre") + " x ";
		}
		else
		{
			Text text2 = this.ingameReferenceTexts[1];
			text2.text = text2.text + databaseShipData.displayLength.ToString() + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceMetre") + " x ";
		}
		if (databaseShipData.displayBeam == 0f)
		{
			Text text3 = this.ingameReferenceTexts[1];
			text3.text = text3.text + databaseShipData.beam.ToString() + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceMetre") + "\n";
		}
		else
		{
			Text text4 = this.ingameReferenceTexts[1];
			text4.text = text4.text + databaseShipData.displayBeam.ToString() + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceMetre") + "\n";
		}
		float num = databaseShipData.surfacespeed;
		if (databaseShipData.shipType == "SUBMARINE")
		{
			num = databaseShipData.submergedspeed;
		}
		string text6;
		if (databaseShipData.shipType != "BIOLOGIC")
		{
			Text text5 = this.ingameReferenceTexts[1];
			text6 = text5.text;
			text5.text = string.Concat(new string[]
			{
				text6,
				num.ToString(),
				" ",
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceKnot"),
				", ",
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceCrew"),
				" ",
				databaseShipData.crew.ToString()
			});
		}
		else
		{
			Text text7 = this.ingameReferenceTexts[1];
			text7.text = text7.text + num.ToString() + " " + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceKnot");
		}
		if (databaseShipData.shipType == "SUBMARINE")
		{
			Text text8 = this.ingameReferenceTexts[1];
			text6 = text8.text;
			text8.text = string.Concat(new object[]
			{
				text6,
				"\n",
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceTestDepth"),
				" ",
				databaseShipData.testDepth,
				" ",
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceFeet")
			});
		}
		else
		{
			Text text9 = this.ingameReferenceTexts[1];
			text9.text += "\n";
		}
		Text text10 = this.ingameReferenceTexts[1];
		text6 = text10.text;
		text10.text = string.Concat(new string[]
		{
			text6,
			"\n",
			string.Format("{0:#,0}", databaseShipData.selfnoise),
			" ",
			LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Decibels"),
			" "
		});
		if (databaseShipData.selfnoise <= 130f)
		{
			Text text11 = this.ingameReferenceTexts[1];
			text11.text += LanguageManager.interfaceDictionary["ReferenceVeryQuiet"];
		}
		else if (databaseShipData.selfnoise <= 140f)
		{
			Text text12 = this.ingameReferenceTexts[1];
			text12.text += LanguageManager.interfaceDictionary["ReferenceQuiet"];
		}
		else if (databaseShipData.selfnoise <= 150f)
		{
			Text text13 = this.ingameReferenceTexts[1];
			text13.text += LanguageManager.interfaceDictionary["ReferenceSemiQuiet"];
		}
		else if (databaseShipData.selfnoise <= 160f)
		{
			Text text14 = this.ingameReferenceTexts[1];
			text14.text += LanguageManager.interfaceDictionary["ReferenceSemiNoisy"];
		}
		else if (databaseShipData.selfnoise <= 170f)
		{
			Text text15 = this.ingameReferenceTexts[1];
			text15.text += LanguageManager.interfaceDictionary["ReferenceNoisy"];
		}
		else
		{
			Text text16 = this.ingameReferenceTexts[1];
			text16.text += LanguageManager.interfaceDictionary["ReferenceVeryNoisy"];
		}
		this.ingameReferenceTexts[2].text = "<color=" + this.highlightColorWord + ">";
		if (databaseShipData.passiveSonarID > -1)
		{
			Text text17 = this.ingameReferenceTexts[2];
			text6 = text17.text;
			text17.text = string.Concat(new string[]
			{
				text6,
				UIFunctions.globaluifunctions.database.databasesonardata[databaseShipData.passiveSonarID].sonarDisplayName,
				" ",
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "FrequencyAbbreviated" + UIFunctions.globaluifunctions.database.databasesonardata[databaseShipData.passiveSonarID].sonarFrequencies[0]),
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "FrequencyAbbreviated"),
				" ",
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferencePassive"),
				"\n"
			});
			index2 = this.SetInfoButton(index2, "SONAR", databaseShipData.passiveSonarID);
		}
		if (databaseShipData.activeSonarID != databaseShipData.passiveSonarID && databaseShipData.activeSonarID > -1)
		{
			Text text18 = this.ingameReferenceTexts[2];
			text6 = text18.text;
			text18.text = string.Concat(new string[]
			{
				text6,
				UIFunctions.globaluifunctions.database.databasesonardata[databaseShipData.activeSonarID].sonarDisplayName,
				" ",
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "FrequencyAbbreviated" + UIFunctions.globaluifunctions.database.databasesonardata[databaseShipData.activeSonarID].sonarFrequencies[0]),
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "FrequencyAbbreviated"),
				" ",
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceActive"),
				"\n"
			});
			index2 = this.SetInfoButton(index2, "SONAR", databaseShipData.activeSonarID);
		}
		if (databaseShipData.towedSonarID > -1)
		{
			Text text19 = this.ingameReferenceTexts[2];
			text6 = text19.text;
			text19.text = string.Concat(new string[]
			{
				text6,
				UIFunctions.globaluifunctions.database.databasesonardata[databaseShipData.towedSonarID].sonarDisplayName,
				" ",
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "FrequencyAbbreviated" + UIFunctions.globaluifunctions.database.databasesonardata[databaseShipData.towedSonarID].sonarFrequencies[0]),
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "FrequencyAbbreviated"),
				" ",
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceTowed"),
				"\n"
			});
			index2 = this.SetInfoButton(index2, "SONAR", databaseShipData.towedSonarID);
		}
		if (databaseShipData.radarID > -1)
		{
			Text text20 = this.ingameReferenceTexts[2];
			text6 = text20.text;
			text20.text = string.Concat(new string[]
			{
				text6,
				UIFunctions.globaluifunctions.database.databaseradardata[databaseShipData.radarID].radarDisplayName,
				" ",
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceRadar"),
				"\n"
			});
			index2 = this.SetInfoButton(index2, "RADAR", databaseShipData.radarID);
		}
		Text text21 = this.ingameReferenceTexts[2];
		text21.text += "</color>";
		this.ingameReferenceTexts[3].text = "<color=" + this.highlightColorWord + ">";
		index2 = 5;
		if (databaseShipData.missileGameObject != null && databaseShipData.missilesPerLauncher.Length > 0)
		{
			Text text22 = this.ingameReferenceTexts[3];
			text6 = text22.text;
			text22.text = string.Concat(new string[]
			{
				text6,
				UIFunctions.globaluifunctions.database.databaseweapondata[databaseShipData.missileType].weaponName,
				" ",
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceMissiles"),
				"\n"
			});
			index2 = this.SetInfoButton(index2, "WEAPON", databaseShipData.missileType);
		}
		List<int> list = new List<int>();
		if (databaseShipData.torpedotubes > 0)
		{
			for (int i = 0; i < databaseShipData.torpedotypes.Length; i++)
			{
				if (UIFunctions.globaluifunctions.database.databaseweapondata[databaseShipData.torpedoIDs[i]].weaponType != "DECOY")
				{
					list.Add(databaseShipData.torpedoIDs[i]);
					Text text23 = this.ingameReferenceTexts[3];
					text23.text = text23.text + UIFunctions.globaluifunctions.database.databaseweapondata[databaseShipData.torpedoIDs[i]].weaponName + " ";
					index2 = this.SetInfoButton(index2, "WEAPON", databaseShipData.torpedoIDs[i]);
					if (UIFunctions.globaluifunctions.database.databaseweapondata[databaseShipData.torpedoIDs[i]].isMissile)
					{
						Text text24 = this.ingameReferenceTexts[3];
						text24.text = text24.text + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceMissiles") + "\n";
					}
					else if (UIFunctions.globaluifunctions.database.databaseweapondata[databaseShipData.torpedoIDs[i]].isDecoy)
					{
						Text text25 = this.ingameReferenceTexts[3];
						text25.text = text25.text + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceDecoys") + "\n";
					}
					else
					{
						Text text26 = this.ingameReferenceTexts[3];
						text26.text = text26.text + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceTorpedoes") + "\n";
					}
				}
			}
		}
		if (databaseShipData.vlsTorpedotypes != null && databaseShipData.vlsTorpedotypes.Length > 0)
		{
			for (int j = 0; j < databaseShipData.vlsTorpedotypes.Length; j++)
			{
				if (UIFunctions.globaluifunctions.database.databaseweapondata[databaseShipData.vlsTorpedoIDs[j]].weaponType != "DECOY" && !list.Contains(databaseShipData.vlsTorpedoIDs[j]))
				{
					list.Add(databaseShipData.vlsTorpedoIDs[j]);
					Text text27 = this.ingameReferenceTexts[3];
					text27.text = text27.text + UIFunctions.globaluifunctions.database.databaseweapondata[databaseShipData.vlsTorpedoIDs[j]].weaponName + " ";
					index2 = this.SetInfoButton(index2, "WEAPON", databaseShipData.vlsTorpedoIDs[j]);
					if (UIFunctions.globaluifunctions.database.databaseweapondata[databaseShipData.vlsTorpedoIDs[j]].isMissile)
					{
						Text text28 = this.ingameReferenceTexts[3];
						text28.text = text28.text + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceMissiles") + "\n";
					}
					else if (UIFunctions.globaluifunctions.database.databaseweapondata[databaseShipData.vlsTorpedoIDs[j]].isDecoy)
					{
						Text text29 = this.ingameReferenceTexts[3];
						text29.text = text29.text + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceDecoys") + "\n";
					}
					else
					{
						Text text30 = this.ingameReferenceTexts[3];
						text30.text = text30.text + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceTorpedoes") + "\n";
					}
				}
			}
		}
		if (databaseShipData.rbuLauncherTypes != null)
		{
			string text31 = string.Empty;
			for (int k = 0; k < databaseShipData.rbuLauncherTypes.Length; k++)
			{
				string depthchargeName = UIFunctions.globaluifunctions.database.databasedepthchargedata[databaseShipData.rbuLauncherTypes[k]].depthchargeName;
				if (!text31.Contains(depthchargeName))
				{
					text31 = text31 + depthchargeName + "\n";
					index2 = this.SetInfoButton(index2, "DEPTHWEAPON", databaseShipData.rbuLauncherTypes[k]);
					break;
				}
			}
			Text text32 = this.ingameReferenceTexts[3];
			text32.text += text31;
		}
		Text text33 = this.ingameReferenceTexts[3];
		text33.text += "</color>";
		if (databaseShipData.navalGunTypes != null)
		{
			string text34 = string.Empty;
			for (int l = 0; l < databaseShipData.navalGunTypes.Length; l++)
			{
				string depthchargeName2 = UIFunctions.globaluifunctions.database.databasedepthchargedata[databaseShipData.navalGunTypes[l]].depthchargeName;
				if (!text34.Contains(depthchargeName2))
				{
					text34 = text34 + depthchargeName2 + "\n";
				}
			}
			Text text35 = this.ingameReferenceTexts[3];
			text35.text += text34;
		}
		if (databaseShipData.gunProbability > 0f)
		{
			Text text36 = this.ingameReferenceTexts[3];
			text36.text = text36.text + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Reference30mmCIWS") + "\n";
		}
	}

	// Token: 0x0600080D RID: 2061 RVA: 0x0004FDEC File Offset: 0x0004DFEC
	private int SetInfoButton(int index, string type, int value)
	{
		this.infoButtonInUse[index] = true;
		this.infoButtonLinks[index] = type;
		this.infoButtonIndexes[index] = value;
		index++;
		return index;
	}

	// Token: 0x0600080E RID: 2062 RVA: 0x0004FE10 File Offset: 0x0004E010
	public void CloseInfoWindow()
	{
		this.infoWindow.gameObject.SetActive(false);
	}

	// Token: 0x0600080F RID: 2063 RVA: 0x0004FE24 File Offset: 0x0004E024
	public void GetInGameReferenceAdditionalInfo(int buttonIndex)
	{
		if (buttonIndex == -5)
		{
			if (!this.infoWindow.activeSelf)
			{
				this.infoWindow.gameObject.SetActive(true);
			}
			DatabaseShipData databaseShipData = UIFunctions.globaluifunctions.database.databaseshipdata[this.currentInGameReferenceIndex];
			this.ingameReferenceTexts[4].text = string.Empty;
			for (int i = 0; i < databaseShipData.history.Length; i++)
			{
				Text text = this.ingameReferenceTexts[4];
				text.text = text.text + databaseShipData.history[i] + "\n";
			}
			this.SetInfoWindowHeight();
			this.PlaceHighlight(this.buttonPositions[10]);
			this.ingameReferenceTexts[4].text = this.ingameReferenceTexts[4].text.Replace("\\n", "\n");
			return;
		}
		if (!this.infoButtonInUse[buttonIndex])
		{
			return;
		}
		if (!this.infoWindow.activeSelf)
		{
			this.infoWindow.gameObject.SetActive(true);
		}
		this.ingameReferenceTexts[4].text = string.Empty;
		this.PlaceHighlight(this.buttonPositions[buttonIndex]);
		if (this.infoButtonLinks[buttonIndex] == "SONAR")
		{
			DatabaseSonarData databaseSonarData = UIFunctions.globaluifunctions.database.databasesonardata[this.infoButtonIndexes[buttonIndex]];
			Text text2 = this.ingameReferenceTexts[4];
			text2.text += databaseSonarData.sonarDescription;
			this.SetInfoWindowHeight();
			this.ingameReferenceTexts[4].text = this.ingameReferenceTexts[4].text.Replace("\\n", "\n");
			return;
		}
		if (this.infoButtonLinks[buttonIndex] == "RADAR")
		{
			DatabaseRADARData databaseRADARData = UIFunctions.globaluifunctions.database.databaseradardata[this.infoButtonIndexes[buttonIndex]];
			Text text3 = this.ingameReferenceTexts[4];
			text3.text += databaseRADARData.radarDescription;
			this.SetInfoWindowHeight();
			this.ingameReferenceTexts[4].text = this.ingameReferenceTexts[4].text.Replace("\\n", "\n");
			return;
		}
		if (this.infoButtonLinks[buttonIndex] == "WEAPON")
		{
			DatabaseWeaponData databaseWeaponData = UIFunctions.globaluifunctions.database.databaseweapondata[this.infoButtonIndexes[buttonIndex]];
			Text text4 = this.ingameReferenceTexts[4];
			string text5 = text4.text;
			text4.text = string.Concat(new string[]
			{
				text5,
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceRange"),
				": ",
				string.Format("{0:#,0}", databaseWeaponData.rangeInYards),
				" ",
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceYard")
			});
			if (databaseWeaponData.weaponType == "MISSILE")
			{
				Text text6 = this.ingameReferenceTexts[4];
				text5 = text6.text;
				text6.text = string.Concat(new string[]
				{
					text5,
					" ",
					LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceAt"),
					" ",
					databaseWeaponData.activeRunSpeed.ToString(),
					" ",
					LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceKnot"),
					"\n"
				});
			}
			else
			{
				Text text7 = this.ingameReferenceTexts[4];
				text5 = text7.text;
				text7.text = string.Concat(new string[]
				{
					text5,
					" ",
					LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceAt"),
					" ",
					databaseWeaponData.runSpeed.ToString(),
					" ",
					LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceKnot"),
					"\n"
				});
			}
			Text text8 = this.ingameReferenceTexts[4];
			text5 = text8.text;
			text8.text = string.Concat(new string[]
			{
				text5,
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceMaxSpeed"),
				": ",
				databaseWeaponData.activeRunSpeed.ToString(),
				" ",
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceKnot"),
				"\n"
			});
			if (databaseWeaponData.sensorRange > 0f)
			{
				Text text9 = this.ingameReferenceTexts[4];
				text5 = text9.text;
				text9.text = string.Concat(new string[]
				{
					text5,
					LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceSeekerRange"),
					": ",
					string.Format("{0:#,0}", databaseWeaponData.sensorRange),
					" ",
					LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "ReferenceYard"),
					"\n"
				});
			}
			for (int j = 0; j < databaseWeaponData.weaponDescription.Length; j++)
			{
				Text text10 = this.ingameReferenceTexts[4];
				text10.text = text10.text + databaseWeaponData.weaponDescription[j] + "\n";
			}
			this.SetInfoWindowHeight();
			this.ingameReferenceTexts[4].text = this.ingameReferenceTexts[4].text.Replace("\\n", "\n");
			return;
		}
		if (this.infoButtonLinks[buttonIndex] == "DEPTHWEAPON")
		{
			DatabaseDepthChargeData databaseDepthChargeData = UIFunctions.globaluifunctions.database.databasedepthchargedata[this.infoButtonIndexes[buttonIndex]];
			Text text11 = this.ingameReferenceTexts[4];
			text11.text = text11.text + databaseDepthChargeData.depthchargeDescriptiveName + "\n";
			this.SetInfoWindowHeight();
			this.ingameReferenceTexts[4].text = this.ingameReferenceTexts[4].text.Replace("\\n", "\n");
			return;
		}
	}

	// Token: 0x06000810 RID: 2064 RVA: 0x000503F0 File Offset: 0x0004E5F0
	private void PlaceHighlight(Transform newTransform)
	{
		this.highlightMarker.transform.SetParent(newTransform);
		this.highlightMarker.transform.localPosition = Vector3.zero;
		this.highlightMarker.transform.SetParent(this.infoWindow.transform);
	}

	// Token: 0x06000811 RID: 2065 RVA: 0x00050440 File Offset: 0x0004E640
	private void SetInfoWindowHeight()
	{
		float preferredHeight = this.ingameReferenceTexts[4].preferredHeight;
		this.ingameReferenceTexts[4].rectTransform.sizeDelta = new Vector2(this.ingameReferenceTexts[4].rectTransform.sizeDelta.x, preferredHeight + 18f);
		this.infoScrollbar.value = 1f;
	}

	// Token: 0x04000BBC RID: 3004
	public GameObject mainPanel;

	// Token: 0x04000BBD RID: 3005
	public int currentInGameReferenceIndex;

	// Token: 0x04000BBE RID: 3006
	public Image[] ingameReferencePanels;

	// Token: 0x04000BBF RID: 3007
	public Text[] ingameReferenceTexts;

	// Token: 0x04000BC0 RID: 3008
	public bool[] infoButtonInUse;

	// Token: 0x04000BC1 RID: 3009
	public string[] infoButtonLinks;

	// Token: 0x04000BC2 RID: 3010
	public int[] infoButtonIndexes;

	// Token: 0x04000BC3 RID: 3011
	public Transform[] buttonPositions;

	// Token: 0x04000BC4 RID: 3012
	public GameObject infoWindow;

	// Token: 0x04000BC5 RID: 3013
	public Scrollbar infoScrollbar;

	// Token: 0x04000BC6 RID: 3014
	public Transform highlightMarker;

	// Token: 0x04000BC7 RID: 3015
	public string highlightColorWord;
}
