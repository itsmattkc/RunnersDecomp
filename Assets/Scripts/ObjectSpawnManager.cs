using Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Scripts/Runners/Game/Level")]
public class ObjectSpawnManager : MonoBehaviour
{
	public class StockData
	{
		public string m_name;

		public int m_stockCount;

		public bool m_bossStage;

		public StockData(string name, int stockCount, bool bossStage)
		{
			m_name = name;
			m_stockCount = stockCount;
			m_bossStage = bossStage;
		}
	}

	private class CheckPointInfo
	{
		public int m_activateID;

		public bool m_onReplace;
	}

	public class DepotObjs
	{
		private GameObject m_depot;

		private List<SpawnableObject> m_objList = new List<SpawnableObject>();

		public DepotObjs(GameObject parentObj, StockObjectType type)
		{
			m_depot = new GameObject();
			m_depot.name = type.ToString();
			m_depot.transform.parent = parentObj.transform;
		}

		public void Add(SpawnableObject obj)
		{
			obj.Share = true;
			m_objList.Add(obj);
		}

		public SpawnableObject Get()
		{
			foreach (SpawnableObject obj in m_objList)
			{
				if (obj.Sleep)
				{
					obj.Sleep = false;
					return obj;
				}
			}
			return null;
		}

		public void Sleep(SpawnableObject obj)
		{
			obj.Sleep = true;
			if (obj.gameObject != null && m_depot != null)
			{
				obj.gameObject.transform.parent = m_depot.transform;
				if (obj.gameObject.activeSelf)
				{
					obj.gameObject.SetActive(false);
				}
			}
		}
	}

	private const float MaxOfRange = 200f;

	private readonly StockData[] STOCK_DATA_TABLE = new StockData[12]
	{
		new StockData("ObjRing", 50, true),
		new StockData("ObjSuperRing", 20, true),
		new StockData("ObjCrystal_A", 40, false),
		new StockData("ObjCrystal_B", 40, false),
		new StockData("ObjCrystal_C", 40, false),
		new StockData("ObjCrystal10_A", 40, false),
		new StockData("ObjCrystal10_B", 40, false),
		new StockData("ObjCrystal10_C", 40, false),
		new StockData("ObjEventCrystal", 50, false),
		new StockData("ObjEventCrystal10", 40, false),
		new StockData("ObjAirTrap", 40, false),
		new StockData("ObjTrap", 10, false)
	};

	private PlayerInformation m_playerInformation;

	private LevelInformation m_levelInformation;

	private StageSpawnableParameterContainer m_stageSetData;

	private List<SpawnableInfo> m_spawnableInfoList;

	private List<string> m_onlyOneObjectName;

	private ResourceManager m_resourceManager;

	private List<CheckPointInfo> m_checkPointInfos = new List<CheckPointInfo>();

	private GameObject m_stock;

	private Dictionary<StockObjectType, DepotObjs> m_dicSpawnableObjs = new Dictionary<StockObjectType, DepotObjs>();

	private bool m_setDataLoaded;

	private void Start()
	{
	}

	private void Update()
	{
		if (m_playerInformation != null)
		{
			Vector3 position = m_playerInformation.Position;
			float x = position.x;
			Vector3 position2 = base.transform.position;
			if (x > position2.x)
			{
				base.transform.position = m_playerInformation.Position;
			}
		}
		Vector3 position3 = base.transform.position;
		float x2 = position3.x;
		CheckRangeIn(x2);
		CheckRangeOut(x2);
	}

	private void OnDestroy()
	{
		if (m_spawnableInfoList == null)
		{
			return;
		}
		foreach (SpawnableInfo spawnableInfo in m_spawnableInfoList)
		{
			DestroyObject(spawnableInfo);
		}
		m_spawnableInfoList.Clear();
	}

	public void Setup(bool bossStage)
	{
		m_resourceManager = ResourceManager.Instance;
		m_playerInformation = ObjUtil.GetPlayerInformation();
		m_levelInformation = ObjUtil.GetLevelInformation();
		m_spawnableInfoList = new List<SpawnableInfo>();
		m_onlyOneObjectName = new List<string>();
		m_stageSetData = new StageSpawnableParameterContainer();
		StockStageObject(bossStage);
		StartCoroutine(LoadSetTableFirst());
	}

	private void StockStageObject(bool bossStage)
	{
		if (m_stock == null)
		{
			m_stock = new GameObject("StockGameObject");
			if ((bool)m_stock)
			{
				m_stock.transform.position = Vector3.zero;
				m_stock.transform.rotation = Quaternion.identity;
			}
		}
		bool flag = false;
		if (StageModeManager.Instance != null)
		{
			flag = StageModeManager.Instance.IsQuickMode();
		}
		if (m_dicSpawnableObjs.Count == 0)
		{
			for (int i = 0; i < 12; i++)
			{
				StockObjectType stockObjectType = (StockObjectType)i;
				m_dicSpawnableObjs.Add(stockObjectType, new DepotObjs(m_stock, stockObjectType));
			}
		}
		if (!(ResourceManager.Instance != null))
		{
			return;
		}
		for (int j = 0; j < 12; j++)
		{
			if ((bossStage && !STOCK_DATA_TABLE[j].m_bossStage) || ((j == 8 || j == 9) && (!flag || (EventManager.Instance != null && EventManager.Instance.Type != EventManager.EventType.QUICK))) || (j == 10 && flag))
			{
				continue;
			}
			GameObject spawnableGameObject = ResourceManager.Instance.GetSpawnableGameObject(STOCK_DATA_TABLE[j].m_name);
			int stockCount = STOCK_DATA_TABLE[j].m_stockCount;
			for (int k = 0; k < stockCount; k++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(spawnableGameObject, Vector3.zero, Quaternion.identity) as GameObject;
				if (gameObject != null)
				{
					gameObject.name = spawnableGameObject.name;
					SpawnableObject component = gameObject.GetComponent<SpawnableObject>();
					if (component != null)
					{
						AddSpawnableObject(component);
					}
				}
			}
		}
	}

	public IEnumerator LoadSetTableFirst()
	{
		base.enabled = false;
		m_setDataLoaded = false;
		yield return StartCoroutine(LoadSetTable(0));
		yield return StartCoroutine(LoadSetTable(91));
		m_setDataLoaded = true;
		base.enabled = true;
		GC.Collect();
	}

	public IEnumerator LoadSetTable(int firstBlock, int numBlock)
	{
		m_setDataLoaded = false;
		for (int i = firstBlock; i < firstBlock + numBlock; i += 5)
		{
			yield return StartCoroutine(LoadSetTable(i));
		}
		m_setDataLoaded = true;
		base.enabled = true;
	}

	public IEnumerator LoadSetTable(int[] blockTable, int readFileCount)
	{
		m_setDataLoaded = false;
		int count = Mathf.Min(blockTable.Length, readFileCount);
		for (int index = 0; index < count; index++)
		{
			yield return StartCoroutine(LoadSetTable(blockTable[index]));
		}
		m_setDataLoaded = true;
		base.enabled = true;
	}

	public bool IsDataLoaded()
	{
		return m_setDataLoaded;
	}

	public void RegisterOnlyOneObject(SpawnableInfo info)
	{
		if (info != null)
		{
			m_onlyOneObjectName.Add(info.m_parameters.ObjectName);
		}
	}

	private IEnumerator LoadSetTable(int blockIndex)
	{
		int arrayIndex2 = 0;
		if (blockIndex == 0)
		{
			arrayIndex2 = 0;
		}
		else
		{
			arrayIndex2 = (blockIndex - 1) / 5 + 1;
			blockIndex = (arrayIndex2 - 1) * 5 + 1;
		}
		if (m_stageSetData.GetBlockData(blockIndex, 0) != null || m_stageSetData.GetBlockData(blockIndex, 1) != null)
		{
			yield break;
		}
		GameObject assetObject = m_resourceManager.GetGameObject(ResourceCategory.TERRAIN_MODEL, TerrainXmlData.DataAssetName);
		if (!assetObject)
		{
			yield break;
		}
		TerrainXmlData terrainData = assetObject.GetComponent<TerrainXmlData>();
		if (!terrainData)
		{
			yield break;
		}
		TextAsset[] asset = terrainData.SetData;
		if (asset != null)
		{
			TextAsset xmlText = asset[arrayIndex2];
			if (xmlText != null)
			{
				GameObject parserObj = new GameObject();
				SpawnableParser parser = parserObj.AddComponent<SpawnableParser>();
				yield return StartCoroutine(parser.CreateSetData(m_resourceManager, xmlText, m_stageSetData));
				UnityEngine.Object.Destroy(parserObj);
			}
		}
	}

	private void LoadSetTable()
	{
		GameObject gameObject = m_resourceManager.GetGameObject(ResourceCategory.TERRAIN_MODEL, TerrainXmlData.DataAssetName);
		if (!gameObject)
		{
			return;
		}
		TerrainXmlData component = gameObject.GetComponent<TerrainXmlData>();
		if ((bool)component)
		{
			TextAsset[] setData = component.SetData;
			if (setData != null)
			{
				GameObject gameObject2 = new GameObject();
				SpawnableParser spawnableParser = gameObject2.AddComponent<SpawnableParser>();
				spawnableParser.CreateSetData(m_resourceManager, setData[0], m_stageSetData);
				UnityEngine.Object.Destroy(gameObject2);
			}
		}
	}

	public void OnActivateBlock(MsgActivateBlock msg)
	{
		if (msg != null)
		{
			ActivateBlock(msg.m_blockNo, msg.m_layerNo, msg.m_activateID, msg.m_originPosition, msg.m_originRotation);
			if (msg.m_checkPoint != 0)
			{
				CheckPointInfo checkPointInfo = new CheckPointInfo();
				checkPointInfo.m_activateID = msg.m_activateID;
				checkPointInfo.m_onReplace = (msg.m_checkPoint == MsgActivateBlock.CheckPoint.First);
				m_checkPointInfos.Add(checkPointInfo);
			}
			if (msg.m_replaceStage)
			{
				CheckRangeIn(msg.m_originPosition.x);
			}
		}
	}

	private void OnDeactivateBlock(MsgDeactivateBlock msg)
	{
		if (msg != null)
		{
			DeactivateBlock(msg.m_activateID, true);
		}
	}

	private void OnDeactivateAllBlock(MsgDeactivateAllBlock msg)
	{
		foreach (SpawnableInfo spawnableInfo in m_spawnableInfoList)
		{
			DestroyObject(spawnableInfo);
		}
		m_spawnableInfoList.Clear();
		m_onlyOneObjectName.Clear();
	}

	private void OnChangeCurerntBlock(MsgChangeCurrentBlock msg)
	{
	}

	private bool CheckAndActivePointMarker(SpawnableObject createdObject, CheckPointInfo info)
	{
		if ((bool)createdObject && createdObject.name.Contains("ObjPointMarker"))
		{
			MsgActivePointMarker msgActivePointMarker = new MsgActivePointMarker((!info.m_onReplace) ? PointMarkerType.OrderBlock : PointMarkerType.BossEnd);
			createdObject.SendMessage("OnActivePointMarker", msgActivePointMarker, SendMessageOptions.DontRequireReceiver);
			if (msgActivePointMarker.m_activated)
			{
				m_checkPointInfos.Remove(info);
			}
			return true;
		}
		return false;
	}

	private void ActivateBlock(int block, int layer, int activateID, Vector3 originPoint, Quaternion originRotation)
	{
		Vector3 position = base.transform.position;
		Quaternion rotation = base.transform.rotation;
		base.transform.position = originPoint;
		base.transform.rotation = originRotation;
		BlockSpawnableParameterContainer blockData = m_stageSetData.GetBlockData(block, layer);
		if (blockData != null)
		{
			foreach (SpawnableParameter parameter in blockData.GetParameters())
			{
				SpawnableInfo spawnableInfo = new SpawnableInfo();
				spawnableInfo.m_block = block;
				spawnableInfo.m_blockActivateID = activateID;
				spawnableInfo.m_parameters = parameter;
				spawnableInfo.m_position = base.transform.TransformPoint(parameter.Position);
				spawnableInfo.m_rotation = rotation * parameter.Rotation;
				spawnableInfo.m_manager = this;
				m_spawnableInfoList.Add(spawnableInfo);
			}
		}
		base.transform.position = position;
		base.transform.rotation = rotation;
		GC.Collect();
	}

	private void DeactivateBlock(int blockActivateID, bool ignoreNotRangeOut)
	{
		for (int num = m_spawnableInfoList.Count - 1; num >= 0; num--)
		{
			SpawnableInfo spawnableInfo = m_spawnableInfoList[num];
			if (spawnableInfo.m_blockActivateID == blockActivateID && (bool)spawnableInfo.m_object)
			{
				if (spawnableInfo.NotRangeOut && ignoreNotRangeOut)
				{
					continue;
				}
				DestroyObject(spawnableInfo);
				m_spawnableInfoList.Remove(m_spawnableInfoList[num]);
			}
			foreach (CheckPointInfo checkPointInfo in m_checkPointInfos)
			{
				if (checkPointInfo.m_activateID == blockActivateID)
				{
					m_checkPointInfos.Remove(checkPointInfo);
					break;
				}
			}
		}
	}

	public SpawnableObject GetSpawnableObject(StockObjectType type)
	{
		if (m_dicSpawnableObjs.ContainsKey(type))
		{
			return m_dicSpawnableObjs[type].Get();
		}
		return null;
	}

	private void AddSpawnableObject(SpawnableObject spawnableObject)
	{
		if (spawnableObject != null && spawnableObject.IsStockObject())
		{
			spawnableObject.AttachModelObject();
			StockObjectType stockObjectType = spawnableObject.GetStockObjectType();
			if (m_dicSpawnableObjs.ContainsKey(stockObjectType))
			{
				m_dicSpawnableObjs[stockObjectType].Add(spawnableObject);
				m_dicSpawnableObjs[stockObjectType].Sleep(spawnableObject);
			}
		}
	}

	public void SleepSpawnableObject(SpawnableObject spawnableObject)
	{
		if (spawnableObject != null && spawnableObject.Share)
		{
			StockObjectType stockObjectType = spawnableObject.GetStockObjectType();
			if (m_dicSpawnableObjs.ContainsKey(stockObjectType))
			{
				m_dicSpawnableObjs[stockObjectType].Sleep(spawnableObject);
			}
		}
	}

	private bool CreateObject(SpawnableInfo info)
	{
		if (m_onlyOneObjectName.IndexOf(info.m_parameters.ObjectName) != -1)
		{
			return false;
		}
		GameObject gameObject = ObjUtil.GetChangeObject(m_resourceManager, m_playerInformation, m_levelInformation, info.m_parameters.ObjectName);
		if (gameObject == null)
		{
			gameObject = m_resourceManager.GetSpawnableGameObject(info.m_parameters.ObjectName);
		}
		if (gameObject != null)
		{
			SpawnableObject component = gameObject.GetComponent<SpawnableObject>();
			if (component != null && !component.IsValid())
			{
				return false;
			}
		}
		SpawnableObject reviveSpawnableObject = GetReviveSpawnableObject(gameObject);
		if (reviveSpawnableObject != null)
		{
			reviveSpawnableObject.Sleep = false;
			reviveSpawnableObject.gameObject.transform.position = info.m_position;
			reviveSpawnableObject.gameObject.transform.rotation = info.m_rotation;
			reviveSpawnableObject.gameObject.SetActive(true);
			reviveSpawnableObject.gameObject.transform.parent = m_stock.transform;
			info.SpawnedObject(reviveSpawnableObject);
			reviveSpawnableObject.AttachSpawnableInfo(info);
			SpawnableBehavior component2 = reviveSpawnableObject.gameObject.GetComponent<SpawnableBehavior>();
			if ((bool)component2)
			{
				component2.SetParameters(info.m_parameters);
			}
			reviveSpawnableObject.OnRevival();
			return true;
		}
		if (gameObject != null)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, info.m_position, info.m_rotation) as GameObject;
			if ((bool)gameObject2)
			{
				gameObject2.gameObject.SetActive(true);
				reviveSpawnableObject = gameObject2.GetComponent<SpawnableObject>();
				if (reviveSpawnableObject != null)
				{
					info.SpawnedObject(reviveSpawnableObject);
					reviveSpawnableObject.AttachSpawnableInfo(info);
				}
				SpawnableBehavior component3 = gameObject2.GetComponent<SpawnableBehavior>();
				if ((bool)component3)
				{
					component3.SetParameters(info.m_parameters);
				}
				if (reviveSpawnableObject != null)
				{
					reviveSpawnableObject.AttachModelObject();
					reviveSpawnableObject.OnCreate();
				}
				CheckPointInfo retInfo = null;
				if (IsCreatePointmarkerBlock(info.m_blockActivateID, ref retInfo) && retInfo != null)
				{
					CheckAndActivePointMarker(reviveSpawnableObject, retInfo);
				}
				return true;
			}
		}
		return false;
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
		if (component.IsStockObject())
		{
			return GetSpawnableObject(component.GetStockObjectType());
		}
		return null;
	}

	private void DestroyObject(SpawnableInfo info)
	{
		if (info.Spawned)
		{
			if (info.m_object.Share)
			{
				info.m_object.SetSleep();
			}
			else
			{
				UnityEngine.Object.Destroy(info.m_object.gameObject);
			}
			info.DestroyedObject();
		}
	}

	public void DetachObject(SpawnableInfo info)
	{
		info.RequestDestroy = true;
		info.DestroyedObject();
		m_spawnableInfoList.Remove(info);
	}

	public void DetachInfoList(SpawnableInfo info)
	{
		m_spawnableInfoList.Remove(info);
	}

	private void CheckRangeIn(float basePosition)
	{
		List<SpawnableInfo> list = null;
		foreach (SpawnableInfo spawnableInfo in m_spawnableInfoList)
		{
			if (spawnableInfo.Spawned || spawnableInfo.Destroyed)
			{
				continue;
			}
			float x = spawnableInfo.m_position.x;
			float num = x - basePosition;
			if (x - basePosition < spawnableInfo.m_parameters.RangeIn && !CreateObject(spawnableInfo))
			{
				if (list == null)
				{
					new List<SpawnableInfo>();
				}
				if (list != null)
				{
					list.Add(spawnableInfo);
				}
			}
			if (!(num > 200f))
			{
				continue;
			}
			break;
		}
		if (list == null)
		{
			return;
		}
		foreach (SpawnableInfo item in list)
		{
			m_spawnableInfoList.Remove(item);
		}
	}

	private void CheckRangeOut(float basePosition)
	{
		List<SpawnableInfo> list = null;
		foreach (SpawnableInfo spawnableInfo in m_spawnableInfoList)
		{
			if (!spawnableInfo.Spawned || spawnableInfo.NotRangeOut)
			{
				continue;
			}
			float num = basePosition - spawnableInfo.m_position.x;
			if (num < 0f)
			{
				break;
			}
			if (num > spawnableInfo.m_parameters.RangeOut)
			{
				if (list == null)
				{
					new List<SpawnableInfo>();
				}
				if (list != null)
				{
					list.Add(spawnableInfo);
				}
				DestroyObject(spawnableInfo);
			}
		}
		if (list == null)
		{
			return;
		}
		foreach (SpawnableInfo item in list)
		{
			m_spawnableInfoList.Remove(item);
		}
	}

	private void DrawObjectInfo(string infoname)
	{
		Debug.Log(infoname + "Start");
		foreach (SpawnableInfo spawnableInfo in m_spawnableInfoList)
		{
			string str = spawnableInfo.m_parameters.ObjectName + ":";
			str += ((!spawnableInfo.Spawned) ? "NotSpawned" : "Spawned ");
			string text = str;
			str = text + "pos " + spawnableInfo.m_position.x.ToString("F2") + " " + spawnableInfo.m_position.y.ToString("F2") + " " + spawnableInfo.m_position.z.ToString("F2");
			Debug.Log(str + "\n");
		}
		Debug.Log(infoname + "End");
	}

	private bool IsCreatePointmarkerBlock(int activateID, ref CheckPointInfo retInfo)
	{
		foreach (CheckPointInfo checkPointInfo in m_checkPointInfos)
		{
			if (checkPointInfo.m_activateID == activateID)
			{
				retInfo = checkPointInfo;
				return true;
			}
		}
		return false;
	}
}
