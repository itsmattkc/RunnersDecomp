using AnimationOrTween;
using System.Collections;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class PlayerGetPartsOverlap : ChaoGetPartsBase
{
	public enum IntroType
	{
		NORMAL,
		NO_EGG,
		NONE
	}

	private int m_severId;

	private int m_rarity;

	private int m_level;

	private int m_currentAnimCount;

	private List<ServerItem> m_itemList;

	private List<int> m_itemListNum;

	private CharacterDataNameInfo.Info m_info;

	private Animation m_animation;

	private GameObject m_rootGameObject;

	private int m_playSeCount;

	private IntroType m_introType;

	private BtnType m_buttonType;

	private UILabel m_caption;

	public void Init(int severId, int rarity, int level, Dictionary<int, ServerItemState> itemList, IntroType introType = IntroType.NORMAL)
	{
		m_playSeCount = 0;
		m_severId = severId;
		m_rarity = rarity;
		m_level = level;
		m_introType = introType;
		m_itemListNum = null;
		m_itemList = null;
		if (itemList != null && itemList.Count > 0)
		{
			m_itemListNum = new List<int>();
			m_itemList = new List<ServerItem>();
			Dictionary<int, ServerItemState>.KeyCollection keys = itemList.Keys;
			foreach (int item2 in keys)
			{
				ServerItem item = new ServerItem((ServerItem.Id)item2);
				m_itemList.Add(item);
				m_itemListNum.Add(itemList[item2].m_num);
			}
		}
		m_currentAnimCount = 0;
		m_info = CharacterDataNameInfo.Instance.GetDataByServerID(m_severId);
	}

	public override void Setup(GameObject chaoGetObjectRoot)
	{
		m_rootGameObject = chaoGetObjectRoot;
		GameObject gameObject = GameObjectUtil.FindChildGameObject(chaoGetObjectRoot, "eff_set");
		gameObject.SetActive(true);
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(chaoGetObjectRoot, "img_egg_0");
		if (uISprite != null)
		{
			string text2 = uISprite.spriteName = "ui_roulette_egg_" + 2 * m_rarity;
		}
		UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(chaoGetObjectRoot, "img_egg_1");
		if (uISprite2 != null)
		{
			string text4 = uISprite2.spriteName = "ui_roulette_egg_" + (2 * m_rarity + 1);
		}
		UITexture uITexture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(chaoGetObjectRoot, "img_player");
		UISprite uISprite3 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(chaoGetObjectRoot, "img_item");
		if (uITexture != null && m_info != null)
		{
			uITexture.gameObject.SetActive(true);
			TextureRequestChara request = new TextureRequestChara(m_info.m_ID, uITexture);
			TextureAsyncLoadManager.Instance.Request(request);
		}
		if (uISprite3 != null)
		{
			uISprite3.gameObject.SetActive(false);
		}
		UISprite uISprite4 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(chaoGetObjectRoot, "img_player_speacies");
		if (uISprite4 != null && m_info != null)
		{
			uISprite4.gameObject.SetActive(true);
			switch (m_info.m_attribute)
			{
			case CharacterAttribute.FLY:
				uISprite4.spriteName = "ui_mm_player_species_1";
				break;
			case CharacterAttribute.POWER:
				uISprite4.spriteName = "ui_mm_player_species_2";
				break;
			case CharacterAttribute.SPEED:
				uISprite4.spriteName = "ui_mm_player_species_0";
				break;
			}
		}
		UISprite uISprite5 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(chaoGetObjectRoot, "img_player_genus");
		if (uISprite5 != null && m_info != null)
		{
			uISprite5.gameObject.SetActive(true);
			uISprite5.spriteName = HudUtility.GetTeamAttributeSpriteName(m_info.m_ID);
		}
		m_caption = GameObjectUtil.FindChildGameObjectComponent<UILabel>(chaoGetObjectRoot, "Lbl_caption");
		if (m_caption != null)
		{
			m_caption.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "PlayerSet", "ui_Lbt_get_captions").text;
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(chaoGetObjectRoot, "Lbl_player_name");
		if (uILabel != null && m_info != null)
		{
			uILabel.gameObject.SetActive(true);
			uILabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "CharaName", m_info.m_name.ToLower()).text;
		}
		UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(chaoGetObjectRoot, "Lbl_player_lv");
		if (uILabel2 != null && m_info != null)
		{
			uILabel2.gameObject.SetActive(true);
			uILabel2.text = string.Format("Lv.{0:D3}", m_level);
		}
		UILabel uILabel3 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(chaoGetObjectRoot, "Lbl_player_attribute");
		if (uILabel3 != null && m_info != null)
		{
			uILabel3.gameObject.SetActive(true);
			ServerPlayerState playerState = ServerInterface.PlayerState;
			if (playerState != null)
			{
				ServerCharacterState serverCharacterState = playerState.CharacterState(m_info.m_ID);
				uILabel3.text = serverCharacterState.GetCharaAttributeText();
			}
		}
		UILabel uILabel4 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(chaoGetObjectRoot, "Lbl_item_name");
		UILabel uILabel5 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(chaoGetObjectRoot, "Lbl_item_info");
		UILabel uILabel6 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(chaoGetObjectRoot, "Lbl_item_number");
		if (uILabel4 != null)
		{
			uILabel4.gameObject.SetActive(false);
		}
		if (uILabel5 != null)
		{
			uILabel5.gameObject.SetActive(false);
		}
		if (uILabel6 != null)
		{
			uILabel6.gameObject.SetActive(false);
		}
	}

	private void SetupItem()
	{
		ServerItem serverItem = m_itemList[m_currentAnimCount];
		int num = m_itemListNum[m_currentAnimCount];
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_rootGameObject, "img_player");
		UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_rootGameObject, "img_item");
		if (uISprite != null && m_info != null)
		{
			uISprite.gameObject.SetActive(false);
		}
		if (uISprite2 != null && AtlasManager.Instance != null)
		{
			uISprite2.atlas = AtlasManager.Instance.ItemAtlas;
			uISprite2.spriteName = serverItem.serverItemSpriteName;
			uISprite2.gameObject.SetActive(true);
		}
		UISprite uISprite3 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_rootGameObject, "img_player_speacies");
		if (uISprite3 != null && m_info != null)
		{
			uISprite3.gameObject.SetActive(false);
		}
		UISprite uISprite4 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_rootGameObject, "img_player_genus");
		if (uISprite4 != null && m_info != null)
		{
			uISprite4.gameObject.SetActive(false);
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_rootGameObject, "Lbl_player_name");
		if (uILabel != null && m_info != null)
		{
			uILabel.gameObject.SetActive(false);
		}
		UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_rootGameObject, "Lbl_player_lv");
		if (uILabel2 != null && m_info != null)
		{
			uILabel2.gameObject.SetActive(false);
		}
		UILabel uILabel3 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_rootGameObject, "Lbl_player_attribute");
		if (uILabel3 != null && m_info != null)
		{
			uILabel3.gameObject.SetActive(false);
		}
		UILabel uILabel4 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_rootGameObject, "Lbl_item_name");
		UILabel uILabel5 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_rootGameObject, "Lbl_item_info");
		UILabel uILabel6 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_rootGameObject, "Lbl_item_number");
		if (uILabel4 != null)
		{
			uILabel4.gameObject.SetActive(true);
			uILabel4.text = serverItem.serverItemName;
			if (m_caption != null)
			{
				string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoRoulette", "overlap_get_item_caption").text;
				if (string.IsNullOrEmpty(text))
				{
					m_caption.text = serverItem.serverItemName;
				}
				else
				{
					m_caption.text = text.Replace("{ITEM}", serverItem.serverItemName);
				}
			}
		}
		if (uILabel5 != null)
		{
			uILabel5.gameObject.SetActive(true);
			uILabel5.text = serverItem.serverItemComment;
			UIDraggablePanel uIDraggablePanel = GameObjectUtil.FindChildGameObjectComponent<UIDraggablePanel>(m_rootGameObject, "window_chaoset_alpha_clip");
			if (uIDraggablePanel != null)
			{
				uIDraggablePanel.ResetPosition();
			}
		}
		if (uILabel6 != null)
		{
			uILabel6.gameObject.SetActive(true);
			uILabel6.text = string.Format("× {0:0000}", num);
		}
	}

	public override void PlayGetAnimation(Animation anim)
	{
		if (anim == null)
		{
			return;
		}
		if (m_currentAnimCount == 0)
		{
			m_animation = anim;
			if (m_itemList != null && m_itemList.Count > 0)
			{
				ActiveAnimation activeAnimation = ActiveAnimation.Play(anim, "ui_ro_PlayerGetWindowUI_1_Anim", Direction.Forward);
				EventDelegate.Add(activeAnimation.onFinished, GetAnimationFinishCallback, true);
				m_buttonType = BtnType.NEXT;
				return;
			}
			string clipName = "ui_ro_PlayerGetWindowUI_intro_Anim";
			switch (m_introType)
			{
			case IntroType.NO_EGG:
			case IntroType.NONE:
				clipName = "ui_ro_PlayerGetWindowUI_noegg_intro_Anim";
				break;
			}
			ActiveAnimation activeAnimation2 = ActiveAnimation.Play(anim, clipName, Direction.Forward);
			EventDelegate.Add(activeAnimation2.onFinished, GetAnimationFinishCallback, true);
			m_buttonType = BtnType.EQUIP_OK;
		}
		else
		{
			SetupItem();
			m_animation = null;
			ActiveAnimation activeAnimation3 = ActiveAnimation.Play(anim, "ui_ro_PlayerGetWindowUI_2_Anim", Direction.Forward);
			EventDelegate.Add(activeAnimation3.onFinished, GetAnimationFinishCallback, true);
			if (m_currentAnimCount >= m_itemList.Count - 1)
			{
				m_buttonType = BtnType.OK;
			}
			else
			{
				m_buttonType = BtnType.NEXT;
			}
		}
	}

	public override BtnType GetButtonType()
	{
		if (RouletteUtility.loginRoulette)
		{
			return BtnType.OK;
		}
		return m_buttonType;
	}

	public override void PlayEndAnimation(Animation anim)
	{
		if (!(anim == null))
		{
			if (m_itemList != null && m_itemList.Count > 0)
			{
				ActiveAnimation activeAnimation = ActiveAnimation.Play(anim, "ui_ro_PlayerGetWindowUI_3_Anim", Direction.Forward);
				EventDelegate.Add(activeAnimation.onFinished, EndAnimationFinishCallback, true);
			}
			else
			{
				ActiveAnimation activeAnimation2 = ActiveAnimation.Play(anim, "ui_ro_PlayerGetWindowUI_outro_Anim", Direction.Forward);
				EventDelegate.Add(activeAnimation2.onFinished, EndAnimationFinishCallback, true);
			}
		}
	}

	public override void PlaySE(string seType)
	{
		if (seType == ChaoWindowUtility.SeHatch)
		{
			if (m_playSeCount > 0)
			{
				SoundManager.SePlay("sys_roulette_itemget");
			}
			else
			{
				SoundManager.SePlay("sys_chao_hatch");
			}
			m_playSeCount++;
		}
		else if (seType == ChaoWindowUtility.SeBreak)
		{
			SoundManager.SePlay("sys_chao_birthS");
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
		if (m_currentAnimCount == 0 && m_itemList != null && m_itemList.Count > 0 && m_animation != null)
		{
			StartCoroutine(FirstOverlap());
			return;
		}
		m_currentAnimCount++;
		if (m_itemList == null || m_currentAnimCount >= m_itemList.Count)
		{
			m_callback(AnimType.GET_ANIM_FINISH);
		}
		else
		{
			m_callback(AnimType.GET_ANIM_CONTINUE);
		}
	}

	private IEnumerator FirstOverlap()
	{
		yield return null;
		SetupItem();
		ActiveAnimation activeAnim = ActiveAnimation.Play(m_animation, "ui_ro_PlayerGetWindowUI_2_Anim", Direction.Forward);
		EventDelegate.Add(activeAnim.onFinished, GetAnimationFinishCallback, true);
		m_animation = null;
	}

	private void EndAnimationFinishCallback()
	{
		m_callback(AnimType.OUT_ANIM);
	}
}
