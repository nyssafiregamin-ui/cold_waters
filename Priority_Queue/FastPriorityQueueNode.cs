using System;

namespace Priority_Queue
{
	// Token: 0x02000149 RID: 329
	public class FastPriorityQueueNode
	{
		// Token: 0x17000141 RID: 321
		// (get) Token: 0x0600098F RID: 2447 RVA: 0x0006F554 File Offset: 0x0006D754
		// (set) Token: 0x06000990 RID: 2448 RVA: 0x0006F55C File Offset: 0x0006D75C
		public double Priority { get; set; }

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x06000991 RID: 2449 RVA: 0x0006F568 File Offset: 0x0006D768
		// (set) Token: 0x06000992 RID: 2450 RVA: 0x0006F570 File Offset: 0x0006D770
		public long InsertionIndex { get; set; }

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x06000993 RID: 2451 RVA: 0x0006F57C File Offset: 0x0006D77C
		// (set) Token: 0x06000994 RID: 2452 RVA: 0x0006F584 File Offset: 0x0006D784
		public int QueueIndex { get; set; }
	}
}
