using Message;
using UnityEngine;

namespace Player
{
	public class StatePhantomAsteroidBoss : FSMState<CharacterState>
	{
		private const float FirstSpeed = 10f;

		private const float MaxSpeed = 15f;

		private const float SpeedAcc = 15f;

		private float m_time;

		private GameObject m_effect;

		private bool m_returnFromPhantom;

		private float m_speed;

		private GameObject m_boss;

		public override void Enter(CharacterState context)
		{
			StateUtil.SetRotation(context, Vector3.up, CharacterDefs.BaseFrontTangent);
			m_effect = PhantomAsteroidUtil.ChangeVisualOnEnter(context);
			StateUtil.SetNotDrawItemEffect(context, true);
			context.OnAttack(AttackPower.PlayerColorPower, DefensePower.PlayerColorPower);
			context.OnAttackAttribute(AttackAttribute.PhantomAsteroid);
			m_time = 5f;
			StateUtil.DeactiveInvincible(context);
			StateUtil.SendMessageTransformPhantom(context, PhantomType.ASTEROID);
			m_returnFromPhantom = false;
			context.ChangeMovement(MOVESTATE_ID.GoTargetBoss);
			MsgBossInfo bossInfo = StateUtil.GetBossInfo(null);
			if (bossInfo != null && bossInfo.m_succeed)
			{
				m_boss = bossInfo.m_boss;
			}
			m_speed = 10f;
			CharacterMoveTargetBoss currentState = context.Movement.GetCurrentState<CharacterMoveTargetBoss>();
			if (currentState != null)
			{
				currentState.SetTarget(m_boss);
				currentState.SetSpeed(m_speed);
			}
			StateUtil.SetPhantomMagnetColliderRange(context, PhantomType.ASTEROID);
		}

		public override void Leave(CharacterState context)
		{
			context.OffAttack();
			PhantomAsteroidUtil.ChangeVisualOnLeave(context, m_effect);
			m_effect = null;
			StateUtil.SetNotDrawItemEffect(context, false);
			StateUtil.SendMessageReturnFromPhantom(context, PhantomType.ASTEROID);
			m_boss = null;
			context.SetChangePhantomCancel(ItemType.UNKNOWN);
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			bool flag = false;
			m_speed = Mathf.Min(m_speed + 15f * deltaTime, 15f);
			CharacterMoveTargetBoss currentState = context.Movement.GetCurrentState<CharacterMoveTargetBoss>();
			if (currentState != null)
			{
				currentState.SetSpeed(m_speed);
				flag = currentState.IsTargetNotFound();
			}
			m_time -= deltaTime;
			if (m_time < 0f || m_returnFromPhantom || flag)
			{
				StateUtil.ReturnFromPhantomAndChangeState(context, PhantomType.ASTEROID, m_returnFromPhantom);
			}
		}

		public override bool DispatchMessage(CharacterState context, int messageId, MessageBase msg)
		{
			if (messageId == 16385)
			{
				MsgHitDamageSucceed msgHitDamageSucceed = msg as MsgHitDamageSucceed;
				if (msgHitDamageSucceed != null)
				{
					if (msgHitDamageSucceed.m_sender == m_boss)
					{
						m_returnFromPhantom = true;
					}
					StateUtil.CreateEffect(context, msgHitDamageSucceed.m_position, msgHitDamageSucceed.m_rotation, "ef_ph_aste_bom01", true);
				}
				return true;
			}
			return false;
		}
	}
}
