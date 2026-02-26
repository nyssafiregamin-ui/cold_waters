using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000110 RID: 272
public class EditorMission : MonoBehaviour
{
	// Token: 0x06000745 RID: 1861 RVA: 0x000405DC File Offset: 0x0003E7DC
	private void Awake()
	{
		EditorMission.instance = this;
		base.gameObject.SetActive(false);
		this.masterVesselRect = this.masterVesselParent.GetComponent<RectTransform>();
		this.currentVesselRect = this.currentVesselParent.GetComponent<RectTransform>();
	}

	// Token: 0x06000746 RID: 1862 RVA: 0x00040620 File Offset: 0x0003E820
	public void SetMissionEditor(bool value)
	{
		base.gameObject.SetActive(value);
		this.singleMissionScrollPanel.SetActive(!value);
		this.selectionScreen.SetActive(!value);
		UIFunctions.globaluifunctions.backgroundImage.gameObject.SetActive(!value);
		if (!value)
		{
			UIFunctions.globaluifunctions.mainTitle.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "SingleMissionHeader");
		}
		else
		{
			UIFunctions.globaluifunctions.mainTitle.text = "QUICK MISSION";
			if (!this.isInitialised)
			{
				this.InitialiseMissionEditor();
			}
		}
	}

	// Token: 0x06000747 RID: 1863 RVA: 0x000406C0 File Offset: 0x0003E8C0
	public void ShowEnvironmentFields()
	{
		this.weatherField.interactable = this.usePresetEnvironment.isOn;
		this.seaStateField.interactable = this.usePresetEnvironment.isOn;
		this.ductStrengthField.interactable = this.usePresetEnvironment.isOn;
		this.layerStrengthField.interactable = this.usePresetEnvironment.isOn;
	}

	// Token: 0x06000748 RID: 1864 RVA: 0x00040728 File Offset: 0x0003E928
	private void InitialiseMissionEditor()
	{
		List<string> list = new List<string>();
		list.Add("RANDOM");
		for (int i = 0; i < UIFunctions.globaluifunctions.playerfunctions.sensormanager.weather.Length; i++)
		{
			list.Add(LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, UIFunctions.globaluifunctions.playerfunctions.sensormanager.weather[i]));
		}
		this.weatherField.ClearOptions();
		this.weatherField.AddOptions(list);
		list.Clear();
		list.Add("RANDOM");
		for (int j = 0; j < UIFunctions.globaluifunctions.playerfunctions.sensormanager.seaStates.Length; j++)
		{
			list.Add(LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, UIFunctions.globaluifunctions.playerfunctions.sensormanager.seaStates[j]));
		}
		this.seaStateField.ClearOptions();
		this.seaStateField.AddOptions(list);
		list.Clear();
		list.Add("RANDOM");
		for (int k = 0; k < UIFunctions.globaluifunctions.playerfunctions.sensormanager.strengthTypes.Length; k++)
		{
			list.Add(LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, UIFunctions.globaluifunctions.playerfunctions.sensormanager.strengthTypes[k]));
		}
		this.ductStrengthField.ClearOptions();
		this.layerStrengthField.ClearOptions();
		this.ductStrengthField.AddOptions(list);
		this.layerStrengthField.AddOptions(list);
		list.Clear();
		list.Add("RANDOM");
		for (int l = 0; l < 24; l++)
		{
			list.Add(l.ToString("00") + ":00");
		}
		this.timeField.ClearOptions();
		this.timeField.AddOptions(list);
		list.Clear();
		for (int m = 1; m < 32; m++)
		{
			list.Add(m.ToString());
		}
		this.dateDay.AddOptions(list);
		list.Clear();
		list.Add("JAN");
		list.Add("FEB");
		list.Add("MAR");
		list.Add("APR");
		list.Add("MAY");
		list.Add("JUN");
		list.Add("JUL");
		list.Add("AUG");
		list.Add("SEP");
		list.Add("OCT");
		list.Add("NOV");
		list.Add("DEC");
		this.dateMonth.AddOptions(list);
		UIFunctions.globaluifunctions.textparser.BuildEditorDictionary(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("language/dictionary/dictionary_editor"));
		this.BuildInGameText(this.interfaceText);
		this.InitialiseMapData();
		this.OpenMissionEditorTab(0);
		this.highlight.gameObject.SetActive(false);
		this.isInitialised = true;
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x00040A28 File Offset: 0x0003EC28
	private void InitialiseMapData()
	{
		this.mapImages = new List<string>();
		this.mapNavData = new List<string>();
		this.mapElevationData = new List<string>();
		this.mapWorldObjects = new List<string>();
		this.mapVesselsAndTraffic = new List<string>();
		this.mapDefaultYear = new List<int>();
		this.mapDefaultCoords = new List<Vector2>();
		this.defaultPlayerCommandFleet = new List<string>();
		this.mapPlayerVessels = new List<string>();
		string[] array = UIFunctions.globaluifunctions.textparser.OpenTextDataFile("editor");
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			string text = array2[0];
			switch (text)
			{
			case "MapImage":
				this.mapImages.Add(array2[1].Trim());
				break;
			case "MapNavigationData":
				this.mapNavData.Add(array2[1].Trim());
				break;
			case "MapElevationData":
				this.mapElevationData.Add(array2[1].Trim());
				break;
			case "WorldObjectsData":
				this.mapWorldObjects.Add(array2[1].Trim());
				break;
			case "VesselsAndTraffic":
				this.mapVesselsAndTraffic.Add(array2[1].Trim());
				break;
			case "MapHemisphere":
				this.mapHemisphere.Add(array2[1].Trim());
				break;
			case "MapDefaultYear":
				this.mapDefaultYear.Add(int.Parse(array2[1].Trim()));
				break;
			case "MapDefaultCoords":
			{
				string[] array3 = array2[1].Trim().Split(new char[]
				{
					','
				});
				this.mapDefaultCoords.Add(new Vector2(float.Parse(array3[0]), float.Parse(array3[1])));
				break;
			}
			case "PlayerCommand":
				this.defaultPlayerCommandFleet.Add(array2[1].Trim());
				break;
			case "PlayerVessels":
				this.mapPlayerVessels.Add(array2[1].Trim());
				break;
			case "HelicopterTypes":
				this.mapHelicopters.Add(array2[1].Trim());
				break;
			case "AircraftTypes":
				this.mapAircraft.Add(array2[1].Trim());
				break;
			}
		}
		this.UpdateMapPreview();
	}

	// Token: 0x0600074A RID: 1866 RVA: 0x00040D3C File Offset: 0x0003EF3C
	public void UpdateMapPreview()
	{
		this.mapPreview.sprite = UIFunctions.globaluifunctions.textparser.GetSprite(this.mapImages[this.mapSelectionField.value]);
		this.dateYear.text = this.mapDefaultYear[this.mapSelectionField.value].ToString();
		this.mapCoordsX.text = this.mapDefaultCoords[this.mapSelectionField.value].x.ToString();
		this.mapCoordsY.text = this.mapDefaultCoords[this.mapSelectionField.value].y.ToString();
		this.SetPlayerCommand();
		this.PlaceMapHighlightAtCoords();
		this.GetOtherVessels();
		this.SetupAircraft();
		this.shipPreviewNameText.text = string.Empty;
		this.shipProfilePreview.enabled = false;
		this.CancelMasterList();
		this.InitialiseVesselGroups();
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x00040E40 File Offset: 0x0003F040
	public void VerifyDaysInMonth()
	{
		int num = CalendarFunctions.MaxDaysInMonth(this.dateMonth.value + 1, int.Parse(this.dateYear.text));
		if (this.dateDay.value + 1 > num)
		{
			this.dateDay.value = num - 1;
		}
	}

	// Token: 0x0600074C RID: 1868 RVA: 0x00040E94 File Offset: 0x0003F094
	private void SetPlayerCommand()
	{
		string[] array = this.defaultPlayerCommandFleet[this.mapSelectionField.value].Split(new char[]
		{
			'|'
		});
		this.playerSubCommand.text = array[0];
		this.playerSubFleet.text = array[1];
		this.playerSubClasses.text = string.Empty;
		string[] array2 = this.mapPlayerVessels[this.mapSelectionField.value].Split(new char[]
		{
			','
		});
		for (int i = 0; i < array2.Length; i++)
		{
			Text text = this.playerSubClasses;
			text.text = text.text + UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.textparser.GetShipID(array2[i])].shipclass + "   ";
		}
	}

	// Token: 0x0600074D RID: 1869 RVA: 0x00040F74 File Offset: 0x0003F174
	public void PlaceMapHighlightAtCoords()
	{
		float num = float.Parse(this.mapCoordsX.text);
		float num2 = float.Parse(this.mapCoordsY.text);
		num = Mathf.Clamp(num, 128f, 3968f);
		num2 = Mathf.Clamp(num2, 128f, 1920f);
		this.mapCoordsX.text = num.ToString();
		this.mapCoordsY.text = num2.ToString();
		num /= 4096f;
		num2 /= 2048f;
		num *= 800f;
		num2 *= 400f;
		num -= 400f;
		num2 -= 200f;
		Draggable.instance.thisImage.transform.localPosition = new Vector3(num, num2, Draggable.instance.thisImage.transform.localPosition.z);
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x00041050 File Offset: 0x0003F250
	private void InitialiseVesselGroups()
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		for (int i = 0; i < 11; i++)
		{
			if (i != 10)
			{
				list.Add(i.ToString());
			}
			if (i != 0)
			{
				list2.Add(i.ToString());
			}
		}
		for (int j = 0; j < this.vesselGroups.Length; j++)
		{
			this.vesselGroups[j].useGroup.isOn = false;
			this.vesselGroups[j].groupVesselList = new List<int>();
			this.vesselGroups[j].numInGroup.text = "0";
			this.vesselGroups[j].groupName.text = LanguageManager.editorDictionary["GroupName"] + " " + (j + 1).ToString();
			this.vesselGroups[j].toLabel.text = LanguageManager.editorDictionary["To"];
			this.vesselGroups[j].minVessels.ClearOptions();
			this.vesselGroups[j].maxVessels.ClearOptions();
			this.vesselGroups[j].minVessels.AddOptions(list);
			this.vesselGroups[j].maxVessels.AddOptions(list2);
		}
		for (int k = 11; k < 31; k++)
		{
			list2.Add(k.ToString());
		}
		this.cruiseSpeed.ClearOptions();
		this.cruiseSpeed.AddOptions(list2);
		this.cruiseSpeed.value = 11;
		this.currentGroup = 0;
		this.DisplayCurrentList();
	}

	// Token: 0x0600074F RID: 1871 RVA: 0x000411F4 File Offset: 0x0003F3F4
	public void SelectVesselGroupToggle(int groupNumber)
	{
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x000411F8 File Offset: 0x0003F3F8
	private void PopulateCurrentGroupVesselListFromMaster()
	{
		this.vesselGroups[this.currentGroup].groupVesselList = new List<int>();
		for (int i = 0; i < this.vesselBarsMaster.Length; i++)
		{
			if (this.vesselBarsMaster[i].checkbox.isOn)
			{
				this.vesselGroups[this.currentGroup].groupVesselList.Add(this.otherVessels[i]);
			}
		}
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x0004126C File Offset: 0x0003F46C
	private void SetGroupDisplayStatus(int index, bool isOn)
	{
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x00041270 File Offset: 0x0003F470
	public void SelectVesselGroup(int index)
	{
		this.SetHighlightPosition(this.vesselGroups[index].useGroup.transform.position.y);
		this.currentGroup = index;
		this.DisplayCurrentList();
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x000412B0 File Offset: 0x0003F4B0
	private void SetHighlightPosition(float y)
	{
		this.highlight.transform.position = new Vector3(this.highlight.transform.position.x, y, this.highlight.transform.position.z);
		if (!this.highlight.gameObject.activeSelf)
		{
			this.highlight.gameObject.SetActive(true);
		}
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x0004132C File Offset: 0x0003F52C
	public void EditCurrentList(int index)
	{
		this.currentGroup = index;
		this.SelectVesselGroup(index);
		this.masterVesselParent.parent.gameObject.SetActive(true);
		this.currentScrollBar.SetActive(false);
		this.masterScrollBar.SetActive(true);
		this.shipPreviewNameText.text = LanguageManager.GetDictionaryString(LanguageManager.editorDictionary, "ShipPreviewName");
		this.shipProfilePreview.enabled = false;
		this.currentVesselParent.parent.gameObject.SetActive(false);
		this.CheckMasterCheckBoxes();
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x000413B8 File Offset: 0x0003F5B8
	private void CheckMasterCheckBoxes()
	{
		for (int i = 0; i < this.vesselBarsMaster.Length; i++)
		{
			this.vesselBarsMaster[i].checkbox.isOn = this.vesselGroups[this.currentGroup].groupVesselList.Contains(this.otherVessels[i]);
		}
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x00041410 File Offset: 0x0003F610
	public void CancelMasterList()
	{
		this.masterVesselParent.parent.gameObject.SetActive(false);
		this.currentVesselParent.parent.gameObject.SetActive(true);
		this.masterScrollBar.SetActive(false);
		this.CheckCurrentListScrollBar();
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x0004145C File Offset: 0x0003F65C
	public void AcceptMasterList()
	{
		this.masterVesselParent.parent.gameObject.SetActive(false);
		this.masterScrollBar.SetActive(false);
		this.PopulateCurrentGroupVesselListFromMaster();
		this.DisplayCurrentList();
		this.currentVesselParent.parent.gameObject.SetActive(true);
	}

	// Token: 0x06000758 RID: 1880 RVA: 0x000414B0 File Offset: 0x0003F6B0
	private void DisplayCurrentList()
	{
		this.masterScrollBar.SetActive(false);
		for (int i = 0; i < this.vesselBarsCurrent.Length; i++)
		{
			UnityEngine.Object.Destroy(this.vesselBarsCurrent[i].gameObject);
		}
		this.vesselBarsCurrent = new EditorVesselBar[this.vesselGroups[this.currentGroup].groupVesselList.Count];
		for (int j = 0; j < this.vesselBarsCurrent.Length; j++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(this.editorVesselBarObject, this.masterVesselParent) as GameObject;
			this.vesselBarsCurrent[j] = gameObject.GetComponent<EditorVesselBar>();
			this.vesselBarsCurrent[j].transform.SetParent(this.currentVesselParent);
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			this.vesselBarsCurrent[j].vesselName.text = "\t\t" + UIFunctions.globaluifunctions.database.databaseshipdata[this.vesselGroups[this.currentGroup].groupVesselList[j]].shipDesignation + "   " + UIFunctions.globaluifunctions.database.databaseshipdata[this.vesselGroups[this.currentGroup].groupVesselList[j]].shipclass;
			this.vesselBarsCurrent[j].checkbox.gameObject.SetActive(false);
			string[] array = UIFunctions.globaluifunctions.database.databaseshipdata[this.vesselGroups[this.currentGroup].groupVesselList[j]].shipPrefabName.Split(new char[]
			{
				'_'
			});
			this.vesselBarsCurrent[j].flag.enabled = false;
			if (UIFunctions.globaluifunctions.database.databaseshipdata[this.vesselGroups[this.currentGroup].groupVesselList[j]].shipDesignation != "FV")
			{
				string text = array[0];
				switch (text)
				{
				case "usn":
					this.vesselBarsCurrent[j].flag.sprite = this.flags[0];
					this.vesselBarsCurrent[j].flag.enabled = true;
					break;
				case "wp":
					this.vesselBarsCurrent[j].flag.sprite = this.flags[1];
					this.vesselBarsCurrent[j].flag.enabled = true;
					break;
				case "plan":
					this.vesselBarsCurrent[j].flag.sprite = this.flags[2];
					this.vesselBarsCurrent[j].flag.enabled = true;
					break;
				}
			}
		}
		this.CheckCurrentListScrollBar();
		float y = 21f * ((float)this.vesselBarsCurrent.Length / 3f);
		this.currentVesselRect.sizeDelta = new Vector2(this.currentVesselRect.sizeDelta.x, y);
		Text numInGroup = this.vesselGroups[this.currentGroup].numInGroup;
		int num = this.vesselBarsCurrent.Length;
		numInGroup.text = num.ToString();
		this.masterVesselParent.parent.gameObject.SetActive(false);
		this.currentVesselParent.parent.gameObject.SetActive(true);
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x00041850 File Offset: 0x0003FA50
	private void CheckCurrentListScrollBar()
	{
		if (this.vesselBarsCurrent.Length > 0)
		{
			this.currentScrollBar.SetActive(true);
		}
		else
		{
			this.currentScrollBar.SetActive(false);
		}
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x00041880 File Offset: 0x0003FA80
	private void SetupAircraft()
	{
		if (this.mapHelicopters[this.mapSelectionField.value] == "FALSE")
		{
			this.helicopterType.ClearOptions();
			this.helicopterType.interactable = false;
		}
		else
		{
			this.helicopterType.interactable = true;
			this.helicopterType.ClearOptions();
			string[] array = this.mapHelicopters[this.mapSelectionField.value].Split(new char[]
			{
				','
			});
			List<string> list = new List<string>();
			for (int i = 0; i < array.Length; i++)
			{
				list.Add(UIFunctions.globaluifunctions.database.databaseaircraftdata[UIFunctions.globaluifunctions.textparser.GetAircraftID(array[i])].aircraftName);
			}
			this.helicopterType.AddOptions(list);
		}
		if (this.mapAircraft[this.mapSelectionField.value] == "FALSE")
		{
			this.aircraftType.ClearOptions();
			this.aircraftType.interactable = false;
		}
		else
		{
			this.aircraftType.interactable = true;
			this.aircraftType.ClearOptions();
			string[] array2 = this.mapAircraft[this.mapSelectionField.value].Split(new char[]
			{
				','
			});
			List<string> list2 = new List<string>();
			for (int j = 0; j < array2.Length; j++)
			{
				list2.Add(UIFunctions.globaluifunctions.database.databaseaircraftdata[UIFunctions.globaluifunctions.textparser.GetAircraftID(array2[j])].aircraftName);
			}
			this.aircraftType.AddOptions(list2);
		}
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x00041A3C File Offset: 0x0003FC3C
	private void GetOtherVessels()
	{
		string[] array = UIFunctions.globaluifunctions.textparser.OpenTextDataFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString(this.mapVesselsAndTraffic[this.mapSelectionField.value]));
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			string text = array2[0];
			if (text != null)
			{
				if (EditorMission.<>f__switch$map15 == null)
				{
					EditorMission.<>f__switch$map15 = new Dictionary<string, int>(1)
					{
						{
							"OtherVessels",
							0
						}
					};
				}
				int num;
				if (EditorMission.<>f__switch$map15.TryGetValue(text, out num))
				{
					if (num == 0)
					{
						string[] array3 = UIFunctions.globaluifunctions.textparser.PopulateStringArray(array2[1].Trim());
						this.otherVessels = new int[array3.Length];
						for (int j = 0; j < array3.Length; j++)
						{
							this.otherVessels[j] = UIFunctions.globaluifunctions.textparser.GetShipID(array3[j]);
						}
					}
				}
			}
		}
		for (int k = 0; k < this.vesselBarsMaster.Length; k++)
		{
			int index = k;
			this.vesselBarsMaster[k].vesselName.gameObject.GetComponent<Button>().onClick.RemoveListener(delegate()
			{
				EditorMission.instance.VesselBarProfile(UIFunctions.globaluifunctions.database.databaseshipdata[index].shipID);
			});
			UnityEngine.Object.Destroy(this.vesselBarsMaster[k].gameObject);
		}
		this.vesselBarsMaster = new EditorVesselBar[this.otherVessels.Length];
		for (int l = 0; l < this.vesselBarsMaster.Length; l++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(this.editorVesselBarObject, this.masterVesselParent) as GameObject;
			this.vesselBarsMaster[l] = gameObject.GetComponent<EditorVesselBar>();
			this.vesselBarsMaster[l].transform.SetParent(this.masterVesselParent);
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			this.vesselBarsMaster[l].vesselName.text = "\t\t" + UIFunctions.globaluifunctions.database.databaseshipdata[this.otherVessels[l]].shipDesignation + "   " + UIFunctions.globaluifunctions.database.databaseshipdata[this.otherVessels[l]].shipclass;
			int index = l;
			this.vesselBarsMaster[l].vesselName.gameObject.GetComponent<Button>().onClick.AddListener(delegate()
			{
				EditorMission.instance.VesselBarProfile(UIFunctions.globaluifunctions.database.databaseshipdata[index].shipID);
			});
			string[] array4 = UIFunctions.globaluifunctions.database.databaseshipdata[this.otherVessels[l]].shipPrefabName.Split(new char[]
			{
				'_'
			});
			this.vesselBarsMaster[l].flag.enabled = false;
			if (UIFunctions.globaluifunctions.database.databaseshipdata[this.otherVessels[l]].shipDesignation != "FV")
			{
				string text = array4[0];
				switch (text)
				{
				case "usn":
					this.vesselBarsMaster[l].flag.sprite = this.flags[0];
					this.vesselBarsMaster[l].flag.enabled = true;
					break;
				case "wp":
					this.vesselBarsMaster[l].flag.sprite = this.flags[1];
					this.vesselBarsMaster[l].flag.enabled = true;
					break;
				case "plan":
					this.vesselBarsMaster[l].flag.sprite = this.flags[2];
					this.vesselBarsMaster[l].flag.enabled = true;
					break;
				}
			}
		}
		float y = 21f * ((float)this.vesselBarsMaster.Length / 3f);
		this.masterVesselRect.sizeDelta = new Vector2(this.masterVesselRect.sizeDelta.x, y);
		this.masterVesselParent.parent.gameObject.SetActive(false);
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x00041ED0 File Offset: 0x000400D0
	private void VesselBarProfile(int index)
	{
		string str = UIFunctions.globaluifunctions.database.databaseshipdata[this.otherVessels[index]].shipPrefabName + "_profile";
		this.shipProfilePreview.sprite = UIFunctions.globaluifunctions.textparser.GetSprite(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("vessels/profile/" + str + ".png"));
		this.shipPreviewNameText.text = this.vesselBarsMaster[index].vesselName.text;
		this.shipProfilePreview.enabled = true;
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x00041F68 File Offset: 0x00040168
	public void SetMissionNumber(int number = 0)
	{
		this.CancelMasterList();
		if (this.GetErrors())
		{
			return;
		}
		UIFunctions.globaluifunctions.missionmanager.currentMission = number;
		UIFunctions.globaluifunctions.missionmanager.currentMissionID = number;
		UIFunctions.globaluifunctions.selectionGroupText.text = null;
		UIFunctions.globaluifunctions.textparser.PopulateLevelLoadData(string.Empty, this.ExportCustomMissionToString().Split(new char[]
		{
			'\n'
		}));
		UIFunctions.globaluifunctions.missionmanager.SelectShip();
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.combatLocation = string.Empty;
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600075E RID: 1886 RVA: 0x00042018 File Offset: 0x00040218
	private bool GetErrors()
	{
		this.errorText.text = string.Empty;
		bool flag = false;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < this.vesselGroups.Length; i++)
		{
			if (this.vesselGroups[i].useGroup.isOn)
			{
				flag = true;
				num += this.vesselGroups[i].minVessels.value;
				num2 += this.vesselGroups[i].maxVessels.value + 1;
				num3 += this.vesselGroups[i].groupVesselList.Count;
			}
		}
		if (!flag)
		{
			this.errorText.text = LanguageManager.GetDictionaryString(LanguageManager.editorDictionary, "NoGroups");
			return true;
		}
		if (num == 0)
		{
			this.errorText.text = LanguageManager.GetDictionaryString(LanguageManager.editorDictionary, "MinZero");
			return true;
		}
		if (num2 > 15)
		{
			this.errorText.text = LanguageManager.GetDictionaryString(LanguageManager.editorDictionary, "MaxHigh");
			return true;
		}
		if (num3 == 0)
		{
			this.errorText.text = LanguageManager.GetDictionaryString(LanguageManager.editorDictionary, "NoShips");
			return true;
		}
		return false;
	}

	// Token: 0x0600075F RID: 1887 RVA: 0x00042144 File Offset: 0x00040344
	private string ExportCustomMissionToString()
	{
		string text = string.Empty;
		text += "UseTerrain=";
		if (this.useTerrain.isOn)
		{
			text += "TRUE";
		}
		else
		{
			text += "FALSE";
		}
		string text2 = text;
		text = string.Concat(new string[]
		{
			text2,
			"\nMapCoordinates=",
			this.mapCoordsX.text,
			",",
			this.mapCoordsY.text
		});
		text = text + "\nMapNavigationData=" + this.mapNavData[this.mapSelectionField.value];
		text = text + "\nMapElevationData=" + this.mapElevationData[this.mapSelectionField.value];
		text = text + "\nWorldObjectsData=" + this.mapWorldObjects[this.mapSelectionField.value];
		text = text + "\nVesselsAndTraffic=" + this.mapVesselsAndTraffic[this.mapSelectionField.value];
		text += "\n\n//Environment";
		text2 = text;
		text = string.Concat(new string[]
		{
			text2,
			"\nDate=",
			this.dateDay.captionText.text,
			" ",
			this.dateMonth.captionText.text,
			" ",
			this.dateYear.text
		});
		text = text + "\nHemisphere=" + this.mapHemisphere[this.mapSelectionField.value];
		if (this.timeField.value == 0)
		{
			text += "\nTime=RANDOM";
		}
		else
		{
			text = text + "\nTime=" + int.Parse(this.timeField.captionText.text.Replace(":", string.Empty));
		}
		text += "\nUsePresetEnvironment=";
		if (this.usePresetEnvironment.isOn)
		{
			text += "TRUE";
			if (this.weatherField.value != 0)
			{
				text = text + "\nWeather=" + UIFunctions.globaluifunctions.playerfunctions.sensormanager.weather[this.weatherField.value - 1];
			}
			else
			{
				text += "\nWeather=RANDOM";
			}
			if (this.seaStateField.value != 0)
			{
				text = text + "\nSeaState=" + UIFunctions.globaluifunctions.playerfunctions.sensormanager.seaStates[this.seaStateField.value - 1];
			}
			else
			{
				text += "\nSeaState=RANDOM";
			}
			if (this.ductStrengthField.value != 0)
			{
				text = text + "\nDuctStrength=" + UIFunctions.globaluifunctions.playerfunctions.sensormanager.strengthTypes[this.ductStrengthField.value - 1];
			}
			else
			{
				text += "\nDuctStrength=RANDOM";
			}
			if (this.layerStrengthField.value != 0)
			{
				text = text + "\nLayerStrength=" + UIFunctions.globaluifunctions.playerfunctions.sensormanager.strengthTypes[this.layerStrengthField.value - 1];
			}
			else
			{
				text += "\nLayerStrength=RANDOM";
			}
		}
		else
		{
			text += "FALSE";
		}
		text += "\n\n//Enemy Ships";
		text += "\nNumberOfEnemyUnits=";
		for (int i = 0; i < this.vesselGroups.Length; i++)
		{
			if (this.vesselGroups[i].useGroup.isOn)
			{
				int num = this.vesselGroups[i].maxVessels.value + 1;
				if (num < this.vesselGroups[i].minVessels.value)
				{
					num = this.vesselGroups[i].minVessels.value;
				}
				text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					this.vesselGroups[i].minVessels.value,
					"-",
					num.ToString(),
					","
				});
			}
		}
		text = text.Remove(text.Length - 1);
		text += "\nCombatBehaviour=";
		for (int j = 0; j < this.vesselGroups.Length; j++)
		{
			if (this.vesselGroups[j].useGroup.isOn)
			{
				if (this.vesselGroups[j].aggressive.isOn)
				{
					text += "OFFENSIVE,";
				}
				else
				{
					text += "DEFENSIVE,";
				}
			}
		}
		text = text.Remove(text.Length - 1);
		text += "\nEnemyShipClasses=";
		for (int k = 0; k < this.vesselGroups.Length; k++)
		{
			if (this.vesselGroups[k].useGroup.isOn)
			{
				for (int l = 0; l < this.vesselGroups[k].groupVesselList.Count; l++)
				{
					text = text + UIFunctions.globaluifunctions.database.databaseshipdata[this.vesselGroups[k].groupVesselList[l]].shipPrefabName + "|";
				}
				text = text.Remove(text.Length - 1);
				text += ",";
			}
		}
		text = text.Remove(text.Length - 1);
		text = text + "\nFormationCruiseSpeed=" + this.cruiseSpeed.value;
		text += "\n\nUsePresetPositions=FALSE";
		text += "\n\n//Enemy Aircraft";
		if (!this.helicopterType.interactable)
		{
			text += "\nNumberOfHelicopters=0";
			text += "\nHelicopterType=FALSE";
		}
		else
		{
			string[] array = this.mapHelicopters[this.mapSelectionField.value].Split(new char[]
			{
				','
			});
			text = text + "\nNumberOfHelicopters=" + this.helicopterNumber.value;
			text = text + "\nHelicopterType=" + array[this.helicopterType.value];
		}
		if (!this.aircraftType.interactable)
		{
			text += "\nNumberOfAircraft=0";
			text += "\nAircraftType=FALSE";
		}
		else
		{
			string[] array2 = this.mapAircraft[this.mapSelectionField.value].Split(new char[]
			{
				','
			});
			text = text + "\nNumberOfAircraft=" + this.aircraftNumber.value;
			text = text + "\nAircraftType=" + array2[this.aircraftType.value];
		}
		text = text + "\n\nPlayerVessels=" + this.mapPlayerVessels[this.mapSelectionField.value];
		string[] array3 = this.defaultPlayerCommandFleet[this.mapSelectionField.value].Split(new char[]
		{
			'|'
		});
		text = text + "\nCommanderName=" + array3[0];
		text = text + "\nCommanderFleetName=" + array3[1];
		Debug.Log(text);
		return text;
	}

	// Token: 0x06000760 RID: 1888 RVA: 0x00042894 File Offset: 0x00040A94
	public void ExportCustomMissionToText()
	{
	}

	// Token: 0x06000761 RID: 1889 RVA: 0x00042898 File Offset: 0x00040A98
	public void OpenMissionEditorTab(int index)
	{
		for (int i = 0; i < this.headerImages.Length; i++)
		{
			this.headerImages[i].enabled = true;
			this.editorTabs[i].SetActive(false);
		}
		this.headerImages[index].enabled = false;
		this.editorTabs[index].SetActive(true);
	}

	// Token: 0x06000762 RID: 1890 RVA: 0x000428F8 File Offset: 0x00040AF8
	private void BuildInGameText(Text[] currenttextArray)
	{
		for (int i = 0; i < currenttextArray.Length; i++)
		{
			if (currenttextArray[i] != null)
			{
				if (LanguageManager.editorDictionary.ContainsKey(currenttextArray[i].name))
				{
					currenttextArray[i].text = LanguageManager.editorDictionary[currenttextArray[i].name];
				}
				else
				{
					Debug.LogError("LanguageManager.editorDictionary does not contain key for " + currenttextArray[i].name);
				}
			}
		}
		this.mapSelectionField.ClearOptions();
		this.mapSelectionField.AddOptions(this.mapNames);
	}

	// Token: 0x040009D1 RID: 2513
	public static EditorMission instance;

	// Token: 0x040009D2 RID: 2514
	public bool isInitialised;

	// Token: 0x040009D3 RID: 2515
	public GameObject singleMissionScrollPanel;

	// Token: 0x040009D4 RID: 2516
	public GameObject selectionScreen;

	// Token: 0x040009D5 RID: 2517
	public GameObject[] editorTabs;

	// Token: 0x040009D6 RID: 2518
	public Image[] headerImages;

	// Token: 0x040009D7 RID: 2519
	public Text errorText;

	// Token: 0x040009D8 RID: 2520
	public Text filepathText;

	// Token: 0x040009D9 RID: 2521
	public InputField filenameField;

	// Token: 0x040009DA RID: 2522
	public GameObject quickMissionButton;

	// Token: 0x040009DB RID: 2523
	public Dropdown mapSelectionField;

	// Token: 0x040009DC RID: 2524
	public Dropdown timeField;

	// Token: 0x040009DD RID: 2525
	public Dropdown weatherField;

	// Token: 0x040009DE RID: 2526
	public Dropdown seaStateField;

	// Token: 0x040009DF RID: 2527
	public Dropdown ductStrengthField;

	// Token: 0x040009E0 RID: 2528
	public Dropdown layerStrengthField;

	// Token: 0x040009E1 RID: 2529
	public Dropdown cruiseSpeed;

	// Token: 0x040009E2 RID: 2530
	public Toggle useTerrain;

	// Token: 0x040009E3 RID: 2531
	public Toggle usePresetEnvironment;

	// Token: 0x040009E4 RID: 2532
	public InputField mapCoordsX;

	// Token: 0x040009E5 RID: 2533
	public InputField mapCoordsY;

	// Token: 0x040009E6 RID: 2534
	public InputField playerSubCommand;

	// Token: 0x040009E7 RID: 2535
	public InputField playerSubFleet;

	// Token: 0x040009E8 RID: 2536
	public Dropdown dateDay;

	// Token: 0x040009E9 RID: 2537
	public Dropdown dateMonth;

	// Token: 0x040009EA RID: 2538
	public InputField dateYear;

	// Token: 0x040009EB RID: 2539
	public Text playerSubClasses;

	// Token: 0x040009EC RID: 2540
	public Image mapPreview;

	// Token: 0x040009ED RID: 2541
	public List<string> mapNames;

	// Token: 0x040009EE RID: 2542
	public List<string> mapImages;

	// Token: 0x040009EF RID: 2543
	public List<string> mapNavData;

	// Token: 0x040009F0 RID: 2544
	public List<string> mapElevationData;

	// Token: 0x040009F1 RID: 2545
	public List<string> mapWorldObjects;

	// Token: 0x040009F2 RID: 2546
	public List<string> mapVesselsAndTraffic;

	// Token: 0x040009F3 RID: 2547
	public List<string> mapHemisphere;

	// Token: 0x040009F4 RID: 2548
	public List<string> defaultPlayerCommandFleet;

	// Token: 0x040009F5 RID: 2549
	public List<int> mapDefaultYear;

	// Token: 0x040009F6 RID: 2550
	public List<Vector2> mapDefaultCoords;

	// Token: 0x040009F7 RID: 2551
	public List<string> mapPlayerVessels;

	// Token: 0x040009F8 RID: 2552
	public List<string> mapHelicopters;

	// Token: 0x040009F9 RID: 2553
	public List<string> mapAircraft;

	// Token: 0x040009FA RID: 2554
	public EditorMissionVesselGroup[] vesselGroups;

	// Token: 0x040009FB RID: 2555
	public Transform highlight;

	// Token: 0x040009FC RID: 2556
	public int currentGroup;

	// Token: 0x040009FD RID: 2557
	public GameObject editorVesselBarObject;

	// Token: 0x040009FE RID: 2558
	public int[] otherVessels;

	// Token: 0x040009FF RID: 2559
	public EditorVesselBar[] vesselBarsMaster;

	// Token: 0x04000A00 RID: 2560
	public EditorVesselBar[] vesselBarsCurrent;

	// Token: 0x04000A01 RID: 2561
	public Transform masterVesselParent;

	// Token: 0x04000A02 RID: 2562
	public Transform currentVesselParent;

	// Token: 0x04000A03 RID: 2563
	public RectTransform masterVesselRect;

	// Token: 0x04000A04 RID: 2564
	public RectTransform currentVesselRect;

	// Token: 0x04000A05 RID: 2565
	public GameObject masterScrollBar;

	// Token: 0x04000A06 RID: 2566
	public GameObject currentScrollBar;

	// Token: 0x04000A07 RID: 2567
	public Sprite[] flags;

	// Token: 0x04000A08 RID: 2568
	public Image shipProfilePreview;

	// Token: 0x04000A09 RID: 2569
	public Text shipPreviewNameText;

	// Token: 0x04000A0A RID: 2570
	public Dropdown helicopterType;

	// Token: 0x04000A0B RID: 2571
	public Dropdown aircraftType;

	// Token: 0x04000A0C RID: 2572
	public Dropdown helicopterNumber;

	// Token: 0x04000A0D RID: 2573
	public Dropdown aircraftNumber;

	// Token: 0x04000A0E RID: 2574
	public Text[] interfaceText;
}
