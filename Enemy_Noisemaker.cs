using System;
using UnityEngine;

// Token: 0x02000116 RID: 278
public class Enemy_Noisemaker : MonoBehaviour
{
	// Token: 0x06000781 RID: 1921 RVA: 0x00044E08 File Offset: 0x00043008
	public void InitialiseEnemyNoisemaker()
	{
		this.noisemakersOnBoard = this.parentVesselMovement.parentVessel.databaseshipdata.numberofnoisemakers;
		this.noisemakerReloadTime = this.parentVesselMovement.parentVessel.databaseshipdata.noisemakerreloadtime;
	}

	// Token: 0x06000782 RID: 1922 RVA: 0x00044E4C File Offset: 0x0004304C
	private void FixedUpdate()
	{
		if (this.noisemakerReloading)
		{
			this.noisemakerReloadingTimer += Time.deltaTime;
			if (this.noisemakerReloadingTimer > this.noisemakerReloadTime)
			{
				this.noisemakerReloadingTimer = 0f;
				this.noisemakerReloading = false;
			}
		}
		else
		{
			base.enabled = false;
		}
	}

	// Token: 0x06000783 RID: 1923 RVA: 0x00044EA8 File Offset: 0x000430A8
	public void DropNoisemaker()
	{
		if (this.noisemakersOnBoard > 0 && !this.noisemakerReloading)
		{
			this.noisemakersOnBoard--;
			this.noisemakerReloading = true;
			Noisemaker component = ObjectPoolManager.CreatePooled(UIFunctions.globaluifunctions.database.databasecountermeasuredata[this.parentVesselMovement.parentVessel.databaseshipdata.noiseMakerID].countermeasureObject, this.noisemakerTubes.position, this.noisemakerTubes.rotation).GetComponent<Noisemaker>();
			component.gameObject.SetActive(true);
			component.databasecountermeasuredata = UIFunctions.globaluifunctions.database.databasecountermeasuredata[this.parentVesselMovement.parentVessel.databaseshipdata.noiseMakerID];
			UIFunctions.globaluifunctions.playerfunctions.sensormanager.AddNoiseMakerToArray(component);
			component.tacMapNoisemakerIcon.shipDisplayIcon.color = UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.navyColors[1];
			base.enabled = true;
			GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.LookAt(base.transform.position);
			string text = LanguageManager.GetDictionaryString(LanguageManager.messageLogDictionary, "NoisemakerDropped");
			text = text.Replace("<BRG>", string.Format("{0:0}", GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.eulerAngles.y));
			UIFunctions.globaluifunctions.playerfunctions.voicemanager.currentNoisemakerBearing = GameDataManager.playervesselsonlevel[0].acoustics.sensorNavigator.transform.eulerAngles.y.ToString("000");
			UIFunctions.globaluifunctions.playerfunctions.PlayerMessage(text, UIFunctions.globaluifunctions.playerfunctions.messageLogColors["Sonar"], "NoisemakerDropped", false);
		}
	}

	// Token: 0x04000A54 RID: 2644
	public VesselMovement parentVesselMovement;

	// Token: 0x04000A55 RID: 2645
	public int noisemakersOnBoard;

	// Token: 0x04000A56 RID: 2646
	public bool noisemakerReloading;

	// Token: 0x04000A57 RID: 2647
	public Transform noisemakerTubes;

	// Token: 0x04000A58 RID: 2648
	public float noisemakerReloadTime;

	// Token: 0x04000A59 RID: 2649
	public float noisemakerReloadingTimer;
}
