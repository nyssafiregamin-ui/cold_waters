using System;
using UnityEngine;

// Token: 0x02000158 RID: 344
public class SkyObjectCenterer : MonoBehaviour
{
	// Token: 0x06000A13 RID: 2579 RVA: 0x0007B2EC File Offset: 0x000794EC
	private void LateUpdate()
	{
		if (GameDataManager.HUDActive)
		{
			base.transform.position = new Vector3(UIFunctions.globaluifunctions.MainCamera.transform.position.x, base.transform.position.y, UIFunctions.globaluifunctions.MainCamera.transform.position.z);
			if (LevelLoadManager.isRaining)
			{
				this.rainholder.transform.eulerAngles = new Vector3(0f, SkyObjectCenterer.rainangley + 160f, SkyObjectCenterer.rainAnglez);
			}
		}
	}

	// Token: 0x06000A14 RID: 2580 RVA: 0x0007B394 File Offset: 0x00079594
	public void ForceSkyboxPosition(Transform newCameraPosition)
	{
		ManualCameraZoom.previousTarget = ManualCameraZoom.target;
		ManualCameraZoom.target = newCameraPosition;
		base.transform.position = new Vector3(UIFunctions.globaluifunctions.MainCamera.transform.position.x, base.transform.position.y, UIFunctions.globaluifunctions.MainCamera.transform.position.z);
		this.manualcamerazoom.LateUpdate();
		if (LevelLoadManager.isRaining)
		{
			this.rainholder.transform.eulerAngles = new Vector3(0f, SkyObjectCenterer.rainangley + 160f, SkyObjectCenterer.rainAnglez);
		}
	}

	// Token: 0x04000F84 RID: 3972
	public Transform rainholder;

	// Token: 0x04000F85 RID: 3973
	public ManualCameraZoom manualcamerazoom;

	// Token: 0x04000F86 RID: 3974
	public static float rainAnglez;

	// Token: 0x04000F87 RID: 3975
	public static float rainangley;
}
