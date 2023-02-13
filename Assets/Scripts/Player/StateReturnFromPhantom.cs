namespace Player
{
	public class StateReturnFromPhantom : FSMState<CharacterState>
	{
		private string m_animTriggerName;

		private PhantomType m_phantomType;

		public override void Enter(CharacterState context)
		{
			StateUtil.ResetVelocity(context);
			context.ChangeMovement(MOVESTATE_ID.Air);
			StateUtil.SetAirMovementToRotateGround(context, true);
			context.OnAttack(AttackPower.PlayerColorPower, DefensePower.PlayerColorPower);
			m_phantomType = PhantomType.NONE;
			m_animTriggerName = null;
			ChangePhantomParameter enteringParameter = context.GetEnteringParameter<ChangePhantomParameter>();
			if (enteringParameter != null)
			{
				m_phantomType = enteringParameter.ChangeType;
				switch (m_phantomType)
				{
				case PhantomType.DRILL:
					m_animTriggerName = "Drill";
					break;
				case PhantomType.LASER:
					m_animTriggerName = "Laser";
					break;
				case PhantomType.ASTEROID:
					m_animTriggerName = "Asteroid";
					break;
				}
			}
			context.GetAnimator().CrossFade(m_animTriggerName, 0.1f);
			SoundManager.SePlay("phantom_change");
			StateUtil.SetPhantomQuickTimerPause(true);
		}

		public override void Leave(CharacterState context)
		{
			context.OffAttack();
			m_animTriggerName = null;
			StateUtil.SetPhantomQuickTimerPause(false);
		}

		public override void Step(CharacterState context, float deltaTime)
		{
			string animName = "End" + m_animTriggerName;
			if (StateUtil.IsAnimationEnd(context, animName))
			{
				context.ChangeState(STATE_ID.Fall);
			}
		}
	}
}
