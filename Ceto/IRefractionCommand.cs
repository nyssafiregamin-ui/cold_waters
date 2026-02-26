using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ceto
{
	// Token: 0x020000BB RID: 187
	public interface IRefractionCommand
	{
		// Token: 0x17000129 RID: 297
		// (get) Token: 0x06000568 RID: 1384
		// (set) Token: 0x06000569 RID: 1385
		REFRACTION_RESOLUTION Resolution { get; set; }

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x0600056A RID: 1386
		// (set) Token: 0x0600056B RID: 1387
		CameraEvent Event { get; set; }

		// Token: 0x0600056C RID: 1388
		CommandBuffer Create(Camera cam);

		// Token: 0x0600056D RID: 1389
		void Remove(Camera cam);

		// Token: 0x0600056E RID: 1390
		void RemoveAll();

		// Token: 0x0600056F RID: 1391
		bool Matches(Camera cam);
	}
}
