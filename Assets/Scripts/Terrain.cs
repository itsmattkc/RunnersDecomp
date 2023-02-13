using System.Collections.Generic;

public class Terrain
{
	private List<TerrainBlock> m_blocks;

	public string m_name
	{
		get;
		private set;
	}

	public float m_meter
	{
		get;
		private set;
	}

	public Terrain(string name, float meter)
	{
		m_name = name;
		m_meter = meter;
		m_blocks = new List<TerrainBlock>();
	}

	public void AddTerrainBlock(TerrainBlock terrainBlock)
	{
		m_blocks.Add(terrainBlock);
	}

	public int GetBlockCount()
	{
		return m_blocks.Count;
	}

	public TerrainBlock GetBlock(int index)
	{
		if (index >= GetBlockCount())
		{
			return null;
		}
		return m_blocks[index];
	}
}
