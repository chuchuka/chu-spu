using System;
using System.Collections.Generic;
using Microsoft.SharePoint;

namespace Chuchuka.SharePoint.Utilities.Extensions
{
	public static class SPListExtensions
	{
		/// <summary>
		/// Get items from SPList (throttling safe)
		/// </summary>
		/// <param name="list"></param>
		/// <param name="query"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public static IEnumerable<SPListItem> GetItemsPaged(this SPList list, SPQuery query, uint pageSize = 1000)
		{
			if (pageSize > 5000)
				throw new ArgumentOutOfRangeException("pageSize");

			query.RowLimit = pageSize;
			query.QueryThrottleMode = SPQueryThrottleOption.Override;

			var pagingToken = string.Empty;
			while (true)
			{
				query.ListItemCollectionPosition = new SPListItemCollectionPosition(pagingToken);
				SPListItemCollection items = list.GetItems(query);

				foreach (SPListItem item in items)
				{
					yield return item;
				}

				if (items.ListItemCollectionPosition == null)
				{
					break;
				}

				pagingToken = items.ListItemCollectionPosition.PagingInfo;
			}
		}
	}
}
