using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200013C RID: 316
public class ObjectPoolManager : MonoBehaviour
{
	// Token: 0x1700013D RID: 317
	// (get) Token: 0x060008AF RID: 2223 RVA: 0x00062B48 File Offset: 0x00060D48
	public static ObjectPoolManager Instance
	{
		get
		{
			if (!ObjectPoolManager.instance)
			{
				ObjectPoolManager.instance = (UnityEngine.Object.FindObjectOfType(typeof(ObjectPoolManager)) as ObjectPoolManager);
				if (!ObjectPoolManager.instance)
				{
					GameObject gameObject = new GameObject("ObjectPoolManager");
					ObjectPoolManager.instance = gameObject.AddComponent<ObjectPoolManager>();
				}
			}
			return ObjectPoolManager.instance;
		}
	}

	// Token: 0x060008B0 RID: 2224 RVA: 0x00062BA8 File Offset: 0x00060DA8
	private void OnApplicationQuit()
	{
		ObjectPoolManager.instance = null;
	}

	// Token: 0x060008B1 RID: 2225 RVA: 0x00062BB0 File Offset: 0x00060DB0
	public static GameObject CreatePooled(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		return ObjectPoolManager.Instance.InternalCreate(prefab, position, rotation);
	}

	// Token: 0x060008B2 RID: 2226 RVA: 0x00062BC0 File Offset: 0x00060DC0
	public static void DestroyPooled(GameObject obj)
	{
		ObjectPoolManager.Instance.InternalDestroy(obj);
	}

	// Token: 0x060008B3 RID: 2227 RVA: 0x00062BD0 File Offset: 0x00060DD0
	public static void DestroyPooled(GameObject obj, float delay)
	{
		ObjectPoolManager.Instance.StartCoroutine(ObjectPoolManager.Instance.InternalDestroy(obj, delay));
	}

	// Token: 0x060008B4 RID: 2228 RVA: 0x00062BEC File Offset: 0x00060DEC
	private void Awake()
	{
		this.prefab2pool = new Dictionary<GameObject, ObjectPool>();
		this.instance2pool = new Dictionary<GameObject, ObjectPool>();
	}

	// Token: 0x060008B5 RID: 2229 RVA: 0x00062C04 File Offset: 0x00060E04
	private ObjectPool CreatePool(GameObject prefab)
	{
		GameObject gameObject = new GameObject(prefab.name + " Pool");
		ObjectPool objectPool = gameObject.AddComponent<ObjectPool>();
		objectPool.Prefab = prefab;
		return objectPool;
	}

	// Token: 0x060008B6 RID: 2230 RVA: 0x00062C38 File Offset: 0x00060E38
	private GameObject InternalCreate(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		ObjectPool objectPool;
		if (!this.prefab2pool.ContainsKey(prefab))
		{
			objectPool = this.CreatePool(prefab);
			objectPool.gameObject.transform.parent = base.gameObject.transform;
			this.prefab2pool[prefab] = objectPool;
		}
		else
		{
			objectPool = this.prefab2pool[prefab];
		}
		GameObject gameObject = objectPool.Instanciate(position, rotation);
		this.instance2pool[gameObject] = objectPool;
		return gameObject;
	}

	// Token: 0x060008B7 RID: 2231 RVA: 0x00062CB0 File Offset: 0x00060EB0
	private void InternalDestroy(GameObject obj)
	{
		if (this.instance2pool.ContainsKey(obj))
		{
			ObjectPool objectPool = this.instance2pool[obj];
			objectPool.Recycle(obj);
			int childCount = base.transform.childCount;
			foreach (object obj2 in obj.transform)
			{
				Transform transform = (Transform)obj2;
				int childCount2 = transform.transform.childCount;
				foreach (object obj3 in transform.transform)
				{
					Transform transform2 = (Transform)obj3;
					transform2.gameObject.SetActive(false);
				}
				transform.gameObject.SetActive(false);
			}
		}
		else
		{
			UnityEngine.Object.Destroy(obj);
		}
	}

	// Token: 0x060008B8 RID: 2232 RVA: 0x00062DDC File Offset: 0x00060FDC
	private IEnumerator InternalDestroy(GameObject obj, float delay)
	{
		yield return new WaitForSeconds(delay);
		this.InternalDestroy(obj);
		yield break;
	}

	// Token: 0x060008B9 RID: 2233 RVA: 0x00062E14 File Offset: 0x00061014
	private void OnGUI()
	{
		if (this.Debug)
		{
			GUILayout.BeginArea(this.DebugGuiRect);
			GUILayout.BeginVertical(new GUILayoutOption[0]);
			GUILayout.Label("Pools: " + this.prefab2pool.Count, new GUILayoutOption[0]);
			foreach (ObjectPool objectPool in this.prefab2pool.Values)
			{
				GUILayout.Label(objectPool.Prefab.name + ": " + objectPool.Count, new GUILayoutOption[0]);
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}

	// Token: 0x04000D6C RID: 3436
	private static ObjectPoolManager instance;

	// Token: 0x04000D6D RID: 3437
	public bool Debug;

	// Token: 0x04000D6E RID: 3438
	public Rect DebugGuiRect = new Rect(5f, 200f, 160f, 400f);

	// Token: 0x04000D6F RID: 3439
	private Dictionary<GameObject, ObjectPool> prefab2pool;

	// Token: 0x04000D70 RID: 3440
	private Dictionary<GameObject, ObjectPool> instance2pool;
}
