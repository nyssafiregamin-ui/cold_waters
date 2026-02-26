using System;
using System.Collections.Generic;

namespace Ceto.Common.Containers.Queues
{
	// Token: 0x02000042 RID: 66
	public class SetQueue<VALUE>
	{
		// Token: 0x06000202 RID: 514 RVA: 0x0000D19C File Offset: 0x0000B39C
		public SetQueue()
		{
			this.m_dictionary = new Dictionary<VALUE, LinkedListNode<VALUE>>();
			this.m_list = new LinkedList<VALUE>();
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000D1BC File Offset: 0x0000B3BC
		public SetQueue(IEqualityComparer<VALUE> comparer)
		{
			this.m_dictionary = new Dictionary<VALUE, LinkedListNode<VALUE>>(comparer);
			this.m_list = new LinkedList<VALUE>();
		}

		// Token: 0x1700007B RID: 123
		public VALUE this[VALUE key]
		{
			get
			{
				return this.m_dictionary[key].Value;
			}
			set
			{
				this.Replace(key, value);
			}
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0000D1FC File Offset: 0x0000B3FC
		public IEnumerator<VALUE> GetEnumerator()
		{
			foreach (VALUE value in this.m_list)
			{
				yield return value;
			}
			yield break;
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0000D218 File Offset: 0x0000B418
		public bool Contains(VALUE val)
		{
			return this.m_dictionary.ContainsKey(val);
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000D228 File Offset: 0x0000B428
		public void Replace(VALUE key, VALUE val)
		{
			LinkedListNode<VALUE> linkedListNode = this.m_dictionary[key];
			linkedListNode.Value = val;
			this.m_dictionary.Remove(key);
			this.m_dictionary.Add(val, linkedListNode);
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000D264 File Offset: 0x0000B464
		public void AddFirst(VALUE val)
		{
			this.m_dictionary.Add(val, this.m_list.AddFirst(val));
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000D280 File Offset: 0x0000B480
		public void AddLast(VALUE val)
		{
			this.m_dictionary.Add(val, this.m_list.AddLast(val));
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x0600020B RID: 523 RVA: 0x0000D29C File Offset: 0x0000B49C
		public int Count
		{
			get
			{
				return this.m_dictionary.Count;
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x0600020C RID: 524 RVA: 0x0000D2AC File Offset: 0x0000B4AC
		public bool IsEmpty
		{
			get
			{
				return this.m_dictionary.Count == 0;
			}
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0000D2BC File Offset: 0x0000B4BC
		public VALUE First()
		{
			return this.m_list.First.Value;
		}

		// Token: 0x0600020E RID: 526 RVA: 0x0000D2D0 File Offset: 0x0000B4D0
		public VALUE Last()
		{
			return this.m_list.Last.Value;
		}

		// Token: 0x0600020F RID: 527 RVA: 0x0000D2E4 File Offset: 0x0000B4E4
		public VALUE RemoveFirst()
		{
			LinkedListNode<VALUE> first = this.m_list.First;
			this.m_list.RemoveFirst();
			this.m_dictionary.Remove(first.Value);
			return first.Value;
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0000D320 File Offset: 0x0000B520
		public VALUE RemoveLast()
		{
			LinkedListNode<VALUE> last = this.m_list.Last;
			this.m_list.RemoveLast();
			this.m_dictionary.Remove(last.Value);
			return last.Value;
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000D35C File Offset: 0x0000B55C
		public void Remove(VALUE val)
		{
			LinkedListNode<VALUE> node = this.m_dictionary[val];
			this.m_dictionary.Remove(val);
			this.m_list.Remove(node);
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0000D390 File Offset: 0x0000B590
		public void Clear()
		{
			this.m_dictionary.Clear();
			this.m_list.Clear();
		}

		// Token: 0x04000257 RID: 599
		private Dictionary<VALUE, LinkedListNode<VALUE>> m_dictionary;

		// Token: 0x04000258 RID: 600
		private LinkedList<VALUE> m_list;
	}
}
