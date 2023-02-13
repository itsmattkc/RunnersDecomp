using UnityEngine;

public class ObjRouletteCannonSpawner : SpawnableBehavior
{
	[SerializeField]
	private ObjRouletteCannonParameter m_parameter;

	public override void SetParameters(SpawnableParameter srcParameter)
	{
		ObjRouletteCannonParameter objRouletteCannonParameter = srcParameter as ObjRouletteCannonParameter;
		if (objRouletteCannonParameter != null)
		{
			ObjRouletteCannon component = GetComponent<ObjRouletteCannon>();
			if ((bool)component)
			{
				component.SetObjRouletteCannonParameter(objRouletteCannonParameter);
			}
		}
	}

	public override SpawnableParameter GetParameter()
	{
		return m_parameter;
	}
}
