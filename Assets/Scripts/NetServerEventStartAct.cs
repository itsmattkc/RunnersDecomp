using LitJson;
using System.Collections.Generic;

public class NetServerEventStartAct : NetBase
{
	public int paramEventId
	{
		get;
		set;
	}

	public int paramEnergyExpend
	{
		get;
		set;
	}

	public long paramRaidBossId
	{
		get;
		set;
	}

	public List<ItemType> paramModifiersItem
	{
		get;
		set;
	}

	public List<BoostItemType> paramModifiersBoostItem
	{
		get;
		set;
	}

	public ServerPlayerState resultPlayerState
	{
		get;
		private set;
	}

	public ServerEventUserRaidBossState userRaidBossState
	{
		get;
		private set;
	}

	public NetServerEventStartAct()
		: this(-1, -1, -1L, null, null)
	{
	}

	public NetServerEventStartAct(int eventId, int energyExpend, long raidBossId, List<ItemType> modifiersItem, List<BoostItemType> modifiersBoostItem)
	{
		paramEventId = eventId;
		paramEnergyExpend = energyExpend;
		paramRaidBossId = raidBossId;
		paramModifiersItem = new List<ItemType>();
		if (modifiersItem != null)
		{
			for (int i = 0; i < modifiersItem.Count; i++)
			{
				paramModifiersItem.Add(modifiersItem[i]);
			}
		}
		paramModifiersBoostItem = new List<BoostItemType>();
		if (modifiersBoostItem != null)
		{
			for (int j = 0; j < modifiersBoostItem.Count; j++)
			{
				paramModifiersBoostItem.Add(modifiersBoostItem[j]);
			}
		}
	}

	protected override void DoRequest()
	{
		SetAction("Event/eventActStart");
		SetParameter_EventId();
		SetParameter_EnergyExpend();
		SetParameter_RaidBossId();
		SetParameter_Modifiers();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		resultPlayerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
		NetUtil.GetResponse_CampaignList(jdata);
		userRaidBossState = NetUtil.AnalyzeEventUserRaidBossState(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_EventId()
	{
		WriteActionParamValue("eventId", paramEventId);
	}

	private void SetParameter_EnergyExpend()
	{
		WriteActionParamValue("energyExpend", paramEnergyExpend);
	}

	private void SetParameter_RaidBossId()
	{
		WriteActionParamValue("raidbossId", paramRaidBossId);
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
}
