using System.Collections.Generic;

public class BlockSpawnableParameterContainer
{
	public readonly int m_block;

	public readonly int m_layer;

	private List<SpawnableParameter> m_parameters;

	public int Block
	{
		get
		{
			return m_block;
		}
	}

	public int Layer
	{
		get
		{
			return m_layer;
		}
	}

	public BlockSpawnableParameterContainer(int blk, int ptn)
	{
		m_parameters = new List<SpawnableParameter>();
		m_block = blk;
		m_layer = ptn;
	}

	private BlockSpawnableParameterContainer()
	{
		m_parameters = new List<SpawnableParameter>();
	}

	public void AddParameter(SpawnableParameter param)
	{
		m_parameters.Add(param);
	}

	public SpawnableParameter GetParameter(int id)
	{
		if (id >= m_parameters.Count)
		{
			return null;
		}
		return m_parameters[id];
	}

	public List<SpawnableParameter> GetParameters()
	{
		return m_parameters;
	}
}
