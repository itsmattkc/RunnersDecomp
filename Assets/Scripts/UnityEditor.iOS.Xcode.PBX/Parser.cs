using System;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class Parser
	{
		private TokenList tokens;

		private int currPos;

		public Parser(TokenList tokens)
		{
			this.tokens = tokens;
			currPos = SkipComments(0);
		}

		private int SkipComments(int pos)
		{
			while (pos < tokens.Count && tokens[pos].type == TokenType.Comment)
			{
				pos++;
			}
			return pos;
		}

		private int IncInternal(int pos)
		{
			if (pos >= tokens.Count)
			{
				return pos;
			}
			pos++;
			return SkipComments(pos);
		}

		private int Inc()
		{
			int result = currPos;
			currPos = IncInternal(currPos);
			return result;
		}

		private TokenType Tok()
		{
			if (currPos >= tokens.Count)
			{
				return TokenType.EOF;
			}
			return tokens[currPos].type;
		}

		private void SkipIf(TokenType type)
		{
			if (Tok() == type)
			{
				Inc();
			}
		}

		private string GetErrorMsg()
		{
			return "Invalid PBX project (parsing line " + tokens[currPos].line + ")";
		}

		public IdentifierAST ParseIdentifier()
		{
			if (Tok() != TokenType.String && Tok() != TokenType.QuotedString)
			{
				throw new Exception(GetErrorMsg());
			}
			IdentifierAST identifierAST = new IdentifierAST();
			identifierAST.value = Inc();
			return identifierAST;
		}

		public TreeAST ParseTree()
		{
			if (Tok() != TokenType.LBrace)
			{
				throw new Exception(GetErrorMsg());
			}
			Inc();
			TreeAST treeAST = new TreeAST();
			while (Tok() != TokenType.RBrace && Tok() != 0)
			{
				treeAST.values.Add(ParseKeyValue());
			}
			SkipIf(TokenType.RBrace);
			return treeAST;
		}

		public ArrayAST ParseList()
		{
			if (Tok() != TokenType.LParen)
			{
				throw new Exception(GetErrorMsg());
			}
			Inc();
			ArrayAST arrayAST = new ArrayAST();
			while (Tok() != TokenType.RParen && Tok() != 0)
			{
				arrayAST.values.Add(ParseValue());
				SkipIf(TokenType.Comma);
			}
			SkipIf(TokenType.RParen);
			return arrayAST;
		}

		public KeyValueAST ParseKeyValue()
		{
			KeyValueAST keyValueAST = new KeyValueAST();
			keyValueAST.key = ParseIdentifier();
			if (Tok() != TokenType.Eq)
			{
				throw new Exception(GetErrorMsg());
			}
			Inc();
			keyValueAST.value = ParseValue();
			SkipIf(TokenType.Semicolon);
			return keyValueAST;
		}

		public ValueAST ParseValue()
		{
			if (Tok() == TokenType.String || Tok() == TokenType.QuotedString)
			{
				return ParseIdentifier();
			}
			if (Tok() == TokenType.LBrace)
			{
				return ParseTree();
			}
			if (Tok() == TokenType.LParen)
			{
				return ParseList();
			}
			throw new Exception(GetErrorMsg());
		}
	}
}
