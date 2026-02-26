using System;
using UnityEngine;

// Token: 0x0200003B RID: 59
[ExecuteInEditMode]
public class Controls : MonoBehaviour
{
	// Token: 0x060001BF RID: 447 RVA: 0x0000C0BC File Offset: 0x0000A2BC
	private void OnGUI()
	{
		GUILayout.BeginArea(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height));
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Space(5f);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		this.occlusion.enabled = GUILayout.Toggle(this.occlusion.enabled, " Amplify Occlusion Enabled", new GUILayoutOption[0]);
		GUILayout.Space(5f);
		this.occlusion.ApplyMethod = ((!GUILayout.Toggle(this.occlusion.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.PostEffect, " Standard Post-effect", new GUILayoutOption[0])) ? this.occlusion.ApplyMethod : AmplifyOcclusionBase.ApplicationMethod.PostEffect);
		this.occlusion.ApplyMethod = ((!GUILayout.Toggle(this.occlusion.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.Deferred, " Deferred Injection", new GUILayoutOption[0])) ? this.occlusion.ApplyMethod : AmplifyOcclusionBase.ApplicationMethod.Deferred);
		this.occlusion.ApplyMethod = ((!GUILayout.Toggle(this.occlusion.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.Debug, " Debug Mode", new GUILayoutOption[0])) ? this.occlusion.ApplyMethod : AmplifyOcclusionBase.ApplicationMethod.Debug);
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Space(5f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Space(-3f);
		GUILayout.Label("Intensity     ", new GUILayoutOption[0]);
		GUILayout.EndVertical();
		this.occlusion.Intensity = GUILayout.HorizontalSlider(this.occlusion.Intensity, 0f, 1f, new GUILayoutOption[]
		{
			GUILayout.Width(100f)
		});
		GUILayout.Space(5f);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Space(-3f);
		GUILayout.Label(" " + this.occlusion.Intensity.ToString("0.00"), new GUILayoutOption[0]);
		GUILayout.EndVertical();
		GUILayout.Space(5f);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Space(-3f);
		GUILayout.Label("Power Exp. ", new GUILayoutOption[0]);
		GUILayout.EndVertical();
		this.occlusion.PowerExponent = GUILayout.HorizontalSlider(this.occlusion.PowerExponent, 0.0001f, 6f, new GUILayoutOption[]
		{
			GUILayout.Width(100f)
		});
		GUILayout.Space(5f);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Space(-3f);
		GUILayout.Label(" " + this.occlusion.PowerExponent.ToString("0.00"), new GUILayoutOption[0]);
		GUILayout.EndVertical();
		GUILayout.Space(5f);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Space(-3f);
		GUILayout.Label("Radius        ", new GUILayoutOption[0]);
		GUILayout.EndVertical();
		this.occlusion.Radius = GUILayout.HorizontalSlider(this.occlusion.Radius, 0.1f, 10f, new GUILayoutOption[]
		{
			GUILayout.Width(100f)
		});
		GUILayout.Space(5f);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Space(-3f);
		GUILayout.Label(" " + this.occlusion.Radius.ToString("0.00"), new GUILayoutOption[0]);
		GUILayout.EndVertical();
		GUILayout.Space(5f);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Space(-3f);
		GUILayout.Label("Quality        ", new GUILayoutOption[0]);
		GUILayout.EndVertical();
		this.occlusion.SampleCount = (AmplifyOcclusionBase.SampleCountLevel)GUILayout.HorizontalSlider((float)this.occlusion.SampleCount, 0f, 3f, new GUILayoutOption[]
		{
			GUILayout.Width(100f)
		});
		GUILayout.Space(5f);
		GUILayout.BeginVertical(new GUILayoutOption[0]);
		GUILayout.Space(-3f);
		GUILayout.Label("        ", new GUILayoutOption[0]);
		GUILayout.EndVertical();
		GUILayout.Space(5f);
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	// Token: 0x0400023A RID: 570
	private const AmplifyOcclusionBase.ApplicationMethod POST = AmplifyOcclusionBase.ApplicationMethod.PostEffect;

	// Token: 0x0400023B RID: 571
	private const AmplifyOcclusionBase.ApplicationMethod DEFERRED = AmplifyOcclusionBase.ApplicationMethod.Deferred;

	// Token: 0x0400023C RID: 572
	private const AmplifyOcclusionBase.ApplicationMethod DEBUG = AmplifyOcclusionBase.ApplicationMethod.Debug;

	// Token: 0x0400023D RID: 573
	public AmplifyOcclusionEffect occlusion;
}
