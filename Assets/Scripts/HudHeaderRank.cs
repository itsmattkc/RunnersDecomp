using UnityEngine;

public class HudHeaderRank : MonoBehaviour
{
	private const string m_rank_label_path = "Anchor_1_TL/mainmenu_info_user/Btn_honor/img_bg_name/Lbl_rank";

	private UILabel m_ui_rank_label;

	private bool m_initEnd;

	private void Start()
	{
		HudMenuUtility.SetTagHudSaveItem(base.gameObject);
		base.enabled = false;
	}

	private void Initialize()
	{
		if (m_initEnd)
		{
			return;
		}
		GameObject mainMenuCmnUIObject = HudMenuUtility.GetMainMenuCmnUIObject();
		if (mainMenuCmnUIObject != null)
		{
			Transform transform = mainMenuCmnUIObject.transform.FindChild("Anchor_1_TL/mainmenu_info_user/Btn_honor/img_bg_name/Lbl_rank");
			if (transform != null)
			{
				GameObject gameObject = transform.gameObject;
				if (gameObject != null)
				{
					m_ui_rank_label = gameObject.GetComponent<UILabel>();
				}
			}
		}
		m_initEnd = true;
	}

	public void OnUpdateSaveDataDisplay()
	{
		Initialize();
		if (m_ui_rank_label != null)
		{
			uint num = 1u;
			SaveDataManager instance = SaveDataManager.Instance;
			if ((bool)instance)
			{
				num = instance.PlayerData.DisplayRank;
			}
			m_ui_rank_label.text = num.ToString();
		}
	}
}
