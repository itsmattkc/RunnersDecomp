using SaveData;

public class ItemParam
{
	public string m_name;

	public string m_objName;

	public SystemData.ItemTutorialFlagStatus m_flagStatus;

	public HudTutorial.Id m_tutorialID;

	public ItemParam(string name, string objName, SystemData.ItemTutorialFlagStatus flagStatus, HudTutorial.Id tutorialID)
	{
		m_name = name;
		m_objName = objName;
		m_flagStatus = flagStatus;
		m_tutorialID = tutorialID;
	}
}
