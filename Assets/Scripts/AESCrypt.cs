using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class AESCrypt
{
	private static PaddingMode mPaddingMode = PaddingMode.Zeros;

	private static CipherMode mCipherMode = CipherMode.CBC;

	private static int mKeySize = 128;

	private static int mBlockSize = 256;

	public static bool HaveKey()
	{
		string kY = AESCryptKey.GetKY();
		return !kY.Equals(string.Empty);
	}

	private static RijndaelManaged _init(ref byte[] kyb, ref byte[] ivb)
	{
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Padding = mPaddingMode;
		rijndaelManaged.Mode = mCipherMode;
		rijndaelManaged.KeySize = mKeySize;
		rijndaelManaged.BlockSize = mBlockSize;
		string kY = AESCryptKey.GetKY();
		string iV = AESCryptKey.GetIV();
		kyb = Encoding.UTF8.GetBytes(kY);
		ivb = Encoding.UTF8.GetBytes(iV);
		return rijndaelManaged;
	}

	public static string Encrypt(int iDeInt)
	{
		return Encrypt(iDeInt.ToString());
	}

	public static string Encrypt(float iDeFloat)
	{
		return Encrypt(iDeFloat.ToString());
	}

	public static string Encrypt(string iDeText)
	{
		//Discarded unreachable code: IL_006e, IL_0082, IL_0096, IL_00a8, IL_00b6
		if (!HaveKey())
		{
			return iDeText;
		}
		try
		{
			byte[] kyb = null;
			byte[] ivb = null;
			using (RijndaelManaged rijndaelManaged = _init(ref kyb, ref ivb))
			{
				ICryptoTransform transform = rijndaelManaged.CreateEncryptor(kyb, ivb);
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
					{
						byte[] bytes = Encoding.UTF8.GetBytes(iDeText);
						cryptoStream.Write(bytes, 0, bytes.Length);
						cryptoStream.FlushFinalBlock();
						byte[] inArray = memoryStream.ToArray();
						return Convert.ToBase64String(inArray);
					}
				}
			}
		}
		catch
		{
			return iDeText;
		}
	}

	public static string Decrypt(string iEnText)
	{
		//Discarded unreachable code: IL_007b, IL_008f, IL_00a3, IL_00b5, IL_00c3
		if (!HaveKey())
		{
			return iEnText;
		}
		try
		{
			byte[] kyb = null;
			byte[] ivb = null;
			using (RijndaelManaged rijndaelManaged = _init(ref kyb, ref ivb))
			{
				ICryptoTransform transform = rijndaelManaged.CreateDecryptor(kyb, ivb);
				byte[] array = Convert.FromBase64String(iEnText);
				using (MemoryStream stream = new MemoryStream(array))
				{
					using (CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read))
					{
						byte[] array2 = new byte[array.Length];
						cryptoStream.Read(array2, 0, array2.Length);
						return Encoding.UTF8.GetString(array2).Replace("\0", string.Empty);
					}
				}
			}
		}
		catch
		{
			return iEnText;
		}
	}
}
