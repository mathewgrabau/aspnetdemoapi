using DemoApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Infrastructure
{
	public class LinkRewriter
	{
		private readonly IUrlHelper _urlHelper;

		public LinkRewriter(IUrlHelper urlHelper)
		{
			_urlHelper = urlHelper;
		}

		public Link Rewrite(Link original)
		{
			if (original == null)
			{
				return null;
			}

			// Using the stashed values to make the Href property
			return new Link { Href = _urlHelper.Link(original.RouteName, original.RouteValues), Method = original.Method, Relations = original.Relations };
		}
	}
}