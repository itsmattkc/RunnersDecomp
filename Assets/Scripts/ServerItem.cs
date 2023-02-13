using System;
using System.Collections.Generic;
using Text;

public struct ServerItem
{
	public enum Id
	{
		NONE = -1,
		BOOST_SCORE = 110000,
		BOOST_TRAMPOLINE = 110001,
		BOOST_SUBCHARA = 110002,
		INVINCIBLE = 120000,
		BARRIER = 120001,
		MAGNET = 120002,
		TRAMPOLINE = 120003,
		COMBO = 120004,
		LASER = 120005,
		DRILL = 120006,
		ASTEROID = 120007,
		RING_BONUS = 120008,
		DISTANCE_BONUS = 120009,
		ANIMAL_BONUS = 120010,
		PACKED_INVINCIBLE_0 = 120100,
		PACKED_BARRIER_0 = 120101,
		PACKED_MAGNET_0 = 120102,
		PACKED_TRAMPOLINE_0 = 120103,
		PACKED_COMBO_0 = 120104,
		PACKED_LASER_0 = 120105,
		PACKED_DRILL_0 = 120106,
		PACKED_ASTEROID_0 = 120107,
		PACKED_RING_BONUS_0 = 120108,
		PACKED_SCORE_BONUS_0 = 120109,
		PACKED_ANIMAL_BONUS_0 = 120110,
		PACKED_INVINCIBLE_1 = 121000,
		PACKED_BARRIER_1 = 121001,
		PACKED_MAGNET_1 = 121002,
		PACKED_TRAMPOLINE_1 = 121003,
		PACKED_COMBO_1 = 121004,
		PACKED_LASER_1 = 121005,
		PACKED_DRILL_1 = 121006,
		PACKED_ASTEROID_1 = 121007,
		PACKED_RING_BONUS_1 = 121008,
		PACKED_SCORE_BONUS_1 = 121009,
		PACKED_ANIMAL_BONUS_1 = 121010,
		BIG = 200000,
		SUPER = 200001,
		JACKPOT = 200002,
		ROULLETE_TOKEN = 210000,
		SPECIAL_EGG = 220000,
		ROULLETE_TICKET_BEGIN = 229999,
		ROULLETE_TICKET_PREMIAM = 230000,
		ROULLETE_TICKET_ITEM = 240000,
		ROULLETE_TICKET_RAID = 250000,
		ROULLETE_TICKET_EVENT = 260000,
		ROULLETE_TICKET_END = 299999,
		CHARA_BEGIN = 300000,
		CHAO_BEGIN = 400000,
		CHAO_BEGIN_RARE = 401000,
		CHAO_BEGIN_SRARE = 402000,
		RSRING = 900000,
		RSRING_0 = 900010,
		RSRING_1 = 900030,
		RSRING_2 = 900060,
		RSRING_3 = 900210,
		RSRING_4 = 900380,
		RING = 910000,
		RING_0 = 910021,
		RING_1 = 910045,
		RING_2 = 910094,
		RING_3 = 910147,
		RING_4 = 910204,
		RING_5 = 910265,
		ENERGY = 920000,
		ENERGY_0 = 920001,
		ENERGY_1 = 920005,
		ENERGY_2 = 920010,
		ENERGY_3 = 920015,
		ENERGY_4 = 920020,
		ENERGY_5 = 920025,
		ENERGY_MAX = 930000,
		SUB_CHARA = 940000,
		CONTINUE = 950000,
		RAIDRING = 960000,
		DAILY_BATTLE_RESET_0 = 980000,
		DAILY_BATTLE_RESET_1 = 980001,
		DAILY_BATTLE_RESET_2 = 980002
	}

	public enum IdType
	{
		NONE = -1,
		BOOST_ITEM = 11,
		EQUIP_ITEM = 12,
		ITEM_ROULLETE_WIN = 20,
		ROULLETE_TOKEN = 21,
		EGG_ITEM = 22,
		PREMIUM_ROULLETE_TICKET = 23,
		ITEM_ROULLETE_TICKET = 24,
		CHARA = 30,
		CHAO = 40,
		RSRING = 90,
		RING = 91,
		ENERGY = 92,
		ENERGY_MAX = 93,
		RAIDRING = 96
	}

	private const int SERVER_ID_INDEX_DIVISOR = 10000;

	private const int SERVER_ID_EQUIP_ITEM_INDEX_DIVISOR = 100;

	private static Dictionary<IdType, string> IdTypeAtlasName = new Dictionary<IdType, string>
	{
		{
			IdType.NONE,
			string.Empty
		},
		{
			IdType.BOOST_ITEM,
			"ui_item_set_3_Atlas"
		},
		{
			IdType.EQUIP_ITEM,
			"ui_player_set_icon_Atlas"
		},
		{
			IdType.ITEM_ROULLETE_WIN,
			string.Empty
		},
		{
			IdType.ROULLETE_TOKEN,
			"ui_cmn_item_Atlas"
		},
		{
			IdType.EGG_ITEM,
			"ui_mainmenu_Atlas"
		},
		{
			IdType.CHARA,
			"ui_cmn_player_bundle_Atlas"
		},
		{
			IdType.CHAO,
			"ui_cmn_chao_Atlas"
		},
		{
			IdType.RSRING,
			"ui_cmn_item_Atlas"
		},
		{
			IdType.RING,
			"ui_cmn_item_Atlas"
		},
		{
			IdType.ENERGY,
			"ui_cmn_item_Atlas"
		},
		{
			IdType.ENERGY_MAX,
			"ui_cmn_item_Atlas"
		}
	};

	private static Dictionary<AbilityType, Id> AbilityToServerId = new Dictionary<AbilityType, Id>
	{
		{
			AbilityType.LASER,
			Id.LASER
		},
		{
			AbilityType.DRILL,
			Id.DRILL
		},
		{
			AbilityType.ASTEROID,
			Id.ASTEROID
		},
		{
			AbilityType.RING_BONUS,
			Id.RING_BONUS
		},
		{
			AbilityType.DISTANCE_BONUS,
			Id.DISTANCE_BONUS
		},
		{
			AbilityType.TRAMPOLINE,
			Id.TRAMPOLINE
		},
		{
			AbilityType.ANIMAL,
			Id.ANIMAL_BONUS
		},
		{
			AbilityType.COMBO,
			Id.COMBO
		},
		{
			AbilityType.MAGNET,
			Id.MAGNET
		},
		{
			AbilityType.INVINCIBLE,
			Id.INVINCIBLE
		}
	};

	private static Dictionary<Id, AbilityType> ServerIdToAbility = new Dictionary<Id, AbilityType>
	{
		{
			Id.LASER,
			AbilityType.LASER
		},
		{
			Id.DRILL,
			AbilityType.DRILL
		},
		{
			Id.ASTEROID,
			AbilityType.ASTEROID
		},
		{
			Id.RING_BONUS,
			AbilityType.RING_BONUS
		},
		{
			Id.DISTANCE_BONUS,
			AbilityType.DISTANCE_BONUS
		},
		{
			Id.TRAMPOLINE,
			AbilityType.TRAMPOLINE
		},
		{
			Id.ANIMAL_BONUS,
			AbilityType.ANIMAL
		},
		{
			Id.COMBO,
			AbilityType.COMBO
		},
		{
			Id.MAGNET,
			AbilityType.MAGNET
		},
		{
			Id.INVINCIBLE,
			AbilityType.INVINCIBLE
		}
	};

	private static int[] s_chaoIdTable;

	private static Dictionary<IdType, ServerItem[]> s_dicServerItemTable = new Dictionary<IdType, ServerItem[]>();

	private Id m_id;

	private static Dictionary<Id, RewardType> s_dic_ServerItemId_to_RewardType = new Dictionary<Id, RewardType>
	{
		{
			Id.INVINCIBLE,
			RewardType.ITEM_INVINCIBLE
		},
		{
			Id.BARRIER,
			RewardType.ITEM_BARRIER
		},
		{
			Id.MAGNET,
			RewardType.ITEM_MAGNET
		},
		{
			Id.TRAMPOLINE,
			RewardType.ITEM_TRAMPOLINE
		},
		{
			Id.COMBO,
			RewardType.ITEM_COMBO
		},
		{
			Id.LASER,
			RewardType.ITEM_LASER
		},
		{
			Id.DRILL,
			RewardType.ITEM_DRILL
		},
		{
			Id.ASTEROID,
			RewardType.ITEM_ASTEROID
		},
		{
			Id.RING,
			RewardType.RING
		},
		{
			Id.RSRING,
			RewardType.RSRING
		},
		{
			Id.ENERGY,
			RewardType.ENERGY
		}
	};

	public Id id
	{
		get
		{
			return m_id;
		}
	}

	public IdType idType
	{
		get
		{
			return (IdType)((int)m_id / 10000);
		}
	}

	public int idIndex
	{
		get
		{
			return (int)m_id % ((idType != IdType.EQUIP_ITEM) ? 10000 : 100);
		}
	}

	public bool isPacked
	{
		get
		{
			return packedNumber != 0;
		}
	}

	public bool isValid
	{
		get
		{
			return m_id != Id.NONE;
		}
	}

	private int packedNumber
	{
		get
		{
			return (idType == IdType.EQUIP_ITEM) ? ((int)m_id % 10000 / 100) : 0;
		}
	}

	public int serverItemNum
	{
		get
		{
			return packedNumber;
		}
	}

	public string serverItemName
	{
		get
		{
			string result = null;
			int num = (int)m_id % 1000;
			switch (idType)
			{
			case IdType.EQUIP_ITEM:
			{
				string cellID = string.Format("name{0}", num % 100 + 1);
				result = TextUtility.GetCommonText("ShopItem", cellID);
				break;
			}
			case IdType.EGG_ITEM:
			{
				string cellID = "sp_egg_name";
				result = TextUtility.GetCommonText("ChaoRoulette", cellID);
				break;
			}
			case IdType.CHARA:
			{
				CharacterDataNameInfo.Info dataByServerID = CharacterDataNameInfo.Instance.GetDataByServerID((int)m_id);
				if (dataByServerID != null)
				{
					string cellID = dataByServerID.m_name.ToLower();
					result = TextUtility.GetCommonText("CharaName", cellID);
				}
				break;
			}
			case IdType.CHAO:
			{
				string cellID2 = "name" + chaoId.ToString("D4");
				result = TextUtility.GetChaoText("Chao", cellID2);
				break;
			}
			case IdType.RSRING:
			{
				string cellID = "red_star_ring";
				result = TextUtility.GetCommonText("Item", cellID);
				break;
			}
			case IdType.RING:
			{
				string cellID = "ring";
				result = TextUtility.GetCommonText("Item", cellID);
				break;
			}
			case IdType.ENERGY:
			{
				string cellID = "energy";
				result = TextUtility.GetCommonText("Item", cellID);
				break;
			}
			case IdType.ENERGY_MAX:
			{
				string cellID = "energy";
				result = TextUtility.GetCommonText("Item", cellID);
				break;
			}
			case IdType.RAIDRING:
			{
				string cellID = "raidboss_ring";
				result = TextUtility.GetCommonText("Item", cellID);
				break;
			}
			case IdType.ITEM_ROULLETE_TICKET:
			{
				string cellID = "item_roulette_ticket";
				result = TextUtility.GetCommonText("Item", cellID);
				break;
			}
			case IdType.PREMIUM_ROULLETE_TICKET:
			{
				string cellID = "premium_roulette_ticket";
				result = TextUtility.GetCommonText("Item", cellID);
				break;
			}
			}
			return result;
		}
	}

	public string serverItemComment
	{
		get
		{
			string result = null;
			int num = (int)m_id % 1000;
			switch (idType)
			{
			case IdType.BOOST_ITEM:
				result = "[BOOST_ITEM]";
				if (num % 3 == 0)
				{
					result = "スコアボーナス100%";
				}
				else if (num % 3 == 1)
				{
					result = "アシストトランポリン";
				}
				else if (num % 3 == 2)
				{
					result = "サブキャラクター";
				}
				break;
			case IdType.EQUIP_ITEM:
			{
				string cellID = string.Format("details{0}", num % 100 + 1);
				result = TextUtility.GetCommonText("ShopItem", cellID);
				break;
			}
			case IdType.ROULLETE_TOKEN:
				switch (num)
				{
				case 0:
					result = "BIG";
					break;
				case 1:
					result = "SUPER";
					break;
				default:
					result = "ジャックポット";
					break;
				}
				break;
			case IdType.EGG_ITEM:
			{
				string cellID = "sp_egg_details";
				result = TextUtility.GetCommonText("ChaoRoulette", cellID);
				break;
			}
			case IdType.CHARA:
			{
				CharacterDataNameInfo.Info dataByServerID = CharacterDataNameInfo.Instance.GetDataByServerID((int)m_id);
				if (dataByServerID != null)
				{
					string cellID = string.Format("chara_attribute_{0}", dataByServerID.m_name.ToLower());
					result = TextUtility.GetCommonText("WindowText", cellID);
				}
				break;
			}
			case IdType.RSRING:
			{
				string cellID = "red_star_ring_details";
				result = TextUtility.GetCommonText("Item", cellID);
				break;
			}
			case IdType.RING:
			{
				string cellID = "ring_details";
				result = TextUtility.GetCommonText("Item", cellID);
				break;
			}
			case IdType.ENERGY:
			{
				string cellID = "energy_details";
				result = TextUtility.GetCommonText("Item", cellID);
				break;
			}
			case IdType.ENERGY_MAX:
			{
				string cellID = "energy_details";
				result = TextUtility.GetCommonText("Item", cellID);
				break;
			}
			}
			return result;
		}
	}

	public string serverItemSpriteName
	{
		get
		{
			string result = null;
			int num = (int)m_id % 1000;
			switch (idType)
			{
			case IdType.BOOST_ITEM:
				result = string.Format("ui_itemset_2_boost_icon_{0}", num);
				break;
			case IdType.EQUIP_ITEM:
				result = string.Format("ui_mm_player_icon_{0}", num % 100);
				break;
			case IdType.ROULLETE_TOKEN:
				result = string.Format("ui_cmn_icon_item_{0}", num % 100);
				break;
			case IdType.EGG_ITEM:
				result = "ui_cmn_icon_item_220000";
				break;
			case IdType.CHARA:
				if (num % 100 < CharacterDataNameInfo.PrefixNameList.Length)
				{
					string arg = CharacterDataNameInfo.PrefixNameList[num % 100];
					result = string.Format("ui_tex_player_{0:00}_{1}", num % 100, arg);
				}
				break;
			case IdType.RSRING:
				result = "ui_shop_img_rsring";
				result = string.Format("ui_cmn_icon_item_{0}", 9);
				break;
			case IdType.RING:
				result = string.Format("ui_cmn_icon_item_{0}", 8);
				break;
			case IdType.ENERGY:
				result = string.Format("ui_cmn_icon_item_{0}", 92000);
				break;
			case IdType.ENERGY_MAX:
				result = string.Format("ui_cmn_icon_item_{0}", 92000);
				break;
			}
			return result;
		}
	}

	public string serverItemSpriteNameRoulette
	{
		get
		{
			string result = null;
			int num = (int)m_id % 1000;
			switch (idType)
			{
			case IdType.EQUIP_ITEM:
				result = string.Format("ui_cmn_icon_item_{0}", num % 100);
				break;
			case IdType.EGG_ITEM:
				result = "ui_cmn_icon_item_220000";
				break;
			case IdType.CHARA:
				if (num % 100 < CharacterDataNameInfo.PrefixNameList.Length)
				{
					string arg = CharacterDataNameInfo.PrefixNameList[num % 100];
					result = string.Format("ui_tex_player_{0:00}_{1}", num % 100, arg);
				}
				break;
			case IdType.RSRING:
				result = "ui_shop_img_rsring";
				result = string.Format("ui_cmn_icon_item_{0}", 9);
				break;
			case IdType.RING:
				result = string.Format("ui_cmn_icon_item_{0}", 8);
				break;
			case IdType.ENERGY:
				result = string.Format("ui_cmn_icon_item_{0}", 92000);
				break;
			case IdType.ENERGY_MAX:
				result = string.Format("ui_cmn_icon_item_{0}", 92000);
				break;
			}
			return result;
		}
	}

	public CharaType charaType
	{
		get
		{
			if (idType == IdType.CHARA && CharacterDataNameInfo.Instance != null)
			{
				CharacterDataNameInfo.Info dataByServerID = CharacterDataNameInfo.Instance.GetDataByServerID((int)m_id);
				if (dataByServerID != null)
				{
					return dataByServerID.m_ID;
				}
			}
			return CharaType.UNKNOWN;
		}
	}

	public ItemType itemType
	{
		get
		{
			return (ItemType)((idType != IdType.EQUIP_ITEM || (uint)idIndex >= 8u) ? (-1) : idIndex);
		}
	}

	public BoostItemType boostItemType
	{
		get
		{
			return (BoostItemType)((idType != IdType.BOOST_ITEM || (uint)idIndex >= 3u) ? (-1) : idIndex);
		}
	}

	public AbilityType abilityType
	{
		get
		{
			if (ServerIdToAbility.ContainsKey(m_id))
			{
				return ServerIdToAbility[m_id];
			}
			return AbilityType.NONE;
		}
	}

	public int chaoId
	{
		get
		{
			return (idType != IdType.CHAO) ? (-1) : idIndex;
		}
	}

	public RewardType rewardType
	{
		get
		{
			RewardType value;
			if (!s_dic_ServerItemId_to_RewardType.TryGetValue(id, out value))
			{
				return (RewardType)id;
			}
			return value;
		}
	}

	public ServerItem(Id id)
	{
		m_id = id;
	}

	public ServerItem(CharaType characterType)
	{
		m_id = Id.NONE;
		if (characterType != CharaType.UNKNOWN && CharacterDataNameInfo.Instance != null)
		{
			CharacterDataNameInfo.Info dataByID = CharacterDataNameInfo.Instance.GetDataByID(characterType);
			if (dataByID != null)
			{
				m_id = (Id)dataByID.m_serverID;
			}
		}
	}

	public ServerItem(ItemType itemType)
	{
		m_id = (Id)((itemType == ItemType.UNKNOWN) ? ItemType.UNKNOWN : (itemType + 120000));
	}

	public ServerItem(BoostItemType boostItemType)
	{
		m_id = (Id)(((uint)boostItemType >= 3u) ? BoostItemType.UNKNOWN : (boostItemType + 110000));
	}

	public ServerItem(AbilityType abilityType)
	{
		if (AbilityToServerId.ContainsKey(abilityType))
		{
			m_id = AbilityToServerId[abilityType];
		}
		else
		{
			m_id = Id.NONE;
		}
	}

	public ServerItem(RewardType rewardType)
	{
		m_id = Id.NONE;
		foreach (Id key in s_dic_ServerItemId_to_RewardType.Keys)
		{
			if (s_dic_ServerItemId_to_RewardType[key] == rewardType)
			{
				m_id = key;
				break;
			}
		}
		if (m_id == Id.NONE)
		{
			m_id = (Id)rewardType;
		}
	}

	public static string GetIdTypeAtlasName(IdType idType)
	{
		string result = null;
		if (idType != IdType.NONE)
		{
			result = IdTypeAtlasName[idType];
		}
		return result;
	}

	public static Id ConvertAbilityId(AbilityType abilityType)
	{
		Id result = Id.NONE;
		if (AbilityToServerId.ContainsKey(abilityType))
		{
			result = AbilityToServerId[abilityType];
		}
		return result;
	}

	public static ServerItem CreateFromChaoId(int chaoId)
	{
		return new ServerItem((Id)((chaoId == -1) ? (-1) : (chaoId + 400000)));
	}

	public static ServerItem[] GetServerItemTable(IdType idType)
	{
		if (!s_dicServerItemTable.ContainsKey(idType))
		{
			List<ServerItem> list = new List<ServerItem>();
			foreach (object value in Enum.GetValues(typeof(Id)))
			{
				Id id = (Id)(int)value;
				if (new ServerItem(id).idType == idType)
				{
					list.Add(new ServerItem(id));
				}
			}
			s_dicServerItemTable[idType] = list.ToArray();
		}
		return s_dicServerItemTable[idType];
	}

	public static int GetServerItemCount(IdType idType)
	{
		return GetServerItemTable(idType).Length;
	}
}
