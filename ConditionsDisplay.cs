using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000FE RID: 254
public class ConditionsDisplay : MonoBehaviour
{
	// Token: 0x060006F6 RID: 1782 RVA: 0x0003C6F8 File Offset: 0x0003A8F8
	public void InitialiseConditions()
	{
		this.depthGaugeMultiplier = 1;
		this.conditionsLabels[0].text = string.Concat(new string[]
		{
			LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "AmbientNoise"),
			" <b>",
			string.Format("{0:0}", this.sensormanager.currentOceanAmbientNoise),
			"</b> ",
			LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Decibels")
		});
		if (this.sensormanager.surfaceDuctStrength > 0f)
		{
			this.conditionsLabels[1].text = UIFunctions.globaluifunctions.missionmanager.GetStrengthDisplay(this.sensormanager.surfaceDuctStrength).ToUpper() + " " + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "SurfaceDuct");
		}
		else
		{
			this.conditionsLabels[1].text = string.Empty;
		}
		if (this.sensormanager.layerStrength > 0f)
		{
			Text text = this.conditionsLabels[1];
			string text2 = text.text;
			text.text = string.Concat(new string[]
			{
				text2,
				"\n",
				UIFunctions.globaluifunctions.missionmanager.GetStrengthDisplay(this.sensormanager.layerStrength).ToUpper(),
				" ",
				LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Layer")
			});
			this.conditionsLabels[2].text = string.Format("{0:0}", this.sensormanager.layerDepthInFeet) + " " + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Feet");
			this.layerLine.gameObject.SetActive(true);
		}
		else
		{
			this.layerLine.gameObject.SetActive(false);
			this.conditionsLabels[2].text = string.Empty;
		}
		this.SetLayerDisplay();
		this.playerVesselHullHeight = GameDataManager.playervesselsonlevel[0].databaseshipdata.hullHeight * GameDataManager.unitsToFeet;
	}

	// Token: 0x060006F7 RID: 1783 RVA: 0x0003C900 File Offset: 0x0003AB00
	private void SetLayerDisplay()
	{
		float displayDepth = this.GetDisplayDepth(this.sensormanager.layerDepthInFeet);
		this.layerLine.localPosition = new Vector3(this.layerLine.localPosition.x, displayDepth, this.layerLine.localPosition.z);
		this.SetConditionsDepthGauge();
	}

	// Token: 0x060006F8 RID: 1784 RVA: 0x0003C95C File Offset: 0x0003AB5C
	public void SetConditionsDepthGauge()
	{
		int num = 0;
		this.depthGauge.text = string.Empty;
		for (int i = 0; i < 11; i++)
		{
			Text text = this.depthGauge;
			text.text = text.text + num.ToString() + "\n";
			num += 100 * this.depthGaugeMultiplier;
		}
		this.depthGauge.text = this.depthGauge.text.Trim();
	}

	// Token: 0x060006F9 RID: 1785 RVA: 0x0003C9D8 File Offset: 0x0003ABD8
	private float GetDisplayDepth(float depthInFeet)
	{
		if (depthInFeet > (float)(1000 * this.depthGaugeMultiplier))
		{
			depthInFeet = (float)(1000 * this.depthGaugeMultiplier);
		}
		else if (depthInFeet < 0f)
		{
			depthInFeet = 0f;
		}
		return 201.8f - 0.15980001f * (depthInFeet / (float)this.depthGaugeMultiplier);
	}

	// Token: 0x060006FA RID: 1786 RVA: 0x0003CA38 File Offset: 0x0003AC38
	private void FixedUpdate()
	{
		this.timer += Time.deltaTime;
		if (this.timer > 0.5f)
		{
			this.UpdateConditionsDisplay(false);
			this.timer -= 0.5f;
		}
	}

	// Token: 0x060006FB RID: 1787 RVA: 0x0003CA78 File Offset: 0x0003AC78
	public void SetDepthMultiplier(int dir)
	{
		int num = this.depthGaugeMultiplier;
		if (dir == -1)
		{
			num--;
		}
		else
		{
			num++;
		}
		num = Mathf.Clamp(num, 1, 3);
		if (this.depthGaugeMultiplier != num)
		{
			this.depthGaugeMultiplier = num;
			this.UpdateConditionsDisplay(true);
		}
	}

	// Token: 0x060006FC RID: 1788 RVA: 0x0003CAC4 File Offset: 0x0003ACC4
	public void UpdateConditionsDisplay(bool updateScale = false)
	{
		if (UIFunctions.globaluifunctions.playerfunctions.currentOpenPanel != 0)
		{
			return;
		}
		if (updateScale)
		{
			this.SetLayerDisplay();
		}
		float displayDepth = this.GetDisplayDepth((float)this.sensormanager.playerfunctions.playerDepthInFeet);
		this.playerIcon.transform.localPosition = new Vector3(-375f, displayDepth, 0f);
		this.contactIcon.gameObject.SetActive(false);
		if (this.sensormanager.playerfunctions.currentTargetIndex != -1 && !GameDataManager.enemyvesselsonlevel[this.sensormanager.playerfunctions.currentTargetIndex].isSinking)
		{
			displayDepth = this.GetDisplayDepth((float)((int)Mathf.Round((GameDataManager.enemyvesselsonlevel[this.sensormanager.playerfunctions.currentTargetIndex].transform.position.y - 1000f) * -GameDataManager.unitsToFeet)));
			this.contactIcon.transform.localPosition = new Vector3(-85f, displayDepth, 0f);
			if (this.sensormanager.identifiedByPlayer[this.sensormanager.playerfunctions.currentTargetIndex] || this.sensormanager.classifiedByPlayer[this.sensormanager.playerfunctions.currentTargetIndex])
			{
				if (UIFunctions.globaluifunctions.database.databaseshipdata[this.sensormanager.classifiedByPlayerAsClass[this.sensormanager.playerfunctions.currentTargetIndex]].shipType == "SUBMARINE")
				{
					this.contactSprite.sprite = this.contactImage[2];
					this.contactIcon.gameObject.SetActive(true);
				}
				else if (UIFunctions.globaluifunctions.database.databaseshipdata[this.sensormanager.classifiedByPlayerAsClass[this.sensormanager.playerfunctions.currentTargetIndex]].shipType == "MERCHANT")
				{
					this.contactSprite.sprite = this.contactImage[3];
					this.contactIcon.gameObject.SetActive(true);
				}
				else if (UIFunctions.globaluifunctions.database.databaseshipdata[this.sensormanager.classifiedByPlayerAsClass[this.sensormanager.playerfunctions.currentTargetIndex]].shipType == "BIOLOGIC")
				{
					this.contactIcon.gameObject.SetActive(false);
				}
				else
				{
					this.contactSprite.sprite = this.contactImage[1];
					this.contactIcon.gameObject.SetActive(true);
				}
			}
		}
		float num = (float)(UIFunctions.globaluifunctions.playerfunctions.playerDepthInFeet + UIFunctions.globaluifunctions.playerfunctions.playerDepthUnderKeel) - this.playerVesselHullHeight;
		this.conditionsLabels[3].text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Floor") + " <b>";
		Text text = this.conditionsLabels[3];
		text.text = text.text + string.Format("{0:0}", num) + "</b> " + LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, "Feet");
		if (num <= (float)(1000 * this.depthGaugeMultiplier))
		{
			if (!this.floorLine.gameObject.activeSelf)
			{
				this.floorLine.gameObject.SetActive(true);
			}
			this.floorLine.localPosition = new Vector3(this.floorLine.localPosition.x, this.GetDisplayDepth(num), this.floorLine.localPosition.z);
		}
		else
		{
			this.floorLine.gameObject.SetActive(false);
		}
		if (this.sensormanager.layerStrength > 0f)
		{
			if (Mathf.Abs(this.sensormanager.layerDepthInFeet - num) < 250f || this.sensormanager.layerDepthInFeet > num)
			{
				this.layerLine.gameObject.SetActive(false);
			}
			else
			{
				this.layerLine.gameObject.SetActive(true);
			}
		}
		if (UIFunctions.globaluifunctions.playerfunctions.currentActiveTorpedo != null)
		{
			displayDepth = this.GetDisplayDepth((float)((int)Mathf.Round((UIFunctions.globaluifunctions.playerfunctions.currentActiveTorpedo.transform.position.y - 1000f) * -GameDataManager.unitsToFeet)));
			this.torpedoIcon.transform.localPosition = new Vector3(-154f, displayDepth, 0f);
			this.torpedoIcon.SetActive(true);
		}
		else
		{
			this.torpedoIcon.SetActive(false);
		}
	}

	// Token: 0x04000850 RID: 2128
	public SensorManager sensormanager;

	// Token: 0x04000851 RID: 2129
	public GameObject playerIcon;

	// Token: 0x04000852 RID: 2130
	public GameObject contactIcon;

	// Token: 0x04000853 RID: 2131
	public GameObject torpedoIcon;

	// Token: 0x04000854 RID: 2132
	public Image contactSprite;

	// Token: 0x04000855 RID: 2133
	public Sprite[] contactImage;

	// Token: 0x04000856 RID: 2134
	public float timer;

	// Token: 0x04000857 RID: 2135
	public Text[] conditionsLabels;

	// Token: 0x04000858 RID: 2136
	public float[] bathyPoints;

	// Token: 0x04000859 RID: 2137
	public LineRenderer[] bathyLines;

	// Token: 0x0400085A RID: 2138
	public RectTransform layerLine;

	// Token: 0x0400085B RID: 2139
	public RectTransform floorLine;

	// Token: 0x0400085C RID: 2140
	public float playerVesselHullHeight;

	// Token: 0x0400085D RID: 2141
	public int depthGaugeMultiplier;

	// Token: 0x0400085E RID: 2142
	public Text depthGauge;
}
