using UnityEngine;

namespace Chao
{
	public class ChaoMoveExit : ChaoMoveBase
	{
		private Vector3 m_chao_pos = new Vector3(0f, 0f, 0f);

		private Vector3 m_init_pos = new Vector3(0f, 0f, 0f);

		private float m_move_distance;

		public override void Enter(ChaoMovement context)
		{
			m_init_pos = context.TargetPosition;
			m_chao_pos = m_init_pos + context.Hovering + context.OffsetPosition;
			m_move_distance = 0f;
		}

		public override void Leave(ChaoMovement context)
		{
		}

		public override void Step(ChaoMovement context, float deltaTime)
		{
			if (context.NextState)
			{
				return;
			}
			m_chao_pos = m_init_pos + context.Hovering + context.OffsetPosition;
			if (context.PlayerInfo != null)
			{
				Vector3 horizonVelocity = context.PlayerInfo.HorizonVelocity;
				float x = horizonVelocity.x;
				float num = x - 2f * context.ComeInSpeed;
				m_move_distance += num * deltaTime;
				m_chao_pos.x += m_move_distance;
				if (IsOffscreen())
				{
					context.NextState = true;
				}
			}
			context.Position = m_chao_pos;
		}

		private bool IsOffscreen()
		{
			Vector3 vector = Camera.main.WorldToScreenPoint(m_chao_pos);
			if (vector.x < 0f)
			{
				return true;
			}
			return false;
		}
	}
}
