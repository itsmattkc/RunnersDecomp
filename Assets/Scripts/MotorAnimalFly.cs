using System;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/Component/MotorAnimalFly")]
public class MotorAnimalFly : MonoBehaviour
{
	private const float UP_SPEED = 2f;

	private float m_moveSpeed;

	private float m_moveDistance;

	private float m_groundDistance;

	private float m_addSpeedX;

	private Vector3 m_angleX = Vector3.zero;

	private Vector3 m_center_position = Vector3.zero;

	private float m_time;

	private bool m_hitCheck;

	private bool m_setup;

	private void Update()
	{
		if (!m_setup)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		m_time += deltaTime;
		float num = Mathf.Sin((float)Math.PI * 2f * m_time * m_moveSpeed);
		float d = m_moveDistance * num;
		float d2 = m_addSpeedX * m_time;
		base.transform.position = m_center_position + base.transform.up * d + m_angleX * d2;
		if (m_hitCheck)
		{
			Vector3 hit_pos = Vector3.zero;
			if (ObjUtil.CheckGroundHit(m_center_position, base.transform.up, 1f, m_moveDistance + m_groundDistance, out hit_pos))
			{
				m_center_position += Vector3.up * deltaTime * 2f;
			}
		}
	}

	public void SetupParam(float speed, float distance, float add_speed_x, Vector3 angle_x, float ground_distance, bool hitCheck)
	{
		m_moveSpeed = speed;
		m_moveDistance = distance;
		m_addSpeedX = add_speed_x;
		m_angleX = angle_x;
		float d = 0f;
		SphereCollider component = GetComponent<SphereCollider>();
		if (component != null)
		{
			d = component.radius;
		}
		m_center_position = base.transform.position + Vector3.up * d;
		m_groundDistance = ground_distance;
		m_hitCheck = hitCheck;
		m_setup = true;
		m_time = 0f;
	}

	public void SetEnd()
	{
		m_setup = false;
	}
}
