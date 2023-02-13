using Message;
using UnityEngine;

namespace Player
{
	public class StatePhantomDrillBoss : FSMState<CharacterState>
	{
		private enum Mode
		{
			Down,
			Run,
			Up
		}

		private const float Speed = 25f;

		private const float SeachGroundLength = 30f;

		private const float DigLength = 2f;

		private float m_time;

		private bool m_returnFromPhantom;

		private float m_speed;

		private GameObject m_boss;

		private Mode m_mode;

		private GameObject m_effect;

		private GameObject m_truck;

		private Vector3 m_prevPosition;

		private bool m_nowInDirt;

		public override void Enter(CharacterState context)
		{
			StateUtil.SetRotation(context, Vector3.up, CharacterDefs.BaseFrontTangent);
			m_effect = PhantomDrillUtil.ChangeVisualOnEnter(context);
			m_truck = PhantomDrillUtil.CreateTruck(context);
			StateUtil.SetNotDrawItemEffect(context, true);
			context.OnAttack(AttackPower.PlayerColorPower, DefensePower.PlayerColorPower);
			context.OnAttackAttribute(AttackAttribute.PhantomDrill);
			m_time = 5f;
			m_returnFromPhantom = false;
			m_speed = 25f;
			m_prevPosition = context.Position;
			m_nowInDirt = false;
			StateUtil.DeactiveInvincible(context);
			StateUtil.SendMessageTransformPhantom(context, PhantomType.DRILL);
			MsgBossInfo bossInfo = StateUtil.GetBossInfo(null);
			if (bossInfo != null && bossInfo.m_succeed)
			{
				m_boss = bossInfo.m_boss;
				GoModeDown(context);
			}
			else
			{
				m_returnFromPhantom = true;
			}
		}

		public override void Leave(CharacterState context)
		{
			StateUtil.SetRotation(context, Vector3.up, CharacterDefs.BaseFrontTangent);
			context.OffAttack();
			StateUtil.SetNotDrawItemEffect(context, false);
			PhantomDrillUtil.ChangeVisualOnLeave(context, m_effect);
			PhantomDrillUtil.DestroyTruck(m_truck);
			m_effect = null;
			m_truck = null;
			StateUtil.SendMessageReturnFromPhantom(context, PhantomType.DRILL);
			m_boss = null;
			context.SetChangePhantomCancel(ItemType.UNKNOWN);
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			switch (m_mode)
			{
			case Mode.Down:
				StepModeDown(context);
				break;
			case Mode.Run:
				StepModeRun(context);
				break;
			case Mode.Up:
				StepModeUp(context);
				break;
			}
			m_time -= deltaTime;
			if (m_time < 0f || m_returnFromPhantom)
			{
				StateUtil.ReturnFromPhantomAndChangeState(context, PhantomType.DRILL, m_returnFromPhantom);
				return;
			}
			bool nowInDirt = m_nowInDirt;
			m_nowInDirt = PhantomDrillUtil.CheckTruckDraw(context, m_truck);
			if ((nowInDirt && !m_nowInDirt) || (!nowInDirt && m_nowInDirt))
			{
				PhantomDrillUtil.CheckAndCreateFogEffect(context, !nowInDirt && m_nowInDirt, m_prevPosition);
			}
			m_prevPosition = context.Position;
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

		private void GoModeDown(CharacterState context)
		{
			Vector3 gravityDir = context.Movement.GetGravityDir();
			Vector3 position = context.Position;
			RaycastHit hitInfo;
			if (Physics.Raycast(context.Position, gravityDir, out hitInfo, 30f))
			{
				position = hitInfo.point + gravityDir * 2f;
			}
			context.ChangeMovement(MOVESTATE_ID.GoTarget);
			CharacterMoveTarget movementState = context.GetMovementState<CharacterMoveTarget>();
			if (movementState != null)
			{
				movementState.SetTargetAndSpeed(context.Movement, position, Quaternion.identity, m_speed);
				movementState.SetRotateVelocityDir(true);
			}
			m_mode = Mode.Down;
		}

		private void GoModeRun(CharacterState context)
		{
			context.ChangeMovement(MOVESTATE_ID.GoTargetBoss);
			CharacterMoveTargetBoss currentState = context.Movement.GetCurrentState<CharacterMoveTargetBoss>();
			if (currentState != null)
			{
				currentState.SetTarget(m_boss);
				currentState.SetSpeed(m_speed);
				currentState.SetRotateVelocityDir(true);
				currentState.SetOnlyHorizon(true);
			}
			m_mode = Mode.Run;
		}

		private void GoModeUp(CharacterState context)
		{
			CharacterMoveTargetBoss currentState = context.Movement.GetCurrentState<CharacterMoveTargetBoss>();
			if (currentState != null)
			{
				currentState.SetTarget(m_boss);
				currentState.SetSpeed(m_speed);
				currentState.SetRotateVelocityDir(true);
				currentState.SetOnlyHorizon(false);
			}
			m_mode = Mode.Up;
		}

		private void StepModeDown(CharacterState context)
		{
			CharacterMoveTarget movementState = context.GetMovementState<CharacterMoveTarget>();
			if (movementState != null)
			{
				if (movementState.DoesReachTarget())
				{
					GoModeRun(context);
				}
			}
			else
			{
				m_returnFromPhantom = true;
			}
		}

		private void StepModeRun(CharacterState context)
		{
			CharacterMoveTargetBoss movementState = context.GetMovementState<CharacterMoveTargetBoss>();
			if (movementState != null)
			{
				if (movementState.DoesReachTarget())
				{
					GoModeUp(context);
				}
				else
				{
					m_returnFromPhantom |= movementState.IsTargetNotFound();
				}
			}
			else
			{
				m_returnFromPhantom = true;
			}
		}

		private void StepModeUp(CharacterState context)
		{
			CharacterMoveTargetBoss currentState = context.Movement.GetCurrentState<CharacterMoveTargetBoss>();
			if (currentState != null)
			{
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
