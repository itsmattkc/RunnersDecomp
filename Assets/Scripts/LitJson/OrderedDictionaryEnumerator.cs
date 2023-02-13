using System.Collections;
using System.Collections.Generic;

namespace LitJson
{
	internal class OrderedDictionaryEnumerator : IEnumerator, IDictionaryEnumerator
	{
		private IEnumerator<KeyValuePair<string, JsonData>> list_enumerator;

		public object Current
		{
			get
			{
				return Entry;
			}
		}

		public DictionaryEntry Entry
		{
			get
			{
				KeyValuePair<string, JsonData> current = list_enumerator.Current;
				return new DictionaryEntry(current.Key, current.Value);
			}
		}

		public object Key
		{
			get
			{
				return list_enumerator.Current.Key;
			}
		}

		public object Value
		{
			get
			{
				return list_enumerator.Current.Value;
			}
		}

		public OrderedDictionaryEnumerator(IEnumerator<KeyValuePair<string, JsonData>> enumerator)
		{
			list_enumerator = enumerator;
		}

		public bool MoveNext()
		{
			return list_enumerator.MoveNext();
		}

		public void Reset()
		{
			list_enumerator.Reset();
		}
	}
}
