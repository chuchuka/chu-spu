using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace Chuchuka.SharePoint.Utilities.CAML
{
	public enum Closure
	{
		And,
		Or
	}

	public enum Operator
	{
		Eq,
		Neq,
		Lt,
		Leq,
		Gt,
		Geq,
		Contains,
		BeginsWith,
		DateRangesOverlap,
		IsNotNull,
		IsNull,
		Membership,
		Includes
	}

	public class CAMLBuilder
	{
		private CAMLFilter _query;
		private readonly List<CAMLOrderBy> _orders = new List<CAMLOrderBy>();

		public static CAMLBuilder Create()
		{
			return new CAMLBuilder();
		}

		private CAMLBuilder()
		{
		}

		public CAMLBuilder Add(CAMLFilter filter, Closure closure = Closure.And)
		{
			if (filter == null) throw new ArgumentNullException("filter");

			if (_query == null)
			{
				_query = filter;
				return this;
			}

			CAMLFilter newQuery;
			switch (closure)
			{
				case Closure.And:
					newQuery = CAMLFilter.And(_query, filter);
					break;
				case Closure.Or:
					newQuery = CAMLFilter.Or(_query, filter);
					break;
				default:
					throw new ArgumentException("Invalid closure type.", "closure");
			}
			_query = newQuery;
			return this;
		}

		public CAMLBuilder OrderBy(string fieldName, bool desc = false)
		{
			_orders.Add(CAMLOrderBy.OrderBy(fieldName, desc));
			return this;
		}

		public CAMLBuilder OrderBy(Guid fieldId, bool desc = false)
		{
			_orders.Add(CAMLOrderBy.OrderBy(fieldId, desc));
			return this;
		}

		/// <summary>
		/// Build a CAML SPQuery object, based on the filter expressions that have been added to it. 
		/// </summary>
		/// <returns></returns>
		public SPQuery BuildQuery()
		{
			return new SPQuery { Query = Build() };
		}

		/// <summary>
		/// Build a CAML SPSiteDataQuery object, based on the filter expressions that have been added to it. 
		/// </summary>
		/// <returns></returns>
		public SPSiteDataQuery BuildSiteDataQuery()
		{
			var query = new SPSiteDataQuery { Query = Build() };
			return query;
		}

		/// <summary>
		/// Build a CAML Query string, based on the filter expressions that have been added to it. 
		/// </summary>
		/// <returns></returns>
		public string Build()
		{
			var queryBuilder = new StringBuilder();
			queryBuilder.AppendFormat("<Where>{0}</Where>", _query);
			if (_orders.Count > 0)
				queryBuilder.AppendFormat("<OrderBy>{0}</OrderBy>", _orders.Select(o => o.Expression).Aggregate((c, x) => c + x));
			return queryBuilder.ToString();
		}
	}
}
