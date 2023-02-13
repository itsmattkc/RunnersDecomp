using App;

public class ServerDailyBattleData
{
	public long maxScore;

	public int league;

	public string userId = string.Empty;

	public string name = string.Empty;

	public long loginTime;

	public int mainChaoId;

	public int mainChaoLevel;

	public int subChaoId;

	public int subChaoLevel;

	public int numRank;

	public int charaId;

	public int charaLevel;

	public int subCharaId;

	public int subCharaLevel;

	public int goOnWin;

	public bool isSentEnergy;

	public Env.Language language;

	public ServerDailyBattleData()
	{
		maxScore = 0L;
		league = 0;
		userId = string.Empty;
		name = string.Empty;
		loginTime = 0L;
		mainChaoId = 0;
		mainChaoLevel = 0;
		subChaoId = 0;
		subChaoLevel = 0;
		numRank = 0;
		charaId = 0;
		charaLevel = 0;
		subCharaId = 0;
		subCharaLevel = 0;
		goOnWin = 0;
		isSentEnergy = false;
		language = Env.Language.JAPANESE;
	}

	public bool CheckFriend()
	{
		bool result = false;
		SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		if (socialInterface != null && socialInterface.IsLoggedIn)
		{
			result = (SocialInterface.GetSocialUserDataFromGameId(socialInterface.FriendList, userId) != null);
		}
		return result;
	}

	public void Dump()
	{
		if (!string.IsNullOrEmpty(userId))
		{
			Debug.Log(string.Format("ServerDailyBattleData  maxScore:{0} league:{1} userId:{2} name:{3} numRank:{4} goOnWin:{5}", maxScore, league, userId, name, numRank, goOnWin));
		}
		else
		{
			Debug.Log("ServerDailyBattleData  null");
		}
	}

	public void CopyTo(ServerDailyBattleData dest)
	{
		dest.maxScore = maxScore;
		dest.league = league;
		dest.userId = userId;
		dest.name = name;
		dest.loginTime = loginTime;
		dest.mainChaoId = mainChaoId;
		dest.mainChaoLevel = mainChaoLevel;
		dest.subChaoId = subChaoId;
		dest.subChaoLevel = subChaoLevel;
		dest.numRank = numRank;
		dest.charaId = charaId;
		dest.charaLevel = charaLevel;
		dest.subCharaId = subCharaId;
		dest.subCharaLevel = subCharaLevel;
		dest.goOnWin = goOnWin;
		dest.isSentEnergy = isSentEnergy;
		dest.language = language;
	}
}
