using System;
using UnityEngine;

// Token: 0x02000128 RID: 296
public class IceSpawnCheck : MonoBehaviour
{
	// Token: 0x06000805 RID: 2053 RVA: 0x0004EC30 File Offset: 0x0004CE30
	private void Start()
	{
		this.radius = base.GetComponentInChildren<SphereCollider>().radius;
	}

	// Token: 0x06000806 RID: 2054 RVA: 0x0004EC44 File Offset: 0x0004CE44
	private void FixedUpdate()
	{
		if (Time.timeScale != 0f)
		{
			bool flag = false;
			if (Vector3.Distance(base.transform.position, GameDataManager.playervesselsonlevel[0].transform.position) < this.radius * 2f)
			{
				flag = true;
			}
			if (!flag)
			{
				for (int i = 0; i < GameDataManager.enemyvesselsonlevel.Length; i++)
				{
					if (Vector3.Distance(base.transform.position, GameDataManager.enemyvesselsonlevel[i].transform.position) < this.radius * 2f)
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				base.gameObject.SetActive(false);
			}
			base.enabled = false;
		}
	}

	// Token: 0x04000BBB RID: 3003
	private float radius;
}
