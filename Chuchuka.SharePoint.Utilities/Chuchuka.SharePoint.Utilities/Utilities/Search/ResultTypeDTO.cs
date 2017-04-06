namespace Chuchuka.SharePoint.Utilities.Utilities.Search
{
	/// <summary>
	/// Data Transfer Object for a Result Type of SharePoint 2013 Search
	/// </summary>
	public struct ResultTypeDTO
	{
		/// <summary>
		/// Name of the search Result Type
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Priority number of the Result Type
		/// </summary>
		public int Priority { get; set; }

		/// <summary>
		/// Name of a mapped search Result Source
		/// </summary>
		public string ResultSource { get; set; }

		/// <summary>
		/// Internal names of mapped built-in types of contents
		/// </summary>
		public string[] BuiltInRules { get; set; }

		/// <summary>
		/// Custom query rules applied to the Result Type
		/// </summary>
		public PropertyRuleDTO[] CustomRules { get; set; }

		/// <summary>
		/// URL of a display template for the Result Type
		/// </summary>
		public string DisplayTemplateUrl { get; set; }
	}
}
