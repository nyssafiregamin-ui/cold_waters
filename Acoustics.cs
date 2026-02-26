using System;
using UnityEngine;

// Token: 0x020000E8 RID: 232
public class Acoustics : MonoBehaviour
{
	// Token: 0x04000669 RID: 1641
	public Transform sensorNavigator;

	// Token: 0x0400066A RID: 1642
	public bool usingActiveSonar;

	// Token: 0x0400066B RID: 1643
	public SphereCollider noiseRadius;

	// Token: 0x0400066C RID: 1644
	public float currentOwnNoise;

	// Token: 0x0400066D RID: 1645
	public float currentNoise;

	// Token: 0x0400066E RID: 1646
	public bool currentlyAboveLayer;

	// Token: 0x0400066F RID: 1647
	public bool masked;

	// Token: 0x04000670 RID: 1648
	public bool[] playerHasDetectedWith;

	// Token: 0x04000671 RID: 1649
	public bool isMaster;
}
