using System;
using UnityEngine;

// Token: 0x020000DE RID: 222
public static class TextureGenerator
{
	// Token: 0x060005F2 RID: 1522 RVA: 0x00028F84 File Offset: 0x00027184
	public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
	{
		Texture2D texture2D = new Texture2D(width, height);
		texture2D.wrapMode = TextureWrapMode.Clamp;
		texture2D.SetPixels(colourMap);
		texture2D.Apply();
		return texture2D;
	}

	// Token: 0x060005F3 RID: 1523 RVA: 0x00028FB0 File Offset: 0x000271B0
	public static Texture2D TextureFromHeightMap(float[,] heightMap)
	{
		int length = heightMap.GetLength(0);
		int length2 = heightMap.GetLength(1);
		Color[] array = new Color[length * length2];
		for (int i = 0; i < length2; i++)
		{
			for (int j = 0; j < length; j++)
			{
				array[i * length + j] = Color.Lerp(Color.black, Color.white, heightMap[j, i]);
			}
		}
		return TextureGenerator.TextureFromColourMap(array, length, length2);
	}
}
