using System;
using UnityEngine;

// Token: 0x02000122 RID: 290
public class GameDataManager : MonoBehaviour
{
	// Token: 0x060007BB RID: 1979 RVA: 0x00049008 File Offset: 0x00047208
	public void Start()
	{
		GameDataManager.manualcamerazoom = UIFunctions.globaluifunctions.MainCamera.GetComponent<ManualCameraZoom>();
		GameDataManager.globalTranslationSpeed = this.globalSpeedModifier * 0.07273f;
		GameDataManager.yardsScale = 75.13f * this.worldYardsScale;
		GameDataManager.inverseYardsScale = 1f / GameDataManager.yardsScale;
		GameDataManager.unitsToFeet = 225.39f;
		GameDataManager.feetToUnits = 1f / GameDataManager.unitsToFeet;
	}

	// Token: 0x04000AE6 RID: 2790
	public float globalSpeedModifier;

	// Token: 0x04000AE7 RID: 2791
	public static float globalTranslationSpeed;

	// Token: 0x04000AE8 RID: 2792
	public static float cameraTimeScale;

	// Token: 0x04000AE9 RID: 2793
	public float worldYardsScale;

	// Token: 0x04000AEA RID: 2794
	public static float yardsScale;

	// Token: 0x04000AEB RID: 2795
	public static float inverseYardsScale;

	// Token: 0x04000AEC RID: 2796
	public static float unitsToFeet;

	// Token: 0x04000AED RID: 2797
	public static float feetToUnits;

	// Token: 0x04000AEE RID: 2798
	public static float yardsToMetres = 1.0936f;

	// Token: 0x04000AEF RID: 2799
	public static float feetToMetres = 3.2808f;

	// Token: 0x04000AF0 RID: 2800
	public static float mainCameraFOV = 30f;

	// Token: 0x04000AF1 RID: 2801
	public static float menuScrollListSpacing = 38f;

	// Token: 0x04000AF2 RID: 2802
	public static bool loadVideo = true;

	// Token: 0x04000AF3 RID: 2803
	public int[] biomeTreeDensity;

	// Token: 0x04000AF4 RID: 2804
	public static int maxShips = 15;

	// Token: 0x04000AF5 RID: 2805
	public static Vessel[] playervesselsonlevel;

	// Token: 0x04000AF6 RID: 2806
	public static Vessel[] enemyvesselsonlevel;

	// Token: 0x04000AF7 RID: 2807
	public static bool missionMode;

	// Token: 0x04000AF8 RID: 2808
	public static bool trainingMode;

	// Token: 0x04000AF9 RID: 2809
	public static bool campaignMode;

	// Token: 0x04000AFA RID: 2810
	public static int CurrentAction;

	// Token: 0x04000AFB RID: 2811
	public static int enemyNumberofShips;

	// Token: 0x04000AFC RID: 2812
	public static int playerNumberofShips;

	// Token: 0x04000AFD RID: 2813
	public static float windX;

	// Token: 0x04000AFE RID: 2814
	public static float windY;

	// Token: 0x04000AFF RID: 2815
	public static float windZ;

	// Token: 0x04000B00 RID: 2816
	public static bool isNight;

	// Token: 0x04000B01 RID: 2817
	public static bool HUDActive;

	// Token: 0x04000B02 RID: 2818
	public static int weathertype;

	// Token: 0x04000B03 RID: 2819
	public static int enemysunk;

	// Token: 0x04000B04 RID: 2820
	public static int playersunk;

	// Token: 0x04000B05 RID: 2821
	public static string playerCommanderName;

	// Token: 0x04000B06 RID: 2822
	public static bool[] optionsBoolSettings;

	// Token: 0x04000B07 RID: 2823
	public static float[] optionsFloatSettings;

	// Token: 0x04000B08 RID: 2824
	public static float currentvolume;

	// Token: 0x04000B09 RID: 2825
	public static float currentmusicvolume;

	// Token: 0x04000B0A RID: 2826
	public static float camerasensitivity;

	// Token: 0x04000B0B RID: 2827
	public static int graphicsdetail;

	// Token: 0x04000B0C RID: 2828
	public static string gameVersion = "1.15g";

	// Token: 0x04000B0D RID: 2829
	public static string saveGameFolder = "savegame";

	// Token: 0x04000B0E RID: 2830
	public static string saveGameFileExtension = ".sav";

	// Token: 0x04000B0F RID: 2831
	public static string language = "_en";

	// Token: 0x04000B10 RID: 2832
	public static int[] screenResolution;

	// Token: 0x04000B11 RID: 2833
	public static float xclock;

	// Token: 0x04000B12 RID: 2834
	public static float yclock;

	// Token: 0x04000B13 RID: 2835
	public static ManualCameraZoom manualcamerazoom;

	// Token: 0x04000B14 RID: 2836
	public static float smokeAngle;

	// Token: 0x04000B15 RID: 2837
	public static int activePlayerSlot;

	// Token: 0x04000B16 RID: 2838
	public AnimationCurve accelerationCurve;

	// Token: 0x04000B17 RID: 2839
	public AnimationCurve decelerationCurve;
}
