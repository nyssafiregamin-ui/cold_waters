using System;
using UnityEngine;

namespace Ceto
{
	// Token: 0x020000C6 RID: 198
	[RequireComponent(typeof(Camera))]
	public class DisplayTexture : MonoBehaviour
	{
		// Token: 0x060005AB RID: 1451 RVA: 0x00025A9C File Offset: 0x00023C9C
		private void Start()
		{
		}

		// Token: 0x060005AC RID: 1452 RVA: 0x00025AA0 File Offset: 0x00023CA0
		private void OnGUI()
		{
			if (Ocean.Instance == null)
			{
				return;
			}
			Camera component = base.GetComponent<Camera>();
			CameraData cameraData = Ocean.Instance.FindCameraData(component);
			if (cameraData == null)
			{
				return;
			}
			Texture texture = this.FindTexture(cameraData, component);
			if (texture == null)
			{
				return;
			}
			int num;
			int num2;
			if ((texture.width == Screen.width && texture.height == Screen.height) || (texture.width == Screen.width / 2 && texture.height == Screen.height / 2))
			{
				num = Screen.width / ((!this.enlarge) ? 3 : 2);
				num2 = Screen.height / ((!this.enlarge) ? 3 : 2);
			}
			else
			{
				num = 256 * ((!this.enlarge) ? 1 : 2);
				num2 = 256 * ((!this.enlarge) ? 1 : 2);
			}
			GUI.DrawTexture(new Rect((float)(Screen.width - num - 5), 5f, (float)num, (float)num2), texture, ScaleMode.StretchToFill, false);
		}

		// Token: 0x060005AD RID: 1453 RVA: 0x00025BC0 File Offset: 0x00023DC0
		private Texture FindTexture(CameraData data, Camera cam)
		{
			if (Ocean.Instance == null)
			{
				return null;
			}
			WaveSpectrum component = Ocean.Instance.GetComponent<WaveSpectrum>();
			switch (this.display)
			{
			case DisplayTexture.DISPLAY.OVERLAY_HEIGHT:
				return (data.overlay != null) ? data.overlay.height : null;
			case DisplayTexture.DISPLAY.OVERLAY_NORMAL:
				return (data.overlay != null) ? data.overlay.normal : null;
			case DisplayTexture.DISPLAY.OVERLAY_FOAM:
				return (data.overlay != null) ? data.overlay.foam : null;
			case DisplayTexture.DISPLAY.OVERLAY_CLIP:
				return (data.overlay != null) ? data.overlay.clip : null;
			case DisplayTexture.DISPLAY.REFLECTION:
				return (data.reflection != null) ? data.reflection.tex : null;
			case DisplayTexture.DISPLAY.OCEAN_MASK:
				return (data.mask != null) ? data.mask.cam.targetTexture : null;
			case DisplayTexture.DISPLAY.OCEAN_DEPTH:
				return (data.depth != null && !(data.depth.cam == null)) ? data.depth.cam.targetTexture : null;
			case DisplayTexture.DISPLAY.WAVE_SLOPEMAP0:
				return (!(component == null)) ? component.SlopeMaps[0] : null;
			case DisplayTexture.DISPLAY.WAVE_SLOPEMAP1:
				return (!(component == null)) ? component.SlopeMaps[1] : null;
			case DisplayTexture.DISPLAY.WAVE_DISPLACEMENTMAP0:
				return (!(component == null)) ? component.DisplacementMaps[0] : null;
			case DisplayTexture.DISPLAY.WAVE_DISPLACEMENTMAP1:
				return (!(component == null)) ? component.DisplacementMaps[1] : null;
			case DisplayTexture.DISPLAY.WAVE_DISPLACEMENTMAP2:
				return (!(component == null)) ? component.DisplacementMaps[2] : null;
			case DisplayTexture.DISPLAY.WAVE_DISPLACEMENTMAP3:
				return (!(component == null)) ? component.DisplacementMaps[3] : null;
			case DisplayTexture.DISPLAY.WAVE_FOAM0:
				return (!(component == null)) ? component.FoamMaps[0] : null;
			case DisplayTexture.DISPLAY.WAVE_FOAM1:
				return (!(component == null)) ? component.FoamMaps[1] : null;
			default:
				return null;
			}
		}

		// Token: 0x04000585 RID: 1413
		public bool enlarge;

		// Token: 0x04000586 RID: 1414
		public DisplayTexture.DISPLAY display;

		// Token: 0x020000C7 RID: 199
		public enum DISPLAY
		{
			// Token: 0x04000588 RID: 1416
			NONE,
			// Token: 0x04000589 RID: 1417
			OVERLAY_HEIGHT,
			// Token: 0x0400058A RID: 1418
			OVERLAY_NORMAL,
			// Token: 0x0400058B RID: 1419
			OVERLAY_FOAM,
			// Token: 0x0400058C RID: 1420
			OVERLAY_CLIP,
			// Token: 0x0400058D RID: 1421
			REFLECTION,
			// Token: 0x0400058E RID: 1422
			OCEAN_MASK,
			// Token: 0x0400058F RID: 1423
			OCEAN_DEPTH,
			// Token: 0x04000590 RID: 1424
			WAVE_SLOPEMAP0,
			// Token: 0x04000591 RID: 1425
			WAVE_SLOPEMAP1,
			// Token: 0x04000592 RID: 1426
			WAVE_DISPLACEMENTMAP0,
			// Token: 0x04000593 RID: 1427
			WAVE_DISPLACEMENTMAP1,
			// Token: 0x04000594 RID: 1428
			WAVE_DISPLACEMENTMAP2,
			// Token: 0x04000595 RID: 1429
			WAVE_DISPLACEMENTMAP3,
			// Token: 0x04000596 RID: 1430
			WAVE_FOAM0,
			// Token: 0x04000597 RID: 1431
			WAVE_FOAM1
		}
	}
}
