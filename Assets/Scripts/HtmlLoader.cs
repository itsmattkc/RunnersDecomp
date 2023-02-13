using UnityEngine;

public abstract class HtmlLoader : MonoBehaviour
{
	private WWW m_www;

	private bool m_isEndLoad;

	public bool IsEndLoad
	{
		get
		{
			return m_isEndLoad;
		}
		protected set
		{
			m_isEndLoad = value;
		}
	}

	public HtmlLoader()
	{
	}

	private void OnDestroy()
	{
		if (m_www != null)
		{
			m_www.Dispose();
		}
	}

	public void Setup(string url)
	{
		m_www = new WWW(url);
		OnSetup();
	}

	public string GetUrlContentsText()
	{
		if (m_www == null)
		{
			return null;
		}
		return m_www.text;
	}

	protected WWW GetWWW()
	{
		return m_www;
	}

	protected abstract void OnSetup();
}
