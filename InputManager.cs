using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200012A RID: 298
public class InputManager : MonoBehaviour
{
	// Token: 0x06000813 RID: 2067 RVA: 0x000504AC File Offset: 0x0004E6AC
	public void InitialiseInputManager()
	{
		InputManager.globalInputManager = this;
		this.buttonKeys = new Dictionary<string, List<KeyCode>>();
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x000504C0 File Offset: 0x0004E6C0
	public bool GetButtonDown(string buttonName, bool heldDown = false)
	{
		if (!this.buttonKeys.ContainsKey(buttonName))
		{
			Debug.LogError("InputManager:: GetButtonDown -- no button named: " + buttonName);
			return false;
		}
		List<KeyCode> modifierKeys = this.GetModifierKeys();
		if (modifierKeys.Count > 0 && this.buttonKeys[buttonName].Count == 1)
		{
			return false;
		}
		int num = 0;
		foreach (KeyCode item in modifierKeys)
		{
			if (this.buttonKeys[buttonName].Contains(item))
			{
				num++;
			}
		}
		if (!heldDown)
		{
			if (Input.GetKeyDown(this.buttonKeys[buttonName].Last<KeyCode>()))
			{
				num++;
			}
		}
		else if (Input.GetKey(this.buttonKeys[buttonName].Last<KeyCode>()))
		{
			num++;
		}
		return num == this.buttonKeys[buttonName].Count;
	}

	// Token: 0x06000815 RID: 2069 RVA: 0x000505EC File Offset: 0x0004E7EC
	public List<KeyCode> GetModifierKeys()
	{
		List<KeyCode> list = new List<KeyCode>();
		if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
		{
			list.Add(KeyCode.LeftControl);
			list.Add(KeyCode.RightControl);
		}
		else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
		{
			list.Add(KeyCode.LeftShift);
			list.Add(KeyCode.RightShift);
		}
		else if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
		{
			list.Add(KeyCode.LeftAlt);
			list.Add(KeyCode.RightAlt);
		}
		return list;
	}

	// Token: 0x06000816 RID: 2070 RVA: 0x000506A8 File Offset: 0x0004E8A8
	public string[] GetButtonNames()
	{
		return this.buttonKeys.Keys.ToArray<string>();
	}

	// Token: 0x06000817 RID: 2071 RVA: 0x000506BC File Offset: 0x0004E8BC
	public string GetKeyNameForButton(string buttonName)
	{
		if (!this.buttonKeys.ContainsKey(buttonName))
		{
			return "N/A";
		}
		if (this.buttonKeys[buttonName].Count > 1)
		{
			string str = string.Empty;
			if (this.buttonKeys[buttonName].First<KeyCode>() == KeyCode.LeftControl || this.buttonKeys[buttonName].First<KeyCode>() == KeyCode.RightControl)
			{
				str = "CONTROL ";
			}
			else if (this.buttonKeys[buttonName].First<KeyCode>() == KeyCode.LeftShift || this.buttonKeys[buttonName].First<KeyCode>() == KeyCode.RightShift)
			{
				str = "SHIFT ";
			}
			if (this.buttonKeys[buttonName].First<KeyCode>() == KeyCode.LeftAlt || this.buttonKeys[buttonName].First<KeyCode>() == KeyCode.RightAlt)
			{
				str = "ALT ";
			}
			return str + " " + this.buttonKeys[buttonName].Last<KeyCode>().ToString();
		}
		return this.buttonKeys[buttonName].Last<KeyCode>().ToString();
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x000507F8 File Offset: 0x0004E9F8
	public void SetButtonForKey(string buttonName, KeyCode keycode, KeyCode modifier)
	{
		this.buttonKeys[buttonName] = new List<KeyCode>();
		if (modifier != KeyCode.None)
		{
			this.buttonKeys[buttonName].Add(modifier);
		}
		this.buttonKeys[buttonName].Add(keycode);
	}

	// Token: 0x06000819 RID: 2073 RVA: 0x00050840 File Offset: 0x0004EA40
	public string DumpButtonKeysDictionary()
	{
		string text = string.Empty;
		foreach (KeyValuePair<string, List<KeyCode>> keyValuePair in this.buttonKeys)
		{
			text = text + keyValuePair.Key + "=";
			if (keyValuePair.Value.Count > 1)
			{
				text = text + keyValuePair.Value.First<KeyCode>().ToString() + "+";
			}
			text = text + keyValuePair.Value.Last<KeyCode>().ToString() + "\n";
		}
		return text;
	}

	// Token: 0x0600081A RID: 2074 RVA: 0x00050910 File Offset: 0x0004EB10
	public void BuildButtonKeysDictionary(string keybindList)
	{
		foreach (KeyValuePair<string, List<KeyCode>> keyValuePair in this.buttonKeys)
		{
			if (keyValuePair.Value.Count == 1)
			{
			}
		}
		string[] array = keybindList.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Contains("="))
			{
				string[] array2 = array[i].Split(new char[]
				{
					'='
				});
				string[] array3 = array2[1].Split(new char[]
				{
					'+'
				});
				this.buttonKeys[array2[0].Trim()] = new List<KeyCode>();
				if (array3.Length > 1)
				{
					this.buttonKeys[array2[0].Trim()].Add((KeyCode)((int)Enum.Parse(typeof(KeyCode), array3[0].Trim())));
					this.buttonKeys[array2[0].Trim()].Add((KeyCode)((int)Enum.Parse(typeof(KeyCode), array3[1].Trim())));
				}
				else
				{
					this.buttonKeys[array2[0].Trim()].Add((KeyCode)((int)Enum.Parse(typeof(KeyCode), array3[0].Trim())));
				}
			}
		}
	}

	// Token: 0x04000BC8 RID: 3016
	public static InputManager globalInputManager;

	// Token: 0x04000BC9 RID: 3017
	public bool allClear;

	// Token: 0x04000BCA RID: 3018
	public Dictionary<string, List<KeyCode>> buttonKeys;
}
