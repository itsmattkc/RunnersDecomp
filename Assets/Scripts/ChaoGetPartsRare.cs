using AnimationOrTween;
using DataTable;
using Text;
using UnityEngine;

public class ChaoGetPartsRare : ChaoGetPartsBase
{
	private int m_rarity;

	public void Init(int chaoId, int rarity)
	{
		m_chaoId = chaoId;
		m_rarity = rarity;
	}

	public override void Setup(GameObject chaoGetObjectRoot)
	{
		UITexture uITexture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(chaoGetObjectRoot, "img_chao_1");
		if (uITexture != null)
		{
			int idFromServerId = ChaoWindowUtility.GetIdFromServerId(m_chaoId);
			ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(uITexture, null, true);
			ChaoTextureManager.Instance.GetTexture(idFromServerId, info);
		}
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(chaoGetObjectRoot, "img_chao_bg_rare");
		if (uISprite != null)
		{
			ChaoWindowUtility.ChangeRaritySpriteFromServerChaoId(uISprite, m_chaoId);
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(chaoGetObjectRoot, "Lbl_chao_name");
		if (uILabel != null)
		{
			string text2 = uILabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_CHAO_TEXT, "Chao", ChaoWindowUtility.GetChaoLabelName(m_chaoId)).text;
		}
		UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(chaoGetObjectRoot, "Lbl_chao_lv");
		if (uILabel2 != null)
		{
			uILabel2.text = TextUtility.GetTextLevel("0");
		}
		UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(chaoGetObjectRoot, "img_type_icon");
		if (uISprite2 != null)
		{
			int idFromServerId2 = ChaoWindowUtility.GetIdFromServerId(m_chaoId);
			ChaoData chaoData = ChaoTable.GetChaoData(idFromServerId2);
			if (chaoData != null)
			{
				CharacterAttribute charaAtribute = chaoData.charaAtribute;
				string text4 = uISprite2.spriteName = "ui_chao_set_type_icon_" + charaAtribute.ToString().ToLower();
			}
		}
		UISprite uISprite3 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(chaoGetObjectRoot, "img_egg_0");
		if (uISprite3 != null)
		{
			string text6 = uISprite3.spriteName = "ui_roulette_egg_" + 2 * m_rarity;
		}
		UISprite uISprite4 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(chaoGetObjectRoot, "img_egg_1");
		if (uISprite4 != null)
		{
			string text8 = uISprite4.spriteName = "ui_roulette_egg_" + (2 * m_rarity + 1);
		}
		UILabel uILabel3 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(chaoGetObjectRoot, "Lbl_type");
		if (uILabel3 != null)
		{
			int idFromServerId3 = ChaoWindowUtility.GetIdFromServerId(m_chaoId);
			ChaoData chaoData2 = ChaoTable.GetChaoData(idFromServerId3);
			if (chaoData2 != null)
			{
				CharacterAttribute charaAtribute2 = chaoData2.charaAtribute;
				string cellName = charaAtribute2.ToString().ToLower();
				string text10 = uILabel3.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "CharaAtribute", cellName).text;
			}
		}
		UISprite uISprite5 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(chaoGetObjectRoot, "img_bonus_icon");
		if (uISprite5 != null)
		{
			uISprite5.enabled = false;
		}
		UILabel uILabel4 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(chaoGetObjectRoot, "Lbl_bonus");
		if (uILabel4 != null)
		{
			int idFromServerId4 = ChaoWindowUtility.GetIdFromServerId(m_chaoId);
			uILabel4.text = HudUtility.GetChaoAbilityText(idFromServerId4);
		}
	}

	public override void PlayGetAnimation(Animation anim)
	{
		if (!(anim == null))
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(anim, "ui_menu_chao_rare_get_Window_Anim", Direction.Forward);
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
			ActiveAnimation activeAnimation = ActiveAnimation.Play(anim, "ui_menu_chao_rare_get_Window_outro_Anim", Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, EndAnimationFinishCallback, true);
		}
	}

	public override void PlaySE(string seType)
	{
		if (seType == ChaoWindowUtility.SeHatch)
		{
			SoundManager.SePlay("sys_chao_hatch");
		}
		else if (seType == ChaoWindowUtility.SeBreak)
		{
			ChaoWindowUtility.PlaySEChaoBtrth(m_chaoId, m_rarity);
		}
	}

	public override EasySnsFeed CreateEasySnsFeed(GameObject rootObject)
	{
		string anchorPath = "Camera/menu_Anim/RouletteUI/Anchor_5_MC";
		string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoRoulette", "feed_chao_get_caption").text;
		TextObject text2 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoRoulette", "feed_chao_get_text");
		string text3 = TextManager.GetText(TextManager.TextType.TEXTTYPE_CHAO_TEXT, "Chao", RouletteUtility.GetChaoCellName(m_chaoId)).text;
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
