using UnityEngine;

[AddComponentMenu("Scripts/Runners/Game/Level")]
public abstract class SpawnableObject : MonoBehaviour
{
	[SerializeField]
	private StockObjectType m_stockObjectType = StockObjectType.UNKNOWN;

	private SpawnableInfo m_spawnableInfo;

	private bool m_sleep;

	private bool m_share;

	public bool Sleep
	{
		get
		{
			return m_sleep;
		}
		set
		{
			m_sleep = value;
		}
	}

	public bool Share
	{
		get
		{
			return m_share;
		}
		set
		{
			m_share = value;
		}
	}

	private void Start()
	{
		Spawn();
	}

	private void Spawn()
	{
		if (!IsSpawnedByManager())
		{
			SpawnableBehavior component = GetComponent<SpawnableBehavior>();
			if ((bool)component)
			{
				component.SetParameters(component.GetParameter());
			}
		}
		OnSpawned();
	}

	public GameObject AttachModelObject()
	{
		string modelName = GetModelName();
		if (modelName != null)
		{
			ResourceCategory modelCategory = GetModelCategory();
			GameObject gameObject = ResourceManager.Instance.GetGameObject(modelCategory, modelName);
			if ((bool)gameObject)
			{
				GameObject gameObject2 = Object.Instantiate(gameObject, Vector3.zero, Quaternion.identity) as GameObject;
				if ((bool)gameObject2)
				{
					gameObject2.SetActive(true);
					gameObject2.transform.parent = base.transform;
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.transform.localRotation = Quaternion.Euler(Vector3.zero);
					return gameObject2;
				}
			}
		}
		return null;
	}

	protected GameObject AttachObject(ResourceCategory category, string objectName)
	{
		return AttachObject(category, objectName, Vector3.zero, Quaternion.identity);
	}

	protected GameObject AttachObject(ResourceCategory category, string objectName, Vector3 localPosition, Quaternion localRotation)
	{
		if (IsValid() && objectName != null)
		{
			GameObject gameObject = ResourceManager.Instance.GetGameObject(category, objectName);
			if ((bool)gameObject)
			{
				GameObject gameObject2 = Object.Instantiate(gameObject, base.transform.position, base.transform.rotation) as GameObject;
				if ((bool)gameObject2)
				{
					gameObject2.SetActive(true);
					gameObject2.transform.parent = base.transform;
					gameObject2.transform.localPosition = localPosition;
					gameObject2.transform.localRotation = localRotation;
					return gameObject2;
				}
			}
		}
		return null;
	}

	private void OnDestroy()
	{
		OnDestroyed();
		ObjectSpawnManager manager = GetManager();
		if (m_spawnableInfo != null && manager != null)
		{
			manager.DetachObject(m_spawnableInfo);
		}
		m_spawnableInfo = null;
	}

	protected void SetSleep(GameObject obj)
	{
		ObjectSpawnManager manager = GetManager();
		if (manager != null)
		{
			SpawnableObject component = obj.GetComponent<SpawnableObject>();
			if (component.IsStockObject())
			{
				manager.SleepSpawnableObject(component);
			}
		}
	}

	public void SetSleep()
	{
		ObjectSpawnManager manager = GetManager();
		if (manager != null && IsStockObject())
		{
			manager.SleepSpawnableObject(this);
		}
	}

	protected abstract void OnSpawned();

	public virtual void OnCreate()
	{
	}

	public virtual void OnRevival()
	{
	}

	protected virtual void OnDestroyed()
	{
	}

	public void AttachSpawnableInfo(SpawnableInfo info)
	{
		m_spawnableInfo = info;
	}

	public bool IsSpawnedByManager()
	{
		return m_spawnableInfo != null;
	}

	protected ObjectSpawnManager GetManager()
	{
		if (m_spawnableInfo != null)
		{
			return m_spawnableInfo.Manager;
		}
		return null;
	}

	protected virtual string GetModelName()
	{
		return null;
	}

	protected virtual ResourceCategory GetModelCategory()
	{
		return ResourceCategory.UNKNOWN;
	}

	protected virtual bool isStatic()
	{
		return false;
	}

	public virtual bool IsValid()
	{
		return true;
	}

	protected void SetOnlyOneObject()
	{
		if (m_spawnableInfo != null)
		{
			m_spawnableInfo.AttributeOnlyOne = true;
			ObjectSpawnManager manager = GetManager();
			if ((bool)manager)
			{
				manager.RegisterOnlyOneObject(m_spawnableInfo);
			}
		}
	}

	protected void SetNotRageout(bool value)
	{
		if (m_spawnableInfo != null)
		{
			m_spawnableInfo.NotRangeOut = value;
		}
	}

	public StockObjectType GetStockObjectType()
	{
		return m_stockObjectType;
	}

	public bool IsStockObject()
	{
		return m_stockObjectType != StockObjectType.UNKNOWN;
	}
}
