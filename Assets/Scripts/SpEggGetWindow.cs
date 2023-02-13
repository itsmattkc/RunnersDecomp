using AnimationOrTween;
using Text;
using UnityEngine;

public class SpEggGetWindow : WindowBase
{
	private enum State
	{
		NONE = -1,
		PLAYING,
		END,
		NUM
	}

	private bool m_isSetuped;

	private bool m_isOpened;

	[SerializeField]
	private UILabel m_caption;

	private SpEggGetPartsBase m_spEggGetParts;

	private HudFlagWatcher m_SeFlagHatch;

	private HudFlagWatcher m_SeFlagBreak;

	private HudFlagWatcher m_SeFlagSpEgg;

	private RouletteUtility.AchievementType m_achievementType;

	private State m_state = State.END;

	public bool IsPlayEnd
	{
		get
		{
			return m_state == State.END;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (m_SeFlagHatch != null)
		{
			m_SeFlagHatch.Update();
		}
		if (m_SeFlagBreak != null)
		{
			m_SeFlagBreak.Update();
		}
		if (m_SeFlagSpEgg != null)
		{
			m_SeFlagSpEgg.Update();
		}
	}

	public void PlayStart(SpEggGetPartsBase spEggGetParts, RouletteUtility.AchievementType achievement = RouletteUtility.AchievementType.NONE)
	{
		RouletteManager.OpenRouletteWindow();
		m_achievementType = achievement;
		m_spEggGetParts = spEggGetParts;
		m_state = State.PLAYING;
		m_isOpened = false;
		if (m_caption != null)
		{
			m_caption.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoRoulette", "gw_get_chao_caption").text;
		}
		if (!m_isSetuped)
		{
			UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(base.gameObject, "Btn_ok");
			if (uIButtonMessage != null)
			{
				uIButtonMessage.target = base.gameObject;
				uIButtonMessage.functionName = "OkButtonClickedCallback";
			}
			UIButtonMessage uIButtonMessage2 = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(base.gameObject, "skip_collider");
			if (uIButtonMessage2 != null)
			{
				uIButtonMessage2.target = base.gameObject;
				uIButtonMessage2.functionName = "SkipButtonClickedCallback";
			}
			m_SeFlagHatch = new HudFlagWatcher();
			GameObject watchObject = GameObjectUtil.FindChildGameObject(base.gameObject, "SE_flag");
			m_SeFlagHatch.Setup(watchObject, SeFlagHatchCallback);
			m_SeFlagBreak = new HudFlagWatcher();
			GameObject watchObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "SE_flag_break");
			m_SeFlagBreak.Setup(watchObject2, SeFlagBreakCallback);
			m_SeFlagSpEgg = new HudFlagWatcher();
			GameObject watchObject3 = GameObjectUtil.FindChildGameObject(base.gameObject, "SE_flag_spegg");
			m_SeFlagSpEgg.Setup(watchObject3, SeFlagSpEggCallback);
			m_isSetuped = true;
		}
		base.gameObject.SetActive(true);
		if (m_spEggGetParts != null)
		{
			m_spEggGetParts.Setup(base.gameObject);
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "skip_collider");
		if (gameObject != null)
		{
			gameObject.SetActive(true);
		}
		Animation component = base.gameObject.GetComponent<Animation>();
		if (component != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(component, Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, InAnimationFinishCallback, true);
			UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(base.gameObject, "Btn_ok");
			if (uIImageButton != null)
			{
				uIImageButton.isEnabled = false;
			}
		}
		UIDraggablePanel uIDraggablePanel = GameObjectUtil.FindChildGameObjectComponent<UIDraggablePanel>(base.gameObject, "window_chaoset_alpha_clip");
		if (uIDraggablePanel != null)
		{
			uIDraggablePanel.ResetPosition();
		}
		SoundManager.SePlay("sys_window_open");
	}

	private void OkButtonClickedCallback()
	{
		RouletteManager.CloseRouletteWindow();
		SoundManager.SePlay("sys_menu_decide");
		if (m_achievementType != 0)
		{
			RouletteManager.RouletteGetWindowClose(m_achievementType);
			m_achievementType = RouletteUtility.AchievementType.NONE;
		}
		Animation component = base.gameObject.GetComponent<Animation>();
		if (component != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(component, "ui_menu_chao_egg_transform_Window_outro_Anim", Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, OutAnimationFinishedCallback, true);
		}
	}

	private void SeFlagHatchCallback(float newValue, float prevValue)
	{
		if (newValue == 1f && m_spEggGetParts != null)
		{
			m_spEggGetParts.PlaySE(ChaoWindowUtility.SeHatch);
		}
	}

	private void SeFlagBreakCallback(float newValue, float prevValue)
	{
		if (newValue == 1f && m_spEggGetParts != null)
		{
			m_spEggGetParts.PlaySE(ChaoWindowUtility.SeBreak);
		}
	}

	private void SeFlagSpEggCallback(float newValue, float prevValue)
	{
		if (newValue == 1f)
		{
			if (m_spEggGetParts != null)
			{
				m_spEggGetParts.PlaySE(ChaoWindowUtility.SeSpEgg);
			}
			if (m_caption != null)
			{
				m_caption.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoRoulette", "gw_got_special_egg_caption").text;
			}
		}
	}

	private void InAnimationFinishCallback()
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "skip_collider");
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
		UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(base.gameObject, "Btn_ok");
		if (uIImageButton != null)
		{
			uIImageButton.isEnabled = true;
		}
		m_isOpened = true;
	}

	private void OutAnimationFinishedCallback()
	{
		DeleteChaoTexture();
		base.gameObject.SetActive(false);
		m_isOpened = false;
		m_state = State.END;
	}

	private void SkipButtonClickedCallback()
	{
		Animation component = base.gameObject.GetComponent<Animation>();
		if (component != null)
		{
			foreach (AnimationState item in component)
			{
				if (!(item == null))
				{
					item.time = item.length * 0.99f;
				}
			}
		}
		if (m_caption != null)
		{
			m_caption.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoRoulette", "gw_got_special_egg_caption").text;
		}
	}

	private void OnSetChaoTexture(ChaoTextureManager.TextureData data)
	{
		UITexture uITexture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(base.gameObject, "img_chao_1");
		if (uITexture != null)
		{
			uITexture.mainTexture = data.tex;
			uITexture.enabled = true;
		}
	}

	private void DeleteChaoTexture()
	{
		UITexture uITexture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(base.gameObject, "img_chao_1");
		if (uITexture != null)
		{
			uITexture.mainTexture = null;
			uITexture.enabled = false;
			ChaoTextureManager.Instance.RemoveChaoTexture(m_spEggGetParts.ChaoId);
		}
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (m_isOpened)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_ok");
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
