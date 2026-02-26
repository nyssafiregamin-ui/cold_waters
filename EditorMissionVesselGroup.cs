using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000111 RID: 273
public class EditorMissionVesselGroup : MonoBehaviour
{
	// Token: 0x04000A13 RID: 2579
	public Toggle useGroup;

	// Token: 0x04000A14 RID: 2580
	public Toggle aggressive;

	// Token: 0x04000A15 RID: 2581
	public Dropdown minVessels;

	// Token: 0x04000A16 RID: 2582
	public Dropdown maxVessels;

	// Token: 0x04000A17 RID: 2583
	public Text groupName;

	// Token: 0x04000A18 RID: 2584
	public Text toLabel;

	// Token: 0x04000A19 RID: 2585
	public Text numInGroup;

	// Token: 0x04000A1A RID: 2586
	public Button viewEdit;

	// Token: 0x04000A1B RID: 2587
	public List<int> groupVesselList;
}
