using UnityEngine;

[AddComponentMenu("Scripts/Runners/Component/MotorConstant")]
public class MotorConstant : MonoBehaviour
{
	private enum State
	{
		Wait,
		Move,
		Idle
	}

	private const string m_anim_name = "move";

	private float m_moveSpeed;

	private float m_moveDistance;

	private float m_startMoveDistance;

	private State m_state;

	private float m_addDistance;

	private PlayerInformation m_playerInfo;

	private Animator m_animator;

	private Vector3 m_angle = Vector3.zero;

	private string m_move_SEName = string.Empty;

	private string m_move_SECatName = string.Empty;

	private uint m_move_SEID;

	private void Start()
	{
		m_playerInfo = ObjUtil.GetPlayerInformation();
		m_animator = GetComponentInChildren<Animator>();
	}

	private void Update()
	{
		switch (m_state)
		{
		case State.Idle:
			break;
		case State.Wait:
			UpdateWait();
			break;
		case State.Move:
			UpdateMove();
			break;
		}
	}

	private void OnDestroy()
	{
		SetMoveSE(false);
	}

	public void SetParam(float speed, float dst, float start_dst, Vector3 agl, string se_category, string se_name)
	{
		m_moveSpeed = speed;
		m_moveDistance = dst;
		m_startMoveDistance = start_dst;
		m_angle = agl;
		m_move_SECatName = se_category;
		m_move_SEName = se_name;
	}

	private void UpdateWait()
	{
		float playerDistance = GetPlayerDistance();
		if (!m_moveSpeed.Equals(0f) && m_moveDistance > 0f && playerDistance < m_startMoveDistance)
		{
			SetMoveAnimation(true);
			SetMoveSE(true);
			m_state = State.Move;
		}
	}

	private void UpdateMove()
	{
		float num = m_moveSpeed * Time.deltaTime;
		base.transform.position += m_angle * num;
		m_addDistance += Mathf.Abs(num);
		if (m_addDistance >= m_moveDistance)
		{
			SetMoveAnimation(false);
			SetMoveSE(false);
			m_state = State.Idle;
		}
	}

	private float GetPlayerDistance()
	{
		if ((bool)m_playerInfo)
		{
			Vector3 position = base.transform.position;
			return Mathf.Abs(Vector3.Distance(position, m_playerInfo.Position));
		}
		return 0f;
	}

	private void SetMoveAnimation(bool flag)
	{
		if ((bool)m_animator)
		{
			m_animator.SetBool("move", flag);
		}
	}

	private void SetMoveSE(bool flag)
	{
		if (m_move_SEName.Equals(string.Empty) || m_move_SECatName.Equals(string.Empty))
		{
			return;
		}
		if (flag)
		{
			if (m_move_SEID == 0)
			{
				m_move_SEID = (uint)ObjUtil.LightPlaySE(m_move_SEName, m_move_SECatName);
			}
		}
		else if (m_move_SEID != 0)
		{
			ObjUtil.StopSE((SoundManager.PlayId)m_move_SEID);
			m_move_SEID = 0u;
		}
	}
}
