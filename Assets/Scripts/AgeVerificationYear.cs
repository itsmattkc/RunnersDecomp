using UnityEngine;

public class AgeVerificationYear : MonoBehaviour
{
	public enum ButtonType
	{
		TYPE_NONE = -1,
		TYPE_THOUSAND,
		TYPE_HUNDRED,
		TYPE_TEN,
		TYPE_ONE,
		TYPE_COUNT
	}

	private static readonly string[] YearName = new string[4]
	{
		"year_0xxx",
		"year_x0xx",
		"year_xx0x",
		"year_xxx0"
	};

	private AgeVerificationButton[] m_buttons = new AgeVerificationButton[4];

	private AgeVerificationButton.ButtonClickedCallback m_callback;

	public bool NoInput
	{
		get
		{
			AgeVerificationButton[] buttons = m_buttons;
			foreach (AgeVerificationButton ageVerificationButton in buttons)
			{
				if (!(ageVerificationButton == null) && ageVerificationButton.NoInput)
				{
					return true;
				}
			}
			return false;
		}
	}

	public int CurrentValue
	{
		get
		{
			int num = 1;
			for (int i = 0; i < 4; i++)
			{
				if (m_buttons[i] == null)
				{
					return 1970;
				}
				num *= 10;
			}
			num /= 10;
			int num2 = 0;
			for (int j = 0; j < 4; j++)
			{
				num2 += m_buttons[j].CurrentValue * num;
				num /= 10;
			}
			return num2;
		}
		private set
		{
		}
	}

	public void SetCallback(AgeVerificationButton.ButtonClickedCallback callback)
	{
		m_callback = callback;
	}

	public void Setup()
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "year_set");
		if (gameObject == null)
		{
			return;
		}
		AgeVerificationButton.ButtonClickedCallback[] array = new AgeVerificationButton.ButtonClickedCallback[4]
		{
			ThousandButtonClickedCallback,
			HundredButtonClickedCallback,
			TenButtonClickedCallback,
			OneButtonClickedCallback
		};
		for (int i = 0; i < 4; i++)
		{
			string name = "Lbl_" + YearName[i];
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, name);
			if (!(gameObject2 == null))
			{
				AgeVerificationButton ageVerificationButton = gameObject2.AddComponent<AgeVerificationButton>();
				UILabel component = gameObject2.GetComponent<UILabel>();
				ageVerificationButton.SetLabel(AgeVerificationButton.LabelType.TYPE_ONE, component);
				string str = "Btn_" + YearName[i];
				GameObject upObject = GameObjectUtil.FindChildGameObject(gameObject, str + "_up");
				GameObject downObject = GameObjectUtil.FindChildGameObject(gameObject, str + "_down");
				ageVerificationButton.SetButton(upObject, downObject);
				ageVerificationButton.Setup(array[i]);
				m_buttons[i] = ageVerificationButton;
			}
		}
		m_buttons[0].AddValuePreset(1);
		m_buttons[0].AddValuePreset(2);
		m_buttons[0].SetDefaultValue(1);
		m_buttons[1].AddValuePreset(0);
		m_buttons[1].AddValuePreset(9);
		m_buttons[1].SetDefaultValue(0);
		for (int j = 0; j <= 9; j++)
		{
			m_buttons[2].AddValuePreset(j);
			m_buttons[3].AddValuePreset(j);
		}
		m_buttons[2].SetDefaultValue(0);
		m_buttons[3].SetDefaultValue(0);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void ThousandButtonClickedCallback()
	{
		if (m_callback != null)
		{
			m_callback();
		}
	}

	private void HundredButtonClickedCallback()
	{
		if (m_callback != null)
		{
			m_callback();
		}
	}

	private void TenButtonClickedCallback()
	{
		if (m_callback != null)
		{
			m_callback();
		}
	}

	private void OneButtonClickedCallback()
	{
		if (m_callback != null)
		{
			m_callback();
		}
	}
}
