using System;
using Microsoft.SharePoint;

namespace Chuchuka.SharePoint.Utilities.Utilities
{
	public class DisabledEventsScope : SPItemEventReceiver, IDisposable
	{
		private readonly bool _originalValue;

		public DisabledEventsScope()
		{
			_originalValue = EventFiringEnabled;
			EventFiringEnabled = false;
		}

		public void Dispose()
		{
			EventFiringEnabled = _originalValue;
		}
	}
}
