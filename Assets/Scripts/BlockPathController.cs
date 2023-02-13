using UnityEngine;

public class BlockPathController : MonoBehaviour
{
	public enum PathType
	{
		SV,
		DRILL,
		LASER,
		NUM_PATH
	}

	private int m_numBlock;

	private int m_activateID;

	private bool m_nowCurrent;

	private PlayerInformation m_playerInformation;

	private PathManager m_pathManager;

	[SerializeField]
	private bool m_drawGismos;

	[SerializeField]
	private bool m_dispInfo;

	private Rect m_window;

	private PathComponent[] m_pathComponent;

	private PathEvaluator[] m_pathEvaluator;

	private static readonly string[] path_name_suffix = new string[3]
	{
		"_sv",
		"_dr",
		"_ls"
	};

	public int BlockNo
	{
		get
		{
			return m_numBlock;
		}
	}

	public int ActivateID
	{
		get
		{
			return m_activateID;
		}
	}

	private void Start()
	{
		m_dispInfo = false;
	}

	private void Update()
	{
		if (!m_nowCurrent || m_pathEvaluator == null || m_playerInformation == null)
		{
			return;
		}
		Vector3 position = m_playerInformation.Position;
		position.y = 0f;
		PathEvaluator[] pathEvaluator = m_pathEvaluator;
		foreach (PathEvaluator pathEvaluator2 in pathEvaluator)
		{
			if (pathEvaluator2 != null)
			{
				float distance = pathEvaluator2.Distance;
				float dist = distance;
				pathEvaluator2.GetClosestPositionAlongSpline(position, distance - 5f, distance + 5f, out dist);
				pathEvaluator2.Distance = dist;
			}
		}
	}

	private void OnDestroy()
	{
		if (!m_pathManager)
		{
			return;
		}
		DestroyPathEvaluator();
		if (m_pathComponent != null)
		{
			for (int i = 0; i < 3; i++)
			{
				if (m_pathComponent[i] != null)
				{
					m_pathManager.DestroyComponent(m_pathComponent[i]);
				}
				m_pathComponent[i] = null;
			}
			m_pathComponent = null;
		}
		m_pathManager = null;
	}

	public void Initialize(string stageName, int numBlock, int activateID, PathManager manager, Vector3 rootPosition)
	{
		m_numBlock = numBlock;
		m_pathManager = manager;
		m_activateID = activateID;
		m_playerInformation = GameObjectUtil.FindGameObjectComponent<PlayerInformation>("PlayerInformation");
		m_pathComponent = new PathComponent[3];
		for (int i = 0; i < 3; i++)
		{
			string name = stageName + "Terrain" + numBlock.ToString("D2") + path_name_suffix[i];
			m_pathComponent[i] = manager.CreatePathComponent(name, rootPosition);
			if (i > 0 && m_pathComponent[i] == null && m_pathComponent[0] != null)
			{
				Vector3 rootPosition2 = rootPosition + ((i != 1) ? new Vector3(0f, 5f, 0f) : new Vector3(0f, -2f, 0f));
				m_pathComponent[i] = manager.CreatePathComponent(name, rootPosition2);
			}
		}
		base.transform.position = rootPosition;
	}

	public void SetCurrent(bool value)
	{
		if (!IsNowCurrent() && value)
		{
			CreatePathEvaluator();
		}
		else if (IsNowCurrent() && !value)
		{
			m_nowCurrent = value;
		}
		m_nowCurrent = value;
	}

	public bool IsNowCurrent()
	{
		return m_nowCurrent;
	}

	public PathEvaluator GetEvaluator(PathType type)
	{
		if (m_pathEvaluator == null)
		{
			return null;
		}
		return m_pathEvaluator[(int)type];
	}

	public PathComponent GetComponent(PathType type)
	{
		if (m_pathComponent == null)
		{
			return null;
		}
		return m_pathComponent[(int)type];
	}

	public bool GetPNT(PathType type, ref Vector3? pos, ref Vector3? nrm, ref Vector3? tan)
	{
		PathEvaluator evaluator = GetEvaluator(type);
		if (evaluator == null)
		{
			return false;
		}
		evaluator.GetPNT(ref pos, ref nrm, ref tan);
		return true;
	}

	private void CreatePathEvaluator()
	{
		if (!(m_pathComponent[0] != null))
		{
			return;
		}
		m_pathEvaluator = new PathEvaluator[3];
		for (int i = 0; i < 3; i++)
		{
			if (!(m_pathComponent[i] == null) && m_pathComponent[i].IsValid())
			{
				m_pathEvaluator[i] = new PathEvaluator();
				m_pathEvaluator[i].SetPathObject(m_pathComponent[i]);
				if (m_playerInformation != null)
				{
					Vector3 position = m_playerInformation.Position;
					float dist = 0f;
					m_pathEvaluator[i].GetClosestPositionAlongSpline(position, 0f, m_pathEvaluator[i].GetLength(), out dist);
					m_pathEvaluator[i].Distance = dist;
				}
			}
		}
	}

	private void DestroyPathEvaluator()
	{
		if (m_pathEvaluator != null)
		{
			for (int i = 0; i < 3; i++)
			{
				m_pathEvaluator[i] = null;
			}
			m_pathEvaluator = null;
		}
	}

	private void OnDrawGizmos()
	{
	}
}
