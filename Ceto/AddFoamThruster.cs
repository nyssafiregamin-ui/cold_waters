using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto
{
	// Token: 0x02000081 RID: 129
	[AddComponentMenu("Ceto/Overlays/AddFoamThruster")]
	public class AddFoamThruster : AddWaveOverlayBase
	{
		// Token: 0x06000367 RID: 871 RVA: 0x00013A90 File Offset: 0x00011C90
		protected override void Start()
		{
			this.m_lastTime = Time.time;
		}

		// Token: 0x06000368 RID: 872 RVA: 0x00013AA0 File Offset: 0x00011CA0
		protected override void Update()
		{
			this.UpdateOverlays();
			this.AddFoam();
			this.RemoveOverlays();
		}

		// Token: 0x06000369 RID: 873 RVA: 0x00013AB4 File Offset: 0x00011CB4
		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_lastTime = Time.time;
		}

		// Token: 0x0600036A RID: 874 RVA: 0x00013AC8 File Offset: 0x00011CC8
		public override void Translate(Vector3 amount)
		{
			base.Translate(amount);
		}

		// Token: 0x0600036B RID: 875 RVA: 0x00013AD4 File Offset: 0x00011CD4
		private float Rotation()
		{
			switch (this.rotation)
			{
			case AddFoamThruster.ROTATION.NONE:
				return 0f;
			case AddFoamThruster.ROTATION.RANDOM:
				return UnityEngine.Random.Range(0f, 360f);
			case AddFoamThruster.ROTATION.RELATIVE:
				return base.transform.eulerAngles.y;
			default:
				return 0f;
			}
		}

		// Token: 0x0600036C RID: 876 RVA: 0x00013B30 File Offset: 0x00011D30
		private void AddFoam()
		{
			if (this.duration <= 0f || Ocean.Instance == null)
			{
				this.m_lastTime = Time.time;
				return;
			}
			this.size = Mathf.Max(1f, this.size);
			float num = base.transform.position.y;
			Vector3 position = base.transform.position;
			Vector3 a = base.transform.forward;
			if (this.mustBeBelowWater)
			{
				num = Ocean.Instance.QueryWaves(position.x, position.z);
			}
			if (num < position.y || (a.x == 0f && a.z == 0f))
			{
				this.m_lastTime = Time.time;
				return;
			}
			float num2 = Time.time - this.m_lastTime;
			a = a.normalized;
			position.y = 0f;
			Vector3 vector = a * this.momentum;
			this.m_remainingTime += num2;
			float num3 = this.rate / 1000f;
			float num4 = 0f;
			while (this.m_remainingTime > num3)
			{
				Vector3 pos = position + a * num4;
				FoamOverlay foamOverlay = new FoamOverlay(pos, this.Rotation(), this.size, this.duration, this.foamTexture);
				foamOverlay.FoamTex.alpha = 0f;
				foamOverlay.FoamTex.textureFoam = this.textureFoam;
				foamOverlay.Momentum = vector;
				foamOverlay.Spin = ((UnityEngine.Random.value <= 0.5f) ? this.spin : (-this.spin));
				foamOverlay.Expansion = this.expansion;
				if (this.jitter > 0f)
				{
					foamOverlay.Spin *= 1f + UnityEngine.Random.Range(-1f, 1f) * this.jitter;
					foamOverlay.Expansion *= 1f + UnityEngine.Random.Range(-1f, 1f) * this.jitter;
				}
				this.m_overlays.Add(foamOverlay);
				Ocean.Instance.OverlayManager.Add(foamOverlay);
				this.m_remainingTime -= num3;
				num4 += num3;
			}
			this.m_lastTime = Time.time;
		}

		// Token: 0x0600036D RID: 877 RVA: 0x00013DA4 File Offset: 0x00011FA4
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

		// Token: 0x0600036E RID: 878 RVA: 0x00013E20 File Offset: 0x00012020
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

		// Token: 0x0600036F RID: 879 RVA: 0x00013EA4 File Offset: 0x000120A4
		private void OnDrawGizmos()
		{
			if (!base.enabled)
			{
				return;
			}
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(base.transform.position, 2f);
		}

		// Token: 0x04000377 RID: 887
		public Texture foamTexture;

		// Token: 0x04000378 RID: 888
		public bool textureFoam = true;

		// Token: 0x04000379 RID: 889
		public AddFoamThruster.ROTATION rotation = AddFoamThruster.ROTATION.RANDOM;

		// Token: 0x0400037A RID: 890
		public AnimationCurve timeLine = AddWaveOverlayBase.DefaultCurve();

		// Token: 0x0400037B RID: 891
		public float duration = 4f;

		// Token: 0x0400037C RID: 892
		public float size = 2f;

		// Token: 0x0400037D RID: 893
		[Range(16f, 1000f)]
		public float rate = 128f;

		// Token: 0x0400037E RID: 894
		public float expansion = 4f;

		// Token: 0x0400037F RID: 895
		public float momentum = 10f;

		// Token: 0x04000380 RID: 896
		public float spin = 10f;

		// Token: 0x04000381 RID: 897
		public bool mustBeBelowWater = true;

		// Token: 0x04000382 RID: 898
		[Range(0f, 2f)]
		public float alpha = 0.8f;

		// Token: 0x04000383 RID: 899
		[Range(0f, 1f)]
		public float jitter = 0.2f;

		// Token: 0x04000384 RID: 900
		private float m_lastTime;

		// Token: 0x04000385 RID: 901
		private float m_remainingTime;

		// Token: 0x02000082 RID: 130
		public enum ROTATION
		{
			// Token: 0x04000387 RID: 903
			NONE,
			// Token: 0x04000388 RID: 904
			RANDOM,
			// Token: 0x04000389 RID: 905
			RELATIVE
		}
	}
}
