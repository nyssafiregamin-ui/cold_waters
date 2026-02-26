using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x0200005B RID: 91
	[DisallowMultipleComponent]
	public abstract class OceanGridBase : OceanComponent
	{
		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x060002C6 RID: 710 RVA: 0x0000FB24 File Offset: 0x0000DD24
		// (set) Token: 0x060002C7 RID: 711 RVA: 0x0000FB2C File Offset: 0x0000DD2C
		public bool ForceRecreate { get; set; }
	}
}
