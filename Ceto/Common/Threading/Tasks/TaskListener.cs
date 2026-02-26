using System;

namespace Ceto.Common.Threading.Tasks
{
	// Token: 0x02000048 RID: 72
	public class TaskListener
	{
		// Token: 0x06000262 RID: 610 RVA: 0x0000E124 File Offset: 0x0000C324
		public TaskListener(ThreadedTask task)
		{
			this.m_task = task;
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000263 RID: 611 RVA: 0x0000E134 File Offset: 0x0000C334
		public ThreadedTask ListeningTask
		{
			get
			{
				return this.m_task;
			}
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000264 RID: 612 RVA: 0x0000E13C File Offset: 0x0000C33C
		// (set) Token: 0x06000265 RID: 613 RVA: 0x0000E148 File Offset: 0x0000C348
		public int Waiting
		{
			get
			{
				return this.m_waiting;
			}
			set
			{
				this.m_waiting = value;
			}
		}

		// Token: 0x06000266 RID: 614 RVA: 0x0000E154 File Offset: 0x0000C354
		public void OnFinish()
		{
			this.m_waiting--;
			if (this.m_waiting == 0 && !this.m_task.Cancelled)
			{
				this.m_task.StopWaiting();
			}
		}

		// Token: 0x04000269 RID: 617
		private ThreadedTask m_task;

		// Token: 0x0400026A RID: 618
		private volatile int m_waiting;
	}
}
