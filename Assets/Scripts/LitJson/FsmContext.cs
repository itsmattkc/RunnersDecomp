namespace LitJson
{
	internal class FsmContext
	{
		public bool Return;

		public int NextState;

		public Lexer L;

		public int StateStack;
	}
}
