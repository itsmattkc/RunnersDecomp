using System.Collections.Generic;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class TreeAST : ValueAST
	{
		public List<KeyValueAST> values = new List<KeyValueAST>();
	}
}
