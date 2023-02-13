using App;
using System;
using UnityEngine;

public class RaidBossUser
{
	public static SocialInterface s_socialInterface;

	public long damage
	{
		get;
		set;
	}

	public long destroyCount
	{
		get;
		set;
	}

	public bool isDestroy
	{
		get;
		set;
	}

	public int mapRank
	{
		get;
		set;
	}

	public string dispMapRank
	{
		get
		{
			return (mapRank + 1).ToString("D3");
		}
	}

	public int rankIndex
	{
		get;
		set;
	}

	public int rankIndexChanged
	{
		get;
		set;
	}

	public string userName
	{
		get;
		set;
	}

	public bool isFriend
	{
		get;
		set;
	}

	public bool isSentEnergy
	{
		get;
		set;
	}

	public CharaType charaType
	{
		get;
		set;
	}

	public CharaType charaSubType
	{
		get;
		set;
	}

	public int charaLevel
	{
		get;
		set;
	}

	public int mainChaoId
	{
		get;
		set;
	}

	public int subChaoId
	{
		get;
		set;
	}

	public int mainChaoLevel
	{
		get;
		set;
	}

	public int subChaoLevel
	{
		get;
		set;
	}

	public Env.Language language
	{
		get;
		set;
	}

	public int leagueIndex
	{
		get;
		set;
	}

	public string id
	{
		get;
		set;
	}

	public DateTime loginTime
	{
		get;
		set;
	}

	public bool isRankTop
	{
		get
		{
			return (rankIndex < 1) ? true : false;
		}
	}

	public int mainChaoRarity
	{
		get
		{
			return mainChaoId / 1000;
		}
	}

	public int subChaoRarity
	{
		get
		{
			return subChaoId / 1000;
		}
	}

	public RaidBossUser(ServerEventRaidBossUserState state)
	{
		isDestroy = state.WrestleBeatFlg;
		damage = state.WrestleDamage;
		destroyCount = state.WrestleCount;
		id = state.WrestleId;
		userName = state.UserName;
		rankIndex = state.Grade - 1;
		mapRank = state.NumRank;
		loginTime = NetUtil.GetLocalDateTime(state.LoginTime);
		charaType = new ServerItem((ServerItem.Id)state.CharaId).charaType;
		charaSubType = new ServerItem((ServerItem.Id)state.SubCharaId).charaType;
		charaLevel = state.CharaLevel;
		mainChaoId = new ServerItem((ServerItem.Id)state.MainChaoId).chaoId;
		subChaoId = new ServerItem((ServerItem.Id)state.SubChaoId).chaoId;
		mainChaoLevel = state.MainChaoLevel;
		subChaoLevel = state.SubChaoLevel;
		leagueIndex = state.League;
		language = (Env.Language)state.Language;
		if (s_socialInterface == null)
		{
			s_socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		}
		if (s_socialInterface != null)
		{
			isFriend = (SocialInterface.GetSocialUserDataFromGameId(s_socialInterface.FriendList, id) != null);
		}
		else
		{
			isFriend = (isSentEnergy || UnityEngine.Random.Range(0, 3) != 0);
		}
	}

	public RaidBossUser(ServerEventRaidBossDesiredState state)
	{
		isDestroy = false;
		damage = 0L;
		destroyCount = 0L;
		id = state.DesireId;
		userName = state.UserName;
		mapRank = state.NumRank;
		loginTime = NetUtil.GetLocalDateTime(state.LoginTime);
		charaType = new ServerItem((ServerItem.Id)state.CharaId).charaType;
		charaSubType = new ServerItem((ServerItem.Id)state.SubCharaId).charaType;
		charaLevel = state.CharaLevel;
		mainChaoId = new ServerItem((ServerItem.Id)state.MainChaoId).chaoId;
		subChaoId = new ServerItem((ServerItem.Id)state.SubChaoId).chaoId;
		mainChaoLevel = state.MainChaoLevel;
		subChaoLevel = state.SubChaoLevel;
		leagueIndex = state.League;
		language = (Env.Language)state.Language;
		if (s_socialInterface == null)
		{
			s_socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		}
		if (s_socialInterface != null)
		{
			isFriend = (SocialInterface.GetSocialUserDataFromGameId(s_socialInterface.FriendList, id) != null);
		}
		else
		{
			isFriend = (isSentEnergy || UnityEngine.Random.Range(0, 3) != 0);
		}
	}

	public RankingUtil.Ranker ConvertRankerData()
	{
		return new RankingUtil.Ranker(this);
	}
}
