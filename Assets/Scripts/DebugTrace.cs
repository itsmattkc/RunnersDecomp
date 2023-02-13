public class DebugTrace
{
	private string m_text;

	public string text
	{
		get
		{
			return m_text;
		}
		private set
		{
		}
	}

	public DebugTrace(string trace)
	{
		m_text = trace;
	}
}
