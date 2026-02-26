using System;
using UnityEngine;

// Token: 0x020000EB RID: 235
public class AnimatedProjector : MonoBehaviour
{
	// Token: 0x0600063C RID: 1596 RVA: 0x0002CC38 File Offset: 0x0002AE38
	private void Start()
	{
		this.projector = base.GetComponent<Projector>();
		this.NextFrame();
		base.InvokeRepeating("NextFrame", 1f / this.fps, 1f / this.fps);
	}

	// Token: 0x0600063D RID: 1597 RVA: 0x0002CC7C File Offset: 0x0002AE7C
	private void NextFrame()
	{
		this.projector.material.SetTexture("_ShadowTex", this.frames[this.frameIndex]);
		this.frameIndex = (this.frameIndex + 1) % this.frames.Length;
	}

	// Token: 0x0400069C RID: 1692
	public float fps = 30f;

	// Token: 0x0400069D RID: 1693
	public Texture2D[] frames;

	// Token: 0x0400069E RID: 1694
	private int frameIndex;

	// Token: 0x0400069F RID: 1695
	private Projector projector;
}
