public class EventData
{
	public PointEventData[] point;

	public bool IsBossEvent()
	{
		int num = point.Length;
		if (num == 6)
		{
			return point[5].boss.boss_flag == 1;
		}
		return false;
	}

	public BossEvent GetBossEvent()
	{
		if (IsBossEvent())
		{
			return point[5].boss;
		}
		return new BossEvent();
	}
}
