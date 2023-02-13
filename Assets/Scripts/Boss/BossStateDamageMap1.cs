namespace Boss
{
	public class BossStateDamageMap1 : BossStateDamageBase
	{
		private const float START_SPEED = 20f;

		private const float START_DOWNSPEED = 40f;

		private const float WAIT_DOWNSPEED = 1f;

		private const float ROT_SPEED = 30f;

		private const float ROT_DOWNSPEED = 0.3f;

		private const float ROT_MIN = 2f;

		protected override string GetStateName()
		{
			return "BossStateDamageMap1";
		}

		protected override void Setup(ObjBossEggmanState context)
		{
			context.SetSpeed(20f);
			SetSpeedDown(40f);
			SetRotSpeed(30f);
			SetRotSpeedDown(0.3f);
			SetRotMin(2f);
			SetRotAngle(-context.transform.right);
			SetDistance(context.BossParam.DefaultPlayerDistance);
		}

		protected override void SetStateSpeedDown(ObjBossEggmanState context)
		{
			SetSpeedDown(1f);
		}

		protected override void ChangeStateWait(ObjBossEggmanState context)
		{
			context.KeepSpeed();
			if (context.BossParam.BossHP > 0)
			{
				context.UpdateBossStateAfterAttack();
				context.ChangeState(STATE_ID.AttackMap1);
			}
			else
			{
				context.ChangeState(STATE_ID.DeadMap);
			}
		}
	}
}
