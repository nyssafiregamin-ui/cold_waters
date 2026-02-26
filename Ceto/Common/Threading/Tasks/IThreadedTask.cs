using System;
using System.Collections;
using Ceto.Common.Threading.Scheduling;

namespace Ceto.Common.Threading.Tasks
{
	// Token: 0x02000047 RID: 71
	public interface IThreadedTask
	{
		// Token: 0x17000087 RID: 135
		// (get) Token: 0x0600024F RID: 591
		// (set) Token: 0x06000250 RID: 592
		float RunTime { get; set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000251 RID: 593
		bool IsThreaded { get; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000252 RID: 594
		bool Ran { get; }

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000253 RID: 595
		bool Done { get; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000254 RID: 596
		// (set) Token: 0x06000255 RID: 597
		bool NoFinish { get; set; }

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000256 RID: 598
		bool Waiting { get; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000257 RID: 599
		// (set) Token: 0x06000258 RID: 600
		bool RunOnStopWaiting { get; set; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000259 RID: 601
		bool Started { get; }

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x0600025A RID: 602
		bool Cancelled { get; }

		// Token: 0x0600025B RID: 603
		void Reset();

		// Token: 0x0600025C RID: 604
		void Start();

		// Token: 0x0600025D RID: 605
		IEnumerator Run();

		// Token: 0x0600025E RID: 606
		void End();

		// Token: 0x0600025F RID: 607
		void Cancel();

		// Token: 0x06000260 RID: 608
		void WaitOn(ThreadedTask task);

		// Token: 0x17000090 RID: 144
		// (set) Token: 0x06000261 RID: 609
		IScheduler Scheduler { set; }
	}
}
