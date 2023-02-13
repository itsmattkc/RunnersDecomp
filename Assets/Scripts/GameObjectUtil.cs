using System.Collections.Generic;
using UnityEngine;

public class GameObjectUtil
{
	public static List<T> FindChildGameObjectsComponents<T>(GameObject parent, string name) where T : Component
	{
		List<T> list = new List<T>();
		if (parent != null)
		{
			IEnumerable<GameObject> enumerable = FindChildGameObjectsEnumerable(parent, name);
			{
				foreach (GameObject item in enumerable)
				{
					list.Add(item.GetComponent<T>());
				}
				return list;
			}
		}
		return list;
	}

	public static List<GameObject> FindChildGameObjects(GameObject parent, string name)
	{
		List<GameObject> list = new List<GameObject>();
		if (parent != null)
		{
			IEnumerable<GameObject> enumerable = FindChildGameObjectsEnumerable(parent, name);
			{
				foreach (GameObject item in enumerable)
				{
					list.Add(item);
				}
				return list;
			}
		}
		return list;
	}

	private static IEnumerable<GameObject> FindChildGameObjectsEnumerable(GameObject parent, string name)
	{
		for (int i = 0; i < parent.transform.childCount; i++)
		{
			GameObject child = parent.transform.GetChild(i).gameObject;
			if (child.name == name)
			{
				yield return child;
			}
			IEnumerable<GameObject> gos = FindChildGameObjectsEnumerable(child, name);
			foreach (GameObject item in gos)
			{
				yield return item;
			}
		}
	}

	public static GameObject FindChildGameObject(GameObject parent, string name)
	{
		Transform transform = parent.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			GameObject gameObject = transform.GetChild(i).gameObject;
			if (gameObject.name == name)
			{
				return gameObject;
			}
			GameObject gameObject2 = FindChildGameObject(gameObject, name);
			if (gameObject2 != null)
			{
				return gameObject2;
			}
		}
		return null;
	}

	public static T FindChildGameObjectComponent<T>(GameObject parent, string name) where T : Component
	{
		if (parent == null)
		{
			return (T)null;
		}
		GameObject gameObject = FindChildGameObject(parent, name);
		if (gameObject == null)
		{
			return (T)null;
		}
		return gameObject.GetComponent<T>();
	}

	public static T FindGameObjectComponent<T>(string name) where T : Component
	{
		GameObject gameObject = GameObject.Find(name);
		if (gameObject == null)
		{
			return (T)null;
		}
		return gameObject.GetComponent<T>();
	}

	public static T FindGameObjectComponentWithTag<T>(string tagName, string name) where T : Component
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag(tagName);
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (gameObject.name == name)
			{
				return gameObject.GetComponent<T>();
			}
		}
		return (T)null;
	}

	public static GameObject FindGameObjectWithTag(string tagName, string name)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag(tagName);
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (gameObject.name == name)
			{
				return gameObject;
			}
		}
		return null;
	}

	public static GameObject FindParentGameObject(GameObject gameObject, string name)
	{
		while (gameObject != null)
		{
			GameObject gameObject2 = null;
			Transform parent = gameObject.transform.parent;
			if (parent != null)
			{
				gameObject2 = parent.gameObject;
				if (gameObject2 != null && gameObject2.name == name)
				{
					return gameObject2;
				}
			}
			gameObject = gameObject2;
		}
		return null;
	}

	public static GameObject SendMessageFindGameObject(string objectName, string methodName, object value, SendMessageOptions options)
	{
		GameObject gameObject = GameObject.Find(objectName);
		if (gameObject != null)
		{
			gameObject.SendMessage(methodName, value, options);
		}
		return gameObject;
	}

	public static GameObject SendMessageFindGameObjectWithTag(string tagName, string objectName, string methodName, object value, SendMessageOptions options)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag(tagName);
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (gameObject.name == objectName)
			{
				gameObject.SendMessage(methodName, value, options);
				return gameObject;
			}
		}
		return null;
	}

	public static void SendMessageToTagObjects(string tagName, string methodName, object value, SendMessageOptions options)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag(tagName);
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			gameObject.SendMessage(methodName, value, options);
		}
	}

	public static void SendDelayedMessageFindGameObject(string objectName, string methodName, object value)
	{
		if (DelayedMessageManager.Instance != null)
		{
			DelayedMessageManager.Instance.AddDelayedMessage(objectName, methodName, value);
		}
	}

	public static void SendDelayedMessageToGameObject(GameObject gameObject, string methodName, object value)
	{
		if (DelayedMessageManager.Instance != null)
		{
			DelayedMessageManager.Instance.AddDelayedMessage(gameObject, methodName, value);
		}
	}

	public static void SendDelayedMessageToTagObjects(string tagName, string methodName, object value)
	{
		if (DelayedMessageManager.Instance != null)
		{
			DelayedMessageManager.Instance.AddDelayedMessageToTag(tagName, methodName, value);
		}
	}
}
