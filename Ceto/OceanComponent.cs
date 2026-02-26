using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x02000067 RID: 103
	[RequireComponent(typeof(Ocean))]
	public abstract class OceanComponent : MonoBehaviour
	{
		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000344 RID: 836 RVA: 0x00012FF0 File Offset: 0x000111F0
		// (set) Token: 0x06000345 RID: 837 RVA: 0x00012FF8 File Offset: 0x000111F8
		public bool WasError { get; protected set; }

		// Token: 0x06000346 RID: 838 RVA: 0x00013004 File Offset: 0x00011204
		protected virtual void Awake()
		{
			try
			{
				this.m_ocean = base.GetComponent<Ocean>();
				this.m_ocean.Register(this);
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				this.WasError = true;
				base.enabled = false;
			}
		}

		// Token: 0x06000347 RID: 839 RVA: 0x0001306C File Offset: 0x0001126C
		protected virtual void OnEnable()
		{
			if (this.WasError || this.m_ocean == null || this.m_ocean.WasError)
			{
				base.enabled = false;
			}
		}

		// Token: 0x06000348 RID: 840 RVA: 0x000130A4 File Offset: 0x000112A4
		protected virtual void OnDisable()
		{
		}

		// Token: 0x06000349 RID: 841 RVA: 0x000130A8 File Offset: 0x000112A8
		protected virtual void OnDestroy()
		{
			try
			{
				this.m_ocean.Deregister(this);
			}
			catch (Exception ex)
			{
				Ocean.LogError(ex.ToString());
				this.WasError = true;
				base.enabled = false;
			}
		}

		// Token: 0x0600034A RID: 842 RVA: 0x00013104 File Offset: 0x00011304
		public virtual void OceanOnPreRender(Camera cam, CameraData data)
		{
		}

		// Token: 0x0600034B RID: 843 RVA: 0x00013108 File Offset: 0x00011308
		public virtual void OceanOnPreCull(Camera cam, CameraData data)
		{
		}

		// Token: 0x0600034C RID: 844 RVA: 0x0001310C File Offset: 0x0001130C
		public virtual void OceanOnPostRender(Camera cam, CameraData data)
		{
		}

		// Token: 0x040002FA RID: 762
		protected Ocean m_ocean;
	}
}
