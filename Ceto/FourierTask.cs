using System;
using System.Collections;
using System.Collections.Generic;
using Ceto.Common.Threading.Tasks;
using UnityEngine;

namespace Ceto
{
	// Token: 0x020000B5 RID: 181
	public class FourierTask : ThreadedTask
	{
		// Token: 0x06000505 RID: 1285 RVA: 0x0001F6C8 File Offset: 0x0001D8C8
		public FourierTask(WaveSpectrumBufferCPU buffer, FourierCPU fourier, int index, int numGrids) : base(true)
		{
			if (this.m_index == -1)
			{
				throw new InvalidOperationException("Index can be -1. Fourier for multiple buffers is not being used");
			}
			this.m_buffer = buffer;
			this.m_fourier = fourier;
			this.m_index = index;
			this.m_numGrids = numGrids;
			WaveSpectrumBufferCPU.Buffer buffer2 = this.m_buffer.GetBuffer(this.m_index);
			this.m_data = buffer2.data;
			this.m_results = buffer2.results;
			this.m_doublePacked = buffer2.doublePacked;
		}

		// Token: 0x06000506 RID: 1286 RVA: 0x0001F748 File Offset: 0x0001D948
		public override void Start()
		{
			base.Start();
		}

		// Token: 0x06000507 RID: 1287 RVA: 0x0001F750 File Offset: 0x0001D950
		public override IEnumerator Run()
		{
			this.PerformSingleFourier();
			this.FinishedRunning();
			return null;
		}

		// Token: 0x06000508 RID: 1288 RVA: 0x0001F760 File Offset: 0x0001D960
		public override void End()
		{
			base.End();
			this.m_buffer.PackData(this.m_index);
		}

		// Token: 0x06000509 RID: 1289 RVA: 0x0001F77C File Offset: 0x0001D97C
		private void PerformSingleFourier()
		{
			int num;
			if (this.m_doublePacked)
			{
				num = this.m_fourier.PeformFFT_DoublePacked(0, this.m_data, this);
			}
			else
			{
				num = this.m_fourier.PeformFFT_SinglePacked(0, this.m_data, this);
			}
			if (this.Cancelled)
			{
				return;
			}
			if (num != 1)
			{
				throw new InvalidOperationException("Fourier transform did not result in the read buffer at index " + 1);
			}
			if (!Ocean.DISABLE_PROCESS_DATA_MULTITHREADING)
			{
				this.m_buffer.ProcessData(this.m_index, this.m_results, this.m_data[num], this.m_numGrids);
			}
		}

		// Token: 0x040004DA RID: 1242
		private FourierCPU m_fourier;

		// Token: 0x040004DB RID: 1243
		private WaveSpectrumBufferCPU m_buffer;

		// Token: 0x040004DC RID: 1244
		private int m_numGrids;

		// Token: 0x040004DD RID: 1245
		private int m_index;

		// Token: 0x040004DE RID: 1246
		private IList<Vector4[]> m_data;

		// Token: 0x040004DF RID: 1247
		private Color[] m_results;

		// Token: 0x040004E0 RID: 1248
		private bool m_doublePacked;
	}
}
