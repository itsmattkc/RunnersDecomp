using LitJson;
using System.Collections.Generic;

public class NetServerQuickModeStartAct : NetBase
{
	private ServerPlayerState m_resultPlayerState;

	private List<ItemType> m_paramModifiersItem = new List<ItemType>();

	private List<BoostItemType> m_paramModifiersBoostItem = new List<BoostItemType>();

	private int m_tutorial;

	public ServerPlayerState resultPlayerState
	{
		get
		{
			return m_resultPlayerState;
		}
	}

	public List<ItemType> paramModifiersItem
	{
		get
		{
			return m_paramModifiersItem;
		}
	}

	public List<BoostItemType> paramModifiersBoostItem
	{
		get
		{
			return m_paramModifiersBoostItem;
		}
	}

	public NetServerQuickModeStartAct(List<ItemType> modifiersItem, List<BoostItemType> modifiersBoostItem, bool tutorial)
	{
		if (modifiersItem != null)
		{
			for (int i = 0; i < modifiersItem.Count; i++)
			{
				m_paramModifiersItem.Add(modifiersItem[i]);
			}
		}
		if (modifiersBoostItem != null)
		{
			for (int j = 0; j < modifiersBoostItem.Count; j++)
			{
				m_paramModifiersBoostItem.Add(modifiersBoostItem[j]);
			}
		}
		if (tutorial)
		{
			m_tutorial = 1;
		}
		else
		{
			m_tutorial = 0;
		}
	}

	protected override void DoRequest()
	{
		SetAction("Game/quickActStart");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < paramModifiersItem.Count; i++)
			{
				ServerItem.Id id = new ServerItem(paramModifiersItem[i]).id;
				list.Add((int)id);
			}
			for (int j = 0; j < paramModifiersBoostItem.Count; j++)
			{
				ServerItem.Id id2 = new ServerItem(paramModifiersBoostItem[j]).id;
				list.Add((int)id2);
			}
			string quickModeActStartString = instance.GetQuickModeActStartString(list, m_tutorial);
			Debug.Log("NetServerQuickModeStartAct.json = " + quickModeActStartString);
			WriteJsonString(quickModeActStartString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		m_resultPlayerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
		NetUtil.GetResponse_CampaignList(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_Modifiers()
	{
		List<object> list = new List<object>();
		for (int i = 0; i < paramModifiersItem.Count; i++)
		{
			ServerItem.Id id = new ServerItem(paramModifiersItem[i]).id;
			list.Add((int)id);
		}
		for (int j = 0; j < paramModifiersBoostItem.Count; j++)
		{
			ServerItem.Id id2 = new ServerItem(paramModifiersBoostItem[j]).id;
			list.Add((int)id2);
		}
		WriteActionParamArray("modifire", list);
		list.Clear();
		list = null;
	}

	private void SetParameter_Tutorial()
	{
		WriteActionParamValue("tutorial", m_tutorial);
	}
}
