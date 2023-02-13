public class SocialResult
{
	private bool m_isError;

	private int m_resultId;

	private string m_result;

	public bool IsError
	{
		get
		{
			return m_isError;
		}
		set
		{
			m_isError = value;
		}
	}

	public int ResultId
	{
		get
		{
			return m_resultId;
		}
		set
		{
			m_resultId = value;
		}
	}

	public string Result
	{
		get
		{
			return m_result;
		}
		set
		{
			m_result = value;
		}
	}
}
