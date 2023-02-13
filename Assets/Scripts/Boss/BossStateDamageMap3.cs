namespace Boss
{
	public class BossStateDamageMap3 : BossStateDamageMap1
	{
		protected override string GetStateName()
		{
			return "BossStateDamageMap3";
		}

		protected override void ChangeStateWait(ObjBossEggmanState context)
		{
			context.KeepSpeed();
			if (context.BossParam.BossHP > 0)
			{
				context.UpdateBossStateAfterAttack();
				context.ChangeState(STATE_ID.AttackMap3);
			}
			else
			{
				context.ChangeState(STATE_ID.DeadMap);
			}
		}
	}
}
