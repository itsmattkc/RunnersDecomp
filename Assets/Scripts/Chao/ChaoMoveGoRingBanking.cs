using UnityEngine;

namespace Chao
{
	public class ChaoMoveGoRingBanking : ChaoMoveBase
	{
		private const float ScreenOffsetWidth = 0.85f;

		private const float UpperOffsetFromHud = 1.5f;

		private const float SpeedRate = 0.6f;

		private GameObject m_cameraObject;

		private float m_posZ;

		private Vector3 m_targetScreenPos = Vector3.zero;

		private Vector3 m_currentScreenPos = Vector3.zero;

		private float m_distance;

		public override void Enter(ChaoMovement context)
		{
			m_cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
			context.Velocity = context.HorzVelocity;
			Vector3 position = context.Position;
			m_posZ = position.z;
			if (!(m_cameraObject != null))
			{
				return;
			}
			Camera component = m_cameraObject.GetComponent<Camera>();
			if (component != null)
			{
				Vector3 vector = component.WorldToScreenPoint(context.Position);
				if (vector.x < 0f)
				{
					vector.x = -0.5f;
					context.Position = component.ScreenToWorldPoint(vector);
				}
				m_currentScreenPos = vector;
				vector.y = component.pixelHeight;
				vector.x = component.pixelWidth * 0.85f;
				Vector3 position2 = component.ScreenToWorldPoint(vector);
				position2 += ChaoMovement.VertDir * 1.5f;
				m_targetScreenPos = component.WorldToScreenPoint(position2);
				m_targetScreenPos.z = m_currentScreenPos.z;
				m_distance = Vector3.Distance(m_targetScreenPos, m_currentScreenPos);
			}
		}

		public override void Leave(ChaoMovement context)
		{
		}

		public override void Step(ChaoMovement context, float deltaTime)
		{
			if (context.PlayerInfo == null || !context.IsPlyayerMoved)
			{
				return;
			}
			m_currentScreenPos = Vector3.MoveTowards(m_currentScreenPos, m_targetScreenPos, m_distance * 0.6f * Time.deltaTime);
			if (m_cameraObject != null)
			{
				Camera component = m_cameraObject.GetComponent<Camera>();
				if (component != null)
				{
					Vector3 position = component.ScreenToWorldPoint(m_currentScreenPos);
					position.z = m_posZ;
					context.Position = position;
				}
			}
		}
	}
}
