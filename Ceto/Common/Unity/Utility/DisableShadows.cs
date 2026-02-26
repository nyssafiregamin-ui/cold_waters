using System;
using UnityEngine;

namespace Ceto.Common.Unity.Utility
{
	// Token: 0x0200004D RID: 77
	public class DisableShadows : MonoBehaviour
	{
		// Token: 0x0600028B RID: 651 RVA: 0x0000E9F4 File Offset: 0x0000CBF4
		private void Start()
		{
		}

		// Token: 0x0600028C RID: 652 RVA: 0x0000E9F8 File Offset: 0x0000CBF8
		private void OnPreRender()
		{
			this.storedShadowDistance = QualitySettings.shadowDistance;
			QualitySettings.shadowDistance = 0f;
		}

		// Token: 0x0600028D RID: 653 RVA: 0x0000EA10 File Offset: 0x0000CC10
		private void OnPostRender()
		{
			QualitySettings.shadowDistance = this.storedShadowDistance;
		}

		// Token: 0x04000279 RID: 633
		private float storedShadowDistance;
	}
}
