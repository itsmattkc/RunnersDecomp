using Message;
using System.Collections.Generic;
using UnityEngine;

public class ObjTrampolineFloorCollision : SpawnableObject
{
	private const int CREATE_COUNT = 5;

	private ObjTrampolineFloorCollisionParameter m_param;

	private List<ObjCreateData> m_dataList = new List<ObjCreateData>();

	private int m_createCountMax;

	private bool m_end;

	protected override void OnSpawned()
	{
		if (!m_end && m_createCountMax == 0 && StageItemManager.Instance != null && StageItemManager.Instance.IsActiveTrampoline())
		{
			CreateTrampolineFloor();
		}
	}

	private void Update()
	{
		if (m_createCountMax <= 0 || m_dataList == null)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < m_dataList.Count; i++)
		{
			ObjCreateData objCreateData = m_dataList[i];
			if (objCreateData.m_obj == null && !objCreateData.m_create)
			{
				objCreateData.m_create = true;
				objCreateData.m_obj = CreateObject(objCreateData.m_src, objCreateData.m_pos, objCreateData.m_rot);
				num++;
				if (num >= m_createCountMax)
				{
					break;
				}
			}
		}
		if (num == 0)
		{
			m_createCountMax = 0;
		}
	}

	public void SetObjCollisionParameter(ObjTrampolineFloorCollisionParameter param)
	{
		m_param = param;
	}

	public void OnTransformPhantom(MsgTransformPhantom msg)
	{
		if (m_dataList != null)
		{
			foreach (ObjCreateData data in m_dataList)
			{
				if ((bool)data.m_obj)
				{
					Object.Destroy(data.m_obj);
				}
			}
			m_dataList.Clear();
		}
		m_end = false;
	}

	private void OnUseItem(MsgUseItem item)
	{
		if (!m_end && item.m_itemType == ItemType.TRAMPOLINE && m_createCountMax == 0)
		{
			CreateTrampolineFloor();
		}
	}

	private void CreateTrampolineFloor()
	{
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.OBJECT_PREFAB, "ObjTrampolineFloor");
		if ((bool)gameObject)
		{
			BoxCollider component = GetComponent<BoxCollider>();
			if ((bool)component)
			{
				Vector3 size = component.size;
				int num = Mathf.FloorToInt(size.x);
				float num2 = 1f;
				float num3 = 0f;
				float num4 = num2 / 2f;
				if (num % 2 != 0)
				{
					AddObject(gameObject, base.transform.position, base.transform.rotation);
					num3 = num4;
				}
				int num5 = num / 2;
				for (int i = 0; i < num5; i++)
				{
					float d = num4 + (float)i * num2 + num3;
					Vector3 pos = base.transform.position + base.transform.right * d;
					AddObject(gameObject, pos, base.transform.rotation);
					Vector3 pos2 = base.transform.position + -base.transform.right * d;
					AddObject(gameObject, pos2, base.transform.rotation);
				}
				m_createCountMax = Mathf.Min(m_dataList.Count, 5);
			}
		}
		m_end = true;
	}

	private void AddObject(GameObject src, Vector3 pos, Quaternion rot)
	{
		m_dataList.Add(new ObjCreateData(src, pos, rot));
	}

	private GameObject CreateObject(GameObject src, Vector3 pos, Quaternion rot)
	{
		GameObject gameObject = Object.Instantiate(src, pos, rot) as GameObject;
		if ((bool)gameObject)
		{
			gameObject.SetActive(true);
			gameObject.transform.parent = base.transform;
			SpawnableObject component = gameObject.GetComponent<SpawnableObject>();
			if ((bool)component)
			{
				component.AttachModelObject();
			}
			if (m_param != null)
			{
				ObjTrampolineFloor component2 = gameObject.GetComponent<ObjTrampolineFloor>();
				if ((bool)component2)
				{
					component2.SetParam(m_param.m_firstSpeed, m_param.m_outOfcontrol);
				}
			}
		}
		return gameObject;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(base.transform.position, 0.5f);
	}
}
