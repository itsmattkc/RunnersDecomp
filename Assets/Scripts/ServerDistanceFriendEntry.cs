public class ServerDistanceFriendEntry
{
	public string m_friendId;

	public int m_distance;

	public ServerDistanceFriendEntry()
	{
		m_friendId = string.Empty;
		m_distance = 0;
	}

	public void CopyTo(ServerDistanceFriendEntry to)
	{
		to.m_friendId = m_friendId;
		to.m_distance = m_distance;
	}
}
