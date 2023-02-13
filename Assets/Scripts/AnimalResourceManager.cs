using System.Collections.Generic;
using UnityEngine;

public class AnimalResourceManager : MonoBehaviour
{
	private class StockParam
	{
		public int m_stockCount;

		public ChaoAbility m_chaoAbility;

		public StockParam(int stockCount, ChaoAbility chaoAbility)
		{
			m_stockCount = stockCount;
			m_chaoAbility = chaoAbility;
		}
	}

	public class DepotObjs
	{
		private List<GameObject> m_objList = new List<GameObject>();

		public void Add(GameObject obj)
		{
			if (obj != null)
			{
				m_objList.Add(obj);
			}
		}

		public GameObject Get()
		{
			foreach (GameObject obj in m_objList)
			{
				ObjAnimalBase component = obj.GetComponent<ObjAnimalBase>();
				if (component != null && component.IsSleep())
				{
					component.Sleep = false;
					component.OnRevival();
					return obj;
				}
			}
			return null;
		}

		public void Sleep(GameObject obj)
		{
			if (obj != null)
			{
				ObjAnimalBase component = obj.GetComponent<ObjAnimalBase>();
				if (component != null)
				{
					component.Sleep = true;
				}
			}
		}
	}

	private static AnimalResourceManager s_instance = null;

	private static readonly StockParam[] STOCK_PARAM = new StockParam[8]
	{
		new StockParam(8, ChaoAbility.SPECIAL_ANIMAL),
		new StockParam(4, ChaoAbility.UNKNOWN),
		new StockParam(4, ChaoAbility.UNKNOWN),
		new StockParam(4, ChaoAbility.UNKNOWN),
		new StockParam(4, ChaoAbility.UNKNOWN),
		new StockParam(4, ChaoAbility.UNKNOWN),
		new StockParam(4, ChaoAbility.UNKNOWN),
		new StockParam(8, ChaoAbility.SPECIAL_ANIMAL_PSO2)
	};

	private Dictionary<AnimalType, DepotObjs> m_depotObjs = new Dictionary<AnimalType, DepotObjs>();

	public static AnimalResourceManager Instance
	{
		get
		{
			return s_instance;
		}
	}

	public GameObject GetStockAnimal(AnimalType type)
	{
		if (m_depotObjs.ContainsKey(type))
		{
			return m_depotObjs[type].Get();
		}
		return null;
	}

	public void SetSleep(AnimalType type, GameObject obj)
	{
		if (m_depotObjs.ContainsKey(type))
		{
			m_depotObjs[type].Sleep(obj);
		}
	}

	public void StockAnimalObject(bool bossStage)
	{
		if (bossStage)
		{
			return;
		}
		for (int i = 0; i < 8; i++)
		{
			if (!CheckAblity(STOCK_PARAM[i].m_chaoAbility))
			{
				continue;
			}
			AnimalType animalType = (AnimalType)i;
			GameObject gameObject = new GameObject(animalType.ToString());
			if (gameObject != null)
			{
				gameObject.transform.parent = base.gameObject.transform;
				DepotObjs depotObjs = new DepotObjs();
				for (int j = 0; j < STOCK_PARAM[i].m_stockCount; j++)
				{
					depotObjs.Add(ObjAnimalUtil.CreateStockAnimal(gameObject, animalType));
				}
				m_depotObjs.Add(animalType, depotObjs);
			}
		}
	}

	private bool CheckAblity(ChaoAbility ability)
	{
		if (ability == ChaoAbility.UNKNOWN)
		{
			return true;
		}
		if (StageAbilityManager.Instance != null)
		{
			return StageAbilityManager.Instance.HasChaoAbility(ability);
		}
		return false;
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
		if (s_instance == this)
		{
			s_instance = null;
		}
	}

	private void SetInstance()
	{
		if (s_instance == null)
		{
			s_instance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}
}
