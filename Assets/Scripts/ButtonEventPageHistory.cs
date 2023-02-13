using System.Collections.Generic;

public class ButtonEventPageHistory
{
	private List<ButtonInfoTable.PageType> m_pageList = new List<ButtonInfoTable.PageType>();

	public void Push(ButtonInfoTable.PageType pageType)
	{
		if (pageType != ButtonInfoTable.PageType.NON && pageType < ButtonInfoTable.PageType.NUM)
		{
			if (pageType == ButtonInfoTable.PageType.MAIN)
			{
				Clear();
			}
			m_pageList.Add(pageType);
		}
	}

	public ButtonInfoTable.PageType Pop()
	{
		if (m_pageList.Count <= 0)
		{
			return ButtonInfoTable.PageType.MAIN;
		}
		int index = m_pageList.Count - 1;
		ButtonInfoTable.PageType result = m_pageList[index];
		m_pageList.RemoveAt(index);
		return result;
	}

	public ButtonInfoTable.PageType Peek()
	{
		if (m_pageList.Count <= 0)
		{
			return ButtonInfoTable.PageType.MAIN;
		}
		int index = m_pageList.Count - 1;
		return m_pageList[index];
	}

	public void Clear()
	{
		m_pageList.Clear();
		m_pageList.Add(ButtonInfoTable.PageType.MAIN);
	}
}
