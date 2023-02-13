using UnityEngine;

public class ObjSpringAirSpawner : SpawnableBehavior
{
	[SerializeField]
	private ObjSpringAirParameter m_parameter;

	public override void SetParameters(SpawnableParameter srcParameter)
	{
		ObjSpringAirParameter objSpringAirParameter = srcParameter as ObjSpringAirParameter;
		if (objSpringAirParameter != null)
		{
			ObjSpringAir component = GetComponent<ObjSpringAir>();
			if ((bool)component)
			{
				component.SetObjSpringAirParameter(objSpringAirParameter);
			}
		}
	}

	public override SpawnableParameter GetParameter()
	{
		return m_parameter;
	}
}
