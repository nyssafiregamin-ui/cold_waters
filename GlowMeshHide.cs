using System;
using UnityEngine;

// Token: 0x02000123 RID: 291
public class GlowMeshHide : MonoBehaviour
{
	// Token: 0x060007BD RID: 1981 RVA: 0x00049080 File Offset: 0x00047280
	private void Start()
	{
		for (int i = 0; i < this.dayMesh.Length; i++)
		{
			this.dayMesh[i].SetActive(!GameDataManager.isNight);
		}
		for (int j = 0; j < this.nightMesh.Length; j++)
		{
			this.nightMesh[j].SetActive(GameDataManager.isNight);
		}
	}

	// Token: 0x04000B18 RID: 2840
	public GameObject[] dayMesh;

	// Token: 0x04000B19 RID: 2841
	public GameObject[] nightMesh;
}
