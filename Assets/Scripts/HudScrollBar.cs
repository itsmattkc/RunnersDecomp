using App;
using UnityEngine;

public class HudScrollBar : MonoBehaviour
{
	public delegate void PageChangeCallback(int prevPage, int currentPage);

	private UIScrollBar m_scrollBar;

	private float m_stepValue;

	private PageChangeCallback m_callback;

	private int m_currentPage;

	private bool m_isInit;

	private float m_lastScrollValue;

	private void Start()
	{
	}

	public void Setup(UIScrollBar scrollBar, int pageCount)
	{
		if (!(scrollBar == null) && pageCount > 1)
		{
			m_scrollBar = scrollBar;
			m_stepValue = 1f / ((float)pageCount - 1f);
			EventDelegate.Add(m_scrollBar.onChange, OnChangeScrollBarValue);
		}
	}

	public void SetPageChangeCallback(PageChangeCallback callback)
	{
		m_callback = callback;
	}

	public void LeftScroll(int pageCount)
	{
		float num = m_stepValue * (float)pageCount;
		float value = m_scrollBar.value;
		if (Math.NearZero(value - 1f))
		{
			m_scrollBar.value = 1f - num;
		}
		else if (Math.NearZero(value - num))
		{
			m_scrollBar.value = 0f;
			if (m_scrollBar.onDragFinished != null)
			{
				m_scrollBar.onDragFinished();
			}
		}
		else
		{
			m_scrollBar.value -= num;
		}
	}

	public void RightScroll(int pageCount)
	{
		float num = m_stepValue * (float)pageCount;
		float value = m_scrollBar.value;
		if (Math.NearZero(value))
		{
			m_scrollBar.value = num;
		}
		else if (Math.NearZero(value + num - 1f))
		{
			m_scrollBar.value = 1f;
			if (m_scrollBar.onDragFinished != null)
			{
				m_scrollBar.onDragFinished();
			}
		}
		else
		{
			m_scrollBar.value += num;
		}
	}

	public void PageJump(int pageIndex, bool isInit)
	{
		m_scrollBar.value = m_stepValue * (float)pageIndex;
		if (m_scrollBar.onDragFinished != null)
		{
			m_scrollBar.onDragFinished();
		}
		m_isInit = isInit;
	}

	private void LateUpdate()
	{
		int currentPage = GetCurrentPage();
		if ((m_currentPage != currentPage || m_isInit) && Math.NearEqual(m_lastScrollValue, m_scrollBar.value))
		{
			if (m_callback != null)
			{
				m_callback(m_currentPage, currentPage);
			}
			m_currentPage = currentPage;
			m_isInit = false;
		}
		m_lastScrollValue = m_scrollBar.value;
	}

	private void OnDestroy()
	{
		EventDelegate.Remove(m_scrollBar.onChange, OnChangeScrollBarValue);
	}

	private void OnChangeScrollBarValue()
	{
	}

	private int GetCurrentPage()
	{
		float num = (0f - m_stepValue) * 0.5f;
		float num2 = m_stepValue * 0.5f;
		int num3 = 0;
		while (!(num <= m_scrollBar.value) || !(m_scrollBar.value <= num2))
		{
			num += m_stepValue;
			num2 += m_stepValue;
			num3++;
		}
		return num3;
	}
}
