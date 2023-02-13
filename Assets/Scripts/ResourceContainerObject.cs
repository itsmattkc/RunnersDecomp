using System.Collections.Generic;
using UnityEngine;

public class ResourceContainerObject : MonoBehaviour
{
	private Dictionary<string, ResourceContainer> m_resContainer = new Dictionary<string, ResourceContainer>();

	public ResourceCategory Category
	{
		get;
		set;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public ResourceContainer GetContainer(string name)
	{
		ResourceContainer value = null;
		m_resContainer.TryGetValue(name, out value);
		return value;
	}

	public ResourceContainer CreateContainer(string name)
	{
		ResourceContainer resourceContainer = new ResourceContainer(Category, name);
		m_resContainer.Add(name, resourceContainer);
		return resourceContainer;
	}

	public ResourceContainer CreateEmptyContainer(string name)
	{
		ResourceContainer resourceContainer = new ResourceContainer(Category, name);
		m_resContainer.Add(name, resourceContainer);
		GameObject gameObject = new GameObject(name);
		resourceContainer.SetRootObject(gameObject);
		resourceContainer.DontDestroyOnChangeScene = true;
		gameObject.transform.parent = base.transform;
		return resourceContainer;
	}

	public GameObject GetGameObject(string name)
	{
		foreach (ResourceContainer value in m_resContainer.Values)
		{
			if (value.Active)
			{
				GameObject @object = value.GetObject(name);
				if (@object != null)
				{
					return @object;
				}
			}
		}
		return null;
	}

	public void RemoveAllResources()
	{
		foreach (ResourceContainer value in m_resContainer.Values)
		{
			value.RemoveAllResources();
		}
		m_resContainer.Clear();
	}

	public void RemoveResourcesOnThisScene()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, ResourceContainer> item in m_resContainer)
		{
			if (!item.Value.DontDestroyOnChangeScene)
			{
				item.Value.RemoveAllResources();
				list.Add(item.Key);
			}
			else
			{
				item.Value.RemoveResourcesOnThisScene();
			}
		}
		foreach (string item2 in list)
		{
			m_resContainer.Remove(item2);
		}
	}

	public void RemoveResources(string[] removeList)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, ResourceContainer> item in m_resContainer)
		{
			string key = item.Key;
			ResourceContainer value = item.Value;
			if (!value.Active)
			{
				continue;
			}
			value.RemoveResources(removeList);
			foreach (string text in removeList)
			{
				if (!string.IsNullOrEmpty(text) && value.Name == text)
				{
					list.Add(key);
					break;
				}
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (string item2 in list)
		{
			if (!string.IsNullOrEmpty(item2))
			{
				m_resContainer.Remove(item2);
			}
		}
	}

	public void SetContainerActive(string name, bool value)
	{
		ResourceContainer container = GetContainer(name);
		if (container != null)
		{
			container.Active = value;
		}
	}

	public void RemoveNotActiveContainer()
	{
		List<string> list = new List<string>();
		foreach (ResourceContainer value in m_resContainer.Values)
		{
			if (!value.Active)
			{
				list.Add(value.Name);
			}
		}
		foreach (string item in list)
		{
			RemoveContainer(item);
		}
	}

	private void RemoveContainer(string name)
	{
		ResourceContainer container = GetContainer(name);
		if (container != null)
		{
			container.RemoveAllResources();
			m_resContainer.Remove(name);
		}
	}
}
