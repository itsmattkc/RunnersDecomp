using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class PathXmlDeserializer
{
	public static IEnumerator CreatePathObjectData(TextAsset xmlData, Dictionary<string, ResPathObjectData> dictonary)
	{
		XmlDocument doc = new XmlDocument();
		doc.LoadXml(xmlData.text);
		yield return null;
		XmlNodeList pathObjDataNode = doc.GetElementsByTagName("ResPathObjectData");
		foreach (XmlNode rootNode in pathObjDataNode)
		{
			ResPathObjectData pathObj = new ResPathObjectData
			{
				name = rootNode.Attributes["Name"].Value
			};
			int indexAt = pathObj.name.IndexOf("@");
			if (indexAt >= 0)
			{
				pathObj.name = pathObj.name.Substring(0, indexAt);
			}
			pathObj.name = pathObj.name.ToLower();
			int value2 = int.Parse(rootNode.Attributes["PlaybackType"].Value);
			pathObj.playbackType = (byte)value2;
			int value = int.Parse(rootNode.Attributes["Flags"].Value);
			pathObj.flags = (byte)value;
			pathObj.numKeys = ushort.Parse(rootNode.Attributes["NumKeys"].Value);
			pathObj.length = float.Parse(rootNode.Attributes["Length"].Value);
			pathObj.distance = new float[pathObj.numKeys];
			pathObj.position = new Vector3[pathObj.numKeys];
			pathObj.normal = new Vector3[pathObj.numKeys];
			pathObj.tangent = new Vector3[pathObj.numKeys];
			XmlNode distance = rootNode.SelectSingleNode("Distance");
			ParseFloatArray(distance, pathObj.distance, pathObj.numKeys);
			XmlNode position = rootNode.SelectSingleNode("Position");
			ParseVector3Array(position, pathObj.position, pathObj.numKeys);
			XmlNode normal = rootNode.SelectSingleNode("Normal");
			ParseVector3Array(normal, pathObj.normal, pathObj.numKeys);
			XmlNode tangent = rootNode.SelectSingleNode("Tangent");
			ParseVector3Array(tangent, pathObj.tangent, pathObj.numKeys);
			XmlNode vertNode = rootNode.SelectSingleNode("Vertices");
			pathObj.numVertices = uint.Parse(vertNode.Attributes["value"].Value);
			if (pathObj.numVertices != 0)
			{
				pathObj.vertices = new Vector3[pathObj.numVertices];
				ParseVector3Array(vertNode, pathObj.vertices, (int)pathObj.numVertices);
			}
			Vector3 vec2 = default(Vector3);
			XmlNode minNode = rootNode.SelectSingleNode("Min");
			XmlNode item2 = minNode.SelectSingleNode("item");
			ParseVector3(item2, ref vec2);
			pathObj.min = vec2;
			vec2 = default(Vector3);
			XmlNode maxNode = rootNode.SelectSingleNode("Max");
			item2 = maxNode.SelectSingleNode("item");
			ParseVector3(item2, ref vec2);
			pathObj.max = vec2;
			pathObj.uid = uint.Parse(rootNode.Attributes["UniqueID"].Value);
			dictonary.Add(pathObj.name, pathObj);
		}
		yield return null;
	}

	private static void ParseFloatArray(XmlNode node, float[] array, int numOfArray)
	{
		if (node == null)
		{
			return;
		}
		XmlNodeList xmlNodeList = node.SelectNodes("item");
		if (xmlNodeList.Count == numOfArray && xmlNodeList.Count > 0)
		{
			int num = 0;
			foreach (XmlNode item in xmlNodeList)
			{
				float.TryParse(item.InnerText, out array[num]);
				num++;
			}
		}
		else
		{
			Debug.Log("Array Num is not Equal keyNum");
		}
	}

	private static void ParseVector3Array(XmlNode node, Vector3[] array, int numOfArray)
	{
		if (node == null)
		{
			return;
		}
		XmlNodeList xmlNodeList = node.SelectNodes("item");
		if (xmlNodeList.Count == numOfArray && xmlNodeList.Count > 0)
		{
			int num = 0;
			foreach (XmlNode item in xmlNodeList)
			{
				ParseVector3(item, ref array[num]);
				num++;
			}
		}
		else
		{
			Debug.Log("Array Num is not Equal keyNum");
		}
	}

	private static void ParseVector3(XmlNode node, ref Vector3 vec)
	{
		if (node != null)
		{
			float.TryParse(node.Attributes["X"].Value, out vec.x);
			float.TryParse(node.Attributes["Y"].Value, out vec.y);
			float.TryParse(node.Attributes["Z"].Value, out vec.z);
		}
	}
}
