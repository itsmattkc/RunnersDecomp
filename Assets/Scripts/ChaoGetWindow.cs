using UnityEngine;

public class ChaoGetWindow : WindowBase
{
	private enum State
	{
		NONE = -1,
		PLAYING,
		WAIT_CLICK_NEXT_BUTTON,
		CLICKED_NEXT_BUTTON,
		END,
		NUM
	}

	private bool m_isSetuped;

	private bool m_isClickedEquip;

	private bool m_isTutorial;

	private bool m_disabledEqip;

	private bool m_backKeyVaildNextBtn;

	private bool m_backKyeVaildOKBtn;

	private ChaoGetPartsBase m_chaoGetParts;

	private HudFlagWatcher m_SeFlagHatch;

	private HudFlagWatcher m_SeFlagBreak;

	private ChaoGetPartsBase.BtnType m_btnType;

	private RouletteUtility.AchievementType m_achievementType;

	private GameObject[] m_buttons = new GameObject[3];

	private State m_state = State.END;

	public bool isSetuped
	{
		get
		{
			return m_isSetuped;
		}
		set
		{
			m_isSetuped = false;
		}
	}

	public bool IsPlayEnd
	{
		get
		{
			return m_state == State.END;
		}
	}

	public bool IsClickedEquip
	{
		get
		{
			return m_isClickedEquip;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (m_state == State.CLICKED_NEXT_BUTTON)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "skip_collider");
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
			GameObject[] buttons = m_buttons;
			foreach (GameObject gameObject2 in buttons)
			{
				if (!(gameObject2 == null))
				{
					gameObject2.SetActive(false);
				}
			}
			if (m_chaoGetParts != null)
			{
				m_chaoGetParts.PlayGetAnimation(base.gameObject.GetComponent<Animation>());
				m_btnType = m_chaoGetParts.GetButtonType();
				m_buttons[(int)m_btnType].SetActive(true);
			}
			SetEnableButton(m_btnType, false);
			m_state = State.PLAYING;
		}
		if (m_SeFlagHatch != null)
		{
			m_SeFlagHatch.Update();
		}
		if (m_SeFlagBreak != null)
		{
			m_SeFlagBreak.Update();
		}
	}

	public void PlayStart(ChaoGetPartsBase chaoGetParts, bool isTutorial, bool disabledEqip = false, RouletteUtility.AchievementType achievement = RouletteUtility.AchievementType.NONE)
	{
		m_achievementType = achievement;
		RouletteManager.OpenRouletteWindow();
		m_chaoGetParts = chaoGetParts;
		m_isTutorial = isTutorial;
		m_backKyeVaildOKBtn = false;
		m_backKeyVaildNextBtn = false;
		m_isClickedEquip = false;
		m_disabledEqip = disabledEqip;
		m_state = State.PLAYING;
		if (m_isTutorial)
		{
			if (RouletteTop.Instance != null && RouletteTop.Instance.category == RouletteCategory.PREMIUM)
			{
				m_isTutorial = true;
			}
			else
			{
				m_isTutorial = false;
			}
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "pattern_btn_use");
		if (gameObject != null)
		{
			m_buttons[0] = GameObjectUtil.FindChildGameObject(gameObject, "pattern_0");
			m_buttons[2] = GameObjectUtil.FindChildGameObject(gameObject, "pattern_5");
			m_buttons[1] = GameObjectUtil.FindChildGameObject(gameObject, "pattern_6");
		}
		if (!m_isSetuped)
		{
			if (m_buttons[0] != null)
			{
				UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(m_buttons[0], "Btn_ok");
				if (uIButtonMessage != null)
				{
					uIButtonMessage.target = base.gameObject;
					uIButtonMessage.functionName = "OkButtonClickedCallback";
				}
			}
			if (m_buttons[1] != null)
			{
				UIButtonMessage uIButtonMessage2 = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(m_buttons[1], "Btn_next");
				if (uIButtonMessage2 != null)
				{
					uIButtonMessage2.target = base.gameObject;
					uIButtonMessage2.functionName = "NextButtonClickedCallback";
				}
			}
			if (m_buttons[2] != null)
			{
				UIButtonMessage uIButtonMessage3 = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(m_buttons[2], "Btn_ok");
				if (uIButtonMessage3 != null)
				{
					uIButtonMessage3.target = base.gameObject;
					uIButtonMessage3.functionName = "OkButtonClickedCallback";
				}
				UIButtonMessage uIButtonMessage4 = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(m_buttons[2], "Btn_post");
				if (uIButtonMessage4 != null)
				{
					uIButtonMessage4.target = base.gameObject;
					uIButtonMessage4.functionName = "EquipButtonClickedCallback";
				}
			}
			UIButtonMessage uIButtonMessage5 = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(base.gameObject, "skip_collider");
			if (uIButtonMessage5 != null)
			{
				uIButtonMessage5.target = base.gameObject;
				uIButtonMessage5.functionName = "SkipButtonClickedCallback";
			}
			m_SeFlagHatch = new HudFlagWatcher();
			GameObject watchObject = GameObjectUtil.FindChildGameObject(base.gameObject, "SE_flag");
			m_SeFlagHatch.Setup(watchObject, SeFlagHatchCallback);
			m_SeFlagBreak = new HudFlagWatcher();
			GameObject watchObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "SE_flag_break");
			m_SeFlagBreak.Setup(watchObject2, SeFlagBreakCallback);
			m_isSetuped = true;
		}
		base.gameObject.SetActive(true);
		GameObject[] buttons = m_buttons;
		foreach (GameObject gameObject2 in buttons)
		{
			if (!(gameObject2 == null))
			{
				gameObject2.SetActive(false);
			}
		}
		if (m_chaoGetParts != null)
		{
			m_chaoGetParts.SetCallback(AnimationEndCallback);
			m_chaoGetParts.Setup(base.gameObject);
			Animation component = base.gameObject.GetComponent<Animation>();
			component.Stop();
			m_chaoGetParts.PlayGetAnimation(component);
			m_btnType = m_chaoGetParts.GetButtonType();
			if (m_achievementType == RouletteUtility.AchievementType.Multi || RouletteUtility.loginRoulette)
			{
				m_btnType = ChaoGetPartsBase.BtnType.OK;
			}
			if (m_btnType >= ChaoGetPartsBase.BtnType.OK && m_btnType < ChaoGetPartsBase.BtnType.NUM)
			{
				if (m_buttons[(int)m_btnType] != null)
				{
					m_buttons[(int)m_btnType].SetActive(true);
				}
				else if (m_buttons[2] != null)
				{
					GameObject gameObject3 = m_buttons[2];
					if (gameObject3 != null)
					{
						gameObject3.SetActive(true);
						UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(gameObject3, "Btn_post");
						if (uIImageButton != null)
						{
							uIImageButton.isEnabled = false;
						}
					}
				}
			}
		}
		GameObject gameObject4 = GameObjectUtil.FindChildGameObject(base.gameObject, "skip_collider");
		if (gameObject4 != null)
		{
			gameObject4.SetActive(true);
		}
		SetEnableButton(m_btnType, false);
		UIDraggablePanel uIDraggablePanel = GameObjectUtil.FindChildGameObjectComponent<UIDraggablePanel>(base.gameObject, "window_chaoset_alpha_clip");
		if (uIDraggablePanel != null)
		{
			uIDraggablePanel.ResetPosition();
		}
		SoundManager.SePlay("sys_window_open");
	}

	private void SetEnableButton(ChaoGetPartsBase.BtnType buttonType, bool isEnable)
	{
		UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(m_buttons[(int)buttonType], "Btn_ok");
		if (uIImageButton != null)
		{
			uIImageButton.isEnabled = isEnable;
		}
		UIImageButton uIImageButton2 = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(m_buttons[(int)buttonType], "Btn_next");
		if (uIImageButton2 != null)
		{
			uIImageButton2.isEnabled = isEnable;
		}
		UIImageButton uIImageButton3 = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(m_buttons[(int)buttonType], "Btn_post");
		if (uIImageButton3 != null)
		{
			uIImageButton3.isEnabled = isEnable;
		}
	}

	private void AnimationEndCallback(ChaoGetPartsBase.AnimType animType)
	{
		switch (animType)
		{
		case ChaoGetPartsBase.AnimType.GET_ANIM_CONTINUE:
		{
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "skip_collider");
			if (gameObject2 != null)
			{
				gameObject2.SetActive(false);
			}
			SetEnableButton(m_btnType, true);
			SetEqipBtnDisabled();
			m_backKeyVaildNextBtn = true;
			m_state = State.WAIT_CLICK_NEXT_BUTTON;
			break;
		}
		case ChaoGetPartsBase.AnimType.GET_ANIM_FINISH:
		{
			SetEnableButton(m_btnType, true);
			SetEqipBtnDisabled();
			if (m_isTutorial)
			{
				TutorialCursor.StartTutorialCursor(TutorialCursor.Type.ROULETTE_OK);
			}
			GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "skip_collider");
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
			m_backKyeVaildOKBtn = true;
			break;
		}
		case ChaoGetPartsBase.AnimType.OUT_ANIM:
			DeleteChaoTexture();
			base.gameObject.SetActive(false);
			m_state = State.END;
			break;
		}
	}

	private void SetEqipBtnDisabled()
	{
		if (!m_disabledEqip)
		{
			return;
		}
		GameObject gameObject = m_buttons[2];
		if (gameObject != null)
		{
			UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(gameObject, "Btn_post");
			if (uIImageButton != null)
			{
				uIImageButton.isEnabled = false;
			}
		}
	}

	private void OkButtonClickedCallback()
	{
		RouletteManager.CloseRouletteWindow();
		m_isClickedEquip = false;
		SoundManager.SePlay("sys_menu_decide");
		if (m_achievementType != 0)
		{
			RouletteManager.RouletteGetWindowClose(m_achievementType);
			m_achievementType = RouletteUtility.AchievementType.NONE;
		}
		if (m_chaoGetParts != null)
		{
			m_chaoGetParts.PlayEndAnimation(base.gameObject.GetComponent<Animation>());
		}
		m_backKyeVaildOKBtn = false;
		m_backKeyVaildNextBtn = false;
	}

	private void NextButtonClickedCallback()
	{
		RouletteManager.CloseRouletteWindow();
		m_backKyeVaildOKBtn = false;
		m_backKeyVaildNextBtn = false;
		m_state = State.CLICKED_NEXT_BUTTON;
	}

	private void EquipButtonClickedCallback()
	{
		RouletteManager.CloseRouletteWindow();
		if (m_achievementType != 0)
		{
			if (m_achievementType == RouletteUtility.AchievementType.PlayerGet)
			{
				RouletteManager.RouletteGetWindowClose(m_achievementType, RouletteUtility.NextType.CHARA_EQUIP);
			}
			else
			{
				RouletteManager.RouletteGetWindowClose(m_achievementType, RouletteUtility.NextType.EQUIP);
			}
			m_achievementType = RouletteUtility.AchievementType.NONE;
		}
		m_isClickedEquip = true;
		SoundManager.SePlay("sys_menu_decide");
		if (m_chaoGetParts != null)
		{
			m_chaoGetParts.PlayEndAnimation(base.gameObject.GetComponent<Animation>());
		}
		m_backKyeVaildOKBtn = false;
		m_backKeyVaildNextBtn = false;
	}

	private void SeFlagHatchCallback(float newValue, float prevValue)
	{
		if (newValue == 1f && m_chaoGetParts != null)
		{
			m_chaoGetParts.PlaySE(ChaoWindowUtility.SeHatch);
		}
	}

	private void SeFlagBreakCallback(float newValue, float prevValue)
	{
		if (newValue == 1f && m_chaoGetParts != null)
		{
			m_chaoGetParts.PlaySE(ChaoWindowUtility.SeBreak);
		}
	}

	private void SkipButtonClickedCallback()
	{
		Animation component = base.gameObject.GetComponent<Animation>();
		if (!(component != null))
		{
			return;
		}
		foreach (AnimationState item in component)
		{
			if (!(item == null))
			{
				item.time = item.length * 0.99f;
			}
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
			ChaoTextureManager.Instance.RemoveChaoTexture(m_chaoGetParts.ChaoId);
		}
	}

	private void SendMessageOnClick(string btnName)
	{
		if (m_btnType == ChaoGetPartsBase.BtnType.NONE || m_btnType == ChaoGetPartsBase.BtnType.NUM || !(m_buttons[(int)m_btnType] != null))
		{
			return;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_buttons[(int)m_btnType], btnName);
		if (gameObject != null && gameObject.activeSelf)
		{
			UIButtonMessage component = gameObject.GetComponent<UIButtonMessage>();
			if (component != null)
			{
				component.SendMessage("OnClick");
			}
		}
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (m_backKyeVaildOKBtn)
		{
			SendMessageOnClick("Btn_ok");
		}
		else if (m_backKeyVaildNextBtn)
		{
			SendMessageOnClick("Btn_next");
		}
		if (msg != null)
		{
			msg.StaySequence();
		}
	}
}
