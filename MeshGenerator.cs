using System;
using UnityEngine;

// Token: 0x020000DA RID: 218
public static class MeshGenerator
{
	// Token: 0x060005EA RID: 1514 RVA: 0x0002872C File Offset: 0x0002692C
	public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, int levelOfDetail)
	{
		AnimationCurve animationCurve = new AnimationCurve(_heightCurve.keys);
		int num = (levelOfDetail != 0) ? (levelOfDetail * 2) : 1;
		int length = heightMap.GetLength(0);
		int num2 = length - 2 * num;
		int num3 = length - 2;
		float num4 = (float)(num3 - 1) / -2f;
		float num5 = (float)(num3 - 1) / 2f;
		int verticesPerLine = (num2 - 1) / num + 1;
		MeshData meshData = new MeshData(verticesPerLine);
		int[,] array = new int[length, length];
		int num6 = 0;
		int num7 = -1;
		for (int i = 0; i < length; i += num)
		{
			for (int j = 0; j < length; j += num)
			{
				bool flag = i == 0 || i == length - 1 || j == 0 || j == length - 1;
				if (flag)
				{
					array[j, i] = num7;
					num7--;
				}
				else
				{
					array[j, i] = num6;
					num6++;
				}
			}
		}
		for (int k = 0; k < length; k += num)
		{
			for (int l = 0; l < length; l += num)
			{
				int num8 = array[l, k];
				Vector2 uv = new Vector2((float)(l - num) / (float)num2, (float)(k - num) / (float)num2);
				float y = animationCurve.Evaluate(heightMap[l, k]) * heightMultiplier;
				Vector3 vertexPosition = new Vector3(num4 + uv.x * (float)num3, y, num5 - uv.y * (float)num3);
				meshData.AddVertex(vertexPosition, uv, num8);
				if (l < length - 1 && k < length - 1)
				{
					int num9 = array[l, k];
					int c = array[l + num, k];
					int c2 = array[l, k + num];
					int num10 = array[l + num, k + num];
					meshData.AddTriangle(num9, num10, c2);
					meshData.AddTriangle(num10, num9, c);
				}
				num8++;
			}
		}
		return meshData;
	}
}
