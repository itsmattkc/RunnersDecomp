using UnityEngine;

public class ObjSpringSpawner : SpawnableBehavior
{
	[SerializeField]
	private ObjSpringParameter m_parameter;

	public override void SetParameters(SpawnableParameter srcParameter)
	{
		ObjSpringParameter objSpringParameter = srcParameter as ObjSpringParameter;
		if (objSpringParameter != null)
		{
			ObjSpring component = GetComponent<ObjSpring>();
			if ((bool)component)
			{
				component.SetObjSpringParameter(objSpringParameter);
			}
		}
	}

	public override SpawnableParameter GetParameter()
	{
		return m_parameter;
	}
}
