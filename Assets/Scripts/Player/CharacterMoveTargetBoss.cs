using Message;
using UnityEngine;

namespace Player
{
	public class CharacterMoveTargetBoss : CharacterMoveBase
	{
		private GameObject m_bossObject;

		private float m_speed;

		private bool m_rotateVelocityDir;

		private bool m_targetNotFound;

		private bool m_onlyHorizon;

		private bool m_reachTarget;

		public override void Enter(CharacterMovement context)
		{
			m_bossObject = null;
			m_rotateVelocityDir = false;
			m_targetNotFound = false;
			m_onlyHorizon = false;
			m_reachTarget = false;
		}

		public override void Leave(CharacterMovement context)
		{
			m_bossObject = null;
		}

		public override void Step(CharacterMovement context, float deltaTime)
		{
			bool flag = false;
			Vector3 position = context.transform.position;
			if (m_bossObject != null)
			{
				MsgBossInfo msgBossInfo = new MsgBossInfo();
				m_bossObject.SendMessage("OnMsgBossInfo", msgBossInfo);
				if (msgBossInfo.m_succeed)
				{
					position = m_bossObject.transform.position;
					flag = true;
				}
			}
			m_targetNotFound = !flag;
			if (m_onlyHorizon)
			{
				Vector3 lhs = position - context.transform.position;
				Vector3 vector = -context.GetGravityDir();
				Vector3 vector2 = Vector3.Dot(lhs, vector) * vector;
				position -= vector2;
			}
			float sqrMagnitude = (context.transform.position - position).sqrMagnitude;
			float num = m_speed * deltaTime;
			if (sqrMagnitude < num * num)
			{
				context.transform.position = position;
				m_reachTarget = true;
				return;
			}
			Vector3 normalized = (position - context.transform.position).normalized;
			context.transform.position += normalized * m_speed * deltaTime;
			if (m_rotateVelocityDir)
			{
				Vector3 front = normalized;
				Vector3 up = Vector3.Cross(normalized, CharacterDefs.BaseRightTangent);
				context.SetLookRotation(front, up);
			}
		}

		public void SetTarget(GameObject targetObject)
		{
			m_bossObject = targetObject;
		}

		public void SetSpeed(float speed)
		{
			m_speed = speed;
		}

		public bool IsTargetNotFound()
		{
			return m_targetNotFound;
		}

		public void SetRotateVelocityDir(bool value)
		{
			m_rotateVelocityDir = value;
		}

		public void SetOnlyHorizon(bool value)
		{
			m_onlyHorizon = value;
		}

		public bool DoesReachTarget()
		{
			return m_reachTarget;
		}
	}
}
