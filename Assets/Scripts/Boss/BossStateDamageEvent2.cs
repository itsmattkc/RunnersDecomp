namespace Boss
{
	public class BossStateDamageEvent2 : BossStateDamageEventBase
	{
		protected override void ChangeStateWait(ObjBossEventBossState context)
		{
			if (context.BossParam.BossHP > 0)
			{
				context.UpdateBossStateAfterAttack();
				context.ChangeState(EVENTBOSS_STATE_ID.AttackEvent2);
			}
			else
			{
				context.ChangeState(EVENTBOSS_STATE_ID.Dead);
			}
		}
	}
}
