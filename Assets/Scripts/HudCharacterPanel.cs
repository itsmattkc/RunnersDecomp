using Message;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class HudCharacterPanel : MonoBehaviour
{
	private enum DataType
	{
		IMAGE,
		NUM
	}

	private enum BonusType
	{
		Score,
		Ring,
		Animal,
		Distance,
		Enemy,
		NUM
	}

	private const string COMMON_PATH = "Anchor_5_MC/2_Character/Btn_2_player/";

	private const string BONUS_COMMON_PATH = "Anchor_5_MC/mainmenu_contents/grid/page_3/slot/ui_mm_main2_page(Clone)/info_bonus/";

	private const string NOTICE_COMMON_PATH = "Anchor_5_MC/mainmenu_contents/grid/page_3/slot/ui_mm_main2_page(Clone)/info_notice";

	private const string PLAYER_COMMON_PATH = "Anchor_5_MC/mainmenu_contents/grid/page_3/slot/ui_mm_main2_page(Clone)/player_set";

	private string[] m_path_name = new string[1]
	{
		"img_player_main"
	};

	private string[] m_bonusPathName = new string[5]
	{
		"Lbl_bonus_0",
		"Lbl_bonus_1",
		"Lbl_bonus_2",
		"Lbl_bonus_3",
		"Lbl_bonus_4"
	};

	private GameObject[] m_data_obj = new GameObject[1];

	private GameObject[] m_bonusDataObj = new GameObject[5];

	private GameObject m_playerSetObj;

	private GameObject m_changeBtnObj;

	private UILabel m_detailTextLabel;

	private UISprite m_detailTextBg;

	private float m_timer;

	private bool m_init_flag;

	private CharaType m_charaType = CharaType.UNKNOWN;

	private TextureRequestChara m_textureRequest;

	private void Start()
	{
		HudMenuUtility.SetTagHudSaveItem(base.gameObject);
		base.enabled = false;
	}

	private void Update()
	{
		m_timer -= Time.deltaTime;
		if (m_timer < 0f)
		{
			SetChageBtn(true);
			base.enabled = false;
		}
	}

	private void Initialize()
	{
		m_init_flag = true;
		GameObject mainMenuUIObject = HudMenuUtility.GetMainMenuUIObject();
		if (!(mainMenuUIObject != null))
		{
			return;
		}
		for (uint num = 0u; num < 1; num++)
		{
			Transform transform = mainMenuUIObject.transform.FindChild("Anchor_5_MC/2_Character/Btn_2_player/" + m_path_name[num]);
			if (transform != null)
			{
				m_data_obj[num] = transform.gameObject;
			}
			else
			{
				m_init_flag = false;
			}
		}
		for (uint num2 = 0u; num2 < 5; num2++)
		{
			Transform transform2 = mainMenuUIObject.transform.FindChild("Anchor_5_MC/mainmenu_contents/grid/page_3/slot/ui_mm_main2_page(Clone)/info_bonus/" + m_bonusPathName[num2]);
			if (transform2 != null)
			{
				m_bonusDataObj[num2] = transform2.gameObject;
			}
			else
			{
				m_init_flag = false;
			}
		}
		Transform transform3 = mainMenuUIObject.transform.FindChild("Anchor_5_MC/mainmenu_contents/grid/page_3/slot/ui_mm_main2_page(Clone)/info_notice");
		if (transform3 != null)
		{
			m_detailTextLabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(transform3.gameObject, "Lbl_bonusnotice");
			m_detailTextBg = GameObjectUtil.FindChildGameObjectComponent<UISprite>(transform3.gameObject, "img_base_bg");
		}
		Transform transform4 = mainMenuUIObject.transform.FindChild("Anchor_5_MC/mainmenu_contents/grid/page_3/slot/ui_mm_main2_page(Clone)/player_set");
		if (!(transform4 != null))
		{
			return;
		}
		m_playerSetObj = transform4.gameObject;
		m_changeBtnObj = GameObjectUtil.FindChildGameObject(m_playerSetObj, "Btn_change");
		if (m_changeBtnObj != null)
		{
			UIButtonMessage component = m_changeBtnObj.GetComponent<UIButtonMessage>();
			if (component != null)
			{
				component.target = base.gameObject;
				component.functionName = "OnClilckChange";
			}
		}
	}

	private void SetBonusParam(GameObject obj, Dictionary<BonusParam.BonusType, float> param, BonusParam.BonusType type)
	{
		if (obj != null && param != null)
		{
			float num = 0f;
			if (param.ContainsKey(type))
			{
				num = param[type];
			}
			UILabel component = obj.GetComponent<UILabel>();
			if (component != null)
			{
				component.text = TextUtility.Replaces(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "clear_percent").text, new Dictionary<string, string>
				{
					{
						"{PARAM}",
						num.ToString()
					}
				});
			}
		}
	}

	private void OnClilckChange()
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		CharaType mainChara = instance.PlayerData.MainChara;
		CharaType subChara = instance.PlayerData.SubChara;
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			int subCharaId = -1;
			ServerCharacterState serverCharacterState = ServerInterface.PlayerState.CharacterState(mainChara);
			if (serverCharacterState != null)
			{
				subCharaId = serverCharacterState.Id;
			}
			int mainCharaId = -1;
			ServerCharacterState serverCharacterState2 = ServerInterface.PlayerState.CharacterState(subChara);
			if (serverCharacterState2 != null)
			{
				mainCharaId = serverCharacterState2.Id;
			}
			loggedInServerInterface.RequestServerChangeCharacter(mainCharaId, subCharaId, base.gameObject);
		}
		else
		{
			instance.PlayerData.MainChara = subChara;
			instance.PlayerData.SubChara = mainChara;
			ServerChangeCharacter_Succeeded(null);
		}
		SoundManager.SePlay("sys_menu_decide");
	}

	private void SetChageBtn(bool flag)
	{
		if (m_changeBtnObj != null)
		{
			UIImageButton component = m_changeBtnObj.GetComponent<UIImageButton>();
			if (component != null)
			{
				component.isEnabled = flag;
			}
		}
	}

	private void ServerChangeCharacter_Succeeded(MsgGetPlayerStateSucceed msg)
	{
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
		base.enabled = true;
		m_timer = 2f;
		SetChageBtn(false);
	}

	private void ServerChangeCharacter_Failed(MsgServerConnctFailed msg)
	{
	}

	public void OnUpdateSaveDataDisplay()
	{
		if (!m_init_flag)
		{
			Initialize();
		}
		SaveDataManager instance = SaveDataManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		CharaType mainChara = instance.PlayerData.MainChara;
		if (HudCharacterPanelUtil.CheckValidChara(mainChara) && m_charaType != mainChara)
		{
			m_charaType = mainChara;
			if (m_textureRequest != null)
			{
			}
			m_textureRequest = new TextureRequestChara(m_charaType, m_data_obj[0].GetComponent<UITexture>());
			TextureAsyncLoadManager.Instance.Request(m_textureRequest);
		}
		CharaType subChara = instance.PlayerData.SubChara;
		int mainChaoID = instance.PlayerData.MainChaoID;
		int subChaoID = instance.PlayerData.SubChaoID;
		BonusParamContainer currentBonusData = BonusUtil.GetCurrentBonusData(mainChara, subChara, mainChaoID, subChaoID);
		if (currentBonusData != null)
		{
			Dictionary<BonusParam.BonusType, float> bonusInfo = currentBonusData.GetBonusInfo();
			if (bonusInfo != null)
			{
				HudCharacterPanelUtil.SetupAbilityIcon(currentBonusData, mainChara, subChara, m_playerSetObj);
				HudCharacterPanelUtil.SetupNoticeView(currentBonusData, m_detailTextLabel, m_detailTextBg);
				SetBonusParam(m_bonusDataObj[0], bonusInfo, BonusParam.BonusType.SCORE);
				SetBonusParam(m_bonusDataObj[1], bonusInfo, BonusParam.BonusType.RING);
				SetBonusParam(m_bonusDataObj[2], bonusInfo, BonusParam.BonusType.ANIMAL);
				SetBonusParam(m_bonusDataObj[3], bonusInfo, BonusParam.BonusType.DISTANCE);
				SetBonusParam(m_bonusDataObj[4], bonusInfo, BonusParam.BonusType.ENEMY_OBJBREAK);
			}
		}
		if (m_changeBtnObj != null)
		{
			m_changeBtnObj.SetActive(subChara != CharaType.UNKNOWN);
		}
	}
}
