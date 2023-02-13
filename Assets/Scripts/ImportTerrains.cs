using System.Xml;
using UnityEngine;

internal class ImportTerrains
{
	public static TerrainList Import(TextAsset textAsset)
	{
		if (textAsset == null)
		{
			return null;
		}
		string xml = AESCrypt.Decrypt(textAsset.text);
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(xml);
		return Import(xmlDocument);
	}

	public static TerrainList Import(XmlDocument document)
	{
		if (document == null)
		{
			return null;
		}
		XmlNode documentElement = document.DocumentElement;
		if (documentElement == null)
		{
			return null;
		}
		XmlNode namedItem = documentElement.Attributes.GetNamedItem("name");
		if (namedItem == null)
		{
			return null;
		}
		TerrainList terrainList = new TerrainList(namedItem.Value);
		XmlNodeList childNodes = documentElement.ChildNodes;
		foreach (XmlNode item in childNodes)
		{
			Terrain terrain = GetTerrain(item);
			terrainList.AddTerrain(terrain);
		}
		return terrainList;
	}

	private static Terrain GetTerrain(XmlNode terrainNode)
	{
		XmlNode namedItem = terrainNode.Attributes.GetNamedItem("name");
		if (namedItem == null)
		{
			return null;
		}
		XmlNode xmlNode = terrainNode.SelectSingleNode("Meter");
		if (xmlNode == null)
		{
			return null;
		}
		string value = namedItem.Value;
		string value2 = xmlNode.Attributes.GetNamedItem("value").Value;
		Terrain terrain = new Terrain(value, float.Parse(value2));
		XmlNodeList childNodes = xmlNode.ChildNodes;
		foreach (XmlNode item in childNodes)
		{
			TerrainBlock terrainBlock = GetTerrainBlock(item);
			if (terrainBlock != null)
			{
				terrain.AddTerrainBlock(terrainBlock);
			}
		}
		return terrain;
	}

	private static TerrainBlock GetTerrainBlock(XmlNode blockNode)
	{
		if (blockNode == null)
		{
			return null;
		}
		XmlNode namedItem = blockNode.Attributes.GetNamedItem("name");
		if (namedItem == null)
		{
			return null;
		}
		string value = namedItem.Value;
		if (value == null)
		{
			return null;
		}
		TransformParam transformParam = GetTransformParam(blockNode);
		if (transformParam == null)
		{
			return null;
		}
		return new TerrainBlock(value, transformParam);
	}

	private static TransformParam GetTransformParam(XmlNode blockNode)
	{
		XmlNode xmlNode = blockNode.SelectSingleNode("Position");
		if (xmlNode == null)
		{
			return null;
		}
		Vector3 pos = default(Vector3);
		float floatValue = GetFloatValue(xmlNode, "posX");
		float floatValue2 = GetFloatValue(xmlNode, "posY");
		float floatValue3 = GetFloatValue(xmlNode, "posZ");
		pos.Set(floatValue, floatValue2, floatValue3);
		XmlNode xmlNode2 = blockNode.SelectSingleNode("Rotation");
		if (xmlNode2 == null)
		{
			return null;
		}
		Vector3 rot = default(Vector3);
		float floatValue4 = GetFloatValue(xmlNode2, "rotX");
		float floatValue5 = GetFloatValue(xmlNode2, "rotY");
		float floatValue6 = GetFloatValue(xmlNode2, "rotZ");
		rot.Set(floatValue4, floatValue5, floatValue6);
		return new TransformParam(pos, rot);
	}

	private static float GetFloatValue(XmlNode node, string attributeName)
	{
		if (node == null)
		{
			return 0f;
		}
		XmlNode namedItem = node.Attributes.GetNamedItem(attributeName);
		if (namedItem == null)
		{
			return 0f;
		}
		string value = namedItem.Value;
		if (value == null)
		{
			return 0f;
		}
		return float.Parse(value);
	}
}
