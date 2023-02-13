namespace SaveData
{
	public class PlayerData
	{
		public const uint MAX_CHALLENGE_COUNT = 99999u;

		private uint m_progress_status;

		private long m_total_distance;

		private uint m_challenge_count;

		private int m_challenge_count_offset;

		private long m_best_score;

		private long m_best_score_quick;

		private uint m_rank;

		private int m_rank_offset;

		private CharaType m_main_chara_type;

		private CharaType m_sub_chara_type;

		private int m_main_chao_id;

		private int m_sub_chao_id;

		private int m_friend_chao_id;

		private int m_friend_chao_level;

		private string m_rental_friend_id;

		private DailyMissionData m_daily_mission_data = new DailyMissionData();

		private DailyMissionData m_beforeDailyMissionData = new DailyMissionData();

		private ItemType[] m_equipped_item = new ItemType[3];

		private ItemStatus[] m_equipped_item_use_status = new ItemStatus[3];

		private bool[] m_boosted_item = new bool[3];

		public uint Rank
		{
			get
			{
				return m_rank;
			}
			set
			{
				m_rank = value;
			}
		}

		public int RankOffset
		{
			get
			{
				return m_rank_offset;
			}
			set
			{
				m_rank_offset = value;
			}
		}

		public uint DisplayRank
		{
			get
			{
				return (uint)(((RankOffset < 0) ? ((int)Rank - -RankOffset) : ((int)Rank + RankOffset)) + 1);
			}
		}

		public uint ProgressStatus
		{
			get
			{
				return m_progress_status;
			}
			set
			{
				m_progress_status = value;
			}
		}

		public long TotalDistance
		{
			get
			{
				return m_total_distance;
			}
			set
			{
				m_total_distance = value;
			}
		}

		public uint ChallengeCount
		{
			get
			{
				return m_challenge_count;
			}
			set
			{
				m_challenge_count = value;
			}
		}

		public int ChallengeCountOffset
		{
			get
			{
				return m_challenge_count_offset;
			}
			set
			{
				m_challenge_count_offset = value;
			}
		}

		public uint DisplayChallengeCount
		{
			get
			{
				return (ChallengeCountOffset < 0) ? (ChallengeCount - (uint)(-ChallengeCountOffset)) : (ChallengeCount + (uint)ChallengeCountOffset);
			}
		}

		public long BestScore
		{
			get
			{
				return m_best_score;
			}
			set
			{
				m_best_score = value;
			}
		}

		public long BestScoreQuick
		{
			get
			{
				return m_best_score_quick;
			}
			set
			{
				m_best_score_quick = value;
			}
		}

		public CharaType MainChara
		{
			get
			{
				return m_main_chara_type;
			}
			set
			{
				m_main_chara_type = value;
			}
		}

		public CharaType SubChara
		{
			get
			{
				return m_sub_chara_type;
			}
			set
			{
				m_sub_chara_type = value;
			}
		}

		public int MainChaoID
		{
			get
			{
				return m_main_chao_id;
			}
			set
			{
				m_main_chao_id = value;
			}
		}

		public int SubChaoID
		{
			get
			{
				return m_sub_chao_id;
			}
			set
			{
				m_sub_chao_id = value;
			}
		}

		public int FriendChaoID
		{
			get
			{
				return m_friend_chao_id;
			}
			set
			{
				m_friend_chao_id = value;
			}
		}

		public int FriendChaoLevel
		{
			get
			{
				return m_friend_chao_level;
			}
			set
			{
				m_friend_chao_level = value;
			}
		}

		public string RentalFriendId
		{
			get
			{
				return m_rental_friend_id;
			}
			set
			{
				m_rental_friend_id = value;
			}
		}

		public ItemType[] EquippedItem
		{
			get
			{
				return m_equipped_item;
			}
			set
			{
				m_equipped_item = value;
			}
		}

		public ItemStatus[] EquippedItemUseStatue
		{
			get
			{
				return m_equipped_item_use_status;
			}
			set
			{
				m_equipped_item_use_status = value;
			}
		}

		public bool[] BoostedItem
		{
			get
			{
				return m_boosted_item;
			}
			set
			{
				m_boosted_item = value;
			}
		}

		public DailyMissionData DailyMission
		{
			get
			{
				return m_daily_mission_data;
			}
			set
			{
				m_daily_mission_data = value;
			}
		}

		public DailyMissionData BeforeDailyMissionData
		{
			get
			{
				return m_beforeDailyMissionData;
			}
			set
			{
				m_beforeDailyMissionData = value;
			}
		}

		public PlayerData()
		{
			m_progress_status = 0u;
			m_total_distance = 0L;
			m_challenge_count = 3u;
			m_best_score = 0L;
			m_best_score_quick = 0L;
			m_main_chao_id = -1;
			m_sub_chao_id = -1;
			m_friend_chao_id = -1;
			m_friend_chao_level = -1;
			m_rental_friend_id = string.Empty;
			m_main_chara_type = CharaType.SONIC;
			m_sub_chara_type = CharaType.UNKNOWN;
			m_rank = 1u;
			for (int i = 0; i < 3; i++)
			{
				m_equipped_item[i] = ItemType.UNKNOWN;
				m_equipped_item_use_status[i] = ItemStatus.NO_USE;
			}
			for (int j = 0; j < 3; j++)
			{
				m_boosted_item[j] = false;
			}
		}
	}
}
