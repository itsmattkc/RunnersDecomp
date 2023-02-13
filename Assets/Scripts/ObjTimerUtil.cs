using UnityEngine;

public class ObjTimerUtil
{
	private static readonly string[] OBJ_NAME = new string[3]
	{
		"ObjTimerBronze",
		"ObjTimerSilver",
		"ObjTimerGold"
	};

	private static readonly string[] MODEL_NAME = new string[3]
	{
		"obj_cmn_timeextenditem_copper",
		"obj_cmn_timeextenditem_silver",
		"obj_cmn_timeextenditem_gold"
	};

	private static readonly string[] EFFECT_NAME = new string[3]
	{
		"ef_ob_get_timeextend_copper",
		"ef_ob_get_timeextend_silver",
		"ef_ob_get_timeextend_gold"
	};

	private static readonly string[] SE_NAME = new string[3]
	{
		"obj_timer_bronze",
		"obj_timer_silver",
		"obj_timer_gold"
	};

	private static bool CheckType(TimerType type)
	{
		if (TimerType.BRONZE <= type && type < TimerType.NUM)
		{
			return true;
		}
		return false;
	}

	private static void CreateTimerObj(GameObject enm_obj, TimerType type)
	{
		if (!(enm_obj != null))
		{
			return;
		}
		string name = OBJ_NAME[(int)type];
		string name2 = MODEL_NAME[(int)type];
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.OBJECT_PREFAB, name);
		GameObject gameObject2 = ResourceManager.Instance.GetGameObject(ResourceCategory.OBJECT_RESOURCE, name2);
		if (!(gameObject != null) || !(gameObject2 != null))
		{
			return;
		}
		GameObject gameObject3 = Object.Instantiate(gameObject, enm_obj.transform.position, Quaternion.identity) as GameObject;
		GameObject gameObject4 = Object.Instantiate(gameObject2, gameObject3.transform.position, gameObject3.transform.rotation) as GameObject;
		if (gameObject3 != null && gameObject4 != null)
		{
			gameObject3.gameObject.SetActive(true);
			SphereCollider component = gameObject3.GetComponent<SphereCollider>();
			if (component != null)
			{
				component.enabled = false;
			}
			gameObject4.gameObject.SetActive(true);
			gameObject4.transform.parent = gameObject3.transform;
			gameObject4.transform.localRotation = Quaternion.identity;
		}
	}

	public static string GetObjectName(TimerType type)
	{
		if (CheckType(type))
		{
			return OBJ_NAME[(int)type];
		}
		return string.Empty;
	}

	public static string GetModelName(TimerType type)
	{
		if (CheckType(type))
		{
			return MODEL_NAME[(int)type];
		}
		return string.Empty;
	}

	public static string GetEffectName(TimerType type)
	{
		if (CheckType(type))
		{
			return EFFECT_NAME[(int)type];
		}
		return string.Empty;
	}

	public static string GetSEName(TimerType type)
	{
		if (CheckType(type))
		{
			return SE_NAME[(int)type];
		}
		return string.Empty;
	}

	public static void CreateTimer(GameObject enm_obj, TimerType type)
	{
		if (CheckType(type))
		{
			CreateTimerObj(enm_obj, type);
		}
	}

	public static bool IsEnableCreateTimer()
	{
		if (StageTimeManager.Instance != null)
		{
			return !StageTimeManager.Instance.IsReachedExtendedLimit();
		}
		return false;
	}
}
