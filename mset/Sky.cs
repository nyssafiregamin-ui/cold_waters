using System;
using UnityEngine;

namespace mset
{
	// Token: 0x020000E7 RID: 231
	[Serializable]
	public class Sky : MonoBehaviour
	{
		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06000613 RID: 1555 RVA: 0x0002A2D4 File Offset: 0x000284D4
		private Material skyboxMaterial
		{
			get
			{
				if (this._skyboxMaterial == null)
				{
					Shader shader = Shader.Find("Hidden/Marmoset/Skybox IBL");
					if (shader)
					{
						this._skyboxMaterial = new Material(shader);
						this._skyboxMaterial.name = "Internal IBL Skybox";
					}
					else
					{
						Debug.LogError("Failed to create IBL Skybox material. Missing shader?");
					}
				}
				return this._skyboxMaterial;
			}
		}

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06000614 RID: 1556 RVA: 0x0002A33C File Offset: 0x0002853C
		private Cubemap blackCube
		{
			get
			{
				if (this._blackCube == null)
				{
					this._blackCube = new Cubemap(16, TextureFormat.ARGB32, true);
					for (int i = 0; i < 6; i++)
					{
						for (int j = 0; j < 16; j++)
						{
							for (int k = 0; k < 16; k++)
							{
								this._blackCube.SetPixel((CubemapFace)i, j, k, Color.black);
							}
						}
					}
					this._blackCube.Apply(true);
				}
				return this._blackCube;
			}
		}

		// Token: 0x06000615 RID: 1557 RVA: 0x0002A3C8 File Offset: 0x000285C8
		public void Apply()
		{
			this.Apply(null);
		}

		// Token: 0x06000616 RID: 1558 RVA: 0x0002A3D4 File Offset: 0x000285D4
		public void Apply(Renderer target)
		{
			if (base.enabled && base.gameObject.activeInHierarchy)
			{
				if (target == null)
				{
					if (Sky.activeSky != null)
					{
						Sky.activeSky.UnApply();
					}
					Sky.activeSky = this;
					this.ToggleChildLights(true);
					this.UpdateExposures();
					this.ApplySkybox();
					Shader.DisableKeyword("MARMO_GAMMA");
					Shader.DisableKeyword("MARMO_LINEAR");
					if (this.linearSpace)
					{
						Shader.EnableKeyword("MARMO_LINEAR");
					}
					else
					{
						Shader.EnableKeyword("MARMO_GAMMA");
					}
					Shader.DisableKeyword("MARMO_BOX_PROJECTION_OFF");
					Shader.DisableKeyword("MARMO_BOX_PROJECTION");
					if (this.hasDimensions)
					{
						Shader.EnableKeyword("MARMO_BOX_PROJECTION");
					}
					else
					{
						Shader.EnableKeyword("MARMO_BOX_PROJECTION_OFF");
					}
				}
				else
				{
					Material material = target.material;
					material.DisableKeyword("MARMO_BOX_PROJECTION_OFF");
					material.DisableKeyword("MARMO_BOX_PROJECTION");
					if (this.hasDimensions)
					{
						material.EnableKeyword("MARMO_BOX_PROJECTION");
					}
					else
					{
						material.EnableKeyword("MARMO_BOX_PROJECTION_OFF");
					}
				}
				this.ApplyExposures(target);
				this.ApplyIBL(target);
				this.ApplySkyTransform(target);
			}
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x0002A508 File Offset: 0x00028708
		public void ApplySkyTransform()
		{
			this.ApplySkyTransform(null);
		}

		// Token: 0x06000618 RID: 1560 RVA: 0x0002A514 File Offset: 0x00028714
		public void ApplySkyTransform(Renderer target)
		{
			if (base.enabled && base.gameObject.activeInHierarchy)
			{
				if (target == null)
				{
					this.UpdateSkyTransform();
					Shader.SetGlobalMatrix("SkyMatrix", this.skyMatrix);
					Shader.SetGlobalMatrix("InvSkyMatrix", this.skyMatrix.inverse);
					Shader.SetGlobalVector("SkyPosition", base.transform.position);
					Shader.SetGlobalVector("_SkySize", 0.5f * base.transform.localScale);
					Shader.SetGlobalFloat("_UseBoxProjection", (!this.hasDimensions) ? 1f : 1f);
				}
				else
				{
					target.material.SetMatrix("SkyMatrix", this.skyMatrix);
					target.material.SetMatrix("InvSkyMatrix", this.skyMatrix.inverse);
					target.material.SetVector("SkyPosition", base.transform.position);
					target.material.SetVector("_SkySize", 0.5f * base.transform.localScale);
					target.material.SetFloat("_UseBoxProjection", (!this.hasDimensions) ? 1f : 1f);
				}
			}
		}

		// Token: 0x06000619 RID: 1561 RVA: 0x0002A680 File Offset: 0x00028880
		public static void SetUniformOcclusion(Renderer target, float diffuse, float specular)
		{
			Vector4 one = Vector4.one;
			one.x = diffuse;
			one.y = specular;
			target.material.SetVector("UniformOcclusion", one);
		}

		// Token: 0x0600061A RID: 1562 RVA: 0x0002A6B4 File Offset: 0x000288B4
		public void ToggleChildLights(bool enable)
		{
			Light[] componentsInChildren = base.GetComponentsInChildren<Light>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = enable;
			}
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x0002A6E8 File Offset: 0x000288E8
		private void UnApply()
		{
			this.ToggleChildLights(false);
		}

		// Token: 0x0600061C RID: 1564 RVA: 0x0002A6F4 File Offset: 0x000288F4
		private void UpdateSkyTransform()
		{
			this.skyMatrix.SetTRS(base.transform.position, base.transform.rotation, Vector3.one);
		}

		// Token: 0x0600061D RID: 1565 RVA: 0x0002A728 File Offset: 0x00028928
		private void UpdateExposures()
		{
			this.exposures.x = this.masterIntensity * this.diffIntensity;
			this.exposures.y = this.masterIntensity * this.specIntensity;
			this.exposures.z = this.masterIntensity * this.skyIntensity * this.camExposure;
			this.exposures.w = this.camExposure;
			float num = 2.2f;
			float p = 1f / num;
			this.hdrScale = 6f;
			if (this.linearSpace)
			{
				this.hdrScale = Mathf.Pow(6f, num);
			}
			else
			{
				this.exposures.x = Mathf.Pow(this.exposures.x, p);
				this.exposures.y = Mathf.Pow(this.exposures.y, p);
				this.exposures.z = Mathf.Pow(this.exposures.z, p);
				this.exposures.w = Mathf.Pow(this.exposures.w, p);
			}
			if (this.hdrDiff)
			{
				this.exposures.x = this.exposures.x * this.hdrScale;
			}
			if (this.hdrSpec)
			{
				this.exposures.y = this.exposures.y * this.hdrScale;
			}
			if (this.hdrSky)
			{
				this.exposures.z = this.exposures.z * this.hdrScale;
			}
			this.exposuresLM.x = this.diffIntensityLM;
			this.exposuresLM.y = this.specIntensityLM;
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x0002A8CC File Offset: 0x00028ACC
		private void ApplyExposures(Renderer target)
		{
			if (target == null)
			{
				Shader.SetGlobalVector("ExposureIBL", this.exposures);
				Shader.SetGlobalVector("ExposureLM", this.exposuresLM);
				Shader.SetGlobalFloat("_EmissionLM", 1f);
				Shader.SetGlobalVector("UniformOcclusion", Vector4.one);
			}
			else
			{
				target.material.SetVector("ExposureIBL", this.exposures);
				target.material.SetVector("ExposureLM", this.exposuresLM);
			}
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x0002A960 File Offset: 0x00028B60
		private void ApplyIBL(Renderer target)
		{
			float value = 1f / this.hdrScale / 3.1415927f;
			if (target == null)
			{
				if (this.diffuseCube)
				{
					Shader.SetGlobalTexture("_DiffCubeIBL", this.diffuseCube);
				}
				else
				{
					Shader.SetGlobalTexture("_DiffCubeIBL", this.blackCube);
				}
				if (this.specularCube)
				{
					Shader.SetGlobalTexture("_SpecCubeIBL", this.specularCube);
				}
				else
				{
					Shader.SetGlobalTexture("_SpecCubeIBL", this.blackCube);
				}
				if (this.skyboxCube)
				{
					Shader.SetGlobalTexture("_SkyCubeIBL", this.skyboxCube);
				}
				else
				{
					Shader.SetGlobalTexture("_SkyCubeIBL", this.blackCube);
				}
				if (this.SH != null)
				{
					for (uint num = 0U; num < 9U; num += 1U)
					{
						Shader.SetGlobalVector("_SH" + num, this.SH.cBuffer[(int)((UIntPtr)num)]);
					}
					Shader.SetGlobalFloat("_SHScale", value);
				}
			}
			else
			{
				if (this.diffuseCube)
				{
					target.material.SetTexture("_DiffCubeIBL", this.diffuseCube);
				}
				else
				{
					target.material.SetTexture("_DiffCubeIBL", this.blackCube);
				}
				if (this.specularCube)
				{
					target.material.SetTexture("_SpecCubeIBL", this.specularCube);
				}
				else
				{
					target.material.SetTexture("_SpecCubeIBL", this.blackCube);
				}
				if (this.skyboxCube)
				{
					target.material.SetTexture("_SkyCubeIBL", this.skyboxCube);
				}
				else
				{
					target.material.SetTexture("_SkyCubeIBL", this.blackCube);
				}
				if (this.SH != null)
				{
					for (int i = 0; i < 9; i++)
					{
						target.material.SetVector("_SH" + i, this.SH.cBuffer[i]);
					}
					target.material.SetFloat("_SHScale", value);
				}
			}
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x0002ABA8 File Offset: 0x00028DA8
		private void ApplySkybox()
		{
			Shader.DisableKeyword("MARMO_RGBM");
			Shader.EnableKeyword("MARMO_RGBA");
			if (this.showSkybox)
			{
				if (RenderSettings.skybox != this.skyboxMaterial)
				{
					RenderSettings.skybox = this.skyboxMaterial;
				}
			}
			else if (RenderSettings.skybox && RenderSettings.skybox.name == "Internal IBL Skybox")
			{
				RenderSettings.skybox = null;
			}
		}

		// Token: 0x06000621 RID: 1569 RVA: 0x0002AC28 File Offset: 0x00028E28
		private void Reset()
		{
			this.skyMatrix = Matrix4x4.identity;
			this.exposures = Vector4.one;
			this.exposuresLM = Vector2.one;
			this.hdrScale = 1f;
			this.diffuseCube = (this.specularCube = (this.skyboxCube = null));
			this.masterIntensity = (this.skyIntensity = (this.specIntensity = (this.diffIntensity = 1f)));
			this.hdrSky = (this.hdrSpec = (this.hdrDiff = false));
		}

		// Token: 0x06000622 RID: 1570 RVA: 0x0002ACBC File Offset: 0x00028EBC
		private void OnEnable()
		{
			if (this.SH == null)
			{
				this.SH = new SHEncoding();
			}
			this.SH.copyToBuffer();
		}

		// Token: 0x06000623 RID: 1571 RVA: 0x0002ACE0 File Offset: 0x00028EE0
		private void Start()
		{
			this.Apply();
		}

		// Token: 0x06000624 RID: 1572 RVA: 0x0002ACE8 File Offset: 0x00028EE8
		private void Update()
		{
			if (Sky.activeSky == this && base.transform.hasChanged)
			{
				this.ApplySkyTransform();
			}
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x0002AD1C File Offset: 0x00028F1C
		private void OnDestroy()
		{
			UnityEngine.Object.DestroyImmediate(this._skyboxMaterial, false);
			this.SH = null;
			this._skyboxMaterial = null;
			this._blackCube = null;
			this.diffuseCube = null;
			this.specularCube = null;
			this.skyboxCube = null;
		}

		// Token: 0x06000626 RID: 1574 RVA: 0x0002AD60 File Offset: 0x00028F60
		private void OnDrawGizmos()
		{
			if (Sky.activeSky == null)
			{
				this.Apply();
			}
			Gizmos.DrawIcon(base.transform.position, "cubelight.tga", true);
			if (this.hasDimensions)
			{
				Color color = new Color(0.4f, 0.7f, 1f, 0.333f);
				Gizmos.matrix = base.transform.localToWorldMatrix;
				Gizmos.color = color;
				Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
			}
		}

		// Token: 0x06000627 RID: 1575 RVA: 0x0002ADE4 File Offset: 0x00028FE4
		private void OnDrawGizmosSelected()
		{
			this.Apply();
			if (this.hasDimensions)
			{
				Color color = new Color(0.4f, 0.7f, 1f, 1f);
				Gizmos.matrix = base.transform.localToWorldMatrix;
				Gizmos.color = color;
				Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
			}
		}

		// Token: 0x04000650 RID: 1616
		public static Sky activeSky;

		// Token: 0x04000651 RID: 1617
		public Cubemap diffuseCube;

		// Token: 0x04000652 RID: 1618
		public Cubemap specularCube;

		// Token: 0x04000653 RID: 1619
		public Cubemap skyboxCube;

		// Token: 0x04000654 RID: 1620
		public float masterIntensity = 1f;

		// Token: 0x04000655 RID: 1621
		public float skyIntensity = 1f;

		// Token: 0x04000656 RID: 1622
		public float specIntensity = 1f;

		// Token: 0x04000657 RID: 1623
		public float diffIntensity = 1f;

		// Token: 0x04000658 RID: 1624
		public float camExposure = 1f;

		// Token: 0x04000659 RID: 1625
		public float specIntensityLM = 1f;

		// Token: 0x0400065A RID: 1626
		public float diffIntensityLM = 1f;

		// Token: 0x0400065B RID: 1627
		public bool hdrSky;

		// Token: 0x0400065C RID: 1628
		public bool hdrSpec;

		// Token: 0x0400065D RID: 1629
		public bool hdrDiff;

		// Token: 0x0400065E RID: 1630
		public bool showSkybox = true;

		// Token: 0x0400065F RID: 1631
		public bool linearSpace = true;

		// Token: 0x04000660 RID: 1632
		public bool autoDetectColorSpace = true;

		// Token: 0x04000661 RID: 1633
		public bool hasDimensions;

		// Token: 0x04000662 RID: 1634
		public SHEncoding SH;

		// Token: 0x04000663 RID: 1635
		private Matrix4x4 skyMatrix = Matrix4x4.identity;

		// Token: 0x04000664 RID: 1636
		private Vector4 exposures = Vector4.one;

		// Token: 0x04000665 RID: 1637
		private Vector2 exposuresLM = Vector2.one;

		// Token: 0x04000666 RID: 1638
		private float hdrScale = 1f;

		// Token: 0x04000667 RID: 1639
		private Material _skyboxMaterial;

		// Token: 0x04000668 RID: 1640
		private Cubemap _blackCube;
	}
}
