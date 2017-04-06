namespace Chuchuka.SharePoint.Utilities.Extensions
{
	public static class StringExtensions
	{
		public static string ToStringSafe(this object @object)
		{
			return (@object ?? string.Empty).ToString();
		}
	}
}
