using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto
{
	// Token: 0x02000083 RID: 131
	[AddComponentMenu("Ceto/Overlays/AddFoamTrail")]
	public class AddFoamTrail : AddWaveOverlayBase
	{
		// Token: 0x06000371 RID: 881 RVA: 0x00013F80 File Offset: 0x00012180
		protected override void Start()
		{
			this.m_lastPosition = base.transform.position;
		}

		// Token: 0x06000372 RID: 882 RVA: 0x00013F94 File Offset: 0x00012194
		protected override void Update()
		{
			this.UpdateOverlays();
			this.AddFoam();
			this.RemoveOverlays();
		}

		// Token: 0x06000373 RID: 883 RVA: 0x00013FA8 File Offset: 0x000121A8
		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_lastPosition = base.transform.position;
		}

		// Token: 0x06000374 RID: 884 RVA: 0x00013FC4 File Offset: 0x000121C4
		public override void Translate(Vector3 amount)
		{
			base.Translate(amount);
			this.m_lastPosition += amount;
		}

		// Token: 0x06000375 RID: 885 RVA: 0x00013FE0 File Offset: 0x000121E0
		private float Rotation()
		{
			switch (this.rotation)
			{
			case AddFoamTrail.ROTATION.NONE:
				return 0f;
			case AddFoamTrail.ROTATION.RANDOM:
				return UnityEngine.Random.Range(0f, 360f);
			case AddFoamTrail.ROTATION.RELATIVE:
				return base.transform.eulerAngles.y;
			default:
				return 0f;
			}
		}

		// Token: 0x06000376 RID: 886 RVA: 0x0001403C File Offset: 0x0001223C
		private void AddFoam()
		{
			if (this.duration <= 0f || Ocean.Instance == null)
			{
				this.m_lastPosition = base.transform.position;
				return;
			}
			this.spacing = Mathf.Max(0.01f, this.spacing);
			this.size = Mathf.Max(0.001f, this.size);
			Vector3 position = base.transform.position;
			float num = position.y;
			if (this.mustBeBelowWater)
			{
				num = Ocean.Instance.QueryWaves(position.x, position.z);
			}
			if (num < position.y)
			{
				this.m_lastPosition = position;
				return;
			}
			position.y = 0f;
			this.m_lastPosition.y = 0f;
			Vector3 vector = this.m_lastPosition - position;
			Vector3 normalized = vector.normalized;
			float num2 = vector.magnitude;
			if (num2 < this.MIN_MOVEMENT)
			{
				return;
			}
			num2 = Mathf.Min(this.MAX_MOVEMENT, num2);
			Vector3 vector2 = normalized * this.momentum;
			this.m_remainingDistance += num2;
			float num3 = 0f;
			while (this.m_remainingDistance > this.spacing)
			{
				Vector3 pos = position + normalized * num3;
				FoamOverlay foamOverlay = new FoamOverlay(pos, this.Rotation(), this.size, this.duration, this.foamTexture);
				foamOverlay.FoamTex.alpha = 0f;
				foamOverlay.FoamTex.textureFoam = this.textureFoam;
				foamOverlay.Momentum = vector2;
				foamOverlay.Spin = ((UnityEngine.Random.value <= 0.5f) ? this.spin : (-this.spin));
				foamOverlay.Expansion = this.expansion;
				if (this.jitter > 0f)
				{
					foamOverlay.Spin *= 1f + UnityEngine.Random.Range(-1f, 1f) * this.jitter;
					foamOverlay.Expansion *= 1f + UnityEngine.Random.Range(-1f, 1f) * this.jitter;
				}
				this.m_overlays.Add(foamOverlay);
				Ocean.Instance.OverlayManager.Add(foamOverlay);
				this.m_remainingDistance -= this.spacing;
				num3 += this.spacing;
			}
			this.m_lastPosition = position;
		}

		// Token: 0x06000377 RID: 887 RVA: 0x000142C0 File Offset: 0x000124C0
		private void UpdateOverlays()
		{
			foreach (WaveOverlay waveOverlay in this.m_overlays)
			{
				float normalizedAge = waveOverlay.NormalizedAge;
				IEnumerator<WaveOverlay> enumerator;
				enumerator.Current.FoamTex.alpha = this.timeLine.Evaluate(normalizedAge) * this.alpha;
				enumerator.Current.FoamTex.textureFoam = this.textureFoam;
				enumerator.Current.UpdateOverlay();
			}
		}

		// Token: 0x06000378 RID: 888 RVA: 0x0001433C File Offset: 0x0001253C
		private void RemoveOverlays()
		{
			LinkedList<WaveOverlay> linkedList = new LinkedList<WaveOverlay>();
			foreach (WaveOverlay waveOverlay in this.m_overlays)
			{
				if (waveOverlay.Age >= waveOverlay.Duration)
				{
					linkedList.AddLast(waveOverlay);
					waveOverlay.Kill = true;
				}
			}
			foreach (WaveOverlay item in linkedList)
			{
				this.m_overlays.Remove(item);
			}
		}

		// Token: 0x06000379 RID: 889 RVA: 0x000143C0 File Offset: 0x000125C0
		private void OnDrawGizmos()
		{
			if (!base.enabled)
			{
				return;
			}
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(base.transform.position, 2f);
		}

		// Token: 0x0400038A RID: 906
		private readonly float MIN_MOVEMENT = 0.01f;

		// Token: 0x0400038B RID: 907
		private readonly float MAX_MOVEMENT = 100f;

		// Token: 0x0400038C RID: 908
		public Texture foamTexture;

		// Token: 0x0400038D RID: 909
		public bool textureFoam = true;

		// Token: 0x0400038E RID: 910
		public AddFoamTrail.ROTATION rotation = AddFoamTrail.ROTATION.RANDOM;

		// Token: 0x0400038F RID: 911
		public AnimationCurve timeLine = AddWaveOverlayBase.DefaultCurve();

		// Token: 0x04000390 RID: 912
		public float duration = 10f;

		// Token: 0x04000391 RID: 913
		public float size = 0.01f;

		// Token: 0x04000392 RID: 914
		public float spacing = 0.01f;

		// Token: 0x04000393 RID: 915
		public float expansion = 1f;

		// Token: 0x04000394 RID: 916
		public float momentum = 1f;

		// Token: 0x04000395 RID: 917
		public float spin = 10f;

		// Token: 0x04000396 RID: 918
		public bool mustBeBelowWater = true;

		// Token: 0x04000397 RID: 919
		[Range(0f, 2f)]
		public float alpha = 0.8f;

		// Token: 0x04000398 RID: 920
		[Range(0f, 1f)]
		public float jitter = 0.2f;

		// Token: 0x04000399 RID: 921
		private Vector3 m_lastPosition;

		// Token: 0x0400039A RID: 922
		private float m_remainingDistance;

		// Token: 0x02000084 RID: 132
		public enum ROTATION
		{
			// Token: 0x0400039C RID: 924
			NONE,
			// Token: 0x0400039D RID: 925
			RANDOM,
			// Token: 0x0400039E RID: 926
			RELATIVE
		}
	}
}
