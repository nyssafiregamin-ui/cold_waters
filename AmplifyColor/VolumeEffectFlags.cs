using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AmplifyColor
{
	// Token: 0x02000031 RID: 49
	[Serializable]
	public class VolumeEffectFlags
	{
		// Token: 0x06000187 RID: 391 RVA: 0x0000A228 File Offset: 0x00008428
		public VolumeEffectFlags()
		{
			this.components = new List<VolumeEffectComponentFlags>();
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0000A23C File Offset: 0x0000843C
		public void AddComponent(Component c)
		{
			VolumeEffectComponentFlags volumeEffectComponentFlags;
			if ((volumeEffectComponentFlags = this.components.Find((VolumeEffectComponentFlags s) => s.componentName == c.GetType() + string.Empty)) != null)
			{
				volumeEffectComponentFlags.UpdateComponentFlags(c);
			}
			else
			{
				this.components.Add(new VolumeEffectComponentFlags(c));
			}
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0000A29C File Offset: 0x0000849C
		public void UpdateFlags(VolumeEffect effectVol)
		{
			VolumeEffectComponent comp;
			foreach (VolumeEffectComponent comp2 in effectVol.components)
			{
				comp = comp2;
				VolumeEffectComponentFlags volumeEffectComponentFlags;
				if ((volumeEffectComponentFlags = this.components.Find((VolumeEffectComponentFlags s) => s.componentName == comp.componentName)) == null)
				{
					this.components.Add(new VolumeEffectComponentFlags(comp));
				}
				else
				{
					volumeEffectComponentFlags.UpdateComponentFlags(comp);
				}
			}
		}

		// Token: 0x0600018A RID: 394 RVA: 0x0000A350 File Offset: 0x00008550
		public static void UpdateCamFlags(AmplifyColorBase[] effects, AmplifyColorVolumeBase[] volumes)
		{
			foreach (AmplifyColorBase amplifyColorBase in effects)
			{
				amplifyColorBase.EffectFlags = new VolumeEffectFlags();
				foreach (AmplifyColorVolumeBase amplifyColorVolumeBase in volumes)
				{
					VolumeEffect volumeEffect = amplifyColorVolumeBase.EffectContainer.FindVolumeEffect(amplifyColorBase);
					if (volumeEffect != null)
					{
						amplifyColorBase.EffectFlags.UpdateFlags(volumeEffect);
					}
				}
			}
		}

		// Token: 0x0600018B RID: 395 RVA: 0x0000A3C8 File Offset: 0x000085C8
		public VolumeEffect GenerateEffectData(AmplifyColorBase go)
		{
			VolumeEffect volumeEffect = new VolumeEffect(go);
			foreach (VolumeEffectComponentFlags volumeEffectComponentFlags in this.components)
			{
				if (volumeEffectComponentFlags.blendFlag)
				{
					Component component = go.GetComponent(volumeEffectComponentFlags.componentName);
					if (component != null)
					{
						volumeEffect.AddComponent(component, volumeEffectComponentFlags);
					}
				}
			}
			return volumeEffect;
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0000A464 File Offset: 0x00008664
		public VolumeEffectComponentFlags FindComponentFlags(string compName)
		{
			for (int i = 0; i < this.components.Count; i++)
			{
				if (this.components[i].componentName == compName)
				{
					return this.components[i];
				}
			}
			return null;
		}

		// Token: 0x0600018D RID: 397 RVA: 0x0000A4B8 File Offset: 0x000086B8
		public string[] GetComponentNames()
		{
			return (from r in this.components
			where r.blendFlag
			select r.componentName).ToArray<string>();
		}

		// Token: 0x040001B1 RID: 433
		public List<VolumeEffectComponentFlags> components;
	}
}
