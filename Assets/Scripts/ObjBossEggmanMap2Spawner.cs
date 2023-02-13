using UnityEngine;

public class ObjBossEggmanMap2Spawner : SpawnableBehavior
{
	[SerializeField]
	private ObjBossEggmanMap2Parameter m_parameter;

	public override void SetParameters(SpawnableParameter srcParameter)
	{
		ObjBossEggmanMap2Parameter objBossEggmanMap2Parameter = srcParameter as ObjBossEggmanMap2Parameter;
		if (objBossEggmanMap2Parameter != null)
		{
			ObjBossEggmanMap2 component = GetComponent<ObjBossEggmanMap2>();
			if ((bool)component)
			{
				component.SetObjBossEggmanMap2Parameter(objBossEggmanMap2Parameter);
			}
		}
	}

	public override SpawnableParameter GetParameter()
	{
		return m_parameter;
	}
}
