using UnityEngine;

public class ObjBossEggmanMap1Spawner : SpawnableBehavior
{
	[SerializeField]
	private ObjBossEggmanMap1Parameter m_parameter;

	public override void SetParameters(SpawnableParameter srcParameter)
	{
		ObjBossEggmanMap1Parameter objBossEggmanMap1Parameter = srcParameter as ObjBossEggmanMap1Parameter;
		if (objBossEggmanMap1Parameter != null)
		{
			ObjBossEggmanMap1 component = GetComponent<ObjBossEggmanMap1>();
			if ((bool)component)
			{
				component.SetObjBossEggmanMap1Parameter(objBossEggmanMap1Parameter);
			}
		}
	}

	public override SpawnableParameter GetParameter()
	{
		return m_parameter;
	}
}
