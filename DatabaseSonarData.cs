using System;
using UnityEngine;

// Token: 0x02000108 RID: 264
public class DatabaseSonarData : ScriptableObject
{
	// Token: 0x04000921 RID: 2337
	public int sonarID;

	// Token: 0x04000922 RID: 2338
	public string sonarModel;

	// Token: 0x04000923 RID: 2339
	public string SonarType;

	// Token: 0x04000924 RID: 2340
	public string[] sonarFrequencies;

	// Token: 0x04000925 RID: 2341
	public float sonarActiveSensitivity;

	// Token: 0x04000926 RID: 2342
	public float sonarPassiveSensitivity;

	// Token: 0x04000927 RID: 2343
	public bool hasBaffle;

	// Token: 0x04000928 RID: 2344
	public float sonarBaffle;

	// Token: 0x04000929 RID: 2345
	public float sonarNoisePerKnot;

	// Token: 0x0400092A RID: 2346
	public float sonarOutput;

	// Token: 0x0400092B RID: 2347
	public bool isTowed;

	// Token: 0x0400092C RID: 2348
	public string sonarDisplayName;

	// Token: 0x0400092D RID: 2349
	public string sonarDescription;
}
