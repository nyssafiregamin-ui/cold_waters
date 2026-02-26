using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ceto
{
	// Token: 0x020000BC RID: 188
	public class RefractionCommand : RefractionCommandBase
	{
		// Token: 0x06000570 RID: 1392 RVA: 0x00023A8C File Offset: 0x00021C8C
		public RefractionCommand(Shader copyDepth)
		{
			this.GrabName = Ocean.REFRACTION_GRAB_TEXTURE_NAME;
			this.DepthName = Ocean.DEPTH_GRAB_TEXTURE_NAME;
			this.m_copyDepthMat = new Material(copyDepth);
			this.m_data = new Dictionary<Camera, RefractionCommandBase.CommandData>();
		}

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x06000571 RID: 1393 RVA: 0x00023ACC File Offset: 0x00021CCC
		// (set) Token: 0x06000572 RID: 1394 RVA: 0x00023AD4 File Offset: 0x00021CD4
		public string GrabName { get; private set; }

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x06000573 RID: 1395 RVA: 0x00023AE0 File Offset: 0x00021CE0
		// (set) Token: 0x06000574 RID: 1396 RVA: 0x00023AE8 File Offset: 0x00021CE8
		public string DepthName { get; private set; }

		// Token: 0x06000575 RID: 1397 RVA: 0x00023AF4 File Offset: 0x00021CF4
		public override CommandBuffer Create(Camera cam)
		{
			CommandBuffer commandBuffer = new CommandBuffer();
			commandBuffer.name = "Ceto DepthGrab Cmd: " + cam.name;
			RenderTextureFormat format;
			if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RFloat))
			{
				format = RenderTextureFormat.RFloat;
			}
			else
			{
				format = RenderTextureFormat.RHalf;
			}
			int nameID = Shader.PropertyToID("Ceto_DepthCopyTexture");
			commandBuffer.GetTemporaryRT(nameID, cam.pixelWidth, cam.pixelHeight, 0, FilterMode.Point, format, RenderTextureReadWrite.Linear);
			commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, nameID, this.m_copyDepthMat, 0);
			commandBuffer.SetGlobalTexture(this.DepthName, nameID);
			cam.AddCommandBuffer(this.Event, commandBuffer);
			RefractionCommandBase.CommandData commandData = new RefractionCommandBase.CommandData();
			commandData.command = commandBuffer;
			commandData.width = cam.pixelWidth;
			commandData.height = cam.pixelHeight;
			if (this.m_data.ContainsKey(cam))
			{
				this.m_data.Remove(cam);
			}
			this.m_data.Add(cam, commandData);
			return commandBuffer;
		}

		// Token: 0x04000544 RID: 1348
		public Material m_copyDepthMat;
	}
}
