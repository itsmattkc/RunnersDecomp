using App;
using SaveData;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class MileageMapUtility
{
	public static Texture GetBGTexture()
	{
		MileageMapDataManager instance = MileageMapDataManager.Instance;
		if (instance != null)
		{
			MileageMapData mileageMapData = instance.GetMileageMapData();
			if (mileageMapData != null)
			{
				int bg_id = mileageMapData.map_data.bg_id;
				GameObject gameObject = GameObject.Find(MileageMapBGDataTable.Instance.GetTextureName(bg_id));
				if (gameObject != null)
				{
					AssetBundleTexture component = gameObject.GetComponent<AssetBundleTexture>();
					if (component != null)
					{
						return component.m_tex;
					}
				}
			}
		}
		return null;
	}

	public static Texture GetWindowBGTexture()
	{
		return GetBGTexture();
	}

	public static string GetFaceTextureName(int face_id)
	{
		return "ui_tex_event_" + face_id.ToString("0000");
	}

	public static Texture GetFaceTexture(int face_id)
	{
		MileageMapDataManager instance = MileageMapDataManager.Instance;
		if (instance != null)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(instance.gameObject, "MileageMapFace");
			if (gameObject != null)
			{
				GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, GetFaceTextureName(face_id));
				if (gameObject2 != null)
				{
					AssetBundleTexture component = gameObject2.GetComponent<AssetBundleTexture>();
					if (component != null)
					{
						return component.m_tex;
					}
				}
			}
		}
		return null;
	}

	public static BossType GetBossType()
	{
		MileageMapDataManager instance = MileageMapDataManager.Instance;
		if (instance != null)
		{
			BossEvent bossEvent = instance.GetMileageMapData().event_data.GetBossEvent();
			if (bossEvent.type_key == "Type1")
			{
				return BossType.MAP1;
			}
			if (bossEvent.type_key == "Type2")
			{
				return BossType.MAP2;
			}
			if (bossEvent.type_key == "Type3")
			{
				return BossType.MAP3;
			}
		}
		return BossType.MAP1;
	}

	public static int GetPointInterval()
	{
		int result = 500;
		MileageMapDataManager instance = MileageMapDataManager.Instance;
		if (instance != null)
		{
			result = instance.GetMileageMapData().map_data.event_interval;
		}
		return result;
	}

	public static bool IsExistBoss()
	{
		MileageMapDataManager instance = MileageMapDataManager.Instance;
		if (instance != null)
		{
			return instance.GetMileageMapData().event_data.IsBossEvent();
		}
		return false;
	}

	public static bool IsBossStage()
	{
		if (IsExistBoss())
		{
			ServerMileageMapState mileageMapState = ServerInterface.MileageMapState;
			if (mileageMapState != null)
			{
				return mileageMapState.m_point == 5;
			}
		}
		return false;
	}

	public static CharacterAttribute[] GetCharacterAttribute(int stageIndex)
	{
		StageSuggestedDataTable instance = StageSuggestedDataTable.Instance;
		if (instance != null)
		{
			return instance.GetStageSuggestedData(stageIndex);
		}
		return null;
	}

	public static void SetMileageStageIndex(int episode, int chapter, PointType type)
	{
		if (!(MileageMapDataManager.Instance != null))
		{
			return;
		}
		MileageMapData mileageMapData = MileageMapDataManager.Instance.GetMileageMapData(episode, chapter);
		if (mileageMapData != null)
		{
			StageData[] stage_data = mileageMapData.map_data.stage_data;
			if ((int)type < stage_data.Length)
			{
				MileageMapDataManager.Instance.MileageStageIndex = GetStageIndex(stage_data[(int)type].key);
			}
		}
	}

	public static string GetMileageStageName()
	{
		if (MileageMapDataManager.Instance != null)
		{
			return StageInfo.GetStageNameByIndex(MileageMapDataManager.Instance.MileageStageIndex);
		}
		return StageInfo.GetStageNameByIndex(1);
	}

	private bool IsRandom(string key)
	{
		if (key.IndexOf("RANDOM") >= 0)
		{
			return true;
		}
		return false;
	}

	public static int GetStageIndex(string key)
	{
		int num = 1;
		if (key.IndexOf("STAGE") >= 0)
		{
			string[] array = key.Split('_');
			string s = array[0].Replace("STAGE", string.Empty);
			num = int.Parse(s);
		}
		else if (key.IndexOf("RANDOM") >= 0)
		{
			int num2 = 3;
			if (key != "RANDOM")
			{
				string[] array2 = key.Split('_');
				if (array2.Length > 1)
				{
					num2 = int.Parse(array2[1]);
				}
			}
			UnityEngine.Random.seed = NetUtil.GetCurrentUnixTime();
			num = UnityEngine.Random.Range(1, num2 + 1);
			if (num > num2)
			{
				num = num2;
			}
		}
		return num;
	}

	public static string GetEventStageName(string key)
	{
		return StageInfo.GetStageNameByIndex(GetStageIndex(key));
	}

	public static TenseType GetTenseType(PointType point_type)
	{
		TenseType result = TenseType.NONE;
		MileageMapDataManager instance = MileageMapDataManager.Instance;
		if (instance != null)
		{
			StageData[] stage_data = instance.GetMileageMapData().map_data.stage_data;
			if ((int)point_type < stage_data.Length)
			{
				result = GetTenseType(stage_data[(int)point_type].key);
			}
		}
		return result;
	}

	public static TenseType GetTenseType(string key)
	{
		TenseType result = TenseType.NONE;
		if (key.IndexOf("AFTERNOON") > 0)
		{
			result = TenseType.AFTERNOON;
		}
		else if (key.IndexOf("NIGHT") > 0)
		{
			result = TenseType.NIGHT;
		}
		else
		{
			int[] array = new int[2]
			{
				0,
				1
			};
			if (array != null)
			{
				App.Random.ShuffleInt(array);
				result = (TenseType)array[0];
			}
		}
		return result;
	}

	public static bool GetChangeTense(PointType point_type)
	{
		MileageMapDataManager instance = MileageMapDataManager.Instance;
		if (instance != null)
		{
			StageData[] stage_data = instance.GetMileageMapData().map_data.stage_data;
			if ((int)point_type < stage_data.Length)
			{
				return GetChangeTense(stage_data[(int)point_type].key);
			}
		}
		return false;
	}

	public static bool GetChangeTense(string key)
	{
		if (key.IndexOf("AFTERNOON") > 0 || key.IndexOf("NIGHT") > 0)
		{
			return false;
		}
		return true;
	}

	public static void AddReward(RewardType rewardType, int count)
	{
		bool flag = ServerInterface.LoggedInServerInterface != null;
		if (flag && (uint)rewardType < 8u)
		{
			return;
		}
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance == null)
		{
			return;
		}
		switch (rewardType)
		{
		case RewardType.ITEM_INVINCIBLE:
		case RewardType.ITEM_BARRIER:
		case RewardType.ITEM_MAGNET:
		case RewardType.ITEM_TRAMPOLINE:
		case RewardType.ITEM_COMBO:
		case RewardType.ITEM_LASER:
		case RewardType.ITEM_DRILL:
		case RewardType.ITEM_ASTEROID:
			instance.ItemData.ItemCount[(int)rewardType] = AddToUint(instance.ItemData.ItemCount[(int)rewardType], count);
			break;
		case RewardType.RING:
			if (flag)
			{
				instance.ItemData.RingCountOffset += count;
				if (instance.ItemData.RingCountOffset > 0)
				{
					instance.ItemData.RingCountOffset = 0;
				}
			}
			else
			{
				instance.ItemData.RingCount = AddToUint(instance.ItemData.RingCount, count);
			}
			break;
		case RewardType.RSRING:
			if (flag)
			{
				instance.ItemData.RedRingCountOffset += count;
				if (instance.ItemData.RedRingCountOffset > 0)
				{
					instance.ItemData.RedRingCountOffset = 0;
				}
			}
			else
			{
				instance.ItemData.RedRingCount = AddToUint(instance.ItemData.RedRingCount, count);
			}
			break;
		case RewardType.ENERGY:
			if (flag)
			{
				instance.PlayerData.ChallengeCountOffset += count;
			}
			else
			{
				instance.PlayerData.ChallengeCount = AddToUint(instance.PlayerData.ChallengeCount, count);
			}
			break;
		}
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
	}

	private static uint AddToUint(uint dst, int src)
	{
		return (src < 0) ? (dst - (uint)(-src)) : (dst + (uint)src);
	}

	public static string GetText(string cellName, Dictionary<string, string> dicReplaces = null)
	{
		string text = TextUtility.GetCommonText("MileageMap", cellName);
		if (dicReplaces != null)
		{
			text = TextUtility.Replaces(text, dicReplaces);
		}
		return text;
	}

	public static int GetDisplayOffset_FromResultData(ResultData resultData, ServerItem.Id id)
	{
		if (resultData != null && resultData.m_mileageIncentiveList != null)
		{
			int num = 0;
			{
				foreach (ServerMileageIncentive mileageIncentive in resultData.m_mileageIncentiveList)
				{
					if (id == (ServerItem.Id)mileageIncentive.m_itemId)
					{
						num += mileageIncentive.m_num;
					}
				}
				return num;
			}
		}
		return 0;
	}

	public static int GetMileageClearDisplayOffset_FromResultData(ResultData resultData, ServerItem.Id id)
	{
		if (resultData != null && resultData.m_mileageIncentiveList != null)
		{
			int num = 0;
			{
				foreach (ServerMileageIncentive mileageIncentive in resultData.m_mileageIncentiveList)
				{
					if ((mileageIncentive.m_type == ServerMileageIncentive.Type.CHAPTER || mileageIncentive.m_type == ServerMileageIncentive.Type.EPISODE) && id == (ServerItem.Id)mileageIncentive.m_itemId)
					{
						num += mileageIncentive.m_num;
					}
				}
				return num;
			}
		}
		return 0;
	}

	public static bool IsRankUp_FromResultData(ResultData resultData)
	{
		if (resultData != null && resultData.m_mileageIncentiveList != null)
		{
			foreach (ServerMileageIncentive mileageIncentive in resultData.m_mileageIncentiveList)
			{
				if (mileageIncentive.m_type == ServerMileageIncentive.Type.CHAPTER)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static void SetDisplayOffset_FromResultData(ResultData resultData)
	{
		if (!(ServerInterface.LoggedInServerInterface != null))
		{
			return;
		}
		int displayOffset_FromResultData = GetDisplayOffset_FromResultData(resultData, ServerItem.Id.RSRING);
		int displayOffset_FromResultData2 = GetDisplayOffset_FromResultData(resultData, ServerItem.Id.RING);
		bool flag = IsRankUp_FromResultData(resultData);
		if (!(SaveDataManager.Instance != null))
		{
			return;
		}
		if (flag)
		{
			PlayerData playerData = SaveDataManager.Instance.PlayerData;
			if (playerData != null)
			{
				playerData.RankOffset = -1;
			}
		}
		ItemData itemData = SaveDataManager.Instance.ItemData;
		if (itemData != null)
		{
			itemData.RedRingCountOffset = -displayOffset_FromResultData;
			itemData.RingCountOffset = -displayOffset_FromResultData2;
		}
	}
}
