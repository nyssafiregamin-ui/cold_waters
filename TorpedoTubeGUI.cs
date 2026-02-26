using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000167 RID: 359
public class TorpedoTubeGUI : MonoBehaviour
{
	// Token: 0x06000ACA RID: 2762 RVA: 0x000961EC File Offset: 0x000943EC
	public void SetAttackSetting()
	{
		UIFunctions.globaluifunctions.playerfunctions.SetWeaponAttack();
	}

	// Token: 0x06000ACB RID: 2763 RVA: 0x00096200 File Offset: 0x00094400
	public void SetHomingSetting()
	{
		UIFunctions.globaluifunctions.playerfunctions.SetWeaponHoming();
	}

	// Token: 0x06000ACC RID: 2764 RVA: 0x00096214 File Offset: 0x00094414
	public void SetDepthSetting()
	{
		UIFunctions.globaluifunctions.playerfunctions.SetWeaponDepth();
	}

	// Token: 0x040010D4 RID: 4308
	public Button attackSettingButton;

	// Token: 0x040010D5 RID: 4309
	public Button depthSettingButton;

	// Token: 0x040010D6 RID: 4310
	public Button homeSettingButton;

	// Token: 0x040010D7 RID: 4311
	public Image tubeBackground;

	// Token: 0x040010D8 RID: 4312
	public Image weaponInTube;

	// Token: 0x040010D9 RID: 4313
	public Mask maskSprite;
}
