using UnityEngine;

[AddComponentMenu("Scripts/Runners/Object/Common/ObjSpringAir")]
public class ObjSpringAir : ObjSpring
{
	private const string ModelName = "obj_cmn_springAir";

	protected override string GetModelName()
	{
		return "obj_cmn_springAir";
	}

	protected override string GetMotionName()
	{
		return "obj_springAir_bounce";
	}

	public void SetObjSpringAirParameter(ObjSpringAirParameter param)
	{
		ObjSpringParameter objSpringParameter = new ObjSpringParameter();
		objSpringParameter.firstSpeed = param.firstSpeed;
		objSpringParameter.outOfcontrol = param.outOfcontrol;
		SetObjSpringParameter(objSpringParameter);
	}
}
