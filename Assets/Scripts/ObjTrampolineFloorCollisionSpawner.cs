using UnityEngine;

public class ObjTrampolineFloorCollisionSpawner : SpawnableBehavior
{
	[SerializeField]
	private ObjTrampolineFloorCollisionParameter m_parameter;

	public override void SetParameters(SpawnableParameter srcParameter)
	{
		ObjTrampolineFloorCollisionParameter objTrampolineFloorCollisionParameter = srcParameter as ObjTrampolineFloorCollisionParameter;
		if (objTrampolineFloorCollisionParameter == null)
		{
			return;
		}
		if (!ObjUtil.IsUseTemporarySet())
		{
			BoxCollider component = GetComponent<BoxCollider>();
			if ((bool)component)
			{
				component.size = objTrampolineFloorCollisionParameter.GetSize();
			}
		}
		ObjTrampolineFloorCollision component2 = GetComponent<ObjTrampolineFloorCollision>();
		if ((bool)component2)
		{
			component2.SetObjCollisionParameter(objTrampolineFloorCollisionParameter);
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
