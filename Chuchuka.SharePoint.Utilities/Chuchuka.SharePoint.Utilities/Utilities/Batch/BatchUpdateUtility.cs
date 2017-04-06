using System.Collections.Generic;
using System.Xml.Linq;

namespace Chuchuka.SharePoint.Utilities.Utilities.Batch
{
	public static class BatchUpdateUtility
	{
		public static IEnumerable<BatchResult> ParseBatchResults(string resultXml)
		{
			var xdoc = XDocument.Parse(resultXml);
			if (xdoc.Root == null) yield break;

			foreach (XElement result in xdoc.Root.Elements("Result"))
			{
				var x = new BatchResult
				{
					MethodID = result.Attribute("ID").Value,
					Code = int.Parse(result.Attribute("Code").Value)
				};

				var itemId = result.Element("ID");
				if (itemId != null) x.ItemID = itemId.Value;

				var error = result.Element("ErrorText");
				if (error != null) x.ErrorText = error.Value;

				yield return x;
			}
		}
	}
}
