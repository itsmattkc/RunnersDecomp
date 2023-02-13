using System;
using System.Collections;
using System.Globalization;
using System.Text;

public static class NewJSON
{
	private static bool IsWhitespace(char c)
	{
		if (c == ' ' || c == '\b' || c == '\f' || c == '\n' || c == '\r' || c == '\t')
		{
			return true;
		}
		return false;
	}

	private static bool AddHexToInt(char c, ref int n, int s)
	{
		switch (c)
		{
		case '0':
			n |= 0 << (s & 0x1F);
			return true;
		case '1':
			n |= 1 << (s & 0x1F);
			return true;
		case '2':
			n |= 2 << (s & 0x1F);
			return true;
		case '3':
			n |= 3 << (s & 0x1F);
			return true;
		case '4':
			n |= 4 << (s & 0x1F);
			return true;
		case '5':
			n |= 5 << (s & 0x1F);
			return true;
		case '6':
			n |= 6 << (s & 0x1F);
			return true;
		case '7':
			n |= 7 << (s & 0x1F);
			return true;
		case '8':
			n |= 8 << (s & 0x1F);
			return true;
		case '9':
			n |= 9 << (s & 0x1F);
			return true;
		case 'a':
			n |= 10 << (s & 0x1F);
			return true;
		case 'A':
			n |= 10 << (s & 0x1F);
			return true;
		case 'b':
			n |= 11 << (s & 0x1F);
			return true;
		case 'B':
			n |= 11 << (s & 0x1F);
			return true;
		case 'c':
			n |= 12 << (s & 0x1F);
			return true;
		case 'C':
			n |= 12 << (s & 0x1F);
			return true;
		case 'd':
			n |= 13 << (s & 0x1F);
			return true;
		case 'D':
			n |= 13 << (s & 0x1F);
			return true;
		case 'e':
			n |= 14 << (s & 0x1F);
			return true;
		case 'E':
			n |= 14 << (s & 0x1F);
			return true;
		case 'f':
			n |= 15 << (s & 0x1F);
			return true;
		case 'F':
			n |= 15 << (s & 0x1F);
			return true;
		default:
			return false;
		}
	}

	private static object DecodeString(char[] buffer, ref int p, int length)
	{
		int num = 0;
		int n = 0;
		StringBuilder stringBuilder = new StringBuilder();
		while (p < length)
		{
			char c = buffer[p++];
			switch (num)
			{
			case 0:
				switch (c)
				{
				case '"':
					return stringBuilder.ToString();
				case '\\':
					num = 1;
					break;
				default:
					stringBuilder.Append(c);
					break;
				}
				break;
			case 1:
				switch (c)
				{
				case '"':
				case '/':
				case '\\':
					stringBuilder.Append(c);
					num = 0;
					break;
				case 'b':
					stringBuilder.Append('\b');
					num = 0;
					break;
				case 'f':
					stringBuilder.Append('\f');
					num = 0;
					break;
				case 'n':
					stringBuilder.Append('\n');
					num = 0;
					break;
				case 'r':
					stringBuilder.Append('\r');
					num = 0;
					break;
				case 't':
					stringBuilder.Append('\t');
					num = 0;
					break;
				case 'u':
					n = 0;
					num = 10;
					break;
				}
				break;
			case 10:
				if (!AddHexToInt(c, ref n, 12))
				{
					return false;
				}
				num = 11;
				break;
			case 11:
				if (!AddHexToInt(c, ref n, 8))
				{
					return false;
				}
				num = 12;
				break;
			case 12:
				if (!AddHexToInt(c, ref n, 4))
				{
					return false;
				}
				num = 13;
				break;
			case 13:
				if (!AddHexToInt(c, ref n, 0))
				{
					return false;
				}
				stringBuilder.Append((char)n);
				num = 0;
				break;
			default:
				return null;
			}
		}
		return null;
	}

	private static object DecodeNumber(char[] buffer, ref int p, int length)
	{
		int num = 0;
		bool flag = false;
		StringBuilder stringBuilder = new StringBuilder();
		while (true)
		{
			if (p < length)
			{
				char c = buffer[p++];
				stringBuilder.Append(c);
				switch (num)
				{
				case 0:
					switch (c)
					{
					case '0':
						num = 2;
						continue;
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						num = 4;
						continue;
					}
					if (c == '-')
					{
						num = 1;
						continue;
					}
					return null;
				case 1:
					switch (c)
					{
					case '0':
						num = 2;
						break;
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						num = 4;
						break;
					default:
						return null;
					}
					continue;
				case 2:
					if (c == '.')
					{
						num = 8;
						continue;
					}
					break;
				case 3:
					if (c >= '0' && c <= '9')
					{
						num = 3;
						continue;
					}
					if (c == 'e' || c == 'E')
					{
						num = 5;
						continue;
					}
					break;
				case 4:
					switch (c)
					{
					case '.':
						num = 8;
						continue;
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						num = 4;
						continue;
					}
					if (c == 'e' || c == 'E')
					{
						num = 5;
						continue;
					}
					break;
				case 5:
					if (c >= '0' && c <= '9')
					{
						num = 7;
						continue;
					}
					if (c == '+' || c == '-')
					{
						num = 6;
						continue;
					}
					return null;
				case 6:
					if (c >= '0' && c <= '9')
					{
						num = 7;
						continue;
					}
					return null;
				case 7:
					if (c >= '0' && c <= '9')
					{
						num = 7;
						continue;
					}
					break;
				case 8:
					if (c >= '0' && c <= '9')
					{
						num = 3;
						continue;
					}
					return null;
				default:
					continue;
				}
			}
			else
			{
				flag = true;
			}
			break;
		}
		if (!flag)
		{
			p--;
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		double result;
		if (!double.TryParse(stringBuilder.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out result))
		{
			return null;
		}
		return result;
	}

	private static object DecodeObject(char[] buffer, ref int p, int length)
	{
		int num = 0;
		object key = null;
		Hashtable hashtable = new Hashtable();
		while (p < length)
		{
			char c = buffer[p++];
			switch (num)
			{
			case 0:
				switch (c)
				{
				case '"':
					if ((key = DecodeString(buffer, ref p, length)) == null)
					{
						return null;
					}
					num = 1;
					break;
				case '}':
					return hashtable;
				default:
					if (!IsWhitespace(c))
					{
						return null;
					}
					break;
				}
				break;
			case 1:
				if (c == ':')
				{
					num = 2;
				}
				else if (!IsWhitespace(c))
				{
					return null;
				}
				break;
			case 2:
				p--;
				hashtable.Add(key, DecodeValue(buffer, ref p, length));
				num = 3;
				break;
			case 3:
				switch (c)
				{
				case ',':
					num = 0;
					break;
				case '}':
					return hashtable;
				default:
					if (!IsWhitespace(c))
					{
						return null;
					}
					break;
				}
				break;
			default:
				return null;
			}
		}
		return null;
	}

	private static object DecodeArray(char[] buffer, ref int p, int length)
	{
		int num = 0;
		ArrayList arrayList = new ArrayList();
		while (p < length)
		{
			char c = buffer[p++];
			switch (num)
			{
			case 0:
				if (c == ']')
				{
					return arrayList;
				}
				if (!IsWhitespace(c))
				{
					p--;
					arrayList.Add(DecodeValue(buffer, ref p, length));
					num = 1;
				}
				break;
			case 1:
				switch (c)
				{
				case ',':
					num = 0;
					break;
				case ']':
					return arrayList;
				default:
					if (!IsWhitespace(c))
					{
						return null;
					}
					break;
				}
				break;
			default:
				return null;
			}
		}
		return null;
	}

	private static object DecodeValue(char[] buffer, ref int p, int length)
	{
		int num = 0;
		while (p < length)
		{
			char c = buffer[p++];
			switch (num)
			{
			case 0:
				switch (c)
				{
				case '"':
					return DecodeString(buffer, ref p, length);
				case '-':
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					p--;
					return DecodeNumber(buffer, ref p, length);
				default:
					switch (c)
					{
					case '{':
						return DecodeObject(buffer, ref p, length);
					case '[':
						return DecodeArray(buffer, ref p, length);
					case 't':
						num = 10;
						break;
					case 'f':
						num = 20;
						break;
					case 'n':
						num = 30;
						break;
					default:
						if (!IsWhitespace(c))
						{
							return null;
						}
						break;
					}
					break;
				}
				break;
			case 10:
				if (c == 'r')
				{
					num = 11;
					break;
				}
				return null;
			case 11:
				if (c == 'u')
				{
					num = 12;
					break;
				}
				return null;
			case 12:
				if (c == 'e')
				{
					return true;
				}
				return null;
			case 20:
				if (c == 'a')
				{
					num = 21;
					break;
				}
				return null;
			case 21:
				if (c == 'l')
				{
					num = 22;
					break;
				}
				return null;
			case 22:
				if (c == 's')
				{
					num = 23;
					break;
				}
				return null;
			case 23:
				if (c == 'e')
				{
					return false;
				}
				return null;
			case 30:
				if (c == 'u')
				{
					num = 31;
					break;
				}
				return null;
			case 31:
				if (c == 'l')
				{
					num = 32;
					break;
				}
				return null;
			case 32:
				if (c == 'l')
				{
					return null;
				}
				return null;
			default:
				return null;
			}
		}
		return null;
	}

	private static void EncodeString(StringBuilder encodedString, string s)
	{
		encodedString.Append('"');
		foreach (char c in s)
		{
			if (c == '"')
			{
				encodedString.Append("\\\"");
			}
			else if (c == '\\')
			{
				encodedString.Append("\\\\");
			}
			else if (c == '/')
			{
				encodedString.Append("\\/");
			}
			else if (c == '\b')
			{
				encodedString.Append("\\b");
			}
			else if (c == '\f')
			{
				encodedString.Append("\\f");
			}
			else if (c == '\n')
			{
				encodedString.Append("\\n");
			}
			else if (c == '\r')
			{
				encodedString.Append("\\r");
			}
			else if (c == '\t')
			{
				encodedString.Append("\\t");
			}
			else if (c > '\u007f')
			{
				encodedString.Append("\\u");
				int num = c;
				encodedString.Append(num.ToString("x4"));
			}
			else
			{
				encodedString.Append(c);
			}
		}
		encodedString.Append('"');
	}

	private static void EncodeNumber(StringBuilder encodedString, double number)
	{
		encodedString.Append(number);
	}

	private static void EncodeObject(StringBuilder encodedString, Hashtable collection)
	{
		bool flag = true;
		encodedString.Append('{');
		foreach (object key in collection.Keys)
		{
			if (!flag)
			{
				encodedString.Append(',');
			}
			EncodeString(encodedString, key.ToString());
			encodedString.Append(':');
			EncodeValue(encodedString, collection[key]);
			flag = false;
		}
		encodedString.Append('}');
	}

	private static void EncodeArray(StringBuilder encodedString, IEnumerable array)
	{
		bool flag = true;
		encodedString.Append('[');
		foreach (object item in array)
		{
			if (!flag)
			{
				encodedString.Append(',');
			}
			EncodeValue(encodedString, item);
			flag = false;
		}
		encodedString.Append(']');
	}

	private static void EncodeValue(StringBuilder encodedString, object JsonObject)
	{
		if (JsonObject == null)
		{
			encodedString.Append("null");
		}
		else if (JsonObject.GetType().IsArray)
		{
			EncodeArray(encodedString, (IEnumerable)JsonObject);
		}
		else if (JsonObject is ArrayList)
		{
			EncodeArray(encodedString, (IEnumerable)JsonObject);
		}
		else if (JsonObject is string)
		{
			EncodeString(encodedString, (string)JsonObject);
		}
		else if (JsonObject is Hashtable)
		{
			EncodeObject(encodedString, (Hashtable)JsonObject);
		}
		else if (JsonObject is bool)
		{
			if ((bool)JsonObject)
			{
				encodedString.Append("true");
			}
			else
			{
				encodedString.Append("false");
			}
		}
		else if (JsonObject.GetType().IsPrimitive)
		{
			EncodeNumber(encodedString, Convert.ToDouble(JsonObject));
		}
		else
		{
			encodedString.Append("null");
		}
	}

	public static string JsonEncode(object JsonObject)
	{
		StringBuilder stringBuilder = new StringBuilder();
		EncodeValue(stringBuilder, JsonObject);
		return stringBuilder.ToString();
	}

	public static object JsonDecode(string JsonString)
	{
		int p = 0;
		char[] array = JsonString.ToCharArray();
		return DecodeValue(array, ref p, array.Length);
	}
}
