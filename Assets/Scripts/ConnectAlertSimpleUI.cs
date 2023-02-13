using UnityEngine;

public class ConnectAlertSimpleUI : MonoBehaviour
{
	private int m_refCount;

	private GameObject m_alertObj;

	private void Start()
	{
		m_alertObj = GameObjectUtil.FindChildGameObject(base.gameObject, "Alert");
		if (m_alertObj != null)
		{
			m_alertObj.SetActive(false);
		}
		base.enabled = false;
	}

	public void StartCollider()
	{
		m_refCount++;
		if (m_alertObj != null)
		{
			m_alertObj.SetActive(true);
		}
	}

	public void EndCollider()
	{
		m_refCount--;
		if (m_refCount <= 0)
		{
			m_refCount = 0;
			if (m_alertObj != null)
			{
				m_alertObj.SetActive(false);
			}
		}
	}
}
