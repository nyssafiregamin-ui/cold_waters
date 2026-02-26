using System;
using UnityEngine;

namespace mset
{
	// Token: 0x020000E5 RID: 229
	public class Logo : MonoBehaviour
	{
		// Token: 0x06000608 RID: 1544 RVA: 0x00029EB4 File Offset: 0x000280B4
		private void Reset()
		{
			this.logoTexture = (Resources.Load("renderedLogo") as Texture2D);
		}

		// Token: 0x06000609 RID: 1545 RVA: 0x00029ECC File Offset: 0x000280CC
		private void Start()
		{
		}

		// Token: 0x0600060A RID: 1546 RVA: 0x00029ED0 File Offset: 0x000280D0
		private void updateTexRect()
		{
			if (this.logoTexture)
			{
				float num = (float)this.logoTexture.width;
				float num2 = (float)this.logoTexture.height;
				float num3 = 0f;
				float num4 = 0f;
				if (base.GetComponent<Camera>())
				{
					num3 = (float)base.GetComponent<Camera>().pixelWidth;
					num4 = (float)base.GetComponent<Camera>().pixelHeight;
				}
				else if (Camera.main)
				{
					num3 = (float)Camera.main.pixelWidth;
					num4 = (float)Camera.main.pixelHeight;
				}
				else if (Camera.current)
				{
				}
				float num5 = this.logoPixelOffset.x + this.logoPercentOffset.x * num3 * 0.01f;
				float num6 = this.logoPixelOffset.y + this.logoPercentOffset.y * num4 * 0.01f;
				switch (this.placement)
				{
				case Corner.TopLeft:
					this.texRect.x = num5;
					this.texRect.y = num6;
					break;
				case Corner.TopRight:
					this.texRect.x = num3 - num5 - num;
					this.texRect.y = num6;
					break;
				case Corner.BottomLeft:
					this.texRect.x = num5;
					this.texRect.y = num4 - num6 - num2;
					break;
				case Corner.BottomRight:
					this.texRect.x = num3 - num5 - num;
					this.texRect.y = num4 - num6 - num2;
					break;
				}
				this.texRect.width = num;
				this.texRect.height = num2;
			}
		}

		// Token: 0x0600060B RID: 1547 RVA: 0x0002A088 File Offset: 0x00028288
		private void OnGUI()
		{
			this.updateTexRect();
			if (this.logoTexture)
			{
				GUI.color = this.color;
				GUI.DrawTexture(this.texRect, this.logoTexture);
			}
		}

		// Token: 0x04000647 RID: 1607
		public Texture2D logoTexture;

		// Token: 0x04000648 RID: 1608
		public Color color = Color.white;

		// Token: 0x04000649 RID: 1609
		public Vector2 logoPixelOffset = new Vector2(0f, 0f);

		// Token: 0x0400064A RID: 1610
		public Vector2 logoPercentOffset = new Vector2(0f, 0f);

		// Token: 0x0400064B RID: 1611
		public Corner placement = Corner.BottomLeft;

		// Token: 0x0400064C RID: 1612
		private Rect texRect = new Rect(0f, 0f, 0f, 0f);
	}
}
