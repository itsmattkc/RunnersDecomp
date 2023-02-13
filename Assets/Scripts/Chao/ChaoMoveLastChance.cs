using UnityEngine;

namespace Chao
{
	public class ChaoMoveLastChance : ChaoMoveBase
	{
		private float m_velocity = 5f;

		private Vector3 m_distancePos = new Vector3(0f, 0f, 0f);

		private Vector3 m_prePositon = new Vector3(0f, 0f, 0f);

		private Vector3 m_stayPosition = new Vector3(0f, 0f, 0f);

		public override void Enter(ChaoMovement context)
		{
			m_distancePos = context.TargetPosition - context.Position;
			m_velocity = context.TargetAccessSpeed;
			m_prePositon = context.Position;
			m_stayPosition = context.Position - context.Hovering;
		}

		public override void Leave(ChaoMovement context)
		{
		}

		public override void Step(ChaoMovement context, float deltaTime)
		{
			Camera main = Camera.main;
			if (deltaTime == 0f || !(main != null))
			{
				return;
			}
			float movingDistance = m_velocity * deltaTime;
			Vector3 vector = main.WorldToViewportPoint(context.TargetPosition);
			if ((double)vector.y < -0.05)
			{
				m_distancePos.x = CalcAccessDistance(movingDistance, m_distancePos.x);
				Vector3 targetPosition = context.TargetPosition;
				m_stayPosition.x = targetPosition.x - m_distancePos.x;
				context.Position = m_stayPosition + context.Hovering;
			}
			else
			{
				m_distancePos.x = CalcAccessDistance(movingDistance, m_distancePos.x);
				m_distancePos.y = CalcAccessDistance(movingDistance, m_distancePos.y);
				context.Position = context.TargetPosition - m_distancePos;
				if (m_distancePos.y < 0f)
				{
					Vector3 position = context.Position;
					float num = position.y - m_prePositon.y;
					if (num > 0f)
					{
						m_distancePos.y += num;
						if (m_distancePos.y > 0f)
						{
							m_distancePos.y = 0f;
						}
						context.Position = context.TargetPosition - m_distancePos;
					}
				}
				else if (m_distancePos.y > 0f)
				{
					Vector3 position2 = context.Position;
					float num2 = position2.y - m_prePositon.y;
					if (num2 < 0f)
					{
						m_distancePos.y -= num2;
						if (m_distancePos.y < 0f)
						{
							m_distancePos.y = 0f;
						}
						context.Position = context.TargetPosition - m_distancePos;
					}
				}
			}
			m_prePositon = context.Position;
		}

		private float CalcAccessDistance(float movingDistance, float targetDistance)
		{
			if (targetDistance != 0f)
			{
				targetDistance = ((targetDistance > 0f) ? ((!(targetDistance > movingDistance)) ? 0f : (targetDistance - movingDistance)) : ((!(targetDistance < 0f - movingDistance)) ? 0f : (targetDistance + movingDistance)));
			}
			return targetDistance;
		}
	}
}
