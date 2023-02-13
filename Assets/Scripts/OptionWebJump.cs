using DataTable;
using UnityEngine;

public class OptionWebJump : MonoBehaviour
{
	public enum WebType
	{
		HELP,
		TERMS_OF_SERVICE,
		NUM
	}

	private ui_option_scroll m_ui_option_scroll;

	private WebType m_webType;

	private bool m_setFlag;

	private void Start()
	{
	}

	public void Setup(ui_option_scroll scroll, WebType type)
	{
		base.enabled = true;
		m_webType = type;
		if (m_ui_option_scroll == null && scroll != null)
		{
			m_ui_option_scroll = scroll;
		}
		m_setFlag = true;
	}

	public void Update()
	{
		if (m_setFlag)
		{
			string urlText = GetUrlText();
			if (urlText != null)
			{
				Application.OpenURL(urlText);
			}
			base.enabled = false;
			m_setFlag = false;
			if (m_ui_option_scroll != null)
			{
				m_ui_option_scroll.OnEndChildPage();
			}
		}
	}

	private string GetUrlText()
	{
		if (m_webType == WebType.HELP)
		{
			return InformationDataTable.GetUrl(InformationDataTable.Type.HELP);
		}
		if (m_webType == WebType.TERMS_OF_SERVICE)
		{
			return NetBaseUtil.RedirectTrmsOfServicePageUrl;
		}
		return null;
	}
}
