using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000F9 RID: 249
public class CampaignMapRevealContact : MonoBehaviour
{
	// Token: 0x060006C0 RID: 1728 RVA: 0x000379AC File Offset: 0x00035BAC
	public void FixedUpdate()
	{
		if (!UIFunctions.globaluifunctions.campaignmanager.enabled)
		{
			return;
		}
		this.timer += Time.deltaTime;
		if (this.iconImage.gameObject.activeSelf)
		{
			if (this.timer < 3f)
			{
				this.iconImage.color = UIFunctions.globaluifunctions.campaignmanager.contactColorsOverTime[0];
			}
			else if (this.timer < 6f)
			{
				this.iconImage.color = UIFunctions.globaluifunctions.campaignmanager.contactColorsOverTime[1];
				if (this.iconImage.transform.parent == base.transform)
				{
					this.lastKnownPosition = new Vector2(base.transform.position.x, base.transform.position.y);
					this.iconImage.transform.SetParent(UIFunctions.globaluifunctions.campaignmanager.shipLayer, true);
				}
			}
			else if (this.timer < 9f)
			{
				this.iconImage.color = UIFunctions.globaluifunctions.campaignmanager.contactColorsOverTime[2];
			}
			else if (this.timer < 12f)
			{
				this.iconImage.color = UIFunctions.globaluifunctions.campaignmanager.contactColorsOverTime[3];
			}
			else
			{
				this.iconImage.gameObject.SetActive(false);
				base.enabled = false;
			}
		}
	}

	// Token: 0x060006C1 RID: 1729 RVA: 0x00037B68 File Offset: 0x00035D68
	private void OnTriggerEnter(Collider otherObject)
	{
		if (base.name == "PLAYER_PORT" && otherObject.name == "PLAYER_SUBMARINE")
		{
			if (!UIFunctions.globaluifunctions.campaignmanager.playerInPort)
			{
				UIFunctions.globaluifunctions.campaignmanager.playerInPort = true;
				UIFunctions.globaluifunctions.campaignmanager.accumulatedTimeInPort = 0f;
				UIFunctions.globaluifunctions.campaignmanager.timeInPort = UIFunctions.globaluifunctions.campaignmanager.timePenaltyOnPortEnter * OptionsManager.difficultySettings["TimeInPortModifier"];
				UIFunctions.globaluifunctions.campaignmanager.timeInPort += GameDataManager.playervesselsonlevel[0].damagesystem.shipCurrentDamagePoints / GameDataManager.playervesselsonlevel[0].damagesystem.shipTotalDamagePoints * UIFunctions.globaluifunctions.campaignmanager.timePenaltyHullDamageMultiplier * OptionsManager.difficultySettings["RepairTimeModifier"];
				UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.playerNoisemakersOnBoard = UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].numberofnoisemakers;
				GameDataManager.playervesselsonlevel[0].vesselmovement.weaponSource.noisemakersOnBoard = UIFunctions.globaluifunctions.database.databaseshipdata[UIFunctions.globaluifunctions.playerfunctions.playerVesselClass].numberofnoisemakers;
				GameDataManager.playervesselsonlevel[0].damagesystem.shipCurrentDamagePoints = 0f;
				UIFunctions.globaluifunctions.campaignmanager.playercampaigndata.vesselTotalDamage = 0f;
				GameDataManager.playervesselsonlevel[0].damagesystem.shipCurrentDamagePoints = 0f;
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentCurrentFlooding = new float[5];
				UIFunctions.globaluifunctions.playerfunctions.damagecontrol.compartmentTotalFlooding = new float[5];
				UIFunctions.globaluifunctions.campaignmanager.PlayerEnterPort();
			}
			return;
		}
		if (otherObject.name == "FRIENDLY" && base.name != "PLAYER_SUBMARINE")
		{
			this.RevealContact();
		}
		else if (otherObject.name == "ENEMY" && base.name == "PLAYER_SUBMARINE")
		{
			UIFunctions.globaluifunctions.campaignmanager.RevealPlayerOnMap();
		}
	}

	// Token: 0x060006C2 RID: 1730 RVA: 0x00037DC8 File Offset: 0x00035FC8
	public void RevealContact()
	{
		this.timer = 0f;
		this.iconImage.transform.SetParent(base.transform, false);
		this.iconImage.transform.localPosition = Vector3.zero;
		this.iconImage.gameObject.SetActive(true);
		base.enabled = true;
	}

	// Token: 0x040007FF RID: 2047
	public float timer;

	// Token: 0x04000800 RID: 2048
	public Image iconImage;

	// Token: 0x04000801 RID: 2049
	public Vector2 lastKnownPosition;
}
