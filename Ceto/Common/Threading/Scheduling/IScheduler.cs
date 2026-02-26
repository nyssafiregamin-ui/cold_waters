using System;
using Ceto.Common.Threading.Tasks;

namespace Ceto.Common.Threading.Scheduling
{
	// Token: 0x02000044 RID: 68
	public interface IScheduler
	{
		// Token: 0x06000214 RID: 532
		int ScheduledTasks();

		// Token: 0x06000215 RID: 533
		int RunningTasks();

		// Token: 0x06000216 RID: 534
		int WaitingTasks();

		// Token: 0x06000217 RID: 535
		int FinishingTasks();

		// Token: 0x06000218 RID: 536
		int Tasks();

		// Token: 0x06000219 RID: 537
		bool HasTasks();

		// Token: 0x0600021A RID: 538
		void Cancel(IThreadedTask task);

		// Token: 0x0600021B RID: 539
		bool Contains(IThreadedTask task);

		// Token: 0x0600021C RID: 540
		bool IsScheduled(IThreadedTask task);

		// Token: 0x0600021D RID: 541
		bool IsRunning(IThreadedTask task);

		// Token: 0x0600021E RID: 542
		bool IsWaiting(IThreadedTask task);

		// Token: 0x0600021F RID: 543
		bool IsFinishing(IThreadedTask task);

		// Token: 0x06000220 RID: 544
		void Add(IThreadedTask task);

		// Token: 0x06000221 RID: 545
		void Run(IThreadedTask task);

		// Token: 0x06000222 RID: 546
		void AddWaiting(IThreadedTask task);

		// Token: 0x06000223 RID: 547
		void StopWaiting(IThreadedTask task, bool run);
	}
}
