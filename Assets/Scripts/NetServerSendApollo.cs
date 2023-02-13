using LitJson;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class NetServerSendApollo : NetBase
{
	public int type
	{
		get;
		set;
	}

	public string[] value
	{
		get;
		set;
	}

	public NetServerSendApollo()
		: this(-1, null)
	{
	}

	public NetServerSendApollo(int type, string[] value)
	{
		this.type = type;
		this.value = value;
	}

	protected override void DoRequest()
	{
		SetAction("Sgn/sendApollo");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (!(instance != null))
		{
			return;
		}
		List<string> list = new List<string>();
		if (this.value != null)
		{
			string[] value = this.value;
			foreach (string item in value)
			{
				list.Add(item);
			}
		}
		string sendApolloString = instance.GetSendApolloString(type, list);
		Debug.Log("CPlusPlusLink.actRetry");
		WriteJsonString(sendApolloString);
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_Data()
	{
		WriteActionParamValue("type", type);
		if (value != null && value.Length != 0)
		{
			WriteActionParamArray("value", new List<object>(value));
		}
	}
}
