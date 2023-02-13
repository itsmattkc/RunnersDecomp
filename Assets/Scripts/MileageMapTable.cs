using UnityEngine;

public class MileageMapTable : MonoBehaviour
{
	[SerializeField]
	private TextAsset m_xml_data;

	private void Start()
	{
		MileageMapDataManager instance = MileageMapDataManager.Instance;
		if (instance != null)
		{
			instance.SetData(m_xml_data);
		}
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
	}
}
