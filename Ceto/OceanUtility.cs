using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x0200007F RID: 127
	public static class OceanUtility
	{
		// Token: 0x06000357 RID: 855 RVA: 0x00013178 File Offset: 0x00011378
		public static int ShowLayer(int mask, string layer)
		{
			return mask | 1 << LayerMask.NameToLayer(layer);
		}

		// Token: 0x06000358 RID: 856 RVA: 0x00013188 File Offset: 0x00011388
		public static int HideLayer(int mask, string layer)
		{
			return mask & ~(1 << LayerMask.NameToLayer(layer));
		}

		// Token: 0x06000359 RID: 857 RVA: 0x00013198 File Offset: 0x00011398
		public static int ToggleLayer(int mask, string layer)
		{
			return mask ^ 1 << LayerMask.NameToLayer(layer);
		}

		// Token: 0x0600035A RID: 858 RVA: 0x000131A8 File Offset: 0x000113A8
		public static int ShowLayer(int mask, LayerMask layer)
		{
			return mask | 1 << layer;
		}

		// Token: 0x0600035B RID: 859 RVA: 0x000131B8 File Offset: 0x000113B8
		public static int HideLayer(int mask, LayerMask layer)
		{
			return mask & ~(1 << layer);
		}

		// Token: 0x0600035C RID: 860 RVA: 0x000131C8 File Offset: 0x000113C8
		public static int ToggleLayer(int mask, LayerMask layer)
		{
			return mask ^ 1 << layer;
		}

		// Token: 0x0600035D RID: 861 RVA: 0x000131D8 File Offset: 0x000113D8
		public static Mesh CreateQuadMesh()
		{
			Vector3[] array = new Vector3[4];
			Vector2[] array2 = new Vector2[4];
			int[] triangles = new int[]
			{
				0,
				2,
				1,
				2,
				3,
				1
			};
			array[0] = new Vector3(-1f, 0f, -1f);
			array[1] = new Vector3(1f, 0f, -1f);
			array[2] = new Vector3(-1f, 0f, 1f);
			array[3] = new Vector3(1f, 0f, 1f);
			array2[0] = new Vector2(0f, 0f);
			array2[1] = new Vector2(1f, 0f);
			array2[2] = new Vector2(0f, 1f);
			array2[3] = new Vector2(1f, 1f);
			return new Mesh
			{
				vertices = array,
				uv = array2,
				triangles = triangles
			};
		}
	}
}
