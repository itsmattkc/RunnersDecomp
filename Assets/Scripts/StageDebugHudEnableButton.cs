using UnityEngine;

public class StageDebugHudEnableButton : MonoBehaviour
{
	private GameObject m_hudCockpit;

	private GameObject m_DebugTraceManager;

	private AllocationStatus m_allocationStatus;

	private Rect m_buttonRect;

	private void Awake()
	{
		Object.Destroy(base.gameObject);
	}

	private void Start()
	{
		m_hudCockpit = GameObject.Find("HudCockpit");
		m_DebugTraceManager = GameObject.Find("DebugTraceManager");
		m_allocationStatus = GameObjectUtil.FindGameObjectComponent<AllocationStatus>("AllocationStatus");
		float num = 100f;
		m_buttonRect = new Rect(((float)Screen.width - num) * 0.5f, 0f, num, num);
	}

	private void OnGUI()
	{
		if (GUI.Button(m_buttonRect, string.Empty, GUIStyle.none))
		{
			bool flag = true;
			if (m_hudCockpit != null)
			{
				flag = !m_hudCockpit.activeSelf;
				m_hudCockpit.SetActive(flag);
			}
			if (m_DebugTraceManager != null)
			{
				m_DebugTraceManager.SetActive(flag);
			}
			if (m_allocationStatus != null)
			{
				m_allocationStatus.show = flag;
			}
		}
	}
}
