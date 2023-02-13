using AnimationOrTween;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class RankingResultLeague : WindowBase
{
	private enum Mode
	{
		Idle,
		Wait,
		End
	}

	private Mode m_mode;

	private RankingServerInfoConverter m_rankingData;

	private Animation m_animation;

	private UILabel m_lblInfo;

	private UILabel m_lblLeague;

	private UISprite m_imgLeague;

	private UISprite m_imgLeagueStar;

	private bool m_quickMode;

	private bool m_isOpened;

	private bool m_close;

	private void OnDestroy()
	{
		Destroy();
	}

	public void Setup(string message, bool quick)
	{
		base.gameObject.SetActive(true);
		m_quickMode = quick;
		m_close = false;
		m_rankingData = new RankingServerInfoConverter(message);
		m_animation = GetComponentInChildren<Animation>();
		if (m_animation != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_cmn_window_Anim", Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, WindowAnimationFinishCallback, true);
			SoundManager.SePlay("sys_window_open");
		}
		m_lblInfo = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_league_resilt_ex");
		m_lblLeague = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_league");
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_word_down");
		UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_word_up");
		UISprite uISprite3 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_word_stay");
		uISprite.transform.localScale = new Vector3(0f, 0f, 1f);
		uISprite2.transform.localScale = new Vector3(0f, 0f, 1f);
		uISprite3.transform.localScale = new Vector3(0f, 0f, 1f);
		m_imgLeague = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_icon_league");
		m_imgLeagueStar = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_icon_league_sub");
		LeagueType currentLeague = m_rankingData.currentLeague;
		m_imgLeague.spriteName = "ui_ranking_league_icon_" + RankingUtil.GetLeagueCategoryName(currentLeague).ToLower();
		m_imgLeagueStar.spriteName = "ui_ranking_league_icon_" + RankingUtil.GetLeagueCategoryClass(currentLeague);
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_caption");
		UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_caption_sh");
		if (uILabel != null && uILabel2 != null)
		{
			string text2 = uILabel2.text = (uILabel.text = TextUtility.GetCommonText("Ranking", (!m_quickMode) ? "ui_Lbl_caption_endless_result" : "ui_Lbl_caption_quickmode_result"));
		}
		string text3 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ranking_result_league_stay").text;
		switch (m_rankingData.leagueResult)
		{
		case RankingServerInfoConverter.ResultType.Up:
			text3 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ranking_result_league_up").text;
			break;
		case RankingServerInfoConverter.ResultType.Down:
			text3 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ranking_result_league_down").text;
			break;
		}
		string text4 = TextUtility.Replaces(text3, new Dictionary<string, string>
		{
			{
				"{PARAM}",
				RankingUtil.GetLeagueName(currentLeague)
			}
		});
		m_lblInfo.text = m_rankingData.rankingResultLeagueText;
		m_lblLeague.text = text4;
		m_mode = Mode.Wait;
	}

	public bool IsEnd()
	{
		if (m_mode == Mode.Wait)
		{
			return false;
		}
		return true;
	}

	public void OnClickNoButton()
	{
		m_close = true;
		m_isOpened = false;
		SoundManager.SePlay("sys_window_close");
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_close");
		UIPlayAnimation component = gameObject.GetComponent<UIPlayAnimation>();
		if (component != null)
		{
			EventDelegate.Add(component.onFinished, WindowAnimationFinishCallback, true);
			component.Play(true);
		}
	}

	private void WindowAnimationFinishCallback()
	{
		if (m_rankingData != null && m_rankingData.leagueResult != RankingServerInfoConverter.ResultType.Error && !m_close)
		{
			switch (m_rankingData.leagueResult)
			{
			case RankingServerInfoConverter.ResultType.Stay:
			{
				ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_ranking_league_stay_Anim", Direction.Forward);
				EventDelegate.Add(activeAnimation.onFinished, ResultAnimationFinishCallback, true);
				SoundManager.SePlay("sys_league_stay");
				break;
			}
			case RankingServerInfoConverter.ResultType.Up:
			{
				ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_ranking_league_up_Anim", Direction.Forward);
				EventDelegate.Add(activeAnimation.onFinished, ResultAnimationFinishCallback, true);
				SoundManager.SePlay("sys_league_up");
				break;
			}
			case RankingServerInfoConverter.ResultType.Down:
			{
				ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_ranking_league_down_Anim", Direction.Forward);
				EventDelegate.Add(activeAnimation.onFinished, ResultAnimationFinishCallback, true);
				SoundManager.SePlay("sys_league_down");
				break;
			}
			}
			m_isOpened = true;
		}
		else
		{
			if (!m_close)
			{
				return;
			}
			base.gameObject.SetActive(false);
			Transform parent = base.transform.parent;
			if (parent != null)
			{
				Transform parent2 = parent.transform.parent;
				if (parent2 != null && parent2.name == "LeagueResultWindowUI")
				{
					parent2.gameObject.SetActive(false);
				}
			}
			m_mode = Mode.End;
		}
	}

	private void ResultAnimationFinishCallback()
	{
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (m_isOpened)
		{
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
		if (msg != null)
		{
			msg.StaySequence();
		}
	}

	public static RankingResultLeague Create(NetNoticeItem item)
	{
		return Create(item.Message, item.Id == NetNoticeItem.OPERATORINFO_QUICKRANKINGRESULT_ID);
	}

	public static RankingResultLeague Create(string message, bool quick)
	{
		GameObject cameraUIObject = HudMenuUtility.GetCameraUIObject();
		if (cameraUIObject != null)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(cameraUIObject, "LeagueResultWindowUI");
			if (gameObject != null)
			{
				gameObject.SetActive(true);
				GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "league_window");
				if (gameObject2 != null)
				{
					RankingResultLeague rankingResultLeague = gameObject2.GetComponent<RankingResultLeague>();
					if (rankingResultLeague == null)
					{
						rankingResultLeague = gameObject2.AddComponent<RankingResultLeague>();
					}
					if (rankingResultLeague != null)
					{
						rankingResultLeague.Setup(message, quick);
					}
					return rankingResultLeague;
				}
			}
		}
		return null;
	}
}
