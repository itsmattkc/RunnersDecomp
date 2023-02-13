namespace Player
{
	public class PhantomLaserUtil
	{
		public const string EffectName = "ef_ph_laser_lp01";

		public const string SEName = "phantom_laser_shoot";

		public static void ChangeVisualOnEnter(CharacterState context)
		{
			StateUtil.EnableChildObject(context, context.BodyModelName, false);
			StateUtil.EnableChildObject(context, CharacterDefs.PhantomBodyName[0], true);
			StateUtil.EnablePlayerCollision(context, false);
			StateUtil.SetActiveEffect(context, "ef_ph_laser_lp01", true);
			StateUtil.SetShadowActive(context, false);
		}

		public static void ChangeVisualOnLeave(CharacterState context)
		{
			StateUtil.SetShadowActive(context, true);
			StateUtil.EnableChildObject(context, context.BodyModelName, true);
			StateUtil.EnableChildObject(context, CharacterDefs.PhantomBodyName[0], false);
			StateUtil.EnablePlayerCollision(context, true);
			StateUtil.SetActiveEffect(context, "ef_ph_laser_lp01", false);
		}
	}
}
