using System;
using UnityEngine;

namespace Ceto.Common.Unity.Utility
{
	// Token: 0x0200004C RID: 76
	public class DisableGameObject : MonoBehaviour
	{
		// Token: 0x06000288 RID: 648 RVA: 0x0000E9CC File Offset: 0x0000CBCC
		private void Update()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x06000289 RID: 649 RVA: 0x0000E9DC File Offset: 0x0000CBDC
		private void OnEnable()
		{
			base.gameObject.SetActive(false);
		}
	}
}
