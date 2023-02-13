using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
	private const string SimpleContainerName = "SimpleContainer";

	private Dictionary<ResourceCategory, ResourceContainerObject> m_container;

	private static ResourceManager instance;

	public static ResourceManager Instance
	{
		get
		{
			return instance;
		}
	}

	protected void Awake()
	{
		CheckInstance();
	}

	private void Start()
	{
		Initialize();
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Initialize()
	{
		if (m_container == null)
		{
			m_container = new Dictionary<ResourceCategory, ResourceContainerObject>();
			for (ResourceCategory resourceCategory = ResourceCategory.OBJECT_RESOURCE; resourceCategory < ResourceCategory.NUM; resourceCategory++)
			{
				GameObject gameObject = new GameObject(resourceCategory.ToString());
				gameObject.transform.parent = base.transform;
				ResourceContainerObject resourceContainerObject = gameObject.AddComponent<ResourceContainerObject>();
				resourceContainerObject.Category = resourceCategory;
				resourceContainerObject.CreateEmptyContainer("SimpleContainer");
				m_container.Add(resourceCategory, resourceContainerObject);
			}
		}
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		if (m_container != null)
		{
			RemoveAllResources();
		}
		if (instance == this)
		{
			instance = null;
		}
	}

	public void AddCategorySceneObjects(ResourceCategory category, string containerName, GameObject resourceRootObject, bool dontDestroyOnChangeScene)
	{
		if (resourceRootObject == null)
		{
			return;
		}
		ResourceContainerObject resourceContainerObject = m_container[category];
		if (containerName == null)
		{
			containerName = resourceRootObject.name;
		}
		ResourceContainer resourceContainer = resourceContainerObject.GetContainer(containerName);
		if (resourceContainer == null)
		{
			resourceContainer = resourceContainerObject.CreateContainer(containerName);
			resourceContainer.DontDestroyOnChangeScene = dontDestroyOnChangeScene;
			resourceContainer.SetRootObject(resourceRootObject);
			resourceRootObject.transform.parent = resourceContainerObject.gameObject.transform;
		}
		foreach (Transform item in resourceRootObject.transform)
		{
			if (!resourceContainer.IsExist(item.gameObject))
			{
				resourceContainer.AddChildObject(item.gameObject, dontDestroyOnChangeScene);
				item.gameObject.SetActive(false);
			}
		}
	}

	public void AddCategorySceneObjectsAndSetActive(ResourceCategory category, string containerName, GameObject resourceRootObject, bool dontDestroyOnSceneChange)
	{
		AddCategorySceneObjects(category, containerName, resourceRootObject, dontDestroyOnSceneChange);
		SetContainerActive(category, containerName, true);
	}

	public GameObject GetGameObject(ResourceCategory category, string name)
	{
		return m_container[category].GetGameObject(name);
	}

	public GameObject GetSpawnableGameObject(string name)
	{
		ResourceCategory[] array = new ResourceCategory[4]
		{
			ResourceCategory.OBJECT_PREFAB,
			ResourceCategory.ENEMY_PREFAB,
			ResourceCategory.STAGE_RESOURCE,
			ResourceCategory.EVENT_RESOURCE
		};
		ResourceCategory[] array2 = array;
		foreach (ResourceCategory category in array2)
		{
			GameObject gameObject = GetGameObject(category, name);
			if ((bool)gameObject)
			{
				return gameObject;
			}
		}
		return null;
	}

	public bool IsExistContainer(string name)
	{
		for (ResourceCategory resourceCategory = ResourceCategory.OBJECT_RESOURCE; resourceCategory < ResourceCategory.NUM; resourceCategory++)
		{
			if (m_container[resourceCategory] == null)
			{
				return false;
			}
			if (m_container[resourceCategory].GetContainer(name) != null)
			{
				return true;
			}
		}
		return false;
	}

	private void AddObject(ResourceCategory category, GameObject addObject, bool dontDestroyOnChangeScene)
	{
		AddObject(category, "SimpleContainer", addObject, dontDestroyOnChangeScene);
	}

	private void AddObject(ResourceCategory category, string containerName, GameObject addObject, bool dontDestroyOnChangeScene)
	{
		if (m_container != null)
		{
			if (category == ResourceCategory.ETC)
			{
				dontDestroyOnChangeScene = false;
			}
			ResourceContainer container = m_container[category].GetContainer(containerName);
			if (container != null)
			{
				container.AddChildObject(addObject, dontDestroyOnChangeScene);
			}
		}
	}

	public void RemoveAllResources()
	{
		if (m_container == null)
		{
			return;
		}
		foreach (ResourceContainerObject value in m_container.Values)
		{
			value.RemoveAllResources();
		}
	}

	public void RemoveResourcesOnThisScene()
	{
		foreach (ResourceContainerObject value in m_container.Values)
		{
			value.RemoveResourcesOnThisScene();
		}
	}

	public void RemoveResources(ResourceCategory category)
	{
		m_container[category].RemoveAllResources();
	}

	public void RemoveResources(ResourceCategory category, string[] removeList)
	{
		m_container[category].RemoveResources(removeList);
	}

	public void SetContainerActive(ResourceCategory category, string name, bool value)
	{
		m_container[category].SetContainerActive(name, value);
	}

	public void RemoveNotActiveContainer(ResourceCategory category)
	{
		m_container[category].RemoveNotActiveContainer();
	}

	protected bool CheckInstance()
	{
		if (instance == null)
		{
			instance = this;
			Initialize();
			return true;
		}
		if (this == Instance)
		{
			return true;
		}
		Object.Destroy(base.gameObject);
		return false;
	}
}
