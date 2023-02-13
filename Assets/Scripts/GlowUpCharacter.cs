using System.Collections;
using Text;
using UnityEngine;

public class GlowUpCharacter : MonoBehaviour
{
	public delegate void GlowUpEndCallback();

	private GlowUpCharaBaseInfo m_baseInfo;

	private GlowUpCharaAfterInfo m_afterInfo;

	private GlowUpExpBar m_expBar;

	private MenuPlayerSetAbilityButton m_abilityPanel;

	private bool m_isEndSetup;

	private bool m_isEnd;

	public bool IsEndSetup
	{
		get
		{
			return m_isEndSetup;
		}
	}

	public bool IsEnd
	{
		get
		{
			return m_isEnd;
		}
	}

	public void Setup(GlowUpCharaBaseInfo baseInfo)
	{
		m_baseInfo = baseInfo;
		if (!m_baseInfo.IsActive)
		{
			m_isEndSetup = true;
			m_isEnd = true;
			base.gameObject.SetActive(false);
		}
		else
		{
			StartCoroutine(OnSetup(baseInfo));
		}
	}

	private IEnumerator OnSetup(GlowUpCharaBaseInfo baseInfo)
	{
		yield return null;
		CharaType charaType = m_baseInfo.charaType;
		UISprite charaIcon = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_player");
		if (charaIcon != null)
		{
			string spriteName = charaIcon.spriteName = HudUtility.MakeCharaTextureName(charaType, HudUtility.TextureType.TYPE_S);
		}
		UILabel levelLabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_player_lv");
		if (levelLabel != null)
		{
			levelLabel.text = TextUtility.GetTextLevel(m_baseInfo.level.ToString());
		}
		UILabel charaNameLabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_player_name");
		if (charaNameLabel != null)
		{
			string charaName = charaNameLabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "CharaName", CharaName.Name[(int)charaType]).text;
		}
		UISprite typeSprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_player_speacies");
		if (typeSprite != null)
		{
			typeSprite.spriteName = HudUtility.GetCharaAttributeSpriteName(charaType);
		}
		UISprite teamTypeSprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_player_genus");
		if (teamTypeSprite != null)
		{
			teamTypeSprite.spriteName = HudUtility.GetTeamAttributeSpriteName(charaType);
		}
		if (m_expBar == null)
		{
			m_expBar = base.gameObject.AddComponent<GlowUpExpBar>();
			UISlider baseSlider = GameObjectUtil.FindChildGameObjectComponent<UISlider>(base.gameObject, "Pgb_b4exp");
			UISlider glowUpSlider = GameObjectUtil.FindChildGameObjectComponent<UISlider>(base.gameObject, "Pgb_exp");
			UILabel expLabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_price_number");
			if (m_expBar != null)
			{
				m_expBar.SetBaseSlider(baseSlider);
				m_expBar.SetGlowUpSlider(glowUpSlider);
				m_expBar.SetExpLabel(expLabel);
				m_expBar.SetCallback(ExpBarLevelUpCallback, ExpBarEndCallback);
			}
		}
		if (m_expBar != null)
		{
			GlowUpExpBar.ExpInfo startInfo = new GlowUpExpBar.ExpInfo
			{
				level = m_baseInfo.level,
				cost = m_baseInfo.levelUpCost,
				exp = m_baseInfo.currentExp
			};
			m_expBar.SetStartExp(startInfo);
		}
		if (m_abilityPanel == null)
		{
			GameObject abilityRootObject = GameObjectUtil.FindChildGameObject(base.gameObject, "ui_player_set_item_2_cell(Clone)");
			if (abilityRootObject != null)
			{
				m_abilityPanel = abilityRootObject.AddComponent<MenuPlayerSetAbilityButton>();
			}
		}
		m_isEndSetup = true;
	}

	public void PlayStart(GlowUpCharaAfterInfo afterInfo)
	{
		if (!m_isEnd)
		{
			StartCoroutine(OnPlayStart(afterInfo));
		}
	}

	public void PlaySkip()
	{
		if (m_expBar != null)
		{
			m_expBar.PlaySkip();
		}
		if (m_abilityPanel != null)
		{
			m_abilityPanel.SkipLevelUp();
		}
	}

	private IEnumerator OnPlayStart(GlowUpCharaAfterInfo afterInfo)
	{
		while (!m_isEndSetup)
		{
			yield return null;
		}
		m_afterInfo = afterInfo;
		m_isEnd = false;
		if (m_expBar != null)
		{
			GlowUpExpBar.ExpInfo endInfo = new GlowUpExpBar.ExpInfo
			{
				level = m_afterInfo.level,
				cost = m_afterInfo.levelUpCost,
				exp = m_afterInfo.exp
			};
			m_expBar.SetEndExp(endInfo);
			m_expBar.PlayStart();
			m_expBar.SetLevelUpCostList(m_afterInfo.abilityListExp);
		}
	}

	private void ExpBarLevelUpCallback(int level)
	{
		SoundManager.SePlay("sys_buy");
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_player_lv");
		if (uILabel != null)
		{
			uILabel.text = TextUtility.GetTextLevel(level.ToString());
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "img_slot_mask");
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
		if (m_abilityPanel != null && m_afterInfo.abilityList.Count > 0)
		{
			AbilityType abilityType = m_afterInfo.abilityList[0];
			m_abilityPanel.Setup(m_baseInfo.charaType, abilityType);
			m_abilityPanel.LevelUp(LevelUpAnimationEndCallback);
			m_afterInfo.abilityList.Remove(abilityType);
		}
	}

	private void LevelUpAnimationEndCallback()
	{
	}

	private void ExpBarEndCallback()
	{
		m_isEnd = true;
	}
}
