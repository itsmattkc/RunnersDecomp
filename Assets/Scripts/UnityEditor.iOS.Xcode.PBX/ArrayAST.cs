using System.Collections.Generic;

namespace UnityEditor.iOS.Xcode.PBX
{
	internal class ArrayAST : ValueAST
	{
		public List<ValueAST> values = new List<ValueAST>();
	}
}
