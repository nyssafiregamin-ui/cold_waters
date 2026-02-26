using System;
using UnityEngine;

// Token: 0x02000156 RID: 342
public class SetParticleWhiteLevel : MonoBehaviour
{
	// Token: 0x06000A0E RID: 2574 RVA: 0x0007B1D0 File Offset: 0x000793D0
	private void Start()
	{
		this.particle.startColor = global::Environment.whiteLevel;
	}

	// Token: 0x04000F81 RID: 3969
	public ParticleSystem particle;
}
