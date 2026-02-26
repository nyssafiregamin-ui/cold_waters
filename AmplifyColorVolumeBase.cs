using System;
using AmplifyColor;
using UnityEngine;

// Token: 0x02000029 RID: 41
[AddComponentMenu("")]
public class AmplifyColorVolumeBase : MonoBehaviour
{
	// Token: 0x06000151 RID: 337 RVA: 0x00008B10 File Offset: 0x00006D10
	private void OnDrawGizmos()
	{
		if (this.ShowInSceneView)
		{
			BoxCollider component = base.GetComponent<BoxCollider>();
			BoxCollider2D component2 = base.GetComponent<BoxCollider2D>();
			if (component != null || component2 != null)
			{
				Vector3 center;
				Vector3 size;
				if (component != null)
				{
					center = component.center;
					size = component.size;
				}
				else
				{
					center = component2.offset;
					size = component2.size;
				}
				Gizmos.color = Color.green;
				Gizmos.DrawIcon(base.transform.position, "lut-volume.png", true);
				Gizmos.matrix = base.transform.localToWorldMatrix;
				Gizmos.DrawWireCube(center, size);
			}
		}
	}

	// Token: 0x06000152 RID: 338 RVA: 0x00008BBC File Offset: 0x00006DBC
	private void OnDrawGizmosSelected()
	{
		BoxCollider component = base.GetComponent<BoxCollider>();
		BoxCollider2D component2 = base.GetComponent<BoxCollider2D>();
		if (component != null || component2 != null)
		{
			Color green = Color.green;
			green.a = 0.2f;
			Gizmos.color = green;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Vector3 center;
			Vector3 size;
			if (component != null)
			{
				center = component.center;
				size = component.size;
			}
			else
			{
				center = component2.offset;
				size = component2.size;
			}
			Gizmos.DrawCube(center, size);
		}
	}

	// Token: 0x04000184 RID: 388
	public Texture2D LutTexture;

	// Token: 0x04000185 RID: 389
	public float Exposure = 1f;

	// Token: 0x04000186 RID: 390
	public float EnterBlendTime = 1f;

	// Token: 0x04000187 RID: 391
	public int Priority;

	// Token: 0x04000188 RID: 392
	public bool ShowInSceneView = true;

	// Token: 0x04000189 RID: 393
	[HideInInspector]
	public VolumeEffectContainer EffectContainer = new VolumeEffectContainer();
}
