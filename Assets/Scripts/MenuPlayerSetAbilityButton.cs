using AnimationOrTween;
using System.Collections;
using Text;
using UnityEngine;

public class MenuPlayerSetAbilityButton : MonoBehaviour
{
	private enum State
	{
		IDLE,
		LEVELUP
	}

	public delegate void AnimEndCallback();

	private AbilityButtonParams m_params;

	private MenuPlayerSetLevelState m_currentState;

	private State m_state;

	private float m_levelUpAnimTime;

	private readonly float LevelUpAnimTotalTime = 1f;

	private AnimEndCallback m_animEndCallback;

	private bool m_animEnd = true;

	public bool AnimEnd
	{
		get
		{
			return m_animEnd;
		}
		private set
		{
		}
	}

	private void Start()
	{
	}

	private void LateUpdate()
	{
		State state = m_state;
		if (state == State.IDLE || state != State.LEVELUP)
		{
			return;
		}
		ImportAbilityTable instance = ImportAbilityTable.GetInstance();
		int maxLevel = instance.GetMaxLevel(m_params.Ability);
		int level = MenuPlayerSetUtil.GetLevel(m_params.Character, m_params.Ability);
		UISlider uISlider = GameObjectUtil.FindChildGameObjectComponent<UISlider>(base.gameObject, "Pgb_item_lv");
		if (uISlider != null)
		{
			m_levelUpAnimTime += Time.deltaTime;
			if (m_levelUpAnimTime >= LevelUpAnimTotalTime)
			{
				m_levelUpAnimTime = LevelUpAnimTotalTime;
				m_state = State.IDLE;
			}
			float num = (float)level - 1f + m_levelUpAnimTime / LevelUpAnimTotalTime;
			num = (uISlider.value = num / (float)maxLevel);
		}
	}

	public void Setup(CharaType charaType, AbilityType abilityType)
	{
		m_params = new AbilityButtonParams();
		m_params.Character = charaType;
		m_params.Ability = abilityType;
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "img_cursor_eff_set");
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
		InitLabels();
		InitButtonState();
		UISlider uISlider = GameObjectUtil.FindChildGameObjectComponent<UISlider>(base.gameObject, "Pgb_item_lv");
		if (uISlider != null)
		{
			ImportAbilityTable instance = ImportAbilityTable.GetInstance();
			int maxLevel = instance.GetMaxLevel(m_params.Ability);
			int level = MenuPlayerSetUtil.GetLevel(m_params.Character, m_params.Ability);
			uISlider.value = (float)level / (float)maxLevel;
		}
		m_state = State.IDLE;
	}

	public void SetActive(bool isActive)
	{
		GameObject x = GameObjectUtil.FindChildGameObject(base.gameObject, "img_cursor");
		if (!(x != null))
		{
		}
	}

	public void LevelUp(AnimEndCallback callback)
	{
		m_animEndCallback = callback;
		m_animEnd = false;
		ImportAbilityTable instance = ImportAbilityTable.GetInstance();
		int maxLevel = instance.GetMaxLevel(m_params.Ability);
		int level = MenuPlayerSetUtil.GetLevel(m_params.Character, m_params.Ability);
		if (level >= maxLevel)
		{
			Object.Destroy(m_currentState);
			m_currentState = base.gameObject.AddComponent<MenuPlayerSetLevelStateMax>();
			m_currentState.Setup(m_params);
			AwakeLevelMax();
		}
		SetPotentialText();
		ChangeLevelLabels();
		Animation animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(base.gameObject, "Btn_toggle");
		if (animation != null)
		{
			animation.Stop();
			ActiveAnimation activeAnimation = ActiveAnimation.Play(animation, Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, LevelUpAnimationEndCallback, true);
			GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "img_cursor_eff_set");
			if (gameObject != null)
			{
				UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_effect_b4");
				if (uILabel != null)
				{
					uILabel.text = instance.GetAbilityPotential(m_params.Ability, level - 1).ToString();
				}
				UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_effect_af");
				if (uILabel2 != null)
				{
					uILabel2.text = instance.GetAbilityPotential(m_params.Ability, level).ToString();
				}
				gameObject.SetActive(true);
			}
			SetActive(false);
		}
		m_levelUpAnimTime = 0f;
		m_state = State.LEVELUP;
	}

	public void SkipLevelUp()
	{
		if (m_animEndCallback == null)
		{
			return;
		}
		Animation animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(base.gameObject, "Btn_toggle");
		if (!(animation != null))
		{
			return;
		}
		foreach (AnimationState item in animation)
		{
			if (!(item == null))
			{
				item.time = item.length * 0.99f;
			}
		}
	}

	private void InitLabels()
	{
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_icon");
		if (uISprite != null)
		{
			string text2 = uISprite.spriteName = "ui_mm_player_icon_" + (int)m_params.Ability;
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_word_item_effect");
		if (uILabel != null)
		{
			string cellName = "abilitycaption" + (int)(m_params.Ability + 1);
			string text4 = uILabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "CharaStatus", cellName).text;
		}
		SetPotentialText();
	}

	private void InitButtonState()
	{
		if (!(m_currentState != null))
		{
			ImportAbilityTable instance = ImportAbilityTable.GetInstance();
			int maxLevel = instance.GetMaxLevel(m_params.Ability);
			int level = MenuPlayerSetUtil.GetLevel(m_params.Character, m_params.Ability);
			if (level >= maxLevel)
			{
				m_currentState = base.gameObject.AddComponent<MenuPlayerSetLevelStateMax>();
				AwakeLevelMax();
			}
			else
			{
				m_currentState = base.gameObject.AddComponent<MenuPlayerSetLevelStateNormal>();
			}
			m_currentState.Setup(m_params);
			ChangeLevelLabels();
		}
	}

	private void SetPotentialText()
	{
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_item_effect");
		if (uILabel != null)
		{
			string cellName = "abilitypotential" + (int)(m_params.Ability + 1);
			TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "CharaStatus", cellName);
			ImportAbilityTable instance = ImportAbilityTable.GetInstance();
			int level = MenuPlayerSetUtil.GetLevel(m_params.Character, m_params.Ability);
			text.ReplaceTag("{ABILITY_POTENTIAL}", instance.GetAbilityPotential(m_params.Ability, level).ToString());
			uILabel.text = text.text;
			UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_item_effect_sh");
			if (uILabel2 != null)
			{
				uILabel2.text = text.text;
			}
		}
	}

	private void ChangeLevelLabels()
	{
		if (m_currentState != null)
		{
			m_currentState.ChangeLabels();
		}
	}

	private void AwakeLevelMax()
	{
		UIImageButton component = base.gameObject.GetComponent<UIImageButton>();
		if (component != null)
		{
			component.isEnabled = false;
		}
	}

	private void LevelUpAnimationEndCallback()
	{
		m_animEnd = true;
		StartCoroutine(HideEffectObject());
		m_animEndCallback();
		m_animEndCallback = null;
	}

	private IEnumerator HideEffectObject()
	{
		yield return null;
		GameObject effectObject = GameObjectUtil.FindChildGameObject(base.gameObject, "img_cursor_eff_set");
		if (effectObject != null)
		{
			effectObject.SetActive(false);
		}
	}
}
