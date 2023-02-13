using System.Collections.Generic;
using Text;
using UnityEngine;

public class DailyInfoChallenge : MonoBehaviour
{
	private DailyInfo m_parent;

	private daily_challenge.DailyMissionInfo m_info;

	public void Setup(DailyInfo parent)
	{
		m_parent = parent;
		m_info = daily_challenge.GetInfoFromSaveData(-1L);
		if (m_info == null)
		{
			return;
		}
		UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(base.gameObject, "Btn_detail");
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_days");
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "today_set");
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "tomorrow_set");
		if (uIButtonMessage != null)
		{
			uIButtonMessage.target = m_parent.gameObject;
			uIButtonMessage.functionName = "OnClickChallenge";
		}
		if (uILabel != null)
		{
			uILabel.text = TextUtility.Replaces(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "day").text, new Dictionary<string, string>
			{
				{
					"{DAY}",
					(m_info.DayMax - m_info.DayIndex).ToString()
				}
			});
		}
		if (gameObject != null)
		{
			UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject, "img_clear");
			UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_daily_challenge");
			UISlider uISlider = GameObjectUtil.FindChildGameObjectComponent<UISlider>(gameObject, "Pgb_attainment");
			GameObject target = GameObjectUtil.FindChildGameObject(gameObject, "item_today");
			SetupItem(target, m_info.IsClearTodayMission);
			if (uISprite != null)
			{
				uISprite.gameObject.SetActive(m_info.IsClearTodayMission);
			}
			if (uILabel2 != null)
			{
				uILabel2.text = TextUtility.Replaces(m_info.TodayMissionText, new Dictionary<string, string>
				{
					{
						"{QUOTA}",
						m_info.TodayMissionQuota.ToString()
					}
				});
			}
			if (uISlider != null)
			{
				float num = (float)m_info.TodayMissionClearQuota / (float)m_info.TodayMissionQuota;
				if (num > 1f)
				{
					num = 1f;
				}
				else if (num < 0f)
				{
					num = 0f;
				}
				uISlider.value = num;
			}
		}
		if (gameObject2 != null)
		{
			GameObject target2 = GameObjectUtil.FindChildGameObject(gameObject2, "item_tomorrow");
			SetupItem(target2, false);
		}
	}

	private void SetupItem(GameObject target, bool isClear)
	{
		if (!(target != null) || m_info == null)
		{
			return;
		}
		UITexture uITexture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(target, "img_chao");
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(target, "img_chara");
		UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(target, "img_daily_item");
		UISprite uISprite3 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(target, "img_check");
		if (uISprite3 != null)
		{
			uISprite3.gameObject.SetActive(isClear);
		}
		int num = m_info.DayIndex;
		if (num >= m_info.InceniveIdTable.Length)
		{
			num = m_info.InceniveIdTable.Length - 1;
		}
		int num2 = m_info.InceniveIdTable[num];
		int num3 = Mathf.FloorToInt((float)num2 / 100000f);
		if (uISprite2 != null)
		{
			uISprite2.gameObject.SetActive(num3 == 0);
			if (num3 == 0)
			{
				uISprite2.spriteName = ((num >= m_info.InceniveIdTable.Length - 1) ? "ui_cmn_icon_item_9" : ("ui_cmn_icon_item_" + num2));
			}
		}
		if (uISprite != null)
		{
			uISprite.gameObject.SetActive(num3 == 3);
			if (num3 == 3)
			{
				uISprite.spriteName = "ui_tex_player_" + CharaTypeUtil.GetCharaSpriteNameSuffix(new ServerItem((ServerItem.Id)num2).charaType);
			}
		}
		if (!(uITexture != null))
		{
			return;
		}
		uITexture.gameObject.SetActive(num3 == 4);
		if (num3 != 4)
		{
			return;
		}
		ChaoTextureManager instance = ChaoTextureManager.Instance;
		int chao_id = num2 - 400000;
		if (instance != null)
		{
			Texture loadedTexture = instance.GetLoadedTexture(chao_id);
			if (loadedTexture != null)
			{
				uITexture.mainTexture = loadedTexture;
				return;
			}
			ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(uITexture, null, true);
			instance.GetTexture(chao_id, info);
		}
	}
}
