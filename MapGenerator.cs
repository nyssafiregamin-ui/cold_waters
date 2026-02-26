using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// Token: 0x020000D6 RID: 214
public class MapGenerator : MonoBehaviour
{
	// Token: 0x060005DF RID: 1503 RVA: 0x000282B4 File Offset: 0x000264B4
	public void InitialiseTerrain(string elevationDataPath)
	{
		MapGenerator.mapgenerator = this;
		MapGenerator.endlessterrain = UnityEngine.Object.FindObjectOfType<EndlessTerrain>();
		this.treeMinMaxElevation.x = this.treeMinMaxElevation.x + base.transform.position.y;
		this.treeMinMaxElevation.y = this.treeMinMaxElevation.y + base.transform.position.y;
		DEMGenerator.combatZoneDEM = DEMGenerator.GetCombatZoneDEM((int)this.demMapCoods.x, (int)this.demMapCoods.y, this.sampleAreaSize, this.scaledAreaSize, elevationDataPath);
		if (!MapGenerator.terrainDetected)
		{
			base.gameObject.SetActive(false);
		}
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.waterDepth = this.meshHeightCurve.Evaluate(UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.waterDepth) * this.meshHeightMultiplier * 225.39f;
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x000283A0 File Offset: 0x000265A0
	public void RequestMapData(Vector2 centre, Action<MapData> callback)
	{
		ThreadStart start = delegate()
		{
			this.MapDataThread(centre, callback);
		};
		new Thread(start).Start();
	}

	// Token: 0x060005E1 RID: 1505 RVA: 0x000283E0 File Offset: 0x000265E0
	private void MapDataThread(Vector2 centre, Action<MapData> callback)
	{
		MapData parameter = this.GenerateMapData(centre);
		Queue<MapGenerator.MapThreadInfo<MapData>> obj = this.mapDataThreadInfoQueue;
		lock (obj)
		{
			this.mapDataThreadInfoQueue.Enqueue(new MapGenerator.MapThreadInfo<MapData>(callback, parameter));
		}
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x0002843C File Offset: 0x0002663C
	public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
	{
		ThreadStart start = delegate()
		{
			this.MeshDataThread(mapData, lod, callback);
		};
		new Thread(start).Start();
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x00028484 File Offset: 0x00026684
	private void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
	{
		MeshData parameter = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, this.meshHeightMultiplier, this.meshHeightCurve, lod);
		Queue<MapGenerator.MapThreadInfo<MeshData>> obj = this.meshDataThreadInfoQueue;
		lock (obj)
		{
			this.meshDataThreadInfoQueue.Enqueue(new MapGenerator.MapThreadInfo<MeshData>(callback, parameter));
		}
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x000284F4 File Offset: 0x000266F4
	private void Update()
	{
		if (this.mapDataThreadInfoQueue.Count > 0)
		{
			for (int i = 0; i < this.mapDataThreadInfoQueue.Count; i++)
			{
				MapGenerator.MapThreadInfo<MapData> mapThreadInfo = this.mapDataThreadInfoQueue.Dequeue();
				mapThreadInfo.callback(mapThreadInfo.parameter);
			}
		}
		if (this.meshDataThreadInfoQueue.Count > 0)
		{
			for (int j = 0; j < this.meshDataThreadInfoQueue.Count; j++)
			{
				MapGenerator.MapThreadInfo<MeshData> mapThreadInfo2 = this.meshDataThreadInfoQueue.Dequeue();
				mapThreadInfo2.callback(mapThreadInfo2.parameter);
			}
		}
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x0002859C File Offset: 0x0002679C
	private MapData GenerateMapData(Vector2 centre)
	{
		float[,] heightMap = Noise.GenerateNoiseMap(32, 32, this.seed, this.noiseScale, this.octaves, this.persistance, this.lacunarity, centre + this.offset, this.normalizeMode);
		return new MapData(heightMap);
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x000285EC File Offset: 0x000267EC
	public MapData GenerateDEMData(Vector2 coord)
	{
		float[,] array = DEMGenerator.GenerateDEMMap(32, coord);
		if (this.applyNoiseToDEM)
		{
			Vector2 a = new Vector2(coord.x * 29f, coord.y * 29f);
			float[,] array2 = Noise.GenerateNoiseMap(32, 32, this.seed, this.noiseScale, this.octaves, this.persistance, this.lacunarity, a + this.offset, this.normalizeMode);
			for (int i = 0; i < array.GetLength(0); i++)
			{
				for (int j = 0; j < array.GetLength(1); j++)
				{
					if (array[i, j] > 0.36f)
					{
						array[i, j] += array2[i, j] * this.noiseFactor;
					}
				}
			}
		}
		return new MapData(array);
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x000286D4 File Offset: 0x000268D4
	private void OnValidate()
	{
		if (this.lacunarity < 1f)
		{
			this.lacunarity = 1f;
		}
		if (this.octaves < 0)
		{
			this.octaves = 0;
		}
	}

	// Token: 0x040005F1 RID: 1521
	public const int mapChunkSize = 30;

	// Token: 0x040005F2 RID: 1522
	public Noise.NormalizeMode normalizeMode;

	// Token: 0x040005F3 RID: 1523
	public float noiseScale;

	// Token: 0x040005F4 RID: 1524
	public int octaves;

	// Token: 0x040005F5 RID: 1525
	[Range(0f, 1f)]
	public float persistance;

	// Token: 0x040005F6 RID: 1526
	public float lacunarity;

	// Token: 0x040005F7 RID: 1527
	public int seed;

	// Token: 0x040005F8 RID: 1528
	public Vector2 offset;

	// Token: 0x040005F9 RID: 1529
	public float meshHeightMultiplier;

	// Token: 0x040005FA RID: 1530
	public AnimationCurve meshHeightCurve;

	// Token: 0x040005FB RID: 1531
	public bool useDEMData;

	// Token: 0x040005FC RID: 1532
	public static bool terrainDetected;

	// Token: 0x040005FD RID: 1533
	public Vector2 demMapCoods;

	// Token: 0x040005FE RID: 1534
	public int sampleAreaSize;

	// Token: 0x040005FF RID: 1535
	public int scaledAreaSize;

	// Token: 0x04000600 RID: 1536
	public bool applyNoiseToDEM;

	// Token: 0x04000601 RID: 1537
	public float noiseFactor;

	// Token: 0x04000602 RID: 1538
	private Queue<MapGenerator.MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapGenerator.MapThreadInfo<MapData>>();

	// Token: 0x04000603 RID: 1539
	private Queue<MapGenerator.MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapGenerator.MapThreadInfo<MeshData>>();

	// Token: 0x04000604 RID: 1540
	public bool addTrees;

	// Token: 0x04000605 RID: 1541
	[Range(0f, 300f)]
	public int treeDensity;

	// Token: 0x04000606 RID: 1542
	public Vector2 treeMinMaxElevation;

	// Token: 0x04000607 RID: 1543
	[Range(0f, 90f)]
	public float surfaceAngleThreshold;

	// Token: 0x04000608 RID: 1544
	public float treeBaseSize;

	// Token: 0x04000609 RID: 1545
	public float treeSizeRandomize;

	// Token: 0x0400060A RID: 1546
	public float treeShadingRandomize;

	// Token: 0x0400060B RID: 1547
	public Material treeMaterial;

	// Token: 0x0400060C RID: 1548
	public Material seaweedMaterial;

	// Token: 0x0400060D RID: 1549
	public float seaweedBaseSize;

	// Token: 0x0400060E RID: 1550
	public float seaweedSizeRandomize;

	// Token: 0x0400060F RID: 1551
	public float seaweedShadingRandomize;

	// Token: 0x04000610 RID: 1552
	public static MapGenerator mapgenerator;

	// Token: 0x04000611 RID: 1553
	public static EndlessTerrain endlessterrain;

	// Token: 0x020000D7 RID: 215
	private struct MapThreadInfo<T>
	{
		// Token: 0x060005E8 RID: 1512 RVA: 0x00028710 File Offset: 0x00026910
		public MapThreadInfo(Action<T> callback, T parameter)
		{
			this.callback = callback;
			this.parameter = parameter;
		}

		// Token: 0x04000612 RID: 1554
		public readonly Action<T> callback;

		// Token: 0x04000613 RID: 1555
		public readonly T parameter;
	}
}
