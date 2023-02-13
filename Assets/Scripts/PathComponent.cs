using UnityEngine;

public class PathComponent : MonoBehaviour
{
	private ResPathObject m_resPathObject;

	private PathManager m_pathManager;

	private uint m_id;

	public void SetObject(ResPathObject pathObject)
	{
		m_resPathObject = pathObject;
	}

	public ResPathObject GetResPathObject()
	{
		return m_resPathObject;
	}

	private PathManager GetManager()
	{
		return m_pathManager;
	}

	public string GetName()
	{
		if (m_resPathObject != null)
		{
			return m_resPathObject.Name;
		}
		return null;
	}

	public uint GetID()
	{
		return m_id;
	}

	public bool IsValid()
	{
		return m_resPathObject != null;
	}

	private void Cleanup()
	{
		if (m_pathManager != null)
		{
			m_pathManager = null;
		}
		m_resPathObject = null;
	}

	private void OnDestroy()
	{
		Cleanup();
	}

	public void SetManager(PathManager manager)
	{
		m_pathManager = manager;
	}

	public void SetID(uint id)
	{
		m_id = id;
	}

	public void DrawGizmos()
	{
		if (m_resPathObject != null)
		{
			ResPathObjectData objectData = m_resPathObject.GetObjectData();
			Vector3 to = objectData.position[0];
			for (int i = 0; i < objectData.numKeys; i++)
			{
				Vector3 vector = objectData.position[i];
				Vector3 a = objectData.normal[i];
				float d = 0.2f;
				Gizmos.color = Color.blue;
				Gizmos.DrawLine(vector, vector + a * d);
				Gizmos.color = Color.yellow;
				Gizmos.DrawLine(vector, to);
				to = vector;
			}
		}
	}
}
