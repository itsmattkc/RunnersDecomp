using Message;
using UnityEngine;

namespace Player
{
	public class StatePhantomLaserBoss : FSMState<CharacterState>
	{
		private enum Mode
		{
			Up,
			GoTarget
		}

		private const float Speed = 25f;

		private float m_time;

		private bool m_returnFromPhantom;

		private float m_speed;

		private GameObject m_boss;

		private Mode m_mode;

		public override void Enter(CharacterState context)
		{
			m_mode = Mode.Up;
			StateUtil.SetRotation(context, Vector3.up, CharacterDefs.BaseFrontTangent);
			PhantomLaserUtil.ChangeVisualOnEnter(context);
			StateUtil.SetNotDrawItemEffect(context, true);
			context.OnAttack(AttackPower.PlayerColorPower, DefensePower.PlayerColorPower);
			context.OnAttackAttribute(AttackAttribute.PhantomLaser);
			SoundManager.SePlay("phantom_laser_shoot");
			m_time = 5f;
			m_returnFromPhantom = false;
			m_speed = 25f;
			StateUtil.DeactiveInvincible(context);
			StateUtil.SendMessageTransformPhantom(context, PhantomType.LASER);
			MsgBossInfo bossInfo = StateUtil.GetBossInfo(null);
			if (bossInfo != null && bossInfo.m_succeed)
			{
				m_boss = bossInfo.m_boss;
				GoModeUp(context, bossInfo.m_position);
			}
			else
			{
				m_returnFromPhantom = true;
			}
			StateUtil.SetPhantomMagnetColliderRange(context, PhantomType.LASER);
		}

		public override void Leave(CharacterState context)
		{
			StateUtil.SetRotation(context, Vector3.up, CharacterDefs.BaseFrontTangent);
			PhantomLaserUtil.ChangeVisualOnLeave(context);
			context.OffAttack();
			SoundManager.SeStop("phantom_laser_shoot");
			StateUtil.SetNotDrawItemEffect(context, false);
			StateUtil.SendMessageReturnFromPhantom(context, PhantomType.LASER);
			m_boss = null;
			context.SetChangePhantomCancel(ItemType.UNKNOWN);
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			switch (m_mode)
			{
			case Mode.Up:
				StepModeUp(context);
				break;
			case Mode.GoTarget:
				StepModeGoTarget(context);
				break;
			}
			m_time -= deltaTime;
			if (m_time < 0f || m_returnFromPhantom)
			{
				StateUtil.ResetVelocity(context);
				StateUtil.ReturnFromPhantomAndChangeState(context, PhantomType.LASER, m_returnFromPhantom);
			}
		}

		public override bool DispatchMessage(CharacterState context, int messageId, MessageBase msg)
		{
			if (messageId == 16385)
			{
				MsgHitDamageSucceed msgHitDamageSucceed = msg as MsgHitDamageSucceed;
				if (msgHitDamageSucceed != null && msgHitDamageSucceed.m_sender == m_boss)
				{
					m_returnFromPhantom = true;
				}
				return true;
			}
			return false;
		}

		private void GoModeUp(CharacterState context, Vector3 targetPos)
		{
			Vector3 lhs = targetPos - context.Position;
			Vector3 upDir = context.Movement.GetUpDir();
			Vector3 b = Vector3.Dot(lhs, upDir) * upDir;
			Vector3 position = context.Position + b;
			context.ChangeMovement(MOVESTATE_ID.GoTarget);
			CharacterMoveTarget movementState = context.GetMovementState<CharacterMoveTarget>();
			if (movementState != null)
			{
				movementState.SetTargetAndSpeed(context.Movement, position, Quaternion.identity, m_speed);
				movementState.SetRotateVelocityDir(true);
			}
			m_mode = Mode.Up;
		}

		private void GoModeGoTarget(CharacterState context)
		{
			context.ChangeMovement(MOVESTATE_ID.GoTargetBoss);
			CharacterMoveTargetBoss currentState = context.Movement.GetCurrentState<CharacterMoveTargetBoss>();
			if (currentState != null)
			{
				currentState.SetTarget(m_boss);
				currentState.SetSpeed(m_speed);
				currentState.SetRotateVelocityDir(true);
			}
			m_mode = Mode.GoTarget;
		}

		private void StepModeUp(CharacterState context)
		{
			CharacterMoveTarget movementState = context.GetMovementState<CharacterMoveTarget>();
			if (movementState != null)
			{
				if (movementState.DoesReachTarget())
				{
					GoModeGoTarget(context);
				}
			}
			else
			{
				m_returnFromPhantom = true;
			}
		}

		private void StepModeGoTarget(CharacterState context)
		{
			CharacterMoveTargetBoss currentState = context.Movement.GetCurrentState<CharacterMoveTargetBoss>();
			if (currentState != null)
			{
				currentState.SetSpeed(m_speed);
				bool flag = currentState.IsTargetNotFound();
				m_returnFromPhantom |= flag;
			}
			else
			{
				m_returnFromPhantom = true;
			}
		}
	}
}
