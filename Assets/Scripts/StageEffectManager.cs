using SaveData;
using System.Collections.Generic;
using UnityEngine;

public class StageEffectManager : MonoBehaviour
{
	public enum EffectCategory
	{
		CHARACTER,
		ENEMY,
		OBJECT,
		SP_EVENT
	}

	public class EffectData
	{
		public EffectCategory m_category;

		public float m_endTime;

		public int m_stockCount;

		public string m_name;

		public bool m_bossStage;

		public EffectData(EffectCategory category, string name, float endTime, int stockCount, bool bossStage)
		{
			m_category = category;
			m_name = name;
			m_endTime = endTime;
			m_stockCount = stockCount;
			m_bossStage = bossStage;
		}
	}

	public class DepotObjs
	{
		private List<GameObject> m_objList = new List<GameObject>();

		private GameObject m_folderObj;

		public DepotObjs(GameObject parentObj, string folderName)
		{
			m_folderObj = new GameObject();
			if (m_folderObj != null)
			{
				m_folderObj.name = folderName;
				if (parentObj != null)
				{
					m_folderObj.transform.parent = parentObj.transform;
				}
			}
		}

		public void Add(GameObject obj)
		{
			if (obj != null)
			{
				m_objList.Add(obj);
				if (m_folderObj != null)
				{
					obj.transform.parent = m_folderObj.transform;
				}
			}
		}

		public GameObject Get()
		{
			foreach (GameObject obj in m_objList)
			{
				EffectShareState component = obj.GetComponent<EffectShareState>();
				if (component != null && component.IsSleep())
				{
					component.SetState(EffectShareState.State.Active);
					return obj;
				}
			}
			return null;
		}

		public void Sleep(GameObject obj)
		{
			if (!(obj != null))
			{
				return;
			}
			EffectShareState component = obj.GetComponent<EffectShareState>();
			if (component != null)
			{
				component.SetState(EffectShareState.State.Sleep);
				obj.SetActive(false);
				if (m_folderObj != null)
				{
					obj.transform.parent = m_folderObj.transform;
				}
			}
		}
	}

	private static StageEffectManager instance;

	private EffectData[] EFFECT_DATA_TBL = new EffectData[19]
	{
		new EffectData(EffectCategory.CHARACTER, "ef_pl_fog_jump_st01", 1f, 3, true),
		new EffectData(EffectCategory.CHARACTER, "ef_pl_fog_jump_ed01", 1f, 3, true),
		new EffectData(EffectCategory.CHARACTER, "ef_pl_fog_run01", 1.5f, 8, true),
		new EffectData(EffectCategory.CHARACTER, "ef_pl_fog_speedrun01", 1.5f, 8, true),
		new EffectData(EffectCategory.ENEMY, "ef_en_dead_s01", 1f, 10, false),
		new EffectData(EffectCategory.ENEMY, "ef_en_dead_m01", 1f, 10, false),
		new EffectData(EffectCategory.ENEMY, "ef_en_dead_l01", 1f, 10, false),
		new EffectData(EffectCategory.ENEMY, "ef_en_guard01", 1f, 3, false),
		new EffectData(EffectCategory.ENEMY, "ef_com_explosion_m01", 1f, 10, false),
		new EffectData(EffectCategory.ENEMY, "ef_com_explosion_s01", 1f, 10, false),
		new EffectData(EffectCategory.OBJECT, "ef_ob_get_ring01", 1f, 10, true),
		new EffectData(EffectCategory.OBJECT, "ef_ob_get_superring01", 1f, 10, true),
		new EffectData(EffectCategory.OBJECT, "ef_ob_get_animal01", 1f, 6, false),
		new EffectData(EffectCategory.OBJECT, "ef_ob_get_crystal_s01", 1f, 10, false),
		new EffectData(EffectCategory.OBJECT, "ef_ob_get_crystal_gr_s01", 1f, 10, false),
		new EffectData(EffectCategory.OBJECT, "ef_ob_get_crystal_rd_s01", 1f, 10, false),
		new EffectData(EffectCategory.OBJECT, "ef_ob_get_crystal_l01", 1f, 10, false),
		new EffectData(EffectCategory.OBJECT, "ef_ob_get_crystal_gr_l01", 1f, 10, false),
		new EffectData(EffectCategory.OBJECT, "ef_ob_get_crystal_rd_l01", 1f, 10, false)
	};

	[SerializeField]
	private GameObject m_charaParentObj;

	[SerializeField]
	private GameObject m_enemyParentObj;

	[SerializeField]
	private GameObject m_objectParentObj;

	[SerializeField]
	private GameObject m_specialEventParentObj;

	private Dictionary<EffectPlayType, DepotObjs> m_depotObjs = new Dictionary<EffectPlayType, DepotObjs>();

	private bool m_lightMode;

	public static StageEffectManager Instance
	{
		get
		{
			return instance;
		}
	}

	public void StockStageEffect(bool bossStage)
	{
		if (SystemSaveManager.GetSystemSaveData() != null)
		{
			m_lightMode = SystemSaveManager.GetSystemSaveData().lightMode;
		}
		if (m_lightMode || !(ResourceManager.Instance != null))
		{
			return;
		}
		for (int i = 0; i < EFFECT_DATA_TBL.Length; i++)
		{
			bool flag = false;
			EffectData effectData = EFFECT_DATA_TBL[i];
			if (bossStage && !effectData.m_bossStage)
			{
				continue;
			}
			EffectPlayType effectPlayType = (EffectPlayType)i;
			GameObject parentObj = m_charaParentObj;
			switch (effectData.m_category)
			{
			case EffectCategory.ENEMY:
				parentObj = m_enemyParentObj;
				break;
			case EffectCategory.OBJECT:
				parentObj = m_objectParentObj;
				break;
			case EffectCategory.SP_EVENT:
				parentObj = m_specialEventParentObj;
				flag = true;
				break;
			}
			DepotObjs depotObjs = new DepotObjs(parentObj, effectPlayType.ToString());
			for (int j = 0; j < effectData.m_stockCount; j++)
			{
				ResourceCategory category = (!flag) ? ResourceCategory.COMMON_EFFECT : ResourceCategory.EVENT_RESOURCE;
				GameObject gameObject = ResourceManager.Instance.GetGameObject(category, effectData.m_name);
				if (!gameObject)
				{
					continue;
				}
				GameObject gameObject2 = Object.Instantiate(gameObject, Vector3.zero, Quaternion.identity) as GameObject;
				if (!(gameObject2 != null))
				{
					continue;
				}
				gameObject2.name = effectData.m_name;
				gameObject2.SetActive(false);
				EffectShareState effectShareState = gameObject2.AddComponent<EffectShareState>();
				if (effectShareState != null)
				{
					effectShareState.m_effectType = effectPlayType;
				}
				ykKillTime component = gameObject2.GetComponent<ykKillTime>();
				if (component != null)
				{
					component.enabled = false;
				}
				EffectPlayTime component2 = gameObject2.GetComponent<EffectPlayTime>();
				if (component2 == null)
				{
					component2 = gameObject2.AddComponent<EffectPlayTime>();
					if (component2 != null)
					{
						component2.m_endTime = effectData.m_endTime;
					}
				}
				depotObjs.Add(gameObject2);
			}
			m_depotObjs.Add(effectPlayType, depotObjs);
		}
	}

	public void PlayEffect(EffectPlayType type, Vector3 pos, Quaternion rot)
	{
		if (m_lightMode)
		{
			return;
		}
		if (m_depotObjs.ContainsKey(type))
		{
			GameObject gameObject = m_depotObjs[type].Get();
			if (gameObject != null)
			{
				gameObject.transform.position = pos;
				gameObject.transform.rotation = rot;
				EffectPlayTime component = gameObject.GetComponent<EffectPlayTime>();
				if (component != null)
				{
					component.PlayEffect();
					return;
				}
			}
		}
		if (IsCreateEffect(type))
		{
			ObjUtil.PlayEffect(EFFECT_DATA_TBL[(int)type].m_name, pos, rot, EFFECT_DATA_TBL[(int)type].m_endTime);
		}
	}

	private bool IsCreateEffect(EffectPlayType type)
	{
		if (EffectPlayType.ENEMY_S <= type && type <= EffectPlayType.AIR_TRAP)
		{
			return true;
		}
		return false;
	}

	public void SleepEffect(GameObject obj)
	{
		if (!m_lightMode && obj != null)
		{
			EffectShareState component = obj.GetComponent<EffectShareState>();
			if (component != null && m_depotObjs.ContainsKey(component.m_effectType))
			{
				m_depotObjs[component.m_effectType].Sleep(obj);
			}
		}
	}

	private void Awake()
	{
		SetInstance();
	}

	private void Start()
	{
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
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
