public class ObjTimerBronze : ObjTimerBase
{
	protected override string GetModelName()
	{
		return ObjTimerUtil.GetModelName(TimerType.BRONZE);
	}

	protected override TimerType GetTimerType()
	{
		return TimerType.BRONZE;
	}
}
