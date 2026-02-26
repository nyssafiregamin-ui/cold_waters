using System;
using UnityEngine;

// Token: 0x0200012F RID: 303
public class LOD : MonoBehaviour
{
	// Token: 0x0600082E RID: 2094 RVA: 0x0005295C File Offset: 0x00050B5C
	private void Start()
	{
		this.currentlod = 0;
		this.loddistances[0] = 11f;
		if (GameDataManager.graphicsdetail == 2)
		{
			this.minlod = 1;
			this.modifier = 10f;
			this.SetLod(1, 0);
			this.SetLod(1, 2);
			this.SetLod(1, 3);
		}
		else
		{
			this.minlod = 0;
			this.modifier = 0f;
			this.SetLod(0, 1);
			this.SetLod(0, 2);
			this.SetLod(0, 3);
		}
	}

	// Token: 0x0600082F RID: 2095 RVA: 0x000529E4 File Offset: 0x00050BE4
	private void Update()
	{
		if (!GameDataManager.HUDActive || base.transform.position.y > 1010f)
		{
			return;
		}
		float num = Vector3.Distance(base.transform.position, UIFunctions.globaluifunctions.MainCamera.transform.position);
		if (ManualCameraZoom.binoculars)
		{
			this.modifier = -80f;
		}
		else
		{
			this.modifier = 0f;
		}
		if (num < this.loddistances[0] - this.modifier && this.minlod < 1)
		{
			if (this.currentlod != 0)
			{
				this.SetLod(0, this.currentlod);
				this.currentlod = 0;
			}
		}
		else if (num < this.loddistances[1] - this.modifier && this.minlod < 2)
		{
			if (this.currentlod != 1)
			{
				this.SetLod(1, this.currentlod);
				this.currentlod = 1;
			}
		}
		else if (this.currentlod != 2)
		{
			this.SetLod(2, this.currentlod);
			this.currentlod = 2;
		}
	}

	// Token: 0x06000830 RID: 2096 RVA: 0x00052B10 File Offset: 0x00050D10
	private void SetLod(int lodlevel, int currentlod)
	{
		switch (currentlod)
		{
		case 0:
			foreach (MeshRenderer meshRenderer in this.lod0)
			{
				meshRenderer.enabled = false;
			}
			break;
		case 1:
			foreach (MeshRenderer meshRenderer2 in this.lod1)
			{
				meshRenderer2.enabled = false;
			}
			break;
		case 2:
			foreach (MeshRenderer meshRenderer3 in this.lod2)
			{
				meshRenderer3.enabled = false;
			}
			break;
		case 3:
			foreach (MeshRenderer meshRenderer4 in this.lod3)
			{
				meshRenderer4.enabled = false;
			}
			break;
		}
		switch (lodlevel)
		{
		case 0:
			foreach (MeshRenderer meshRenderer5 in this.lod0)
			{
				meshRenderer5.enabled = true;
			}
			if (!this.parentVessel.submerged)
			{
				this.BowWave(true);
			}
			this.PropsAndCav(true);
			break;
		case 1:
			foreach (MeshRenderer meshRenderer6 in this.lod1)
			{
				meshRenderer6.enabled = true;
			}
			if (!this.parentVessel.submerged)
			{
				this.BowWave(true);
			}
			this.PropsAndCav(false);
			break;
		case 2:
			foreach (MeshRenderer meshRenderer7 in this.lod2)
			{
				meshRenderer7.enabled = true;
			}
			this.BowWave(false);
			this.PropsAndCav(false);
			break;
		case 3:
			this.BowWave(false);
			this.PropsAndCav(false);
			break;
		}
	}

	// Token: 0x06000831 RID: 2097 RVA: 0x00052D20 File Offset: 0x00050F20
	public void PropsAndCav(bool enabled)
	{
	}

	// Token: 0x06000832 RID: 2098 RVA: 0x00052D24 File Offset: 0x00050F24
	public void BowWave(bool enabled)
	{
	}

	// Token: 0x04000BE3 RID: 3043
	public Vessel parentVessel;

	// Token: 0x04000BE4 RID: 3044
	public int currentlod;

	// Token: 0x04000BE5 RID: 3045
	public int minlod;

	// Token: 0x04000BE6 RID: 3046
	public bool displaylodanddistance;

	// Token: 0x04000BE7 RID: 3047
	public bool displaydistanceeveryframe;

	// Token: 0x04000BE8 RID: 3048
	public float[] loddistances;

	// Token: 0x04000BE9 RID: 3049
	public float modifier;

	// Token: 0x04000BEA RID: 3050
	public MeshRenderer[] lod0;

	// Token: 0x04000BEB RID: 3051
	public MeshRenderer[] lod1;

	// Token: 0x04000BEC RID: 3052
	public MeshRenderer[] lod2;

	// Token: 0x04000BED RID: 3053
	public MeshRenderer[] lod3;
}
