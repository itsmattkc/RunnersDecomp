using UnityEngine;

public class ObjAirTrapSpawner : SpawnableBehavior
{
	[SerializeField]
	private ObjAirTrapParameter m_parameter;

	public override void SetParameters(SpawnableParameter srcParameter)
	{
		ObjAirTrapParameter objAirTrapParameter = srcParameter as ObjAirTrapParameter;
		if (objAirTrapParameter != null)
		{
			ObjAirTrap component = GetComponent<ObjAirTrap>();
			if ((bool)component)
			{
				component.SetObjAirTrapParameter(objAirTrapParameter);
			}
		}
	}

	public override SpawnableParameter GetParameter()
	{
		return m_parameter;
	}
}
