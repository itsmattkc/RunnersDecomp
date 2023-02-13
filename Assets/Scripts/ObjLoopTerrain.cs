using Message;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Common/ObjFloor")]
public class ObjLoopTerrain : SpawnableObject
{
	private const float Path_DefaultZOFfset = 1.5f;

	private PathComponent m_component;

	private string m_pathName;

	private float m_pathZOffset;

	protected override void OnSpawned()
	{
		if (m_pathName != null)
		{
			PathManager pathManager = GameObjectUtil.FindGameObjectComponent<PathManager>("StagePathManager");
			if (pathManager != null)
			{
				Vector3 position = base.transform.position;
				position.z += m_pathZOffset + 1.5f;
				m_component = pathManager.CreatePathComponent(m_pathName, position);
			}
		}
	}

	private void OnDestroy()
	{
		PathManager pathManager = GameObjectUtil.FindGameObjectComponent<PathManager>("StagePathManager");
		if (pathManager != null && m_component != null)
		{
			pathManager.DestroyComponent(m_component);
		}
		m_component = null;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (m_component != null)
		{
			MsgRunLoopPath value = new MsgRunLoopPath(m_component);
			other.gameObject.SendMessage("OnRunLoopPath", value, SendMessageOptions.DontRequireReceiver);
		}
	}

	public void SetPathName(string pathName)
	{
		m_pathName = pathName;
	}

	public void SetZOffset(float zoffset)
	{
		m_pathZOffset = zoffset;
	}
}
