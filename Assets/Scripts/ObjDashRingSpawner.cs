using UnityEngine;

public class ObjDashRingSpawner : SpawnableBehavior
{
	[SerializeField]
	private ObjDashRingParameter m_parameter;

	public override void SetParameters(SpawnableParameter srcParameter)
	{
		ObjDashRingParameter objDashRingParameter = srcParameter as ObjDashRingParameter;
		if (objDashRingParameter != null)
		{
			ObjDashRing component = GetComponent<ObjDashRing>();
			if ((bool)component)
			{
				component.SetObjDashRingParameter(objDashRingParameter);
			}
		}
	}

	public override SpawnableParameter GetParameter()
	{
		return m_parameter;
	}
}
