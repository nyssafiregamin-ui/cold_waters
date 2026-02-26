using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200012B RID: 299
public class KeybindDialogBox : MonoBehaviour
{
	// Token: 0x0600081C RID: 2076 RVA: 0x00050ABC File Offset: 0x0004ECBC
	private void Start()
	{
		this.inputManager = UnityEngine.Object.FindObjectOfType<InputManager>();
		string[] buttonNames = this.inputManager.GetButtonNames();
		this.buttonToLabel = new Dictionary<string, Text>();
		this.buttonsList = new List<GameObject>();
		for (int i = 0; i < buttonNames.Length; i++)
		{
			string bn = buttonNames[i];
			GameObject gameObject = UnityEngine.Object.Instantiate(this.keyItemPrefab, this.keyList.transform) as GameObject;
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, 0f);
			this.buttonsList.Add(gameObject);
			Text component = gameObject.transform.Find("Button Name").GetComponent<Text>();
			component.text = LanguageManager.GetDictionaryString(LanguageManager.interfaceDictionary, bn);
			Text component2 = gameObject.transform.Find("Button/Key Name").GetComponent<Text>();
			component2.text = this.inputManager.GetKeyNameForButton(bn);
			this.buttonToLabel[bn] = component2;
			Button keyBindButton = gameObject.transform.Find("Button").GetComponent<Button>();
			keyBindButton.onClick.AddListener(delegate()
			{
				this.StartRebindFor(bn, keyBindButton);
			});
		}
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x00050C44 File Offset: 0x0004EE44
	public void RebuildButtonList()
	{
		UIFunctions.globaluifunctions.optionsmanager.BuildDefaultButtonsKeybindList();
		foreach (GameObject gameObject in this.buttonsList)
		{
			Button component = gameObject.transform.Find("Button").GetComponent<Button>();
			component.onClick.RemoveAllListeners();
			UnityEngine.Object.Destroy(gameObject);
		}
		this.Start();
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x00050CE0 File Offset: 0x0004EEE0
	private void Update()
	{
		if (this.buttonToRebind != null && Input.anyKeyDown)
		{
			List<KeyCode> modifierKeys = this.inputManager.GetModifierKeys();
			foreach (KeyCode key in modifierKeys)
			{
				if (Input.GetKeyDown(key))
				{
					return;
				}
			}
			foreach (object obj in Enum.GetValues(typeof(KeyCode)))
			{
				KeyCode keyCode = (KeyCode)((int)obj);
				if (Input.GetKeyDown(keyCode))
				{
					KeyCode modifier = KeyCode.None;
					if (modifierKeys.Count > 0)
					{
						if (modifierKeys.Contains(KeyCode.LeftControl))
						{
							modifier = KeyCode.LeftControl;
						}
						else if (modifierKeys.Contains(KeyCode.LeftShift))
						{
							modifier = KeyCode.LeftShift;
						}
						else if (modifierKeys.Contains(KeyCode.LeftAlt))
						{
							modifier = KeyCode.LeftAlt;
						}
					}
					this.inputManager.SetButtonForKey(this.buttonToRebind, keyCode, modifier);
					this.buttonToLabel[this.buttonToRebind].text = this.inputManager.GetKeyNameForButton(this.buttonToRebind);
					this.buttonObjectToRebind.interactable = true;
					this.buttonToRebind = null;
					this.buttonObjectToRebind = null;
					break;
				}
			}
		}
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x00050E98 File Offset: 0x0004F098
	private void StartRebindFor(string buttonName, Button button)
	{
		this.buttonToRebind = buttonName;
		button.interactable = false;
		this.buttonObjectToRebind = button;
	}

	// Token: 0x04000BCB RID: 3019
	private InputManager inputManager;

	// Token: 0x04000BCC RID: 3020
	public GameObject keyItemPrefab;

	// Token: 0x04000BCD RID: 3021
	public GameObject keyList;

	// Token: 0x04000BCE RID: 3022
	private string buttonToRebind;

	// Token: 0x04000BCF RID: 3023
	private Button buttonObjectToRebind;

	// Token: 0x04000BD0 RID: 3024
	private Dictionary<string, Text> buttonToLabel;

	// Token: 0x04000BD1 RID: 3025
	private List<GameObject> buttonsList;
}
