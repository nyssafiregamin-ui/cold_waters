using System;
using UnityEngine.UI;

namespace AmplifyBloom
{
	// Token: 0x0200001F RID: 31
	public sealed class DemoUISlider : DemoUIElement
	{
		// Token: 0x06000116 RID: 278 RVA: 0x00006DB4 File Offset: 0x00004FB4
		private void Start()
		{
			this.m_slider = base.GetComponent<Slider>();
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00006DC4 File Offset: 0x00004FC4
		public override void DoAction(DemoUIElementAction action, params object[] vars)
		{
			if (!this.m_slider.IsInteractable())
			{
				return;
			}
			if (action == DemoUIElementAction.Slide)
			{
				float num = (float)vars[0];
				if (this.SingleStep)
				{
					if (this.m_lastStep)
					{
						return;
					}
					this.m_lastStep = true;
				}
				if (this.m_slider.wholeNumbers)
				{
					if (num > 0f)
					{
						this.m_slider.value += 1f;
					}
					else if (num < 0f)
					{
						this.m_slider.value -= 1f;
					}
				}
				else
				{
					this.m_slider.value += num;
				}
			}
		}

		// Token: 0x06000118 RID: 280 RVA: 0x00006E84 File Offset: 0x00005084
		public override void Idle()
		{
			this.m_lastStep = false;
		}

		// Token: 0x04000135 RID: 309
		public bool SingleStep;

		// Token: 0x04000136 RID: 310
		private Slider m_slider;

		// Token: 0x04000137 RID: 311
		private bool m_lastStep;
	}
}
