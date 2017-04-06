using System.Text.RegularExpressions;

namespace Chuchuka.SharePoint.Utilities.Utilities
{
	public class SPStringUtility
	{
		/// <summary>
		/// Replace special characters forbidden in File names
		/// </summary>
		/// <param name="str">Initial string</param>
		/// <param name="replacer">String to replace invalid characters with</param>
		/// <returns></returns>
		public static string ReplaceForbiddenFileCharacters(string str, string replacer = "")
		{
			Regex pattern = new Regex("[~#%&\\*\\{}[\\]\\\\:<>\\?\\/+=,;@|'\\\"]?");
			return pattern.Replace(str, replacer);
		}

		/// <summary>
		/// Replace special characters forbidden in Site and Group names
		/// </summary>
		/// <param name="str">Initial string</param>
		/// <param name="replacer">String to replace invalid characters with</param>
		/// <returns></returns>
		public static string ReplaceForbiddenSiteCharacters(string str, string replacer = "")
		{
			Regex pattern = new Regex("[~#%&\\*\\{}[\\]\\\\:<>\\?\\/+=,;@|'\\\"]?");
			return pattern.Replace(str, replacer);
		}
	}
}
