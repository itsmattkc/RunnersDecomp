using UnityEngine;

public class UIDraggablePanelAssistance : MonoBehaviour
{
	private UIDraggablePanel m_draggablePanel;

	private SpringPanel m_spring;

	[SerializeField]
	private bool X_Axis = true;

	[SerializeField]
	private float X;

	[SerializeField]
	private bool Y_Axis = true;

	[SerializeField]
	private float Y;

	private bool autoCheck = true;

	private float checkTime = 0.5f;

	private long m_count;

	private bool m_isAssistanceEnable = true;

	private bool m_init;

	private float m_currentCheckTime;

	public bool isAssistanceEnable
	{
		get
		{
			bool result = false;
			if (m_draggablePanel != null && m_isAssistanceEnable)
			{
				result = true;
			}
			return result;
		}
		set
		{
			m_isAssistanceEnable = value;
		}
	}

	private void Start()
	{
		Init();
	}

	private void Update()
	{
		if (!m_init)
		{
			Init();
		}
		if (isAssistanceEnable)
		{
			Check();
		}
	}

	private void Check()
	{
		if (m_currentCheckTime <= 0f && !autoCheck)
		{
			return;
		}
		bool flag = false;
		if (autoCheck && m_count % 3 == 0L)
		{
			if (m_count % 3 == 0L)
			{
				flag = true;
			}
		}
		else if (m_currentCheckTime > 0f)
		{
			flag = true;
		}
		if (flag)
		{
			if (m_spring == null)
			{
				m_spring = base.gameObject.GetComponent<SpringPanel>();
			}
			if (m_spring != null && (X_Axis || Y_Axis))
			{
				float x = m_spring.target.x;
				float y = m_spring.target.y;
				float z = m_spring.target.z;
				bool flag2 = false;
				if (X_Axis && x != X)
				{
					m_spring.target = new Vector3(X, y, z);
					flag2 = true;
				}
				if (Y_Axis && y != Y)
				{
					m_spring.target = new Vector3(x, Y, z);
					flag2 = true;
				}
				if (flag2 && !m_spring.enabled)
				{
					m_spring.enabled = true;
				}
			}
			if (!autoCheck)
			{
				m_currentCheckTime -= Time.deltaTime;
			}
		}
		m_count++;
	}

	private void Init()
	{
		m_draggablePanel = base.gameObject.GetComponent<UIDraggablePanel>();
		m_init = true;
	}

	public void CheckDraggable()
	{
		m_currentCheckTime = checkTime;
	}
}
