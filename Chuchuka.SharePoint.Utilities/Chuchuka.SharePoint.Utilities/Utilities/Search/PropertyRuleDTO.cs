using Microsoft.Office.Server.Search.Administration;

namespace Chuchuka.SharePoint.Utilities.Utilities.Search
{
	/// <summary>
	/// Data transfer object for a Property Rule of SharePoint 2013 Search
	/// </summary>
	public struct PropertyRuleDTO
	{
		public string ManagedPropertyName { get; set; }
		public PropertyRuleOperator.DefaultOperator Operator { get; set; }
		public string[] Values { get; set; }
	}
}
