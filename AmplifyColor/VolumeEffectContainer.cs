using System;
using System.Collections.Generic;
using System.Linq;

namespace AmplifyColor
{
	// Token: 0x0200002E RID: 46
	[Serializable]
	public class VolumeEffectContainer
	{
		// Token: 0x06000176 RID: 374 RVA: 0x00009D84 File Offset: 0x00007F84
		public VolumeEffectContainer()
		{
			this.volumes = new List<VolumeEffect>();
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00009D98 File Offset: 0x00007F98
		public void AddColorEffect(AmplifyColorBase colorEffect)
		{
			VolumeEffect volumeEffect;
			if ((volumeEffect = this.FindVolumeEffect(colorEffect)) != null)
			{
				volumeEffect.UpdateVolume();
			}
			else
			{
				volumeEffect = new VolumeEffect(colorEffect);
				this.volumes.Add(volumeEffect);
				volumeEffect.UpdateVolume();
			}
		}

		// Token: 0x06000178 RID: 376 RVA: 0x00009DD8 File Offset: 0x00007FD8
		public VolumeEffect AddJustColorEffect(AmplifyColorBase colorEffect)
		{
			VolumeEffect volumeEffect = new VolumeEffect(colorEffect);
			this.volumes.Add(volumeEffect);
			return volumeEffect;
		}

		// Token: 0x06000179 RID: 377 RVA: 0x00009DFC File Offset: 0x00007FFC
		public VolumeEffect FindVolumeEffect(AmplifyColorBase colorEffect)
		{
			for (int i = 0; i < this.volumes.Count; i++)
			{
				if (this.volumes[i].gameObject == colorEffect)
				{
					return this.volumes[i];
				}
			}
			for (int j = 0; j < this.volumes.Count; j++)
			{
				if (this.volumes[j].gameObject != null && this.volumes[j].gameObject.SharedInstanceID == colorEffect.SharedInstanceID)
				{
					return this.volumes[j];
				}
			}
			return null;
		}

		// Token: 0x0600017A RID: 378 RVA: 0x00009EBC File Offset: 0x000080BC
		public void RemoveVolumeEffect(VolumeEffect volume)
		{
			this.volumes.Remove(volume);
		}

		// Token: 0x0600017B RID: 379 RVA: 0x00009ECC File Offset: 0x000080CC
		public AmplifyColorBase[] GetStoredEffects()
		{
			return (from r in this.volumes
			select r.gameObject).ToArray<AmplifyColorBase>();
		}

		// Token: 0x040001A7 RID: 423
		public List<VolumeEffect> volumes;
	}
}
