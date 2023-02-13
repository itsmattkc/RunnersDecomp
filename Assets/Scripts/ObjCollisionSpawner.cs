using UnityEngine;

public class ObjCollisionSpawner : SpawnableBehavior
{
	[SerializeField]
	private ObjCollisionParameter m_parameter;

	public override void SetParameters(SpawnableParameter srcParameter)
	{
		ObjCollisionParameter objCollisionParameter = srcParameter as ObjCollisionParameter;
		if (objCollisionParameter != null && !ObjUtil.IsUseTemporarySet())
		{
			BoxCollider component = GetComponent<BoxCollider>();
			if ((bool)component)
			{
				component.size = objCollisionParameter.GetSize();
			}
		}
	}

	public override SpawnableParameter GetParameter()
	{
		return m_parameter;
	}

	public override SpawnableParameter GetParameterForExport()
	{
		BoxCollider component = GetComponent<BoxCollider>();
		if ((bool)component)
		{
			m_parameter.SetSize(component.size);
		}
		return m_parameter;
	}
}
