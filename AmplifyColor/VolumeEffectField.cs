using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AmplifyColor
{
	// Token: 0x0200002B RID: 43
	[Serializable]
	public class VolumeEffectField
	{
		// Token: 0x0600015B RID: 347 RVA: 0x00008D84 File Offset: 0x00006F84
		public VolumeEffectField(string fieldName, string fieldType)
		{
			this.fieldName = fieldName;
			this.fieldType = fieldType;
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00008D9C File Offset: 0x00006F9C
		public VolumeEffectField(FieldInfo pi, Component c) : this(pi.Name, pi.FieldType.FullName)
		{
			object value = pi.GetValue(c);
			this.UpdateValue(value);
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00008DD0 File Offset: 0x00006FD0
		public static bool IsValidType(string type)
		{
			if (type != null)
			{
				if (VolumeEffectField.<>f__switch$map0 == null)
				{
					VolumeEffectField.<>f__switch$map0 = new Dictionary<string, int>(6)
					{
						{
							"System.Single",
							0
						},
						{
							"System.Boolean",
							0
						},
						{
							"UnityEngine.Color",
							0
						},
						{
							"UnityEngine.Vector2",
							0
						},
						{
							"UnityEngine.Vector3",
							0
						},
						{
							"UnityEngine.Vector4",
							0
						}
					};
				}
				int num;
				if (VolumeEffectField.<>f__switch$map0.TryGetValue(type, out num))
				{
					if (num == 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600015E RID: 350 RVA: 0x00008E64 File Offset: 0x00007064
		public void UpdateValue(object val)
		{
			string text = this.fieldType;
			switch (text)
			{
			case "System.Single":
				this.valueSingle = (float)val;
				break;
			case "System.Boolean":
				this.valueBoolean = (bool)val;
				break;
			case "UnityEngine.Color":
				this.valueColor = (Color)val;
				break;
			case "UnityEngine.Vector2":
				this.valueVector2 = (Vector2)val;
				break;
			case "UnityEngine.Vector3":
				this.valueVector3 = (Vector3)val;
				break;
			case "UnityEngine.Vector4":
				this.valueVector4 = (Vector4)val;
				break;
			}
		}

		// Token: 0x04000192 RID: 402
		public string fieldName;

		// Token: 0x04000193 RID: 403
		public string fieldType;

		// Token: 0x04000194 RID: 404
		public float valueSingle;

		// Token: 0x04000195 RID: 405
		public Color valueColor;

		// Token: 0x04000196 RID: 406
		public bool valueBoolean;

		// Token: 0x04000197 RID: 407
		public Vector2 valueVector2;

		// Token: 0x04000198 RID: 408
		public Vector3 valueVector3;

		// Token: 0x04000199 RID: 409
		public Vector4 valueVector4;
	}
}
