using System.Collections.Generic;
using Text;

public class PresentBoxUtility
{
	public static string GetItemSpriteName(ServerItem serverItem)
	{
		string result = string.Empty;
		switch (serverItem.idType)
		{
		case ServerItem.IdType.EQUIP_ITEM:
			result = "ui_cmn_icon_item_" + serverItem.idIndex;
			break;
		case ServerItem.IdType.ROULLETE_TOKEN:
			result = "ui_cmn_icon_item_210000";
			break;
		case ServerItem.IdType.CHARA:
			result = "ui_tex_player_" + serverItem.idIndex.ToString("D2") + "_" + CharaName.PrefixName[(int)serverItem.charaType];
			break;
		case ServerItem.IdType.CHAO:
			result = "ui_tex_chao_" + serverItem.idIndex.ToString("D4");
			break;
		case ServerItem.IdType.RSRING:
			result = "ui_cmn_icon_item_9";
			break;
		case ServerItem.IdType.RING:
			result = "ui_cmn_icon_item_8";
			break;
		case ServerItem.IdType.ENERGY:
			result = "ui_cmn_icon_item_920000";
			break;
		case ServerItem.IdType.RAIDRING:
			result = "ui_cmn_icon_item_960000";
			break;
		case ServerItem.IdType.ITEM_ROULLETE_TICKET:
			result = "ui_cmn_icon_item_240000";
			break;
		case ServerItem.IdType.PREMIUM_ROULLETE_TICKET:
			result = "ui_cmn_icon_item_230000";
			break;
		}
		return result;
	}

	public static string GetItemName(ServerItem serverItem)
	{
		return serverItem.serverItemName;
	}

	public static string GetItemInfo(PresentBoxUI.PresentInfo info)
	{
		string result = string.Empty;
		if (info != null)
		{
			switch (info.messageType)
			{
			case ServerMessageEntry.MessageType.SendEnergy:
				result = TextUtility.GetCommonText("PresentBox", "present_from_friend", "{FRIEND_NAME}", info.name);
				break;
			case ServerMessageEntry.MessageType.ReturnSendEnergy:
				result = TextUtility.GetCommonText("PresentBox", "remuneration_friend_present");
				break;
			case ServerMessageEntry.MessageType.InviteCode:
				result = TextUtility.GetCommonText("PresentBox", "remuneration_friend_invite");
				break;
			}
		}
		return result;
	}

	public static string GetReceivedTime(int expireTime)
	{
		string empty = string.Empty;
		if (expireTime == 0)
		{
			return TextUtility.GetCommonText("PresentBox", "unlimited_duration");
		}
		int num = expireTime - NetUtil.GetCurrentUnixTime();
		if (num >= 86400)
		{
			return TextUtility.GetCommonText("PresentBox", "expire_days", "{DAYS}", (num / 86400).ToString());
		}
		if (num >= 3600)
		{
			return TextUtility.GetCommonText("PresentBox", "expire_hours", "{HOURS}", (num / 3600).ToString());
		}
		return TextUtility.GetCommonText("PresentBox", "expire_minutes", "{MINUTES}", (num / 60).ToString());
	}

	public static bool IsWithinTimeLimit(int expireTime)
	{
		int num = expireTime - NetUtil.GetCurrentUnixTime();
		return num > 0;
	}

	public static string GetPresetTextList(List<ServerPresentState> presentStateList)
	{
		string text = string.Empty;
		foreach (ServerPresentState presentState in presentStateList)
		{
			ServerItem serverItem = new ServerItem((ServerItem.Id)presentState.m_itemId);
			string itemName = GetItemName(serverItem);
			string text2 = text;
			text = text2 + itemName + "×" + presentState.m_numItem + "\n";
		}
		return text;
	}
}
