using UnityEngine;

namespace AP.Utilities.Attributes
{
	public class EncryptedStringAttribute : PropertyAttribute
	{
		private readonly string key;
    
		public EncryptedStringAttribute(string key)
		{
			this.key = key;
		}
    
		public string Encrypt(string value) => UnityAES.EncryptToString(value, key);
    
		public string Decrypt(string value) => UnityAES.DecryptString(value, key);
	}
}