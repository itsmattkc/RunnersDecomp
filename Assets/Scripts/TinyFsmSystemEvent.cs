public class TinyFsmSystemEvent
{
	private int m_sig;

	public int Signal
	{
		get
		{
			return m_sig;
		}
	}

	protected TinyFsmSystemEvent(int sig)
	{
		m_sig = sig;
	}
}
