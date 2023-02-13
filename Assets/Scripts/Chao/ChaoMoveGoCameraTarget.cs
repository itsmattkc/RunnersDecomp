using UnityEngine;

namespace Chao
{
	public class ChaoMoveGoCameraTarget : ChaoMoveBase
	{
		private Vector3 m_screenOffsetRate = Vector3.zero;

		private Vector3 m_targetScreenPos = Vector3.zero;

		private Vector3 m_currentScreenPos = Vector3.zero;

		private GameObject m_cameraObject;

		private float m_speedRate;

		private float m_distance;

		private float m_speed;

		private float m_posZ;

		public override void Enter(ChaoMovement context)
		{
			Vector3 position = context.Position;
			m_posZ = position.z;
			m_cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
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
			}
		}

		public override void Leave(ChaoMovement context)
		{
			m_cameraObject = null;
		}

		public override void Step(ChaoMovement context, float deltaTime)
		{
			if (!(context.PlayerInfo == null) && context.IsPlyayerMoved)
			{
				UpdateTargetPosition(context);
			}
		}

		public void SetParameter(Vector3 screenOffsetRate, float speedRate)
		{
			m_screenOffsetRate = screenOffsetRate;
			m_speedRate = speedRate;
			if (m_cameraObject != null)
			{
				Camera component = m_cameraObject.GetComponent<Camera>();
				if (component != null)
				{
					m_targetScreenPos.x = component.pixelWidth * m_screenOffsetRate.x;
					m_targetScreenPos.y = component.pixelHeight * m_screenOffsetRate.y;
					m_targetScreenPos.z = m_currentScreenPos.z;
				}
			}
			m_distance = Vector3.Distance(m_targetScreenPos, m_currentScreenPos);
		}

		private void UpdateTargetPosition(ChaoMovement context)
		{
			m_currentScreenPos = Vector3.MoveTowards(m_currentScreenPos, m_targetScreenPos, m_distance * m_speedRate * Time.deltaTime);
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
