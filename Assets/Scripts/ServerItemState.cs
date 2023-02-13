public class ServerItemState
{
	public int m_itemId;

	public int m_num;

	public ServerItemState()
	{
		m_itemId = 0;
		m_num = 0;
	}

	public void CopyTo(ServerItemState to)
	{
		to.m_itemId = m_itemId;
		to.m_num = m_num;
	}

	public ServerItem GetItem()
	{
		if (m_itemId >= 0)
		{
			return new ServerItem((ServerItem.Id)m_itemId);
		}
		return default(ServerItem);
	}
}
