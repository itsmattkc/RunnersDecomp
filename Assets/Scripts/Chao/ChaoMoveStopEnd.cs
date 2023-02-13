using UnityEngine;

namespace Chao
{
	public class ChaoMoveStopEnd : ChaoMoveBase
	{
		private Vector3 m_chao_pos = new Vector3(0f, 0f, 0f);

		private float m_distance;

		public override void Enter(ChaoMovement context)
		{
			m_chao_pos = context.TargetPosition + context.Hovering + context.OffsetPosition;
			Camera main = Camera.main;
			if (main != null)
			{
				Vector3 position = main.WorldToScreenPoint(context.Position);
				position.x = 0f;
				Vector3 position2 = main.ScreenToWorldPoint(position);
				float x = position2.x;
				Vector3 position3 = context.Position;
				if (x > position3.x)
				{
					context.Position = position2;
					m_distance = m_chao_pos.x - position2.x;
				}
				else
				{
					float x2 = m_chao_pos.x;
					Vector3 position4 = context.Position;
					m_distance = x2 - position4.x;
				}
			}
			else
			{
				m_distance = 10f;
				Vector3 chao_pos = m_chao_pos;
				chao_pos.x -= m_distance;
				context.Position = chao_pos;
			}
		}

		public override void Leave(ChaoMovement context)
		{
		}

		public override void Step(ChaoMovement context, float deltaTime)
		{
			m_chao_pos = context.TargetPosition + context.Hovering + context.OffsetPosition;
			m_distance -= context.ComeInSpeed * deltaTime;
			if (m_distance < 0f)
			{
				m_distance = 0f;
				context.NextState = true;
			}
			m_chao_pos.x -= m_distance;
			context.Position = m_chao_pos;
		}
	}
}
