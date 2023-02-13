using System.Collections.Generic;
using Text;
using UnityEngine;

public class HudCharacterPanelUtil
{
	public static void SetImage(CharaType chara_type, GameObject obj)
	{
		if (obj != null)
		{
			UISprite component = obj.GetComponent<UISprite>();
			if (component != null)
			{
				int num = (int)chara_type;
				string text2 = component.spriteName = "ui_tex_player_" + num.ToString("00") + "_" + CharaName.PrefixName[(int)chara_type];
			}
		}
	}

	public static void SetIcon(CharaType chara_type, GameObject obj)
	{
		if (obj != null)
		{
			UISprite component = obj.GetComponent<UISprite>();
			if (component != null)
			{
				int num = (int)chara_type;
				string text2 = component.spriteName = "ui_tex_player_set_" + num.ToString("00") + "_" + CharaName.PrefixName[(int)chara_type];
			}
		}
	}

	public static void SetName(CharaType chara_type, GameObject obj)
	{
		if (!(obj != null))
		{
			return;
		}
		UILabel component = obj.GetComponent<UILabel>();
		if (component != null)
		{
			if (chara_type != CharaType.UNKNOWN)
			{
				component.text = TextUtility.GetCommonText("CharaName", CharaName.Name[(int)chara_type]);
			}
			else
			{
				component.text = string.Empty;
			}
		}
	}

	public static void SetLevel(CharaType chara_type, GameObject obj)
	{
		if (obj != null)
		{
			UILabel component = obj.GetComponent<UILabel>();
			if (component != null)
			{
				component.text = TextUtility.GetTextLevel(SaveDataUtil.GetCharaLevel(chara_type).ToString());
			}
		}
	}

	public static void SetCharaType(CharaType chara_type, GameObject obj)
	{
		if (!(obj != null))
		{
			return;
		}
		if (chara_type == CharaType.UNKNOWN)
		{
			SetGameObjectActive(obj, false);
			return;
		}
		SetGameObjectActive(obj, true);
		UISprite component = obj.GetComponent<UISprite>();
		if (component != null)
		{
			component.spriteName = HudUtility.GetCharaAttributeSpriteName(chara_type);
		}
	}

	public static void SetTeamType(CharaType chara_type, GameObject obj)
	{
		if (!(obj != null))
		{
			return;
		}
		if (chara_type == CharaType.UNKNOWN)
		{
			SetGameObjectActive(obj, false);
			return;
		}
		SetGameObjectActive(obj, true);
		UISprite component = obj.GetComponent<UISprite>();
		if (component != null)
		{
			component.spriteName = HudUtility.GetTeamAttributeSpriteName(chara_type);
		}
	}

	public static void SetGameObjectActive(GameObject obj, bool active_flag)
	{
		if (obj != null)
		{
			obj.SetActive(active_flag);
		}
	}

	public static bool CheckValidChara(CharaType chara_type)
	{
		if (CharaType.SONIC <= chara_type && chara_type < CharaType.NUM)
		{
			return true;
		}
		return false;
	}

	public static void SetChaoImage(int chaoId, GameObject obj)
	{
		if (!(obj != null))
		{
			return;
		}
		UITexture component = obj.GetComponent<UITexture>();
		if (component != null)
		{
			if (!obj.activeSelf)
			{
				obj.SetActive(true);
			}
			ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(component, null, true);
			ChaoTextureManager.Instance.GetTexture(chaoId, info);
		}
		else
		{
			obj.SetActive(false);
		}
	}

	public static void SetupNoticeView(BonusParamContainer bonus, UILabel detailTextLabel, UISprite detailTextBg)
	{
		string a = string.Empty;
		string detailText;
		if (bonus.IsDetailInfo(out detailText) && detailTextLabel != null)
		{
			a = detailTextLabel.text;
			detailTextLabel.text = detailText;
		}
		if (!(detailTextBg != null) || !(detailTextLabel != null))
		{
			return;
		}
		TweenAlpha component = detailTextBg.GetComponent<TweenAlpha>();
		TweenAlpha component2 = detailTextLabel.GetComponent<TweenAlpha>();
		if (!string.IsNullOrEmpty(detailText))
		{
			if (component != null && component2 != null)
			{
				if (a != detailText)
				{
					component.Reset();
					component2.Reset();
					detailTextBg.alpha = 0f;
					detailTextLabel.alpha = 0f;
					component.enabled = true;
					component2.enabled = true;
					component.Play();
					component2.Play();
				}
				else if (!component.enabled)
				{
					detailTextBg.alpha = 0f;
					detailTextLabel.alpha = 0f;
					component.enabled = true;
					component2.enabled = true;
					component.Play();
					component2.Play();
				}
			}
		}
		else
		{
			detailTextBg.alpha = 0f;
			detailTextLabel.alpha = 0f;
			if (component != null && component2 != null)
			{
				component.Reset();
				component2.Reset();
				component.enabled = false;
				component2.enabled = false;
			}
		}
	}

	public static void SetupAbilityIcon(BonusParamContainer bonus, CharaType playerMain, CharaType playerSub, GameObject player_set)
	{
		if (!(player_set != null))
		{
			return;
		}
		List<UISprite> icons = new List<UISprite>();
		List<UISprite> icons2 = new List<UISprite>();
		GameObject gameObject = GameObjectUtil.FindChildGameObject(player_set, "Btn_player_main");
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(player_set, "Btn_player_sub");
		if (gameObject != null)
		{
			GameObject gameObject3 = GameObjectUtil.FindChildGameObject(gameObject, "ability");
			if (gameObject3 != null)
			{
				for (int i = 0; i < 7; i++)
				{
					UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject3, "img_ability_icon_" + i);
					if (uISprite != null)
					{
						icons.Add(uISprite);
						continue;
					}
					break;
				}
			}
		}
		if (gameObject2 != null)
		{
			GameObject gameObject4 = GameObjectUtil.FindChildGameObject(gameObject2, "ability");
			if (gameObject4 != null)
			{
				for (int j = 0; j < 7; j++)
				{
					UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject4, "img_ability_icon_" + j);
					if (uISprite2 != null)
					{
						icons2.Add(uISprite2);
						continue;
					}
					break;
				}
			}
		}
		if (bonus != null)
		{
			if (icons.Count > 0)
			{
				for (int k = 0; k < icons.Count; k++)
				{
					icons[k].enabled = false;
					icons[k].gameObject.SetActive(false);
				}
			}
			if (icons2.Count > 0)
			{
				for (int l = 0; l < icons2.Count; l++)
				{
					icons2[l].enabled = false;
					icons2[l].gameObject.SetActive(false);
				}
			}
			BonusParam bonusParam = bonus.GetBonusParam(0);
			BonusParam bonusParam2 = bonus.GetBonusParam(1);
			if (bonusParam != null && bonusParam.GetBonusInfo(BonusParam.BonusTarget.CHARA) != null)
			{
				Dictionary<BonusParam.BonusType, float> bonusInfo = bonusParam.GetBonusInfo(BonusParam.BonusTarget.CHARA);
				SetAbilityIconSprite(ref icons, bonusInfo, playerMain);
			}
			if (bonusParam2 != null && bonusParam2.GetBonusInfo(BonusParam.BonusTarget.CHARA) != null)
			{
				Dictionary<BonusParam.BonusType, float> bonusInfo2 = bonusParam2.GetBonusInfo(BonusParam.BonusTarget.CHARA);
				SetAbilityIconSprite(ref icons2, bonusInfo2, playerSub);
			}
			return;
		}
		if (icons.Count > 0)
		{
			for (int m = 0; m < icons.Count; m++)
			{
				icons[m].enabled = false;
			}
		}
		if (icons2.Count > 0)
		{
			for (int n = 0; n < icons2.Count; n++)
			{
				icons2[n].enabled = false;
			}
		}
	}

	private static void SetAbilityIconSprite(ref List<UISprite> icons, Dictionary<BonusParam.BonusType, float> info, CharaType charaType)
	{
		if (info == null || icons == null)
		{
			return;
		}
		string text = "ui_chao_set_ability_icon_{PARAM}";
		if (info.Count <= 0)
		{
			return;
		}
		int num = 0;
		Dictionary<BonusParam.BonusType, float>.KeyCollection keys = info.Keys;
		List<BonusParam.BonusType> list = new List<BonusParam.BonusType>();
		foreach (BonusParam.BonusType item in keys)
		{
			list.Add(item);
		}
		Dictionary<BonusParam.BonusType, bool> dictionary = BonusUtil.IsTeamBonus(charaType, list);
		foreach (BonusParam.BonusType item2 in keys)
		{
			if (info[item2] == 0f || dictionary == null || !dictionary.ContainsKey(item2) || !dictionary[item2])
			{
				continue;
			}
			switch (item2)
			{
			case BonusParam.BonusType.SCORE:
				if (BonusUtil.IsBonusMerit(item2, info[item2]))
				{
					icons[num].spriteName = text.Replace("{PARAM}", "Uscore");
				}
				else
				{
					icons[num].spriteName = text.Replace("{PARAM}", "Dscore");
				}
				icons[num].enabled = true;
				icons[num].gameObject.SetActive(true);
				num++;
				break;
			case BonusParam.BonusType.RING:
				if (BonusUtil.IsBonusMerit(item2, info[item2]))
				{
					icons[num].spriteName = text.Replace("{PARAM}", "Uring");
				}
				else
				{
					icons[num].spriteName = text.Replace("{PARAM}", "Dring");
				}
				icons[num].enabled = true;
				icons[num].gameObject.SetActive(true);
				num++;
				break;
			case BonusParam.BonusType.ANIMAL:
				if (BonusUtil.IsBonusMerit(item2, info[item2]))
				{
					icons[num].spriteName = text.Replace("{PARAM}", "Uanimal");
				}
				else
				{
					icons[num].spriteName = text.Replace("{PARAM}", "Danimal");
				}
				icons[num].enabled = true;
				icons[num].gameObject.SetActive(true);
				num++;
				break;
			case BonusParam.BonusType.DISTANCE:
				if (BonusUtil.IsBonusMerit(item2, info[item2]))
				{
					icons[num].spriteName = text.Replace("{PARAM}", "Urange");
				}
				else
				{
					icons[num].spriteName = text.Replace("{PARAM}", "Drange");
				}
				icons[num].enabled = true;
				icons[num].gameObject.SetActive(true);
				num++;
				break;
			case BonusParam.BonusType.ENEMY_OBJBREAK:
				if (BonusUtil.IsBonusMerit(item2, info[item2]))
				{
					icons[num].spriteName = text.Replace("{PARAM}", "Uenemy");
				}
				else
				{
					icons[num].spriteName = text.Replace("{PARAM}", "Denemy");
				}
				icons[num].enabled = true;
				icons[num].gameObject.SetActive(true);
				num++;
				break;
			case BonusParam.BonusType.TOTAL_SCORE:
				if (BonusUtil.IsBonusMerit(item2, info[item2]))
				{
					icons[num].spriteName = text.Replace("{PARAM}", "Ufscore");
				}
				else
				{
					icons[num].spriteName = text.Replace("{PARAM}", "Dfscore");
				}
				icons[num].enabled = true;
				icons[num].gameObject.SetActive(true);
				num++;
				break;
			case BonusParam.BonusType.SPEED:
				if (BonusUtil.IsBonusMerit(item2, info[item2]))
				{
					if (info[item2] > 100f)
					{
						icons[num].spriteName = text.Replace("{PARAM}", "Uspeed");
					}
					else
					{
						icons[num].spriteName = text.Replace("{PARAM}", "Dspeed");
					}
					icons[num].enabled = true;
					icons[num].gameObject.SetActive(true);
					num++;
				}
				break;
			}
		}
	}
}
