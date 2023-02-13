namespace Boss
{
	public class BossStateAppearMap1 : BossStateAppearMapBase
	{
		private static float WAIT_TIME1 = 3f;

		private static float WAIT_TIME2 = 1f;

		protected override float GetTime1()
		{
			return WAIT_TIME1;
		}

		protected override float GetTime2()
		{
			return WAIT_TIME2;
		}

		protected override STATE_ID GetNextChangeState()
		{
			return STATE_ID.AttackMap1;
		}
	}
}
