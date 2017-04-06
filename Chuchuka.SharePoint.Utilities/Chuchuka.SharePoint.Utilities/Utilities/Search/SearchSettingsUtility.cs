using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Office.Server.Search.Administration;
using Microsoft.Office.Server.Search.Administration.Query;
using Microsoft.SharePoint;

namespace Chuchuka.SharePoint.Utilities.Utilities.Search
{
	/// <summary>
	/// Utility class for SharePoint 2013 Search configuration
	/// </summary>
	public class SearchSettingsUtility
	{
		private readonly SearchObjectOwner _searchOwner;
		private readonly SearchServiceApplicationProxy _searchProxy;

		private SearchSettingsUtility(SPSite site)
		{
			_searchOwner = new SearchObjectOwner(SearchObjectLevel.SPSite, site.RootWeb);

			SPServiceContext context = SPServiceContext.GetContext(site);
			_searchProxy = context.GetDefaultProxy(typeof(SearchServiceApplicationProxy)) as SearchServiceApplicationProxy;

			if (_searchProxy == null) throw new ArgumentException("SearchServiceApplicationProxy not found for site collection {0}", site.Url);
		}

		public static SearchSettingsUtility Init(SPSite site)
		{
			var utility = new SearchSettingsUtility(site);
			return utility;
		}

		/// <summary>
		/// Get managed property by its name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ManagedPropertyInfo GetManagedProperty(string name)
		{
			return _searchProxy.GetManagedProperty(name, _searchOwner);
		}

		/// <summary>
		/// Ensure managed property and set its metadata
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public ManagedPropertyInfo EnsureManagedProperty(ManagedPropertyDTO property)
		{
			ManagedPropertyInfo managedProperty;
			try
			{
				managedProperty = _searchProxy.GetManagedProperty(property.Name, _searchOwner);
			}
			catch (ObjectNotFoundException)
			{
				managedProperty = _searchProxy.CreateManagedProperty(property.Name, property.Type, _searchOwner);
			}

			managedProperty.Searchable = property.Searchable;
			managedProperty.Retrievable = property.Retrievable;
			managedProperty.Refinable = property.Refinable;
			managedProperty.Queryable = property.Queryable;
			managedProperty.Sortable = property.Sortable;
			managedProperty.SafeForAnonymous = property.SafeForAnonymous;
			managedProperty.HasMultipleValues = property.HasMultipleValues;
			_searchProxy.UpdateManagedProperty(managedProperty, _searchOwner);

			return managedProperty;
		}

		/// <summary>
		/// Map a crawled property to a managed property
		/// </summary>
		/// <param name="managedPropertyName"></param>
		/// <param name="crawledPropertyName"></param>
		public void MapManagedProperty(string managedPropertyName, string crawledPropertyName)
		{
			ManagedPropertyInfo managedProperty = _searchProxy.GetManagedProperty(managedPropertyName, _searchOwner);
			IList<CrawledPropertyInfo> crawledProperties = _searchProxy
				.GetAllCrawledProperties(crawledPropertyName, "SharePoint", 0, _searchOwner)
				.Where(prop => prop.Name == crawledPropertyName)
				.ToList();

			var mappings = _searchProxy.GetManagedPropertyMappings(managedProperty, _searchOwner);

			foreach (CrawledPropertyInfo crawledProperty in crawledProperties)
			{
				if (mappings.Exists(mi => mi.CrawledPropertyName == crawledProperty.Name)) continue;

				var mapping = new MappingInfo
				{
					CrawledPropertyName = crawledProperty.Name,
					CrawledPropset = crawledProperty.Propset,
					ManagedPid = managedProperty.Pid
				};
				mappings.Add(mapping);
			}

			_searchProxy.SetManagedPropertyMappings(managedProperty, mappings, _searchOwner);
		}

		/// <summary>
		/// Ensure search Result Type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public ResultItemType EnsureResultType(ResultTypeDTO type)
		{
			var resultTypesManager = new ResultItemTypeManager(_searchProxy);
			var resultType = resultTypesManager.GetResultItemTypes(_searchOwner, true).FirstOrDefault(rt => rt.Name.Equals(type.Name, StringComparison.OrdinalIgnoreCase));
			if (resultType != null)
			{
				return UpdateResultType(resultType.ID, type);
			}

			resultType = new ResultItemType(_searchOwner)
			{
				Name = type.Name,
				DisplayTemplateUrl = type.DisplayTemplateUrl,
				RulePriority = type.Priority
			};

			if (!string.IsNullOrEmpty(type.ResultSource))
			{
				var source = GetResultsSource(type.ResultSource);
				resultType.SourceID = source.Id;
			}

			var rules = new PropertyRuleCollection();
			foreach (var builtInRule in type.BuiltInRules)
			{
				rules.Add(GetBuiltInPropertyRule(builtInRule));
			}
			foreach (var rule in type.CustomRules)
			{
				rules.Add(CreatePropertyRule(rule.ManagedPropertyName, rule.Operator, rule.Values));
			}
			resultType.Rules = rules;

			return resultTypesManager.AddResultItemType(resultType);
		}

		/// <summary>
		/// Update search Result Type with specified data
		/// </summary>
		/// <param name="resultTypeId">Existing Result Type Id</param>
		/// <param name="type">Data transfer object used for updating</param>
		/// <returns></returns>
		public ResultItemType UpdateResultType(int resultTypeId, ResultTypeDTO type)
		{
			var resultTypesManager = new ResultItemTypeManager(_searchProxy);
			var resultType = resultTypesManager.GetResultItemTypeByID(resultTypeId, _searchOwner);

			resultType.Name = type.Name;
			resultType.DisplayTemplateUrl = type.DisplayTemplateUrl;
			resultType.RulePriority = type.Priority;

			if (!string.IsNullOrEmpty(type.ResultSource))
			{
				var source = GetResultsSource(type.ResultSource);
				resultType.SourceID = source.Id;
			}

			var rules = new PropertyRuleCollection();
			foreach (var builtInRule in type.BuiltInRules)
			{
				rules.Add(GetBuiltInPropertyRule(builtInRule));
			}
			foreach (var rule in type.CustomRules)
			{
				rules.Add(CreatePropertyRule(rule.ManagedPropertyName, rule.Operator, rule.Values));
			}
			resultType.Rules = rules;

			return resultTypesManager.UpdateResultItemType(resultType);
		}

		/// <summary>
		/// Get search Results Source by name
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Source> GetResultsSources()
		{
			FederationManager fedManager = new FederationManager(_searchProxy);
			return fedManager.ListSources(new SearchObjectFilter(SearchObjectLevel.SPSite), false);
		}

		/// <summary>
		/// Get search Results Source by name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public Source GetResultsSource(string name)
		{
			FederationManager fedManager = new FederationManager(_searchProxy);
			return fedManager.GetSourceByName(name, _searchOwner);
		}

		/// <summary>
		/// Get built-in SharePoint rules for types of content
		/// </summary>
		/// <param name="typeOfContent">Name of the type of content</param>
		/// <returns></returns>
		private static PropertyRule GetBuiltInPropertyRule(string typeOfContent)
		{
			Type type = typeof(PropertyRule.MappedPropertyRules);
			FieldInfo info = type.GetField(typeOfContent, BindingFlags.NonPublic | BindingFlags.Static);
			object value = info.GetValue(null);
			return (PropertyRule)value;
		}

		/// <summary>
		/// Create a new property rule
		/// </summary>
		/// <param name="managedPropertyName">Name</param>
		/// <param name="operator"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		private static PropertyRule CreatePropertyRule(string managedPropertyName, PropertyRuleOperator.DefaultOperator @operator, string[] values)
		{
			Type type = typeof(PropertyRuleOperator);
			PropertyInfo info = type.GetProperty("DefaultOperators", BindingFlags.NonPublic | BindingFlags.Static);
			object value = info.GetValue(null);
			var defaultOperators = (Dictionary<PropertyRuleOperator.DefaultOperator, PropertyRuleOperator>)value;
			PropertyRule rule = new PropertyRule(managedPropertyName, defaultOperators[@operator]) { PropertyValues = new List<string>(values) };
			return rule;
		}
	}
}
