using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200013B RID: 315
public class ObjectPool : MonoBehaviour
{
	// Token: 0x1700013B RID: 315
	// (get) Token: 0x060008A7 RID: 2215 RVA: 0x00062A34 File Offset: 0x00060C34
	// (set) Token: 0x060008A8 RID: 2216 RVA: 0x00062A3C File Offset: 0x00060C3C
	public GameObject Prefab
	{
		get
		{
			return this.prefab;
		}
		set
		{
			this.prefab = value;
		}
	}

	// Token: 0x1700013C RID: 316
	// (get) Token: 0x060008A9 RID: 2217 RVA: 0x00062A48 File Offset: 0x00060C48
	public int Count
	{
		get
		{
			return this.pool.Count;
		}
	}

	// Token: 0x060008AA RID: 2218 RVA: 0x00062A58 File Offset: 0x00060C58
	public void Awake()
	{
		this.pool = new Queue<GameObject>();
	}

	// Token: 0x060008AB RID: 2219 RVA: 0x00062A68 File Offset: 0x00060C68
	public GameObject Instanciate(Vector3 position, Quaternion rotation)
	{
		GameObject gameObject;
		if (this.pool.Count < 1)
		{
			gameObject = (UnityEngine.Object.Instantiate(this.prefab, position, rotation) as GameObject);
		}
		else
		{
			gameObject = this.pool.Dequeue();
			gameObject.transform.SetParent(null);
			gameObject.transform.position = position;
			gameObject.transform.rotation = rotation;
			gameObject.SetActiveRecursively(true);
			gameObject.SendMessage("Start", SendMessageOptions.DontRequireReceiver);
		}
		return gameObject;
	}

	// Token: 0x060008AC RID: 2220 RVA: 0x00062AE4 File Offset: 0x00060CE4
	public void Recycle(GameObject obj)
	{
		obj.active = false;
		obj.transform.SetParent(base.gameObject.transform, false);
		this.pool.Enqueue(obj);
	}

	// Token: 0x04000D6A RID: 3434
	private GameObject prefab;

	// Token: 0x04000D6B RID: 3435
	private Queue<GameObject> pool;
}
