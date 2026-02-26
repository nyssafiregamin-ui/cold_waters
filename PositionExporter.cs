using System;
using System.IO;
using UnityEngine;

// Token: 0x02000147 RID: 327
public class PositionExporter : MonoBehaviour
{
	// Token: 0x06000978 RID: 2424 RVA: 0x0006EB2C File Offset: 0x0006CD2C
	public void ExportPrefabData()
	{
		MapGenerator mapGenerator = UnityEngine.Object.FindObjectOfType<MapGenerator>();
		Vector2 demMapCoods = this.worldCoordinatesOverride;
		if (mapGenerator != null)
		{
			demMapCoods = mapGenerator.demMapCoods;
		}
		string text = "//" + this.comment + "\n";
		string text2 = text;
		text = string.Concat(new string[]
		{
			text2,
			"WorldObject=",
			this.prefabPath,
			base.name,
			"\n"
		});
		text = text + this.CalculateWorldObjectCoords(demMapCoods) + "\n";
		text2 = text;
		text = string.Concat(new string[]
		{
			text2,
			"MeshRotation=",
			this.GetFloatValueAsString(base.transform.eulerAngles.x),
			",",
			this.GetFloatValueAsString(base.transform.eulerAngles.y),
			",",
			this.GetFloatValueAsString(base.transform.eulerAngles.z),
			"\n"
		});
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			text += "\n";
			text2 = text;
			text = string.Concat(new string[]
			{
				text2,
				"ChildObject=",
				this.prefabPath,
				transform.name,
				"\n"
			});
			text2 = text;
			text = string.Concat(new string[]
			{
				text2,
				"ChildMeshPosition=",
				this.GetFloatValueAsString(transform.transform.localPosition.x),
				",",
				this.GetFloatValueAsString(transform.transform.localPosition.y),
				",",
				this.GetFloatValueAsString(transform.transform.localPosition.z),
				"\n"
			});
			text2 = text;
			text = string.Concat(new string[]
			{
				text2,
				"ChildMeshRotation=",
				this.GetFloatValueAsString(transform.transform.localEulerAngles.x),
				",",
				this.GetFloatValueAsString(transform.transform.localEulerAngles.y),
				",",
				this.GetFloatValueAsString(transform.transform.localRotation.z),
				"\n"
			});
		}
		Debug.Log(text);
		string path = "Assets/StreamingAssets/default/campaign/maps/" + this.filename + ".txt";
		StreamWriter streamWriter = new StreamWriter(path, true);
		streamWriter.Write(text);
		streamWriter.Close();
	}

	// Token: 0x06000979 RID: 2425 RVA: 0x0006EE34 File Offset: 0x0006D034
	private string CalculateWorldObjectCoords(Vector2 coords)
	{
		int num = (int)(base.transform.position.x / 20f);
		num = (int)coords.x + num;
		int num2 = (int)(base.transform.position.z / 20f);
		num2 = (int)coords.y + num2;
		string text = string.Concat(new string[]
		{
			"WorldObjectCoords=",
			num.ToString(),
			",",
			num2.ToString(),
			"\n"
		});
		float value = base.transform.position.x % 20f;
		float value2 = base.transform.position.z % 20f;
		string text2 = text;
		return string.Concat(new string[]
		{
			text2,
			"MeshPosition=",
			this.GetFloatValueAsString(value),
			",0,",
			this.GetFloatValueAsString(value2)
		});
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x0006EF38 File Offset: 0x0006D138
	private string GetFloatValueAsString(float value)
	{
		if (value.ToString().Contains("."))
		{
			return string.Format("{0:0.000}", value);
		}
		return string.Format("{0:0}", value);
	}

	// Token: 0x04000E6F RID: 3695
	public string prefabPath = "terrain/scenery/";

	// Token: 0x04000E70 RID: 3696
	public string filename = "PrefabPositions";

	// Token: 0x04000E71 RID: 3697
	public string comment;

	// Token: 0x04000E72 RID: 3698
	public Vector2 worldCoordinatesOverride = Vector2.zero;
}
