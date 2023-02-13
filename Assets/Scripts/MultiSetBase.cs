using System.Collections.Generic;
using UnityEngine;

public class MultiSetBase : SpawnableObject
{
	private static int CREATE_COUNT = 5;

	protected List<ObjCreateData> m_dataList;

	private int m_createCountMax;

	private PlayerInformation m_playerInformation;

	private LevelInformation m_levelInformation;

	protected override void OnSpawned()
	{
	}

	protected virtual void OnCreateSetup()
	{
	}

	protected virtual void UpdateLocal()
	{
	}

	protected override void OnDestroyed()
	{
		for (int i = 0; i < m_dataList.Count; i++)
		{
			if (m_dataList[i].m_obj != null)
			{
				SpawnableObject component = m_dataList[i].m_obj.GetComponent<SpawnableObject>();
				if (component != null && component.Share)
				{
					SetSleep(m_dataList[i].m_obj);
				}
			}
		}
	}

	private void Update()
	{
		if (m_createCountMax > 0 && m_dataList != null)
		{
			int num = 0;
			for (int i = 0; i < m_dataList.Count; i++)
			{
				ObjCreateData objCreateData = m_dataList[i];
				if (objCreateData.m_obj == null && !objCreateData.m_create)
				{
					m_dataList[i].m_create = true;
					objCreateData.m_obj = CreateObject(base.gameObject, objCreateData.m_src, objCreateData.m_pos, objCreateData.m_rot);
					if (objCreateData.m_obj != null)
					{
						OnCreateSetup();
					}
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
		UpdateLocal();
	}

	public void Setup()
	{
		if (m_dataList == null)
		{
			m_dataList = new List<ObjCreateData>();
		}
		m_playerInformation = ObjUtil.GetPlayerInformation();
		m_levelInformation = ObjUtil.GetLevelInformation();
	}

	public void AddObject(GameObject srcObject, Vector3 pos, Quaternion rot)
	{
		if (m_dataList == null)
		{
			return;
		}
		if (srcObject != null)
		{
			SpawnableObject component = srcObject.GetComponent<SpawnableObject>();
			if (component != null && component.IsValid())
			{
				m_dataList.Add(new ObjCreateData(srcObject, pos, rot));
			}
		}
		if (m_dataList.Count > 0)
		{
			if (m_dataList.Count >= CREATE_COUNT)
			{
				m_createCountMax = m_dataList.Count / CREATE_COUNT + 1;
			}
			else
			{
				m_createCountMax = 1;
			}
		}
	}

	private GameObject CreateObject(GameObject parent, GameObject srcObject, Vector3 pos, Quaternion rot)
	{
		GameObject gameObject = null;
		SpawnableObject spawnableObject = null;
		GameObject gameObject2 = ObjUtil.GetChangeObject(ResourceManager.Instance, m_playerInformation, m_levelInformation, srcObject.name);
		if (gameObject2 != null)
		{
			spawnableObject = GetReviveSpawnableObject(gameObject2);
		}
		else
		{
			gameObject2 = ObjUtil.GetCrystalChangeObject(ResourceManager.Instance, srcObject);
			spawnableObject = ((!(gameObject2 != null)) ? GetReviveSpawnableObject(srcObject) : GetReviveSpawnableObject(gameObject2));
		}
		if (spawnableObject != null)
		{
			SetRevivalSpawnableObject(spawnableObject, pos, rot);
			gameObject = spawnableObject.gameObject;
		}
		else
		{
			gameObject = ((!(gameObject2 != null)) ? (Object.Instantiate(srcObject, pos, rot) as GameObject) : (Object.Instantiate(gameObject2, pos, rot) as GameObject));
			spawnableObject = gameObject.GetComponent<SpawnableObject>();
			if (spawnableObject != null)
			{
				spawnableObject.AttachModelObject();
			}
		}
		if ((bool)gameObject && (bool)parent)
		{
			gameObject.SetActive(true);
			gameObject.transform.parent = parent.transform;
		}
		return gameObject;
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
		ObjectSpawnManager manager = GetManager();
		if (manager != null && component.IsStockObject())
		{
			return manager.GetSpawnableObject(component.GetStockObjectType());
		}
		return null;
	}

	private void SetRevivalSpawnableObject(SpawnableObject spawnableObject, Vector3 pos, Quaternion rot)
	{
		if (spawnableObject != null)
		{
			spawnableObject.Sleep = false;
			spawnableObject.gameObject.transform.position = pos;
			spawnableObject.gameObject.transform.rotation = rot;
			spawnableObject.OnRevival();
		}
	}
}
