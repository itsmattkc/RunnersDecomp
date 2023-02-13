public class ServerMessageEntry
{
	public enum MessageState
	{
		Unread,
		Read,
		Used,
		Deleted
	}

	public enum MessageType
	{
		RequestEnergy = 0,
		ReturnRequestEnergy = 1,
		SendEnergy = 2,
		ReturnSendEnergy = 3,
		LentChao = 4,
		InviteCode = 5,
		Unknown = -1
	}

	public int m_messageId;

	public MessageType m_messageType;

	public string m_fromId;

	public string m_name;

	public string m_url;

	public MessageState m_messageState;

	public int m_expireTiem;

	public ServerPresentState m_presentState;

	public ServerMessageEntry()
	{
		m_messageId = 2346789;
		m_fromId = "0123456789abcdef";
		m_messageType = MessageType.SendEnergy;
		m_messageState = MessageState.Unread;
		m_name = "0123456789abcdef";
		m_url = "0123456789abcdef";
		m_expireTiem = 0;
		m_presentState = new ServerPresentState();
	}

	public void CopyTo(ServerMessageEntry to)
	{
		to.m_messageId = m_messageId;
		to.m_fromId = m_fromId;
		to.m_messageType = m_messageType;
		to.m_messageState = m_messageState;
		to.m_name = m_name;
		to.m_url = m_url;
		to.m_expireTiem = m_expireTiem;
		m_presentState.CopyTo(to.m_presentState);
	}
}
