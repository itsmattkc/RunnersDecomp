using UnityEngine;

public class ObjBossEggmanFeverSpawner : SpawnableBehavior
{
	[SerializeField]
	private ObjBossEggmanFeverParameter m_parameter;

	public override void SetParameters(SpawnableParameter srcParameter)
	{
		ObjBossEggmanFeverParameter objBossEggmanFeverParameter = srcParameter as ObjBossEggmanFeverParameter;
		if (objBossEggmanFeverParameter != null)
		{
			ObjBossEggmanFever component = GetComponent<ObjBossEggmanFever>();
			if ((bool)component)
			{
				component.SetObjBossEggmanFeverParameter(objBossEggmanFeverParameter);
			}
		}
	}

	public override SpawnableParameter GetParameter()
	{
		return m_parameter;
	}
}
