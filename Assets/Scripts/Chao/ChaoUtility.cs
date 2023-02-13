using UnityEngine;

namespace Chao
{
	public class ChaoUtility
	{
		public static ChaoType GetChaoType(GameObject obj)
		{
			if (obj != null)
			{
				if (obj.name == "MainChao")
				{
					return ChaoType.MAIN;
				}
				if (obj.name == "SubChao")
				{
					return ChaoType.SUB;
				}
			}
			return ChaoType.MAIN;
		}

		public static ShaderType GetChaoShaderType(GameObject parentObj, ChaoType type)
		{
			if (parentObj != null)
			{
				string name = (type != 0) ? "SubChao" : "MainChao";
				ChaoState chaoState = GameObjectUtil.FindChildGameObjectComponent<ChaoState>(parentObj, name);
				if (chaoState != null)
				{
					return chaoState.ShaderOffset;
				}
			}
			return ShaderType.NORMAL;
		}
	}
}
