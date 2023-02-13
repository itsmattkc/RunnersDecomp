using System;
using UnityEngine;

public class MultiSetParaloopCircleSpawner : SpawnableBehavior
{
	[SerializeField]
	private MultiSetParaloopCircleParameter m_parameter;

	public override void SetParameters(SpawnableParameter srcParameter)
	{
		MultiSetParaloopCircleParameter multiSetParaloopCircleParameter = srcParameter as MultiSetParaloopCircleParameter;
		if (multiSetParaloopCircleParameter == null)
		{
			return;
		}
		m_parameter = multiSetParaloopCircleParameter;
		GameObject @object = multiSetParaloopCircleParameter.m_object;
		MultiSetParaloopCircle component = GetComponent<MultiSetParaloopCircle>();
		if (!(@object != null) || !(component != null))
		{
			return;
		}
		if (!ObjUtil.IsUseTemporarySet())
		{
			BoxCollider component2 = GetComponent<BoxCollider>();
			if ((bool)component2)
			{
				component2.size = multiSetParaloopCircleParameter.GetSize();
				component2.center = multiSetParaloopCircleParameter.GetCenter();
			}
		}
		component.Setup();
		int count = multiSetParaloopCircleParameter.m_count;
		float radius = multiSetParaloopCircleParameter.m_radius;
		float num = 360f / (float)count;
		for (int i = 0; i < count; i++)
		{
			float f = (float)Math.PI / 180f * (num * (float)i);
			float x = radius * Mathf.Cos(f);
			float y = radius * Mathf.Sin(f);
			Vector3 b = new Vector3(x, y, 0f);
			Vector3 pos = base.transform.position + b;
			component.AddObject(@object, pos, Quaternion.identity);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, m_parameter.m_radius);
		BoxCollider component = GetComponent<BoxCollider>();
		if ((bool)component)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(base.transform.position + component.center, component.size);
		}
		if (!ObjUtil.IsUseTemporarySet())
		{
			return;
		}
		GameObject @object = m_parameter.m_object;
		if (!(@object != null))
		{
			return;
		}
		SphereCollider component2 = @object.GetComponent<SphereCollider>();
		BoxCollider component3 = @object.GetComponent<BoxCollider>();
		int count = m_parameter.m_count;
		float radius = m_parameter.m_radius;
		float num = 360f / (float)count;
		for (int i = 0; i < count; i++)
		{
			float f = (float)Math.PI / 180f * (num * (float)i);
			float x = radius * Mathf.Cos(f);
			float y = radius * Mathf.Sin(f);
			Vector3 b = new Vector3(x, y, 0f);
			Vector3 a = base.transform.position + b;
			if (component2 != null)
			{
				Gizmos.DrawWireSphere(a + component2.center, component2.radius);
			}
			else if (component3 != null)
			{
				Gizmos.DrawWireCube(a + component3.center, component3.size);
			}
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
