using System;

namespace Chuchuka.SharePoint.Utilities.Utilities.CAML
{
	public class CAMLOrderBy
	{
		public string Expression { get; private set; }

		private CAMLOrderBy()
		{
		}

		public static CAMLOrderBy OrderBy(Guid fieldId, bool desc = false)
		{
			var expression = new CAMLOrderBy();
			expression.Expression = string.Format("<FieldRef ID=\"{0}\" {1} />", fieldId, desc ? "Ascending=\"False\"" : "");
			return expression;
		}

		public static CAMLOrderBy OrderBy(string fieldName, bool desc = false)
		{
			var expression = new CAMLOrderBy();
			expression.Expression = string.Format("<FieldRef Name=\"{0}\" {1} />", fieldName, desc ? "Ascending=\"False\"" : "");
			return expression;
		}
	}
}
