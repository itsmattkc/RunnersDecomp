using SaveData;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

public class SaveLoad
{
	private const string PLAYER_FILE_PATH = "Assets/Runners Assets/Save/save_data_player.xml";

	private const string CHAO_FILE_PATH = "Assets/Runners Assets/Save/save_data_chao.xml";

	private const string OPTION_FILE_PATH = "Assets/Runners Assets/Save/save_data_option.xml";

	private const string ITEM_FILE_PATH = "Assets/Runners Assets/Save/save_data_item.xml";

	private const string CHARA_FILE_PATH = "Assets/Runners Assets/Save/save_data_chara.xml";

	public static void SaveData<Type>(Type obj)
	{
		System.Type type = obj.GetType();
		if (type == typeof(PlayerData))
		{
			CreateSaveData(obj, "Assets/Runners Assets/Save/save_data_player.xml");
		}
		else if (type == typeof(CharaData))
		{
			CreateSaveData(obj, "Assets/Runners Assets/Save/save_data_chara.xml");
		}
		else if (type == typeof(ChaoData))
		{
			CreateSaveData(obj, "Assets/Runners Assets/Save/save_data_chao.xml");
		}
		else if (type == typeof(ItemData))
		{
			CreateSaveData(obj, "Assets/Runners Assets/Save/save_data_item.xml");
		}
		else if (type == typeof(OptionData))
		{
			CreateSaveData(obj, "Assets/Runners Assets/Save/save_data_option.xml");
		}
	}

	public static void LoadSaveData<Type>(ref Type obj)
	{
		System.Type type = obj.GetType();
		if (type == typeof(PlayerData))
		{
			LoadSaveData(ref obj, "Assets/Runners Assets/Save/save_data_player.xml");
		}
		else if (type == typeof(CharaData))
		{
			LoadSaveData(ref obj, "Assets/Runners Assets/Save/save_data_chara.xml");
		}
		else if (type == typeof(ChaoData))
		{
			LoadSaveData(ref obj, "Assets/Runners Assets/Save/save_data_chao.xml");
		}
		else if (type == typeof(ItemData))
		{
			LoadSaveData(ref obj, "Assets/Runners Assets/Save/save_data_item.xml");
		}
		else if (type == typeof(OptionData))
		{
			LoadSaveData(ref obj, "Assets/Runners Assets/Save/save_data_option.xml");
		}
	}

	private static void LoadSaveData<Type>(ref Type obj, string path)
	{
		if (CheckSaveData(path))
		{
			if (!LoadXMLSaveData(ref obj, path))
			{
				CreateSaveData(obj, path);
			}
		}
		else
		{
			CreateSaveData(obj, path);
		}
	}

	private static void CreateSaveData<Type>(Type obj, string path)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(Type));
		StreamWriter streamWriter = new StreamWriter(path, false, Encoding.UTF8);
		if (streamWriter != null)
		{
			if (obj != null)
			{
				xmlSerializer.Serialize(streamWriter, obj);
			}
			streamWriter.Close();
		}
	}

	private static bool LoadXMLSaveData<Type>(ref Type obj, string path)
	{
		bool result = false;
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(Type));
		StreamReader streamReader = new StreamReader(path, Encoding.UTF8);
		object obj2 = null;
		if (streamReader != null)
		{
			obj2 = (Type)xmlSerializer.Deserialize(streamReader);
			if (obj2 != null)
			{
				obj = (Type)obj2;
				result = true;
			}
			streamReader.Close();
		}
		return result;
	}

	public static bool CheckSaveData(string path)
	{
		if (File.Exists(path))
		{
			return true;
		}
		return false;
	}
}
