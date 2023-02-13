namespace UnityEditor.iOS.Xcode.PBX
{
	internal class PBXStream
	{
		public static string QuoteStringIfNeeded(string src)
		{
			if (PBXRegex.DontNeedQuotes.IsMatch(src) && !src.Contains("//") && !src.Contains("/*") && !src.Contains("*/"))
			{
				return src;
			}
			return "\"" + src.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n") + "\"";
		}

		public static string UnquoteString(string src)
		{
			if (!src.StartsWith("\"") || !src.EndsWith("\""))
			{
				return src;
			}
			return src.Substring(1, src.Length - 2).Replace("\\\\", "嚟").Replace("\\\"", "\"")
				.Replace("\\n", "\n")
				.Replace("嚟", "\\");
		}
	}
}
