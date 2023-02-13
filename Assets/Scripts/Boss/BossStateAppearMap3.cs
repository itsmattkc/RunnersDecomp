namespace Boss
{
	public class BossStateAppearMap3 : BossStateAppearMap1
	{
		protected override STATE_ID GetNextChangeState()
		{
			return STATE_ID.AttackMap3;
		}
	}
}
