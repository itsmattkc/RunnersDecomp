using LitJson;
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class CryptoUtility
{
	private const string CryptoKey = "Ec7bLaTdSuXuf5pW";

	private static string CryptoCode = string.Empty;

	public static bool CryptoFlag
	{
		get
		{
			bool result = true;
			DebugGameObject instance = SingletonGameObject<DebugGameObject>.Instance;
			if (instance != null)
			{
				result = instance.crypt;
			}
			return result;
		}
	}

	public static string code
	{
		get
		{
			if (string.IsNullOrEmpty(CryptoCode))
			{
				for (int i = 0; i < 16; i++)
				{
					int num = (UnityEngine.Random.Range(0, 50) + i) % 18;
					if (num < 10)
					{
						CryptoCode += num;
						continue;
					}
					switch (num % 12)
					{
					case 0:
						CryptoCode += "A";
						break;
					case 1:
						CryptoCode += "B";
						break;
					case 2:
						CryptoCode += "C";
						break;
					case 3:
						CryptoCode += "D";
						break;
					case 4:
						CryptoCode += "E";
						break;
					case 5:
						CryptoCode += "F";
						break;
					case 6:
						CryptoCode += "a";
						break;
					case 7:
						CryptoCode += "b";
						break;
					case 8:
						CryptoCode += "c";
						break;
					case 9:
						CryptoCode += "d";
						break;
					case 10:
						CryptoCode += "e";
						break;
					case 11:
						CryptoCode += "f";
						break;
					}
				}
			}
			return CryptoCode;
		}
		set
		{
			CryptoCode = value;
		}
	}

	public static string Encrypt(string text)
	{
		//Discarded unreachable code: IL_0084
		AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider();
		aesCryptoServiceProvider.Mode = CipherMode.CBC;
		aesCryptoServiceProvider.Padding = PaddingMode.PKCS7;
		aesCryptoServiceProvider.BlockSize = 128;
		aesCryptoServiceProvider.KeySize = 128;
		aesCryptoServiceProvider.IV = Encoding.UTF8.GetBytes(code);
		aesCryptoServiceProvider.Key = Encoding.UTF8.GetBytes(CryptoKey);
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		using (ICryptoTransform cryptoTransform = aesCryptoServiceProvider.CreateEncryptor())
		{
			byte[] inArray = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
			return Convert.ToBase64String(inArray);
		}
	}

	public static string Decrypt(string text)
	{
		//Discarded unreachable code: IL_009b
		AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider();
		aesCryptoServiceProvider.Mode = CipherMode.CBC;
		aesCryptoServiceProvider.Padding = PaddingMode.PKCS7;
		aesCryptoServiceProvider.BlockSize = 128;
		aesCryptoServiceProvider.KeySize = 128;
		aesCryptoServiceProvider.IV = Encoding.UTF8.GetBytes(code);
		aesCryptoServiceProvider.Key = Encoding.UTF8.GetBytes(CryptoKey);
		string cryptoCode = text.Substring(0, 16);
		byte[] array = Convert.FromBase64String(text);
		using (ICryptoTransform cryptoTransform = aesCryptoServiceProvider.CreateDecryptor())
		{
			byte[] bytes = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
			string @string = Encoding.UTF8.GetString(bytes);
			CryptoCode = cryptoCode;
			return @string;
		}
	}

	public static JsonData SRDecryptJson(JsonData json)
	{
		JsonData result = json;
		if (NetUtil.GetJsonInt(json, "secure") != 0)
		{
			string jsonString = NetUtil.GetJsonString(json, "key");
			if (!string.IsNullOrEmpty(jsonString))
			{
				string jsonString2 = NetUtil.GetJsonString(json, "param");
				code = jsonString;
				string json2 = Decrypt(jsonString2);
				result = JsonMapper.ToObject(json2);
			}
		}
		else
		{
			result = NetUtil.GetJsonObject(json, "param");
		}
		return result;
	}
}
