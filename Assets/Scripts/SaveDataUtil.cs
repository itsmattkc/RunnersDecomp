using SaveData;
using UnityEngine;

public class SaveDataUtil : MonoBehaviour
{
	public static void ReflctResultsData()
	{
		StageScoreManager instance = StageScoreManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		uint num = (uint)instance.FinalScore;
		SaveDataManager instance2 = SaveDataManager.Instance;
		if (instance2 != null)
		{
			PlayerData playerData = instance2.PlayerData;
			if (playerData.BestScore < num)
			{
				playerData.BestScore = num;
			}
		}
	}

	public static uint GetCharaLevel(CharaType chara_type)
	{
		if (CharaType.SONIC <= chara_type && chara_type < CharaType.NUM)
		{
			SaveDataManager instance = SaveDataManager.Instance;
			if (instance != null)
			{
				CharaAbility[] abilityArray = instance.CharaData.AbilityArray;
				if (abilityArray != null)
				{
					CharaAbility charaAbility = abilityArray[(int)chara_type];
					if (charaAbility != null)
					{
						return charaAbility.GetTotalLevel();
					}
				}
			}
		}
		return 0u;
	}

	public static void SetBeforeDailyMissionSaveData(ServerPlayerState serverPlayerState)
	{
		if (SaveDataManager.Instance != null)
		{
			PlayerData playerData = SaveDataManager.Instance.PlayerData;
			if (playerData != null)
			{
				DailyMissionData beforeDailyMissionData = playerData.BeforeDailyMissionData;
				playerData.DailyMission.CopyTo(beforeDailyMissionData);
			}
		}
	}
}
