namespace LitJson
{
	internal enum ParserToken
	{
		None = 0x10000,
		Number,
		True,
		False,
		Null,
		CharSeq,
		Char,
		Text,
		Object,
		ObjectPrime,
		Pair,
		PairRest,
		Array,
		ArrayPrime,
		Value,
		ValueRest,
		String,
		End,
		Epsilon
	}
}
