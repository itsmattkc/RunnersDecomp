using UnityEngine;

public class MotorThrow : MonoBehaviour
{
	public class ThrowParam
	{
		public GameObject m_obj;

		public float m_speed;

		public float m_gravity;

		public float m_add_x;

		public float m_add_y;

		public float m_rot_speed;

		public float m_rot_downspeed;

		public Vector3 m_up = Vector3.zero;

		public Vector3 m_forward = Vector3.zero;

		public Vector3 m_rot_angle = Vector3.zero;

		public bool m_bound;

		public float m_bound_pos_y;

		public float m_bound_add_y;

		public float m_bound_down_x;

		public float m_bound_down_y;
	}

	private float m_time;

	private float m_rot_speed;

	private bool m_jump = true;

	private float m_add_x;

	private float m_add_y;

	private bool m_bound;

	private ThrowParam m_param;

	public void Setup(ThrowParam param)
	{
		m_param = param;
		m_rot_speed = param.m_rot_speed;
		m_add_x = param.m_add_x;
		m_add_y = param.m_add_y;
		m_time = 0f;
		m_bound = false;
		m_jump = true;
	}

	public void SetEnd()
	{
		m_param = null;
	}

	private void Update()
	{
		if (m_param != null)
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
		Vector3 position = obj.transform.position + m_param.m_up * num3 + m_param.m_forward * num2;
		if (m_param.m_bound)
		{
			if (position.y < m_param.m_bound_pos_y)
			{
				position.y = m_param.m_bound_pos_y;
				if (!m_bound)
				{
					m_add_y = m_param.m_bound_add_y;
				}
				else
				{
					m_add_y = Mathf.Max(m_add_y - m_add_y * m_param.m_bound_down_y, 0f);
				}
				m_add_x = Mathf.Max(m_add_x - m_add_x * m_param.m_bound_down_x, 0f);
				m_bound = true;
				m_jump = true;
				m_time = 0f;
			}
			else if (m_bound)
			{
				m_add_x = Mathf.Max(m_add_x - delta * m_add_x * 0.01f, 0f);
				m_add_y = Mathf.Max(m_add_y - delta * m_add_y * 0.01f, 0f);
			}
		}
		obj.transform.position = position;
		if (m_rot_speed > 0f)
		{
			float d = 60f * delta * m_rot_speed;
			obj.transform.rotation = Quaternion.Euler(d * m_param.m_rot_angle) * obj.transform.rotation;
			m_rot_speed = Mathf.Max(m_rot_speed - delta * m_param.m_rot_downspeed, 0f);
		}
	}
}
