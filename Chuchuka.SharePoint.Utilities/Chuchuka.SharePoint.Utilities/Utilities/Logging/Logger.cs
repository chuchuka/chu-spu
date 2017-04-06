using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chuchuka.SharePoint.Utilities.Extensions;
using Microsoft.SharePoint.Administration;

namespace Chuchuka.SharePoint.Utilities.Utilities.Logging
{
	public class Logger: SPDiagnosticsServiceBase
	{
		public const string LoggerName = "My Logger";
		public const string Area = "My Logging Area";
		public const Category DefaultCategory = Category.Default;

		protected static readonly Logger Instance = new Logger();

		protected Logger() : base(LoggerName, SPFarm.Local)
		{
		}
		
		/// <summary>
		/// Write information about Exception to ULS with Unexpected severity level
		/// </summary>
		/// <param name="exception">Exception (must be not null)</param>
		/// <param name="message">Additional message</param>
		/// <param name="eventId">Event ID</param>
		/// <param name="category">Log category</param>
		public static void Exception(Exception exception, string message = null, uint eventId = 0, Category category = DefaultCategory)
		{
			if (exception == null) return;

			var messageBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(message))
			{
				messageBuilder.AppendFormat("{0}\r\n", message);
			}
			exception.WriteExceptionDetails(messageBuilder);
			WriteEntry(messageBuilder.ToString(), eventId, TraceSeverity.Unexpected, category);
		}

		/// <summary>
		/// Write entry to ULS with Unexpected severity level
		/// </summary>
		/// <param name="message">Entry message</param>
		/// <param name="eventId">EventID</param>
		/// <param name="category">Log category</param>
		public static void Unexpected(string message, uint eventId = 0, Category category = DefaultCategory)
		{
			WriteEntry(message, eventId, TraceSeverity.Unexpected, category);
		}

		/// <summary>
		/// Write entry to ULS with Medium severity level
		/// </summary>
		/// <param name="message">Entry message</param>
		/// <param name="eventId">EventID</param>
		/// <param name="category">Log category</param>
		public static void Log(string message, uint eventId = 0, Category category = DefaultCategory)
		{
			WriteEntry(message, eventId, TraceSeverity.Medium, category);
		}
		
		protected static void WriteEntry(string message, uint eventId = 0, TraceSeverity severity = TraceSeverity.Medium, Category category = DefaultCategory)
		{
			var spCategory = Instance.Areas[Area].Categories[category.ToString()];
			Instance.WriteTrace(eventId, spCategory, severity, message);
		}

		protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
		{
			yield return new SPDiagnosticsArea(Area, ProvideCategories());
		}

		private IEnumerable<SPDiagnosticsCategory> ProvideCategories()
		{
			var values = Enum.GetNames(typeof(Category));
			return values.Select(value => new SPDiagnosticsCategory(value, TraceSeverity.Medium, EventSeverity.Information));
		}
	}
}
