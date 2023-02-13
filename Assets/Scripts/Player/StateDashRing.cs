using Message;
using UnityEngine;

namespace Player
{
	public class StateDashRing : FSMState<CharacterState>
	{
		private const float lerpDelta = 0.5f;

		private float m_outOfControlTime;

		private float m_lerpRotate;

		private bool m_isFalling;

		private float m_speed;

		public override void Enter(CharacterState context)
		{
			context.ChangeMovement(MOVESTATE_ID.Air);
			StateUtil.SetAirMovementToRotateGround(context, false);
			m_outOfControlTime = 0f;
			m_lerpRotate = 0f;
			Vector3 velocity = Vector3.zero;
			JumpSpringParameter enteringParameter = context.GetEnteringParameter<JumpSpringParameter>();
			if (enteringParameter != null)
			{
				Vector3 up = enteringParameter.m_rotation * Vector3.up;
				context.Movement.ResetPosition(enteringParameter.m_position);
				StateUtil.SetRotation(context, up);
				m_outOfControlTime = enteringParameter.m_outOfControlTime;
				m_speed = enteringParameter.m_firstSpeed;
				velocity = enteringParameter.m_rotation * Vector3.up * m_speed;
			}
			context.Movement.Velocity = velocity;
			context.GetAnimator().CrossFade("DashRing", 0.1f);
			m_isFalling = false;
			context.OnAttack(AttackPower.PlayerPower, DefensePower.PlayerPower);
			StateUtil.ThroughBreakable(context, true);
			context.ClearAirAction();
			StateUtil.SetDashRingMagnet(context, true);
		}

		public override void Leave(CharacterState context)
		{
			context.OffAttack();
			StateUtil.ThroughBreakable(context, false);
			StateUtil.SetDashRingMagnet(context, false);
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			m_outOfControlTime -= deltaTime;
			if (m_outOfControlTime < 0f)
			{
				if (!m_isFalling)
				{
					StateUtil.ThroughBreakable(context, false);
					context.GetAnimator().CrossFade("Fall", 0.3f);
				}
				m_isFalling = true;
				Vector3 gravityDir = context.Movement.GetGravityDir();
				context.Movement.Velocity += context.Movement.GetGravity() * deltaTime;
				if (m_lerpRotate < 1f)
				{
					m_lerpRotate = Mathf.Min(m_lerpRotate + 0.5f * deltaTime, 1f);
					Vector3 up;
					if (m_lerpRotate < 1f)
					{
						up = Vector3.Lerp(context.Movement.GetUpDir(), -gravityDir, m_lerpRotate);
					}
					else
					{
						up = -gravityDir;
						m_isFalling = true;
					}
					MovementUtil.RotateByCollision(context.transform, context.GetComponent<CapsuleCollider>(), up);
				}
				if (context.m_input.IsTouched() && StateUtil.CheckAndChangeStateToAirAttack(context, true, true))
				{
					return;
				}
			}
			if (context.Movement.GetVertVelocityScalar() <= 0f && context.Movement.IsOnGround())
			{
				StateUtil.NowLanding(context, true);
				context.Movement.Velocity = context.transform.forward * m_speed;
				context.ChangeState(STATE_ID.Run);
			}
		}

		public override bool DispatchMessage(CharacterState context, int messageId, MessageBase msg)
		{
			if (StateUtil.ChangeAfterSpinattack(context, messageId, msg))
			{
				return true;
			}
			return false;
		}
	}
}
