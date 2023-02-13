using UnityEngine;

public class ObjCannonSpawner : SpawnableBehavior
{
	[SerializeField]
	private ObjCannonParameter m_parameter;

	public override void SetParameters(SpawnableParameter srcParameter)
	{
		ObjCannonParameter objCannonParameter = srcParameter as ObjCannonParameter;
		if (objCannonParameter != null)
		{
			ObjCannon component = GetComponent<ObjCannon>();
			if ((bool)component)
			{
				component.SetObjCannonParameter(objCannonParameter);
			}
		}
	}

	public override SpawnableParameter GetParameter()
	{
		return m_parameter;
	}
}
