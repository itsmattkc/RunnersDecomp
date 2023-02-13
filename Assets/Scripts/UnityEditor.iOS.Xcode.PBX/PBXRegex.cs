using System.Text.RegularExpressions;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class PBXRegex
	{
		public static string GuidRegexString = "[A-Fa-f0-9]{24}";

		public static Regex DontNeedQuotes = new Regex("^[\\w\\d\\./_*]+$");
	}
}
