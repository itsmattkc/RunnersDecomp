namespace LitJson
{
	internal class WriterContext
	{
		public int Count;

		public bool InArray;

		public bool InObject;

		public bool ExpectingValue;

		public int Padding;
	}
}
