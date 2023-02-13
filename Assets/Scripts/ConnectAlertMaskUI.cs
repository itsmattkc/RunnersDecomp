using AnimationOrTween;
using DataTable;
using System;
using System.Collections.Generic;
using Text;
using UI;
using UnityEngine;

public class ConnectAlertMaskUI : MonoBehaviour
{
	public enum dispCategory
	{
		CHAO_INFO,
		TIPS_INFO,
		END
	}

	[SerializeField]
	private GameObject m_alertGameObject;

	[SerializeField]
	private GameObject m_eventObject;

	[SerializeField]
	private Animation m_screenAnimation;

	[SerializeField]
	private UISprite m_eventLogImg;

	private static ConnectAlertMaskUI s_instance;

	private GameObject m_chaoObject;

	private GameObject m_tipsObject;

	private UILabel m_lblName;

	private UILabel m_lblBonus;

	private UISprite m_imgBg;

	private UISprite m_imgIcon;

	private UITexture m_imgChao;

	private int m_chaoId = -1;

	private List<UIAtlas> m_atlas = new List<UIAtlas>();

	private Action m_onFinishedFadeOutCallbackAction;

	private void Awake()
	{
		s_instance = this;
		CheckChaoObject();
	}

	private void OnDestroy()
	{
		if (s_instance != null)
		{
			s_instance.RemoveAtlasList();
			s_instance.DeleteTexture();
		}
		s_instance = null;
	}

	public void SetChaoInfo()
	{
		CheckChaoObject();
		if (m_chaoObject != null)
		{
			if (m_tipsObject != null)
			{
				m_tipsObject.SetActive(false);
			}
			SetChaoInfoData();
		}
	}

	private void CheckChaoObject()
	{
		if (m_chaoObject == null)
		{
			m_chaoObject = GameObjectUtil.FindChildGameObject(base.gameObject, "info_chao");
			if (m_chaoObject != null)
			{
				m_lblName = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_chaoObject, "Lbl_chao_name");
				m_lblBonus = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_chaoObject, "Lbl_chao_bonus");
				m_imgBg = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_chaoObject, "img_chao_rank_bg");
				m_imgIcon = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_chaoObject, "img_chao_type");
				m_imgChao = GameObjectUtil.FindChildGameObjectComponent<UITexture>(m_chaoObject, "img_chao");
			}
		}
	}

	private bool SetChaoInfoData()
	{
		int loadingChaoId = ChaoTextureManager.Instance.LoadingChaoId;
		ChaoData chaoData = ChaoTable.GetChaoData(loadingChaoId);
		Debug.Log("ConnectAlertMaskUI:SetChaoInfoData  DisplayChaoID = " + loadingChaoId);
		if (chaoData != null)
		{
			SetEventBanner();
			if (m_chaoObject != null)
			{
				m_chaoObject.SetActive(true);
			}
			m_lblName.text = chaoData.name;
			int chaoLevel = ChaoTable.ChaoMaxLevel();
			m_lblBonus.text = chaoData.GetLoadingPageDetailLevelPlusSP(chaoLevel);
			if (!string.IsNullOrEmpty(m_lblBonus.text))
			{
				UILabel lblBonus = m_lblBonus;
				lblBonus.text = lblBonus.text + "\n" + TextUtility.GetChaoText("Chao", "level_max");
			}
			switch (chaoData.rarity)
			{
			case ChaoData.Rarity.NORMAL:
				m_imgBg.spriteName = "ui_chao_set_bg_load_0";
				break;
			case ChaoData.Rarity.RARE:
				m_imgBg.spriteName = "ui_chao_set_bg_load_1";
				break;
			case ChaoData.Rarity.SRARE:
				m_imgBg.spriteName = "ui_chao_set_bg_load_2";
				break;
			}
			switch (chaoData.charaAtribute)
			{
			case CharacterAttribute.SPEED:
				m_imgIcon.spriteName = "ui_chao_set_type_icon_speed";
				break;
			case CharacterAttribute.POWER:
				m_imgIcon.spriteName = "ui_chao_set_type_icon_power";
				break;
			case CharacterAttribute.FLY:
				m_imgIcon.spriteName = "ui_chao_set_type_icon_fly";
				break;
			}
			if (m_imgChao != null)
			{
				m_chaoId = ChaoWindowUtility.GetIdFromServerId(chaoData.id + 400000);
				ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(m_imgChao, null, true);
				ChaoTextureManager.Instance.GetTexture(m_chaoId, info);
				m_imgChao.enabled = true;
			}
			return true;
		}
		Debug.Log("ConnectAlertMaskUI:SetChaoInfoData  ChaoInfoData is Null!!");
		if (m_chaoObject != null)
		{
			m_chaoObject.SetActive(false);
		}
		return false;
	}

	public void SetTipsInfo()
	{
		CheckTipsObject();
		if (m_tipsObject != null)
		{
			if (m_chaoObject != null)
			{
				m_chaoObject.SetActive(false);
			}
			SetTipsInfoData();
		}
	}

	private void CheckTipsObject()
	{
		m_tipsObject = GameObjectUtil.FindChildGameObject(base.gameObject, "info_tips");
	}

	private void SetTipsInfoData()
	{
		SetEventBanner();
		m_tipsObject.SetActive(true);
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_tipsObject, "Lbl_tips_body");
		if (uILabel != null)
		{
			int categoryCellCount = TextManager.GetCategoryCellCount(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Tips");
			string text = string.Empty;
			if (categoryCellCount > 0)
			{
				int num = UnityEngine.Random.Range(1, categoryCellCount);
				string cellID = "tips_message_" + num;
				text = TextUtility.GetCommonText("Tips", cellID);
			}
			if (text != null)
			{
				uILabel.text = text;
			}
		}
	}

	public void SetTipsCategory(dispCategory category)
	{
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_tips_category");
		if (uILabel != null)
		{
			if (category == dispCategory.CHAO_INFO)
			{
				uILabel.text = TextUtility.GetCommonText("MainMenu", "loading_chaoInfo_caption");
				SetChaoInfo();
			}
			else
			{
				uILabel.text = TextUtility.GetCommonText("MainMenu", "loading_tipsInfo_caption");
				SetTipsInfo();
			}
		}
	}

	public void PlayReverse()
	{
		ActiveAnimation activeAnimation = ActiveAnimation.Play(m_screenAnimation, Direction.Reverse);
		if (activeAnimation != null)
		{
			EventDelegate.Add(activeAnimation.onFinished, OnFinishedAnimation, true);
		}
	}

	private void SetEventBanner()
	{
		bool eventObject = false;
		if (EventManager.Instance != null && EventManager.Instance.Type != EventManager.EventType.UNKNOWN)
		{
			eventObject = EventManager.Instance.IsInEvent();
			if (m_eventObject != null && EventManager.Instance.Type == EventManager.EventType.ADVERT)
			{
				string advertEventTitleText = GetAdvertEventTitleText();
				UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_eventObject, "ui_Lbl_event_caption");
				if (uILabel != null)
				{
					uILabel.text = advertEventTitleText;
				}
				UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_eventObject, "ui_Lbl_event_caption_sh");
				if (uILabel2 != null)
				{
					uILabel2.text = advertEventTitleText;
				}
				UILocalizeText uILocalizeText = GameObjectUtil.FindChildGameObjectComponent<UILocalizeText>(m_eventObject, "ui_Lbl_event_caption");
				if (uILocalizeText != null)
				{
					uILocalizeText.enabled = false;
				}
				UILocalizeText uILocalizeText2 = GameObjectUtil.FindChildGameObjectComponent<UILocalizeText>(m_eventObject, "ui_Lbl_event_caption_sh");
				if (uILocalizeText2 != null)
				{
					uILocalizeText2.enabled = false;
				}
			}
		}
		SetEventObject(eventObject);
	}

	private string GetAdvertEventTitleText()
	{
		string result = string.Empty;
		if (EventManager.Instance != null)
		{
			switch (EventManager.Instance.AdvertType)
			{
			case EventManager.AdvertEventType.ROULETTE:
			{
				EyeCatcherCharaData[] eyeCatcherCharaDatas = EventManager.Instance.GetEyeCatcherCharaDatas();
				if (eyeCatcherCharaDatas != null)
				{
					bool flag = false;
					EyeCatcherCharaData[] array = eyeCatcherCharaDatas;
					foreach (EyeCatcherCharaData eyeCatcherCharaData in array)
					{
						if (eyeCatcherCharaData.id >= -1 && eyeCatcherCharaData.id != 0)
						{
							flag = true;
							break;
						}
					}
					result = ((!flag) ? TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Common", "ui_Lbl_word_header_event_roulette_o") : TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Common", "ui_Lbl_word_header_event_roulette_c"));
				}
				else
				{
					result = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Common", "ui_Lbl_word_header_event_roulette_o");
				}
				break;
			}
			case EventManager.AdvertEventType.CHARACTER:
				result = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Common", "ui_Lbl_word_header_event_character");
				break;
			case EventManager.AdvertEventType.SHOP:
				result = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Common", "ui_Lbl_word_header_event_shop");
				break;
			}
		}
		return result;
	}

	public void SetEventObject(bool enabled)
	{
		if (!(m_eventObject != null))
		{
			return;
		}
		if (enabled && AtlasManager.Instance != null)
		{
			UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_eventObject, "bg_deco");
			if (uISprite != null)
			{
				m_atlas.Add(uISprite.atlas);
			}
			if (m_eventLogImg != null)
			{
				m_atlas.Add(m_eventLogImg.atlas);
			}
			AtlasManager.Instance.ReplaceAtlasForMenuLoading(m_atlas.ToArray());
		}
		m_eventObject.SetActive(enabled);
	}

	private void OnFinishedAnimation()
	{
		if (m_onFinishedFadeOutCallbackAction != null)
		{
			m_onFinishedFadeOutCallbackAction();
		}
		DeleteTexture();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnSetChaoTexture(ChaoTextureManager.TextureData data)
	{
		if (m_chaoId == data.chao_id && m_imgChao != null)
		{
			m_imgChao.enabled = true;
			m_imgChao.mainTexture = data.tex;
		}
	}

	private void RemoveAtlasList()
	{
		if (m_eventObject != null)
		{
			UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_eventObject, "bg_deco");
			if (uISprite != null)
			{
				uISprite.atlas = null;
			}
		}
		if (m_eventLogImg != null)
		{
			m_eventLogImg.atlas = null;
		}
		m_atlas.Clear();
	}

	private void DeleteTexture()
	{
		if (m_imgChao != null && m_imgChao.mainTexture != null)
		{
			m_imgChao.mainTexture = null;
		}
		if (m_eventLogImg != null)
		{
		}
		if (m_chaoId > 0)
		{
			ChaoTextureManager.Instance.RemoveChaoTexture(m_chaoId);
		}
		m_chaoId = -1;
	}

	public static void StartScreen()
	{
		if (s_instance != null)
		{
			s_instance.m_alertGameObject.SetActive(true);
			dispCategory dispCategory = dispCategory.CHAO_INFO;
			s_instance.SetTipsCategory(dispCategory);
			Debug.Log("ConnectAlertMaskUI:StartScreen  dispCategory = " + dispCategory);
		}
	}

	public static void EndScreen(Action onFinishedFadeOutCallbackAction = null)
	{
		if (s_instance != null)
		{
			s_instance.SetEventObject(false);
			s_instance.m_onFinishedFadeOutCallbackAction = onFinishedFadeOutCallbackAction;
			s_instance.PlayReverse();
		}
	}
}
