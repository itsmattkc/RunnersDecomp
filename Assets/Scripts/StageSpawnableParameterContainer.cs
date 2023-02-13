using System.Collections.Generic;

public class StageSpawnableParameterContainer
{
	private Dictionary<uint, BlockSpawnableParameterContainer> m_setData;

	public StageSpawnableParameterContainer()
	{
		m_setData = new Dictionary<uint, BlockSpawnableParameterContainer>();
	}

	public void AddData(int block, int layer, BlockSpawnableParameterContainer data)
	{
		uint uniqueID = GetUniqueID(block, layer);
		m_setData.Add(uniqueID, data);
	}

	public BlockSpawnableParameterContainer GetBlockData(int blockID, int layerID)
	{
		uint uniqueID = GetUniqueID(blockID, layerID);
		BlockSpawnableParameterContainer value;
		m_setData.TryGetValue(uniqueID, out value);
		return value;
	}

	public SpawnableParameter GetParameter(int blockID, int layerID, int id)
	{
		uint uniqueID = GetUniqueID(blockID, layerID);
		if (!m_setData.ContainsKey(uniqueID))
		{
			return null;
		}
		BlockSpawnableParameterContainer blockSpawnableParameterContainer = m_setData[uniqueID];
		if (blockSpawnableParameterContainer != null)
		{
			return blockSpawnableParameterContainer.GetParameter(id);
		}
		return null;
	}

	private static uint GetUniqueID(int block, int layer)
	{
		return (uint)((block << 4) + layer);
	}
}
