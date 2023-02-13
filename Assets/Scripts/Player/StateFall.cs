using Message;
using UnityEngine;

namespace Player
{
	public class StateFall : FSMState<CharacterState>
	{
		public override void Enter(CharacterState context)
		{
			context.ChangeMovement(MOVESTATE_ID.Air);
			StateUtil.SetAirMovementToRotateGround(context, true);
			context.Movement.OffGround();
			float d = Mathf.Max(context.Movement.GetForwardVelocityScalar(), 0f);
			context.Movement.Velocity = context.transform.forward * d + context.Movement.VertVelocity;
			context.GetAnimator().CrossFade("Fall", 0.2f);
			context.OnAttack(AttackPower.PlayerStomp, DefensePower.PlayerStomp);
		}

		public override void Leave(CharacterState context)
		{
			context.OffAttack();
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			context.Movement.Velocity = context.Movement.Velocity + context.Movement.GetGravity() * deltaTime;
			if (!context.m_input.IsTouched() || !StateUtil.CheckAndChangeStateToAirAttack(context, true, false))
			{
				STATE_ID state = STATE_ID.Non;
				if (StateUtil.CheckHitWallAndGoDeadOrStumble(context, deltaTime, ref state))
				{
					context.ChangeState(state);
				}
				else if (context.Movement.IsOnGround())
				{
					StateUtil.NowLanding(context, true);
					context.ChangeState(STATE_ID.Run);
				}
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
