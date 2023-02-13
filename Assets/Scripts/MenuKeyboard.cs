using UnityEngine;

public class MenuKeyboard : MonoBehaviour
{
	private TouchScreenKeyboard m_keyboard;

	private string m_inputText = string.Empty;

	private bool m_isOpen;

	private bool m_isDone;

	private bool m_isCanceled;

	public bool IsDone
	{
		get
		{
			return m_isDone;
		}
		private set
		{
			m_isDone = value;
		}
	}

	public bool IsCanceled
	{
		get
		{
			return m_isCanceled;
		}
		private set
		{
			m_isCanceled = value;
		}
	}

	public string InputText
	{
		get
		{
			return m_inputText;
		}
		private set
		{
			m_inputText = value;
		}
	}

	public void Open()
	{
		m_keyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.Default);
		m_isOpen = true;
		m_isDone = false;
		m_isCanceled = false;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (m_keyboard != null && m_isOpen)
		{
			m_inputText = m_keyboard.text;
			if (m_keyboard.done)
			{
				m_isDone = true;
				m_keyboard = null;
				m_isOpen = false;
			}
			else if (m_keyboard.wasCanceled)
			{
				m_isCanceled = true;
				m_keyboard = null;
				m_isOpen = false;
			}
		}
	}
}
