using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x02000097 RID: 151
	[DisallowMultipleComponent]
	public abstract class ReflectionBase : OceanComponent
	{
		// Token: 0x06000424 RID: 1060
		public abstract void RenderReflection(GameObject go);

		// Token: 0x04000458 RID: 1112
		public Func<GameObject, RenderTexture> RenderReflectionCustom;
	}
}
