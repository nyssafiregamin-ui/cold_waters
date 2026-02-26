using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AmplifyColor
{
	// Token: 0x02000030 RID: 48
	[Serializable]
	public class VolumeEffectComponentFlags
	{
		// Token: 0x0600017F RID: 383 RVA: 0x00009F5C File Offset: 0x0000815C
		public VolumeEffectComponentFlags(string name)
		{
			this.componentName = name;
			this.componentFields = new List<VolumeEffectFieldFlags>();
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00009F78 File Offset: 0x00008178
		public VolumeEffectComponentFlags(VolumeEffectComponent comp) : this(comp.componentName)
		{
			this.blendFlag = true;
			foreach (VolumeEffectField volumeEffectField in comp.fields)
			{
				if (VolumeEffectField.IsValidType(volumeEffectField.fieldType))
				{
					this.componentFields.Add(new VolumeEffectFieldFlags(volumeEffectField));
				}
			}
		}

		// Token: 0x06000181 RID: 385 RVA: 0x0000A00C File Offset: 0x0000820C
		public VolumeEffectComponentFlags(Component c) : this(c.GetType() + string.Empty)
		{
			FieldInfo[] fields = c.GetType().GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				if (VolumeEffectField.IsValidType(fieldInfo.FieldType.FullName))
				{
					this.componentFields.Add(new VolumeEffectFieldFlags(fieldInfo));
				}
			}
		}

		// Token: 0x06000182 RID: 386 RVA: 0x0000A07C File Offset: 0x0000827C
		public void UpdateComponentFlags(VolumeEffectComponent comp)
		{
			VolumeEffectField field;
			foreach (VolumeEffectField field2 in comp.fields)
			{
				field = field2;
				if (this.componentFields.Find((VolumeEffectFieldFlags s) => s.fieldName == field.fieldName) == null && VolumeEffectField.IsValidType(field.fieldType))
				{
					this.componentFields.Add(new VolumeEffectFieldFlags(field));
				}
			}
		}

		// Token: 0x06000183 RID: 387 RVA: 0x0000A130 File Offset: 0x00008330
		public void UpdateComponentFlags(Component c)
		{
			FieldInfo[] fields = c.GetType().GetFields();
			foreach (FieldInfo pi in fields)
			{
				if (!this.componentFields.Exists((VolumeEffectFieldFlags s) => s.fieldName == pi.Name) && VolumeEffectField.IsValidType(pi.FieldType.FullName))
				{
					this.componentFields.Add(new VolumeEffectFieldFlags(pi));
				}
			}
		}

		// Token: 0x06000184 RID: 388 RVA: 0x0000A1BC File Offset: 0x000083BC
		public string[] GetFieldNames()
		{
			return (from r in this.componentFields
			where r.blendFlag
			select r.fieldName).ToArray<string>();
		}

		// Token: 0x040001AC RID: 428
		public string componentName;

		// Token: 0x040001AD RID: 429
		public List<VolumeEffectFieldFlags> componentFields;

		// Token: 0x040001AE RID: 430
		public bool blendFlag;
	}
}
