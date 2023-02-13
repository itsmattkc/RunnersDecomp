using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
	private Dictionary<string, ResPathObjectData> m_pathList;

	private uint m_idCounter;

	public bool m_drawGismos;

	private string m_svPathName = string.Empty;

	public bool SetupEnd
	{
		get;
		private set;
	}

	public Dictionary<string, ResPathObjectData> PathList
	{
		get
		{
			return m_pathList;
		}
	}

	private void Start()
	{
		if (m_pathList == null)
		{
			m_pathList = new Dictionary<string, ResPathObjectData>();
		}
	}

	public void CreatePathObjectData()
	{
		ResourceManager instance = ResourceManager.Instance;
		if (m_pathList == null)
		{
			m_pathList = new Dictionary<string, ResPathObjectData>();
		}
		GameObject gameObject = instance.GetGameObject(ResourceCategory.TERRAIN_MODEL, TerrainXmlData.DataAssetName);
		if ((bool)gameObject)
		{
			TerrainXmlData component = gameObject.GetComponent<TerrainXmlData>();
			StartCoroutine(ParseAndCreateDatas(component));
		}
	}

	public PathComponent CreatePathComponent(string name, Vector3 rootPosition)
	{
		ResPathObject resPathObject = CreatePathObject(name, rootPosition);
		if (resPathObject != null)
		{
			GameObject gameObject = new GameObject("PathComponentObject");
			PathComponent pathComponent = gameObject.AddComponent<PathComponent>();
			if ((bool)pathComponent)
			{
				gameObject.transform.position = rootPosition;
				gameObject.transform.parent = base.transform;
				pathComponent.SetManager(this);
				pathComponent.SetObject(resPathObject);
				pathComponent.SetID(m_idCounter);
				m_idCounter++;
				return pathComponent;
			}
		}
		return null;
	}

	public PathComponent GetPathComponent(string name)
	{
		PathComponent[] componentsInChildren = GetComponentsInChildren<PathComponent>();
		PathComponent[] array = componentsInChildren;
		foreach (PathComponent pathComponent in array)
		{
			if (pathComponent.GetName() == name)
			{
				return pathComponent;
			}
		}
		return null;
	}

	public PathComponent GetPathComponent(uint id)
	{
		PathComponent[] componentsInChildren = GetComponentsInChildren<PathComponent>();
		PathComponent[] array = componentsInChildren;
		foreach (PathComponent pathComponent in array)
		{
			if (pathComponent.GetID() == id)
			{
				return pathComponent;
			}
		}
		return null;
	}

	private ResPathObject CreatePathObject(string name, Vector3 rootPosition)
	{
		if (m_pathList == null)
		{
			return null;
		}
		string key = name.ToLower();
		ResPathObjectData value;
		m_pathList.TryGetValue(key, out value);
		if (value != null)
		{
			return new ResPathObject(value, rootPosition);
		}
		return null;
	}

	public void DestroyComponent(string pathname)
	{
		PathComponent pathComponent = GetPathComponent(pathname);
		DestroyComponent(pathComponent);
	}

	public void DestroyComponent(PathComponent component)
	{
		if (component != null)
		{
			Object.Destroy(component.gameObject);
		}
	}

	public string GetSVPathName()
	{
		return m_svPathName;
	}

	private IEnumerator ParseAndCreateDatas(TerrainXmlData terrainData)
	{
		if (terrainData != null)
		{
			TextAsset asset2 = terrainData.LoopPath;
			if (asset2 != null && asset2.text != null)
			{
				yield return StartCoroutine(PathXmlDeserializer.CreatePathObjectData(asset2, m_pathList));
			}
			asset2 = terrainData.SideViewPath;
			if (asset2 != null)
			{
				m_svPathName = asset2.name;
			}
			if (asset2 != null && asset2.text != null)
			{
				yield return StartCoroutine(PathXmlDeserializer.CreatePathObjectData(asset2, m_pathList));
			}
		}
		SetupEnd = true;
	}

	private void Update()
	{
	}

	private void OnDrawGizmos()
	{
	}
}
