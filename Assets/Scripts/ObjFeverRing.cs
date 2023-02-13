using System.Collections.Generic;
using UnityEngine;

public class ObjFeverRing : MonoBehaviour
{
	private enum Type
	{
		RING,
		SUPER_RING,
		REDSTAR_RING,
		BRONZE_TIMER,
		SILVER_TIMER,
		GOLD_TIMER,
		NUM
	}

	private class FeverRingData
	{
		public string m_name;

		public ResourceCategory m_category;

		public FeverRingData(string name, ResourceCategory category)
		{
			m_name = name;
			m_category = category;
		}
	}

	private class FeverRingParam
	{
		public float m_add_x;

		public float m_add_y;

		public FeverRingParam(float add_x, float add_y)
		{
			m_add_x = add_x;
			m_add_y = add_y;
		}
	}

	private struct FeverRingInfo
	{
		public FeverRingParam m_param;

		public Type m_type;
	}

	private const float END_TIME = 6f;

	private const float RING_SPEED = 6f;

	private const float RING_GRAVITY = -6.1f;

	private const float BOUND_Y = 1f;

	private const float BOUND_DOWN_X = 0.5f;

	private const float BOUND_DOWN_Y = 0.01f;

	private const float BOUND_MIN = 1.8f;

	private const int CREATE_MAX = 5;

	private static readonly FeverRingData[] OBJDATA_PARAMS = new FeverRingData[6]
	{
		new FeverRingData("ObjRing", ResourceCategory.OBJECT_PREFAB),
		new FeverRingData("ObjSuperRing", ResourceCategory.OBJECT_PREFAB),
		new FeverRingData("ObjRedStarRing", ResourceCategory.OBJECT_PREFAB),
		new FeverRingData("ObjTimerBronze", ResourceCategory.OBJECT_PREFAB),
		new FeverRingData("ObjTimerSilver", ResourceCategory.OBJECT_PREFAB),
		new FeverRingData("ObjTimerGold", ResourceCategory.OBJECT_PREFAB)
	};

	private static readonly FeverRingParam[] FEVERRING_PARAM = new FeverRingParam[20]
	{
		new FeverRingParam(4f, 0.81f),
		new FeverRingParam(3.5f, 1.22f),
		new FeverRingParam(2.5f, 0.23f),
		new FeverRingParam(4.4f, 0.44f),
		new FeverRingParam(3.8f, 1.35f),
		new FeverRingParam(5f, 0.56f),
		new FeverRingParam(2f, 1.17f),
		new FeverRingParam(4f, 1.58f),
		new FeverRingParam(3.5f, 1.39f),
		new FeverRingParam(3f, 0.31f),
		new FeverRingParam(1.3f, 0.72f),
		new FeverRingParam(3.2f, 0.43f),
		new FeverRingParam(2.4f, 1.24f),
		new FeverRingParam(4.3f, 0.55f),
		new FeverRingParam(3.2f, 1.26f),
		new FeverRingParam(3.6f, 1.27f),
		new FeverRingParam(2.4f, 0.88f),
		new FeverRingParam(4.2f, 0.49f),
		new FeverRingParam(3f, 1.21f),
		new FeverRingParam(4.1f, 0.82f)
	};

	private int m_count;

	private int m_createCount;

	private float m_time;

	private float m_add_player_speed;

	private FeverRingInfo[] m_info;

	private int m_bossType;

	private PlayerInformation m_playerInformation;

	private LevelInformation m_levelInformation;

	private List<SpawnableObject> m_rivivalObj = new List<SpawnableObject>();

	private GameObject m_stageBlockManager;

	private void Update()
	{
		if (m_count <= 0)
		{
			return;
		}
		if (m_createCount < m_count)
		{
			CreateRing(m_createCount, Mathf.Min(m_createCount + 5, m_count));
		}
		m_time += Time.deltaTime;
		if (!(m_time > 6f))
		{
			return;
		}
		foreach (SpawnableObject item in m_rivivalObj)
		{
			if (item.Share)
			{
				item.Sleep = true;
				GameObject gameObject = GameObjectUtil.FindChildGameObject(item.gameObject, "MotorThrow(Clone)");
				if (gameObject != null)
				{
					Object.Destroy(gameObject);
				}
				item.SetSleep();
			}
		}
		Object.Destroy(base.gameObject);
	}

	public void Setup(int ringCount, int in_superRing, int in_redStarRing, int in_bronzeTimer, int in_silverTimer, int in_goldTimer, BossType type)
	{
		m_playerInformation = ObjUtil.GetPlayerInformation();
		m_levelInformation = ObjUtil.GetLevelInformation();
		m_stageBlockManager = GameObjectUtil.FindGameObjectWithTag("StageManager", "StageBlockManager");
		/* Below is the code that SEGA removed in 2.0.3 that broke the abilities of the RC Battle Cruiser and RC Pirate Spaceship buddies.
		It is commented out because it is not accurate to a vanilla 2.0.3 decompilation, but is here as a reference if needed to fix these abilites
		and have them function exactly how they did in 2.0.1 and older.
		int chaoAbliltyValue = ObjUtil.GetChaoAbliltyValue(ChaoAbility.BOSS_SUPER_RING_RATE, in_superRing);
		int chaoAbliltyValue2 = ObjUtil.GetChaoAbliltyValue(ChaoAbility.BOSS_RED_RING_RATE, in_redStarRing);
		*/
		int num = in_redStarRing;
		if (StageModeManager.Instance != null && StageModeManager.Instance.FirstTutorial)
		{
			num = 0;
		}
		m_bossType = (int)type;
		m_add_player_speed = ObjUtil.GetPlayerAddSpeed();
		m_time = 0f;
		m_count = ringCount;
		if (m_count <= 0)
		{
			return;
		}
		m_info = new FeverRingInfo[m_count];
		int num2 = Random.Range(0, FEVERRING_PARAM.Length);
		float num3 = 0f;
		int num4 = num2;
		int num5 = num2;
		for (int i = 0; i < m_count; i++)
		{
			m_info[i].m_type = Type.RING;
			m_info[i].m_param = new FeverRingParam(0f, 0f);
			if (num4 >= FEVERRING_PARAM.Length)
			{
				num3 = num5 / FEVERRING_PARAM.Length;
				num4 = 0;
			}
			m_info[i].m_param.m_add_x = FEVERRING_PARAM[num4].m_add_x + num3 * 0.05f;
			m_info[i].m_param.m_add_y = FEVERRING_PARAM[num4].m_add_y + num3 * 0.05f;
			switch (m_bossType)
			{
			case 1:
			case 3:
				m_info[i].m_param.m_add_x *= 0.5f;
				break;
			}
			num4++;
			num5++;
		}
		//float num6 = (float)chaoAbilityValue * 0.01f (RC Pirate Spaceship fix)
		float num6 = (float)in_superRing * 0.01f;
		int a = (int)((float)m_count * num6);
		a = Mathf.Min(a, m_info.Length);
		for (int j = 0; j < a; j++)
		{
			m_info[j].m_type = Type.SUPER_RING;
		}
		if (IsEnableCreateTimer() && m_info.Length > 1)
		{
			int randomRange = ObjUtil.GetRandomRange100();
			int num7 = in_bronzeTimer + in_silverTimer;
			int num8 = num7 + in_goldTimer;
			if (randomRange < in_bronzeTimer)
			{
				m_info[m_info.Length - 2].m_type = Type.BRONZE_TIMER;
			}
			else if (randomRange < num7)
			{
				m_info[m_info.Length - 2].m_type = Type.SILVER_TIMER;
			}
			else if (randomRange < num8)
			{
				m_info[m_info.Length - 2].m_type = Type.GOLD_TIMER;
			}
		}
		int randomRange2 = ObjUtil.GetRandomRange100();
		//if (randomRange2 < chaoAbilityValue2) (RC Battle Cruiser fix)
		if (randomRange2 < num)
		{
			m_info[m_info.Length - 1].m_type = Type.REDSTAR_RING;
		}
		CreateRing(0, Mathf.Min(5, m_count));
	}

	private bool IsEnableCreateTimer()
	{
		if (StageModeManager.Instance != null && StageModeManager.Instance.IsQuickMode())
		{
			return ObjTimerUtil.IsEnableCreateTimer();
		}
		return false;
	}

	private void CreateRing(int startCount, int endCount)
	{
		if (endCount > m_info.Length)
		{
			return;
		}
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.OBJECT_PREFAB, "MotorThrow");
		if (!(gameObject != null))
		{
			return;
		}
		float num = m_add_player_speed * 0.2f;
		for (int i = startCount; i < endCount; i++)
		{
			string text = string.Empty;
			ResourceCategory category = ResourceCategory.UNKNOWN;
			uint type = (uint)m_info[i].m_type;
			if (type < OBJDATA_PARAMS.Length)
			{
				text = OBJDATA_PARAMS[type].m_name;
				category = OBJDATA_PARAMS[type].m_category;
			}
			GameObject gameObject2 = ObjUtil.GetChangeObject(ResourceManager.Instance, m_playerInformation, m_levelInformation, text);
			if (gameObject2 == null)
			{
				gameObject2 = ResourceManager.Instance.GetGameObject(category, text);
			}
			if (gameObject2 != null)
			{
				Vector3 vector = base.transform.position + new Vector3(1f + num, 0f, 0f);
				SpawnableObject reviveSpawnableObject = GetReviveSpawnableObject(gameObject2);
				GameObject gameObject3 = null;
				bool flag = false;
				if (reviveSpawnableObject != null)
				{
					SetRevivalSpawnableObject(reviveSpawnableObject, vector);
					m_rivivalObj.Add(reviveSpawnableObject);
					gameObject3 = reviveSpawnableObject.gameObject;
					flag = true;
				}
				else
				{
					gameObject3 = (Object.Instantiate(gameObject2, vector, base.transform.rotation) as GameObject);
				}
				GameObject gameObject4 = Object.Instantiate(gameObject, vector, base.transform.rotation) as GameObject;
				if ((bool)gameObject3 && (bool)gameObject4)
				{
					gameObject3.gameObject.SetActive(true);
					gameObject3.transform.parent = base.gameObject.transform;
					if (!flag)
					{
						reviveSpawnableObject = gameObject3.GetComponent<SpawnableObject>();
						reviveSpawnableObject.AttachModelObject();
						ObjTimerBase component = gameObject3.GetComponent<ObjTimerBase>();
						if (component != null)
						{
							component.SetMoveType(ObjTimerBase.MoveType.Bound);
						}
					}
					gameObject4.gameObject.SetActive(true);
					gameObject4.transform.parent = gameObject3.transform;
					MotorThrow component2 = gameObject4.GetComponent<MotorThrow>();
					if ((bool)component2)
					{
						MotorThrow.ThrowParam throwParam = new MotorThrow.ThrowParam();
						throwParam.m_obj = gameObject3;
						throwParam.m_speed = 6f;
						throwParam.m_gravity = -6.1f;
						throwParam.m_add_x = m_info[i].m_param.m_add_x + num;
						throwParam.m_add_y = m_info[i].m_param.m_add_y;
						throwParam.m_up = base.transform.up;
						throwParam.m_forward = base.transform.right;
						throwParam.m_rot_angle = Vector3.zero;
						throwParam.m_rot_speed = 0f;
						throwParam.m_rot_downspeed = 0f;
						throwParam.m_bound = true;
						throwParam.m_bound_pos_y = 0f;
						throwParam.m_bound_add_y = 1.8f + m_info[i].m_param.m_add_y * 1f;
						throwParam.m_bound_down_x = 0.5f;
						throwParam.m_bound_down_y = 0.01f;
						component2.Setup(throwParam);
					}
				}
			}
			m_createCount++;
		}
	}

	private void SetRevivalSpawnableObject(SpawnableObject spawnableObject, Vector3 pos)
	{
		if (spawnableObject != null)
		{
			spawnableObject.Sleep = false;
			spawnableObject.gameObject.transform.position = pos;
			spawnableObject.gameObject.transform.rotation = base.transform.rotation;
			spawnableObject.OnRevival();
		}
	}

	private SpawnableObject GetReviveSpawnableObject(GameObject srcObj)
	{
		if (srcObj == null)
		{
			return null;
		}
		SpawnableObject component = srcObj.GetComponent<SpawnableObject>();
		if (component == null)
		{
			return null;
		}
		if (m_stageBlockManager != null)
		{
			ObjectSpawnManager component2 = m_stageBlockManager.GetComponent<ObjectSpawnManager>();
			if (component2 != null && component.IsStockObject())
			{
				return component2.GetSpawnableObject(component.GetStockObjectType());
			}
		}
		return null;
	}
}
