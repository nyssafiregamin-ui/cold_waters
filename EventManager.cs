using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200011B RID: 283
public class EventManager : MonoBehaviour
{
	// Token: 0x0600079B RID: 1947 RVA: 0x00046EF8 File Offset: 0x000450F8
	public void BringInEvent(int eventID, bool award = false, bool summary = false)
	{
		Time.timeScale = 0f;
		if (UIFunctions.globaluifunctions.playerfunctions.menuPanel.activeSelf)
		{
			UIFunctions.globaluifunctions.playerfunctions.menuPanel.SetActive(false);
		}
		this.campaignmanager.enabled = false;
		this.continueButton.SetActive(false);
		UIFunctions.globaluifunctions.SetMenuSystem("EVENT");
		UIFunctions.globaluifunctions.scrollbarDefault.value = 1f;
		this.nextEvent = -1;
		this.nextEvent = this.GetEventID(this.campaignevents[eventID].nextAction);
		this.currentEvent = eventID;
		this.CreateNewEventTemplate(award, summary);
		Time.timeScale = 1f;
		if (this.eventDebugMode)
		{
			Time.timeScale = 20f;
		}
	}

	// Token: 0x0600079C RID: 1948 RVA: 0x00046FC8 File Offset: 0x000451C8
	public void BringInCampaignSummary()
	{
		this.BringInEvent(this.specialEventIDs[23], true, true);
		this.campaignmanager.enabled = false;
		Time.timeScale = 1f;
		this.SetCampaignStats();
	}

	// Token: 0x0600079D RID: 1949 RVA: 0x00046FF8 File Offset: 0x000451F8
	private void SetCampaignStats()
	{
		this.courtMartial = this.GetIsCourtMartial();
		if (this.courtMartial && !this.campaignOver)
		{
			this.BringInEvent(this.specialEventIDs[24], false, false);
			return;
		}
		EventTemplate component = this.currentEventTemplate.GetComponent<EventTemplate>();
		int num = Mathf.FloorToInt(this.campaignmanager.hoursDurationOfCampaign / 24f);
		component.templateTexts[4].text = num.ToString() + "\n\n";
		Text text = component.templateTexts[4];
		text.text = text.text + this.campaignmanager.playercampaigndata.campaignStats[0] + "\n";
		Text text2 = component.templateTexts[4];
		text2.text = text2.text + this.campaignmanager.playercampaigndata.campaignStats[1] + "\n";
		Text text3 = component.templateTexts[4];
		text3.text = text3.text + this.campaignmanager.playercampaigndata.campaignStats[2] + "\n\n";
		Text text4 = component.templateTexts[4];
		text4.text = text4.text + string.Format("{0:0}", this.campaignmanager.playercampaigndata.campaignStats[3]) + "\n";
		Text text5 = component.templateTexts[4];
		text5.text = text5.text + string.Format("{0:0}", this.campaignmanager.playercampaigndata.campaignStats[4]) + "\n";
		Text text6 = component.templateTexts[4];
		text6.text = text6.text + string.Format("{0:0}", this.campaignmanager.playercampaigndata.campaignStats[5]) + "\n";
		Text text7 = component.templateTexts[4];
		text7.text = text7.text + string.Format("{0:0}", this.campaignmanager.playercampaigndata.campaignStats[6]) + "\n";
		Text text8 = component.templateTexts[4];
		text8.text = text8.text + (this.campaignmanager.playercampaigndata.campaignStats[3] + this.campaignmanager.playercampaigndata.campaignStats[4] + this.campaignmanager.playercampaigndata.campaignStats[5] + this.campaignmanager.playercampaigndata.campaignStats[6]).ToString() + "\n\n";
		Text text9 = component.templateTexts[4];
		text9.text = text9.text + string.Format("{0:#,0}", this.campaignmanager.playercampaigndata.campaignStats[7]) + "\n";
		Text text10 = component.templateTexts[4];
		text10.text = text10.text + string.Format("{0:#,0}", this.campaignmanager.playercampaigndata.campaignStats[8]) + "\n";
		Text text11 = component.templateTexts[4];
		text11.text = text11.text + string.Format("{0:#,0}", this.campaignmanager.playercampaigndata.campaignStats[9]) + "\n";
		Text text12 = component.templateTexts[4];
		text12.text = text12.text + string.Format("{0:#,0}", this.campaignmanager.playercampaigndata.campaignStats[10]) + "\n";
		Text text13 = component.templateTexts[4];
		text13.text = text13.text + string.Format("{0:#,0}", this.campaignmanager.playercampaigndata.campaignStats[7] + this.campaignmanager.playercampaigndata.campaignStats[8] + this.campaignmanager.playercampaigndata.campaignStats[9] + this.campaignmanager.playercampaigndata.campaignStats[10]) + "\n\n";
		this.BuildMedalsCase(component);
		component.templateTexts[2].text = string.Empty;
		if (this.campaignOver)
		{
			switch (this.endingType)
			{
			case 0:
				component.templateTexts[2].text = UIFunctions.globaluifunctions.textparser.GetRandomTextLineFromFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/ending_win"));
				break;
			case 1:
				component.templateTexts[2].text = UIFunctions.globaluifunctions.textparser.GetRandomTextLineFromFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/ending_draw"));
				break;
			case 2:
				component.templateTexts[2].text = UIFunctions.globaluifunctions.textparser.GetRandomTextLineFromFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/ending_fail"));
				break;
			case 3:
				component.templateTexts[2].text = UIFunctions.globaluifunctions.textparser.GetRandomTextLineFromFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/ending_critical_fail"));
				break;
			case 4:
				component.templateTexts[2].text = UIFunctions.globaluifunctions.textparser.GetRandomTextLineFromFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/ending_captured"));
				break;
			case 5:
				component.templateTexts[2].text = UIFunctions.globaluifunctions.textparser.GetRandomTextLineFromFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/ending_lost_at_sea"));
				break;
			}
			component.templateTexts[2].text = UIFunctions.globaluifunctions.textparser.PopulateDynamicTags(component.templateTexts[2].text, false);
		}
		else if (this.campaignmanager.playerInPort)
		{
			string randomTextLineFromFile = UIFunctions.globaluifunctions.textparser.GetRandomTextLineFromFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/patrol_outcomes"));
			string[] array = randomTextLineFromFile.Split(new char[]
			{
				'|'
			});
			float patrolPoints = this.campaignmanager.playercampaigndata.patrolPoints;
			int num2 = 2;
			if (patrolPoints <= -50f)
			{
				num2 = 0;
			}
			else if (patrolPoints <= -20f)
			{
				num2 = 1;
			}
			else if (patrolPoints >= 20f)
			{
				num2 = 3;
			}
			else if (patrolPoints >= 50f)
			{
				num2 = 4;
			}
			component.templateTexts[2].text = array[num2];
			component.templateTexts[2].text = UIFunctions.globaluifunctions.textparser.PopulateDynamicTags(component.templateTexts[2].text, false);
		}
		if (this.courtMartial)
		{
			component.templateTexts[2].text = UIFunctions.globaluifunctions.textparser.GetRandomTextLineFromFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/patrol_outcome_end"));
			component.templateTexts[2].text = UIFunctions.globaluifunctions.textparser.PopulateDynamicTags(component.templateTexts[2].text, false);
		}
	}

	// Token: 0x0600079E RID: 1950 RVA: 0x00047770 File Offset: 0x00045970
	private bool GetIsCourtMartial()
	{
		return this.campaignmanager.playercampaigndata.neutralCasualties != null && (this.campaignmanager.playercampaigndata.neutralCasualties[2] > (float)(100000 - (int)GameDataManager.optionsFloatSettings[3] * 10000) || this.campaignmanager.playercampaigndata.neutralCasualties[1] + this.campaignmanager.playercampaigndata.neutralCasualties[0] > (float)(9 - (int)GameDataManager.optionsFloatSettings[3]));
	}

	// Token: 0x0600079F RID: 1951 RVA: 0x000477FC File Offset: 0x000459FC
	private void BuildMedalsCase(EventTemplate et)
	{
		GridLayoutGroup componentInChildren = et.GetComponentInChildren<GridLayoutGroup>();
		int num = this.patrolAwards.Length + this.cumulativeAwards.Length + this.woundedAwards.Length;
		if (num > 12)
		{
			componentInChildren.constraintCount = 5;
			if (num > 15)
			{
				componentInChildren.constraintCount = 6;
				componentInChildren.spacing = new Vector2(-20f, 0f);
			}
			if (num > 18)
			{
				componentInChildren.spacing = new Vector2(-20f, -20f);
			}
			if (num > 24)
			{
				componentInChildren.constraintCount = 8;
				componentInChildren.cellSize = new Vector2(120f, 120f);
				componentInChildren.spacing = new Vector2(-20f, 0f);
			}
		}
		this.ClearMedalsCase();
		this.medalsCase = new GameObject[num];
		string spritePath = string.Empty;
		string str = string.Empty;
		int num2 = 0;
		string filePathFromString = UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/events/template/medaldisplay");
		for (int i = 0; i < this.patrolAwards.Length; i++)
		{
			if (this.campaignmanager.playercampaigndata.patrolMedals[i] != 0)
			{
				str = this.patrolAwards[i].Replace("event_award_", string.Empty);
				spritePath = UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/images/awards/" + str);
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load(filePathFromString));
				gameObject.transform.SetParent(componentInChildren.gameObject.transform, false);
				gameObject.GetComponent<Image>().sprite = UIFunctions.globaluifunctions.textparser.GetSprite(spritePath);
				this.medalsCase[num2] = gameObject;
				num2++;
			}
		}
		for (int j = 0; j < this.cumulativeAwards.Length; j++)
		{
			if (this.campaignmanager.playercampaigndata.cumulativeMedals[j] != 0)
			{
				str = this.cumulativeAwards[j].Replace("event_award_", string.Empty);
				spritePath = UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/images/awards/" + str);
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load(filePathFromString));
				gameObject2.transform.SetParent(componentInChildren.gameObject.transform, false);
				gameObject2.GetComponent<Image>().sprite = UIFunctions.globaluifunctions.textparser.GetSprite(spritePath);
				this.medalsCase[num2] = gameObject2;
				num2++;
			}
		}
		for (int k = 0; k < this.woundedAwards.Length; k++)
		{
			if (this.campaignmanager.playercampaigndata.woundedMedals[k] != 0)
			{
				str = this.woundedAwards[k].Replace("event_award_", string.Empty);
				spritePath = UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/images/awards/" + str);
				GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load(filePathFromString));
				gameObject3.transform.SetParent(componentInChildren.gameObject.transform, false);
				gameObject3.GetComponent<Image>().sprite = UIFunctions.globaluifunctions.textparser.GetSprite(spritePath);
				this.medalsCase[num2] = gameObject3;
				num2++;
			}
		}
	}

	// Token: 0x060007A0 RID: 1952 RVA: 0x00047B30 File Offset: 0x00045D30
	private void ClearMedalsCase()
	{
		if (this.medalsCase != null)
		{
			for (int i = 0; i < this.medalsCase.Length; i++)
			{
				if (this.medalsCase[i] != null)
				{
					UnityEngine.Object.Destroy(this.medalsCase[i]);
				}
			}
		}
		this.medalsCase = null;
	}

	// Token: 0x060007A1 RID: 1953 RVA: 0x00047B88 File Offset: 0x00045D88
	private void CreateNewEventTemplate(bool award, bool summary)
	{
		int num = UnityEngine.Random.Range(1, 7);
		if (award)
		{
			num = 0;
		}
		string filePathFromString = UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/events/template/eventtemplate" + num.ToString());
		if (summary)
		{
			filePathFromString = UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/events/template/statstemplate");
		}
		this.DestroyCurrentEventTemplate();
		this.currentEventTemplate = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load(filePathFromString));
		this.currentEventTemplate.transform.SetParent(this.eventtemplatetransform, false);
		this.currentEventTemplate.transform.localPosition = Vector3.zero;
		this.GetEventText(this.campaignevents[this.currentEvent].eventFilename, award, summary);
	}

	// Token: 0x060007A2 RID: 1954 RVA: 0x00047C44 File Offset: 0x00045E44
	private void GetEventText(string filename, bool award, bool summary)
	{
		EventTemplate component = this.currentEventTemplate.GetComponent<EventTemplate>();
		int num = Mathf.FloorToInt(this.campaignmanager.hoursDurationOfCampaign / 24f);
		string text = CalendarFunctions.FromJulian((long)(this.campaignmanager.julianStartDay + (float)num), "d/MM/yyyy");
		string[] array = text.Split(new char[]
		{
			'/'
		});
		int month = int.Parse(array[1]);
		this.currentFullDate = string.Concat(new string[]
		{
			array[0],
			" ",
			CalendarFunctions.GetFullMonth(month, false, false),
			" ",
			array[2]
		});
		string filePathFromString = UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/" + filename);
		string[] array2 = UIFunctions.globaluifunctions.textparser.OpenTextDataFile(filePathFromString);
		int labelFont = 0;
		Color32 labelColor = Color.gray;
		Color32 outlineColor = Color.gray;
		bool labelOutline = false;
		component.templateTexts[0].text = string.Empty;
		component.templateTexts[1].text = string.Empty;
		component.templateTexts[2].text = string.Empty;
		bool flag = false;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = array2[i].Replace("<size=", "<size");
			array2[i] = array2[i].Replace("<color=", "<color");
			string[] array3 = array2[i].Split(new char[]
			{
				'='
			});
			string text2 = array3[0];
			switch (text2)
			{
			case "Image":
			{
				string[] array4 = UIFunctions.globaluifunctions.textparser.PopulateStringArray(array3[1].Trim());
				int num3 = UnityEngine.Random.Range(0, array4.Length);
				if (this.eventDebugMode)
				{
					if (this.DebugImageNumber >= array4.Length - 1)
					{
						this.DebugImageNumber = array4.Length - 1;
						flag = true;
					}
					num3 = this.DebugImageNumber;
					this.DebugText.text = string.Concat(new object[]
					{
						"Debug Mode Enabled: config.txt EventsDebugOnContinue=TRUE\nEvent #",
						this.currentEvent,
						"  File: ",
						this.campaignevents[this.currentEvent].eventFilename,
						"\n",
						array4[num3],
						"\n"
					});
				}
				UIFunctions.globaluifunctions.textparser.SetImageSprite(array4[num3], this.eventImage);
				break;
			}
			case "Photo":
				if (component.templateImages.Length > 1)
				{
					if (array3[1].Trim() != "FALSE")
					{
						string[] array5 = array3[1].Split(new char[]
						{
							','
						});
						string text3 = array5[UnityEngine.Random.Range(0, array5.Length)];
						if (this.eventDebugMode)
						{
							if (this.DebugPhotoNumber >= array5.Length - 1)
							{
								this.DebugPhotoNumber = array5.Length - 1;
								if (flag)
								{
									this.DebugMoveOn = true;
								}
							}
							text3 = array5[this.DebugPhotoNumber];
							Text debugText = this.DebugText;
							debugText.text += array5[this.DebugPhotoNumber];
						}
						UIFunctions.globaluifunctions.textparser.SetImageSprite(text3.Trim(), component.templateImages[1]);
						if (!award)
						{
							component.templateImages[1].rectTransform.sizeDelta = new Vector2(component.templateImages[1].rectTransform.sizeDelta.x, component.templateImages[1].rectTransform.sizeDelta.x * 0.67f);
						}
					}
					else
					{
						component.templateImages[1].gameObject.SetActive(false);
						if (this.eventDebugMode && flag)
						{
							this.DebugMoveOn = true;
						}
					}
				}
				break;
			case "Music":
				if (array3[1].Trim() != "FALSE")
				{
					string filePathFromString2 = UIFunctions.globaluifunctions.textparser.GetFilePathFromString(array3[1].Trim());
					AudioManager.audiomanager.PlayMusicClip(-1, filePathFromString2);
				}
				else
				{
					AudioManager.audiomanager.StopMusic();
				}
				break;
			case "Outline":
				labelOutline = (array3[1].Trim() == "TRUE");
				break;
			case "Font":
				labelFont = int.Parse(array3[1].Trim());
				break;
			case "Color":
				labelColor = UIFunctions.globaluifunctions.textparser.GetColor32(array3[1].Trim());
				break;
			case "OutlineColor":
				outlineColor = UIFunctions.globaluifunctions.textparser.GetColor32(array3[1].Trim());
				break;
			case "Title":
			{
				if (!(array3[1].Trim() != "BLANK"))
				{
					component.gameObject.SetActive(false);
					UIFunctions.globaluifunctions.campaignmanager.eventManager.continueButton.SetActive(true);
					return;
				}
				string[] array6 = array3[1].Trim().Split(new char[]
				{
					'|'
				});
				string eventContent = array6[UnityEngine.Random.Range(0, array6.Length)];
				if (!award)
				{
					this.BuildEventText(component.templateTexts[0], eventContent, labelColor, labelFont, labelOutline, outlineColor, true);
				}
				else
				{
					this.BuildEventText(component.templateTexts[0], eventContent, labelColor, labelFont, labelOutline, outlineColor, false);
				}
				break;
			}
			case "Sentence1":
			{
				string[] array7 = array3[1].Trim().Split(new char[]
				{
					'|'
				});
				string eventContent2 = array7[UnityEngine.Random.Range(0, array7.Length)];
				if (!award)
				{
					this.BuildEventText(component.templateTexts[1], eventContent2, labelColor, labelFont, labelOutline, outlineColor, false);
				}
				else
				{
					this.BuildEventText(component.templateTexts[1], eventContent2, labelColor, labelFont, labelOutline, outlineColor, true);
				}
				break;
			}
			case "Sentence2":
			{
				string[] array8 = array3[1].Trim().Split(new char[]
				{
					'|'
				});
				string eventContent3 = array8[UnityEngine.Random.Range(0, array8.Length)];
				this.BuildEventText(component.templateTexts[2], eventContent3, labelColor, labelFont, labelOutline, outlineColor, false);
				break;
			}
			case "Header":
				if (array3[1].Trim() == "FALSE")
				{
					component.templateTexts[3].text = string.Empty;
				}
				else if (array3[1].Trim() == "<DATE>")
				{
					component.templateTexts[3].text = this.currentFullDate;
				}
				else if (!summary && !award)
				{
					this.BuildEventText(component.templateTexts[2], array3[1].Trim(), labelColor, labelFont, labelOutline, outlineColor, false);
				}
				else
				{
					component.templateTexts[3].text = array3[1].Trim();
					component.templateTexts[3].text = component.templateTexts[3].text.Replace("<n>", "\n");
				}
				break;
			}
		}
	}

	// Token: 0x060007A3 RID: 1955 RVA: 0x00048428 File Offset: 0x00046628
	private void BuildEventText(Text eventText, string eventContent, Color32 labelColor, int labelFont, bool labelOutline, Color32 outlineColor, bool forceUpper)
	{
		eventContent = eventContent.Replace("<size", "<size=");
		eventContent = eventContent.Replace("<color", "<color=");
		eventContent = UIFunctions.globaluifunctions.textparser.PopulateDynamicTags(eventContent, false);
		if (forceUpper)
		{
			eventContent = eventContent.ToUpper();
		}
		eventText.text = eventContent;
		eventText.font = this.eventFonts[labelFont];
		eventText.color = labelColor;
		if (eventText.name == "dark")
		{
			eventText.color = new Color32(57, 61, 73, byte.MaxValue);
		}
		else if (eventText.name == "light")
		{
			eventText.color = new Color32(218, 218, 208, byte.MaxValue);
		}
		Outline component = eventText.GetComponent<Outline>();
		if (!labelOutline)
		{
			component.enabled = false;
		}
		else
		{
			component.enabled = true;
			component.effectColor = outlineColor;
		}
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x0004853C File Offset: 0x0004673C
	private void RefreshWarBar()
	{
		int num = Mathf.RoundToInt(this.campaignmanager.campaignPoints / this.campaignmanager.totalCampaignPoints * (float)this.campaignmanager.warbar.Length);
		for (int i = 0; i < UIFunctions.globaluifunctions.campaignmanager.warbar.Length; i++)
		{
			if (i <= num)
			{
				UIFunctions.globaluifunctions.campaignmanager.warbar[i].color = UIFunctions.globaluifunctions.campaignmanager.warbarColors[0];
			}
			else
			{
				UIFunctions.globaluifunctions.campaignmanager.warbar[i].color = UIFunctions.globaluifunctions.campaignmanager.warbarColors[1];
			}
		}
	}

	// Token: 0x060007A5 RID: 1957 RVA: 0x00048604 File Offset: 0x00046804
	private void DestroyCurrentEventTemplate()
	{
		if (this.currentEventTemplate != null)
		{
			UnityEngine.Object.Destroy(this.currentEventTemplate);
		}
	}

	// Token: 0x060007A6 RID: 1958 RVA: 0x00048624 File Offset: 0x00046824
	public void ContinueButton()
	{
		if (this.eventDebugMode)
		{
			if (this.currentEvent == this.campaignevents.Length - 1)
			{
				return;
			}
			this.DebugImageNumber++;
			this.DebugPhotoNumber++;
			if (this.DebugMoveOn)
			{
				this.currentEvent++;
				this.DebugImageNumber = 0;
				this.DebugPhotoNumber = 0;
				this.DebugMoveOn = false;
			}
			this.BringInEvent(this.currentEvent, false, false);
			return;
		}
		else
		{
			base.gameObject.SetActive(false);
			if (!UIFunctions.globaluifunctions.campaignmanager.strategicMapToolbar.activeSelf)
			{
				UIFunctions.globaluifunctions.campaignmanager.strategicMapToolbar.SetActive(true);
			}
			if (this.campaignOver)
			{
				Time.timeScale = 1f;
				UIFunctions.globaluifunctions.ClearDefaultTextBox();
				UIFunctions.globaluifunctions.OpenCredits();
				return;
			}
			this.ClearMedalsCase();
			this.DestroyCurrentEventTemplate();
			this.campaignmanager.enabled = true;
			if (this.currentEvent >= 0)
			{
				if (this.campaignevents[this.currentEvent].nextAction == "END")
				{
					this.campaignOver = true;
					this.BringInCampaignSummary();
					return;
				}
				if (this.campaignevents[this.currentEvent].nextAction == "ASSIGN_NEW")
				{
					AudioManager.audiomanager.PlayMusicClip(1, string.Empty);
					this.campaignmanager.playerGameObject.transform.localPosition = new Vector3(this.campaignmanager.playerBasePosition.x, this.campaignmanager.playerBasePosition.y, 0f);
					this.campaignmanager.playerInPort = true;
					UIFunctions.globaluifunctions.missionmanager.SelectShip();
					UIFunctions.globaluifunctions.backgroundImagesOnly.SetActive(true);
					return;
				}
				if (this.campaignevents[this.currentEvent].nextAction == "CHECK_MISSION" && UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.playerMissionType == "RETURN_TO_BASE")
				{
					UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.playerHasMission = false;
					UIFunctions.globaluifunctions.campaignmanager.GenerateCampaignMission(true);
					return;
				}
			}
			if (this.nextEvent >= 0)
			{
				this.BringInEvent(this.nextEvent, false, false);
				return;
			}
			if (this.campaignmanager.currentTaskForceEngagedWith == this.campaignmanager.playercampaigndata.currentMissionTaskForceID)
			{
				this.campaignmanager.EndCampaignCombat();
				return;
			}
			if (!UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.playerHasMission)
			{
				UIFunctions.globaluifunctions.campaignmanager.GenerateCampaignMission(true);
				return;
			}
			bool flag = false;
			if (GameDataManager.playervesselsonlevel == null)
			{
				flag = true;
			}
			else if (GameDataManager.playervesselsonlevel[0] == null)
			{
				flag = true;
			}
			if (flag)
			{
				CampaignManager.isEncounter = false;
				UIFunctions.globaluifunctions.campaignmanager.OpenMissionBriefing(0);
			}
			if (!this.campaignmanager.playerInPort)
			{
				UIFunctions.globaluifunctions.SetMenuSystem("CAMPAIGN");
				UIFunctions.globaluifunctions.backgroundTemplate.SetActive(false);
			}
			else
			{
				UIFunctions.globaluifunctions.campaignmanager.OpenPassiveBriefingMenu();
				if (UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.playerMissionType == "RETURN_TO_BASE")
				{
					UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.playerHasMission = false;
					UIFunctions.globaluifunctions.campaignmanager.GenerateCampaignMission(true);
				}
			}
			AudioManager.audiomanager.PlayMusicClip(1, string.Empty);
			Time.timeScale = UIFunctions.globaluifunctions.campaignmanager.mapTimeCompression;
			this.currentEvent = -1;
			return;
		}
	}

	// Token: 0x060007A7 RID: 1959 RVA: 0x000489D4 File Offset: 0x00046BD4
	public string GetDynamicWarContent(string path)
	{
		string randomTextLineFromFile = UIFunctions.globaluifunctions.textparser.GetRandomTextLineFromFile(UIFunctions.globaluifunctions.textparser.GetFilePathFromString(path));
		string[] array = randomTextLineFromFile.Split(new char[]
		{
			'|'
		});
		if (array.Length != 3)
		{
		}
		return array[this.GetContentIndex()];
	}

	// Token: 0x060007A8 RID: 1960 RVA: 0x00048A24 File Offset: 0x00046C24
	public string GetDynamicWarTakeOverContent(string tag)
	{
		tag = tag.TrimStart(new char[]
		{
			'<'
		});
		tag = tag.TrimEnd(new char[]
		{
			'>'
		});
		string b = tag + this.currentTakeOverStatus;
		string filePathFromString = UIFunctions.globaluifunctions.textparser.GetFilePathFromString("campaign/" + CampaignManager.campaignReferenceName + "/language/events/content/takeover");
		string[] array = UIFunctions.globaluifunctions.textparser.OpenTextDataFile(filePathFromString);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'='
			});
			if (array2[0].Trim() == b)
			{
				string[] array3 = array2[1].Trim().Split(new char[]
				{
					'|'
				});
				return array3[UnityEngine.Random.Range(0, array3.Length)];
			}
		}
		return null;
	}

	// Token: 0x060007A9 RID: 1961 RVA: 0x00048B00 File Offset: 0x00046D00
	private int GetContentIndex()
	{
		int result = 1;
		float num = this.campaignmanager.campaignPoints / this.campaignmanager.totalCampaignPoints;
		if (num < 0.3f)
		{
			result = 0;
		}
		else if (num > 0.6f)
		{
			result = 2;
		}
		return result;
	}

	// Token: 0x060007AA RID: 1962 RVA: 0x00048B48 File Offset: 0x00046D48
	public int GetEventID(string eventString)
	{
		for (int i = 0; i < this.campaignevents.Length; i++)
		{
			if (eventString == this.campaignevents[i].eventFilename)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x04000A9A RID: 2714
	public CampaignManager campaignmanager;

	// Token: 0x04000A9B RID: 2715
	public CampaignEvent[] campaignevents;

	// Token: 0x04000A9C RID: 2716
	public bool eventDebugMode;

	// Token: 0x04000A9D RID: 2717
	public int DebugImageNumber;

	// Token: 0x04000A9E RID: 2718
	public int DebugPhotoNumber;

	// Token: 0x04000A9F RID: 2719
	public bool DebugMoveOn;

	// Token: 0x04000AA0 RID: 2720
	public Text DebugText;

	// Token: 0x04000AA1 RID: 2721
	public GameObject continueButton;

	// Token: 0x04000AA2 RID: 2722
	public Transform eventtemplatetransform;

	// Token: 0x04000AA3 RID: 2723
	public int nextEvent;

	// Token: 0x04000AA4 RID: 2724
	public int currentEvent;

	// Token: 0x04000AA5 RID: 2725
	public bool missionPassed;

	// Token: 0x04000AA6 RID: 2726
	public bool missionMissed;

	// Token: 0x04000AA7 RID: 2727
	public bool playerDead;

	// Token: 0x04000AA8 RID: 2728
	public bool playerWounded;

	// Token: 0x04000AA9 RID: 2729
	public bool sealsReleased;

	// Token: 0x04000AAA RID: 2730
	public bool medalAwarded;

	// Token: 0x04000AAB RID: 2731
	public bool campaignOver;

	// Token: 0x04000AAC RID: 2732
	public bool courtMartial;

	// Token: 0x04000AAD RID: 2733
	public Image eventImage;

	// Token: 0x04000AAE RID: 2734
	public GameObject currentEventTemplate;

	// Token: 0x04000AAF RID: 2735
	public Font[] eventFonts;

	// Token: 0x04000AB0 RID: 2736
	public int[] specialEventIDs;

	// Token: 0x04000AB1 RID: 2737
	public string[] patrolAwards;

	// Token: 0x04000AB2 RID: 2738
	public int[] patrolTonnage;

	// Token: 0x04000AB3 RID: 2739
	public string[] cumulativeAwards;

	// Token: 0x04000AB4 RID: 2740
	public int[] cumulativeTonnage;

	// Token: 0x04000AB5 RID: 2741
	public int[] missionsPassed;

	// Token: 0x04000AB6 RID: 2742
	public string[] woundedAwards;

	// Token: 0x04000AB7 RID: 2743
	public float[] probabilityWounded;

	// Token: 0x04000AB8 RID: 2744
	public bool missionOverBriefingDisplayed;

	// Token: 0x04000AB9 RID: 2745
	public string playerMapRegion;

	// Token: 0x04000ABA RID: 2746
	public bool playerMapRegionIsLocation;

	// Token: 0x04000ABB RID: 2747
	public string currentFullDate;

	// Token: 0x04000ABC RID: 2748
	public int lastZoneModified;

	// Token: 0x04000ABD RID: 2749
	public string impassePath;

	// Token: 0x04000ABE RID: 2750
	public string warStatus1Path;

	// Token: 0x04000ABF RID: 2751
	public string warStatus2Path;

	// Token: 0x04000AC0 RID: 2752
	public string currentTakeOverStatus;

	// Token: 0x04000AC1 RID: 2753
	public string[] friendly;

	// Token: 0x04000AC2 RID: 2754
	public string[] enemy;

	// Token: 0x04000AC3 RID: 2755
	public string[] friendlySingular;

	// Token: 0x04000AC4 RID: 2756
	public string[] enemySingular;

	// Token: 0x04000AC5 RID: 2757
	public string[] friendlyPrefix;

	// Token: 0x04000AC6 RID: 2758
	public string[] enemyPrefix;

	// Token: 0x04000AC7 RID: 2759
	public string[] friendlySuffix;

	// Token: 0x04000AC8 RID: 2760
	public string[] enemySuffix;

	// Token: 0x04000AC9 RID: 2761
	public GameObject[] medalsCase;

	// Token: 0x04000ACA RID: 2762
	public int endingType;

	// Token: 0x04000ACB RID: 2763
	public float forwardDate;
}
