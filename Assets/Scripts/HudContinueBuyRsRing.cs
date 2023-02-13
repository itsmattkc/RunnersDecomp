using AnimationOrTween;
using DataTable;
using Message;
using System.Collections;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class HudContinueBuyRsRing : MonoBehaviour
{
	private enum State
	{
		IDLE,
		WAIT_CLICK_BUTTON,
		PURCHASING,
		SHOW_RESULT
	}

	private enum Result
	{
		NONE = -1,
		SUCCESS,
		FAILED,
		CANCELED
	}

	private bool m_isEndPlay;

	private State m_state;

	private Result m_result = Result.NONE;

	private static readonly int ProductIndex;

	public ServerCampaignData m_campaignData;

	private List<Constants.Campaign.emType> CampaignTypeList = new List<Constants.Campaign.emType>
	{
		Constants.Campaign.emType.PurchaseAddRsrings,
		Constants.Campaign.emType.PurchaseAddRsringsNoChargeUser
	};

	public bool IsEndPlay
	{
		get
		{
			return m_isEndPlay;
		}
		private set
		{
		}
	}

	public bool IsSuccess
	{
		get
		{
			return m_result == Result.SUCCESS;
		}
	}

	public bool IsFailed
	{
		get
		{
			return m_result == Result.FAILED;
		}
	}

	public bool IsCanceled
	{
		get
		{
			return m_result == Result.CANCELED;
		}
	}

	public void Setup()
	{
		UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(base.gameObject, "Btn_item_rs_1");
		if (uIButtonMessage != null)
		{
			uIButtonMessage.target = base.gameObject;
			uIButtonMessage.functionName = "OnClickBuyButton";
		}
		UIButtonMessage uIButtonMessage2 = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(base.gameObject, "Btn_shop_legal");
		if (uIButtonMessage2 != null)
		{
			uIButtonMessage2.target = base.gameObject;
			uIButtonMessage2.functionName = "OnClickTradeLowButton";
		}
		UIButtonMessage uIButtonMessage3 = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(base.gameObject, "Btn_close");
		if (uIButtonMessage3 != null)
		{
			uIButtonMessage3.target = base.gameObject;
			uIButtonMessage3.functionName = "OnClickCloseButton";
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "img_sale_icon_1");
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
		NativeObserver instance = NativeObserver.Instance;
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (instance != null && loggedInServerInterface != null)
		{
			FinishedToGetProductList(NativeObserver.IAPResult.ProductsRequestCompleted);
		}
	}

	public void PlayStart()
	{
		base.gameObject.SetActive(true);
		Animation component = base.gameObject.GetComponent<Animation>();
		if (component != null)
		{
			ActiveAnimation.Play(component, Direction.Forward);
		}
		ServerCampaignData serverCampaignData = null;
		int num = 10;
		int num2 = num;
		if (GameObject.Find("ServerInterface") != null && ServerInterface.RedStarItemList != null && ServerInterface.RedStarItemList.Count > 0)
		{
			ServerRedStarItemState serverRedStarItemState = ServerInterface.RedStarItemList[ProductIndex];
			if (serverRedStarItemState != null)
			{
				foreach (Constants.Campaign.emType campaignType in CampaignTypeList)
				{
					int storeItemId = serverRedStarItemState.m_storeItemId;
					serverCampaignData = ServerInterface.CampaignState.GetCampaignInSession(campaignType, storeItemId);
					if (serverCampaignData != null)
					{
						break;
					}
				}
				float num3 = ServerCampaignData.fContentBasis;
				if (serverCampaignData != null)
				{
					num3 = serverCampaignData.iContent;
				}
				num2 = serverRedStarItemState.m_numItem;
				num = (int)((float)num2 * num3 / ServerCampaignData.fContentBasis);
			}
		}
		bool flag = (serverCampaignData != null) ? true : false;
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "img_sale_icon_1");
		if (gameObject != null)
		{
			gameObject.SetActive(flag);
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "img_bonus_icon_bg_1");
		if (gameObject2 != null)
		{
			gameObject2.SetActive(flag);
		}
		if (flag)
		{
			GameObject gameObject3 = GameObjectUtil.FindChildGameObject(base.gameObject, "img_sale_icon_bg_1");
			if (gameObject3 != null)
			{
				gameObject3.SetActive(true);
				UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject3, "Lbl_rs_gift_1");
				if (uILabel != null)
				{
					int num4 = num - num2;
					TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Shop", "label_rsring_bonus");
					if (text != null)
					{
						text.ReplaceTag("{COUNT}", HudUtility.GetFormatNumString(num4));
						uILabel.text = text.text;
						UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(uILabel.gameObject, "Lbl_bonus_icon_sdw_1");
						if (uILabel2 != null)
						{
							uILabel2.text = text.text;
						}
					}
				}
			}
		}
		else
		{
			GameObject gameObject4 = GameObjectUtil.FindChildGameObject(base.gameObject, "img_sale_icon_bg_1");
			if (gameObject4 != null)
			{
				gameObject4.SetActive(false);
			}
		}
		UILabel uILabel3 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_rs_quantity_1");
		if (uILabel3 != null)
		{
			uILabel3.text = num2.ToString();
		}
		m_isEndPlay = false;
		m_state = State.WAIT_CLICK_BUTTON;
		m_result = Result.NONE;
	}

	private void Start()
	{
	}

	private void Update()
	{
		switch (m_state)
		{
		case State.SHOW_RESULT:
			if (GeneralWindow.IsOkButtonPressed)
			{
				GeneralWindow.Close();
			}
			break;
		}
		if ((GeneralWindow.IsCreated("RsrBuyLegal") || GeneralWindow.IsCreated("AgeVerificationError")) && GeneralWindow.IsButtonPressed)
		{
			GeneralWindow.Close();
		}
	}

	private void FinishedToGetProductList(NativeObserver.IAPResult result)
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "item_1");
		if (gameObject == null)
		{
			return;
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_price_1");
		if (uILabel == null)
		{
			return;
		}
		NativeObserver instance = NativeObserver.Instance;
		if (!(instance == null))
		{
			string productPrice = instance.GetProductPrice(instance.GetProductName(ProductIndex));
			if (productPrice != null)
			{
				uILabel.text = productPrice;
			}
		}
	}

	private void OnClickBuyButton()
	{
		SoundManager.SePlay("sys_menu_decide");
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			ServerSettingState settingState = ServerInterface.SettingState;
			NativeObserver instance = NativeObserver.Instance;
			string productName = instance.GetProductName(ProductIndex);
			int mixedStringToInt = HudUtility.GetMixedStringToInt(instance.GetProductPrice(productName));
			if (!HudUtility.CheckPurchaseOver(settingState.m_birthday, settingState.m_monthPurchase, mixedStringToInt))
			{
				instance.BuyProduct(productName, PurchaseSuccessCallback, PurchaseFailedCallback, PurchaseCanceledCallback);
				return;
			}
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "AgeVerificationError";
			info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Shop", "gw_age_verification_error_caption").text;
			info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Shop", "gw_age_verification_error_text").text;
			info.anchor_path = "Camera/Anchor_5_MC";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			GeneralWindow.Create(info);
		}
	}

	private void OnClickTradeLowButton()
	{
		StartCoroutine(OpenLegalWindow());
	}

	private IEnumerator OpenLegalWindow()
	{
		SoundManager.SePlay("sys_menu_decide");
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
			Object.Destroy(htmlParserGameObject);
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "RsrBuyLegal";
			info.anchor_path = "Camera/Anchor_5_MC";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "Title", "gw_legal_caption").text;
			info.message = legalText;
			GeneralWindow.Create(info);
		}
	}

	private void OnClickCloseButton()
	{
		SoundManager.SePlay("sys_window_close");
		Animation component = base.gameObject.GetComponent<Animation>();
		if (component != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(base.animation, Direction.Reverse);
			if (activeAnimation != null)
			{
				EventDelegate.Add(activeAnimation.onFinished, OnFinishedAnimationCallbck, true);
			}
		}
	}

	public void OnPushBackKey()
	{
		OnClickCloseButton();
	}

	private void PurchaseSuccessCallback(int result)
	{
		SoundManager.SePlay("sys_buy_real_money");
		Debug.Log("HudContinue.PurchaseSuccessCallback");
		string replaceString = "1";
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			ServerRedStarItemState serverRedStarItemState = ServerInterface.RedStarItemList[ProductIndex];
			if (serverRedStarItemState != null)
			{
				replaceString = serverRedStarItemState.m_numItem.ToString();
			}
		}
		TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Shop", "gw_purchase_success_text");
		if (text != null)
		{
			text.ReplaceTag("{COUNT}", replaceString);
		}
		int num = 0;
		if (ServerInterface.LoggedInServerInterface != null)
		{
			num = ServerInterface.RedStarItemList[ProductIndex].m_storeItemId;
			Debug.Log("HudContinueBuyRsRing.PurchaseSuccessCallback:result = " + num);
		}
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetRedStarExchangeList(0, base.gameObject);
		}
		else
		{
			ServerGetRedStarExchangeList_Succeeded(null);
		}
	}

	private void ServerGetRedStarExchangeList_Succeeded(MsgGetRedStarExchangeListSucceed msg)
	{
		Animation component = base.gameObject.GetComponent<Animation>();
		if (component != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(base.animation, Direction.Reverse);
			if (activeAnimation != null)
			{
				EventDelegate.Add(activeAnimation.onFinished, OnFinishedAnimationCallbck, true);
			}
		}
		m_state = State.SHOW_RESULT;
		m_result = Result.SUCCESS;
	}

	private void PurchaseFailedCallback(NativeObserver.FailStatus status)
	{
		string cellID = "gw_purchase_failed_caption";
		string cellID2 = "gw_purchase_failed_text";
		if (status == NativeObserver.FailStatus.Deferred)
		{
			cellID = "gw_purchase_deferred_caption";
			cellID2 = "gw_purchase_deferred_text";
		}
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.caption = TextUtility.GetCommonText("Shop", cellID);
		info.message = TextUtility.GetCommonText("Shop", cellID2);
		info.anchor_path = "Camera/Anchor_5_MC";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.isPlayErrorSe = true;
		GeneralWindow.Create(info);
		m_state = State.SHOW_RESULT;
		m_result = Result.FAILED;
	}

	private void PurchaseCanceledCallback()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.caption = TextUtility.GetCommonText("Shop", "gw_purchase_canceled_caption");
		info.message = TextUtility.GetCommonText("Shop", "gw_purchase_canceled_text");
		info.anchor_path = "Camera/Anchor_5_MC";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.isPlayErrorSe = true;
		GeneralWindow.Create(info);
		m_state = State.SHOW_RESULT;
		m_result = Result.CANCELED;
	}

	private void OnFinishedAnimationCallbck()
	{
		base.gameObject.SetActive(false);
		m_isEndPlay = true;
		m_state = State.IDLE;
		m_result = Result.CANCELED;
	}
}
