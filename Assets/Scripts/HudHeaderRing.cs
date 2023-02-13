using UnityEngine;

public class HudHeaderRing : MonoBehaviour
{
	private const string m_ring_path = "Anchor_3_TR/mainmenu_info_quantum/img_bg_stock_ring";

	private const string m_sale_path = "Anchor_3_TR/mainmenu_info_quantum/Btn_shop/img_sale_icon_ring";

	private UILabel m_ui_ring_label;

	private GameObject m_sale_obj;

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
			GameObject gameObject = mainMenuCmnUIObject.transform.FindChild("Anchor_3_TR/mainmenu_info_quantum/img_bg_stock_ring/Lbl_stock").gameObject;
			if (gameObject != null)
			{
				m_ui_ring_label = gameObject.GetComponent<UILabel>();
			}
			m_sale_obj = mainMenuCmnUIObject.transform.FindChild("Anchor_3_TR/mainmenu_info_quantum/Btn_shop/img_sale_icon_ring").gameObject;
		}
		m_initEnd = true;
	}

	public void OnUpdateSaveDataDisplay()
	{
		Initialize();
		if (m_ui_ring_label != null)
		{
			int num = 0;
			SaveDataManager instance = SaveDataManager.Instance;
			if (instance != null)
			{
				num = instance.ItemData.DisplayRingCount;
			}
			m_ui_ring_label.text = HudUtility.GetFormatNumString(num);
		}
		if (m_sale_obj != null)
		{
			bool active = HudMenuUtility.IsSale(Constants.Campaign.emType.PurchaseAddRings);
			m_sale_obj.SetActive(active);
		}
	}
}
