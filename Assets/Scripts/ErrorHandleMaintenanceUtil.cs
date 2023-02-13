using DataTable;

public class ErrorHandleMaintenanceUtil
{
	public static string GetMaintenancePageURL()
	{
		return InformationDataTable.GetUrl(InformationDataTable.Type.MAINTENANCE_PAGE);
	}

	public static bool IsExistMaintenancePage()
	{
		string maintenancePageURL = GetMaintenancePageURL();
		if (string.IsNullOrEmpty(maintenancePageURL))
		{
			return false;
		}
		return true;
	}
}
