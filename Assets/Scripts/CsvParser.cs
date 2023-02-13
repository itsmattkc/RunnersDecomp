using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CsvParser
{
	public class CsvFields
	{
		public List<string> field = new List<string>();

		public List<string> FieldList
		{
			get
			{
				return field;
			}
		}
	}

	public static List<CsvFields> ParseCsvFromText(string i_text)
	{
		return CsvToArrayList(i_text);
	}

	private static List<CsvFields> CsvToArrayList(string csvText)
	{
		List<CsvFields> list = new List<CsvFields>();
		csvText = csvText.Trim('\r', '\n');
		Regex regex = new Regex("^.*(?:\\n|$)", RegexOptions.Multiline);
		Regex regex2 = new Regex("\\s*(\"(?:[^\"]|\"\")*\"|[^,]*)\\s*,", RegexOptions.None);
		Match match = regex.Match(csvText);
		string empty = string.Empty;
		while (match.Success)
		{
			empty = match.Value;
			while (CountString(empty, "\"") % 2 == 1)
			{
				match = match.NextMatch();
				if (!match.Success)
				{
				}
				empty += match.Value;
			}
			empty = empty.TrimEnd('\r', '\n');
			empty += ",";
			CsvFields csvFields = new CsvFields();
			Match match2 = regex2.Match(empty);
			while (match2.Success)
			{
				string value = match2.Groups[1].Value;
				value = value.Trim();
				if (value.StartsWith("\"") && value.EndsWith("\""))
				{
					value = value.Substring(1, value.Length - 2);
					value = value.Replace("\"\"", "\"");
				}
				match2 = match2.NextMatch();
				if (value.IndexOf('#') == 0)
				{
					break;
				}
				csvFields.field.Add(value);
			}
			if (csvFields.field.Count != 0)
			{
				list.Add(csvFields);
			}
			match = match.NextMatch();
		}
		return list;
	}

	private static int CountString(string strInput, string strFind)
	{
		int num = 0;
		for (int num2 = strInput.IndexOf(strFind); num2 > -1; num2 = strInput.IndexOf(strFind, num2 + 1))
		{
			num++;
		}
		return num;
	}
}
