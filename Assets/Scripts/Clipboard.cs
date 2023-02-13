public class Clipboard
{
	private static string m_oldText;

	public static string text
	{
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				if (string.IsNullOrEmpty(m_oldText))
				{
					m_oldText = value;
				}
				else
				{
					m_oldText = value;
				}
			}
			if (Binding.Instance != null)
			{
				Binding.Instance.SetClipBoard(value);
			}
		}
	}

	public static string oldText
	{
		get
		{
			return m_oldText;
		}
	}
}
