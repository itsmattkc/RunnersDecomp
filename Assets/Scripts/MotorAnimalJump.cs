using UnityEngine;

public class MotorAnimalJump : MonoBehaviour
{
	public struct JumpParam
	{
		public GameObject m_obj;

		public float m_speed;

		public float m_gravity;

		public float m_add_x;

		public Vector3 m_up;

		public Vector3 m_forward;

		public bool m_bound;

		public float m_bound_add_y;

		public float m_bound_down_x;

		public float m_bound_down_y;
	}

	private const float HitLength = 1f;

	private float m_time;

	private bool m_jump = true;

	private float m_add_x;

	private float m_add_y;

	private bool m_setup;

	private JumpParam m_param;

	public void Setup(ref JumpParam param)
	{
		m_param = param;
		m_add_x = param.m_add_x;
		m_add_y = m_param.m_bound_add_y;
		m_jump = true;
		m_time = 0f;
		m_setup = true;
	}

	public void SetEnd()
	{
		m_setup = false;
	}

	private void Update()
	{
		if (m_setup)
		{
			UpdateThrow(Time.deltaTime, m_param.m_obj);
		}
	}

	private void UpdateThrow(float delta, GameObject obj)
	{
		if (!obj)
		{
			return;
		}
		float num = delta * m_param.m_speed;
		float num2 = 0f;
		float num3 = 0f;
		if (m_jump)
		{
			m_time += num;
			num2 = num * m_add_x;
			float num4 = m_add_y - m_time * (0f - m_param.m_gravity) * 0.15f;
			if (num4 < 0f)
			{
				num4 = 0f;
				m_time = 0f;
				m_jump = false;
			}
			num3 = delta * num4 * 3f;
		}
		else
		{
			m_time += num;
			float num5 = m_add_x - m_time * 0.1f;
			if (num5 < 0f)
			{
				num5 = 0f;
			}
			num2 = delta * num5 * 3f;
			num3 = m_time * m_param.m_gravity * delta;
		}
		Vector3 vector = obj.transform.position + m_param.m_up * num3 + m_param.m_forward * num2;
		if (m_param.m_bound)
		{
			Vector3 hit_pos = Vector3.zero;
			if (ObjUtil.CheckGroundHit(vector, m_param.m_up, 1f, 1f, out hit_pos))
			{
				vector.y = hit_pos.y;
				m_add_y = Mathf.Max(m_add_y - m_add_y * m_param.m_bound_down_y, 0f);
				m_add_x = Mathf.Max(m_add_x - m_add_x * m_param.m_bound_down_x, 0f);
				m_jump = true;
				m_time = 0f;
			}
			else
			{
				m_add_x = Mathf.Max(m_add_x - delta * m_add_x * 0.01f, 0f);
				m_add_y = Mathf.Max(m_add_y - delta * m_add_y * 0.01f, 0f);
			}
		}
		obj.transform.position = vector;
	}
}
