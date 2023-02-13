namespace Boss
{
	public class BossStateDamageMap2 : BossStateDamageFever
	{
		protected override string GetStateName()
		{
			return "BossStateDamageMap2";
		}

		protected override void Setup(ObjBossEggmanState context)
		{
			base.Setup(context);
			SetRotAngle(-context.transform.right);
			SetDistance(context.BossParam.DefaultPlayerDistance);
		}

		protected override void ChangeStateWait(ObjBossEggmanState context)
		{
			context.KeepSpeed();
			if (context.BossParam.BossHP > 0)
			{
				context.UpdateBossStateAfterAttack();
				context.ChangeState(STATE_ID.AttackMap2);
			}
			else
			{
				context.ChangeState(STATE_ID.DeadMap);
			}
		}
	}
}
