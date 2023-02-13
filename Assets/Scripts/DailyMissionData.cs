public class DailyMissionData
{
	private enum Reward
	{
		DAYS = 7
	}

	public int id;

	public long progress;

	public int date;

	public int[] reward_id = new int[7];

	public int[] reward_count = new int[7];

	public int reward_max;

	public int max;

	public int clear_count;

	public bool missions_complete;

	public DailyMissionData()
	{
		id = 1;
		progress = 0L;
		date = 1;
		reward_max = 3;
		for (int i = 0; i < 7; i++)
		{
			reward_id[i] = 0;
			reward_count[i] = 1;
		}
		clear_count = 0;
		missions_complete = false;
	}

	public void CopyTo(DailyMissionData dst)
	{
		dst.id = id;
		dst.progress = progress;
		dst.date = date;
		dst.reward_max = reward_max;
		for (int i = 0; i < 7; i++)
		{
			dst.reward_id[i] = reward_id[i];
			dst.reward_count[i] = reward_count[i];
		}
		dst.clear_count = clear_count;
		dst.missions_complete = missions_complete;
	}
}
