public class ServerUserTransformData
{
	public string m_userId;

	public string m_facebookId;

	public ServerUserTransformData()
	{
		m_userId = string.Empty;
		m_facebookId = string.Empty;
	}

	public void Dump()
	{
		Debug.Log(string.Format("userId={0}, facebookId={1}", m_userId, m_facebookId));
	}
}
