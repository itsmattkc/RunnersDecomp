public class ObjTimerSilver : ObjTimerBase
{
	protected override string GetModelName()
	{
		return ObjTimerUtil.GetModelName(TimerType.SILVER);
	}

	protected override TimerType GetTimerType()
	{
		return TimerType.SILVER;
	}
}
