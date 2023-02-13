using System.Linq;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class Lexer
	{
		private string text;

		private int pos;

		private int length;

		private int line;

		public static TokenList Tokenize(string text)
		{
			Lexer lexer = new Lexer();
			lexer.SetText(text);
			return lexer.ScanAll();
		}

		public void SetText(string text)
		{
			this.text = text + "    ";
			pos = 0;
			length = text.Length;
			line = 0;
		}

		public TokenList ScanAll()
		{
			TokenList tokenList = new TokenList();
			Token token;
			do
			{
				token = new Token();
				ScanOne(token);
				tokenList.Add(token);
			}
			while (token.type != 0);
			return tokenList;
		}

		private void UpdateNewlineStats(char ch)
		{
			if (ch == '\n')
			{
				line++;
			}
		}

		private void ScanOne(Token tok)
		{
			//Discarded unreachable code: IL_0105
			while (pos < length && char.IsWhiteSpace(text[pos]))
			{
				UpdateNewlineStats(text[pos]);
				pos++;
			}
			if (pos >= length)
			{
				tok.type = TokenType.EOF;
				return;
			}
			char c = text[pos];
			char c2 = text[pos + 1];
			switch (c)
			{
			case '"':
				ScanQuotedString(tok);
				return;
			case '/':
				if (c2 == '*')
				{
					ScanMultilineComment(tok);
					return;
				}
				break;
			}
			if (c == '/' && c2 == '/')
			{
				ScanComment(tok);
			}
			else if (IsOperator(c))
			{
				ScanOperator(tok);
			}
			else
			{
				ScanString(tok);
			}
		}

		private void ScanString(Token tok)
		{
			tok.type = TokenType.String;
			tok.begin = pos;
			while (pos < length)
			{
				char c = text[pos];
				char c2 = text[pos + 1];
				if (char.IsWhiteSpace(c) || c == '"' || (c == '/' && c2 == '*') || (c == '/' && c2 == '/') || IsOperator(c))
				{
					break;
				}
				pos++;
			}
			tok.end = pos;
			tok.line = line;
		}

		private void ScanQuotedString(Token tok)
		{
			tok.type = TokenType.QuotedString;
			tok.begin = pos;
			pos++;
			while (pos < length)
			{
				if (text[pos] == '\\' && text[pos + 1] == '"')
				{
					pos += 2;
					continue;
				}
				if (text[pos] == '"')
				{
					break;
				}
				UpdateNewlineStats(text[pos]);
				pos++;
			}
			pos++;
			tok.end = pos;
			tok.line = line;
		}

		private void ScanMultilineComment(Token tok)
		{
			tok.type = TokenType.Comment;
			tok.begin = pos;
			pos += 2;
			while (pos < length && (text[pos] != '*' || text[pos + 1] != '/'))
			{
				UpdateNewlineStats(text[pos]);
				pos++;
			}
			pos += 2;
			tok.end = pos;
			tok.line = line;
		}

		private void ScanComment(Token tok)
		{
			tok.type = TokenType.Comment;
			tok.begin = pos;
			pos += 2;
			while (pos < length && text[pos] != '\n')
			{
				pos++;
			}
			UpdateNewlineStats(text[pos]);
			pos++;
			tok.end = pos;
			tok.line = line;
		}

		private bool IsOperator(char ch)
		{
			if (";,=(){}".Contains(ch))
			{
				return true;
			}
			return false;
		}

		private void ScanOperator(Token tok)
		{
			switch (text[pos])
			{
			case ';':
				ScanOperatorSpecific(tok, TokenType.Semicolon);
				break;
			case ',':
				ScanOperatorSpecific(tok, TokenType.Comma);
				break;
			case '=':
				ScanOperatorSpecific(tok, TokenType.Eq);
				break;
			case '(':
				ScanOperatorSpecific(tok, TokenType.LParen);
				break;
			case ')':
				ScanOperatorSpecific(tok, TokenType.RParen);
				break;
			case '{':
				ScanOperatorSpecific(tok, TokenType.LBrace);
				break;
			case '}':
				ScanOperatorSpecific(tok, TokenType.RBrace);
				break;
			}
		}

		private void ScanOperatorSpecific(Token tok, TokenType type)
		{
			tok.type = type;
			tok.begin = pos;
			pos++;
			tok.end = pos;
			tok.line = line;
		}
	}
}
