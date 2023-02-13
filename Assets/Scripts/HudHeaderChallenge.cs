using System;
using UnityEngine;

public class HudHeaderChallenge : MonoBehaviour
{
	private enum LabelType
	{
		CHALLENGE_COUNT,
		TIME_COUNT,
		OVER_CHALLENGE,
		NUM
	}

	private string[] m_labelName = new string[3]
	{
		"Lbl_challenge",
		"Lbl_time",
		"Lbl_over_challenge"
	};

	private GameObject[] m_ui_obj = new GameObject[3];

	private UILabel[] m_ui_label = new UILabel[3];

	private EnergyStorage m_energy_storage;

	private float m_time;

	private bool m_fill_up_flag;

	private bool m_initEnd;

	private bool m_calledInit;

	private GameObject m_sale_obj;

	private void Start()
	{
		HudMenuUtility.SetTagHudSaveItem(base.gameObject);
	}

	private void Update()
	{
		if (m_initEnd)
		{
			UpdateTimeCountDisplay();
		}
		else if (m_calledInit)
		{
			m_time -= Time.deltaTime;
			if (m_time < 0f)
			{
				Initialize();
			}
		}
	}

	private void Initialize()
	{
		if (m_initEnd)
		{
			return;
		}
		if (m_energy_storage == null)
		{
			GameObject gameObject = GameObject.Find("EnergyStorage");
			if (gameObject != null)
			{
				m_energy_storage = gameObject.GetComponent<EnergyStorage>();
				if (m_energy_storage != null)
				{
					m_fill_up_flag = m_energy_storage.IsFillUpCount();
				}
			}
		}
		GameObject mainMenuCmnUIObject = HudMenuUtility.GetMainMenuCmnUIObject();
		if (mainMenuCmnUIObject != null)
		{
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(mainMenuCmnUIObject, "Anchor_3_TR");
			if (gameObject2 != null)
			{
				for (int i = 0; i < 3; i++)
				{
					if (m_ui_obj[i] == null)
					{
						m_ui_obj[i] = GameObjectUtil.FindChildGameObject(gameObject2, m_labelName[i]);
						if (m_ui_obj[i] != null)
						{
							m_ui_label[i] = m_ui_obj[i].GetComponent<UILabel>();
						}
					}
				}
				if (m_sale_obj == null)
				{
					m_sale_obj = GameObjectUtil.FindChildGameObject(gameObject2, "img_sale_icon_challenge");
				}
			}
		}
		m_initEnd = true;
		for (int j = 0; j < 3; j++)
		{
			if (m_ui_obj[j] == null)
			{
				m_initEnd = false;
				break;
			}
		}
		if (m_sale_obj == null)
		{
			m_initEnd = false;
		}
		if (m_energy_storage == null)
		{
			m_initEnd = false;
		}
		m_calledInit = true;
		m_time = 1f;
	}

	private void SetChallengeCount()
	{
		uint num = 0u;
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null)
		{
			num = instance.PlayerData.DisplayChallengeCount;
		}
		if (m_ui_label[0] != null)
		{
			m_ui_label[0].text = num.ToString();
		}
		if ((bool)m_ui_label[2])
		{
			m_ui_label[2].text = HudUtility.GetFormatNumString(num);
		}
	}

	private void SetLabelActive()
	{
		if (m_ui_obj[2] != null)
		{
			m_ui_obj[2].SetActive(m_fill_up_flag);
		}
		if (m_ui_obj[0] != null)
		{
			m_ui_obj[0].SetActive(!m_fill_up_flag);
		}
		if (m_ui_obj[1] != null)
		{
			m_ui_obj[1].SetActive(!m_fill_up_flag);
		}
	}

	private void UpdateTimeCountDisplay()
	{
		if (!(m_energy_storage != null))
		{
			return;
		}
		m_fill_up_flag = m_energy_storage.IsFillUpCount();
		if (m_fill_up_flag)
		{
			return;
		}
		TimeSpan restTimeForRenew = m_energy_storage.GetRestTimeForRenew();
		if (m_ui_label[1] != null)
		{
			int num = restTimeForRenew.Seconds;
			int num2 = restTimeForRenew.Minutes;
			if (num < 0 && num2 <= 0)
			{
				num2 = 15;
				num = 0;
			}
			m_ui_label[1].text = string.Format("{0}:{1}", num2, num.ToString("D2"));
		}
	}

	public void OnUpdateSaveDataDisplay()
	{
		Initialize();
		if (m_energy_storage != null)
		{
			m_fill_up_flag = m_energy_storage.IsFillUpCount();
		}
		SetChallengeCount();
		SetLabelActive();
		UpdateTimeCountDisplay();
		if (m_sale_obj != null)
		{
			bool active = HudMenuUtility.IsSale(Constants.Campaign.emType.PurchaseAddEnergys);
			m_sale_obj.SetActive(active);
		}
	}

	public void OnUpdateChallengeCountDisply()
	{
		OnUpdateSaveDataDisplay();
	}
}
