using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class MileageMapDataManager : MonoBehaviour
{
	private Dictionary<string, MileageMapData> m_mileage_datas;

	private string m_current_key;

	private int m_mileageStageIndex = 1;

	private List<string> m_loadingFaceList = new List<string>();

	private List<string> m_keepList = new List<string>();

	private List<ServerMileageReward> m_rewardList = new List<ServerMileageReward>();

	private static MileageMapDataManager instance;

	public static MileageMapDataManager Instance
	{
		get
		{
			return instance;
		}
	}

	public int MileageStageIndex
	{
		get
		{
			return m_mileageStageIndex;
		}
		set
		{
			m_mileageStageIndex = value;
		}
	}

	protected void Awake()
	{
		SetInstance();
	}

	private void Start()
	{
		if (m_mileage_datas == null)
		{
			m_mileage_datas = new Dictionary<string, MileageMapData>();
		}
		base.enabled = false;
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	private void SetInstance()
	{
		if (instance == null)
		{
			Object.DontDestroyOnLoad(base.gameObject);
			instance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void SetData(TextAsset xml_data)
	{
		string s = AESCrypt.Decrypt(xml_data.text);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(MileageMapData[]));
		StringReader textReader = new StringReader(s);
		MileageMapData[] array = (MileageMapData[])xmlSerializer.Deserialize(textReader);
		if (array[0] == null)
		{
			return;
		}
		int episode = array[0].scenario.episode;
		int chapter = array[0].scenario.chapter;
		string key = GetKey(episode, chapter);
		if (!IsExist(key))
		{
			if (m_current_key == null)
			{
				m_current_key = key;
			}
			m_mileage_datas.Add(key, array[0]);
		}
	}

	public void SetCurrentData(int episode, int chapter)
	{
		string key = GetKey(episode, chapter);
		if (IsExist(key))
		{
			m_current_key = key;
		}
	}

	public void DestroyData()
	{
		m_mileage_datas.Clear();
		DestroyFaceAndBGData();
		m_current_key = null;
	}

	public void DestroyFaceAndBGData(bool keepFlag = false)
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "MileageMapFace");
		List<GameObject> list = new List<GameObject>();
		if (gameObject != null)
		{
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				GameObject gameObject2 = gameObject.transform.GetChild(i).gameObject;
				if (m_loadingFaceList.Contains(gameObject2.name))
				{
					continue;
				}
				if (keepFlag)
				{
					if (!m_keepList.Contains(gameObject2.name))
					{
						list.Add(gameObject2);
					}
				}
				else
				{
					list.Add(gameObject2);
				}
			}
		}
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(base.gameObject, "MileageMapBG");
		if (gameObject3 != null)
		{
			for (int j = 0; j < gameObject3.transform.childCount; j++)
			{
				GameObject gameObject4 = gameObject3.transform.GetChild(j).gameObject;
				if (keepFlag)
				{
					if (!m_keepList.Contains(gameObject4.name))
					{
						list.Add(gameObject4);
					}
				}
				else
				{
					list.Add(gameObject4);
				}
			}
		}
		foreach (GameObject item in list)
		{
			Object.Destroy(item);
		}
		if (!keepFlag)
		{
			m_keepList.Clear();
		}
	}

	public MileageMapData GetMileageMapData()
	{
		if (IsExist(m_current_key))
		{
			return m_mileage_datas[m_current_key];
		}
		return null;
	}

	public ServerMileageReward GetMileageReward(int episode, int chapter, int point)
	{
		foreach (ServerMileageReward reward in m_rewardList)
		{
			if (reward.m_episode == episode && reward.m_chapter == chapter && reward.m_point == point)
			{
				return reward;
			}
		}
		return null;
	}

	public MileageMapData GetMileageMapData(int episode, int chapter)
	{
		string key = GetKey(episode, chapter);
		if (IsExist(key))
		{
			return m_mileage_datas[key];
		}
		return null;
	}

	public int GetRouteId(int episode, int chapter, int point)
	{
		string key = GetKey(episode, chapter);
		if (IsExist(key) && point < 5)
		{
			return m_mileage_datas[key].map_data.route_data[point].id;
		}
		return -1;
	}

	public void SetPointIncentiveData(int episode, int chapter, int point, RewardData src_reward)
	{
		string key = GetKey(episode, chapter);
		if (IsExist(key))
		{
			int num = m_mileage_datas[key].event_data.point.Length;
			if (point < num)
			{
				m_mileage_datas[key].event_data.point[point].reward.Set(src_reward);
			}
		}
	}

	public void SetChapterIncentiveData(int episode, int chapter, int index, RewardData src_reward)
	{
		string key = GetKey(episode, chapter);
		if (IsExist(key) && m_mileage_datas[key].map_data.reward != null)
		{
			int num = m_mileage_datas[key].map_data.reward.Length;
			if (index < num)
			{
				m_mileage_datas[key].map_data.reward[index].Set(src_reward);
			}
		}
	}

	public void SetEpisodeIncentiveData(int episode, int chapter, int index, RewardData src_reward)
	{
		string key = GetKey(episode, chapter);
		if (IsExist(key) && m_mileage_datas[key].scenario.reward != null)
		{
			int num = m_mileage_datas[key].scenario.reward.Length;
			if (index < num)
			{
				m_mileage_datas[key].scenario.reward[index].Set(src_reward);
			}
		}
	}

	public void SetRewardData(List<ServerMileageReward> rewardList)
	{
		m_rewardList.Clear();
		if (rewardList == null)
		{
			return;
		}
		foreach (ServerMileageReward reward in rewardList)
		{
			m_rewardList.Add(reward);
		}
	}

	public bool IsExist(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return false;
		}
		if (m_mileage_datas != null)
		{
			return m_mileage_datas.ContainsKey(key);
		}
		return false;
	}

	public bool IsExist(int episode, int chapter)
	{
		string key = GetKey(episode, chapter);
		return IsExist(key);
	}

	private string GetKey(int episode, int chapter)
	{
		return episode.ToString("000") + chapter.ToString("00");
	}

	public void AddKeepList(string name)
	{
		foreach (string keep in m_keepList)
		{
			if (name == keep)
			{
				return;
			}
		}
		m_keepList.Add(name);
	}

	public void SetLoadingFaceId(List<int> loadingList)
	{
		m_loadingFaceList.Clear();
		foreach (int loading in loadingList)
		{
			m_loadingFaceList.Add(MileageMapUtility.GetFaceTextureName(loading));
		}
	}
}
