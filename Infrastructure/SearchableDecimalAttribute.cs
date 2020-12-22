using System;

namespace DemoApi.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SearchableDecimalAttribute : SearchableAttribute
    {
        public SearchableDecimalAttribute()
        {
            // OVerride the provider
            ExpressionProvider = new DecimalToIntSearchExpressionProvider();
        }
    }
}
