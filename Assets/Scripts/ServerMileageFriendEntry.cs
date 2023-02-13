public class ServerMileageFriendEntry : ServerFriendEntry
{
	public string m_friendId;

	public ServerMileageMapState m_mapState;

	public ServerMileageFriendEntry()
	{
		m_friendId = "0123456789abcdef";
		m_mapState = new ServerMileageMapState();
	}

	public void CopyTo(ServerMileageFriendEntry to)
	{
		if (to != null)
		{
			CopyTo((ServerFriendEntry)to);
			to.m_friendId = m_friendId;
			m_mapState.CopyTo(to.m_mapState);
		}
	}
}
