using System.Security.Cryptography;
using System.IO;
using System.Text;
using System;

namespace AP.Utilities
{
	public static class UnityAES
	{
		#region Public Encrypt

		/// <summary>
		/// Encrypt a string string, based on a string key.
		/// </summary>
		/// <param name="stringToEncrypt"></param>
		/// <param name="password"></param>
		/// <returns>String in base64 format</returns>
		public static string EncryptToString(string stringToEncrypt, string password)
		{
			// Get the bytes of the string
			byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(stringToEncrypt);
			byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

			// Hash the password with SHA256
			passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

			byte[] bytesEncrypted = Encrypt(bytesToBeEncrypted, passwordBytes);
			string result = Convert.ToBase64String(bytesEncrypted);
			return result;
		}

		/// <summary>
		/// Encrypt a string string, based on a string key.
		/// </summary>
		/// <param name="stringToEncrypt"></param>
		/// <param name="password"></param>
		/// <returns>Byte Array data</returns>
		public static byte[] EncryptToBytes(string stringToEncrypt, string password)
		{
			// Get the bytes of the string
			byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(stringToEncrypt);
			byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

			// Hash the password with SHA256
			passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

			byte[] bytesEncrypted = Encrypt(bytesToBeEncrypted, passwordBytes);
			return bytesEncrypted;
		}

		private static byte[] Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
		{
			// Set your salt here, change it to meet your flavor:
			// The salt bytes must be at least 8 bytes.
			byte[] saltBytes = { 1, 3, 5, 7, 2, 4, 6, 8 };

			using var ms = new MemoryStream();
			using var aes = new RijndaelManaged();
			aes.KeySize = 256;
			aes.BlockSize = 128;

			var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
			// AES.Key = passwordBytes;
			// AES.IV = new byte[]{
			//     0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
			// };
			aes.Key = key.GetBytes(aes.KeySize / 8);
			aes.IV = key.GetBytes(aes.BlockSize / 8);
			aes.Mode = CipherMode.CBC;

			try
			{
				using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
				{
					cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
					cs.Close();
				}

				return ms.ToArray();
			}
			catch
			{
				return Array.Empty<byte>();
			}
		}

		#endregion

		#region Public Decrypt

		/// <summary>
		/// decrypt a string in base64 using a key as reference
		/// and opt for a string with the original information 
		/// </summary>
		/// <param name="data">Data to Decrypt in base64 format</param>
		/// <param name="password">Key to decrypt the data</param> 
		/// <returns>Returns a string with the decrypted data</returns>
		public static string DecryptString(string data, string password)
		{
			// Get the bytes of the string
			byte[] bytesToBeDecrypted = Convert.FromBase64String(data);
			byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
			passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
			byte[] bytesDecrypted = Decrypt(bytesToBeDecrypted, passwordBytes);
			string result = Encoding.UTF8.GetString(bytesDecrypted);
			return result;
		}

		/// <summary>
		/// Decrypts an Array Bytes using a key as reference
		/// and get a string with the original information. 
		/// </summary>
		/// <param name="bytesToBeDecrypted">Data to be Decrypted</param>
		/// <param name="password">Key to decrypt the data</param>.
		/// <returns>Returns a string with the decrypted data</returns>.
		public static string DecryptString(byte[] bytesToBeDecrypted, string password)
		{
			// Get the bytes of the string
			byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
			passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
			byte[] bytesDecrypted = Decrypt(bytesToBeDecrypted, passwordBytes);
			string result = Encoding.UTF8.GetString(bytesDecrypted);
			return result;
		}

		private static byte[] Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
		{
			// Set your salt here, change it to meet your flavor:
			// The salt bytes must be at least 8 bytes.
			byte[] saltBytes = { 1, 3, 5, 7, 2, 4, 6, 8 };

			using var ms = new MemoryStream();
			using var aes = new RijndaelManaged();
			aes.KeySize = 256;
			aes.BlockSize = 128;

			var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
			aes.Key = key.GetBytes(aes.KeySize / 8);
			aes.IV = key.GetBytes(aes.BlockSize / 8);

			aes.Mode = CipherMode.CBC;

			try
			{
				using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
				{
					cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
					cs.Close();
				}

				return ms.ToArray();
			}
			catch
			{
				return Array.Empty<byte>();
			}
		}

		#endregion
	}
}