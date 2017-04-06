using System;
using Microsoft.SharePoint;

namespace Chuchuka.SharePoint.Utilities.Utilities
{
	public class UnsafeUpdatesContext : IDisposable
	{
		public SPWeb Web { get; protected set; }

		private readonly bool _originalValue;

		public UnsafeUpdatesContext(SPWeb web)
		{
			_originalValue = web.AllowUnsafeUpdates;

			Web = web;
			Web.AllowUnsafeUpdates = true;
		}
		
		public void Dispose()
		{
			Web.AllowUnsafeUpdates = _originalValue;
			Web = null;
		}
	}
}
