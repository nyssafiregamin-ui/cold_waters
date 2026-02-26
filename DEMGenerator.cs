using System;
using UnityEngine;

// Token: 0x020000D0 RID: 208
public class DEMGenerator
{
	// Token: 0x060005CD RID: 1485 RVA: 0x00027758 File Offset: 0x00025958
	public static Texture2D GetCombatZoneDEM(int xPos, int yPos, int sampleAreaSize, int scaledAreaSize, string mapPath)
	{
		xPos -= sampleAreaSize / 2;
		yPos -= sampleAreaSize / 2;
		Texture2D texture2D = Resources.Load<Texture2D>(mapPath);
		texture2D.wrapMode = TextureWrapMode.Clamp;
		Texture2D texture2D2 = new Texture2D(sampleAreaSize, sampleAreaSize);
		texture2D2.wrapMode = TextureWrapMode.Clamp;
		MapGenerator.terrainDetected = false;
		for (int i = 0; i < sampleAreaSize; i++)
		{
			for (int j = 0; j < sampleAreaSize; j++)
			{
				Color pixel = texture2D.GetPixel(i + xPos, j + yPos);
				texture2D2.SetPixel(i, j, pixel);
				if (!MapGenerator.terrainDetected && pixel.r > 0.05f)
				{
					MapGenerator.terrainDetected = true;
				}
			}
		}
		TextureScale.Bilinear(texture2D2, scaledAreaSize, scaledAreaSize);
		DEMGenerator.mapCentreOffset = scaledAreaSize / 2;
		UIFunctions.globaluifunctions.levelloadmanager.levelloaddata.waterDepth = texture2D2.GetPixel(Mathf.FloorToInt((float)(sampleAreaSize / 2)), Mathf.FloorToInt((float)(sampleAreaSize / 2))).r;
		return texture2D2;
	}

	// Token: 0x060005CE RID: 1486 RVA: 0x0002783C File Offset: 0x00025A3C
	public static float[,] GenerateDEMMap(int mapSize, Vector2 coord)
	{
		Texture2D texture2D = DEMGenerator.combatZoneDEM;
		int num = mapSize - 1;
		int num2 = DEMGenerator.mapCentreOffset;
		int num3 = DEMGenerator.mapCentreOffset;
		num2 += Mathf.RoundToInt(coord.x) * (num - 2) - 1;
		num3 += Mathf.RoundToInt(coord.y) * (num - 2) - 1;
		float[,] array = new float[mapSize, mapSize];
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				array[i, num - 1 - j] = texture2D.GetPixel(i + num2, j + num3).r;
			}
		}
		return array;
	}

	// Token: 0x040005D0 RID: 1488
	public static Texture2D combatZoneDEM;

	// Token: 0x040005D1 RID: 1489
	public static int mapCentreOffset;
}
