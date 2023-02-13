using System;
using System.Text;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class Serializer
	{
		private static string k_Indent = "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t";

		public static PBXElementDict ParseTreeAST(TreeAST ast, TokenList tokens, string text)
		{
			PBXElementDict pBXElementDict = new PBXElementDict();
			foreach (KeyValueAST value2 in ast.values)
			{
				PBXElementString pBXElementString = ParseIdentifierAST(value2.key, tokens, text);
				PBXElement value = ParseValueAST(value2.value, tokens, text);
				pBXElementDict[pBXElementString.value] = value;
			}
			return pBXElementDict;
		}

		public static PBXElementArray ParseArrayAST(ArrayAST ast, TokenList tokens, string text)
		{
			PBXElementArray pBXElementArray = new PBXElementArray();
			foreach (ValueAST value in ast.values)
			{
				pBXElementArray.values.Add(ParseValueAST(value, tokens, text));
			}
			return pBXElementArray;
		}

		public static PBXElement ParseValueAST(ValueAST ast, TokenList tokens, string text)
		{
			if (ast is TreeAST)
			{
				return ParseTreeAST((TreeAST)ast, tokens, text);
			}
			if (ast is ArrayAST)
			{
				return ParseArrayAST((ArrayAST)ast, tokens, text);
			}
			if (ast is IdentifierAST)
			{
				return ParseIdentifierAST((IdentifierAST)ast, tokens, text);
			}
			return null;
		}

		public static PBXElementString ParseIdentifierAST(IdentifierAST ast, TokenList tokens, string text)
		{
			Token token = tokens[ast.value];
			switch (token.type)
			{
			case TokenType.String:
			{
				string src = text.Substring(token.begin, token.end - token.begin);
				return new PBXElementString(src);
			}
			case TokenType.QuotedString:
			{
				string src = text.Substring(token.begin, token.end - token.begin);
				src = PBXStream.UnquoteString(src);
				return new PBXElementString(src);
			}
			default:
				throw new Exception("Internal parser error");
			}
		}

		private static string GetIndent(int indent)
		{
			return k_Indent.Substring(0, indent);
		}

		private static void WriteStringImpl(StringBuilder sb, string s, bool comment, GUIDToCommentMap comments)
		{
			if (comment)
			{
				sb.Append(comments.Write(s));
			}
			else
			{
				sb.Append(PBXStream.QuoteStringIfNeeded(s));
			}
		}

		public static void WriteDictKeyValue(StringBuilder sb, string key, PBXElement value, int indent, bool compact, PropertyCommentChecker checker, GUIDToCommentMap comments)
		{
			if (!compact)
			{
				sb.Append("\n");
				sb.Append(GetIndent(indent));
			}
			WriteStringImpl(sb, key, checker.CheckKeyInDict(key), comments);
			sb.Append(" = ");
			if (value is PBXElementString)
			{
				WriteStringImpl(sb, value.AsString(), checker.CheckStringValueInDict(key, value.AsString()), comments);
			}
			else if (value is PBXElementDict)
			{
				WriteDict(sb, value.AsDict(), indent, compact, checker.NextLevel(key), comments);
			}
			else if (value is PBXElementArray)
			{
				WriteArray(sb, value.AsArray(), indent, compact, checker.NextLevel(key), comments);
			}
			sb.Append(";");
			if (compact)
			{
				sb.Append(" ");
			}
		}

		public static void WriteDict(StringBuilder sb, PBXElementDict el, int indent, bool compact, PropertyCommentChecker checker, GUIDToCommentMap comments)
		{
			sb.Append("{");
			if (el.Contains("isa"))
			{
				WriteDictKeyValue(sb, "isa", el["isa"], indent + 1, compact, checker, comments);
			}
			foreach (string key in el.values.Keys)
			{
				if (key != "isa")
				{
					WriteDictKeyValue(sb, key, el[key], indent + 1, compact, checker, comments);
				}
			}
			if (!compact)
			{
				sb.Append("\n");
				sb.Append(GetIndent(indent));
			}
			sb.Append("}");
		}

		public static void WriteArray(StringBuilder sb, PBXElementArray el, int indent, bool compact, PropertyCommentChecker checker, GUIDToCommentMap comments)
		{
			sb.Append("(");
			foreach (PBXElement value in el.values)
			{
				if (!compact)
				{
					sb.Append("\n");
					sb.Append(GetIndent(indent + 1));
				}
				if (value is PBXElementString)
				{
					WriteStringImpl(sb, value.AsString(), checker.CheckStringValueInArray(value.AsString()), comments);
				}
				else if (value is PBXElementDict)
				{
					WriteDict(sb, value.AsDict(), indent + 1, compact, checker.NextLevel("*"), comments);
				}
				else if (value is PBXElementArray)
				{
					WriteArray(sb, value.AsArray(), indent + 1, compact, checker.NextLevel("*"), comments);
				}
				sb.Append(",");
				if (compact)
				{
					sb.Append(" ");
				}
			}
			if (!compact)
			{
				sb.Append("\n");
				sb.Append(GetIndent(indent));
			}
			sb.Append(")");
		}
	}
}
