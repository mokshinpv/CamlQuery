using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace Mokshin.CamlQuery
{
	#region Interfaces

	public interface ICamlQueryPart
	{
	}

	public interface ICamlWhereClause
	{
	}

	#endregion

	/// <summary>
	/// Class to simplify work with SPQuery. Allows to write caml queries in "linq to xml" way
	/// </summary>
	public class CamlQuery
	{
		#region Fields

		private readonly ICamlWhereClause m_clause;
		private readonly CamlOrderBy m_orderBy;
		private readonly CamlViewFields m_viewFields;
		private CamlScope? m_scope;
		private bool m_viewFieldsOnly;
		private int m_rowLimit;

		#endregion

		#region Constructors

		public CamlQuery(ICamlWhereClause clause, CamlOrderBy orderBy = null, CamlViewFields viewFields = null)
		{
			m_clause = clause;
			m_orderBy = orderBy;
			m_viewFields = viewFields;
		}

		public CamlQuery(ICamlWhereClause clause, CamlViewFields viewFields)
		{
			m_clause = clause;
			m_viewFields = viewFields;
		}

		public CamlQuery(CamlOrderBy orderBy, CamlViewFields viewFields = null)
		{
			m_orderBy = orderBy;
			m_viewFields = viewFields;
		}

		public CamlQuery(CamlViewFields viewFields)
		{
			m_viewFields = viewFields;
		}

		#endregion

		#region Not static methods

		/// <summary>
		/// Returns SPQuery object
		/// </summary>
		/// <returns></returns>
		public SPQuery GetSPQuery()
		{
			return new SPQuery
			{
				Query = GetQuery(),
				ViewFields = m_viewFields != null ? m_viewFields.ToString() : null,
				ViewAttributes = m_scope != null ? string.Format("Scope=\"{0}\"", Enum.GetName(typeof(CamlScope), m_scope)) : null,
				ViewFieldsOnly = m_viewFieldsOnly,
				RowLimit = (uint)m_rowLimit
			};
		}

		/// <summary>
		/// Returns only Query part of SPQuery object
		/// </summary>
		/// <returns></returns>
		public string GetQuery()
		{
			return (m_clause != null && m_orderBy != null)
						? string.Format("<Where>{0}</Where>{1}", m_clause.ToString(), m_orderBy.ToString())
						: (m_clause != null ? string.Format("<Where>{0}</Where>", m_clause.ToString()) : m_orderBy.ToString());
		}

		/// <summary>
		/// Sets view attribute scope for this CamlQuery
		/// </summary>
		/// <param name="scope">Must be constant from CamlScope static class</param>
		/// <returns></returns>
		public CamlQuery Scope(CamlScope scope)
		{
			m_scope = scope;
			return this;
		}

		public CamlQuery ViewFieldsOnly(bool value)
		{
			m_viewFieldsOnly = value;
			return this;
		}

		public CamlQuery RowLimit(int value)
		{
			m_rowLimit = value;
			return this;
		}

		#endregion

		#region Static methods

		public static string EscapeValue(string value, CamlFieldType fieldType)
		{
			return new[] { CamlFieldType.Choice, CamlFieldType.Text, CamlFieldType.Note }.Contains(fieldType)
				? string.Format("<![CDATA[{0}]]>", value.Replace("]]>", "]]]]><![CDATA[>"))
				: value;
		}

		public static CamlOrderBy OrderBy(string fieldName, bool ascending = true)
		{
			return new CamlOrderBy(fieldName, ascending);
		}

		public static CamlOrderBy OrderBy(IEnumerable<CamlFieldRef> fields)
		{
			return new CamlOrderBy(fields);
		}

		public static CamlOrderBy OrderBy(params CamlFieldRef[] fields)
		{
			return new CamlOrderBy(fields);
		}

		public static CamlViewFields ViewFields(IEnumerable<CamlFieldRef> fields)
		{
			return new CamlViewFields(fields);
		}

		public static CamlViewFields ViewFields(params CamlFieldRef[] fields)
		{
			return new CamlViewFields(fields);
		}

		public static CamlBeginsWith BeginsWith(string fieldName, CamlFieldType valueType, object value)
		{
			return new CamlBeginsWith(fieldName, valueType, value.ToString());
		}

		public static CamlBeginsWith BeginsWith(string fieldName, string value)
		{
			return new CamlBeginsWith(fieldName, CamlFieldType.Text, value);
		}

		public static CamlContains Contains(string fieldName, CamlFieldType valueType, object value)
		{
			return new CamlContains(fieldName, valueType, value.ToString());
		}

		public static CamlContains Contains(string fieldName, string value)
		{
			return new CamlContains(fieldName, CamlFieldType.Text, value);
		}

		public static CamlIsNull IsNull(string fieldName)
		{
			return new CamlIsNull(fieldName);
		}

		public static CamlIsNotNull IsNotNull(string fieldName)
		{
			return new CamlIsNotNull(fieldName);
		}

		public static CamlEq Eq(string fieldName, CamlFieldType valueType, object value, bool useLookupID = false, bool includeTimeValue = false)
		{
			return new CamlEq(fieldName, valueType, value.ToString(), useLookupID, includeTimeValue);
		}

		public static CamlEq Eq(string fieldName, bool value)
		{
			return new CamlEq(fieldName, CamlFieldType.Boolean, value ? "1" : "0");
		}

		public static CamlEq Eq(string fieldName, int value)
		{
			return new CamlEq(fieldName, CamlFieldType.Integer, value.ToString(CultureInfo.InvariantCulture));
		}

		public static CamlEq Eq(string fieldName, double value)
		{
			return new CamlEq(fieldName, CamlFieldType.Number, value.ToString(CultureInfo.InvariantCulture));
		}

		public static CamlEq Eq(string fieldName, string value)
		{
			return new CamlEq(fieldName, CamlFieldType.Text, value);
		}

		public static CamlEq Eq(string fieldName, DateTime value, bool includeTimeValue = false)
		{
			return new CamlEq(fieldName, CamlFieldType.DateTime, SPUtility.CreateISO8601DateTimeFromSystemDateTime(value), false, includeTimeValue);
		}

		public static CamlNeq Neq(string fieldName, CamlFieldType valueType, object value, bool useLookupID = false, bool includeTimeValue = false)
		{
			return new CamlNeq(fieldName, valueType, value.ToString(), useLookupID, includeTimeValue);
		}

		public static CamlNeq Neq(string fieldName, DateTime value, bool includeTimeValue = false)
		{
			return new CamlNeq(fieldName, CamlFieldType.DateTime, SPUtility.CreateISO8601DateTimeFromSystemDateTime(value), false, includeTimeValue);
		}

		public static CamlNeq Neq(string fieldName, bool value)
		{
			return new CamlNeq(fieldName, CamlFieldType.Boolean, value ? "1" : "0");
		}

		public static CamlNeq Neq(string fieldName, int value)
		{
			return new CamlNeq(fieldName, CamlFieldType.Integer, value.ToString(CultureInfo.InvariantCulture));
		}

		public static CamlNeq Neq(string fieldName, double value)
		{
			return new CamlNeq(fieldName, CamlFieldType.Number, value.ToString(CultureInfo.InvariantCulture));
		}

		public static CamlNeq Neq(string fieldName, string value)
		{
			return new CamlNeq(fieldName, CamlFieldType.Text, value);
		}

		public static CamlLt Lt(string fieldName, CamlFieldType valueType, object value, bool includeTimeValue = false)
		{
			return new CamlLt(fieldName, valueType, value.ToString(), includeTimeValue);
		}

		public static CamlLt Lt(string fieldName, DateTime value, bool includeTimeValue = false)
		{
			return new CamlLt(fieldName, CamlFieldType.DateTime, SPUtility.CreateISO8601DateTimeFromSystemDateTime(value), includeTimeValue);
		}

		public static CamlLt Lt(string fieldName, int value)
		{
			return new CamlLt(fieldName, CamlFieldType.Integer, value.ToString(CultureInfo.InvariantCulture));
		}

		public static CamlLt Lt(string fieldName, double value)
		{
			return new CamlLt(fieldName, CamlFieldType.Number, value.ToString(CultureInfo.InvariantCulture));
		}

		public static CamlGt Gt(string fieldName, CamlFieldType valueType, object value, bool includeTimeValue = false)
		{
			return new CamlGt(fieldName, valueType, value.ToString(), includeTimeValue);
		}

		public static CamlGt Gt(string fieldName, DateTime value, bool includeTimeValue = false)
		{
			return new CamlGt(fieldName, CamlFieldType.DateTime, SPUtility.CreateISO8601DateTimeFromSystemDateTime(value), includeTimeValue);
		}

		public static CamlGt Gt(string fieldName, int value)
		{
			return new CamlGt(fieldName, CamlFieldType.Integer, value.ToString(CultureInfo.InvariantCulture));
		}

		public static CamlGt Gt(string fieldName, double value)
		{
			return new CamlGt(fieldName, CamlFieldType.Number, value.ToString(CultureInfo.InvariantCulture));
		}

		public static CamlLeq Leq(string fieldName, CamlFieldType valueType, object value, bool includeTimeValue = false)
		{
			return new CamlLeq(fieldName, valueType, value.ToString(), includeTimeValue);
		}

		public static CamlLeq Leq(string fieldName, DateTime value, bool includeTimeValue = false)
		{
			return new CamlLeq(fieldName, CamlFieldType.DateTime, SPUtility.CreateISO8601DateTimeFromSystemDateTime(value), includeTimeValue);
		}

		public static CamlLeq Leq(string fieldName, int value)
		{
			return new CamlLeq(fieldName, CamlFieldType.Integer, value.ToString(CultureInfo.InvariantCulture));
		}

		public static CamlLeq Leq(string fieldName, double value)
		{
			return new CamlLeq(fieldName, CamlFieldType.Number, value.ToString(CultureInfo.InvariantCulture));
		}

		public static CamlGeq Geq(string fieldName, CamlFieldType valueType, object value, bool includeTimeValue = false)
		{
			return new CamlGeq(fieldName, valueType, value.ToString(), includeTimeValue);
		}

		public static CamlGeq Geq(string fieldName, DateTime value, bool includeTimeValue = false)
		{
			return new CamlGeq(fieldName, CamlFieldType.DateTime, SPUtility.CreateISO8601DateTimeFromSystemDateTime(value), includeTimeValue);
		}

		public static CamlGeq Geq(string fieldName, int value)
		{
			return new CamlGeq(fieldName, CamlFieldType.Integer, value.ToString(CultureInfo.InvariantCulture));
		}

		public static CamlGeq Geq(string fieldName, double value)
		{
			return new CamlGeq(fieldName, CamlFieldType.Number, value.ToString(CultureInfo.InvariantCulture));
		}

		public static CamlAnd And(IEnumerable<ICamlWhereClause> clauses)
		{
			return new CamlAnd(clauses);
		}

		public static CamlAnd And(params ICamlWhereClause[] clauses)
		{
			return new CamlAnd(clauses);
		}

		public static CamlOr Or(IEnumerable<ICamlWhereClause> clauses)
		{
			return new CamlOr(clauses);
		}

		public static CamlOr Or(params ICamlWhereClause[] clauses)
		{
			return new CamlOr(clauses);
		}

		public static CamlIn In(string fieldName, CamlFieldType valueType, IEnumerable<object> values, bool useLookupID = false, bool includeTimeValue = false)
		{
			return new CamlIn(fieldName, valueType, values.Select(x => x.ToString()), useLookupID, includeTimeValue);
		}

		public static CamlIn In(string fieldName, IEnumerable<DateTime> values, bool includeTimeValue = false)
		{
			return new CamlIn(fieldName, CamlFieldType.DateTime, values.Select(SPUtility.CreateISO8601DateTimeFromSystemDateTime), false, includeTimeValue);
		}

		public static CamlIn In(string fieldName, IEnumerable<bool> values)
		{
			return new CamlIn(fieldName, CamlFieldType.Boolean, values.Select(x => x ? "1" : "0"));
		}

		public static CamlIn In(string fieldName, IEnumerable<int> values)
		{
			return new CamlIn(fieldName, CamlFieldType.Integer, values.Select(x => x.ToString(CultureInfo.InvariantCulture)));
		}

		public static CamlIn In(string fieldName, IEnumerable<double> values)
		{
			return new CamlIn(fieldName, CamlFieldType.Number, values.Select(x => x.ToString(CultureInfo.InvariantCulture)));
		}

		public static CamlIn In(string fieldName, IEnumerable<string> values)
		{
			return new CamlIn(fieldName, CamlFieldType.Text, values);
		}

		public static CamlIncludes Includes(string fieldName, CamlFieldType valueType, object value, bool useLookupID = false)
		{
			return new CamlIncludes(fieldName, valueType, value.ToString(), useLookupID);
		}

		public static CamlNotIncludes NotIncludes(string fieldName, CamlFieldType valueType, object value, bool useLookupID = false)
		{
			return new CamlNotIncludes(fieldName, valueType, value.ToString(), useLookupID);
		}

		public static CamlOr IsNullOrEmpty(string fieldName)
		{
			return Or(
				IsNull(fieldName),
				Eq(fieldName, string.Empty)
			);
		}

		public static CamlOr IsNullOrEmpty(string fieldName, CamlFieldType valueType)
		{
			return Or(
				IsNull(fieldName),
				Eq(fieldName, valueType, string.Empty)
			);
		}

		public static CamlAnd Between(string fieldName, CamlFieldType valueType, object lowerBound, object upperBound)
		{
			return And(
				Geq(fieldName, valueType, lowerBound),
				Leq(fieldName, valueType, upperBound)
			);
		}

		public static CamlAnd Between(string fieldName, int lowerBound, int upperBound)
		{
			return And(
				Geq(fieldName, lowerBound),
				Leq(fieldName, upperBound)
			);
		}

		public static CamlAnd Between(string fieldName, double lowerBound, double upperBound)
		{
			return And(
				Geq(fieldName, lowerBound),
				Leq(fieldName, upperBound)
			);
		}

		public static CamlAnd Between(string fieldName, DateTime lowerBound, DateTime upperBound, bool includeTimeValue = false)
		{
			return And(
				Geq(fieldName, lowerBound, includeTimeValue),
				Leq(fieldName, upperBound, includeTimeValue)
			);
		}

		//TODO create function NotIn

		#endregion
	}

	#region FieldRef

	public class CamlFieldRef : ICamlQueryPart
	{
		private readonly string m_fieldName;
		private readonly bool m_ascending;
		private Guid m_guid;

		public CamlFieldRef(string fieldName, bool ascending = true)
		{
			m_fieldName = fieldName;
			m_ascending = ascending;
		}

		public CamlFieldRef(Guid fieldID, bool ascending = true)
		{
			m_guid = fieldID;
			m_ascending = ascending;
		}

		public override string ToString()
		{
			return !string.IsNullOrEmpty(m_fieldName)
				? string.Format("<FieldRef Name='{0}'{1} />", m_fieldName, m_ascending ? string.Empty : " Ascending='FALSE'")
				: string.Format("<FieldRef ID='{0}'{1} />", m_guid.ToString("B"), m_ascending ? string.Empty : " Ascending='FALSE'");
		}

		public static implicit operator CamlFieldRef(string fieldName)
		{
			return new CamlFieldRef(fieldName, true);
		}

		public static implicit operator CamlFieldRef(Guid fieldID)
		{
			return new CamlFieldRef(fieldID, true);
		}
	}

	public class CamlViewFields : ICamlQueryPart
	{
		private readonly List<CamlFieldRef> m_fields;

		public CamlViewFields(IEnumerable<CamlFieldRef> fields)
		{
			m_fields = fields.ToList();
		}

		public CamlViewFields(params CamlFieldRef[] fields)
		{
			m_fields = fields.ToList();
		}

		public override string ToString()
		{
			return m_fields.Count > 0
				? string.Join(string.Empty, m_fields.Select(x => x.ToString()).ToArray())
				: string.Empty;
		}
	}

	#endregion

	#region Constants

	public enum CamlScope
	{
		Recursive = 0,
		RecursiveAll = 1,
		FilesOnly = 2
	}

	public enum CamlFieldType
	{
		Text = 0,
		Integer = 1,
		Choice = 2,
		Boolean = 3,
		Number = 4,
		DateTime = 5,
		User = 6,
		UserMulti = 7,
		Lookup = 8,
		LookupMulti = 9,
		Note = 10,
		Image = 11,
		Counter = 12,
		ModStat = 13
	}

	public static class CamlTag
	{
		public const string Gt = "Gt";
		public const string Lt = "Lt";
		public const string Geq = "Geq";
		public const string Leq = "Leq";
		public const string Eq = "Eq";
		public const string Neq = "Neq";
		public const string In = "In";
		public const string Contains = "Contains";
		public const string BeginsWith = "BeginsWith";
		public const string FieldRef = "FieldRef";
		public const string Where = "Where";
		public const string And = "And";
		public const string Or = "Or";
		public const string Value = "Value";
		public const string Values = "Values";
		public const string IsNull = "IsNull";
		public const string IsNotNull = "IsNotNull";
		public const string Includes = "Includes";
		public const string NotIncludes = "NotIncludes";
	}

	#endregion

	#region CompareOperators

	public abstract class CompareOperator : ICamlWhereClause
	{
		private readonly string m_tagName;
		private readonly string m_fieldName;
		private readonly CamlFieldType m_valueType;
		private readonly string m_value;
		private readonly bool m_useLookupID;
		private readonly bool m_includeTimeValue;

		protected CompareOperator(string tagName, string fieldName, CamlFieldType valueType, string value, bool useLookupID = false, bool includeTimeValue = false)
		{
			m_tagName = tagName;
			m_fieldName = fieldName;
			m_valueType = valueType;
			m_value = value;
			m_useLookupID = useLookupID;
			m_includeTimeValue = includeTimeValue;
		}

		public override string ToString()
		{
			return string.Format("<{0}><FieldRef Name='{1}' /><Value Type='{2}'{4}{5}>{3}</Value></{0}>",
				m_tagName, m_fieldName, Enum.GetName(typeof(CamlFieldType), m_valueType), CamlQuery.EscapeValue(m_value, m_valueType),
				m_useLookupID ? " LookupId='TRUE'" : string.Empty,
				m_includeTimeValue ? " IncludeTimeValue='TRUE'" : string.Empty);
		}
	}

	public class CamlGt : CompareOperator
	{
		public CamlGt(string fieldName, CamlFieldType valueType, string value, bool includeTimeValue = false)
			: base(CamlTag.Gt, fieldName, valueType, value, false, includeTimeValue)
		{
		}
	}

	public class CamlLt : CompareOperator
	{
		public CamlLt(string fieldName, CamlFieldType valueType, string value, bool includeTimeValue = false)
			: base(CamlTag.Lt, fieldName, valueType, value, false, includeTimeValue)
		{
		}
	}

	public class CamlEq : CompareOperator
	{
		public CamlEq(string fieldName, CamlFieldType valueType, string value, bool useLookupID = false, bool includeTimeValue = false)
			: base(CamlTag.Eq, fieldName, valueType, value, useLookupID, includeTimeValue)
		{
		}
	}

	public class CamlNeq : CompareOperator
	{
		public CamlNeq(string fieldName, CamlFieldType valueType, string value, bool useLookupID = false, bool includeTimeValue = false)
			: base(CamlTag.Neq, fieldName, valueType, value, useLookupID, includeTimeValue)
		{
		}
	}

	public class CamlLeq : CompareOperator
	{
		public CamlLeq(string fieldName, CamlFieldType valueType, string value, bool includeTimeValue = false)
			: base(CamlTag.Leq, fieldName, valueType, value, false, includeTimeValue)
		{
		}
	}

	public class CamlGeq : CompareOperator
	{
		public CamlGeq(string fieldName, CamlFieldType valueType, string value, bool includeTimeValue = false)
			: base(CamlTag.Geq, fieldName, valueType, value, false, includeTimeValue)
		{
		}
	}

	public class CamlContains : CompareOperator
	{
		public CamlContains(string fieldName, CamlFieldType valueType, string value)
			: base(CamlTag.Contains, fieldName, valueType, value)
		{
		}
	}

	public class CamlBeginsWith : CompareOperator
	{
		public CamlBeginsWith(string fieldName, CamlFieldType valueType, string value)
			: base(CamlTag.BeginsWith, fieldName, valueType, value)
		{
		}
	}

	public class CamlIncludes : CompareOperator
	{
		public CamlIncludes(string fieldName, CamlFieldType valueType, string value, bool useLookupID = false)
			: base(CamlTag.Includes, fieldName, valueType, value, useLookupID)
		{
		}
	}

	public class CamlNotIncludes : CompareOperator
	{
		public CamlNotIncludes(string fieldName, CamlFieldType valueType, string value, bool useLookupID = false)
			: base(CamlTag.NotIncludes, fieldName, valueType, value, useLookupID)
		{
		}
	}

	#endregion

	#region OrderBy

	public class CamlOrderBy : ICamlQueryPart
	{
		private readonly List<CamlFieldRef> m_fields;

		public CamlOrderBy(string fieldName, bool ascending)
		{
			m_fields = new List<CamlFieldRef> 
			{
				new CamlFieldRef(fieldName, ascending) 
			};
		}

		public CamlOrderBy(IEnumerable<CamlFieldRef> fields)
		{
			m_fields = fields.ToList();
		}

		public CamlOrderBy(params CamlFieldRef[] fields)
		{
			m_fields = fields.ToList();
		}

		public override string ToString()
		{
			return m_fields.Count > 0
				? string.Format("<OrderBy>{0}</OrderBy>", string.Join(string.Empty, m_fields.Select(x => x.ToString()).ToArray()))
				: string.Empty;
		}
	}

	#endregion

	#region CheckOperators

	public abstract class CheckOperator : ICamlWhereClause
	{
		private readonly string m_tagName;
		private readonly string m_fieldName;

		protected CheckOperator(string tagName, string fieldName)
		{
			m_tagName = tagName;
			m_fieldName = fieldName;
		}

		public override string ToString()
		{
			return string.Format("<{0}><FieldRef Name='{1}' /></{0}>", m_tagName, m_fieldName);
		}
	}

	public class CamlIsNull : CheckOperator
	{
		public CamlIsNull(string fieldName)
			: base(CamlTag.IsNull, fieldName)
		{
		}
	}

	public class CamlIsNotNull : CheckOperator
	{
		public CamlIsNotNull(string fieldName)
			: base(CamlTag.IsNotNull, fieldName)
		{
		}
	}

	#endregion

	#region LogicalOperators

	public abstract class LogicalOperator : ICamlWhereClause
	{
		private readonly string m_tagName;
		private readonly List<ICamlWhereClause> m_clauses;

		protected LogicalOperator(string tagName, IEnumerable<ICamlWhereClause> clauses)
		{
			m_tagName = tagName;
			m_clauses = clauses.ToList();
		}

		public LogicalOperator Add(ICamlWhereClause clause)
		{
			m_clauses.Add(clause);
			return this;
		}

		public LogicalOperator AddIf(ICamlWhereClause clause, bool shouldAdd)
		{
			if (shouldAdd)
			{
				m_clauses.Add(clause);
			}
			return this;
		}

		public override string ToString()
		{
			// Not use empty conditions
			var clauses = m_clauses.Where(x => !string.IsNullOrEmpty(x.ToString())).ToList();

			if (clauses.Count == 0)
			{
				return string.Empty;
			}

			var stringBuilder = new StringBuilder();

			if (clauses.Count > 1)
			{
				foreach (var clause in clauses)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Insert(0, string.Format("<{0}>", m_tagName));
						stringBuilder.Append(clause);
						stringBuilder.AppendFormat("</{0}>", m_tagName);
					}
					else
					{
						stringBuilder.Append(clause);
					}
				}
			}
			else
			{
				stringBuilder.Append(clauses[0]);
			}

			return stringBuilder.ToString();
		}
	}

	public class CamlAnd : LogicalOperator
	{
		public CamlAnd(IEnumerable<ICamlWhereClause> clauses)
			: base(CamlTag.And, clauses)
		{
		}
	}

	public class CamlOr : LogicalOperator
	{
		public CamlOr(IEnumerable<ICamlWhereClause> clauses)
			: base(CamlTag.Or, clauses)
		{
		}
	}

	#endregion

	#region In

	public class CamlIn : ICamlWhereClause
	{
		private readonly string m_fieldName;
		private readonly CamlFieldType m_valueType;
		private readonly List<string> m_values;
		private readonly bool m_useLookupID;
		private readonly bool m_includeTimeValue;

		public CamlIn(string fieldName, CamlFieldType valueType, IEnumerable<string> values, bool useLookupID = false, bool includeTimeValue = false)
		{
			m_fieldName = fieldName;
			m_valueType = valueType;
			m_values = values.ToList();
			m_useLookupID = useLookupID;
			m_includeTimeValue = includeTimeValue;
		}

		public override string ToString()
		{
			return m_values.Count > 0
				? string.Format("<In><FieldRef Name='{0}' /><Values>{1}</Values></In>", m_fieldName,
					string.Join(string.Empty, m_values.Select(x => string.Format("<Value Type='{0}'{2}{3}>{1}</Value>",
						Enum.GetName(typeof(CamlFieldType), m_valueType), CamlQuery.EscapeValue(x, m_valueType),
						m_useLookupID ? " LookupId='TRUE'" : string.Empty,
						m_includeTimeValue ? " IncludeTimeValue='TRUE'" : string.Empty)).ToArray()))
				: string.Empty;
		}
	}

	#endregion
}