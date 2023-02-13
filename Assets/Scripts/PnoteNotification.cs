using App.Utility;
using SaveData;

public class PnoteNotification
{
	public enum LaunchOption
	{
		None,
		SendEnergy,
		NoLogin
	}

	private static readonly string[] LaunchString = new string[3]
	{
		"None",
		"SendEnergy",
		"nologin"
	};

	public static void RequestRegister()
	{
		if (SystemSaveManager.Instance != null)
		{
			string gameID = SystemSaveManager.GetGameID();
			if (!string.IsNullOrEmpty(gameID) && !gameID.Equals("0"))
			{
				Binding.Instance.RegistPnote(gameID);
			}
		}
	}

	public static void RequestUnregister()
	{
		Binding.Instance.UnregistPnote();
	}

	public static void SendMessage(string message, string reciever, LaunchOption option)
	{
		if (SystemSaveManager.Instance != null)
		{
			string gameID = SystemSaveManager.GetGameID();
			if (!string.IsNullOrEmpty(gameID) && !gameID.Equals("0"))
			{
				string launchOption = LaunchString[(int)option];
				Binding.Instance.SendMessagePnote(message, gameID, reciever, launchOption);
			}
		}
	}

	public static bool CheckEnableGetNoLoginIncentive()
	{
		string pnoteLaunchString = Binding.Instance.GetPnoteLaunchString();
		if (string.IsNullOrEmpty(pnoteLaunchString))
		{
			return false;
		}
		pnoteLaunchString = pnoteLaunchString.ToLower();
		if (pnoteLaunchString.Contains(LaunchString[2]))
		{
			return true;
		}
		return false;
	}

	public static void RegistTagsPnote(Bitset32 tag_bit)
	{
		string gameID = SystemSaveManager.GetGameID();
		string empty = string.Empty;
		empty = ((!tag_bit.Test(0)) ? "0" : "1");
		for (int i = 1; i < 5; i++)
		{
			empty = ((!tag_bit.Test(i)) ? (empty + ",0") : (empty + ",1"));
		}
		Binding.Instance.RegistTagsPnote(empty, gameID);
	}
}
