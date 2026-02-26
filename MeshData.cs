using System;
using UnityEngine;

// Token: 0x020000DB RID: 219
public class MeshData
{
	// Token: 0x060005EB RID: 1515 RVA: 0x00028930 File Offset: 0x00026B30
	public MeshData(int verticesPerLine)
	{
		this.vertices = new Vector3[verticesPerLine * verticesPerLine];
		this.uvs = new Vector2[verticesPerLine * verticesPerLine];
		this.triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];
		this.borderVertices = new Vector3[verticesPerLine * 4 + 4];
		this.borderTriangles = new int[24 * verticesPerLine];
	}

	// Token: 0x060005EC RID: 1516 RVA: 0x00028994 File Offset: 0x00026B94
	public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
	{
		if (vertexIndex < 0)
		{
			this.borderVertices[-vertexIndex - 1] = vertexPosition;
		}
		else
		{
			this.vertices[vertexIndex] = vertexPosition;
			this.uvs[vertexIndex] = uv;
		}
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x000289E8 File Offset: 0x00026BE8
	public void AddTriangle(int a, int b, int c)
	{
		if (a < 0 || b < 0 || c < 0)
		{
			this.borderTriangles[this.borderTriangleIndex] = a;
			this.borderTriangles[this.borderTriangleIndex + 1] = b;
			this.borderTriangles[this.borderTriangleIndex + 2] = c;
			this.borderTriangleIndex += 3;
		}
		else
		{
			this.triangles[this.triangleIndex] = a;
			this.triangles[this.triangleIndex + 1] = b;
			this.triangles[this.triangleIndex + 2] = c;
			this.triangleIndex += 3;
		}
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x00028A88 File Offset: 0x00026C88
	private Vector3[] CalculateNormals()
	{
		Vector3[] array = new Vector3[this.vertices.Length];
		int num = this.triangles.Length / 3;
		for (int i = 0; i < num; i++)
		{
			int num2 = i * 3;
			int num3 = this.triangles[num2];
			int num4 = this.triangles[num2 + 1];
			int num5 = this.triangles[num2 + 2];
			Vector3 b = this.SurfaceNormalFromIndices(num3, num4, num5);
			array[num3] += b;
			array[num4] += b;
			array[num5] += b;
		}
		int num6 = this.borderTriangles.Length / 3;
		for (int j = 0; j < num6; j++)
		{
			int num7 = j * 3;
			int num8 = this.borderTriangles[num7];
			int num9 = this.borderTriangles[num7 + 1];
			int num10 = this.borderTriangles[num7 + 2];
			Vector3 b2 = this.SurfaceNormalFromIndices(num8, num9, num10);
			if (num8 >= 0)
			{
				array[num8] += b2;
			}
			if (num9 >= 0)
			{
				array[num9] += b2;
			}
			if (num10 >= 0)
			{
				array[num10] += b2;
			}
		}
		for (int k = 0; k < array.Length; k++)
		{
			array[k].Normalize();
		}
		return array;
	}

	// Token: 0x060005EF RID: 1519 RVA: 0x00028C2C File Offset: 0x00026E2C
	private Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
	{
		Vector3 b = (indexA >= 0) ? this.vertices[indexA] : this.borderVertices[-indexA - 1];
		Vector3 a = (indexB >= 0) ? this.vertices[indexB] : this.borderVertices[-indexB - 1];
		Vector3 a2 = (indexC >= 0) ? this.vertices[indexC] : this.borderVertices[-indexC - 1];
		Vector3 lhs = a - b;
		Vector3 rhs = a2 - b;
		return Vector3.Cross(lhs, rhs).normalized;
	}

	// Token: 0x060005F0 RID: 1520 RVA: 0x00028CF4 File Offset: 0x00026EF4
	public Mesh CreateMesh()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = this.vertices;
		mesh.triangles = this.triangles;
		mesh.uv = this.uvs;
		mesh.RecalculateNormals();
		return mesh;
	}

	// Token: 0x04000618 RID: 1560
	private Vector3[] vertices;

	// Token: 0x04000619 RID: 1561
	private int[] triangles;

	// Token: 0x0400061A RID: 1562
	private Vector2[] uvs;

	// Token: 0x0400061B RID: 1563
	private Vector3[] borderVertices;

	// Token: 0x0400061C RID: 1564
	private int[] borderTriangles;

	// Token: 0x0400061D RID: 1565
	private int triangleIndex;

	// Token: 0x0400061E RID: 1566
	private int borderTriangleIndex;
}
