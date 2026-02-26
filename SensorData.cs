using System;
using UnityEngine;

// Token: 0x02000154 RID: 340
public class SensorData : MonoBehaviour
{
	// Token: 0x04000F06 RID: 3846
	public bool playerDetected;

	// Token: 0x04000F07 RID: 3847
	public float timeTrackingPlayer;

	// Token: 0x04000F08 RID: 3848
	public float decibelsTotalDetected;

	// Token: 0x04000F09 RID: 3849
	public float radarTotalDetected;

	// Token: 0x04000F0A RID: 3850
	public float decibelsLastDetected;

	// Token: 0x04000F0B RID: 3851
	public float bearingLastDetected;

	// Token: 0x04000F0C RID: 3852
	public float rangeYardsLastDetected;

	// Token: 0x04000F0D RID: 3853
	public float detectingDecoy;

	// Token: 0x04000F0E RID: 3854
	public Vector3 lastKnownTargetPosition;

	// Token: 0x04000F0F RID: 3855
	public bool usingPassiveSonar;

	// Token: 0x04000F10 RID: 3856
	public bool usingActiveSonar;

	// Token: 0x04000F11 RID: 3857
	public bool usingTowedArray;

	// Token: 0x04000F12 RID: 3858
	public bool lastDipDetected;

	// Token: 0x04000F13 RID: 3859
	public Noisemaker[] sonobuoys;

	// Token: 0x04000F14 RID: 3860
	public int[] sonobuoySonarIDs;

	// Token: 0x04000F15 RID: 3861
	public Vector2[] unitWaypoints;

	// Token: 0x04000F16 RID: 3862
	public float[] playerSignatureData = new float[3];

	// Token: 0x04000F17 RID: 3863
	public float[] contactSignatureData = new float[3];
}
