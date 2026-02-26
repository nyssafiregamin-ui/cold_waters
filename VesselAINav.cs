using System;
using UnityEngine;

// Token: 0x0200016C RID: 364
public class VesselAINav : MonoBehaviour
{
	// Token: 0x06000B1A RID: 2842 RVA: 0x0009D1A8 File Offset: 0x0009B3A8
	private void OnTriggerEnter(Collider otherObject)
	{
		if (otherObject.transform.parent != null && otherObject.transform.parent.gameObject.name == "Ice")
		{
			if (this.vesselai.distanceToAvoidIceBy < otherObject.GetComponent<SphereCollider>().radius * 2f)
			{
				this.vesselai.distanceToAvoidIceBy = otherObject.GetComponent<SphereCollider>().radius * 2f;
			}
			this.vesselai.avoidingIceberg = true;
			this.vesselai.icebergTransform = otherObject.transform;
			if (!this.vesselai.icebergPositions.Contains(otherObject.transform))
			{
				this.vesselai.icebergPositions.Add(otherObject.transform);
			}
			return;
		}
		if (this.vesselai.takingAction == 9)
		{
			return;
		}
		if (Mathf.Abs(otherObject.transform.position.y - this.vesselai.parentVessel.transform.position.y) < 0.3f)
		{
			this.vesselai.distanceToAvoidHazardBy = base.gameObject.GetComponent<SphereCollider>().radius * 1.9f;
			this.vesselai.AvoidHazard(otherObject.gameObject.transform);
		}
	}

	// Token: 0x040011C5 RID: 4549
	public VesselAI vesselai;
}
