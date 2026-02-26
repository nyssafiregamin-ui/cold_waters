using System;
using UnityEngine;
using UnityEngine.UI;

namespace AmplifyBloom
{
	// Token: 0x0200001E RID: 30
	public class DemoUIElement : MonoBehaviour
	{
		// Token: 0x06000110 RID: 272 RVA: 0x00006D40 File Offset: 0x00004F40
		public void Init()
		{
			this.m_text = base.transform.GetComponentInChildren<Text>();
			this.m_unselectedColor = this.m_text.color;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00006D70 File Offset: 0x00004F70
		public virtual void DoAction(DemoUIElementAction action, params object[] vars)
		{
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00006D74 File Offset: 0x00004F74
		public virtual void Idle()
		{
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000113 RID: 275 RVA: 0x00006D78 File Offset: 0x00004F78
		// (set) Token: 0x06000114 RID: 276 RVA: 0x00006D80 File Offset: 0x00004F80
		public bool Select
		{
			get
			{
				return this.m_isSelected;
			}
			set
			{
				this.m_isSelected = value;
				this.m_text.color = ((!value) ? this.m_unselectedColor : this.m_selectedColor);
			}
		}

		// Token: 0x04000131 RID: 305
		private bool m_isSelected;

		// Token: 0x04000132 RID: 306
		private Text m_text;

		// Token: 0x04000133 RID: 307
		private Color m_selectedColor = new Color(1f, 1f, 1f);

		// Token: 0x04000134 RID: 308
		private Color m_unselectedColor;
	}
}
