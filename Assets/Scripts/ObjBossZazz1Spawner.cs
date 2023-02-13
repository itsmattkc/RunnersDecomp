using UnityEngine;

public class ObjBossZazz1Spawner : SpawnableBehavior
{
	[SerializeField]
	private ObjBossZazz1Parameter m_parameter;

	public override void SetParameters(SpawnableParameter srcParameter)
	{
		ObjBossZazz1Parameter objBossZazz1Parameter = srcParameter as ObjBossZazz1Parameter;
		if (objBossZazz1Parameter != null)
		{
			ObjBossZazz1 component = GetComponent<ObjBossZazz1>();
			if ((bool)component)
			{
				component.SetObjBossZazz1Parameter(objBossZazz1Parameter);
			}
		}
	}

	public override SpawnableParameter GetParameter()
	{
		return m_parameter;
	}
}
