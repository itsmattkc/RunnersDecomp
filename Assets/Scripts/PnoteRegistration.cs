using UnityEngine;

public class PnoteRegistration : MonoBehaviour
{
	private bool m_enable = true;

	private void Start()
	{
		m_enable = false;
		Object.Destroy(base.gameObject);
	}

	private void Update()
	{
		if (!m_enable)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
