using UnityEngine;

public class EventRaidBossAttackRateTable : MonoBehaviour
{
	[SerializeField]
	private TextAsset m_xml_data;

	private void Start()
	{
		EventManager instance = EventManager.Instance;
		if (instance != null)
		{
			instance.SetRaidBossAttacRate(m_xml_data);
		}
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
	}
}
