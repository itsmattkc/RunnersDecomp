namespace Text
{
	public class TextObject
	{
		private string m_text = string.Empty;

		public string text
		{
			get
			{
				return m_text;
			}
			set
			{
				m_text = value;
			}
		}

		public TextObject(string text)
		{
			m_text = text;
		}

		public void ReplaceTag(string tagString, string replaceString)
		{
			if (tagString != null && replaceString != null && !string.IsNullOrEmpty(m_text))
			{
				m_text = m_text.Replace(tagString, replaceString);
			}
		}
	}
}
