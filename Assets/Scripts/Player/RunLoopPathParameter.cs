namespace Player
{
	public class RunLoopPathParameter : StateEnteringParameter
	{
		public PathComponent m_pathComponent;

		public override void Reset()
		{
			m_pathComponent = null;
		}

		public void Set(PathComponent component)
		{
			m_pathComponent = component;
		}
	}
}
