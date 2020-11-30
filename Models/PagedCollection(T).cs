using Microsoft.AspNetCore.Routing;
using System.Text.Json.Serialization;

namespace DemoApi.Models
{
	public class PagedCollection<T> : Collection<T>
	{				  
		public static PagedCollection<T> Create(Link self, T[] items, int size, PagingOptions pagingOptions)
        {
			return new PagedCollection<T>
			{
				Self = self,
				Value = items,
				Size = size,
				Limit = pagingOptions.Limit,
				Offset = pagingOptions.Offset,
				First = self,
				Next = GetNextLink(self, size, pagingOptions),
				Previous = GetPreviousLink(self, size, pagingOptions),
				Last = GetLastLink(self, size, pagingOptions)
			};
        }

		public static TResponse Create<TResponse>(Link self, T[] items, int size, PagingOptions pagingOptions) 
			where TResponse : PagedCollection<T>, new()
        {
			return new TResponse
			{
				Self = self,
				Value = items,
				Size = size,
				Limit = pagingOptions.Limit,
				Offset = pagingOptions.Offset,
				First = self,
				Next = GetNextLink(self, size, pagingOptions),
				Previous = GetPreviousLink(self, size, pagingOptions),
				Last = GetLastLink(self, size, pagingOptions)
			};
        }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public int? Offset { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public int? Limit { get; set; }

		public int Size { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public Link First { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public Link Previous { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public Link Next { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public Link Last { get; set; }

		static Link GetNextLink(Link self, int size, PagingOptions pagingOptions)
		{
			if (pagingOptions?.Limit == null)
			{
				return null;
			}
			if (pagingOptions?.Offset == null)
			{
				return null;
			}

			var limit = pagingOptions.Limit.Value;
			var offset = pagingOptions.Offset.Value;

			var nextPage = offset + limit;
			if (nextPage >= size)
			{
				return null;
			}

			var parameters = new RouteValueDictionary(self.RouteValues)
			{
				["limit"] = limit,
				["offset"] = nextPage
			};

			var newLink = Link.ToCollection(self.RouteName, parameters);
			return newLink;
		}

		static Link GetPreviousLink(Link self, int size, PagingOptions pagingOptions)
		{
			if (pagingOptions?.Limit == null)
			{
				return null;
			}
			if (pagingOptions?.Offset == null)
			{
				return null;
			}

			var limit = pagingOptions.Limit.Value;
			var offset = pagingOptions.Offset.Value;

			var nextPage = offset - limit;
			if (nextPage <= 0)
			{
				return null;
			}

			var parameters = new RouteValueDictionary(self.RouteValues)
			{
				["limit"] = limit,
				["offset"] = nextPage
			};

			var newLink = Link.ToCollection(self.RouteName, parameters);
			return newLink;
		}

		static Link GetLastLink(Link self, int size, PagingOptions pagingOptions)
		{
			if (pagingOptions?.Limit == null)
			{
				return null;
			}
			if (pagingOptions?.Offset == null)
			{
				return null;
			}

			var limit = pagingOptions.Limit.Value;
			var offset = pagingOptions.Offset.Value;

			// For the last, that's fine (the number of applications).
			var lastPage = size - limit;
			if (lastPage <= 0)
			{
				return null;
			}

			var parameters = new RouteValueDictionary(self.RouteValues)
			{
				["limit"] = limit,
				["offset"] = lastPage
			};

			var newLink = Link.ToCollection(self.RouteName, parameters);
			return newLink;
		}
	}
}