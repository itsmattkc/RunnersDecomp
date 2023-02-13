using App;
using Message;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[AddComponentMenu("Scripts/Runners/Game/Level/StageBlockManager")]
public class StageBlockManager : MonoBehaviour
{
	public class StageBlockInfo
	{
		public int m_activateNo
		{
			get;
			set;
		}

		public int m_blockNo
		{
			get;
			set;
		}

		public int m_layerNo
		{
			get;
			set;
		}

		public float m_totalLength
		{
			get;
			set;
		}

		public Vector3 m_startPoint
		{
			get;
			set;
		}

		public Vector3 m_endPoint
		{
			get;
			set;
		}

		public OrderType m_orderType
		{
			get;
			set;
		}

		public int m_orderNum
		{
			get;
			set;
		}

		public Callback startCurrentCallback
		{
			get;
			set;
		}

		public Callback endCurrentCallback
		{
			get;
			set;
		}

		public float GetBlockRunningLength(float totalLength)
		{
			Vector3 startPoint = m_startPoint;
			return totalLength - startPoint.x;
		}

		public float GetPastDistanceFromEndPoint(float pos)
		{
			Vector3 endPoint = m_endPoint;
			return pos - endPoint.x;
		}

		public bool IsNearToGoal(float nowLength, float remainLength)
		{
			Vector3 endPoint = m_endPoint;
			float num = endPoint.x - nowLength;
			return remainLength > num;
		}
	}

	private class BlockOrderInfo
	{
		public int blockNo;

		public bool fixedLayerNo;

		public int layerNo;

		public int rndLayerNo;

		public bool isBoss;

		public Callback startCurrentCallback;

		public Callback endCurrentCallback;

		public void SetBlockAndFixedLayer(int block, int layer)
		{
			blockNo = block;
			fixedLayerNo = true;
			layerNo = layer;
		}

		public void SetBlockAndRandomLayer(int block, int rndNum)
		{
			blockNo = block;
			fixedLayerNo = false;
			rndLayerNo = rndNum;
		}

		public int GetNewLayerNo()
		{
			if (fixedLayerNo)
			{
				return layerNo;
			}
			return UnityEngine.Random.Range(0, rndLayerNo);
		}
	}

	[Serializable]
	public class BlockLevelData
	{
		public int numBlock = 5;

		public int numPlacement = 5;

		public int layerNum = 1;

		public int repeatNum = 1;

		public int fixedLayerNo = -1;

		public void CopyTo(BlockLevelData dst)
		{
			dst.numBlock = numBlock;
			dst.numPlacement = numPlacement;
			dst.layerNum = layerNum;
			dst.repeatNum = repeatNum;
			dst.fixedLayerNo = fixedLayerNo;
		}
	}

	[Serializable]
	public class StartingBlockInfo
	{
		public int fixedBlockNo = 2;

		public int startingLayerNum = 1;
	}

	[Serializable]
	public class DebugBlockInfo
	{
		public int block;

		public int layer;
	}

	public enum FixedLayerNumber
	{
		SYSTEM = 0,
		FEVER_BOSS_1 = 1,
		FEVER_BOSS_2 = 2,
		FEVER_BOSS_3 = 3,
		MAP_BOSS_1 = 11,
		MAP_BOSS_2 = 12,
		MAP_BOSS_3 = 13,
		EVENT_BOSS_1 = 21,
		EVENT_BOSS_2 = 22,
		EVENT_BOSS_3 = 23
	}

	public struct CreateInfo
	{
		public string stageName;

		public bool isSpawnableManager;

		public bool isTerrainManager;

		public bool isPathBlockManager;

		public PathManager pathManager;

		public bool showInfo;

		public bool randomBlock;

		public bool bossMode;

		public bool quickMode;
	}

	public enum OrderType
	{
		BOSS_SINGLE,
		ASCENDING,
		RANDOM,
		TUTORIAL,
		DEBUG
	}

	public delegate void Callback(StageBlockInfo blockInfo);

	private const int BLOCK_LEVEL_DATA_NUM = 5;

	private const int HIGH_SPEED_FILE_MAX_COUNT = 6;

	private const int HIGH_SPEED_FILE_ONCE_READ_LIMIT = 2;

	private const int DefaultLayerNo = 0;

	private const float DistanceOfActivateNext = 150f;

	private const float DistanceOfDeactivate = 150f;

	private const float DistanceOfChangeCurrent = 1f;

	public const int BossSingleBlockNo = 0;

	public const int TutorialBlockNo = 91;

	public const int StartActBlockNo = 92;

	[SerializeField]
	public DebugBlockInfo[] m_debugBlockInfo;

	private float m_totalDistance;

	private int m_numCreateBlock;

	private PlayerInformation m_playerInformation;

	private string m_stageName;

	private bool m_showStageInfo;

	private Rect m_window;

	private Dictionary<int, float> m_terrainPlacement;

	private Dictionary<int, StageBlockInfo> m_activeBlockInfo;

	private StageBlockInfo m_currentBlock;

	private OrderType m_orderType;

	private OrderType m_firstOrderType;

	private BlockOrderInfo[] m_blockOrder;

	private int m_currentOrderNum;

	private int m_nowBlockOrderNum;

	private int m_stageRepeatNum;

	private ObjectSpawnManager m_objectSpawnableManager;

	private TerrainPlacementManager m_terrainPlacementManager;

	private StageBlockPathManager m_stagePathManager;

	private LevelInformation m_levelInformation;

	private int[] m_highSpeedTable = new int[6]
	{
		21,
		26,
		31,
		36,
		41,
		46
	};

	private int m_highSpeedCount = 1;

	private bool m_highSpeedSetFlag;

	[SerializeField]
	private BlockLevelData[] m_blockLevelData = new BlockLevelData[5];

	[SerializeField]
	private BlockLevelData[] m_blockLevelDataForQuickMode = new BlockLevelData[5];

	private BlockLevelData[] m_currentBlockLevelData = new BlockLevelData[5];

	private int m_nowBlockLevelNum;

	private int m_nextBlockLevelNum;

	[SerializeField]
	private int m_apeearCheckPointNumber = 3;

	[SerializeField]
	private StartingBlockInfo m_startingBlockInfo = new StartingBlockInfo();

	[SerializeField]
	private bool m_useDebugOrder;

	private int[] m_firstBlockNoOnLevel;

	private bool m_nextBossBlock;

	private bool m_quickMode;

	private bool m_quickFirstReplaceEnd;

	private StageBlockInfo CurrentBlock
	{
		get
		{
			return m_currentBlock;
		}
		set
		{
			m_currentBlock = value;
		}
	}

	private StageBlockInfo NextBlock
	{
		get
		{
			StageBlockInfo currentBlock = CurrentBlock;
			if (currentBlock == null)
			{
				return null;
			}
			StageBlockInfo value = null;
			if (m_activeBlockInfo.TryGetValue(currentBlock.m_activateNo + 1, out value))
			{
				return value;
			}
			return null;
		}
	}

	public string StageName
	{
		get
		{
			return m_stageName;
		}
	}

	private void Start()
	{
		m_showStageInfo = false;
		App.Random.ShuffleInt(m_highSpeedTable);
	}

	private void Update()
	{
		if (m_playerInformation != null)
		{
			Vector3 position = m_playerInformation.Position;
			float x = position.x;
			Vector3 position2 = base.transform.position;
			if (!(x > position2.x))
			{
				return;
			}
			Vector3 position3 = base.transform.position;
			Vector3 position4 = m_playerInformation.Position;
			float x2 = position4.x;
			Vector3 position5 = base.transform.position;
			float y = position5.y;
			Vector3 position6 = base.transform.position;
			position3.Set(x2, y, position6.z);
			Vector3 position7 = base.transform.position;
			m_totalDistance = position7.x;
			if (m_levelInformation != null)
			{
				m_levelInformation.DistanceOnStage = m_totalDistance;
			}
		}
		CheckNextBlock();
	}

	private void CheckNextBlock()
	{
		if (CurrentBlock != null)
		{
			if (NextBlock == null)
			{
				if (CurrentBlock.IsNearToGoal(m_totalDistance, 150f))
				{
					BlockOrderInfo nextActivateBlockInfo = GetNextActivateBlockInfo();
					if (nextActivateBlockInfo != null)
					{
						Vector3 endPoint = CurrentBlock.m_endPoint;
						MsgActivateBlock.CheckPoint checkPoint = MsgActivateBlock.CheckPoint.None;
						if (m_orderType == OrderType.DEBUG || m_orderType == OrderType.RANDOM || m_orderType == OrderType.ASCENDING)
						{
							if (m_currentOrderNum == 1 && m_playerInformation != null && m_playerInformation.SpeedLevel > PlayerSpeed.LEVEL_1)
							{
								checkPoint = MsgActivateBlock.CheckPoint.First;
							}
							else if (m_currentOrderNum > 1 && m_currentOrderNum % m_apeearCheckPointNumber == 1)
							{
								checkPoint = MsgActivateBlock.CheckPoint.Internal;
							}
						}
						ActivateBlock(nextActivateBlockInfo, endPoint, false, m_orderType, m_currentOrderNum, checkPoint);
					}
				}
			}
			else
			{
				StageBlockInfo nextBlock = NextBlock;
				if (CurrentBlock.IsNearToGoal(m_totalDistance, 1f) && nextBlock != null)
				{
					ChangeCurrentBlock(nextBlock);
					if (m_nextBossBlock)
					{
						MsgBossStart value = new MsgBossStart();
						GameObjectUtil.SendMessageFindGameObject("GameModeStage", "OnSendToGameModeStage", value, SendMessageOptions.DontRequireReceiver);
						m_nextBossBlock = false;
					}
				}
			}
		}
		SearchAndDeleteRangeOutedBlock(m_totalDistance);
	}

	public void Initialize(CreateInfo cinfo)
	{
		m_quickMode = cinfo.quickMode;
		m_stageName = cinfo.stageName;
		m_totalDistance = 0f;
		GameObject gameObject = GameObject.Find("PlayerInformation");
		if (gameObject != null && m_playerInformation == null)
		{
			m_playerInformation = gameObject.GetComponent<PlayerInformation>();
		}
		if (m_levelInformation == null)
		{
			m_levelInformation = GameObjectUtil.FindGameObjectComponent<LevelInformation>("LevelInformation");
		}
		for (int i = 0; i < m_currentBlockLevelData.Length; i++)
		{
			m_currentBlockLevelData[i] = new BlockLevelData();
			if (m_quickMode)
			{
				if (i < m_blockLevelDataForQuickMode.Length)
				{
					m_blockLevelDataForQuickMode[i].CopyTo(m_currentBlockLevelData[i]);
				}
			}
			else if (i < m_blockLevelData.Length)
			{
				m_blockLevelData[i].CopyTo(m_currentBlockLevelData[i]);
			}
		}
		if (m_terrainPlacement == null)
		{
			m_terrainPlacement = new Dictionary<int, float>();
		}
		if (m_activeBlockInfo == null)
		{
			m_activeBlockInfo = new Dictionary<int, StageBlockInfo>();
		}
		if (cinfo.isSpawnableManager)
		{
			m_objectSpawnableManager = base.gameObject.AddComponent<ObjectSpawnManager>();
			m_objectSpawnableManager.enabled = false;
		}
		if (cinfo.isTerrainManager)
		{
			m_terrainPlacementManager = base.gameObject.AddComponent<TerrainPlacementManager>();
			m_terrainPlacementManager.enabled = false;
		}
		if (cinfo.isPathBlockManager && (bool)cinfo.pathManager)
		{
			m_stagePathManager = base.gameObject.AddComponent<StageBlockPathManager>();
			m_stagePathManager.SetPathManager(cinfo.pathManager);
			m_stagePathManager.enabled = false;
		}
		m_showStageInfo = cinfo.showInfo;
		if (cinfo.bossMode)
		{
			m_orderType = OrderType.BOSS_SINGLE;
		}
		else
		{
			m_orderType = ((!cinfo.randomBlock) ? OrderType.ASCENDING : OrderType.RANDOM);
		}
		m_firstOrderType = m_orderType;
		m_currentOrderNum = 0;
		m_stageRepeatNum = 0;
		if (m_levelInformation != null)
		{
			for (int j = 0; j < m_currentBlockLevelData.Length; j++)
			{
				if (m_currentBlockLevelData[j].repeatNum > 0)
				{
					m_nowBlockLevelNum = j;
					break;
				}
			}
			m_nextBlockLevelNum = m_nowBlockLevelNum;
			m_levelInformation.FeverBossCount = m_nowBlockLevelNum;
		}
		if (m_orderType == OrderType.DEBUG)
		{
			return;
		}
		m_firstBlockNoOnLevel = new int[m_currentBlockLevelData.Length];
		int num = 1;
		for (int k = 0; k < m_firstBlockNoOnLevel.Length; k++)
		{
			if (m_currentBlockLevelData[k] == null)
			{
				m_currentBlockLevelData[k] = new BlockLevelData();
			}
			m_firstBlockNoOnLevel[k] = num;
			if (k < 2)
			{
				num += m_currentBlockLevelData[k].numBlock;
			}
		}
	}

	public void Setup(bool bossStage)
	{
		if (m_objectSpawnableManager != null)
		{
			m_objectSpawnableManager.Setup(bossStage);
		}
		if (m_terrainPlacementManager != null)
		{
			m_terrainPlacementManager.Setup(bossStage);
			m_terrainPlacementManager.enabled = true;
		}
		if (m_stagePathManager != null)
		{
			m_stagePathManager.Setup();
			m_stagePathManager.enabled = true;
		}
	}

	public bool IsSetupEnded()
	{
		if ((bool)m_objectSpawnableManager)
		{
			return m_objectSpawnableManager.IsDataLoaded();
		}
		return true;
	}

	public void PauseTerrainPlacement(bool value)
	{
		if ((bool)m_terrainPlacementManager)
		{
			m_terrainPlacementManager.enabled = !value;
		}
	}

	public void AddTerrainInfo(int terrainIndex, float terrainLength)
	{
		if (!m_terrainPlacement.ContainsKey(terrainIndex))
		{
			m_terrainPlacement.Add(terrainIndex, terrainLength);
		}
	}

	public void DeactivateAll()
	{
		MsgDeactivateAllBlock value = new MsgDeactivateAllBlock();
		base.gameObject.SendMessage("OnDeactivateAllBlock", value, SendMessageOptions.DontRequireReceiver);
		m_activeBlockInfo.Clear();
		m_numCreateBlock = 0;
		m_currentBlock = null;
	}

	public void ReCreateTerrain()
	{
		if (m_terrainPlacementManager != null)
		{
			m_terrainPlacementManager.ReCreateTerrain();
		}
	}

	private void ActiveFirstBlock(Vector3 position, Quaternion rotation, bool isGameStartBlock, bool insertStartAct)
	{
		switch (m_orderType)
		{
		case OrderType.DEBUG:
			m_blockOrder = null;
			break;
		default:
			m_blockOrder = null;
			MakeOrderTable(m_nowBlockLevelNum, m_firstBlockNoOnLevel[m_nowBlockLevelNum], isGameStartBlock, insertStartAct);
			break;
		case OrderType.BOSS_SINGLE:
		case OrderType.TUTORIAL:
			break;
		}
		if (m_blockOrder != null && m_blockOrder.Length > 0)
		{
			m_currentOrderNum = 0;
			StageBlockInfo stageBlockInfo = ActivateBlock(m_blockOrder[m_currentOrderNum], position, true, m_orderType, m_currentOrderNum, MsgActivateBlock.CheckPoint.None);
			if (stageBlockInfo != null)
			{
				ChangeCurrentBlock(stageBlockInfo);
			}
		}
	}

	private void SetOrderMode(OrderType type)
	{
		m_orderType = type;
	}

	private void SetOrderModeToDefault()
	{
		m_orderType = m_firstOrderType;
	}

	private void ChangeNextOrderModeToFeverBoss()
	{
		SetOrderMode(OrderType.BOSS_SINGLE);
		CreateBlockOrderInfo(1);
		int layer = Mathf.Min(1 + m_nowBlockLevelNum, 3);
		m_blockOrder[0].SetBlockAndFixedLayer(0, layer);
		m_blockOrder[0].isBoss = true;
		m_currentOrderNum = 0;
		m_nextBossBlock = true;
	}

	private void ResetLevelInformationOnReplace()
	{
		if (m_levelInformation != null && m_blockOrder != null)
		{
			float num = 0f;
			BlockOrderInfo[] blockOrder = m_blockOrder;
			foreach (BlockOrderInfo blockOrderInfo in blockOrder)
			{
				num += GetTerrainLength(blockOrderInfo.blockNo);
			}
			m_levelInformation.DistanceToBossOnStart = num;
			m_levelInformation.DistanceOnStage = 0f;
		}
	}

	private StageBlockInfo ActivateBlock(BlockOrderInfo orderInfo, Vector3 originPoint, bool replaceStage, OrderType orderType, int orderNum, MsgActivateBlock.CheckPoint checkPoint)
	{
		if (orderInfo == null)
		{
			return null;
		}
		float terrainLength = GetTerrainLength(orderInfo.blockNo);
		if (App.Math.NearZero(terrainLength))
		{
			return null;
		}
		StageBlockInfo stageBlockInfo = new StageBlockInfo();
		m_numCreateBlock++;
		stageBlockInfo.m_activateNo = m_numCreateBlock;
		stageBlockInfo.m_blockNo = orderInfo.blockNo;
		stageBlockInfo.m_layerNo = orderInfo.GetNewLayerNo();
		stageBlockInfo.m_totalLength = terrainLength;
		stageBlockInfo.m_startPoint = originPoint;
		stageBlockInfo.m_endPoint = originPoint + new Vector3(terrainLength, 0f, 0f);
		stageBlockInfo.startCurrentCallback = orderInfo.startCurrentCallback;
		stageBlockInfo.endCurrentCallback = orderInfo.endCurrentCallback;
		stageBlockInfo.m_orderNum = orderNum;
		stageBlockInfo.m_orderType = orderType;
		m_activeBlockInfo.Add(stageBlockInfo.m_activateNo, stageBlockInfo);
		MsgActivateBlock msgActivateBlock = new MsgActivateBlock(m_stageName, stageBlockInfo.m_blockNo, stageBlockInfo.m_layerNo, stageBlockInfo.m_activateNo, originPoint, Quaternion.identity);
		msgActivateBlock.m_replaceStage = replaceStage;
		msgActivateBlock.m_checkPoint = checkPoint;
		ObjectSpawnManager component = base.gameObject.GetComponent<ObjectSpawnManager>();
		if (component != null)
		{
			component.OnActivateBlock(msgActivateBlock);
		}
		TerrainPlacementManager component2 = base.gameObject.GetComponent<TerrainPlacementManager>();
		if (component2 != null)
		{
			component2.OnActivateBlock(msgActivateBlock);
		}
		StageBlockPathManager component3 = base.gameObject.GetComponent<StageBlockPathManager>();
		if (component3 != null)
		{
			component3.OnActivateBlock(msgActivateBlock);
		}
		return stageBlockInfo;
	}

	private void DeactivateBlock(StageBlockInfo info, float pos)
	{
		MsgDeactivateBlock value = new MsgDeactivateBlock(info.m_blockNo, info.m_activateNo, pos);
		base.gameObject.SendMessage("OnDeactivateBlock", value, SendMessageOptions.DontRequireReceiver);
		m_activeBlockInfo.Remove(info.m_activateNo);
	}

	private void ChangeCurrentBlock(StageBlockInfo nextBlock)
	{
		if (CurrentBlock != null && CurrentBlock.endCurrentCallback != null)
		{
			CurrentBlock.endCurrentCallback(CurrentBlock);
		}
		CurrentBlock = nextBlock;
		if (CurrentBlock != null && CurrentBlock.startCurrentCallback != null)
		{
			CurrentBlock.startCurrentCallback(CurrentBlock);
		}
		MsgChangeCurrentBlock value = new MsgChangeCurrentBlock(CurrentBlock.m_blockNo, CurrentBlock.m_layerNo, CurrentBlock.m_activateNo);
		base.gameObject.SendMessage("OnChangeCurerntBlock", value, SendMessageOptions.DontRequireReceiver);
	}

	private void SearchAndDeleteRangeOutedBlock(float pos)
	{
		if (m_activeBlockInfo == null)
		{
			return;
		}
		List<StageBlockInfo> list = null;
		foreach (StageBlockInfo value in m_activeBlockInfo.Values)
		{
			if (value.GetPastDistanceFromEndPoint(pos) > 150f)
			{
				if (list == null)
				{
					list = new List<StageBlockInfo>();
				}
				list.Add(value);
			}
		}
		if (list == null)
		{
			return;
		}
		foreach (StageBlockInfo item in list)
		{
			DeactivateBlock(item, pos);
		}
	}

	private float GetTerrainLength(int block)
	{
		float value = 0f;
		if (m_terrainPlacement.TryGetValue(block, out value))
		{
			return value;
		}
		return 0f;
	}

	private BlockOrderInfo GetNextActivateBlockInfo()
	{
		switch (m_orderType)
		{
		case OrderType.BOSS_SINGLE:
			Debug.Log("CheckNextBlock:BOSS_SINGLE  " + m_currentBlock.m_blockNo);
			m_currentOrderNum = 0;
			break;
		case OrderType.TUTORIAL:
			ChangeNextOrderModeToFeverBoss();
			break;
		default:
			m_currentOrderNum++;
			if (m_currentOrderNum >= m_nowBlockOrderNum)
			{
				ChangeNextOrderModeToFeverBoss();
			}
			else
			{
				Debug.Log("CheckNextBlock " + m_currentOrderNum + " " + m_blockOrder[m_currentOrderNum].blockNo);
			}
			break;
		}
		if (m_currentOrderNum < m_blockOrder.Length)
		{
			return m_blockOrder[m_currentOrderNum];
		}
		return null;
	}

	private void MakeOrderTable(int numBlockLevelNum, int startBlock, bool isGameStartBlock, bool insertStartAct)
	{
		int num = m_currentBlockLevelData[numBlockLevelNum].numPlacement;
		int numBlock = m_currentBlockLevelData[numBlockLevelNum].numBlock;
		int num2 = startBlock + numBlock;
		int num3 = 0;
		if (insertStartAct)
		{
			num++;
		}
		CreateBlockOrderInfo(num);
		if (insertStartAct)
		{
			m_blockOrder[0].SetBlockAndFixedLayer(92, 0);
			BlockOrderInfo obj = m_blockOrder[0];
			obj.startCurrentCallback = (Callback)Delegate.Combine(obj.startCurrentCallback, new Callback(CallbackOnStageStartAct));
			if (isGameStartBlock)
			{
				BlockOrderInfo obj2 = m_blockOrder[0];
				obj2.endCurrentCallback = (Callback)Delegate.Combine(obj2.endCurrentCallback, new Callback(CallbackOnStageStartActEnd));
			}
			else
			{
				BlockOrderInfo obj3 = m_blockOrder[0];
				obj3.endCurrentCallback = (Callback)Delegate.Combine(obj3.endCurrentCallback, new Callback(CallbackOnStageReplaceActEnd));
			}
			num3 = 1;
		}
		switch (m_orderType)
		{
		case OrderType.ASCENDING:
		{
			int num4 = startBlock;
			for (int i = num3; i < m_blockOrder.Length; i++)
			{
				m_blockOrder[i].SetBlockAndFixedLayer(num4, m_currentBlockLevelData[m_nowBlockLevelNum].layerNum);
				if (++num4 >= num2)
				{
					num4 = startBlock;
				}
			}
			break;
		}
		case OrderType.RANDOM:
			if (m_highSpeedSetFlag)
			{
				MakeOrderHighSpeedTable(numBlockLevelNum, startBlock, num3, isGameStartBlock, insertStartAct);
			}
			else
			{
				MakeOrderLowSpeedTable(numBlockLevelNum, startBlock, num3, isGameStartBlock, insertStartAct);
			}
			break;
		}
		m_currentOrderNum = 0;
		m_nowBlockOrderNum = num;
	}

	private void MakeOrderHighSpeedTable(int numBlockLevelNum, int startBlock, int startOrderNum, bool isGameStartBlock, bool insertStartAct)
	{
		int numPlacement = m_currentBlockLevelData[numBlockLevelNum].numPlacement;
		int numBlock = m_currentBlockLevelData[numBlockLevelNum].numBlock;
		int num = startBlock + numBlock;
		int num2 = Mathf.Min(m_highSpeedCount * 2, 6);
		int num3 = Mathf.Min(num2 * 5, numBlock);
		int[] array = new int[num3];
		int num4 = 0;
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				array[num4] = m_highSpeedTable[i] + j;
				if (array[num4] >= num)
				{
					array[num4] = startBlock;
				}
				num4++;
				if (num4 >= num3)
				{
					num4 = num3 - 1;
				}
			}
		}
		App.Random.ShuffleInt(array);
		int num5 = 0;
		if (insertStartAct)
		{
			num5++;
		}
		for (int k = 0; k < array.Length; k++)
		{
			if (num5 >= m_blockOrder.Length)
			{
				break;
			}
			int fixedLayerNo = m_currentBlockLevelData[m_nowBlockLevelNum].fixedLayerNo;
			if (fixedLayerNo == -1)
			{
				int layerNum = m_currentBlockLevelData[m_nowBlockLevelNum].layerNum;
				m_blockOrder[num5].SetBlockAndRandomLayer(array[k], layerNum);
			}
			else
			{
				m_blockOrder[num5].SetBlockAndFixedLayer(array[k], fixedLayerNo);
			}
			num5++;
		}
		m_highSpeedCount++;
	}

	private void MakeOrderLowSpeedTable(int numBlockLevelNum, int startBlock, int startOrderNum, bool isGameStartBlock, bool insertStartAct)
	{
		int numPlacement = m_currentBlockLevelData[numBlockLevelNum].numPlacement;
		int numBlock = m_currentBlockLevelData[numBlockLevelNum].numBlock;
		int num = startBlock + numBlock;
		int num2 = (numBlock <= numPlacement) ? numPlacement : numBlock;
		int num3 = startBlock;
		int fixedBlockNo = m_startingBlockInfo.fixedBlockNo;
		if (isGameStartBlock && !m_quickMode)
		{
			int num4 = startOrderNum;
			for (int i = 0; i < fixedBlockNo; i++)
			{
				int startingLayerNum = m_startingBlockInfo.startingLayerNum;
				m_blockOrder[num4].SetBlockAndRandomLayer(startBlock + i, startingLayerNum);
				num4++;
			}
			num3 += fixedBlockNo;
			num2 -= fixedBlockNo;
		}
		int[] array = new int[num2];
		for (int j = 0; j < num2; j++)
		{
			array[j] = num3;
			if (++num3 >= num)
			{
				num3 = startBlock;
			}
		}
		App.Random.ShuffleInt(array);
		int num5 = (isGameStartBlock && !m_quickMode) ? fixedBlockNo : 0;
		if (insertStartAct)
		{
			num5++;
		}
		for (int k = 0; k < array.Length; k++)
		{
			if (num5 >= m_blockOrder.Length)
			{
				break;
			}
			int fixedLayerNo = m_currentBlockLevelData[m_nowBlockLevelNum].fixedLayerNo;
			if (fixedLayerNo == -1)
			{
				int layerNum = m_currentBlockLevelData[m_nowBlockLevelNum].layerNum;
				m_blockOrder[num5].SetBlockAndRandomLayer(array[k], layerNum);
			}
			else
			{
				m_blockOrder[num5].SetBlockAndFixedLayer(array[k], fixedLayerNo);
			}
			num5++;
		}
	}

	private BlockOrderInfo GetCurerntBlockOrderInfo()
	{
		return m_blockOrder[m_currentOrderNum];
	}

	public int GetBlockLevel()
	{
		return m_nowBlockLevelNum;
	}

	public bool IsBlockLevelUp()
	{
		return m_nowBlockLevelNum != m_nextBlockLevelNum;
	}

	private void OnMsgPrepareStageReplace(MsgPrepareStageReplace msg)
	{
		if (m_objectSpawnableManager == null)
		{
			return;
		}
		switch (m_firstOrderType)
		{
		case OrderType.BOSS_SINGLE:
			return;
		case OrderType.TUTORIAL:
			return;
		case OrderType.DEBUG:
			StartCoroutine(m_objectSpawnableManager.LoadSetTable(1, 91));
			return;
		}
		m_highSpeedSetFlag = (m_nextBlockLevelNum > 1);
		if (m_highSpeedSetFlag)
		{
			int readFileCount = Mathf.Min(m_highSpeedCount * 2, 6);
			StartCoroutine(m_objectSpawnableManager.LoadSetTable(m_highSpeedTable, readFileCount));
		}
		else
		{
			int num = Mathf.Min(m_nextBlockLevelNum, m_firstBlockNoOnLevel.Length - 1);
			StartCoroutine(m_objectSpawnableManager.LoadSetTable(m_firstBlockNoOnLevel[num], m_currentBlockLevelData[num].numBlock));
		}
	}

	public void OnMsgStageReplace(MsgStageReplace msg)
	{
		m_totalDistance = 0f;
		base.transform.position = msg.m_position;
		DeactivateAll();
		SetOrderModeToDefault();
		m_nowBlockLevelNum = Mathf.Min(m_nextBlockLevelNum, m_currentBlockLevelData.Length - 1);
		m_stageRepeatNum++;
		if (m_nowBlockLevelNum < m_currentBlockLevelData.Length && m_stageRepeatNum >= m_currentBlockLevelData[m_nowBlockLevelNum].repeatNum)
		{
			m_stageRepeatNum = 0;
			m_nextBlockLevelNum++;
		}
		bool flag = false;
		if (m_quickMode)
		{
			flag = !m_quickFirstReplaceEnd;
			m_quickFirstReplaceEnd = true;
		}
		else
		{
			flag = (msg.m_speedLevel == PlayerSpeed.LEVEL_1);
		}
		ActiveFirstBlock(msg.m_position, msg.m_rotation, flag, true);
		ResetLevelInformationOnReplace();
	}

	private void OnMsgSetStageOnMapBoss(MsgSetStageOnMapBoss msg)
	{
		m_totalDistance = 0f;
		base.transform.position = msg.m_position;
		int layer = 0;
		switch (BossTypeUtil.GetBossCategory(msg.m_bossType))
		{
		case BossCategory.MAP:
			layer = 11 + BossTypeUtil.GetLayerNumber(msg.m_bossType);
			break;
		case BossCategory.EVENT:
			layer = 21 + BossTypeUtil.GetLayerNumber(msg.m_bossType);
			break;
		}
		SetOrderMode(OrderType.BOSS_SINGLE);
		CreateBlockOrderInfo(1);
		m_blockOrder[0].SetBlockAndFixedLayer(0, layer);
		if (m_levelInformation != null)
		{
			m_levelInformation.DistanceToBossOnStart = 0f;
		}
		ActiveFirstBlock(msg.m_position, msg.m_rotation, false, false);
	}

	public void SetStageOnTutorial(Vector3 position)
	{
		m_totalDistance = 0f;
		base.transform.position = position;
		m_nowBlockLevelNum = 0;
		SetOrderMode(OrderType.TUTORIAL);
		CreateBlockOrderInfo(1);
		m_blockOrder[0].SetBlockAndFixedLayer(91, 0);
		ActiveFirstBlock(position, Quaternion.identity, false, false);
		ResetLevelInformationOnReplace();
	}

	private void OnMsgTutorialResetForRetry(MsgTutorialResetForRetry msg)
	{
		base.transform.position = msg.m_position;
		OrderType orderType = m_currentBlock.m_orderType;
		Vector3 startPoint = m_currentBlock.m_startPoint;
		DeactivateAll();
		if (orderType == OrderType.TUTORIAL)
		{
			SetOrderMode(OrderType.TUTORIAL);
			CreateBlockOrderInfo(1);
			m_blockOrder[0].SetBlockAndFixedLayer(91, 0);
		}
		StageBlockInfo stageBlockInfo = ActivateBlock(m_blockOrder[m_currentOrderNum], startPoint, true, m_orderType, m_currentOrderNum, MsgActivateBlock.CheckPoint.None);
		if (stageBlockInfo != null)
		{
			ChangeCurrentBlock(stageBlockInfo);
		}
	}

	private void CallbackOnStageStartAct(StageBlockInfo info)
	{
		ObjUtil.PushCamera(CameraType.START_ACT, 0.1f);
		ObjUtil.PauseCombo(MsgPauseComboTimer.State.PAUSE);
	}

	private void CallbackOnStageStartActEnd(StageBlockInfo info)
	{
		ObjUtil.PopCamera(CameraType.START_ACT, 0f);
		SetQuickModeStart();
		ObjUtil.SendStartItemAndChao();
		ObjUtil.PauseCombo(MsgPauseComboTimer.State.PLAY);
	}

	private void CallbackOnStageReplaceActEnd(StageBlockInfo info)
	{
		ObjUtil.PopCamera(CameraType.START_ACT, 0f);
		SetQuickModeStart();
		if (StageItemManager.Instance != null)
		{
			MsgPhatomItemOnBoss msg = new MsgPhatomItemOnBoss();
			StageItemManager.Instance.OnResumeItemOnBoss(msg);
		}
		GameObjectUtil.SendMessageToTagObjects("Chao", "OnResumeChangeLevel", null, SendMessageOptions.DontRequireReceiver);
		ObjUtil.PauseCombo(MsgPauseComboTimer.State.PLAY_RESET);
	}

	private BlockOrderInfo[] CreateBlockOrderInfo(int num)
	{
		m_blockOrder = new BlockOrderInfo[num];
		for (int i = 0; i < m_blockOrder.Length; i++)
		{
			m_blockOrder[i] = new BlockOrderInfo();
		}
		return m_blockOrder;
	}

	private bool IsHighSpeed()
	{
		if (m_nextBlockLevelNum > 1 && m_nowBlockLevelNum > 1)
		{
			return true;
		}
		return false;
	}

	private void SetQuickModeStart()
	{
		if (m_quickMode && StageTimeManager.Instance != null)
		{
			StageTimeManager.Instance.PlayStart();
		}
	}

	public StageBlockInfo GetCurrenBlockInfo()
	{
		return m_currentBlock;
	}

	public float GetBlockLocalDistance()
	{
		if (CurrentBlock != null)
		{
			return CurrentBlock.GetBlockRunningLength(m_totalDistance);
		}
		return 0f;
	}

	public Vector3 GetBlockLocalPosition(Vector3 pos)
	{
		if (CurrentBlock != null)
		{
			return pos - CurrentBlock.m_startPoint;
		}
		return Vector3.zero;
	}
}
