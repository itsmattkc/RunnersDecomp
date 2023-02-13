using UnityEngine;

public class TerrainXmlData : MonoBehaviour
{
	public const string DefaultSetDataAssetName = "TerrainBlockData";

	[SerializeField]
	private TextAsset m_terrainBlock;

	[SerializeField]
	private TextAsset m_sideViewPath;

	[SerializeField]
	private TextAsset m_loopPath;

	[SerializeField]
	private TextAsset[] m_setData = new TextAsset[22];

	[SerializeField]
	private TextAsset m_itemTableData;

	[SerializeField]
	private TextAsset m_rareEnemyTableData;

	[SerializeField]
	private TextAsset m_bossTableData;

	[SerializeField]
	private TextAsset m_bossMap3TableData;

	[SerializeField]
	private TextAsset m_objectPartTableData;

	[SerializeField]
	private TextAsset m_EnemyExtendItemTableData;

	[SerializeField]
	private int m_moveTrapBooRand;

	public static string m_setDataAssetName = "TerrainBlockData";

	public static string DataAssetName
	{
		get
		{
			return m_setDataAssetName;
		}
	}

	public TextAsset TerrainBlock
	{
		get
		{
			return m_terrainBlock;
		}
	}

	public TextAsset SideViewPath
	{
		get
		{
			return m_sideViewPath;
		}
	}

	public TextAsset LoopPath
	{
		get
		{
			return m_loopPath;
		}
	}

	public TextAsset[] SetData
	{
		get
		{
			return m_setData;
		}
	}

	public TextAsset ItemTableData
	{
		get
		{
			return m_itemTableData;
		}
	}

	public TextAsset RareEnemyTableData
	{
		get
		{
			return m_rareEnemyTableData;
		}
	}

	public TextAsset BossTableData
	{
		get
		{
			return m_bossTableData;
		}
	}

	public TextAsset BossMap3TableData
	{
		get
		{
			return m_bossMap3TableData;
		}
	}

	public TextAsset ObjectPartTableData
	{
		get
		{
			return m_objectPartTableData;
		}
	}

	public TextAsset EnemyExtendItemTableData
	{
		get
		{
			return m_EnemyExtendItemTableData;
		}
	}

	public int MoveTrapBooRand
	{
		get
		{
			return m_moveTrapBooRand;
		}
	}

	public static void SetAssetName(string stageName)
	{
		m_setDataAssetName = stageName + "_TerrainBlockData";
	}
}
