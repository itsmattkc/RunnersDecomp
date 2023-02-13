using App.Utility;
using Message;
using UnityEngine;

namespace Player
{
	public class StateGride : FSMState<CharacterState>
	{
		private enum Flag
		{
			DISABLE_GRAVITY
		}

		private float m_timer;

		private Bitset32 m_flag;

		private string m_effectname;

		private string m_attackEffectname;

		private bool DisableGravity
		{
			get
			{
				return m_flag.Test(0);
			}
			set
			{
				m_flag.Set(0, value);
			}
		}

		public override void Enter(CharacterState context)
		{
			context.ChangeMovement(MOVESTATE_ID.Air);
			context.GetAnimator().CrossFade("SecondJump", 0.1f);
			CharaSEUtil.PlayPowerAttackSE(context.charaType);
			context.Movement.Velocity = context.Movement.GetForwardDir() * context.DefaultSpeed * context.Parameter.m_powerGrideSpeedRate + -context.Movement.GetGravityDir() * context.Parameter.m_grideFirstUpForce;
			StateUtil.SetAirMovementToRotateGround(context, true);
			context.OnAttack(AttackPower.PlayerPower, DefensePower.PlayerPower);
			context.OnAttackAttribute(AttackAttribute.Power);
			m_effectname = "ef_pl_" + context.CharacterName.ToLower() + "_attack_aura01";
			m_attackEffectname = "ef_pl_" + context.CharacterName.ToLower() + "_attack01";
			GameObject gameobj = StateUtil.CreateEffect(context, m_effectname, false, ResourceCategory.CHARA_EFFECT);
			StateUtil.SetObjectLocalPositionToCenter(context, gameobj);
			m_timer = 0f;
			m_flag.Reset();
			DisableGravity = true;
			context.AddAirAction();
			StateUtil.ThroughBreakable(context, true);
			StateUtil.SetSpecialtyJumpMagnet(context, CharacterAttribute.POWER, ChaoAbility.MAGNET_POWER_TYPE_JUMP, true);
			StateUtil.SetSpecialtyJumpDestroyEnemy(ChaoAbility.JUMP_DESTROY_ENEMY_AND_TRAP);
			StateUtil.SetSpecialtyJumpDestroyEnemy(ChaoAbility.JUMP_DESTROY_ENEMY);
		}

		public override void Leave(CharacterState context)
		{
			context.OffAttack();
			StateUtil.StopEffect(context, m_effectname, 0.5f);
			StateUtil.ThroughBreakable(context, false);
			StateUtil.SetSpecialtyJumpMagnet(context, CharacterAttribute.POWER, ChaoAbility.MAGNET_POWER_TYPE_JUMP, false);
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			m_timer += deltaTime;
			if (DisableGravity && m_timer > context.Parameter.m_disableGravityTime)
			{
				DisableGravity = false;
			}
			Vector3 vertVelocity = context.Movement.VertVelocity;
			if (!DisableGravity)
			{
				vertVelocity += context.Movement.GetGravity() * context.Parameter.m_grideGravityRate * deltaTime;
			}
			Vector3 b = context.Movement.GetForwardDir() * context.DefaultSpeed * context.Parameter.m_powerGrideSpeedRate;
			context.Movement.Velocity = vertVelocity + b;
			if (context.m_input.IsTouched() && StateUtil.CheckAndChangeStateToAirAttack(context, true, false))
			{
				return;
			}
			if (context.Movement.IsOnGround())
			{
				if (!StateUtil.ChangeToJumpStateIfPrecedeInputTouch(context, 0.1f, false))
				{
					StateUtil.SetVelocityForwardRun(context, false);
					StateUtil.NowLanding(context, false);
					context.ChangeState(STATE_ID.Run);
				}
			}
			else if (m_timer > context.Parameter.m_grideTime)
			{
				StateUtil.SetVelocityForwardRun(context, true);
				context.ChangeState(STATE_ID.Fall);
			}
		}

		public override bool DispatchMessage(CharacterState context, int messageId, MessageBase msg)
		{
			if (messageId == 16385)
			{
				StateUtil.CreateEffect(context, context.Position, context.transform.rotation, m_attackEffectname, true, ResourceCategory.CHARA_EFFECT);
				context.Movement.Velocity = context.Movement.GetForwardDir() * context.DefaultSpeed * context.Parameter.m_powerGrideSpeedRate;
				m_timer = 0f;
				return true;
			}
			return false;
		}
	}
}
