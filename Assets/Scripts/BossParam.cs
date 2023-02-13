using SaveData;

public class BossParam
{
	public string m_name;

	public SystemData.FlagStatus m_flagStatus;

	public HudTutorial.Id m_tutorialID;

	public BossCharaType m_bossCharaType;

	public BossCategory m_bossCategory;

	public int m_layerNumber;

	public int m_indexNumber;

	public BossParam(string in_name, SystemData.FlagStatus flagStatus, HudTutorial.Id tutorialID, BossCharaType bossCharaType, BossCategory category, int number1, int number2)
	{
		m_name = in_name;
		m_flagStatus = flagStatus;
		m_tutorialID = tutorialID;
		m_bossCharaType = bossCharaType;
		m_bossCategory = category;
		m_layerNumber = number1;
		m_indexNumber = number2;
	}
}
