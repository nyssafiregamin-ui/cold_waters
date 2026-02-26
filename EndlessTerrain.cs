using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020000D1 RID: 209
public class EndlessTerrain : MonoBehaviour
{
	// Token: 0x060005D1 RID: 1489 RVA: 0x00027918 File Offset: 0x00025B18
	public void InitialiseEndlessTerrain()
	{
		EndlessTerrain.viewerPosition = new Vector2(this.viewer.position.x, this.viewer.position.z) / 5f;
		EndlessTerrain.terrainChunksVisibleLastUpdate.Clear();
		this.chunkSize = 29;
		this.chunksVisibleInViewDst = Mathf.RoundToInt(EndlessTerrain.maxViewDst / (float)this.chunkSize);
		this.UpdateVisibleChunks();
		this.UpdateVisibleChunks();
	}

	// Token: 0x060005D2 RID: 1490 RVA: 0x00027998 File Offset: 0x00025B98
	private void Update()
	{
		bool flag = false;
		if (this.viewer != GameDataManager.playervesselsonlevel[0].transform)
		{
			EndlessTerrain.viewerPosition = new Vector2(GameDataManager.playervesselsonlevel[0].transform.position.x, GameDataManager.playervesselsonlevel[0].transform.position.z) / 5f;
			if ((this.playerPositionOld - EndlessTerrain.viewerPosition).sqrMagnitude > 25f)
			{
				this.playerPositionOld = EndlessTerrain.viewerPosition;
				this.UpdateVisibleChunks();
				flag = true;
			}
		}
		EndlessTerrain.viewerPosition = new Vector2(this.viewer.position.x, this.viewer.position.z) / 5f;
		if ((this.viewerPositionOld - EndlessTerrain.viewerPosition).sqrMagnitude > 25f || flag)
		{
			this.viewerPositionOld = EndlessTerrain.viewerPosition;
			this.UpdateVisibleChunks();
		}
	}

	// Token: 0x060005D3 RID: 1491 RVA: 0x00027AB4 File Offset: 0x00025CB4
	private void UpdateVisibleChunks()
	{
		for (int i = 0; i < EndlessTerrain.terrainChunksVisibleLastUpdate.Count; i++)
		{
			EndlessTerrain.terrainChunksVisibleLastUpdate[i].SetVisible(false);
		}
		EndlessTerrain.terrainChunksVisibleLastUpdate.Clear();
		int num = Mathf.RoundToInt(EndlessTerrain.viewerPosition.x / (float)this.chunkSize);
		int num2 = Mathf.RoundToInt(EndlessTerrain.viewerPosition.y / (float)this.chunkSize);
		for (int j = -this.chunksVisibleInViewDst; j <= this.chunksVisibleInViewDst; j++)
		{
			for (int k = -this.chunksVisibleInViewDst; k <= this.chunksVisibleInViewDst; k++)
			{
				Vector2 vector = new Vector2((float)(num + k), (float)(num2 + j));
				if (this.terrainChunkDictionary.ContainsKey(vector))
				{
					this.terrainChunkDictionary[vector].UpdateTerrainChunk();
				}
				else
				{
					this.terrainChunkDictionary.Add(vector, new EndlessTerrain.TerrainChunk(vector, this.chunkSize, this.detailLevels, base.transform, this.mapMaterial));
				}
			}
		}
	}

	// Token: 0x040005D2 RID: 1490
	private const float scale = 5f;

	// Token: 0x040005D3 RID: 1491
	private const float viewerMoveThresholdForChunkUpdate = 5f;

	// Token: 0x040005D4 RID: 1492
	private const float sqrViewerMoveThresholdForChunkUpdate = 25f;

	// Token: 0x040005D5 RID: 1493
	public EndlessTerrain.LODInfo[] detailLevels;

	// Token: 0x040005D6 RID: 1494
	public static float maxViewDst = 50f;

	// Token: 0x040005D7 RID: 1495
	public Transform viewer;

	// Token: 0x040005D8 RID: 1496
	public Material mapMaterial;

	// Token: 0x040005D9 RID: 1497
	public static Vector2 viewerPosition;

	// Token: 0x040005DA RID: 1498
	private Vector2 viewerPositionOld;

	// Token: 0x040005DB RID: 1499
	private Vector2 playerPositionOld;

	// Token: 0x040005DC RID: 1500
	private int chunkSize;

	// Token: 0x040005DD RID: 1501
	private int chunksVisibleInViewDst;

	// Token: 0x040005DE RID: 1502
	private Dictionary<Vector2, EndlessTerrain.TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, EndlessTerrain.TerrainChunk>();

	// Token: 0x040005DF RID: 1503
	private static List<EndlessTerrain.TerrainChunk> terrainChunksVisibleLastUpdate = new List<EndlessTerrain.TerrainChunk>();

	// Token: 0x020000D2 RID: 210
	public class TerrainChunk
	{
		// Token: 0x060005D4 RID: 1492 RVA: 0x00027BCC File Offset: 0x00025DCC
		public TerrainChunk(Vector2 coord, int size, EndlessTerrain.LODInfo[] detailLevels, Transform parent, Material material)
		{
			this.detailLevels = detailLevels;
			this.position = coord * (float)size;
			this.bounds = new Bounds(this.position, Vector2.one * (float)size);
			Vector3 a = new Vector3(this.position.x, 0f, this.position.y);
			this.meshObject = new GameObject("Terrain Chunk");
			this.meshRenderer = this.meshObject.AddComponent<MeshRenderer>();
			this.meshFilter = this.meshObject.AddComponent<MeshFilter>();
			this.meshRenderer.sharedMaterial = material;
			this.meshObject.transform.position = a * 5f;
			this.meshObject.transform.parent = parent;
			Vector3 localPosition = this.meshObject.transform.position;
			localPosition.y = 0f;
			this.meshObject.transform.localPosition = localPosition;
			this.meshObject.transform.localScale = Vector3.one * 5f;
			this.SetVisible(false);
			this.lodMeshes = new EndlessTerrain.LODMesh[detailLevels.Length];
			for (int i = 0; i < detailLevels.Length; i++)
			{
				this.lodMeshes[i] = new EndlessTerrain.LODMesh(detailLevels[i].lod, new Action(this.UpdateTerrainChunk));
			}
			if (!MapGenerator.mapgenerator.useDEMData)
			{
				MapGenerator.mapgenerator.RequestMapData(this.position, new Action<MapData>(this.OnMapDataReceived));
			}
			else
			{
				this.OnMapDataReceived(MapGenerator.mapgenerator.GenerateDEMData(coord));
			}
			this.meshObject.layer = 30;
			this.meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			this.meshRenderer.receiveShadows = true;
		}

		// Token: 0x060005D5 RID: 1493 RVA: 0x00027DB4 File Offset: 0x00025FB4
		private void OnMapDataReceived(MapData mapData)
		{
			this.mapData = mapData;
			this.mapDataReceived = true;
			this.UpdateTerrainChunk();
		}

		// Token: 0x060005D6 RID: 1494 RVA: 0x00027DCC File Offset: 0x00025FCC
		public void UpdateTerrainChunk()
		{
			if (this.mapDataReceived)
			{
				float num = Mathf.Sqrt(this.bounds.SqrDistance(EndlessTerrain.viewerPosition));
				bool flag = num <= EndlessTerrain.maxViewDst;
				if (flag)
				{
					int num2 = 0;
					for (int i = 0; i < this.detailLevels.Length - 1; i++)
					{
						if (num <= this.detailLevels[i].visibleDstThreshold)
						{
							break;
						}
						num2 = i + 1;
					}
					if (num2 != this.previousLODIndex)
					{
						EndlessTerrain.LODMesh lodmesh = this.lodMeshes[num2];
						if (lodmesh.hasMesh)
						{
							this.previousLODIndex = num2;
							this.meshFilter.mesh = lodmesh.mesh;
							this.meshFilter.gameObject.AddComponent<MeshCollider>().sharedMesh = this.meshFilter.mesh;
							Vector3 min = this.meshFilter.gameObject.GetComponent<Collider>().bounds.min;
							Vector3 max = this.meshFilter.gameObject.GetComponent<Collider>().bounds.max;
							if (UIFunctions.globaluifunctions.levelloadmanager.icePresent && min.y < 999.8f)
							{
								UIFunctions.globaluifunctions.levelloadmanager.SetupIcebergChunk(this.meshObject.transform);
							}
							if (MapGenerator.mapgenerator.addTrees)
							{
								if (max.y >= MapGenerator.mapgenerator.treeMinMaxElevation.x)
								{
									if (min.y <= MapGenerator.mapgenerator.treeMinMaxElevation.y)
									{
										TurboForest turboForest = this.meshFilter.gameObject.AddComponent<TurboForest>();
										turboForest.GenerateTrees(MapGenerator.mapgenerator.treeDensity, MapGenerator.mapgenerator.treeMinMaxElevation, MapGenerator.mapgenerator.surfaceAngleThreshold, MapGenerator.mapgenerator.treeBaseSize, MapGenerator.mapgenerator.treeSizeRandomize, MapGenerator.mapgenerator.treeShadingRandomize, MapGenerator.mapgenerator.treeMaterial, min, max, 20f);
										UnityEngine.Object.Destroy(turboForest);
										GC.Collect();
									}
								}
								Vector2 elevations = new Vector2(998.25f, 999.7f);
								if (max.y >= elevations.x)
								{
									if (min.y <= elevations.y)
									{
										TurboForest turboForest2 = this.meshFilter.gameObject.AddComponent<TurboForest>();
										turboForest2.GenerateTrees(MapGenerator.mapgenerator.treeDensity, elevations, MapGenerator.mapgenerator.surfaceAngleThreshold, MapGenerator.mapgenerator.seaweedBaseSize, MapGenerator.mapgenerator.seaweedSizeRandomize, MapGenerator.mapgenerator.seaweedShadingRandomize, MapGenerator.mapgenerator.seaweedMaterial, min, max, 60f);
										UnityEngine.Object.Destroy(turboForest2);
										GC.Collect();
									}
								}
							}
						}
						else if (!lodmesh.hasRequestedMesh)
						{
							lodmesh.RequestMesh(this.mapData);
						}
					}
					EndlessTerrain.terrainChunksVisibleLastUpdate.Add(this);
				}
				this.SetVisible(flag);
			}
		}

		// Token: 0x060005D7 RID: 1495 RVA: 0x000280D0 File Offset: 0x000262D0
		public void SetVisible(bool visible)
		{
			this.meshObject.GetComponent<MeshRenderer>().enabled = visible;
			foreach (object obj in this.meshObject.transform)
			{
				Transform transform = (Transform)obj;
				transform.gameObject.SetActive(visible);
			}
		}

		// Token: 0x060005D8 RID: 1496 RVA: 0x0002815C File Offset: 0x0002635C
		public bool IsVisible()
		{
			return this.meshObject.GetComponent<MeshRenderer>().enabled;
		}

		// Token: 0x040005E0 RID: 1504
		private GameObject meshObject;

		// Token: 0x040005E1 RID: 1505
		private Vector2 position;

		// Token: 0x040005E2 RID: 1506
		private Bounds bounds;

		// Token: 0x040005E3 RID: 1507
		private MeshRenderer meshRenderer;

		// Token: 0x040005E4 RID: 1508
		private MeshFilter meshFilter;

		// Token: 0x040005E5 RID: 1509
		private EndlessTerrain.LODInfo[] detailLevels;

		// Token: 0x040005E6 RID: 1510
		private EndlessTerrain.LODMesh[] lodMeshes;

		// Token: 0x040005E7 RID: 1511
		private MapData mapData;

		// Token: 0x040005E8 RID: 1512
		private bool mapDataReceived;

		// Token: 0x040005E9 RID: 1513
		private int previousLODIndex = -1;
	}

	// Token: 0x020000D3 RID: 211
	private class LODMesh
	{
		// Token: 0x060005D9 RID: 1497 RVA: 0x00028170 File Offset: 0x00026370
		public LODMesh(int lod, Action updateCallback)
		{
			this.lod = lod;
			this.updateCallback = updateCallback;
		}

		// Token: 0x060005DA RID: 1498 RVA: 0x00028188 File Offset: 0x00026388
		private void OnMeshDataReceived(MeshData meshData)
		{
			this.mesh = meshData.CreateMesh();
			this.hasMesh = true;
			this.updateCallback();
		}

		// Token: 0x060005DB RID: 1499 RVA: 0x000281A8 File Offset: 0x000263A8
		public void RequestMesh(MapData mapData)
		{
			this.hasRequestedMesh = true;
			MapGenerator.mapgenerator.RequestMeshData(mapData, this.lod, new Action<MeshData>(this.OnMeshDataReceived));
		}

		// Token: 0x040005EA RID: 1514
		public Mesh mesh;

		// Token: 0x040005EB RID: 1515
		public bool hasRequestedMesh;

		// Token: 0x040005EC RID: 1516
		public bool hasMesh;

		// Token: 0x040005ED RID: 1517
		private int lod;

		// Token: 0x040005EE RID: 1518
		private Action updateCallback;
	}

	// Token: 0x020000D4 RID: 212
	[Serializable]
	public struct LODInfo
	{
		// Token: 0x040005EF RID: 1519
		public int lod;

		// Token: 0x040005F0 RID: 1520
		public float visibleDstThreshold;
	}
}
