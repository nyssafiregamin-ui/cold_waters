using System;
using System.Collections.Generic;
using Ceto.Common.Threading.Scheduling;
using Ceto.Common.Threading.Tasks;
using UnityEngine;

namespace Ceto
{
	// Token: 0x0200009E RID: 158
	public abstract class WaveSpectrumBufferCPU : WaveSpectrumBuffer
	{
		// Token: 0x06000461 RID: 1121 RVA: 0x0001B688 File Offset: 0x00019888
		public WaveSpectrumBufferCPU(int size, int numBuffers, Scheduler scheduler)
		{
			this.m_buffers = new WaveSpectrumBufferCPU.Buffer[numBuffers];
			this.m_fourier = new FourierCPU(size);
			this.m_fourierTasks = new List<ThreadedTask>();
			this.m_scheduler = scheduler;
			for (int i = 0; i < numBuffers; i++)
			{
				this.m_buffers[i] = this.CreateBuffer(size);
			}
		}

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x06000462 RID: 1122 RVA: 0x0001B6E8 File Offset: 0x000198E8
		public override bool Done
		{
			get
			{
				return this.IsDone();
			}
		}

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x06000463 RID: 1123 RVA: 0x0001B6F0 File Offset: 0x000198F0
		public override int Size
		{
			get
			{
				return this.m_fourier.size;
			}
		}

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x06000464 RID: 1124 RVA: 0x0001B700 File Offset: 0x00019900
		public override bool IsGPU
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000465 RID: 1125 RVA: 0x0001B704 File Offset: 0x00019904
		// (set) Token: 0x06000466 RID: 1126 RVA: 0x0001B70C File Offset: 0x0001990C
		public Color[] WTable { get; private set; }

		// Token: 0x06000467 RID: 1127 RVA: 0x0001B718 File Offset: 0x00019918
		private WaveSpectrumBufferCPU.Buffer CreateBuffer(int size)
		{
			WaveSpectrumBufferCPU.Buffer buffer = new WaveSpectrumBufferCPU.Buffer();
			buffer.doublePacked = true;
			buffer.data = new List<Vector4[]>();
			buffer.data.Add(new Vector4[size * size]);
			buffer.data.Add(new Vector4[size * size]);
			buffer.results = new Color[size * size];
			buffer.map = new Texture2D(size, size, TextureFormat.RGBAFloat, false, true);
			buffer.map.wrapMode = TextureWrapMode.Repeat;
			buffer.map.filterMode = FilterMode.Bilinear;
			buffer.map.hideFlags = HideFlags.HideAndDontSave;
			buffer.map.name = "Ceto Wave Spectrum CPU Buffer";
			buffer.map.SetPixels(buffer.results);
			buffer.map.Apply();
			return buffer;
		}

		// Token: 0x06000468 RID: 1128 RVA: 0x0001B7D4 File Offset: 0x000199D4
		public override void Release()
		{
			int num = this.m_buffers.Length;
			for (int i = 0; i < num; i++)
			{
				UnityEngine.Object.Destroy(this.m_buffers[i].map);
				this.m_buffers[i].map = null;
			}
		}

		// Token: 0x06000469 RID: 1129 RVA: 0x0001B81C File Offset: 0x00019A1C
		public override Texture GetTexture(int idx)
		{
			if (idx < 0 || idx >= this.m_buffers.Length)
			{
				return Texture2D.blackTexture;
			}
			if (this.m_buffers[idx].disabled)
			{
				return Texture2D.blackTexture;
			}
			return this.m_buffers[idx].map;
		}

		// Token: 0x0600046A RID: 1130 RVA: 0x0001B86C File Offset: 0x00019A6C
		public Vector4[] GetWriteBuffer(int idx)
		{
			if (idx < 0 || idx >= this.m_buffers.Length)
			{
				return null;
			}
			if (this.m_buffers[idx].disabled)
			{
				return null;
			}
			return this.m_buffers[idx].data[0];
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x0001B8B8 File Offset: 0x00019AB8
		public Vector4[] GetReadBuffer(int idx)
		{
			if (idx < 0 || idx >= this.m_buffers.Length)
			{
				return null;
			}
			if (this.m_buffers[idx].disabled)
			{
				return null;
			}
			return this.m_buffers[idx].data[1];
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x0001B904 File Offset: 0x00019B04
		public WaveSpectrumBufferCPU.Buffer GetBuffer(int idx)
		{
			if (idx < 0 || idx >= this.m_buffers.Length)
			{
				return null;
			}
			if (this.m_buffers[idx].disabled)
			{
				return null;
			}
			return this.m_buffers[idx];
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x0001B93C File Offset: 0x00019B3C
		public IList<IList<Vector4[]>> GetData(int idx)
		{
			IList<IList<Vector4[]>> list = new List<IList<Vector4[]>>(3);
			int num = this.m_buffers.Length;
			if (idx < -1 || idx >= num)
			{
				return list;
			}
			if (idx == -1)
			{
				for (int i = 0; i < num; i++)
				{
					if (!this.m_buffers[i].disabled)
					{
						list.Add(this.m_buffers[i].data);
					}
				}
			}
			else if (!this.m_buffers[idx].disabled)
			{
				list.Add(this.m_buffers[idx].data);
			}
			return list;
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x0001B9D4 File Offset: 0x00019BD4
		public IList<Color[]> GetResults(int idx)
		{
			IList<Color[]> list = new List<Color[]>(3);
			int num = this.m_buffers.Length;
			if (idx < -1 || idx >= num)
			{
				return list;
			}
			if (idx == -1)
			{
				for (int i = 0; i < num; i++)
				{
					if (!this.m_buffers[i].disabled)
					{
						list.Add(this.m_buffers[i].results);
					}
				}
			}
			else if (!this.m_buffers[idx].disabled)
			{
				list.Add(this.m_buffers[idx].results);
			}
			return list;
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x0001BA6C File Offset: 0x00019C6C
		public IList<Texture2D> GetMaps(int idx)
		{
			IList<Texture2D> list = new List<Texture2D>(3);
			int num = this.m_buffers.Length;
			if (idx < -1 || idx >= num)
			{
				return list;
			}
			if (idx == -1)
			{
				for (int i = 0; i < num; i++)
				{
					if (!this.m_buffers[i].disabled)
					{
						list.Add(this.m_buffers[i].map);
					}
				}
			}
			else if (!this.m_buffers[idx].disabled)
			{
				list.Add(this.m_buffers[idx].map);
			}
			return list;
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x0001BB04 File Offset: 0x00019D04
		public override void EnableBuffer(int idx)
		{
			int num = this.m_buffers.Length;
			if (idx < -1 || idx >= num)
			{
				return;
			}
			if (idx == -1)
			{
				for (int i = 0; i < num; i++)
				{
					this.m_buffers[i].disabled = false;
				}
			}
			else
			{
				this.m_buffers[idx].disabled = false;
			}
		}

		// Token: 0x06000471 RID: 1137 RVA: 0x0001BB64 File Offset: 0x00019D64
		public override void DisableBuffer(int idx)
		{
			int num = this.m_buffers.Length;
			if (idx < -1 || idx >= num)
			{
				return;
			}
			if (idx == -1)
			{
				for (int i = 0; i < num; i++)
				{
					this.m_buffers[i].disabled = true;
				}
			}
			else
			{
				this.m_buffers[idx].disabled = true;
			}
		}

		// Token: 0x06000472 RID: 1138 RVA: 0x0001BBC4 File Offset: 0x00019DC4
		public bool IsDone()
		{
			if (this.m_initTask == null)
			{
				return true;
			}
			if (!this.m_initTask.Done)
			{
				return false;
			}
			int count = this.m_fourierTasks.Count;
			for (int i = 0; i < count; i++)
			{
				if (!this.m_fourierTasks[i].Done)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x0001BC28 File Offset: 0x00019E28
		public override int EnabledBuffers()
		{
			int num = 0;
			int num2 = this.m_buffers.Length;
			for (int i = 0; i < num2; i++)
			{
				if (!this.m_buffers[i].disabled)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06000474 RID: 1140 RVA: 0x0001BC6C File Offset: 0x00019E6C
		public override bool IsEnabledBuffer(int idx)
		{
			return idx >= 0 && idx < this.m_buffers.Length && !this.m_buffers[idx].disabled;
		}

		// Token: 0x06000475 RID: 1141
		public abstract void ProcessData(int index, Color[] results, Vector4[] data, int numGrids);

		// Token: 0x06000476 RID: 1142 RVA: 0x0001BC98 File Offset: 0x00019E98
		public virtual void PackData(int index)
		{
			IList<Color[]> results = this.GetResults(index);
			IList<Texture2D> maps = this.GetMaps(index);
			for (int i = 0; i < results.Count; i++)
			{
				if (!(maps[i] == null))
				{
					maps[i].SetPixels(results[i]);
					maps[i].Apply();
				}
			}
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x0001BD04 File Offset: 0x00019F04
		public override void Run(WaveSpectrumCondition condition, float time)
		{
			if (!this.IsDone())
			{
				throw new InvalidOperationException("Can not run when there are tasks that have not finished");
			}
			base.TimeValue = time;
			base.HasRun = true;
			base.BeenSampled = false;
			this.m_fourierTasks.Clear();
			if (this.EnabledBuffers() == 0)
			{
				return;
			}
			this.Initilize(condition, time);
			ThreadedTask initTask = this.m_initTask;
			if (initTask == null)
			{
				throw new InvalidCastException("Init spectrum task is not a threaded task");
			}
			if (Ocean.DISABLE_FOURIER_MULTITHREADING)
			{
				initTask.Start();
				initTask.Run();
				initTask.End();
			}
			int num = this.m_buffers.Length;
			for (int i = 0; i < num; i++)
			{
				if (!this.m_buffers[i].disabled)
				{
					FourierTask fourierTask = new FourierTask(this, this.m_fourier, i, this.m_initTask.NumGrids);
					if (Ocean.DISABLE_FOURIER_MULTITHREADING)
					{
						fourierTask.Start();
						fourierTask.Run();
						fourierTask.End();
					}
					else
					{
						fourierTask.RunOnStopWaiting = true;
						fourierTask.WaitOn(initTask);
						this.m_scheduler.AddWaiting(fourierTask);
					}
					this.m_fourierTasks.Add(fourierTask);
				}
			}
			if (!Ocean.DISABLE_FOURIER_MULTITHREADING)
			{
				initTask.NoFinish = true;
				this.m_scheduler.Run(initTask);
			}
		}

		// Token: 0x04000467 RID: 1127
		public const int READ = 1;

		// Token: 0x04000468 RID: 1128
		public const int WRITE = 0;

		// Token: 0x04000469 RID: 1129
		protected WaveSpectrumBufferCPU.Buffer[] m_buffers;

		// Token: 0x0400046A RID: 1130
		private FourierCPU m_fourier;

		// Token: 0x0400046B RID: 1131
		private Scheduler m_scheduler;

		// Token: 0x0400046C RID: 1132
		private List<ThreadedTask> m_fourierTasks;

		// Token: 0x0400046D RID: 1133
		protected InitSpectrumDisplacementsTask m_initTask;

		// Token: 0x0200009F RID: 159
		public class Buffer
		{
			// Token: 0x0400046F RID: 1135
			public IList<Vector4[]> data;

			// Token: 0x04000470 RID: 1136
			public Color[] results;

			// Token: 0x04000471 RID: 1137
			public Texture2D map;

			// Token: 0x04000472 RID: 1138
			public bool disabled;

			// Token: 0x04000473 RID: 1139
			public bool doublePacked;
		}
	}
}
