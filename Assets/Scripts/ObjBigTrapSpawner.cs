using UnityEngine;

public class ObjBigTrapSpawner : SpawnableBehavior
{
	[SerializeField]
	private ObjBigTrapParameter m_parameter;

	public override void SetParameters(SpawnableParameter srcParameter)
	{
		ObjBigTrapParameter objBigTrapParameter = srcParameter as ObjBigTrapParameter;
		if (objBigTrapParameter != null)
		{
			ObjBigTrap component = GetComponent<ObjBigTrap>();
			if ((bool)component)
			{
				component.SetObjBigTrapParameter(objBigTrapParameter);
			}
		}
	}

	public override SpawnableParameter GetParameter()
	{
		return m_parameter;
	}
}
