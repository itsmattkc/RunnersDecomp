using Message;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/Component/MagnetControl")]
public class MagnetControl : MonoBehaviour
{
	private const float MAGNET_SPEED = 0.2f;

	private PlayerInformation m_playerInfo;

	private GameObject m_object;

	private GameObject m_target;

	private float m_time;

	private float m_waitTime;

	private float m_speed;

	private bool m_active;

	private void Start()
	{
		m_playerInfo = ObjUtil.GetPlayerInformation();
	}

	private void OnEnable()
	{
		base.enabled = true;
	}

	private void OnDisable()
	{
		Reset();
	}

	private void Update()
	{
		if (m_active && (bool)m_object)
		{
			float num = m_speed;
			if (m_waitTime > 0f)
			{
				m_waitTime -= Time.deltaTime;
				num = m_speed * 0.1f;
			}
			m_time += Time.deltaTime;
			float num2 = 0.1f - m_time * num;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			Vector3 currentVelocity = Vector3.zero;
			Vector3 target = (!(m_target != null)) ? m_playerInfo.Position : m_target.transform.position;
			m_object.transform.position = Vector3.SmoothDamp(m_object.transform.position, target, ref currentVelocity, num2);
		}
	}

	public void OnUseMagnet(MsgUseMagnet msg)
	{
		if ((bool)msg.m_obj)
		{
			m_object = msg.m_obj;
			m_target = msg.m_target;
			m_waitTime = msg.m_time;
			m_active = true;
			float num = ObjUtil.GetPlayerAddSpeed();
			if (num < 0f)
			{
				num = 0f;
			}
			m_speed = 0.2f + 0.02f * num;
		}
	}

	public void Reset()
	{
		m_speed = 0f;
		m_object = null;
		m_target = null;
		m_waitTime = 0f;
		m_time = 0f;
		m_active = false;
		base.enabled = true;
	}
}
