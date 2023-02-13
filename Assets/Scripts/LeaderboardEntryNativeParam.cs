public struct LeaderboardEntryNativeParam
{
	public int mode;

	public int first;

	public int count;

	public int type;

	public int eventId;

	public void Init(int mode, int first, int count, int type, int eventId)
	{
		this.mode = mode;
		this.first = first;
		this.count = count;
		this.type = type;
		this.eventId = eventId;
	}
}
