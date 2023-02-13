public class SocialUserCustomData
{
	private string m_actionId = string.Empty;

	private string m_objectId = string.Empty;

	private string m_gameId = string.Empty;

	public string ActionId
	{
		get
		{
			return m_actionId;
		}
		set
		{
			m_actionId = value;
		}
	}

	public string ObjectId
	{
		get
		{
			return m_objectId;
		}
		set
		{
			m_objectId = value;
		}
	}

	public string GameId
	{
		get
		{
			return m_gameId;
		}
		set
		{
			m_gameId = value;
		}
	}
}
