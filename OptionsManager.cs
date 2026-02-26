using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200013E RID: 318
public class OptionsManager : MonoBehaviour
{
	// Token: 0x060008BD RID: 2237 RVA: 0x00063078 File Offset: 0x00061278
	public void OptionsStart()
	{
		UIFunctions.globaluifunctions.textparser.ReadConfigData();
		this.resolutions = Screen.resolutions;
		this.currentResolution = this.resolutions.Length - 1;
		GameDataManager.optionsBoolSettings = new bool[40];
		GameDataManager.optionsFloatSettings = new float[40];
		InputManager.globalInputManager.buttonKeys = new Dictionary<string, List<KeyCode>>();
		this.BuildDefaultButtonsKeybindList();
		GameDataManager.optionsFloatSettings[9] = 0.5f;
		GameDataManager.optionsBoolSettings[19] = true;
		string path = Path.Combine(Application.persistentDataPath, "options.txt");
		if (File.Exists(path))
		{
			string[] array = new string[0];
			string text = null;
			StreamReader streamReader = new StreamReader(path);
			while (!streamReader.EndOfStream)
			{
				text = text + streamReader.ReadLine() + "\n";
				array = text.Split(new char[]
				{
					'\n'
				});
			}
			bool flag = false;
			string text2 = string.Empty;
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Trim().Split(new char[]
				{
					'='
				});
				if (array2[0] == "[KEYBINDS]")
				{
					flag = true;
				}
				else if (array2[0] == "playercommandername")
				{
					GameDataManager.playerCommanderName = array2[1].Trim();
				}
				else if (array2[0] == "resolution")
				{
					this.currentResolution = this.GetCurrentResolutionIndex(array2[1].Trim());
				}
				if (!flag)
				{
					if (array2.Length > 1)
					{
						this.SetOptionVariable(array2[0], array2[1]);
					}
				}
				else if (array2.Length > 1)
				{
					string text3 = text2;
					text2 = string.Concat(new string[]
					{
						text3,
						array2[0],
						"=",
						array2[1],
						"|"
					});
				}
			}
			streamReader.Close();
			InputManager.globalInputManager.BuildButtonKeysDictionary(text2);
			GameDataManager.currentvolume = GameDataManager.optionsFloatSettings[0];
			GameDataManager.currentmusicvolume = GameDataManager.optionsFloatSettings[1];
			GameDataManager.camerasensitivity = GameDataManager.optionsFloatSettings[2];
		}
		else
		{
			for (int j = 0; j < this.optionsButtons.Length; j++)
			{
				GameDataManager.optionsBoolSettings[j] = true;
			}
			GameDataManager.currentvolume = 0f;
			GameDataManager.currentmusicvolume = 0f;
			GameDataManager.camerasensitivity = 0.5f;
			GameDataManager.optionsFloatSettings[0] = GameDataManager.currentvolume;
			GameDataManager.optionsFloatSettings[1] = GameDataManager.currentmusicvolume;
			GameDataManager.optionsFloatSettings[2] = GameDataManager.camerasensitivity;
			GameDataManager.optionsFloatSettings[3] = 1f;
			GameDataManager.optionsFloatSettings[4] = 3f;
			GameDataManager.optionsFloatSettings[5] = 2f;
			GameDataManager.optionsFloatSettings[6] = 0f;
			GameDataManager.optionsFloatSettings[7] = 1f;
			GameDataManager.optionsFloatSettings[8] = 1.5f;
			GameDataManager.optionsFloatSettings[9] = 1f;
			GameDataManager.playerCommanderName = "Thomas Chesterson";
			this.BringInGameOptions();
			this.AcceptOptions();
		}
		this.DisplayCurrentResolution();
		UIFunctions.globaluifunctions.gamedatamanager.globalSpeedModifier = GameDataManager.optionsFloatSettings[8] / 10f;
		if (GameDataManager.optionsFloatSettings[7] == 0f)
		{
			UIFunctions.globaluifunctions.gamedatamanager.worldYardsScale = 1f;
		}
		else if (GameDataManager.optionsFloatSettings[7] == 1f)
		{
			UIFunctions.globaluifunctions.gamedatamanager.worldYardsScale = 2f;
		}
		else if (GameDataManager.optionsFloatSettings[7] == 2f)
		{
			UIFunctions.globaluifunctions.gamedatamanager.worldYardsScale = 3f;
		}
		else if (GameDataManager.optionsFloatSettings[7] == 3f)
		{
			UIFunctions.globaluifunctions.gamedatamanager.worldYardsScale = 4f;
		}
		UIFunctions.globaluifunctions.gamedatamanager.Start();
		if (!GameDataManager.optionsBoolSettings[1])
		{
			QualitySettings.SetQualityLevel(0, true);
		}
		else
		{
			QualitySettings.SetQualityLevel(1, true);
		}
		if (!GameDataManager.optionsBoolSettings[2])
		{
			QualitySettings.vSyncCount = 0;
		}
		else
		{
			QualitySettings.vSyncCount = 1;
		}
		switch ((int)GameDataManager.optionsFloatSettings[4])
		{
		case 0:
			QualitySettings.antiAliasing = 0;
			break;
		case 1:
			QualitySettings.antiAliasing = 2;
			break;
		case 2:
			QualitySettings.antiAliasing = 4;
			break;
		case 3:
			QualitySettings.antiAliasing = 8;
			break;
		}
		UIFunctions.globaluifunctions.textparser.ReadDifficultyData();
		if (UIFunctions.globaluifunctions.levelloadmanager.submarineMarker.playerTransform != null)
		{
			UIFunctions.globaluifunctions.levelloadmanager.submarineMarker.gameObject.SetActive(GameDataManager.optionsBoolSettings[13]);
			UIFunctions.globaluifunctions.particlefield.gameObject.SetActive(GameDataManager.optionsBoolSettings[14]);
		}
		if (GameDataManager.optionsBoolSettings[6])
		{
			UIFunctions.globaluifunctions.levelloadmanager.amplifycoloreffect.enabled = true;
		}
		else
		{
			UIFunctions.globaluifunctions.levelloadmanager.amplifycoloreffect.enabled = false;
		}
		if (GameDataManager.optionsBoolSettings[7])
		{
			UIFunctions.globaluifunctions.levelloadmanager.amplifyocclusioneffect.enabled = true;
		}
		else
		{
			UIFunctions.globaluifunctions.levelloadmanager.amplifyocclusioneffect.enabled = false;
		}
		if (GameDataManager.optionsBoolSettings[8])
		{
			UIFunctions.globaluifunctions.levelloadmanager.amplifybloom.enabled = true;
		}
		else
		{
			UIFunctions.globaluifunctions.levelloadmanager.amplifybloom.enabled = false;
		}
		float num = 1.7777778f;
		float aspect = UIFunctions.globaluifunctions.GUICameraObject.aspect;
		float num2 = num / aspect;
		this.CalculateAndSetMiniMapDimensions();
		this.SetDefaultCameraOrth(aspect, num);
		UIFunctions.globaluifunctions.playerfunctions.hudScaler.scaleFactor = this.GetHUDSize(GameDataManager.optionsFloatSettings[6]);
		float scaleFactor = UIFunctions.globaluifunctions.playerfunctions.hudScaler.scaleFactor;
		UIFunctions.globaluifunctions.bearingMarker.gameObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
		if (GameDataManager.optionsBoolSettings[10])
		{
			ManualCameraZoom.invertMouse = 1f;
		}
		else
		{
			ManualCameraZoom.invertMouse = -1f;
		}
		UIFunctions.globaluifunctions.playerfunctions.voicemanager.voiceSource.volume = GameDataManager.optionsFloatSettings[9];
		AudioManager.audiomanager.SetEffectsVolume(GameDataManager.currentvolume);
		AudioManager.audiomanager.SetMusicVolume(GameDataManager.currentmusicvolume);
	}

	// Token: 0x060008BE RID: 2238 RVA: 0x000636D8 File Offset: 0x000618D8
	private void SetDefaultCameraOrth(float currentRatio, float defaultRatio)
	{
		if (this.defaultMenuCameraOrth && currentRatio < defaultRatio)
		{
			float num;
			if (currentRatio < 1.3f)
			{
				num = 43f;
			}
			else if (currentRatio < 1.49f)
			{
				num = 34f;
			}
			else if (currentRatio < 1.59f)
			{
				num = 19f;
			}
			else
			{
				num = 11f;
			}
			UIFunctions.globaluifunctions.GUICameraObject.orthographicSize = 100f + num;
		}
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x0006375C File Offset: 0x0006195C
	public void CalculateAndSetMiniMapDimensions()
	{
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.tacMapCameraRects = new Rect[2];
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.tacMapCameraRects[1] = new Rect(0f, 0f, 1f, 1f);
		Rect rect = default(Rect);
		this.minimapLowerLeft = new Vector2(12.6f, 12.8f);
		this.miniMapUpperRight = new Vector2(318f, 244.6f);
		rect.x = (this.minimapLowerLeft.x + this.miniMapOffset.x) * this.GetHUDSize(GameDataManager.optionsFloatSettings[6]) / (float)Screen.width;
		rect.y = (this.minimapLowerLeft.y + this.miniMapOffset.y) * this.GetHUDSize(GameDataManager.optionsFloatSettings[6]) / (float)Screen.height;
		rect.width = (this.miniMapUpperRight.x - this.minimapLowerLeft.x) / (float)Screen.width;
		rect.height = (this.miniMapUpperRight.y - this.minimapLowerLeft.y) / (float)Screen.height;
		this.miniMapUpperRight = new Vector2((318f + this.miniMapOffset.x) * this.GetHUDSize(GameDataManager.optionsFloatSettings[6]), (244.6f + this.miniMapOffset.y) * this.GetHUDSize(GameDataManager.optionsFloatSettings[6]));
		rect.width *= this.GetHUDSize(GameDataManager.optionsFloatSettings[6]);
		rect.height *= this.GetHUDSize(GameDataManager.optionsFloatSettings[6]);
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.tacMapCameraRects[0] = rect;
		UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.tacMapCamera.rect = rect;
	}

	// Token: 0x060008C0 RID: 2240 RVA: 0x0006396C File Offset: 0x00061B6C
	public void BuildDefaultButtonsKeybindList()
	{
		string[] array = UIFunctions.globaluifunctions.textparser.OpenTextDataFile("default_keys");
		string text = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			text = text + array[i] + "|";
		}
		InputManager.globalInputManager.BuildButtonKeysDictionary(text);
	}

	// Token: 0x060008C1 RID: 2241 RVA: 0x000639C4 File Offset: 0x00061BC4
	private int GetCurrentResolutionIndex(string input)
	{
		string[] array = input.Split(new char[]
		{
			'x'
		});
		int[] array2 = new int[]
		{
			int.Parse(array[0]),
			int.Parse(array[1])
		};
		for (int i = 0; i < this.resolutions.Length; i++)
		{
			if (array2[0] == this.resolutions[i].width && array2[1] == this.resolutions[i].height)
			{
				return i;
			}
		}
		return this.resolutions.Length - 1;
	}

	// Token: 0x060008C2 RID: 2242 RVA: 0x00063A58 File Offset: 0x00061C58
	public void RestoreControlsToDefault()
	{
		this.BuildDefaultButtonsKeybindList();
		this.keybinddialogbox.RebuildButtonList();
	}

	// Token: 0x060008C3 RID: 2243 RVA: 0x00063A6C File Offset: 0x00061C6C
	public void QuitApp()
	{
		Application.Quit();
	}

	// Token: 0x060008C4 RID: 2244 RVA: 0x00063A74 File Offset: 0x00061C74
	public void OpenOptionsTab(int optionsTabIndex)
	{
		this.CloseAllOptionsTabs();
		this.optionsTabs[optionsTabIndex].SetActive(true);
		this.tabBackgrounds[optionsTabIndex].enabled = false;
	}

	// Token: 0x060008C5 RID: 2245 RVA: 0x00063AA4 File Offset: 0x00061CA4
	private void CloseAllOptionsTabs()
	{
		for (int i = 0; i < this.optionsTabs.Length; i++)
		{
			this.optionsTabs[i].SetActive(false);
			this.tabBackgrounds[i].enabled = true;
			this.tabBackgrounds[i].gameObject.SetActive(false);
			this.tabBackgrounds[i].gameObject.SetActive(true);
		}
	}

	// Token: 0x060008C6 RID: 2246 RVA: 0x00063B0C File Offset: 0x00061D0C
	public void BringInGameOptions()
	{
		UIFunctions.globaluifunctions.SetMenuSystem("OPTIONS");
		UIFunctions.globaluifunctions.scrollbarDefault.gameObject.SetActive(false);
		UIFunctions.globaluifunctions.backgroundImage.gameObject.SetActive(false);
		UIFunctions.globaluifunctions.backgroundImagesOnly.gameObject.SetActive(false);
		UIFunctions.globaluifunctions.mainTitle.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "OptionsHeader");
		UIFunctions.globaluifunctions.mainColumn.gameObject.SetActive(false);
		UIFunctions.globaluifunctions.secondColumm.gameObject.SetActive(false);
		UIFunctions.globaluifunctions.secondaryTitle.gameObject.SetActive(false);
		for (int i = 0; i < this.optionsButtons.Length; i++)
		{
			if (!GameDataManager.optionsBoolSettings[i])
			{
				this.optionsButtons[i].isOn = false;
			}
			else
			{
				this.optionsButtons[i].isOn = true;
			}
		}
		for (int j = 0; j < this.optionsSliders.Length; j++)
		{
			this.optionsSliders[j].value = GameDataManager.optionsFloatSettings[j];
		}
		this.defaultCommanderName.text = GameDataManager.playerCommanderName;
		this.DisplayCurrentResolution();
		this.AdjustDifficulty();
		this.AdjustAliasing();
		this.AdjustOceanQuality();
		this.AdjustWorldScale();
		this.AdjustSpeedMultiplier();
		this.CloseAllOptionsTabs();
		this.optionsTabs[0].SetActive(true);
		this.tabBackgrounds[0].enabled = false;
	}

	// Token: 0x060008C7 RID: 2247 RVA: 0x00063C90 File Offset: 0x00061E90
	public void AdjustSpeedMultiplier()
	{
		this.speedMultiplierReadout.text = string.Format("{0:0.0}", this.optionsSliders[8].value / 10f);
	}

	// Token: 0x060008C8 RID: 2248 RVA: 0x00063CC0 File Offset: 0x00061EC0
	public void AdjustWorldScale()
	{
		switch ((int)this.optionsSliders[7].value)
		{
		case 0:
			this.worldScaleReadout.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Scale1");
			break;
		case 1:
			this.worldScaleReadout.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Scale2");
			break;
		case 2:
			this.worldScaleReadout.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Scale3");
			break;
		case 3:
			this.worldScaleReadout.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Scale4");
			break;
		}
	}

	// Token: 0x060008C9 RID: 2249 RVA: 0x00063D74 File Offset: 0x00061F74
	public void AdjustDifficulty()
	{
		switch ((int)this.optionsSliders[3].value)
		{
		case 0:
			this.difficultyReadout.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Easy");
			break;
		case 1:
			this.difficultyReadout.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Normal");
			break;
		case 2:
			this.difficultyReadout.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Hard");
			break;
		case 3:
			this.difficultyReadout.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Elite");
			break;
		}
	}

	// Token: 0x060008CA RID: 2250 RVA: 0x00063E28 File Offset: 0x00062028
	public void AdjustAliasing()
	{
		switch ((int)this.optionsSliders[4].value)
		{
		case 0:
			this.aliasingReadout.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Off");
			break;
		case 1:
			this.aliasingReadout.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "2x");
			break;
		case 2:
			this.aliasingReadout.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "4x");
			break;
		case 3:
			this.aliasingReadout.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "8x");
			break;
		}
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x00063EDC File Offset: 0x000620DC
	public void AdjustHUDSize()
	{
		this.HUDSizeReadout.text = "x " + this.GetHUDSize(this.optionsSliders[6].value).ToString();
	}

	// Token: 0x060008CC RID: 2252 RVA: 0x00063F1C File Offset: 0x0006211C
	private float GetHUDSize(float value)
	{
		value /= 20f;
		value += 1f;
		return value;
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x00063F34 File Offset: 0x00062134
	public void AdjustOceanQuality()
	{
		switch ((int)this.optionsSliders[5].value)
		{
		case 0:
			this.oceanQualityReadout.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Low");
			break;
		case 1:
			this.oceanQualityReadout.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Medium");
			break;
		case 2:
			this.oceanQualityReadout.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "High");
			break;
		}
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x00063FC4 File Offset: 0x000621C4
	public void AdjustFullScreen()
	{
		Screen.fullScreen = this.optionsButtons[9].isOn;
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x00063FDC File Offset: 0x000621DC
	public void AdjustResolution()
	{
		this.currentResolution++;
		if (this.currentResolution == this.resolutions.Length)
		{
			this.currentResolution = 0;
		}
		this.DisplayCurrentResolution();
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x00064018 File Offset: 0x00062218
	private void DisplayCurrentResolution()
	{
		this.resolutionText.text = this.resolutions[this.currentResolution].width.ToString() + " x " + this.resolutions[this.currentResolution].height.ToString();
	}

	// Token: 0x060008D1 RID: 2257 RVA: 0x00064078 File Offset: 0x00062278
	public void ManuallyApplyResolution()
	{
		Screen.SetResolution(this.resolutions[this.currentResolution].width, this.resolutions[this.currentResolution].height, this.optionsButtons[9].isOn);
	}

	// Token: 0x060008D2 RID: 2258 RVA: 0x000640C4 File Offset: 0x000622C4
	public void AcceptOptions()
	{
		for (int i = 0; i < this.optionsButtons.Length; i++)
		{
			if (!this.optionsButtons[i].isOn)
			{
				GameDataManager.optionsBoolSettings[i] = false;
			}
			else
			{
				GameDataManager.optionsBoolSettings[i] = true;
			}
		}
		if ((Screen.width != this.resolutions[this.currentResolution].width || Screen.height != this.resolutions[this.currentResolution].height) && Screen.width > 1000)
		{
			Screen.SetResolution(this.resolutions[this.currentResolution].width, this.resolutions[this.currentResolution].height, GameDataManager.optionsBoolSettings[9]);
		}
		this.SaveOptions();
		if (this.cameraAutoPan.activeSelf)
		{
			this.cameraParams.x = float.Parse(this.cameraX.text);
			this.cameraParams.y = float.Parse(this.cameraY.text);
			this.cameraParams.z = float.Parse(this.cameraZ.text) / 10f;
			this.cameraMoveParams.x = float.Parse(this.cameraMoveX.text) / 20f;
			this.cameraMoveParams.y = float.Parse(this.cameraMoveY.text) / 20f;
			this.cameraMoveParams.z = float.Parse(this.cameraMoveZ.text) / 20f;
		}
	}

	// Token: 0x060008D3 RID: 2259 RVA: 0x0006426C File Offset: 0x0006246C
	private void SetOptionVariable(string option, string value)
	{
		if (option == "playercommandername")
		{
			GameDataManager.playerCommanderName = value;
			return;
		}
		if (option == "resolution")
		{
			return;
		}
		if (value == "TRUE" || value == "FALSE")
		{
			for (int i = 0; i < this.boolOptionsNames.Length; i++)
			{
				if (this.boolOptionsNames[i] == option)
				{
					if (value == "TRUE")
					{
						GameDataManager.optionsBoolSettings[i] = true;
					}
					else
					{
						GameDataManager.optionsBoolSettings[i] = false;
					}
					return;
				}
			}
		}
		else
		{
			for (int j = 0; j < this.boolOptionsNames.Length; j++)
			{
				if (this.floatOptionsNames[j] == option)
				{
					GameDataManager.optionsFloatSettings[j] = float.Parse(value);
					return;
				}
			}
		}
	}

	// Token: 0x060008D4 RID: 2260 RVA: 0x00064354 File Offset: 0x00062554
	public void SaveOptions()
	{
		for (int i = 0; i < this.optionsSliders.Length; i++)
		{
			GameDataManager.optionsFloatSettings[i] = this.optionsSliders[i].value;
		}
		string path = Path.Combine(Application.persistentDataPath, "options.txt");
		string text = null;
		string text2 = text;
		text = string.Concat(new string[]
		{
			text2,
			"resolution=",
			this.resolutions[this.currentResolution].width.ToString(),
			"x",
			this.resolutions[this.currentResolution].height.ToString(),
			"\n"
		});
		text = text + "playercommandername=" + SaveLoadManager.StripSpecialCharsFromString(this.defaultCommanderName.text) + "\n";
		for (int j = 0; j < this.boolOptionsNames.Length; j++)
		{
			text2 = text;
			text = string.Concat(new string[]
			{
				text2,
				this.boolOptionsNames[j],
				"=",
				GameDataManager.optionsBoolSettings[j].ToString().ToUpper(),
				"\n"
			});
		}
		for (int k = 0; k < this.floatOptionsNames.Length; k++)
		{
			text2 = text;
			text = string.Concat(new string[]
			{
				text2,
				this.floatOptionsNames[k],
				"=",
				GameDataManager.optionsFloatSettings[k].ToString(),
				"\n"
			});
		}
		text += "[KEYBINDS]\n";
		text += InputManager.globalInputManager.DumpButtonKeysDictionary();
		text += "[/KEYBINDS]\n";
		File.WriteAllText(path, text);
		this.OptionsStart();
		this.ExitFromOptionsPage();
	}

	// Token: 0x060008D5 RID: 2261 RVA: 0x00064530 File Offset: 0x00062730
	public void ExitFromOptionsPage()
	{
		if (GameDataManager.playervesselsonlevel.Length == 0)
		{
			UIFunctions.globaluifunctions.levelloadmanager.CloseMuseum();
		}
		else
		{
			UIFunctions.globaluifunctions.backgroundImage.gameObject.SetActive(true);
			UIFunctions.globaluifunctions.mainColumn.gameObject.SetActive(true);
			UIFunctions.globaluifunctions.missionmanager.DismissExitMenu();
			UIFunctions.globaluifunctions.menuSystemParent.SetActive(false);
		}
	}

	// Token: 0x04000D77 RID: 3447
	public KeybindDialogBox keybinddialogbox;

	// Token: 0x04000D78 RID: 3448
	public GameObject optionSelectBar;

	// Token: 0x04000D79 RID: 3449
	public Toggle[] optionsButtons;

	// Token: 0x04000D7A RID: 3450
	public Slider[] optionsSliders;

	// Token: 0x04000D7B RID: 3451
	public bool screenSet;

	// Token: 0x04000D7C RID: 3452
	public float screenSetValue;

	// Token: 0x04000D7D RID: 3453
	public Camera GUICameraObject;

	// Token: 0x04000D7E RID: 3454
	public Camera HUDCameraObject;

	// Token: 0x04000D7F RID: 3455
	public bool optionsInCombat;

	// Token: 0x04000D80 RID: 3456
	public float HUDCameraHeight;

	// Token: 0x04000D81 RID: 3457
	public ToggleGroup difficultyToggle;

	// Token: 0x04000D82 RID: 3458
	public Text[] optionsTextFields;

	// Token: 0x04000D83 RID: 3459
	public Text difficultyReadout;

	// Token: 0x04000D84 RID: 3460
	public Text aliasingReadout;

	// Token: 0x04000D85 RID: 3461
	public Text oceanQualityReadout;

	// Token: 0x04000D86 RID: 3462
	public Text HUDSizeReadout;

	// Token: 0x04000D87 RID: 3463
	public Text worldScaleReadout;

	// Token: 0x04000D88 RID: 3464
	public Text speedMultiplierReadout;

	// Token: 0x04000D89 RID: 3465
	public InputField defaultCommanderName;

	// Token: 0x04000D8A RID: 3466
	public string[] boolOptionsNames;

	// Token: 0x04000D8B RID: 3467
	public string[] floatOptionsNames;

	// Token: 0x04000D8C RID: 3468
	public Resolution[] resolutions;

	// Token: 0x04000D8D RID: 3469
	public int currentResolution;

	// Token: 0x04000D8E RID: 3470
	public Text resolutionText;

	// Token: 0x04000D8F RID: 3471
	public GameObject[] optionsTabs;

	// Token: 0x04000D90 RID: 3472
	public Image[] tabBackgrounds;

	// Token: 0x04000D91 RID: 3473
	public static Dictionary<string, float> difficultySettings;

	// Token: 0x04000D92 RID: 3474
	public Transform[] miniMapBounds;

	// Token: 0x04000D93 RID: 3475
	public Vector2 minimapLowerLeft;

	// Token: 0x04000D94 RID: 3476
	public Vector2 miniMapUpperRight;

	// Token: 0x04000D95 RID: 3477
	public bool defaultMenuCameraOrth;

	// Token: 0x04000D96 RID: 3478
	public GameObject cameraAutoPan;

	// Token: 0x04000D97 RID: 3479
	public InputField cameraX;

	// Token: 0x04000D98 RID: 3480
	public InputField cameraY;

	// Token: 0x04000D99 RID: 3481
	public InputField cameraZ;

	// Token: 0x04000D9A RID: 3482
	public Vector3 cameraParams;

	// Token: 0x04000D9B RID: 3483
	public InputField cameraMoveX;

	// Token: 0x04000D9C RID: 3484
	public InputField cameraMoveY;

	// Token: 0x04000D9D RID: 3485
	public InputField cameraMoveZ;

	// Token: 0x04000D9E RID: 3486
	public Vector3 cameraMoveParams;

	// Token: 0x04000D9F RID: 3487
	public Vector2 miniMapOffset;
}
