using System;
using UnityEngine;

namespace Ceto.Common.Unity.Utility
{
	// Token: 0x02000056 RID: 86
	public class Quit : MonoBehaviour
	{
		// Token: 0x060002AC RID: 684 RVA: 0x0000F18C File Offset: 0x0000D38C
		private void OnGUI()
		{
			if (Input.GetKeyDown(this.quitKey))
			{
				Application.Quit();
			}
		}

		// Token: 0x04000284 RID: 644
		public KeyCode quitKey = KeyCode.Escape;
	}
}
