using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000130 RID: 304
public class LanguageManager : MonoBehaviour
{
	// Token: 0x06000834 RID: 2100 RVA: 0x00052D30 File Offset: 0x00050F30
	public void InitialiseLanguageSetUp()
	{
		UIFunctions.globaluifunctions.textparser.BuildInterfaceDictionary(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("language/dictionary/dictionary_interface"));
		this.BuildInGameText(this.mainMenuTextFields);
		this.BuildInGameText(this.museumTextFields);
		this.BuildInGameText(this.selectionScreenTextFields);
		this.BuildInGameText(this.briefingGUITextFields);
		this.BuildInGameText(this.eventGUITextFields);
		this.BuildInGameText(this.statusScreenTextFields);
		this.BuildInGameText(this.actionReportGUITextFields);
		this.BuildInGameText(this.missionExitTextFields);
		this.BuildInGameText(this.campaignExitTextFields);
		this.BuildInGameText(this.campaignTextFields);
		this.BuildInGameText(this.campaignSelectionTextFields);
		this.BuildInGameText(this.campaignGUITextFields);
		this.BuildInGameText(this.optionsTextFields);
		this.BuildInGameText(this.fatalErrorTextFields);
		this.BuildInGameText(this.hudPanelTextFields);
		UIFunctions.globaluifunctions.textparser.BuildMessageLogDictionary(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("language/dictionary/dictionary_message_log"));
	}

	// Token: 0x06000835 RID: 2101 RVA: 0x00052E38 File Offset: 0x00051038
	public void BuildCombatHUD()
	{
		this.titlesPlayer.text = LanguageManager.interfaceDictionary["Course"] + "\n";
		Text text = this.titlesPlayer;
		text.text = text.text + LanguageManager.interfaceDictionary["Speed"] + "\n";
		Text text2 = this.titlesPlayer;
		text2.text = text2.text + LanguageManager.interfaceDictionary["Depth"] + "\n";
		Text text3 = this.titlesPlayer;
		text3.text = text3.text + LanguageManager.interfaceDictionary["Rudder"] + "\n";
		Text text4 = this.titlesPlayer;
		text4.text = text4.text + LanguageManager.interfaceDictionary["Planes"] + "\n";
		Text text5 = this.titlesPlayer;
		text5.text = text5.text + LanguageManager.interfaceDictionary["Ballast"] + "\n";
		this.unitsPlayer.text = "\n" + LanguageManager.interfaceDictionary["Knot"];
		Text text6 = this.unitsPlayer;
		text6.text = text6.text + "\n" + LanguageManager.interfaceDictionary["Feet"];
		this.titlesContact.text = "\n" + LanguageManager.interfaceDictionary["Bearing"] + "\n";
		Text text7 = this.titlesContact;
		text7.text = text7.text + LanguageManager.interfaceDictionary["Course"] + "\n";
		Text text8 = this.titlesContact;
		text8.text = text8.text + LanguageManager.interfaceDictionary["Speed"] + "\n";
		Text text9 = this.titlesContact;
		text9.text = text9.text + LanguageManager.interfaceDictionary["Range"] + "\n";
		Text text10 = this.titlesContact;
		text10.text += LanguageManager.interfaceDictionary["Solution"];
		this.unitsContact.text = "\n\n\n" + LanguageManager.interfaceDictionary["Knot"];
		Text text11 = this.unitsContact;
		text11.text = text11.text + "\n" + LanguageManager.interfaceDictionary["KiloYard"];
		Text text12 = this.unitsContact;
		text12.text = text12.text + "\n" + LanguageManager.interfaceDictionary["Percentage"];
		this.titlesWeapon.text = LanguageManager.interfaceDictionary["WeaponData"] + "\n";
		Text text13 = this.titlesWeapon;
		text13.text = text13.text + LanguageManager.interfaceDictionary["Course"] + "\n";
		Text text14 = this.titlesWeapon;
		text14.text = text14.text + LanguageManager.interfaceDictionary["Range"] + "\n";
		Text text15 = this.titlesWeapon;
		text15.text = text15.text + LanguageManager.interfaceDictionary["Bearing"] + "\n";
		Text text16 = this.titlesWeapon;
		text16.text = text16.text + LanguageManager.interfaceDictionary["RangeToEnable"] + "\n";
		Text text17 = this.titlesWeapon;
		text17.text = text17.text + LanguageManager.interfaceDictionary["TimeToRun"] + "\n";
		this.unitsWeapon.text = "\n\n" + LanguageManager.interfaceDictionary["KiloYard"];
		Text text18 = this.unitsWeapon;
		text18.text = text18.text + "\n\n" + LanguageManager.interfaceDictionary["KiloYard"];
		this.BuildInGameText(this.contextualTabsTextFields);
		this.signatureLabels.text = LanguageManager.interfaceDictionary["Contact"] + "\n";
		Text text19 = this.signatureLabels;
		text19.text = text19.text + LanguageManager.interfaceDictionary["SignalStrength"] + "\n";
		Text text20 = this.signatureLabels;
		text20.text = text20.text + LanguageManager.interfaceDictionary["OwnShip"] + "\n\n";
		Text text21 = this.signatureLabels;
		text21.text = text21.text + LanguageManager.interfaceDictionary["SensorComparison"] + "\n";
		Text text22 = this.signatureLabels;
		text22.text += LanguageManager.interfaceDictionary["Contact"];
		UIFunctions.globaluifunctions.playerfunctions.vlsLabel.text = LanguageManager.interfaceDictionary["VerticalLaunchSystem"];
		this.profile.text = LanguageManager.interfaceDictionary["Profile"];
		int num = 0;
		for (int i = 0; i < this.signatureUnits.Length; i++)
		{
			this.signatureUnits[i].text = num.ToString();
			num += 500;
		}
		UIFunctions.globaluifunctions.portRearm.sealTeamLabels[0] = LanguageManager.interfaceDictionary["LoadCommando"];
		UIFunctions.globaluifunctions.portRearm.sealTeamLabels[1] = LanguageManager.interfaceDictionary["UnloadCommando"];
	}

	// Token: 0x06000836 RID: 2102 RVA: 0x00053398 File Offset: 0x00051598
	private void BuildInGameText(Text[] currenttextArray)
	{
		for (int i = 0; i < currenttextArray.Length; i++)
		{
			if (currenttextArray[i] != null && LanguageManager.interfaceDictionary.ContainsKey(currenttextArray[i].name))
			{
				currenttextArray[i].text = LanguageManager.interfaceDictionary[currenttextArray[i].name];
			}
		}
	}

	// Token: 0x06000837 RID: 2103 RVA: 0x00053400 File Offset: 0x00051600
	public static string GetDictionaryString(Dictionary<string, string> reference, string lookupValue)
	{
		if (reference.ContainsKey(lookupValue))
		{
			return reference[lookupValue];
		}
		if (UIFunctions.globaluifunctions.GUICamera.activeSelf)
		{
			UIFunctions.globaluifunctions.SetPlayerErrorMessage(string.Concat(new object[]
			{
				"ERROR: ",
				reference,
				" does not contain a value for: ",
				lookupValue
			}));
		}
		else
		{
			UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(string.Concat(new object[]
			{
				"ERROR: ",
				reference,
				" does not contain a value for: ",
				lookupValue
			}), UIFunctions.globaluifunctions.playerfunctions.messageLogColors["Warning"], string.Empty, false);
		}
		return null;
	}

	// Token: 0x04000BEE RID: 3054
	public Text[] mainMenuTextFields;

	// Token: 0x04000BEF RID: 3055
	public Text[] museumTextFields;

	// Token: 0x04000BF0 RID: 3056
	public Text[] selectionScreenTextFields;

	// Token: 0x04000BF1 RID: 3057
	public Text[] briefingGUITextFields;

	// Token: 0x04000BF2 RID: 3058
	public Text[] eventGUITextFields;

	// Token: 0x04000BF3 RID: 3059
	public Text[] statusScreenTextFields;

	// Token: 0x04000BF4 RID: 3060
	public Text[] actionReportGUITextFields;

	// Token: 0x04000BF5 RID: 3061
	public Text[] missionExitTextFields;

	// Token: 0x04000BF6 RID: 3062
	public Text[] campaignExitTextFields;

	// Token: 0x04000BF7 RID: 3063
	public Text[] campaignTextFields;

	// Token: 0x04000BF8 RID: 3064
	public Text[] campaignSelectionTextFields;

	// Token: 0x04000BF9 RID: 3065
	public Text[] campaignGUITextFields;

	// Token: 0x04000BFA RID: 3066
	public Text[] optionsTextFields;

	// Token: 0x04000BFB RID: 3067
	public Text[] fatalErrorTextFields;

	// Token: 0x04000BFC RID: 3068
	public Text[] hudPanelTextFields;

	// Token: 0x04000BFD RID: 3069
	public Text[] contextualTabsTextFields;

	// Token: 0x04000BFE RID: 3070
	public Text titlesPlayer;

	// Token: 0x04000BFF RID: 3071
	public Text unitsPlayer;

	// Token: 0x04000C00 RID: 3072
	public Text titlesContact;

	// Token: 0x04000C01 RID: 3073
	public Text unitsContact;

	// Token: 0x04000C02 RID: 3074
	public Text titlesWeapon;

	// Token: 0x04000C03 RID: 3075
	public Text unitsWeapon;

	// Token: 0x04000C04 RID: 3076
	public Text signatureLabels;

	// Token: 0x04000C05 RID: 3077
	public Text profile;

	// Token: 0x04000C06 RID: 3078
	public Text[] signatureUnits;

	// Token: 0x04000C07 RID: 3079
	public static Dictionary<string, string> interfaceDictionary;

	// Token: 0x04000C08 RID: 3080
	public static Dictionary<string, string> messageLogDictionary;

	// Token: 0x04000C09 RID: 3081
	public static Dictionary<string, string> messageLogAudioClipDictionary;

	// Token: 0x04000C0A RID: 3082
	public static Dictionary<string, string> messageLogVoiceDictionary;

	// Token: 0x04000C0B RID: 3083
	public static Dictionary<string, string> editorDictionary;

	// Token: 0x04000C0C RID: 3084
	public Vector2[] guiElementBasePositions;
}
