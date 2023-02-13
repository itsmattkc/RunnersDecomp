using UnityEngine;

public class MotorShot : MonoBehaviour
{
	private enum State
	{
		Idle,
		Shot,
		After
	}

	public class ShotParam
	{
		public GameObject m_obj;

		public float m_gravity;

		public float m_rot_speed;

		public float m_rot_downspeed;

		public Vector3 m_rot_angle = Vector3.zero;

		public Quaternion m_shot_rotation = Quaternion.identity;

		public float m_shot_time;

		public float m_shot_speed;

		public float m_shot_downspeed;

		public bool m_bound;

		public float m_bound_pos_y;

		public float m_bound_add_y;

		public float m_bound_down_x;

		public float m_bound_down_y;

		public float m_after_speed;

		public float m_after_add_x;

		public Vector3 m_after_up = Vector3.zero;

		public Vector3 m_after_forward = Vector3.zero;
	}

	private float m_time;

	private float m_rot_speed;

	private float m_add_x;

	private float m_add_y;

	private float m_shot_speed;

	private bool m_jump;

	private bool m_bound;

	private State m_state;

	private ShotParam m_param;

	public void Setup(ShotParam param)
	{
		m_param = param;
		m_time = 0f;
		m_rot_speed = param.m_rot_speed;
		m_add_x = param.m_after_add_x;
		m_add_y = 0f;
		m_shot_speed = param.m_shot_speed;
		m_jump = false;
		m_bound = false;
		if (m_param.m_shot_time > 0f)
		{
			m_state = State.Shot;
		}
		else
		{
			m_state = State.After;
		}
	}

	public void SetEnd()
	{
		m_param = null;
		m_state = State.Idle;
	}

	private void Update()
	{
		if (m_param != null)
		{
			float deltaTime = Time.deltaTime;
			switch (m_state)
			{
			case State.Shot:
				UpdateShot(deltaTime, m_param.m_obj);
				UpdateRot(deltaTime, m_param.m_obj);
				break;
			case State.After:
				UpdateAfter(deltaTime, m_param.m_obj);
				UpdateRot(deltaTime, m_param.m_obj);
				break;
			}
		}
	}

	private void UpdateShot(float delta, GameObject obj)
	{
		if (!obj)
		{
			return;
		}
		m_time += delta;
		if (m_time > m_param.m_shot_time)
		{
			m_time = 0f;
			m_jump = false;
			m_bound = false;
			m_state = State.After;
			return;
		}
		Vector3 a = m_param.m_shot_rotation * Vector3.up * m_shot_speed;
		Vector3 b = a * delta;
		Vector3 vector = obj.transform.position + b;
		float num = m_shot_speed * delta * m_param.m_shot_downspeed;
		m_shot_speed -= num;
		if (num < 0f)
		{
			m_shot_speed = 0f;
			m_param.m_shot_time = 0f;
		}
		if (m_param.m_bound)
		{
			vector = SetBound(delta, vector);
		}
		obj.transform.position = vector;
	}

	private void UpdateAfter(float delta, GameObject obj)
	{
		if (!obj)
		{
			return;
		}
		float num = delta * m_param.m_after_speed;
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
		Vector3 vector = obj.transform.position + m_param.m_after_up * num3 + m_param.m_after_forward * num2;
		if (m_param.m_bound)
		{
			vector = SetBound(delta, vector);
		}
		obj.transform.position = vector;
	}

	private Vector3 SetBound(float delta, Vector3 pos)
	{
		Vector3 result = pos;
		if (result.y < m_param.m_bound_pos_y)
		{
			result.y = m_param.m_bound_pos_y;
			if (m_param.m_bound_add_y > 0f)
			{
				if (!m_bound)
				{
					m_add_y = m_param.m_bound_add_y;
				}
				else
				{
					m_add_y = Mathf.Max(m_add_y - m_add_y * m_param.m_bound_down_y, 0f);
				}
				m_add_x = Mathf.Max(m_add_x - m_add_x * m_param.m_bound_down_x, 0f);
				m_time = 0f;
				m_bound = true;
				m_jump = true;
				if (m_state == State.Shot)
				{
					m_state = State.After;
				}
			}
			else if (m_state == State.Shot)
			{
				m_state = State.Idle;
			}
		}
		else if (m_bound)
		{
			m_add_x = Mathf.Max(m_add_x - delta * m_add_x * 0.01f, 0f);
			m_add_y = Mathf.Max(m_add_y - delta * m_add_y * 0.01f, 0f);
		}
		return result;
	}

	private void UpdateRot(float delta, GameObject obj)
	{
		if ((bool)obj)
		{
			float d = 60f * delta * m_rot_speed;
			obj.transform.rotation = Quaternion.Euler(d * m_param.m_rot_angle) * obj.transform.rotation;
			m_rot_speed = Mathf.Max(m_rot_speed - delta * m_param.m_rot_downspeed, 0f);
		}
	}
}
