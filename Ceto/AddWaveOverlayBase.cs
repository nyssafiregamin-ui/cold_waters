using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto
{
	// Token: 0x02000089 RID: 137
	public abstract class AddWaveOverlayBase : MonoBehaviour
	{
		// Token: 0x06000386 RID: 902 RVA: 0x00014B98 File Offset: 0x00012D98
		protected virtual void Start()
		{
		}

		// Token: 0x06000387 RID: 903 RVA: 0x00014B9C File Offset: 0x00012D9C
		public virtual void Translate(Vector3 amount)
		{
			if (this.m_overlays != null)
			{
				IEnumerator<WaveOverlay> enumerator = this.m_overlays.GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.Position = enumerator.Current.Position + amount;
				}
			}
		}

		// Token: 0x06000388 RID: 904 RVA: 0x00014BEC File Offset: 0x00012DEC
		protected virtual void Update()
		{
			if (this.m_overlays != null)
			{
				foreach (WaveOverlay waveOverlay in this.m_overlays)
				{
					waveOverlay.UpdateOverlay();
				}
			}
		}

		// Token: 0x06000389 RID: 905 RVA: 0x00014C2C File Offset: 0x00012E2C
		protected virtual void OnEnable()
		{
			if (this.m_overlays != null)
			{
				foreach (WaveOverlay waveOverlay in this.m_overlays)
				{
					waveOverlay.Hide = false;
				}
			}
		}

		// Token: 0x0600038A RID: 906 RVA: 0x00014C6C File Offset: 0x00012E6C
		protected virtual void OnDisable()
		{
			if (this.m_overlays != null)
			{
				foreach (WaveOverlay waveOverlay in this.m_overlays)
				{
					waveOverlay.Hide = true;
				}
			}
		}

		// Token: 0x0600038B RID: 907 RVA: 0x00014CAC File Offset: 0x00012EAC
		protected virtual void OnDestroy()
		{
			if (this.m_overlays != null)
			{
				foreach (WaveOverlay waveOverlay in this.m_overlays)
				{
					waveOverlay.Kill = true;
				}
			}
		}

		// Token: 0x0600038C RID: 908 RVA: 0x00014CEC File Offset: 0x00012EEC
		protected static AnimationCurve DefaultCurve()
		{
			Keyframe[] keys = new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(0.012f, 0.98f),
				new Keyframe(0.026f, 1f),
				new Keyframe(1f, 0f)
			};
			return new AnimationCurve(keys);
		}

		// Token: 0x0600038D RID: 909 RVA: 0x00014D74 File Offset: 0x00012F74
		protected void CheckCanSampleTex(Texture tex, string name)
		{
			if (tex == null)
			{
				return;
			}
			if (!(tex is Texture2D))
			{
				Ocean.LogWarning("Can not query overlays " + name + " if texture is not Texture2D");
				return;
			}
			Texture2D texture2D = tex as Texture2D;
			try
			{
				Color pixel = texture2D.GetPixel(0, 0);
			}
			catch
			{
				Ocean.LogWarning("Can not query overlays " + name + " if read/write is not enabled");
			}
		}

		// Token: 0x040003C1 RID: 961
		protected IList<WaveOverlay> m_overlays = new List<WaveOverlay>();
	}
}
