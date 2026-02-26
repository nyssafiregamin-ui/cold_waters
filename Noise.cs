using System;
using UnityEngine;

// Token: 0x020000DC RID: 220
public static class Noise
{
	// Token: 0x060005F1 RID: 1521 RVA: 0x00028D34 File Offset: 0x00026F34
	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, Noise.NormalizeMode normalizeMode)
	{
		float[,] array = new float[mapWidth, mapHeight];
		System.Random random = new System.Random(seed);
		Vector2[] array2 = new Vector2[octaves];
		float num = 0f;
		float num2 = 1f;
		for (int i = 0; i < octaves; i++)
		{
			float x = (float)random.Next(-100000, 100000) + offset.x;
			float y = (float)random.Next(-100000, 100000) - offset.y;
			array2[i] = new Vector2(x, y);
			num += num2;
			num2 *= persistance;
		}
		if (scale <= 0f)
		{
			scale = 0.0001f;
		}
		float num3 = float.MinValue;
		float num4 = float.MaxValue;
		float num5 = (float)mapWidth / 2f;
		float num6 = (float)mapHeight / 2f;
		for (int j = 0; j < mapHeight; j++)
		{
			for (int k = 0; k < mapWidth; k++)
			{
				num2 = 1f;
				float num7 = 1f;
				float num8 = 0f;
				for (int l = 0; l < octaves; l++)
				{
					float x2 = ((float)k - num5 + array2[l].x) / scale * num7;
					float y2 = ((float)j - num6 + array2[l].y) / scale * num7;
					float num9 = Mathf.PerlinNoise(x2, y2) * 2f - 1f;
					num8 += num9 * num2;
					num2 *= persistance;
					num7 *= lacunarity;
				}
				if (num8 > num3)
				{
					num3 = num8;
				}
				else if (num8 < num4)
				{
					num4 = num8;
				}
				array[k, j] = num8;
			}
		}
		for (int m = 0; m < mapHeight; m++)
		{
			for (int n = 0; n < mapWidth; n++)
			{
				if (normalizeMode == Noise.NormalizeMode.Local)
				{
					array[n, m] = Mathf.InverseLerp(num4, num3, array[n, m]);
				}
				else
				{
					float value = (array[n, m] + 1f) / (num / 0.9f);
					array[n, m] = Mathf.Clamp(value, 0f, 2.1474836E+09f);
				}
			}
		}
		return array;
	}

	// Token: 0x020000DD RID: 221
	public enum NormalizeMode
	{
		// Token: 0x04000620 RID: 1568
		Local,
		// Token: 0x04000621 RID: 1569
		Global
	}
}
