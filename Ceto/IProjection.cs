using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x02000063 RID: 99
	public interface IProjection
	{
		// Token: 0x06000302 RID: 770
		void UpdateProjection(Camera cam, CameraData data, bool projectSceneView);

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000303 RID: 771
		bool IsDouble { get; }
	}
}
