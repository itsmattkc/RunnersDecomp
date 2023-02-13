using UnityEngine;

public class UIObjectContainer : MonoBehaviour
{
	[SerializeField]
	private HudMenuUtility.EffectPriority m_priority;

	[SerializeField]
	private GameObject[] m_objects = new GameObject[0];

	private bool[] m_activeFlags;

	public HudMenuUtility.EffectPriority Priority
	{
		get
		{
			return m_priority;
		}
		set
		{
			m_priority = value;
		}
	}

	public GameObject[] Objects
	{
		get
		{
			return m_objects;
		}
		set
		{
			m_objects = value;
		}
	}

	public void SetActive(bool isActive)
	{
		if (m_activeFlags == null || m_objects.Length != m_activeFlags.Length)
		{
			return;
		}
		for (int i = 0; i < m_objects.Length; i++)
		{
			if (m_objects[i] == null)
			{
				continue;
			}
			if (isActive)
			{
				if (!m_objects[i].activeSelf && m_activeFlags[i])
				{
					m_objects[i].SetActive(true);
				}
			}
			else if (m_objects[i].activeSelf)
			{
				m_activeFlags[i] = true;
				m_objects[i].SetActive(false);
			}
		}
	}

	private void Start()
	{
		UIEffectManager instance = UIEffectManager.Instance;
		if (instance != null)
		{
			instance.AddEffect(this);
		}
		if (m_objects.Length > 0)
		{
			m_activeFlags = new bool[m_objects.Length];
			if (m_activeFlags != null)
			{
				for (int i = 0; i < m_activeFlags.Length; i++)
				{
					m_activeFlags[i] = false;
				}
			}
		}
		base.enabled = false;
	}

	private void OnDestroy()
	{
		m_objects = null;
		m_activeFlags = null;
		UIEffectManager instance = UIEffectManager.Instance;
		if (instance != null)
		{
			instance.RemoveEffect(this);
		}
	}

	private void Update()
	{
	}
}
