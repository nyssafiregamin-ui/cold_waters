using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x0200009D RID: 157
	public abstract class WaveSpectrumBuffer
	{
		// Token: 0x06000449 RID: 1097 RVA: 0x0001B610 File Offset: 0x00019810
		public WaveSpectrumBuffer()
		{
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x0600044A RID: 1098
		public abstract bool Done { get; }

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x0600044B RID: 1099
		public abstract int Size { get; }

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x0600044C RID: 1100
		public abstract bool IsGPU { get; }

		// Token: 0x0600044D RID: 1101
		public abstract Texture GetTexture(int idx);

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x0600044E RID: 1102 RVA: 0x0001B618 File Offset: 0x00019818
		// (set) Token: 0x0600044F RID: 1103 RVA: 0x0001B620 File Offset: 0x00019820
		public float TimeValue { get; protected set; }

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x06000450 RID: 1104 RVA: 0x0001B62C File Offset: 0x0001982C
		// (set) Token: 0x06000451 RID: 1105 RVA: 0x0001B634 File Offset: 0x00019834
		public bool HasRun { get; protected set; }

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000452 RID: 1106 RVA: 0x0001B640 File Offset: 0x00019840
		// (set) Token: 0x06000453 RID: 1107 RVA: 0x0001B648 File Offset: 0x00019848
		public bool BeenSampled { get; set; }

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000454 RID: 1108 RVA: 0x0001B654 File Offset: 0x00019854
		// (set) Token: 0x06000455 RID: 1109 RVA: 0x0001B65C File Offset: 0x0001985C
		public Material InitMaterial { get; set; }

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000456 RID: 1110 RVA: 0x0001B668 File Offset: 0x00019868
		// (set) Token: 0x06000457 RID: 1111 RVA: 0x0001B670 File Offset: 0x00019870
		public int InitPass { get; set; }

		// Token: 0x06000458 RID: 1112
		protected abstract void Initilize(WaveSpectrumCondition condition, float time);

		// Token: 0x06000459 RID: 1113
		public abstract void Run(WaveSpectrumCondition condition, float time);

		// Token: 0x0600045A RID: 1114
		public abstract void EnableBuffer(int idx);

		// Token: 0x0600045B RID: 1115
		public abstract void DisableBuffer(int idx);

		// Token: 0x0600045C RID: 1116
		public abstract int EnabledBuffers();

		// Token: 0x0600045D RID: 1117
		public abstract bool IsEnabledBuffer(int i);

		// Token: 0x0600045E RID: 1118 RVA: 0x0001B67C File Offset: 0x0001987C
		public virtual void Release()
		{
		}

		// Token: 0x0600045F RID: 1119 RVA: 0x0001B680 File Offset: 0x00019880
		public virtual void EnableSampling()
		{
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x0001B684 File Offset: 0x00019884
		public virtual void DisableSampling()
		{
		}
	}
}
