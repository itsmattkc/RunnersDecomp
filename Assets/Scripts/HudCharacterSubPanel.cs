using UnityEngine;

public class HudCharacterSubPanel : MonoBehaviour
{
	private enum DataType
	{
		IMAGE,
		IMAGE_SILHOUETTE,
		NUM
	}

	private const string COMMON_PATH = "Anchor_5_MC/2_Character/Btn_2_player/";

	private string[] m_path_name = new string[2]
	{
		"img_player_sub",
		"img_player_sub_default"
	};

	private GameObject[] m_data_obj = new GameObject[2];

	private UIToggle m_toggle;

	private bool m_init_flag;

	private CharaType m_charaType = CharaType.UNKNOWN;

	private TextureRequestChara m_textureRequest;

	private void Start()
	{
		HudMenuUtility.SetTagHudSaveItem(base.gameObject);
		base.enabled = false;
	}

	private void Initialize()
	{
		m_init_flag = true;
		GameObject mainMenuUIObject = HudMenuUtility.GetMainMenuUIObject();
		if (!(mainMenuUIObject != null))
		{
			return;
		}
		for (uint num = 0u; num < 2; num++)
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
		CharaType subChara = instance.PlayerData.SubChara;
		if (HudCharacterPanelUtil.CheckValidChara(subChara))
		{
			HudCharacterPanelUtil.SetGameObjectActive(m_data_obj[0], true);
			HudCharacterPanelUtil.SetGameObjectActive(m_data_obj[1], false);
			if (m_charaType != subChara)
			{
				m_charaType = subChara;
				if (m_textureRequest != null)
				{
				}
				UITexture component = m_data_obj[0].GetComponent<UITexture>();
				if (component != null)
				{
					m_textureRequest = new TextureRequestChara(subChara, component);
					TextureAsyncLoadManager.Instance.Request(m_textureRequest);
				}
			}
		}
		else
		{
			HudCharacterPanelUtil.SetGameObjectActive(m_data_obj[1], true);
			HudCharacterPanelUtil.SetGameObjectActive(m_data_obj[0], false);
		}
	}

	private void SetCheckFlag(bool check_flag)
	{
		if (m_toggle != null)
		{
			m_toggle.value = check_flag;
		}
	}
}
