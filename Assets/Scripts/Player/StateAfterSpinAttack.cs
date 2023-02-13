using Message;
using UnityEngine;

namespace Player
{
	public class StateAfterSpinAttack : FSMState<CharacterState>
	{
		private float m_jumpForce;

		private float m_speed;

		private CharacterLoopEffect m_effect;

		public override void Enter(CharacterState context)
		{
			StateUtil.SetRotationOnGravityUp(context);
			context.ChangeMovement(MOVESTATE_ID.Air);
			StateUtil.SetAirMovementToRotateGround(context, true);
			m_jumpForce = context.Parameter.m_spinAttackForce;
			m_speed = Mathf.Max(context.Movement.GetForwardVelocityScalar(), 0f);
			Vector3 a = -context.Movement.GetGravityDir();
			Vector3 forward = context.transform.forward;
			context.Movement.Velocity = forward * m_speed + m_jumpForce * a;
			context.GetAnimator().CrossFade("Jump", 0.05f);
			context.OnAttack(AttackPower.PlayerSpin, DefensePower.PlayerSpin);
			StateUtil.SetAttackAttributePowerIfPowerType(context, true);
			context.SetAirAction(1);
			m_effect = context.GetSpinAttackEffect();
			if (m_effect != null)
			{
				m_effect.SetValid(true);
			}
		}

		public override void Leave(CharacterState context)
		{
			context.OffAttack();
			if (m_effect != null)
			{
				m_effect.SetValid(false);
			}
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			Vector3 vertVelocity = context.Movement.VertVelocity;
			Vector3 vector = context.Movement.Velocity - vertVelocity;
			vertVelocity += context.Movement.GetGravity() * deltaTime;
			vector = context.Movement.GetForwardDir() * StateUtil.GetForwardSpeedAir(context, context.DefaultSpeed, deltaTime);
			context.Movement.Velocity = vector + vertVelocity;
			STATE_ID state = STATE_ID.Non;
			if (StateUtil.CheckHitWallAndGoDeadOrStumble(context, deltaTime, ref state))
			{
				context.ChangeState(state);
			}
			else if ((!context.m_input.IsTouched() || !StateUtil.CheckAndChangeStateToAirAttack(context, false, false)) && context.Movement.GetVertVelocityScalar() <= 0f && context.Movement.IsOnGround() && !StateUtil.ChangeToJumpStateIfPrecedeInputTouch(context, 0.1f, true))
			{
				StateUtil.NowLanding(context, true);
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
