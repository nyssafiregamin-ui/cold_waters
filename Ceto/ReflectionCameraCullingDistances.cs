using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x02000098 RID: 152
	[AddComponentMenu("Ceto/Camera/ReflectionCameraCullingDistances")]
	[RequireComponent(typeof(Camera))]
	public class ReflectionCameraCullingDistances : MonoBehaviour
	{
		// Token: 0x06000426 RID: 1062 RVA: 0x00019E2C File Offset: 0x0001802C
		private void Start()
		{
			this.m_camera = base.GetComponent<Camera>();
		}

		// Token: 0x06000427 RID: 1063 RVA: 0x00019E3C File Offset: 0x0001803C
		private void Update()
		{
			if (Ocean.Instance == null || this.distances.Length != 32)
			{
				return;
			}
			CameraData cameraData = Ocean.Instance.FindCameraData(this.m_camera);
			if (cameraData.reflection == null)
			{
				return;
			}
			Camera cam = cameraData.reflection.cam;
			cam.layerCullDistances = this.distances;
			cam.layerCullSpherical = this.sphericalCulling;
		}

		// Token: 0x04000459 RID: 1113
		public bool sphericalCulling = true;

		// Token: 0x0400045A RID: 1114
		public float[] distances = new float[32];

		// Token: 0x0400045B RID: 1115
		private Camera m_camera;
	}
}
