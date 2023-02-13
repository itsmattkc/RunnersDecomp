using System.Collections.Generic;
using UnityEngine;

public class IncentiveWindowQueue : MonoBehaviour
{
	private List<IncentiveWindow> m_queue = new List<IncentiveWindow>();

	private ButtonEventResourceLoader m_resourceLoader;

	public bool SetUpped
	{
		get
		{
			if (m_resourceLoader != null && !m_resourceLoader.IsLoaded)
			{
				return false;
			}
			return true;
		}
	}

	public void AddWindow(IncentiveWindow window)
	{
		m_queue.Add(window);
	}

	public void PlayStart()
	{
		if (!IsEmpty() && m_queue != null)
		{
			m_queue[0].PlayStart();
		}
	}

	public bool IsEmpty()
	{
		if (m_queue.Count > 0)
		{
			return false;
		}
		return true;
	}

	private void Start()
	{
		if (m_resourceLoader == null)
		{
			m_resourceLoader = base.gameObject.AddComponent<ButtonEventResourceLoader>();
		}
		m_resourceLoader.LoadResourceIfNotLoadedAsync("item_get_Window", delegate
		{
			if (FontManager.Instance != null)
			{
				FontManager.Instance.ReplaceFont();
			}
			if (AtlasManager.Instance != null)
			{
				AtlasManager.Instance.ReplaceAtlasForMenu(true);
			}
		});
	}

	private void Update()
	{
		if (IsEmpty() || !SetUpped)
		{
			return;
		}
		IncentiveWindow incentiveWindow = m_queue[0];
		if (incentiveWindow == null)
		{
			return;
		}
		incentiveWindow.Update();
		if (incentiveWindow.IsEnd)
		{
			m_queue.Remove(incentiveWindow);
			if (!IsEmpty())
			{
				m_queue[0].PlayStart();
			}
		}
	}
}
