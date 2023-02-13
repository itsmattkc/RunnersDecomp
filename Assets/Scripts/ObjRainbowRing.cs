using GameScore;

public class ObjRainbowRing : ObjDashRing
{
	protected override string GetModelName()
	{
		return "obj_cmn_rainbowring";
	}

	protected override string GetEffectName()
	{
		return "ef_ob_pass_rainbowring01";
	}

	protected override string GetSEName()
	{
		return "obj_rainbowring";
	}

	protected override int GetScore()
	{
		return Data.RainbowRing;
	}

	public void SetObjRainbowRingParameter(ObjRainbowRingParameter param)
	{
		ObjDashRingParameter objDashRingParameter = new ObjDashRingParameter();
		objDashRingParameter.firstSpeed = param.firstSpeed;
		objDashRingParameter.outOfcontrol = param.outOfcontrol;
		SetObjDashRingParameter(objDashRingParameter);
	}
}
