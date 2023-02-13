using UnityEngine;

namespace Chao
{
	public class ChaoMoveAttackBoss : ChaoMoveBase
	{
		public enum Mode
		{
			Up,
			Homing,
			AfterAttack
		}

		private const float UpVelocity = 4.5f;

		private const float AfterAttackVelocity = 7f;

		private const float SpeedRate = 4f;

		private Mode m_mode;

		private GameObject m_boss;

		private Vector3 m_prevPlrPos;

		public override void Enter(ChaoMovement context)
		{
			m_mode = Mode.Up;
			m_boss = null;
			context.Velocity = Vector3.zero;
			if (context.PlayerInfo != null)
			{
				m_prevPlrPos = context.PlayerInfo.Position;
			}
		}

		public override void Leave(ChaoMovement context)
		{
		}

		public override void Step(ChaoMovement context, float deltaTime)
		{
			if (context.PlayerInfo == null)
			{
				return;
			}
			if (m_boss == null)
			{
				context.Position += context.Velocity * deltaTime;
				m_prevPlrPos = context.PlayerInfo.Position;
				return;
			}
			Vector3 position = m_boss.transform.position;
			switch (m_mode)
			{
			case Mode.Up:
				MoveUp(context, deltaTime);
				break;
			case Mode.Homing:
			{
				float speed = context.PlayerInfo.DefaultSpeed * 4f;
				MoveHoming(context, position, speed, deltaTime);
				break;
			}
			case Mode.AfterAttack:
				MoveAfterAttack(context, deltaTime);
				break;
			}
			m_prevPlrPos = context.PlayerInfo.Position;
		}

		public void SetTarget(GameObject boss)
		{
			m_boss = boss;
		}

		public void ChangeMode(Mode mode)
		{
			m_mode = mode;
		}

		private void MoveHoming(ChaoMovement context, Vector3 targetPosition, float speed, float deltaTime)
		{
			Vector3 vector = targetPosition - context.Position;
			float magnitude = vector.magnitude;
			Vector3 normalized = vector.normalized;
			if (magnitude < speed * deltaTime)
			{
				context.Position = targetPosition;
				return;
			}
			context.Velocity = normalized * speed;
			context.Position += context.Velocity * deltaTime;
		}

		private void MoveUp(ChaoMovement context, float deltaTime)
		{
			if (!(context.PlayerInfo == null) && !(Vector3.Distance(m_prevPlrPos, context.PlayerInfo.Position) < float.Epsilon))
			{
				context.Velocity = ChaoMovement.HorzDir * context.PlayerInfo.DefaultSpeed + ChaoMovement.VertDir * 4.5f;
				context.Position += context.Velocity * deltaTime;
			}
		}

		private void MoveAfterAttack(ChaoMovement context, float deltaTime)
		{
			if (!(context.PlayerInfo == null))
			{
				Vector3 position = context.PlayerInfo.Position;
				if (!(Vector3.Distance(m_prevPlrPos, position) < float.Epsilon))
				{
					Vector3 lhs = position - context.Position;
					float num = Vector3.Dot(lhs, ChaoMovement.HorzDir);
					Vector3 a = (!(num > 0f)) ? (ChaoMovement.HorzDir * context.PlayerInfo.DefaultSpeed * 0.25f) : (ChaoMovement.HorzDir * context.PlayerInfo.DefaultSpeed);
					context.Velocity = a + ChaoMovement.VertDir * 7f;
					context.Position += context.Velocity * deltaTime;
				}
			}
		}
	}
}
