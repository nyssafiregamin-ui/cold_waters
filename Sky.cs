using System;
using mset;
using UnityEngine;

// Token: 0x020000E3 RID: 227
public class Sky : mset.Sky
{
	// Token: 0x06000604 RID: 1540 RVA: 0x00029E04 File Offset: 0x00028004
	public void OnValidate()
	{
		Debug.LogWarning("Skyshop sky \"" + base.gameObject.name + "\" is using a deprecated script. Please Run the \"Edit->Skyshop->Upgrade Skies\" macro on this scene.");
	}

	// Token: 0x17000138 RID: 312
	// (get) Token: 0x06000606 RID: 1542 RVA: 0x00029E34 File Offset: 0x00028034
	// (set) Token: 0x06000605 RID: 1541 RVA: 0x00029E28 File Offset: 0x00028028
	public new static global::Sky activeSky
	{
		get
		{
			Debug.LogError("Trying to access Sky.activeSky in the global namespace (deprecated script). Use mset.Sky.activeSky instead.");
			return null;
		}
		set
		{
			Debug.LogError("Trying to access Sky.activeSky in the global namespace (deprecated script). Use mset.Sky.activeSky instead.");
		}
	}
}
