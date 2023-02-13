public class TerrainBlock
{
	public string m_name
	{
		get;
		private set;
	}

	public TransformParam m_transform
	{
		get;
		private set;
	}

	public TerrainBlock(string name, TransformParam transform)
	{
		m_name = name;
		m_transform = transform;
	}
}
