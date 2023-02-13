using SaveData;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RaidBossData
{
	public delegate void CallbackRaidBossDataUpdate(RaidBossData data);

	private RaidBossUser m_myData;

	private List<RaidBossUser> m_listData;

	private ServerEventRaidBossBonus m_raidbossReward;

	private long m_id;

	private int m_lv;

	private int m_rarity;

	private string m_discoverer;

	private string m_name;

	private bool m_participation;

	private bool m_end;

	private bool m_clear;

	private bool m_encounter;

	private bool m_creowded;

	private DateTime m_limitTime;

	private long m_bossHp;

	private long m_bossHpMax;

	private RaidBossWindow m_parent;

	private CallbackRaidBossDataUpdate m_callback;

	public RaidBossWindow parent
	{
		get
		{
			return m_parent;
		}
		set
		{
			m_parent = value;
		}
	}

	public long id
	{
		get
		{
			return m_id;
		}
	}

	public int rarity
	{
		get
		{
			return m_rarity;
		}
	}

	public int lv
	{
		get
		{
			return m_lv;
		}
	}

	public string discoverer
	{
		get
		{
			return m_discoverer;
		}
	}

	public string name
	{
		get
		{
			return m_name;
		}
	}

	public bool participation
	{
		get
		{
			return m_participation;
		}
	}

	public bool end
	{
		get
		{
			return m_end;
		}
	}

	public bool clear
	{
		get
		{
			return m_clear;
		}
	}

	public bool crowded
	{
		get
		{
			return m_creowded;
		}
	}

	public long hp
	{
		get
		{
			return m_bossHp;
		}
	}

	public long hpMax
	{
		get
		{
			return m_bossHpMax;
		}
	}

	public RaidBossUser myData
	{
		get
		{
			return m_myData;
		}
	}

	public List<RaidBossUser> listData
	{
		get
		{
			return m_listData;
		}
	}

	public RaidBossData(ServerEventRaidBossState state)
	{
		m_myData = null;
		m_listData = null;
		m_callback = null;
		SetData(state);
	}

	public void SetData(ServerEventRaidBossState state)
	{
		m_callback = null;
		m_id = state.Id;
		m_rarity = state.Rarity;
		m_lv = state.Level;
		m_encounter = state.Encounter;
		m_discoverer = state.EncounterName;
		m_name = EventUtility.GetRaidBossName(m_rarity);
		m_participation = state.Participation;
		m_end = false;
		m_clear = false;
		m_creowded = state.Crowded;
		switch (state.Status)
		{
		case 2:
			m_end = true;
			break;
		case 3:
			m_clear = true;
			m_end = true;
			break;
		case 4:
			m_clear = true;
			m_end = true;
			break;
		}
		m_limitTime = state.EscapeAt;
		m_bossHp = state.HitPoint;
		m_bossHpMax = state.MaxHitPoint;
		m_limitTime = m_limitTime.AddSeconds(1.0);
	}

	public void SetReward(ServerEventRaidBossBonus bonus)
	{
		m_raidbossReward = bonus;
	}

	public float GetHpRate()
	{
		if (m_bossHpMax < 0 && m_bossHp < 0)
		{
			return 0f;
		}
		if (m_bossHp >= m_bossHpMax)
		{
			return 1f;
		}
		return (float)m_bossHp / (float)m_bossHpMax;
	}

	public TimeSpan GetTimeLimit()
	{
		DateTime currentTime = NetBase.GetCurrentTime();
		return m_limitTime - currentTime;
	}

	public string GetTimeLimitString(bool slightlyChangeColor = false)
	{
		string text = "--:--:--";
		if (!end)
		{
			DateTime currentTime = NetBase.GetCurrentTime();
			TimeSpan timeSpan = m_limitTime - currentTime;
			if (timeSpan.Ticks > 0)
			{
				if (timeSpan.TotalSeconds > 60.0 || !slightlyChangeColor)
				{
					return string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
				}
				return string.Format("[ff0000]{0:D2}:{1:D2}:{2:D2}[-]", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			}
			if (slightlyChangeColor)
			{
				return "[ff0000]00:00:00[-]";
			}
			return "00:00:00";
		}
		if (slightlyChangeColor)
		{
			return "[ff0000]00:00:00[-]";
		}
		return "00:00:00";
	}

	public bool IsLimit()
	{
		return GetTimeLimit().Ticks <= 0;
	}

	public bool IsDiscoverer()
	{
		return m_encounter;
	}

	public bool IsList()
	{
		return m_listData != null || m_myData != null;
	}

	public bool GetListData(CallbackRaidBossDataUpdate callback, MonoBehaviour obj = null)
	{
		Debug.Log("GetListData:" + IsList());
		m_callback = callback;
		m_callback(this);
		return true;
	}

	public void SetUserList(List<ServerEventRaidBossUserState> stateList)
	{
		if (m_listData == null)
		{
			m_listData = new List<RaidBossUser>();
		}
		else
		{
			m_listData.Clear();
		}
		if (m_listData == null || stateList == null)
		{
			return;
		}
		string gameID = SystemSaveManager.GetGameID();
		foreach (ServerEventRaidBossUserState state in stateList)
		{
			RaidBossUser raidBossUser = new RaidBossUser(state);
			if (!string.IsNullOrEmpty(raidBossUser.id) && raidBossUser.id != "0000000000" && raidBossUser.destroyCount > 0)
			{
				m_listData.Add(raidBossUser);
				if (!string.IsNullOrEmpty(gameID) && gameID == raidBossUser.id)
				{
					m_myData = raidBossUser;
				}
			}
		}
		if (m_listData.Count > 0)
		{
			m_listData.Sort((RaidBossUser userA, RaidBossUser userB) => (int)(userB.damage - userA.damage));
		}
	}

	public string GetRewardText()
	{
		string text = null;
		if (end && clear && m_raidbossReward != null)
		{
			text = new ServerItem(ServerItem.Id.RAIDRING).serverItemName;
			int num = 0;
			num += m_raidbossReward.BeatBonus;
			num += m_raidbossReward.DamageRateBonus;
			num += m_raidbossReward.DamageTopBonus;
			num += m_raidbossReward.EncounterBonus;
			num += m_raidbossReward.WrestleBonus;
			if (num > 1)
			{
				text = text + " × " + num;
			}
			else if (num <= 0)
			{
				text = null;
			}
		}
		return text;
	}
}
