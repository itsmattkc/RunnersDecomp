using App;
using DataTable;
using LitJson;
using SaveData;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class NetUtil
{
	public static readonly float ConnectTimeOut = 15f;

	private static readonly TimeSpan ReloginStartTime = new TimeSpan(0, 30, 0);

	private static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	public static string GetWebPageURL(InformationDataTable.Type type)
	{
		return NetBaseUtil.InformationServerURL + InformationDataTable.GetUrl(type);
	}

	public static string GetAssetBundleUrl()
	{
		string str = NetBaseUtil.AssetServerURL + "assetbundle/";
		if (Application.platform == RuntimePlatform.IPhonePlayer) 
		{
			return str + "iphone/";
		}
		else
		{
		return str + "android/";
        }
	}

	public static string Base64Encode(string text)
	{
		return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
	}

	public static byte[] Base64EncodeToBytes(string text)
	{
		string s = Base64Encode(text);
		return Encoding.ASCII.GetBytes(s);
	}

	public static string Base64Decode(string text)
	{
		byte[] bytes = Convert.FromBase64String(text);
		return Encoding.UTF8.GetString(bytes);
	}

	public static string Base64DecodeFromBytes(byte[] byte_base64)
	{
		string @string = Encoding.ASCII.GetString(byte_base64);
		return Base64Decode(@string);
	}

	public static byte[] Xor(byte[] bytes)
	{
		byte[] array = new byte[bytes.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (byte)(bytes[i] ^ 0xFF);
		}
		return array;
	}

	public static string CalcMD5String(string plainText)
	{
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] bytes = Encoding.UTF8.GetBytes(plainText);
		byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes);
		string text = string.Empty;
		byte[] array2 = array;
		foreach (byte b in array2)
		{
			string text2 = b.ToString("X2");
			text += text2.ToLower();
		}
		return text;
	}

	public static void SyncSaveDataAndDataBase(ServerPlayerState playerState)
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance == null)
		{
			return;
		}
		ItemData itemData = instance.ItemData;
		if (itemData != null)
		{
			itemData.RingCount = (uint)playerState.m_numRings;
			itemData.RedRingCount = (uint)playerState.m_numRedRings;
			for (ItemType itemType = ItemType.INVINCIBLE; itemType < ItemType.NUM; itemType++)
			{
				ServerItemState itemStateById = playerState.GetItemStateById((int)new ServerItem(itemType).id);
				if (itemStateById != null)
				{
					itemData.ItemCount[(int)itemType] = (uint)itemStateById.m_num;
				}
			}
		}
		PlayerData playerData = instance.PlayerData;
		if (playerData != null)
		{
			playerData.ProgressStatus = (uint)playerState.m_dailyMissionValue;
			playerData.TotalDistance = playerState.m_totalDistance;
			playerData.ChallengeCount = (uint)playerState.m_numEnergy;
			playerData.BestScore = playerState.m_totalHighScore;
			playerData.BestScoreQuick = playerState.m_totalHighScoreQuick;
			playerData.MainChara = new ServerItem((ServerItem.Id)playerState.m_mainCharaId).charaType;
			playerData.SubChara = new ServerItem((ServerItem.Id)playerState.m_subCharaId).charaType;
			playerData.Rank = (uint)playerState.m_numRank;
			playerData.MainChaoID = new ServerItem((ServerItem.Id)playerState.m_mainChaoId).chaoId;
			playerData.SubChaoID = new ServerItem((ServerItem.Id)playerState.m_subChaoId).chaoId;
			playerData.DailyMission.id = playerState.m_dailyMissionId;
			playerData.DailyMission.progress = playerState.m_dailyMissionValue;
			playerData.DailyMission.missions_complete = playerState.m_dailyChallengeComplete;
			playerData.DailyMission.clear_count = playerState.m_numDailyChalCont;
		}
	}

	public static void SyncSaveDataAndDataBase(ServerCharacterState[] charaState)
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance == null)
		{
			return;
		}
		CharaData charaData = instance.CharaData;
		if (charaData == null)
		{
			return;
		}
		for (int i = 0; i < 29; i++)
		{
			ServerCharacterState serverCharacterState = charaState[i];
			if (serverCharacterState == null)
			{
				continue;
			}
			CharaAbility charaAbility = charaData.AbilityArray[i];
			if (charaAbility == null)
			{
				continue;
			}
			for (int j = 0; j < serverCharacterState.AbilityLevel.Count; j++)
			{
				int id = 120000 + j;
				AbilityType abilityType = new ServerItem((ServerItem.Id)id).abilityType;
				if (abilityType != AbilityType.NONE)
				{
					charaAbility.Ability[(int)abilityType] = (uint)serverCharacterState.AbilityLevel[j];
				}
			}
			if (serverCharacterState != null)
			{
				if (serverCharacterState.IsUnlocked)
				{
					charaData.Status[i] = 1;
				}
				else
				{
					charaData.Status[i] = 0;
				}
			}
		}
	}

	public static void SyncSaveDataAndDataBase(List<ServerChaoState> chaoState)
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (!(instance == null) && chaoState != null)
		{
			instance.ChaoData = new SaveData.ChaoData(chaoState);
		}
	}

	public static void SyncSaveDataAndDailyMission(ServerDailyChallengeState dailyChallengeState = null)
	{
		if (dailyChallengeState == null)
		{
			dailyChallengeState = ServerInterface.DailyChallengeState;
		}
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance == null)
		{
			return;
		}
		PlayerData playerData = instance.PlayerData;
		if (playerData == null)
		{
			return;
		}
		playerData.DailyMission.clear_count = dailyChallengeState.m_numIncentiveCont;
		playerData.DailyMission.date = dailyChallengeState.m_numDailyChalDay;
		playerData.DailyMission.max = dailyChallengeState.m_maxDailyChalDay;
		playerData.DailyMission.reward_max = dailyChallengeState.m_maxIncentive;
		foreach (ServerDailyChallengeIncentive incentive in dailyChallengeState.m_incentiveList)
		{
			int num = incentive.m_numIncentiveCont - 1;
			Debug.Log(string.Empty);
			playerData.DailyMission.reward_id[num] = (int)new ServerItem((ServerItem.Id)incentive.m_itemId).rewardType;
			playerData.DailyMission.reward_count[num] = incentive.m_num;
		}
	}

	public static bool IsAlreadySessionTimeOut(DateTime sessionTimeLimit, DateTime currentTime)
	{
		if (sessionTimeLimit <= currentTime)
		{
			return true;
		}
		TimeSpan t = new TimeSpan(0, 0, 0, (int)ConnectTimeOut);
		DateTime t2 = currentTime + t;
		if (sessionTimeLimit <= t2)
		{
			return true;
		}
		return false;
	}

	public static bool IsSessionTimeOutSoon(DateTime sessionTimeLimit, DateTime currentTime)
	{
		if (IsAlreadySessionTimeOut(sessionTimeLimit, currentTime + ReloginStartTime))
		{
			return true;
		}
		return false;
	}

	private static JsonData Find(JsonData from, string key)
	{
		return from[key];
	}

	public static bool IsExist(JsonData from, string key)
	{
		return from.ContainsKey(key);
	}

	public static JsonData GetJsonArray(JsonData jdata, string key)
	{
		if (jdata == null)
		{
			return null;
		}
		if (!IsExist(jdata, key))
		{
			return null;
		}
		JsonData jsonData = Find(jdata, key);
		if (jsonData == null)
		{
			return null;
		}
		if (jsonData.IsArray)
		{
			return jsonData;
		}
		Debug.Log("GetJsonArray : There is not array : " + key);
		return null;
	}

	public static JsonData GetJsonArrayObject(JsonData jdata, string key, int index)
	{
		JsonData jsonArray = GetJsonArray(jdata, key);
		if (jsonArray != null)
		{
			return jsonArray[index];
		}
		return null;
	}

	public static JsonData GetJsonObject(JsonData jdata, string key)
	{
		if (!jdata.ContainsKey(key))
		{
			return null;
		}
		JsonData jsonData = Find(jdata, key);
		if (jsonData != null && jsonData.IsObject)
		{
			return jsonData;
		}
		return null;
	}

	public static string GetJsonString(JsonData jdata, string key)
	{
		if (!jdata.ContainsKey(key))
		{
			return string.Empty;
		}
		JsonData jdata2 = Find(jdata, key);
		return GetJsonString(jdata2);
	}

	public static string GetJsonString(JsonData jdata)
	{
		string result = null;
		if (jdata != null)
		{
			if (jdata.IsString)
			{
				result = (string)jdata;
			}
			else if (jdata.IsInt)
			{
				result = ((int)jdata).ToString();
			}
			else if (jdata.IsLong)
			{
				result = ((long)jdata).ToString();
			}
			else
			{
				Debug.Log("GetJsonIntValue : Illegal JSON Object");
			}
		}
		return result;
	}

	public static float GetJsonFloat(JsonData jdata, string key)
	{
		if (!jdata.ContainsKey(key))
		{
			return 0f;
		}
		JsonData jdata2 = Find(jdata, key);
		return GetJsonFloat(jdata2);
	}

	public static float GetJsonFloat(JsonData jdata)
	{
		float result = 0f;
		if (jdata != null)
		{
			if (jdata.IsDouble)
			{
				result = (long)jdata;
			}
			else if (jdata.IsString)
			{
				string s = (string)jdata;
				if (!float.TryParse(s, out result))
				{
					result = 0f;
				}
			}
			else
			{
				Debug.Log("GetJsonFloat : Illegal JSON Object");
			}
		}
		return result;
	}

	public static int GetJsonInt(JsonData jdata, string key)
	{
		if (!jdata.ContainsKey(key))
		{
			return 0;
		}
		JsonData jdata2 = Find(jdata, key);
		return GetJsonInt(jdata2);
	}

	public static int GetJsonInt(JsonData jdata)
	{
		int result = 0;
		if (jdata != null)
		{
			if (jdata.IsInt)
			{
				result = (int)jdata;
			}
			else if (jdata.IsString)
			{
				string s = (string)jdata;
				if (!int.TryParse(s, out result))
				{
					result = 0;
				}
			}
			else
			{
				Debug.Log("GetJsonIntValue : Illegal JSON Object");
			}
		}
		return result;
	}

	public static long GetJsonLong(JsonData jdata, string key)
	{
		if (!jdata.ContainsKey(key))
		{
			return 0L;
		}
		JsonData jdata2 = Find(jdata, key);
		return GetJsonLong(jdata2);
	}

	public static long GetJsonLong(JsonData jdata)
	{
		long result = 0L;
		if (jdata != null)
		{
			if (jdata.IsLong)
			{
				result = (long)jdata;
			}
			else if (jdata.IsInt)
			{
				result = (int)jdata;
			}
			else if (jdata.IsString)
			{
				string s = (string)jdata;
				if (!long.TryParse(s, out result))
				{
					result = 0L;
				}
			}
			else
			{
				Debug.Log("GetJsonIntValue : Illegal JSON Object");
			}
		}
		return result;
	}

	public static bool GetJsonFlag(JsonData jdata, string key)
	{
		if (!jdata.ContainsKey(key))
		{
			return false;
		}
		JsonData jdata2 = Find(jdata, key);
		return GetJsonFlag(jdata2);
	}

	public static bool GetJsonFlag(JsonData jdata)
	{
		if (GetJsonInt(jdata) != 0)
		{
			return true;
		}
		return false;
	}

	public static bool GetJsonBoolean(JsonData jdata, string key)
	{
		if (!jdata.ContainsKey(key))
		{
			return false;
		}
		JsonData jdata2 = Find(jdata, key);
		return GetJsonBoolean(jdata2);
	}

	public static bool GetJsonBoolean(JsonData jdata)
	{
		if (jdata.IsBoolean)
		{
			return (bool)jdata;
		}
		return false;
	}

	public static void WriteValue(JsonWriter writer, string propertyName, object value)
	{
		if (writer != null)
		{
			if (propertyName != null && string.Empty != propertyName)
			{
				writer.WritePropertyName(propertyName);
			}
			if (value is string)
			{
				writer.Write((string)value);
			}
			else if (value is int)
			{
				writer.Write(((int)value).ToString());
			}
			else if (value is long)
			{
				writer.Write(((long)value).ToString());
			}
			else if (value is ulong)
			{
				writer.Write(((ulong)value).ToString());
			}
			else if (value is float)
			{
				writer.Write(((float)value).ToString());
			}
			else if (value is bool)
			{
				writer.Write(((bool)value).ToString());
			}
		}
	}

	public static void WriteObject(JsonWriter writer, string objectName, Dictionary<string, object> properties)
	{
		if (writer == null || properties == null)
		{
			return;
		}
		if (objectName != null && string.Empty != objectName)
		{
			writer.WritePropertyName(objectName);
		}
		writer.WriteObjectStart();
		foreach (KeyValuePair<string, object> property in properties)
		{
			string key = property.Key;
			object value = property.Value;
			WriteValue(writer, key, value);
		}
		writer.WriteObjectEnd();
	}

	public static void WriteArray(JsonWriter writer, string arrayName, List<object> objects)
	{
		if (writer != null && objects != null)
		{
			writer.WritePropertyName(arrayName);
			writer.WriteArrayStart();
			int count = objects.Count;
			for (int i = 0; i < count; i++)
			{
				object value = objects[i];
				WriteValue(writer, string.Empty, value);
			}
			writer.WriteArrayEnd();
		}
	}

	public static List<ServerDailyBattleDataPair> AnalyzeDailyBattleDataPairListJson(JsonData jdata, string key = "battleDataList")
	{
		List<ServerDailyBattleDataPair> list = new List<ServerDailyBattleDataPair>();
		JsonData jsonArray = GetJsonArray(jdata, key);
		if (jsonArray == null)
		{
			return list;
		}
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jsonData = jsonArray[i];
			if (jsonData != null)
			{
				ServerDailyBattleDataPair item = AnalyzeDailyBattleDataPairJson(jsonData);
				list.Add(item);
			}
		}
		return list;
	}

	public static ServerDailyBattleDataPair AnalyzeDailyBattleDataPairJson(JsonData jdata, string myKey = "battleData", string rivalKey = "rivalBattleData", string startTimeKey = "startTime", string endTimeKey = "endTime")
	{
		ServerDailyBattleDataPair serverDailyBattleDataPair = new ServerDailyBattleDataPair();
		if (string.IsNullOrEmpty(myKey))
		{
			myKey = "battleData";
		}
		if (string.IsNullOrEmpty(rivalKey))
		{
			rivalKey = "rivalBattleData";
		}
		if (string.IsNullOrEmpty(startTimeKey))
		{
			startTimeKey = "startTime";
		}
		if (string.IsNullOrEmpty(endTimeKey))
		{
			endTimeKey = "endTime";
		}
		serverDailyBattleDataPair.starTime = AnalyzeDateTimeJson(jdata, startTimeKey);
		serverDailyBattleDataPair.endTime = AnalyzeDateTimeJson(jdata, endTimeKey);
		serverDailyBattleDataPair.myBattleData = AnalyzeDailyBattleDataJson(jdata, myKey);
		serverDailyBattleDataPair.rivalBattleData = AnalyzeDailyBattleDataJson(jdata, rivalKey);
		return serverDailyBattleDataPair;
	}

	public static ServerDailyBattleData AnalyzeDailyBattleDataJson(JsonData jdata, string key)
	{
		ServerDailyBattleData serverDailyBattleData = new ServerDailyBattleData();
		JsonData jsonObject = GetJsonObject(jdata, key);
		if (jsonObject != null)
		{
			serverDailyBattleData.maxScore = GetJsonLong(jsonObject, "maxScore");
			serverDailyBattleData.league = GetJsonInt(jsonObject, "league");
			serverDailyBattleData.userId = GetJsonString(jsonObject, "userId");
			serverDailyBattleData.name = GetJsonString(jsonObject, "name");
			serverDailyBattleData.loginTime = GetJsonLong(jsonObject, "loginTime");
			serverDailyBattleData.mainChaoId = GetJsonInt(jsonObject, "mainChaoId");
			serverDailyBattleData.mainChaoLevel = GetJsonInt(jsonObject, "mainChaoLevel");
			serverDailyBattleData.subChaoId = GetJsonInt(jsonObject, "subChaoId");
			serverDailyBattleData.subChaoLevel = GetJsonInt(jsonObject, "subChaoLevel");
			serverDailyBattleData.numRank = GetJsonInt(jsonObject, "numRank");
			serverDailyBattleData.charaId = GetJsonInt(jsonObject, "charaId");
			serverDailyBattleData.charaLevel = GetJsonInt(jsonObject, "charaLevel");
			serverDailyBattleData.subCharaId = GetJsonInt(jsonObject, "subCharaId");
			serverDailyBattleData.subCharaLevel = GetJsonInt(jsonObject, "subCharaLevel");
			serverDailyBattleData.goOnWin = GetJsonInt(jsonObject, "goOnWin");
			serverDailyBattleData.isSentEnergy = GetJsonFlag(jsonObject, "energyFlg");
			serverDailyBattleData.language = (Env.Language)GetJsonInt(jsonObject, "language");
		}
		return serverDailyBattleData;
	}

	public static ServerDailyBattleStatus AnalyzeDailyBattleStatusJson(JsonData jdata, string key)
	{
		ServerDailyBattleStatus serverDailyBattleStatus = new ServerDailyBattleStatus();
		JsonData jsonObject = GetJsonObject(jdata, key);
		if (jsonObject != null)
		{
			serverDailyBattleStatus.numWin = GetJsonInt(jsonObject, "numWin");
			serverDailyBattleStatus.numLose = GetJsonInt(jsonObject, "numLose");
			serverDailyBattleStatus.numDraw = GetJsonInt(jsonObject, "numDraw");
			serverDailyBattleStatus.numLoseByDefault = GetJsonInt(jsonObject, "numLoseByDefault");
			serverDailyBattleStatus.goOnWin = GetJsonInt(jsonObject, "goOnWin");
			serverDailyBattleStatus.goOnLose = GetJsonInt(jsonObject, "goOnLose");
		}
		return serverDailyBattleStatus;
	}

	public static List<ServerDailyBattlePrizeData> AnalyzeDailyBattlePrizeDataJson(JsonData jdata, string key)
	{
		List<ServerDailyBattlePrizeData> list = new List<ServerDailyBattlePrizeData>();
		JsonData jsonArray = GetJsonArray(jdata, key);
		if (jsonArray == null)
		{
			return list;
		}
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jsonData = jsonArray[i];
			if (jsonData == null)
			{
				continue;
			}
			ServerDailyBattlePrizeData serverDailyBattlePrizeData = new ServerDailyBattlePrizeData();
			serverDailyBattlePrizeData.operatorData = GetJsonInt(jsonData, "operator");
			serverDailyBattlePrizeData.number = GetJsonInt(jsonData, "number");
			JsonData jsonArray2 = GetJsonArray(jsonData, "presentList");
			int count2 = jsonArray2.Count;
			for (int j = 0; j < count2; j++)
			{
				JsonData jsonData2 = jsonArray2[j];
				if (jsonData2 != null)
				{
					ServerItemState itemState = AnalyzeItemStateJson(jsonData2, string.Empty);
					serverDailyBattlePrizeData.AddItemState(itemState);
				}
			}
			list.Add(serverDailyBattlePrizeData);
		}
		return list;
	}

	public static DateTime AnalyzeDateTimeJson(JsonData jdata, string key)
	{
		long jsonLong = GetJsonLong(jdata, key);
		return GetLocalDateTime(jsonLong);
	}

	public static ServerPlayerState AnalyzePlayerStateJson(JsonData jdata, string key)
	{
		JsonData jsonObject = GetJsonObject(jdata, key);
		if (jsonObject == null)
		{
			return null;
		}
		ServerPlayerState playerState = new ServerPlayerState();
		AnalyzePlayerState_Scores(jsonObject, ref playerState);
		AnalyzePlayerState_Rings(jsonObject, ref playerState);
		AnalyzePlayerState_Enegies(jsonObject, ref playerState);
		AnalyzePlayerState_Messages(jsonObject, ref playerState);
		AnalyzePlayerState_DailyChallenge(jsonObject, ref playerState);
		ServerCharacterState[] array = AnalyzePlayerState_CharactersStates(jsonObject);
		ServerCharacterState[] array2 = array;
		foreach (ServerCharacterState serverCharacterState in array2)
		{
			if (serverCharacterState != null)
			{
				playerState.SetCharacterState(serverCharacterState);
			}
		}
		List<ServerChaoState> list = AnalyzePlayerState_ChaoStates(jsonObject);
		foreach (ServerChaoState item in list)
		{
			if (item != null)
			{
				playerState.ChaoStates.Add(item);
			}
		}
		AnalyzePlayerState_ItemsStates(jsonObject, ref playerState);
		AnalyzePlayerState_EquipItemList(jsonObject, ref playerState);
		AnalyzePlayerState_Other(jsonObject, ref playerState);
		return playerState;
	}

	private static void AnalyzePlayerState_Scores(JsonData jdata, ref ServerPlayerState playerState)
	{
		playerState.m_totalHighScore = GetJsonLong(jdata, "totalHighScore");
		playerState.m_totalHighScoreQuick = GetJsonLong(jdata, "quickTotalHighScore");
		playerState.m_totalDistance = GetJsonLong(jdata, "totalDistance");
		playerState.m_maxDistance = GetJsonLong(jdata, "maximumDistance");
		playerState.m_leagueIndex = GetJsonInt(jdata, "rankingLeague");
		playerState.m_leagueIndexQuick = GetJsonInt(jdata, "quickRankingLeague");
	}

	private static void AnalyzePlayerState_Rings(JsonData jdata, ref ServerPlayerState playerState)
	{
		playerState.m_numFreeRings = GetJsonInt(jdata, "numRings");
		playerState.m_numBuyRings = GetJsonInt(jdata, "numBuyRings");
		playerState.m_numRings = playerState.m_numFreeRings + playerState.m_numBuyRings;
		playerState.m_numFreeRedRings = GetJsonInt(jdata, "numRedRings");
		playerState.m_numBuyRedRings = GetJsonInt(jdata, "numBuyRedRings");
		playerState.m_numRedRings = playerState.m_numFreeRedRings + playerState.m_numBuyRedRings;
	}

	private static void AnalyzePlayerState_Enegies(JsonData jdata, ref ServerPlayerState playerState)
	{
		playerState.m_numFreeEnergy = GetJsonInt(jdata, "energy");
		playerState.m_numBuyEnergy = GetJsonInt(jdata, "energyBuy");
		playerState.m_numEnergy = playerState.m_numFreeEnergy + playerState.m_numBuyEnergy;
		long jsonLong = GetJsonLong(jdata, "energyRenewsAt");
		playerState.m_energyRenewsAt = GetLocalDateTime(jsonLong);
	}

	private static void AnalyzePlayerState_Messages(JsonData jdata, ref ServerPlayerState playerState)
	{
		playerState.m_numUnreadMessages = GetJsonInt(jdata, "mumMessages");
	}

	private static void AnalyzePlayerState_DailyChallenge(JsonData jdata, ref ServerPlayerState playerState)
	{
		playerState.m_dailyMissionId = GetJsonInt(jdata, "dailyMissionId");
		playerState.m_dailyMissionValue = GetJsonInt(jdata, "dailyChallengeValue");
		playerState.m_dailyChallengeComplete = GetJsonFlag(jdata, "dailyChallengeComplete");
		long jsonLong = GetJsonLong(jdata, "dailyMissionEndTime");
		playerState.m_endDailyMissionDate = GetLocalDateTime(jsonLong);
		playerState.m_numDailyChalCont = GetJsonInt(jdata, "numDailyChalCont");
	}

	public static ServerCharacterState[] AnalyzePlayerState_CharactersStates(JsonData jdata, string arrayName)
	{
		ServerCharacterState[] array = new ServerCharacterState[29];
		for (int i = 0; i < 29; i++)
		{
			array[i] = new ServerCharacterState();
		}
		JsonData jsonArray = GetJsonArray(jdata, arrayName);
		if (jsonArray == null)
		{
			return array;
		}
		int count = jsonArray.Count;
		for (int j = 0; j < count; j++)
		{
			JsonData jsonData = jsonArray[j];
			int jsonInt = GetJsonInt(jsonData, "characterId");
			CharaType charaType = CharaType.UNKNOWN;
			charaType = new ServerItem((ServerItem.Id)jsonInt).charaType;
			ServerCharacterState serverCharacterState = array[(int)charaType];
			if (serverCharacterState == null)
			{
				continue;
			}
			serverCharacterState.Id = jsonInt;
			serverCharacterState.Status = (ServerCharacterState.CharacterStatus)GetJsonInt(jsonData, "status");
			serverCharacterState.OldStatus = serverCharacterState.Status;
			serverCharacterState.Level = GetJsonInt(jsonData, "level");
			serverCharacterState.Cost = GetJsonInt(jsonData, "numRings");
			serverCharacterState.OldCost = serverCharacterState.Cost;
			serverCharacterState.NumRedRings = GetJsonInt(jsonData, "numRedRings");
			JsonData jsonArray2 = GetJsonArray(jsonData, "abilityLevel");
			int count2 = jsonArray2.Count;
			serverCharacterState.AbilityLevel.Clear();
			for (int k = 0; k < count2; k++)
			{
				serverCharacterState.AbilityLevel.Add(GetJsonInt(jsonArray2[k]));
				serverCharacterState.OldAbiltyLevel.Add(GetJsonInt(jsonArray2[k]));
			}
			serverCharacterState.Condition = (ServerCharacterState.LockCondition)GetJsonInt(jsonData, "lockCondition");
			serverCharacterState.star = GetJsonInt(jsonData, "star");
			serverCharacterState.starMax = GetJsonInt(jsonData, "starMax");
			serverCharacterState.priceNumRings = GetJsonInt(jsonData, "priceNumRings");
			serverCharacterState.priceNumRedRings = GetJsonInt(jsonData, "priceNumRedRings");
			serverCharacterState.Exp = GetJsonInt(jsonData, "exp");
			serverCharacterState.OldExp = serverCharacterState.Exp;
			if (!IsExist(jsonData, "campaignList"))
			{
				continue;
			}
			JsonData jsonArray3 = GetJsonArray(jsonData, "campaignList");
			if (jsonArray3 == null)
			{
				continue;
			}
			ServerCampaignData serverCampaignData = new ServerCampaignData();
			serverCampaignData.campaignType = Constants.Campaign.emType.CharacterUpgradeCost;
			serverCampaignData.id = serverCharacterState.Id;
			ServerInterface.CampaignState.RemoveCampaign(serverCampaignData);
			int count3 = jsonArray3.Count;
			for (int l = 0; l < count3; l++)
			{
				ServerCampaignData serverCampaignData2 = AnalyzeCampaignDataJson(jsonArray3[l], string.Empty);
				if (serverCampaignData2 != null)
				{
					serverCampaignData2.id = serverCharacterState.Id;
					ServerInterface.CampaignState.RegistCampaign(serverCampaignData2);
				}
			}
		}
		return array;
	}

	public static ServerPlayCharacterState[] AnalyzePlayerState_PlayCharactersStates(JsonData jdata)
	{
		ServerCharacterState[] array = AnalyzePlayerState_CharactersStates(jdata, "playCharacterState");
		List<ServerPlayCharacterState> list = new List<ServerPlayCharacterState>();
		ServerCharacterState[] array2 = array;
		foreach (ServerCharacterState serverCharacterState in array2)
		{
			if (serverCharacterState != null)
			{
				ServerPlayCharacterState serverPlayCharacterState = ServerPlayCharacterState.CreatePlayCharacterState(serverCharacterState);
				if (serverPlayCharacterState != null)
				{
					list.Add(serverPlayCharacterState);
				}
			}
		}
		JsonData jsonArray = GetJsonArray(jdata, "playCharacterState");
		if (jsonArray == null)
		{
			return list.ToArray();
		}
		int count = jsonArray.Count;
		for (int j = 0; j < count; j++)
		{
			JsonData jsonData = jsonArray[j];
			if (jsonData == null)
			{
				continue;
			}
			if (j >= list.Count)
			{
				break;
			}
			int jsonInt = GetJsonInt(jsonArray[j], "characterId");
			CharaType charaType = CharaType.UNKNOWN;
			charaType = new ServerItem((ServerItem.Id)jsonInt).charaType;
			ServerPlayCharacterState serverPlayCharacterState2 = list[(int)charaType];
			if (serverPlayCharacterState2 == null)
			{
				continue;
			}
			serverPlayCharacterState2.star = GetJsonInt(jsonData, "star");
			serverPlayCharacterState2.starMax = GetJsonInt(jsonData, "starMax");
			serverPlayCharacterState2.priceNumRings = GetJsonInt(jsonData, "priceNumRings");
			serverPlayCharacterState2.priceNumRedRings = GetJsonInt(jsonData, "priceNumRedRings");
			JsonData jsonArray2 = GetJsonArray(jsonData, "abilityLevelup");
			int count2 = jsonArray2.Count;
			serverPlayCharacterState2.abilityLevelUp.Clear();
			for (int k = 0; k < count2; k++)
			{
				ServerItem serverItem = new ServerItem((ServerItem.Id)GetJsonInt(jsonArray2[k]));
				serverPlayCharacterState2.abilityLevelUp.Add((int)serverItem.abilityType);
			}
			if (IsExist(jsonData, "abilityLevelupExp"))
			{
				JsonData jsonArray3 = GetJsonArray(jsonData, "abilityLevelupExp");
				int count3 = jsonArray3.Count;
				serverPlayCharacterState2.abilityLevelUpExp.Clear();
				for (int l = 0; l < count3; l++)
				{
					int jsonInt2 = GetJsonInt(jsonArray3[l]);
					serverPlayCharacterState2.abilityLevelUpExp.Add(jsonInt2);
				}
			}
		}
		return list.ToArray();
	}

	public static ServerCharacterState[] AnalyzePlayerState_CharactersStates(JsonData jdata)
	{
		return AnalyzePlayerState_CharactersStates(jdata, "characterState");
	}

	public static List<ServerChaoState> AnalyzePlayerState_ChaoStates(JsonData jdata)
	{
		List<ServerChaoState> list = new List<ServerChaoState>();
		JsonData jsonArray = GetJsonArray(jdata, "chaoState");
		if (jsonArray == null)
		{
			jsonArray = GetJsonArray(jdata, "chaoStatus");
		}
		if (jsonArray == null)
		{
			return list;
		}
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jdata2 = jsonArray[i];
			int jsonInt = GetJsonInt(jdata2, "chaoId");
			ServerChaoState serverChaoState = new ServerChaoState();
			serverChaoState.Id = jsonInt;
			serverChaoState.Status = (ServerChaoState.ChaoStatus)GetJsonInt(jdata2, "status");
			serverChaoState.Level = GetJsonInt(jdata2, "level");
			serverChaoState.Dealing = (ServerChaoState.ChaoDealing)GetJsonInt(jdata2, "setStatus");
			serverChaoState.Rarity = GetJsonInt(jdata2, "rarity");
			serverChaoState.NumInvite = GetJsonInt(jdata2, "numInvite");
			serverChaoState.NumAcquired = GetJsonInt(jdata2, "acquired");
			serverChaoState.Hidden = GetJsonFlag(jdata2, "hidden");
			list.Add(serverChaoState);
		}
		return list;
	}

	private static void AnalyzePlayerState_ItemsStates(JsonData jdata, ref ServerPlayerState playerState)
	{
		JsonData jsonArray = GetJsonArray(jdata, "items");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jdata2 = jsonArray[i];
			ServerItemState itemState = AnalyzeItemStateJson(jdata2, string.Empty);
			playerState.AddItemState(itemState);
		}
	}

	private static void AnalyzePlayerState_EquipItemList(JsonData jdata, ref ServerPlayerState playerState)
	{
		JsonData jsonArray = GetJsonArray(jdata, "equipItemList");
		for (int i = 0; i < playerState.m_equipItemList.Length; i++)
		{
			if (i < jsonArray.Count)
			{
				int jsonInt = GetJsonInt(jsonArray[i]);
				playerState.m_equipItemList[i] = jsonInt;
			}
			else
			{
				playerState.m_equipItemList[i] = -1;
			}
		}
	}

	private static void AnalyzePlayerState_Other(JsonData jdata, ref ServerPlayerState playerState)
	{
		playerState.m_numContinuesUsed = GetJsonInt(jdata, "numContinuesUsed");
		playerState.m_mainCharaId = GetJsonInt(jdata, "mainCharaID");
		playerState.m_subCharaId = GetJsonInt(jdata, "subCharaID");
		playerState.m_useSubCharacter = GetJsonFlag(jdata, "useSubCharacter");
		playerState.m_mainChaoId = GetJsonInt(jdata, "mainChaoID");
		playerState.m_subChaoId = GetJsonInt(jdata, "subChaoID");
		playerState.m_numPlaying = GetJsonInt(jdata, "numPlaying");
		playerState.m_numAnimals = GetJsonInt(jdata, "numAnimals");
		playerState.m_numRank = GetJsonInt(jdata, "numRank");
	}

	public static List<ServerWheelSpinInfo> AnalyzeWheelSpinInfo(JsonData jdata, string key)
	{
		List<ServerWheelSpinInfo> list = new List<ServerWheelSpinInfo>();
		JsonData jsonArray = GetJsonArray(jdata, key);
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jdata2 = jsonArray[i];
			ServerWheelSpinInfo serverWheelSpinInfo = new ServerWheelSpinInfo();
			serverWheelSpinInfo.id = GetJsonInt(jdata2, "id");
			serverWheelSpinInfo.start = GetLocalDateTime(GetJsonLong(jdata2, "start"));
			serverWheelSpinInfo.end = GetLocalDateTime(GetJsonLong(jdata2, "end"));
			serverWheelSpinInfo.param = GetJsonString(jdata2, "param");
			list.Add(serverWheelSpinInfo);
		}
		return list;
	}

	public static ServerWheelOptions AnalyzeWheelOptionsJson(JsonData jdata, string key)
	{
		JsonData jsonObject = GetJsonObject(jdata, key);
		if (jsonObject == null)
		{
			return null;
		}
		ServerWheelOptions wheelOptions = new ServerWheelOptions();
		AnalyzeWheelOptions_Items(jsonObject, ref wheelOptions);
		AnalyzeWheelOptions_Other(jsonObject, ref wheelOptions);
		return wheelOptions;
	}

	private static void AnalyzeWheelOptions_Items(JsonData jdata, ref ServerWheelOptions wheelOptions)
	{
		wheelOptions.m_itemWon = GetJsonInt(jdata, "itemWon");
		wheelOptions.ResetItemList();
		JsonData jsonArray = GetJsonArray(jdata, "itemList");
		if (jsonArray != null)
		{
			int count = jsonArray.Count;
			for (int i = 0; i < count; i++)
			{
				JsonData jdata2 = jsonArray[i];
				ServerItemState serverItemState = AnalyzeItemStateJson(jdata2, string.Empty);
				if (serverItemState != null)
				{
					wheelOptions.AddItemList(serverItemState);
				}
			}
		}
		JsonData jsonArray2 = GetJsonArray(jdata, "items");
		int count2 = jsonArray2.Count;
		for (int j = 0; j < count2; j++)
		{
			int jsonInt = GetJsonInt(jsonArray2[j]);
			wheelOptions.m_items[j] = jsonInt;
		}
		JsonData jsonArray3 = GetJsonArray(jdata, "item");
		int count3 = jsonArray3.Count;
		for (int k = 0; k < count3; k++)
		{
			int jsonInt2 = GetJsonInt(jsonArray3[k]);
			wheelOptions.m_itemQuantities[k] = jsonInt2;
		}
		JsonData jsonArray4 = GetJsonArray(jdata, "itemWeight");
		int count4 = jsonArray4.Count;
		for (int l = 0; l < count4; l++)
		{
			int jsonInt3 = GetJsonInt(jsonArray4[l]);
			wheelOptions.m_itemWeight[l] = jsonInt3;
		}
		if (wheelOptions.NumRequiredSpEggs > 0)
		{
			RouletteManager.Instance.specialEgg = RouletteManager.Instance.specialEgg + wheelOptions.NumRequiredSpEggs;
		}
	}

	private static void AnalyzeWheelOptions_Other(JsonData jdata, ref ServerWheelOptions wheelOptions)
	{
		long jsonLong = GetJsonLong(jdata, "nextFreeSpin");
		wheelOptions.m_nextFreeSpin = GetLocalDateTime(jsonLong);
		wheelOptions.m_spinCost = GetJsonInt(jdata, "spinCost");
		wheelOptions.m_rouletteRank = (RouletteUtility.WheelRank)GetJsonInt(jdata, "rouletteRank");
		wheelOptions.m_numRouletteToken = GetJsonInt(jdata, "numRouletteToken");
		wheelOptions.m_numJackpotRing = GetJsonInt(jdata, "numJackpotRing");
		wheelOptions.m_numRemaining = GetJsonInt(jdata, "numRemainingRoulette");
		if (IsExist(jdata, "campaignList"))
		{
			JsonData jsonArray = GetJsonArray(jdata, "campaignList");
			if (jsonArray != null)
			{
				int count = jsonArray.Count;
				for (int i = 0; i < count; i++)
				{
					ServerCampaignData serverCampaignData = AnalyzeCampaignDataJson(jsonArray[i], string.Empty);
					if (serverCampaignData != null)
					{
						ServerInterface.CampaignState.RegistCampaign(serverCampaignData);
					}
				}
			}
		}
		GeneralUtil.SetItemCount(ServerItem.Id.ROULLETE_TICKET_ITEM, wheelOptions.m_numRouletteToken);
	}

	public static ServerWheelOptionsGeneral AnalyzeWheelOptionsGeneralJson(JsonData jdata, string key)
	{
		JsonData jsonObject = GetJsonObject(jdata, key);
		if (jsonObject == null)
		{
			return null;
		}
		ServerWheelOptionsGeneral chaoWheelOptions = new ServerWheelOptionsGeneral();
		AnalyzeChaoWheelOptionsGeneral_Items(jsonObject, ref chaoWheelOptions);
		AnalyzeChaoWheelOptionsGeneral_Other(jsonObject, ref chaoWheelOptions);
		return chaoWheelOptions;
	}

	private static void AnalyzeChaoWheelOptionsGeneral_Items(JsonData jdata, ref ServerWheelOptionsGeneral chaoWheelOptions)
	{
		JsonData jsonArray = GetJsonArray(jdata, "items");
		JsonData jsonArray2 = GetJsonArray(jdata, "item");
		JsonData jsonArray3 = GetJsonArray(jdata, "itemWeight");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			int jsonInt = GetJsonInt(jsonArray[i]);
			int num = 1;
			int weight = 1;
			if (jsonArray2 != null && jsonArray2.Count > i)
			{
				num = GetJsonInt(jsonArray2[i]);
			}
			if (jsonArray3 != null && jsonArray3.Count > i)
			{
				weight = GetJsonInt(jsonArray3[i]);
			}
			chaoWheelOptions.SetupItem(i, jsonInt, weight, num);
		}
	}

	private static void AnalyzeChaoWheelOptionsGeneral_Other(JsonData jdata, ref ServerWheelOptionsGeneral wheelOptionsGeneral)
	{
		int jsonInt = GetJsonInt(jdata, "spinID");
		int jsonInt2 = GetJsonInt(jdata, "rouletteRank");
		int jsonInt3 = GetJsonInt(jdata, "numRemainingRoulette");
		int jsonInt4 = GetJsonInt(jdata, "numJackpotRing");
		int jsonInt5 = GetJsonInt(jdata, "numSpecialEgg");
		long jsonLong = GetJsonLong(jdata, "nextFreeSpin");
		if (RouletteManager.Instance != null)
		{
			RouletteManager.Instance.specialEgg = jsonInt5;
		}
		if (IsExist(jdata, "campaignList"))
		{
			JsonData jsonArray = GetJsonArray(jdata, "campaignList");
			if (jsonArray != null)
			{
				int count = jsonArray.Count;
				for (int i = 0; i < count; i++)
				{
					ServerCampaignData serverCampaignData = AnalyzeCampaignDataJson(jsonArray[i], string.Empty);
					if (serverCampaignData != null)
					{
						serverCampaignData.id = serverCampaignData.iSubContent;
						ServerInterface.CampaignState.RegistCampaign(serverCampaignData);
					}
				}
			}
		}
		wheelOptionsGeneral.SetupParam(jsonInt, jsonInt3, jsonInt4, jsonInt2, jsonInt5, GetLocalDateTime(jsonLong));
		wheelOptionsGeneral.ResetupCostItem();
		JsonData jsonArray2 = GetJsonArray(jdata, "costItemList");
		int count2 = jsonArray2.Count;
		for (int j = 0; j < count2; j++)
		{
			JsonData jdata2 = jsonArray2[j];
			int jsonInt6 = GetJsonInt(jdata2, "itemId");
			int jsonInt7 = GetJsonInt(jdata2, "numItem");
			int jsonInt8 = GetJsonInt(jdata2, "itemStock");
			if (jsonInt6 > 0)
			{
				wheelOptionsGeneral.AddCostItem(jsonInt6, jsonInt8, jsonInt7);
			}
		}
	}

	public static ServerChaoWheelOptions AnalyzeChaoWheelOptionsJson(JsonData jdata, string key)
	{
		JsonData jsonObject = GetJsonObject(jdata, key);
		if (jsonObject == null)
		{
			return null;
		}
		ServerChaoWheelOptions chaoWheelOptions = new ServerChaoWheelOptions();
		AnalyzeChaoWheelOptions_Items(jsonObject, ref chaoWheelOptions);
		AnalyzeChaoWheelOptions_Other(jsonObject, ref chaoWheelOptions);
		if (IsExist(jsonObject, "campaignList"))
		{
			JsonData jsonArray = GetJsonArray(jsonObject, "campaignList");
			if (jsonArray != null)
			{
				int count = jsonArray.Count;
				for (int i = 0; i < count; i++)
				{
					ServerCampaignData serverCampaignData = AnalyzeCampaignDataJson(jsonArray[i], string.Empty);
					if (serverCampaignData != null)
					{
						serverCampaignData.id = serverCampaignData.iSubContent;
						ServerInterface.CampaignState.RegistCampaign(serverCampaignData);
					}
				}
			}
		}
		return chaoWheelOptions;
	}

	private static void AnalyzeChaoWheelOptions_Items(JsonData jdata, ref ServerChaoWheelOptions chaoWheelOptions)
	{
		JsonData jsonArray = GetJsonArray(jdata, "rarity");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			chaoWheelOptions.Rarities[i] = GetJsonInt(jsonArray[i]);
		}
		JsonData jsonArray2 = GetJsonArray(jdata, "itemWeight");
		int count2 = jsonArray2.Count;
		for (int j = 0; j < count2; j++)
		{
			chaoWheelOptions.ItemWeight[j] = GetJsonInt(jsonArray2[j]);
		}
	}

	private static void AnalyzeChaoWheelOptions_Other(JsonData jdata, ref ServerChaoWheelOptions chaoWheelOptions)
	{
		chaoWheelOptions.Cost = GetJsonInt(jdata, "spinCost");
		chaoWheelOptions.SpinType = (ServerChaoWheelOptions.ChaoSpinType)GetJsonInt(jdata, "chaoRouletteType");
		chaoWheelOptions.NumSpecialEggs = GetJsonInt(jdata, "numSpecialEgg");
		chaoWheelOptions.IsValid = ((GetJsonInt(jdata, "rouletteAvailable") != 0) ? true : false);
		chaoWheelOptions.NumRouletteToken = GetJsonInt(jdata, "numChaoRouletteToken");
		chaoWheelOptions.IsTutorial = ((GetJsonInt(jdata, "numChaoRoulette") == 0) ? true : false);
		if (RouletteManager.Instance != null)
		{
			RouletteManager.Instance.specialEgg = chaoWheelOptions.NumSpecialEggs;
		}
		GeneralUtil.SetItemCount(ServerItem.Id.ROULLETE_TICKET_PREMIAM, chaoWheelOptions.NumRouletteToken);
	}

	public static ServerSpinResultGeneral AnalyzeSpinResultJson(JsonData jdata, string key)
	{
		JsonData jsonArray = GetJsonArray(jdata, key);
		ServerSpinResultGeneral serverSpinResultGeneral = null;
		if (jsonArray != null)
		{
			int count = jsonArray.Count;
			if (count > 0)
			{
				int itemWon = 0;
				serverSpinResultGeneral = new ServerSpinResultGeneral();
				for (int i = 0; i < count; i++)
				{
					JsonData jsonData = jsonArray[i];
					if (jsonData == null)
					{
						continue;
					}
					ServerChaoData serverChaoData = AnalyzeChaoDataJson(jsonData, "getChao");
					if (serverChaoData != null)
					{
						serverSpinResultGeneral.AddChaoState(serverChaoData);
					}
					else
					{
						ServerChaoData serverChaoData2 = AnalyzeItemDataJson(jsonData, "getItem");
						if (serverChaoData2 != null)
						{
							serverSpinResultGeneral.AddChaoState(serverChaoData2);
						}
					}
					JsonData jsonArray2 = GetJsonArray(jsonData, "itemList");
					int count2 = jsonArray2.Count;
					for (int j = 0; j < count2; j++)
					{
						JsonData jdata2 = jsonArray2[j];
						ServerItemState serverItemState = AnalyzeItemStateJson(jdata2, string.Empty);
						if (serverItemState != null)
						{
							serverSpinResultGeneral.AddItemState(serverItemState);
						}
					}
					itemWon = GetJsonInt(jsonData, "itemWon");
				}
				if (count > 1)
				{
					serverSpinResultGeneral.ItemWon = -1;
				}
				else
				{
					serverSpinResultGeneral.ItemWon = itemWon;
				}
			}
		}
		return serverSpinResultGeneral;
	}

	public static ServerSpinResultGeneral AnalyzeSpinResultGeneralJson(JsonData jdata, string key)
	{
		JsonData jsonData = jdata;
		if (key != null && string.Empty != key)
		{
			jsonData = GetJsonObject(jsonData, key);
		}
		if (jsonData == null)
		{
			return null;
		}
		ServerSpinResultGeneral serverSpinResultGeneral = new ServerSpinResultGeneral();
		JsonData jsonArray = GetJsonArray(jsonData, "getChao");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jdata2 = jsonArray[i];
			ServerChaoData serverChaoData = AnalyzeChaoDataJson(jdata2, string.Empty);
			if (serverChaoData != null)
			{
				serverSpinResultGeneral.AddChaoState(serverChaoData);
			}
		}
		JsonData jsonArray2 = GetJsonArray(jsonData, "itemList");
		int count2 = jsonArray2.Count;
		for (int j = 0; j < count2; j++)
		{
			JsonData jdata3 = jsonArray2[j];
			ServerItemState serverItemState = AnalyzeItemStateJson(jdata3, string.Empty);
			if (serverItemState != null)
			{
				serverSpinResultGeneral.AddItemState(serverItemState);
			}
		}
		serverSpinResultGeneral.ItemWon = GetJsonInt(jsonData, "itemWon");
		return serverSpinResultGeneral;
	}

	public static ServerChaoSpinResult AnalyzeChaoSpinResultJson(JsonData jdata, string key)
	{
		JsonData jsonData = jdata;
		if (key != null && string.Empty != key)
		{
			jsonData = GetJsonObject(jsonData, key);
		}
		if (jsonData == null)
		{
			return null;
		}
		ServerChaoSpinResult serverChaoSpinResult = new ServerChaoSpinResult();
		serverChaoSpinResult.AcquiredChaoData = AnalyzeChaoDataJson(jsonData, "getChao");
		serverChaoSpinResult.NumRequiredSpEggs = 0;
		JsonData jsonArray = GetJsonArray(jsonData, "itemList");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jdata2 = jsonArray[i];
			ServerItemState serverItemState = AnalyzeItemStateJson(jdata2, string.Empty);
			if (serverItemState != null)
			{
				serverChaoSpinResult.AddItemState(serverItemState);
				if (serverItemState.m_itemId == 220000)
				{
					serverChaoSpinResult.NumRequiredSpEggs += serverItemState.m_num;
				}
			}
		}
		serverChaoSpinResult.ItemWon = GetJsonInt(jsonData, "itemWon");
		return serverChaoSpinResult;
	}

	public static ServerChaoData AnalyzeChaoDataJson(JsonData jdata, string key)
	{
		JsonData jsonData = jdata;
		if (key != null && string.Empty != key)
		{
			jsonData = GetJsonObject(jsonData, key);
		}
		if (jsonData == null)
		{
			return null;
		}
		ServerChaoData serverChaoData = new ServerChaoData();
		serverChaoData.Id = GetJsonInt(jsonData, "chaoId");
		serverChaoData.Level = GetJsonInt(jsonData, "level");
		serverChaoData.Rarity = GetJsonInt(jsonData, "rarity");
		return serverChaoData;
	}

	public static ServerChaoData AnalyzeItemDataJson(JsonData jdata, string key)
	{
		JsonData jsonData = jdata;
		if (key != null && string.Empty != key)
		{
			jsonData = GetJsonObject(jsonData, key);
		}
		if (jsonData == null)
		{
			return null;
		}
		ServerChaoData serverChaoData = new ServerChaoData();
		serverChaoData.Id = GetJsonInt(jsonData, "itemId");
		serverChaoData.Level = GetJsonInt(jsonData, "level_after");
		serverChaoData.Rarity = GetJsonInt(jsonData, "rarity");
		return serverChaoData;
	}

	public static ServerChaoRentalState AnalyzeChaoRentalStateJson(JsonData jdata, string key)
	{
		JsonData jsonData = jdata;
		if (key != null && string.Empty != key)
		{
			jsonData = GetJsonObject(jsonData, key);
		}
		if (jsonData == null)
		{
			return null;
		}
		ServerChaoRentalState serverChaoRentalState = new ServerChaoRentalState();
		serverChaoRentalState.FriendId = GetJsonString(jsonData, "friendId");
		serverChaoRentalState.Name = GetJsonString(jsonData, "name");
		serverChaoRentalState.Url = GetJsonString(jsonData, "url");
		serverChaoRentalState.ChaoData = AnalyzeChaoDataJson(jsonData, "chaoData");
		serverChaoRentalState.RentalState = GetJsonInt(jsonData, "rentalFlg");
		serverChaoRentalState.NextRentalAt = GetJsonInt(jsonData, "nextRentalAt");
		return serverChaoRentalState;
	}

	public static ServerPrizeState AnalyzePrizeChaoWheelSpin(JsonData jdata)
	{
		JsonData jsonArray = GetJsonArray(jdata, "prizeList");
		if (jsonArray == null)
		{
			return null;
		}
		ServerPrizeState serverPrizeState = new ServerPrizeState();
		for (int i = 0; i < jsonArray.Count; i++)
		{
			ServerPrizeData serverPrizeData = new ServerPrizeData();
			serverPrizeData.itemId = GetJsonInt(jsonArray[i], "chao_id");
			serverPrizeData.num = 1;
			serverPrizeData.weight = 1;
			serverPrizeState.AddPrize(serverPrizeData);
		}
		return serverPrizeState;
	}

	public static ServerPrizeState AnalyzePrizeWheelSpinGeneral(JsonData jdata)
	{
		JsonData jsonArray = GetJsonArray(jdata, "raidbossPrizeList");
		if (jsonArray == null)
		{
			return null;
		}
		ServerPrizeState serverPrizeState = new ServerPrizeState();
		for (int i = 0; i < jsonArray.Count; i++)
		{
			ServerPrizeData serverPrizeData = new ServerPrizeData();
			serverPrizeData.itemId = GetJsonInt(jsonArray[i], "itemId");
			serverPrizeData.num = GetJsonInt(jsonArray[i], "numItem");
			serverPrizeData.weight = GetJsonInt(jsonArray[i], "itemRate");
			serverPrizeData.spinId = GetJsonInt(jsonArray[i], "spinId");
			serverPrizeState.AddPrize(serverPrizeData);
		}
		return serverPrizeState;
	}

	public static ServerLeaderboardEntry AnalyzeLeaderboardEntryJson(JsonData jdata, string key)
	{
		JsonData jsonData = jdata;
		if (key != null && string.Empty != key)
		{
			jsonData = GetJsonObject(jsonData, key);
		}
		if (jsonData == null)
		{
			return null;
		}
		ServerLeaderboardEntry serverLeaderboardEntry = new ServerLeaderboardEntry();
		serverLeaderboardEntry.m_hspId = GetJsonString(jsonData, "friendId");
		serverLeaderboardEntry.m_score = GetJsonLong(jsonData, "rankingScore");
		serverLeaderboardEntry.m_hiScore = GetJsonLong(jsonData, "maxScore");
		serverLeaderboardEntry.m_userData = GetJsonInt(jsonData, "userData");
		serverLeaderboardEntry.m_name = GetJsonString(jsonData, "name");
		serverLeaderboardEntry.m_url = GetJsonString(jsonData, "url");
		serverLeaderboardEntry.m_energyFlg = GetJsonFlag(jsonData, "energyFlg");
		serverLeaderboardEntry.m_grade = GetJsonInt(jsonData, "grade");
		serverLeaderboardEntry.m_gradeChanged = GetJsonInt(jsonData, "rankChanged");
		serverLeaderboardEntry.m_expireTime = GetJsonLong(jsonData, "expireTime");
		serverLeaderboardEntry.m_numRank = GetJsonInt(jsonData, "numRank");
		serverLeaderboardEntry.m_loginTime = GetJsonInt(jsonData, "loginTime");
		serverLeaderboardEntry.m_charaId = GetJsonInt(jsonData, "charaId");
		serverLeaderboardEntry.m_charaLevel = GetJsonInt(jsonData, "charaLevel");
		serverLeaderboardEntry.m_subCharaId = GetJsonInt(jsonData, "subCharaId");
		serverLeaderboardEntry.m_subCharaLevel = GetJsonInt(jsonData, "subCharaLevel");
		serverLeaderboardEntry.m_mainChaoId = GetJsonInt(jsonData, "mainChaoId");
		serverLeaderboardEntry.m_mainChaoLevel = GetJsonInt(jsonData, "mainChaoLevel");
		serverLeaderboardEntry.m_subChaoId = GetJsonInt(jsonData, "subChaoId");
		serverLeaderboardEntry.m_subChaoLevel = GetJsonInt(jsonData, "subChaoLevel");
		serverLeaderboardEntry.m_leagueIndex = GetJsonInt(jsonData, "league");
		serverLeaderboardEntry.m_language = (Env.Language)GetJsonInt(jsonData, "language");
		return serverLeaderboardEntry;
	}

	public static ServerMileageFriendEntry AnalyzeMileageFriendEntryJson(JsonData jdata, string key)
	{
		JsonData jsonData = jdata;
		if (key != null && string.Empty != key)
		{
			jsonData = GetJsonObject(jsonData, key);
		}
		if (jsonData == null)
		{
			return null;
		}
		ServerMileageFriendEntry serverMileageFriendEntry = new ServerMileageFriendEntry();
		serverMileageFriendEntry.m_friendId = GetJsonString(jsonData, "friendId");
		serverMileageFriendEntry.m_name = GetJsonString(jsonData, "name");
		serverMileageFriendEntry.m_url = GetJsonString(jsonData, "url");
		serverMileageFriendEntry.m_mapState = AnalyzeMileageMapStateJson(jsonData, "mapState");
		return serverMileageFriendEntry;
	}

	public static ServerMileageMapState AnalyzeMileageMapStateJson(JsonData jdata, string key)
	{
		JsonData jsonData = jdata;
		if (key != null && string.Empty != key)
		{
			jsonData = GetJsonObject(jsonData, key);
		}
		if (jsonData == null)
		{
			return null;
		}
		ServerMileageMapState serverMileageMapState = new ServerMileageMapState();
		serverMileageMapState.m_episode = GetJsonInt(jsonData, "episode");
		serverMileageMapState.m_chapter = GetJsonInt(jsonData, "chapter");
		serverMileageMapState.m_point = GetJsonInt(jsonData, "point");
		serverMileageMapState.m_numBossAttack = GetJsonInt(jsonData, "numBossAttack");
		serverMileageMapState.m_stageTotalScore = GetJsonLong(jsonData, "stageTotalScore");
		serverMileageMapState.m_stageMaxScore = GetJsonLong(jsonData, "stageMaxScore");
		long unixTime = GetJsonInt(jsonData, "chapterStartTime");
		serverMileageMapState.m_chapterStartTime = GetLocalDateTime(unixTime);
		return serverMileageMapState;
	}

	public static PresentItem AnalyzePresentItemJson(JsonData jdata, string key)
	{
		JsonData jsonData = jdata;
		if (key != null && string.Empty != key)
		{
			jsonData = GetJsonObject(jsonData, key);
		}
		if (jsonData == null)
		{
			return null;
		}
		PresentItem presentItem = new PresentItem();
		presentItem.m_itemId = GetJsonInt(jsonData, "itemId");
		presentItem.m_numItem = GetJsonInt(jsonData, "numItem");
		presentItem.m_additionalInfo1 = GetJsonInt(jsonData, "additionalInfo1");
		presentItem.m_additionalInfo1 = GetJsonInt(jsonData, "additionalInfo2");
		return presentItem;
	}

	public static ServerMessageEntry AnalyzeMessageEntryJson(JsonData jdata, string key)
	{
		JsonData jsonData = jdata;
		if (key != null && string.Empty != key)
		{
			jsonData = GetJsonObject(jsonData, key);
		}
		if (jsonData == null)
		{
			return null;
		}
		ServerMessageEntry serverMessageEntry = new ServerMessageEntry();
		serverMessageEntry.m_messageId = GetJsonInt(jsonData, "messageId");
		serverMessageEntry.m_messageType = (ServerMessageEntry.MessageType)GetJsonInt(jsonData, "messageType");
		serverMessageEntry.m_fromId = GetJsonString(jsonData, "friendId");
		serverMessageEntry.m_name = GetJsonString(jsonData, "name");
		serverMessageEntry.m_url = GetJsonString(jsonData, "url");
		serverMessageEntry.m_presentState = AnalyzePresentStateJson(jsonData, "item");
		serverMessageEntry.m_expireTiem = GetJsonInt(jsonData, "expireTime");
		return serverMessageEntry;
	}

	public static ServerOperatorMessageEntry AnalyzeOperatorMessageEntryJson(JsonData jdata, string key)
	{
		JsonData jsonData = jdata;
		if (key != null && string.Empty != key)
		{
			jsonData = GetJsonObject(jsonData, key);
		}
		if (jsonData == null)
		{
			return null;
		}
		ServerOperatorMessageEntry serverOperatorMessageEntry = new ServerOperatorMessageEntry();
		serverOperatorMessageEntry.m_messageId = GetJsonInt(jsonData, "messageId");
		serverOperatorMessageEntry.m_content = GetJsonString(jsonData, "contents");
		serverOperatorMessageEntry.m_presentState = AnalyzePresentStateJson(jsonData, "item");
		serverOperatorMessageEntry.m_expireTiem = GetJsonInt(jsonData, "expireTime");
		return serverOperatorMessageEntry;
	}

	public static ServerItemState AnalyzeItemStateJson(JsonData jdata, string key)
	{
		JsonData jsonData = jdata;
		if (key != null && string.Empty != key)
		{
			jsonData = GetJsonObject(jsonData, key);
		}
		if (jsonData == null)
		{
			return null;
		}
		ServerItemState serverItemState = new ServerItemState();
		serverItemState.m_itemId = GetJsonInt(jsonData, "itemId");
		serverItemState.m_num = GetJsonInt(jsonData, "numItem");
		return serverItemState;
	}

	public static ServerPresentState AnalyzePresentStateJson(JsonData jdata, string key)
	{
		JsonData jsonData = jdata;
		if (key != null && string.Empty != key)
		{
			jsonData = GetJsonObject(jsonData, key);
		}
		if (jsonData == null)
		{
			return null;
		}
		ServerPresentState serverPresentState = new ServerPresentState();
		serverPresentState.m_itemId = GetJsonInt(jsonData, "itemId");
		serverPresentState.m_numItem = GetJsonInt(jsonData, "numItem");
		serverPresentState.m_additionalInfo1 = GetJsonInt(jsonData, "additionalInfo1");
		serverPresentState.m_additionalInfo2 = GetJsonInt(jsonData, "additionalInfo2");
		return serverPresentState;
	}

	public static ServerRedStarItemState AnalyzeRedStarItemStateJson(JsonData jdata, string key)
	{
		JsonData jsonData = jdata;
		if (key != null && string.Empty != key)
		{
			jsonData = GetJsonObject(jsonData, key);
		}
		if (jsonData == null)
		{
			return null;
		}
		ServerRedStarItemState serverRedStarItemState = new ServerRedStarItemState();
		serverRedStarItemState.m_storeItemId = GetJsonInt(jsonData, "storeItemId");
		serverRedStarItemState.m_itemId = GetJsonInt(jsonData, "itemId");
		serverRedStarItemState.m_numItem = GetJsonInt(jsonData, "numItem");
		serverRedStarItemState.m_price = GetJsonInt(jsonData, "price");
		serverRedStarItemState.m_priceDisp = GetJsonString(jsonData, "priceDisp");
		serverRedStarItemState.m_productId = GetJsonString(jsonData, "productId");
		if (serverRedStarItemState.m_itemId == 900000)
		{
			ServerCampaignData serverCampaignData = new ServerCampaignData();
			serverCampaignData.id = serverRedStarItemState.m_storeItemId;
			serverCampaignData.campaignType = Constants.Campaign.emType.PurchaseAddRsrings;
			ServerInterface.CampaignState.RemoveCampaign(serverCampaignData);
			serverCampaignData.campaignType = Constants.Campaign.emType.PurchaseAddRsringsNoChargeUser;
			ServerInterface.CampaignState.RemoveCampaign(serverCampaignData);
		}
		if (IsExist(jsonData, "campaign"))
		{
			ServerCampaignData serverCampaignData2 = AnalyzeCampaignDataJson(jsonData, "campaign");
			if (serverCampaignData2 != null)
			{
				serverCampaignData2.id = serverRedStarItemState.m_storeItemId;
				ServerInterface.CampaignState.RegistCampaign(serverCampaignData2);
			}
		}
		return serverRedStarItemState;
	}

	public static ServerRingItemState AnalyzeRingItemStateJson(JsonData jdata, string key)
	{
		JsonData jsonData = jdata;
		if (key != null && string.Empty != key)
		{
			jsonData = GetJsonObject(jsonData, key);
		}
		if (jsonData == null)
		{
			return null;
		}
		ServerRingItemState serverRingItemState = new ServerRingItemState();
		serverRingItemState.m_itemId = GetJsonInt(jsonData, "item_id");
		serverRingItemState.m_cost = GetJsonInt(jsonData, "price");
		if (IsExist(jsonData, "campaign"))
		{
			ServerCampaignData serverCampaignData = AnalyzeCampaignDataJson(jsonData, "campaign");
			if (serverCampaignData != null)
			{
				serverCampaignData.id = serverRingItemState.m_itemId;
				ServerInterface.CampaignState.RegistCampaign(serverCampaignData);
			}
		}
		return serverRingItemState;
	}

	public static ServerMileageIncentive AnalyzeMileageIncentiveJson(JsonData jdata, string key)
	{
		JsonData jsonData = jdata;
		if (key != null && string.Empty != key)
		{
			jsonData = GetJsonObject(jsonData, key);
		}
		if (jsonData == null)
		{
			return null;
		}
		ServerMileageIncentive serverMileageIncentive = new ServerMileageIncentive();
		serverMileageIncentive.m_type = (ServerMileageIncentive.Type)GetJsonInt(jsonData, "type");
		serverMileageIncentive.m_itemId = GetJsonInt(jsonData, "itemId");
		serverMileageIncentive.m_num = GetJsonInt(jsonData, "numItem");
		serverMileageIncentive.m_pointId = GetJsonInt(jsonData, "pointId");
		if (jsonData.ContainsKey("friendId"))
		{
			serverMileageIncentive.m_friendId = GetJsonString(jsonData, "friendId");
		}
		return serverMileageIncentive;
	}

	public static ServerMileageEvent AnalyzeMileageEventJson(JsonData jdata, string key)
	{
		JsonData jsonData = jdata;
		if (key != null && string.Empty != key)
		{
			jsonData = GetJsonObject(jsonData, key);
		}
		if (jsonData == null)
		{
			return null;
		}
		ServerMileageEvent serverMileageEvent = new ServerMileageEvent();
		serverMileageEvent.Distance = GetJsonInt(jsonData, "distance");
		serverMileageEvent.EventType = (ServerMileageEvent.emEventType)GetJsonInt(jsonData, "eventType");
		serverMileageEvent.Content = GetJsonInt(jsonData, "content");
		serverMileageEvent.NumType = (ServerConstants.NumType)GetJsonInt(jsonData, "numType");
		serverMileageEvent.Num = GetJsonInt(jsonData, "numContent");
		serverMileageEvent.Level = GetJsonInt(jsonData, "level");
		return serverMileageEvent;
	}

	public static List<ServerMileageFriendEntry> AnalyzeMileageFriendListJson(JsonData jdata, string key)
	{
		List<ServerMileageFriendEntry> list = new List<ServerMileageFriendEntry>();
		JsonData jsonArray = GetJsonArray(jdata, key);
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			ServerMileageFriendEntry item = AnalyzeMileageFriendEntryJson(jsonArray[i], string.Empty);
			list.Add(item);
		}
		return list;
	}

	public static ServerDailyChallengeIncentive AnalyzeDailyMissionIncentiveJson(JsonData jdata, string key)
	{
		JsonData jsonData = jdata;
		if (key != null && string.Empty != key)
		{
			jsonData = GetJsonObject(jsonData, key);
		}
		if (jsonData == null)
		{
			return null;
		}
		ServerDailyChallengeIncentive serverDailyChallengeIncentive = new ServerDailyChallengeIncentive();
		serverDailyChallengeIncentive.m_itemId = GetJsonInt(jsonData, "itemId");
		serverDailyChallengeIncentive.m_num = GetJsonInt(jsonData, "numItem");
		serverDailyChallengeIncentive.m_numIncentiveCont = GetJsonInt(jsonData, "numIncentiveCont");
		return serverDailyChallengeIncentive;
	}

	public static ServerCampaignData AnalyzeCampaignDataJson(JsonData jdata, string key)
	{
		JsonData jsonData = jdata;
		if (key != null && string.Empty != key)
		{
			jsonData = GetJsonObject(jsonData, key);
		}
		if (jsonData == null)
		{
			return null;
		}
		ServerCampaignData serverCampaignData = new ServerCampaignData();
		serverCampaignData.campaignType = (Constants.Campaign.emType)GetJsonInt(jsonData, "campaignType");
		serverCampaignData.iContent = GetJsonInt(jsonData, "campaignContent");
		serverCampaignData.iSubContent = GetJsonInt(jsonData, "campaignSubContent");
		serverCampaignData.beginDate = GetJsonLong(jsonData, "campaignStartTime");
		serverCampaignData.endDate = GetJsonLong(jsonData, "campaignEndTime");
		return serverCampaignData;
	}

	public static List<ServerRingExchangeList> AnalyzeRingExchangeList(JsonData jdata)
	{
		if (!IsExist(jdata, "itemList"))
		{
			return null;
		}
		List<ServerRingExchangeList> list = new List<ServerRingExchangeList>();
		JsonData jsonArray = GetJsonArray(jdata, "itemList");
		for (int i = 0; i < jsonArray.Count; i++)
		{
			ServerRingExchangeList serverRingExchangeList = new ServerRingExchangeList();
			serverRingExchangeList.m_ringItemId = GetJsonInt(jsonArray[i], "ringItemId");
			serverRingExchangeList.m_itemId = GetJsonInt(jsonArray[i], "itemId");
			serverRingExchangeList.m_itemNum = GetJsonInt(jsonArray[i], "numItem");
			serverRingExchangeList.m_price = GetJsonInt(jsonArray[i], "price");
			if (IsExist(jsonArray[i], "campaign"))
			{
				ServerCampaignData serverCampaignData = AnalyzeCampaignDataJson(jsonArray[i], "campaign");
				if (serverCampaignData != null)
				{
					serverCampaignData.id = serverRingExchangeList.m_itemId;
					ServerInterface.CampaignState.RegistCampaign(serverCampaignData);
				}
			}
			list.Add(serverRingExchangeList);
		}
		return list;
	}

	public static int AnalyzeRingExchangeListTotalItems(JsonData jdata)
	{
		JsonData jsonObject = GetJsonObject(jdata, "totalItems");
		if (jsonObject == null)
		{
			return 0;
		}
		return GetJsonInt(jdata, "totalItems");
	}

	public static ServerLeagueData AnalyzeLeagueData(JsonData jdata, string key)
	{
		JsonData jsonData = jdata;
		if (key != null && string.Empty != key)
		{
			jsonData = GetJsonObject(jsonData, key);
		}
		if (jsonData == null)
		{
			return null;
		}
		ServerLeagueData serverLeagueData = new ServerLeagueData();
		serverLeagueData.mode = GetJsonInt(jdata, "mode");
		serverLeagueData.leagueId = GetJsonInt(jsonData, "leagueId");
		serverLeagueData.groupId = GetJsonInt(jsonData, "groupId");
		serverLeagueData.numGroupMember = GetJsonInt(jsonData, "numGroupMember");
		serverLeagueData.numLeagueMember = GetJsonInt(jsonData, "numLeagueMember");
		serverLeagueData.numUp = GetJsonInt(jsonData, "numUp");
		serverLeagueData.numDown = GetJsonInt(jsonData, "numDown");
		JsonData jsonArray = GetJsonArray(jsonData, "highScoreOpe");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jdata2 = jsonArray[i];
			ServerRemainOperator remainOperator = AnalyzeRemainOperator(jdata2);
			serverLeagueData.AddHighScoreRemainOperator(remainOperator);
		}
		JsonData jsonArray2 = GetJsonArray(jsonData, "totalScoreOpe");
		int count2 = jsonArray2.Count;
		for (int j = 0; j < count2; j++)
		{
			JsonData jdata3 = jsonArray2[j];
			ServerRemainOperator remainOperator2 = AnalyzeRemainOperator(jdata3);
			serverLeagueData.AddTotalScoreRemainOperator(remainOperator2);
		}
		return serverLeagueData;
	}

	public static ServerWeeklyLeaderboardOptions AnalyzeWeeklyLeaderboardOptions(JsonData jdata)
	{
		ServerWeeklyLeaderboardOptions serverWeeklyLeaderboardOptions = new ServerWeeklyLeaderboardOptions();
		serverWeeklyLeaderboardOptions.mode = GetJsonInt(jdata, "mode");
		serverWeeklyLeaderboardOptions.type = GetJsonInt(jdata, "type");
		serverWeeklyLeaderboardOptions.param = GetJsonInt(jdata, "param");
		serverWeeklyLeaderboardOptions.startTime = GetJsonInt(jdata, "startTime");
		serverWeeklyLeaderboardOptions.endTime = GetJsonInt(jdata, "endTime");
		return serverWeeklyLeaderboardOptions;
	}

	public static List<ServerLeagueOperatorData> AnalyzeLeagueDatas(JsonData jdata, string key)
	{
		List<ServerLeagueOperatorData> list = new List<ServerLeagueOperatorData>();
		JsonData jsonArray = GetJsonArray(jdata, key);
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jdata2 = jsonArray[i];
			ServerLeagueOperatorData serverLeagueOperatorData = new ServerLeagueOperatorData();
			serverLeagueOperatorData.leagueId = GetJsonInt(jdata2, "leagueId");
			serverLeagueOperatorData.numUp = GetJsonInt(jdata2, "numUp");
			serverLeagueOperatorData.numDown = GetJsonInt(jdata2, "numDown");
			JsonData jsonArray2 = GetJsonArray(jdata2, "highScoreOpe");
			int count2 = jsonArray2.Count;
			for (int j = 0; j < count2; j++)
			{
				JsonData jdata3 = jsonArray2[j];
				ServerRemainOperator remainOperator = AnalyzeRemainOperator(jdata3);
				serverLeagueOperatorData.AddHighScoreRemainOperator(remainOperator);
			}
			JsonData jsonArray3 = GetJsonArray(jdata2, "totalScoreOpe");
			int count3 = jsonArray3.Count;
			for (int k = 0; k < count3; k++)
			{
				JsonData jdata4 = jsonArray3[k];
				ServerRemainOperator remainOperator2 = AnalyzeRemainOperator(jdata4);
				serverLeagueOperatorData.AddTotalScoreRemainOperator(remainOperator2);
			}
			list.Add(serverLeagueOperatorData);
		}
		return list;
	}

	private static ServerRemainOperator AnalyzeRemainOperator(JsonData jdata)
	{
		ServerRemainOperator serverRemainOperator = new ServerRemainOperator();
		serverRemainOperator.operatorData = GetJsonInt(jdata, "operator");
		serverRemainOperator.number = GetJsonInt(jdata, "number");
		JsonData jsonArray = GetJsonArray(jdata, "presentList");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jdata2 = jsonArray[i];
			ServerItemState itemState = AnalyzeItemStateJson(jdata2, string.Empty);
			serverRemainOperator.AddItemState(itemState);
		}
		return serverRemainOperator;
	}

	public static void GetResponse_CampaignList(JsonData jdata)
	{
		if (!IsExist(jdata, "campaignList"))
		{
			return;
		}
		JsonData jsonArray = GetJsonArray(jdata, "campaignList");
		if (jsonArray == null)
		{
			return;
		}
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			ServerCampaignData serverCampaignData = AnalyzeCampaignDataJson(jsonArray[i], string.Empty);
			if (serverCampaignData != null)
			{
				ServerInterface.CampaignState.RegistCampaign(serverCampaignData);
			}
		}
	}

	public static ServerFreeItemState AnalyzeFreeItemList(JsonData jdata)
	{
		ServerFreeItemState serverFreeItemState = new ServerFreeItemState();
		if (IsExist(jdata, "freeItemList"))
		{
			JsonData jsonArray = GetJsonArray(jdata, "freeItemList");
			if (jsonArray != null)
			{
				int count = jsonArray.Count;
				for (int i = 0; i < count; i++)
				{
					ServerItemState serverItemState = new ServerItemState();
					serverItemState.m_itemId = GetJsonInt(jsonArray[i], "itemId");
					serverItemState.m_num = GetJsonInt(jsonArray[i], "numItem");
					serverFreeItemState.AddItem(serverItemState);
				}
			}
		}
		return serverFreeItemState;
	}

	public static List<ServerConsumedCostData> AnalyzeConsumedCostDataList(JsonData jdata)
	{
		if (!IsExist(jdata, "consumedCostList"))
		{
			return null;
		}
		List<ServerConsumedCostData> list = new List<ServerConsumedCostData>();
		JsonData jsonArray = GetJsonArray(jdata, "consumedCostList");
		for (int i = 0; i < jsonArray.Count; i++)
		{
			ServerConsumedCostData serverConsumedCostData = new ServerConsumedCostData();
			serverConsumedCostData.consumedItemId = GetJsonInt(jsonArray[i], "consumedItemId");
			serverConsumedCostData.itemId = GetJsonInt(jsonArray[i], "itemId");
			serverConsumedCostData.numItem = GetJsonInt(jsonArray[i], "numItem");
			list.Add(serverConsumedCostData);
		}
		return list;
	}

	public static List<ServerEventEntry> AnalyzeEventList(JsonData jdata)
	{
		if (!IsExist(jdata, "eventList"))
		{
			return null;
		}
		JsonData jsonArray = GetJsonArray(jdata, "eventList");
		if (jsonArray == null)
		{
			return null;
		}
		List<ServerEventEntry> list = new List<ServerEventEntry>();
		for (int i = 0; i < jsonArray.Count; i++)
		{
			ServerEventEntry serverEventEntry = new ServerEventEntry();
			serverEventEntry.EventId = GetJsonInt(jsonArray[i], "eventId");
			serverEventEntry.EventType = GetJsonInt(jsonArray[i], "eventType");
			serverEventEntry.EventStartTime = GetLocalDateTime(GetJsonLong(jsonArray[i], "eventStartTime"));
			serverEventEntry.EventEndTime = GetLocalDateTime(GetJsonLong(jsonArray[i], "eventEndTime"));
			serverEventEntry.EventCloseTime = GetLocalDateTime(GetJsonLong(jsonArray[i], "eventCloseTime"));
			list.Add(serverEventEntry);
		}
		return list;
	}

	public static ServerEventState AnalyzeEventState(JsonData jdata)
	{
		if (!IsExist(jdata, "eventState"))
		{
			return null;
		}
		JsonData jsonObject = GetJsonObject(jdata, "eventState");
		if (jsonObject == null)
		{
			return null;
		}
		ServerEventState serverEventState = new ServerEventState();
		serverEventState.Param = GetJsonLong(jsonObject, "param");
		serverEventState.RewardId = GetJsonInt(jsonObject, "rewardId");
		return serverEventState;
	}

	public static List<ServerEventReward> AnalyzeEventReward(JsonData jdata)
	{
		if (!IsExist(jdata, "eventRewardList"))
		{
			return null;
		}
		JsonData jsonArray = GetJsonArray(jdata, "eventRewardList");
		if (jsonArray == null)
		{
			return null;
		}
		List<ServerEventReward> list = new List<ServerEventReward>();
		for (int i = 0; i < jsonArray.Count; i++)
		{
			ServerEventReward serverEventReward = new ServerEventReward();
			serverEventReward.RewardId = GetJsonInt(jsonArray[i], "rewardId");
			serverEventReward.Param = GetJsonLong(jsonArray[i], "param");
			serverEventReward.m_itemId = GetJsonInt(jsonArray[i], "itemId");
			serverEventReward.m_num = GetJsonInt(jsonArray[i], "numItem");
			list.Add(serverEventReward);
		}
		return list;
	}

	public static ServerEventRaidBossState AnalyzeRaidBossState(JsonData jdata)
	{
		if (!IsExist(jdata, "eventRaidboss"))
		{
			return null;
		}
		JsonData jsonObject = GetJsonObject(jdata, "eventRaidboss");
		if (jsonObject == null)
		{
			return null;
		}
		ServerEventRaidBossState serverEventRaidBossState = new ServerEventRaidBossState();
		serverEventRaidBossState.Id = GetJsonLong(jsonObject, "raidbossId");
		serverEventRaidBossState.Level = GetJsonInt(jsonObject, "raidbossLevel");
		serverEventRaidBossState.Rarity = GetJsonInt(jsonObject, "raidbossRarity");
		serverEventRaidBossState.HitPoint = GetJsonInt(jsonObject, "raidbossHitPoint");
		serverEventRaidBossState.MaxHitPoint = GetJsonInt(jsonObject, "raidbossMaxHitPoint");
		serverEventRaidBossState.Status = GetJsonInt(jsonObject, "raidbossStatus");
		serverEventRaidBossState.EscapeAt = GetLocalDateTime(GetJsonLong(jsonObject, "raidbossEscapeAt"));
		serverEventRaidBossState.EncounterName = GetJsonString(jsonObject, "encounterName");
		serverEventRaidBossState.Encounter = GetJsonBoolean(jsonObject, "encounterFlg");
		serverEventRaidBossState.Crowded = GetJsonBoolean(jsonObject, "crowdedFlg");
		serverEventRaidBossState.Participation = (GetJsonInt(jsonObject, "participateCount") != 0);
		return serverEventRaidBossState;
	}

	public static List<ServerEventRaidBossState> AnalyzeRaidBossStateList(JsonData jdata)
	{
		if (!IsExist(jdata, "eventUserRaidbossList"))
		{
			return null;
		}
		JsonData jsonArray = GetJsonArray(jdata, "eventUserRaidbossList");
		if (jsonArray == null)
		{
			return null;
		}
		List<ServerEventRaidBossState> list = new List<ServerEventRaidBossState>();
		for (int i = 0; i < jsonArray.Count; i++)
		{
			ServerEventRaidBossState serverEventRaidBossState = new ServerEventRaidBossState();
			serverEventRaidBossState.Id = GetJsonLong(jsonArray[i], "raidbossId");
			serverEventRaidBossState.Level = GetJsonInt(jsonArray[i], "raidbossLevel");
			serverEventRaidBossState.Rarity = GetJsonInt(jsonArray[i], "raidbossRarity");
			serverEventRaidBossState.HitPoint = GetJsonInt(jsonArray[i], "raidbossHitPoint");
			serverEventRaidBossState.MaxHitPoint = GetJsonInt(jsonArray[i], "raidbossMaxHitPoint");
			serverEventRaidBossState.Status = GetJsonInt(jsonArray[i], "raidbossStatus");
			serverEventRaidBossState.EscapeAt = GetLocalDateTime(GetJsonLong(jsonArray[i], "raidbossEscapeAt"));
			serverEventRaidBossState.EncounterName = GetJsonString(jsonArray[i], "encounterName");
			serverEventRaidBossState.Encounter = GetJsonBoolean(jsonArray[i], "encounterFlg");
			serverEventRaidBossState.Crowded = GetJsonBoolean(jsonArray[i], "crowdedFlg");
			serverEventRaidBossState.Participation = (GetJsonInt(jsonArray[i], "participateCount") != 0);
			list.Add(serverEventRaidBossState);
		}
		return list;
	}

	public static ServerEventUserRaidBossState AnalyzeEventUserRaidBossState(JsonData jdata)
	{
		if (!IsExist(jdata, "eventUserRaidboss"))
		{
			return null;
		}
		JsonData jsonObject = GetJsonObject(jdata, "eventUserRaidboss");
		if (jsonObject == null)
		{
			return null;
		}
		ServerEventUserRaidBossState serverEventUserRaidBossState = new ServerEventUserRaidBossState();
		int num = 0;
		int num2 = 0;
		serverEventUserRaidBossState.NumRaidbossRings = GetJsonInt(jsonObject, "numRaidbossRings");
		num = GetJsonInt(jsonObject, "raidbossEnergy");
		num2 = GetJsonInt(jsonObject, "raidbossEnergyBuy");
		serverEventUserRaidBossState.NumBeatedEncounter = GetJsonInt(jsonObject, "numBeatedEncounter");
		serverEventUserRaidBossState.NumBeatedEnterprise = GetJsonInt(jsonObject, "numBeatedEnterprise");
		serverEventUserRaidBossState.NumRaidBossEncountered = GetJsonInt(jsonObject, "numTotalEncountered");
		DateTime atTime = GetLocalDateTime(GetJsonLong(jsonObject, "raidbossEnergyRenewsAt"));
		RaidEnergyStorage.CheckEnergy(ref num, ref num2, ref atTime);
		serverEventUserRaidBossState.RaidBossEnergy = num;
		serverEventUserRaidBossState.RaidbossEnergyBuy = num2;
		serverEventUserRaidBossState.EnergyRenewsAt = atTime;
		return serverEventUserRaidBossState;
	}

	public static List<ServerEventRaidBossUserState> AnalyzeEventRaidBossUserStateList(JsonData jdata)
	{
		if (!IsExist(jdata, "eventRaidbossUserList"))
		{
			return null;
		}
		JsonData jsonArray = GetJsonArray(jdata, "eventRaidbossUserList");
		if (jsonArray == null)
		{
			return null;
		}
		List<ServerEventRaidBossUserState> list = new List<ServerEventRaidBossUserState>();
		for (int i = 0; i < jsonArray.Count; i++)
		{
			ServerEventRaidBossUserState serverEventRaidBossUserState = new ServerEventRaidBossUserState();
			serverEventRaidBossUserState.WrestleId = GetJsonString(jsonArray[i], "wrestleId");
			serverEventRaidBossUserState.UserName = GetJsonString(jsonArray[i], "name");
			serverEventRaidBossUserState.Grade = GetJsonInt(jsonArray[i], "grade");
			serverEventRaidBossUserState.NumRank = GetJsonInt(jsonArray[i], "numRank");
			serverEventRaidBossUserState.LoginTime = GetJsonInt(jsonArray[i], "loginTime");
			serverEventRaidBossUserState.CharaId = GetJsonInt(jsonArray[i], "charaId");
			serverEventRaidBossUserState.CharaLevel = GetJsonInt(jsonArray[i], "charaLevel");
			serverEventRaidBossUserState.SubCharaId = GetJsonInt(jsonArray[i], "subCharaId");
			serverEventRaidBossUserState.SubCharaLevel = GetJsonInt(jsonArray[i], "subCharaLevel");
			serverEventRaidBossUserState.MainChaoId = GetJsonInt(jsonArray[i], "mainChaoId");
			serverEventRaidBossUserState.MainChaoLevel = GetJsonInt(jsonArray[i], "mainChaoLevel");
			serverEventRaidBossUserState.SubChaoId = GetJsonInt(jsonArray[i], "subChaoId");
			serverEventRaidBossUserState.SubChaoLevel = GetJsonInt(jsonArray[i], "subChaoLevel");
			serverEventRaidBossUserState.Language = GetJsonInt(jsonArray[i], "language");
			serverEventRaidBossUserState.League = GetJsonInt(jsonArray[i], "league");
			serverEventRaidBossUserState.WrestleCount = GetJsonInt(jsonArray[i], "wrestleCount");
			serverEventRaidBossUserState.WrestleDamage = GetJsonInt(jsonArray[i], "wrestleDamage");
			serverEventRaidBossUserState.WrestleBeatFlg = GetJsonBoolean(jsonArray[i], "wrestleBeatFlg");
			list.Add(serverEventRaidBossUserState);
		}
		return list;
	}

	public static List<ServerEventRaidBossDesiredState> AnalyzeEventRaidbossDesiredList(JsonData jdata)
	{
		if (!IsExist(jdata, "eventRaidbossDesiredList"))
		{
			return null;
		}
		JsonData jsonArray = GetJsonArray(jdata, "eventRaidbossDesiredList");
		if (jsonArray == null)
		{
			return null;
		}
		List<ServerEventRaidBossDesiredState> list = new List<ServerEventRaidBossDesiredState>();
		for (int i = 0; i < jsonArray.Count; i++)
		{
			ServerEventRaidBossDesiredState serverEventRaidBossDesiredState = new ServerEventRaidBossDesiredState();
			serverEventRaidBossDesiredState.DesireId = GetJsonString(jsonArray[i], "desireId");
			serverEventRaidBossDesiredState.UserName = GetJsonString(jsonArray[i], "name");
			serverEventRaidBossDesiredState.NumRank = GetJsonInt(jsonArray[i], "numRank");
			serverEventRaidBossDesiredState.LoginTime = GetJsonInt(jsonArray[i], "loginTime");
			serverEventRaidBossDesiredState.CharaId = GetJsonInt(jsonArray[i], "charaId");
			serverEventRaidBossDesiredState.CharaLevel = GetJsonInt(jsonArray[i], "charaLevel");
			serverEventRaidBossDesiredState.SubCharaId = GetJsonInt(jsonArray[i], "subCharaId");
			serverEventRaidBossDesiredState.SubCharaLevel = GetJsonInt(jsonArray[i], "subCharaLevel");
			serverEventRaidBossDesiredState.MainChaoId = GetJsonInt(jsonArray[i], "mainChaoId");
			serverEventRaidBossDesiredState.MainChaoLevel = GetJsonInt(jsonArray[i], "mainChaoLevel");
			serverEventRaidBossDesiredState.SubChaoId = GetJsonInt(jsonArray[i], "subChaoId");
			serverEventRaidBossDesiredState.SubChaoLevel = GetJsonInt(jsonArray[i], "subChaoLevel");
			serverEventRaidBossDesiredState.Language = GetJsonInt(jsonArray[i], "language");
			serverEventRaidBossDesiredState.League = GetJsonInt(jsonArray[i], "league");
			serverEventRaidBossDesiredState.NumBeatedEnterprise = GetJsonInt(jsonArray[i], "numBeatedEnterprise");
			list.Add(serverEventRaidBossDesiredState);
		}
		return list;
	}

	public static ServerEventRaidBossBonus AnalyzeEventRaidBossBonus(JsonData jdata)
	{
		if (!IsExist(jdata, "eventRaidbossBonus"))
		{
			return null;
		}
		JsonData jsonObject = GetJsonObject(jdata, "eventRaidbossBonus");
		if (jsonObject == null)
		{
			return null;
		}
		ServerEventRaidBossBonus serverEventRaidBossBonus = new ServerEventRaidBossBonus();
		serverEventRaidBossBonus.EncounterBonus = GetJsonInt(jsonObject, "eventRaidbossEncounterBonus");
		serverEventRaidBossBonus.WrestleBonus = GetJsonInt(jsonObject, "eventRaidbossWrestleBonus");
		serverEventRaidBossBonus.DamageRateBonus = GetJsonInt(jsonObject, "eventRaidbossDamageRateBonus");
		serverEventRaidBossBonus.DamageTopBonus = GetJsonInt(jsonObject, "eventRaidbossDamageTopBonus");
		serverEventRaidBossBonus.BeatBonus = GetJsonInt(jsonObject, "eventRaidbossBeatBonus");
		return serverEventRaidBossBonus;
	}

	public static List<ServerUserTransformData> AnalyzeUserTransformData(JsonData jdata)
	{
		if (!IsExist(jdata, "transformDataList"))
		{
			return null;
		}
		JsonData jsonArray = GetJsonArray(jdata, "transformDataList");
		if (jsonArray == null)
		{
			return null;
		}
		List<ServerUserTransformData> list = new List<ServerUserTransformData>();
		for (int i = 0; i < jsonArray.Count; i++)
		{
			ServerUserTransformData serverUserTransformData = new ServerUserTransformData();
			serverUserTransformData.m_userId = GetJsonString(jsonArray[i], "userId");
			serverUserTransformData.m_facebookId = GetJsonString(jsonArray[i], "facebookId");
			list.Add(serverUserTransformData);
		}
		return list;
	}

	public static DateTime GetLocalDateTime(long unixTime)
	{
		return UNIX_EPOCH.AddSeconds(unixTime).ToLocalTime();
	}

	public static long GetUnixTime(DateTime dateTime)
	{
		dateTime = dateTime.ToUniversalTime();
		return (long)(dateTime - UNIX_EPOCH).TotalSeconds;
	}

	public static int GetCurrentUnixTime()
	{
		DateTime utcNow = DateTime.UtcNow;
		return (int)(utcNow - UNIX_EPOCH).TotalSeconds;
	}

	public static DateTime GetCurrentTime()
	{
		return NetBase.GetCurrentTime();
	}

	public static bool IsServerTimeWithinPeriod(long start, long end)
	{
		return IsWithinPeriod(NetBase.LastNetServerTime, start, end);
	}

	public static bool IsWithinPeriod(long current, long start, long end)
	{
		if (start == 0L && end == 0L)
		{
			return true;
		}
		if (start == 0L)
		{
			if (current <= end)
			{
				return true;
			}
		}
		else if (end == 0L)
		{
			if (start <= current)
			{
				return true;
			}
		}
		else if (start <= current && current <= end)
		{
			return true;
		}
		return false;
	}
}
