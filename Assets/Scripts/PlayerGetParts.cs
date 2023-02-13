using AnimationOrTween;
using Text;
using UnityEngine;

public class PlayerGetParts : ChaoGetPartsBase
{
	private int m_severId;

	private int m_level;

	private CharacterDataNameInfo.Info m_info;

	public void Init(int severId, int level)
	{
		m_severId = severId;
		m_level = level;
		m_info = CharacterDataNameInfo.Instance.GetDataByServerID(m_severId);
	}

	public override void Setup(GameObject chaoGetObjectRoot)
	{
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(chaoGetObjectRoot, "img_player");
		if (uISprite != null && m_info != null)
		{
			uISprite.spriteName = m_info.characterSpriteName;
		}
		UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(chaoGetObjectRoot, "img_player_speacies");
		if (uISprite2 != null && m_info != null)
		{
			switch (m_info.m_attribute)
			{
			case CharacterAttribute.FLY:
				uISprite2.spriteName = "ui_mm_player_species_1";
				break;
			case CharacterAttribute.POWER:
				uISprite2.spriteName = "ui_mm_player_species_2";
				break;
			case CharacterAttribute.SPEED:
				uISprite2.spriteName = "ui_mm_player_species_0";
				break;
			}
		}
		UISprite uISprite3 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(chaoGetObjectRoot, "img_player_genus");
		if (uISprite3 != null && m_info != null)
		{
			uISprite3.spriteName = HudUtility.GetTeamAttributeSpriteName(m_info.m_ID);
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(chaoGetObjectRoot, "Lbl_player_name");
		if (uILabel != null && m_info != null)
		{
			uILabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "CharaName", m_info.m_name.ToLower()).text;
		}
		UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(chaoGetObjectRoot, "Lbl_player_lv");
		if (uILabel2 != null && m_info != null)
		{
			uILabel2.text = string.Format("Lv.{0:D3}", m_level);
		}
		UILabel uILabel3 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(chaoGetObjectRoot, "Lbl_player_attribute");
		if (uILabel3 != null && m_info != null)
		{
			ServerPlayerState playerState = ServerInterface.PlayerState;
			if (playerState != null)
			{
				ServerCharacterState serverCharacterState = playerState.CharacterState(m_info.m_ID);
				uILabel3.text = serverCharacterState.GetCharaAttributeText();
			}
		}
	}

	public override void PlayGetAnimation(Animation anim)
	{
		if (!(anim == null))
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(anim, "ui_ro_PlayerGetWindowUI_intro_Anim", Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, GetAnimationFinishCallback, true);
		}
	}

	public override BtnType GetButtonType()
	{
		return BtnType.EQUIP_OK;
	}

	public override void PlayEndAnimation(Animation anim)
	{
		if (!(anim == null))
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(anim, "ui_ro_PlayerGetWindowUI_outro_Anim", Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, EndAnimationFinishCallback, true);
		}
	}

	public override void PlaySE(string seType)
	{
		if (!(seType == ChaoWindowUtility.SeHatch) && seType == ChaoWindowUtility.SeBreak)
		{
			SoundManager.SePlay("sys_chao_birth");
		}
	}

	public override EasySnsFeed CreateEasySnsFeed(GameObject rootObject)
	{
		string anchorPath = "Camera/menu_Anim/RouletteUI/Anchor_5_MC";
		string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoRoulette", "feed_chao_get_caption").text;
		TextObject text2 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoRoulette", "feed_chao_get_text");
		string text3 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "CharaName", m_info.m_name.ToLower()).text;
		text2.ReplaceTag("{CHAO}", text3);
		return new EasySnsFeed(rootObject, anchorPath, text, text2.text);
	}

	private void GetAnimationFinishCallback()
	{
		m_callback(AnimType.GET_ANIM_FINISH);
	}

	private void EndAnimationFinishCallback()
	{
		m_callback(AnimType.OUT_ANIM);
	}
}
