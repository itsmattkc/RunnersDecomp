using UnityEngine;

public class SpecialStageEventDataTable : MonoBehaviour
{
	[SerializeField]
	private TextAsset m_xml_data;

	private void Start()
	{
		EventManager instance = EventManager.Instance;
		if (instance != null)
		{
			instance.SetEventMenuData(m_xml_data);
		}
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
	}
}
