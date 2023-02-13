using AnimationOrTween;
using System.Collections;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class RankingFriendOptionWindow : MonoBehaviour
{
	private enum LabelId
	{
		CAPTION,
		BODY,
		ACTIVE_FRIEND,
		PAGE,
		SORT_ORDER,
		NEXT,
		BACK,
		NUM
	}

	private enum SortOrder
	{
		Ascending,
		Descending,
		NUM
	}

	private UILabel[] m_uiLabels = new UILabel[7];

	private UIDraggablePanel m_draggablePanel;

	private GameObject m_buttonPage;

	private UIPanel m_panel;

	private UIPanel m_scrollViewPanel;

	private List<RankingSimpleScroll> m_simpleScrolls = new List<RankingSimpleScroll>();

	private List<SocialUserData> m_friendList = new List<SocialUserData>();

	private List<SocialUserData> m_chosenFriend = new List<SocialUserData>();

	private List<SocialUserData> m_confirmList = new List<SocialUserData>();

	private readonly int CHOSE_FRIEND_MAX = 50;

	private readonly int LOAD_FRIEND_IMAGE_NUM = 1;

	private readonly int VISIBLE_IMAGE_MAX = 6;

	private int m_loadedFriendImageNum;

	private int m_activeScrollCount;

	private int m_page;

	private int m_pageMax;

	private ActiveAnimation m_activeAnim;

	private bool m_isAnimationEnd;

	private bool m_showConfirmWindow;

	private string[] m_sortOrderText = new string[2];

	private SortOrder m_sortOrder;

	private void Start()
	{
		GameObject parent = GameObjectUtil.FindChildGameObject(base.gameObject, "body");
		m_uiLabels[1] = GameObjectUtil.FindChildGameObjectComponent<UILabel>(parent, "Lbl_body");
		m_uiLabels[0] = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_caption");
		m_uiLabels[2] = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_num_activefriend");
		m_uiLabels[3] = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_page");
		m_uiLabels[4] = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_0d");
		m_uiLabels[5] = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_next");
		m_uiLabels[6] = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_prev");
		m_uiLabels[5].text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ui_friend_select_page_next").text;
		m_uiLabels[6].text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ui_friend_select_page_back").text;
		m_sortOrderText[0] = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ui_friend_select_sort_ascending").text;
		m_sortOrderText[1] = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ui_friend_select_sort_descending").text;
		m_draggablePanel = GameObjectUtil.FindChildGameObjectComponent<UIDraggablePanel>(base.gameObject, "ScrollView");
		m_buttonPage = GameObjectUtil.FindChildGameObject(base.gameObject, "btn_page");
		m_panel = GetComponent<UIPanel>();
		if (m_panel != null)
		{
			m_panel.alpha = 0f;
		}
		m_scrollViewPanel = GameObjectUtil.FindChildGameObjectComponent<UIPanel>(base.gameObject, "ScrollView");
		if (m_scrollViewPanel != null)
		{
			m_scrollViewPanel.alpha = 0f;
		}
	}

	public IEnumerator SetUp()
	{
		m_isAnimationEnd = false;
		yield return null;
		yield return null;
		m_panel.alpha = 1f;
		m_scrollViewPanel.alpha = 1f;
		if (base.animation != null)
		{
			ActiveAnimation m_activeAnim = ActiveAnimation.Play(base.animation, Direction.Forward);
			EventDelegate.Add(m_activeAnim.onFinished, OnFinishedActiveAnimation, true);
		}
		m_showConfirmWindow = false;
		m_page = 0;
		m_sortOrder = SortOrder.Ascending;
		m_uiLabels[0].text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ui_friend_select_caption").text;
		m_uiLabels[1].text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ui_friend_select_body").text;
		m_buttonPage.SetActive(true);
		m_friendList.Clear();
		SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		if (socialInterface != null && socialInterface.IsLoggedIn)
		{
			foreach (SocialUserData user in socialInterface.AllFriendList)
			{
				m_friendList.Add(user);
			}
		}
		m_pageMax = (m_friendList.Count - 1) / CHOSE_FRIEND_MAX;
		m_chosenFriend = new List<SocialUserData>(socialInterface.FriendList);
		m_simpleScrolls = GameObjectUtil.FindChildGameObjectsComponents<RankingSimpleScroll>(base.gameObject, "ui_rankingsimple_scroll(Clone)");
		ScrollListUpdate();
		Sort();
		EntryBackKeyCallBack();
	}

	private void Update()
	{
		if (!(m_draggablePanel != null) || !m_isAnimationEnd)
		{
			return;
		}
		for (int i = 0; i < VISIBLE_IMAGE_MAX; i++)
		{
			float num = (float)(m_loadedFriendImageNum - VISIBLE_IMAGE_MAX) / (float)m_activeScrollCount;
			if (m_draggablePanel.verticalScrollBar.value > num)
			{
				NextImageLoad();
				continue;
			}
			break;
		}
	}

	private void NextImageLoad()
	{
		for (int i = 0; i < LOAD_FRIEND_IMAGE_NUM; i++)
		{
			if (m_loadedFriendImageNum < m_simpleScrolls.Count)
			{
				if (!m_simpleScrolls[m_loadedFriendImageNum].gameObject.activeSelf)
				{
					break;
				}
				m_simpleScrolls[m_loadedFriendImageNum].LoadImage();
				m_loadedFriendImageNum++;
			}
		}
	}

	private void OnFinishedActiveAnimation()
	{
		m_isAnimationEnd = true;
	}

	private void LabelUpdate()
	{
		m_uiLabels[3].text = 1 + m_page + "/" + (1 + m_pageMax);
		m_uiLabels[2].text = m_chosenFriend.Count + "/" + CHOSE_FRIEND_MAX;
	}

	public void ScrollListUpdate()
	{
		m_loadedFriendImageNum = 0;
		m_activeScrollCount = 0;
		List<SocialUserData> list = (!m_showConfirmWindow) ? m_friendList : m_confirmList;
		int num = m_page * m_simpleScrolls.Count;
		foreach (RankingSimpleScroll simpleScroll in m_simpleScrolls)
		{
			if (list.Count > num)
			{
				simpleScroll.gameObject.SetActive(true);
				simpleScroll.SetUserData(list[num]);
				simpleScroll.m_toggle.value = false;
				foreach (SocialUserData item in m_chosenFriend)
				{
					if (item.Id == list[num].Id)
					{
						simpleScroll.m_toggle.value = true;
						break;
					}
				}
				m_activeScrollCount++;
			}
			else
			{
				simpleScroll.gameObject.SetActive(false);
			}
			num++;
		}
		m_draggablePanel.ResetPosition();
		LabelUpdate();
	}

	private void SetUpConfirmWindow()
	{
		m_showConfirmWindow = true;
		m_page = 0;
		m_confirmList = new List<SocialUserData>(m_chosenFriend);
		Sort();
		m_uiLabels[0].text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ui_friend_select_confirmation_caption").text;
		m_uiLabels[1].text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ui_friend_select_confirmation_body").text;
		m_buttonPage.SetActive(false);
		if (base.animation != null)
		{
			m_isAnimationEnd = false;
			m_activeAnim = ActiveAnimation.Play(base.animation, Direction.Forward);
			EventDelegate.Add(m_activeAnim.onFinished, OnFinishedActiveAnimation, true);
		}
	}

	public void ChooseFriend(SocialUserData user, UIToggle toggle)
	{
		if (user != null)
		{
			if (toggle.value)
			{
				if (CHOSE_FRIEND_MAX > m_chosenFriend.Count)
				{
					SoundManager.SePlay("sys_menu_decide");
					m_chosenFriend.Add(user);
				}
				else
				{
					toggle.value = false;
				}
			}
			else
			{
				SoundManager.SePlay("sys_window_close");
				m_chosenFriend.RemoveAll((SocialUserData chooseFriend) => chooseFriend.Id == user.Id);
			}
		}
		LabelUpdate();
		StartCoroutine("UpdateDraggablePanel");
	}

	public IEnumerator UpdateDraggablePanel()
	{
		yield return null;
		yield return null;
		m_draggablePanel.SendMessage("OnVerticalBar");
	}

	public void PageUp()
	{
		if (m_page > 0)
		{
			m_page--;
			ScrollListUpdate();
			SoundManager.SePlay("sys_page_skip");
		}
	}

	public void PageDown()
	{
		if (m_page < m_pageMax)
		{
			m_page++;
			ScrollListUpdate();
			SoundManager.SePlay("sys_page_skip");
		}
	}

	public void OnClickOkButton()
	{
		SoundManager.SePlay("sys_menu_decide");
		if (!m_showConfirmWindow)
		{
			SetUpConfirmWindow();
			return;
		}
		SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		if (socialInterface != null)
		{
			socialInterface.FriendList = new List<SocialUserData>(m_chosenFriend);
		}
		m_showConfirmWindow = false;
		if (!(SingletonGameObject<RankingManager>.Instance != null))
		{
			return;
		}
		SingletonGameObject<RankingManager>.Instance.Reset(RankingUtil.RankingMode.ENDLESS, RankingUtil.RankingRankerType.FRIEND);
		SingletonGameObject<RankingManager>.Instance.Reset(RankingUtil.RankingMode.QUICK, RankingUtil.RankingRankerType.FRIEND);
		if (EventManager.Instance.Type != 0 || (SpecialStageWindow.Instance != null && !SpecialStageWindow.Instance.enabledAnchorObjects))
		{
			RankingUI rankingUI = GameObjectUtil.FindGameObjectComponent<RankingUI>("ui_mm_ranking_page(Clone)");
			if (rankingUI != null)
			{
				rankingUI.SendMessage("OnClickFriendOptionOk");
			}
		}
	}

	public void OnClickNoButton()
	{
		SoundManager.SePlay("sys_window_close");
	}

	public void OnFinishedAnimationCallback()
	{
		RemoveBackKeyCallBack();
		base.gameObject.SetActive(false);
	}

	public void OnPressSortName()
	{
		switch (m_sortOrder)
		{
		case SortOrder.Ascending:
			m_sortOrder = SortOrder.Descending;
			break;
		case SortOrder.Descending:
			m_sortOrder = SortOrder.Ascending;
			break;
		}
		Sort();
		SoundManager.SePlay("sys_menu_decide");
	}

	public void Sort()
	{
		m_uiLabels[4].text = m_sortOrderText[(int)m_sortOrder];
		List<SocialUserData> list = (!m_showConfirmWindow) ? m_friendList : m_confirmList;
		list.Sort(delegate(SocialUserData a, SocialUserData b)
		{
			switch (m_sortOrder)
			{
			case SortOrder.Ascending:
				return string.Compare(a.Name, b.Name, true);
			case SortOrder.Descending:
				return string.Compare(b.Name, a.Name, true);
			default:
				break;
			}
			return string.Compare(a.Name, b.Name, true);
		});
		ScrollListUpdate();
	}

	private void EntryBackKeyCallBack()
	{
		BackKeyManager.AddWindowCallBack(base.gameObject);
	}

	private void RemoveBackKeyCallBack()
	{
		BackKeyManager.RemoveWindowCallBack(base.gameObject);
	}

	public void OnClickPlatformBackButton(WindowBase.BackButtonMessage msg)
	{
		if (msg != null)
		{
			msg.StaySequence();
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_close");
		if (gameObject != null)
		{
			UIButtonMessage component = gameObject.GetComponent<UIButtonMessage>();
			if (component != null)
			{
				component.SendMessage("OnClick");
			}
		}
	}
}
