using System.Collections;

namespace Chuchuka.SharePoint.Utilities.Extensions
{
	public static class HashtableExtensions
	{
		public static string GetStringSafe(this Hashtable hashtable, string key, string defaultValue = null)
		{
			if (!hashtable.ContainsKey(key)) return defaultValue;
			return GetString(hashtable, key);
		}

		public static string GetString(this Hashtable hashtable, string key)
		{
			object value = hashtable[key];
			return value.ToStringSafe();
		}

		public static bool? GetBoolSafe(this Hashtable hashtable, string key, bool? defaultValue = null)
		{
			if (!hashtable.ContainsKey(key)) return defaultValue;
			return GetBool(hashtable, key);
		}

		public static bool GetBool(this Hashtable hashtable, string key)
		{
			object value = hashtable[key];
			return bool.Parse(value.ToStringSafe());
		}
	}
}
