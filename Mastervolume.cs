using System;
using UnityEngine;

// Token: 0x02000138 RID: 312
public class Mastervolume : MonoBehaviour
{
	// Token: 0x06000876 RID: 2166 RVA: 0x0005E684 File Offset: 0x0005C884
	private void Start()
	{
		base.GetComponent<AudioSource>().volume = GameDataManager.currentvolume;
	}
}
