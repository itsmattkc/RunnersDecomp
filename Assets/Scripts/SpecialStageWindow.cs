using AnimationOrTween;
using DataTable;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class SpecialStageWindow : EventWindowBase
{
	private enum BUTTON_ACT
	{
		CLOSE,
		PLAY,
		INFO,
		RANK,
		ROULETTE,
		SHOP_RSRING,
		SHOP_RING,
		SHOP_CHALLENGE,
		NONE
	}

	private enum Mode
	{
		Idle,
		Wait,
		End
	}

	public const RankingUtil.RankingRankerType SPECILA_RANKING_RANKER_TYPE = RankingUtil.RankingRankerType.SP_ALL;

	public const int DRAW_RANKER_MAX = 10;

	private const string LBL_CAPTION = "Lbl_caption";

	private const string LBL_CAPTION_SH = "Lbl_caption_sh";

	private const string LBL_PAGE = "Lbl_page";

	private const string IMG_CHAO_BG = "img_chao_bg";

	private const string IMG_CHAO_TYPE = "img_type_icon";

	private const string IMG_CHAO_SPRITE = "sprite_chao";

	private const string TEX_CHAO = "Tex_chao";

	private const string GO_SNS = "facebook_top";

	private const string GO_PATTERN_0 = "pattern_0";

	private const string GO_PATTERN_1 = "pattern_1";

	private const string GO_PARTS = "ranking_cmn_parts";

	private const string GO_BTN_ALL = "Btn_all";

	private const string GO_BTN_FRIEND = "Btn_friend";

	private const string GO_BTN_BACK = "Btn_cmn_back";

	private const string GO_BTN_REWARD = "Btn_Reward";

	[SerializeField]
	[Header("読み込み中表示")]
	private GameObject m_loading;

	[SerializeField]
	private UITexture m_bgTexture;

	private SpecialStageInfo m_spInfoData;

	private Mode m_mode;

	[SerializeField]
	private Animation m_animation;

	private static SocialInterface s_socialInterface;

	private bool m_close;

	private bool m_alertCollider;

	private BUTTON_ACT m_btnAct = BUTTON_ACT.NONE;

	private GameObject m_btnOption;

	private GameObject m_btnMore;

	private UIRectItemStorage m_myDataArea;

	private UIRectItemStorage m_topRankerArea;

	private UIRectItemStorageRanking m_listArea;

	private RankingUtil.RankingRankerType m_currentRankerType = RankingUtil.RankingRankerType.SP_ALL;

	private RankingUtil.RankingScoreType m_currentScoreType = RankingUtil.RankingScoreType.TOTAL_SCORE;

	private List<RankingUtil.Ranker> m_currentRankerList;

	private int m_page;

	private bool m_pageNext;

	private bool m_pagePrev;

	private float m_rankingInitloadingTime;

	private UIDraggablePanel m_mainListPanel;

	private bool m_facebookLock;

	private bool m_toggleLock;

	private bool m_opened;

	private EasySnsFeed m_easySnsFeed;

	private static SpecialStageWindow s_instance;

	public SpecialStageInfo infoData
	{
		get
		{
			return m_spInfoData;
		}
	}

	public static SpecialStageWindow Instance
	{
		get
		{
			return s_instance;
		}
	}

	public void SetLoadingObject()
	{
		if (m_loading != null)
		{
			m_loading.SetActive(true);
		}
	}

	public bool IsInitLoading()
	{
		if (m_loading != null)
		{
			return m_loading.activeSelf;
		}
		return false;
	}

	public RankingUtil.Ranker GetCurrentRanker(int slot)
	{
		RankingUtil.Ranker result = null;
		if (m_currentRankerList != null && slot >= 0 && m_currentRankerList.Count > slot + 1)
		{
			result = m_currentRankerList[slot + 1];
		}
		return result;
	}

	protected override void SetObject()
	{
		if (m_isSetObject)
		{
			return;
		}
		base.gameObject.transform.localPosition = default(Vector3);
		if (m_animation == null)
		{
			m_animation = base.gameObject.GetComponent<Animation>();
		}
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		List<string> list3 = new List<string>();
		List<string> list4 = new List<string>();
		list.Add("Lbl_caption");
		list.Add("Lbl_caption_sh");
		list.Add("Lbl_page");
		list2.Add("img_chao_bg");
		list2.Add("img_type_icon");
		list2.Add("sprite_chao");
		list3.Add("Tex_chao");
		list4.Add("facebook_top");
		list4.Add("pattern_0");
		list4.Add("pattern_1");
		list4.Add("ranking_cmn_parts");
		list4.Add("Btn_all");
		list4.Add("Btn_friend");
		list4.Add("Btn_cmn_back");
		list4.Add("Btn_Reward");
		m_objectLabels = ObjectUtility.GetObjectLabel(base.gameObject, list);
		m_objectSprites = ObjectUtility.GetObjectSprite(base.gameObject, list2);
		m_objectTextures = ObjectUtility.GetObjectTexture(base.gameObject, list3);
		m_objects = ObjectUtility.GetGameObject(base.gameObject, list4);
		if (m_objects != null)
		{
			if (m_objects.ContainsKey("pattern_0") && m_objects["pattern_0"] != null)
			{
				m_btnMore = GameObjectUtil.FindChildGameObject(m_objects["pattern_0"], "Btn_PageDown");
				m_myDataArea = GameObjectUtil.FindChildGameObjectComponent<UIRectItemStorage>(m_objects["pattern_0"], "row_0");
				m_topRankerArea = GameObjectUtil.FindChildGameObjectComponent<UIRectItemStorage>(m_objects["pattern_0"], "row_1");
			}
			if (m_objects.ContainsKey("pattern_1") && m_objects["pattern_1"] != null)
			{
				m_listArea = GameObjectUtil.FindChildGameObjectComponent<UIRectItemStorageRanking>(m_objects["pattern_1"], "slot");
				m_mainListPanel = GameObjectUtil.FindChildGameObjectComponent<UIDraggablePanel>(m_objects["pattern_1"], "Panel_alpha_clip");
			}
			m_btnOption = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_option");
		}
		UIPlayAnimation uIPlayAnimation = GameObjectUtil.FindChildGameObjectComponent<UIPlayAnimation>(base.gameObject, "blinder");
		UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(base.gameObject, "blinder");
		if (uIPlayAnimation != null && uIButtonMessage != null)
		{
			uIPlayAnimation.enabled = false;
			uIButtonMessage.enabled = false;
		}
		m_currentRankerType = RankingUtil.RankingRankerType.SP_ALL;
		m_currentScoreType = RankingManager.EndlessSpecialRankingScoreType;
		m_page = 0;
		m_isSetObject = true;
		m_facebookLock = true;
		if (RegionManager.Instance != null)
		{
			m_facebookLock = !RegionManager.Instance.IsUseSNS();
		}
		if (m_objects != null && m_objects.ContainsKey("Btn_friend") && m_objects["Btn_friend"] != null)
		{
			UIImageButton component = m_objects["Btn_friend"].GetComponent<UIImageButton>();
			if (component != null)
			{
				component.isEnabled = !m_facebookLock;
			}
		}
	}

	public void Setup(SpecialStageInfo info)
	{
		m_toggleLock = false;
		m_opened = false;
		SetAlertSimpleUI(true);
		SetObject();
		s_socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		base.enabledAnchorObjects = true;
		m_mode = Mode.Idle;
		m_close = false;
		m_rankingInitloadingTime = 0f;
		Texture bGTexture = EventUtility.GetBGTexture();
		if (bGTexture != null && m_bgTexture != null)
		{
			m_bgTexture.mainTexture = bGTexture;
		}
		if (m_animation != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_eventmenu_intro", Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, WindowAnimationFinishCallback, true);
			Debug.Log("Call hogehogehoge");
		}
		if (info != null)
		{
			m_spInfoData = info;
			m_objectLabels["Lbl_caption"].text = m_spInfoData.eventCaption;
			m_objectLabels["Lbl_caption_sh"].text = m_spInfoData.eventCaption;
			ChaoData rewardChao = m_spInfoData.GetRewardChao();
			if (rewardChao != null && m_objectSprites["img_chao_bg"] != null && m_objectSprites["img_type_icon"] != null)
			{
				switch (rewardChao.rarity)
				{
				case ChaoData.Rarity.NORMAL:
					m_objectSprites["img_chao_bg"].spriteName = "ui_tex_chao_bg_0";
					break;
				case ChaoData.Rarity.RARE:
					m_objectSprites["img_chao_bg"].spriteName = "ui_tex_chao_bg_1";
					break;
				case ChaoData.Rarity.SRARE:
					m_objectSprites["img_chao_bg"].spriteName = "ui_tex_chao_bg_2";
					break;
				default:
					m_objectSprites["img_chao_bg"].spriteName = "ui_tex_chao_bg_x";
					break;
				}
				switch (rewardChao.charaAtribute)
				{
				case CharacterAttribute.SPEED:
					m_objectSprites["img_type_icon"].spriteName = "ui_chao_set_type_icon_speed";
					break;
				case CharacterAttribute.FLY:
					m_objectSprites["img_type_icon"].spriteName = "ui_chao_set_type_icon_fly";
					break;
				case CharacterAttribute.POWER:
					m_objectSprites["img_type_icon"].spriteName = "ui_chao_set_type_icon_power";
					break;
				}
				if (m_objectTextures["Tex_chao"] != null)
				{
					ChaoTextureManager.CallbackInfo info2 = new ChaoTextureManager.CallbackInfo(m_objectTextures["Tex_chao"], null, true);
					ChaoTextureManager.Instance.GetTexture(rewardChao.id, info2);
				}
			}
			GeneralUtil.SetRouletteBtnIcon(base.gameObject);
		}
		m_page = 0;
		m_currentRankerType = RankingUtil.RankingRankerType.SP_ALL;
		m_currentScoreType = RankingManager.EndlessSpecialRankingScoreType;
		Debug.Log("SpecialStageWindow  Setup!");
		ResetRankerList(m_page, m_currentRankerType);
		SetLoadingObject();
		RankingManager instance = SingletonGameObject<RankingManager>.Instance;
		if (instance != null && !instance.isLoading && instance.IsRankingTop(RankingUtil.RankingMode.ENDLESS, RankingManager.EndlessSpecialRankingScoreType, RankingUtil.RankingRankerType.SP_ALL))
		{
			SetRanking(RankingUtil.RankingRankerType.SP_ALL, RankingManager.EndlessSpecialRankingScoreType, -1);
		}
		BackKeyManager.AddWindowCallBack(base.gameObject);
		base.enabledAnchorObjects = true;
		m_mode = Mode.Wait;
		if (m_objects != null && m_objects.ContainsKey("Btn_all") && m_objects["Btn_all"] != null)
		{
			UIToggle uIToggle = GameObjectUtil.FindChildGameObjectComponent<UIToggle>(base.gameObject, "Btn_all");
			if (uIToggle != null && !uIToggle.value)
			{
				m_toggleLock = true;
				uIToggle.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
				m_toggleLock = false;
			}
		}
	}

	private void OnSetChaoTexture(ChaoTextureManager.TextureData data)
	{
		if (data != null && data.tex != null)
		{
			m_objectTextures["Tex_chao"].mainTexture = data.tex;
			m_objectTextures["Tex_chao"].alpha = 1f;
			if (m_objectSprites["sprite_chao"] != null)
			{
				m_objectSprites["sprite_chao"].alpha = 0f;
			}
		}
	}

	public bool SetRanking(RankingUtil.RankingRankerType type, RankingUtil.RankingScoreType score, int page)
	{
		bool flag = false;
		if (page == -1 || m_currentRankerType != type || m_currentScoreType != score || m_page != page)
		{
			RankingManager instance = SingletonGameObject<RankingManager>.Instance;
			if (instance != null && !instance.isLoading)
			{
				if (page <= 0)
				{
					ResetRankerList(0, type);
					ResetRankerList(1, type);
				}
				m_page = page;
				m_currentRankerType = type;
				m_currentScoreType = score;
				if (m_page < 0)
				{
					m_page = 0;
				}
				if (s_socialInterface != null)
				{
					if (type == RankingUtil.RankingRankerType.SP_FRIEND && !s_socialInterface.IsLoggedIn)
					{
						if (m_objects.ContainsKey("facebook_top") && m_objects["facebook_top"] != null)
						{
							m_objects["facebook_top"].SetActive(true);
							ResetRankerList(0, type);
							return true;
						}
					}
					else if (type == RankingUtil.RankingRankerType.SP_FRIEND && s_socialInterface.IsLoggedIn)
					{
						if (m_objects.ContainsKey("facebook_top") && m_objects["facebook_top"] != null)
						{
							m_objects["facebook_top"].SetActive(false);
						}
						if (m_btnOption != null)
						{
							m_btnOption.SetActive(true);
						}
					}
					else
					{
						if (m_objects.ContainsKey("facebook_top") && m_objects["facebook_top"] != null)
						{
							m_objects["facebook_top"].SetActive(false);
						}
						if (m_btnOption != null)
						{
							m_btnOption.SetActive(false);
						}
					}
				}
				else if (type == RankingUtil.RankingRankerType.SP_FRIEND)
				{
					if (m_objects.ContainsKey("facebook_top") && m_objects["facebook_top"] != null)
					{
						m_objects["facebook_top"].SetActive(true);
						ResetRankerList(0, type);
						return true;
					}
				}
				else if (m_objects.ContainsKey("facebook_top") && m_objects["facebook_top"] != null)
				{
					m_objects["facebook_top"].SetActive(false);
				}
				if (m_btnMore != null)
				{
					m_btnMore.SetActive(false);
				}
				return instance.GetRanking(RankingUtil.RankingMode.ENDLESS, score, type, m_page, CallbackRanking);
			}
		}
		return true;
	}

	private void CallbackRanking(List<RankingUtil.Ranker> rankerList, RankingUtil.RankingScoreType score, RankingUtil.RankingRankerType type, int page, bool isNext, bool isPrev, bool isCashData)
	{
		if (m_currentRankerType != type || m_currentScoreType != score)
		{
			return;
		}
		if (m_loading != null)
		{
			m_loading.SetActive(false);
		}
		m_pageNext = isNext;
		m_pagePrev = isPrev;
		SetRankerList(rankerList, type, page);
		if (m_mainListPanel != null && page <= 1)
		{
			if (type == RankingUtil.RankingRankerType.RIVAL)
			{
				GameObject myDataGameObject = m_listArea.GetMyDataGameObject();
				if (myDataGameObject != null)
				{
					Vector3 localPosition = myDataGameObject.transform.localPosition;
					float num = localPosition.y * -1f - 367f;
					if (num < -1.25f)
					{
						num = -1.25f;
					}
					Transform transform = m_mainListPanel.transform;
					Vector3 localPosition2 = m_mainListPanel.transform.localPosition;
					float x = localPosition2.x;
					float y = num;
					Vector3 localPosition3 = m_mainListPanel.transform.localPosition;
					transform.localPosition = new Vector3(x, y, localPosition3.z);
					UIPanel panel = m_mainListPanel.panel;
					Vector4 clipRange = m_mainListPanel.panel.clipRange;
					float x2 = clipRange.x;
					float y2 = 0f - num;
					Vector4 clipRange2 = m_mainListPanel.panel.clipRange;
					float z = clipRange2.z;
					Vector4 clipRange3 = m_mainListPanel.panel.clipRange;
					panel.clipRange = new Vector4(x2, y2, z, clipRange3.w);
					m_listArea.CheckItemDrawAll(true);
				}
				else
				{
					m_mainListPanel.Scroll(0f);
					m_mainListPanel.ResetPosition();
				}
			}
			else
			{
				m_mainListPanel.Scroll(0f);
				m_mainListPanel.ResetPosition();
			}
		}
		if (isNext && m_btnMore != null)
		{
			m_btnMore.SetActive(true);
		}
	}

	private void ResetRankerList(int page, RankingUtil.RankingRankerType type)
	{
		if (m_page > 1)
		{
			return;
		}
		if (page > 0 || type == RankingUtil.RankingRankerType.RIVAL)
		{
			if (m_objects.ContainsKey("pattern_0") && m_objects["pattern_0"] != null)
			{
				m_objects["pattern_0"].SetActive(false);
			}
			if (m_objects.ContainsKey("pattern_1") && m_objects["pattern_1"] != null)
			{
				m_objects["pattern_1"].SetActive(true);
			}
		}
		else
		{
			if (m_objects.ContainsKey("pattern_0") && m_objects["pattern_0"] != null)
			{
				m_objects["pattern_0"].SetActive(true);
			}
			if (m_objects.ContainsKey("pattern_1") && m_objects["pattern_1"] != null)
			{
				m_objects["pattern_1"].SetActive(false);
			}
		}
		if (m_listArea != null)
		{
			m_listArea.Reset();
		}
		if (m_myDataArea != null)
		{
			m_myDataArea.maxItemCount = (m_myDataArea.maxRows = 0);
			m_myDataArea.Restart();
		}
		if (m_topRankerArea != null)
		{
			m_topRankerArea.maxItemCount = (m_topRankerArea.maxRows = 0);
			m_topRankerArea.Restart();
		}
	}

	private void SetRankerList(List<RankingUtil.Ranker> rankers, RankingUtil.RankingRankerType type, int page)
	{
		if (page > 0 || type == RankingUtil.RankingRankerType.RIVAL)
		{
			m_currentRankerList = rankers;
		}
		if (page > 0 || type == RankingUtil.RankingRankerType.RIVAL)
		{
			if (m_objects.ContainsKey("pattern_0") && m_objects["pattern_0"] != null)
			{
				m_objects["pattern_0"].SetActive(false);
			}
			if (m_objects.ContainsKey("pattern_1") && m_objects["pattern_1"] != null)
			{
				m_objects["pattern_1"].SetActive(true);
			}
			if (m_listArea != null)
			{
				if (page < 1)
				{
					m_listArea.Reset();
					AddRectItemStorageRanking(m_listArea, rankers, type);
				}
				else
				{
					if (page == 1)
					{
						m_listArea.Reset();
					}
					AddRectItemStorageRanking(m_listArea, rankers, type);
				}
			}
			if (m_myDataArea != null)
			{
				m_myDataArea.maxItemCount = (m_myDataArea.maxRows = 0);
				m_myDataArea.Restart();
			}
			if (m_topRankerArea != null)
			{
				m_topRankerArea.maxItemCount = (m_topRankerArea.maxRows = 0);
				m_topRankerArea.Restart();
			}
			return;
		}
		if (m_objects.ContainsKey("pattern_0") && m_objects["pattern_0"] != null)
		{
			m_objects["pattern_0"].SetActive(true);
		}
		if (m_objects.ContainsKey("pattern_1") && m_objects["pattern_1"] != null)
		{
			m_objects["pattern_1"].SetActive(false);
		}
		if (m_listArea != null)
		{
			m_listArea.Reset();
		}
		if (m_myDataArea != null)
		{
			if (rankers != null && rankers.Count > 0)
			{
				if (rankers[0] != null)
				{
					m_myDataArea.maxItemCount = (m_myDataArea.maxRows = 1);
					UpdateRectItemStorage(m_myDataArea, rankers, 0);
				}
				else
				{
					m_myDataArea.maxItemCount = (m_myDataArea.maxRows = 0);
					m_myDataArea.Restart();
				}
			}
			else
			{
				m_myDataArea.maxItemCount = (m_myDataArea.maxRows = 0);
				m_myDataArea.Restart();
			}
		}
		if (m_topRankerArea != null && rankers != null)
		{
			if (rankers.Count - 1 >= RankingManager.GetRankingMax(type, page))
			{
				m_topRankerArea.maxItemCount = (m_topRankerArea.maxRows = 3);
			}
			else
			{
				m_topRankerArea.maxItemCount = (m_topRankerArea.maxRows = rankers.Count - 1);
			}
			UpdateRectItemStorage(m_topRankerArea, rankers);
		}
	}

	private void AddRectItemStorageRanking(UIRectItemStorageRanking ui_rankers, List<RankingUtil.Ranker> rankerList, RankingUtil.RankingRankerType type)
	{
		int childCount = ui_rankers.childCount;
		int num = rankerList.Count - childCount;
		if (m_pageNext)
		{
			num--;
		}
		ui_rankers.rankingType = type;
		if (ui_rankers.callback == null)
		{
			ui_rankers.callback = CallbackItemStorageRanking;
			ui_rankers.callbackTopOrLast = CallbackItemStorageRankingTopOrLast;
		}
		ui_rankers.AddItem(num);
	}

	private bool CallbackItemStorageRankingTopOrLast(bool isTop)
	{
		bool result = false;
		RankingManager instance = SingletonGameObject<RankingManager>.Instance;
		if (instance != null && !instance.isLoading)
		{
			if (isTop)
			{
				if (m_pagePrev)
				{
					result = false;
				}
			}
			else if (m_pageNext)
			{
				result = instance.GetRankingScroll(RankingUtil.RankingMode.ENDLESS, true, CallbackRanking);
			}
		}
		return result;
	}

	private void CallbackItemStorageRanking(ui_ranking_scroll_dummy obj, UIRectItemStorageRanking storage)
	{
		if (!(obj != null) || m_currentRankerList == null)
		{
			return;
		}
		int num = obj.slot + 1;
		if (num > 0 && m_currentRankerList.Count > num)
		{
			RankingUtil.Ranker rankerData = m_currentRankerList[num];
			if (obj.myRankerData == null)
			{
				obj.myRankerData = m_currentRankerList[0];
			}
			obj.spWindow = this;
			obj.rankerData = rankerData;
			obj.rankerType = m_currentRankerType;
			obj.scoreType = m_currentScoreType;
			obj.SetActiveObject(storage.CheckItemDraw(obj.slot), 0f);
			obj.end = (obj.slot + 1 == m_currentRankerList.Count);
		}
		else
		{
			Object.Destroy(obj.gameObject);
		}
	}

	private void UpdateRectItemStorage(UIRectItemStorage ui_rankers, List<RankingUtil.Ranker> rankerList, int head = 1)
	{
		ui_rankers.Restart();
		ui_ranking_scroll[] componentsInChildren = ui_rankers.GetComponentsInChildren<ui_ranking_scroll>(true);
		for (int i = 0; i < ui_rankers.maxItemCount && i + head < rankerList.Count; i++)
		{
			RankingUtil.Ranker ranker = rankerList[i + head];
			if (ranker != null)
			{
				componentsInChildren[i].UpdateView(m_currentScoreType, m_currentRankerType, ranker, i == ui_rankers.maxItemCount - 1);
				bool myRanker = false;
				if (rankerList[0] != null && ranker.id == rankerList[0].id)
				{
					myRanker = true;
				}
				componentsInChildren[i].SetMyRanker(myRanker);
			}
		}
	}

	public bool IsEnd()
	{
		if (m_mode == Mode.Wait)
		{
			return false;
		}
		return true;
	}

	private void Update()
	{
		if (m_easySnsFeed != null)
		{
			switch (m_easySnsFeed.Update())
			{
			case EasySnsFeed.Result.COMPLETED:
				m_easySnsFeed = null;
				m_currentRankerList = null;
				ResetRankerList(0, m_currentRankerType);
				Debug.Log("SetRanking m_easySnsFeed sp!");
				SetRanking(RankingUtil.RankingRankerType.SP_FRIEND, RankingManager.EndlessSpecialRankingScoreType, -1);
				break;
			case EasySnsFeed.Result.FAILED:
				m_easySnsFeed = null;
				break;
			}
		}
		if (base.enabledAnchorObjects)
		{
			if (!(m_loading != null) || !m_loading.activeSelf)
			{
				return;
			}
			m_rankingInitloadingTime += Time.deltaTime;
			if (m_rankingInitloadingTime > 5f)
			{
				if (SingletonGameObject<RankingManager>.Instance != null)
				{
					SingletonGameObject<RankingManager>.Instance.Init(null);
				}
				m_rankingInitloadingTime = -20f;
			}
		}
		else
		{
			m_rankingInitloadingTime = 0f;
		}
	}

	public void OnClickNoButton()
	{
		SetAlertSimpleUI(true);
		m_btnAct = BUTTON_ACT.CLOSE;
		m_close = true;
		SoundManager.SePlay("sys_menu_decide");
		if (m_animation != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_eventmenu_outro", Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, WindowAnimationFinishCallback, true);
		}
	}

	public void OnClickNoBgButton()
	{
		SetAlertSimpleUI(true);
		m_btnAct = BUTTON_ACT.CLOSE;
		m_close = true;
		SoundManager.SePlay("sys_menu_decide");
		if (m_animation != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_eventmenu_outro", Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, WindowAnimationFinishCallback, true);
		}
	}

	public void OnClickPlayButton()
	{
		SetAlertSimpleUI(true);
		m_btnAct = BUTTON_ACT.PLAY;
		m_close = true;
		SoundManager.SePlay("sys_menu_decide");
	}

	public void OnClickInfoButton()
	{
		m_btnAct = BUTTON_ACT.INFO;
		EventRewardWindow.Create(m_spInfoData);
	}

	public void OnClickRouletteButton()
	{
		SetAlertSimpleUI(true);
		m_btnAct = BUTTON_ACT.ROULETTE;
		m_close = true;
		SoundManager.SePlay("sys_menu_decide");
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_roulette");
		UIPlayAnimation component = gameObject.GetComponent<UIPlayAnimation>();
		if (component != null)
		{
			EventDelegate.Add(component.onFinished, WindowAnimationFinishCallback, true);
			component.Play(true);
		}
	}

	public void OnClickSnsLogin()
	{
		SoundManager.SePlay("sys_menu_decide");
		m_easySnsFeed = new EasySnsFeed(base.gameObject, "Camera/SpecialStageWindowUI/Anchor_5_MC");
	}

	public void OnClickAllToggle()
	{
		if (!m_toggleLock && m_currentRankerType != RankingUtil.RankingRankerType.SP_ALL && m_loading != null && !m_loading.activeSelf)
		{
			m_currentRankerList = null;
			ResetRankerList(0, m_currentRankerType);
			if (!SetRanking(RankingUtil.RankingRankerType.SP_ALL, RankingManager.EndlessSpecialRankingScoreType, 0) && m_loading != null)
			{
				m_loading.SetActive(true);
			}
			SoundManager.SePlay("sys_menu_decide");
		}
	}

	public void OnClickFriendToggle()
	{
		if (!m_toggleLock && m_currentRankerType != RankingUtil.RankingRankerType.SP_FRIEND && m_loading != null && !m_loading.activeSelf)
		{
			m_currentRankerList = null;
			ResetRankerList(0, m_currentRankerType);
			if (!SetRanking(RankingUtil.RankingRankerType.SP_FRIEND, RankingManager.EndlessSpecialRankingScoreType, 0) && m_loading != null)
			{
				m_loading.SetActive(true);
			}
			SoundManager.SePlay("sys_menu_decide");
		}
	}

	public void OnClickReward()
	{
		SoundManager.SePlay("sys_menu_decide");
		ChaoSetWindowUI window = ChaoSetWindowUI.GetWindow();
		if (!(window != null) || m_spInfoData == null || m_spInfoData.GetRewardChao() == null || m_spInfoData.GetRewardChao().id < 0)
		{
			return;
		}
		ChaoData chaoData = ChaoTable.GetChaoData(m_spInfoData.GetRewardChao().id);
		if (chaoData != null)
		{
			ChaoSetWindowUI.ChaoInfo chaoInfo = new ChaoSetWindowUI.ChaoInfo(chaoData);
			chaoInfo.level = ChaoTable.ChaoMaxLevel();
			chaoInfo.detail = chaoData.GetDetailsLevel(chaoInfo.level);
			if (chaoInfo.level == ChaoTable.ChaoMaxLevel())
			{
				chaoInfo.detail = chaoInfo.detail + "\n" + TextUtility.GetChaoText("Chao", "level_max");
			}
			window.OpenWindow(chaoInfo, ChaoSetWindowUI.WindowType.WINDOW_ONLY);
		}
	}

	public void OnClickMoreButton()
	{
		RankingManager instance = SingletonGameObject<RankingManager>.Instance;
		if (instance != null && !instance.isLoading)
		{
			Transform transform = m_mainListPanel.transform;
			Vector3 localPosition = m_mainListPanel.transform.localPosition;
			float x = localPosition.x;
			Vector3 localPosition2 = m_mainListPanel.transform.localPosition;
			transform.localPosition = new Vector3(x, 0f, localPosition2.z);
			m_mainListPanel.Scroll(0f);
			m_mainListPanel.ResetPosition();
			ResetRankerList(1, m_currentRankerType);
			SetRanking(m_currentRankerType, m_currentScoreType, 1);
		}
		SoundManager.SePlay("sys_menu_decide");
	}

	private void OnClickFriendOption()
	{
		SoundManager.SePlay("sys_menu_decide");
		GameObject loadMenuChildObject = HudMenuUtility.GetLoadMenuChildObject("RankingFriendOptionWindow", true);
		if (loadMenuChildObject != null)
		{
			RankingFriendOptionWindow component = loadMenuChildObject.GetComponent<RankingFriendOptionWindow>();
			if (component != null)
			{
				component.StartCoroutine("SetUp");
			}
		}
	}

	private void OnClickFriendOptionOk()
	{
		if (SingletonGameObject<RankingManager>.Instance != null && EventManager.Instance != null && EventManager.Instance.Type == EventManager.EventType.SPECIAL_STAGE)
		{
			SingletonGameObject<RankingManager>.Instance.Reset(RankingUtil.RankingMode.ENDLESS, RankingUtil.RankingRankerType.SP_FRIEND);
			SetRanking(RankingUtil.RankingRankerType.SP_FRIEND, RankingManager.EndlessSpecialRankingScoreType, -1);
		}
	}

	public void OnClickEndButton(ButtonInfoTable.ButtonType btnType)
	{
		SetAlertSimpleUI(true);
		switch (btnType)
		{
		case ButtonInfoTable.ButtonType.REDSTAR_TO_SHOP:
			m_btnAct = BUTTON_ACT.SHOP_RSRING;
			break;
		case ButtonInfoTable.ButtonType.RING_TO_SHOP:
			m_btnAct = BUTTON_ACT.SHOP_RING;
			break;
		case ButtonInfoTable.ButtonType.CHALLENGE_TO_SHOP:
			m_btnAct = BUTTON_ACT.SHOP_CHALLENGE;
			break;
		case ButtonInfoTable.ButtonType.REWARDLIST_TO_CHAO_ROULETTE:
			m_btnAct = BUTTON_ACT.ROULETTE;
			break;
		case ButtonInfoTable.ButtonType.EVENT_BACK:
			m_btnAct = BUTTON_ACT.CLOSE;
			break;
		}
		m_close = true;
		if (m_animation != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_eventmenu_outro", Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, WindowAnimationFinishCallback, true);
		}
	}

	public void WindowAnimationFinishCallbackByPlay()
	{
		HudMenuUtility.SendVirtualNewItemSelectClicked(HudMenuUtility.ITEM_SELECT_MODE.EVENT_STAGE);
		base.enabledAnchorObjects = false;
		SetAlertSimpleUI(false);
		m_opened = false;
	}

	private void WindowAnimationFinishCallback()
	{
		if (m_close)
		{
			m_mode = Mode.End;
			m_toggleLock = false;
			switch (m_btnAct)
			{
			case BUTTON_ACT.PLAY:
				HudMenuUtility.SendVirtualNewItemSelectClicked(HudMenuUtility.ITEM_SELECT_MODE.EVENT_STAGE);
				base.enabledAnchorObjects = false;
				break;
			case BUTTON_ACT.INFO:
				EventRewardWindow.Create(m_spInfoData);
				break;
			case BUTTON_ACT.ROULETTE:
				base.enabledAnchorObjects = false;
				HudMenuUtility.SendChaoRouletteButtonClicked();
				break;
			case BUTTON_ACT.SHOP_RSRING:
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.REDSTAR_TO_SHOP);
				base.enabledAnchorObjects = false;
				break;
			case BUTTON_ACT.SHOP_RING:
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.RING_TO_SHOP);
				base.enabledAnchorObjects = false;
				break;
			case BUTTON_ACT.SHOP_CHALLENGE:
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.CHALLENGE_TO_SHOP);
				base.enabledAnchorObjects = false;
				break;
			default:
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.EVENT_BACK);
				base.enabledAnchorObjects = false;
				break;
			}
			SetAlertSimpleUI(false);
			BackKeyManager.RemoveWindowCallBack(base.gameObject);
			m_opened = false;
		}
		else
		{
			m_opened = true;
			SetAlertSimpleUI(false);
		}
	}

	private void OnClickPlatformBackButton(WindowBase.BackButtonMessage msg)
	{
		if (msg != null && m_alertCollider)
		{
			msg.StaySequence();
		}
	}

	private void SetAlertSimpleUI(bool flag)
	{
		if (m_alertCollider)
		{
			if (!flag)
			{
				HudMenuUtility.SetConnectAlertSimpleUI(false);
				m_alertCollider = false;
			}
		}
		else if (flag)
		{
			HudMenuUtility.SetConnectAlertSimpleUI(true);
			m_alertCollider = true;
		}
	}

	public static SpecialStageWindow Create(SpecialStageInfo info)
	{
		if (s_instance != null)
		{
			if (s_instance.gameObject.transform.parent != null && s_instance.gameObject.transform.parent.name != "Camera")
			{
				return null;
			}
			s_instance.gameObject.SetActive(true);
			s_instance.Setup(info);
			RankingManager instance = SingletonGameObject<RankingManager>.Instance;
			if (instance != null && !instance.isLoading && !instance.IsRankingTop(RankingUtil.RankingMode.ENDLESS, RankingManager.EndlessSpecialRankingScoreType, RankingUtil.RankingRankerType.SP_ALL))
			{
				RankingSetup(true);
			}
			return s_instance;
		}
		return null;
	}

	public static SpecialStageWindow RankingSetup(bool createRankingUsetList = false)
	{
		if (s_instance != null)
		{
			if (s_instance.enabledAnchorObjects)
			{
				if (createRankingUsetList)
				{
					s_instance.SetRanking(RankingUtil.RankingRankerType.SP_ALL, RankingManager.EndlessSpecialRankingScoreType, -1);
				}
				else
				{
					s_instance.SetLoadingObject();
				}
			}
			return s_instance;
		}
		return null;
	}

	public static bool IsOpend()
	{
		if (s_instance != null)
		{
			return s_instance.m_opened;
		}
		return false;
	}

	public static void SetLoading()
	{
		if (s_instance != null)
		{
			s_instance.SetLoadingObject();
		}
	}

	public void UpdateSendChallengeOrg(RankingUtil.RankingRankerType type, string id)
	{
		if (m_currentRankerType != type || m_objects == null || m_objects.Count <= 0)
		{
			return;
		}
		if (m_objects.ContainsKey("pattern_0") && m_objects["pattern_0"] != null && m_objects["pattern_0"].activeSelf && m_topRankerArea != null)
		{
			ui_ranking_scroll[] componentsInChildren = m_topRankerArea.GetComponentsInChildren<ui_ranking_scroll>();
			if (componentsInChildren != null && componentsInChildren.Length > 0)
			{
				ui_ranking_scroll[] array = componentsInChildren;
				foreach (ui_ranking_scroll ui_ranking_scroll in array)
				{
					ui_ranking_scroll.UpdateSendChallenge(id);
				}
			}
		}
		else
		{
			if (!m_objects.ContainsKey("pattern_1") || !(m_objects["pattern_0"] != null) || !m_objects["pattern_1"].activeSelf || !(m_listArea != null))
			{
				return;
			}
			ui_ranking_scroll[] componentsInChildren2 = m_listArea.GetComponentsInChildren<ui_ranking_scroll>();
			if (componentsInChildren2 != null && componentsInChildren2.Length > 0)
			{
				ui_ranking_scroll[] array2 = componentsInChildren2;
				foreach (ui_ranking_scroll ui_ranking_scroll2 in array2)
				{
					ui_ranking_scroll2.UpdateSendChallenge(id);
				}
			}
		}
	}

	public static void UpdateSendChallenge(RankingUtil.RankingRankerType type, string id)
	{
		if (s_instance != null)
		{
			s_instance.UpdateSendChallengeOrg(type, id);
		}
	}

	private void Awake()
	{
		SetInstance();
		base.enabledAnchorObjects = false;
	}

	private void OnDestroy()
	{
		if (s_instance == this)
		{
			s_instance = null;
		}
	}

	private void SetInstance()
	{
		if (s_instance == null)
		{
			s_instance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
