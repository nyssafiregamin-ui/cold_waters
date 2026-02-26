using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000145 RID: 325
public class PortRearm : MonoBehaviour
{
	// Token: 0x0600096E RID: 2414 RVA: 0x0006D450 File Offset: 0x0006B650
	public void InitialisePortRearmRepair()
	{
		this.playerVessel = GameDataManager.playervesselsonlevel[0];
		this.currentWeapon = 0;
		this.weaponIDList = new int[]
		{
			-1,
			-1,
			-1,
			-1,
			-1,
			-1
		};
		this.vlsOnly = new bool[6];
		for (int i = 0; i < this.weaponImages.Length; i++)
		{
			this.weaponImages[i].gameObject.SetActive(false);
		}
		int num = 0;
		for (int j = 0; j < this.playerVessel.vesselmovement.weaponSource.torpedoNames.Length; j++)
		{
			this.weaponImages[j].gameObject.SetActive(true);
			this.weaponImages[j].sprite = UIFunctions.globaluifunctions.database.databaseweapondata[this.playerVessel.vesselmovement.weaponSource.torpedoTypes[j]].weaponImage;
			this.reloadNameNumber[j].text = this.playerVessel.vesselmovement.weaponSource.torpedoNames[j];
			Text text = this.reloadNameNumber[j];
			text.text = text.text + "\n" + this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[j];
			this.vlsNumber[j].text = string.Empty;
			this.weaponIDList[j] = this.playerVessel.vesselmovement.weaponSource.torpedoTypes[j];
			num++;
		}
		if (this.playerVessel.vesselmovement.weaponSource.hasVLS)
		{
			for (int k = 0; k < this.playerVessel.vesselmovement.weaponSource.vlsTorpedoTypes.Length; k++)
			{
				bool flag = false;
				foreach (int num2 in this.playerVessel.vesselmovement.weaponSource.torpedoTypes)
				{
					if (this.playerVessel.vesselmovement.weaponSource.vlsTorpedoTypes[k] == num2)
					{
						flag = true;
					}
				}
				if (!flag && num < 6)
				{
					this.weaponImages[num].gameObject.SetActive(true);
					this.weaponImages[num].sprite = UIFunctions.globaluifunctions.database.databaseweapondata[this.playerVessel.vesselmovement.weaponSource.vlsTorpedoTypes[k]].weaponImage;
					this.reloadNameNumber[num].text = this.playerVessel.vesselmovement.weaponSource.vlsTorpedoNames[k];
					this.vlsNumber[num].text = string.Empty;
					this.weaponIDList[num] = this.playerVessel.vesselmovement.weaponSource.vlsTorpedoTypes[k];
					this.vlsOnly[num] = true;
					num++;
				}
			}
		}
		this.SetLoadoutStats();
	}

	// Token: 0x0600096F RID: 2415 RVA: 0x0006D730 File Offset: 0x0006B930
	public void PortLoadSealTeam()
	{
		this.AddSealTeamOnBoard(GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.sealsOnBoard);
		UIFunctions.globaluifunctions.campaignmanager.PortTopMenu();
	}

	// Token: 0x06000970 RID: 2416 RVA: 0x0006D760 File Offset: 0x0006B960
	public void SetPlayerNumberOfWires()
	{
		this.numberOfPlayerWires.text = string.Concat(new object[]
		{
			LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "WIRES"),
			" ",
			UIFunctions.globaluifunctions.playerfunctions.numberOfWiresUsed,
			"/",
			UIFunctions.globaluifunctions.playerfunctions.numberOfWires
		});
	}

	// Token: 0x06000971 RID: 2417 RVA: 0x0006D7D4 File Offset: 0x0006B9D4
	private void AddSealTeamOnBoard(bool addSeals)
	{
		int num = 0;
		if (this.playerVessel.vesselmovement.weaponSource.hasVLS)
		{
			num = 1;
		}
		if (!addSeals)
		{
			float num2 = 0f;
			int num3 = 0;
			for (int i = 0; i < this.playerVessel.databaseshipdata.torpedoNumbers.Length; i++)
			{
				num3 += this.playerVessel.databaseshipdata.torpedoNumbers[i];
			}
			int num4 = 0;
			for (int j = 0; j < this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard.Length; j++)
			{
				num4 += this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[j];
			}
			int num5 = 0;
			for (int k = 0; k < this.playerVessel.vesselmovement.weaponSource.tubeStatus.Length - num; k++)
			{
				if (this.playerVessel.vesselmovement.weaponSource.tubeStatus[k] >= 0)
				{
					num5++;
				}
			}
			int num6 = num4 - num5;
			if (num6 > 0)
			{
				for (int l = 0; l < this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard.Length; l++)
				{
					int num7 = 0;
					for (int m = 0; m < this.playerVessel.vesselmovement.weaponSource.tubeStatus.Length - num; m++)
					{
						if (this.playerVessel.vesselmovement.weaponSource.tubeStatus[m] == l)
						{
							num7++;
						}
					}
					num2 += UIFunctions.globaluifunctions.database.databaseweapondata[this.playerVessel.vesselmovement.weaponSource.torpedoTypes[l]].replenishTime * (float)(this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[l] - num7);
					this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[l] = num7;
				}
			}
			this.SetPortTimerBasedOnReload(num2 + UIFunctions.globaluifunctions.campaignmanager.commandoLoadTime);
			this.sealTeamImage.gameObject.SetActive(true);
			this.playerVessel.vesselmovement.weaponSource.sealsOnBoard = true;
			this.SetLoadoutStats();
		}
		else
		{
			this.SetPortTimerBasedOnReload(UIFunctions.globaluifunctions.campaignmanager.commandoLoadTime);
			this.sealTeamImage.gameObject.SetActive(false);
			this.playerVessel.vesselmovement.weaponSource.sealsOnBoard = false;
			this.SetLoadoutStats();
		}
	}

	// Token: 0x06000972 RID: 2418 RVA: 0x0006DA6C File Offset: 0x0006BC6C
	public void SetLoadoutStats()
	{
		this.numberOfNoiseMakers.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "NoisemakerStores") + " " + this.playerVessel.vesselmovement.weaponSource.noisemakersOnBoard.ToString();
		this.noisemakerHelmPanelText.text = this.playerVessel.vesselmovement.weaponSource.noisemakersOnBoard.ToString();
		for (int i = 0; i < this.reloadNameNumber.Length; i++)
		{
			this.reloadNameNumber[i].color = this.textColors[1];
			this.highlights[i].enabled = false;
			if (i == this.currentWeapon && this.portControls.activeSelf)
			{
				this.highlights[i].enabled = true;
			}
		}
		int num = 0;
		if (this.playerVessel.vesselmovement.weaponSource.hasVLS)
		{
			num = 1;
		}
		int[] array = new int[this.playerVessel.vesselmovement.weaponSource.torpedoNames.Length];
		for (int j = 0; j < this.playerVessel.vesselmovement.weaponSource.tubeStatus.Length - num; j++)
		{
			if (this.playerVessel.vesselmovement.weaponSource.tubeStatus[j] >= 0)
			{
				array[this.playerVessel.vesselmovement.weaponSource.tubeStatus[j]]++;
			}
		}
		for (int k = 0; k < this.weaponIDList.Length; k++)
		{
			if (this.weaponIDList[k] >= 0)
			{
				this.reloadNameNumber[k].text = UIFunctions.globaluifunctions.database.databaseweapondata[this.weaponIDList[k]].weaponName + "\n";
				if (UIFunctions.globaluifunctions.menuSystemParent.activeSelf && UIFunctions.globaluifunctions.campaignmanager.playerInPort)
				{
					if (!GameDataManager.missionMode && !GameDataManager.trainingMode)
					{
						this.vlsNumber[k].text = string.Concat(new string[]
						{
							"  <b>",
							Mathf.RoundToInt(UIFunctions.globaluifunctions.database.databaseweapondata[this.weaponIDList[k]].replenishTime * OptionsManager.difficultySettings["RestockTimeModifier"]).ToString(),
							" ",
							LanguageManager.interfaceDictionary["Minutes"],
							"</b>\n"
						});
					}
				}
				else
				{
					this.vlsNumber[k].text = string.Empty;
				}
			}
		}
		for (int l = 0; l < this.playerVessel.vesselmovement.weaponSource.torpedoNames.Length; l++)
		{
			Text text = this.reloadNameNumber[l];
			text.text += (this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[l] - array[l]).ToString();
			Text text2 = this.reloadNameNumber[l];
			text2.text = text2.text + "  (" + array[l].ToString() + ")";
		}
		for (int m = 0; m < this.playerVessel.databaseshipdata.torpedotubes; m++)
		{
			if (this.playerVessel.vesselmovement.weaponSource.tubeStatus[m] >= 0)
			{
				UIFunctions.globaluifunctions.playerfunctions.torpedoTubeImages[m].gameObject.SetActive(true);
				UIFunctions.globaluifunctions.playerfunctions.torpedoTubeImages[m].sprite = UIFunctions.globaluifunctions.database.databaseweapondata[UIFunctions.globaluifunctions.playerfunctions.GetPlayerTorpedoIDInTube(m)].weaponImage;
			}
			else if (this.playerVessel.vesselmovement.weaponSource.weaponInTube[m] == -100)
			{
				UIFunctions.globaluifunctions.playerfunctions.torpedoTubeImages[m].gameObject.SetActive(true);
				UIFunctions.globaluifunctions.playerfunctions.torpedoTubeImages[m].sprite = UIFunctions.globaluifunctions.playerfunctions.wireSprite;
			}
			else if (this.playerVessel.vesselmovement.weaponSource.tubeStatus[m] != -200)
			{
				UIFunctions.globaluifunctions.playerfunctions.torpedoTubeImages[m].gameObject.SetActive(false);
			}
		}
		int num2 = 0;
		for (int n = 0; n < this.playerVessel.databaseshipdata.torpedoNumbers.Length; n++)
		{
			num2 += this.playerVessel.databaseshipdata.torpedoNumbers[n];
		}
		int num3 = 0;
		for (int num4 = 0; num4 < this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard.Length; num4++)
		{
			num3 += this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[num4];
		}
		int num5 = 0;
		int num6 = 0;
		if (this.playerVessel.vesselmovement.weaponSource.hasVLS)
		{
			for (int num7 = 0; num7 < this.playerVessel.databaseshipdata.vlsTorpedoNumbers.Length; num7++)
			{
				num5 += this.playerVessel.databaseshipdata.vlsTorpedoNumbers[num7];
			}
			for (int num8 = 0; num8 < this.playerVessel.vesselmovement.weaponSource.vlsCurrentTorpsOnBoard.Length; num8++)
			{
				num6 += this.playerVessel.vesselmovement.weaponSource.vlsCurrentTorpsOnBoard[num8];
			}
			for (int num9 = 0; num9 < this.playerVessel.vesselmovement.weaponSource.vlsTorpedoTypes.Length; num9++)
			{
				for (int num10 = 0; num10 < this.weaponIDList.Length; num10++)
				{
					if (this.playerVessel.vesselmovement.weaponSource.vlsTorpedoTypes[num9] == this.weaponIDList[num10])
					{
						if (!this.vlsOnly[num10])
						{
							Text text3 = this.reloadNameNumber[num10];
							text3.text += "  ";
						}
						Text text4 = this.reloadNameNumber[num10];
						text4.text = text4.text + "VLS: " + this.playerVessel.vesselmovement.weaponSource.vlsCurrentTorpsOnBoard[num9];
					}
				}
			}
		}
		if (GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.sealsOnBoard)
		{
			num2 = this.playerVessel.databaseshipdata.torpedotubes - num;
		}
		this.slotNumbers.text = num3.ToString() + " / " + num2.ToString();
		if (this.playerVessel.vesselmovement.weaponSource.hasVLS)
		{
			Text text5 = this.slotNumbers;
			string text6 = text5.text;
			text5.text = string.Concat(new string[]
			{
				text6,
				"\nVLS: ",
				num6.ToString(),
				" / ",
				num5.ToString()
			});
		}
		this.sealTeamImage.gameObject.SetActive(GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.sealsOnBoard);
	}

	// Token: 0x06000973 RID: 2419 RVA: 0x0006E1F4 File Offset: 0x0006C3F4
	private void SetPortTimerBasedOnReload(float time)
	{
		if (!GameDataManager.missionMode && !GameDataManager.trainingMode)
		{
			time /= 60f;
			UIFunctions.globaluifunctions.campaignmanager.timeInPort += time * OptionsManager.difficultySettings["RestockTimeModifier"];
			UIFunctions.globaluifunctions.campaignmanager.gameObject.SetActive(true);
			UIFunctions.globaluifunctions.campaignmanager.enabled = true;
		}
	}

	// Token: 0x06000974 RID: 2420 RVA: 0x0006E26C File Offset: 0x0006C46C
	public void PortLoadWeapon(bool isAdded)
	{
		int num = 0;
		if (this.playerVessel.vesselmovement.weaponSource.hasVLS)
		{
			num = 1;
		}
		if (!isAdded)
		{
			int num2 = 0;
			for (int i = 0; i < this.playerVessel.vesselmovement.weaponSource.tubeStatus.Length - num; i++)
			{
				if (this.playerVessel.vesselmovement.weaponSource.tubeStatus[i] == this.currentWeapon)
				{
					num2++;
				}
			}
			bool flag = false;
			if (this.playerVessel.vesselmovement.weaponSource.hasVLS)
			{
				for (int j = 0; j < this.playerVessel.vesselmovement.weaponSource.vlsTorpedoTypes.Length; j++)
				{
					if (this.weaponIDList[this.currentWeapon] == this.playerVessel.vesselmovement.weaponSource.vlsTorpedoTypes[j] && this.playerVessel.vesselmovement.weaponSource.vlsCurrentTorpsOnBoard[j] > 0)
					{
						this.playerVessel.vesselmovement.weaponSource.vlsCurrentTorpsOnBoard[j]--;
						flag = true;
						this.SetPortTimerBasedOnReload(UIFunctions.globaluifunctions.database.databaseweapondata[this.weaponIDList[this.currentWeapon]].replenishTime);
						break;
					}
				}
			}
			if (!flag)
			{
				bool flag2 = false;
				for (int k = 0; k < this.playerVessel.vesselmovement.weaponSource.torpedoTypes.Length; k++)
				{
					if (this.playerVessel.vesselmovement.weaponSource.torpedoTypes[k] == this.weaponIDList[this.currentWeapon])
					{
						flag2 = true;
					}
				}
				if (flag2)
				{
					if (this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[this.currentWeapon] > num2)
					{
						this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[this.currentWeapon]--;
						this.SetPortTimerBasedOnReload(UIFunctions.globaluifunctions.database.databaseweapondata[this.weaponIDList[this.currentWeapon]].replenishTime);
					}
					else
					{
						for (int l = 0; l < this.playerVessel.vesselmovement.weaponSource.tubeStatus.Length - num; l++)
						{
							if (this.playerVessel.vesselmovement.weaponSource.tubeStatus[l] == this.currentWeapon)
							{
								this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[this.currentWeapon]--;
								this.SetPortTimerBasedOnReload(UIFunctions.globaluifunctions.database.databaseweapondata[this.weaponIDList[this.currentWeapon]].replenishTime);
								this.playerVessel.vesselmovement.weaponSource.tubeStatus[l] = -1;
								break;
							}
						}
					}
				}
			}
		}
		else
		{
			bool flag3 = false;
			if (this.playerVessel.vesselmovement.weaponSource.hasVLS)
			{
				int num3 = 0;
				for (int m = 0; m < this.playerVessel.databaseshipdata.vlsTorpedoNumbers.Length; m++)
				{
					num3 += this.playerVessel.databaseshipdata.vlsTorpedoNumbers[m];
				}
				int num4 = 0;
				for (int n = 0; n < this.playerVessel.vesselmovement.weaponSource.vlsCurrentTorpsOnBoard.Length; n++)
				{
					num4 += this.playerVessel.vesselmovement.weaponSource.vlsCurrentTorpsOnBoard[n];
				}
				if (num3 - num4 > 0)
				{
					for (int num5 = 0; num5 < this.playerVessel.vesselmovement.weaponSource.vlsTorpedoTypes.Length; num5++)
					{
						if (this.weaponIDList[this.currentWeapon] == this.playerVessel.vesselmovement.weaponSource.vlsTorpedoTypes[num5])
						{
							this.playerVessel.vesselmovement.weaponSource.vlsCurrentTorpsOnBoard[num5]++;
							this.SetPortTimerBasedOnReload(UIFunctions.globaluifunctions.database.databaseweapondata[this.weaponIDList[this.currentWeapon]].replenishTime);
							flag3 = true;
							break;
						}
					}
				}
			}
			if (!flag3)
			{
				bool flag4 = false;
				for (int num6 = 0; num6 < this.playerVessel.vesselmovement.weaponSource.torpedoTypes.Length; num6++)
				{
					if (this.playerVessel.vesselmovement.weaponSource.torpedoTypes[num6] == this.weaponIDList[this.currentWeapon])
					{
						flag4 = true;
					}
				}
				if (flag4)
				{
					int num7 = 0;
					for (int num8 = 0; num8 < this.playerVessel.databaseshipdata.torpedoNumbers.Length; num8++)
					{
						num7 += this.playerVessel.databaseshipdata.torpedoNumbers[num8];
					}
					int num9 = 0;
					for (int num10 = 0; num10 < this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard.Length; num10++)
					{
						num9 += this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[num10];
					}
					int num11 = 0;
					for (int num12 = 0; num12 < this.playerVessel.vesselmovement.weaponSource.tubeStatus.Length - num; num12++)
					{
						if (this.playerVessel.vesselmovement.weaponSource.tubeStatus[num12] >= 0)
						{
							num11++;
						}
					}
					int num13 = num9 - num11;
					if (num13 < num7 - this.playerVessel.vesselmovement.weaponSource.tubeStatus.Length + num && !this.playerVessel.vesselmovement.weaponSource.sealsOnBoard)
					{
						this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[this.currentWeapon]++;
						this.SetPortTimerBasedOnReload(UIFunctions.globaluifunctions.database.databaseweapondata[this.weaponIDList[this.currentWeapon]].replenishTime);
					}
					else if (num11 < this.playerVessel.vesselmovement.weaponSource.tubeStatus.Length)
					{
						for (int num14 = 0; num14 < this.playerVessel.vesselmovement.weaponSource.tubeStatus.Length - num; num14++)
						{
							if (this.playerVessel.vesselmovement.weaponSource.tubeStatus[num14] < 0)
							{
								this.playerVessel.vesselmovement.weaponSource.currentTorpsOnBoard[this.currentWeapon]++;
								this.SetPortTimerBasedOnReload(UIFunctions.globaluifunctions.database.databaseweapondata[this.weaponIDList[this.currentWeapon]].replenishTime);
								this.playerVessel.vesselmovement.weaponSource.tubeStatus[num14] = this.currentWeapon;
								this.playerVessel.vesselmovement.weaponSource.weaponInTube[num14] = this.currentWeapon;
								int playerTorpedoIDInTube = UIFunctions.globaluifunctions.playerfunctions.GetPlayerTorpedoIDInTube(num14);
								this.playerVessel.vesselmovement.weaponSource.torpedoSearchPattern[num14] = UIFunctions.globaluifunctions.playerfunctions.GetSettingIndex(UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTube].searchSettings[0], UIFunctions.globaluifunctions.playerfunctions.attackSettingDefinitions);
								this.playerVessel.vesselmovement.weaponSource.torpedoDepthPattern[num14] = UIFunctions.globaluifunctions.playerfunctions.GetSettingIndex(UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTube].heightSettings[0], UIFunctions.globaluifunctions.playerfunctions.depthSettingDefinitions);
								this.playerVessel.vesselmovement.weaponSource.torpedoHomingPattern[num14] = UIFunctions.globaluifunctions.playerfunctions.GetSettingIndex(UIFunctions.globaluifunctions.database.databaseweapondata[playerTorpedoIDInTube].homeSettings[0], UIFunctions.globaluifunctions.playerfunctions.homeSettingDefinitions);
								this.playerVessel.vesselmovement.weaponSource.tubeReloadingDirection[num14] = 0f;
								UIFunctions.globaluifunctions.playerfunctions.SetTubeSettingButtons(num14);
								break;
							}
						}
					}
				}
			}
		}
		this.SetLoadoutStats();
		UIFunctions.globaluifunctions.campaignmanager.PortTopMenu();
	}

	// Token: 0x06000975 RID: 2421 RVA: 0x0006EAE8 File Offset: 0x0006CCE8
	public void SelectWeapon(int number)
	{
		this.currentWeapon = number;
		this.SetLoadoutStats();
	}

	// Token: 0x04000E58 RID: 3672
	public Vessel playerVessel;

	// Token: 0x04000E59 RID: 3673
	public int currentWeapon;

	// Token: 0x04000E5A RID: 3674
	public Image[] weaponImages;

	// Token: 0x04000E5B RID: 3675
	public Image[] highlights;

	// Token: 0x04000E5C RID: 3676
	public Image noisemakerImage;

	// Token: 0x04000E5D RID: 3677
	public Image sealTeamImage;

	// Token: 0x04000E5E RID: 3678
	public string[] sealTeamLabels;

	// Token: 0x04000E5F RID: 3679
	public Text slotNumbers;

	// Token: 0x04000E60 RID: 3680
	public Text numberOfNoiseMakers;

	// Token: 0x04000E61 RID: 3681
	public Text sealTeamText;

	// Token: 0x04000E62 RID: 3682
	public Text sealTeamTimeText;

	// Token: 0x04000E63 RID: 3683
	public Text noisemakerHelmPanelText;

	// Token: 0x04000E64 RID: 3684
	public Text[] reloadNameNumber;

	// Token: 0x04000E65 RID: 3685
	public Text[] vlsNumber;

	// Token: 0x04000E66 RID: 3686
	public GameObject portControls;

	// Token: 0x04000E67 RID: 3687
	public GameObject combatControls;

	// Token: 0x04000E68 RID: 3688
	public Color32[] textColors;

	// Token: 0x04000E69 RID: 3689
	public int[] weaponIDList;

	// Token: 0x04000E6A RID: 3690
	public bool[] vlsOnly;

	// Token: 0x04000E6B RID: 3691
	public Text numberOfPlayerWires;
}
