using Message;
using UnityEngine;

namespace Player
{
	public class StateDead : FSMState<CharacterState>
	{
		private bool m_sendMessage;

		private float m_timer = 1f;

		public override void Enter(CharacterState context)
		{
			context.GetAnimator().CrossFade("Dead", 0.1f);
			StateUtil.Dead(context);
			context.ChangeMovement(MOVESTATE_ID.Air);
			context.Movement.OffGround();
			Vector3 velocity = context.Movement.GetGravityDir() * -6f + context.Movement.GetForwardDir() * -2f;
			context.Movement.Velocity = velocity;
			SoundManager.SePlay("act_damage");
			m_sendMessage = false;
			m_timer = 1f;
			context.ClearAirAction();
			MsgChaoStateUtil.SendMsgChaoState(MsgChaoState.State.STOP);
			if (StageTutorialManager.Instance != null)
			{
				MsgTutorialMiss value = new MsgTutorialMiss();
				StageTutorialManager.Instance.SendMessage("OnMsgTutorialMiss", value, SendMessageOptions.DontRequireReceiver);
			}
			GameObjectUtil.SendDelayedMessageToTagObjects("Boss", "OnPlayerDamage", new MsgBossPlayerDamage(true));
			ObjUtil.SetPlayerDeadRecoveryRing(context.GetPlayerInformation());
		}

		public override void Leave(CharacterState context)
		{
			context.SetStatus(Status.Dead, false);
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			if (!context.Movement.IsOnGround())
			{
				context.Movement.Velocity = context.Movement.Velocity + context.Movement.GetGravity() * deltaTime;
			}
			else
			{
				context.Movement.Velocity = Vector3.zero;
			}
			if (!m_sendMessage && m_timer > 0f)
			{
				m_timer -= deltaTime;
				if (m_timer <= 0f)
				{
					StateUtil.CheckCharaChangeOnDieAndSendMessage(context);
					m_sendMessage = true;
				}
			}
		}
	}
}
