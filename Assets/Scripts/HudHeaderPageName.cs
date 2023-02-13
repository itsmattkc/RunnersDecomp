using Text;
using UnityEngine;

public class HudHeaderPageName : MonoBehaviour
{
	private UILabel m_header_label;

	private UILabel m_header_label_sdw;

	private bool m_initEnd;

	public void ChangeHeaderText(string headerText)
	{
		if (headerText != null && !(m_header_label == null) && !(m_header_label_sdw == null))
		{
			m_header_label.text = headerText;
			m_header_label_sdw.text = headerText;
		}
	}

	public void OnUpdateSaveDataDisplay()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (m_initEnd)
		{
			return;
		}
		GameObject mainMenuCmnUIObject = HudMenuUtility.GetMainMenuCmnUIObject();
		if (mainMenuCmnUIObject != null)
		{
			GameObject gameObject = mainMenuCmnUIObject.transform.FindChild("Anchor_1_TL/mainmenu_info_user/img_header/Lbl_header").gameObject;
			if (gameObject != null)
			{
				m_header_label = gameObject.GetComponent<UILabel>();
				GameObject gameObject2 = gameObject.transform.FindChild("Lbl_header_sdw").gameObject;
				if (gameObject2 != null)
				{
					m_header_label_sdw = gameObject2.GetComponent<UILabel>();
				}
			}
		}
		m_initEnd = true;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public static string CalcHeaderTextByCellName(string cellName)
	{
		if (cellName == null)
		{
			return null;
		}
		string result = string.Empty;
		TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", cellName);
		if (text != null)
		{
			result = text.text;
		}
		return result;
	}
}
