using Message;
using UnityEngine;

public class ObjBossWisp : MonoBehaviour
{
	private const string MODEL_NAME = "obj_raid_wisp";

	private const ResourceCategory MODEL_CATEGORY = ResourceCategory.EVENT_RESOURCE;

	private const float WAIT_END_TIME = 7f;

	private const float ADD_SPEED = 0.12f;

	public static string EFFECT_NAME = "ef_raid_wisp_get01";

	public static string SE_NAME = "rb_wisp_get";

	private float m_speed = 0.5f;

	private float m_distance = 1f;

	private float m_addX = 1f;

	private float m_time;

	private float m_move_speed;

	private GameObject m_objBoss;

	private static Vector3 ModelLocalRotation = new Vector3(0f, 180f, 0f);

	public void Setup(GameObject objBoss, float speed, float distance, float addX)
	{
		m_objBoss = objBoss;
		m_speed = speed;
		m_distance = distance;
		m_addX = addX;
		m_move_speed = 0.12f * ObjUtil.GetPlayerAddSpeed();
		CreateModel();
		MotorAnimalFly component = GetComponent<MotorAnimalFly>();
		if ((bool)component)
		{
			component.SetupParam(m_speed, m_distance, m_addX + m_move_speed, base.transform.right, 0f, false);
		}
	}

	private void Update()
	{
		m_time += Time.deltaTime;
		if (m_time > 7f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void EndMotorComponent()
	{
		MotorAnimalFly component = GetComponent<MotorAnimalFly>();
		if ((bool)component)
		{
			component.enabled = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other)
		{
			return;
		}
		GameObject gameObject = other.gameObject;
		if ((bool)gameObject)
		{
			string a = LayerMask.LayerToName(gameObject.layer);
			if (a == "Player" || a == "Chao")
			{
				TakeWisp();
			}
			else if (!(a == "Magnet"))
			{
			}
		}
	}

	private void OnDrawingRings(MsgOnDrawingRings msg)
	{
		ObjUtil.StartMagnetControl(base.gameObject);
		EndMotorComponent();
		m_time = 0f;
	}

	private void TakeWisp()
	{
		if (m_objBoss != null)
		{
			m_objBoss.SendMessage("OnGetWisp", SendMessageOptions.DontRequireReceiver);
		}
		ObjUtil.PlayEffect(EFFECT_NAME, ObjUtil.GetCollisionCenterPosition(base.gameObject), base.transform.rotation, 1f);
		ObjUtil.PlayEventSE(SE_NAME, EventManager.EventType.RAID_BOSS);
		Object.Destroy(base.gameObject);
	}

	private void CreateModel()
	{
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.EVENT_RESOURCE, "obj_raid_wisp");
		if ((bool)gameObject)
		{
			GameObject gameObject2 = Object.Instantiate(gameObject, base.transform.position, base.transform.rotation) as GameObject;
			if ((bool)gameObject2)
			{
				gameObject2.gameObject.SetActive(true);
				gameObject2.transform.parent = base.gameObject.transform;
				gameObject2.transform.localRotation = Quaternion.Euler(ModelLocalRotation);
			}
		}
	}
}
