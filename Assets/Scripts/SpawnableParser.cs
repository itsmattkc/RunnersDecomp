using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEngine;

public class SpawnableParser : MonoBehaviour
{
	public enum Object
	{
		Object,
		ID,
		ClassName,
		ParameterName,
		RangeIn,
		RangeOut,
		Position,
		Angle,
		NUM
	}

	public enum Parameter
	{
		Parameters,
		Item,
		Name,
		Type,
		Value,
		NUM
	}

	public static readonly string[] ObjectKeyTable = new string[8]
	{
		"Obj",
		"ID",
		"C",
		"P",
		"I",
		"O",
		"P",
		"A"
	};

	public static readonly string[] ParameterKeyTable = new string[5]
	{
		"PS",
		"It",
		"N",
		"T",
		"V"
	};

	public IEnumerator CreateSetData(ResourceManager resManager, TextAsset xmlFile, StageSpawnableParameterContainer stageDataList)
	{
		TimeProfiler.StartCountTime("CreateSetData:CreateSetData");
		if (xmlFile != null)
		{
			TimeProfiler.StartCountTime("CreateSetData:AESCrypt.Decrypt");
			string text_data = AESCrypt.Decrypt(xmlFile.text);
			TimeProfiler.EndCountTime("CreateSetData:AESCrypt.Decrypt");
			XmlReader reader = XmlReader.Create(new StringReader(text_data));
			if (reader != null)
			{
				reader.Read();
				if (reader.ReadToFollowing("Stage") && reader.ReadToDescendant("Block"))
				{
					do
					{
						if (reader.MoveToAttribute("ID"))
						{
							int blockID = int.Parse(reader.Value);
							reader.MoveToElement();
							if (reader.ReadToDescendant("Layer"))
							{
								yield return StartCoroutine(ReadLayer(resManager, reader, blockID, stageDataList));
							}
						}
					}
					while (reader.ReadToNextSibling("Block"));
				}
				reader.Close();
			}
		}
		TimeProfiler.EndCountTime("CreateSetData:CreateSetData");
	}

	private static Vector3 ReadVector3(XmlReader reader)
	{
		Vector3 zero = Vector3.zero;
		reader.MoveToFirstAttribute();
		do
		{
			switch (reader.Name)
			{
			case "X":
				zero.x = float.Parse(reader.Value);
				break;
			case "Y":
				zero.y = float.Parse(reader.Value);
				break;
			case "Z":
				zero.z = float.Parse(reader.Value);
				break;
			}
		}
		while (reader.MoveToNextAttribute());
		reader.MoveToElement();
		return zero;
	}

	private static void ReadUserParameters(ResourceManager resManager, XmlReader reader, object o, Type t, string parameterName)
	{
		SpawnableParameter spawnableParameter = o as SpawnableParameter;
		if (spawnableParameter == null || !reader.ReadToDescendant("It"))
		{
			return;
		}
		do
		{
			reader.MoveToFirstAttribute();
			string name = null;
			string text = null;
			string text2 = null;
			do
			{
				switch (reader.Name)
				{
				case "N":
					name = reader.Value;
					break;
				case "V":
					text = reader.Value;
					break;
				case "T":
					text2 = reader.Value;
					break;
				}
			}
			while (reader.MoveToNextAttribute());
			reader.MoveToElement();
			FieldInfo field = t.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field == null)
			{
				continue;
			}
			switch (text2)
			{
			case "g":
			{
				GameObject spawnableGameObject = resManager.GetSpawnableGameObject(text);
				if (spawnableGameObject != null)
				{
					field.SetValue(o, spawnableGameObject);
				}
				break;
			}
			case "f":
			{
				float result2;
				if (float.TryParse(text, out result2))
				{
					field.SetValue(o, result2);
				}
				break;
			}
			case "i":
			{
				int result3;
				if (int.TryParse(text, out result3))
				{
					field.SetValue(o, result3);
				}
				break;
			}
			case "u":
			{
				uint result;
				if (uint.TryParse(text, out result))
				{
					result = uint.Parse(text, NumberStyles.AllowHexSpecifier);
					field.SetValue(o, result);
				}
				break;
			}
			case "s":
			{
				string text3 = text;
				if (text3 != null)
				{
					field.SetValue(o, text3);
				}
				break;
			}
			}
		}
		while (reader.ReadToNextSibling("It"));
	}

	private void ReadObjects(ResourceManager resManager, XmlReader reader, BlockSpawnableParameterContainer blockData)
	{
		do
		{
			string text = null;
			uint iD = 0u;
			string text2 = null;
			int num = 20;
			int num2 = 30;
			reader.MoveToFirstAttribute();
			do
			{
				switch (reader.Name)
				{
				case "C":
					text = reader.Value;
					break;
				case "ID":
					iD = uint.Parse(reader.Value, NumberStyles.AllowHexSpecifier);
					break;
				case "P":
					text2 = reader.Value;
					break;
				case "I":
					num = int.Parse(reader.Value);
					break;
				case "O":
					num2 = int.Parse(reader.Value);
					break;
				}
			}
			while (reader.MoveToNextAttribute());
			reader.MoveToElement();
			if (string.IsNullOrEmpty(text))
			{
				continue;
			}
			Type type = null;
			object obj = null;
			SpawnableParameter spawnableParameter = null;
			if (!string.IsNullOrEmpty(text2))
			{
				type = Type.GetType(text2);
				if (type != null)
				{
					obj = Activator.CreateInstance(type);
					spawnableParameter = (obj as SpawnableParameter);
				}
				if (spawnableParameter == null)
				{
					type = null;
					obj = null;
					spawnableParameter = new SpawnableParameter();
					text2 = null;
				}
			}
			else
			{
				spawnableParameter = new SpawnableParameter();
			}
			spawnableParameter.ObjectName = text;
			spawnableParameter.ID = iD;
			spawnableParameter.RangeIn = num;
			spawnableParameter.RangeOut = num2;
			XmlReader xmlReader = reader.ReadSubtree();
			while (xmlReader.Read())
			{
				switch (xmlReader.Name)
				{
				case "P":
					spawnableParameter.Position = ReadVector3(xmlReader);
					break;
				case "A":
				{
					Vector3 euler = ReadVector3(xmlReader);
					spawnableParameter.Rotation = Quaternion.Euler(euler);
					break;
				}
				case "PS":
					if (!string.IsNullOrEmpty(text2))
					{
						ReadUserParameters(resManager, xmlReader, obj, type, text2);
					}
					else
					{
						xmlReader.Skip();
					}
					break;
				}
			}
			xmlReader.Close();
			blockData.AddParameter(spawnableParameter);
		}
		while (reader.ReadToNextSibling("Obj"));
	}

	private IEnumerator ReadLayer(ResourceManager resManager, XmlReader reader, int blockID, StageSpawnableParameterContainer stageDataList)
	{
		do
		{
			if (reader.MoveToAttribute("ID"))
			{
				int layerID = int.Parse(reader.Value);
				reader.MoveToElement();
				if (reader.ReadToDescendant("Obj"))
				{
					BlockSpawnableParameterContainer blockData = new BlockSpawnableParameterContainer(blockID, layerID);
					ReadObjects(resManager, reader, blockData);
					stageDataList.AddData(blockID, layerID, blockData);
				}
			}
			yield return null;
		}
		while (reader.ReadToNextSibling("Layer"));
	}
}
