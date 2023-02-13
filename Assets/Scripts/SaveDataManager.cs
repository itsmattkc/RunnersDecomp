using SaveData;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{
	private static PlayerData m_player_data = new PlayerData();

	private static CharaData m_chara_data = new CharaData();

	private static ChaoData m_chao_data = new ChaoData();

	private static ItemData m_item_data = new ItemData();

	private static OptionData m_option_data = new OptionData();

	private static ConnectData m_connect_data = new ConnectData();

	private static SaveDataManager instance = null;

	public static SaveDataManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (Object.FindObjectOfType(typeof(SaveDataManager)) as SaveDataManager);
			}
			return instance;
		}
	}

	public PlayerData PlayerData
	{
		get
		{
			return m_player_data;
		}
		set
		{
			m_player_data = value;
		}
	}

	public CharaData CharaData
	{
		get
		{
			return m_chara_data;
		}
		set
		{
			m_chara_data = value;
		}
	}

	public ChaoData ChaoData
	{
		get
		{
			return m_chao_data;
		}
		set
		{
			m_chao_data = value;
		}
	}

	public ItemData ItemData
	{
		get
		{
			return m_item_data;
		}
		set
		{
			m_item_data = value;
		}
	}

	public OptionData OptionData
	{
		get
		{
			return m_option_data;
		}
		set
		{
			m_option_data = value;
		}
	}

	public ConnectData ConnectData
	{
		get
		{
			return m_connect_data;
		}
		set
		{
			m_connect_data = value;
		}
	}

	protected void Awake()
	{
		CheckInstance();
	}

	private void Start()
	{
		base.enabled = false;
	}

	private bool CheckInstance()
	{
		if (instance == null)
		{
			LoadSaveData();
			instance = this;
			Object.DontDestroyOnLoad(instance);
			return true;
		}
		if (this == Instance)
		{
			return true;
		}
		Object.Destroy(base.gameObject);
		return false;
	}

	public void SaveAllData()
	{
	}

	public void SavePlayerData()
	{
	}

	public void SaveCharaData()
	{
	}

	public void SaveChaoData()
	{
	}

	public void SaveItemData()
	{
	}

	public void SaveOptionData()
	{
	}

	public void LoadSaveData()
	{
	}
}
