using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020000E2 RID: 226
public class TurboForest : MonoBehaviour
{
	// Token: 0x060005FF RID: 1535 RVA: 0x000295A8 File Offset: 0x000277A8
	public void GenerateTrees(int treeDensity, Vector2 elevations, float surfaceAngleThreshold, float treeBaseSize, float treeSizeRandomize, float treeShadingRandomize, Material treeMaterial, Vector3 cmin, Vector3 cmax, float castDistance)
	{
		this.elevations = elevations;
		this.isize = treeDensity;
		this.treeMaterial = treeMaterial;
		this.surfaceAngleThreshold = surfaceAngleThreshold;
		this.eachTreeBaseSize = treeBaseSize;
		this.eachTreeSizeRandomize = treeSizeRandomize;
		this.eachTreeShadingRandomize = treeShadingRandomize;
		this.cmin = cmin;
		this.cmax = cmax;
		this.qv0 *= this.eachTreeBaseSize;
		this.qv1 *= this.eachTreeBaseSize;
		this.qv2 *= this.eachTreeBaseSize;
		this.qv3 *= this.eachTreeBaseSize;
		this.GenerateTurboForest(castDistance);
	}

	// Token: 0x06000600 RID: 1536 RVA: 0x00029660 File Offset: 0x00027860
	private int Clamp(int v, int size)
	{
		if (v < 0)
		{
			return v + size;
		}
		if (v >= size)
		{
			return v - size;
		}
		return v;
	}

	// Token: 0x06000601 RID: 1537 RVA: 0x0002967C File Offset: 0x0002787C
	private void GenerateTurboForest(float castDistance)
	{
		int layer = 31;
		if (castDistance != 20f)
		{
			layer = 16;
		}
		tfQuad.quads.Clear();
		Vector3 origin = new Vector3(0f, 1020f, 0f);
		Ray ray = default(Ray);
		ray.direction = -Vector3.up;
		float num = (this.cmax.x - this.cmin.x) / (float)this.isize;
		float num2 = num * 2f;
		float num3 = 25f;
		for (int i = 0; i < this.isize; i++)
		{
			for (int j = 0; j < this.isize; j++)
			{
				origin.x = this.cmin.x + (float)i * num - num + UnityEngine.Random.value * num2;
				origin.z = this.cmin.z + (float)j * num - num + UnityEngine.Random.value * num2;
				float num4 = Mathf.PerlinNoise(origin.x / num3, origin.z / num3);
				if (num4 > 0.5f)
				{
					ray.origin = origin;
					RaycastHit raycastHit;
					if (base.GetComponent<Collider>().Raycast(ray, out raycastHit, castDistance))
					{
						if (raycastHit.point.y >= this.elevations.x && raycastHit.point.y <= this.elevations.y)
						{
							if (Vector3.Angle(raycastHit.normal, Vector3.up) <= this.surfaceAngleThreshold)
							{
								tfQuad tfQuad = new tfQuad(origin.x, raycastHit.point.y, origin.z);
								tfQuad.scale = 1f - this.eachTreeSizeRandomize + UnityEngine.Random.value * this.eachTreeSizeRandomize * 2f;
								if (tfQuad.quads.Count == 10666)
								{
									this.BuildMesh(layer);
									tfQuad.quads.Clear();
									GC.Collect();
								}
							}
						}
					}
				}
			}
		}
		if (tfQuad.quads.Count > 0)
		{
			this.BuildMesh(layer);
			tfQuad.quads.Clear();
			GC.Collect();
		}
	}

	// Token: 0x06000602 RID: 1538 RVA: 0x000298CC File Offset: 0x00027ACC
	private void BuildMesh(int layer)
	{
		if (tfQuad.quads.Count == 0)
		{
			return;
		}
		Vector3[] array = new Vector3[tfQuad.quads.Count * 4];
		Vector2[] array2 = new Vector2[tfQuad.quads.Count * 4];
		Vector2[] array3 = new Vector2[tfQuad.quads.Count * 4];
		Vector3[] array4 = new Vector3[tfQuad.quads.Count * 4];
		int[] array5 = new int[tfQuad.quads.Count * 4];
		Vector3 pos = tfQuad.quads[0].pos;
		Vector3 vector = pos;
		Vector2 vector2 = new Vector2(0f, 0f);
		for (int i = 0; i < tfQuad.quads.Count; i++)
		{
			tfQuad tfQuad = tfQuad.quads[i];
			pos.x = Mathf.Min(pos.x, tfQuad.pos.x);
			pos.y = Mathf.Min(pos.y, tfQuad.pos.y);
			pos.z = Mathf.Min(pos.z, tfQuad.pos.z);
			vector.x = Mathf.Max(vector.x, tfQuad.pos.x);
			vector.y = Mathf.Max(vector.y, tfQuad.pos.y);
			vector.z = Mathf.Max(vector.z, tfQuad.pos.z);
			int num = i * 4;
			array[num] = this.qv0 * tfQuad.scale;
			array[num + 1] = this.qv1 * tfQuad.scale;
			array[num + 2] = this.qv2 * tfQuad.scale;
			array[num + 3] = this.qv3 * tfQuad.scale;
			float z = 1f - this.eachTreeShadingRandomize + UnityEngine.Random.value * this.eachTreeShadingRandomize * 2f;
			array[num].z = z;
			array[num + 1].z = z;
			array[num + 2].z = z;
			array[num + 3].z = z;
			array2[num] = this.uv0;
			array2[num + 1] = this.uv1;
			array2[num + 2] = this.uv2;
			array2[num + 3] = this.uv3;
			array5[num] = num;
			array5[num + 1] = num + 1;
			array5[num + 2] = num + 2;
			array5[num + 3] = num + 3;
			tfQuad tfQuad2 = tfQuad;
			tfQuad2.pos.y = tfQuad2.pos.y + -this.qv0.y * tfQuad.scale;
			array4[num] = tfQuad.pos;
			array4[num + 1] = tfQuad.pos;
			array4[num + 2] = tfQuad.pos;
			array4[num + 3] = tfQuad.pos;
			vector2.x = 0f;
			vector2.y = 0f;
			int num2 = 0;
			float value = UnityEngine.Random.value;
			if (value > 0.25f)
			{
				num2 = 1;
			}
			if (value > 0.5f)
			{
				num2 = 2;
			}
			if (value > 0.75f)
			{
				num2 = 3;
			}
			if (num2 == 1)
			{
				vector2.x = 0.5f;
			}
			else if (num2 == 2)
			{
				vector2.y = 0.5f;
			}
			else if (num2 == 3)
			{
				vector2.x = 0.5f;
				vector2.y = 0.5f;
			}
			array3[num] = vector2;
			array3[num + 1] = vector2;
			array3[num + 2] = vector2;
			array3[num + 3] = vector2;
		}
		GameObject gameObject = new GameObject();
		gameObject.transform.parent = base.transform;
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		meshFilter.sharedMesh = new Mesh();
		meshFilter.sharedMesh.vertices = array;
		meshFilter.sharedMesh.normals = array4;
		meshFilter.sharedMesh.uv = array2;
		meshFilter.sharedMesh.uv2 = array3;
		meshFilter.sharedMesh.SetIndices(array5, MeshTopology.Quads, 0);
		meshFilter.sharedMesh.bounds = new Bounds((pos + vector) / 2f, vector - pos);
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = this.treeMaterial;
		meshRenderer.shadowCastingMode = ShadowCastingMode.On;
		meshRenderer.receiveShadows = false;
		meshRenderer.gameObject.layer = layer;
		if (layer == 16)
		{
			meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		}
	}

	// Token: 0x0400062F RID: 1583
	private const float frameSize = 0.5f;

	// Token: 0x04000630 RID: 1584
	public Material treeMaterial;

	// Token: 0x04000631 RID: 1585
	public float eachTreeBaseSize = 1.5f;

	// Token: 0x04000632 RID: 1586
	public float eachTreeSizeRandomize = 0.2f;

	// Token: 0x04000633 RID: 1587
	public float eachTreeShadingRandomize = 0.5f;

	// Token: 0x04000634 RID: 1588
	public int treesCount = 1;

	// Token: 0x04000635 RID: 1589
	private int isize = 100;

	// Token: 0x04000636 RID: 1590
	public Vector3 cmin;

	// Token: 0x04000637 RID: 1591
	public Vector3 cmax;

	// Token: 0x04000638 RID: 1592
	public Vector2 elevations;

	// Token: 0x04000639 RID: 1593
	public float surfaceAngleThreshold;

	// Token: 0x0400063A RID: 1594
	private Vector3 qv0 = new Vector3(-1f, -1f, 0f);

	// Token: 0x0400063B RID: 1595
	private Vector3 qv1 = new Vector3(1f, -1f, 0f);

	// Token: 0x0400063C RID: 1596
	private Vector3 qv2 = new Vector3(1f, 1f, 0f);

	// Token: 0x0400063D RID: 1597
	private Vector3 qv3 = new Vector3(-1f, 1f, 0f);

	// Token: 0x0400063E RID: 1598
	private Vector2 uv0 = new Vector2(0f, 0f);

	// Token: 0x0400063F RID: 1599
	private Vector2 uv1 = new Vector2(0.5f, 0f);

	// Token: 0x04000640 RID: 1600
	private Vector2 uv2 = new Vector2(0.5f, 0.5f);

	// Token: 0x04000641 RID: 1601
	private Vector2 uv3 = new Vector2(0f, 0.5f);
}
