using UnityEngine;

public class ObjEnemyConstantSpawner : SpawnableBehavior
{
	[SerializeField]
	private ObjEnemyConstantParameter m_parameter;

	public override void SetParameters(SpawnableParameter srcParameter)
	{
		ObjEnemyConstantParameter objEnemyConstantParameter = srcParameter as ObjEnemyConstantParameter;
		if (objEnemyConstantParameter != null)
		{
			ObjEnemyConstant component = GetComponent<ObjEnemyConstant>();
			if ((bool)component)
			{
				component.SetObjEnemyConstantParameter(objEnemyConstantParameter);
			}
		}
	}

	public override SpawnableParameter GetParameter()
	{
		return m_parameter;
	}

	private void OnDrawGizmos()
	{
		if (m_parameter.moveDistance > 0f && !base.transform.name.Contains("ObjEnmValkyne"))
		{
			Vector3 position = base.transform.position;
			Vector3 a = base.transform.forward;
			Gizmos.color = Color.green;
			if (base.transform.name.Contains("ObjEnmGanigani"))
			{
				a = base.transform.right;
			}
			Gizmos.DrawLine(position, position + a * m_parameter.moveDistance);
		}
	}
}
