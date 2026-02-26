using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x02000170 RID: 368
	public class WakeDisplacement : MonoBehaviour
	{
		// Token: 0x06000B3E RID: 2878 RVA: 0x000A5AE8 File Offset: 0x000A3CE8
		private void Start()
		{
			if (LevelLoadManager.lowOcean)
			{
				base.enabled = false;
				return;
			}
			this.timer = 0f;
			this.wakeMesh = this.wakeMeshFilter.mesh;
			this.vertexCount = this.wakeMesh.vertexCount;
			this.vertices = new Vector3[this.vertexCount];
		}

		// Token: 0x06000B3F RID: 2879 RVA: 0x000A5B48 File Offset: 0x000A3D48
		private void FixedUpdate()
		{
			if (Time.timeScale != 1f)
			{
				return;
			}
			this.timer += Time.deltaTime;
			if (this.timer > this.interval)
			{
				this.RecalculateMesh();
				this.timer = 0f;
			}
		}

		// Token: 0x06000B40 RID: 2880 RVA: 0x000A5B9C File Offset: 0x000A3D9C
		public void RecalculateMesh()
		{
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f), 1f);
			for (int i = 0; i < this.wakeMesh.vertexCount; i++)
			{
				this.vertices[i] = base.transform.TransformPoint(this.wakeMesh.vertices[i]);
				float y = Ocean.Instance.QueryWaves(this.vertices[i].x, this.vertices[i].z) - 0.015f;
				this.vertices[i] = new Vector3(this.vertices[i].x, y, this.vertices[i].z);
				this.vertices[i] = base.transform.InverseTransformPoint(this.vertices[i]);
			}
			this.wakeMesh.vertices = this.vertices;
		}

		// Token: 0x04001232 RID: 4658
		public MeshFilter wakeMeshFilter;

		// Token: 0x04001233 RID: 4659
		public Mesh wakeMesh;

		// Token: 0x04001234 RID: 4660
		internal int vertexCount;

		// Token: 0x04001235 RID: 4661
		internal Vector3[] vertices;

		// Token: 0x04001236 RID: 4662
		public float interval;

		// Token: 0x04001237 RID: 4663
		private float timer;
	}
}
