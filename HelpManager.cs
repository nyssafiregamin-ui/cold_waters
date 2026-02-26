using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000126 RID: 294
public class HelpManager : MonoBehaviour
{
	// Token: 0x060007F3 RID: 2035 RVA: 0x0004D0E4 File Offset: 0x0004B2E4
	private void InitialiseHelp()
	{
		if (this.helpInitialised)
		{
			return;
		}
		this.helpInitialised = true;
		this.helpTabBackgrounds = new Image[this.helpTabs.Length];
		for (int i = 0; i < this.helpTabBackgrounds.Length; i++)
		{
			this.helpTabBackgrounds[i] = this.helpTabs[i].GetComponent<Image>();
			this.helpTabBackgrounds[i].GetComponentInChildren<Text>().text = LanguageManager.interfaceDictionary[this.helpTabBackgrounds[i].GetComponentInChildren<Text>().name];
		}
		this.helpImages = new List<GameObject>();
		this.chapterSelection.sprite = UIFunctions.globaluifunctions.textparser.GetSprite(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("help_images/chapter_highlight.png"));
	}

	// Token: 0x060007F4 RID: 2036 RVA: 0x0004D1AC File Offset: 0x0004B3AC
	public void InitialiseHelpDictionaries()
	{
		this.tagsForKeys = new Dictionary<string, List<KeyCode>>();
		foreach (string text in InputManager.globalInputManager.buttonKeys.Keys)
		{
			this.tagsForKeys.Add("<KEY:" + text + ">", InputManager.globalInputManager.buttonKeys[text]);
		}
		this.tagsForInterface = new Dictionary<string, string>();
		foreach (string text2 in LanguageManager.interfaceDictionary.Keys)
		{
			this.tagsForInterface.Add("<DICTIONARY:" + text2 + ">", LanguageManager.interfaceDictionary[text2]);
		}
	}

	// Token: 0x060007F5 RID: 2037 RVA: 0x0004D2D0 File Offset: 0x0004B4D0
	private void OnEnable()
	{
		this.lastTimeScale = Time.timeScale;
		Time.timeScale = 0f;
		if (!UIFunctions.globaluifunctions.GUICameraObject.enabled)
		{
		}
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x0004D2FC File Offset: 0x0004B4FC
	public void OpenHelp()
	{
		if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.background.activeSelf)
		{
			this.wasInTacMap = true;
		}
		if (UIFunctions.globaluifunctions.levelloadmanager.currentMuseumInstantiatedObject != null)
		{
			UIFunctions.globaluifunctions.levelloadmanager.currentMuseumInstantiatedObject.SetActive(false);
		}
		this.ClearHelpImages();
		if (!this.helpInitialised)
		{
			this.InitialiseHelp();
		}
		if (this.helpBackground.sprite == null)
		{
			this.helpBackground.sprite = UIFunctions.globaluifunctions.textparser.GetSprite(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("help_images/help_background.png"));
		}
		if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.background.activeSelf && UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.playerMapContact != null && !UIFunctions.globaluifunctions.menuSystem[2].activeSelf)
		{
			UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.SetTacticalMap();
		}
		if (GameDataManager.trainingMode)
		{
			if (!this.tutorialObject.activeSelf && !UIFunctions.globaluifunctions.helpmanager.gameObject.activeSelf)
			{
				this.tutorialObject.SetActive(true);
			}
			else if (!UIFunctions.globaluifunctions.helpmanager.gameObject.activeSelf)
			{
				UIFunctions.globaluifunctions.helpmanager.gameObject.SetActive(true);
				this.tutorialPanelState = this.tutorialObject.activeSelf;
				this.tutorialObject.SetActive(false);
				if (ManualCameraZoom.binoculars)
				{
					UIFunctions.globaluifunctions.playerfunctions.playerVessel.submarineFunctions.LeavePeriscopeView();
				}
				if (UIFunctions.globaluifunctions.playerfunctions.otherPanel.activeSelf && !UIFunctions.globaluifunctions.menuSystem[2].activeSelf)
				{
					UIFunctions.globaluifunctions.missionmanager.BringInExitMenu(false);
				}
			}
			else
			{
				UIFunctions.globaluifunctions.helpmanager.gameObject.SetActive(false);
				if (UIFunctions.globaluifunctions.levelloadmanager.currentMuseumInstantiatedObject != null)
				{
					UIFunctions.globaluifunctions.levelloadmanager.currentMuseumInstantiatedObject.SetActive(true);
				}
				this.tutorialObject.SetActive(this.tutorialPanelState);
				this.wasInTacMap = false;
				if (UIFunctions.globaluifunctions.missionmanager.missionExitMenu.gameObject.activeSelf && !UIFunctions.globaluifunctions.menuSystem[2].activeSelf)
				{
					UIFunctions.globaluifunctions.missionmanager.DismissExitMenu();
				}
			}
		}
		else
		{
			if (UIFunctions.globaluifunctions.helpmanager.gameObject.activeSelf)
			{
				UIFunctions.globaluifunctions.helpmanager.gameObject.SetActive(false);
				if (UIFunctions.globaluifunctions.levelloadmanager.currentMuseumInstantiatedObject != null)
				{
					UIFunctions.globaluifunctions.levelloadmanager.currentMuseumInstantiatedObject.SetActive(true);
				}
				if (UIFunctions.globaluifunctions.missionmanager.missionExitMenu.gameObject.activeSelf && !LevelLoadManager.inMuseum && !UIFunctions.globaluifunctions.menuSystem[2].activeSelf)
				{
					UIFunctions.globaluifunctions.missionmanager.DismissExitMenu();
				}
				return;
			}
			UIFunctions.globaluifunctions.helpmanager.gameObject.SetActive(true);
			if (UIFunctions.globaluifunctions.playerfunctions.otherPanel.active && !LevelLoadManager.inMuseum && !UIFunctions.globaluifunctions.menuSystem[2].activeSelf)
			{
				UIFunctions.globaluifunctions.missionmanager.BringInExitMenu(false);
				if (ManualCameraZoom.binoculars)
				{
					UIFunctions.globaluifunctions.playerfunctions.playerVessel.submarineFunctions.LeavePeriscopeView();
				}
			}
		}
		this.GetRelevantHelpContent();
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x0004D704 File Offset: 0x0004B904
	private void GetRelevantHelpContent()
	{
		if (this.inUnitReference)
		{
			this.OpenHelpTabToBookmark(0, "UnitReference");
			return;
		}
		if (UIFunctions.globaluifunctions.menuSystem[0].activeSelf)
		{
			this.OpenHelpTabToBookmark(0, string.Empty);
			return;
		}
		if (UIFunctions.globaluifunctions.menuSystem[1].activeSelf)
		{
			this.OpenHelpTabToBookmark(0, "MissionList");
			return;
		}
		if (UIFunctions.globaluifunctions.menuSystem[4].activeSelf)
		{
			this.OpenHelpTabToBookmark(0, "SelectVessel");
			return;
		}
		if (UIFunctions.globaluifunctions.menuSystem[5].activeSelf)
		{
			if (GameDataManager.missionMode || GameDataManager.trainingMode)
			{
				this.OpenHelpTabToBookmark(0, "MissionBriefing");
				return;
			}
			if (UIFunctions.globaluifunctions.campaignmanager.playerInPort)
			{
				this.OpenHelpTabToBookmark(8, "CampaignPort");
				return;
			}
			this.OpenHelpTabToBookmark(8, "CampaignMissionBriefing");
			return;
		}
		else
		{
			if (UIFunctions.globaluifunctions.menuSystem[2].activeSelf)
			{
				this.OpenHelpTabToBookmark(0, "AfterActionReport");
				return;
			}
			if (this.wasInTacMap)
			{
				this.OpenHelpTabToBookmark(4, string.Empty);
				return;
			}
			if (UIFunctions.globaluifunctions.menuSystem[8].activeSelf)
			{
				this.OpenHelpTabToBookmark(8, "Events");
				return;
			}
			if (UIFunctions.globaluifunctions.menuSystem[10].activeSelf)
			{
				this.OpenHelpTabToBookmark(8, "CampaignMenu");
				return;
			}
			if (UIFunctions.globaluifunctions.menuSystem[11].activeSelf)
			{
				this.OpenHelpTabToBookmark(0, "Options");
				return;
			}
			if (UIFunctions.globaluifunctions.campaignmanager.gameObject.activeSelf)
			{
				this.OpenHelpTabToBookmark(8, "StrategicMap");
				return;
			}
			if (GameDataManager.trainingMode)
			{
				if (this.currentTutorialFilename == "training_combat_1")
				{
					if (this.trainingIndex == 3)
					{
						this.OpenHelpTabToBookmark(5, "Periscope");
						return;
					}
					if (this.trainingIndex == 4)
					{
						this.OpenHelpTabToBookmark(4, "TacticalMap");
						return;
					}
				}
				else if (this.currentTutorialFilename == "training_combat_2")
				{
					if (this.trainingIndex == 1)
					{
						this.OpenHelpTabToBookmark(2, "TorpedoSettingsAndWireGuiding");
						return;
					}
					if (this.trainingIndex == 4)
					{
						this.OpenHelpTabToBookmark(2, "WireBreakage");
						return;
					}
				}
				else if (this.currentTutorialFilename == "training_combat_3")
				{
					if (this.trainingIndex == 1)
					{
						this.OpenHelpTabToBookmark(2, "MissileSettings");
						return;
					}
				}
				else if (this.currentTutorialFilename == "training_combat_5")
				{
					if (this.trainingIndex == 3)
					{
						this.OpenHelpTabToBookmark(4, "TargetMotionAnalysis");
						return;
					}
					if (this.trainingIndex == 4)
					{
						this.OpenHelpTabToBookmark(4, "SignatureAnalysis");
						return;
					}
				}
				else if (this.currentTutorialFilename == "training_combat_6")
				{
					if (this.trainingIndex == 1)
					{
						this.OpenHelpTabToBookmark(5, "Sensors");
						return;
					}
					if (this.trainingIndex == 2)
					{
						this.OpenHelpTabToBookmark(5, "Conditions");
						return;
					}
				}
				else if (this.currentTutorialFilename == "training_combat_7")
				{
					if (this.trainingIndex == 1)
					{
						this.OpenHelpTabToBookmark(6, "MaintainingStealth");
						return;
					}
					if (this.trainingIndex == 5)
					{
						this.OpenHelpTabToBookmark(7, "DamageControl");
						return;
					}
					if (this.trainingIndex == 6)
					{
						this.OpenHelpTabToBookmark(7, "Flooding");
						return;
					}
					if (this.trainingIndex == 8)
					{
						this.OpenHelpTabToBookmark(6, "Tactics");
						return;
					}
				}
			}
			if (UIFunctions.globaluifunctions.playerfunctions.currentOpenPanel == 2)
			{
				this.OpenHelpTabToBookmark(7, "DamageControl");
				return;
			}
			if (UIFunctions.globaluifunctions.playerfunctions.currentOpenPanel == 0)
			{
				this.OpenHelpTabToBookmark(5, "Conditions");
				return;
			}
			if (UIFunctions.globaluifunctions.playerfunctions.currentOpenPanel == 1)
			{
				this.OpenHelpTabToBookmark(4, "SignatureAnalysis");
				return;
			}
			if (UIFunctions.globaluifunctions.playerfunctions.currentOpenPanel == 3)
			{
				this.OpenHelpTabToBookmark(2, "Stores");
				return;
			}
			if (UIFunctions.globaluifunctions.missionmanager.missionExitMenu.activeSelf)
			{
				this.OpenHelpTabToBookmark(0, "EndingCombat");
				return;
			}
			this.OpenHelpTabToBookmark(0, string.Empty);
			return;
		}
	}

	// Token: 0x060007F8 RID: 2040 RVA: 0x0004DB64 File Offset: 0x0004BD64
	private void OnDisable()
	{
		Time.timeScale = this.lastTimeScale;
	}

	// Token: 0x060007F9 RID: 2041 RVA: 0x0004DB74 File Offset: 0x0004BD74
	private void ClearHelpImages()
	{
		if (this.helpImages.Count > 0)
		{
			for (int i = 0; i < this.helpImages.Count; i++)
			{
				GameObject obj = this.helpImages[i];
				UnityEngine.Object.Destroy(obj);
			}
		}
		this.helpImages = new List<GameObject>();
	}

	// Token: 0x060007FA RID: 2042 RVA: 0x0004DBCC File Offset: 0x0004BDCC
	public void OpenHelpTabToBookmark(int helpTabIndex, string bookmark = "")
	{
		this.chapterSelection.transform.position = this.helpTabs[helpTabIndex].transform.position;
		this.helpBookmarks = new Dictionary<string, float>();
		string filePathFromString = UIFunctions.globaluifunctions.textparser.GetFilePathFromString("language/help/HelpBookmarks");
		string[] array = UIFunctions.globaluifunctions.textparser.OpenTextDataFile(filePathFromString);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			if (array2.Length == 2)
			{
				this.helpBookmarks.Add(array2[0], float.Parse(array2[1].Trim()));
			}
		}
		this.BuildHelpContent(helpTabIndex);
		if (bookmark != string.Empty)
		{
			this.scrollRectHelp.verticalNormalizedPosition = this.helpBookmarks[bookmark];
		}
		else
		{
			this.scrollRectHelp.verticalNormalizedPosition = 1f;
		}
	}

	// Token: 0x060007FB RID: 2043 RVA: 0x0004DCBC File Offset: 0x0004BEBC
	public void OpenHelpTab(int helpTabIndex)
	{
		this.ClearHelpImages();
		this.chapterSelection.transform.position = this.helpTabs[helpTabIndex].transform.position;
		this.BuildHelpContent(helpTabIndex);
	}

	// Token: 0x060007FC RID: 2044 RVA: 0x0004DCF8 File Offset: 0x0004BEF8
	private void BuildHelpContent(int helpTabIndex)
	{
		this.helpText.text = string.Empty;
		this.helpText0.text = string.Empty;
		this.helpText1.text = string.Empty;
		this.helpText2.text = string.Empty;
		this.helpText3.text = string.Empty;
		string filePathFromString = UIFunctions.globaluifunctions.textparser.GetFilePathFromString("language/help/" + this.helpTabBackgrounds[helpTabIndex].GetComponentInChildren<Text>().name);
		string[] array = UIFunctions.globaluifunctions.textparser.OpenTextDataFile(filePathFromString);
		string text = string.Empty;
		Vector2 vector = Vector2.zero;
		float y = 0f;
		bool flag = false;
		foreach (string text in array)
		{
			if (text.Trim() == "[/Image]")
			{
				flag = true;
			}
			else if (flag)
			{
				if (text.Contains("<") && text.Contains(">"))
				{
					text = this.PopulateHelpTags(text);
				}
				if (helpTabIndex != 1)
				{
					Text text2 = this.helpText;
					text2.text = text2.text + text + "\n";
				}
				else
				{
					string[] array2 = text.Split(new char[]
					{
						'|'
					});
					if (array2.Length == 1)
					{
						Text text3 = this.helpText;
						text3.text = text3.text + array2[0] + "\n";
						Text text4 = this.helpText0;
						text4.text += "\n";
						Text text5 = this.helpText1;
						text5.text += "\n";
						Text text6 = this.helpText2;
						text6.text += "\n";
						Text text7 = this.helpText3;
						text7.text += "\n";
					}
					else if (array2.Length == 4)
					{
						Text text8 = this.helpText;
						text8.text += "\n";
						array2[0] = array2[0].Replace("maroon", "black");
						array2[2] = array2[2].Replace("maroon", "black");
						if (array2[1].Length > 0)
						{
							array2[0] = array2[0].Replace("<b>", string.Empty);
							array2[0] = array2[0].Replace("</b>", string.Empty);
						}
						if (array2[3].Trim().Length > 0)
						{
							array2[2] = array2[2].Replace("<b>", string.Empty);
							array2[2] = array2[2].Replace("</b>", string.Empty);
						}
						Text text9 = this.helpText0;
						text9.text = text9.text + array2[0] + "\n";
						Text text10 = this.helpText1;
						text10.text = text10.text + array2[1] + "\n";
						Text text11 = this.helpText2;
						text11.text = text11.text + array2[2] + "\n";
						Text text12 = this.helpText3;
						text12.text = text12.text + array2[3] + "\n";
					}
				}
			}
			else
			{
				string[] array3 = text.Split(new char[]
				{
					'='
				});
				if (text.Contains("Background"))
				{
					string filePathFromString2 = UIFunctions.globaluifunctions.textparser.GetFilePathFromString(array3[1].Trim());
					this.helpBackgroundImage.sprite = UIFunctions.globaluifunctions.textparser.GetSprite(UIFunctions.globaluifunctions.textparser.GetFilePathFromString(filePathFromString2));
				}
				else if (text.Contains("Position"))
				{
					vector = UIFunctions.globaluifunctions.textparser.PopulateVector2(array3[1]);
				}
				else if (text.Contains("Height"))
				{
					y = float.Parse(array3[1].Trim());
				}
				else
				{
					string filePathFromString3 = UIFunctions.globaluifunctions.textparser.GetFilePathFromString(array3[1].Trim());
					GameObject gameObject = UnityEngine.Object.Instantiate(this.helpImageObject, this.helpText.transform) as GameObject;
					Image component = gameObject.GetComponent<Image>();
					RectTransform component2 = gameObject.GetComponent<RectTransform>();
					component2.localScale = Vector3.one;
					component2.localPosition = new Vector3(vector.x, vector.y, 0f);
					component.sprite = UIFunctions.globaluifunctions.textparser.GetSprite(filePathFromString3);
					component.SetNativeSize();
					component2.sizeDelta = new Vector2(component2.sizeDelta.x, y);
					this.helpImages.Add(gameObject);
				}
			}
		}
		float preferredHeight = this.helpText.preferredHeight;
		this.helpText.rectTransform.sizeDelta = new Vector2(this.helpText.rectTransform.sizeDelta.x, preferredHeight);
		if (preferredHeight > 660f)
		{
			this.scrollbarHelp.gameObject.SetActive(true);
			this.scrollRectHelp.enabled = true;
			this.scrollbarHelp.value = 1f;
		}
		else
		{
			this.scrollbarHelp.gameObject.SetActive(false);
			this.scrollRectHelp.enabled = false;
		}
	}

	// Token: 0x060007FD RID: 2045 RVA: 0x0004E24C File Offset: 0x0004C44C
	public string PopulateHelpTags(string currentLine)
	{
		if (currentLine.Contains("<MASTTHRESHOLDDEPTH>"))
		{
			currentLine = currentLine.Replace("<MASTTHRESHOLDDEPTH>", UIFunctions.globaluifunctions.playerfunctions.mastThresholdDepth + " " + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Feet").ToLower());
		}
		if (currentLine.Contains("<LEFTCLICK>"))
		{
			currentLine = currentLine.Replace("<LEFTCLICK>", "<color=maroon><b>" + LanguageManager.interfaceDictionary["LeftMouse"] + "</b></color>");
		}
		if (currentLine.Contains("<RIGHTCLICK>"))
		{
			currentLine = currentLine.Replace("<RIGHTCLICK>", "<color=maroon><b>" + LanguageManager.interfaceDictionary["RightMouse"] + "</b></color>");
		}
		if (currentLine.Contains("<MOUSEWHEEL>"))
		{
			currentLine = currentLine.Replace("<MOUSEWHEEL>", "<color=maroon><b>" + LanguageManager.interfaceDictionary["MouseWheel"] + "</b></color>");
		}
		if (currentLine.Contains("<DICTIONARY"))
		{
			foreach (string text in this.tagsForInterface.Keys)
			{
				if (currentLine.Contains(text))
				{
					string text2 = "<color=maroon><b>";
					text2 += this.tagsForInterface[text];
					text2 += "</b></color>";
					currentLine = currentLine.Replace(text, text2);
				}
			}
		}
		if (currentLine.Contains("<KEY"))
		{
			foreach (string text3 in this.tagsForKeys.Keys)
			{
				if (currentLine.Contains(text3))
				{
					string text4 = "<color=maroon><b>";
					foreach (KeyCode keyCode in this.tagsForKeys[text3])
					{
						text4 += keyCode.ToString();
					}
					text4.Trim();
					text4 = text4.Replace("LeftShift", LanguageManager.interfaceDictionary["ShiftKey"]);
					text4 = text4.Replace("RightShift", LanguageManager.interfaceDictionary["ShiftKey"]);
					text4 = text4.Replace("LeftControl", LanguageManager.interfaceDictionary["ControlKey"]);
					text4 = text4.Replace("RightControl", LanguageManager.interfaceDictionary["ControlKey"]);
					text4 = text4.Replace("LeftAlt", LanguageManager.interfaceDictionary["AltKey"]);
					text4 = text4.Replace("RightAlt", LanguageManager.interfaceDictionary["AltKey"]);
					text4 += "</b></color>";
					currentLine = currentLine.Replace(text3, text4);
					currentLine = currentLine.Replace("Alpha", string.Empty);
				}
			}
		}
		return currentLine;
	}

	// Token: 0x060007FE RID: 2046 RVA: 0x0004E5C4 File Offset: 0x0004C7C4
	public void NextTrain()
	{
		this.trainingIndex++;
		if (this.trainingIndex > this.currentTutorialMax)
		{
			this.trainingIndex = this.currentTutorialMax;
		}
		this.GetTrainingContent(this.trainingIndex, this.currentTutorialFilename, this.currentTutorialTag, true);
		if (this.currentTutorialFilename == "training_combat_7")
		{
			if (this.trainingIndex == 5 && !this.tutorialEventDone[5])
			{
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.KnockoutSubsystem("TUBES", true);
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.KnockoutSubsystem("BOWSONAR", false);
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.damageControlCurrentTimers[0] = 75f;
				this.tutorialEventDone[5] = true;
			}
			else if (this.trainingIndex == 6 && !this.tutorialEventDone[6])
			{
				UIFunctions.globaluifunctions.playerfunctions.playerVessel.bouyancyCompartments[7].ApplyFlooding(1);
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentCurrentFlooding[3] = 0.1f;
				float num = UIFunctions.globaluifunctions.playerfunctions.playerVessel.damagesystem.shipCurrentDamagePoints / UIFunctions.globaluifunctions.playerfunctions.playerVessel.damagesystem.shipTotalDamagePoints;
				int num2 = Mathf.RoundToInt((1f - num) * 100f);
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.hullStatusReadout.text = LanguageManager.interfaceDictionary["HullStatus"] + " " + num2.ToString() + LanguageManager.interfaceDictionary["Percentage"];
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentTotalFlooding[3] = 1f;
				this.tutorialEventDone[6] = true;
			}
			else if (this.trainingIndex == 7 && !this.tutorialEventDone[7])
			{
				UIFunctions.globaluifunctions.playerfunctions.playerVessel.bouyancyCompartments[9].ApplyFlooding(1);
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentCurrentFlooding[4] = 0.1f;
				float num3 = UIFunctions.globaluifunctions.playerfunctions.playerVessel.damagesystem.shipCurrentDamagePoints / UIFunctions.globaluifunctions.playerfunctions.playerVessel.damagesystem.shipTotalDamagePoints;
				int num4 = Mathf.RoundToInt((1f - num3) * 100f);
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.hullStatusReadout.text = LanguageManager.interfaceDictionary["HullStatus"] + " " + num4.ToString() + LanguageManager.interfaceDictionary["Percentage"];
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentTotalFlooding[3] = 1f;
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentTotalFlooding[4] = 1f;
				this.tutorialEventDone[7] = true;
			}
		}
	}

	// Token: 0x060007FF RID: 2047 RVA: 0x0004E8C8 File Offset: 0x0004CAC8
	public void PreviousTrain()
	{
		this.trainingIndex--;
		if (this.trainingIndex < 1)
		{
			this.trainingIndex = 1;
		}
		this.GetTrainingContent(this.trainingIndex, this.currentTutorialFilename, this.currentTutorialTag, true);
	}

	// Token: 0x06000800 RID: 2048 RVA: 0x0004E910 File Offset: 0x0004CB10
	public void CloseHelp()
	{
		this.ClearHelpImages();
		if (UIFunctions.globaluifunctions.levelloadmanager.currentMuseumInstantiatedObject != null)
		{
			UIFunctions.globaluifunctions.levelloadmanager.currentMuseumInstantiatedObject.SetActive(true);
		}
		base.gameObject.SetActive(false);
		this.tutorialObject.SetActive(this.tutorialPanelState);
		if (UIFunctions.globaluifunctions.missionmanager.missionExitMenu.gameObject.activeSelf)
		{
			UIFunctions.globaluifunctions.missionmanager.DismissExitMenu();
		}
		this.wasInTacMap = false;
		this.hadModelDisplayed = false;
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x0004E9AC File Offset: 0x0004CBAC
	public void GetTrainingContent(int index, string filename, string tag, bool navOn = false)
	{
		if (this.tutorialBackground.sprite == null)
		{
			this.tutorialBackground.sprite = UIFunctions.globaluifunctions.textparser.GetSprite(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("help_images/tutorial_background.png"));
		}
		this.currentTutorialFilename = filename;
		this.currentTutorialTag = tag;
		string filePathFromString = UIFunctions.globaluifunctions.textparser.GetFilePathFromString("language/training/" + filename);
		string[] array = UIFunctions.globaluifunctions.textparser.OpenTextDataFile(filePathFromString);
		tag = tag + " " + index.ToString();
		bool flag = false;
		bool flag2 = false;
		string text = string.Empty;
		string text2 = string.Empty;
		foreach (string text2 in array)
		{
			if (text2.Contains(tag))
			{
				if (flag)
				{
					break;
				}
				flag = true;
				if (index == 1 && text2.Contains("="))
				{
					string[] array2 = text2.Split(new char[]
					{
						'='
					});
					this.currentTutorialMax = int.Parse(array2[1].Trim());
				}
			}
			else if (flag)
			{
				if (!flag2)
				{
					this.tutorialTitle.text = text2;
					flag2 = true;
				}
				else
				{
					if (text2.Contains("<") && text2.Contains(">"))
					{
						text2 = this.PopulateHelpTags(text2);
					}
					text = text + text2 + "\n";
				}
			}
		}
		this.tutorialText.text = text;
		float preferredHeight = this.tutorialText.preferredHeight;
		this.tutorialText.rectTransform.sizeDelta = new Vector2(this.tutorialText.rectTransform.sizeDelta.x, preferredHeight + 18f);
		this.scrollbarTutorial.value = 1f;
		this.tutorialNav.SetActive(navOn);
	}

	// Token: 0x04000B98 RID: 2968
	public GameObject[] helpTabs;

	// Token: 0x04000B99 RID: 2969
	private Image[] helpTabBackgrounds;

	// Token: 0x04000B9A RID: 2970
	public Image chapterSelection;

	// Token: 0x04000B9B RID: 2971
	public Image helpBackgroundImage;

	// Token: 0x04000B9C RID: 2972
	public Image helpBackground;

	// Token: 0x04000B9D RID: 2973
	public Image tutorialBackground;

	// Token: 0x04000B9E RID: 2974
	public int trainingIndex;

	// Token: 0x04000B9F RID: 2975
	public Text helpText;

	// Token: 0x04000BA0 RID: 2976
	public Text tutorialText;

	// Token: 0x04000BA1 RID: 2977
	public Text tutorialTitle;

	// Token: 0x04000BA2 RID: 2978
	public Text helpText0;

	// Token: 0x04000BA3 RID: 2979
	public Text helpText1;

	// Token: 0x04000BA4 RID: 2980
	public Text helpText2;

	// Token: 0x04000BA5 RID: 2981
	public Text helpText3;

	// Token: 0x04000BA6 RID: 2982
	public Scrollbar scrollbarHelp;

	// Token: 0x04000BA7 RID: 2983
	public Scrollbar scrollbarTutorial;

	// Token: 0x04000BA8 RID: 2984
	public ScrollRect scrollRectHelp;

	// Token: 0x04000BA9 RID: 2985
	public List<GameObject> helpImages;

	// Token: 0x04000BAA RID: 2986
	public GameObject helpImageObject;

	// Token: 0x04000BAB RID: 2987
	public Dictionary<string, List<KeyCode>> tagsForKeys;

	// Token: 0x04000BAC RID: 2988
	public Dictionary<string, string> tagsForInterface;

	// Token: 0x04000BAD RID: 2989
	public GameObject tutorialObject;

	// Token: 0x04000BAE RID: 2990
	public GameObject tutorialNav;

	// Token: 0x04000BAF RID: 2991
	public string currentTutorialFilename;

	// Token: 0x04000BB0 RID: 2992
	public string currentTutorialTag;

	// Token: 0x04000BB1 RID: 2993
	public int currentTutorialMax;

	// Token: 0x04000BB2 RID: 2994
	public bool[] tutorialEventDone;

	// Token: 0x04000BB3 RID: 2995
	public bool tutorialPanelState;

	// Token: 0x04000BB4 RID: 2996
	public float lastTimeScale;

	// Token: 0x04000BB5 RID: 2997
	public Dictionary<string, float> helpBookmarks;

	// Token: 0x04000BB6 RID: 2998
	public bool helpInitialised;

	// Token: 0x04000BB7 RID: 2999
	public bool inUnitReference;

	// Token: 0x04000BB8 RID: 3000
	public bool wasInTacMap;

	// Token: 0x04000BB9 RID: 3001
	public bool hadModelDisplayed;
}
