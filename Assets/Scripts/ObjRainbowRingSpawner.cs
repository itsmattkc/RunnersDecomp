using UnityEngine;

public class ObjRainbowRingSpawner : SpawnableBehavior
{
	[SerializeField]
	private ObjRainbowRingParameter m_parameter;

	public override void SetParameters(SpawnableParameter srcParameter)
	{
		ObjRainbowRingParameter objRainbowRingParameter = srcParameter as ObjRainbowRingParameter;
		if (objRainbowRingParameter != null)
		{
			ObjRainbowRing component = GetComponent<ObjRainbowRing>();
			if ((bool)component)
			{
				component.SetObjRainbowRingParameter(objRainbowRingParameter);
			}
		}
	}

	public override SpawnableParameter GetParameter()
	{
		return m_parameter;
	}
}
