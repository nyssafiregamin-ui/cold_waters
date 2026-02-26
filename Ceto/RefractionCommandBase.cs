using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ceto
{
	// Token: 0x020000BD RID: 189
	public abstract class RefractionCommandBase : IRefractionCommand
	{
		// Token: 0x06000576 RID: 1398 RVA: 0x00023BE0 File Offset: 0x00021DE0
		public RefractionCommandBase()
		{
			this.m_data = new Dictionary<Camera, RefractionCommandBase.CommandData>();
		}

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06000577 RID: 1399 RVA: 0x00023BF4 File Offset: 0x00021DF4
		// (set) Token: 0x06000578 RID: 1400 RVA: 0x00023BFC File Offset: 0x00021DFC
		public CameraEvent Event { get; set; }

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06000579 RID: 1401 RVA: 0x00023C08 File Offset: 0x00021E08
		// (set) Token: 0x0600057A RID: 1402 RVA: 0x00023C10 File Offset: 0x00021E10
		public REFRACTION_RESOLUTION Resolution { get; set; }

		// Token: 0x0600057B RID: 1403
		public abstract CommandBuffer Create(Camera cam);

		// Token: 0x0600057C RID: 1404 RVA: 0x00023C1C File Offset: 0x00021E1C
		public virtual void Remove(Camera cam)
		{
			if (this.m_data.ContainsKey(cam))
			{
				RefractionCommandBase.CommandData commandData = this.m_data[cam];
				cam.RemoveCommandBuffer(this.Event, commandData.command);
				this.m_data.Remove(cam);
			}
		}

		// Token: 0x0600057D RID: 1405 RVA: 0x00023C68 File Offset: 0x00021E68
		public virtual void RemoveAll()
		{
			if (this.m_data.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<Camera, RefractionCommandBase.CommandData> keyValuePair in this.m_data)
			{
				Camera key = keyValuePair.Key;
				Dictionary<Camera, RefractionCommandBase.CommandData>.Enumerator enumerator;
				KeyValuePair<Camera, RefractionCommandBase.CommandData> keyValuePair2 = enumerator.Current;
				CommandBuffer command = keyValuePair2.Value.command;
				key.RemoveCommandBuffer(this.Event, command);
			}
			this.m_data.Clear();
		}

		// Token: 0x0600057E RID: 1406 RVA: 0x00023CE4 File Offset: 0x00021EE4
		public virtual bool Matches(Camera cam)
		{
			if (!this.m_data.ContainsKey(cam))
			{
				return false;
			}
			RefractionCommandBase.CommandData commandData = this.m_data[cam];
			return commandData.width == cam.pixelWidth && commandData.height == cam.pixelHeight;
		}

		// Token: 0x0600057F RID: 1407 RVA: 0x00023D38 File Offset: 0x00021F38
		protected virtual int ResolutionToNumber(REFRACTION_RESOLUTION resolution)
		{
			switch (resolution)
			{
			case REFRACTION_RESOLUTION.FULL:
				return 1;
			case REFRACTION_RESOLUTION.HALF:
				return 2;
			case REFRACTION_RESOLUTION.QUARTER:
				return 4;
			default:
				return 2;
			}
		}

		// Token: 0x04000547 RID: 1351
		protected Dictionary<Camera, RefractionCommandBase.CommandData> m_data;

		// Token: 0x020000BE RID: 190
		public class CommandData
		{
			// Token: 0x0400054A RID: 1354
			public CommandBuffer command;

			// Token: 0x0400054B RID: 1355
			public int width;

			// Token: 0x0400054C RID: 1356
			public int height;
		}
	}
}
