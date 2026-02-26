using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto.Common.Unity.Utility
{
	// Token: 0x0200004E RID: 78
	public static class ExtendedFind
	{
		// Token: 0x0600028E RID: 654 RVA: 0x0000EA20 File Offset: 0x0000CC20
		public static T GetInterface<T>(GameObject obj) where T : class
		{
			Component[] components = obj.GetComponents<Component>();
			foreach (Component component in components)
			{
				if (component is T)
				{
					return component as T;
				}
			}
			return (T)((object)null);
		}

		// Token: 0x0600028F RID: 655 RVA: 0x0000EA70 File Offset: 0x0000CC70
		public static T GetInterfaceInChildren<T>(GameObject obj) where T : class
		{
			Component[] componentsInChildren = obj.GetComponentsInChildren<Component>();
			foreach (Component component in componentsInChildren)
			{
				if (component is T)
				{
					return component as T;
				}
			}
			return (T)((object)null);
		}

		// Token: 0x06000290 RID: 656 RVA: 0x0000EAC0 File Offset: 0x0000CCC0
		public static T GetInterfaceImmediateChildren<T>(GameObject obj) where T : class
		{
			foreach (object obj2 in obj.transform)
			{
				Transform transform = (Transform)obj2;
				Component[] components = transform.GetComponents<Component>();
				foreach (Component component in components)
				{
					if (component is T)
					{
						return component as T;
					}
				}
			}
			return (T)((object)null);
		}

		// Token: 0x06000291 RID: 657 RVA: 0x0000EB80 File Offset: 0x0000CD80
		public static T[] GetInterfaces<T>(GameObject obj) where T : class
		{
			Component[] components = obj.GetComponents<Component>();
			List<T> list = new List<T>();
			foreach (Component component in components)
			{
				if (component is T)
				{
					list.Add(component as T);
				}
			}
			return list.ToArray();
		}

		// Token: 0x06000292 RID: 658 RVA: 0x0000EBE0 File Offset: 0x0000CDE0
		public static T[] GetInterfacesInChildren<T>(GameObject obj) where T : class
		{
			Component[] componentsInChildren = obj.GetComponentsInChildren<Component>();
			List<T> list = new List<T>();
			foreach (Component component in componentsInChildren)
			{
				if (component is T)
				{
					list.Add(component as T);
				}
			}
			return list.ToArray();
		}

		// Token: 0x06000293 RID: 659 RVA: 0x0000EC40 File Offset: 0x0000CE40
		public static T[] GetInterfacesImmediateChildren<T>(GameObject obj) where T : class
		{
			List<T> list = new List<T>();
			foreach (object obj2 in obj.transform)
			{
				Transform transform = (Transform)obj2;
				Component[] components = transform.GetComponents<Component>();
				foreach (Component component in components)
				{
					if (component is T)
					{
						list.Add(component as T);
					}
				}
			}
			return list.ToArray();
		}

		// Token: 0x06000294 RID: 660 RVA: 0x0000ED04 File Offset: 0x0000CF04
		public static T GetComponetInImmediateParent<T>(GameObject obj) where T : Component
		{
			if (obj.transform.parent == null)
			{
				return (T)((object)null);
			}
			return obj.transform.parent.GetComponent<T>();
		}

		// Token: 0x06000295 RID: 661 RVA: 0x0000ED40 File Offset: 0x0000CF40
		public static T[] GetComponentsInImmediateParent<T>(GameObject obj) where T : Component
		{
			if (obj.transform.parent == null)
			{
				return new T[0];
			}
			return obj.transform.parent.GetComponents<T>();
		}

		// Token: 0x06000296 RID: 662 RVA: 0x0000ED7C File Offset: 0x0000CF7C
		public static T GetComponetInImmediateChildren<T>(GameObject obj) where T : Component
		{
			foreach (object obj2 in obj.transform)
			{
				Transform transform = (Transform)obj2;
				T component = transform.GetComponent<T>();
				if (component != null)
				{
					return component;
				}
			}
			return (T)((object)null);
		}

		// Token: 0x06000297 RID: 663 RVA: 0x0000EE10 File Offset: 0x0000D010
		public static T[] GetComponetsInImmediateChildren<T>(GameObject obj) where T : Component
		{
			List<T> list = new List<T>();
			foreach (object obj2 in obj.transform)
			{
				Transform transform = (Transform)obj2;
				T[] components = transform.GetComponents<T>();
				foreach (T item in components)
				{
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		// Token: 0x06000298 RID: 664 RVA: 0x0000EEC0 File Offset: 0x0000D0C0
		public static T FindComponentOnGameObject<T>(string name) where T : Component
		{
			GameObject gameObject = GameObject.Find(name);
			if (gameObject == null)
			{
				return (T)((object)null);
			}
			return gameObject.GetComponent<T>();
		}

		// Token: 0x06000299 RID: 665 RVA: 0x0000EEF0 File Offset: 0x0000D0F0
		public static T[] FindComponentsOnGameObject<T>(string name) where T : Component
		{
			GameObject gameObject = GameObject.Find(name);
			if (gameObject == null)
			{
				return new T[0];
			}
			return gameObject.GetComponents<T>();
		}

		// Token: 0x0600029A RID: 666 RVA: 0x0000EF20 File Offset: 0x0000D120
		public static T FindInterfaceOnGameObject<T>(string name) where T : class
		{
			GameObject gameObject = GameObject.Find(name);
			if (gameObject == null)
			{
				return (T)((object)null);
			}
			return ExtendedFind.GetInterface<T>(gameObject);
		}

		// Token: 0x0600029B RID: 667 RVA: 0x0000EF50 File Offset: 0x0000D150
		public static T[] FindInterfacesOnGameObject<T>(string name) where T : class
		{
			GameObject gameObject = GameObject.Find(name);
			if (gameObject == null)
			{
				return new T[0];
			}
			return ExtendedFind.GetInterfaces<T>(gameObject);
		}
	}
}
