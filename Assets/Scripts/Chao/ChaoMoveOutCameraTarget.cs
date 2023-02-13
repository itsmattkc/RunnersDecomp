using App;
using UnityEngine;

namespace Chao
{
	public class ChaoMoveOutCameraTarget : ChaoMoveBase
	{
		private Vector3 m_screenOffsetRate;

		private Vector3 m_targetPos;

		private GameObject m_cameraObject;

		private float m_speedRate;

		public override void Enter(ChaoMovement context)
		{
			m_cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
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
				Vector3 vector = m_targetPos - context.Position;
				Vector3 normalized = vector.normalized;
				float num = m_speedRate * context.PlayerInfo.DefaultSpeed;
				context.Velocity = normalized * num;
				Vector3 position = context.Position;
				if (vector.sqrMagnitude < Math.Sqr(num * deltaTime))
				{
					position = m_targetPos;
				}
				else
				{
					position += context.Velocity * deltaTime;
				}
				context.Position = position;
			}
		}

		public void SetParameter(Vector3 screenOffsetRate, float speedRate)
		{
			m_screenOffsetRate = screenOffsetRate;
			m_speedRate = speedRate;
		}

		private bool UpdateTargetPosition(ChaoMovement context)
		{
			if (m_cameraObject == null)
			{
				return false;
			}
			Camera component = m_cameraObject.GetComponent<Camera>();
			if (component == null)
			{
				return false;
			}
			Vector3 position = component.WorldToScreenPoint(context.Position);
			position.x = -150f;
			position.y = component.pixelHeight * m_screenOffsetRate.y;
			m_targetPos = component.ScreenToWorldPoint(position);
			return true;
		}

		private bool IsOffscreen(Vector3 pos)
		{
			Vector3 vector = Camera.main.WorldToScreenPoint(pos);
			if (vector.x < 0f)
			{
				return true;
			}
			return false;
		}
	}
}
