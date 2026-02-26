using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x0200007E RID: 126
	public class OceanTime : IOceanTime
	{
		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000356 RID: 854 RVA: 0x00013170 File Offset: 0x00011370
		public float Now
		{
			get
			{
				return Time.time;
			}
		}
	}
}
