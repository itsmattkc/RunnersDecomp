public class ServerMileageIncentive
{
	public enum Type
	{
		NONE,
		POINT,
		CHAPTER,
		EPISODE,
		FRIEND
	}

	public Type m_type;

	public int m_itemId;

	public int m_num;

	public int m_pointId;

	public string m_friendId;

	public ServerMileageIncentive()
	{
		m_type = Type.NONE;
		m_itemId = 0;
		m_num = 0;
		m_pointId = 0;
		m_friendId = null;
	}

	public void CopyTo(ServerMileageIncentive to)
	{
		if (to != null)
		{
			to.m_type = m_type;
			to.m_itemId = m_itemId;
			to.m_num = m_num;
			to.m_pointId = m_pointId;
			to.m_friendId = m_friendId;
		}
	}
}
