using System;

namespace DemoApi.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SearchableAttribute : Attribute
    {
        /// <summary>
        /// Each instance can include it's own impelmentation as desired.
        /// </summary>
        public ISearchExpressionProvider ExpressionProvider { get; set; }

        public SearchableAttribute()
        {
            ExpressionProvider = new DefaultSearchExpressionProvider();
        }
    }
}
