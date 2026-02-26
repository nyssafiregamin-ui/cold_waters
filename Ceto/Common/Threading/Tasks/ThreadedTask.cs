using System;
using System.Collections;
using System.Collections.Generic;
using Ceto.Common.Threading.Scheduling;

namespace Ceto.Common.Threading.Tasks
{
	// Token: 0x02000049 RID: 73
	public abstract class ThreadedTask : ICancelToken, IThreadedTask
	{
		// Token: 0x06000267 RID: 615 RVA: 0x0000E19C File Offset: 0x0000C39C
		protected ThreadedTask(bool isThreaded)
		{
			this.m_scheduler = null;
			this.m_isThreaded = isThreaded;
			this.m_listeners = new LinkedList<TaskListener>();
			this.m_listener = new TaskListener(this);
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000268 RID: 616 RVA: 0x0000E1E0 File Offset: 0x0000C3E0
		// (set) Token: 0x06000269 RID: 617 RVA: 0x0000E1E8 File Offset: 0x0000C3E8
		public float RunTime { get; set; }

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x0600026A RID: 618 RVA: 0x0000E1F4 File Offset: 0x0000C3F4
		public bool IsThreaded
		{
			get
			{
				return this.m_isThreaded;
			}
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x0600026B RID: 619 RVA: 0x0000E1FC File Offset: 0x0000C3FC
		public bool Done
		{
			get
			{
				return this.m_done;
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x0600026C RID: 620 RVA: 0x0000E208 File Offset: 0x0000C408
		public bool Ran
		{
			get
			{
				return this.m_ran;
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x0600026D RID: 621 RVA: 0x0000E214 File Offset: 0x0000C414
		// (set) Token: 0x0600026E RID: 622 RVA: 0x0000E220 File Offset: 0x0000C420
		public bool NoFinish
		{
			get
			{
				return this.m_noFinish;
			}
			set
			{
				this.m_noFinish = value;
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x0600026F RID: 623 RVA: 0x0000E22C File Offset: 0x0000C42C
		public bool Waiting
		{
			get
			{
				return this.m_listener.Waiting > 0;
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x06000270 RID: 624 RVA: 0x0000E23C File Offset: 0x0000C43C
		// (set) Token: 0x06000271 RID: 625 RVA: 0x0000E248 File Offset: 0x0000C448
		public bool RunOnStopWaiting
		{
			get
			{
				return this.m_runOnStopWaiting;
			}
			set
			{
				this.m_runOnStopWaiting = value;
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06000272 RID: 626 RVA: 0x0000E254 File Offset: 0x0000C454
		public bool Started
		{
			get
			{
				return this.m_started;
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000273 RID: 627 RVA: 0x0000E260 File Offset: 0x0000C460
		public bool Cancelled
		{
			get
			{
				return this.m_cancelled;
			}
		}

		// Token: 0x1700009C RID: 156
		// (set) Token: 0x06000274 RID: 628 RVA: 0x0000E26C File Offset: 0x0000C46C
		public IScheduler Scheduler
		{
			set
			{
				this.m_scheduler = value;
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06000275 RID: 629 RVA: 0x0000E278 File Offset: 0x0000C478
		protected LinkedList<TaskListener> Listeners
		{
			get
			{
				return this.m_listeners;
			}
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000276 RID: 630 RVA: 0x0000E280 File Offset: 0x0000C480
		protected TaskListener Listener
		{
			get
			{
				return this.m_listener;
			}
		}

		// Token: 0x06000277 RID: 631 RVA: 0x0000E288 File Offset: 0x0000C488
		public virtual void Start()
		{
			this.m_started = true;
		}

		// Token: 0x06000278 RID: 632 RVA: 0x0000E294 File Offset: 0x0000C494
		public virtual void Reset()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_listeners.Clear();
				this.m_listener.Waiting = 0;
				this.m_ran = false;
				this.m_done = false;
				this.m_cancelled = false;
				this.m_started = false;
				this.RunTime = 0f;
			}
		}

		// Token: 0x06000279 RID: 633
		public abstract IEnumerator Run();

		// Token: 0x0600027A RID: 634 RVA: 0x0000E31C File Offset: 0x0000C51C
		protected virtual void FinishedRunning()
		{
			this.m_ran = true;
			if (this.m_noFinish)
			{
				this.m_done = true;
			}
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_noFinish && !this.m_cancelled)
				{
					foreach (TaskListener taskListener in this.m_listeners)
					{
						taskListener.OnFinish();
					}
					this.m_listeners.Clear();
				}
			}
		}

		// Token: 0x0600027B RID: 635 RVA: 0x0000E3CC File Offset: 0x0000C5CC
		public virtual void End()
		{
			this.m_done = true;
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (!this.m_cancelled)
				{
					foreach (TaskListener taskListener in this.m_listeners)
					{
						taskListener.OnFinish();
					}
				}
				this.m_listeners.Clear();
			}
		}

		// Token: 0x0600027C RID: 636 RVA: 0x0000E458 File Offset: 0x0000C658
		public virtual void Cancel()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_cancelled = true;
				this.m_listeners.Clear();
			}
		}

		// Token: 0x0600027D RID: 637 RVA: 0x0000E4B0 File Offset: 0x0000C6B0
		public virtual void WaitOn(ThreadedTask task)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (task.Cancelled)
				{
					throw new InvalidOperationException("Can not wait on a task that is cancelled");
				}
				if (task.Done)
				{
					throw new InvalidOperationException("Can not wait on a task that is already done");
				}
				if (task.IsThreaded && task.NoFinish && !this.m_isThreaded)
				{
					throw new InvalidOperationException("A non-threaded task cant wait on a threaded task with no finish");
				}
				this.m_listener.Waiting++;
				task.Listeners.AddLast(this.m_listener);
			}
		}

		// Token: 0x0600027E RID: 638 RVA: 0x0000E570 File Offset: 0x0000C770
		public virtual void StopWaiting()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_scheduler != null && !this.m_cancelled)
				{
					this.m_scheduler.StopWaiting(this, this.m_runOnStopWaiting);
				}
			}
		}

		// Token: 0x0600027F RID: 639 RVA: 0x0000E5E4 File Offset: 0x0000C7E4
		public override string ToString()
		{
			return string.Format("[Task: isThreaded={0}, started={1}, ran={2}, done={3}, cancelled={4}]", new object[]
			{
				this.m_isThreaded,
				this.m_started,
				this.m_ran,
				this.m_done,
				this.m_cancelled
			});
		}

		// Token: 0x0400026B RID: 619
		private readonly bool m_isThreaded;

		// Token: 0x0400026C RID: 620
		private volatile bool m_done;

		// Token: 0x0400026D RID: 621
		private volatile bool m_ran;

		// Token: 0x0400026E RID: 622
		private volatile bool m_noFinish;

		// Token: 0x0400026F RID: 623
		private volatile bool m_runOnStopWaiting;

		// Token: 0x04000270 RID: 624
		private volatile bool m_started;

		// Token: 0x04000271 RID: 625
		private volatile bool m_cancelled;

		// Token: 0x04000272 RID: 626
		protected IScheduler m_scheduler;

		// Token: 0x04000273 RID: 627
		private LinkedList<TaskListener> m_listeners;

		// Token: 0x04000274 RID: 628
		private TaskListener m_listener;

		// Token: 0x04000275 RID: 629
		private readonly object m_lock = new object();
	}
}
