using System.ComponentModel.DataAnnotations;

namespace DemoApi.Models
{
	public class PagingOptions
	{
		[Range(0, 99999, ErrorMessage = "Offset must be greater than 0.")]
		public int? Offset { get; set; }

		[Range(1, 100, ErrorMessage = "Limit must be greater than 0 and less than 100.")]
		public int? Limit { get; set; }
	}
}