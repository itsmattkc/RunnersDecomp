using LitJson;
using System.Collections.Generic;

public class NetServerEquipItem : NetBase
{
	public List<ItemType> items
	{
		get;
		set;
	}

	public ServerPlayerState resultPlayerState
	{
		get;
		private set;
	}

	public NetServerEquipItem()
		: this(null)
	{
	}

	public NetServerEquipItem(List<ItemType> items)
	{
		this.items = items;
	}

	protected override void DoRequest()
	{
		SetAction("Item/equipItem");
		SetParameter_EquipItem();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_EquipItem()
	{
		if (items == null)
		{
			items = new List<ItemType>();
		}
		List<object> list = new List<object>();
		foreach (ItemType item in items)
		{
			int id = (int)new ServerItem(item).id;
			list.Add(id);
		}
		WriteActionParamArray("equipItemList", list);
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		resultPlayerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}
}
