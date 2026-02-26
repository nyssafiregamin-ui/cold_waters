using System;
using UnityEngine;
using UnityEngine.UI;

namespace AmplifyBloom
{
	// Token: 0x0200001B RID: 27
	public class DemoFPSCounter : MonoBehaviour
	{
		// Token: 0x060000FC RID: 252 RVA: 0x00006334 File Offset: 0x00004534
		private void Start()
		{
			this.m_fpsText = base.GetComponent<Text>();
			this.m_timeleft = this.UpdateInterval;
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00006350 File Offset: 0x00004550
		private void Update()
		{
			this.m_timeleft -= Time.deltaTime;
			this.m_accum += Time.timeScale / Time.deltaTime;
			this.m_frames++;
			if ((double)this.m_timeleft <= 0.0)
			{
				this.m_fps = this.m_accum / (float)this.m_frames;
				this.m_format = string.Format("{0:F2} FPS", this.m_fps);
				this.m_fpsText.text = this.m_format;
				if (this.m_fps < 50f)
				{
					this.m_fpsText.color = Color.yellow;
				}
				else if (this.m_fps < 30f)
				{
					this.m_fpsText.color = Color.red;
				}
				else
				{
					this.m_fpsText.color = Color.green;
				}
				this.m_timeleft = this.UpdateInterval;
				this.m_accum = 0f;
				this.m_frames = 0;
			}
		}

		// Token: 0x04000110 RID: 272
		public float UpdateInterval = 0.5f;

		// Token: 0x04000111 RID: 273
		private Text m_fpsText;

		// Token: 0x04000112 RID: 274
		private float m_accum;

		// Token: 0x04000113 RID: 275
		private int m_frames;

		// Token: 0x04000114 RID: 276
		private float m_timeleft;

		// Token: 0x04000115 RID: 277
		private float m_fps;

		// Token: 0x04000116 RID: 278
		private string m_format;
	}
}
