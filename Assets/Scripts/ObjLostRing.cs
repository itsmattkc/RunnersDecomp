using System.Collections.Generic;
using UnityEngine;

public class ObjLostRing : MonoBehaviour
{
	private class LostRingParam
	{
		public float m_add_x;

		public float m_add_y;

		public LostRingParam(float add_x, float add_y)
		{
			m_add_x = add_x;
			m_add_y = add_y;
		}
	}

	private const float END_TIME = 1.2f;

	private const float RING_SPEED = 6f;

	private const float RING_GRAVITY = -6.1f;

	private const int RING_MAX = 6;

	private const float MAGNET_SPEED = 0.1f;

	private const float MAGNET_TIME = 0.2f;

	private const float MAGNET_ADDX = 0f;

	private const float MAGNET_ADDY = 0f;

	private int m_count;

	private int m_createCount;

	private float m_time;

	private static readonly LostRingParam[] LOSTRING_PARAM = new LostRingParam[10]
	{
		new LostRingParam(0.8f, 0.8f),
		new LostRingParam(0.5f, 1.2f),
		new LostRingParam(0.2f, 0.2f),
		new LostRingParam(0.4f, 0.4f),
		new LostRingParam(0.8f, 1.2f),
		new LostRingParam(0.5f, 0.5f),
		new LostRingParam(1.2f, 1.1f),
		new LostRingParam(0.6f, 0.5f),
		new LostRingParam(0.6f, 0.9f),
		new LostRingParam(1f, 1.3f)
	};

	private GameObject m_chaoObj;

	private List<GameObject> m_objList = new List<GameObject>();

	private float m_magnetSpeed;

	private bool m_magnetStart;

	private int m_ringCount;

	private void Start()
	{
		m_count = m_ringCount;
		if (m_count > 6)
		{
			m_count = 6;
		}
		if (m_count > 0 && m_count <= LOSTRING_PARAM.Length)
		{
			CreateRing(0, Mathf.Min(m_count, 3));
		}
		if (m_chaoObj != null)
		{
			ObjUtil.RequestStartAbilityToChao(ChaoAbility.RECOVERY_RING, false);
		}
	}

	private void Update()
	{
		if (m_createCount < m_count)
		{
			CreateRing(m_createCount, m_count);
			return;
		}
		m_time += Time.deltaTime;
		if (IsChaoMagnet())
		{
			if (!m_magnetStart)
			{
				if (m_time > 0.2f)
				{
					StartChaoMagnet();
					m_magnetStart = true;
				}
			}
			else if (UpdateChaoMagnet())
			{
				m_time = 2.4f;
			}
		}
		if (m_time > 1.2f)
		{
			if (m_magnetStart)
			{
				TakeChaoRing();
			}
			Object.Destroy(base.gameObject);
		}
	}

	private void CreateRing(int startCount, int endCount)
	{
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ObjRing.GetRingModelCategory(), ObjRing.GetRingModelName());
		GameObject gameObject2 = ResourceManager.Instance.GetGameObject(ResourceCategory.OBJECT_PREFAB, "MotorThrow");
		if (!(gameObject != null) || !(gameObject2 != null))
		{
			return;
		}
		for (int i = startCount; i < endCount; i++)
		{
			GameObject gameObject3 = Object.Instantiate(gameObject, base.transform.position, base.transform.rotation) as GameObject;
			GameObject gameObject4 = Object.Instantiate(gameObject2, base.transform.position, base.transform.rotation) as GameObject;
			if ((bool)gameObject3 && (bool)gameObject4)
			{
				gameObject3.gameObject.SetActive(true);
				gameObject3.transform.parent = base.gameObject.transform;
				gameObject3.transform.localRotation = Quaternion.Euler(Vector3.zero);
				gameObject4.gameObject.SetActive(true);
				gameObject4.transform.parent = gameObject3.transform;
				MotorThrow component = gameObject4.GetComponent<MotorThrow>();
				if ((bool)component)
				{
					MotorThrow.ThrowParam throwParam = new MotorThrow.ThrowParam();
					throwParam.m_obj = gameObject3;
					throwParam.m_speed = 6f;
					throwParam.m_gravity = -6.1f;
					throwParam.m_add_x = LOSTRING_PARAM[i].m_add_x;
					throwParam.m_add_y = LOSTRING_PARAM[i].m_add_y;
					throwParam.m_up = base.transform.up;
					throwParam.m_rot_speed = 0f;
					throwParam.m_rot_angle = Vector3.zero;
					throwParam.m_forward = -base.transform.right;
					if (IsChaoMagnet())
					{
						throwParam.m_add_x = throwParam.m_add_x;
						throwParam.m_add_y = throwParam.m_add_y;
					}
					component.Setup(throwParam);
				}
				m_objList.Add(gameObject3);
			}
			m_createCount++;
		}
	}

	public void SetChaoMagnet(GameObject chaoObj)
	{
		if (m_chaoObj == null)
		{
			m_chaoObj = chaoObj;
		}
	}

	public void SetRingCount(int ringCount)
	{
		m_ringCount = ringCount;
	}

	private bool IsChaoMagnet()
	{
		if (m_chaoObj != null)
		{
			return true;
		}
		return false;
	}

	private void StartChaoMagnet()
	{
		if (!(m_chaoObj != null))
		{
			return;
		}
		float num = ObjUtil.GetPlayerAddSpeed();
		if (num < 0f)
		{
			num = 0f;
		}
		m_magnetSpeed = 0.1f + 0.01f * num;
		foreach (GameObject obj in m_objList)
		{
			if (!obj)
			{
				continue;
			}
			for (int i = 0; i < obj.transform.childCount; i++)
			{
				GameObject gameObject = obj.transform.GetChild(i).gameObject;
				MotorThrow component = gameObject.GetComponent<MotorThrow>();
				if ((bool)component)
				{
					component.SetEnd();
					break;
				}
			}
		}
	}

	private bool UpdateChaoMagnet()
	{
		if (m_chaoObj == null)
		{
			return true;
		}
		bool result = true;
		foreach (GameObject obj in m_objList)
		{
			if ((bool)obj)
			{
				float num = 0.1f - m_time * m_magnetSpeed;
				if (num < 0.02f)
				{
					num = 0f;
				}
				else
				{
					result = false;
				}
				Vector3 currentVelocity = Vector3.zero;
				Vector3 position = m_chaoObj.transform.position;
				obj.transform.position = Vector3.SmoothDamp(obj.transform.position, position, ref currentVelocity, num);
			}
		}
		return result;
	}

	private void TakeChaoRing()
	{
		StageAbilityManager instance = StageAbilityManager.Instance;
		if ((bool)instance)
		{
			instance.SetLostRingCount(m_ringCount);
		}
	}
}
