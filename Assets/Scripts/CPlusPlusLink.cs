using System.Collections.Generic;
using UnityEngine;

public class CPlusPlusLink : MonoBehaviour
{
	private static CPlusPlusLink m_instance;

	private BindingLink m_binding;

	public static CPlusPlusLink Instance
	{
		get
		{
			return m_instance;
		}
		private set
		{
		}
	}

	private void Awake()
	{
		if (m_instance == null)
		{
			m_instance = this;
			m_binding = new BindingLink();
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (m_binding != null)
		{
			m_binding.Dispose();
		}
	}

	public bool IsEnable()
	{
		if (m_binding != null)
		{
			return true;
		}
		return false;
	}

	public bool IsSecure()
	{
		bool result = false;
		if (m_binding != null)
		{
			result = m_binding.IsSecure();
		}
		return result;
	}

	public void ResetNativeResultScore()
	{
		if (m_binding != null)
		{
			m_binding.ResetNativeResultScore();
		}
	}

	public NativeScoreResult GetNativeResultScore()
	{
		if (m_binding != null)
		{
			return m_binding.GetNativeResultScore();
		}
		return default(NativeScoreResult);
	}

	public void CheckNativeHalfWayResultScore(ServerGameResults gameResult)
	{
		if (m_binding != null)
		{
			m_binding.CheckNativeHalfWayResultScore(gameResult);
			GetNativeResultScore();
		}
	}

	public void CheckNativeHalfWayQuickModeResultScore(ServerQuickModeGameResults gameResult)
	{
		if (m_binding != null)
		{
			m_binding.CheckNativeHalfWayQuickModeResultScore(gameResult);
			GetNativeResultScore();
		}
	}

	public void CheckNativeQuickModeResultTimer(int gold, int silver, int bronze, int continueCount, int main, int sub, int total, long playTime)
	{
		if (m_binding != null)
		{
			m_binding.CheckNativeQuickModeResultTimer(gold, silver, bronze, continueCount, main, sub, total, playTime);
		}
	}

	public void BootGameCheatCheck()
	{
		if (m_binding != null)
		{
			Debug.Log("CPlusPlusLink.BootGameCheatCheck");
			m_binding.BootGameCheatCheck();
		}
	}

	public void BeforeGameCheatCheck()
	{
		if (m_binding != null)
		{
			Debug.Log("CPlusPlusLink.BeforeGameCheatCheck");
			m_binding.BeforeGameCheatCheck();
		}
	}

	public string DecodeServerResponseText(string responseText)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.DecodeServerResponseText(responseText);
		}
		return result;
	}

	public string GetOnlySendBaseParamString()
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetOnlySendBaseParamString();
		}
		return result;
	}

	public string GetLoginString(string userId, string password, string migrationPassword, int platform, string device, int language, int salesLocale, int storeId, int platformSns)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetLoginString(userId, password, migrationPassword, platform, device, language, salesLocale, storeId, platformSns);
		}
		return result;
	}

	public string GetMigrationString(string userId, string migrationPassword, string migrationUserPassword, int platform, string device, int language, int salesLocale, int storeId, int platformSns)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetMigrationString(userId, migrationPassword, migrationUserPassword, platform, device, language, salesLocale, storeId, platformSns);
		}
		return result;
	}

	public string GetGetMigrationPasswordString(string userPassword)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetGetMigrationPasswordString(userPassword);
		}
		return result;
	}

	public string GetSetUserNameString(string userName)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetSetUserNameString(userName);
		}
		return result;
	}

	public string GetActStartString(List<int> modifire, List<string> distanceFriendList, bool tutorial, int eventId)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetActStartString(modifire, distanceFriendList, tutorial, eventId);
		}
		return result;
	}

	public string GetQuickModeActStartString(List<int> modifire, int tutorial)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetQuickModeActStartString(modifire, tutorial);
		}
		return result;
	}

	public string GetActRetryString()
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetActRetryString();
		}
		return result;
	}

	public string GetPostGameResultString(ServerGameResults result)
	{
		string result2 = string.Empty;
		if (m_binding != null)
		{
			result2 = m_binding.GetPostGameResultString(result);
		}
		return result2;
	}

	public string GetQuickModePostGameResultString(ServerQuickModeGameResults result)
	{
		string result2 = string.Empty;
		if (m_binding != null)
		{
			result2 = m_binding.GetQuickModePostGameResultString(result);
		}
		return result2;
	}

	public string GetGetMileageRewardString(int episode, int chapter)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetGetMileageRewardString(episode, chapter);
		}
		return result;
	}

	public string GetUpgradeCharacterString(int characterId, int abilityId)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetUpgradeCharacterString(characterId, abilityId);
		}
		return result;
	}

	public string GetUnlockedCharacterString(int characterId, int itemId)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetUnlockedCharacterString(characterId, itemId);
		}
		return result;
	}

	public string GetChangeCharacterString(int mainCharacterId, int subCharacterId)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetChangeCharacterString(mainCharacterId, subCharacterId);
		}
		return result;
	}

	public string GetGetWeeklyLeaderboardEntries(int mode, int first, int count, int type, List<string> friendIdList, int eventId)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetGetWeeklyLeaderboardEntries(mode, first, count, type, friendIdList, eventId);
		}
		return result;
	}

	public string GetGetLeagueOperatorDataString(int mode, int league)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetGetLeagueOperatorDataString(mode, league);
		}
		return result;
	}

	public string GetGetLeagueDataString(int mode)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetGetLeagueDataString(mode);
		}
		return result;
	}

	public string GetGetWeeklyLeaderboardOptionsString(int mode)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetGetWeeklyLeaderboardOptionsString(mode);
		}
		return result;
	}

	public string GetSetInviteCodeString(string friendId)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetSetInviteCodeString(friendId);
		}
		return result;
	}

	public string GetGetFacebookIncentiveString(int type, int achievementCount)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetGetFacebookIncentiveString(type, achievementCount);
		}
		return result;
	}

	public string GetSetFacebookScopedIdString(string facebookId)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetSetFacebookScopedIdString(facebookId);
		}
		return result;
	}

	public string GetGetFacebookFriendUserIdList(List<string> facebookIdList)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetGetFacebookFriendUserIdList(facebookIdList);
		}
		return result;
	}

	public string GetSendEnergyString(string friendId)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetSendEnergyString(friendId);
		}
		return result;
	}

	public string GetGetMessageString(List<int> messageId, List<int> operationMessageId)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetGetMessageString(messageId, operationMessageId);
		}
		return result;
	}

	public string GetGetRedStarExchangeListString(int itemType)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetGetRedStarExchangeListString(itemType);
		}
		return result;
	}

	public string GetRedStarExchangeString(int itemId)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetRedStarExchangeString(itemId);
		}
		return result;
	}

	public string GetBuyIosString(string receiptData)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetBuyIosString(receiptData);
		}
		return result;
	}

	public string GetBuyAndroidString(string receiptData, string receiptSignature)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetBuyAndroidString(receiptData, receiptSignature);
		}
		return result;
	}

	public string GetSetBirthdayString(string birthday)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetSetBirthdayString(birthday);
		}
		return result;
	}

	public string GetCommitWheelSpinString(int count)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetCommitWheelSpinString(count);
		}
		return result;
	}

	public string GetCommitChaoWheelSpinString(int count)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetCommitChaoWheelSpinString(count);
		}
		return result;
	}

	public string GetEquipChaoString(int mainChaoId, int subChaoId)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetEquipChaoString(mainChaoId, subChaoId);
		}
		return result;
	}

	public string GetAddSpecialEggString(int numSpecialEgg)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetAddSpecialEggString(numSpecialEgg);
		}
		return result;
	}

	public string GetGetPrizeChaoWheelSpinString(int chaoWheelSpinType)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetGetPrizeChaoWheelSpinString(chaoWheelSpinType);
		}
		return result;
	}

	public string GetGetWheelSpinGeneralString(int eventId, int spinId)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetGetWheelSpinGeneralString(eventId, spinId);
		}
		return result;
	}

	public string GetCommitWheelSpinGeneralString(int eventId, int spinCostItemId, int spinId, int spinNum)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetCommitWheelSpinGeneralString(eventId, spinCostItemId, spinId, spinNum);
		}
		return result;
	}

	public string GetGetPrizeWheelSpinGeneralString(int eventId, int spinType)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetGetPrizeWheelSpinGeneralString(eventId, spinType);
		}
		return result;
	}

	public string GetSendApolloString(int type, List<string> value)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetSendApolloString(type, value);
		}
		return result;
	}

	public string GetSetSerialCodeString(string campaignId, string serialCode, bool newUser)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetSetSerialCodeString(campaignId, serialCode, newUser);
		}
		return result;
	}

	public string GetSetNoahIdString(string noahId)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetSetNoahIdString(noahId);
		}
		return result;
	}

	public string GetEventRewardString(int eventId)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetEventRewardString(eventId);
		}
		return result;
	}

	public string GetGetEventStateString(int eventId)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetEventRewardString(eventId);
		}
		return result;
	}

	public string GetGetItemStockNumString(int eventId, int[] itemIdList)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetGetItemStockNumString(eventId, itemIdList);
		}
		return result;
	}

	public string GetDailyBattleDataHistoryString(int count)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.GetDailyBattleDataHistoryString(count);
		}
		return result;
	}

	public string ResetDailyBattleMatchingString(int type)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.ResetDailyBattleMatchingString(type);
		}
		return result;
	}

	public string LoginBonusSelectString(int rewardId, int rewardDays, int rewardSelect, int firstRewardDays, int firstRewardSelect)
	{
		string result = string.Empty;
		if (m_binding != null)
		{
			result = m_binding.LoginBonusSelectString(rewardId, rewardDays, rewardSelect, firstRewardDays, firstRewardSelect);
		}
		return result;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
