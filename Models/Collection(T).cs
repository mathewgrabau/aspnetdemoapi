namespace DemoApi.Models
{
	public class Collection<T> : Resource
	{
		public T[] Value { get; set; }
	}
}