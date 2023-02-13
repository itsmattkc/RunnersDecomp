using UnityEngine;

public class HudQuickModeStagePicture
{
	private GameObject m_mainMenuObject;

	public void Initialize(GameObject mainMenuObject)
	{
		if (!(mainMenuObject == null))
		{
			m_mainMenuObject = mainMenuObject;
			UpdateDisplay();
		}
	}

	public void UpdateDisplay()
	{
		if (m_mainMenuObject == null)
		{
			return;
		}
		StageModeManager instance = StageModeManager.Instance;
		if (instance == null)
		{
			return;
		}
		CharacterAttribute quickStageCharaAttribute = instance.QuickStageCharaAttribute;
		int quickStageIndex = instance.QuickStageIndex;
		UITexture uITexture = null;
		UISprite uISprite = null;
		UISprite uISprite2 = null;
		UISprite uISprite3 = null;
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_mainMenuObject, "Anchor_5_MC");
		if (gameObject == null)
		{
			return;
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "1_Quick");
		if (gameObject2 == null)
		{
			return;
		}
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(gameObject2, "Btn_3_play");
		if (gameObject3 == null)
		{
			return;
		}
		uITexture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(gameObject3, "img_tex_next_map");
		uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject3, "img_icon_type_1");
		uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject3, "img_icon_type_2");
		uISprite3 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject3, "img_icon_type_3");
		if (uITexture != null)
		{
			TextureRequestStagePicture request = new TextureRequestStagePicture(quickStageIndex, uITexture);
			TextureAsyncLoadManager.Instance.Request(request);
		}
		if (quickStageCharaAttribute == CharacterAttribute.UNKNOWN)
		{
			if (uISprite != null)
			{
				uISprite.gameObject.SetActive(true);
				uISprite.spriteName = CalcCharaTypeSpriteName(CharacterAttribute.SPEED);
			}
			if (uISprite2 != null)
			{
				uISprite2.gameObject.SetActive(true);
				uISprite2.spriteName = CalcCharaTypeSpriteName(CharacterAttribute.FLY);
			}
			if (uISprite3 != null)
			{
				uISprite3.gameObject.SetActive(true);
				uISprite3.spriteName = CalcCharaTypeSpriteName(CharacterAttribute.POWER);
			}
		}
		else
		{
			if (uISprite != null)
			{
				uISprite.gameObject.SetActive(true);
				uISprite.spriteName = CalcCharaTypeSpriteName(quickStageCharaAttribute);
			}
			if (uISprite2 != null)
			{
				uISprite2.gameObject.SetActive(false);
			}
			if (uISprite3 != null)
			{
				uISprite3.gameObject.SetActive(false);
			}
		}
	}

	private static string CalcCharaTypeSpriteName(CharacterAttribute charaAttribute)
	{
		string text = "ui_chao_set_type_icon_";
		switch (charaAttribute)
		{
		case CharacterAttribute.SPEED:
			text += "speed";
			break;
		case CharacterAttribute.FLY:
			text += "fly";
			break;
		case CharacterAttribute.POWER:
			text += "power";
			break;
		}
		return text;
	}
}
