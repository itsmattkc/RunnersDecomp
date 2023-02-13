public class TinyFsmState
{
	private const int IDENTIFIER_INVALID = 0;

	private const int IDENTIFIER_VALID = 1;

	private const int IDENTIFIER_TOP = 2;

	private const int IDENTIFIER_END = 3;

	private uint m_identifier;

	private EventFunction m_delegate;

	public TinyFsmState(EventFunction delegator)
	{
		m_identifier = 1u;
		m_delegate = delegator;
	}

	public TinyFsmState(int identifier)
	{
		m_identifier = (uint)identifier;
	}

	public static TinyFsmState Top()
	{
		return new TinyFsmState(2);
	}

	public static TinyFsmState End()
	{
		return new TinyFsmState(3);
	}

	public bool IsValid()
	{
		return m_identifier != 0;
	}

	public bool IsTop()
	{
		return m_identifier == 2;
	}

	public bool IsEnd()
	{
		return m_identifier == 3;
	}

	public void Clear()
	{
		m_delegate = null;
	}

	public TinyFsmState Call(TinyFsmEvent e)
	{
		if (m_delegate != null)
		{
			return m_delegate(e);
		}
		return null;
	}
}
