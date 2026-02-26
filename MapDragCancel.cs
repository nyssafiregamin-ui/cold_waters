using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Token: 0x02000136 RID: 310
public class MapDragCancel : MonoBehaviour, IEventSystemHandler, IDragHandler
{
	// Token: 0x06000873 RID: 2163 RVA: 0x0005E5EC File Offset: 0x0005C7EC
	public void OnDrag(PointerEventData data)
	{
		if (UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.autoCentreMap)
		{
			UIFunctions.globaluifunctions.playerfunctions.sensormanager.tacticalmap.autoCentreMap = false;
			UIFunctions.globaluifunctions.playerfunctions.helmmanager.guiButtonImages[23].color = UIFunctions.globaluifunctions.playerfunctions.helmmanager.buttonColors[0];
		}
	}
}
