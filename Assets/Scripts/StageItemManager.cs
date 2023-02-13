using App;
using Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/GameMode/Stage")]
public class StageItemManager : MonoBehaviour
{
	private class EquippedItem
	{
		public ItemType item;

		public int index;

		public bool revivedFlag;
	}

	private class ChaoAbilityOption
	{
		public float m_specifiedTime;
	}

	private enum PhantomUseType
	{
		ATTACK_PAUSE,
		DISABLE,
		ENABLE
	}

	private const float DEBUG_TIME = -1f;

	private const float VALIDATE_TIME = 3f;

	private const float PHANTOM_VALIDATE_TIME = 6f;

	private const float ALTITUDE_TRAMPOLINE_OFFSET = 6f;

	private const float INVALID_TIME_ON_PAUSED = 2f;

	private const int ITEM_COMBO_SCORE_RATE = 2;

	private const int ITEM_COMBO_SCORE_MAX_RATE = 10;

	private const int EQUIP_ITEM_MAX_COUNT = 3;

	[SerializeField]
	public bool m_debugEquipItem;

	[SerializeField]
	public ItemType[] m_debugEquipItemTypes = new ItemType[3];

	private float[] m_items_fullTime = new float[8];

	private float[] m_items_time = new float[8];

	private bool[] m_items_paused;

	private float m_chaoAblityComboUpRate = 1f;

	private float m_niths_combo_time;

	private float m_item_combo_time;

	private ItemTable m_item_table;

	private bool m_disable_equipItem;

	private bool m_nowPhantom;

	private bool m_characterChange;

	private bool m_bossStage;

	private bool m_forcePhantomInvalidate;

	private bool m_equipItemTutorial;

	public static Dictionary<ItemType, AbilityType> s_dicItemTypeToCharAbilityType = new Dictionary<ItemType, AbilityType>
	{
		{
			ItemType.INVINCIBLE,
			AbilityType.INVINCIBLE
		},
		{
			ItemType.BARRIER,
			AbilityType.NUM
		},
		{
			ItemType.MAGNET,
			AbilityType.MAGNET
		},
		{
			ItemType.TRAMPOLINE,
			AbilityType.TRAMPOLINE
		},
		{
			ItemType.COMBO,
			AbilityType.COMBO
		},
		{
			ItemType.LASER,
			AbilityType.LASER
		},
		{
			ItemType.DRILL,
			AbilityType.DRILL
		},
		{
			ItemType.ASTEROID,
			AbilityType.ASTEROID
		}
	};

	private static StageItemManager instance = null;

	private bool m_availableItem;

	private List<ItemType> m_displayEquipItems = new List<ItemType>();

	private List<EquippedItem> m_equipItems = new List<EquippedItem>();

	private ItemType m_busyItem = ItemType.UNKNOWN;

	private int[] m_itemChangeTable = new int[8]
	{
		0,
		1,
		2,
		3,
		4,
		5,
		6,
		7
	};

	private static ItemType[] m_phantomItemTypes = new ItemType[3]
	{
		ItemType.LASER,
		ItemType.DRILL,
		ItemType.ASTEROID
	};

	private static ItemType[] m_feverBossNoPauseItemTypes = new ItemType[3]
	{
		ItemType.BARRIER,
		ItemType.MAGNET,
		ItemType.COMBO
	};

	private PhantomUseType m_phantomUseType;

	private LevelInformation m_levelInformation;

	private PlayerInformation m_playerInformation;

	private List<ItemType> m_stockColorItems = new List<ItemType>();

	private List<ItemType> m_stockItems = new List<ItemType>();

	private bool m_activeTrampoline;

	private bool m_activeAltitudeTrampoline;

	private bool m_Altitude;

	private bool m_bossItemTutorial;

	private static ItemType[] m_cautionItemTypePriority = new ItemType[7]
	{
		ItemType.LASER,
		ItemType.DRILL,
		ItemType.ASTEROID,
		ItemType.INVINCIBLE,
		ItemType.TRAMPOLINE,
		ItemType.MAGNET,
		ItemType.COMBO
	};

	public static StageItemManager Instance
	{
		get
		{
			return instance;
		}
	}

	public float CautionItemTimeRate
	{
		get
		{
			ItemType[] cautionItemTypePriority = m_cautionItemTypePriority;
			foreach (ItemType itemType in cautionItemTypePriority)
			{
				float num = m_items_time[(int)itemType];
				float num2 = m_items_fullTime[(int)itemType];
				if (num > 0f && num2 > 0f)
				{
					return num / num2;
				}
			}
			return 0f;
		}
	}

	public float[] ItemsTime
	{
		get
		{
			return m_items_time;
		}
	}

	private void Awake()
	{
		SetInstance();
	}

	private void Start()
	{
		m_items_fullTime = new float[8];
		m_items_time = new float[8];
		m_items_paused = new bool[8];
		m_item_table = new ItemTable();
		m_availableItem = false;
		m_equipItems = new List<EquippedItem>();
		m_busyItem = ItemType.UNKNOWN;
		m_nowPhantom = false;
		m_forcePhantomInvalidate = false;
		m_equipItemTutorial = false;
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	private void SetInstance()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void UpdateStockItem(List<ItemType> itemList)
	{
		if (itemList.Count <= 0)
		{
			return;
		}
		if (itemList[0] != ItemType.UNKNOWN)
		{
			if (!m_nowPhantom && (m_phantomUseType != PhantomUseType.DISABLE || Array.IndexOf(m_phantomItemTypes, itemList[0]) < 0) && !m_playerInformation.IsDead() && IsAskEquipItemUsed(itemList[0]))
			{
				AddItem(itemList[0], false);
				itemList.RemoveAt(0);
			}
		}
		else
		{
			itemList.RemoveAt(0);
		}
	}

	private void Update()
	{
		if (m_levelInformation == null)
		{
			m_levelInformation = GameObjectUtil.FindGameObjectComponent<LevelInformation>("LevelInformation");
		}
		if (m_playerInformation == null)
		{
			m_playerInformation = GameObjectUtil.FindGameObjectComponent<PlayerInformation>("PlayerInformation");
		}
		if (m_phantomUseType == PhantomUseType.ATTACK_PAUSE && m_levelInformation != null && m_levelInformation.NowBoss)
		{
			m_phantomUseType = PhantomUseType.DISABLE;
		}
		CheckDisablePhantom();
		UpdateStockItem(m_stockColorItems);
		UpdateStockItem(m_stockItems);
		for (int i = 0; i < 8; i++)
		{
			if (!(m_items_time[i] > 0f) || m_items_paused[i])
			{
				continue;
			}
			if (i == 4)
			{
				if (m_niths_combo_time > Time.deltaTime)
				{
					m_niths_combo_time -= Time.deltaTime;
				}
				else
				{
					m_niths_combo_time = 0f;
				}
				if (m_item_combo_time > Time.deltaTime)
				{
					m_item_combo_time -= Time.deltaTime;
				}
				else
				{
					m_item_combo_time = 0f;
				}
			}
			if (m_items_time[i] > Time.deltaTime)
			{
				m_items_time[i] -= Time.deltaTime;
				continue;
			}
			m_items_time[i] = 0f;
			TimeOutItem((ItemType)i);
		}
		if (IsActiveAltitudeTrampoline())
		{
			UpdateAltitudeTrampoline();
		}
	}

	private void CheckDisablePhantom()
	{
		if (m_disable_equipItem && m_playerInformation != null && m_playerInformation.PhantomType != PhantomType.NONE && !m_forcePhantomInvalidate)
		{
			ItemType phantomItemType = GetPhantomItemType();
			if (phantomItemType != ItemType.UNKNOWN)
			{
				m_items_paused[(int)phantomItemType] = true;
				MsgInvalidateItem value = new MsgInvalidateItem(GetPhantomItemType());
				GameObjectUtil.SendMessageToTagObjects("Player", "OnInvalidateItem", value, SendMessageOptions.DontRequireReceiver);
				GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnPauseItemOnBoss", null, SendMessageOptions.DontRequireReceiver);
				m_forcePhantomInvalidate = true;
			}
		}
	}

	private GameObject GetPlayerObject()
	{
		return GameObject.FindWithTag("Player");
	}

	public int GetComboScoreRate()
	{
		int num = 1;
		if (m_item_combo_time > 0f)
		{
			num = 2;
		}
		if (m_niths_combo_time > 0f)
		{
			num *= (int)m_chaoAblityComboUpRate;
		}
		return Mathf.Min(num, 10);
	}

	public ItemTable GetItemTable()
	{
		return m_item_table;
	}

	public void OnRequestItemUse(MsgAskEquipItemUsed msg)
	{
		bool flag = false;
		int num = -1;
		for (int i = 0; i < m_equipItems.Count; i++)
		{
			if (m_equipItems[i].item != msg.m_itemType)
			{
				continue;
			}
			num = i;
			if (!m_equipItems[i].revivedFlag && StageAbilityManager.Instance != null && StageAbilityManager.Instance.HasChaoAbility(ChaoAbility.ITEM_REVIVE))
			{
				int num2 = (int)StageAbilityManager.Instance.GetChaoAbilityValue(ChaoAbility.ITEM_REVIVE);
				if (num2 >= ObjUtil.GetRandomRange100())
				{
					flag = true;
					m_equipItems[i].revivedFlag = true;
				}
			}
			break;
		}
		if (num < 0)
		{
			return;
		}
		if (m_equipItems[num].item == ItemType.UNKNOWN)
		{
			m_equipItems.RemoveRange(num, 1);
			return;
		}
		msg.m_ok = IsAskEquipItemUsed(m_equipItems[num].item);
		if (msg.m_ok)
		{
			AddItem(m_equipItems[num].item, true, flag);
			if (flag)
			{
				ObjUtil.RequestStartAbilityToChao(ChaoAbility.ITEM_REVIVE, false);
			}
			else
			{
				m_equipItems.RemoveRange(num, 1);
			}
		}
	}

	public void OnAddItem(MsgAddItemToManager msg)
	{
		if (IsAskEquipItemUsed(msg.m_itemType))
		{
			AddItem(msg.m_itemType, false);
		}
	}

	public void OnAddEquipItem()
	{
		if ((m_playerInformation != null && (m_playerInformation.IsDead() || m_playerInformation.IsNowLastChance())) || m_equipItems.Count >= 3)
		{
			return;
		}
		List<ItemType> list = new List<ItemType>();
		for (int i = 0; i < 8; i++)
		{
			bool flag = false;
			for (int j = 0; j < m_equipItems.Count; j++)
			{
				if (i == (int)m_equipItems[j].item)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add((ItemType)i);
			}
		}
		if (list.Count > 1)
		{
			ItemType item = list[UnityEngine.Random.Range(0, list.Count)];
			EquippedItem equippedItem = new EquippedItem();
			equippedItem.item = item;
			equippedItem.index = m_equipItems.Count;
			m_equipItems.Add(equippedItem);
			ItemType[] array = new ItemType[m_equipItems.Count];
			for (int k = 0; k < m_equipItems.Count; k++)
			{
				array[k] = m_equipItems[k].item;
			}
			MsgSetEquippedItem msgSetEquippedItem = new MsgSetEquippedItem(array);
			bool flag2 = false;
			if (Array.IndexOf(m_phantomItemTypes, m_busyItem) >= 0)
			{
				flag2 = true;
			}
			msgSetEquippedItem.m_enabled = (!m_disable_equipItem && !flag2);
			GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnSetPresentEquippedItem", msgSetEquippedItem, SendMessageOptions.DontRequireReceiver);
		}
	}

	public void OnChangeItem()
	{
		int count = m_equipItems.Count;
		if (count > 0)
		{
			m_displayEquipItems.Clear();
			App.Random.ShuffleInt(m_itemChangeTable);
			for (int i = 0; i < count; i++)
			{
				m_equipItems[i].item = (ItemType)m_itemChangeTable[i];
				m_displayEquipItems.Add(m_equipItems[i].item);
			}
			GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnChangeItem", new MsgSetEquippedItem(m_displayEquipItems.ToArray()), SendMessageOptions.DontRequireReceiver);
		}
	}

	public void OnAddColorItem(MsgAddItemToManager msg)
	{
		if (IsAskEquipItemUsed(msg.m_itemType))
		{
			AddItem(msg.m_itemType, false);
			return;
		}
		ItemType phantomItemType = GetPhantomItemType();
		if (phantomItemType == msg.m_itemType)
		{
			float itemTimeFromChara = GetItemTimeFromChara(phantomItemType);
			m_items_time[(int)phantomItemType] = Mathf.Max(m_items_time[(int)phantomItemType], itemTimeFromChara);
			m_items_fullTime[(int)phantomItemType] = m_items_time[(int)phantomItemType];
			CountdownMeter();
		}
		else if (IsAskItemStock(msg.m_itemType))
		{
			m_stockColorItems.Add(msg.m_itemType);
		}
	}

	public void OnAddDamageTrampoline()
	{
		if (IsAskEquipItemUsed(ItemType.TRAMPOLINE))
		{
			AddItem(ItemType.TRAMPOLINE, false);
		}
		else if (m_items_time[3] > 0f)
		{
			float itemTimeFromChara = GetItemTimeFromChara(ItemType.TRAMPOLINE);
			m_items_time[3] = Mathf.Max(m_items_time[3], itemTimeFromChara);
			m_items_fullTime[3] = m_items_time[3];
			CountdownMeter();
		}
		else if (IsAskItemStock(ItemType.TRAMPOLINE))
		{
			m_stockItems.Add(ItemType.TRAMPOLINE);
		}
	}

	public bool IsEquipItem()
	{
		return m_equipItems.Count > 0;
	}

	public bool IsActiveTrampoline()
	{
		return m_activeTrampoline;
	}

	private bool IsActiveAltitudeTrampoline()
	{
		return m_activeAltitudeTrampoline;
	}

	public void SetActiveAltitudeTrampoline(bool on)
	{
		m_activeAltitudeTrampoline = on;
	}

	public static float GetItemTimeFromChara(ItemType itemType)
	{
		StageAbilityManager stageAbilityManager = StageAbilityManager.Instance;
		switch (itemType)
		{
		case ItemType.INVINCIBLE:
		case ItemType.MAGNET:
		case ItemType.TRAMPOLINE:
		case ItemType.COMBO:
			return (!(stageAbilityManager != null)) ? 3f : stageAbilityManager.GetItemTimePlusAblityBonus(itemType);
		case ItemType.LASER:
		case ItemType.DRILL:
		case ItemType.ASTEROID:
			return (!(stageAbilityManager != null)) ? 6f : stageAbilityManager.GetItemTimePlusAblityBonus(itemType);
		case ItemType.BARRIER:
			return 0f;
		default:
			return 0f;
		}
	}

	public void SetEquipItemTutorial(bool equipItemTutorial)
	{
		m_equipItemTutorial = equipItemTutorial;
	}

	public void SetEquippedItem(ItemType[] items)
	{
		if (items != null && items.Length != 0)
		{
			for (int i = 0; i < items.Length; i++)
			{
				m_displayEquipItems.Add(items[i]);
			}
			GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnSetEquippedItem", new MsgSetEquippedItem(m_displayEquipItems.ToArray()), SendMessageOptions.DontRequireReceiver);
		}
	}

	private void AddItem(ItemType itemType, bool equipped, bool revive = false, ChaoAbilityOption option = null)
	{
		if (option != null)
		{
			switch (itemType)
			{
			case ItemType.MAGNET:
			{
				float num = Mathf.Max(m_items_time[(int)itemType], 0f);
				m_items_time[(int)itemType] = num + option.m_specifiedTime;
				break;
			}
			case ItemType.COMBO:
				m_niths_combo_time = option.m_specifiedTime;
				m_items_time[(int)itemType] = Mathf.Max(m_item_combo_time, m_niths_combo_time);
				break;
			}
		}
		else if (itemType == ItemType.COMBO)
		{
			m_item_combo_time = GetItemTimeFromChara(itemType);
			m_items_time[(int)itemType] = Mathf.Max(m_item_combo_time, m_niths_combo_time);
		}
		else
		{
			float itemTimeFromChara = GetItemTimeFromChara(itemType);
			m_items_time[(int)itemType] = Mathf.Max(m_items_time[(int)itemType], itemTimeFromChara);
		}
		m_items_paused[(int)itemType] = false;
		switch (itemType)
		{
		case ItemType.COMBO:
		{
			GameObject playerObject4 = GetPlayerObject();
			if (playerObject4 != null)
			{
				MsgUseItem value5 = new MsgUseItem(itemType, -1f);
				playerObject4.SendMessage("OnUseItem", value5, SendMessageOptions.DontRequireReceiver);
			}
			break;
		}
		case ItemType.INVINCIBLE:
		case ItemType.MAGNET:
		case ItemType.LASER:
		case ItemType.DRILL:
		case ItemType.ASTEROID:
		{
			GameObject playerObject2 = GetPlayerObject();
			if (playerObject2 != null)
			{
				MsgUseItem value2 = new MsgUseItem(itemType, -1f);
				playerObject2.SendMessage("OnUseItem", value2, SendMessageOptions.DontRequireReceiver);
			}
			break;
		}
		case ItemType.TRAMPOLINE:
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("Gimmick");
			MsgUseItem value3 = new MsgUseItem(itemType);
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				gameObject.SendMessage("OnUseItem", value3, SendMessageOptions.DontRequireReceiver);
			}
			GameObject playerObject3 = GetPlayerObject();
			if (playerObject3 != null)
			{
				MsgUseItem value4 = new MsgUseItem(itemType, -1f);
				playerObject3.SendMessage("OnUseItem", value4, SendMessageOptions.DontRequireReceiver);
			}
			ObjUtil.RequestStartAbilityToChao(ChaoAbility.TRAMPOLINE_TIME, true);
			ObjUtil.RequestStartAbilityToChao(ChaoAbility.ITEM_TIME, true);
			m_activeTrampoline = true;
			break;
		}
		case ItemType.BARRIER:
		{
			GameObject playerObject = GetPlayerObject();
			if (playerObject != null)
			{
				MsgUseItem value = new MsgUseItem(itemType, -1f);
				playerObject.SendMessage("OnUseItem", value, SendMessageOptions.DontRequireReceiver);
			}
			break;
		}
		}
		if (equipped && !revive)
		{
			SendUsedItemMessageToCockpit(itemType);
		}
		if (itemType == ItemType.BARRIER)
		{
			itemType = ItemType.UNKNOWN;
		}
		if ((uint)itemType < 8u)
		{
			m_items_fullTime[(int)itemType] = m_items_time[(int)itemType];
		}
		SetupBusyItem();
		HudTutorial.SendItemTutorial(itemType);
	}

	private void SetupBusyItem()
	{
		ItemType busyItem = ItemType.UNKNOWN;
		ItemType[] cautionItemTypePriority = m_cautionItemTypePriority;
		foreach (ItemType itemType in cautionItemTypePriority)
		{
			float num = m_items_time[(int)itemType];
			float num2 = m_items_fullTime[(int)itemType];
			if (num > 0f && num2 > 0f)
			{
				busyItem = itemType;
				break;
			}
		}
		m_busyItem = busyItem;
		CountdownMeter();
	}

	private void CountdownMeter()
	{
		MsgCaution caution = new MsgCaution(HudCaution.Type.COUNTDOWN, CautionItemTimeRate);
		HudCaution.Instance.SetCaution(caution);
	}

	private void TimeOutItem(ItemType itemType)
	{
		switch (itemType)
		{
		case ItemType.INVINCIBLE:
		case ItemType.MAGNET:
		case ItemType.COMBO:
		case ItemType.LASER:
		case ItemType.DRILL:
		case ItemType.ASTEROID:
		{
			GameObject playerObject2 = GetPlayerObject();
			if (playerObject2 != null)
			{
				MsgInvalidateItem value2 = new MsgInvalidateItem(itemType);
				playerObject2.SendMessage("OnInvalidateItem", value2, SendMessageOptions.DontRequireReceiver);
			}
			break;
		}
		case ItemType.TRAMPOLINE:
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("Gimmick");
			MsgInvalidateItem value = new MsgInvalidateItem(itemType);
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				gameObject.SendMessage("OnInvalidateItem", value, SendMessageOptions.DontRequireReceiver);
			}
			GameObject playerObject = GetPlayerObject();
			if (playerObject != null)
			{
				playerObject.SendMessage("OnInvalidateItem", value, SendMessageOptions.DontRequireReceiver);
			}
			m_activeTrampoline = false;
			break;
		}
		}
		InvalidateItem(itemType);
	}

	private void ResetItemTime(ItemType itemType)
	{
		m_items_time[(int)itemType] = 0f;
		if (itemType == ItemType.TRAMPOLINE)
		{
			m_activeTrampoline = false;
		}
	}

	private void OnInvalidateItem(MsgInvalidateItem msg)
	{
		if (m_items_time[(int)msg.m_itemType] > 0f)
		{
			ResetItemTime(msg.m_itemType);
			m_items_paused[(int)msg.m_itemType] = false;
			InvalidateItem(msg.m_itemType);
		}
	}

	public void OnTerminateItem(MsgTerminateItem msg)
	{
		if (!m_items_paused[(int)msg.m_itemType] && m_items_time[(int)msg.m_itemType] > 0f)
		{
			ResetItemTime(msg.m_itemType);
			InvalidateItem(msg.m_itemType);
		}
	}

	public void OnPauseItemOnBoss(MsgPauseItemOnBoss msg)
	{
		m_disable_equipItem = true;
		m_forcePhantomInvalidate = false;
		for (int i = 0; i < 8; i++)
		{
			ItemType itemType = (ItemType)i;
			if (IsBossNoPauseItem(itemType))
			{
				continue;
			}
			float num = m_items_time[i];
			if (num > 0f)
			{
				if (m_items_time[i] < 2f)
				{
					ResetItemTime(itemType);
					InvalidateItem(itemType);
					MsgInvalidateItem value = new MsgInvalidateItem(itemType);
					GameObjectUtil.SendMessageToTagObjects("Player", "OnInvalidateItem", value, SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					m_items_paused[i] = true;
					MsgInvalidateItem value2 = new MsgInvalidateItem(itemType);
					GameObjectUtil.SendMessageToTagObjects("Player", "OnInvalidateItem", value2, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnPauseItemOnBoss", null, SendMessageOptions.DontRequireReceiver);
	}

	public void OnPauseItemOnChangeLevel(MsgPauseItemOnChageLevel msg)
	{
		for (int i = 0; i < 8; i++)
		{
			ItemType itemType = (ItemType)i;
			if (!IsBossNoPauseItem(itemType))
			{
				continue;
			}
			float num = m_items_time[i];
			if (num > 0f)
			{
				if (m_items_time[i] < 2f)
				{
					ResetItemTime(itemType);
					InvalidateItem(itemType);
					MsgInvalidateItem value = new MsgInvalidateItem(itemType);
					GameObjectUtil.SendMessageToTagObjects("Player", "OnInvalidateItem", value, SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					m_items_paused[i] = true;
					MsgInvalidateItem value2 = new MsgInvalidateItem(itemType);
					GameObjectUtil.SendMessageToTagObjects("Player", "OnInvalidateItem", value2, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	public void OnResumeItemOnBoss(MsgPhatomItemOnBoss msg)
	{
		bool flag = false;
		for (int i = 0; i < 8; i++)
		{
			ItemType itemType = (ItemType)i;
			float num = m_items_time[i];
			if (num > 0f && m_items_paused[i])
			{
				if (itemType == ItemType.ASTEROID || itemType == ItemType.LASER || itemType == ItemType.DRILL)
				{
					flag = true;
				}
				m_items_paused[i] = false;
				MsgUseItem value = new MsgUseItem(itemType, -1f);
				GameObjectUtil.SendMessageToTagObjects("Player", "OnUseItem", value, SendMessageOptions.DontRequireReceiver);
			}
		}
		m_disable_equipItem = false;
		m_phantomUseType = PhantomUseType.ATTACK_PAUSE;
		GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnResumeItemOnBoss", flag, SendMessageOptions.DontRequireReceiver);
	}

	public void OnDisableEquipItem(MsgDisableEquipItem msg)
	{
		m_disable_equipItem = msg.m_disable;
	}

	private bool IsBossNoPauseItem(ItemType itemType)
	{
		if (Array.IndexOf(m_feverBossNoPauseItemTypes, itemType) >= 0)
		{
			return true;
		}
		return false;
	}

	private void InvalidateItem(ItemType itemType)
	{
		bool flag = false;
		foreach (EquippedItem equipItem in m_equipItems)
		{
			if (equipItem.item == itemType)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			SendUsedItemMessageToCockpit(itemType);
		}
		SetupBusyItem();
	}

	public void OnUseEquipItem(MsgUseEquipItem msg)
	{
		if (m_availableItem)
		{
			return;
		}
		m_availableItem = true;
		m_equipItems.Clear();
		if (m_displayEquipItems.Count == 0)
		{
			return;
		}
		foreach (ItemType displayEquipItem in m_displayEquipItems)
		{
			if (displayEquipItem != ItemType.UNKNOWN)
			{
				EquippedItem equippedItem = new EquippedItem();
				equippedItem.item = displayEquipItem;
				m_equipItems.Add(equippedItem);
			}
		}
		if (m_equipItems.Count == 0)
		{
			return;
		}
		int num = 0;
		foreach (EquippedItem equipItem in m_equipItems)
		{
			equipItem.index = num;
			num++;
		}
		if (m_equipItemTutorial)
		{
			SendItemBtnTutorial();
		}
		if (m_levelInformation != null && !m_levelInformation.NowBoss)
		{
			MsgItemButtonEnable value = new MsgItemButtonEnable(true);
			GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnItemEnable", value, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void SendItemBtnTutorial()
	{
		GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnMsgTutorialItemButton", new MsgTutorialItemButton(), SendMessageOptions.DontRequireReceiver);
		GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnStartTutorial", null, SendMessageOptions.DontRequireReceiver);
	}

	public void OnMsgBossCheckState(MsgBossCheckState msg)
	{
		bool flag = msg.IsAttackOK();
		m_phantomUseType = ((!flag) ? PhantomUseType.DISABLE : PhantomUseType.ENABLE);
		if (m_levelInformation != null && m_levelInformation.NowBoss)
		{
			MsgItemButtonEnable value = new MsgItemButtonEnable(flag);
			GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnItemEnable", value, SendMessageOptions.DontRequireReceiver);
		}
		if (m_bossItemTutorial && flag)
		{
			m_bossItemTutorial = false;
			SendItemBtnTutorial();
		}
	}

	private static void SendUsedItemMessageToCockpit(ItemType itemType)
	{
		GameObjectUtil.SendMessageFindGameObject("HudCockpit", "OnUsedItem", new MsgUsedItem(itemType), SendMessageOptions.DontRequireReceiver);
	}

	public void OnAskEquipItemUsed(MsgAskEquipItemUsed msg)
	{
		msg.m_ok = IsAskEquipItemUsed(msg.m_itemType);
	}

	public ItemType GetPhantomItemType()
	{
		if (m_items_time[5] > 0f)
		{
			return ItemType.LASER;
		}
		if (m_items_time[6] > 0f)
		{
			return ItemType.DRILL;
		}
		if (m_items_time[7] > 0f)
		{
			return ItemType.ASTEROID;
		}
		return ItemType.UNKNOWN;
	}

	private bool IsAskEquipItemUsed(ItemType itemType)
	{
		if (m_playerInformation != null)
		{
			if (m_playerInformation.IsDead())
			{
				return false;
			}
			if (m_playerInformation.IsNowLastChance())
			{
				return false;
			}
		}
		if (m_characterChange)
		{
			return false;
		}
		if (!m_availableItem)
		{
			return false;
		}
		if ((uint)itemType >= 8u)
		{
			return false;
		}
		if (m_items_paused[(int)itemType])
		{
			return false;
		}
		if (m_disable_equipItem && !IsBossNoPauseItem(itemType))
		{
			return false;
		}
		bool flag = false;
		if (Array.IndexOf(m_phantomItemTypes, m_busyItem) >= 0)
		{
			flag = true;
		}
		switch (itemType)
		{
		case ItemType.INVINCIBLE:
			if (flag)
			{
				return false;
			}
			break;
		case ItemType.LASER:
		case ItemType.DRILL:
		case ItemType.ASTEROID:
			if (flag && m_busyItem != itemType)
			{
				return false;
			}
			if (m_phantomUseType == PhantomUseType.DISABLE)
			{
				return false;
			}
			break;
		}
		return true;
	}

	private bool IsAskItemStock(ItemType itemType)
	{
		if (m_playerInformation != null)
		{
			if (m_playerInformation.IsDead())
			{
				return false;
			}
			if (m_playerInformation.IsNowLastChance())
			{
				return false;
			}
		}
		if (m_characterChange)
		{
			return false;
		}
		if (!m_availableItem)
		{
			return false;
		}
		if ((uint)itemType >= 8u)
		{
			return false;
		}
		return true;
	}

	private void UpdateAltitudeTrampoline()
	{
		if (!(m_playerInformation != null))
		{
			return;
		}
		Vector3 sideViewPathPos = m_playerInformation.SideViewPathPos;
		float y = sideViewPathPos.y;
		Vector3 position = m_playerInformation.Position;
		float y2 = position.y;
		if (y + 6f < y2)
		{
			m_Altitude = true;
			return;
		}
		if (m_Altitude)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("Gimmick");
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				MsgUseItem value = new MsgUseItem(ItemType.TRAMPOLINE);
				gameObject.SendMessage("OnUseItem", value, SendMessageOptions.DontRequireReceiver);
			}
		}
		m_Altitude = false;
	}

	public void OnTransformPhantom(MsgTransformPhantom msg)
	{
		m_nowPhantom = true;
	}

	public void OnReturnFromPhantom(MsgReturnFromPhantom msg)
	{
		m_nowPhantom = false;
	}

	public void OnChangeCharaStart(MsgChangeCharaSucceed msg)
	{
		m_characterChange = true;
	}

	private void OnChangeCharaSucceed(MsgChangeCharaSucceed msg)
	{
		m_characterChange = false;
	}

	private void OnChaoAbilityLoopComboUp()
	{
		StageAbilityManager stageAbilityManager = StageAbilityManager.Instance;
		if (stageAbilityManager != null && IsAskEquipItemUsed(ItemType.COMBO))
		{
			float chaoAbilityValue = stageAbilityManager.GetChaoAbilityValue(ChaoAbility.LOOP_COMBO_UP);
			chaoAbilityValue = Mathf.Max(chaoAbilityValue, 1f);
			ChaoAbilityOption chaoAbilityOption = new ChaoAbilityOption();
			chaoAbilityOption.m_specifiedTime = chaoAbilityValue;
			m_chaoAblityComboUpRate = stageAbilityManager.GetChaoAbilityExtraValue(ChaoAbility.LOOP_COMBO_UP, ChaoType.MAIN);
			m_chaoAblityComboUpRate += stageAbilityManager.GetChaoAbilityExtraValue(ChaoAbility.LOOP_COMBO_UP, ChaoType.SUB);
			AddItem(ItemType.COMBO, false, false, chaoAbilityOption);
		}
	}

	private void OnChaoAbilityLoopMagnet()
	{
		StageAbilityManager stageAbilityManager = StageAbilityManager.Instance;
		if (stageAbilityManager != null && IsAskEquipItemUsed(ItemType.MAGNET))
		{
			float chaoAbilityValue = stageAbilityManager.GetChaoAbilityValue(ChaoAbility.LOOP_MAGNET);
			chaoAbilityValue = Mathf.Max(chaoAbilityValue, 1f);
			ChaoAbilityOption chaoAbilityOption = new ChaoAbilityOption();
			chaoAbilityOption.m_specifiedTime = chaoAbilityValue;
			AddItem(ItemType.MAGNET, false, false, chaoAbilityOption);
		}
	}

	private void OnMsgExitStage(MsgExitStage msg)
	{
		base.enabled = false;
	}

	public static ItemType GetRandomPhantomItem()
	{
		int num = UnityEngine.Random.Range(0, m_phantomItemTypes.Length);
		return m_phantomItemTypes[num];
	}

	[Conditional("DEBUG_INFO")]
	private static void DebugLog(string s)
	{
		Debug.Log("@ms " + s);
	}

	[Conditional("DEBUG_INFO")]
	private static void DebugLogWarning(string s)
	{
		Debug.LogWarning("@ms " + s);
	}
}
