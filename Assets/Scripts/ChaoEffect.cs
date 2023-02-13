using DataTable;
using System.Collections.Generic;
using UnityEngine;

public class ChaoEffect : MonoBehaviour
{
	public enum TargetType
	{
		MainChao,
		SubChao,
		BothChao,
		Unknown
	}

	public class DataInfo
	{
		public string m_normal;

		public string m_rare;

		public string m_srare;

		public float m_time;

		public bool m_loop;

		public DataInfo()
		{
			m_normal = null;
			m_rare = null;
			m_srare = null;
			m_time = 0f;
			m_loop = false;
		}

		public DataInfo(string normal, string rare, string srare, float time, bool loop)
		{
			m_normal = normal;
			m_rare = rare;
			m_srare = srare;
			m_time = time;
			m_loop = loop;
		}
	}

	public class LoopEffetData
	{
		public GameObject m_obj;

		public ChaoAbility m_ability;

		public ChaoType m_chaoType;

		public LoopEffetData(GameObject obj, ChaoAbility ability, ChaoType chaoType)
		{
			m_obj = obj;
			m_ability = ability;
			m_chaoType = chaoType;
		}
	}

	public readonly DataInfo[] m_effectTable = new DataInfo[94]
	{
		new DataInfo("ef_ch_bonus_all_01", null, "ef_ch_bonus_all_sr01", -1f, false),
		new DataInfo("ef_ch_bonus_score_01", "ef_ch_bonus_score_r01", null, -1f, false),
		new DataInfo("ef_ch_bonus_ring_01", "ef_ch_bonus_ring_r01", "ef_ch_bonus_ring_sr01", -1f, false),
		new DataInfo(),
		new DataInfo("ef_ch_bonus_animal_01", "ef_ch_bonus_animal_r01", null, -1f, false),
		new DataInfo("ef_ch_bonus_run_01", "ef_ch_bonus_run_r01", null, -1f, false),
		new DataInfo(null, "ef_ch_sp_score_spitem_r01", "ef_ch_sp_score_spitem_sr01", -1f, false),
		new DataInfo("ef_ch_raid_up_ring_c01", "ef_ch_raid_up_ring_r01", "ef_ch_raid_up_ring_sr01", -1f, false),
		new DataInfo(),
		new DataInfo(null, "ef_ch_combo_crystal_s_r01", "ef_ch_combo_crystal_s_sr01", -1f, false),
		new DataInfo(null, "ef_ch_combo_crystal_s_r01", null, -1f, false),
		new DataInfo(null, "ef_ch_combo_crystal_l_r01", "ef_ch_combo_crystal_l_sr01", -1f, false),
		new DataInfo(null, "ef_ch_combo_crystal_l_r01", null, -1f, false),
		new DataInfo(null, "ef_ch_combo_enemy_g_r01", "ef_ch_combo_enemy_g_sr01", -1f, false),
		new DataInfo(),
		new DataInfo(null, "ef_ch_combo_animal_r01", null, -1f, false),
		new DataInfo(null, "ef_ch_bomber_bullet_r01", null, 6f, true),
		new DataInfo(),
		new DataInfo(null, "ef_ch_combo_ring10_r01", null, -1f, false),
		new DataInfo(null, "ef_ch_combo_combo_up_r01", null, -1f, false),
		new DataInfo("ef_ch_sp_combo_crystal_sp_c01", "ef_ch_sp_combo_crystal_sp_r01", null, -1f, false),
		new DataInfo("ef_ch_combo_brk_trap_c01", null, null, -1f, false),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(null, null, "ef_ch_score_cpower_sr01", -1f, false),
		new DataInfo(null, "ef_ch_score_asteroid_r01", null, -1f, false),
		new DataInfo(null, "ef_ch_score_drill_r01", null, -1f, false),
		new DataInfo(null, "ef_ch_score_laser_r01", null, -1f, false),
		new DataInfo(null, "ef_ch_time_cpower_r01", null, -1f, false),
		new DataInfo(null, "ef_ch_time_item_r01", null, -1f, false),
		new DataInfo("ef_ch_time_combo_c01", "ef_ch_time_combo_r01", null, -1f, false),
		new DataInfo("ef_ch_time_trampoline_c01", "ef_ch_time_trampoline_r01", null, -1f, false),
		new DataInfo("ef_ch_time_magnet_c01", "ef_ch_time_magnet_r01", null, -1f, false),
		new DataInfo("ef_ch_time_asteroid_01", null, null, -1f, false),
		new DataInfo("ef_ch_time_drill_01", null, null, -1f, false),
		new DataInfo("ef_ch_time_laser_01", null, null, -1f, false),
		new DataInfo(null, null, "ef_ch_magic_atk_st_sr01", -1f, false),
		new DataInfo(),
		new DataInfo(null, null, "ef_ch_beam_atk_st_sr01", -1f, false),
		new DataInfo(null, null, "ef_ch_beam_atk_st_sr02", -1f, false),
		new DataInfo("ef_ch_up_rareenemy_c01", "ef_ch_up_rareenemy_sr01", "ef_ch_up_rareenemy_sr01", -1f, false),
		new DataInfo("ef_ch_sp_up_spitem_c01", "ef_ch_sp_up_spitem_r01", "ef_ch_sp_up_spitem_sr01", -1f, false),
		new DataInfo(null, null, "ef_ch_lastchance_sr01", 1f, true),
		new DataInfo("ef_ch_ring_absorb_c01", "ef_ch_ring_absorb_r01", "ef_ch_ring_absorb_sr01", -1f, false),
		new DataInfo(),
		new DataInfo(null, null, "ef_ch_magic_atk_st_sr01", -1f, false),
		new DataInfo("ef_ch_check_combo_01", "ef_ch_check_combo_r01", null, -1f, false),
		new DataInfo(),
		new DataInfo("ef_ch_random_magnet_01", null, null, -1f, false),
		new DataInfo("ef_ch_random_jump_01", null, null, -1f, false),
		new DataInfo("ef_ch_bonus_rsr_01", null, "ef_ch_bonus_rsr_sr01", -1f, false),
		new DataInfo(null, "ef_ch_up_magnet_r01", null, -1f, false),
		new DataInfo(null, "ef_ch_raid_up_atk_r01", null, -1f, false),
		new DataInfo("ef_ch_canon_magnet_c01", "ef_ch_canon_magnet_r01", null, -1f, false),
		new DataInfo("ef_ch_dashring_magnet_c01", null, null, -1f, false),
		new DataInfo("ef_ch_jumpboard_magnet_c01", "ef_ch_jumpboard_magnet_r01", null, -1f, false),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(null, "ef_ch_change_rareanimal_r01", null, -1f, false),
		new DataInfo(null, "ef_ch_change_rappy_r01", null, -1f, false),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo(),
		new DataInfo()
	};

	private List<LoopEffetData> m_loopDataList = new List<LoopEffetData>();

	private readonly string[] ChaoTypeName = new string[2]
	{
		"MainChao",
		"SubChao"
	};

	private static ChaoEffect instance;

	public static ChaoEffect Instance
	{
		get
		{
			return instance;
		}
	}

	protected void Awake()
	{
		SetInstance();
	}

	private void Start()
	{
		base.enabled = false;
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
		else if (this != Instance)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private ChaoData.Rarity GetRarity(ChaoType chaotype)
	{
		SaveDataManager saveDataManager = SaveDataManager.Instance;
		if (saveDataManager != null)
		{
			int id = (chaotype != 0) ? saveDataManager.PlayerData.SubChaoID : saveDataManager.PlayerData.MainChaoID;
			ChaoData chaoData = ChaoTable.GetChaoData(id);
			if (chaoData != null)
			{
				return chaoData.rarity;
			}
		}
		return ChaoData.Rarity.NORMAL;
	}

	private string GetEffectName(ChaoData.Rarity rarity, ChaoAbility ability)
	{
		if (ability != ChaoAbility.UNKNOWN && ability < ChaoAbility.NUM)
		{
			switch (rarity)
			{
			case ChaoData.Rarity.NORMAL:
				return m_effectTable[(int)ability].m_normal;
			case ChaoData.Rarity.RARE:
				return m_effectTable[(int)ability].m_rare;
			default:
				return m_effectTable[(int)ability].m_srare;
			}
		}
		return null;
	}

	private float GetEffectPlayTime(ChaoAbility ability)
	{
		if (ability != ChaoAbility.UNKNOWN && ability < ChaoAbility.NUM)
		{
			return m_effectTable[(int)ability].m_time;
		}
		return 0f;
	}

	private bool GetEffectPlayLoop(ChaoAbility ability)
	{
		if (ability != ChaoAbility.UNKNOWN && ability < ChaoAbility.NUM)
		{
			return m_effectTable[(int)ability].m_loop;
		}
		return false;
	}

	private void PlayEffect(GameObject chaoObj, ChaoAbility ability, ChaoType chaoType)
	{
		if (!(chaoObj != null))
		{
			return;
		}
		string effectName = GetEffectName(GetRarity(chaoType), ability);
		if (effectName == null)
		{
			return;
		}
		if (GetEffectPlayLoop(ability))
		{
			GameObject gameObject = ObjUtil.PlayChaoEffect(chaoObj, effectName, -1f);
			if (gameObject != null)
			{
				LoopEffetData item = new LoopEffetData(gameObject, ability, chaoType);
				m_loopDataList.Add(item);
			}
		}
		else
		{
			float effectPlayTime = GetEffectPlayTime(ability);
			ObjUtil.PlayChaoEffect(chaoObj, effectName, effectPlayTime);
		}
	}

	private void StopEffect(ChaoAbility ability, ChaoType chaoType)
	{
		foreach (LoopEffetData loopData in m_loopDataList)
		{
			if (loopData.m_ability == ability && loopData.m_chaoType == chaoType)
			{
				Object.Destroy(loopData.m_obj);
				m_loopDataList.Remove(loopData);
				break;
			}
		}
	}

	private void PlayChaoSE()
	{
		SoundManager.SePlay("act_chao_effect");
	}

	public void RequestPlayChaoEffect(TargetType target, ChaoAbility ability)
	{
		if (ability != ChaoAbility.UNKNOWN && ability < ChaoAbility.NUM)
		{
			switch (target)
			{
			case TargetType.BothChao:
				PlayEffect(GetChaoObject(ChaoType.MAIN), ability, ChaoType.MAIN);
				PlayEffect(GetChaoObject(ChaoType.SUB), ability, ChaoType.SUB);
				break;
			case TargetType.MainChao:
				PlayEffect(GetChaoObject(ChaoType.MAIN), ability, ChaoType.MAIN);
				break;
			case TargetType.SubChao:
				PlayEffect(GetChaoObject(ChaoType.SUB), ability, ChaoType.SUB);
				break;
			}
			PlayChaoSE();
		}
	}

	public void RequestStopChaoEffect(TargetType target, ChaoAbility ability)
	{
		if (ability != ChaoAbility.UNKNOWN && ability < ChaoAbility.NUM)
		{
			switch (target)
			{
			case TargetType.MainChao:
				StopEffect(ability, ChaoType.MAIN);
				break;
			case TargetType.SubChao:
				StopEffect(ability, ChaoType.SUB);
				break;
			case TargetType.BothChao:
				StopEffect(ability, ChaoType.MAIN);
				StopEffect(ability, ChaoType.SUB);
				break;
			}
		}
	}

	private GameObject GetChaoObject(ChaoType chaotype)
	{
		Transform parent = base.transform.parent;
		if (parent != null)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(parent.gameObject, ChaoTypeName[(int)chaotype]);
			if (gameObject != null && gameObject.activeSelf)
			{
				return gameObject;
			}
			return null;
		}
		return null;
	}
}
