using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NetServerCommitWheelSpin : NetBase
{
	public int paramCount;

	public ServerPlayerState resultPlayerState
	{
		get;
		private set;
	}

	public ServerCharacterState[] resultCharacterState
	{
		get;
		private set;
	}

	public List<ServerChaoState> resultChaoState
	{
		get;
		private set;
	}

	public ServerWheelOptions resultWheelOptions
	{
		get;
		private set;
	}

	public ServerSpinResultGeneral resultSpinResultGeneral
	{
		get;
		private set;
	}

	public NetServerCommitWheelSpin(int count)
	{
		paramCount = count;
	}

	protected override void DoRequest()
	{
		SetAction("Spin/commitWheelSpin");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string commitWheelSpinString = instance.GetCommitWheelSpinString(paramCount);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(commitWheelSpinString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
		GetResponse_CharacterState(jdata);
		GetResponse_ChaoState(jdata);
		GetResponse_WheelOptions(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
		resultPlayerState = ServerInterface.PlayerState;
		resultWheelOptions = ServerInterface.WheelOptions;
		resultPlayerState.RefreshFakeState();
		resultWheelOptions.RefreshFakeState();
		if (resultWheelOptions.m_spinCost > resultPlayerState.m_numRedRings)
		{
			base.resultStCd = ServerInterface.StatusCode.NotEnoughRedStarRings;
			return;
		}
		resultPlayerState.m_numRedRings -= resultWheelOptions.m_spinCost;
		resultWheelOptions.m_spinCost = resultWheelOptions.m_nextSpinCost;
		resultWheelOptions.m_nextSpinCost++;
		ServerWheelOptions.WheelItemType wheelItemType = (ServerWheelOptions.WheelItemType)resultWheelOptions.m_items[resultWheelOptions.m_itemWon];
		if (wheelItemType == ServerWheelOptions.WheelItemType.SpinAgain)
		{
			resultWheelOptions.m_nextSpinCost = resultWheelOptions.m_spinCost;
			resultWheelOptions.m_spinCost = 0;
		}
		DateTime now = DateTime.Now;
		if (resultWheelOptions.m_nextFreeSpin <= now)
		{
			TimeSpan timeSpan = new TimeSpan(1, 0, 0, 0);
			while (resultWheelOptions.m_nextFreeSpin <= now)
			{
				resultWheelOptions.m_nextFreeSpin += timeSpan;
			}
		}
		Array values = Enum.GetValues(typeof(ServerWheelOptions.WheelItemType));
		resultWheelOptions.m_items[resultWheelOptions.m_itemWon] = (int)values.GetValue(UnityEngine.Random.Range(0, values.Length - 1));
		resultWheelOptions.m_itemWon = UnityEngine.Random.Range(0, resultWheelOptions.m_items.Length);
	}

	private void SetParameter_WheelSpin()
	{
		WriteActionParamValue("count", paramCount);
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		resultPlayerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}

	private void GetResponse_CharacterState(JsonData jdata)
	{
		resultCharacterState = NetUtil.AnalyzePlayerState_CharactersStates(jdata);
	}

	private void GetResponse_ChaoState(JsonData jdata)
	{
		resultChaoState = NetUtil.AnalyzePlayerState_ChaoStates(jdata);
	}

	private void GetResponse_WheelOptions(JsonData jdata)
	{
		resultWheelOptions = NetUtil.AnalyzeWheelOptionsJson(jdata, "wheelOptions");
	}

	private void GetResponse_WheelResult(JsonData jdata)
	{
		resultSpinResultGeneral = NetUtil.AnalyzeSpinResultJson(jdata, "spinResultList");
	}
}
