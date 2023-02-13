using App.Utility;
using Message;
using UnityEngine;

namespace Player
{
	public class StateLaunchCannon : FSMState<CharacterState>
	{
		private enum Flags
		{
			MODEL_ON
		}

		private enum SubState
		{
			LAUNCH,
			FALL
		}

		private const float lerpDelta = 0.5f;

		private SubState m_substate;

		private float m_outOfControlTime;

		private float m_lerpRotate;

		private float m_speed;

		private float m_drawLength;

		private Vector3 m_startPosition;

		private Bitset32 m_flags;

		private GameObject m_effect;

		public override void Enter(CharacterState context)
		{
			StateUtil.ResetVelocity(context);
			context.ChangeMovement(MOVESTATE_ID.Air);
			StateUtil.SetAirMovementToRotateGround(context, false);
			m_outOfControlTime = 0f;
			m_lerpRotate = 0f;
			Vector3 velocity = Vector3.zero;
			m_startPosition = context.Position;
			m_drawLength = 0f;
			CannonLaunchParameter enteringParameter = context.GetEnteringParameter<CannonLaunchParameter>();
			if (enteringParameter != null)
			{
				Vector3 up = enteringParameter.m_rotation * Vector3.up;
				context.Movement.ResetPosition(enteringParameter.m_position);
				StateUtil.SetRotation(context, up);
				m_outOfControlTime = enteringParameter.m_outOfControlTime;
				m_speed = enteringParameter.m_firstSpeed;
				velocity = enteringParameter.m_rotation * Vector3.up * m_speed;
				m_drawLength = enteringParameter.m_height;
			}
			context.Movement.Velocity = velocity;
			context.GetAnimator().Play("Cannon");
			m_flags.Reset();
			context.OnAttack(AttackPower.PlayerPower, DefensePower.PlayerPower);
			context.OnAttackAttribute(AttackAttribute.Power);
			context.SetModelNotDraw(true);
			StateUtil.SetNotDrawItemEffect(context, true);
			StateUtil.ThroughBreakable(context, true);
			m_substate = SubState.LAUNCH;
			context.SetNotCharaChange(true);
			context.SetNotUseItem(true);
			StateUtil.SetCannonMagnet(context, true);
		}

		public override void Leave(CharacterState context)
		{
			context.OffAttack();
			context.SetModelNotDraw(false);
			StateUtil.SetNotDrawItemEffect(context, false);
			context.SetNotCharaChange(false);
			context.SetNotUseItem(false);
			StateUtil.ThroughBreakable(context, false);
			StateUtil.SetCannonMagnet(context, false);
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			m_outOfControlTime -= deltaTime;
			Vector3 position = context.Position;
			switch (m_substate)
			{
			case SubState.LAUNCH:
				if ((position - m_startPosition).sqrMagnitude > m_drawLength * m_drawLength)
				{
					context.SetModelNotDraw(false);
					StateUtil.SetNotDrawItemEffect(context, false);
					m_flags.Set(0, true);
				}
				if (m_outOfControlTime < 0f)
				{
					StateUtil.ThroughBreakable(context, false);
					context.GetAnimator().CrossFade("Fall", 0.5f);
					context.OffAttack();
					context.SetNotCharaChange(false);
					context.SetNotUseItem(false);
					m_substate = SubState.FALL;
				}
				break;
			case SubState.FALL:
			{
				Vector3 gravityDir = context.Movement.GetGravityDir();
				Vector3 a = context.Movement.GetForwardDir() * StateUtil.GetForwardSpeedAir(context, context.DefaultSpeed, deltaTime);
				Vector3 vertVelocity = context.Movement.VertVelocity;
				context.Movement.Velocity = a + vertVelocity + context.Movement.GetGravity() * deltaTime;
				if (m_lerpRotate < 1f)
				{
					m_lerpRotate = Mathf.Min(m_lerpRotate + 0.5f * deltaTime, 1f);
					MovementUtil.RotateByCollision(up: (!(m_lerpRotate < 1f)) ? (-gravityDir) : Vector3.Lerp(context.Movement.GetUpDir(), -gravityDir, m_lerpRotate), transform: context.transform, collider: context.GetComponent<CapsuleCollider>());
				}
				if (context.m_input.IsTouched() && StateUtil.CheckAndChangeStateToAirAttack(context, true, false))
				{
					return;
				}
				break;
			}
			}
			if (context.Movement.IsOnGround())
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
