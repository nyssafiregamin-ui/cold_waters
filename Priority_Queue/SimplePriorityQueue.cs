using System;
using System.Collections;
using System.Collections.Generic;

namespace Priority_Queue
{
	// Token: 0x0200014B RID: 331
	public sealed class SimplePriorityQueue<T> : IPriorityQueue<T>, IEnumerable<T>, IEnumerable
	{
		// Token: 0x0600099D RID: 2461 RVA: 0x0006F590 File Offset: 0x0006D790
		public SimplePriorityQueue()
		{
			this._queue = new FastPriorityQueue<SimplePriorityQueue<T>.SimpleNode>(10);
		}

		// Token: 0x0600099E RID: 2462 RVA: 0x0006F5A8 File Offset: 0x0006D7A8
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x0600099F RID: 2463 RVA: 0x0006F5B0 File Offset: 0x0006D7B0
		private SimplePriorityQueue<T>.SimpleNode GetExistingNode(T item)
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			foreach (SimplePriorityQueue<T>.SimpleNode simpleNode in this._queue)
			{
				if (@default.Equals(simpleNode.Data, item))
				{
					return simpleNode;
				}
			}
			throw new InvalidOperationException("Item cannot be found in queue: " + item);
		}

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x060009A0 RID: 2464 RVA: 0x0006F644 File Offset: 0x0006D844
		public int Count
		{
			get
			{
				FastPriorityQueue<SimplePriorityQueue<T>.SimpleNode> queue = this._queue;
				int count;
				lock (queue)
				{
					count = this._queue.Count;
				}
				return count;
			}
		}

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x060009A1 RID: 2465 RVA: 0x0006F698 File Offset: 0x0006D898
		public T First
		{
			get
			{
				FastPriorityQueue<SimplePriorityQueue<T>.SimpleNode> queue = this._queue;
				T result;
				lock (queue)
				{
					if (this._queue.Count <= 0)
					{
						throw new InvalidOperationException("Cannot call .First on an empty queue");
					}
					SimplePriorityQueue<T>.SimpleNode first = this._queue.First;
					result = ((first == null) ? default(T) : first.Data);
				}
				return result;
			}
		}

		// Token: 0x060009A2 RID: 2466 RVA: 0x0006F724 File Offset: 0x0006D924
		public void Clear()
		{
			FastPriorityQueue<SimplePriorityQueue<T>.SimpleNode> queue = this._queue;
			lock (queue)
			{
				this._queue.Clear();
			}
		}

		// Token: 0x060009A3 RID: 2467 RVA: 0x0006F774 File Offset: 0x0006D974
		public bool Contains(T item)
		{
			FastPriorityQueue<SimplePriorityQueue<T>.SimpleNode> queue = this._queue;
			bool result;
			lock (queue)
			{
				EqualityComparer<T> @default = EqualityComparer<T>.Default;
				foreach (SimplePriorityQueue<T>.SimpleNode simpleNode in this._queue)
				{
					if (@default.Equals(simpleNode.Data, item))
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060009A4 RID: 2468 RVA: 0x0006F82C File Offset: 0x0006DA2C
		public T Dequeue()
		{
			FastPriorityQueue<SimplePriorityQueue<T>.SimpleNode> queue = this._queue;
			T data;
			lock (queue)
			{
				if (this._queue.Count <= 0)
				{
					throw new InvalidOperationException("Cannot call Dequeue() on an empty queue");
				}
				SimplePriorityQueue<T>.SimpleNode simpleNode = this._queue.Dequeue();
				data = simpleNode.Data;
			}
			return data;
		}

		// Token: 0x060009A5 RID: 2469 RVA: 0x0006F8A4 File Offset: 0x0006DAA4
		public void Enqueue(T item, double priority)
		{
			FastPriorityQueue<SimplePriorityQueue<T>.SimpleNode> queue = this._queue;
			lock (queue)
			{
				SimplePriorityQueue<T>.SimpleNode node = new SimplePriorityQueue<T>.SimpleNode(item);
				if (this._queue.Count == this._queue.MaxSize)
				{
					this._queue.Resize(this._queue.MaxSize * 2 + 1);
				}
				this._queue.Enqueue(node, priority);
			}
		}

		// Token: 0x060009A6 RID: 2470 RVA: 0x0006F930 File Offset: 0x0006DB30
		public void Remove(T item)
		{
			FastPriorityQueue<SimplePriorityQueue<T>.SimpleNode> queue = this._queue;
			lock (queue)
			{
				try
				{
					this._queue.Remove(this.GetExistingNode(item));
				}
				catch (InvalidOperationException innerException)
				{
					throw new InvalidOperationException("Cannot call Remove() on a node which is not enqueued: " + item, innerException);
				}
			}
		}

		// Token: 0x060009A7 RID: 2471 RVA: 0x0006F9C0 File Offset: 0x0006DBC0
		public void UpdatePriority(T item, double priority)
		{
			FastPriorityQueue<SimplePriorityQueue<T>.SimpleNode> queue = this._queue;
			lock (queue)
			{
				try
				{
					SimplePriorityQueue<T>.SimpleNode existingNode = this.GetExistingNode(item);
					this._queue.UpdatePriority(existingNode, priority);
				}
				catch (InvalidOperationException innerException)
				{
					throw new InvalidOperationException("Cannot call UpdatePriority() on a node which is not enqueued: " + item, innerException);
				}
			}
		}

		// Token: 0x060009A8 RID: 2472 RVA: 0x0006FA54 File Offset: 0x0006DC54
		public IEnumerator<T> GetEnumerator()
		{
			List<T> list = new List<T>();
			FastPriorityQueue<SimplePriorityQueue<T>.SimpleNode> queue = this._queue;
			lock (queue)
			{
				foreach (SimplePriorityQueue<T>.SimpleNode simpleNode in this._queue)
				{
					list.Add(simpleNode.Data);
				}
			}
			return list.GetEnumerator();
		}

		// Token: 0x060009A9 RID: 2473 RVA: 0x0006FB00 File Offset: 0x0006DD00
		public bool IsValidQueue()
		{
			FastPriorityQueue<SimplePriorityQueue<T>.SimpleNode> queue = this._queue;
			bool result;
			lock (queue)
			{
				result = this._queue.IsValidQueue();
			}
			return result;
		}

		// Token: 0x04000E79 RID: 3705
		private const int INITIAL_QUEUE_SIZE = 10;

		// Token: 0x04000E7A RID: 3706
		private readonly FastPriorityQueue<SimplePriorityQueue<T>.SimpleNode> _queue;

		// Token: 0x0200014C RID: 332
		private class SimpleNode : FastPriorityQueueNode
		{
			// Token: 0x060009AA RID: 2474 RVA: 0x0006FB54 File Offset: 0x0006DD54
			public SimpleNode(T data)
			{
				this.Data = data;
			}

			// Token: 0x17000148 RID: 328
			// (get) Token: 0x060009AB RID: 2475 RVA: 0x0006FB64 File Offset: 0x0006DD64
			// (set) Token: 0x060009AC RID: 2476 RVA: 0x0006FB6C File Offset: 0x0006DD6C
			public T Data { get; private set; }
		}
	}
}
