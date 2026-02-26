using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AmplifyColor
{
	// Token: 0x0200002C RID: 44
	[Serializable]
	public class VolumeEffectComponent
	{
		// Token: 0x0600015F RID: 351 RVA: 0x00008F78 File Offset: 0x00007178
		public VolumeEffectComponent(string name)
		{
			this.componentName = name;
			this.fields = new List<VolumeEffectField>();
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00008F94 File Offset: 0x00007194
		public VolumeEffectComponent(Component c, VolumeEffectComponentFlags compFlags) : this(compFlags.componentName)
		{
			foreach (VolumeEffectFieldFlags volumeEffectFieldFlags in compFlags.componentFields)
			{
				if (volumeEffectFieldFlags.blendFlag)
				{
					FieldInfo field = c.GetType().GetField(volumeEffectFieldFlags.fieldName);
					VolumeEffectField volumeEffectField = (!VolumeEffectField.IsValidType(field.FieldType.FullName)) ? null : new VolumeEffectField(field, c);
					if (volumeEffectField != null)
					{
						this.fields.Add(volumeEffectField);
					}
				}
			}
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00009058 File Offset: 0x00007258
		public VolumeEffectField AddField(FieldInfo pi, Component c)
		{
			return this.AddField(pi, c, -1);
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00009064 File Offset: 0x00007264
		public VolumeEffectField AddField(FieldInfo pi, Component c, int position)
		{
			VolumeEffectField volumeEffectField = (!VolumeEffectField.IsValidType(pi.FieldType.FullName)) ? null : new VolumeEffectField(pi, c);
			if (volumeEffectField != null)
			{
				if (position < 0 || position >= this.fields.Count)
				{
					this.fields.Add(volumeEffectField);
				}
				else
				{
					this.fields.Insert(position, volumeEffectField);
				}
			}
			return volumeEffectField;
		}

		// Token: 0x06000163 RID: 355 RVA: 0x000090D4 File Offset: 0x000072D4
		public void RemoveEffectField(VolumeEffectField field)
		{
			this.fields.Remove(field);
		}

		// Token: 0x06000164 RID: 356 RVA: 0x000090E4 File Offset: 0x000072E4
		public void UpdateComponent(Component c, VolumeEffectComponentFlags compFlags)
		{
			VolumeEffectFieldFlags fieldFlags;
			foreach (VolumeEffectFieldFlags fieldFlags2 in compFlags.componentFields)
			{
				fieldFlags = fieldFlags2;
				if (fieldFlags.blendFlag)
				{
					if (!this.fields.Exists((VolumeEffectField s) => s.fieldName == fieldFlags.fieldName))
					{
						FieldInfo field = c.GetType().GetField(fieldFlags.fieldName);
						VolumeEffectField volumeEffectField = (!VolumeEffectField.IsValidType(field.FieldType.FullName)) ? null : new VolumeEffectField(field, c);
						if (volumeEffectField != null)
						{
							this.fields.Add(volumeEffectField);
						}
					}
				}
			}
		}

		// Token: 0x06000165 RID: 357 RVA: 0x000091CC File Offset: 0x000073CC
		public VolumeEffectField FindEffectField(string fieldName)
		{
			for (int i = 0; i < this.fields.Count; i++)
			{
				if (this.fields[i].fieldName == fieldName)
				{
					return this.fields[i];
				}
			}
			return null;
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00009220 File Offset: 0x00007420
		public static FieldInfo[] ListAcceptableFields(Component c)
		{
			if (c == null)
			{
				return new FieldInfo[0];
			}
			FieldInfo[] source = c.GetType().GetFields();
			return (from f in source
			where VolumeEffectField.IsValidType(f.FieldType.FullName)
			select f).ToArray<FieldInfo>();
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00009274 File Offset: 0x00007474
		public string[] GetFieldNames()
		{
			return (from r in this.fields
			select r.fieldName).ToArray<string>();
		}

		// Token: 0x0400019C RID: 412
		public string componentName;

		// Token: 0x0400019D RID: 413
		public List<VolumeEffectField> fields;
	}
}
