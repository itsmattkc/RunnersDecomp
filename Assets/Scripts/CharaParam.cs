using SaveData;

public class CharaParam
{
	public SystemData.CharaTutorialFlagStatus m_flagStatus;

	public HudTutorial.Id m_tutorialID;

	public CharaParam(SystemData.CharaTutorialFlagStatus flagStatus, HudTutorial.Id tutorialID)
	{
		m_flagStatus = flagStatus;
		m_tutorialID = tutorialID;
	}
}
