using Message;
using UnityEngine;

namespace Player
{
	public class StateChangePhantom : FSMState<CharacterState>
	{
		private string m_animTriggerName;

		private PhantomType m_phantomType;

		private float m_transformTime;

		private ChaoAbility[] m_chaoAbility;

		public override void Enter(CharacterState context)
		{
			StateUtil.ResetVelocity(context);
			context.ChangeMovement(MOVESTATE_ID.Air);
			context.OnAttack(AttackPower.PlayerColorPower, DefensePower.PlayerColorPower);
			m_phantomType = PhantomType.NONE;
			m_animTriggerName = null;
			m_transformTime = -1f;
			string effectname = null;
			m_chaoAbility = new ChaoAbility[4]
			{
				ChaoAbility.COLOR_POWER_SCORE,
				ChaoAbility.COLOR_POWER_TIME,
				ChaoAbility.UNKNOWN,
				ChaoAbility.UNKNOWN
			};
			ChangePhantomParameter enteringParameter = context.GetEnteringParameter<ChangePhantomParameter>();
			if (enteringParameter != null)
			{
				m_phantomType = enteringParameter.ChangeType;
				switch (m_phantomType)
				{
				case PhantomType.DRILL:
					m_animTriggerName = "StartDrill";
					effectname = "ef_pl_change_drill01";
					m_chaoAbility[2] = ChaoAbility.DRILL_SCORE;
					m_chaoAbility[3] = ChaoAbility.DRILL_TIME;
					break;
				case PhantomType.LASER:
					m_animTriggerName = "StartLaser";
					effectname = "ef_pl_change_laser01";
					m_chaoAbility[2] = ChaoAbility.LASER_SCORE;
					m_chaoAbility[3] = ChaoAbility.LASER_TIME;
					break;
				case PhantomType.ASTEROID:
					m_animTriggerName = "StartAsteroid";
					effectname = "ef_pl_change_asteroid01";
					m_chaoAbility[2] = ChaoAbility.ASTEROID_SCORE;
					m_chaoAbility[3] = ChaoAbility.ASTEROID_TIME;
					break;
				}
				m_transformTime = enteringParameter.Timer;
			}
			context.GetAnimator().CrossFade(m_animTriggerName, 0.1f);
			GameObject gameobj = StateUtil.CreateEffect(context, effectname, true);
			StateUtil.SetObjectLocalPositionToCenter(context, gameobj);
			SoundManager.SePlay("phantom_change");
			context.SetNotCharaChange(true);
			context.SetNotUseItem(true);
			context.ClearAirAction();
			MsgPhantomActStart value = new MsgPhantomActStart(m_phantomType);
			GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnSendToGameModeStage", value, SendMessageOptions.DontRequireReceiver);
			ObjUtil.RequestStartAbilityToChao(m_chaoAbility, true);
			StateUtil.SetPhantomQuickTimerPause(true);
		}

		public override void Leave(CharacterState context)
		{
			context.OffAttack();
			m_animTriggerName = null;
			if (!context.IsOnDestroy())
			{
				MsgPhantomActEnd value = new MsgPhantomActEnd(m_phantomType);
				GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnSendToGameModeStage", value, SendMessageOptions.DontRequireReceiver);
			}
			StateUtil.SetPhantomQuickTimerPause(false);
			m_chaoAbility = null;
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			string animTriggerName = m_animTriggerName;
			if (StateUtil.IsAnimationEnd(context, animTriggerName))
			{
				bool flag = false;
				if (context.GetLevelInformation() != null)
				{
					flag = context.GetLevelInformation().NowBoss;
				}
				STATE_ID state = STATE_ID.Run;
				switch (m_phantomType)
				{
				case PhantomType.DRILL:
					state = ((!flag) ? STATE_ID.PhantomDrill : STATE_ID.PhantomDrillBoss);
					break;
				case PhantomType.LASER:
					state = ((!flag) ? STATE_ID.PhantomLaser : STATE_ID.PhantomLaserBoss);
					break;
				case PhantomType.ASTEROID:
					state = ((!flag) ? STATE_ID.PhantomAsteroid : STATE_ID.PhantomAsteroidBoss);
					break;
				}
				if (!flag)
				{
					ChangePhantomParameter changePhantomParameter = context.CreateEnteringParameter<ChangePhantomParameter>();
					changePhantomParameter.Set(m_phantomType, m_transformTime);
				}
				ObjUtil.RequestEndAbilityToChao(m_chaoAbility);
				context.ChangeState(state);
			}
		}
	}
}
