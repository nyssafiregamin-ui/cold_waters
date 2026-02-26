using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto.Common.Unity.Utility
{
	// Token: 0x02000058 RID: 88
	public static class RTUtility
	{
		// Token: 0x060002AE RID: 686 RVA: 0x0000F1F0 File Offset: 0x0000D3F0
		public static void Blit(RenderTexture des, Material mat, int pass = 0)
		{
			Graphics.SetRenderTarget(des);
			GL.PushMatrix();
			GL.LoadOrtho();
			mat.SetPass(pass);
			GL.Begin(7);
			GL.TexCoord2(0f, 0f);
			GL.Vertex3(0f, 0f, 0.1f);
			GL.TexCoord2(1f, 0f);
			GL.Vertex3(1f, 0f, 0.1f);
			GL.TexCoord2(1f, 1f);
			GL.Vertex3(1f, 1f, 0.1f);
			GL.TexCoord2(0f, 1f);
			GL.Vertex3(0f, 1f, 0.1f);
			GL.End();
			GL.PopMatrix();
		}

		// Token: 0x060002AF RID: 687 RVA: 0x0000F2B4 File Offset: 0x0000D4B4
		public static void Blit(RenderTexture des, Material mat, Vector3[] verts, int pass = 0)
		{
			Graphics.SetRenderTarget(des);
			GL.PushMatrix();
			GL.LoadOrtho();
			mat.SetPass(pass);
			GL.Begin(7);
			GL.TexCoord2(0f, 0f);
			GL.Vertex(verts[0]);
			GL.TexCoord2(1f, 0f);
			GL.Vertex(verts[1]);
			GL.TexCoord2(1f, 1f);
			GL.Vertex(verts[2]);
			GL.TexCoord2(0f, 1f);
			GL.Vertex(verts[3]);
			GL.End();
			GL.PopMatrix();
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x0000F36C File Offset: 0x0000D56C
		public static void Blit(RenderTexture des, Material mat, Vector3[] verts, Vector2[] uvs, int pass = 0)
		{
			Graphics.SetRenderTarget(des);
			GL.PushMatrix();
			GL.LoadOrtho();
			mat.SetPass(pass);
			GL.Begin(7);
			GL.TexCoord(uvs[0]);
			GL.Vertex(verts[0]);
			GL.TexCoord(uvs[1]);
			GL.Vertex(verts[1]);
			GL.TexCoord(uvs[2]);
			GL.Vertex(verts[2]);
			GL.TexCoord(uvs[3]);
			GL.Vertex(verts[3]);
			GL.End();
			GL.PopMatrix();
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x0000F440 File Offset: 0x0000D640
		public static void MultiTargetBlit(IList<RenderTexture> des, Material mat, int pass = 0)
		{
			RenderBuffer[] array = new RenderBuffer[des.Count];
			for (int i = 0; i < des.Count; i++)
			{
				array[i] = des[i].colorBuffer;
			}
			Graphics.SetRenderTarget(array, des[0].depthBuffer);
			GL.PushMatrix();
			GL.LoadOrtho();
			mat.SetPass(pass);
			GL.Begin(7);
			GL.TexCoord2(0f, 0f);
			GL.Vertex3(0f, 0f, 0.1f);
			GL.TexCoord2(1f, 0f);
			GL.Vertex3(1f, 0f, 0.1f);
			GL.TexCoord2(1f, 1f);
			GL.Vertex3(1f, 1f, 0.1f);
			GL.TexCoord2(0f, 1f);
			GL.Vertex3(0f, 1f, 0.1f);
			GL.End();
			GL.PopMatrix();
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x0000F548 File Offset: 0x0000D748
		public static void MultiTargetBlit(RenderBuffer[] des_rb, RenderBuffer des_db, Material mat, int pass = 0)
		{
			Graphics.SetRenderTarget(des_rb, des_db);
			GL.PushMatrix();
			GL.LoadOrtho();
			mat.SetPass(pass);
			GL.Begin(7);
			GL.TexCoord2(0f, 0f);
			GL.Vertex3(0f, 0f, 0.1f);
			GL.TexCoord2(1f, 0f);
			GL.Vertex3(1f, 0f, 0.1f);
			GL.TexCoord2(1f, 1f);
			GL.Vertex3(1f, 1f, 0.1f);
			GL.TexCoord2(0f, 1f);
			GL.Vertex3(0f, 1f, 0.1f);
			GL.End();
			GL.PopMatrix();
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x0000F60C File Offset: 0x0000D80C
		public static void ClearColor(RenderTexture tex, Color col)
		{
			if (tex == null)
			{
				return;
			}
			if (!SystemInfo.SupportsRenderTextureFormat(tex.format))
			{
				return;
			}
			Graphics.SetRenderTarget(tex);
			GL.Clear(false, true, col);
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x0000F648 File Offset: 0x0000D848
		public static void Release(RenderTexture tex)
		{
			if (tex == null)
			{
				return;
			}
			tex.Release();
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x0000F660 File Offset: 0x0000D860
		public static void Release(IList<RenderTexture> texList)
		{
			if (texList == null)
			{
				return;
			}
			int count = texList.Count;
			for (int i = 0; i < count; i++)
			{
				if (!(texList[i] == null))
				{
					texList[i].Release();
				}
			}
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x0000F6B0 File Offset: 0x0000D8B0
		public static void ReleaseAndDestroy(RenderTexture tex)
		{
			if (tex == null)
			{
				return;
			}
			tex.Release();
			UnityEngine.Object.Destroy(tex);
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x0000F6CC File Offset: 0x0000D8CC
		public static void ReleaseAndDestroy(IList<RenderTexture> texList)
		{
			if (texList == null)
			{
				return;
			}
			int count = texList.Count;
			for (int i = 0; i < count; i++)
			{
				if (!(texList[i] == null))
				{
					texList[i].Release();
					UnityEngine.Object.Destroy(texList[i]);
				}
			}
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x0000F728 File Offset: 0x0000D928
		public static void ReleaseTemporary(RenderTexture tex)
		{
			if (tex == null)
			{
				return;
			}
			RenderTexture.ReleaseTemporary(tex);
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x0000F740 File Offset: 0x0000D940
		public static void ReleaseTemporary(IList<RenderTexture> texList)
		{
			if (texList == null)
			{
				return;
			}
			int count = texList.Count;
			for (int i = 0; i < count; i++)
			{
				if (!(texList[i] == null))
				{
					RenderTexture.ReleaseTemporary(texList[i]);
				}
			}
		}

		// Token: 0x060002BA RID: 698 RVA: 0x0000F790 File Offset: 0x0000D990
		private static RenderTextureFormat CheckFormat(RTSettings setting)
		{
			RenderTextureFormat renderTextureFormat = setting.format;
			if (!SystemInfo.SupportsRenderTextureFormat(renderTextureFormat))
			{
				Debug.Log("System does not support " + renderTextureFormat + " render texture format.");
				bool flag = false;
				int count = setting.fallbackFormats.Count;
				for (int i = 0; i < count; i++)
				{
					if (SystemInfo.SupportsRenderTextureFormat(setting.fallbackFormats[i]))
					{
						renderTextureFormat = setting.fallbackFormats[i];
						Debug.Log("Found fallback format: " + renderTextureFormat);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					throw new InvalidOperationException("Could not find fallback render texture format");
				}
			}
			return renderTextureFormat;
		}

		// Token: 0x060002BB RID: 699 RVA: 0x0000F83C File Offset: 0x0000DA3C
		public static RenderTexture CreateRenderTexture(RTSettings setting)
		{
			if (setting == null)
			{
				throw new NullReferenceException("RTSettings is null");
			}
			if (!SystemInfo.supportsRenderTextures)
			{
				throw new InvalidOperationException("This system does not support render textures");
			}
			RenderTextureFormat format = RTUtility.CheckFormat(setting);
			return new RenderTexture(setting.width, setting.height, setting.depth, format, setting.readWrite)
			{
				name = setting.name,
				wrapMode = setting.wrap,
				filterMode = setting.filer,
				useMipMap = setting.mipmaps,
				anisoLevel = setting.ansioLevel,
				enableRandomWrite = setting.randomWrite
			};
		}

		// Token: 0x060002BC RID: 700 RVA: 0x0000F8E0 File Offset: 0x0000DAE0
		public static RenderTexture CreateTemporyRenderTexture(RTSettings setting)
		{
			if (setting == null)
			{
				throw new NullReferenceException("RTSettings is null");
			}
			if (!SystemInfo.supportsRenderTextures)
			{
				throw new InvalidOperationException("This system does not support render textures");
			}
			RenderTextureFormat format = RTUtility.CheckFormat(setting);
			RenderTexture temporary = RenderTexture.GetTemporary(setting.width, setting.height, setting.depth, format, setting.readWrite);
			temporary.name = setting.name;
			temporary.wrapMode = setting.wrap;
			temporary.filterMode = setting.filer;
			temporary.anisoLevel = setting.ansioLevel;
			return temporary;
		}
	}
}
