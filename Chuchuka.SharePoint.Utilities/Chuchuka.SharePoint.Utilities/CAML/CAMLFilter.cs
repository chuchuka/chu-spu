using System;
using System.Globalization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace Chuchuka.SharePoint.Utilities.CAML
{
	/// <summary>
	/// Class that holds filter expressions that can be used by the <see cref="CAMLBuilder"/>. 
	/// </summary>
	public class CAMLFilter
	{
		/// <summary>
		/// The filter expression to use when building a query. 
		/// </summary>
		public string FilterExpression { get; private set; }

		public override string ToString()
		{
			return FilterExpression;
		}

		private CAMLFilter()
		{
		}

		public static CAMLFilter Filter(Operator @operator, string name, object value, SPFieldType camlType)
		{
			var filter = new CAMLFilter();
			filter.FilterExpression = string.Format(CultureInfo.InvariantCulture,
				"<{0}><FieldRef Name='{1}' {4}/><Value Type='{2}'>{3}</Value></{0}>",
				@operator, name, camlType, value, camlType == SPFieldType.User || camlType == SPFieldType.Lookup ? "LookupId='TRUE'" : "");
			return filter;
		}

		public static CAMLFilter Filter(Operator @operator, Guid fieldId, object value, SPFieldType camlType)
		{
			var filter = new CAMLFilter();
			filter.FilterExpression = string.Format(CultureInfo.InvariantCulture,
				"<{0}><FieldRef ID='{1}' {4}/><Value Type='{2}'>{3}</Value></{0}>",
				@operator, fieldId, camlType, value, camlType == SPFieldType.User || camlType == SPFieldType.Lookup ? "LookupId='TRUE'" : "");
			return filter;
		}

		public static CAMLFilter Unary(Operator @operator, Guid fieldId)
		{
			var filter = new CAMLFilter();
			filter.FilterExpression = string.Format(CultureInfo.InvariantCulture,
				"<{0}><FieldRef ID='{1}'/></{0}>", @operator, fieldId);
			return filter;
		}

		public static CAMLFilter Unary(Operator @operator, string name)
		{
			var filter = new CAMLFilter();
			filter.FilterExpression = string.Format(CultureInfo.InvariantCulture,
				"<{0}><FieldRef Name='{1}'/></{0}>", @operator, name);
			return filter;
		}

		public static CAMLFilter And(CAMLFilter filter1, CAMLFilter filter2)
		{
			var filter = new CAMLFilter();
			filter.FilterExpression = string.Format("<And>{0}{1}</And>", filter1, filter2);
			return filter;
		}

		public static CAMLFilter Or(CAMLFilter filter1, CAMLFilter filter2)
		{
			var filter = new CAMLFilter();
			filter.FilterExpression = string.Format("<Or>{0}{1}</Or>", filter1, filter2);
			return filter;
		}

#region Type-Specific
		public static CAMLFilter Eq(Guid id, string value)
		{
			return Filter(Operator.Eq, id, value, SPFieldType.Text);
		}

		public static CAMLFilter Eq(string name, string value)
		{
			return Filter(Operator.Eq, name, value, SPFieldType.Text);
		}

		public static CAMLFilter Eq(Guid id, int value)
		{
			return Filter(Operator.Eq, id, value, SPFieldType.Integer);
		}

		public static CAMLFilter Eq(string name, int value)
		{
			return Filter(Operator.Eq, name, value, SPFieldType.Integer);
		}

		public static CAMLFilter Eq(Guid id, DateTime value)
		{
			return Filter(Operator.Eq, id, SPUtility.CreateISO8601DateTimeFromSystemDateTime(value), SPFieldType.DateTime);
		}

		public static CAMLFilter Eq(string name, DateTime value)
		{
			return Filter(Operator.Eq, name, SPUtility.CreateISO8601DateTimeFromSystemDateTime(value), SPFieldType.DateTime);
		}

		public static CAMLFilter Eq(Guid id, bool value)
		{
			return Filter(Operator.Eq, id, value ? 1 : 0, SPFieldType.Boolean);
		}

		public static CAMLFilter Eq(string name, bool value)
		{
			return Filter(Operator.Eq, name, value ? 1 : 0, SPFieldType.Boolean);
		}

		public static CAMLFilter Eq(Guid id, Guid value)
		{
			return Filter(Operator.Eq, id, value, SPFieldType.Guid);
		}

		public static CAMLFilter Eq(string name, Guid value)
		{
			return Filter(Operator.Eq, name, value, SPFieldType.Guid);
		}

		public static CAMLFilter Neq(Guid id, bool value)
		{
			return Filter(Operator.Neq, id, value ? 1 : 0, SPFieldType.Boolean);
		}

		public static CAMLFilter Neq(string name, bool value)
		{
			return Filter(Operator.Neq, name, value ? 1 : 0, SPFieldType.Boolean);
		}

		public static CAMLFilter Neq(Guid id, int value)
		{
			return Filter(Operator.Neq, id, value, SPFieldType.Number);
		}

		public static CAMLFilter Neq(string name, int value)
		{
			return Filter(Operator.Neq, name, value, SPFieldType.Number);
		}

		public static CAMLFilter BeginsWith(Guid id, string value)
		{
			return Filter(Operator.BeginsWith, id, value, SPFieldType.Text);
		}

		public static CAMLFilter BeginsWith(string name, string value)
		{
			return Filter(Operator.BeginsWith, name, value, SPFieldType.Text);
		}

		public static CAMLFilter BeginsWith(Guid id, SPContentTypeId value)
		{
			return Filter(Operator.BeginsWith, id, value, SPFieldType.ContentTypeId);
		}

		public static CAMLFilter BeginsWith(string name, SPContentTypeId value)
		{
			return Filter(Operator.BeginsWith, name, value, SPFieldType.ContentTypeId);
		}

		public static CAMLFilter Leq(Guid id, DateTime value)
		{
			return Filter(Operator.Leq, id, SPUtility.CreateISO8601DateTimeFromSystemDateTime(value), SPFieldType.DateTime);
		}

		public static CAMLFilter Leq(string name, DateTime value)
		{
			return Filter(Operator.Leq, name, SPUtility.CreateISO8601DateTimeFromSystemDateTime(value), SPFieldType.DateTime);
		}
#endregion
	}
}