using System;
using UnityEngine;

namespace Ceto.Common.Unity.Utility
{
	// Token: 0x0200004B RID: 75
	public class DisableFog : MonoBehaviour
	{
		// Token: 0x06000284 RID: 644 RVA: 0x0000E99C File Offset: 0x0000CB9C
		private void Start()
		{
		}

		// Token: 0x06000285 RID: 645 RVA: 0x0000E9A0 File Offset: 0x0000CBA0
		private void OnPreRender()
		{
			this.revertFogState = RenderSettings.fog;
			RenderSettings.fog = false;
		}

		// Token: 0x06000286 RID: 646 RVA: 0x0000E9B4 File Offset: 0x0000CBB4
		private void OnPostRender()
		{
			RenderSettings.fog = this.revertFogState;
		}

		// Token: 0x04000278 RID: 632
		private bool revertFogState;
	}
}
