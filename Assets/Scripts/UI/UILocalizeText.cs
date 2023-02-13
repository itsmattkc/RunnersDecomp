using System;
using Text;
using UnityEngine;

namespace UI
{
	[AddComponentMenu("Scripts/UI/LocalizeText")]
	public class UILocalizeText : MonoBehaviour
	{
		private enum TagType
		{
			TAG_01,
			TAG_02,
			TAG_03,
			NUM
		}

		[Serializable]
		public class TextData
		{
			[SerializeField]
			public TextManager.TextType text_type;

			[SerializeField]
			public string group_id;

			[SerializeField]
			public string cell_id;
		}

		[Serializable]
		public class TagTextData
		{
			[SerializeField]
			public TextData text_data;

			[SerializeField]
			public string tag;
		}

		[SerializeField]
		public TextData m_main_text_data = new TextData();

		[SerializeField]
		public TagTextData[] m_tag_text_data = new TagTextData[3];

		private string m_main_text;

		public string MainText
		{
			get
			{
				return m_main_text;
			}
		}

		public TextData MainTextData
		{
			get
			{
				return m_main_text_data;
			}
			set
			{
				m_main_text_data = value;
			}
		}

		public TagTextData[] TagTextDatas
		{
			get
			{
				return m_tag_text_data;
			}
			set
			{
				m_tag_text_data = value;
			}
		}

		private void Start()
		{
			base.enabled = false;
			SetUILabelText();
		}

		public void SetUILabelText()
		{
			m_main_text = GetMainText();
			if (m_main_text != null)
			{
				UILabel component = base.gameObject.GetComponent<UILabel>();
				if (component != null)
				{
					component.text = m_main_text;
				}
			}
		}

		private string GetMainText()
		{
			if (m_main_text_data != null && m_main_text_data.group_id != null && m_main_text_data.cell_id != null && m_main_text_data.group_id != string.Empty && m_main_text_data.cell_id != string.Empty)
			{
				TextObject text_obj = TextManager.GetText(m_main_text_data.text_type, m_main_text_data.group_id, m_main_text_data.cell_id);
				if (text_obj != null && text_obj.text != null)
				{
					ReplaceTag(ref text_obj);
					return text_obj.text;
				}
			}
			return null;
		}

		private string GetText(ref TextData text_data)
		{
			if (text_data != null && text_data.group_id != null && text_data.cell_id != null && text_data.group_id != string.Empty && text_data.cell_id != string.Empty)
			{
				TextObject text = TextManager.GetText(text_data.text_type, text_data.group_id, text_data.cell_id);
				if (text != null)
				{
					return text.text;
				}
			}
			return null;
		}

		private void ReplaceTag(ref TextObject text_obj)
		{
			if (m_tag_text_data == null || text_obj == null)
			{
				return;
			}
			int num = m_tag_text_data.Length;
			for (int i = 0; i < num; i++)
			{
				if (m_tag_text_data[i] != null)
				{
					string text = GetText(ref m_tag_text_data[i].text_data);
					if (text != null)
					{
						text_obj.ReplaceTag(m_tag_text_data[i].tag, text);
					}
				}
			}
		}
	}
}
