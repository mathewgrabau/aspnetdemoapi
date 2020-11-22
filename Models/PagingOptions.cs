using System.ComponentModel.DataAnnotations;

namespace DemoApi.Models
{
	public class PagingOptions
	{
		[Range(0, 99999, ErrorMessage = "Offset must be greater than 0.")]
		public int? Offset { get; set; }

		[Range(1, 100, ErrorMessage = "Limit must be greater than 0 and less than 100.")]
		public int? Limit { get; set; }

		public PagingOptions Replace(PagingOptions options)
		{
			return new PagingOptions
			{
				Offset = options.Offset ?? this.Offset,
				Limit = options.Limit ?? this.Limit
			};
        }
	}
}