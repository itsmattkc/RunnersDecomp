namespace Boss
{
	public class BossStateDamageEvent1 : BossStateDamageEventBase
	{
		protected override void ChangeStateWait(ObjBossEventBossState context)
		{
			if (context.BossParam.BossHP > 0)
			{
				context.UpdateBossStateAfterAttack();
				context.ChangeState(EVENTBOSS_STATE_ID.AttackEvent1);
			}
			else
			{
				context.ChangeState(EVENTBOSS_STATE_ID.Dead);
			}
		}
	}
}
