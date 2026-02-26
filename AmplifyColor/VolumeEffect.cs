using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AmplifyColor
{
	// Token: 0x0200002D RID: 45
	[Serializable]
	public class VolumeEffect
	{
		// Token: 0x0600016A RID: 362 RVA: 0x000092C0 File Offset: 0x000074C0
		public VolumeEffect(AmplifyColorBase effect)
		{
			this.gameObject = effect;
			this.components = new List<VolumeEffectComponent>();
		}

		// Token: 0x0600016B RID: 363 RVA: 0x000092DC File Offset: 0x000074DC
		public static VolumeEffect BlendValuesToVolumeEffect(VolumeEffectFlags flags, VolumeEffect volume1, VolumeEffect volume2, float blend)
		{
			VolumeEffect volumeEffect = new VolumeEffect(volume1.gameObject);
			foreach (VolumeEffectComponentFlags volumeEffectComponentFlags in flags.components)
			{
				if (volumeEffectComponentFlags.blendFlag)
				{
					VolumeEffectComponent volumeEffectComponent = volume1.FindEffectComponent(volumeEffectComponentFlags.componentName);
					VolumeEffectComponent volumeEffectComponent2 = volume2.FindEffectComponent(volumeEffectComponentFlags.componentName);
					if (volumeEffectComponent != null && volumeEffectComponent2 != null)
					{
						VolumeEffectComponent volumeEffectComponent3 = new VolumeEffectComponent(volumeEffectComponent.componentName);
						foreach (VolumeEffectFieldFlags volumeEffectFieldFlags in volumeEffectComponentFlags.componentFields)
						{
							if (volumeEffectFieldFlags.blendFlag)
							{
								VolumeEffectField volumeEffectField = volumeEffectComponent.FindEffectField(volumeEffectFieldFlags.fieldName);
								VolumeEffectField volumeEffectField2 = volumeEffectComponent2.FindEffectField(volumeEffectFieldFlags.fieldName);
								if (volumeEffectField != null && volumeEffectField2 != null)
								{
									VolumeEffectField volumeEffectField3 = new VolumeEffectField(volumeEffectField.fieldName, volumeEffectField.fieldType);
									string fieldType = volumeEffectField3.fieldType;
									switch (fieldType)
									{
									case "System.Single":
										volumeEffectField3.valueSingle = Mathf.Lerp(volumeEffectField.valueSingle, volumeEffectField2.valueSingle, blend);
										break;
									case "System.Boolean":
										volumeEffectField3.valueBoolean = volumeEffectField2.valueBoolean;
										break;
									case "UnityEngine.Vector2":
										volumeEffectField3.valueVector2 = Vector2.Lerp(volumeEffectField.valueVector2, volumeEffectField2.valueVector2, blend);
										break;
									case "UnityEngine.Vector3":
										volumeEffectField3.valueVector3 = Vector3.Lerp(volumeEffectField.valueVector3, volumeEffectField2.valueVector3, blend);
										break;
									case "UnityEngine.Vector4":
										volumeEffectField3.valueVector4 = Vector4.Lerp(volumeEffectField.valueVector4, volumeEffectField2.valueVector4, blend);
										break;
									case "UnityEngine.Color":
										volumeEffectField3.valueColor = Color.Lerp(volumeEffectField.valueColor, volumeEffectField2.valueColor, blend);
										break;
									}
									volumeEffectComponent3.fields.Add(volumeEffectField3);
								}
							}
						}
						volumeEffect.components.Add(volumeEffectComponent3);
					}
				}
			}
			return volumeEffect;
		}

		// Token: 0x0600016C RID: 364 RVA: 0x000095B4 File Offset: 0x000077B4
		public VolumeEffectComponent AddComponent(Component c, VolumeEffectComponentFlags compFlags)
		{
			if (compFlags == null)
			{
				VolumeEffectComponent volumeEffectComponent = new VolumeEffectComponent(c.GetType() + string.Empty);
				this.components.Add(volumeEffectComponent);
				return volumeEffectComponent;
			}
			VolumeEffectComponent volumeEffectComponent2;
			if ((volumeEffectComponent2 = this.FindEffectComponent(c.GetType() + string.Empty)) != null)
			{
				volumeEffectComponent2.UpdateComponent(c, compFlags);
				return volumeEffectComponent2;
			}
			VolumeEffectComponent volumeEffectComponent3 = new VolumeEffectComponent(c, compFlags);
			this.components.Add(volumeEffectComponent3);
			return volumeEffectComponent3;
		}

		// Token: 0x0600016D RID: 365 RVA: 0x00009628 File Offset: 0x00007828
		public void RemoveEffectComponent(VolumeEffectComponent comp)
		{
			this.components.Remove(comp);
		}

		// Token: 0x0600016E RID: 366 RVA: 0x00009638 File Offset: 0x00007838
		public void UpdateVolume()
		{
			if (this.gameObject == null)
			{
				return;
			}
			VolumeEffectFlags effectFlags = this.gameObject.EffectFlags;
			foreach (VolumeEffectComponentFlags volumeEffectComponentFlags in effectFlags.components)
			{
				if (volumeEffectComponentFlags.blendFlag)
				{
					Component component = this.gameObject.GetComponent(volumeEffectComponentFlags.componentName);
					if (component != null)
					{
						this.AddComponent(component, volumeEffectComponentFlags);
					}
				}
			}
		}

		// Token: 0x0600016F RID: 367 RVA: 0x000096EC File Offset: 0x000078EC
		public void SetValues(AmplifyColorBase targetColor)
		{
			VolumeEffectFlags effectFlags = targetColor.EffectFlags;
			GameObject gameObject = targetColor.gameObject;
			foreach (VolumeEffectComponentFlags volumeEffectComponentFlags in effectFlags.components)
			{
				if (volumeEffectComponentFlags.blendFlag)
				{
					Component component = gameObject.GetComponent(volumeEffectComponentFlags.componentName);
					VolumeEffectComponent volumeEffectComponent = this.FindEffectComponent(volumeEffectComponentFlags.componentName);
					if (!(component == null) && volumeEffectComponent != null)
					{
						foreach (VolumeEffectFieldFlags volumeEffectFieldFlags in volumeEffectComponentFlags.componentFields)
						{
							if (volumeEffectFieldFlags.blendFlag)
							{
								FieldInfo field = component.GetType().GetField(volumeEffectFieldFlags.fieldName);
								VolumeEffectField volumeEffectField = volumeEffectComponent.FindEffectField(volumeEffectFieldFlags.fieldName);
								if (field != null && volumeEffectField != null)
								{
									string fullName = field.FieldType.FullName;
									switch (fullName)
									{
									case "System.Single":
										field.SetValue(component, volumeEffectField.valueSingle);
										break;
									case "System.Boolean":
										field.SetValue(component, volumeEffectField.valueBoolean);
										break;
									case "UnityEngine.Vector2":
										field.SetValue(component, volumeEffectField.valueVector2);
										break;
									case "UnityEngine.Vector3":
										field.SetValue(component, volumeEffectField.valueVector3);
										break;
									case "UnityEngine.Vector4":
										field.SetValue(component, volumeEffectField.valueVector4);
										break;
									case "UnityEngine.Color":
										field.SetValue(component, volumeEffectField.valueColor);
										break;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000170 RID: 368 RVA: 0x00009988 File Offset: 0x00007B88
		public void BlendValues(AmplifyColorBase targetColor, VolumeEffect other, float blendAmount)
		{
			VolumeEffectFlags effectFlags = targetColor.EffectFlags;
			GameObject gameObject = targetColor.gameObject;
			for (int i = 0; i < effectFlags.components.Count; i++)
			{
				VolumeEffectComponentFlags volumeEffectComponentFlags = effectFlags.components[i];
				if (volumeEffectComponentFlags.blendFlag)
				{
					Component component = gameObject.GetComponent(volumeEffectComponentFlags.componentName);
					VolumeEffectComponent volumeEffectComponent = this.FindEffectComponent(volumeEffectComponentFlags.componentName);
					VolumeEffectComponent volumeEffectComponent2 = other.FindEffectComponent(volumeEffectComponentFlags.componentName);
					if (!(component == null) && volumeEffectComponent != null && volumeEffectComponent2 != null)
					{
						for (int j = 0; j < volumeEffectComponentFlags.componentFields.Count; j++)
						{
							VolumeEffectFieldFlags volumeEffectFieldFlags = volumeEffectComponentFlags.componentFields[j];
							if (volumeEffectFieldFlags.blendFlag)
							{
								FieldInfo field = component.GetType().GetField(volumeEffectFieldFlags.fieldName);
								VolumeEffectField volumeEffectField = volumeEffectComponent.FindEffectField(volumeEffectFieldFlags.fieldName);
								VolumeEffectField volumeEffectField2 = volumeEffectComponent2.FindEffectField(volumeEffectFieldFlags.fieldName);
								if (field != null && volumeEffectField != null && volumeEffectField2 != null)
								{
									string fullName = field.FieldType.FullName;
									switch (fullName)
									{
									case "System.Single":
										field.SetValue(component, Mathf.Lerp(volumeEffectField.valueSingle, volumeEffectField2.valueSingle, blendAmount));
										break;
									case "System.Boolean":
										field.SetValue(component, volumeEffectField2.valueBoolean);
										break;
									case "UnityEngine.Vector2":
										field.SetValue(component, Vector2.Lerp(volumeEffectField.valueVector2, volumeEffectField2.valueVector2, blendAmount));
										break;
									case "UnityEngine.Vector3":
										field.SetValue(component, Vector3.Lerp(volumeEffectField.valueVector3, volumeEffectField2.valueVector3, blendAmount));
										break;
									case "UnityEngine.Vector4":
										field.SetValue(component, Vector4.Lerp(volumeEffectField.valueVector4, volumeEffectField2.valueVector4, blendAmount));
										break;
									case "UnityEngine.Color":
										field.SetValue(component, Color.Lerp(volumeEffectField.valueColor, volumeEffectField2.valueColor, blendAmount));
										break;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00009C48 File Offset: 0x00007E48
		public VolumeEffectComponent FindEffectComponent(string compName)
		{
			for (int i = 0; i < this.components.Count; i++)
			{
				if (this.components[i].componentName == compName)
				{
					return this.components[i];
				}
			}
			return null;
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00009C9C File Offset: 0x00007E9C
		public static Component[] ListAcceptableComponents(AmplifyColorBase go)
		{
			if (go == null)
			{
				return new Component[0];
			}
			Component[] source = go.GetComponents(typeof(Component));
			return (from comp in source
			where comp != null && (!(comp.GetType() + string.Empty).StartsWith("UnityEngine.") && comp.GetType() != typeof(AmplifyColorBase))
			select comp).ToArray<Component>();
		}

		// Token: 0x06000173 RID: 371 RVA: 0x00009CF8 File Offset: 0x00007EF8
		public string[] GetComponentNames()
		{
			return (from r in this.components
			select r.componentName).ToArray<string>();
		}

		// Token: 0x040001A0 RID: 416
		public AmplifyColorBase gameObject;

		// Token: 0x040001A1 RID: 417
		public List<VolumeEffectComponent> components;
	}
}
