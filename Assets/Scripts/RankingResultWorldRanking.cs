using AnimationOrTween;
using System.Collections;
using UnityEngine;

public class RankingResultWorldRanking : WindowBase
{
	public enum ResultType
	{
		WORLD_RANKING,
		QUICK_WORLD_RANKING,
		EVENT_RANKING,
		NUM
	}

	private bool m_isSetup;

	private bool m_isOpened;

	private bool m_isEnd;

	private ResultType m_resultType;

	private InfoDecoder m_decoder;

	public bool IsEnd
	{
		get
		{
			return m_isEnd;
		}
	}

	private void Start()
	{
	}

	private void OnDestroy()
	{
		Destroy();
	}

	private void Update()
	{
	}

	private ResultType GetResultType(int id)
	{
		if (id == NetNoticeItem.OPERATORINFO_RANKINGRESULT_ID)
		{
			return ResultType.WORLD_RANKING;
		}
		if (id == NetNoticeItem.OPERATORINFO_QUICKRANKINGRESULT_ID)
		{
			return ResultType.QUICK_WORLD_RANKING;
		}
		if (id == NetNoticeItem.OPERATORINFO_EVENTRANKINGRESULT_ID)
		{
			return ResultType.EVENT_RANKING;
		}
		return ResultType.WORLD_RANKING;
	}

	public void Setup(NetNoticeItem item)
	{
		ResultType resultType = GetResultType((int)item.Id);
		Setup(resultType, item.Message);
	}

	public void Setup(ResultType resultType, string messageInfo)
	{
		base.gameObject.SetActive(true);
		m_resultType = resultType;
		switch (m_resultType)
		{
		case ResultType.WORLD_RANKING:
			m_decoder = new InfoDecoderWorldRanking(messageInfo);
			break;
		case ResultType.QUICK_WORLD_RANKING:
			m_decoder = new InfoDecoderWorldRanking(messageInfo);
			break;
		case ResultType.EVENT_RANKING:
			m_decoder = new InfoDecoderEvent(messageInfo);
			break;
		}
		if (m_decoder == null)
		{
			return;
		}
		if (!m_isSetup)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_close");
			if (gameObject != null)
			{
				UIButtonMessage uIButtonMessage = gameObject.GetComponent<UIButtonMessage>();
				if (uIButtonMessage == null)
				{
					uIButtonMessage = gameObject.AddComponent<UIButtonMessage>();
				}
				uIButtonMessage.target = base.gameObject;
				uIButtonMessage.functionName = "OnClickCloseButton";
			}
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "blinder");
			if (gameObject2 != null)
			{
				UIButtonMessage uIButtonMessage2 = gameObject2.GetComponent<UIButtonMessage>();
				if (uIButtonMessage2 == null)
				{
					uIButtonMessage2 = gameObject2.AddComponent<UIButtonMessage>();
				}
				uIButtonMessage2.target = base.gameObject;
				uIButtonMessage2.functionName = "OnClickCloseButton";
			}
			m_isSetup = true;
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_caption");
		if (uILabel != null)
		{
			uILabel.text = m_decoder.GetCaption();
		}
		UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_caption_sh");
		if (uILabel2 != null)
		{
			uILabel2.text = m_decoder.GetCaption();
		}
		UILabel uILabel3 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_ranking_ex");
		if (uILabel3 != null)
		{
			uILabel3.text = m_decoder.GetResultString();
		}
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_icon_medal_blue");
		if (uISprite != null)
		{
			uISprite.spriteName = m_decoder.GetMedalSpriteName();
		}
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(base.gameObject, "word_anim");
		if (gameObject3 != null)
		{
			gameObject3.transform.localScale = new Vector3(0f, 0f, 1f);
		}
	}

	public void PlayStart()
	{
		m_isEnd = false;
		m_isOpened = false;
		SoundManager.SePlay("sys_window_open");
		Animation animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(base.gameObject, "ranking_window");
		if (animation != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(animation, "ui_cmn_window_Anim", Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, InAnimationFinishCallack, true);
			SoundManager.SePlay("sys_result_best");
		}
	}

	private void OnClickCloseButton()
	{
		SoundManager.SePlay("sys_window_close");
		m_isOpened = false;
		Animation animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(base.gameObject, "ranking_window");
		if (animation != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(animation, "ui_cmn_window_Anim", Direction.Reverse);
			EventDelegate.Add(activeAnimation.onFinished, OutAnimationFinishCallback, true);
			SoundManager.SePlay("sys_window_close");
		}
	}

	private void InAnimationFinishCallack()
	{
		SoundManager.SePlay("sys_league_up");
		StartCoroutine(OnInAnimationFinishCallback());
		m_isOpened = true;
	}

	private IEnumerator OnInAnimationFinishCallback()
	{
		yield return null;
		Animation animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(base.gameObject, "ranking_window");
		if (animation != null)
		{
			ActiveAnimation.Play(animation, "ui_ranking_world_event_Anim", Direction.Forward);
		}
	}

	private void OutAnimationFinishCallback()
	{
		m_isEnd = true;
		base.gameObject.SetActive(false);
	}

	public static RankingResultWorldRanking GetResultWorldRanking()
	{
		RankingResultWorldRanking result = null;
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (gameObject != null)
		{
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "menu_Anim");
			if (gameObject2 != null)
			{
				result = GameObjectUtil.FindChildGameObjectComponent<RankingResultWorldRanking>(gameObject2, "WorldRankingWindowUI");
			}
		}
		return result;
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
}
