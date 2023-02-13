using System.Collections.Generic;
using UnityEngine;

public class ResourceContainer
{
	private bool m_active = true;

	private ResourceInfo m_rootResource;

	private Dictionary<string, ResourceInfo> m_resources;

	private string m_name;

	public string Name
	{
		get
		{
			return m_name;
		}
	}

	public ResourceCategory Category
	{
		get
		{
			return m_rootResource.Category;
		}
	}

	public bool Active
	{
		get
		{
			return m_active;
		}
		set
		{
			m_active = value;
		}
	}

	public bool DontDestroyOnChangeScene
	{
		get
		{
			if (m_rootResource != null)
			{
				return m_rootResource.DontDestroyOnChangeScene;
			}
			return false;
		}
		set
		{
			if (m_rootResource != null)
			{
				m_rootResource.DontDestroyOnChangeScene = value;
			}
		}
	}

	public ResourceContainer(ResourceCategory category, string name)
	{
		m_rootResource = new ResourceInfo(category);
		m_resources = new Dictionary<string, ResourceInfo>();
		m_name = name;
	}

	public bool SetRootObject(GameObject srcObject)
	{
		if (m_rootResource.ResObject == null)
		{
			m_rootResource.ResObject = srcObject;
			return true;
		}
		return false;
	}

	public bool SetRootObject(ResourceInfo resInfo)
	{
		if (m_rootResource.ResObject == null)
		{
			resInfo.CopyTo(m_rootResource);
			return true;
		}
		return false;
	}

	public void AddChildObject(GameObject srcObject, bool dontDestoryOnChangeScene)
	{
		ResourceInfo resourceInfo = new ResourceInfo(m_rootResource.Category);
		resourceInfo.ResObject = srcObject;
		resourceInfo.PathName = m_rootResource.PathName;
		resourceInfo.DontDestroyOnChangeScene = dontDestoryOnChangeScene;
		m_resources.Add(srcObject.name, resourceInfo);
	}

	public bool IsExist(GameObject gameObject)
	{
		return m_resources.ContainsKey(gameObject.name);
	}

	public GameObject GetObject(string objectName)
	{
		if (m_rootResource.ResObject != null && m_rootResource.ResObject.name == objectName)
		{
			return m_rootResource.ResObject;
		}
		ResourceInfo value;
		m_resources.TryGetValue(objectName, out value);
		if (value != null)
		{
			return value.ResObject;
		}
		return null;
	}

	public void RemoveAllResources()
	{
		foreach (ResourceInfo value in m_resources.Values)
		{
			DestroyObject(value);
		}
		m_resources.Clear();
		if (m_rootResource != null && m_rootResource.ResObject != null)
		{
			Object.Destroy(m_rootResource.ResObject);
			m_rootResource = null;
		}
	}

	public void RemoveResourcesOnThisScene()
	{
		List<string> list = new List<string>();
		foreach (ResourceInfo value in m_resources.Values)
		{
			if (!value.DontDestroyOnChangeScene)
			{
				string name = value.ResObject.name;
				DestroyObject(value);
				list.Add(name);
			}
		}
		foreach (string item in list)
		{
			m_resources.Remove(item);
		}
	}

	public void RemoveResources(string[] names)
	{
		foreach (string text in names)
		{
			ResourceInfo value;
			m_resources.TryGetValue(text, out value);
			if (value != null)
			{
				DestroyObject(value);
				m_resources.Remove(text);
			}
			if (m_rootResource.ResObject != null && m_rootResource.ResObject.name == text)
			{
				DestroyObject(m_rootResource);
				break;
			}
		}
	}

	private void DestroyObject(ResourceInfo info)
	{
		Object.Destroy(info.ResObject);
	}
}
