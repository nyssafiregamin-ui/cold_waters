using System;
using System.Collections.Generic;
using Ceto.Common.Containers.Interpolation;
using Ceto.Common.Threading.Scheduling;
using UnityEngine;

namespace Ceto
{
	// Token: 0x02000099 RID: 153
	public class DisplacementBufferCPU : WaveSpectrumBufferCPU, IDisplacementBuffer
	{
		// Token: 0x06000428 RID: 1064 RVA: 0x00019EAC File Offset: 0x000180AC
		public DisplacementBufferCPU(int size, Scheduler scheduler) : base(size, 3, scheduler)
		{
			int grids = QueryDisplacements.GRIDS;
			int channels = QueryDisplacements.CHANNELS;
			this.m_displacements = new List<InterpolatedArray2f[]>(2);
			this.m_displacements.Add(new InterpolatedArray2f[grids]);
			this.m_displacements.Add(new InterpolatedArray2f[grids]);
			for (int i = 0; i < grids; i++)
			{
				this.m_displacements[0][i] = new InterpolatedArray2f(size, size, channels, true);
				this.m_displacements[1][i] = new InterpolatedArray2f(size, size, channels, true);
			}
		}

		// Token: 0x06000429 RID: 1065 RVA: 0x00019F3C File Offset: 0x0001813C
		protected override void Initilize(WaveSpectrumCondition condition, float time)
		{
			InterpolatedArray2f[] writeDisplacements = this.GetWriteDisplacements();
			writeDisplacements[0].Clear();
			writeDisplacements[1].Clear();
			writeDisplacements[2].Clear();
			writeDisplacements[3].Clear();
			if (this.m_initTask == null)
			{
				this.m_initTask = condition.GetInitSpectrumDisplacementsTask(this, time);
			}
			else if (this.m_initTask.SpectrumType != condition.Key.SpectrumType || this.m_initTask.NumGrids != condition.Key.NumGrids)
			{
				this.m_initTask = condition.GetInitSpectrumDisplacementsTask(this, time);
			}
			else
			{
				this.m_initTask.Reset(condition, time);
			}
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x00019FE4 File Offset: 0x000181E4
		public InterpolatedArray2f[] GetWriteDisplacements()
		{
			return this.m_displacements[0];
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x00019FF4 File Offset: 0x000181F4
		public InterpolatedArray2f[] GetReadDisplacements()
		{
			return this.m_displacements[1];
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x0001A004 File Offset: 0x00018204
		public override void Run(WaveSpectrumCondition condition, float time)
		{
			this.SwapDisplacements();
			base.Run(condition, time);
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x0001A014 File Offset: 0x00018214
		public void CopyAndCreateDisplacements(out IList<InterpolatedArray2f> displacements)
		{
			InterpolatedArray2f[] readDisplacements = this.GetReadDisplacements();
			QueryDisplacements.CopyAndCreateDisplacements(readDisplacements, out displacements);
		}

		// Token: 0x0600042E RID: 1070 RVA: 0x0001A030 File Offset: 0x00018230
		public void CopyDisplacements(IList<InterpolatedArray2f> displacements)
		{
			InterpolatedArray2f[] readDisplacements = this.GetReadDisplacements();
			QueryDisplacements.CopyDisplacements(readDisplacements, displacements);
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x0001A04C File Offset: 0x0001824C
		private void SwapDisplacements()
		{
			InterpolatedArray2f[] value = this.m_displacements[0];
			this.m_displacements[0] = this.m_displacements[1];
			this.m_displacements[1] = value;
		}

		// Token: 0x06000430 RID: 1072 RVA: 0x0001A08C File Offset: 0x0001828C
		public override void PackData(int index)
		{
			if (Ocean.DISABLE_PROCESS_DATA_MULTITHREADING)
			{
				IList<IList<Vector4[]>> data = base.GetData(index);
				IList<Color[]> results = base.GetResults(index);
				for (int i = 0; i < results.Count; i++)
				{
					Color[] result = results[i];
					Vector4[] data2 = data[i][1];
					int index2 = (index != -1) ? index : i;
					this.ProcessData(index2, result, data2, this.m_initTask.NumGrids);
				}
			}
			base.PackData(index);
		}

		// Token: 0x06000431 RID: 1073 RVA: 0x0001A110 File Offset: 0x00018310
		public override void ProcessData(int index, Color[] result, Vector4[] data, int numGrids)
		{
			int channels = QueryDisplacements.CHANNELS;
			int size = this.Size;
			InterpolatedArray2f[] writeDisplacements = this.GetWriteDisplacements();
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					int num = j + i * size;
					int num2 = num * channels;
					if (numGrids == 1)
					{
						result[num].r = data[num].x;
						result[num].g = data[num].y;
						result[num].b = 0f;
						result[num].a = 0f;
						if (index == 0)
						{
							writeDisplacements[0].Data[num2 + 1] = result[num].r;
						}
						else if (index == 1)
						{
							writeDisplacements[0].Data[num2] += result[num].r;
							writeDisplacements[0].Data[num2 + 2] += result[num].g;
						}
					}
					else if (numGrids == 2)
					{
						result[num].r = data[num].x;
						result[num].g = data[num].y;
						result[num].b = data[num].z;
						result[num].a = data[num].w;
						if (index == 0)
						{
							writeDisplacements[0].Data[num2 + 1] = result[num].r;
							writeDisplacements[1].Data[num2 + 1] = result[num].g;
						}
						else if (index == 1)
						{
							writeDisplacements[0].Data[num2] += result[num].r;
							writeDisplacements[0].Data[num2 + 2] += result[num].g;
							writeDisplacements[1].Data[num2] += result[num].b;
							writeDisplacements[1].Data[num2 + 2] += result[num].a;
						}
					}
					else if (numGrids == 3)
					{
						result[num].r = data[num].x;
						result[num].g = data[num].y;
						result[num].b = data[num].z;
						result[num].a = data[num].w;
						if (index == 0)
						{
							writeDisplacements[0].Data[num2 + 1] = result[num].r;
							writeDisplacements[1].Data[num2 + 1] = result[num].g;
							writeDisplacements[2].Data[num2 + 1] = result[num].b;
							writeDisplacements[3].Data[num2 + 1] = result[num].a;
						}
						else if (index == 1)
						{
							writeDisplacements[0].Data[num2] += result[num].r;
							writeDisplacements[0].Data[num2 + 2] += result[num].g;
							writeDisplacements[1].Data[num2] += result[num].b;
							writeDisplacements[1].Data[num2 + 2] += result[num].a;
						}
						else if (index == 2)
						{
							writeDisplacements[2].Data[num2] += result[num].r;
							writeDisplacements[2].Data[num2 + 2] += result[num].g;
						}
					}
					else if (numGrids == 4)
					{
						result[num].r = data[num].x;
						result[num].g = data[num].y;
						result[num].b = data[num].z;
						result[num].a = data[num].w;
						if (index == 0)
						{
							writeDisplacements[0].Data[num2 + 1] = result[num].r;
							writeDisplacements[1].Data[num2 + 1] = result[num].g;
							writeDisplacements[2].Data[num2 + 1] = result[num].b;
							writeDisplacements[3].Data[num2 + 1] = result[num].a;
						}
						else if (index == 1)
						{
							writeDisplacements[0].Data[num2] += result[num].r;
							writeDisplacements[0].Data[num2 + 2] += result[num].g;
							writeDisplacements[1].Data[num2] += result[num].b;
							writeDisplacements[1].Data[num2 + 2] += result[num].a;
						}
						else if (index == 2)
						{
							writeDisplacements[2].Data[num2] += result[num].r;
							writeDisplacements[2].Data[num2 + 2] += result[num].g;
							writeDisplacements[3].Data[num2] += result[num].b;
							writeDisplacements[3].Data[num2 + 2] += result[num].a;
						}
					}
					else
					{
						result[num].r = 0f;
						result[num].g = 0f;
						result[num].b = 0f;
						result[num].a = 0f;
					}
				}
			}
		}

		// Token: 0x06000432 RID: 1074 RVA: 0x0001A788 File Offset: 0x00018988
		public Vector4 MaxRange(Vector4 choppyness, Vector2 gridScale)
		{
			InterpolatedArray2f[] readDisplacements = this.GetReadDisplacements();
			return QueryDisplacements.MaxRange(readDisplacements, choppyness, gridScale, null);
		}

		// Token: 0x06000433 RID: 1075 RVA: 0x0001A7A8 File Offset: 0x000189A8
		public void QueryWaves(WaveQuery query, QueryGridScaling scaling)
		{
			int num = this.EnabledBuffers();
			if (num == 0)
			{
				return;
			}
			InterpolatedArray2f[] readDisplacements = this.GetReadDisplacements();
			QueryDisplacements.QueryWaves(query, num, readDisplacements, scaling);
		}

		// Token: 0x0400045C RID: 1116
		private const int NUM_BUFFERS = 3;

		// Token: 0x0400045D RID: 1117
		private IList<InterpolatedArray2f[]> m_displacements;
	}
}
