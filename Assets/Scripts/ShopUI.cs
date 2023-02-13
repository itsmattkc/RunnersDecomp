using DataTable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Text;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
	private enum ServerEexchangeType
	{
		RSRING,
		RING,
		CHALLENGE,
		Count
	}

	[Serializable]
	public class EexchangeItem
	{
		[SerializeField]
		public UILabel m_quantityLabel;

		[SerializeField]
		public UILabel m_costLabel;

		[SerializeField]
		public GameObject m_bonusGameObject;

		[SerializeField]
		public UILabel[] m_bonusLabels = new UILabel[2];

		[SerializeField]
		public UISprite m_saleSprite;

		[SerializeField]
		public UILabel m_saleQuantityLabel;

		[HideInInspector]
		public ServerRedStarItemState m_storeItem;

		[HideInInspector]
		public ServerCampaignData m_campaignData;

		[HideInInspector]
		public bool isCampaign
		{
			get
			{
				return m_campaignData != null;
			}
		}

		[HideInInspector]
		public int quantity
		{
			get
			{
				if (m_storeItem == null)
				{
					return 0;
				}
				float num = ServerCampaignData.fContentBasis;
				if (m_campaignData != null)
				{
					num = m_campaignData.iContent;
				}
				return (int)((float)m_storeItem.m_numItem * num / ServerCampaignData.fContentBasis);
			}
		}
	}

	[Serializable]
	private class EexchangeType
	{
		[SerializeField]
		public EexchangeItem[] m_exchangeItems = new EexchangeItem[5];
	}

	public const int EXCHANGE_ITEM_PACK_COUNT = 5;

	[SerializeField]
	private EexchangeType[] m_exchangeTypes = new EexchangeType[3];

	[SerializeField]
	private GameObject[] m_exchangeObjects = new GameObject[3];

	[SerializeField]
	public int m_tabOffsetX;

	private GameObject m_freeGetButton;

	private GameObject m_tabRSR;

	private string[] m_redStarPrice = new string[5];

	private bool m_isInitShop;

	private static List<Constants.Campaign.emType>[] s_campaignTypes = new List<Constants.Campaign.emType>[3]
	{
		new List<Constants.Campaign.emType>
		{
			Constants.Campaign.emType.PurchaseAddRsrings,
			Constants.Campaign.emType.PurchaseAddRsringsNoChargeUser
		},
		new List<Constants.Campaign.emType>
		{
			Constants.Campaign.emType.PurchaseAddRings
		},
		new List<Constants.Campaign.emType>
		{
			Constants.Campaign.emType.PurchaseAddEnergys
		}
	};

	public bool IsInitShop
	{
		get
		{
			return m_isInitShop;
		}
	}

	private bool isStarted
	{
		get;
		set;
	}

	private void SetCurrentType(ServerEexchangeType type)
	{
		for (int i = 0; i < 3; i++)
		{
			if (m_exchangeObjects[i] != null)
			{
				if (type == (ServerEexchangeType)i)
				{
					m_exchangeObjects[i].SetActive(true);
				}
				else
				{
					m_exchangeObjects[i].SetActive(false);
				}
			}
		}
	}

	public static GameObject CalcSaleIconObject(GameObject rootObject, int itemIndex)
	{
		if (rootObject == null)
		{
			return null;
		}
		string name = "img_sale_icon_" + (itemIndex + 1);
		return GameObjectUtil.FindChildGameObject(rootObject, name);
	}

	private void Start()
	{
		isStarted = true;
	}

	private void OnStartShopRedStarRing()
	{
		SetCurrentType(ServerEexchangeType.RSRING);
		StartCoroutine(StartShopCoroutine("Btn_tab_rsring"));
	}

	private void OnStartShopRing()
	{
		SetCurrentType(ServerEexchangeType.RING);
		StartCoroutine(StartShopCoroutine("Btn_tab_ring"));
	}

	private void OnStartShopChallenge()
	{
		SetCurrentType(ServerEexchangeType.CHALLENGE);
		StartCoroutine(StartShopCoroutine("Btn_tab_challenge"));
	}

	private void OnStartShopEvent()
	{
		if (isRaidbossEvent())
		{
			StartCoroutine(StartShopCoroutine("Btn_tab_raidboss"));
		}
		else
		{
			OnStartShopChallenge();
		}
	}

	private IEnumerator StartShopCoroutine(string buttonName)
	{
		yield return null;
		UIToggle uiToggle = GameObjectUtil.FindChildGameObjectComponent<UIToggle>(base.gameObject.transform.root.gameObject, buttonName);
		if (uiToggle != null)
		{
			uiToggle.value = true;
		}
		for (ServerEexchangeType type = ServerEexchangeType.RSRING; type < ServerEexchangeType.Count; type++)
		{
			EexchangeType exchangeType = m_exchangeTypes[(int)type];
			if (exchangeType == null)
			{
				continue;
			}
			EexchangeItem[] exchangeItems = exchangeType.m_exchangeItems;
			foreach (EexchangeItem exchangeItem in exchangeItems)
			{
				if (exchangeItem != null)
				{
					exchangeItem.m_quantityLabel.gameObject.SetActive(false);
					exchangeItem.m_costLabel.gameObject.SetActive(false);
					if (exchangeItem.m_bonusGameObject != null)
					{
						exchangeItem.m_bonusGameObject.SetActive(false);
					}
					exchangeItem.m_saleSprite.gameObject.SetActive(false);
				}
			}
		}
		OnStartShop();
	}

	private void OnStartShop()
	{
		HudMenuUtility.SendEnableShopButton(false);
		if (m_tabRSR == null)
		{
			m_tabRSR = GameObjectUtil.FindChildGameObject(base.gameObject.transform.root.gameObject, "tab_rsring");
		}
		if (!ServerInterface.IsRSREnable())
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject.transform.root.gameObject, "Btn_tab_rsring");
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
		}
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			SetExchangeItems(ServerEexchangeType.RSRING, ServerInterface.RedStarItemList);
			SetExchangeItems(ServerEexchangeType.RING, ServerInterface.RedStarExchangeRingItemList);
			SetExchangeItems(ServerEexchangeType.CHALLENGE, ServerInterface.RedStarExchangeEnergyItemList);
		}
		NativeObserver instance = NativeObserver.Instance;
		List<string> list = new List<string>();
		for (int i = 0; i < NativeObserver.ProductCount; i++)
		{
			list.Add(instance.GetProductName(i));
		}
		instance.RequestProductsInfo(list, FinishedToGetProductList);
		m_freeGetButton = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_shop_free_get_rt");
		if (m_freeGetButton != null)
		{
			m_freeGetButton.SetActive(false);
			UIButtonMessage uIButtonMessage = m_freeGetButton.GetComponent<UIButtonMessage>();
			if (uIButtonMessage == null)
			{
				uIButtonMessage = m_freeGetButton.AddComponent<UIButtonMessage>();
			}
			uIButtonMessage.target = base.gameObject;
			uIButtonMessage.functionName = "OnClickGetFreeRsRing";
			uIButtonMessage.enabled = true;
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_shop_free_get_ct");
		if (gameObject2 != null)
		{
			gameObject2.SetActive(false);
		}
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_shop_legal");
		if (gameObject3 != null)
		{
			RegionManager instance2 = RegionManager.Instance;
			if (instance2 != null)
			{
				bool active = false;
				if (instance2.IsJapan())
				{
					active = true;
				}
				gameObject3.SetActive(active);
			}
		}
		GameObject gameObject4 = GameObjectUtil.FindChildGameObject(base.gameObject, "billing_notice");
		if (gameObject4 != null && loggedInServerInterface != null && ServerInterface.SettingState != null)
		{
			if (ServerInterface.SettingState.m_isPurchased || !ServerInterface.IsRSREnable())
			{
				gameObject4.SetActive(false);
			}
			else
			{
				gameObject4.SetActive(true);
			}
		}
		GameObject gameObject5 = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_tab_raidboss");
		if (gameObject5 != null)
		{
			gameObject5.SetActive(false);
		}
		m_isInitShop = true;
	}

	private void SetExchangeItems(ServerEexchangeType type, List<ServerRedStarItemState> storeItems)
	{
		EexchangeType eexchangeType = m_exchangeTypes[(int)type];
		if (eexchangeType == null)
		{
			return;
		}
		for (int i = 0; i < 5; i++)
		{
			if (i < storeItems.Count)
			{
				EexchangeItem eexchangeItem = eexchangeType.m_exchangeItems[i];
				eexchangeItem.m_storeItem = storeItems[i];
				eexchangeItem.m_campaignData = null;
				foreach (Constants.Campaign.emType item in s_campaignTypes[(int)type])
				{
					eexchangeItem.m_campaignData = ServerInterface.CampaignState.GetCampaignInSession(item, eexchangeItem.m_storeItem.m_storeItemId);
					if (eexchangeItem.m_campaignData != null)
					{
						break;
					}
				}
			}
			else if (type == ServerEexchangeType.RSRING)
			{
				SetRSRItemBtn(i);
			}
		}
	}

	private void SetRSRItemBtn(int index)
	{
		if (!(m_tabRSR != null))
		{
			return;
		}
		int num = index + 1;
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_tabRSR, "item_" + num);
		if (!(gameObject != null))
		{
			return;
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "Btn_item_rs_" + num);
		if (gameObject2 != null)
		{
			UIImageButton component = gameObject2.GetComponent<UIImageButton>();
			if (component != null)
			{
				component.isEnabled = false;
			}
		}
	}

	private void OnShopBackButtonClicked()
	{
		NativeObserver instance = NativeObserver.Instance;
		if (instance != null)
		{
			instance.ResetIapDelegate();
		}
		HudMenuUtility.SendEnableShopButton(true);
	}

	private void FinishedToGetProductList(NativeObserver.IAPResult result)
	{
		string text = string.Format("Finished to get Product List from AppStore!(result:{0})", (int)result);
		EexchangeType eexchangeType = m_exchangeTypes[0];
		if (eexchangeType == null)
		{
			return;
		}
		for (int i = 0; i < 5; i++)
		{
			EexchangeItem eexchangeItem = eexchangeType.m_exchangeItems[i];
			if (eexchangeItem.m_storeItem != null)
			{
				string productPrice = NativeObserver.Instance.GetProductPrice(eexchangeItem.m_storeItem.m_productId);
				if (productPrice != null)
				{
					m_redStarPrice[i] = productPrice;
				}
				else
				{
					m_redStarPrice[i] = eexchangeItem.m_storeItem.m_priceDisp;
				}
			}
			else
			{
				m_redStarPrice[i] = string.Empty;
			}
		}
		UpdateView();
	}

	private void UpdateView()
	{
		if (!m_isInitShop)
		{
			return;
		}
		for (ServerEexchangeType serverEexchangeType = ServerEexchangeType.RSRING; serverEexchangeType < ServerEexchangeType.Count; serverEexchangeType++)
		{
			EexchangeType eexchangeType = m_exchangeTypes[(int)serverEexchangeType];
			if (eexchangeType == null)
			{
				continue;
			}
			for (int i = 0; i < 5; i++)
			{
				EexchangeItem eexchangeItem = eexchangeType.m_exchangeItems[i];
				if (eexchangeItem == null || eexchangeItem.m_storeItem == null)
				{
					continue;
				}
				int num = eexchangeItem.m_storeItem.m_numItem - GetBonusCount(serverEexchangeType, i);
				eexchangeItem.m_quantityLabel.text = HudUtility.GetFormatNumString(num);
				eexchangeItem.m_quantityLabel.gameObject.SetActive(true);
				if (serverEexchangeType == ServerEexchangeType.RSRING)
				{
					if (!string.IsNullOrEmpty(m_redStarPrice[i]))
					{
						eexchangeItem.m_costLabel.text = m_redStarPrice[i];
					}
				}
				else
				{
					eexchangeItem.m_costLabel.text = HudUtility.GetFormatNumString(eexchangeItem.m_storeItem.m_price);
				}
				eexchangeItem.m_costLabel.gameObject.SetActive(true);
				string presentString = GetPresentString(serverEexchangeType, i);
				bool active = presentString != null;
				eexchangeItem.m_saleSprite.gameObject.SetActive(active);
				bool isCampaign = eexchangeItem.isCampaign;
				GameObject gameObject = CalcSaleIconObject(eexchangeItem.m_saleSprite.gameObject, i);
				if (gameObject != null)
				{
					gameObject.SetActive(isCampaign);
				}
				if (eexchangeItem.m_bonusGameObject != null && presentString != null)
				{
					eexchangeItem.m_saleQuantityLabel.text = presentString;
					UILabel[] bonusLabels = eexchangeItem.m_bonusLabels;
					foreach (UILabel uILabel in bonusLabels)
					{
						uILabel.text = presentString;
					}
				}
			}
		}
	}

	private string GetPresentString(ServerEexchangeType type, int itemIndex)
	{
		EexchangeType eexchangeType = m_exchangeTypes[(int)type];
		if (eexchangeType == null)
		{
			return null;
		}
		if (eexchangeType.m_exchangeItems[itemIndex] == null)
		{
			return null;
		}
		int numItem = eexchangeType.m_exchangeItems[itemIndex].m_storeItem.m_numItem;
		EexchangeItem eexchangeItem = eexchangeType.m_exchangeItems[itemIndex];
		if (eexchangeItem.isCampaign)
		{
			int quantity = eexchangeItem.quantity;
			int num = quantity - numItem;
			int bonusCount = GetBonusCount(type, itemIndex);
			int bonusCount2 = num + bonusCount;
			return CalcBonusString(type, bonusCount2);
		}
		return CalcBonusString(type, GetBonusCount(type, itemIndex));
	}

	private int GetBonusCount(ServerEexchangeType type, int itemIndex)
	{
		EexchangeType eexchangeType = m_exchangeTypes[(int)type];
		if (eexchangeType == null)
		{
			return 0;
		}
		int numItem = eexchangeType.m_exchangeItems[0].m_storeItem.m_numItem;
		int num = (type != 0) ? eexchangeType.m_exchangeItems[0].m_storeItem.m_price : HudUtility.GetMixedStringToInt(eexchangeType.m_exchangeItems[0].m_costLabel.text);
		EexchangeItem eexchangeItem = eexchangeType.m_exchangeItems[itemIndex];
		if (eexchangeItem == null)
		{
			return 0;
		}
		int numItem2 = eexchangeType.m_exchangeItems[itemIndex].m_storeItem.m_numItem;
		int num2 = (type != 0) ? eexchangeItem.m_storeItem.m_price : HudUtility.GetMixedStringToInt(eexchangeItem.m_costLabel.text);
		return (int)((float)numItem2 - (float)numItem * (float)num2 / (float)num);
	}

	private string CalcBonusString(ServerEexchangeType type, int bonusCount)
	{
		string result = null;
		if (bonusCount > 0)
		{
			result = GetText("label_" + type.ToString().ToLower() + "_bonus", new Dictionary<string, string>
			{
				{
					"{COUNT}",
					HudUtility.GetFormatNumString(bonusCount)
				}
			});
		}
		return result;
	}

	private void Update()
	{
		UpdateView();
		if (GeneralWindow.IsCreated("RsrBuyLegal") && GeneralWindow.IsButtonPressed)
		{
			GeneralWindow.Close();
		}
		if (GeneralWindow.IsCreated("EventEndError") && GeneralWindow.IsButtonPressed)
		{
			GeneralWindow.Close();
			UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(base.gameObject, "Btn_cmn_back");
			if (uIButtonMessage != null)
			{
				uIButtonMessage.SendMessage("OnClick");
			}
		}
		if (m_freeGetButton != null && !m_freeGetButton.activeSelf)
		{
			// Permanently enabled for fun, don't know what it does yet
			m_freeGetButton.SetActive(true);
		}
	}

	private void OnClickRsr1()
	{
		BuyRsring(0);
	}

	private void OnClickRsr2()
	{
		BuyRsring(1);
	}

	private void OnClickRsr3()
	{
		BuyRsring(2);
	}

	private void OnClickRsr4()
	{
		BuyRsring(3);
	}

	private void OnClickRsr5()
	{
		BuyRsring(4);
	}

	private void OnClickRing1()
	{
		BuyRing(0);
	}

	private void OnClickRing2()
	{
		BuyRing(1);
	}

	private void OnClickRing3()
	{
		BuyRing(2);
	}

	private void OnClickRing4()
	{
		BuyRing(3);
	}

	private void OnClickRing5()
	{
		BuyRing(4);
	}

	private void OnClickChallenge1()
	{
		BuyChallenge(0);
	}

	private void OnClickChallenge2()
	{
		BuyChallenge(1);
	}

	private void OnClickChallenge3()
	{
		BuyChallenge(2);
	}

	private void OnClickChallenge4()
	{
		BuyChallenge(3);
	}

	private void OnClickChallenge5()
	{
		BuyChallenge(4);
	}

	private void OnClickRaidbossEnergy1()
	{
		BuyRaidbossEnergy(0);
	}

	private void OnClickRaidbossEnergy2()
	{
		BuyRaidbossEnergy(1);
	}

	private void OnClickRaidbossEnergy3()
	{
		BuyRaidbossEnergy(2);
	}

	private void OnClickRaidbossEnergy4()
	{
		BuyRaidbossEnergy(3);
	}

	private void OnClickRaidbossEnergy5()
	{
		BuyRaidbossEnergy(4);
	}

	private void BuyRsring(int i)
	{
		GameObject gameObject = base.gameObject.transform.parent.gameObject;
		ShopWindowRsringUI shopWindowRsringUI = GameObjectUtil.FindChildGameObjectComponent<ShopWindowRsringUI>(gameObject, "ShopWindowRsringUI");
		if (shopWindowRsringUI != null)
		{
			shopWindowRsringUI.gameObject.SetActive(true);
			shopWindowRsringUI.OpenWindow(i, m_exchangeTypes[0].m_exchangeItems[i], PurchaseSuccessCallback);
		}
	}

	private void BuyRing(int i)
	{
		GameObject gameObject = base.gameObject.transform.parent.gameObject;
		ShopWindowRingUI shopWindowRingUI = GameObjectUtil.FindChildGameObjectComponent<ShopWindowRingUI>(gameObject, "ShopWindowRingUI");
		if (shopWindowRingUI != null)
		{
			shopWindowRingUI.gameObject.SetActive(true);
			shopWindowRingUI.OpenWindow(i, m_exchangeTypes[1].m_exchangeItems[i]);
		}
	}

	private void BuyChallenge(int i)
	{
		GameObject gameObject = base.gameObject.transform.parent.gameObject;
		ShopWindowChallengeUI shopWindowChallengeUI = GameObjectUtil.FindChildGameObjectComponent<ShopWindowChallengeUI>(gameObject, "ShopWindowChallengeUI");
		if (shopWindowChallengeUI != null)
		{
			shopWindowChallengeUI.gameObject.SetActive(true);
			shopWindowChallengeUI.OpenWindow(i, m_exchangeTypes[2].m_exchangeItems[i]);
		}
	}

	private void BuyRaidbossEnergy(int i)
	{
		if (!isRaidbossEvent())
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "EventEndError";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.caption = GetText("gw_raid_count_error_caption");
			info.message = GetText("gw_raid_count_error_text");
			info.isPlayErrorSe = true;
			GeneralWindow.Create(info);
		}
		else
		{
			GameObject gameObject = base.gameObject.transform.parent.gameObject;
			ShopWindowRaidUI shopWindowRaidUI = GameObjectUtil.FindChildGameObjectComponent<ShopWindowRaidUI>(gameObject, "ShopWindowRaidUI");
			if (shopWindowRaidUI != null)
			{
				shopWindowRaidUI.gameObject.SetActive(true);
			}
		}
	}

	private void OnClickLegal()
	{
		StartCoroutine(OpenLegalWindow());
	}

	private IEnumerator OpenLegalWindow()
	{
		BackKeyManager.InvalidFlag = true;
		HudMenuUtility.SetConnectAlertMenuButtonUI(true);
		string url = NetUtil.GetWebPageURL(InformationDataTable.Type.SHOP_LEGAL);
		GameObject htmlParserGameObject = HtmlParserFactory.Create(url, HtmlParser.SyncType.TYPE_ASYNC, HtmlParser.SyncType.TYPE_ASYNC);
		if (htmlParserGameObject == null)
		{
			yield return null;
		}
		HtmlParser htmlParser = htmlParserGameObject.GetComponent<HtmlParser>();
		if (htmlParser == null)
		{
			yield return null;
		}
		if (htmlParser != null)
		{
			while (!htmlParser.IsEndParse)
			{
				yield return null;
			}
			string legalText = htmlParser.ParsedString;
			UnityEngine.Object.Destroy(htmlParserGameObject);
			BackKeyManager.InvalidFlag = false;
			HudMenuUtility.SetConnectAlertMenuButtonUI(false);
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "RsrBuyLegal";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.caption = TextUtility.GetCommonText("Shop", "ui_Lbl_word_legal_caption");
			info.message = legalText;
			GeneralWindow.Create(info);
		}
	}

	private void SetShopBtnObj(bool flag)
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Anchor_8_BC");
		if (gameObject != null)
		{
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "shop_btn");
			if (gameObject2 != null)
			{
				gameObject2.SetActive(flag);
			}
		}
	}

	private void OnClickGetFreeRsRing()
	{
		Debug.Log("ShopUI.OnClickGetFreeRsRing");
		SaveDataManager.Instance.ConnectData.ReplaceMessageBox = true;
	}

	public void OnRingTabChange()
	{
		SetCurrentType(ServerEexchangeType.RING);
		SetShopBtnObj(true);
		UIToggle uIToggle = GameObjectUtil.FindChildGameObjectComponent<UIToggle>(base.gameObject.transform.root.gameObject, "Btn_tab_ring");
		if (uIToggle != null && uIToggle.value && isStarted)
		{
			SoundManager.SePlay("sys_page_skip");
		}
	}

	public void OnRsringTabChange()
	{
		SetCurrentType(ServerEexchangeType.RSRING);
		SetShopBtnObj(true);
		UIToggle uIToggle = GameObjectUtil.FindChildGameObjectComponent<UIToggle>(base.gameObject.transform.root.gameObject, "Btn_tab_rsring");
		if (uIToggle != null && uIToggle.value && isStarted)
		{
			SoundManager.SePlay("sys_page_skip");
		}
	}

	public void OnChallengeTabChange()
	{
		SetCurrentType(ServerEexchangeType.CHALLENGE);
		SetShopBtnObj(true);
		UIToggle uIToggle = GameObjectUtil.FindChildGameObjectComponent<UIToggle>(base.gameObject.transform.root.gameObject, "Btn_tab_challenge");
		if (uIToggle != null && uIToggle.value && isStarted)
		{
			SoundManager.SePlay("sys_page_skip");
		}
	}

	public void OnRaidbossEnergyTabChange()
	{
		SetShopBtnObj(false);
		UIToggle uIToggle = GameObjectUtil.FindChildGameObjectComponent<UIToggle>(base.gameObject.transform.root.gameObject, "Btn_tab_raidboss");
		if (uIToggle != null && uIToggle.value && isStarted)
		{
			SoundManager.SePlay("sys_page_skip");
		}
	}

	public static string GetText(string cellName, Dictionary<string, string> dicReplaces = null)
	{
		string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Shop", cellName).text;
		if (dicReplaces != null)
		{
			text = TextUtility.Replaces(text, dicReplaces);
		}
		return text;
	}

	[Conditional("DEBUG_INFO")]
	private static void DebugLog(List<ServerRedStarItemState> itemList)
	{
		foreach (ServerRedStarItemState item in itemList)
		{
		}
	}

	[Conditional("DEBUG_INFO")]
	private static void DebugLog(string s)
	{
		Debug.Log("@ms " + s);
	}

	[Conditional("DEBUG_INFO")]
	private static void DebugLogWarning(string s)
	{
		Debug.LogWarning("@ms " + s);
	}

	public void OnApplicationPause(bool flag)
	{
		if (m_freeGetButton != null)
		{
			m_freeGetButton.SetActive(false);
		}
	}

	private void PurchaseSuccessCallback(bool isSuccess)
	{
		if (isSuccess)
		{
			SetExchangeItems(ServerEexchangeType.RSRING, ServerInterface.RedStarItemList);
			UpdateView();
		}
	}

	public static bool isRaidbossEvent()
	{
		bool result = false;
		EventManager instance = EventManager.Instance;
		if (instance != null && instance.Type == EventManager.EventType.RAID_BOSS)
		{
			result = instance.IsChallengeEvent();
		}
		return result;
	}
}
