namespace UnityEditor.iOS.Xcode.PBX
{
	internal enum TokenType
	{
		EOF,
		Invalid,
		String,
		QuotedString,
		Comment,
		Semicolon,
		Comma,
		Eq,
		LParen,
		RParen,
		LBrace,
		RBrace
	}
}
