using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace PreviewLabs
{
	// Token: 0x02000143 RID: 323
	public static class PlayerPrefs
	{
		// Token: 0x06000954 RID: 2388 RVA: 0x0006CDB0 File Offset: 0x0006AFB0
		static PlayerPrefs()
		{
			if (File.Exists(PlayerPrefs.fileName))
			{
				StreamReader streamReader = new StreamReader(PlayerPrefs.fileName);
				PlayerPrefs.serializedInput = streamReader.ReadLine();
				PlayerPrefs.Deserialize();
				streamReader.Close();
			}
		}

		// Token: 0x06000955 RID: 2389 RVA: 0x0006CE28 File Offset: 0x0006B028
		public static bool HasKey(string key)
		{
			return PlayerPrefs.playerPrefsHashtable.ContainsKey(key);
		}

		// Token: 0x06000956 RID: 2390 RVA: 0x0006CE38 File Offset: 0x0006B038
		public static void SetString(string key, string value)
		{
			if (!PlayerPrefs.playerPrefsHashtable.ContainsKey(key))
			{
				PlayerPrefs.playerPrefsHashtable.Add(key, value);
			}
			else
			{
				PlayerPrefs.playerPrefsHashtable[key] = value;
			}
			PlayerPrefs.hashTableChanged = true;
		}

		// Token: 0x06000957 RID: 2391 RVA: 0x0006CE70 File Offset: 0x0006B070
		public static void SetInt(string key, int value)
		{
			if (!PlayerPrefs.playerPrefsHashtable.ContainsKey(key))
			{
				PlayerPrefs.playerPrefsHashtable.Add(key, value);
			}
			else
			{
				PlayerPrefs.playerPrefsHashtable[key] = value;
			}
			PlayerPrefs.hashTableChanged = true;
		}

		// Token: 0x06000958 RID: 2392 RVA: 0x0006CEB0 File Offset: 0x0006B0B0
		public static void SetFloat(string key, float value)
		{
			if (!PlayerPrefs.playerPrefsHashtable.ContainsKey(key))
			{
				PlayerPrefs.playerPrefsHashtable.Add(key, value);
			}
			else
			{
				PlayerPrefs.playerPrefsHashtable[key] = value;
			}
			PlayerPrefs.hashTableChanged = true;
		}

		// Token: 0x06000959 RID: 2393 RVA: 0x0006CEF0 File Offset: 0x0006B0F0
		public static void SetBool(string key, bool value)
		{
			if (!PlayerPrefs.playerPrefsHashtable.ContainsKey(key))
			{
				PlayerPrefs.playerPrefsHashtable.Add(key, value);
			}
			else
			{
				PlayerPrefs.playerPrefsHashtable[key] = value;
			}
			PlayerPrefs.hashTableChanged = true;
		}

		// Token: 0x0600095A RID: 2394 RVA: 0x0006CF30 File Offset: 0x0006B130
		public static string GetString(string key)
		{
			if (PlayerPrefs.playerPrefsHashtable.ContainsKey(key))
			{
				return PlayerPrefs.playerPrefsHashtable[key].ToString();
			}
			return null;
		}

		// Token: 0x0600095B RID: 2395 RVA: 0x0006CF60 File Offset: 0x0006B160
		public static string GetString(string key, string defaultValue)
		{
			if (PlayerPrefs.playerPrefsHashtable.ContainsKey(key))
			{
				return PlayerPrefs.playerPrefsHashtable[key].ToString();
			}
			PlayerPrefs.playerPrefsHashtable.Add(key, defaultValue);
			PlayerPrefs.hashTableChanged = true;
			return defaultValue;
		}

		// Token: 0x0600095C RID: 2396 RVA: 0x0006CFA4 File Offset: 0x0006B1A4
		public static int GetInt(string key)
		{
			if (PlayerPrefs.playerPrefsHashtable.ContainsKey(key))
			{
				return (int)PlayerPrefs.playerPrefsHashtable[key];
			}
			return 0;
		}

		// Token: 0x0600095D RID: 2397 RVA: 0x0006CFD4 File Offset: 0x0006B1D4
		public static int GetInt(string key, int defaultValue)
		{
			if (PlayerPrefs.playerPrefsHashtable.ContainsKey(key))
			{
				return (int)PlayerPrefs.playerPrefsHashtable[key];
			}
			PlayerPrefs.playerPrefsHashtable.Add(key, defaultValue);
			PlayerPrefs.hashTableChanged = true;
			return defaultValue;
		}

		// Token: 0x0600095E RID: 2398 RVA: 0x0006D01C File Offset: 0x0006B21C
		public static float GetFloat(string key)
		{
			if (PlayerPrefs.playerPrefsHashtable.ContainsKey(key))
			{
				return (float)PlayerPrefs.playerPrefsHashtable[key];
			}
			return 0f;
		}

		// Token: 0x0600095F RID: 2399 RVA: 0x0006D050 File Offset: 0x0006B250
		public static float GetFloat(string key, float defaultValue)
		{
			if (PlayerPrefs.playerPrefsHashtable.ContainsKey(key))
			{
				return (float)PlayerPrefs.playerPrefsHashtable[key];
			}
			PlayerPrefs.playerPrefsHashtable.Add(key, defaultValue);
			PlayerPrefs.hashTableChanged = true;
			return defaultValue;
		}

		// Token: 0x06000960 RID: 2400 RVA: 0x0006D098 File Offset: 0x0006B298
		public static bool GetBool(string key)
		{
			return PlayerPrefs.playerPrefsHashtable.ContainsKey(key) && (bool)PlayerPrefs.playerPrefsHashtable[key];
		}

		// Token: 0x06000961 RID: 2401 RVA: 0x0006D0C8 File Offset: 0x0006B2C8
		public static bool GetBool(string key, bool defaultValue)
		{
			if (PlayerPrefs.playerPrefsHashtable.ContainsKey(key))
			{
				return (bool)PlayerPrefs.playerPrefsHashtable[key];
			}
			PlayerPrefs.playerPrefsHashtable.Add(key, defaultValue);
			PlayerPrefs.hashTableChanged = true;
			return defaultValue;
		}

		// Token: 0x06000962 RID: 2402 RVA: 0x0006D110 File Offset: 0x0006B310
		public static void DeleteKey(string key)
		{
			PlayerPrefs.playerPrefsHashtable.Remove(key);
		}

		// Token: 0x06000963 RID: 2403 RVA: 0x0006D120 File Offset: 0x0006B320
		public static void DeleteAll()
		{
			PlayerPrefs.playerPrefsHashtable.Clear();
		}

		// Token: 0x06000964 RID: 2404 RVA: 0x0006D12C File Offset: 0x0006B32C
		public static void Flush()
		{
			if (PlayerPrefs.hashTableChanged)
			{
				PlayerPrefs.Serialize();
				StreamWriter streamWriter = File.CreateText(PlayerPrefs.fileName);
				if (streamWriter == null)
				{
					Debug.LogWarning("PlayerPrefs::Flush() opening file for writing failed: " + PlayerPrefs.fileName);
				}
				streamWriter.WriteLine(PlayerPrefs.serializedOutput);
				streamWriter.Close();
				PlayerPrefs.serializedOutput = string.Empty;
			}
		}

		// Token: 0x06000965 RID: 2405 RVA: 0x0006D18C File Offset: 0x0006B38C
		private static void Serialize()
		{
			IDictionaryEnumerator enumerator = PlayerPrefs.playerPrefsHashtable.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (PlayerPrefs.serializedOutput != string.Empty)
				{
					PlayerPrefs.serializedOutput += " ; ";
				}
				string text = PlayerPrefs.serializedOutput;
				PlayerPrefs.serializedOutput = string.Concat(new object[]
				{
					text,
					PlayerPrefs.EscapeNonSeperators(enumerator.Key.ToString()),
					" : ",
					PlayerPrefs.EscapeNonSeperators(enumerator.Value.ToString()),
					" : ",
					enumerator.Value.GetType()
				});
			}
		}

		// Token: 0x06000966 RID: 2406 RVA: 0x0006D23C File Offset: 0x0006B43C
		private static void Deserialize()
		{
			string[] array = PlayerPrefs.serializedInput.Split(new string[]
			{
				" ; "
			}, StringSplitOptions.None);
			foreach (string text in array)
			{
				string[] array3 = text.Split(new string[]
				{
					" : "
				}, StringSplitOptions.None);
				PlayerPrefs.playerPrefsHashtable.Add(PlayerPrefs.DeEscapeNonSeperators(array3[0]), PlayerPrefs.GetTypeValue(array3[2], PlayerPrefs.DeEscapeNonSeperators(array3[1])));
				if (array3.Length > 3)
				{
					Debug.LogWarning("PlayerPrefs::Deserialize() parameterContent has " + array3.Length + " elements");
				}
			}
		}

		// Token: 0x06000967 RID: 2407 RVA: 0x0006D2E0 File Offset: 0x0006B4E0
		private static string EscapeNonSeperators(string inputToEscape)
		{
			inputToEscape = inputToEscape.Replace(":", "\\:");
			inputToEscape = inputToEscape.Replace(";", "\\;");
			return inputToEscape;
		}

		// Token: 0x06000968 RID: 2408 RVA: 0x0006D308 File Offset: 0x0006B508
		private static string DeEscapeNonSeperators(string inputToDeEscape)
		{
			inputToDeEscape = inputToDeEscape.Replace("\\:", ":");
			inputToDeEscape = inputToDeEscape.Replace("\\;", ";");
			return inputToDeEscape;
		}

		// Token: 0x06000969 RID: 2409 RVA: 0x0006D330 File Offset: 0x0006B530
		public static object GetTypeValue(string typeName, string value)
		{
			if (typeName == "System.String")
			{
				return value.ToString();
			}
			if (typeName == "System.Int32")
			{
				return Convert.ToInt32(value);
			}
			if (typeName == "System.Boolean")
			{
				return Convert.ToBoolean(value);
			}
			if (typeName == "System.Single")
			{
				return Convert.ToSingle(value);
			}
			Debug.LogError("Unsupported type: " + typeName);
			return null;
		}

		// Token: 0x04000E4D RID: 3661
		private const string PARAMETERS_SEPERATOR = ";";

		// Token: 0x04000E4E RID: 3662
		private const string KEY_VALUE_SEPERATOR = ":";

		// Token: 0x04000E4F RID: 3663
		private static Hashtable playerPrefsHashtable = new Hashtable();

		// Token: 0x04000E50 RID: 3664
		private static bool hashTableChanged = false;

		// Token: 0x04000E51 RID: 3665
		private static string serializedOutput = string.Empty;

		// Token: 0x04000E52 RID: 3666
		private static string serializedInput = string.Empty;

		// Token: 0x04000E53 RID: 3667
		private static readonly string fileName = Application.persistentDataPath + "/PlayerPrefs.txt";
	}
}
