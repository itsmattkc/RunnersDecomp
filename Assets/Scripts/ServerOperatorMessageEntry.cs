public class ServerOperatorMessageEntry
{
	public int m_messageId;

	public string m_content;

	public int m_expireTiem;

	public ServerPresentState m_presentState;

	public ServerOperatorMessageEntry()
	{
		m_messageId = 2346789;
		m_content = string.Empty;
		m_expireTiem = 0;
		m_presentState = new ServerPresentState();
	}

	public void CopyTo(ServerOperatorMessageEntry to)
	{
		to.m_messageId = m_messageId;
		to.m_content = m_content;
		to.m_expireTiem = m_expireTiem;
		m_presentState.CopyTo(to.m_presentState);
	}
}
