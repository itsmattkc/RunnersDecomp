using System.Collections.Generic;

public class ServerFreeItemState
{
	private List<ServerItemState> m_itemList;

	private bool m_isExpired;

	public List<ServerItemState> itemList
	{
		get
		{
			return m_itemList;
		}
	}

	public ServerFreeItemState()
	{
		m_itemList = new List<ServerItemState>();
	}

	public void AddItem(ServerItemState item)
	{
		m_itemList.Add(item);
	}

	public void SetExpiredFlag(bool flag)
	{
		m_isExpired = flag;
	}

	public bool IsExpired()
	{
		return m_isExpired;
	}

	public void ClearList()
	{
		m_itemList.Clear();
	}

	public void CopyTo(ServerFreeItemState dest)
	{
		foreach (ServerItemState item in m_itemList)
		{
			dest.AddItem(item);
		}
		dest.m_isExpired = m_isExpired;
	}
}
