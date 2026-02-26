using System;
using UnityEngine;

namespace Ceto.Common.Unity.Utility
{
	// Token: 0x0200005A RID: 90
	public class Wireframe : MonoBehaviour
	{
		// Token: 0x060002C1 RID: 705 RVA: 0x0000FACC File Offset: 0x0000DCCC
		private void Start()
		{
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x0000FAD0 File Offset: 0x0000DCD0
		private void Update()
		{
			if (Input.GetKeyDown(this.toggleKey))
			{
				this.on = !this.on;
			}
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x0000FAF4 File Offset: 0x0000DCF4
		private void OnPreRender()
		{
			if (this.on)
			{
				GL.wireframe = true;
			}
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x0000FB08 File Offset: 0x0000DD08
		private void OnPostRender()
		{
			if (this.on)
			{
				GL.wireframe = false;
			}
		}

		// Token: 0x04000295 RID: 661
		public bool on;

		// Token: 0x04000296 RID: 662
		public KeyCode toggleKey = KeyCode.F2;
	}
}
