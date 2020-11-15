using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DemoApi.Infrastructure;
using DemoApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;

namespace DemoApi.Filters
{
	public class LinkRewritingFilter : IAsyncResultFilter
	{
		private IUrlHelperFactory _urlHelperFactory;

		public LinkRewritingFilter(IUrlHelperFactory urlHelperFactory)
		{
			_urlHelperFactory = urlHelperFactory;
		}

		public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
		{
			var asObjectResult = context.Result as ObjectResult;
			bool skip = asObjectResult?.StatusCode >= 400
				|| asObjectResult?.Value == null
				|| asObjectResult?.Value as Resource == null;

			if (skip)
			{
				return next();
			}

			var rewriter = new LinkRewriter(_urlHelperFactory.GetUrlHelper(context));
			RewriteAllLinks(asObjectResult.Value, rewriter);
			return next();
		}

		private static void RewriteAllLinks(object resultModel, LinkRewriter rewriter)
		{
			// Need to take care of any nulls that might be sneaking through here.
			if (resultModel == null)
			{
				return;
			}

			var allProperties = resultModel.GetType().GetTypeInfo().GetAllProperties().Where(p => p.CanRead).ToArray();

			var linkProperties = allProperties.Where(p => p.CanWrite && p.PropertyType == typeof(Link));

			foreach (var lp in linkProperties)
			{
				var rewritten = rewriter.Rewrite(lp.GetValue(resultModel) as Link);
				if (rewritten == null)
				{
					continue;
				}

				lp.SetValue(resultModel, rewritten);

				// Special handling of the hidden Self property:
				// unwrap into the root property
				if (lp.Name == nameof(Resource.Self))
				{
					allProperties.SingleOrDefault(p => p.Name == nameof(Resource.Href))?.SetValue(resultModel, rewritten.Href);

					allProperties.SingleOrDefault(p => p.Name == nameof(Resource.Method))?.SetValue(resultModel, rewritten.Method);

					allProperties.SingleOrDefault(p => p.Name == nameof(Resource.Relations))?.SetValue(resultModel, rewritten.Relations);
				}
			}

			var arrayProperties = allProperties.Where(p => p.PropertyType.IsArray);
			RewriteLinksInArrays(arrayProperties, resultModel, rewriter);

			var objectProperties = allProperties.Except(linkProperties).Except(arrayProperties);
			RewriteLinksInNestedObjects(objectProperties, resultModel, rewriter);
		}

		private static void RewriteLinksInNestedObjects(IEnumerable<PropertyInfo> objectProperties, object resultModel, LinkRewriter rewriter)
		{
			foreach (var op in objectProperties)
			{
				if (op.PropertyType == typeof(string))
				{
					continue;
				}

				var typeInfo = op.PropertyType.GetTypeInfo();
				if (typeInfo.IsClass)
				{
					RewriteAllLinks(op.GetValue(resultModel), rewriter);
				}
			}
		}

		private static void RewriteLinksInArrays(IEnumerable<PropertyInfo> arrayProperties, object resultModel, LinkRewriter rewriter)
		{
			foreach (var ap in arrayProperties)
			{
				var array = ap.GetValue(resultModel) as Array ?? new Array[0];

				foreach (var element in array)
				{
					RewriteAllLinks(element, rewriter);
				}
			}
		}
	}
}