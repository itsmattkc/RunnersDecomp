using System.Collections.Generic;
using UnityEngine;

public class AgeVerificationButton : MonoBehaviour
{
	public enum LabelType
	{
		TYPE_NONE = -1,
		TYPE_ONE,
		TYPE_TEN,
		TYPE_COUNT
	}

	public delegate void ButtonClickedCallback();

	private ButtonClickedCallback m_callback;

	private LabelType m_labelType;

	private UILabel m_label;

	private GameObject m_upObject;

	private GameObject m_downObject;

	private int m_currentIndex;

	private List<int> m_valuePreset = new List<int>();

	private bool m_noInput = true;

	public int CurrentValue
	{
		get
		{
			if (m_valuePreset != null)
			{
				return m_valuePreset[m_currentIndex];
			}
			return -1;
		}
		private set
		{
		}
	}

	public bool NoInput
	{
		get
		{
			return m_noInput;
		}
		private set
		{
		}
	}

	public void SetLabel(LabelType labelType, UILabel label)
	{
		m_labelType = labelType;
		m_label = label;
	}

	public void SetButton(GameObject upObject, GameObject downObject)
	{
		if (!(upObject == null) && !(downObject == null))
		{
			m_upObject = upObject;
			m_downObject = downObject;
			UIButtonMessage uIButtonMessage = m_upObject.GetComponent<UIButtonMessage>();
			if (uIButtonMessage == null)
			{
				uIButtonMessage = m_upObject.AddComponent<UIButtonMessage>();
			}
			uIButtonMessage.target = base.gameObject;
			uIButtonMessage.functionName = "ButtonMessageUpClicked";
			UIButtonMessage uIButtonMessage2 = m_downObject.GetComponent<UIButtonMessage>();
			if (uIButtonMessage2 == null)
			{
				uIButtonMessage2 = m_downObject.AddComponent<UIButtonMessage>();
			}
			uIButtonMessage2.target = base.gameObject;
			uIButtonMessage2.functionName = "ButtonMessageDownClicked";
		}
	}

	public void Setup(ButtonClickedCallback callback)
	{
		m_callback = callback;
	}

	public void AddValuePreset(int value)
	{
		m_valuePreset.Add(value);
	}

	public void SetDefaultValue(int value)
	{
		for (int i = 0; i < m_valuePreset.Count; i++)
		{
			if (value == m_valuePreset[i])
			{
				m_currentIndex = i;
				SetValue(m_valuePreset[i]);
			}
		}
	}

	private void SetValue(int value)
	{
		string str = string.Empty;
		string empty = string.Empty;
		if (m_noInput)
		{
			str = string.Empty;
			empty = "-";
		}
		else
		{
			if (m_labelType == LabelType.TYPE_TEN)
			{
				str = (value / 10).ToString();
			}
			empty = (value % 10).ToString();
		}
		if (m_label != null)
		{
			string text = str + empty;
			m_label.text = text;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void ButtonMessageUpClicked()
	{
		int num = 0;
		if (m_noInput)
		{
			m_noInput = false;
		}
		else
		{
			m_currentIndex++;
			if (m_currentIndex >= m_valuePreset.Count)
			{
				m_currentIndex = 0;
			}
		}
		num = m_valuePreset[m_currentIndex];
		SetValue(num);
		m_callback();
	}

	private void ButtonMessageDownClicked()
	{
		int num = 0;
		if (m_noInput)
		{
			m_noInput = false;
		}
		else
		{
			m_currentIndex--;
			if (m_currentIndex < 0)
			{
				m_currentIndex = m_valuePreset.Count - 1;
			}
		}
		num = m_valuePreset[m_currentIndex];
		SetValue(num);
		m_callback();
	}
}
