using System.Collections.Generic;
using UnityEngine;

public class ObjBossParameter : MonoBehaviour
{
	private const float ADDSPEED_DISTANCE = 0.2f;

	private static Vector3 SHOT_ROT_BASE = new Vector3(-1f, 0f, 0f);

	private int m_bossType;

	private BossCharaType m_bossCharaType;

	private float m_speed;

	private float m_min_speed;

	private float m_player_speed;

	private float m_add_speed;

	private float m_add_speed_ratio = 1f;

	private Vector3 m_start_pos = Vector3.zero;

	private int m_ring = 20;

	private int m_super_ring;

	private int m_red_star_ring;

	private int m_bronze_timer;

	private int m_silver_timer;

	private int m_gold_timer;

	private int m_hp;

	private int m_hp_max;

	private int m_distance;

	private float m_step_move_y;

	private bool m_data_setup;

	private int m_level;

	private int m_attackPower = 1;

	private float m_down_speed;

	private float m_attackInterspaceMin;

	private float m_attackInterspaceMax;

	private float m_defaultPlayerDistance;

	private int m_tbl_id;

	private int m_attack_tbl_id;

	private int m_trapRand;

	private float m_boundParamMin;

	private float m_boundParamMax;

	private int m_boundMaxRand;

	private float m_shotSpeed;

	private float m_attackSpeed;

	private float m_attackSpeedMin;

	private float m_attackSpeedMax;

	private float m_bumperFirstSpeed;

	private float m_bumperOutOfcontrol;

	private float m_bumperSpeedup;

	private float m_ballSpeed = 8f;

	private int m_bumperRand;

	private bool m_attackBallFlag;

	private int m_attackTrapCount;

	private int m_attackTrapCountMax;

	private float m_missileSpeed;

	private float m_missileInterspace;

	private float m_rotSpeed;

	private bool m_afterAttack;

	private List<Map3AttackData> m_map3AttackDataList;

	private static readonly Vector3[] BOM_TYPE_A = new Vector3[11]
	{
		new Vector3(0f, 0f, 0f),
		new Vector3(1f, 0f, 0f),
		new Vector3(0f, 1f, 0f),
		new Vector3(1f, 0f, 0f),
		new Vector3(1f, 0f, 0f),
		new Vector3(0f, 1f, 0f),
		new Vector3(0f, 1f, 0f),
		new Vector3(1f, 0f, 0f),
		new Vector3(1f, 0f, 0f),
		new Vector3(0f, 1f, 0f),
		new Vector3(0f, 1f, 0f)
	};

	private static readonly Vector3[] BOM_TYPE_B = new Vector3[11]
	{
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(1f, 0f, 0f),
		new Vector3(0f, 1f, 0f),
		new Vector3(0f, 1f, 0f),
		new Vector3(1f, 0f, 0f),
		new Vector3(1f, 0f, 0f),
		new Vector3(0f, 1f, 0f),
		new Vector3(0f, 1f, 0f),
		new Vector3(1f, 0f, 0f)
	};

	public Vector3 ShotRotBase
	{
		get
		{
			return SHOT_ROT_BASE;
		}
	}

	public int TypeBoss
	{
		get
		{
			return m_bossType;
		}
		set
		{
			m_bossType = value;
			m_bossCharaType = BossTypeUtil.GetBossCharaType((BossType)m_bossType);
		}
	}

	public BossCharaType CharaTypeBoss
	{
		get
		{
			return m_bossCharaType;
		}
	}

	public float Speed
	{
		get
		{
			return m_speed;
		}
		set
		{
			m_speed = value;
		}
	}

	public float MinSpeed
	{
		get
		{
			return m_min_speed;
		}
		set
		{
			m_min_speed = value;
		}
	}

	public float PlayerSpeed
	{
		get
		{
			return m_player_speed;
		}
	}

	public float AddSpeed
	{
		get
		{
			return m_add_speed;
		}
	}

	public float AddSpeedRatio
	{
		get
		{
			return m_add_speed_ratio;
		}
	}

	public float AddSpeedDistance
	{
		get
		{
			return AddSpeed * 0.2f;
		}
	}

	public Vector3 StartPos
	{
		get
		{
			return m_start_pos;
		}
	}

	public int RingCount
	{
		get
		{
			return m_ring;
		}
	}

	public int SuperRingRatio
	{
		get
		{
			return m_super_ring;
		}
	}

	public int RedStarRingRatio
	{
		get
		{
			return m_red_star_ring;
		}
	}

	public int BronzeTimerRatio
	{
		get
		{
			return m_bronze_timer;
		}
	}

	public int SilverTimerRatio
	{
		get
		{
			return m_silver_timer;
		}
	}

	public int GoldTimerRatio
	{
		get
		{
			return m_gold_timer;
		}
	}

	public int BossHP
	{
		get
		{
			return m_hp;
		}
		set
		{
			m_hp = value;
		}
	}

	public int BossHPMax
	{
		get
		{
			return m_hp_max;
		}
		set
		{
			m_hp_max = value;
		}
	}

	public int BossDistance
	{
		get
		{
			return m_distance;
		}
		set
		{
			if (TypeBoss == 0)
			{
				m_distance = ObjUtil.GetChaoAbliltyValue(ChaoAbility.BOSS_STAGE_TIME, value);
			}
			else
			{
				m_distance = value;
			}
		}
	}

	public float StepMoveY
	{
		get
		{
			return m_step_move_y;
		}
		set
		{
			m_step_move_y = value;
		}
	}

	public int BossLevel
	{
		get
		{
			return m_level;
		}
		set
		{
			m_level = value;
		}
	}

	public int BossAttackPower
	{
		get
		{
			return m_attackPower;
		}
		set
		{
			m_attackPower = value;
		}
	}

	public float DownSpeed
	{
		get
		{
			return m_down_speed;
		}
		set
		{
			m_down_speed = value;
		}
	}

	public float AttackInterspaceMin
	{
		get
		{
			return m_attackInterspaceMin;
		}
		set
		{
			m_attackInterspaceMin = value;
		}
	}

	public float AttackInterspaceMax
	{
		get
		{
			return m_attackInterspaceMax;
		}
		set
		{
			m_attackInterspaceMax = value;
		}
	}

	public float DefaultPlayerDistance
	{
		get
		{
			return m_defaultPlayerDistance;
		}
		set
		{
			m_defaultPlayerDistance = value;
		}
	}

	public int TableID
	{
		get
		{
			return m_tbl_id;
		}
		set
		{
			m_tbl_id = value;
		}
	}

	public int AttackTableID
	{
		get
		{
			return m_attack_tbl_id;
		}
		set
		{
			m_attack_tbl_id = value;
		}
	}

	public int TrapRand
	{
		get
		{
			return m_trapRand;
		}
		set
		{
			m_trapRand = value;
		}
	}

	public float BoundParamMin
	{
		get
		{
			return m_boundParamMin;
		}
		set
		{
			m_boundParamMin = value;
		}
	}

	public float BoundParamMax
	{
		get
		{
			return m_boundParamMax;
		}
		set
		{
			m_boundParamMax = value;
		}
	}

	public int BoundMaxRand
	{
		get
		{
			return m_boundMaxRand;
		}
		set
		{
			m_boundMaxRand = value;
		}
	}

	public float ShotSpeed
	{
		get
		{
			return m_shotSpeed;
		}
		set
		{
			m_shotSpeed = value;
		}
	}

	public float AttackSpeed
	{
		get
		{
			return m_attackSpeed;
		}
		set
		{
			m_attackSpeed = value;
		}
	}

	public float AttackSpeedMin
	{
		get
		{
			return m_attackSpeedMin;
		}
		set
		{
			m_attackSpeedMin = value;
		}
	}

	public float AttackSpeedMax
	{
		get
		{
			return m_attackSpeedMax;
		}
		set
		{
			m_attackSpeedMax = value;
		}
	}

	public float BumperFirstSpeed
	{
		get
		{
			return m_bumperFirstSpeed;
		}
		set
		{
			m_bumperFirstSpeed = value;
		}
	}

	public float BumperOutOfcontrol
	{
		get
		{
			return m_bumperOutOfcontrol;
		}
		set
		{
			m_bumperOutOfcontrol = value;
		}
	}

	public float BumperSpeedup
	{
		get
		{
			return m_bumperSpeedup;
		}
		set
		{
			m_bumperSpeedup = value;
		}
	}

	public float BallSpeed
	{
		get
		{
			return m_ballSpeed;
		}
		set
		{
			m_ballSpeed = value;
		}
	}

	public int BumperRand
	{
		get
		{
			return m_bumperRand;
		}
		set
		{
			m_bumperRand = value;
		}
	}

	public bool AttackBallFlag
	{
		get
		{
			return m_attackBallFlag;
		}
		set
		{
			m_attackBallFlag = value;
		}
	}

	public int AttackTrapCount
	{
		get
		{
			return m_attackTrapCount;
		}
		set
		{
			m_attackTrapCount = value;
		}
	}

	public int AttackTrapCountMax
	{
		get
		{
			return m_attackTrapCountMax;
		}
		set
		{
			m_attackTrapCountMax = value;
		}
	}

	public float MissileSpeed
	{
		get
		{
			return m_missileSpeed;
		}
		set
		{
			m_missileSpeed = value;
		}
	}

	public float MissileInterspace
	{
		get
		{
			return m_missileInterspace;
		}
		set
		{
			m_missileInterspace = value;
		}
	}

	public float RotSpeed
	{
		get
		{
			return m_rotSpeed;
		}
		set
		{
			m_rotSpeed = value;
		}
	}

	public bool AfterAttack
	{
		get
		{
			return m_afterAttack;
		}
		set
		{
			m_afterAttack = value;
		}
	}

	public void Setup()
	{
		m_speed = 0f;
		m_min_speed = 0f;
		m_player_speed = ObjUtil.GetPlayerDefaultSpeed();
		m_add_speed = ObjUtil.GetPlayerAddSpeed();
		m_add_speed_ratio = ObjUtil.GetPlayerAddSpeedRatio();
		m_start_pos = base.transform.position;
		m_hp = BossHPMax;
		OnSetup();
		SetupBossTable();
	}

	protected virtual void OnSetup()
	{
	}

	public Map3AttackData GetMap3AttackData()
	{
		if (m_map3AttackDataList != null && m_map3AttackDataList.Count > 0)
		{
			int num = Random.Range(0, m_map3AttackDataList.Count);
			if (num < m_map3AttackDataList.Count)
			{
				return m_map3AttackDataList[num];
			}
		}
		return null;
	}

	public Vector3 GetMap3BomTblA(BossAttackType type)
	{
		if ((uint)type < BOM_TYPE_A.Length)
		{
			return BOM_TYPE_A[(int)type];
		}
		return BOM_TYPE_A[0];
	}

	public Vector3 GetMap3BomTblB(BossAttackType type)
	{
		if ((uint)type < BOM_TYPE_B.Length)
		{
			return BOM_TYPE_B[(int)type];
		}
		return BOM_TYPE_B[0];
	}

	public void SetupBossTable()
	{
		if (m_data_setup)
		{
			return;
		}
		GameObject gameObject = GameObject.Find("GameModeStage");
		if (!(gameObject != null))
		{
			return;
		}
		GameModeStage component = gameObject.GetComponent<GameModeStage>();
		if (!(component != null))
		{
			return;
		}
		BossTable bossTable = component.GetBossTable();
		BossMap3Table bossMap3Table = component.GetBossMap3Table();
		if (bossTable == null || !bossTable.IsSetupEnd() || bossMap3Table == null || !bossMap3Table.IsSetupEnd() || m_map3AttackDataList != null)
		{
			return;
		}
		m_super_ring = bossTable.GetItemData(TableID, BossTableItem.SuperRing);
		m_red_star_ring = bossTable.GetItemData(TableID, BossTableItem.RedStarRing);
		m_bronze_timer = bossTable.GetItemData(TableID, BossTableItem.BronzeWatch);
		m_silver_timer = bossTable.GetItemData(TableID, BossTableItem.SilverWatch);
		m_gold_timer = bossTable.GetItemData(TableID, BossTableItem.GoldWatch);
		m_map3AttackDataList = new List<Map3AttackData>();
		for (int i = 0; i < 16; i++)
		{
			Map3AttackData map3AttackData = bossMap3Table.GetMap3AttackData(AttackTableID, i);
			if (map3AttackData.GetAttackCount() != 0)
			{
				m_map3AttackDataList.Add(map3AttackData);
				continue;
			}
			break;
		}
		m_data_setup = true;
	}
}
