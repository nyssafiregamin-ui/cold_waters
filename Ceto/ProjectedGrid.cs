using System;
using System.Collections.Generic;
using Ceto.Common.Unity.Utility;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ceto
{
	// Token: 0x0200005C RID: 92
	[AddComponentMenu("Ceto/Components/ProjectedGrid")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Ocean))]
	public class ProjectedGrid : OceanGridBase
	{
		// Token: 0x060002CA RID: 714 RVA: 0x0000FB78 File Offset: 0x0000DD78
		private void Start()
		{
			try
			{
				if (SystemInfo.graphicsShaderLevel < 30)
				{
					throw new InvalidOperationException("The projected grids needs at least SM3 to render.");
				}
				if (this.oceanTopSideMat == null)
				{
					Ocean.LogWarning("Top side material is null. There will be no top ocean mesh rendered");
				}
				if (this.oceanUnderSideMat == null)
				{
					Ocean.LogWarning("Under side material is null. There will be no under ocean mesh rendered");
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		// Token: 0x060002CB RID: 715 RVA: 0x0000FC14 File Offset: 0x0000DE14
		protected override void OnEnable()
		{
			base.OnEnable();
			if (base.WasError)
			{
				return;
			}
			try
			{
				foreach (KeyValuePair<MESH_RESOLUTION, ProjectedGrid.Grid> keyValuePair in this.m_grids)
				{
					ProjectedGrid.Grid value = keyValuePair.Value;
					this.Activate(value.top, true);
					this.Activate(value.under, true);
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		// Token: 0x060002CC RID: 716 RVA: 0x0000FCBC File Offset: 0x0000DEBC
		protected override void OnDisable()
		{
			base.OnDisable();
			try
			{
				foreach (KeyValuePair<MESH_RESOLUTION, ProjectedGrid.Grid> keyValuePair in this.m_grids)
				{
					ProjectedGrid.Grid value = keyValuePair.Value;
					this.Activate(value.top, false);
					this.Activate(value.under, false);
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		// Token: 0x060002CD RID: 717 RVA: 0x0000FD58 File Offset: 0x0000DF58
		protected override void OnDestroy()
		{
			try
			{
				foreach (KeyValuePair<MESH_RESOLUTION, ProjectedGrid.Grid> keyValuePair in this.m_grids)
				{
					ProjectedGrid.Grid value = keyValuePair.Value;
					this.ClearGrid(value);
				}
				this.m_grids.Clear();
				this.m_grids = null;
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		// Token: 0x060002CE RID: 718 RVA: 0x0000FDEC File Offset: 0x0000DFEC
		private void Update()
		{
			try
			{
				Shader.SetGlobalFloat("Ceto_GridEdgeBorder", Mathf.Max(0f, this.borderLength));
				int num = this.ResolutionToNumber(this.resolution);
				Vector2 v = new Vector2((float)num / (float)this.ScreenWidth(), (float)num / (float)this.ScreenHeight());
				Shader.SetGlobalVector("Ceto_ScreenGridSize", v);
				this.CreateGrid(this.resolution);
				foreach (KeyValuePair<MESH_RESOLUTION, ProjectedGrid.Grid> keyValuePair in this.m_grids)
				{
					ProjectedGrid.Grid value = keyValuePair.Value;
					Dictionary<MESH_RESOLUTION, ProjectedGrid.Grid>.Enumerator enumerator;
					KeyValuePair<MESH_RESOLUTION, ProjectedGrid.Grid> keyValuePair2 = enumerator.Current;
					bool flag = keyValuePair2.Key == this.resolution;
					if (flag)
					{
						this.UpdateGrid(value);
						this.Activate(value.top, true);
						this.Activate(value.under, true);
					}
					else
					{
						this.Activate(value.top, false);
						this.Activate(value.under, false);
					}
				}
				if (this.m_ocean.UnderWater == null || this.m_ocean.UnderWater.Mode == UNDERWATER_MODE.ABOVE_ONLY)
				{
					foreach (KeyValuePair<MESH_RESOLUTION, ProjectedGrid.Grid> keyValuePair3 in this.m_grids)
					{
						this.Activate(keyValuePair3.Value.under, false);
					}
				}
				if (this.oceanTopSideMat != null && this.m_ocean.UnderWater != null && this.m_ocean.UnderWater.DepthMode == DEPTH_MODE.USE_DEPTH_BUFFER && this.oceanTopSideMat.shader.isSupported && this.oceanTopSideMat.renderQueue <= 2500)
				{
					Ocean.LogWarning("Underwater depth mode must be USE_OCEAN_DEPTH_PASS if using opaque material. Underwater effect will not look correct.");
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		// Token: 0x060002CF RID: 719 RVA: 0x00010000 File Offset: 0x0000E200
		public override void OceanOnPostRender(Camera cam, CameraData data)
		{
			if (!base.enabled || cam == null || data == null)
			{
				return;
			}
			ProjectedGrid.Grid grid = null;
			this.m_grids.TryGetValue(this.resolution, out grid);
			if (grid == null)
			{
				return;
			}
			this.ResetBounds(grid);
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x00010050 File Offset: 0x0000E250
		private void Activate(IList<GameObject> list, bool active)
		{
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				if (list[i] != null)
				{
					list[i].SetActive(active);
				}
			}
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x00010098 File Offset: 0x0000E298
		private void UpdateGrid(ProjectedGrid.Grid grid)
		{
			this.ResetBounds(grid);
			int count = grid.topRenderer.Count;
			for (int i = 0; i < count; i++)
			{
				grid.topRenderer[i].receiveShadows = this.receiveShadows;
				grid.topRenderer[i].reflectionProbeUsage = this.reflectionProbes;
				if (this.oceanTopSideMat != null)
				{
					grid.topRenderer[i].sharedMaterial = this.oceanTopSideMat;
				}
			}
			count = grid.underRenderer.Count;
			for (int j = 0; j < count; j++)
			{
				grid.underRenderer[j].receiveShadows = this.receiveShadows;
				grid.underRenderer[j].reflectionProbeUsage = this.reflectionProbes;
				if (this.oceanUnderSideMat != null)
				{
					grid.underRenderer[j].sharedMaterial = this.oceanUnderSideMat;
				}
			}
			count = grid.top.Count;
			for (int k = 0; k < count; k++)
			{
				grid.top[k].transform.localPosition = Vector3.zero;
				grid.top[k].transform.localRotation = Quaternion.identity;
				grid.top[k].transform.localScale = Vector3.one;
			}
			count = grid.under.Count;
			for (int l = 0; l < count; l++)
			{
				grid.under[l].transform.localPosition = Vector3.zero;
				grid.under[l].transform.localRotation = Quaternion.identity;
				grid.under[l].transform.localScale = Vector3.one;
			}
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x0001027C File Offset: 0x0000E47C
		private void ResetBounds(ProjectedGrid.Grid grid)
		{
			float level = this.m_ocean.level;
			float num = this.m_ocean.FindMaxDisplacement(true);
			float num2 = 100000000f;
			Bounds bounds = new Bounds(Vector3.zero, new Vector3(num2, level + num, num2));
			int count = grid.topFilters.Count;
			for (int i = 0; i < count; i++)
			{
				grid.topFilters[i].mesh.bounds = bounds;
			}
			count = grid.underFilters.Count;
			for (int j = 0; j < count; j++)
			{
				grid.underFilters[j].mesh.bounds = bounds;
			}
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x00010338 File Offset: 0x0000E538
		private void UpdateBounds(GameObject go, Camera cam)
		{
			MeshFilter component = go.GetComponent<MeshFilter>();
			if (component == null)
			{
				return;
			}
			Vector3 position = cam.transform.position;
			float level = this.m_ocean.level;
			float y = this.m_ocean.FindMaxDisplacement(true);
			float num = cam.farClipPlane * 2f;
			position.y = level;
			component.mesh.bounds = new Bounds(position, new Vector3(num, y, num));
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x000103B0 File Offset: 0x0000E5B0
		private int ScreenWidth()
		{
			return Mathf.Min(Screen.width, ProjectedGrid.MAX_SCREEN_WIDTH);
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x000103C4 File Offset: 0x0000E5C4
		private int ScreenHeight()
		{
			return Mathf.Min(Screen.height, ProjectedGrid.MAX_SCREEN_HEIGHT);
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x000103D8 File Offset: 0x0000E5D8
		private void ApplyProjection(GameObject go)
		{
			try
			{
				if (base.enabled)
				{
					Camera current = Camera.current;
					if (!(current == null))
					{
						CameraData cameraData = this.m_ocean.FindCameraData(current);
						if (cameraData.projection == null)
						{
							cameraData.projection = new ProjectionData();
						}
						if (!cameraData.projection.updated)
						{
							this.m_ocean.Projection.UpdateProjection(current, cameraData, this.m_ocean.ProjectSceneView);
							Shader.SetGlobalMatrix("Ceto_Interpolation", cameraData.projection.interpolation);
							Shader.SetGlobalMatrix("Ceto_ProjectorVP", cameraData.projection.projectorVP);
						}
						this.UpdateBounds(go, current);
					}
				}
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				base.WasError = true;
				base.enabled = false;
			}
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x000104D0 File Offset: 0x0000E6D0
		private void CreateGrid(MESH_RESOLUTION meshRes)
		{
			if (!this.m_grids.ContainsKey(meshRes))
			{
				this.m_grids.Add(meshRes, new ProjectedGrid.Grid());
			}
			ProjectedGrid.Grid grid = this.m_grids[meshRes];
			int num = this.ScreenWidth();
			int num2 = this.ScreenHeight();
			int num3 = this.ResolutionToNumber(meshRes);
			int num4 = this.ChooseGroupSize(num3, this.gridGroups, num, num2);
			if (!base.ForceRecreate && grid.screenWidth == num && grid.screenHeight == num2 && grid.groups == num4)
			{
				return;
			}
			this.ClearGrid(grid);
			grid.screenWidth = num;
			grid.screenHeight = num2;
			grid.resolution = num3;
			grid.groups = num4;
			base.ForceRecreate = false;
			IList<Mesh> list = this.CreateScreenQuads(num3, num4, num, num2);
			foreach (Mesh mesh in list)
			{
				if (this.oceanTopSideMat != null)
				{
					GameObject gameObject = new GameObject("Ceto TopSide Grid LOD: " + meshRes);
					MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
					MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
					NotifyOnWillRender notifyOnWillRender = gameObject.AddComponent<NotifyOnWillRender>();
					meshFilter.sharedMesh = mesh;
					meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
					meshRenderer.receiveShadows = this.receiveShadows;
					meshRenderer.sharedMaterial = this.oceanTopSideMat;
					meshRenderer.reflectionProbeUsage = this.reflectionProbes;
					gameObject.layer = LayerMask.NameToLayer(Ocean.OCEAN_LAYER);
					gameObject.hideFlags = HideFlags.HideAndDontSave;
					notifyOnWillRender.AddAction(new Action<GameObject>(this.m_ocean.RenderReflection));
					notifyOnWillRender.AddAction(new Action<GameObject>(this.ApplyProjection));
					notifyOnWillRender.AddAction(new Action<GameObject>(this.m_ocean.RenderWaveOverlays));
					notifyOnWillRender.AddAction(new Action<GameObject>(this.m_ocean.RenderOceanMask));
					notifyOnWillRender.AddAction(new Action<GameObject>(this.m_ocean.RenderOceanDepth));
					grid.top.Add(gameObject);
					grid.topRenderer.Add(meshRenderer);
					grid.topFilters.Add(meshFilter);
				}
				if (this.oceanUnderSideMat)
				{
					GameObject gameObject2 = new GameObject("Ceto UnderSide Grid LOD: " + meshRes);
					MeshFilter meshFilter2 = gameObject2.AddComponent<MeshFilter>();
					MeshRenderer meshRenderer2 = gameObject2.AddComponent<MeshRenderer>();
					NotifyOnWillRender notifyOnWillRender2 = gameObject2.AddComponent<NotifyOnWillRender>();
					meshFilter2.sharedMesh = mesh;
					meshRenderer2.shadowCastingMode = ShadowCastingMode.Off;
					meshRenderer2.receiveShadows = this.receiveShadows;
					meshRenderer2.reflectionProbeUsage = this.reflectionProbes;
					meshRenderer2.sharedMaterial = this.oceanUnderSideMat;
					gameObject2.layer = LayerMask.NameToLayer(Ocean.OCEAN_LAYER);
					gameObject2.hideFlags = HideFlags.HideAndDontSave;
					notifyOnWillRender2.AddAction(new Action<GameObject>(this.ApplyProjection));
					notifyOnWillRender2.AddAction(new Action<GameObject>(this.m_ocean.RenderWaveOverlays));
					notifyOnWillRender2.AddAction(new Action<GameObject>(this.m_ocean.RenderOceanMask));
					notifyOnWillRender2.AddAction(new Action<GameObject>(this.m_ocean.RenderOceanDepth));
					grid.under.Add(gameObject2);
					grid.underRenderer.Add(meshRenderer2);
					grid.underFilters.Add(meshFilter2);
				}
				UnityEngine.Object.Destroy(mesh);
			}
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x0001083C File Offset: 0x0000EA3C
		private void ClearGrid(ProjectedGrid.Grid grid)
		{
			if (grid == null)
			{
				return;
			}
			grid.screenWidth = 0;
			grid.screenHeight = 0;
			grid.resolution = 0;
			grid.groups = 0;
			if (grid.top != null)
			{
				int count = grid.top.Count;
				for (int i = 0; i < count; i++)
				{
					if (!(grid.top[i] == null))
					{
						UnityEngine.Object.Destroy(grid.top[i]);
					}
				}
				grid.top.Clear();
			}
			if (grid.topFilters != null)
			{
				int count2 = grid.topFilters.Count;
				for (int j = 0; j < count2; j++)
				{
					if (!(grid.topFilters[j] == null))
					{
						UnityEngine.Object.Destroy(grid.topFilters[j].mesh);
					}
				}
				grid.topFilters.Clear();
			}
			if (grid.under != null)
			{
				int count3 = grid.under.Count;
				for (int k = 0; k < count3; k++)
				{
					if (!(grid.under[k] == null))
					{
						UnityEngine.Object.Destroy(grid.under[k]);
					}
				}
				grid.under.Clear();
			}
			if (grid.underFilters != null)
			{
				int count4 = grid.underFilters.Count;
				for (int l = 0; l < count4; l++)
				{
					if (!(grid.underFilters[l] == null))
					{
						UnityEngine.Object.Destroy(grid.underFilters[l].mesh);
					}
				}
				grid.underFilters.Clear();
			}
			if (grid.topRenderer != null)
			{
				grid.topRenderer.Clear();
			}
			if (grid.underRenderer != null)
			{
				grid.underRenderer.Clear();
			}
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x00010A38 File Offset: 0x0000EC38
		private int ResolutionToNumber(MESH_RESOLUTION resolution)
		{
			switch (resolution)
			{
			case MESH_RESOLUTION.LOW:
				return 16;
			case MESH_RESOLUTION.MEDIUM:
				return 8;
			case MESH_RESOLUTION.HIGH:
				return 4;
			case MESH_RESOLUTION.ULTRA:
				return 2;
			case MESH_RESOLUTION.EXTREME:
				return 1;
			default:
				return 16;
			}
		}

		// Token: 0x060002DA RID: 730 RVA: 0x00010A74 File Offset: 0x0000EC74
		private int GroupToNumber(GRID_GROUPS groups)
		{
			switch (groups)
			{
			case GRID_GROUPS.SINGLE:
				return -1;
			case GRID_GROUPS.LOW:
				return 512;
			case GRID_GROUPS.MEDIUM:
				return 256;
			case GRID_GROUPS.HIGH:
				return 196;
			case GRID_GROUPS.EXTREME:
				return 128;
			default:
				return 128;
			}
		}

		// Token: 0x060002DB RID: 731 RVA: 0x00010AC4 File Offset: 0x0000ECC4
		private int ChooseGroupSize(int resolution, GRID_GROUPS groups, int width, int height)
		{
			int num = this.GroupToNumber(groups);
			int num2;
			int num3;
			if (num == -1)
			{
				num2 = width / resolution;
				num3 = height / resolution;
			}
			else
			{
				num2 = num / resolution;
				num3 = num / resolution;
			}
			while (num2 * num3 > 65000)
			{
				if (groups == GRID_GROUPS.EXTREME)
				{
					throw new InvalidOperationException("Can not increase group size");
				}
				int num4 = (int)(groups + 1);
				groups = (GRID_GROUPS)num4;
				num = this.GroupToNumber(groups);
				num2 = num / resolution;
				num3 = num / resolution;
			}
			return num;
		}

		// Token: 0x060002DC RID: 732 RVA: 0x00010B38 File Offset: 0x0000ED38
		private IList<Mesh> CreateScreenQuads(int resolution, int groupSize, int width, int height)
		{
			int numVertsX;
			int numVertsY;
			int num;
			int num2;
			float num3;
			float num4;
			if (groupSize != -1)
			{
				while (width % groupSize != 0)
				{
					width++;
				}
				while (height % groupSize != 0)
				{
					height++;
				}
				numVertsX = groupSize / resolution;
				numVertsY = groupSize / resolution;
				num = width / groupSize;
				num2 = height / groupSize;
				num3 = (float)groupSize / (float)width;
				num4 = (float)groupSize / (float)height;
			}
			else
			{
				numVertsX = width / resolution;
				numVertsY = height / resolution;
				num = 1;
				num2 = 1;
				num3 = 1f;
				num4 = 1f;
			}
			List<Mesh> list = new List<Mesh>();
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num2; j++)
				{
					float ux = (float)i * num3;
					float uy = (float)j * num4;
					Mesh item = this.CreateQuad(numVertsX, numVertsY, ux, uy, num3, num4);
					list.Add(item);
				}
			}
			return list;
		}

		// Token: 0x060002DD RID: 733 RVA: 0x00010C28 File Offset: 0x0000EE28
		public Mesh CreateQuad(int numVertsX, int numVertsY, float ux, float uy, float w, float h)
		{
			Vector3[] array = new Vector3[numVertsX * numVertsY];
			Vector2[] array2 = new Vector2[numVertsX * numVertsY];
			int[] array3 = new int[numVertsX * numVertsY * 6];
			float num = 0.1f;
			for (int i = 0; i < numVertsX; i++)
			{
				for (int j = 0; j < numVertsY; j++)
				{
					Vector2 vector = new Vector3((float)i / (float)(numVertsX - 1), (float)j / (float)(numVertsY - 1));
					vector.x *= w;
					vector.x += ux;
					vector.y *= h;
					vector.y += uy;
					if (!Ocean.DISABLE_PROJECTED_GRID_BORDER)
					{
						vector.x = vector.x * (1f + num * 2f) - num;
						vector.y = vector.y * (1f + num * 2f) - num;
						Vector2 vector2 = vector;
						vector2.x = Mathf.Clamp01(vector2.x);
						vector2.y = Mathf.Clamp01(vector2.y);
						Vector2 vector3 = vector;
						if (vector3.x < 0f)
						{
							vector3.x = Mathf.Abs(vector3.x) / num;
						}
						else if (vector3.x > 1f)
						{
							vector3.x = Mathf.Max(0f, vector3.x - 1f) / num;
						}
						else
						{
							vector3.x = 0f;
						}
						if (vector3.y < 0f)
						{
							vector3.y = Mathf.Abs(vector3.y) / num;
						}
						else if (vector3.y > 1f)
						{
							vector3.y = Mathf.Max(0f, vector3.y - 1f) / num;
						}
						else
						{
							vector3.y = 0f;
						}
						vector3.x = Mathf.Pow(vector3.x, 2f);
						vector3.y = Mathf.Pow(vector3.y, 2f);
						array2[i + j * numVertsX] = vector3;
						array[i + j * numVertsX] = new Vector3(vector2.x, vector2.y, 0f);
					}
					else
					{
						array2[i + j * numVertsX] = new Vector2(0f, 0f);
						array[i + j * numVertsX] = new Vector3(vector.x, vector.y, 0f);
					}
				}
			}
			int num2 = 0;
			for (int k = 0; k < numVertsX - 1; k++)
			{
				for (int l = 0; l < numVertsY - 1; l++)
				{
					array3[num2++] = k + l * numVertsX;
					array3[num2++] = k + (l + 1) * numVertsX;
					array3[num2++] = k + 1 + l * numVertsX;
					array3[num2++] = k + (l + 1) * numVertsX;
					array3[num2++] = k + 1 + (l + 1) * numVertsX;
					array3[num2++] = k + 1 + l * numVertsX;
				}
			}
			Mesh mesh = new Mesh();
			mesh.vertices = array;
			mesh.uv = array2;
			mesh.triangles = array3;
			mesh.name = "Ceto Projected Grid Mesh";
			mesh.hideFlags = HideFlags.HideAndDontSave;
			mesh.Optimize();
			return mesh;
		}

		// Token: 0x04000298 RID: 664
		private static readonly int MAX_SCREEN_WIDTH = 2048;

		// Token: 0x04000299 RID: 665
		private static readonly int MAX_SCREEN_HEIGHT = 2048;

		// Token: 0x0400029A RID: 666
		public MESH_RESOLUTION resolution = MESH_RESOLUTION.HIGH;

		// Token: 0x0400029B RID: 667
		private GRID_GROUPS gridGroups;

		// Token: 0x0400029C RID: 668
		public bool receiveShadows;

		// Token: 0x0400029D RID: 669
		private float borderLength = 200f;

		// Token: 0x0400029E RID: 670
		private ReflectionProbeUsage reflectionProbes;

		// Token: 0x0400029F RID: 671
		public Material oceanTopSideMat;

		// Token: 0x040002A0 RID: 672
		public Material oceanUnderSideMat;

		// Token: 0x040002A1 RID: 673
		private Dictionary<MESH_RESOLUTION, ProjectedGrid.Grid> m_grids = new Dictionary<MESH_RESOLUTION, ProjectedGrid.Grid>();

		// Token: 0x0200005D RID: 93
		public class Grid
		{
			// Token: 0x040002A2 RID: 674
			public int screenWidth;

			// Token: 0x040002A3 RID: 675
			public int screenHeight;

			// Token: 0x040002A4 RID: 676
			public int resolution;

			// Token: 0x040002A5 RID: 677
			public int groups;

			// Token: 0x040002A6 RID: 678
			public IList<MeshFilter> topFilters = new List<MeshFilter>();

			// Token: 0x040002A7 RID: 679
			public IList<Renderer> topRenderer = new List<Renderer>();

			// Token: 0x040002A8 RID: 680
			public IList<GameObject> top = new List<GameObject>();

			// Token: 0x040002A9 RID: 681
			public IList<MeshFilter> underFilters = new List<MeshFilter>();

			// Token: 0x040002AA RID: 682
			public IList<Renderer> underRenderer = new List<Renderer>();

			// Token: 0x040002AB RID: 683
			public IList<GameObject> under = new List<GameObject>();
		}
	}
}
