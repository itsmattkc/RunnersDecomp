using UnityEngine;

namespace Player
{
	public class PhantomAsteroidUtil
	{
		public const string EffectName = "ef_ph_aste_lp01";

		public const string SEName = "phantom_asteroid";

		public static GameObject ChangeVisualOnEnter(CharacterState context)
		{
			string name = CharacterDefs.PhantomBodyName[2];
			StateUtil.EnableChildObject(context, context.BodyModelName, false);
			StateUtil.EnableChildObject(context, name, true);
			StateUtil.EnablePlayerCollision(context, false);
			GameObject result = null;
			GameObject gameObject = GameObjectUtil.FindChildGameObject(context.gameObject, name);
			if (gameObject != null)
			{
				result = StateUtil.CreateEffectOnTransform(context, gameObject.transform, "ef_ph_aste_lp01", false);
			}
			SoundManager.SePlay("phantom_asteroid");
			return result;
		}

		public static void ChangeVisualOnLeave(CharacterState context, GameObject effect)
		{
			StateUtil.EnableChildObject(context, context.BodyModelName, true);
			StateUtil.EnableChildObject(context, CharacterDefs.PhantomBodyName[2], false);
			StateUtil.EnablePlayerCollision(context, true);
			if (effect != null)
			{
				Object.Destroy(effect);
			}
			SoundManager.SeStop("phantom_asteroid");
		}

		public static GameObject GetModelObject(CharacterState context)
		{
			return GameObjectUtil.FindChildGameObject(context.gameObject, CharacterDefs.PhantomBodyName[2]);
		}
	}
}
