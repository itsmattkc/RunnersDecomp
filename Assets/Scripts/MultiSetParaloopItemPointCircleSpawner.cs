using UnityEngine;

public class MultiSetParaloopItemPointCircleSpawner : SpawnableBehavior
{
	[SerializeField]
	private MultiSetParaloopItemPointCircleParameter m_parameter;

	public override void SetParameters(SpawnableParameter srcParameter)
	{
		MultiSetParaloopItemPointCircleParameter multiSetParaloopItemPointCircleParameter = srcParameter as MultiSetParaloopItemPointCircleParameter;
		if (multiSetParaloopItemPointCircleParameter == null)
		{
			return;
		}
		m_parameter = multiSetParaloopItemPointCircleParameter;
		GameObject @object = multiSetParaloopItemPointCircleParameter.m_object;
		MultiSetParaloopItemPointCircle component = GetComponent<MultiSetParaloopItemPointCircle>();
		if (!(@object != null) || !(component != null))
		{
			return;
		}
		if (!ObjUtil.IsUseTemporarySet())
		{
			BoxCollider component2 = GetComponent<BoxCollider>();
			if ((bool)component2)
			{
				component2.size = multiSetParaloopItemPointCircleParameter.GetSize();
				component2.center = multiSetParaloopItemPointCircleParameter.GetCenter();
			}
		}
		component.Setup();
		component.SetID(multiSetParaloopItemPointCircleParameter.m_tblID);
		component.AddObject(@object, base.transform.position, Quaternion.identity);
	}

	private void OnDrawGizmos()
	{
		BoxCollider component = GetComponent<BoxCollider>();
		if ((bool)component)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(base.transform.position + component.center, component.size);
		}
		if (ObjUtil.IsUseTemporarySet())
		{
			Gizmos.DrawWireSphere(base.transform.position, 0.5f);
		}
	}

	public override SpawnableParameter GetParameter()
	{
		return m_parameter;
	}

	public override SpawnableParameter GetParameterForExport()
	{
		BoxCollider component = GetComponent<BoxCollider>();
		if ((bool)component)
		{
			m_parameter.SetSize(component.size);
			m_parameter.SetCenter(component.center);
		}
		return m_parameter;
	}
}
