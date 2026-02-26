using System;
using System.Reflection;

namespace AmplifyColor
{
	// Token: 0x0200002F RID: 47
	[Serializable]
	public class VolumeEffectFieldFlags
	{
		// Token: 0x0600017D RID: 381 RVA: 0x00009F04 File Offset: 0x00008104
		public VolumeEffectFieldFlags(FieldInfo pi)
		{
			this.fieldName = pi.Name;
			this.fieldType = pi.FieldType.FullName;
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00009F34 File Offset: 0x00008134
		public VolumeEffectFieldFlags(VolumeEffectField field)
		{
			this.fieldName = field.fieldName;
			this.fieldType = field.fieldType;
			this.blendFlag = true;
		}

		// Token: 0x040001A9 RID: 425
		public string fieldName;

		// Token: 0x040001AA RID: 426
		public string fieldType;

		// Token: 0x040001AB RID: 427
		public bool blendFlag;
	}
}
