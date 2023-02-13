public class CharaSEUtil
{
	public class CharaSEData
	{
		public string m_jump;

		public string m_jump2;

		public string m_spin;

		public string m_fly;

		public string m_attack;

		public CharaSEData(string jump, string jump2, string spin, string fly, string attack)
		{
			m_jump = jump;
			m_jump2 = jump2;
			m_spin = spin;
			m_fly = fly;
			m_attack = attack;
		}
	}

	private static readonly CharaSEData[] CHARA_SE_TBL = new CharaSEData[29]
	{
		new CharaSEData("act_jump", "act_2ndjump", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump", "act_2ndjump", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump", "act_2ndjump", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump", "act_2ndjump_2", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump", "act_2ndjump", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump", "act_2ndjump_2", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump_large", "act_2ndjump", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump_large", "act_2ndjump", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump_large", "act_2ndjump", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump", "act_2ndjump", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump", "act_2ndjump", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump", "act_2ndjump", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump_large", "act_2ndjump", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump", "act_2ndjump", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump", "act_2ndjump", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump", "act_2ndjump_2", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump_cla", "act_jump_cla", "act_spindash_cla", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump", "act_2ndjump", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump", "act_2ndjump_2", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump", "act_2ndjump", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump", "act_2ndjump", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump_large", "act_2ndjump", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump", "act_2ndjump", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump", "act_2ndjump", "act_spindash", "act_flight_silver", "act_powertype_attack"),
		new CharaSEData("act_jump", "act_2ndjump", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump", "act_2ndjump", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump", "act_2ndjump", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump", "act_2ndjump", "act_spindash", "act_flytype_fly", "act_powertype_attack"),
		new CharaSEData("act_jump", "act_2ndjump_2", "act_spindash", "act_flytype_fly", "act_powertype_attack")
	};

	private static bool EnableChara(CharaType charaType)
	{
		if (CharaType.SONIC <= charaType && charaType < CharaType.NUM)
		{
			return true;
		}
		return false;
	}

	public static void PlayJumpSE(CharaType charaType)
	{
		if (EnableChara(charaType))
		{
			SoundManager.SePlay(CHARA_SE_TBL[(int)charaType].m_jump);
		}
		else
		{
			SoundManager.SePlay("act_jump");
		}
	}

	public static void Play2ndJumpSE(CharaType charaType)
	{
		if (EnableChara(charaType))
		{
			SoundManager.SePlay(CHARA_SE_TBL[(int)charaType].m_jump2);
		}
		else
		{
			SoundManager.SePlay("act_2ndjump");
		}
	}

	public static string GetSpinDashSEName(CharaType charaType)
	{
		if (EnableChara(charaType))
		{
			return CHARA_SE_TBL[(int)charaType].m_spin;
		}
		return "act_spindash";
	}

	public static void PlaySpinDashSE(CharaType charaType)
	{
		if (EnableChara(charaType))
		{
			SoundManager.SePlay(CHARA_SE_TBL[(int)charaType].m_spin);
		}
		else
		{
			SoundManager.SePlay("act_spindash");
		}
	}

	public static void PlayFlySE(CharaType charaType)
	{
		if (EnableChara(charaType))
		{
			SoundManager.SePlay(CHARA_SE_TBL[(int)charaType].m_fly);
		}
		else
		{
			SoundManager.SePlay("act_flytype_fly");
		}
	}

	public static void PlayPowerAttackSE(CharaType charaType)
	{
		if (EnableChara(charaType))
		{
			SoundManager.SePlay(CHARA_SE_TBL[(int)charaType].m_attack);
		}
		else
		{
			SoundManager.SePlay("act_powertype_attack");
		}
	}
}
