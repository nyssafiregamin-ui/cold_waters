using System;
using System.Collections;
using System.Collections.Generic;

namespace Priority_Queue
{
	// Token: 0x02000148 RID: 328
	public sealed class FastPriorityQueue<T> : IPriorityQueue<T>, IEnumerable<T>, IEnumerable where T : FastPriorityQueueNode
	{
		// Token: 0x0600097B RID: 2427 RVA: 0x0006EF7C File Offset: 0x0006D17C
		public FastPriorityQueue(int maxNodes)
		{
			this._numNodes = 0;
			this._nodes = new T[maxNodes + 1];
			this._numNodesEverEnqueued = 0L;
		}

		// Token: 0x0600097C RID: 2428 RVA: 0x0006EFA4 File Offset: 0x0006D1A4
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x0600097D RID: 2429 RVA: 0x0006EFAC File Offset: 0x0006D1AC
		public int Count
		{
			get
			{
				return this._numNodes;
			}
		}

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x0600097E RID: 2430 RVA: 0x0006EFB4 File Offset: 0x0006D1B4
		public int MaxSize
		{
			get
			{
				return this._nodes.Length - 1;
			}
		}

		// Token: 0x0600097F RID: 2431 RVA: 0x0006EFC0 File Offset: 0x0006D1C0
		public void Clear()
		{
			Array.Clear(this._nodes, 1, this._numNodes);
			this._numNodes = 0;
		}

		// Token: 0x06000980 RID: 2432 RVA: 0x0006EFDC File Offset: 0x0006D1DC
		public bool Contains(T node)
		{
			return this._nodes[node.QueueIndex] == node;
		}

		// Token: 0x06000981 RID: 2433 RVA: 0x0006F004 File Offset: 0x0006D204
		public void Enqueue(T node, double priority)
		{
			node.Priority = priority;
			this._numNodes++;
			this._nodes[this._numNodes] = node;
			node.QueueIndex = this._numNodes;
			long numNodesEverEnqueued;
			this._numNodesEverEnqueued = (numNodesEverEnqueued = this._numNodesEverEnqueued) + 1L;
			node.InsertionIndex = numNodesEverEnqueued;
			this.CascadeUp(this._nodes[this._numNodes]);
		}

		// Token: 0x06000982 RID: 2434 RVA: 0x0006F088 File Offset: 0x0006D288
		private void Swap(T node1, T node2)
		{
			this._nodes[node1.QueueIndex] = node2;
			this._nodes[node2.QueueIndex] = node1;
			int queueIndex = node1.QueueIndex;
			node1.QueueIndex = node2.QueueIndex;
			node2.QueueIndex = queueIndex;
		}

		// Token: 0x06000983 RID: 2435 RVA: 0x0006F100 File Offset: 0x0006D300
		private void CascadeUp(T node)
		{
			for (int i = node.QueueIndex / 2; i >= 1; i = node.QueueIndex / 2)
			{
				T t = this._nodes[i];
				if (this.HasHigherPriority(t, node))
				{
					break;
				}
				this.Swap(node, t);
			}
		}

		// Token: 0x06000984 RID: 2436 RVA: 0x0006F160 File Offset: 0x0006D360
		private void CascadeDown(T node)
		{
			int num = node.QueueIndex;
			for (;;)
			{
				T t = node;
				int num2 = 2 * num;
				if (num2 > this._numNodes)
				{
					break;
				}
				T t2 = this._nodes[num2];
				if (this.HasHigherPriority(t2, t))
				{
					t = t2;
				}
				int num3 = num2 + 1;
				if (num3 <= this._numNodes)
				{
					T t3 = this._nodes[num3];
					if (this.HasHigherPriority(t3, t))
					{
						t = t3;
					}
				}
				if (t == node)
				{
					goto IL_D1;
				}
				this._nodes[num] = t;
				int queueIndex = t.QueueIndex;
				t.QueueIndex = num;
				num = queueIndex;
			}
			node.QueueIndex = num;
			this._nodes[num] = node;
			return;
			IL_D1:
			node.QueueIndex = num;
			this._nodes[num] = node;
		}

		// Token: 0x06000985 RID: 2437 RVA: 0x0006F264 File Offset: 0x0006D464
		private bool HasHigherPriority(T higher, T lower)
		{
			return higher.Priority < lower.Priority || (higher.Priority == lower.Priority && higher.InsertionIndex < lower.InsertionIndex);
		}

		// Token: 0x06000986 RID: 2438 RVA: 0x0006F2D4 File Offset: 0x0006D4D4
		public T Dequeue()
		{
			T t = this._nodes[1];
			this.Remove(t);
			return t;
		}

		// Token: 0x06000987 RID: 2439 RVA: 0x0006F2F8 File Offset: 0x0006D4F8
		public void Resize(int maxNodes)
		{
			T[] array = new T[maxNodes + 1];
			int num = Math.Min(maxNodes, this._numNodes);
			for (int i = 1; i <= num; i++)
			{
				array[i] = this._nodes[i];
			}
			this._nodes = array;
		}

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x06000988 RID: 2440 RVA: 0x0006F348 File Offset: 0x0006D548
		public T First
		{
			get
			{
				return this._nodes[1];
			}
		}

		// Token: 0x06000989 RID: 2441 RVA: 0x0006F358 File Offset: 0x0006D558
		public void UpdatePriority(T node, double priority)
		{
			node.Priority = priority;
			this.OnNodeUpdated(node);
		}

		// Token: 0x0600098A RID: 2442 RVA: 0x0006F370 File Offset: 0x0006D570
		private void OnNodeUpdated(T node)
		{
			int num = node.QueueIndex / 2;
			T lower = this._nodes[num];
			if (num > 0 && this.HasHigherPriority(node, lower))
			{
				this.CascadeUp(node);
			}
			else
			{
				this.CascadeDown(node);
			}
		}

		// Token: 0x0600098B RID: 2443 RVA: 0x0006F3C4 File Offset: 0x0006D5C4
		public void Remove(T node)
		{
			if (node.QueueIndex == this._numNodes)
			{
				this._nodes[this._numNodes] = (T)((object)null);
				this._numNodes--;
				return;
			}
			T t = this._nodes[this._numNodes];
			this.Swap(node, t);
			this._nodes[this._numNodes] = (T)((object)null);
			this._numNodes--;
			this.OnNodeUpdated(t);
		}

		// Token: 0x0600098C RID: 2444 RVA: 0x0006F458 File Offset: 0x0006D658
		public IEnumerator<T> GetEnumerator()
		{
			for (int i = 1; i <= this._numNodes; i++)
			{
				yield return this._nodes[i];
			}
			yield break;
		}

		// Token: 0x0600098D RID: 2445 RVA: 0x0006F474 File Offset: 0x0006D674
		public bool IsValidQueue()
		{
			for (int i = 1; i < this._nodes.Length; i++)
			{
				if (this._nodes[i] != null)
				{
					int num = 2 * i;
					if (num < this._nodes.Length && this._nodes[num] != null && this.HasHigherPriority(this._nodes[num], this._nodes[i]))
					{
						return false;
					}
					int num2 = num + 1;
					if (num2 < this._nodes.Length && this._nodes[num2] != null && this.HasHigherPriority(this._nodes[num2], this._nodes[i]))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x04000E73 RID: 3699
		private int _numNodes;

		// Token: 0x04000E74 RID: 3700
		private T[] _nodes;

		// Token: 0x04000E75 RID: 3701
		private long _numNodesEverEnqueued;
	}
}
