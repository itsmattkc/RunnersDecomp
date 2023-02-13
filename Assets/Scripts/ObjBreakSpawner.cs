using UnityEngine;

public class ObjBreakSpawner : SpawnableBehavior
{
	[SerializeField]
	private ObjBreakParameter m_parameter;

	public override void SetParameters(SpawnableParameter srcParameter)
	{
		ObjBreakParameter objBreakParameter = srcParameter as ObjBreakParameter;
		if (objBreakParameter == null)
		{
			return;
		}
		GameObject modelObject = objBreakParameter.m_modelObject;
		ObjBreak component = GetComponent<ObjBreak>();
		if (modelObject != null && component != null && !(component.ModelObject != null))
		{
			GameObject gameObject = Object.Instantiate(modelObject, base.transform.position, base.transform.rotation) as GameObject;
			if (gameObject != null)
			{
				gameObject.transform.parent = base.transform;
				component.ModelObject = gameObject;
			}
			component.SetObjName(modelObject.name);
		}
	}

	public override SpawnableParameter GetParameter()
	{
		return m_parameter;
	}
}
