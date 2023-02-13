using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Common/ObjInfomationSign")]
public class ObjInfomationSign : SpawnableObject
{
	private const string ModelName = "obj_cmn_infomationsign";

	private GameObject m_infomationObject;

	public GameObject InfomationObject
	{
		get
		{
			return m_infomationObject;
		}
		set
		{
			if (m_infomationObject == null)
			{
				m_infomationObject = value;
				m_infomationObject.SetActive(true);
			}
		}
	}

	protected override string GetModelName()
	{
		return "obj_cmn_infomationsign";
	}

	protected override ResourceCategory GetModelCategory()
	{
		return ResourceCategory.OBJECT_RESOURCE;
	}

	protected override void OnSpawned()
	{
	}
}
