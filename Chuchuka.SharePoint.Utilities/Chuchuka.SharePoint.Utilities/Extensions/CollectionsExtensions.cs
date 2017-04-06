﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Chuchuka.SharePoint.Utilities.Extensions
{
	public static class CollectionsExtensions
	{
		public static bool In<T>(this T value, IEnumerable<T> collection)
		{
			return collection.Contains(value);
		}

		public static IEnumerable<T> DistinctBy<T, TIdentity>(this IEnumerable<T> source, Func<T, TIdentity> identitySelector)
		{
			return source.Distinct(By(identitySelector));
		}

		public static IEqualityComparer<TSource> By<TSource, TIdentity>(Func<TSource, TIdentity> identitySelector)
		{
			return new DelegateComparer<TSource, TIdentity>(identitySelector);
		}

		private class DelegateComparer<T, TIdentity> : IEqualityComparer<T>
		{
			private readonly Func<T, TIdentity> _identitySelector;

			public DelegateComparer(Func<T, TIdentity> identitySelector)
			{
				_identitySelector = identitySelector;
			}

			public bool Equals(T x, T y)
			{
				return Equals(_identitySelector(x), _identitySelector(y));
			}

			public int GetHashCode(T obj)
			{
				return _identitySelector(obj).GetHashCode();
			}
		}
	}
}
