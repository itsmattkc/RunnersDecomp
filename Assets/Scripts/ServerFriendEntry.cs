using System;

public class ServerFriendEntry
{
	[Flags]
	public enum FriendStateFlags
	{
		None = 0x0,
		SentEnergy = 0x1,
		RequestedEnergy = 0x2,
		Invited = 0x4
	}

	public string m_mid;

	public string m_name;

	public string m_url;

	public FriendStateFlags m_stateFlags;

	public ServerFriendEntry()
	{
		m_mid = "0123456789abcdef";
		m_name = "0123456789abcdef";
		m_url = "0123456789abcdef";
		m_stateFlags = FriendStateFlags.None;
	}

	public void CopyTo(ServerFriendEntry to)
	{
		to.m_mid = m_mid;
		to.m_name = m_name;
		to.m_url = m_url;
		to.m_stateFlags = m_stateFlags;
	}

	public bool IsInvited()
	{
		return FriendStateFlags.Invited == m_stateFlags;
	}
}
