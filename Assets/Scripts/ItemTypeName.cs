using SaveData;

public class ItemTypeName
{
	private static readonly ItemParam[] ITEM_PARAMS = new ItemParam[8]
	{
		new ItemParam("INVINCIBLE", "obj_item_invincibility", SystemData.ItemTutorialFlagStatus.INVINCIBLE, HudTutorial.Id.ITEM_1),
		new ItemParam("BARRIER", "obj_item_barrier", SystemData.ItemTutorialFlagStatus.BARRIER, HudTutorial.Id.ITEM_2),
		new ItemParam("MAGNET", "obj_item_magnetbarrier", SystemData.ItemTutorialFlagStatus.MAGNET, HudTutorial.Id.ITEM_3),
		new ItemParam("TRAMPOLINE", "obj_item_trampolinefloor", SystemData.ItemTutorialFlagStatus.TRAMPOLINE, HudTutorial.Id.ITEM_4),
		new ItemParam("COMBO", "obj_item_combobounus", SystemData.ItemTutorialFlagStatus.COMBO, HudTutorial.Id.ITEM_5),
		new ItemParam("LASER", "obj_itemWisp_laser", SystemData.ItemTutorialFlagStatus.LASER, HudTutorial.Id.ITEM_6),
		new ItemParam("DRILL", "obj_itemWisp_dril", SystemData.ItemTutorialFlagStatus.DRILL, HudTutorial.Id.ITEM_7),
		new ItemParam("ASTEROID", "obj_itemWisp_asteroid", SystemData.ItemTutorialFlagStatus.ASTEROID, HudTutorial.Id.ITEM_8)
	};

	private static readonly string[] OTHERITEM_TYPES = new string[4]
	{
		"REDSTAR_RING",
		"TIMER_BRONZE",
		"TIMER_SILVER",
		"TIMER_GOLD"
	};

	public static string GetItemTypeName(ItemType type)
	{
		if ((uint)type < 8u)
		{
			return ITEM_PARAMS[(int)type].m_name;
		}
		return string.Empty;
	}

	public static string GetItemFileName(ItemType type)
	{
		if ((uint)type < 8u)
		{
			return ITEM_PARAMS[(int)type].m_objName;
		}
		return string.Empty;
	}

	public static SystemData.ItemTutorialFlagStatus GetItemTutorialStatus(ItemType type)
	{
		if ((uint)type < 8u)
		{
			return ITEM_PARAMS[(int)type].m_flagStatus;
		}
		return SystemData.ItemTutorialFlagStatus.NONE;
	}

	public static HudTutorial.Id GetItemTutorialID(ItemType type)
	{
		if ((uint)type < 8u)
		{
			return ITEM_PARAMS[(int)type].m_tutorialID;
		}
		return HudTutorial.Id.NONE;
	}

	public static string GetOtherItemTypeName(OtherItemType type)
	{
		if ((uint)type < 4u)
		{
			return OTHERITEM_TYPES[(int)type];
		}
		return string.Empty;
	}
}
