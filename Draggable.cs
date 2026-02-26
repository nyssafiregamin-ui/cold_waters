using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200010F RID: 271
public class Draggable : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
{
	// Token: 0x0600073E RID: 1854 RVA: 0x0004023C File Offset: 0x0003E43C
	private void Awake()
	{
		Draggable.instance = this;
	}

	// Token: 0x0600073F RID: 1855 RVA: 0x00040244 File Offset: 0x0003E444
	private void Start()
	{
		this.xConstraints = new Vector2(this.thisImage.rectTransform.sizeDelta.x / 2f, this.parentImage.rectTransform.sizeDelta.x - this.thisImage.rectTransform.sizeDelta.x / 2f);
		this.yConstraints = new Vector2(this.thisImage.rectTransform.sizeDelta.y / 2f, this.parentImage.rectTransform.sizeDelta.y - this.thisImage.rectTransform.sizeDelta.y / 2f);
		this.xConstraints.x = this.xConstraints.x - this.parentImage.rectTransform.sizeDelta.x / 2f;
		this.xConstraints.y = this.xConstraints.y - this.parentImage.rectTransform.sizeDelta.x / 2f;
		this.yConstraints.x = this.yConstraints.x - this.parentImage.rectTransform.sizeDelta.y / 2f;
		this.yConstraints.y = this.yConstraints.y - this.parentImage.rectTransform.sizeDelta.y / 2f;
	}

	// Token: 0x06000740 RID: 1856 RVA: 0x000403D8 File Offset: 0x0003E5D8
	public void OnBeginDrag(PointerEventData eventData)
	{
	}

	// Token: 0x06000741 RID: 1857 RVA: 0x000403DC File Offset: 0x0003E5DC
	public void OnDrag(PointerEventData eventData)
	{
		base.transform.position = UIFunctions.globaluifunctions.GUICameraObject.ScreenToWorldPoint(eventData.position);
		float num = base.transform.localPosition.x;
		float num2 = base.transform.localPosition.y;
		num = Mathf.Clamp(num, this.xConstraints.x, this.xConstraints.y);
		num2 = Mathf.Clamp(num2, this.yConstraints.x, this.yConstraints.y);
		base.transform.localPosition = new Vector3(num, num2, 0f);
		this.CalculateMapCoords();
	}

	// Token: 0x06000742 RID: 1858 RVA: 0x00040490 File Offset: 0x0003E690
	public void OnEndDrag(PointerEventData eventData)
	{
		this.CalculateMapCoords();
	}

	// Token: 0x06000743 RID: 1859 RVA: 0x00040498 File Offset: 0x0003E698
	public void CalculateMapCoords()
	{
		Vector2 vector = new Vector2(base.transform.localPosition.x, base.transform.localPosition.y);
		vector.x += this.parentImage.rectTransform.sizeDelta.x / 2f;
		vector.y += this.parentImage.rectTransform.sizeDelta.y / 2f;
		vector.x = vector.x / this.parentImage.rectTransform.sizeDelta.x * 4096f;
		vector.y = vector.y / this.parentImage.rectTransform.sizeDelta.y * 2048f;
		EditorMission.instance.mapCoordsX.text = string.Format("{0:0}", vector.x);
		EditorMission.instance.mapCoordsY.text = string.Format("{0:0}", vector.y);
	}

	// Token: 0x040009CB RID: 2507
	public static Draggable instance;

	// Token: 0x040009CC RID: 2508
	private Vector3 positionToReturnTo;

	// Token: 0x040009CD RID: 2509
	public Image parentImage;

	// Token: 0x040009CE RID: 2510
	public Image thisImage;

	// Token: 0x040009CF RID: 2511
	public Vector2 xConstraints;

	// Token: 0x040009D0 RID: 2512
	public Vector2 yConstraints;
}
