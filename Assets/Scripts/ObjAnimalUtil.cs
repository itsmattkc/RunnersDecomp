using System.Collections.Generic;
using UnityEngine;

public class ObjAnimalUtil
{
	private class AnimalParam
	{
		public ResourceCategory m_category;

		public string m_model;

		public int m_count;

		public AnimalMoveType m_moveType;

		public ChaoAbility m_chaoAbility;

		public AnimalParam(ResourceCategory category, string model, int count, AnimalMoveType moveType, ChaoAbility chaoAbility)
		{
			m_category = category;
			m_model = model;
			m_count = count;
			m_moveType = moveType;
			m_chaoAbility = chaoAbility;
		}
	}

	private static readonly AnimalParam[] ANIMAL_PARAM = new AnimalParam[8]
	{
		new AnimalParam(ResourceCategory.ENEMY_RESOURCE, "ani_flicky", 6, AnimalMoveType.FLY, ChaoAbility.SPECIAL_ANIMAL),
		new AnimalParam(ResourceCategory.ENEMY_RESOURCE, "ani_picky", 1, AnimalMoveType.JUMP, ChaoAbility.UNKNOWN),
		new AnimalParam(ResourceCategory.ENEMY_RESOURCE, "ani_pecky", 1, AnimalMoveType.JUMP, ChaoAbility.UNKNOWN),
		new AnimalParam(ResourceCategory.ENEMY_RESOURCE, "ani_rocky", 1, AnimalMoveType.JUMP, ChaoAbility.UNKNOWN),
		new AnimalParam(ResourceCategory.ENEMY_RESOURCE, "ani_ricky", 1, AnimalMoveType.JUMP, ChaoAbility.UNKNOWN),
		new AnimalParam(ResourceCategory.ENEMY_RESOURCE, "ani_cookie", 1, AnimalMoveType.JUMP, ChaoAbility.UNKNOWN),
		new AnimalParam(ResourceCategory.ENEMY_RESOURCE, "ani_pocky", 1, AnimalMoveType.JUMP, ChaoAbility.UNKNOWN),
		new AnimalParam(ResourceCategory.CHAO_MODEL, "ani_rappy", 6, AnimalMoveType.FLY, ChaoAbility.SPECIAL_ANIMAL_PSO2)
	};

	private static readonly AnimalType[] NORMALTYPE_TABLE = new AnimalType[6]
	{
		AnimalType.PICKY,
		AnimalType.PECKY,
		AnimalType.ROCKY,
		AnimalType.RICKY,
		AnimalType.COOKIE,
		AnimalType.POCKY
	};

	private static readonly string[] MOVECOMP_NAMES = new string[2]
	{
		"ObjAnimalFly",
		"ObjAnimalJump"
	};

	private static Vector3 ModelLocalRotation = new Vector3(0f, 180f, 0f);

	private static string GetMoveCompName(AnimalMoveType moveType)
	{
		if ((uint)moveType < MOVECOMP_NAMES.Length)
		{
			return MOVECOMP_NAMES[(int)moveType];
		}
		return string.Empty;
	}

	private static int GetChaoAbilityAnimalCount(ChaoAbility ability, int defaultCount)
	{
		int num = 0;
		if (StageAbilityManager.Instance != null)
		{
			num = (int)StageAbilityManager.Instance.GetChaoAbilityExtraValue(ability, ChaoType.MAIN);
			if (num == 0)
			{
				num = (int)StageAbilityManager.Instance.GetChaoAbilityExtraValue(ability, ChaoType.SUB);
			}
		}
		if (num == 0)
		{
			num = defaultCount;
		}
		return num;
	}

	public static void CreateAnimal(GameObject enm_obj, AnimalType type)
	{
		if ((uint)type >= ANIMAL_PARAM.Length || !enm_obj)
		{
			return;
		}
		string moveCompName = GetMoveCompName(ANIMAL_PARAM[(int)type].m_moveType);
		string model = ANIMAL_PARAM[(int)type].m_model;
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.ENEMY_PREFAB, moveCompName);
		GameObject gameObject2 = ResourceManager.Instance.GetGameObject(ANIMAL_PARAM[(int)type].m_category, model);
		if (!(gameObject != null) || !(gameObject2 != null))
		{
			return;
		}
		GameObject gameObject3 = Object.Instantiate(gameObject, enm_obj.transform.position, Quaternion.identity) as GameObject;
		GameObject gameObject4 = Object.Instantiate(gameObject2, gameObject3.transform.position, gameObject3.transform.rotation) as GameObject;
		if (!(gameObject3 != null) || !(gameObject4 != null))
		{
			return;
		}
		gameObject3.gameObject.SetActive(true);
		SphereCollider component = gameObject3.GetComponent<SphereCollider>();
		if (component != null)
		{
			component.enabled = false;
		}
		gameObject4.gameObject.SetActive(true);
		gameObject4.transform.parent = gameObject3.transform;
		gameObject4.transform.localRotation = Quaternion.Euler(ModelLocalRotation);
		int animalAddCount = (ANIMAL_PARAM[(int)type].m_chaoAbility != ChaoAbility.UNKNOWN) ? GetChaoAbilityAnimalCount(ANIMAL_PARAM[(int)type].m_chaoAbility, ANIMAL_PARAM[(int)type].m_count) : ANIMAL_PARAM[(int)type].m_count;
		if (ANIMAL_PARAM[(int)type].m_moveType == AnimalMoveType.FLY)
		{
			ObjAnimalFly component2 = gameObject3.GetComponent<ObjAnimalFly>();
			if (component2 != null)
			{
				component2.SetAnimalAddCount(animalAddCount);
			}
		}
		else
		{
			ObjAnimalJump component3 = gameObject3.GetComponent<ObjAnimalJump>();
			if (component3 != null)
			{
				component3.SetAnimalAddCount(animalAddCount);
			}
		}
		switch (type)
		{
		case AnimalType.FLICKY:
			ObjUtil.RequestStartAbilityToChao(ChaoAbility.SPECIAL_ANIMAL, true);
			break;
		case AnimalType.PSO2_1:
			ObjUtil.RequestStartAbilityToChao(ChaoAbility.SPECIAL_ANIMAL_PSO2, true);
			break;
		}
	}

	public static void CreateAnimal(GameObject enm_obj)
	{
		AnimalType animalType = AnimalType.ERROR;
		if (StageAbilityManager.Instance != null)
		{
			List<AnimalType> list = new List<AnimalType>();
			if (StageAbilityManager.Instance.HasChaoAbility(ChaoAbility.SPECIAL_ANIMAL))
			{
				int num = (int)StageAbilityManager.Instance.GetChaoAbilityValue(ChaoAbility.SPECIAL_ANIMAL);
				if (num >= ObjUtil.GetRandomRange100())
				{
					list.Add(AnimalType.FLICKY);
				}
			}
			if (StageAbilityManager.Instance.HasChaoAbility(ChaoAbility.SPECIAL_ANIMAL_PSO2))
			{
				int num2 = (int)StageAbilityManager.Instance.GetChaoAbilityValue(ChaoAbility.SPECIAL_ANIMAL_PSO2);
				if (num2 >= ObjUtil.GetRandomRange100())
				{
					list.Add(AnimalType.PSO2_1);
				}
			}
			if (list.Count == 1)
			{
				animalType = list[0];
			}
			else if (list.Count >= 2)
			{
				int index = Random.Range(0, list.Count);
				animalType = list[index];
			}
		}
		if (animalType == AnimalType.ERROR)
		{
			int num3 = Random.Range(0, NORMALTYPE_TABLE.Length);
			animalType = NORMALTYPE_TABLE[num3];
		}
		if (AnimalResourceManager.Instance != null)
		{
			GameObject stockAnimal = AnimalResourceManager.Instance.GetStockAnimal(animalType);
			if (stockAnimal != null)
			{
				stockAnimal.transform.position = enm_obj.transform.position;
				stockAnimal.transform.rotation = Quaternion.identity;
				stockAnimal.SetActive(true);
				switch (animalType)
				{
				case AnimalType.FLICKY:
					ObjUtil.RequestStartAbilityToChao(ChaoAbility.SPECIAL_ANIMAL, true);
					break;
				case AnimalType.PSO2_1:
					ObjUtil.RequestStartAbilityToChao(ChaoAbility.SPECIAL_ANIMAL_PSO2, true);
					break;
				}
				return;
			}
		}
		CreateAnimal(enm_obj, animalType);
	}

	public static GameObject CreateStockAnimal(GameObject parentObj, AnimalType type)
	{
		if ((uint)type >= ANIMAL_PARAM.Length)
		{
			return null;
		}
		if (parentObj != null)
		{
			string moveCompName = GetMoveCompName(ANIMAL_PARAM[(int)type].m_moveType);
			string model = ANIMAL_PARAM[(int)type].m_model;
			GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.ENEMY_PREFAB, moveCompName);
			GameObject gameObject2 = ResourceManager.Instance.GetGameObject(ANIMAL_PARAM[(int)type].m_category, model);
			if (gameObject != null && gameObject2 != null)
			{
				GameObject gameObject3 = Object.Instantiate(gameObject, Vector3.zero, Quaternion.identity) as GameObject;
				GameObject gameObject4 = Object.Instantiate(gameObject2, Vector3.zero, Quaternion.identity) as GameObject;
				if (gameObject3 != null && gameObject4 != null)
				{
					gameObject3.name = moveCompName;
					gameObject3.gameObject.SetActive(false);
					gameObject3.gameObject.transform.parent = parentObj.transform;
					SphereCollider component = gameObject3.GetComponent<SphereCollider>();
					if (component != null)
					{
						component.enabled = false;
					}
					gameObject4.name = model;
					gameObject4.gameObject.SetActive(true);
					gameObject4.transform.parent = gameObject3.transform;
					gameObject4.transform.localRotation = Quaternion.Euler(ModelLocalRotation);
					int animalAddCount = (ANIMAL_PARAM[(int)type].m_chaoAbility != ChaoAbility.UNKNOWN) ? GetChaoAbilityAnimalCount(ANIMAL_PARAM[(int)type].m_chaoAbility, ANIMAL_PARAM[(int)type].m_count) : ANIMAL_PARAM[(int)type].m_count;
					if (ANIMAL_PARAM[(int)type].m_moveType == AnimalMoveType.FLY)
					{
						ObjAnimalFly component2 = gameObject3.GetComponent<ObjAnimalFly>();
						if (component2 != null)
						{
							component2.SetAnimalAddCount(animalAddCount);
							component2.SetShareState(type);
						}
					}
					else
					{
						ObjAnimalJump component3 = gameObject3.GetComponent<ObjAnimalJump>();
						if (component3 != null)
						{
							component3.SetAnimalAddCount(animalAddCount);
							component3.SetShareState(type);
						}
					}
					switch (type)
					{
					case AnimalType.FLICKY:
						ObjUtil.RequestStartAbilityToChao(ChaoAbility.SPECIAL_ANIMAL, false);
						break;
					case AnimalType.PSO2_1:
						ObjUtil.RequestStartAbilityToChao(ChaoAbility.SPECIAL_ANIMAL_PSO2, false);
						break;
					}
					return gameObject3;
				}
			}
		}
		return null;
	}
}
