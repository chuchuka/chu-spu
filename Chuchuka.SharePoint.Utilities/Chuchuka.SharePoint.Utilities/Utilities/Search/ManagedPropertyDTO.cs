using Microsoft.Office.Server.Search.Administration;

namespace Chuchuka.SharePoint.Utilities.Utilities.Search
{
	/// <summary>
	/// Data Transfer Object for a Managed Property of SharePoint 2013 Search
	/// </summary>
	public struct ManagedPropertyDTO
	{
		public string Name { get; set; }
		public ManagedDataType Type { get; set; }
		public bool Searchable { get; set; }
		public bool Queryable { get; set; }
		public bool Retrievable { get; set; }
		public bool Refinable { get; set; }
		public bool Sortable { get; set; }
		public bool SafeForAnonymous { get; set; }
		public bool HasMultipleValues { get; set; }
	}
}
