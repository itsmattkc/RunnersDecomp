using System;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/Component/MotorSwing")]
public class MotorSwing : MonoBehaviour
{
	private float m_moveSpeed;

	private float m_moveDistanceX;

	private float m_moveDistanceY;

	private Vector3 m_center_position = Vector3.zero;

	private Vector3 m_angle_x = Vector3.zero;

	private float m_time;

	private void Start()
	{
		m_center_position = base.transform.position;
	}

	private void Update()
	{
		if (m_moveSpeed > 0f)
		{
			m_time += Time.deltaTime;
			float num = Mathf.Sin((float)Math.PI * 2f * m_time * m_moveSpeed);
			float d = m_moveDistanceX * num;
			float d2 = m_moveDistanceY * num;
			Vector3 b = base.transform.up * d2 + m_angle_x * d;
			base.transform.position = m_center_position + b;
		}
	}

	public void SetParam(float speed, float x, float y, Vector3 agl)
	{
		m_moveSpeed = speed;
		m_moveDistanceX = x;
		m_moveDistanceY = y;
		m_angle_x = agl;
	}
}
