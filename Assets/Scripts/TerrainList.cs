using System.Collections.Generic;

internal class TerrainList
{
	private Dictionary<string, Terrain> m_terrainList;

	public string m_name
	{
		get;
		private set;
	}

	public TerrainList(string name)
	{
		m_name = name;
		m_terrainList = new Dictionary<string, Terrain>();
	}

	public void AddTerrain(Terrain terrain)
	{
		m_terrainList.Add(terrain.m_name, terrain);
	}

	public int GetTerrainCount()
	{
		return m_terrainList.Count;
	}

	public Terrain GetTerrain(string name)
	{
		if (!m_terrainList.ContainsKey(name))
		{
			return null;
		}
		return m_terrainList[name];
	}

	public Terrain GetTerrain(int index)
	{
		string name = string.Format("{0:00}", index);
		return GetTerrain(name);
	}
}
