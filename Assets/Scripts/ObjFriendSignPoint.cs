using System.Collections.Generic;
using UnityEngine;

public class ObjFriendSignPoint : SpawnableObject
{
	private const float m_offsetPosY = 2.3f;

	private const float m_offsetArea = 10f;

	private const float m_checkArea = 20f;

	private static int CREATE_COUNT = 5;

	private FriendSignPointData m_myData;

	private List<FriendSignCreateData> m_createDataList;

	private int m_createCount;

	private int m_createCountMax;

	private bool m_setupFind;

	private bool m_setup;

	private PlayerInformation m_playerInfo;

	private float m_setupDistance;

	private bool m_debugDraw;

	protected override void OnSpawned()
	{
	}

	private void Update()
	{
		if (!m_setupFind)
		{
			if (IsFindFriendSignPoint())
			{
				SetupFriendSign();
				m_setup = true;
			}
			else
			{
				m_setupDistance = GetTotalDistance();
				object[] obj = new object[4]
				{
					"IsFindFriendSignPoint NG 1 m_setupDistance=",
					m_setupDistance,
					" pos.x=",
					null
				};
				Vector3 position = base.transform.position;
				obj[3] = position.x;
				DebugDraw(string.Concat(obj));
			}
			m_setupFind = true;
		}
		if (!m_setup)
		{
			float num = GetTotalDistance() - m_setupDistance;
			if (num > 20f)
			{
				Vector3 position2 = base.transform.position;
				DebugDraw("IsFindFriendSignPoint OK pos.x=" + position2.x);
				SetupFriendSign();
				m_setup = true;
			}
		}
		if (!m_setup || m_createCountMax <= 0)
		{
			return;
		}
		int num2 = 0;
		foreach (FriendSignCreateData createData in m_createDataList)
		{
			if (!createData.m_create)
			{
				CreateObject(createData.m_texture);
				createData.m_create = true;
				num2++;
				if (num2 >= m_createCountMax)
				{
					break;
				}
			}
		}
		if (num2 == 0)
		{
			m_createCountMax = 0;
		}
	}

	private void SetupFriendSign()
	{
		m_createDataList = new List<FriendSignCreateData>();
		float totalDistance = GetTotalDistance();
		Vector3 playerPos = GetPlayerPos();
		float x = playerPos.x;
		List<GameObject> objList = new List<GameObject>();
		FindFriendSignPoint(ref objList);
		List<FriendSignPointData> list = new List<FriendSignPointData>();
		foreach (GameObject item in objList)
		{
			Vector3 position = item.transform.position;
			float num = position.x - x + totalDistance - 10f;
			if (num < 0f)
			{
				num = 0f;
			}
			float distance = num;
			Vector3 position2 = base.transform.position;
			float x2 = position2.x;
			Vector3 position3 = item.transform.position;
			FriendSignPointData friendSignPointData = new FriendSignPointData(item, distance, 0f, x2 == position3.x);
			list.Add(friendSignPointData);
			object[] obj = new object[6]
			{
				"ObjFriendSignPoint Data : my=",
				friendSignPointData.m_myPoint.ToString(),
				" distance=",
				friendSignPointData.m_distance,
				" pos.x=",
				null
			};
			Vector3 position4 = item.transform.position;
			obj[5] = position4.x;
			DebugDraw(string.Concat(obj));
		}
		list.Sort((FriendSignPointData d1, FriendSignPointData d2) => d2.m_distance.CompareTo(d1.m_distance));
		float num2 = 0f;
		foreach (FriendSignPointData item2 in list)
		{
			if (num2 == 0f)
			{
				num2 = item2.m_distance + 50f + 10f;
			}
			if (item2.m_myPoint)
			{
				m_myData = new FriendSignPointData(item2.m_obj, item2.m_distance, num2, item2.m_myPoint);
				object[] obj2 = new object[6]
				{
					"ObjFriendSignPoint myPoint :  distance=",
					m_myData.m_distance,
					" next=",
					m_myData.m_nextDistance,
					" pos.x=",
					null
				};
				Vector3 position5 = m_myData.m_obj.transform.position;
				obj2[5] = position5.x;
				DebugDraw(string.Concat(obj2));
				break;
			}
			num2 = item2.m_distance + 10f;
		}
		FriendSignManager instance = FriendSignManager.Instance;
		if ((bool)instance)
		{
			List<FriendSignData> friendSignDataList = instance.GetFriendSignDataList();
			foreach (FriendSignData item3 in friendSignDataList)
			{
				if (!item3.m_appear && AddFriendSignData(item3.m_distance, item3.m_texture))
				{
					instance.SetAppear(item3.m_index);
				}
			}
		}
		if (m_createDataList.Count > 0)
		{
			if (m_createDataList.Count >= CREATE_COUNT)
			{
				m_createCountMax = m_createDataList.Count / CREATE_COUNT + 1;
			}
			else
			{
				m_createCountMax = 1;
			}
		}
	}

	private bool AddFriendSignData(float friendDistance, Texture2D texture)
	{
		if (m_myData.m_distance <= friendDistance && friendDistance < m_myData.m_nextDistance)
		{
			m_createDataList.Add(new FriendSignCreateData(texture));
			return true;
		}
		return false;
	}

	private void CreateObject(Texture2D texture)
	{
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.OBJECT_PREFAB, "ObjFriendSign");
		if (!gameObject)
		{
			return;
		}
		float num = 2.3f * (float)m_createCount;
		Vector3 position = base.transform.position;
		float x = position.x;
		Vector3 position2 = base.transform.position;
		float y = position2.y + num;
		Vector3 position3 = base.transform.position;
		Vector3 position4 = new Vector3(x, y, position3.z);
		GameObject gameObject2 = Object.Instantiate(gameObject, position4, base.transform.rotation) as GameObject;
		if ((bool)gameObject2)
		{
			gameObject2.SetActive(true);
			gameObject2.transform.parent = base.transform;
			SpawnableObject component = gameObject2.GetComponent<SpawnableObject>();
			if ((bool)component)
			{
				component.AttachModelObject();
			}
			BoxCollider component2 = gameObject2.GetComponent<BoxCollider>();
			if ((bool)component2)
			{
				Vector3 center = component2.center;
				float x2 = center.x;
				Vector3 center2 = component2.center;
				float y2 = center2.y - num;
				Vector3 center3 = component2.center;
				component2.center = new Vector3(x2, y2, center3.z);
			}
			ObjFriendSign component3 = gameObject2.GetComponent<ObjFriendSign>();
			if ((bool)component3)
			{
				component3.ChangeTexture(texture);
			}
			m_createCount++;
		}
	}

	private void FindFriendSignPoint(ref List<GameObject> objList)
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("FriendSign");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			Vector3 position = base.transform.position;
			float x = position.x;
			Vector3 position2 = gameObject.transform.position;
			if (x <= position2.x)
			{
				objList.Add(gameObject);
			}
		}
	}

	private bool IsFindFriendSignPoint()
	{
		List<GameObject> objList = new List<GameObject>();
		FindFriendSignPoint(ref objList);
		if (objList.Count > 1)
		{
			return true;
		}
		return false;
	}

	private float GetTotalDistance()
	{
		PlayerInformation playerInfo = GetPlayerInfo();
		if (playerInfo != null)
		{
			return playerInfo.TotalDistance;
		}
		return 0f;
	}

	private Vector3 GetPlayerPos()
	{
		PlayerInformation playerInfo = GetPlayerInfo();
		if (playerInfo != null)
		{
			return playerInfo.Position;
		}
		return Vector3.zero;
	}

	private PlayerInformation GetPlayerInfo()
	{
		if (m_playerInfo == null)
		{
			m_playerInfo = ObjUtil.GetPlayerInformation();
		}
		return m_playerInfo;
	}

	private void DebugDraw(string msg)
	{
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(base.transform.position, 0.5f);
	}
}
