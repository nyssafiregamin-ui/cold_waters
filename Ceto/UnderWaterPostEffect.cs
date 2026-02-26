using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x020000C4 RID: 196
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Ceto/Camera/UnderWaterPostEffect")]
	public class UnderWaterPostEffect : MonoBehaviour
	{
		// Token: 0x060005A2 RID: 1442 RVA: 0x00025428 File Offset: 0x00023628
		private void Start()
		{
			this.m_material = new Material(this.underWaterPostEffectSdr);
			this.m_imageBlur = new ImageBlur(this.blurShader);
		}

		// Token: 0x060005A3 RID: 1443 RVA: 0x00025458 File Offset: 0x00023658
		private void LateUpdate()
		{
			Camera component = base.GetComponent<Camera>();
			this.m_underWaterIsVisible = this.UnderWaterIsVisible(component);
			if (this.controlUnderwaterMode && Ocean.Instance != null && Ocean.Instance.UnderWater is UnderWater)
			{
				UnderWater underWater = Ocean.Instance.UnderWater as UnderWater;
				if (!this.m_underWaterIsVisible)
				{
					underWater.underwaterMode = UNDERWATER_MODE.ABOVE_ONLY;
				}
				else
				{
					underWater.underwaterMode = UNDERWATER_MODE.ABOVE_AND_BELOW;
				}
			}
		}

		// Token: 0x060005A4 RID: 1444 RVA: 0x000254D8 File Offset: 0x000236D8
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (this.underWaterPostEffectSdr == null || this.m_material == null || SystemInfo.graphicsShaderLevel < 30)
			{
				Graphics.Blit(source, destination);
				return;
			}
			if (Ocean.Instance == null || Ocean.Instance.UnderWater == null)
			{
				Graphics.Blit(source, destination);
				return;
			}
			if (!Ocean.Instance.gameObject.activeInHierarchy)
			{
				Graphics.Blit(source, destination);
				return;
			}
			if (Ocean.Instance.UnderWater.Mode != UNDERWATER_MODE.ABOVE_AND_BELOW)
			{
				Graphics.Blit(source, destination);
				return;
			}
			if (!this.m_underWaterIsVisible)
			{
				Graphics.Blit(source, destination);
				return;
			}
			Camera component = base.GetComponent<Camera>();
			float nearClipPlane = component.nearClipPlane;
			float farClipPlane = component.farClipPlane;
			float fieldOfView = component.fieldOfView;
			float aspect = component.aspect;
			Matrix4x4 identity = Matrix4x4.identity;
			float num = fieldOfView * 0.5f;
			Vector3 b = component.transform.right * nearClipPlane * Mathf.Tan(num * 0.017453292f) * aspect;
			Vector3 b2 = component.transform.up * nearClipPlane * Mathf.Tan(num * 0.017453292f);
			Vector3 vector = component.transform.forward * nearClipPlane - b + b2;
			float d = vector.magnitude * farClipPlane / nearClipPlane;
			vector.Normalize();
			vector *= d;
			Vector3 vector2 = component.transform.forward * nearClipPlane + b + b2;
			vector2.Normalize();
			vector2 *= d;
			Vector3 vector3 = component.transform.forward * nearClipPlane + b - b2;
			vector3.Normalize();
			vector3 *= d;
			Vector3 vector4 = component.transform.forward * nearClipPlane - b - b2;
			vector4.Normalize();
			vector4 *= d;
			identity.SetRow(0, vector);
			identity.SetRow(1, vector2);
			identity.SetRow(2, vector3);
			identity.SetRow(3, vector4);
			this.m_material.SetMatrix("_FrustumCorners", identity);
			Color color = Color.white;
			if (this.attenuateBySun)
			{
				color = Ocean.Instance.SunColor() * Mathf.Max(0f, Vector3.Dot(Vector3.up, Ocean.Instance.SunDir()));
			}
			this.m_material.SetColor("_MultiplyCol", color);
			RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0);
			this.CustomGraphicsBlit(source, temporary, this.m_material, 0);
			this.m_imageBlur.BlurIterations = this.blurIterations;
			this.m_imageBlur.BlurMode = this.blurMode;
			this.m_imageBlur.BlurSpread = this.blurSpread;
			this.m_imageBlur.Blur(temporary);
			this.m_material.SetTexture("_BelowTex", temporary);
			Graphics.Blit(source, destination, this.m_material, 1);
			RenderTexture.ReleaseTemporary(temporary);
		}

		// Token: 0x060005A5 RID: 1445 RVA: 0x00025820 File Offset: 0x00023A20
		private void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material mat, int pass)
		{
			RenderTexture.active = dest;
			mat.SetTexture("_MainTex", source);
			GL.PushMatrix();
			GL.LoadOrtho();
			mat.SetPass(pass);
			GL.Begin(7);
			GL.MultiTexCoord2(0, 0f, 0f);
			GL.Vertex3(0f, 0f, 3f);
			GL.MultiTexCoord2(0, 1f, 0f);
			GL.Vertex3(1f, 0f, 2f);
			GL.MultiTexCoord2(0, 1f, 1f);
			GL.Vertex3(1f, 1f, 1f);
			GL.MultiTexCoord2(0, 0f, 1f);
			GL.Vertex3(0f, 1f, 0f);
			GL.End();
			GL.PopMatrix();
		}

		// Token: 0x060005A6 RID: 1446 RVA: 0x000258F4 File Offset: 0x00023AF4
		private bool UnderWaterIsVisible(Camera cam)
		{
			if (Ocean.Instance == null)
			{
				return false;
			}
			Vector3 position = cam.transform.position;
			if (this.disableOnClip)
			{
				WaveQuery waveQuery = new WaveQuery(position);
				waveQuery.mode = QUERY_MODE.CLIP_TEST;
				Ocean.Instance.QueryWaves(waveQuery);
				if (waveQuery.result.isClipped)
				{
					return false;
				}
			}
			float num = Ocean.Instance.FindMaxDisplacement(true) + Ocean.Instance.level;
			if (position.y < num)
			{
				return true;
			}
			Matrix4x4 inverse = (cam.projectionMatrix * cam.worldToCameraMatrix).inverse;
			for (int i = 0; i < 4; i++)
			{
				Vector4 vector = inverse * UnderWaterPostEffect.m_corners[i];
				vector.y /= vector.w;
				if (vector.y < num)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04000578 RID: 1400
		public bool disableOnClip = true;

		// Token: 0x04000579 RID: 1401
		public bool controlUnderwaterMode;

		// Token: 0x0400057A RID: 1402
		public bool attenuateBySun;

		// Token: 0x0400057B RID: 1403
		public ImageBlur.BLUR_MODE blurMode;

		// Token: 0x0400057C RID: 1404
		[Range(0f, 4f)]
		public int blurIterations = 3;

		// Token: 0x0400057D RID: 1405
		[Range(0.5f, 1f)]
		private float blurSpread = 0.6f;

		// Token: 0x0400057E RID: 1406
		public Shader underWaterPostEffectSdr;

		// Token: 0x0400057F RID: 1407
		[HideInInspector]
		public Shader blurShader;

		// Token: 0x04000580 RID: 1408
		private Material m_material;

		// Token: 0x04000581 RID: 1409
		private ImageBlur m_imageBlur;

		// Token: 0x04000582 RID: 1410
		private bool m_underWaterIsVisible;

		// Token: 0x04000583 RID: 1411
		private static readonly Vector4[] m_corners = new Vector4[]
		{
			new Vector4(-1f, -1f, -1f, 1f),
			new Vector4(1f, -1f, -1f, 1f),
			new Vector4(1f, 1f, -1f, 1f),
			new Vector4(-1f, 1f, -1f, 1f)
		};
	}
}
