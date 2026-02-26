using System;
using UnityEngine.UI;

namespace AmplifyBloom
{
	// Token: 0x02000020 RID: 32
	public sealed class DemoUIToggle : DemoUIElement
	{
		// Token: 0x0600011A RID: 282 RVA: 0x00006E98 File Offset: 0x00005098
		private void Start()
		{
			this.m_toggle = base.GetComponent<Toggle>();
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00006EA8 File Offset: 0x000050A8
		public override void DoAction(DemoUIElementAction action, params object[] vars)
		{
			if (!this.m_toggle.IsInteractable())
			{
				return;
			}
			if (action == DemoUIElementAction.Press)
			{
				this.m_toggle.isOn = !this.m_toggle.isOn;
			}
		}

		// Token: 0x04000138 RID: 312
		private Toggle m_toggle;
	}
}
