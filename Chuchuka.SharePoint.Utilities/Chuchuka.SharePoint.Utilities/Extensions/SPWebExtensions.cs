using System.IO;
using Chuchuka.SharePoint.Utilities.Utilities.Logging;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace Chuchuka.SharePoint.Utilities.Extensions
{
	public static class SPWebExtensions
	{
		/// <summary>
		/// Get SPFile by its URL
		/// </summary>
		/// <param name="web"></param>
		/// <param name="url">URL of a file</param>
		/// <returns>SPFile object or null if the file is not found</returns>
		public static SPFile TryGetFile(this SPWeb web, string url)
		{
			SPFile file = web.GetFile(url);
			return !file.Exists ? null : file;
		}

		/// <summary>
		/// Get SPList by its site-relative URL
		/// </summary>
		/// <param name="web"></param>
		/// <param name="url">Site-relative URL of a list</param>
		/// <returns>SPList object or null if the list is not found</returns>
		public static SPList TryGetList(this SPWeb web, string url)
		{
			try
			{
				string listUrl = SPUrlUtility.CombineUrl(web.ServerRelativeUrl, url);
				return web.GetList(listUrl);
			}
			catch (FileNotFoundException exception)
			{
				string message = string.Format("List with '{0}' URL not found.", url);
				Logger.Exception(exception, message);

				return null;
			}
		}
	}
}
