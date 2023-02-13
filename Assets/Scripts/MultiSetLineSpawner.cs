using UnityEngine;

public class MultiSetLineSpawner : SpawnableBehavior
{
	private static Vector3 LINE_0 = new Vector3(0f, 1f, 0f);

	private static Vector3 LINE_1 = new Vector3(1f, 0f, 0f);

	[SerializeField]
	private MultiSetLineParameter m_parameter;

	public override void SetParameters(SpawnableParameter srcParameter)
	{
		MultiSetLineParameter multiSetLineParameter = srcParameter as MultiSetLineParameter;
		if (multiSetLineParameter == null)
		{
			return;
		}
		m_parameter = multiSetLineParameter;
		GameObject @object = multiSetLineParameter.m_object;
		MultiSetLine component = GetComponent<MultiSetLine>();
		if (@object != null && component != null)
		{
			component.Setup();
			int count = multiSetLineParameter.m_count;
			float distance = multiSetLineParameter.m_distance;
			int type = multiSetLineParameter.m_type;
			Vector3 angle = GetAngle(type);
			Vector3 position = base.transform.position;
			for (int i = 0; i < count; i++)
			{
				Vector3 pos = position + angle * (distance * (float)i);
				component.AddObject(@object, pos, Quaternion.identity);
			}
		}
	}

	private void OnDrawGizmos()
	{
		Vector3 position = base.transform.position;
		Gizmos.color = Color.green;
		float d = (float)m_parameter.m_count - 1f;
		Vector3 vector = position + GetAngle(m_parameter.m_type) * d * m_parameter.m_distance;
		Gizmos.DrawLine(position, vector);
		Vector3 b = (m_parameter.m_type != 0) ? LINE_1 : LINE_0;
		Gizmos.DrawLine(position + b, vector + b);
		Gizmos.DrawLine(position, position + b);
		Gizmos.DrawLine(vector, vector + b);
		if (!ObjUtil.IsUseTemporarySet())
		{
			return;
		}
		GameObject @object = m_parameter.m_object;
		if (!(@object != null))
		{
			return;
		}
		SphereCollider component = @object.GetComponent<SphereCollider>();
		BoxCollider component2 = @object.GetComponent<BoxCollider>();
		int count = m_parameter.m_count;
		float distance = m_parameter.m_distance;
		int type = m_parameter.m_type;
		Vector3 angle = GetAngle(type);
		Vector3 position2 = base.transform.position;
		for (int i = 0; i < count; i++)
		{
			Vector3 a = position2 + angle * (distance * (float)i);
			if (component != null)
			{
				Gizmos.DrawWireSphere(a + component.center, component.radius);
			}
			else if (component2 != null)
			{
				Gizmos.DrawWireCube(a + component2.center, component2.size);
			}
		}
	}

	private Vector3 GetAngle(int type)
	{
		if (type == 0)
		{
			return base.transform.right;
		}
		return -base.transform.up;
	}

	public override SpawnableParameter GetParameter()
	{
		return m_parameter;
	}
}
