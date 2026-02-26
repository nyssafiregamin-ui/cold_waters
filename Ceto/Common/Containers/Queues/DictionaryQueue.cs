using System;
using System.Collections.Generic;

namespace Ceto.Common.Containers.Queues
{
	// Token: 0x02000041 RID: 65
	public class DictionaryQueue<KEY, VALUE>
	{
		// Token: 0x060001F1 RID: 497 RVA: 0x0000CF58 File Offset: 0x0000B158
		public DictionaryQueue()
		{
			this.m_dictionary = new Dictionary<KEY, LinkedListNode<KeyValuePair<KEY, VALUE>>>();
			this.m_list = new LinkedList<KeyValuePair<KEY, VALUE>>();
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0000CF78 File Offset: 0x0000B178
		public DictionaryQueue(IEqualityComparer<KEY> comparer)
		{
			this.m_dictionary = new Dictionary<KEY, LinkedListNode<KeyValuePair<KEY, VALUE>>>(comparer);
			this.m_list = new LinkedList<KeyValuePair<KEY, VALUE>>();
		}

		// Token: 0x17000078 RID: 120
		public VALUE this[KEY key]
		{
			get
			{
				return this.m_dictionary[key].Value.Value;
			}
			set
			{
				this.Replace(key, value);
			}
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000CFCC File Offset: 0x0000B1CC
		public IEnumerator<VALUE> GetEnumerator()
		{
			foreach (KeyValuePair<KEY, VALUE> keyValuePair in this.m_list)
			{
				yield return keyValuePair.Value;
			}
			yield break;
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000CFE8 File Offset: 0x0000B1E8
		public bool ContainsKey(KEY key)
		{
			return this.m_dictionary.ContainsKey(key);
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000CFF8 File Offset: 0x0000B1F8
		public void Replace(KEY key, VALUE val)
		{
			LinkedListNode<KeyValuePair<KEY, VALUE>> linkedListNode = this.m_dictionary[key];
			linkedListNode.Value = new KeyValuePair<KEY, VALUE>(key, val);
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000D020 File Offset: 0x0000B220
		public void AddFirst(KEY key, VALUE val)
		{
			this.m_dictionary.Add(key, this.m_list.AddFirst(new KeyValuePair<KEY, VALUE>(key, val)));
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000D040 File Offset: 0x0000B240
		public void AddLast(KEY key, VALUE val)
		{
			this.m_dictionary.Add(key, this.m_list.AddLast(new KeyValuePair<KEY, VALUE>(key, val)));
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x060001FA RID: 506 RVA: 0x0000D060 File Offset: 0x0000B260
		public int Count
		{
			get
			{
				return this.m_dictionary.Count;
			}
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x060001FB RID: 507 RVA: 0x0000D070 File Offset: 0x0000B270
		public bool IsEmpty
		{
			get
			{
				return this.m_dictionary.Count == 0;
			}
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000D080 File Offset: 0x0000B280
		public KeyValuePair<KEY, VALUE> First()
		{
			return this.m_list.First.Value;
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000D094 File Offset: 0x0000B294
		public KeyValuePair<KEY, VALUE> Last()
		{
			return this.m_list.Last.Value;
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000D0A8 File Offset: 0x0000B2A8
		public VALUE RemoveFirst()
		{
			LinkedListNode<KeyValuePair<KEY, VALUE>> first = this.m_list.First;
			this.m_list.RemoveFirst();
			this.m_dictionary.Remove(first.Value.Key);
			return first.Value.Value;
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000D0F4 File Offset: 0x0000B2F4
		public VALUE RemoveLast()
		{
			LinkedListNode<KeyValuePair<KEY, VALUE>> last = this.m_list.Last;
			this.m_list.RemoveLast();
			this.m_dictionary.Remove(last.Value.Key);
			return last.Value.Value;
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000D140 File Offset: 0x0000B340
		public VALUE Remove(KEY key)
		{
			LinkedListNode<KeyValuePair<KEY, VALUE>> linkedListNode = this.m_dictionary[key];
			this.m_dictionary.Remove(key);
			this.m_list.Remove(linkedListNode);
			return linkedListNode.Value.Value;
		}

		// Token: 0x06000201 RID: 513 RVA: 0x0000D184 File Offset: 0x0000B384
		public void Clear()
		{
			this.m_dictionary.Clear();
			this.m_list.Clear();
		}

		// Token: 0x04000255 RID: 597
		private Dictionary<KEY, LinkedListNode<KeyValuePair<KEY, VALUE>>> m_dictionary;

		// Token: 0x04000256 RID: 598
		private LinkedList<KeyValuePair<KEY, VALUE>> m_list;
	}
}
