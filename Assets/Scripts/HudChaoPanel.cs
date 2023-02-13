using DataTable;
using Text;
using UnityEngine;

public class HudChaoPanel : MonoBehaviour
{
	private enum DataType
	{
		MAIN_IMAGE,
		MAIN_SILHOUETTE,
		SUB_IMAGE,
		SUB_SILHOUETTE,
		NUM
	}

	private string[] m_path_name = new string[4]
	{
		"Btn_1_chao/img_chao_main",
		"Btn_1_chao/img_chao_main_default",
		"Btn_1_chao/img_chao_sub",
		"Btn_1_chao/img_chao_sub_default"
	};

	private GameObject[] m_data_obj = new GameObject[4];

	private bool m_init_flag;

	private void Start()
	{
		HudMenuUtility.SetTagHudSaveItem(base.gameObject);
		base.enabled = false;
	}

	private void OnDestroy()
	{
	}

	private void Initialize()
	{
		m_init_flag = true;
		GameObject mainMenuUIObject = HudMenuUtility.GetMainMenuUIObject();
		if (!(mainMenuUIObject != null))
		{
			return;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(mainMenuUIObject, "2_Character");
		if (!(gameObject != null))
		{
			return;
		}
		for (uint num = 0u; num < 4; num++)
		{
			Transform transform = gameObject.transform.FindChild(m_path_name[num]);
			if (transform != null)
			{
				m_data_obj[num] = transform.gameObject;
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
		if (instance != null)
		{
			int mainChaoID = instance.PlayerData.MainChaoID;
			SetChaoImage(mainChaoID, m_data_obj[0], m_data_obj[1]);
			int subChaoID = instance.PlayerData.SubChaoID;
			SetChaoImage(subChaoID, m_data_obj[2], m_data_obj[3]);
		}
	}

	private void SetChaoImage(int chao_id, GameObject imageObj, GameObject silhouetteObj)
	{
		if (imageObj == null || silhouetteObj == null)
		{
			return;
		}
		if (chao_id >= 0)
		{
			imageObj.SetActive(true);
			silhouetteObj.SetActive(false);
			UITexture component = imageObj.GetComponent<UITexture>();
			if (component != null)
			{
				ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(component, null, true);
				ChaoTextureManager.Instance.GetTexture(chao_id, info);
				component.enabled = true;
			}
		}
		else
		{
			silhouetteObj.SetActive(true);
			imageObj.SetActive(false);
		}
	}

	private void SetChaoName(int chao_id, GameObject obj)
	{
		if (!(obj != null))
		{
			return;
		}
		UILabel component = obj.GetComponent<UILabel>();
		if (!(component != null))
		{
			return;
		}
		if (chao_id >= 0)
		{
			string chaoText = TextUtility.GetChaoText("Chao", "name_for_menu_" + chao_id.ToString("D4"));
			if (chaoText != null)
			{
				component.text = chaoText;
			}
		}
		else
		{
			component.text = string.Empty;
		}
	}

	private void SetChaoBonusName(int chao_id, GameObject obj)
	{
		if (obj != null)
		{
			UILabel component = obj.GetComponent<UILabel>();
			if (component != null)
			{
				component.text = HudUtility.GetChaoMenuAbilityText(chao_id);
			}
		}
	}

	private void SetChaoLevel(int chao_id, GameObject obj)
	{
		if (!(obj != null))
		{
			return;
		}
		UILabel component = obj.GetComponent<UILabel>();
		if (!(component != null))
		{
			return;
		}
		if (chao_id >= 0)
		{
			ChaoData chaoData = ChaoTable.GetChaoData(chao_id);
			if (chaoData != null)
			{
				int level = chaoData.level;
				if (level > -1)
				{
					obj.SetActive(true);
					component.text = TextUtility.GetTextLevel(level.ToString());
				}
			}
		}
		else
		{
			obj.SetActive(false);
		}
	}

	private void SetChaoTypeImage(int chao_id, GameObject obj)
	{
		if (!(obj != null))
		{
			return;
		}
		if (chao_id >= 0)
		{
			obj.SetActive(true);
			UISprite component = obj.GetComponent<UISprite>();
			if (component != null)
			{
				ChaoData chaoData = ChaoTable.GetChaoData(chao_id);
				if (chaoData != null)
				{
					string str = chaoData.charaAtribute.ToString().ToLower();
					component.spriteName = "ui_chao_set_type_icon_" + str;
				}
			}
		}
		else
		{
			obj.SetActive(false);
		}
	}

	private void SetRareImage(int chao_id, GameObject obj)
	{
		if (!(obj != null))
		{
			return;
		}
		obj.SetActive(true);
		UISprite component = obj.GetComponent<UISprite>();
		if (!(component != null))
		{
			return;
		}
		if (chao_id >= 0)
		{
			ChaoData chaoData = ChaoTable.GetChaoData(chao_id);
			if (chaoData != null)
			{
				component.spriteName = "ui_chao_set_bg_ll_" + (int)chaoData.rarity;
			}
		}
		else
		{
			component.spriteName = "ui_chao_set_bg_ll_3";
		}
	}

	private void SetChaoTexture(GameObject obj, Texture tex)
	{
		if (!(obj != null))
		{
			return;
		}
		obj.SetActive(true);
		UITexture component = obj.GetComponent<UITexture>();
		if (component != null)
		{
			component.enabled = (tex != null);
			if (tex != null)
			{
				component.mainTexture = tex;
			}
		}
	}

	private void OnSetChaoTexture(ChaoTextureManager.TextureData data)
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null)
		{
			if (data.chao_id == instance.PlayerData.MainChaoID)
			{
				SetChaoTexture(m_data_obj[0], data.tex);
			}
			else if (data.chao_id == instance.PlayerData.SubChaoID)
			{
				SetChaoTexture(m_data_obj[2], data.tex);
			}
		}
	}
}
