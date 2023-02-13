using System.Collections.Generic;
using UnityEngine;

public class RouletteItem : MonoBehaviour
{
	[SerializeField]
	private GameObject m_common;

	[SerializeField]
	private GameObject m_egg;

	[SerializeField]
	private GameObject m_item;

	[SerializeField]
	private GameObject m_rank;

	[SerializeField]
	private GameObject m_campaign;

	[SerializeField]
	private List<GameObject> m_campaignList;

	private RouletteBoard m_parent;

	private int m_cellIndex;

	private Vector3 m_basePos = new Vector3(0f, 0f, 0f);

	public bool isRank
	{
		get
		{
			bool result = false;
			if (m_parent != null && m_parent.wheelData != null)
			{
				ServerWheelOptionsData wheelData = m_parent.wheelData;
				switch (wheelData.wheelType)
				{
				case RouletteUtility.WheelType.Normal:
					result = false;
					break;
				case RouletteUtility.WheelType.Rankup:
					if (wheelData.GetCellItem(m_cellIndex).idType == ServerItem.IdType.ITEM_ROULLETE_WIN)
					{
						result = true;
					}
					break;
				}
			}
			return result;
		}
	}

	public void Setup(RouletteBoard parent, int cellIndex)
	{
		m_parent = parent;
		m_cellIndex = cellIndex;
		m_basePos = base.gameObject.transform.parent.localPosition;
		if (!(m_parent != null) || m_parent.wheelData == null)
		{
			return;
		}
		ServerWheelOptionsData wheelData = m_parent.wheelData;
		if (wheelData.isGeneral)
		{
			SetGeneral(wheelData);
		}
		else
		{
			switch (wheelData.wheelType)
			{
			case RouletteUtility.WheelType.Normal:
				SetEgg(wheelData);
				break;
			case RouletteUtility.WheelType.Rankup:
				SetItem(wheelData);
				break;
			}
		}
		SetTweenDelay();
	}

	private void SetTweenDelay()
	{
		TweenColor[] componentsInChildren = base.gameObject.GetComponentsInChildren<TweenColor>();
		TweenRotation[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<TweenRotation>();
		TweenAlpha[] componentsInChildren3 = base.gameObject.GetComponentsInChildren<TweenAlpha>();
		TweenScale[] componentsInChildren4 = base.gameObject.GetComponentsInChildren<TweenScale>();
		TweenPosition[] componentsInChildren5 = base.gameObject.GetComponentsInChildren<TweenPosition>();
		if (componentsInChildren != null)
		{
			TweenColor[] array = componentsInChildren;
			foreach (TweenColor tweenColor in array)
			{
				tweenColor.delay = Random.Range(0f, tweenColor.duration);
			}
		}
		if (componentsInChildren2 != null)
		{
			TweenRotation[] array2 = componentsInChildren2;
			foreach (TweenRotation tweenRotation in array2)
			{
				tweenRotation.delay = Random.Range(0f, tweenRotation.duration);
			}
		}
		if (componentsInChildren3 != null)
		{
			TweenAlpha[] array3 = componentsInChildren3;
			foreach (TweenAlpha tweenAlpha in array3)
			{
				tweenAlpha.delay = Random.Range(0f, tweenAlpha.duration);
			}
		}
		if (componentsInChildren4 != null)
		{
			TweenScale[] array4 = componentsInChildren4;
			foreach (TweenScale tweenScale in array4)
			{
				tweenScale.delay = Random.Range(0f, tweenScale.duration);
			}
		}
		if (componentsInChildren5 != null)
		{
			TweenPosition[] array5 = componentsInChildren5;
			foreach (TweenPosition tweenPosition in array5)
			{
				tweenPosition.delay = Random.Range(0f, tweenPosition.duration);
			}
		}
	}

	private void SetCampaign(ServerItem.Id itemId)
	{
		bool flag = false;
		bool flag2 = false;
		if (m_parent.wheelData.IsCampaign(Constants.Campaign.emType.PremiumRouletteOdds) && ServerInterface.CampaignState != null)
		{
			ServerCampaignData campaignInSession = ServerInterface.CampaignState.GetCampaignInSession(Constants.Campaign.emType.PremiumRouletteOdds, m_cellIndex);
			if (campaignInSession != null && campaignInSession.iContent > 0)
			{
				float cellWeight = m_parent.wheel.GetCellWeight(m_cellIndex);
				if ((float)campaignInSession.iContent > cellWeight)
				{
					flag = true;
				}
			}
		}
		if (m_parent.wheelData.IsCampaign(Constants.Campaign.emType.JackPotValueBonus) && ServerInterface.CampaignState != null)
		{
			ServerCampaignData campaignInSession2 = ServerInterface.CampaignState.GetCampaignInSession(Constants.Campaign.emType.JackPotValueBonus, m_cellIndex);
			if (campaignInSession2 != null && campaignInSession2.iContent > 0)
			{
				flag2 = true;
			}
		}
		if (!(m_campaign != null))
		{
			return;
		}
		bool active = false;
		if ((flag || flag2) && m_campaignList != null && m_campaignList.Count > 0)
		{
			foreach (GameObject campaign in m_campaignList)
			{
				bool flag3 = false;
				if (campaign.name.IndexOf("jackpot") != -1)
				{
					if (itemId == ServerItem.Id.JACKPOT && flag2)
					{
						flag3 = true;
					}
				}
				else if (itemId != ServerItem.Id.JACKPOT && flag)
				{
					flag3 = true;
					float num = 0f;
					num = 50f / m_basePos.magnitude;
					float num2 = m_basePos.x * num;
					float num3 = m_basePos.y * num;
					num3 += 45f + num3 / 10f;
					if (num3 <= 20f && Mathf.Abs(num2) <= 60f)
					{
						num2 = ((!(num2 >= 0f)) ? (-60f) : 60f);
						num3 = 20f;
					}
					campaign.gameObject.transform.localPosition = new Vector3(num2, num3, 0f);
				}
				campaign.SetActive(flag3);
				if (flag3)
				{
					active = true;
					break;
				}
			}
		}
		m_campaign.SetActive(active);
	}

	private void SetEgg(ServerWheelOptionsData data)
	{
		if (m_egg != null)
		{
			m_egg.SetActive(true);
			UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_egg, "main");
			uISprite.spriteName = "ui_roulette_chao_egg_" + data.GetCellEgg(m_cellIndex);
		}
		if (m_item != null)
		{
			m_item.SetActive(false);
		}
		if (m_rank != null)
		{
			m_rank.SetActive(false);
		}
		if (m_common != null)
		{
			m_common.SetActive(false);
		}
		SetCampaign(ServerItem.Id.CHAO_BEGIN);
	}

	private void SetItem(ServerWheelOptionsData data)
	{
		if (m_egg != null)
		{
			m_egg.SetActive(false);
		}
		if (m_item != null)
		{
			m_item.SetActive(false);
		}
		if (m_rank != null)
		{
			m_rank.SetActive(false);
		}
		if (m_common != null)
		{
			m_common.SetActive(false);
		}
		int num;
		ServerItem cellItem = data.GetCellItem(m_cellIndex, out num);
		if (cellItem.serverItemNum > 0)
		{
			num *= cellItem.serverItemNum;
		}
		switch (cellItem.idType)
		{
		case ServerItem.IdType.CHARA:
		case ServerItem.IdType.CHAO:
			if (m_egg != null)
			{
				m_egg.SetActive(true);
				UISprite uISprite6 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_egg, "main");
				uISprite6.spriteName = "ui_roulette_chao_egg_" + data.GetCellEgg(m_cellIndex);
			}
			break;
		case ServerItem.IdType.EQUIP_ITEM:
		case ServerItem.IdType.RSRING:
		case ServerItem.IdType.RING:
		case ServerItem.IdType.ENERGY:
			base.transform.localScale = new Vector3(1f, 1f, 1f);
			if (m_item != null)
			{
				m_item.SetActive(true);
				UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_item, "main");
				uISprite2.spriteName = cellItem.serverItemSpriteNameRoulette;
			}
			if (m_common != null)
			{
				m_common.SetActive(true);
				UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_common, "num");
				uILabel2.text = "×" + num;
			}
			break;
		case ServerItem.IdType.ITEM_ROULLETE_WIN:
		{
			if (!(m_rank != null))
			{
				break;
			}
			Transform transform = base.transform;
			Vector3 localScale = base.transform.parent.transform.localScale;
			float x = 1f / localScale.x;
			Vector3 localScale2 = base.transform.parent.transform.localScale;
			transform.localScale = new Vector3(x, 1f / localScale2.x, 1f);
			m_rank.SetActive(true);
			UISprite uISprite3 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_rank, "jack");
			UISprite uISprite4 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_rank, "big");
			UISprite uISprite5 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_rank, "super");
			if (!(uISprite3 != null) || !(uISprite4 != null) || !(uISprite5 != null))
			{
				break;
			}
			uISprite3.gameObject.SetActive(false);
			uISprite4.gameObject.SetActive(false);
			uISprite5.gameObject.SetActive(false);
			switch (data.GetRouletteRank())
			{
			case RouletteUtility.WheelRank.Normal:
				uISprite4.gameObject.SetActive(true);
				break;
			case RouletteUtility.WheelRank.Big:
				uISprite5.gameObject.SetActive(true);
				break;
			case RouletteUtility.WheelRank.Super:
			{
				uISprite3.gameObject.SetActive(true);
				UILabel uILabel3 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(uISprite3.gameObject, "ring");
				if (uILabel3 != null)
				{
					int num2 = RouletteManager.numJackpotRing;
					if (num2 <= 0)
					{
						num2 = 30000;
					}
					uILabel3.text = HudUtility.GetFormatNumString(num2);
				}
				break;
			}
			}
			break;
		}
		default:
			base.transform.localScale = new Vector3(1f, 1f, 1f);
			if (m_item != null)
			{
				m_item.SetActive(true);
				UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_item, "main");
				uISprite.spriteName = "ui_cmn_icon_item_" + cellItem.id;
			}
			if (m_common != null)
			{
				m_common.SetActive(true);
				UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_common, "num");
				uILabel.text = "×" + num;
			}
			break;
		}
		SetCampaign(cellItem.id);
	}

	private void SetGeneral(ServerWheelOptionsData data)
	{
		if (m_egg != null)
		{
			m_egg.SetActive(false);
		}
		if (m_item != null)
		{
			m_item.SetActive(false);
		}
		if (m_rank != null)
		{
			m_rank.SetActive(false);
		}
		if (m_common != null)
		{
			m_common.SetActive(false);
		}
		int num;
		ServerItem cellItem = data.GetCellItem(m_cellIndex, out num);
		if (cellItem.serverItemNum > 0)
		{
			num *= cellItem.serverItemNum;
		}
		switch (cellItem.idType)
		{
		case ServerItem.IdType.EQUIP_ITEM:
		case ServerItem.IdType.RSRING:
		case ServerItem.IdType.RING:
		case ServerItem.IdType.ENERGY:
			base.transform.localScale = new Vector3(1f, 1f, 1f);
			if (m_item != null)
			{
				m_item.SetActive(true);
				UISprite uISprite6 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_item, "main");
				uISprite6.spriteName = cellItem.serverItemSpriteNameRoulette;
			}
			if (m_common != null)
			{
				m_common.SetActive(true);
				UILabel uILabel3 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_common, "num");
				uILabel3.text = "×" + num;
			}
			break;
		case ServerItem.IdType.ITEM_ROULLETE_WIN:
		{
			if (!(m_rank != null))
			{
				break;
			}
			Transform transform = base.transform;
			Vector3 localScale = base.transform.parent.transform.localScale;
			float x = 1f / localScale.x;
			Vector3 localScale2 = base.transform.parent.transform.localScale;
			transform.localScale = new Vector3(x, 1f / localScale2.x, 1f);
			m_rank.SetActive(true);
			UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_rank, "jack");
			UISprite uISprite3 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_rank, "big");
			UISprite uISprite4 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_rank, "super");
			if (!(uISprite2 != null) || !(uISprite3 != null) || !(uISprite4 != null))
			{
				break;
			}
			uISprite2.gameObject.SetActive(false);
			uISprite3.gameObject.SetActive(false);
			uISprite4.gameObject.SetActive(false);
			switch (data.GetRouletteRank())
			{
			case RouletteUtility.WheelRank.Normal:
				uISprite3.gameObject.SetActive(true);
				break;
			case RouletteUtility.WheelRank.Big:
				uISprite4.gameObject.SetActive(true);
				break;
			case RouletteUtility.WheelRank.Super:
			{
				uISprite2.gameObject.SetActive(true);
				UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(uISprite2.gameObject, "ring");
				if (uILabel2 != null)
				{
					int num2 = RouletteManager.numJackpotRing;
					if (num2 <= 0)
					{
						num2 = 30000;
					}
					uILabel2.text = HudUtility.GetFormatNumString(num2);
				}
				break;
			}
			}
			break;
		}
		case ServerItem.IdType.CHARA:
		case ServerItem.IdType.CHAO:
		{
			if (!(m_egg != null))
			{
				break;
			}
			m_egg.SetActive(true);
			UISprite uISprite5 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_egg, "main");
			if (uISprite5 != null)
			{
				if (cellItem.idType == ServerItem.IdType.CHARA)
				{
					uISprite5.spriteName = "ui_roulette_chao_egg_100";
					break;
				}
				int id = (int)cellItem.id;
				int num3 = id / 1000 % 10;
				uISprite5.spriteName = "ui_roulette_chao_egg_" + num3;
			}
			break;
		}
		default:
			base.transform.localScale = new Vector3(1f, 1f, 1f);
			if (m_item != null)
			{
				m_item.SetActive(true);
				UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_item, "main");
				uISprite.spriteName = "ui_cmn_icon_item_" + cellItem.id;
			}
			if (m_common != null)
			{
				m_common.SetActive(true);
				UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_common, "num");
				uILabel.text = "×" + num;
			}
			break;
		}
		SetCampaign(cellItem.id);
	}
}
