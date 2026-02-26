using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x020000C5 RID: 197
	[RequireComponent(typeof(Camera))]
	public class AddRenderTarget : MonoBehaviour
	{
		// Token: 0x060005A8 RID: 1448 RVA: 0x000259FC File Offset: 0x00023BFC
		private void Start()
		{
			Camera component = base.GetComponent<Camera>();
			component.targetTexture = new RenderTexture(Screen.width / this.scale, Screen.height / this.scale, 24);
		}

		// Token: 0x060005A9 RID: 1449 RVA: 0x00025A38 File Offset: 0x00023C38
		private void OnGUI()
		{
			Camera component = base.GetComponent<Camera>();
			if (component.targetTexture == null)
			{
				return;
			}
			int width = component.targetTexture.width;
			int height = component.targetTexture.height;
			GUI.DrawTexture(new Rect(10f, 10f, (float)width, (float)height), component.targetTexture);
		}

		// Token: 0x04000584 RID: 1412
		public int scale = 2;
	}
}
