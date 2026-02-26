using System;

// Token: 0x020000D9 RID: 217
public struct MapData
{
	// Token: 0x060005E9 RID: 1513 RVA: 0x00028720 File Offset: 0x00026920
	public MapData(float[,] heightMap)
	{
		this.heightMap = heightMap;
	}

	// Token: 0x04000617 RID: 1559
	public readonly float[,] heightMap;
}
