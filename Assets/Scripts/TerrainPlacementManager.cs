using Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPlacementManager : MonoBehaviour
{
	public class TerrainReserveInfo
	{
		public string m_baseName;

		public int m_count;

		public TerrainReserveInfo(string name, int count)
		{
			m_baseName = name;
			m_count = count;
		}

		public string GetBlockName(string stageName)
		{
			return stageName + m_baseName;
		}
	}

	private const float RangeInDistance = 40f;

	private const float RangeOutDistance = 30f;

	private const float MaxOfRange = 200f;

	private const int SystemBlockIndexStart = 91;

	private const int SystemBlockIndexEnd = 99;

	private const float ZOffset = 1.5f;

	public TextAsset m_setData;

	private TerrainList m_terrainList;

	private List<TerrainPlacementInfo> m_placementList;

	private List<TerrainReserveObject> m_reserveObjectList = new List<TerrainReserveObject>();

	private int m_defaultLayer;

	private bool m_isBossStage;

	private bool m_isCreateTerrain;

	private TerrainReserveInfo[] TERRAIN_MODEL_TBL = new TerrainReserveInfo[17]
	{
		new TerrainReserveInfo("_pf_002m", 8),
		new TerrainReserveInfo("_pf_006m", 4),
		new TerrainReserveInfo("_pf_012m", 3),
		new TerrainReserveInfo("_pf_024m", 3),
		new TerrainReserveInfo("_pfloop_014m", 3),
		new TerrainReserveInfo("_pfslopedown_012m", 3),
		new TerrainReserveInfo("_pfslopedown_014m", 3),
		new TerrainReserveInfo("_pfslopedown_018m", 2),
		new TerrainReserveInfo("_pfslopeup_012m", 3),
		new TerrainReserveInfo("_pfslopeup_014m", 3),
		new TerrainReserveInfo("_pfslopeup_018m", 2),
		new TerrainReserveInfo("_pfslope_down_012m", 3),
		new TerrainReserveInfo("_pfslope_down_014m", 3),
		new TerrainReserveInfo("_pfslope_down_018m", 2),
		new TerrainReserveInfo("_pfslope_up_012m", 3),
		new TerrainReserveInfo("_pfslope_up_014m", 3),
		new TerrainReserveInfo("_pfslope_up_018m", 2)
	};

	private TerrainReserveInfo[] BOSS_STAGE_TERRAIN_MODEL_TBL = new TerrainReserveInfo[4]
	{
		new TerrainReserveInfo("_pf_002m", 4),
		new TerrainReserveInfo("_pf_006m", 4),
		new TerrainReserveInfo("_pf_012m", 3),
		new TerrainReserveInfo("_pf_024m", 3)
	};

	private void Start()
	{
		base.tag = "StageManager";
		m_defaultLayer = LayerMask.NameToLayer("Terrain");
	}

	private void Update()
	{
		Vector3 position = base.transform.position;
		float x = position.x;
		CheckRangeIn(x);
		CheckRangeOut(x);
	}

	public void Setup(bool isBossStage)
	{
		m_isBossStage = isBossStage;
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.TERRAIN_MODEL, TerrainXmlData.DataAssetName);
		if (gameObject == null)
		{
			return;
		}
		TerrainXmlData component = gameObject.GetComponent<TerrainXmlData>();
		if (component == null)
		{
			return;
		}
		m_terrainList = ImportTerrains.Import(component.TerrainBlock);
		m_placementList = new List<TerrainPlacementInfo>();
		StageBlockManager component2 = base.gameObject.GetComponent<StageBlockManager>();
		if (!(component2 == null) && m_terrainList != null)
		{
			int terrainCount = m_terrainList.GetTerrainCount();
			for (int i = 0; i < terrainCount; i++)
			{
				AddTerrainInfo(component2, i);
			}
			for (int j = 91; j <= 99; j++)
			{
				AddTerrainInfo(component2, j);
			}
			StartCoroutine(CreateTerrain());
		}
	}

	public void ReCreateTerrain()
	{
		DeleteTerrain();
		StartCoroutine(CreateTerrain());
	}

	private void AddTerrainInfo(StageBlockManager blockManager, int index)
	{
		Terrain terrain = m_terrainList.GetTerrain(index);
		if (terrain != null)
		{
			int terrainIndex = int.Parse(terrain.m_name);
			float meter = terrain.m_meter;
			blockManager.AddTerrainInfo(terrainIndex, meter);
		}
	}

	public void ActivateTerrain(int terrainIndex, Vector3 originPosition)
	{
		if (m_terrainList == null)
		{
			return;
		}
		Terrain terrain = m_terrainList.GetTerrain(terrainIndex);
		if (terrain == null)
		{
			return;
		}
		int blockCount = terrain.GetBlockCount();
		for (int i = 0; i < blockCount; i++)
		{
			TerrainBlock block = terrain.GetBlock(i);
			if (block != null)
			{
				string name = block.m_name;
				GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.TERRAIN_MODEL, name);
				if (!(gameObject == null))
				{
					TerrainPlacementInfo terrainPlacementInfo = new TerrainPlacementInfo();
					terrainPlacementInfo.m_terrainIndex = terrainIndex;
					Vector3 pos = originPosition + block.m_transform.m_pos;
					Vector3 rot = block.m_transform.m_rot;
					TransformParam transform = new TransformParam(pos, rot);
					terrainPlacementInfo.m_block = new TerrainBlock(block.m_name, transform);
					gameObject.layer = m_defaultLayer;
					m_placementList.Add(terrainPlacementInfo);
				}
			}
		}
	}

	public void DeactivateTerrain(int terrainIndex, float basePosition)
	{
		for (int num = m_placementList.Count - 1; num >= 0; num--)
		{
			TerrainPlacementInfo terrainPlacementInfo = m_placementList[num];
			if (terrainPlacementInfo != null && terrainPlacementInfo.m_terrainIndex == terrainIndex)
			{
				Vector3 pos = terrainPlacementInfo.m_block.m_transform.m_pos;
				if (StageBlockUtil.IsPastPosition(pos.x, basePosition, 0f))
				{
					if (terrainPlacementInfo.IsReserveTerrain())
					{
						ReturnTerrainReserveObject(terrainPlacementInfo);
					}
					else
					{
						DestroyTerrain(terrainPlacementInfo);
					}
					m_placementList.Remove(terrainPlacementInfo);
				}
			}
		}
	}

	private void DeleteTerrain()
	{
		foreach (TerrainReserveObject reserveObject in m_reserveObjectList)
		{
			GameObject gameObject = reserveObject.GetGameObject();
			if (gameObject != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
		m_reserveObjectList.Clear();
		m_isCreateTerrain = false;
	}

	private IEnumerator CreateTerrain()
	{
		if (m_isCreateTerrain)
		{
			yield break;
		}
		string stageName = "w01";
		GameModeStage gameModeStage = GameObjectUtil.FindGameObjectComponent<GameModeStage>("GameModeStage");
		if (gameModeStage != null)
		{
			stageName = gameModeStage.GetStageName();
		}
		TerrainReserveInfo[] tbl = (!m_isBossStage) ? TERRAIN_MODEL_TBL : BOSS_STAGE_TERRAIN_MODEL_TBL;
		TerrainReserveInfo[] array = tbl;
		foreach (TerrainReserveInfo dataInfo in array)
		{
			string blockName = dataInfo.GetBlockName(stageName);
			GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.TERRAIN_MODEL, blockName);
			if (!(gameObject != null))
			{
				continue;
			}
			for (int index = 0; index < dataInfo.m_count; index++)
			{
				GameObject gameObjectCopy = UnityEngine.Object.Instantiate(gameObject, Vector3.zero, Quaternion.identity) as GameObject;
				if (gameObjectCopy != null)
				{
					gameObjectCopy.isStatic = true;
					gameObjectCopy.SetActive(true);
					TerrainReserveObject data = new TerrainReserveObject(gameObjectCopy, blockName, m_reserveObjectList.Count);
					if (data != null)
					{
						m_reserveObjectList.Add(data);
					}
				}
			}
		}
		if (m_reserveObjectList.Count <= 0)
		{
			yield break;
		}
		int waiteframe = 1;
		while (waiteframe > 0)
		{
			waiteframe--;
			yield return null;
		}
		foreach (TerrainReserveObject obj in m_reserveObjectList)
		{
			if (obj != null)
			{
				obj.ReturnObject();
			}
		}
		m_isCreateTerrain = true;
		GC.Collect();
	}

	private TerrainReserveObject GetTerrainReserveObject(string blockName)
	{
		foreach (TerrainReserveObject reserveObject in m_reserveObjectList)
		{
			if (reserveObject != null && reserveObject.EableReservation && reserveObject.blockName == blockName)
			{
				return reserveObject;
			}
		}
		return null;
	}

	private void ReturnTerrainReserveObject(TerrainPlacementInfo info)
	{
		if (info == null)
		{
			return;
		}
		foreach (TerrainReserveObject reserveObject in m_reserveObjectList)
		{
			if (reserveObject != null && reserveObject.ReserveIndex == info.ReserveIndex)
			{
				reserveObject.ReturnObject();
				info.DestroyObject();
				break;
			}
		}
	}

	private void CheckRangeIn(float basePosition)
	{
		if (m_placementList == null)
		{
			return;
		}
		List<TerrainPlacementInfo> list = null;
		foreach (TerrainPlacementInfo placement in m_placementList)
		{
			if (placement == null || placement.Created)
			{
				continue;
			}
			float num = 0f;
			Vector3 pos = placement.m_block.m_transform.m_pos;
			float x = pos.x;
			num = x - basePosition;
			if (num > 200f)
			{
				break;
			}
			if (!(num < 40f))
			{
				continue;
			}
			TerrainReserveObject terrainReserveObject = GetTerrainReserveObject(placement.m_block.m_name);
			if (terrainReserveObject != null)
			{
				placement.m_gameObject = terrainReserveObject.ReserveObject();
				placement.ReserveIndex = terrainReserveObject.ReserveIndex;
				Vector3 pos2 = placement.m_block.m_transform.m_pos;
				pos2.z += 1.5f;
				placement.m_gameObject.transform.position = pos2;
				placement.m_gameObject.transform.rotation = Quaternion.Euler(placement.m_block.m_transform.m_rot);
				placement.m_gameObject.SetActive(true);
				continue;
			}
			Debug.Log("Terrain Instantiate!!  m_block = " + placement.m_block.m_name);
			if (!CreateTerrainObject(placement))
			{
				if (list == null)
				{
					list = new List<TerrainPlacementInfo>();
				}
				list.Add(placement);
			}
		}
		if (list == null)
		{
			return;
		}
		foreach (TerrainPlacementInfo item in list)
		{
			if (item != null)
			{
				m_placementList.Remove(item);
			}
		}
	}

	private void CheckRangeOut(float basePosition)
	{
		if (m_placementList == null)
		{
			return;
		}
		List<TerrainPlacementInfo> list = null;
		foreach (TerrainPlacementInfo placement in m_placementList)
		{
			if (placement == null || !placement.Created || placement.Destroyed)
			{
				continue;
			}
			Vector3 pos = placement.m_block.m_transform.m_pos;
			float num = basePosition - pos.x;
			if (num < 0f)
			{
				break;
			}
			if (num > 30f)
			{
				if (placement.IsReserveTerrain())
				{
					ReturnTerrainReserveObject(placement);
				}
				else
				{
					DestroyTerrain(placement);
				}
				if (list == null)
				{
					list = new List<TerrainPlacementInfo>();
				}
				list.Add(placement);
			}
		}
		if (list == null)
		{
			return;
		}
		foreach (TerrainPlacementInfo item in list)
		{
			if (item != null)
			{
				m_placementList.Remove(item);
			}
		}
	}

	private bool CreateTerrainObject(TerrainPlacementInfo info)
	{
		if (info == null)
		{
			return false;
		}
		string name = info.m_block.m_name;
		GameObject gameObject = ResourceManager.Instance.GetGameObject(ResourceCategory.TERRAIN_MODEL, name);
		if (gameObject == null)
		{
			return false;
		}
		Vector3 pos = info.m_block.m_transform.m_pos;
		pos.z += 1.5f;
		Vector3 rot = info.m_block.m_transform.m_rot;
		Quaternion rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, pos, rotation) as GameObject;
		if (gameObject2 == null)
		{
			return false;
		}
		gameObject2.isStatic = true;
		gameObject2.SetActive(true);
		info.m_gameObject = gameObject2;
		return true;
	}

	private void DestroyTerrain(TerrainPlacementInfo info)
	{
		UnityEngine.Object.Destroy(info.m_gameObject);
		info.DestroyObject();
	}

	public void OnActivateBlock(MsgActivateBlock msg)
	{
		if (msg != null)
		{
			ActivateTerrain(msg.m_blockNo, msg.m_originPosition);
		}
	}

	private void OnDeactivateBlock(MsgDeactivateBlock msg)
	{
		if (msg != null)
		{
			DeactivateTerrain(msg.m_blockNo, msg.m_distance);
		}
	}

	private void OnDeactivateAllBlock(MsgDeactivateAllBlock msg)
	{
		foreach (TerrainPlacementInfo placement in m_placementList)
		{
			if (placement.IsReserveTerrain())
			{
				ReturnTerrainReserveObject(placement);
			}
			else
			{
				DestroyTerrain(placement);
			}
		}
		m_placementList.Clear();
	}

	private void OnMsgExitStage(MsgExitStage msg)
	{
		base.enabled = false;
	}
}
