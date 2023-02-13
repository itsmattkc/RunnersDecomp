using UnityEngine;

public class HudShopButton : MonoBehaviour
{
	private GameObject m_shoBtn;

	private bool m_forceDisable;

	private void Start()
	{
		HudMenuUtility.SetTagHudSaveItem(base.gameObject);
		base.enabled = false;
	}

	private void Initialize()
	{
		if (!(m_shoBtn != null))
		{
			GameObject mainMenuCmnUIObject = HudMenuUtility.GetMainMenuCmnUIObject();
			if (mainMenuCmnUIObject != null)
			{
				m_shoBtn = GameObjectUtil.FindChildGameObject(mainMenuCmnUIObject, "Btn_shop");
			}
			SetBtn();
		}
	}

	private void SetShopButton(bool flag)
	{
		if (m_shoBtn != null)
		{
			m_shoBtn.SetActive(flag && !m_forceDisable);
		}
		SetBtn();
	}

	public void OnEnableShopButton(bool enableFlag)
	{
		Initialize();
		SetShopButton(enableFlag);
	}

	public void OnForceDisableShopButton(bool disableFlag)
	{
		Initialize();
		m_forceDisable = disableFlag;
		SetShopButton(!disableFlag);
	}

	private void SetBtn()
	{
		if (m_shoBtn != null)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(m_shoBtn, "Btn_charge_rsring");
			if (gameObject != null)
			{
				gameObject.SetActive(ServerInterface.IsRSREnable());
			}
		}
	}
}
