using System;
using System.Text;

namespace Chuchuka.SharePoint.Utilities.Extensions
{
	public static class ExceptionExtensions
	{
		/// <summary>
		/// Write details of exception to string builder. This method is called recursively to format all the inner exceptions.
		/// </summary>
		/// <param name="exception">Exception (and all its inner exceptions) to add</param>
		/// <param name="messageBuilder">StringBuilder that will hold the full exception message</param>
		/// <param name="level">Indentation level</param>
		public static void WriteExceptionDetails(this Exception exception, StringBuilder messageBuilder, int level = 0)
		{
			if (messageBuilder == null) return;

			int nextLevel = level + 1;

			messageBuilder.AppendFormat("{0}Exception Type: '{1}'\r\n", Indent(level), exception.GetType().Name);
			messageBuilder.AppendFormat("{0}Exception Message: '{1}'\r\n", Indent(level), EnsureIndentation(exception.Message, level));
			messageBuilder.AppendFormat("{0}Stack Trace: '{1}'\r\n", Indent(level), EnsureIndentation(exception.StackTrace, level));
			messageBuilder.AppendFormat("{0}Source: '{1}'\r\n", Indent(level), EnsureIndentation(exception.Source, level));

			try
			{
				messageBuilder.AppendFormat("{0}Target Site: '{1}'\r\n", Indent(level), EnsureIndentation(exception.TargetSite, level));
			}
			catch (TypeLoadException)
			{
				// in some cases exceptions may originate from a proxy in full trust with types
				// that are not allowed in the sandbox.  In this case a TypeLoadException will be
				// thrown. We do not want to mask the original error if this gets thrown.
			}

			if (exception.Data.Count > 0)
			{
				messageBuilder.AppendLine(Indent(level) + "Additional Data:");
				foreach (string key in exception.Data.Keys)
				{
					WriteAdditionalExceptionData(level, messageBuilder, exception, key, nextLevel);
				}
			}

			if (exception.InnerException != null)
			{
				messageBuilder.AppendLine(Indent(level) + "------------------------------------------------------------");
				messageBuilder.AppendLine(Indent(level) + "Inner exception:");
				messageBuilder.AppendLine(Indent(level) + "------------------------------------------------------------");
				exception.InnerException.WriteExceptionDetails(messageBuilder, nextLevel);
			}
		}

		private static void WriteAdditionalExceptionData(int level, StringBuilder messageBuilder, Exception exception, string key, int nextLevel)
		{
			object value = exception.Data[key];
			if (value == null) return;

			var valueAsException = value as Exception;
			if (valueAsException != null)
			{
				messageBuilder.AppendFormat("{0}{1} is an exception. Exception Details:\r\n", Indent(nextLevel), key);
				valueAsException.WriteExceptionDetails(messageBuilder, nextLevel + 1);
			}
			else
			{
				messageBuilder.AppendFormat("{0}'{1}' : '{2}'\r\n", Indent(nextLevel), key, EnsureIndentation(value, level));
			}
		}

		private static string EnsureIndentation(object obj, int indentationLevel)
		{
			if (obj == null)
				return string.Empty;

			return obj.ToString().Replace("\n", "\n" + Indent(indentationLevel + 1));
		}

		private static string Indent(int indentationLevel)
		{
			var builder = new StringBuilder();
			for (int i = 0; i < indentationLevel; i++)
			{
				builder.Append("\t");
			}
			return builder.ToString();
		}
	}
}
