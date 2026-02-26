using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Ceto.Common.Threading.Tasks;

namespace Ceto.Common.Threading.Scheduling
{
	// Token: 0x02000045 RID: 69
	public class Scheduler : IScheduler
	{
		// Token: 0x06000224 RID: 548 RVA: 0x0000D3A8 File Offset: 0x0000B5A8
		public Scheduler()
		{
			this.MaxWaitTime = 1000f;
			this.MinWaitTime = 100f;
			this.m_coroutine = null;
			this.MaxTasksPerUpdate = 100;
			this.MaxFinishPerUpdate = 100;
			this.m_scheduledTasks = new LinkedList<IThreadedTask>();
			this.m_finishedTasks = new LinkedList<IThreadedTask>();
			this.m_runningTasks = new LinkedList<IThreadedTask>();
			this.m_waitingTasks = new LinkedList<IThreadedTask>();
			this.m_haveRan = new LinkedList<IThreadedTask>();
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000D42C File Offset: 0x0000B62C
		public Scheduler(int maxTasksPerUpdate, int maxFinishPerUpdate, ICoroutine coroutine)
		{
			this.MaxWaitTime = 1000f;
			this.MinWaitTime = 100f;
			this.m_coroutine = coroutine;
			this.MaxTasksPerUpdate = Math.Max(1, maxTasksPerUpdate);
			this.MaxFinishPerUpdate = Math.Max(1, maxFinishPerUpdate);
			this.m_scheduledTasks = new LinkedList<IThreadedTask>();
			this.m_finishedTasks = new LinkedList<IThreadedTask>();
			this.m_runningTasks = new LinkedList<IThreadedTask>();
			this.m_waitingTasks = new LinkedList<IThreadedTask>();
			this.m_haveRan = new LinkedList<IThreadedTask>();
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000226 RID: 550 RVA: 0x0000D4B8 File Offset: 0x0000B6B8
		// (set) Token: 0x06000227 RID: 551 RVA: 0x0000D4C0 File Offset: 0x0000B6C0
		public int TasksRanThisUpdate { get; private set; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000228 RID: 552 RVA: 0x0000D4CC File Offset: 0x0000B6CC
		// (set) Token: 0x06000229 RID: 553 RVA: 0x0000D4D4 File Offset: 0x0000B6D4
		public int TasksFinishedThisUpdate { get; private set; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600022A RID: 554 RVA: 0x0000D4E0 File Offset: 0x0000B6E0
		// (set) Token: 0x0600022B RID: 555 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
		public int MaxTasksPerUpdate { get; private set; }

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600022C RID: 556 RVA: 0x0000D4F4 File Offset: 0x0000B6F4
		// (set) Token: 0x0600022D RID: 557 RVA: 0x0000D4FC File Offset: 0x0000B6FC
		public int MaxFinishPerUpdate { get; private set; }

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600022E RID: 558 RVA: 0x0000D508 File Offset: 0x0000B708
		// (set) Token: 0x0600022F RID: 559 RVA: 0x0000D510 File Offset: 0x0000B710
		public float MaxWaitTime { get; set; }

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000230 RID: 560 RVA: 0x0000D51C File Offset: 0x0000B71C
		// (set) Token: 0x06000231 RID: 561 RVA: 0x0000D524 File Offset: 0x0000B724
		public float MinWaitTime { get; set; }

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000232 RID: 562 RVA: 0x0000D530 File Offset: 0x0000B730
		// (set) Token: 0x06000233 RID: 563 RVA: 0x0000D538 File Offset: 0x0000B738
		public bool DisableMultithreading { get; set; }

		// Token: 0x17000085 RID: 133
		// (set) Token: 0x06000234 RID: 564 RVA: 0x0000D544 File Offset: 0x0000B744
		public bool ShutingDown
		{
			set
			{
				this.m_shutingDown = value;
			}
		}

		// Token: 0x06000235 RID: 565 RVA: 0x0000D550 File Offset: 0x0000B750
		public void Update()
		{
			this.TasksRanThisUpdate = 0;
			this.TasksFinishedThisUpdate = 0;
			this.FinishTasks();
			while (this.TasksRanThisUpdate < this.MaxTasksPerUpdate)
			{
				if (this.ScheduledTasks() > 0)
				{
					IThreadedTask value = this.m_scheduledTasks.First.Value;
					this.m_scheduledTasks.RemoveFirst();
					this.RunTask(value);
				}
				this.CheckForException();
				if (this.ScheduledTasks() == 0)
				{
					break;
				}
			}
			this.FinishTasks();
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0000D5D4 File Offset: 0x0000B7D4
		private void RunTask(IThreadedTask task)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.TasksRanThisUpdate++;
				if (!task.IsThreaded || this.DisableMultithreading)
				{
					task.Start();
					IEnumerator enumerator = task.Run();
					if (enumerator != null)
					{
						if (this.m_coroutine == null)
						{
							throw new InvalidOperationException("Scheduler trying to run a coroutine task when coroutine interface is null");
						}
						this.m_coroutine.RunCoroutine(enumerator);
						if (!this.IsFinishing(task))
						{
							this.m_runningTasks.AddLast(task);
						}
					}
					else
					{
						task.End();
					}
				}
				else
				{
					task.Start();
					this.m_runningTasks.AddLast(task);
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.RunThreaded), task);
				}
			}
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0000D6C0 File Offset: 0x0000B8C0
		private void RunThreaded(object o)
		{
			IThreadedTask threadedTask = o as IThreadedTask;
			if (threadedTask == null)
			{
				this.Throw(new InvalidCastException("Object is not a ITask or is null"));
			}
			else
			{
				try
				{
					threadedTask.Run();
				}
				catch (Exception e)
				{
					this.Throw(e);
				}
			}
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000D724 File Offset: 0x0000B924
		public void FinishTasks()
		{
			if (this.TasksFinishedThisUpdate >= this.MaxFinishPerUpdate)
			{
				return;
			}
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_haveRan.Clear();
				foreach (IThreadedTask threadedTask in this.m_runningTasks)
				{
					if (threadedTask.Ran)
					{
						this.m_haveRan.AddLast(threadedTask);
					}
				}
				foreach (IThreadedTask task in this.m_haveRan)
				{
					this.Finished(task);
				}
				if (this.m_finishedTasks.Count != 0)
				{
					IThreadedTask threadedTask2 = this.m_finishedTasks.First.Value;
					this.m_finishedTasks.RemoveFirst();
					while (threadedTask2 != null)
					{
						threadedTask2.End();
						this.TasksFinishedThisUpdate++;
						if (this.m_finishedTasks.Count == 0 || this.TasksFinishedThisUpdate >= this.MaxFinishPerUpdate)
						{
							threadedTask2 = null;
						}
						else
						{
							threadedTask2 = this.m_finishedTasks.First.Value;
							this.m_finishedTasks.RemoveFirst();
						}
					}
					this.m_haveRan.Clear();
				}
			}
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000D890 File Offset: 0x0000BA90
		public bool HasTasks()
		{
			return this.ScheduledTasks() > 0 || this.RunningTasks() > 0 || this.FinishingTasks() > 0 || this.WaitingTasks() > 0;
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000D8D0 File Offset: 0x0000BAD0
		public void Cancel(IThreadedTask task)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_scheduledTasks.Contains(task))
				{
					task.Cancel();
					this.m_scheduledTasks.Remove(task);
				}
				else if (this.m_waitingTasks.Contains(task))
				{
					task.Cancel();
					this.m_waitingTasks.Remove(task);
				}
			}
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000D960 File Offset: 0x0000BB60
		public void CancelAllTasks()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_scheduledTasks.Clear();
				this.m_waitingTasks.Clear();
				foreach (IThreadedTask threadedTask in this.m_runningTasks)
				{
					threadedTask.Cancel();
				}
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				bool flag = false;
				while (!flag && (float)stopwatch.ElapsedMilliseconds < this.MaxWaitTime)
				{
					flag = true;
					foreach (IThreadedTask threadedTask2 in this.m_runningTasks)
					{
						if (!threadedTask2.Ran)
						{
							flag = false;
							break;
						}
					}
				}
				while ((float)stopwatch.ElapsedMilliseconds < this.MinWaitTime)
				{
				}
				this.m_runningTasks.Clear();
				this.m_finishedTasks.Clear();
			}
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0000DAD0 File Offset: 0x0000BCD0
		public bool Contains(IThreadedTask task)
		{
			return this.IsScheduled(task) || this.IsWaiting(task) || this.IsRunning(task) || this.IsFinishing(task);
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0000DB18 File Offset: 0x0000BD18
		public int ScheduledTasks()
		{
			int result = 0;
			object @lock = this.m_lock;
			lock (@lock)
			{
				result = this.m_scheduledTasks.Count;
			}
			return result;
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0000DB6C File Offset: 0x0000BD6C
		public int RunningTasks()
		{
			int result = 0;
			object @lock = this.m_lock;
			lock (@lock)
			{
				result = this.m_runningTasks.Count;
			}
			return result;
		}

		// Token: 0x0600023F RID: 575 RVA: 0x0000DBC0 File Offset: 0x0000BDC0
		public int WaitingTasks()
		{
			int result = 0;
			object @lock = this.m_lock;
			lock (@lock)
			{
				result = this.m_waitingTasks.Count;
			}
			return result;
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0000DC14 File Offset: 0x0000BE14
		public int FinishingTasks()
		{
			int result = 0;
			object @lock = this.m_lock;
			lock (@lock)
			{
				result = this.m_finishedTasks.Count;
			}
			return result;
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0000DC68 File Offset: 0x0000BE68
		public int Tasks()
		{
			return this.RunningTasks() + this.ScheduledTasks() + this.WaitingTasks() + this.FinishingTasks();
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000DC90 File Offset: 0x0000BE90
		public bool IsScheduled(IThreadedTask task)
		{
			object @lock = this.m_lock;
			bool result;
			lock (@lock)
			{
				result = this.m_scheduledTasks.Contains(task);
			}
			return result;
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000DCE0 File Offset: 0x0000BEE0
		public bool IsRunning(IThreadedTask task)
		{
			object @lock = this.m_lock;
			bool result;
			lock (@lock)
			{
				result = this.m_runningTasks.Contains(task);
			}
			return result;
		}

		// Token: 0x06000244 RID: 580 RVA: 0x0000DD30 File Offset: 0x0000BF30
		public bool IsWaiting(IThreadedTask task)
		{
			object @lock = this.m_lock;
			bool result;
			lock (@lock)
			{
				result = this.m_waitingTasks.Contains(task);
			}
			return result;
		}

		// Token: 0x06000245 RID: 581 RVA: 0x0000DD80 File Offset: 0x0000BF80
		public bool IsFinishing(IThreadedTask task)
		{
			object @lock = this.m_lock;
			bool result;
			lock (@lock)
			{
				result = this.m_finishedTasks.Contains(task);
			}
			return result;
		}

		// Token: 0x06000246 RID: 582 RVA: 0x0000DDD0 File Offset: 0x0000BFD0
		public void Add(IThreadedTask task)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (!this.m_shutingDown)
				{
					task.Scheduler = this;
					this.m_scheduledTasks.AddLast(task);
				}
			}
		}

		// Token: 0x06000247 RID: 583 RVA: 0x0000DE38 File Offset: 0x0000C038
		public void Run(IThreadedTask task)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (!this.m_shutingDown)
				{
					task.Scheduler = this;
					if (this.TasksRanThisUpdate >= this.MaxTasksPerUpdate)
					{
						this.Add(task);
					}
					else
					{
						this.RunTask(task);
					}
				}
			}
		}

		// Token: 0x06000248 RID: 584 RVA: 0x0000DEB8 File Offset: 0x0000C0B8
		public void AddWaiting(IThreadedTask task)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (!this.m_shutingDown)
				{
					task.Scheduler = this;
					this.m_waitingTasks.AddLast(task);
				}
			}
		}

		// Token: 0x06000249 RID: 585 RVA: 0x0000DF20 File Offset: 0x0000C120
		public void Finished(IThreadedTask task)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_runningTasks.Remove(task);
				if (!this.m_shutingDown && !task.NoFinish && !task.Cancelled)
				{
					this.m_finishedTasks.AddLast(task);
				}
			}
		}

		// Token: 0x0600024A RID: 586 RVA: 0x0000DFA0 File Offset: 0x0000C1A0
		public void CheckForException()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_exception != null)
				{
					Exception exception = this.m_exception;
					this.m_exception = null;
					throw exception;
				}
			}
		}

		// Token: 0x0600024B RID: 587 RVA: 0x0000E000 File Offset: 0x0000C200
		public void Throw(Exception e)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_exception = e;
			}
		}

		// Token: 0x0600024C RID: 588 RVA: 0x0000E04C File Offset: 0x0000C24C
		public void StopWaiting(IThreadedTask task, bool run)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (!this.m_shutingDown)
				{
					this.m_waitingTasks.Remove(task);
					if (run)
					{
						this.RunTask(task);
					}
					else
					{
						this.m_scheduledTasks.AddLast(task);
					}
				}
			}
		}

		// Token: 0x0600024D RID: 589 RVA: 0x0000E0CC File Offset: 0x0000C2CC
		public void Clear()
		{
			if (this.RunningTasks() > 0)
			{
				throw new InvalidOperationException("Can not clear the scheduler when there are running tasks.");
			}
			this.m_scheduledTasks.Clear();
			this.m_runningTasks.Clear();
			this.m_finishedTasks.Clear();
			this.m_waitingTasks.Clear();
			this.m_exception = null;
		}

		// Token: 0x04000259 RID: 601
		private LinkedList<IThreadedTask> m_scheduledTasks;

		// Token: 0x0400025A RID: 602
		private LinkedList<IThreadedTask> m_finishedTasks;

		// Token: 0x0400025B RID: 603
		private LinkedList<IThreadedTask> m_runningTasks;

		// Token: 0x0400025C RID: 604
		private LinkedList<IThreadedTask> m_waitingTasks;

		// Token: 0x0400025D RID: 605
		private ICoroutine m_coroutine;

		// Token: 0x0400025E RID: 606
		private Exception m_exception;

		// Token: 0x0400025F RID: 607
		private readonly object m_lock = new object();

		// Token: 0x04000260 RID: 608
		private LinkedList<IThreadedTask> m_haveRan;

		// Token: 0x04000261 RID: 609
		private volatile bool m_shutingDown;
	}
}
