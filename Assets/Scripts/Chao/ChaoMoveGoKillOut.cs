using UnityEngine;

namespace Chao
{
	public class ChaoMoveGoKillOut : ChaoMoveBase
	{
		public enum Mode
		{
			Up,
			Forward
		}

		private const float UpVelocity = 4.5f;

		private const float SpeedRate = 5f;

		private const float UpScrenRate = 0.8f;

		private Mode m_mode;

		private Vector3 m_screenRate;

		private GameObject m_cameraObject;

		public override void Enter(ChaoMovement context)
		{
			m_mode = Mode.Up;
			m_cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
			UpdateScreenPoint(context);
		}

		public override void Leave(ChaoMovement context)
		{
			m_cameraObject = null;
		}

		public override void Step(ChaoMovement context, float deltaTime)
		{
			UpdateScreenPoint(context);
			if (!(context.PlayerInfo == null) && context.IsPlyayerMoved)
			{
				switch (m_mode)
				{
				case Mode.Up:
					MoveUp(context, deltaTime);
					break;
				case Mode.Forward:
				{
					float speed = context.PlayerInfo.DefaultSpeed * 5f;
					MoveForward(context, speed, deltaTime);
					break;
				}
				}
			}
		}

		public void ChangeMode(Mode mode)
		{
			m_mode = mode;
		}

		private void MoveUp(ChaoMovement context, float deltaTime)
		{
			if (!(context.PlayerInfo == null))
			{
				Vector3 a = context.PlayerPosition + context.OffsetPosition;
				float num = Vector3.Dot(a - context.Position, ChaoMovement.HorzDir);
				Vector3 a2 = (!(num < 0f)) ? (ChaoMovement.HorzDir * context.PlayerInfo.DefaultSpeed) : (ChaoMovement.HorzDir * context.PlayerInfo.DefaultSpeed * 0.5f);
				Vector3 b = (!(m_screenRate.y < 0.8f)) ? Vector3.zero : (ChaoMovement.VertDir * 4.5f);
				context.Velocity = a2 + b;
				context.Position += context.Velocity * deltaTime;
			}
		}

		private void MoveForward(ChaoMovement context, float speed, float deltaTime)
		{
			if (!(context.PlayerInfo == null))
			{
				context.Velocity = ChaoMovement.HorzDir * context.PlayerInfo.DefaultSpeed * 5f;
				context.Position += context.Velocity * deltaTime;
			}
		}

		private bool UpdateScreenPoint(ChaoMovement context)
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
			Vector3 vector = component.WorldToScreenPoint(context.Position);
			m_screenRate.x = vector.x / component.pixelWidth;
			m_screenRate.y = vector.y / component.pixelHeight;
			return true;
		}
	}
}
