using Text;
using UnityEngine;

public class SpEggGetPartsNormal : SpEggGetPartsBase
{
	private int m_acquiredSpEggCount;

	public SpEggGetPartsNormal(int chaoId, int acquiredSpEggCount)
	{
		m_chaoId = chaoId;
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
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(spEggGetObjectRoot, "Lbl_item_name");
		if (uILabel != null)
		{
			string text2 = uILabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoRoulette", "sp_egg_name").text;
		}
		UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(spEggGetObjectRoot, "Lbl_item_number");
		if (uILabel2 != null)
		{
			TextObject text3 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Score", "number_of_pieces");
			text3.ReplaceTag("{NUM}", m_acquiredSpEggCount.ToString());
			uILabel2.text = text3.text;
		}
		UILabel uILabel3 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(spEggGetObjectRoot, "Lbl_info");
		if (uILabel3 != null)
		{
			TextObject text4 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoRoulette", "sp_egg_info");
			string text5 = TextManager.GetText(TextManager.TextType.TEXTTYPE_CHAO_TEXT, "Chao", ChaoWindowUtility.GetChaoLabelName(m_chaoId)).text;
			text4.ReplaceTag("{CHAO_NAME}", text5);
			uILabel3.text = text4.text;
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
