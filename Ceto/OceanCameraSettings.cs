using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x02000066 RID: 102
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Ceto/Camera/OceanCameraSettings")]
	public class OceanCameraSettings : MonoBehaviour
	{
		// Token: 0x040002F0 RID: 752
		public bool disableAllOverlays;

		// Token: 0x040002F1 RID: 753
		public OVERLAY_MAP_SIZE heightOverlaySize = OVERLAY_MAP_SIZE.HALF;

		// Token: 0x040002F2 RID: 754
		public OVERLAY_MAP_SIZE normalOverlaySize = OVERLAY_MAP_SIZE.FULL;

		// Token: 0x040002F3 RID: 755
		public OVERLAY_MAP_SIZE foamOverlaySize = OVERLAY_MAP_SIZE.FULL;

		// Token: 0x040002F4 RID: 756
		public OVERLAY_MAP_SIZE clipOverlaySize = OVERLAY_MAP_SIZE.HALF;

		// Token: 0x040002F5 RID: 757
		public bool disableReflections;

		// Token: 0x040002F6 RID: 758
		public LayerMask reflectionMask = 1;

		// Token: 0x040002F7 RID: 759
		public REFLECTION_RESOLUTION reflectionResolution = REFLECTION_RESOLUTION.HALF;

		// Token: 0x040002F8 RID: 760
		public bool disableUnderwater;

		// Token: 0x040002F9 RID: 761
		public LayerMask oceanDepthsMask = 1;
	}
}
