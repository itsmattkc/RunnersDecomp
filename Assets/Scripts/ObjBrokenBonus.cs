using UnityEngine;

public class ObjBrokenBonus : MonoBehaviour
{
	private const float END_TIME = 2f;

	private const float MOVE_SPEED = 3f;

	private const float ROT_SPEED = 20f;

	private static readonly string[] OBJ_MODELNAMES = new string[3]
	{
		ObjSuperRing.ModelName,
		ObjRedStarRing.ModelName,
		string.Empty
	};

	private static readonly string[] OBJ_SENAMES = new string[3]
	{
		ObjSuperRing.SEName,
		ObjRedStarRing.SEName,
		string.Empty
	};

	private float m_time;

	private BrokenBonusType m_type;

	private PlayerInformation m_playerInfo;

	private GameObject m_obj;

	private CtystalType m_ctystalType = CtystalType.BIG_C;

	private void Start()
	{
		m_playerInfo = ObjUtil.GetPlayerInformation();
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		float d = 3f * deltaTime;
		base.transform.position += Vector3.up * d;
		if (m_type == BrokenBonusType.SUPER_RING)
		{
			ObjUtil.SetTextureAnimationSpeed(m_obj, 20f);
		}
		else
		{
			float y = 60f * deltaTime * 20f;
			base.transform.rotation = Quaternion.Euler(0f, y, 0f) * base.transform.rotation;
		}
		m_time += deltaTime;
		if (m_time > 2f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void Setup(BrokenBonusType type, GameObject playerObject)
	{
		m_type = type;
		uint type2 = (uint)m_type;
		if (type2 >= OBJ_MODELNAMES.Length)
		{
			return;
		}
		string name = string.Empty;
		string name2 = string.Empty;
		if (m_type == BrokenBonusType.CRYSTAL10)
		{
			CtystalParam crystalParam = ObjCrystalData.GetCrystalParam(m_ctystalType);
			if (crystalParam != null)
			{
				name = crystalParam.m_model;
				name2 = crystalParam.m_se;
			}
			ObjUtil.LightPlaySE(name2);
		}
		else
		{
			name = OBJ_MODELNAMES[type2];
			if (type2 < OBJ_SENAMES.Length)
			{
				name2 = OBJ_SENAMES[type2];
			}
			ObjUtil.PlaySE(name2);
		}
		m_obj = CreateBonus(name);
		switch (type)
		{
		case BrokenBonusType.SUPER_RING:
			ObjSuperRing.AddSuperRing(playerObject);
			break;
		case BrokenBonusType.REDSTAR_RING:
			ObjUtil.SendMessageAddRedRing();
			ObjUtil.SendMessageScoreCheck(new StageScoreData(8, 1));
			break;
		case BrokenBonusType.CRYSTAL10:
		{
			CtystalParam crystalParam2 = ObjCrystalData.GetCrystalParam(m_ctystalType);
			if (crystalParam2 != null)
			{
				ObjCrystalBase.SetChaoAbliltyScoreEffect(m_playerInfo, crystalParam2, m_obj);
			}
			break;
		}
		}
	}

	private GameObject CreateBonus(string name)
	{
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.OBJECT_RESOURCE, name);
		if (gameObject != null)
		{
			GameObject gameObject2 = Object.Instantiate(gameObject, Vector3.zero, Quaternion.identity) as GameObject;
			if ((bool)gameObject2)
			{
				gameObject2.gameObject.SetActive(true);
				gameObject2.transform.parent = base.gameObject.transform;
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.transform.localRotation = Quaternion.Euler(Vector3.zero);
				return gameObject2;
			}
		}
		return null;
	}
}
