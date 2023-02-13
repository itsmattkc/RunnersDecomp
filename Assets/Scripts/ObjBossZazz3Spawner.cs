using UnityEngine;

public class ObjBossZazz3Spawner : SpawnableBehavior
{
	[SerializeField]
	private ObjBossZazz3Parameter m_parameter;

	public override void SetParameters(SpawnableParameter srcParameter)
	{
		ObjBossZazz3Parameter objBossZazz3Parameter = srcParameter as ObjBossZazz3Parameter;
		if (objBossZazz3Parameter != null)
		{
			ObjBossZazz3 component = GetComponent<ObjBossZazz3>();
			if ((bool)component)
			{
				component.SetObjBossZazz3Parameter(objBossZazz3Parameter);
			}
		}
	}

	public override SpawnableParameter GetParameter()
	{
		return m_parameter;
	}
}
