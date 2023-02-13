using Text;
using UnityEngine;

public class SpEggGetPartsRare : SpEggGetPartsBase
{
	private int m_rarity;

	private int m_acquiredSpEggCount;

	public SpEggGetPartsRare(int chaoId, int rarity, int acquiredSpEggCount)
	{
		m_chaoId = chaoId;
		m_rarity = rarity;
		m_acquiredSpEggCount = acquiredSpEggCount;
	}

	public override void Setup(GameObject spEggGetObjectRoot)
	{
		UITexture uITexture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(spEggGetObjectRoot, "img_chao_1");
		if (uITexture != null)
		{
			int idFromServerId = ChaoWindowUtility.GetIdFromServerId(m_chaoId);
			ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(uITexture, null, true);
			ChaoTextureManager.Instance.GetTexture(idFromServerId, info);
		}
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(spEggGetObjectRoot, "img_egg_0");
		if (uISprite != null)
		{
			string text2 = uISprite.spriteName = "ui_roulette_egg_" + 2 * m_rarity;
		}
		UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(spEggGetObjectRoot, "img_egg_1");
		if (uISprite2 != null)
		{
			string text4 = uISprite2.spriteName = "ui_roulette_egg_" + (2 * m_rarity + 1);
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(spEggGetObjectRoot, "Lbl_item_name");
		if (uILabel != null)
		{
			string text6 = uILabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoRoulette", "sp_egg_name").text;
		}
		UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(spEggGetObjectRoot, "Lbl_item_number");
		if (uILabel2 != null)
		{
			TextObject text7 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Score", "number_of_pieces");
			text7.ReplaceTag("{NUM}", m_acquiredSpEggCount.ToString());
			uILabel2.text = text7.text;
		}
		UILabel uILabel3 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(spEggGetObjectRoot, "Lbl_info");
		if (uILabel3 != null)
		{
			TextObject text8 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoRoulette", "sp_egg_info");
			string text9 = TextManager.GetText(TextManager.TextType.TEXTTYPE_CHAO_TEXT, "Chao", ChaoWindowUtility.GetChaoLabelName(m_chaoId)).text;
			text8.ReplaceTag("{CHAO_NAME}", text9);
			uILabel3.text = text8.text;
		}
	}

	public override void PlaySE(string seType)
	{
		if (seType == ChaoWindowUtility.SeHatch)
		{
			SoundManager.SePlay("sys_chao_hatch");
		}
		else if (seType == ChaoWindowUtility.SeSpEgg)
		{
			SoundManager.SePlay("sys_specialegg");
		}
		else if (seType == ChaoWindowUtility.SeBreak)
		{
			SoundManager.SePlay("sys_chao_birth");
		}
	}
}
