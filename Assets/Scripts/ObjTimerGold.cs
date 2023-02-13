public class ObjTimerGold : ObjTimerBase
{
	protected override string GetModelName()
	{
		return ObjTimerUtil.GetModelName(TimerType.GOLD);
	}

	protected override TimerType GetTimerType()
	{
		return TimerType.GOLD;
	}
}
