using UnityEngine;

public class ObjItemPointSpawner : SpawnableBehavior
{
	[SerializeField]
	private ObjItemPointParameter m_parameter;

	public override void SetParameters(SpawnableParameter srcParameter)
	{
		ObjItemPointParameter objItemPointParameter = srcParameter as ObjItemPointParameter;
		if (objItemPointParameter != null)
		{
			ObjItemPoint component = GetComponent<ObjItemPoint>();
			if (component != null)
			{
				component.SetID(objItemPointParameter.m_tblID);
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, 0.5f);
	}

	public override SpawnableParameter GetParameter()
	{
		return m_parameter;
	}
}
