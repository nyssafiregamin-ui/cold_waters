using System;
using UnityEngine;

// Token: 0x0200015C RID: 348
public class SortArray : MonoBehaviour
{
	// Token: 0x06000A1C RID: 2588 RVA: 0x0007B4BC File Offset: 0x000796BC
	public string[] SortInputArray(string[] input)
	{
		Array.Sort<string>(input);
		return input;
	}
}
