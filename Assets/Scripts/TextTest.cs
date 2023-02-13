using Text;
using UnityEngine;

public class TextTest : MonoBehaviour
{
	public string m_labelName = "mission001";

	private UILabel m_label;

	private void Start()
	{
		m_label = GameObject.Find("TextTestLabel").GetComponent<UILabel>();
		if (!(m_label == null))
		{
			string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "mission", m_labelName).text;
			m_label.text = text;
		}
	}

	private void Update()
	{
	}
}
