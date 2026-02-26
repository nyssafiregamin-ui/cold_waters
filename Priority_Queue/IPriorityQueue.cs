using System;
using System.Collections;
using System.Collections.Generic;

namespace Priority_Queue
{
	// Token: 0x0200014A RID: 330
	public interface IPriorityQueue<T> : IEnumerable<T>, IEnumerable
	{
		// Token: 0x06000995 RID: 2453
		void Enqueue(T node, double priority);

		// Token: 0x06000996 RID: 2454
		T Dequeue();

		// Token: 0x06000997 RID: 2455
		void Clear();

		// Token: 0x06000998 RID: 2456
		bool Contains(T node);

		// Token: 0x06000999 RID: 2457
		void Remove(T node);

		// Token: 0x0600099A RID: 2458
		void UpdatePriority(T node, double priority);

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x0600099B RID: 2459
		T First { get; }

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x0600099C RID: 2460
		int Count { get; }
	}
}
