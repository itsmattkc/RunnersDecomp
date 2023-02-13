using UnityEngine;

namespace Player
{
	public class CharacterMoveTarget : CharacterMoveBase
	{
		private Vector3 m_targetPosition;

		private float m_speed;

		private bool m_rotateVelocityDir;

		private bool m_reachTarget;

		public override void Enter(CharacterMovement context)
		{
			m_rotateVelocityDir = false;
			m_reachTarget = false;
		}

		public override void Leave(CharacterMovement context)
		{
		}

		public override void Step(CharacterMovement context, float deltaTime)
		{
			float sqrMagnitude = (context.transform.position - m_targetPosition).sqrMagnitude;
			float num = m_speed * deltaTime;
			if (sqrMagnitude < num * num)
			{
				context.transform.position = m_targetPosition;
				m_reachTarget = true;
				return;
			}
			Vector3 normalized = (m_targetPosition - context.transform.position).normalized;
			context.transform.position += normalized * m_speed * deltaTime;
			if (m_rotateVelocityDir)
			{
				Vector3 front = normalized;
				Vector3 up = Vector3.Cross(normalized, CharacterDefs.BaseRightTangent);
				context.SetLookRotation(front, up);
			}
		}

		public void SetTarget(CharacterMovement context, Vector3 position, Quaternion rotation, float time)
		{
			m_targetPosition = position;
			m_speed = Vector3.Distance(m_targetPosition, context.transform.position) / time;
		}

		public void SetTargetAndSpeed(CharacterMovement context, Vector3 position, Quaternion rotation, float speed)
		{
			m_targetPosition = position;
			m_speed = speed;
		}

		public void SetRotateVelocityDir(bool value)
		{
			m_rotateVelocityDir = value;
		}

		public bool DoesReachTarget()
		{
			return m_reachTarget;
		}
	}
}
