namespace Boss
{
	public class BossStateAppearEvent1 : BossStateAppearEventBase
	{
		protected override EVENTBOSS_STATE_ID GetNextChangeState()
		{
			return EVENTBOSS_STATE_ID.AttackEvent1;
		}
	}
}
