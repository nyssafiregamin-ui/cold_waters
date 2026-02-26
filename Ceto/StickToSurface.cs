using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x02000061 RID: 97
	[AddComponentMenu("Ceto/Buoyancy/StickToSurface")]
	public class StickToSurface : MonoBehaviour
	{
		// Token: 0x060002FF RID: 767 RVA: 0x00011610 File Offset: 0x0000F810
		private void Start()
		{
		}

		// Token: 0x06000300 RID: 768 RVA: 0x00011614 File Offset: 0x0000F814
		private void Update()
		{
			if (Ocean.Instance == null)
			{
				return;
			}
			Vector3 position = base.transform.position;
			position.y = Ocean.Instance.QueryWaves(position.x, position.z);
			base.transform.position = position;
		}
	}
}
